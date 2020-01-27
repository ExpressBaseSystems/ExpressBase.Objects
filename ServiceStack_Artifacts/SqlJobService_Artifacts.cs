using ExpressBase.Common;
using ExpressBase.Common.Data;
using ExpressBase.Common.EbServiceStack.ReqNRes;
using ExpressBase.Objects;
using ExpressBase.Objects.Objects.DVRelated;
using ServiceStack;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExpressBase.Objects.ServiceStack_Artifacts
{
    public class ExecuteSqlJobRequest : EbServiceStackAuthRequest, IReturn<ExecuteSqlJobResponse> 
    {
        public string RefId { get; set; }

        public int ObjId { get; set; }

        public List<Param> GlobalParams { get; set; }
    }

    public class ExecuteSqlJobResponse : IEbSSResponse
    {
        public ApiMessage Message { get; set; }
        public ResponseStatus ResponseStatus { get; set; }

        public ExecuteSqlJobResponse()
        {
            Message = new ApiMessage();
        }
    }

    public class SqlJobInternalRequest:EbServiceStackAuthRequest
    {
        public int ObjId { get; set; }

        public List<Param> GlobalParams { get; set; }
    }

    public class RetryJobRequest : EbServiceStackAuthRequest, IReturn<RetryJobResponse>
    {
        public string RefId { get; set; }

        public int JoblogId { get; set; } 

    };


    public class ListSqlJobsRequest : EbServiceStackAuthRequest, IReturn<ListSqlJobsResponse>
    {
        public string RefId { get; set; }

        public string Date { get; set; }
    }

    public class ListSqlJobsResponse
    {
        public ColumnColletion SqlJobsColumns { get; set; }
        public RowColletion SqlJobsRows { get; set; }

        public string SqlJobsDvColumns { get; set; }

        public List<GroupingDetails> Levels { get; set; }
    }


    public class RetryJobResponse : IEbSSResponse
    {
        public ResponseStatus ResponseStatus { get; set; }
        public bool Status { get; set; }

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
