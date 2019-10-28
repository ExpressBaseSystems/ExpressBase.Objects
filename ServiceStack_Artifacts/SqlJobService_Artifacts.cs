using ExpressBase.Common;
using ExpressBase.Common.Data;
using ExpressBase.Common.EbServiceStack.ReqNRes;
using ExpressBase.Objects.Objects.DVRelated;
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
    

    public class SqlJobsListGetRequest : IReturn<SqlJobsListGetResponse>
    {
        public string Refid { get; set; }

        public string Date { get; set; }
    }

    public class SqlJobsListGetResponse
    {
        public ColumnColletion SqlJobsColumns { get; set; }
        public RowColletion SqlJobsRows { get; set; }

        public string SqlJobsDvColumns { get; set; }
    }
}
