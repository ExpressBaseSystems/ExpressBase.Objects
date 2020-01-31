using ExpressBase.Common;
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

    [EnableInBuilder(BuilderType.UserControl ,BuilderType.DashBoard)]
    public class EbGauge : EbControlUI
    {

        public EbGauge() { }

        [OnDeserialized]
        public void OnDeserializedMethod(StreamingContext context)
        {
            this.BareControlHtml = this.GetBareHtml();
            this.ObjType = this.GetType().Name.Substring(2, this.GetType().Name.Length - 2);
        }

        public override string ToolIconHtml { get { return "<i class='fa fa-tachometer'></i>"; } set { } }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.UserControl, BuilderType.DashBoard)]
        public string DataObjCtrlName { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.UserControl , BuilderType.DashBoard)]
        public string DataObjColName { get; set; }


        [EnableInBuilder(BuilderType.DashBoard)]
        [PropertyEditor(PropertyEditorType.Expandable)]
        public EbGaugeConfig GaugeConfig { get; set; }

        
        //public bool GenerateGradient { get; set; }

        //public bool HighDpiSupport { get; set; }


        public override string UIchangeFns
        {
            get
            {
                return @"";
            }
        }

        public override string GetBareHtml()
        {
            return @"<div class='gaugeChart' style='border:solid 1px'></div>";

        }

        public override string GetDesignHtml()
        {
            return GetHtml().RemoveCR().DoubleQuoted();
        }

        public override string GetHtml()
        {
            string EbCtrlHTML = @"
        <div id='@id' ebsid='@id' name='@name@' class='gaugeChart' eb-type='Gauge'>
        </div>";    
            return ReplacePropsInHTML(EbCtrlHTML);
        }
    }

    public class EbGaugeConfig
    {

        [HideForUser]
        [EnableInBuilder(BuilderType.DashBoard)]
        [PropertyGroup("Appearance")]
        [OnChangeExec(@"if(this.Angle > 100){
            console.log('Hello world!')
            }")]
        public int Angle { get; set; }


        [EnableInBuilder(BuilderType.DashBoard)]
        [UIproperty]
        [PropertyGroup("Appearance")]
        public int LineWidth { get; set; }

        [EnableInBuilder(BuilderType.DashBoard)]
        [UIproperty]
        [PropertyGroup("Appearance")]
        public int RadiusScale { get; set; }

        [EnableInBuilder(BuilderType.DashBoard)]
        [UIproperty]
        [PropertyGroup("Appearance")]
        public int PointerLength { get; set; }

        [EnableInBuilder(BuilderType.DashBoard)]
        [UIproperty]
        [PropertyGroup("Appearance")]
        [DefaultPropValue("50")]
        public int PointerStrokeWidth { get; set; }


        [EnableInBuilder(BuilderType.DashBoard)]
        [UIproperty]
        [PropertyGroup("Appearance")]
        [PropertyEditor(PropertyEditorType.Color)]
        public string PointerColor { get; set; }


        [EnableInBuilder(BuilderType.DashBoard)]
        [UIproperty]
        [PropertyGroup("Appearance")]
        public bool LimitMax { get; set; }


        [EnableInBuilder(BuilderType.DashBoard)]
        [UIproperty]
        [PropertyGroup("Appearance")]
        public bool LimitMin { get; set; }


        [EnableInBuilder(BuilderType.DashBoard)]
        [UIproperty]
        [PropertyGroup("Appearance")]
        [PropertyEditor(PropertyEditorType.Color)]
        public string ColorStart { get; set; }


        [EnableInBuilder(BuilderType.DashBoard)]
        [UIproperty]
        [PropertyGroup("Appearance")]
        [PropertyEditor(PropertyEditorType.Color)]
        public string ColorStop { get; set; }


        [EnableInBuilder(BuilderType.DashBoard)]
        [UIproperty]
        [PropertyGroup("Appearance")]
        [PropertyEditor(PropertyEditorType.Color)]
        public string StrokeColor { get; set; }


        [EnableInBuilder(BuilderType.DashBoard)]
        [HideInPropertyGrid]
        [JsonIgnore]
        public int GaugeValue { get; set; }

        [EnableInBuilder(BuilderType.DashBoard)]
        [HideInPropertyGrid]
        [JsonIgnore]
        public int GaugeContainer { get; set; }
    }
}

