﻿using ExpressBase.Common.EbServiceStack.ReqNRes;
using ServiceStack;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExpressBase.Objects.ServiceStack_Artifacts
{
    public class Authenticate2FARequest : EbServiceStackAuthRequest, IReturn<Authenticate2FAResponse>
    {
        public MyAuthenticateResponse MyAuthenticateResponse { get; set; }
    }

    public class Validate2FARequest : EbServiceStackAuthRequest, IReturn<Authenticate2FAResponse> 
    {
        public string Token  { get; set; }         
    }

    public class ResendOTP2FARequest : EbServiceStackAuthRequest, IReturn<Authenticate2FAResponse>
    {
        public string Token { get; set; }
    }

    public class Authenticate2FAResponse
    {
        public string TwoFAToken { get; set; }

        public string ErrorMessage { get; set; }

        public bool AuthStatus { set; get; }

        public bool Is2fa { get; set; }

        public string OtpTo { get; set; }

    }
}