using ExpressBase.Common.Extensions;
using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExpressBase.Objects
{
    [EnableInBuilder(BuilderType.MobilePage)]
    public class EbMobileLayout : EbMobilePageBase
    {
        [EnableInBuilder(BuilderType.MobilePage)]
        public List<EbMobileControls> ChiledControls { get; set; }
    }

    [EnableInBuilder(BuilderType.MobilePage)]
    public class EbMobileForm : EbMobileLayout
    {
        public override string GetDesignHtml()
        {
            return @"<div class='eb_mob_formlayout' eb-type='MobileForm' id='@id'>
                        <div class='eb_mob_formlayout_inner'>
                        </div>
                    </div>".RemoveCR().DoubleQuoted();
        }
    }
}
