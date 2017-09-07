using ExpressBase.Common.Connections;
using ExpressBase.Objects.ServiceStack_Artifacts;
using ServiceStack;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExpressBase.Objects.Objects.TenantConnectionsRelated
{
    public class AddSMTPConnectionRequest : IReturn<bool>,IEbSSRequest
    {
        public SMTPConnection SMTPConnection { get; set; }
        
        public string TenantAccountId { get; set; }

        public int UserId { get; set; }
    }

    public class RefreshSolutionConnectionsRequests : IReturn<bool>,IEbSSRequest
    {
        public string TenantAccountId { get; set; }

        public int UserId { get; set; }
    }
}
