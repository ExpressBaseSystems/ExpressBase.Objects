using ExpressBase.Common;
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
        public int appid { get; set; }

        public string TenantAccountId { get; set; }

        public int UserId { get; set; }

        public string Token { get; set; }

    }

    [DataContract]
    public class CreateApplicationResponse : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public int id { get; set; }

        [DataMember(Order = 2)]
        public string Token { get; set; }

        [DataMember(Order = 3)]
        public ResponseStatus ResponseStatus { get; set; }

    }

    [DataContract]
    public class CreateApplicationDevRequest : IReturn<CreateApplicationResponse>, IEbSSRequest
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
        public int appid { get; set; }

        public string TenantAccountId { get; set; }

        public int UserId { get; set; }

        public string Token { get; set; }

    }

    [DataContract]
    public class GetApplicationRequest : IReturn<GetApplicationResponse>, IEbSSRequest
    {
        [DataMember(Order = 1)]
        public string Token { get; set; }

        [DataMember(Order = 2)]
        public int id { get; set; }

        public string TenantAccountId { get; set; }

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
    }

    public class GetAllApplicationRequest : IReturn<GetAllApplicationResponse>, IEbSSRequest
    {
        [DataMember(Order = 1)]
        public string Token { get; set; }

        public string TenantAccountId { get; set; }

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

    [DataContract]
    public class AppWrapper
    {
        [DataMember(Order = 1)]
        public int Id { set; get; }

        [DataMember(Order = 2)]
        public string Name { set; get; }

        [DataMember(Order = 3)]
        public int AppType { set; get; }

        [DataMember(Order = 4)]
        public string Icon { set; get; }

        [DataMember(Order = 5)]
        public string Description { set; get; }

    }

    public class GetObjectsByAppIdRequest : IReturn<GetObjectsByAppIdResponse>, IEbSSRequest
    {
        [DataMember(Order = 1)]
        public int Id { get; set; }

        [DataMember(Order = 2)]
        public EbApplicationTypes AppType { get; set; }

        public string TenantAccountId { get; set; }

        public int UserId { get; set; }
    }

    public class GetObjectsByAppIdResponse: IEbSSResponse
    {
        [DataMember(Order = 1)]
        public Dictionary<int, TypeWrap> Data { get; set; }

        [DataMember(Order = 2)]
        public ResponseStatus ResponseStatus { get; set; }

        [DataMember(Order = 3)]
        public AppWrapper AppInfo { get; set; }
    }

    public class GetObjectRequest : EbServiceStackRequest, IReturn<GetObjectResponse>
    {

    }

    public class GetObjectResponse : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public Dictionary<int, TypeWrap> Data { get; set; }

        [DataMember(Order = 3)]
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
        public string TenantAccountId { get; set; }

        public int UserId { get; set; }
    }

    public class GetTbaleSchemaResponse : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public Dictionary<string, List<Coloums>> Data { get; set; }

       [DataMember(Order = 2)]
        public ResponseStatus ResponseStatus { get; set; }
    }
}
