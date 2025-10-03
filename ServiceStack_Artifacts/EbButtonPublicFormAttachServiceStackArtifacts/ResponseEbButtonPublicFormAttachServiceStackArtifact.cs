using ExpressBase.Objects.Dtos;

namespace ExpressBase.Objects.ServiceStack_Artifacts.EbButtonPublicFormAttachServiceStackArtifacts
{
    public class ResponseEbButtonPublicFormAttachServiceStackArtifact
    {
        public bool Success { get; set; }
        public EbButtonPublicFormAttachServiceDto EbButtonPublicFormAttachServiceDto { get; set; }
        public ErrorEbButtonPublicFormAttachServiceStackArtifact Error { get; set; }
    }
}
