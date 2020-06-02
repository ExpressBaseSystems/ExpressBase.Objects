//using ExpressBase.Common;
//using ExpressBase.Common.Extensions;
//using ExpressBase.Common.Objects;
//using ExpressBase.Common.Structures;
//using Newtonsoft.Json;
//using ServiceStack;
//using System;
//using System.Collections.Generic;
//using System.Data.Common;
//using System.Globalization;
//using System.Linq;
//using System.Text;

//namespace ExpressBase.Objects.WebFormRelated
//{
//    static class EbAuditTrail
//    {
//        public static int UpdateAuditTrail(EbWebForm _this, IDatabase DataDB)
//        {
//            List<EbWebForm> FormCollection = new List<EbWebForm> { _this };
//            if (_this.ExeDataPusher)
//            {
//                foreach (EbDataPusher pusher in _this.DataPushers)
//                    FormCollection.Add(pusher.WebForm);
//            }
//            List<AuditTrailInsertData> auditTrails = new List<AuditTrailInsertData>();

//            foreach (EbWebForm WebForm in FormCollection)
//            {
//                List<AuditTrailEntry> FormFields = new List<AuditTrailEntry>();
//                if (WebForm.FormDataBackup == null)
//                {
//                    auditTrails.Add(new AuditTrailInsertData { Action = 1, Fields = FormFields, RefId = WebForm.RefId, TableRowId = WebForm.TableRowId });
//                }
//                else
//                {
//                    foreach (KeyValuePair<string, SingleTable> entry in WebForm.FormData.MultipleTables)
//                    {
//                        bool IsGridTable = false;
//                        TableSchema _table = WebForm.FormSchema.Tables.FirstOrDefault(tbl => tbl.TableName.Equals(entry.Key));
//                        if (_table != null)
//                            IsGridTable = _table.TableType == WebFormTableTypes.Grid;

//                        if (!WebForm.FormDataBackup.MultipleTables.ContainsKey(entry.Key))//insert mode
//                        {
//                            foreach (SingleRow rField in entry.Value)
//                            {
//                                PushAuditTrailEntry(_this, entry.Key, rField, FormFields, true, IsGridTable, _table);
//                            }
//                        }
//                        else//update mode
//                        {
//                            List<int> rids = new List<int>();
//                            foreach (SingleRow rField in entry.Value)
//                            {
//                                rids.Add(rField.RowId);
//                                SingleRow orF = WebForm.FormDataBackup.MultipleTables[entry.Key].Find(e => e.RowId == rField.RowId);
//                                if (orF == null)//if it is new row
//                                {
//                                    PushAuditTrailEntry(_this, entry.Key, rField, FormFields, true, IsGridTable, _table);
//                                }
//                                else//row edited
//                                {
//                                    string relation = string.Concat(WebForm.TableRowId, "-", rField.RowId);

//                                    if (WebForm.FormSchema.MasterTable.Equals(entry.Key))
//                                        relation = WebForm.TableRowId.ToString();

//                                    bool IsRowEdited = false;
//                                    Dictionary<string, string> dic1 = null;
//                                    Dictionary<string, string> dic2 = null;
//                                    if (IsGridTable)
//                                    {
//                                        dic1 = new Dictionary<string, string>();
//                                        dic2 = new Dictionary<string, string>();
//                                    }
//                                    foreach (SingleColumn cField in rField.Columns)
//                                    {
//                                        if (cField.Name.Equals("id"))//skipping 'id' field
//                                            continue;
//                                        if (cField.Name.Equals("eb_row_num") && IsGridTable)//skipping 'eb_row_num' field
//                                            continue;
//                                        ColumnSchema _column = _table.Columns.Find(c => c.ColumnName.Equals(cField.Name));
//                                        if (_column != null)
//                                        {
//                                            if (_column.Control.DoNotPersist)//skip DoNotPersist field from audit entry// written for EbSystemControls
//                                                continue;
//                                        }
//                                        SingleColumn ocf = orF.Columns.Find(e => e.Name == cField.Name);

