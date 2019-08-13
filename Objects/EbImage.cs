using ExpressBase.Common;
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
    //[EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
    public class EbImage: EbControlUI
	{

        public EbImage() { }

        [OnDeserialized]
        public void OnDeserializedMethod(StreamingContext context)
        {
            this.BareControlHtml = this.GetBareHtml();
            this.ObjType = this.GetType().Name.Substring(2, this.GetType().Name.Length - 2);
        }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
        public string ImageID { get; set; }

        [EnableInBuilder(BuilderType.BotForm)]
        [PropertyEditor(PropertyEditorType.ObjectSelector)]
        public string DataSourceId { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
        public string Alt { get; set; }

        public override bool isFullViewContol { get => true; set => base.isFullViewContol = value; }

		[HideInPropertyGrid]
		[EnableInBuilder(BuilderType.BotForm)]		
		public override bool IsReadOnly { get => true;}

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
        public override string ToolIconHtml { get { return "<i class='fa fa-image'></i>"; } set { } }

        //public override string GetToolHtml()
        //{
        //    return @"<div eb-type='@toolName' class='tool'><i class='fa fa-image'></i>  @toolName</div>".Replace("@toolName", this.GetType().Name.Substring(2));
        //}

        public override string GetBareHtml()
        {
            return @" 
        <div class='ebimg-cont' style='width:100%;'>
            <img id='@name@' src='@src@'  style='width:100%;' alt='@alt@'>
        </div>"
.Replace("@name@", this.Name)
.Replace("@src@", String.IsNullOrWhiteSpace(this.ImageID) ? "/images/imageControlSampleImage.jpg" : this.ImageID)
.Replace("@toolTipText@", this.ToolTipText)
.Replace("@value@", "")//"value='" + this.Value + "'")
    .Replace("@alt@ ", this.Alt ?? "@alt@ ");
        }

        public override string GetHtml()
        {
            string EbCtrlHTML = HtmlConstants.CONTROL_WRAPER_HTML4WEB

    .Replace("@LabelForeColor ", "color:" + ((this.LabelForeColor != null) ? this.LabelForeColor : "@LabelForeColor ") + ";")
    .Replace("@LabelBackColor ", "background-color:" + ((this.LabelBackColor != null) ? this.LabelBackColor : "@LabelBackColor ") + ";");

            return ReplacePropsInHTML(EbCtrlHTML);
        }
    }
}
