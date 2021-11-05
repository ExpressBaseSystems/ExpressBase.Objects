using ExpressBase.Common;
using ExpressBase.Common.Data;
using ExpressBase.Common.EbServiceStack.ReqNRes;
using ExpressBase.Common.Messaging;
using ExpressBase.Objects.Objects.SmsRelated;
using ServiceStack;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ExpressBase.Objects.ServiceStack_Artifacts
{

    [DataContract]
    public class SMSSentRequest : EbServiceStackAuthRequest
    {
        [DataMember(Order = 1)]
        public string To { get; set; }

        [DataMember(Order = 2)]
        public string Body { get; set; }

        [DataMember(Order = 3)]
        public string MediaUrl { get; set; }

        [DataMember(Order = 4)]
        public string RefId { get; set; }

        [DataMember(Order = 5)]
        public List<Param> Params { get; set; }

        [DataMember(Order = 6)]
        public Int32 RetryOf { get; set; }
    }

    [DataContract]
    public class SmsDirectRequest : EbServiceStackAuthRequest
    {
        [DataMember(Order = 1)]
        public string To { get; set; }

        [DataMember(Order = 2)]
        public string Body { get; set; }

        [DataMember(Order = 3)]
        public string MediaUrl { get; set; }

        [DataMember(Order = 4)]
        public Int32 Retryof { get; set; }

        [DataMember(Order = 5)]
        public string RefId { get; set; }
    }
    [DataContract]
    public class SMSPrepareRequest : EbServiceStackAuthRequest
    {
        [DataMember(Order = 1)]
        public int ObjId { get; set; }

        [DataMember(Order = 2)]
        public List<Param> Params { get; set; }

        [DataMember(Order = 3)]
        public string MediaUrl { get; set; }

        [DataMember(Order = 4)]
        public string Message { get; set; }

        [DataMember(Order = 5)]
        public string To { get; set; }

        [DataMember(Order = 6)]
        public string RefId { get; set; }

    }

    [DataContract]
    public class SMSInitialRequest : EbServiceStackAuthRequest
    {
        [DataMember(Order = 1)]
        public int ObjId { get; set; }

        [DataMember(Order = 2)]
        public List<Param> Params { get; set; }

        [DataMember(Order = 3)]
        public string MediaUrl { get; set; }

        [DataMember(Order = 4)]
        public string RefId { get; set; }
    }


    [DataContract]
    public class GetFilledSmsTemplateRequest : EbServiceStackAuthRequest, IReturn<GetFilledSmsTemplateResponse>
    {
        [DataMember(Order = 1)]
        public int ObjId { get; set; }

        [DataMember(Order = 2)]
        public List<Param> Params { get; set; }

        [DataMember(Order = 4)]
        public string RefId { get; set; }
    }

    public class GetFilledSmsTemplateResponse
    {
        public FilledSmsTemplate FilledSmsTemplate { get; set; }
    }

    public class FilledSmsTemplate
    {
        public string SmsTo { get; set; }

        public EbSmsTemplate SmsTemplate { get; set; }
    }


    [DataContract]
    public class SMSStatusLogMqRequest : EbServiceStackAuthRequest
    {
        [DataMember(Order = 1)]
        public SentStatus SentStatus { get; set; }

        [DataMember(Order = 2)]
        public string RefId { get; set; }

        [DataMember(Order = 3)]
        public string MetaData { get; set; }

        [DataMember(Order = 4)]
        public Int32 RetryOf { get; set; }
    }

    [DataContract]
    public class ListSMSLogsResponse
    {
        [DataMember(Order = 1)]
        public ColumnColletion SMSLogsColumns { get; set; }

        [DataMember(Order = 2)]
        public RowColletion SMSLogsRows { get; set; }

        [DataMember(Order = 3)]
        public string SMSLogsDvColumns { get; set; }

        [DataMember(Order = 4)]
        public List<GroupingDetails> Levels { get; set; }

        [DataMember(Order = 5)]
        public RowColletion FormattedData { get; set; }

        [DataMember(Order = 6)]
        public string Visualization { get; set; }
    }

    [DataContract]
    public class RetrySmsResponse : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public ResponseStatus ResponseStatus { get; set; }
    }

    public class RetrySmsRequest : EbServiceStackAuthRequest, IReturn<RetrySmsResponse>
    {
        public string RefId { get; set; }

        public int SmslogId { get; set; }

    };
}