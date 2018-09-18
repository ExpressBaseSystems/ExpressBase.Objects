using ExpressBase.Common.Extensions;
using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExpressBase.Objects
{
    [EnableInBuilder(BuilderType.WebForm)]
    class EbGroupBox : EbControlContainer
    {
        public EbGroupBox()
        {
            this.Controls = new List<EbControl>();
            this.ObjType = this.GetType().Name.Substring(2, this.GetType().Name.Length - 2);
        }

        public override string Label { get; set; }

        public override string GetToolHtml()
        {
            return @"<div eb-type='@toolName' class='tool'><i class='fa fa-square-o'></i>  @toolName</div>".Replace("@toolName", this.GetType().Name.Substring(2));
        }

        public override string GetDesignHtml()
        {
            return GetHtml().RemoveCR().DoubleQuoted();
        }

        public override string GetHtml()
        {
            string html = @"
            <div id='@name@' ebsid='@ebsid@' class='Eb-ctrlContainer' Ctype='TableLayout'>
                <div class='group-box'>";

            foreach (EbControl ec in this.Controls)
                html += ec.GetHtml();

            return (html + "</div></div>").Replace("@name@", this.Name).Replace("@ebsid@", this.EbSid);
        }

    }
}
