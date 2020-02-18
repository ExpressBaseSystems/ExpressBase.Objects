using ExpressBase.Common;
using ExpressBase.Common.Data;
using ExpressBase.Common.EbServiceStack.ReqNRes;
using ExpressBase.Objects;
using ExpressBase.Objects.Objects.DVRelated;
using ServiceStack;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace ExpressBase.Objects.ServiceStack_Artifacts
{
    public class ExecuteSqlJobRequest : EbServiceStackAuthRequest, IReturn<ExecuteSqlJobResponse>
    {
        public string RefId { get; set; }

        public int ObjId { get; set; }

        public List<Param> GlobalParams { get; set; }

        public int InMasterId { get; set; }
    }

    public class ExecuteSqlJobResponse : IEbSSResponse
    {
        public String Message { get; set; }
        public ResponseStatus ResponseStatus { get; set; }

       
    }

    public class SqlJobInternalRequest : EbServiceStackAuthRequest
    {
        public int ObjId { get; set; }

        public List<Param> GlobalParams { get; set; }
    }

    public class RetryLineRequest : EbServiceStackAuthRequest, IReturn<RetryLineResponse>
    {
        public string RefId { get; set; }

        public int JoblogId { get; set; }

    };

    public class RetryLineResponse : IEbSSResponse
    {
        public ResponseStatus ResponseStatus { get; set; }
        public bool Status { get; set; }

    }

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

        public RowColletion FormattedData { get; set; }
        public string Visualization { get; set; }
        public EbDataVisualization Visobject { get; set; }
    }

    public class RetryMasterRequest : EbServiceStackAuthRequest, IReturn<RetryMasterResponse>
    {
        public string RefId { get; set; }

        public int MasterId { get; set; }

        public List<Param> GlobalParams { get; set; }
    }

    public class RetryMasterResponse : IEbSSResponse
    {
        public ResponseStatus ResponseStatus { get; set; }
    }

    public class DeleteJobExecutionRequest : EbServiceStackAuthRequest, IReturn<DeleteJobExecutionResponse>
    {
        public string RefId { get; set; }

        public int MasterId { get; set; }
    }

    public class DeleteJobExecutionResponse : IEbSSResponse
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

    public class SqlJobResult
    {
        public string RefId { get; set; }

        public int RtnId { get; set; }//returning Id

        public string Status { get; set; }

        public string Message { get; set; }

        public string Type { get; set; }
    }

    public class SqlJobResults : List<SqlJobResult>
    {
        public string Message { get; set; }
    }
}
