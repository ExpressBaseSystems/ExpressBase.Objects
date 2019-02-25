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
        public string SolnId { get; set; }

        public int UserId { get; set; }
    }

    [DataContract]
    public class GetDbTablesResponse : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public EbDbExplorerTablesDict Tables { get; set; }

        [DataMember(Order = 2)]
        public ResponseStatus ResponseStatus { get; set; }

    }

    [DataContract]
    public class DbClientQueryRequest
    {
        [DataMember(Order =1)]
        public string Query { get; set; }
    }

    [DataContract]
    public class DbClientQueryResponse
    {
        [DataMember(Order = 1)]
        public EbDataSet Dataset { get; set; }

        [DataMember(Order = 1)]
        public List<DVColumnCollection> ColumnCollection { get; set; }

        [DataMember(Order = 1)]
        public List<RowColletion> RowCollection { get; set; }
    }

    public class EbDbExplorerTablesDict
    {
        public Dictionary<string, EbDbExplorerTable> TableCollection { set; get; }

        public EbDbExplorerTablesDict()
        {
            this.TableCollection = new Dictionary<string, EbDbExplorerTable>();
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
        public List<EbDbExplorerColumn> Columns {set;get;}

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
