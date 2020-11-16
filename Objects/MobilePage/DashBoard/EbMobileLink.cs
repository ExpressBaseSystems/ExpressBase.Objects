using ExpressBase.Common;
using ExpressBase.Common.Extensions;
using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using ExpressBase.Common.Structures;

namespace ExpressBase.Objects
{
    [EnableInBuilder(BuilderType.MobilePage)]
    public class EbMobileLink : EbMobileDashBoardControl
    {
        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyEditor(PropertyEditorType.ObjectSelector)]
        [PropertyGroup("Link Settings")]
        [Alias("Link")]
        [OSE_ObjectTypes(EbObjectTypes.iMobilePage)]
        public string LinkRefId { get; set; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyGroup("Link Settings")]
        [OnChangeExec(@"
            if(this.Title){
                  $(`#${this.EbSid} .eb_mob_link_title`).text(this.Title);
            }
            ")]
        public string Title { get; set; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyGroup("Link Settings")]
        [UIproperty]
        [PropertyEditor(PropertyEditorType.FontSelector)]
        [Alias("Title font style")]
        public EbFont Font { get; set; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyGroup("Link Settings")]
        [PropertyEditor(PropertyEditorType.IconPicker)]
        public string Icon { get; set; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyGroup("Link Settings")]
        [PropertyEditor(PropertyEditorType.Color)]
        public string IconColor { set; get; }

        public override string GetDesignHtml()
        {
            return @"<div class='mob_dash_control dropped' id='@id' eb-type='EbMobileLink' tabindex='1' onclick='$(this).focus()'>                            
                            <div class='eb_dash_ctrlhtml'>
                              <div class ='eb_mob_link_inner'>                                 
                                  <div class='eb_mob_link_icon'><i class='fa fa-external-link-square'></i></div>
                                  <div class='eb_mob_link_title'>Title empty</div>
                               </div>
                            </div>
                        </div>".RemoveCR().DoubleQuoted();
        }

    }
}
