using ExpressBase.Common.Extensions;
using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace ExpressBase.Objects
{
    [EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm)]
    class EbFileUploader : EbControlUI
	{

        public EbFileUploader() { }

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
        <div class='input-group' style='width:100%;'>
            <input id='@name@txt' type='text' style='width:100%;' onclick=""$('#@name@').click()""  />
            <input id='@name@' data-toggle='tooltip' title='@toolTipText@' type='file' onchange=""$('#@name@txt').val($(this).val())"" name='@name@' @value@ @tabIndex@ style='display:none; width:100%; @BackColor@ @ForeColor@ @fontStyle@ @readOnlyString@ @required@ />
            <span class='input-group-addon' onclick=""$('#@name@').click()""> <i id='@name@TglBtn' class='fa  fa-file' aria-hidden='true'></i> </span>
        </div>"
.Replace("@name@", this.Name)
.Replace("@toolTipText@", this.ToolTipText)
.Replace("@value@", "")//"value='" + this.Value + "'")
.Replace("@tabIndex@", "tabindex='" + this.TabIndex + "'")
    .Replace("@BackColor@ ", ("background-color:" + ((this.BackColor != null) ? this.BackColor : "@BackColor@ ") + ";"))
    .Replace("@ForeColor@ ", "color:" + ((this.ForeColor != null) ? this.ForeColor : "@ForeColor@ ") + ";")
.Replace("@required@", " required")//(this.Required && !this.Hidden ? " required" : string.Empty))
.Replace("@readOnlyString@", this.ReadOnlyString);
        }

        public override string GetHtml()
        {
            string EbCtrlHTML = @"
    <div id='cont_@name@' Ctype='FileUploader' class='Eb-ctrlContainer' style='@hiddenString'>
        <div class='eb-ctrl-label' id='@name@Lbl' style='@LabelBackColor  @LabelForeColor '> @Label </div>
       @barehtml@
        <span class='helpText'> @HelpText </span>
    </div>
"
.Replace("@barehtml@", this.GetBareHtml())
.Replace("@name@", this.Name)
.Replace("@hiddenString", this.HiddenString)

    .Replace("@LabelForeColor ", "color:" + ((this.LabelForeColor != null) ? this.LabelForeColor : "@LabelForeColor ") + ";")
    .Replace("@LabelBackColor ", "background-color:" + ((this.LabelBackColor != null) ? this.LabelBackColor : "@LabelBackColor ") + ";")
    .Replace("@HelpText ", ((this.HelpText != null) ? this.HelpText : "@HelpText "))
    .Replace("@Label ", ((this.Label != null) ? this.Label : "@Label "));
            return EbCtrlHTML;
        }
    }
}
