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
    public class PdfCreateServiceMqRequest : EbServiceStackRequest, IReturn<PdfCreateServicesResponse>
    {
        [DataMember(Order = 1)]
        public string Refid  { get; set; }

    }
    [DataContract]
    public class PdfCreateServiceRequest : EbServiceStackRequest, IReturn<PdfCreateServicesResponse>
    {
        [DataMember(Order = 1)]
        public string Refid { get; set; }

    }
    public  class PdfCreateServicesResponse
    {
        [DataMember(Order = 2)]
        public ResponseStatus ResponseStatus { get; set; }
    }
}
