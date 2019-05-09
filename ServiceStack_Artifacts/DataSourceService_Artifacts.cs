using ExpressBase.Common;
using ExpressBase.Common.Data;
using ExpressBase.Common.EbServiceStack.ReqNRes;
using ExpressBase.Common.Singletons;
using ExpressBase.Common.Structures;
using ExpressBase.Data;
using ExpressBase.Objects.Objects.DVRelated;
using Newtonsoft.Json;
using ServiceStack;
using ServiceStack.Text;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
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
        //public LevelInfoCollection Levels { get; set; }
        public List<GroupingDetails> Levels { get; set; }

        [DataMember(Order = 11)]
        public List<string> Permission { get; set; }

        [DataMember(Order = 12)]
        public Dictionary<int, List<object>> Summary { get; set; }

        [DataMember(Order = 13)]
        public byte[] excel_file { get; set; }

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

    public abstract class GroupingDetails : IComparable
    {
        [JsonIgnore]
        public Dictionary<string, GroupingDetails> RowGrouping { get; set; }

        [JsonIgnore]
        public bool IsMultiLevel { get; set; }

        [JsonIgnore]
        public int ColumnCount { get; set; }

        [JsonIgnore]
        private int _currentLevel = 0;
        [JsonIgnore]
        public int CurrentLevel
        {
            get
            {
                if (_currentLevel == 0)
                    _currentLevel = Regex.Matches(CollectionKey, ":-:").Count + 1;
                return _currentLevel;
            }
        }

        private int _rowIndex = -1;
        public int RowIndex
        {
            get
            {
                return _rowIndex;
            }
            set
            {
                _rowIndex = value;
            }
        }

        private int _sortIndex = -1;
        public int SortIndex
        {
            get { return _sortIndex; }
            set { _sortIndex = value; }
        }

        public string InsertionType { get; set; }

        [JsonIgnore]
        public int ChildCount { get; set; }

        //[JsonIgnore]
        public int GroupingCount { get; set; }

        [JsonIgnore]
        public int LevelCount { get; set; }

        public virtual string Html { get; set; }

        [JsonIgnore]
        public string CollectionKey { get; set; }

        public int CompareTo(object obj)
        {
            return SortIndex.CompareTo((obj as GroupingDetails).SortIndex);
        }
    }

    public class HeaderGroupingDetails : GroupingDetails
    {
        private const string Delimiter = ":-:";
        private HeaderGroupingDetails _parentHeader = null;

        [JsonIgnore]
        public HeaderGroupingDetails ParentHeader
        {
            get
            {
                if (_parentHeader == null && IsMultiLevel)
                {
                    var index = CollectionKey.LastIndexOf(Delimiter);

                    if (index > 0)
                    {
                        _parentHeader = RowGrouping[CollectionKey.Substring(0, index)] as HeaderGroupingDetails;
                        _parentHeader.LevelCount++;
                    }
                }

                return _parentHeader;
            }
        }

        public void SetRowIndex(int index)
        {
            this.RowIndex = index;
            if (ParentHeader != null && ParentHeader.RowIndex == -1)
                ParentHeader.SetRowIndex(index);
        }

        public void SetSortIndex(int index)
        {
            this.SortIndex = index;
            if (this.ParentHeader != null && this.ParentHeader.SortIndex == -1)
                this.ParentHeader.SetSortIndex(index - 1);
        }

        [JsonIgnore]
        public List<DVBaseColumn> RowGroupingColumns
        {
            get;
            set;
        }

        [JsonIgnore]
        private string[] _groupingTexts = null;

        [JsonIgnore]
        public string[] GroupingTexts
        {
            get
            {
                if (_groupingTexts == null)
                    _groupingTexts = CollectionKey.Split(":-:");
                return _groupingTexts;
            }
        }

        [JsonIgnore]
        public int TotalLevels { get; set; }

        public HeaderGroupingDetails() { }

        public HeaderGroupingDetails(bool _isMultiLevel, int _columnCount)
        {
            base.IsMultiLevel = _isMultiLevel;
            base.ColumnCount = _columnCount;
        }

        [OnSerializing]
        public void Process(System.Runtime.Serialization.StreamingContext context)
        {
            if (string.IsNullOrEmpty(base.Html))
            {
                string tempstr = string.Empty;
                string _singleLevelTempStr = string.Empty;

                if (IsMultiLevel)
                {
                    for (int itr = 0; itr < CurrentLevel; itr++)
                        tempstr += "<td> &nbsp;</td>";
                    string cleanedHeaderText = (CurrentLevel == 1) ? GroupingTexts[CurrentLevel - 1].Substring(2, GroupingTexts[CurrentLevel - 1].Length - 2) : GroupingTexts[CurrentLevel - 1];
                    string GroupingColumnName = RowGroupingColumns[CurrentLevel - 1].sTitle;
                    base.Html = string.Format("<tr class='group' group='{0}'>{1}<td><i class='fa fa-minus-square-o' style='cursor:pointer;'></i></td><td colspan='{2}'>{3}: <b>{4}</b> ({5})</td></tr>",
                    base.CurrentLevel, tempstr, base.ColumnCount.ToString(), GroupingColumnName, cleanedHeaderText, (CurrentLevel == TotalLevels) ? base.GroupingCount.ToString() : base.LevelCount.ToString());
                }
                else
                {
                    tempstr += "<td> &nbsp;</td>";
                    int count = 0;
                    string GroupingColumnName = string.Empty;
                    foreach (string groupString in GroupingTexts)
                    {
                        _singleLevelTempStr += RowGroupingColumns[count].sTitle + ": ";
                        _singleLevelTempStr += (count == 0) ? "<b>" + groupString.Substring(2, groupString.Length - 2) + "</b>" : "<b>" + groupString + "</b>";
                        if (groupString.Equals(GroupingTexts.Last()) == false)
                            _singleLevelTempStr += ", ";

                        count++;
                    }
                    base.Html = string.Format("<tr class='group' group='{0}'>{1}<td><i class='fa fa-minus-square-o' style='cursor:pointer;'></i></td><td colspan='{2}'>{3} ({4})</td></tr>",
                         1, tempstr, base.ColumnCount.ToString(), _singleLevelTempStr, base.GroupingCount + 1);
                }
            }
        }
    }

    public class FooterGroupingDetails : GroupingDetails
    {
        private const string Delimiter = ":-:";
        private FooterGroupingDetails _parentFooter = null;

        [JsonIgnore]
        public FooterGroupingDetails ParentFooter
        {
            get
            {
                if (_parentFooter == null && IsMultiLevel)
                {
                    var index = CollectionKey.LastIndexOf(Delimiter);
                    if (index > 0)
                        _parentFooter = RowGrouping[CollectionKey.Substring(0, index)] as FooterGroupingDetails;
                }

                return _parentFooter;
            }
        }

        [JsonIgnore]
        public Dictionary<int, NumericAggregates> Aggregations { get; set; }

        [JsonIgnore]
        public int TotalLevels { get; set; }

        [JsonIgnore]
        public DVColumnCollection TableColumns { get; set; }

        [JsonIgnore]
        public CultureInfo CultureDetails { get; set; }

        public void SetRowIndex(int index)
        {
            this.RowIndex = index;
            if (ParentFooter != null)
                ParentFooter.SetRowIndex(index);
        }

        public void SetSortIndex(int index)
        {
            this.SortIndex = index;
            if (this.ParentFooter != null)
                this.ParentFooter.SetSortIndex(index + 1);
        }

        public FooterGroupingDetails() { }

        public FooterGroupingDetails(int _totalLevels, List<int> aggregateColumnIndexes, DVColumnCollection _columns, CultureInfo _culture)
        {
            TotalLevels = _totalLevels;
            Aggregations = new Dictionary<int, NumericAggregates>();
            TableColumns = _columns;
            CultureDetails = _culture;

            foreach (int index in aggregateColumnIndexes)
            {
                Aggregations.Add(index, new NumericAggregates());
            }
        }

        [OnSerializing]
        public void Process(System.Runtime.Serialization.StreamingContext context)
        {
            if (string.IsNullOrEmpty(base.Html))
            {
                string _tempFooterPadding = string.Empty;

                int LevelSize = 0;
                if (TotalLevels == 1)
                    LevelSize = CollectionKey.Split(":-:").Length;
                else
                    LevelSize = TotalLevels;

                for (int i = 0; i < LevelSize; i++)
                    _tempFooterPadding += "<td>&nbsp;</td>";
                if (IsMultiLevel)
                    _tempFooterPadding += "<td>&nbsp;</td>";
                _tempFooterPadding += "<td>&nbsp;</td>";//serial column

                string _tempFooterText = string.Empty;
                foreach (DVBaseColumn Column in TableColumns)
                {
                    var ColumnCulture = Column.GetColumnCultureInfo(CultureDetails);
                    if (Column.bVisible)
                    {
                        if ((Column is DVNumericColumn) && (Column as DVNumericColumn).Aggregate)
                        {
                            string _style = string.Empty;
                            if ((Column as DVNumericColumn).Align == Align.Left)
                                _style = "text-align:left;";
                            else if ((Column as DVNumericColumn).Align == Align.Right || (Column as DVNumericColumn).Align == Align.Auto)
                                _style = "text-align:right;";
                            else 
                                _style = "text-align:center;";
                            _tempFooterText += "<td style="+ _style + "><b>" + (this.Aggregations[Column.Data].Sum).ToString("N", ColumnCulture.NumberFormat)
                                + "</b></td>";
                        }
                        else
                            _tempFooterText += "<td>&nbsp;</td>";
                    }
                }

                base.Html = string.Format("<tr class='group-sum' group='{0}'>{1}{2}</tr>", (this.IsMultiLevel) ? base.CurrentLevel.ToString() : 1.ToString(), _tempFooterPadding,
                    _tempFooterText);
            }
        }

        public void SetValue(EbDataRow currentRow)
        {
            foreach (var _key in this.Aggregations.Keys)
                this.Aggregations[_key].SetValue(Convert.ToDecimal(currentRow[_key]));

            if (ParentFooter != null)
                ParentFooter.SetValue(currentRow);
        }
    }

    public class NumericAggregates
    {
        public decimal Minimum { get; private set; }
        public decimal Maximum { get; private set; }
        public decimal Average
        {
            get
            {
                return (Count > 0) ? (Sum / Count) : 0;
            }
        }
        public decimal Sum { get; private set; }
        public int Count { get; private set; }

        public void SetValue(decimal _value)
        {
            Count++;
            Sum += _value;
            Minimum = (_value < Minimum) ? _value : Minimum;
            Maximum = (_value > Maximum) ? _value : Maximum;
        }
    }

    public class SqlFunParamWrapper
    {
        public string FunctionName { set; get; }

        public List<Param> Arguments { set; get; }
    }

    [DataContract]
    public class SqlFuncTestRequest : IReturn<string>, IEbSSRequest
    {
        [DataMember(Order = 1)]
        public string FunctionName { get; set; }

        [DataMember(Order = 2)]
        public List<Param> Parameters { get; set; }

        [DataMember(Order = 3)]
        public string SolnId { get; set; }

        [DataMember(Order = 4)]
        public int UserId { get; set; }
    }

    [DataContract]
    public class SqlFuncTestResponse
    {
        [DataMember(Order = 1)]
        public bool Reponse { set; get; }

        [DataMember(Order = 2)]
        public EbDataTable Data { set; get; }
    }

    [DataContract]
    public class DatawriterRequest : IReturn<string>, IEbSSRequest
    {
        [DataMember(Order = 1)]
        public string Sql { get; set; }

        [DataMember(Order = 2)]
        public List<Param> Parameters { get; set; }

        [DataMember(Order = 3)]
        public string SolnId { get; set; }

        [DataMember(Order = 4)]
        public int UserId { get; set; }
    }

    [DataContract]
    public class DatawriterResponse : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public ResponseStatus ResponseStatus { get; set; }

        [DataMember(Order = 2)]
        public string Reponse { set; get; }

        [DataMember(Order = 3)]
        public EbDataTable Data { set; get; }

        public DatawriterResponse()
        {
            ResponseStatus = new ResponseStatus();
        }
    }

    public class SqlFuncDataTable
    {
        public RowColletion Rows { set; get; }

        public DVColumnCollection Colums { set; get; }
    }

    public class DataWriterDataTable
    {
        public RowColletion Rows { set; get; }

        public DVColumnCollection Colums { set; get; }
    }

    [DataContract]
    public class DataSourceDataSetColumnsRequest : IReturn<DataSourceColumnsResponse>, IEbSSRequest
    {
        [DataMember(Order = 1)]
        public string RefId { get; set; }

        [DataMember(Order = 7)]
        public string SolnId { get; set; }

        [DataMember(Order = 8)]
        public int UserId { get; set; }

        [DataMember(Order = 9)]
        public List<Param> Params { get; set; }
    }

    [DataContract]
    [Csv(CsvBehavior.FirstEnumerable)]
    public class DataSourceDataSetColumnsResponse : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public List<ColumnColletion> ColumnsCollection { get; set; }

        [DataMember(Order = 3)]
        public bool IsPaged { get; set; }

        [DataMember(Order = 4)]
        public ResponseStatus ResponseStatus { get; set; }

        [DataMember(Order = 5)]
        public List<Param> ParamsList { get; set; }
    }

    
    [DataContract]
    public class DataSourceDataSetDataRequest : IReturn<DataSourceDataSetDataResponse>, IEbSSRequest
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
        public OrderBy OrderBy { get; set; }

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

        [DataMember(Order = 3)]
        public int QueryIndex { get; set; }

    }

    [DataContract]
    [Csv(CsvBehavior.FirstEnumerable)]
    public class DataSourceDataSetDataResponse : IEbSSResponse
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
        //public LevelInfoCollection Levels { get; set; }
        public List<GroupingDetails> Levels { get; set; }

        [DataMember(Order = 11)]
        public List<string> Permission { get; set; }

        [DataMember(Order = 12)]
        public Dictionary<int, List<object>> Summary { get; set; }

        [DataMember(Order = 13)]
        public byte[] excel_file { get; set; }

    }
}
