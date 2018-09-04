using ExpressBase.Common.Extensions;
using ExpressBase.Common.LocationNSolution;
using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using ExpressBase.Security;
using ServiceStack;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.Serialization;
using System.Text;

namespace ExpressBase.Objects.Objects
{
    [EnableInBuilder(BuilderType.FilterDialog)]
    public class EbUserLocation: EbControlUI
    {
        [EnableInBuilder(BuilderType.FilterDialog)]
        [HideInPropertyGrid]
        public List<EbSimpleSelectOption> Options { get; set; }

        public EbUserLocation()
        {
            this.Options = new List<EbSimpleSelectOption>();
        }

        public string OptionHtml { get; set; }

        public void InitFromDataBase(JsonServiceClient ServiceClient, User _user, Eb_Solution _sol)
        {
            string _html = string.Empty;

            if (_user.LocationIds.Contains(-1))
            {
                foreach (var key in _sol.Locations)
                {
                    _html += string.Format("<option value='{0}'>{1}</option>", key.Value.LocId, key.Value.ShortName);
                }
            }
            else
            {
                foreach (int loc in _user.LocationIds)
                {
                    _html += string.Format("<option value='{0}'>{1}</option>", _sol.Locations[loc].LocId, _sol.Locations[loc].ShortName);
                }
            }

            this.OptionHtml = _html;
        }

        public override string GetDesignHtml()
        {
            return @"

    <div id='cont_@name@' Ctype='UserLocation' class='Eb-ctrlContainer' style='@hiddenString'>
        <div id='@name@' class='btn-group bootstrap-select show-tick' style='width: 100%;'><button type='button' class='btn dropdown-toggle btn-default'><span class='filter-option pull-left'>user location</span>&nbsp;<span class='bs-caret'><span class='caret'></span></span></button></div>
    </div>".RemoveCR().GraveAccentQuoted();//GetHtmlHelper(RenderMode.Developer).RemoveCR().DoubleQuoted();
        }

        public override string GetHtml()
        {
            return GetHtmlHelper(RenderMode.User);
        }

        public override string GetBareHtml()
        {
            return @"
        <select id='@name@' name='@name@' data-ebtype='@data-ebtype@' style='width: 100%;'>
            @options@
        </select>"
.Replace("@name@", this.Name)
.Replace("@options@", this.OptionHtml)
.Replace("@data-ebtype@", "16");
        }

        private string GetHtmlHelper(RenderMode mode)
        {
            return @"
<div id='cont_@name@  ' class='Eb-ctrlContainer' Ctype='TextBox' style='@HiddenString '>
    <div class='eb-ctrl-label' id='@name@Lbl' style='@LabelBackColor@ @LabelForeColor@ '> @Label@  </div>
       @barehtml@
    <span class='helpText'> @HelpText@ </span>
</div>"
.Replace("@barehtml@", this.GetBareHtml())
.Replace("@HelpText@", this.HelpText)
.Replace("@Label@", this.Label)
.Replace("@name@", this.Name)
.Replace("@HiddenString ", this.HiddenString)
.Replace("@ToolTipText ", this.ToolTipText);

        }
    }
}
