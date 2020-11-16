using ExpressBase.Common.Constants;
using ExpressBase.Common.Extensions;
using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using System.Collections.Generic;

namespace ExpressBase.Objects
{
    [EnableInBuilder(BuilderType.MobilePage)]
    public class EbMobileDashBoard : EbMobileContainer
    {
        [EnableInBuilder(BuilderType.MobilePage)]
        [HideInPropertyGrid]
        public override string Name { get; set; }

        [PropertyGroup(PGConstants.APPEARANCE)]
        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyEditor(PropertyEditorType.Expandable)]
        public EbThickness Padding { set; get; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [HideInPropertyGrid]
        public List<EbMobileDashBoardControl> ChildControls { get; set; }

        public override string GetDesignHtml()
        {
            return @"<div class='eb_mob_dashboard_container mob_container dropped' tabindex='1' eb-type='EbMobileDashBoard' id='@id'>
                        <div class='eb_mob_container_inner'>
                        </div>
                    </div>".RemoveCR().DoubleQuoted();
        }

        public override List<string> DiscoverRelatedRefids()
        {
            return base.DiscoverRelatedRefids();
        }

        public EbMobileDashBoard()
        {
            Padding = new EbThickness(10);
        }
    }
}
