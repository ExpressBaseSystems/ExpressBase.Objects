using ExpressBase.Common;
using ExpressBase.Common.Application;
using ExpressBase.Common.Data;
using ExpressBase.Common.EbServiceStack.ReqNRes;
using ExpressBase.Common.Structures;
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

    public class ValidateSidResponse
    {
        public bool IsValid { set; get; }

        public byte[] Logo { set; get; }
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

        public EbMobileSettings AppSettings { set; get; }

        public List<MobilePagesWraper> MobilePages { set; get; }

        public List<WebObjectsWraper> WebObjects { set; get; }

        public EbDataSet OfflineData { set; get; }

        public AppDataToMob()
        {
            MobilePages = new List<MobilePagesWraper>();
            WebObjects = new List<WebObjectsWraper>();
            OfflineData = new EbDataSet();
        }
    }

    public class MobilePagesWraper
    {
        public string DisplayName { set; get; }

        public string Name { set; get; }

        public string Version { set; get; }

        public string RefId { set; get; }

        public string Json { set; get; }
    }

    public class WebObjectsWraper
    {
        public string DisplayName { set; get; }

        public string Name { set; get; }

        public string Version { set; get; }

        public string RefId { set; get; }

        public string Json { set; get; }

        public int ObjectType { set; get; }
    }

    public class EbStageActionsMobile
    {
        public string ActionName { set; get; }

        public string ActionUniqueId { set; get; }
    }

    public class EbMyActionsMobile
    {
        public int Id { set; get; }

        public DateTime StartDate { set; get; }

        public DateTime EndDate { set; get; }

        public int StageId { set; get; }

        public string WebFormRefId { set; get; }

        public int WebFormDataId { set; get; }

        public int ApprovalLinesId { set; get; }

        public string Description { set; get; }
    }

    public class SortColumn
    {
        public string Name { set; get; }

        public SortOrder Order { set; get; }

        public string GetString()
        {
            return Order == SortOrder.Ascending ? "ASC" : "DESC";
        }
    }

    //objects to mobile
    public class GetMobilePagesRequest : EbServiceStackAuthRequest, IReturn<GetMobilePagesResponse>
    {
        public int LocationId { set; get; }

        public int AppId { set; get; }

        public bool PullData { set; get; }
    }

    public class GetMobilePagesResponse
    {
        [DataMember(Order = 1)]
        public List<MobilePagesWraper> Pages { set; get; }

        [DataMember(Order = 2)]
        public List<WebObjectsWraper> WebObjects { set; get; }

        [DataMember(Order = 3)]
        public EbDataSet Data { set; get; }

        [DataMember(Order = 4)]
        public List<string> TableNames { set; get; }

        public GetMobilePagesResponse()
        {
            Pages = new List<MobilePagesWraper>();
            WebObjects = new List<WebObjectsWraper>();
        }
    }

    [DataContract]
    public class GetMobileVisDataRequest : EbServiceStackAuthRequest, IReturn<GetMobileVisDataResponse>
    {
        [DataMember(Order = 1)]
        public string DataSourceRefId { set; get; }

        [DataMember(Order = 2)]
        public List<Param> Params { get; set; }

        [DataMember(Order = 3)]
        public List<SortColumn> SortOrder { set; get; }

        [DataMember(Order = 4)]
        public int Limit { set; get; }

        [DataMember(Order = 5)]
        public int Offset { set; get; }

        [DataMember(Order = 6)]
        public bool IsPowerSelect { set; get; }

        public GetMobileVisDataRequest()
        {
            Params = new List<Param>();
            SortOrder = new List<SortColumn>();
        }
    }

    [DataContract]
    public class GetMobileVisDataResponse
    {
        [DataMember(Order = 1)]
        public string Message { set; get; }

        [DataMember(Order = 2)]
        public EbDataSet Data { set; get; }
    }

    [DataContract]
    public class GetMobileFormDataRequest : EbServiceStackAuthRequest, IReturn<GetMobileFormDataResponse>
    {
        [DataMember(Order = 1)]
        public string MobilePageRefId { set; get; }

        [DataMember(Order = 2)]
        public int RowId { get; set; }

        [DataMember(Order = 3)]
        public int LocId { get; set; }
    }

    [DataContract]
    public class GetMobileFormDataResponse
    {
        [DataMember(Order = 1)]
        public string Message { set; get; }

        [DataMember(Order = 2)]
        public WebformData Data { set; get; }
    }

    public class GetMyActionsRequest : EbServiceStackAuthRequest, IReturn<GetMyActionsResponse>
    {

    }

    [DataContract]
    public class GetMyActionsResponse
    {
        [DataMember(Order = 1)]
        public List<EbMyActionsMobile> Actions { get; set; }

        public GetMyActionsResponse()
        {
            Actions = new List<EbMyActionsMobile>();
        }
    }

    public class GetMyActionInfoRequest : EbServiceStackAuthRequest, IReturn<GetMyActionInfoResponse>
    {
        public int StageId { set; get; }

        public string WebFormRefId { set; get; }

        public int WebFormDataId { set; get; }
    }

    [DataContract]
    public class GetMyActionInfoResponse
    {
        [DataMember(Order = 1)]
        public string StageUniqueId { set; get; }

        [DataMember(Order = 2)]
        public string StageName { set; get; }

        [DataMember(Order = 3)]
        public List<EbStageActionsMobile> StageActions { set; get; }

        [DataMember(Order = 4)]
        public List<Param> Data { set; get; }

        public GetMyActionInfoResponse()
        {
            StageActions = new List<EbStageActionsMobile>();
        }
    }

    //bulk data
    [DataContract]
    public class EbMobileSolutionData
    {
        [DataMember(Order = 1)]
        public List<AppDataToMob> Applications { set; get; }

        public EbMobileSolutionData()
        {
            Applications = new List<AppDataToMob>();
        }
    }

    public class MobileSolutionDataRequest : EbServiceStackAuthRequest, IReturn<EbMobileSolutionData>
    {
        
    }
}
