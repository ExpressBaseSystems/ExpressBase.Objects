using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExpressBase.Objects
{
    [ProtoBuf.ProtoContract]
    public class EbComboBox : EbControl
    {
        private string OptionString { get; set; }

        public EbComboBox() { }
        public override string GetHead()
        {
            return @"";
        }

        public override string GetHtml()
        {
            return string.Format(@"

<div style='position:absolute; left:{1}px; min-height: 12px; top:{2}px; {6}'>
    <div style='{14} {15}'>{5}</div
    <div  class='tooltp'>
    <div class='attachedlabel atchdLblL'>$</div>
        <select type='text'   name='{0}' value={16} placeholder='0.00' class='numinput numinputL ' id='{0}' style='width:{3}px; height:{4}px; {12} {13} display:inline-block;{8} {7} {11} />
            {16}
        </select>
        <div style='display: inline-block;'></div> {9}
    </div>
    <div class='helpText'> {10} </div>
</div>",
this.Name, this.Left, this.Top, this.Width, this.Height, this.Label, //5
this.HiddenString, (this.Required && !this.Hidden ? " required" : string.Empty), this.ReadOnlyString,//9
((this.ToolTipText == null) ? string.Empty : ((this.ToolTipText.Trim().Length == 0) ? string.Empty : ("<span class='tooltptext'>" + this.ToolTipText + "</span>"))),
this.HelpText, "tabindex='" + this.TabIndex + "'",//11
"background-color:" + this.BackColorSerialized + ";", "color:" + this.ForeColorSerialized + ";", "background-color:" + this.LabelBackColorSerialized + ";",
"color:" + this.LabelForeColorSerialized + ";", this.OptionString);
        }
    }
}
