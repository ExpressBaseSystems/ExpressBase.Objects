using ExpressBase.Common.Objects;
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

        public string Title { get; set; }

        public int Left { get; set; }

        public int Width { get; set; }

        public int Top { get; set; }

        public int Height { get; set; }

        public HorizontalAlignment HAlign { get; set; }

        public VerticalAlignment VAlign { get; set; }

        public int DecimalPlaces { get; set; }

        public bool Sum { get; set; }
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

