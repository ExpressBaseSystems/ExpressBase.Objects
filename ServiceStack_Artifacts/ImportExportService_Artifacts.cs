using ExpressBase.Common.EbServiceStack.ReqNRes;
using ServiceStack;
using System.Runtime.Serialization;

namespace ExpressBase.Objects.ServiceStack_Artifacts
{
    public class ExportApplicationMqRequest : EbServiceStackAuthRequest, IReturn<GetAllFromAppstoreResponse>
    {
        [DataMember(Order = 1)]
        public string Refids { get; set; }

        [DataMember(Order = 2)]
        public int AppId { get; set; }
    }

    public class ExportApplicationRequest : EbMqRequest
    {
        [DataMember(Order = 1)]
        public string Refids { get; set; }

        [DataMember(Order = 2)]
        public int AppId { get; set; }
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
    }

    public class ImportApplicationRequest : EbMqRequest
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
}
