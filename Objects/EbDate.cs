using ExpressBase.Common;
using ExpressBase.Common.Extensions;
using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using ExpressBase.Common.Structures;
using ExpressBase.Objects.Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Common;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using ExpressBase.Security;
using System.Globalization;
using ExpressBase.Common.LocationNSolution;
using ExpressBase.Common.Constants;

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

    //public enum DateFormat
    //{
    //    dd_mm_yyyy = 0,
    //    mm_dd_yyyy = 1,
    //    yyyy_mm_dd = 2,
    //    yyyy_dd_mm = 3,
    //}


    [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl)]
    public class EbDate : EbControlUI, IEbInputControls
    {

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl)]
        [PropertyEditor(PropertyEditorType.Expandable)]
        [PropertyGroup(PGConstants.APPEARANCE)]
        [UIproperty]
        [DefaultPropValue(7, 10, 7, 10)]
        [OnChangeUIFunction("Common.INP_PADDING")]
        public UISides Padding { get; set; }


        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl)]
        [PropertyGroup(PGConstants.APPEARANCE)]
        [PropertyEditor(PropertyEditorType.FontSelector)]
        [OnChangeUIFunction("Common.INP_FONT_STYLE")]
        public EbFont FontStyle { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl)]
        [HideInPropertyGrid]
        public override EbDbTypes EbDbType { get { return (EbDbTypes)this.EbDateType; } }

        public EbDate()
        {
        }

        [JsonIgnore]
        public override string EnableJSfn
        {
            get
            {
                return JSFnsConstants.Ctrl_EnableJSfn
+ @"
if(this.IsNullable && !($('#' + this.EbSid_CtxId).closest('.input-group').find(`input[type='checkbox']`).is(':checked')))
    $('#' + this.EbSid_CtxId).prop('disabled', true).next('.input-group-addon').css('pointer-events', 'none');"
;
            }
            set { }
        }

        [OnDeserialized]
        public void OnDeserializedMethod(StreamingContext context)
        {
            this.BareControlHtml = this.GetBareHtml();
            this.BareControlHtml4Bot = this.BareControlHtml;
            this.ObjType = this.GetType().Name.Substring(2, this.GetType().Name.Length - 2);
            if (this.Padding == null)
                this.Padding = new UISides() { Bottom = 7, Left = 10, Top = 7, Right = 10 };
            //this.EbDbType = this.EbDbType;
        }

        public override string GetHtml4Bot()
        {
            return ReplacePropsInHTML(HtmlConstants.CONTROL_WRAPER_HTML4BOT);
        }

        [HideInPropertyGrid]
        public override bool Unique { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl)]
        [PropertyGroup(PGConstants.CORE)]
        public EbDateType EbDateType { get; set; }

        //[EnableInBuilder(BuilderType.BotForm)]
        public DateTime Min { get; set; }

        //[EnableInBuilder( BuilderType.BotForm)]
        public DateTime Max { get; set; }

        //[EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm)]
        public DateTime Value { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl)]
        public string PlaceHolder { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl)]
        [PropertyGroup(PGConstants.EXTENDED)]
        public bool AutoCompleteOff { get; set; }

		[EnableInBuilder(BuilderType.BotForm)]
		[HideInPropertyGrid]
		public bool IsBasicControl { get => true; }

		[EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl)]
        [OnChangeExec(@"
                if (this.DoNotPersist){
                        pg.HideProperty('IsNullable');
                }
                else {
                       pg.ShowProperty('IsNullable');
                }
            ")]
        public override bool DoNotPersist { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl)]
        [PropertyGroup(PGConstants.EXTENDED)]
        public bool IsNullable { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl)]
        [PropertyGroup(PGConstants.EXTENDED)]
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

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl)]
        [PropertyGroup(PGConstants.EXTENDED)]
        public TimeShowFormat ShowTimeAs_ { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl)]
        [PropertyGroup(PGConstants.EXTENDED)]
        public DateShowFormat ShowDateAs_ { get; set; }

        //[EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl)]
        //public DateFormat DateFormat { get; set; }

        //[HideInPropertyGrid]
        //[EnableInBuilder(BuilderType.BotForm)]
        //public override bool IsReadOnly { get => this.IsDisable; }

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

        [HideInPropertyGrid]
        [JsonIgnore]
        public override string ToolIconHtml { get { return "<i class='fa fa-calendar'></i>"; } set { } }

        [HideInPropertyGrid]
        [JsonIgnore]
        public override string ToolNameAlias { get { return "Date Time"; } set { } }

        //public override string GetToolHtml()
        //{
        //    return @"<div eb-type='@toolName' class='tool'><i class='fa fa-calendar'></i>  @toolName</div>".Replace("@toolName", this.GetType().Name.Substring(2));
        //}

        public override string GetDesignHtml()
        {
            return GetHtml().RemoveCR().GraveAccentQuoted();
        }

        public override string GetBareHtml()
        {
            return @"
        <div class='input-group' style='width:100%;'>
            @IsNullable@
            <input id='@ebsid@' ui-inp data-ebtype='@data-ebtype@'  data-toggle='tooltip' title='@toolTipText@' class='date' type='text' name='@name@' autocomplete = '@autoComplete@' @value@ @tabIndex@ style='width:100%; display:inline-block;' @required@ @placeHolder@ />
            <span class='input-group-addon' style='padding: 0px;'> <i id='@ebsid@TglBtn' class='fa  @atchdLbl@' aria-hidden='true'></i> </span>
        </div>"
.Replace("@name@", (this.Name != null ? this.Name.Trim() : ""))
.Replace("@data-ebtype@", "6")//( (int)this.EbDateType ).ToString())
.Replace("@toolTipText@", this.ToolTipText)
.Replace("@ebsid@", String.IsNullOrEmpty(this.EbSid_CtxId) ? "@ebsid@" : this.EbSid_CtxId)
.Replace("@autoComplete@", this.AutoCompleteOff ? "off" : "on")
.Replace("@value@", "")//"value='" + this.Value + "'")
.Replace("@tabIndex@", "tabindex='" + this.TabIndex + "'")
.Replace("@required@", (this.Required && !this.Hidden ? " required" : string.Empty))
.Replace("@placeHolder@", "placeholder='" + this.PlaceHolder + "'")
.Replace("@atchdLbl@", (this.EbDateType.ToString().ToLower() == "time") ? "fa-clock-o" : "fa-calendar")
.Replace("@IsNullable@", (this.IsNullable) ? "<span class='input-group-addon nullable-check' style='padding: 0px 7px !important;'><input type='checkbox' style='min-height:unset;'></span>" : "");
        }

        [JsonIgnore]
        public override string GetValueFromDOMJSfn
        {
            get
            {
                return
                    @"if((this.IsNullable && !($('#' + this.EbSid_CtxId).siblings('.nullable-check').find('input[type=checkbox]').prop('checked'))) || $('#' + this.EbSid_CtxId).val() === '')
                        return undefined;
                    else if(this.ShowDateAs_ === 1 || this.ShowDateAs_ === 2) //month picker or year picker
                        return $('#' + this.EbSid_CtxId).val();
                    else if(this.EbDateType === 5) //Date
                        return moment($('#' + this.EbSid_CtxId).val(), ebcontext.user.Preference.ShortDatePattern).format('YYYY-MM-DD');
                    else if(this.EbDateType === 6) //DateTime
                        return moment($('#' + this.EbSid_CtxId).val(), ebcontext.user.Preference.ShortDatePattern + ' ' + ebcontext.user.Preference.ShortTimePattern).format('YYYY-MM-DD HH:mm:ss');
                    else if(this.EbDateType === 17) //Time
                        return moment($('#' + this.EbSid_CtxId).val(), ebcontext.user.Preference.ShortTimePattern).format('HH:mm:ss');";
            }
            set { }
        }

        [JsonIgnore]
        public override string SetDisplayMemberJSfn
        {
            get
            {
                return
                    @" SetDisplayMemberDate_EB.bind(this)(p1, p2);";
            }
            set { }
        }

        [JsonIgnore]
        public override string GetDisplayMemberFromDOMJSfn
        {
            get
            {
                return @"if((this.IsNullable && !($('#' + this.EbSid_CtxId).siblings('.nullable-check').find('input[type=checkbox]').prop('checked'))) || $('#' + this.EbSid_CtxId).val() === '')
                        return undefined;
                    else
                        return $('#' + this.EbSid_CtxId).val();";
            }
            set { }
        }

        [JsonIgnore]
        public override string ClearJSfn
        {
            get
            {

                return @"
                    console.log(this.Label);
                    if(this.IsNullable)
                        $('#' + this.EbSid_CtxId).siblings('.nullable-check').find('input[type=checkbox]').prop('checked', false);
                    else
                        return;";// cannot clear none-nullable date
            }
            set { }
        }

        [JsonIgnore]
        public override string OnChangeBindJSFn { get { return @"$('#' + this.EbSid_CtxId).on('change', p1); $('#' + this.EbSid_CtxId).siblings('.nullable-check').find('input[type=checkbox]').on('change', p1);"; } set { } }

        public override bool ParameterizeControl(IDatabase DataDB, List<DbParameter> param, string tbl, SingleColumn cField, bool ins, ref int i, ref string _col, ref string _val, ref string _extqry, User usr, SingleColumn ocF)
        {
            try
            {
                if (this.EbDateType == EbDateType.Date)
                {
                    if (this.ShowDateAs_ == DateShowFormat.Year)
                        cField.Value = DateTime.ParseExact(cField.Value.ToString(), "yyyy", CultureInfo.InvariantCulture);
                    else if (this.ShowDateAs_ == DateShowFormat.Year_Month)
                        cField.Value = DateTime.ParseExact(cField.Value.ToString(), "MM/yyyy", CultureInfo.InvariantCulture);
                    else
                        cField.Value = DateTime.ParseExact(cField.Value.ToString(), "yyyy-MM-dd", CultureInfo.InvariantCulture);
                }
                else
                {
                    DateTime dt;
                    if (this.EbDateType == EbDateType.DateTime)
                        dt = DateTime.ParseExact(cField.Value.ToString(), "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                    else
                        dt = DateTime.ParseExact(cField.Value.ToString(), "HH:mm:ss", CultureInfo.InvariantCulture);
                    cField.Value = dt.ConvertToUtc(usr.Preference.TimeZone);
                }

                param.Add(DataDB.GetNewParameter(cField.Name + "_" + i, EbDbTypes.DateTime, cField.Value));
            }
            catch (Exception e)
            {
                if (!this.IsNullable)
                    Console.WriteLine($"Found unexpected value for EbDate control field...\nName : {cField.Name}\nValue : {cField.Value}\nMessage : {e.Message}"); ;
                DbParameter p = DataDB.GetNewParameter(cField.Name + "_" + i, (EbDbTypes)cField.Type);
                p.Value = DBNull.Value;
                param.Add(p);
            }

            if (ins)
            {
                _col += string.Concat(cField.Name, ", ");
                _val += string.Concat("@", cField.Name, "_", i, ", ");
            }
            else
                _col += string.Concat(cField.Name, "=@", cField.Name, "_", i, ", ");
            i++;
            return true;
        }

        public override SingleColumn GetSingleColumn(User UserObj, Eb_Solution SoluObj, object Value)
        {
            return EbDate.GetSingleColumn(this, UserObj, SoluObj, Value);
        }

        public static SingleColumn GetSingleColumn(dynamic _this, User UserObj, Eb_Solution SoluObj, object Value)
        {
            object _formattedData = Value;
            string _displayMember = Value == null ? string.Empty : Value.ToString();
            bool skip = false;

            if (_this is EbDate || _this is EbDGDateColumn)
            {
                if (Value == null && _this.IsNullable)
                    skip = true;
            }

            if (!skip)
            {
                DateTime dt;
                if (Value == null)
                    dt = DateTime.UtcNow;
                else
                {
                    if (Value.GetType() == typeof(TimeSpan))
                        dt = DateTime.MinValue + (TimeSpan)Value;
                    else
                        dt = Convert.ToDateTime(Value);
                }
                DateTime dt_cov = dt.ConvertFromUtc(UserObj.Preference.TimeZone);

                if (_this.EbDateType == EbDateType.Date)
                {
                    if (!(_this is EbDate)) //EbSysCreatedAt EbSysModifiedAt EbDGDateColumn EbDGCreatedAtColumn EbDGModifiedAtColumn
                    {
                        _formattedData = dt_cov.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
                        _displayMember = dt_cov.ToString(UserObj.Preference.GetShortDatePattern(), CultureInfo.InvariantCulture);
                    }
                    else //EbDate
                    {
                        if (_this.ShowDateAs_ == DateShowFormat.Year)
                        {
                            _formattedData = dt.ToString("yyyy", CultureInfo.InvariantCulture);
                            _displayMember = _formattedData.ToString();
                        }
                        else if (_this.ShowDateAs_ == DateShowFormat.Year_Month)
                        {
                            _formattedData = dt.ToString("MM/yyyy", CultureInfo.InvariantCulture);
                            _displayMember = _formattedData.ToString();
                        }
                        else
                        {
                            _formattedData = dt.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
                            _displayMember = dt.ToString(UserObj.Preference.GetShortDatePattern(), CultureInfo.InvariantCulture);
                        }
                    }
                }
                else if (_this.EbDateType == EbDateType.DateTime)
                {
                    _formattedData = dt_cov.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                    _displayMember = dt_cov.ToString(UserObj.Preference.GetShortDatePattern() + " " + UserObj.Preference.GetShortTimePattern(), CultureInfo.InvariantCulture);
                }
                else
                {
                    _formattedData = dt_cov.ToString("HH:mm:ss", CultureInfo.InvariantCulture);
                    _displayMember = dt_cov.ToString(UserObj.Preference.GetShortTimePattern(), CultureInfo.InvariantCulture);
                }
            }

            return new SingleColumn()
            {
                Name = _this.Name,
                Type = (int)_this.EbDbType,
                Value = _formattedData,
                Control = _this,
                ObjType = _this.ObjType,
                F = _displayMember
            };
        }
    }
}
