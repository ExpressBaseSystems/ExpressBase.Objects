using ExpressBase.Common;
using ExpressBase.Common.Constants;
using ExpressBase.Common.Extensions;
using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;

namespace ExpressBase.Objects
{
    [EnableInBuilder(BuilderType.MobilePage)]
    public class EbMobileDataLabel : EbMobileDashBoardControl
    {
        [EnableInBuilder(BuilderType.MobilePage)]
        [OnChangeExec(@"
            if(this.Text){
                  $(`#${this.EbSid} .eb_dash_datalabel`).text(this.Text);
            }
            ")]
        [UIproperty]
        [PropertyGroup(PGConstants.DATA)]
        [DefaultPropValue("Label")]
        public string Text { get; set; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyEditor(PropertyEditorType.FontSelector)]
        [OnChangeExec(@"
                setFontCss(this.Font,$(`#${this.EbSid} .eb_dash_datalabel`));
            ")]
        [PropertyGroup(PGConstants.DATA)]
        public EbFont Font { get; set; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyGroup(PGConstants.APPEARANCE)]
        public bool RenderAsIcon { set; get; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyGroup(PGConstants.APPEARANCE)]
        public string Icon { set; get; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyGroup(PGConstants.APPEARANCE)]
        [Alias("Align X")]
        public MobileHorrizontalAlign HorrizontalAlign { set; get; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyGroup(PGConstants.APPEARANCE)]
        [Alias("Align Y")]
        public MobileVerticalAlign VerticalAlign { set; get; }

        public override string GetDesignHtml()
        {
            return @"<div class='mob_dash_control dropped' id='@id' eb-type='EbMobileDataLabel' tabindex='1' onclick='$(this).focus()'>                            
                            <div class='eb_dash_ctrlhtml'>
                              <div class ='eb_dash_datalabel'>                                 
                                  @Text
                               </div>
                            </div>
                        </div>".RemoveCR().DoubleQuoted();
        }
    }
}
