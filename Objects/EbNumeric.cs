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

        //[ProtoBuf.ProtoMember(4)]
        //private string PlaceHolder { get; set; }

        [ProtoBuf.ProtoMember(5)]
        [System.ComponentModel.Category("Behavior")]
        public bool AllowNegative { get; set; }

        private string MaxLengthString
        {
            get { return ((this.MaxLength > 0) ? "$('#{0}').focus(function() {$(this).select();});".Replace("{0}", this.Name).Replace("{1}", this.Value.ToString()) : string.Empty); }
        }

        public override string GetHead()
        {
            return (this.MaxLengthString  + ((!this.Hidden) ? this.UniqueString + this.RequiredString : string.Empty) + @"

{
                $('#{0}').parent().prev().css({'padding':   ( $('#{0}').parent().height()/5 + 2) + 'px' });
                $('#{0}').parent().prev().css({'font-size': ($('#{0}').css('font-size')) });
                if( $('#{0}').css('font-size').replace('px','') <= 10 )
                    $('#{0}').parent().prev().css({'height':   ( $('#{0}').parent().height() - ( 10 - $('#{0}').css('font-size').replace('px','')) ) + 'px' });  
                else
                    $('#{0}').parent().prev().css({'height':   ( $('#{0}').parent().height()) + 'px' });  
}
            
$('#{0}').mask('S000,000.00', {
    translation: {
    'S':{
        pattern: /-/,
        optional:true
        }
  }
    }); ").Replace("{0}", this.Name); 
        }

        public override string GetHtml()
        {
            return string.Format(@"

<div style='position:absolute; left:{1}px; min-height: 12px; top:{2}px; {7}'>
    <div style='{15} {16}'>{5}</div>
    <div class='attachedlabel atchdLblL'>$</div>
    <div  class='tooltp'>
        <input type='text'   name='{0}' value={17} placeholder='0.00' class='numinput numinputL ' id='{0}' {6} style='width:{3}px; height:{4}px; {13} {14} {18} display:inline-block;{9} {8} {12} />
        <div style='display: inline-block;'></div> {10}
    </div>
    <div class='helpText'> {11} </div>
</div>",
this.Name, this.Left, this.Top, this.Width, this.Height, this.Label, this.MaxLengthString,//6
this.HiddenString, (this.Required && !this.Hidden ? " required" : string.Empty), this.ReadOnlyString,//9
((this.ToolTipText == null) ? string.Empty : ((this.ToolTipText.Trim().Length == 0) ? string.Empty : ("<span class='tooltptext'>" + this.ToolTipText + "</span>"))),//10
this.HelpText, "tabindex='" + this.TabIndex + "'", "background-color:" + this.BackColorSerialized + ";",//13
"color:" + this.ForeColorSerialized + ";",  "background-color:" + this.LabelBackColorSerialized + ";",//15
"color:" + this.LabelForeColorSerialized + ";", this.Value,//17
(this.FontSerialized != null) ? (" font-family:" + this.FontSerialized.FontFamily + ";" + "font-style:" + this.FontSerialized.Style + ";" + "font-size:" + this.FontSerialized.SizeInPoints + "px;") : string.Empty );
        }
    }
}
