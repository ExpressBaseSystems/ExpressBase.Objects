using ExpressBase.Common.Data;
using ExpressBase.Common.EbServiceStack.ReqNRes;
using ServiceStack;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExpressBase.Objects.ServiceStack_Artifacts
{
    public class SqlJobRequest : IReturn<SqlJobResponse>, IEbSSRequest
    {
        public string SolnId { get; set; }

        public int UserId { get; set; }

        public List<Param> GlobalParams { get; set; }
    }

    public class SqlJobResponse : IEbSSResponse
    {
        public ApiMessage Message { get; set; }
        public ResponseStatus ResponseStatus { get; set; }

        public SqlJobResponse()
        {
            Message = new ApiMessage();
        }
    }
}
