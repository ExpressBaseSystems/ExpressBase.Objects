using ExpressBase.Common;
using ExpressBase.Common.Constants;
using ExpressBase.Common.Data;
using ExpressBase.Common.Extensions;
using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using ExpressBase.Common.Structures;
using ExpressBase.Security;
using ExpressBase.Objects.Objects;
using ExpressBase.Objects.ServiceStack_Artifacts;
using ExpressBase.Objects.WebFormRelated;
using Newtonsoft.Json;
using ServiceStack;
using ServiceStack.Redis;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Reflection;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Net;
using ExpressBase.Common.LocationNSolution;
using System.Threading.Tasks;

namespace ExpressBase.Objects
{
    public static class EbFormHelper
    {
        public static List<string> DiscoverRelatedRefids(EbControlContainer _this)
        {
            List<string> refids = new List<string>();
            if (_this is EbWebForm webForm)
            {
                foreach (EbDataPusher pusher in webForm.DataPushers)
                {
                    if (!string.IsNullOrWhiteSpace(pusher.FormRefId))
                        refids.Add(pusher.FormRefId);
                }
            }
            EbControl[] _flatCtrls = new List<EbControl>() { _this }.FlattenAllEbControls();
            for (int i = 0; i < _flatCtrls.Length; i++)
            {
                if (_flatCtrls[i] is EbUserControl || _flatCtrls[i] is EbDGUserControlColumn)
                {
                    if (!string.IsNullOrEmpty(_flatCtrls[i].RefId))
                        refids.Add(_flatCtrls[i].RefId);
                }
                else
                {
                    PropertyInfo[] _props = _flatCtrls[i].GetType().GetProperties();
                    foreach (PropertyInfo _prop in _props)
                    {
                        if (_prop.IsDefined(typeof(OSE_ObjectTypes)))
                        {
                            object _val = _prop.GetValue(_flatCtrls[i], null);
                            if (_val != null)
                            {
                                if (_prop.PropertyType == typeof(string))
                                {
                                    if (!_val.ToString().IsEmpty())
                                        refids.Add(_val.ToString());
                                }
                                else if (_prop.PropertyType == typeof(List<ObjectBasicInfo>))
                                {
                                    foreach (ObjectBasicInfo info in _val as List<ObjectBasicInfo>)
                                    {
                                        if (!string.IsNullOrEmpty(info.ObjRefId))
                                            refids.Add(info.ObjRefId);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return refids;
        }

        public static void ReplaceRefid(EbControlContainer _this, Dictionary<string, string> RefidMap)
        {
            if (_this is EbWebForm webForm)
            {
                foreach (EbDataPusher pusher in webForm.DataPushers)
                {
                    if (!string.IsNullOrWhiteSpace(pusher.FormRefId))
                    {
                        if (RefidMap.ContainsKey(pusher.FormRefId))
                            pusher.FormRefId = RefidMap[pusher.FormRefId];
                        else
                            pusher.FormRefId = string.Empty;
                    }
                }
            }
            EbControl[] _flatCtrls = new List<EbControl>() { _this }.FlattenAllEbControls();
            for (int i = 0; i < _flatCtrls.Length; i++)
            {
                if (_flatCtrls[i] is EbUserControl || _flatCtrls[i] is EbDGUserControlColumn)
                {
                    if (!string.IsNullOrEmpty(_flatCtrls[i].RefId))
                    {
                        if (RefidMap.ContainsKey(_flatCtrls[i].RefId))
                            _flatCtrls[i].RefId = RefidMap[_flatCtrls[i].RefId];
                        else
                            _flatCtrls[i].RefId = "";
                    }
                }
                else
                {
                    PropertyInfo[] _props = _flatCtrls[i].GetType().GetProperties();
                    foreach (PropertyInfo _prop in _props)
                    {
                        if (_prop.IsDefined(typeof(OSE_ObjectTypes)))
                        {
                            object _val = _prop.GetValue(_flatCtrls[i], null);
                            if (_val != null)
                            {
                                if (_prop.PropertyType == typeof(string))
                                {
                                    string st_val = _val.ToString();
                                    if (!st_val.IsEmpty())
                                    {
                                        if (RefidMap.ContainsKey(st_val))
                                            _prop.SetValue(_flatCtrls[i], RefidMap[st_val], null);
                                        else
                                            _prop.SetValue(_flatCtrls[i], "");
                                    }
                                }
                                else if (_prop.PropertyType == typeof(List<ObjectBasicInfo>))
                                {
                                    foreach (ObjectBasicInfo info in _val as List<ObjectBasicInfo>)
                                    {
                                        if (!string.IsNullOrEmpty(info.ObjRefId))
                                        {
                                            if (RefidMap.ContainsKey(info.ObjRefId))
                                                info.ObjRefId = info.ObjRefId;
                                            else
                                                info.ObjRefId = "";
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        //Rendering side -> service = null
        public static void AfterRedisGet(EbControlContainer _this, IRedisClient Redis, IServiceClient client, Service service)
        {
            try
            {
                for (int i = 0; i < _this.Controls.Count; i++)
                {
                    EbControl c = _this.Controls[i];
                    c.IsRenderMode = _this.IsRenderMode;
                    if (c is EbUserControl || c is EbDGUserControlColumn)
                    {
                        EbUserControl _temp = GetEbObject<EbUserControl>(c.RefId, client, Redis, service);
                        if (c is EbDGUserControlColumn)
                        {
                            foreach (EbControl Control in _temp.Controls)
                            {
                                RenameControlsRec(Control, c.Name);
                            }
                            (c as EbDGUserControlColumn).InitUserControl(_temp);
                        }
                        else
                        {
                            (c as EbUserControl).Controls = _temp.Controls;
                            (c as EbUserControl).DisplayName = _temp.DisplayName;
                            (c as EbUserControl).VersionNumber = _temp.VersionNumber;
                            foreach (EbControl Control in (c as EbUserControl).Controls)
                            {
                                RenameControlsRec(Control, c.Name);
                            }
                            c.AfterRedisGet(Redis as RedisClient, client);
                        }
                    }
                    else if (c is EbControlContainer)
                    {
                        if (c is EbDataGrid dg && !string.IsNullOrWhiteSpace(dg.CustomSelectDS))
                        {
                            EbDataReader _DR = GetEbObject<EbDataReader>(dg.CustomSelectDS, client, Redis, service);
                            dg.CustomSelectDSQuery = _DR.Sql.Replace(CharConstants.SEMI_COLON, CharConstants.SPACE) + CharConstants.SEMI_COLON;
                        }
                        else if (c is EbTableLayout tl && c.IsRenderMode)
                        {
                            tl.AdjustColumnWidth();
                        }
                        AfterRedisGet(c as EbControlContainer, Redis, client, service);
                    }
                    else if (c is EbProvisionLocation)//add unmapped ctrls as DoNotPersist controls
                    {
                        //if (_this.IsRenderMode)
                        //{
                        //    EbProvisionLocation prvnCtrl = c as EbProvisionLocation;
                        //    for (int j = 0; j < prvnCtrl.Fields.Count; j++)
                        //    {
                        //        UsrLocField prvnFld = prvnCtrl.Fields[j] as UsrLocField;
                        //        if (prvnFld.ControlName.IsNullOrEmpty() && prvnFld.IsRequired)
                        //        {
                        //            if (prvnFld.Type == "image")
                        //            {
                        //                _this.Controls.Insert(i, new EbDisplayPicture()
                        //                {
                        //                    Name = "namecustom" + i,
                        //                    EbSid = "ebsidcustom" + i,
                        //                    EbSid_CtxId = "ebsidcustom" + i,
                        //                    Label = prvnFld.DisplayName,
                        //                    DoNotPersist = true,
                        //                    MaxHeight = 100
                        //                });
                        //            }
                        //            else
                        //            {
                        //                _this.Controls.Insert(i, new EbTextBox()
                        //                {
                        //                    Name = "namecustom" + i,
                        //                    EbSid = "ebsidcustom" + i,
                        //                    EbSid_CtxId = "ebsidcustom" + i,
                        //                    Label = prvnFld.DisplayName,
                        //                    DoNotPersist = true
                        //                });
                        //            }
                        //            prvnFld.ControlName = "namecustom" + i;
                        //            i++;
                        //        }
                        //    }
                        //}
                    }
                    else if (c is EbProvisionUser)
                    {
                        //if (_this.IsRenderMode)
                        //{
                        //    EbProvisionUser prvnCtrl = c as EbProvisionUser;
                        //    for (int j = 0; j < prvnCtrl.Fields.Count; j++)
                        //    {
                        //        UsrLocField prvnFld = prvnCtrl.Fields[j] as UsrLocField;
                        //        if (prvnFld.ControlName.IsNullOrEmpty() && (prvnFld.IsRequired || prvnFld.Name == "email" || prvnFld.Name == "phprimary"))
                        //        {
                        //            _this.Controls.Insert(i, new EbTextBox()
                        //            {
                        //                Name = "namecustom" + i,
                        //                EbSid = "ebsidcustom" + i,
                        //                EbSid_CtxId = "ebsidcustom" + i,
                        //                Label = prvnFld.DisplayName,
                        //                DoNotPersist = true
                        //            });
                        //            prvnFld.ControlName = "namecustom" + i;
                        //            i++;
                        //        }
                        //    }
                        //}
                    }
                }

                if (_this is EbWebForm ebWebForm)
                {
                    ebWebForm.MatViewConfig = new EbMaterializedViewConfig(ebWebForm);
                    if (!string.IsNullOrWhiteSpace(ebWebForm.MatViewRefId))
                    {
                        string date_s = ebWebForm.SolutionObj?.SolutionSettings?.MaterializedViewDate;
                        List<string> matVws = ebWebForm.SolutionObj?.SolutionSettings?.MaterializedViews;

                        if (!string.IsNullOrWhiteSpace(date_s) && matVws != null && matVws.Contains(ebWebForm.MatViewRefId))
                        {
                            ebWebForm.MatViewConfig.SetMatViewObject(GetEbObject<EbMaterializedView>(ebWebForm.MatViewRefId, client, Redis, service));
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("EXCEPTION : FormAfterRedisGet " + e.Message);
            }
        }

        public static void RenameControlsRec(EbControl _control, string _ucName)
        {
            if (_control is EbControlContainer)
            {
                if (!(_control is EbUserControl))
                {
                    foreach (EbControl _ctrl in (_control as EbControlContainer).Controls)
                    {
                        RenameControlsRec(_ctrl, _ucName);
                    }
                }
            }
            else
            {
                _control.ChildOf = "EbUserControl";
                _control.Name = _ucName + "_" + _control.Name;
                _control.EbSid = _ucName + "_" + _control.EbSid;
            }
        }

        public static void InitDataPushers(EbWebForm _this, IRedisClient Redis, IServiceClient client, Service service)
        {
            int i = 1;
            _this.CrudContext = 0.ToString();
            _this.FormCollection = new EbWebFormCollection(_this);
            if (_this.DataPushers != null)
            {
                foreach (EbFormDataPusher pusher in _this.DataPushers.FindAll(e => e is EbFormDataPusher))
                {
                    EbWebForm _form = GetEbObject<EbWebForm>(pusher.FormRefId, client, Redis, service);
                    _form.RefId = pusher.FormRefId;
                    _form.UserObj = _this.UserObj;
                    _form.SolutionObj = _this.SolutionObj;
                    if (service == null)
                        _form.AfterRedisGet_All(Redis as RedisClient, client);
                    else
                        _form.AfterRedisGet_All(service);
                    string _multipushId = null;
                    if (pusher.MultiPushIdType == MultiPushIdTypes.Default)
                        _multipushId = _this.RefId + CharConstants.UNDERSCORE + pusher.Name;
                    if (pusher.MultiPushIdType == MultiPushIdTypes.Row)
                        _multipushId = pusher.Name;

                    _form.DataPusherConfig = new EbDataPusherConfig
                    {
                        SourceTable = _this.FormSchema.MasterTable,
                        MultiPushId = _multipushId,
                        DisableAutoReadOnly = pusher.DisableAutoReadOnly,
                        DisableAutoLock = pusher.DisableAutoLock
                    };
                    _form.CrudContext = i++.ToString();
                    pusher.WebForm = _form;
                    _this.FormCollection.Add(_form);
                    _this.FormDataPusherCount++;
                }
            }
        }

        public static T GetEbObject<T>(string RefId, IServiceClient ServiceClient, IRedisClient Redis, Service Service, PooledRedisClientManager pooledRedisManager = null)
        {
            if (string.IsNullOrEmpty(RefId))
                throw new Exception("RefId is null or empty. FormHelper >GetEbObject. Type: " + typeof(T).Name);

            T _ebObject = default;
            if (pooledRedisManager != null)
            {
                using (var redisReadOnly = pooledRedisManager.GetReadOnlyClient())
                    _ebObject = redisReadOnly.Get<T>(RefId);
            }
            else if (Redis != null)
                _ebObject = Redis.Get<T>(RefId);

            if (_ebObject == null)
            {
                EbObjectParticularVersionResponse resp;
                if (ServiceClient != null)
                    resp = ServiceClient.Get(new EbObjectParticularVersionRequest { RefId = RefId });
                else if (Service != null)
                    resp = Service.Gateway.Send<EbObjectParticularVersionResponse>(new EbObjectParticularVersionRequest { RefId = RefId });
                else
                    throw new Exception($"ServiceClient and Service is null. FormHelper >GetEbObject. RefId: {RefId}, Type: {typeof(T).Name}");

                if (resp.Data == null || resp.Data.Count == 0)
                    throw new Exception($"EbObject not found. FormHelper >GetEbObject. RefId: {RefId}, Type: {typeof(T).Name}");

                _ebObject = EbSerializers.Json_Deserialize(resp.Data[0].Json);
                if (_ebObject == null)
                    throw new Exception($"Json_Deserialize returned a null EbObject. FormHelper >GetEbObject. RefId: {RefId}, Type: {typeof(T).Name}");// , Json: {resp.Data[0].Json} - TempData size issue
                if (Redis != null) Redis.Set<T>(RefId, _ebObject);
            }
            (_ebObject as EbObject).RefId = RefId;// temp fix (sometimes refid missing from ebObject)
            return _ebObject;
        }

        public static T DeepClone<T>(this T obj)
        {
            using (var ms = new MemoryStream())
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(ms, obj);
                ms.Position = 0;

                return (T)bf.Deserialize(ms);
            }
        }

        public static bool HasPermission(User UserObj, string RefId, string ForWhat, int LocId, bool NeglectLoc = false, string WC = TokenConstants.UC)
        {
            if (UserObj.Roles.Contains(SystemRoles.SolutionOwner.ToString()) ||
                UserObj.Roles.Contains(SystemRoles.SolutionAdmin.ToString()) ||
                UserObj.Roles.Contains(SystemRoles.SolutionPM.ToString()))
                return true;

            EbOperation Op = EbWebForm.Operations.Get(ForWhat);
            EbObjectType EbType = RefId.GetEbObjectType();
            if (EbType.IntCode == EbObjectTypes.Report)
                Op = EbReport.Operations.Get(ForWhat);
            else if (EbType.IntCode == EbObjectTypes.TableVisualization)
                Op = EbTableVisualization.Operations.Get(ForWhat);
            else if (EbType.IntCode == EbObjectTypes.ChartVisualization)
                Op = EbChartVisualization.Operations.Get(ForWhat);
            else if (EbType.IntCode == EbObjectTypes.MobilePage)
                Op = EbMobilePage.Operations.Get(ForWhat);
            else if (EbType.IntCode == EbObjectTypes.PosForm)
                Op = EbMobilePage.Operations.Get(ForWhat);

            if (WC == TokenConstants.UC && !Op.IsAvailableInWeb)
                return false;
            else if (WC == TokenConstants.BC && !Op.IsAvailableInBot)
                return false;
            else if (WC == TokenConstants.MC && !Op.IsAvailableInMobile)
                return false;
            else if (WC == TokenConstants.PC && !Op.IsAvailableInPos)
                return false;

            try
            {
                //Permission string format => 020-00-00982-02:5
                string[] refidParts = RefId.Split("-");
                string objType = refidParts[2].PadLeft(2, '0');
                string objId = refidParts[3].PadLeft(5, '0');
                string operation = Op.IntCode.ToString().PadLeft(2, '0');
                string pWithLoc = objType + '-' + objId + '-' + operation + (NeglectLoc ? "" : (":" + LocId));///////////
                string pGlobalLoc = objType + '-' + objId + '-' + operation + ":-1";
                string temp = UserObj.Permissions.Find(p => p.Contains(pWithLoc) || p.Contains(pGlobalLoc));
                if (temp != null)
                    return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(string.Format("Exception when checking user permission: {0}\nRefId = {1}\nOperation = {2}\nLocId = {3}", e.Message, RefId, ForWhat, LocId));
            }

            return false;
        }

        public static void AddExtraSqlParams(List<DbParameter> param, IDatabase DataDB, string tableName, int rowId, int locId, int userId)
        {
            if (param.Find(e => e.ParameterName == FormConstants.eb_loc_id) == null)
                param.Add(DataDB.GetNewParameter(FormConstants.eb_loc_id, EbDbTypes.Decimal, locId));
            if (param.Find(e => e.ParameterName == FormConstants.eb_currentuser_id) == null)
                param.Add(DataDB.GetNewParameter(FormConstants.eb_currentuser_id, EbDbTypes.Decimal, userId));
            if (param.Find(e => e.ParameterName == tableName + FormConstants._id) == null)
                param.Add(DataDB.GetNewParameter(tableName + FormConstants._id, EbDbTypes.Int32, rowId));
            if (param.Find(e => e.ParameterName == FormConstants.id) == null)
                param.Add(DataDB.GetNewParameter(FormConstants.id, EbDbTypes.Int32, rowId));
        }

        public static bool IsExtraSqlParam(string paramName, string tableName)
        {
            List<string> _params = new List<string>
            {
                { FormConstants.eb_loc_id},
                { FormConstants.eb_currentuser_id},
                { FormConstants.eb_current_language_id},
                { FormConstants.eb_current_locale},
                { tableName + FormConstants._id},
                { FormConstants.id}
            };
            return _params.Contains(paramName);
        }

        public static string GetJsRegex(string r, string n, string p)
        {
            //r=>root, n=>name, p=>path
            return $@"{r}.currentRow\[""{n}""\]|{r}.currentRow\['{n}'\]|{r}.currentRow.{n}|{r}.getRowByIndex\((.*?)\)\[""{n}""\]|{r}.getRowByIndex\((.*?)\)\['{n}'\]|{r}.sum\((""{n}"")\)|{r}.sum\(('{n}')\)|{p}";
        }

        //public static bool ContainsAnyDgProperty(string code, string name)
        //{
        //    string[] props = new string[] 
        //    { 
        //        "addRow",
        //        "clear",
        //        "disableRow",
        //        "enableRow",
        //        "disable",
        //        "enable",
        //        "showRow",
        //        "hideRow",
        //        "hideRows",
        //    };
        //    return code.ContainsAny(props);
        //}

        //Create a NEW WebFormData version from EDIT mode WebFormData of 'same' form.
        public static WebformData GetFilledNewFormData(EbWebForm FormSrc, bool clearFyDate, bool isClone)// FormSrc = Source Form
        {
            WebformData newFormData = new WebformData() { MasterTable = FormSrc.FormSchema.MasterTable };
            foreach (TableSchema _t in FormSrc.FormSchema.Tables)
            {
                if (_t.TableType == WebFormTableTypes.Normal || _t.TableType == WebFormTableTypes.Grid)
                {
                    newFormData.MultipleTables.Add(_t.TableName, FormSrc.FormData.MultipleTables[_t.TableName]);
                    SingleTable Table = newFormData.MultipleTables[_t.TableName];
                    if (_t.TableType == WebFormTableTypes.Normal)
                    {
                        if (Table.Count > 0)
                        {
                            SingleColumn c = Table[0].Columns.Find(e => e.Name == FormConstants.id);
                            if (c != null)
                                c.Value = 0;
                            Table[0].RowId = 0;
                            foreach (SingleColumn c_ in Table[0].Columns.FindAll(e => e.Control?.IsSysControl == true || (!isClone && e.Control?.DoNotImport == true) || (isClone && e.Control?.DoNotClone == true)))
                            {
                                SingleColumn t = c_.Control.GetSingleColumn(FormSrc.UserObj, FormSrc.SolutionObj, null, true);
                                c_.Value = t.Value;
                                c_.F = t.F;
                            }
                            if (clearFyDate)
                            {
                                foreach (SingleColumn c__ in Table[0].Columns.FindAll(e => e.Control is EbDate _Date && _Date.RestrictionRule != DateRestrictionRule.None))
                                {
                                    SingleColumn t = c__.Control.GetSingleColumn(FormSrc.UserObj, FormSrc.SolutionObj, null, true);
                                    c__.Value = t.Value;
                                    c__.F = t.F;
                                }
                            }
                        }
                    }
                    else
                    {
                        int id = -1;
                        foreach (SingleRow Row in Table)
                        {
                            Row.RowId = id--;
                            SingleColumn c = Row.Columns.Find(e => e.Name == FormConstants.id);
                            if (c != null) c.Value = 0;

                            foreach (SingleColumn c_ in Row.Columns.FindAll(e => e.Control?.IsSysControl == true || (!isClone && e.Control?.DoNotImport == true) || (isClone && e.Control?.DoNotClone == true)))
                            {
                                SingleColumn t = c_.Control.GetSingleColumn(FormSrc.UserObj, FormSrc.SolutionObj, null, true);
                                c_.Value = t.Value;
                                c_.F = t.F;
                            }
                        }
                    }
                }
                else if (_t.TableType == WebFormTableTypes.Review)
                {
                    newFormData.MultipleTables.Add(_t.TableName, new SingleTable());
                }
            }
            newFormData.DGsRowDataModel = FormSrc.FormData.DGsRowDataModel;
            return newFormData;
        }

        //Copy FormData in form SOURCE to DESTINATION (Different form)
        public static void CopyFormDataToFormData(IDatabase DataDB, EbWebForm FormSrc, EbWebForm FormDes, Dictionary<EbControl, string> psDict, List<DbParameter> psParams, bool CopyAutoId, string srcCtrl)
        {
            List<DataFlowMapAbstract> DataFlowMap = null;
            bool ExportMapedDataOnly = false;
            if (!string.IsNullOrWhiteSpace(srcCtrl))
            {
                ColumnSchema _column = FormSrc.FormSchema.Tables[0].Columns.Find(e => e.Control is EbExportButton && e.Control.Name == srcCtrl);
                if (_column != null && _column.Control is EbExportButton ExprtCtrl && ExprtCtrl.DataFlowMap?.Count > 0)
                {
                    DataFlowMap = ExprtCtrl.DataFlowMap;
                    ExportMapedDataOnly = ExprtCtrl.ExportMapedDataOnly;
                }
            }
            if (DataFlowMap == null)
                DataFlowMap = new List<DataFlowMapAbstract>();

            foreach (TableSchema _tableDes in FormDes.FormSchema.Tables)
            {
                if (_tableDes.TableType == WebFormTableTypes.Grid)
                {
                    if (ExportMapedDataOnly)//Grid is not considered when ExportMapedDataOnly property is set
                        continue;

                    TableSchema _tableSrc = FormSrc.FormSchema.Tables.Find(e => e.ContainerName == _tableDes.ContainerName);
                    if (_tableSrc != null)
                    {
                        int rc = -501;
                        string rmodelDes = JsonConvert.SerializeObject(FormDes.FormData.DGsRowDataModel[_tableDes.TableName]);
                        foreach (SingleRow RowSrc in FormSrc.FormData.MultipleTables[_tableSrc.TableName])
                        {
                            SingleRow RowDes = JsonConvert.DeserializeObject<SingleRow>(rmodelDes);
                            RowDes.RowId = rc--;
                            foreach (ColumnSchema _columnDes in _tableDes.Columns)
                            {
                                SingleColumn ColumnSrc = RowSrc.GetColumn(_columnDes.ColumnName);
                                if (ColumnSrc != null)
                                {
                                    if (!_columnDes.Control.DoNotImport)
                                        RowDes.SetColumn(_columnDes.ColumnName, _columnDes.Control.GetSingleColumn(FormDes.UserObj, FormDes.SolutionObj, ColumnSrc.Value, false));
                                    string _formattedData = Convert.ToString(RowDes[_columnDes.ColumnName]);
                                    if (_columnDes.Control is EbDGPowerSelectColumn && !string.IsNullOrEmpty(_formattedData))
                                    {
                                        if (psDict.ContainsKey(_columnDes.Control))
                                            psDict[_columnDes.Control] += CharConstants.COMMA + _formattedData;
                                        else
                                            psDict.Add(_columnDes.Control, _formattedData);
                                    }
                                    try//temporary solution to avoid exception : 1$$text 
                                    {
                                        if (!psParams.Exists(e => e.ParameterName == _columnDes.ColumnName))
                                            psParams.Add(DataDB.GetNewParameter(_columnDes.ColumnName, (EbDbTypes)_columnDes.EbDbType, _formattedData));
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine($"Exception catched: WebForm -> GetImportData\nMessage: {ex.Message}\nStackTrace: {ex.StackTrace}");
                                    }
                                }
                            }
                            FormDes.FormData.MultipleTables[_tableDes.TableName].Add(RowDes);
                        }
                    }
                }
                else if (_tableDes.TableType == WebFormTableTypes.Normal)
                {
                    SingleTable TableDes = FormDes.FormData.MultipleTables[_tableDes.TableName];
                    if (TableDes.Count > 0)
                    {
                        string _formattedData;
                        SingleColumn ColumnSrc;
                        bool mustCopy;
                        foreach (ColumnSchema _columnDes in _tableDes.Columns)
                        {
                            string srcCtrlName = null;
                            DataFlowMapAbstract DFM = DataFlowMap.Find(e => e is DataFlowForwardMap dffm && dffm.DestCtrlName == _columnDes.ColumnName);

                            if (DFM != null && DFM is DataFlowForwardMap _dffm && !string.IsNullOrWhiteSpace(_dffm.SrcCtrlName))
                                srcCtrlName = _dffm.SrcCtrlName;

                            if (srcCtrlName == null && !ExportMapedDataOnly)
                                srcCtrlName = _columnDes.ColumnName;

                            mustCopy = false;
                            ColumnSrc = srcCtrlName == null ? null : FormSrc.FormData.MultipleTables[FormSrc.FormData.MasterTable][0].GetColumn(srcCtrlName);
                            if (ColumnSrc != null)//source ctrl not found
                            {
                                mustCopy = _columnDes.Control is EbAutoId && CopyAutoId;//import auto id
                                if (!mustCopy)
                                {
                                    mustCopy = _columnDes.Control.IsSysControl && _columnDes.Control is EbSysLocation && !_columnDes.Control.DoNotImport;//sys location must be imported
                                    if (!mustCopy)
                                        mustCopy = !_columnDes.Control.DoNotImport;
                                }
                            }

                            if (mustCopy)
                            {
                                TableDes[0].SetColumn(_columnDes.ColumnName, _columnDes.Control.GetSingleColumn(FormDes.UserObj, FormDes.SolutionObj, ColumnSrc.Value, false));
                                _formattedData = Convert.ToString(ColumnSrc.Value);
                                if (ColumnSrc.Control is EbSimpleFileUploader && _columnDes.Control is EbSimpleFileUploader)
                                {
                                    TableDes[0].GetColumn(_columnDes.ColumnName).F = ColumnSrc.F;
                                }
                            }
                            else
                                _formattedData = Convert.ToString(TableDes[0][_columnDes.ColumnName]);
                            if (_columnDes.Control is EbPowerSelect && !string.IsNullOrEmpty(_formattedData))
                            {
                                if (psDict.ContainsKey(_columnDes.Control))
                                    psDict[_columnDes.Control] += CharConstants.COMMA + _formattedData;
                                else
                                    psDict.Add(_columnDes.Control, _formattedData);
                            }
                            try//temporary solution to avoid exception : 1$$text 
                            {
                                psParams.Add(DataDB.GetNewParameter(_columnDes.ColumnName, (EbDbTypes)_columnDes.EbDbType, _formattedData));
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Exception catched: WebForm -> GetImportData\nMessage: {ex.Message}\nStackTrace: {ex.StackTrace}");
                            }
                        }
                    }
                }
            }
        }

        //Copy API data to DESTINATION
        public static void CopyApiDataToFormData(IDatabase DataDB, Dictionary<string, SingleTable> _PsApiTables, EbWebForm FormDes, Dictionary<EbControl, string> psDict, List<DbParameter> psParams)
        {
            if (_PsApiTables.Count == 0)
                return;
            foreach (KeyValuePair<string, SingleTable> entry in FormDes.FormData.MultipleTables)
            {
                TableSchema _table = FormDes.FormSchema.Tables.Find(e => e.TableName == entry.Key);
                if (_table == null) continue;

                if (_table.TableType == WebFormTableTypes.Normal && _PsApiTables[FormDes.Name].Count > 0)//normal tables
                {
                    foreach (ColumnSchema _column in _table.Columns)
                    {
                        SingleColumn ColumnSrc = _PsApiTables[FormDes.Name][0].GetColumn(_column.ColumnName);
                        string _formattedData = Convert.ToString(entry.Value[0][_column.ColumnName]);
                        if (ColumnSrc != null && !(_column.Control is EbAutoId) && (!_column.Control.IsSysControl || _column.Control is EbSysLocation))
                        {
                            entry.Value[0].SetColumn(_column.ColumnName, _column.Control.GetSingleColumn(FormDes.UserObj, FormDes.SolutionObj, ColumnSrc.Value, false));
                            _formattedData = Convert.ToString(ColumnSrc.Value);
                        }
                        if (_column.Control is EbPowerSelect && !string.IsNullOrEmpty(_formattedData))
                        {
                            if (psDict.ContainsKey(_column.Control))
                                psDict[_column.Control] += CharConstants.COMMA + _formattedData;
                            else
                                psDict.Add(_column.Control, _formattedData);
                        }
                        try//temporary solution to avoid exception : 1$$text 
                        {
                            psParams.Add(DataDB.GetNewParameter(_column.ColumnName, (EbDbTypes)_column.EbDbType, _formattedData));
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Exception catched: WebForm -> FormatImportData\nMessage: {ex.Message}\nStackTrace: {ex.StackTrace}");
                        }
                    }
                }
                else if (_table.TableType == WebFormTableTypes.Grid && _PsApiTables[_table.ContainerName].Count > 0)//grid data
                {
                    int rowCount = -1;
                    string rmodelDes = JsonConvert.SerializeObject(FormDes.FormData.DGsRowDataModel[_table.TableName]);
                    foreach (SingleRow RowSrc in _PsApiTables[_table.ContainerName])
                    {
                        SingleRow RowDes = JsonConvert.DeserializeObject<SingleRow>(rmodelDes);
                        RowDes.RowId = rowCount--;
                        foreach (ColumnSchema _column in _table.Columns)
                        {
                            SingleColumn ColumnSrc = RowSrc.GetColumn(_column.ColumnName);
                            if (ColumnSrc != null)
                            {
                                RowDes.SetColumn(_column.ColumnName, _column.Control.GetSingleColumn(FormDes.UserObj, FormDes.SolutionObj, ColumnSrc.Value, false));
                                string _formattedData = Convert.ToString(ColumnSrc.Value);
                                if (_column.Control is EbDGPowerSelectColumn && !string.IsNullOrEmpty(_formattedData))
                                {
                                    if (psDict.ContainsKey(_column.Control))
                                        psDict[_column.Control] += CharConstants.COMMA + _formattedData;
                                    else
                                        psDict.Add(_column.Control, _formattedData);
                                }
                            }
                        }
                        FormDes.FormData.MultipleTables[_table.TableName].Add(RowDes);
                    }
                }
            }
        }

        //refreshFormData, psDataImport
        //isData = false(get ps data)
        public static Dictionary<string, SingleTable> GetDataFormApi(Service service, EbControl Ctrl, EbWebForm ebWebForm, WebformData _FormData, bool isPsImport)
        {
            string ApiUrl;
            ApiMethods ApiMethod;
            List<ApiRequestHeader> ApiHeaders;
            List<Param> ApiParamsList;
            if (isPsImport)
            {
                EbPowerSelect PwrSel = Ctrl as EbPowerSelect;
                ApiUrl = PwrSel.ImportApiUrl;
                ApiMethod = PwrSel.ImportApiMethod;
                ApiHeaders = PwrSel.ImportApiHeaders;
                ApiParamsList = PwrSel.ImportParamsList;
            }
            else
            {
                IEbPowerSelect IPwrSel = Ctrl as IEbPowerSelect;
                ApiUrl = IPwrSel.Url;
                ApiMethod = IPwrSel.Method;
                ApiHeaders = IPwrSel.Headers;
                ApiParamsList = (Ctrl as IEbDataReaderControl).ParamsList;
            }

            Dictionary<string, SingleTable> Tables = new Dictionary<string, SingleTable>();
            SingleColumn Column;

            foreach (Param param in ApiParamsList)
            {
                Column = null;
                foreach (TableSchema _table in ebWebForm.FormSchema.Tables)
                {
                    if (_table.TableType != WebFormTableTypes.Normal || !_FormData.MultipleTables.ContainsKey(_table.TableName) || _FormData.MultipleTables[_table.TableName].Count == 0)
                        continue;
                    Column = _FormData.MultipleTables[_table.TableName][0].Columns.Find(e => e.Name == param.Name);
                    if (Column != null)
                        break;
                }
                if (Column != null)
                    param.Value = Convert.ToString(Column.Value);
                else
                    Console.WriteLine("Api parameter not found in webformdata: " + param.Name);
            }
            try
            {
                ApiConversionResponse apiResp = service.Gateway.Send<ApiConversionResponse>(new ApiConversionRequest
                {
                    Url = ApiUrl,
                    Headers = ApiHeaders,
                    Method = ApiMethod,
                    Parameters = ApiParamsList
                });
                if (apiResp.dataset?.Tables?.Count > 0)
                {
                    for (int i = 0; i < apiResp.dataset.Tables.Count; i++)
                    {
                        EbDataTable dt = apiResp.dataset.Tables[i];
                        SingleTable Table = new SingleTable();
                        ebWebForm.GetFormattedData(dt, Table);
                        string dtKey = (dt.TableName ?? string.Empty).ToLower().Replace(CharConstants.SPACE, CharConstants.UNDERSCORE);
                        if (ebWebForm.FormSchema.Tables.Find(e => e.ContainerName == dtKey && e.TableType == WebFormTableTypes.Grid) == null)
                            dtKey = ebWebForm.Name;

                        if (Tables.ContainsKey(dtKey))
                        {
                            if (Tables[dtKey].Count > 0 && Table.Count > 0)
                                Tables[dtKey][0].Columns.AddRange(Table[0].Columns);
                            else if (Tables[dtKey].Count == 0 && Table.Count > 0)
                                Tables[dtKey].Add(Table[0]);
                        }
                        else
                            Tables.Add(dtKey, Table);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exception when tried to get PowerSelect rows by calling api\nMessge: {e.Message}\nStackTrace: {e.StackTrace}");
            }

            return Tables;
        }

        public static Dictionary<string, object> GetFormObjectRelatedData(EbWebForm WebForm_L, JsonServiceClient ServiceClient, IRedisClient Redis, string WC)
        {
            //foreach (EbControl control in WebForm_L.Controls.FlattenAllEbControls().ToList().FindAll(e => e is EbDataGrid_New))// for old objects
            //{
            //	(control as EbDataGrid_New).ProcessDvColumnCollection();
            //}

            Dictionary<string, object> dataDict = new Dictionary<string, object>();

            foreach (EbControl control in WebForm_L.Controls.FlattenAllEbControls())
            {
                if (control is EbSimpleSelect ssCtrl)
                {
                    ssCtrl.InitFromDataBase(ServiceClient);
                    //if (ssCtrl.IsDynamic)
                    //    dataDict.Add(ssCtrl.EbSid_CtxId, ssCtrl.Options);
                }
                if (control is EbChartControl chrtCtrl)
                {
                    chrtCtrl.InitFromDataBase(ServiceClient, Redis);
                }
                else if (control is EbTVcontrol tvCtrl)
                {
                    tvCtrl.InitFromDataBase(ServiceClient, Redis);
                }
                else if (control is IEbPowerSelect psCtrl && psCtrl.RenderAsSimpleSelect)
                {
                    psCtrl.InitFromDataBase_SS(ServiceClient);
                    //dataDict.Add(psCtrl.EbSid_CtxId, psCtrl.Options);
                }
                else if (control is EbDGSimpleSelectColumn dgssCtrl)
                {
                    dgssCtrl.EbSimpleSelect.InitFromDataBase(ServiceClient);
                    dgssCtrl.DBareHtml = dgssCtrl.EbSimpleSelect.GetBareHtml();
                    //dataDict.Add(dgssCtrl.EbSid_CtxId, dgssCtrl.Options);
                }
                else if (control is EbUserLocation uloc)
                {
                    uloc.InitFromDataBase(WebForm_L.UserObj, WebForm_L.SolutionObj, WebForm_L.RefId);
                    if (!string.IsNullOrWhiteSpace(uloc.EbSid_CtxId))
                        dataDict.TryAdd(uloc.EbSid_CtxId, uloc.IsGlobalLocAvail);
                }
                else if ((control is EbRadioButton) && control.Name.Equals("eb_default"))
                {
                    if (WC == RoutingConstants.UC)
                    {
                        if (!(WebForm_L.UserObj.Roles.Contains(SystemRoles.SolutionOwner.ToString()) || WebForm_L.UserObj.Roles.Contains(SystemRoles.SolutionAdmin.ToString()) || WebForm_L.UserObj.Roles.Contains(SystemRoles.SolutionPM.ToString())))
                        {
                            control.IsDisable = true;
                            if (!string.IsNullOrWhiteSpace(control.EbSid_CtxId))
                                dataDict.TryAdd(control.EbSid_CtxId, control.IsDisable);
                        }
                    }
                }
                else if (control is EbUserSelect usrSelCtrl)
                {
                    usrSelCtrl.InitOptions(WebForm_L.SolutionObj.Users);
                    if (!string.IsNullOrWhiteSpace(usrSelCtrl.EbSid_CtxId))
                        dataDict.TryAdd(usrSelCtrl.EbSid_CtxId, usrSelCtrl.UserList);
                }
                else if (control is EbDGUserSelectColumn dgUsrSelCtrl)
                {
                    dgUsrSelCtrl.InitOptions(WebForm_L.SolutionObj.Users);
                    if (!string.IsNullOrWhiteSpace(dgUsrSelCtrl.EbSid_CtxId))
                        dataDict.TryAdd(dgUsrSelCtrl.EbSid_CtxId, dgUsrSelCtrl.UserList);
                }
                else if (control is EbTextBox txtCtrl)
                {
                    txtCtrl.InitFromDataBase(ServiceClient);
                    if (txtCtrl.AutoSuggestion && !string.IsNullOrWhiteSpace(txtCtrl.EbSid_CtxId))
                        dataDict.TryAdd(txtCtrl.EbSid_CtxId, txtCtrl.Suggestions);
                }
                else if (control is EbDGStringColumn dgTxtCtrl)
                {
                    dgTxtCtrl.InitFromDataBase(ServiceClient);
                    if (dgTxtCtrl.AutoSuggestion && !string.IsNullOrWhiteSpace(dgTxtCtrl.EbSid_CtxId))
                        dataDict.TryAdd(dgTxtCtrl.EbSid_CtxId, dgTxtCtrl.Suggestions);
                }
                else if (control is EbMeetingScheduler)
                {
                    //(control as EbMeetingScheduler).UsersList = WebForm.SolutionObj.Users;
                    (control as EbMeetingScheduler).InitParticipantsList(ServiceClient);
                }
                else if (control is EbInputGeoLocation geoLocCtrl)
                {
                    geoLocCtrl.GetDefaultApikey(ServiceClient);
                }
                else if (control is EbTagInput tagCtrl)
                {
                    tagCtrl.InitFromDataBase(ServiceClient);
                    if (tagCtrl.AutoSuggestion && !string.IsNullOrWhiteSpace(tagCtrl.EbSid_CtxId))
                        dataDict.TryAdd(tagCtrl.EbSid_CtxId, tagCtrl.Suggestions);
                }
                else if (control is EbQuestionnaireConfigurator qusCtrl)
                {
                    qusCtrl.InitFromDataBase(ServiceClient);
                }
                else if (control is EbRenderQuestionsControl qusrCtrl)
                {
                    qusrCtrl.InitFromDataBase(ServiceClient);
                }
            }
            return dataDict;
        }


        //change the redis get of soln and user objects if making this a common function
        public static InsertOrUpdateFormDataResp InsertOrUpdateFormData(InsertOrUpdateFormDataRqst request, IDatabase DataDB, Service Service,
            IRedisClient Redis, EbConnectionFactory ebConnectionFactory)
        {
            try
            {
                Console.WriteLine("InsertOrUpdateFormDataRqst Service start");
                EbWebForm FormObj = GetWebFormObject(request.RefId, request.UserAuthId, request.SolnId, Redis, Service, ebConnectionFactory, request.LocId);
                FormObj.TableRowId = request.RecordId;
                Console.WriteLine("InsertOrUpdateFormDataRqst PrepareWebFormData start : " + DateTime.Now);
                FormObj.PrepareWebFormData(DataDB, Service, request.PushJson, request.FormGlobals);
                Console.WriteLine("InsertOrUpdateFormDataRqst Save start : " + DateTime.Now);
                string r = FormObj.Save(DataDB, Service, request.TransactionConnection);
                Console.WriteLine("InsertOrUpdateFormDataRqst returning");
                return new InsertOrUpdateFormDataResp() { Status = (int)HttpStatusCode.OK, Message = "success", RecordId = FormObj.TableRowId };
            }
            catch (FormException ex)
            {
                Console.WriteLine("FormException in InsertOrUpdateFormDataRqst\nMessage : " + ex.Message + "\nMessageInternal : " + ex.MessageInternal + "\nStackTraceInternal : " + ex.StackTraceInternal + "\nStackTrace : " + ex.StackTrace);
                return new InsertOrUpdateFormDataResp() { Status = ex.ExceptionCode, Message = ex.Message };
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception in InsertOrUpdateFormDataRqst\nMessage" + ex.Message + "\nStackTrace" + ex.StackTrace);
                return new InsertOrUpdateFormDataResp() { Status = (int)HttpStatusCode.InternalServerError, Message = ex.Message };
            }
        }

        public static EbWebForm GetWebFormObject(string RefId, string UserAuthId, string SolnId, IRedisClient Redis, Service service,
            EbConnectionFactory EbConnectionFactory, int CurrrentLocation = 0)
        {
            EbWebForm _form = EbFormHelper.GetEbObject<EbWebForm>(RefId, null, Redis, service);
            _form.LocationId = CurrrentLocation;
            _form.SetRedisClient(Redis);
            _form.SetConnectionFactory(EbConnectionFactory);
            if (UserAuthId != null)
            {
                _form.UserObj = Redis.Get<User>(UserAuthId);
                if (_form.UserObj == null)
                    throw new Exception("User Object is null. AuthId: " + UserAuthId);
                if (_form.UserObj.Preference != null)
                    _form.UserObj.Preference.CurrrentLocation = CurrrentLocation;
            }
            if (SolnId != null)
            {
                _form.SolutionObj = Redis.Get<Eb_Solution>(String.Format("solution_{0}", SolnId));
                if (_form.SolutionObj == null)
                    throw new Exception("Solution Object is null. SolnId: " + SolnId);
                if (_form.SolutionObj.SolutionSettings == null)
                    _form.SolutionObj.SolutionSettings = new SolutionSettings() { SystemColumns = new EbSystemColumns(EbSysCols.Values) };
                else if (_form.SolutionObj.SolutionSettings.SystemColumns == null)
                    _form.SolutionObj.SolutionSettings.SystemColumns = new EbSystemColumns(EbSysCols.Values);
            }
            _form.AfterRedisGet_All(service);
            return _form;
        }

        public static InsertDataFromWebformResponse InsertDataFromWebform(InsertDataFromWebformRequest request, IRedisClient Redis, Service service,
            EbConnectionFactory EbConnectionFactory)
        {
            EbWebForm FormObj = null;
            try
            {
                Dictionary<string, string> MetaData = new Dictionary<string, string>();
                DateTime startdt = DateTime.Now;
                Console.WriteLine("Insert/Update WebFormData : start - " + startdt);
                FormObj = GetWebFormObject(request.RefId, request.UserAuthId, request.SolnId, Redis, service, EbConnectionFactory, request.CurrentLoc);
                //CheckDataPusherCompatibility(FormObj);
                FormObj.TableRowId = request.RowId;
                FormObj.FormData = JsonConvert.DeserializeObject<WebformData>(request.FormData);
                FormObj.DraftId = request.DraftId;
                //CheckForMyProfileForms(FormObj, request.WhichConsole, request.MobilePageRefId);

                Console.WriteLine("Insert/Update WebFormData : MergeFormData start - " + DateTime.Now);
                FormObj.MergeFormData();
                Console.WriteLine("Insert/Update WebFormData : Save start - " + DateTime.Now);
                string r = FormObj.Save(EbConnectionFactory, service, request.WhichConsole, request.MobilePageRefId);
                Console.WriteLine("Insert/Update WebFormData : AfterExecutionIfUserCreated start - " + DateTime.Now);
                //FormObj.AfterExecutionIfUserCreated(this, EbConnectionFactory.EmailConnection, MessageProducer3, request.WhichConsole, MetaData);
                Console.WriteLine("Insert/Update WebFormData end : Execution Time = " + (DateTime.Now - startdt).TotalMilliseconds);
                bool isMobInsert = request.WhichConsole == RoutingConstants.MC;

                return new InsertDataFromWebformResponse()
                {
                    Message = "Success",
                    RowId = FormObj.TableRowId,
                    FormData = isMobInsert ? null : JsonConvert.SerializeObject(FormObj.FormData),
                    RowAffected = 1,
                    AffectedEntries = r,
                    Status = (int)HttpStatusCode.OK,
                    MetaData = MetaData
                };
            }
            catch (FormException ex)
            {
                Console.WriteLine("FormException in Insert/Update WebFormData\nMessage : " + ex.Message + "\nMessageInternal : " + ex.MessageInternal + "\nStackTraceInternal : " + ex.StackTraceInternal + "\nStackTrace" + ex.StackTrace);

                //if (IsErrorDraftCandidate(request, FormObj))
                //    return FormDraftsHelper.SubmitErrorAndGetResponse(this.EbConnectionFactory.DataDB, FormObj, request, ex);

                return new InsertDataFromWebformResponse()
                {
                    Message = ex.Message,
                    Status = ex.ExceptionCode,
                    MessageInt = ex.MessageInternal,
                    StackTraceInt = ex.StackTraceInternal
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception in Insert/Update WebFormData\nMessage : " + ex.Message + "\nStackTrace : " + ex.StackTrace);

                //if (IsErrorDraftCandidate(request, FormObj))
                //    return FormDraftsHelper.SubmitErrorAndGetResponse(this.EbConnectionFactory.DataDB, FormObj, request, ex);

                return new InsertDataFromWebformResponse()
                {
                    Message = FormErrors.E0129 + ex.Message,
                    Status = (int)HttpStatusCode.InternalServerError,
                    MessageInt = "Exception in InsertDataFromWebform[service]",
                    StackTraceInt = ex.StackTrace
                };
            }
        }

        # region Form Submission Job ID

        public static int SetFsWebReceivedCxtId(IServiceClient ServiceClient, IRedisClient Redis, string SolnId, string RefId, int UserId, string fsCxtId, int RowId)
        {
            if (string.IsNullOrWhiteSpace(fsCxtId))
                return 0;

            int DataId = 0;
            string ObjVerId = RefId.Split("-")[4];
            string RedisKey = string.Format(RedisKeyPrefixConstants.FormSubmissionJobId, SolnId, ObjVerId, UserId, fsCxtId);
            FormSubmissionJobStatus status = Redis.Get<FormSubmissionJobStatus>(RedisKey);
            if (status == FormSubmissionJobStatus.Default)
            {
                Redis.Set(RedisKey, FormSubmissionJobStatus.WebReceived, new TimeSpan(0, 5, 0));
            }
            else if (status == FormSubmissionJobStatus.WebReceived || status == FormSubmissionJobStatus.SsReceived)
            {
                try
                {
                    LogEbErrorResponse Resp = ServiceClient.Post<LogEbErrorResponse>(new LogEbErrorRequest
                    {
                        Code = (int)EbErrorCode.DuplicateFormSubmission,
                        Title = "Duplicate Form Submission is in progress",
                        Message = $"Form Submission Context: {fsCxtId}, Status: {status}",
                        SourceId = 0,
                        SourceVerId = ObjVerId
                    });
                }
                catch (Exception ex)
                {

                }
                throw new FormException("This form submission is already in progress.", (int)HttpStatusCode.MethodNotAllowed, $"Form Submission Context: {fsCxtId}{status}", "WebCheck");
            }
            else if (status == FormSubmissionJobStatus.SsProcessed || status == FormSubmissionJobStatus.WebProcessed)
            {
                RedisKey = string.Format(RedisKeyPrefixConstants.FormSubmissionDataId, SolnId, ObjVerId, UserId, fsCxtId);
                DataId = Redis.Get<int>(RedisKey);
                try
                {
                    LogEbErrorResponse Resp = ServiceClient.Post<LogEbErrorResponse>(new LogEbErrorRequest
                    {
                        Code = (int)EbErrorCode.DuplicateFormSubmission,
                        Title = "Duplicate Form Submission is completed",
                        Message = $"Form Submission Context: {fsCxtId}, Status: {status}",
                        SourceId = DataId,
                        SourceVerId = ObjVerId
                    });
                }
                catch (Exception ex)
                {
                    throw new FormException("This form submission is already saved.", (int)HttpStatusCode.MethodNotAllowed, $"Form Submission Context: {fsCxtId}{status}", "WebCheck");
                }
            }
            return DataId;
        }

        public static void SetFsWebProcessedCxtId(IServiceClient ServiceClient, IRedisClient Redis, string SolnId, string RefId, int UserId, string fsCxtId, int RowId)
        {
            if (string.IsNullOrWhiteSpace(fsCxtId))
                return;

            string ObjVerId = RefId.Split("-")[4];
            string RedisKey = string.Format(RedisKeyPrefixConstants.FormSubmissionJobId, SolnId, ObjVerId, UserId, fsCxtId);
            FormSubmissionJobStatus status = Redis.Get<FormSubmissionJobStatus>(RedisKey);
            if (status == FormSubmissionJobStatus.Default || status == FormSubmissionJobStatus.SsProcessed)
            {
                Redis.Set(RedisKey, FormSubmissionJobStatus.WebProcessed, new TimeSpan(1, 0, 0));
            }
            else
            {
                try
                {
                    LogEbErrorResponse Resp = ServiceClient.Post<LogEbErrorResponse>(new LogEbErrorRequest
                    {
                        Code = (int)EbErrorCode.DuplicateFormSubmission,
                        Title = "Duplicate Form Submission is in invalid state",
                        Message = $"Form Submission Context: {fsCxtId}, Status: {status}",
                        SourceId = 0,
                        SourceVerId = ObjVerId
                    });
                }
                catch (Exception ex)
                {

                }
                throw new FormException("Invalid status of form submission job. Please refresh and try again.", (int)HttpStatusCode.MethodNotAllowed, $"Form Submission Context: {fsCxtId}{status}", "WebCheck");
            }
        }

        public static void SetFsSsReceivedCxtId(IRedisClient Redis, string SolnId, string RefId, int UserId, string fsCxtId, int RowId)
        {
            if (string.IsNullOrWhiteSpace(fsCxtId))
                return;

            string ObjVerId = RefId.Split("-")[4];
            string RedisKey = string.Format(RedisKeyPrefixConstants.FormSubmissionJobId, SolnId, ObjVerId, UserId, fsCxtId);
            FormSubmissionJobStatus status = Redis.Get<FormSubmissionJobStatus>(RedisKey);
            if (status == FormSubmissionJobStatus.Default || status == FormSubmissionJobStatus.WebReceived)
            {
                Redis.Set(RedisKey, FormSubmissionJobStatus.SsReceived, new TimeSpan(0, 5, 0));//form save timeout is 2 min
            }
            else if (status == FormSubmissionJobStatus.SsReceived)
            {
                throw new FormException("This form submission is already in progress.", (int)HttpStatusCode.MethodNotAllowed, $"Form Submission Context: {fsCxtId}{status}", "SsCheck");
            }
            else if (status == FormSubmissionJobStatus.SsProcessed || status == FormSubmissionJobStatus.WebProcessed)
            {
                throw new FormException("This form submission is already saved.", (int)HttpStatusCode.MethodNotAllowed, $"Form Submission Context: {fsCxtId}{status}", "SsCheck");
            }
        }

        public static void SetFsSsProcessedCxtId(IRedisClient Redis, string SolnId, string RefId, int UserId, string fsCxtId, int RowId, int NewRowId)
        {
            if (string.IsNullOrWhiteSpace(fsCxtId))
                return;

            string ObjVerId = RefId.Split("-")[4];
            string RedisKey = string.Format(RedisKeyPrefixConstants.FormSubmissionJobId, SolnId, ObjVerId, UserId, fsCxtId);
            FormSubmissionJobStatus status = Redis.Get<FormSubmissionJobStatus>(RedisKey);
            if (status == FormSubmissionJobStatus.Default || status == FormSubmissionJobStatus.SsReceived)
            {
                Redis.Set(RedisKey, FormSubmissionJobStatus.SsProcessed, new TimeSpan(1, 0, 0));
                RedisKey = string.Format(RedisKeyPrefixConstants.FormSubmissionDataId, SolnId, ObjVerId, UserId, fsCxtId);
                Redis.Set(RedisKey, NewRowId, new TimeSpan(1, 0, 0));
            }
            //else
            //{
            //    throw new FormException("Invalid status of form submission job. Please refresh and try again.", (int)HttpStatusCode.MethodNotAllowed, $"Form Submission Context: {fsCxtId}{status}", "SsCheck");
            //}
        }

        public static void ReSetFormSubmissionCxtId(IRedisClient Redis, string SolnId, string RefId, int UserId, string fsCxtId, int RowId)
        {
            if (string.IsNullOrWhiteSpace(fsCxtId))
                return;

            string ObjVerId = RefId.Split("-")[4];
            string RedisKey = string.Format(RedisKeyPrefixConstants.FormSubmissionJobId, SolnId, ObjVerId, UserId, fsCxtId);
            Redis.Set(RedisKey, FormSubmissionJobStatus.Default, new TimeSpan(0, 0, 30));
        }

        public static void LogEbError(IDatabase DataDB, int Code, string Title, string Message, int SourceId, string SourceVerId, int UserId)
        {
            Task.Run(() =>
            {
                try
                {
                    Console.WriteLine("LogEbError Async start");

                    string Qry = $@"
INSERT INTO eb_errors (code, title, message, eb_src_id, eb_src_ver_id, eb_created_by, eb_created_at,eb_del)
VALUES({Code}, '{Title}', '{Message}', {SourceId}, {SourceVerId}, {UserId}, {DataDB.EB_CURRENT_TIMESTAMP}, 'F');";

                    int temp = DataDB.DoNonQuery(Qry);

                    Console.WriteLine("LogEbError Async end: " + temp);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Exception in LogEbError. Message: {ex.Message}\nStackTrace: {ex.StackTrace}");
                }
            });
        }

        #endregion
    }

    public class EbColumnExtra
    {
        public static Dictionary<string, EbDbTypes> Params
        {
            get
            {
                return new Dictionary<string, EbDbTypes> {
                    { "eb_row_num",EbDbTypes.Int32},
                    { SystemColumns.eb_created_at_device,EbDbTypes.DateTime},
                    { SystemColumns.eb_device_id,EbDbTypes.String},
                    { SystemColumns.eb_appversion,EbDbTypes.String},
                    { "eb_created_aid", EbDbTypes.Int32},
                    { SystemColumns.eb_created_at_pos,EbDbTypes.DateTime},
                    { SystemColumns.eb_created_by_pos,EbDbTypes.Int32}
                };
            }
        }
    }

    public class EbSignUpUserInfo
    {
        public string AuthId { get; set; }
        public string UserName { get; set; }
        public int UserType { get; set; }
        public bool VerificationRequired { get; set; }
        public string VerifyEmail { get; set; }
        public string VerifyPhone { get; set; }
        public string Message { get; set; }
        public string Token { get; set; }
    }

    public class EbFormAndDataWrapper
    {
        public string RefId { get; set; }
        public int RenderMode { get; set; }
        public int RowId { get; set; }
        public int DraftId { get; set; }
        public string DraftInfo { get; set; }
        public string Draft_FormData { get; set; }
        public string Mode { get; set; }
        public string FormDataWrap { get; set; }
        public Dictionary<int, List<int>> FormPermissions { get; set; }
        public string WebFormHtml { get; set; }
        public string WebFormObj { get; set; }
        public string WebFormObjJsUrl { get; set; }
        public Dictionary<string, string> DisableEditButton { get; set; }
        public bool IsPartial { get; set; }//can avoid last
        public Dictionary<string, object> RelatedData { get; set; }
        public bool DisableLocCheck { get; set; }

        public string HtmlHead { get; set; }//

        public string Url { get; set; }
        public int ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
        public string Message { get; set; }
    }

}
