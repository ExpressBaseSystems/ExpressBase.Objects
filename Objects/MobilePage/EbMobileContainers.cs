using ExpressBase.Common.Constants;
using ExpressBase.Common.Extensions;
using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using ExpressBase.Common.Structures;
using System.Collections.Generic;

namespace ExpressBase.Objects
{
    [EnableInBuilder(BuilderType.MobilePage)]
    public class EbMobileContainer : EbMobilePageBase
    {
        public override List<string> DiscoverRelatedRefids()
        {
            return base.DiscoverRelatedRefids();
        }
    }

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
        public List<EbMobileDashBoardControls> ChildControls { get; set; }

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

    [EnableInBuilder(BuilderType.MobilePage)]
    public class EbMobilePdf : EbMobileContainer
    {
        [EnableInBuilder(BuilderType.MobilePage)]
        [HideInPropertyGrid]
        public override string Name { get; set; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyEditor(PropertyEditorType.ObjectSelector)]
        [OSE_ObjectTypes(EbObjectTypes.iReport)]
        public string Template { set; get; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyEditor(PropertyEditorType.ScriptEditorCS)]
        [HelpText("sql query to get data from offline database")]
        [Alias("Offline Query")]
        public EbScript OfflineQuery { set; get; }

        public override string GetDesignHtml()
        {
            return @"<div class='eb_mob_pdf_container mob_container dropped' tabindex='1' eb-type='EbMobilePdf' id='@id'>
                        <div class='eb_mob_container_inner'>
                        </div>
                    </div>".RemoveCR().DoubleQuoted();
        }

        public override List<string> DiscoverRelatedRefids()
        {
            return base.DiscoverRelatedRefids();
        }
    }
}
