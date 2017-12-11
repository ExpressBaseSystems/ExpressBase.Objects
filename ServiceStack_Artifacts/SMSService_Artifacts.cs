using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace ExpressBase.Objects.ServiceStack_Artifacts
{

    [DataContract]
    public class SMSSentMqRequest : EbServiceStackRequest
    {
        [DataMember(Order = 1)]
        public string To { get; set; }

        [DataMember(Order = 2)]
        public string From { get; set; }

        [DataMember(Order = 3)]
        public string Body { get; set; }

        [DataMember(Order = 4)]
        public string MediaUrl { get; set; }
    }

    [DataContract]
    public class SMSSentRequest : EbServiceStackRequest
    {
        [DataMember(Order = 1)]
        public string To { get; set; }

        [DataMember(Order = 2)]
        public string From { get; set; }

        [DataMember(Order = 3)]
        public string Body { get; set; }

        [DataMember(Order = 4)]
        public string MediaUrl { get; set; }
    }

    [DataContract]
    public class SMSSentStatus : EbServiceStackRequest
    {
        [DataMember(Order = 1)]
        public string To { get; set; }

        [DataMember(Order = 2)]
        public string From { get; set; }

        [DataMember(Order = 3)]
        public string MessageId { get; set; }

        [DataMember(Order = 4)]
        public string Body { get; set; }

        [DataMember(Order = 5)]
        public string Status { get; set; }

        [DataMember(Order = 6)]
        public DateTime SentTime { get; set; }

        [DataMember(Order = 7)]
        public DateTime ReceivedTime { get; set; }

        [DataMember(Order = 8)]
        public string ErrorMessage { get; set; }

        [DataMember(Order = 9)]
        public string Uri { get; set; }

    }
    [DataContract]
    public class SMSStatusLogMqRequest : EbServiceStackRequest
    {
        [DataMember(Order = 1)]
        public SMSSentStatus SMSSentStatus { get; set; }

        [DataMember(Order = 2)]
        public String ContextId { get; set; }
    }
}