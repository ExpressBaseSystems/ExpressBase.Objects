using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using ServiceStack;
using ExpressBase.Common.EbServiceStack.ReqNRes;
using ExpressBase.Common.Data;
using ExpressBase.Common.LocationNSolution;
using ExpressBase.Common;

namespace ExpressBase.Objects.ServiceStack_Artifacts
{
    [DataContract]
    public class CreateAccountRequest : IReturn<CreateAccountResponse>, IEbSSRequest
    {
        [DataMember(Order = 0)]
        public string Op { get; set; }

        [DataMember(Order = 1)]
        public string Email { get; set; }

        [DataMember(Order = 2)]
        public string Name { get; set; }

        [DataMember(Order = 3)]
        public string Company { get; set; }

        [DataMember(Order = 4)]
        public string Password { get; set; }

        [DataMember(Order = 5)]
        public string SolnId { get; set; }

        [DataMember(Order = 6)]
        public string DbName { get; set; }

        [DataMember(Order = 7)]
        public string Country { get; set; }

        [DataMember(Order = 8)]
        public string ActivationCode { get; set; }

        [DataMember(Order = 9)]
        public string PageUrl { get; set; }

        [DataMember(Order = 10)]
        public string PagePath { get; set; }

        [DataMember(Order =11)]
        public string Account_type { get; set; }

        [DataMember(Order =12)]
        public string DisplayName { get; set; }


        public int UserId { get; set; }

        public string Token { get; set; }
    }

    [DataContract]
    public class CreateAccountResponse : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public int id { get; set; }

        [DataMember(Order = 2)]
        public string email { get; set; }

        [DataMember(Order = 3)]
        public string Token { get; set; }

        [DataMember(Order = 4)]
        public ResponseStatus ResponseStatus { get; set; }

        [DataMember(Order = 5)]
        public string u_token { get; set; }

