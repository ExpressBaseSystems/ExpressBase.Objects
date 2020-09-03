using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using ExpressBase.Common.Structures;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExpressBase.Objects
{
    [EnableInBuilder(BuilderType.MobilePage)]
    public class EbMobileControl : EbMobilePageBase
    {
        public virtual string EbSid { set; get; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyEditor(PropertyEditorType.MultiLanguageKeySelector)]
        [UIproperty]
        [PropertyGroup("Core")]
        public virtual string Label { set; get; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [HideInPropertyGrid]
        public virtual EbDbTypes EbDbType { get { return EbDbTypes.String; } set { } }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyGroup("Behavior")]
        public virtual bool Hidden { set; get; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [HelpText("Set true if want unique value for this control on every form save.")]
        [PropertyGroup("Behavior")]
        public virtual bool Unique { get; set; }

        [PropertyGroup("Behavior")]
        [EnableInBuilder(BuilderType.MobilePage)]
        [HelpText("Set true if you want to make this control read only.")]
        public virtual bool ReadOnly { get; set; }

        [PropertyGroup("Behavior")]
        [EnableInBuilder(BuilderType.MobilePage)]
        [HelpText("Set true if you dont want to save value from this field.")]
        public virtual bool DoNotPersist { get; set; }

        [PropertyGroup("Behavior")]
        [EnableInBuilder(BuilderType.MobilePage)]
        public virtual bool Required { get; set; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [HideInPropertyGrid]
        public virtual string Icon { get { return string.Empty; } }

        public virtual EbControl GetWebFormCtrl(int counter) { return null; }

        public override List<string> DiscoverRelatedRefids()
        {
            return base.DiscoverRelatedRefids();
        }
    }

    [EnableInBuilder(BuilderType.MobilePage)]
    public class EbMobileDashBoardControls : EbMobilePageBase
    {
        [EnableInBuilder(BuilderType.MobilePage)]
        [HideInPropertyGrid]
        public override string Name { get; set; }
    }
}
