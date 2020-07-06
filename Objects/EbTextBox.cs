using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;
using Newtonsoft.Json;
using ExpressBase.Common.Objects.Attributes;
using ServiceStack.Pcl;
using ExpressBase.Common.Objects;
using ExpressBase.Common.Extensions;
using System.Runtime.Serialization;
using ExpressBase.Common.Structures;
using ExpressBase.Objects.Helpers;
using ExpressBase.Common;
using ServiceStack;
using ExpressBase.Objects.ServiceStack_Artifacts;
using ExpressBase.Common.Constants;

namespace ExpressBase.Objects
{
    public enum TextTransform
    {
        lowercase = 1,
        Normal = 0,
        UPPERCASE = 2,
    }

    public enum TextMode
    {
        Color = 3,
        Email = 2,
        MultiLine = 4,
        Name = 5,
        Password = 1,
        SingleLine = 0,
    }

    [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl)]
    public class EbTextBox : EbControlUI, IEbInputControls
    {
        public EbTextBox() { }


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

        public override string UIchangeFns
        {
            get
            {
                return @"EbTextBox = {
                }";
            }
        }

        [OnDeserialized]
        public void OnDeserializedMethod(StreamingContext context)
        {
            this.BareControlHtml = this.GetBareHtml();
            this.BareControlHtml4Bot = this.BareControlHtml;
            this.ObjType = this.GetType().Name.Substring(2, this.GetType().Name.Length - 2);
            if (this.Padding == null)
                this.Padding = new UISides() { Bottom = 7, Left = 10, Top = 7, Right = 10 };
        }

        [JsonIgnore]
        public override string DisableJSfn
        {
            get
            {
                return @"
this.__IsDisable = true; 
$('#cont_' + this.EbSid_CtxId + ' *')
.attr('disabled', 'disabled').css('pointer-events', 'none')
.find('[ui-inp]').css('background-color', '#f3f3f3');
$('#cont_' + this.EbSid_CtxId + ' .ctrl-cover').css('pointer-events', 'inherit')
.find('[ui-inp]').css('pointer-events', 'inherit')";
            }
            set { }
        }

        [JsonIgnore]
        public override string EnableJSfn
        {
            get
            {
                return @"
this.__IsDisable = false;
$('#cont_' + this.EbSid_CtxId + ' *')
.removeAttr('disabled').css('pointer-events', 'inherit')
.find('[ui-inp]').css('background-color', '#fff');";
            }
            set { }
        }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.UserControl)]
        [HelpText("To limit number of charecters")]
        [PropertyGroup(PGConstants.EXTENDED)]
        [PropertyEditor(PropertyEditorType.Number)]
        [PropertyPriority(99)]
        [OnChangeExec(@"
if (this.MaxLength <= 10 ){
    pg.MakeReadOnly('PlaceHolder');
}
else {
    pg.MakeReadWrite('PlaceHolder');
}
            ")]
        public int MaxLength { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.UserControl, BuilderType.BotForm)]
        [Alias("Text Transform")]
        [DefaultPropValue("'UPPERCASE'")]
        [PropertyGroup(PGConstants.CORE)]
        [PropertyEditor(PropertyEditorType.DropDown)]
        [OnChangeExec(@"
if (this.TextTransform === 'UPPERCASE' ){
    pg.HideProperty('Text');
}
else {
    pg.ShowProperty('Text');
}
            ")]
        public TextTransform TextTransform { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl)]
        [PropertyGroup(PGConstants.CORE)]
        [DefaultPropValue("'SingleLine'")]
        [OnChangeExec(@"
if (this.TextMode === 4 ){
    pg.ShowProperty('RowsVisible');
}
else {
    pg.HideProperty('RowsVisible');
}
            ")]
        public TextMode TextMode { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl)]
        [PropertyGroup(PGConstants.CORE)]
        [DefaultPropValue("3")]
        public int RowsVisible { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl)]
        [PropertyGroup(PGConstants.HELP)]
        [PropertyPriority(8)]
        [HelpText("specifies a short hint that describes the expected value of an input field (e.g. a sample value or a short description of the expected format)")]
        public string PlaceHolder { get; set; }

        //[PropertyGroup("Behavior")]
        //[EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl)]
        //public string MaskPattern { get; set; }

        //[EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl)]
        //[PropertyGroup(PGConstants.APPEARANCE)]
        //[PropertyEditor(PropertyEditorType.MultiLanguageKeySelector)]
        public string LabelT { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl)]
        [PropertyGroup("test")]
        [MetaOnly]
        public string MetaOnly { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl)]
        [PropertyGroup(PGConstants.APPEARANCE)]
        [PropertyEditor(PropertyEditorType.FontSelector)]
        [HideInPropertyGrid]
        public string Only4Dev { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl)]
        [PropertyGroup(PGConstants.APPEARANCE)]
        [EbRequired]
        [Unique]
        public string Text { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl)]
        [PropertyGroup("Behavior")]
        public bool AutoCompleteOff { get; set; }

        [EnableInBuilder(BuilderType.WebForm)]
        [PropertyGroup(PGConstants.EXTENDED)]
        public bool AutoSuggestion { get; set; }

        [EnableInBuilder(BuilderType.WebForm)]
        [HideInPropertyGrid]
        public string TableName { get; set; }

        [EnableInBuilder(BuilderType.WebForm)]
        [HideInPropertyGrid]
        public List<string> Suggestions { get; set; }

        [PropertyGroup("Behavior")]
        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl)]
        [HideInPropertyGrid]
        public string MaxDateExpression { get; set; }

        [PropertyGroup("Behavior")]
        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl)]
        [HideInPropertyGrid]
        public string MinDateExpression { get; set; }

        //[ProtoBuf.ProtoMember(9)]
        //[Description("Core")]
        //public override string Name { get; set; }

        //[ProtoBuf.ProtoMember(10)]
        //[Description("Core")]
        //public override string Label { get; set; }
        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl)]
        [HideInPropertyGrid]
        public override EbDbTypes EbDbType { get { return EbDbTypes.String; } }

		//[HideInPropertyGrid]
		//[EnableInBuilder(BuilderType.BotForm)]
		//public override bool IsReadOnly { get => this.IsDisable; }

		//[EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl)]
		//[PropertyEditor(PropertyEditorType.JS)]
		//public string OnChangeExe { get; set; }

		[EnableInBuilder(BuilderType.BotForm)]
		[HideInPropertyGrid]
		public bool IsBasicControl { get => true; }

		private string TextTransformString
        {
            get { return (((int)this.TextTransform > 0) ? "$('#{0}').keydown(function(event) { textTransform(this, {1}); }); $('#{0}').on('paste', function(event) { textTransform(this, {1}); });".Replace("{0}", this.Name).Replace("{1}", ((int)this.TextTransform).ToString()) : string.Empty); }
        }


        public override void SetData(object value)
        {
            this.Text = (value != null) ? value.ToString() : string.Empty;
        }

        public override object GetData()
        {
            return this.Text;
        }

        [HideInPropertyGrid]
        [JsonIgnore]
        public override string ToolIconHtml { get { return "<i class='fa fa-i-cursor'></i>"; } set { } }

        //public override string GetToolHtml()
        //{
        //    return @"<div eb-type='@toolName' class='tool'><i class='fa fa-i-cursor'></i>  @toolName</div>".Replace("@toolName", this.GetType().Name.Substring(2));
        //}


        public void InitFromDataBase(JsonServiceClient ServiceClient)
        {
            if (this.AutoSuggestion)
            {
                var result = ServiceClient.Get<GetDistinctValuesResponse>(new GetDistinctValuesRequest { TableName = this.TableName, ColumnName = this.Name });
                this.Suggestions = result.Suggestions;
            }

        }
        public override string GetWrapedCtrlHtml4bot()
        {

            return ReplacePropsInHTML(HtmlConstants.CONTROL_WRAPER_HTML4BOT);

        }

        //control html definition- for builder side
        public override string GetDesignHtml()
        {
            return GetHtml().RemoveCR().DoubleQuoted();
        }

        //control html definition - for rendering side(with wrapper divs)
        public override string GetHtml()
        {
            return GetHtmlHelper(RenderMode.User);
        }
        public override string GetHtml4Bot()
        {
            return GetWrapedCtrlHtml4bot();

        }

        public string TexboxHtml
        {
            get
            {
                string html = @"
            @attachedLbl@
            <input type='@TextMode '  data-ebtype='@data-ebtype@' ui-inp id='@ebsid@' name='@name@' @AutoCompleteOff@ ' data-toggle='tooltip' data-placement='top' title='@ToolTipText@' 
@TabIndex@ @MaxLength@  style='width:100%; height:@heightpx; @BackColor @ForeColor display:inline-block; @fontStyle'  @Required  @PlaceHolder  @Text@  />
        @attachedLblClose@"
 .Replace("@ebsid@", this.IsRenderMode && this.IsDynamicTabChild ? "@" + this.EbSid_CtxId + "_ebsid@" : (String.IsNullOrEmpty(this.EbSid_CtxId) ? "@ebsid@" : this.EbSid_CtxId))
 .Replace("@name@", this.EbSid_CtxId)
 .Replace("@data-ebtype@", "16")//( (int)this.EbDateType ).ToString())
 .Replace("@MaxLength@", (this.MaxLength > 0) ? "maxlength='" + this.MaxLength.ToString() + "'" : "")
 .Replace("@TextMode ", (this.TextMode == TextMode.SingleLine) ? "text" : this.TextMode.ToString().ToLower())
 .Replace("@Required ", (this.Required && !this.Hidden ? " required" : string.Empty))
 .Replace("@PlaceHolder ", "placeholder='" + this.PlaceHolder + "'")
 .Replace("@TabIndex@ ", "tabindex='" + this.TabIndex + "' ")
 .Replace("@AutoCompleteOff@ ", " autocomplete = '" + ((this.AutoCompleteOff || this.TextMode.ToString().ToLower() == "password") ? "off" : "on") + "'")
     .Replace("@BackColor ", ("background-color:" + ((this.BackColor != null) ? this.BackColor : "@BackColor ") + ";"))
     .Replace("@ForeColor ", "color:" + ((this.ForeColor != null) ? this.ForeColor : "@ForeColor ") + ";")
     .Replace("@Text@ ", "value='" + ((this.Text != null) ? this.Text : "") + "' ")

 .Replace("@attachedLblClose@", (this.TextMode == TextMode.SingleLine) ? string.Empty : "</div>");

                html = AddIcon2Html(html);

                return html;
            }
            set { }
        }

        private string AddIcon2Html(string html)
        {
            if (this.TextMode != TextMode.SingleLine)
            {
                string attachedLableHtml = @"<div  class='input-group' style='width: 100%;'>
                                    <span class='input-group-addon' onclick='$(\'#@ebsid@\').click()'><i class='fa fa-$class aria-hidden='true'
                                             class='input-group-addon'></i></span>";
                if (this.TextMode == TextMode.Email)
                    attachedLableHtml = attachedLableHtml.Replace("$class", "envelope");
                else if (this.TextMode == TextMode.Password)
                    attachedLableHtml = attachedLableHtml.Replace("$class", "key");
                else if (this.TextMode == TextMode.Color)
                    attachedLableHtml = attachedLableHtml.Replace("$class", "phone");
                else if (this.TextMode == TextMode.Name)
                    attachedLableHtml = attachedLableHtml.Replace("$class", "user");
                //else if (this.TextMode == TextMode.Link)
                //    attachedLableHtml = attachedLableHtml.Replace("$class", "link");

                html = html.Replace("@attachedLbl@", attachedLableHtml);
            }
            else
                html = html.Replace("@attachedLbl@", string.Empty);
            return html;
        }

        public string TextareaHtml
        {
            get
            {
                return @"
            <textarea id='@ebsid@' class='eb-textarea' ui-inp name='@name@' rows='@RowsVisible@' '@AutoCompleteOff@' data-toggle='tooltip'  data-placement='top' title='@ToolTipText@' 
                @tabIndex@ @MaxLength@  style='width:100%;resize: none;' @Required@  @PlaceHolder@  @Text@  @TabIndex></textarea>"
.Replace("@name@", this.Name)
.Replace("@ebsid@", this.IsRenderMode && this.IsDynamicTabChild ? "@" + this.EbSid_CtxId + "_ebsid@" : (String.IsNullOrEmpty(this.EbSid_CtxId) ? "@ebsid@" : this.EbSid_CtxId))
.Replace("@MaxLength@", "maxlength='" + ((this.MaxLength > 0) ? this.MaxLength.ToString() : "@MaxLength") + "'")
.Replace("@Required@", (this.Required && !this.Hidden ? " required" : string.Empty))
.Replace("@PlaceHolder@", "placeholder='" + this.PlaceHolder + "'")
.Replace("@TabIndex@", "tabindex='" + this.TabIndex + "' ")
.Replace("@AutoCompleteOff@ ", "autocomplete = " + ((this.AutoCompleteOff || this.TextMode.ToString().ToLower() == "password") ? "off" : "on") + "'")
    .Replace("@BackColor@", ("background-color:" + ((this.BackColor != null) ? this.BackColor : "@BackColor ") + ";"))
    .Replace("@ForeColor@", "color:" + ((this.ForeColor != null) ? this.ForeColor : "@ForeColor ") + ";")
    .Replace("@Text@ ", "value='" + ((this.Text != null) ? this.Text : "@Text@") + "' ")
    .Replace("@RowsVisible@", (this.RowsVisible != 0) ? this.RowsVisible.ToString() : "3");
            }
            set { }
        }


        //control html definition - for rendering side(without wrapper divs)
        public override string GetBareHtml()
        {

            string Html = string.Empty;
            if (this.TextMode == TextMode.MultiLine)
                Html = this.TextareaHtml;
            else
                Html = this.TexboxHtml;
            return Html
                .Replace("@ToolTipText@", this.ToolTipText ?? String.Empty);
        }

        private string GetHtmlHelper(RenderMode mode)
        {
            string EbCtrlHTML = HtmlConstants.CONTROL_WRAPER_HTML4WEB
               .Replace("@LabelForeColor ", "color:" + (LabelForeColor ?? "@LabelForeColor ") + ";")
               .Replace("@LabelBackColor ", "background-color:" + (LabelBackColor ?? "@LabelBackColor ") + ";");

            return ReplacePropsInHTML(EbCtrlHTML);
        }

        //        private string GetHtmlHelper(RenderMode mode)
        //        {
        //            return @"
        //<div id='cont_@name@  ' class='Eb-ctrlContainer' Ctype='TextBox' eb-hidden='@isHidden@'>
        //    <div class='eb-ctrl-label' ui-label id='@name@Lbl' style='@LabelBackColor@ @LabelForeColor@ '> @Label@  </div>
        //       @barehtml@
        //    <span ui-helptxt class='helpText'> @HelpText@ </span>
        //</div>"
        //.Replace("@barehtml@", this.GetBareHtml())
        //.Replace("@name@", this.Name)
        //.Replace("@isHidden@", this.Hidden.ToString())
        //.Replace("@ToolTipText ", this.ToolTipText)

        ////.Replace("@name ", (this.Name != null) ? this.Name : "@name ")
        //.Replace("@LabelForeColor@ ", "color:" + ((this.LabelForeColor != null) ? this.LabelForeColor : "@LabelForeColor@ ") + ";")
        //.Replace("@LabelBackColor@ ", "background-color:" + ((this.LabelBackColor != null) ? this.LabelBackColor : "@LabelBackColor@ ") + ";")
        //.Replace("@HelpText@ ", ((this.HelpText != null) ? this.HelpText : "@HelpText@ "))
        //.Replace("@Label@ ", this.Label ?? "@Label@ ");
        //        }
    }

    public interface IEbInputControls
    {
        UISides Padding { get; set; }

        EbFont FontStyle { get; set; }
    }

    public enum RenderMode
    {
        Developer,
        User
    }
}
