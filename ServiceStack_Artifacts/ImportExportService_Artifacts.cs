using ExpressBase.Common.EbServiceStack.ReqNRes;
using ServiceStack;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace ExpressBase.Objects.ServiceStack_Artifacts
{
        public class RelationTreeRequest : EbServiceStackRequest, IReturn<RelationTreeResponse>
        {
            
        }
        public class RelationTreeResponse : IEbSSResponse
        {
            [DataMember(Order = 2)]
            public string Token { get; set; }

            [DataMember(Order = 3)]
            public ResponseStatus ResponseStatus { get; set; }
        }
}
