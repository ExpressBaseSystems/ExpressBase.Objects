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

namespace ExpressBase.Objects
{
    [UsedWithTopObjectParent(typeof(EbObject))]
    [EnableInBuilder(BuilderType.WebForm)]
    public class EbDataPusher
    {
        public EbDataPusher() { }

        [PropertyEditor(PropertyEditorType.ObjectSelector)]
        [EnableInBuilder(BuilderType.WebForm)]
        [OSE_ObjectTypes(EbObjectTypes.iWebForm)]
        public string FormRefId { get; set; }

        [PropertyEditor(PropertyEditorType.String)]
        [EnableInBuilder(BuilderType.WebForm)]
        public string Json { get; set; }

        [HideInPropertyGrid]
        [EnableInBuilder(BuilderType.WebForm)]
        public string EbSid { get; set; }

        [EnableInBuilder(BuilderType.WebForm)]
        [Alias("Multi push id")]
        public string Name { get; set; }

        [PropertyEditor(PropertyEditorType.String)]
        [EnableInBuilder(BuilderType.WebForm)]
        public string PushOnlyIf { get; set; }

        [PropertyEditor(PropertyEditorType.String)]
        [EnableInBuilder(BuilderType.WebForm)]
        public string SkipLineItemIf { get; set; }

        public EbWebForm WebForm { get; set; }
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
                pusher.WebForm.DataPusherConfig.SourceRecId = this.WebForm.TableRowId;
                pusher.WebForm.RefId = pusher.FormRefId;
                pusher.WebForm.UserObj = this.WebForm.UserObj;
                pusher.WebForm.LocationId = this.WebForm.LocationId;
                pusher.WebForm.SolutionObj = this.WebForm.SolutionObj;

                if (!string.IsNullOrEmpty(pusher.PushOnlyIf))
                {
                    object status = this.GetValueFormOutDict(OutputDict, ref Index);
                    if (Convert.ToBoolean(status))
                        pusher.WebForm.DataPusherConfig.AllowPush = true;
                }
                else
                    pusher.WebForm.DataPusherConfig.AllowPush = true;

                if (pusher.WebForm.DataPusherConfig.AllowPush)
                {
                    pusher.WebForm.FormData = new WebformData() { MasterTable = pusher.WebForm.FormSchema.MasterTable };
                    JObject JObj = JObject.Parse(pusher.Json);

                    foreach (TableSchema _table in pusher.WebForm.FormSchema.Tables)
                    {
                        if (JObj[_table.TableName] != null)
                        {
                            SingleTable Table = new SingleTable();
                            foreach (JToken jRow in JObj[_table.TableName])
                            {
                                if (_table.TableType == WebFormTableTypes.Grid && !string.IsNullOrEmpty(pusher.SkipLineItemIf))
                                {
                                    object status = this.GetValueFormOutDict(OutputDict, ref Index);
                                    if (Convert.ToBoolean(status))
                                        continue;
                                }
                                SingleRow Row = new SingleRow() { RowId = 0 };
                                foreach (ColumnSchema _column in _table.Columns)
                                {
                                    object val = null;
                                    if (jRow[_column.ColumnName] != null)
                                        val = this.GetValueFormOutDict(OutputDict, ref Index);

                                    if (this.WebForm.AutoId != null && Convert.ToString(val) == FormConstants.AutoId_PlaceHolder)
                                    {
                                        if (_column.Control is EbAutoId)
                                        {
                                            (_column.Control as EbAutoId).BypassParameterization = true;
                                            val = $"(SELECT {this.WebForm.AutoId.Name} FROM {this.WebForm.AutoId.TableName} WHERE {(this.WebForm.AutoId.TableName == this.WebForm.TableName ? string.Empty : (this.WebForm.TableName + CharConstants.UNDERSCORE))}id = eb_currval('{this.WebForm.TableName}_id_seq'))";
                                        }
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
                                Table.Add(Row);
                            }
                            pusher.WebForm.FormData.MultipleTables.Add(_table.TableName, Table);
                        }
                    }

                    pusher.WebForm.MergeFormData();
                }
                else
                    pusher.WebForm.FormData = new WebformData();

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
            int Index = 1;
            foreach (EbDataPusher pusher in this.WebForm.DataPushers)
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

                JObject JObj = JObject.Parse(pusher.Json);
                foreach (TableSchema _table in pusher.WebForm.FormSchema.Tables)
                {
                    if (JObj[_table.TableName] != null)
                    {
                        foreach (JToken jRow in JObj[_table.TableName])
                        {
                            string RowWrapIf = string.Empty, RowFnCall = string.Empty;
                            if (_table.TableType == WebFormTableTypes.Grid && !string.IsNullOrEmpty(pusher.SkipLineItemIf))
                            {
                                this.CodeDict.Add(Index, pusher.SkipLineItemIf);
                                FnDef += GetFunctionDefinition(pusher.SkipLineItemIf, Index);
                                PusherFnCall += GetFunctionCall(Index);
                                RowWrapIf = GetWrappedFnCall(Index, false);
                                Index++;
                            }
                            foreach (ColumnSchema _column in _table.Columns)
                            {
                                if (jRow[_column.ColumnName] != null)
                                {
                                    this.CodeDict.Add(Index, jRow[_column.ColumnName].ToString());
                                    FnDef += GetFunctionDefinition(jRow[_column.ColumnName].ToString(), Index);
                                    RowFnCall += GetFunctionCall(Index);
                                    Index++;
                                }
                            }
                            if (RowWrapIf == string.Empty)
                                PusherFnCall += RowFnCall;
                            else
                                PusherFnCall += RowWrapIf.Replace("@InnerCode@", RowFnCall);
                        }
                    }
                }
                if (PusherWrapIf == string.Empty)
                    FnCall += PusherFnCall;
                else
                    FnCall += PusherWrapIf.Replace("@InnerCode@", PusherFnCall);
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
    {(Regex.IsMatch(Code, "\breturn\b") ? string.Empty : "return ")} {Code} ;
}}".RemoveCR() + "\n";
        }

        private string GetFunctionCall(int Index)
        {
            string s = $@"try 
{{ 
    out_dict.Add({Index}, new object[]{{ 1, fn_{Index}()}}); 
}} 
catch (Exception e) 
{{
    out_dict.Add({Index}, new object[]{{ 2, e.Message}}); 
}}";
            return s;
        }

        private string GetWrappedFnCall(int Index, bool TrueContinue)
        {
            string s = $"if (Convert.ToInt32(out_dict[{Index}][0]) == 1 && bool.TryParse(Convert.ToString(out_dict[{Index}][1]), out bool temp_{Index}) && temp_{Index} == {(TrueContinue ? "true" : "false")}) {{ @InnerCode@ }}";

            return s;
        }

        //============================== Excel Import start ===============================

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

        //============================== Excel Import end ===============================
    }
}
