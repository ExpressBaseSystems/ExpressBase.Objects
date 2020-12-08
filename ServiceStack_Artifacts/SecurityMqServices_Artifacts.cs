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
		public string ColUserIds { get; set; }

	}

	public class SaveUserMqRequest : EbMqRequest
	{
		
		[DataMember(Order = 1)]
		public string SubscriptionId { get; set; }		

	}

}
