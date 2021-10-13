using ExpressBase.Common;
using ExpressBase.Common.Extensions;
using ExpressBase.Common.Objects;
using ExpressBase.Common.Structures;
using Newtonsoft.Json;
using ServiceStack;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Globalization;
using System.Linq;

namespace ExpressBase.Objects.WebFormRelated/////////////
{
    public enum DataModificationAction
    {
        Created = 1,
        Updated = 2,
        Deleted = 3,
        Cancelled = 4,
        DeleteReverted = 5,
        CancelReverted = 6,
        Saved_with_no_changes = 7, //Updated but no changes to show
        Locked = 8,
        Unlocked = 9
    }

    class EbAuditTrail
    {
        private EbWebForm WebForm { get; set; }

        private IDatabase DataDB { get; set; }

        private Service Service { get; set; }

        public EbAuditTrail(EbWebForm ebWebForm, IDatabase dataDB, Service service = null)
        {
            this.WebForm = ebWebForm;
            this.DataDB = dataDB;
            this.Service = service;
        }

        public int UpdateAuditTrail(DataModificationAction action)
        {
            //DataPusher not included
            List<AuditTrailInsertData> auditTrails = new List<AuditTrailInsertData>()
            {
                new AuditTrailInsertData
                {
                    Action = (int)action,
                    Fields = new List<AuditTrailEntry>(),
                    RefId = this.WebForm.RefId,
                    TableRowId = this.WebForm.TableRowId
                }
            };
            return this.UpdateAuditTrail(auditTrails);
        }

        public int UpdateAuditTrail()
        {
            List<AuditTrailInsertData> auditTrails = new List<AuditTrailInsertData>();
            foreach (EbWebForm _webForm in this.WebForm.FormCollection)
            {
                if (_webForm.TableRowId <= 0)
                    continue;

                if (_webForm.FormDataBackup == null ||
                    (_webForm.DataPusherConfig != null && _webForm.FormDataBackup.MultipleTables[_webForm.TableName].Count == 0))// Created
                {
                    auditTrails.Add(new AuditTrailInsertData
                    {
                        Action = (int)DataModificationAction.Created,
                        Fields = new List<AuditTrailEntry>(),
                        RefId = _webForm.RefId,
                        TableRowId = _webForm.TableRowId
                    });
                }
                else if (_webForm.DataPusherConfig != null && _webForm.FormData.MultipleTables[_webForm.TableName].Count == 0)
                {
                    auditTrails.Add(new AuditTrailInsertData
                    {
                        Action = (int)DataModificationAction.Deleted,
                        Fields = new List<AuditTrailEntry>(),
                        RefId = _webForm.RefId,
                        TableRowId = _webForm.TableRowId
                    });
                }
                else
                {
                    List<AuditTrailEntry> trailEntries = new List<AuditTrailEntry>();
                    List<AuditTrailEntry> entries = null;
                    foreach (TableSchema _table in _webForm.FormSchema.Tables)
                    {
                        List<int> rowIds = new List<int>();
                        foreach (SingleRow Row in _webForm.FormData.MultipleTables[_table.TableName])
                        {
                            if (Row.RowId <= 0)
                                continue;
                            rowIds.Add(Row.RowId);
                            SingleRow RowBkup = _webForm.FormDataBackup.MultipleTables[_table.TableName].Find(e => e.RowId == Row.RowId);
                            entries = this.GetTrailEntries(_webForm, _table, Row, RowBkup);
                            if (entries.Count > 0)
                                trailEntries.AddRange(entries);
                        }

                        foreach (SingleRow RowBkup in _webForm.FormDataBackup.MultipleTables[_table.TableName])
                        {
                            if (!rowIds.Contains(RowBkup.RowId) && RowBkup.RowId > 0)
                            {
                                entries = this.GetTrailEntries(_webForm, _table, null, RowBkup);
                                if (entries.Count > 0)
                                    trailEntries.AddRange(entries);
                            }
                        }
                    }
                    if (trailEntries.Count > 0)
                    {
                        auditTrails.Add(new AuditTrailInsertData
                        {
                            Action = (int)DataModificationAction.Updated,
                            Fields = trailEntries,
                            RefId = _webForm.RefId,
                            TableRowId = _webForm.TableRowId
                        });
                    }
                    else
                    {
                        auditTrails.Add(new AuditTrailInsertData
                        {
                            Action = (int)DataModificationAction.Saved_with_no_changes,
                            Fields = new List<AuditTrailEntry>(),
                            RefId = _webForm.RefId,
                            TableRowId = _webForm.TableRowId
                        });
                    }
                }
            }
            return this.UpdateAuditTrail(auditTrails);
        }

