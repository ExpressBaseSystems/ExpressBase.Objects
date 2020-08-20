using ExpressBase.Common;
using ExpressBase.Common.Extensions;
using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using ExpressBase.Common.Structures;

namespace ExpressBase.Objects
{
    [EnableInBuilder(BuilderType.MobilePage)]
    public class EbMobileDataColumn : EbMobileControl, INonPersistControl, IMobileDataPart
    {
        public override string Label { set; get; }

        public override bool Unique { get; set; }

        public override bool ReadOnly { get; set; }

        public override bool DoNotPersist { get; set; }

        public override bool Required { get; set; }

        public override bool Hidden { set; get; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [MetaOnly]
        public override string Name { get; set; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [HideInPropertyGrid]
        public int TableIndex { get; set; }

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
        [PropertyGroup("UI")]
        public int RowSpan { set; get; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyGroup("UI")]
        public int ColumnSpan { set; get; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyGroup("UI")]
        [OnChangeExec(@"
                if (this.RenderAs === 1 || this.RenderAs === 5){ 
                        pg.ShowProperty('TextFormat');
                        pg.ShowProperty('Font');
                }
                else {
                        pg.HideProperty('TextFormat');
                        pg.HideProperty('Font');
                }
            ")]
        public DataColumnRenderType RenderAs { set; get; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyGroup("UI")]
        public string TextFormat { get; set; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [UIproperty]
        [PropertyGroup("UI")]
        [PropertyEditor(PropertyEditorType.FontSelector)]
        public EbFont Font { get; set; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyGroup("UI")]
        [Alias("Align X")]
        public MobileHorrizontalAlign HorrizontalAlign { set; get; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyGroup("UI")]
        [Alias("Align Y")]
        public MobileVerticalAlign VerticalAlign { set; get; }

        public override string GetDesignHtml()
        {
            return @"<div class='data_column mob_control dropped' title=' @ColumnName' tabindex='1' onclick='$(this).focus()' eb-type='EbMobileDataColumn' id='@id'>
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
}
