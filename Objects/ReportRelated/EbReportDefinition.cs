using ExpressBase.Common;
using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using ExpressBase.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExpressBase.Objects.ReportRelated
{
    public enum EbReportSectionType
    {
        ReportHeader,
        PageHeader,
        Detail,
        PageFooter,
        ReportFooter,
    }

    public class EbReport
    {
        //public EbReportPaperSize PaperSize { get; set; }

        //public EbReportMargins Margins { get; set; }

        public bool IsLandscape { get; set; }

        //public EbReportSectionCollection ReportHeaders { get; set; }

        //public EbReportSectionCollection ReportFooters { get; set; }

        //public EbReportSectionCollection PageHeaders { get; set; }

        //public EbReportSectionCollection PageFooters { get; set; }

        //public EbReportSectionCollection Details { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        [PropertyEditor(PropertyEditorType.ObjectSelector)]
        [OSE_ObjectTypes(EbObjectType.DataSource)]
        public string DataSourceRefId { get; set; }

        public ColumnColletion ColumnColletion { get; set; }

        public EbReport()
        {
            //this.ReportHeaders = new EbReportSectionCollection(EbReportSectionType.ReportHeader);
            //this.ReportHeaders.AddSection();

            //this.PageHeaders = new EbReportSectionCollection(EbReportSectionType.PageHeader);
            //this.PageHeaders.AddSection();

            //this.Details = new EbReportSectionCollection(EbReportSectionType.Detail);
            //this.Details.AddSection();

            //this.PageFooters = new EbReportSectionCollection(EbReportSectionType.PageFooter);
            //this.PageFooters.AddSection();

            //this.ReportFooters = new EbReportSectionCollection(EbReportSectionType.ReportFooter);
            //this.ReportFooters.AddSection();
        }
    }

    //public class EbReportPaperSize
    //{
    //    public string Name { get; set; }

    //    public float Width { get; set; }

    //    public float Height { get; set; }
    //}

    //public class EbReportMargins
    //{
    //    public float Left { get; set; }

    //    public float Right { get; set; }

    //    public float Top { get; set; }

    //    public float Bottom { get; set; }

    //    public EbReportMargins(float l, float r, float t, float b)
    //    {
    //        this.Left = l;
    //        this.Right = r;
    //        this.Top = t;
    //        this.Bottom = b;
    //    }
    //}

    //public class EbReportSection : IComparable
    //{
    //    public string Name { get { return this.Prefix + this.Pos.ToString(); } }

    //    public List<EbReportField> Fields { get; set; }

    //    internal int Pos { get; set; }

    //    internal string Prefix { get; set; }

    //    public EbReportSectionType Type { get; set; }

    //    public int Height { get; set; }

    //    public EbReportSection(EbReportSectionType type)
    //    {
    //        this.Type = type;

    //        if (this.Type == EbReportSectionType.ReportHeader)
    //            this.Prefix = "RH";
    //        else if (this.Type == EbReportSectionType.PageHeader)
    //            this.Prefix = "PH";
    //        else if (this.Type == EbReportSectionType.Detail)
    //            this.Prefix = "DT";
    //        else if (this.Type == EbReportSectionType.PageFooter)
    //            this.Prefix = "PF";
    //        else if (this.Type == EbReportSectionType.ReportFooter)
    //            this.Prefix = "RF";
    //    }

    //    int IComparable.CompareTo(object obj)
    //    {
    //        return this.Pos.CompareTo((obj as EbReportSection).Pos);
    //    }
    //}

    //public class EbReportSectionCollection : List<EbReportSection>
    //{
    //    public EbReportSectionType Type { get; set; }

    //    public EbReportSectionCollection(EbReportSectionType type)
    //    {
    //        this.Type = type;
    //    }

    //    public void AddSection()
    //    {
    //        this.Add(GetNewSectionInstance(this.Count));
    //    }

    //    public void InsertAbove(EbReportSection section)
    //    {
    //        int idx = this.IndexOf(section);
    //        this.Insert(idx, GetNewSectionInstance(idx));
    //        for (int i = idx + 1; i < this.Count; i++)
    //            this[i].Pos++;
    //    }

    //    public void InsertBelow(EbReportSection section)
    //    {
    //        int idx = this.IndexOf(section) + 1;
    //        this.Insert(idx, GetNewSectionInstance(idx));
    //        for (int i = idx + 1; i < this.Count; i++)
    //            this[i].Pos++;
    //    }

    //    public void DeleteSection(EbReportSection section)
    //    {
    //        this.Remove(section);
    //        for (int i = 0; i < this.Count; i++)
    //            this[i].Pos = i;
    //    }

    //    private EbReportSection GetNewSectionInstance(int pos)
    //    {
    //        EbReportSection section = null;

    //        if (this.Type == EbReportSectionType.ReportHeader)
    //            section = new EbReportSection(this.Type) { Pos = pos };
    //        else if (this.Type == EbReportSectionType.PageHeader)
    //            section = new EbReportSection(this.Type) { Pos = pos };
    //        else if (this.Type == EbReportSectionType.Detail)
    //            section = new EbReportSection(this.Type) { Pos = pos };
    //        else if (this.Type == EbReportSectionType.PageFooter)
    //            section = new EbReportSection(this.Type) { Pos = pos };
    //        else if (this.Type == EbReportSectionType.ReportFooter)
    //            section = new EbReportSection(this.Type) { Pos = pos };

    //        return section;
    //    }
    //}
}
