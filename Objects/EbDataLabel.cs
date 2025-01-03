﻿using ExpressBase.Common;
using ExpressBase.Common.Constants;
using ExpressBase.Common.Extensions;
using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using ExpressBase.Common.Structures;
using ExpressBase.Objects.Objects.DVRelated;
using Newtonsoft.Json;
using Org.BouncyCastle.Asn1.Cms;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ExpressBase.Objects
{

    [EnableInBuilder(BuilderType.WebForm, BuilderType.DashBoard)]
    public class EbDataLabel : EbControlUI
    {

        public EbDataLabel() { }

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
                LabelBackgroudColor : function(elementId, props) {
                console.log('Hello world!');
                },
                Style4PlaceHolder : function(elementId , props){
                 $(`#cont_${elementId} .data-dynamic-label`).css(getEbFontStyleObject(props.PlaceHolderFont));
                },
                Style4StaticLabel : function(elementId , props){
                 $(`#cont_${elementId} .data-dynamic-label`).css(getEbFontStyleObject(props.Style4StaticLabel));
                }
                }";
            }
        }


        //[EnableInBuilder(BuilderType.WebForm, BuilderType.UserControl)]
        //[PropertyGroup(PGConstants.CORE)]
        //[OnChangeUIFunction("EbDataLabel.DescriptionLabel")]
        //public override string Description { get; set; }


        [EnableInBuilder(BuilderType.WebForm, BuilderType.UserControl)]
        [PropertyGroup(PGConstants.APPEARANCE)]
        //[OnChangeUIFunction("EbDataLabel.Style4DataLabel")]
        public Align TextAlign { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
        [HideInPropertyGrid]
        public override UISides Margin { get; set; }


        [EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
        [PropertyGroup("Appearance")]
        [UIproperty]
        [PropertyEditor(PropertyEditorType.FontSelector)]
        [DefaultPropValue(null)]
        [OnChangeUIFunction("EbDataLabel.Style4PlaceHolder")]
        public EbFont PlaceHolderFont { get; set; }



        [EnableInBuilder(BuilderType.DashBoard, BuilderType.WebForm)]
        [PropertyGroup("Core")]
        public LabelStyle LabelStyle { get; set; }


        [EnableInBuilder(BuilderType.DashBoard, BuilderType.WebForm)]
        [PropertyGroup("LabelConfig")]
        [DefaultPropValue("4")]
        [OnChangeExec(@"if(this.LabelborderRadius > 50){
            this.LabelborderRadius = 50;
            $('#' + pg.wraperId + 'LabelborderRadius').val(50);
            console.log('hhh');
            }
            else if(this.LabelborderRadius < 0){
            this.LabelborderRadius = 0;
            $('#' + pg.wraperId + 'LabelborderRadius').val(0);
            }
            ")]
        public int LabelBorderRadius { get; set; }


        [EnableInBuilder(BuilderType.DashBoard, BuilderType.WebForm)]
        [PropertyEditor(PropertyEditorType.Color)]
        [PropertyGroup("LabelConfig")]
        [DefaultPropValue("#d2d2d7")]
        public string LabelBorderColor { get; set; }


        [EnableInBuilder(BuilderType.WebForm, BuilderType.DashBoard, BuilderType.UserControl)]
        [PropertyGroup("LabelConfig")]
        [OnChangeUIFunction("EbDataLabel.LabelBackgroudColor")]
        [PropertyEditor(PropertyEditorType.Color)]
        public override string LabelBackColor { get; set; }

        [EnableInBuilder(BuilderType.DashBoard)]
        [PropertyGroup("LabelConfig")]
        [OnChangeExec(@"
                if (this.IsGradient === true ){      
                        pg.ShowProperty('GradientColor1');     
                        pg.ShowProperty('GradientColor2');     
                        pg.ShowProperty('Direction');
                        pg.HideProperty('LabelBackColor');
                }
                else {
                        pg.ShowProperty('LabelBackColor');    
                        pg.HideProperty('GradientColor1');     
                        pg.HideProperty('GradientColor2');     
                        pg.HideProperty('Direction');      
                }
            ")]
        public bool IsGradient { get; set; }

        [EnableInBuilder(BuilderType.DashBoard)]
        [PropertyEditor(PropertyEditorType.Color)]
        [PropertyGroup("LabelConfig")]
        [OnChangeUIFunction("EbDataLabel.LabelGradientColor")]
        [DefaultPropValue("#2f2f51")]
        public string GradientColor1 { get; set; }

        [EnableInBuilder(BuilderType.DashBoard)]
        [PropertyEditor(PropertyEditorType.Color)]
        [PropertyGroup("LabelConfig")]
        [OnChangeUIFunction("EbDataLabel.LabelGradientColor")]
        [DefaultPropValue("#59a4a6")]
        public string GradientColor2 { get; set; }


        [EnableInBuilder(BuilderType.DashBoard)]
        [PropertyGroup("LabelConfig")]
        public GradientDirection Direction { get; set; }

        [EnableInBuilder(BuilderType.DashBoard)]
        [PropertyGroup("LabelConfig")]
        public LabelTextPosition TextPosition { get; set; }

        [EnableInBuilder(BuilderType.DashBoard)]
        [PropertyGroup("LabelConfig")]
        [PropertyEditor(PropertyEditorType.ShadowEditor)]
        public string Shadow { get; set; }


        [EnableInBuilder(BuilderType.DashBoard)]
        [PropertyGroup("LabelConfig")]
        [UIproperty]
        [Alias("CustomTextPosition")]
        [OnChangeExec(@"if (this.ChangeTextPositon === true ){      
                        pg.ShowProperty('StaticLabelPosition');     
                        pg.ShowProperty('DynamicLabelPositon');     
                        pg.ShowProperty('DescriptionPosition');     
                        pg.ShowProperty('Left');    
                        pg.ShowProperty('Top');    
                }
                else {
                        pg.HideProperty('StaticLabelPosition');      
                        pg.HideProperty('DynamicLabelPositon');      
                        pg.HideProperty('DescriptionPosition');  
                        pg.HideProperty('Left');    
                        pg.HideProperty('Top');    
                }
            ")]
        public bool ChangeTextPositon { get; set; }

        //[EnableInBuilder(BuilderType.DashBoard)]
        //[HideInPropertyGrid]
        //public string LabelValue { get; set; }

        //[EnableInBuilder(BuilderType.DashBoard)]
        //[HideInPropertyGrid]
        //public string LabelCtrlName { get; set; }


        //[EnableInBuilder(BuilderType.BotForm, BuilderType.UserControl)]
        //[HideInPropertyGrid]
        //public override float FontSize { get; set; }

        //Static Label

        [EnableInBuilder(BuilderType.WebForm, BuilderType.UserControl, BuilderType.DashBoard)]
        [PropertyGroup("StaticLabel")]
        [PropertyEditor(PropertyEditorType.MultiLanguageKeySelector)]
        [UIproperty]
        public string StaticLabel { get; set; }

        [EnableInBuilder(BuilderType.DashBoard, BuilderType.WebForm)]
        [PropertyGroup("StaticLabel")]
        [UIproperty]
        [PropertyEditor(PropertyEditorType.FontSelector)]
        [DefaultPropValue(null)]
        [OnChangeUIFunction("EbDataLabel.Style4StaticLabel")]
        public EbFont StaticLabelFont { get; set; }

        [EnableInBuilder(BuilderType.DashBoard, BuilderType.WebForm)]
        [PropertyGroup("StaticLabel")]
        [UIproperty]
        [PropertyEditor(PropertyEditorType.Expandable)]
        [OnChangeExec(@"if(this.StaticLabelPadding > 100){
            this.StaticLabelPadding = 100;
            $('#' + pg.wraperId + 'StaticLabelPadding').val(100);
            }
            else if(this.StaticLabelPadding < 0){
            this.StaticLabelPadding = 0;
            $('#' + pg.wraperId + 'StaticLabelPadding').val(0);
            }
            ")]
        public TextPositon StaticLabelPosition { get; set; }


        //dynamic label config

        [EnableInBuilder(BuilderType.DashBoard, BuilderType.WebForm)]
        [PropertyGroup("DynamicLabel")]
        [UIproperty]
        [PropertyEditor(PropertyEditorType.FontSelector)]
        [DefaultPropValue(null)]
        public EbFont DynamicLabelFont { get; set; }


        [EnableInBuilder(BuilderType.DashBoard, BuilderType.WebForm)]
        [PropertyGroup("DynamicLabel")]
        [PropertyEditor(PropertyEditorType.Expandable)]
        [UIproperty]
        [OnChangeExec(@"if(this.DynamicLabelPadding > 100){
            this.DynamicLabelPadding = 100;
            $('#' + pg.wraperId + 'DynamicLabelPadding').val(100);
            console.log('hhh');
            }
            else if(this.DynamicLabelPadding < 0){
            this.DynamicLabelPadding = 0;
            $('#' + pg.wraperId + 'DynamicLabelPadding').val(0);
            }
            ")]
        public TextPositon DynamicLabelPositon { get; set; }



        //Description of Label

        [EnableInBuilder(BuilderType.DashBoard)]
        [PropertyGroup("Description")]
        public override string Description { get; set; }

        [EnableInBuilder(BuilderType.BotForm, BuilderType.UserControl, BuilderType.DashBoard)]
        [PropertyGroup("Description")]
        [UIproperty]
        [PropertyEditor(PropertyEditorType.FontSelector)]
        [DefaultPropValue(null)]
        public EbFont DescriptionFont { get; set; }

        [EnableInBuilder(BuilderType.DashBoard)]
        [PropertyGroup("Description")]
        [PropertyEditor(PropertyEditorType.Expandable)]
        [UIproperty]
        [OnChangeExec(@"if(this.DescriptionPadding > 100){
            this.DescriptionPadding = 100;
            $('#' + pg.wraperId + 'DescriptionPadding').val(100);
            }
            else if(this.DescriptionPadding < 0){
            this.DescriptionPadding = 0;
            $('#' + pg.wraperId + 'DescriptionPadding').val(0);
            }
            ")]
        public TextPositon DescriptionPosition { get; set; }

        //Icon

        [EnableInBuilder(BuilderType.DashBoard)]
        [PropertyGroup("Icon")]
        [UIproperty]
        [DefaultPropValue("true")]
        [OnChangeExec(@"if (this.RenderIcon === true ){      
                        pg.ShowProperty('Icon');     
                        pg.ShowProperty('IconColor');     
                        pg.ShowProperty('IconGradientColor1');     
                        pg.ShowProperty('IconGradientColor2');    
                        pg.ShowProperty('IconDirection');    
                        pg.ShowProperty('FooterText');    
                        pg.ShowProperty('FooterTextColor');    
                }
                else {     
                        pg.HideProperty('Icon');     
                        pg.HideProperty('IconColor');     
                        pg.HideProperty('IconGradientColor1');     
                        pg.HideProperty('IconGradientColor2');    
                        pg.HideProperty('IconDirection');    
                        pg.HideProperty('FooterText');    
                        pg.HideProperty('FooterTextColor');    
                }
            ")]
        public bool RenderIcon { get; set; }


        [EnableInBuilder(BuilderType.DashBoard, BuilderType.WebForm)]
        [PropertyGroup("Icon")]
        [UIproperty]
        [PropertyEditor(PropertyEditorType.IconPicker)]
        [DefaultPropValue("fa-building-o")]
        public string Icon { get; set; }


        [EnableInBuilder(BuilderType.DashBoard, BuilderType.WebForm)]
        [PropertyGroup("Icon")]
        [UIproperty]
        [DefaultPropValue("")]
        public string IconText { get; set; }

        [EnableInBuilder(BuilderType.DashBoard, BuilderType.WebForm)]
        [PropertyGroup("Icon")]
        [UIproperty]
        [PropertyEditor(PropertyEditorType.Color)]
        public string IconColor { get; set; }

        [EnableInBuilder(BuilderType.DashBoard)]
        [PropertyGroup("Icon")]
        [UIproperty]
        [PropertyEditor(PropertyEditorType.Color)]
        [Alias("GradientColor1")]
        [DefaultPropValue("#0202ff")]
        public string IconGradientColor1 { get; set; }


        [EnableInBuilder(BuilderType.DashBoard)]
        [PropertyGroup("Icon")]
        [UIproperty]
        [PropertyEditor(PropertyEditorType.Color)]
        [DefaultPropValue("#0000a0")]
        [Alias("GradientColor2")]
        public string IconGradientColor2 { get; set; }


        [EnableInBuilder(BuilderType.DashBoard)]
        [PropertyGroup("Icon")]
        public GradientDirection IconDirection { get; set; }

        [EnableInBuilder(BuilderType.DashBoard)]
        [PropertyGroup("Icon")]
        [UIproperty]
        [DefaultPropValue("true")]
        [OnChangeExec(@"if (this.HideFooter === false ){      
                        pg.ShowProperty('FooterIcon');     
                        pg.ShowProperty('FooterIconColor');     
                        pg.ShowProperty('FooterText');     
                        pg.ShowProperty('FooterTextColor');    
                }
                else {     
                        pg.HideProperty('FooterIcon');     
                        pg.HideProperty('FooterIconColor');     
                        pg.HideProperty('FooterText');     
                        pg.HideProperty('FooterTextColor');                           
                }
            ")]
        public bool HideFooter { get; set; }

        [EnableInBuilder(BuilderType.DashBoard)]
        [PropertyGroup("Icon")]
        [UIproperty]
        [PropertyEditor(PropertyEditorType.IconPicker)]
        [DefaultPropValue("fa-calendar-o")]
        public string FooterIcon { get; set; }

        [EnableInBuilder(BuilderType.DashBoard)]
        [PropertyGroup("Icon")]
        [UIproperty]
        [PropertyEditor(PropertyEditorType.Color)]
        [DefaultPropValue("#000000")]
        public string FooterIconColor { get; set; }


        [EnableInBuilder(BuilderType.DashBoard)]
        [PropertyGroup("Icon")]
        [UIproperty]
        [DefaultPropValue("Today")]
        public string FooterText { get; set; }

        [EnableInBuilder(BuilderType.DashBoard)]
        [PropertyGroup("Icon")]
        [UIproperty]
        [PropertyEditor(PropertyEditorType.Color)]
        [DefaultPropValue("#000000")]
        public string FooterTextColor { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.UserControl, BuilderType.DashBoard)]
        [HideInPropertyGrid]
        [HideInToolBox]
        public string DynamicLabel { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.UserControl, BuilderType.DashBoard)]
        [HideInPropertyGrid]
        public string DataObjCtrlName { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.UserControl, BuilderType.DashBoard)]
        [HideInPropertyGrid]
        public string DataObjColName { get; set; }


        [EnableInBuilder(BuilderType.DashBoard)]
        [HideInPropertyGrid]
        public string LabelValue { get; set; }


        [EnableInBuilder(BuilderType.DashBoard)]
        [HideInPropertyGrid]
        public string LabelContainer { get; set; }

        public override bool IsRenderMode { get; set; }


        [EnableInBuilder(BuilderType.WebForm, BuilderType.UserControl, BuilderType.DashBoard)]
        [PropertyEditor(PropertyEditorType.ScriptEditorJS)]
        [HelpText("Define default value of the control.")]
        public override EbScript DefaultValueExpression { get; set; }

        [EnableInBuilder(BuilderType.DashBoard)]
        [PropertyEditor(PropertyEditorType.ObjectSelector)]
        [PropertyGroup("Data Settings")]
        [OSE_ObjectTypes(EbObjectTypes.iChartVisualization, EbObjectTypes.iTableVisualization, EbObjectTypes.iDashBoard, EbObjectTypes.iWebForm)]
        public string Object_Selector { get; set; }

        [HideInPropertyGrid]
        [JsonIgnore]
        public override string ToolIconHtml { get { return "<i class='fa fa-font'></i>"; } set { } }


        //for webform only

        [EnableInBuilder(BuilderType.WebForm, BuilderType.UserControl, BuilderType.DashBoard)]
        [PropertyEditor(PropertyEditorType.ObjectSelector)]
        [OSE_ObjectTypes(EbObjectTypes.iDataReader)]
        [UIproperty]

        [PropertyPriority(99)]
        public string DataSourceId { get; set; }

        [EnableInBuilder(BuilderType.WebForm)]
        [MetaOnly]
        public DVColumnCollection Columns { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl)]
        [PropertyEditor(PropertyEditorType.CollectionFrmSrc, "Columns", 1)]
        [OnChangeExec(@"if (this.Columns.$values.length === 0 ){pg.MakeReadOnly('ValueMember');} else {pg.MakeReadWrite('ValueMember');}")]
        [PropertyPriority(69)]
        public DVBaseColumn ValueMember { get; set; }


        [EnableInBuilder(BuilderType.WebForm)]
        [DefaultPropValue("1000")]
        public int RefreshTime { get; set; }
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
                    <div class='card-icon' id='@ebsid@_icon'><i class=''></i></div><div class='lb2-data'>
                        <div> <span class='eb-ctrl-label eb-label-editable' ui-label id='@ebsidLbl'>@Label@</span> 
                        <input id='@ebsid@lbltxtb' class='eb-lbltxtb' type='text'/> @req@  </div>
                        <div class='eb-des-label' id='@ebLblDescription'> @Description@ </div>
                        <div class='data-dynamic-label'> @PlaceHolder@ </div></div>
                    </div>
                <span class='helpText' ui-helptxt>@helpText@ </span>
            </div>"
               .Replace("@LabelForeColor ", "color:" + (LabelForeColor ?? "@LabelForeColor ") + ";")
               .Replace("@LabelBackColor ", "background-color:" + (LabelBackColor ?? "@LabelBackColor ") + ";")
               .Replace("@name@", this.Name)
               .Replace("@Label@", this.Label)
               .Replace("@Description@", this.Description)
               .Replace("@textalign@", this.TextAlign.ToString())
               .Replace("Right", "flex-end")
               .Replace("Left", "flex-start")
               .Replace("@PlaceHolder@", string.IsNullOrEmpty(this.DynamicLabel) ? "PlaceHolder" : this.DynamicLabel);
            return ReplacePropsInHTML(EbCtrlHTML);
        }

        public override string GetHtml()
        {
            string EbCtrlHTML = @"  
            <div id='cont_@ebsid@' ebsid='@ebsid@' name='@name@' class='Eb-ctrlContainer' @childOf@ ctype='@type@' eb-hidden='@isHidden@'>
                    <div  id='@ebsid@Wraper' class='ctrl-cover' ui-inp style='align-items: @textalign@ ; display:flex;flex-flow:row;'>
                    <div class='card-icon' id='@ebsid@_icon'><i class='' style='font-size:30px;padding:10px;'></i></div><div class='lb2-data'>
                        <div> <span class='eb-ctrl-label eb-label-editable' ui-label id='@ebsidLbl'>@Label@</span> 
                        <input id='@ebsid@lbltxtb' class='eb-lbltxtb' type='text'/> @req@  </div>
                        <div class='data-dynamic-label'> @PlaceHolder@ </div></div>
                    </div>
                <span class='helpText' ui-helptxt>@helpText@ </span>
            </div>"
               .Replace("@LabelForeColor ", "color:" + (LabelForeColor ?? "@LabelForeColor ") + ";")
               .Replace("@style@", this.TextAlign.ToString())
               .Replace("Right", "flex-end")
               .Replace("Left", "flex-start")
               .Replace("@bgColor@", this.BackColor)
               .Replace("@forecolor@", this.ForeColor)
               .Replace("@fontVal@", (this.PlaceHolderFont == null) ? string.Empty : "https://fonts.googleapis.com/css?family=" + GetGoogleFontName(this.PlaceHolderFont.FontName))
               .Replace("@LabelBackColor ", "background-color:" + (LabelBackColor ?? "@LabelBackColor ") + ";");

            return ReplacePropsInHTML(EbCtrlHTML);
        }

        public string GetEbFontStyle()
        {
            string fontObj = "";
            if (PlaceHolderFont != null)
            {
                fontObj = $"font-family : {PlaceHolderFont.FontName} ; font-size : {PlaceHolderFont.Size}px ;" +
                    $"color : {PlaceHolderFont.color};";
                if (PlaceHolderFont.Caps) { fontObj += "text-transform : line-through ;"; }
                if (PlaceHolderFont.Strikethrough) { fontObj += "text-decoration : line-through ;"; }
                if (PlaceHolderFont.Underline) { fontObj += "text-decoration : underline ;"; }
                if (PlaceHolderFont.Style == FontStyle.BOLD) { fontObj += "font-weight : bold"; }
                else if (PlaceHolderFont.Style == FontStyle.ITALIC) { fontObj += "font-style : italic"; }
                else if (PlaceHolderFont.Style == FontStyle.BOLDITALIC) { fontObj += "font-weight : bold ; font-style : italic ;"; }

            }
            return fontObj;
        }
        public string GetGoogleFontName(string font)
        {
            Dictionary<string, string> GfontList = new Dictionary<string, string>();
            GfontList.Add("Arapey", "Arapey");
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

        public override void LocalizeControl(Dictionary<string, string> Keys)
        {
            if (!string.IsNullOrWhiteSpace(this.StaticLabel) && Keys.ContainsKey(this.StaticLabel))
            {
                this.StaticLabel = Keys[this.StaticLabel];
            }
        }

        public override void AddMultiLangKeys(List<string> keysList)
        {
            if (!string.IsNullOrWhiteSpace(this.StaticLabel) && !keysList.Contains(this.StaticLabel))
            {
                keysList.Add(this.StaticLabel);
            }
        }
    }
    public class LabelAppearance
    {


    }
    public class GradientLabel
    {

    }
    public enum GradientDirection
    {
        to_right = 0,
        to_left = 1,
        to_bottom = 2,
        to_top = 3,
        to_bottom_right = 4,
        to_bottom_left = 5,
        to_top_right = 6,
        to_top_left = 7,
    }
    public enum FlexType
    {
        row = 0,
        colomn = 1,
    }
    public enum LabelTextPosition
    {
        left = 0,
        center = 1,
        right = 2
    }
}

public enum LabelStyle
{
    Label_1 = 0,
    Label_2,
    Label_3,
    Label_4,
    Label_5,
}