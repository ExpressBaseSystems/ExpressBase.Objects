using ExpressBase.Common;
using ExpressBase.Common.Data;
using ExpressBase.Common.EbServiceStack;
using ExpressBase.Common.Extensions;
using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using ExpressBase.Common.Structures;
using ExpressBase.Objects.Objects.ReportRelated;
using ExpressBase.Objects.ReportRelated;
using ExpressBase.Objects.ServiceStack_Artifacts;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.CodeAnalysis.Scripting;
using Newtonsoft.Json;
using ServiceStack;
using ServiceStack.Redis;
using System;
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
        left,
        center,
        right,
        justify
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
        public float Left { get; set; }

        public float Right { get; set; }

        public float Top { get; set; }

        public float Bottom { get; set; }

    }

    [EnableInBuilder(BuilderType.Report)]
    public class EbReport : EbReportObject
    {
        [EnableInBuilder(BuilderType.Report)]
        [OnChangeExec(@"
if (this.PaperSize === 6 ){  
     pg.ShowProperty('CustomPaperHeight');
     pg.ShowProperty('CustomPaperWidth');
}
else {
     pg.HideProperty('CustomPaperHeight');
     pg.HideProperty('CustomPaperWidth');
}
            ")]
        [PropertyGroup("General")]
        public PaperSize PaperSize { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        [PropertyEditor(PropertyEditorType.Expandable)]
        [PropertyGroup("Appearance")]
        public Margin Margin { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        [HideInPropertyGrid]
        public float DesignPageHeight { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        [PropertyGroup("General")]
        [UIproperty]
        public float CustomPaperHeight { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        [PropertyGroup("General")]
        [UIproperty]
        public float CustomPaperWidth { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        [PropertyGroup("General")]
        [UIproperty]
        public string UserPassword { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        [PropertyGroup("General")]
        [UIproperty]
        public string OwnerPassword { get; set; }

        //[HideInPropertyGrid]
        //[JsonIgnore]
        //public new string Description { get; set; }

        //[EnableInBuilder(BuilderType.Report)]
        //[HideInPropertyGrid]
        //public new string Left { get; set; }

        //[HideInPropertyGrid]
        //public new string Top { get; set; }

        //[HideInPropertyGrid]
        //public new string Height { get; set; }

        //[HideInPropertyGrid]
        //public new string Width { get; set; }

        //[HideInPropertyGrid]
        //public new string Title { get; set; }

        //[HideInPropertyGrid]
        //public new string ForeColor { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        [PropertyGroup("General")]
        public bool IsLandscape { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        [PropertyEditor(PropertyEditorType.ImageSeletor)]
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
        public EbBaseService FileService { get; set; }

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
        public Dictionary<string, object> FieldDict { get; set; }

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
            // return this.DataRows[i][column_name].ToString();
            return this.DataSet.Tables[tableIndex].Rows[i][column_name].ToString();
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
                    {
                        fileByte = WatermarkImages[(field as EbWaterMark).Image];
                    }
                (field as EbWaterMark).DrawMe(d, writer, fileByte, HeightPt);
                }
            }
        }

        public void CallSummerize(EbDataField field, int serialnumber)
        {
            //var column_name = string.Empty;
            var column_val = string.Empty;
            Globals globals = new Globals();
            globals.CurrentField = field;
            if (field is EbCalcField)
            {
                foreach (string calcfd in (field as EbCalcField).DataFieldsUsedCalc)
                {
                    string TName = calcfd.Split('.')[0];
                    string fName = calcfd.Split('.')[1];
                    int tableIndex = Convert.ToInt32(TName.Substring(1));
                    //globals[TName].Add(fName, new NTV { Name = fName, Type = this.DataRows.Table.Columns[fName].Type, Value = this.DataRows[serialnumber][fName] });
                    globals[TName].Add(fName, new NTV { Name = fName, Type = this.DataSet.Tables[0].Columns[fName].Type, Value = this.DataSet.Tables[0].Rows[serialnumber][fName] });
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
            int tableIndex = 0;
            foreach (EbReportDetail detail in Detail)
            {
                foreach (EbReportField field in detail.Fields)
                {
                    if (field is EbDataField)
                        tableIndex = (field as EbDataField).TableIndex;   //Detail[0].Fields[0] hack by Amal
                }
            }
            //var rows = (DataSourceRefId != string.Empty) ? DataRows: null;
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
                var SortedList = this.FieldsNotSummaryPerDetail[detail];
                for (int iSortPos = 0; iSortPos < SortedList.Length; iSortPos++)
                {
                    var field = SortedList[iSortPos];
                    int tableIndex = field.TableIndex;
                    var column_name = field.ColumnName;
                    Phrase phrase;
                    var column_val = GetDataFieldtValue(column_name, serialnumber, tableIndex);
                    if ((field as EbDataField).RenderInMultiLine)
                    {
                        var datatype = (DbType)field.DbType;
                        var val_length = column_val.Length;
                        if (field.Font == null)
                            phrase = new Phrase(column_val);
                        else
                            phrase = new Phrase(column_val, field.ITextFont);
                        var calculatedValueSize = phrase.Font.CalculatedSize * val_length;
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
                        //Space to add summary logic
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
            var column_name = string.Empty;
            var column_val = string.Empty;
            var column_type = DbType.String;

            if (PageSummaryFields.ContainsKey(field.Name) || ReportSummaryFields.ContainsKey(field.EbSid))
                CallSummerize(field as EbDataField, serialnumber);

            if (field is EbDataField)
            {
                column_type = (DbType)(field as EbDataField).DbType;
                int tableIndex = (field as EbDataField).TableIndex;
                column_name = (field as EbDataField).ColumnName;
                Globals globals = new Globals();
                globals.CurrentField = field;
                if (AppearanceScriptCollection.ContainsKey(field.Name))
                {

                    if (field.Font == null)
                    {
                        globals.CurrentField.Font = (new EbFont { color = "#000000", Font = "Courier", Caps = false, Size = 14, Strikethrough = false, Style = 0, Underline = false });
                    }
                    foreach (string calcfd in (field as EbDataField).DataFieldsUsedAppearance)
                    {
                        string TName = calcfd.Split('.')[0];
                        string fName = calcfd.Split('.')[1];
                        // globals[TName].Add(fName, new NTV { Name = fName, Type = this.DataRows.Table.Columns[fName].Type, Value = this.DataRows[serialnumber][fName] });
                        globals[TName].Add(fName, new NTV { Name = fName, Type = this.DataSet.Tables[0].Columns[fName].Type, Value = this.DataSet.Tables[0].Rows[serialnumber][fName] });
                    }
                    try
                    {
                        AppearanceScriptCollection[field.Name].RunAsync(globals);

                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
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
                            string fName = calcfd.Split('.')[1];
                            //globals[TName].Add(fName, new NTV { Name = fName, Type = this.DataRows.Table.Columns[fName].Type, Value = this.DataRows[serialnumber][fName] });
                            globals[TName].Add(fName, new NTV { Name = fName, Type = this.DataSet.Tables[0].Columns[fName].Type, Value = this.DataSet.Tables[0].Rows[serialnumber][fName] });
                        }
                        column_val = (ValueScriptCollection[field.Name].RunAsync(globals)).Result.ReturnValue.ToString();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }

                else
                    column_val = GetDataFieldtValue(column_name, serialnumber, tableIndex);

                field.DrawMe(Canvas, HeightPt, section_Yposition, column_val, detailprintingtop, column_type);
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
                byte[] fileByte = this.ReportService.GetFile(this.SolutionId, (field as EbImg).Image);
                field.DrawMe(Doc, fileByte);
            }
            else if ((field is EbText) || (field is EbReportFieldShape))
            {
                field.DrawMe(Canvas, HeightPt, section_Yposition, this);
            }
            else if (field is EbBarcode)
            {
                int tableIndex = Convert.ToInt32((field as EbBarcode).Code.Split('.')[0]);
                column_name = (field as EbBarcode).Code.Split('.')[1];
                column_val = GetDataFieldtValue(column_name, serialnumber, tableIndex);
                field.DrawMe(Doc, Canvas, HeightPt, section_Yposition, detailprintingtop, column_val);
            }
            else if (field is EbQRcode)
            {
                int tableIndex = Convert.ToInt32((field as EbQRcode).Code.Split('.')[0]);
                column_name = (field as EbQRcode).Code.Split('.')[1];
                column_val = GetDataFieldtValue(column_name, serialnumber, tableIndex);
                field.DrawMe(Doc, Canvas, HeightPt, section_Yposition, detailprintingtop, column_val);
            }
        }

        public void GetWatermarkImages()
        {
            if (this.ReportObjects != null)
            {
                foreach (var field in this.ReportObjects)
                {
                    if ((field is EbWaterMark) && (field as EbWaterMark).Image != string.Empty)
                    {
                        byte[] fileByte = this.ReportService.GetFile(this.SolutionId, (field as EbWaterMark).Image);
                        //  byte[] fileByte = myFileService.Post
                        //(new DownloadFileRequest
                        //{
                        //    FileDetails = new FileMeta
                        //    {
                        //        FileName = (field as EbWaterMark).Image + ".jpg",
                        //        FileType = "jpg"
                        //    }
                        //});
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

        public override void AfterRedisGet(RedisClient Redis, IServiceClient client)
        {
            try
            {
                this.EbDataSource = Redis.Get<EbDataSource>(DataSourceRefId);
                if (this.EbDataSource == null || this.EbDataSource.Sql == null || this.EbDataSource.Sql == string.Empty)
                {
                    var result = client.Get<EbObjectParticularVersionResponse>(new EbObjectParticularVersionRequest { RefId = this.DataSourceRefId });
                    this.EbDataSource = EbSerializers.Json_Deserialize(result.Data[0].Json);
                    Redis.Set<EbDataSource>(DataSourceRefId, this.EbDataSource);
                }
                if (this.EbDataSource.FilterDialogRefId != string.Empty)
                    this.EbDataSource.AfterRedisGet(Redis, client);
            }
            catch (Exception e)
            {

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
    }

    public class EbReportSection : EbReportObject
    {
        [EnableInBuilder(BuilderType.Report)]
        [UIproperty]
        //[HideInPropertyGrid]
        public string SectionHeight { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        [HideInPropertyGrid]
        public List<EbReportField> Fields { get; set; }

        //[EnableInBuilder(BuilderType.Report)]
        //[HideInPropertyGrid]
        //public new string Left { get; set; }

        //[EnableInBuilder(BuilderType.Report)]
        //[HideInPropertyGrid]
        //public new string Top { get; set; }

        //[EnableInBuilder(BuilderType.Report)]
        //[UIproperty]
        //[HideInPropertyGrid]
        //public new string Height { get; set; }

        //[EnableInBuilder(BuilderType.Report)]
        //[UIproperty]
        //[HideInPropertyGrid]
        //public new string Width { get; set; }

        //[EnableInBuilder(BuilderType.Report)]
        //[UIproperty]
        //[HideInPropertyGrid]
        //public new string Title { get; set; }

        //[HideInPropertyGrid]
        //[JsonIgnore]
        //public new string Description { get; set; }

        //[HideInPropertyGrid]
        //public new string ForeColor { get; set; }

    }

    [EnableInBuilder(BuilderType.Report)]
    public class EbReportHeader : EbReportSection
    {

        public override string GetDesignHtml()
        {
            return "<div class='pageHeaders' eb-type='ReportHeader' tabindex='1' id='@id' data_val='0' style='width :100%;height: @SectionHeight ; background-color:@BackColor ;position: relative'> </div>".RemoveCR().DoubleQuoted();
        }
    }

    [EnableInBuilder(BuilderType.Report)]
    public class EbPageHeader : EbReportSection
    {
        public override string GetDesignHtml()
        {
            return "<div class='pageHeaders' eb-type='PageHeader' tabindex='1' id='@id' data_val='1' style='width :100%;height: @SectionHeight ; background-color:@BackColor ;position: relative'> </div>".RemoveCR().DoubleQuoted();
        }
    }

    [EnableInBuilder(BuilderType.Report)]
    public class EbReportDetail : EbReportSection
    {
        public override string GetDesignHtml()
        {
            return "<div class='pageHeaders' eb-type='ReportDetail' tabindex='1' id='@id' data_val='2' style='width :100%;height: @SectionHeight ; background-color:@BackColor ;position: relative'> </div>".RemoveCR().DoubleQuoted();
        }

    }

    [EnableInBuilder(BuilderType.Report)]
    public class EbPageFooter : EbReportSection
    {
        public override string GetDesignHtml()
        {
            return "<div class='pageHeaders' eb-type='PageFooter' tabindex='1' id='@id' data_val='3' style='width :100%;height: @SectionHeight ; background-color:@BackColor ;position: relative'> </div>".RemoveCR().DoubleQuoted();
        }
    }

    [EnableInBuilder(BuilderType.Report)]
    public class EbReportFooter : EbReportSection
    {
        public override string GetDesignHtml()
        {
            return "<div class='pageHeaders' eb-type='ReportFooter' tabindex='1' id='@id' data_val='4' style='width :100%;height: @SectionHeight ; background-color:@BackColor ;position: relative'> </div>".RemoveCR().DoubleQuoted();
        }
    }
}
