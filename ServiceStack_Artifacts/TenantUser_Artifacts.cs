using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using ServiceStack;
using System.Text;

namespace ExpressBase.Objects.ServiceStack_Artifacts
{
    [DataContract] 
    public class CreateUserRequest : IReturn<CreateUserResponse>, IEbSSRequest
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
    public class CreateUserResponse : IEbSSResponse
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
    public class GetUserEditRequest : IReturn<GetUserEditResponse>, IEbSSRequest
    {
      
        [DataMember(Order = 1)]
        public int Id { get; set; }

        [DataMember(Order = 2)]
        public string TenantAccountId { get; set; }

        public int UserId { get; set; }

        public string Token { get; set; }
    }

    [DataContract]
    public class GetUserEditResponse : IEbSSResponse
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
    public class GetApplicationObjectsRequest : IReturn<GetApplicationObjectsResponse>, IEbSSRequest
    {
       
        [DataMember(Order = 1)]
        public int Id { get; set; }

        [DataMember(Order = 2)]
        public int objtype { get; set; }

        public string TenantAccountId { get; set; }

        public int UserId { get; set; }

        public string Token { get; set; }
    }

    [DataContract]
    public class GetApplicationObjectsResponse : IEbSSResponse
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
    public class GetUserRolesRequest : IReturn<GetUserRolesResponse>, IEbSSRequest
    {
        [DataMember(Order = 1)]
        public string Token { get; set; }

        [DataMember(Order = 2)]
        public int id { get; set; }
      
        public string TenantAccountId { get; set; }

        public int UserId { get; set; }
    }

    [DataContract]
    public class GetUserRolesResponse : IEbSSResponse
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
    public class RBACRolesRequest : IReturn<RBACRolesResponse>, IEbSSRequest
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
    public class RBACRolesResponse : IEbSSResponse
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
    public class CreateUserGroupRequest : IReturn<CreateUserGroupResponse>, IEbSSRequest
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
    public class CreateUserGroupResponse : IEbSSResponse
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
    public class UserPreferenceRequest : IReturn<UserPreferenceResponse>, IEbSSRequest
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
    public class UserPreferenceResponse : IEbSSResponse
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
    public class EditUserPreferenceRequest : IReturn<EditUserPreferenceResponse>, IEbSSRequest
    {
        
        public string TenantAccountId { get; set; }

        public int UserId { get; set; }

        public string Token { get; set; }
    }

    [DataContract]
    public class EditUserPreferenceResponse : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public Dictionary<string, object> Data { get; set; }

        [DataMember(Order = 2)]
        public ResponseStatus ResponseStatus { get; set; }

      
    }

    [DataContract]
    public class GetSubRolesRequest : IReturn<GetSubRolesResponse>, IEbSSRequest
    {
        [DataMember(Order = 1)]
        public string Token { get; set; }

        [DataMember(Order = 2)]
        public int id { get; set; }

        [DataMember(Order = 3)]
        public Dictionary<string, object> Colvalues { get; set; }

        public string TenantAccountId { get; set; }

        public int UserId { get; set; }
    }

    [DataContract]
    public class GetSubRolesResponse : IEbSSResponse
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
    public class GetRolesRequest : IReturn<GetRolesResponse>, IEbSSRequest
    {
       
        public string Token { get; set; }

        public string TenantAccountId { get; set; }

        public int UserId { get; set; }
    }

    [DataContract]
    public class GetRolesResponse : IEbSSResponse
    {
        
        [DataMember(Order = 1)]
        public Dictionary<string, object> Data { get; set; }

        [DataMember(Order = 2)]
        public string Token { get; set; }

        [DataMember(Order = 3)]
        public ResponseStatus ResponseStatus { get; set; }
    }

    [DataContract]
    public class GetPermissionsRequest : IReturn<GetPermissionsResponse>, IEbSSRequest
    {
        [DataMember(Order = 2)]
        public int id { get; set; }
        public string Token { get; set; }

        public string TenantAccountId { get; set; }

        public int UserId { get; set; }
    }

    [DataContract]
    public class GetPermissionsResponse : IEbSSResponse
    {

        [DataMember(Order = 1)]
        public Dictionary<string, object> Data { get; set; }
        [DataMember(Order = 2)]
        public List<string> Permissions { get; set; }

        [DataMember(Order = 3)]
        public string Token { get; set; }

        [DataMember(Order = 4)]
        public ResponseStatus ResponseStatus { get; set; }
    }

    [DataContract]
    public class GetUsersRequest : IReturn<GetUsersResponse>, IEbSSRequest
    {
        [DataMember(Order = 1)]
        public Dictionary<string, object> Colvalues { get; set; }
        public string Token { get; set; }

        public string TenantAccountId { get; set; }

        public int UserId { get; set; }
    }

    [DataContract]
    public class GetUsersResponse : IEbSSResponse
    {

        [DataMember(Order = 1)]
        public Dictionary<string, object> Data { get; set; }
       
        [DataMember(Order = 2)]
        public string Token { get; set; }

        [DataMember(Order = 3)]
        public ResponseStatus ResponseStatus { get; set; }
    }

    [DataContract]
    public class GetUsersRoleRequest : IReturn<GetUsersRoleResponse>, IEbSSRequest
    {
        [DataMember(Order = 1)]
        public Dictionary<string, object> Colvalues { get; set; }
        [DataMember(Order = 2)]
        public int id { get; set; }
        public string Token { get; set; }

        public string TenantAccountId { get; set; }

        public int UserId { get; set; }
    }

    [DataContract]
    public class GetUsersRoleResponse : IEbSSResponse
    {

        [DataMember(Order = 1)]
        public Dictionary<string, object> Data { get; set; }       

        [DataMember(Order = 2)]
        public string Token { get; set; }

        [DataMember(Order = 3)]
        public ResponseStatus ResponseStatus { get; set; }
    }

    [DataContract]
    public class GetUser2UserGroupRequest : IReturn<GetUser2UserGroupResponse>, IEbSSRequest
    {
       
        [DataMember(Order = 1)]
        public int id { get; set; }
        public string Token { get; set; }

        public string TenantAccountId { get; set; }

        public int UserId { get; set; }
    }

    [DataContract]
    public class GetUser2UserGroupResponse : IEbSSResponse
    {

        [DataMember(Order = 1)]
        public Dictionary<string, object> Data { get; set; }

        [DataMember(Order = 2)]
        public string Token { get; set; }

        [DataMember(Order = 3)]
        public ResponseStatus ResponseStatus { get; set; }
    }

    [DataContract]
    public class GetUserGroupRequest : IReturn<GetUserGroupResponse>, IEbSSRequest
    {
        [DataMember(Order = 1)]
        public Dictionary<string, object> Colvalues { get; set; }

        [DataMember(Order = 2)]
        public int id { get; set; }

        public string Token { get; set; }

        public string TenantAccountId { get; set; }

        public int UserId { get; set; }
    }

    [DataContract]
    public class GetUserGroupResponse : IEbSSResponse
    {

        [DataMember(Order = 1)]
        public Dictionary<string, object> Data { get; set; }

        [DataMember(Order = 2)]
        public string Token { get; set; }

        [DataMember(Order = 3)]
        public ResponseStatus ResponseStatus { get; set; }
    }



}
