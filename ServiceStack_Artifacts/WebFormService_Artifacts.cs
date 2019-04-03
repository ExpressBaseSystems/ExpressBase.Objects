using ExpressBase.Common;
using ExpressBase.Common.Data;
using ExpressBase.Common.EbServiceStack.ReqNRes;
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
        public WebformData FormData { get; set; }

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
		public ResponseStatus ResponseStatus { get; set; }
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
	}

	[DataContract]
	public class GetAuditTrailResponse : IEbSSResponse
	{
		[DataMember(Order = 1)]
		public Dictionary<int, FormTransaction> Logs { get; set; }

		[DataMember(Order = 2)]
		public ResponseStatus ResponseStatus { get; set; }
	}

	[DataContract]
	public class FormTransaction
	{
		[DataMember(Order = 1)]
		public string CreatedBy;

		[DataMember(Order = 2)]
		public string CreatedAt;

		[DataMember(Order = 3)]
		public List<FormTransactionLine> Details;
	}

	[DataContract]
	public class FormTransactionLine
	{
		[DataMember(Order = 1)]
		public string FieldName;

		[DataMember(Order = 2)]
		public string OldValue;

		[DataMember(Order = 3)]
		public string NewValue;
	}


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

    [Serializable()]
    public class FormException : Exception
    {
        public FormException() : base() { }

        public FormException(string message) : base(message) { }
    }
}
