using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using ExpressBase.Common.Structures;
using System.Collections.Generic;
using ExpressBase.Common.Extensions;
using ExpressBase.Common;
using ExpressBase.Common.Data;
using ExpressBase.Objects.ServiceStack_Artifacts;
using ExpressBase.Objects.Objects;
using System;
using System.Data.Common;
using ServiceStack;
using ServiceStack.Redis;
using ExpressBase.Security;
using ExpressBase.Common.Constants;
using System.Linq;
using ExpressBase.Objects.Helpers;
using ExpressBase.Common.Helpers;
using ServiceStack.RabbitMq;
using System.Net;
using ExpressBase.Common.Messaging;
using ExpressBase.Objects.WebFormRelated;
using ExpressBase.Common.ServiceClients;
using System.IO;
using System.Security.Cryptography;
using System.Xml.Serialization;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Encodings;
using Org.BouncyCastle.Crypto.Engines;
using System.Text;
using Org.BouncyCastle.Crypto.Parameters;
using FluentFTP;
using DocumentFormat.OpenXml.Packaging;
using Microsoft.AspNetCore.Http;
using DocumentFormat.OpenXml.Spreadsheet;
using ExpressBase.Common.Excel;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using System.Dynamic;
using DocumentFormat.OpenXml.Presentation;

namespace ExpressBase.Objects
{
    public class ListOrdered : List<ApiResources>
    {
        public ListOrdered()
        {
            this.Sort((x, y) => x.RouteIndex.CompareTo(y.RouteIndex));
        }

        public int GetIndex(string name)
        {
            int index = 0;
            foreach (ApiResources resource in this)
            {
                if (resource.Name == name)
                    index = this.IndexOf(resource);
            }
            return index;
        }
    }

    public class ApiParams
    {
        public List<Param> Default { set; get; }

        public List<Param> Custom { set; get; }

        public ApiParams()
        {
            Default = new List<Param>();
            Custom = new List<Param>();
        }

        public Param GetParam(string name)
        {
            Param p = Default.Find(item => item.Name == name);

            if (p == null)
            {
                p = Custom.Find(item => item.Name == name);
            }

            return p;
        }
    }

    public abstract class EbApiWrapper : EbObject
    {

    }

    [EnableInBuilder(BuilderType.ApiBuilder)]
    [BuilderTypeEnum(BuilderType.ApiBuilder)]
    public class EbApi : EbApiWrapper, IEBRootObject
    {
        public bool HideInMenu { get; set; }

        [EnableInBuilder(BuilderType.ApiBuilder)]
        [HideInPropertyGrid]
        public override string RefId { get; set; }

        [EnableInBuilder(BuilderType.ApiBuilder)]
        public override string DisplayName { get; set; }

        [EnableInBuilder(BuilderType.ApiBuilder)]
        public override string Description { get; set; }

        [EnableInBuilder(BuilderType.ApiBuilder)]
        [HideInPropertyGrid]
        public override string VersionNumber { get; set; }

        [EnableInBuilder(BuilderType.ApiBuilder)]
        [HideInPropertyGrid]
        public override string Status { get; set; }

        [EnableInBuilder(BuilderType.ApiBuilder)]
        [HideInPropertyGrid]
        public ListOrdered Resources { set; get; }

        [EnableInBuilder(BuilderType.ApiBuilder)]
        [HideInPropertyGrid]
        public ApiParams Request { set; get; }

        [EnableInBuilder(BuilderType.ApiBuilder)]
        public ApiMethods Method { set; get; }

        public ApiResponse ApiResponse { set; get; }

        public new IRedisClient Redis { get; set; }

        public EbStaticFileClient FileClient { get; set; }

        public RabbitMqProducer MessageProducer { get; set; }

        public Service Service { get; set; }

        public int Step = 0;

        public override List<string> DiscoverRelatedRefids()
        {
            List<string> refids = new List<string>();

            foreach (ApiResources resource in this.Resources)
            {
                if (!string.IsNullOrEmpty(resource.Reference))
                {
                    refids.Add(resource.Reference);
                }
            }
            return refids;
        }

        public override void ReplaceRefid(Dictionary<string, string> RefidMap)
        {
            foreach (ApiResources resource in this.Resources)
            {
                if (!string.IsNullOrEmpty(resource.Reference))
                {
                    if (RefidMap.ContainsKey(resource.Reference))
                    {
                        resource.Reference = RefidMap[resource.Reference];
                    }
                }
            }
        }

        public User UserObject { set; get; }

        public string SolutionId { set; get; }

        public Dictionary<string, object> GlobalParams { set; get; }

        public IDatabase ObjectsDB { get; set; }

        public IDatabase DataDB { get; set; }

        public int LogMasterId { get; set; }

        public T GetEbObject<T>(string refId, IRedisClient Redis, IDatabase ObjectsDB)
        {

            T ebObject = EbApiHelper.GetEbObject<T>(refId, Redis, ObjectsDB);

            if (ebObject == null)
            {
                string message = $"{typeof(T).Name} not found";
                this.ApiResponse.Message.Description = message;

                throw new ApiException(message);
            }
            return ebObject;
        }

        public T GetEbObject<T>(int ObjId, IRedisClient Redis, IDatabase ObjectsDB)
        {

            T ebObject = EbApiHelper.GetEbObject<T>(ObjId, Redis, ObjectsDB);

            if (ebObject == null)
            {
                string message = $"{typeof(T).Name} not found";
                this.ApiResponse.Message.Description = message;

                throw new ApiException(message);
            }
            return ebObject;
        }