//                                        if (ocf == null)
//                                        {
//                                            ocf = new SingleColumn() { Name = cField.Name, Value = "[null]" };
//                                        }
//                                        if (IsGridTable)
//                                        {
//                                            dic1.Add(cField.Name, cField.Value == null ? "[null]" : cField.Value.ToString());
//                                            dic2.Add(ocf.Name, ocf.Value == null ? "[null]" : ocf.Value.ToString());
//                                        }
//                                        if (ocf.Value != cField.Value)//checking for changes /////// modifications required
//                                        {
//                                            IsRowEdited = true;
//                                            if (IsGridTable)
//                                                continue;

//                                            FormFields.Add(new AuditTrailEntry
//                                            {
//                                                Name = cField.Name,
//                                                NewVal = cField.Value == null ? "[null]" : cField.Value.ToString(),
//                                                OldVal = ocf.Value == null ? "[null]" : ocf.Value.ToString(),
//                                                DataRel = relation,
//                                                TableName = entry.Key
//                                            });
//                                        }
//                                    }
//                                    if (IsGridTable && IsRowEdited)
//                                    {
//                                        FormFields.Add(new AuditTrailEntry
//                                        {
//                                            Name = "dgrow",
//                                            NewVal = JsonConvert.SerializeObject(dic1),
//                                            OldVal = JsonConvert.SerializeObject(dic2),
//                                            DataRel = relation,
//                                            TableName = entry.Key
//                                        });
//                                    }
//                                }
//                            }
//                            foreach (SingleRow Row in WebForm.FormDataBackup.MultipleTables[entry.Key])//looking for deleted rows
//                            {
//                                if (!rids.Contains(Row.RowId))
//                                {
//                                    PushAuditTrailEntry(_this, entry.Key, Row, FormFields, false, IsGridTable, _table);
//                                }
//                            }
//                        }
//                    }
//                    if (FormFields.Count > 0)
//                    {
//                        auditTrails.Add(new AuditTrailInsertData { Action = 2, Fields = FormFields, RefId = WebForm.RefId, TableRowId = WebForm.TableRowId });
//                    }
//                }
//            }
//            return UpdateAuditTrail(_this, DataDB, auditTrails);
//        }

//        //managing new or deleted row
//        private static void PushAuditTrailEntry(EbWebForm _this, string Table, SingleRow Row, List<AuditTrailEntry> FormFields, bool IsIns, bool IsGridRow, TableSchema _table)
//        {
//            string relation = string.Concat(_this.TableRowId, "-", Row.RowId);

//            if (_this.FormSchema.MasterTable.Equals(Table))
//                relation = _this.TableRowId.ToString();

//            if (IsGridRow)
//            {
//                Dictionary<string, string> dic = new Dictionary<string, string>();
//                foreach (SingleColumn cField in Row.Columns)
//                {
//                    if (cField.Name.Equals("id") || cField.Name.Equals("eb_row_num"))//skipping 'id' field
//                        continue;
//                    dic.Add(cField.Name, cField.Value == null ? "[null]" : cField.Value.ToString());
//                }
//                string val = JsonConvert.SerializeObject(dic);
//                FormFields.Add(new AuditTrailEntry
//                {
//                    Name = "dgrow",
//                    NewVal = IsIns ? val : "[null]",
//                    OldVal = IsIns ? "[null]" : val,
//                    DataRel = relation,
//                    TableName = Table
//                });
//            }
//            else
//            {
//                foreach (SingleColumn cField in Row.Columns)
//                {
//                    if (cField.Name.Equals("id"))//skipping 'id' field
//                        continue;
//                    ColumnSchema _column = _table.Columns.Find(c => c.ColumnName.Equals(cField.Name));
//                    if (_column != null)
//                    {
//                        if (_column.Control.DoNotPersist)//skip DoNotPersist field from audit entry// written for EbSystemControls
//                            continue;
//                    }

