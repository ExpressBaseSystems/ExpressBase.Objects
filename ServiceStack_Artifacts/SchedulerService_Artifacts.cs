using ExpressBase.Common;
using ExpressBase.Common.Data;
using ExpressBase.Common.EbServiceStack.ReqNRes;
using ExpressBase.Scheduler.Jobs;
using ServiceStack;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace ExpressBase.Objects.ServiceStack_Artifacts
{
    public class SchedulerRequest : EbMqRequest
    {
        [DataMember(Order = 1)]
        public EbTask Task { get; set; }
    }

    public class SchedulerResponse : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public string Token { get; set; }

        [DataMember(Order = 2)]
        public ResponseStatus ResponseStatus { get; set; }

        [DataMember(Order = 3)]
        public string Result { get; set; }
    }

    public class SchedulerMQRequest : EbMqRequest
    {
        [DataMember(Order = 1)]
        public EbTask Task { get; set; }
    }

    public class SchedulerMQResponse : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public string Token { get; set; }

        [DataMember(Order = 2)]
        public ResponseStatus ResponseStatus { get; set; }

        [DataMember(Order = 3)]
        public string Result { get; set; }
    }
}
