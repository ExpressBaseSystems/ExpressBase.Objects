using ExpressBase.Common;
using ExpressBase.Common.Constants;
using ExpressBase.Common.LocationNSolution;
using ExpressBase.Common.Objects;
using ExpressBase.Common.Structures;
using ExpressBase.Objects.ServiceStack_Artifacts;
using ExpressBase.Objects.WebFormRelated;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Net;

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

        public EbWebFormCollection(List<EbWebForm> ebWebFormList) : base(ebWebFormList)
        {
            if (ebWebFormList.Count > 0)
                this.MasterForm = ebWebFormList[0];/////////dummy master
        }

        public void Insert(IDatabase DataDB, List<DbParameter> param, ref string fullqry, ref string _extqry, ref int i, bool pushAuditTrail)
        {
            ParameterizeCtrl_Params args = new ParameterizeCtrl_Params(DataDB, param, i, _extqry);
            foreach (EbWebForm WebForm in this)
            {
                args.SetFormRelated(WebForm.TableName, WebForm.UserObj, WebForm);

                if (WebForm.DataPusherConfig?.AllowPush == false)
                    continue;
                WebForm.DoRequiredCheck(WebForm == MasterForm);
                if (!(WebForm.FormData.MultipleTables.ContainsKey(WebForm.FormSchema.MasterTable) && WebForm.FormData.MultipleTables[WebForm.FormSchema.MasterTable].Count > 0))
                {
                    string _q = QueryGetter.GetInsertQuery(WebForm, DataDB, WebForm.FormSchema.MasterTable, true);
                    fullqry += string.Format(_q, string.Empty, string.Empty);
                }
                foreach (TableSchema _table in WebForm.FormSchema.Tables.FindAll(e => e.TableType != WebFormTableTypes.Review && !e.DoNotPersist))
                {
                    if (!WebForm.FormData.MultipleTables.ContainsKey(_table.TableName))
                        continue;

                    foreach (SingleRow row in WebForm.FormData.MultipleTables[_table.TableName])
                    {
                        args.ResetColsAndVals();

                        foreach (SingleColumn cField in row.Columns)
                        {
                            args.InsertSet(cField);

                            if (cField.Control != null)
                                cField.Control.ParameterizeControl(args, WebForm.CrudContext);
                            else
                                WebForm.ParameterizeUnknown(args);
                        }

                        string _qry = QueryGetter.GetInsertQuery(WebForm, DataDB, _table.TableName, true);
                        fullqry += string.Format(_qry, args._cols, args._vals);

                        fullqry += WebForm.InsertUpdateLines(_table.TableName, row, args);
                    }
                }
                if (!param.Exists(e => e.ParameterName == WebForm.TableName + FormConstants._eb_ver_id))
                {
                    param.Add(DataDB.GetNewParameter(WebForm.TableName + FormConstants._eb_ver_id, EbDbTypes.Int32, WebForm.RefId.Split(CharConstants.DASH)[4]));
                    param.Add(DataDB.GetNewParameter(WebForm.TableName + FormConstants._refid, EbDbTypes.String, WebForm.RefId));
                }

                if (pushAuditTrail)
                    fullqry += EbAuditTrail.GetInsertModeQuery(DataDB, WebForm.RefId, WebForm.TableName);
            }

            args.CopyBack(ref _extqry, ref i);
        }

        public void Update(IDatabase DataDB, List<DbParameter> param, ref string fullqry, ref string _extqry, ref int i)
        {
            ParameterizeCtrl_Params args = new ParameterizeCtrl_Params(DataDB, param, i, _extqry);
            foreach (EbWebForm WebForm in this)
            {
                args.SetFormRelated(WebForm.TableName, WebForm.UserObj, WebForm);
                WebForm.DoRequiredCheck(WebForm == MasterForm);

                foreach (TableSchema _table in WebForm.FormSchema.Tables.FindAll(e => e.TableType != WebFormTableTypes.Review && !e.DoNotPersist))
                {
                    if (!WebForm.FormData.MultipleTables.ContainsKey(_table.TableName))
                        continue;

                    foreach (SingleRow row in WebForm.FormData.MultipleTables[_table.TableName])
                    {
                        args.ResetColVals();
                        if (row.RowId > 0)
                        {
                            SingleRow bkup_Row = WebForm.FormDataBackup.MultipleTables[_table.TableName].Find(e => e.RowId == row.RowId);
                            if (bkup_Row == null)
                            {
                                Console.WriteLine($"Row edit request ignored(Row not in backup table). \nTable name: {_table.TableName}, RowId: {row.RowId}, RefId: {WebForm.RefId}");
                                continue;
                            }
                            string t = string.Empty;
                            if (!row.IsDelete)
                            {
                                foreach (SingleColumn cField in row.Columns)
                                {
                                    if (cField.Control != null)
                                    {
                                        SingleColumn ocF = bkup_Row.Columns.Find(e => e.Name.Equals(cField.Name));
                                        args.UpdateSet(cField, ocF);
                                        cField.Control.ParameterizeControl(args, WebForm.CrudContext);
                                    }
                                    else
                                    {
                                        args.UpdateSet(cField);
                                        WebForm.ParameterizeUnknown(args);
                                    }
                                }
                            }
                            else if (WebForm.DataPusherConfig == null && !_table.TableName.Equals(WebForm.TableName))
                            {
                                List<TableSchema> _tables = WebForm.FormSchema.Tables.FindAll(e => e.IsDynamic && e.TableType == WebFormTableTypes.Grid);
                                foreach (TableSchema _tbl in _tables)
                                {
                                    t += $@"UPDATE {_tbl.TableName} SET eb_del = 'T', eb_lastmodified_by = @eb_modified_by, eb_lastmodified_at = {DataDB.EB_CURRENT_TIMESTAMP} WHERE
                                        {_table.TableName}_id = @{_table.TableName}_id_{args.i} AND {WebForm.TableName}_id = @{WebForm.TableName}_id AND COALESCE(eb_del, 'F') = 'F'; ";
                                    param.Add(DataDB.GetNewParameter(_table.TableName + "_id_" + args.i, EbDbTypes.Int32, row.RowId));
                                    args.i++;
                                }
                            }

                            string _qry = QueryGetter.GetUpdateQuery(WebForm, DataDB, _table.TableName, row.IsDelete);
                            fullqry += string.Format(_qry, args._colvals, row.RowId);
                            fullqry += t;
                        }
                        else
                        {
                            args.ResetColsAndVals();

                            foreach (SingleColumn cField in row.Columns)
                            {
                                args.InsertSet(cField);
                                if (cField.Control != null)
                                    cField.Control.ParameterizeControl(args, WebForm.CrudContext);
                                else
                                    WebForm.ParameterizeUnknown(args);
                            }
                            string _qry = QueryGetter.GetInsertQuery(WebForm, DataDB, _table.TableName, WebForm.TableRowId == 0);
                            fullqry += string.Format(_qry, args._cols, args._vals);
                        }
                        fullqry += WebForm.InsertUpdateLines(_table.TableName, row, args);
                    }
                }
                string IdParamName = WebForm.TableName + FormConstants._id + (WebForm.DataPusherConfig != null ? WebForm.CrudContext : string.Empty);
                param.Add(DataDB.GetNewParameter(IdParamName, EbDbTypes.Int32, WebForm.TableRowId));
                if (!param.Exists(e => e.ParameterName == WebForm.TableName + FormConstants._eb_ver_id))
                {
                    param.Add(DataDB.GetNewParameter(WebForm.TableName + FormConstants._eb_ver_id, EbDbTypes.Int32, WebForm.RefId.Split(CharConstants.DASH)[4]));
                    param.Add(DataDB.GetNewParameter(WebForm.TableName + FormConstants._refid, EbDbTypes.String, WebForm.RefId));
                }
            }
            args.CopyBack(ref _extqry, ref i);
        }

        public void Update_Batch(IDatabase DataDB, List<DbParameter> param, ref string fullqry, ref string _extqry, ref int i)
        {
            ParameterizeCtrl_Params args = new ParameterizeCtrl_Params(DataDB, param, i, _extqry);
            foreach (EbWebForm WebForm in this)
            {
                args.SetFormRelated(WebForm.TableName, WebForm.UserObj, WebForm);
                EbDataPusherConfig conf = WebForm.DataPusherConfig;

                foreach (KeyValuePair<string, SingleTable> entry in WebForm.FormData.MultipleTables)
                {
                    foreach (SingleRow row in entry.Value)
                    {
                        args.ResetColVals();
                        if (row.RowId > 0)
                        {
                            //SingleRow bkup_Row = WebForm.FormDataBackup.MultipleTables[entry.Key].Find(e => e.RowId == row.RowId);
                            //if (bkup_Row == null)
                            //{
                            //    Console.WriteLine($"Row edit request ignored(Row not in backup table). \nTable name: {entry.Key}, RowId: {row.RowId}, RefId: {WebForm.RefId}");
                            //    continue;
                            //}
                            string t = string.Empty;
                            if (!row.IsDelete)
                            {
                                foreach (SingleColumn cField in row.Columns)
                                {
                                    if (cField.Control != null)
                                    {
                                        SingleColumn ocF = null;// bkup_Row.Columns.Find(e => e.Name.Equals(cField.Name));
                                        args.UpdateSet(cField, ocF);
                                        cField.Control.ParameterizeControl(args, WebForm.CrudContext);
                                    }
                                    else
                                    {
                                        args.UpdateSet(cField);
                                        WebForm.ParameterizeUnknown(args);
                                    }
                                }
                            }

                            string _qry = QueryGetter.GetUpdateQuery_Batch(WebForm, DataDB, entry.Key, row.IsDelete, row.RowId);
                            fullqry += string.Format(_qry, args._colvals);
                            fullqry += t;
                        }
                        else
                        {
                            args.ResetColsAndVals();

                            foreach (SingleColumn cField in row.Columns)
                            {
                                args.InsertSet(cField);
                                if (cField.Control != null)
                                    cField.Control.ParameterizeControl(args, WebForm.CrudContext);
                                else
                                    WebForm.ParameterizeUnknown(args);
                            }
                            string _qry = QueryGetter.GetInsertQuery_Batch(WebForm, DataDB, entry.Key);
                            fullqry += string.Format(_qry, args._cols, args._vals);
                        }
                        fullqry += WebForm.InsertUpdateLines(entry.Key, row, args);
                    }
                }
                if (!param.Exists(e => e.ParameterName == WebForm.TableName + FormConstants._eb_ver_id))
                {
                    param.Add(DataDB.GetNewParameter(WebForm.TableName + FormConstants._eb_ver_id, EbDbTypes.Int32, WebForm.RefId.Split(CharConstants.DASH)[4]));
                    param.Add(DataDB.GetNewParameter(WebForm.TableName + FormConstants._refid, EbDbTypes.String, WebForm.RefId));
                }
            }
            args.CopyBack(ref _extqry, ref i);
        }

        public void ExecUniqueCheck(IDatabase DataDB, DbConnection DbCon)
        {
            string fullQuery = string.Empty;
            List<DbParameter> Dbparams = new List<DbParameter>();
            List<EbControl> UniqueCtrls = new List<EbControl>();
            int paramCounter = 0, mstrFormCtrls = 0;
            EbSystemColumns SysCols = this.MasterForm.SolutionObj.SolutionSettings.SystemColumns;

            foreach (EbWebForm WebForm in this)
            {
                if (WebForm.DataPusherConfig?.AllowPush == false)
                    continue;

                foreach (TableSchema _table in WebForm.FormSchema.Tables.FindAll(e => e.TableType == WebFormTableTypes.Normal))
                {
                    if (!(WebForm.FormData.MultipleTables.TryGetValue(_table.TableName, out SingleTable Table) && Table.Count > 0))
                        continue;

                    foreach (ColumnSchema _column in _table.Columns.FindAll(e => e.Control.Unique && !e.Control.BypassParameterization))
                    {
                        SingleColumn cField = Table[0].GetColumn(_column.ColumnName);

                        if (cField == null || cField.Value == null || (Double.TryParse(Convert.ToString(cField.Value), out double __val) && __val == 0))
                            continue;

                        if (WebForm.FormDataBackup != null)
                        {
                            if (WebForm.FormDataBackup.MultipleTables.TryGetValue(_table.TableName, out SingleTable TableBkUp) && TableBkUp.Count > 0)
                            {
                                SingleColumn ocField = TableBkUp[0].GetColumn(_column.ColumnName);
                                if (ocField != null && Convert.ToString(cField.Value).Trim().ToLower() == Convert.ToString(ocField.Value ?? string.Empty).Trim().ToLower())
                                    continue;
                            }
                        }

                        fullQuery += string.Format("SELECT id FROM {0} WHERE {5}{1}{6} = {5}@{1}_{2}{6} AND COALESCE({3}, {4}) = {4} AND id <> {7};",//check eb_ver_id here!
                            _table.TableName,//0
                            _column.ColumnName,//1
                            paramCounter,//2
                            SysCols[SystemColumns.eb_del],//3
                            SysCols.GetBoolFalse(SystemColumns.eb_del),//4
                            _column.Control.EbDbType == EbDbTypes.String ? "LOWER(TRIM(" : string.Empty,//5
                            _column.Control.EbDbType == EbDbTypes.String ? "))" : string.Empty,//6
                            WebForm.TableRowId);//7
                        Dbparams.Add(DataDB.GetNewParameter($"{_column.ColumnName}_{paramCounter++}", _column.Control.EbDbType, cField.Value));
                        UniqueCtrls.Add(_column.Control);
                        if (WebForm == MasterForm)
                            mstrFormCtrls++;
                    }
                }
            }

            if (fullQuery != string.Empty)
            {
                EbDataSet ds;
                if (DbCon == null)
                    ds = DataDB.DoQueries(fullQuery, Dbparams.ToArray());
                else
                    ds = DataDB.DoQueries(DbCon, fullQuery, Dbparams.ToArray());

                for (int i = 0; i < ds.Tables.Count; i++)
                {
                    if (ds.Tables[i].Rows.Count > 0)
                    {
                        if (mstrFormCtrls > i)
                            throw new FormException($"{UniqueCtrls[i].Label} must be unique", (int)HttpStatusCode.BadRequest, $"Value of {UniqueCtrls[i].Label} is not unique. Control name: {UniqueCtrls[i].Name}", "EbWebFormCollection -> ExecUniqueCheck");
                        else
                            throw new FormException($"{UniqueCtrls[i].Label} in data pusher must be unique", (int)HttpStatusCode.BadRequest, $"Value of {UniqueCtrls[i].Label} in data pusher is not unique. Control name: {UniqueCtrls[i].Name}", "EbWebFormCollection -> ExecUniqueCheck");
                    }
                }
            }
        }

        public void ExecDGUniqueCheck()
        {
            foreach (EbWebForm WebForm in this)
            {
                if (WebForm.DataPusherConfig?.AllowPush == false)
                    continue;

                foreach (TableSchema _table in WebForm.FormSchema.Tables.FindAll(e => e.TableType == WebFormTableTypes.Grid))
                {
                    if (!(WebForm.FormData.MultipleTables.TryGetValue(_table.TableName, out SingleTable Table) && Table.Count > 0))
                        continue;

                    foreach (ColumnSchema _column in _table.Columns.FindAll(e => e.Control.Unique))
                    {
                        List<string> Vals = new List<string>();
                        foreach (SingleRow Row in Table)
                            CheckDGUniqe(Row, _column, _table, Vals, WebForm);

                        if (WebForm.FormDataBackup != null && WebForm.FormDataBackup.MultipleTables.TryGetValue(_table.TableName, out SingleTable TableBkUp) && TableBkUp.Count > 0)
                        {
                            foreach (SingleRow RowBkUp in TableBkUp)
                            {
                                SingleRow Row = Table.Find(e => e.RowId == RowBkUp.RowId);
                                if (Row == null)
                                    CheckDGUniqe(RowBkUp, _column, _table, Vals, WebForm);
                            }
                        }
                    }
                }
            }
        }

        private void CheckDGUniqe(SingleRow Row, ColumnSchema _column, TableSchema _table, List<string> Vals, EbWebForm WebForm)
        {
            if (Row.IsDelete)
                return;
            SingleColumn cField = Row.GetColumn(_column.ColumnName);
            if (cField == null || cField.Value == null || (Double.TryParse(Convert.ToString(cField.Value), out double __val) && __val == 0))
                return;
            if (Vals.Contains(Convert.ToString(cField.Value)))
            {
                string msg2 = $" {(WebForm == MasterForm ? "" : "(DataPusher)")} {(cField.Control.Hidden ? "[Hidden]" : "")}";
                string msg1 = $"Value in the '{(cField.Control as EbDGColumn).Title ?? cField.Control.Name}' column ({_table.Title ?? _table.ContainerName} Grid) must be unique" + msg2;
                msg2 = $"DG column is not unique. Control name: {_table.ContainerName}.{cField.Control.Name}" + msg2;
                throw new FormException(msg1, (int)HttpStatusCode.BadRequest, msg2, "EbWebFormCollection -> ExecUniqueCheck");
            }
            Vals.Add(Convert.ToString(cField.Value));
        }

    }
}
