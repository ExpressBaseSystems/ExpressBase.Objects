using ExpressBase.Common;
using ExpressBase.Common.Extensions;
using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExpressBase.Objects.Objects
{
    [EnableInBuilder(BuilderType.FilterDialog, BuilderType.WebForm)]
    public class EbCalendarControl : EbControlUI, IEbPlaceHolderControl
    {
        [HideInPropertyGrid]
        [JsonIgnore]
        public override string ToolNameAlias { get { return " Calendar "; } set { } }

        public string GetOptionHtml()
        {
            string _html = string.Empty;
            try
            {
                foreach (var key in Enum.GetValues(typeof(AttendanceType)))
                {
                    _html += string.Format("<option value='{0}'>{1}</option>", key.ToString(), key.ToString());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return _html;
        }
        public override string GetDesignHtml()
        {
            return @"

    <div id='cont_@name@' Ctype='Calendar' class='Eb-ctrlContainer' eb-hidden='@isHidden@'>
        <div id='@name@' class='btn-group bootstrap-select show-tick' style='width: 100%;'><button type='button' class='btn dropdown-toggle btn-default'><span class='filter-option pull-left'>Calendar</span>&nbsp;<span class='bs-caret'><span class='caret'></span></span></button></div>
    </div>".RemoveCR().GraveAccentQuoted();//GetHtmlHelper(RenderMode.Developer).RemoveCR().DoubleQuoted();
        }

        public override string GetHtml()
        {
            return GetHtmlHelper(RenderMode.User);
        }

        public override string GetBareHtml()
        {
            return @"
        <select id='@ebsid@' name='@name@' data-ebtype='@data-ebtype@' style='width: 100%;' class='selectpicker'>
            @options@
        </select>
        <div class='input-group' style='width:100%;'>
            <input id='date' ui-inp data-toggle='tooltip'  class='date' type='text'  style='width:100%; display:inline-block;/>
            <span class='input-group-addon' style='padding: 0px;'> <i  class='fa  fa-calendar' aria-hidden='true'></i> </span>
        </div>
        <div class='input-group' style='width:100%;'>
            <input id='month' ui-inp data-toggle='tooltip'  class='date' type='text'  style='width:100%; display:inline-block;/>
            <span class='input-group-addon' style='padding: 0px;'> <i  class='fa fa-calendar' aria-hidden='true'></i> </span>
        </div>
        <input type='text' class='date' id='datefrom' hidden/><input type='text' class='date' id='dateto' hidden/>
        "
.Replace("@name@", this.Name)
.Replace("@ebsid@", this.EbSid_CtxId)
.Replace("@options@", this.GetOptionHtml())
.Replace("@data-ebtype@", "16");
        }

        private string GetHtmlHelper(RenderMode mode)
        {

            string EbCtrlHTML = HtmlConstants.CONTROL_WRAPER_HTML4WEB
.Replace("@HelpText@", this.HelpText)
.Replace("@Label@", this.Label)
.Replace("@ToolTipText ", this.ToolTipText);
            return ReplacePropsInHTML(EbCtrlHTML);

        }
    }
}
