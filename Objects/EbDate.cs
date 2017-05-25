using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace ExpressBase.Objects
{
    public enum EbDateType
    {
        Date = (int)System.Data.DbType.Date,
        Time = (int)System.Data.DbType.Time,
        DateTime = (int)System.Data.DbType.DateTime,
    }

    public enum TimeShowFormat
    {
        Hour_Minute_Second_12hrs,
        Hour_Minute_Second_24hrs,
        Hour_Minute_12hrs,       
        Hour_Minute_24hrs,
        Hour_12hrs,
        Hour_24hrs,
    }

    public enum DateShowFormat
    {
        Year_Month_Date,
        Year_Month,
        Year,
    }


    [ProtoBuf.ProtoContract]
    public class EbDate : EbControl
    {
        public EbDate() { }

        public EbDate(object parent)
        {
            this.Parent = parent;
        }

        [ProtoBuf.ProtoMember(1)]
        [System.ComponentModel.Category("Behavior")]
        public EbDateType EbDateType { get; set; }

        [ProtoBuf.ProtoMember(2)]
        [System.ComponentModel.Category("Data")]
        public DateTime Min { get; set; }

        [ProtoBuf.ProtoMember(3)]
        [System.ComponentModel.Category("Data")]
        public DateTime Max { get; set; }

        [ProtoBuf.ProtoMember(4)]
        [System.ComponentModel.Category("Data")]
        public DateTime Value { get; set; }

        [ProtoBuf.ProtoMember(5)]
        [System.ComponentModel.Category("Behavior")]
        public string PlaceHolder { get; set; }

        [ProtoBuf.ProtoMember(6)]
        [System.ComponentModel.Category("Behavior")]
        public bool AutoCompleteOff { get; set; }

        [ProtoBuf.ProtoMember(7)]
        private string maskPattern {
            get
            {
                if(this.EbDateType.ToString() == "Date")
                    return "0000/00/00";
                else if(this.EbDateType.ToString() == "Time")
                    return "00:00";
                else
                    return "0000/00/00 00:00";

            }
            set { }
        }

        [ProtoBuf.ProtoMember(8)]
        [System.ComponentModel.Category("Behavior")]
        public TimeShowFormat ShowTimeAs_ { get; set; }

        [ProtoBuf.ProtoMember(9)]
        [System.ComponentModel.Category("Behavior")]
        public DateShowFormat ShowDateAs_ { get; set; }

        public override string GetHead()
        {
            return  (((!this.Hidden) ? this.UniqueString + this.RequiredString : string.Empty) + @"".Replace("{0}", this.Name)) + @"
$('#@idTglBtn').click(function(){
    //$('#@idContainer [class=date]').toggle();
        $('#@id').focus();
        $('#@id').trigger('click');
});

$('#@id').mask('@maskPattern'); 
$('#@id').datetimepicker({
    format:'@format',
    //minDate:'@maxDate',
    //maxDate:'@minDate',
    @dateType
});"
.Replace("@dateType", (this.EbDateType.ToString()=="Date") ? "timepicker:false" : ((this.EbDateType.ToString() == "Time") ? "datepicker:false" : string.Empty) )
.Replace("@id", this.Name)
.Replace("@maskPattern", this.maskPattern)
.Replace("@format", (this.EbDateType.ToString() == "Date") ? "Y/m/d" : (this.EbDateType.ToString() == "Time") ? "H:i" : "Y/m/d H:i")
.Replace("@maxDate", this.Max.ToString())
.Replace("@minDate", this.Min.ToString());
        }

        public override string GetHtml()
        {
            return @"
<div id='@nameContainer' class='Eb-ctrlContainer' style='position:absolute; left:@leftpx; top:@top; @hiddenString'>
    <span id='@nameLbl' style='@lblBackColor @LblForeColor'>@label</span>
    <div  class='input-group' style='width:1px;'>
        <input id='@name' data-ebtype='@datetype'  data-toggle='tooltip' title='@toolTipText' class='date' type='text'  name='@name'  autocomplete = '@autoComplete' @value @tabIndex style='width:@widthpx; height:@heightpx; @backColor @foreColor display:inline-block; @fontStyle @readOnlyString @required @placeHolder />
        <span class='input-group-addon'> <i id='@nameTglBtn' class='fa  @atchdLbl' aria-hidden='true'></i> </span>
    </div>
    <span class='helpText'> @helpText </span>
</div>
"
.Replace("@name", this.Name)
.Replace("@left", this.Left.ToString())
.Replace("@top", this.Top.ToString())
.Replace("@width", this.Width.ToString())
.Replace("@height", this.Height.ToString())
.Replace("@datetype", ( (int)this.EbDateType ).ToString())
.Replace("@value", "value='" + this.Value + "'")
.Replace("@label", this.Label)
.Replace("@hiddenString", this.HiddenString)
.Replace("@required", (this.Required && !this.Hidden ? " required" : string.Empty))
.Replace("@readOnlyString", this.ReadOnlyString)
.Replace("@toolTipText", this.ToolTipText)
.Replace("@helpText", this.HelpText)
.Replace("@placeHolder", "placeholder='" + this.PlaceHolder + "'")
.Replace("@tabIndex", "tabindex='" + this.TabIndex + "'")
.Replace("@autoComplete", this.AutoCompleteOff ? "off" : "on")
.Replace("@backColor", "background-color:" + this.BackColorSerialized + ";")
.Replace("@foreColor", "color:" + this.ForeColorSerialized + ";")
.Replace("@lblBackColor", "background-color:" + this.LabelBackColorSerialized + ";")
.Replace("@LblForeColor", "color:" + this.LabelForeColorSerialized + ";")
.Replace("@fontStyle", (this.FontSerialized != null) ?
                            (" font-family:" + this.FontSerialized.FontFamily + ";" + "font-style:" + this.FontSerialized.Style
                            + ";" + "font-size:" + this.FontSerialized.SizeInPoints + "px;")
                        : string.Empty)
.Replace("@atchdLbl", (this.EbDateType.ToString().ToLower() == "time") ? "fa-clock-o" : "fa-calendar")
;
        }
    }
}