//                    FormFields.Add(new AuditTrailEntry
//                    {
//                        Name = cField.Name,
//                        NewVal = IsIns && cField.Value != null ? cField.Value.ToString() : "[null]",
//                        OldVal = !IsIns && cField.Value != null ? cField.Value.ToString() : "[null]",
//                        DataRel = relation,
//                        TableName = Table
//                    });
//                }
//            }
//        }

//        private static int UpdateAuditTrail(EbWebForm _this, IDatabase DataDB, List<AuditTrailInsertData> Data)
//        {
//            List<DbParameter> parameters = new List<DbParameter>
//            {
//                DataDB.GetNewParameter("eb_createdby", EbDbTypes.Int32, _this.UserObj.UserId),
//                DataDB.GetNewParameter("eb_createdat", EbDbTypes.DateTime, DateTime.UtcNow)
//            };
//            int i = 0;
//            string fullQry = string.Empty;
//            foreach (AuditTrailInsertData data in Data)
//            {
//                parameters.Add(DataDB.GetNewParameter("formid_" + i, EbDbTypes.String, data.RefId));
//                parameters.Add(DataDB.GetNewParameter("dataid_" + i, EbDbTypes.Int32, data.TableRowId));
//                parameters.Add(DataDB.GetNewParameter("actiontype_" + i, EbDbTypes.Int32, data.Action));

//                fullQry += string.Format(@"INSERT INTO eb_audit_master(formid, dataid, actiontype, eb_createdby, eb_createdat) 
//                        VALUES (@formid_{0}, @dataid_{0}, @actiontype_{0}, @eb_createdby, @eb_createdat);", i);
//                if (DataDB.Vendor == DatabaseVendors.MYSQL)
//                    fullQry += "SELECT eb_persist_currval('eb_audit_master_id_seq');";
//                if (data.Fields.Count != 0)
//                {
//                    List<string> lineQry = new List<string>();
//                    foreach (AuditTrailEntry _field in data.Fields)
//                    {
//                        lineQry.Add(string.Format("((SELECT eb_currval('eb_audit_master_id_seq')), @{0}_{1}, @old{0}_{1}, @new{0}_{1}, @idrel{0}_{1}, @tblname{0}_{1})", _field.Name, i));
//                        parameters.Add(DataDB.GetNewParameter(_field.Name + "_" + i, EbDbTypes.String, _field.Name));
//                        parameters.Add(DataDB.GetNewParameter("new" + _field.Name + "_" + i, EbDbTypes.String, _field.NewVal));
//                        parameters.Add(DataDB.GetNewParameter("old" + _field.Name + "_" + i, EbDbTypes.String, _field.OldVal));
//                        parameters.Add(DataDB.GetNewParameter("idrel" + _field.Name + "_" + i, EbDbTypes.String, _field.DataRel));
//                        parameters.Add(DataDB.GetNewParameter("tblname" + _field.Name + "_" + i, EbDbTypes.String, _field.TableName));
//                        i++;
//                    }
//                    fullQry += string.Format("INSERT INTO eb_audit_lines(masterid, fieldname, oldvalue, newvalue, idrelation, tablename) VALUES {0};", lineQry.Join(","));
//                }
//                i++;
//            }
//            if (fullQry.IsEmpty())
//                return 0;
//            return DataDB.DoNonQuery(_this.DbConnection, fullQry, parameters.ToArray());
//        }

//        public static string GetAuditTrail(EbWebForm _this, IDatabase DataDB, Service Service)
//        {
//            _this.RefreshFormData(DataDB, Service);
//            Dictionary<string, string> DictVmAll = new Dictionary<string, string>();

