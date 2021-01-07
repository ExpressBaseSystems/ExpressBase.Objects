using ExpressBase.Common.Constants;
using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;

namespace ExpressBase.Objects
{
    [EnableInBuilder(BuilderType.MobilePage)]
    public class EbMobileDashBoardControl : EbMobilePageBase
    {
        [EnableInBuilder(BuilderType.MobilePage)]
        [HideInPropertyGrid]
        public override string Name { get; set; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyGroup(PGConstants.APPEARANCE)]
        public int BorderThickness { get; set; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyGroup(PGConstants.APPEARANCE)]
        [OnChangeExec(@"
            if(this.BorderRadius){
                $(`#${this.EbSid}`).css('border-radius',`${this.BorderRadius}px`);
            }
            ")]
        public virtual int BorderRadius { get; set; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyGroup(PGConstants.APPEARANCE)]
        [PropertyEditor(PropertyEditorType.Color)]
        [Alias("Border/Shadow Color")]
        [DefaultPropValue("#cccccc")]
        [OnChangeExec(@"
            if(this.BorderColor && this.BoxShadow){
                setShadow($(`#${this.EbSid}`),this.BorderColor);
            }
            else{
                setBorderColor($(`#${this.EbSid}`),this.BorderColor)
            }
            ")]
        public virtual string BorderColor { set; get; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyGroup(PGConstants.APPEARANCE)]
        [PropertyEditor(PropertyEditorType.Color)]
        public virtual string BackgroundColor { set; get; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyGroup(PGConstants.APPEARANCE)]
        [OnChangeExec(@"
            if(this.BorderColor && this.BoxShadow){
                setShadow($(`#${this.EbSid}`),this.BorderColor);
            }
            else{
                setBorderColor($(`#${this.EbSid}`),this.BorderColor)
            }
            ")]
        public virtual bool BoxShadow { set; get; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyEditor(PropertyEditorType.Expandable)]
        [PropertyGroup(PGConstants.APPEARANCE)]
        public virtual EbThickness Margin { set; get; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyEditor(PropertyEditorType.Expandable)]
        [PropertyGroup(PGConstants.APPEARANCE)]
        public virtual EbThickness Padding { set; get; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyGroup(PGConstants.BEHAVIOR)]
        public virtual bool Hidden { set; get; }
    }
}
