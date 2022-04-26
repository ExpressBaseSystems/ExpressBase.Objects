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

    public class ValidateTokenRequest : EbServiceStackNoAuthRequest, IReturn<Authenticate2FAResponse>
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
        public OtpType SignInOtpType { get; set; }

        public string UName { get; set; }

        public string SolutionId { get; set; }

        public string WhichConsole { get; set; }
    }

    public class Authenticate2FAResponse
    {
        public string TwoFAToken { get; set; }

        public string ErrorMessage { get; set; }

        public string Message { get; set; }

        public bool AuthStatus { set; get; }

        public bool Is2fa { get; set; }

        public string OtpTo { get; set; }

        public string UserAuthId { get; set; }

        public Authenticate2FAResponse MobileVerifCode { get; set; }

        public Authenticate2FAResponse EmailVerifCode { get; set; }
    }

    public class SendVerificationCodeRequest : EbServiceStackAuthRequest, IReturn<Authenticate2FAResponse>
    {
        public string Email { get; set; }

        public string Mobile { get; set; }

        public string Key { get; set; }
    }

    public class VerifyVerificationCodeRequest : EbServiceStackAuthRequest, IReturn<Authenticate2FAResponse>
    {
        public string Email { get; set; }

        public string Mobile { get; set; }

        public string Otp { get; set; }

        public string Key { get; set; }
    }

    public class SendUserVerifCodeRequest : EbServiceStackAuthRequest, IReturn<Authenticate2FAResponse>
    {
        public int UserId { get; set; }

        public string WC { get; set; }
    }

    public class SetForgotPWInRedisRequest : EbServiceStackAuthRequest, IReturn<Authenticate2FAResponse>
    {
        public string UName { get; set; }

        public string SolutionId { get; set; }

        public string WhichConsole { get; set; }
    }


    public class VerifyUserConfirmationRequest : EbServiceStackNoAuthRequest, IReturn<Authenticate2FAResponse>
    {
        public string VerificationCode { get; set; }

        public string WC { get; set; }

        public string UserAuthId { get; set; }
    }

    public class GetOTPRequest : EbServiceStackAuthRequest, IReturn<GetOTPResponse>
    {
    }
    public class GetOTPResponse
    {

        public string OTP { get; set; }
    }


    public enum OtpType
    {
        Sms = 1,
        Email = 2
    }
}
