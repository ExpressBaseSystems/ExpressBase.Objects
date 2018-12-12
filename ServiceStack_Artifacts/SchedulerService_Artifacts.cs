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
    public class ScheduleRequest : EbMqRequest
    {
        [DataMember(Order = 1)]
        public EbTask Task { get; set; }
    }

    public class ScheduleResponse : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public string Token { get; set; }

        [DataMember(Order = 2)]
        public ResponseStatus ResponseStatus { get; set; }

        [DataMember(Order = 3)]
        public string Result { get; set; }
    }
    public class UnscheduleRequest : EbMqRequest
    {
        [DataMember(Order = 1)]
        public string TriggerKey { get; set; }
    }

    public class UnscheduleResponse : IEbSSResponse
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
    public class UnschedulerMQRequest : EbMqRequest
    {
        [DataMember(Order = 1)]
        public string TriggerKey { get; set; }
    }

    public class UnschedulerMQResponse : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public string Token { get; set; }

        [DataMember(Order = 2)]
        public ResponseStatus ResponseStatus { get; set; }

        [DataMember(Order = 3)]
        public string Result { get; set; }
    }

    public class GetAllUsersRequest : EbServiceStackAuthRequest, IReturn<GetAllUsersResponse>
    {

    }

    public class GetAllUsersResponse : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public Dictionary<int, string> Users { get; set; }

        [DataMember(Order = 2)]
        public Dictionary<int, string> UserGroups { get; set; }

        [DataMember(Order = 3)]
        public ResponseStatus ResponseStatus { get; set; }

    }

    public class GetUserEmailsRequest:EbServiceStackAuthRequest,IReturn<GetUserEmailsResponse>
    {
        [DataMember(Order = 1)]
        public string UserIds { get; set; }

        [DataMember(Order = 2)]
        public string UserGroupIds { get; set; }
    }
    public class GetUserEmailsResponse:IEbSSResponse
    {
        [DataMember(Order = 1)]
        public Dictionary<int, string> UserEmails { get; set; }

        [DataMember(Order = 2)]
        public Dictionary<int, string> UserGroupEmails { get; set; }

        [DataMember(Order = 3)]
        public ResponseStatus ResponseStatus { get; set; }
    }
}
