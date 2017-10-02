using MongoDB.Bson;
using MongoDB.Driver.GridFS;
using ServiceStack;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ExpressBase.Objects.ServiceStack_Artifacts
{
    [DataContract]
    public class FilesInfo
    {

        [DataMember(Order = 1)]
        public string ObjectId { get; set; }

        [DataMember(Order = 2)]
        public BsonDocument MetaData { get; set; }

    }

    [DataContract]
    public class UploadFileRequest : EbServiceStackRequest
    {

        [DataMember(Order = 1)]
        public string FileName { get; set; }

        [DataMember(Order = 2)]
        public byte[] ByteArray { get; set; }

        [DataMember(Order = 3)]
        public BsonDocument MetaData { get; set; }

        [DataMember(Order = 4)]
        public bool IsAsync { get; set; }

        [DataMember(Order = 5)]
        public IDictionary<String, String> metaDataPair { get; set; }
    }

    [DataContract]
    public class UploadFileMqRequest : EbServiceStackRequest
    {
        [DataMember(Order = 1)]
        public string FileName { get; set; }

        [DataMember(Order = 2)]
        public byte[] ByteArray { get; set; }

        [DataMember(Order = 3)]
        public BsonDocument MetaData { get; set; }

        [DataMember(Order = 4)]
        public IDictionary<String, String> metaDataPair { get; set; }

    }

    public class UploadFileControllerResponse
    {
        public string Uploaded { get; set; }
    }

    public class UploadFileControllerError
    {
        public string Uploaded { get; set; }
    }

    [DataContract]
    public class DownloadFileRequest : EbServiceStackRequest
    {
        [DataMember(Order = 1)]
        public string ObjectId { get; set; }

    }

    [DataContract]
    public class FindFilesByTagRequest : EbServiceStackRequest, IReturn<FindFilesByTagResponse>
    {
        [DataMember(Order = 1)]
        public KeyValuePair<string, string> Filter { get; set; }

    }
   
    [DataContract]
    public class FindFilesByTagResponse
    {
        [DataMember(Order = 1)]
        public List<FilesInfo> FileList { get; set; }
    }

    [DataContract]
    public class ImageResizeMqRequest : EbServiceStackRequest
    {
        [DataMember(Order = 1)]
        public string ObjectId { get; set; }

        [DataMember(Order = 2)]
        public string FileName { get; set; }

        [DataMember(Order = 3)]
        public BsonDocument MetaData { get; set; }

        [DataMember(Order = 4)]
        public byte[] ImageByte { get; set; }

        [DataMember(Order = 5)]
        public IDictionary<String, String> metaDataPair { get; set; }

    }
}
