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
using ExpressBase.Common;
using ExpressBase.Common.Structures;
using ExpressBase.Common.Data;
using ExpressBase.Objects.Objects;
using System.Globalization;
using ExpressBase.CoreBase.Globals;

namespace ExpressBase.Objects
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
        [PropertyGroup("Appearance")]
        [UIproperty]
        public Boolean RenderInMultiLine { get; set; } = true;

        [EnableInBuilder(BuilderType.Report)]
        [PropertyEditor(PropertyEditorType.ObjectSelector)]
        [OSE_ObjectTypes(EbObjectTypes.iReport, EbObjectTypes.iTableVisualization, EbObjectTypes.iChartVisualization)]
        [PropertyGroup("Data Settings")]
        public string LinkRefId { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        [PropertyGroup("Appearance")]
        [UIproperty]
        [PropertyEditor(PropertyEditorType.ScriptEditorCS, PropertyEditorType.ScriptEditorJS)]
        [Alias("Appearance Expression")]
        public EbScript AppearExpression { get; set; }

        private string[] _dataFieldsUsed;
        public string[] DataFieldsUsedAppearance
        {
            get
            {
                if (_dataFieldsUsed == null || _dataFieldsUsed.Count() <= 0)
                {
                    if (this.AppearExpression != null && this.AppearExpression.Code != null)
                    {
                        IEnumerable<string> matches = Regex.Matches(this.AppearExpression.Code, @"T[0-9]{1}.\w+").OfType<Match>()
                               .Select(m => m.Groups[0].Value)
                               .Distinct();

                        _dataFieldsUsed = new string[matches.Count()];
                        int i = 0;
                        foreach (string match in matches)
                            _dataFieldsUsed[i++] = match;
                    }
                }
                else
                {
                    _dataFieldsUsed = new string[0];
                }

                return _dataFieldsUsed;
            }
        }

        public override void DrawMe(float printingTop, EbReport Rep, List<Param> Params, int slno)
        {

            ColumnText ct = new ColumnText(Rep.Canvas);
            string column_val = Rep.GetDataFieldValue(ColumnName, slno, TableIndex);
            if (Prefix != "" || Suffix != "")
                column_val = Prefix + " " + column_val + " " + Suffix;

            Phrase phrase = GetPhrase(column_val, (DbType)DbType, Rep.Font);

            if (!string.IsNullOrEmpty(LinkRefId))
            {
                Anchor a = CreateLink(phrase, LinkRefId, Rep.Doc, Params);
                Paragraph p = new Paragraph { a };
                p.Font = GetItextFont(this.Font, Rep.Font);
                ct.AddText(p);
            }
            else
                ct.AddText(phrase);
            float ury = Rep.HeightPt - (printingTop + TopPt + Rep.detailprintingtop);
            float lly = Rep.HeightPt - (printingTop + TopPt + HeightPt + Rep.detailprintingtop);
            ct.SetSimpleColumn(Llx, lly, Urx, ury, Leading, (int)TextAlign);
            ct.Go();
        }

        public Phrase GetPhrase(string column_val, DbType column_type, EbFont _reportFont)
        {

            Phrase phrase = GetFormattedPhrase(this.Font, _reportFont, column_val);

            if (this.RenderInMultiLine && column_val != string.Empty && column_type == System.Data.DbType.Decimal || column_type == System.Data.DbType.Double || column_type == System.Data.DbType.Int16 || column_type == System.Data.DbType.Int32 || column_type == System.Data.DbType.Int64 || column_type == System.Data.DbType.VarNumeric)
            {
                try
                {
                    BaseFont calcbasefont = phrase.Font.GetCalculatedBaseFont(false);
                    float stringwidth = calcbasefont.GetWidthPoint(column_val, phrase.Font.CalculatedSize);
                    if (stringwidth > 0)
                    {
                        float charwidth = stringwidth / column_val.Length;
                        int numberofCharsInALine = Convert.ToInt32(Math.Floor(WidthPt / charwidth));
                        if (numberofCharsInALine < column_val.Length)
                        {

                            column_val = "###";
                            //else if (column_type == System.Data.DbType.String)
                            //    column_val = column_val.Substring(0, numberofCharsInALine - 2) + "...";
                        }
                    }
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
            return GetFormattedPhrase(this.Font, _reportFont, column_val);
        }

        public Anchor CreateLink(Phrase phrase, string LinkRefid, Document doc, List<Param> Params)
        {
            string _ref = string.Empty;
            byte[] toEncodeAsBytes = System.Text.ASCIIEncoding.ASCII.GetBytes(JsonConvert.SerializeObject(Params));
            string returnValue = System.Convert.ToBase64String(toEncodeAsBytes);
            int type = Convert.ToInt32(LinkRefid.Split("-")[2]);
            if (type == 3)
                _ref = "/ReportRender/Renderlink?refid=" + LinkRefid + "&_params=" + returnValue;
            else if (type == 16 || type == 17)
                _ref = "/DV/renderlink?_refid=" + LinkRefid + "&Params=" + returnValue;
            Anchor anchor = new Anchor(phrase)
            {
                Reference = _ref
            };
            return anchor;
        }

        public string FormatDecimals(string column_val, bool _inWords, int _decimalPlaces, NumberFormatInfo _numberFormat)
        {
            if (_inWords)
            {
                NumberToEnglish numToE = new NumberToEnglish();
                column_val = numToE.changeCurrencyToWords(column_val);
            }
            else
            {
                column_val = Convert.ToDecimal(column_val).ToString("N", _numberFormat);
                column_val = Convert.ToDecimal(column_val).ToString("F" + _decimalPlaces);
            }
            return column_val;
        }

        public void DoRenderInMultiLine(string column_val, EbReport Report)
        {
            //Report.RowHeight = 0;
            Report.MultiRowTop = 0;
            DbType datatype = (DbType)DbType;
            int val_length = column_val.Length;
            Phrase phrase = new Phrase(column_val, this.GetItextFont(this.Font, this.Font));
            float calculatedValueSize = phrase.Font.CalculatedSize * val_length;
            if (calculatedValueSize > this.WidthPt)
            {
                int rowsneeded;
                if (datatype == System.Data.DbType.Decimal || datatype == System.Data.DbType.Double || datatype == System.Data.DbType.Int16 || datatype == System.Data.DbType.Int32 || datatype == System.Data.DbType.Int64 || datatype == System.Data.DbType.VarNumeric)
                    rowsneeded = 1;
                else
                    rowsneeded = Convert.ToInt32(Math.Floor(calculatedValueSize / this.WidthPt));
                if (rowsneeded > 1)
                {
                    if (Report.MultiRowTop == 0)
                    {
                        Report.MultiRowTop = this.TopPt;
                    }
                    float k = (phrase.Font.CalculatedSize) * (rowsneeded - 1);
                    if (k > Report.RowHeight)
                    {
                        Report.RowHeight = k;
                    }
                }
            }
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
                        this.Leading = 12;
                    };";
        }

        public override void DrawMe(float printingTop, EbReport Rep, List<Param> Params, int slno)
        {
            ColumnText ct = new ColumnText(Rep.Canvas);
            string column_val = Rep.GetDataFieldValue(ColumnName, slno, TableIndex);
            if (Prefix != "" || Suffix != "")
                column_val = Prefix + " " + column_val + " " + Suffix;
            Phrase phrase = GetPhrase(column_val, (DbType)DbType, Rep.Font);
            if (!string.IsNullOrEmpty(LinkRefId))
            {
                Anchor a = CreateLink(phrase, LinkRefId, Rep.Doc, Params);
                Paragraph p = new Paragraph { a };
                p.Font = GetItextFont(this.Font, Rep.Font);
                ct.AddText(p);
            }
            else
                ct.AddText(phrase);
            float ury = Rep.HeightPt - (printingTop + TopPt + Rep.detailprintingtop);
            float lly = Rep.HeightPt - (printingTop + TopPt + HeightPt + Rep.detailprintingtop);
            ct.SetSimpleColumn(Llx, lly, Urx, ury, Leading, (int)TextAlign);
            ct.Go();
        }

    }

    [EnableInBuilder(BuilderType.Report)]
    public class EbDataFieldDateTime : EbDataField
    {
        [EnableInBuilder(BuilderType.Report)]
        [PropertyGroup("Data Settings")]
        public DateFormatReport Format { get; set; } = DateFormatReport.from_culture;

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
                        this.Leading = 12;
                    };";
        }


        public override void DrawMe(float printingTop, EbReport Rep, List<Param> Params, int slno)
        {
            ColumnText ct = new ColumnText(Rep.Canvas);
            string column_val = Rep.GetDataFieldValue(ColumnName, slno, TableIndex);
            column_val = FormatDate(column_val, Format, Rep);
            if (Prefix != "" || Suffix != "")
                column_val = Prefix + " " + column_val + " " + Suffix;
            Phrase phrase = GetPhrase(column_val, (DbType)DbType, Rep.Font);
            if (!string.IsNullOrEmpty(LinkRefId))
            {
                Anchor a = CreateLink(phrase, LinkRefId, Rep.Doc, Params);
                Paragraph p = new Paragraph
                {
                    a
                };
                p.Font = GetItextFont(this.Font, Rep.Font);
                ct.AddText(p);
            }
            else
            {
                ct.AddText(phrase);
            }
            float ury = Rep.HeightPt - (printingTop + TopPt + Rep.detailprintingtop);
            float lly = Rep.HeightPt - (printingTop + TopPt + HeightPt + Rep.detailprintingtop);
            ct.SetSimpleColumn(Llx, lly, Urx, ury, Leading, (int)TextAlign);
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
                        this.Leading = 12;
                };";
        }

        public override void DrawMe(float printingTop, EbReport Rep, List<Param> Params, int slno)
        {

            ColumnText ct = new ColumnText(Rep.Canvas);
            string column_val = Rep.GetDataFieldValue(ColumnName, slno, TableIndex);
            if (Prefix != "" || Suffix != "")
                column_val = Prefix + " " + column_val + " " + Suffix;
            Phrase phrase = GetFormattedPhrase(this.Font, Rep.Font, column_val);
            if (!string.IsNullOrEmpty(LinkRefId))
            {
                Anchor a = CreateLink(phrase, LinkRefId, Rep.Doc, Params);
                Paragraph p = new Paragraph { a };
                p.Font = GetItextFont(this.Font, Rep.Font);
                ct.AddText(p);
            }
            else
                ct.AddText(phrase);
            float ury = Rep.HeightPt - (printingTop + TopPt + Rep.detailprintingtop);
            float lly = Rep.HeightPt - (printingTop + TopPt + HeightPt + Rep.detailprintingtop);
            ct.SetSimpleColumn(Llx, lly, Urx, ury, Leading, (int)TextAlign);
            ct.Go();
        }
    }

    [EnableInBuilder(BuilderType.Report)]
    public class EbDataFieldNumeric : EbDataField
    {
        [EnableInBuilder(BuilderType.Report)]
        [PropertyGroup("Data Settings")]
        public bool AmountInWords { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        [PropertyGroup("Data Settings")]
        public bool SuppressIfZero { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        [PropertyGroup("Data Settings")]
        public int DecimalPlaces { get; set; } = 2;



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
                        this.DecimalPlaces = 2;
                        this.Leading = 12;
                    };";
        }

        public override void DrawMe(float printingTop, EbReport Rep, List<Param> Params, int slno)
        {
            ColumnText ct = new ColumnText(Rep.Canvas);
            string column_val = Rep.GetDataFieldValue(ColumnName, slno, TableIndex);
            float ury = Rep.HeightPt - (printingTop + TopPt + Rep.detailprintingtop);
            float lly = Rep.HeightPt - (printingTop + TopPt + HeightPt + Rep.detailprintingtop);

            if (SuppressIfZero && !(Convert.ToDecimal(column_val) > 0))
                column_val = String.Empty;
            else
            {
                column_val = FormatDecimals(column_val, AmountInWords, DecimalPlaces, Rep.CultureInfo.NumberFormat);
                if (Prefix != "" || Suffix != "")
                    column_val = Prefix + " " + column_val + " " + Suffix;
            }
            Phrase phrase = GetPhrase(column_val, (DbType)DbType, Rep.Font);
            if (!string.IsNullOrEmpty(LinkRefId))
            {
                Anchor a = CreateLink(phrase, LinkRefId, Rep.Doc, Params);
                Paragraph p = new Paragraph { a };
                p.Font = GetItextFont(this.Font, Rep.Font);
                ct.AddText(p);
            }
            else
                ct.AddText(phrase);
            ct.Canvas.SetColorFill(GetColor(ForeColor));
            ct.SetSimpleColumn(Llx, lly, Urx, ury, Leading, (int)TextAlign);
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
        [PropertyGroup("Data Settings")]
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
                    this.DecimalPlaces = 2;
                    this.Leading = 12;
                };";
        }

        public override void DrawMe(float printingTop, EbReport Rep, List<Param> Params, int slno)
        {
            float ury = Rep.HeightPt - (printingTop + TopPt + Rep.detailprintingtop);
            float lly = Rep.HeightPt - (printingTop + TopPt + HeightPt + Rep.detailprintingtop);
            string column_val = SummarizedValue.ToString();
            column_val = FormatDecimals(column_val, AmountInWords, DecimalPlaces, Rep.CultureInfo.NumberFormat);

            if (Rep.SummaryValInRow.ContainsKey(Title))
                Rep.SummaryValInRow[Title] = new PdfNTV { Name = Title, Type = PdfEbDbTypes.Int32, Value = column_val };
            else
                Rep.SummaryValInRow.Add(Title, new PdfNTV { Name = Title, Type = PdfEbDbTypes.Int32, Value = column_val });

            Phrase phrase = GetPhrase(column_val, (DbType)DbType, Rep.Font);
            ColumnText ct = new ColumnText(Rep.Canvas);
            ct.SetSimpleColumn(phrase, Llx, lly, Urx, ury, Leading, (int)TextAlign);
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
        [PropertyGroup("Data Settings")]
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
            string myvalue = value.ToString();
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
                    this.Leading = 12;
                };";
        }

        public override void DrawMe(float printingTop, EbReport Rep, List<Param> Params, int slno)
        {
            float ury = Rep.HeightPt - (printingTop + TopPt + Rep.detailprintingtop);
            float lly = Rep.HeightPt - (printingTop + TopPt + HeightPt + Rep.detailprintingtop);
            string column_val = SummarizedValue.ToString();
            Phrase phrase = GetPhrase(column_val, (DbType)DbType, Rep.Font);
            ColumnText ct = new ColumnText(Rep.Canvas);
            ct.SetSimpleColumn(phrase, Llx, lly, Urx, ury, Leading, (int)TextAlign);
            ct.Go();
        }
    }

    [EnableInBuilder(BuilderType.Report)]
    public class EbDataFieldDateTimeSummary : EbDataFieldDateTime, IEbDataFieldSummary
    {
        [EnableInBuilder(BuilderType.Report)]
        [MetaOnly]
        public SummaryFunctionsDateTime Function { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        [PropertyGroup("Data Settings")]
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
            DateTime myvalue = Convert.ToDateTime(value);
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
        this.Leading = 12;
        };";
        }

        public override void DrawMe(float printingTop, EbReport Rep, List<Param> Params, int slno)
        {
            ColumnText ct = new ColumnText(Rep.Canvas);
            string column_val = FormatDate(SummarizedValue.ToString(), Format, Rep);
            if (Prefix != "" || Suffix != "")
                column_val = Prefix + " " + column_val + " " + Suffix;
            Phrase phrase = GetPhrase(column_val, (DbType)DbType, Rep.Font);

            ct.AddText(phrase);
            float ury = Rep.HeightPt - (printingTop + TopPt + Rep.detailprintingtop);
            float lly = Rep.HeightPt - (printingTop + TopPt + HeightPt + Rep.detailprintingtop);
            ct.SetSimpleColumn(Llx, lly, Urx, ury, Leading, (int)TextAlign);
            ct.Go();
        }
    }

    [EnableInBuilder(BuilderType.Report)]
    public class EbDataFieldBooleanSummary : EbDataFieldBoolean, IEbDataFieldSummary
    {
        [EnableInBuilder(BuilderType.Report)]
        [MetaOnly]
        public SummaryFunctionsBoolean Function { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        [PropertyGroup("Data Settings")]
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
        this.Leading = 12;
        };";
        }

        public override void DrawMe(float printingTop, EbReport Rep, List<Param> Params, int slno)
        {

            ColumnText ct = new ColumnText(Rep.Canvas);
            string column_val = SummarizedValue.ToString();
            if (Prefix != "" || Suffix != "")
                column_val = Prefix + " " + column_val + " " + Suffix;
            Phrase phrase = GetFormattedPhrase(this.Font, Rep.Font, column_val);

            if (!string.IsNullOrEmpty(LinkRefId))
            {
                Anchor a = CreateLink(phrase, LinkRefId, Rep.Doc, Params);
                Paragraph p = new Paragraph { a };
                p.Font = GetItextFont(this.Font, Rep.Font);
                ct.AddText(p);
            }
            else
                ct.AddText(phrase);
            float ury = Rep.HeightPt - (printingTop + TopPt + Rep.detailprintingtop);
            float lly = Rep.HeightPt - (printingTop + TopPt + HeightPt + Rep.detailprintingtop);
            ct.SetSimpleColumn(Llx, lly, Urx, ury, Leading, (int)TextAlign);
            ct.Go();
        }
    }

    [EnableInBuilder(BuilderType.Report)]
    public class EbCalcField : EbDataField
    {
        [EnableInBuilder(BuilderType.Report)]
        [PropertyGroup("Data Settings")]
        [PropertyEditor(PropertyEditorType.ScriptEditorCS)]
        [Alias("Value Expression")]
        public EbScript ValExpression { get; set; }

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
        public bool AmountInWords { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        [PropertyGroup("Data Settings")]
        public bool SuppressIfZero { get; set; }

        private string[] _dataFieldsUsed;
        public string[] DataFieldsUsedInCalc
        {
            get
            {
                if (_dataFieldsUsed == null)
                    if (ValExpression?.Code != null)
                    {
                        IEnumerable<string> matches = Regex.Matches(ValExpression.Code, @"T[0-9]{1}.\w+").OfType<Match>()
                             .Select(m => m.Groups[0].Value)
                             .Distinct();


                        _dataFieldsUsed = new string[matches.Count()];
                        int i = 0;
                        foreach (string match in matches)
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
        this.DecimalPlaces = 2;
        this.Leading = 12;
        };";
        }

        public override void DrawMe(float printingTop, EbReport Rep, List<Param> Params, int slno)
        {
            ColumnText ct = new ColumnText(Rep.Canvas);
            string column_val = string.Empty;
            EbDbTypes dbtype = EbDbTypes.String;
            EbPdfGlobals globals = new EbPdfGlobals
            {
                //CurrentField = this
            };

            Rep.AddParamsNCalcsInGlobal(globals);
            try
            {
                if (DataFieldsUsedInCalc != null && DataFieldsUsedInCalc.Count() > 0)
                    foreach (string datafd in DataFieldsUsedInCalc)
                    {
                        string TName = datafd.Split('.')[0];
                        int TableIndex = Convert.ToInt32(TName.Substring(1));
                        string fName = datafd.Split('.')[1];
                        int RowIndex = (TableIndex == Rep.DetailTableIndex) ? slno : 0;
                        globals[TName].Add(fName, new PdfNTV { Name = fName, Type = (PdfEbDbTypes)(int)Rep.DataSet.Tables[TableIndex].Columns[fName].Type, Value = Rep.DataSet.Tables[TableIndex].Rows[RowIndex][fName] });
                    }
                column_val = (Rep.ValueScriptCollection[Name].RunAsync(globals)).Result.ReturnValue.ToString();

                dbtype = (EbDbTypes)CalcFieldIntType;

                if (Rep.CalcValInRow.ContainsKey(Title))
                    Rep.CalcValInRow[Title] = new PdfNTV { Name = Title, Type = (PdfEbDbTypes)(int)dbtype, Value = column_val };
                else
                    Rep.CalcValInRow.Add(Title, new PdfNTV { Name = Title, Type = (PdfEbDbTypes)(int)dbtype, Value = column_val });
                Rep.AddParamsNCalcsInGlobal(globals);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message + e.StackTrace);

            }
            if (SuppressIfZero && !(Convert.ToDecimal(column_val) > 0))
                column_val = String.Empty;
            else
            {
                if (dbtype == EbDbTypes.Decimal)
                    column_val = FormatDecimals(column_val, AmountInWords, DecimalPlaces, Rep.CultureInfo.NumberFormat);
                if (Prefix != "" || Suffix != "")
                {
                    column_val = Prefix + " " + column_val + " " + Suffix;
                }
            }
            Phrase phrase = GetPhrase(column_val, (DbType)DbType, Rep.Font);

            if (!string.IsNullOrEmpty(LinkRefId))
            {
                Anchor a = CreateLink(phrase, LinkRefId, Rep.Doc, Params);
                Paragraph p = new Paragraph { a };
                p.Font = GetItextFont(this.Font, Rep.Font);
                ct.AddText(p);
            }
            else
            {
                ct.AddText(phrase);
            }

            float ury = Rep.HeightPt - (printingTop + TopPt + Rep.detailprintingtop);
            float lly = Rep.HeightPt - (printingTop + TopPt + HeightPt + Rep.detailprintingtop);
            ct.SetSimpleColumn(Llx, lly, Urx, ury, Leading, (int)TextAlign);
            ct.Go();
        }

        public dynamic GetCalcFieldValue(EbPdfGlobals globals, EbDataSet DataSet, int serialnumber, EbReport Rep)
        {
            dynamic value = null;
            foreach (string calcfd in this.DataFieldsUsedInCalc)
            {
                string TName = calcfd.Split('.')[0];
                string fName = calcfd.Split('.')[1];
                int tableindex = Convert.ToInt32(TName.Substring(1));
                globals[TName].Add(fName, new PdfNTV { Name = fName, Type = (PdfEbDbTypes)(int)DataSet.Tables[tableindex].Columns[fName].Type, Value = DataSet.Tables[tableindex].Rows[serialnumber][fName] });
            }
            value = (Rep.ValueScriptCollection[this.Name].RunAsync(globals)).Result.ReturnValue.ToString();
            return value;
        }
    }

    [EnableInBuilder(BuilderType.Report)]
    public class EbCalcFieldNumericSummary : EbCalcField, IEbDataFieldSummary
    {
        [EnableInBuilder(BuilderType.Report)]
        [MetaOnly]
        public SummaryFunctionsNumeric Function { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        [PropertyGroup("Data Settings")]
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
                    this.DecimalPlaces = 2;
                    this.Leading = 12;
                };";
        }

        public override void DrawMe(float printingTop, EbReport Rep, List<Param> Params, int slno)
        {
            float ury = Rep.HeightPt - (printingTop + TopPt + Rep.detailprintingtop);
            float lly = Rep.HeightPt - (printingTop + TopPt + HeightPt + Rep.detailprintingtop);
            string column_val = SummarizedValue.ToString();

            if (SuppressIfZero && !(Convert.ToDecimal(column_val) > 0))
                column_val = String.Empty;
            else
                column_val = FormatDecimals(column_val, AmountInWords, DecimalPlaces, Rep.CultureInfo.NumberFormat);

            if (Rep.SummaryValInRow.ContainsKey(Title))
                Rep.SummaryValInRow[Title] = new PdfNTV { Name = Title, Type = PdfEbDbTypes.Int32, Value = column_val };
            else
                Rep.SummaryValInRow.Add(Title, new PdfNTV { Name = Title, Type = PdfEbDbTypes.Int32, Value = column_val });
            Phrase phrase = GetPhrase(column_val, (DbType)DbType, Rep.Font);
            ColumnText ct = new ColumnText(Rep.Canvas);
            if (!string.IsNullOrEmpty(LinkRefId))
            {
                Anchor a = CreateLink(phrase, LinkRefId, Rep.Doc, Params);
                Paragraph p = new Paragraph { a };
                p.Font = GetItextFont(this.Font, Rep.Font);
                ct.AddText(p);
            }
            else
                ct.AddText(phrase);
            ct.SetSimpleColumn(Llx, lly, Urx, ury, Leading, (int)TextAlign);
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
        [PropertyGroup("Data Settings")]
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
            string myvalue = value.ToString();
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
                    this.Leading = 12;
                };";
        }

        public override void DrawMe(float printingTop, EbReport Rep, List<Param> Params, int slno)
        {
            float ury = Rep.HeightPt - (printingTop + TopPt + Rep.detailprintingtop);
            float lly = Rep.HeightPt - (printingTop + TopPt + HeightPt + Rep.detailprintingtop);
            string column_val = SummarizedValue.ToString();
            Phrase phrase = GetPhrase(column_val, (DbType)DbType, Rep.Font);
            ColumnText ct = new ColumnText(Rep.Canvas);
            if (!string.IsNullOrEmpty(LinkRefId))
            {
                Anchor a = CreateLink(phrase, LinkRefId, Rep.Doc, Params);
                Paragraph p = new Paragraph { a };
                p.Font = GetItextFont(this.Font, Rep.Font);
                ct.AddText(p);
            }
            else
                ct.AddText(phrase);
            ct.SetSimpleColumn(Llx, lly, Urx, ury, Leading, (int)TextAlign);
            ct.Go();
        }
    }

    [EnableInBuilder(BuilderType.Report)]
    public class EbCalcFieldDateTimeSummary : EbCalcField, IEbDataFieldSummary
    {
        [EnableInBuilder(BuilderType.Report)]
        [MetaOnly]
        public SummaryFunctionsDateTime Function { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        [PropertyGroup("Data Settings")]
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
            DateTime myvalue = Convert.ToDateTime(value);
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
                    this.Leading = 12;
                };";
        }

        public override void DrawMe(float printingTop, EbReport Rep, List<Param> Params, int slno)
        {
            float ury = Rep.HeightPt - (printingTop + TopPt + Rep.detailprintingtop);
            float lly = Rep.HeightPt - (printingTop + TopPt + HeightPt + Rep.detailprintingtop);
            string column_val = SummarizedValue.ToString();

            Phrase phrase = GetPhrase(column_val, (DbType)DbType, Rep.Font);
            ColumnText ct = new ColumnText(Rep.Canvas);
            if (!string.IsNullOrEmpty(LinkRefId))
            {
                Anchor a = CreateLink(phrase, LinkRefId, Rep.Doc, Params);
                Paragraph p = new Paragraph { a };
                p.Font = GetItextFont(this.Font, Rep.Font);
                ct.AddText(p);
            }
            else
                ct.AddText(phrase);
            ct.SetSimpleColumn(Llx, lly, Urx, ury, Leading, (int)TextAlign);
            ct.Go();
        }
    }

    [EnableInBuilder(BuilderType.Report)]
    public class EbCalcFieldBooleanSummary : EbCalcField, IEbDataFieldSummary
    {
        [EnableInBuilder(BuilderType.Report)]
        [MetaOnly]
        public SummaryFunctionsBoolean Function { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        [PropertyGroup("Data Settings")]
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
                    this.Leading = 12;
                };";
        }

        public override void DrawMe(float printingTop, EbReport Rep, List<Param> Params, int slno)
        {
            float ury = Rep.HeightPt - (printingTop + TopPt + Rep.detailprintingtop);
            float lly = Rep.HeightPt - (printingTop + TopPt + HeightPt + Rep.detailprintingtop);
            string column_val = SummarizedValue.ToString();
            Phrase phrase = GetFormattedPhrase(this.Font, Rep.Font, column_val);
            ColumnText ct = new ColumnText(Rep.Canvas);
            if (!string.IsNullOrEmpty(LinkRefId))
            {
                Anchor a = CreateLink(phrase, LinkRefId, Rep.Doc, Params);
                Paragraph p = new Paragraph { a };
                p.Font = GetItextFont(this.Font, Rep.Font);
                ct.AddText(p);
            }
            else
                ct.AddText(phrase);
            ct.SetSimpleColumn(Llx, lly, Urx, ury, Leading, (int)TextAlign);
            ct.Go();
        }
    }
}