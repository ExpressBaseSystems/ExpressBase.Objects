
using ExpressBase.Common.Extensions;
using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using ExpressBase.Objects.ReportRelated;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExpressBase.Objects
{

    public class EbReportSections : EbReportField
    {
        [EnableInBuilder(BuilderType.Report)]
        public List<EbReportField> SubSection { get; set; }       
    }

    [EnableInBuilder(BuilderType.Report)]
    public class EbMultiSplitBox : EbReportSections
    {
        public override string GetDesignHtml()
        {
            return "<div class='multiSplitHbox' data_val='@data' eb-type='MultiSplitBox' id='@id' style='width: 100%;'></div>".RemoveCR().DoubleQuoted();
        }
    }

    [EnableInBuilder(BuilderType.Report)]
    public class EbMultiSplitBoxSub : EbReportSections
    {
        public override string GetDesignHtml()
        {
            return "<div class='multiSplitHboxSub' eb-type='MultiSplitBox' id='@id' style='width: 100%;'><p> @SubDivName </p></div>".RemoveCR().DoubleQuoted();
        }
    }

    [EnableInBuilder(BuilderType.Report)]
    public class EbReportPage : EbReportSections
    {
        [EnableInBuilder(BuilderType.Report)]
        public string PageSize { get; set; }

        public override string GetDesignHtml()
        {
            return "<div class='page' PageSize='@PageSize' eb-type='ReportPage' id='@id' style='width: @Width ;height: @Height '> </div>".RemoveCR().DoubleQuoted();
        }
    }

    [EnableInBuilder(BuilderType.Report)]
    public class EbReportHeader : EbReportSections
    {
        public override string GetDesignHtml()
        {
            return "<div class='pageHeaders' eb-type='ReportHeader' id='@id' data_val='0' style='width :100%;position: relative'> </div>".RemoveCR().DoubleQuoted();
        }
    }
    [EnableInBuilder(BuilderType.Report)]
    public class EbPageHeader : EbReportSections
    {
        public override string GetDesignHtml()
        {
            return "<div class='pageHeaders' eb-type='PageHeader' id='@id' data_val='1' style='width :100%;position: relative'> </div>".RemoveCR().DoubleQuoted();
        }
    }
    [EnableInBuilder(BuilderType.Report)]
    public class EbDetail : EbReportSections
    {
        public override string GetDesignHtml()
        {
            return "<div class='pageHeaders' eb-type='Detail' id='@id' data_val='2' style='width :100%;position: relative'> </div>".RemoveCR().DoubleQuoted();
        }
    }
    [EnableInBuilder(BuilderType.Report)]
    public class EbPageFooter : EbReportSections
    {
        public override string GetDesignHtml()
        {
            return "<div class='pageHeaders' eb-type='PageFooter' id='@id' data_val='3' style='width :100%;position: relative'> </div>".RemoveCR().DoubleQuoted();
        }
    }
    [EnableInBuilder(BuilderType.Report)]
    public class EbReportFooter : EbReportSections
    {
        public override string GetDesignHtml()
        {
            return "<div class='pageHeaders' eb-type='ReportFooter' id='@id' data_val='4' style='width :100%;position: relative'> </div>".RemoveCR().DoubleQuoted();
        }
    }
    [EnableInBuilder(BuilderType.Report)]
    public class EbSubSection : EbReportSections
    {
        public override string GetDesignHtml()
        {
            return "<div class='subdivs droppable' eb-type='SubSection' id='@id' style='width :100%;position: relative; height: @Height %;background-color:@BackColor '></div>".RemoveCR().DoubleQuoted();
        }       
    }
}
