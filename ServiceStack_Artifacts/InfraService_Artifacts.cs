using ServiceStack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace ExpressBase.Objects.ServiceStack_Artifacts
{
    [DataContract]
    public class InfraRequest : IReturn<InfraResponse>
    {
        [DataMember(Order = 0)]
        public Dictionary<string, object> Colvalues { get; set; }

        [DataMember(Order = 1)]
        public string ltype { get; set; }
    }

    [DataContract]
    public class InfraResponse
    {
        [DataMember(Order = 1)]
        public int id { get; set; }
    }

    [DataContract]
    [Route("/unc", "POST")]
    public class UnRequest : IReturn<bool>
    {
        [DataMember(Order = 0)]
        public Dictionary<string, object> Colvalues { get; set; }

    }

    [DataContract]
    public class DbCheckRequest : IReturn<bool>
    {
        [DataMember(Order = 0)]
        public Dictionary<string, object> DBColvalues { get; set; }

        [DataMember(Order = 1)]
        public int CId { get; set; }
    }

    [DataContract]
    public class AccountRequest : IReturn<AccountResponse>
    {
        [DataMember(Order = 0)]
        public Dictionary<string, object> Colvalues { get; set; }

        [DataMember(Order = 1)]
        public string op { get; set; }

        [DataMember(Order = 2)]
        public int TId { get; set; }
    }

    [DataContract]
    public class AccountResponse
    {
        [DataMember(Order = 1)]
        public int id { get; set; }
    }

    [DataContract]
    public class GetAccount : IReturn<GetAccountResponse>, IEbSSRequest
    {
        [DataMember(Order = 0)]
        public int Uid { get; set; }

        [DataMember(Order = 1)]
        public string restype { get; set; }

        public string Token { get; set; }

        public string TenantAccountId { get; set; }
    }

    [DataContract]
    public class GetAccountResponse
    {
        [DataMember(Order = 1)]
        public Dictionary<int, string> dict { get; set; }
    }

    [DataContract]
    public class SendMail : IReturn<bool>
    {
        [DataMember(Order = 0)]
        public Dictionary<string, object> Emailvals { get; set; }
    }
}
