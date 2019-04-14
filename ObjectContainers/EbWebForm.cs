using ExpressBase.Common;
using ExpressBase.Common.Data;
using ExpressBase.Common.Extensions;
using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using ExpressBase.Common.Singletons;
using ExpressBase.Common.Structures;
using ExpressBase.Data;
using ExpressBase.Objects.Objects;
using ExpressBase.Objects.ServiceStack_Artifacts;
using ExpressBase.Security;
using Newtonsoft.Json;
using ServiceStack;
using ServiceStack.Redis;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Common;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ExpressBase.Objects
{
    [EnableInBuilder(BuilderType.WebForm)]
    [HideInToolBox]
    [BuilderTypeEnum(BuilderType.WebForm)]
    public class EbWebForm : EbForm
    {
        [HideInPropertyGrid]
        public bool IsUpdate { get; set; }

        public bool IsRenderMode { get; set; }

        public EbWebForm() { }

        public override int TableRowId { get; set; }

        public WebformData FormData { get; set; }

        public WebFormSchema FormSchema { get; set; }
        
        public User UserObj { get; set; }

        public int LocationId { get; set; }
                
        [PropertyGroup("Events")]
        [EnableInBuilder(BuilderType.WebForm)]
        [PropertyEditor(PropertyEditorType.Collection)]
        public List<EbSQLValidator> DisableDelete { get; set; }

        [PropertyGroup("Events")]
        [EnableInBuilder(BuilderType.WebForm)]
        [PropertyEditor(PropertyEditorType.Collection)]
        public List<EbSQLValidator> DisableCancel { get; set; }

        [PropertyGroup("Events")]
        [EnableInBuilder(BuilderType.WebForm)]
        [PropertyEditor(PropertyEditorType.Collection)]
        public List<EbRoutines> BeforeSaveRoutines { get; set; }

        [PropertyGroup("Events")]
        [EnableInBuilder(BuilderType.WebForm)]
        [PropertyEditor(PropertyEditorType.Collection)]
        public List<EbRoutines> AfterSaveRoutines { get; set; }

        public static EbOperations Operations = WFOperations.Instance;

        public override string GetHead()
        {
            string head = string.Empty;

            foreach (EbControl c in this.Controls)
                head += c.GetHead();

            return head;
        }

        public override string GetHtml()
        {
            string html = "<form id='@ebsid@' isrendermode='@rmode@' ebsid='@ebsid@' class='formB-box form-buider-form ebcont-ctrl' eb-form='true' ui-inp eb-type='WebForm' @tabindex@>";

            foreach (EbControl c in this.Controls)
                html += c.GetHtml();

            html += "</form>";

            return html
                .Replace("@name@", this.Name)
                .Replace("@ebsid@", this.EbSid)
                .Replace("@rmode@", IsRenderMode.ToString())
                .Replace("@tabindex@", IsRenderMode ? string.Empty : " tabindex='1'");
        }

        public string GetControlNames()
        {
            List<string> _lst = new List<string>();

            //foreach (EbControl _c in this.FlattenedControls)
            //{
            //    if (!(_c is EbControlContainer))
            //        _lst.Add(_c.Name);
            //}

            return string.Join(",", _lst.ToArray());
        }

        public override void BeforeSave()
        {
            //BeforeSaveRec(this);
        }

        private void BeforeSaveRec(EbControlContainer _container)
        {
            foreach (EbControl c in _container.Controls)
            {
                if (c is EbControlContainer)
                {
                    if ((c as EbControlContainer).TableName.IsNullOrEmpty())
                        (c as EbControlContainer).TableName = _container.TableName;
                    BeforeSaveRec(c as EbControlContainer);
                }
            }
        }

        public string GetSelectQuery(WebFormSchema _schema = null, Service _service = null)
        {
            string query = string.Empty;
            string queryExt = string.Empty;
            if (_schema == null)
                _schema = this.GetWebFormSchema();
            foreach (TableSchema _table in _schema.Tables)
            {
                string _cols = "id";
                string _id = "id";

                if (_table.Columns.Count > 0)
                    _cols = "id, " + String.Join(", ", _table.Columns.Select(x => x.ColumnName));
                if (_table.TableName != _schema.MasterTable)
                    _id = _schema.MasterTable + "_id";

                query += string.Format("SELECT {0} FROM {1} WHERE {2} = :id AND eb_del='F';", _cols, _table.TableName, _id);

                foreach (ColumnSchema Col in _table.Columns)
                {
                    if (Col.Control is EbPowerSelect)
                        queryExt += (Col.Control as EbPowerSelect).GetSelectQuery(_service, Col.ColumnName, _table.TableName, _id);
                    else if (Col.Control is EbDGPowerSelectColumn)
                        queryExt += (Col.Control as EbDGPowerSelectColumn).GetSelectQuery(_service, Col.ColumnName, _table.TableName, _id);
                }
            }
            foreach (Object Ctrl in _schema.ExtendedControls)
            {
                if (Ctrl is EbFileUploader)
                    queryExt += (Ctrl as EbFileUploader).GetSelectQuery();
            }
            return query + queryExt;
        }

        public string GetDeleteQuery(WebFormSchema _schema = null)
        {
            string query = string.Empty;
            if (_schema == null)
                _schema = this.GetWebFormSchema();
            foreach (TableSchema _table in _schema.Tables)
            {
                string _id = "id";
                string _dupcols = string.Empty;
                if (_table.TableName != _schema.MasterTable)
                    _id = _schema.MasterTable + "_id";
                foreach (ColumnSchema _column in _table.Columns)
                {
                    if (_column.Control is EbAutoId)
                    {
                        _dupcols += string.Format(", {0}_ebbkup = {0}, {0} = {0} || '_ebbkup'", _column.ColumnName);
                    }
                }
                query += string.Format("UPDATE {0} SET eb_del='T',eb_lastmodified_by = :eb_modified_by, eb_lastmodified_at = CURRENT_TIMESTAMP AT TIME ZONE 'UTC' {1} WHERE {2} = :id AND eb_del='F';", _table.TableName, _dupcols, _id);
            }
            return query;
        }

        public string GetCancelQuery(WebFormSchema _schema = null)
        {
            string query = string.Empty;
            if (_schema == null)
                _schema = this.GetWebFormSchema();
            foreach (TableSchema _table in _schema.Tables)
            {
                string _id = "id";
                if (_table.TableName != _schema.MasterTable)
                    _id = _schema.MasterTable + "_id";
                query += string.Format("UPDATE {0} SET eb_void='T',eb_lastmodified_by = :eb_modified_by, eb_lastmodified_at = CURRENT_TIMESTAMP AT TIME ZONE 'UTC' WHERE {1} = :id AND eb_void='F' AND eb_del='F';", _table.TableName, _id);
            }
            return query;
        }

        //get formdata as globals for c# script engine
        public FormAsGlobal GetFormAsGlobal(WebformData _formData, EbControlContainer _container = null, FormAsGlobal _globals = null)
        {
            if (_container == null)
                _container = this;
            if (_globals == null)
                _globals = new FormAsGlobal { Name = this.Name };

            ListNTV listNTV = new ListNTV();

            if (_formData.MultipleTables.ContainsKey(_container.TableName))
            {
                for (int i = 0; i < _formData.MultipleTables[_container.TableName].Count; i++)
                {
                    foreach (EbControl control in _container.Controls)
                    {
                        if (control is EbControlContainer)
                        {
                            FormAsGlobal g = new FormAsGlobal();
                            g.Name = (control as EbControlContainer).Name;
                            _globals.AddContainer(g);
                            g = GetFormAsGlobal(_formData, control as EbControlContainer, g);
                        }
                        else
                        {
                            NTV n = GetNtvFromFormData(_formData, _container.TableName, i, control.Name);
                            if (n != null)
                                listNTV.Columns.Add(n);
                        }
                    }
                }
                _globals.Add(listNTV);
            }
            return _globals;
        }

        private NTV GetNtvFromFormData(WebformData _formData, string _table, int _row, string _column)
        {
            NTV ntv = null;
            if (_formData.MultipleTables.ContainsKey(_table))
            {
                foreach (SingleColumn col in _formData.MultipleTables[_table][_row].Columns)
                {
                    if (col.Name.Equals(_column))
                    {
                        ntv = new NTV()
                        {
                            Name = _column,
                            Type = (EbDbTypes)col.Type,
                            Value = col.Value
                        };
                        break;
                    }
                }
            }
            return ntv;
        }

        //get controls in webform as a single dimensional structure 
        public static Dictionary<int, EbControlWrapper> GetControlsAsDict(EbControlContainer _container, string _path = "", Dictionary<int, EbControlWrapper> _dict = null, int _counter = 0)
        {
            if (_dict == null)
            {
                _dict = new Dictionary<int, EbControlWrapper>();
            }
            IEnumerable<EbControl> FlatCtrls = _container.Controls.Get1stLvlControls();
            foreach (EbControl control in FlatCtrls)
            {
                _dict.Add(_counter++, new EbControlWrapper
                {
                    TableName = _container.TableName,
                    Path = _path == "" ? control.Name : _path + "." + control.Name,
                    Control = control
                });
            }
            foreach (EbControl control in _container.Controls)
            {
                if (control is EbControlContainer)
                {
                    _dict = GetControlsAsDict(control as EbControlContainer, _path + "." + (control as EbControlContainer).Name, _dict, _counter);
                }
            }
            return _dict;
        }

        //get all control container as flat structure
        public List<EbControlContainer> GetAllContainers(EbControlContainer _container, List<EbControlContainer> _list = null)
        {
            if (_list == null)
                _list = new List<EbControlContainer>();
            _list.Add(_container);
            foreach (EbControl c in this.Controls)
            {
                if (c is EbControlContainer)
                {
                    _list = GetAllContainers(_container, _list);
                }
            }
            return _list;
        }

        //merge formdata and webform object
        public void MergeFormData()
        {
            MergeFormDataInner(this);
        }

        private void MergeFormDataInner(EbControlContainer _container)
        {
            foreach (EbControl c in _container.Controls)
            {
                if (c is EbDataGrid)
                {
                    foreach (EbControl control in (c as EbDataGrid).Controls)
                    {
                        if (!control.DoNotPersist)
                        {
                            List<object> val = new List<object>();
                            for (int i = 0; i < FormData.MultipleTables[(c as EbDataGrid).TableName].Count; i++)
                            {
                                if (FormData.MultipleTables[(c as EbDataGrid).TableName][i][control.Name] != null)
                                {
                                    val.Add(FormData.MultipleTables[(c as EbDataGrid).TableName][i][control.Name]);
                                    FormData.MultipleTables[(c as EbDataGrid).TableName][i].SetEbDbType(control.Name, control.EbDbType);
                                    FormData.MultipleTables[(c as EbDataGrid).TableName][i].SetControl(control.Name, control);
                                }
                            }
                            control.ValueFE = val;
                        }
                    }
                }
                else if (c is EbControlContainer)
                {
                    if (string.IsNullOrEmpty((c as EbControlContainer).TableName))
                        (c as EbControlContainer).TableName = _container.TableName;
                    MergeFormDataInner(c as EbControlContainer);
                }
                else if (c is EbAutoId)
                {
                    Dictionary<string, string> dict = new Dictionary<string, string>();
                    dict.Add("{currentlocation.id}", this.LocationId.ToString());
                    dict.Add("{user.id}", this.UserObj.UserId.ToString());

                    MatchCollection mc = Regex.Matches((c as EbAutoId).Pattern.sPattern, @"{(.*?)}");
                    foreach (Match m in mc)
                    {
                        if (dict.ContainsKey(m.Value))
                            (c as EbAutoId).Pattern.sPattern = (c as EbAutoId).Pattern.sPattern.Replace(m.Value, dict[m.Value]);
                    }
                    FormData.MultipleTables[_container.TableName][0].SetEbDbType(c.Name, c.EbDbType);
                    FormData.MultipleTables[_container.TableName][0].SetControl(c.Name, c);
                    FormData.MultipleTables[_container.TableName][0][c.Name] = (c as EbAutoId).Pattern.sPattern;
                    c.ValueFE = FormData.MultipleTables[_container.TableName][0][c.Name];
                }
                else if (!(c is EbFileUploader))
                {
                    if (!c.DoNotPersist)
                    {
                        if (FormData.MultipleTables[_container.TableName][0][c.Name] != null)
                        {
                            c.ValueFE = FormData.MultipleTables[_container.TableName][0][c.Name];
                            FormData.MultipleTables[_container.TableName][0].SetEbDbType(c.Name, c.EbDbType);
                            FormData.MultipleTables[_container.TableName][0].SetControl(c.Name, c);
                        }
                    }
                }
            }
        }

        private void GetFormattedData(EbDataTable dataTable, SingleTable Table)
        {
            foreach (EbDataRow dataRow in dataTable.Rows)
            {
                SingleRow Row = new SingleRow();
                foreach (EbDataColumn dataColumn in dataTable.Columns)
                {
                    if (dataRow.IsDBNull(dataColumn.ColumnIndex))
                    {
                        Row.Columns.Add(new SingleColumn()
                        {
                            Name = dataColumn.ColumnName,
                            Type = (int)dataColumn.Type,
                            Value = null
                        });
                    }
                    else
                    {
                        object _unformattedData = dataRow[dataColumn.ColumnIndex];
                        object _formattedData = _unformattedData;

                        if (dataColumn.Type == EbDbTypes.Date)
                        {
                            _unformattedData = (_unformattedData == DBNull.Value) ? DateTime.MinValue : _unformattedData;
                            _formattedData = ((DateTime)_unformattedData).Date != DateTime.MinValue ? Convert.ToDateTime(_unformattedData).Add(CultureHelper.GetDifference(this.UserObj.Preference.TimeZone, true)).ToString(this.UserObj.Preference.ShortDatePattern, CultureInfo.InvariantCulture) : string.Empty;
                        }
                        //else if(dataColumn.Type == EbDbTypes.DateTime)
                        //{
                        //    _unformattedData = (_unformattedData == DBNull.Value) ? DateTime.MinValue : _unformattedData;
                        //    _formattedData = ((DateTime)_unformattedData).Date != DateTime.MinValue ? Convert.ToDateTime(_unformattedData).ToString("yyyy-MM-dd hh:mm tt", CultureInfo.InvariantCulture) : string.Empty;
                        //}
                        Row.Columns.Add(new SingleColumn()
                        {
                            Name = dataColumn.ColumnName,
                            Type = (int)dataColumn.Type,
                            Value = _formattedData
                        });
                    }                    
                }
                Row.RowId = dataRow[dataTable.Columns[0].ColumnIndex].ToString();
                Table.Add(Row);
            }
        }

        //For Normal Mode
        public void RefreshFormData(IDatabase DataDB, Service service)
        {
            WebFormSchema _schema = this.GetWebFormSchema();
            string query = this.GetSelectQuery(_schema, service);
            string context = this.RefId.Split("-")[3] + "_" + this.TableRowId.ToString();//context format = objectId_rowId_ControlId

            EbDataSet dataset = DataDB.DoQueries(query, new DbParameter[]
            {
                DataDB.GetNewParameter("id", EbDbTypes.Int32, this.TableRowId),
                DataDB.GetNewParameter("context", EbDbTypes.String, context)
            });

            Console.WriteLine("From RefreshFormData : Schema table count = " + _schema.Tables.Count + " Dataset count = " + dataset.Tables.Count);

            this.FormData = new WebformData();

            for (int i = 0; i < _schema.Tables.Count && dataset.Tables.Count >= _schema.Tables.Count; i++)
            {
                EbDataTable dataTable = dataset.Tables[i];////
                SingleTable Table = new SingleTable();

                GetFormattedData(dataTable, Table);

                if (!this.FormData.MultipleTables.ContainsKey(dataTable.TableName) && Table.Count > 0)
                    this.FormData.MultipleTables.Add(dataTable.TableName, Table);
            }
            if (this.FormData.MultipleTables.Count > 0)
                this.FormData.MasterTable = dataset.Tables[0].TableName;

            if (dataset.Tables.Count > _schema.Tables.Count)
            {
                int tableIndex = _schema.Tables.Count;
                foreach (TableSchema Tbl in _schema.Tables)
                {
                    foreach (ColumnSchema Col in Tbl.Columns)
                    {
                        if (Col.Control is EbPowerSelect || Col.Control is EbDGPowerSelectColumn)
                        {
                            SingleTable Table = new SingleTable();
                            GetFormattedData(dataset.Tables[tableIndex], Table);
                            this.FormData.ExtendedTables.Add((Col.Control as EbControl).EbSid, Table);
                            tableIndex++;
                        }
                    }
                }
                foreach (Object Ctrl in _schema.ExtendedControls)//FileUploader Controls
                {
                    SingleTable Table = new SingleTable();
                    GetFormattedData(dataset.Tables[tableIndex], Table);
                    //--------------
                    List<FileMetaInfo> _list = new List<FileMetaInfo>();
                    foreach (SingleRow dr in Table)
                    {
                        FileMetaInfo info = new FileMetaInfo
                        {
                            FileRefId = dr["id"],
                            FileName = dr["filename"],
                            Meta = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(dr["tags"] as string),
                            UploadTime = dr["uploadts"]
                        };

                        if (!_list.Contains(info))
                            _list.Add(info);
                    }
                    SingleTable _Table = new SingleTable {
                        new SingleRow() {
                            Columns = new List<SingleColumn> {
                                new SingleColumn { Name = "Files", Type = (int)EbDbTypes.Json, Value = JsonConvert.SerializeObject(_list) }
                            }
                        }
                    };
                    //--------------
                    this.FormData.ExtendedTables.Add((Ctrl as EbControl).EbSid, _Table);
                    tableIndex++;
                }
            }

            this.ExeDeleteCancelScript(DataDB);
        }

        //For Prefill Mode
        public void RefreshFormData(IDatabase DataDB, Service service, List<Param> _params)
        {
            WebFormSchema _schema = this.GetWebFormSchema();
            this.FormData = new WebformData
            {
                MasterTable = _schema.MasterTable
            };
            Dictionary<string, string> QrsDict = new Dictionary<string, string>();
            for (int i = 0; i < _params.Count; i++)
            {
                for (int j = 0; j < _schema.Tables.Count; j++)
                {
                    for (int k = 0; k < _schema.Tables[j].Columns.Count; k++)
                    {
                        if (_schema.Tables[j].Columns[k].ColumnName.Equals(_params[i].Name))
                        {
                            if(_schema.Tables[j].Columns[k].Control is EbPowerSelect)
                            {
                                string t = (_schema.Tables[j].Columns[k].Control as EbPowerSelect).GetSelectQuery(service, _params[i].Value);
                                QrsDict.Add((_schema.Tables[j].Columns[k].Control as EbPowerSelect).EbSid, t);
                            }
                            if (!this.FormData.MultipleTables.ContainsKey(_schema.Tables[j].TableName))
                            {
                                SingleTable tbl = new SingleTable();
                                tbl.Add(new SingleRow());
                                this.FormData.MultipleTables.Add(_schema.Tables[j].TableName, tbl);
                            }
                            SingleColumn col = new SingleColumn() {
                                Name = _params[i].Name,
                                Type = _schema.Tables[j].Columns[k].EbDbType,
                                Value = _params[i].ValueTo
                            };
                            this.FormData.MultipleTables[_schema.Tables[j].TableName][0].Columns.Add(col);
                        }
                    }
                }
            }
            if(QrsDict.Count > 0)
            {
                EbDataSet dataset = DataDB.DoQueries(string.Join(" ", QrsDict.Select(d => d.Value)), new DbParameter[] { });
                int i = 0;
                foreach (KeyValuePair<string, string> item in QrsDict)
                {
                    SingleTable Table = new SingleTable();
                    GetFormattedData(dataset.Tables[i++], Table);
                    this.FormData.ExtendedTables.Add(item.Key, Table);
                }
            }
        }

        public int Save(IDatabase DataDB, Service service)
        {
            int r = 0;
            if (this.TableRowId > 0)
                r = this.Update(DataDB);
            else
            {
                this.TableRowId = this.Insert(DataDB);
                r = 1;
            }
            this.RefreshFormData(DataDB, service);
            return r;
        }

        public int Update(IDatabase DataDB)
        {
            string fullqry = string.Empty;
            List<DbParameter> param = new List<DbParameter>();
            int i = 0;
            foreach (KeyValuePair<string, SingleTable> entry in this.FormData.MultipleTables)
            {
                foreach (SingleRow row in entry.Value)
                {
                    string _tblname = entry.Key;
                    if (Convert.ToInt32(row.RowId) > 0)
                    {
                        string _qry = "UPDATE {0} SET {1} eb_lastmodified_by = :eb_modified_by, eb_lastmodified_at = CURRENT_TIMESTAMP AT TIME ZONE 'UTC' WHERE id={2};";
                        string _colvals = string.Empty;
                        if (row.IsDelete && !_tblname.Equals(this.FormData.MasterTable))
                        {
                            _qry = "UPDATE {0} SET {1}, eb_lastmodified_by = :eb_modified_by, eb_lastmodified_at = CURRENT_TIMESTAMP AT TIME ZONE 'UTC' WHERE id={2} AND eb_del='F';";
                            _colvals = "eb_del='T'";
                        }
                        else
                        {
                            foreach (SingleColumn rField in row.Columns)
                            {
                                if (!(rField.Control is EbAutoId))
                                {
                                    _colvals += string.Concat(rField.Name, "=:", rField.Name, "_", i, ",");
                                    if (rField.Value == null)
                                    {
                                        var p = DataDB.GetNewParameter(rField.Name + "_" + i, (EbDbTypes)rField.Type);
                                        p.Value = DBNull.Value;
                                        param.Add(p);
                                    }
                                    else if ((EbDbTypes)rField.Type == EbDbTypes.Date || (EbDbTypes)rField.Type == EbDbTypes.DateTime)
                                    {
                                        rField.Value = DateTime.ParseExact(rField.Value.ToString(), this.UserObj.Preference.ShortDatePattern, CultureInfo.InvariantCulture).Add(CultureHelper.GetDifference(this.UserObj.Preference.TimeZone));
                                        param.Add(DataDB.GetNewParameter(rField.Name + "_" + i, (EbDbTypes)rField.Type, rField.Value));
                                    }
                                    else
                                        param.Add(DataDB.GetNewParameter(rField.Name + "_" + i, (EbDbTypes)rField.Type, rField.Value));                                    
                                }
                            }
                        }

                        fullqry += string.Format(_qry, _tblname, _colvals, row.RowId);
                    }
                    else
                    {
                        string _qry = "INSERT INTO {0} ({1} eb_created_by, eb_created_at, eb_loc_id, {3}_id ) VALUES ({2} :eb_createdby, CURRENT_TIMESTAMP AT TIME ZONE 'UTC', :eb_loc_id, :{4}_id);";
                        string _cols = string.Empty, _vals = string.Empty;
                        foreach (SingleColumn rField in row.Columns)
                        {
                            _cols += string.Concat(rField.Name, ",");
                            _vals += string.Concat(":", rField.Name, "_", i, ",");
                            if (rField.Value == null)
                            {
                                var p = DataDB.GetNewParameter(rField.Name + "_" + i, (EbDbTypes)rField.Type);
                                p.Value = DBNull.Value;
                                param.Add(p);
                            }
                            else if ((EbDbTypes)rField.Type == EbDbTypes.Date || (EbDbTypes)rField.Type == EbDbTypes.DateTime)
                            {
                                rField.Value = DateTime.ParseExact(rField.Value.ToString(), this.UserObj.Preference.ShortDatePattern, CultureInfo.InvariantCulture).Add(CultureHelper.GetDifference(this.UserObj.Preference.TimeZone));
                                param.Add(DataDB.GetNewParameter(rField.Name + "_" + i, (EbDbTypes)rField.Type, rField.Value));
                            }
                            else
                                param.Add(DataDB.GetNewParameter(rField.Name + "_" + i, (EbDbTypes)rField.Type, rField.Value));
                        }
                        fullqry += string.Format(_qry, _tblname, _cols, _vals, this.FormData.MasterTable, this.FormData.MasterTable);
                    }
                    i++;
                }
            }

            //------------------
            string EbObId = this.RefId.Split("-")[3];
            List<string> InnerVals = new List<string>();
            List<string> Innercxt = new List<string>();
            List<string> InnerIds = new List<string>();
            foreach (KeyValuePair<string, SingleTable> entry in this.FormData.ExtendedTables)
            {
                foreach (SingleRow row in entry.Value)
                {
                    string cn = entry.Key + "_" + i.ToString();
                    i++;
                    InnerVals.Add(string.Format("(:{0}, '{1}_{2}_{3}')", cn, EbObId, this.TableRowId, entry.Key));
                    param.Add(DataDB.GetNewParameter(cn, EbDbTypes.Decimal, row.Columns[0].Value));
                    InnerIds.Add(":" + cn);
                }
                Innercxt.Add("context = '" + EbObId + "_" + this.TableRowId + "_" + entry.Key + "'");
            }
            if (InnerVals.Count > 0)
            {
                fullqry += string.Format(@"UPDATE 
                                            eb_files_ref AS t
                                        SET
                                            context = c.context
                                        FROM
                                            (VALUES{0}) AS c(id, context)
                                        WHERE
                                            c.id = t.id AND t.eb_del = 'F';", InnerVals.Join(","));
                fullqry += string.Format(@"UPDATE eb_files_ref 
                                        SET eb_del='T' 
                                        WHERE ({0}) AND eb_del='F' AND id NOT IN ({1});", Innercxt.Join(" OR "), InnerIds.Join(","));
            }

            //-------------------------

            param.Add(DataDB.GetNewParameter(this.FormData.MasterTable + "_id", EbDbTypes.Int32, this.FormData.MultipleTables[this.FormData.MasterTable][0].RowId));
            param.Add(DataDB.GetNewParameter("eb_loc_id", EbDbTypes.Int32, this.LocationId));
            param.Add(DataDB.GetNewParameter("eb_createdby", EbDbTypes.Int32, this.UserObj.UserId));
            //param.Add(this.EbConnectionFactory.DataDB.GetNewParameter("eb_createdat", EbDbTypes.DateTime, System.DateTime.Now));
            param.Add(DataDB.GetNewParameter("eb_modified_by", EbDbTypes.Int32, this.UserObj.UserId));
            //param.Add(this.EbConnectionFactory.DataDB.GetNewParameter("eb_modified_at", EbDbTypes.DateTime, System.DateTime.Now));

            return DataDB.InsertTable(fullqry, param.ToArray());
        }

        public int Insert(IDatabase DataDB)
        {
            string fullqry = string.Empty;
            List<DbParameter> param = new List<DbParameter>();
            int count = 0;
            int i = 0;
            foreach (KeyValuePair<string, SingleTable> entry in FormData.MultipleTables)
            {
                foreach (SingleRow row in entry.Value)
                {
                    string _qry = "INSERT INTO {0} ({1} eb_created_by, eb_created_at, eb_loc_id {3} ) VALUES ({2} :eb_createdby, CURRENT_TIMESTAMP AT TIME ZONE 'UTC', :eb_loc_id {4});";
                    string _tblname = entry.Key;
                    string _cols = string.Empty;
                    string _values = string.Empty;
                    //_cols = FormObj.GetCtrlNamesOfTable(entry.Key);

                    foreach (SingleColumn rField in row.Columns)
                    {
                        if (rField.Control is EbAutoId)
                        {
                            _cols += string.Concat(rField.Name, ", ");
                            _values += string.Format(":{0}_{1} || (SELECT LPAD((COUNT(*) + 1)::TEXT, {2}, '0') FROM {3} WHERE {0} LIKE '{4}%'),", rField.Name, i, (rField.Control as EbAutoId).Pattern.SerialLength, entry.Key, rField.Value);
                            param.Add(DataDB.GetNewParameter(rField.Name + "_" + i, (EbDbTypes)rField.Type, rField.Value));
                        }
                        else if (rField.Control != null)
                        {
                            _cols += string.Concat(rField.Name, ", ");
                            _values += string.Concat(":", rField.Name, "_", i, ", ");
                            if (rField.Value == null)
                            {
                                var p = DataDB.GetNewParameter(rField.Name + "_" + i, (EbDbTypes)rField.Type);
                                p.Value = DBNull.Value;
                                param.Add(p);
                            }
                            else if ((EbDbTypes)rField.Type == EbDbTypes.Date || (EbDbTypes)rField.Type == EbDbTypes.DateTime)
                            {
                                rField.Value = DateTime.ParseExact(rField.Value.ToString(), this.UserObj.Preference.ShortDatePattern, CultureInfo.InvariantCulture).Add(CultureHelper.GetDifference(this.UserObj.Preference.TimeZone));
                                param.Add(DataDB.GetNewParameter(rField.Name + "_" + i, (EbDbTypes)rField.Type, rField.Value));
                            }
                            else
                                param.Add(DataDB.GetNewParameter(rField.Name + "_" + i, (EbDbTypes)rField.Type, rField.Value));   
                        }
                    }
                    i++;

                    if (count == 0)
                        _qry = _qry.Replace("{3}", "").Replace("{4}", "");
                    else
                        _qry = _qry.Replace("{3}", string.Concat(",", this.TableName, "_id")).Replace("{4}", string.Concat(", (SELECT cur_val('", this.TableName, "_id_seq'" + "))"));
                    fullqry += string.Format(_qry, _tblname, _cols, _values);
                }
                count++;

            }

            //------------------File Uploader entries------------------------------------
            string EbObId = this.RefId.Split("-")[3];
            List<string> InnerVals = new List<string>();
            List<string> Innercxt = new List<string>();
            List<string> InnerIds = new List<string>();
            foreach (KeyValuePair<string, SingleTable> entry in FormData.ExtendedTables)
            {
                foreach (SingleRow row in entry.Value)
                {
                    string cn = entry.Key + "_" + i.ToString();
                    i++;
                    InnerVals.Add(string.Format("(:{0}, '{1}_' || cur_val('{2}_id_seq')::text || '_{3}')", cn, EbObId, this.TableName, entry.Key));
                    param.Add(DataDB.GetNewParameter(cn, EbDbTypes.Decimal, row.Columns[0].Value));
                    InnerIds.Add(":" + cn);
                }
                Innercxt.Add("context = '" + EbObId + "_' || cur_val('" + this.TableName + "_id_seq')::text || '_" + entry.Key + "'");
            }
            if (InnerVals.Count > 0)
            {
                fullqry += string.Format(@"UPDATE 
                                            eb_files_ref AS t
                                        SET
                                            context = c.context
                                        FROM
                                            (VALUES{0}) AS c(id, context)
                                        WHERE
                                            c.id = t.id AND t.eb_del = 'F';", InnerVals.Join(","));
                fullqry += string.Format(@"UPDATE eb_files_ref 
                                        SET eb_del='T' 
                                        WHERE ({0}) AND eb_del='F' AND id NOT IN ({1});", Innercxt.Join(" OR "), InnerIds.Join(","));
            }
            //-----------------------------------------------------------------------------

            param.Add(DataDB.GetNewParameter("eb_createdby", EbDbTypes.Int32, this.UserObj.UserId));
            param.Add(DataDB.GetNewParameter("eb_loc_id", EbDbTypes.Int32, this.LocationId));
            //param.Add(DataDB.GetNewParameter("eb_auto_id", EbDbTypes.String, FormData.AutoIdText ?? string.Empty));
            //fullqry += string.Format("UPDATE {0} SET eb_auto_id = :eb_auto_id || cur_val('{0}_id_seq')::text WHERE id = cur_val('{0}_id_seq');", this.TableName);
            fullqry += string.Concat("SELECT cur_val('", this.TableName, "_id_seq');");

            EbDataTable temp = DataDB.DoQuery(fullqry, param.ToArray());
            int _rowid = temp.Rows.Count > 0 ? Convert.ToInt32(temp.Rows[0][0]) : 0;

            return _rowid;
        }

        public int AfterSave(IDatabase DataDB, bool IsUpdate)
        {
            string q = string.Empty;
            if (this.AfterSaveRoutines != null && this.AfterSaveRoutines.Count > 0)
            {
                foreach(EbRoutines e in this.AfterSaveRoutines)
                {
                    if (IsUpdate && !e.IsDisabledOnEdit)
                        q += e.Script.Code + ";";
                    else if (!IsUpdate && !e.IsDisabledOnNew)
                        q += e.Script.Code + ";";
                }
            }
            if (!q.Equals(string.Empty))
            {
                List<DbParameter> param = new List<DbParameter>();
                foreach (KeyValuePair<string, SingleTable> item in this.FormData.MultipleTables)
                {
                    foreach (SingleColumn rField in item.Value[0].Columns)
                    {
                        if(q.Contains(":" + item.Key + "_" + rField.Name))
                        {
                            if (rField.Value == null)
                            {
                                var p = DataDB.GetNewParameter(item.Key + "_" + rField.Name, (EbDbTypes)rField.Type);
                                p.Value = DBNull.Value;
                                param.Add(p);
                            }
                            else
                                param.Add(DataDB.GetNewParameter(item.Key + "_" + rField.Name, (EbDbTypes)rField.Type, rField.Value));
                        }                        
                    }
                }
                return DataDB.InsertTable(q, param.ToArray());
            }
            return -1;
        }

        private bool CanDelete(IDatabase DataDB)
        {
            if (this.DisableDelete != null && this.DisableDelete.Count > 0)
            {
                string q = string.Join(";", this.DisableDelete.Select(e => e.Script.Code));
                DbParameter[] p = new DbParameter[] {
                    DataDB.GetNewParameter("id", EbDbTypes.Int32, this.TableRowId)
                };
                EbDataSet ds = DataDB.DoQueries(q, p);

                for (int i = 0; i < ds.Tables.Count; i++)
                {
                    if (ds.Tables[0].Rows.Count > 0 && ds.Tables[0].Rows[0].Count > 0)
                    {
                        if (!this.DisableDelete[i].IsDisabled && Convert.ToInt32(ds.Tables[0].Rows[0][0]) > 0 && !this.DisableDelete[i].IsWarningOnly)
                            return false;
                    }
                }
            }
            return true;
        }

        public int Delete(IDatabase DataDB)
        {            
            if (this.CanDelete(DataDB))
            {
                string query = this.GetDeleteQuery();
                DbParameter[] param = new DbParameter[] {
                    DataDB.GetNewParameter("eb_modified_by", EbDbTypes.Int32, this.UserObj.UserId),
                    DataDB.GetNewParameter("id", EbDbTypes.Int32, this.TableRowId)
                };
                return DataDB.UpdateTable(query, param);
            }
            return -1;
        }

        private bool CanCancel(IDatabase DataDB)
        {
            if (this.DisableCancel != null && this.DisableCancel.Count > 0)
            {
                string q = string.Join(";", this.DisableCancel.Select(e => e.Script.Code));
                DbParameter[] p = new DbParameter[] {
                    DataDB.GetNewParameter("id", EbDbTypes.Int32, this.TableRowId)
                };
                EbDataSet ds = DataDB.DoQueries(q, p);

                for (int i = 0; i < ds.Tables.Count; i++)
                {
                    if (ds.Tables[0].Rows.Count > 0 && ds.Tables[0].Rows[0].Count > 0)
                    {
                        if (!this.DisableCancel[i].IsDisabled && Convert.ToInt32(ds.Tables[0].Rows[0][0]) > 0 && !this.DisableCancel[i].IsWarningOnly)
                            return false;
                    }
                }
            }
            return true;
        }

        public int Cancel(IDatabase DataDB)
        {            
            if (this.CanCancel(DataDB))
            {
                string query = this.GetCancelQuery();
                DbParameter[] param = new DbParameter[] {
                    DataDB.GetNewParameter("eb_modified_by", EbDbTypes.Int32, this.UserObj.UserId),
                    DataDB.GetNewParameter("id", EbDbTypes.Int32, this.TableRowId)
                };
                return DataDB.UpdateTable(query, param);
            }
            return -1;
        }

        private void ExeDeleteCancelScript(IDatabase DataDB)
        {
            string q = string.Empty;
            if (this.DisableDelete != null && this.DisableDelete.Count > 0)
            {
                q = string.Join(";", this.DisableDelete.Select(e => e.Script.Code));
            }
            if (this.DisableCancel != null && this.DisableCancel.Count > 0)
            {
                q += string.Join(";", this.DisableCancel.Select(e => e.Script.Code));
            }
            if (!q.Equals(string.Empty))
            {
                DbParameter[] p = new DbParameter[] {
                    DataDB.GetNewParameter("id", EbDbTypes.Int32, this.TableRowId)
                };
                EbDataSet ds = DataDB.DoQueries(q, p);
                int i = 0;
                for (; i < this.DisableDelete.Count; i++)
                {
                    if (ds.Tables[i].Rows.Count > 0 && ds.Tables[i].Rows[0].Count > 0)
                    {
                        if (this.DisableDelete[i].IsDisabled || Convert.ToInt32(ds.Tables[i].Rows[0][0]) == 0)
                        {
                            this.FormData.DisableDelete.Add(this.DisableDelete[i].Name, false);
                        }
                        else
                        {
                            this.FormData.DisableDelete.Add(this.DisableDelete[i].Name, true);
                        }
                    }
                }

                for (int j = 0; j < this.DisableCancel.Count; i++, j++)
                {
                    if (ds.Tables[i].Rows.Count > 0 && ds.Tables[i].Rows[0].Count > 0)
                    {
                        if (this.DisableCancel[j].IsDisabled || Convert.ToInt32(ds.Tables[i].Rows[0][0]) == 0)
                        {
                            this.FormData.DisableCancel.Add(this.DisableCancel[j].Name, false);
                        }
                        else
                        {
                            this.FormData.DisableCancel.Add(this.DisableCancel[j].Name, true);
                        }
                    }
                }
            }
        }


        public WebFormSchema GetWebFormSchema()
        {
            WebFormSchema _formSchema = new WebFormSchema();
            _formSchema.FormName = this.Name;
            _formSchema.MasterTable = this.TableName.ToLower();
            _formSchema = GetWebFormSchemaRec(_formSchema, this, this.TableName.ToLower());
            return _formSchema;
        }

        private WebFormSchema GetWebFormSchemaRec(WebFormSchema _schema, EbControlContainer _container, string _parentTable)
        {
            IEnumerable<EbControl> _flatControls = _container.Controls.Get1stLvlControls();
            TableSchema _table = _schema.Tables.FirstOrDefault(tbl => tbl.TableName == _container.TableName);
            if (_table == null)
            {
                _table = new TableSchema { TableName = _container.TableName.ToLower(), ParentTable = _parentTable };
                _schema.Tables.Add(_table);
            }
            foreach (EbControl control in _flatControls)
            {
                if (!control.DoNotPersist)
                {
                    if (control is EbFileUploader)
                        _schema.ExtendedControls.Add(control);
                    else
                        _table.Columns.Add(new ColumnSchema { ColumnName = control.Name, EbDbType = (int)control.EbDbType, Control = control });
                }
            }

            foreach (EbControl _control in _container.Controls)
            {
                if (_control is EbControlContainer)
                {
                    EbControlContainer Container = _control as EbControlContainer;
                    string __parentTbl = _parentTable;
                    if (Container.TableName.IsNullOrEmpty())
                        Container.TableName = _container.TableName;
                    else
                        __parentTbl = _container.TableName;
                    _schema = GetWebFormSchemaRec(_schema, Container, __parentTbl);
                }
            }
            return _schema;
        }

        public void AfterRedisGet(Service service)
        {
            EbFormHelper.AfterRedisGet(this, service);
        }

        public override void AfterRedisGet(RedisClient Redis, IServiceClient client)
        {
            EbFormHelper.AfterRedisGet(this, Redis, client);
        }

        public override string DiscoverRelatedRefids()
        {
            return EbFormHelper.DiscoverRelatedRefids(this);
        }
    }

    public static class EbFormHelper
    {
        public static string DiscoverRelatedRefids(EbControlContainer _this)
        {
            string refids = string.Empty;
            for (int i = 0; i < _this.Controls.Count; i++)
            {
                if (_this.Controls[i] is EbUserControl)
                {
                    refids += _this.Controls[i].RefId + ",";
                }
                else
                {
                    PropertyInfo[] _props = _this.Controls[i].GetType().GetProperties();
                    foreach (PropertyInfo _prop in _props)
                    {
                        if (_prop.IsDefined(typeof(OSE_ObjectTypes)))
                            refids += _prop.GetValue(_this.Controls[i], null).ToString() + ",";
                    }
                }
            }
            return refids;
        }

        public static void AfterRedisGet(EbControlContainer _this, RedisClient Redis, IServiceClient client)
        {
            try
            {
                for (int i = 0; i < _this.Controls.Count; i++)
                {
                    EbControl c = _this.Controls[i];
                    if (c is EbUserControl || c is EbDGUserControlColumn)
                    {
                        EbUserControl _temp = Redis.Get<EbUserControl>(c.RefId);
                        if (_temp == null)
                        {
                            var result = client.Get<EbObjectParticularVersionResponse>(new EbObjectParticularVersionRequest { RefId = c.RefId });
                            _temp = EbSerializers.Json_Deserialize(result.Data[0].Json);
                            Redis.Set<EbUserControl>(c.RefId, _temp);
                        }
                        //_temp.RefId = _this.Controls[i].RefId;
                        if (c is EbDGUserControlColumn)
                        {
                            (c as EbDGUserControlColumn).Columns = _temp.Controls;
                            foreach (EbControl Control in (c as EbDGUserControlColumn).Columns)
                            {
                                RenameControlsRec(Control, c.Name);
                            }
                        }
                        else
                        {
                            (c as EbUserControl).Controls = _temp.Controls;
                            foreach (EbControl Control in (c as EbUserControl).Controls)
                            {
                                RenameControlsRec(Control, c.Name);
                                //Control.ChildOf = "EbUserControl";
                                //Control.Name = c.Name + "_" + Control.Name;
                            }
                            //c = _temp;
                            c.AfterRedisGet(Redis, client);
                        }                        
                    }
                    else if(c is EbControlContainer)
                    {
                        AfterRedisGet(c as EbControlContainer, Redis, client);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("EXCEPTION : FormAfterRedisGet " + e.Message);
            }
        }

        public static void AfterRedisGet(EbControlContainer _this, Service service)
        {
            try
            {
                for (int i = 0; i < _this.Controls.Count; i++)
                {
                    if (_this.Controls[i] is EbUserControl || _this.Controls[i] is EbDGUserControlColumn)
                    {
                        EbUserControl _temp = service.Redis.Get<EbUserControl>(_this.Controls[i].RefId);
                        if (_temp == null)
                        {
                            var result = service.Gateway.Send<EbObjectParticularVersionResponse>(new EbObjectParticularVersionRequest { RefId = _this.Controls[i].RefId });
                            _temp = EbSerializers.Json_Deserialize(result.Data[0].Json);
                            service.Redis.Set<EbUserControl>(_this.Controls[i].RefId, _temp);
                        }
                        //_temp.RefId = _this.Controls[i].RefId;
                        if (_this.Controls[i] is EbDGUserControlColumn)
                        {
                            (_this.Controls[i] as EbDGUserControlColumn).Columns = _temp.Controls;
                            foreach (EbControl Control in (_this.Controls[i] as EbDGUserControlColumn).Columns)
                            {
                                RenameControlsRec(Control, _this.Controls[i].Name);
                            }
                        }
                        else
                        {
                            (_this.Controls[i] as EbUserControl).Controls = _temp.Controls;
                            foreach (EbControl Control in (_this.Controls[i] as EbUserControl).Controls)
                            {
                                RenameControlsRec(Control, _this.Controls[i].Name);
                                //Control.ChildOf = "EbUserControl";
                                //Control.Name = _this.Controls[i].Name + "_" + Control.Name;
                            }
                        //_this.Controls[i] = _temp;
                        (_this.Controls[i] as EbUserControl).AfterRedisGet(service);
                        }                        
                    }
                    else if (_this.Controls[i] is EbControlContainer)
                    {
                        AfterRedisGet(_this.Controls[i] as EbControlContainer, service);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("EXCEPTION : EbFormAfterRedisGet(service) " + e.Message);
            }
        }

        private static void RenameControlsRec(EbControl _control, string _ucName)
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
    }

    public class EbControlWrapper
    {
        public string TableName { get; set; }

        public string Path { get; set; }

        //public object Value { get; set; }///////

        public EbControl Control { get; set; }
    }

    public class EbSQLValidator : EbValidator
    {
        public EbSQLValidator() { }

        [EnableInBuilder(BuilderType.WebForm)]
        [PropertyEditor(PropertyEditorType.ScriptEditorCS)]
        public override EbScript Script { get; set; }
    }

    public class EbRoutines : EbValidator
    {
        public EbRoutines() { }

        [EnableInBuilder(BuilderType.WebForm)]
        [PropertyEditor(PropertyEditorType.ScriptEditorJS, PropertyEditorType.ScriptEditorCS)]
        public override EbScript Script { get; set; }

        [EnableInBuilder(BuilderType.WebForm)]
        public bool IsDisabledOnNew { get; set; }

        [EnableInBuilder(BuilderType.WebForm)]
        public bool IsDisabledOnEdit { get; set; }

        public override bool IsWarningOnly { get; set; }

        public override string FailureMSG { get; set; }

        public override bool IsDisabled { get; set; }
    }
}
