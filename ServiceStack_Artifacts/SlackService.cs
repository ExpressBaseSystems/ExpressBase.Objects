using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Text;

namespace ExpressBase.Objects.ServiceStack_Artifacts
{
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
        [DataMember(Order =1)]
        public string Token { get; set; }

        [DataMember(Order = 2)]
        public string Channel { get; set; }

        [DataMember(Order = 3)]
        public string Text { get; set; }

        [DataMember(Order = 4)]
        public string Content { get; set; }

        [DataMember(Order = 5)]
        public SlackFile SlackFile { get; set; }
    }

    public class SlackPostRequest : EbServiceStackRequest
    {
        public SlackPayload Payload { get; set; }

        public int PostType { get; set; }
    }

    public class SlackPostMqRequest : EbServiceStackRequest
    {
        public SlackPayload Payload { get; set; }

        public int PostType { get; set; }
    }
}