//            string qry = @"	SELECT 
//            	m.id, l.id, u.fullname, m.eb_createdby, m.eb_createdat, m.actiontype, l.tablename, l.fieldname, l.idrelation, l.oldvalue, l.newvalue
//            FROM 
//            	eb_audit_master m 
//                LEFT JOIN eb_audit_lines l ON m.id = l.masterid
//                LEFT JOIN eb_users u ON m.eb_createdby = u.id
//            WHERE
//            	 m.formid = @formid AND m.dataid = @dataid
//            ORDER BY
//            	m.id DESC, l.tablename, l.idrelation;";
//            DbParameter[] parameters = new DbParameter[] {
//                     DataDB.GetNewParameter("formid", EbDbTypes.String, _this.RefId),
//                     DataDB.GetNewParameter("dataid", EbDbTypes.Int32, _this.TableRowId)
//                 };
//            EbDataTable dt = DataDB.DoQuery(qry, parameters);

//            Dictionary<int, FormTransaction> Trans = new Dictionary<int, FormTransaction>();
//            Dictionary<int, int> temp_d = new Dictionary<int, int>();
//            int counter = 1;
//            TableSchema _table = null;
//            ColumnSchema _column = null;

//            foreach (EbDataRow dr in dt.Rows)
//            {
//                int m_id = Convert.ToInt32(dr["id"]);
//                if (!temp_d.ContainsKey(m_id))
//                    temp_d.Add(m_id, counter++);
//                m_id = temp_d[m_id];
//                string new_val = dr["newvalue"].ToString();
//                string old_val = dr["oldvalue"].ToString();

//                if (Convert.ToInt32(dr["actiontype"]) != 1)
//                {
//                    if (_table == null || !_table.TableName.Equals(dr["tablename"].ToString()))
//                    {
//                        _table = _this.FormSchema.Tables.FirstOrDefault(tbl => tbl.TableName == dr["tablename"].ToString());
//                        if (_table == null)//skipping invalid Audit Trail entry
//                            continue;
//                    }

//                    if (_table.TableType != WebFormTableTypes.Grid)
//                    {
//                        _column = _table.Columns.FirstOrDefault(col => col.ColumnName == dr["fieldname"].ToString());
//                        if (_column == null)//skipping invalid Audit Trail entry
//                            continue;
//                    }
//                }

//                if (!Trans.ContainsKey(m_id))
//                {
//                    Trans.Add(m_id, new FormTransaction()
//                    {
//                        ActionType = Convert.ToInt32(dr["actiontype"]) == 1 ? "Insert" : "Update",
//                        CreatedBy = dr["fullname"].ToString(),
//                        CreatedById = dr["eb_createdby"].ToString(),
//                        CreatedAt = Convert.ToDateTime(dr["eb_createdat"]).ConvertFromUtc(_this.UserObj.Preference.TimeZone).ToString(_this.UserObj.Preference.GetShortDatePattern() + " " + _this.UserObj.Preference.GetShortTimePattern(), CultureInfo.InvariantCulture)
//                    });

//                    if (Convert.ToInt32(dr["actiontype"]) == 1)
//                        continue;
//                }

//                string[] ids = dr["idrelation"].ToString().Split('-');

//                if (_table.TableType == WebFormTableTypes.Grid)
//                {
//                    Dictionary<string, string> new_val_dict = new_val == "[null]" ? null : JsonConvert.DeserializeObject<Dictionary<string, string>>(new_val);
//                    Dictionary<string, string> old_val_dict = old_val == "[null]" ? null : JsonConvert.DeserializeObject<Dictionary<string, string>>(old_val);
//                    if (new_val_dict == null)
//                    {
//                        new_val_dict = new Dictionary<string, string>();
//                        foreach (KeyValuePair<string, string> entry in old_val_dict)
//                        {
//                            new_val_dict.Add(entry.Key, "[null]");
//                        }
//                    }
//                    else if (old_val_dict == null)
//                    {
//                        old_val_dict = new Dictionary<string, string>();
//                        foreach (KeyValuePair<string, string> entry in new_val_dict)
//                        {
//                            old_val_dict.Add(entry.Key, "[null]");
//                        }
//                    }

