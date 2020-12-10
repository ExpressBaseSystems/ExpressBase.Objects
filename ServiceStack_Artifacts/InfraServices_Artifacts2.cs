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
    public class CreateAccountRequest : EbServiceStackNoAuthRequest
    {
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

        [DataMember(Order = 7)]
        public string Country { get; set; }

        [DataMember(Order = 8)]
        public string ActivationCode { get; set; }

        [DataMember(Order = 9)]
        public string PageUrl { get; set; }

        [DataMember(Order = 10)]
        public string PagePath { get; set; }

        [DataMember(Order = 11)]
        public string Account_type { get; set; }
    }

    [DataContract]
    public class CreateAccountResponse : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public int Id { get; set; }

        [DataMember(Order = 2)]
        public bool Notified { get; set; } = false;

        [DataMember(Order = 3)]
        public bool DbCreated { get; set; } = false;

        [DataMember(Order = 4)]
        public ResponseStatus ResponseStatus { get; set; }

        [DataMember(Order = 5)]
        public bool IsEmailUniq { get; set; }

        [DataMember(Order = 6)]
        public bool AccountCreated { get; set; } = false;
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

        [DataMember(Order = 6)]
        public string SolnId { get; set; }

        [DataMember(Order = 7)]
        public bool IsFurther { get; set; }

        [DataMember(Order = 8)]
        public int PackageId { get; set; }

        [DataMember(Order = 9)]
        public string PrimarySId { get; set; }
    }

    [DataContract]
    public class CreateSolutionResponse : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public int Id { get; set; }

        [DataMember(Order = 2)]
        public ResponseStatus ResponseStatus { get; set; }

        [DataMember(Order = 3)]
        public string ErrSolMessage { get; set; }

        [DataMember(Order = 4)]
        public string ErrDbMessage { get; set; }

        [DataMember(Order = 5)]
        public string SolURL { get; set; }
    }

    [DataContract]
    public class EditSolutionRequest : IReturn<EditSolutionResponse>, IEbSSRequest
    {
        [DataMember(Order = 1)]
        public string NewESolutionId { get; set; }

        [DataMember(Order = 2)]
        public string OldESolutionId { get; set; }

        [DataMember(Order = 3)]
        public int UserId { get; set; }

        [DataMember(Order = 4)]
        public string SolutionName { get; set; }

        [DataMember(Order = 5)]
        public string Description { get; set; }

        [DataMember(Order = 6)]
        public bool IsDelete { get; set; }

        public string SolnId { get; set; }
    }

    [DataContract]
    public class EditSolutionResponse : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public ResponseStatus ResponseStatus { get; set; }

        [DataMember(Order = 2)]
        public bool Status { get; set; }

        [DataMember(Order = 3)]
        public string Message { get; set; }
    }

    public class DeleteSolutionRequset : EbServiceStackAuthRequest, IReturn<DeleteSolutionResponse>
    {
        [DataMember(Order = 2)]
        public string ISolutionId { get; set; }

        [DataMember(Order = 2)]
        public string ESolutionId { get; set; }
    }

    public class DeleteSolutionResponse : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public bool Status { get; set; }

        [DataMember(Order = 1)]
        public ResponseStatus ResponseStatus { get; set; }
    }

    [DataContract]
    public class CreateSolutionFurtherRequest : EbServiceStackAuthRequest, IReturn<CreateSolutionFurtherResponse>
    {
        [DataMember(Order = 1)]
        public int PackageId { get; set; }

        [DataMember(Order = 2)]
        public string PrimarySId { get; set; }
    }

    [DataContract]
    public class CreateSolutionFurtherResponse : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public bool Status { get; set; }

        [DataMember(Order = 2)]
        public int SolId { get; set; }

        [DataMember(Order = 3)]
        public ResponseStatus ResponseStatus { get; set; }

        public CreateSolutionFurtherResponse()
        {
            ResponseStatus = new ResponseStatus();
        }
    }

    [DataContract]
    public class SocialAutoSignInRequest : EbServiceStackNoAuthRequest
    {
        [DataMember(Order = 1)]
        public string Email { get; set; }

        [DataMember(Order = 2)]
        public string Social_id { get; set; }
    }

    [DataContract]
    public class SocialAutoSignInResponse
    {
        [DataMember(Order = 1)]
        public int Id { get; set; }

        [DataMember(Order = 2)]
        public string psw { get; set; }

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
        public List<EbSolutionsWrapper> AllSolutions { get; set; }

        [DataMember(Order = 5)]
        public List<EbSolutionsWrapper> PrimarySolutions { get; set; }

        [DataMember(Order = 6)]
        public Dictionary<string, List<AppStore>> MasterPackages { get; set; }
    }

    public class GetPrimarySolutionsRequest : EbServiceStackAuthRequest, IReturn<GetPrimarySolutionsResponse>
    {

    }

    public class GetPrimarySolutionsResponse  
    {
        [DataMember(Order = 1)]
        public List<EbSolutionsWrapper> PrimarySolutions { get; set; }
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

        public bool IsVersioningEnabled { get; set; }

        public bool Is2faEnabled { get; set; }

        public string OtpDelivery { get; set; }

        public SolutionSettings SolutionSettings { get; set; }

        public SolutionType SolutionType { get; set; }

        public string PrimarySolution { get; set; }
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
        public bool VerifyStatus { get; set; } = false;

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

    [DataContract]
    public class SocialLoginRequest : EbServiceStackNoAuthRequest
    {
        [DataMember(Order = 1)]
        public string Email { get; set; }

        [DataMember(Order = 2)]
        public string Fbid { get; set; }

        [DataMember(Order = 3)]
        public string Name { get; set; }

        [DataMember(Order = 4)]
        public string Goglid { get; set; }
    }

    [DataContract]
    public class SocialLoginResponse
    {
        [DataMember(Order = 1)]
        public string jsonval { get; set; }
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

    public class GetVersioning : IEbSSRequest
    {
        public bool res { get; set; }

        public ResponseStatus status { get; set; }

        public string SolnId { get; set; }

        public int UserId { get; set; }
    }

    public class SolutionEditRequest
    {
        public bool Value { get; set; }

        public string DeliveryMethod { get; set; }

        public string solution_id { get; set; }

        public solutionChangeColumn ChangeColumn { get; set; }
    }
    public enum solutionChangeColumn
    {
        version = 1,
        TwoFa = 2
    }
    public class UpdateSidMapRequest : EbServiceStackNoAuthRequest, IReturn<UpdateSidMapResponse>
    {
        public string ExtSolutionId { get; set; }
    }

    public class UpdateSidMapResponse : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public ResponseStatus ResponseStatus { get; set; }
    }

    //public class UpdateSidMapMqRequest : EbServiceStackAuthRequest, IReturn<UpdateSidMapMqResponse> { }

    public class UpdateSidMapMqResponse : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public ResponseStatus ResponseStatus { get; set; }
    }
    public class UpdateRedisConnectionsRequest : EbServiceStackAuthRequest, IReturn<UpdateRedisConnectionsResponse> { }
    public class UpdateRedisConnectionsResponse : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public ResponseStatus ResponseStatus { get; set; }
    }
    public class UpdateRedisConnectionsMqRequest : EbServiceStackNoAuthRequest, IReturn<UpdateRedisConnectionsMqResponse> { }
    public class UpdateRedisConnectionsMqResponse : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public ResponseStatus ResponseStatus { get; set; }
    }
    public class CheckSolutionOwnerReq : EbServiceStackAuthRequest, IReturn<CheckSolutionOwnerResp>
    {
        [DataMember(Order = 1)]
        public string ESolutionId { get; set; }
    }

    public class CheckSolutionOwnerResp : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public ResponseStatus ResponseStatus { get; set; }

        [DataMember(Order = 2)]
        public bool IsValid { get; set; }

        [DataMember(Order = 3)]
        public EbSolutionsWrapper SolutionInfo { get; set; }

        public CheckSolutionOwnerResp()
        {
            SolutionInfo = new EbSolutionsWrapper();
        }
    }
}
