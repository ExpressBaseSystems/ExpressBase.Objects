using ExpressBase.Common.Constants;
using ExpressBase.Common.JsonConverters;
using ExpressBase.Common.Objects.Attributes;
using ExpressBase.Common.Objects;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using ExpressBase.Common.Structures;

namespace ExpressBase.Objects
{
    public abstract class EbPosFormBase : EbObject
    {
        public override List<string> DiscoverRelatedRefids()
        {
            return new List<string>();
        }
    }

    [EnableInBuilder(BuilderType.PosForm)]
    [BuilderTypeEnum(BuilderType.PosForm)]
    public class EbPosForm : EbPosFormBase, IEBRootObject
    {
        [EnableInBuilder(BuilderType.PosForm)]
        [HideInPropertyGrid]
        public override string RefId { get; set; }

        [EnableInBuilder(BuilderType.PosForm)]
        public override string DisplayName { get; set; }

        [EnableInBuilder(BuilderType.PosForm)]
        public override string Name { get; set; }

        [EnableInBuilder(BuilderType.PosForm)]
        public override string Description { get; set; }

        public bool HideInMenu { get; set; }

        [EnableInBuilder(BuilderType.PosForm)]
        [HideInPropertyGrid]
        public override string VersionNumber { get; set; }

        [EnableInBuilder(BuilderType.PosForm)]
        [HideInPropertyGrid]
        public override string Status { get; set; }

        public static EbOperations Operations = PosFormOperations.Instance;

        [EnableInBuilder(BuilderType.PosForm)]
        [PropertyEditor(PropertyEditorType.ObjectSelector)]
        [OSE_ObjectTypes(EbObjectTypes.iWebForm)]
        [Alias("Web Form")]
        [PropertyGroup("Core")]
        public string WebFormRefId { set; get; }
    }
}
