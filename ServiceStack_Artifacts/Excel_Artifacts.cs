using ExpressBase.Common.EbServiceStack.ReqNRes;
using ExpressBase.Common.Excel;
using ExpressBase.Objects.Objects;
using ServiceStack;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace ExpressBase.Objects.ServiceStack_Artifacts
{
    public class ExcelDownloadRequest : EbServiceStackAuthRequest, IReturn<ExcelDownloadResponse>
    {
        [DataMember(Order = 1)]
        public string _refid { get; set; }
    }

    public class ExcelDownloadResponse : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public List<ColumnsInfo> colsInfo { get; set; }

        [DataMember(Order = 2)]
        public string formName { get; set; }

        [DataMember(Order = 2)]
        public string Token { get; set; }

        [DataMember(Order = 3)]
        public ResponseStatus ResponseStatus { get; set; }
    }
}
