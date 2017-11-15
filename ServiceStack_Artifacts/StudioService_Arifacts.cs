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
    public class EbObjectRelationsRequest : EbServiceStackRequest, IReturn<EbObjectRelationsResponse>
    {
        public string DominantId { get; set; }

        public int EbObjectType { get; set; }
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

    public class EbObjectAllVersionsRequest : EbServiceStackRequest, IReturn<EbObjectAllVersionsResponse>
    {
        public string RefId { get; set; }
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

    public class EbObjectParticularVersionRequest : EbServiceStackRequest, IReturn<EbObjectParticularVersionResponse>
    {
        public string RefId { get; set; }
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

    public class EbObjectNonCommitedVersionRequest : EbServiceStackRequest, IReturn<EbObjectNonCommitedVersionResponse>
    {
        public string RefId { get; set; }
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

    public class EbObjectLatestCommitedRequest : EbServiceStackRequest, IReturn<EbObjectLatestCommitedResponse>
    {
        public string RefId { get; set; }
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

    public class EbObjectObjListRequest : EbServiceStackRequest, IReturn<EbObjectObjListResponse>
    {
        public int EbObjectType { get; set; }
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

    public class EbObjectObjLisAllVerRequest : EbServiceStackRequest, IReturn<EbObjectObjListAllVerResponse>
    {
        public int EbObjectType { get; set; }
    }

    public class EbObjectObjListAllVerResponse : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public Dictionary<string, List<EbObjectWrapper>> Data { get; set; }

        [DataMember(Order = 2)]
        public string Token { get; set; }

        [DataMember(Order = 3)]
        public ResponseStatus ResponseStatus { get; set; }
    }

    public class EbObjectListRequest : EbServiceStackRequest, IReturn<EbObjectListResponse>
    {
        public int EbObjectType { get; set; }
    }

    public class EbObjectListResponse : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public List<EbObjectWrapper> Data { get; set; }

        [DataMember(Order = 2)]
        public string Token { get; set; }

        [DataMember(Order = 3)]
        public ResponseStatus ResponseStatus { get; set; }
    }

    public class EbObjectExploreObjectRequest : EbServiceStackRequest, IReturn<EbObjectExploreObjectResponse>
    {
        public int Id { get; set; }
    }

    public class EbObjectExploreObjectResponse : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public List<EbObjectWrapper> Data { get; set; }

        [DataMember(Order = 2)]
        public string Token { get; set; }

        [DataMember(Order = 3)]
        public ResponseStatus ResponseStatus { get; set; }
    }

     public class EbObjectUpdateDashboardRequest : EbServiceStackRequest, IReturn<EbObjectUpdateDashboardResponse>
    {
        public string Refid { get; set; }
    }
    public class EbObjectUpdateDashboardResponse : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public List<EbObjectWrapper> Data { get; set; }

        [DataMember(Order = 2)]
        public string Token { get; set; }

        [DataMember(Order = 3)]
        public ResponseStatus ResponseStatus { get; set; }
    }
    public class EbObjectStatusHistoryRequest : EbServiceStackRequest, IReturn<EbObjectStatusHistoryResponse>
    {
        public string RefId { get; set; }
    }

    public class EbObjectStatusHistoryResponse : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public List<EbObjectWrapper> Data { get; set; }

        [DataMember(Order = 2)]
        public string Token { get; set; }

        [DataMember(Order = 3)]
        public ResponseStatus ResponseStatus { get; set; }
    }

    public class EbObjectFetchLiveVersionRequest : EbServiceStackRequest, IReturn<EbObjectFetchLiveVersionResponse>
    {
        public int Id { get; set; }
    }

    public class EbObjectFetchLiveVersionResponse : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public List<EbObjectWrapper> Data { get; set; }

        [DataMember(Order = 2)]
        public string Token { get; set; }

        [DataMember(Order = 3)]
        public ResponseStatus ResponseStatus { get; set; }
    }
    [Route("/ebo", "POST")]
    public class EbObjectSaveOrCommitRequest : EbServiceStackRequest, IReturn<EbObjectSaveOrCommitResponse>
    {
        public bool IsSave { get; set; } // If (IsSave == true) Save else Commit

        public bool NeedRun { get; set; } // If (NeedRun == true) Save and Run else save Only

        public string RefId { get; set; } // (Id == 0) First Commit else Subsequent Commit

        public int EbObjectType { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Json { get; set; }

        public ObjectLifeCycleStatus Status { get; set; }

        public string ChangeLog { get; set; }

        public string Relations { get; set; }
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

    public class EbObject_CommitRequest : EbServiceStackRequest, IReturn<EbObject_CommitResponse>
    {
        public string RefId { get; set; } // (Id == 0) First Commit else Subsequent Commit

        public int EbObjectType { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Json { get; set; }

        public string ChangeLog { get; set; }

        public string Relations { get; set; }

        public string Tags { get; set; }

        public int AppId { get; set; }
    }

    [DataContract]
    public class EbObject_CommitResponse : IEbSSResponse
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

    public class EbObject_SaveRequest : EbServiceStackRequest, IReturn<EbObject_SaveResponse>
    {
        public string RefId { get; set; } // (Id == 0) First Commit else Subsequent Commit

        public int EbObjectType { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Json { get; set; }

        public string Relations { get; set; }

        public string Tags { get; set; }

        public int AppId { get; set; }
    }

    [DataContract]
    public class EbObject_SaveResponse : IEbSSResponse
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

    public class EbObject_Create_Major_VersionRequest : EbServiceStackRequest, IReturn<EbObject_Create_Major_VersionResponse>
    {
        public string RefId { get; set; } // (Id == 0) First Commit else Subsequent Commit

        public int EbObjectType { get; set; }

        public string Relations { get; set; }
    }

    [DataContract]
    public class EbObject_Create_Major_VersionResponse : IEbSSResponse
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

    public class EbObject_Create_Minor_VersionRequest : EbServiceStackRequest, IReturn<EbObject_Create_Minor_VersionResponse>
    {
        public string RefId { get; set; } // (Id == 0) First Commit else Subsequent Commit

        public int EbObjectType { get; set; }

        public string Relations { get; set; }
    }

    [DataContract]
    public class EbObject_Create_Minor_VersionResponse : IEbSSResponse
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


    public class EbObject_Create_New_ObjectRequest : EbServiceStackRequest, IReturn<EbObject_Create_New_ObjectResponse>
    {
        public string RefId { get; set; } // (Id == 0) First Commit else Subsequent Commit

        public string Name { get; set; }

        public int EbObjectType { get; set; }

        public string Description { get; set; }

        public string Json { get; set; }

        public ObjectLifeCycleStatus Status { get; set; }

        public string Relations { get; set; }

        public bool IsSave { get; set; }

        public string Tags { get; set; }

        public int AppId { get; set; }
    }

    [DataContract]
    public class EbObject_Create_New_ObjectResponse : IEbSSResponse
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

    public class EbObject_Create_Patch_VersionRequest : EbServiceStackRequest, IReturn<EbObject_Create_Patch_VersionResponse>
    {
        public string RefId { get; set; } // (Id == 0) First Commit else Subsequent Commit

        public int EbObjectType { get; set; }

        public string Relations { get; set; }
    }

    [DataContract]
    public class EbObject_Create_Patch_VersionResponse : IEbSSResponse
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

    public class EbObjectChangeStatusRequest : EbServiceStackRequest, IReturn<EbObjectChangeStatusResponse>
    {
        public string RefId { get; set; }

        public string ChangeLog { get; set; }

        public ObjectLifeCycleStatus Status { get; set; }
    }

    [DataContract]
    public class EbObjectChangeStatusResponse : IEbSSResponse
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

    public class EbObjectRunSqlFunctionRequest : EbServiceStackRequest, IReturn<EbObjectRunSqlFunctionResponse>
    {
        public string Json { get; set; }
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
        public string Status { get; set; }

        [DataMember(Order = 7)]
        public string VersionNumber { get; set; }

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

        [DataMember(Order = 13)]
        public bool WorkingMode { get; set; }

        [DataMember(Order = 14)]
        public string Json_wc { get; set; }

        [DataMember(Order = 15)]
        public string Json_lc { get; set; }

        [DataMember(Order =16)]
        public string[] Wc_All { get; set; }

        [DataMember(Order =17)]
        public int MajorVersionNumber { get; set; }

        [DataMember(Order = 18)]
        public int MinorVersionNumber { get; set; }

        [DataMember(Order = 19)]
        public int PatchVersionNumber { get; set; }

        [DataMember(Order = 19)]
        public string Tags { get; set; }

        [DataMember(Order = 20)]
        public string ProfileImage { get; set; }

        [DataMember(Order = 21)]
        public int AppId { get; set; }

        [DataMember(Order = 22)]
        public EbObjectWrapper_Dashboard Dashboard_Tiles { get; set; }
        public EbObjectWrapper() { }
    }

    [DataContract]
    public class EbObjectWrapper_Dashboard
    {
        [DataMember(Order = 1)]
        public string LiveVersionRefid { get; set; }

        [DataMember(Order = 2)]
        public string LiveVersionNumber { get; set; }

        [DataMember(Order = 3)]
        public DateTime LiveVersionCommit_ts { get; set; }

        [DataMember(Order = 4)]
        public string LiveVersionCommitby_Name { get; set; } 

        [DataMember(Order = 5)]
        public int LiveVersionCommitby_Id { get; set; }

        [DataMember(Order = 6)]
        public string LiveVersion_Status { get; set; }

        [DataMember(Order = 7)]
        public string LastCommitedVersionRefid { get; set; }

        [DataMember(Order = 8)]
        public string LastCommitedVersionNumber { get; set; }

        [DataMember(Order = 9)]
        public DateTime LastCommitedVersionCommit_ts { get; set; }

        [DataMember(Order = 10)]
        public string LastCommitedby_Name { get; set; }

        [DataMember(Order = 11)]
        public string LastCommitedVersion_Status { get; set; }

        [DataMember(Order = 12)]
        public int LastCommitedby_Id { get; set; }

        public EbObjectWrapper_Dashboard() { }
    }
    }
