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


        [EnableInBuilder(BuilderType.WebForm)]
        public bool DisableAutoReadOnly { get; set; }


        [EnableInBuilder(BuilderType.WebForm)]
        public bool DisableAutoLock { get; set; }

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

    [Alias("Batch WebForm")]
    [EnableInBuilder(BuilderType.WebForm)]
    public class EbBatchFormDataPusher : EbDataPusher
    {
        public EbBatchFormDataPusher() { }

        [EnableInBuilder(BuilderType.WebForm)]
        public override string Name { get; set; }

        [HideInPropertyGrid]
        [EnableInBuilder(BuilderType.WebForm)]
        public override string SkipLineItemIf { get; set; }

        [EnableInBuilder(BuilderType.WebForm)]
        [Alias("Source datagrid")]
        public string SourceDG { get; set; }

        [EnableInBuilder(BuilderType.WebForm)]
        public bool DisableAutoReadOnly { get; set; }

        [EnableInBuilder(BuilderType.WebForm)]
        public bool DisableAutoLock { get; set; }
    }

    public class EbDataPusherConfig
    {
        public EbDataPusherConfig() { }

        public string SourceTable { get; set; }

        public string MultiPushId { get; set; }

        public bool AllowPush { get; set; }

        public int SourceRecId { get; set; }

        public bool DisableAutoReadOnly { get; set; }

        public bool DisableAutoLock { get; set; }

        public bool IsBatch { get; set; }//Is batch datapusher

        public string GridTableName { get; set; }

        public string GridName { get; set; }

        public SingleTable GridSingleTable { get; set; }

        public SingleTable GridSingleTableDelete { get; set; }

        public int GridDataId { get; set; }

        public int ScriptCount { get; set; }

        public EbDataPusherConfig ShallowCopy()
        {
            return (EbDataPusherConfig)this.MemberwiseClone();
        }
    }

    public class EbDataPushHelper
    {
        public EbWebForm WebForm { get; set; }

        public Dictionary<int, string> CodeDict { get; set; }

        public EbDataPushHelper(EbWebForm WebForm)
        {
            this.WebForm = WebForm;
        }

        public EbWebFormCollection CreateWebFormDataBatch(object out_dict)
        {
            Dictionary<int, object[]> OutputDict = (Dictionary<int, object[]>)out_dict;
            int Index = 1, CrudContext = 0;

            List<EbWebForm> FormList = new List<EbWebForm>();

            foreach (EbBatchFormDataPusher pusher in this.WebForm.DataPushers.FindAll(e => e is EbBatchFormDataPusher))
            {
                JObject JObj = JObject.Parse(pusher.Json);

                foreach (SingleRow Row in pusher.WebForm.DataPusherConfig.GridSingleTable)
                {
                    EbWebForm Form = pusher.WebForm.ShallowCopy();
                    Form.DataPusherConfig = pusher.WebForm.DataPusherConfig.ShallowCopy();

                    Form.FormData = CreateWebFormData_inner(OutputDict, ref Index, JObj, pusher.WebForm.FormSchema, this.WebForm, pusher.SkipLineItemIf, pusher.WebForm.DataPusherConfig.ScriptCount);
                    Form.DataPusherConfig.GridDataId = Row.RowId;
                    Form.MergeFormData();
                    Form.CrudContext = CrudContext++.ToString();
                    if (!string.IsNullOrWhiteSpace(pusher.PushOnlyIf))
                    {
                        object status = this.GetValueFormOutDict(OutputDict, ref Index);
                        if (!Convert.ToBoolean(status))
                            Form.DataPusherConfig.AllowPush = false;
                    }
                    FormList.Add(Form);
                }
            }

            return new EbWebFormCollection(FormList);
        }

        public void CreateWebFormData(object out_dict)
        {
            Dictionary<int, object[]> OutputDict = (Dictionary<int, object[]>)out_dict;
            int Index = 1;

            foreach (EbDataPusher pusher in this.WebForm.DataPushers.FindAll(e => e is EbFormDataPusher))
            {
                JObject JObj = JObject.Parse(pusher.ProcessedJson);
                pusher.WebForm.DataPusherConfig.SourceRecId = this.WebForm.TableRowId;
                pusher.WebForm.RefId = pusher.FormRefId;
                pusher.WebForm.UserObj = this.WebForm.UserObj;
                pusher.WebForm.LocationId = this.WebForm.LocationId;
                pusher.WebForm.SolutionObj = this.WebForm.SolutionObj;
                pusher.WebForm.DataPusherConfig.AllowPush = true;
                pusher.WebForm.FormData = CreateWebFormData_inner(OutputDict, ref Index, JObj, pusher.WebForm.FormSchema, this.WebForm, pusher.SkipLineItemIf);

                if (!string.IsNullOrWhiteSpace(pusher.PushOnlyIf))
                {
                    object status = this.GetValueFormOutDict(OutputDict, ref Index);
                    if (!Convert.ToBoolean(status))
                        pusher.WebForm.DataPusherConfig.AllowPush = false;
                }
                pusher.WebForm.MergeFormData();

                if (this.WebForm.TableRowId > 0)//if edit mode then fill or map the id by refering FormDataBackup
                {
                    pusher.WebForm.FormData = MergeFormDataWithBackUp(pusher.WebForm.FormData, pusher.WebForm.FormDataBackup, pusher.WebForm.DataPusherConfig.AllowPush, pusher.WebForm.FormSchema);
                }
            }
        }

        private WebformData CreateWebFormData_inner(Dictionary<int, object[]> OutputDict, ref int Index, JObject JObj, WebFormSchema DestSchema, EbWebForm SrcWebForm, string SkipLineItemIf, int CodeIdx = 1000)
        {
            WebformData FormData = new WebformData() { MasterTable = DestSchema.MasterTable };

            foreach (TableSchema _table in DestSchema.Tables)
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
                                val = this.GetValueFormOutDict(OutputDict, ref Index, CodeIdx);

                            if (SrcWebForm.AutoId != null && Convert.ToString(val).Contains(FG_Constants.AutoId_PlaceHolder))
                            {
                                if (_column.Control is EbAutoId || _column.Control is EbTextBox)
                                {
                                    _column.Control.BypassParameterization = true;
                                    string[] val_s = Convert.ToString(val).Split(FG_Constants.AutoId_PlaceHolder);

                                    val = string.Format("(SELECT {4}{0}{5} FROM {1} WHERE {2}id = (SELECT(eb_currval('{3}_id_seq'))))",
                                        SrcWebForm.AutoId.Name,
                                        SrcWebForm.AutoId.TableName,
                                        SrcWebForm.AutoId.TableName == SrcWebForm.TableName ? string.Empty : (SrcWebForm.TableName + CharConstants.UNDERSCORE),
                                        SrcWebForm.TableName,
                                        string.IsNullOrWhiteSpace(val_s[0]) ? string.Empty : $"'{val_s[0]}' || ",
                                        string.IsNullOrWhiteSpace(val_s[1]) ? string.Empty : $" || '{val_s[1]}'");
                                }
                                else
                                    val = string.Empty;
                            }
                            else if (SrcWebForm.AutoId != null && Convert.ToString(val).Contains(FG_Constants.AutoId_SerialNo_PlaceHolder))
                            {
                                if (_column.Control is EbAutoId || _column.Control is EbTextBox)
                                {
                                    _column.Control.BypassParameterization = true;
                                    val = string.Format("(SELECT '{5}' || RIGHT({0}, {4}) FROM {1} WHERE {2}id = (SELECT(eb_currval('{3}_id_seq'))))",
                                        SrcWebForm.AutoId.Name,
                                        SrcWebForm.AutoId.TableName,
                                        SrcWebForm.AutoId.TableName == SrcWebForm.TableName ? string.Empty : (SrcWebForm.TableName + CharConstants.UNDERSCORE),
                                        SrcWebForm.TableName,
                                        SrcWebForm.AutoId.Pattern.SerialLength,
                                        Convert.ToString(val).Replace(FG_Constants.AutoId_SerialNo_PlaceHolder, string.Empty));
                                }
                                else
                                    val = string.Empty;
                            }
                            else if (SrcWebForm.AutoId != null && _column.Control is EbAutoId && SrcWebForm.TableRowId > 0 && !string.IsNullOrWhiteSpace(Convert.ToString(val)))
                            {
                                _column.Control.BypassParameterization = true;
                                val = $"'{val}'";
                            }
                            else if (Convert.ToString(val).Contains(FG_Constants.DataId_PlaceHolder))
                            {
                                val = Convert.ToString(val).Replace(FG_Constants.DataId_PlaceHolder, string.Empty);
                                _column.Control.BypassParameterization = true;
                            }

                            Row.Columns.Add(new SingleColumn
                            {
                                Name = _column.ColumnName,
                                Type = _column.EbDbType,
                                Value = val
                            });
                        }
                        if (_table.TableType == WebFormTableTypes.Grid)
                        {
                            if (!string.IsNullOrWhiteSpace(SkipLineItemIf))
                            {
                                object status = this.GetValueFormOutDict(OutputDict, ref Index);
                                if (Convert.ToBoolean(status))
                                    continue;
                            }
                            if (!string.IsNullOrWhiteSpace(_table.CustomSelectQuery))//Data pushing to grid with CustomSelectQuery is blocked
                                continue;
                        }
                        Table.Add(Row);
                    }
                    FormData.MultipleTables.Add(_table.TableName, Table);
                }
            }
            return FormData;
        }

        private static WebformData MergeFormDataWithBackUp(WebformData FormData, WebformData FormDataBackup, bool AllowPush, WebFormSchema DestSchema)
        {
            if (FormDataBackup == null)
                return FormData;

            if (AllowPush)
            {
                foreach (KeyValuePair<string, SingleTable> entry in FormDataBackup.MultipleTables)
                {
                    TableSchema _table = DestSchema.Tables.Find(e => e.TableName == entry.Key);
                    if (_table?.TableType == WebFormTableTypes.Grid && !string.IsNullOrWhiteSpace(_table.CustomSelectQuery))
                        continue;

                    if (FormData.MultipleTables.ContainsKey(entry.Key))
                    {
                        for (int i = 0; i < entry.Value.Count; i++)
                        {
                            if (i < FormData.MultipleTables[entry.Key].Count)
                                FormData.MultipleTables[entry.Key][i].RowId = entry.Value[i].RowId;
                            else
                            {
                                FormData.MultipleTables[entry.Key].Add(entry.Value[i]);
                                FormData.MultipleTables[entry.Key][i].IsDelete = true;
                            }
                        }
                    }
                    else
                    {
                        FormData.MultipleTables.Add(entry.Key, entry.Value);
                        foreach (SingleRow Row in FormData.MultipleTables[entry.Key])
                            Row.IsDelete = true;
                    }
                }
            }
            else
            {
                FormData = FormDataBackup;
                foreach (KeyValuePair<string, SingleTable> entry in FormData.MultipleTables)
                {
                    foreach (SingleRow Row in entry.Value)
                        Row.IsDelete = true;
                }
            }
            return FormData;
        }

        private object GetValueFormOutDict(Dictionary<int, object[]> OutDict, ref int Index, int CodeIndx = 1000)
        {
            int StopCounter = 1000;// to avoid infinite loop in case of any unexpected error/exception
            //assuming that maximum cs expressions in a data pusher is 1000
            while (StopCounter > 0 && !OutDict.ContainsKey(Index))
            {
                StopCounter--;
                Index++;
            }
            if (!OutDict.ContainsKey(Index))
                throw new FormException("Exception in C# code evaluation", (int)HttpStatusCode.InternalServerError, "Malformed OutputDict from combined cs script evaluation", "Stopped by StopCounter1000");

            if (Convert.ToInt32(OutDict[Index][0]) == 1)// 1 = success, 2 = exception
            {
                return OutDict[Index++][1];
            }
            throw new FormException("Exception in C# code evaluation", (int)HttpStatusCode.InternalServerError, $"{OutDict[Index][1]} \n C# code : {CodeDict[((Index - 1) % CodeIndx) + 1]}", "");
        }

        private string GetProcessedCodeForBatch()
        {
            this.CodeDict = new Dictionary<int, string>();
            string FnDef = string.Empty, FnCall = string.Empty;
            int Index = 1; //Code index

            foreach (EbDataPusher pusher in this.WebForm.DataPushers.FindAll(e => e is EbBatchFormDataPusher))
            {
                EbDataPusherConfig conf = pusher.WebForm.DataPusherConfig;
                int startIndex = Index;
                FnCall += $"\nfor (int i = 0; i < {conf.GridSingleTable.Count}; i++) \n{{";
                FnCall += $"\nsourceform.{conf.GridName}.GetEnumerator();";
                JObject JObj = JObject.Parse(pusher.Json);

                foreach (TableSchema _table in pusher.WebForm.FormSchema.Tables)
                {
                    if (JObj[_table.TableName] != null)
                    {
                        foreach (JToken jRow in JObj[_table.TableName])
                        {
                            foreach (ColumnSchema _column in _table.Columns)
                            {
                                if (jRow[_column.ColumnName] != null)
                                {
                                    this.CodeDict.Add(Index, jRow[_column.ColumnName].ToString());
                                    FnDef += GetFunctionDefinition(jRow[_column.ColumnName].ToString(), Index);
                                    FnCall += GetFunctionCall(Index, true);
                                    Index++;
                                }
                            }
                            if (_table.TableType == WebFormTableTypes.Grid && !string.IsNullOrEmpty(pusher.SkipLineItemIf))
                            {
                                this.CodeDict.Add(Index, pusher.SkipLineItemIf);
                                FnDef += GetFunctionDefinition(pusher.SkipLineItemIf, Index);
                                FnCall += GetFunctionCall(Index, true);
                                Index++;
                            }
                        }
                    }
                }
                if (Index - startIndex > 0)
                {
                    if (!string.IsNullOrEmpty(pusher.PushOnlyIf))////multiplier avoided - fix this
                    {
                        this.CodeDict.Add(Index, pusher.PushOnlyIf);
                        FnDef += GetFunctionDefinition(pusher.PushOnlyIf, Index);
                        FnCall += GetFunctionCall(Index, true);
                        Index++;
                    }
                    FnCall = FnCall.Replace("$Multiplier$", $"(i*{Index - startIndex})+");
                    conf.ScriptCount = Index - startIndex;
                }
                FnCall += "\n}";
            }
            if (FnDef == string.Empty)
                return string.Empty;

            return FnDef + "\nInitOutputDict();\n" + FnCall + "\nreturn out_dict;";
        }

        //Combining all c sharp scripts to be executed as a single program
        public string GetProcessedSingleCode()
        {
            this.CodeDict = new Dictionary<int, string>();
            string FnDef = string.Empty, FnCall = string.Empty;
            int Index = 1; //Code index
            int formIdx = 0; //form index
            foreach (EbDataPusher pusher in this.WebForm.DataPushers.FindAll(e => e is EbFormDataPusher))
            {
                FnCall += $"\ndestinationform = DestinationForms[{formIdx++}];";

                JObject JObj = JObject.Parse(pusher.Json);
                PreprocessJsonRec(JObj);//__eb_loop_through
                pusher.ProcessedJson = JObj.ToString();

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
                            if (jRow is JObject temp && temp.TryGetValue(FormConstants.__eb_loop_through, out JToken _val))
                            {
                                FnCall += $"\n{_val};";//.GetEnumerator()
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

            return FnDef + "InitOutputDict(); \n" + FnCall + "return out_dict;";
        }

        private string GetFunctionDefinition(string Code, int Index)
        {
            if (this.WebForm.EvaluatorVersion == EvaluatorVersion.Version_1)
            {
                return $@"
public object fn_{Index}() 
{{ 
  {(Regex.IsMatch(Code, @"\breturn\b") ? string.Empty : "return ")} {Code} ;
}}".RemoveCR() + "\n";
            }

            return $@"
fn_{Index} = () => 
{{ 
  {(Regex.IsMatch(Code, @"\breturn\b") ? string.Empty : "return ")} {Code} ;
}};".RemoveCR() + "\n";
        }

        private string GetFunctionCall(int Index, bool PlaceHolder = false)
        {
            string s = $@"
try 
{{ 
    out_dict.Add({(PlaceHolder ? "$Multiplier$" : "")}{Index}, new object[]{{ 1, fn_{Index}()}}); 
}} 
catch (Exception e) 
{{
    out_dict.Add({(PlaceHolder ? "$Multiplier$" : "")}{Index}, new object[]{{ 2, e.Message}}); 
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


        #region _______________Batch_Data_Pusher_______________

        public static string ProcessBatchFormDataPushers(EbWebForm _this, Service service, IDatabase DataDB, DbConnection DbCon, WebformData in_data)
        {
            if (_this.DataPushers == null || !_this.DataPushers.Exists(e => e is EbBatchFormDataPusher))
                return "No BatchFormDataPushers";

            List<EbBatchFormDataPusher> pushers = new List<EbBatchFormDataPusher>();
            WebformData _FormData = JsonConvert.DeserializeObject<WebformData>(JsonConvert.SerializeObject(_this.FormData));
            bool ChangeDetected = false;
            foreach (EbBatchFormDataPusher batchDp in _this.DataPushers.FindAll(e => e is EbBatchFormDataPusher))
            {
                pushers.Add(batchDp);
                EbWebForm _form = EbFormHelper.GetEbObject<EbWebForm>(batchDp.FormRefId, null, service.Redis, service);
                batchDp.WebForm = _form; _form.RefId = batchDp.FormRefId;
                _form.UserObj = _this.UserObj;
                _form.SolutionObj = _this.SolutionObj;
                _form.LocationId = _this.LocationId;
                _form.AfterRedisGet_All(service);
                TableSchema _table = _this.FormSchema.Tables.Find(e => e.ContainerName == batchDp.SourceDG);
                SingleTable Table = _this.FormData.MultipleTables[_table.TableName];
                SingleTable TableBkUp = _this.FormDataBackup?.MultipleTables.ContainsKey(_table.TableName) == true ? _this.FormDataBackup.MultipleTables[_table.TableName] : null;
                (SingleTable TableEdited, SingleTable TableDeleted) = MergeGridData(TableBkUp, Table);
                _FormData.MultipleTables[_table.TableName] = TableEdited;//Grid aggregate in script may give wrong values because only edited rows are considered 
                if (!ChangeDetected && (TableEdited.Count > 0 || TableDeleted.Count > 0))
                    ChangeDetected = true;

                _form.DataPusherConfig = new EbDataPusherConfig()
                {
                    IsBatch = true,
                    GridTableName = _table.TableName,
                    GridName = batchDp.SourceDG,
                    GridSingleTable = TableEdited,
                    GridSingleTableDelete = TableDeleted,
                    SourceTable = _this.TableName,
                    SourceRecId = _this.TableRowId,
                    AllowPush = true,
                    DisableAutoReadOnly = batchDp.DisableAutoReadOnly,
                    DisableAutoLock = batchDp.DisableAutoLock
                };
            }
            if (!ChangeDetected)
                return "No Change found";

            EbDataPushHelper ebDataPushHelper = new EbDataPushHelper(_this);
            string code = ebDataPushHelper.GetProcessedCodeForBatch();

            if (code != string.Empty)
            {
                FG_Root globals = GlobalsGenerator.GetCSharpFormGlobals_NEW(_this, _FormData, null, DataDB, DbCon, true);
                globals.DestinationForms = new List<FG_WebForm>();
                for (int idx = 1; idx < _this.FormCollection.Count; idx++)
                {
                    EbWebForm __form = _this.FormCollection[idx];
                    FG_WebForm fG_WebForm = new FG_WebForm(__form.TableName, __form.TableRowId, __form.LocationId, __form.RefId, __form.FormData.CreatedBy, __form.FormData.CreatedAt);
                    GlobalsGenerator.GetCSharpFormGlobalsRec_NEW(fG_WebForm, __form, __form.FormData, null);
                    globals.DestinationForms.Add(fG_WebForm);
                }

                object out_dict = _this.ExecuteCSharpScriptNew(code, globals);
                EbWebFormCollection FormCollection = ebDataPushHelper.CreateWebFormDataBatch(out_dict);//new + change identified formCollectios (Data in FormData)
                EbWebFormCollection FormCollectionBkUp = RefreshBatchFormData(pushers, DataDB, DbCon);//change identified + going to delete formCollectios backup (Data in FormDataBackup)
                MergeFormData(FormCollection, FormCollectionBkUp, pushers);

                string fullqry = string.Empty;
                string _extqry = string.Empty;
                List<DbParameter> param = new List<DbParameter>();
                int i = 0;

                FormCollection.Update_Batch(DataDB, param, ref fullqry, ref _extqry, ref i);

                param.Add(DataDB.GetNewParameter(_this.TableName + FormConstants._eb_ver_id, EbDbTypes.Int32, _this.RefId.Split(CharConstants.DASH)[4]));
                param.Add(DataDB.GetNewParameter(FormConstants.eb_createdby, EbDbTypes.Int32, _this.UserObj.UserId));
                param.Add(DataDB.GetNewParameter(FormConstants.eb_modified_by, EbDbTypes.Int32, _this.UserObj.UserId));
                param.Add(DataDB.GetNewParameter(FormConstants.eb_currentuser_id, EbDbTypes.Int32, _this.UserObj.UserId));
                param.Add(DataDB.GetNewParameter(FormConstants.eb_loc_id, EbDbTypes.Int32, _this.LocationId));
                param.Add(DataDB.GetNewParameter(FormConstants.eb_signin_log_id, EbDbTypes.Int32, _this.UserObj.SignInLogId));

                int tem = DataDB.DoNonQuery(_this.DbConnection, fullqry, param.ToArray());
                return "rows affected " + tem;
            }
            return "Nothing to process";
        }

        //for batch data pusher cancel/delete
        public static List<EbWebForm> GetBatchPushedForms(EbWebForm _this, Service service, IDatabase DataDB, DbConnection DbCon)
        {
            List<EbWebForm> Forms = new List<EbWebForm>();

            if (_this.DataPushers == null || !_this.DataPushers.Exists(e => e is EbBatchFormDataPusher))
                return Forms;

            List<EbBatchFormDataPusher> pushers = new List<EbBatchFormDataPusher>();

            foreach (EbBatchFormDataPusher batchDp in _this.DataPushers.FindAll(e => e is EbBatchFormDataPusher))
            {
                pushers.Add(batchDp);
                EbWebForm _form = EbFormHelper.GetEbObject<EbWebForm>(batchDp.FormRefId, null, service.Redis, service);
                _form.RefId = batchDp.FormRefId;
                _form.UserObj = _this.UserObj;
                _form.SolutionObj = _this.SolutionObj;
                _form.LocationId = _this.LocationId;
                _form.AfterRedisGet_All(service);
                TableSchema _table = _this.FormSchema.Tables.Find(e => e.ContainerName == batchDp.SourceDG);
                SingleTable Table = _this.FormDataBackup.MultipleTables[_table.TableName];

                _form.DataPusherConfig = new EbDataPusherConfig()
                {
                    IsBatch = true,
                    GridTableName = _table.TableName,
                    GridName = batchDp.SourceDG,
                    GridSingleTable = Table,
                    GridSingleTableDelete = new SingleTable(),
                    SourceTable = _this.TableName,
                    SourceRecId = _this.TableRowId,
                    AllowPush = true,
                    DisableAutoReadOnly = batchDp.DisableAutoReadOnly,
                    DisableAutoLock = batchDp.DisableAutoLock
                };
                batchDp.WebForm = _form;
            }
            Forms.AddRange(RefreshBatchFormData(pushers, DataDB, DbCon));

            return Forms;
        }

        private static (SingleTable, SingleTable) MergeGridData(SingleTable OldTable, SingleTable NewTable)
        {
            SingleTable TableEdited = new SingleTable();
            SingleTable TableDeleted = new SingleTable();
            if (OldTable == null)
                return (NewTable, TableDeleted);
            foreach (SingleRow nRow in NewTable)
            {
                bool RowChanged = false;
                SingleRow oRow = OldTable.Find(e => e.RowId == nRow.RowId);
                if (oRow != null)
                {
                    foreach (SingleColumn nColumn in nRow.Columns)
                    {
                        if (Convert.ToString(oRow[nColumn.Name]) != Convert.ToString(nColumn.Value))
                        {
                            RowChanged = true;
                            break;
                        }
                    }
                    OldTable.Remove(oRow);
                }
                else
                    RowChanged = true;

                if (RowChanged)
                    TableEdited.Add(nRow);
            }
            foreach (SingleRow oRow in OldTable)
            {
                oRow.IsDelete = true;
                TableDeleted.Add(oRow);
            }
            return (TableEdited, TableDeleted);
        }

        private static void MergeFormData(EbWebFormCollection FormCollection, EbWebFormCollection FormCollectionBkUp, List<EbBatchFormDataPusher> Pushers)
        {
            foreach (EbBatchFormDataPusher batchDp in Pushers)
            {
                List<EbWebForm> RmForm = new List<EbWebForm>();
                foreach (EbWebForm Form in FormCollection)
                {
                    EbDataPusherConfig conf = Form.DataPusherConfig;
                    EbWebForm FormBkUp = FormCollectionBkUp.Find(e => e.DataPusherConfig.GridDataId == conf.GridDataId && e.DataPusherConfig.GridName == conf.GridName);

                    if (!conf.AllowPush && FormBkUp == null)
                        RmForm.Add(Form);
                    if (FormBkUp == null)//new
                        continue;
                    else
                    {
                        Form.FormDataBackup = FormBkUp.FormDataBackup;
                        Form.TableRowId = FormBkUp.TableRowId;
                        foreach (KeyValuePair<string, SingleTable> entry in Form.FormData.MultipleTables)
                        {
                            for (int i = 0; i < entry.Value.Count; i++)
                            {
                                if (Form.FormDataBackup.MultipleTables[entry.Key].Count > i)
                                    entry.Value[i].RowId = Form.FormDataBackup.MultipleTables[entry.Key][i].RowId;
                            }
                        }
                        if (!conf.AllowPush)
                        {
                            foreach (KeyValuePair<string, SingleTable> entry in Form.FormData.MultipleTables)
                                entry.Value.ForEach(e => e.IsDelete = true);
                        }
                    }
                }

                RmForm.ForEach(e => FormCollection.Remove(e));

                foreach (EbWebForm FormBkUp in FormCollectionBkUp)
                {
                    EbDataPusherConfig conf = FormBkUp.DataPusherConfig;
                    EbWebForm Form = FormCollection.Find(e => e.DataPusherConfig.GridDataId == conf.GridDataId && e.DataPusherConfig.GridName == conf.GridName);

                    if (Form == null)
                    {
                        FormBkUp.FormData = FormBkUp.FormDataBackup;
                        foreach (KeyValuePair<string, SingleTable> entry in FormBkUp.FormData.MultipleTables)
                            entry.Value.ForEach(e => e.IsDelete = true);
                        FormCollection.Add(FormBkUp);
                    }
                }
            }
        }

        private static EbWebFormCollection RefreshBatchFormData(List<EbBatchFormDataPusher> batchDataPushers, IDatabase DataDB, DbConnection DbCon)
        {
            string fullQuery = string.Empty;
            List<int> QryCount = new List<int>();

            foreach (EbBatchFormDataPusher batchDp in batchDataPushers)
            {
                string _qry = QueryGetter.GetSelectQuery_Batch(batchDp.WebForm, out int qCount);
                EbDataPusherConfig _conf = batchDp.WebForm.DataPusherConfig;

                foreach (SingleRow Row in _conf.GridSingleTable)
                    fullQuery += string.Format(_qry, Row.RowId);

                foreach (SingleRow Row in _conf.GridSingleTableDelete)
                    fullQuery += string.Format(_qry, Row.RowId);

                QryCount.Add(qCount);
            }

            EbDataSet dataset = DataDB.DoQueries(DbCon, fullQuery);

            List<EbWebForm> FormListBkUp = new List<EbWebForm>();

            for (int i = 0; i < batchDataPushers.Count; i++)
            {
                EbWebForm _form = batchDataPushers[i].WebForm;

                AddToFormList(FormListBkUp, dataset, DataDB, _form.DataPusherConfig.GridSingleTable, QryCount[i], _form, i * batchDataPushers.Count);
                AddToFormList(FormListBkUp, dataset, DataDB, _form.DataPusherConfig.GridSingleTableDelete, QryCount[i], _form, i * batchDataPushers.Count);
            }

            return new EbWebFormCollection(FormListBkUp);
        }

        private static void AddToFormList(List<EbWebForm> FormListBkUp, EbDataSet dataset, IDatabase DataDB, SingleTable Table, int QueryCount, EbWebForm ebWebForm, int bchDpIdx)
        {
            for (int j = 0; j < Table.Count; j++)
            {
                EbWebForm Form = GetShallowCopy(ebWebForm);
                Form.DataPusherConfig.GridDataId = Table[j].RowId;
                Form.CrudContext = j + "_";

                EbDataSet ds = new EbDataSet();

                for (int k = 0; k < QueryCount; k++)
                {
                    int idx = bchDpIdx + (j * Table.Count) + k;
                    ds.Tables.Add(dataset.Tables[idx]);
                }

                Form.RefreshFormDataInner(ds, DataDB, true, null);
                FormListBkUp.Add(Form);
            }
        }

        private static EbWebForm GetShallowCopy(EbWebForm WebForm)
        {
            EbWebForm Form = WebForm.ShallowCopy();
            Form.DataPusherConfig = WebForm.DataPusherConfig.ShallowCopy();
            return Form;
        }

        #endregion Batch_Data_Pusher

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

            return FnDef + "InitOutputDict();\n" + FnCall + "return out_dict;";
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
                                        dupliCount = this.WebForm.FormData.MultipleTables[_table.TableName].FindAll(e => !e.IsDelete).Count;
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

        //ApiDataPusher
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

            return FnDef + "InitOutputDict();\n" + FnCall + "return out_dict;";
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
