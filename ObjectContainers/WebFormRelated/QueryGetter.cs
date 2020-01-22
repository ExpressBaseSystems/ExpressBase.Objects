using ExpressBase.Common;
using ServiceStack;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ExpressBase.Objects.WebFormRelated
{
    static class QueryGetter
    {
        public static string GetSelectQuery(EbWebForm _this, IDatabase DataDB, Service _service, out string _queryPs, out int _qryCount)
        {
            string query = string.Empty;
            string extquery = string.Empty;
            _queryPs = string.Empty;
            _qryCount = 0;
            foreach (TableSchema _table in _this.FormSchema.Tables)
            {
                if (_table.IsDynamic)
                    continue;
                string _cols = "eb_loc_id, id";
                string _id = "id";

                if (_table.TableName == _this.FormSchema.MasterTable)
                    _cols = "eb_loc_id, eb_ver_id, eb_lock, eb_push_id, eb_src_id, id";
                else
                    _id = _this.FormSchema.MasterTable + "_id";

                if (_table.TableType == WebFormTableTypes.Grid)
                    _cols = "eb_loc_id, id, eb_row_num";

                IEnumerable<ColumnSchema> _columns = _table.Columns.Where(x => (!x.Control.DoNotPersist || x.Control.IsSysControl));
                if (_columns.Count() > 0)
                    _cols += ", " + String.Join(", ", _columns.Select(x => x.ColumnName));

                if (_this.DataPusherConfig == null)
                {
                    query += string.Format("SELECT {0} FROM {1} WHERE {2} = @{3}_id AND COALESCE(eb_del, 'F') = 'F' {4};",
                        _cols, _table.TableName, _id, _this.FormSchema.MasterTable, _table.TableType == WebFormTableTypes.Grid ? (_table.DescOdr ? "ORDER BY eb_row_num DESC" : "ORDER BY eb_row_num") : "ORDER BY id");
                }
                else
                {
                    if (_table.TableName == _this.FormSchema.MasterTable)
                        query += string.Format("SELECT {0} FROM {1} WHERE {2}_id = @{2}_id AND eb_push_id = '{3}' AND COALESCE(eb_del, 'F') = 'F';",
                            _cols, _table.TableName, _this.DataPusherConfig.SourceTable, _this.DataPusherConfig.MultiPushId);
                    else
                        query += string.Format("SELECT {0} FROM {1} WHERE {2}_id = (SELECT id FROM {2} WHERE {3}_id = @{3}_id AND eb_push_id = '{4}' AND COALESCE(eb_del, 'F') = 'F' LIMIT 1) AND COALESCE(eb_del, 'F') = 'F' {5};",
                            _cols, _table.TableName, _this.FormSchema.MasterTable, _this.DataPusherConfig.SourceTable, _this.DataPusherConfig.MultiPushId, _table.TableType == WebFormTableTypes.Grid ? "ORDER BY eb_row_num" : "ORDER BY id");
                }
                _qryCount++;
                foreach (ColumnSchema Col in _table.Columns)
                {
                    if (Col.Control.DoNotPersist)
                        continue;
                    if (Col.Control is EbPowerSelect)
                        _queryPs += (Col.Control as EbPowerSelect).GetSelectQuery(DataDB, _service, Col.ColumnName, _table.TableName, _id, _this.FormSchema.MasterTable);
                    else if (Col.Control is EbDGPowerSelectColumn)
                        _queryPs += (Col.Control as EbDGPowerSelectColumn).GetSelectQuery(DataDB, _service, Col.ColumnName, _table.TableName, _id, _this.FormSchema.MasterTable);
                }
            }
            bool MuCtrlFound = false;
            foreach (Object Ctrl in _this.FormSchema.ExtendedControls)
            {
                if (Ctrl is EbProvisionUser && !MuCtrlFound)
                {
                    extquery += (Ctrl as EbProvisionUser).GetSelectQuery(_this.FormSchema.MasterTable);
                    MuCtrlFound = true;
                    _qryCount++;
                }
                else if (Ctrl is EbProvisionLocation)
                {
                    extquery += (Ctrl as EbProvisionLocation).GetSelectQuery(_this.FormSchema.MasterTable);
                    _qryCount++;
                }
            }
            return query + extquery;
        }

        public static string GetDynamicGridSelectQuery(EbWebForm _this, IDatabase DataDB, Service _service, string _prntTbl, string[] _targetTbls, out string _queryPs, out int _qryCount)
        {
            string query = string.Empty;
            _queryPs = string.Empty;
            _qryCount = 0;

            for (int i = 0; i < _targetTbls.Length; i++)
            {
                TableSchema _table = _this.FormSchema.Tables.Find(e => e.TableName == _targetTbls[i] && e.IsDynamic && e.TableType == WebFormTableTypes.Grid);
                string _cols = "eb_loc_id, id, eb_row_num";
                IEnumerable<ColumnSchema> _columns = _table.Columns.Where(x => (!x.Control.DoNotPersist || x.Control.IsSysControl));
                if (_columns.Count() > 0)
                    _cols += ", " + String.Join(", ", _columns.Select(x => x.ColumnName));

                query += $@"SELECT {_cols} FROM {_table.TableName} WHERE {_this.FormSchema.MasterTable}_id = @{_this.FormSchema.MasterTable}_id AND
                             {_prntTbl}_id = @{_prntTbl}_id AND COALESCE(eb_del, 'F') = 'F' {(_table.DescOdr ? "ORDER BY eb_row_num DESC" : "ORDER BY eb_row_num")}; ";

                _qryCount++;

                //foreach (ColumnSchema Col in _table.Columns)
                //{
                //    if (Col.Control.DoNotPersist)
                //        continue;
                //    if (Col.Control is EbPowerSelect)
                //        _queryPs += (Col.Control as EbPowerSelect).GetSelectQuery(DataDB, _service, Col.ColumnName, _table.TableName, _id, _this.FormSchema.MasterTable);
                //    else if (Col.Control is EbDGPowerSelectColumn)
                //        _queryPs += (Col.Control as EbDGPowerSelectColumn).GetSelectQuery(DataDB, _service, Col.ColumnName, _table.TableName, _id, _this.FormSchema.MasterTable);
                //}
            }
            return query;
        }

        public static string GetDeleteQuery(EbWebForm _this, IDatabase DataDB)
        {
            string query = string.Empty;
            foreach (TableSchema _table in _this.FormSchema.Tables)
            {
                string _id = "id";
                string _dupcols = string.Empty;
                if (_table.TableName != _this.FormSchema.MasterTable)
                    _id = _this.FormSchema.MasterTable + "_id";
                foreach (ColumnSchema _column in _table.Columns)
                {
                    if (_column.Control is EbAutoId)
                    {
                        _dupcols += string.Format(", {0}_ebbkup = {0}, {0} = CONCAT({0}, '_ebbkup')", _column.ColumnName);
                    }
                }
                query += string.Format("UPDATE {0} SET eb_del='T',eb_lastmodified_by = @eb_lastmodified_by, eb_lastmodified_at = " + DataDB.EB_CURRENT_TIMESTAMP + " {1} WHERE {2} = @id AND COALESCE(eb_del, 'F') = 'F';", _table.TableName, _dupcols, _id);
            }
            return query;
        }

        public static string GetCancelQuery(EbWebForm _this, IDatabase DataDB)
        {
            string query = string.Empty;
            foreach (TableSchema _table in _this.FormSchema.Tables)
            {
                string _id = "id";
                if (_table.TableName != _this.FormSchema.MasterTable)
                    _id = _this.FormSchema.MasterTable + "_id";
                query += string.Format("UPDATE {0} SET eb_void='T',eb_lastmodified_by = @eb_lastmodified_by, eb_lastmodified_at = " + DataDB.EB_CURRENT_TIMESTAMP + " WHERE {1} = @id AND COALESCE(eb_void, 'F') = 'F' AND COALESCE(eb_del, 'F') = 'F';", _table.TableName, _id);
            }
            return query;
        }

        public static string GetInsertQuery(EbWebForm _this, IDatabase DataDB, string tblName, bool isIns)
        {
            string _qry;

            if (tblName.Equals(_this.TableName))
            {
                if (_this.DataPusherConfig == null)
                {
                    _qry = $@"INSERT INTO {tblName} ({{0}} eb_created_by, eb_created_at, eb_loc_id, eb_ver_id) 
                                VALUES ({{1}} @eb_createdby, {DataDB.EB_CURRENT_TIMESTAMP}, @eb_loc_id, @{_this.TableName}_eb_ver_id); ";
                }
                else
                {
                    if (_this.DataPusherConfig.SourceRecId <= 0)
                        _qry = $@"INSERT INTO {tblName} ({{0}} eb_created_by, eb_created_at, eb_loc_id, eb_ver_id, {_this.DataPusherConfig.SourceTable}_id, eb_push_id, eb_lock) 
                                    VALUES ({{1}} @eb_createdby, {DataDB.EB_CURRENT_TIMESTAMP}, @eb_loc_id, @{_this.TableName}_eb_ver_id, (SELECT eb_currval('{_this.DataPusherConfig.SourceTable}_id_seq')), '{_this.DataPusherConfig.MultiPushId}', 'T'); ";
                    else
                        _qry = $@"INSERT INTO {tblName} ({{0}} eb_created_by, eb_created_at, eb_loc_id, eb_ver_id, {_this.DataPusherConfig.SourceTable}_id, eb_push_id, eb_lock) 
                                    VALUES ({{1}} @eb_createdby, {DataDB.EB_CURRENT_TIMESTAMP}, @eb_loc_id, @{_this.TableName}_eb_ver_id, @{_this.DataPusherConfig.SourceTable}_id, '{_this.DataPusherConfig.MultiPushId}', 'T'); ";
                }

                if (DataDB.Vendor == DatabaseVendors.MYSQL)
                    _qry += $"SELECT eb_persist_currval('{tblName}_id_seq'); ";
                if (_this.IsLocEditable)
                    _qry = _qry.Replace(", eb_loc_id", string.Empty).Replace(", @eb_loc_id", string.Empty);
            }
            else if (isIns)
            {
                _qry = $@"INSERT INTO {tblName} ({{0}} eb_created_by, eb_created_at, eb_loc_id, {_this.TableName}_id) 
                            VALUES ({{1}} @eb_createdby, {DataDB.EB_CURRENT_TIMESTAMP}, @eb_loc_id , (SELECT eb_currval('{_this.TableName}_id_seq'))); ";
                if (DataDB.Vendor == DatabaseVendors.MYSQL)
                    _qry += $"SELECT eb_persist_currval('{tblName}_id_seq'); ";
            }
            else
                _qry = $@"INSERT INTO {tblName} ({{0}} eb_created_by, eb_created_at, eb_loc_id, {_this.TableName}_id) 
                            VALUES ({{1}} @eb_createdby, {DataDB.EB_CURRENT_TIMESTAMP}, @eb_loc_id , @{_this.TableName}_id); ";

            return _qry;
        }

        public static string GetUpdateQuery(EbWebForm _this, IDatabase DataDB, string tblName, bool isDel)
        {
            string _qry;
            if (_this.DataPusherConfig == null)
            {
                if (tblName.Equals(_this.TableName))
                    _qry = $"UPDATE {tblName} SET {{0}} eb_lastmodified_by = @eb_modified_by, eb_lastmodified_at = {DataDB.EB_CURRENT_TIMESTAMP} WHERE id = {{1}} AND COALESCE(eb_del, 'F') = 'F'; ";
                else
                    _qry = $"UPDATE {tblName} SET {(isDel ? "eb_del = 'T', " : "{0}")} eb_lastmodified_by = @eb_modified_by, eb_lastmodified_at = {DataDB.EB_CURRENT_TIMESTAMP} WHERE id = {{1}} AND {_this.TableName}_id = @{_this.TableName}_id AND COALESCE(eb_del, 'F') = 'F'; ";
            }
            else
            {
                string parentTbl = _this.TableName;
                string pushIdChk = string.Empty;
                if (tblName.Equals(_this.TableName))
                {
                    parentTbl = _this.DataPusherConfig.SourceTable;
                    pushIdChk = $"AND eb_push_id = '{_this.DataPusherConfig.MultiPushId}'";
                }
                _qry = $@"UPDATE {tblName} SET {(isDel ? "eb_del = 'T', " : "{0}")} eb_lastmodified_by = @eb_modified_by, eb_lastmodified_at = {DataDB.EB_CURRENT_TIMESTAMP} 
                            WHERE id = {{1}} AND {parentTbl}_id = @{parentTbl}_id AND COALESCE(eb_del, 'F') = 'F' {pushIdChk}; ";
            }
            return _qry;
        }

    }
}
