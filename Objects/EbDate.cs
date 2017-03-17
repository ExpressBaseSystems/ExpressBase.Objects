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
        public EbDateType EbDateType { get; set; }

        private string EbDateTypeString
        {
            get
            {
                string returnval = string.Empty;
                switch (this.EbDateType)
                {
                    case EbDateType.Time:
                        returnval = "time";
                        break;
                    case EbDateType.Date:
                        returnval = "date";
                        break;
                    case EbDateType.DateTime:
                        returnval = "datetime";
                        break;
                }
                return returnval;
            }
        }

        public override string GetHead()
        {
            return @"
$('#$idTglBtn').click(function(){
    //$('#$idContainer [class=date]').toggle();
        $('#$id').focus();
        $('#$id').trigger('click');

});

$('.date').mask('00:00:00'); 
$('#$id').$$$$$$$picker({
    dateFormat: 'y/MM/DD',
	timeFormat: 'hh:mm:ss:tt',
	stepHour: 1,
	stepMinute: 1,
	stepSecond: 1
});".Replace("$$$$$$$", this.EbDateTypeString)
.Replace("$id", this.Name);
        }

        public override string GetHtml()
        {
            return string.Format(@"
<div id='{0}Container' style='position:absolute; left:400px; top:500px; {6}'>
    <div style='{14} {15}'>{5}</div>
    <div  class='input-group' style='width:1px;'>
        <input id='{0}' data-toggle='tooltip' title='{9}' class='date' type='text'  name='{0}'   style='width:{3}px; height:{4}px; {12} {13} display:inline-block;{8} {7} {11} />
        <i id='{0}TglBtn' class='fa fa-calendar input-group-addon' aria-hidden='true'></i>
    </div>
    <div class='helpText'> {10} </div>
</div>
",
this.Name, this.Left, this.Top, this.Width, this.Height, this.Label, //5
this.HiddenString, (this.Required && !this.Hidden ? " required" : string.Empty), this.ReadOnlyString,//8
this.ToolTipText, this.HelpText, "tabindex='" + this.TabIndex + "'",//11
 "background-color:" + this.BackColorSerialized + ";", "color:" + this.ForeColorSerialized + ";", "background-color:" + this.LabelBackColorSerialized + ";", "color:" + this.LabelForeColorSerialized + ";");
        }
    }
}
