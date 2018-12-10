using ExpressBase.Common;
using ExpressBase.Common.Constants;
using ExpressBase.Common.JsonConverters;
using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using ExpressBase.Common.Structures;
using ExpressBase.Objects.ServiceStack_Artifacts;
using Newtonsoft.Json;
using ServiceStack;
using ServiceStack.Redis;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpressBase.Objects
{

    public abstract class EbDataSourceMain : EbObject
    {

        [EnableInBuilder(BuilderType.DataReader, BuilderType.DataWriter, BuilderType.SqlFunctions)]
        [HideInPropertyGrid]
        [JsonConverter(typeof(Base64Converter))]
        public string Sql { get; set; }
    }

    [EnableInBuilder(BuilderType.DataReader)]
    public class EbDataReader : EbDataSourceMain
    {
        [EnableInBuilder(BuilderType.DataReader)]
        [PropertyEditor(PropertyEditorType.ObjectSelector)]
        [OSE_ObjectTypes(EbObjectTypes.iFilterDialog)]
        public string FilterDialogRefId { get; set; }

        //public string VersionNumber { get; set; }

        //public string Status { get; set; }

        [JsonIgnore]
        public EbFilterDialog FilterDialog { get; set; }

        public string SqlDecoded()
        {
            return Encoding.UTF8.GetString(Convert.FromBase64String(this.Sql));
        }

        public override void AfterRedisGet(RedisClient Redis)
        {
            try
            {
                this.FilterDialog = Redis.Get<EbFilterDialog>(this.FilterDialogRefId);
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception:" + e.ToString());
            }
        }

        public override void AfterRedisGet(RedisClient Redis, IServiceClient client)
        {
            try
            {
                this.FilterDialog = Redis.Get<EbFilterDialog>(this.FilterDialogRefId);
                if (this.FilterDialog == null && this.FilterDialogRefId != "")
                {
                    var result = client.Get<EbObjectParticularVersionResponse>(new EbObjectParticularVersionRequest { RefId = this.FilterDialogRefId });
                    this.FilterDialog = EbSerializers.Json_Deserialize(result.Data[0].Json);
                    Redis.Set<EbFilterDialog>(this.FilterDialogRefId, this.FilterDialog);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception:" + e.ToString());
            }
        }

        public override void ReplaceRefid(Dictionary<string, string> RefidMap)
        {
            if (!FilterDialogRefId.IsEmpty())
            {
                if (RefidMap.ContainsKey(FilterDialogRefId))
                    FilterDialogRefId = RefidMap[FilterDialogRefId];
                else
                    FilterDialogRefId = "failed-to-update-";
            }
        }

        public override string DiscoverRelatedRefids()
        {
            if (!FilterDialogRefId.IsEmpty())
            {
                EbFilterDialog fd = FilterDialog;
                if (fd is null)
                {
                    //  Console.WriteLine(this.RefId + "-->" + FilterDialogRefId);
                    return FilterDialogRefId;
                }
            }
            return "";
        }
    }

    [EnableInBuilder(BuilderType.DataWriter)]
    public class EbDataWriter : EbDataSourceMain
    {
        [EnableInBuilder(BuilderType.DataWriter)]
        [HideInPropertyGrid]
        List<InputParam> InputParams { get; set; }
    }

    [EnableInBuilder(BuilderType.SqlFunctions)]
    public class EbSqlFunction : EbDataSourceMain
    {
        [JsonIgnore]
        public WebformData FormData { set; get; }

        [JsonIgnore]
        public List<JsonTable> JsonColoumsInsert { get; set; }

        [JsonIgnore]
        public List<JsonTable> JsonColoumsUpdate { get; set; }

        public EbSqlFunction() { }

        public EbSqlFunction(WebformData data)
        {
            this.FormData = data;
            this.GenJsonColumns();
            this.Sql = this.GenSqlFunc();
        }

        private void GenJsonColumns()
        {
            this.JsonColoumsInsert = new List<JsonTable>();
            this.JsonColoumsUpdate = new List<JsonTable>();

            foreach (KeyValuePair<string, SingleTable> kp in this.FormData.MultipleTables)
            {
                List<JsonColVal> insertcols = new List<JsonColVal>();
                List<JsonColVal> updatecols = new List<JsonColVal>();

                foreach (SingleRow _row in kp.Value)
                {
                    JsonColVal jsoncols_ins = new JsonColVal();
                    JsonColVal jsoncols_upd = new JsonColVal();

                    if (_row.IsUpdate)
                        updatecols.Add(this.GetCols(jsoncols_upd, _row));
                    else
                        insertcols.Add(this.GetCols(jsoncols_ins, _row));
                }
                if (insertcols.Count > 0)
                {
                    this.JsonColoumsInsert.Add(new JsonTable
                    {
                        TableName = kp.Key,
                        Rows = insertcols
                    });
                }
                if (updatecols.Count > 0)
                {
                    this.JsonColoumsUpdate.Add(new JsonTable
                    {
                        TableName = kp.Key,
                        Rows = updatecols
                    });
                }
            }
        }

        private JsonColVal GetCols(JsonColVal col, SingleRow row)
        {
            foreach (SingleColumn _cols in row.Columns)
            {
                col.Add(_cols.Name, _cols.Value);
            }
            return col;
        }

        private string GenSqlFunc()
        {
            StringBuilder qry = new StringBuilder();
            qry.AppendFormat(SqlConstants.SQL_FUNC_HEADER, this.FormData.Name, "'plpgsql'");
            qry.Append(@" DECLARE 
temp_table jsonb;
_row jsonb;
BEGIN ");
            qry.AppendLine();
            for (int i = 0; i < this.JsonColoumsInsert.Count; i++)
            {
                qry.AppendLine();
                qry.AppendFormat(@"SELECT
_table->'Rows' 
FROM 
jsonb_array_elements(insert_json) _table 
INTO 
temp_table 
WHERE 
_table->>'TableName' = '{0}';", this.JsonColoumsInsert[i].TableName);
                qry.AppendLine();
                qry.AppendFormat(@" FOR _row IN SELECT * FROM jsonb_array_elements(temp_table)
LOOP 
{0} 
END LOOP;", GetExecuteQryI(this.JsonColoumsInsert[i]));
            }

            for(int k = 0; k < this.JsonColoumsUpdate.Count; k++)
            {
                qry.AppendLine();
                qry.AppendFormat(@"SELECT
_table->'Rows' 
FROM 
jsonb_array_elements(update_json) _table 
INTO 
temp_table 
WHERE 
_table->>'TableName' = '{0}';", this.JsonColoumsUpdate[k].TableName);
                qry.AppendLine();
                qry.AppendFormat(@" FOR _row IN SELECT * FROM jsonb_array_elements(temp_table)
LOOP 
{0} 
END LOOP;", GetExecuteQryU(this.JsonColoumsUpdate[k]));

            }
            qry.AppendLine("$BODY$");
            return qry.ToString();
        }

        private string GetExecuteQryI(JsonTable _rowCollection)
        {
            string qry = "EXECUTE 'INSERT INTO " + _rowCollection.TableName + "(";
            string _using_clas = string.Empty;
            foreach (KeyValuePair<string, dynamic> _pair in _rowCollection.Rows[0])
            {
                if (!_pair.Equals(_rowCollection.Rows[0].Last()))
                {
                    qry = qry + _pair.Key + CharConstants.COMMA;
                    _using_clas = _using_clas + "_row->>'" + _pair.Key + "'" + CharConstants.COMMA;
                }
                else
                {
                    qry = qry + _pair.Key + ") VALUES(";
                    _using_clas = _using_clas+ "_row->>'" + _pair.Key + "'" + CharConstants.SEMI_COLON;
                }
            }

            for (int i = 1; i <= _rowCollection.Rows[0].Count; i++)
            {
                if (i != _rowCollection.Rows[0].Count)
                    qry = qry + "$" + i + CharConstants.COMMA;
                else
                    qry = qry + "$" + i + ")'";
            }
            qry = qry + "USING " + _using_clas;
            return qry;
        }

        private string GetExecuteQryU(JsonTable _rowCollection)
        {
            string qry = string.Format("EXECUTE 'UPDATE {0} SET ", _rowCollection.TableName);
            string _using_clas = string.Empty;
            int _counter = 1;
            foreach (KeyValuePair<string, dynamic> _pair in _rowCollection.Rows[0])
            {
                _counter = _counter++;
                if (!_pair.Equals(_rowCollection.Rows[0].Last()))
                {
                    qry = qry + _pair.Key + CharConstants.EQUALS +"$"+ _counter + CharConstants.COMMA;
                    _using_clas = _using_clas + "_row->>'" + _pair.Key + "'" + CharConstants.COMMA;
                }
                else
                {
                    qry = qry + _pair.Key + CharConstants.EQUALS + "$" + _counter + "WHERE id=$"+ _counter+";'";
                    _using_clas = _using_clas + "_row->>'" + _pair.Key + "'" + CharConstants.SEMI_COLON;
                }
            }
            qry = qry + "USING " + _using_clas;
            return qry;
        }
    }

    [ProtoBuf.ProtoContract]
    public class EbJsFunction : EbObject
    {
        [ProtoBuf.ProtoMember(1)]
        public string JsCode { get; set; }
    }

    [ProtoBuf.ProtoContract]
    public class EbJsValidator : EbObject
    {
        [ProtoBuf.ProtoMember(1)]
        public string JsCode { get; set; }
    }
}
