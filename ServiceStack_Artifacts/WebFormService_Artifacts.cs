using ExpressBase.Common.EbServiceStack.ReqNRes;
using ExpressBase.Common.Objects;
using ServiceStack;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace ExpressBase.Objects.ServiceStack_Artifacts
{
	
	//===================================== TABLE CREATION  ==========================================
	//[Route("/bots")]
	[DataContract]
	public class CreateWebFormTableRequest : EbServiceStackAuthRequest, IReturn<CreateWebFormTableResponse>
	{
		[DataMember(Order = 1)]
		public EbControlContainer WebObj { get; set; }

		[DataMember(Order = 1)]
		public string Apps { get; set; }


		//public string  TableName { get; set; }

		//[DataMember(Order = 2)]
		//public Dictionary<string, string> Fields { get; set; }
	}

	[DataContract]
	public class CreateWebFormTableResponse : IEbSSResponse
	{
		[DataMember(Order = 3)]
		public ResponseStatus ResponseStatus { get; set; }
	}

	//================================== GET PARTICULAR RECORD ================================================
	[DataContract]
	public class GetRowDataRequest : EbServiceStackAuthRequest
	{
		[DataMember(Order = 1)]
		public string RefId { get; set; }

		[DataMember(Order = 2)]
		public int RowId { get; set; }
	}

	//======================================= SAVE OR UPDATE RECORD =============================================


}
