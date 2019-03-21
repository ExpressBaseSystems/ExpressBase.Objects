using ExpressBase.Common.EbServiceStack.ReqNRes;
using ServiceStack;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using ExpressBase.Common;

namespace ExpressBase.Objects.ServiceStack_Artifacts
{
    public class LogRedisInsertRequest : EbServiceStackAuthRequest, IReturn<LogRedisInsertResponse>
    {
        public RedisOperations Operation { get; set; }

        public string PreviousValue { get; set; }

        public string NewValue { get; set; }

        public string Key { get; set; }

        public int SolutionId { get; set; }

    }

    public class LogRedisInsertResponse : IEbSSResponse
    {

        [DataMember(Order = 1)]
        public ResponseStatus ResponseStatus { get; set; }
    }


    public class LogRedisGetRequest : EbServiceStackAuthRequest, IReturn<LogRedisGetResponse>
    {
        public int SolutionId { get; set; }

    }
    public class LogRedisGetResponse : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public List<EbRedisLogs> Logs { get; set; }

        [DataMember(Order = 2)]
        public ResponseStatus ResponseStatus { get; set; }
    }

    public class LogRedisViewChangesRequest : EbServiceStackAuthRequest, IReturn<LogRedisViewChangesResponse>
    {
        public int LogId { get; set; }

    }
    public class LogRedisViewChangesResponse : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public EbRedisLogValues RedisLogValues { get; set; }
        [DataMember(Order = 2)]
        public ResponseStatus ResponseStatus { get; set; }
    }


    public class RedisGetGroupDetails : EbServiceStackAuthRequest, IReturn<RedisGroupDetailsResponse>
    { }

    public class RedisGroupDetailsResponse : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public Dictionary<string, List<EbRedisGroupDetails>> GroupsDict { get; set; }
        //public List<EbRedisGroupDetails> GrpLst { get; set; }
        [DataMember(Order = 2)]
        public ResponseStatus ResponseStatus { get; set; }
    }
}