//                    foreach (ColumnSchema __column in _table.Columns)
//                    {
//                        if (!Trans[m_id].GridTables.ContainsKey(_table.TableName))
//                        {
//                            Trans[m_id].GridTables.Add(_table.TableName, new FormTransactionTable() { Title = _table.Title });
//                            for (int i = 0; i < _table.Columns.Count; i++)
//                            {
//                                EbControl _control = _table.Columns.ElementAt(i).Control;
//                                if (_control.DoNotPersist)
//                                    continue;
//                                if (_control is EbDGColumn)
//                                {
//                                    if (_control is EbDGUserControlColumn)
//                                        continue;
//                                    else
//                                        Trans[m_id].GridTables[_table.TableName].ColumnMeta.Add(i, (_control as EbDGColumn).Title);
//                                }
//                                else
//                                    Trans[m_id].GridTables[_table.TableName].ColumnMeta.Add(i, _control.Label);
//                            }
//                        }
//                        if (__column.Control.DoNotPersist)
//                            continue;
//                        int curid = Convert.ToInt32(ids[1]);
//                        FormTransactionTable TblRef = Trans[m_id].GridTables[_table.TableName];
//                        if (!TblRef.Rows.ContainsKey(curid))
//                        {
//                            TblRef.Rows.Add(curid, new FormTransactionRow() { });
//                        }
//                        bool IsModified = false;
//                        if (!new_val_dict.ContainsKey(__column.ColumnName))
//                            new_val_dict.Add(__column.ColumnName, "[null]");
//                        if (!old_val_dict.ContainsKey(__column.ColumnName))
//                            old_val_dict.Add(__column.ColumnName, "[null]");

//                        if (new_val_dict[__column.ColumnName] != old_val_dict[__column.ColumnName])
//                            IsModified = true;
//                        string a = old_val_dict[__column.ColumnName];
//                        string b = new_val_dict[__column.ColumnName];
//                        PreProcessTransationData(_this, DictVmAll, _table, __column, ref a, ref b);
//                        if (!TblRef.Rows[curid].Columns.ContainsKey(__column.ColumnName))
//                            TblRef.Rows[curid].Columns.Add(__column.ColumnName, new FormTransactionEntry() { OldValue = a, NewValue = b, IsModified = IsModified });
//                    }
//                }
//                else
//                {
//                    if (!Trans[m_id].Tables.ContainsKey(_table.TableName))
//                        Trans[m_id].Tables.Add(_table.TableName, new FormTransactionRow() { });

//                    PreProcessTransationData(_this, DictVmAll, _table, _column, ref old_val, ref new_val);

//                    FormTransactionEntry curtrans = new FormTransactionEntry()
//                    {
//                        OldValue = old_val,
//                        NewValue = new_val,
//                        IsModified = true,
//                        Title = _column.Control.Label
//                    };
//                    Trans[m_id].Tables[_table.TableName].Columns.Add(_column.ColumnName, curtrans);
//                }
//            }
//            PostProcessTransationData(_this, DataDB, Service, Trans, DictVmAll);

//            return JsonConvert.SerializeObject(Trans);
//        }

//        private static void PreProcessTransationData(EbWebForm _this, Dictionary<string, string> DictVmAll, TableSchema _table, ColumnSchema _column, ref string old_val, ref string new_val)
//        {
//            if (_column.Control is EbPowerSelect || _column.Control is EbDGPowerSelectColumn)//copy vm for dm
//            {
//                string key = string.Concat(_table.TableName, "_", _column.ColumnName);
//                string temp = string.Empty;
//                if (!(new_val.Equals(string.Empty) || new_val.Equals("[null]")))/////
//                    temp = string.Concat(new_val, ",");
//                if (!(old_val.Equals(string.Empty) || old_val.Equals("[null]")))/////
//                    temp += string.Concat(old_val, ",");

