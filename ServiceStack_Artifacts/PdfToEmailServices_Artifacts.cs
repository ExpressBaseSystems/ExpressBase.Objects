using ExpressBase.Common.Data;
using ExpressBase.Common.EbServiceStack.ReqNRes;
using ExpressBase.Objects;
using ServiceStack;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace ExpressBase.Objects.ServiceStack_Artifacts
{
    [DataContract]
    public class EmailTemplateWithAttachmentMqRequest : EbServiceStackAuthRequest, IReturn<EmailAttachmenResponse>
    {
        [DataMember(Order = 1)]
        public int ObjId  { get; set; }

        [DataMember(Order = 2)]
        public List<Param> Params { get; set; }

        [DataMember(Order = 3)]
        public string RefId { get; set; } 
        
        [DataMember(Order = 4)]
        public string BToken { get; set; } 
        
        [DataMember(Order = 5)]
        public string RToken { get; set; }

    }
    [DataContract]
    public class EmailAttachmentRequest : EbServiceStackAuthRequest, IReturn<EmailAttachmenResponse>
    {
        [DataMember(Order = 1)]
        public int ObjId { get; set; }

        [DataMember(Order =2)]
        public List<Param> Params { get; set; }

        [DataMember(Order =3)]
        public string RefId { get; set; }

        [DataMember(Order = 4)]
        public string BToken { get; set; }

        [DataMember(Order = 5)]
        public string RToken { get; set; }
    }
    public  class EmailAttachmenResponse
    {
        [DataMember(Order = 1)]
        public ResponseStatus ResponseStatus { get; set; }
    }
}
