using ExpressBase.Common;
using ExpressBase.Common.Constants;
using ExpressBase.Common.Extensions;
using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using ExpressBase.Common.Structures;
using System.Collections.Generic;

namespace ExpressBase.Objects
{
    [EnableInBuilder(BuilderType.MobilePage)]
    public class EbMobileDataColumn : EbMobileControl, INonPersistControl
    {
        public override string Label { set; get; }
        public override bool Unique { get; set; }
        public override bool ReadOnly { get; set; }
        public override bool DoNotPersist { get; set; }
        public override bool Required { get; set; }

        public override EbScript DisableExpr { get; set; }
        public override EbScript DefaultValueExpression { get; set; }
        public override List<EbMobileValidator> Validators { set; get; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [MetaOnly]
        public override string Name { get; set; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [HideInPropertyGrid]
        public int ColumnIndex { get; set; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [UIproperty]
        [MetaOnly]
        public string ColumnName { get; set; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [HideInPropertyGrid]
        public EbDbTypes Type { get; set; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyGroup(PGConstants.APPEARANCE)]
        public int RowSpan { set; get; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyGroup(PGConstants.APPEARANCE)]
        public int ColumnSpan { set; get; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyGroup(PGConstants.CORE)]
        [OnChangeExec(@"
                if ([1,3,5].includes(this.RenderAs)){ 
                        pg.ShowPropertiesExt(['TextFormat','Font','TextWrap']);
                        pg.HidePropertiesExt(['BorderRadius','Height','Width']);
                }
                else {
                        pg.HidePropertiesExt(['TextFormat','TextWrap']);
                        pg.ShowPropertiesExt(['BorderRadius','Height','Width','Font']);
                        if(this.RenderAs === 2){
                            pg.HideProperty('Font');
                        }
                }
            ")]
        public DataColumnRenderType RenderAs { set; get; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [UIproperty]
        [PropertyGroup(PGConstants.APPEARANCE)]
        [PropertyEditor(PropertyEditorType.Color)]
        public string BackgroundColor { get; set; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyGroup(PGConstants.APPEARANCE)]
        public int BorderRadius { get; set; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyGroup(PGConstants.APPEARANCE)]
        [PropertyEditor(PropertyEditorType.FontSelector)]
        public EbFont Font { get; set; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyGroup(PGConstants.APPEARANCE)]
        public string TextFormat { get; set; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyGroup(PGConstants.APPEARANCE)]
        public MobileTextWrap TextWrap { set; get; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyGroup(PGConstants.APPEARANCE)]
        [Alias("Align X")]
        public MobileHorrizontalAlign HorrizontalAlign { set; get; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyGroup(PGConstants.APPEARANCE)]
        [Alias("Align Y")]
        public MobileVerticalAlign VerticalAlign { set; get; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyGroup(PGConstants.APPEARANCE)]
        public int Height { set; get; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyGroup(PGConstants.APPEARANCE)]
        public int Width { set; get; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyGroup(PGConstants.BEHAVIOR)]
        public bool HideInContext { set; get; }

        public override string GetDesignHtml()
        {
            return @"<div class='data_column mob_control dropped' tabindex='1' onclick='$(this).focus()' eb-type='EbMobileDataColumn' id='@id'>
                        <div class='data_column_inner'>
                            <span> @ColumnName </span>
                        </div>
                    </div>".RemoveCR().DoubleQuoted();
        }
    }

    [EnableInBuilder(BuilderType.MobilePage)]
    public class EbMobileDataColToControlMap : EbMobilePageBase
    {
        [EnableInBuilder(BuilderType.MobilePage)]
        [HideInPropertyGrid]
        public string EbSid { set; get; }

        [EnableInBuilder(BuilderType.MobilePage)]
        public string ColumnName { set; get; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [HideInPropertyGrid]
        public EbDbTypes Type { get; set; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [HideInPropertyGrid]
        public EbMobileControlMeta FormControl { set; get; }
    }

    [EnableInBuilder(BuilderType.MobilePage)]
    public class EbMobileControlMeta : EbMobilePageBase
    {
        [EnableInBuilder(BuilderType.MobilePage)]
        [HideInPropertyGrid]
        public string EbSid { set; get; }

        [EnableInBuilder(BuilderType.MobilePage)]
        public string ControlName { set; get; }

        [EnableInBuilder(BuilderType.MobilePage)]
        public string ControlType { set; get; }
    }

    [EnableInBuilder(BuilderType.MobilePage)]
    public class EbCTCMapper : EbMobilePageBase
    {
        [EnableInBuilder(BuilderType.MobilePage)]
        [HideInPropertyGrid]
        public string EbSid { set; get; }

        [EnableInBuilder(BuilderType.MobilePage)]
        public string ColumnName { set; get; }

        [EnableInBuilder(BuilderType.MobilePage)]
        public string ControlName { set; get; }
    }

    [EnableInBuilder(BuilderType.MobilePage)]
    public class EbThickness
    {
        [EnableInBuilder(BuilderType.MobilePage)]
        public int Left { set; get; }

        [EnableInBuilder(BuilderType.MobilePage)]
        public int Top { set; get; }

        [EnableInBuilder(BuilderType.MobilePage)]
        public int Right { set; get; }

        [EnableInBuilder(BuilderType.MobilePage)]
        public int Bottom { set; get; }

        public EbThickness() { }

        public EbThickness(int value)
        {
            Left = Top = Right = Bottom = value;
        }

        public EbThickness(int left, int top, int right, int bottom)
        {
            Left = left;
            Top = top;
            Right = right;
            Bottom = bottom;
        }
    }

    [EnableInBuilder(BuilderType.MobilePage)]
    public class EbMobileValidator : EbMobilePageBase
    {
        [HideInPropertyGrid]
        [EnableInBuilder(BuilderType.MobilePage)]
        public string EbSid { get; set; }

        [EnableInBuilder(BuilderType.MobilePage)]
        public bool IsDisabled { get; set; }

        [EnableInBuilder(BuilderType.MobilePage)]
        public bool IsWarningOnly { get; set; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyEditor(PropertyEditorType.ScriptEditorCS)]
        public EbScript Script { get; set; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [Alias("Failure message")]
        public string FailureMSG { get; set; }
    }

    [EnableInBuilder(BuilderType.MobilePage)]
    public class EbMobileLabel : EbMobileControl, INonPersistControl
    {
        public override string Label { set; get; }
        public override bool Unique { get; set; }
        public override bool ReadOnly { get; set; }
        public override bool DoNotPersist { get; set; }
        public override bool Required { get; set; }

        public override EbScript DisableExpr { get; set; }
        public override EbScript DefaultValueExpression { get; set; }
        public override List<EbMobileValidator> Validators { set; get; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [MetaOnly]
        public override string Name { get; set; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyGroup(PGConstants.APPEARANCE)]
        [DefaultPropValue("Label")]
        [UIproperty]
        [OnChangeExec(@"
                $(`#${this.EbSid} .mobile-lbl-text`).text(this.Text);
            ")]
        public string Text { get; set; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyGroup(PGConstants.APPEARANCE)]
        public bool RenderAsIcon { set; get; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyGroup(PGConstants.APPEARANCE)]
        public new string Icon { set; get; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyGroup(PGConstants.APPEARANCE)]
        [PropertyEditor(PropertyEditorType.FontSelector)]
        public EbFont Font { get; set; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyGroup(PGConstants.APPEARANCE)]
        public MobileTextWrap TextWrap { set; get; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyGroup(PGConstants.APPEARANCE)]
        public int BorderRadius { get; set; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [UIproperty]
        [PropertyGroup(PGConstants.APPEARANCE)]
        [PropertyEditor(PropertyEditorType.Color)]
        public string BackgroundColor { get; set; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyGroup(PGConstants.APPEARANCE)]
        public int RowSpan { set; get; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyGroup(PGConstants.APPEARANCE)]
        public int ColumnSpan { set; get; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyGroup(PGConstants.APPEARANCE)]
        [Alias("Align X")]
        public MobileHorrizontalAlign HorrizontalAlign { set; get; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyGroup(PGConstants.APPEARANCE)]
        [Alias("Align Y")]
        public MobileVerticalAlign VerticalAlign { set; get; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyGroup(PGConstants.APPEARANCE)]
        public int Height { set; get; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyGroup(PGConstants.APPEARANCE)]
        public int Width { set; get; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [MetaOnly]
        public List<EbMobileStaticParameter> BindableParams => new List<EbMobileStaticParameter>();

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyGroup("Static List Settings")]
        [Alias("Binding Parameter")]
        [PropertyEditor(PropertyEditorType.CollectionFrmSrc, "BindableParams", 1)]
        public EbMobileStaticParameter BindingParam { set; get; }

        public override string GetDesignHtml()
        {
            return @"<div class='data_column mob_control dropped' tabindex='1' onclick='$(this).focus()' eb-type='EbMobileLabel' id='@id'>
                        <div class='data_column_inner'>
                            <span class='mobile-lbl-text'> @Text </span>
                        </div>
                    </div>".RemoveCR().DoubleQuoted();
        }
    }

    [EnableInBuilder(BuilderType.MobilePage)]
    public class EbMobileStaticParameter : EbMobilePageBase
    {
        [HideInPropertyGrid]
        [EnableInBuilder(BuilderType.MobilePage)]
        public string EbSid { get; set; }

        [EnableInBuilder(BuilderType.MobilePage)]
        public override string Name { get; set; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyGroup(PGConstants.CORE)]
        public string Value { set; get; }

        [EnableInBuilder(BuilderType.MobilePage)]
        public bool EnableSearch { set; get; }
    }

    [EnableInBuilder(BuilderType.MobilePage)]
    public class EbMobileStaticListItem : EbMobilePageBase
    {
        [HideInPropertyGrid]
        [EnableInBuilder(BuilderType.MobilePage)]
        public string EbSid { get; set; }

        [HideInPropertyGrid]
        [EnableInBuilder(BuilderType.MobilePage)]
        public override string Name { get; set; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyEditor(PropertyEditorType.Collection)]
        [PropertyGroup(PGConstants.CORE)]
        public List<EbMobileStaticParameter> Parameters { set; get; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyEditor(PropertyEditorType.ObjectSelector)]
        [OSE_ObjectTypes(EbObjectTypes.iMobilePage)]
        [Alias("Link")]
        [PropertyGroup(PGConstants.CORE)]
        public string LinkRefId { get; set; }
    }
}
