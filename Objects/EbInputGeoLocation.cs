﻿using ExpressBase.Common;
using ExpressBase.Common.Extensions;
using ExpressBase.Common.LocationNSolution;
using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using ExpressBase.Common.Structures;
using ExpressBase.Objects.ServiceStack_Artifacts;
using ExpressBase.Security;
using Newtonsoft.Json;
using ServiceStack;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace ExpressBase.Objects
{
    [EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
    public class EbInputGeoLocation : EbControlUI
    {

        [HideInPropertyGrid]
        public override bool Unique { get; set; }

        [HideInPropertyGrid]
        public override bool IsDisable { get; set; }

        [HideInPropertyGrid]
        public override bool DoNotPersist { get; set; }

        [HideInPropertyGrid]
        public override EbScript ValueExpr { get; set; }

        public override bool SelfTrigger { get; set; }

        [HideInPropertyGrid]
        public override EbScript DefaultValueExpression { get; set; }

        [HideInPropertyGrid]
        public override List<EbValidator> Validators { get; set; }

        [HideInPropertyGrid]
        public override EbScript OnChangeFn { get; set; }

        [HideInPropertyGrid]
        public override EbScript HiddenExpr { get; set; }

        [HideInPropertyGrid]
        public override EbScript DisableExpr { get; set; }

        public EbInputGeoLocation() { }

        [OnDeserialized]
        public void OnDeserializedMethod(StreamingContext context)
        {
            this.BareControlHtml = this.GetBareHtml();
            this.BareControlHtml4Bot = this.BareControlHtml;
            this.ObjType = this.GetType().Name.Substring(2, this.GetType().Name.Length - 2);
        }
        public override string GetHtml4Bot()
        {
            return ReplacePropsInHTML(HtmlConstants.CONTROL_WRAPER_HTML4BOT);
        }

        //[EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
        //[PropertyEditor(PropertyEditorType.Expandable)]
        //public LatLng Position { get; set; }

        //[EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
        //public string ContentHTML { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
        [DefaultPropValue("400")]
        public override int Height { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
        [HideInPropertyGrid]
        public override bool IsFullViewContol { get => true; set => base.IsFullViewContol = value; }

        //[HideInPropertyGrid]
        //[EnableInBuilder(BuilderType.BotForm)]
        //public override bool IsReadOnly { get => this.IsDisable; }

        public override string GetDesignHtml()
        {
            return GetHtml().RemoveCR().DoubleQuoted();
        }

        [HideInPropertyGrid]
        [JsonIgnore]
        public override string ToolIconHtml { get { return "<i class='fa fa-map-marker'></i>"; } set { } }

        [HideInPropertyGrid]
        [JsonIgnore]
        public override string ToolNameAlias { get { return "LatLng Input"; } set { } }

        //public override string GetToolHtml()
        //{
        //    return @"<div eb-type='@toolName' class='tool'><i class='fa fa-map-marker'></i> Location Input </div>".Replace("@toolName", this.GetType().Name.Substring(2));
        //}

        [EnableInBuilder(BuilderType.WebForm)]
        [HideInPropertyGrid]
        public string DefaultApikey { get; set; }

        public void GetDefaultApikey(JsonServiceClient ServiceClient)
        {
            this.DefaultApikey = ServiceClient.Get<string>(new GetDefaultMapApiKeyFromConnectionRequest());
        }

        public override string GetBareHtml()
        {
            return @" 
                    <div id='@EbSid@_Cont' class='location-box picker-box' style='display:block;'>
                        <div id='@EbSid@' class='map-div' style='height: @Height@;'>@mapimgforbuilder@</div>
                        <div class='locinp-cont'>
                            <div class='locinp-wraper-address'>
                                <div style='display: inline-block; min-width: 5em;'>Address</div>
                                <div class='locinp-address'>
                                    <input id='@EbSid@address' class='locinp' type='text'/><div class='loc-close'><i class='fa fa-times' aria-hidden='true'></i></div>
                                </div>
                            </div>
                            <div>
                                <div class='locinp-wraper'><div class='locinp-span'>Latitude</div><input id='@EbSid@lat' class='locinp' type='text'/></div>
                                <div class='locinp-wraper'><div class='locinp-span'>Longitude</div><input id='@EbSid@long' class='locinp' type='text'/></div>
                            </div>
                        </div>
                        <div class='card-btn-cont' style='margin-top: 8px; margin-bottom: 8px;'>
                            <button id='@EbSid@_subbtn' class='btn btn-default ctrl-submit-btn'  data-toggle='tooltip' title=''> OK </button>
                        </div>
                    </div>  
                "
.Replace("@EbSid@", (this.EbSid != null) ? this.EbSid : "@EbSid@")
.Replace("@mapimgforbuilder@", (this.Name != null) ? string.Empty : "<img style='width:100%;height: 100%;' src='/images/LocMapImg3.png'>")
.Replace("@BackColor@ ", ("background-color:" + ((this.BackColor != null) ? this.BackColor : "@BackColor@ ") + ";"))
.Replace("@ForeColor@ ", "color:" + ((this.ForeColor != null) ? this.ForeColor : "@ForeColor@ ") + ";")
//.Replace("@Height@", this.Height == 0 ? "200px" : this.Height + "px"); // for bot
.Replace("@Height@", this.Height == 0 ? "auto" : this.Height - 61.7 + "px");
        }

        public override string GetHtml()
        {
            string EbCtrlHTML = HtmlConstants.CONTROL_WRAPER_HTML4WEB;
            return ReplacePropsInHTML(EbCtrlHTML);

            //            return @"
            //            <div id='cont_@EbSid@' Ctype='InputGeoLocation' ebsid='@EbSid@' class='Eb-ctrlContainer' eb-hidden='@isHidden@'>
            //                <span class='eb-ctrl-label' ui-label='' id='@EbSidLbl' style=' @BackColor@ @ForeColor@ '>@Label@</span>
            //                    @GetBareHtml@
            //            </div>"
            //.Replace("@EbSid@", (this.EbSid != null) ? this.EbSid : "@EbSid@")
            //.Replace("@Label@", this.Label)
            //.Replace("@LabelBackColor@", this.LabelBackColor)
            //.Replace("@LabelForeColor@", this.LabelForeColor)
            //.Replace("@isHidden@", this.Hidden.ToString())
            //.Replace("@GetBareHtml@", this.GetBareHtml());
        }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl)]
        [HideInPropertyGrid]
        public override EbDbTypes EbDbType { get { return EbDbTypes.String; } set { } }

        [JsonIgnore]
        public override string GetValueFromDOMJSfn
        {
            get
            {
                return @"if (this.__mapVendor === 'osm'){
                            return this.__mapMarker.getLatLng().lat + ',' + this.__mapMarker.getLatLng().lng;
                        }
                        else{
                            let loc = $('#' + this.EbSid_CtxId).locationpicker('location');
                            return loc.latitude + ',' + loc.longitude;
                        }";
            }
            set { }
        }

        [JsonIgnore]
        public override string SetValueJSfn
        {
            get
            {
                return @"if (p1){
                            let tmp = p1.includes(',') ? p1.split(',') : p1.split('/');
                            if (tmp.length === 2 && !isNaN(parseFloat(tmp[0])) && !isNaN(parseFloat(tmp[1]))){
                                if (this.__mapVendor === 'osm'){
                                    this.__mapMarker.setLatLng([parseFloat(tmp[0]), parseFloat(tmp[1])]);
                                    this.__map.setView(this.__mapMarker.getLatLng());
                                    $('#' + this.EbSid_CtxId + 'lat').val(this.__mapMarker.getLatLng().lat);
                                    $('#' + this.EbSid_CtxId + 'long').val(this.__mapMarker.getLatLng().lng);
                                }
                                else
                                    $('#' + this.EbSid_CtxId).locationpicker('location', { latitude : parseFloat(tmp[0]), longitude : parseFloat(tmp[1])});                                
                            }
                        };";
            }
            set { }
        }

        [JsonIgnore]
        public override string SetDisplayMemberJSfn
        {
            get
            {
                return
                    @"this.__isJustSetValue = true; " + SetValueJSfn + ";";
            }
            set { }
        }

        [JsonIgnore]
        public override string GetDisplayMemberFromDOMJSfn
        {
            get
            {
                return @"if (this.__mapVendor === 'osm') return ''; else return $('#' + this.EbSid_CtxId).locationpicker('location').name;";
            }
            set { }
        }

        [JsonIgnore]
        public override string ClearJSfn
        {
            get
            {
                return @"p1 = '0,0';" + SetValueJSfn;
            }
            set { }
        }

        [JsonIgnore]
        public override string OnChangeBindJSFn { get { return @"
if(!this.__ebonchangeFns)
    this.__ebonchangeFns = [];
this.__ebonchangeFns.push(p1);
"; } set { } }


        public override SingleColumn GetSingleColumn(User UserObj, Eb_Solution SoluObj, object Value, bool Default)
        {
            object _formattedData = Value == null || Value.ToString().Trim().Length < 3 ? "0,0" : Value;
            string _displayMember = Value == null ? string.Empty : Value.ToString();

            return new SingleColumn()
            {
                Name = this.Name,
                Type = (int)this.EbDbType,
                Value = _formattedData,
                Control = this,
                ObjType = this.ObjType,
                F = _displayMember
            };
        }

    }
}
