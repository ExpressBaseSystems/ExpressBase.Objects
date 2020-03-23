using ExpressBase.Common;
using ExpressBase.Common.Constants;
using ExpressBase.Common.Extensions;
using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using ExpressBase.Common.Structures;
using ExpressBase.Objects.ServiceStack_Artifacts;
using Newtonsoft.Json;
using ServiceStack;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
namespace ExpressBase.Objects
{
    [EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
    public class EbImage : EbControlUI
    {

        public EbImage() { }

        [OnDeserialized]
        public void OnDeserializedMethod(StreamingContext context)
        {
            this.BareControlHtml = this.GetBareHtml();
            this.ObjType = this.GetType().Name.Substring(2, this.GetType().Name.Length - 2);
        }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
        [PropertyEditor(PropertyEditorType.ImageSeletor)]
        [OnChangeUIFunction("EbImage.changeSource")]
        public int ImageId { get; set; }

        [EnableInBuilder(BuilderType.BotForm)]
        [PropertyEditor(PropertyEditorType.ObjectSelector)]
        public string DataSourceId { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
        public string Alt { get; set; }

        public override bool isFullViewContol { get => true; set => base.isFullViewContol = value; }

        [HideInPropertyGrid]
        [EnableInBuilder(BuilderType.BotForm)]
        public override bool IsReadOnly { get => true; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.UserControl)]
        [PropertyGroup(PGConstants.APPEARANCE)]
        [OnChangeUIFunction("EbImage.adjustMaxHeight")]
        [DefaultPropValue("200")]
        public int MaxHeight { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
        [OnChangeUIFunction("EbImage.adjustMaxWidth")]
        [DefaultPropValue("200")]
        public int MaxWidth { get; set; }


        [EnableInBuilder(BuilderType.WebForm)]
        public override EbScript ValueExpr { get; set; }

        public override string UIchangeFns
        {
            get
            {
                return @"EbImage = {
                adjustMaxHeight : function(elementId, props) {
                    $(`#cont_${elementId} .ebimg-cont img`).css('max-height', `${props.MaxHeight}px`);
                },
                adjustMaxWidth : function(elementId, props){
                    $(`#cont_${elementId} .ebimg-cont img`).css('max-width', `${props.MaxWidth}px`);
                },
            changeSource : function(elementId, props){
                    if( props.ImageId > 0){
                       $(`#${elementId.toLowerCase()}`).attr('src', '../images/'+ `${props.ImageId}` +'.jpg'); 
                        }
                }
                }";
            }
        } 

        public override string SetValueJSfn
        {
            get
            {
                return @"
                $('#' + this.EbSid_CtxId.toLowerCase()).attr('src', '../images/'+ p1 +'.jpg');";
            }
            set { }
        }

        public void InitFromDataBase(JsonServiceClient ServiceClient)
        {
            //this.DataSourceId = "eb_roby_dev-eb_roby_dev-2-1015-1739";
            var result = ServiceClient.Get<DataSourceDataResponse>(new DataSourceDataRequest { RefId = this.DataSourceId });
            string _html = string.Empty;
            this.BareControlHtml = "";
        }
        public override string GetHead()
        {
            return string.Empty;
        }
        public override string GetDesignHtml()
        {
            return this.GetHtml().RemoveCR().GraveAccentQuoted();
        }

        [HideInPropertyGrid]
        [JsonIgnore]
        public override string ToolIconHtml { get { return "<i class='fa fa-file-image-o'></i>"; } set { } }

        //public override string GetToolHtml()
        //{
        //    return @"<div eb-type='@toolName' class='tool'><i class='fa fa-image'></i>  @toolName</div>".Replace("@toolName", this.GetType().Name.Substring(2));
        //}

        public override string GetBareHtml()
        {
            return @" 
        <div class='ebimg-cont'>
            <img id='@name@' src='@src@' style='max-width:@maxwidth@px; max-height:@maxheight@px;'>
        </div>"
    .Replace("@name@", this.Name)
    .Replace("@src@", (this.ImageId > 0) ? "../images/" + this.ImageId + ".jpg" : "/images/image.png")
    .Replace("@toolTipText@", this.ToolTipText)
    .Replace("@value@", "")//"value='" + this.Value + "'")
    .Replace("@maxwidth@", this.MaxWidth > 0 ? this.MaxWidth.ToString() : "200")
    .Replace("@maxheight@", this.MaxHeight > 0 ? this.MaxHeight.ToString() : "200"); ;
        }

        public override string GetHtml()
        {
            string EbCtrlHTML = HtmlConstants.CONTROL_WRAPER_HTML4WEB

    .Replace("@LabelForeColor ", "color:" + ((this.LabelForeColor != null) ? this.LabelForeColor : "@LabelForeColor ") + ";")
    .Replace("@LabelBackColor ", "background-color:" + ((this.LabelBackColor != null) ? this.LabelBackColor : "@LabelBackColor ") + ";");

            return ReplacePropsInHTML(EbCtrlHTML);
        }

        //--------Hide in property grid------------start

        [HideInPropertyGrid]
        public override bool Unique { get; set; }

        [HideInPropertyGrid]
        public override List<EbValidator> Validators { get; set; }

        [HideInPropertyGrid]
        public override EbScript DefaultValueExpression { get; set; }

        [HideInPropertyGrid]
        public override EbScript VisibleExpr { get; set; }

        [HideInPropertyGrid]
        public override string BackColor { get; set; }

        [HideInPropertyGrid]
        public override string ForeColor { get; set; }

        [HideInPropertyGrid]
        public override EbScript OnChangeFn { get; set; }

        [HideInPropertyGrid]
        public override bool Hidden { get; set; }

        [HideInPropertyGrid]
        public override bool IsDisable { get; set; }

        [HideInPropertyGrid]
        public override bool Required { get; set; }

        [HideInPropertyGrid]
        public override string ToolTipText { get; set; }

        [HideInPropertyGrid]
        public override string HelpText { get; set; }
    }
}
