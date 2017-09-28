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

    [EnableInBuilder(BuilderType.Report)]
    public class EbReport : EbReportObject
    {
        //public EbReportPaperSize PaperSize { get; set; }

        //public EbReportMargins Margins { get; set; }
        [EnableInBuilder(BuilderType.Report)]
        public string ReportName { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        public string Description { get; set; }

        [EnableInBuilder(BuilderType.Report)]
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
        public EbReportDetail Detail { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        [PropertyEditor(PropertyEditorType.ObjectSelector)]
        [OSE_ObjectTypes(EbObjectType.DataSource)]
        public string DataSourceRefId { get; set; }

        public ColumnColletion ColumnColletion { get; set; }

        public EbReport()
        {
            this.ReportHeaders = new List<EbReportHeader>();

            this.PageHeaders = new List<EbPageHeader>();

            this.Detail = new EbReportDetail();

            this.PageFooters = new List<EbPageFooter>();

            this.ReportFooters = new List<EbReportFooter>();
        }
    }
    public class EbReportSection : EbReportObject
    {        
        [EnableInBuilder(BuilderType.Report)]
        [UIproperty]
        public string SectionHeight { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        public List<EbReportFields> Fields { get; set; }

    }

    [EnableInBuilder(BuilderType.Report)]
    public class EbReportHeader : EbReportSection
    {

        public override string GetDesignHtml()
        {
            return "<div class='pageHeaders' eb-type='ReportHeader' id='@id' data_val='0' style='width :100%;position: relative'> </div>".RemoveCR().DoubleQuoted();
        }
    }

    [EnableInBuilder(BuilderType.Report)]
    public class EbPageHeader : EbReportSection
    {
        public override string GetDesignHtml()
        {
            return "<div class='pageHeaders' eb-type='PageHeader' id='@id' data_val='1' style='width :100%;position: relative'> </div>".RemoveCR().DoubleQuoted();
        }
    }

    [EnableInBuilder(BuilderType.Report)]
    public class EbReportDetail : EbReportSection
    {
        public override string GetDesignHtml()
        {
            return "<div class='pageHeaders' eb-type='ReportDetail' id='@id' data_val='2' style='width :100%;position: relative'> </div>".RemoveCR().DoubleQuoted();
        }
    }

    [EnableInBuilder(BuilderType.Report)]
    public class EbPageFooter : EbReportSection
    {
        public override string GetDesignHtml()
        {
            return "<div class='pageHeaders' eb-type='PageFooter' id='@id' data_val='3' style='width :100%;position: relative'> </div>".RemoveCR().DoubleQuoted();
        }
    }

    [EnableInBuilder(BuilderType.Report)]
    public class EbReportFooter : EbReportSection
    {
        public override string GetDesignHtml()
        {
            return "<div class='pageHeaders' eb-type='ReportFooter' id='@id' data_val='4' style='width :100%;position: relative'> </div>".RemoveCR().DoubleQuoted();
        }
    }
}
