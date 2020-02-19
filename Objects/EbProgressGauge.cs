using ExpressBase.Common.Extensions;
using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace ExpressBase.Objects
{

    [EnableInBuilder(BuilderType.UserControl, BuilderType.DashBoard)]
    public class EbProgressGauge : EbControlUI
    {

        public EbProgressGauge() { }

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

        public override string UIchangeFns
        {
            get
            {
                return @"";
            }
        }

        public override string GetBareHtml()
        {
            return @"< div class='ProgressGauge guage' style='border:solid 1px'></div>";

        }

        public override string GetDesignHtml()
        {
            return GetHtml().RemoveCR().DoubleQuoted();
        }

        public override string GetHtml()
        {
            string EbCtrlHTML = @"
        <div id='@id' ebsid='@id' name='@name@' class='progressGauge guage' eb-type='ProgressGauge'>
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


        [EnableInBuilder(BuilderType.DashBoard)]
        [UIproperty]
        [PropertyGroup("Appearance")]
        [PropertyEditor(PropertyEditorType.Color)]
        [DefaultPropValue("#08ff3d")]
        public string Color { get; set; }

        [EnableInBuilder(BuilderType.DashBoard)]
        [UIproperty]
        [PropertyGroup("Appearance")]
        [OnChangeExec(@"
                if (this.Gradient === true ){ 
                        pg.ShowProperty('Color2');         
                }
                else {
                        pg.HideProperty('Color2');                          
                }
            ")]
        public bool Gradient { get; set; }

        [EnableInBuilder(BuilderType.DashBoard)]
        [UIproperty]
        [PropertyGroup("Appearance")]
        [PropertyEditor(PropertyEditorType.Color)]
        [DefaultPropValue("#00e4ef")]
        public string Color2 { get; set; }



        [EnableInBuilder(BuilderType.DashBoard)]
        [UIproperty]
        [PropertyGroup("Angle")]
        [DefaultPropValue("-150")]
        public int StartAngle { get; set; }

        [EnableInBuilder(BuilderType.DashBoard)]
        [UIproperty]
        [PropertyGroup("Angle")]
        [DefaultPropValue("150")]
        public int EndAngle { get; set; }

        [EnableInBuilder(BuilderType.DashBoard)]
        [UIproperty]
        [PropertyGroup("Hollow")]
        public bool Hollow { get; set; }

        [EnableInBuilder(BuilderType.DashBoard)]
        [UIproperty]
        [PropertyGroup("Hollow")]
        public int HollowMargin { get; set; }

        [EnableInBuilder(BuilderType.DashBoard)]
        [UIproperty]
        [PropertyGroup("Hollow")]
        [DefaultPropValue("72")]
        public int HollowSize { get; set; }

        [EnableInBuilder(BuilderType.DashBoard)]
        [UIproperty]
        [PropertyGroup("Hollow")]
        [PropertyEditor(PropertyEditorType.Color)]
        [DefaultPropValue("#000040")]
        public string HollowColor { get; set; }



        [EnableInBuilder(BuilderType.DashBoard)]
        [UIproperty]
        [PropertyGroup("Track")]
        public bool DropShadow { get; set; }


        [EnableInBuilder(BuilderType.DashBoard)]
        [UIproperty]
        [PropertyGroup("Track")]
        [PropertyEditor(PropertyEditorType.Color)]
        [DefaultPropValue("#b5b5b5")]
        public string TrackBgColor { get; set; }

        [EnableInBuilder(BuilderType.DashBoard)]
        [UIproperty]
        [PropertyGroup("Track")]
        public int Left { get; set; }

        [EnableInBuilder(BuilderType.DashBoard)]
        [UIproperty]
        [PropertyGroup("Track")]
        public int Top { get; set; }

        [EnableInBuilder(BuilderType.DashBoard)]
        [UIproperty]
        [PropertyGroup("Track")]
        public int Blur { get; set; }

        [EnableInBuilder(BuilderType.DashBoard)]
        [UIproperty]
        [PropertyGroup("Track")]
        public int Opacity { get; set; }

        [EnableInBuilder(BuilderType.DashBoard)]
        [UIproperty]
        [PropertyGroup("Track")]
        public LineCap LineCap { get; set; }


        //DataLabels - Value & Name
        [EnableInBuilder(BuilderType.DashBoard)]
        [UIproperty]
        [PropertyGroup("dataLabels")]
        public string GaugeName { get; set; }

        [EnableInBuilder(BuilderType.DashBoard)]
        [UIproperty]
        [PropertyGroup("dataLabels")]
        public int NameOffsetY { get; set; }

        [EnableInBuilder(BuilderType.DashBoard)]
        [UIproperty]
        [PropertyGroup("dataLabels")]
        [PropertyEditor(PropertyEditorType.Color)]
        [DefaultPropValue("#000000")]
        public string NameColor { get; set; }

        [EnableInBuilder(BuilderType.DashBoard)]
        [UIproperty]
        [PropertyGroup("dataLabels")]
        public int NameFontSize { get; set; }


        [EnableInBuilder(BuilderType.DashBoard)]
        [UIproperty]
        [PropertyGroup("dataLabels")]
        public int ValueFontSize { get; set; }

        [EnableInBuilder(BuilderType.DashBoard)]
        [UIproperty]
        [PropertyGroup("dataLabels")]
        [PropertyEditor(PropertyEditorType.Color)]
        [DefaultPropValue("#000000")]
        public string ValueColor { get; set; }

        [EnableInBuilder(BuilderType.DashBoard)]
        [UIproperty]
        [PropertyGroup("dataLabels")]
        public string ValueUnit { get; set; }



        //[EnableInBuilder(BuilderType.DashBoard)]
        //[UIproperty]
        //[PropertyGroup("Appearance")]
        //[PropertyEditor(PropertyEditorType.Color)]
        //public string Color { get; set; }

    }
}
public enum LineCap
{
    Square = 0,
    Round = 1
}

//startAngle: -150,
//               endAngle: 150,
//               hollow: {
//                   margin: 0,
//                   size: "72%",
//                   background: "#293450"
//               },
//               track: {
//                   dropShadow: {
//                       enabled: true,
//                       top: 2,
//                       left: 0,
//                       blur: 4,
//                       opacity: 0.15
//                   }
//               },
//               dataLabels: {
//                   name: {
//                       offsetY: -10,
//                       color: "#fff",
//                       fontSize: "14px"
//                   },
//                   value: {
//                       color: "#fff",
//                       fontSize: "30px",
//                       show: true,
//                       formatter: function (val) {
//                           return val + '%'


//fill: {
//            type: "gradient",
//            gradient: {
//                shade: "dark",
//                type: "vertical",
//                gradientToColors: ["#87D4F9"],
//                stops: [0, 100]
//            }
//        },
//        stroke: {
//            lineCap: "round"
//        },
//        labels: ["Current"] 