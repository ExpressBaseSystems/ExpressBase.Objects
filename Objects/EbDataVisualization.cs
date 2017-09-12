using ExpressBase.Common;
using ExpressBase.Common.Data;
using ExpressBase.Data;
using ExpressBase.Objects.Objects.DVRelated;
using ExpressBase.Objects.ServiceStack_Artifacts;
using Newtonsoft.Json;
using Npgsql;
using Npgsql.Schema;
using ServiceStack.Redis;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

namespace ExpressBase.Objects
{
    public class EbDataVisualization : EbObject
    {
        public string DataSourceRefId { get; set; }

        [JsonIgnore]
        public EbDataSource EbDataSource { get; set; }

        public string Description { get; set; }

        public DVColumnCollection Columns { get; set; }

        public string RenderAs { get; set; }

        public string IsPaged { get; set; }

        public GOptions options { get; set; }
        public override void AfterRedisGet(RedisClient Redis)
        {
            try
            {
                this.EbDataSource = Redis.Get<EbDataSource>(this.DataSourceRefId);
                this.EbDataSource.AfterRedisGet(Redis);
            }
            catch(Exception e)
            {

            }
        }

        public EbDataSet DoQueries4DataVis(string sql ,ITenantDbFactory df, params DbParameter[] parameters)
        {
            EbDataSet ds = new EbDataSet();
            
            using (var con = df.DataDBRO.GetNewConnection())
            {
                try
                {
                    con.Open();
                    using (var cmd = df.DataDBRO.GetNewCommand(con, sql))
                    {
                        if (parameters != null && parameters.Length > 0)
                            cmd.Parameters.AddRange(parameters);

                        using (var reader = cmd.ExecuteReader())
                        {
                            do
                            {
                                EbDataTable dt = new EbDataTable();
                                this.AddColumns(dt, reader as NpgsqlDataReader);
                                PrepareDataTable4DataVis(reader, dt);
                                ds.Tables.Add(dt);
                            }
                            while (reader.NextResult());
                        }
                    }
                }
                catch (Npgsql.NpgsqlException npgse) { }
            }

            return ds;
        }

        public void PrepareDataTable4DataVis(DbDataReader reader, EbDataTable dt)
        {
            int _fieldCount = reader.FieldCount;
            while (reader.Read())
            {
                EbDataRow dr = dt.NewDataRow();
                for (int i = 0; i < _fieldCount; i++)
                {
                    var _typ = reader.GetFieldType(i);
                    var _coln = dt.Columns[i].ColumnName;
                    if (_typ == typeof(DateTime))
                    {
                        var _val = reader.IsDBNull(i) ? DateTime.Now : reader.GetDateTime(i);
                        var _dvCol = this.Columns.Get(_coln) as DVDateTimeColumn;
                        if (_dvCol.Format == DateFormat.Date) {
                            dr[i] = _val.ToString("dd-MM-yyyy");
                            continue;
                        }
                        else if (_dvCol.Format == DateFormat.Time) {
                            dr[i] = _val.ToString("HH:mm:ss tt");
                            continue;
                        }
                        else if (_dvCol.Format == DateFormat.TimeWithoutTT) {
                            dr[i] = _val.ToString("HH:mm:ss");
                            continue;
                        }
                    }
                    else if (_typ == typeof(string))
                    {
                        dr[i] = reader.IsDBNull(i) ? string.Empty : reader.GetString(i);
                        continue;
                    }
                    else if (_typ == typeof(bool))
                    {
                        dr[i] = reader.IsDBNull(i) ? false : reader.GetBoolean(i);
                        continue;
                    }
                    else if (_typ == typeof(decimal))
                    {
                        dr[i] = reader.IsDBNull(i) ? 0 : reader.GetDecimal(i);
                        continue;
                    }
                    else if (_typ == typeof(int) || _typ == typeof(Int32))
                    {
                        dr[i] = reader.IsDBNull(i) ? 0 : reader.GetInt32(i);
                        continue;
                    }
                    else if (_typ == typeof(Int64))
                    {
                        dr[i] = reader.IsDBNull(i) ? 0 : reader.GetInt64(i);
                        continue;
                    }
                    else
                    {
                        dr[i] = reader.GetValue(i);
                        continue;
                    }
                }

                dt.Rows.Add(dr);
            }
        }

        private void AddColumns(EbDataTable dt, NpgsqlDataReader reader)
        {
            int pos = 0;
            foreach (NpgsqlDbColumn drow in reader.GetColumnSchema())
            {
                string columnName = System.Convert.ToString(drow["ColumnName"]);
                EbDataColumn column = new EbDataColumn(columnName, ConvertToDbType((Type)(drow["DataType"])));
                column.ColumnIndex = pos++;
                dt.Columns.Add(column);
            }
        }

        private DbType ConvertToDbType(Type _typ)
        {
            if (_typ == typeof(DateTime))
                return DbType.DateTime;
            else if (_typ == typeof(string))
                return DbType.String;
            else if (_typ == typeof(bool))
                return DbType.Boolean;
            else if (_typ == typeof(decimal))
                return DbType.Decimal;
            else if (_typ == typeof(int) || _typ == typeof(Int32))
                return DbType.Int32;
            else if (_typ == typeof(Int64))
                return DbType.Int64;

            return DbType.String;
        }

        public enum Operations
        {
            Create,
            Edit,
            PageSummary,
            TotalSummary,
            Filtering,
            Zooming,
            Graph,
            PDFExport,
            ExcelExport,
            CSVExport,
            CopyToClipboard,
            Print
        }
    }


    public class GOptions
    {
        public List<axis> Xaxis { get; set; }

        public List<axis> Yaxis { get; set; }

        public string type { get; set; }
    }
    public class axis
    {
        public string index { get; set; }

        public string name { get; set; }
    }
}
