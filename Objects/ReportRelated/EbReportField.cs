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
        public bool Sum { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        [UIproperty]
        public string ColVal { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        [UIproperty]
        public int Border { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        [UIproperty]
        [PropertyEditor(PropertyEditorType.Color)]
        public string BorderColor { get; set; }
    }

    [EnableInBuilder(BuilderType.Report)]
    public class EbReportCol : EbReportField
    {       

        public override string GetDesignHtml()
        {
            return "<div class='EbCol dropped' $type='@type' eb-type='ReportCol' id='@id' style='border: @Border px solid;border-color: @BorderColor ; width: @Width px; background-color:@BackColor ; color:@ForeColor ; height: @Height px; position: absolute; left: @Left px; top: @Top px;'> @ColVal </div>".RemoveCR().DoubleQuoted();
        }
        public override string GetJsInitFunc()
        {
            return @"
    this.Init = function(id)
        {
    this.Height =25;
    this.Width= 200;
    this.ForeColor = '#201c1c';
    this.Border = 1;
    this.BorderColor = '#aaaaaa'
};";
        }
    }

    [EnableInBuilder(BuilderType.Report)]
    public class EbImg : EbReportFieldShape
    {       
        public override string GetDesignHtml()
        {
            return "<div class='img-container dropped' eb-type='Img' id='@id' style='border:1px solid #aaaaaa; width: @Width px; background-color:@BackColor ; color:@ForeColor ; height: @Height px; position: absolute; left: @Left px; top: @Top px;'></div>".RemoveCR().DoubleQuoted();
        }
        public override string GetJsInitFunc()
        {
            return @"
    this.Init = function(id)
        {
    this.Height =100;
    this.Width= 100;
};";
        }
    }

    [EnableInBuilder(BuilderType.Report)]
    public class EbDateTime : EbReportFieldShape
    {
        [EnableInBuilder(BuilderType.Report)]
        [UIproperty]
        public string CurrentTime { get; set; }

        public override string GetDesignHtml()
        {
            return "<div class='date-time dropped' eb-type='DateTime' id='@id' style='border: @Border px solid;border-color: @BorderColor ; width: @Width px; height: @Height px; background-color:@BackColor ; color:@ForeColor ; position: absolute; left: @Left px; top: @Top px;'> @CurrentTime </div>".RemoveCR().DoubleQuoted();
        }
        public override string GetJsInitFunc()
        {
            return @"
    this.Init = function(id)
        {
     this.Height =25;
    this.Width= 200;
    this.ForeColor = '#201c1c';
    this.Border = 1;
    this.BorderColor = '#aaaaaa'
};";
        }
    }

    [EnableInBuilder(BuilderType.Report)]
    public class EbPageNo : EbReportFieldShape
    {       

        public override string GetDesignHtml()
        {
            return "<div class='page-no dropped' eb-type='PageNo' id='@id' style='border: @Border px solid;border-color: @BorderColor ; width: @Width px; height: @Height px; background-color:@BackColor ; color:@ForeColor ; position: absolute; left: @Left px; top: @Top px;'> @ColVal </div>".RemoveCR().DoubleQuoted();
        }
        public override string GetJsInitFunc()
        {
            return @"
    this.Init = function(id)
        {
     this.Height =25;
    this.Width= 100;
    this.ForeColor = '#201c1c';
    this.Border = 1;
    this.BorderColor = '#aaaaaa'
};";
        }
    }

    [EnableInBuilder(BuilderType.Report)]
    public class EbPageXY : EbReportFieldShape
    {

        public override string GetDesignHtml()
        {
            return "<div class='page-x/y dropped' eb-type='PageXY' id='@id' style='border: @Border px solid;border-color: @BorderColor ; width: @Width px; height: @Height px; background-color:@BackColor ; color:@ForeColor ; position: absolute; left: @Left px; top: @Top px;'> @ColVal </div>".RemoveCR().DoubleQuoted();
        }
        public override string GetJsInitFunc()
        {
            return @"
    this.Init = function(id)
        {
     this.Height =25;
    this.Width= 100;
    this.ForeColor = '#201c1c';
    this.Border = 1;
    this.BorderColor = '#aaaaaa'
};";
        }
    }

    [EnableInBuilder(BuilderType.Report)]
    public class UserName : EbReportFieldShape
    {

        public override string GetDesignHtml()
        {
            return "<div class='User-name dropped' eb-type='UserName' id='@id' style='border: @Border px solid;border-color: @BorderColor ; width: @Width px; height: @Height px; background-color:@BackColor ; color:@ForeColor ; position: absolute; left: @Left px; top: @Top px;'> @ColVal </div>".RemoveCR().DoubleQuoted();
        }
        public override string GetJsInitFunc()
        {
            return @"
    this.Init = function(id)
        {
     this.Height =25;
    this.Width= 100;
    this.ForeColor = '#201c1c';
    this.Border = 1;
    this.BorderColor = '#aaaaaa'
};";
        }
    }
}

