using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace ExpressBase.Objects
{
    [ProtoBuf.ProtoContract]
    public class EbReportFieldNumeric : EbReportField
    {
        [Browsable(false)]
        public object Parent { get; set; }

        [ProtoBuf.ProtoMember(1)]
        [System.ComponentModel.Category("Behavior")]
        public int MaxLength { get; set; }

        [ProtoBuf.ProtoMember(2)]
        [System.ComponentModel.Category("Behavior")]
        public int DecimalPlaces { get; set; }

        public EbReportFieldNumeric() { }

        public EbReportFieldNumeric(object parent)
        {
            this.Parent = parent;
        }
    }
}

