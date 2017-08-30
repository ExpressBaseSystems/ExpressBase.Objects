using ExpressBase.Common.Objects;
using ServiceStack;
using ServiceStack.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace ExpressBase.Objects.ServiceStack_Artifacts
{
    public class EbObjectRelationsRequest : IReturn<EbObjectRelationsResponse>, IEbSSRequest
    {
        public string DominantId { get; set; }

        public int EbObjectType { get; set; }

        public string Token { get; set; }

        public string TenantAccountId { get; set; }

        public int UserId { get; set; }
    }

    public class EbObjectRelationsResponse : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public List<EbObjectWrapper> Data { get; set; }

        [DataMember(Order = 2)]
        public string Token { get; set; }

        [DataMember(Order = 3)]
        public ResponseStatus ResponseStatus { get; set; }
    }

    public class EbObjectAllVersionsRequest : IReturn<EbObjectAllVersionsResponse>, IEbSSRequest
    {
        public string RefId { get; set; }

        public string Token { get; set; }

        public string TenantAccountId { get; set; }

        public int UserId { get; set; }

    }

    public class EbObjectAllVersionsResponse : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public List<EbObjectWrapper> Data { get; set; }

        [DataMember(Order = 2)]
        public string Token { get; set; }

        [DataMember(Order = 3)]
        public ResponseStatus ResponseStatus { get; set; }
    }

    public class EbObjectParticularVersionRequest : IReturn<EbObjectParticularVersionResponse>, IEbSSRequest
    {
        public string RefId { get; set; }

        public string Token { get; set; }

        public string TenantAccountId { get; set; }

        public int UserId { get; set; }

    }

    public class EbObjectParticularVersionResponse : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public List<EbObjectWrapper> Data { get; set; }

        [DataMember(Order = 2)]
        public string Token { get; set; }

        [DataMember(Order = 3)]
        public ResponseStatus ResponseStatus { get; set; }
    }

    public class EbObjectNonCommitedVersionRequest : IReturn<EbObjectNonCommitedVersionResponse>, IEbSSRequest
    {
        public string RefId { get; set; }

        public string Token { get; set; }

        public string TenantAccountId { get; set; }

        public int UserId { get; set; }

    }

    public class EbObjectNonCommitedVersionResponse : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public List<EbObjectWrapper> Data { get; set; }

        [DataMember(Order = 2)]
        public string Token { get; set; }

        [DataMember(Order = 3)]
        public ResponseStatus ResponseStatus { get; set; }
    }

    public class EbObjectLatestCommitedRequest : IReturn<EbObjectLatestCommitedResponse>, IEbSSRequest
    {
        public string RefId { get; set; }

        public string Token { get; set; }

        public string TenantAccountId { get; set; }

        public int UserId { get; set; }

    }

    public class EbObjectLatestCommitedResponse : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public List<EbObjectWrapper> Data { get; set; }

        [DataMember(Order = 2)]
        public string Token { get; set; }

        [DataMember(Order = 3)]
        public ResponseStatus ResponseStatus { get; set; }
    }

    public class EbObjectObjListRequest : IReturn<EbObjectObjListResponse>, IEbSSRequest
    {
        public int EbObjectType { get; set; }

        public string Token { get; set; }

        public string TenantAccountId { get; set; }

        public int UserId { get; set; }

    }

    public class EbObjectObjListResponse : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public List<EbObjectWrapper> Data { get; set; }

        [DataMember(Order = 2)]
        public string Token { get; set; }

        [DataMember(Order = 3)]
        public ResponseStatus ResponseStatus { get; set; }
    }

    [Route("/ebo", "POST")]
    public class EbObjectSaveOrCommitRequest : IReturn<EbObjectSaveOrCommitResponse>, IEbSSRequest
    {
        public bool IsSave { get; set; } // If (IsSave == true) Save else Commit

        public bool NeedRun { get; set; } // If (NeedRun == true) Save and Run else save Only

        public string RefId { get; set; } // (Id == 0) First Commit else Subsequent Commit

        public int EbObjectType { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public byte[] Bytea { get; set; }

        public string Json { get; set; }

        public EbObject EbObject { get; set; }

        public ObjectLifeCycleStatus Status { get; set; }

        public string ChangeLog { get; set; }

        public string Relations { get; set; }

        public string Token { get; set; }

        public string TenantAccountId { get; set; }

        public int UserId { get; set; }
    }

    [DataContract]
    public class EbObjectSaveOrCommitResponse : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public int Id { get; set; }

        [DataMember(Order = 2)]
        public string Token { get; set; }

        [DataMember(Order = 3)]
        public ResponseStatus ResponseStatus { get; set; }

        [DataMember(Order = 4)]
        public string RefId { get; set; }
    }

    public class EbObjectFirstCommitRequest : IReturn<EbObjectFirstCommitResponse>, IEbSSRequest
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public int EbObjectType { get; set; }

        public ObjectLifeCycleStatus Status { get; set; }

        public string Json { get; set; }

        public string Relations { get; set; }

        public EbObject EbObject { get; set; }

        public string Token { get; set; }

        public string TenantAccountId { get; set; }

        public int UserId { get; set; }
    }

    [DataContract]
    public class EbObjectFirstCommitResponse : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public int Id { get; set; }

        [DataMember(Order = 2)]
        public string Token { get; set; }

        [DataMember(Order = 3)]
        public ResponseStatus ResponseStatus { get; set; }

        [DataMember(Order = 4)]
        public string RefId { get; set; }        
    }

    public class EbObjectSubsequentCommitRequest : IReturn<EbObjectSubsequentCommitResponse>, IEbSSRequest
    {
        public string RefId { get; set; } // (Id == 0) First Commit else Subsequent Commit

        public int EbObjectType { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public ObjectLifeCycleStatus Status { get; set; }

        public string Json { get; set; }

        public string ChangeLog { get; set; }

        public string Relations { get; set; }

        public bool IsSave { get; set; } // If (IsSave == true) Save else Commit

        public EbObject EbObject { get; set; }

        public string Token { get; set; }

        public string TenantAccountId { get; set; }

        public int UserId { get; set; }
    }

    [DataContract]
    public class EbObjectSubsequentCommitResponse : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public int Id { get; set; }

        [DataMember(Order = 2)]
        public string Token { get; set; }

        [DataMember(Order = 3)]
        public ResponseStatus ResponseStatus { get; set; }

        [DataMember(Order = 4)]
        public string RefId { get; set; }        
    }

    public class EbObjectRunSqlFunctionRequest : IReturn<EbObjectRunSqlFunctionResponse>, IEbSSRequest
    {
        public string Json { get; set; }

        public string Token { get; set; }

        public string TenantAccountId { get; set; }

        public int UserId { get; set; }
    }

        [DataContract]
    public class EbObjectRunSqlFunctionResponse : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public int Id { get; set; }

        [DataMember(Order = 2)]
        public string Token { get; set; }

        [DataMember(Order = 3)]
        public ResponseStatus ResponseStatus { get; set; }

        [DataMember(Order = 4)]
        public string RefId { get; set; }
    }

    [DataContract]
    public class EbObjectWrapper
    {
        [DataMember(Order = 1)]
        public int Id { get; set; }

        [DataMember(Order = 2)]
        public EbObjectType EbObjectType { get; set; }

        [DataMember(Order = 3)]
        public string Name { get; set; }

        [DataMember(Order = 4)]
        public string Description { get; set; }

        [DataMember(Order = 5)]
        public byte[] Bytea { get; set; }

        [DataMember(Order = 6)]
        public ObjectLifeCycleStatus Status { get; set; }

        [DataMember(Order = 7)]
        public int VersionNumber { get; set; }

        [DataMember(Order = 8)]
        public string ChangeLog { get; set; }

        [DataMember(Order = 9)]
        public string CommitUname { get; set; }

        [DataMember(Order = 10)]
        public DateTime CommitTs { get; set; }

        [DataMember(Order = 11)]
        public string Json { get; set; }

        [DataMember(Order = 12)]
        public string RefId { get; set; }

        public EbObjectWrapper() { }
    }
}
