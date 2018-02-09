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
    public class EbLocation : EbControl
    {
        public EbLocation() { }

        [OnDeserialized]
        public void OnDeserializedMethod(StreamingContext context)
        {
            this.BareControlHtml = this.GetBareHtml();
            this.ObjType = this.GetType().Name.Substring(2, this.GetType().Name.Length - 2);
        }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm)]
        [PropertyEditor(PropertyEditorType.Expandable)]
        public Location Position { get; set; }

        public override string GetBareHtml()
        {
            return "<div id='@name@' class='map-cont'></div>"
.Replace("@name@", (this.Name != null) ? this.Name : "@name@");
        }

        public override string GetDesignHtml()
        {
            return GetHtml().RemoveCR().DoubleQuoted();
        }

        public override string GetHtml()
        {
            return @"
            <div id='cont_@name@' Ctype='Location' class='Eb-ctrlContainer' style='@hiddenString'>
                @GetBareHtml@
            </div>"
.Replace("@name@", (this.Name != null) ? this.Name : "@name@")
.Replace("@GetBareHtml@", this.GetBareHtml())
;
        }
    }

    [EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm)]
    public class Location
    {
        public Location() { }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm)]
        [PropertyEditor(PropertyEditorType.Number)]
        public Decimal Latitude { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm)]
        [PropertyEditor(PropertyEditorType.Number)]
        public Decimal Longitude { get; set; }

        public string GetHtml()
        {
            return "";
        }
    }
}
