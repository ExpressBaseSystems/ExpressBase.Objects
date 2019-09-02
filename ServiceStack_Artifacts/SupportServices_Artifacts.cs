using ExpressBase.Common.EbServiceStack.ReqNRes;
using ServiceStack;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExpressBase.Objects.ServiceStack_Artifacts
{
	//class SupportServices_Artifacts
	//{
	//}

	public class SaveBugRequest : EbServiceStackAuthRequest, IReturn<SaveBugResponse>
	{
		public string title { get; set; }

		public string description { get; set; }

		public string priority { get; set; }

		public string solutionid { get; set; }

		public string usertype { get; set; }

		public string status { get; set; }

		public string type_b_f { get; set; }

		public string fullname { get; set; }

		public string email { get; set; }



	}
	public class SaveBugResponse
	{
		public int Id { get; set; }
		public string status { get; set; }
	}

	public class TenantSolutionsRequest : EbServiceStackAuthRequest, IReturn<TenantSolutionsResponse>
	{
		public string usertype { get; set; }

	}


	public class TenantSolutionsResponse
	{
		public TenantSolutionsResponse()
		{
			solid = new List<string>();
			solname = new List<string>();
			soldispid= new List<string>();
		}

		public List<string> solid { get; set; }

		public List<string> solname { get; set; }

		public List<string> soldispid { get; set; }
	}

	public class FetchSupportRequest : EbServiceStackAuthRequest, IReturn<FetchSupportResponse>
	{
		public int usrid { get; set; }
	}
	public class FetchSupportResponse
	{
		public FetchSupportResponse()
		{
			supporttkt = new List<SupportTktCls>();
		}
		public List<SupportTktCls> supporttkt { get; set; }
	}


	public class SupportDetailsRequest : EbServiceStackAuthRequest, IReturn<SupportDetailsResponse>
	{
		public string ticketno { get; set; }
	}

	public class SupportDetailsResponse
	{
		public SupportDetailsResponse()
		{
			supporttkt = new List<SupportTktCls>();
		}
		public List<SupportTktCls> supporttkt { get; set; }
	}


	public class SupportTktCls
	{
		public string ticketid { get; set; }

		public string title { get; set; }

		public string description { get; set; }

		public string priority { get; set; }

		public string solutionid { get; set; }

		public string status { get; set; }

		public string assignedto { get; set; }

		public string type_b_f { get; set; }

		public string lstmodified { get; set; }

		public string createdat { get; set; }

		public string remarks { get; set; }
	}

}
