using ExpressBase.Common.Extensions;
using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExpressBase.Objects.Objects
{
    [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm)]
    class EbRadioButton : EbControlUI
	{
        public EbRadioButton() { }

        public override string GetToolHtml()
        {
            return @"<div eb-type='@toolName' class='tool'><i class='fa fa-toggle-on'></i>  @toolName</div>".Replace("@toolName", this.GetType().Name.Substring(2));
        }

        public override string GetDesignHtml()
        {
            //return GetHtml().RemoveCR().DoubleQuoted();
            return @"<div class='toggle btn btn-xs btn-primary' data-toggle='toggle' style='width: 34px; height: 22px;'><input type='checkbox' checked='' data-toggle='toggle' data-size='mini'><div class='toggle-group'><label class='btn btn-primary btn-xs toggle-on'>On</label><label class='btn btn-default btn-xs active toggle-off'>Off</label><span class='toggle-handle btn btn-default btn-xs'></span></div></div>".RemoveCR().DoubleQuoted();
        }

        public override string GetHead()
        {
            return this.RequiredString + @"
$('#@idcontainer [type=checkbox]').bootstrapToggle();
$('#@idcontainer [type=radio]').on('click', function () {
    $(this).button('toggle');
})




".Replace("@id", this.Name);
        }

        public override string GetBareHtml()
        {
            return @"
                        <input id='@namehidden' type ='hidden' name='Ebradio'>
                        <span id='@nameLbl' style='@lblBackColor @LblForeColor'>@label  </span>
                        <div data-toggle='tooltip' title='@toolTipText'>
                        <input id='@name' type = 'checkbox' data-toggle = 'toggle' data-on='@OnValue' data-off='@OffValue'>
                        <span class='helpText'> @helpText </span>"
.Replace("@name", this.Name)
.Replace("@label", this.Label)
.Replace("@toolTipText", this.ToolTipText)
.Replace("@helpText", this.HelpText)
//.Replace("@backColor", "background-color:" + this.BackColorSerialized + ";")
//.Replace("@foreColor", "color:" + this.ForeColorSerialized + ";")
//.Replace("@lblBackColor", "background-color:" + this.LabelBackColorSerialized + ";")
//.Replace("@LblForeColor", "color:" + this.LabelForeColorSerialized + ";");
;
        }

        public override string GetHtml()
        {
            return @"<div id='cont_@name  ' class='Eb-ctrlContainer' Ctype='RadioButton' style='@HiddenString '>
                    </div>"
.Replace("@name", this.Name)
.Replace("@hiddenString", this.HiddenString)
;
        }
    }
}