        private List<AuditTrailEntry> GetTrailEntries(EbWebForm _webForm, TableSchema _table, SingleRow Row, SingleRow RowBkup)
        {
            string _newVal, _oldVal;
            bool isUpdate = RowBkup != null && Row != null;
            List<AuditTrailEntry> trailEntries = new List<AuditTrailEntry>();
            string relation = string.Concat(_webForm.TableRowId, "-", Row != null ? Row.RowId : RowBkup.RowId);

            if (_webForm.FormSchema.MasterTable.Equals(_table.TableName))
                relation = _webForm.TableRowId.ToString();

            if (_table.TableType == WebFormTableTypes.Normal)
            {
                foreach (ColumnSchema _column in _table.Columns)
                {
                    if (_column.Control.DoNotPersist)
                        continue;
                    _newVal = null; _oldVal = null;

                    if (Row != null)
                        _newVal = Row[_column.ColumnName]?.ToString() ?? null;
                    if (RowBkup != null)
                        _oldVal = RowBkup[_column.ColumnName]?.ToString() ?? null;
                    if (isUpdate && _newVal == _oldVal)
                        continue;
                    trailEntries.Add(new AuditTrailEntry
                    {
                        Name = _column.ColumnName,
                        NewVal = _newVal,
                        OldVal = _oldVal,
                        DataRel = relation,
                        TableName = _table.TableName
                    });
                }
            }
            else
            {
                bool isChangeFound = false;
                Dictionary<string, string> dict4new = new Dictionary<string, string>();
                Dictionary<string, string> dict4old = new Dictionary<string, string>();
                foreach (ColumnSchema _column in _table.Columns)
                {
                    if (_column.Control.DoNotPersist)
                        continue;
                    if (Row != null)
                        dict4new.Add(_column.ColumnName, Row[_column.ColumnName]?.ToString() ?? null);
                    if (RowBkup != null)
                        dict4old.Add(_column.ColumnName, RowBkup[_column.ColumnName]?.ToString() ?? null);
                    if (isUpdate && dict4new[_column.ColumnName] != dict4old[_column.ColumnName])
                        isChangeFound = true;
                }
                if (isUpdate && !isChangeFound)//no changes
                    return trailEntries;
                _newVal = null; _oldVal = null;
                if (Row != null)
                    _newVal = JsonConvert.SerializeObject(dict4new);
                if (RowBkup != null)
                    _oldVal = JsonConvert.SerializeObject(dict4old);

                trailEntries.Add(new AuditTrailEntry
                {
                    Name = "RowData",
                    NewVal = _newVal,
                    OldVal = _oldVal,
                    DataRel = relation,
                    TableName = _table.TableName
                });
            }

            return trailEntries;
        }

