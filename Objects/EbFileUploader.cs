using ExpressBase.Common;
using ExpressBase.Common.Extensions;
using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using ExpressBase.Objects.Helpers;
using Newtonsoft.Json;
using ServiceStack;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace ExpressBase.Objects
{
    [EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
    public class EbFileUploader : EbControlUI
	{

        public EbFileUploader()
        {
            this.Categories = new List<EbFupCategories>();
        }

		[HideInPropertyGrid]
		[EnableInBuilder(BuilderType.BotForm)]
		public override bool IsReadOnly { get => this.ReadOnly; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
        [PropertyGroup("General")]
        public FileClass FileType { set; get; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
        [PropertyGroup("General")]
        [PropertyEditor(PropertyEditorType.Collection)]
        public List<EbFupCategories> Categories { set; get; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
        [PropertyGroup("General")]
        public bool IsMultipleUpload { set; get; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
        [PropertyGroup("General")]
        public bool EnableTag { set; get; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
        [PropertyGroup("General")]
        public bool EnableCrop { set; get; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
        [PropertyGroup("General")]
        public int MaxFileSize { set; get; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
        [PropertyGroup("Crop Properties")]
        public bool ResizeViewPort { set; get; }

        [OnDeserialized]
        public void OnDeserializedMethod(StreamingContext context)
        {
            this.BareControlHtml = this.GetBareHtml();
            this.ObjType = this.GetType().Name.Substring(2, this.GetType().Name.Length - 2);
        }
        public override string GetHead()
        {
            return string.Empty;
        }
        public override string GetDesignHtml()
       {
            return HtmlConstants.CONTROL_WRAPER_HTML4WEB
                .Replace("@barehtml@", @"
                    <div id='Wraper' class='ctrl-cover'>                             
                        <div class='input-group'style='width: 100 %; '> 
                                 <input id='' ui-inp='' data-ebtype='6' class='date' type='text' name=' tabindex='0' style='width: 100%; display: inline - block; background - color: rgb(255, 255, 255); color: rgb(51, 51, 51);' placeholder=''>
                                 <span class='input-group-addon'>
                                <i class='fa fa fa-upload' aria-hidden='true'  style='padding: 6px 12px;'></i>  
                            </span>
                        </div>
                    </div>
                ").RemoveCR().DoubleQuoted();
            //return this.GetHtml().RemoveCR().GraveAccentQuoted();
        }
        public override string GetToolHtml()
        {
            return @"<div eb-type='@toolName' class='tool'><i class='fa fa-upload'></i>  @toolName</div>".Replace("@toolName", this.GetType().Name.Substring(2));
        }

        public override string GetBareHtml()
        {
            return @" 
        <div id='@ebsid@' style='width:100%;'></div>"
            .Replace("@ebsid@", this.EbSid);
        }

        public override string GetHtml()
        {
            //            string EbCtrlHTML = @"
            //    <div id='cont_@name@' Ctype='FileUploader' class='Eb-ctrlContainer' eb-hidden='@isHidden@'>
            //        <div class='eb-ctrl-label' id='@name@Lbl' style='@LabelBackColor  @LabelForeColor '> @Label </div>
            //       @barehtml@
            //        <span class='helpText'> @HelpText </span>
            //    </div>
            //"
            //.Replace("@barehtml@", this.GetBareHtml())
            //.Replace("@name@", this.Name)
            //.Replace("@isHidden@", this.Hidden.ToString())

            //    .Replace("@LabelForeColor ", "color:" + ((this.LabelForeColor != null) ? this.LabelForeColor : "@LabelForeColor ") + ";")
            //    .Replace("@LabelBackColor ", "background-color:" + ((this.LabelBackColor != null) ? this.LabelBackColor : "@LabelBackColor ") + ";")
            //    .Replace("@HelpText ", ((this.HelpText != null) ? this.HelpText : "@HelpText "))
            //    .Replace("@Label ", ((this.Label != null) ? this.Label : "@Label "));
            //            return EbCtrlHTML;

            string EbCtrlHTML = HtmlConstants.CONTROL_WRAPER_HTML4WEB
               .Replace("@LabelForeColor ", "color:" + (LabelForeColor ?? "@LabelForeColor ") + ";")
               .Replace("@LabelBackColor ", "background-color:" + (LabelBackColor ?? "@LabelBackColor ") + ";");

            return ReplacePropsInHTML(EbCtrlHTML);
        }

        //INCOMPLETE
        public string GetSelectQuery()
        {
            string Qry = @"
SELECT 
	B.id, B.filename, B.tags, B.uploadts
FROM
	eb_files_ref B
WHERE
	B.context = :context || '_@Name@' AND B.eb_del = 'F';".Replace("@Name@", this.Name?? this.EbSid);

            return Qry;
        }


        [JsonIgnore]
        public override string EnableJSfn { get { return @"$('#cont_' + this.EbSid_CtxId + ' .Col_apndBody, #cont_' + this.EbSid_CtxId + ' .FUP_Head_W').prop('disabled',false).css('pointer-events', 'inherit');"; } set { } }

        [JsonIgnore]
        public override string DisableJSfn { get { return @"$('#cont_' + this.EbSid_CtxId + ' .Col_apndBody, #cont_' + this.EbSid_CtxId + ' .FUP_Head_W').attr('disabled', 'disabled').css('pointer-events', 'none');"; } set { } }

    }

    [EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
    [HideInToolBox]
    public class EbFupCategories : EbControl
    {
        public EbFupCategories()
        {

        }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
        [PropertyGroup("General")]
        public string CategoryTitle { set; get; }
    }
}
