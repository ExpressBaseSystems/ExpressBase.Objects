using ExpressBase.Common;
using ExpressBase.Common.Data;
using ExpressBase.Common.EbServiceStack.ReqNRes;
using ExpressBase.Common.Messaging;
using ServiceStack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace ExpressBase.Objects.ServiceStack_Artifacts
{
    [DataContract]
    public class EmailServicesRequest : EbServiceStackAuthRequest, IReturn<EmailServicesResponse>
    {
        [DataMember(Order = 1)]
        public string From { get; set; }

        [DataMember(Order = 2)]
        public string To { get; set; }

        [DataMember(Order = 3)]
        public string Subject { get; set; }

        [DataMember(Order = 4)]
        public string Message { get; set; }

        [DataMember(Order = 5)]
        public string[] Cc { get; set; }

        [DataMember(Order = 6)]
        public string[] Bcc { get; set; }

        [DataMember(Order = 7)]
        // public MemorystreamWrapper AttachmentReport { get; set; }
        public byte[] AttachmentReport { get; set; }

        [DataMember(Order = 7)]
        public string AttachmentName { get; set; }

        [DataMember(Order = 8)]
        public string ReplyTo { get; set; }

        [DataMember(Order = 9)]
        public string RefId { get; set; }

        [DataMember(Order = 10)]
        public List<Param> Params { get; set; }

        [DataMember(Order = 11)]
        public Int32 RetryOf { get; set; }
    }

    [DataContract]
    public class EmailServicesResponse 
    {
        [DataMember(Order = 1)]
        public bool Success { get; set; }

        [DataMember(Order = 2)]
        public ResponseStatus ResponseStatus { get; set; }
    }

    [DataContract]
    public class EmailDirectRequest : EbServiceStackAuthRequest, IReturn<EmailServicesResponse>
    {
        [DataMember(Order = 1)]
        public string From { get; set; }

        [DataMember(Order = 2)]
        public string Subject { get; set; }

        [DataMember(Order = 3)]
        public string To { get; set; }

        [DataMember(Order = 4)]
        public string Message { get; set; }
 

    }

    public class ResetPasswordMqRequest: EbServiceStackNoAuthRequest,IReturn<ResetPasswordMqResponse>
    {
        [DataMember(Order = 1)]
        public string Email { get; set; }

        [DataMember(Order = 2)]
        public string Refid { get; set; }
    }

    public class ResetPasswordMqResponse : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public ResponseStatus ResponseStatus { get; set; }
    }

    [DataContract]
    public class EmailStatusLogMqRequest : EbServiceStackAuthRequest
    {
        [DataMember(Order = 1)]
        public SentStatus SentStatus { get; set; }

        [DataMember(Order = 2)]
        public string RefId { get; set; }

        [DataMember(Order = 3)]
        public string MetaData { get; set; }

        [DataMember(Order = 4)]
        public Int32 RetryOf { get; set; }
    }
}
