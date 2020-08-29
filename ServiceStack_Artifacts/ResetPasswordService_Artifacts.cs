using ExpressBase.Common.EbServiceStack.ReqNRes;
using ServiceStack;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExpressBase.Objects.ServiceStack_Artifacts
{
     
    public class ResetPwRequest : EbServiceStackNoAuthRequest, IReturn<ResetPwResponse>
    {
        public string Email { get; set; }

        public PwDetails PwDetails { get; set; }

        public int UserId { get; set; }

        public string SolnId { get; set; }
    }

    public class ResetPwResponse
    {
        public bool Status { get; set; }
        public string ErrorMessage { get; set; }
    }

    public class PwDetails
    {
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
    }
}
