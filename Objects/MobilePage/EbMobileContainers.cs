using ExpressBase.Common;
using ExpressBase.Common.Extensions;
using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using ExpressBase.Common.Structures;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExpressBase.Objects
{
    [EnableInBuilder(BuilderType.MobilePage)]
    public class EbMobileContainer : EbMobilePageBase
    {
       
    }

    [EnableInBuilder(BuilderType.MobilePage)]
    public class EbMobileForm : EbMobileContainer
    {
        [EnableInBuilder(BuilderType.MobilePage)]
        public List<EbMobileControl> ChiledControls { get; set; }

        public override string GetDesignHtml()
        {
            return @"<div class='eb_mob_form_container mob_container dropped' eb-type='EbMobileForm' id='@id'>
                        <div class='eb_mob_container_inner'>
                        </div>
                    </div>".RemoveCR().DoubleQuoted();
        }
    }

    [EnableInBuilder(BuilderType.MobilePage)]
    public class EbMobileVisualization : EbMobileContainer
    {
        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyEditor(PropertyEditorType.ObjectSelector)]
        [OSE_ObjectTypes(EbObjectTypes.iDataReader)]
        public string DataSourceRefId { set; get; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [HideInPropertyGrid]
        public EbMobileTableLayout DataLayout { set; get; }

        public override string GetDesignHtml()
        {
            return @"<div class='eb_mob_vis_container mob_container dropped' eb-type='EbMobileVisualization' id='@id'>
                        <div class='eb_mob_container_inner'>
                        </div>
                    </div>".RemoveCR().DoubleQuoted();
        }
    }
}
