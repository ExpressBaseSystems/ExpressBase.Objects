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
        [PropertyGroup("General")]
        new public string Name { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        [UIproperty]
        [PropertyGroup("General")]
        public string Title { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        [PropertyGroup("Appearance")]
        [UIproperty]
        public decimal Left { get; set; }

        [UIproperty]
        [EnableInBuilder(BuilderType.Report)]
        [PropertyGroup("Appearance")]
        public int Width { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        [UIproperty]
        [PropertyGroup("Appearance")]
        public decimal Top { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        [UIproperty]
        [PropertyGroup("Appearance")]
        public int Height { get; set; }

        //[EnableInBuilder(BuilderType.Report)]
        //public HorizontalAlignment HAlign { get; set; }

        //[EnableInBuilder(BuilderType.Report)]
        //public VerticalAlignment VAlign { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        [UIproperty]
        [PropertyEditor(PropertyEditorType.Color)]
        [PropertyGroup("Appearance")]
        public string BackColor { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        [UIproperty]
        [PropertyEditor(PropertyEditorType.Color)]
        [PropertyGroup("Appearance")]
        public string ForeColor { get; set; }        
    }
}
