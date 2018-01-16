using ServiceStack;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace ExpressBase.Objects.ServiceStack_Artifacts
{
    [DataContract]
    public class TestRequest : EbServiceStackRequest, IReturn<EbDbCreateResponse>
    {
        //[DataMember(Order = 1)]
        //public string dbName { get; set; }
    }
    [DataContract]
    public class TestResponse : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public bool resp { get; set; }

        [DataMember(Order = 2)]
        public string Token { get; set; }

        [DataMember(Order = 3)]
        public ResponseStatus ResponseStatus { get; set; }
    }
}
