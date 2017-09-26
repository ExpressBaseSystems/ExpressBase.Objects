using ExpressBase.Common.JsonConverters;
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

    public class EbEmailBuilder : EbObject
    {
        public EmailPriority Priority { get; set; }

        [JsonConverter(typeof(Base64Converter))]
        public string Subject { get; set; }

        [JsonConverter(typeof(Base64Converter))]
        public string Body { get; set; }
    }
}
