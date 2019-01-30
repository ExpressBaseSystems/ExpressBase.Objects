using System;
using System.Collections.Generic;
using System.Text;

namespace ExpressBase.Objects.Helpers
{
    public static class HtmlConstants
    {
        public const string Label = @"";

        public const string HelpText = @"";

        public const string CONTROL_WRAPER_HTML4WEB = @"
        <div id='cont_@ebsid@' ebsid='@ebsid@' name='@name@' class='Eb-ctrlContainer' @childOf@ ctype='@type@' style='@hiddenString@'>
            <span class='eb-ctrl-label' ui-label id='@ebsidLbl'>@Label@ @req@ </span>
                <div  id='@ebsid@Wraper' class='ctrl-cover'>
                    @barehtml@
                </div>
            <span class='helpText' ui-helptxt >@helpText@ </span>
        </div>";
    }
}
