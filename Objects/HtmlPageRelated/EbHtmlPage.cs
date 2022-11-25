using ExpressBase.Common.JsonConverters;
using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using ExpressBase.Common.Structures;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace ExpressBase.Objects
{
    public abstract class EbHtmlPageBase : EbObject
    {
        public override List<string> DiscoverRelatedRefids()
        {
            return new List<string>();
        }
    }

    [EnableInBuilder(BuilderType.HtmlPage)]
    [BuilderTypeEnum(BuilderType.HtmlPage)]
    public class EbHtmlPage : EbHtmlPageBase, IEBRootObject
    {
        public static EbOperations Operations = HtmlPageOperations.Instance;

        [EnableInBuilder(BuilderType.HtmlPage)]
        [HideInPropertyGrid]
        public override string RefId { get; set; }

        [EnableInBuilder(BuilderType.HtmlPage)]
        public override string DisplayName { get; set; }

        [EnableInBuilder(BuilderType.HtmlPage)]
        public override string Name { get; set; }

        [EnableInBuilder(BuilderType.HtmlPage)]
        public override string Description { get; set; }

        public bool HideInMenu { get; set; }

        [EnableInBuilder(BuilderType.HtmlPage)]
        [HideInPropertyGrid]
        public override string VersionNumber { get; set; }

        [EnableInBuilder(BuilderType.HtmlPage)]
        [HideInPropertyGrid]
        public override string Status { get; set; }

        [EnableInBuilder(BuilderType.HtmlPage)]
        [HideInPropertyGrid]
        [JsonConverter(typeof(Base64Converter))]
        public string Html { get; set; }
    }
}
