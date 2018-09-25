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
        public List<GroupingDetails> Levels { get; set; }
        
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

    public class GroupingDetails
    {
        public GroupingDetails()
        {
            GroupingTexts = new List<string>();
        }

        public GroupingDetails(GroupingDetails CurLevel)
        {

            CurrentLevel = CurLevel.CurrentLevel;
            RowIndex = CurLevel.RowIndex;
            LevelText = CurLevel.LevelText;
            GroupingTexts = new List<string>(CurLevel.GroupingTexts);
            ParentLevelText = CurLevel.ParentLevelText;
            InsertionType = CurLevel.InsertionType;
            LevelCount = CurLevel.LevelCount;
            ParentLevel = CurLevel.ParentLevel;
            Html = CurLevel.Html;
            //FooterHtml = CurLevel.FooterHtml;
            //AggregateValues = new Dictionary<int, int>(CurLevel.AggregateValues);
            GroupingCount = CurLevel.GroupingCount;
            IsHeader = CurLevel.IsHeader;
            GroupingString = CurLevel.GroupingString;
        }

        /// <summary>
        /// The concatenated grouping string for a row grouping
        /// </summary>
        public string GroupingString { get; set; }

        /// <summary>
        /// The concatenated grouping string for the previous row grouping
        /// </summary>
        public string PreviousGroupingString { get; set; }

        /// <summary>
        /// The current level's group string - only used in multi-level grouping. It is an element of the GroupingTexts list, 
        /// but stored separately for convenience. Might be refactored away later.
        /// </summary>
        public string LevelBasedGroupingString { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public int GroupingCount { get; set; }
        
        /// <summary>
        /// The level of row grouping for an element. Used in multi-level row grouping.
        /// FOr single level, it is always 1.
        /// </summary>
        public int CurrentLevel { get; set; }
        
        /// <summary>
        /// Stores the level previous to the current level.
        /// Updated when the parent level text changes.
        /// Used in multi-level row grouping. For single level, it is always set to 0.
        /// </summary>
        public int ParentLevel { get; set; }
        
        /// <summary>
        /// Contains the name of the grouping text for a particular row grouping at a particular level.
        /// Changes for each grouping at each level.
        /// </summary>
        public string LevelText { get; set; }
        
        /// <summary>
        /// The list of strings that denotes each row grouping string.
        /// For multiple level row grouping, it denotes each different level too.
        /// For single level, each column value is denoted separately for ease of processing.
        /// </summary>
        public List<string> GroupingTexts { get; set; }

        /// <summary>
        /// The index in the EbDataTable object of the row before/after which grouping should be inserted.
        /// </summary>
        public int RowIndex { get; set; }
        
        /// <summary>
        /// The insertion type for HTML contents at a particular row index. 
        /// Possible values are 'Before' and 'After'.
        /// </summary>
        public string InsertionType { get; set; }
        
        /// <summary>
        /// Number of groupings in a particular level.
        /// If the grouping is single level or the final single level iteration of multi-level grouping,
        /// this contains the actual number of data rows.
        /// </summary>
        public int LevelCount { get; set; }
        //public Dictionary<int, int> AggregateValues { get; set; }
        
        /// <summary>
        /// The level text of the parent level. For different row groupings at the same level, it should be the same.
        /// Empty string for single level or topmost level of multi-level grouping.
        /// </summary>
        public string ParentLevelText { get; set; }
        
        /// <summary>
        /// The HTML text generated for the particular header or footer
        /// </summary>
        public string Html { get; set; }
        
        /// <summary>
        /// Flag to check whether the generated HTML is a header or footer.
        /// </summary>
        public bool IsHeader { get; set; }

        ///// <summary>
        ///// The number where the current index in the Table processed is 
        ///// taken and returned to the calling function, so that many elements
        ///// can be skipped over in the next iteration.
        ///// </summary>
        //public int CurrentIndex { get; set; }
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
                if (_lvl.RowIndex == _index-1 && _lvl.GroupString.IndexOf("group-sum") == -1 && _lvl.Level == level-1)
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
