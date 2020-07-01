using ExpressBase.Common;
using ExpressBase.Common.Data;
using ExpressBase.Common.EbServiceStack.ReqNRes;
using ServiceStack;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

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
    public class SMSSentStatus : EbServiceStackAuthRequest
    {
        [DataMember(Order = 1)]
        public string To { get; set; }

        [DataMember(Order = 2)]
        public string From { get; set; }

        [DataMember(Order = 3)]
        public string MessageId { get; set; }

        [DataMember(Order = 4)]
        public string Body { get; set; }

        [DataMember(Order = 5)]
        public string Status { get; set; }

        [DataMember(Order = 6)]
        public DateTime SentTime { get; set; }

        [DataMember(Order = 7)]
        public DateTime ReceivedTime { get; set; }

        [DataMember(Order = 8)]
        public string ErrorMessage { get; set; }

        [DataMember(Order = 9)]
        public string Uri { get; set; }

        [DataMember(Order = 10)]
        public string Result { get; set; }

        [DataMember(Order = 11)]
        public int ConId { get; set; }

    }
    [DataContract]
    public class SMSStatusLogMqRequest : EbServiceStackAuthRequest
    {
        [DataMember(Order = 1)]
        public SMSSentStatus SMSSentStatus { get; set; }

        [DataMember(Order = 2)]
        public string RefId { get; set; }   
        
        [DataMember(Order = 3)]
        public string MetaData { get; set; }
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
}