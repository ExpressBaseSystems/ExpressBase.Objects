#if !NET462
using ExpressBase.Data;
#endif
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using ServiceStack.Redis;
using ExpressBase.Objects.Attributes;

namespace ExpressBase.Objects
{
    public enum EnumOperator
    {
        Equal,
        NotEqual,
        StartsWith,
        Contains,
        GreaterThan,
        GreaterThanOrEqual,
        LessThan,
        LessThanOrEqual
    }

    [ProtoBuf.ProtoContract]
    [ProtoBuf.ProtoInclude(3000, typeof(EbForm))]
    [ProtoBuf.ProtoInclude(3001, typeof(EbDataGridView))]
    [ProtoBuf.ProtoInclude(3002, typeof(EbTableLayout))]
    [ProtoBuf.ProtoInclude(3003, typeof(EbTableTd))]
    public class EbControlContainer : EbControl
    {
        [ProtoBuf.ProtoMember(1)]
        [EnableInBuilder(BuilderType.WebFormBuilder, BuilderType.FilterDialogBuilder)]
        [HideInPropertyGrid]
        public List<EbControl> Controls { get; set; }

        [ProtoBuf.ProtoMember(2)]
        [HideInPropertyGrid]
        public EbTable Table { get; set; }

        public EbControlContainer()
        {
            this.Controls = new List<EbControl>();
        }

        public override void Init4Redis(IRedisClient redisclient, ServiceStack.IServiceClient serviceclient)
        {
            base.Redis = redisclient;
            base.ServiceStackClient = serviceclient;
        }
    }
}