        private int UpdateAuditTrail(List<AuditTrailInsertData> Data)
        {
            List<DbParameter> parameters = new List<DbParameter>
            {
                this.DataDB.GetNewParameter("eb_createdby", EbDbTypes.Int32, this.WebForm.UserObj.UserId)
            };
            int i = 0;
            string fullQry = string.Empty;
            foreach (AuditTrailInsertData data in Data)
            {
                parameters.Add(this.DataDB.GetNewParameter("formid_" + i, EbDbTypes.String, data.RefId));
                parameters.Add(this.DataDB.GetNewParameter("dataid_" + i, EbDbTypes.Int32, data.TableRowId));
                parameters.Add(this.DataDB.GetNewParameter("actiontype_" + i, EbDbTypes.Int32, data.Action));

                fullQry += $@"INSERT INTO eb_audit_master(formid, dataid, actiontype, eb_createdby, eb_createdat) 
                        VALUES (@formid_{i}, @dataid_{i}, @actiontype_{i}, @eb_createdby, {this.DataDB.EB_CURRENT_TIMESTAMP}); ";

                if (this.DataDB.Vendor == DatabaseVendors.MYSQL)
                    fullQry += "SELECT eb_persist_currval('eb_audit_master_id_seq'); ";
                if (data.Fields.Count != 0)
                {
                    List<string> lineQry = new List<string>();
                    foreach (AuditTrailEntry _field in data.Fields)
                    {
                        lineQry.Add(string.Format("((SELECT eb_currval('eb_audit_master_id_seq')), @{0}_{1}, @old{0}_{1}, @new{0}_{1}, @idrel{0}_{1}, @tblname{0}_{1})", _field.Name, i));
                        parameters.Add(this.DataDB.GetNewParameter($"{_field.Name}_{i}", EbDbTypes.String, _field.Name));
                        parameters.Add(this.GetNewParameter($"new{_field.Name}_{i}", _field.NewVal));
                        parameters.Add(this.GetNewParameter($"old{_field.Name}_{i}", _field.OldVal));
                        parameters.Add(this.DataDB.GetNewParameter($"idrel{_field.Name}_{i}", EbDbTypes.String, _field.DataRel));
                        parameters.Add(this.DataDB.GetNewParameter($"tblname{_field.Name}_{i}", EbDbTypes.String, _field.TableName));
                        i++;
                    }
                    fullQry += string.Format("INSERT INTO eb_audit_lines(masterid, fieldname, oldvalue, newvalue, idrelation, tablename) VALUES {0};", lineQry.Join(","));
                }
                i++;
            }
            if (fullQry.IsEmpty())
                return 0;
            return this.DataDB.DoNonQuery(this.WebForm.DbConnection, fullQry, parameters.ToArray());
        }

        private DbParameter GetNewParameter(string Name, string Value)
        {
            if (Value == null)
            {
                DbParameter temp = this.DataDB.GetNewParameter(Name, EbDbTypes.String);
                temp.Value = DBNull.Value;
                return temp;
            }
            else
                return this.DataDB.GetNewParameter(Name, EbDbTypes.String, Value);
        }


