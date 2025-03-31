using ExpressBase.Common.Objects.Attributes;
using ExpressBase.Common.Objects;
using System.Collections.Generic;
using ExpressBase.Common.Structures;
using ExpressBase.Common.Constants;
using ExpressBase.Common;

namespace ExpressBase.Objects
{
    public abstract class EbPrintLayoutBase : EbObject
    {
        public override List<string> DiscoverRelatedRefids()
        {
            return new List<string>();
        }
    }

    [EnableInBuilder(BuilderType.PrintLayout)]
    [BuilderTypeEnum(BuilderType.PrintLayout)]
    public class EbPrintLayout : EbPrintLayoutBase, IEBRootObject
    {
        [EnableInBuilder(BuilderType.PrintLayout)]
        [HideInPropertyGrid]
        public override string RefId { get; set; }

        [EnableInBuilder(BuilderType.PrintLayout)]
        public override string DisplayName { get; set; }

        [EnableInBuilder(BuilderType.PrintLayout)]
        public override string Name { get; set; }

        [EnableInBuilder(BuilderType.PrintLayout)]
        public override string Description { get; set; }

        public bool HideInMenu { get; set; }

        [EnableInBuilder(BuilderType.PrintLayout)]
        [HideInPropertyGrid]
        public override string VersionNumber { get; set; }

        [EnableInBuilder(BuilderType.PrintLayout)]
        [HideInPropertyGrid]
        public override string Status { get; set; }

        public static EbOperations Operations = PrintLayoutOperations.Instance;

        [EnableInBuilder(BuilderType.PrintLayout)]
        [PropertyEditor(PropertyEditorType.ScriptEditorSQ)]
        [HelpText("sql query to get data from offline database")]
        [Alias("Offline Query")]
        [PropertyGroup(PGConstants.CORE)]
        public EbScript OfflineQuery { get; set; }

        [EnableInBuilder(BuilderType.PrintLayout)]
        [PropertyGroup(PGConstants.CORE)]
        public ThermalPrintTemplates ThermalPrintTemplate { set; get; }
    }
}
