using ExpressBase.Common;
using ExpressBase.Common.Application;
using ExpressBase.Common.EbServiceStack.ReqNRes;
using ExpressBase.Common.Objects;
using ExpressBase.Common.Structures;
using ExpressBase.Objects.Objects;
using ServiceStack;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace ExpressBase.Objects.ServiceStack_Artifacts
{
    public class CreateBotRequest : EbServiceStackAuthRequest, IReturn<CreateBotResponse>
    {
        public string SolutionId { get; set; }

        public string BotName { get; set; }

        public string FullName { get; set; }

        public string WebURL { get; set; }

        public string WelcomeMsg { get; set; }

        public string CreatedBy { get; set; }

        public string BotId { get; set; }

        public string ChatId { get; set; }
    }

    public class GetBotForm4UserRequest : EbServiceStackAuthRequest, IReturn<CreateBotResponse>
    {
        public string BotFormIds { get; set; }

        public string AppId { get; set; }
    }

    [System.Runtime.Serialization.DataContract]
    public class GetAppListResponse : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public int Id { get; set; }

        [DataMember(Order = 2)]
        public string Token { get; set; }

        [DataMember(Order = 3)]
        public ResponseStatus ResponseStatus { get; set; }

        [DataMember(Order = 4)]
        public Dictionary<string, List<string>> AppList { get; set; }

        public GetAppListResponse()
        {
            this.AppList = new Dictionary<string, List<string>>();
        }
    }

    [System.Runtime.Serialization.DataContract]
    public class GetBotDetailsResponse : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public int Id { get; set; }

        [DataMember(Order = 2)]
        public string Token { get; set; }

        [DataMember(Order = 3)]
        public ResponseStatus ResponseStatus { get; set; }

        [DataMember(Order = 4)]
        public string Name { get; set; }

        [DataMember(Order = 5)]
        public string Url { get; set; }

        [DataMember(Order = 6)]
        public string WelcomeMsg { get; set; }

        [DataMember(Order = 7)]
        public string BotId { get; set; }

        [DataMember(Order = 8)]
        public string FullName { get; set; }
    }

    public class AppListRequest : EbServiceStackAuthRequest, IReturn<CreateBotResponse>
    {
        public int SolutionId { get; set; }
    }

    public class BotDetailsRequest : EbServiceStackAuthRequest, IReturn<CreateBotResponse>
    {
        public int AppId { get; set; }
    }

    [System.Runtime.Serialization.DataContract]
    public class BotListResponse : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public int Id { get; set; }

        [DataMember(Order = 2)]
        public string Token { get; set; }

        [DataMember(Order = 3)]
        public ResponseStatus ResponseStatus { get; set; }

        [DataMember(Order = 4)]
        public List<ChatBot> Data { get; set; }
    }

    [System.Runtime.Serialization.DataContract]
    public class GetBotForm4UserResponse : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public int Id { get; set; }

        [DataMember(Order = 2)]
        public string Token { get; set; }

        [DataMember(Order = 3)]
        public ResponseStatus ResponseStatus { get; set; }

        [DataMember(Order = 4)]
        public Dictionary<string, string> BotForms { get; set; }

        public GetBotForm4UserResponse()
        {
            this.BotForms = new Dictionary<string, string>();
        }
    }

    [System.Runtime.Serialization.DataContract]
    public class CreateBotResponse : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public int Id { get; set; }

        [DataMember(Order = 2)]
        public string Token { get; set; }

        [DataMember(Order = 3)]
        public ResponseStatus ResponseStatus { get; set; }

        [DataMember(Order = 4)]
        public string BotId { get; set; }
    }

    public class BotListRequest : EbServiceStackAuthRequest, IReturn<CreateBotResponse>
    {
        public int SolutionId { get; set; }
    }

    [DataContract]
    public class ChatBot
    {

        [DataMember(Order = 1)]
        public string Name { get; set; }

        [DataMember(Order = 2)]
        public string FullName { get; set; }

        [DataMember(Order = 3)]
        public string WebsiteURL { get; set; }

        [DataMember(Order = 4)]
        public string BotId { get; set; }

        [DataMember(Order = 5)]
        public string CreatedBy { get; set; }

        [DataMember(Order = 6)]
        public DateTime CreatedAt { get; set; }

        [DataMember(Order = 7)]
        public string LastModifiedBy { get; set; }

        [DataMember(Order = 8)]
        public DateTime LastModifiedAt { get; set; }

        [DataMember(Order = 9)]
        public string WelcomeMsg { get; set; }

        [DataMember(Order = 10)]
        public string ChatId { get; set; }

        public ChatBot() { }
    }

    
    [DataContract]
    public class CreateBotFormTableRequest : EbServiceStackAuthRequest, IReturn<CreateBotResponse>
    {
        [DataMember(Order = 1)]
        public dynamic BotObj { get; set; }

        [DataMember(Order = 1)]
        public string Apps { get; set; }


        //public string  TableName { get; set; }

        //[DataMember(Order = 2)]
        //public Dictionary<string, string> Fields { get; set; }
    }

    [DataContract]
    public class CreateBotFormTableResponse : IEbSSResponse
    {
        [DataMember(Order = 3)]
        public ResponseStatus ResponseStatus { get; set; }

        [DataMember(Order = 4)]
        public string BotId { get; set; }
    }

    
    
    [Route("/bots")]
    [DataContract]
    public class InsertIntoBotFormTableRequest : EbServiceStackAuthRequest, IReturn<CreateBotResponse>
    {
        [DataMember(Order = 1)]
        public string TableName { get; set; }

        [DataMember(Order = 2)]
        public List<BotFormField> Fields { get; set; }

        [DataMember(Order = 3)]
        public int AnonUserId { get; set; }

        [DataMember(Order = 4)]
        public int Id { get; set; }
    }


	[DataContract]
    public class InsertDataFromWebformResponse : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public int RowAffected { get; set; }

        [DataMember(Order = 2)]
        public ResponseStatus ResponseStatus { get; set; }
    }
    
    [DataContract]
    public class InsertDataFromWebformRequest : EbServiceStackAuthRequest, IReturn<CreateBotResponse>
    {
        [DataMember(Order = 1)]
        public string TableName { get; set; }

        [DataMember(Order = 2)]
        public Dictionary<string, List<TableColumnMetaS>> Values { get; set; }

        [DataMember(Order = 3)]
        public string RefId { get; set; }

        [DataMember(Order = 3)]
        public int RowId { get; set; }
    }

    [DataContract]
    public class InsertIntoBotFormTableResponse : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public int RowAffected { get; set; }

        [DataMember(Order = 2)]
        public ResponseStatus ResponseStatus { get; set; }

        [DataMember(Order = 3)]
        public string BotId { get; set; }
    }

    [DataContract]
    public class GetRowDataResponse : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public List<Object> RowValues { get; set; }

        [DataMember(Order = 2)]
        public ResponseStatus ResponseStatus { get; set; }
    }

    [DataContract]
    public class SubmitBotFormRequest : EbServiceStackAuthRequest, IReturn<SubmitBotFormResponse>
    {
        [DataMember(Order = 1)]
        public int Id { get; set; }

        [DataMember(Order = 2)]
        public string RefId { get; set; }

        [DataMember(Order = 3)]
        public List<BotFormField> Fields { get; set; }

        [DataMember(Order = 4)]
        public int AnonUserId { get; set; }
    }

    [DataContract]
    public class SubmitBotFormResponse : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public int RowAffected { get; set; }

        [DataMember(Order = 2)]
        public ResponseStatus ResponseStatus { get; set; }
    }

    [DataContract]
    public class BotFormField
    {
        [DataMember(Order = 1)]
        public string Value { get; set; }

        [DataMember(Order = 2)]
        public EbDbTypes Type { get; set; }

        [DataMember(Order = 3)]
        public string Name { get; set; }

        [DataMember(Order = 4)]
        public bool AutoIncrement { get; set; }

        [DataMember(Order = 5)]
        public string OldValue { get; set; }
    }

    [DataContract]
    public class GetBotsResponse : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public ResponseStatus ResponseStatus { get; set; }

        [DataMember(Order = 2)]
        public List<BotDetails> BotList { get; set; }
    }

    [DataContract]
    public class GetBotsRequest : EbServiceStackNoAuthRequest, IReturn<GetBotsResponse>
    {
        [DataMember(Order = 1)]
        public string Value { get; set; }

    }

    [DataContract]
    public class BotDetails
    {
        [DataMember(Order = 1)]
        public int id { get; set; }

        [DataMember(Order = 2)]
        public string name { get; set; }

        [DataMember(Order = 3)]
        public string icon { get; set; }

        [DataMember(Order = 4)]
        public EbBotSettings botsettings { get; set; }
    }

    [DataContract]
    public class GetBotSettingsRequest : EbServiceStackAuthRequest, IReturn<GetBotSettingsResponse>
    {
        [DataMember(Order = 1)]
        public int AppId { get; set; }

    }

    [DataContract]
    public class GetBotSettingsResponse : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public ResponseStatus ResponseStatus { get; set; }

        [DataMember(Order = 2)]
        public EbBotSettings Settings { get; set; }
    }

}
