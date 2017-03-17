using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace ExpressBase.Objects
{
    [ProtoBuf.ProtoContract]
    public class EbReportFieldText: EbReportField
    {
        [Browsable(false)]
        public object Parent { get; set; }

        [ProtoBuf.ProtoMember(1)]
        [System.ComponentModel.Category("Behavior")]
        public TextTransform TextTransform { get; set; }

        public EbReportFieldText() { }

        public EbReportFieldText(object parent)
        {
            this.Parent = parent;
        }
    }
}
