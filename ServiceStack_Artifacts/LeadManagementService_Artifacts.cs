using ExpressBase.Common.EbServiceStack.ReqNRes;
using ExpressBase.Common.Structures;
using ServiceStack;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace ExpressBase.Objects.ServiceStack_Artifacts
{
	[DataContract]
	public class GetManageLeadRequest : IReturn<GetManageLeadResponse>, IEbSSRequest
	{
		[DataMember(Order = 1)]
		public string AccId { get; set; }

		[DataMember(Order = 2)]
		public int RequestMode { get; set; }

		[DataMember(Order = 3)]
		public string TenantAccountId { get; set; }

		public int UserId { get; set; }
	}

	[DataContract]
	public class GetManageLeadResponse : IEbSSResponse
	{
		[DataMember(Order = 1)]
		public string AccId { get; set; }

		[DataMember(Order = 2)]
		public Dictionary<string, string> CustomerDataDict { get; set; }

		[DataMember(Order = 3)]
		public Dictionary<int, string> CostCenterDict { get; set; }
		
		[DataMember(Order = 4)]
		public ResponseStatus ResponseStatus { get; set; }

	}

	[DataContract]
	public class SaveCustomerRequest : IReturn<SaveCustomerResponse>, IEbSSRequest
	{
		[DataMember(Order = 1)]
		public string CustomerData { get; set; }

		[DataMember(Order = 2)]
		public int RequestMode { get; set; }

		[DataMember(Order = 3)]
		public string TenantAccountId { get; set; }

		public int UserId { get; set; }
	}

	[DataContract]
	public class SaveCustomerResponse : IEbSSResponse
	{
		[DataMember(Order = 1)]
		public int Status { get; set; }

		[DataMember(Order = 3)]
		public ResponseStatus ResponseStatus { get; set; }

	}

	[DataContract]
	public class KeyValueType_Field
	{
		[DataMember(Order = 1)]
		public string Key { get; set; }

		[DataMember(Order = 2)]
		public object Value { get; set; }
		
		[DataMember(Order = 3)]
		public EbDbTypes Type { get; set; }
	}
}
