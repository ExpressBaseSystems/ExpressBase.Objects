using ExpressBase.Common.EbServiceStack.ReqNRes;
using ServiceStack;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using ExpressBase.Common.SqlProfiler;

namespace ExpressBase.Objects.ServiceStack_Artifacts
{
    public class GetExecLogsRequest : EbServiceStackAuthRequest, IReturn<GetExecLogsResponse>
    {
        public string RefId { get; set; }
    }

    public class GetProfilersRequest : EbServiceStackAuthRequest, IReturn<GetExecLogsResponse>
    {
        public string RefId { get; set; }
    }

    public class GetExecLogsResponse : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public List<EbExecutionLogs> Logs { get; set; }

        [DataMember(Order = 2)]
        public string Token { get; set; }

        [DataMember(Order = 3)]
        public ResponseStatus ResponseStatus { get; set; }
    }

    public class GetProfilersResponse : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public Profiler Profiler { get; set; }

        [DataMember(Order = 2)]
        public string Token { get; set; }

        [DataMember(Order = 3)]
        public ResponseStatus ResponseStatus { get; set; }
    }
}
