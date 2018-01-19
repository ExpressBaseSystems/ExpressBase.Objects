using ExpressBase.Common;
using ExpressBase.Common.Extensions;
using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using ExpressBase.Objects.ReportRelated;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
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
        [PropertyGroup("General")]
        [UIproperty]
        public float CustomPaperHeight { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        [PropertyGroup("General")]
        [UIproperty]
        public float CustomPaperWidth { get; set; }

        //public EbReportMargins Margins { get; set; }
        //[EnableInBuilder(BuilderType.Report)]
        //[PropertyGroup("General")]
        //public string ReportName { get; set; }

        //[EnableInBuilder(BuilderType.Report)]
        //[PropertyGroup("General")]
        //public string Description { get; set; }
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

        [EnableInBuilder(BuilderType.Report)]
        [PropertyEditor(PropertyEditorType.ObjectSelector)]
        [OSE_ObjectTypes(EbObjectType.DataSource)]
        public string DataSourceRefId { get; set; }

        public ColumnColletion ColumnColletion { get; set; }

        public int SerialNumber { get; set; } = 1;

        public Dictionary<string, List<object>> PageSummaryFields { get; set; }

        public Dictionary<string, List<object>> ReportSummaryFields { get; set; }

        public Dictionary<string, byte[]> watermarkImages { get; set; }

        public List<object> WaterMarkList { get; set; }

        public RowColletion DataRow { get; set; }

        public ColumnColletion DataColumns { get; set; }

        public bool IsLastpage { get; set; }

        public int PageNumber { get; set; }

        private float _rhHeight = 0;
        public float ReportHeaderHeight
        {
            get
            {
                if (_rhHeight == 0)
                {
                    foreach (EbReportHeader r_header in ReportHeaders)
                        _rhHeight += r_header.Height;
                }

                return _rhHeight;
            }
        }

        private float _phHeight = 0;
        public float PageHeaderHeight
        {
            get
            {
                if (_phHeight == 0)
                {
                    foreach (EbPageHeader p_header in PageHeaders)
                        _phHeight += p_header.Height;
                }
                return _phHeight;
            }
        }

        private float _pfHeight = 0;
        public float PageFooterHeight
        {
            get
            {
                if (_pfHeight == 0)
                {

                    foreach (EbPageFooter p_footer in PageFooters)
                        _pfHeight += p_footer.Height;
                }
                return _pfHeight;
            }
        }

        private float _rfHeight = 0;
        public float ReportFooterHeight
        {
            get
            {
                if (_rfHeight == 0)
                {
                    foreach (EbReportFooter r_footer in ReportFooters)
                        _rfHeight += r_footer.Height;
                }
                return _rfHeight;
            }
        }

        private float _dtHeight = 0;
        public float DetailHeight
        {
            get
            {
                if (_dtHeight == 0)
                {
                    foreach (EbReportDetail detail in Detail)
                        _dtHeight += detail.Height;
                }
                return _dtHeight;
            }
        }

        private float dt_fillheight = 0;
        public float DT_FillHeight
        {
            get
            {
                if (DataRow != null)
                {
                    if (DataRow.Count > 0)
                    {
                        var a = DataRow.Count * DetailHeight;
                        var b = Height - (PageHeaderHeight + PageFooterHeight + ReportHeaderHeight + ReportFooterHeight);
                        if (a < b && PageNumber == 1)
                            IsLastpage = true;
                    }
                }

                if (PageNumber == 1 && IsLastpage == true)
                    dt_fillheight = Height - (PageHeaderHeight + PageFooterHeight + ReportHeaderHeight + ReportFooterHeight);
                else if (PageNumber == 1)
                    dt_fillheight = Height - (ReportHeaderHeight + PageHeaderHeight + PageFooterHeight);
                else if (IsLastpage == true)
                    dt_fillheight = Height - (PageHeaderHeight + PageFooterHeight + ReportFooterHeight);
                else
                    dt_fillheight = Height - (PageHeaderHeight + PageFooterHeight);
                return dt_fillheight;
            }
        }

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
                        EbDataField f = (field as EbDataField);
                        if (!PageSummaryFields.ContainsKey(f.DataField))
                        {
                            SummaryFieldsList = new List<object>();
                            SummaryFieldsList.Add(f);
                            PageSummaryFields.Add(f.DataField, SummaryFieldsList);
                        }
                        else
                        {
                            PageSummaryFields[f.DataField].Add(f);
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
                        EbDataField f = (field as EbDataField);
                        if (!ReportSummaryFields.ContainsKey((field as EbDataField).DataField))
                        {
                            SummaryFieldsList = new List<object>();
                            SummaryFieldsList.Add(f);
                            ReportSummaryFields.Add(f.DataField, SummaryFieldsList);
                        }
                        else
                        {
                            ReportSummaryFields[f.DataField].Add(f);
                        }
                    }
                }
            }

        }

        public string GeFieldtData(string column_name, int i)
        {
            var columnindex = 0;
            var column_val = "";
            foreach (var col in DataColumns)
            {
                if (col.ColumnName == column_name)
                {
                    column_val = DataRow[i - 1][columnindex].ToString();
                    return column_val;
                }
                columnindex++;
            }
            return column_val;

            //return this.DataRow[i - 1][column_name].ToString();
        }

        public void DrawWaterMark(PdfReader pdfReader, Document d, PdfWriter writer)
        {
            byte[] fileByte = null;
            if (ReportObjects != null)
            {
                foreach (var field in ReportObjects)
                {
                    if ((field as EbWaterMark).Image != string.Empty)
                    {
                        fileByte = watermarkImages[(field as EbWaterMark).Image];
                    }
                (field as EbWaterMark).DrawMe(pdfReader, d, writer, fileByte, Height);
                }
            }
        }

        public void CallSummerize(string title, int i)
        {
            var column_name = string.Empty;
            var column_val = string.Empty;

            List<object> SummaryList;
            if (PageSummaryFields.ContainsKey(title))
            {
                SummaryList = PageSummaryFields[title];
                foreach (var item in SummaryList)
                {
                    var table = title.Split('.')[0];
                    column_name = title.Split('.')[1];
                    column_val = GeFieldtData(column_name, i);
                    (item as IEbDataFieldSummary).Summarize(column_val);
                }
            }
            if (ReportSummaryFields.ContainsKey(title))
            {
                SummaryList = ReportSummaryFields[title];
                foreach (var item in SummaryList)
                {
                    var table = title.Split('.')[0];
                    column_name = title.Split('.')[1];
                    column_val = GeFieldtData(column_name, i);
                    (item as IEbDataFieldSummary).Summarize(column_val);
                }
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

        public enum Operations
        {
            Print
        }
    }
    public class EbReportSection : EbReportObject
    {
        [EnableInBuilder(BuilderType.Report)]
        [UIproperty]
        public string SectionHeight { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        [HideInPropertyGrid]
        public List<EbReportField> Fields { get; set; }

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
