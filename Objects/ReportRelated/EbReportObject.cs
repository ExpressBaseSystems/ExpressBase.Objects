using ExpressBase.Common.Constants;
using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExpressBase.Objects
{
    public class EbReportObject : EbObject
    {
        [EnableInBuilder(BuilderType.Report)]
        [HideInPropertyGrid]
        public string EbSid { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        [PropertyGroup("General")]
        [HideInPropertyGrid]
        public override string Name { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        [UIproperty]
        [PropertyGroup("General")]
        public virtual string Title { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        [PropertyGroup(PGConstants.APPEARANCE)]
        [UIproperty]
        public virtual string Left { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        [HideInPropertyGrid]
        public virtual float LeftPt { get; set; }

        [UIproperty]
        [EnableInBuilder(BuilderType.Report)]
        [PropertyGroup(PGConstants.APPEARANCE)]
        public virtual string Width { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        [HideInPropertyGrid]
        public virtual float WidthPt { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        [UIproperty]
        [PropertyGroup(PGConstants.APPEARANCE)]
        public virtual string Top { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        [HideInPropertyGrid]
        public virtual float TopPt { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        [UIproperty]
        [PropertyGroup(PGConstants.APPEARANCE)]
        public virtual string Height { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        [HideInPropertyGrid]
        public virtual float HeightPt { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        [UIproperty]
        [PropertyEditor(PropertyEditorType.Color)]
        [PropertyGroup(PGConstants.APPEARANCE)]
        public virtual string BackColor { get; set; }
    }
}
