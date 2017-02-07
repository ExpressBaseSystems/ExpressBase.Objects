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

        [ProtoBuf.ProtoMember(2)]
        [System.ComponentModel.Category("Behavior")]
        public int DecimalPlaces { get; set; }

        [ProtoBuf.ProtoMember(3)]
        [System.ComponentModel.Category("Appearance")]
        public decimal Value { get; set; }

        [ProtoBuf.ProtoMember(4)]
        private string PlaceHolder { get; set; }

        private string MaxLengthString
        {
            get { return ((this.MaxLength > 0) ? "$('#{0}').focus(function() {$(this).select();});".Replace("{0}", this.Name).Replace("{1}", this.Value.ToString()) : string.Empty); }
        }

        public override string GetHead()
        {
            return (this.MaxLengthString + @"
$('.money').mask('#,##0.00', {
    reverse: true,
    translation: {
    placeholder: '000,000.00'
  }
    }); ");
        }

        public override string GetHtml()
        {
            return string.Format(@"
<div style='position:absolute; left:{1}px; top:{2}px; {7}'>
    <div style='{15} {16}'>{5}</div>
    <div class='attachedlabel'>$</div>
    <div  class='tooltp'>
        <input type='text' step='0.1' name='{0}' value={17} placeholder={18} class='money' id='{0}' {6} style='width:{3}px; height:{4}px; {13} {14} display:inline-block;{9} {8} {12} />
        <div style='display: inline-block;'></div> {10}
    </div>
    <div class='helpText'> {11} </div>
</div>",
this.Name, this.Left, this.Top, this.Width, this.Height, this.Label, this.MaxLengthString,//6
this.HiddenString, (this.Required && !this.Hidden ? " required" : string.Empty), this.ReadOnlyString,//9
((this.ToolTipText == null) ? string.Empty : ((this.ToolTipText.Trim().Length == 0) ? string.Empty : ("<span class='tooltptext'>" + this.ToolTipText + "</span>"))),
this.HelpText, "tabindex='" + this.TabIndex + "'",//12
 "background-color:" + this.BackColorSerialized + ";", "color:" + this.ForeColorSerialized + ";",  "background-color:" + this.LabelBackColorSerialized + ";", "color:" + this.LabelForeColorSerialized + ";", this.Value, (this.PlaceHolder.Trim() != ""|| this.PlaceHolder==null) ?  this.PlaceHolder  :"0.00");
        }
    }
}
