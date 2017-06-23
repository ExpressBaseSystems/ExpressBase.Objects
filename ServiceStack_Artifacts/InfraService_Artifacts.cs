using ServiceStack;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace ExpressBase.Objects.ServiceStack_Artifacts
{
    [DataContract]
    public class InfraDb_GENERIC_SELECTRequest : IEbSSRequest
    {
        [DataMember(Order = 1)]
        public string Token { get; set; }

        [DataMember(Order = 2)]
        public string TenantAccountId { get; set; }

        [DataMember(Order = 3)]
        public string InfraDbSqlQueryKey { get; set; }

        [DataMember(Order = 4)]
        public Dictionary<string, object> Parameters { get; set; }

        public int UserId { get; set; }
    }

    [DataContract]
    public class InfraDb_GENERIC_SELECTResponse
    {
        [DataMember(Order = 1)]
        public List<Dictionary<string, object>> Data { get; set; }
    }

    [DataContract]
    public class InfraRequest : IReturn<InfraResponse>,IEbSSRequest
    {
        [DataMember(Order = 0)]
        public Dictionary<string, object> Colvalues { get; set; }

        [DataMember(Order = 1)]
        public string ltype { get; set; }

        [DataMember(Order = 2)]
        public string Token { get; set; }

        public string TenantAccountId { get; set; }

        public int UserId { get; set; }
    }

    [DataContract]
    public class InfraResponse : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public int id { get; set; }

        [DataMember(Order = 2)]
        public string Token { get; set; }

        [DataMember(Order = 3)]
        public ResponseStatus ResponseStatus { get; set; }

        [DataMember(Order = 4)]
        public string u_token { get; set; }
    }

    [DataContract]
    [Route("/unc", "POST")]
    public class UnRequest : IReturn<bool>,IEbSSRequest
    {
        [DataMember(Order = 0)]
        public Dictionary<string, object> Colvalues { get; set; }

        [DataMember(Order = 1)]
        public string Token { get; set; }

        public string TenantAccountId { get; set; }

        public int UserId { get; set; }


    }

    [DataContract]
    public class DbCheckRequest : IReturn<bool>,IEbSSRequest
    {
        [DataMember(Order = 0)]
        public Dictionary<string, object> DBColvalues { get; set; }

        [DataMember(Order = 1)]
        public int CId { get; set; }

        [DataMember(Order = 2)]
        public string Token { get; set; }

        public string TenantAccountId { get; set; }

        public int UserId { get; set; }
    }

    [DataContract]
    [Route("/infra")]
    public class TokenRequiredUploadRequest : IReturn<TokenRequiredUploadResponse>,IEbSSRequest
    {
        [DataMember(Order = 0)]
        public Dictionary<string, object> Colvalues { get; set; }

        [DataMember(Order = 1)]
        public string op { get; set; }

        [DataMember(Order = 2)]
        public int Id { get; set; }

        [DataMember(Order = 3)]
        public string Token { get; set; }

        [DataMember(Order = 4)]
        public int TId { get; set; }

        public string TenantAccountId { get; set; }

        public int UserId { get; set; }
    }

    [DataContract]
    public class TokenRequiredUploadResponse : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public int id { get; set; }

        [DataMember(Order = 2)]
        public Dictionary<string, object> Data { get; set; }

        [DataMember(Order = 3)]
        public string Token { get; set; }

        [DataMember(Order = 4)]
        public ResponseStatus ResponseStatus { get; set; }

    }

    [DataContract] 
    public class TokenRequiredSelectRequest : IReturn<TokenRequiredSelectResponse>, IEbSSRequest
    {
        [DataMember(Order = 0)]
        public int Uid { get; set; }

        [DataMember(Order = 1)]
        public string restype { get; set; }

        [DataMember(Order = 2)]
        public string Token { get; set; }

        [DataMember(Order = 3)]
        public string Uname { get; set; }

        [DataMember(Order = 4)]
        public int id { get; set; }

        public string TenantAccountId { get; set; }

        public int UserId { get; set; }
    }

    [DataContract]
    public class TokenRequiredSelectResponse: IEbSSResponse
    {
       
        [DataMember(Order = 1)]
        public List<List<object>> returnlist { get; set; }

        [DataMember(Order = 2)]
        public Dictionary<string, object> Data { get; set; }

        [DataMember(Order = 3)]
        public string Token { get; set; }

        [DataMember(Order = 4)]
        public ResponseStatus ResponseStatus { get; set; }
    }

    [DataContract]
    public class SendMail : IReturn<bool>
    {
        [DataMember(Order = 0)]
        public Dictionary<string, object> Emailvals { get; set; }
    }
}
