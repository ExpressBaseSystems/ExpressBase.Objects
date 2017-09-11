using ExpressBase.Data;
using ExpressBase.Security;
using ServiceStack;
using System.Runtime.Serialization;

namespace ExpressBase.Objects.ServiceStack_Artifacts
{
    public interface IEbSSRequest
    {
        string TenantAccountId { get; set; }

        int UserId { get; set; }
    }

    [DataContract]
    public abstract class EbServiceStackRequest
    {
        [DataMember(Order = 1)]
        public string TenantAccountId { get; set; }

        [DataMember(Order = 2)]
        public int UserId { get; set; }
    }

    public interface IEbSSResponse
    {
        ResponseStatus ResponseStatus { get; set; } //Exception gets serialized here
    }

    [DataContract]
    public abstract class EbServiceStackResponse
    {
        [DataMember(Order = 2)]
        public ResponseStatus ResponseStatus { get; set; } //Exception gets serialized here
    }

    [DataContract]
    public class MyAuthenticateResponse : AuthenticateResponse
    {
        [DataMember(Order = 1)]
        public User User { get; set; }
    }
}
