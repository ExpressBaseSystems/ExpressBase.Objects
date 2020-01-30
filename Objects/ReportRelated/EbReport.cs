using ExpressBase.Common;
using ExpressBase.Common.Constants;
using ExpressBase.Common.Data;
using ExpressBase.Common.EbServiceStack;
using ExpressBase.Common.EbServiceStack.ReqNRes;
using ExpressBase.Common.Extensions;
using ExpressBase.Common.LocationNSolution;
using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using ExpressBase.Common.ServiceClients;
using ExpressBase.Common.Singletons;
using ExpressBase.Common.Structures;
using ExpressBase.Objects.Objects;
using ExpressBase.Objects.ServiceStack_Artifacts;
using ExpressBase.Security;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using Newtonsoft.Json;
using ServiceStack;
using ServiceStack.Redis;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

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
        [PropertyGroup("General")]
        public override string DisplayName { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        [PropertyGroup("General")]
        public override string Name { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        [PropertyGroup("General")]
        public override string Description { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        [HideInPropertyGrid]
        public override string VersionNumber { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        [HideInPropertyGrid]
        public override string Status { get; set; }

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
        [PropertyGroup("General")]
        [UIproperty]
        public string UserPassword { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        [PropertyGroup("General")]
        [UIproperty]
        public string OwnerPassword { get; set; }

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
        [PropertyGroup("Appearance")]
        public bool IsLandscape { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        [PropertyEditor(PropertyEditorType.ImageSeletor)]
        [PropertyGroup("Appearance")]
        public string BackgroundImage { get; set; }

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
        [PropertyGroup("Data")]
        public string DataSourceRefId { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        [PropertyGroup("Appearance")]
        [UIproperty]
        [PropertyEditor(PropertyEditorType.FontSelector)]
        public EbFont Font { get; set; }

        //[JsonIgnore]
        //public ColumnColletion ColumnColletion { get; set; }

        [JsonIgnore]
        public int DetailTableIndex { get; set; } = 0;

        [JsonIgnore]
        public int MaxRowCount { get; set; } = 0;

        [JsonIgnore]
        public bool HasRows = false;

        [JsonIgnore]
        public int iDetailRowPos { get; set; }

        [JsonIgnore]
        public Dictionary<string, List<EbDataField>> PageSummaryFields { get; set; }

        [JsonIgnore]
        public Dictionary<string, List<EbDataField>> ReportSummaryFields { get; set; }

        [JsonIgnore]
        public Dictionary<int, byte[]> WatermarkImages { get; set; }

        [JsonIgnore]
        public List<object> WaterMarkList { get; set; }

        [JsonIgnore]
        public Dictionary<string, Script> ValueScriptCollection { get; set; }

        [JsonIgnore]
        public Dictionary<string, Script> AppearanceScriptCollection { get; set; }

        [JsonIgnore]
        public Dictionary<string, ReportGroupItem> Groupheaders { get; set; }

        [JsonIgnore]
        public Dictionary<string, ReportGroupItem> GroupFooters { get; set; }

        [JsonIgnore]
        public EbDataSet DataSet { get; set; }

        [JsonIgnore]
        public bool IsLastpage { get; set; }

        [JsonIgnore]
        public int PageNumber { get; set; }

        private DateTime _currentTimeStamp = DateTime.MinValue;
        [JsonIgnore]
        public DateTime CurrentTimestamp
        {
            get
            {
                if (_currentTimeStamp == DateTime.MinValue)
                {
                    _currentTimeStamp = DateTime.UtcNow;
                    string timezone = ReadingUser.Preference.TimeZone;
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
        public MemoryStream Ms1 { get; set; }

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

        private float _dtHeight = 0;
        [JsonIgnore]
        public float DetailHeight
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
        private float dt_fillheight = 0;
        [JsonIgnore]
        public float DT_FillHeight
        {
            get
            {
                RowColletion rows = (DataSourceRefId != string.Empty) ? DataSet.Tables[0].Rows : null;
                //if (rows != null)
                //{
                //    if (rows.Count > 0)
                //    {
                //        float a = rows.Count * DetailHeight;
                //        float b = HeightPt - (PageHeaderHeight + PageFooterHeight + ReportHeaderHeight + ReportFooterHeight);
                //        if (a < b && PageNumber == 1)
                //            IsLastpage = true;
                //    }
                //}

                if (PageNumber == 1 && IsLastpage)
                    dt_fillheight = HeightPt - (PageHeaderHeight + PageFooterHeight + ReportHeaderHeight + ReportFooterHeight + Margin.Top + Margin.Bottom);
                else if (PageNumber == 1)
                    dt_fillheight = HeightPt - (ReportHeaderHeight + Margin.Top + PageHeaderHeight + PageFooterHeight + Margin.Bottom);
                else if (IsLastpage == true)
                    dt_fillheight = HeightPt - (PageHeaderHeight + PageFooterHeight + Margin.Bottom + Margin.Top + ReportFooterHeight);
                else
                    dt_fillheight = HeightPt - (PageHeaderHeight + PageFooterHeight + Margin.Bottom + Margin.Top);
                return dt_fillheight;
            }
        }

        [JsonIgnore]
        public EbBaseService ReportService { get; set; }

        [JsonIgnore]
        public EbStaticFileClient FileClient { get; set; }

        [JsonIgnore]
        public string SolutionId { get; set; }

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
        public float detailprintingtop = 0;

        [JsonIgnore]
        public double detailEnd = 0;

        [JsonIgnore]
        public bool FooterDrawn = false;

        [JsonIgnore]
        private int PreviousGheadersSlNo { get; set; }

        [JsonIgnore]
        public Dictionary<string, List<EbControl>> LinkCollection { get; set; }

        [JsonIgnore]
        public Dictionary<string, NTV> CalcValInRow { get; set; } = new Dictionary<string, NTV>();

        public dynamic GetDataFieldtValue(string column_name, int i, int tableIndex)
        {
            dynamic value = null;
            int index;
            if (DataSet != null && DataSet.Tables.Count > 0 && DataSet.Tables[tableIndex].Rows != null)
            {
                index = (DataSet.Tables[tableIndex].Rows.Count > 1) ? i : 0;
                EbDbTypes type = (DataSet.Tables[tableIndex].Columns[column_name].Type);
                value = (type == EbDbTypes.Bytea) ? DataSet.Tables[tableIndex].Rows[index][column_name] : DataSet.Tables[tableIndex].Rows[index][column_name].ToString();
            }
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

        public void CallSummerize(EbDataField field, int serialnumber)
        {
            string column_val = string.Empty;
            Globals globals = new Globals
            {
                CurrentField = field
            };
            AddParamsNCalcsInGlobal(globals);
            if (field is EbCalcField)
            {
                EbCalcField field_orig = field as EbCalcField;
                foreach (string calcfd in field_orig.DataFieldsUsedCalc)
                {
                    string TName = calcfd.Split('.')[0];
                    string fName = calcfd.Split('.')[1];
                    int tableindex = Convert.ToInt32(TName.Substring(1));
                    globals[TName].Add(fName, new NTV { Name = fName, Type = DataSet.Tables[tableindex].Columns[fName].Type, Value = DataSet.Tables[tableindex].Rows[serialnumber][fName] });
                }
                column_val = (ValueScriptCollection[field_orig.Name].RunAsync(globals)).Result.ReturnValue.ToString();
            }
            else
            {
                column_val = GetDataFieldtValue(field.ColumnName, serialnumber, field.TableIndex);
            }
            List<EbDataField> SummaryList;
            if (PageSummaryFields.ContainsKey(field.Name))
            {
                SummaryList = PageSummaryFields[field.Name];
                foreach (object item in SummaryList)
                {
                    (item as IEbDataFieldSummary).Summarize(column_val);
                }
            }
            if (ReportSummaryFields.ContainsKey(field.Name))
            {
                SummaryList = ReportSummaryFields[field.Name];
                foreach (object item in SummaryList)
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
            detailprintingtop = 0;
            foreach (EbReportHeader r_header in ReportHeaders)
            {
                foreach (EbReportField field in r_header.Fields)
                {
                    DrawFields(field, rh_Yposition, 0);
                }
                rh_Yposition += r_header.HeightPt;
            }
        }

        public void DrawPageHeader()
        {
            RowHeight = 0;
            MultiRowTop = 0;
            detailprintingtop = 0;
            ph_Yposition = (PageNumber == 1) ? ReportHeaderHeight + this.Margin.Top : this.Margin.Top;
            foreach (EbPageHeader p_header in PageHeaders)
            {
                foreach (EbReportField field in p_header.Fields)
                {
                    DrawFields(field, ph_Yposition, 0);
                }
                ph_Yposition += p_header.HeightPt;
            }
        }

        public void DrawDetail()
        {
            RowColletion rows = (DataSourceRefId != string.Empty) ? DataSet.Tables[DetailTableIndex].Rows : null;

            ph_Yposition = (PageNumber == 1) ? ReportHeaderHeight + this.Margin.Top : this.Margin.Top;
            dt_Yposition = ph_Yposition + PageHeaderHeight;
            if (rows != null && HasRows == true)
            {
                for (iDetailRowPos = 0; iDetailRowPos < rows.Count; iDetailRowPos++)
                {
                    if (Groupheaders != null && Groupheaders.Count > 0)
                        foreach (KeyValuePair<string, ReportGroupItem> grp in Groupheaders)
                        {
                            ReportGroupItem grpitem = grp.Value;
                            string column_val = GetDataFieldtValue(grpitem.field.ColumnName, iDetailRowPos, grpitem.field.TableIndex);
                            if (grpitem.PreviousValue != column_val)
                            {
                                DrawGroupHeader(grpitem.order, iDetailRowPos);
                                grpitem.PreviousValue = column_val;
                                int i;
                                for (i = grpitem.order + 1; i < Groupheaders.Count; i++)
                                    Groupheaders.Values.ElementAt(i).PreviousValue = string.Empty;
                            }
                        }
                    if (detailprintingtop < DT_FillHeight && DT_FillHeight - detailprintingtop >= DetailHeight)
                    {
                        DoLoopInDetail(iDetailRowPos);
                    }
                    else
                    {
                        AddNewPage();
                        DoLoopInDetail(iDetailRowPos);
                    }
                }
                IsLastpage = true;
            }
            else
            {
                IsLastpage = true;
                DoLoopInDetail(0);
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
                        __fieldsNotSummaryPerDetail[detail] = detail.Fields.Where(x => (x is EbDataField && !(x is IEbDataFieldSummary) && !(x is EbCalcField))).OrderBy(o => o.Top).Cast<EbDataField>().ToArray();
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
                        __reportFieldsSortedPerDetail[detail] = detail.Fields.OrderBy(o => o.Top).ToArray();
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
                        __reportFieldsSortedPerRFooter[_footer] = _footer.Fields.OrderBy(o => o.TopPt).ToArray();
                    }
                }

                return __reportFieldsSortedPerRFooter;
            }
        }

        public void AddNewPage()
        {
            detailprintingtop = 0;
            Doc.NewPage();
            ph_Yposition = this.Margin.Top;
            PageNumber = Writer.PageNumber;
        }

        public void DoLoopInDetail(int serialnumber)
        {
            int rowsneeded = 1;
            RowHeight = 0;
            MultiRowTop = 0;
            string column_val = string.Empty;

            ph_Yposition = (PageNumber == 1) ? ReportHeaderHeight + this.Margin.Top : this.Margin.Top;
            dt_Yposition = ph_Yposition + PageHeaderHeight;
            foreach (EbReportDetail detail in Detail)
            {
                EbDataField[] SortedList = FieldsNotSummaryPerDetail[detail];
                for (int iSortPos = 0; iSortPos < SortedList.Length; iSortPos++)
                {
                    EbDataField field = SortedList[iSortPos];
                    Phrase phrase;
                    column_val = GetDataFieldtValue(field.ColumnName, serialnumber, field.TableIndex);

                    if ((field as EbDataField).RenderInMultiLine)
                    {
                        DbType datatype = (DbType)field.DbType;
                        int val_length = column_val.Length;
                        phrase = new Phrase(column_val, field.GetItextFont(field.Font, this.Font));
                        float calculatedValueSize = phrase.Font.CalculatedSize * val_length;
                        if (calculatedValueSize > field.WidthPt)
                        {
                            rowsneeded = (datatype == DbType.Decimal) ? 1 : Convert.ToInt32(Math.Floor(calculatedValueSize / field.WidthPt));
                            if (rowsneeded > 1)
                            {
                                if (MultiRowTop == 0)
                                {
                                    MultiRowTop = field.TopPt;
                                }
                                float k = (phrase.Font.CalculatedSize) * (rowsneeded - 1);
                                if (k > RowHeight)
                                {
                                    RowHeight = k;
                                }
                            }
                        }
                    }
                }
                EbReportField[] SortedReportFields = this.ReportFieldsSortedPerDetail[detail];
                if (SortedReportFields.Length > 0)
                {
                    for (int iSortPos = 0; iSortPos < SortedReportFields.Length; iSortPos++)
                    {
                        EbReportField field = SortedReportFields[iSortPos];
                        if (field is EbDataField)
                            field.HeightPt += RowHeight;
                        DrawFields(field, dt_Yposition, serialnumber);
                    }
                    detailprintingtop += detail.HeightPt + RowHeight;
                    detailEnd = detailprintingtop;
                }
                else
                {
                    detailEnd = detailprintingtop;
                    IsLastpage = true;
                    Writer.PageEvent.OnEndPage(Writer, Doc);
                    return;
                }
            }
        }

        public void DrawGroupHeader(int order, int serialnumber)
        {
            if (PreviousGheadersSlNo != serialnumber && GroupHeaderHeight + DetailHeight > DT_FillHeight - detailprintingtop)
            {
                AddNewPage();
                dt_Yposition = PageHeaderHeight + this.Margin.Top;
            }

            foreach (EbReportField field in ReportGroups[order].GroupHeader.Fields)
            {
                DrawFields(field, dt_Yposition, serialnumber);
            }
            detailprintingtop += ReportGroups[order].GroupHeader.HeightPt;
            PreviousGheadersSlNo = serialnumber;
        }

        public void DrawPageFooter()
        {
            RowHeight = 0;
            MultiRowTop = 0;
            detailprintingtop = 0;
            ph_Yposition = (PageNumber == 1) ? ReportHeaderHeight + this.Margin.Top : this.Margin.Top;
            dt_Yposition = ph_Yposition + PageHeaderHeight;
            //  pf_Yposition = dt_Yposition + DT_FillHeight;
            pf_Yposition = (float)detailEnd + /*DetailHeight +*/ dt_Yposition;
            foreach (EbPageFooter p_footer in PageFooters)
            {
                foreach (EbReportField field in p_footer.Fields)
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
            detailprintingtop = 0;
            dt_Yposition = ph_Yposition + PageHeaderHeight;
            //pf_Yposition = dt_Yposition + DT_FillHeight;
            pf_Yposition = (float)detailEnd /*+ DetailHeight*/ + dt_Yposition;
            rf_Yposition = pf_Yposition + PageFooterHeight;

            foreach (EbReportFooter r_footer in ReportFooters)
            {
                float footer_diffrence = 0;
                EbReportField[] SortedReportFields = this.ReportFieldsSortedPerRFooter[r_footer];
                if (SortedReportFields.Length > 0)
                {
                    for (int iSortPos = 0; iSortPos < SortedReportFields.Length; iSortPos++)
                    {
                        EbReportField field = SortedReportFields[iSortPos];
                        // if (HeightPt - rf_Yposition + Margin.Top < field.TopPt)
                        if (HeightPt < field.TopPt + rf_Yposition + field.HeightPt + Margin.Bottom)
                        {
                            AddNewPage();
                            //footer_diffrence = HeightPt - rf_Yposition - Margin.Bottom;
                            footer_diffrence = field.TopPt;
                            FooterDrawn = true;
                            rf_Yposition = Margin.Top; 
                        } 
                            field.TopPt -= footer_diffrence;
                        DrawFields(field, rf_Yposition, 0);
                    }
                }
                rf_Yposition += r_footer.HeightPt;
            }
        }

        public void DrawFields(EbReportField field, float section_Yposition, int serialnumber)
        {
            List<Param> RowParams = null;
            if (field is EbDataField)
            {
                EbDataField field_org = field as EbDataField;
                if (PageSummaryFields.ContainsKey(field.Name) || ReportSummaryFields.ContainsKey(field.Name))
                    CallSummerize(field_org, serialnumber);
                if (AppearanceScriptCollection.ContainsKey(field.Name))
                    RunAppearanceExpression(field_org, serialnumber);
                if (!string.IsNullOrEmpty(field_org.LinkRefId))
                    RowParams = CreateRowParamForLink(field_org, serialnumber);
            }
            field.DrawMe(section_Yposition, this, RowParams, serialnumber);
        }

        public void RunAppearanceExpression(EbDataField field, int slno)
        {
            Globals globals = new Globals { CurrentField = field };
            AddParamsNCalcsInGlobal(globals);
            if (field.Font is null)
                globals.CurrentField.Font = (new EbFont { color = "#000000", FontName = "Times-Roman", Caps = false, Size = 10, Strikethrough = false, Style = 0, Underline = false });
            foreach (string calcfd in field.DataFieldsUsedAppearance)
            {
                string TName = calcfd.Split('.')[0];
                int TableIndex = Convert.ToInt32(TName.Substring(1));
                string fName = calcfd.Split('.')[1];
                globals[TName].Add(fName, new NTV { Name = fName, Type = DataSet.Tables[TableIndex].Columns[fName].Type, Value = DataSet.Tables[TableIndex].Rows[slno][fName] });
            }
            AppearanceScriptCollection[field.Name].RunAsync(globals);
        }

        public List<Param> CreateRowParamForLink(EbDataField field, int slno)
        {
            List<Param> RowParams = new List<Param>();
            foreach (EbControl control in LinkCollection[field.LinkRefId])
            {
                Param x = DataSet.Tables[field.TableIndex].Rows[slno].GetCellParam(control.Name);
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

        public void SetPassword()
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

        public void SetDetail()
        {
            string timestamp = String.Format("{0:" + CultureInfo.DateTimeFormat.FullDateTimePattern + "}", CurrentTimestamp);
            ColumnText ct = new ColumnText(Canvas);
            Phrase phrase = new Phrase("page:" + PageNumber.ToString() + ", " + RenderingUser.FullName + ", " + timestamp);
            phrase.Font.Size = 6;
            ct.SetSimpleColumn(phrase, 5, 2 + Margin.Bottom, (WidthPt - Margin.Right - Margin.Left) - Margin.Right, 20 + Margin.Bottom, 15, Element.ALIGN_RIGHT);
            ct.Go();
        }

        public override void AfterRedisGet(RedisClient Redis, IServiceClient client)
        {
            try
            {
                if (DataSourceRefId != string.Empty)
                {
                    EbDataSource = Redis.Get<EbDataReader>(DataSourceRefId);
                    if (EbDataSource == null || EbDataSource.Sql == null || EbDataSource.Sql == string.Empty)
                    {
                        EbObjectParticularVersionResponse result = client.Get(new EbObjectParticularVersionRequest { RefId = DataSourceRefId });
                        this.EbDataSource = EbSerializers.Json_Deserialize(result.Data[0].Json);
                        Redis.Set<EbDataReader>(DataSourceRefId, EbDataSource);
                    }
                    if (EbDataSource.FilterDialogRefId != string.Empty)
                        EbDataSource.AfterRedisGet(Redis, client);
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
                foreach (EbReportField field in dt.Fields)
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
                DataSourceRefId = "failed-to-update-";

            foreach (EbReportDetail dt in Detail)
            {
                foreach (EbReportField field in dt.Fields)
                {
                    if (field is EbDataField)
                    {
                        EbDataField fd_org = field as EbDataField;
                        if (!fd_org.LinkRefId.IsEmpty())
                        {
                            if (RefidMap.ContainsKey(fd_org.LinkRefId))
                                fd_org.LinkRefId = RefidMap[fd_org.LinkRefId];
                            else
                                fd_org.LinkRefId = "failed-to-update-";
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
            DownloadFileResponse dfs = null;

            byte[] fileByte = new byte[0];
            dfs = FileClient.Get
                 (new DownloadImageByIdRequest
                 {
                     ImageInfo = new ImageMeta
                     {
                         FileRefId = refId,
                         FileCategory =Common.Enums.EbFileCategory.Images
                     }
                 });
            if (dfs.StreamWrapper != null)
            {
                dfs.StreamWrapper.Memorystream.Position = 0;
                fileByte = dfs.StreamWrapper.Memorystream.ToBytes();
            }

            return fileByte;
        }

        public void AddParamsNCalcsInGlobal(Globals globals)
        {
            foreach (string key in CalcValInRow.Keys) //adding Calc to global
            {
                globals["Calc"].Add(key, CalcValInRow[key]);
            }
            if (Parameters != null)
                foreach (Param p in Parameters) //adding Params to global
                {
                    globals["Params"].Add(p.Name, new NTV { Name = p.Name, Type = (EbDbTypes)Convert.ToInt32(p.Type), Value = p.Value });
                }
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
    }

    [EnableInBuilder(BuilderType.Report)]
    public class EbReportHeader : EbReportSection
    {

        public override string GetDesignHtml()
        {
            return "<div class='pageHeaders' eb-type='ReportHeader' tabindex='1' id='@id' data_val='0' style='width :100%;height: @SectionHeight ; background-color:@BackColor ;position: relative'> </div>".RemoveCR().DoubleQuoted();
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
            return "<div class='pageHeaders' eb-type='PageHeader' tabindex='1' id='@id' data_val='1' style='width :100%;height: @SectionHeight ; background-color:@BackColor ;position: relative'> </div>".RemoveCR().DoubleQuoted();
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
            return "<div class='pageHeaders' eb-type='ReportDetail' tabindex='1' id='@id' data_val='2' style='width :100%;height: @SectionHeight ; background-color:@BackColor ;position: relative'> </div>".RemoveCR().DoubleQuoted();
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
            return "<div class='pageHeaders' eb-type='PageFooter' tabindex='1' id='@id' data_val='3' style='width :100%;height: @SectionHeight ; background-color:@BackColor ;position: relative'> </div>".RemoveCR().DoubleQuoted();
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
        public override string GetDesignHtml()
        {
            return "<div class='pageHeaders' eb-type='ReportFooter' tabindex='1' id='@id' data_val='4' style='width :100%;height: @SectionHeight ; background-color:@BackColor ;position: relative'> </div>".RemoveCR().DoubleQuoted();
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
