using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using ServiceStack;
using ExpressBase.Common.Connections;
using ExpressBase.Common.Data;
using ExpressBase.Common.EbServiceStack.ReqNRes;
using ExpressBase.Common;

namespace ExpressBase.Objects.ServiceStack_Artifacts
{
    public class RefreshSolutionConnectionsRequest : EbServiceStackRequest
    {
        public string BToken { get; set; }

        public string RToken { get; set; }
    }


    public class RefreshSolutionConnectionsAsyncRequest : EbServiceStackRequest, IReturn<RefreshSolutionConnectionsAsyncResponse>
    {
        public string SolutionId { get; set; }
    }

    public class RefreshSolutionConnectionsAsyncResponse : IEbSSResponse
    {
        public ResponseStatus ResponseStatus { get; set; }
    }


    public class RefreshSolutionConnectionsBySolutionIdAsyncRequest : EbServiceStackRequest, IReturn<RefreshSolutionConnectionsAsyncResponse>
    {
        public string SolutionId { get; set; }
    }

    public class GetConnectionsRequest : EbServiceStackRequest, IReturn<GetConnectionsResponse>
    {
        public int ConnectionType { get; set; }
    }

    public class GetConnectionsResponse : IEbSSResponse
    {
        public EbConnectionsConfig EBSolutionConnections { get; set; }
        public string Token { get; set; }
        public ResponseStatus ResponseStatus { get; set; }
    }

    public class ChangeDataDBConnectionRequest : EbServiceStackRequest, IReturn<ChangeConnectionResponse>
    {
        public bool IsNew { get; set; }
        public string SolutionId { get; set; }
        public EbDataDbConnection DataDBConnection { get; set; }

    }

    public class ChangeObjectsDBConnectionRequest : EbServiceStackRequest, IReturn<ChangeConnectionResponse>
    {
        public bool IsNew { get; set; }
        public EbObjectsDbConnection ObjectsDBConnection { get; set; }
        public string SolutionId { get; set; }
    }

    public class ChangeFilesDBConnectionRequest : EbServiceStackRequest, IReturn<ChangeConnectionResponse>
    {
        public bool IsNew { get; set; }
        public EbFilesDbConnection FilesDBConnection { get; set; }
    }

    public class ChangeSMSConnectionRequest : EbServiceStackRequest, IReturn<ChangeConnectionResponse>
    {
        public bool IsNew { get; set; }
        public SMSConnection SMSConnection { get; set; }
    }

    public class InitialSolutionConnectionsRequest : EbServiceStackRequest, IReturn<InitialSolutionConnectionsResponse>
    {
        public string SolutionId { get; set; }
    }

    public class InitialSolutionConnectionsResponse : IEbSSResponse
    {
        public ResponseStatus ResponseStatus { get ; set ; }
    }

    public class ChangeSMTPConnectionRequest : EbServiceStackRequest, IReturn<ChangeConnectionResponse>
    {
        public bool IsNew { get; set; }
        public SMTPConnection SMTPConnection { get; set; }
    }

    public class ChangeConnectionResponse : IEbSSResponse
    {
        public ResponseStatus ResponseStatus { get; set; }
    }

    public class TestConnectionRequest : EbServiceStackRequest
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
        public string TenantAccountId { get; set; }
    }

    public class TestFileDbconnectionResponse : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public bool ConnectionStatus { set; get; }

        [DataMember(Order = 2)]
        public ResponseStatus ResponseStatus { get; set; }
    }
}
