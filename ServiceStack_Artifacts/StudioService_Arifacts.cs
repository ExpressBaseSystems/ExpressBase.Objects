﻿using ExpressBase.Common.Data;
using ExpressBase.Common.EbServiceStack.ReqNRes;
using ExpressBase.Common.Objects;
using ExpressBase.Common.SqlProfiler;
using ExpressBase.Common.Structures;
using ServiceStack;
using ServiceStack.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace ExpressBase.Objects.ServiceStack_Artifacts
{
    public class EbObjectRelationsRequest : EbServiceStackAuthRequest, IReturn<EbObjectRelationsResponse>
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

    public class EbObjectAllVersionsRequest : EbServiceStackAuthRequest, IReturn<EbObjectAllVersionsResponse>
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

    public class EbObjectParticularVersionRequest : EbServiceStackAuthRequest, IReturn<EbObjectParticularVersionResponse>
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
    public class EbObjectLatestCommitedRequest : EbServiceStackAuthRequest, IReturn<EbObjectLatestCommitedResponse>
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

    public class EbObjectObjListRequest : EbServiceStackAuthRequest, IReturn<EbObjectObjListResponse>
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
    public class EbAllObjNVerRequest : EbServiceStackAuthRequest, IReturn<EbObjectObjListAllVerResponse>
    {
        public string ObjectIds { get; set; }
    }

    public class EbObjectObjLisAllVerRequest : EbServiceStackAuthRequest, IReturn<EbObjectObjListAllVerResponse>
    {
        public int EbObjectType { get; set; }
    }
    public class PublicObjListAllVerRequest : EbServiceStackAuthRequest, IReturn<EbObjectObjListAllVerResponse>
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

    public class GetFormBuilderRelatedDataRqst : EbServiceStackAuthRequest, IReturn<GetFormBuilderRelatedDataResp>
    {
        public string EbObjectRefId { get; set; }

        public int EbObjType { get; set; }
    }

    public class GetFormBuilderRelatedDataResp : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public Dictionary<string, List<EbObjectWrapper>> Data { get; set; }

        [DataMember(Order = 2)]
        public Dictionary<int, string> Roles { get; set; }

        [DataMember(Order = 3)]
        public Dictionary<int, string> UserGroups { get; set; }

        [DataMember(Order = 6)]
        public Dictionary<int, string> UserTypes { get; set; }

        [DataMember(Order = 4)]
        public string Token { get; set; }

        [DataMember(Order = 5)]
        public ResponseStatus ResponseStatus { get; set; }
    }
    public class GetAllLiveObjectsRqst : EbServiceStackAuthRequest, IReturn<GetAllLiveObjectsResp>
    {
        public List<int> Typelist { get; set; }
    }

    public class GetAllCommitedObjectsRqst : EbServiceStackAuthRequest, IReturn<GetAllLiveObjectsResp>
    {
        public List<int> Typelist { get; set; }
    }

    public class GetAllLiveObjectsResp : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public Dictionary<string, List<EbObjectWrapper>> Data { get; set; }

        [DataMember(Order = 2)]
        public string Token { get; set; }

        [DataMember(Order = 3)]
        public ResponseStatus ResponseStatus { get; set; }
    }

    public class GetAllCommitedObjectsResp : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public Dictionary<string, List<EbObjectWrapper>> Data { get; set; }

        [DataMember(Order = 2)]
        public string Token { get; set; }

        [DataMember(Order = 3)]
        public ResponseStatus ResponseStatus { get; set; }
    }

    public class EbObjectListRequest : EbServiceStackAuthRequest, IReturn<EbObjectListResponse>
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

    public class EbObjectExploreObjectRequest : EbServiceStackAuthRequest, IReturn<EbObjectExploreObjectResponse>
    {
        public int Id { get; set; }
    }

    public class EbObjectExploreObjectResponse : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public EbObjectWrapper Data { get; set; }

        [DataMember(Order = 2)]
        public string Token { get; set; }

        [DataMember(Order = 3)]
        public ResponseStatus ResponseStatus { get; set; }
    }

    public class EbObjectUpdateDashboardRequest : EbServiceStackAuthRequest, IReturn<EbObjectUpdateDashboardResponse>
    {
        public string Refid { get; set; }
    }
    public class EbObjectUpdateDashboardResponse : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public EbObjectWrapper Data { get; set; }

        [DataMember(Order = 2)]
        public string Token { get; set; }

        [DataMember(Order = 3)]
        public ResponseStatus ResponseStatus { get; set; }
    }
    public class EbObjectStatusHistoryRequest : EbServiceStackAuthRequest, IReturn<EbObjectStatusHistoryResponse>
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

    public class EbObjectFetchLiveVersionRequest : EbServiceStackAuthRequest, IReturn<EbObjectFetchLiveVersionResponse>
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
    public class EbObjectSaveOrCommitRequest : EbServiceStackAuthRequest, IReturn<EbObjectSaveOrCommitResponse>
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

    public class EbObject_CommitRequest : EbServiceStackAuthRequest, IReturn<EbObject_CommitResponse>
    {
        public string RefId { get; set; } // (Id == 0) First Commit else Subsequent Commit

        public int EbObjectType { get; set; }

        public string Name { get; set; }

        public string DisplayName { get; set; }

        public string Description { get; set; }

        public string Json { get; set; }

        public string ChangeLog { get; set; }

        public string Relations { get; set; }

        public string Tags { get; set; }

        public string Apps { get; set; }

        public bool HideInMenu { get; set; }
    }

    [DataContract]
    public class EbObject_CommitResponse : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public string Message { get; set; }

        [DataMember(Order = 2)]
        public string Token { get; set; }

        [DataMember(Order = 3)]
        public ResponseStatus ResponseStatus { get; set; }

        [DataMember(Order = 4)]
        public string RefId { get; set; }
    }

    public class EbObject_SaveRequest : EbServiceStackAuthRequest, IReturn<EbObject_SaveResponse>
    {
        public string RefId { get; set; } // (Id == 0) First Commit else Subsequent Commit

        public int EbObjectType { get; set; }

        public string Name { get; set; }

        public string DisplayName { get; set; }

        public string Description { get; set; }

        public string Json { get; set; }

        public string Relations { get; set; }

        public string Tags { get; set; }

        public string Apps { get; set; }

        public bool IsImport { get; set; }

        public bool HideInMenu { get; set; }
    }

    [DataContract]
    public class EbObject_SaveResponse : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public string Message { get; set; }
        //public int Id { get; set; }

        [DataMember(Order = 2)]
        public string Token { get; set; }

        [DataMember(Order = 3)]
        public ResponseStatus ResponseStatus { get; set; }

        [DataMember(Order = 4)]
        public string RefId { get; set; }
    }

    public class EbObject_Create_Major_VersionRequest : EbServiceStackAuthRequest, IReturn<EbObject_Create_Major_VersionResponse>
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

    public class EbObject_Create_Minor_VersionRequest : EbServiceStackAuthRequest, IReturn<EbObject_Create_Minor_VersionResponse>
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


    public class EbObject_Create_New_ObjectRequest : EbServiceStackAuthRequest, IReturn<EbObject_Create_New_ObjectResponse>
    {
        public string RefId { get; set; } // (Id == 0) First Commit else Subsequent Commit

        public string Name { get; set; }

        public string DisplayName { get; set; }

        public int EbObjectType { get; set; }

        public string Description { get; set; }

        public string Json { get; set; }

        public ObjectLifeCycleStatus Status { get; set; }

        public string Relations { get; set; }

        public bool IsSave { get; set; }

        public string Tags { get; set; }

        public string Apps { get; set; }

        public string SourceSolutionId { get; set; }

        public string SourceObjId { get; set; }

        public string SourceVerID { get; set; }

        public bool IsImport { get; set; }

        public bool HideInMenu { get; set; }
    }

    [DataContract]
    public class EbObject_Create_New_ObjectResponse : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public int Id { get; set; }

        [DataMember(Order = 2)]
        public string Message { get; set; }

        [DataMember(Order = 3)]
        public string Token { get; set; }

        [DataMember(Order = 4)]
        public ResponseStatus ResponseStatus { get; set; }

        [DataMember(Order = 5)]
        public string RefId { get; set; }
    }

    public class EbObject_Create_Patch_VersionRequest : EbServiceStackAuthRequest, IReturn<EbObject_Create_Patch_VersionResponse>
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

    public class EbObjectChangeStatusRequest : EbServiceStackAuthRequest, IReturn<EbObjectChangeStatusResponse>
    {
        public string RefId { get; set; }

        public string ChangeLog { get; set; }

        public ObjectLifeCycleStatus Status { get; set; }
    }

    [DataContract]
    public class EbObjectChangeStatusResponse : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public ResponseStatus ResponseStatus { get; set; }

        [DataMember(Order = 2)]
        public bool Response { get; set; }
    }

    public class RunSqlFunctionRequest : EbServiceStackAuthRequest, IReturn<RunSqlFunctionResponse>
    {
        public string Json { get; set; }
    }

    [DataContract]
    public class RunSqlFunctionResponse : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public int Status { get; set; }

        [DataMember(Order = 2)]
        public ResponseStatus ResponseStatus { get; set; }
    }

    public class ChangeObjectAccessRequest : EbServiceStackAuthRequest, IReturn<ChangeObjectAccessResponse>
    {
        public int ObjId { get; set; }

        public int Status { get; set; }
    }

    public class ChangeObjectAccessResponse : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public bool Status { get; set; }

        [DataMember(Order = 2)]
        public ResponseStatus ResponseStatus { get; set; }
    }
    public class DeleteEbObjectRequest : EbServiceStackAuthRequest, IReturn<DeleteEbObjectResponse>
    {
        public int ObjId { get; set; }
    }

    public class DeleteEbObjectResponse : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public int RowsDeleted { get; set; }

        [DataMember(Order = 2)]
        public ResponseStatus ResponseStatus { get; set; }
    }

    public class CloneEbObjectRequest : EbServiceStackAuthRequest, IReturn<CloneEbObjectResponse>
    {
        public string RefId { get; set; }

        public string Apps { get; set; }
    }

    public class CloneEbObjectResponse : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public bool Status { get; set; }

        [DataMember(Order = 2)]
        public int ObjId { get; set; }

        [DataMember(Order = 3)]
        public int ObjectType { get; set; }

        [DataMember(Order = 4)]
        public ResponseStatus ResponseStatus { get; set; }

        [DataMember(Order = 5)]
        public string ClonedRefid { get; set; }

        [DataMember(Order = 6)]
        public string Message { get; set; }
    }

    public class EnableLogRequest : EbServiceStackAuthRequest, IReturn<EnableLogResponse>
    {
        public int ObjId { get; set; }
        public bool Islog { get; set; }
    }

    public class EnableLogResponse : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public int RowsDeleted { get; set; }

        [DataMember(Order = 2)]
        public ResponseStatus ResponseStatus { get; set; }
    }

    public class GetRefIdByVerIdRequest : EbServiceStackAuthRequest, IReturn<GetRefIdByVerIdResponse>
    {
        public int ObjVerId { get; set; }
    }

    public class GetRefIdByVerIdResponse : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public string RefId { get; set; }

        [DataMember(Order = 2)]
        public ResponseStatus ResponseStatus { get; set; }
    }

    [DataContract]
    public class EbObjectWrapper
    {
        [DataMember(Order = 1)]
        public int Id { get; set; }

        [DataMember(Order = 2)]
        public int EbObjectType { get; set; }

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

        [DataMember(Order = 16)]
        public string[] Wc_All { get; set; }

        [DataMember(Order = 17)]
        public string Tags { get; set; }

        [DataMember(Order = 18)]
        public int CommitUId { get; set; }

        [DataMember(Order = 19)]
        public string Apps { get; set; }

        [DataMember(Order = 20)]
        public EbObj_Dashboard Dashboard_Tiles { get; set; }

        [DataMember(Order = 21)]
        public string DisplayName { get; set; }

        [DataMember(Order = 22)]
        public bool IsLogEnabled { get; set; }

        [DataMember(Order = 23)]

        public bool IsPublic { get; set; }

        public EbObjectWrapper() { }
    }

    [DataContract]
    public class EbObj_Dashboard
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

        [DataMember(Order = 13)]
        public int OwnerUid { get; set; }

        [DataMember(Order = 14)]
        public string OwnerName { get; set; }

        [DataMember(Order = 15)]
        public DateTime OwnerTs { get; set; }

        [DataMember(Order = 16)]
        public int MajorVersionNumber { get; set; }

        [DataMember(Order = 17)]
        public int MinorVersionNumber { get; set; }

        [DataMember(Order = 18)]
        public int PatchVersionNumber { get; set; }

        [DataMember(Order = 19)]
        public List<RelatedObject> Dependants { get; set; }

        [DataMember(Order = 18)]
        public List<RelatedObject> Dominants { get; set; }

        public EbObj_Dashboard() { }
    }

    public class RelatedObject
    {
        public string Refid { get; set; }

        public string DisplayName { get; set; }

        public string VersionNumber { get; set; }

        public int RelationType { get; set; }

        public int ObjectType { get; set; }
    }

    public class EbObjectTaggedRequest : EbServiceStackAuthRequest, IReturn<EbObjectTaggedResponse>
    {
        public string Tags { get; set; }

        public int EbObjectType { get; set; }
    }

    public class EbObjectTaggedResponse : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public List<EbObjectWrapper> Data { get; set; }

        [DataMember(Order = 2)]
        public string Token { get; set; }

        [DataMember(Order = 3)]
        public ResponseStatus ResponseStatus { get; set; }
    }

    public class EbObjectGetAllTagsRequest : EbServiceStackAuthRequest, IReturn<EbObjectGetAllTagsResponse>
    {

    }

    public class EbObjectGetAllTagsResponse : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public string Data { get; set; }

        [DataMember(Order = 2)]
        public string Token { get; set; }

        [DataMember(Order = 3)]
        public ResponseStatus ResponseStatus { get; set; }
    }

    public class UniqueObjectNameCheckRequest : EbServiceStackAuthRequest, IReturn<UniqueObjectNameCheckResponse>
    {
        public string ObjName { get; set; }
    }

    public class UniqueObjectNameCheckResponse : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public bool IsUnique { get; set; }

        [DataMember(Order = 2)]
        public ResponseStatus ResponseStatus { get; set; }
    }

}