//                if (!temp.Equals(string.Empty))
//                {
//                    if (!DictVmAll.ContainsKey(key))
//                        DictVmAll.Add(key, temp);
//                    else
//                        DictVmAll[key] = string.Concat(DictVmAll[key], temp);
//                }
//            }
//            else if (_column.Control is EbDate || _column.Control is EbDGDateColumn)
//            {
//                EbDateType _type = _column.Control is EbDate ? (_column.Control as EbDate).EbDateType : (_column.Control as EbDGDateColumn).EbDateType;
//                DateShowFormat _showtype = _column.Control is EbDate ? (_column.Control as EbDate).ShowDateAs_ : (_column.Control as EbDGDateColumn).EbDate.ShowDateAs_;
//                if (!old_val.Equals("[null]"))
//                {
//                    if (_type == EbDateType.Date)
//                    {
//                        if (_showtype != DateShowFormat.Year_Month)
//                            old_val = DateTime.ParseExact(old_val, "yyyy-MM-dd", CultureInfo.InvariantCulture).ToString(_this.UserObj.Preference.GetShortDatePattern(), CultureInfo.InvariantCulture);
//                    }
//                    else if (_type == EbDateType.DateTime)
//                    {
//                        old_val = DateTime.ParseExact(old_val, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture).ToString(_this.UserObj.Preference.GetShortDatePattern() + " " + _this.UserObj.Preference.GetShortTimePattern(), CultureInfo.InvariantCulture);
//                        //old_val = dt.ConvertFromUtc(this.UserObj.Preference.TimeZone);
//                    }
//                    else if (_type == EbDateType.Time)
//                    {
//                        old_val = DateTime.ParseExact(old_val, "HH:mm:ss", CultureInfo.InvariantCulture).ToString(_this.UserObj.Preference.GetShortTimePattern(), CultureInfo.InvariantCulture);
//                        //old_val = dt.ConvertFromUtc(this.UserObj.Preference.TimeZone);
//                    }
//                }
//                if (!new_val.Equals("[null]"))
//                {
//                    if (_type == EbDateType.Date)
//                    {
//                        if (_showtype != DateShowFormat.Year_Month)
//                            new_val = DateTime.ParseExact(new_val, "yyyy-MM-dd", CultureInfo.InvariantCulture).ToString(_this.UserObj.Preference.GetShortDatePattern(), CultureInfo.InvariantCulture);
//                    }
//                    else if (_type == EbDateType.DateTime)
//                    {
//                        new_val = DateTime.ParseExact(new_val, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture).ToString(_this.UserObj.Preference.GetShortDatePattern() + " " + _this.UserObj.Preference.GetShortTimePattern(), CultureInfo.InvariantCulture);
//                    }
//                    else if (_type == EbDateType.Time)
//                    {
//                        new_val = DateTime.ParseExact(new_val, "HH:mm:ss", CultureInfo.InvariantCulture).ToString(_this.UserObj.Preference.GetShortTimePattern(), CultureInfo.InvariantCulture);
//                    }
//                }
//            }
//            else if (_column.Control is EbUserSelect)
//            {
//                if (int.TryParse(old_val, out int j))
//                    old_val = _this.SolutionObj.Users.ContainsKey(j) ? _this.SolutionObj.Users[j] : old_val;
//                if (int.TryParse(new_val, out int i))
//                    new_val = _this.SolutionObj.Users.ContainsKey(i) ? _this.SolutionObj.Users[i] : new_val;
//            }
//        }

