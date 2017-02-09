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
        public EbReportPaperSize PaperSize { get; set; }

        [ProtoBuf.ProtoMember(2)]
        public EbReportMargins Margins { get; set; }

        [ProtoBuf.ProtoMember(3)]
        public bool IsLandscape { get; set; }

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

    [ProtoBuf.ProtoContract]
    public class EbReportPaperSize
    {
        [ProtoBuf.ProtoMember(1)]
        public string Name { get; set; }

        [ProtoBuf.ProtoMember(2)]
        public float Width { get; set; }

        [ProtoBuf.ProtoMember(3)]
        public float Height { get; set; }

        public EbReportPaperSize(string name, float w, float h)
        {
            this.Name = name;
            this.Width = w;
            this.Height = h;
        }

        public override string ToString()
        {
            return this.Name;
        }
    }

    [ProtoBuf.ProtoContract]
    public class EbReportMargins
    {
        [ProtoBuf.ProtoMember(1)]
        public float Left { get; set; }

        [ProtoBuf.ProtoMember(2)]
        public float Right { get; set; }

        [ProtoBuf.ProtoMember(3)]
        public float Top { get; set; }

        [ProtoBuf.ProtoMember(4)]
        public float Bottom { get; set; }

        public EbReportMargins(float l, float r, float t, float b)
        {
            this.Left = l;
            this.Right = r;
            this.Top = t;
            this.Bottom = b;
        }
    }
}
