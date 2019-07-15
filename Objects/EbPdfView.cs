
using ExpressBase.Common;
using ExpressBase.Common.Data;
using ExpressBase.Common.Extensions;
using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using ExpressBase.Common.Structures;
using ExpressBase.Objects.Helpers;
using ExpressBase.Objects.ServiceStack_Artifacts;
using ServiceStack;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
namespace ExpressBase.Objects
{
    [HideInToolBox]
    [EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
    class EbPdfView : EbControlUI
    {

        public EbPdfView() { }

        [OnDeserialized]
        public void OnDeserializedMethod(StreamingContext context)
        {
            this.BareControlHtml = this.GetBareHtml();
            this.ObjType = this.GetType().Name.Substring(2, this.GetType().Name.Length - 2);
        }

        [EnableInBuilder(BuilderType.BotForm)]
        [PropertyEditor(PropertyEditorType.ObjectSelector)]
        public string DataSourceId { get; set; }

        public override bool isFullViewContol { get => true; set => base.isFullViewContol = value; }

		[HideInPropertyGrid]
		[EnableInBuilder(BuilderType.BotForm)]
		public override bool IsReadOnly { get => true; }

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
                                    <i class='fa fa-file-pdf-o fa-2x'></i>
                            </span>
                        </div>
                    </div>
                ").RemoveCR().DoubleQuoted();
            //return this.GetHtml().RemoveCR().GraveAccentQuoted();
        }
        public override string GetToolHtml()
        {
            return @"<div eb-type='@toolName' class='tool'><i class='fa fa-file-pdf'></i>  @toolName</div>".Replace("@toolName", this.GetType().Name.Substring(2));
        }

        public override string GetBareHtml()
        {
            return @"
        <div class='ebimg-cont' style='width:100%;'>
            <div class='pdf-extbtn' onclick=""window.open($(this).next().attr('src'))""><i class='fa fa-external-link'></i></div>
            <iframe class='pdf-frame' id='@name@' src='@src@'  style='width:100%;'></iframe>
        </div>"
.Replace("@name@", this.Name)
.Replace("@toolTipText@", this.ToolTipText)
.Replace("@src@", "../ReportRender/RenderforBot?refid=@refid@&Params=@Params@".Replace("@refid@", (this.DataSourceId.IsNullOrEmpty()) ? "eb_dbpjl5pgxleq20180130063835-eb_dbpjl5pgxleq20180130063835-3-1603-2339" : this.DataSourceId))
.Replace("@value@", "");//"value='" + this.Value + "'");
        }

        public override string GetHtml()
        {
            string EbCtrlHTML = @"
    <div id='cont_@name@' Ctype='Image' class='Eb-ctrlContainer' eb-hidden='@isHidden@'>
        <div class='eb-ctrl-label' id='@name@Lbl' style='@LabelBackColor  @LabelForeColor '> @Label </div>
       @barehtml@
        <span class='helpText'> @HelpText </span>
    </div>
"
.Replace("@barehtml@", this.GetBareHtml())
.Replace("@name@", this.Name)
.Replace("@isHidden@", this.Hidden.ToString())

    .Replace("@LabelForeColor ", "color:" + ((this.LabelForeColor != null) ? this.LabelForeColor : "@LabelForeColor ") + ";")
    .Replace("@LabelBackColor ", "background-color:" + ((this.LabelBackColor != null) ? this.LabelBackColor : "@LabelBackColor ") + ";")
    .Replace("@HelpText ", ((this.HelpText != null) ? this.HelpText : "@HelpText "))
    .Replace("@Label ", ((this.Label != null) ? this.Label : "@Label "));
            return EbCtrlHTML;
        }
    }
}