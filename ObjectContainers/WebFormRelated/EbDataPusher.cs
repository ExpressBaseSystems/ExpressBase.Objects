using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using ExpressBase.Common.Structures;
using ExpressBase.Common.Extensions;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using ExpressBase.Common;
using System;
using ExpressBase.Objects.ServiceStack_Artifacts;
using System.Text.RegularExpressions;
using System.Net;
using ExpressBase.Objects.WebFormRelated;
using ExpressBase.Common.Constants;
using ServiceStack;
using ExpressBase.CoreBase.Globals;
using System.Data.Common;
using Newtonsoft.Json;

namespace ExpressBase.Objects
{
    [UsedWithTopObjectParent(typeof(EbObject))]
    [EnableInBuilder(BuilderType.WebForm)]
    [HideInPropertyGrid]
    public class EbDataPusher
    {
        public EbDataPusher() { }

        [PropertyEditor(PropertyEditorType.ObjectSelector)]
        [EnableInBuilder(BuilderType.WebForm)]
        [OSE_ObjectTypes(EbObjectTypes.iWebForm)]
        public virtual string FormRefId { get; set; }

        [PropertyEditor(PropertyEditorType.String)]
        [EnableInBuilder(BuilderType.WebForm)]
        public string Json { get; set; }

        [JsonIgnore]
        public string ProcessedJson { get; set; }

        [HideInPropertyGrid]
        [EnableInBuilder(BuilderType.WebForm)]
        public string EbSid { get; set; }

        [EnableInBuilder(BuilderType.WebForm)]
        public virtual string Name { get; set; }

        [PropertyEditor(PropertyEditorType.String)]
        [EnableInBuilder(BuilderType.WebForm)]
        public string PushOnlyIf { get; set; }

        [PropertyEditor(PropertyEditorType.String)]
        [EnableInBuilder(BuilderType.WebForm)]
        public virtual string SkipLineItemIf { get; set; }

        public EbWebForm WebForm { get; set; }
    }

    [Alias("Webform")]
    [EnableInBuilder(BuilderType.WebForm)]
    public class EbFormDataPusher : EbDataPusher
    {
        public EbFormDataPusher() { }

        [EnableInBuilder(BuilderType.WebForm)]
        [Alias("Multi push id")]
        public override string Name { get; set; }