        public string GetAuditTrail()
        {
            string qry = @"	SELECT 
            	m.id, l.id, u.fullname, m.eb_createdby, m.eb_createdat, m.actiontype, l.tablename, l.fieldname, l.idrelation, l.oldvalue, l.newvalue
            FROM 
            	eb_audit_master m 
                LEFT JOIN eb_audit_lines l ON m.id = l.masterid
                LEFT JOIN eb_users u ON m.eb_createdby = u.id
            WHERE
            	 m.formid = @formid AND m.dataid = @dataid
            ORDER BY
            	m.id DESC, l.tablename, l.idrelation;";
            //0 id, 1 id, 2 fullname, 3 eb_createdby, 4 eb_createdat, 5 actiontype, 6 tablename, 7 fieldname, 8 idrelation, 9 oldvalue, 10 newvalue
            DbParameter[] parameters = new DbParameter[] {
                     DataDB.GetNewParameter("formid", EbDbTypes.String, this.WebForm.RefId),
                     DataDB.GetNewParameter("dataid", EbDbTypes.Int32, this.WebForm.TableRowId)
                 };
            EbDataTable dt = this.DataDB.DoQuery(qry, parameters);

            Dictionary<int, FormTransaction> Trans = new Dictionary<int, FormTransaction>();
            Dictionary<int, int> m_id_map = new Dictionary<int, int>();
            int counter = 1;
            TableSchema _table = null;
            ColumnSchema _column = null;
            Dictionary<string, string> DictVmAll = new Dictionary<string, string>();

            foreach (EbDataRow dr in dt.Rows)
            {
                int m_id = Convert.ToInt32(dr["id"]);
                if (!m_id_map.ContainsKey(m_id))
                    m_id_map.Add(m_id, counter++);
                m_id = m_id_map[m_id];

                DataModificationAction action = (DataModificationAction)Convert.ToInt32(dr["actiontype"]);

                if (action == DataModificationAction.Updated)
                {
                    string TableName = Convert.ToString(dr["tablename"]);
                    if (_table == null || !_table.TableName.Equals(TableName))
                    {
                        _table = this.WebForm.FormSchema.Tables.FirstOrDefault(tbl => tbl.TableName == TableName);
                        if (_table == null)//no such table - skipping invalid Audit Trail entry
                            continue;
                    }
                    if (_table.TableType == WebFormTableTypes.Normal)
                    {
                        _column = _table.Columns.FirstOrDefault(col => col.ColumnName == Convert.ToString(dr["fieldname"]));
                        if (_column == null)//no such control - skipping invalid Audit Trail entry
                            continue;
                        if (_column.Control.DoNotPersist || _column.Control.Hidden)
                            continue;
                    }
                }

                if (!Trans.ContainsKey(m_id))
                {
                    Trans.Add(m_id, new FormTransaction()
                    {
                        ActionType = action.ToString().Replace("_", " "),
                        CreatedBy = Convert.ToString(dr["fullname"]),
                        CreatedById = Convert.ToString(dr["eb_createdby"]),
                        CreatedAt = Convert.ToDateTime(dr["eb_createdat"]).ConvertFromUtc(this.WebForm.UserObj.Preference.TimeZone).ToString(this.WebForm.UserObj.Preference.GetShortDatePattern() + " " + this.WebForm.UserObj.Preference.GetShortTimePattern(), CultureInfo.InvariantCulture)
                    });
                }
                if (action != DataModificationAction.Updated)
                    continue;

                string[] rel_ids = Convert.ToString(dr["idrelation"]).Split('-');
                string new_val = dr.IsDBNull(10) ? null : Convert.ToString(dr["newvalue"]);
                string old_val = dr.IsDBNull(9) ? null : Convert.ToString(dr["oldvalue"]);

                if (_table.TableType != WebFormTableTypes.Normal)
                {
                    Dictionary<string, string> new_val_dict = (new_val == null || new_val == "[null]") ? null : JsonConvert.DeserializeObject<Dictionary<string, string>>(new_val);
                    Dictionary<string, string> old_val_dict = (old_val == null || old_val == "[null]") ? null : JsonConvert.DeserializeObject<Dictionary<string, string>>(old_val);

                    if (!Trans[m_id].GridTables.ContainsKey(_table.TableName))
                    {
                        Trans[m_id].GridTables.Add(_table.TableName, new FormTransactionTable() { Title = _table.Title });
                        for (int i = 0; i < _table.Columns.Count; i++)
                        {
                            EbControl _control = _table.Columns.ElementAt(i).Control;
                            if (_control.DoNotPersist || _control.Hidden)
                                continue;
                            if (_control is EbDGColumn _dgcol)
                                Trans[m_id].GridTables[_table.TableName].ColumnMeta.Add(new FormTransactionMetaInfo() { Index = i, Title = _dgcol.Title ?? _control.Label, IsNumeric = _dgcol is EbDGNumericColumn });
                            else
                                Trans[m_id].GridTables[_table.TableName].ColumnMeta.Add(new FormTransactionMetaInfo() { Index = i, Title = _control.Label, IsNumeric = _control is EbNumeric });
                        }
                    }

                    int curid = Convert.ToInt32(rel_ids[1]);
                    FormTransactionTable TblRef = Trans[m_id].GridTables[_table.TableName];
                    if (!TblRef.NewRows.ContainsKey(curid) && !TblRef.EditedRows.ContainsKey(curid) && !TblRef.DeletedRows.ContainsKey(curid))
                    {
                        //TblRef.Rows.Add(curid, new FormTransactionRow() { }); 
                        if (new_val_dict == null)
                            TblRef.DeletedRows.Add(curid, new FormTransactionRow() { });
                        else if (old_val_dict == null)
                            TblRef.NewRows.Add(curid, new FormTransactionRow() { });
                        else
                            TblRef.EditedRows.Add(curid, new FormTransactionRow() { });
                    }
                    FormTransactionRow curRow = TblRef.NewRows.ContainsKey(curid) ? TblRef.NewRows[curid] : TblRef.DeletedRows.ContainsKey(curid) ? TblRef.DeletedRows[curid] : TblRef.EditedRows[curid];

                    foreach (ColumnSchema __column in _table.Columns)
                    {
                        if (__column.Control.DoNotPersist || __column.Control.Hidden)
                            continue;
                        bool IsModified = false;
                        if (new_val_dict?.ContainsKey(__column.ColumnName) == true && old_val_dict?.ContainsKey(__column.ColumnName) == true)
                        {
                            if (new_val_dict[__column.ColumnName] != old_val_dict[__column.ColumnName])
                                IsModified = true;
                        }

                        string a = old_val_dict?.ContainsKey(__column.ColumnName) == true ? old_val_dict[__column.ColumnName] : null;
                        string b = new_val_dict?.ContainsKey(__column.ColumnName) == true ? new_val_dict[__column.ColumnName] : null;

                        this.PreProcessTransationData(DictVmAll, _table, __column, ref a, ref b);
                        curRow.Columns.Add(__column.ColumnName, new FormTransactionEntry() { OldValue = a, NewValue = b, IsModified = IsModified, IsNumeric = __column.Control is EbDGNumericColumn });
                    }
                }
                else
                {
                    if (old_val == new_val)
                        continue;
                    if (!Trans[m_id].Tables.ContainsKey(_table.TableName))
                        Trans[m_id].Tables.Add(_table.TableName, new FormTransactionRow() { });

                    this.PreProcessTransationData(DictVmAll, _table, _column, ref old_val, ref new_val);

                    FormTransactionEntry curtrans = new FormTransactionEntry()
                    {
                        OldValue = old_val,
                        NewValue = new_val,
                        IsModified = true,
                        Title = string.IsNullOrEmpty(_column.Control.Label) ? _column.ColumnName : _column.Control.Label
                    };
                    Trans[m_id].Tables[_table.TableName].Columns.Add(_column.ColumnName, curtrans);
                }
            }
            try
            {
                this.PostProcessTransationData(Trans, DictVmAll);
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception in AuditTrail.PostProcessTransationData. Message : " + e.Message + "\nStackTrace : " + e.StackTrace);
            }

            return JsonConvert.SerializeObject(Trans);
        }


