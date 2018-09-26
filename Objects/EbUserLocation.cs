using ExpressBase.Common.Extensions;
using ExpressBase.Common.LocationNSolution;
using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using ExpressBase.Common.Structures;
using ExpressBase.Objects.Helpers;
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
    public class EbUserLocation : EbControlUI
    {
        [EnableInBuilder(BuilderType.FilterDialog)]
        [HideInPropertyGrid]
        public List<EbSimpleSelectOption> Options { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm)]
        public override EbDbTypes EbDbType { get { return EbDbTypes.Decimal; } }

        [EnableInBuilder(BuilderType.FilterDialog)]
        [DefaultPropValue("eb_location_id")]
        [PropertyEditor(PropertyEditorType.Label)]
        public override string Name { get; set; }

        public EbUserLocation()
        {
            this.Options = new List<EbSimpleSelectOption>();
        }

        public string OptionHtml { get; set; }

        public override string GetValueJSfn
        {
            get
            {
                return @"
                    return $('#' + this.EbSid_CtxId + ' option:selected').val();
                ";
            }
            set { }
        }

        public void InitFromDataBase(JsonServiceClient ServiceClient, User _user, Eb_Solution _sol)
        {
            string _html = string.Empty;
            try
            {
                Console.WriteLine("Location:  " + _user.LocationIds);
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
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
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
        <select id='@ebsid@' name='@name@' data-ebtype='@data-ebtype@' style='width: 100%;'>
            @options@
        </select>"
.Replace("@name@", this.Name)
.Replace("@ebsid@", this.EbSid_CtxId)
.Replace("@options@", this.OptionHtml)
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
