using ExpressBase.Common;
using ExpressBase.Common.Extensions;
using ExpressBase.Common.LocationNSolution;
using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using ExpressBase.Common.Structures;
using ExpressBase.Objects.Helpers;
using ExpressBase.Security;
using Newtonsoft.Json;
using ServiceStack;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.Serialization;
using System.Text;

namespace ExpressBase.Objects.Objects
{
    [EnableInBuilder(BuilderType.FilterDialog, BuilderType.WebForm)]
    public class EbUserLocation : EbControlUI, IEbPlaceHolderControl
    {
        [EnableInBuilder(BuilderType.FilterDialog, BuilderType.WebForm)]
        [HideInPropertyGrid]
        public List<EbSimpleSelectOption> Options { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm)]
        public override EbDbTypes EbDbType { get { return EbDbTypes.String; } }

        [EnableInBuilder(BuilderType.FilterDialog, BuilderType.WebForm)]
        [DefaultPropValue("eb_loc_id")]
        //[PropertyEditor(PropertyEditorType.Label)]
        public override string Name { get; set; }

        [EnableInBuilder(BuilderType.FilterDialog, BuilderType.WebForm)]
        public bool LoadCurrentLocation { get; set; }


        public EbUserLocation()
        {
            this.Options = new List<EbSimpleSelectOption>();
        }

        public string OptionHtml { get; set; }

        public override string GetValueFromDOMJSfn
        {
            get
            {
                return @"
let $c = $('#' + this.EbSid_CtxId);
let allsld = $c.next('div').find('[value=multiselect-all').prop('checked');
if (allsld && $c.attr('isglobal') === 'y')
    return '-1';
if ($c.val())
    return $c.val().toString();
return '';";
            }
            set { }
        }


        [JsonIgnore]// roby
        public override string OnChangeBindJSFn
        {
            get
            {
                return @"$('#' + this.EbSid_CtxId).on('change', p1);";
            }
            set { }
        }

        public override string SetValueJSfn
        {
            get
            {
                return @"
let $c = $('#' + this.EbSid_CtxId);
if (p1) {
    if (p1 === '-1')
        $c.multiselect('selectAll').multiselect('refresh');
    else {
        let ar = p1.split(',');
        $c.val(ar).multiselect('refresh');
    }
}
else
    $c.val([]).multiselect('refresh');";
            }
            set { }
        }

        [HideInPropertyGrid]
        [JsonIgnore]
        public override string ToolIconHtml { get { return "<i class='fa fa-user'></i><i class='fa fa-map-marker'></i>"; } set { } }

        [HideInPropertyGrid]
        [JsonIgnore]
        public override string ToolNameAlias { get { return " Location List"; } set { } }
        //public override string GetToolHtml()
        //{
        //    return @"<div eb-type='@toolName' class='tool'><i class='fa fa-user'></i><i class='fa fa-map-marker'></i>  @toolName</div>".Replace("@toolName", this.GetType().Name.Substring(2));
        //}

        public bool IsGlobalLocAvail { get; set; }

        public void InitFromDataBase(User _user, Eb_Solution _sol, string ParentRefid)
        {
            string _html = string.Empty;
            try
            {
                if (_sol?.Locations == null)
                {
                    Console.WriteLine("Solution/Locations is null");
                    throw new Exception("Solution/Locations is null");
                }
                List<int> locList = (ParentRefid == null) ? new List<int> { -1 } : _user.GetLocationsByObject(ParentRefid);
                List<KeyValuePair<int, string>> pairs = new List<KeyValuePair<int, string>>();
                if (locList.Contains(-1))
                {
                    this.IsGlobalLocAvail = true;
                    foreach (KeyValuePair<int, EbLocation> item in _sol.Locations)
                        pairs.Add(new KeyValuePair<int, string>(item.Value.LocId, item.Value.ShortName));
                }
                else
                {
                    foreach (int loc in locList)
                        pairs.Add(new KeyValuePair<int, string>(_sol.Locations[loc].LocId, _sol.Locations[loc].ShortName));
                }
                pairs.Sort((pair1, pair2) => pair1.Value.CompareTo(pair2.Value));
                foreach (KeyValuePair<int, string> item in pairs)
                    _html += $"<option value='{item.Key}'>{item.Value}</option>";
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

    <div id='cont_@name@' Ctype='UserLocation' class='Eb-ctrlContainer' eb-hidden='@isHidden@'>
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
        <select id='@ebsid@' name='@name@' data-ebtype='@data-ebtype@' style='width: 100%;' class='multiselect-ui form-control' multiple='multiple' isglobal='@IsGlobal@'>
            @options@
        </select>"
.Replace("@name@", this.Name)
.Replace("@ebsid@", this.EbSid_CtxId)
.Replace("@IsGlobal@", this.IsGlobalLocAvail ? "y" : "n")
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
