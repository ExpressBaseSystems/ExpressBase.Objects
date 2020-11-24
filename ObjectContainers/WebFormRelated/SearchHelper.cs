using ExpressBase.Common;
using ExpressBase.Common.Data;
using ExpressBase.Common.Extensions;
using ExpressBase.Common.LocationNSolution;
using ExpressBase.Common.Structures;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Globalization;

namespace ExpressBase.Objects.WebFormRelated
{
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
                        if (!_data.ContainsKey(Column.Name))
                            _data.Add(Column.Name, Convert.ToString(Column.Value));
                    }
                }
            }
            if (_data.Count == 0)
                return string.Empty;
            return JsonConvert.SerializeObject(_data);
        }

        public static async void InsertOrUpdate(IDatabase DataDB, EbWebForm _webForm) 
        {
            string _data = GetJsonData(_webForm);
            if (string.IsNullOrEmpty(_data))
                return;
            InsertOrUpdate(DataDB, _data, _webForm.RefId, _webForm.TableRowId, _webForm.UserObj.UserId, _webForm.DisplayName);
        }

        public static int InsertOrUpdate(IDatabase DataDB, string JsonData, string RefId, int DataId, int UserId, string DispName)
        {
            if (string.IsNullOrEmpty(JsonData))
                return 0;

            string Qry = "SELECT id, data_json FROM eb_index_table WHERE ref_id = @ref_id AND data_id = @data_id AND COALESCE(eb_del, '') = 'F';";
            DbParameter[] parameters = new DbParameter[]
            {
                DataDB.GetNewParameter("ref_id", EbDbTypes.String, RefId),
                DataDB.GetNewParameter("data_id", EbDbTypes.Int32, DataId)
            };
            EbDataTable dt = DataDB.DoQuery(Qry, parameters);

            List<DbParameter> Param = new List<DbParameter>() 
            {
                 DataDB.GetNewParameter("data_json", EbDbTypes.String, JsonData),
                 DataDB.GetNewParameter("eb_user_id", EbDbTypes.Int32, UserId)
            };

            if (dt.Rows.Count > 0)
            {
                int eit_id = Convert.ToInt32(dt.Rows);
                Qry = $@"UPDATE eb_index_table SET data_json = @data_json, modified_by = @eb_user_id, modified_at = {DataDB.EB_CURRENT_TIMESTAMP}
                    WHERE id = @id;";
                Param.Add(DataDB.GetNewParameter("id", EbDbTypes.Int32, eit_id));
            }
            else
            {
                Qry = $@"INSERT INTO eb_index_table (display_name, data_json, ref_id, data_id, created_by, created_at, modified_by, modified_at, eb_del)
                    VALUES (@display_name, @data_json, @ref_id, @data_id, @eb_user_id, {DataDB.EB_CURRENT_TIMESTAMP}, @eb_user_id, {DataDB.EB_CURRENT_TIMESTAMP}, 'F');";
                Param.Add(DataDB.GetNewParameter("display_name", EbDbTypes.String, DispName));
                Param.Add(DataDB.GetNewParameter("ref_id", EbDbTypes.String, RefId));
                Param.Add(DataDB.GetNewParameter("data_id", EbDbTypes.Int32, DataId));
            }
            
            return DataDB.DoNonQuery(Qry, Param.ToArray());
        }

        public static int Delete(IDatabase DataDB, EbWebForm _webForm)
        {
            throw new NotImplementedException();
        }

        public static string GetSearchResults(IDatabase DataDB, Eb_Solution SolutionObj, string searchTxt)
        {
            List<SearchRsltData> _data = new List<SearchRsltData>();
            string Qry = @"SELECT eit.id, eit.display_name, eit.data_json, eit.ref_id, eit.data_id, eit.created_by, eit.created_at, eit.modified_by, eit.modified_at FROM eb_index_table eit
                WHERE COALESCE(eit.eb_del, '') = 'F' AND (SELECT COUNT(*) from json_each_text(eit.data_json :: JSON) WHERE LOWER(value) like '%' || @searchTxt || '%') > 0 LIMIT 100; ";

            EbDataTable dt = DataDB.DoQuery(Qry, new DbParameter[]
            {
                DataDB.GetNewParameter("searchTxt", EbDbTypes.String, string.IsNullOrEmpty(searchTxt) ? "" : searchTxt.ToLower())
            });

            foreach (EbDataRow dr in dt.Rows)
                _data.Add(new SearchRsltData(dr, SolutionObj));

            return JsonConvert.SerializeObject(_data);
        }
    }

    class SearchRsltData
    {
        public SearchRsltData(EbDataRow dr, Eb_Solution SolutionObj)
        {
            this.DisplayName = Convert.ToString(dr[1]);
            this.Data = JsonConvert.DeserializeObject<Dictionary<string, string>>(Convert.ToString(dr[2]));
            string param = JsonConvert.SerializeObject(new Param[] { new Param() { Name = "id", Type = "7", Value = Convert.ToString(dr[4]) } }).ToBase64();
            this.Link = $"../WebForm/Index?refid={Convert.ToString(dr[3])}&_params={param}&_mode=1";
            int createdBy = Convert.ToInt32(dr[5]);
            this.CreatedBy = SolutionObj.Users.ContainsKey(createdBy) ? SolutionObj.Users[createdBy] : "No Name";
            this.CreatedAt = Convert.ToDateTime(dr[6]).ConvertFromUtc("(UTC+05:30) Chennai, Kolkata, Mumbai, New Delhi").ToString("dd-MM-yyyy hh:mm tt", CultureInfo.InvariantCulture);
            int modifiedBy = Convert.ToInt32(dr[7]);
            this.ModifiedBy = SolutionObj.Users.ContainsKey(modifiedBy) ? SolutionObj.Users[modifiedBy] : "No Name";
            this.ModifiedAt = Convert.ToDateTime(dr[8]).ConvertFromUtc("(UTC+05:30) Chennai, Kolkata, Mumbai, New Delhi").ToString("dd-MM-yyyy hh:mm tt", CultureInfo.InvariantCulture);
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
