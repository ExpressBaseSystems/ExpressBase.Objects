using ExpressBase.Common.JsonConverters;
using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExpressBase.Objects.Objects
{
    public enum EmailPriority
    {
        High,
        Low,
        Medium
    }
    [EnableInBuilder(BuilderType.EmailBuilder)]
    public class EbEmailBuilder : EbObject
    {
        [EnableInBuilder(BuilderType.EmailBuilder)]
        public EmailPriority Priority { get; set; }

        [EnableInBuilder(BuilderType.EmailBuilder)]
        [JsonConverter(typeof(Base64Converter))]
        public string Subject { get; set; }

        [EnableInBuilder(BuilderType.EmailBuilder)]
        [JsonConverter(typeof(Base64Converter))]
        public string Body { get; set; }
    }
}
