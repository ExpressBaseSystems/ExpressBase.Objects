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

namespace ExpressBase.Objects
{
    public enum TextTransform
    {
        Normal,
        lowercase,
        UPPERCASE,
    }

    public enum TextMode
    {
        SingleLine = 2,
        Email = 0,
        Password = 1,
        Color = 3,
        MultiLine = 4
    }

    [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm)]
    public class EbTextBox : EbControlUI
    {
        public EbTextBox() { }

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
            this.ObjType = this.GetType().Name.Substring(2, this.GetType().Name.Length - 2);
        }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog)]
        [HelpText("To limit number of charecters")]
        [PropertyGroup("Behavior")]
        [PropertyEditor(PropertyEditorType.Number)]
        [OnChangeExec(@"
if (this.MaxLength <= 10 ){
    pg.MakeReadOnly('PlaceHolder');
}
else {
    pg.MakeReadWrite('PlaceHolder');
}
            ")]
        public int MaxLength { get; set; }

        [EnableInBuilder(BuilderType.WebForm)]
        [Alias("TextTransform-Alias")]
        [DefaultPropValue("'UPPERCASE'")]
        [PropertyGroup("Behavior")]
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

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm)]
        [PropertyGroup("Behavior")]
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

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm)]
        [PropertyGroup("Behavior")]
        [DefaultPropValue("5")]
        public int RowsVisible { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm)]
        [PropertyGroup(@"Behavior")]
        [HelpText("specifies a short hint that describes the expected value of an input field (e.g. a sample value or a short description of the expected format)")]
        public string PlaceHolder { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm)]
        [PropertyGroup("Appearance")]
        [PropertyEditor(PropertyEditorType.MultiLanguageKeySelector)]
        public string LabelT { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm)]
        [PropertyGroup("Appearance")]
        [PropertyEditor(PropertyEditorType.FontSelector)]
        public string FontFamilyT { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm)]
        [PropertyGroup("test")]
        [MetaOnly]
        public string MetaOnly { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm)]
        [PropertyGroup("Appearance")]
        [PropertyEditor(PropertyEditorType.FontSelector)]
        //[HideForUser]
        public string Only4Dev { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm)]
        [PropertyGroup("Appearance")]
        [EbRequired]
        [Unique]
        public string Text { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm)]
        [PropertyGroup("Behavior")]
        public bool AutoCompleteOff { get; set; }

        [PropertyGroup("Behavior")]
        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm)]
        public string MaxDateExpression { get; set; }

        [PropertyGroup("Behavior")]
        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm)]
        public string MinDateExpression { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm)]
        [PropertyGroup(@"Behavior")]
        [PropertyEditor(PropertyEditorType.JS)]
        public string Validation { get; set; }

        //[ProtoBuf.ProtoMember(9)]
        //[Description("Identity")]
        //public override string Name { get; set; }

        //[ProtoBuf.ProtoMember(10)]
        //[Description("Identity")]
        //public override string Label { get; set; }
        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm)]
        public override EbDbTypes EbDbType { get { return EbDbTypes.String; } }

        [HideInPropertyGrid]
        [EnableInBuilder(BuilderType.BotForm)]
        public override bool IsReadOnly { get => this.ReadOnly; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm)]
        [PropertyEditor(PropertyEditorType.JS)]
        public string OnChangeExe { get; set; }

        public override string GetHead()
        {
            return (((!this.Hidden) ? this.UniqueString + this.RequiredString : string.Empty) + @"".Replace("{0}", this.Name));
        }

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

        public override string GetToolHtml()
        {
            return @"<div eb-type='@toolName' class='tool'><i class='fa fa-i-cursor'></i>  @toolName</div>".Replace("@toolName", this.GetType().Name.Substring(2));
        }

        public override string GetWrapedCtrlHtml4bot()
        {
            return @"
<div class='Eb-ctrlContainer iw-mTrigger' ctype='TextBox'  eb-type='TextBox'>
   <div class='msg-cont'>
      <div class='bot-icon'></div>
      <div class='msg-cont-bot'>
         <div class='msg-wraper-bot'>
            @Label@
            <div class='msg-time'>3:44pm</div>
         </div>
      </div>
   </div>
   <div class='msg-cont' for='TextBox1' form='LeaveJS'>
      <div class='msg-cont-bot'>
         <div class='msg-wraper-bot' style='border: none; background-color: transparent; width: 99%; padding-right: 3px;'>
            <div class='chat-ctrl-cont'>
               <div class='ctrl-wraper'>
                    @barehtml@
               </div>
            </div>
         </div>
      </div>
   </div>
</div>"
.Replace("@barehtml@", this.GetBareHtml())
.RemoveCR().DoubleQuoted();
        }

        public override string GetDesignHtml()
        {
            return GetHtml().RemoveCR().DoubleQuoted();
        }

        public override string GetHtml()
        {
            return GetHtmlHelper(RenderMode.User);
        }

        public string TexboxHtml
        {
            get
            {
                return @"
            @attachedLbl@
            <input type='@TextMode '  data-ebtype='@data-ebtype@' ui-inp id='@ebsid@' name='@name@' autocomplete = '@AutoCompleteOff ' data-toggle='tooltip' title='@ToolTipText ' 
@tabIndex @MaxLength  style='width:100%; height:@heightpx; @BackColor @ForeColor display:inline-block; @fontStyle @ReadOnlyString  @Required  @PlaceHolder  @Text  @TabIndex  />
        @attachedLblClose@"
.Replace("@ebsid@", this.EbSid_CtxId)
.Replace("@name@", this.EbSid_CtxId)
.Replace("@data-ebtype@", "16")//( (int)this.EbDateType ).ToString())
.Replace("@MaxLength ", "maxlength='" + ((this.MaxLength > 0) ? this.MaxLength.ToString() : "@MaxLength") + "'")
.Replace("@TextMode ", (this.TextMode == TextMode.SingleLine) ? "text" : this.TextMode.ToString().ToLower())
.Replace("@Required ", (this.Required && !this.Hidden ? " required" : string.Empty))
.Replace("@ReadOnlyString ", this.ReadOnlyString)
.Replace("@PlaceHolder ", "placeholder='" + this.PlaceHolder + "'")
.Replace("@TabIndex ", "tabindex='" + this.TabIndex + "' ")
.Replace("@AutoCompleteOff ", (this.AutoCompleteOff || this.TextMode.ToString().ToLower() == "password") ? "off" : "on")
    .Replace("@BackColor ", ("background-color:" + ((this.BackColor != null) ? this.BackColor : "@BackColor ") + ";"))
    .Replace("@ForeColor ", "color:" + ((this.ForeColor != null) ? this.ForeColor : "@ForeColor ") + ";")
    .Replace("@Text ", "value='" + ((this.Text != null) ? this.Text : "@Text ") + "' ")

.Replace("@attachedLblClose@", (this.TextMode == TextMode.SingleLine) ? string.Empty : "</div>")
.Replace("@attachedLbl@", (this.TextMode != TextMode.SingleLine) ?
                                (
                                    @"<div  class='input-group' style='width: 100%;'>
                                        <span class='input-group-addon' onclick='$(\'#@ebsid@\').click()'><i class='fa fa-$class aria-hidden='true'"
                                        + (
                                            (this.FontFamily != null) ?
                                                ("style='font-size:" + this.FontSize + "px;'")
                                            : string.Empty
                                          )
                                        + "class='input-group-addon'></i></span>"
                                )
                                .Replace("$class", (this.TextMode == TextMode.Email) ?
                                                            ("envelope")
                                                        : (this.TextMode == TextMode.Password) ?
                                                            "key"
                                                        : ("eyedropper")
                                )
                        : string.Empty);
            }
            set { }
        }

        public string TextareaHtml
        {
            get
            {
                return @"
            <textarea id='@name@' name='@name@' rows='@RowsVisible@' autocomplete = '@AutoCompleteOff ' data-toggle='tooltip' title='@ToolTipText ' 
                @tabIndex @MaxLength  style='width:100%; height:@heightpx; @BackColor @ForeColor display:inline-block; @fontStyle @ReadOnlyString  @Required  @PlaceHolder  @Text  @TabIndex></textarea>"
.Replace("@name@", this.Name)
.Replace("@ebsid@", this.EbSid_CtxId)
.Replace("@MaxLength ", "maxlength='" + ((this.MaxLength > 0) ? this.MaxLength.ToString() : "@MaxLength") + "'")
.Replace("@Required ", (this.Required && !this.Hidden ? " required" : string.Empty))
.Replace("@ReadOnlyString ", this.ReadOnlyString)
.Replace("@PlaceHolder ", "placeholder='" + this.PlaceHolder + "'")
.Replace("@TabIndex ", "tabindex='" + this.TabIndex + "' ")
.Replace("@AutoCompleteOff ", (this.AutoCompleteOff || this.TextMode.ToString().ToLower() == "password") ? "off" : "on")
    .Replace("@BackColor ", ("background-color:" + ((this.BackColor != null) ? this.BackColor : "@BackColor ") + ";"))
    .Replace("@ForeColor ", "color:" + ((this.ForeColor != null) ? this.ForeColor : "@ForeColor ") + ";")
    .Replace("@Text ", "value='" + ((this.Text != null) ? this.Text : "@Text ") + "' ")
    .Replace("@RowsVisible@", (this.RowsVisible != 0) ? this.RowsVisible.ToString() : "5");
            }
            set { }
        }

        public override string GetBareHtml()
        {
            if (this.TextMode == TextMode.MultiLine)
                return this.TextareaHtml;
            else
                return this.TexboxHtml;
        }

        private string GetHtmlHelper(RenderMode mode)
        {
            return (
                HtmlConstants.CONTROL_WRAPER_HTML4WEB
.Replace("@barehtml@", this.GetBareHtml())
.Replace("@name@", this.Name)
.Replace("@ebsid@", this.EbSid_CtxId)
.Replace("@type@", this.ObjType))

    .Replace("@LabelForeColor ", "color:" + (LabelForeColor ?? "@LabelForeColor ") + ";")
    .Replace("@LabelBackColor ", "background-color:" + (LabelBackColor ?? "@LabelBackColor ") + ";")
    .Replace("@HelpText@ ", (HelpText ?? ""))
    .Replace("@Label@ ", (Label ?? ""));
        }

        //        private string GetHtmlHelper(RenderMode mode)
        //        {
        //            return @"
        //<div id='cont_@name@  ' class='Eb-ctrlContainer' Ctype='TextBox' style='@HiddenString '>
        //    <div class='eb-ctrl-label' ui-label id='@name@Lbl' style='@LabelBackColor@ @LabelForeColor@ '> @Label@  </div>
        //       @barehtml@
        //    <span ui-helptxt class='helpText'> @HelpText@ </span>
        //</div>"
        //.Replace("@barehtml@", this.GetBareHtml())
        //.Replace("@name@", this.Name)
        //.Replace("@HiddenString ", this.HiddenString)
        //.Replace("@ToolTipText ", this.ToolTipText)

        ////.Replace("@name ", (this.Name != null) ? this.Name : "@name ")
        //.Replace("@LabelForeColor@ ", "color:" + ((this.LabelForeColor != null) ? this.LabelForeColor : "@LabelForeColor@ ") + ";")
        //.Replace("@LabelBackColor@ ", "background-color:" + ((this.LabelBackColor != null) ? this.LabelBackColor : "@LabelBackColor@ ") + ";")
        //.Replace("@HelpText@ ", ((this.HelpText != null) ? this.HelpText : "@HelpText@ "))
        //.Replace("@Label@ ", this.Label ?? "@Label@ ");
        //        }
    }

    public enum RenderMode
    {
        Developer,
        User
    }
}
