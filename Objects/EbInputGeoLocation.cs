using ExpressBase.Common.Extensions;
using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace ExpressBase.Objects
{
    [EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm)]
    class EbInputGeoLocation : EbControl
    {
        public EbInputGeoLocation() { }

        [OnDeserialized]
        public void OnDeserializedMethod(StreamingContext context)
        {
            this.BareControlHtml = this.GetBareHtml();
            this.ObjType = this.GetType().Name.Substring(2, this.GetType().Name.Length - 2);
        }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm)]
        [PropertyEditor(PropertyEditorType.Expandable)]
        public LatLng Position { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm)]
        public string ContentHTML { get; set; }

        public override string GetDesignHtml()
        {
            return GetHtml().RemoveCR().DoubleQuoted();
        }

        public override string GetBareHtml()
        {
            return @" 
                <div id='@name@' class='location-cont'>
                    <div id='@name@_Cont' class='location-box'>
                        <div id='@name@' class='map-div'></div>
                        <div class='loc-bottom'>
                            <div id='@name@Lbl' class='loc-label' style='@LabelBackColor@  @LabelForeColor@ font-weight: bold'> @Label@ </div>
                            <div class='loc-content'>
                                @ContentHTML@
                            </div>
                        </div>
                    </div>  
                </div>"
.Replace("@name@", (this.Name != null) ? this.Name : "@name@")
.Replace("@LabelBackColor@", this.LabelBackColor)
.Replace("@LabelForeColor@", this.LabelForeColor)
.Replace("@Label@", this.Label)
.Replace("@ContentHTML@", this.ContentHTML);
            ;
        }

        public override string GetHtml()
        {
            return @"
            <div id='cont_@name@' Ctype='Locations' class='Eb-ctrlContainer' style='@hiddenString'>
                @GetBareHtml@
            </div>"
.Replace("@name@", (this.Name != null) ? this.Name : "@name@")
.Replace("@GetBareHtml@", this.GetBareHtml());
        }
    }
}
