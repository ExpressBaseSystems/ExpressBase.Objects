﻿using ExpressBase.Common.Extensions;
using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace ExpressBase.Objects
{
    [EnableInBuilder(BuilderType.BotForm, BuilderType.UserControl)]
    public class EbLocations : EbControlUI
	{
        [EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
        [PropertyEditor(PropertyEditorType.Collection)]
        public List<EbLocationCard> LocationCollection { get; set; }


        [EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
        [PropertyEditor(PropertyEditorType.Boolean)]
        public bool showTabed { get; set; }

		[HideInPropertyGrid]
		[EnableInBuilder(BuilderType.BotForm)]
		public override bool IsDisable { get => true; }

		public EbLocations()
        {
            this.LocationCollection = new List<EbLocationCard>();
        }

        [OnDeserialized]
        public void OnDeserializedMethod(StreamingContext context)
        {
            this.BareControlHtml = this.GetBareHtml();
            this.BareControlHtml4Bot = this.BareControlHtml;
            this.ObjType = this.GetType().Name.Substring(2, this.GetType().Name.Length - 2);
        }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
        [HideInPropertyGrid]
        public override bool IsFullViewContol { get => true; set => base.IsFullViewContol = value; }

        [HideInPropertyGrid]
        [JsonIgnore]
        public override string ToolIconHtml { get { return "<i class='fa fa-map'></i>"; } set { } }
        //public override string GetToolHtml()
        //{
        //    return @"<div eb-type='@toolName' class='tool'><i class='fa fa-map'></i>  @toolName</div>".Replace("@toolName", this.GetType().Name.Substring(2));
        //}

        public override string DesignHtml4Bot { get => @"
    <div class='location-cont'>
       <div class='loc-opt-cont'>
          <select class='loc-opt-DD'>
             <option class='loc-opt-btn' value='Locations0_loc0' for='Locations0_loc0'> Select Location</option>
          </select>
       </div>
       <div class='location-box' style='display: block;'>
          <div class='map-div' style='position: relative; overflow: hidden;'>
            <img style='width:100%;height: 100%;' src='/images/LocMapImg1.png'>
          </div>
          <div class='loc-bottom'>
             <div class='loc-label' style='@LabelBackColor  @LabelForeColor font-weight: bold'> Location name </div>
             <div class='loc-content'>
                Address: Lorem ipsum dolor , sit amet, Lorem , qety 673001  consectetur adipiscing elitullamcorper lacus. Phone: 0123456789
             </div>
          </div>
       </div>
    </div>"; set => base.DesignHtml4Bot = value; }
        public override string GetJsInitFunc()
        {
            return @"
this.Init = function(id)
{
    this.LocationCollection.$values.push(new EbObjects.EbLocationCard(id + '_loc0'));
    this.LocationCollection.$values.push(new EbObjects.EbLocationCard(id + '_loc1'));
};";
        }
        
        private string getOptDD()
        {
            string optHTML = "<select class='loc-opt-DD'>";

            foreach (EbLocationCard ec in this.LocationCollection)
                optHTML += "<option class='loc-opt-btn' value='@name@' for='@name@' >@optName@</option>"
.Replace("@optName@", (ec.ShortName != null) ? ((ec.ShortName.Trim() == string.Empty) ? ec.Label.Split(" ")[0] : ec.ShortName) : "@ShortName@")
.Replace("@name@", ec.Name);

            return optHTML + "</select>";
        }

        private string getOptButtons()
        {
            string optHTML = string.Empty;

            foreach (EbLocationCard ec in this.LocationCollection)
                optHTML += "<div class='loc-opt-btn' for='@name@' tabindex='0' style='width:@width@%;'>@optName@</div>"
.Replace("@optName@", (ec.ShortName != null) ?( (ec.ShortName.Trim() == string.Empty) ? ec.Label.Split(" ")[0] : ec.ShortName) :"@ShortName@")
.Replace("@width@", (100 / this.LocationCollection.Count).ToString())
.Replace("@name@", ec.Name);

            return optHTML;
        }

        public override string GetDesignHtml()
        {
            //this.LocationCollection.Add(new EbLocation());
            //this.LocationCollection.Add(new EbLocation());
            //return GetHtml().RemoveCR().DoubleQuoted();
            return @"`
<div id='cont_@name@' Ctype='Locations' class='Eb-ctrlContainer' eb-hidden='@isHidden@'>
    <div class='location-cont'>
       <div class='loc-opt-cont'>
          <select class='loc-opt-DD'>
             <option class='loc-opt-btn' value='Locations0_loc0' for='Locations0_loc0'> Select Location</option>
          </select>
       </div>
       <div class='location-box' style='display: block;'>
          <div class='map-div' style='position: relative; overflow: hidden;'>
            <img style='width:100%;height: 100%;' src='/images/LocMapImg1.png'>
          </div>
          <div class='loc-bottom'>
             <div class='loc-label' style='@LabelBackColor  @LabelForeColor font-weight: bold'> Location name </div>
             <div class='loc-content'>
                Address: Lorem ipsum dolor , sit amet, Lorem , qety 673001  consectetur adipiscing elitullamcorper lacus. Phone: 0123456789
             </div>
          </div>
       </div>
    </div>
</div>
`";
        }

        public override string GetBareHtml()
        {
            string html = @"
                <div id='@ebsid@' class='location-cont'>
                    <div class='loc-opt-cont' loc-count='@loccount@'>
                            @options@
                    </div>"
.Replace("@ebsid@", (this.EbSid != null) ? this.EbSid: "@ebsid@")
.Replace("@loccount@", this.LocationCollection.Count.ToString())
.Replace("@name@", (this.Name != null) ? this.Name : "@name@")
.Replace("@options@", (this.showTabed == true) ? this.getOptButtons() : this.getOptDD());

            foreach (EbLocationCard ec in this.LocationCollection)
                html += ec.GetHtml();

            return html + "</div>"
.Replace("@name@", (this.Name != null) ? this.Name : "@name@");
        }

        public override string GetHtml()
        {
            return @"
            <div id='cont_@name@' Ctype='Locations' class='Eb-ctrlContainer' eb-hidden='@isHidden@'>
                @GetBareHtml@
            </div>"
.Replace("@name@", (this.Name != null) ? this.Name : "@name@")
.Replace("@GetBareHtml@", this.GetBareHtml());
        }

    }

    /// ////////////////////////////////

    [EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
    [HideInToolBox]
    public class EbLocationCard : EbControlUI
	{

        [EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
        [PropertyEditor(PropertyEditorType.Expandable)]
        public LatLng Position { get; set; }
        //public LatLng Lat_Long { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
        public string ContentHTML { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
        public string ShortName { get; set; }

        public EbLocationCard() { }

        public override string GetBareHtml()
        {
            return @"
<div id='@name@_Cont' class='location-box'>
    <div id='@name@' class='map-div'></div>
    <div class='loc-bottom'>
        <div id='@name@Lbl' class='loc-label' style='@LabelBackColor  @LabelForeColor font-weight: bold'> @Label@ </div>
        <div class='loc-content'>
            @ContentHTML@
        </div>
    </div>
</div>"
.Replace("@name@", (this.Name != null) ? this.Name : "@name@")
.Replace("@Label@", this.Label)
.Replace("@ContentHTML@", this.ContentHTML);
        }

        public override string GetHtml()
        {
            return GetBareHtml();
        }
    }

    [EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
    public class LatLng
    {
        public LatLng() { }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
        [PropertyEditor(PropertyEditorType.Number)]
        public Decimal Latitude { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
        [PropertyEditor(PropertyEditorType.Number)]
        public Decimal Longitude { get; set; }

        public string GetHtml()
        {
            return "";
        }
    }
}
