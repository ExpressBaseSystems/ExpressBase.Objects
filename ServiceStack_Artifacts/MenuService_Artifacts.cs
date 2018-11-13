using ExpressBase.Common.EbServiceStack.ReqNRes;
using ExpressBase.Common.Objects;
using ExpressBase.Common.Structures;
using ServiceStack;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace ExpressBase.Objects.ServiceStack_Artifacts
{
    public class SidebarUserRequest : EbServiceStackAuthRequest, IReturn<SidebarUserResponse>
    {
        public string Ids { get; set; }

        public List<string> SysRole { get; set; }
    }

    public class SidebarUserResponse :IEbSSResponse
    {
        [DataMember(Order = 1)]
        public Dictionary<int, AppWrap> Data { get; set; }

        [DataMember(Order = 2)]
        public Dictionary<int, AppObject> AppList { get; set; }

        [DataMember(Order = 2)]
        public ResponseStatus ResponseStatus { get; set; }
    }

    [DataContract]
    public class AppObject
    {
        [DataMember(Order = 1)]
        public string AppName { get; set; }

        [DataMember(Order = 1)]
        public string AppIcon { get; set; }
    }

    [DataContract]
    public class AppWrap
    {
        [DataMember(Order = 2)]
        public Dictionary<int, TypeWrap> Types { get; set; }
    }

    [DataContract]
    public class TypeWrap
    {
        [DataMember(Order = 1)]
        public List<ObjWrap> Objects { get; set; }
    }

    public class EbObjectTypeWrap
    {
        [DataMember(Order = 1)]
        public string Name { get; set; }

        [DataMember(Order = 1)]
        public int IntCode { get; set; }

        [DataMember(Order = 1)]
        public string BMW { get; set; }

        [DataMember(Order = 1)]
        public bool IsUserFacing { get; set; }

        [DataMember(Order = 1)]
        public string Icon { get; set; }

    }

    [DataContract]
    public class ObjWrap
    {
        [DataMember(Order = 1)]
        public int Id { get; set; }

        [DataMember(Order = 2)]
        public string ObjName { get; set; }

        [DataMember(Order = 3)]
        public string VersionNumber { get; set; }

        [DataMember(Order = 4)]
        public string Refid { get; set; }

        [DataMember(Order = 5)]
        public int EbObjectType { get; set; }

        [DataMember(Order = 6)]
        public int AppId { get; set; }

        [DataMember(Order = 7)]
        public string Description { get; set; }

        [DataMember(Order = 5)]
        public string EbType { get; set; }

        [DataMember(Order = 6)]
        public string DisplayName { get; set; }
    }

    public class SidebarDevRequest : EbServiceStackAuthRequest, IReturn<SidebarDevResponse>
    {
    }

    public class SidebarDevResponse : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public Dictionary<int, AppWrap> Data { get; set; }

        [DataMember(Order = 2)]
        public Dictionary<int, AppObject> AppList { get; set; }

        [DataMember(Order = 3)]
        public ResponseStatus ResponseStatus { get; set; }
    }
}
