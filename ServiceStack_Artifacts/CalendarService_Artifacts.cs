using ExpressBase.Common;
using ExpressBase.Common.Data;
using ExpressBase.Common.EbServiceStack.ReqNRes;
using ExpressBase.Common.LocationNSolution;
using ExpressBase.Security;
using ServiceStack;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace ExpressBase.Objects.ServiceStack_Artifacts
{
    
    [DataContract]
    public class CalendarDataRequest : IReturn<CalendarDataResponse>, IEbSSRequest
    {
        [DataMember(Order = 0)]
        public string RefId { get; set; }

        [DataMember(Order = 4)]
        public List<OrderBy> OrderBy { get; set; }

        [DataMember(Order = 6)]
        public string Token { get; set; }

        [DataMember(Order = 7)]
        public string rToken { get; set; }

        [DataMember(Order = 8)]
        public string SolnId { get; set; }

        [DataMember(Order = 9)]
        public int UserId { get; set; }

        [DataMember(Order = 10)]
        public List<Param> Params { get; set; }

        [DataMember(Order = 11)]
        public string CalendarObjString { get; set; }

        [DataMember(Order = 12)]
        public EbCalendarView CalendarObj { get; set; }

        [DataMember(Order = 13)]
        public User UserInfo { get; set; }

        [DataMember(Order = 18)]
        public Eb_Solution eb_Solution { get; set; }

        [DataMember(Order = 18)]
        public bool ModifyDv { get; set; }

    }
    
    [DataContract]
    public class CalendarDataResponse : IEbSSResponse
    {

        [DataMember(Order = 4)]
        public RowColletion Data { get; set; }

        [DataMember(Order = 5)]
        public string Token { get; set; }

        [DataMember(Order = 6)]
        public ResponseStatus ResponseStatus { get; set; }

        [DataMember(Order = 7)]
        public System.Data.DataSet DataSet { get; set; }

        [DataMember(Order = 9)]
        public RowColletion FormattedData { get; set; }

        [DataMember(Order = 11)]
        public string CalendarReturnObjString { get; set; }

        [DataMember(Order = 12)]
        public EbCalendarView CalendarReturnObj { get; set; }
    }
}
