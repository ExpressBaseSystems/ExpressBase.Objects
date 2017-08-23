using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace ExpressBase.Objects.ReportRelated
{
    public class EbReportField
    {
        public string Name { get; set; }
    }

    public class EbReportFieldNumeric : EbReportField
    {
        public int MaxLength { get; set; }

        public int DecimalPlaces { get; set; }
    }

    public class EbReportFieldText : EbReportField
    {
        public TextTransform TextTransform { get; set; }
    }
}

