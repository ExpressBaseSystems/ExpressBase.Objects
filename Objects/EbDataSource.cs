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
        public WebFormSchema FormSchema { set; get; }

        public EbSqlFunction() { }

        public EbSqlFunction(WebFormSchema data)
        {
            this.FormSchema = data;
            this.Sql = this.GenSqlFunc();
        }
        
        private string GenSqlFunc()
        {
            StringBuilder qry = new StringBuilder();
            qry.AppendFormat(SqlConstants.SQL_FUNC_HEADER, this.FormSchema.FormName, "'plpgsql'");
            qry.AppendLine();
            qry.Append(@" DECLARE 
temp_table jsonb;
_row jsonb;
BEGIN ");
            qry.AppendLine();
            for (int i = 0; i < this.FormSchema.Tables.Count; i++)
            {
                //insertquery
                qry.AppendLine();
                qry.AppendFormat(@"SELECT
    _table->'Rows' 
FROM 
    jsonb_array_elements(insert_json) _table 
INTO 
    temp_table 
WHERE 
    _table->>'TableName' = '{0}';", this.FormSchema.Tables[i].TableName);
                qry.AppendLine();
                qry.AppendFormat(@"FOR _row IN SELECT * FROM jsonb_array_elements(temp_table)
LOOP 
    {0} 
END LOOP;", GetExecuteQryI(this.FormSchema.Tables[i]));
                //update query
                qry.AppendLine();
                qry.AppendFormat(@"SELECT
    _table->'Rows' 
FROM 
    jsonb_array_elements(update_json) _table 
INTO 
    temp_table 
WHERE 
    _table->>'TableName' = '{0}';", this.FormSchema.Tables[i].TableName);
                qry.AppendLine();
                qry.AppendFormat(@"FOR _row IN SELECT * FROM jsonb_array_elements(temp_table)
LOOP 
    {0} 
END LOOP;", GetExecuteQryU(this.FormSchema.Tables[i]));
            }
            qry.AppendLine("\r\n$BODY$");
            return qry.ToString();
        }

        private string GetExecuteQryI(TableSchema _schema)
        {
            string qry = "EXECUTE 'INSERT INTO " + _schema.TableName + "(";
            string _using_clas = string.Empty;

            foreach (ColumSchema col in _schema.Colums)
            {
                if (!col.Equals(_schema.Colums.Last()))
                {
                    qry = qry + col.ColumName + CharConstants.COMMA;
                    _using_clas = _using_clas + "_row->>'" + col.ColumName + "'" + CharConstants.COMMA;
                }
                else
                {
                    qry = qry + col.ColumName + ") VALUES(";
                    _using_clas = _using_clas+ "_row->>'" + col.ColumName + "'" + CharConstants.SEMI_COLON;
                }
            }

            for (int i = 1; i <= _schema.Colums.Count; i++)
            {
                if (i != _schema.Colums.Count)
                    qry = qry + "$" + i + CharConstants.COMMA;
                else
                    qry = qry + "$" + i + ")'";
            }
            qry = qry + " USING " + _using_clas;
            return qry;
        }

        private string GetExecuteQryU(TableSchema _schema)
        {
            string qry = string.Format("EXECUTE 'UPDATE {0} SET ", _schema.TableName);
            string _using_clas = string.Empty;
            int _counter = 0;
            foreach (ColumSchema col in _schema.Colums)
            {
                _counter++;
                if (!col.Equals(_schema.Colums.Last()))
                {
                    qry = qry + col.ColumName + CharConstants.EQUALS +"$"+ _counter + CharConstants.COMMA;
                    _using_clas = _using_clas + "_row->>'" + col.ColumName + "'" + CharConstants.COMMA;
                }
                else
                {
                    qry = qry + col.ColumName + CharConstants.EQUALS + "$" + _counter + " WHERE id=$"+ _counter+";'";
                    _using_clas = _using_clas + "_row->>'" + col.ColumName + "'" + CharConstants.SEMI_COLON;
                }
            }
            qry = qry + " USING " + _using_clas;
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
