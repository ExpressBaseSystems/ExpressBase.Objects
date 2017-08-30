using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExpressBase.Objects.ReportRelated
{
    public abstract class EbReportFieldShape : EbReportField
    {
        
    }

    [EnableInBuilder(BuilderType.Report)]
    public class EbCircle : EbReportFieldShape
    {
        public override string GetDesignHtml()
        {
            return "<div class='circle' id='@id' style='border-radius: 50%; border: 1px solid; width: 50px; height: 50px; position: absolute; left: @leftpx; top: @toppx;'></div>".RemoveCR().DoubleQuoted();
        }
    }
    [EnableInBuilder(BuilderType.Report)]
    public class EbRect : EbReportFieldShape
    {

    }
    [EnableInBuilder(BuilderType.Report)]
    public class EbArrR : EbReportFieldShape
    {

    }
    [EnableInBuilder(BuilderType.Report)]
    public class EbArrL : EbReportFieldShape
    {

    }
    [EnableInBuilder(BuilderType.Report)]
    public class EbArrD : EbReportFieldShape
    {

    }
    [EnableInBuilder(BuilderType.Report)]
    public class ByArrH : EbReportFieldShape
    {

    }

    public class ByArrV : EbReportFieldShape
    {

    }
    public class EbHl : EbReportFieldShape
    {

    }
    public class EbVl : EbReportFieldShape
    {

    }


}
