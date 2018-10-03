using ExpressBase.Common.JsonConverters;
using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using ExpressBase.Common.Structures;
using ExpressBase.Objects.EmailRelated;
using Newtonsoft.Json;
using ServiceStack.Redis;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExpressBase.Objects.Objects.SmsRelated
{
    [EnableInBuilder(BuilderType.SmsBuilder)]
    public class EbSmsTemplateBase : EbObject
    {

    }

    [EnableInBuilder(BuilderType.SmsBuilder)]
    public class EbSmsTemplate : EbSmsTemplateBase
    {
        [HideInPropertyGrid]
        public string To { get; set; }

        [EnableInBuilder(BuilderType.SmsBuilder)]
        [JsonConverter(typeof(Base64Converter))]
        [HideInPropertyGrid]
        public string Body { get; set; }

        [JsonIgnore]
        public EbDataSource EbDataSource { get; set; }

        [EnableInBuilder(BuilderType.SmsBuilder)]
        [PropertyEditor(PropertyEditorType.ObjectSelector)]
        [OSE_ObjectTypes(EbObjectTypes.iDataSource)]
        public string DataSourceRefId { get; set; }

        [EnableInBuilder(BuilderType.SmsBuilder)]
        [HideInPropertyGrid]
        public StringList Parameters { get; set; }

        [EnableInBuilder(BuilderType.SmsBuilder)]
        [HideInPropertyGrid]
        public List<DsColumns> DsColumnsCollection { get; set; }

        public override void AfterRedisGet(RedisClient Redis)
        {
            try
            {
                this.EbDataSource = Redis.Get<EbDataSource>(this.DataSourceRefId);
                this.EbDataSource.AfterRedisGet(Redis);
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.ToString());
            }
        }
    }
}
