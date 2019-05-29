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
    public class EbDbCreateRequest : EbServiceStackAuthRequest, IReturn<EbDbCreateResponse>
    {
        [DataMember(Order = 1)]
        public string dbName { get; set; }
        [DataMember(Order = 2)]
        public bool ischange { get; set; }
        [DataMember(Order = 3)]
        public EbDbConfig DataDBConfig { get; set; }
        //[DataMember(Order = 4)]
        //public IDatabase Idbcon { get; set; }

    }
    [DataContract]
    public class EbDbCreateResponse : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public bool resp { get; set; }
        [DataMember(Order = 2)]
        public string dbname { get; set; }
        [DataMember(Order = 3)]
        public EbDbUsers dbusers { get; set; }
        //[DataMember(Order = 3)]
        //public string AdminUserName { get; set; }
        //[DataMember(Order = 4)]
        //public string AdminPassword { get; set; }
        //[DataMember(Order = 5)]
        //public string ReadWriteUserName { get; set; }
        //[DataMember(Order = 6)]
        //public string ReadWritePassword { get; set; }
        //[DataMember(Order = 7)]
        //public string ReadOnlyUserName { get; set; }
        //[DataMember(Order = 8)]
        //public string ReadOnlyPassword { get; set; }
        //[DataMember(Order = 9)]
        //public string Token { get; set; }
        //[DataMember(Order = 10)]
        [DataMember(Order = 4)]
        public ResponseStatus ResponseStatus { get; set; }
    }

    [DataContract]
    public class EbCreateOracleDBRequest : EbServiceStackAuthRequest, IReturn<bool>
    {
       
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
