using ExpressBase.Common.EbServiceStack.ReqNRes;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace ExpressBase.Objects.ServiceStack_Artifacts
{

	public class SaveRoleMqRequest : EbMqRequest
	{
		
		[DataMember(Order = 1)]
		public string SubscriptionId { get; set; }

		[DataMember(Order = 2)]
		public string NewUserIds { get; set; }

		[DataMember(Order = 3)]
		public List<string> OldUserIds { get; set; } = new List<string>();

		[DataMember(Order = 4)]
		public List<string> Old_Permission { get; set; } = new List<string>();

		[DataMember(Order = 5)]
		public string New_Permission { get; set; }



	}

	public class SaveUserMqRequest : EbMqRequest
	{
		
		[DataMember(Order = 1)]
		public string SubscriptionId { get; set; }		

	}

}
