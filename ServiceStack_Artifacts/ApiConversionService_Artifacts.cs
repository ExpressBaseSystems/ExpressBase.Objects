using ExpressBase.Common;
using ExpressBase.Common.EbServiceStack.ReqNRes;
using ServiceStack;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace ExpressBase.Objects.ServiceStack_Artifacts
{
    [DataContract]
    public class ApiConversionRequest :  IReturn<ApiResponse>, IEbSSRequest
    {
        [DataMember(Order = 8)]
        public string Url { get; set; }

        [DataMember(Order = 8)]
        public ApiMethods Method { get; set; }

        [DataMember(Order = 8)]
        public List<ApiRequestHeader> Headers { get; set; }

        [DataMember(Order = 8)]
        public List<ApiRequestParam> Parameters { get; set; }

        [DataMember(Order = 8)]
        public string SolnId { get; set; }

        [DataMember(Order = 9)]
        public int UserId { get; set; }
    }

    [DataContract]
    public class ApiConversionResponse: IEbSSResponse
    {
        [DataMember(Order = 6)]
        public EbDataSet dataset { get; set; }

        [DataMember(Order = 6)]
        public string Message { get; set; }

        [DataMember(Order = 6)]
        public ResponseStatus ResponseStatus { get; set; }
    }
}
