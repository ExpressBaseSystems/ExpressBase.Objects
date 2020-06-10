using ExpressBase.Common;
using ExpressBase.Common.Extensions;
using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using ExpressBase.Objects.Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace ExpressBase.Objects
{
    [HideInToolBox]
    [EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
    class EbImageUploader : EbControlUI
	{

        public EbImageUploader() { }

		//[HideInPropertyGrid]
		//[EnableInBuilder(BuilderType.BotForm)]
		//public override bool IsReadOnly { get => this.IsDisable; }

		[OnDeserialized]
        public void OnDeserializedMethod(StreamingContext context)
        {
            this.BareControlHtml = this.GetBareHtml();
            this.BareControlHtml4Bot = this.BareControlHtml;
            this.ObjType = this.GetType().Name.Substring(2, this.GetType().Name.Length - 2);
        }
        public override string GetHead()
        {
            return string.Empty;
        }

        [HideInPropertyGrid]
        [JsonIgnore]
        public override string ToolIconHtml { get { return "<i class='fa fa-level-up'></i>"; } set { } }

        //public override string GetToolHtml()
        //{
        //    return @"<div eb-type='@toolName' class='tool'><i class='fa fa-image'></i><i class='fa fa-level-up'></i>  @toolName</div>".Replace("@toolName", this.GetType().Name.Substring(2));
        //}

        public override string GetDesignHtml()
        {
            return HtmlConstants.CONTROL_WRAPER_HTML4WEB
            .Replace("@barehtml@", @"
                    <div id='Wraper' class='ctrl-cover'>                             
                        <div class='input-group'style='width: 100 %; '> 
                                 <input id='' ui-inp='' data-ebtype='6' class='date' type='text' name=' tabindex='0' style='width: 100%; display: inline - block; background - color: rgb(255, 255, 255); color: rgb(51, 51, 51);' placeholder=''>
                                 <span class='input-group-addon'>
                                    <i class='fa fa-image'></i>
                                    <i class='fa fa-level-up'></i>
                            </span>
                        </div>
                    </div>
                ").RemoveCR().DoubleQuoted();

            //return this.GetHtml().RemoveCR().GraveAccentQuoted();
        }

        public override string GetBareHtml()
        {
            return @" 
        <div class='input-group' style='width:100%;'>
            <input id='@name@txt' type='text' style='width:100%;' onclick=""$('#@name@').click()"" />
            <input id='@name@' data-toggle='tooltip' title='@toolTipText@' type='file' onchange=""$('#@name@txt').val($(this).val())""  accept='image/*' name='@name@' @value@ @tabIndex@ style='display:none; width:100%; @BackColor@ @ForeColor@ @fontStyle@' @required@ />
            <span class='input-group-addon' onclick=""$('#@name@').click()""> <i id='@name@TglBtn' class='fa  fa-picture-o' aria-hidden='true'></i> </span>
        </div>"
.Replace("@name@", this.Name)
.Replace("@toolTipText@", this.ToolTipText)
.Replace("@value@", "")//"value='" + this.Value + "'")
.Replace("@tabIndex@", "tabindex='" + this.TabIndex + "'")
    .Replace("@BackColor@ ", ("background-color:" + ((this.BackColor != null) ? this.BackColor : "@BackColor@ ") + ";"))
    .Replace("@ForeColor@ ", "color:" + ((this.ForeColor != null) ? this.ForeColor : "@ForeColor@ ") + ";")
.Replace("@required@", " required")//(this.Required && !this.Hidden ? " required" : string.Empty))
;
        }

        public override string GetHtml()
        {
            string EbCtrlHTML = @"
    <div id='cont_@name@' Ctype='ImageUploader' class='Eb-ctrlContainer' eb-hidden='@isHidden@'>
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
