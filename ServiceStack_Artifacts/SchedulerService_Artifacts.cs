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
    public class RescheduleRequest : EbMqRequest
    {
        [DataMember(Order = 1)]
        public EbTask Task { get; set; }

        [DataMember(Order = 2)]
        public string TriggerKey { get; set; }

        [DataMember(Order = 2)]
        public string JobKey { get; set; }

    }

    public class RescheduleResponse : IEbSSResponse
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
    public class DeleteJobRequest : EbMqRequest
    {
        [DataMember(Order = 1)]
        public string JobKey { get; set; }
    }

    public class DeleteJobResponse : IEbSSResponse
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

    public class AddSchedulesToSolutionRequest : EbMqRequest
    {
        [DataMember(Order = 1)]
        public EbTask Task { get; set; }

        [DataMember(Order = 2)]
        public string JobKey { get; set; }

        [DataMember(Order = 3)]
        public string TriggerKey { get; set; }

        [DataMember(Order = 4)]
        public ScheduleStatuses Status { get; set; }

        [DataMember(Order = 5)]
        public int ObjId { get; set; }

        [DataMember(Order = 5)]
        public string Name { get; set; }
    }

    public class DeleteJobMQRequest : EbServiceStackAuthRequest, IReturn<DeleteJobMQResponse>
    {
        [DataMember(Order = 1)]
        public int Id { get; set; }

        [DataMember(Order = 2)]
        public string JobKey { get; set; }
    }

    public class DeleteJobMQResponse : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public string Token { get; set; }

        [DataMember(Order = 2)]
        public ResponseStatus ResponseStatus { get; set; }

        [DataMember(Order = 3)]
        public string Result { get; set; }
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

    public class RescheduleMQRequest : EbServiceStackAuthRequest, IReturn<DeleteJobMQResponse>
    {
        [DataMember(Order = 1)]
        public string TriggerKey { get; set; }

        [DataMember(Order = 2)]
        public string JobKey { get; set; }

        [DataMember(Order = 3)]
        public EbTask Task { get; set; }

        [DataMember(Order = 4)]
        public int Id { get; set; }
    }

    public class RescheduleMQResponse : IEbSSResponse
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
    public class GetSchedulesOfSolutionRequest : EbServiceStackAuthRequest, IReturn<GetSchedulesOfSolutionResponse>
    {
        [DataMember(Order = 1)]
        public int ObjectId { get; set; }
    }
    public class GetSchedulesOfSolutionResponse : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public List<EbSchedule> Schedules { get; set; }

        [DataMember(Order = 2)]
        public ResponseStatus ResponseStatus { get; set; }

    }

    public class GetUserEmailsRequest : EbServiceStackAuthRequest, IReturn<GetUserEmailsResponse>
    {
        [DataMember(Order = 1)]
        public string UserIds { get; set; }

        [DataMember(Order = 2)]
        public string UserGroupIds { get; set; }
    }
    public class GetUserEmailsResponse : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public Dictionary<int, string> UserEmails { get; set; }

        [DataMember(Order = 2)]
        public Dictionary<int, string> UserGroupEmails { get; set; }

        [DataMember(Order = 3)]
        public ResponseStatus ResponseStatus { get; set; }
    }
}
