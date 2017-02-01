using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExpressBase.Objects
{
    [ProtoBuf.ProtoContract]
    public class EbNumeric : EbControl
    {
        public EbNumeric() { }

        [ProtoBuf.ProtoMember(1)]
        [System.ComponentModel.Category("Behavior")]
        public int MaxLength { get; set; }

        private string MaxLengthString
        {
            get { return (this.MaxLength > 0) ? string.Format("maxlength='{0}'", this.MaxLength) : string.Empty; }
        }

        public override string GetHtml()
        {
            return string.Format(@"
<div style='position:absolute; left:{1}px; top:{2}px; {7}'>
<div style='{15} {16}'>{5}</div>
<div  class='tooltp'><input type='number'  name='{0}' id='{0}' {6} style='width:{3}px; height:{4}px; {13} {14} display:inline-block;{9} {8} {12} />
<div style='display: inline-block;'></div> {10}</div>
<div class='helpText'> {11} </div>
</div>",
this.Name, this.Left, this.Top, this.Width, this.Height, this.Label, this.MaxLengthString,//6
this.HiddenString, (this.Required && !this.Hidden ? " required" : string.Empty), this.ReadOnlyString,//9
((this.ToolTipText == null) ? string.Empty : ((this.ToolTipText.Trim().Length == 0) ? string.Empty : ("<span class='tooltptext'>" + this.ToolTipText + "</span>"))),
this.HelpText, "tabindex='" + this.TabIndex + "'",//12
 "background-color:" + this.BackColorSerialized + ";", "color:" + this.ForeColorSerialized + ";", "background-color:" + this.LabelBackColorSerialized + ";", "color:" + this.LabelForeColorSerialized + ";");
        }
    }
}
