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
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace ExpressBase.Objects
{

    public enum NumInpMode
    {
        Numeric = 0,
        Currency = 1,
        Phone = 2,
    }

    [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl)]
    public class EbNumeric : EbControlUI
    {
        public EbNumeric() { }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl)]
        [HideInPropertyGrid]
        public override EbDbTypes EbDbType { get { return EbDbTypes.Decimal; } }

        [JsonIgnore]
        public override string GetValueFromDOMJSfn { get { return @" return parseFloat($('#' + this.EbSid_CtxId).val()) || 0; "; } set { } }

        [OnDeserialized]
        public void OnDeserializedMethod(StreamingContext context)
        {
            this.BareControlHtml = this.GetBareHtml();
            this.BareControlHtml4Bot = this.BareControlHtml;
            this.ObjType = this.GetType().Name.Substring(2, this.GetType().Name.Length - 2);
        }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl)]
        public int MaxLength { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl)]
        [DefaultPropValue("2")]
        [PropertyGroup("Core")]
        [Alias("Decimal Places")]
        [HelpText("Number of decimal places")]
        public int DecimalPlaces { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl)]
        public decimal Value { get; set; }

        [HideInPropertyGrid]
        [EnableInBuilder(BuilderType.BotForm)]
        public override bool IsReadOnly { get => this.ReadOnly; }

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
        [PropertyGroup("Core")]
        public bool AllowNegative { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl)]
        [Alias("Maximum")]
        [HelpText("Maximum value allowed")]
        [PropertyGroup("Core")]
        public int MaxLimit { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl)]
        [Alias("Minimum")]
        [HelpText("Minimum value allowed")]
        [PropertyGroup("Core")]
        public int MinLimit { get; set; }

        //[EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl)]
        //[PropertyGroup("Core")]
        //public bool IsCurrency { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl)]
        [HideInPropertyGrid]
        public bool AutoCompleteOff { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl)]
        [PropertyGroup("Appearance")]
        [UIproperty]
        [OnChangeUIFunction("Common.CONTROL_ICON")]
        public bool ShowIcon { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl)]
        [PropertyGroup("Core")]
        [DefaultPropValue("'SingleLine'")]
        [OnChangeExec(@"
if (this.TextMode === 4 ){
    pg.ShowProperty('RowsVisible');
}
else {
    pg.HideProperty('RowsVisible');
}
            ")]
        public NumInpMode InputMode { get; set; }

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
        public override string ToolIconHtml { get { return "<b style='letter-spacing: -0.7px;'>01</b>"; } set { } }

        //public override string GetToolHtml() { return @"<div eb-type='@toolName' class='tool'><b>0-9 </b></i>  @toolName</div>".Replace("@toolName", this.GetType().Name.Substring(2)); }

        public override string GetDesignHtml()
        {
            return GetHtmlHelper(RenderMode.Developer).RemoveCR().DoubleQuoted();
        }

        public override string GetHtml()
        {
            return GetHtmlHelper(RenderMode.User);
        }

        //        public override string GetBareHtml()
        //        {
        //            return @" 
        //        <div class='input-group' style='width:100%;'>
        //                <span style='font-size: @fontSize@' class='input-group-addon'><i class='fa fa-sort-numeric-asc' aria-hidden='true'></i></span>   
        //                <input type='text' class='numinput' ui-inp data-ebtype='@datetype@' id='@name@' name='@name@' data-toggle='tooltip' style=' width:100%; display:inline-block;'/>
        //        </div>"
        //.Replace("@name@", this.Name)
        //.Replace("@datetype@", "11");
        //        }


        public override string GetHtml4Bot()
        {
            return ReplacePropsInHTML(HtmlConstants.CONTROL_WRAPER_HTML4BOT);
        }

        public override string GetBareHtml()
        {
            string html = @"
        <div class='input-group' style='width:100%;'>
            <span class='input-group-addon'> @attachedLbl@ </span>
            <input type='text' data-ebtype='@datetype@' class='numinput' ui-inp id='@ebsid@' name='@name@' @max@ @min@ value='@value@' @placeHolder autocomplete = '@autoComplete@' data-toggle='tooltip' title='@toolTipText@' style=' width:100%; @backColor@ @foreColor@ @fontStyle@ display:inline-block; @readOnlyString@ @required@ @tabIndex@ />
        </div>"
.Replace("@name@", this.Name)
.Replace("@ebsid@", this.IsRenderMode && this.IsDynamicTabChild ? "@" + this.EbSid_CtxId + "_ebsid@" : (String.IsNullOrEmpty(this.EbSid_CtxId) ? "@ebsid@" : this.EbSid_CtxId))
.Replace("@toolTipText@", this.ToolTipText)
.Replace("@autoComplete@", this.AutoCompleteOff ? "off" : "on")
.Replace("@value@", "")//"value='" + this.Value + "'")
.Replace("@tabIndex@", "tabindex='" + this.TabIndex + "'")
    .Replace("@BackColor@ ", ("background-color:" + ((this.BackColor != null) ? this.BackColor : "@BackColor@ ") + ";"))
    .Replace("@ForeColor@ ", "color:" + ((this.ForeColor != null) ? this.ForeColor : "@ForeColor@ ") + ";")
.Replace("@required@", " required")//(this.Required && !this.Hidden ? " required" : string.Empty))
.Replace("@readOnlyString@", this.ReadOnlyString)
.Replace("@max@", this.MaxLimit != 0 ? "max='" + this.MaxLimit + "'" : string.Empty)
.Replace("@min@", this.MinLimit != 0 ? "min='" + this.MinLimit + "'" : string.Empty)
.Replace("@placeHolder@", "placeholder='" + this.PlaceHolder + "'")
.Replace("@datetype@", "11");
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

        private string GetHtmlHelper(RenderMode mode)
        {
            string EbCtrlHTML = HtmlConstants.CONTROL_WRAPER_HTML4WEB
               .Replace("@LabelForeColor ", "color:" + (LabelForeColor ?? "@LabelForeColor ") + ";")
               .Replace("@LabelBackColor ", "background-color:" + (LabelBackColor ?? "@LabelBackColor ") + ";");

            return ReplacePropsInHTML(EbCtrlHTML);
        }
    }
}
