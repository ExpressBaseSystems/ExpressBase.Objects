using ExpressBase.Common;
using ExpressBase.Common.Extensions;
using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using ExpressBase.Objects.ReportRelated;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExpressBase.Objects
{
    public enum EbReportSectionType
    {
        ReportHeader,
        PageHeader,
        Detail,
        PageFooter,
        ReportFooter,
    }
    public enum PaperSize
    {
        A2,
        A3,
        A4,
        A5,
        Letter,
        Custom
    }

    public enum SummaryFunctionsText
    {
        Count,
        Max,
        Min
    }
    public enum EbTextAlign
    {
        left,
        center,
        right,
        justify
    }
    public enum SummaryFunctionsNumeric
    {
        Average,
        Count,
        Max,
        Min,
        Sum
    }
    public enum SummaryFunctionsDateTime
    {
        Count,
        Max,
        Min
    }
    public enum SummaryFunctionsBoolean
    {
        Count
    }

    [EnableInBuilder(BuilderType.Report)]
    public class EbReport : EbReportObject
    {
        [EnableInBuilder(BuilderType.Report)]
        [OnChangeExec(@"
if (this.PaperSize === 6 ){  
     pg.ShowProperty('CustomPaperHeight');
     pg.ShowProperty('CustomPaperWidth');
}
else {
     pg.HideProperty('CustomPaperHeight');
     pg.HideProperty('CustomPaperWidth');
}
            ")]
        [PropertyGroup("General")]
        public PaperSize PaperSize { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        [PropertyGroup("General")]
        [UIproperty]
        public float CustomPaperHeight { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        [PropertyGroup("General")]
        [UIproperty]
        public float CustomPaperWidth { get; set; }

        //public EbReportMargins Margins { get; set; }
        //[EnableInBuilder(BuilderType.Report)]
        //[PropertyGroup("General")]
        //public string ReportName { get; set; }

        //[EnableInBuilder(BuilderType.Report)]
        //[PropertyGroup("General")]
        //public string Description { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        [PropertyGroup("General")]
        public bool IsLandscape { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        [HideInPropertyGrid]
        public List<EbReportHeader> ReportHeaders { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        [HideInPropertyGrid]
        public List<EbReportFooter> ReportFooters { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        [HideInPropertyGrid]
        public List<EbPageHeader> PageHeaders { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        [HideInPropertyGrid]
        public List<EbPageFooter> PageFooters { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        [HideInPropertyGrid]
        public List<EbReportDetail> Detail { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        [PropertyEditor(PropertyEditorType.ObjectSelector)]
        [OSE_ObjectTypes(EbObjectType.DataSource)]
        public string DataSourceRefId { get; set; }        

        public ColumnColletion ColumnColletion { get; set; }

        public int SerialNumber { get; set; } = 1;

        public EbReport()
        {
            this.ReportHeaders = new List<EbReportHeader>();

            this.PageHeaders = new List<EbPageHeader>();

            this.Detail = new List<EbReportDetail>();

            this.PageFooters = new List<EbPageFooter>();

            this.ReportFooters = new List<EbReportFooter>();
        }

        public enum Operations
        {
            Print
        }
    }
    public class EbReportSection : EbReportObject
    {        
        [EnableInBuilder(BuilderType.Report)]
        [UIproperty]
        public string SectionHeight { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        [HideInPropertyGrid]
        public List<EbReportField> Fields { get; set; }

    }

    [EnableInBuilder(BuilderType.Report)]
    public class EbReportHeader : EbReportSection
    {

        public override string GetDesignHtml()
        {
            return "<div class='pageHeaders' eb-type='ReportHeader' tabindex='1' id='@id' data_val='0' style='width :100%;height: @SectionHeight ; background-color:@BackColor ;position: relative'> </div>".RemoveCR().DoubleQuoted();
        }
    }

    [EnableInBuilder(BuilderType.Report)]
    public class EbPageHeader : EbReportSection
    {
        public override string GetDesignHtml()
        {
            return "<div class='pageHeaders' eb-type='PageHeader' tabindex='1' id='@id' data_val='1' style='width :100%;height: @SectionHeight ; background-color:@BackColor ;position: relative'> </div>".RemoveCR().DoubleQuoted();
        }
    }

    [EnableInBuilder(BuilderType.Report)]
    public class EbReportDetail : EbReportSection
    {
        public override string GetDesignHtml()
        {
            return "<div class='pageHeaders' eb-type='ReportDetail' tabindex='1' id='@id' data_val='2' style='width :100%;height: @SectionHeight ; background-color:@BackColor ;position: relative'> </div>".RemoveCR().DoubleQuoted();
        }

    }

    [EnableInBuilder(BuilderType.Report)]
    public class EbPageFooter : EbReportSection
    {
        public override string GetDesignHtml()
        {
            return "<div class='pageHeaders' eb-type='PageFooter' tabindex='1' id='@id' data_val='3' style='width :100%;height: @SectionHeight ; background-color:@BackColor ;position: relative'> </div>".RemoveCR().DoubleQuoted();
        }
    }

    [EnableInBuilder(BuilderType.Report)]
    public class EbReportFooter : EbReportSection
    {
        public override string GetDesignHtml()
        {
            return "<div class='pageHeaders' eb-type='ReportFooter' tabindex='1' id='@id' data_val='4' style='width :100%;height: @SectionHeight ; background-color:@BackColor ;position: relative'> </div>".RemoveCR().DoubleQuoted();
        }
    }
}
