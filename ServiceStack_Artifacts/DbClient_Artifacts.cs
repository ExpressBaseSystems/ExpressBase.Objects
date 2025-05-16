using ExpressBase.Common;
using ExpressBase.Common.Data;
using ExpressBase.Common.EbServiceStack.ReqNRes;
using ExpressBase.Objects.Objects.DVRelated;
using ServiceStack;
using System;
using System.Collections.Generic;
using System.Data;
using System.Runtime.Serialization;
using System.Text;

namespace ExpressBase.Objects.ServiceStack_Artifacts
{
    [DataContract]
    public class GetDbTablesRequest : IReturn<GetDbTablesResponse>, IEbSSRequest
    {
        [DataMember(Order = 1)]
        public string SolnId { get; set; }
        [DataMember(Order = 2)]
        public int UserId { get; set; }
        [DataMember(Order = 3)]
        public bool IsAdminOwn { get; set; }
        [DataMember(Order = 4)]
        public string ClientSolnid { get; set; }

        [DataMember(Order = 5)]
        public Boolean SupportLogin { get; set; }
    }

    [DataContract]
    public class GetDbTablesResponse : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public EbDbExplorerTablesDict Tables { get; set; }

        [DataMember(Order = 2)]
        public ResponseStatus ResponseStatus { get; set; }

        [DataMember(Order = 3)]
        public string DB_Name { get; set; }

        [DataMember(Order = 4)]
        public int TableCount { get; set; }

        [DataMember(Order = 5)]
        public List<string> SolutionCollection { get; set; }

