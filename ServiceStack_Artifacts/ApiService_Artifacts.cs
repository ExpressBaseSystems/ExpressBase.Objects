﻿using ExpressBase.Common.Data;
using ExpressBase.Common.EbServiceStack.ReqNRes;
using ServiceStack;
using ServiceStack.Text;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using ExpressBase.Security;
using System.Net;
using ExpressBase.Common.Structures;

namespace ExpressBase.Objects.ServiceStack_Artifacts
{
    public interface IEbApiStatusCode
    {
        HttpStatusCode StatusCode { set; get; }
    }

    public interface IApiResponse
    {
        string Name { set; get; }

        string Version { set; get; }

        ApiMessage Message { get; set; }

        object Result { get; set; }
    }

    [DataContract]
    public class FormDataJsonRequest : IReturn<FormDataJsonResponse>, IEbSSRequest
    {
        public string SolnId { get; set; }

        public int UserId { get; set; }

        [DataMember(Order = 1)]
        public string JsonData { set; get; }
    }

    [DataContract]
    public class FormDataJsonResponse : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public ResponseStatus ResponseStatus { get; set; }

        [DataMember(Order = 2)]
        public string RefId { get; set; }
    }

    [DataContract]
    public class ApiComponetRequest : EbServiceStackAuthRequest, IReturn<ApiResponse>
    {

        [DataMember(Order = 1)]
        public ApiResources Component { set; get; }

        [DataMember(Order = 2)]
        public List<Param> Params { set; get; }
    }


    public class InsertLogRequest : EbServiceStackNoAuthRequest, IReturn<InsertLogResponse>
    {
        public string Version { set; get; }

        public string Name { set; get; }

        public string RefId { set; get; }

        public string Parameters { get; set; }

        public string Message { get; set; }

        public string Status { get; set; }

        public string Result { get; set; }

        public string SolnId { get; set; }

        public int UserId { get; set; }

    }

    public class InsertLogResponse : IEbSSResponse
    {
        public int Id { get; set; }

        public ResponseStatus ResponseStatus { get; set; }
    }

    public class ApiRequest : EbServiceStackNoAuthRequest, IReturn<ApiResponse>
    {
        [DataMember(Order = 1)]
        public string Version { set; get; }

        [DataMember(Order = 2)]
        public string Name { set; get; }

        [DataMember(Order = 3)]
        public Dictionary<string, object> Data { set; get; }

        [DataMember(Order = 4)]
        public string RefId { set; get; }

        [DataMember(Order = 5)]
        public string SolnId { get; set; }

        [DataMember(Order = 6)]
        public int UserId { get; set; }

        [DataMember(Order = 7)]
        public string UserAuthId { get; set; }

        [DataMember(Order = 8)]
        public string WhichConsole { get; set; }

        public EbJobArguments JobArgs { set; get; }

        public bool HasRefId()
        {
            return !string.IsNullOrEmpty(RefId);
        }
    }

    public class ApiMqRequest : EbMqRequest, IReturn<ApiResponse>
    {
        [DataMember(Order = 1)]
        public string Version { set; get; }

        [DataMember(Order = 2)]
        public string Name { set; get; }

        [DataMember(Order = 3)]
        public Dictionary<string, object> Data { set; get; }

        [DataMember(Order = 4)]
        public string RefId { set; get; }
        public EbJobArguments JobArgs { set; get; }

        public bool HasRefId()
        {
            return !string.IsNullOrEmpty(JobArgs?.RefId);
        }

        public bool HasObjectId()
        {
            return (JobArgs?.ObjId > 0);
        }
    }

    [DataContract]
    public class ApiResponse : IApiResponse
    {
        [DataMember(Order = 1)]
        public string Name { set; get; }

        [DataMember(Order = 2)]
        public string Version { set; get; }

        [DataMember(Order = 3)]
        public ApiMessage Message { get; set; }

        [DataMember(Order = 4)]
        public object Result { get; set; }

        public ApiResponse()
        {
            this.Message = new ApiMessage();
        }
    }

    [DataContract]
    public class ApiMetaRequest : EbServiceStackNoAuthRequest, IReturn<ApiMetaResponse>
    {
        [DataMember(Order = 1)]
        public string Name { set; get; }

        [DataMember(Order = 2)]
        public string Version { set; get; }

        [DataMember(Order = 3)]
        public string SolutionId { set; get; }
    }

    [DataContract]
    public class ApiMetaResponse
    {
        [DataMember(Order = 1)]
        public List<Param> Params { set; get; }

        [DataMember(Order = 2)]
        public string Name { set; get; }

        [DataMember(Order = 3)]
        public string Version { set; get; }
    }

    [DataContract]
    public class ApiAllMetaRequest : EbServiceStackNoAuthRequest, IReturn<ApiAllMetaResponse>
    {
        [DataMember(Order = 1)]
        public string SolutionId { set; get; }
    }

    [DataContract]
    public class ApiAllMetaResponse
    {
        [DataMember(Order = 1)]
        public List<EbObjectWrapper> AllMetas { set; get; }

        public ApiAllMetaResponse()
        {
            AllMetas = new List<EbObjectWrapper>();
        }
    }

    [DataContract]
    public class ApiReqJsonRequest : EbServiceStackAuthRequest, IReturn<ApiReqJsonResponse>
    {
        [DataMember(Order = 1)]
        public ListOrdered Components { set; get; }
    }

    [DataContract]
    public class ApiReqJsonResponse
    {
        [DataMember(Order = 1)]
        public List<Param> Params { set; get; }
    }

    [DataContract]
    public class ApiMessage
    {
        [DataMember(Order = 1)]
        public string Status { get; set; }

        [DataMember(Order = 2)]
        public string Description { get; set; }

        [DataMember(Order = 3)]
        public string ExecutedOn { set; get; }

        [DataMember(Order = 4)]
        public string ExecutionTime { set; get; }

        [DataMember(Order = 4)]
        public ApiErrorCode ErrorCode { set; get; }
    }

    public class ApiComponent
    {
        [DataMember(Order = 1)]
        public string Name { get; set; }

        [DataMember(Order = 2)]
        public string Version { get; set; }

        [DataMember(Order = 3)]
        public string Parameters { get; set; }
    }

    [DataContract]
    public class ApiByNameRequest : EbServiceStackAuthRequest, IReturn<ApiByNameResponse>
    {
        [DataMember(Order = 1)]
        public string Version { set; get; }

        [DataMember(Order = 2)]
        public string Name { set; get; }
    }

    [DataContract]
    public class ApiByNameResponse : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public ResponseStatus ResponseStatus { get; set; }

        [DataMember(Order = 2)]
        public EbApi Api { get; set; }
    }

    [RuntimeSerializable]
    public class JsonTableSet
    {
        public List<JsonTable> Tables { set; get; }

        public JsonTableSet()
        {
            Tables = new List<JsonTable>();
        }
    }

    public class JsonTable
    {
        public string TableName { set; get; }

        public List<JsonColVal> Rows { set; get; }

        public JsonTable()
        {
            this.Rows = new List<JsonColVal>();
        }
    }

    public class JsonColVal : Dictionary<string, object>
    {

    }

    public class FormSqlData
    {
        public List<JsonTable> JsonColoumsInsert { set; get; }

        public List<JsonTable> JsonColoumsUpdate { set; get; }

        public FormSqlData()
        {
            this.JsonColoumsInsert = new List<JsonTable>();

            this.JsonColoumsUpdate = new List<JsonTable>();
        }
    }

    public class ResultWrapper
    {
        public ResultWrapper()
        {
            this.InputParams = new List<Param>();
        }

        public object Result { get; set; }

        public List<Param> InputParams { set; get; }
    }

    [Serializable()]
    public class ApiException : Exception
    {
        public ApiException() : base() { }

        public ApiException(string message) : base(message) { }

        public ApiException(string message, Exception innerException) : base(message, innerException) { }
    }

    //[Serializable()]
    //public class ExplicitExitException : Exception
    //{
    //    public ExplicitExitException() : base() { }

    //    public ExplicitExitException(string message) : base(message) { }
    //}

    [RuntimeSerializable]
    public class ApiScript
    {
        public Type ResultType { get { return this.Data.GetType(); } }

        public string Data { set; get; }
    }

    public class ApiAuthResponse
    {
        public bool IsValid { set; get; }

        public string BToken { set; get; }

        public string RToken { set; get; }

        public int UserId { set; get; }

        public string UserAuthId { set; get; }

        public string DisplayName { set; get; }

        public User User { set; get; }

        public byte[] DisplayPicture { set; get; }

        public bool Is2FEnabled { get; set; }

        public string TwoFAToken { set; get; }

        public bool TwoFAStatus { set; get; }

        public string TwoFAToAddress { set; get; }

        public string Message { set; get; }
    }

    public class ApiGenerateOTPResponse : IEbApiStatusCode
    {
        public bool IsValid { set; get; }

        public HttpStatusCode StatusCode { get; set; }
    }

    public class ApiFileData
    {
        public string FileName { set; get; }

        public string FileType { set; get; }

        public int FileRefId { set; get; }
    }

    public class ApiFileResponse
    {
        public string ContentType { set; get; }

        public byte[] Bytea { set; get; }
    }
}
