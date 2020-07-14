using ExpressBase.Common.EbServiceStack.ReqNRes;
using ServiceStack;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace ExpressBase.Objects.ServiceStack_Artifacts
{
    public class IoTDataRequest : EbServiceStackAuthRequest, IReturn<IoTDataResponse>
    {
        public string Data { get; set; }
    }

    [DataContract]
    public class IoTDataResponse : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public ResponseStatus ResponseStatus { get; set; }

        [DataMember(Order = 2)]
        public string RefId { get; set; }
    }
}
