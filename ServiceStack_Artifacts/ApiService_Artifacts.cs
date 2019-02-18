using ExpressBase.Common;
using ExpressBase.Common.Data;
using ExpressBase.Common.EbServiceStack.ReqNRes;
using Newtonsoft.Json;
using ServiceStack;
using ServiceStack.Text;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

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
    public class ApiComponetRequest: IReturn<ApiResponse>, IEbSSRequest
    {
        public string SolnId { get; set; }

        public int UserId { get; set; }

        [DataMember(Order = 1)]
        public EbApiWrapper Component { set; get; }

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
    public class ApiMessage
    {
        [DataMember(Order = 1)]
        public string Status { get; set; }

        [DataMember(Order = 2)]
        public string Description { get; set; }

        [DataMember(Order = 3)]
        [JsonProperty("Executed On")]
        public string ExecutedOn { set; get; }

        [DataMember(Order = 4)]
        [JsonProperty("Execution Time")]
        public string ExecutionTime { set; get; }
    }

    public class ApiComponent
    {
        [DataMember(Order = 1)]
        public string Name { get; set; }

        [DataMember(Order = 1)]
        public string Version { get; set; }
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
        public const string SQL_FUNC_HEADER = @"CREATE OR REPLACE FUNCTION {0}(insert_json jsonb,update_json jsonb)
RETURNS void
LANGUAGE {1} AS $BODY$";

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
}
