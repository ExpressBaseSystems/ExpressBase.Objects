using ExpressBase.Common;
using ExpressBase.Common.Data;
using ExpressBase.Common.EbServiceStack.ReqNRes;
using ExpressBase.Scheduler.Jobs;
using ServiceStack;
using ServiceStack.Auth;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace ExpressBase.Objects.ServiceStack_Artifacts
{
    [DataContract]
    public class TestRequest : EbServiceStackAuthRequest, IReturn<EbDbCreateResponse>
    {
        //[DataMember(Order = 1)]
        //public string dbName { get; set; }
        public EbSurveyQuery Query { get; set; }
    }

    [DataContract]
    public class GenerateAPIKey : EbServiceStackAuthRequest, IReturn<GenerateAPIKeyResponse>
    {
        
    }

    [DataContract]
    public class GenerateAPIKeyResponse : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public List<ApiKey> APIKeys { get; set; }
        [DataMember(Order = 2)]
        public ResponseStatus ResponseStatus { get; set ; }
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