        private void PreProcessTransationData(Dictionary<string, string> DictVmAll, TableSchema _table, ColumnSchema _column, ref string old_val, ref string new_val)
        {
            if (old_val == "[null]") old_val = null;
            if (new_val == "[null]") new_val = null;
            bool fetchVm = _column.Control is EbPowerSelect || _column.Control is EbDGPowerSelectColumn;
            fetchVm = fetchVm || (_column.Control is EbSimpleSelect && (_column.Control as EbSimpleSelect).IsDynamic);
            fetchVm = fetchVm || (_column.Control is EbDGSimpleSelectColumn && (_column.Control as EbDGSimpleSelectColumn).IsDynamic);

            if (fetchVm)//copy vm for dm
            {
                string key = string.Concat(_table.TableName, "_", _column.ColumnName);
                string temp = string.Empty;
                if (!string.IsNullOrEmpty(new_val))
                    temp = string.Concat(new_val, ",");
                if (!string.IsNullOrEmpty(old_val))
                    temp += string.Concat(old_val, ",");

                if (!temp.Equals(string.Empty))
                {
                    if (!DictVmAll.ContainsKey(key))
                        DictVmAll.Add(key, temp);
                    else
                        DictVmAll[key] = string.Concat(DictVmAll[key], temp);
                }
            }
            else if (_table.TableType == WebFormTableTypes.Review)
            {
                EbReview reviewCtrl = this.WebForm.FormSchema.ExtendedControls.Find(e => e is EbReview) as EbReview;
                old_val = this.GetFormattedApprovalData(old_val, _column, reviewCtrl);
                new_val = this.GetFormattedApprovalData(new_val, _column, reviewCtrl);
            }
            else
            {
                old_val = this.GetFormattedData(_column.Control, old_val);
                new_val = this.GetFormattedData(_column.Control, new_val);
            }
        }

