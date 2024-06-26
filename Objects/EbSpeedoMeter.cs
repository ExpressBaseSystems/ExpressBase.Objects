﻿using ExpressBase.Common;
using ExpressBase.Common.Extensions;
using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using ExpressBase.Objects.Objects.DVRelated;
using Newtonsoft.Json;
using Org.BouncyCastle.Asn1.Cms;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ExpressBase.Objects
{

    [EnableInBuilder(BuilderType.UserControl, BuilderType.DashBoard)]
    public class EbSpeedoMeter : EbControlUI
    {

        public EbSpeedoMeter() { }

        [OnDeserialized]
        public void OnDeserializedMethod(StreamingContext context)
        {
            this.BareControlHtml = this.GetBareHtml();
            this.ObjType = this.GetType().Name.Substring(2, this.GetType().Name.Length - 2);
        }

        public override string ToolIconHtml { get { return "<i class='fa fa-tachometer'></i>"; } set { } }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.UserControl, BuilderType.DashBoard)]
        [HideInPropertyGrid]
        public string DataObjCtrlName { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.UserControl, BuilderType.DashBoard)]
        [HideInPropertyGrid]
        public string DataObjColName { get; set; }


        [EnableInBuilder(BuilderType.DashBoard)]
        public override string DisplayName { get; set; }

        //[EnableInBuilder(BuilderType.DashBoard)]
        //[PropertyEditor(PropertyEditorType.Expandable)]
        //[PropertyGroup("GaugeStyle")]
        //public EbGaugeConfig GaugeConfig { get; set; }
        public override string UIchangeFns
        {
            get
            {
                return @"";
            }
        }

        public override string GetBareHtml()
        {
            return @" < div class='speedometer guage' style='border:solid 1px'></div>";
        }

        public override string GetDesignHtml()
        {
            return GetHtml().RemoveCR().DoubleQuoted();
        }

        public override string GetHtml()
        {
            string EbCtrlHTML = @"
        <div id='@id' ebsid='@id' name='@name@' class='speedometer guage' eb-type='SpeedoMeter'>
        </div>";
            return ReplacePropsInHTML(EbCtrlHTML);
        }

        [EnableInBuilder(BuilderType.DashBoard)]
        [HideInPropertyGrid]
        [JsonIgnore]
        public int GaugeValue { get; set; }

        [EnableInBuilder(BuilderType.DashBoard)]
        [HideInPropertyGrid]
        [JsonIgnore]
        public string GaugeContainer { get; set; }
    }
}