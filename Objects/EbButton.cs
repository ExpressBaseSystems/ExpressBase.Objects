using ExpressBase.Common.Extensions;
using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace ExpressBase.Objects
{
    [ProtoBuf.ProtoContract]
    [EnableInBuilder(BuilderType.WebForm)]
    public class EbButton : EbControl
    {
        public EbButton() { }

        [EnableInBuilder(BuilderType.WebForm)]
        [ProtoBuf.ProtoMember(1)]
        [System.ComponentModel.Category("Appearance")]
        public string Text { get; set; }

        public override string GetHead()
        {
            return this.RequiredString + @"
                    ".Replace("@name", this.Id.ToString());
        }

        public override string GetDesignHtml()
        {
            return GetHtmlHelper(RenderMode.Developer).RemoveCR().DoubleQuoted();
        }

        public override string GetHtml()
        {
            return GetHtmlHelper(RenderMode.User);
        }

        private string GetHtmlHelper(RenderMode mode)
        {
            return string.Format(@"
<div id='@namecontainer' class='Eb-ctrlContainer'>
    <button id='@name' class='btn btn-default'  data-toggle='tooltip' title='@toolTipText' style='width:100%; @backColor @foreColor @fontStyle'>@text</button>
</div>
"
.Replace("@name", this.Name)
.Replace("@left", this.Left.ToString())
.Replace("@top", this.Top.ToString())
.Replace("@width", this.Width.ToString())
.Replace("@height", this.Height.ToString())
.Replace("@hiddenString", this.HiddenString)
.Replace("@toolTipText", this.ToolTipText)

    .Replace("@Text ", (this.Text != null) ? this.Text : "@Text ")

.Replace("@tabIndex", "tabindex='" + this.TabIndex + "'")
.Replace("@backColor", "background-color:" + this.BackColor + ";")
.Replace("@foreColor", "color:" + this.ForeColor + ";")
//.Replace("@fontStyle", (this.FontSerialized != null) ?
//                            (" font-family:" + this.FontSerialized.FontFamily + ";" + "font-style:" + this.FontSerialized.Style
//                            + ";" + "font-size:" + this.FontSerialized.SizeInPoints + "px;")
//                        : string.Empty)
);
        }
       
    }
}
