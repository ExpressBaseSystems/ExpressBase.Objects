using ExpressBase.Common.EbServiceStack.ReqNRes;
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

    public class ValidateOtpRequest : EbServiceStackNoAuthRequest, IReturn<Authenticate2FAResponse>
    {
        public string Token { get; set; }

        public string UserAuthId { get; set; }
    }

    public class ResendOTP2FARequest : EbServiceStackAuthRequest, IReturn<Authenticate2FAResponse>
    {
        public string Token { get; set; }
    }

    public class ResendOTPSignInRequest : EbServiceStackNoAuthRequest, IReturn<Authenticate2FAResponse>
    {
        public string Token { get; set; }

        public string UserAuthId { get; set; }

        public string SolnId { get; set; }
    }

    public class SendSignInOtpRequest : EbServiceStackNoAuthRequest, IReturn<Authenticate2FAResponse>
    {
        public SignInOtpType SignInOtpType { get; set; }

        public string UName { get; set; }

        public string SolutionId { get; set; }

        public string WhichConsole { get; set; }
    }

    public class Authenticate2FAResponse
    {
        public string TwoFAToken { get; set; }

        public string ErrorMessage { get; set; }

        public bool AuthStatus { set; get; }

        public bool Is2fa { get; set; }

        public string OtpTo { get; set; }

        public string UserAuthId { get; set; }
    }

    public enum SignInOtpType
    {
        Sms = 1,
        Email = 2
    }
}
