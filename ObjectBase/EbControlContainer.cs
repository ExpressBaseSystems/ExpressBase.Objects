using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace ExpressBase.Objects
{
    [ProtoBuf.ProtoContract]
    [ProtoBuf.ProtoInclude(3000, typeof(EbForm))]
    [ProtoBuf.ProtoInclude(3001, typeof(EbDataGridView))]
    [ProtoBuf.ProtoInclude(3002, typeof(EbTableLayout))]
    public class EbControlContainer : EbControl
    {
        [ProtoBuf.ProtoMember(1)]
        [Browsable(false)]
        public List<EbControl> Controls { get; set; }

        public EbControlContainer() { }
    }
}
