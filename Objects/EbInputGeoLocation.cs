using ExpressBase.Common.Extensions;
using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using ExpressBase.Common.Structures;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace ExpressBase.Objects
{
    [EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
    public class EbInputGeoLocation : EbControlUI
	{
        #region Hide from PropertyGrid

        [HideInPropertyGrid]
        public override bool Unique { get; set; }

        [HideInPropertyGrid]
        public override bool IsDisable { get; set; }

        [HideInPropertyGrid]
        public override bool DoNotPersist { get; set; }

        [HideInPropertyGrid]
        public override EbScript ValueExpr { get; set; }

        [HideInPropertyGrid]
        public override EbScript DefaultValueExpression { get; set; }

        [HideInPropertyGrid]
        public override List<EbValidator> Validators { get; set; }

        [HideInPropertyGrid]
        public override EbScript OnChangeFn { get; set; }

        [HideInPropertyGrid]
        public override EbScript VisibleExpr { get; set; }

        #endregion

        public EbInputGeoLocation() { }

        [OnDeserialized]
        public void OnDeserializedMethod(StreamingContext context)
        {
            this.BareControlHtml = this.GetBareHtml();
            this.ObjType = this.GetType().Name.Substring(2, this.GetType().Name.Length - 2);
        }

        //[EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
        //[PropertyEditor(PropertyEditorType.Expandable)]
        //public LatLng Position { get; set; }

        //[EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
        //public string ContentHTML { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
        [DefaultPropValue("200")]
        public override int Height { get; set; }

        public override bool isFullViewContol { get => true; set => base.isFullViewContol = value; }

		[HideInPropertyGrid]
		[EnableInBuilder(BuilderType.BotForm)]
		public override bool IsReadOnly { get => this.ReadOnly; }

		public override string GetDesignHtml()
        {
            return GetHtml().RemoveCR().DoubleQuoted();
        }

        [HideInPropertyGrid]
        [JsonIgnore]
        public override string ToolIconHtml { get { return "<i class='fa fa-map-marker'></i>"; } set { } }

        [HideInPropertyGrid]
        [JsonIgnore]
        public override string ToolNameAlias { get { return "Location Input"; } set { } }

        //public override string GetToolHtml()
        //{
        //    return @"<div eb-type='@toolName' class='tool'><i class='fa fa-map-marker'></i> Location Input </div>".Replace("@toolName", this.GetType().Name.Substring(2));
        //}

        public override string GetBareHtml()
        {
            return @" 
                    <div id='@EbSid@_Cont' class='location-box picker-box' style='display:block;'>
                        <div class='locinp-cont'>
                            <div class='locinp-wraper-address'>
                                <div style='display: inline-block; min-width: 50px;'>Address</div>
                                <div style='display: inline-block; min-width: calc(100% - 54px); border: 1px solid rgba(34,36,38,.15);' ><input id='@EbSid@address' type='text' style='width: 100%;' /> </div>
                            </div>
                            <div>
                                <div class='locinp-wraper' style='display: inline-block;'><span class='locinp-span'>Latitude</span><input id='@EbSid@lat' class='locinp' type='text'/></div>
                                <div class='locinp-wraper' style='display: inline-block;'><span class='locinp-span'>Longitude</span><input id='@EbSid@long' class='locinp' type='text'/></div>
                            </div>
                        </div>
                        <div id='@EbSid@' class='map-div' style='height: @Height@;'>@mapimgforbuilder@</div>                        
                    </div>  
                "
.Replace("@EbSid@", (this.EbSid != null) ? this.EbSid : "@EbSid@")
.Replace("@mapimgforbuilder@", (this.Name != null) ? string.Empty : "<img style='width:100%;height: 100%;' src='/images/LocMapImg3.png'>")
.Replace("@BackColor@ ", ("background-color:" + ((this.BackColor != null) ? this.BackColor : "@BackColor@ ") + ";"))
.Replace("@ForeColor@ ", "color:" + ((this.ForeColor != null) ? this.ForeColor : "@ForeColor@ ") + ";")
//.Replace("@Height@", this.Height == 0 ? "200px" : this.Height + "px"); // for bot
.Replace("@Height@", this.Height == 0 ? "auto" : this.Height + "px");
        }

        public override string GetHtml()
        {
            return @"
            <div id='cont_@EbSid@' Ctype='InputGeoLocation' ebsid='@EbSid@' class='Eb-ctrlContainer' eb-hidden='@isHidden@'>
                <span class='eb-ctrl-label' ui-label='' id='@EbSidLbl' style=' @BackColor@ @ForeColor@ '>@Label@</span>
                    @GetBareHtml@
            </div>"
.Replace("@EbSid@", (this.EbSid != null) ? this.EbSid : "@EbSid@")
.Replace("@Label@", this.Label)
.Replace("@LabelBackColor@", this.LabelBackColor)
.Replace("@LabelForeColor@", this.LabelForeColor)
.Replace("@isHidden@", this.Hidden.ToString())
.Replace("@GetBareHtml@", this.GetBareHtml());
        }

        public override EbDbTypes EbDbType { get { return EbDbTypes.String; } set { } }

        [JsonIgnore]
        public override string GetValueJSfn
        {
            get
            {
                return @"let loc = $('#' + this.EbSid_CtxId).locationpicker('location');
                        return loc.latitude + ',' + loc.longitude;";
            }
            set { }
        }

        [JsonIgnore]
        public override string SetValueJSfn
        {
            get
            {
                return @"if(p1){
                            let tmp = p1.split(',');
                            if(tmp.length === 2 && parseFloat(tmp[0]) && parseFloat(tmp[1]))
                                $('#' + this.EbSid_CtxId).locationpicker('location', { latitude : parseFloat(tmp[0]), longitude : parseFloat(tmp[1])});
                        }";
            }
            set { }
        }

    }
}
