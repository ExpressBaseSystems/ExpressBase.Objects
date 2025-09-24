

using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;

namespace ExpressBase.Objects
{
    [EnableInBuilder(BuilderType.WebForm)]
    [HideInToolBox]
    [UsedWithTopObjectParent(typeof(EbObject))]
    [Alias("Field Maps")]
    public class EbButtonPublicFromAttachFieldMaps : EbObject
    {
        [EnableInBuilder(BuilderType.WebForm)]
        [PropertyEditor(PropertyEditorType.Text)]
        [Alias("Source Form Contol Name")]
        public string SourceFormContolName { get; set; }

        [EnableInBuilder(BuilderType.WebForm)]
        [PropertyEditor(PropertyEditorType.Text)]
        [Alias("Desitnation Form Contol Name")]
        public string DesitnationFormContolName { get; set; }

    }
}
