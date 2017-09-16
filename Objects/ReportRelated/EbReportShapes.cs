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

        [EnableInBuilder(BuilderType.Report)]
        public int ColoumNo { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        public int RowNo { get; set; }
    }

    [EnableInBuilder(BuilderType.Report)]
    public class EbCircle : EbReportFieldShape
    {
        public override string GetDesignHtml()
        {
            return "<div class='circle dropped' eb-type='Circle' id='@id' style='border-radius: 50%; background-color:@BackColor ; color:@ForeColor ; border: 1px solid black; width: @Width px; height: @Height px; position: relative; left: @Left px; top: @Top px;'></div>".RemoveCR().DoubleQuoted();
        }

        public override string GetJsInitFunc()
        {
            return @"
    this.Init = function(id)
        {
    this.Height =50;
    this.Width= 50;
};";
        }
    }
    [EnableInBuilder(BuilderType.Report)]
    public class EbRect : EbReportFieldShape
    {
        public override string GetDesignHtml()
        {
            return "<div class='rectangle dropped' eb-type='Rect' id='@id' style='border: 1px solid black; width: @Width px; height: @Height px; position: relative; left: @Left px; top: @Top px;'></div>".RemoveCR().DoubleQuoted();
        }
        public override string GetJsInitFunc()
        {
            return @"
    this.Init = function(id)
        {
    this.Height =50;
    this.Width= 50;
};";
        }
    }
    [EnableInBuilder(BuilderType.Report)]
    public class EbArrR : EbReportFieldShape
    {
        public override string GetDesignHtml()
        {
            return "<div class='arrow arrow-r-draggable dropped' eb-type='ArrR' id='@id' style='border: 1px solid black; width: @Width px; height: @Height px; position: relative; left: @Left px; top: @Top px;'><div class='arrow-right'></div></div>".RemoveCR().DoubleQuoted();
        }
        public override string GetJsInitFunc()
        {
            return @"
    this.Init = function(id)
        {
    this.Height =1;
    this.Width= 50;
};";
        }
    }
    [EnableInBuilder(BuilderType.Report)]
    public class EbArrL : EbReportFieldShape
    {
        public override string GetDesignHtml()
        {
            return "<div class='arrow arrow-l-draggable dropped' eb-type='ArrL' id='@id' style='border: 1px solid black; width: @Width px; height: @Height px; position: relative; left: @Left px; top: @Top px;'><div class='arrow-left'></div></div>".RemoveCR().DoubleQuoted();
        }
        public override string GetJsInitFunc()
        {
            return @"
    this.Init = function(id)
        {
     this.Height =1;
    this.Width= 50;
};";
        }
    }
    [EnableInBuilder(BuilderType.Report)]
    public class EbArrD : EbReportFieldShape
    {
        public override string GetDesignHtml()
        {
            return "<div class='arrow arrow-d-draggable dropped' eb-type='ArrD' id='@id' style='border: 1px solid black; width: @Width px; height: @Height px; position: relative; left: @Left px; top: @Top px;'><div class='arrow-down'></div></div>".RemoveCR().DoubleQuoted();
        }
        public override string GetJsInitFunc()
        {
            return @"
    this.Init = function(id)
        {
    this.Height =50;
    this.Width= 1;
};";
        }
    }
    [EnableInBuilder(BuilderType.Report)]
    public class EbArrU : EbReportFieldShape
    {
        public override string GetDesignHtml()
        {
            return "<div class='arrow arrow-u-draggable dropped' $type='@type'eb-type='ArrU' id='@id' style='border: 1px solid black; width: @Width px; height: @Height px; 50px; position: relative; left: @Left px; top: @Top px;'><div class='arrow-up'></div></div>".RemoveCR().DoubleQuoted();
        }
        public override string GetJsInitFunc()
        {
            return @"
    this.Init = function(id)
        {
    this.Height =50;
    this.Width= 1;
};";
        }
    }

    [EnableInBuilder(BuilderType.Report)]
    public class EbByArrH : EbReportFieldShape
    {
        public override string GetDesignHtml()
        {
            return "<div class='arrow arrow-by-d-h-draggable dropped' eb-type='ByArrH' id='@id' style='border: 1px solid black; width: @Width px; height: @Height px; position: relative; left: @Left px; top: @Top px;'><div class='arrow-By-dir-h'></div></div>".RemoveCR().DoubleQuoted();
        }
        public override string GetJsInitFunc()
        {
            return @"
    this.Init = function(id)
        {
    this.Height =1;
    this.Width= 50;
};";
        }
    }
    [EnableInBuilder(BuilderType.Report)]
    public class EbByArrV : EbReportFieldShape
    {
        public override string GetDesignHtml()
        {
            return "<div class='arrow arrow-by-d-v-draggable dropped' eb-type='ByArrV' id='@id' style='border: 1px solid black; width: @Width px; height: @Height px; position: relative; left: @Left px; top: @Top px;'><div class='arrow-By-dir-v-b'></div></div>".RemoveCR().DoubleQuoted();
        }
        public override string GetJsInitFunc()
        {
            return @"
    this.Init = function(id)
        {
    this.Height =50;
    this.Width= 1;
};";
        }
    }

    [EnableInBuilder(BuilderType.Report)]
    public class EbHl : EbReportFieldShape
    {
        public override string GetDesignHtml()
        {
            return "<div class='h-line h-line-dropped dropped' eb-type='Hl' id='@id' style='border: 1px solid black; width: @Width px; height: @Height px; position: relative; left: @Left px; top: @Top px;'></div>".RemoveCR().DoubleQuoted();
        }
        public override string GetJsInitFunc()
        {
            return @"
    this.Init = function(id)
        {
    this.Height =1;
    this.Width= 50;
};";
        }
    }

    [EnableInBuilder(BuilderType.Report)]
    public class EbVl : EbReportFieldShape
    {
        public override string GetDesignHtml()
        {
            return "<div class='v-line v-line-dropped dropped' eb-type='Vl' id='@id' style='border: 1px solid black; width: @Width px; height: @Height px; position: relative; left: @Left px; top: @Top px;'></div>".RemoveCR().DoubleQuoted();
        }
        public override string GetJsInitFunc()
        {
            return @"
    this.Init = function(id)
        {
    this.Height =50;
    this.Width= 1;
};";
        }
    }
    [EnableInBuilder(BuilderType.Report)]
    public class EbTable : EbReportFieldShape
    {
        public override string GetDesignHtml()
        {
            return "<table class='table table-bordered dropped' eb-type='Table' id='@id' style='border:1px solid black; width: @Width px; height: @Height px; position: relative; left: @Left px; top: @Top px;'><tr><td></td><td></td></tr></table>".RemoveCR().DoubleQuoted();
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

}
