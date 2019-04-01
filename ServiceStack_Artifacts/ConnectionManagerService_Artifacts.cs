using ExpressBase.Common.Connections;
using ExpressBase.Common.Data;
using ExpressBase.Common.EbServiceStack.ReqNRes;
using ExpressBase.Common.Messaging;
using ServiceStack;
using System.Runtime.Serialization;

namespace ExpressBase.Objects.ServiceStack_Artifacts
{
    public class RefreshSolutionConnectionsRequest : EbMqRequest
    {
    }

    public class RefreshSolutionConnectionsResponse : IEbSSResponse
    {
        public ResponseStatus ResponseStatus { get; set; }
    }

    public class RefreshSolutionConnectionsAsyncRequest : EbServiceStackAuthRequest, IReturn<RefreshSolutionConnectionsAsyncResponse>
    {
        public string SolutionId { get; set; }
    }

    public class RefreshSolutionConnectionsAsyncResponse : IEbSSResponse
    {
        public ResponseStatus ResponseStatus { get; set; }
    }

    public class RefreshSolutionConnectionsBySolutionIdAsyncRequest : EbServiceStackAuthRequest, IReturn<RefreshSolutionConnectionsAsyncResponse>
    {
        public string SolutionId { get; set; }
    }

    public class GetConnectionsRequest : EbServiceStackAuthRequest, IReturn<GetConnectionsResponse>
    {
        public string SolutionId { get; set; }

        public int ConnectionType { get; set; }
    }

    public class GetConnectionsResponse : IEbSSResponse
    {
        public EbConnectionsConfig EBSolutionConnections { get; set; }
        public string Token { get; set; }
        public ResponseStatus ResponseStatus { get; set; }
    }

    public class ChangeDataDBConnectionRequest : EbServiceStackAuthRequest, IReturn<ChangeConnectionResponse>
    {
        public bool IsNew { get; set; }
        public string SolutionId { get; set; }
        public EbDataDbConnection DataDBConnection { get; set; }

    }

    public class ChangeObjectsDBConnectionRequest : EbServiceStackAuthRequest, IReturn<ChangeConnectionResponse>
    {
        public bool IsNew { get; set; }
        public EbObjectsDbConnection ObjectsDBConnection { get; set; }
        public string SolutionId { get; set; }
    }

    public class ChangeFilesDBConnectionRequest : EbServiceStackAuthRequest, IReturn<ChangeConnectionResponse>
    {
        public bool IsNew { get; set; }
        public string SolutionId { get; set; }
        public EbFilesDbConnection FilesDBConnection { get; set; }
    }

    public class ChangeFTPConnectionRequest : EbServiceStackAuthRequest, IReturn<ChangeConnectionResponse>
    {
        public bool IsNew { get; set; }
        public string SolutionId { get; set; }
        public EbFTPConnection FTPConnection { get; set; }
    }

    public class ChangeSMSConnectionRequest : EbServiceStackAuthRequest, IReturn<ChangeConnectionResponse>
    {
        public bool IsNew { get; set; }

        public string SolutionId { get; set; }

        public ISMSConnection SMSConnection { get; set; }
    }

    public class InitialSolutionConnectionsRequest : EbServiceStackAuthRequest, IReturn<InitialSolutionConnectionsResponse>
    {
        public string NewSolnId { get; set; }

        [DataMember(Order = 2)]
        public EbDbUsers DbUsers { get; set; }
    }

    public class InitialSolutionConnectionsResponse : IEbSSResponse
    {
        public ResponseStatus ResponseStatus { get; set; }
    }

    public class ChangeSMTPConnectionRequest : EbServiceStackAuthRequest, IReturn<ChangeConnectionResponse>
    {
        public bool IsNew { get; set; }

        public string SolutionId { get; set; }

        public EbEmail Email { get; set; }
    }

    public class ChangeCloudinaryConnectionRequest : EbServiceStackAuthRequest, IReturn<ChangeConnectionResponse>
    {
        public bool IsNew { get; set; }

