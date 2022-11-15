using ExpressBase.Common.Extensions;
using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Data;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using ExpressBase.Common;
using ExpressBase.Common.Structures;
using ExpressBase.Common.Data;
using System.Globalization;
using ExpressBase.CoreBase.Globals;
using System.IO;
using Microsoft.CodeAnalysis.Scripting;

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
                        IEnumerable<string> matches = Regex.Matches(this.AppearExpression.Code, @"T[0-9]{1}\.\w+").OfType<Match>()
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

            if (column_val != string.Empty)
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
                            if (!this.RenderInMultiLine && (column_type == System.Data.DbType.Int32 || column_type == System.Data.DbType.Decimal || column_type == System.Data.DbType.Double || column_type == System.Data.DbType.Int16 || column_type == System.Data.DbType.Int64 || column_type == System.Data.DbType.VarNumeric))
                                column_val = "###";

                            else if (!this.RenderInMultiLine)
                                column_val = column_val.Substring(0, numberofCharsInALine - 2) + "...";
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

        public string FormatDecimals(string column_val, bool _inWords, int _decimalPlaces, NumberFormatInfo _numberFormat, bool formatUsingCulture, string decimalCurrency)
        {
            if (_inWords)
            {
                //NumberToEnglishOld numToE = new NumberToEnglishOld();
                //column_val = numToE.changeCurrencyToWords(column_val); 
                column_val = NumberToWords.ConvertNumber(column_val, decimalCurrency, _decimalPlaces);
            }
            else
            {
                if (formatUsingCulture)
                    column_val = Convert.ToDecimal(column_val).ToString("N", _numberFormat);
                if (_numberFormat.NumberDecimalDigits != _decimalPlaces)
                    column_val = Convert.ToDecimal(column_val).ToString("F" + _decimalPlaces);
            }
            return column_val;
        }

        public void DoRenderInMultiLine(string column_val, EbReport Report, bool _inwords)
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
                if (!_inwords && (datatype == System.Data.DbType.Decimal || datatype == System.Data.DbType.Double || datatype == System.Data.DbType.Int16 || datatype == System.Data.DbType.Int32 || datatype == System.Data.DbType.Int64 || datatype == System.Data.DbType.VarNumeric))
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
        [EnableInBuilder(BuilderType.Report)]
        [PropertyGroup("Appearance")]
        [UIproperty]
        public bool RenderAsHtml { get; set; }

        public override string GetDesignHtml()
        {
            return @"<div class='EbCol dropped' $type='@type' eb-type='DataFieldText' id='@id' style='border: @Border px solid;border-color: @BorderColor ; width: @Width px; background-color:@BackColor ; color:@ForeColor ; height: @Height px; left: @Left px; top: @Top px;text-align: @TextAlign ;'> 
                        <div onclick='$(this).parent().focus()' class='dropped-inner'> @Title </div> 
                     </div>".RemoveCR().DoubleQuoted();
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

            if (RenderAsHtml)
            {
                using (StringReader sr = new StringReader(column_val))
                {
                    var elements = iTextSharp.text.html.simpleparser.HtmlWorker.ParseToList(sr, null);
                    foreach (IElement e in elements)
                    {
                        ct.AddElement(e);
                    }
                }
            }
            else if (!string.IsNullOrEmpty(LinkRefId))
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
            return @"<div class='EbCol dropped' $type='@type' eb-type='DataFieldDateTime' id='@id' style='border: @Border px solid;border-color: @BorderColor ; width: @Width px; background-color:@BackColor ; color:@ForeColor ; height: @Height px; left: @Left px; top: @Top px;text-align: @TextAlign;'> 
                        <div onclick='$(this).parent().focus()' class='dropped-inner'> @Title </div>  
                    </div>".RemoveCR().DoubleQuoted();
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
            if (column_val != string.Empty)
            {
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
    }

    [EnableInBuilder(BuilderType.Report)]
    public class EbDataFieldBoolean : EbDataField
    {
        public override string GetDesignHtml()
        {
            return @"<div class='EbCol dropped' $type='@type' eb-type='DataFieldBoolean' id='@id' style='border: @Border px solid;border-color: @BorderColor ; width: @Width px; background-color:@BackColor ; color:@ForeColor ; height: @Height px; left: @Left px; top: @Top px;text-align: @TextAlign;'>
                        <div onclick='$(this).parent().focus()' class='dropped-inner'> @Title </div>  
                    </div>".RemoveCR().DoubleQuoted();
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
        public string DecimalCurrency { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        [PropertyGroup("Data Settings")]
        public bool SuppressIfZero { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        [PropertyGroup("Data Settings")]
        public bool FormatUsingCulture { get; set; } = true;

        [EnableInBuilder(BuilderType.Report)]
        [PropertyGroup("Data Settings")]
        public int DecimalPlaces { get; set; } = 2;

        public override string GetDesignHtml()
        {
            return @"<div class='EbCol dropped' $type='@type' eb-type='DataFieldNumeric' id='@id' style='border: @Border px solid;border-color: @BorderColor ; width: @Width px; background-color:@BackColor ; color:@ForeColor ; height: @Height px; left: @Left px; top: @Top px;text-align: @TextAlign;'> 
                        <div onclick='$(this).parent().focus()' class='dropped-inner'> @Title </div>  
                    </div>".RemoveCR().DoubleQuoted();
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
            if (SuppressIfZero && !(Convert.ToDecimal(column_val) > 0))
                column_val = String.Empty;
            else
            {
                column_val = FormatDecimals(column_val, AmountInWords, DecimalPlaces, Rep.CultureInfo.NumberFormat, FormatUsingCulture, DecimalCurrency);
                if (Prefix != "" || Suffix != "")
                    column_val = Prefix + " " + column_val + " " + Suffix;
            }
            Phrase phrase = GetPhrase(column_val, (DbType)DbType, Rep.Font);
            if (RenderInMultiLine && AmountInWords)
                DoRenderInMultiLine(column_val, Rep, this.AmountInWords);

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


            float ury = Rep.HeightPt - (printingTop + TopPt + Rep.detailprintingtop);
            float lly = Rep.HeightPt - (printingTop + TopPt + HeightPt + Rep.detailprintingtop + Rep.RowHeight);

            ct.SetSimpleColumn(Llx, lly, Urx, ury, Leading, (int)TextAlign);
            ct.Go();
        }

    }

    public interface IEbDataFieldSummary
    {
        object SummarizedValue { get; }
        void Summarize(object value);
        void ResetSummary();
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

        public void ResetSummary()
        {
            if (Function == SummaryFunctionsNumeric.Sum)
                Sum = 0;
            else if (Function == SummaryFunctionsNumeric.Average && this.Count > 0)
                Sum = Count = 0;
            else if (Function == SummaryFunctionsNumeric.Count)
                Count = 0;
            else if (Function == SummaryFunctionsNumeric.Max)
                Max = 0;
            else if (Function == SummaryFunctionsNumeric.Min)
                Min = 0;
        }
        public override void NotifyNewPage(bool status)
        {
            if (status && ResetOnNewPage)
                Sum = 0;
        }

        public override string GetDesignHtml()
        {
            return @"<div class='dropped' $type='@type' eb-type='DataFieldNumericSummary' id='@id' style='border: @Border px solid;border-color: @BorderColor ; width: @Width px; background-color:@BackColor ; color:@ForeColor ; height: @Height px; left: @Left px; top: @Top px;text-align: @TextAlign;'> 
                        <div onclick='$(this).parent().focus()' class='dropped-inner'> @Title </div>  
                    </div>".RemoveCR().DoubleQuoted();
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
            ResetSummary();
            column_val = FormatDecimals(column_val, AmountInWords, DecimalPlaces, Rep.CultureInfo.NumberFormat, FormatUsingCulture, DecimalCurrency);

            if (Rep.SummaryValInRow.ContainsKey(Title))
                Rep.SummaryValInRow[Title] = new GNTV { Name = Title.Replace(".", "_"), Type = GlobalDbType.Int32, Value = column_val };
            else
                Rep.SummaryValInRow.Add(Title, new GNTV { Name = Title.Replace(".", "_"), Type = GlobalDbType.Int32, Value = column_val });

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

        public void ResetSummary()
        {
            if (Function == SummaryFunctionsText.Count)
                Count = 0;
            else if (Function == SummaryFunctionsText.Max)
                Max = string.Empty;
            else if (Function == SummaryFunctionsText.Min)
                Min = string.Empty;
        }

        public override string GetDesignHtml()
        {
            return @"<div class='dropped' $type='@type' eb-type='DataFieldTextSummary' id='@id' style='border: @Border px solid;border-color: @BorderColor ; width: @Width px; background-color:@BackColor ; color:@ForeColor ; height: @Height px; left: @Left px; top: @Top px;text-align: @TextAlign;'> 
                        <div onclick='$(this).parent().focus()' class='dropped-inner'> @Title </div> 
                    </div>".RemoveCR().DoubleQuoted();
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
            ResetSummary();
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

        public void ResetSummary()
        {
            if (Function == SummaryFunctionsDateTime.Count)
                Count = 0;
            else if (Function == SummaryFunctionsDateTime.Max)
                Max = DateTime.MinValue;
            else if (Function == SummaryFunctionsDateTime.Min)
                Min = DateTime.MinValue;
        }
        public override string GetDesignHtml()
        {
            return @"<div class='dropped' $type='@type' eb-type='DataFieldDateTimeSummary' id='@id' style='border: @Border px solid;border-color: @BorderColor ; width: @Width px; background-color:@BackColor ; color:@ForeColor ; height: @Height px; left: @Left px; top: @Top px;text-align: @TextAlign;'> 
                        <div onclick='$(this).parent().focus()' class='dropped-inner'> @Title </div>  
                    </div>".RemoveCR().DoubleQuoted();
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
            ResetSummary();
            if (column_val != string.Empty)
            {
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

        public void ResetSummary()
        {
            if (Function == SummaryFunctionsBoolean.Count)
                Count = 0;
        }
        public override string GetDesignHtml()
        {
            return @"<div class='dropped' $type='@type' eb-type='DataFieldBooleanSummary' id='@id' style='border: @Border px solid;border-color: @BorderColor ; width: @Width px; background-color:@BackColor ; color:@ForeColor ; height: @Height px; left: @Left px; top: @Top px;text-align: @TextAlign ;'> 
                        <div onclick='$(this).parent().focus()' class='dropped-inner'> @Title </div> 
                    </div>".RemoveCR().DoubleQuoted();
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
            ResetSummary();
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

        [EnableInBuilder(BuilderType.Report)]
        [PropertyGroup("Data Settings")]
        public bool FormatUsingCulture { get; set; } = true;

        private string[] _dataFieldsUsed;
        public string[] DataFieldsUsedInCalc
        {
            get
            {
                if (_dataFieldsUsed == null)
                    if (ValExpression?.Code != null)
                    {
                        IEnumerable<string> matches = Regex.Matches(ValExpression.Code, @"T[0-9]{1}\.\w+").OfType<Match>()
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
            return @"<div class='dropped CalcField' $type='@type' cfType='@CalcFieldType ' eb-type='CalcField' id='@id' style='border: @Border px solid;border-color: @BorderColor ; width: @Width px; background-color:@BackColor ; color:@ForeColor ; height: @Height px; left: @Left px; top: @Top px;text-align: @TextAlign ;'> 
                        <div onclick='$(this).parent().focus()' class='dropped-inner'> @Title </div>
                    </div>".RemoveCR().DoubleQuoted();
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
            EbPdfGlobals globals = new EbPdfGlobals();
            try
            {
                if (Rep.EvaluatorVersion == EvaluatorVersion.Version_1)
                {
                    column_val = Rep.ExecuteExpressionV1((Script)Rep.ValueScriptCollection[Name], slno, globals, DataFieldsUsedInCalc).ToString();
                }
                else
                {
                    column_val = Rep.ExecuteExpressionV2(Rep.ValueScriptCollection[Name].ToString(), slno, globals, DataFieldsUsedInCalc, true).ToString();
                }

                dbtype = (EbDbTypes)CalcFieldIntType;

                if (Rep.CalcValInRow.ContainsKey(Title))
                    Rep.CalcValInRow[Title] = new GNTV { Name = Title, Type = (GlobalDbType)(int)dbtype, Value = column_val };
                else
                    Rep.CalcValInRow.Add(Title, new GNTV { Name = Title, Type = (GlobalDbType)(int)dbtype, Value = column_val });
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
                    column_val = FormatDecimals(column_val, AmountInWords, DecimalPlaces, Rep.CultureInfo.NumberFormat, FormatUsingCulture, string.Empty);
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

        public string GetCalcFieldValue(EbPdfGlobals globals, EbDataSet DataSet, int serialnumber, EbReport Rep)
        {
            string value;
            if (Rep.EvaluatorVersion == EvaluatorVersion.Version_1)
            {
                value = Rep.ExecuteExpressionV1((Script)Rep.ValueScriptCollection[this.Name], serialnumber, globals, DataFieldsUsedInCalc)?.ToString();
            }
            else
            {
                value = Rep.ExecuteExpressionV2(Rep.ValueScriptCollection[Name].ToString(), serialnumber, globals, DataFieldsUsedInCalc, true)?.ToString();
            }
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

        public void ResetSummary()
        {
            if (Function == SummaryFunctionsNumeric.Sum)
                Sum = 0;
            else if (Function == SummaryFunctionsNumeric.Average && Count > 0)
                Sum = Count = 0;
            else if (Function == SummaryFunctionsNumeric.Count)
                Count = 0;
            else if (Function == SummaryFunctionsNumeric.Max)
                Max = 0;
            else if (Function == SummaryFunctionsNumeric.Min)
                Min = 0;
        }
        public override void NotifyNewPage(bool status)
        {
            if (status && ResetOnNewPage)
                Sum = 0;
        }

        public override string GetDesignHtml()
        {
            return @"<div class='dropped' $type='@type' eb-type='CalcFieldNumericSummary' id='@id' style='border: @Border px solid;border-color: @BorderColor ; width: @Width px; background-color:@BackColor ; color:@ForeColor ; height: @Height px; left: @Left px; top: @Top px;text-align: @TextAlign;'> 
                        <div onclick='$(this).parent().focus()' class='dropped-inner'> @Title </div>
                    </div>".RemoveCR().DoubleQuoted();
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
            ResetSummary();

            if (SuppressIfZero && !(Convert.ToDecimal(column_val) > 0))
                column_val = string.Empty;
            else
                column_val = FormatDecimals(column_val, AmountInWords, DecimalPlaces, Rep.CultureInfo.NumberFormat, FormatUsingCulture, string.Empty);

            if (Rep.SummaryValInRow.ContainsKey(Title))
                Rep.SummaryValInRow[Title] = new GNTV { Name = Title.Replace(".", "_"), Type = GlobalDbType.Int32, Value = column_val };
            else
                Rep.SummaryValInRow.Add(Title, new GNTV { Name = Title.Replace(".", "_"), Type = GlobalDbType.Int32, Value = column_val });
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

        public void ResetSummary()
        {
            if (Function == SummaryFunctionsText.Count)
                Count = 0;
            else if (Function == SummaryFunctionsText.Max)
                Max = string.Empty;
            else if (Function == SummaryFunctionsText.Min)
                Min = string.Empty;
        }
        public override string GetDesignHtml()
        {
            return @"<div class='dropped' eb-type='CalcFieldTextSummary' id='@id' style='border: @Border px solid;border-color: @BorderColor ; width: @Width px; background-color:@BackColor ; color:@ForeColor ; height: @Height px; left: @Left px; top: @Top px;text-align: @TextAlign;'> 
                        <div onclick='$(this).parent().focus()' class='dropped-inner'> @Title </div>
                    </div>".RemoveCR().DoubleQuoted();
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
            ResetSummary();
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

        public void ResetSummary()
        {
            if (Function == SummaryFunctionsDateTime.Count)
                Count = 0;
            else if (Function == SummaryFunctionsDateTime.Max)
                Max = DateTime.MinValue;
            else if (Function == SummaryFunctionsDateTime.Min)
                Min = DateTime.MinValue;
        }
        public override string GetDesignHtml()
        {
            return @"<div class='dropped' eb-type='CalcFieldDatetimeSummary' id='@id' style='border: @Border px solid;border-color: @BorderColor ; width: @Width px; background-color:@BackColor ; color:@ForeColor ; height: @Height px; left: @Left px; top: @Top px;text-align: @TextAlign;'> 
                        <div onclick='$(this).parent().focus()' class='dropped-inner'> @Title </div>
                    </div>".RemoveCR().DoubleQuoted();
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
            ResetSummary();
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

        public void ResetSummary()
        {
            if (Function == SummaryFunctionsBoolean.Count)
                Count = 0;
        }

        public override string GetDesignHtml()
        {
            return @"<div class='dropped' eb-type='CalcFieldBooleanSummary' id='@id' style='border: @Border px solid;border-color: @BorderColor ; width: @Width px; background-color:@BackColor ; color:@ForeColor ; height: @Height px; left: @Left px; top: @Top px;text-align: @TextAlign ;'> 
                        <div onclick='$(this).parent().focus()' class='dropped-inner'> @Title </div>
                    </div>".RemoveCR().DoubleQuoted();
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
            ResetSummary();
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