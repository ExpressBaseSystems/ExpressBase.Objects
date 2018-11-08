using ExpressBase.Common.Extensions;
using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using ExpressBase.Common.Structures;
using ExpressBase.Objects.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace ExpressBase.Objects
{
    public enum EbDateType // integers corresponding to  EbDbTypes Enum
    {
        Date = 5,
        Time = 17,
        DateTime = 6,
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
    public class EbDate : EbControlUI
    {
        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm)]
        public override EbDbTypes EbDbType { get { return (EbDbTypes)this.EbDateType; } }

        public EbDate()
        {

        }

        [OnDeserialized]
        public void OnDeserializedMethod(StreamingContext context)
        {
            this.BareControlHtml = this.GetBareHtml();
            this.ObjType = this.GetType().Name.Substring(2, this.GetType().Name.Length - 2);
            //this.EbDbType = this.EbDbType;
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

        [HideInPropertyGrid]
        [EnableInBuilder(BuilderType.BotForm)]
        public override bool IsReadOnly { get => this.ReadOnly; }

        public override VendorDbType GetvDbType(IVendorDbTypes vDbTypes)
        {
            VendorDbType _vdbtype = vDbTypes.String;
            if (EbDateType == EbDateType.Date)
                _vdbtype = vDbTypes.Date;
            else if (EbDateType == EbDateType.DateTime)
                _vdbtype = vDbTypes.DateTime;
            else if (EbDateType == EbDateType.Time)
                _vdbtype = vDbTypes.Time;

            return _vdbtype;
        }

        public override string GetToolHtml()
        {
            return @"<div eb-type='@toolName' class='tool'><i class='fa fa-calendar'></i>  @toolName</div>".Replace("@toolName", this.GetType().Name.Substring(2));
        }

        public override string GetHead()
        {
            return (((!this.Hidden) ? this.UniqueString + this.RequiredString : string.Empty) + @"".Replace("{0}", this.Name)) + @"
$('#@idContainer [class=input-group-addon]').click(function(){
    //$('#@idContainer [class=date]').toggle();
        $('#@id').focus();
        $('#@id').trigger('click');
});

$('#@id').mask('@maskPattern'); 
//$('#@id').datetimepicker({
//    format:'@format',
    //minDate:'@maxDate',
    //maxDate:'@minDate',
//    @dateType
//});

alert('chinchu');
$('#@id').MonthPicker({ StartYear: 2018, ShowIcon: false });"

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
            <input id='@ebsid@' data-ebtype='@data-ebtype@'  data-toggle='tooltip' title='@toolTipText@' class='date' type='text' name='@name@' autocomplete = '@autoComplete@' @value@ @tabIndex@ style='width:100%; @BackColor@ @ForeColor@ display:inline-block; @fontStyle@ @readOnlyString@ @required@ @placeHolder@ />
            <span class='input-group-addon' style='padding: 0px;'> <i id='@ebsid@TglBtn' class='fa  @atchdLbl@' aria-hidden='true' style='padding: 6px 12px;'></i> </span>
        </div>"
.Replace("@name@", (this.Name != null ? this.Name.Trim() : ""))
.Replace("@data-ebtype@", "6")//( (int)this.EbDateType ).ToString())
.Replace("@toolTipText@", this.ToolTipText)
.Replace("@ebsid@", String.IsNullOrEmpty(this.EbSid_CtxId) ? "@ebsid@" : this.EbSid_CtxId)
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
            //            string EbCtrlHTML =
            //                HtmlConstants.CONTROL_WRAPER_HTML4WEB
            //.Replace("@barehtml@", this.GetBareHtml())
            //.Replace("@name@", this.Name)
            //.Replace("@ebsid@", EbSid_CtxId)
            //.Replace("@type@", this.ObjType)

            string EbCtrlHTML = HtmlConstants.CONTROL_WRAPER_HTML4WEB
               .Replace("@LabelForeColor ", "color:" + (LabelForeColor ?? "@LabelForeColor ") + ";")
               .Replace("@LabelBackColor ", "background-color:" + (LabelBackColor ?? "@LabelBackColor ") + ";");

            return ReplacePropsInHTML(EbCtrlHTML);
        }
    }
}
