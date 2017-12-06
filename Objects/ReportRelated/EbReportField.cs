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
    public abstract class EbReportField : EbReportObject
    {
        [EnableInBuilder(BuilderType.Report)]
        [UIproperty]
        [PropertyGroup("Appearance")]
        public int Border { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        [UIproperty]
        [PropertyEditor(PropertyEditorType.Color)]
        [PropertyGroup("Appearance")]
        public string BorderColor { get; set; }               
    }

    [EnableInBuilder(BuilderType.Report)]
    public class EbDataFieldText : EbReportField
    {       

        public override string GetDesignHtml()
        {
            return "<div class='EbCol dropped' $type='@type' eb-type='DataFieldText' id='@id' style='border: @Border px solid;border-color: @BorderColor ; width: @Width px; background-color:@BackColor ; color:@ForeColor ; height: @Height px; position: absolute; left: @Left px; top: @Top px;'> @Title </div>".RemoveCR().DoubleQuoted();
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
    public class EbDataFieldDateTime : EbReportField
    {

        public override string GetDesignHtml()
        {
            return "<div class='EbCol dropped' $type='@type' eb-type='DataFieldText' id='@id' style='border: @Border px solid;border-color: @BorderColor ; width: @Width px; background-color:@BackColor ; color:@ForeColor ; height: @Height px; position: absolute; left: @Left px; top: @Top px;'> @Title </div>".RemoveCR().DoubleQuoted();
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
    public class EbDataFieldBoolean : EbReportField
    {

        public override string GetDesignHtml()
        {
            return "<div class='EbCol dropped' $type='@type' eb-type='DataFieldText' id='@id' style='border: @Border px solid;border-color: @BorderColor ; width: @Width px; background-color:@BackColor ; color:@ForeColor ; height: @Height px; position: absolute; left: @Left px; top: @Top px;'> @Title </div>".RemoveCR().DoubleQuoted();
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
    public class EbDataFieldNumeric : EbReportField
    {

        public override string GetDesignHtml()
        {
            return "<div class='EbCol dropped' $type='@type' eb-type='DataFieldText' id='@id' style='border: @Border px solid;border-color: @BorderColor ; width: @Width px; background-color:@BackColor ; color:@ForeColor ; height: @Height px; position: absolute; left: @Left px; top: @Top px;'> @Title </div>".RemoveCR().DoubleQuoted();
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
    public class EbImg : EbReportField
    {
        [EnableInBuilder(BuilderType.Report)]
        [UIproperty]
        //[HideInPropertyGrid]
        public string Background { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        [PropertyEditor(PropertyEditorType.ImageSeletor)]
        public string Image { get; set; }

        public override string GetDesignHtml()
        {
            return "<div class='img-container dropped' eb-type='Img' id='@id' style='border:1px solid #aaaaaa; width: @Width px; background: @Background ; background-size: cover; height: @Height px; position: absolute; left: @Left px; top: @Top px;'></div>".RemoveCR().DoubleQuoted();
        }
        public override string GetJsInitFunc()
        {
            return @"
    this.Init = function(id)
        {
    this.Height =100;
    this.Width= 100;
    this.Background = 'url(../images/image.png) center no-repeat';
};";
        }
    }

    [EnableInBuilder(BuilderType.Report)]
    public class EbWaterMark : EbReportField
    {
        [EnableInBuilder(BuilderType.Report)]
        [UIproperty]
        //[HideInPropertyGrid]
        public string Background { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        [PropertyEditor(PropertyEditorType.ImageSeletor)]
        public string Image { get; set; }

        public override string GetDesignHtml()
        {
            return "<div class='wm-container dropped' eb-type='WaterMark' id='@id' style='opacity:.1; width: @Width px; background: @Background ; background-size: cover; height: @Height px; position: absolute; left: @Left px; top: @Top px;'></div>".RemoveCR().DoubleQuoted();
        }
        public override string GetJsInitFunc()
        {
            return @"
    this.Init = function(id)
        {
    this.Height =100;
    this.Width= 100;
    this.Background = 'url(../images/image.png) center no-repeat';
};";
        }
    }

    [EnableInBuilder(BuilderType.Report)]
    public class EbDateTime : EbReportField
    {      
        public override string GetDesignHtml()
        {
            return "<div class='date-time dropped' eb-type='DateTime' id='@id' style='border: @Border px solid;border-color: @BorderColor ; width: @Width px; height: @Height px; background-color:@BackColor ; color:@ForeColor ; position: absolute; left: @Left px; top: @Top px;'> @Title </div>".RemoveCR().DoubleQuoted();
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
    public class EbPageNo : EbReportField
    {       

        public override string GetDesignHtml()
        {
            return "<div class='page-no dropped' eb-type='PageNo' id='@id' style='border: @Border px solid;border-color: @BorderColor ; width: @Width px; height: @Height px; background-color:@BackColor ; color:@ForeColor ; position: absolute; left: @Left px; top: @Top px;'> @Title </div>".RemoveCR().DoubleQuoted();
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
    public class EbPageXY : EbReportField
    {

        public override string GetDesignHtml()
        {
            return "<div class='page-x/y dropped' eb-type='PageXY' id='@id' style='border: @Border px solid;border-color: @BorderColor ; width: @Width px; height: @Height px; background-color:@BackColor ; color:@ForeColor ; position: absolute; left: @Left px; top: @Top px;'> @Title </div>".RemoveCR().DoubleQuoted();
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
    public class UserName : EbReportField
    {

        public override string GetDesignHtml()
        {
            return "<div class='User-name dropped' eb-type='UserName' id='@id' style='border: @Border px solid;border-color: @BorderColor ; width: @Width px; height: @Height px; background-color:@BackColor ; color:@ForeColor ; position: absolute; left: @Left px; top: @Top px;'> @Title </div>".RemoveCR().DoubleQuoted();
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
    public class EbText : EbReportField
    {

        public override string GetDesignHtml()
        {
            return "<div class='Text-Field dropped' eb-type='Text' id='@id' style='border: @Border px solid;border-color: @BorderColor ; width: @Width px; height: @Height px; background-color:@BackColor ; color:@ForeColor ; position: absolute; left: @Left px; top: @Top px;'> @Title </div>".RemoveCR().DoubleQuoted();
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
    public class EbBarcode : EbReportField
    {
        [EnableInBuilder(BuilderType.Report)]
        [UIproperty]
        [PropertyGroup("Appearance")]
        public string Background { get; set; }

        public override string GetDesignHtml()
        {
            return "<div class='Bar-code dropped' eb-type='Barcode' id='@id' style=' border: @Border px solid;border-color: @BorderColor ; width: @Width px; height: @Height px; background: @Background ; position: absolute; left: @Left px; top: @Top px;background-size: 100% 100%;'></div>".RemoveCR().DoubleQuoted();
        }
        public override string GetJsInitFunc()
        {
            return @"
    this.Init = function(id)
        {
     this.Height =50;
    this.Width= 150;    
    this.Border = 1;
    this.BorderColor = '#aaaaaa'
    this.Background = 'url(../images/barcode.png) center no-repeat';
};";
        }
    }

    [EnableInBuilder(BuilderType.Report)]
    public class EbQRcode : EbReportField
    {
        [EnableInBuilder(BuilderType.Report)]
        [UIproperty]
        [PropertyGroup("Appearance")]
        public string Background { get; set; }

        public override string GetDesignHtml()
        {
            return "<div class='QRcode dropped' eb-type='QRcode' id='@id' style='border: @Border px solid;border-color: @BorderColor ; width: @Width px; height: @Height px; background: @Background ; position: absolute; left: @Left px; top: @Top px;background-size: 100% 100%;'></div>".RemoveCR().DoubleQuoted();
        }
        public override string GetJsInitFunc()
        {
            return @"
    this.Init = function(id)
        {
     this.Height =100;
    this.Width= 100;    
    this.Border = 1;
    this.BorderColor = '#aaaaaa';
    this.Background = 'url(../images/Qr-code.png) center no-repeat';
};";
        }
    }

    [EnableInBuilder(BuilderType.Report)]
    public class EbDataFieldSummary : EbReportField
    {
        [EnableInBuilder(BuilderType.Report)]
        [HideInPropertyGrid]       
        public string DataField { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        public SummaryFunctions Function { get; set; }

        public override string GetDesignHtml()
        {
            return "<div class='dropped' $type='@type' eb-type='DataFieldSummary' id='@id' style='border: @Border px solid;border-color: @BorderColor ; width: @Width px; background-color:@BackColor ; color:@ForeColor ; height: @Height px; position: absolute; left: @Left px; top: @Top px;'> @Title </div>".RemoveCR().DoubleQuoted();
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
}

