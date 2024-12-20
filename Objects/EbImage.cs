﻿using ExpressBase.Common;
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
    [EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl, BuilderType.SurveyControl)]
    [SurveyBuilderRoles(SurveyRoles.AnswerControl,SurveyRoles.QuestionControl)]
    public class EbImage : EbControlUI
    {

        public EbImage() { }

        [OnDeserialized]
        public void OnDeserializedMethod(StreamingContext context)
        {
            this.BareControlHtml = this.GetBareHtml();
			this.BareControlHtml4Bot = this.BareControlHtml;
            this.ObjType = this.GetType().Name.Substring(2, this.GetType().Name.Length - 2);
        }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
        [PropertyEditor(PropertyEditorType.ImageSeletor)]
        [OnChangeUIFunction("EbImage.changeSource")]
        public int ImageId { get; set; }

		[EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
		[PropertyEditor(PropertyEditorType.ObjectSelector)]
        public string DataSourceId { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
        public string Alt { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
        [HideInPropertyGrid]
        public override bool IsFullViewContol { get => true; set => base.IsFullViewContol = value; }

        //[HideInPropertyGrid]
        //[EnableInBuilder(BuilderType.BotForm)]
        //public override bool IsReadOnly { get => true; }

        [HideInPropertyGrid]
        [EnableInBuilder(BuilderType.BotForm)]
        public override bool IsDisable { get => true; }

		[EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl)]
        public override bool IsNonDataInputControl { get => true; }

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

        public override bool SelfTrigger { get; set; }

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
                if(parseInt(p1)>0){
                            $('#' + this.EbSid_CtxId.toLowerCase()).attr('src', '../images/'+ p1 +'.jpg');
                }
                else{
                            $('#' + this.EbSid_CtxId.toLowerCase()).attr('src', '/images/image.png');
                }
                ";
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
        [EnableInBuilder(BuilderType.SurveyControl)]
        public override string ToolIconHtml { get { return "<i class=\"fa fa-file-image-o\" ></i>"; } set { } }

        //public override string GetToolHtml()
        //{
        //    return @"<div eb-type='@toolName' class='tool'><i class='fa fa-image'></i>  @toolName</div>".Replace("@toolName", this.GetType().Name.Substring(2));
        //}

        public override string GetBareHtml()
        {
            return @" 
        <div class='ebimg-cont'>
            <img id='@ebsid@' name='@name@' src='@src@' style='max-width:@maxwidth@px; max-height:@maxheight@px;'>
        </div>"
	.Replace("@name@", this.Name)
	.Replace("@ebsid@", this.EbSid)
    .Replace("@src@",  "/images/image.png")
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

		public override string DesignHtml4Bot
		{
			get => this.GetBareHtml();
			set => base.DesignHtml4Bot = value;
		}
		//--------Hide in property grid------------start

		[HideInPropertyGrid]
        public override bool Unique { get; set; }

        [HideInPropertyGrid]
        public override List<EbValidator> Validators { get; set; }

        [HideInPropertyGrid]
        public override EbScript DefaultValueExpression { get; set; }

        [HideInPropertyGrid]
        public override EbScript HiddenExpr { get; set; }
        
        [HideInPropertyGrid]
        public override EbScript DisableExpr { get; set; }

        [HideInPropertyGrid]
        public override string BackColor { get; set; }

        [HideInPropertyGrid]
        public override string ForeColor { get; set; }

        [HideInPropertyGrid]
        public override EbScript OnChangeFn { get; set; }

        [HideInPropertyGrid]
        public override bool Hidden { get; set; }
		
        [HideInPropertyGrid]
        public override bool Required { get; set; }

        [HideInPropertyGrid]
        public override string ToolTipText { get; set; }

        [HideInPropertyGrid]
        public override string HelpText { get; set; }
    }
}
