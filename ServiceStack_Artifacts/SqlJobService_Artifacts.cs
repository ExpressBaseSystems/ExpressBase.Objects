using ExpressBase.Common;
using ExpressBase.Common.Data;
using ExpressBase.Common.EbServiceStack.ReqNRes;
using ExpressBase.Objects.Objects.SqlJobRelated;
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

    public class RetryJobRequest : IEbSSRequest, IReturn<RetryJobResponse>
    {

        public int JoblogId { get; set; }

        public string SolnId { get; set; }

        public int UserId { get; set; }

    };

    public class RetryJobResponse : IEbSSResponse
    {
        public ResponseStatus ResponseStatus { get; set; }

    }
    public class LogLine
    {
        public int linesid { get; set; }

        public int masterid { get; set; }

        public string Refid { get; set; }

        public Dictionary<string, TV> Params { get; set; }

        public string Message { get; set; }

        public string Status { get; set; }

        public Dictionary<string, TV> Keyvalues { get; set; }

        public int RetryOf { get; set; }

    }

    public class LoopLocation
    {
        public EbLoop Loop { get; set; }

        public int Step { get; set; }

        public int ParentIndex { get; set; }
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

    public class ProcessorRequest:IReturn<ProcessorResponse>,IEbSSRequest
    {
        public string SolnId { get; set; }

        public int UserId { get; set; }
    }

    public class ProcessorResponse:IEbSSResponse
    {
        public ResponseStatus ResponseStatus { get; set; }
    }
}
