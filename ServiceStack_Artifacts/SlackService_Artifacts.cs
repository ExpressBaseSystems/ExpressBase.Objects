using ExpressBase.Common.Data;
using ExpressBase.Common.EbServiceStack.ReqNRes;
using Newtonsoft.Json;
using ServiceStack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Text;

namespace ExpressBase.Objects.ServiceStack_Artifacts
{
    public class SlackPostRequest : EbServiceStackAuthRequest
    {
        public SlackPayload Payload { get; set; }

        public int PostType { get; set; }
    }

    public class SlackAuthRequest : EbServiceStackAuthRequest
    {
        public SlackJson SlackJson { get; set; }

        public bool IsNew { get; set; }
    }

    [DataContract]
    public class SlackJson
    {
        [DataMember(Order = 1)]
        [JsonProperty("ok")]
        public bool Ok { get; set; }

        [DataMember(Order = 2)]
        [JsonProperty("team_id")]
        public string TeamId { get; set; }

        [DataMember(Order = 3)]
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }

        [DataMember(Order = 4)]
        [JsonProperty("scope")]
        public string Scope { get; set; }

        [DataMember(Order = 5)]
        [JsonProperty("team_name")]
        public string TeamName { get; set; }

        [DataMember(Order = 6)]
        [JsonProperty("user_id")]
        public string UserId { get; set; }
    }

    public class SlackAuthAsyncRequest : EbServiceStackAuthRequest
    {
        public SlackJson SlackJson { get; set; }

        public bool IsNew { get; set; }
    }

    [DataContract]
    public class SlackFile
    {
        [DataMember(Order = 1)]
        public string FileName;

        [DataMember(Order = 2)]
        public byte[] FileByte;

        [DataMember(Order = 3)]
        public string FileType;

        [DataMember(Order = 4)]
        public string Title;

        [DataMember(Order = 5)]
        public string InitalComment;
    }

    [DataContract]
    public class SlackPayload
    {

        [DataMember(Order = 1)]
        public string Channel { get; set; }

        [DataMember(Order = 2)]
        public string Text { get; set; }

        [DataMember(Order = 3)]
        public SlackFile SlackFile { get; set; }
    }

    public class SlackCreateRequest : EbServiceStackAuthRequest, IReturn<SlackAttachmenResponse>
    {
        [DataMember(Order = 1)]
        public int ObjId { get; set; }

        [DataMember(Order = 2)]
        public List<Param> Params { get; set; }

        [DataMember(Order = 3)]
        public string Message { get; set; }

        [DataMember(Order = 4)]
        public byte[] AttachmentReport { get; set; }

        [DataMember(Order = 5)]
        public string AttachmentName { get; set; }

        [DataMember(Order = 6)]
        public string To { get; set; }
    }

    public class SlackAttachmenResponse
    {
        [DataMember(Order = 1)]
        public ResponseStatus ResponseStatus { get; set; }
    }

    public class SlackPostAsyncRequest : EbServiceStackAuthRequest
    {
        public SlackPayload Payload { get; set; }

        public int PostType { get; set; }
    }
}
