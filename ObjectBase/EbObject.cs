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

        public EbObject() { }
    }
}
