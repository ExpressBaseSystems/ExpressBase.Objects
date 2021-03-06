﻿using ExpressBase.Common;
using ExpressBase.Common.Application;
using ExpressBase.Common.EbServiceStack.ReqNRes;
using ServiceStack;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace ExpressBase.Objects.ServiceStack_Artifacts
{
    [DataContract]
    public class CreateApplicationRequest : IReturn<CreateApplicationResponse>, IEbSSRequest
    {
        [DataMember(Order = 1)]
        public string AppName { get; set; }

        [DataMember(Order = 2)]
        public int AppType { get; set; }

        [DataMember(Order = 3)]
        public string Description { get; set; }

        [DataMember(Order = 4)]
        public string AppIcon { get; set; }

        [DataMember(Order = 5)]
        public string Sid { get; set; }

        [DataMember(Order = 6)]
        public int AppId { get; set; }

        [DataMember(Order = 7)]
        public string AppSettings { set; get; }

        public string SolnId { get; set; }

        public int UserId { get; set; }

        public string Token { get; set; }
    }

    [DataContract]
    public class CreateApplicationResponse : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public int Id { get; set; }

        [DataMember(Order = 2)]
        public string Token { get; set; }

        [DataMember(Order = 3)]
        public ResponseStatus ResponseStatus { get; set; }
    }

    [DataContract]
    public class GetApplicationRequest : IReturn<GetApplicationResponse>, IEbSSRequest
    {
        [DataMember(Order = 1)]
        public string Token { get; set; }

        [DataMember(Order = 2)]
        public int Id { get; set; }

        public string SolnId { get; set; }

        public int UserId { get; set; }
    }

    [DataContract]
    public class GetApplicationResponse : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public Dictionary<string, object> Data { get; set; }

        [DataMember(Order = 2)]
        public string Token { get; set; }

        [DataMember(Order = 3)]
        public ResponseStatus ResponseStatus { get; set; }

        [DataMember(Order = 4)]
        public AppWrapper AppInfo { get; set; }
    }

    public class GetAllApplicationRequest : IReturn<GetAllApplicationResponse>, IEbSSRequest
    {
        [DataMember(Order = 1)]
        public string Token { get; set; }

        public string SolnId { get; set; }

        public int UserId { get; set; }
    }

    public class GetAllApplicationResponse : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public List<AppWrapper> Data { get; set; }

        [DataMember(Order = 2)]
        public string Token { get; set; }

        [DataMember(Order = 3)]
        public ResponseStatus ResponseStatus { get; set; }
    }

    public class GetObjectsByAppIdRequest : IReturn<GetObjectsByAppIdResponse>, IEbSSRequest
    {
        [DataMember(Order = 1)]
        public int Id { get; set; }

        [DataMember(Order = 2)]
        public EbApplicationTypes AppType { get; set; }

        public string SolnId { get; set; }

        public int UserId { get; set; }
    }

    public class GetObjectsByAppIdResponse : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public Dictionary<int, TypeWrap> Data { get; set; }

        [DataMember(Order = 2)]
        public ResponseStatus ResponseStatus { get; set; }

        [DataMember(Order = 3)]
        public AppWrapper AppInfo { get; set; }

        [DataMember(Order = 4)]
        public int ObjectsCount { get; set; }
    }

    public class GetObjectRequest : EbServiceStackAuthRequest, IReturn<GetObjectResponse>
    {

    }

    public class GetObjectResponse : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public Dictionary<int, TypeWrap> Data { get; set; }

        [DataMember(Order = 3)]
        public ResponseStatus ResponseStatus { get; set; }
    }

    public class DeleteAppRequest : EbServiceStackAuthRequest, IReturn<DeleteAppResponse>
    {
        [DataMember(Order = 1)]
        public int AppId { set; get; }
    }

    public class DeleteAppResponse : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public bool Status { set; get; }

        [DataMember(Order = 2)]
        public ResponseStatus ResponseStatus { get; set; }
    }

    public class Coloums
    {
        public string cname { get; set; }

        public string type { get; set; }

        public string constraints { get; set; }

        public string foreign_tnm { get; set; }

        public string foreign_cnm { get; set; }
    }

    public class GetTableSchemaRequest : IReturn<GetTbaleSchemaResponse>, IEbSSRequest
    {
        public string SolnId { get; set; }

        public int UserId { get; set; }
    }

    public class GetTbaleSchemaResponse : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public Dictionary<string, List<Coloums>> Data { get; set; }

        [DataMember(Order = 2)]
        public ResponseStatus ResponseStatus { get; set; }
    }

    [DataContract]
    public class SaveAppSettingsRequest : EbServiceStackAuthRequest, IReturn<SaveAppSettingsResponse>
    {
        [DataMember(Order = 1)]
        public int AppId { get; set; }

        [DataMember(Order = 1)]
        public int AppType { get; set; }

        [DataMember(Order = 2)]
        public string Settings { get; set; }

    }

    [DataContract]
    public class SaveAppSettingsResponse
    {
        [DataMember(Order = 1)]
        public int ResStatus { get; set; }
    }

    public class UniqueApplicationNameCheckRequest : EbServiceStackAuthRequest, IReturn<UniqueObjectNameCheckResponse>
    {
        public string AppName { get; set; }
    }

    public class UniqueApplicationNameCheckResponse : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public bool IsUnique { get; set; }

        [DataMember(Order = 2)]
        public ResponseStatus ResponseStatus { get; set; }

        [DataMember(Order = 2)]
        public int AppId { get; set; }
    }

    public class GetDefaultMapApiKeyFromConnectionRequest : EbServiceStackAuthRequest
    {

    }

    public class UpdateAppSettingsRequest : EbServiceStackAuthRequest, IReturn<UpdateAppSettingsResponse>
    {
        [DataMember(Order = 1)]
        public string Settings { get; set; }

        [DataMember(Order = 2)]
        public int AppId { get; set; }

        [DataMember(Order = 3)]
        public EbApplicationTypes AppType { get; set; }
    }

    public class UpdateAppSettingsResponse
    {
        [DataMember(Order = 1)]
        public bool Status { set; get; }

        [DataMember(Order = 2)]
        public string Message { set; get; }
    }

    public class SaveSolutionSettingsRequest : EbServiceStackAuthRequest, IReturn<SaveSolutionSettingsResponse>
    {
        public string SolutionSettings { get; set; }

        public string CleanupQueries { get; set; }
    }

    public class SaveSolutionSettingsResponse : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public ResponseStatus ResponseStatus { get; set; }

        public string Message { get; set; }

    }

    public class GetCleanupQueryRequest : EbServiceStackAuthRequest, IReturn<GetCleanupQueryResponse>
    {
    }

    public class GetCleanupQueryResponse : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public ResponseStatus ResponseStatus { get; set; }

        public string CleanupQueries { get; set; }

        public string Message { get; set; }

    }

    public class MobileFormControlInfo
    {
        public List<EbMobileControl> Controls { set; get; }

        public List<EbMobileControlMeta> ControlMetas { set; get; }

        public bool IsForm { set; get; }

        public MobileFormControlInfo()
        {
            Controls = new List<EbMobileControl>();
            ControlMetas = new List<EbMobileControlMeta>();
        }
    }
}
