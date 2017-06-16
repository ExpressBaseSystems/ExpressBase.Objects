using ExpressBase.Common;
using ExpressBase.Security;
using ServiceStack;
using ServiceStack.Auth;
using ServiceStack.Configuration;
using System;
using System.Collections.Generic;
using ServiceStack.Web;
using System.IO;
using ExpressBase.Data;
using ServiceStack.Logging;
using System.Runtime.Serialization;
using ExpressBase.Objects.ServiceStack_Artifacts;
using System.Globalization;

namespace ExpressBase.Objects.ServiceStack_Artifacts
{
    [DataContract]
    public class MyAuthenticateResponse : AuthenticateResponse
    {
        [DataMember(Order = 1)]
        public User User { get; set; }
    }

    [DataContract]
    public class CustomUserSession : AuthUserSession
    {
        [DataMember(Order = 1)]
        public string CId { get; set; }

        [DataMember(Order = 2)]
        public int Uid { get; set; }

        [DataMember(Order = 3)]
        public User User { get; set; }

        //[DataMember(Order = 3)]
        //public string BearerToken { get; set; }

        //[DataMember(Order = 4)]
        //public string RefreshToken { get; set; }

        public override bool IsAuthorized(string provider)
        {
            return true;
        }
    }

    public class MyCredentialsAuthProvider: CredentialsAuthProvider
    {
        public MyCredentialsAuthProvider(IAppSettings settings) : base(settings) { }

        public override bool TryAuthenticate(IServiceBase authService, string cidAndUserName, string password)
        {
            var redisClient = (authService as AuthenticateService).Redis;
            User _authUser = null;
            ILog log = LogManager.GetLogger(GetType());

            var arrTemp = cidAndUserName.Split('/');
            var cid = arrTemp[0];
            var userName = arrTemp[1];

            EbBaseService bservice = new EbBaseService();

            if (cid == "expressbase")
            {
                string path = Directory.GetParent(System.IO.Directory.GetCurrentDirectory()).FullName;
                var infraconf = EbSerializers.ProtoBuf_DeSerialize<EbInfraDBConf>(EbFile.Bytea_FromFile(Path.Combine(path, "EbInfra.conn")));
                var df = new DatabaseFactory(infraconf);
                _authUser = InfraUser.GetDetails(df, userName, password);
                log.Info("#Eb reached 1");
            }
            else
            {
                bservice.ClientID = cid;
                _authUser = User.GetDetails(bservice.DatabaseFactory, userName, password);
                log.Info("#Eb reached 2");
            }

            if (_authUser != null)
            {
                CustomUserSession session = authService.GetSession(false) as CustomUserSession;
                session.Company = cid;
                session.UserAuthId = _authUser.Id.ToString();
                session.UserName = userName;
                session.IsAuthenticated = true;
                session.User = _authUser;
            }

            return (_authUser != null);
        }

        public override object Authenticate(IServiceBase authService, IAuthSession session, Authenticate request)
        {
            AuthenticateResponse authResponse =  base.Authenticate(authService, session, request) as AuthenticateResponse;

            var _customUserSession = authService.GetSession() as CustomUserSession;

            if (!string.IsNullOrEmpty(authResponse.SessionId) && _customUserSession != null)
            {
                var x = new MyAuthenticateResponse
                {
                    UserId = _customUserSession.UserAuthId,
                    UserName = _customUserSession.UserName,
                    User = _customUserSession.User,
                };

                return x;
            }

            return authResponse;
        }

        public override object Logout(IServiceBase service, Authenticate request)
        {
            return base.Logout(service, request);
        }
    }
}