        private string GetFormattedApprovalData(string unf_val, ColumnSchema _column, EbReview reviewCtrl)
        {
            string val = unf_val;
            if (_column.ColumnName == FormConstants.stage_unique_id)
            {
                unf_val = reviewCtrl.FormStages.Find(e => e.EbSid == val)?.Name;
                if (val == FormConstants.__system_stage)
                    unf_val = "System";
            }
            else if (_column.ColumnName == FormConstants.action_unique_id)
            {
                foreach (EbReviewStage st in reviewCtrl.FormStages)
                {
                    EbReviewAction act = st.StageActions.Find(e => e.EbSid == val);
                    if (act != null)
                    {
                        unf_val = act.Name;
                        break;
                    }
                    else if (val == FormConstants.__review_reset)
                    {
                        unf_val = "Reset";
                        break;
                    }
                }
            }
            return unf_val;
        }

        private string GetFormattedData(EbControl ctrl, string value)// missing: ss !IsDynamic
        {
            try
            {
                if (ctrl is EbNumeric || ctrl is EbDGNumericColumn)
                {
                    if (float.TryParse(value, out float val) && val != 0)
                        value = ctrl.GetSingleColumn(this.WebForm.UserObj, this.WebForm.SolutionObj, value, false).F;
                    else
                        value = string.Empty;
                }
                else if (ctrl is EbBooleanSelect || ctrl is EbDGBooleanSelectColumn || ctrl is EbDGBooleanColumn || ctrl is EbRadioButton || ctrl is EbTextBox)
                {
                    value = ctrl.GetSingleColumn(this.WebForm.UserObj, this.WebForm.SolutionObj, value, false).F;
                }
                else if (value != null)
                {
                    if (ctrl is EbDate || ctrl is EbDGDateColumn)
                        value = ctrl.GetSingleColumn(this.WebForm.UserObj, this.WebForm.SolutionObj, value, false).F;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception in GetAuditTrail -> PreProcessTransationData -> GetFormattedData -> GetSingleColumn\nControl Name: " + ctrl.Name + "\nold_val/new_value Value: " + value + "\nMessage: " + e.Message);
            }
            return value;
        }

        private void PostProcessTransationData(Dictionary<int, FormTransaction> Trans, Dictionary<string, string> DictVmAll)
        {
            string Qry = string.Empty;
            Dictionary<string, Dictionary<string, List<string>>> DictDm = new Dictionary<string, Dictionary<string, List<string>>>();
            List<DbParameter> param = new List<DbParameter>();

            foreach (TableSchema _table in this.WebForm.FormSchema.Tables)
            {
                foreach (ColumnSchema _column in _table.Columns)
                {
                    bool fetchVm = _column.Control is IEbPowerSelect && !(_column.Control as IEbPowerSelect).IsDataFromApi;
                    fetchVm = fetchVm || (_column.Control is EbSimpleSelect && (_column.Control as EbSimpleSelect).IsDynamic);
                    fetchVm = fetchVm || (_column.Control is EbDGSimpleSelectColumn && (_column.Control as EbDGSimpleSelectColumn).IsDynamic);

                    if (fetchVm)
                    {
                        string key = string.Concat(_table.TableName, "_", _column.ColumnName);
                        if (DictVmAll.ContainsKey(key))
                        {
                            if (!DictDm.ContainsKey(key))
                            {
                                if (_column.Control is IEbPowerSelect)
                                    Qry += (_column.Control as IEbPowerSelect).GetDisplayMembersQuery(this.DataDB, this.Service, DictVmAll[key].Substring(0, DictVmAll[key].Length - 1), param);
                                else if (_column.Control is EbSimpleSelect)
                                    Qry += (_column.Control as EbSimpleSelect).GetDisplayMembersQuery(this.DataDB, this.Service, DictVmAll[key].Substring(0, DictVmAll[key].Length - 1));
                                else
                                    Qry += (_column.Control as EbDGSimpleSelectColumn).GetDisplayMembersQuery(this.DataDB, this.Service, DictVmAll[key].Substring(0, DictVmAll[key].Length - 1));

                                DictDm.Add(key, new Dictionary<string, List<string>>());
                            }
                        }
                    }
                }
            }

            EbDataSet ds = DataDB.DoQueries(Qry, param.ToArray());

            for (int i = 0; i < ds.Tables.Count; i++)
            {
                foreach (EbDataRow row in ds.Tables[i].Rows)
                {
                    List<string> list = new List<string>();
                    for (int j = 1; j < row.Count; j++)
                    {
                        list.Add(row[j].ToString());
                    }
                    if (!DictDm.ElementAt(i).Value.ContainsKey(row[0].ToString()))
                        DictDm.ElementAt(i).Value.Add(row[0].ToString(), list);
                }
            }

            foreach (KeyValuePair<int, FormTransaction> trans in Trans)
            {
                foreach (KeyValuePair<string, FormTransactionRow> table in trans.Value.Tables)
                {
                    ReplaceVmWithDm(table.Value.Columns, DictDm, table.Key);
                }

                foreach (KeyValuePair<string, FormTransactionTable> table in trans.Value.GridTables)
                {
                    foreach (KeyValuePair<int, FormTransactionRow> row in table.Value.NewRows)
                    {
                        this.ReplaceVmWithDm(row.Value.Columns, DictDm, table.Key);
                    }
                    foreach (KeyValuePair<int, FormTransactionRow> row in table.Value.DeletedRows)
                    {
                        this.ReplaceVmWithDm(row.Value.Columns, DictDm, table.Key);
                    }
                    foreach (KeyValuePair<int, FormTransactionRow> row in table.Value.EditedRows)
                    {
                        this.ReplaceVmWithDm(row.Value.Columns, DictDm, table.Key);
                    }
                }
            }
        }

        private void ReplaceVmWithDm(Dictionary<string, FormTransactionEntry> Columns, Dictionary<string, Dictionary<string, List<string>>> DictDm, string tablename)
        {
            foreach (KeyValuePair<string, FormTransactionEntry> column in Columns)
            {
                if (DictDm.ContainsKey(tablename + "_" + column.Key))
                {
                    column.Value.OldValue = this.GetRowVm(DictDm, tablename, column.Key, column.Value.OldValue);
                    column.Value.NewValue = this.GetRowVm(DictDm, tablename, column.Key, column.Value.NewValue);
                }
            }
        }

        private string GetRowVm(Dictionary<string, Dictionary<string, List<string>>> DictDm, string tablename, string columnname, string Value)
        {
            if (!string.IsNullOrEmpty(Value))
            {
                string[] vm_arr = Value.Split(',');
                string dm = string.Empty;
                for (int i = 0; i < vm_arr.Length; i++)
                {
                    List<string> dmlist = DictDm[tablename + "_" + columnname].ContainsKey(vm_arr[i]) ? DictDm[tablename + "_" + columnname][vm_arr[i]] : null;
                    if (dmlist != null)
                    {
                        foreach (string d in dmlist)
                            dm += " " + d;
                    }
                    else
                        dm = vm_arr[i];
                    if (i < vm_arr.Length - 1)
                        dm += "<br>";
                }
                Value = dm;
            }
            return Value;
        }
    }
}
