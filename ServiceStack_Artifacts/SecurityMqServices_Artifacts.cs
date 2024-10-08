﻿using ExpressBase.Common.EbServiceStack.ReqNRes;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace ExpressBase.Objects.ServiceStack_Artifacts
{
    public class SaveRoleMqRequest : EbMqRequest
    {
        [DataMember(Order = 1)]
        public string SubscriptionId { get; set; }

        [DataMember(Order = 2)]
        public List<int> UserIdsToUpdate { get; set; }
    }

    public class SuspendUserMqRequest : EbMqRequest
    {
        [DataMember(Order = 1)]
        public string SubscriptionId { get; set; }
    }

    public class SaveUserMqRequest : EbMqRequest
    {
        [DataMember(Order = 1)]
        public string SubscriptionId { get; set; }

        [DataMember(Order = 2)]
        public string LocationAdd { get; set; }

        [DataMember(Order = 3)]
        public string LocationDelete { get; set; }

        [DataMember(Order = 4)]
        public List<string> NewRole_Ids { get; set; } = new List<string>();

        [DataMember(Order = 5)]
        public List<string> OldRole_Ids { get; set; } = new List<string>();

        [DataMember(Order = 6)]
        public List<string> OldUserGroups { get; set; } = new List<string>();

        [DataMember(Order = 7)]
        public List<string> NewUserGroups { get; set; } = new List<string>();
    }

    public class SaveUserGroupMqRequest : EbMqRequest
    {
        [DataMember(Order = 1)]
        public string SubscriptionId { get; set; }

        [DataMember(Order = 2)]
        public List<string> OldUserGroups { get; set; } = new List<string>();

        [DataMember(Order = 3)]
        public List<string> NewUserGroups { get; set; } = new List<string>();
    }
}