        public void FillParams(List<Param> inputParam)
        {
            try
            {
                foreach (Param p in inputParam)
                {
                    object value = this.GetParameterValue(p.Name);

                    if (IsRequired(p.Name))
                    {
                        if (value == null || string.IsNullOrEmpty(value.ToString()))
                        {
                            this.ApiResponse.Message.ErrorCode = ApiErrorCode.ParameterNotFound;
                            this.ApiResponse.Message.Status = $"Parameter Error";

                            throw new ApiException($"Parameter '{p.Name}' must be set");
                        }
                        else p.Value = value.ToString();
                    }
                    else
                    {
                        if (value == null || string.IsNullOrEmpty(value.ToString()))
                            p.Value = null;
                        else
                            p.Value = value.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ApiException("[Params], " + ex.Message);
            }
        }

        private object GetParameterValue(string name)
        {
            if (this.GlobalParams.ContainsKey(name))
            {
                return this.GlobalParams[name];
            }
            return null;
        }

        private bool IsRequired(string name)
        {
            if (this.Request == null)
                return true;

            Param p = this.Request.GetParam(name);

            return p == null || p.Required;
        }

        [MetaOnly]
        //[HideInPropertyGrid]
        [EnableInBuilder(BuilderType.ApiBuilder)]
        public ColumnColletion FirstReaderKeyColumnsColl { get; set; }

        private List<string> _firstReaderKeyColumns = null;
        public List<string> FirstReaderKeyColumns

        {
            get
            {
                if (_firstReaderKeyColumns == null)
                {
                    _firstReaderKeyColumns = new List<string>();
                    foreach (EbDataColumn c in FirstReaderKeyColumnsTemp)
                        if (c != null)
                            _firstReaderKeyColumns.Add(c.ColumnName);
                }
                return _firstReaderKeyColumns;
            }
        }

        [EnableInBuilder(BuilderType.ApiBuilder)]
        [PropertyEditor(PropertyEditorType.CollectionFrmSrc, "FirstReaderKeyColumnsColl")]
        [Alias("First Reader Key Column")]
        public ColumnColletion FirstReaderKeyColumnsTemp { get; set; }

        [MetaOnly]
        //[HideInPropertyGrid]
        [EnableInBuilder(BuilderType.ApiBuilder)]
        public List<Param> ParameterKeyColumnsColl { get; set; }


        private List<string> _parameterKeyColumns = null;
        public List<string> ParameterKeyColumns
        {
            get
            {
                if (_parameterKeyColumns == null)
                {
                    _parameterKeyColumns = new List<string>();
                    foreach (Param _p in ParameterKeyColumnsTemp)
                    {
                        if (_p != null)
                            _parameterKeyColumns.Add(_p.Name);
                    }
                }
                return _parameterKeyColumns;
            }
        }

        [EnableInBuilder(BuilderType.ApiBuilder)]
        [Alias("Parameter Key Column")]
        [PropertyEditor(PropertyEditorType.CollectionFrmSrc, "ParameterKeyColumnsColl")]

        public List<Param> ParameterKeyColumnsTemp { get; set; }



        public EbApi GetApi(string RefId, int ObjId, IRedisClient Redis, IDatabase ObjectsDB, IDatabase DataDB)
        {
            EbApi Api = null;

            if (ObjId > 0)
            {
                Api = GetEbObject<EbApi>(ObjId, Redis, ObjectsDB);
            }
            else if (RefId != string.Empty)
            {
                Api = GetEbObject<EbApi>(RefId, Redis, ObjectsDB);
            }
            if (Api != null)
            {
                Api.Redis = Redis;
                Api.ObjectsDB = ObjectsDB;
                Api.DataDB = DataDB;
                Api.ApiResponse = new ApiResponse();
            }
            return Api;
        }

        public void InsertLog()
        {
            string query = @" 
                        INSERT INTO 
                            eb_api_logs_master(refid, type, params, status, message, result, eb_created_by, eb_created_at) 
                        VALUES
                            (:refid, :type, :params, :status, :message, :result, :eb_created_by, NOW()) 
                        RETURNING id;";
            DbParameter[] parameters = new DbParameter[] {
                            DataDB.GetNewParameter("refid", EbDbTypes.String, this.RefId) ,
                            DataDB.GetNewParameter("type", EbDbTypes.Int32, 1),
                            DataDB.GetNewParameter("message", EbDbTypes.String, this.ApiResponse.Message.Description),
                            DataDB.GetNewParameter("status", EbDbTypes.String, this.ApiResponse.Message.Status),
                            DataDB.GetNewParameter("params", EbDbTypes.Json, JsonConvert.SerializeObject(this.GlobalParams)),
                            DataDB.GetNewParameter("result", EbDbTypes.Json, JsonConvert.SerializeObject(this.ApiResponse.Result)),
                            DataDB.GetNewParameter("eb_created_by", EbDbTypes.Int32, this.UserObject.UserId)
                        };
            EbDataTable dt = DataDB.DoQuery(query, parameters);

            //this.LogMasterId = Convert.ToInt32(dt?.Rows[0][0]);
        }

        public int InsertLinesLog(string status, string source)
        {
            string query = @"
                        INSERT INTO
                            eb_api_logs_lines(eb_api_logs_master_id, source, status, , eb_created_by, eb_created_at)
                        VALUES
                            (:eb_api_logs_master_id, :source, :status, , :eb_created_by, NOW())
                        RETURNING id;";
            DbParameter[] parameters = new DbParameter[]
            {
                DataDB.GetNewParameter(":eb_api_logs_master_id", EbDbTypes.Int32, this.LogMasterId),
                DataDB.GetNewParameter("source", EbDbTypes.Int32, source),
                DataDB.GetNewParameter("status", EbDbTypes.String, status),
                DataDB.GetNewParameter("eb_created_by", EbDbTypes.Int32, this.UserObject.UserId),
            };
            EbDataTable dt = DataDB.DoQuery(query, parameters);

            return Convert.ToInt32(dt?.Rows[0][0]);
        }

        public void UpdateLog()
        {
            DbParameter[] _dbparameters = new DbParameter[]
                       {
                            DataDB.GetNewParameter("id", EbDbTypes.Int32, LogMasterId),
                            DataDB.GetNewParameter("refid", EbDbTypes.String, this.RefId),
                            DataDB.GetNewParameter("message", EbDbTypes.String, this.ApiResponse.Message.Description),
                            DataDB.GetNewParameter("status", EbDbTypes.String, this.ApiResponse.Message.Status),
                            DataDB.GetNewParameter("params", EbDbTypes.Json, JsonConvert.SerializeObject(this.GlobalParams)),
                            DataDB.GetNewParameter("result", EbDbTypes.Json, JsonConvert.SerializeObject(this.ApiResponse.Result))
                       };

            this.DataDB.DoNonQuery("UPDATE eb_api_logs_master SET refid = :refid, params = :params, status = :status, message = :message, result =:result WHERE id = :id;", _dbparameters);

        }

        private Dictionary<string, TV> _keyValuePairs = null;

        public Dictionary<string, TV> GetKeyvalueDict
        {
            get
            {
                if (_keyValuePairs == null)
                {
                    _keyValuePairs = new Dictionary<string, TV>();
                    foreach (string key in this.FirstReaderKeyColumns)
                    {
                        if (!_keyValuePairs.ContainsKey(key))
                            _keyValuePairs.Add(key, new TV { });
                    }
                    foreach (string key in this.ParameterKeyColumns)
                    {
                        if (!_keyValuePairs.ContainsKey(key))
                            _keyValuePairs.Add(key, new TV { });
                    }
                }
                return _keyValuePairs;
            }
        }

        public string FillKeys(EbDataRow dataRow)
        {
            foreach (var item in this.GlobalParams)
                if (GetKeyvalueDict.ContainsKey(item.Key))
                    this.GetKeyvalueDict[item.Key] = null;// this.Api.GlobalParams[item.Key];
            for (int i = 0; i < dataRow.Count; i++)
            {
                if (GetKeyvalueDict.ContainsKey(dataRow.Table.Columns[i].ColumnName))
                    this.GetKeyvalueDict[dataRow.Table.Columns[i].ColumnName] = new TV { Value = dataRow[i].ToString(), Type = ((int)dataRow.Table.Columns[i].Type).ToString() };
            }
            return JsonConvert.SerializeObject(GetKeyvalueDict); ;
        }

        public EbApi()
        {
            ParameterKeyColumnsColl = new List<Param>();

            ParameterKeyColumnsTemp = new List<Param>();

            FirstReaderKeyColumnsTemp = new ColumnColletion();

            FirstReaderKeyColumnsColl = new ColumnColletion();
        }
    }

    public abstract class ApiResources : EbApiWrapper
    {
        [EnableInBuilder(BuilderType.ApiBuilder)]
        [HideInPropertyGrid]
        public int RouteIndex { set; get; }

        [EnableInBuilder(BuilderType.ApiBuilder)]
        [UIproperty]
        [MetaOnly]
        public string Label { set; get; }

        public virtual string Reference { set; get; }

        public object Result { set; get; }

        public virtual object GetResult() { return this.Result; }

        public virtual List<Param> GetParameters(Dictionary<string, object> requestParams) { return null; }

        public virtual List<Param> GetOutParams(List<Param> _param, int step) { return new List<Param>(); }
    }

    [EnableInBuilder(BuilderType.ApiBuilder)]
    public class EbSqlReader : ApiResources
    {
        [EnableInBuilder(BuilderType.ApiBuilder)]
        [PropertyEditor(PropertyEditorType.ObjectSelector)]
        [PropertyGroup("Data Settings")]
        [OSE_ObjectTypes(EbObjectTypes.iDataReader)]
        public override string Reference { get; set; }

        [EnableInBuilder(BuilderType.ApiBuilder)]
        [MetaOnly]
        [UIproperty]
        public string RefName { set; get; }

        [EnableInBuilder(BuilderType.ApiBuilder)]
        [MetaOnly]
        [UIproperty]
        public string Version { set; get; }

        [EnableInBuilder(BuilderType.ApiBuilder)]
        [PropertyGroup("Data Settings")]
        public DataReaderResult ResultType { set; get; }

        public override string GetDesignHtml()
        {
            return @"<div class='apiPrcItem dropped' eb-type='SqlReader' id='@id'>
                        <div tabindex='1' class='drpbox' onclick='$(this).focus();'>  
                            <div class='CompLabel'> @Label </div>
                            <div class='CompName'> @RefName </div>
                            <div class='CompVersion'> @Version </div>
                        </div>
                     </div>".RemoveCR().DoubleQuoted(); ;
        }

        public override object GetResult()
        {
            if (this.ResultType == DataReaderResult.Formated)
            {
                JsonTableSet table = new JsonTableSet();
                foreach (EbDataTable t in (this.Result as EbDataSet).Tables)
                {
                    JsonTable jt = new JsonTable { TableName = t.TableName };
                    for (int k = 0; k < t.Rows.Count; k++)
                    {
                        JsonColVal d = new JsonColVal();
                        for (int i = 0; i < t.Columns.Count; i++)
                        {
                            d.Add(t.Columns[i].ColumnName, t.Rows[k][t.Columns[i].ColumnIndex]);
                        }
                        jt.Rows.Add(d);
                    }
                    table.Tables.Add(jt);
                }
                return table;
            }
            else
                return this.Result;
        }

        public object ExecuteDataReader(EbApi Api)
        {
            EbDataSet dataSet;
            try
            {
                EbDataReader dataReader = Api.GetEbObject<EbDataReader>(this.Reference, Api.Redis, Api.ObjectsDB);

                List<DbParameter> dbParameters = new List<DbParameter>();

                List<Param> InputParams = dataReader.GetParams((RedisClient)Api.Redis);

                Api.FillParams(InputParams);

                foreach (Param param in InputParams)
                {
                    dbParameters.Add(Api.DataDB.GetNewParameter(param.Name, (EbDbTypes)Convert.ToInt32(param.Type), param.ValueTo));
                }

                dataSet = Api.DataDB.DoQueries(dataReader.Sql, dbParameters.ToArray());
            }
            catch (Exception ex)
            {
                throw new ApiException("[ExecuteDataReader], " + ex.Message);
            }
            return dataSet;
        }
    }

    [EnableInBuilder(BuilderType.ApiBuilder)]
    public class EbSqlFunc : ApiResources
    {
        [EnableInBuilder(BuilderType.ApiBuilder)]
        [PropertyEditor(PropertyEditorType.ObjectSelector)]
        [PropertyGroup("Data Settings")]
        [OSE_ObjectTypes(EbObjectTypes.iSqlFunction)]
        public override string Reference { get; set; }

        [EnableInBuilder(BuilderType.ApiBuilder)]
        [MetaOnly]
        [UIproperty]
        public string RefName { set; get; }

        [EnableInBuilder(BuilderType.ApiBuilder)]
        [MetaOnly]
        [UIproperty]
        public string Version { set; get; }

        public override string GetDesignHtml()
        {
            return @"<div class='apiPrcItem dropped' eb-type='SqlFunc' id='@id'>
                        <div tabindex='1' class='drpbox' onclick='$(this).focus();'>  
                            <div class='CompLabel'> @Label </div>
                            <div class='CompName'> @RefName </div>
                            <div class='CompVersion'> @Version </div>
                        </div>
                    </div>".RemoveCR().DoubleQuoted();
        }

        public object ExecuteSqlFunction(EbApi Api)
        {
            SqlFuncTestResponse response;
            try
            {
                EbSqlFunction sqlFunc = Api.GetEbObject<EbSqlFunction>(this.Reference, Api.Redis, Api.ObjectsDB);

                List<Param> InputParams = sqlFunc.GetParams(null);

                Api.FillParams(InputParams);

                response = EbObjectsHelper.SqlFuncTest(InputParams, sqlFunc.Name, Api.ObjectsDB);
            }
            catch (Exception ex)
            {
                throw new ApiException("[ExecuteSqlFunction], " + ex.Message);
            }
            return response;
        }
    }

    [EnableInBuilder(BuilderType.ApiBuilder)]
    public class EbSqlWriter : ApiResources
    {
        [EnableInBuilder(BuilderType.ApiBuilder)]
        [PropertyEditor(PropertyEditorType.ObjectSelector)]
        [PropertyGroup("Data Settings")]
        [OSE_ObjectTypes(EbObjectTypes.iDataWriter)]
        public override string Reference { get; set; }

        [EnableInBuilder(BuilderType.ApiBuilder)]
        [MetaOnly]
        [UIproperty]
        public string RefName { set; get; }

        [EnableInBuilder(BuilderType.ApiBuilder)]
        [MetaOnly]
        [UIproperty]
        public string Version { set; get; }

        public override string GetDesignHtml()
        {
            return @"<div class='apiPrcItem dropped' eb-type='SqlWriter' id='@id'>
                        <div tabindex='1' class='drpbox' onclick='$(this).focus();'>  
                            <div class='CompLabel'> @Label </div>
                            <div class='CompName'> @RefName </div>
                            <div class='CompVersion'> @Version </div>
                        </div>
                    </div>".RemoveCR().DoubleQuoted();
        }

        public object ExecuteDataWriter(EbApi Api)
        {
            List<DbParameter> dbParams = new List<DbParameter>();
            try
            {
                EbDataWriter dataWriter = Api.GetEbObject<EbDataWriter>(Reference, Api.Redis, Api.ObjectsDB);

                List<Param> InputParams = dataWriter.GetParams(null);

                Api.FillParams(InputParams);

                foreach (Param param in InputParams)
                {
                    dbParams.Add(Api.DataDB.GetNewParameter(param.Name, (EbDbTypes)Convert.ToInt32(param.Type), param.ValueTo));
                }

                int status = Api.DataDB.DoNonQuery(dataWriter.Sql, dbParams.ToArray());

                if (status > 0)
                {
                    Api.ApiResponse.Message.Description = status + "row inserted";
                    return true;
                }
                else
                {
                    Api.ApiResponse.Message.Description = status + "row inserted";
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw new ApiException("[ExecuteDataWriter], " + ex.Message);
            }
        }

    }

    [EnableInBuilder(BuilderType.ApiBuilder)]
    public class EbEmailNode : ApiResources
    {
        [EnableInBuilder(BuilderType.ApiBuilder)]
        [PropertyEditor(PropertyEditorType.ObjectSelector)]
        [PropertyGroup("Data Settings")]
        [OSE_ObjectTypes(EbObjectTypes.iEmailBuilder)]
        public override string Reference { get; set; }

        [EnableInBuilder(BuilderType.ApiBuilder)]
        [MetaOnly]
        [UIproperty]
        public string RefName { set; get; }

        [EnableInBuilder(BuilderType.ApiBuilder)]
        [MetaOnly]
        [UIproperty]
        public string Version { set; get; }

        public override string GetDesignHtml()
        {
            return @"<div class='apiPrcItem dropped' eb-type='EmailNode' id='@id'>
                        <div tabindex='1' class='drpbox' onclick='$(this).focus();'>  
                            <div class='CompLabel'> @Label </div>
                            <div class='CompName'> @RefName </div>
                            <div class='CompVersion'> @Version </div>
                        </div>
                    </div>".RemoveCR().DoubleQuoted();
        }

        public bool ExecuteEmail(EbApi Api)
        {
            bool status;

            try
            {
                EbEmailTemplate emailTemplate = Api.GetEbObject<EbEmailTemplate>(this.Reference, Api.Redis, Api.ObjectsDB);

                List<Param> InputParams = EbApiHelper.GetEmailParams(emailTemplate, Api.Redis, Api.ObjectsDB);

                Api.FillParams(InputParams);

                Api.MessageProducer.Publish(new EmailAttachmentRequest()
                {
                    ObjId = Convert.ToInt32(this.Reference.Split(CharConstants.DASH)[3]),
                    Params = InputParams,
                    UserId = Api.UserObject.UserId,
                    UserAuthId = Api.UserObject.AuthId,
                    SolnId = Api.SolutionId,
                    RefId = this.Reference
                });

                status = true;

                string msg = $"The mail has been sent successfully to {emailTemplate.To} with subject {emailTemplate.Subject} and cc {emailTemplate.Cc}";

                Api.ApiResponse.Message.Description = msg;
            }
            catch (Exception ex)
            {
                throw new ApiException("[ExecuteEmail], " + ex.Message);
            }
            return status;
        }

    }


    [EnableInBuilder(BuilderType.ApiBuilder)]
    public class EbSmsNode : ApiResources
    {
        [EnableInBuilder(BuilderType.ApiBuilder)]
        [PropertyEditor(PropertyEditorType.ObjectSelector)]
        [PropertyGroup("Data Settings")]
        [OSE_ObjectTypes(EbObjectTypes.iSmsBuilder)]
        public override string Reference { get; set; }

        [EnableInBuilder(BuilderType.ApiBuilder)]
        [MetaOnly]
        [UIproperty]
        public string RefName { set; get; }

        [EnableInBuilder(BuilderType.ApiBuilder)]
        [MetaOnly]
        [UIproperty]
        public string Version { set; get; }

        public override string GetDesignHtml()
        {
            return @"<div class='apiPrcItem dropped' eb-type='SmsNode' id='@id'>
                        <div tabindex='1' class='drpbox' onclick='$(this).focus();'>  
                            <div class='CompLabel'> @Label </div>
                            <div class='CompName'> @RefName </div>
                            <div class='CompVersion'> @Version </div>
                        </div>
                    </div>".RemoveCR().DoubleQuoted();
        }
    }


    [EnableInBuilder(BuilderType.ApiBuilder)]
    public class EbConnectApi : ApiResources
    {
        [EnableInBuilder(BuilderType.ApiBuilder)]
        [PropertyEditor(PropertyEditorType.ObjectSelector)]
        [PropertyGroup("Data Settings")]
        [OSE_ObjectTypes(EbObjectTypes.iApi)]
        public override string Reference { get; set; }

        [EnableInBuilder(BuilderType.ApiBuilder)]
        [MetaOnly]
        [UIproperty]
        public string RefName { set; get; }

        [EnableInBuilder(BuilderType.ApiBuilder)]
        [MetaOnly]
        [UIproperty]
        public string Version { set; get; }

        public override string GetDesignHtml()
        {
            return @"<div class='apiPrcItem dropped' eb-type='ConnectApi' id='@id'>
                        <div tabindex='1' class='drpbox' onclick='$(this).focus();'>  
                            <div class='CompLabel'> @Label </div>
                            <div class='CompName'> @RefName </div>
                            <div class='CompVersion'> @Version </div>
                        </div>
                    </div>".RemoveCR().DoubleQuoted();
        }

        public override object GetResult()
        {
            return (this.Result as ApiResponse).Result;
        }

        public object ExecuteConnectApi(EbApi Api)
        {
            ApiResponse resp = null;

            try
            {
                EbApi apiObject = Api.GetEbObject<EbApi>(this.Reference, Api.Redis, Api.ObjectsDB);

                if (apiObject.Name.Equals(Api.Name))
                {
                    Api.ApiResponse.Message.ErrorCode = ApiErrorCode.ResourceCircularRef;
                    Api.ApiResponse.Message.Description = "Calling Api from the same not allowed, terminated due to circular reference";

                    throw new ApiException("[ExecuteConnectApi], Circular refernce");
                }
                else
                {
                    List<Param> InputParam = EbApiHelper.GetReqJsonParameters(apiObject.Resources, Api.Redis, Api.ObjectsDB);


                    Api.FillParams(InputParam);

                    Dictionary<string, object> d = InputParam.Select(p => new { prop = p.Name, val = p.Value }).ToDictionary(x => x.prop, x => x.val as object);

                    string version = this.Version.Replace(".w", "");

                    resp = Api.Service.Gateway.Send(new ApiMqRequest
                    {
                        Name = this.RefName,
                        Version = version,
                        Data = d,
                        SolnId = Api.SolutionId,
                        UserAuthId = Api.UserObject.AuthId,
                        UserId = Api.UserObject.UserId
                    });

                    if (resp.Message.ErrorCode == ApiErrorCode.NotFound)
                    {
                        Api.ApiResponse.Message.ErrorCode = ApiErrorCode.ResourceNotFound;
                        Api.ApiResponse.Message.Description = resp.Message.Description;

                        throw new ApiException("[ExecuteConnectApi], resource api not found");
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ApiException("[ExecuteConnectApi], " + ex.Message);
            }
            return resp;
        }
    }

    [EnableInBuilder(BuilderType.ApiBuilder)]
    public class EbFormResource : ApiResources
    {
        [EnableInBuilder(BuilderType.ApiBuilder)]
        [PropertyEditor(PropertyEditorType.ObjectSelector)]
        [PropertyGroup("Data Settings")]
        [OSE_ObjectTypes(EbObjectTypes.iWebForm)]
        public override string Reference { get; set; }

        [EnableInBuilder(BuilderType.ApiBuilder)]
        [MetaOnly]
        [UIproperty]
        public string RefName { set; get; }

        [EnableInBuilder(BuilderType.ApiBuilder)]
        [MetaOnly]
        [UIproperty]
        public string Version { set; get; }

        [EnableInBuilder(BuilderType.ApiBuilder)]
        [PropertyEditor(PropertyEditorType.String)]
        public string PushJson { get; set; }

        [EnableInBuilder(BuilderType.ApiBuilder)]
        public string DataIdParam { get; set; }

        public override string GetDesignHtml()
        {
            return @"<div class='apiPrcItem dropped' eb-type='FormResource' id='@id'>
                        <div tabindex='1' class='drpbox' onclick='$(this).focus();'>  
                            <div class='CompLabel'> @Label </div>
                            <div class='CompName'> @RefName </div>
                            <div class='CompVersion'> @Version </div>
                        </div>
                    </div>".RemoveCR().DoubleQuoted();
        }

        public object ExecuteFormResource(EbApi Api)
        {
            try
            {
                int RecordId = 0;
                NTVDict _params = new NTVDict();
                foreach (KeyValuePair<string, object> p in Api.GlobalParams)
                {
                    EbDbTypes _type;
                    if (p.Value is int)
                        _type = EbDbTypes.Int32;
                    else //check other types here if required
                        _type = EbDbTypes.String;
                    _params.Add(p.Key, new NTV() { Name = p.Key, Type = _type, Value = p.Value });
                }

                if (!string.IsNullOrWhiteSpace(this.DataIdParam) && Api.GlobalParams.ContainsKey(this.DataIdParam))
                {
                    int.TryParse(Convert.ToString(Api.GlobalParams[this.DataIdParam]), out RecordId);
                }
                InsertOrUpdateFormDataRqst request = new InsertOrUpdateFormDataRqst
                {
                    RefId = this.Reference,
                    PushJson = this.PushJson,
                    UserId = Api.UserObject.UserId,
                    UserAuthId = Api.UserObject.AuthId,
                    RecordId = RecordId,
                    LocId = Convert.ToInt32(Api.GlobalParams["eb_loc_id"]),
                    SolnId = Api.SolutionId,
                    WhichConsole = "uc",
                    FormGlobals = new FormGlobals { Params = _params },
                    //TransactionConnection = TransactionConnection
                };

                EbConnectionFactory ebConnection = new EbConnectionFactory(Api.SolutionId, Api.Redis);
                InsertOrUpdateFormDataResp resp = EbFormHelper.InsertOrUpdateFormData(request, Api.DataDB, Api.Service, Api.Redis, ebConnection);

                if (resp.Status == (int)HttpStatusCode.OK)
                    return resp.RecordId;
                else
                    throw new Exception(resp.Message);
            }
            catch (Exception ex)
            {
                throw new ApiException("[ExecuteFormResource], " + ex.Message);
            }
        }
    }

    [EnableInBuilder(BuilderType.ApiBuilder)]
    public class EbPivotTable : ApiResources
    {
        [EnableInBuilder(BuilderType.ApiBuilder)]
        [PropertyEditor(PropertyEditorType.PivotConfiguration)]
        public PivotConfig Pivotconfig { get; set; }

        public override string GetDesignHtml()
        {
            return @"<div class='apiPrcItem dropped' eb-type='PivotTable' id='@id'>
                        <div tabindex='1' class='drpbox' onclick='$(this).focus();'>  
                            <div class='CompLabel'> @Label </div>
                            <div class='CompName'> @RefName </div>
                            <div class='CompVersion'> @Version </div>
                        </div>
                    </div>".RemoveCR().DoubleQuoted();
        }
    }

    [EnableInBuilder(BuilderType.ApiBuilder)]
    public class EbEmailRetriever : ApiResources
    {
        [EnableInBuilder(BuilderType.ApiBuilder)]
        [PropertyEditor(PropertyEditorType.ObjectSelector)]
        [PropertyGroup("Data Settings")]
        [OSE_ObjectTypes(EbObjectTypes.iWebForm)]
        public override string Reference { get; set; }

        [EnableInBuilder(BuilderType.ApiBuilder)]
        [MetaOnly]
        [UIproperty]
        public string RefName { set; get; }

        [EnableInBuilder(BuilderType.ApiBuilder)]
        [MetaOnly]
        [UIproperty]
        public string Version { set; get; }

        [EnableInBuilder(BuilderType.ApiBuilder)]
        [PropDataSourceJsFn("return ebcontext.EmailRetrieveConnections")]
        [PropertyEditor(PropertyEditorType.DropDown)]
        public int MailConnection { get; set; }

        [EnableInBuilder(BuilderType.ApiBuilder)]
        [PropertyEditor(PropertyEditorType.DateTime)]
        public DateTime DefaultSyncDate { get; set; }

        [EnableInBuilder(BuilderType.ApiBuilder)]
        public bool SubmitAttachmentAsMultipleForm { get; set; }

        public override string GetDesignHtml()
        {
            return @"<div class='apiPrcItem dropped' eb-type='EmailRetriever' id='@id'>
                        <div tabindex='1' class='drpbox' onclick='$(this).focus();'>  
                            <div class='CompLabel'> @Label </div>
                            <div class='CompName'> @RefName </div>
                            <div class='CompVersion'> @Version </div>
                        </div>
                    </div>".RemoveCR().DoubleQuoted();
        }

        public object ExecuteEmailRetriever(EbApi Api, bool isMq)
        {
            try
            {
                EbConnectionFactory EbConnectionFactory = new EbConnectionFactory(Api.SolutionId, Api.Redis);
                if (EbConnectionFactory.EmailRetrieveConnection[this.MailConnection] != null)
                {
                    RetrieverResponse retrieverResponse = EbConnectionFactory.EmailRetrieveConnection[this.MailConnection].Retrieve(Api.Service, this.DefaultSyncDate, Api.SolutionId, isMq, SubmitAttachmentAsMultipleForm);

                    EbWebForm _form = Api.Redis.Get<EbWebForm>(this.Reference);
                    SchemaHelper.GetWebFormSchema(_form);
                    if (!(_form is null))
                    {
                        WebformData data = _form.GetEmptyModel();

                        foreach (RetrieverMessage _m in retrieverResponse?.RetrieverMessages)
                        {
                            InsertFormData(_form, data, _m, this.Reference, Api.SolutionId, Api.UserObject, Api.Redis, Api.Service, EbConnectionFactory);
                        }
                    }
                    else
                    {
                        throw new ApiException("[ExecuteEmailRetriever], form objects is not in redis.");
                    }
                }
                else
                {
                    throw new ApiException("[ExecuteEmailRetriever], mail connection doesn't exist.");
                }
            }
            catch (Exception ex)
            {
                throw new ApiException("[ExecuteEmailRetriever], " + ex.Message);
            }
            return 0;
        }

        public int InsertFormData(EbWebForm _form, WebformData data, RetrieverMessage _m, string refid,
            string SolnId, User UserObject, IRedisClient Redis, Service Service, EbConnectionFactory EbConnectionFactory)
        {
            data.MultipleTables[_form.TableName][0]["mail_subject"] = _m.Message.Subject;
            data.MultipleTables[_form.TableName][0]["mail_body"] = _m.Message.Body;
            data.MultipleTables[_form.TableName][0]["mail_from"] = _m.Message.From.Address;
            data.MultipleTables[_form.TableName][0]["mail_to"] = _m.Message.To.ToString();
            data.MultipleTables[_form.TableName][0]["mail_cc"] = _m.Message.CC.ToString();
            data.MultipleTables[_form.TableName][0]["mail_bcc"] = _m.Message.Bcc.ToString();
            data.MultipleTables[_form.TableName][0]["mail_bcc"] = _m.Message.Bcc.ToString();
            data.MultipleTables[_form.TableName][0]["mail_attachment_names"] = _m.AttachmentsName.ToString();

            foreach (int _att in _m.Attachemnts)
            {
                if (!data.ExtendedTables.ContainsKey("mail_attachments"))
                    data.ExtendedTables.Add("mail_attachments", new SingleTable());

                SingleRow r = new SingleRow();
                r.Columns.Add(new SingleColumn
                {
                    Value = _att,
                    Type = 7
                });

                data.ExtendedTables["mail_attachments"].Add(r);
            }

            InsertDataFromWebformRequest request = new InsertDataFromWebformRequest
            {
                RefId = refid,
                FormData = EbSerializers.Json_Serialize(data),
                SolnId = SolnId,
                UserAuthId = UserObject?.AuthId,
                CurrentLoc = UserObject.Preference.DefaultLocation,
            };

            InsertDataFromWebformResponse response = EbFormHelper.InsertDataFromWebform(request, Redis, Service, EbConnectionFactory);


            return response.RowId;
        }
    }

    [EnableInBuilder(BuilderType.ApiBuilder)]
    public class EbFtpPuller : ApiResources
    {
        [EnableInBuilder(BuilderType.ApiBuilder)]
        public string UserName { set; get; }

        [EnableInBuilder(BuilderType.ApiBuilder)]
        public string ServerAddress { set; get; }

        [EnableInBuilder(BuilderType.ApiBuilder)]
        public string Password { set; get; }

        [EnableInBuilder(BuilderType.ApiBuilder)]
        public string FileName { get; set; }

        [EnableInBuilder(BuilderType.ApiBuilder)]
        public string DirectoryPath { get; set; } = "/";

        [EnableInBuilder(BuilderType.ApiBuilder)]
        public bool DeleteAfterProcessing { get; set; }

        public override string GetDesignHtml()
        {
            return @"<div class='apiPrcItem dropped' eb-type='FtpPuller' id='@id'>
                        <div tabindex='1' class='drpbox' onclick='$(this).focus();'>  
                            <div class='CompLabel'> @Label </div>
                        </div>
                    </div>".RemoveCR().DoubleQuoted();
        }

        public object ExecuteFtpPuller()
        {
            string fName = this.DirectoryPath + this.FileName;
            bool is_downloaded;
            try
            {
                if (!string.IsNullOrEmpty(ServerAddress))
                {
                    MemoryStream ms = new MemoryStream();
                    FtpClient client = new FtpClient(this.ServerAddress, this.UserName, this.Password);
                    client.AutoConnect();
                    if (DeleteAfterProcessing)
                    {
                        string datePart = DateTime.Now.ToString("dd-MM-yyyy_HH-mm"); ;
                        string fileName = Path.GetFileNameWithoutExtension(fName) + datePart + Path.GetExtension(fName);
                        bool is_renamed = client.MoveFile(fName, fileName);
                        if (is_renamed)
                            is_downloaded = client.DownloadStream(ms, fileName);
                    }
                    else
                    {
                        is_downloaded = client.DownloadStream(ms, fName);
                    }

                    ms.Position = 0;
                    this.Result = ms;

                    client.Disconnect();
                }
                else
                {
                    throw new ApiException("[ExecuteFtpPuller], ServerAddress doesn't exist.");
                }
            }
            catch (Exception ex)
            {
                string message = "[ExecuteFtpPuller], " + ex.Message;
                throw new ApiException(message);
            }
            return this.Result;
        }

        public override object GetResult()
        {
            return this.Result;
        }

    }

    [EnableInBuilder(BuilderType.ApiBuilder)]
    public class EbCSVPusher : ApiResources
    {
        [EnableInBuilder(BuilderType.ApiBuilder)]
        [PropertyEditor(PropertyEditorType.ObjectSelector)]
        [PropertyGroup("Data Settings")]
        [OSE_ObjectTypes(EbObjectTypes.iWebForm)]
        public override string Reference { get; set; }

        public override string GetDesignHtml()
        {
            return @"<div class='apiPrcItem dropped' eb-type='CSVPusher' id='@id'>
                        <div tabindex='1' class='drpbox' onclick='$(this).focus();'>  
                            <div class='CompLabel'> @Label </div>
                        </div>
                    </div>".RemoveCR().DoubleQuoted();
        }

        public object ExecuteCSVPusher(EbApi Api)
        {
            try
            {
                EbConnectionFactory EbConnectionFactory = new EbConnectionFactory(Api.SolutionId, Api.Redis);
                object data = (Api.Resources[Api.Step - 1])?.Result;
                if (data != null && (data as Stream).Length > 0)
                {
                    UploadCSVByLoopHoc(data as Stream, Api.Service, Api);
                }
                else
                {
                    throw new ApiException("[ExecuteCSVPusher], No input.");
                }
            }
            catch (Exception ex)
            {
                throw new ApiException("[ExecuteCSVPusher], " + ex.Message);
            }
            return this.Result;
        }

        public void UploadCSVByLoopHoc(Stream stream, Service Service, EbApi Api)
        {
            using (StreamReader CsvReader = new StreamReader(stream))
            {
                string inputLine = "";
                int line_number = 1;

                EbConnectionFactory EbConnectionFactory = new EbConnectionFactory(Api.SolutionId, Api.Redis);
                EbWebForm _form = Api.Redis.Get<EbWebForm>(this.Reference);
                if (_form != null)
                {
                    SchemaHelper.GetWebFormSchema(_form);
                    List<int> RowIds = new List<int>();

                    while ((inputLine = CsvReader.ReadLine()) != null)
                    {
                        string[] values = inputLine.Split('\t');
                        try
                        {
                            if (values?.Length > 0 && line_number > 1) // line 1 is header
                            {
                                WebformData data = _form.GetEmptyModel();
                                SingleRow row = data.MultipleTables[_form.TableName][0];

                                row["campaign_name"] = values[0];
                                row["name"] = values[1];
                                row["genurl"] = values[2];
                                row["genemail"] = values[3];
                                row["city"] = (values.Length >= 5) ? values[4] : "";
                                row["treatment"] = (values.Length >= 6) ? values[5] : "";
                                if (values.Length >= 7 && values[6].ToLower() == "google")
                                    row["google_lead"] = "Yes";
                                else
                                    row["fb_lead"] = "Yes";

                                row["preferred_location"] = (values.Length >=8 )?values[7] : "";
                                row["alternate_number"] = (values.Length >= 9) ? values[8] : "";

                                InsertDataFromWebformRequest request = new InsertDataFromWebformRequest
                                {
                                    RefId = this.Reference,
                                    FormData = EbSerializers.Json_Serialize(data),
                                    CurrentLoc = 22,
                                    UserId = Api.UserObject.UserId,
                                    UserAuthId = Api.UserObject.AuthId,
                                    SolnId = Api.SolutionId
                                };

                                InsertDataFromWebformResponse response = EbFormHelper.InsertDataFromWebform(request, Api.Redis, Service, EbConnectionFactory);
                                //Api.InsertLinesLog(response.Status.ToString(), "CsvPusherForm");
                                RowIds.Add(response.RowId);
                            }
                            line_number++;
                        }
                        catch (Exception)
                        {

                        }
                    }

                    if (RowIds.Count > 0)
                    {
                        this.Result = RowIds;
                    }
                }
            }
        }
        public void UploadCSVByBatch(Stream stream, Service Service, EbApi Api)
        {
            using (StreamReader CsvReader = new StreamReader(stream))
            {
                string inputLine = "";
                int line_number = 1;
                EbDataTable dt = new EbDataTable();
                List<ColumnsInfo> _colInfo = new List<ColumnsInfo>();

                EbWebForm _form = Api.Redis.Get<EbWebForm>(this.Reference);
                dt.TableName = _form.TableName;

                while ((inputLine = CsvReader.ReadLine()) != null)
                {
                    string[] values = inputLine.Split('\t');

                    if (line_number == 1)
                    {
                        int colIndex = 0;
                        foreach (string value in values)
                        {
                            EbDataColumn dc = new EbDataColumn
                            {
                                ColumnName = value.ToLower().Replace(" ", "_"),
                                Type = EbDbTypes.String,
                                ColumnIndex = colIndex,
                                TableName = dt.TableName
                            };
                            dt.Columns.Add(dc);
                            colIndex++;
                        }
                    }
                    else
                    {
                        EbDataRow dr = dt.NewDataRow2();
                        int colIndex = 0;
                        foreach (string value in values)
                        {
                            dr[colIndex] = value;
                            colIndex++;
                        }
                        dt.Rows.Add(dr);
                    }
                    line_number++;
                }
                CsvReader.Close();

                if (dt.Rows.Count > 0)
                {
                    InsertBatchDataResponse response = Service.Gateway.Send<InsertBatchDataResponse>(new InsertBatchDataRequest
                    {
                        Data = dt,
                        RefId = this.Reference,
                        UserId = Api.UserObject.UserId,
                        UserAuthId = Api.UserObject.AuthId,
                        SolnId = Api.SolutionId
                    });
                    this.Result = response?.RecordIds;
                }

            }
        }

        public override object GetResult()
        {
            return this.Result;
        }

    }

    [EnableInBuilder(BuilderType.ApiBuilder)]
    public class EbEncrypt : ApiResources
    {
        [EnableInBuilder(BuilderType.ApiBuilder)]
        [PropertyGroup("Data Settings")]
        public EncryptionAlgorithm EncryptionAlgorithm { get; set; }

        [PropertyGroup("Data Settings")]
        [EnableInBuilder(BuilderType.ApiBuilder)]
        public KeySource KeySource { set; get; }

        [PropertyGroup("Data Settings")]
        [EnableInBuilder(BuilderType.ApiBuilder)]
        public String DataParameterName { set; get; }

        [PropertyGroup("Static")]
        [EnableInBuilder(BuilderType.ApiBuilder)]
        public string PublicKey { set; get; }

        [EnableInBuilder(BuilderType.ApiBuilder)]
        [PropertyEditor(PropertyEditorType.String)]
        [PropertyGroup("Api")]
        public string Url { set; get; }

        [EnableInBuilder(BuilderType.ApiBuilder)]
        [PropertyEditor(PropertyEditorType.ObjectSelector)]
        [PropertyGroup("DataReader")]
        [OSE_ObjectTypes(EbObjectTypes.iDataReader)]
        public string KeyReference { get; set; }


        public override string GetDesignHtml()
        {
            return @"<div class='apiPrcItem dropped' eb-type='Encrypt' id='@id'>
                        <div tabindex='1' class='drpbox' onclick='$(this).focus();'>  
                            <div class='CompLabel'> @Label </div>
                        </div>
                    </div>".RemoveCR().DoubleQuoted();
        }
    }

    [EnableInBuilder(BuilderType.ApiBuilder)]
    public class EbDecrypt : ApiResources
    {
        [EnableInBuilder(BuilderType.ApiBuilder)]
        [PropertyGroup("Data Settings")]
        public EncryptionAlgorithm EncryptionAlgorithm { get; set; }

        [EnableInBuilder(BuilderType.ApiBuilder)]
        [PropertyGroup("Static")]
        public string PrivateKey { set; get; }

        [EnableInBuilder(BuilderType.ApiBuilder)]
        public KeySource KeySource { set; get; }

        [EnableInBuilder(BuilderType.ApiBuilder)]
        [PropertyEditor(PropertyEditorType.String)]
        [PropertyGroup("Api")]
        public string Url { set; get; }

        [EnableInBuilder(BuilderType.ApiBuilder)]
        [PropertyEditor(PropertyEditorType.ObjectSelector)]
        [PropertyGroup("DataReader")]
        [OSE_ObjectTypes(EbObjectTypes.iDataReader)]
        public string KeyReference { get; set; }

        public override string GetDesignHtml()
        {
            return @"<div class='apiPrcItem dropped' eb-type='Decrypt' id='@id'>
                        <div tabindex='1' class='drpbox' onclick='$(this).focus();'>  
                            <div class='CompLabel'> @Label </div>
                        </div>
                    </div>".RemoveCR().DoubleQuoted();
        }
    }

    [EnableInBuilder(BuilderType.ApiBuilder)]
    public class EbLoop : ApiResources, IApiCtrlCollection
    {
        [EnableInBuilder(BuilderType.ApiBuilder)]
        [HideInPropertyGrid]
        public OrderedList InnerResources { get; set; }
        public override List<Param> GetOutParams(List<Param> _param, int step)
        {
            List<Param> OutParams;
            if (this.InnerResources[0] is IApiCtrlCollection)
                OutParams = ((this.InnerResources[0] as IApiCtrlCollection).InnerResources[step - 1]).GetOutParams(_param, step);
            else
                OutParams = this.InnerResources[step - 1].GetOutParams(_param, step);

            return OutParams;
        }

        public override string GetDesignHtml()
        {
            return @"<div  class='apiPrcItem jobItem dropped api-item-border' eb-type='Loop' id='@id'> <div class='api-item-inner'>
                        <div tabindex='1' class='drpboxInt lineDrp' onclick='$(this).focus();' id='@id_LpStr' >  
                            <div class='CompLabel'> Loop Start</div>
                        </div>
                        <div class='Api_Dropable'> </div>
                        <div tabindex='1' class='drpbox lineDrp' onclick='$(this).focus();' id='@id_LpEnd'>  
                            <div class='CompLabel'> Loop End</div>
                        </div>
                    </div></div>".RemoveCR().DoubleQuoted();
        }

        public const string EB_LOC_ID = "eb_loc_id";

        public bool DoLoop( EbApi Api, int step, int parentindex)
        {
            EbDataTable _table;
            try
            {
                if (parentindex == 0 && step == 1)
                    _table = (Api.Resources[step - 1].Result as EbDataSet).Tables[0];
                else
                    _table = (Api.Resources[parentindex - 1].Result as EbDataSet).Tables[0];

                int _rowcount = _table.Rows.Count;
                for (int i = 0; i < _rowcount; i++)
                {
                    try
                    {
                        EbDataColumn cl = _table.Columns[0];
                        Param _outparam = new Param
                        {
                            Name = cl.ColumnName,
                            Type = cl.Type.ToString(),
                            Value = _table.Rows[i][cl.ColumnIndex].ToString()
                        };
                        Api.Resources[step].Result = _outparam;
                        if (Api.GlobalParams.ContainsKey(_outparam.Name))
                            Api.GlobalParams[_outparam.Name] = new TV { Type = _outparam.Type, Value = _outparam.Value };
                        else
                            Api.GlobalParams.Add(_outparam.Name, new TV { Type = _outparam.Type, Value = _outparam.Value });


                        if (!Api.GlobalParams.ContainsKey(EB_LOC_ID))
                        {
                            if (_table.Columns[EB_LOC_ID] != null)
                            {
                                Api.GlobalParams.Add(EB_LOC_ID, new TV { Type = EbDbTypes.Int32.ToString(), Value = _table.Rows[i][EB_LOC_ID].ToString() });
                            }
                        }

                        ExecuteLoop(Api, 0, step, parentindex, _table.Rows[i], null);
                        //    message = "Loop Execution Success. counter " + i + " of " + _rowcount;
                    }
                    catch (Exception e)
                    {
                        // message = "Loop Failed to execute. counter " + i + " of " + _rowcount;
                        Console.WriteLine(e.Message + e.StackTrace);
                    }
                }
                // MasterResult.Add(new SqlJobResult { Message = "Loop execution Success with " + _rowcount + " iterations.", Type = ResourceType.Loop });
            }
            catch (Exception e)
            {
                // MasterResult.Add(new SqlJobResult { Message = "Loop execution Failed" + e.Message, Type = ResourceType.Loop });
                Console.WriteLine(e.Message + e.StackTrace);
            }
            return true;
        }

       

        

        public void ExecuteLoop(EbApi Api, int retryof, int step, int parentindex, EbDataRow dataRow, Dictionary<string, TV> keyvals)
        {
            try
            {
                int counter;
                string _keyvalues = (dataRow is null) ? JsonConvert.SerializeObject(keyvals) : Api.FillKeys(dataRow);
                try
                {
                    for (counter = 0; counter < this.InnerResources.Count; counter++)
                    {
                        if (this.InnerResources[counter] is EbProcessor)
                        {
                            if (this.InnerResources[counter - 1] is EbSqlReader)
                                if (((this.InnerResources[counter - 1] as EbSqlReader).Result as EbDataSet).Tables[0].Rows.Count > 0)
                                    this.InnerResources[counter].Result = EbApiHelper.GetResult(this.InnerResources[counter],Api, counter, step);
                                else
                                {
                                    Console.WriteLine("Datareader returned 0 rows : " + (this.InnerResources[counter - 1] as EbSqlReader).RefId + "\n" +
                                        JsonConvert.SerializeObject(dataRow));
                                    return;
                                }
                        }
                        this.InnerResources[counter].Result = EbApiHelper.GetResult(this.InnerResources[counter],Api, counter, step);
                    }
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error at LoopExecution");
                throw e;
            }
        }
    }

    [EnableInBuilder(BuilderType.ApiBuilder)]
    public class EbTransaction : ApiResources, IApiCtrlCollection
    {
        [EnableInBuilder(BuilderType.ApiBuilder)]
        [HideInPropertyGrid]
        public OrderedList InnerResources { get; set; }

        public override List<Param> GetOutParams
            (List<Param> _param, int step)
        {
            List<Param> OutParams;
            if (this.InnerResources[0] is IApiCtrlCollection)
                OutParams = ((this.InnerResources[0] as IApiCtrlCollection).InnerResources[step - 1]).GetOutParams(_param, step);
            else
                OutParams = this.InnerResources[step - 1].GetOutParams(_param, step);

            foreach (Param p in OutParams)
            {
                p.Value = (this.Result as Param).Value;
            }
            return _param;
        }
        public override string GetDesignHtml()
        {
            return @"<div id='@id' class='apiPrcItem jobItem dropped api-item-border' eb-type='Transaction'> <div class='api-item-inner'>
                        <div tabindex='1' class='drpboxInt lineDrp apiPrcItem' onclick='$(this).focus();' id='@id_TrStr'>  
                            <div class='CompLabel'> Transaction Start</div>
                        </div>
                        <div class='Api_Dropable'> </div>
                        <div tabindex='1' class='drpbox lineDrp apiPrcItem' onclick='$(this).focus();' id='@id_TrEnd'>  
                            <div class='CompLabel'> Transaction End</div>
                        </div>
                    </div></div>".RemoveCR().DoubleQuoted();
        }
    }

    public interface IApiCtrlCollection
    {
        [EnableInBuilder(BuilderType.ApiBuilder)]
        OrderedList InnerResources { get; set; }
    }

    [EnableInBuilder(BuilderType.ApiBuilder)]
    public class OrderedList : List<ApiResources>
    {
        public OrderedList()
        {
            this.Sort((x, y) => x.RouteIndex.CompareTo(y.RouteIndex));
        }
    }
}
