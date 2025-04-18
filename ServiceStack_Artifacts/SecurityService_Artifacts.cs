﻿using ExpressBase.Common;
using ExpressBase.Common.EbServiceStack.ReqNRes;
using ExpressBase.Common.LocationNSolution;
using ExpressBase.Security;
using ExpressBase.Security.Core;
using ServiceStack;
using ServiceStack.Auth;
using ServiceStack.Logging;
using ServiceStack.Web;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Runtime.Serialization;
using System.Text;

namespace ExpressBase.Objects.ServiceStack_Artifacts
{
    [DataContract]
    public class GetUsersRequest1 : IReturn<GetUsersResponse1>, IEbSSRequest
    {
        [DataMember(Order = 1)]
        public string Show { get; set; }

        public string Token { get; set; }

        public string SolnId { get; set; }

        public int UserId { get; set; }
    }

    [DataContract]
    public class GetUsersResponse1 : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public List<Eb_User_ForCommonList> Data { get; set; }

        [DataMember(Order = 2)]
        public string Token { get; set; }

        [DataMember(Order = 3)]
        public ResponseStatus ResponseStatus { get; set; }
    }

    [DataContract]
    public class Eb_User_ForCommonList
    {
        [DataMember(Order = 1)]
        public int Id;

        [DataMember(Order = 2)]
        public string Name;

        [DataMember(Order = 3)]
        public string Email;

        [DataMember(Order = 4)]
        public string Nick_Name;

        [DataMember(Order = 5)]
        public string Sex;

        [DataMember(Order = 6)]
        public string Phone_Number;

        [DataMember(Order = 7)]
        public string Status;

        [DataMember(Order = 8)]
        public string User_Type;

        [DataMember(Order = 9)]
        public string Location;

        [DataMember(Order = 10)]
        public string Api_Key;
    }

    [DataContract]
    public class GetAnonymousUserRequest : IReturn<GetAnonymousUserResponse>, IEbSSRequest
    {
        [DataMember(Order = 1)]
        public Dictionary<string, object> Colvalues { get; set; }
        public string Token { get; set; }

        public string SolnId { get; set; }

        public int UserId { get; set; }
    }

    [DataContract]
    public class GetAnonymousUserResponse : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public List<Eb_AnonymousUser_ForCommonList> Data { get; set; }

        [DataMember(Order = 2)]
        public string Token { get; set; }

        [DataMember(Order = 3)]
        public ResponseStatus ResponseStatus { get; set; }
    }

    [DataContract]
    public class Eb_AnonymousUser_ForCommonList
    {
        [DataMember(Order = 1)]
        public int Id;

        [DataMember(Order = 2)]
        public string Full_Name;

        [DataMember(Order = 3)]
        public string Social_Id;

        [DataMember(Order = 4)]
        public string Email_Id;

        [DataMember(Order = 5)]
        public string Phone_No;

        [DataMember(Order = 6)]
        public string First_Visit;

        [DataMember(Order = 7)]
        public string Last_Visit;

        [DataMember(Order = 8)]
        public int Total_Visits;

        [DataMember(Order = 9)]
        public string App_Name;
    }


    [DataContract]
    public class GetUserGroupRequest1 : IReturn<GetUserGroupResponse1>, IEbSSRequest
    {
        [DataMember(Order = 1)]
        public Dictionary<string, object> Colvalues { get; set; }
        public string Token { get; set; }

        public string SolnId { get; set; }

        public int UserId { get; set; }
    }

    [DataContract]
    public class GetUserGroupResponse1 : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public List<Eb_UserGroup_ForCommonList> Data { get; set; }

        [DataMember(Order = 2)]
        public string Token { get; set; }

        [DataMember(Order = 3)]
        public ResponseStatus ResponseStatus { get; set; }
    }

    [DataContract]
    public class Eb_UserGroup_ForCommonList
    {
        [DataMember(Order = 1)]
        public int Id;

        [DataMember(Order = 2)]
        public string Name;

        [DataMember(Order = 3)]
        public string Description;
    }

    [DataContract]
    public class GetRolesRequest1 : IReturn<GetRolesResponse1>, IEbSSRequest
    {
        [DataMember(Order = 1)]
        public Dictionary<string, object> Colvalues { get; set; }
        public string Token { get; set; }

        public string SolnId { get; set; }

        public int UserId { get; set; }
    }

    [DataContract]
    public class GetRolesResponse1 : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public List<Eb_Roles_ForCommonList> Data { get; set; }

        [DataMember(Order = 2)]
        public string Token { get; set; }

        [DataMember(Order = 3)]
        public ResponseStatus ResponseStatus { get; set; }
    }

    [DataContract]
    public class Eb_Roles_ForCommonList
    {
        [DataMember(Order = 1)]
        public int Id;

        [DataMember(Order = 2)]
        public string Name;

        [DataMember(Order = 3)]
        public string Description;

        [DataMember(Order = 4)]
        public string Application_Name;

        [DataMember(Order = 5)]
        public int SubRole_Count;

        [DataMember(Order = 6)]
        public int User_Count;

        [DataMember(Order = 7)]
        public int Permission_Count;
    }


    //USER START-----------------------------------------
    [DataContract]
    public class GetManageUserRequest : IReturn<GetManageUserResponse>, IEbSSRequest
    {

        [DataMember(Order = 1)]
        public int Id { get; set; }

        [DataMember(Order = 2)]
        public int RqstMode { get; set; }

        [DataMember(Order = 2)]
        public string SolnId { get; set; }

        public int UserId { get; set; }

        public string Token { get; set; }
    }

    [DataContract]
    public class GetManageUserResponse : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public Dictionary<string, string> UserData { get; set; }

        [DataMember(Order = 2)]
        public List<EbRole> Roles { get; set; }

        [DataMember(Order = 3)]
        public List<int> UserRoles { get; set; }

        [DataMember(Order = 4)]
        public List<EbUserGroups> EbUserGroups { get; set; }

        [DataMember(Order = 5)]
        public List<int> UserGroups { get; set; }

        [DataMember(Order = 6)]
        public List<Eb_RoleToRole> Role2RoleList { get; set; }

        [DataMember(Order = 7)]
        public string Token { get; set; }

        [DataMember(Order = 8)]
        public ResponseStatus ResponseStatus { get; set; }

        [DataMember(Order = 9)]
        public string u_token { get; set; }

        [DataMember(Order = 10)]
        public Dictionary<int, int> LocConstraint { get; set; }

        [DataMember(Order = 11)]
        public Dictionary<int, string> UserTypes { get; set; }
    }

    [DataContract]
    public class SaveUserRequest : IReturn<SaveUserResponse>, IEbSSRequest
    {
        [DataMember(Order = 1)]
        public int Id { get; set; }

        [DataMember(Order = 2)]
        public string FullName { get; set; }

        [DataMember(Order = 3)]
        public string NickName { get; set; }

        [DataMember(Order = 4)]
        public string EmailPrimary { get; set; }

        [DataMember(Order = 5)]
        public string EmailSecondary { get; set; }

        [DataMember(Order = 6)]
        public string Sex { get; set; }

        [DataMember(Order = 7)]
        public string DateOfBirth { get; set; }

        [DataMember(Order = 8)]
        public string PhonePrimary { get; set; }

        [DataMember(Order = 9)]
        public string PhoneSecondary { get; set; }

        [DataMember(Order = 10)]
        public string LandPhone { get; set; }

        [DataMember(Order = 11)]
        public string PhoneExtension { get; set; }

        [DataMember(Order = 12)]
        public string FbId { get; set; }

        [DataMember(Order = 13)]
        public string FbName { get; set; }

        [DataMember(Order = 14)]
        public string Roles { get; set; }

        [DataMember(Order = 15)]
        public string UserGroups { get; set; }

        [DataMember(Order = 16)]
        public string StatusId { get; set; }

        [DataMember(Order = 17)]
        public string Hide { get; set; }

        [DataMember(Order = 18)]
        public string Password { get; set; }

        [DataMember(Order = 19)]
        public int AnonymousUserId { get; set; }

        [DataMember(Order = 20)]
        public string Preference { get; set; }

        [DataMember(Order = 22)]
        public string LocationAdd { get; set; }

        [DataMember(Order = 23)]
        public string LocationDelete { get; set; }

        [DataMember(Order = 24)]
        public string SolnId { get; set; }

        [DataMember(Order = 25)]
        public int UserType { get; set; }

        [DataMember(Order = 26)]
        public string ForceResetPassword { get; set; }

        public int UserId { get; set; }

        public string Token { get; set; }
    }

    [DataContract]
    public class SaveUserResponse : IEbSSResponse
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
    public class DeleteUserRequest : IReturn<DeleteUserResponse>, IEbSSRequest
    {
        [DataMember(Order = 1)]
        public int Id { get; set; }

        public int UserId { get; set; }

        public string SolnId { get; set; }
    }

    [DataContract]
    public class DeleteUserResponse : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public int Status { get; set; }

        public ResponseStatus ResponseStatus { get; set; }
    }

    [DataContract]
    public class IsApiKeyExistsRequest : IReturn<IsApiKeyExistsResponse>, IEbSSRequest
    {
        [DataMember(Order = 1)]
        public int Id { get; set; }

        public int UserId { get; set; }

        public string SolnId { get; set; }
    }

    [DataContract]
    public class IsApiKeyExistsResponse : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public int KeyId { get; set; }

        [DataMember(Order = 2)]
        public ResponseStatus ResponseStatus { get; set; }

        public IsApiKeyExistsResponse()
        {
            ResponseStatus = new ResponseStatus();
        }
    }

    [DataContract]
    public class ViewApiKeyRequest : IReturn<ViewApiKeyResponse>, IEbSSRequest
    {
        [DataMember(Order = 1)]
        public int Id { get; set; }

        public int UserId { get; set; }

        public string SolnId { get; set; }
    }

    [DataContract]
    public class ViewApiKeyResponse : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public string ApiKey { get; set; }

        [DataMember(Order = 2)]
        public ResponseStatus ResponseStatus { get; set; }

        public ViewApiKeyResponse()
        {
            ResponseStatus = new ResponseStatus();
        }
    }

    [DataContract]
    public class DeleteApiKeyRequest : IReturn<DeleteApiKeyResponse>, IEbSSRequest
    {
        [DataMember(Order = 1)]
        public int Id { get; set; }

        public int UserId { get; set; }

        public string SolnId { get; set; }
    }

    [DataContract]
    public class DeleteApiKeyResponse : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public int Status { get; set; }

        [DataMember(Order = 2)]
        public ResponseStatus ResponseStatus { get; set; }

        public DeleteApiKeyResponse()
        {
            ResponseStatus = new ResponseStatus();
        }
    }

    [DataContract]
    public class GenerateApiKeyRequest : IReturn<GenerateApiKeyResponse>, IEbSSRequest
    {
        [DataMember(Order = 1)]
        public int Id { get; set; }

        public int UserId { get; set; }

        public string SolnId { get; set; }
    }

    [DataContract]
    public class GenerateApiKeyResponse : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public string ApiKey { get; set; }

        [DataMember(Order = 2)]
        public ResponseStatus ResponseStatus { get; set; }

        public GenerateApiKeyResponse()
        {
            ResponseStatus = new ResponseStatus();
        }
    }

    public class UniqueCheckRequest
    {
        [DataMember(Order = 1)]
        public string Value { get; set; }

        [DataMember(Order = 2)]
        public int Id { get; set; }

        [DataMember(Order = 3)]
        public UniqueCheckQueryId QryId { get; set; }

        [DataMember(Order = 4)]
        public string email { get; set; }

        [DataMember(Order = 5)]
        public string roleName { get; set; }
    }
    public class UniqueCheckResponse
    {
        [DataMember(Order = 1)]
        public bool unrespose { get; set; }
    }

    public enum UniqueCheckQueryId
    {
        eb_users__email = 1,
        eb_users__phnoprimary,
        eb_roles__role_name,
        eb_users__nickname
    }

    [DataContract]
    public class ResetUserPasswordRequest : IReturn<ResetUserPasswordResponse>, IEbSSRequest
    {
        [DataMember(Order = 1)]
        public int Id { get; set; }

        [DataMember(Order = 2)]
        public string Email { get; set; }

        [DataMember(Order = 3)]
        public string NewPwd { get; set; }

        [DataMember(Order = 4)]
        public string SolnId { get; set; }

        [DataMember(Order = 5)]
        public int UserId { get; set; }
    }

    [DataContract]
    public class ResetUserPasswordResponse
    {
        [DataMember(Order = 1)]
        public bool isSuccess { get; set; }
    }

    //MY PROFILE ----------------------------------------------------

    [DataContract]
    public class GetMyProfileRequest : IReturn<GetMyProfileResponse>, IEbSSRequest
    {
        [DataMember(Order = 1)]
        public string SolnId { get; set; }

        [DataMember(Order = 2)]
        public string WC { get; set; }

        public int UserId { get; set; }

        public string Token { get; set; }

        [DataMember(Order = 3)]
        public List<int> DBIds { get; set; }

        [DataMember(Order = 4)]
        public bool IsSolutionOwner { get; set; }
    }

    [DataContract]
    public class GetMyProfileResponse : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public Dictionary<string, string> UserData { get; set; }

        [DataMember(Order = 2)]
        public ResponseStatus ResponseStatus { get; set; }

        [DataMember(Order = 2)]
        public Dictionary<string, string> RefIds { get; set; }
    }

    [DataContract]
    public class SaveMyProfileRequest : IReturn<SaveMyProfileResponse>, IEbSSRequest
    {
        [DataMember(Order = 1)]
        public string UserData { get; set; }

        [DataMember(Order = 2)]
        public string SolnId { get; set; }

        [DataMember(Order = 3)]
        public string WC { get; set; }

        [DataMember(Order = 4)]
        public bool PreferenceOnly { get; set; }

        public int UserId { get; set; }

        public string Token { get; set; }
    }

    [DataContract]
    public class SaveMyProfileResponse : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public int RowsAffectd { get; set; }

        [DataMember(Order = 2)]
        public ResponseStatus ResponseStatus { get; set; }
    }

    [DataContract]
    public class ChangeUserPasswordRequest : IReturn<ChangeUserPasswordResponse>, IEbSSRequest
    {
        [DataMember(Order = 1)]
        public string OldPwd { get; set; }

        [DataMember(Order = 2)]
        public string NewPwd { get; set; }

        [DataMember(Order = 3)]
        public string Email { get; set; }

        [DataMember(Order = 4)]
        public string SolnId { get; set; }

        [DataMember(Order = 5)]
        public string WC { get; set; }

        public int UserId { get; set; }
    }

    [DataContract]
    public class ChangeUserPasswordResponse
    {
        [DataMember(Order = 1)]
        public bool isSuccess { get; set; }
    }


    //ANONYMOUS USER START-----------------------------------------
    [DataContract]
    public class GetManageAnonymousUserRequest : IReturn<GetManageAnonymousUserResponse>, IEbSSRequest
    {

        [DataMember(Order = 1)]
        public int Id { get; set; }

        [DataMember(Order = 2)]
        public string SolnId { get; set; }

        public int UserId { get; set; }

        public string Token { get; set; }
    }

    [DataContract]
    public class GetManageAnonymousUserResponse : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public Dictionary<string, string> UserData { get; set; }

        [DataMember(Order = 2)]
        public string Token { get; set; }

        [DataMember(Order = 3)]
        public ResponseStatus ResponseStatus { get; set; }
    }

    [DataContract]
    public class UpdateAnonymousUserRequest : IReturn<UpdateAnonymousUserResponse>, IEbSSRequest
    {
        [DataMember(Order = 1)]
        public int Id { get; set; }

        [DataMember(Order = 2)]
        public string FullName { get; set; }

        [DataMember(Order = 3)]
        public string EmailID { get; set; }

        [DataMember(Order = 4)]
        public string PhoneNumber { get; set; }

        [DataMember(Order = 5)]
        public string Remarks { get; set; }

        [DataMember(Order = 6)]
        public string SolnId { get; set; }

        public int UserId { get; set; }

        public string Token { get; set; }
    }

    [DataContract]
    public class UpdateAnonymousUserResponse : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public int RowAffected { get; set; }

        [DataMember(Order = 2)]
        public ResponseStatus ResponseStatus { get; set; }
    }

    [DataContract]
    public class ConvertAnonymousUserRequest : IReturn<ConvertAnonymousUserResponse>, IEbSSRequest
    {
        [DataMember(Order = 1)]
        public int Id { get; set; }

        [DataMember(Order = 2)]
        public string FullName { get; set; }

        [DataMember(Order = 3)]
        public string EmailID { get; set; }

        [DataMember(Order = 4)]
        public string PhoneNumber { get; set; }

        [DataMember(Order = 5)]
        public string Remarks { get; set; }

        [DataMember(Order = 6)]
        public string SolnId { get; set; }

        public int UserId { get; set; }

        public string Token { get; set; }
    }

    [DataContract]
    public class ConvertAnonymousUserResponse : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public int status { get; set; }

        [DataMember(Order = 2)]
        public ResponseStatus ResponseStatus { get; set; }
    }



    //USER GROUP STRAT--------------------------------------
    [DataContract]
    public class GetManageUserGroupRequest : IReturn<GetManageUserGroupResponse>, IEbSSRequest
    {
        [DataMember(Order = 1)]
        public int id { get; set; }

        [DataMember(Order = 2)]
        public string Timezone { get; set; }

        public string Token { get; set; }

        public string SolnId { get; set; }

        public int UserId { get; set; }
    }

    [DataContract]
    public class GetManageUserGroupResponse : IEbSSResponse
    {

        [DataMember(Order = 1)]
        public Dictionary<string, object> SelectedUserGroupInfo { get; set; }

        [DataMember(Order = 2)]
        public List<Eb_Users> UsersList { get; set; }

        [DataMember(Order = 3)]
        public List<Eb_Constraints1> IpConsList { get; set; }

        [DataMember(Order = 4)]
        public List<Eb_Constraints1> DtConsList { get; set; }

        [DataMember(Order = 5)]
        public List<Eb_Users> UsersListAll { get; set; }

        [DataMember(Order = 6)]
        public string Token { get; set; }

        [DataMember(Order = 7)]
        public ResponseStatus ResponseStatus { get; set; }
    }

    [DataContract]
    public class SaveUserGroupRequest : IReturn<SaveUserGroupResponse>, IEbSSRequest
    {
        [DataMember(Order = 1)]
        public int Id { get; set; }

        [DataMember(Order = 2)]
        public string Name { get; set; }

        [DataMember(Order = 3)]
        public string Description { get; set; }

        [DataMember(Order = 4)]
        public string Users { get; set; }

        [DataMember(Order = 5)]
        public string IpConstraintNw { get; set; }

        [DataMember(Order = 6)]
        public string IpConstraintOld { get; set; }

        [DataMember(Order = 7)]
        public string DtConstraintNw { get; set; }

        [DataMember(Order = 8)]
        public string DtConstraintOld { get; set; }

        [DataMember(Order = 9)]
        public string UsersTimezone { get; set; }

        [DataMember(Order = 10)]
        public string SolnId { get; set; }

        public int UserId { get; set; }

        public string Token { get; set; }
    }

    [DataContract]
    public class SaveUserGroupResponse : IEbSSResponse
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

    //ROLES START---------------------------------------------
    [DataContract]
    public class GetManageRolesRequest : IReturn<GetManageRolesResponse>, IEbSSRequest
    {
        [DataMember(Order = 1)]
        public int id { get; set; }

        [DataMember(Order = 2)]
        public string Token { get; set; }

        [DataMember(Order = 3)]
        public string SolnId { get; set; }

        [DataMember(Order = 4)]
        public int UserId { get; set; }
    }

    [DataContract]
    public class GetManageRolesResponse : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public ApplicationCollection ApplicationCollection { get; set; }

        [DataMember(Order = 2)]
        public Dictionary<string, object> SelectedRoleInfo { get; set; }

        [DataMember(Order = 3)]
        public List<string> PermissionList { get; set; }

        [DataMember(Order = 4)]
        public List<Eb_RoleObject> RoleList { get; set; }

        [DataMember(Order = 5)]
        public List<Eb_RoleToRole> Role2RoleList { get; set; }

        [DataMember(Order = 6)]
        public List<Eb_Users> UsersList { get; set; }

        [DataMember(Order = 7)]
        public List<Eb_Location> LocationList { get; set; }

        [DataMember(Order = 8)]
        public List<Eb_Users> UsersListAll { get; set; }

        [DataMember(Order = 9)]
        public string Token { get; set; }

        [DataMember(Order = 10)]
        public ResponseStatus ResponseStatus { get; set; }
    }

    //[DataContract]
    //public class GetObjectAndPermissionRequest : IReturn<GetObjectAndPermissionResponse>, IEbSSRequest
    //{
    //	[DataMember(Order = 1)]
    //	public int RoleId { get; set; }

    //	[DataMember(Order =2)]
    //	public int AppId { get; set; }

    //	[DataMember(Order = 3)]
    //	public string TenantAccountId { get; set; }

    //	[DataMember(Order = 4)]
    //	public int UserId { get; set; }
    //}

    //[DataContract]
    //public class GetObjectAndPermissionResponse : IEbSSResponse
    //{
    //	[DataMember(Order = 1)]
    //	public Dictionary<int, List<Eb_Object>> Data { get; set; }

    //	[DataMember(Order = 2)]
    //	public List<string> Permissions { get; set; }

    //	[DataMember(Order = 3)]
    //	public string Token { get; set; }

    //	[DataMember(Order = 4)]
    //	public ResponseStatus ResponseStatus { get; set; }
    //}

    [DataContract]
    public class ApplicationCollection
    {
        [DataMember(Order = 1)]
        public List<Application> _acol { get; set; }

        public ApplicationCollection() { _acol = new List<Application>(); }

        public ApplicationCollection(EbDataTable dtApp, EbDataTable dtObjects)
        {
            _acol = new List<Application>();
            foreach (var dr in dtApp.Rows)
            {
                int appid = Convert.ToInt32(dr[0]);
                _acol.Add(new Application { Id = appid, Name = dr[1].ToString() });
            }

            foreach (EbDataRow dr in dtObjects.Rows)
            {
                var app_id = Convert.ToInt32(dr[3]);
                var ob_type = Convert.ToInt32(dr[2]);
                if (app_id != 0)
                {
                    Application _app = this.GetApplication(app_id);
                    if (_app == null)
                        continue;
                    if (!_app.ObjectTypes.ContainsKey(ob_type))
                        _app.ObjectTypes.Add(ob_type);

                    _app.ObjectTypes[ob_type].Add(new Eb_Object { Obj_Id = Convert.ToInt32(dr[0]), Obj_Name = dr[1].ToString() });
                }
            }
        }

        public Application GetApplication(int appid)
        {
            Application _result = null;
            foreach (Application app in _acol)
            {
                if (app.Id == appid)
                {
                    _result = app;
                    break;
                }
            }

            return _result;
        }

        //// CREATE NEW INDEXER
        //new public Application this[int appid]
        //{
        //	get
        //	{
        //		Application _result = null;
        //		foreach(Application app in this)
        //		{
        //			if (app.Id == appid)
        //			{
        //				_result = app;
        //				break;
        //			}
        //		}

        //		return _result;
        //	}
        //}
    }

    [DataContract]
    public class Application
    {
        [DataMember(Order = 1)]
        public int Id { get; set; }

        [DataMember(Order = 2)]
        public string Name { get; set; }

        [DataMember(Order = 3)]
        public ObjectTypeCollection ObjectTypes { get; set; }

        public Application()
        {
            this.ObjectTypes = new ObjectTypeCollection();
        }

    }

    [DataContract]
    public class ObjectTypeCollection
    {
        [DataMember(Order = 1)]
        public Dictionary<int, ObjectCollection> _otypecol { get; set; }

        public ObjectTypeCollection()
        {
            _otypecol = new Dictionary<int, ObjectCollection>();
        }

        public void Add(int obj_type)
        {
            _otypecol.Add(obj_type, new ObjectCollection());
        }

        public bool ContainsKey(int obj_type)
        {
            return (_otypecol.ContainsKey(obj_type));
        }

        public ObjectCollection this[int obj_type]
        {
            get { return _otypecol[obj_type]; }
        }
    }

    [DataContract]
    public class ObjectCollection
    {
        [DataMember(Order = 1)]
        public List<Eb_Object> _obcol { get; set; }

        public ObjectCollection() { _obcol = new List<Eb_Object>(); }

        public void Add(Eb_Object ob)
        {
            _obcol.Add(ob);
        }
    }

    [DataContract]
    public class Eb_Object
    {
        [DataMember(Order = 1)]
        public int Obj_Id;

        [DataMember(Order = 2)]
        public string Obj_Name;

        public Eb_Object() { }
    }

    [DataContract]
    public class Eb_ObjectTypeOperations
    {
        [DataMember(Order = 1)]
        public int Op_Id;

        [DataMember(Order = 2)]
        public string Op_Name;

        [DataMember(Order = 3)]
        public List<string> Operations;
    }

    [DataContract]
    public class Eb_RoleObject
    {
        [DataMember(Order = 1)]
        public int Id;

        [DataMember(Order = 2)]
        public string Name;

        [DataMember(Order = 3)]
        public string Description;

        [DataMember(Order = 4)]
        public int App_Id;

        [DataMember(Order = 5)]
        public bool Is_Anonymous;

        [DataMember(Order = 6)]
        public bool Is_System;
    }

    [DataContract]
    public class Eb_RoleToRole
    {
        [DataMember(Order = 1)]
        public int Id;

        [DataMember(Order = 2)]
        public int Dominant;

        [DataMember(Order = 3)]
        public int Dependent;
    }

    [DataContract]
    public class Eb_Location
    {
        [DataMember(Order = 1)]
        public int Id;

        [DataMember(Order = 2)]
        public string LongName;

        [DataMember(Order = 3)]
        public string ShortName;
    }

    [DataContract]
    public class Eb_Users
    {
        [DataMember(Order = 1)]
        public int Id;

        [DataMember(Order = 2)]
        public string Name;

        [DataMember(Order = 3)]
        public string Email;

        [DataMember(Order = 4)]
        public string Phone;

        [DataMember(Order = 5)]
        public int Role2User_Id;
    }

    [DataContract]
    public class Eb_Constraints1
    {
        [DataMember(Order = 1)]
        public int Id;

        [DataMember(Order = 2)]
        public string Title;

        [DataMember(Order = 3)]
        public string Description;
    }

    [DataContract]
    public class GetUserDetailsRequest : EbServiceStackAuthRequest, IReturn<GetUserDetailsResponse>
    {
        [DataMember(Order = 1)]
        public string SearchText { get; set; }
    }
    [DataContract]
    public class GetUserDetailsResponse : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public List<Eb_Users> UserList { get; set; }

        [DataMember(Order = 2)]
        public string Token { get; set; }

        [DataMember(Order = 3)]
        public ResponseStatus ResponseStatus { get; set; }
    }

    [DataContract]
    public class SaveRoleRequest : EbServiceStackAuthRequest, IReturn<SaveRoleResponse>
    {
        [DataMember(Order = 0)]
        public Dictionary<string, object> Colvalues { get; set; }

        [DataMember(Order = 2)]
        public int Id { get; set; }

        [DataMember(Order = 3)]
        public string Token { get; set; }
    }

    [DataContract]
    public class SaveRoleResponse : IEbSSResponse
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


    //Login Activity 
    public class LoginActivityRequest : EbServiceStackAuthRequest, IReturn<LoginActivityResponse>
    {
        [DataMember(Order = 1)]


        public int TargetUser { get; set; }

        public bool Alluser { get; set; }

        public User UserObject { get; set; }
    }

    [DataContract]
    public class LoginActivityResponse
    {
        [DataMember(Order = 1)]
        public List<LoginLogCls> ActivityList { get; set; } = new List<LoginLogCls>();

        [DataMember(Order = 2)]
        public EbDataTable _data { get; set; }

        [DataMember(Order = 2)]
        public string ErrMsg { get; set; }

        [DataMember(Order = 3)]
        public ResponseStatus ResponseStatus { get; set; }
    }

    public class GetUserTypesRequest : EbServiceStackAuthRequest, IReturn<GetUserTypesResponse>
    {
        public int Id { get; set; }
    }

    public class GetUserTypesResponse
    {
        public List<EbProfileUserType> UserTypes { get; set; }
    }

    public class UpdateUserTypeRequset : EbServiceStackAuthRequest, IReturn<UpdateUserTypeResponse>
    {
        public int Id { get; set; }

        public string Name { get; set; }
    }
    public class UpdateUserTypeResponse
    {
        public bool Status { get; set; }
    }
    public class LoginLogCls
    {
        public string UserName { get; set; }
        public string IpAddress { get; set; }
        public string SignInAt { get; set; }
        public string SignOutAt { get; set; }
        public string SignOutDate { get; set; }
    }

    public class BrowserExceptionMqRequest : EbServiceStackAuthRequest, IReturn<BrowserExceptionResponse>
    {
        public string Device_info { get; set; }

        public string Ip_address { get; set; }
        public string Error_msg { get; set; }
        public string Network_info { get; set; }

    }
    public class BrowserExceptionRequest : EbMqRequest
    {
        public string Device_info { get; set; }

        public string Ip_address { get; set; }
        public string Error_msg { get; set; }
        public string Network_info { get; set; }

    }

    public class BrowserExceptionResponse : IEbSSResponse
    {
        public bool Status { get; set; }

        public ResponseStatus ResponseStatus { get; set; }

    }
}
