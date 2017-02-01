using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExpressBase.Objects
{
    public enum EbDateType
    {
        Date,
        Time,
        DateTime,
    }

    [ProtoBuf.ProtoContract]
    public class EbDate : EbControl
    {
        public EbDate() { }

        [ProtoBuf.ProtoMember(1)]
        [System.ComponentModel.Category("Behavior")]
        public EbDateType Type { get; set; }

        public override string GetHead()
        {
            return @"
$('.date').mask('00/00/0000'); 
$('#datetimepicker').datepicker({
    dateFormat: 'y/MM/DD',
	timeFormat: 'HH:mm:ss:tt',
	stepHour: 2,
	stepMinute: 10,
	stepSecond: 10
});";
        }

        public override string GetHtml()
        {
            return string.Format(@"<div style='position:absolute; left:{1}px; top:{2}px; {6}'><div style='{14} {15}'>{5}</div>
<div  class='tooltp'><input id='datetimepicker' class='date' type='text'  name='{0}'   style='width:{3}px; height:{4}px; {12} {13} 
display:inline-block;{8} {7} {11} />
<div style='display: inline-block;'></div> {9}</div>
<div class='helpText'> {10} </div>
</div>",
this.Name, this.Left, this.Top, this.Width, this.Height, this.Label, //5
this.HiddenString, (this.Required && !this.Hidden ? " required" : string.Empty), this.ReadOnlyString,//8
((this.ToolTipText == null) ? string.Empty : ((this.ToolTipText.Trim().Length == 0) ? string.Empty : ("<span class='tooltptext'>" + this.ToolTipText + "</span>"))),
this.HelpText, "tabindex='" + this.TabIndex + "'",//11
 "background-color:" + this.BackColorSerialized + ";", "color:" + this.ForeColorSerialized + ";", "background-color:" + this.LabelBackColorSerialized + ";", "color:" + this.LabelForeColorSerialized + ";");
        }
    }
}
