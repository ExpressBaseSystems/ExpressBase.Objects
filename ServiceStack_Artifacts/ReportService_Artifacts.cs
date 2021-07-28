using ExpressBase.Common;
using ExpressBase.Common.Data;
using ExpressBase.Common.EbServiceStack.ReqNRes;
using ExpressBase.Common.Structures;
using ExpressBase.Security;
using ServiceStack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Text;

namespace ExpressBase.Objects.ServiceStack_Artifacts
{
    public class ReportRenderRequest : EbServiceStackAuthRequest, IReturn<ReportRenderResponse>
    {
        public string Refid { get; set; }

        public string RenderingUserAuthId { get; set; }

        public string ReadingUserAuthId { get; set; }}

        public List<Param> Params { get; set; }
    }

    [DataContract]
    public class ReportRenderResponse : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public MemorystreamWrapper StreamWrapper { get; set; }

        [DataMember(Order = 2)]
        public ResponseStatus ResponseStatus { get; set; }

        [DataMember(Order = 3)]
        public String ReportName { get; set; }

        [DataMember (Order =4)]
        public byte[] ReportBytea { get; set; }

        [DataMember(Order =5)]
        public DateTime CurrentTimestamp { get; set; }
    }

    public class ValidateCalcExpressionRequest : EbServiceStackAuthRequest, IReturn<ReportRenderResponse>
    {
        public string DataSourceRefId { get; set; }

        public string ValueExpression { get; set; }

        public List<Param> Parameters { get; set; }
    }

    [DataContract]
    public class ValidateCalcExpressionResponse : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public bool IsValid { get; set; }

        [DataMember(Order = 2)]
        public int Type { get; set; }

        [DataMember(Order = 3)]
        public string ExceptionMessage { get; set; }

        [DataMember(Order = 4)]
        public ResponseStatus ResponseStatus { get; set; }
    }

    public class ReportInternalRequest : EbServiceStackAuthRequest, IReturn<ReportRenderResponse>
    {
        [DataMember(Order = 1)]
        public EbJobArguments JobArgs { get; set; }
    }

    public class ReportInternalResponse : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public ResponseStatus ResponseStatus { get; set; }
    }
}
