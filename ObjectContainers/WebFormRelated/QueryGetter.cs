using ExpressBase.Common;
using ExpressBase.Common.Constants;
using ExpressBase.Common.LocationNSolution;
using ExpressBase.Common.Objects;
using ServiceStack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace ExpressBase.Objects.WebFormRelated
{
    static class QueryGetter
    {
        public static string GetSelectQuery(EbWebForm _this, IDatabase DataDB, Service _service, out int _qryCount)
        {
            string query = string.Empty;
            string extquery = string.Empty;
            _qryCount = 0;
            EbSystemColumns ebs = _this.SolutionObj.SolutionSettings.SystemColumns;
            string _pshId = _this.DataPusherConfig?.MultiPushId == null ? string.Empty : $"AND {ebs[SystemColumns.eb_push_id]} = '{_this.DataPusherConfig.MultiPushId}'";
            string form_ver_id = _this.RefId.Split(CharConstants.DASH)[4];

            foreach (TableSchema _table in _this.FormSchema.Tables)
            {
                if (_table.DoNotPersist)
                    continue;
                string _cols = $"{ebs[SystemColumns.eb_loc_id]}, id";
                string _id = "id";

                if (_table.TableName == _this.FormSchema.MasterTable)
                    _cols = string.Format("{0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11}{12}{13}, id",
                        ebs[SystemColumns.eb_loc_id],//0
                        ebs[SystemColumns.eb_ver_id],//1
                        ebs[SystemColumns.eb_lock],//2
                        ebs[SystemColumns.eb_push_id],//3 //need only for slave forms in datapusher
                        ebs[SystemColumns.eb_src_id],//4
                        ebs[SystemColumns.eb_created_by],//5
                        ebs[SystemColumns.eb_void],//6
                        ebs[SystemColumns.eb_created_at],//7
                        ebs[SystemColumns.eb_src_ver_id],//8
                        ebs[SystemColumns.eb_ro],//9
                        ebs[SystemColumns.eb_lastmodified_by],//10
                        ebs[SystemColumns.eb_lastmodified_at],//11
                        _this.CancelReason ? ", " + ebs[SystemColumns.eb_void_reason] : string.Empty,//12
                        _this.MultiLocAccess ? ", " + SystemColumns.eb_loc_permissions : string.Empty);//13
                else if (_table.TableType == WebFormTableTypes.Review)
                {
                    _id = $"eb_ver_id = {form_ver_id} AND eb_src_id";
                    _cols = "eb_loc_id, id";
                }
                else
                    _id = _this.FormSchema.MasterTable + "_id";

                if (_table.TableType == WebFormTableTypes.Grid)
                {
                    if (!string.IsNullOrWhiteSpace(_table.CustomSelectQuery))
                    {
                        string Id;
                        if (_this.DataPusherConfig == null)
                        {
                            Id = $"@{_this.FormSchema.MasterTable}_id";
                        }
                        else
                        {
                            Id = string.Format("(SELECT id FROM {0} WHERE {1}_id = @{1}_id {2} AND COALESCE({3}, {4}) = {4} LIMIT 1)",
                                _this.FormSchema.MasterTable,//0
                                _this.DataPusherConfig.SourceTable,//1
                                _pshId,//2
                                ebs[SystemColumns.eb_del],//3
                                ebs.GetBoolFalse(SystemColumns.eb_del));//4
                        }
                        query += Regex.Replace(_table.CustomSelectQuery, @":id|@id", Id);
                        _qryCount++;
                        continue;
                    }
                    _cols = $"{ebs[SystemColumns.eb_loc_id]}, id, {ebs[SystemColumns.eb_row_num]}";
                }

                IEnumerable<ColumnSchema> _columns = _table.Columns.Where(x => !x.Control.DoNotPersist || x.Control.IsSysControl || _table.TableType == WebFormTableTypes.Review);
                if (_columns.Count() > 0)
                {
                    _cols += ", " + String.Join(", ", _columns.Select(x => { return x.Control.IsSysControl ? ebs[x.ColumnName] : x.ColumnName; }));
                    IEnumerable<ColumnSchema> _ph_em_cols =
                        _columns.Where(x => (x.Control is EbPhone _phCtrl && _phCtrl.Sendotp) || (x.Control is EbEmailControl _emCtrl && _emCtrl.Sendotp));
                    if (_ph_em_cols.Count() > 0)
                        _cols += ", " + String.Join(", ", _ph_em_cols.Select(x => x.ColumnName + FormConstants._verified));
                }

                if (_this.DataPusherConfig == null)// master form
                {
                    query += string.Format("SELECT {0}{7} FROM {1} WHERE {2} = @{3}_id AND COALESCE({5}, {6}) = {6} {4};",
                        _cols,//0
                        _table.TableName,//1
                        _id,//2
                        _this.FormSchema.MasterTable,//3
                        _table.TableType == WebFormTableTypes.Grid ? ("ORDER BY " + ebs[SystemColumns.eb_row_num] + (_table.DescOdr ? " DESC" : string.Empty)) : "ORDER BY id",//4
                        _table.TableType == WebFormTableTypes.Review ? SystemColumns.eb_del : ebs[SystemColumns.eb_del],//5
                        _table.TableType == WebFormTableTypes.Review ? "'F'" : ebs.GetBoolFalse(SystemColumns.eb_del),//6
                        _table.TableType == WebFormTableTypes.Review ? EbReview.GetPrimaryRoleNameQuery() : string.Empty);//7
                }
                else
                {

                    if (_table.TableName == _this.FormSchema.MasterTable)
                    {
                        query += string.Format("SELECT {0} FROM {1} WHERE {2}_id = @{2}_id {3} AND COALESCE({4}, {5}) = {5};",
                            _cols,//0
                            _table.TableName,//1
                            _this.DataPusherConfig.SourceTable,//2
                            _pshId,//3
                            ebs[SystemColumns.eb_del],//4
                            ebs.GetBoolFalse(SystemColumns.eb_del));//5
                    }
                    else
                    {
                        query += string.Format("SELECT {0} FROM {1} WHERE {10} = (SELECT id FROM {2} WHERE {3}_id = @{3}_id {4} AND COALESCE({6}, {7}) = {7} LIMIT 1) AND COALESCE({8}, {9}) = {9} {5};",
                            _cols,//0
                            _table.TableName,//1
                            _this.FormSchema.MasterTable,//2
                            _this.DataPusherConfig.SourceTable,//3
                            _pshId,//4
                            _table.TableType == WebFormTableTypes.Grid ? ("ORDER BY " + ebs[SystemColumns.eb_row_num] + (_table.DescOdr ? " DESC" : string.Empty)) : "ORDER BY id",//5
                            ebs[SystemColumns.eb_del],//6
                            ebs.GetBoolFalse(SystemColumns.eb_del),//7
                            _table.TableType == WebFormTableTypes.Review ? SystemColumns.eb_del : ebs[SystemColumns.eb_del],//8
                            _table.TableType == WebFormTableTypes.Review ? "'F'" : ebs.GetBoolFalse(SystemColumns.eb_del),//9
                            _id);//10
                    }
                }
                _qryCount++;
            }
            foreach (EbControl Ctrl in _this.FormSchema.ExtendedControls)
            {
                if (Ctrl is EbProvisionUser)
                {
                    extquery += (Ctrl as EbProvisionUser).GetMappedUserQuery(_this.FormSchema.MasterTable, ebs[SystemColumns.eb_del], ebs.GetBoolFalse(SystemColumns.eb_del));
                    _qryCount++;
                }
                else if (Ctrl is IEbExtraQryCtrl)
                {
                    if (Ctrl is EbReview Rev && _this.DataPusherConfig != null)
                    {
                        extquery += Rev.GetSelectQuery(_this, _pshId, ebs[SystemColumns.eb_del], ebs.GetBoolFalse(SystemColumns.eb_del));
                        _qryCount++;
                    }
                    else if (_this.DataPusherConfig == null && !Ctrl.DoNotPersist)
                    {
                        extquery += (Ctrl as IEbExtraQryCtrl).GetSelectQuery(DataDB, _this.FormSchema.MasterTable, form_ver_id, _this.RefId);
                        _qryCount++;
                    }
                }
            }
            return query + extquery;
        }

        public static int GetSelectQueryCount(EbWebForm _this, bool includePsQryCount)
        {
            int _qryCount = 0;
            foreach (TableSchema _table in _this.FormSchema.Tables)
            {
                if (_table.DoNotPersist)
                    continue;

                if (_table.TableType == WebFormTableTypes.Grid)
                {
                    if (!string.IsNullOrWhiteSpace(_table.CustomSelectQuery))
                    {

                        _qryCount++;
                        continue;
                    }
                }
                _qryCount++;
            }
            foreach (EbControl Ctrl in _this.FormSchema.ExtendedControls)
            {
                if (Ctrl is EbProvisionUser)
                {
                    _qryCount++;
                }
                else if (Ctrl is IEbExtraQryCtrl)
                {
                    if (Ctrl is EbReview Rev && _this.DataPusherConfig != null)
                    {
                        _qryCount++;
                    }
                    else if (_this.DataPusherConfig == null && !Ctrl.DoNotPersist)
                    {
                        _qryCount++;
                    }
                }
            }

            if (includePsQryCount)
            {
                List<EbControl> drPsList = new List<EbControl>();

                foreach (TableSchema Tbl in _this.FormSchema.Tables)
                {
                    drPsList.AddRange(Tbl.Columns.FindAll(e => !e.Control.DoNotPersist && e.Control is IEbPowerSelect && !(e.Control as IEbPowerSelect).IsDataFromApi).Select(e => e.Control));
                }
                if (drPsList.Count > 0)
                {
                    _qryCount += drPsList.Count;
                }
            }
            return _qryCount;
        }

        //public static string GetDynamicGridSelectQuery(EbWebForm _this, IDatabase DataDB, Service _service, string _prntTbl, string[] _targetTbls, out string _queryPs, out int _qryCount)
        //{
        //    string query = string.Empty;
        //    _queryPs = string.Empty;
        //    _qryCount = 0;
        //    EbSystemColumns ebs = _this.SolutionObj.SolutionSettings.SystemColumns;

        //    for (int i = 0; i < _targetTbls.Length; i++)
        //    {
        //        TableSchema _table = _this.FormSchema.Tables.Find(e => e.TableName == _targetTbls[i] && e.IsDynamic && e.TableType == WebFormTableTypes.Grid);
        //        string _cols = string.Format("{0}, id, {1}",
        //            ebs[SystemColumns.eb_loc_id],
        //            ebs[SystemColumns.eb_row_num]);

        //        IEnumerable<ColumnSchema> _columns = _table.Columns.Where(x => !x.Control.DoNotPersist || x.Control.IsSysControl || _table.TableType == WebFormTableTypes.Review);
        //        if (_columns.Count() > 0)
        //            _cols += ", " + string.Join(", ", _columns.Select(x => { return x.Control.IsSysControl ? ebs[x.ColumnName] : x.ColumnName; }));

        //        query += string.Format("SELECT {0} FROM {1} WHERE {2}_id = @{2}_id AND {3}_id = @{3}_id AND COALESCE({4}, {6}) = {6} {5}; ",
        //            _cols,
        //            _table.TableName,
        //            _this.FormSchema.MasterTable,
        //            _prntTbl,
        //            ebs[SystemColumns.eb_del],
        //            "ORDER BY " + ebs[SystemColumns.eb_row_num] + (_table.DescOdr ? " DESC" : string.Empty),
        //            ebs.GetBoolFalse(SystemColumns.eb_del));

        //        _qryCount++;

        //        foreach (ColumnSchema Col in _table.Columns.FindAll(e => !e.Control.DoNotPersist && e.Control is EbDGPowerSelectColumn && !(e.Control as EbDGPowerSelectColumn).IsDataFromApi))
        //        {
        //            _queryPs += (Col.Control as EbDGPowerSelectColumn).GetSelectQuery123(DataDB, _service, _table.TableName, Col.ColumnName, _prntTbl, _this.FormSchema.MasterTable);
        //        }
        //    }
        //    return query;
        //}

        public static string GetSelectQuery_Batch(EbWebForm _this, out int _qryCount)
        {
            _qryCount = 0;
            string query = string.Empty;
            EbSystemColumns ebs = _this.SolutionObj.SolutionSettings.SystemColumns;
            EbDataPusherConfig conf = _this.DataPusherConfig;
            string _pshId = conf.MultiPushId == null ? string.Empty : $"AND {ebs[SystemColumns.eb_push_id]} = '{conf.MultiPushId}'";

            foreach (TableSchema _table in _this.FormSchema.Tables)
            {
                string _cols = string.Empty;
                IEnumerable<ColumnSchema> _columns = _table.Columns.Where(x => !x.Control.DoNotPersist || x.Control.IsSysControl || _table.TableType == WebFormTableTypes.Review);
                if (_columns.Count() > 0)
                {
                    _cols = ", " + String.Join(", ", _columns.Select(x => { return x.Control.IsSysControl ? ebs[x.ColumnName] : x.ColumnName; }));
                    IEnumerable<ColumnSchema> _em_ph_cols = _columns.Where(x => (x.Control is EbPhone _phCtrl && _phCtrl.Sendotp) || (x.Control is EbEmailControl _emCtrl && _emCtrl.Sendotp));
                    if (_em_ph_cols.Count() > 0)
                        _cols += ", " + String.Join(", ", _em_ph_cols.Select(x => x.ColumnName + FormConstants._verified));
                }

                if (_table.TableName == _this.FormSchema.MasterTable)
                {
                    //imp: column order is same as normal primary table 
                    query += $@"
SELECT
    {ebs[SystemColumns.eb_loc_id]},
    {ebs[SystemColumns.eb_ver_id]},
    {ebs[SystemColumns.eb_lock]},
    {ebs[SystemColumns.eb_push_id]},
    {ebs[SystemColumns.eb_src_id]},
    {ebs[SystemColumns.eb_created_by]},
    {ebs[SystemColumns.eb_void]},
    {ebs[SystemColumns.eb_created_at]},
    {ebs[SystemColumns.eb_src_ver_id]},
    {ebs[SystemColumns.eb_ro]},
    {ebs[SystemColumns.eb_lastmodified_by]},
    {ebs[SystemColumns.eb_lastmodified_at]},
    id 
    {_cols}
FROM
    {_table.TableName}
WHERE
    {conf.GridTableName}_id={{0}} AND
    COALESCE({ebs[SystemColumns.eb_del]}, {ebs.GetBoolFalse(SystemColumns.eb_del)}) = {ebs.GetBoolFalse(SystemColumns.eb_del)} {_pshId}; ";
                }
                else
                {
                    if (_table.TableType == WebFormTableTypes.Grid)
                    {
                        if (!string.IsNullOrWhiteSpace(_table.CustomSelectQuery))
                        {
                            string Id = string.Format("(SELECT id FROM {0} WHERE {1}_id={5} AND COALESCE({2}, {3})={3} {4})",
                                _this.FormSchema.MasterTable,//0
                                conf.GridTableName,//1
                                ebs[SystemColumns.eb_del],//2
                                ebs.GetBoolFalse(SystemColumns.eb_del),//3
                                _pshId,//4
                                "{0}");//5

                            query += Regex.Replace(_table.CustomSelectQuery, @":id|@id", Id);
                            _qryCount++;
                            continue;
                        }
                        _cols = $"{ebs[SystemColumns.eb_loc_id]}, id, {ebs[SystemColumns.eb_row_num]}" + _cols;
                    }
                    else
                        _cols = $"{ebs[SystemColumns.eb_loc_id]}, id" + _cols;

                    query += string.Format("SELECT {0} FROM {2} WHERE {1}_id = (SELECT id FROM {1} WHERE {3}_id={8} AND COALESCE({4}, {5})={5} {6}) AND COALESCE({4}, {5})={5} {7};",
                        _cols,//0
                        _this.FormSchema.MasterTable,//1
                        _table.TableName,//2
                        conf.GridTableName,//3
                        ebs[SystemColumns.eb_del],//4
                        ebs.GetBoolFalse(SystemColumns.eb_del),//5
                        _pshId,//6
                        _table.TableType == WebFormTableTypes.Grid ? ("ORDER BY " + ebs[SystemColumns.eb_row_num] + (_table.DescOdr ? " DESC" : "")) : "",//7
                        "{0}");//8
                }
                _qryCount++;
            }
            return query;
        }

        public static string GetDeleteQuery(EbWebForm _this, IDatabase DataDB)
        {
            string FullQry = string.Empty;
            EbSystemColumns ebs = _this.SolutionObj.SolutionSettings.SystemColumns;

            foreach (EbWebForm ebWebForm in _this.FormCollection)
            {
                WebFormSchema _schema = ebWebForm.FormSchema;
                foreach (TableSchema _table in _schema.Tables.FindAll(e => e.TableType != WebFormTableTypes.Review && !e.DoNotPersist))
                {
                    if (ebWebForm.FormDataBackup.MultipleTables.TryGetValue(_table.TableName, out SingleTable Table) && Table.Count > 0)
                    {
                        string Ids = Table.Select(e => e.RowId).Join(",");
                        string autoIdBckUp = string.Empty;
                        //if (ebWebForm.AutoId != null && ebWebForm.AutoId.TableName == _table.TableName) // uncomment this and check the autoid reassignment
                        //    autoIdBckUp = string.Format(", {0}_ebbkup = {0}, {0} = CONCAT('DEL_', {0}, '_ebbkup')", ebWebForm.AutoId.Name);

                        string Qry = string.Format("UPDATE {0} SET {1} = {7}, {2} = @eb_lastmodified_by, {3} = {4} {5} WHERE id IN ({6}) AND COALESCE({1}, {8}) = {8};",
                            _table.TableName,//0
                            ebs[SystemColumns.eb_del],//1
                            ebs[SystemColumns.eb_lastmodified_by],//2
                            ebs[SystemColumns.eb_lastmodified_at],//3
                            DataDB.EB_CURRENT_TIMESTAMP,//4
                            autoIdBckUp,//5
                            Ids,//6
                            ebs.GetBoolTrue(SystemColumns.eb_del),//7
                            ebs.GetBoolFalse(SystemColumns.eb_del));//8

                        FullQry = Qry + FullQry;//First Lines table data cancelled
                    }
                }
            }
            EbReview ebReview = (EbReview)_this.FormSchema.ExtendedControls.FirstOrDefault(e => e is EbReview);
            if (ebReview != null)
            {
                FullQry += $@"
UPDATE 
  eb_my_actions
SET
  hide='T'
WHERE
  COALESCE(is_completed, 'F') = 'F' AND 
  COALESCE(eb_del, 'F') = 'F' AND 
  COALESCE(hide, 'F') = 'F' AND
  id=(SELECT eb_my_actions_id FROM eb_approval APP WHERE 
    COALESCE(APP.eb_del, 'F') = 'F' AND status={(int)ReviewStatusEnum.In_Process} AND
	eb_src_id=@{_this.TableName}_id AND eb_ver_id=@{_this.TableName}_eb_ver_id LIMIT 1);";
            }
            return FullQry;
        }

        //public static string GetDeleteQuery(EbWebForm _this, IDatabase DataDB)
        //{
        //    string query = string.Empty;
        //    foreach (TableSchema _table in _this.FormSchema.Tables)
        //    {
        //        string _id = "id";
        //        string _dupcols = string.Empty;
        //        if (_table.TableName != _this.FormSchema.MasterTable)
        //            _id = _this.FormSchema.MasterTable + "_id";
        //        foreach (ColumnSchema _column in _table.Columns)
        //        {
        //            if (_column.Control is EbAutoId)
        //            {
        //                _dupcols += string.Format(", {0}_ebbkup = {0}, {0} = CONCAT({0}, '_ebbkup')", _column.ColumnName);
        //            }
        //        }
        //        query += string.Format("UPDATE {0} SET eb_del='T',eb_lastmodified_by = @eb_lastmodified_by, eb_lastmodified_at = " + DataDB.EB_CURRENT_TIMESTAMP + " {1} WHERE {2} = @id AND COALESCE(eb_del, 'F') = 'F';", _table.TableName, _dupcols, _id);
        //    }
        //    return query;
        //}

        public static string GetCancelQuery(EbWebForm _this, IDatabase DataDB, bool Cancel)
        {
            string FullQry = string.Empty;
            EbSystemColumns ebs = _this.SolutionObj.SolutionSettings.SystemColumns;

            foreach (EbWebForm ebWebForm in _this.FormCollection)
            {
                WebFormSchema _schema = ebWebForm.FormSchema;
                foreach (TableSchema _table in _schema.Tables.FindAll(e => e.TableType != WebFormTableTypes.Review && !e.DoNotPersist))
                {
                    if (_table.TableType == WebFormTableTypes.Grid && !string.IsNullOrWhiteSpace(_table.CustomSelectQuery) && !Cancel)
                        continue;

                    bool _cnlRsn = ebWebForm.CancelReason && _schema.MasterTable == _table.TableName;

                    if (ebWebForm.FormDataBackup.MultipleTables.TryGetValue(_table.TableName, out SingleTable Table) && Table.Count > 0)
                    {
                        string Ids = Table.Select(e => e.RowId).Join(",");

                        //USE ANY
                        string Qry = string.Format("UPDATE {0} SET {1} = {2}, {3} = @eb_lastmodified_by, {4} = {5} {11} WHERE id IN ({6}) AND COALESCE({7}, {8}) = {8} AND COALESCE({1}, {9}) = {10};",
                            _table.TableName,//0
                            ebs[SystemColumns.eb_void],//1
                            Cancel ? ebs.GetBoolTrue(SystemColumns.eb_void) : ebs.GetBoolFalse(SystemColumns.eb_void),//2
                            ebs[SystemColumns.eb_lastmodified_by],//3
                            ebs[SystemColumns.eb_lastmodified_at],//4
                            DataDB.EB_CURRENT_TIMESTAMP,//5
                            Ids,//6
                            ebs[SystemColumns.eb_del],//7
                            ebs.GetBoolFalse(SystemColumns.eb_del),//8
                            ebs.GetBoolFalse(SystemColumns.eb_void),//9
                            Cancel ? ebs.GetBoolFalse(SystemColumns.eb_void) : ebs.GetBoolTrue(SystemColumns.eb_void),//10
                            _cnlRsn ? $", {ebs[SystemColumns.eb_void_reason]} = @{FormConstants.eb_void_reason}" : string.Empty);//11

                        FullQry = Qry + FullQry;//First Lines table data cancelled
                    }
                }
            }
            EbReview ebReview = (EbReview)_this.FormSchema.ExtendedControls.FirstOrDefault(e => e is EbReview);
            if (ebReview != null)
            {
                FullQry += $@"
UPDATE 
  eb_my_actions
SET
  hide='{(Cancel ? "T" : "F")}'
WHERE
  COALESCE(is_completed, 'F') = 'F' AND 
  COALESCE(eb_del, 'F') = 'F' AND 
  COALESCE(hide, 'F') = '{(Cancel ? "F" : "T")}' AND
  id=(SELECT eb_my_actions_id FROM eb_approval APP WHERE 
    COALESCE(APP.eb_del, 'F') = 'F' AND status={(int)ReviewStatusEnum.In_Process} AND
	eb_src_id=@{_this.TableName}_id AND eb_ver_id=@{_this.TableName}_eb_ver_id LIMIT 1);";
            }

            return FullQry;
        }

        public static string GetChangeLocationQuery(EbWebForm _this, IDatabase DataDB, int UserId, int NewLocId)
        {
            string FullQry = string.Empty;
            EbSystemColumns ebs = _this.SolutionObj.SolutionSettings.SystemColumns;

            foreach (EbWebForm ebWebForm in _this.FormCollection)
            {
                WebFormSchema _schema = ebWebForm.FormSchema;
                foreach (TableSchema _table in _schema.Tables.FindAll(e => e.TableType != WebFormTableTypes.Review && !e.DoNotPersist))
                {
                    if (_table.TableType == WebFormTableTypes.Grid && !string.IsNullOrWhiteSpace(_table.CustomSelectQuery))
                        continue;

                    if (ebWebForm.FormDataBackup.MultipleTables.TryGetValue(_table.TableName, out SingleTable Table) && Table.Count > 0)
                    {
                        string Ids = Table.Select(e => e.RowId).Join(",");

                        //USE ANY
                        string Qry = string.Format("UPDATE {0} SET {1} = {2}, {3} = {9}, {4} = {5} WHERE id IN ({6}) AND COALESCE({7}, {8}) = {8};",
                            _table.TableName,//0
                            ebs[SystemColumns.eb_loc_id],//1
                            NewLocId,//2
                            ebs[SystemColumns.eb_lastmodified_by],//3
                            ebs[SystemColumns.eb_lastmodified_at],//4
                            DataDB.EB_CURRENT_TIMESTAMP,//5
                            Ids,//6
                            ebs[SystemColumns.eb_del],//7
                            ebs.GetBoolFalse(SystemColumns.eb_del),//8
                            UserId);//9

                        FullQry = Qry + FullQry;//Lines table data is processed first
                    }
                }
            }
            return FullQry;
        }

        //public static string GetCancelQuery(EbWebForm _this, IDatabase DataDB)
        //{
        //    string query = string.Empty;
        //    foreach (TableSchema _table in _this.FormSchema.Tables)
        //    {
        //        string _id = "id";
        //        if (_table.TableName != _this.FormSchema.MasterTable)
        //            _id = _this.FormSchema.MasterTable + "_id";
        //        query += string.Format("UPDATE {0} SET eb_void='T',eb_lastmodified_by = @eb_lastmodified_by, eb_lastmodified_at = " + DataDB.EB_CURRENT_TIMESTAMP + " WHERE {1} = @id AND COALESCE(eb_void, 'F') = 'F' AND COALESCE(eb_del, 'F') = 'F';", _table.TableName, _id);
        //    }
        //    return query;
        //}

        public static string GetLockOrUnlockQuery(EbWebForm _this, IDatabase DataDB, bool Lock)
        {
            EbSystemColumns ebs = _this.SolutionObj.SolutionSettings.SystemColumns;

            string Qry = string.Format("UPDATE {0} SET {1} = {6}, {2} = @eb_lastmodified_by, {3} = {4} WHERE id = @{0}_id AND COALESCE({1}, {5}) = {7};",
                _this.TableName,
                ebs[SystemColumns.eb_lock],//1
                ebs[SystemColumns.eb_lastmodified_by],//2
                ebs[SystemColumns.eb_lastmodified_at],//3
                DataDB.EB_CURRENT_TIMESTAMP,//4
                ebs.GetBoolFalse(SystemColumns.eb_lock),//5
                Lock ? ebs.GetBoolTrue(SystemColumns.eb_lock) : ebs.GetBoolFalse(SystemColumns.eb_lock),//6
                Lock ? ebs.GetBoolFalse(SystemColumns.eb_lock) : ebs.GetBoolTrue(SystemColumns.eb_lock));//7

            return Qry;
        }

        public static string GetInsertQuery(EbWebForm _this, IDatabase DataDB, string tblName, bool isIns, bool bFirstRow)
        {
            string _qry = string.Empty;
            EbSystemColumns ebs = _this.SolutionObj.SolutionSettings.SystemColumns;
            EbDataPusherConfig conf = _this.DataPusherConfig;
            string currencyCols = string.Empty, currencyVals = string.Empty;
            TableSchema _table = _this.FormSchema.Tables.Find(e => e.TableName == tblName);
            if (_table != null)
            {
                if (_table.Columns.Exists(e => !e.Control.DoNotPersist && !e.Control.IsSysControl &&
                ((e.Control is EbNumeric numCtrl && numCtrl.InputMode == NumInpMode.Currency) ||
                (e.Control is EbDGNumericColumn numCol && numCol.InputMode == NumInpMode.Currency))))
                {
                    currencyCols = $", {ebs[SystemColumns.eb_currency_id]}, {ebs[SystemColumns.eb_currency_xid]}, {ebs[SystemColumns.eb_xrate1]}, {ebs[SystemColumns.eb_xrate2]}";
                    currencyVals = $", 1, 'AED', 1, 1";
                }
            }

            if (tblName.Equals(_this.TableName))
            {
                if (conf == null)
                {
                    //if (_this.AutoId != null)
                    //    _qry = $"LOCK TABLE ONLY {_this.AutoId.TableName} IN EXCLUSIVE MODE; ";

                    _qry += string.Format("INSERT INTO {0} ({20} {1}, {2}, {3}, {4}, {5}, {10}, {11}, {12}, {13}, {19}{8}) VALUES ({21} @eb_createdby, {6}, @{18}, @{7}_eb_ver_id, @eb_signin_log_id, {14}, {15}, {16}, {17}, @{7}_eb_ver_id{9}); ",
                        tblName,//0
                        ebs[SystemColumns.eb_created_by],//1
                        ebs[SystemColumns.eb_created_at],//2
                        ebs[SystemColumns.eb_loc_id],//3
                        ebs[SystemColumns.eb_ver_id],//4
                        ebs[SystemColumns.eb_signin_log_id],//5
                        DataDB.EB_CURRENT_TIMESTAMP,//6
                        _this.TableName,//7
                        currencyCols,//8
                        currencyVals,//9
                        ebs[SystemColumns.eb_void],//10
                        ebs[SystemColumns.eb_del],//11
                        ebs[SystemColumns.eb_lock],//12
                        ebs[SystemColumns.eb_ro],//13
                        ebs.GetBoolFalse(SystemColumns.eb_void),//14
                        ebs.GetBoolFalse(SystemColumns.eb_del),//15
                        _this.LockOnSave ? ebs.GetBoolTrue(SystemColumns.eb_lock) : ebs.GetBoolFalse(SystemColumns.eb_lock),//16
                        ebs.GetBoolFalse(SystemColumns.eb_ro),//17
                        FormConstants.eb_loc_id_ + _this.CrudContext,//18
                        ebs[SystemColumns.eb_src_ver_id],//19
                        "{0}",//20
                        "{1}");//21
                    _qry += $"UPDATE {tblName} SET {ebs[SystemColumns.eb_src_id]}=id WHERE id=(SELECT eb_currval('{tblName}_id_seq'));";
                }
                else
                {
                    bool refCtrlExists = false;// source-primary-table_id is present as a control in destination form
                    if (_table != null && _table.Columns.Exists(e => e.Control.Name == conf.SourceTable + "_id"))
                        refCtrlExists = true;

                    string srcRef = conf.SourceRecId <= 0 ? $"(SELECT eb_currval('{conf.SourceTable}_id_seq'))" : $"@{conf.SourceTable}_id";

                    _qry = string.Format(@"INSERT INTO {0} ({27} {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {15}, {16}, {20}, {21}{18}{24}) " +
                                    "VALUES ({28} @eb_createdby, {10}, @{26}, @{11}_eb_ver_id, {12}, {13}, {14}, @eb_signin_log_id, @{9}_eb_ver_id, {17}, {22}, {23}{19}{25}); ",
                        tblName,//0
                        ebs[SystemColumns.eb_created_by],//1
                        ebs[SystemColumns.eb_created_at],//2
                        ebs[SystemColumns.eb_loc_id],//3
                        ebs[SystemColumns.eb_ver_id],//4
                        ebs[SystemColumns.eb_src_id],//5
                        ebs[SystemColumns.eb_push_id],//6
                        ebs[SystemColumns.eb_lock],//7
                        ebs[SystemColumns.eb_signin_log_id],//8
                        conf.SourceTable,//9
                        DataDB.EB_CURRENT_TIMESTAMP,//10
                        _this.TableName,//11
                        srcRef,//12
                        conf.MultiPushId == null ? "null" : $"'{conf.MultiPushId}'",//13
                        conf.DisableAutoLock ? ebs.GetBoolFalse(SystemColumns.eb_lock) : ebs.GetBoolTrue(SystemColumns.eb_lock),//14
                        ebs[SystemColumns.eb_src_ver_id],//15
                        ebs[SystemColumns.eb_ro],//16
                        conf.DisableAutoReadOnly ? ebs.GetBoolFalse(SystemColumns.eb_ro) : ebs.GetBoolTrue(SystemColumns.eb_ro),//17
                        currencyCols,//18
                        currencyVals,//19
                        ebs[SystemColumns.eb_void],//20
                        ebs[SystemColumns.eb_del],//21
                        ebs.GetBoolFalse(SystemColumns.eb_void),//22
                        ebs.GetBoolFalse(SystemColumns.eb_del),//23
                        refCtrlExists ? string.Empty : $", {conf.SourceTable}_id",//24
                        refCtrlExists ? string.Empty : $", {srcRef}",//25
                        FormConstants.eb_loc_id_ + _this.CrudContext,//26
                        "{0}",//27
                        "{1}");//28
                }

                if (DataDB.Vendor == DatabaseVendors.MYSQL)
                    _qry += $"SELECT eb_persist_currval('{tblName}_id_seq'); ";
                //if (_this.IsLocEditable)
                //    _qry = _qry.Replace($", {ebs[SystemColumns.eb_loc_id]}", string.Empty).Replace(", @eb_loc_id", string.Empty);
            }
            //else if (tblName.Equals("eb_approval_lines"))
            //{
            //    _qry = $@"INSERT INTO eb_approval_lines ({{0}} eb_created_by, eb_created_at, eb_loc_id, eb_src_id, eb_ver_id, eb_signin_log_id) 
            //                VALUES ({{1}} @eb_createdby, {DataDB.EB_CURRENT_TIMESTAMP}, @eb_loc_id, @{_this.TableName}_id, @{_this.TableName}_eb_ver_id, @eb_signin_log_id); ";
            //    if (DataDB.Vendor == DatabaseVendors.MYSQL)
            //        _qry += "SELECT eb_persist_currval('eb_approval_lines_id_seq'); ";
            //    // eb_approval - update eb_approval_lines_id
            //    _qry += $@"UPDATE eb_approval SET eb_approval_lines_id = (SELECT eb_currval('eb_approval_lines_id_seq')), eb_lastmodified_by = @eb_modified_by, eb_lastmodified_at = {DataDB.EB_CURRENT_TIMESTAMP} 
            //                   WHERE eb_src_id = @{_this.TableName}_id AND eb_ver_id =  @{_this.TableName}_eb_ver_id AND COALESCE(eb_del, 'F') = 'F'; ";
            //}
            else
            {
                string srcRef = isIns ? $"(SELECT eb_currval('{_this.TableName}_id_seq'))" : $"@{_this.TableName}_id" + (conf == null ? string.Empty : _this.CrudContext);
                if (bFirstRow)
                {
                    _qry = $@"
INSERT INTO {tblName} (
{{0}} 
{ebs[SystemColumns.eb_created_by]}, 
{ebs[SystemColumns.eb_created_at]}, 
{ebs[SystemColumns.eb_loc_id]}, 
{_this.TableName}_id, 
{ebs[SystemColumns.eb_signin_log_id]}, 
{ebs[SystemColumns.eb_void]}, 
{ebs[SystemColumns.eb_del]} 
{currencyCols} 
) 
VALUES ".Replace("\r", "").Replace("\n", "");
                }
                _qry += $@"
{(bFirstRow ? "" : ", ")} 
( 
{{1}} 
@eb_createdby, 
{DataDB.EB_CURRENT_TIMESTAMP}, 
@{FormConstants.eb_loc_id_ + _this.CrudContext}, 
{srcRef}, 
@eb_signin_log_id, 
{ebs.GetBoolFalse(SystemColumns.eb_void)}, 
{ebs.GetBoolFalse(SystemColumns.eb_del)} 
{currencyVals} 
)".Replace("\r", "").Replace("\n", "");
                if (isIns && DataDB.Vendor == DatabaseVendors.MYSQL)
                    _qry += $"SELECT eb_persist_currval('{tblName}_id_seq'); ";
            }

            return _qry;
        }

        public static string GetUpdateQuery(EbWebForm _this, IDatabase DataDB, string tblName, bool isDel, bool DGCustSelect)
        {
            string _qry;
            EbSystemColumns ebs = _this.SolutionObj.SolutionSettings.SystemColumns;
            EbDataPusherConfig conf = _this.DataPusherConfig;

            if (conf != null && DGCustSelect && !isDel)//don't edit datapusher line item with custom select query
                return string.Empty;
            string LocUpdateQry = string.Empty;
            if (_this.IsLocEditable)
                LocUpdateQry = $"{ebs[SystemColumns.eb_loc_id]}=@{FormConstants.eb_loc_id_ + _this.CrudContext}, ";

            if (conf == null)
            {
                if (tblName.Equals(_this.TableName))
                    _qry = string.Format("UPDATE {0} SET {8} {6}{7}{1} = @eb_modified_by, {2} = {3} WHERE id = {9} AND COALESCE({4}, {5}) = {5}; ",
                        tblName,//0
                        ebs[SystemColumns.eb_lastmodified_by],//1
                        ebs[SystemColumns.eb_lastmodified_at],//2
                        DataDB.EB_CURRENT_TIMESTAMP,//3
                        ebs[SystemColumns.eb_del],//4
                        ebs.GetBoolFalse(SystemColumns.eb_del),//5
                        _this.LockOnSave ? $"{ebs[SystemColumns.eb_lock]} = {ebs.GetBoolTrue(SystemColumns.eb_lock)}, " : string.Empty,//6
                        LocUpdateQry,//7
                        "{0}",
                        "{1}");
                else
                    _qry = string.Format("UPDATE {0} SET {7} {1} = @eb_modified_by, {2} = {3} WHERE id = {8} AND {4} AND COALESCE({5}, {6}) = {6}; ",
                        tblName,//0
                        ebs[SystemColumns.eb_lastmodified_by],//1
                        ebs[SystemColumns.eb_lastmodified_at],//2
                        DataDB.EB_CURRENT_TIMESTAMP,//3
                        DGCustSelect ? "true" : $"{_this.TableName}_id = @{_this.TableName}_id",//4
                        ebs[SystemColumns.eb_del],//5
                        ebs.GetBoolFalse(SystemColumns.eb_del),//6
                        isDel ? $"{ebs[SystemColumns.eb_del]} = {ebs.GetBoolTrue(SystemColumns.eb_del)}, " : ("{0}" + LocUpdateQry),//7
                        "{1}");
            }
            else
            {
                // if isDel is true then consider lines table also
                string parentTbl = _this.TableName;
                string pushIdChk = string.Empty;
                string cxt = _this.CrudContext;
                if (tblName.Equals(_this.TableName))
                {
                    cxt = string.Empty;
                    parentTbl = conf.SourceTable;
                    pushIdChk = conf.MultiPushId == null ? string.Empty : $"AND {ebs[SystemColumns.eb_push_id]} = '{conf.MultiPushId}'";
                }
                _qry = string.Format("UPDATE {0} SET {8} {1} = @eb_modified_by, {2} = {3} WHERE id = {9} AND {4} AND COALESCE({5}, {6}) = {6} {7}; ",
                    tblName,//0
                    ebs[SystemColumns.eb_lastmodified_by],//1
                    ebs[SystemColumns.eb_lastmodified_at],//2
                    DataDB.EB_CURRENT_TIMESTAMP,//3
                    DGCustSelect ? "true" : $"{parentTbl}_id = @{parentTbl}_id{cxt}",//4
                    ebs[SystemColumns.eb_del],//5
                    ebs.GetBoolFalse(SystemColumns.eb_del),//6
                    pushIdChk,//7
                    isDel ? $"{ebs[SystemColumns.eb_del]} = {ebs.GetBoolTrue(SystemColumns.eb_del)}, " : ("{0}" + LocUpdateQry),//8
                    "{1}");
            }
            return _qry;
        }

        public static string GetInsertQuery_Batch(EbWebForm _this, IDatabase DataDB, string tblName, TableSchema _table, bool bFirstRow)
        {
            EbSystemColumns ebs = _this.SolutionObj.SolutionSettings.SystemColumns;
            EbDataPusherConfig conf = _this.DataPusherConfig;
            string _qry = string.Empty;

            if (tblName.Equals(_this.TableName))
            {
                bool refCtrlExists = false;// source-primary-table_id is present as a control in destination form
                if (_table != null && _table.Columns.Exists(e => e.Control.Name == conf.SourceTable + "_id"))
                    refCtrlExists = true;

                _qry = $@"
INSERT INTO {tblName} 
({{0}} 
{ebs[SystemColumns.eb_created_by]}, 
{ebs[SystemColumns.eb_created_at]}, 
{ebs[SystemColumns.eb_loc_id]}, 
{ebs[SystemColumns.eb_ver_id]}, 
{ebs[SystemColumns.eb_src_ver_id]}, 
{ebs[SystemColumns.eb_src_id]}, 
{ebs[SystemColumns.eb_push_id]}, 
{ebs[SystemColumns.eb_lock]}, 
{ebs[SystemColumns.eb_signin_log_id]}, 
{ebs[SystemColumns.eb_ro]}, 
{ebs[SystemColumns.eb_void]}, 
{ebs[SystemColumns.eb_del]}, 
{(refCtrlExists ? string.Empty : (conf.SourceTable + "_id,"))} 
{conf.GridTableName}_id) 
VALUES 
({{1}} 
@eb_createdby, 
{DataDB.EB_CURRENT_TIMESTAMP}, 
@{FormConstants.eb_loc_id_ + _this.CrudContext}, 
@{_this.TableName}_eb_ver_id, 
@{conf.SourceTable}_eb_ver_id, 
{conf.SourceRecId}, 
{(conf.MultiPushId == null ? "null" : $"'{conf.MultiPushId}'")}, 
{(conf.DisableAutoLock ? ebs.GetBoolFalse(SystemColumns.eb_lock) : ebs.GetBoolTrue(SystemColumns.eb_lock))}, 
@eb_signin_log_id, 
{(conf.DisableAutoReadOnly ? ebs.GetBoolFalse(SystemColumns.eb_ro) : ebs.GetBoolTrue(SystemColumns.eb_ro))}, 
{ebs.GetBoolFalse(SystemColumns.eb_void)}, 
{ebs.GetBoolFalse(SystemColumns.eb_del)}, 
{(refCtrlExists ? string.Empty : (conf.SourceRecId + ","))} 
{conf.GridDataId}); ".Replace("\r", "").Replace("\n", "");

                //if (_this.IsLocEditable)
                //    _qry = _qry.Replace("@eb_loc_id,", string.Empty).Replace($"{ebs[SystemColumns.eb_loc_id]},", string.Empty);

                if (!conf.DisableReverseLink)
                    _qry += $"UPDATE {conf.GridTableName} SET {tblName}_id=(SELECT eb_currval('{tblName}_id_seq')) WHERE id={conf.GridDataId}; ";
            }
            else
            {
                string srcRef = _this.TableRowId > 0 ? $"{_this.TableRowId}" : $"(SELECT eb_currval('{_this.TableName}_id_seq'))";
                if (bFirstRow)
                {
                    _qry = $@"
INSERT INTO {tblName} 
({{0}} 
{ebs[SystemColumns.eb_created_by]}, 
{ebs[SystemColumns.eb_created_at]}, 
{ebs[SystemColumns.eb_loc_id]}, 
{_this.TableName}_id, 
{ebs[SystemColumns.eb_signin_log_id]}, 
{ebs[SystemColumns.eb_void]}, 
{ebs[SystemColumns.eb_del]}) 
VALUES ".Replace("\r", "").Replace("\n", "");
                }

                _qry += $@"
{(bFirstRow ? "" : ", ")}
({{1}} 
@eb_createdby, 
{DataDB.EB_CURRENT_TIMESTAMP}, 
@{FormConstants.eb_loc_id_ + _this.CrudContext}, 
{srcRef}, 
@eb_signin_log_id, 
{ebs.GetBoolFalse(SystemColumns.eb_void)}, 
{ebs.GetBoolFalse(SystemColumns.eb_del)}) ".Replace("\r", "").Replace("\n", "");

            }

            return _qry;
        }

        public static string GetUpdateQuery_Batch(EbWebForm _this, IDatabase DataDB, string tblName, bool isDel, int id, bool DGCustSelect)
        {
            string _qry, parentTblChk, pushIdChk;
            EbSystemColumns ebs = _this.SolutionObj.SolutionSettings.SystemColumns;
            EbDataPusherConfig conf = _this.DataPusherConfig;

            if (conf != null && DGCustSelect && !isDel)//don't edit datapusher line item with custom select query
                return string.Empty;
            string LocUpdateQry = string.Empty;
            if (_this.IsLocEditable)
                LocUpdateQry = $"{ebs[SystemColumns.eb_loc_id]}=@{FormConstants.eb_loc_id_ + _this.CrudContext}, ";

            if (tblName.Equals(_this.TableName))
            {
                parentTblChk = $"{conf.GridTableName}_id = {conf.GridDataId} AND id = {id} AND";
                pushIdChk = conf.MultiPushId == null ? string.Empty : $"AND {ebs[SystemColumns.eb_push_id]} = '{conf.MultiPushId}'";
            }
            else
            {
                parentTblChk = (DGCustSelect ? string.Empty : $"{_this.TableName}_id = {_this.TableRowId} AND ") + $"id = {id} AND";
                pushIdChk = string.Empty;
            }

            _qry = $@"
UPDATE 
    {tblName} 
SET 
    {(isDel ? $"{ebs[SystemColumns.eb_del]} = {ebs.GetBoolTrue(SystemColumns.eb_del)}, " : ("{0}" + LocUpdateQry))} 
    {ebs[SystemColumns.eb_lastmodified_by]} = @eb_modified_by, 
    {ebs[SystemColumns.eb_lastmodified_at]} = {DataDB.EB_CURRENT_TIMESTAMP} 
WHERE 
    {parentTblChk}
    COALESCE({ebs[SystemColumns.eb_del]}, {ebs.GetBoolFalse(SystemColumns.eb_del)}) = {ebs.GetBoolFalse(SystemColumns.eb_del)} 
    {pushIdChk}; ".Replace("\r", "").Replace("\n", "");

            return _qry;
        }

    }
}
