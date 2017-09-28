using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using ExpressBase.Objects.ObjectContainers;
using Newtonsoft.Json;
using ServiceStack.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpressBase.Objects
{
    [EnableInBuilder(BuilderType.DataSource)]
    public class EbDatasourceMain : EbObject{

        [EnableInBuilder(BuilderType.DataSource)]
        new public string Name { get; set; }

        [EnableInBuilder(BuilderType.DataSource)]
        public string Description { get; set; }
    }

    [EnableInBuilder(BuilderType.DataSource)]
    public class EbDataSource : EbDatasourceMain
    {
        [ProtoBuf.ProtoMember(1)]
        public string Sql { get; set; }

        [EnableInBuilder(BuilderType.DataSource)]
        [PropertyEditor(PropertyEditorType.ObjectSelector)]
        [OSE_ObjectTypes(EbObjectType.FilterDialog)]
        public string FilterDialogRefId { get; set; }

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

            }
        }
    }

    [ProtoBuf.ProtoContract]
    public class EbSqlFunction : EbObject
    {
        [ProtoBuf.ProtoMember(1)]
        public string Sql { get; set; }

        [ProtoBuf.ProtoMember(2)]
        public int FilterDialogId { get; set; }
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
