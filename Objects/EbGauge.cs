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

    [EnableInBuilder(BuilderType.UserControl, BuilderType.DashBoard)]
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
        [HideInPropertyGrid]
        public string DataObjCtrlName { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.UserControl, BuilderType.DashBoard)]
        [HideInPropertyGrid]
        public string DataObjColName { get; set; }


        [EnableInBuilder(BuilderType.DashBoard)]
        public override string DisplayName { get; set; }

        [EnableInBuilder(BuilderType.DashBoard)]
        [PropertyEditor(PropertyEditorType.Expandable)]
        [PropertyGroup("GaugeStyle")]
        public EbGaugeConfig GaugeConfig { get; set; }


        [EnableInBuilder(BuilderType.DashBoard)]
        [PropertyGroup("GaugeStyle")]
        [OnChangeExec(@"
                if (this.PointerConfig === true ){ 
                        pg.ShowProperty('Pointer');
                        pg.ShowProperty('PointerLength');
                        pg.ShowProperty('PointerStrokeWidth');
                        pg.ShowProperty('PointerColor');
                }
                else {
                        pg.HideProperty('Pointer');
                        pg.HideProperty('PointerLength');
                        pg.HideProperty('PointerStrokeWidth');
                        pg.HideProperty('PointerColor');                     
                }
            ")]
        public bool PointerConfig { get; set; }



        [EnableInBuilder(BuilderType.DashBoard)]
        [PropertyEditor(PropertyEditorType.Expandable)]
        [PropertyGroup("GaugeStyle")]
        public EbGaugePointer Pointer { get; set; }


        [EnableInBuilder(BuilderType.DashBoard)]
        [PropertyGroup("GaugeStyle")]
        [OnChangeExec(@"
                if (this.RenderTicks === true ){ 
                        pg.ShowProperty('TicksConfig');
                        pg.ShowProperty('Divisions');
                        pg.ShowProperty('DivWidth');
                        pg.ShowProperty('DivLength');
                        pg.ShowProperty('DivColor');
                        pg.ShowProperty('SubDivisions');
                        pg.ShowProperty('SubLength');
                        pg.ShowProperty('SubWidth');
                        pg.ShowProperty('SubColor');
                }
                else {
                        pg.HideProperty('TicksConfig');
                        pg.HideProperty('Divisions');
                        pg.HideProperty('DivWidth');
                        pg.HideProperty('DivLength');
                        pg.HideProperty('DivColor');
                        pg.HideProperty('SubDivisions');
                        pg.HideProperty('SubLength');
                        pg.HideProperty('SubWidth');
                        pg.HideProperty('SubColor');
                }
            ")]
        public bool RenderTicks { get; set; }

        [EnableInBuilder(BuilderType.DashBoard)]
        [PropertyEditor(PropertyEditorType.Expandable)]
        [PropertyGroup("GaugeStyle")]
        public GaugeTicks TicksConfig { get; set; }

        [EnableInBuilder(BuilderType.DashBoard)]
        [UIproperty]
        [PropertyGroup("GaugeStyle")]
        public bool LimitMax { get; set; }



        [EnableInBuilder(BuilderType.DashBoard)]
        [UIproperty]
        [PropertyGroup("GaugeStyle")]
        public bool LimitMin { get; set; }


        [EnableInBuilder(BuilderType.DashBoard)]
        [UIproperty]
        [PropertyGroup("LabelStyle")]
        [Alias("Unit")]
        public string ValueText { get; set; }


        [EnableInBuilder(BuilderType.DashBoard)]
        [UIproperty]
        [PropertyGroup("LabelStyle")]
        [PropertyEditor(PropertyEditorType.FontSelector)]
        public EbFont ValueFont { get; set; }


        [EnableInBuilder(BuilderType.DashBoard)]
        [PropertyGroup("LabelStyle")]
        [PropertyEditor(PropertyEditorType.Expandable)]
        public TextPositon ValuePosition { get; set; }


        [EnableInBuilder(BuilderType.DashBoard)]
        [UIproperty]
        [PropertyGroup("LabelStyle")]
        public string LabelName { get; set; }


        [EnableInBuilder(BuilderType.DashBoard)]
        [UIproperty]
        [PropertyGroup("LabelStyle")]
        [PropertyEditor(PropertyEditorType.FontSelector)]
        public EbFont LabelFont { get; set; }


        [EnableInBuilder(BuilderType.DashBoard)]
        [PropertyGroup("LabelStyle")]
        [PropertyEditor(PropertyEditorType.Expandable)]
        public TextPositon LabelPosition { get; set; }

        //[EnableInBuilder(BuilderType.DashBoard)]
        //[PropertyEditor(PropertyEditorType.Expandable)]
        //[PropertyGroup("GaugeStyle")]
        //public GaugeLabelConfig LabelStyle { get; set; }


        //[EnableInBuilder(BuilderType.DashBoard)]
        //public bool GenerateGradient { get; set; }

        //public bool HighDpiSupport { get; set; }


        //[EnableInBuilder(BuilderType.DashBoard)]
        //[UIproperty]
        //[PropertyEditor(PropertyEditorType.FontSelector)]
        //public EbFont ValueFont { get; set; }

        public override string UIchangeFns
        {
            get
            {
                return @"";
            }
        }

        public override string GetBareHtml()
        {
            return @" < div class='gaugeChart guage' style='border:solid 1px'></div>";

        }

        public override string GetDesignHtml()
        {
            return GetHtml().RemoveCR().DoubleQuoted();
        }

        public override string GetHtml()
        {
            string EbCtrlHTML = @"
        <div id='@id' ebsid='@id' name='@name@' class='gaugeChart guage' eb-type='Gauge'>
        </div>";
            return ReplacePropsInHTML(EbCtrlHTML);
        }
    }

    public class EbGaugeConfig
    {

        [HideForUser]
        [EnableInBuilder(BuilderType.DashBoard)]
        [PropertyGroup("Appearance")]
        [DefaultPropValue("0")]
        [OnChangeExec(@"if(this.Angle > 50){
            this.Angle = 50;
            $('#' + pg.wraperId + 'Angle').val(50);
            }
            else if(this.Angle < -50){
            this.Angle = -50;
            $('#' + pg.wraperId + 'Angle').val(-50);
            }
            ")]
        public int Angle { get; set; }


        [EnableInBuilder(BuilderType.DashBoard)]
        [UIproperty]
        [PropertyGroup("Appearance")]
        [DefaultPropValue("20")]
        [OnChangeExec(@"if(this.LineWidth > 70){
            this.LineWidth = 70;
            $('#' + pg.wraperId + 'LineWidth').val(50);
            }
            else if(this.LineWidth < 0){
            this.LineWidth = 0;
            $('#' + pg.wraperId + 'LineWidth').val(0);
            }
            ")]
        public int LineWidth { get; set; }

        [EnableInBuilder(BuilderType.DashBoard)]
        [UIproperty]
        [PropertyGroup("Appearance")]
        [DefaultPropValue("100")]
        [OnChangeExec(@"if(this.RadiusScale > 100){
            this.RadiusScale = 100;
            $('#' + pg.wraperId + 'RadiusScale').val(100);
            }
            else if(this.RadiusScale < 50){
            this.RadiusScale = 50;
            $('#' + pg.wraperId + 'RadiusScale').val(50);
            }
            ")]
        public int RadiusScale { get; set; }




        [EnableInBuilder(BuilderType.DashBoard)]
        [UIproperty]
        [PropertyGroup("Appearance")]
        [PropertyEditor(PropertyEditorType.Color)]
        [DefaultPropValue("#FE1A1A")]
        public string ColorStart { get; set; }


        [EnableInBuilder(BuilderType.DashBoard)]
        [UIproperty]
        [PropertyGroup("Appearance")]
        [PropertyEditor(PropertyEditorType.Color)]
        [DefaultPropValue("#2b2b2b")]
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
        public string GaugeContainer { get; set; }
    }
    public class EbGaugePointer
    {

        [EnableInBuilder(BuilderType.DashBoard)]
        [UIproperty]
        [PropertyGroup("Appearance")]
        [DefaultPropValue("50")]
        public int PointerLength { get; set; }

        [EnableInBuilder(BuilderType.DashBoard)]
        [UIproperty]
        [PropertyGroup("Appearance")]
        [DefaultPropValue("5")]
        public int PointerStrokeWidth { get; set; }


        [EnableInBuilder(BuilderType.DashBoard)]
        [UIproperty]
        [PropertyGroup("Appearance")]
        [PropertyEditor(PropertyEditorType.Color)]
        [DefaultPropValue("#191717")]
        public string PointerColor { get; set; }
    }

    public class GaugeTicks
    {


        [EnableInBuilder(BuilderType.DashBoard)]
        [OnChangeExec(@"if(this.Divisions > 20){
            this.Divisions = 20;
            $('#' + pg.wraperId + 'Divisions').val(20);
            }
            else if(this.Divisions < 0){
            this.Divisions = 0;
            $('#' + pg.wraperId + 'Divisions').val(0);
            }
            ")]
        public int Divisions { get; set; }

        [EnableInBuilder(BuilderType.DashBoard)]
        [OnChangeExec(@"if(this.DivWidth > 100){
            this.DivWidth = 100;
            $('#' + pg.wraperId + 'DivWidth').val(100);
            }
            else if(this.DivWidth < 0){
            this.DivWidth = 0;
            $('#' + pg.wraperId + 'DivWidth').val(0);
            }
            ")]
        public int DivWidth { get; set; }

        [EnableInBuilder(BuilderType.DashBoard)]
        [OnChangeExec(@"if(this.DivLength > 100){
            this.DivLength = 100;
            $('#' + pg.wraperId + 'DivLength').val(100);
            }
            else if(this.DivLength < 0){
            this.DivLength = 0;
            $('#' + pg.wraperId + 'DivLength').val(0);
            }
            ")]
        public int DivLength { get; set; }

        [EnableInBuilder(BuilderType.DashBoard)]
        [UIproperty]
        [PropertyGroup("Appearance")]
        [PropertyEditor(PropertyEditorType.Color)]
        public string DivColor { get; set; }

        [EnableInBuilder(BuilderType.DashBoard)]
        [OnChangeExec(@"if(this.SubDivisions > 20){
            this.SubDivisions = 20;
            $('#' + pg.wraperId + 'Divisions').val(20);
            }
            else if(this.SubDivisions < 0){
            this.SubDivisions = 0;
            $('#' + pg.wraperId + 'SubDivisions').val(0);
            }
            ")]
        public int SubDivisions { get; set; }

        [EnableInBuilder(BuilderType.DashBoard)]
        [OnChangeExec(@"if(this.SubLength > 100){
            this.SubLength = 100;
            $('#' + pg.wraperId + 'SubLength').val(100);
            }
            else if(this.SubLength < 0){
            this.SubLength = 0;
            $('#' + pg.wraperId + 'SubLength').val(0);
            }
            ")]
        public int SubLength { get; set; }

        [EnableInBuilder(BuilderType.DashBoard)]
        [OnChangeExec(@"if(this.SubWidth > 100){
            this.SubWidth = 100;
            $('#' + pg.wraperId + 'SubWidth').val(100);
            }
            else if(this.SubWidth < 0){
            this.SubWidth = 0;
            $('#' + pg.wraperId + 'SubWidth').val(0);
            }
            ")]
        public int SubWidth { get; set; }

        [EnableInBuilder(BuilderType.DashBoard)]
        [UIproperty]
        [PropertyGroup("Appearance")]
        [PropertyEditor(PropertyEditorType.Color)]
        public string SubColor { get; set; }
    }
    public class GaugeLabelConfig
    {

      

    }
    public class TextPositon
    {

        [EnableInBuilder(BuilderType.DashBoard)]
        [DefaultPropValue("10")]
        public int Left { get; set; }

        [EnableInBuilder(BuilderType.DashBoard)]
        [DefaultPropValue("10")]
        public int Top { get; set; }
    }
}

