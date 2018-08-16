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
        public int Id { get; set; }

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

        [DataMember(Order = 4)]
        public AppWrapper AppInfo { get; set; }
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

		[DataMember(Order = 6)]
		public object AppSettings { set; get; }

        [DataMember(Order =7)]
        public List<EbObject> ObjCollection { get; set; }
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

	[DataContract]
	public class SaveAppSettingsRequest : EbServiceStackRequest, IReturn<SaveAppSettingsResponse>
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

    public class UniqueApplicationNameCheckRequest : EbServiceStackRequest, IReturn<UniqueObjectNameCheckResponse>
    {
        public string AppName { get; set; }
    }

    public class UniqueApplicationNameCheckResponse : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public bool IsUnique { get; set; }

        [DataMember(Order = 2)]
        public ResponseStatus ResponseStatus { get; set; }
    }
    //survey
    public class GetSurveyQueriesRequest : IEbSSRequest, IReturn<GetSurveyQueriesResponse>
    {
        public string TenantAccountId { get; set; }

        public int UserId { get; set; }
    }
    public class GetSurveyQueriesResponse : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public Dictionary<int,EbSurveyQuery> Data { get; set; }

        [DataMember(Order = 2)]
        public ResponseStatus ResponseStatus { get; set; }
    }

    public class SurveyQuesRequest: IReturn<SurveyQuesResponse>, IEbSSRequest
    {
        public string TenantAccountId { get; set; }

        public int UserId { get; set; }

        [DataMember(Order = 1)]
        public EbSurveyQuery Query { set; get; }
    }
    public class SurveyQuesResponse: IEbSSResponse
    {
        [DataMember(Order = 1)]
        public bool Status { get; set; }

        [DataMember(Order = 2)]
        public ResponseStatus ResponseStatus { get; set; }
    }

    public class EbSurveyQuery
    {
        [DataMember(Order = 1)]
        public int QuesId { set; get; }

        [DataMember(Order = 2)]
        public string Question { set; get; }

        [DataMember(Order = 3)]
        public int QuesType { set; get; }

        [DataMember(Order = 4)]
        public List<QueryChoices> Choices { set; get; }
    }

    public class QueryChoices
    {
        [DataMember(Order = 1)]
        public int ChoiceId { set; get; }

        [DataMember(Order = 2)]
        public string Choice { set; get; }

        [DataMember(Order = 3)]
        public bool EbDel { set; get; }

        [DataMember(Order = 4)]
        public int Score { set; get; }

        [DataMember(Order = 5)]
        public bool IsNew { set; get; }
    }

	public class ManageSurveyRequest : IReturn<ManageSurveyResponse>, IEbSSRequest
	{
		[DataMember(Order = 1)]
		public int Id { get; set; }

		public string TenantAccountId { get; set; }

		public int UserId { get; set; }

	}
	public class ManageSurveyResponse : IEbSSResponse
	{
		[DataMember(Order = 1)]
		public Eb_Survey Obj { get; set; }

		[DataMember(Order = 2)]
		public List<Eb_SurveyQuestion> AllQuestions { get; set; }
		
		[DataMember(Order = 2)]
		public ResponseStatus ResponseStatus { get; set; }
	}

	public class SaveSurveyRequest : IReturn<SaveSurveyResponse>, IEbSSRequest
	{
		[DataMember(Order = 1)]
		public string Data { get; set; }

		public string TenantAccountId { get; set; }

		public int UserId { get; set; }

	}
	public class SaveSurveyResponse : IEbSSResponse
	{
		[DataMember(Order = 1)]
		public int Status { get; set; }

		[DataMember(Order = 2)]
		public ResponseStatus ResponseStatus { get; set; }
	}

	public class GetSurveyListRequest : IReturn<GetSurveyListResponse>, IEbSSRequest
	{
		public string TenantAccountId { get; set; }

		public int UserId { get; set; }

	}
	public class GetSurveyListResponse : IEbSSResponse
	{
		[DataMember(Order = 1)]
		public Dictionary<int, string> SurveyDict { get; set; }

		[DataMember(Order = 2)]
		public ResponseStatus ResponseStatus { get; set; }
	}

	public class GetParticularSurveyRequest : IReturn<GetParticularSurveyResponse>, IEbSSRequest
	{
		[DataMember(Order = 1)]
		public int SurveyId { get; set; }

		public string TenantAccountId { get; set; }

		public int UserId { get; set; }

	}
	public class GetParticularSurveyResponse : IEbSSResponse
	{
		[DataMember(Order = 1)]
		public int SurveyId { get; set; }
		
		[DataMember(Order = 2)]
		public string SurveyName { get; set; }

		[DataMember(Order = 3)]
		public Dictionary<int, Eb_SurveyQuestion> Queries { get; set; }

		[DataMember(Order = 4)]
		public ResponseStatus ResponseStatus { get; set; }
	}

	[DataContract]
	public class Eb_Survey
	{
		[DataMember(Order = 1)]
		public int Id { get; set; }

		[DataMember(Order = 2)]
		public string Name { get; set; }

		[DataMember(Order = 3)]
		public string Start { get; set; }

		[DataMember(Order = 4)]
		public string End { get; set; }

		[DataMember(Order = 5)]
		public int Status { get; set; }

		[DataMember(Order = 6)]
		public List<int> QuesIds { get; set; }

		public Eb_Survey() { }
	}

	[DataContract]
	public class Eb_SurveyQuestion
	{
		[DataMember(Order = 1)]
		public int Id;

		[DataMember(Order = 2)]
		public string Question;

		[DataMember(Order = 3)]
		public List<string> Choices { get; set; }

		[DataMember(Order = 4)]
		public int ChoiceType;
		
		public Eb_SurveyQuestion() { }
	}
}
