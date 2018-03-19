﻿using ExpressBase.Common;
using ExpressBase.Common.EbServiceStack.ReqNRes;
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
    }

    [DataContract]
    public class ReportRenderResponse : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public MemorystreamWrapper StreamWrapper { get; set; }

        [DataMember(Order = 2)]
        public ResponseStatus ResponseStatus { get; set; }
    }
}