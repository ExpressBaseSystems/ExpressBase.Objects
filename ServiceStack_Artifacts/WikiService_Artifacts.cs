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
        public string CreatedBy { get; set; }
    }

    public class PersistWikiRequest : IEbTenentRequest, IReturn<PersistWikiResponse>
    {

        public Wiki Wiki { get; set; }

        public string SolnId { get ; set ; }
        public int UserId { get; set; }

        //public string Title { get; set; }
        //public string Subtitle { get; set; }
        //public string Content { get; set; }

    }

    public class PersistWikiResponse
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
        public Wiki Wiki { get; set; }
    }

    public class GetWikiByIdResponse
    {
        public Wiki Wiki { get; set; }
    }

    //public class GetTitleRequest : EbServiceStackNoAuthRequest
    //{

    //}
    //public class GetTitleResponse : EbServiceStackNoAuthRequest
    //{
    //    public List<string> Title { get; set; }

    //    public GetTitleResponse()
    //    {
    //        Title = new List<string>();
    //    }
    //}

    //public class GetContentResponse : EbServiceStackNoAuthRequest
    //{
    //    public List<string> Content { get; set; }

    //    public GetContentResponse()
    //    {
    //        Content = new List<string>();
    //    }
    //}

    //public class GetContentRequest : EbServiceStackNoAuthRequest
    //{
    //    public string Contentname { get; set; }
    //}


}
