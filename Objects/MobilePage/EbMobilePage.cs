using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using ExpressBase.Common.Structures;
using System.Collections.Generic;

namespace ExpressBase.Objects
{
    public enum NetworkMode
    {
        Online,
        Offline,
        Mixed
    }

    public enum NumericBoxTypes
    {
        TextType = 0,
        ButtonType = 1
    }

    public interface ILinesEnabled { }

    public interface INonPersistControl { }

    public interface ILayoutControl { }

    public abstract class EbMobilePageBase : EbObject { }

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
        [Alias("Behavior")]
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
        [PropertyGroup("Link Style")]
        [HelpText("FontAwesome unicode string eg: f2b9 ")]
        public string Icon { set; get; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyGroup("Link Style")]
        [PropertyEditor(PropertyEditorType.Color)]
        [DefaultPropValue("#333333")]
        public string IconColor { get; set; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyGroup("Link Style")]
        [PropertyEditor(PropertyEditorType.Color)]
        public string IconBackground { get; set; }
    }
}
