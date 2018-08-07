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

    public class GetOneFromAppstoreResponse:IEbSSResponse
    {
        [DataMember(Order =1)]
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

    public class AppStore
    {
        public int Id { get; set; }

        public string AppName { get; set; }

        public int Status { get; set; }

        public int Cost { get; set; }

        public string Json { get; set; }

        public string Currency { get; set; }

        public int CreatedBy { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
