using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using ServiceStack;

namespace ExpressBase.Objects.ServiceStack_Artifacts
{
    [DataContract]
    public class CreateAccountRequest : IReturn<CreateAccountResponse>, IEbSSRequest
    {
        [DataMember(Order = 0)]
        public Dictionary<string, object> Colvalues { get; set; }

        [DataMember(Order = 1)]
        public string op { get; set; }

        [DataMember(Order = 2)]
        public int Id { get; set; }


        [DataMember(Order = 3)]
        public string TenantAccountId { get; set; }

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
    }

    [DataContract]
    public class DataBaseConfigRequest : IReturn<DataBaseConfigResponse>, IEbSSRequest
    {
        [DataMember(Order = 0)]
        public Dictionary<string, object> Colvalues { get; set; }

        [DataMember(Order = 2)]
        public int Id { get; set; }

        [DataMember(Order = 3)]
        public string TenantAccountId { get; set; }

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
        public string TenantAccountId { get; set; }

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
        public string TenantAccountId { get; set; }

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

    [DataContract]
    public class GetAccountRequest : IReturn<GetAccountResponse>, IEbSSRequest
    {
      
        [DataMember(Order = 1)]
        public int Id { get; set; }

        [DataMember(Order = 2)]
        public string TenantAccountId { get; set; }

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

    [DataContract]
    public class GetProductPlanRequest : IReturn<GetProductPlanResponse>, IEbSSRequest
    {       

        [DataMember(Order = 1)]
        public string TenantAccountId { get; set; }

        public int UserId { get; set; }
    }

    [DataContract]
    public class GetProductPlanResponse : IEbSSResponse
    {        

        [DataMember(Order = 2)]
        public Dictionary<int,List<ProductPlan>> Plans { get; set; }

        [DataMember(Order = 3)]
        public ResponseStatus ResponseStatus { get; set; }

    }

    public class AutoGenSolIdRequest
    {
        [DataMember(Order = 1)]
        public string TenantAccountId { get; set; }

        public int UserId { get; set; }
    }

    [DataContract]
    public class AutoGenSolIdResponse: IEbSSResponse
    {
        [DataMember(Order = 1)]
        public string Sid { get; set; }

        [DataMember(Order = 2)]
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
        public string TenantAccountId { get; set; }

        [DataMember(Order = 2)]
        public int UserId { get; set; }

        [DataMember(Order = 3)]
        public string Subscription { get; set; }

        [DataMember(Order = 4)]
        public string SolutionName { get; set; }

        [DataMember(Order = 6)]
        public string EsolutionId { get; set; }

        [DataMember(Order = 7)]
        public string IsolutionId { get; set; }

        [DataMember(Order = 8)]
        public string Description { get; set; }
    }

    [DataContract]
    public class CreateSolutionResponse : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public int Solnid { get; set; }

        [DataMember(Order = 2)]
        public ResponseStatus ResponseStatus { get; set; }

    }

    [DataContract]
    public class CreateApplicationRequest : IReturn<CreateApplicationResponse>, IEbSSRequest
    {
        [DataMember(Order = 0)]
        public Dictionary<string, object> Colvalues { get; set; }

        [DataMember(Order = 1)]
        public int Id { get; set; }

        public string TenantAccountId { get; set; }

        public int UserId { get; set; }

        public string Token { get; set; }
    }

    [DataContract]
    public class CreateApplicationResponse : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public int id { get; set; }

        [DataMember(Order = 2)]
        public string Token { get; set; }

        [DataMember(Order = 3)]
        public ResponseStatus ResponseStatus { get; set; }

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
