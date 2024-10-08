﻿using ExpressBase.Common;
using ExpressBase.Common.Constants;
using ExpressBase.Common.Extensions;
using ExpressBase.Common.LocationNSolution;
using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using ExpressBase.Common.Structures;
using ExpressBase.Objects.ServiceStack_Artifacts;
using ExpressBase.Security;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace ExpressBase.Objects
{

    public enum NumInpMode
    {
        Currency = 1,
        Numeric = 0,
        Phone = 2,
    }

    [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl, BuilderType.SurveyControl)]
    [SurveyBuilderRoles(SurveyRoles.AnswerControl)]
    public class EbNumeric : EbControlUI, IEbInputControls
    {
        public EbNumeric() { }


        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl, BuilderType.SurveyControl)]
        [PropertyEditor(PropertyEditorType.Expandable)]
        [PropertyGroup(PGConstants.APPEARANCE)]
        [UIproperty]
        [DefaultPropValue(7, 10, 7, 10)]
        [OnChangeUIFunction("Common.INP_PADDING")]
        public UISides Padding { get; set; }


        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl, BuilderType.SurveyControl)]
        [PropertyGroup(PGConstants.APPEARANCE)]
        [PropertyEditor(PropertyEditorType.FontSelector)]
        [OnChangeUIFunction("Common.INP_FONT_STYLE")]
        public EbFont FontStyle { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl)]
        [HideInPropertyGrid]
        public override EbDbTypes EbDbType { get { return EbDbTypes.Decimal; } }

        [JsonIgnore]
        public override string GetValueFromDOMJSfn
        {
            get
            {
                return @" 
                if(this.InputMode == 1)
                { return parseFloat($('#' + this.EbSid_CtxId).val().replace(/,/g, '')) || 0; }
                else{ return parseFloat($('#' + this.EbSid_CtxId).val()) || 0;}";
            }
            set { }
        }

        [JsonIgnore]
        public override string GetDisplayMemberFromDOMJSfn { get { return @"return $('#' + this.EbSid_CtxId).val();"; } set { } }

        [JsonIgnore]
        public override string OnChangeBindJSFn { get { return @"$('#' + this.EbSid_CtxId).on('keyup change', p1);"; } set { } }

        [OnDeserialized]
        public void OnDeserializedMethod(StreamingContext context)
        {
            this.BareControlHtml = this.GetBareHtml();
            this.BareControlHtml4Bot = this.BareControlHtml;
            if (this.Padding == null)
                this.Padding = new UISides() { Bottom = 7, Left = 10, Top = 7, Right = 10 };
            this.ObjType = this.GetType().Name.Substring(2, this.GetType().Name.Length - 2);
        }

        //[EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl)]
        //[PropertyGroup(PGConstants.EXTENDED)]
        //public int MaxLength { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl)]
        [DefaultPropValue("2")]
        [PropertyGroup(PGConstants.EXTENDED)]
        [Alias("Decimal Places")]
        [HelpText("Number of decimal places")]
        public int DecimalPlaces { get; set; }

        [EnableInBuilder(BuilderType.BotForm)]
        [HideInPropertyGrid]
        public bool IsBasicControl { get => true; }

        //[HideInPropertyGrid]
        //[EnableInBuilder(BuilderType.BotForm)]
        //public override bool IsReadOnly { get => this.IsDisable; }

        [EnableInBuilder(BuilderType.BotForm)]
        public bool AutoIncrement { get; set; }

        //[ProtoBuf.ProtoMember(4)]
        private string PlaceHolder
        {
            get
            {
                //for ( int i=0; i< this.DecimalPlaces; i++)
                //    res += "0";
                return (this.DecimalPlaces == 0) ? "0" : "0." + new String('0', this.DecimalPlaces);
            }
        }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl)]
        [PropertyGroup(PGConstants.EXTENDED)]
        public bool AllowNegative { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl)]
        [Alias("Maximum")]
        [HelpText("Maximum value allowed")]
        [PropertyGroup("Validations")]
        public int MaxLimit { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl)]
        [Alias("Minimum")]
        [HelpText("Minimum value allowed")]
        [PropertyGroup("Validations")]
        public int MinLimit { get; set; }

        //[EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl)]
        //[PropertyGroup(PGConstants.EXTENDED)]
        //public bool IsCurrency { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl)]
        [HideInPropertyGrid]
        public bool AutoCompleteOff { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl)]
        [PropertyGroup(PGConstants.APPEARANCE)]
        [UIproperty]
        [OnChangeUIFunction("Common.CONTROL_ICON")]
        [DefaultPropValue("true")]
        public bool HideInputIcon { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl)]
        [PropertyGroup(PGConstants.APPEARANCE)]
        public bool ShowAddInput { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl)]
        [PropertyGroup(PGConstants.CORE)]
        [OnChangeExec(@"
if (this.TextMode === 4 ){
    pg.ShowProperty('RowsVisible');
}
else {
    pg.HideProperty('RowsVisible');
}
            ")]
        public NumInpMode InputMode { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm)]
        public override bool Index { get; set; }

        //private string MaxLengthString
        //{
        //    get { return ((this.MaxLength > 0) ? "$('#{0}').focus(function() {$(this).select();});".Replace("{0}", this.Name).Replace("{1}", this.Value.ToString()) : string.Empty); }
        //}

        public override VendorDbType GetvDbType(IVendorDbTypes vDbTypes)
        {
            return vDbTypes.Decimal;
        }

        [HideInPropertyGrid]
        [JsonIgnore]
        [EnableInBuilder(BuilderType.SurveyControl)]
        public override string ToolIconHtml { get { return "<b class=\"fa\" style=\"letter-spacing: -0.7px;\">01</b>"; } set { } }

        //public override string GetToolHtml() { return @"<div eb-type='@toolName' class='tool'><b>0-9 </b></i>  @toolName</div>".Replace("@toolName", this.GetType().Name.Substring(2)); }

        public override string GetDesignHtml()
        {
            return GetHtml().RemoveCR().DoubleQuoted();
        }


        public override string GetHtml4Bot()
        {
            return ReplacePropsInHTML(HtmlConstants.CONTROL_WRAPER_HTML4BOT);
        }

        public override string GetBareHtml()
        {
            string html = @"
<div class='input-group' style='width:100%;'>
    @attachedLbl@
    <input type='text' data-ebtype='@datetype@' class='numinput' ui-inp id='@ebsid@' name='@name@' @max@ @min@ value='@value@' @placeHolder autocomplete = '@autoComplete@' data-toggle='tooltip' title='@toolTipText@' style='@width@ @backColor@ @foreColor@ @fontStyle@ display:inline-block;' @required@ @tabIndex@ />
    @AddButton@
</div>"
.Replace("@name@", this.Name)
.Replace("@ebsid@", String.IsNullOrEmpty(this.EbSid_CtxId) ? "@ebsid@" : this.EbSid_CtxId)
.Replace("@toolTipText@", this.ToolTipText)
.Replace("@autoComplete@", this.AutoCompleteOff ? "off" : "on")
.Replace("@value@", "")//"value='" + this.Value + "'")
.Replace("@tabIndex@", "tabindex='" + this.TabIndex + "'")
.Replace("@BackColor@ ", ("background-color:" + ((this.BackColor != null) ? this.BackColor : "@BackColor@ ") + ";"))
.Replace("@ForeColor@ ", "color:" + ((this.ForeColor != null) ? this.ForeColor : "@ForeColor@ ") + ";")
.Replace("@width@", this.ShowAddInput ? "width:calc(100% - 48px);" : "width:100%;")
.Replace("@required@", " required")//(this.Required && !this.Hidden ? " required" : string.Empty))
.Replace("@max@", this.MaxLimit != 0 ? "max='" + this.MaxLimit + "'" : string.Empty)
.Replace("@min@", this.MinLimit != 0 ? "min='" + this.MinLimit + "'" : string.Empty)
.Replace("@placeHolder@", "placeholder='" + this.PlaceHolder + "'")
.Replace("@datetype@", "11")
.Replace("@AddButton@", this.ShowAddInput ? "<span class='numplus-btn'><i aria-hidden='true' class='fa fa-plus'></i></span> <input class='numplus-inp' type='text' />" : string.Empty);
            //.Replace("@fontStyle@", (this.FontSerialized != null) ?
            //                            (" font-family:" + this.FontSerialized.FontFamily + ";" + "font-style:" + this.FontSerialized.Style
            //                            + ";" + "font-size:" + this.FontSerialized.SizeInPoints + "px;")
            //                        : string.Empty)
            html = AddIcon2Html(html);
            return html;
        }

        private string AddIcon2Html(string html)
        {

            string attachedLableHtml = @"<span class='input-group-addon'>@icon@</span>";
            if (this.InputMode == NumInpMode.Currency)
                attachedLableHtml = attachedLableHtml.Replace("@icon@", @"<i class='fa fa-money aria-hidden='true' class='input-group-addon'></i>");
            else if (this.InputMode == NumInpMode.Phone)
                attachedLableHtml = attachedLableHtml.Replace("@icon@", @"<i class='fa fa-phone aria-hidden='true' class='input-group-addon'></i>");
            else if (this.InputMode == NumInpMode.Numeric)
                attachedLableHtml = attachedLableHtml.Replace("@icon@", "<span style='font-size: 11px;font-weight: bold;margin: 0 6px;'>01</span>");

            html = html.Replace("@attachedLbl@", attachedLableHtml);

            return html;
        }

        public override SingleColumn GetSingleColumn(User UserObj, Eb_Solution SoluObj, object Value, bool Default)
        {
            return EbNumeric.GetSingleColumn(this, UserObj, SoluObj, Value);
        }

        public static SingleColumn GetSingleColumn(dynamic _this, User UserObj, Eb_Solution SoluObj, object Value)
        {
            object _formattedData;
            string _displayMember;
            string padding = _this.DecimalPlaces > 0 ? ".".PadRight(_this.DecimalPlaces + 1, '0') : string.Empty;

            if (Value == null)
            {
                _formattedData = 0;
                _displayMember = "0" + padding;
            }
            else
            {
                if (double.TryParse(Convert.ToString(Value), out double _t))
                    _formattedData = _t;
                else
                    throw new FormException($"Invalid numeric value found({_this.Name})", (int)HttpStatusCode.InternalServerError, $"Unable to parse '{Convert.ToString(Value)}' as numeric value for {_this.Name}", "From EbNumeric.GetSingleColumn()");
                _displayMember = string.Format("{0:0" + padding + "}", _formattedData);
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
