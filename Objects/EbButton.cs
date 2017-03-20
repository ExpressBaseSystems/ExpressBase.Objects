using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace ExpressBase.Objects
{
    [ProtoBuf.ProtoContract]
    public class EbButton : EbControl
    {
        public EbButton() { }

        public EbButton(object parent)
        {
            this.Parent = parent;
        }

        [ProtoBuf.ProtoMember(1)]
        [System.ComponentModel.Category("Appearance")]
        public string Text { get; set; }

        public override string GetHead()
        {
            return this.RequiredString + @"
                    ".Replace("{0}", this.Id.ToString());
        }

        public override string GetHtml()
        {
            return string.Format(@"
<div id='@namecontainer' style='position:absolute; left:@leftpx; top:@toppx; '>
    <span id='@nameLbl' style='@lblBackColor @LblForeColor'>@label</span>
    <button id='@name' class='btn btn-default'  data-toggle='tooltip' title='{4}'
    style='width:@widthpx; height:@heightpx; @backColor @foreColor display:inline-block; @fontStyle>{1}</button>
</div>
"
.Replace("@name", this.Name)
.Replace("@left", this.Left.ToString())
.Replace("@top", this.Top.ToString())
.Replace("@width", this.Width.ToString())
.Replace("@height", this.Height.ToString())
.Replace("@label", this.Label)
.Replace("@hiddenString", this.HiddenString)
.Replace("@required", (this.Required && !this.Hidden ? " required" : string.Empty))
.Replace("@readOnlyString", this.ReadOnlyString)
.Replace("@toolTipText", this.ToolTipText)
.Replace("@helpText", this.HelpText)
.Replace("@text", "value='" + this.Text + "'")
.Replace("@tabIndex", "tabindex='" + this.TabIndex + "'")
.Replace("@backColor", "background-color:" + this.BackColorSerialized + ";")
.Replace("@foreColor", "color:" + this.ForeColorSerialized + ";")
.Replace("@lblBackColor", "background-color:" + this.LabelBackColorSerialized + ";")
.Replace("@LblForeColor", "color:" + this.LabelForeColorSerialized + ";")
.Replace("@fontStyle", (this.FontSerialized != null) ?
                            (" font-family:" + this.FontSerialized.FontFamily + ";" + "font-style:" + this.FontSerialized.Style
                            + ";" + "font-size:" + this.FontSerialized.SizeInPoints + "px;")
                        : string.Empty));
        }
    }
}
