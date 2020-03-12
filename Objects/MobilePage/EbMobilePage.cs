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
    public enum NetworkMode
    {
        Online,
        Offline,
        Mixed
    }
    public enum EbMobileFupRenderType
    {
        Normal = 1,
        DisplayPicture = 2
    }

    public abstract class EbMobilePageBase : EbObject
    {

    }

    [EnableInBuilder(BuilderType.MobilePage)]
    [BuilderTypeEnum(BuilderType.MobilePage)]
    public class EbMobilePage : EbMobilePageBase, IEBRootObject
    {
        //security model
        public static EbOperations Operations = MobilePageOperations.Instance;

        [EnableInBuilder(BuilderType.MobilePage)]
        [HideInPropertyGrid]
        public override string RefId { get; set; }

        [EnableInBuilder(BuilderType.MobilePage)]
        public override string DisplayName { get; set; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyGroup("Identity")]
        public override string Description { get; set; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyGroup("Identity")]
        [Alias("Mode")]
        [HelpText("Should the page work online or offline or in mixed")]
        public NetworkMode NetworkMode { get; set; }

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
        [PropertyGroup("Behavior")]
        public bool HideFromMenu { set; get; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyGroup("Behavior")]
        [HelpText("FontAwesome unicode string eg: f2b9 ")]
        public string Icon { set; get; }
    }
}
