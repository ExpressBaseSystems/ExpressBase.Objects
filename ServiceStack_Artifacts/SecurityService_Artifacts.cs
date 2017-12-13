using ServiceStack;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace ExpressBase.Objects.ServiceStack_Artifacts
{
	[DataContract]
	public class GetApplicationRequest1 : IReturn<GetApplicationResponse1>, IEbSSRequest
	{
		[DataMember(Order = 1)]
		public string Token { get; set; }

		[DataMember(Order = 2)]
		public int id { get; set; }

		public string TenantAccountId { get; set; }

		public int UserId { get; set; }
	}

	[DataContract]
	public class GetApplicationResponse1 : IEbSSResponse
	{
		[DataMember(Order = 1)]
		public Dictionary<string, object> Data { get; set; }

		[DataMember(Order = 2)]
		public string Token { get; set; }

		[DataMember(Order = 3)]
		public ResponseStatus ResponseStatus { get; set; }
	}

	[DataContract]
	public class GetPermissionsRequest1 : IReturn<GetPermissionsResponse1>, IEbSSRequest
	{
		[DataMember(Order = 2)]
		public int id { get; set; }
		public string Token { get; set; }

		public string TenantAccountId { get; set; }

		public int UserId { get; set; }
	}

	[DataContract]
	public class GetPermissionsResponse1 : IEbSSResponse
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
}
