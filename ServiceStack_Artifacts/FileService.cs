using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExpressBase.Objects.ServiceStack_Artifacts
{
    public class UploadFileRequest : IEbSSRequest
    {
        public string FileName { get; set; }

        public byte[] ByteArray { get; set; }

        public BsonDocument MetaData { get; set; }

        public string TenantAccountId { get; set; }

        public int UserId { get; set; }
    }

    public class UploadFileMqRequest : IEbSSRequest
    {
        public string FileName { get; set; }

        public byte[] ByteArray { get; set; }

        public BsonDocument MetaData { get; set; }

        public string TenantAccountId { get; set; }

        public int UserId { get; set; }
    }
}
