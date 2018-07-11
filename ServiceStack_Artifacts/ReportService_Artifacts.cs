using ExpressBase.Common;
using ExpressBase.Common.Data;
using ExpressBase.Common.EbServiceStack.ReqNRes;
using ExpressBase.Common.Structures;
using ServiceStack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Text;

namespace ExpressBase.Objects.ServiceStack_Artifacts
{
    public class ReportRenderRequest : EbServiceStackRequest, IReturn<ReportRenderResponse>
    {
        public string Refid { get; set; }

        public string Fullname { get; set; }

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
    }

    public class ValidateCalcExpressionRequest : EbServiceStackRequest, IReturn<ReportRenderResponse>
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
}
