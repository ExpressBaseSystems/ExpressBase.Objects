using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using ExpressBase.Common.EbServiceStack.ReqNRes;
using ServiceStack;

namespace ExpressBase.Objects.ServiceStack_Artifacts
{
    public class FunctionCheckRequest : IEbTenentRequest, IReturn<FunctionCheckResponse>
    {
        public int UserId { get; set; }

        public string SolnId { get; set; }
    }

    public class FunctionCheckResponse : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public ResponseStatus ResponseStatus { get; set; }
    }
    
}
