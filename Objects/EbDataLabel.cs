using ExpressBase.Common.Extensions;
using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using ExpressBase.Objects.Objects.DVRelated;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ExpressBase.Objects
{

    [EnableInBuilder(BuilderType.BotForm, BuilderType.WebForm, BuilderType.UserControl)]
    [HideInToolBox]
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
                $(`#cont_${elementId} .ctrl-cover`).removeAttr('style').css(`align-items`, styleVar);
                }
                }";
            }
        }
    

        [EnableInBuilder(BuilderType.WebForm, BuilderType.UserControl)]
        [PropertyGroup("Identity")]
        [OnChangeUIFunction("EbDataLabel.DescriptionLabel")]
        public string Description { get; set; }


        [EnableInBuilder(BuilderType.WebForm, BuilderType.UserControl)]
        [PropertyGroup("Appearance")]
        [OnChangeUIFunction("EbDataLabel.Style4DataLabel")]
        public Align TextAlign { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.UserControl)]
        public string StaticLabel { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.UserControl)]
        [HideInToolBox]
        public string DynamicLabel { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.UserControl)]
        public string DataObjCtrlName { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.UserControl)]
        public string DataObjColName { get; set; }

        public bool IsRenderMode { get; set; }

        [HideInPropertyGrid]
        [JsonIgnore]
        public override string ToolIconHtml { get { return "<i class='fa fa-font'></i>"; } set { } }

        public override string GetDesignHtml()
        {
            return new EbDataLabel() { EbSid = "Label1", Label = "Label1" }.GetDesignHtmlHelper().RemoveCR().GraveAccentQuoted(); ;
        }

        public override string GetBareHtml()
        {
            return @"
        <div class='data-static-label'> @Label@ </div>
        <div class='data-Description-label' > @Description@ </div>
        <div class='data-dynamic-label'> @PlaceHolder@ </div>
"
.Replace("@name@", this.Name)
.Replace("@Label@", this.Label)
.Replace("@Description@", this.Description)
.Replace("@PlaceHolder@", string.IsNullOrEmpty(this.DynamicLabel) ? "PlaceHolder" : this.DynamicLabel);
        }

        public string GetDesignHtmlHelper()
        {

            string EbCtrlHTML = @" 
            <div id='cont_@ebsid@' ebsid='@ebsid@' name='@name@' class='Eb-ctrlContainer' @childOf@ ctype='@type@' eb-hidden='@isHidden@'>
                    <div  id='@ebsid@Wraper' class='ctrl-cover' style='align-items: @style@ '>
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
               .Replace("@style@", this.TextAlign.ToString())
               .Replace("Right" , "flex-end")
               .Replace("Left" , "flex-start")
               .Replace("@PlaceHolder@", string.IsNullOrEmpty(this.DynamicLabel) ? "PlaceHolder" : this.DynamicLabel); ;
            return ReplacePropsInHTML(EbCtrlHTML);
        }

        public override string GetHtml()
        {
            string EbCtrlHTML = @" 
        <div id='cont_@ebsid@' ebsid='@ebsid@' name='@name@' class='Eb-ctrlContainer' @childOf@ ctype='@type@' eb-hidden='@isHidden@'>
                <div  id='@ebsid@Wraper' class='ctrl-cover' style='align-items: @style@ '>
                    @barehtml@
                </div>
            <span class='helpText' ui-helptxt>@helpText@ </span>
        </div>"
               .Replace("@LabelForeColor ", "color:" + (LabelForeColor ?? "@LabelForeColor ") + ";")
               .Replace("@style@", this.TextAlign.ToString())
               .Replace("Right", "flex-end")
               .Replace("Left", "flex-start")
               .Replace("@LabelBackColor ", "background-color:" + (LabelBackColor ?? "@LabelBackColor ") + ";");

            return ReplacePropsInHTML(EbCtrlHTML);
        }
    }
}
