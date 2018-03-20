using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using ServiceStack;
using ExpressBase.Common.Connections;
using ExpressBase.Common.Data;
using ExpressBase.Common.EbServiceStack.ReqNRes;

namespace ExpressBase.Objects.ServiceStack_Artifacts
{
    public class RefreshSolutionConnectionsRequest : EbServiceStackRequest
    {
    }

    public class RefreshSolutionConnectionsAsyncRequest : EbServiceStackRequest
    {
        public string SolutionId { get; set; }
    }

    public class RefreshSolutionConnectionsBySolutionIdAsyncRequest : EbServiceStackRequest
    {
        public string SolutionId { get; set; }
    }

    public class GetConnectionsRequest : EbServiceStackRequest
    {
        public int ConnectionType { get; set; }
    }

    public class GetConnectionsResponse
    {    
        public EbConnectionsConfig EBSolutionConnections { get; set; }
        public string Token { get; set; }
    }

    public class ChangeDataDBConnectionRequest : EbServiceStackRequest
    {
        public bool IsNew { get; set; }
        public EbDataDbConnection DataDBConnection { get; set; }
    }

    public class ChangeObjectsDBConnectionRequest : EbServiceStackRequest
    {
        public bool IsNew { get; set; }
        public EbObjectsDbConnection ObjectsDBConnection { get; set; }
    }

    public class ChangeFilesDBConnectionRequest : EbServiceStackRequest
    {
        public bool IsNew { get; set; }
        public EbFilesDbConnection FilesDBConnection { get; set; }
    }

    public class ChangeSMSConnectionRequest : EbServiceStackRequest
    {
        public bool IsNew { get; set; }
        public SMSConnection SMSConnection { get; set; }
    }

    public class InitialSolutionConnectionsRequest : EbServiceStackRequest
    {
        public string SolutionId { get; set; }
    }

    public class ChangeSMTPConnectionRequest : EbServiceStackRequest
    {
        public bool IsNew { get; set; }
        public SMTPConnection SMTPConnection { get; set; }
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
