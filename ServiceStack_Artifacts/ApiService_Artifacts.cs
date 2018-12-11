using ExpressBase.Common.EbServiceStack.ReqNRes;
using ServiceStack;
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

    public class JsonColVal:Dictionary<string, dynamic>
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
        public const string SQL_FUNC_HEADER = @"CREATE OR REPLACE FUNCTION {0}(insert_json,update_json)
RETURNS void
LANGUAGE {1} AS $BODY$"; 
    }
}
