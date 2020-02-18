using ExpressBase.Common;
using ExpressBase.Common.Constants;
using ExpressBase.Common.Extensions;
using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using ExpressBase.Objects.Objects.DVRelated;
using Newtonsoft.Json;
using Org.BouncyCastle.Asn1.Cms;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ExpressBase.Objects
{

    [EnableInBuilder(BuilderType.BotForm, BuilderType.WebForm, BuilderType.UserControl, BuilderType.DashBoard)]
    [HideInToolBox]
    public class EbDataLabel : EbControlUI
    {

        public EbDataLabel() {}

        [OnDeserialized]
        public void OnDeserializedMethod(StreamingContext context)
        {
            this.BareControlHtml = this.GetBareHtml();
            this.ObjType = this.GetType().Name.Substring(2, this.GetType().Name.Length - 2);
        }

       

        public override string UIchangeFns
        {
            get
            {
                return @"EbDataLabel = {
                DescriptionLabel : function(elementId, props) {
                 $(`#cont_${elementId} .eb-des-label`).text(props.Description);
                },
                Style4DataLabel : function(elementId, props) {
                let styleVar=  getKeyByVal(EbEnums.Align , props.TextAlign.toString());
                    if(styleVar === 'Right'){styleVar='flex-end'}
                    else if(styleVar === 'Left'){styleVar='flex-start'}
                $(`#cont_${elementId} .ctrl-cover`).css(`align-items`, styleVar);
                },
                Style4PlaceHolder : function(elementId , props){
                 $(`#cont_${elementId} .data-dynamic-label`).css(getEbFontStyleObject(props.PlaceHolderFont));
                }
                }";
            }
        }


        [EnableInBuilder(BuilderType.WebForm, BuilderType.UserControl)]
        [PropertyGroup(PGConstants.CORE)]
        [OnChangeUIFunction("EbDataLabel.DescriptionLabel")]
        public override string Description { get; set; }


        [EnableInBuilder(BuilderType.WebForm, BuilderType.UserControl)]
        [PropertyGroup("Appearance")]
        [OnChangeUIFunction("EbDataLabel.Style4DataLabel")]
        public Align TextAlign { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
        [HideInPropertyGrid]
        public override UISides Margin { get; set; }


        [EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
        [HideInPropertyGrid]
        public override string LabelForeColor { get; set; }

        [EnableInBuilder(BuilderType.BotForm, BuilderType.UserControl)]
        [HideInPropertyGrid]
        public override float FontSize { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
        [HideInPropertyGrid]
        public override string LabelBackColor { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
        [PropertyGroup("Appearance")]
        [UIproperty]
        [PropertyEditor(PropertyEditorType.FontSelector)]
        [OnChangeUIFunction("EbDataLabel.Style4PlaceHolder")]
        public  EbFont PlaceHolderFont { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.UserControl)]
        public string StaticLabel { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.UserControl)]
        [HideInToolBox]
        public string DynamicLabel { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.UserControl)]
        public string DataObjCtrlName { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.UserControl)]
        public string DataObjColName { get; set; }

        public  override bool IsRenderMode { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.UserControl)]
        [PropertyEditor(PropertyEditorType.ScriptEditorJS)]
        [HelpText("Define default value of the control.")]
        public override EbScript DefaultValueExpression { get; set; }

        [HideInPropertyGrid]
        [JsonIgnore]
        public override string ToolIconHtml { get { return "<i class='fa fa-font'></i>"; } set { } }

        public override string GetDesignHtml()
        {
            return new EbDataLabel() { EbSid = "Label1", Label = "Label1" }.GetDesignHtmlHelper().RemoveCR().GraveAccentQuoted(); ;
        }

        //public string fontObj { get; set; }

        public override string GetBareHtml()
        {
            return @"
        <div class='data-static-label'> @Label@ </div>
        @Description@
        <div class='data-dynamic-label' style= ' @style@ '> @PlaceHolder@ </div>
"
.Replace("@name@", this.Name)
.Replace("@Label@", this.Label)
.Replace("@Description@", string.IsNullOrEmpty(this.Description) ? "" : $"<div class='data-Description-label'> {this.Description} </div>")
.Replace("@PlaceHolder@", string.IsNullOrEmpty(this.DynamicLabel) ? "PlaceHolder" : this.DynamicLabel)
.Replace("@style@", this.GetEbFontStyle());
        }

        public string GetDesignHtmlHelper()
        {

            string EbCtrlHTML = @" 
            <div id='cont_@ebsid@' ebsid='@ebsid@' name='@name@' class='Eb-ctrlContainer' @childOf@ ctype='@type@' eb-hidden='@isHidden@'>
                    <div  id='@ebsid@Wraper' class='ctrl-cover' ui-inp style='align-items: @textalign@ '>
                        <div> <span class='eb-ctrl-label eb-label-editable' ui-label id='@ebsidLbl'>@Label@</span> 
                        <input id='@ebsid@lbltxtb' class='eb-lbltxtb' type='text'/> @req@  </div>
                        <div class='eb-des-label' id='@ebLblDescription'> @Description@ </div> 
                        <div class='data-dynamic-label'> @PlaceHolder@ </div>
                    </div>
                <span class='helpText' ui-helptxt>@helpText@ </span>
            </div>"
               .Replace("@LabelForeColor ", "color:" + (LabelForeColor ?? "@LabelForeColor ") + ";")
               .Replace("@LabelBackColor ", "background-color:" + (LabelBackColor ?? "@LabelBackColor ") + ";")
               .Replace("@name@", this.Name)
               .Replace("@Label@", this.Label)
               .Replace("@Description@", this.Description)
               .Replace("@textalign@", this.TextAlign.ToString())
               .Replace("Right" , "flex-end")
               .Replace("Left" , "flex-start")
               .Replace("@PlaceHolder@", string.IsNullOrEmpty(this.DynamicLabel) ? "PlaceHolder" : this.DynamicLabel); ;
            return ReplacePropsInHTML(EbCtrlHTML);
        }

        public override string GetHtml()
        {
            string EbCtrlHTML = @" 
        <div id='cont_@ebsid@' ebsid='@ebsid@' name='@name@' class='Eb-ctrlContainer' @childOf@ ctype='@type@' eb-hidden='@isHidden@'>
                <div  id='@ebsid@Wraper' class='ctrl-cover' ui-inp style='align-items: @style@ ;background-color: @bgColor@ ; color :@forecolor@'>        
                 <link rel='stylesheet' type='text/css' href='@fontVal@'/>
                    @barehtml@
                </div>
            <span class='helpText' ui-helptxt>@helpText@ </span>
        </div>"
               .Replace("@LabelForeColor ", "color:" + (LabelForeColor ?? "@LabelForeColor ") + ";")
               .Replace("@style@", this.TextAlign.ToString())
               .Replace("Right", "flex-end")
               .Replace("Left", "flex-start")
               .Replace("@bgColor@" , this.BackColor)
               .Replace("@forecolor@", this.ForeColor)
               .Replace("@fontVal@",(this.PlaceHolderFont==null) ? string.Empty : "https://fonts.googleapis.com/css?family=" + GetGoogleFontName(this.PlaceHolderFont.FontName))
               .Replace("@LabelBackColor ", "background-color:" + (LabelBackColor ?? "@LabelBackColor ") + ";");

            return ReplacePropsInHTML(EbCtrlHTML);
        }

        public string GetEbFontStyle()
        {
            string fontObj = "";
            if (PlaceHolderFont != null) { 
                fontObj = $"font-family : {PlaceHolderFont.FontName} ; font-size : {PlaceHolderFont.Size}px ;" +
                    $"color : {PlaceHolderFont.color};";
                if (PlaceHolderFont.Caps){ fontObj += "text-transform : line-through ;";}   
                if (PlaceHolderFont.Strikethrough){ fontObj += "text-decoration : line-through ;"; }   
                if (PlaceHolderFont.Underline){ fontObj += "text-decoration : underline ;";}
                if (PlaceHolderFont.Style == FontStyle.BOLD) { fontObj += "font-weight : bold"; }
                else if (PlaceHolderFont.Style == FontStyle.ITALIC) { fontObj += "font-style : italic"; }
                else if (PlaceHolderFont.Style == FontStyle.BOLDITALIC) { fontObj += "font-weight : bold ; font-style : italic ;"; }

            }
            return fontObj;
        }
        public string GetGoogleFontName(string font)
        {
            Dictionary<string, string> GfontList = new Dictionary<string, string>();
            GfontList.Add("Arapey" , "Arapey");
            GfontList.Add("Arvo", "Arvo");
            GfontList.Add("Baskerville", "Libre Baskerville");
            GfontList.Add("Cabin Condensed", "Cabin Condensed");
            GfontList.Add("Century Gothic", "Didact Gothic");
            GfontList.Add("Courier", "Courier");
            GfontList.Add("Crimson Text", "Crimson Text");
            GfontList.Add("EB Garamond", "EB Garamond");
            GfontList.Add("GFS Didot", "GFS Didot");
            GfontList.Add("Gotham", "Montserrat");
            GfontList.Add("Helvetica", "Helvetica");
            GfontList.Add("Libre Franklin", "Libre Franklin");
            GfontList.Add("Maven Pro", "Maven Pro");
            GfontList.Add("Merriweather", "Merriweather");
            GfontList.Add("News Cycle", "News Cycle");
            GfontList.Add("Puritan", "Puritan");
            GfontList.Add("Questrial", "Questrial");
            GfontList.Add("Times-Roman", "Tinos");
            GfontList.Add("Times", "Tinos");
            GfontList.Add("ZapfDingbats", "Heebo");

            return GfontList[font];
        }
           public override string GetValueFromDOMJSfn { get { return @"return this.DynamicLabel"; } set { } }
           public override string StyleJSFn { get { return @"return $('#cont_' + this.EbSid).find('.ctrl-cover').css(p1, p2);"; } set { } }
           public override string GetValueJSfn { get { return @"return this.DynamicLabel"; } set { } }
    }
}
