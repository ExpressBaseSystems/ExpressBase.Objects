using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace ExpressBase.Objects.ReportRelated
{
    public class EbReportField : EbObject
    {
        [EnableInBuilder(BuilderType.Report)]
        new public string Name { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        public string Title { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        public int Left { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        public int Width { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        public int Top { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        public int Height { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        public HorizontalAlignment HAlign { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        public VerticalAlignment VAlign { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        public string BackColor { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        public string ForeColor { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        public int DecimalPlaces { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        public bool Sum { get; set; }
    }

    [EnableInBuilder(BuilderType.Report)]
    public class EbReportFieldNumeric : EbReportField
    {
        [EnableInBuilder(BuilderType.Report)]
        public int MaxLength { get; set; }

        //[EnableInBuilder(BuilderType.Report)]
        //public int DecimalPlaces { get; set; }

        //[EnableInBuilder(BuilderType.Report)]
        //public bool Sum { get; set; }

        public override string GetDesignHtml()
        {
            return "";
        }
    }

    [EnableInBuilder(BuilderType.Report)]
    public class EbReportFieldText : EbReportField
    {
        [EnableInBuilder(BuilderType.Report)]
        public TextTransform TextTransform { get; set; }
    }
}

