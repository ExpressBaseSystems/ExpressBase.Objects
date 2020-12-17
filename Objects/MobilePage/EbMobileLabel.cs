using ExpressBase.Common;
using ExpressBase.Common.Constants;
using ExpressBase.Common.Extensions;
using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using System.Collections.Generic;

namespace ExpressBase.Objects
{
    [EnableInBuilder(BuilderType.MobilePage)]
    public class EbMobileLabel : EbMobileControl, INonPersistControl, IMobileUIControl
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
        [PropertyGroup(PGConstants.CORE)]
        [DefaultPropValue("Label")]
        [UIproperty]
        [OnChangeExec(@"
                $(`#${this.EbSid} .mobile-lbl-text`).text(this.Text);
            ")]
        public string Text { get; set; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyGroup(PGConstants.CORE)]
        public bool RenderAsIcon { set; get; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyGroup(PGConstants.CORE)]
        public new string Icon { set; get; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyGroup(PGConstants.ALIGNMENT)]
        public int RowSpan { set; get; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyGroup(PGConstants.ALIGNMENT)]
        public int ColumnSpan { set; get; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyGroup(PGConstants.ALIGNMENT)]
        [Alias("Align X")]
        public MobileHorrizontalAlign HorrizontalAlign { set; get; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyGroup(PGConstants.ALIGNMENT)]
        [Alias("Align Y")]
        public MobileVerticalAlign VerticalAlign { set; get; }

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
        public int BorderThickness { get; set; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyGroup(PGConstants.APPEARANCE)]
        [PropertyEditor(PropertyEditorType.Color)]
        public string BorderColor { get; set; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyGroup(PGConstants.APPEARANCE)]
        [PropertyEditor(PropertyEditorType.FontSelector)]
        public EbFont Font { get; set; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyGroup(PGConstants.APPEARANCE)]
        public MobileTextWrap TextWrap { set; get; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyGroup(PGConstants.APPEARANCE)]
        [PropertyEditor(PropertyEditorType.Expandable)]
        public EbThickness Padding { set; get; }

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
}
