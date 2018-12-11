using ExpressBase.Common.Extensions;
using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExpressBase.Objects
{
    [EnableInBuilder(BuilderType.UserControl, BuilderType.WebForm, BuilderType.FilterDialog)]
    [HideInToolBox]
    public class EbUserControl : EbControlContainer
    {
        public EbUserControl() { }

        //public string RefId { get; set; }

        public override string GetToolHtml()
        {
            return @"<div eb-type='@toolName' class='tool'><i class='fa fa-puzzle-piece'></i>  @toolName</div>".Replace("@toolName", this.GetType().Name.Substring(2));
        }

        public override string GetBareHtml()
        {
            return base.GetBareHtml();
        }

        public override string GetDesignHtml()
        {
            string html = @"
        <div id='cont_@ebsid@' ebsid='@ebsid@' name='@name@' class='Eb-ctrlContainer' ctype='@type@' style='@hiddenString@'>
            <span class='eb-ctrl-label' ui-label id='@ebsidLbl'>____ UserControl ____</span>
                
        </div>";
            return html.RemoveCR().DoubleQuoted();
        }

        public override string GetHtml()
        {
            string html = "<form id='@ebsid@' IsRenderMode='@rmode@' ebsid='@ebsid@' class='formB-box form-buider-form ebcont-ctrl' eb-form='true' ui-inp eb-type='UserControl' @tabindex@>";

            foreach (EbControl c in this.Controls)
                html += c.GetHtml();

            html += "</form>";

            return html
                .Replace("@name@", this.Name)
                .Replace("@ebsid@", this.EbSid);
                //.Replace("@rmode@", IsRenderMode.ToString())
                //.Replace("@tabindex@", IsRenderMode ? string.Empty : " tabindex='1'");
        }
    }
}
