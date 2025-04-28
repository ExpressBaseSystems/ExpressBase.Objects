using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Office2010.PowerPoint;
using ExpressBase.Common;
using ExpressBase.Common.Constants;
using ExpressBase.Common.Data;
using ExpressBase.Common.EbServiceStack.ReqNRes;
using ExpressBase.Common.Extensions;
using ExpressBase.Common.LocationNSolution;
using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using ExpressBase.Common.ServiceClients;
using ExpressBase.Common.Singletons;
using ExpressBase.Common.Structures;
using ExpressBase.CoreBase.Globals;
using ExpressBase.Objects.Helpers;
using ExpressBase.Objects.Objects;
using ExpressBase.Objects.ServiceStack_Artifacts;
using ExpressBase.Security;
using iTextSharp.text;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using Newtonsoft.Json;
using ServiceStack;
using ServiceStack.Redis;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using static ExpressBase.CoreBase.Globals.PdfGEbFont;
using HeaderFooter = ExpressBase.Objects.Helpers.HeaderFooter;
using Paragraph = iTextSharp.text.Paragraph;
using Rectangle = iTextSharp.text.Rectangle;

namespace ExpressBase.Objects
{
    public enum EbReportSectionType
    {
        ReportHeader,
        PageHeader,
        Detail,
        PageFooter,
        ReportFooter,
        ReportGroups
    }
    public enum PaperSize
    {
        A2,
        A3,
        A4,
        A5,
        Letter,
        Custom
    }
    public enum SummaryFunctionsText
    {
        Count,
        Max,
        Min
    }
    public enum EbTextAlign
    {
        Left = 0,
        Center = 1,
        Right = 2,
        Justify = 3,
        Top = 4,
        Middle = 5,
        Bottom = 6,
        Baseline = 7,
        JustifiedAll = 8,
        Undefined = -1
    }

    public enum EvaluatorVersion
    {
        Version_1 = 0,
        Version_2 = 1
    }

    public enum DateFormatReport
    {
        M_d_yyyy,
        MM_dd_yyyy,
        ddd_MMM_d_yyyy,
        dddd_MMMM_d_yyyy,
        MM_dd_yy,
        dd_MM_yyyy,
        dd_MM_yyyy_slashed,
        from_culture,
        dd_MMMM_yyyy
        //Year_Month_Date,
        //Year_Month,
        //Year,
    }

    public enum SummaryFunctionsNumeric
    {
        Average,
        Count,
        Max,
        Min,
        Sum
    }
    public enum SummaryFunctionsDateTime
    {
        Count,
        Max,
        Min
    }
    public enum SummaryFunctionsBoolean
    {
        Count
    }

