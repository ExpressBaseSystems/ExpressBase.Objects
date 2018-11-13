using ExpressBase.Common;
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

    public abstract class EbDataSourceMain : EbObject {

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
            return "" ;
        }
    }

    [EnableInBuilder(BuilderType.DataWriter)]
    public class EbDataWriter: EbDataSourceMain
    {
        [EnableInBuilder(BuilderType.DataWriter)]
        [HideInPropertyGrid]
        List<InputParam> InputParams { get; set; }
    }

    [EnableInBuilder(BuilderType.SqlFunctions)]
    public class EbSqlFunction : EbDataSourceMain
    {
        
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
