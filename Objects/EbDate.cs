using ExpressBase.Common.Extensions;
using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using ExpressBase.Common.Structures;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace ExpressBase.Objects
{
    public enum EbDateType
    {
        Date ,
        Time ,
        DateTime ,
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


    [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm)]
    public class EbDate : EbControl
    {
        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm)]
        public  EbDbTypes EbDbType { get { return EbDbTypes.Date; } }

        public EbDate()
        {
            
        }

        [OnDeserialized]
        public void OnDeserializedMethod(StreamingContext context)
        {
            this.BareControlHtml = this.GetBareHtml();
            this.ObjType = this.GetType().Name.Substring(2, this.GetType().Name.Length - 2);
        }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm)]
        public EbDateType EbDateType { get; set; }

        //[EnableInBuilder(BuilderType.BotForm)]
        public DateTime Min { get; set; }

        //[EnableInBuilder( BuilderType.BotForm)]
        public DateTime Max { get; set; }

        //[EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm)]
        public DateTime Value { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm)]
        public string PlaceHolder { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm)]
        public bool AutoCompleteOff { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm)]
        private string maskPattern
        {
            get
            {
                if (this.EbDateType.ToString() == "Date")
                    return "0000/00/00";
                else if (this.EbDateType.ToString() == "Time")
                    return "00:00";
                else
                    return "0000/00/00 00:00";

            }
            set { }
        }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm)]
        public TimeShowFormat ShowTimeAs_ { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm)]
        public DateShowFormat ShowDateAs_ { get; set; }

        public override string GetHead()
        {
            return (((!this.Hidden) ? this.UniqueString + this.RequiredString : string.Empty) + @"".Replace("{0}", this.Name)) + @"
$('#@idContainer [class=input-group-addon]').click(function(){
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
.Replace("@dateType", (this.EbDateType.ToString() == "Date") ? "timepicker:false" : ((this.EbDateType.ToString() == "Time") ? "datepicker:false" : string.Empty))
.Replace("@id", this.Name)
.Replace("@maskPattern", this.maskPattern)
.Replace("@format", (this.EbDateType.ToString() == "Date") ? "Y/m/d" : (this.EbDateType.ToString() == "Time") ? "H:i" : "Y/m/d H:i")
.Replace("@maxDate", this.Max.ToString())
.Replace("@minDate", this.Min.ToString());
        }

        public string Wrap4Developer(string EbCtrlHTML)
        {
            return @"<div class='controlTile' tabindex='1' onclick='event.stopPropagation();$(this).focus()'>
                                <div class='ctrlHead' style='display:none;'>
                                    <i class='fa fa-arrows moveBtn' aria-hidden='true'></i>
                                    <a href='#' class='close' style='cursor:default' data-dismiss='alert' aria-label='close' title='close'>×</a>
                                </div>"
                                    + EbCtrlHTML
                        + "</div>";
        }

        public override string GetDesignHtml()
        {
            string _html = null; 
            
            if (this.Name == null) //if in new mode
                _html = GetHtml();

            else //if edit mode
                _html = Wrap4Developer(GetHtml());

            return _html.RemoveCR().GraveAccentQuoted();
        }

        public override string GetBareHtml()
        {
            return @" 
        <div class='input-group' style='width:100%;'>
            <input id='@name@' data-ebtype='@datetype@'  data-toggle='tooltip' title='@toolTipText@' class='date' type='text'name='@name@' autocomplete = '@autoComplete@' @value@ @tabIndex@ style='width:100%; @BackColor@ @ForeColor@ display:inline-block; @fontStyle@ @readOnlyString@ @required@ @placeHolder@ />
            <span class='input-group-addon' onclick=""$('#@name@').focus().focus()""> <i id='@name@TglBtn' class='fa  @atchdLbl@' aria-hidden='true'></i> </span>
        </div>"
.Replace("@name@", this.Name)
.Replace("@datetype@", "6")//( (int)this.EbDateType ).ToString())
.Replace("@toolTipText@", this.ToolTipText)
.Replace("@autoComplete@", this.AutoCompleteOff ? "off" : "on")
.Replace("@value@", "")//"value='" + this.Value + "'")
.Replace("@tabIndex@", "tabindex='" + this.TabIndex + "'")
    .Replace("@BackColor@ ", ("background-color:" + ((this.BackColor != null) ? this.BackColor : "@BackColor@ ") + ";"))
    .Replace("@ForeColor@ ", "color:" + ((this.ForeColor != null) ? this.ForeColor : "@ForeColor@ ") + ";")
.Replace("@required@", " required")//(this.Required && !this.Hidden ? " required" : string.Empty))
.Replace("@readOnlyString@", this.ReadOnlyString)
.Replace("@placeHolder@", "placeholder='" + this.PlaceHolder + "'")
.Replace("@atchdLbl@", (this.EbDateType.ToString().ToLower() == "time") ? "fa-clock-o" : "fa-calendar")

//.Replace("@fontStyle@", (this.FontSerialized != null) ?
//                            (" font-family:" + this.FontSerialized.FontFamily + ";" + "font-style:" + this.FontSerialized.Style
//                            + ";" + "font-size:" + this.FontSerialized.SizeInPoints + "px;")
//                        : string.Empty)
;
        }

        public override string GetHtml()
        {
            return GetHtmlHelper();
        }

        private string GetHtmlHelper()
        {
            string EbCtrlHTML = @"
    <div id='cont_@name@' Ctype='Date' class='Eb-ctrlContainer' style='@hiddenString'>
        <span id='@name@Lbl' style='@LabelBackColor  @LabelForeColor '> @Label </span>
       @barehtml@
        <span class='helpText'> @HelpText </span>
    </div>
"
.Replace("@barehtml@", this.GetBareHtml())
.Replace("@name@", this.Name)
.Replace("@hiddenString", this.HiddenString)

    .Replace("@LabelForeColor ", "color:" + ((this.LabelForeColor != null) ? this.LabelForeColor : "@LabelForeColor ") + ";")
    .Replace("@LabelBackColor ", "background-color:" + ((this.LabelBackColor != null) ? this.LabelBackColor : "@LabelBackColor ") + ";")
    .Replace("@HelpText ", ((this.HelpText != null) ? this.HelpText : "@HelpText "))
    .Replace("@Label ", ((this.Label != null) ? this.Label : "@Label "));
            return EbCtrlHTML;
        }
    }
}
