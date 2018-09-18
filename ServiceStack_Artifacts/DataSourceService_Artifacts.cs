using ExpressBase.Common;
using ExpressBase.Common.Data;
using ExpressBase.Common.EbServiceStack.ReqNRes;
using ExpressBase.Data;
using ServiceStack;
using ServiceStack.Text;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace ExpressBase.Objects.ServiceStack_Artifacts
{
    [Route("/ds")]
    [Route("/ds/data/{RefId}")]
    [DataContract]
    public class DataSourceDataRequest : IReturn<DataSourceDataResponse>, IEbSSRequest
    {
        [DataMember(Order = 0)]
        public string RefId { get; set; }

        [DataMember(Order = 1)]
        public int Start { get; set; }

        [DataMember(Order = 2)]
        public int Length { get; set; }

        [DataMember(Order = 3)]
        public int Draw { get; set; }

        [DataMember(Order = 4)]
        public int OrderByDir { get; set; }

        [DataMember(Order = 5)]
        public string OrderByCol { get; set; }

        [DataMember(Order = 6)]
        public string Token { get; set; }

        [DataMember(Order = 7)]
        public string rToken { get; set; }

        [DataMember(Order = 8)]
        public string SolnId { get; set; }

        [DataMember(Order = 9)]
        public int UserId { get; set; }

        [DataMember(Order = 10)]
        public List<Param> Params { get; set; }

        [DataMember(Order = 11)]
        public List<TFilters> TFilters { get; set; }
    }

    [DataContract]
    public class TFilters
    {
        [DataMember(Order = 1)]
        public string Operator { get; set; }

        [DataMember(Order = 2)]
        public string Column { get; set; }

        [DataMember(Order = 3)]
        public string Value { get; set; }

        [DataMember(Order = 3)]
        public string Type { get; set; }
    }

    [DataContract]
    public class DataSourceDataRequestbot : IReturn<string>, IEbSSRequest
    {
        [DataMember(Order = 0)]
        public string RefId { get; set; }

        [DataMember(Order = 1)]
        public int Start { get; set; }

        [DataMember(Order = 2)]
        public int Length { get; set; }

        [DataMember(Order = 3)]
        public int Draw { get; set; }

        [DataMember(Order = 4)]
        public int OrderByDir { get; set; }

        [DataMember(Order = 5)]
        public string OrderByCol { get; set; }

        [DataMember(Order = 6)]
        public string Token { get; set; }

        [DataMember(Order = 7)]
        public string rToken { get; set; }

        [DataMember(Order = 8)]
        public string SolnId { get; set; }

        [DataMember(Order = 9)]
        public int UserId { get; set; }

        [DataMember(Order = 10)]
        public List<Dictionary<string, object>> Params { get; set; }

        [DataMember(Order = 11)]
        public List<Dictionary<string, string>> TFilters { get; set; }
    }

    [DataContract]
    public class DataSourceDataResponsebot : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public int Draw { get; set; }

        [DataMember(Order = 2)]
        public int RecordsTotal { get; set; }

        [DataMember(Order = 3)]
        public int RecordsFiltered { get; set; }

        //[DataMember(Order = 4)]
        //public RowColletion2 Data { get; set; }

        [DataMember(Order = 5)]
        public string Token { get; set; }

        [DataMember(Order = 6)]
        public ResponseStatus ResponseStatus { get; set; }

        //[DataMember(Order = 7)]
        public DataSet DataSet { get; set; }
    }


    [Route("/ds")]
    [Route("/ds/columns/{RefId}")]
    [DataContract]
    public class DataSourceColumnsRequest : IReturn<DataSourceColumnsResponse>, IEbSSRequest
    {
        [DataMember(Order = 1)]
        public string RefId { get; set; }

        [DataMember(Order = 2)]
        public string SearchText { get; set; }

        [DataMember(Order = 3)]
        public string OrderByDirection { get; set; }

        [DataMember(Order = 4)]
        public string SelectedColumnName { get; set; }

        [DataMember(Order = 5)]
        public string Token { get; set; }

        [DataMember(Order = 6)]
        public string rToken { get; set; }

        [DataMember(Order = 7)]
        public string SolnId { get; set; }

        [DataMember(Order = 8)]
        public int UserId { get; set; }

        [DataMember(Order = 9)]
        public List<Param> Params { get; set; }

        [DataMember(Order = 10)]
        public string CrossDomain { get; set; }
    }

    public class DataSourceDataSetRequest : IReturn<DataSourceDataResponse>, IEbSSRequest
    {
        [DataMember(Order = 1)]
        public string RefId { get; set; }

        [DataMember(Order = 2)]
        public string SolnId { get; set; }

        [DataMember(Order = 3)]
        public int UserId { get; set; }

        [DataMember(Order = 4)]
        public List<Param> Params { get; set; }


    }

    [DataContract]
    [Csv(CsvBehavior.FirstEnumerable)]
    public class DataSourceDataSetResponse : IEbSSResponse
    {
        [DataMember(Order = 5)]
        public string Token { get; set; }

        [DataMember(Order = 6)]
        public ResponseStatus ResponseStatus { get; set; }

        [DataMember(Order = 7)]
        public EbDataSet DataSet { get; set; }
    }

    [DataContract]
    [Csv(CsvBehavior.FirstEnumerable)]
    public class DataSourceDataResponse : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public int Draw { get; set; }

        [DataMember(Order = 2)]
        public int RecordsTotal { get; set; }

        [DataMember(Order = 3)]
        public int RecordsFiltered { get; set; }

        [DataMember(Order = 4)]
        public RowColletion Data { get; set; }

        [DataMember(Order = 5)]
        public string Token { get; set; }

        [DataMember(Order = 6)]
        public ResponseStatus ResponseStatus { get; set; }

        [DataMember(Order = 7)]
        public DataSet DataSet { get; set; }

        [DataMember(Order = 8)]
        public bool Ispaged { get; set; }

        [DataMember(Order = 9)]
        public RowColletion FormattedData { get; set; }

        [DataMember(Order = 10)]
        public LevelInfoCollection Levels { get; set; }
        
    }

    [DataContract]
    [Csv(CsvBehavior.FirstEnumerable)]
    public class DataSourceColumnsResponse : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public List<ColumnColletion> Columns { get; set; }

        [DataMember(Order = 3)]
        public bool IsPaged { get; set; }

        [DataMember(Order = 4)]
        public ResponseStatus ResponseStatus { get; set; }

        [DataMember(Order = 5)]
        public List<Param> ParamsList { get; set; }

        public bool IsNull
        {
            get
            {
                return (this.Columns == null || this.Columns.Count == 0);
            }
        }
    }

    [DataContract]
    public class EbObjectWithRelatedDVRequest : EbServiceStackAuthRequest, IReturn<EbObjectWithRelatedDVResponse>
    {
        [DataMember(Order = 1)]
        public string Refid { get; set; }

        [DataMember(Order = 2)]
        public string Ids { get; set; }

        [DataMember(Order = 3)]
        public string DsRefid { get; set; }

    }

    [DataContract]
    public class EbObjectWithRelatedDVResponse : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public EbDataVisualization Dsobj { get; set; }

        [DataMember(Order = 2)]
        public List<EbObjectWrapper> DvList { get; set; }

        [DataMember(Order = 3)]
        public List<EbObjectWrapper> DvTaggedList { get; set; }

        [DataMember(Order = 4)]
        public ResponseStatus ResponseStatus { get; set; }
    }

    [DataContract]
    public class FDDataRequest : IReturn<FDDataResponse>, IEbSSRequest
    {
        [DataMember(Order = 0)]
        public string RefId { get; set; }

        [DataMember(Order = 1)]
        public int Start { get; set; }

        [DataMember(Order = 2)]
        public int Length { get; set; }

        [DataMember(Order = 3)]
        public int Draw { get; set; }

        [DataMember(Order = 4)]
        public int OrderByDir { get; set; }

        [DataMember(Order = 5)]
        public string OrderByCol { get; set; }

        [DataMember(Order = 6)]
        public string Token { get; set; }

        [DataMember(Order = 7)]
        public string rToken { get; set; }

        [DataMember(Order = 8)]
        public string SolnId { get; set; }

        [DataMember(Order = 9)]
        public int UserId { get; set; }

        [DataMember(Order = 10)]
        public List<Param> Params { get; set; }

        [DataMember(Order = 11)]
        public List<TFilters> TFilters { get; set; }
    }

    [DataContract]
    [Csv(CsvBehavior.FirstEnumerable)]
    public class FDDataResponse : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public int Draw { get; set; }

        [DataMember(Order = 2)]
        public int RecordsTotal { get; set; }

        [DataMember(Order = 3)]
        public int RecordsFiltered { get; set; }

        [DataMember(Order = 4)]
        public RowColletion Data { get; set; }

        [DataMember(Order = 5)]
        public string Token { get; set; }

        [DataMember(Order = 6)]
        public ResponseStatus ResponseStatus { get; set; }

        [DataMember(Order = 7)]
        public DataSet DataSet { get; set; }

        [DataMember(Order = 8)]
        public bool Ispaged { get; set; }
    }

    public class LevelInfo
    {
        public string LevelText { get; set; }
        public int Count { get; set; }
        public int RowIndex { get; set; }
        public string Type { get; set; }
        public int Level { get; set; }
        public string GroupString { get; set; }
    }

    public class LevelInfoCollection : List<LevelInfo>
    {
        public LevelInfo Update(int _index, int _count, int level)
        {
            foreach (LevelInfo _lvl in this)
            {
                if (_lvl.RowIndex == _index && _lvl.GroupString.IndexOf("group-sum") == -1 && _lvl.Level == level)
                {
                    _lvl.Count = _count;
                    return _lvl;
                }
            }
            return null;
        }

        public LevelInfo PreviousLevelCheck(int _index, int level)
        {
            foreach (LevelInfo _lvl in this)
            {
                if (_lvl.RowIndex == _index && _lvl.GroupString.IndexOf("group-sum") == -1 && _lvl.Level == level)
                {
                    return _lvl;
                }
            }
            return null;
        }

        public LevelInfo UpdatePreviousLevelCount(int _index, int _count, int level)
        {
            foreach (LevelInfo _lvl in this)
            {
                if (_lvl.RowIndex == _index && _lvl.GroupString.IndexOf("group-sum") == -1 && _lvl.Level == level - 1)
                {
                    _lvl.Count = _count;
                    return _lvl;
                }
            }
            return null;
        }

        public LevelInfo CurrentLevelDataCheck(int level, string data)
        {
            foreach (LevelInfo _lvl in this)
            {
                if ( _lvl.LevelText != data && _lvl.Level == level && _lvl.GroupString.IndexOf("group-sum") == -1)
                {
                    return _lvl;
                }
            }
            return null;
        }

        public int GetCount(int r1, int r2, int level)
        {
            int Count = 0;
            foreach (LevelInfo _lvl in this)
            {
                if (_lvl.RowIndex >= r1 &&  _lvl.RowIndex <= r2 && _lvl.Level == level && _lvl.GroupString.IndexOf("group-sum") == -1)
                {
                    Count++;
                }
            }
            return Count;
        }
    }
}
