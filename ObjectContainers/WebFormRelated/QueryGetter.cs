using ExpressBase.Common;
using ExpressBase.Common.Objects;
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
                    _cols = "eb_loc_id, eb_ver_id, eb_lock, eb_push_id, eb_src_id, eb_created_by, eb_void, eb_created_at, id";
                else if (_table.TableType == WebFormTableTypes.Review)
                    _id = $"eb_ver_id = @{_this.FormSchema.MasterTable}_eb_ver_id AND eb_src_id";
                else
                    _id = _this.FormSchema.MasterTable + "_id";

                if (_table.TableType == WebFormTableTypes.Grid)
                    _cols = "eb_loc_id, id, eb_row_num";

                IEnumerable<ColumnSchema> _columns = _table.Columns.Where(x => (!x.Control.DoNotPersist || x.Control.IsSysControl));
                if (_columns.Count() > 0)
                {
                    _cols += ", " + String.Join(", ", _columns.Select(x => x.ColumnName));
                    IEnumerable<ColumnSchema> _ph_cols = _columns.Where(x => x.Control is EbPhone && (x.Control as EbPhone).Sendotp);
                    if (_ph_cols.Count() > 0)
                        _cols += ", " + String.Join(", ", _ph_cols.Select(x => x.ColumnName + FormConstants._verified));
                }

                if (_this.DataPusherConfig == null)// master form
                {
                    query += string.Format("SELECT {0} FROM {1} WHERE {2} = @{3}_id AND COALESCE(eb_del, 'F') = 'F' {4};",
                        _cols, _table.TableName, _id, _this.FormSchema.MasterTable, _table.TableType == WebFormTableTypes.Grid ? (_table.DescOdr ? "ORDER BY eb_row_num DESC" : "ORDER BY eb_row_num") : "ORDER BY id");

                    foreach (ColumnSchema Col in _table.Columns.FindAll(e => !e.Control.DoNotPersist && e.Control is IEbPowerSelect && !(e.Control as IEbPowerSelect).IsDataFromApi))
                        _queryPs += (Col.Control as IEbPowerSelect).GetSelectQuery(DataDB, _service, Col.ColumnName, _table.TableName, _id, _this.FormSchema.MasterTable);
                }
                else
                {
                    if (_table.TableName == _this.FormSchema.MasterTable)
                        query += string.Format("SELECT {0} FROM {1} WHERE {2}_id = @{2}_id AND eb_push_id = '{3}' AND COALESCE(eb_del, 'F') = 'F';",
                            _cols, _table.TableName, _this.DataPusherConfig.SourceTable, _this.DataPusherConfig.MultiPushId);
                    else
                        query += string.Format("SELECT {0} FROM {1} WHERE {2}_id = (SELECT id FROM {2} WHERE {3}_id = @{3}_id AND eb_push_id = '{4}' AND COALESCE(eb_del, 'F') = 'F' LIMIT 1) AND COALESCE(eb_del, 'F') = 'F' {5};",
                            _cols, _table.TableName, _this.FormSchema.MasterTable, _this.DataPusherConfig.SourceTable, _this.DataPusherConfig.MultiPushId, _table.TableType == WebFormTableTypes.Grid ? (_table.DescOdr ? "ORDER BY eb_row_num DESC" : "ORDER BY eb_row_num") : "ORDER BY id");
                }
                _qryCount++;
            }
            bool MuCtrlFound = false;
            foreach (EbControl Ctrl in _this.FormSchema.ExtendedControls)
            {
                if (Ctrl is EbProvisionUser)
                {
                    if (!MuCtrlFound)
                    {
                        extquery += (Ctrl as EbProvisionUser).GetSelectQuery(_this.FormSchema.MasterTable);
                        MuCtrlFound = true;
                        _qryCount++;
                    }
                    extquery += (Ctrl as EbProvisionUser).GetMappedUserQuery(_this.FormSchema.MasterTable);
                    _qryCount++;
                }
                else if (Ctrl is EbProvisionLocation)
                {
                    extquery += (Ctrl as EbProvisionLocation).GetSelectQuery(_this.FormSchema.MasterTable);
                    _qryCount++;
                }
                else if (Ctrl is EbReview)
                {
                    extquery += (Ctrl as EbReview).GetSelectQuery(_this.RefId, _this.FormSchema.MasterTable);
                    _qryCount++;
                }
                else if (Ctrl is EbDisplayPicture)
                {
                    extquery += (Ctrl as EbDisplayPicture).GetSelectQuery(DataDB, _this.FormSchema.MasterTable);
                    _qryCount++;
                }
                else if (Ctrl is EbSimpleFileUploader)
                {
                    extquery += (Ctrl as EbSimpleFileUploader).GetSelectQuery(DataDB, _this.FormSchema.MasterTable);
                    _qryCount++;
                }
                else if (Ctrl is EbMeetingPicker)
                {
                    extquery += (Ctrl as EbMeetingPicker).GetSelectQuery(DataDB, _this.FormSchema.MasterTable);
                    //_qryCount++;
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

                foreach (ColumnSchema Col in _table.Columns.FindAll(e => !e.Control.DoNotPersist && e.Control is EbDGPowerSelectColumn && !(e.Control as EbDGPowerSelectColumn).IsDataFromApi))
                {
                    _queryPs += (Col.Control as EbDGPowerSelectColumn).GetSelectQuery123(DataDB, _service, _table.TableName, Col.ColumnName, _prntTbl, _this.FormSchema.MasterTable);
                }
            }
            return query;
        }

        public static string GetDeleteQuery(EbWebForm _this, IDatabase DataDB)
        {
            string FullQry = string.Empty;
            foreach (EbWebForm ebWebForm in _this.FormCollection)
            {
                WebFormSchema _schema = ebWebForm.FormSchema;
                foreach (TableSchema _table in _schema.Tables)
                {
                    string autoIdBckUp = string.Empty;
                    //if (ebWebForm.AutoId != null && ebWebForm.AutoId.TableName == _table.TableName)
                    //    autoIdBckUp = string.Format(", {0}_ebbkup = {0}, {0} = CONCAT({0}, '_ebbkup')", ebWebForm.AutoId.Name);

                    string Qry = $"UPDATE {_table.TableName} SET eb_del='T', eb_lastmodified_by = @eb_lastmodified_by, eb_lastmodified_at = {DataDB.EB_CURRENT_TIMESTAMP } {autoIdBckUp} ";
                    
                    if (ebWebForm.DataPusherConfig == null)
                        Qry += $"WHERE {(_table.TableName == _schema.MasterTable ? "id" : (_schema.MasterTable + "_id"))} = @{_schema.MasterTable}_id AND COALESCE(eb_del, 'F') = 'F';";                    
                    else
                    {
                        EbDataPusherConfig _conf = ebWebForm.DataPusherConfig;
                        if (_table.TableName == _schema.MasterTable)
                            Qry += $"WHERE {_conf.SourceTable}_id = @{_conf.SourceTable}_id AND eb_push_id = '{_conf.MultiPushId}' AND COALESCE(eb_del, 'F') = 'F';";
                        else
                            Qry += $"WHERE {_schema.MasterTable}_id = (SELECT id FROM {_schema.MasterTable} WHERE {_conf.SourceTable}_id = @{_conf.SourceTable}_id " +
                                $"AND eb_push_id = '{_conf.MultiPushId}' AND COALESCE(eb_del, 'F') = 'F' LIMIT 1) AND COALESCE(eb_del, 'F') = 'F';";
                    }
                    FullQry = Qry + FullQry;
                }                    
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

        public static string GetCancelQuery(EbWebForm _this, IDatabase DataDB)
        {
            string FullQry = string.Empty;
            foreach (EbWebForm ebWebForm in _this.FormCollection)
            {
                WebFormSchema _schema = ebWebForm.FormSchema;
                foreach (TableSchema _table in _schema.Tables)
                {
                    string Qry = $"UPDATE {_table.TableName} SET eb_void='T', eb_lastmodified_by = @eb_lastmodified_by, eb_lastmodified_at = {DataDB.EB_CURRENT_TIMESTAMP } ";

                    if (ebWebForm.DataPusherConfig == null)
                        Qry += $"WHERE {(_table.TableName == _schema.MasterTable ? "id" : (_schema.MasterTable + "_id"))} = @{_schema.MasterTable}_id AND COALESCE(eb_del, 'F') = 'F' AND COALESCE(eb_void, 'F') = 'F';";
                    else
                    {
                        EbDataPusherConfig _conf = ebWebForm.DataPusherConfig;
                        if (_table.TableName == _schema.MasterTable)
                            Qry += $"WHERE {_conf.SourceTable}_id = @{_conf.SourceTable}_id AND eb_push_id = '{_conf.MultiPushId}' AND COALESCE(eb_del, 'F') = 'F' AND COALESCE(eb_void, 'F') = 'F';";
                        else
                            Qry += $"WHERE {_schema.MasterTable}_id = (SELECT id FROM {_schema.MasterTable} WHERE {_conf.SourceTable}_id = @{_conf.SourceTable}_id " +
                                $"AND eb_push_id = '{_conf.MultiPushId}' AND COALESCE(eb_del, 'F') = 'F' AND COALESCE(eb_void, 'F') = 'F' LIMIT 1) AND COALESCE(eb_del, 'F') = 'F' AND COALESCE(eb_void, 'F') = 'F';";
                    }
                    FullQry = Qry + FullQry;
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

        public static string GetInsertQuery(EbWebForm _this, IDatabase DataDB, string tblName, bool isIns)
        {
            string _qry;

            if (tblName.Equals(_this.TableName))
            {
                if (_this.DataPusherConfig == null)
                {
                    _qry = $@"INSERT INTO {tblName} ({{0}} eb_created_by, eb_created_at, eb_loc_id, eb_ver_id, eb_signin_log_id) 
                                VALUES ({{1}} @eb_createdby, {DataDB.EB_CURRENT_TIMESTAMP}, @eb_loc_id, @{_this.TableName}_eb_ver_id, @eb_signin_log_id); ";
                }
                else
                {
                    string srcRef = _this.DataPusherConfig.SourceRecId <= 0 ? $"(SELECT eb_currval('{_this.DataPusherConfig.SourceTable}_id_seq'))" : $"@{_this.DataPusherConfig.SourceTable}_id";

                    _qry = $@"INSERT INTO {tblName} ({{0}} eb_created_by, eb_created_at, eb_loc_id, eb_ver_id, {_this.DataPusherConfig.SourceTable}_id, eb_src_id, eb_push_id, eb_lock, eb_signin_log_id) 
                                    VALUES ({{1}} @eb_createdby, {DataDB.EB_CURRENT_TIMESTAMP}, @eb_loc_id, @{_this.TableName}_eb_ver_id, {srcRef}, {srcRef}, '{_this.DataPusherConfig.MultiPushId}', 'T', @eb_signin_log_id); ";
                }

                if (DataDB.Vendor == DatabaseVendors.MYSQL)
                    _qry += $"SELECT eb_persist_currval('{tblName}_id_seq'); ";
                if (_this.IsLocEditable)
                    _qry = _qry.Replace(", eb_loc_id", string.Empty).Replace(", @eb_loc_id", string.Empty);
            }
            else if (tblName.Equals("eb_approval_lines"))
            {
                _qry = $@"INSERT INTO eb_approval_lines ({{0}} eb_created_by, eb_created_at, eb_loc_id, eb_src_id, eb_ver_id, eb_signin_log_id) 
                            VALUES ({{1}} @eb_createdby, {DataDB.EB_CURRENT_TIMESTAMP}, @eb_loc_id, @{_this.TableName}_id, @{_this.TableName}_eb_ver_id, @eb_signin_log_id); ";
                if (DataDB.Vendor == DatabaseVendors.MYSQL)
                    _qry += "SELECT eb_persist_currval('eb_approval_lines_id_seq'); ";
                // eb_approval - update eb_approval_lines_id
                _qry += $@"UPDATE eb_approval SET eb_approval_lines_id = (SELECT eb_currval('eb_approval_lines_id_seq')), eb_lastmodified_by = @eb_modified_by, eb_lastmodified_at = {DataDB.EB_CURRENT_TIMESTAMP} 
                               WHERE eb_src_id = @{_this.TableName}_id AND eb_ver_id =  @{_this.TableName}_eb_ver_id AND COALESCE(eb_del, 'F') = 'F'; ";
            }
            else
            {
                string srcRef = isIns ? $"(SELECT eb_currval('{_this.TableName}_id_seq'))" : $"@{_this.TableName}_id";
                _qry = $@"INSERT INTO {tblName} ({{0}} eb_created_by, eb_created_at, eb_loc_id, {_this.TableName}_id, eb_signin_log_id) 
                            VALUES ({{1}} @eb_createdby, {DataDB.EB_CURRENT_TIMESTAMP}, @eb_loc_id , {srcRef}, @eb_signin_log_id); ";
                if (isIns && DataDB.Vendor == DatabaseVendors.MYSQL)
                    _qry += $"SELECT eb_persist_currval('{tblName}_id_seq'); ";
            }

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
                // if isDel is true then consider lines table also
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
