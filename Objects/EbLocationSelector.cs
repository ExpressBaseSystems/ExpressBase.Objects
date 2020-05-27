using ExpressBase.Common;
using ExpressBase.Common.Extensions;
using ExpressBase.Common.LocationNSolution;
using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using ExpressBase.Common.Structures;
using Newtonsoft.Json;
using ServiceStack;
using System;
using System.Collections.Generic;
using ExpressBase.Security;
using System.Text;

namespace ExpressBase.Objects.Objects
{
    [EnableInBuilder(BuilderType.FilterDialog, BuilderType.WebForm)]
    public class EbLocationSelector: EbControlUI, IEbPlaceHolderControl
    {
        [EnableInBuilder(BuilderType.FilterDialog, BuilderType.WebForm)]
        [HideInPropertyGrid]
        public List<LocTreeData> LocData { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm)]
        public override EbDbTypes EbDbType { get { return EbDbTypes.String; } }

        [EnableInBuilder(BuilderType.FilterDialog, BuilderType.WebForm)]
        [DefaultPropValue("eb_location_id")]
        //[PropertyEditor(PropertyEditorType.Label)]
        public override string Name { get; set; }

        [EnableInBuilder(BuilderType.FilterDialog, BuilderType.WebForm)]
        public bool LoadCurrentLocation { get; set; }

        public EbLocationSelector()
        {
            this.LocData = new List<LocTreeData>();
        }

        public override string GetValueFromDOMJSfn
        {
            get
            {
                return @"
                    var value =( $('#' + this.EbSid_CtxId+'_checkbox').prop('checked') ) ?  -1 : $('#' + this.EbSid_CtxId).val();
                    if(value !== null)
                        return value.toString();
                    else
                        return null;
                ";
            }
            set { }
        }


        [JsonIgnore]// roby
        public override string OnChangeBindJSFn
        {
            get
            {
                return @"$('#' + this.EbSid_CtxId).bind('cssClassChanged', p1);";
            }
            set { }
        }

        public override string SetValueJSfn
        {
            get
            {
                return @"
                    if(p1 !== '-1'){
                        $('.sim-tree-checkbox').toArray().map(el => $(el).hasClass('checked') ? $(el).trigger('click') : console.log('Unchecked'));
                        let xx = p1.split(',');
                        xx.map(qq =>$(`[data-id='${qq}']`).find('.sim-tree-checkbox').eq(0).trigger('click'));
                    }
                    else{
                        $('#' + ctrl.EbSid_CtxId + '_checkbox').trigger('click');                          
                    }
                ";
            }
            set { }
        }

        [HideInPropertyGrid]
        [JsonIgnore]
        public override string ToolIconHtml { get { return "<i class='fa fa-user'></i><i class='fa fa-map-marker'></i>"; } set { } }

        [HideInPropertyGrid]
        [JsonIgnore]
        public override string ToolNameAlias { get { return " Location Selector"; } set { } }
        //public override string GetToolHtml()
        //{
        //    return @"<div eb-type='@toolName' class='tool'><i class='fa fa-user'></i><i class='fa fa-map-marker'></i>  @toolName</div>".Replace("@toolName", this.GetType().Name.Substring(2));
        //}

        public void InitFromDataBase(JsonServiceClient ServiceClient, User _user, Eb_Solution _sol, string ParentRefid)
        {
            try
            {
                List<int> locations = (ParentRefid == null) ? new List<int> { -1 } : _user.GetLocationsByObject(ParentRefid);
                if (locations.Contains(-1))
                {
                    Console.WriteLine("Location: Only -1 " + locations.ToString());
                    if (_sol == null)
                    {
                        Console.WriteLine("Solution null");
                        throw new Exception("Solution null");
                    }
                    else
                    {
                        foreach (var key in _sol.Locations)
                        {
                            this.LocData.Add(new LocTreeData { id= key.Value.LocId, pid=key.Value.ParentId, name=key.Value.LongName});
                        }
                    }
                }
                else
                {
                    Console.WriteLine("===========ObjectId: " + ParentRefid + "Locations: ");
                    foreach (int loc in locations)
                    {
                        this.LocData.Add(new LocTreeData { id = _sol.Locations[loc].LocId, pid = _sol.Locations[loc].ParentId, name = _sol.Locations[loc].LongName });
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public override string GetDesignHtml()
        {
            return @"

    <div id='cont_@name@' Ctype='LocationSelector' class='Eb-ctrlContainer' eb-hidden='@isHidden@'>
        <div id='@name@' class='btn-group bootstrap-select show-tick' style='width: 100%;'>
            <button type='button' class='btn dropdown-toggle btn-default'>
                <span class='filter-option pull-left'>Location Selector</span>&nbsp;<span class='bs-caret'><span class='caret'></span></span>
            </button>
        </div>
    </div>".RemoveCR().GraveAccentQuoted();//GetHtmlHelper(RenderMode.Developer).RemoveCR().DoubleQuoted();
        }

        public override string GetHtml()
        {
            return GetHtmlHelper(RenderMode.User);
        }

        public override string GetBareHtml()
        {
            return @"<div id='@ebsid@_checkbox_div'>
                        <input type='checkbox' id='@ebsid@_checkbox' value='-1' class='userloc-checkbox'>Global
                    </div>
                    <div class=''>
                        <button id='@ebsid@_button' type='button' class='btn-block text-left btn btn-default'>
                            <span class='multiselect-selected-text' disabled='disabled'>All selected (68)</span> 
                            <b class='caret' disabled='disabled'></b>
                        </button>
                    <div id='@ebsid@' name='@name@' data-ebtype='@data-ebtype@'></div>
                    </div>
                    "
.Replace("@name@", this.Name)
.Replace("@ebsid@", this.EbSid_CtxId)
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

    public class LocTreeData
    {
        public int id { get; set; }
        public int pid { get; set; }
        public string name { get; set; }
    }
}
