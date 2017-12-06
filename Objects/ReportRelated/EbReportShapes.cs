using ExpressBase.Common.Extensions;
using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExpressBase.Objects.ReportRelated
{
    public abstract class EbReportFieldShape : EbReportField
    {        
    }

    [EnableInBuilder(BuilderType.Report)]
    public class EbCircle : EbReportFieldShape
    {
        public override string GetDesignHtml()
        {
            return "<div class='circle Ebshapes dropped' eb-type='Circle' id='@id' style='border-radius: 50%; border: @Border px solid; border-color: @BorderColor ; background-color:@BackColor ; width: @Width px; height: @Height px; position: absolute; left: @Left px; top: @Top px;'></div>".RemoveCR().DoubleQuoted();
        }

        public override string GetJsInitFunc()
        {
            return @"
    this.Init = function(id)
        {
    this.Height =50;
    this.Width= 50;
    this.Border = 1;
    this.BorderColor = '#000000'
};";
        }
    }
    [EnableInBuilder(BuilderType.Report)]
    public class EbRect : EbReportFieldShape
    {
        public override string GetDesignHtml()
        {
            return "<div class='rectangle Ebshapes dropped' eb-type='Rect' id='@id' style='border: @Border px solid; border-color: @BorderColor ;background-color:@BackColor ; color:@ForeColor ;width: @Width px; height: @Height px; position: absolute; left: @Left px; top: @Top px;'></div>".RemoveCR().DoubleQuoted();
        }
        public override string GetJsInitFunc()
        {
            return @"
    this.Init = function(id)
        {
    this.Height =50;
    this.Width= 50;
    this.Border = 1;
    this.BorderColor = '#000000'
};";
        }
    }
    [EnableInBuilder(BuilderType.Report)]
    public class EbArrR : EbReportFieldShape
    {
        public override string GetDesignHtml()
        {
            return "<div class='Ebshapes shape-horrizontal dropped' eb-type='ArrR' id='@id' style='border: @Border px solid; border-color: @BorderColor ; background-color:@BackColor ; color:@ForeColor ;width: @Width px; height: @Height px; position: absolute; left: @Left px; top: @Top px;'><div class='arrow-right'></div></div>".RemoveCR().DoubleQuoted();
        }
        public override string GetJsInitFunc()
        {
            return @"
    this.Init = function(id)
        {
    this.Height =1;
    this.Width= 50;
    this.Border = 1;
    this.BorderColor = '#000000'
};";
        }
    }
    [EnableInBuilder(BuilderType.Report)]
    public class EbArrL : EbReportFieldShape
    {
        public override string GetDesignHtml()
        {
            return "<div class='arrow Ebshapes shape-horrizontal dropped' eb-type='ArrL' id='@id' style='border: @Border px solid; border-color: @BorderColor ; background-color:@BackColor ; color:@ForeColor ; width: @Width px; height: @Height px; position: absolute; left: @Left px; top: @Top px;'><div class='arrow-left'></div></div>".RemoveCR().DoubleQuoted();
        }
        public override string GetJsInitFunc()
        {
            return @"
    this.Init = function(id)
        {
     this.Height =1;
    this.Width= 50;
    this.Border = 1;
    this.BorderColor = '#000000'
};";
        }
    }
    [EnableInBuilder(BuilderType.Report)]
    public class EbArrD : EbReportFieldShape
    {
        public override string GetDesignHtml()
        {
            return "<div class='Ebshapes shape-vertical dropped' eb-type='ArrD' id='@id' style='border: @Border px solid; border-color: @BorderColor ; background-color:@BackColor ; color:@ForeColor ; width: @Width px; height: @Height px; position: absolute; left: @Left px; top: @Top px;'><div class='arrow-down'></div></div>".RemoveCR().DoubleQuoted();
        }
        public override string GetJsInitFunc()
        {
            return @"
    this.Init = function(id)
        {
    this.Height =50;
    this.Width= 1;
    this.Border = 1;
    this.BorderColor = '#000000'
};";
        }
    }
    [EnableInBuilder(BuilderType.Report)]
    public class EbArrU : EbReportFieldShape
    {
        public override string GetDesignHtml()
        {
            return "<div class='Ebshapes shape-vertical dropped' $type='@type'eb-type='ArrU' id='@id' style='border: @Border px solid; border-color: @BorderColor ; background-color:@BackColor ; color:@ForeColor ; width: @Width px; height: @Height px; 50px; position: absolute; left: @Left px; top: @Top px;'><div class='arrow-up'></div></div>".RemoveCR().DoubleQuoted();
        }
        public override string GetJsInitFunc()
        {
            return @"
    this.Init = function(id)
        {
    this.Height =50;
    this.Width= 1;
    this.Border = 1;
    this.BorderColor = '#000000'
};";
        }
    }

    [EnableInBuilder(BuilderType.Report)]
    public class EbByArrH : EbReportFieldShape
    {
        public override string GetDesignHtml()
        {
            return "<div class='Ebshapes shape-horrizontal dropped' eb-type='ByArrH' id='@id' style='border: @Border px solid; border-color: @BorderColor ; background-color:@BackColor ; color:@ForeColor ; width: @Width px; height: @Height px; absolute: relative; left: @Left px; top: @Top px;'><div class='arrow-left'></div><div class='arrow-right'></div></div>".RemoveCR().DoubleQuoted();
        }
        public override string GetJsInitFunc()
        {
            return @"
    this.Init = function(id)
        {
    this.Height =1;
    this.Width= 50;
    this.Border = 1;
    this.BorderColor = '#000000'
};";
        }
    }
    [EnableInBuilder(BuilderType.Report)]
    public class EbByArrV : EbReportFieldShape
    {
        public override string GetDesignHtml()
        {
            return "<div class='Ebshapes shape-vertical dropped' eb-type='ByArrV' id='@id' style='border: @Border px solid; border-color: @BorderColor ; background-color:@BackColor ; color:@ForeColor ; width: @Width px; height: @Height px; absolute: relative; left: @Left px; top: @Top px;'><div class='arrow-up'></div><div class='arrow-down'></div></div>".RemoveCR().DoubleQuoted();
        }
        public override string GetJsInitFunc()
        {
            return @"
    this.Init = function(id)
        {
    this.Height =50;
    this.Width= 1;
    this.Border = 1;
    this.BorderColor = '#000000'
};";
        }
    }

    [EnableInBuilder(BuilderType.Report)]
    public class EbHl : EbReportFieldShape
    {
        public override string GetDesignHtml()
        {
            return "<div class='Ebshapes shape-horrizontal dropped' eb-type='Hl' id='@id' style='border: @Border px solid; border-color: @BorderColor ; background-color:@BackColor ; color:@ForeColor ; width: @Width px; height: @Height px; position: absolute; left: @Left px; top: @Top px;'></div>".RemoveCR().DoubleQuoted();
        }
        public override string GetJsInitFunc()
        {
            return @"
    this.Init = function(id)
        {
    this.Height =1;
    this.Width= 50;
    this.Border = 1;
    this.BorderColor = '#000000'
};";
        }
    }

    [EnableInBuilder(BuilderType.Report)]
    public class EbVl : EbReportFieldShape
    {
        public override string GetDesignHtml()
        {
            return "<div class='Ebshapes shape-vertical dropped' eb-type='Vl' id='@id' style='border: @Border px solid; border-color: @BorderColor ; background-color:@BackColor ; color:@ForeColor ; width: @Width px; height: @Height px; position: absolute; left: @Left px; top: @Top px;'></div>".RemoveCR().DoubleQuoted();
        }
        public override string GetJsInitFunc()
        {
            return @"
    this.Init = function(id)
        {
    this.Height =50;
    this.Width= 1;
    this.Border = 1;
    this.BorderColor = '#000000'
};";
        }
    }
    [EnableInBuilder(BuilderType.Report)]
    public class EbTable : EbReportFieldShape
    {
        [EnableInBuilder(BuilderType.Report)]
        public int ColoumNo { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        public int RowNo { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        [UIproperty]
        public string TableRowCol { get; set; }

        public override string GetDesignHtml()
        {
            return "<table class='table Ebshapes table-bordered dropped' eb-type='Table' id='@id' style='border: @Border px solid; border-color: @BorderColor ; background-color:@BackColor ; color:@ForeColor ; width: @Width px; height: @Height px; position: absolute; left: @Left px; top: @Top px;'> @TableRowCol </table>".RemoveCR().DoubleQuoted();
        }
        public override string GetJsInitFunc()
        {
            return @"
    this.Init = function(id)
        {
    this.Height =100;
    this.Width= 100;
    this.Border = 1;
    this.BorderColor = '#000000'
    this.TableRowCol= '<tr><td></td><td></td></tr><tr><td></td><td></td></tr>';
};";
        }
    }
}
