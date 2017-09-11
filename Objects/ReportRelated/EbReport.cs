
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
    }
    [EnableInBuilder(BuilderType.Report)]
    public class EbReportHeader : EbReportSections
    {
        public override string GetDesignHtml()
        {
            return "<div class='pageHeaders' eb-type='ReportHeader' $type='@type' id='rpthead' data_val='0' style='width :100%;position: relative'> </div>".RemoveCR().DoubleQuoted();
        }
    }
    [EnableInBuilder(BuilderType.Report)]
    public class EbPageHeader : EbReportSections
    {
        public override string GetDesignHtml()
        {
            return "<div class='pageHeaders' eb-type='PageHeader' $type='@type' id='pghead' data_val='1' style='width :100%;position: relative'> </div>".RemoveCR().DoubleQuoted();
        }
    }
    [EnableInBuilder(BuilderType.Report)]
    public class EbDetail : EbReportSections
    {
        public override string GetDesignHtml()
        {
            return "<div class='pageHeaders' eb-type='Detail' $type='@type' id='pgbody' data_val='2' style='width :100%;position: relative'> </div>".RemoveCR().DoubleQuoted();
        }
    }
    [EnableInBuilder(BuilderType.Report)]
    public class EbPageFooter : EbReportSections
    {
        public override string GetDesignHtml()
        {
            return "<div class='pageHeaders' eb-type='PageFooter' $type='@type' id='pgfooter' data_val='3' style='width :100%;position: relative'> </div>".RemoveCR().DoubleQuoted();
        }
    }
    [EnableInBuilder(BuilderType.Report)]
    public class EbReportFooter : EbReportSections
    {
        public override string GetDesignHtml()
        {
            return "<div class='pageHeaders' eb-type='ReportFooter' $type='@type' id='rptfooter' data_val='4' style='width :100%;position: relative'> </div>".RemoveCR().DoubleQuoted();
        }
    }
}
