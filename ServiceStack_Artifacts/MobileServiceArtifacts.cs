using ExpressBase.Common;
using ExpressBase.Common.Application;
using ExpressBase.Common.Data;
using ExpressBase.Common.EbServiceStack.ReqNRes;
using ExpressBase.Common.LocationNSolution;
using ServiceStack;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

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

        public Eb_Solution SolutionObj { get; set; }

        public string SignUpPage { set; get; }

        public string Message { set; get; }
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

        public int StageId { set; get; }

        public string WebFormRefId { set; get; }

        public int WebFormDataId { set; get; }

        public int ApprovalLinesId { set; get; }

        public string Description { set; get; }

        public MyActionTypes ActionType { set; get; }
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

    [DataContract]
    public class MobileVisDataRequest : EbServiceStackAuthRequest, IReturn<MobileVisDataResponse>
    {
        [DataMember(Order = 1)]
        public string DataSourceRefId { set; get; }

        [DataMember(Order = 2)]
        public List<Param> Params { get; set; }

        [DataMember(Order = 3)]
        public List<SortColumn> SortOrder { set; get; }

        [DataMember(Order = 4)]
        public List<Param> SearchColumns { set; get; }

        [DataMember(Order = 5)]
        public int Limit { set; get; }

        [DataMember(Order = 6)]
        public int Offset { set; get; }

        [DataMember(Order = 7)]
        public bool IsPowerSelect { set; get; }

        public MobileVisDataRequest()
        {
            Params = new List<Param>();
            SortOrder = new List<SortColumn>();
            SearchColumns = new List<Param>();
        }
    }

    [DataContract]
    public class MobileVisDataResponse
    {
        [DataMember(Order = 1)]
        public string Message { set; get; }

        [DataMember(Order = 2)]
        public EbDataSet Data { set; get; }
    }

    [DataContract]
    public class MobileFormDataRequest : EbServiceStackAuthRequest, IReturn<MobileFormDataResponse>
    {
        [DataMember(Order = 1)]
        public string MobilePageRefId { set; get; }

        [DataMember(Order = 2)]
        public int RowId { get; set; }

        [DataMember(Order = 3)]
        public int LocId { get; set; }
    }

    [DataContract]
    public class MobileFormDataResponse
    {
        [DataMember(Order = 1)]
        public string Message { set; get; }

        [DataMember(Order = 2)]
        public WebformData Data { set; get; }
    }

    [DataContract]
    public class MobileProfileData
    {
        [DataMember(Order = 1)]
        public string Message { set; get; }

        [DataMember(Order = 2)]
        public WebformData Data { set; get; }

        [DataMember(Order = 3)]
        public int RowId { set; get; }
    }

    public class MyActionsRequest : EbServiceStackAuthRequest, IReturn<MyActionsResponse>
    {

    }

    [DataContract]
    public class MyActionsResponse
    {
        [DataMember(Order = 1)]
        public List<EbMyActionsMobile> Actions { get; set; }

        public MyActionsResponse()
        {
            Actions = new List<EbMyActionsMobile>();
        }
    }

    public class ParticularActionsRequest : EbServiceStackAuthRequest, IReturn<ParticularActionResponse>
    {
        public int ActionId { set; get; }
    }

    [DataContract]
    public class ParticularActionResponse
    {
        [DataMember(Order = 1)]
        public EbMyActionsMobile Action { set; get; }

        [DataMember(Order = 1)]
        public MyActionInfoResponse ActionInfo { set; get; }
    }

    public class MyActionInfoRequest : EbServiceStackAuthRequest, IReturn<MyActionInfoResponse>
    {
        public int StageId { set; get; }

        public string WebFormRefId { set; get; }

        public int WebFormDataId { set; get; }
    }

    [DataContract]
    public class MyActionInfoResponse
    {
        [DataMember(Order = 1)]
        public string StageUniqueId { set; get; }

        [DataMember(Order = 2)]
        public string StageName { set; get; }

        [DataMember(Order = 3)]
        public List<EbStageActionsMobile> StageActions { set; get; }

        [DataMember(Order = 4)]
        public List<Param> Data { set; get; }

        public MyActionInfoResponse()
        {
            StageActions = new List<EbStageActionsMobile>();
        }
    }

    [DataContract]
    public class EbMobileSolutionData 
    {
        [DataMember(Order = 1)]
        public List<AppDataToMob> Applications { set; get; }

        [DataMember(Order = 2)]
        public List<EbLocation> Locations { get; set; }

        [DataMember(Order = 3)]
        public List<MobilePagesWraper> ProfilePages { set; get; }

        public EbMobileSolutionData()
        {
            Applications = new List<AppDataToMob>();
        }
    }

    public class MobileSolutionDataRequest : EbServiceStackAuthRequest, IReturn<EbMobileSolutionData>
    {
        public bool Export { set; get; }
    }
}
