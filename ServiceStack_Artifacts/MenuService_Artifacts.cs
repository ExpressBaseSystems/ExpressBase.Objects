using ExpressBase.Common.Objects;
using ServiceStack;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace ExpressBase.Objects.ServiceStack_Artifacts
{
    public class SidebarUserRequest : EbServiceStackRequest, IReturn<SidebarUserResponse>
    {
        public string Ids { get; set; }
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
        [DataMember(Order = 2)]
        public List<ObjWrap> Objects { get; set; }
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
        public EbObjectType EbObjectType { get; set; }

        [DataMember(Order = 6)]
        public int AppId { get; set; }

        [DataMember(Order = 7)]
        public string Description { get; set; }

        [DataMember(Order = 5)]
        public string EbType { get; set; }

    }

    public class SidebarDevRequest : EbServiceStackRequest, IReturn<SidebarDevResponse>
    {
    }

    public class SidebarDevResponse : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public Dictionary<int, TypeWrap> Data { get; set; }

        [DataMember(Order = 2)]
        public ResponseStatus ResponseStatus { get; set; }
    }
}
