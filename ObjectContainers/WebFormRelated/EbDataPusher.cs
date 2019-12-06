using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using ExpressBase.Common.Structures;

namespace ExpressBase.Objects
{
    [UsedWithTopObjectParent(typeof(EbObject))]
    [EnableInBuilder(BuilderType.WebForm)]
    public class EbDataPusher
    {
        public EbDataPusher() { }

        [PropertyEditor(PropertyEditorType.ObjectSelector)]
        [EnableInBuilder(BuilderType.WebForm)]
        [OSE_ObjectTypes(EbObjectTypes.iWebForm)]
        public string FormRefId { get; set; }

        [PropertyEditor(PropertyEditorType.String)]
        [EnableInBuilder(BuilderType.WebForm)]
        public string Json { get; set; }

        [HideInPropertyGrid]
        [EnableInBuilder(BuilderType.WebForm)]
        public string EbSid { get; set; }

        [EnableInBuilder(BuilderType.WebForm)]
        [Alias("Multi push id")]
        public string Name { get; set; }

        [PropertyEditor(PropertyEditorType.String)]
        [EnableInBuilder(BuilderType.WebForm)]
        public string PushOnlyIf { get; set; }

        [PropertyEditor(PropertyEditorType.String)]
        [EnableInBuilder(BuilderType.WebForm)]
        public string SkipLineItemIf { get; set; }

        public EbWebForm WebForm { get; set; }
    }

    public class EbDataPusherConfig
    {
        public EbDataPusherConfig() { }

        public string SourceTable { get; set; }

        public string MultiPushId { get; set; }

        public bool AllowPush { get; set; }

        public int SourceRecId { get; set; }
    }
}
