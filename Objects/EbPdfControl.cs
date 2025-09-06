using ExpressBase.Common;
using ExpressBase.Common.Data;
using ExpressBase.Common.Extensions;
using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using ExpressBase.Common.Structures;
using ExpressBase.Objects.ServiceStack_Artifacts;
using ServiceStack;
using ServiceStack.Redis;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace ExpressBase.Objects
{
    [EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
    public class EbPdfControl : EbControlUI
    {
        public EbPdfControl()
        {
        }

        [OnDeserialized]
        public void OnDeserializedMethod(StreamingContext context)
        {
            this.BareControlHtml = this.GetBareHtml();
            this.BareControlHtml4Bot = this.BareControlHtml;
            this.ObjType = this.GetType().Name.Substring(2, this.GetType().Name.Length - 2);
        }

        public override string ToolIconHtml { get { return "<i class='fa fa-file-pdf-o'></i>"; } set { } }

        public override string ToolNameAlias { get { return "Pdf Control"; } set { } }

        public override string ToolHelpText { get { return "Pdf Control"; } set { } }
        public override string UIchangeFns
        {
            get
            {
                return @"EbPdfControl = {
                
            }";
            }
        }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
        [HideInPropertyGrid]
        public override string HelpText { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
        [HideInPropertyGrid]
        public override string ToolTipText { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
        [HideInPropertyGrid]
        public override bool Unique { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
        [HideInPropertyGrid]
        public override List<EbValidator> Validators { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
        [HideInPropertyGrid]
        public override EbScript DefaultValueExpression { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
        [HideInPropertyGrid]
        public override EbScript HiddenExpr { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
        [HideInPropertyGrid]
        public override EbScript DisableExpr { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
        [HideInPropertyGrid]
        public override EbScript ValueExpr { get; set; }

        public override bool IgnoreDataConsistencyCheck { get; set; }

        public override bool SelfTrigger { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
        [HideInPropertyGrid]
        public override bool Required { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
        [HideInPropertyGrid]
        public override bool DoNotPersist { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
        [HideInPropertyGrid]
        public override string BackColor { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
        [HideInPropertyGrid]
        public override string ForeColor { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
        [HideInPropertyGrid]
        public override string LabelBackColor { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
        [HideInPropertyGrid]
        public override string LabelForeColor { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
        [HideInPropertyGrid]
        public override EbScript OnChangeFn { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
        public override bool IsNonDataInputControl { get => true; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
        [HideInPropertyGrid]
        public override bool IsFullViewContol { get => true; set => base.IsFullViewContol = value; }

        [HideInPropertyGrid]
        [EnableInBuilder(BuilderType.BotForm)]
        public override bool IsDisable { get => true; }

        [PropertyGroup("Behavior")]
        [EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
        [PropertyEditor(PropertyEditorType.ObjectSelector)]
        [OSE_ObjectTypes(EbObjectTypes.iReport)]
        public virtual string PdfRefid { get; set; }

        public void FetchParamsMeta(IServiceClient ServiceClient, IRedisClient redis, Service service)
        {
            EbReport PdfObj = EbFormHelper.GetEbObject<EbReport>(PdfRefid, ServiceClient, redis, service);
            if (string.IsNullOrEmpty(PdfObj.DataSourceRefId))
                throw new FormException($"Missing Data Reader of pdf view that is connected to {this.Label}.");
            EbDataReader DrObj = EbFormHelper.GetEbObject<EbDataReader>(PdfObj.DataSourceRefId, ServiceClient, redis, service);
            this.ParamsList = DrObj.GetParams(redis as RedisClient);
        }

        [HideInPropertyGrid]
        [EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm)]
        public List<Param> ParamsList { get; set; }

        public override string GetDesignHtml()
        {
            return GetHtml().RemoveCR().DoubleQuoted();
        }

        public override string GetHtml()
        {
            string EbCtrlHTML = HtmlConstants.CONTROL_WRAPER_HTML4WEB
                 .Replace("@LabelForeColor ", "color:" + (LabelForeColor ?? "@LabelForeColor ") + ";")
                 .Replace("@LabelBackColor ", "background-color:" + (LabelBackColor ?? "@LabelBackColor ") + ";");

            return ReplacePropsInHTML(EbCtrlHTML);
        }



        public override string GetBareHtml()
        {
            string html = @"
			<div id='@ebsid@' name='@name@' class='pdf_control_cont'>
				<div class='pdfwrapper-cont'>
			        <img id='img_@ebsid@' src='/images/pdf-image.png' style='width: 100px;'>
						<div style=' position: relative;'>
							<span id='spn_@ebsid@' class='dwnld_txt'>Download</span>
							<i id='icon_@ebsid@'  class='fa fa-arrow-circle-o-down '></i>
						</div>
				</div>
			</div>"
            //string html = @"
            //<div id='@ebsid@' name='@name@' style='@style@' class='pdf_control_cont'>

            //	<span class='pdfwrapper-cont'>
            //                  <i id='icon_@ebsid@' style='font-size: 150px;' class='fa fa-file-pdf-o fa-2x'></i>
            //          </span>
            //</div>"
            .Replace("@name@", this.Name)
            .Replace("@ebsid@", this.EbSid);

            return html;
        }

        public override string DesignHtml4Bot
        {
            get => this.GetBareHtml().Replace("@style@", "text-align: center;");
            set => base.DesignHtml4Bot = value;
        }

    }
}
