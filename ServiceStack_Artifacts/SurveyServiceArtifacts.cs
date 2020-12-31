using ExpressBase.Common;
using ExpressBase.Common.EbServiceStack.ReqNRes;
using ServiceStack;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace ExpressBase.Objects.ServiceStack_Artifacts
{
    public class GetSurveyQueriesRequest : IEbSSRequest, IReturn<GetSurveyQueriesResponse>
    {
        public string SolnId { get; set; }

        public int UserId { get; set; }
    }
    public class GetSurveyQueriesResponse : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public Dictionary<int, EbSurveyQuery> Data { get; set; }

        [DataMember(Order = 2)]
        public ResponseStatus ResponseStatus { get; set; }
    }

    public class SurveyQuesRequest : IReturn<SurveyQuesResponse>, IEbSSRequest
    {
        public string SolnId { get; set; }

        public int UserId { get; set; }

        [DataMember(Order = 1)]
        public EbSurveyQuery Query { set; get; }
    }

    public class SaveQuestionRequest : IReturn<SaveQuestionResponse>, IEbSSRequest
    {
        public string SolnId { get; set; }

        public int UserId { get; set; }

        [DataMember(Order = 1)]
        public EbQuestion Query { set; get; }
    }
    public class SurveyQuesResponse : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public bool Status { get; set; }

        [DataMember(Order = 2)]
        public ResponseStatus ResponseStatus { get; set; }

        [DataMember(Order = 3)]
        public int Quesid { set; get; }
    }
    
    public class SaveQuestionResponse : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public bool Status { get; set; }

        [DataMember(Order = 2)]
        public ResponseStatus ResponseStatus { get; set; }

        [DataMember(Order = 3)]
        public int Quesid { set; get; }
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

        [DataMember(Order = 5)]
        public bool IsRequired { set; get; }
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

        public string SolnId { get; set; }

        public int UserId { get; set; }

    }
    public class ManageSurveyResponse : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public Eb_Survey Obj { get; set; }

        [DataMember(Order = 2)]
        public List<EbSurveyQuery> AllQuestions { get; set; }

        [DataMember(Order = 2)]
        public ResponseStatus ResponseStatus { get; set; }
    }

    public class SaveSurveyRequest : IReturn<SaveSurveyResponse>, IEbSSRequest
    {
        [DataMember(Order = 1)]
        public string Data { get; set; }

        public string SolnId { get; set; }

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
        public string SolnId { get; set; }

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

        public string SolnId { get; set; }

        public int UserId { get; set; }

    }
    public class GetParticularSurveyResponse : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public Eb_Survey SurveyInfo { get; set; }

        [DataMember(Order = 2)]
        public Dictionary<int, EbSurveyQuery> Queries { get; set; }

        [DataMember(Order = 3)]
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
    }

    public class GetSurveysByAppRequest : IReturn<GetSurveysByAppResponse>, IEbSSRequest
    {
        public string SolnId { get; set; }

        public int UserId { get; set; }
    }

    public class GetSurveysByAppResponse : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public List<Eb_Survey> Surveys { get; set; }

        [DataMember(Order = 2)]
        public ResponseStatus ResponseStatus { get; set; }
    }

    public class SurveyMasterRequest : IReturn<SurveyMasterResponse>, IEbSSRequest
    {
        public string SolnId { get; set; }

        public int UserId { get; set; }

        [DataMember(Order = 1)]
        public int SurveyId { get; set; }

        [DataMember(Order = 2)]
        public int AnonId { get; set; }
    }

    public class SurveyMasterResponse : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public int Id { get; set; }

        [DataMember(Order = 2)]
        public ResponseStatus ResponseStatus { get; set; }
    }

    public class SurveyLinesRequest : IReturn<SurveyLinesResponse>, IEbSSRequest
    {
        public string SolnId { get; set; }

        public int UserId { get; set; }

        [DataMember(Order = 1)]
        public int MasterId { get; set; }

        [DataMember(Order = 2)]
        public int QuesId { get; set; }

        [DataMember(Order = 3)]
        public string ChoiceIds { get; set; }

        [DataMember(Order = 4)]
        public int QuesType { get; set; }

        [DataMember(Order = 5)]
        public string Answer { get; set; }
    }

    public class SurveyLinesResponse : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public bool Status { get; set; }

        [DataMember(Order = 2)]
        public ResponseStatus ResponseStatus { get; set; }
    }

    public class SurveyLine
    {
        public int QuesId { get; set; }

        public int MasterId { get; set; }

        public string ChoiceIds { get; set; }

        public int QuesType { get; set; }

        public string Answer { get; set; }
    }
    public class GetSurveyEnqRequest : IReturn<GetSurveyEnqResponse>, IEbSSRequest
    {
        public string SolnId { get; set; }

        public int UserId { get; set; }
    }

    public class GetSurveyEnqResponse : IEbSSResponse
    {
        [DataMember(Order = 2)]
        public ResponseStatus ResponseStatus { get; set; }
    }
}

