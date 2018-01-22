using ExpressBase.Common;
using ExpressBase.Security.Core;
using ServiceStack;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace ExpressBase.Objects.ServiceStack_Artifacts
{
	//COMMON LIST-----------------------------------------
	[DataContract]
	public class GetUsersRequest1 : IReturn<GetUsersResponse1>, IEbSSRequest
	{
		[DataMember(Order = 1)]
		public Dictionary<string, object> Colvalues { get; set; }
		public string Token { get; set; }

		public string TenantAccountId { get; set; }

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
	}


	//USER START-----------------------------------------
	[DataContract]
	public class GetManageUserRequest : IReturn<GetManageUserResponse>, IEbSSRequest
	{

		[DataMember(Order = 1)]
		public int Id { get; set; }

		[DataMember(Order = 2)]
		public string TenantAccountId { get; set; }

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
		public string TenantAccountId { get; set; }

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

	public class UniqueCheckRequest
	{
		[DataMember(Order = 1)]
		public string email { get; set; }
	}

	//USER GROUP STRAT--------------------------------------
	[DataContract]
	public class GetManageUserGroupRequest : IReturn<GetManageUserGroupResponse>, IEbSSRequest
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
	public class GetManageUserGroupResponse : IEbSSResponse
	{

		[DataMember(Order = 1)]
		public Dictionary<string, object> Data { get; set; }

		[DataMember(Order = 2)]
		public string Token { get; set; }

		[DataMember(Order = 3)]
		public ResponseStatus ResponseStatus { get; set; }
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
		public string TenantAccountId { get; set; }

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
		public string Token { get; set; }

		[DataMember(Order = 8)]
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
	public class ApplicationCollection : List<Application>
	{
		public ApplicationCollection() { }

		public ApplicationCollection(EbDataTable dtApp, EbDataTable dtObjects)
		{
			foreach (var dr in dtApp.Rows)
			{
				int appid = Convert.ToInt32(dr[0]);
				this.Add(new Application { Id = appid, Name = dr[1].ToString() });
			}

			foreach (EbDataRow dr in dtObjects.Rows)
			{
				var app_id = Convert.ToInt32(dr[3]);
				var ob_type = Convert.ToInt32(dr[2]);

				if (!this[app_id].ObjectTypes.ContainsKey(ob_type))
					this[app_id].ObjectTypes.Add(ob_type);

				this[app_id].ObjectTypes[ob_type].Add(new Eb_Object { Obj_Id = Convert.ToInt32(dr[0]), Obj_Name = dr[1].ToString() });
			}
		}

		// CREATE NEW INDEXER
		new public Application this[int appid]
		{
			get
			{
				Application _result = null;
				foreach(Application app in this)
				{
					if (app.Id == appid)
					{
						_result = app;
						break;
					}
				}

				return _result;
			}
		}
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
	public class ObjectTypeCollection : Dictionary<int, ObjectCollection>
	{
		public void Add(int obj_type)
		{
			this.Add(obj_type, new ObjectCollection());
		}
	}

	[DataContract]
	public class ObjectCollection : List<Eb_Object>
	{

	}

	[DataContract]
	public class Eb_Object
	{
		[DataMember(Order = 1)]
		public int Obj_Id;

		[DataMember(Order = 2)]
		public string Obj_Name;
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
	public class Eb_Users
	{
		[DataMember(Order = 1)]
		public int Id;

		[DataMember(Order = 2)]
		public string Name;

		[DataMember(Order = 3)]
		public string Email;

		[DataMember(Order = 4)]
		public int Role2User_Id;
	}
	
	[DataContract]
	public class GetUserDetailsRequest : IReturn<GetUserDetailsResponse>, IEbSSRequest
	{
		[DataMember(Order = 1)]
		public string SearchText { get; set; }
		
		[DataMember(Order = 2)]
		public string TenantAccountId { get; set; }

		[DataMember(Order = 3)]
		public int UserId { get; set; }
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
	public class SaveRoleRequest : IReturn<SaveRoleResponse>, IEbSSRequest
	{
		[DataMember(Order = 0)]
		public Dictionary<string, object> Colvalues { get; set; }

		[DataMember(Order = 2)]
		public int Id { get; set; }

		[DataMember(Order = 3)]
		public string TenantAccountId { get; set; }

		[DataMember(Order = 4)]
		public int UserId { get; set; }

		[DataMember(Order = 5)]
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

}
