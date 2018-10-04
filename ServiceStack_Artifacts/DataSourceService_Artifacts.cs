using ExpressBase.Common;
using ExpressBase.Common.Data;
using ExpressBase.Common.EbServiceStack.ReqNRes;
using ExpressBase.Common.Singletons;
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

    public abstract class GroupingDetails
    {
        [JsonIgnore]
        public bool IsMultiLevel { get; set; }

        [JsonIgnore]
        public int ColumnCount { get; set; }

        /// <summary>
        /// The level of row grouping for an element. Used in multi-level row grouping.
        /// FOr single level, it is always 1.
        /// </summary>
        [JsonIgnore]
        public int CurrentLevel { get; set; }

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
        [JsonIgnore]
        public int ChildCount { get; set; }

        [JsonIgnore]
        public int GroupingCount { get; set; }

        [JsonIgnore]
        public int LevelCount { get; set; }
        /// <summary>
        /// The HTML text generated for the particular header or footer
        /// </summary>
        public virtual string Html { get; set; }
    }

    public class HeaderGroupingDetails : GroupingDetails
    {
        /// <summary>
        /// The list of strings that denotes each row grouping string.
        /// For multiple level row grouping, it denotes each different level too.
        /// For single level, each column value is denoted separately for ease of processing.
        /// </summary> 
        [JsonIgnore]
        public List<string> GroupingTexts { get; set; }

        [JsonIgnore]
        public int TotalLevels { get; set; }

        public HeaderGroupingDetails() { }

        public HeaderGroupingDetails(bool _isMultiLevel, int _columnCount)
        {
            base.IsMultiLevel = _isMultiLevel;
            base.ColumnCount = _columnCount;
            GroupingTexts = new List<string>();
        }

        [OnSerializing]
        public void Process(System.Runtime.Serialization.StreamingContext context)
        {
            if (string.IsNullOrEmpty(base.Html))
            {
                string tempstr = string.Empty;
                string _singleLevelTempStr = string.Empty;
                for (int itr = 0; itr < base.CurrentLevel + 1; itr++)
                    tempstr += "<td> &nbsp;</td>";

                if (IsMultiLevel)
                    base.Html = string.Format("<tr class='group' group='{0}'>{1}<td><i class='fa fa-minus-square-o' style='cursor:pointer;'></i></td><td colspan='{2}'>{3} : {4}</td></tr>",
                    (base.CurrentLevel + 1).ToString(), tempstr, base.ColumnCount.ToString(), 
                    GroupingTexts[CurrentLevel - 1], (CurrentLevel==TotalLevels)?base.GroupingCount.ToString():base.LevelCount.ToString());
                else
                {
                    foreach (string groupString in GroupingTexts)
                    {
                        _singleLevelTempStr += groupString;
                        if (groupString.Equals(GroupingTexts.Last()) == false)
                            _singleLevelTempStr += " - ";
                    }
                    base.Html = string.Format("<tr class='group' group='{0}'>{1}<td><i class='fa fa-minus-square-o' style='cursor:pointer;'></i></td><td colspan='{2}'>{3} : {4}</td></tr>",
                        (base.CurrentLevel + 1).ToString(), tempstr, base.ColumnCount.ToString(), _singleLevelTempStr, base.GroupingCount.ToString());
                }
            }
        }
    }

    public class FooterGroupingDetails : GroupingDetails
    {
        [JsonIgnore]
        public Dictionary<int, NumericAggregates> Aggregations { get; set; }

        [JsonIgnore]
        public List<string> GroupingTexts { get; set; }

        [JsonIgnore]
        public int TotalLevels { get; set; }

        [JsonIgnore]
        public EbDataVisualization Visualization { get; set; }

        [JsonIgnore]
        public CultureInfo CultureDetails { get; set; }

        [JsonIgnore]
        public FooterGroupingDetails ParentLevelFooter { get; set; }

        public FooterGroupingDetails() { }

        public FooterGroupingDetails(int _totalLevels, List<int> aggregateColumnIndexes, EbDataVisualization _visualization, CultureInfo _culture)
        {
            TotalLevels = _totalLevels;
            Aggregations = new Dictionary<int, NumericAggregates>();
            Visualization = _visualization;
            CultureDetails = _culture;
            GroupingTexts = new List<string>();
            ParentLevelFooter = new FooterGroupingDetails();

            foreach(int index in aggregateColumnIndexes)
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
                for (int i = 0; i < TotalLevels; i++)
                    _tempFooterPadding += "<td>&nbsp;</td>";

                if (TotalLevels > 1)
                    _tempFooterPadding += "<td>&nbsp;</td>";

                _tempFooterPadding += "<td>&nbsp;</td>";//serial column

                string _tempFooterText = string.Empty;
                foreach (DVBaseColumn col in (Visualization as EbTableVisualization).Columns)
                {
                    var ColumnCulture = col.GetColumnCultureInfo(CultureDetails);
                    if (col.bVisible)
                    {
                        if ((col is DVNumericColumn) && (col as DVNumericColumn).Aggregate)
                            _tempFooterText += "<td class='dt-body-right'>" + (this.Aggregations[col.Data].Sum).ToString("N", ColumnCulture.NumberFormat) 
                                + "</td>";
                        else
                            _tempFooterText += "<td>&nbsp;</td>";
                    }
                }

                base.Html = string.Format("<tr class='group-sum' group='{0}'>{1}{2}</tr>", (base.CurrentLevel + 1).ToString(), _tempFooterPadding,
                    _tempFooterText);
            }
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
                return (Count >0) ? (Sum / Count) : 0;
            }
        }
        public decimal Sum     { get; private set; }
        public int Count      { get; private set; }

        public void SetValue(decimal _value)
        {
            Count++;
            Sum += _value;
            Minimum = (_value < Minimum) ? _value : Minimum;
            Maximum = (_value > Maximum) ? _value : Maximum;
        }
    }
}
