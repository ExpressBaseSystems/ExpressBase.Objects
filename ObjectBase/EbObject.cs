using ServiceStack;
using ServiceStack.Redis;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace ExpressBase.Objects
{
    [ProtoBuf.ProtoContract]
    [ProtoBuf.ProtoInclude(1000, typeof(EbControl))]
    [ProtoBuf.ProtoInclude(1001, typeof(EbDataSource))]
    [ProtoBuf.ProtoInclude(1002, typeof(EbReportDefinition))]
    [ProtoBuf.ProtoInclude(1003, typeof(EbSqlFunction))]
    [ProtoBuf.ProtoInclude(1004, typeof(EbJsFunction))]
    [ProtoBuf.ProtoInclude(1005, typeof(EbJsValidator))]
    [ProtoBuf.ProtoInclude(1006, typeof(EbDataVisualization))]
    [ProtoBuf.ProtoInclude(1007, typeof(EbFilterDialog))]
    
#if NET462
    [System.Serializable]
#endif
    public class EbObject
    {
        [Browsable(false)]
        public int Id { get; set; }

        [ProtoBuf.ProtoMember(1)]
        [Browsable(false)]
        public EbObjectType EbObjectType { get; set; }

        [ProtoBuf.ProtoMember(2)]
        [Browsable(false)]
        public string TargetType { get; set; }

        [ProtoBuf.ProtoMember(3)]
        [Description("Identity")]
        public virtual string Name { get; set; }

        [ProtoBuf.ProtoMember(4)]
        [Description("Identity")]
        public virtual string Description { get; set; }

        [ProtoBuf.ProtoMember(5)]
        public string ChangeLog { get; set; }

        public EbObject() { }

        public virtual void Init4Redis(IRedisClient redisclient, IServiceClient serviceclient) { }

        protected IRedisClient Redis { get; set; }

        protected IServiceClient ServiceStackClient { get; set; }
    }
}
