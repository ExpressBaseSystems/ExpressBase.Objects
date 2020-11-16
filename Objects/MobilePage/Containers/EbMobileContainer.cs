using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
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
}
