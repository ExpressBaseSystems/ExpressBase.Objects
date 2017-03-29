using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExpressBase.Objects.ServiceStack_Artifacts
{
    public interface IEbSSRequest
    {
        string Token { get; set; }

        string TenantAccountId { get; set; }
    }
}
