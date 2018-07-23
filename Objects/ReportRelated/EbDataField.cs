using ExpressBase.Common.Extensions;
using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Data;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using ExpressBase.Common.JsonConverters;
using ExpressBase.Objects.Objects.ReportRelated;
using ExpressBase.Common;
using ExpressBase.Common.Structures;
using ExpressBase.Common.Data;

namespace ExpressBase.Objects.ReportRelated
{
    public abstract class EbDataField : EbReportField
    {
        public virtual void NotifyNewPage(bool status) { }

        [EnableInBuilder(BuilderType.Report)]
        [HideInPropertyGrid]
        public string SummaryOf { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        [HideInPropertyGrid]
        public int DbType { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        [HideInPropertyGrid]
        public int TableIndex { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        [HideInPropertyGrid]
        public string ColumnName { get; set; }

        [OnChangeExec(@"
            pg.MakeReadOnly('Title');
        ")]
        [EnableInBuilder(BuilderType.Report)]
        public override string Title { set; get; }

        [EnableInBuilder(BuilderType.Report)]
        [PropertyGroup("Data Settings")]
        [UIproperty]
        public string Prefix { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        [PropertyGroup("Data Settings")]
        [UIproperty]
        public string Suffix { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        [PropertyGroup("Data Settings")]
        [UIproperty]
        public Boolean RenderInMultiLine { get; set; } = true;

        [EnableInBuilder(BuilderType.Report)]
        [PropertyEditor(PropertyEditorType.ObjectSelector)]
        [OSE_ObjectTypes(EbObjectTypes.iReport)]
        [PropertyGroup("Data Settings")]
        public string LinkRefid { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        [PropertyGroup("Data Settings")]
        [UIproperty]
        [PropertyEditor(PropertyEditorType.ScriptEditorCS)]
        [JsonConverter(typeof(Base64Converter))]
        public string AppearanceExpression { get; set; }

        private string[] _dataFieldsUsed;
        public string[] DataFieldsUsedAppearance
        {
            get
            {
                if (_dataFieldsUsed == null)
                {
                    var matches = Regex.Matches(this.AppearanceExpression, @"T[0-9]{1}.\w+").OfType<Match>()
     .Select(m => m.Groups[0].Value)
     .Distinct();


                    _dataFieldsUsed = new string[matches.Count()];
                    int i = 0;
                    foreach (var match in matches)
                        _dataFieldsUsed[i++] = match;
                }

                return _dataFieldsUsed;
            }
        }
        public override void DrawMe(Document doc, PdfContentByte canvas, float reportHeight, float printingTop, string column_val, float detailprintingtop, DbType column_type, List<Param> Params)
        {

            ColumnText ct = new ColumnText(canvas);
            Phrase text;
            
            if (Prefix != "" || Suffix != "")
            {
                column_val = Prefix + " " + column_val + " " + Suffix;
            }
            text = new Phrase(column_val, ITextFont);
            if (this.RenderInMultiLine)
            {
                column_val = RenderMultiLine(column_val, text, column_type);
                text = new Phrase(column_val, ITextFont);
            }
            if (!string.IsNullOrEmpty(LinkRefid))
            {
                Anchor a = CreateLink(text, LinkRefid, doc, Params);
                Paragraph p = new Paragraph
                {
                    a
                };
                p.Font = ITextFont;
                ct.AddText(p);
            }
            else
            {
                ct.AddText(text);
            }
            var ury = reportHeight - (printingTop + TopPt + detailprintingtop);
            var lly = reportHeight - (printingTop + TopPt + HeightPt + detailprintingtop);
            ct.SetSimpleColumn(LeftPt, lly,WidthPt + LeftPt, ury, 15, (int)TextAlign);
            ct.Go();
        }

        public string RenderMultiLine(string column_val, Phrase text, DbType column_type)
        {
            var calcbasefont = text.Font.GetCalculatedBaseFont(false);
            float stringwidth = calcbasefont.GetWidthPoint(column_val, text.Font.CalculatedSize);
            var charwidth = stringwidth / column_val.Length;
            int numberofCharsInALine = Convert.ToInt32(Math.Floor(WidthPt / charwidth));
            if (numberofCharsInALine < column_val.Length)
            {
                if (column_type == System.Data.DbType.Decimal)
                    column_val = "###";
            }
            return column_val;
        }

        public Anchor CreateLink(Phrase text, string LinkRefid, Document doc, List<Param> Params)
        {
            Anchor anchor = new Anchor(text)
            {
                Reference = "../ReportRender/RenderReport2?refid=" + LinkRefid + "&&Params=" + JsonConvert.SerializeObject(Params)
            };
            return anchor;
        }
    }

    [EnableInBuilder(BuilderType.Report)]
    public class EbDataFieldText : EbDataField
    {

        public override string GetDesignHtml()
        {
            return "<div class='EbCol dropped' $type='@type' eb-type='DataFieldText' id='@id' style='border: @Border px solid;border-color: @BorderColor ; width: @Width px; background-color:@BackColor ; color:@ForeColor ; height: @Height px; left: @Left px; top: @Top px;text-align: @TextAlign ;'> @Title </div>".RemoveCR().DoubleQuoted();
        }

        public override string GetJsInitFunc()
        {
            return @"
        this.Init = function(id)
            {
        this.Height =25;
        this.Width= 200;
        this.ForeColor = '#201c1c';
        this.Border = 1;
        this.BorderColor = '#eae6e6';
        };";
        }

        public override void DrawMe(Document doc, PdfContentByte canvas, float reportHeight, float printingTop, string column_val, float detailprintingtop, DbType column_type, List<Param> Params)
        {
            ColumnText ct = new ColumnText(canvas);
            Phrase text;
            text = new Phrase(column_val, ITextFont);
            if (this.RenderInMultiLine)
            {
                column_val = RenderMultiLine(column_val, text, column_type);
                text = new Phrase(column_val, ITextFont);
            }
            if (!string.IsNullOrEmpty(LinkRefid))
            {
                Anchor a = CreateLink(text, LinkRefid, doc, Params);
                Paragraph p = new Paragraph
                {
                    a
                };
                p.Font = ITextFont;
                ct.AddText(p);
            }
            else
            {
                ct.AddText(text);
            }
            var ury = reportHeight - (printingTop + TopPt + detailprintingtop);
                var lly = reportHeight - (printingTop + TopPt + HeightPt + detailprintingtop);
                ct.SetSimpleColumn(LeftPt, lly, WidthPt + LeftPt, ury, 15, (int)TextAlign);
                ct.Go();
        }

    }

    [EnableInBuilder(BuilderType.Report)]
    public class EbDataFieldDateTime : EbDataField
    {
        [EnableInBuilder(BuilderType.Report)]
        [PropertyGroup("Data Settings")]
        public DateFormatReport Format { get; set; }

        public override string GetDesignHtml()
        {
            return "<div class='EbCol dropped' $type='@type' eb-type='DataFieldDateTime' id='@id' style='border: @Border px solid;border-color: @BorderColor ; width: @Width px; background-color:@BackColor ; color:@ForeColor ; height: @Height px; left: @Left px; top: @Top px;text-align: @TextAlign;'> @Title </div>".RemoveCR().DoubleQuoted();
        }

        public override string GetJsInitFunc()
        {
            return @"
        this.Init = function(id)
            {
        this.Height =25;
        this.Width= 200;
        this.ForeColor = '#201c1c';
        this.Border = 1;
        this.BorderColor = '#eae6e6';
        };";
        }

        public string FormatDate(string column_val)
        {
            DateTime dt = Convert.ToDateTime(column_val);
            if (Format == DateFormatReport.dddd_MMMM_d_yyyy)
                return String.Format("{0:dddd, MMMM d, yyyy}", dt);
            else if (Format == DateFormatReport.M_d_yyyy)
                return String.Format("{0:M/d/yyyy}", dt);
            else if (Format == DateFormatReport.ddd_MMM_d_yyyy)
                return String.Format("{0:ddd, MMM d, yyyy}", dt);
            else if (Format == DateFormatReport.MM_dd_yy)
                return String.Format("{0:MM/dd/yy}", dt);
            else if (Format == DateFormatReport.MM_dd_yyyy)
                return String.Format("{0:MM/dd/yyyy}", dt);
            return column_val;
        }

        public override void DrawMe(Document doc, PdfContentByte canvas, float reportHeight, float printingTop, string column_val, float detailprintingtop, DbType column_type, List<Param> Params)
        {
            ColumnText ct = new ColumnText(canvas);
            Phrase text;
            column_val = FormatDate(column_val);
            if (Prefix != "" || Suffix != "")
            {
                column_val = Prefix + " " + column_val + " " + Suffix;
            }
            text = new Phrase(column_val, ITextFont);
            if (RenderInMultiLine)
            {
                column_val = RenderMultiLine(column_val, text, column_type);
                text = new Phrase(column_val, ITextFont);
            }
            if (!string.IsNullOrEmpty(LinkRefid))
            {
                Anchor a = CreateLink(text, LinkRefid, doc, Params);
                Paragraph p = new Paragraph
                {
                    a
                };
                p.Font = ITextFont;
                ct.AddText(p);
            }
            else
            {
                ct.AddText(text);
            }
            var ury = reportHeight - (printingTop + TopPt + detailprintingtop);
            var lly = reportHeight - (printingTop + TopPt + HeightPt + detailprintingtop);
            ct.SetSimpleColumn(LeftPt, lly, WidthPt + LeftPt, ury, 15, (int)TextAlign);
            ct.Go();
        }
    }

    [EnableInBuilder(BuilderType.Report)]
    public class EbDataFieldBoolean : EbDataField
    {
        public override string GetDesignHtml()
        {
            return "<div class='EbCol dropped' $type='@type' eb-type='DataFieldBoolean' id='@id' style='border: @Border px solid;border-color: @BorderColor ; width: @Width px; background-color:@BackColor ; color:@ForeColor ; height: @Height px; left: @Left px; top: @Top px;text-align: @TextAlign;'> @Title </div>".RemoveCR().DoubleQuoted();
        }

        public override string GetJsInitFunc()
        {
            return @"
         this.Init = function(id)
        {
        this.Height =25;
        this.Width= 200;
        this.ForeColor = '#201c1c';
        this.Border = 1;
        this.BorderColor = '#eae6e6';
        };";
        }
    }

    [EnableInBuilder(BuilderType.Report)]
    public class EbDataFieldNumeric : EbDataField
    {
        [EnableInBuilder(BuilderType.Report)]
        [PropertyGroup("Data Settings")]
        public int DecimalPlaces { get; set; } = 2;

        [EnableInBuilder(BuilderType.Report)]
        [PropertyGroup("Data Settings")]
        public bool InLetters { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        [PropertyGroup("Data Settings")]
        public bool SuppressIfZero { get; set; }

        public override string GetDesignHtml()
        {
            return "<div class='EbCol dropped' $type='@type' eb-type='DataFieldNumeric' id='@id' style='border: @Border px solid;border-color: @BorderColor ; width: @Width px; background-color:@BackColor ; color:@ForeColor ; height: @Height px; left: @Left px; top: @Top px;text-align: @TextAlign;'> @Title </div>".RemoveCR().DoubleQuoted();
        }

        public override string GetJsInitFunc()
        {
            return @"
    this.Init = function(id)
        {
    this.Height =25;
    this.Width= 200;
    this.ForeColor = '#201c1c';
this.Border = 1;
this.BorderColor = '#eae6e6';
};";
        }

        public override void DrawMe(Document doc, PdfContentByte canvas, float reportHeight, float printingTop, string column_val, float detailprintingtop, DbType column_type, List<Param> Params)
        {
            Phrase text;
            ColumnText ct = new ColumnText(canvas);
            var ury = reportHeight - (printingTop + TopPt + detailprintingtop);
            var lly = reportHeight - (printingTop + TopPt + HeightPt + detailprintingtop);
            if (DecimalPlaces > 0)
                column_val = Convert.ToDecimal(column_val).ToString("F" + DecimalPlaces);
            if (InLetters)
            {
                NumberToEnglish numToE = new NumberToEnglish();
                column_val = numToE.changeCurrencyToWords(column_val);
            }

            if (Prefix != "" || Suffix != "")
            {
                column_val = Prefix + " " + column_val + " " + Suffix;
            }
            text = new Phrase(column_val, ITextFont);
            if (RenderInMultiLine)
            {
                column_val = RenderMultiLine(column_val, text, column_type);
                text = new Phrase(column_val, ITextFont);
            }
            if (!string.IsNullOrEmpty(LinkRefid))
            {
                Anchor a = CreateLink(text, LinkRefid, doc, Params);
                Paragraph p = new Paragraph
                {
                    a
                };
                p.Font = ITextFont;
                ct.AddText(p);
            }
            else
            {
                ct.AddText(text);
            }
            ct.Canvas.SetColorFill(GetColor(ForeColor));
            ct.SetSimpleColumn(LeftPt, lly, WidthPt + LeftPt, ury, 15, (int)TextAlign);
            ct.Go();
        }

    }

    public interface IEbDataFieldSummary
    {
        object SummarizedValue { get; }
        void Summarize(object value);
    }

    [EnableInBuilder(BuilderType.Report)]
    public class EbDataFieldNumericSummary : EbDataFieldNumeric, IEbDataFieldSummary
    {
        [EnableInBuilder(BuilderType.Report)]
        [MetaOnly]
        public SummaryFunctionsNumeric Function { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        [HideInPropertyGrid]
        public bool ResetOnNewPage { get; set; }

        private int Count { get; set; }

        private decimal Sum { get; set; }

        private decimal Max { get; set; }

        private decimal Min { get; set; }

        public object SummarizedValue
        {
            get
            {
                if (Function == SummaryFunctionsNumeric.Sum)
                    return Sum;
                else if (Function == SummaryFunctionsNumeric.Average && this.Count > 0)
                    return Sum / Count;
                else if (Function == SummaryFunctionsNumeric.Count)
                    return Count;
                else if (Function == SummaryFunctionsNumeric.Max)
                    return Max;
                else if (Function == SummaryFunctionsNumeric.Min)
                    return Min;

                return 0;
            }
        }

        public void Summarize(object value)
        {
            Count++;
            decimal myvalue = Convert.ToDecimal(value);

            if (Function == SummaryFunctionsNumeric.Sum || Function == SummaryFunctionsNumeric.Average)
            {
                if (Function == SummaryFunctionsNumeric.Sum || Function == SummaryFunctionsNumeric.Average)
                    Sum += myvalue;
            }

            if (Count > 1)
            {
                if (Function == SummaryFunctionsNumeric.Max)
                    Max = (Max > myvalue) ? Max : myvalue;
                else if (Function == SummaryFunctionsNumeric.Min)
                    Min = (Min < myvalue) ? Min : myvalue;
            }
            else
            {
                if (Function == SummaryFunctionsNumeric.Max)
                    Max = myvalue;
                else if (Function == SummaryFunctionsNumeric.Min)
                    Min = myvalue;
            }
        }

        public override void NotifyNewPage(bool status)
        {
            if (status && ResetOnNewPage)
                Sum = 0;
        }

        public override string GetDesignHtml()
        {
            return "<div class='dropped' $type='@type' eb-type='DataFieldNumericSummary' id='@id' style='border: @Border px solid;border-color: @BorderColor ; width: @Width px; background-color:@BackColor ; color:@ForeColor ; height: @Height px; left: @Left px; top: @Top px;text-align: @TextAlign;'> @Title </div>".RemoveCR().DoubleQuoted();
        }

        public override string GetJsInitFunc()
        {
            return @"
    this.Init = function(id)
        {
    this.Height =25;
    this.Width= 200;
    this.ForeColor = '#201c1c';
this.Border = 1;
this.BorderColor = '#eae6e6';
};";
        }
        public override void DrawMe(Document doc, PdfContentByte canvas, float reportHeight, float printingTop, string column_val, float detailprintingtop, DbType column_type, List<Param> Params)
        {
            Phrase text;
            var ury = reportHeight - (printingTop + TopPt + detailprintingtop);
            var lly = reportHeight - (printingTop + TopPt + HeightPt + detailprintingtop);
            if (DecimalPlaces > 0)
                column_val = Convert.ToDecimal(column_val).ToString("F" + DecimalPlaces);
            if (InLetters)
            {
                NumberToEnglish numToE = new NumberToEnglish();
                column_val = numToE.changeCurrencyToWords(column_val);
            }
            text = new Phrase(column_val, ITextFont);
            if (this.RenderInMultiLine)
            {
                column_val = RenderMultiLine(column_val, text, column_type);
                text = new Phrase(column_val, ITextFont);
            }
            ColumnText ct = new ColumnText(canvas);
            ct.SetSimpleColumn(text, LeftPt, lly, WidthPt + LeftPt, ury, 15, (int)TextAlign);
            ct.Go();
        }
    }

    [EnableInBuilder(BuilderType.Report)]
    public class EbDataFieldTextSummary : EbDataFieldText, IEbDataFieldSummary
    {
        [EnableInBuilder(BuilderType.Report)]
        [MetaOnly]
        public SummaryFunctionsText Function { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        [HideInPropertyGrid]
        public bool ResetOnNewPage { get; set; }

        private int Count { get; set; }

        private string Max { get; set; } = "";

        private string Min { get; set; } = "";

        public object SummarizedValue
        {
            get
            {
                if (Function == SummaryFunctionsText.Count)
                    return Count;
                else if (Function == SummaryFunctionsText.Max)
                    return Max;
                else if (Function == SummaryFunctionsText.Min)
                    return Min;

                return 0;
            }
        }

        public void Summarize(object value)
        {
            var myvalue = value.ToString();
            Count++;
            if (Count > 1)
            {
                if (Function == SummaryFunctionsText.Max)
                    Max = (Max.CompareTo(myvalue) > 0) ? Max : myvalue;
                else if (Function == SummaryFunctionsText.Min)
                    Min = (Min.CompareTo(myvalue) > 0) ? myvalue : Min;
            }
            else
            {
                if (Function == SummaryFunctionsText.Max)
                    Max = myvalue;
                else if (Function == SummaryFunctionsText.Min)
                    Min = myvalue;
            }
        }

        public override string GetDesignHtml()
        {
            return "<div class='dropped' $type='@type' eb-type='DataFieldTextSummary' id='@id' style='border: @Border px solid;border-color: @BorderColor ; width: @Width px; background-color:@BackColor ; color:@ForeColor ; height: @Height px; left: @Left px; top: @Top px;text-align: @TextAlign;'> @Title </div>".RemoveCR().DoubleQuoted();
        }

        public override string GetJsInitFunc()
        {
            return @"
        this.Init = function(id)
            {
        this.Height =25;
        this.Width= 200;
        this.ForeColor = '#201c1c';
        this.Border = 1;
        this.BorderColor = '#eae6e6';
        };";
        }
    }

    [EnableInBuilder(BuilderType.Report)]
    public class EbDataFieldDateTimeSummary : EbDataFieldDateTime, IEbDataFieldSummary
    {
        [EnableInBuilder(BuilderType.Report)]
        [MetaOnly]
        public SummaryFunctionsDateTime Function { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        [HideInPropertyGrid]
        public bool ResetOnNewPage { get; set; }

        private int Count { get; set; }

        private DateTime Max { get; set; }

        private DateTime Min { get; set; }

        public object SummarizedValue
        {
            get
            {
                if (Function == SummaryFunctionsDateTime.Count)
                    return Count;
                else if (Function == SummaryFunctionsDateTime.Max)
                    return Max;
                else if (Function == SummaryFunctionsDateTime.Min)
                    return Min;

                return 0;
            }
        }

        public void Summarize(object value)
        {
            var myvalue = Convert.ToDateTime(value);
            Count++;
            if (Count > 1)
            {
                if (Function == SummaryFunctionsDateTime.Max)
                    Max = (DateTime.Compare(Max, myvalue) > 0) ? Max : myvalue;
                if (Function == SummaryFunctionsDateTime.Min)
                    Min = (DateTime.Compare(Min, myvalue) > 0) ? myvalue : Min;
            }
            else
            {
                if (Function == SummaryFunctionsDateTime.Max)
                    Max = myvalue;
                if (Function == SummaryFunctionsDateTime.Min)
                    Min = myvalue;
            }
        }

        public override string GetDesignHtml()
        {
            return "<div class='dropped' $type='@type' eb-type='DataFieldDateTimeSummary' id='@id' style='border: @Border px solid;border-color: @BorderColor ; width: @Width px; background-color:@BackColor ; color:@ForeColor ; height: @Height px; left: @Left px; top: @Top px;text-align: @TextAlign;'> @Title </div>".RemoveCR().DoubleQuoted();
        }

        public override string GetJsInitFunc()
        {
            return @"
        this.Init = function(id)
            {
        this.Height =25;
        this.Width= 200;
        this.ForeColor = '#201c1c';
        this.Border = 1;
        this.BorderColor = '#eae6e6';
        };";
        }
    }

    [EnableInBuilder(BuilderType.Report)]
    public class EbDataFieldBooleanSummary : EbDataFieldBoolean, IEbDataFieldSummary
    {
        [EnableInBuilder(BuilderType.Report)]
        [MetaOnly]
        public SummaryFunctionsBoolean Function { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        [HideInPropertyGrid]
        public bool ResetOnNewPage { get; set; }

        private int Count { get; set; }

        public object SummarizedValue
        {
            get
            {
                if (Function == SummaryFunctionsBoolean.Count)
                    return Count;
                return 0;
            }
        }

        public void Summarize(object value)
        {
            Count++;
        }

        public override string GetDesignHtml()
        {
            return "<div class='dropped' $type='@type' eb-type='DataFieldBooleanSummary' id='@id' style='border: @Border px solid;border-color: @BorderColor ; width: @Width px; background-color:@BackColor ; color:@ForeColor ; height: @Height px; left: @Left px; top: @Top px;text-align: @TextAlign ;'> @Title </div>".RemoveCR().DoubleQuoted();
        }

        public override string GetJsInitFunc()
        {
            return @"
        this.Init = function(id)
        {
        this.Height =25;
        this.Width= 200;
        this.ForeColor = '#201c1c';
        this.Border = 1;
        this.BorderColor = '#eae6e6';
        };";
        }
    }

    [EnableInBuilder(BuilderType.Report)]
    public class EbCalcField : EbDataField
    {
        [EnableInBuilder(BuilderType.Report)]
        [PropertyGroup("Data Settings")]
        [PropertyEditor(PropertyEditorType.ScriptEditorCS)]
        [JsonConverter(typeof(Base64Converter))]
        public string ValueExpression { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        [UIproperty]
        [MetaOnly]
        public string CalcFieldType { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        [HideInPropertyGrid]
        public int CalcFieldIntType { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        [PropertyGroup("Data Settings")]
        public int DecimalPlaces { get; set; } = 2;

        [EnableInBuilder(BuilderType.Report)]
        [PropertyGroup("Data Settings")]
        public bool InLetters { get; set; }

        private string[] _dataFieldsUsed;
        public new string[] DataFieldsUsedCalc
        {
            get
            {
                if (_dataFieldsUsed == null)
                {
                    var matches = Regex.Matches(ValueExpression, @"T[0-9]{1}.\w+").OfType<Match>()
     .Select(m => m.Groups[0].Value)
     .Distinct();


                    _dataFieldsUsed = new string[matches.Count()];
                    int i = 0;
                    foreach (var match in matches)
                        _dataFieldsUsed[i++] = match;
                }

                return _dataFieldsUsed;
            }
        }

        public override string GetDesignHtml()
        {
            return "<div class='dropped CalcField' $type='@type' cfType='@CalcFieldType ' eb-type='CalcField' id='@id' style='border: @Border px solid;border-color: @BorderColor ; width: @Width px; background-color:@BackColor ; color:@ForeColor ; height: @Height px; left: @Left px; top: @Top px;text-align: @TextAlign ;'> @Title </div>".RemoveCR().DoubleQuoted();
        }

        public override string GetJsInitFunc()
        {
            return @"
        this.Init = function(id)
            {
        this.Height =25;
        this.Width= 200;
        this.ForeColor = '#201c1c';
        this.Border = 1;
        this.BorderColor = '#eae6e6';
        };";
        }

        public override void DrawMe(Document doc, PdfContentByte canvas, float reportHeight, float printingTop, string column_val, float detailprintingtop, DbType column_type, List<Param> Params)
        {
            ColumnText ct = new ColumnText(canvas);
            Phrase text = null;

            if (column_val == "")
            {
                column_val = "0";
            }
            if (DecimalPlaces > 0)
                column_val = Convert.ToDecimal(column_val).ToString("F" + DecimalPlaces);
            if (InLetters)
            {
                NumberToEnglish numToE = new NumberToEnglish();
                column_val = numToE.changeCurrencyToWords(column_val);
            }

            if (Prefix != "" || Suffix != "")
            {
                column_val = Prefix + " " + column_val + " " + Suffix;
            }
            text = new Phrase(column_val, ITextFont);
            if (RenderInMultiLine)
            {
                column_val = RenderMultiLine(column_val, text, column_type);
                text = new Phrase(column_val, ITextFont);
            }
            var ury = reportHeight - (printingTop + TopPt + detailprintingtop);
            var lly = reportHeight - (printingTop + TopPt + HeightPt + detailprintingtop);
            ct.SetSimpleColumn(text, LeftPt, lly, WidthPt + LeftPt, ury, 15, (int)TextAlign);
            ct.Go();
        }
    }

    [EnableInBuilder(BuilderType.Report)]
    public class EbCalcFieldNumericSummary : EbCalcField, IEbDataFieldSummary
    {
        [EnableInBuilder(BuilderType.Report)]
        [MetaOnly]
        public SummaryFunctionsNumeric Function { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        [HideInPropertyGrid]
        public bool ResetOnNewPage { get; set; }

        private int Count { get; set; }

        private decimal Sum { get; set; }

        private decimal Max { get; set; }

        private decimal Min { get; set; }

        public object SummarizedValue
        {
            get
            {
                if (Function == SummaryFunctionsNumeric.Sum)
                    return Sum;
                else if (Function == SummaryFunctionsNumeric.Average && Count > 0)
                    return Sum / Count;
                else if (Function == SummaryFunctionsNumeric.Count)
                    return Count;
                else if (Function == SummaryFunctionsNumeric.Max)
                    return Max;
                else if (Function == SummaryFunctionsNumeric.Min)
                    return Min;

                return 0;
            }
        }

        public void Summarize(object value)
        {
            Count++;
            decimal myvalue = Convert.ToDecimal(value);

            if (Function == SummaryFunctionsNumeric.Sum || Function == SummaryFunctionsNumeric.Average)
            {
                if (Function == SummaryFunctionsNumeric.Sum || Function == SummaryFunctionsNumeric.Average)
                    Sum += myvalue;
            }

            if (Count > 1)
            {
                if (Function == SummaryFunctionsNumeric.Max)
                    Max = (Max > myvalue) ? Max : myvalue;
                else if (Function == SummaryFunctionsNumeric.Min)
                    Min = (Min < myvalue) ? Min : myvalue;
            }
            else
            {
                if (Function == SummaryFunctionsNumeric.Max)
                    Max = myvalue;
                else if (Function == SummaryFunctionsNumeric.Min)
                    Min = myvalue;
            }
        }

        public override void NotifyNewPage(bool status)
        {
            if (status && ResetOnNewPage)
                Sum = 0;
        }

        public override string GetDesignHtml()
        {
            return "<div class='dropped' $type='@type' eb-type='CalcFieldNumericSummary' id='@id' style='border: @Border px solid;border-color: @BorderColor ; width: @Width px; background-color:@BackColor ; color:@ForeColor ; height: @Height px; left: @Left px; top: @Top px;text-align: @TextAlign;'> @Title </div>".RemoveCR().DoubleQuoted();
        }

        public override string GetJsInitFunc()
        {
            return @"
         this.Init = function(id)
            {
        this.Height =25;
        this.Width= 200;
        this.ForeColor = '#201c1c';
        this.Border = 1;
        this.BorderColor = '#eae6e6';
        };";
        }
        public override void DrawMe(Document doc, PdfContentByte canvas, float reportHeight, float printingTop, string column_val, float detailprintingtop, DbType column_type, List<Param> Params)
        {
            Phrase text;
            var ury = reportHeight - (printingTop + TopPt + detailprintingtop);
            var lly = reportHeight - (printingTop + TopPt + HeightPt + detailprintingtop);
            if (DecimalPlaces > 0)
                column_val = Convert.ToDecimal(column_val).ToString("F" + DecimalPlaces);
            if (InLetters)
            {
                NumberToEnglish numToE = new NumberToEnglish();
                column_val = numToE.changeCurrencyToWords(column_val);
            }
            text = new Phrase(column_val, ITextFont);
            if (RenderInMultiLine)
            {
                column_val = RenderMultiLine(column_val, text, column_type);
                text = new Phrase(column_val, ITextFont);
            }
            ColumnText ct = new ColumnText(canvas);
            ct.SetSimpleColumn(text, LeftPt, lly, WidthPt + LeftPt, ury, 15, (int)TextAlign);
            ct.Go();
        }

    }

    [EnableInBuilder(BuilderType.Report)]
    public class EbCalcFieldTextSummary : EbCalcField, IEbDataFieldSummary
    {
        [EnableInBuilder(BuilderType.Report)]
        [MetaOnly]
        public SummaryFunctionsText Function { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        [HideInPropertyGrid]
        public bool ResetOnNewPage { get; set; }

        private int Count { get; set; }

        private string Max { get; set; } = "";

        private string Min { get; set; } = "";

        public object SummarizedValue
        {
            get
            {
                if (Function == SummaryFunctionsText.Count)
                    return Count;
                else if (Function == SummaryFunctionsText.Max)
                    return Max;
                else if (Function == SummaryFunctionsText.Min)
                    return Min;

                return 0;
            }
        }

        public void Summarize(object value)
        {
            var myvalue = value.ToString();
            Count++;
            if (Count > 1)
            {
                if (Function == SummaryFunctionsText.Max)
                    Max = (Max.CompareTo(myvalue) > 0) ? Max : myvalue;
                else if (Function == SummaryFunctionsText.Min)
                    Min = (Min.CompareTo(myvalue) > 0) ? myvalue : Min;
            }
            else
            {
                if (Function == SummaryFunctionsText.Max)
                    Max = myvalue;
                else if (Function == SummaryFunctionsText.Min)
                    Min = myvalue;
            }
        }

        public override string GetDesignHtml()
        {
            return "<div class='dropped' eb-type='CalcFieldTextSummary' id='@id' style='border: @Border px solid;border-color: @BorderColor ; width: @Width px; background-color:@BackColor ; color:@ForeColor ; height: @Height px; left: @Left px; top: @Top px;text-align: @TextAlign;'> @Title </div>".RemoveCR().DoubleQuoted();
        }

        public override string GetJsInitFunc()
        {
            return @"
    this.Init = function(id)
        {
    this.Height =25;
    this.Width= 200;
    this.ForeColor = '#201c1c';
this.Border = 1;
this.BorderColor = '#eae6e6';
};";
        }
    }

    [EnableInBuilder(BuilderType.Report)]
    public class EbCalcFieldDateTimeSummary : EbCalcField, IEbDataFieldSummary
    {
        [EnableInBuilder(BuilderType.Report)]
        [MetaOnly]
        public SummaryFunctionsDateTime Function { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        [HideInPropertyGrid]
        public bool ResetOnNewPage { get; set; }

        private int Count { get; set; }

        private DateTime Max { get; set; }

        private DateTime Min { get; set; }

        public object SummarizedValue
        {
            get
            {
                if (Function == SummaryFunctionsDateTime.Count)
                    return Count;
                else if (Function == SummaryFunctionsDateTime.Max)
                    return Max;
                else if (Function == SummaryFunctionsDateTime.Min)
                    return Min;

                return 0;
            }
        }

        public void Summarize(object value)
        {
            var myvalue = Convert.ToDateTime(value);
            Count++;
            if (Count > 1)
            {
                if (Function == SummaryFunctionsDateTime.Max)
                    Max = (DateTime.Compare(Max, myvalue) > 0) ? Max : myvalue;
                if (Function == SummaryFunctionsDateTime.Min)
                    Min = (DateTime.Compare(Min, myvalue) > 0) ? myvalue : Min;
            }
            else
            {
                if (Function == SummaryFunctionsDateTime.Max)
                    Max = myvalue;
                if (Function == SummaryFunctionsDateTime.Min)
                    Min = myvalue;
            }
        }

        public override string GetDesignHtml()
        {
            return "<div class='dropped' eb-type='CalcFieldDatetimeSummary' id='@id' style='border: @Border px solid;border-color: @BorderColor ; width: @Width px; background-color:@BackColor ; color:@ForeColor ; height: @Height px; left: @Left px; top: @Top px;text-align: @TextAlign;'> @Title </div>".RemoveCR().DoubleQuoted();
        }
        public override string GetJsInitFunc()
        {
            return @"
    this.Init = function(id)
        {
    this.Height =25;
    this.Width= 200;
    this.ForeColor = '#201c1c';
this.Border = 1;
this.BorderColor = '#eae6e6';
};";
        }
    }

    [EnableInBuilder(BuilderType.Report)]
    public class EbCalcFieldBooleanSummary : EbCalcField, IEbDataFieldSummary
    {
        [EnableInBuilder(BuilderType.Report)]
        [MetaOnly]
        public SummaryFunctionsBoolean Function { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        [HideInPropertyGrid]
        public bool ResetOnNewPage { get; set; }

        private int Count { get; set; }

        public object SummarizedValue
        {
            get
            {
                if (Function == SummaryFunctionsBoolean.Count)
                    return Count;
                return 0;
            }
        }

        public void Summarize(object value)
        {
            Count++;
        }

        public override string GetDesignHtml()
        {
            return "<div class='dropped' eb-type='CalcFieldBooleanSummary' id='@id' style='border: @Border px solid;border-color: @BorderColor ; width: @Width px; background-color:@BackColor ; color:@ForeColor ; height: @Height px; left: @Left px; top: @Top px;text-align: @TextAlign ;'> @Title </div>".RemoveCR().DoubleQuoted();
        }
        public override string GetJsInitFunc()
        {
            return @"
    this.Init = function(id)
        {
    this.Height =25;
    this.Width= 200;
    this.ForeColor = '#201c1c';
this.Border = 1;
this.BorderColor = '#eae6e6';
};";
        }
    }
}