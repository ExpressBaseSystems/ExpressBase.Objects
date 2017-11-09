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
        public string Token { get; set; }

        [DataMember(Order = 3)]
        public ResponseStatus ResponseStatus { get; set; }

        [DataMember(Order = 4)]
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
        public Dictionary<string, object> Colvalues { get; set; }

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
