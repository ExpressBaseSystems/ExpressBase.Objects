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
		public string SolnId { get; set; }

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

		[DataMember(Order = 11)]
		public List<string> CrntCityList { get; set; }

		[DataMember(Order = 12)]
		public List<string> CrntCountryList { get; set; }

		[DataMember(Order = 13)]
		public List<string> CityList { get; set; }

		[DataMember(Order = 14)]
		public List<string> DistrictList { get; set; }

		[DataMember(Order = 15)]
		public List<string> SourceCategoryList { get; set; }

		[DataMember(Order = 16)]
		public List<string> SubCategoryList { get; set; }

		//[DataMember(Order = 17)]
		//public List<string> ImageIdList { get; set; }

		[DataMember(Order = 18)]
		public Dictionary<string, string> StatusDict { get; set; }

		[DataMember(Order = 19)]
		public List<string> ServiceList { get; set; }

		[DataMember(Order = 20)]
		public int RespMode { get; set; }

		[DataMember(Order = 21)]
		public Dictionary<string, int> NurseDict { get; set; }

	}

	[DataContract]
	public class SaveCustomerRequest : IReturn<SaveCustomerResponse>, IEbSSRequest
	{
		[DataMember(Order = 1)]
		public string CustomerData { get; set; }

		[DataMember(Order = 2)]
		public int RequestMode { get; set; }

		[DataMember(Order = 3)]
		public string SolnId { get; set; }

		public int UserId { get; set; }

        [DataMember(Order = 4)]
        public string ImgRefId { get; set; }

        [DataMember(Order = 5)]
		public string UserName { get; set; }
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
    public class GetImageInfoRequest : IReturn<GetImageInfoResponse>, IEbSSRequest
    {
        [DataMember(Order = 1)]
        public int CustomerId { get; set; }

        [DataMember(Order = 2)]
        public string SolnId { get; set; }

        public int UserId { get; set; }
    }

    [DataContract]
    public class GetImageInfoResponse : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public List<FileMetaInfo> Data { get; set; }

        [DataMember(Order = 3)]
        public ResponseStatus ResponseStatus { get; set; }
    }

    public class FileMetaInfo
    {
        public string FileName { get; set; }

        public int FileSize { get; set; }

        public int FileRefId { get; set; }

        public Dictionary<string,List<string>> Meta { set; get; }

        public string UploadTime { get; set; }
    }

    [DataContract]
	public class SaveCustomerFollowupRequest : IReturn<SaveCustomerFollowupResponse>, IEbSSRequest
	{
		[DataMember(Order = 1)]
		public string Data { get; set; }

        [DataMember(Order = 5)]
        public bool Permission { get; set; }

        [DataMember(Order = 2)]
		public string SolnId { get; set; }

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
		public string SolnId { get; set; }

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
		public string SolnId { get; set; }

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
		public string SolnId { get; set; }

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
	public class LmUniqueCheckRequest : IReturn<LmUniqueCheckResponse>, IEbSSRequest
	{
		[DataMember(Order = 1)]
		public string Key { get; set; }

		[DataMember(Order = 2)]
		public string Value { get; set; }

		[DataMember(Order = 3)]
		public string SolnId { get; set; }

		public int UserId { get; set; }
	}

	[DataContract]
	public class LmUniqueCheckResponse : IEbSSResponse
	{
		[DataMember(Order = 1)]
		public bool Status { get; set; }

		[DataMember(Order = 2)]
		public ResponseStatus ResponseStatus { get; set; }
	}

    [DataContract]
    public class LmDeleteImageRequest : IReturn<LmDeleteImageResponse>, IEbSSRequest
    {
        [DataMember(Order = 1)]
        public int CustId { get; set; }

        [DataMember(Order = 2)]
        public string ImgRefIds { get; set; }

        [DataMember(Order = 3)]
        public string SolnId { get; set; }

        public int UserId { get; set; }
    }

    [DataContract]
    public class LmDeleteImageResponse : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public int RowsAffected { get; set; }

        [DataMember(Order = 2)]
        public ResponseStatus ResponseStatus { get; set; }
    }

    [DataContract]
    public class LmDeleteCustomerRequest : IReturn<LmDeleteCustomerResponse>, IEbSSRequest
    {
        [DataMember(Order = 1)]
        public int CustId { get; set; }

        [DataMember(Order = 2)]
        public string SolnId { get; set; }

        public int UserId { get; set; }
    }

    [DataContract]
    public class LmDeleteCustomerResponse : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public bool Status { get; set; }

        [DataMember(Order = 2)]
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
		public string Fup_Date { get; set; }
		
		[DataMember(Order = 5)]
		public string Comments { get; set; }

		[DataMember(Order = 6)]
		public string Account_Code { get; set; }

		[DataMember(Order = 7)]
		public string Created_By { get; set; }

		[DataMember(Order = 8)]
		public string Created_Date { get; set; }
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
		public int Account_Code { get; set; }

		[DataMember(Order = 12)]
		public string Created_By { get; set; }

		[DataMember(Order = 13)]
		public bool PDC { get; set; }//Post Dated Cheque
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
		public int Account_Code { get; set; }

		[DataMember(Order = 5)]
		public string Created_By { get; set; }

		[DataMember(Order = 6)]
		public string Created_Date { get; set; }

		[DataMember(Order = 7)]
		public int Extract_By { get; set; }

		[DataMember(Order = 8)]
		public int Implant_By { get; set; }

		[DataMember(Order = 9)]
		public int Consent_By { get; set; }

		[DataMember(Order = 10)]
		public int Anaesthesia_By { get; set; }

		[DataMember(Order = 11)]
		public int Post_Brief_By { get; set; }

		[DataMember(Order = 12)]
		public int Nurse { get; set; }
        
        [DataMember(Order = 13)]
        public string Complimentary { get; set; }

        [DataMember(Order = 14)]
        public string Method { get; set; }
    }
}