        [DataMember(Order = 6)]
        public string Sol_id_autogen { get; set; }

    }

    [DataContract]
    public class DataBaseConfigRequest : IReturn<DataBaseConfigResponse>, IEbSSRequest
    {
        [DataMember(Order = 0)]
        public Dictionary<string, object> Colvalues { get; set; }

        [DataMember(Order = 2)]
        public int Id { get; set; }

        [DataMember(Order = 3)]
        public string SolnId { get; set; }

        public int UserId { get; set; }

        public string Token { get; set; }
    }

    [DataContract]
    public class DataBaseConfigResponse : IEbSSResponse
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
    public class EditAccountRequest : IReturn<EditAccountResponse>, IEbSSRequest
    {
        [DataMember(Order = 0)]
        public Dictionary<string, object> Colvalues { get; set; }

        [DataMember(Order = 1)]
        public string op { get; set; }

        [DataMember(Order = 2)]
        public int Id { get; set; }


        [DataMember(Order = 3)]
        public string SolnId { get; set; }

        public int UserId { get; set; }

        public string Token { get; set; }
    }

    [DataContract]
    public class EditAccountResponse : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public Dictionary<string, object> Data { get; set; }

        [DataMember(Order = 2)]
        public string Token { get; set; }

        [DataMember(Order = 3)]
        public ResponseStatus ResponseStatus { get; set; }

        [DataMember(Order = 4)]
        public string u_token { get; set; }
    }

    [DataContract]
    public class EditDBConfigRequest : IReturn<EditDBConfigResponse>, IEbSSRequest
    {
        [DataMember(Order = 0)]
        public Dictionary<string, object> Colvalues { get; set; }

        [DataMember(Order = 1)]
        public string op { get; set; }

        [DataMember(Order = 2)]
        public int Id { get; set; }


        [DataMember(Order = 3)]
        public string SolnId { get; set; }

        public int UserId { get; set; }

        public string Token { get; set; }
    }

    [DataContract]
    public class EditDBConfigResponse : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public Dictionary<string, object> Data { get; set; }

        [DataMember(Order = 2)]
        public string Token { get; set; }

        [DataMember(Order = 3)]
        public ResponseStatus ResponseStatus { get; set; }

        [DataMember(Order = 4)]
        public string u_token { get; set; }
    }

    [DataContract]
    public class RegisterRequest : Register
    {

        [DataMember(Order = 1)]
        public string TenantAccountId { get; set; }
    }

    [DataContract]
    [Route("/unique", "POST")]
    public class UniqueRequest
    {
        [DataMember(Order = 1)]
        public string email { get; set; }

    }

    public class UniqueRequestResponse
    {
        public int Id { get; set; }

        public bool Unique { set; get; }

        public bool HasPassword { get; set; }
    }

    [DataContract]
    public class GetAccountRequest : IReturn<GetAccountResponse>, IEbSSRequest
    {

        [DataMember(Order = 1)]
        public int Id { get; set; }

        [DataMember(Order = 2)]
        public string SolnId { get; set; }

        public int UserId { get; set; }

        public string Token { get; set; }
    }

    [DataContract]
    public class GetAccountResponse : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public int id { get; set; }

        [DataMember(Order = 2)]
        public List<List<object>> returnlist { get; set; }

        [DataMember(Order = 3)]
        public Dictionary<string, object> Data { get; set; }

        [DataMember(Order = 4)]
        public string Token { get; set; }

        [DataMember(Order = 5)]
        public ResponseStatus ResponseStatus { get; set; }
    }

    public class ProductPlan
    {
        public string Plan { get; set; }

        public decimal Amount { get; set; }

        public decimal EvalDays { get; set; }
    }

    [DataContract]
    public class CreateSolutionRequest : IReturn<CreateSolutionResponse>, IEbSSRequest
    {
        [DataMember(Order = 1)]
        public string SolnUrl { get; set; }

        [DataMember(Order = 2)]
        public int UserId { get; set; }

        [DataMember(Order = 3)]
        public string SolutionName { get; set; }

        [DataMember(Order = 4)]
        public string Description { get; set; }

        [DataMember(Order = 5)]
        public bool DeployDB { get; set; }

        public string SolnId { get; set; }
    }

    [DataContract]
    public class CreateSolutionResponse : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public int Status { get; set; }

        [DataMember(Order = 2)]
        public ResponseStatus ResponseStatus { get; set; }

        [DataMember(Order = 3)]
        public string ErrSolMessage { get; set; }

        [DataMember(Order = 4)]
        public string ErrDbMessage { get; set; }
    }

    [DataContract]
    public class GetSolutionRequest : IReturn<GetSolutionResponse>, IEbSSRequest
    {
        public string SolnId { get; set; }

        public int UserId { get; set; }

        public string Token { get; set; }
    }

    [DataContract]
    public class GetSolutionResponse : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public int id { get; set; }

        [DataMember(Order = 2)]
        public string Token { get; set; }

        [DataMember(Order = 3)]
        public ResponseStatus ResponseStatus { get; set; }

        [DataMember(Order = 4)]
        public List<EbSolutionsWrapper> Data { get; set; }
    }

    [DataContract]
    public class GetSolutioInfoRequest : IReturn<GetSolutioInfoResponse>, IEbSSRequest
    {
        [DataMember(Order = 0)]
        public string IsolutionId { get; set; }

        public string SolnId { get; set; }

        public int UserId { get; set; }

        public string Token { get; set; }
    }

    [DataContract]
    public class GetSolutioInfoResponse : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public int id { get; set; }

        [DataMember(Order = 2)]
        public string Token { get; set; }

        [DataMember(Order = 3)]
        public ResponseStatus ResponseStatus { get; set; }

        [DataMember(Order = 4)]
        public EbSolutionsWrapper Data { get; set; }

        [DataMember(Order = 5)]
        public EbConnectionsConfig EBSolutionConnections { get; set; }
    }

    public class EbSolutionsWrapper
    {
        public string SolutionName { get; set; }

        public string Description { get; set; }

        public string DateCreated { get; set; }

        public string IsolutionId { get; set; }

        public string EsolutionId { get; set; }

        public PricingTiers PricingTier { get; set; }
    }

    [DataContract]
    public class EmailverifyRequest : EbServiceStackNoAuthRequest
    {
        [DataMember(Order = 1)]
        public string Id { get; set; }

        [DataMember(Order = 2)]
        public string ActvCode { get; set; }
    }

    [DataContract]
    public class EmailverifyResponse
    {
        [DataMember(Order = 1)]
        public bool VerifyStatus { get; set; }
    }

    [DataContract]
    public class ForgotPasswordRequest : EbServiceStackNoAuthRequest
    {
        [DataMember(Order = 1)]
        public string Resetcode { get; set; }

        [DataMember(Order = 2)]
        public string Email { get; set; }

        [DataMember(Order = 4)]
        public string PageUrl { get; set; }

        [DataMember(Order = 5)]
        public string PagePath { get; set; }
    }

    [DataContract]
    public class ForgotPasswordResponse
    {
        [DataMember(Order = 1)]
        public bool VerifyStatus { get; set; }

    }

    [DataContract]
    public class ResetPasswordRequest : EbServiceStackNoAuthRequest
    {
        [DataMember(Order = 1)]
        public string Email { get; set; }

        [DataMember(Order = 2)]
        public string Resetcode { get; set; }

        [DataMember(Order = 3)]
        public string Password { get; set; }
    }

    [DataContract]
    public class ResetPasswordResponse
    {
        [DataMember(Order = 1)]
        public bool VerifyStatus { get; set; }
    }

    //[DataContract]
    //public class GetUsersResponse : IEbSSResponse
    //{

    //    [DataMember(Order = 1)]
    //    public Dictionary<string, object> Data { get; set; }

    //    [DataMember(Order = 2)]
    //    public string Token { get; set; }

    //    [DataMember(Order = 3)]
    //    public ResponseStatus ResponseStatus { get; set; }
    //}
}
