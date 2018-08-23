using ExpressBase.Common.EbServiceStack.ReqNRes;
using ServiceStack;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace ExpressBase.Objects.ServiceStack_Artifacts
{
    public class SaveToAppStoreRequest : EbServiceStackRequest, IReturn<SaveToAppStoreResponse>
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

    public class GetOneFromAppStoreRequest : EbServiceStackRequest, IReturn<GetOneFromAppstoreResponse>
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

    public class GetAllFromAppStoreRequest : EbServiceStackRequest, IReturn<GetAllFromAppstoreResponse>
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

    public class ExportApplicationRequest : EbServiceStackRequest, IReturn<GetAllFromAppstoreResponse>
    {
        [DataMember(Order = 1)]
        public string Refids { get; set; }
    }

    public class ExportApplicationResponse : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public string Token { get; set; }

        [DataMember(Order = 2)]
        public ResponseStatus ResponseStatus { get; set; }

        [DataMember(Order = 3)]
        public string Result { get; set; }
    }

    public class ImportApplicationRequest : EbServiceStackRequest, IReturn<GetAllFromAppstoreResponse>
    {
        [DataMember(Order = 1)]
        public int Id { get; set; }
    }

    public class ImportApplicationResponse : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public string Token { get; set; }

        [DataMember(Order = 2)]
        public ResponseStatus ResponseStatus { get; set; }

        [DataMember(Order = 3)]
        public string Result { get; set; }

    }

    public class ShareToPublicRequest : EbServiceStackRequest, IReturn<ShareToPublicResponse>
    {
        public int AppStoreId { get; set; }

        public string Title { get; set; }

        public string IsFree { get; set; }

        public int Price { get; set; }

        public string ShortDesc { get; set; }

        public string Tags { get; set; }

        public string DetailedDesc { get; set; }

        public string DemoLinks { get; set; }

        public string VideoLinks { get; set; }

        public string Images { get; set; }

        public string PricingDesc { get; set; }

        public int Cost { get; set; }        
    }

    public class ShareToPublicResponse : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public string Token { get; set; }

        [DataMember(Order = 2)]
        public ResponseStatus ResponseStatus { get; set; }
    }
    public class AppStore
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int Status { get; set; }

        public int Cost { get; set; }

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
    }
}
