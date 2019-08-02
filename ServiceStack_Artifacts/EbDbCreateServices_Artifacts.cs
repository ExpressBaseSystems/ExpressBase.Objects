using ExpressBase.Common;
using ExpressBase.Common.Connections;
using ExpressBase.Common.EbServiceStack.ReqNRes;
using ServiceStack;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace ExpressBase.Objects.ServiceStack_Artifacts
{
    [DataContract]
    public class EbDbCreateRequest : IEbTenentRequest, IReturn<EbDbCreateResponse>
    {
        [DataMember(Order = 1)]
        public string DBName { get; set; }

        [DataMember(Order = 2)]
        public bool IsChange { get; set; }

        [DataMember(Order = 3)]
        public EbDbConfig DataDBConfig { get; set; }

        [DataMember(Order = 4)]
        public string SolnId { get; set; }

        [DataMember(Order = 5)]
        public int UserId { get; set; }

        [DataMember(Order = 5)]
        public bool IsFurther { get; set; }
    }

    [DataContract]
    public class EbDbCreateResponse : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public bool DeploymentCompled { get; set; }

        [DataMember(Order = 2)]
        public string DbName { get; set; }

        [DataMember(Order = 3)]
        public EbDbUsers DbUsers { get; set; }

        [DataMember(Order = 4)]
        public ResponseStatus ResponseStatus { get; set; }
    }

    public class EbDbUsers
    {
        public string AdminUserName { get; set; }

        public string AdminPassword { get; set; }

        public string ReadWriteUserName { get; set; }

        public string ReadWritePassword { get; set; }

        public string ReadOnlyUserName { get; set; }

        public string ReadOnlyPassword { get; set; }
    }
}
