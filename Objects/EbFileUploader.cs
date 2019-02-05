using ExpressBase.Common;
using ExpressBase.Common.Extensions;
using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using ExpressBase.Objects.Helpers;
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

        public EbFileUploader() { }

		[HideInPropertyGrid]
		[EnableInBuilder(BuilderType.BotForm)]
		public override bool IsReadOnly { get => this.ReadOnly; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
        [PropertyGroup("General")]
        public FileClass FileType { set; get; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
        [PropertyGroup("General")]
        [PropertyEditor(PropertyEditorType.Collection)]
        public List<string> Categories { set; get; }

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
            return this.GetHtml().RemoveCR().GraveAccentQuoted();
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
            //    <div id='cont_@name@' Ctype='FileUploader' class='Eb-ctrlContainer' style='@hiddenString'>
            //        <div class='eb-ctrl-label' id='@name@Lbl' style='@LabelBackColor  @LabelForeColor '> @Label </div>
            //       @barehtml@
            //        <span class='helpText'> @HelpText </span>
            //    </div>
            //"
            //.Replace("@barehtml@", this.GetBareHtml())
            //.Replace("@name@", this.Name)
            //.Replace("@hiddenString", this.HiddenString)

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
	B.contextid = :context || '_@EbSid@' AND B.eb_del = false;".Replace("EbSid", this.EbSid);

            return Qry;
        }
    }
}
