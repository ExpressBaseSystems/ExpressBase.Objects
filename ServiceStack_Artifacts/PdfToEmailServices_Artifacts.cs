using ExpressBase.Common.Data;
using ExpressBase.Common.EbServiceStack.ReqNRes;
using ExpressBase.Objects.EmailRelated;
using ServiceStack;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace ExpressBase.Objects.ServiceStack_Artifacts
{
    [DataContract]
    public class EmailAttachmentMqRequest : EbServiceStackAuthRequest, IReturn<EmailAttachmenResponse>
    {
        [DataMember(Order = 1)]
        public int ObjId  { get; set; }

        [DataMember(Order = 2)]
        public List<Param> Params { get; set; }

    }
    [DataContract]
    public class EmailAttachmenRequest : EbServiceStackAuthRequest, IReturn<EmailAttachmenResponse>
    {
        [DataMember(Order = 1)]
        public int ObjId { get; set; }

        [DataMember(Order =2)]
        public List<Param> Params { get; set; }
    }
    public  class EmailAttachmenResponse
    {
        [DataMember(Order = 1)]
        public ResponseStatus ResponseStatus { get; set; }
    }
}
