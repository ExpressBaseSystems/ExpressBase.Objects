using ExpressBase.Common.EbServiceStack.ReqNRes;
using ServiceStack;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ExpressBase.Objects.ServiceStack_Artifacts
{
    public class ExportApplicationMqRequest : EbServiceStackAuthRequest, IReturn<GetAllFromAppstoreResponse>
    {
        [DataMember(Order = 1)]
        public Dictionary<int,List<string>> AppCollection { get; set; }

        [DataMember(Order = 2)]
        public string PackageName { get; set; }

        [DataMember(Order = 3)]
        public string PackageDescription { get; set; }

        [DataMember(Order = 4)]
        public string PackageIcon { get; set; }

    }

    public class ExportApplicationRequest : EbMqRequest
    {
        [DataMember(Order = 1)]
        public Dictionary<int, List<string>> AppCollection { get; set; }

        [DataMember(Order = 2)]
        public string PackageName { get; set; }

        [DataMember(Order = 3)]
        public string PackageDescription { get; set; }

        [DataMember(Order = 4)]
        public string PackageIcon { get; set; }

        [DataMember(Order = 5)]
        public string SubscriptionId { get; set; }
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

    public class ImportApplicationMqRequest : EbServiceStackAuthRequest, IReturn<GetAllFromAppstoreResponse>
    {
        [DataMember(Order = 1)]
        public int Id { get; set; }

        [DataMember(Order = 2)]
        public string SelectedSolutionId { get; set; }

        [DataMember(Order = 3)]
        public bool IsDemoApp { get; set; }
    }

    public class ImportApplicationRequest : EbMqRequest
    {
        [DataMember(Order = 1)]
        public int Id { get; set; }

        [DataMember(Order = 2)]
        public bool IsDemoApp { get; set; }

        [DataMember(Order = 3)]
        public string SelectedSolutionId { get; set; }

        [DataMember(Order = 4)]
        public string SubscriptionId { get; set; }
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
}
