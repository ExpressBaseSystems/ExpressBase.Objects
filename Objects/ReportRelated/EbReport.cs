using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using ExpressBase.Objects.ReportRelated;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExpressBase.Objects.Objects.ReportRelated
{
    public class EbReport : EbReportField
    {     
    }
    [EnableInBuilder(BuilderType.Report)]
    public class EbReportHeader : EbReport
    {
        public override string GetDesignHtml()
        {
            return "<div class='EbCol dropped' eb-type='ReportCol' id='@id' style='border:1px solid black; width: @Width px;height: @Height px; position: relative; left: @Left px; top: @Top px;'> @ColVal </div>".RemoveCR().DoubleQuoted();
        }
    }
    [EnableInBuilder(BuilderType.Report)]
    public class EbPageHeader : EbReport
    {

    }
    [EnableInBuilder(BuilderType.Report)]
    public class EbDetail : EbReport
    {

    }
    [EnableInBuilder(BuilderType.Report)]
    public class EbPageFooter : EbReport
    {

    }
    [EnableInBuilder(BuilderType.Report)]
    public class EbReportFooter : EbReport
    {

    }
}
