using ExpressBase.Common;
using ExpressBase.Common.Data;
using ExpressBase.Common.EbServiceStack.ReqNRes;
using ExpressBase.Common.LocationNSolution;
using ExpressBase.Common.Objects;
using ExpressBase.Objects.Objects;
using ExpressBase.Security;
using ServiceStack;
using System;
using System.Collections.Generic;
using System.Data.Common;
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

		[DataMember(Order = 2)]
		public string Apps { get; set; }

        [DataMember(Order = 3)]
        public bool IsImport { get; set; }

        [DataMember(Order = 4)]
        public bool DontThrowException { get; set; }
	}

	[DataContract]
	public class CreateWebFormTableResponse : IEbSSResponse
	{
		[DataMember(Order = 1)]
		public ResponseStatus ResponseStatus { get; set; }
	}

    [DataContract]
    public class CreateMyProfileTableRequest : EbServiceStackAuthRequest, IReturn<CreateMyProfileTableResponse>
    {
        [DataMember(Order = 1)]
        public List<EbProfileUserType> UserTypeForms { get; set; }
    }

    [DataContract]
    public class CreateMyProfileTableResponse : IEbSSResponse
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
        public int CurrentLoc { get; set; }

        [DataMember(Order = 4)]
        public WebFormRenderModes RenderMode { get; set; }
    }

	[DataContract]
	public class GetRowDataResponse : IEbSSResponse
	{
		[DataMember(Order = 1)]
		public string FormDataWrap { get; set; }

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
        
        [DataMember(Order = 3)]
        public int CurrentLoc { get; set; }

        [DataMember(Order = 4)]
        public WebFormRenderModes RenderMode { get; set; }
    }

    [DataContract]
    public class GetPrefillDataResponse : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public string FormDataWrap { get; set; }

        [DataMember(Order = 2)]
        public ResponseStatus ResponseStatus { get; set; }
    }

    [DataContract]
    public class GetExportFormDataRequest : EbServiceStackAuthRequest, IReturn<GetExportFormDataResponse>
    {
        [DataMember(Order = 1)]
        public string SourceRefId { get; set; }

        [DataMember(Order = 2)]
        public int SourceRowId { get; set; }

        [DataMember(Order = 3)]
        public User UserObj { get; set; }

        [DataMember(Order = 4)]
        public int CurrentLoc { get; set; }

        [DataMember(Order = 5)]
        public WebFormRenderModes RenderMode { get; set; }

        [DataMember(Order = 6)]
        public string DestRefId { get; set; }
    }

    [DataContract]
    public class GetExportFormDataResponse : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public string FormDataWrap { get; set; }

        [DataMember(Order = 2)]
        public ResponseStatus ResponseStatus { get; set; }
    }

    [DataContract]
    public class GetFormData4MobileRequest : EbServiceStackAuthRequest, IReturn<GetFormData4MobileResponse>
    {
        [DataMember(Order = 1)]
        public string RefId { get; set; }
        
        [DataMember(Order = 2)]
        public int DataId { get; set; }
    }

    [DataContract]
    public class GetFormData4MobileResponse : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public List<Param> Params { get; set; }
        
        [DataMember(Order = 2)]
        public int Status { get; set; }

        [DataMember(Order = 3)]
        public string Message { get; set; }

        [DataMember(Order = 4)]
        public ResponseStatus ResponseStatus { get; set; }
    }

    [DataContract]
    public class GetImportDataRequest : EbServiceStackAuthRequest, IReturn<GetImportDataResponse>
    {
        [DataMember(Order = 1)]
        public string RefId { get; set; }

        [DataMember(Order = 2)]
        public int RowId { get; set; }

        [DataMember(Order = 3)]
        public List<Param> Params { get; set; }

        [DataMember(Order = 4)]
        public string Trigger { get; set; }
        
        [DataMember(Order = 5)]
        public string WebFormData { get; set; }

        [DataMember(Order = 6)]
        public ImportDataType Type { get; set; }
    }

    public enum ImportDataType
    {
        PowerSelect = 1,
        DataGrid
    }

    [DataContract]
    public class GetImportDataResponse : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public string FormDataWrap { get; set; }

        [DataMember(Order = 2)]
        public ResponseStatus ResponseStatus { get; set; }
    }

    [DataContract]
    public class GetDynamicGridDataRequest : EbServiceStackAuthRequest, IReturn<GetDynamicGridDataResponse>
    {
        [DataMember(Order = 1)]
        public string RefId { get; set; }

        [DataMember(Order = 2)]
        public int RowId { get; set; }

        [DataMember(Order = 3)]
        public string SourceId { get; set; }

        [DataMember(Order = 4)]
        public string[] Target { get; set; }
    }

    [DataContract]
    public class GetDynamicGridDataResponse : IEbSSResponse
    {
        [DataMember(Order = 3)]
        public string FormDataWrap { get; set; }

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
        public UniqCheckParam[] UniqCheckParam { get; set; }
    }


    [DataContract]
	public class UniqCheckParam
	{
		[DataMember(Order = 1)]
		public string TableName { get; set; }

		[DataMember(Order = 2)]
		public string Field { get; set; }

		[DataMember(Order = 3)]
		public string Value { get; set; }

		[DataMember(Order = 4)]
		public int TypeI { get; set; }
	}

	[DataContract]
	public class DoUniqueCheckResponse : IEbSSResponse
	{
		[DataMember(Order = 1)]
		public Dictionary<string, bool> Response { get; set; }

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
		public WebformData FormData { get; set; }

		[DataMember(Order = 2)]
		public string RefId { get; set; }

		[DataMember(Order = 3)]
		public int RowId { get; set; }

        [DataMember(Order = 4)]
        public int CurrentLoc { get; set; }
        
        [DataMember(Order = 5)]
        public int DraftId { get; set; }
    }
	
	[DataContract]
	public class InsertDataFromWebformResponse : IEbSSResponse
	{
		[DataMember(Order = 1)]
		public int RowAffected { get; set; }

        [DataMember(Order = 2)]
        public int RowId { get; set; }

        [DataMember(Order = 3)]
        //public WebformData FormData { get; set; }
        public string FormData { get; set; }
        
        [DataMember(Order = 4)]
		public ResponseStatus ResponseStatus { get; set; }

        [DataMember(Order = 5)]
        public int Status { get; set; }

        [DataMember(Order = 6)]
        public string Message { get; set; }

        [DataMember(Order = 7)]
        public string MessageInt { get; set; }

        [DataMember(Order = 8)]
        public string StackTraceInt { get; set; }
        
        [DataMember(Order = 9)]
        public string AffectedEntries { get; set; }

        [DataMember(Order = 9)]
        public Dictionary<string, string> MetaData { get; set; }
    }
    
	[DataContract]
	public class InsertOrUpdateFormDataRqst : EbServiceStackAuthRequest, IReturn<InsertOrUpdateFormDataResp>
	{
		[DataMember(Order = 1)]
		public FormGlobals FormGlobals { get; set; }

        [DataMember(Order = 2)]
		public string PushJson { get; set; }

		[DataMember(Order = 3)]
		public string RefId { get; set; }

		[DataMember(Order = 4)]
		public int RecordId { get; set; }

        [DataMember(Order = 5)]
        public int LocId { get; set; }

        [DataMember(Order = 6) ]
        public DbConnection TransactionConnection { get; set; }
    }
	
	[DataContract]
	public class InsertOrUpdateFormDataResp : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public int Status { get; set; }

        [DataMember(Order = 2)]
        public string Message { get; set; }

        [DataMember(Order = 3)]
        public int RecordId { get; set; }

        [DataMember(Order = 4)]
		public ResponseStatus ResponseStatus { get; set; }
    }

    [DataContract]
    public class InsertBatchDataRequest : EbServiceStackAuthRequest, IReturn<InsertBatchDataResponse>
    {
        [DataMember(Order = 1)]
        public string RefId { get; set; }
        
        [DataMember(Order = 2)]
        public int LocId { get; set; }

        [DataMember(Order = 3)]
        public EbDataTable Data { get; set; }

        [DataMember(Order = 4)]
        public DbConnection TransactionConnection { get; set; }
    }

    [DataContract]
    public class InsertBatchDataResponse : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public int Status { get; set; }

        [DataMember(Order = 2)]
        public string Message { get; set; }

        [DataMember(Order = 3)]
        public List<int> RecordIds { get; set; }

        [DataMember(Order = 4)]
        public ResponseStatus ResponseStatus { get; set; }
    }

    [DataContract]
    public class SaveFormDraftRequest : EbServiceStackAuthRequest, IReturn<SaveFormDraftResponse>
    {
        [DataMember(Order = 1)]
        public string RefId { get; set; }

        [DataMember(Order = 2)]
        public int LocId { get; set; }

        [DataMember(Order = 3)]
        public string Data { get; set; }

        [DataMember(Order = 4)]
        public int DraftId { get; set; }
        
        [DataMember(Order = 5)]
        public string Title { get; set; }
    }

    [DataContract]
    public class SaveFormDraftResponse : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public int Status { get; set; }

        [DataMember(Order = 2)]
        public string Message { get; set; }

        [DataMember(Order = 3)]
        public int DraftId { get; set; }

        [DataMember(Order = 4)]
        public string MessageInt { get; set; }

        [DataMember(Order = 5)]
        public string StackTraceInt { get; set; }

        [DataMember(Order = 6)]
        public ResponseStatus ResponseStatus { get; set; }
    }

    [DataContract]
    public class DiscardFormDraftRequest : EbServiceStackAuthRequest, IReturn<DiscardFormDraftResponse>
    {
        [DataMember(Order = 1)]
        public string RefId { get; set; }

        [DataMember(Order = 2)]
        public int DraftId { get; set; }
    }

    [DataContract]
    public class DiscardFormDraftResponse : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public int Status { get; set; }

        [DataMember(Order = 2)]
        public string Message { get; set; }

        [DataMember(Order = 3)]
        public string MessageInt { get; set; }

        [DataMember(Order = 4)]
        public string StackTraceInt { get; set; }

        [DataMember(Order = 5)]
        public ResponseStatus ResponseStatus { get; set; }
    }

    [DataContract]
    public class GetFormDraftRequest : EbServiceStackAuthRequest, IReturn<GetFormDraftResponse>
    {
        [DataMember(Order = 1)]
        public string RefId { get; set; }

        [DataMember(Order = 2)]
        public int DraftId { get; set; }
    }

    [DataContract]
    public class GetFormDraftResponse : IEbSSResponse
    {
        
        [DataMember(Order = 1)]
        public string DataWrapper { get; set; }

        [DataMember(Order = 2)]
        public ResponseStatus ResponseStatus { get; set; }

        [DataMember(Order = 3)]
        public string FormDatajson{ get; set; }
    }

    [DataContract]
    public class CheckEmailAndPhoneRequest : EbServiceStackAuthRequest, IReturn<CheckEmailAndPhoneResponse>
    {
        [DataMember(Order = 1)]
        public string RefId { get; set; }

        [DataMember(Order = 2)]
        public int RowId { get; set; }

        [DataMember(Order = 3)]
        public string Data { get; set; }
    }

    [DataContract]
    public class CheckEmailAndPhoneResponse : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public string Data { get; set; }

        [DataMember(Order = 2)]
        public ResponseStatus ResponseStatus { get; set; }
    }

    [DataContract]
    public class GetProvUserListRequest : EbServiceStackAuthRequest, IReturn<GetProvUserListResponse>
    {
        [DataMember(Order = 1)]
        public string RefId { get; set; }

        [DataMember(Order = 2)]
        public int RowId { get; set; }
    }

    [DataContract]
    public class GetProvUserListResponse : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public string Data { get; set; }

        [DataMember(Order = 2)]
        public ResponseStatus ResponseStatus { get; set; }
    }

    [DataContract]
    public class GetGlobalSrchRsltsReq : EbServiceStackAuthRequest, IReturn<GetGlobalSrchRsltsResp>
    {
        [DataMember(Order = 1)]
        public string SrchText { get; set; }
    }

    [DataContract]
    public class GetGlobalSrchRsltsResp : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public string Data { get; set; }

        [DataMember(Order = 2)]
        public ResponseStatus ResponseStatus { get; set; }
    }

    [DataContract]
    public class UpdateIndexesRequest : EbServiceStackAuthRequest, IReturn<UpdateIndexesRespone>
    {
        [DataMember(Order = 1)]
        public string RefId { get; set; }
    }

    [DataContract]
    public class UpdateIndexesRespone : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public string Message { get; set; }

        [DataMember(Order = 2)]
        public ResponseStatus ResponseStatus { get; set; }
    }

    [DataContract]
    public class DeleteDataFromWebformRequest : EbServiceStackAuthRequest, IReturn<DeleteDataFromWebformResponse>
    {
        [DataMember(Order = 1)]
        public string RefId { get; set; }

        [DataMember(Order = 2)]
        public List<int> RowId { get; set; }

        [DataMember(Order = 3)]
        public DbConnection TransactionConnection { get; set; }
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
    public class UpdateAllFormTablesRequest : EbServiceStackAuthRequest, IReturn<UpdateAllFormTablesResponse>
    {
        [DataMember(Order = 1)]
        public string InMsg { get; set; }
    }
    
    [DataContract]
    public class UpdateAllFormTablesResponse : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public string Message { get; set; }

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

         [DataMember(Order = 2)]
        public List<Param> Param { get; set; }



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


    public class GetMyProfileEntryRequest : EbServiceStackAuthRequest, IReturn<GetMyProfileEntryResponse>
    {
        public string TableName { get; set; }
    }

    public class GetMyProfileEntryResponse:IEbSSResponse
    {
        [DataMember(Order = 1)]
        public int RowId { get; set; }

        [DataMember(Order = 2)]
        public ResponseStatus ResponseStatus { get; set; }
    }

    [DataContract]
    public class GetAllRolesRequest : EbServiceStackAuthRequest, IReturn<GetAllRolesResponse>
    {

    }

    [DataContract]
    public class GetAllRolesResponse : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public Dictionary<int, string> Roles { get; set; }

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
