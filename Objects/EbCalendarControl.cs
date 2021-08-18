using ExpressBase.Common;
using ExpressBase.Common.Extensions;
using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using ExpressBase.Common.Structures;
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

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm)]
        public override EbDbTypes EbDbType { get { return EbDbTypes.Date; } }

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

        public override string GetValueFromDOMJSfn
        {
            get
            {
                return @"
                    console.log('datefrom     :'+$('#datefrom').val());
                    console.log('dateto     :'+$('#dateto').val());
                    var value = $('#datefrom').val() + ',' + $('#dateto').val();
                    if(value !== null)
                        return value.toString();
                    else
                        return null;
                ";
            }
            set { }
        }

        public override string OnChangeBindJSFn
        {
            get
            {
                return @"
                    $('#dateto').on('change', p1);
";
            }
            set { }
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
<div id='@ebsid@' name='@name@' data-ebtype='@data-ebtype@' style='width: 100%;'>
        <select id='@ebsid@_dd'  class='selectpicker' style='width: 100%;'>
            @options@
        </select>
        <div id='@ebsid@_date' name='date' ctype='Date' eb-hidden='false'>
            <span class='eb-ctrl-label eb-label-editable' ui-label=''>Date</span>
            <input id='@ebsid@_datelbltxtb' class='eb-lbltxtb' type='text'> 
            <div id='@ebsid@_dateWraper' class='ctrl-cover' eb-readonly='false'>                    
                <div class='input-group' style='width:100%;'>            
                    <input id='date' ui-inp='' data-ebtype='6' data-toggle='tooltip' title='' class='date month-year-input' type='text' name='date' autocomplete='on' tabindex='0' style='width:100%; background-color:@BackColor@ ;color:@ForeColor@ ;display:inline-block; @fontStyle@ ' placeholder='' data-original-title=''>
                    <span class='input-group-addon' style='padding: 0px;'> <i id='@ebsid@_dateTglBtn' class='fa  fa-calendar' aria-hidden='true'></i> </span>
                </div>
            </div>
            <span class='helpText' ui-helptxt=''> </span>
        </div>
        <div id='@ebsid@_month' name='month'  ctype='Date' eb-hidden='false'>
            <span class='eb-ctrl-label eb-label-editable' ui-label=''>month</span>
            <input id='@ebsid@_monthlbltxtb' class='eb-lbltxtb' type='text'> 
            <div id='@ebsid@_monthWraper' class='ctrl-cover' eb-readonly='false'>                    
                <div class='input-group' style='width:100%;'>            
                    <input id='month' ui-inp='' data-ebtype='6' data-toggle='tooltip' title='' class='date month-year-input' type='text' name='month' autocomplete='on' tabindex='0' style='width:100%; background-color:@BackColor@ ;color:@ForeColor@ ;display:inline-block; @fontStyle@ ' placeholder='' data-original-title=''>
                    <span class='input-group-addon' style='padding: 0px;'> <i id='@ebsid@_monthTglBtn' class='fa  fa-calendar' aria-hidden='true'></i> </span>
                </div>
            </div>
            <span class='helpText' ui-helptxt=''> </span>
        </div>
        <div id='@ebsid@_year' name='year'  ctype='Date' eb-hidden='false'>
            <span class='eb-ctrl-label eb-label-editable' ui-label=''>Year</span>
            <input id='@ebsid@_yearlbltxtb' class='eb-lbltxtb' type='text'> 
            <div id='@ebsid@_yearWraper' class='ctrl-cover' eb-readonly='false'>                    
                <div class='input-group' style='width:100%;'>            
                    <input id='year' ui-inp='' data-ebtype='6' data-toggle='tooltip' title='' class='date month-year-input yearpicker' type='text' name='year' autocomplete='on' tabindex='0' style='width:100%; background-color:@BackColor@ ;color:@ForeColor@ ;display:inline-block; @fontStyle@ ' placeholder='' data-original-title=''>
                    <span class='input-group-addon' style='padding: 0px;'> <i id='@ebsid@_yearTglBtn' class='fa  fa-calendar' aria-hidden='true'></i> </span>
                </div>
            </div>
            <span class='helpText' ui-helptxt=''> </span>
        </div>
       Range : <input type='text' class='date' id='datefrom' /*hidden*/ style='width: 30%;'/>
       -<input type='text' class='date' id='dateto' /*hidden*/style='width: 30%;'/>
</div>
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
