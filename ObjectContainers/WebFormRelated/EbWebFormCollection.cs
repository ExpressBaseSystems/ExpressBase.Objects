using ExpressBase.Common;
using ExpressBase.Common.Constants;
using ExpressBase.Common.Structures;
using ExpressBase.Objects.ServiceStack_Artifacts;
using ExpressBase.Objects.WebFormRelated;
using System.Collections.Generic;
using System.Data.Common;

namespace ExpressBase.Objects
{
    public class EbWebFormCollection : List<EbWebForm>
    {
        private EbWebForm MasterForm { get; set; }

        public EbWebFormCollection(EbWebForm ebWebForm)
        {
            this.Add(ebWebForm);
            this.MasterForm = ebWebForm;
        }

        public void Insert(IDatabase DataDB, List<DbParameter> param, ref string fullqry, ref string _extqry, ref int i)
        {
            foreach (EbWebForm WebForm in this)
            {
                if (WebForm.DataPusherConfig?.AllowPush == false)
                    continue;
                if (!(WebForm.FormData.MultipleTables.ContainsKey(WebForm.FormSchema.MasterTable) && WebForm.FormData.MultipleTables[WebForm.FormSchema.MasterTable].Count > 0))
                {
                    string _q = QueryGetter.GetInsertQuery(WebForm, DataDB, WebForm.FormSchema.MasterTable, true);
                    fullqry += string.Format(_q, string.Empty, string.Empty);
                }
                foreach (TableSchema _table in WebForm.FormSchema.Tables)
                {
                    if (!WebForm.FormData.MultipleTables.ContainsKey(_table.TableName))
                        continue;

                    foreach (SingleRow row in WebForm.FormData.MultipleTables[_table.TableName])
                    {
                        string _cols = string.Empty;
                        string _values = string.Empty;

                        foreach (SingleColumn cField in row.Columns)
                        {
                            if (cField.Control != null)
                                cField.Control.ParameterizeControl(DataDB, param, WebForm.TableName, cField, true, ref i, ref _cols, ref _values, ref _extqry, WebForm.UserObj, null);
                            else
                                WebForm.ParameterizeUnknown(DataDB, param, cField, true, ref i, ref _cols, ref _values);
                        }

                        string _qry = QueryGetter.GetInsertQuery(WebForm, DataDB, _table.TableName, true);
                        fullqry += string.Format(_qry, _cols, _values);

                        fullqry += WebForm.InsertUpdateLines(_table.TableName, row, DataDB, param, ref i);
                    }
                }
                param.Add(DataDB.GetNewParameter(WebForm.TableName + FormConstants._eb_ver_id, EbDbTypes.Int32, WebForm.RefId.Split(CharConstants.DASH)[4]));
                param.Add(DataDB.GetNewParameter("refid", EbDbTypes.String, WebForm.RefId));
            }
        }

        public void Update(IDatabase DataDB, List<DbParameter> param, ref string fullqry, ref string _extqry, ref int i)
        {
            foreach (EbWebForm WebForm in this)
            {
                foreach (KeyValuePair<string, SingleTable> entry in WebForm.FormData.MultipleTables)
                {
                    foreach (SingleRow row in entry.Value)
                    {
                        string _colvals = string.Empty;
                        string _temp = string.Empty;
                        int _rowId = row.RowId;
                        if (_rowId > 0)
                        {
                            string t = string.Empty;
                            if (!row.IsDelete)
                            {
                                foreach (SingleColumn cField in row.Columns)
                                {
                                    if (cField.Control != null)
                                    {
                                        SingleColumn ocF = WebForm.FormDataBackup.MultipleTables[entry.Key].Find(e => e.RowId == row.RowId).Columns.Find(e => e.Name.Equals(cField.Name));
                                        cField.Control.ParameterizeControl(DataDB, param, WebForm.TableName, cField, false, ref i, ref _colvals, ref _temp, ref _extqry, WebForm.UserObj, ocF);
                                    }
                                    else
                                        WebForm.ParameterizeUnknown(DataDB, param, cField, false, ref i, ref _colvals, ref _temp);
                                }
                            }
                            else if (WebForm.DataPusherConfig == null && !entry.Key.Equals(WebForm.TableName))
                            {
                                List<TableSchema> _tables = WebForm.FormSchema.Tables.FindAll(e => e.IsDynamic && e.TableType == WebFormTableTypes.Grid);
                                foreach (TableSchema _table in _tables)
                                {
                                    t += $@"UPDATE {_table.TableName} SET eb_del = 'T', eb_lastmodified_by = @eb_modified_by, eb_lastmodified_at = {DataDB.EB_CURRENT_TIMESTAMP} WHERE
                                        {entry.Key}_id = @{entry.Key}_id_{i} AND {WebForm.TableName}_id = @{WebForm.TableName}_id AND COALESCE(eb_del, 'F') = 'F'; ";
                                    param.Add(DataDB.GetNewParameter(entry.Key + "_id_" + i, EbDbTypes.Int32, _rowId));
                                    i++;
                                }
                            }

                            string _qry = QueryGetter.GetUpdateQuery(WebForm, DataDB, entry.Key, row.IsDelete);
                            fullqry += string.Format(_qry, _colvals, row.RowId);
                            fullqry += t;
                        }
                        else
                        {
                            string _cols = string.Empty;
                            string _vals = string.Empty;

                            foreach (SingleColumn cField in row.Columns)
                            {
                                if (cField.Control != null)
                                    cField.Control.ParameterizeControl(DataDB, param, WebForm.TableName, cField, true, ref i, ref _cols, ref _vals, ref _extqry, WebForm.UserObj, null);
                                else
                                    WebForm.ParameterizeUnknown(DataDB, param, cField, true, ref i, ref _cols, ref _vals);
                            }
                            string _qry = QueryGetter.GetInsertQuery(WebForm, DataDB, entry.Key, WebForm.TableRowId == 0);
                            fullqry += string.Format(_qry, _cols, _vals);
                        }

                        fullqry += WebForm.InsertUpdateLines(entry.Key, row, DataDB, param, ref i);

                    }
                }
                param.Add(DataDB.GetNewParameter(WebForm.TableName + FormConstants._id, EbDbTypes.Int32, WebForm.TableRowId));
                param.Add(DataDB.GetNewParameter(WebForm.TableName + FormConstants._eb_ver_id, EbDbTypes.Int32, WebForm.RefId.Split(CharConstants.DASH)[4]));
            }
        }
    }
}
