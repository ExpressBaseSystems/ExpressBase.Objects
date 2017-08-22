using ExpressBase.Data;
using ExpressBase.Security;
using ServiceStack;
using System.Runtime.Serialization;

namespace ExpressBase.Objects.ServiceStack_Artifacts
{
    public interface IEbSSRequest
    {
        string Token { get; set; }

        string TenantAccountId { get; set; }

        int UserId { get; set; }
    }

    public interface IEbSSResponse
    {
        ResponseStatus ResponseStatus { get; set; } //Exception gets serialized here
    }

    [DataContract]
    public class MyAuthenticateResponse : AuthenticateResponse
    {
        [DataMember(Order = 1)]
        public User User { get; set; }
    }
}
