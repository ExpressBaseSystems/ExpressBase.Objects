using ExpressBase.Common;
using ExpressBase.Common.EbServiceStack.ReqNRes;
using ServiceStack;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace ExpressBase.Objects.ServiceStack_Artifacts
{
    public class SaveToAppStoreRequest : EbServiceStackAuthRequest, IReturn<SaveToAppStoreResponse>
    {

        public AppStore Store { get; set; }
    }

    public class SaveToAppStoreResponse : IEbSSResponse
    {
        [DataMember(Order = 2)]
        public string Token { get; set; }

        [DataMember(Order = 3)]
        public ResponseStatus ResponseStatus { get; set; }
    }

    public class GetOneFromAppStoreRequest : EbServiceStackAuthRequest, IReturn<GetOneFromAppstoreResponse>
    {
        public int Id { get; set; }
    }

    public class GetOneFromAppstoreResponse : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public AppWrapper Wrapper { get; set; }

        [DataMember(Order = 2)]
        public string Token { get; set; }

        [DataMember(Order = 3)]
        public ResponseStatus ResponseStatus { get; set; }
    }
    public class GetAllFromAppStoreInternalRequest : EbServiceStackAuthRequest, IReturn<GetAllFromAppstoreResponse>
    {
    }
    public class GetAllFromAppStoreExternalRequest : EbServiceStackAuthRequest, IReturn<GetAllFromAppstoreResponse>
    {
    }

    public class GetAllFromAppstoreResponse : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public List<AppStore> Apps { get; set; }

        [DataMember(Order = 2)]
        public string Token { get; set; }

        [DataMember(Order = 3)]
        public ResponseStatus ResponseStatus { get; set; }
    }

    public class ShareToPublicRequest : EbServiceStackAuthRequest, IReturn<ShareToPublicResponse>
    {
        public AppStore Store { get; set; }
    }

    public class ShareToPublicResponse : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public string Token { get; set; }

        [DataMember(Order = 2)]
        public ResponseStatus ResponseStatus { get; set; }

        [DataMember(Order = 3)]
        public int ReturningId { get; set; }
    }

    public class GetAppDetailsRequest : EbServiceStackAuthRequest, IReturn<GetAppDetailsResponse>
    {
        public int Id { get; set; }
    }

    public class GetAppDetailsResponse : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public string Token { get; set; }

        [DataMember(Order = 2)]
        public ResponseStatus ResponseStatus { get; set; }

        [DataMember(Order = 3)]
        public List<AppStore> StoreCollection { get; set; }

    }

    public class AppStore
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int Status { get; set; }

        public decimal Cost { get; set; }

        public string Json { get; set; }

        public string Currency { get; set; }

        public int CreatedBy { get; set; }

        public DateTime CreatedAt { get; set; }

        public int AppType { get; set; }

        public string Description { get; set; }

        public string Icon { get; set; }

        public string SolutionName { get; set; }

        public string TenantName { get; set; }

        public string SolutionId { get; set; }

        public string Title { get; set; }

        public string IsFree { get; set; }

        public string ShortDesc { get; set; }

        public string Tags { get; set; }

        public string DetailedDesc { get; set; }

        public string DemoLinks { get; set; }

        public string VideoLinks { get; set; }

        public string Images { get; set; }

        public string PricingDesc { get; set; }

        public int DetailId { get; set; }
    }
}