        public string SolutionId { get; set; }

        public EbCloudinaryConnection ImageManipulateConnection { get; set; }
    }

    public class ChangeConnectionResponse : IEbSSResponse
    {
        public ResponseStatus ResponseStatus { get; set; }
    }

    public class TestConnectionRequest : EbServiceStackAuthRequest
    {
        public EbDataDbConnection DataDBConnection { get; set; }
    }
    public class TestConnectionResponse
    {
        public bool ConnectionStatus { set; get; }
    }

    [DataContract]
    public class TestFileDbconnectionRequest : IReturn<TestFileDbconnectionRequest>, IEbSSRequest
    {
        [DataMember(Order = 1)]
        public EbFilesDbConnection FilesDBConnection { get; set; }

        public int UserId { get; set; }

        [DataMember(Order = 2)]
        public string SolnId { get; set; }
    }

    public class TestFileDbconnectionResponse : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public bool ConnectionStatus { set; get; }

        [DataMember(Order = 2)]
        public ResponseStatus ResponseStatus { get; set; }
    }

    //--------------------------------------------------------------------------Integrations--------------------------------------
    public class AddDBRequest : IReturn<AddDBResponse>, IEbSSRequest
    {
        public EbDbConfig DbConfig { get; set; }

        public int UserId { get; set; }

        public string SolnId { get; set; }

        //public bool IsNew { get; set; }

        public string SolutionId { get; set; }
    }

    public class AddDBResponse : IEbSSResponse
    {
        public ResponseStatus ResponseStatus { get; set; }
    }

    public class GetIntegrationConfigsRequest : IReturn<GetIntegrationConfigsResponse>, IEbSSRequest
    {
        public int UserId { get; set; }

        public string SolnId { get; set; }
    }

    public class GetIntegrationConfigsResponse : IEbSSResponse
    {
        public ResponseStatus ResponseStatus { get; set; }
    }

    public class GetIntegrationsRequest : IReturn<GetIntegrationsResponse>, IEbSSRequest
    {
        public int UserId { get; set; }

        public string SolnId { get; set; }
    }

    public class GetIntegrationsResponse : IEbSSResponse
    {
        public ResponseStatus ResponseStatus { get; set; }
    }

    public class AddTwilioRequest : IReturn<AddTwilioResponse>, IEbSSRequest
    {
        public EbTwilioConfig Config { get; set; }

        public int UserId { get; set; }

        public string SolnId { get; set; }
    }

    public class AddTwilioResponse : IEbSSResponse
    {
        public ResponseStatus ResponseStatus { get; set; }
    }

    public class AddETRequest : IReturn<AddETResponse>, IEbSSRequest
    {
        public EbExpertTextingConfig Config { get; set; }

        public int UserId { get; set; }

        public string SolnId { get; set; }
    }

    public class AddETResponse : IEbSSResponse
    {
        public ResponseStatus ResponseStatus { get; set; }
    }

    public class AddMongoRequest : IReturn<AddMongoResponse>, IEbSSRequest
    {
        public EbMongoConfig Config { get; set; }

        public int UserId { get; set; }

        public string SolnId { get; set; }
    }

    public class AddMongoResponse : IEbSSResponse
    {
        public ResponseStatus ResponseStatus { get; set; }
    }

    public class AddSmtpRequest : IReturn<AddMongoResponse>, IEbSSRequest
    {
        public EbSmtpConfig Config { get; set; }

        public int UserId { get; set; }

        public string SolnId { get; set; }
    }

    public class AddSmtpResponse : IEbSSResponse
    {
        public ResponseStatus ResponseStatus { get; set; }
    }

    public class EbIntegrationRequest : IReturn<EbIntegrationResponse>, IEbSSRequest
    {
        public EbIntegration IntegrationO { get; set; }

        public int UserId { get; set; }

        public string SolnId { get; set; }
    }

    public class EbIntegrationResponse : IEbSSResponse
    {
        public ResponseStatus ResponseStatus { get; set; }
    }
}
