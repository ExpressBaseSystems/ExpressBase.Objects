using ExpressBase.Common.Connections;
using ExpressBase.Common.Data;

namespace ExpressBase.Objects.ServiceStack_Artifacts
{
    public class GetConnectionsRequest : EbServiceStackRequest
    {
        public int ConnectionType { get; set; }
    }

    public class GetConnectionsResponse
    {
        public EbSolutionConnections EBSolutionConnections { get; set; }
        public string Token { get; set; }
    }

    public class ChangeDataDBConnectionRequest : EbServiceStackRequest
    {
        public bool IsNew { get; set; }
        public EbDataDbConnection DataDBConnection { get; set; }
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

    public class ChangeSMTPConnectionRequest : EbServiceStackRequest
    {
        public bool IsNew { get; set; } 
        public SMTPConnection SMTPConnection { get; set; }
    }
    
    public class RefreshSolutionConnectionsMqRequest : EbServiceStackRequest
    {
       
    }
}
