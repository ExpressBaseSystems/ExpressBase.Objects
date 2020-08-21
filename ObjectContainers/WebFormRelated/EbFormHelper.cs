using ExpressBase.Common;
using ExpressBase.Common.Constants;
using ExpressBase.Common.Data;
using ExpressBase.Common.Extensions;
using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using ExpressBase.Common.Structures;
using ExpressBase.Objects.Objects;
using ExpressBase.Objects.ServiceStack_Artifacts;
using ExpressBase.Objects.WebFormRelated;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Newtonsoft.Json;
using ServiceStack;
using ServiceStack.Redis;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Reflection;
using System.Text;

namespace ExpressBase.Objects
{
    public static class EbFormHelper
    {
        public static List<string> DiscoverRelatedRefids(EbControlContainer _this)
        {
            List<string> refids = new List<string>();
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
                            _flatCtrls[i].RefId = "failed-to-update-";
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
                                            _prop.SetValue(_flatCtrls[i], "failed-to-update-");
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
                                                info.ObjRefId = "failed-to-update-";
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
                    c.IsDynamicTabChild = _this.IsDynamicTabChild;
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
                        if (c is EbTabPane && (c as EbTabPane).IsDynamic)
                            c.IsDynamicTabChild = true;
                        AfterRedisGet(c as EbControlContainer, Redis, client, null);
                    }
                    else if (c is EbProvisionLocation)//add unmapped ctrls as DoNotPersist controls
                    {
                        if (_this.IsRenderMode)
                        {
                            EbProvisionLocation prvnCtrl = c as EbProvisionLocation;
                            for (int j = 0; j < prvnCtrl.Fields.Count; j++)
                            {
                                UsrLocField prvnFld = prvnCtrl.Fields[j] as UsrLocField;
                                if (prvnFld.ControlName.IsNullOrEmpty() && prvnFld.IsRequired)
                                {
                                    if (prvnFld.Type == "image")
                                    {
                                        _this.Controls.Insert(i, new EbDisplayPicture()
                                        {
                                            Name = "namecustom" + i,
                                            EbSid = "ebsidcustom" + i,
                                            EbSid_CtxId = "ebsidcustom" + i,
                                            Label = prvnFld.DisplayName,
                                            DoNotPersist = true,
                                            MaxHeight = 100
                                        });
                                    }
                                    else
                                    {
                                        _this.Controls.Insert(i, new EbTextBox()
                                        {
                                            Name = "namecustom" + i,
                                            EbSid = "ebsidcustom" + i,
                                            EbSid_CtxId = "ebsidcustom" + i,
                                            Label = prvnFld.DisplayName,
                                            DoNotPersist = true
                                        });
                                    }
                                    prvnFld.ControlName = "namecustom" + i;
                                    i++;
                                }
                            }
                        }
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
            _this.FormCollection = new EbWebFormCollection(_this);
            if (_this.DataPushers != null)
            {
                foreach (EbDataPusher pusher in _this.DataPushers)
                {
                    EbWebForm _form = GetEbObject<EbWebForm>(pusher.FormRefId, client, Redis, service);
                    _form.RefId = pusher.FormRefId;
                    _form.UserObj = _this.UserObj;
                    _form.SolutionObj = _this.SolutionObj;
                    _form.AfterRedisGet(Redis as RedisClient, client);
                    _form.DataPusherConfig = new EbDataPusherConfig { SourceTable = _this.FormSchema.MasterTable, MultiPushId = _this.RefId + "_" + pusher.Name };
                    pusher.WebForm = _form;
                    _this.FormCollection.Add(_form);
                    _this.ExeDataPusher = true;
                }
            }
        }