    [EnableInBuilder(BuilderType.Report)]
    public class Margin
    {
        [EnableInBuilder(BuilderType.Report)]
        public float Left { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        public float Right { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        public float Top { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        public float Bottom { get; set; }
    }

    [EnableInBuilder(BuilderType.Report)]
    [BuilderTypeEnum(BuilderType.Report)]
    public class EbReport : EbReportObject, IEBRootObject
    {
        [EnableInBuilder(BuilderType.Report)]
        [HideInPropertyGrid]
        public override string RefId { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        [PropertyGroup(PGConstants.GENERAL)]
        public override string DisplayName { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        [PropertyGroup(PGConstants.GENERAL)]
        public override string Name { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        [PropertyGroup(PGConstants.GENERAL)]
        public override string Description { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        [HideInPropertyGrid]
        public override string VersionNumber { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        public bool HideInMenu { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        [HideInPropertyGrid]
        public override string Status { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        [PropertyEditor(PropertyEditorType.ScriptEditorSQ)]
        [HelpText("sql query to get data from offline database")]
        [Alias("Offline Query")]
        [PropertyGroup(PGConstants.DATA)]
        public EbScript OfflineQuery { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        [OnChangeExec(@"
                if (this.PaperSize === 5 ){ 
                        pg.ShowProperty('CustomPaperHeight');
                        pg.ShowProperty('CustomPaperWidth');
                }
                else {
                        pg.HideProperty('CustomPaperHeight');
                        pg.HideProperty('CustomPaperWidth');
                }
            ")]
        [PropertyGroup("Dimensions")]
        public PaperSize PaperSize { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        [PropertyGroup("Dimensions")]
        [UIproperty]
        public float CustomPaperHeight { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        [PropertyGroup("Dimensions")]
        [UIproperty]
        public float CustomPaperWidth { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        [PropertyEditor(PropertyEditorType.Expandable)]
        [PropertyGroup("Dimensions")]
        public Margin Margin { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        [HideInPropertyGrid]
        public float DesignPageHeight { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        [PropertyGroup(PGConstants.GENERAL)]
        [UIproperty]
        public string UserPassword { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        [PropertyGroup(PGConstants.GENERAL)]
        [UIproperty]
        public string OwnerPassword { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        [PropertyGroup(PGConstants.GENERAL)]
        public EvaluatorVersion EvaluatorVersion { get; set; }

        private string _docName = null;
        public string DocumentName
        {
            get
            {
                if (!string.IsNullOrEmpty(DocumentNameString) && _docName == null)
                {
                    _docName = DocumentNameString;
                    string pattern = @"\{{(.*?)\}}";
                    IEnumerable<string> matches = Regex.Matches(DocumentNameString, pattern).OfType<Match>().Select(m => m.Groups[0].Value).Distinct();
                    foreach (string _col in matches)
                    {
                        string str = _col.Replace("{{", "").Replace("}}", "");
                        int tbl = Convert.ToInt32(str.Split('.')[0].Replace("T", ""));
                        string colval = string.Empty;
                        if (this.DataSet?.Tables[tbl]?.Rows.Count > 0)
                            colval = DataSet?.Tables[tbl]?.Rows[0][str.Split('.')[1]].ToString();
                        _docName = _docName.Replace(_col, colval);
                    }
                }
                else if (_docName == null)
                {
                    _docName = DisplayName;
                }
                return _docName;
            }
        }

        [EnableInBuilder(BuilderType.Report)]
        [PropertyGroup(PGConstants.GENERAL)]
        [UIproperty]
        [Alias("Document Name")]
        public string DocumentNameString { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        [HideInPropertyGrid]
        public override string Width { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        [HideInPropertyGrid]
        public override string Height { get; set; }

        public override string Left { get; set; }

        public override string Top { get; set; }

        public override string Title { get; set; }

        public override float LeftPt { get; set; }

        public override float TopPt { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        [PropertyGroup(PGConstants.APPEARANCE)]
        public bool IsLandscape { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        [PropertyGroup(PGConstants.APPEARANCE)]
        public bool RenderReportFooterInBottom { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        [PropertyEditor(PropertyEditorType.ImageSeletor)]
        [PropertyGroup(PGConstants.APPEARANCE)]
        public string BackgroundImage { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        [PropertyGroup(PGConstants.APPEARANCE)]
        public bool IsLongDetailSection { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        [HideInPropertyGrid]
        public List<EbReportField> ReportObjects { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        [HideInPropertyGrid]
        public List<EbReportHeader> ReportHeaders { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        [HideInPropertyGrid]
        public List<EbReportFooter> ReportFooters { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        [HideInPropertyGrid]
        public List<EbPageHeader> PageHeaders { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        [HideInPropertyGrid]
        public List<EbPageFooter> PageFooters { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        [HideInPropertyGrid]
        public List<EbReportDetail> Detail { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        [HideInPropertyGrid]
        public List<EbReportGroup> ReportGroups { set; get; }

        [JsonIgnore]
        public EbDataReader EbDataSource { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        [PropertyEditor(PropertyEditorType.ObjectSelector)]
        [OSE_ObjectTypes(EbObjectTypes.iDataReader)]
        [PropertyGroup(PGConstants.DATA)]
        public string DataSourceRefId { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        [PropertyGroup(PGConstants.APPEARANCE)]
        [UIproperty]
        [PropertyEditor(PropertyEditorType.FontSelector)]
        public EbFont Font { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        public bool HidePrintDetails { get; set; }
        //[JsonIgnore]
        //public ColumnColletion ColumnColletion { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        public bool IsLanguageEnabled { get; set; } = false;

        [EnableInBuilder(BuilderType.Report)]
        public bool RestartSerialNumberOnGroup { set; get; }

        [JsonIgnore]
        public int DetailTableIndex { get; set; } = 0;

        [JsonIgnore]
        public int MaxRowCount { get; set; } = 0;

        [JsonIgnore]
        public bool DrawDetailCompleted = false;

        [JsonIgnore]
        public bool IsInEndPageEvent = false;

        [JsonIgnore]
        public bool HasRows = false;

        [JsonIgnore]
        public int iDetailRowPos { get; set; }

        [JsonIgnore]
        public Dictionary<string, List<EbDataField>> PageSummaryFields { get; set; } = new Dictionary<string, List<EbDataField>>();

        [JsonIgnore]
        public Dictionary<string, List<EbDataField>> ReportSummaryFields { get; set; } = new Dictionary<string, List<EbDataField>>();

        [JsonIgnore]
        public Dictionary<string, List<EbDataField>> GroupSummaryFields { get; set; } = new Dictionary<string, List<EbDataField>>();

        [JsonIgnore]
        public Dictionary<int, byte[]> WatermarkImages = new Dictionary<int, byte[]>();

        [JsonIgnore]
        public List<object> WaterMarkList { get; set; } = new List<object>();

        [JsonIgnore]
        public EbSciptEvaluator evaluator = new EbSciptEvaluator
        {
            OptionScriptNeedSemicolonAtTheEndOfLastExpression = false
        };

        [JsonIgnore]
        public Dictionary<string, object> ValueScriptCollection { get; set; } = new Dictionary<string, object>();

        [JsonIgnore]
        public Dictionary<string, object> AppearanceScriptCollection { get; set; } = new Dictionary<string, object>();

        [JsonIgnore]
        public Dictionary<string, ReportGroupItem> Groupheaders { get; set; } = new Dictionary<string, ReportGroupItem>();

        [JsonIgnore]
        public Dictionary<string, ReportGroupItem> GroupFooters { get; set; } = new Dictionary<string, ReportGroupItem>();

        [JsonIgnore]
        public EbDataSet DataSet { get; set; }

        [JsonIgnore]
        public bool IsLastpage { get; set; } = false;

        [JsonIgnore]
        public int CurrentReportPageNumber { get; set; }
        [JsonIgnore]
        public bool NextReport { get; set; }
        [JsonIgnore]
        public int MasterPageNumber { get; set; }

        [JsonIgnore]
        public int SerialNumber = 0;

        private DateTime _currentTimeStamp = DateTime.MinValue;
        [JsonIgnore]
        public DateTime CurrentTimestamp
        {
            get
            {
                if (_currentTimeStamp == DateTime.MinValue)
                {
                    _currentTimeStamp = DateTime.UtcNow;
                    string timezone = ReadingUser?.Preference?.TimeZone ?? "(UTC) Coordinated Universal Time";
                    _currentTimeStamp = _currentTimeStamp.ConvertFromUtc(timezone);
                }
                return _currentTimeStamp;
            }
        }

        [JsonIgnore]
        public User ReadingUser { get; set; }

        [JsonIgnore]
        public User RenderingUser { get; set; }

        [JsonIgnore]
        public CultureInfo CultureInfo { get; set; }

        [JsonIgnore]
        public PdfContentByte Canvas { get; set; }

        [JsonIgnore]
        public PdfWriter Writer { get; set; }

        [JsonIgnore]
        public Document Doc { get; set; }

        [JsonIgnore]
        public PdfReader PdfReader { get; set; }

        [JsonIgnore]
        public PdfStamper Stamp { get; set; }


        [JsonIgnore]
        public Eb_Solution Solution { get; set; }

        private float _rhHeight = 0;

        [JsonIgnore]
        public float ReportHeaderHeight
        {
            get
            {
                if (_rhHeight == 0)
                {
                    foreach (EbReportHeader r_header in ReportHeaders)
                        _rhHeight += r_header.HeightPt;
                }

                return _rhHeight;
            }
        }

        private float _rhHeightAsPh = 0;

        [JsonIgnore]
        public float ReportHeaderHeightRepeatAsPH
        {
            get
            {
                if (_rhHeightAsPh == 0)
                {
                    foreach (EbReportHeader r_header in ReportHeaders)
                        if (r_header.RepeatOnAllPages)
                            _rhHeightAsPh += r_header.HeightPt;
                }

                return _rhHeightAsPh;
            }
        }

        private float _phHeight = 0;
        [JsonIgnore]
        public float PageHeaderHeight
        {
            get
            {
                if (_phHeight == 0)
                {
                    foreach (EbPageHeader p_header in PageHeaders)
                        _phHeight += p_header.HeightPt;
                }
                return _phHeight;
            }
        }

        private float _pfHeight = 0;
        [JsonIgnore]
        public float PageFooterHeight
        {
            get
            {
                if (_pfHeight == 0)
                {

                    foreach (EbPageFooter p_footer in PageFooters)
                        _pfHeight += p_footer.HeightPt;
                }
                return _pfHeight;
            }
        }

        private float _rfHeight = 0;
        [JsonIgnore]
        public float ReportFooterHeight
        {
            get
            {
                if (_rfHeight == 0)
                {
                    foreach (EbReportFooter r_footer in ReportFooters)
                        _rfHeight += r_footer.HeightPt;
                }
                return _rfHeight;
            }
        }

        private float _rfHeightAsPf = 0;
        [JsonIgnore]
        public float ReportFooterHeightRepeatAsPf
        {
            get
            {
                if (_rfHeightAsPf == 0)
                {
                    foreach (EbReportFooter r_footer in ReportFooters)
                        if (r_footer.RepeatOnAllPages)
                            _rfHeightAsPf += r_footer.HeightPt;
                }
                return _rfHeightAsPf;
            }
        }


        private float _dtHeight = 0;
        [JsonIgnore]
        public float DetailsHeight
        {
            get
            {
                if (_dtHeight == 0)
                {
                    foreach (EbReportDetail detail in Detail)
                        _dtHeight += detail.HeightPt;
                }
                return _dtHeight + RowHeight;
            }
        }

        private float _ghHeight = 0;
        public float GroupHeaderHeight
        {
            get
            {
                if (_ghHeight == 0)
                {
                    foreach (EbReportGroup grp in ReportGroups)
                    {
                        _ghHeight += grp.GroupHeader.HeightPt;
                    }
                }
                return _ghHeight;
            }
        }

        private float _gfHeight = 0;
        public float GroupFooterHeight
        {
            get
            {
                if (_gfHeight == 0)
                {
                    foreach (EbReportGroup grp in ReportGroups)
                    {
                        _gfHeight += grp.GroupFooter.HeightPt;
                    }
                }
                return _gfHeight;
            }
        }

        private float possibleSpaceForDetail = 0;
        [JsonIgnore]
        public float PossibleSpaceForDetail
        {
            get
            {
                float headerHeight = CurrentReportPageNumber == 1 ? ReportHeaderHeight : ReportHeaderHeightRepeatAsPH;
                float footerHeight =/* IsLastpage ? ReportFooterHeight :*/ ReportFooterHeightRepeatAsPf;
                possibleSpaceForDetail = HeightPt - (headerHeight + PageHeaderHeight + PageFooterHeight + footerHeight + Margin.Top + Margin.Bottom);

                return possibleSpaceForDetail;
            }
        }

        [JsonIgnore]
        public EbStaticFileClient FileClient { get; set; }

        [JsonIgnore]
        public float RowHeight { get; set; }

        [JsonIgnore]
        public float MultiRowTop { get; set; }

        [JsonIgnore]
        public List<Param> Parameters { get; set; }

        private float rh_Yposition;
        private float rf_Yposition;
        private float pf_Yposition;
        private float ph_Yposition;
        private float dt_Yposition;

        [JsonIgnore]
        public float detailCursorPosition = 0;

        [JsonIgnore]
        public float detailEnd = 0;

        [JsonIgnore]
        public bool FooterDrawn = false;

        [JsonIgnore]
        private int PreviousGheadersIterator { get; set; }

        [JsonIgnore]
        public Dictionary<string, List<EbControl>> LinkCollection { get; set; } = new Dictionary<string, List<Common.Objects.EbControl>>();

        [JsonIgnore]
        public Dictionary<string, GNTV> CalcValInRow { get; set; } = new Dictionary<string, GNTV>();

        [JsonIgnore]
        public Dictionary<string, GNTV> SummaryValInRow { get; set; } = new Dictionary<string, GNTV>();

        [JsonIgnore]
        public List<string> LabelsCollection { get; set; } = new List<string>();

        [JsonIgnore]
        public Dictionary<string, string> LabelKeyValues { get; set; } = new Dictionary<string, string>();

        [JsonIgnore]
        public IDatabase ObjectsDB { get; set; }

        [JsonIgnore]
        public IRedisClient Redis { get; set; }

        [JsonIgnore]
        public PooledRedisClientManager pooledRedisManager { get; set; }

        public dynamic GetDataFieldValue(string columnName, int iterator, int tableIndex)
        {
            if (DataSet == null)
                throw new ArgumentNullException(nameof(DataSet), "DataSet cannot be null.");

            if (tableIndex < 0 || tableIndex >= DataSet.Tables.Count)
                throw new ArgumentOutOfRangeException(nameof(tableIndex), $"Table index {tableIndex} is out of range. Total tables: {DataSet.Tables.Count}");

            var table = DataSet.Tables[tableIndex];

            if (!table.Columns.Contains(columnName))
                throw new ArgumentException($"Column '{columnName}' does not exist in the table at index {tableIndex}.");

            if (table.Rows.Count == 0)
                throw new InvalidOperationException($"The table at index {tableIndex} contains no rows.");

            int rowIndex = (table.Rows.Count > 1) ? iterator : 0;

            if (rowIndex < 0 || rowIndex >= table.Rows.Count)
                throw new ArgumentOutOfRangeException(nameof(iterator), $"Row index {rowIndex} is out of range. Total rows: {table.Rows.Count}");

            dynamic value = (table.Columns[columnName].Type == EbDbTypes.Bytea) ? table.Rows[rowIndex][columnName] : table.Rows[rowIndex][columnName]?.ToString();

            return value;
        }

        public void DrawWaterMark(Document d, PdfWriter writer)
        {
            if (ReportObjects != null)
            {
                foreach (EbReportField field in ReportObjects)
                {
                    DrawFields(field, 0, 0);
                }
            }
        }

        public void CallSummerize(EbDataField field, int iterator)
        {
            List<EbDataField> SummaryList;
            string column_val = field is EbCalcField calcField
                                ? calcField.GetCalcFieldValue(new EbPdfGlobals(), DataSet, iterator, this)
                                : GetDataFieldValue(field.ColumnName, iterator, field.TableIndex);

            if (GroupSummaryFields.ContainsKey(field.Name))
            {
                SummaryList = GroupSummaryFields[field.Name];
                foreach (EbDataField item in SummaryList)
                {
                    (item as IEbDataFieldSummary).Summarize(column_val);
                }
            }
            if (PageSummaryFields.ContainsKey(field.Name))
            {
                SummaryList = PageSummaryFields[field.Name];
                foreach (EbDataField item in SummaryList)
                {
                    (item as IEbDataFieldSummary).Summarize(column_val);
                }
            }
            if (ReportSummaryFields.ContainsKey(field.Name))
            {
                SummaryList = ReportSummaryFields[field.Name];
                foreach (EbDataField item in SummaryList)
                {
                    (item as IEbDataFieldSummary).Summarize(column_val);
                }
            }

        }

        public void DrawReportHeader()
        {
            RowHeight = 0;
            MultiRowTop = 0;
            rh_Yposition = this.Margin.Top;
            detailCursorPosition = 0;
            foreach (EbReportHeader r_header in ReportHeaders)
            {
                if ((CurrentReportPageNumber == 1 || r_header.RepeatOnAllPages))
                {
                    foreach (EbReportField field in r_header.GetFields())
                    {
                        DrawFields(field, rh_Yposition, 0);
                        Console.WriteLine(rh_Yposition);
                    }
                    rh_Yposition += r_header.HeightPt;
                }
            }
        }

        public void DrawPageHeader()
        {
            RowHeight = 0;
            MultiRowTop = 0;
            detailCursorPosition = 0;
            ph_Yposition = Margin.Top + (CurrentReportPageNumber == 1 ? ReportHeaderHeight : ReportHeaderHeightRepeatAsPH);

            foreach (EbPageHeader p_header in PageHeaders)
            {
                foreach (EbReportField field in p_header.GetFields())
                {
                    DrawFields(field, ph_Yposition, 0);
                }
                ph_Yposition += p_header.HeightPt;
            }
        }

        public void DrawDetail()
        {
            RowColletion rows = (DataSourceRefId != string.Empty) ? DataSet.Tables[DetailTableIndex].Rows : null;

            ph_Yposition = this.Margin.Top + (CurrentReportPageNumber == 1 ? ReportHeaderHeight : ReportHeaderHeightRepeatAsPH);

            dt_Yposition = ph_Yposition + PageHeaderHeight;
            if (HasRows)
            {
                for (iDetailRowPos = 0; iDetailRowPos < rows.Count; iDetailRowPos++)
                {
                    SerialNumber++;
                    if (Groupheaders?.Count > 0)
                        DrawGroup();

                    if (PossibleSpaceForDetail - detailCursorPosition < DetailsHeight)
                        AddNewPage();

                    DoLoopInDetail(iDetailRowPos);
                }
                if (GroupFooters?.Count > 0)
                    EndGroups();
                IsLastpage = true;
            }
            else
            {
                IsLastpage = true;
                DoLoopInDetail(0);
            }
            DrawDetailCompleted = true;
            if (ReportFooterHeightRepeatAsPf > 0)
            {
                float remainingSpaceAfterDetailsDrawn = PossibleSpaceForDetail - detailCursorPosition;
                float spaceNeededForNonRepeatingFooters = ReportFooterHeight - ReportFooterHeightRepeatAsPf;
                if (remainingSpaceAfterDetailsDrawn < spaceNeededForNonRepeatingFooters)
                {
                    IsLastpage = false;
                    detailEnd = ReportHeaderHeightRepeatAsPH;//no [age header as it is lastpg
                    detailCursorPosition = 0;
                    AddNewPage();
                    IsLastpage = true;
                }
            }
        }

        public void DrawGroup()
        {
            foreach (KeyValuePair<string, ReportGroupItem> grp in Groupheaders)
            {
                ReportGroupItem grpitem = grp.Value;
                string column_val = GetDataFieldValue(grpitem.field.ColumnName, iDetailRowPos, grpitem.field.TableIndex);

                if (grpitem.PreviousValue != column_val)
                {
                    if (iDetailRowPos > 0)
                        DrawGroupFooter(grpitem.order, iDetailRowPos);
                    DrawGroupHeader(grpitem.order, iDetailRowPos);
                    grpitem.PreviousValue = column_val;
                    int i;
                    for (i = grpitem.order + 1; i < Groupheaders.Count; i++)
                        Groupheaders.Values.ElementAt(i).PreviousValue = string.Empty;
                }
            }
        }

        public void EndGroups()
        {
            foreach (EbReportGroup grp in this.ReportGroups)
            {
                DrawGroupFooter(grp.GroupFooter.Order, iDetailRowPos);
                detailEnd += ReportGroups[grp.GroupFooter.Order].GroupFooter.HeightPt;
            }
        }

        private Dictionary<EbReportDetail, EbDataField[]> __fieldsNotSummaryPerDetail = null;
        private Dictionary<EbReportDetail, EbDataField[]> FieldsNotSummaryPerDetail
        {
            get
            {
                if (__fieldsNotSummaryPerDetail == null)
                {
                    __fieldsNotSummaryPerDetail = new Dictionary<EbReportDetail, EbDataField[]>();
                    foreach (EbReportDetail detail in Detail)
                        __fieldsNotSummaryPerDetail[detail] = detail.GetFields().Where(x => (x is EbDataField && !(x is IEbDataFieldSummary))).OrderBy(o => o.Top).Cast<EbDataField>().ToArray();
                }

                return __fieldsNotSummaryPerDetail;
            }
        }

        private Dictionary<EbReportDetail, EbReportField[]> __reportFieldsSortedPerDetail = null;
        private Dictionary<EbReportDetail, EbReportField[]> ReportFieldsSortedPerDetail
        {
            get
            {
                if (__reportFieldsSortedPerDetail == null)
                {
                    __reportFieldsSortedPerDetail = new Dictionary<EbReportDetail, EbReportField[]>();
                    foreach (EbReportDetail detail in Detail)
                        if (this.IsLongDetailSection)
                            __reportFieldsSortedPerDetail[detail] = detail.GetFields().OrderBy(o => o.TopPt).ToArray();
                        else
                            __reportFieldsSortedPerDetail[detail] = detail.GetFields().OrderBy(o => o.Left).ToArray();
                }
                return __reportFieldsSortedPerDetail;
            }
        }

        private Dictionary<EbReportFooter, EbReportField[]> __reportFieldsSortedPerRFooter = null;
        private Dictionary<EbReportFooter, EbReportField[]> ReportFieldsSortedPerRFooter
        {
            get
            {
                if (__reportFieldsSortedPerRFooter == null)
                {
                    __reportFieldsSortedPerRFooter = new Dictionary<EbReportFooter, EbReportField[]>();
                    foreach (EbReportFooter _footer in ReportFooters)
                    {
                        __reportFieldsSortedPerRFooter[_footer] = _footer.GetFields().OrderBy(o => o.TopPt).ToArray();
                    }
                }

                return __reportFieldsSortedPerRFooter;
            }
        }

        public void AddNewPage()
        {
            detailCursorPosition = 0;
            Doc.NewPage();
            ph_Yposition = this.Margin.Top;
        }

        public void DoLoopInDetail(int iterator)
        {
            ph_Yposition = Margin.Top + (CurrentReportPageNumber == 1 ? ReportHeaderHeight : ReportHeaderHeightRepeatAsPH);
            dt_Yposition = ph_Yposition + PageHeaderHeight;
            string column_val = string.Empty;

            foreach (EbReportDetail detail in Detail)
            {
                float nextpage_quotient = 0;
                RowHeight = 0;
                MultiRowTop = 0;
                EbDataField[] SortedList = FieldsNotSummaryPerDetail[detail];
                EbPdfGlobals globals = new EbPdfGlobals();
                if (!IsLongDetailSection)
                {
                    for (int iSortPos = 0; iSortPos < SortedList.Length; iSortPos++)
                    {
                        EbDataField field = SortedList[iSortPos];
                        if (field is EbCalcField)
                        {
                            globals.CurrentField = field;
                            column_val = (field as EbCalcField).GetCalcFieldValue(globals, DataSet, iterator, this);
                            EbDbTypes dbtype = (EbDbTypes)((field as EbCalcField).CalcFieldIntType);

                            if (CalcValInRow.ContainsKey(field.Title))
                                CalcValInRow[field.Title] = new GNTV { Name = field.Title, Type = (GlobalDbType)(int)dbtype, Value = column_val };
                            else
                                CalcValInRow.Add(field.Title, new GNTV { Name = field.Title, Type = (GlobalDbType)(int)dbtype, Value = column_val });
                        }
                        else
                        {
                            column_val = GetDataFieldValue(field.ColumnName, iterator, field.TableIndex);
                        }

                        if (field.RenderInMultiLine)
                        {
                            float ury = HeightPt - (dt_Yposition + field.TopPt + detailCursorPosition);
                            float lly = HeightPt - (dt_Yposition + field.TopPt + detailCursorPosition + field.HeightPt);
                            field.DoRenderInMultiLine2(column_val, this, false, lly, ury);
                        }
                    }
                    EbReportField[] SortedReportFields = this.ReportFieldsSortedPerDetail[detail];
                    if (SortedReportFields.Length > 0)
                    {
                        for (int iSortPos = 0; iSortPos < SortedReportFields.Length; iSortPos++)
                        {
                            EbReportField field = SortedReportFields[iSortPos];
                            //if (field is EbDataField)
                            //    field.HeightPt += RowHeight;
                            DrawFields(field, dt_Yposition, iterator);
                        }
                        detailCursorPosition += detail.HeightPt + RowHeight;
                        detailEnd = detailCursorPosition;
                    }
                    else
                    {
                        detailEnd = detailCursorPosition;
                        IsLastpage = true;
                        //Writer.PageEvent.OnEndPage(Writer, Doc);
                        return;
                    }
                }
                else
                {
                    EbReportField[] SortedReportFields = this.ReportFieldsSortedPerDetail[detail];
                    if (SortedReportFields.Length > 0)
                    {
                        for (int iSortPos = 0; iSortPos < SortedReportFields.Length; iSortPos++)
                        {
                            EbReportField field = SortedReportFields[iSortPos];
                            if (field is EbCalcField)
                            {
                                globals.CurrentField = field;
                                column_val = (field as EbCalcField).GetCalcFieldValue(globals, DataSet, iterator, this);
                                EbDbTypes dbtype = (EbDbTypes)((field as EbCalcField).CalcFieldIntType);

                                if (CalcValInRow.ContainsKey(field.Title))
                                    CalcValInRow[field.Title] = new GNTV { Name = field.Title, Type = (GlobalDbType)(int)dbtype, Value = column_val };
                                else
                                    CalcValInRow.Add(field.Title, new GNTV { Name = field.Title, Type = (GlobalDbType)(int)dbtype, Value = column_val });
                            }
                            else
                            {
                                if (field is EbDataField)
                                    column_val = GetDataFieldValue((field as EbDataField).ColumnName, iterator, (field as EbDataField).TableIndex);
                            }

                            if (field is EbDataField && (field as EbDataField).RenderInMultiLine)
                            {
                                float ury = HeightPt - (dt_Yposition + field.TopPt + detailCursorPosition);
                                float lly = HeightPt - (dt_Yposition + field.TopPt + detailCursorPosition + field.HeightPt);
                                (field as EbDataField).DoRenderInMultiLine2(column_val, this, false, lly, ury);
                            }
                            //if (DT_FillHeight < field.TopPt - nextpage_quotient + field.HeightPt + RowHeight)
                            if (this.PossibleSpaceForDetail < detailCursorPosition + Margin.Bottom)
                            {
                                AddNewPage();
                                detailCursorPosition = 0;
                                nextpage_quotient = field.TopPt;
                            }
                            field.TopPt -= nextpage_quotient;
                            DrawFields(field, dt_Yposition, iterator);
                            if (RowHeight > 0)
                                detailCursorPosition += RowHeight;
                            // detailprintingtop += field.HeightPt;
                            detailEnd = detailCursorPosition;
                            RowHeight = 0;
                        }
                        detailCursorPosition += detail.HeightPt;
                    }
                    else
                    {
                        detailEnd = detailCursorPosition;
                        IsLastpage = true;
                        Writer.PageEvent.OnEndPage(Writer, Doc);
                        return;
                    }
                }
            }
        }

        public void DrawGroupHeader(int order, int iterator)
        {
            bool IsNewGroup = PreviousGheadersIterator != iterator;
            bool GroupInNewPageEnabled = ReportGroups[order].GroupHeader.GroupInNewPage && iterator > 0;

            if ((IsNewGroup && GroupHeaderHeight + DetailsHeight > possibleSpaceForDetail - detailCursorPosition) || GroupInNewPageEnabled)
            {
                AddNewPage();
                dt_Yposition = ReportHeaderHeightRepeatAsPH + PageHeaderHeight + this.Margin.Top;
            }

            foreach (EbReportField field in ReportGroups[order].GroupHeader.GetFields())
            {
                DrawFields(field, dt_Yposition, iterator);
            }
            detailCursorPosition += ReportGroups[order].GroupHeader.HeightPt;
            detailEnd = detailCursorPosition;

            PreviousGheadersIterator = iterator;
        }

        public void DrawGroupFooter(int order, int iterator)
        {
            foreach (EbReportField field in ReportGroups[order].GroupFooter.GetFields())
            {
                DrawFields(field, dt_Yposition, iterator);
            }
            detailCursorPosition += ReportGroups[order].GroupFooter.HeightPt;
            detailEnd = detailCursorPosition;
            if (RestartSerialNumberOnGroup)
                SerialNumber = 1;
        }

        public void DrawPageFooter()
        {
            RowHeight = 0;
            MultiRowTop = 0;
            detailCursorPosition = 0;

            ph_Yposition = Margin.Top + (CurrentReportPageNumber == 1 ? ReportHeaderHeight : ReportHeaderHeightRepeatAsPH);
            dt_Yposition = ph_Yposition + PageHeaderHeight;
            pf_Yposition = detailEnd + dt_Yposition;

            foreach (EbPageFooter p_footer in PageFooters)
            {
                foreach (EbReportField field in p_footer.GetFields())
                {
                    DrawFields(field, pf_Yposition, 0);
                }
                pf_Yposition += p_footer.HeightPt;
            }
        }

        public void DrawReportFooter()
        {
            RowHeight = 0;
            MultiRowTop = 0;
            detailCursorPosition = 0;

            dt_Yposition = ph_Yposition + PageHeaderHeight;
            pf_Yposition = detailEnd + dt_Yposition;
            rf_Yposition = pf_Yposition + PageFooterHeight;

            if (RenderReportFooterInBottom)
            {
                float remainingHeight = HeightPt - rf_Yposition;
                float requiredHeight = ReportFooterHeight + Margin.Bottom;

                if (remainingHeight < requiredHeight)
                {
                    IsLastpage = false;
                    AddNewPage();
                    IsLastpage = true;
                }
                rf_Yposition = HeightPt - requiredHeight;
            }

            foreach (EbReportFooter r_footer in ReportFooters)
            {
                if (!r_footer.RepeatOnAllPages)
                {
                    if (r_footer.AlwaysPrintTogether)
                    {
                        float remainingHeight = HeightPt - (pf_Yposition + PageFooterHeight) - Margin.Bottom;

                        if (remainingHeight < r_footer.HeightPt)
                        {
                            FooterDrawn = true;

                            IsLastpage = false;
                            AddNewPage();
                            IsLastpage = true;

                            rf_Yposition = RenderReportFooterInBottom
                                ? HeightPt - (r_footer.HeightPt + Margin.Bottom)
                                : Margin.Top;
                        }
                    }

                    float footerOffset = 0;
                    EbReportField[] SortedReportFields = this.ReportFieldsSortedPerRFooter[r_footer];
                    if (SortedReportFields.Length > 0)
                    {
                        for (int iSortPos = 0; iSortPos < SortedReportFields.Length; iSortPos++)
                        {
                            EbReportField field = SortedReportFields[iSortPos];

                            if ((HeightPt < field.TopPt + rf_Yposition + field.HeightPt + Margin.Bottom) && !RenderReportFooterInBottom)
                            {
                                AddNewPage();

                                footerOffset = field.TopPt;
                                FooterDrawn = true;
                                rf_Yposition = Margin.Top;
                            }
                            field.TopPt -= footerOffset;
                            DrawFields(field, rf_Yposition, 0);
                            field.TopPt += footerOffset;
                        }
                    }

                    rf_Yposition += r_footer.HeightPt;
                    if (footerOffset > 0)
                        rf_Yposition = rf_Yposition - footerOffset;
                }
            }
        }

        public void DrawRepeatingReportFooter()
        {
            RowHeight = 0;
            MultiRowTop = 0;
            detailCursorPosition = 0;

            dt_Yposition = ph_Yposition + (DrawDetailCompleted ? 0 : PageHeaderHeight);//PageHeaderHeight wont draw on last page if detail is completed and only footer is remaining 
            pf_Yposition = detailEnd + dt_Yposition;
            rf_Yposition = pf_Yposition + PageFooterHeight;

            if (RenderReportFooterInBottom)
            {
                float requiredHeight = ReportFooterHeightRepeatAsPf + Margin.Bottom;

                rf_Yposition = HeightPt - requiredHeight;
            }

            foreach (EbReportFooter r_footer in ReportFooters)
            {
                if (r_footer.RepeatOnAllPages)
                {

                    if (ReportFooterHeightRepeatAsPf != ReportFooterHeight)
                        rf_Yposition = HeightPt - r_footer.HeightPt - Margin.Bottom;

                    float footerOffset = 0;

                    EbReportField[] SortedReportFields = this.ReportFieldsSortedPerRFooter[r_footer];
                    if (SortedReportFields.Length > 0)
                    {
                        for (int iSortPos = 0; iSortPos < SortedReportFields.Length; iSortPos++)
                        {
                            EbReportField field = SortedReportFields[iSortPos];

                            field.TopPt -= footerOffset;
                            DrawFields(field, rf_Yposition, 0);
                            field.TopPt += footerOffset;

                        }
                    }
                    rf_Yposition += r_footer.HeightPt;
                }
            }
        }

        public void DrawFields(EbReportField field, float section_Yposition, int iterator)
        {
            if (!field.IsHidden)
            {
                List<Param> RowParams = null;
                if (field is EbDataField)
                {
                    EbDataField field_org = field as EbDataField;
                    if (GroupSummaryFields.ContainsKey(field.Name) || PageSummaryFields.ContainsKey(field.Name) || ReportSummaryFields.ContainsKey(field.Name))
                        CallSummerize(field_org, iterator);
                    if (AppearanceScriptCollection.ContainsKey(field.Name))
                        RunAppearanceExpression(field_org, iterator);
                    if (!string.IsNullOrEmpty(field_org.LinkRefId))
                        RowParams = CreateRowParamForLink(field_org, iterator);
                }
                field.DrawMe(section_Yposition, this, RowParams, iterator);
            }
        }

        public void RunAppearanceExpression(EbDataField field, int iterator)
        {
            if (field.Font is null || field.Font.Size == 0)
                field.Font = new EbFont { color = "#000000", FontName = "Roboto", Caps = false, Size = 10, Strikethrough = false, Style = FontStyle.NORMAL, Underline = false };
            PdfGEbFont pg_font = new PdfGEbFont
            {
                Caps = field.Font.Caps,
                color = field.Font.color,
                FontName = field.Font.FontName,
                Size = field.Font.Size,
                Strikethrough = field.Font.Strikethrough,
                Style = (PdfGFontStyle)(int)field.Font.Style,
                Underline = field.Font.Underline
            };
            EbPdfGlobals globals = new EbPdfGlobals
            {
                CurrentField = new PdfGReportField(field.LeftPt, field.WidthPt, field.TopPt, field.HeightPt, field.BackColor, field.ForeColor, field.IsHidden, pg_font)
            };

            if (this.EvaluatorVersion == EvaluatorVersion.Version_1)
                ExecuteExpressionV1((Script)AppearanceScriptCollection[field.Name], iterator, globals, field.DataFieldsUsedAppearance);
            else
                ExecuteExpressionV2(AppearanceScriptCollection[field.Name].ToString(), iterator, globals, field.DataFieldsUsedAppearance, true);

            field.SetValuesFromGlobals(globals.CurrentField);
        }

        public List<Param> CreateRowParamForLink(EbDataField field, int iterator)
        {
            List<Param> RowParams = new List<Param>();
            foreach (EbControl control in LinkCollection[field.LinkRefId])
            {
                Param x = DataSet.Tables[field.TableIndex].Rows[iterator].GetCellParam(control.Name);
                ArrayList IndexToRemove = new ArrayList();
                for (int i = 0; i < RowParams.Count; i++)
                {
                    if (RowParams[i].Name == control.Name)
                    {
                        IndexToRemove.Add(i); //the parameter will be in the report alredy
                    }
                }
                for (int i = 0; i < IndexToRemove.Count; i++)
                {
                    RowParams.RemoveAt(Convert.ToInt32(IndexToRemove[i]));
                }
                RowParams.Add(x);
            }
            if (!Parameters.IsEmpty())//the parameters which are alredy present in the rendering of current report
            {
                foreach (Param p in Parameters)
                {
                    RowParams.Add(p);
                }
            }
            return RowParams;
        }


        public Dictionary<int, byte[]> ImageCollection = new Dictionary<int, byte[]>();

        public void GetWatermarkImages()
        {
            if (this.ReportObjects != null)
            {
                foreach (EbReportField field in ReportObjects)
                {
                    if (field is EbWaterMark)
                    {
                        int id = (field as EbWaterMark).ImageRefId;
                        if (id != 0)
                        {
                            byte[] fileByte = GetImage(id);
                            if (!fileByte.IsEmpty())
                                WatermarkImages.Add(id, fileByte);
                        }
                    }
                }
            }
        }

        public void SetPassword(MemoryStream Ms1)
        {
            if (this.UserPassword != string.Empty || this.OwnerPassword != string.Empty)
            {
                Ms1.Position = 0;
                PdfReader = new PdfReader(Ms1);
                Stamp = new PdfStamper(PdfReader, Ms1);
                byte[] USER = Encoding.ASCII.GetBytes(UserPassword);
                byte[] OWNER = Encoding.ASCII.GetBytes(OwnerPassword);
                Stamp.SetEncryption(USER, OWNER, 0, PdfWriter.ENCRYPTION_AES_128);
                Stamp.FormFlattening = true;
                Stamp.Close();
            }
        }

        public void SetDetail()
        {
            if (!this.HidePrintDetails)
            {
                string timestamp = String.Format("{0:" + CultureInfo.DateTimeFormat.FullDateTimePattern + "}", CurrentTimestamp);
                ColumnText ct = new ColumnText(Canvas);
                Phrase phrase = new Phrase("page:" + CurrentReportPageNumber.ToString() + /*IsFirstpage.ToString() +*/ ", " + (RenderingUser?.FullName ?? "Machine User") + ", " + timestamp);
                phrase.Font.Size = 6;
                phrase.Font.Color = BaseColor.Gray;
                ct.SetSimpleColumn(phrase, 5, 2 + Margin.Bottom, (WidthPt - 20 - Margin.Left) - 20, 20 + Margin.Bottom, 15, Element.ALIGN_LEFT);
                ct.Go();
            }
        }

        public void AfterRedisGet(RedisClient Redis, IServiceClient client, RedisClient RedisReadOnly)
        {
            try
            {
                if (DataSourceRefId != string.Empty)
                {
                    EbDataSource = RedisReadOnly.Get<EbDataReader>(DataSourceRefId);
                    if (EbDataSource == null || EbDataSource.Sql == null || EbDataSource.Sql == string.Empty)
                    {
                        EbObjectParticularVersionResponse result = client.Get(new EbObjectParticularVersionRequest { RefId = DataSourceRefId });
                        this.EbDataSource = EbSerializers.Json_Deserialize(result.Data[0].Json);
                        Redis.Set<EbDataReader>(DataSourceRefId, EbDataSource);
                    }
                    if (EbDataSource.FilterDialogRefId != string.Empty)
                        EbDataSource.AfterRedisGet(Redis, client, RedisReadOnly);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.ToString());
            }
        }

        public override List<string> DiscoverRelatedRefids()
        {
            List<string> refids = new List<string>();
            if (!DataSourceRefId.IsEmpty())
            {
                EbDataReader ds = EbDataSource;
                if (ds is null)
                    refids.Add(DataSourceRefId);
            }
            foreach (EbReportDetail dt in Detail)
                foreach (EbReportField field in dt.GetFields())
                    if (field is EbDataField)
                    {
                        EbDataField fd_org = field as EbDataField;
                        if (!fd_org.LinkRefId.IsEmpty())
                            refids.Add(fd_org.LinkRefId);
                    }
            return refids;
        }

        public override void ReplaceRefid(Dictionary<string, string> RefidMap)
        {
            if (!DataSourceRefId.IsEmpty() && RefidMap.ContainsKey(DataSourceRefId))
                DataSourceRefId = RefidMap[DataSourceRefId];
            else
                DataSourceRefId = "";

            foreach (EbReportDetail dt in Detail)
            {
                foreach (EbReportField field in dt.GetFields())
                {
                    if (field is EbDataField)
                    {
                        EbDataField fd_org = field as EbDataField;
                        if (!fd_org.LinkRefId.IsEmpty())
                        {
                            if (RefidMap.ContainsKey(fd_org.LinkRefId))
                                fd_org.LinkRefId = RefidMap[fd_org.LinkRefId];
                            else
                                fd_org.LinkRefId = "";
                        }
                    }
                }
            }
        }

        public EbReport()
        {
            ReportHeaders = new List<EbReportHeader>();

            PageHeaders = new List<EbPageHeader>();

            Detail = new List<EbReportDetail>();

            PageFooters = new List<EbPageFooter>();

            ReportFooters = new List<EbReportFooter>();

            ReportGroups = new List<EbReportGroup>();
        }

        public static EbOperations Operations = ReportOperations.Instance;

        public byte[] GetImage(int refId)
        {
            if (ImageCollection.TryGetValue(refId, out var ImageBytea))
                return ImageBytea;

            byte[] ImgBytes = new byte[0];
            try
            {
                bool isSpecificSoln = Solution.SolutionID == "ebdbxrogi7imbm20220927054819";
                string key = string.Format("Img_{0}_{1}_{2}", Solution.SolutionID, 0, refId); //solnid, quality, filerefid

                if (isSpecificSoln)
                {
                    using (var redisReadOnly = this.pooledRedisManager.GetReadOnlyClient())
                    {
                        ImgBytes = redisReadOnly.Get<byte[]>(key) ?? new byte[0];
                    }
                }

                if (ImgBytes.Length == 0 && !string.IsNullOrEmpty(FileClient?.BearerToken))
                {
                    DownloadFileResponse response = FileClient.Get(new DownloadImageByIdRequest
                    {
                        ImageInfo = new ImageMeta
                        {
                            FileRefId = refId,
                            FileCategory = Common.Enums.EbFileCategory.Images
                        }
                    });
                    MemoryStream stream = response?.StreamWrapper?.Memorystream;
                    if (stream != null)
                    {
                        stream.Position = 0;
                        ImgBytes = stream.ToBytes();

                        if (isSpecificSoln && ImgBytes.Length > 0)
                            Redis.Set(key, ImgBytes);
                    }
                }

                ImageCollection.Add(refId, ImgBytes);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message + e.StackTrace);
            }

            return ImgBytes;
        }

        public void AddParamsNCalcsInGlobal(EbPdfGlobals globals)
        {
            foreach (string key in CalcValInRow.Keys) //adding Calc to global
            {
                globals["Calc"].Add(key, CalcValInRow[key]);
            }

            if (Parameters != null)
                foreach (Param p in Parameters) //adding Params to global
                {
                    globals["Params"].Add(p.Name, new GNTV { Name = p.Name, Type = (GlobalDbType)Convert.ToInt32(p.Type), Value = p.Value });
                }

            if (SummaryValInRow.Count > 0)
                foreach (string key in SummaryValInRow.Keys)
                {
                    globals["Summary"].Add(key.Replace(".", "_"), SummaryValInRow[key]);
                }
        }

        public string[] GetDataFieldsUsed(string code)
        {
            int i = 0;
            IEnumerable<string> matches = Regex.Matches(code, @"T[0-9]{1}\.\w+").OfType<Match>()
                         .Select(m => m.Groups[0].Value)
                         .Distinct();
            string[] _dataFieldsUsed = new string[matches.Count()];
            foreach (string match in matches)
                _dataFieldsUsed[i++] = match;
            return _dataFieldsUsed;
        }

        public Script CompileScriptV1(string code)
        {
            Script valscript = CSharpScript.Create<dynamic>(
                code, ScriptOptions.Default.WithReferences("Microsoft.CSharp", "System.Core").WithImports("System", "System.Collections.Generic", "System.Linq"),
                globalsType: typeof(EbPdfGlobals));
            valscript.Compile();
            return valscript;
        }

        public dynamic ExecuteScriptV1(EbPdfGlobals globals, Script valscript)
        {
            return valscript.RunAsync(globals).Result?.ReturnValue;
        }

        public object ExecuteExpressionV1(Script valscript, int irow, EbPdfGlobals globals, string[] _dataFieldsUsed)
        {
            foreach (string calcfd in _dataFieldsUsed)
            {
                string TName = calcfd.Split('.')[0];
                string fName = calcfd.Split('.')[1];
                int tableIndex = Convert.ToInt32(TName.Substring(1));
                int RowIndex = (tableIndex == this.DetailTableIndex) ? irow : 0;
                globals[TName].Add(fName, new GNTV { Name = fName, Type = (GlobalDbType)(int)this.DataSet.Tables[tableIndex].Columns[fName].Type, Value = this.DataSet.Tables[tableIndex].Rows[RowIndex][fName] });
            }

            AddParamsNCalcsInGlobal(globals);

            dynamic value = ExecuteScriptV1(globals, valscript);
            return value;

        }

        public void ExecuteHideExpressionV1(EbReportField field)
        {
            EbPdfGlobals globals = new EbPdfGlobals();
            string[] _dataFieldsUsed = GetDataFieldsUsed(field.HideExpression.Code);
            dynamic value = ExecuteExpressionV1(CompileScriptV1(field.HideExpression.Code), 0, globals, _dataFieldsUsed);

            if (value != null)
                field.IsHidden = (bool)value;
        }

        public void ExecuteLayoutExpressionV1(EbReportField field)
        {
            EbPdfGlobals globals = new EbPdfGlobals
            {
                CurrentField = new PdfGReportField(field.LeftPt, field.WidthPt, field.TopPt, field.HeightPt, field.BackColor, field.ForeColor, field.IsHidden, null)
            };

            string[] _dataFieldsUsed = GetDataFieldsUsed(field.LayoutExpression.Code);
            ExecuteExpressionV1(CompileScriptV1(field.LayoutExpression.Code), 0, globals, _dataFieldsUsed);

            field.SetValuesFromGlobals(globals.CurrentField);

        }

        public string[] GetOtherGlobalFieldsUsedV2(string code)
        {
            if (code.Contains("Summary.GetValue"))
            {
                int q = 1;
            }
            if (code.Contains("Summary"))
            {
                int q = 1;
            }
            int i = 0;
            IEnumerable<string> matches = Regex.Matches(code, @"Summary.\w+|Params.\w+|Calc.\w+").OfType<Match>()
                         .Select(m => m.Groups[0].Value)
                         .Distinct();
            string[] _dataFieldsUsed = new string[matches.Count()];
            foreach (string match in matches)
                _dataFieldsUsed[i++] = match;
            return _dataFieldsUsed;
        }

        public string GetProcessedCodeForScriptCollectionV2(string code)
        {
            string[] _dataFieldsUsed = GetDataFieldsUsed(code);
            return GetProcessedCodeV2(code, _dataFieldsUsed);
        }

        public string GetProcessedCodeV2(string code, string[] _dataFieldsUsed)
        {
            String processedCode = code;

            string[] others = GetOtherGlobalFieldsUsedV2(code);
            _dataFieldsUsed = (_dataFieldsUsed.Concat(others).ToArray()).OrderByDescending(c => c).ToArray();

            foreach (string calcfd in _dataFieldsUsed)
            {
                string fName = (calcfd.Split('.')[1]).Replace(";", "");
                processedCode = processedCode.Replace("." + fName, ".GetValue(\"" + fName + "\")");
            }

            return processedCode;
        }

        public void SetVariableV2(EbPdfGlobals globals)
        {
            Dictionary<string, object> dict = new Dictionary<string, object>
            {
                {"T0", globals["T0"]},
                {"T1", globals["T1"]},
                {"T2", globals["T2"]},
                {"T3", globals["T3"]},
                {"T4", globals["T4"]},
                {"T5", globals["T5"]},
                {"T6", globals["T6"]},
                {"T7", globals["T7"]},
                {"T8", globals["T8"]},
                {"T9", globals["T9"]},
                {"Params", globals["Params"]},
                {"Calc", globals["Calc"]},
                {"Summary", globals["Summary"]},
                { "CurrentField", globals["CurrentField"] }
            };
            evaluator.SetVariable(dict);
        }

        public object ExecuteExpressionV2(string code, int irow, EbPdfGlobals globals, string[] _dataFieldsUsed, bool Isprocessed)
        {
            object value = null;
            try
            {
                string processedCode;
                if (Isprocessed)
                    processedCode = code;
                else
                    processedCode = GetProcessedCodeV2(code, _dataFieldsUsed);

                foreach (string calcfd in _dataFieldsUsed)
                {
                    string TName = calcfd.Split('.')[0];
                    string fName = calcfd.Split('.')[1];
                    int tableIndex = Convert.ToInt32(TName.Substring(1));
                    int RowIndex = (tableIndex == this.DetailTableIndex) ? irow : 0;
                    globals[TName].Add(fName, new GNTV { Name = fName, Type = (GlobalDbType)(int)this.DataSet.Tables[tableIndex].Columns[fName].Type, Value = this.DataSet.Tables[tableIndex].Rows[RowIndex][fName] });
                }
                AddParamsNCalcsInGlobal(globals);
                SetVariableV2(globals);

                value = evaluator.Execute(processedCode);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message + e.StackTrace);
            }
            return value;
        }

        public void ExecuteHideExpressionV2(EbReportField field)
        {
            EbPdfGlobals globals = new EbPdfGlobals();

            string[] _dataFieldsUsed = GetDataFieldsUsed(field.HideExpression.Code);
            object value = ExecuteExpressionV2(field.HideExpression.Code, 0, globals, _dataFieldsUsed, false);

            if (value != null)
                field.IsHidden = (bool)value;
        }

        public void ExecuteLayoutExpressionV2(EbReportField field)
        {
            EbPdfGlobals globals = new EbPdfGlobals
            {
                CurrentField = new PdfGReportField(field.LeftPt, field.WidthPt, field.TopPt, field.HeightPt, field.BackColor, field.ForeColor, field.IsHidden, null)
            };
            string[] _dataFieldsUsed = GetDataFieldsUsed(field.LayoutExpression.Code);
            ExecuteExpressionV2(field.LayoutExpression.Code, 0, globals, _dataFieldsUsed, false);

            field.SetValuesFromGlobals(globals.CurrentField);
        }

        public void ExecuteRendering(string BToken, string RToken, MemoryStream Ms1, List<Param> _params, EbConnectionFactory EbConnectionFactory, bool useRwDb, Document MainDocument = null)
        {
            this.InitializeReportObects(BToken, RToken);

            this.GetData4Pdf(_params, EbConnectionFactory, useRwDb);

            this.InitializePdfObjects(Ms1, MainDocument);

            if (IsLanguageEnabled && Solution.IsMultiLanguageEnabled)
            {
                string _language = "ml";
                string[] Keys = LabelsCollection.ToArray();
                LabelKeyValues = EbObjectsHelper.GetKeyValues(new GetDictionaryValueRequest { Keys = Keys, Language = _language }, EbConnectionFactory.ObjectsDB);
            }
            detailCursorPosition = 0;

            this.Doc.Open();

            if (this.DataSet != null)
                this.Draw();
            else
                throw new Exception();
        }

        public void GetData4Pdf(List<Param> _params, EbConnectionFactory EbConnectionFactory, bool useRwDb = false)
        {
            DataSourceDataSetResponse resp = null;
            using (var redisReadOnly = this.pooledRedisManager.GetReadOnlyClient())
                resp = EbObjectsHelper.ExecuteDataset(this.DataSourceRefId, this.RenderingUser.Id, _params, EbConnectionFactory, this.Redis, redisReadOnly, useRwDb);
            this.Parameters = _params;
            this.DataSet = resp.DataSet;

            if (this.DataSet != null)
                FillingCollections(this);

        }

        public void InitializeReportObects(string BToken, string RToken)
        {
            if (string.IsNullOrEmpty(FileClient.BearerToken) && !string.IsNullOrEmpty(BToken))
            {
                FileClient.BearerToken = BToken;
                FileClient.RefreshToken = RToken;
            }

            this.CultureInfo = CultureHelper.GetSerializedCultureInfo(this.ReadingUser?.Preference.Locale ?? "en-US").GetCultureInfo();

            this.GetWatermarkImages();
        }

        public void InitializePdfObjects(MemoryStream Ms1, Document MainDocument = null)
        {
            if (MainDocument == null)
            {
                float _width = this.WidthPt - this.Margin.Left;
                float _height = this.HeightPt - this.Margin.Top - this.Margin.Bottom;
                this.HeightPt = _height;
                Rectangle rec = new Rectangle(_width, _height);

                this.Doc = new Document(rec);
                this.Doc.SetMargins(this.Margin.Left, this.Margin.Right, this.Margin.Top, this.Margin.Bottom);
                this.Writer = PdfWriter.GetInstance(this.Doc, Ms1);
                this.Writer.PageEvent = new HeaderFooter(this);
                this.Writer.Open();
                this.Writer.CloseStream = true;//important
                this.MasterPageNumber = this.Writer.PageNumber;
                this.CurrentReportPageNumber = 1;
                this.Canvas = this.Writer.DirectContent;
                MainDocument = this.Doc;
            }
            else
                this.Doc = MainDocument;
        }

        public void Draw()
        {
            if (this?.DataSet?.Tables[this.DetailTableIndex]?.Rows.Count > 0)
            {
                this.DrawDetail();
            }
            else
            {
                this.DrawPageHeader();
                this.detailEnd += 30;
                this.DrawPageFooter();
                this.DrawReportFooter();
                throw new Exception("Dataset is null, refid " + this.DataSourceRefId);
            }
            if (ReportFooterHeightRepeatAsPf != ReportFooterHeight)
                this.DrawReportFooter();
        }

        public void HandleExceptionPdf(Exception e)
        {
            ColumnText ct = new ColumnText(this.Canvas);
            Phrase phrase;
            if (this?.DataSet?.Tables[this.DetailTableIndex]?.Rows.Count > 0)
                phrase = new Phrase("Something went wrong. Please check the parameters or contact admin" + e.Message + e.StackTrace);
            else
                phrase = new Phrase("No Data available. Please check the parameters or contact admin" + e.Message + e.StackTrace);

            phrase.Font.Size = 10;
            float y = this.HeightPt - (this.ReportHeaderHeight + this.Margin.Top + this.PageHeaderHeight);

            ct.SetSimpleColumn(phrase, this.LeftPt + 30, y - 80, this.WidthPt - 30, y, 15, Element.ALIGN_CENTER);
            ct.Go();
        }

        private Paragraph CreateSimpleHtmlParagraph(String text)
        {

            // Report.Doc.Add(CreateSimpleHtmlParagraph("this is <b>bold</b> text"));
            // Report.Doc.Add(CreateSimpleHtmlParagraph("this is <i>italic</i> text"));
            //Our return object
            Paragraph p = new Paragraph();

            //ParseToList requires a StreamReader instead of just text
            using (StringReader sr = new StringReader(text))
            {
                //Parse and get a collection of elements
                var elements = HtmlWorker.ParseToList(sr, null);
                foreach (IElement e in elements)
                {
                    //Add those elements to the paragraph
                    p.Add(e);
                }
            }
            //Return the paragraph
            return p;
        }

        public void FillingCollections(EbReport Report)
        {
            foreach (EbReportHeader r_header in Report.ReportHeaders)
                Fill(Report, r_header.GetFields(), EbReportSectionType.ReportHeader);

            foreach (EbReportFooter r_footer in Report.ReportFooters)
                Fill(Report, r_footer.GetFields(), EbReportSectionType.ReportFooter);

            foreach (EbPageHeader p_header in Report.PageHeaders)
                Fill(Report, p_header.GetFields(), EbReportSectionType.PageHeader);

            foreach (EbReportDetail detail in Report.Detail)
                Fill(Report, detail.GetFields(), EbReportSectionType.Detail);

            foreach (EbPageFooter p_footer in Report.PageFooters)
                Fill(Report, p_footer.GetFields(), EbReportSectionType.PageFooter);

            foreach (EbReportGroup group in Report.ReportGroups)
            {
                Fill(Report, group.GroupHeader.GetFields(), EbReportSectionType.ReportGroups);
                Fill(Report, group.GroupFooter.GetFields(), EbReportSectionType.ReportGroups);
                foreach (EbReportField field in group.GroupHeader.GetFields())
                {
                    if (field is EbDataField && !Report.Groupheaders.ContainsKey((field as EbDataField).ColumnName))
                    {
                        Report.Groupheaders.Add((field as EbDataField).ColumnName, new ReportGroupItem
                        {
                            field = field as EbDataField,
                            PreviousValue = string.Empty,
                            order = group.GroupHeader.Order
                        });
                    }
                }
                foreach (EbReportField field in group.GroupFooter.GetFields())
                {
                    if (field is EbDataField && !Report.GroupFooters.ContainsKey((field as EbDataField).ColumnName))
                    {
                        Report.GroupFooters.Add((field as EbDataField).ColumnName, new ReportGroupItem
                        {
                            field = field as EbDataField,
                            PreviousValue = string.Empty,
                            order = group.GroupHeader.Order
                        });
                    }
                }
            }

        }

        private void Fill(EbReport Report, List<EbReportField> fields, EbReportSectionType section_typ)
        {
            foreach (EbReportField field in fields)
            {
                if (!String.IsNullOrEmpty(field.HideExpression?.Code))
                {
                    if (Report.EvaluatorVersion == EvaluatorVersion.Version_1)
                        Report.ExecuteHideExpressionV1(field);
                    else
                        Report.ExecuteHideExpressionV2(field);
                }
                if (!field.IsHidden && !String.IsNullOrEmpty(field.LayoutExpression?.Code))
                {
                    if (Report.EvaluatorVersion == EvaluatorVersion.Version_1)
                        Report.ExecuteLayoutExpressionV1(field);
                    else
                        Report.ExecuteLayoutExpressionV2(field);
                }
                if (field is EbDataField)
                {
                    EbDataField field_org = field as EbDataField;
                    if (!string.IsNullOrEmpty(field_org.LinkRefId) && !Report.LinkCollection.ContainsKey(field_org.LinkRefId))
                        FindControls(Report, field_org, ObjectsDB, Redis);//Finding the link's parameter controls

                    if (section_typ == EbReportSectionType.Detail)
                        FindLargerDataTable(Report, field_org);// finding the table of highest rowcount from dataset

                    if (field is IEbDataFieldSummary)
                        FillSummaryCollection(Report, field_org, section_typ);

                    if (field is EbCalcField && !Report.ValueScriptCollection.ContainsKey(field.Name) && !string.IsNullOrEmpty((field_org as EbCalcField).ValExpression?.Code))
                    {
                        if (Report.EvaluatorVersion == EvaluatorVersion.Version_1)
                        {
                            Script valscript = Report.CompileScriptV1((field as EbCalcField).ValExpression.Code);
                            Report.ValueScriptCollection.Add(field.Name, valscript);
                        }
                        else
                        {
                            string processedCode = Report.GetProcessedCodeForScriptCollectionV2((field as EbCalcField).ValExpression.Code);
                            Report.ValueScriptCollection.Add(field.Name, processedCode);
                        }
                    }

                    if (!field.IsHidden && !Report.AppearanceScriptCollection.ContainsKey(field.Name) && !string.IsNullOrEmpty(field_org.AppearExpression?.Code))
                    {
                        Script appearscript = Report.CompileScriptV1(field_org.AppearExpression.Code);
                        Report.AppearanceScriptCollection.Add(field.Name, appearscript);
                    }
                }
                else if (field is EbText)
                {
                    LabelsCollection.Add(field.Title);
                }
            }
        }

        public void FindLargerDataTable(EbReport Report, EbDataField field)
        {
            if (!Report.HasRows || field.TableIndex != Report.DetailTableIndex)
            {
                if (Report.DataSet?.Tables.Count > 0)
                {
                    if (Report.DataSet.Tables[field.TableIndex].Rows != null)
                    {
                        Report.HasRows = true;
                        int r_count = Report.DataSet.Tables[field.TableIndex].Rows.Count;
                        Report.DetailTableIndex = (r_count > Report.MaxRowCount) ? field.TableIndex : Report.DetailTableIndex;
                        Report.MaxRowCount = (r_count > Report.MaxRowCount) ? r_count : Report.MaxRowCount;
                    }
                    else
                    {
                        Console.WriteLine("Report.DataSet.Tables[field.TableIndex].Rows is null");
                    }
                }
                else
                {
                    Console.WriteLine("Report.DataSet.Tables.Count is 0");
                }
            }
        }

        public void FindControls(EbReport report, EbDataField field, IDatabase ObjectsDB, IRedisClient Redis)
        {
            string LinkRefid = field.LinkRefId;
            string linkDsRefid = string.Empty;

            List<EbObjectWrapper> res = EbObjectsHelper.GetParticularVersion(ObjectsDB, LinkRefid);
            if (res[0].EbObjectType == 3)
                linkDsRefid = EbSerializers.Json_Deserialize<EbReport>(res[0].Json).DataSourceRefId;//Getting the linked report
            else if (res[0].EbObjectType == 16)
                linkDsRefid = EbSerializers.Json_Deserialize<EbTableVisualization>(res[0].Json).DataSourceRefId;//Getting the linked table viz
            else if (res[0].EbObjectType == 17)
                linkDsRefid = EbSerializers.Json_Deserialize<EbChartVisualization>(res[0].Json).DataSourceRefId;//Getting the linked chart viz

            EbDataReader LinkDatasource = null;
            using (var redisReadOnly = this.pooledRedisManager.GetReadOnlyClient())
                LinkDatasource = redisReadOnly.Get<EbDataReader>(linkDsRefid);
            if (LinkDatasource == null || LinkDatasource.Sql == null || LinkDatasource.Sql == string.Empty)
            {
                List<EbObjectWrapper> result = EbObjectsHelper.GetParticularVersion(ObjectsDB, linkDsRefid);
                LinkDatasource = EbSerializers.Json_Deserialize(result[1].Json);
                Redis.Set<EbDataReader>(linkDsRefid, LinkDatasource);
            }

            if (!string.IsNullOrEmpty(LinkDatasource.FilterDialogRefId))
            {
                using (var redisReadOnly = this.pooledRedisManager.GetReadOnlyClient())
                    LinkDatasource.FilterDialog = redisReadOnly.Get<EbFilterDialog>(LinkDatasource.FilterDialogRefId);
                if (LinkDatasource.FilterDialog == null)
                {
                    List<EbObjectWrapper> result = EbObjectsHelper.GetParticularVersion(ObjectsDB, LinkDatasource.FilterDialogRefId);

                    LinkDatasource.FilterDialog = EbSerializers.Json_Deserialize(result[1].Json);
                    Redis.Set<EbFilterDialog>(LinkDatasource.FilterDialogRefId, LinkDatasource.FilterDialog);
                }
                report.LinkCollection[LinkRefid] = LinkDatasource.FilterDialog.Controls;
            }
        }

        public void FillSummaryCollection(EbReport report, EbDataField field, EbReportSectionType section_typ)
        {
            if (section_typ == EbReportSectionType.ReportGroups)
            {
                if (!report.GroupSummaryFields.ContainsKey(field.SummaryOf))
                {
                    report.GroupSummaryFields.Add(field.SummaryOf, new List<EbDataField> { field });
                }
                else
                {
                    report.GroupSummaryFields[field.SummaryOf].Add(field);
                }
            }
            if (section_typ == EbReportSectionType.PageFooter)
            {
                if (!report.PageSummaryFields.ContainsKey(field.SummaryOf))
                {
                    report.PageSummaryFields.Add(field.SummaryOf, new List<EbDataField> { field });
                }
                else
                {
                    report.PageSummaryFields[field.SummaryOf].Add(field);
                }
            }
            if (section_typ == EbReportSectionType.ReportFooter)
            {
                if (!report.ReportSummaryFields.ContainsKey(field.SummaryOf))
                {
                    report.ReportSummaryFields.Add(field.SummaryOf, new List<EbDataField> { field });
                }
                else
                {
                    report.ReportSummaryFields[field.SummaryOf].Add(field);
                }
            }
        }

        [JsonIgnore]
        public bool IsRenderingComplete { get; set; } = false;

        public void Reset()
        {
            _docName = null;
            _rhHeight = 0;
            _rhHeightAsPh = 0;
            _phHeight = 0;
            _pfHeight = 0;
            _rfHeight = 0;
            _rfHeightAsPf = 0;
            _dtHeight = 0;
            _ghHeight = 0;
            _gfHeight = 0;
            possibleSpaceForDetail = 0;

            rh_Yposition = 0f;
            rf_Yposition = 0f;
            pf_Yposition = 0f;
            ph_Yposition = 0f;
            dt_Yposition = 0f;

            detailCursorPosition = 0;
            detailEnd = 0;
            FooterDrawn = false;
            PreviousGheadersIterator = 0;

            Groupheaders?.Values.ToList().ForEach(g => g.PreviousValue = string.Empty);
            GroupFooters?.Clear();

            PageSummaryFields?.Clear();
            ReportSummaryFields?.Clear();
            GroupSummaryFields?.Clear();

            WatermarkImages?.Clear();
            WaterMarkList?.Clear();

            ValueScriptCollection?.Clear();
            AppearanceScriptCollection?.Clear();

            CalcValInRow?.Clear();
            SummaryValInRow?.Clear();

            LabelsCollection?.Clear();
            LabelKeyValues?.Clear();

            LinkCollection?.Clear();

            SerialNumber = 0;
            //CurrentReportPageNumber = 1;
            MasterPageNumber = 0;
            IsLastpage = false;
            //IsFirstpage = false;
            _currentTimeStamp = DateTime.MinValue;

            __fieldsNotSummaryPerDetail = null;
            __reportFieldsSortedPerDetail = null;
            __reportFieldsSortedPerRFooter = null;

            DataSet.Tables.Clear();
            DataSet = null;
            DataSet = null;
            evaluator = new EbSciptEvaluator
            {
                OptionScriptNeedSemicolonAtTheEndOfLastExpression = false
            };

        }

    }

    [EnableInBuilder(BuilderType.Report)]
    public class EbTableLayoutCell : EbReportObject
    {
        [EnableInBuilder(BuilderType.Report)]
        public int RowIndex { set; get; }

        [EnableInBuilder(BuilderType.Report)]
        public int CellIndex { set; get; }

        [EnableInBuilder(BuilderType.Report)]
        public List<EbReportField> ControlCollection { set; get; }
    }

    [EnableInBuilder(BuilderType.Report)]
    public class EbReportSection : EbReportObject
    {
        [EnableInBuilder(BuilderType.Report)]
        [UIproperty]
        [MetaOnly]
        public string SectionHeight { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        [HideInPropertyGrid]
        public List<EbReportField> Fields { get; set; }

        public override string Left { get; set; }

        public override string Top { get; set; }

        public override float LeftPt { get; set; }

        public override float TopPt { get; set; }

        public override string Title { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        [HideInPropertyGrid]
        public override string Height { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        [HideInPropertyGrid]
        public override string Width { get; set; }

        private List<EbReportField> _list = null;
        public List<EbReportField> GetFields()
        {
            if (_list == null)
            {
                _list = new List<EbReportField>();
                foreach (EbReportField f in Fields)
                {
                    if (f is EbTable_Layout)
                    {
                        foreach (EbTableLayoutCell c in (f as EbTable_Layout).CellCollection)
                        {
                            foreach (EbReportField r in c.ControlCollection)
                                _list.Add(r);
                        }
                    }
                    else
                        _list.Add(f);
                }
            }
            return _list;
        }
    }

    [EnableInBuilder(BuilderType.Report)]
    public class EbReportHeader : EbReportSection
    {
        [EnableInBuilder(BuilderType.Report)]
        public bool RepeatOnAllPages { get; set; }

        public override string GetDesignHtml()
        {
            return "<div class='pageHeaders' eb-type='ReportHeader' tabindex='1' id='@id' data_val='0' style='width :100%;height: @SectionHeight ;position: relative'> </div>".RemoveCR().DoubleQuoted();  // background-color:@BackColor ;
        }

        public override string GetJsInitFunc()
        {
            return @"
    this.Init = function(id)
        {
    this.BackColor = 'transparent';
};";
        }
    }

    [EnableInBuilder(BuilderType.Report)]
    public class EbPageHeader : EbReportSection
    {
        public override string GetDesignHtml()
        {
            return "<div class='pageHeaders' eb-type='PageHeader' tabindex='1' id='@id' data_val='1' style='width :100%;height: @SectionHeight ; position: relative'> </div>".RemoveCR().DoubleQuoted(); //background-color:@BackColor ;
        }

        public override string GetJsInitFunc()
        {
            return @"
    this.Init = function(id)
        {
    this.BackColor = 'transparent';
};";
        }
    }

    [EnableInBuilder(BuilderType.Report)]
    public class EbReportDetail : EbReportSection
    {
        public override string GetDesignHtml()
        {
            return "<div class='pageHeaders' eb-type='ReportDetail' tabindex='1' id='@id' data_val='2' style='width :100%;height: @SectionHeight ; position: relative'> </div>".RemoveCR().DoubleQuoted(); //background-color:@BackColor ;
        }

        public override string GetJsInitFunc()
        {
            return @"
    this.Init = function(id)
        {
    this.BackColor = 'transparent';
};";
        }

    }

    [EnableInBuilder(BuilderType.Report)]
    public class EbPageFooter : EbReportSection
    {
        public override string GetDesignHtml()
        {
            return "<div class='pageHeaders' eb-type='PageFooter' tabindex='1' id='@id' data_val='3' style='width :100%;height: @SectionHeight ; position: relative'> </div>".RemoveCR().DoubleQuoted(); //background-color:@BackColor ;
        }

        public override string GetJsInitFunc()
        {
            return @"
    this.Init = function(id)
        {
    this.BackColor = 'transparent';
};";
        }
    }

    [EnableInBuilder(BuilderType.Report)]
    public class EbReportFooter : EbReportSection
    {
        [EnableInBuilder(BuilderType.Report)]
        public bool RepeatOnAllPages { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        public bool AlwaysPrintTogether { get; set; }

        public override string GetDesignHtml()
        {
            return "<div class='pageHeaders' eb-type='ReportFooter' tabindex='1' id='@id' data_val='4' style='width :100%;height: @SectionHeight ; position: relative'> </div>".RemoveCR().DoubleQuoted(); //background-color:@BackColor ;
        }

        public override string GetJsInitFunc()
        {
            return @"
    this.Init = function(id)
        {
    this.BackColor = 'transparent';
};";
        }
    }

    [EnableInBuilder(BuilderType.Report)]
    public class EbReportGroup : EbReportObject
    {
        [EnableInBuilder(BuilderType.Report)]
        [HideInPropertyGrid]
        public EbGroupHeader GroupHeader { set; get; }

        [EnableInBuilder(BuilderType.Report)]
        [HideInPropertyGrid]
        public EbGroupFooter GroupFooter { set; get; }

        public override string Height { get; set; }

        public override string Width { get; set; }

        public override string Left { get; set; }

        public override string Top { get; set; }

        public override float LeftPt { get; set; }

        public override float TopPt { get; set; }

        public override string Title { get; set; }

        public override string BackColor { get; set; }

        public override float HeightPt { get; set; }

        public override float WidthPt { get; set; }
    }

    [EnableInBuilder(BuilderType.Report)]
    public class EbGroupHeader : EbReportSection
    {
        [EnableInBuilder(BuilderType.Report)]
        [UIproperty]
        [MetaOnly]
        public int Order { set; get; }

        [EnableInBuilder(BuilderType.Report)]
        public bool GroupInNewPage { get; set; }

        public override string GetDesignHtml()
        {
            return "<div class='pageGroups_header' eb-type='GroupHeader' type='GroupHeader' g-order='@Order' tabindex='1' id='@id' style='width :100%;height: @SectionHeight ; position: relative'> </div>".RemoveCR().DoubleQuoted();
        }

        public override string GetJsInitFunc()
        {
            return @"
    this.Init = function(id)
        {
    this.SectionHeight = '60px';
};";
        }
    }

    [EnableInBuilder(BuilderType.Report)]
    public class EbGroupFooter : EbReportSection
    {
        [EnableInBuilder(BuilderType.Report)]
        [UIproperty]
        [MetaOnly]
        public int Order { set; get; }

        public override string GetDesignHtml()
        {
            return "<div class='pageGroups_footer' eb-type='GroupFooter' type='GroupFooter' g-order='@Order' tabindex='1' id='@id' style='width :100%;height: @SectionHeight ; position: relative'> </div>".RemoveCR().DoubleQuoted();
        }

        public override string GetJsInitFunc()
        {
            return @"
    this.Init = function(id)
        {
    this.SectionHeight = '60px';
};";
        }
    }
    public class ReportGroupItem
    {
        public EbDataField field;
        public string PreviousValue;
        public int order;
    }
}
