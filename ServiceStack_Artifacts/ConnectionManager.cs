using ExpressBase.Common.Data;

namespace ExpressBase.Objects.ServiceStack_Artifacts
{
    public class GetConnectionsRequest : EbServiceStackRequest
    {

    }

    public class GetConnectionsResponse
    {
        public EbSolutionConnections EBSolutionConnections { get; set; }
        public string Token { get; set; }
    }

    public class ChangeConnectionRequest : EbServiceStackRequest
    {
        public EbSolutionConnections EBSolutionConnections { get; set; }
        public string Token { get; set; }
    }

}
