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
        SingleLine,
        Email,
        Password,
        Color
    }

    [ProtoBuf.ProtoContract]
    [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog)]
    public class EbTextBox : EbControl
    {
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
        [PropertyGroup("Behavior")]
        [PropertyEditor(PropertyEditorType.DropDown)]
        [OnChangeExec(@"
if (this.TextTransform === 'UPPERCASE' ){
    pg.HideProperty('Text');
    console.log('HideProperty');
}
else {
    pg.ShowProperty('Text');
}
            ")]
        public TextTransform TextTransform { get; set; }
        
        [EnableInBuilder(BuilderType.WebForm)]
        [PropertyGroup("Behavior")]
        public TextMode TextMode { get; set; }
        
        [EnableInBuilder(BuilderType.WebForm)]
        [PropertyGroup(@"Behavior")]
        [HelpText("specifies a short hint that describes the expected value of an input field (e.g. a sample value or a short description of the expected format)")]
        public string PlaceHolder { get; set; }

        [EnableInBuilder(BuilderType.WebForm)]
        [PropertyGroup("Appearance")]
        public string Text { get; set; }
        
        
        [PropertyGroup("Behavior")]
        [EnableInBuilder(BuilderType.WebForm)]
        public bool AutoCompleteOff { get; set; }
        
        [PropertyGroup("Behavior")]
        public string MaxDateExpression { get; set; }
        
        [PropertyGroup("Behavior")]
        public string MinDateExpression { get; set; }

        [EnableInBuilder(BuilderType.WebForm)]
        [PropertyGroup(@"Behavior")]
        [PropertyEditor(PropertyEditorType.JS)]
        public string Validation { get; set; }

        //[ProtoBuf.ProtoMember(9)]
        //[Description("Identity")]
        //public override string Name { get; set; }

        //[ProtoBuf.ProtoMember(10)]
        //[Description("Identity")]
        //public override string Label { get; set; }

        public EbTextBox() { }

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

        public override string GetDesignHtml()
        {
            return GetHtmlHelper(RenderMode.Developer).RemoveCR().DoubleQuoted();
        }

        public override string GetHtml()
        {
            return GetHtmlHelper(RenderMode.User);
        }

        private string GetHtmlHelper(RenderMode mode)
        {
            return @"

<div class='Eb-ctrlContainer' Ctype='TextBox' style='@HiddenString '>
    <span id='@nameLbl' style='@LabelBackColor @LabelForeColor '> @Label  </span>
        <div  class='input-group' style='width: 100%;'>
            @attachedLbl
            <input type='@TextMode '  id='@name ' name='@name ' autocomplete = '@AutoCompleteOff ' data-toggle='tooltip' title='@ToolTipText ' 
@tabIndex @MaxLength  style='width:100%; height:@heightpx; @BackColor @ForeColor display:inline-block; @fontStyle @ReadOnlyString  @Required  @PlaceHolder  @Text  @TabIndex  />
        </div>
    <span class='helpText'> @HelpText </span>
</div>"
.Replace("@Name ", (this.Name != null) ? this.Name : "@Name ")
.Replace("@MaxLength ", "maxlength='" + ( (this.MaxLength > 0) ? this.MaxLength.ToString() : "@MaxLength" ) + "'")
.Replace("@TextMode ", this.TextMode.ToString().ToLower())
.Replace("@HiddenString ", this.HiddenString)
.Replace("@Required ", (this.Required && !this.Hidden ? " required" : string.Empty))
.Replace("@ReadOnlyString ", this.ReadOnlyString)
.Replace("@ToolTipText ", this.ToolTipText)
.Replace("@PlaceHolder ", "placeholder='" + this.PlaceHolder + "'")
.Replace("@TabIndex ", "tabindex='" + this.TabIndex + "' ")
.Replace("@AutoCompleteOff ", (this.AutoCompleteOff || this.TextMode.ToString().ToLower() == "password") ? "off" : "on")

    .Replace("@LabelForeColor ", "color:" + ((this.LabelForeColor != null) ? this.LabelForeColor : "@LabelForeColor ") + ";")
    .Replace("@LabelBackColor ", "background-color:" + ((this.LabelBackColor != null) ? this.LabelBackColor : "@LabelBackColor ") + ";")
    .Replace("@BackColor ", ("background-color:" + ((this.BackColor != null) ? this.BackColor : "@BackColor ") + ";"))
    .Replace("@ForeColor ", "color:" + ((this.ForeColor != null) ? this.ForeColor : "@ForeColor ") + ";")
    .Replace("@HelpText ", ((this.HelpText != null) ? this.HelpText : "@HelpText "))
    .Replace("@Label ", ((this.Label != null) ? this.Label : "@Label "))
    .Replace("@Text ", "value='" + ((this.Text != null) ? this.Text : "@Text ") + "' ")

.Replace("@attachedLbl", (this.TextMode.ToString() != "SingleLine") ?
                                (
                                    "<i class='fa fa-$class input-group-addon' aria-hidden='true'"
                                    + (
                                        (this.FontFamily != null) ?
                                            ("style='font-size:" + this.FontSize + "px;'")
                                        : string.Empty
                                      )
                                    + "class='input-group-addon'></i>"
                                )
                                .Replace("$class", (this.TextMode.ToString() == "Email") ?
                                                            ("envelope")
                                                        : (this.TextMode.ToString() == "Password") ?
                                                            "key"
                                                        : ("eyedropper")
                                )
                        : string.Empty);
        }
    }

    public enum RenderMode
    {
        Developer,
        User
    }
}
