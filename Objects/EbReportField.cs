using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExpressBase.Objects
{
    [ProtoBuf.ProtoContract]
    public class EbReportField : EbControl
    {
        [ProtoBuf.ProtoMember(1)]
        [System.ComponentModel.Category("Behavior")]
        public TextTransform TextTransform { get; set; }

        public EbReportField() { }
    }
}

