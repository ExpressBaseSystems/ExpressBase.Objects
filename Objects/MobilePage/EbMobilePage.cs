using ExpressBase.Common;
using ExpressBase.Common.Extensions;
using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExpressBase.Objects
{
    public abstract class EbMobilePageBase : EbObject
    {

    }

    [EnableInBuilder(BuilderType.MobilePage)]
    [BuilderTypeEnum(BuilderType.MobilePage)]
    public class EbMobilePage : EbMobilePageBase, IEBRootObject
    {
        [EnableInBuilder(BuilderType.MobilePage)]
        [HideInPropertyGrid]
        public override string RefId { get; set; }

        [EnableInBuilder(BuilderType.MobilePage)]
        public override string DisplayName { get; set; }

        [EnableInBuilder(BuilderType.MobilePage)]
        public override string Description { get; set; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [HideInPropertyGrid]
        public override string VersionNumber { get; set; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [HideInPropertyGrid]
        public override string Status { get; set; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [HideInPropertyGrid]
        public EbMobileContainer Container { set; get; }

        public override List<string> DiscoverRelatedRefids() { return new List<string>(); }

        [EnableInBuilder(BuilderType.MobilePage)]
        public bool HideFromMenu { set; get; }
    }
}
