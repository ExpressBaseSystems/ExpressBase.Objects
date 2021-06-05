using ExpressBase.Common;
using ExpressBase.Common.Constants;
using ExpressBase.Common.Data;
using ExpressBase.Common.Extensions;
using ExpressBase.Common.LocationNSolution;
using ExpressBase.Common.Objects;
using ExpressBase.Common.Structures;
using ExpressBase.Security;
using Newtonsoft.Json;
using ServiceStack;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpressBase.Objects.WebFormRelated
{
    public enum SH_LinkType
    {
        FormViewMode = 0,
        LM = 1
    }

    public static class SearchHelper
    {
        public static string GetJsonData(EbWebForm _webForm)
        {
            if (_webForm.FormData == null)
                return string.Empty;
            Dictionary<string, string> _data = new Dictionary<string, string>();

            foreach (TableSchema _table in _webForm.FormSchema.Tables)
            {
                if (_table.TableType == WebFormTableTypes.Normal &&
                    _webForm.FormData.MultipleTables.ContainsKey(_table.TableName) && _webForm.FormData.MultipleTables[_table.TableName].Count > 0)
                {
                    foreach (SingleColumn Column in _webForm.FormData.MultipleTables[_table.TableName][0].Columns.FindAll(e => e.Control?.Index == true))
                    {
                        if (!_data.ContainsKey(Column.Control.Label))
                        {
                            string _val = Convert.ToString(Column.Value);
                            if (!string.IsNullOrEmpty(_val))
                                _data.Add(Column.Control.Label, _val);
                        }
                    }
                }
            }
            if (_data.Count == 0)
                return string.Empty;
            return JsonConvert.SerializeObject(_data);
        }

        public static void InsertOrUpdate(IDatabase DataDB, EbWebForm _webForm)
        {
            try
            {
                string _data = GetJsonData(_webForm);
                if (string.IsNullOrEmpty(_data))
                {
                    if (ExistsIndexControls(_webForm))
                        Task.Run(() => Delete(DataDB, _webForm.RefId, _webForm.TableRowId));
                }
                else
                    Task.Run(() => InsertOrUpdate(DataDB, _data, _webForm.RefId, _webForm.TableRowId, _webForm.UserObj.UserId, _webForm.DisplayName));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in insert/update global search data. Message: {ex.Message}\nStackTrace: {ex.StackTrace}");
            }
        }

        public static int InsertOrUpdate(IDatabase DataDB, string JsonData, string RefId, int DataId, int UserId, string DispName)
        {
            string Qry = $@"UPDATE eb_index_table SET data_json = @data_json, modified_by = @eb_user_id, modified_at = {DataDB.EB_CURRENT_TIMESTAMP}
                            WHERE ref_id = @ref_id AND data_id = @data_id AND COALESCE(eb_del, 'F') = 'F';
                        INSERT INTO eb_index_table (display_name, data_json, ref_id, data_id, created_by, created_at, modified_by, modified_at, eb_del)
                            SELECT @display_name, @data_json, @ref_id, @data_id, @eb_user_id, {DataDB.EB_CURRENT_TIMESTAMP}, @eb_user_id, {DataDB.EB_CURRENT_TIMESTAMP}, 'F'
                            WHERE NOT EXISTS (SELECT 1 FROM eb_index_table WHERE ref_id = @ref_id AND data_id = @data_id AND COALESCE(eb_del, 'F') = 'F');";

            DbParameter[] parameters = new DbParameter[]
            {
                DataDB.GetNewParameter("ref_id", EbDbTypes.String, RefId),
                DataDB.GetNewParameter("data_id", EbDbTypes.Int32, DataId),
                DataDB.GetNewParameter("display_name", EbDbTypes.String, DispName),
                DataDB.GetNewParameter("data_json", EbDbTypes.String, JsonData),
                DataDB.GetNewParameter("eb_user_id", EbDbTypes.Int32, UserId)
            };

            int temp = DataDB.DoNonQuery(Qry, parameters);
            return temp;
        }

        public static void Delete(IDatabase DataDB, string RefId, int TableRowId)
        {
            try
            {
                int t = DataDB.DoNonQuery(GetDeleteQuery(0), new DbParameter[]
                {
                    DataDB.GetNewParameter("ref_id", EbDbTypes.String, RefId),
                    DataDB.GetNewParameter("data_id_0", EbDbTypes.Int32, TableRowId)
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exeption in SearchHelper.index.delete. \nMessage: {ex.Message}\nStackTrace: {ex.StackTrace}");
            }
        }

        public static bool ExistsIndexControls(EbWebForm _webForm)
        {
            bool indexCtrlFound = false;
            foreach (TableSchema _table in _webForm.FormSchema.Tables)
            {
                if (_table.TableType == WebFormTableTypes.Normal)
                {
                    indexCtrlFound = _table.Columns.Exists(e => e.Control.Index);
                    if (indexCtrlFound)
                        break;
                }
            }
            return indexCtrlFound;
        }

        public static string UpdateIndexes(IDatabase DataDB, EbWebForm _webForm)
        {
            DateTime startdt = DateTime.Now;
            string Message = $"Service started at {startdt}.";
            Console.WriteLine("Update all indexes start: " + startdt);
            EbSystemColumns ebs = _webForm.SolutionObj.SolutionSettings.SystemColumns;

            string qCols = $"mtbl.id, mtbl.{ebs[SystemColumns.eb_created_by]}, mtbl.{ebs[SystemColumns.eb_created_at]}, mtbl.{ebs[SystemColumns.eb_lastmodified_by]}, mtbl.{ebs[SystemColumns.eb_lastmodified_at]}",
                qJoin = string.Empty,
                qCdtn = $"COALESCE(mtbl.{ebs[SystemColumns.eb_del]}, {ebs.GetBoolFalse(SystemColumns.eb_del)}) = {ebs.GetBoolFalse(SystemColumns.eb_del)}";
            const int sysColCnt = 5;
            int tblCnt = 1, colCnt = sysColCnt;//Count
            List<string> labels = new List<string>();
            bool deleteOld = false;

            foreach (TableSchema _table in _webForm.FormSchema.Tables)
            {
                if (_table.TableType == WebFormTableTypes.Normal)
                {
                    List<ColumnSchema> _columns = _table.Columns.FindAll(e => e.Control?.Index == true && !e.Control.DoNotPersist);
                    if (_columns.Count > 0)
                    {
                        if (_table.TableName == _webForm.FormSchema.MasterTable)
                        {
                            qCols += ", " + _columns.Select(e => "mtbl." + e.ColumnName).Join(", ");
                        }
                        else
                        {
                            qCols += ", " + _columns.Select(e => $"tbl{tblCnt}." + e.ColumnName).Join(", ");
                            qJoin += $" LEFT JOIN {_table.TableName} tbl{tblCnt} ON mtbl.id = tbl{tblCnt}.{_webForm.FormSchema.MasterTable}_id ";
                            qCdtn += $" AND COALESCE(tbl{tblCnt}.eb_del, 'F') = 'F'";
                            tblCnt++;
                        }

                        foreach (ColumnSchema _column in _columns)
                        {
                            labels.Add(_column.Control.Label);
                            colCnt++;
                            Message += $"\nIndexed {_column.Control.Label}.";
                        }
                    }
                }
            }
            if (labels.Count > 0)
            {
                string fullQry = $"SELECT {qCols} FROM {_webForm.FormSchema.MasterTable} mtbl {qJoin} WHERE {qCdtn};";
                EbDataTable dt = DataDB.DoQuery(fullQry);
                if (dt.Rows.Count > 0)
                {
                    Console.WriteLine($"Existing {dt.Rows.Count} records.");
                    Message += $"\nExisting {dt.Rows.Count} records.";
                    string upsertQry = string.Empty;
                    List<DbParameter> parameters = new List<DbParameter>()
                    {
                        DataDB.GetNewParameter("ref_id", EbDbTypes.String, _webForm.RefId),
                        DataDB.GetNewParameter("display_name", EbDbTypes.String, _webForm.DisplayName)
                    };
                    int i, j;
                    int flushCounter = 0, UpserteRecords = 0;
                    for (i = 0; i < dt.Rows.Count; i++, flushCounter++)
                    {
                        EbDataRow dr = dt.Rows[i];
                        Dictionary<string, string> JsonData = new Dictionary<string, string>();
                        for (j = sysColCnt; j < colCnt; j++)
                        {
                            string _val = Convert.ToString(dr[j]);
                            string _lbl = labels[j - sysColCnt];
                            if (!(dr.IsDBNull(j) || string.IsNullOrEmpty(_val) || JsonData.ContainsKey(_lbl)))
                            {
                                JsonData.Add(_lbl, _val);
                            }
                        }
                        parameters.Add(DataDB.GetNewParameter($"data_id_{i}", EbDbTypes.Int32, dr[0]));
                        if (JsonData.Count > 0)
                        {
                            parameters.Add(DataDB.GetNewParameter($"created_by_{i}", EbDbTypes.Int32, dr[1]));
                            parameters.Add(DataDB.GetNewParameter($"created_at_{i}", EbDbTypes.DateTime, dr[2]));
                            parameters.Add(DataDB.GetNewParameter($"modified_by_{i}", EbDbTypes.Int32, dr[3]));
                            parameters.Add(DataDB.GetNewParameter($"modified_at_{i}", EbDbTypes.DateTime, dr[4]));
                            parameters.Add(DataDB.GetNewParameter($"data_json_{i}", EbDbTypes.String, JsonConvert.SerializeObject(JsonData)));
                            upsertQry += GetUpsertQuery(i);
                        }
                        else
                        {
                            upsertQry += GetDeleteQuery(i);
                        }

                        if (flushCounter >= 10000)////
                        {
                            UpserteRecords += DataDB.DoNonQuery(upsertQry, parameters.ToArray());
                            Console.WriteLine($"Upserted {UpserteRecords} records.");
                            Message += $"\nUpserted {UpserteRecords} records.";

                            flushCounter = 0;
                            parameters = new List<DbParameter>()
                            {
                                DataDB.GetNewParameter("ref_id", EbDbTypes.String, _webForm.RefId),
                                DataDB.GetNewParameter("display_name", EbDbTypes.String, _webForm.DisplayName)
                            };
                            upsertQry = string.Empty;
                        }
                    }

                    UpserteRecords += DataDB.DoNonQuery(upsertQry, parameters.ToArray());
                    Console.WriteLine($"Upserted {UpserteRecords} records.");
                    Message += $"\nUpserted {UpserteRecords} records.";
                }
                else
                {
                    deleteOld = true;
                }
            }
            else
            {
                deleteOld = true;
            }
            if (deleteOld)
            {
                string delQry = "UPDATE eb_index_table SET eb_del = 'T' WHERE ref_id = @ref_id AND COALESCE(eb_del, 'F') = 'F';";
                int t = DataDB.DoNonQuery(delQry, new DbParameter[] { DataDB.GetNewParameter("ref_id", EbDbTypes.String, _webForm.RefId) });
                Console.WriteLine($"Nothing to index. Deleted old {t} records.");
                Message += $"\nDeleted {t} records.";
            }

            Console.WriteLine("Update all indexes end : Execution Time = " + (DateTime.Now - startdt).TotalMilliseconds);
            Message += $"\nCompleted. Execution Time: {(DateTime.Now - startdt).TotalMilliseconds}.";
            return Message;
        }

        private static string GetUpsertQuery(int i)
        {
            return $@"UPDATE eb_index_table SET display_name = @display_name, data_json = @data_json_{i}, modified_by = @modified_by_{i}, modified_at = @modified_at_{i}
                        WHERE ref_id = @ref_id AND data_id = @data_id_{i} AND COALESCE(eb_del, 'F') = 'F';
                    INSERT INTO eb_index_table (display_name, data_json, ref_id, data_id, created_by, created_at, modified_by, modified_at, eb_del)
                        SELECT @display_name, @data_json_{i}, @ref_id, @data_id_{i}, @created_by_{i}, @created_at_{i}, @modified_by_{i}, @modified_at_{i}, 'F'
                        WHERE NOT EXISTS (SELECT 1 FROM eb_index_table WHERE ref_id = @ref_id AND data_id = @data_id_{i} AND COALESCE(eb_del, 'F') = 'F');";
        }

        private static string GetDeleteQuery(int i)
        {
            return $"UPDATE eb_index_table SET eb_del = 'T' WHERE ref_id = @ref_id AND data_id = @data_id_{i} AND COALESCE(eb_del, 'F') = 'F';";
        }

        public static string GetSearchResults(IDatabase DataDB, Eb_Solution SolutionObj, User UserObj, string searchTxt)
        {
            List<SearchRsltData> _data = new List<SearchRsltData>();
            string Qry = @"SELECT COUNT(*) FROM eb_index_table eit WHERE COALESCE(eit.eb_del, 'F') = 'F' AND (SELECT COUNT(*) from json_each_text(eit.data_json :: JSON) WHERE LOWER(value) like '%' || @searchTxt || '%') > 0;
                SELECT eit.id, eit.display_name, eit.data_json, eit.ref_id, eit.data_id, eit.created_by, eit.created_at, eit.modified_by, eit.modified_at, eit.link_type FROM eb_index_table eit
                WHERE COALESCE(eit.eb_del, 'F') = 'F' AND (SELECT COUNT(*) from json_each_text(eit.data_json :: JSON) WHERE LOWER(value) like '%' || @searchTxt || '%') > 0 ORDER BY eit.modified_at DESC LIMIT 100; ";

            EbDataSet ds = DataDB.DoQueries(Qry, new DbParameter[]
            {
                DataDB.GetNewParameter("searchTxt", EbDbTypes.String, string.IsNullOrEmpty(searchTxt) ? "" : searchTxt.ToLower())
            });
            int rowCount = Convert.ToInt32(ds.Tables[0].Rows[0][0]);
            foreach (EbDataRow dr in ds.Tables[1].Rows)
                _data.Add(new SearchRsltData(dr, SolutionObj, UserObj));

            return JsonConvert.SerializeObject(new SearchResponse() { Data = _data, RowCount = rowCount });
        }

        public static void InsertOrUpdate_LM(IDatabase DataDB, string JsonData, int DataId, int UserId)
        {
            string Qry = $@"UPDATE eb_index_table SET display_name = 'Lead Management', data_json = @data_json, modified_by = @eb_user_id, modified_at = {DataDB.EB_CURRENT_TIMESTAMP}
                            WHERE link_type = @link_type AND data_id = @data_id AND COALESCE(eb_del, 'F') = 'F';

                        INSERT INTO eb_index_table (display_name, data_json, link_type, data_id, created_by, created_at, modified_by, modified_at, eb_del)
                            SELECT 'Lead Management', @data_json, @link_type, @data_id, @eb_user_id, {DataDB.EB_CURRENT_TIMESTAMP}, @eb_user_id, {DataDB.EB_CURRENT_TIMESTAMP}, 'F'
                            WHERE NOT EXISTS (SELECT 1 FROM eb_index_table WHERE link_type = @link_type AND data_id = @data_id AND COALESCE(eb_del, 'F') = 'F');";

            DbParameter[] parameters = new DbParameter[]
            {
                DataDB.GetNewParameter("link_type", EbDbTypes.Int32, (int)SH_LinkType.LM),
                DataDB.GetNewParameter("data_id", EbDbTypes.Int32, DataId),
                DataDB.GetNewParameter("data_json", EbDbTypes.String, JsonData),
                DataDB.GetNewParameter("eb_user_id", EbDbTypes.Int32, UserId)
            };

            int temp = DataDB.DoNonQuery(Qry, parameters);
            Console.WriteLine($"InsertOrUpdate_LM for {DataId}. Status: {temp}");
        }

        public static void Delete_LM(IDatabase DataDB, int DataId)
        {
            try
            {
                string delQry = "UPDATE eb_index_table SET eb_del = 'T' WHERE link_type = @link_type AND data_id = @data_id AND COALESCE(eb_del, 'F') = 'F';";
                int t = DataDB.DoNonQuery(delQry, new DbParameter[]
                {
                    DataDB.GetNewParameter("link_type", EbDbTypes.Int32, (int)SH_LinkType.LM),
                    DataDB.GetNewParameter("data_id", EbDbTypes.Int32, DataId)
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exeption in SearchHelper.index.delete. \nMessage: {ex.Message}\nStackTrace: {ex.StackTrace}");
            }
        }

        public static string UpdateIndexes_LM(IDatabase DataDB, int Limit, int Offset)
        {
            DateTime startdt = DateTime.Now;
            string Message = $"Service started at {startdt}.";
            Console.WriteLine("Update all indexes start: " + startdt);
            string lim = string.Empty, off = string.Empty;
            if (Limit > 0)
                lim = "LIMIT " + Limit;
            if (Offset >= 0)
                off = "OFFSET " + Offset;
            string fullQry = $"SELECT id, eb_createdby, eb_createdat, eb_modifiedby, eb_modifiedat, name, genurl, genphoffice, watsapp_phno FROM customers WHERE COALESCE(eb_del, 'F') = 'F' ORDER BY id {lim} {off};";

            EbDataTable dt = DataDB.DoQuery(fullQry);
            if (dt.Rows.Count > 0)
            {
                Console.WriteLine($"Existing {dt.Rows.Count} records.");
                Message += $"\nExisting {dt.Rows.Count} records.";

                try
                {
                    StringBuilder upsertQry = new StringBuilder();
                    List<DbParameter> parameters = new List<DbParameter>() { DataDB.GetNewParameter("link_type", EbDbTypes.Int32, (int)SH_LinkType.LM) };

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        EbDataRow dr = dt.Rows[i];
                        Dictionary<string, string> JsonData = new Dictionary<string, string>();

                        string _nam = Convert.ToString(dr[5]),
                            _mob = Convert.ToString(dr[6]),
                            _pho = Convert.ToString(dr[7]),
                            _wap = Convert.ToString(dr[8]);

                        if (!(dr.IsDBNull(5) || string.IsNullOrEmpty(_nam)))
                            JsonData.Add("Name", _nam);
                        if (!(dr.IsDBNull(6) || string.IsNullOrEmpty(_mob)))
                            JsonData.Add("Mobile", _mob);
                        if (!(dr.IsDBNull(7) || string.IsNullOrEmpty(_pho)))
                            JsonData.Add("Phone", _pho);
                        if (!(dr.IsDBNull(8) || string.IsNullOrEmpty(_wap)))
                            JsonData.Add("WhatsApp", _wap);

                        parameters.Add(DataDB.GetNewParameter($"data_id_{i}", EbDbTypes.Int32, dr[0]));

                        if (JsonData.Count > 0)
                        {
                            parameters.Add(DataDB.GetNewParameter($"created_by_{i}", EbDbTypes.Int32, dr[1]));
                            parameters.Add(DataDB.GetNewParameter($"created_at_{i}", EbDbTypes.DateTime, dr[2]));
                            parameters.Add(DataDB.GetNewParameter($"modified_by_{i}", EbDbTypes.Int32, dr[3]));
                            parameters.Add(DataDB.GetNewParameter($"modified_at_{i}", EbDbTypes.DateTime, dr[4]));
                            parameters.Add(DataDB.GetNewParameter($"data_json_{i}", EbDbTypes.String, JsonConvert.SerializeObject(JsonData)));

                            upsertQry.Append($@"UPDATE eb_index_table SET display_name = 'Lead Management', data_json = @data_json_{i}, modified_by = @modified_by_{i}, modified_at = @modified_at_{i}
                        WHERE link_type = @link_type AND data_id = @data_id_{i} AND COALESCE(eb_del, 'F') = 'F';

                        INSERT INTO eb_index_table (display_name, data_json, link_type, data_id, created_by, created_at, modified_by, modified_at, eb_del)
                            SELECT 'Lead Management', @data_json_{i}, @link_type, @data_id_{i}, @created_by_{i}, @created_at_{i}, @modified_by_{i}, @modified_at_{i}, 'F'
                            WHERE NOT EXISTS (SELECT 1 FROM eb_index_table WHERE link_type = @link_type AND data_id = @data_id_{i} AND COALESCE(eb_del, 'F') = 'F');");
                        }
                        else
                        {
                            upsertQry.Append($"UPDATE eb_index_table SET eb_del = 'T' WHERE link_type = @link_type AND data_id = @data_id_{i} AND COALESCE(eb_del, 'F') = 'F';");
                        }
                    }

                    int temp = DataDB.DoNonQuery(upsertQry.ToString(), parameters.ToArray());
                    Console.WriteLine($"Upserted {temp} records.");
                    Message += $"\nUpserted {temp} records.";
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Exception in UpdateIndexes: {ex.Message}\n{ex.StackTrace}");
                    Message += $"Exception in UpdateIndexes: {ex.Message}\n{ex.StackTrace}";
                }
            }
            else if (Limit == 0 && Offset == 0)
            {
                string delQry = "UPDATE eb_index_table SET eb_del = 'T' WHERE link_type = @link_type AND COALESCE(eb_del, 'F') = 'F';";
                int t = DataDB.DoNonQuery(delQry, new DbParameter[] { DataDB.GetNewParameter("link_type", EbDbTypes.Int32, (int)SH_LinkType.LM) });
                Console.WriteLine($"Nothing to index. Deleted old {t} records.");
                Message += $"\nDeleted {t} records.";
            }
            Console.WriteLine("Update all indexes end : Execution Time = " + (DateTime.Now - startdt).TotalMilliseconds);
            Message += $"\nCompleted. Execution Time: {(DateTime.Now - startdt).TotalMilliseconds}.";
            return Message;
        }
    }

    class SearchResponse
    {
        public List<SearchRsltData> Data { get; set; }
        public int RowCount { get; set; }
    }

    class SearchRsltData
    {
        public SearchRsltData(EbDataRow dr, Eb_Solution SolutionObj, User UserObj)
        {
            this.DisplayName = Convert.ToString(dr[1]);
            this.Data = JsonConvert.DeserializeObject<Dictionary<string, string>>(Convert.ToString(dr[2]));
            int linkType = Convert.ToInt32(dr[9]);
            if (linkType == (int)SH_LinkType.LM)
            {
                this.Link = $"/leadmanagement/{Convert.ToString(dr[4])}";
            }
            else
            {
                string param = JsonConvert.SerializeObject(new Param[] { new Param() { Name = "id", Type = "7", Value = Convert.ToString(dr[4]) } }).ToBase64();
                this.Link = $"/WebForm/Index?_r={Convert.ToString(dr[3])}&_p={param}&_m=1";
            }
            int createdBy = Convert.ToInt32(dr[5]);
            this.CreatedBy = SolutionObj.Users.ContainsKey(createdBy) ? SolutionObj.Users[createdBy] : "No Name";
            this.CreatedAt = Convert.ToDateTime(dr[6]).ConvertFromUtc(UserObj.Preference.TimeZone).ToString(UserObj.Preference.GetShortDatePattern() + " " + UserObj.Preference.GetShortTimePattern(), CultureInfo.InvariantCulture);
            int modifiedBy = Convert.ToInt32(dr[7]);
            this.ModifiedBy = SolutionObj.Users.ContainsKey(modifiedBy) ? SolutionObj.Users[modifiedBy] : "No Name";
            this.ModifiedAt = Convert.ToDateTime(dr[8]).ConvertFromUtc(UserObj.Preference.TimeZone).ToString(UserObj.Preference.GetShortDatePattern() + " " + UserObj.Preference.GetShortTimePattern(), CultureInfo.InvariantCulture);
        }

        public string DisplayName { get; set; }
        public Dictionary<string, string> Data { get; set; }
        public string Link { get; set; }//RefId, data_id
        public string CreatedBy { get; set; }
        public string CreatedAt { get; set; }
        public string ModifiedBy { get; set; }
        public string ModifiedAt { get; set; }
    }
}
