using ExpressBase.Common.EbServiceStack.ReqNRes;
using ServiceStack;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace ExpressBase.Objects.ServiceStack_Artifacts
{
    public class NotifyTestRequest : EbServiceStackAuthRequest, IReturn<NotifyTestResponse>
    {
        
    }

    public class NotifyTestResponse : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public ResponseStatus ResponseStatus { get; set; }

    }

    public class Notifications : List<NotificationContents>
    {

    }

    public class NotificationContents
    {
        public string Title { get; set; }

        public string Link { get; set; }
    }
}
