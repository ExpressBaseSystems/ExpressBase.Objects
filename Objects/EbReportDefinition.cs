using ExpressBase.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExpressBase.Objects
{
    [ProtoBuf.ProtoContract]
    public enum EbReportSectionType
    {
        ReportHeader,
        PageHeader,
        Detail,
        PageFooter,
        ReportFooter,
    }

    [ProtoBuf.ProtoContract]
    public class EbReportDefinition
    {
        [ProtoBuf.ProtoMember(1)]
        public EbReportPaperSize PaperSize { get; set; }

        [ProtoBuf.ProtoMember(2)]
        public EbReportMargins Margins { get; set; }

        [ProtoBuf.ProtoMember(3)]
        public bool IsLandscape { get; set; }

        [ProtoBuf.ProtoMember(4)]
        public EbReportSectionCollection ReportHeaders { get; set; }

        [ProtoBuf.ProtoMember(5)]
        public EbReportSectionCollection ReportFooters { get; set; }

        [ProtoBuf.ProtoMember(6)]
        public EbReportSectionCollection PageHeaders { get; set; }

        [ProtoBuf.ProtoMember(7)]
        public EbReportSectionCollection PageFooters { get; set; }

        [ProtoBuf.ProtoMember(8)]
        public EbReportSectionCollection Details { get; set; }

        [ProtoBuf.ProtoMember(9)]
        public int EbDataSourceId { get; set; }

        public ColumnColletion ColumnColletion { get; set; }

        public EbReportDefinition()
        {
            this.ReportHeaders = new EbReportSectionCollection(EbReportSectionType.ReportHeader);
            this.ReportHeaders.AddSection();

            this.PageHeaders = new EbReportSectionCollection(EbReportSectionType.PageHeader);
            this.PageHeaders.AddSection();

            this.Details = new EbReportSectionCollection(EbReportSectionType.Detail);
            this.Details.AddSection();

            this.PageFooters = new EbReportSectionCollection(EbReportSectionType.PageFooter);
            this.PageFooters.AddSection();

            this.ReportFooters = new EbReportSectionCollection(EbReportSectionType.ReportFooter);
            this.ReportFooters.AddSection();
        }
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

    [ProtoBuf.ProtoContract]
    public class EbReportSection : EbControlContainer, IComparable
    {
        public override string Name { get { return this.Prefix + this.Pos.ToString(); } }

        internal int Pos { get; set; }

        internal string Prefix { get; set; }

        [ProtoBuf.ProtoMember(1)]
        public EbReportSectionType Type { get; set; }

#if NET462
        public System.Windows.Forms.Panel Panel { get; set; }
#endif

        public EbReportSection(EbReportSectionType type)
        {
            this.Type = type;

            if (this.Type == EbReportSectionType.ReportHeader)
                this.Prefix = "RH";
            else if (this.Type == EbReportSectionType.PageHeader)
                this.Prefix = "PH";
            else if (this.Type == EbReportSectionType.Detail)
                this.Prefix = "DT";
            else if (this.Type == EbReportSectionType.PageFooter)
                this.Prefix = "PF";
            else if (this.Type == EbReportSectionType.ReportFooter)
                this.Prefix = "RF";
        }

        int IComparable.CompareTo(object obj)
        {
            return this.Pos.CompareTo((obj as EbReportSection).Pos);
        }
    }

    [ProtoBuf.ProtoContract]
    public class EbReportSectionCollection : List<EbReportSection>
    {
        [ProtoBuf.ProtoMember(1)]
        public EbReportSectionType Type { get; set; }

        public EbReportSectionCollection(EbReportSectionType type)
        {
            this.Type = type;
        }

        public void AddSection()
        {
            this.Add(GetNewSectionInstance(this.Count));
        }

        public void InsertAbove(EbReportSection section)
        {
            int idx = this.IndexOf(section);
            this.Insert(idx, GetNewSectionInstance(idx));
            for (int i = idx + 1; i < this.Count; i++)
                this[i].Pos++;
        }

        public void InsertBelow(EbReportSection section)
        {
            int idx = this.IndexOf(section) + 1;
            this.Insert(idx, GetNewSectionInstance(idx));
            for (int i = idx + 1; i < this.Count; i++)
                this[i].Pos++;
        }

        public void DeleteSection(EbReportSection section)
        {
            this.Remove(section);
            for (int i = 0; i < this.Count; i++)
                this[i].Pos = i;
        }

        private EbReportSection GetNewSectionInstance(int pos)
        {
            EbReportSection section = null;

            if (this.Type == EbReportSectionType.ReportHeader)
                section = new EbReportSection(this.Type) { Pos = pos };
            else if (this.Type == EbReportSectionType.PageHeader)
                section = new EbReportSection(this.Type) { Pos = pos };
            else if (this.Type == EbReportSectionType.Detail)
                section = new EbReportSection(this.Type) { Pos = pos };
            else if (this.Type == EbReportSectionType.PageFooter)
                section = new EbReportSection(this.Type) { Pos = pos };
            else if (this.Type == EbReportSectionType.ReportFooter)
                section = new EbReportSection(this.Type) { Pos = pos };

            return section;
        }
    }
}
