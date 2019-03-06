using ExpressBase.Common.EbServiceStack.ReqNRes;
using ServiceStack;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using ExpressBase.Common.SqlProfiler;
using ExpressBase.Common;
using ExpressBase.Objects.Objects.DVRelated;
using ExpressBase.Common.Data;

namespace ExpressBase.Objects.ServiceStack_Artifacts
{
    public class GetExecLogsRequest /*: EbServiceStackAuthRequest, IReturn<GetExecLogsResponse>*/
    {
        [DataMember(Order = 1)]
        public string RefId { get; set; }
    }

    public class GetProfilersRequest : EbServiceStackAuthRequest, IReturn<GetExecLogsResponse>
    {
        public string RefId { get; set; }
    }

    public class GetExecLogsResponse : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public List<EbExecutionLogs> Logs { get; set; }

        [DataMember(Order = 2)]
        public string Token { get; set; }

        [DataMember(Order = 3)]
        public ResponseStatus ResponseStatus { get; set; }
    }

    public class GetProfilersResponse : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public Profiler Profiler { get; set; }

        [DataMember(Order = 2)]
        public string Token { get; set; }

        [DataMember(Order = 3)]
        public ResponseStatus ResponseStatus { get; set; }
    }

    [DataContract]
    public class ProfilerQueryColumnRequest : EbServiceStackAuthRequest, IReturn<ProfilerQueryResponse>
    {
        [DataMember(Order = 1)]
        public string RefId { get; set; }
    }

    [DataContract]
    public class ProfilerQueryDataRequest : EbServiceStackAuthRequest, IReturn<ProfilerQueryResponse>
    {
        [DataMember(Order = 1)]
        public string RefId { get; set; }

        [DataMember(Order = 2)]
        public int Start { get; set; }

        [DataMember(Order = 3)]
        public int Length { get; set; }

        [DataMember(Order = 4)]
        public int Draw { get; set; }
    }

    [DataContract]
    public class ProfilerQueryResponse
    {
        [DataMember(Order = 1)]
        public int Draw { get; set; }

        [DataMember(Order = 1)]
        public ColumnColletion ColumnCollection { get; set; }

        [DataMember(Order = 1)]
        public RowColletion data { get; set; }

        [DataMember(Order = 2)]
        public int RecordsTotal { get; set; }

        [DataMember(Order = 3)]
        public int RecordsFiltered { get; set; }
    }

    public class GetLogdetailsRequest : EbServiceStackAuthRequest, IReturn<GetLogdetailsResponse>
    {
        public int Index { get; set; }
    }

    public class GetLogdetailsResponse : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public EbExecutionLogs logdetails { get; set; }

        [DataMember(Order = 2)]
        public ResponseStatus ResponseStatus { get; set; }
    }

    public class GetChartDetailsRequest : EbServiceStackAuthRequest, IReturn<GetChartDetailsResponse>
    {
        public string Refid { get; set; }
    }

    public class GetChartDetailsResponse : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public RowColletion data { get; set; }

        [DataMember(Order = 2)]
        public ResponseStatus ResponseStatus { get; set; }
    }

    public class GetChart2DetailsRequest : EbServiceStackAuthRequest, IReturn<GetChart2DetailsResponse>
    {
        public string Refid { get; set; }
    }

    public class GetChart2DetailsResponse : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public RowColletion data { get; set; }

        [DataMember(Order = 2)]
        public ResponseStatus ResponseStatus { get; set; }
    }

    public class GetExplainRequest : EbServiceStackAuthRequest, IReturn<GetExplainResponse>
    {
        public string Query { get; set; }

        public List<Param> Params { get; set; }
    }

    public class GetExplainResponse : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public string Explain { get; set; }

        [DataMember(Order = 2)]
        public ResponseStatus ResponseStatus { get; set; }
    }
}
