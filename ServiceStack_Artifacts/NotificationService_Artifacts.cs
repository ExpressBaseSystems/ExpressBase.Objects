using ExpressBase.Common;
using ExpressBase.Common.EbServiceStack.ReqNRes;
using ExpressBase.Common.ServerEvents_Artifacts;
using ServiceStack;
using System;
using System.Collections.Generic;
using System.Data;
using System.Runtime.Serialization;
using System.Text;

namespace ExpressBase.Objects.ServiceStack_Artifacts
{
    public class NotifyLogOutRequest : EbServiceStackAuthRequest, IReturn<NotifyLogOutResponse>
    {
    }

    public class NotifyLogOutResponse : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public ResponseStatus ResponseStatus { get; set; }

    }

    public class NotifyByUserIDRequest : EbServiceStackAuthRequest, IReturn<NotifyByUserIDResponse>
    {
        public int UsersID { get; set; }

        public string Link { get; set; }

        public string Title { get; set; }
    }

    public class NotifyByUserIDResponse : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public ResponseStatus ResponseStatus { get; set; }

    }

    public class NotifyByUserRoleRequest : EbServiceStackAuthRequest, IReturn<NotifyByUserRoleResponse>
    {
        public List<int> RoleID { get; set; }

        public string Link { get; set; }

        public string Title { get; set; }
    }

    public class NotifyByUserRoleResponse : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public ResponseStatus ResponseStatus { get; set; }

    }

    public class NotifyByUserGroupRequest : EbServiceStackAuthRequest, IReturn<NotifyByUserGroupResponse>
    {
        public List<int> GroupId { get; set; }

        public string Link { get; set; }

        public string Title { get; set; }
    }

    public class NotifyByUserGroupResponse : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public ResponseStatus ResponseStatus { get; set; }

    }

    public class GetNotificationFromDbRequest : EbServiceStackAuthRequest, IReturn<GetNotificationFromDbResponse>
    {
        public string NotificationId { get; set; }
    }

    public class GetNotificationFromDbResponse : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public ResponseStatus ResponseStatus { get; set; }

        [DataMember(Order = 2)]
        public List<NotificationInfo> Notifications { get; set; }
    }

    public class Notifications 
    {
        public List<NotificationInfo> Notification { get; set; }
    }
    
    public class GetNotificationsRequest : EbServiceStackAuthRequest, IReturn<GetNotificationsResponse>
    {
    }

    public class GetNotificationsResponse : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public ResponseStatus ResponseStatus { get; set; }

        [DataMember(Order = 2)]
        public List<NotificationInfo> Notifications { get; set; }
    }
    
}
