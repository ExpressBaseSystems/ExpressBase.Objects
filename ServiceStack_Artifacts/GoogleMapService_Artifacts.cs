using System;
using System.Collections.Generic;
using System.Text;
using ServiceStack;
using System.Runtime.Serialization;
using ExpressBase.Common.EbServiceStack.ReqNRes;

namespace ExpressBase.Objects.ServiceStack_Artifacts
{
    public class GoogleMapRequest : IReturn<GoogleMapResponse>, IEbSSRequest
    {
        [DataMember(Order = 1)]
        public string TenantAccountId { get; set; }

        [DataMember(Order = 2)]
        public int UserId { get; set; }
    }

    public class GoogleMapResponse : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public List<EbGoogleData> Data { get; set; }

        [DataMember(Order = 2)]
        public string TenantAccountId { get; set; }

        [DataMember(Order = 3)]
        public int UserId { get; set; }

        public ResponseStatus ResponseStatus { get; set ; }
    }

    public class EbGoogleData
    {
        [DataMember(Order = 1)]
        public string lat { get; set; }

        [DataMember(Order = 2)]
        public string lon { get; set; }

        [DataMember(Order = 3)]
        public string name { get; set; }

        public EbGoogleData() { }
    }
}
