using ExpressBase.Common.Extensions;
using ExpressBase.Common.Objects.Attributes;
using ExpressBase.Common.Objects;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExpressBase.Objects
{
    [EnableInBuilder(BuilderType.Report)]
    public class EbPageFooter : EbReportSection
    {
        public override string GetDesignHtml()
        {
            return "<div class='pageHeaders' eb-type='PageFooter' tabindex='1' id='@id' data_val='3' style='width :100%;height: @SectionHeight ; position: relative'> </div>".RemoveCR().DoubleQuoted(); //background-color:@BackColor ;
        }

        public override string GetJsInitFunc()
        {
            return @"
    this.Init = function(id)
        {
    this.BackColor = 'transparent';
};";
        }
    }
}
