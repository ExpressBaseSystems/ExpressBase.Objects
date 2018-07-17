using ExpressBase.Common;
using ExpressBase.Common.Constants;
using ExpressBase.Common.Data;
using ExpressBase.Common.EbServiceStack;
using ExpressBase.Common.EbServiceStack.ReqNRes;
using ExpressBase.Common.Extensions;
using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using ExpressBase.Common.ServiceClients;
using ExpressBase.Common.Structures;
using ExpressBase.Objects.Objects;
using ExpressBase.Objects.ReportRelated;
using ExpressBase.Objects.ServiceStack_Artifacts;
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
using System.Data;
using System.Drawing;
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
        MM_dd_yy
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
        [PropertyEditor(PropertyEditorType.Number)]
        public float Left { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        [PropertyEditor(PropertyEditorType.Number)]
        public float Right { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        [PropertyEditor(PropertyEditorType.Number)]
        public float Top { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        [PropertyEditor(PropertyEditorType.Number)]
        public float Bottom { get; set; }

    }

    [EnableInBuilder(BuilderType.Report)]
    public class EbReport : EbReportObject
    {
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
        public override string Left { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        [HideInPropertyGrid]
        public override string Top { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        [HideInPropertyGrid]
        public override string Height { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        [HideInPropertyGrid]
        public override string ForeColor { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        [HideInPropertyGrid]
        public override string Title { get; set; }

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

        [JsonIgnore]
        public EbDataSource EbDataSource { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        [PropertyEditor(PropertyEditorType.ObjectSelector)]
        [OSE_ObjectTypes(EbObjectTypes.iDataSource)]
        [PropertyGroup("Data")]
        public string DataSourceRefId { get; set; }

        //[JsonIgnore]
        //public ColumnColletion ColumnColletion { get; set; }

        [JsonIgnore]
        public int iDetailRowPos { get; set; }

        [JsonIgnore]
        public Dictionary<string, List<object>> PageSummaryFields { get; set; }

        [JsonIgnore]
        public Dictionary<string, List<object>> ReportSummaryFields { get; set; }

        [JsonIgnore]
        public Dictionary<string, byte[]> WatermarkImages { get; set; }

        [JsonIgnore]
        public List<object> WaterMarkList { get; set; }

        [JsonIgnore]
        public Dictionary<string, Script> ValueScriptCollection { get; set; }
        [JsonIgnore]
        public Dictionary<string, Script> AppearanceScriptCollection { get; set; }

        [JsonIgnore]
        public EbDataSet DataSet { get; set; }

        //[JsonIgnore]
        //public RowColletion DataRows { get; set; }

        //[JsonIgnore]
        //public ColumnColletion DataColumns { get; set; }

        [JsonIgnore]
        public bool IsLastpage { get; set; }

        [JsonIgnore]
        public int PageNumber { get; set; }

        [JsonIgnore]
        public DateTime CurrentTimestamp { get; set; }

        [JsonIgnore]
        public string UserName { get; set; }

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

        private float dt_fillheight = 0;
        [JsonIgnore]
        public float DT_FillHeight
        {
            get
            {
                var rows = (DataSourceRefId != string.Empty) ? DataSet.Tables[0].Rows : null;
                //  var rows = (DataSourceRefId != string.Empty) ? DataRows : null;
                if (rows != null)
                {
                    if (rows.Count > 0)
                    {
                        var a = rows.Count * DetailHeight;
                        var b = HeightPt - (PageHeaderHeight + PageFooterHeight + ReportHeaderHeight + ReportFooterHeight);
                        if (a < b && PageNumber == 1)
                            IsLastpage = true;
                    }
                }

                if (PageNumber == 1 && IsLastpage)
                    dt_fillheight = HeightPt - (PageHeaderHeight + PageFooterHeight + ReportHeaderHeight + ReportFooterHeight);
                else if (PageNumber == 1)
                    dt_fillheight = HeightPt - (ReportHeaderHeight + PageHeaderHeight + PageFooterHeight);
                else if (IsLastpage == true)
                    dt_fillheight = HeightPt - (PageHeaderHeight + PageFooterHeight + ReportFooterHeight);
                else
                    dt_fillheight = HeightPt - (PageHeaderHeight + PageFooterHeight);
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

        //[JsonIgnore]
        //public Dictionary<string, object> FieldDict { get; set; }

        [JsonIgnore]
        public Dictionary<string, List<EbControl>> LinkCollection { get; set; }

        [JsonIgnore]
        public Dictionary<string, NTV> CalcValInRow { get; set; } = new Dictionary<string, NTV>();

        public void InitializeSummaryFields()
        {
            List<object> SummaryFieldsList = null;
            PageSummaryFields = new Dictionary<string, List<object>>();
            ReportSummaryFields = new Dictionary<string, List<object>>();
            foreach (EbPageFooter p_footer in PageFooters)
            {
                foreach (EbReportField field in p_footer.Fields)
                {
                    if (field is IEbDataFieldSummary)
                    {
                        //string fname = "";
                        //if (field is EbCalcField)
                        //    fname = (field as EbCalcField).ValueExpression;
                        //else
                        //    fname = (field as EbDataField).DataField;
                        EbDataField f = (field as EbDataField);
                        if (!PageSummaryFields.ContainsKey(f.SummaryOf))
                        {
                            SummaryFieldsList = new List<object>();
                            SummaryFieldsList.Add(f);
                            PageSummaryFields.Add(f.SummaryOf, SummaryFieldsList);
                        }
                        else
                        {
                            PageSummaryFields[f.SummaryOf].Add(f);
                        }
                    }
                }
            }

            foreach (EbReportFooter r_footer in ReportFooters)
            {
                foreach (EbReportField field in r_footer.Fields)
                {
                    if (field is IEbDataFieldSummary)
                    {
                        //string fname = "";
                        //if (field is EbCalcField)
                        //    fname = (field as EbCalcField).ValueExpression;
                        //else
                        //    fname = (field as EbDataField).DataField;
                        EbDataField f = (field as EbDataField);
                        if (!ReportSummaryFields.ContainsKey(f.SummaryOf))
                        {
                            SummaryFieldsList = new List<object>();
                            SummaryFieldsList.Add(f);
                            ReportSummaryFields.Add(f.SummaryOf, SummaryFieldsList);
                        }
                        else
                        {
                            ReportSummaryFields[f.SummaryOf].Add(f);
                        }
                    }
                }
            }

        }

        public string GetDataFieldtValue(string column_name, int i, int tableIndex)
        {
            if (this.DataSet.Tables[tableIndex].Rows.Count > 1)
                return this.DataSet.Tables[tableIndex].Rows[i][column_name].ToString();
            else
                return this.DataSet.Tables[tableIndex].Rows[0][column_name].ToString();
        }

        //public DbType GetFieldtDataType(string column_name)
        //{
        //    return (DbType)this.DataRow.Table.Columns[column_name].Type;
        //    //return this.DataSet.Tables[0].Rows[i][column_name].GetType().ToString();
        //}

        public void DrawWaterMark(Document d, PdfWriter writer)
        {
            byte[] fileByte = null;
            if (ReportObjects != null)
            {
                foreach (var field in ReportObjects)
                {
                    if ((field as EbWaterMark).Image != string.Empty)
                        fileByte = WatermarkImages[(field as EbWaterMark).Image];
                    (field as EbWaterMark).DrawMe(d, writer, fileByte, HeightPt);
                }
            }
        }

        public void CallSummerize(EbDataField field, int serialnumber)
        {
            //var column_name = string.Empty;
            var column_val = string.Empty;
            Globals globals = new Globals
            {
                CurrentField = field
            };
            AddParamsNCalcsInGlobal(globals);
            if (field is EbCalcField)
            {
                foreach (string calcfd in (field as EbCalcField).DataFieldsUsedCalc)
                {
                    string TName = calcfd.Split('.')[0];
                    int TableIndex = Convert.ToInt32(TName.Substring(1));
                    string fName = calcfd.Split('.')[1];
                    int tableIndex = Convert.ToInt32(TName.Substring(1));
                    //globals[TName].Add(fName, new NTV { Name = fName, Type = this.DataRows.Table.Columns[fName].Type, Value = this.DataRows[serialnumber][fName] });
                    globals[TName].Add(fName, new NTV { Name = fName, Type = this.DataSet.Tables[TableIndex].Columns[fName].Type, Value = this.DataSet.Tables[TableIndex].Rows[serialnumber][fName] });
                }
                column_val = (ValueScriptCollection[(field as EbCalcField).Name].RunAsync(globals)).Result.ReturnValue.ToString();
            }
            else
            {
                //int tableIndex = Convert.ToInt32((field as EbDataField).SummaryOf.Substring(1, 1));
                //column_name = (field as EbDataField).SummaryOf.Split('.')[1];

                column_val = GetDataFieldtValue(field.ColumnName, serialnumber, field.TableIndex);
            }
            List<object> SummaryList;
            if (PageSummaryFields.ContainsKey(field.Name))
            {
                SummaryList = PageSummaryFields[field.Name];
                foreach (var item in SummaryList)
                {
                    //int tableIndex = Convert.ToInt32(title.Split('.')[0]);
                    //int tableIndex = Convert.ToInt32(Title.Substring(1, 1));
                    //column_name = Title.Split('.')[1];
                    //column_val = GetDataFieldtValue(column_name, i, tableIndex);
                    (item as IEbDataFieldSummary).Summarize(column_val);
                }
            }
            if (ReportSummaryFields.ContainsKey(field.Name))
            {
                SummaryList = ReportSummaryFields[field.Name];
                foreach (var item in SummaryList)
                {
                    //int tableIndex = Convert.ToInt32(title.Split('.')[0]);
                    //int tableIndex = Convert.ToInt32(title.Substring(1, 1));
                    //column_name = title.Split('.')[1];
                    //column_val = GetDataFieldtValue(column_name, i, tableIndex);
                    (item as IEbDataFieldSummary).Summarize(column_val);
                }
            }

        }

        public void DrawReportHeader()
        {
            RowHeight = 0;
            MultiRowTop = 0;
            rh_Yposition = 0;
            detailprintingtop = 0;
            foreach (EbReportHeader r_header in this.ReportHeaders)
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
            ph_Yposition = (PageNumber == 1) ? ReportHeaderHeight : 0;
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
            List<int> tableindexes = new List<int>();

            int tableIndex = 0;
            int maxRowCount = 0;

            foreach (EbReportDetail _detail in Detail)
            {
                foreach (EbReportField field in _detail.Fields)
                {
                    if (field is EbDataField && !tableindexes.Contains((field as EbDataField).TableIndex))
                    {
                        int r_count = DataSet.Tables[(field as EbDataField).TableIndex].Rows.Count;
                        tableIndex = (r_count > maxRowCount) ? (field as EbDataField).TableIndex : tableIndex;
                        maxRowCount = (r_count > maxRowCount) ? r_count : maxRowCount;
                    }
                }
            }
            var rows = (DataSourceRefId != string.Empty) ? DataSet.Tables[tableIndex].Rows : null;
            if (rows != null)
            {
                for (iDetailRowPos = 0; iDetailRowPos < rows.Count; iDetailRowPos++)
                {
                    if (detailprintingtop < DT_FillHeight && DT_FillHeight - detailprintingtop >= DetailHeight)
                    {
                        DoLoopInDetail(iDetailRowPos);
                    }
                    else
                    {
                        detailprintingtop = 0;
                        Doc.NewPage();
                        PageNumber = Writer.PageNumber;
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

        public void DoLoopInDetail(int serialnumber)
        {
            int rowsneeded = 1;
            RowHeight = 0;
            MultiRowTop = 0;

            ph_Yposition = (PageNumber == 1) ? ReportHeaderHeight : 0;
            dt_Yposition = ph_Yposition + PageHeaderHeight;

            foreach (EbReportDetail detail in Detail)
            {
                EbDataField[] SortedList = this.FieldsNotSummaryPerDetail[detail];
                for (int iSortPos = 0; iSortPos < SortedList.Length; iSortPos++)
                {
                    EbDataField field = SortedList[iSortPos];
                    int tableIndex = field.TableIndex;
                    string column_name = field.ColumnName;
                    Phrase phrase;
                    string column_val = GetDataFieldtValue(column_name, serialnumber, tableIndex);
                    if ((field as EbDataField).RenderInMultiLine)
                    {
                        DbType datatype = (DbType)field.DbType;
                        int val_length = column_val.Length;
                        if (field.Font == null)
                            field.Font = (new EbFont { color = "#000000", Font = "Courier", Caps = false, Size = 10, Strikethrough = false, Style = 0, Underline = false });
                        phrase = new Phrase(column_val, field.ITextFont);
                        float calculatedValueSize = phrase.Font.CalculatedSize * val_length;
                        if (calculatedValueSize > field.WidthPt)
                        {
                            rowsneeded = (datatype == DbType.Decimal) ? 1 : Convert.ToInt32(Math.Floor(calculatedValueSize / field.WidthPt));

                            if (MultiRowTop == 0)
                            {
                                MultiRowTop = field.TopPt;
                            }
                            float k = (phrase.Font.CalculatedSize) * rowsneeded;
                            if (k > RowHeight)
                            {
                                RowHeight = k;
                            }
                        }
                    }
                }
                var SortedReportFields = this.ReportFieldsSortedPerDetail[detail];
                if (SortedReportFields.Length > 0)
                {
                    for (int iSortPos = 0; iSortPos < SortedReportFields.Length; iSortPos++)
                    {
                        var field = SortedReportFields[iSortPos];
                        field.HeightPt += RowHeight;
                        DrawFields(field, dt_Yposition, serialnumber);
                    }
                    detailprintingtop += detail.HeightPt + RowHeight;
                }
                else
                {
                    IsLastpage = true;
                    this.Writer.PageEvent.OnEndPage(Writer, Doc);
                    return;
                }
            }
        }

        public void DrawPageFooter()
        {
            RowHeight = 0;
            MultiRowTop = 0;
            detailprintingtop = 0;
            ph_Yposition = (PageNumber == 1) ? ReportHeaderHeight : 0;
            dt_Yposition = ph_Yposition + PageHeaderHeight;
            pf_Yposition = dt_Yposition + DT_FillHeight;
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
            pf_Yposition = dt_Yposition + DT_FillHeight;
            rf_Yposition = pf_Yposition + PageFooterHeight;
            foreach (EbReportFooter r_footer in ReportFooters)
            {
                foreach (EbReportField field in r_footer.Fields)
                {
                    DrawFields(field, rf_Yposition, 0);
                }
                rf_Yposition += r_footer.HeightPt;
            }
        }

        public void DrawFields(EbReportField field, float section_Yposition, int serialnumber)
        {
            List<Param> RowParams = new List<Param>();
            try
            {
                string column_name = string.Empty;
                string column_val = string.Empty;
                DbType column_type = DbType.String;

                if (PageSummaryFields.ContainsKey(field.Name) || ReportSummaryFields.ContainsKey(field.EbSid))
                    CallSummerize(field as EbDataField, serialnumber);

                if (field is EbDataField)
                {
                    EbDataField _field = field as EbDataField;
                    column_type = (DbType)_field.DbType;
                    int tableIndex = _field.TableIndex;
                    column_name = _field.ColumnName;
                    Globals globals = new Globals
                    {
                        CurrentField = field
                    };
                    if (AppearanceScriptCollection.ContainsKey(field.Name) || field is EbCalcField)
                    {
                        AddParamsNCalcsInGlobal(globals);
                    }
                    if (AppearanceScriptCollection.ContainsKey(field.Name))
                    {

                        if (field.Font is null)
                        {
                            globals.CurrentField.Font = (new EbFont { color = "#000000", Font = "Courier", Caps = false, Size = 10, Strikethrough = false, Style = 0, Underline = false });
                        }
                        foreach (string calcfd in (field as EbDataField).DataFieldsUsedAppearance)
                        {
                            string TName = calcfd.Split('.')[0];
                            int TableIndex = Convert.ToInt32(TName.Substring(1));
                            string fName = calcfd.Split('.')[1];
                            globals[TName].Add(fName, new NTV { Name = fName, Type = this.DataSet.Tables[TableIndex].Columns[fName].Type, Value = this.DataSet.Tables[TableIndex].Rows[serialnumber][fName] });
                        }
                        try
                        {
                            AppearanceScriptCollection[field.Name].RunAsync(globals);

                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message + e.StackTrace);
                        }
                    }
                    if (field is IEbDataFieldSummary)
                    {
                        if ((field is EbDataFieldNumericSummary) && (field as EbDataFieldNumericSummary).InLetters)
                        {
                            column_val = (field as IEbDataFieldSummary).SummarizedValue.ToString();
                            (field as EbDataFieldNumericSummary).DrawMe(Canvas, HeightPt, section_Yposition, detailprintingtop, column_val);
                            return;
                        }
                        else
                            column_val = (field as IEbDataFieldSummary).SummarizedValue.ToString();
                    }
                    else if (field is EbCalcField)
                    {
                        try
                        {
                            foreach (string calcfd in (field as EbCalcField).DataFieldsUsedCalc)
                            {
                                string TName = calcfd.Split('.')[0];
                                int TableIndex = Convert.ToInt32(TName.Substring(1));
                                string fName = calcfd.Split('.')[1];
                                globals[TName].Add(fName, new NTV { Name = fName, Type = this.DataSet.Tables[TableIndex].Columns[fName].Type, Value = this.DataSet.Tables[TableIndex].Rows[serialnumber][fName] });
                            }
                            column_val = (ValueScriptCollection[field.Name].RunAsync(globals)).Result.ReturnValue.ToString();
                            if (CalcValInRow.ContainsKey(field.Title))
                                CalcValInRow[field.Title] = new NTV { Name = field.Title, Type = (EbDbTypes)((field as EbCalcField).CalcFieldIntType), Value = column_val };
                            else
                                CalcValInRow.Add(field.Title, new NTV { Name = field.Title, Type = (EbDbTypes)((field as EbCalcField).CalcFieldIntType), Value = column_val });
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message + e.StackTrace);
                        }
                    }

                    else
                        column_val = GetDataFieldtValue(column_name, serialnumber, tableIndex);

                    if (!string.IsNullOrEmpty(_field.LinkRefid))
                    {
                        foreach (EbControl control in LinkCollection[(field as EbDataField).LinkRefid])
                        {
                            int flag = 0;
                            foreach (Param param in this.Parameters)
                            {
                                if (control.Name == param.Name)
                                {
                                    flag = 1;
                                }
                            }
                            if (flag == 0)
                            {
                                Param x = this.DataSet.Tables[tableIndex].Rows[serialnumber].GetCellParam(control.Name);
                                ArrayList IndexToRemove = new ArrayList();
                                for (int i = 0; i < RowParams.Count; i++)
                                {
                                    if (RowParams[i].Name == control.Name)
                                    {
                                        IndexToRemove.Add(i);
                                    }
                                }
                                for (int i = 0; i < IndexToRemove.Count; i++)
                                {
                                    RowParams.RemoveAt(Convert.ToInt32(IndexToRemove[i]));
                                }
                                RowParams.Add(x);
                            }
                        }
                        if (!this.Parameters.IsEmpty())
                        {
                            foreach (Param p in this.Parameters)
                            {
                                RowParams.Add(p);
                            }
                        }
                    }
                    field.DrawMe(Doc, Canvas, HeightPt, section_Yposition, column_val, detailprintingtop, column_type, RowParams);
                }

                if ((field is EbPageNo) || (field is EbPageXY) || (field is EbDateTime) || (field is EbSerialNumber) || (field is EbUserName) || (field is EbParameter))
                {
                    if (field is EbPageNo)
                        column_val = PageNumber.ToString();
                    else if (field is EbPageXY)
                        column_val = PageNumber + "/"/* + writer.PageCount*/;
                    else if (field is EbDateTime)
                        column_val = CurrentTimestamp.ToString();
                    else if (field is EbSerialNumber)
                        column_val = (iDetailRowPos + 1).ToString();
                    else if (field is EbUserName)
                        column_val = this.UserName;
                    else if (field is EbParameter)
                    {
                        foreach (Param p in Parameters)
                            if (p.Name == field.Title)
                                column_val = p.Value;
                    }
                    field.DrawMe(Canvas, HeightPt, section_Yposition, detailprintingtop, column_val);
                }

                else if (field is EbImg)
                {
                    byte[] fileByte = GetFile((field as EbImg).Image);
                    if (fileByte != null)
                        field.DrawMe(Doc, fileByte, HeightPt, section_Yposition, detailprintingtop);
                }

                else if ((field is EbText) || (field is EbReportFieldShape))
                {
                    field.DrawMe(Canvas, HeightPt, section_Yposition, this);
                }

                else if (field is EbBarcode)
                {
                    int tableIndex = Convert.ToInt32((field as EbBarcode).Code.Split('.')[0].Substring(1));
                    column_name = (field as EbBarcode).Code.Split('.')[1];
                    column_val = GetDataFieldtValue(column_name, serialnumber, tableIndex);
                    field.DrawMe(Doc, Canvas, HeightPt, section_Yposition, detailprintingtop, column_val);
                }

                else if (field is EbQRcode)
                {
                    int tableIndex = Convert.ToInt32((field as EbQRcode).Code.Split('.')[0].Substring(1));
                    column_name = (field as EbQRcode).Code.Split('.')[1];
                    column_val = GetDataFieldtValue(column_name, serialnumber, tableIndex);
                    field.DrawMe(Doc, Canvas, HeightPt, section_Yposition, detailprintingtop, column_val);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message + e.StackTrace);
            }
        }

        public void GetWatermarkImages()
        {
            if (this.ReportObjects != null)
            {
                foreach (EbReportField field in this.ReportObjects)
                {
                    if ((field is EbWaterMark) && (field as EbWaterMark).Image != string.Empty)
                    {
                        byte[] fileByte = GetFile((field as EbWaterMark).Image);
                        WatermarkImages.Add((field as EbWaterMark).Image, fileByte);
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
            ColumnText ct = new ColumnText(Canvas);
            Phrase phrase = new Phrase("page:" + PageNumber.ToString() + ", " + UserName + ", " + CurrentTimestamp);
            phrase.Font.Size = 6;
            ct.SetSimpleColumn(phrase, 5, 2, WidthPt - 10, 20, 15, Element.ALIGN_RIGHT);
            ct.Go();
        }

        public override void AfterRedisGet(RedisClient Redis, IServiceClient client)
        {
            try
            {
                if (DataSourceRefId != string.Empty)
                {
                    this.EbDataSource = Redis.Get<EbDataSource>(DataSourceRefId);
                    if (this.EbDataSource == null || this.EbDataSource.Sql == null || this.EbDataSource.Sql == string.Empty)
                    {
                        EbObjectParticularVersionResponse result = client.Get(new EbObjectParticularVersionRequest { RefId = this.DataSourceRefId });
                        this.EbDataSource = EbSerializers.Json_Deserialize(result.Data[0].Json);
                        Redis.Set<EbDataSource>(DataSourceRefId, this.EbDataSource);
                    }
                    if (this.EbDataSource.FilterDialogRefId != string.Empty)
                        this.EbDataSource.AfterRedisGet(Redis, client);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.ToString());
            }
        }

        public EbReport()
        {
            this.ReportHeaders = new List<EbReportHeader>();

            this.PageHeaders = new List<EbPageHeader>();

            this.Detail = new List<EbReportDetail>();

            this.PageFooters = new List<EbPageFooter>();

            this.ReportFooters = new List<EbReportFooter>();
        }

        public static EbOperations Operations = ReportOperations.Instance;

        public byte[] GetFile(string Image)
        {
            DownloadFileResponse dfs = null;

            byte[] fileByte = new byte[0];
            dfs = this.FileClient.Get
                 (new DownloadFileRequest
                 {
                     TenantAccountId = this.SolutionId,
                     FileDetails = new FileMeta
                     {
                         FileName = Image + StaticFileConstants.DOTJPG,
                         FileType = StaticFileConstants.JPG
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
            foreach (string key in CalcValInRow.Keys)//adding Calc to global
            {
                globals["Calc"].Add(key, CalcValInRow[key]);
            }
            if (Parameters != null)
                foreach (Param p in Parameters) //adding Params to global
                {
                    globals["Params"].Add(p.Name, new NTV { Name = p.Name, Type = (EbDbTypes)Convert.ToInt32(p.Type), Value = p.Value });
                }
        }
        public void FillingCollections()
        {
            foreach (EbReportHeader r_header in ReportHeaders)
            {
                FillScriptCollection(r_header.Fields);
               // FillFieldDict(r_header.Fields);
               // FillLinkCollection(Report, r_header.Fields);
            }

            foreach (EbReportFooter r_footer in ReportFooters)
            {
                FillScriptCollection(r_footer.Fields);
                //FillFieldDict(r_footer.Fields);
               // FillLinkCollection(Report, r_footer.Fields);
            }

            foreach (EbPageHeader p_header in PageHeaders)
            {
                FillScriptCollection( p_header.Fields);
               // FillFieldDict(p_header.Fields);
               // FillLinkCollection(Report, p_header.Fields);
            }

            foreach (EbReportDetail detail in Detail)
            {
                FillScriptCollection( detail.Fields);
               // FillFieldDict(detail.Fields);
              // FillLinkCollection(Report, detail.Fields);
            }

            foreach (EbPageFooter p_footer in PageFooters)
            {
                FillScriptCollection( p_footer.Fields);
               // FillFieldDict(p_footer.Fields);
               // FillLinkCollection(Report, p_footer.Fields);
            }
        }
        private void FillScriptCollection(List<EbReportField> fields)
        {
            foreach (EbReportField field in fields)
            {
                try
                {
                    if (field is EbCalcField && !ValueScriptCollection.ContainsKey(field.Name))
                    {
                        Script valscript = CSharpScript.Create<dynamic>((field as EbCalcField).ValueExpression, ScriptOptions.Default.WithReferences("Microsoft.CSharp", "System.Core").WithImports("System.Dynamic"), globalsType: typeof(Globals));
                        valscript.Compile();
                        ValueScriptCollection.Add(field.Name, valscript);

                    }
                    if ((field is EbDataField && !AppearanceScriptCollection.ContainsKey(field.Name) && (field as EbDataField).AppearanceExpression != ""))
                    {
                        Script appearscript = Microsoft.CodeAnalysis.CSharp.Scripting.CSharpScript.Create<dynamic>((field as EbDataField).AppearanceExpression, ScriptOptions.Default.WithReferences("Microsoft.CSharp", "System.Core").WithImports("System.Dynamic"), globalsType: typeof(Globals));
                        appearscript.Compile();
                        AppearanceScriptCollection.Add(field.Name, appearscript);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message + e.StackTrace);
                }
            }
        }
        //private void FillFieldDict(List<EbReportField> fields)
        //{
        //    foreach (EbReportField field in fields)
        //    {
        //        //FieldDict.Add(field.Name, field);
        //    }
        //}    
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

    public class EbReportSection : EbReportObject
    {
        [EnableInBuilder(BuilderType.Report)]
        [UIproperty]
        [MetaOnly]
        public string SectionHeight { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        [HideInPropertyGrid]
        public List<EbReportField> Fields { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        [HideInPropertyGrid]
        public override string Left { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        [HideInPropertyGrid]
        public override string Top { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        [HideInPropertyGrid]
        public override string Height { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        [HideInPropertyGrid]
        public override string Width { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        [HideInPropertyGrid]
        public override string Title { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        [HideInPropertyGrid]
        public override string ForeColor { get; set; }

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
}
