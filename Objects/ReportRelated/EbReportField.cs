using ExpressBase.Common.Extensions;
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
        public int TabIndex { get; set; }

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

        [EnableInBuilder(BuilderType.Report)]
        public int DecimalPlaces { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        [UIproperty]
        public string ColVal { get; set; }

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
    }

    [EnableInBuilder(BuilderType.Report)]
    public class EbReportFieldText : EbReportField
    {
        [EnableInBuilder(BuilderType.Report)]
        public TextTransform TextTransform { get; set; }
    }

    [EnableInBuilder(BuilderType.Report)]
    public class EbReportCol : EbReportField
    {
         public override string GetDesignHtml()
        {
            return "<div class='EbCol dropped' $type='@type' eb-type='ReportCol' id='@id' style='border:1px solid black; width: @Width px; background-color:@BackColor ; color:@ForeColor; height: @Height px; position: relative; left: @Left px; top: @Top px;'> @ColVal </div>".RemoveCR().DoubleQuoted();
        }
        public override string GetJsInitFunc()
        {
            return @"
    this.Init = function(id)
        {
    this.Height =25;
    this.Width= 200;
};";
        }
    }
   
}

