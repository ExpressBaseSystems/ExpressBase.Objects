using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExpressBase.Objects
{
    [ProtoBuf.ProtoContract]
    public class EbReportDefinition
    {
        [ProtoBuf.ProtoMember(1)]
        public EbControl ReportHeader { get; set; }

        [ProtoBuf.ProtoMember(2)]
        public EbControl PageHeader { get; set; }

        [ProtoBuf.ProtoMember(3)]
        public EbControl Details { get; set; }

        [ProtoBuf.ProtoMember(4)]
        public EbControl PageFooter { get; set; }

        [ProtoBuf.ProtoMember(5)]
        public EbControl ReportFooter { get; set; }
    }
}
