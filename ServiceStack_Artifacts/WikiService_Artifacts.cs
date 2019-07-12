using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using ExpressBase.Common.EbServiceStack.ReqNRes;
using ServiceStack;

namespace ExpressBase.Objects.ServiceStack_Artifacts
{
    public class Wiki
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Category { get; set; }

        public string HTML { get; set; }

        public DateTime CreatedAt { get; set; }

        public Int32 CreatedBy { get; set; }

        public string Tags { get; set; }

        public string Status { get; set; }
    }

    public class PersistWikiRequest : IEbTenentRequest, IReturn<PersistWikiResponse>
    {

        public Wiki Wiki { get; set; }

        public string SolnId { get; set; }

        public int UserId { get; set; }
    }

    public class PersistWikiResponse
    {
        public Wiki Wiki { get; set; }

        public bool ResponseStatus { get; set; }
    }

    public class FileRefByContextRequest:IEbTenentRequest, IReturn<PersistWikiResponse>
    {
        public string Context { set; get; }

        public string SolnId { get; set; }

        public int UserId { get; set; }
    }

    public class FileRefByContextResponse
    {
        public List<FileMetaInfo> Images { set; get; }

        public FileRefByContextResponse()
        {
            this.Images = new List<FileMetaInfo>();
        }
    }

    public class UpdateWikiRequest : IEbTenentRequest, IReturn<UpdateWikiResponse>
    {
        public Wiki Wiki { get; set; }

        public string SolnId { get; set; }

        public int UserId { get; set; }

    }

    public class UpdateWikiResponse
    {
        public Wiki Wiki { get; set; }

        public bool ResponseStatus { get; set; }
    }

    public class GetWikiListRequest : EbServiceStackNoAuthRequest, IReturn<GetWikiListResponse>
    {
        public string Category { get; set; }

        public string Tag { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }
    }

    public class GetWikiListResponse
    {
        public GetWikiListResponse()
        {
            WikiList = new List<Wiki>();
        }

        public List<Wiki> WikiList { get; set; }

    }

    public class GetWikiByIdRequest : EbServiceStackNoAuthRequest, IReturn<GetWikiByIdResponse>
    {
        public int Id { get; set; }
    }

    public class GetWikiByIdResponse
    {
        public Wiki Wiki { get; set; }
    }

    public class GetWikiRequest : EbServiceStackNoAuthRequest, IReturn<GetWikiResponse>
    {
        public int Id { get; set; }
    }

    public class GetWikiResponse
    {
        public Wiki Wiki { get; set; }
    }

    public class GetWikiBySearchRequest : EbServiceStackNoAuthRequest, IReturn<GetWikiBySearchResponse>
    {
        public string Wiki_Search { get; set; }
    }

    public class GetWikiBySearchResponse
    {
        public GetWikiBySearchResponse()
        {
            WikiListBySearch = new List<Wiki>();
        }
        public List<Wiki> WikiListBySearch { get; set; }
    }




    public class WikiAdminRequest : EbServiceStackAuthRequest, IReturn<WikiAdminResponse>
    {
        public string Category { get; set; }

        public string Tag { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }
    }

    public class WikiAdminResponse
    {
        public WikiAdminResponse()
        {
            WikiList = new List<Wiki>();
        }

        public List<Wiki> WikiList { get; set; }

    }

    public class Admin_Wiki_ListRequest : EbServiceStackAuthRequest, IReturn<Admin_Wiki_ListResponse>
    {
        public string Status { get; set; }
    }
    public class Admin_Wiki_ListResponse
    {
        public Admin_Wiki_ListResponse()
        {
            WikiList = new List<Wiki>();
        }
        public List<Wiki> WikiList { get; set; }
    }

    public class Publish_wikiRequest : EbServiceStackAuthRequest, IReturn<Publish_wikiResponse>
    {
        public int Wiki_id { get; set; }
        public string Status { get; set; }
    }
    public class Publish_wikiResponse
    {
        public int Id { get; set; }
    }

    public class PublicViewRequest : EbServiceStackAuthRequest, IReturn<PublicViewResponse>
    {
    }
    public class PublicViewResponse
    {
        public PublicViewResponse()
        {
            WikiList = new List<Wiki>();
        }

        public List<Wiki> WikiList { get; set; }
    }

    public class UpdateOrderRequest : EbServiceStackAuthRequest, IReturn<UpdateOrderResponse>
    {
        public string Wiki_id { get; set; }
        public int Order { get; set; }
    }

    public class UpdateOrderResponse
    {
        public bool ResponseStatus { get; set; }
    }
    public class UserReviewRateRequest : EbServiceStackNoAuthRequest, IReturn<UserReviewRateResponse>
    {
        public string UserReview { get; set; }
    }
    public class UserReviewRateResponse
    {
        public bool Resp { get; set; }
    }
}