//        private static void PostProcessTransationData(EbWebForm _this, IDatabase DataDB, Service Service, Dictionary<int, FormTransaction> Trans, Dictionary<string, string> DictVmAll)
//        {
//            string Qry = string.Empty;
//            foreach (TableSchema _table in _this.FormSchema.Tables)
//            {
//                foreach (ColumnSchema _column in _table.Columns)
//                {
//                    if (_column.Control is EbPowerSelect || _column.Control is EbDGPowerSelectColumn)
//                    {
//                        string key = string.Concat(_table.TableName, "_", _column.ColumnName);
//                        if (DictVmAll.ContainsKey(key))
//                        {
//                            if (_column.Control is EbPowerSelect)
//                                Qry += (_column.Control as EbPowerSelect).GetDisplayMembersQuery(DataDB, Service, DictVmAll[key].Substring(0, DictVmAll[key].Length - 1));
//                            else
//                                Qry += (_column.Control as EbDGPowerSelectColumn).GetDisplayMembersQuery(DataDB, Service, DictVmAll[key].Substring(0, DictVmAll[key].Length - 1));
//                        }
//                    }
//                }
//            }

//            EbDataSet ds = DataDB.DoQueries(Qry);

//            Dictionary<string, Dictionary<string, List<string>>> DictDm = new Dictionary<string, Dictionary<string, List<string>>>();
//            foreach (string key in DictVmAll.Keys)
//                DictDm.Add(key, new Dictionary<string, List<string>>());

//            for (int i = 0; i < ds.Tables.Count; i++)
//            {
//                foreach (EbDataRow row in ds.Tables[i].Rows)
//                {
//                    List<string> list = new List<string>();
//                    for (int j = 1; j < row.Count; j++)
//                    {
//                        list.Add(row[j].ToString());
//                    }
//                    if (!DictDm.ElementAt(i).Value.ContainsKey(row[0].ToString()))
//                        DictDm.ElementAt(i).Value.Add(row[0].ToString(), list);
//                }
//            }

//            foreach (KeyValuePair<int, FormTransaction> trans in Trans)
//            {
//                foreach (KeyValuePair<string, FormTransactionRow> table in trans.Value.Tables)
//                {
//                    ReplaceVmWithDm(table.Value.Columns, DictDm, table.Key);
//                }

//                foreach (KeyValuePair<string, FormTransactionTable> table in trans.Value.GridTables)
//                {
//                    foreach (KeyValuePair<int, FormTransactionRow> row in table.Value.Rows)
//                    {
//                        ReplaceVmWithDm(row.Value.Columns, DictDm, table.Key);
//                    }
//                }
//            }
//        }

//        private static void ReplaceVmWithDm(Dictionary<string, FormTransactionEntry> Columns, Dictionary<string, Dictionary<string, List<string>>> DictDm, string tablename)
//        {
//            foreach (KeyValuePair<string, FormTransactionEntry> column in Columns)
//            {
//                if (DictDm.ContainsKey(tablename + "_" + column.Key))
//                {
//                    if (column.Value.OldValue != "[null]")
//                    {
//                        string[] vm_arr = column.Value.OldValue.Split(',');
//                        string dm = string.Empty;
//                        for (int i = 0; i < vm_arr.Length; i++)
//                        {
//                            List<string> dmlist = DictDm[tablename + "_" + column.Key][vm_arr[i]];
//                            foreach (string d in dmlist)
//                            {
//                                dm += " " + d;
//                            }
//                            if (i < vm_arr.Length - 1)
//                                dm += "<br>";
//                        }
//                        column.Value.OldValue = dm;
//                    }
//                    if (column.Value.NewValue != "[null]")
//                    {
//                        string[] vm_arr = column.Value.NewValue.Split(',');
//                        string dm = string.Empty;
//                        for (int i = 0; i < vm_arr.Length; i++)
//                        {
//                            List<string> dmlist = DictDm[tablename + "_" + column.Key][vm_arr[i]];
//                            foreach (string d in dmlist)
//                            {
//                                dm += " " + d;
//                            }
//                            if (i < vm_arr.Length - 1)
//                                dm += "<br>";
//                        }
//                        column.Value.NewValue = dm;
//                    }
//                }
//            }
//        }
//    }
//}
