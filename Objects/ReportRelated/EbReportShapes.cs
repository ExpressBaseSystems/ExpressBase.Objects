using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExpressBase.Objects.ReportRelated
{
    public abstract class EbReportShape : EbReportField
    {
    }

    [EnableInBuilder(BuilderType.Report)]
    public class EbReportLine : EbReportShape
    {

    }

    [EnableInBuilder(BuilderType.Report)]
    public class EbReportCircle : EbReportShape
    {

    }
}
