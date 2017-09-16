using ExpressBase.Objects.ServiceStack_Artifacts;

namespace ExpressBase.Common.Connections
{
    public class GetConnectionsRequest : IEbSSRequest
    {
        public string TenantAccountId { get; set; }

        public int UserId { get; set; }

    }
    
}
