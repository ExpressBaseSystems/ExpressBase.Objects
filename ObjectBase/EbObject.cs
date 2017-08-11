using ExpressBase.Objects.Attributes;
using ExpressBase.Objects.Objects;
using ServiceStack;
using ServiceStack.Redis;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace ExpressBase.Objects
{
    public class EbObject
    {
        public int Id { get; set; }

        public string RefId { get; set; }

        public EbObjectType EbObjectType { get; set; }

        [Description("Identity")]
        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog)]
        public virtual string Name { get; set; }

        public string ChangeLog { get; set; }

        public EbObject() { }

        public virtual void BeforeRedisSet() { }

        public virtual void AfterRedisGet(RedisClient Redis) { }

        public virtual void Init4Redis(IRedisClient redisclient, IServiceClient serviceclient) { }

        protected IRedisClient Redis { get; set; }

        protected IServiceClient ServiceStackClient { get; set; }
    }
}
