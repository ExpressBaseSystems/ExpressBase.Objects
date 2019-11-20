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

    public class AppDataToMob
    {
        public int AppId { set; get; }

        public string AppName { set; get; }

        public string AppIcon { set; get; }
    }

    public class MobilePagesWraper
    {
        public string DisplayName { set; get; }

        public string Name { set; get; }

        public string Version { set; get; }

        public string Json { set; get; }
    }

    //objects to mobile
    public class GetMobilePagesRequest : EbServiceStackAuthRequest, IReturn<GetMobilePagesResponse>
    {
        public int LocationId { set; get; }

        public int AppId { set; get; }
    }

    public class GetMobilePagesResponse
    {
        [DataMember(Order = 1)]
        public List<MobilePagesWraper> Pages { set; get; }

        public GetMobilePagesResponse()
        {
            this.Pages = new List<MobilePagesWraper>();
        }
    }
}
