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
                        _cols, _table.TableName, _id, _this.FormSchema.MasterTable, _table.TableType == WebFormTableTypes.Grid ? "ORDER BY eb_row_num" : "ORDER BY id");
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
            if (_this.DataPusherConfig == null)
            {
                if (tblName.Equals(_this.TableName))
                {
                    _qry = string.Format("INSERT INTO {0} ({2} eb_created_by, eb_created_at, eb_loc_id, eb_ver_id) VALUES ({3} @eb_createdby, {1}, @eb_loc_id, @{0}_eb_ver_id); ", tblName, DataDB.EB_CURRENT_TIMESTAMP, "{0}", "{1}");//eb_ver_id included
                    //_qry = string.Format("INSERT INTO {0} ({2} eb_created_by, eb_created_at, eb_loc_id) VALUES ({3} :eb_createdby, {1}, :eb_loc_id); ", tblName, DataDB.EB_CURRENT_TIMESTAMP, "{0}", "{1}");
                    if (DataDB.Vendor == DatabaseVendors.MYSQL)
                        _qry += string.Format("SELECT eb_persist_currval('{0}_id_seq');", tblName);
                    if (_this.IsLocEditable)
                        _qry = _qry.Replace(", eb_loc_id", string.Empty).Replace(", @eb_loc_id", string.Empty);
                }
                else if (isIns)
                    _qry = string.Format("INSERT INTO {0} ({3} eb_created_by, eb_created_at, eb_loc_id, {2}_id) VALUES ({4} @eb_createdby, {1}, @eb_loc_id , (SELECT eb_currval('{2}_id_seq')));", tblName, DataDB.EB_CURRENT_TIMESTAMP, _this.TableName, "{0}", "{1}");
                else
                    _qry = string.Format("INSERT INTO {0} ({3} eb_created_by, eb_created_at, eb_loc_id, {2}_id) VALUES ({4} @eb_createdby, {1}, @eb_loc_id , @{2}_id);", tblName, DataDB.EB_CURRENT_TIMESTAMP, _this.TableName, "{0}", "{1}");
            }
            else
            {
                if (tblName.Equals(_this.TableName))
                {
                    if (_this.DataPusherConfig.SourceRecId <= 0)
                        _qry = string.Format("INSERT INTO {0} ({4} eb_created_by, eb_created_at, eb_loc_id, eb_ver_id, {2}_id, eb_push_id, eb_lock) VALUES ({5} @eb_createdby, {1}, @eb_loc_id, @{0}_eb_ver_id, (SELECT eb_currval('{2}_id_seq')), '{3}', 'T'); ", tblName, DataDB.EB_CURRENT_TIMESTAMP, _this.DataPusherConfig.SourceTable, _this.DataPusherConfig.MultiPushId, "{0}", "{1}");
                    else
                        _qry = string.Format("INSERT INTO {0} ({4} eb_created_by, eb_created_at, eb_loc_id, eb_ver_id, {2}_id, eb_push_id, eb_lock) VALUES ({5} @eb_createdby, {1}, @eb_loc_id, @{0}_eb_ver_id, @{2}_id, '{3}', 'T'); ", tblName, DataDB.EB_CURRENT_TIMESTAMP, _this.DataPusherConfig.SourceTable, _this.DataPusherConfig.MultiPushId, "{0}", "{1}");
                    if (DataDB.Vendor == DatabaseVendors.MYSQL)
                        _qry += string.Format("SELECT eb_persist_currval('{0}_id_seq');", tblName);
                    if (_this.IsLocEditable)
                        _qry = _qry.Replace(", eb_loc_id", string.Empty).Replace(", @eb_loc_id", string.Empty);
                }
                else if (isIns)
                    _qry = string.Format("INSERT INTO {0} ({3} eb_created_by, eb_created_at, eb_loc_id, {2}_id) VALUES ({4} @eb_createdby, {1}, @eb_loc_id , (SELECT eb_currval('{2}_id_seq')));", tblName, DataDB.EB_CURRENT_TIMESTAMP, _this.TableName, "{0}", "{1}");
                else
                    _qry = string.Format("INSERT INTO {0} ({3} eb_created_by, eb_created_at, eb_loc_id, {2}_id) VALUES ({4} @eb_createdby, {1}, @eb_loc_id , @{2}_id);", tblName, DataDB.EB_CURRENT_TIMESTAMP, _this.TableName, "{0}", "{1}");
            }
            return _qry;
        }

        public static string GetUpdateQuery(EbWebForm _this, IDatabase DataDB, string tblName, bool isDel)
        {
            string _qry;
            if (_this.DataPusherConfig == null)
            {
                if (tblName.Equals(_this.TableName))
                    _qry = string.Format("UPDATE {0} SET {2} eb_lastmodified_by = @eb_modified_by, eb_lastmodified_at = {1} WHERE id = {3} AND COALESCE(eb_del, 'F') = 'F';", tblName, DataDB.EB_CURRENT_TIMESTAMP, "{0}", "{1}");
                else
                    _qry = string.Format("UPDATE {0} SET {3} eb_lastmodified_by = @eb_modified_by, eb_lastmodified_at = {1} WHERE id = {4} AND {2}_id = @{2}_id AND COALESCE(eb_del, 'F') = 'F';", tblName, DataDB.EB_CURRENT_TIMESTAMP, _this.TableName, isDel ? "eb_del = 'T', " : "{0}", "{1}");
            }
            else
            {
                _qry = string.Format("UPDATE {0} SET {4} eb_lastmodified_by = @eb_modified_by, eb_lastmodified_at = {1} WHERE id = {5} AND {2}_id = @{2}_id AND COALESCE(eb_del, 'F') = 'F' {3};",
                    tblName, DataDB.EB_CURRENT_TIMESTAMP, tblName.Equals(_this.TableName) ? _this.DataPusherConfig.SourceTable : _this.TableName, tblName.Equals(_this.TableName) ? "AND eb_push_id = '" + _this.DataPusherConfig.MultiPushId + "'" : string.Empty, isDel ? "eb_del = 'T', " : "{0}", "{1}");
            }
            return _qry;
        }

    }
}
