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
    public class EbMobileLayout : EbMobilePageBase
    {
       
    }

    [EnableInBuilder(BuilderType.MobilePage)]
    public class EbMobileForm : EbMobileLayout
    {
        [EnableInBuilder(BuilderType.MobilePage)]
        public List<EbMobileControls> ChiledControls { get; set; }

        public override string GetDesignHtml()
        {
            return @"<div class='eb_mob_formlayout layout' eb-type='EbMobileForm' id='@id'>
                        <div class='eb_mob_layout_inner'>
                        </div>
                    </div>".RemoveCR().DoubleQuoted();
        }
    }

    [EnableInBuilder(BuilderType.MobilePage)]
    public class EbMobileVisualization : EbMobileLayout
    {
        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyEditor(PropertyEditorType.ObjectSelector)]
        [OSE_ObjectTypes(EbObjectTypes.iDataReader)]
        public string DataSourceRefId { set; get; }

        public override string GetDesignHtml()
        {
            return @"<div class='eb_mob_vislayout layout' eb-type='EbMobileVisualization' id='@id'>
                        <div class='eb_mob_layout_inner'>
                        </div>
                    </div>".RemoveCR().DoubleQuoted();
        }
    }
}