        public static T GetEbObject<T>(string RefId, IServiceClient ServiceClient, IRedisClient Redis, Service Service)
        {
            if (string.IsNullOrEmpty(RefId))
                throw new Exception("RefId is null or empty. FormHelper >GetEbObject. Type: " + typeof(T).Name);

            T _ebObject = default;
            if (Redis != null) _ebObject = Redis.Get<T>(RefId);

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
                if (Redis != null) Redis.Set<T>(RefId, _ebObject);
            }
            return _ebObject;
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
                { tableName + FormConstants._id},
                { FormConstants.id}
            };
            return _params.Contains(paramName);
        }

        //Create a NEW WebFormData version from EDIT mode WebFormData of 'same' form.
        public static WebformData GetFilledNewFormData(EbWebForm FormSrc)// FormSrc = Source Form
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
                            SingleColumn c = Table[0].Columns.Find(e => e.Control is EbAutoId);
                            if (c != null) c.Value = null;
                            foreach (SingleColumn c_ in Table[0].Columns.FindAll(e => e.Control?.IsSysControl == true))
                            {
                                SingleColumn t = c_.Control.GetSingleColumn(FormSrc.UserObj, FormSrc.SolutionObj, null);
                                c_.Value = t.Value; c_.F = t.F;
                            }
                            c = Table[0].Columns.Find(e => e.Name == FormConstants.id);
                            if (c != null) c.Value = 0;
                            Table[0].RowId = 0;
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
        public static void CopyFormDataToFormData(IDatabase DataDB, EbWebForm FormSrc, EbWebForm FormDes, Dictionary<EbControl, string> psDict, List<DbParameter> psParams)
        {
            foreach (TableSchema _tableDes in FormDes.FormSchema.Tables)
            {
                if (_tableDes.TableType == WebFormTableTypes.Grid)
                {
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
                                    RowDes.SetColumn(_columnDes.ColumnName, _columnDes.Control.GetSingleColumn(FormDes.UserObj, FormDes.SolutionObj, ColumnSrc.Value));
                                    string _formattedData = Convert.ToString(ColumnSrc.Value);
                                    if (_columnDes.Control is EbDGPowerSelectColumn && !string.IsNullOrEmpty(_formattedData))
                                    {
                                        if (psDict.ContainsKey(_columnDes.Control))
                                            psDict[_columnDes.Control] += CharConstants.COMMA + _formattedData;
                                        else
                                            psDict.Add(_columnDes.Control, _formattedData);
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
                        foreach (ColumnSchema _columnDes in _tableDes.Columns)
                        {
                            ColumnSrc = FormSrc.FormData.MultipleTables[FormSrc.FormData.MasterTable][0].GetColumn(_columnDes.ColumnName);
                            if (ColumnSrc != null && !(_columnDes.Control is EbAutoId) && !_columnDes.Control.IsSysControl)
                            {
                                FormDes.FormData.MultipleTables[_tableDes.TableName][0].SetColumn(_columnDes.ColumnName, _columnDes.Control.GetSingleColumn(FormDes.UserObj, FormDes.SolutionObj, ColumnSrc.Value));
                                _formattedData = Convert.ToString(ColumnSrc.Value);
                            }
                            else
                                _formattedData = Convert.ToString(FormDes.FormData.MultipleTables[_tableDes.TableName][0][_columnDes.ColumnName]);
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
                        if (ColumnSrc != null && !(_column.Control is EbAutoId) && !_column.Control.IsSysControl)
                        {
                            entry.Value[0].SetColumn(_column.ColumnName, _column.Control.GetSingleColumn(FormDes.UserObj, FormDes.SolutionObj, ColumnSrc.Value));
                            _formattedData = Convert.ToString(ColumnSrc.Value);
                            if (_column.Control is EbPowerSelect && !string.IsNullOrEmpty(_formattedData))
                            {
                                if (psDict.ContainsKey(_column.Control))
                                    psDict[_column.Control] += CharConstants.COMMA + _formattedData;
                                else
                                    psDict.Add(_column.Control, _formattedData);
                            }
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
                                RowDes.SetColumn(_column.ColumnName, _column.Control.GetSingleColumn(FormDes.UserObj, FormDes.SolutionObj, ColumnSrc.Value));
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
            List<string> urlParts = new List<string>();

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
                {
                    param.Value = Convert.ToString(Column.Value);
                    urlParts.Add(param.Name + "=" + Column.Value);
                }
                else
                    Console.WriteLine("Api parameter not found in webformdata: " + param.Name);
            }
            try
            {
                ApiConversionResponse apiResp = service.Gateway.Send<ApiConversionResponse>(new ApiConversionRequest
                {
                    Url = ApiUrl + "?" + string.Join('&', urlParts),
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
    }

    public class EbColumnExtra
    {
        public static Dictionary<string, EbDbTypes> Params
        {
            get
            {
                return new Dictionary<string, EbDbTypes> {
                    { "eb_row_num",EbDbTypes.Decimal},
                    { "eb_created_at_device",EbDbTypes.DateTime},
                    { "eb_device_id",EbDbTypes.String},
                    { "eb_appversion",EbDbTypes.String},
                    { "eb_created_aid", EbDbTypes.Int32}
                };
            }
        }
    }
}
