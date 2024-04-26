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
using System.Text;

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
                    string _q = QueryGetter.GetInsertQuery(WebForm, DataDB, WebForm.FormSchema.MasterTable, true, true);
                    fullqry += string.Format(_q, string.Empty, string.Empty);
                }
                foreach (TableSchema _table in WebForm.FormSchema.Tables.FindAll(e => e.TableType != WebFormTableTypes.Review && !e.DoNotPersist))
                {
                    if (!WebForm.FormData.MultipleTables.ContainsKey(_table.TableName))
                        continue;

                    int rowCounter = 0;
                    int totalInsertCount = WebForm.FormData.MultipleTables[_table.TableName].Count;

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

                        string _qry = QueryGetter.GetInsertQuery(WebForm, DataDB, _table.TableName, true, rowCounter == 0);
                        rowCounter++;
                        if (totalInsertCount == rowCounter && _table.TableName != WebForm.TableName)
                            _qry += "; ";

                        fullqry += string.Format(_qry, args._cols, args._vals);

                        fullqry += WebForm.InsertUpdateLines(_table.TableName, row, args);
                    }
                }
                AddSysParam(DataDB, param, WebForm.TableName + FormConstants._eb_ver_id, EbDbTypes.Int32, WebForm.RefId.Split(CharConstants.DASH)[4]);
                AddSysParam(DataDB, param, WebForm.TableName + FormConstants._refid, EbDbTypes.String, WebForm.RefId);
                AddSysParam(DataDB, param, FormConstants.eb_loc_id_ + WebForm.CrudContext, EbDbTypes.Int32, WebForm.LocationId);

                if (pushAuditTrail)
                {
                    fullqry += EbAuditTrail.GetInsertModeQuery(DataDB, WebForm.RefId, WebForm.TableName);
                    fullqry += WebForm.MatViewConfig.GetInsertModeQuery(false);
                }
            }

            args.CopyBack(ref _extqry, ref i);
        }

        public void Update(IDatabase DataDB, List<DbParameter> param, ref string fullqry, ref string _extqry, ref int i)
        {
            ParameterizeCtrl_Params args = new ParameterizeCtrl_Params(DataDB, param, i, _extqry);
            string eb_row_num = this[0].SolutionObj.SolutionSettings.SystemColumns[SystemColumns.eb_row_num];
            foreach (EbWebForm WebForm in this)
            {
                args.SetFormRelated(WebForm.TableName, WebForm.UserObj, WebForm);
                WebForm.DoRequiredCheck(WebForm == MasterForm);

                foreach (TableSchema _table in WebForm.FormSchema.Tables.FindAll(e => e.TableType != WebFormTableTypes.Review && !e.DoNotPersist))
                {
                    if (!WebForm.FormData.MultipleTables.ContainsKey(_table.TableName))
                        continue;

                    int rowCounter = 0;
                    int totalInsertCount = WebForm.FormData.MultipleTables[_table.TableName].FindAll(e => e.RowId <= 0).Count;

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
                            else if (_table.TableType == WebFormTableTypes.Grid && !row.IsDelete)
                            {
                                bool ValChangeFound = false;
                                foreach (SingleColumn Column in row.Columns)
                                {
                                    if (Column.Control == null && Column.Name != eb_row_num)
                                        continue;
                                    SingleColumn ocF = bkup_Row.Columns.Find(e => e.Name.Equals(Column.Name));
                                    if (ocF == null || Convert.ToString(ocF?.Value) != Convert.ToString(Column.Value))
                                    {
                                        ValChangeFound = true;
                                        break;
                                    }
                                }
                                if (!ValChangeFound && bkup_Row.LocId != WebForm.LocationId)
                                    ValChangeFound = true;
                                if (!ValChangeFound)
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
                            //else if (WebForm.DataPusherConfig == null && !_table.TableName.Equals(WebForm.TableName))
                            //{
                            //    List<TableSchema> _tables = WebForm.FormSchema.Tables.FindAll(e => e.IsDynamic && e.TableType == WebFormTableTypes.Grid);
                            //    foreach (TableSchema _tbl in _tables)
                            //    {
                            //        t += $@"UPDATE {_tbl.TableName} SET eb_del = 'T', eb_lastmodified_by = @eb_modified_by, eb_lastmodified_at = {DataDB.EB_CURRENT_TIMESTAMP} WHERE
                            //            {_table.TableName}_id = @{_table.TableName}_id_{args.i} AND {WebForm.TableName}_id = @{WebForm.TableName}_id AND COALESCE(eb_del, 'F') = 'F'; ";
                            //        param.Add(DataDB.GetNewParameter(_table.TableName + "_id_" + args.i, EbDbTypes.Int32, row.RowId));
                            //        args.i++;
                            //    }
                            //}
                            bool DGCustSelect = _table.TableType == WebFormTableTypes.Grid && !string.IsNullOrWhiteSpace(_table.CustomSelectQuery);

                            string _qry = QueryGetter.GetUpdateQuery(WebForm, DataDB, _table.TableName, row.IsDelete, DGCustSelect);
                            if (rowCounter > 0)
                            {
                                fullqry += "; ";
                                totalInsertCount -= rowCounter;
                                rowCounter = 0;
                            }
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
                            string _qry = QueryGetter.GetInsertQuery(WebForm, DataDB, _table.TableName, WebForm.TableRowId == 0, rowCounter == 0);
                            rowCounter++;
                            if (totalInsertCount == rowCounter && _table.TableName != WebForm.TableName)
                                _qry += "; ";

                            fullqry += string.Format(_qry, args._cols, args._vals);

                        }
                        fullqry += WebForm.InsertUpdateLines(_table.TableName, row, args);
                    }
                }
                string IdParamName = WebForm.TableName + FormConstants._id + (WebForm.DataPusherConfig != null ? WebForm.CrudContext : string.Empty);
                AddSysParam(DataDB, param, IdParamName, EbDbTypes.Int32, WebForm.TableRowId);
                AddSysParam(DataDB, param, WebForm.TableName + FormConstants._eb_ver_id, EbDbTypes.Int32, WebForm.RefId.Split(CharConstants.DASH)[4]);
                AddSysParam(DataDB, param, WebForm.TableName + FormConstants._refid, EbDbTypes.String, WebForm.RefId);
                AddSysParam(DataDB, param, FormConstants.eb_loc_id_ + WebForm.CrudContext, EbDbTypes.Int32, WebForm.LocationId);
            }
            args.CopyBack(ref _extqry, ref i);
        }

        private void AddSysParam(IDatabase DataDB, List<DbParameter> pList, string pName, EbDbTypes dbType, object value)
        {
            if (pList.Exists(e => e.ParameterName == pName))
                pList.RemoveAll(e => e.ParameterName == pName);
            pList.Add(DataDB.GetNewParameter(pName, dbType, value));
        }

        public void Update_Batch(IDatabase DataDB, List<DbParameter> param, StringBuilder fullqry, ref string _extqry, ref int i)
        {
            ParameterizeCtrl_Params args = new ParameterizeCtrl_Params(DataDB, param, i, _extqry);
            foreach (EbWebForm WebForm in this)
            {
                args.SetFormRelated(WebForm.TableName, WebForm.UserObj, WebForm);
                EbDataPusherConfig conf = WebForm.DataPusherConfig;

                foreach (TableSchema _table in WebForm.FormSchema.Tables.FindAll(e => e.TableType != WebFormTableTypes.Review && !e.DoNotPersist))
                {
                    if (!WebForm.FormData.MultipleTables.ContainsKey(_table.TableName))
                        continue;

                    int totalInsertCount = WebForm.FormData.MultipleTables[_table.TableName].FindAll(e => e.RowId <= 0).Count;
                    int rowCounter = 0;
                    foreach (SingleRow row in WebForm.FormData.MultipleTables[_table.TableName])
                    {
                        args.ResetColVals();
                        if (row.RowId > 0)//edit
                        {
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
                            bool DGCustSelect = _table.TableType == WebFormTableTypes.Grid && !string.IsNullOrWhiteSpace(_table.CustomSelectQuery);

                            string _qry = QueryGetter.GetUpdateQuery_Batch(WebForm, DataDB, _table.TableName, row.IsDelete, row.RowId, DGCustSelect);
                            if (rowCounter > 0)
                            {
                                fullqry.Append("; ");
                                totalInsertCount -= rowCounter;
                                rowCounter = 0;
                            }
                            fullqry.Append(string.Format(_qry, args._colvals));
                            fullqry.Append(t);
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
                            string _qry = QueryGetter.GetInsertQuery_Batch(WebForm, DataDB, _table.TableName, _table, rowCounter == 0);
                            rowCounter++;

                            if (totalInsertCount == rowCounter && _table.TableName != WebForm.TableName)
                                _qry += "; ";

                            fullqry.Append(string.Format(_qry, args._cols, args._vals));
                        }
                        fullqry.Append(WebForm.InsertUpdateLines(_table.TableName, row, args));
                    }
                }

                AddSysParam(DataDB, param, WebForm.TableName + FormConstants._eb_ver_id, EbDbTypes.Int32, WebForm.RefId.Split(CharConstants.DASH)[4]);
                AddSysParam(DataDB, param, WebForm.TableName + FormConstants._refid, EbDbTypes.String, WebForm.RefId);
                AddSysParam(DataDB, param, FormConstants.eb_loc_id_ + WebForm.CrudContext, EbDbTypes.Int32, WebForm.LocationId);
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

                        if (cField == null || string.IsNullOrWhiteSpace(Convert.ToString(cField.Value)) || (Double.TryParse(Convert.ToString(cField.Value), out double __val) && __val == 0))
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
                        int Pos = 0;
                        Dictionary<string, int> Vals = new Dictionary<string, int>();//<Value, Position>
                        for (int i = 0; i < Table.Count; i++)
                            CheckDGUniqe(Table[i], _column, _table, Vals, WebForm, ref Pos);

                        if (WebForm.FormDataBackup != null && WebForm.FormDataBackup.MultipleTables.TryGetValue(_table.TableName, out SingleTable TableBkUp) && TableBkUp.Count > 0)
                        {
                            for (int i = 0; i < TableBkUp.Count; i++)
                            {
                                SingleRow Row = Table.Find(e => e.RowId == TableBkUp[i].RowId);
                                if (Row == null)
                                    CheckDGUniqe(TableBkUp[i], _column, _table, Vals, WebForm, ref Pos);
                            }
                        }
                    }
                }
            }
        }

        private void CheckDGUniqe(SingleRow Row, ColumnSchema _column, TableSchema _table, Dictionary<string, int> Vals, EbWebForm WebForm, ref int Pos)
        {
            if (Row.IsDelete)
                return;
            Pos++;
            SingleColumn cField = Row.GetColumn(_column.ColumnName);
            if (cField == null || string.IsNullOrWhiteSpace(Convert.ToString(cField.Value)) || (Double.TryParse(Convert.ToString(cField.Value), out double __val) && __val == 0))
                return;
            string stVal = Convert.ToString(cField.Value);
            if (Vals.ContainsKey(stVal))
            {
                string msg = $"Error in Grid '{_table.Title ?? _table.ContainerName}' Row#{Vals[stVal]}, #{Pos}: Duplicate value in unique column '{(_column.Control as EbDGColumn).Title ?? _column.Control.Name}'";
                msg += $" {(WebForm == MasterForm ? "" : "(DataPusher)")} {(_column.Control.Hidden ? "[Hidden]" : "")}";
                throw new FormException(msg, (int)HttpStatusCode.BadRequest, msg, "EbWebFormCollection -> ExecUniqueCheck");
            }
            Vals.Add(stVal, Pos);
        }

    }
}
