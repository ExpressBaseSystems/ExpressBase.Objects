using ExpressBase.Common;
using ExpressBase.Common.Data;
using ExpressBase.Common.EbServiceStack.ReqNRes;
using ServiceStack;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace ExpressBase.Objects.ServiceStack_Artifacts
{
    [DataContract]
    public class ApiConversionRequest : EbServiceStackAuthRequest, IReturn<ApiResponse>
    {
        [DataMember(Order = 8)]
        public string Url { get; set; }

        [DataMember(Order = 8)]
        public ApiMethods Method { get; set; }

        [DataMember(Order = 8)]
        public List<ApiRequestHeader> Headers { get; set; }

        [DataMember(Order = 8)]
        public List<Param> Parameters { get; set; }

        [DataMember(Order = 8)]
        public string SolnId { get; set; }

        [DataMember(Order = 9)]
        public int UserId { get; set; }
    }

    [DataContract]
    public class ApiConversionResponse : IEbSSResponse
    {
        [DataMember(Order = 6)]
        public EbDataSet dataset { get; set; }

        [DataMember(Order = 6)]
        public ResponseStatus ResponseStatus { get; set; }

        [DataMember(Order = 6)]
        public int statusCode { get; set; }

        public string Message
        {
            get
            {
                switch (this.statusCode)
                {
                    case 200:
                        return string.Empty;
                    case 201:
                        return "201 Created";
                    case 404:
                        return "Resource not found";
                    case 500:
                        return "An unhandled error occurred";
                    default:
                        return "Invalid URI";
                }
            }

        }
    }
}
