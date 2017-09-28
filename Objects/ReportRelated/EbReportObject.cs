using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExpressBase.Objects.ReportRelated
{
    public class EbReportObject : EbObject
    {
        [EnableInBuilder(BuilderType.Report)]
        new public string Name { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        [UIproperty]
        public string Title { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        [UIproperty]
        public int Left { get; set; }

        [UIproperty]
        [EnableInBuilder(BuilderType.Report)]
        public int Width { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        [UIproperty]
        public int Top { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        [UIproperty]
        public int Height { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        public HorizontalAlignment HAlign { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        public VerticalAlignment VAlign { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        [UIproperty]
        [PropertyEditor(PropertyEditorType.Color)]
        public string BackColor { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        [UIproperty]
        [PropertyEditor(PropertyEditorType.Color)]
        public string ForeColor { get; set; }
    }
}
