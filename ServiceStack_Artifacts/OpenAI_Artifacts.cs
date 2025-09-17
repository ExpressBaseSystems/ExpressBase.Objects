using ExpressBase.Common;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace ExpressBase.Objects.ServiceStack_Artifacts
{
    internal class OpenAI_Artifacts
    {

    }

    [DataContract]
    public class OpenAIQueryRequest
    {
        [DataMember(Order = 1)]
        public string Query { get; set; }
        [DataMember(Order = 2)]
        public string SessionId { get; set; }
        [DataMember(Order = 3)]
        public bool IsAdminOwn { get; set; }
        [DataMember(Order = 4)]
        public string SolutionId { get; set; }
    }

    [DataContract]
    public class OpenAISessionStoreResponse
    {
        [DataMember(Order = 1)]
        public int Id { get; set; }
    }

    [DataContract]
    public class OpenAISessionStoreRequest
    {
        [DataMember(Order = 1)]
        public string SessionId { get; set; }

        [DataMember(Order = 2)]
        public string ChatHeading { get; set; }

        [DataMember(Order = 3)]
        public string Query { get; set; }

        [DataMember(Order = 4)]
        public string UserId { get; set; }
    }

    [DataContract]
    public class SessionObject
    {
        [DataMember(Order = 1)]
        public string SessionId { get; set; }

        [DataMember(Order = 2)]
        public string ChatHeading { get; set; }
    }

    public class ChatHistoryResponse
    {
        [DataMember(Order = 1)]
        public Dictionary<int, SessionObject> Sessions { get; set; }
    }
}
