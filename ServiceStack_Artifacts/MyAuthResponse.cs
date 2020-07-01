using System;
using System.Collections.Generic;
using System.Text;
using ServiceStack;
using System.Runtime.Serialization;
using ExpressBase.Security;

namespace ExpressBase.Objects.ServiceStack_Artifacts
{
    [DataContract]
    public class MyAuthenticateResponse : AuthenticateResponse
    {
        [DataMember(Order = 1)]
        public User User { get; set; }
		
		[DataMember(Order = 2)]
		public int AnonId { get; set; }
    }

}
