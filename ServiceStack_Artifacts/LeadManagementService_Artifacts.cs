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
		public int AccId { get; set; }

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
		public List<FeedbackEntry> FeedbackList { get; set; }

		[DataMember(Order = 5)]
		public List<GraftEntry> GraftList { get; set; }

		[DataMember(Order = 6)]
		public List<BillingEntry> BillingList { get; set; }

		[DataMember(Order = 7)]
		public List<SurgeryEntry> SurgeryList { get; set; }

		[DataMember(Order = 8)]
		public ResponseStatus ResponseStatus { get; set; }

		[DataMember(Order = 9)]
		public Dictionary<string, int> DoctorDict { get; set; }

		[DataMember(Order = 10)]
		public Dictionary<string, int> StaffDict { get; set; }

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
	public class SaveCustomerFollowupRequest : IReturn<SaveCustomerFollowupResponse>, IEbSSRequest
	{
		[DataMember(Order = 1)]
		public string Data { get; set; }

		[DataMember(Order = 2)]
		public string TenantAccountId { get; set; }

		[DataMember(Order = 3)]
		public int UserId { get; set; }

		[DataMember(Order = 4)]
		public string UserName { get; set; }
	}

	[DataContract]
	public class SaveCustomerFollowupResponse : IEbSSResponse
	{
		[DataMember(Order = 1)]
		public int Status { get; set; }

		[DataMember(Order = 3)]
		public ResponseStatus ResponseStatus { get; set; }

	}

	[DataContract]
	public class SaveCustomerGraftRequest : IReturn<SaveCustomerGraftResponse>, IEbSSRequest
	{
		[DataMember(Order = 1)]
		public string Data { get; set; }

		[DataMember(Order = 2)]
		public int RequestMode { get; set; }

		[DataMember(Order = 3)]
		public string TenantAccountId { get; set; }

		public int UserId { get; set; }
	}

	[DataContract]
	public class SaveCustomerGraftResponse : IEbSSResponse
	{
		[DataMember(Order = 1)]
		public int Status { get; set; }

		[DataMember(Order = 3)]
		public ResponseStatus ResponseStatus { get; set; }

	}

	[DataContract]
	public class SaveCustomerPaymentRequest : IReturn<SaveCustomerPaymentResponse>, IEbSSRequest
	{
		[DataMember(Order = 1)]
		public string Data { get; set; }

		[DataMember(Order = 2)]
		public int RequestMode { get; set; }

		[DataMember(Order = 3)]
		public string TenantAccountId { get; set; }

		public int UserId { get; set; }

		[DataMember(Order = 4)]
		public string UserName { get; set; }
	}

	[DataContract]
	public class SaveCustomerPaymentResponse : IEbSSResponse
	{
		[DataMember(Order = 1)]
		public int Status { get; set; }

		[DataMember(Order = 3)]
		public ResponseStatus ResponseStatus { get; set; }

	}

	[DataContract]
	public class SaveSurgeryDetailsRequest : IReturn<SaveSurgeryDetailsResponse>, IEbSSRequest
	{
		[DataMember(Order = 1)]
		public string Data { get; set; }

		[DataMember(Order = 2)]
		public int RequestMode { get; set; }

		[DataMember(Order = 3)]
		public string TenantAccountId { get; set; }

		public int UserId { get; set; }

		[DataMember(Order = 4)]
		public string UserName { get; set; }
	}

	[DataContract]
	public class SaveSurgeryDetailsResponse : IEbSSResponse
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

	[DataContract]
	public class FeedbackEntry
	{
		[DataMember(Order = 1)]
		public int Id { get; set; }

		[DataMember(Order = 2)]
		public string Date { get; set; }

		[DataMember(Order = 3)]
		public string Status { get; set; }

		[DataMember(Order = 4)]
		public string Followup_Date { get; set; }
		
		[DataMember(Order = 5)]
		public string Comments { get; set; }

		[DataMember(Order = 6)]
		public string Account_Code { get; set; }

		[DataMember(Order = 7)]
		public string Created_By { get; set; }
	}

	[DataContract]
	public class GraftEntry
	{
		[DataMember(Order = 1)]
		public string Id { get; set; }

		[DataMember(Order = 2)]
		public string NoOfGrafts { get; set; }

		[DataMember(Order = 3)]
		public string TotalRate { get; set; }

		[DataMember(Order = 4)]
		public string ConsultingDoctor { get; set; }

		[DataMember(Order = 5)]
		public string ProbableMonth { get; set; }
	}

	[DataContract]
	public class BillingEntry
	{
		[DataMember(Order = 1)]
		public int Id { get; set; }

		[DataMember(Order = 2)]
		public string Date { get; set; }

		[DataMember(Order = 3)]
		public int Total_Amount { get; set; }

		[DataMember(Order = 4)]
		public int Amount_Received { get; set; }

		[DataMember(Order = 5)]
		public int Balance_Amount { get; set; }

		[DataMember(Order = 6)]
		public int Cash_Paid { get; set; }

		[DataMember(Order = 7)]
		public string Payment_Mode { get; set; }

		[DataMember(Order = 8)]
		public string Bank { get; set; }

		[DataMember(Order = 9)]
		public string Clearence_Date { get; set; }

		[DataMember(Order = 10)]
		public string Narration { get; set; }

		[DataMember(Order = 11)]
		public string Account_Code { get; set; }

		[DataMember(Order = 12)]
		public string Created_By { get; set; }
	}

	public class SurgeryEntry
	{
		[DataMember(Order = 1)]
		public int Id { get; set; }

		[DataMember(Order = 2)]
		public string Date { get; set; }

		[DataMember(Order = 3)]
		public string Branch { get; set; }

		[DataMember(Order = 4)]
		public string Account_Code { get; set; }

		[DataMember(Order = 5)]
		public string Created_By { get; set; }

		[DataMember(Order = 5)]
		public string Created_Date { get; set; }

	}
}
