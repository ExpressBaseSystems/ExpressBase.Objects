using ExpressBase.Common;
using ExpressBase.Common.EbServiceStack.ReqNRes;
using ExpressBase.Security;
using ServiceStack;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace ExpressBase.Objects.ServiceStack_Artifacts
{
    [DataContract]
    public class CreateMobileFormTableRequest : EbServiceStackAuthRequest, IReturn<CreateMobileFormTableResponse>
    {
        [DataMember(Order = 1)]
        public EbMobilePage MobilePage { get; set; }

        [DataMember(Order = 2)]
        public string Apps { get; set; }
    }

    public class CreateMobileFormTableResponse
    {

    }

    [DataContract]
    public class GetMobMenuRequest : EbServiceStackAuthRequest, IReturn<GetMobMenuResonse>
    {
        //request to mobile menu
        [DataMember(Order = 1)]
        public int LocationId { get; set; }
    }

    [DataContract]
    public class GetMobMenuResonse : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public ResponseStatus ResponseStatus { get; set; }

        [DataMember(Order = 2)]
        public List<AppDataToMob> Applications { get; set; }

        public GetMobMenuResonse()
        {
            Applications = new List<AppDataToMob>();
        }
    }

    //objects to mobile
    public class ObjectListToMobRequest : EbServiceStackAuthRequest, IReturn<ObjectListToMob>
    {
        public int LocationId { set; get; }

        public int AppId { set; get; }
    }

    public class ObjectListToMob
    {
        [DataMember(Order = 1)]
        public Dictionary<int, List<ObjWrap>> ObjectTypes { set; get; }

        public ObjectListToMob()
        {
            ObjectTypes = new Dictionary<int, List<ObjWrap>>();
        }
    }

    [DataContract]
    public class EbObjectToMobRequest : IReturn<EbObjectToMobResponse>, IEbSSRequest
    {
        public string SolnId { get; set; }

        public int UserId { get; set; }

        [DataMember(Order = 1)]
        public string RefId { set; get; }

        [DataMember(Order = 2)]
        public User User { set; get; }
    }

    [DataContract]
    public class EbObjectToMobResponse : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public EbObjectWrapper ObjectWraper { set; get; }

        [DataMember(Order = 2)]
        public byte[] ReportResult { get; set; }

        [DataMember(Order = 3)]
        public EbDataSet TableResult { get; set; }

        [DataMember(Order = 5)]
        public ResponseStatus ResponseStatus { get; set; }
    }
}
