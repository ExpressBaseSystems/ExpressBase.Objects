using ExpressBase.Common;
using ExpressBase.Common.Data;
using ExpressBase.Common.EbServiceStack.ReqNRes;
using ExpressBase.Common.LocationNSolution;
using ExpressBase.Common.Objects;
using ExpressBase.Security;
using ServiceStack;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace ExpressBase.Objects.ServiceStack_Artifacts
{

	//===================================== FORM TABLE CREATION   ==========================================

	[DataContract]
	public class CreateWebFormTableRequest : EbServiceStackAuthRequest, IReturn<CreateWebFormTableResponse>
	{
		[DataMember(Order = 1)]
		public EbControlContainer WebObj { get; set; }

		[DataMember(Order = 1)]
		public string Apps { get; set; }

        [DataMember(Order = 1)]
        public bool IsImport { get; set; }
	}

	[DataContract]
	public class CreateWebFormTableResponse : IEbSSResponse
	{
		[DataMember(Order = 1)]
		public ResponseStatus ResponseStatus { get; set; }
	}

	//================================== GET RECORD FOR RENDERING ================================================

	[DataContract]
	public class GetRowDataRequest : EbServiceStackAuthRequest, IReturn<GetRowDataResponse>
	{
		[DataMember(Order = 1)]
		public string RefId { get; set; }

		[DataMember(Order = 2)]
		public int RowId { get; set; }
        
        [DataMember(Order = 3)]
        public User UserObj { get; set; }
    }

	[DataContract]
	public class GetRowDataResponse : IEbSSResponse
	{
		[DataMember(Order = 1)]
		public WebformData FormData { get; set; }

		[DataMember(Order = 2)]
		public ResponseStatus ResponseStatus { get; set; }
	}

    [DataContract]
    public class GetPrefillDataRequest : EbServiceStackAuthRequest, IReturn<GetPrefillDataResponse>
    {
        [DataMember(Order = 1)]
        public string RefId { get; set; }

        [DataMember(Order = 2)]
        public List<Param> Params { get; set; }
    }

    [DataContract]
    public class GetPrefillDataResponse : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public WebformDataWrapper FormData { get; set; }

        [DataMember(Order = 2)]
        public ResponseStatus ResponseStatus { get; set; }
    }

    [DataContract]
    public class GetImportDataRequest : EbServiceStackAuthRequest, IReturn<GetImportDataResponse>
    {
        [DataMember(Order = 1)]
        public string RefId { get; set; }

        [DataMember(Order = 2)]
        public List<Param> Params { get; set; }

        [DataMember(Order = 3)]
        public string Trigger { get; set; }
    }

    [DataContract]
    public class GetImportDataResponse : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public WebformData FormData { get; set; }

        [DataMember(Order = 3)]
        public WebformDataWrapper FormDataWrap { get; set; }

        [DataMember(Order = 2)]
        public ResponseStatus ResponseStatus { get; set; }
    }

    [DataContract]
    public class ExecuteSqlValueExprRequest : EbServiceStackAuthRequest, IReturn<ExecuteSqlValueExprResponse>
    {
        [DataMember(Order = 1)]
        public string RefId { get; set; }

        [DataMember(Order = 2)]
        public List<Param> Params { get; set; }

        [DataMember(Order = 3)]
        public string Trigger { get; set; }
    }

    [DataContract]
    public class ExecuteSqlValueExprResponse : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public string Data { get; set; }
        
        [DataMember(Order = 2)]
        public ResponseStatus ResponseStatus { get; set; }
    }

    [DataContract]
    public class GetDataPusherJsonRequest : EbServiceStackAuthRequest, IReturn<GetDataPusherJsonResponse>
    {
        [DataMember(Order = 1)]
        public string RefId { get; set; }
    }

    [DataContract]
    public class GetDataPusherJsonResponse : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public string Json { get; set; }
        
        [DataMember(Order = 2)]
        public ResponseStatus ResponseStatus { get; set; }
    }


    [DataContract]
	public class DoUniqueCheckRequest : EbServiceStackAuthRequest, IReturn<GetRowDataResponse>
	{
		[DataMember(Order = 1)]
		public string TableName { get; set; }

		[DataMember(Order = 2)]
		public string Field { get; set; }

		[DataMember(Order = 3)]
		public string Value { get; set; }

		[DataMember(Order = 4)]
		public string TypeS { get; set; }
	}

	[DataContract]
	public class DoUniqueCheckResponse : IEbSSResponse
	{
		[DataMember(Order = 1)]
		public int NoRowsWithSameValue { get; set; }

		[DataMember(Order = 2)]
		public ResponseStatus ResponseStatus { get; set; }
	}

	[DataContract]
	public class GetDictionaryValueRequest : EbServiceStackAuthRequest, IReturn<GetDictionaryValueResponse>
	{
		[DataMember(Order = 1)]
		public string[] Keys { get; set; }

		[DataMember(Order = 2)]
		public string Locale { get; set; }
	}

	[DataContract]
	public class GetDictionaryValueResponse : IEbSSResponse
	{
		[DataMember(Order = 1)]
		public Dictionary<string, string> Dict { get; set; }

		[DataMember(Order = 2)]
		public ResponseStatus ResponseStatus { get; set; }
	}


	//======================================= INSERT OR UPDATE OR DELETE RECORD =============================================

	[DataContract]
	public class InsertDataFromWebformRequest : EbServiceStackAuthRequest, IReturn<InsertDataFromWebformResponse>
	{
		[DataMember(Order = 1)]
		public string TableName { get; set; }

		[DataMember(Order = 2)]
		public WebformData FormData { get; set; }

		[DataMember(Order = 3)]
		public string RefId { get; set; }

		[DataMember(Order = 4)]
		public int RowId { get; set; }

        [DataMember(Order = 5)]
        public int CurrentLoc { get; set; }

        [DataMember(Order = 6)]
        public User UserObj { get; set; }
    }
	
	[DataContract]
	public class InsertDataFromWebformResponse : IEbSSResponse
	{
		[DataMember(Order = 1)]
		public int RowAffected { get; set; }

        [DataMember(Order = 2)]
        public int RowId { get; set; }

        [DataMember(Order = 3)]
        public WebformData FormData { get; set; }

        [DataMember(Order = 4)]
        public int AfterSaveStatus { get; set; }

        [DataMember(Order = 5)]
		public ResponseStatus ResponseStatus { get; set; }

        [DataMember(Order = 6)]
        public int Status { get; set; }

        [DataMember(Order = 7)]
        public string Message { get; set; }

        [DataMember(Order = 8)]
        public string MessageInt { get; set; }

        [DataMember(Order = 9)]
        public string StackTraceInt { get; set; }
    }

    [DataContract]
    public class DeleteDataFromWebformRequest : EbServiceStackAuthRequest, IReturn<DeleteDataFromWebformResponse>
    {
        [DataMember(Order = 1)]
        public string RefId { get; set; }

        [DataMember(Order = 2)]
        public int RowId { get; set; }

        [DataMember(Order = 3)]
        public User UserObj { get; set; }
    }

    [DataContract]
    public class DeleteDataFromWebformResponse : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public int RowAffected { get; set; }

        [DataMember(Order = 2)]
        public ResponseStatus ResponseStatus { get; set; }
    }

    [DataContract]
    public class CancelDataFromWebformRequest : EbServiceStackAuthRequest, IReturn<CancelDataFromWebformResponse>
    {
        [DataMember(Order = 1)]
        public string RefId { get; set; }

        [DataMember(Order = 2)]
        public int RowId { get; set; }
        
        [DataMember(Order = 3)]
        public User UserObj { get; set; }
    }

    [DataContract]
    public class CancelDataFromWebformResponse : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public int RowAffected { get; set; }

        [DataMember(Order = 2)]
        public ResponseStatus ResponseStatus { get; set; }
    }



    //=============================================== AUDIT TRAIL ====================================================

    [DataContract]
	public class GetAuditTrailRequest : EbServiceStackAuthRequest, IReturn<GetAuditTrailResponse>
	{
		[DataMember(Order = 1)]
		public string FormId { get; set; }

		[DataMember(Order = 2)]
		public int RowId { get; set; }

        [DataMember(Order = 3)]
        public User UserObj { get; set; }
    }

	[DataContract]
	public class GetAuditTrailResponse : IEbSSResponse
	{
		[DataMember(Order = 1)]
		public string Json { get; set; }

		[DataMember(Order = 2)]
		public ResponseStatus ResponseStatus { get; set; }
	}

	//[DataContract]
	//public class FormTransaction
	//{
	//	[DataMember(Order = 1)]
	//	public string CreatedBy;

	//	[DataMember(Order = 2)]
	//	public string CreatedAt;

	//	[DataMember(Order = 3)]
	//	public List<FormTransactionLine> Details;
	//}

	//[DataContract]
	//public class FormTransactionLine
	//{
	//	[DataMember(Order = 1)]
	//	public string FieldName;

	//	[DataMember(Order = 2)]
	//	public string OldValue;

	//	[DataMember(Order = 3)]
	//	public string NewValue;
	//}


    //=============================================== MISCELLANEOUS ====================================================

    [DataContract]
    public class GetDesignHtmlRequest : EbServiceStackAuthRequest, IReturn<GetDesignHtmlResponse>
    {
        [DataMember(Order = 1)]
        public string RefId { get; set; }
    }

    [DataContract]
    public class GetDesignHtmlResponse : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public string Html { get; set; }

        [DataMember(Order = 2)]
        public ResponseStatus ResponseStatus { get; set; }
    }
    
    [DataContract]
    public class GetCtrlsFlatRequest : EbServiceStackAuthRequest, IReturn<GetCtrlsFlatResponse>
    {
        [DataMember(Order = 1)]
        public string RefId { get; set; }
    }
    
    [DataContract]
    public class GetCtrlsFlatResponse : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public List<EbControl> Controls { get; set; }

        [DataMember(Order = 2)]
        public ResponseStatus ResponseStatus { get; set; }
    }
    
    [DataContract]
    public class CheckEmailConAvailableRequest : EbServiceStackAuthRequest, IReturn<CheckEmailConAvailableResponse>
    {
    }

    [DataContract]
    public class CheckEmailConAvailableResponse : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public bool ConnectionAvailable { get; set; }

        [DataMember(Order = 2)]
        public ResponseStatus ResponseStatus { get; set; }
    }

    [DataContract]
    public class GetDashBoardUserCtrlRequest : EbServiceStackAuthRequest, IReturn<GetDashBoardUserCtrlResponse>
    {
        [DataMember(Order = 1)]
        public string RefId { get; set; }

    }

    [DataContract]
    public class GetDashBoardUserCtrlResponse : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public string UcObjJson { get; set; }

        [DataMember(Order = 1)]
        public string UcHtml { get; set; }

        [DataMember(Order = 2)]
        public ResponseStatus ResponseStatus { get; set; }
    }

    [DataContract]
    public class GetDistinctValuesRequest : EbServiceStackAuthRequest, IReturn<GetDistinctValuesResponse>
    {
        [DataMember(Order = 1)]
        public string TableName { get; set; }  
        
        [DataMember(Order = 2)]
        public string ColumnName { get; set; }

    }

    [DataContract]
    public class GetDistinctValuesResponse : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public List<string> Suggestions { get; set; }

        [DataMember(Order = 2)]
        public ResponseStatus ResponseStatus { get; set; }
    }



    [Serializable()]
    public class FormException : Exception
    {
        public FormException() : base() { }

        public FormException(string message) : base(message) { }

        public FormException(string message, int code) : base(message) 
        {
            this.ExceptionCode = code;
        }

        public FormException(string message, int code, string msg, string stacktrace) : base(message) 
        {
            this.ExceptionCode = code;
            this.MessageInternal = msg;
            this.StackTraceInternal = stacktrace;
        }

        public int ExceptionCode { get; set; }

        public string MessageInternal { get; set; }

        public string StackTraceInternal { get; set; }
    }
}
