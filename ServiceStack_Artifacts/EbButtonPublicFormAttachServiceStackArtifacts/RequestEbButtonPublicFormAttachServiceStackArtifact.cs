using ExpressBase.Common.EbServiceStack.ReqNRes;
using ServiceStack;

namespace ExpressBase.Objects.ServiceStack_Artifacts.EbButtonPublicFormAttachServiceStackArtifacts
{
    
    public class RequestEbButtonPublicFormAttachServiceStackArtifact : EbServiceStackAuthRequest, IReturn<ResponseEbButtonPublicFormAttachServiceStackArtifact>
    {
        
        public string SourceFormRefId { get; set; }
        public string PublicFormRefId { get; set; }

        public int SourceFormDataId { get; set; }
    }
}
