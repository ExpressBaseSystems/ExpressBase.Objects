using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExpressBase.Objects.ReportRelated
{
    public class EbReportObject : EbObject
    {
        public string EbSid { get; set; }

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
        public string Left { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        [PropertyGroup("Appearance")]
        [UIproperty]
        public float LeftPt { get; set; }

        [UIproperty]
        [EnableInBuilder(BuilderType.Report)]
        [PropertyGroup("Appearance")]
        public string Width { get; set; }

        [UIproperty]
        [EnableInBuilder(BuilderType.Report)]
        [PropertyGroup("Appearance")]
        public float WidthPt { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        [UIproperty]
        [PropertyGroup("Appearance")]
        public string Top { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        [UIproperty]
        [PropertyGroup("Appearance")]
        public float TopPt { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        [UIproperty]
        [PropertyGroup("Appearance")]
        public string Height { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        [UIproperty]
        [PropertyGroup("Appearance")]
        public float HeightPt { get; set; }

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