        [OnChangeExec(@"
if (this.MultiPushIdType === 0 || this.MultiPushIdType === 1)
    pg.ShowProperty('Name');
else if (this.MultiPushIdType === 2)
    pg.HideProperty('Name');
")]
        [EnableInBuilder(BuilderType.WebForm)]
        public MultiPushIdTypes MultiPushIdType { get; set; }

        #region commented for backward compatibility
        //[PropertyEditor(PropertyEditorType.ObjectSelector)]
        //[EnableInBuilder(BuilderType.WebForm)]
        //[OSE_ObjectTypes(EbObjectTypes.iWebForm)]
        //public string FormRefId { get; set; }

        //[PropertyEditor(PropertyEditorType.String)]
        //[EnableInBuilder(BuilderType.WebForm)]
        //public string SkipLineItemIf { get; set; }

        //public EbWebForm WebForm { get; set; }
        #endregion
    }

    [Alias("Internal API")]
    [EnableInBuilder(BuilderType.WebForm)]
    public class EbApiDataPusher : EbDataPusher
    {
        public EbApiDataPusher() { }

        public override string FormRefId { get; set; }
        public override string SkipLineItemIf { get; set; }

        [PropertyEditor(PropertyEditorType.ObjectSelector)]
        [EnableInBuilder(BuilderType.WebForm)]
        [OSE_ObjectTypes(EbObjectTypes.iApi)]
        public string ApiRefId { get; set; }
    }

    public class EbDataPusherConfig
    {
        public EbDataPusherConfig() { }

        public string SourceTable { get; set; }

        public string MultiPushId { get; set; }

        public bool AllowPush { get; set; }

        public int SourceRecId { get; set; }
    }

    public class EbDataPushHelper
    {
        public EbWebForm WebForm { get; set; }

        public Dictionary<int, string> CodeDict { get; set; }

        public EbDataPushHelper(EbWebForm WebForm)
        {
            this.WebForm = WebForm;
        }

        public void CreateWebFormData(object out_dict)
        {
            Dictionary<int, object[]> OutputDict = (Dictionary<int, object[]>)out_dict;
            int Index = 1;

            foreach (EbDataPusher pusher in this.WebForm.DataPushers)
            {
                if (pusher is EbApiDataPusher)
                    continue;
                pusher.WebForm.DataPusherConfig.SourceRecId = this.WebForm.TableRowId;
                pusher.WebForm.RefId = pusher.FormRefId;
                pusher.WebForm.UserObj = this.WebForm.UserObj;
                pusher.WebForm.LocationId = this.WebForm.LocationId;
                pusher.WebForm.SolutionObj = this.WebForm.SolutionObj;
                pusher.WebForm.FormData = new WebformData() { MasterTable = pusher.WebForm.FormSchema.MasterTable };

                JObject JObj = JObject.Parse(pusher.Json);

                foreach (TableSchema _table in pusher.WebForm.FormSchema.Tables)
                {
                    if (JObj[_table.TableName] != null)
                    {
                        SingleTable Table = new SingleTable();
                        foreach (JToken jRow in JObj[_table.TableName])
                        {
                            SingleRow Row = new SingleRow() { RowId = 0 };
                            foreach (ColumnSchema _column in _table.Columns)
                            {
                                object val = null;
                                if (jRow[_column.ColumnName] != null)
                                    val = this.GetValueFormOutDict(OutputDict, ref Index);

                                if (this.WebForm.AutoId != null && Convert.ToString(val) == FormConstants.AutoId_PlaceHolder)
                                {
                                    val = $"(SELECT {this.WebForm.AutoId.Name} FROM {this.WebForm.AutoId.TableName} WHERE {(this.WebForm.AutoId.TableName == this.WebForm.TableName ? string.Empty : (this.WebForm.TableName + CharConstants.UNDERSCORE))}id = (SELECT(eb_currval('{this.WebForm.TableName}_id_seq'))))";
                                    if (_column.Control is EbAutoId)
                                        (_column.Control as EbAutoId).BypassParameterization = true;
                                    else if (_column.Control is EbTextBox)
                                        (_column.Control as EbTextBox).BypassParameterization = true;
                                    else
                                        val = string.Empty;
                                }

                                Row.Columns.Add(new SingleColumn
                                {
                                    Name = _column.ColumnName,
                                    Type = _column.EbDbType,
                                    Value = val
                                });
                            }
                            if (_table.TableType == WebFormTableTypes.Grid && !string.IsNullOrEmpty(pusher.SkipLineItemIf))
                            {
                                object status = this.GetValueFormOutDict(OutputDict, ref Index);
                                if (Convert.ToBoolean(status))
                                    continue;
                            }
                            Table.Add(Row);
                        }
                        pusher.WebForm.FormData.MultipleTables.Add(_table.TableName, Table);
                    }
                }

                pusher.WebForm.DataPusherConfig.AllowPush = true;
                if (!string.IsNullOrWhiteSpace(pusher.PushOnlyIf))
                {
                    object status = this.GetValueFormOutDict(OutputDict, ref Index);
                    if (!Convert.ToBoolean(status))
                        pusher.WebForm.DataPusherConfig.AllowPush = false;
                }

                pusher.WebForm.MergeFormData();

                if (this.WebForm.TableRowId > 0)//if edit mode then fill or map the id by refering FormDataBackup
                {
                    if (pusher.WebForm.DataPusherConfig.AllowPush)
                    {
                        if (pusher.WebForm.FormDataBackup != null)
                        {
                            foreach (KeyValuePair<string, SingleTable> entry in pusher.WebForm.FormDataBackup.MultipleTables)
                            {
                                if (pusher.WebForm.FormData.MultipleTables.ContainsKey(entry.Key))
                                {
                                    for (int i = 0; i < entry.Value.Count; i++)
                                    {
                                        if (i < pusher.WebForm.FormData.MultipleTables[entry.Key].Count)
                                            pusher.WebForm.FormData.MultipleTables[entry.Key][i].RowId = entry.Value[i].RowId;
                                        else
                                        {
                                            pusher.WebForm.FormData.MultipleTables[entry.Key].Add(entry.Value[i]);
                                            pusher.WebForm.FormData.MultipleTables[entry.Key][i].IsDelete = true;
                                        }
                                    }
                                }
                                else
                                {
                                    pusher.WebForm.FormData.MultipleTables.Add(entry.Key, entry.Value);
                                    foreach (SingleRow Row in pusher.WebForm.FormData.MultipleTables[entry.Key])
                                        Row.IsDelete = true;
                                }
                            }
                        }
                    }
                    else
                    {
                        pusher.WebForm.FormData = pusher.WebForm.FormDataBackup;
                        foreach (KeyValuePair<string, SingleTable> entry in pusher.WebForm.FormData.MultipleTables)
                        {
                            foreach (SingleRow Row in entry.Value)
                                Row.IsDelete = true;
                        }
                    }
                }
            }
        }

        private object GetValueFormOutDict(Dictionary<int, object[]> OutDict, ref int Index)
        {
            int StopCounter = 500;// to avoid infinite loop in case of any unexpected error/exception
            //assuming that maximum cs expressions in a data pusher is 500
            while (StopCounter > 0 && !OutDict.ContainsKey(Index))
            {
                StopCounter--;
                Index++;
            }
            if (!OutDict.ContainsKey(Index))
                throw new FormException("Exception in C# code evaluation", (int)HttpStatusCode.InternalServerError, "Malformed OutputDict from combined cs script evaluation", "Stopped by StopCounter500");

            if (Convert.ToInt32(OutDict[Index][0]) == 1)// 1 = success, 2 = exception
            {
                return OutDict[Index++][1];
            }
            throw new FormException("Exception in C# code evaluation", (int)HttpStatusCode.InternalServerError, $"{OutDict[Index][1]} \n C# code : {CodeDict[Index]}", "");
        }

        //Combining all c sharp scripts to be executed as a single program
        public string GetProcessedSingleCode()
        {
            this.CodeDict = new Dictionary<int, string>();
            string FnDef = string.Empty, FnCall = string.Empty;
            int Index = 1; //Code index
            int formIdx = 0; //form index
            foreach (EbDataPusher pusher in this.WebForm.DataPushers)
            {
                if (pusher is EbApiDataPusher)
                    continue;

                FnCall += $"\ndestinationform = slaveForms[{formIdx++}];";

                JObject JObj = JObject.Parse(pusher.Json);

                foreach (TableSchema _table in pusher.WebForm.FormSchema.Tables)
                {
                    if (JObj[_table.TableName] != null)
                    {
                        foreach (JToken jRow in JObj[_table.TableName])
                        {
                            string DgName = null;
                            if (_table.TableType == WebFormTableTypes.Grid)
                            {
                                FnCall += $"\ndestinationform.UpdateCurrentRowOfDG(\"{_table.ContainerName}\");";
                                DgName = _table.ContainerName;
                            }
                            foreach (ColumnSchema _column in _table.Columns)
                            {
                                if (jRow[_column.ColumnName] != null)
                                {
                                    this.CodeDict.Add(Index, jRow[_column.ColumnName].ToString());
                                    FnDef += GetFunctionDefinition(jRow[_column.ColumnName].ToString(), Index);
                                    FnCall += GetFunctionCall_NEW(Index, _column.ColumnName, DgName);
                                    Index++;
                                }
                            }
                            if (_table.TableType == WebFormTableTypes.Grid && !string.IsNullOrEmpty(pusher.SkipLineItemIf))
                            {
                                this.CodeDict.Add(Index, pusher.SkipLineItemIf);
                                FnDef += GetFunctionDefinition(pusher.SkipLineItemIf, Index);
                                FnCall += GetFunctionCall(Index);
                                Index++;
                            }
                        }
                    }
                }

                if (!string.IsNullOrEmpty(pusher.PushOnlyIf))
                {
                    this.CodeDict.Add(Index, pusher.PushOnlyIf);
                    FnDef += GetFunctionDefinition(pusher.PushOnlyIf, Index);
                    FnCall += GetFunctionCall(Index);
                    Index++;
                }
            }
            if (FnDef == string.Empty)
                return string.Empty;

            return FnDef + "Dictionary<int, object[]> out_dict = new Dictionary<int, object[]>();\n" + FnCall + "return out_dict;";
        }

        private string GetFunctionDefinition(string Code, int Index)
        {

            return $@"
public object fn_{Index}() 
{{ 
    {(Regex.IsMatch(Code, @"\breturn\b") ? string.Empty : "return ")} {Code} ;
}}".RemoveCR() + "\n";
        }

        private string GetFunctionCall(int Index)
        {
            string s = $@"
try 
{{ 
    out_dict.Add({Index}, new object[]{{ 1, fn_{Index}()}}); 
}} 
catch (Exception e) 
{{
    out_dict.Add({Index}, new object[]{{ 2, e.Message}}); 
}}";
            return s;
        }

        private string GetFunctionCall_NEW(int Index, string CtrlName, string DgName)
        {
            string code = string.Format(@"
try
{{
    var res{0} = fn_{0}();
    destinationform.{1}.setValue(res{0});
    out_dict.Add({0}, new object[]{{ 1, res{0}}});
}}
catch (Exception e)
{{
    out_dict.Add({0}, new object[]{{ 2, e.Message}});
}}
",
Index,
DgName == null ? CtrlName : $"{DgName}.currentRow[\"{CtrlName}\"]");

            return code;
        }

        private string GetWrappedFnCall(int Index, bool TrueContinue)
        {
            string s = $@"if (Convert.ToInt32(out_dict[{Index}][0]) == 1 && bool.TryParse(Convert.ToString(out_dict[{Index}][1]), out bool temp_{Index}) && temp_{Index} == {(TrueContinue ? "true" : "false")}) 
{{
@InnerCode@ 
}}";
            return s;
        }

        #region _______________Excel_Import_______________

        public string GetPusherJson(EbDataTable Data)
        {
            JObject Obj = new JObject();

            foreach (TableSchema _table in this.WebForm.FormSchema.Tables)
            {
                JObject o = new JObject();
                foreach (ColumnSchema _column in _table.Columns)
                {
                    if (Data.Columns.Find(e => e.ColumnName == _column.ColumnName && e.TableName == _table.TableName) != null)
                        o[_column.ColumnName] = "parameters." + _table.TableName + "." + _column.ColumnName;
                }
                if (o.Count > 0)
                {
                    JArray array = new JArray();
                    array.Add(o);
                    Obj[_table.TableName] = array;
                }
            }
            return Obj.ToString();
        }

        public string GetProcessedCode(string Json)
        {
            int Index = 1;
            string FnDef = string.Empty, FnCall = string.Empty;
            Dictionary<int, string> _codeDict = new Dictionary<int, string>();
            JObject JObj = JObject.Parse(Json);
            foreach (TableSchema _table in this.WebForm.FormSchema.Tables)
            {
                if (JObj[_table.TableName] != null)
                {
                    foreach (JToken jRow in JObj[_table.TableName])
                    {
                        foreach (ColumnSchema _column in _table.Columns)
                        {
                            if (jRow[_column.ColumnName] != null)
                            {
                                _codeDict.Add(Index, jRow[_column.ColumnName].ToString());
                                FnDef += GetFunctionDefinition(jRow[_column.ColumnName].ToString(), Index);
                                FnCall += GetFunctionCall(Index);
                                Index++;
                            }
                        }
                    }
                }
            }

            if (FnDef == string.Empty)
                return string.Empty;

            return FnDef + "Dictionary<int, object[]> out_dict = new Dictionary<int, object[]>();\n" + FnCall + "return out_dict;";
        }

        public void CreateWebFormData_Demo(object out_dict, string Json)
        {
            Dictionary<int, object[]> OutputDict = (Dictionary<int, object[]>)out_dict;
            int Index = 1;

            this.WebForm.FormData = new WebformData() { MasterTable = this.WebForm.FormSchema.MasterTable };
            JObject JObj = JObject.Parse(Json);

            foreach (TableSchema _table in this.WebForm.FormSchema.Tables)
            {
                if (JObj[_table.TableName] != null)
                {
                    SingleTable Table = new SingleTable();
                    foreach (JToken jRow in JObj[_table.TableName])
                    {
                        SingleRow Row = new SingleRow() { RowId = 0 };
                        foreach (ColumnSchema _column in _table.Columns)
                        {
                            object val = null;
                            if (jRow[_column.ColumnName] != null)
                                val = this.GetValueFormOutDict(OutputDict, ref Index);

                            Row.Columns.Add(new SingleColumn
                            {
                                Name = _column.ColumnName,
                                Type = _column.EbDbType,
                                Value = val
                            });
                        }
                        Table.Add(Row);
                    }
                    this.WebForm.FormData.MultipleTables.Add(_table.TableName, Table);
                }
            }
            this.WebForm.MergeFormData();
        }

        #endregion Excel_Import

        #region _____________Api_data_pusher_____________

        public enum DataPusherLogStatus
        {
            Success,
            Failed,
            RetryFailed,
            RetrySuccess,
            InternalError
        }

        private void PreprocessJson(EbDataPusher pusher)
        {
            JToken JTok = JToken.Parse(pusher.Json);
            this.PreprocessJsonRec(JTok);
            pusher.ProcessedJson = JTok.ToString();
        }

        private void PreprocessJsonRec(JToken JTok)
        {
            if (JTok is JObject)
            {
                JObject jObj = JTok as JObject;
                foreach (KeyValuePair<string, JToken> jObjEntry in jObj)
                {
                    if (jObjEntry.Value is JObject || jObjEntry.Value is JArray)
                        PreprocessJsonRec(jObjEntry.Value);
                }
            }
            else if (JTok is JArray)
            {
                JArray jArr = JTok as JArray;

                for (int i = 0; i < jArr.Count; i++)
                {
                    if (jArr[i] is JObject)
                    {
                        JObject candObj = jArr[i] as JObject;

                        if (candObj.TryGetValue(FormConstants.__eb_loop_through, out JToken _val))
                        {
                            string _co = _val.ToString();
                            int dupliCount = 0;
                            foreach (TableSchema _table in this.WebForm.FormSchema.Tables.FindAll(e => e.TableType == WebFormTableTypes.Grid))
                            {
                                if (_co.Contains($"form.{_table.ContainerName}.GetEnumerator()"))
                                {
                                    if (this.WebForm.FormData.MultipleTables.ContainsKey(_table.TableName))
                                        dupliCount = this.WebForm.FormData.MultipleTables[_table.TableName].Count;
                                    break;
                                }
                            }
                            if (dupliCount == 0)
                            {
                                candObj.Remove();
                                i--;
                            }
                            else
                            {
                                for (int j = 1; j < dupliCount; j++)
                                    candObj.AddAfterSelf(candObj.DeepClone());
                                i = i + dupliCount - 1;
                            }
                        }
                    }
                }

                foreach (JToken jt in jArr)
                    PreprocessJsonRec(jt);
            }
        }

        public string GetProcessedCode()
        {
            this.CodeDict = new Dictionary<int, string>();
            string FnDef = string.Empty, FnCall = string.Empty;
            int Index = 1;
            foreach (EbApiDataPusher pusher in this.WebForm.DataPushers.FindAll(e => e is EbApiDataPusher))
            {
                string PusherWrapIf = string.Empty, PusherFnCall = string.Empty;
                if (!string.IsNullOrEmpty(pusher.PushOnlyIf))
                {
                    this.CodeDict.Add(Index, pusher.PushOnlyIf);
                    FnDef += GetFunctionDefinition(pusher.PushOnlyIf, Index);
                    FnCall += GetFunctionCall(Index);
                    PusherWrapIf = GetWrappedFnCall(Index, true);
                    Index++;
                }

                this.PreprocessJson(pusher);
                JToken JTok = JToken.Parse(pusher.ProcessedJson);
                GetFnDefinitionRec(JTok, ref FnDef, ref PusherFnCall, ref Index);

                if (PusherWrapIf == string.Empty)
                    FnCall += PusherFnCall;
                else
                    FnCall += PusherWrapIf.Replace("@InnerCode@", PusherFnCall);
            }
            if (FnDef == string.Empty)
                return string.Empty;

            return FnDef + "Dictionary<int, object[]> out_dict = new Dictionary<int, object[]>();\n" + FnCall + "return out_dict;";
        }

        private void GetFnDefinitionRec(JToken JTok, ref string FnDef, ref string PusherFnCall, ref int Index)
        {
            if (JTok is JObject)
            {
                JObject jObj = JTok as JObject;
                foreach (KeyValuePair<string, JToken> jObjEntry in jObj)
                {
                    if (jObjEntry.Value is JObject || jObjEntry.Value is JArray)
                        GetFnDefinitionRec(jObjEntry.Value, ref FnDef, ref PusherFnCall, ref Index);
                    else
                    {
                        this.CodeDict.Add(Index, jObjEntry.Value.ToString());
                        FnDef += GetFunctionDefinition(jObjEntry.Value.ToString(), Index);
                        PusherFnCall += GetFunctionCall(Index);
                        Index++;
                    }
                }
            }
            else if (JTok is JArray)
            {
                JArray jArr = JTok as JArray;
                foreach (JToken jt in jArr)
                    GetFnDefinitionRec(jt, ref FnDef, ref PusherFnCall, ref Index);
            }
        }

        private void FillJsonWithValuesRec(JToken JTok, Dictionary<int, object[]> OutputDict, ref int Index)
        {
            if (JTok is JObject)
            {
                JObject jObj = JTok as JObject;
                foreach (KeyValuePair<string, JToken> jObjEntry in jObj)
                {
                    if (jObjEntry.Value is JObject || jObjEntry.Value is JArray)
                        FillJsonWithValuesRec(jObjEntry.Value, OutputDict, ref Index);
                    else
                    {
                        object val = this.GetValueFormOutDict(OutputDict, ref Index);
                        if (val == null)
                            val = string.Empty;
                        jObj[jObjEntry.Key] = JToken.FromObject(val);
                    }
                }
                if (jObj.TryGetValue(FormConstants.__eb_loop_through, out JToken value))
                    jObj.Remove(FormConstants.__eb_loop_through);
            }
            else if (JTok is JArray)
            {
                JArray jArr = JTok as JArray;
                foreach (JToken jt in jArr)
                    FillJsonWithValuesRec(jt, OutputDict, ref Index);
            }
        }

        public List<ApiRequest> CallApiInApiDataPushers(object out_dict, List<ApiRequest> ApiRqsts)
        {
            Dictionary<int, object[]> OutputDict = (Dictionary<int, object[]>)out_dict;
            int Index = 1;

            foreach (EbApiDataPusher pusher in this.WebForm.DataPushers.FindAll(e => e is EbApiDataPusher))
            {
                bool allowPush = false;
                if (!string.IsNullOrEmpty(pusher.PushOnlyIf))
                {
                    object status = this.GetValueFormOutDict(OutputDict, ref Index);
                    if (Convert.ToBoolean(status))
                        allowPush = true;
                }
                else
                    allowPush = true;

                if (allowPush)
                {
                    JObject JObj = JObject.Parse(pusher.ProcessedJson);
                    FillJsonWithValuesRec(JObj, OutputDict, ref Index);
                    Dictionary<string, object> RqstObj = new Dictionary<string, object>();

                    foreach (KeyValuePair<string, JToken> jEntry in JObj)
                    {
                        object val = jEntry.Value is JValue ? (jEntry.Value as JValue).Value : Convert.ToString(jEntry.Value);
                        RqstObj.Add(jEntry.Key, val);
                    }

                    ApiRqsts.Add(new ApiRequest
                    {
                        RefId = pusher.ApiRefId,
                        Data = RqstObj,
                        SolnId = this.WebForm.SolutionObj.SolutionID,
                        UserAuthId = this.WebForm.UserObj.AuthId,
                        UserId = this.WebForm.UserObj.UserId,
                        WhichConsole = this.WebForm.UserObj.wc
                    });

                    //try
                    //{
                    //    ApiResponse result = service.Gateway.Send<ApiResponse>(new ApiRequest
                    //    {
                    //        RefId = pusher.ApiRefId,
                    //        Data = RqstObj,
                    //        SolnId = this.WebForm.SolutionObj.SolutionID,
                    //        UserAuthId = this.WebForm.UserObj.AuthId,
                    //        UserId = this.WebForm.UserObj.UserId,
                    //        WhichConsole = this.WebForm.UserObj.wc
                    //    });
                    //    resp += "\n\n" + JsonConvert.SerializeObject(result);
                    //}
                    //catch (Exception ex)
                    //{
                    //    Console.WriteLine($"Exception in CallApiInApiDataPushers: {ex.Message}\n{ex.StackTrace}");
                    //    throw new FormException("something went wrong", (int)HttpStatusCode.InternalServerError, ex.Message + " \n" + ex.StackTrace, "From EbDataPusher -> CallApiInApiDataPushers");
                    //}
                }
            }
            return ApiRqsts;
        }

        public static string ProcessApiDataPushers(EbWebForm _this, Service service, IDatabase DataDB, DbConnection DbCon, List<ApiRequest> ApiRqsts)
        {
            if (_this.DataPushers == null || !_this.DataPushers.Exists(e => e is EbApiDataPusher))
                return "No ApiDataPushers";
            string resp = string.Empty;
            //try
            //{
            FG_Root globals = GlobalsGenerator.GetCSharpFormGlobals_NEW(_this, _this.FormData, _this.FormDataBackup, DataDB, DbCon, false);
            EbDataPushHelper ebDataPushHelper = new EbDataPushHelper(_this);
            string code = ebDataPushHelper.GetProcessedCode();
            if (code != string.Empty)
            {
                object out_dict = _this.ExecuteCSharpScriptNew(code, globals);
                ebDataPushHelper.CallApiInApiDataPushers(out_dict, ApiRqsts);
            }
            //}
            //catch (Exception ex) 
            //{
            //    Console.WriteLine($"Exception in ProcessApiDataPushers: {ex.Message}\n{ex.StackTrace}");
            //List<DbParameter> _params = new List<DbParameter>()
            //{
            //    DataDB.GetNewParameter("form_refid", EbDbTypes.String, _this.RefId),
            //    DataDB.GetNewParameter("data_id", EbDbTypes.Int32, _this.TableRowId),
            //    DataDB.GetNewParameter("created_by", EbDbTypes.Int32, _this.UserObj.UserId),
            //    DataDB.GetNewParameter("modified_by", EbDbTypes.Int32, _this.UserObj.UserId),
            //    DataDB.GetNewParameter($"message", EbDbTypes.String, ex.Message)
            //};
            //int i = 0;
            //string fullQry = string.Empty;
            //foreach (EbApiDataPusher pusher in _this.DataPushers.FindAll(e => e is EbApiDataPusher))
            //{
            //    fullQry += GetFailLogInsertQuery(DataDB, i);
            //    _params.Add(DataDB.GetNewParameter($"api_refid_{i}", EbDbTypes.String, pusher.ApiRefId));
            //    i++;
            //}

            //int stat = DataDB.DoNonQuery(fullQry, _params.ToArray());
            //}
            return resp;
        }

        public static string CallInternalApis(List<ApiRequest> ApiRqsts, Service service)
        {
            string resp = string.Empty;
            foreach (ApiRequest rq in ApiRqsts)
            {
                try
                {
                    ApiResponse result = service.Gateway.Send<ApiResponse>(rq);
                    resp += "\n\n" + JsonConvert.SerializeObject(result);
                }
                catch (Exception ex)
                {
                    string temp = $"Exception in CallApiInApiDataPushers (CallInternalApis ReqObj): {JsonConvert.SerializeObject(rq)}\n{ex.Message}\n{ex.StackTrace}";
                    resp += "\n\n" + temp;
                    Console.WriteLine(temp);
                }
            }
            return resp;
        }

        private static string GetFailLogInsertQuery(IDatabase DataDB, int i)
        {
            return $@"INSERT INTO eb_apidatapuhser_log (form_refid, data_id, api_refid, status, message, created_by, created_at, modified_by, modified_at, eb_del)
                VALUES (@form_refid, @data_id, @api_refid_{i}, {(int)DataPusherLogStatus.InternalError}, @message, @created_by, {DataDB.EB_CURRENT_TIMESTAMP}, @modified_by, {DataDB.EB_CURRENT_TIMESTAMP}, 'F');";
        }

        #endregion Api_data_pusher
    }
}
