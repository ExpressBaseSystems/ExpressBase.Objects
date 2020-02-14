using ExpressBase.Common;
using ExpressBase.Common.Data;
using ExpressBase.Common.EbServiceStack.ReqNRes;
using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using ExpressBase.Common.Structures;
using Newtonsoft.Json;
using ServiceStack;
using ServiceStack.Text;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;
using ExpressBase.Security;
using ExpressBase.Common.LocationNSolution;

namespace ExpressBase.Objects.ServiceStack_Artifacts
{
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
    public class ApiComponetRequest : IReturn<ApiResponse>, IEbSSRequest
    {
        public string SolnId { get; set; }

        public int UserId { get; set; }

        [DataMember(Order = 1)]
        public ApiResources Component { set; get; }

        [DataMember(Order = 2)]
        public List<Param> Params { set; get; }
    }

    [DataContract]
    public class ApiRequest : IReturn<ApiResponse>, IEbSSRequest
    {
        public string SolnId { get; set; }

        public int UserId { get; set; }

        [DataMember(Order = 1)]
        public string Version { set; get; }

        [DataMember(Order = 2)]
        public string Name { set; get; }

        [DataMember(Order = 3)]
        public Dictionary<string, object> Data { set; get; }
    }

    [DataContract]
    public class ApiResponse
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
    public class ApiReqJsonRequest : IReturn<ApiReqJsonResponse>, IEbSSRequest
    {
        public string SolnId { get; set; }

        public int UserId { get; set; }

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
    public class ApiByNameRequest : IReturn<ApiByNameResponse>, IEbSSRequest
    {
        public string SolnId { get; set; }

        public int UserId { get; set; }

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

    public static class SqlConstants
    {
        //        public const string SQL_FUNC_HEADER = @"CREATE OR REPLACE FUNCTION {0}(insert_json jsonb,update_json jsonb)
        //RETURNS void
        //LANGUAGE {1} AS $BODY$";

        public const string JSON_ROW_SELECT = @"CREATE OR REPLACE FUNCTION {0}(_json,table_name)
RETURNS jsonb
LANGUAGE {1} AS $BODY$
DECLARE 
    temp_table jsonb;
BEGIN
    SELECT
        _table->'Rows' 
    FROM 
        jsonb_array_elements(_json) _table 
    INTO 
        temp_table 
    WHERE 
        _table->>'TableName' = table_name;
    RETURN temp_table;
END;";

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

    public class ObjWrapperInt
    {
        public int ObjectType { get; set; }

        public EbObject EbObj { set; get; }
    }

    [Serializable()]
    public class ApiException : Exception
    {
        public ApiException() : base() { }

        public ApiException(string message) : base(message) { }

        public ApiException(string message, Exception innerException) : base(message, innerException) { }
    }

    [Serializable()]
    public class ExplicitExitException : Exception
    {
        public ExplicitExitException() : base() { }

        public ExplicitExitException(string message) : base(message) { }
    }

    [RuntimeSerializable]
    public class ApiScript
    {
        public Type ResultType { get { return this.Data.GetType(); } }

        public string Data { set; get; }
    }

    public static class ApiConstants
    {
        public const string CIRCULAR_REF = "Cannot call {0} from {0}";

        public const string DESCRPT_ERR = "Error at position {0}, Resource {1} failed to execute. Resource Name = '{2}'";

        public const string EXE_FAIL = "Execution failed,{0}";

        public const string UNSET_PARAM = "Parameter {0} must be set";

        public const string EXE_SUCCESS = "Execution success";

        public const string MAIL_SUCCESS = "The mail has been sent successfully to {0} with subject {1} and cc {2}";

        public const string SUCCESS = "Success";

        public const string FAIL = "Failed";

        public const string API_NOTFOUND = "Api,{0} does not Exist";
    }

    public enum ApiErrorCode
    {
        NotFound = 404,
        Success = 1,
        Failed = -1,
        ParamNFound = 0,
        ExplicitExit = 255
    }

    public enum ApiMethods
    {
        POST = 1,
        GET = 2,
        //PUT = 3,
        //PATCH = 4,
        //DELETE = 5,
    }

    public class ApiAuthResponse
    {
        public string BToken { set; get; }

        public string RToken { set; get; }

        public bool IsValid { set; get; }

        public int UserId { set; get; }

        public string DisplayName { set; get; }

        public User User { set; get; }

        public byte[] DisplayPicture { set; get; }

        public List<EbLocation> Locations { get; set; }

        public ApiAuthResponse()
        {
            Locations = new List<EbLocation>();
        }
    }

    public class ApiFileData
    {
        public string FileName { set; get; }

        public string FileType { set; get; }

        public int FileRefId { set; get; }
    }
}