        [DataMember(Order = 6)]
        public string Message { get; set; }

    }

    [DataContract]
    public class DbClientCreateRequest
    {
        [DataMember(Order = 1)]
        public string Query { get; set; }

        [DataMember(Order = 2)]
        public bool IsAdminOwn { get; set; }

        [DataMember(Order = 3)]
        public string ClientSolnid { get; set; }
    }

    [DataContract]
    public class DbClientSelectRequest
    {
        [DataMember(Order = 1)]
        public string Query { get; set; }


        [DataMember(Order = 2)]
        public bool IsAdminOwn { get; set; }

        [DataMember(Order = 3)]
        public string ClientSolnid { get; set; }
    }

    [DataContract]
    public class DbClientInsertRequest
    {
        [DataMember(Order = 1)]
        public string Query { get; set; }

        [DataMember(Order = 2)]
        public bool IsAdminOwn { get; set; }

        [DataMember(Order = 3)]
        public string ClientSolnid { get; set; }
    }

    [DataContract]
    public class DbClientIndexRequest
    {
        [DataMember(Order = 1)]
        public string TableName { get; set; }

        [DataMember(Order = 2)]
        public string IndexName { get; set; }

        [DataMember(Order = 3)]
        public string IndexColumns { get; set; }

        [DataMember(Order = 4)]
        public string ClientSolnid { get; set; }

        [DataMember(Order = 5)]
        public bool IsAdminOwn { get; set; }
    }
    [DataContract]
    public class DbClientIndexResponse
    {
        [DataMember(Order = 1)]
        public int Result { get; set; }

        [DataMember(Order = 2)]
        public string Message { get; set; }

        [DataMember(Order = 3)]
        public DBOperations Type { get; set; }
    }


    [DataContract]
    public class DbClientEditIndexRequest
    {
        [DataMember(Order = 1)]
        public string CurrentIndexName { get; set; }

        [DataMember(Order = 2)]
        public string NewIndexName { get; set; }

        [DataMember(Order = 3)]
        public string TableName { get; set; }

        [DataMember(Order = 4)]
        public string ClientSolnid { get; set; }

        [DataMember(Order = 5)]
        public bool IsAdminOwn { get; set; }
    }

    [DataContract]
    public class DbClientEditIndexResponse
    {
        [DataMember(Order = 1)]
        public int Result { get; set; }

        [DataMember(Order = 2)]
        public string Message { get; set; }

        public DBOperations Type { get; set; }
    }

    [DataContract]
    public class DbClientConstraintRequest
    {
        [DataMember(Order = 1)]
        public string TableName { get; set; }

        [DataMember(Order = 2)]
        public string ColumnName { get; set; }

        [DataMember(Order = 3)]
        public string ConstraintType { get; set; }

        [DataMember(Order = 4)]
        public string ConstraintName { get; set; }

        [DataMember(Order = 5)]
        public string ClientSolnid { get; set; }

        [DataMember(Order = 6)]
        public bool IsAdminOwn { get; set; }
    }

    [DataContract]
    public class DbClientConstraintResponse
    {
        [DataMember(Order = 1)]
        public int Result { get; set; }

        [DataMember(Order = 2)]
        public string Message { get; set; }

        public DBOperations Type { get; set; }
    }


    [DataContract]
    public class DbClientDropRequest
    {
        [DataMember(Order = 1)]
        public string Query { get; set; }

        [DataMember(Order = 2)]
        public bool IsAdminOwn { get; set; }

        [DataMember(Order = 3)]
        public string ClientSolnid { get; set; }
    }

    [DataContract]
    public class DbClientDeleteRequest
    {
        [DataMember(Order = 1)]
        public string Query { get; set; }

        [DataMember(Order = 2)]
        public bool IsAdminOwn { get; set; }

        [DataMember(Order = 3)]
        public string ClientSolnid { get; set; }
    }

    [DataContract]
    public class DbClientAlterRequest
    {
        [DataMember(Order = 1)]
        public string Query { get; set; }

        [DataMember(Order = 2)]
        public bool IsAdminOwn { get; set; }

        [DataMember(Order = 3)]
        public string ClientSolnid { get; set; }
    }

    [DataContract]
    public class DbClientTruncateRequest
    {
        [DataMember(Order = 1)]
        public string Query { get; set; }

        [DataMember(Order = 2)]
        public bool IsAdminOwn { get; set; }

        [DataMember(Order = 3)]
        public string ClientSolnid { get; set; }
    }

    [DataContract]
    public class DbClientUpdateRequest
    {
        [DataMember(Order = 1)]
        public string Query { get; set; }

        [DataMember(Order = 2)]
        public bool IsAdminOwn { get; set; }

        [DataMember(Order = 3)]
        public string ClientSolnid { get; set; }
    }

    [DataContract]
    public class DbClientQueryResponse
    {
        [DataMember(Order = 1)]
        public EbDataSet Dataset { get; set; }

        [DataMember(Order = 2)]
        public int Result { get; set; }

        [DataMember(Order = 3)]
        public List<DVColumnCollection> ColumnCollection { get; set; }

        [DataMember(Order = 4)]
        public List<RowColletion> RowCollection { get; set; }

        [DataMember(Order = 5)]
        public string Message { get; set; }

        [DataMember(Order = 5)]
        public DBOperations Type { get; set; }
    }

    public class EbDbExplorerTablesDict
    {
        public Dictionary<string, EbDbExplorerTable> TableCollection { set; get; }
        public List<EbDbExplorerFunctions> FunctionCollection { set; get; }

        public EbDbExplorerTablesDict()
        {
            this.TableCollection = new Dictionary<string, EbDbExplorerTable>();
            this.FunctionCollection = new List<EbDbExplorerFunctions>();
        }
    }
    public class EbDbExplorerColumn
    {
        [DataMember(Order = 1)]
        public string ColumnName { get; set; }

        [DataMember(Order = 2)]
        public string ColumnType { get; set; }

        [DataMember(Order = 3)]
        public string ColumnKey { get; set; }

        [DataMember(Order = 4)]
        public string ColumnTable { get; set; }

        [DataMember(Order = 5)]
        public string ConstraintName { get; set; } // Add this property
    }


    public class EbDbExplorerFunctions
    {
        [DataMember(Order = 1)]
        public string FunctionName { get; set; }

        [DataMember(Order = 2)]
        public string FunctionQuery { get; set; }
    }

    //public class MyColumnCollection: List<MyColumn>
    //{

    //}

    public class EbDbExplorerTable //: List<MyColumn>
    {
        [DataMember(Order = 1)]
        public string Name { get; set; }

        [DataMember(Order = 2)]
        public string Schema { get; set; }

        [DataMember(Order = 3)]
        public List<string> Index { set; get; }

        [DataMember(Order = 4)]
        public Sequence Sequence { set; get; }

        [DataMember(Order = 5)]
        public List<EbDbExplorerColumn> Columns { set; get; }

        public EbDbExplorerTable()
        {
            Index = new List<string>();
            Sequence = new Sequence();
            Columns = new List<EbDbExplorerColumn>();
        }
    }


    public class Sequence
    {
    }

}
