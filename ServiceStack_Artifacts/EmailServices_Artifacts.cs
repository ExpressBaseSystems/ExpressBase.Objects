using ExpressBase.Common;
using ExpressBase.Common.EbServiceStack.ReqNRes;
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
        public string Cc { get; set; }

        [DataMember(Order = 6)]
        public string Bcc { get; set; }

        [DataMember(Order = 7)]
        // public MemorystreamWrapper AttachmentReport { get; set; }
        public byte[] AttachmentReport { get; set; }

        [DataMember(Order = 7)]
        public string AttachmentName { get; set; }
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
    public class EmailServicesMqRequest : EbMqRequest
    {
        [DataMember(Order = 1)]
        public string From { get; set; }

        [DataMember(Order = 2)]
        public string Subject { get; set; }

        [DataMember(Order = 3)]
        public string Refid { get; set; }

       
    }

}
