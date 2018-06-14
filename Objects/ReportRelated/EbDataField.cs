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

namespace ExpressBase.Objects.ReportRelated
{
    public abstract class EbDataField : EbReportField
    {
        public virtual void NotifyNewPage(bool status) { }

        [EnableInBuilder(BuilderType.Report)]
        public string SummaryOf { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        [PropertyGroup("General")]
        [UIproperty]
        public int DbType { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        [PropertyGroup("General")]
        [UIproperty]
        public int TableIndex { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        [PropertyGroup("General")]
        [UIproperty]
        public string ColumnName { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        [PropertyGroup("General")]
        [UIproperty]
        public string Prefix { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        [PropertyGroup("General")]
        [UIproperty]
        public string Suffix { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        [PropertyGroup("General")]
        [UIproperty]
        public Boolean RenderInMultiLine { get; set; } = true;

        [EnableInBuilder(BuilderType.Report)]
        [PropertyGroup("General")]
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
        public override void DrawMe(PdfContentByte canvas, float reportHeight, float printingTop, string column_val, float detailprintingtop, DbType column_type)
        {
            ColumnText ct = new ColumnText(canvas);
            Phrase text;

            if (this.Prefix != "" || this.Suffix != "")
            {
                column_val = this.Prefix + " " + column_val + " " + this.Suffix;
            }

            //if (this.Font == null)
            //    Font = (new EbFont { color = "#000000", Font = "Courier", Caps = false, Size = 10, Strikethrough = false, Style = 0, Underline = false });
            //    text = new Phrase(column_val);
            //else
            //{
            text = new Phrase(column_val, ITextFont);
            if (this.ForeColor != "")
                text.Font.Color = GetColor(this.ForeColor);//ct.Canvas.SetColorFill(GetColor(this.Color));
            //}

            if (this.RenderInMultiLine)
                column_val = RenderMultiLine(column_val, text, column_type);
            var ury = reportHeight - (printingTop + this.TopPt + detailprintingtop);
            var lly = reportHeight - (printingTop + this.TopPt + this.HeightPt + detailprintingtop);
            ct.SetSimpleColumn(text, this.LeftPt, lly, this.WidthPt + this.LeftPt, ury, 15, (int)TextAlign);
            ct.Go();
        }
        public string RenderMultiLine(string column_val, Phrase text, DbType column_type)
        {
            var calcbasefont = text.Font.GetCalculatedBaseFont(false);
            float stringwidth = calcbasefont.GetWidthPoint(column_val, text.Font.CalculatedSize);
            var charwidth = stringwidth / column_val.Length;
            int numberofCharsInALine = Convert.ToInt32(Math.Floor(this.WidthPt / charwidth));
            if (numberofCharsInALine < column_val.Length)
            {
                if (column_type == System.Data.DbType.Decimal)
                    column_val = "###";
            }
            return column_val;
        }
    }

    [EnableInBuilder(BuilderType.Report)]
    public class EbDataFieldText : EbDataField
    {

        public override string GetDesignHtml()
        {
            return "<div class='EbCol dropped' $type='@type' eb-type='DataFieldText' id='@id' style='border: @Border px solid;border-color: @BorderColor ; width: @Width px; background-color:@BackColor ; color:@ForeColor ; height: @Height px; position: absolute; left: @Left px; top: @Top px;text-align: @TextAlign ;'> @Title </div>".RemoveCR().DoubleQuoted();
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
    public class EbDataFieldDateTime : EbDataField
    {
        [EnableInBuilder(BuilderType.Report)]
        [UIproperty]
        [PropertyGroup("Appearance")]
        public DateFormatReport Format { get; set; }

        public override string GetDesignHtml()
        {
            return "<div class='EbCol dropped' $type='@type' eb-type='DataFieldDateTime' id='@id' style='border: @Border px solid;border-color: @BorderColor ; width: @Width px; background-color:@BackColor ; color:@ForeColor ; height: @Height px; position: absolute; left: @Left px; top: @Top px;text-align: @TextAlign;'> @Title </div>".RemoveCR().DoubleQuoted();
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
    public class EbDataFieldBoolean : EbDataField
    {
        public override string GetDesignHtml()
        {
            return "<div class='EbCol dropped' $type='@type' eb-type='DataFieldBoolean' id='@id' style='border: @Border px solid;border-color: @BorderColor ; width: @Width px; background-color:@BackColor ; color:@ForeColor ; height: @Height px; position: absolute; left: @Left px; top: @Top px;text-align: @TextAlign;'> @Title </div>".RemoveCR().DoubleQuoted();
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
        [PropertyGroup("General")]
        public int DecimalPlaces { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        [PropertyGroup("General")]
        public bool InLetters { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        [PropertyGroup("General")]
        public bool SuppressIfZero { get; set; }

        public override string GetDesignHtml()
        {
            return "<div class='EbCol dropped' $type='@type' eb-type='DataFieldNumeric' id='@id' style='border: @Border px solid;border-color: @BorderColor ; width: @Width px; background-color:@BackColor ; color:@ForeColor ; height: @Height px; position: absolute; left: @Left px; top: @Top px;text-align: @TextAlign;'> @Title </div>".RemoveCR().DoubleQuoted();
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
        public override void DrawMe(PdfContentByte canvas, float reportHeight, float printingTop, string column_val, float detailprintingtop, DbType column_type)
        {
            Phrase text;
            var ury = reportHeight - (printingTop + this.TopPt + detailprintingtop);
            var lly = reportHeight - (printingTop + this.TopPt + this.HeightPt + detailprintingtop);
            if (this.DecimalPlaces > 0)
                column_val = Convert.ToDecimal(column_val).ToString("F" + this.DecimalPlaces);
            if (this.InLetters)
            {
                NumberToEnglish numToE = new NumberToEnglish();
                column_val = numToE.changeCurrencyToWords(column_val);
            }

            if (this.Prefix != "" || this.Suffix != "")
            {
                column_val = this.Prefix + " " + column_val + " " + this.Suffix;
            }

            //if (this.Font == null)
            //    Font = (new EbFont { color = "#000000", Font = "Courier", Caps = false, Size = 10, Strikethrough = false, Style = 0, Underline = false });
            //    text = new Phrase(column_val);
            //else
            //{
            text = new Phrase(column_val, ITextFont);
            if (this.RenderInMultiLine)
                column_val = RenderMultiLine(column_val, text, column_type);
            if (this.ForeColor != "")
                text.Font.Color = GetColor(this.ForeColor);//ct.Canvas.SetColorFill(GetColor(this.Color));
            //}

            ColumnText ct = new ColumnText(canvas);
            ct.Canvas.SetColorFill(GetColor(this.ForeColor));
            ct.SetSimpleColumn(text, this.LeftPt, lly, this.WidthPt + this.LeftPt, ury, 15, (int)TextAlign);
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
                if (this.Function == SummaryFunctionsNumeric.Sum)
                    return this.Sum;
                else if (this.Function == SummaryFunctionsNumeric.Average && this.Count > 0)
                    return this.Sum / this.Count;
                else if (this.Function == SummaryFunctionsNumeric.Count)
                    return this.Count;
                else if (this.Function == SummaryFunctionsNumeric.Max)
                    return this.Max;
                else if (this.Function == SummaryFunctionsNumeric.Min)
                    return this.Min;

                return 0;
            }
        }

        public void Summarize(object value)
        {
            this.Count++;
            decimal myvalue = Convert.ToDecimal(value);

            if (this.Function == SummaryFunctionsNumeric.Sum || this.Function == SummaryFunctionsNumeric.Average)
            {
                if (this.Function == SummaryFunctionsNumeric.Sum || this.Function == SummaryFunctionsNumeric.Average)
                    this.Sum += myvalue;
            }

            if (this.Count > 1)
            {
                if (this.Function == SummaryFunctionsNumeric.Max)
                    this.Max = (this.Max > myvalue) ? this.Max : myvalue;
                else if (this.Function == SummaryFunctionsNumeric.Min)
                    this.Min = (this.Min < myvalue) ? this.Min : myvalue;
            }
            else
            {
                if (this.Function == SummaryFunctionsNumeric.Max)
                    this.Max = myvalue;
                else if (this.Function == SummaryFunctionsNumeric.Min)
                    this.Min = myvalue;
            }
        }

        public override void NotifyNewPage(bool status)
        {
            if (status && this.ResetOnNewPage)
                this.Sum = 0;
        }

        public override string GetDesignHtml()
        {
            return "<div class='dropped' $type='@type' eb-type='DataFieldNumericSummary' id='@id' style='border: @Border px solid;border-color: @BorderColor ; width: @Width px; background-color:@BackColor ; color:@ForeColor ; height: @Height px; position: absolute; left: @Left px; top: @Top px;text-align: @TextAlign;'> @Title </div>".RemoveCR().DoubleQuoted();
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

        public override void DrawMe(PdfContentByte canvas, float reportHeight, float printingTop, string column_val, float detailprintingtop, DbType column_type)
        {
            Phrase phrase;
            var ury = reportHeight - (printingTop + this.TopPt + detailprintingtop);
            var lly = reportHeight - (printingTop + this.TopPt + this.HeightPt + detailprintingtop);
            if (this.DecimalPlaces > 0)
                column_val = Convert.ToDecimal(column_val).ToString("F" + this.DecimalPlaces);
            if (this.InLetters)
            {
                NumberToEnglish numToE = new NumberToEnglish();
                column_val = numToE.changeCurrencyToWords(column_val);
            }
            //if (this.Font == null)
            //    Font = (new EbFont { color = "#000000", Font = "Courier", Caps = false, Size = 10, Strikethrough = false, Style = 0, Underline = false });
            // phrase = new Phrase(column_val);
            //else
            //{
            phrase = new Phrase(column_val, ITextFont);
            if (this.RenderInMultiLine)
                column_val = RenderMultiLine(column_val, phrase, column_type);
            if (this.ForeColor != "")
                phrase.Font.Color = GetColor(this.ForeColor);//ct.Canvas.SetColorFill(GetColor(this.Color));
            //}
            ColumnText ct = new ColumnText(canvas);
            ct.SetSimpleColumn(phrase, this.LeftPt, lly, this.WidthPt + this.LeftPt, ury, 15, (int)TextAlign);
            ct.Go();
        }
    }

    [EnableInBuilder(BuilderType.Report)]
    public class EbDataFieldTextSummary : EbDataFieldText, IEbDataFieldSummary
    {
        [EnableInBuilder(BuilderType.Report)]
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
                if (this.Function == SummaryFunctionsText.Count)
                    return this.Count;
                else if (this.Function == SummaryFunctionsText.Max)
                    return this.Max;
                else if (this.Function == SummaryFunctionsText.Min)
                    return this.Min;

                return 0;
            }
        }

        public void Summarize(object value)
        {
            var myvalue = value.ToString();
            this.Count++;
            if (this.Count > 1)
            {
                if (this.Function == SummaryFunctionsText.Max)
                    this.Max = (this.Max.CompareTo(myvalue) > 0) ? this.Max : myvalue;
                else if (this.Function == SummaryFunctionsText.Min)
                    this.Min = (this.Min.CompareTo(myvalue) > 0) ? myvalue : this.Min;
            }
            else
            {
                if (this.Function == SummaryFunctionsText.Max)
                    this.Max = myvalue;
                else if (this.Function == SummaryFunctionsText.Min)
                    this.Min = myvalue;
            }
        }

        public override string GetDesignHtml()
        {
            return "<div class='dropped' $type='@type' eb-type='DataFieldTextSummary' id='@id' style='border: @Border px solid;border-color: @BorderColor ; width: @Width px; background-color:@BackColor ; color:@ForeColor ; height: @Height px; position: absolute; left: @Left px; top: @Top px;text-align: @TextAlign;'> @Title </div>".RemoveCR().DoubleQuoted();
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
                if (this.Function == SummaryFunctionsDateTime.Count)
                    return this.Count;
                else if (this.Function == SummaryFunctionsDateTime.Max)
                    return this.Max;
                else if (this.Function == SummaryFunctionsDateTime.Min)
                    return this.Min;

                return 0;
            }
        }

        public void Summarize(object value)
        {
            var myvalue = Convert.ToDateTime(value);
            this.Count++;
            if (this.Count > 1)
            {
                if (this.Function == SummaryFunctionsDateTime.Max)
                    this.Max = (DateTime.Compare(this.Max, myvalue) > 0) ? this.Max : myvalue;
                if (this.Function == SummaryFunctionsDateTime.Min)
                    this.Min = (DateTime.Compare(this.Min, myvalue) > 0) ? myvalue : this.Min;
            }
            else
            {
                if (this.Function == SummaryFunctionsDateTime.Max)
                    this.Max = myvalue;
                if (this.Function == SummaryFunctionsDateTime.Min)
                    this.Min = myvalue;
            }
        }

        public override string GetDesignHtml()
        {
            return "<div class='dropped' $type='@type' eb-type='DataFieldDateTimeSummary' id='@id' style='border: @Border px solid;border-color: @BorderColor ; width: @Width px; background-color:@BackColor ; color:@ForeColor ; height: @Height px; position: absolute; left: @Left px; top: @Top px;text-align: @TextAlign;'> @Title </div>".RemoveCR().DoubleQuoted();
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
        public SummaryFunctionsBoolean Function { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        [HideInPropertyGrid]
        public bool ResetOnNewPage { get; set; }

        private int Count { get; set; }

        public object SummarizedValue
        {
            get
            {
                if (this.Function == SummaryFunctionsBoolean.Count)
                    return this.Count;
                return 0;
            }
        }

        public void Summarize(object value)
        {
            this.Count++;
        }

        public override string GetDesignHtml()
        {
            return "<div class='dropped' $type='@type' eb-type='DataFieldBooleanSummary' id='@id' style='border: @Border px solid;border-color: @BorderColor ; width: @Width px; background-color:@BackColor ; color:@ForeColor ; height: @Height px; position: absolute; left: @Left px; top: @Top px;text-align: @TextAlign ;'> @Title </div>".RemoveCR().DoubleQuoted();
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
        [PropertyGroup("General")]
        [UIproperty]
        [PropertyEditor(PropertyEditorType.ScriptEditorCS)]
        [JsonConverter(typeof(Base64Converter))]
        public string ValueExpression { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        [UIproperty]
        public string CalcFieldType { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        [PropertyGroup("General")]
        public int DecimalPlaces { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        [PropertyGroup("General")]
        public bool InLetters { get; set; }

        private string[] _dataFieldsUsed;
        public new string[] DataFieldsUsedCalc
        {
            get
            {
                if (_dataFieldsUsed == null)
                {
                    var matches = Regex.Matches(this.ValueExpression, @"T[0-9]{1}.\w+").OfType<Match>()
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
            return "<div class='dropped CalcField' $type='@type' cfType='@CalcFieldType ' eb-type='CalcField' id='@id' style='border: @Border px solid;border-color: @BorderColor ; width: @Width px; background-color:@BackColor ; color:@ForeColor ; height: @Height px; position: absolute; left: @Left px; top: @Top px;text-align: @TextAlign ;'> @Title </div>".RemoveCR().DoubleQuoted();
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

        public override void DrawMe(PdfContentByte canvas, float reportHeight, float printingTop, string column_val, float detailprintingtop, DbType column_type)
        {
            ColumnText ct = new ColumnText(canvas);
            Phrase text = null;

            if (column_val == "")
            {
                column_val = "0";
            }
            if (this.DecimalPlaces > 0)
                column_val = Convert.ToDecimal(column_val).ToString("F" + this.DecimalPlaces);
            if (this.InLetters)
            {
                NumberToEnglish numToE = new NumberToEnglish();
                column_val = numToE.changeCurrencyToWords(column_val);
            }

            if (this.Prefix != "" || this.Suffix != "")
            {
                column_val = this.Prefix + " " + column_val + " " + this.Suffix;
            }

            //if (this.Font == null)
            //    Font = (new EbFont { color = "#000000", Font = "Courier", Caps = false, Size = 10, Strikethrough = false, Style = 0, Underline = false });
            //    text = new Phrase(column_val);
            //else
            //{
            text = new Phrase(column_val, ITextFont);
            if (this.ForeColor != "")
                text.Font.Color = GetColor(this.ForeColor);//ct.Canvas.SetColorFill(GetColor(this.Color));
                                                           // }

            if (this.RenderInMultiLine)
                column_val = RenderMultiLine(column_val, text, column_type);

            var ury = reportHeight - (printingTop + this.TopPt + detailprintingtop);
            var lly = reportHeight - (printingTop + this.TopPt + this.HeightPt + detailprintingtop);
            ct.SetSimpleColumn(text, this.LeftPt, lly, this.WidthPt + this.LeftPt, ury, 15, (int)TextAlign);
            ct.Go();
        }
    }

    [EnableInBuilder(BuilderType.Report)]
    public class EbCalcFieldNumericSummary : EbCalcField, IEbDataFieldSummary
    {
        [EnableInBuilder(BuilderType.Report)]
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
                if (this.Function == SummaryFunctionsNumeric.Sum)
                    return this.Sum;
                else if (this.Function == SummaryFunctionsNumeric.Average && this.Count > 0)
                    return this.Sum / this.Count;
                else if (this.Function == SummaryFunctionsNumeric.Count)
                    return this.Count;
                else if (this.Function == SummaryFunctionsNumeric.Max)
                    return this.Max;
                else if (this.Function == SummaryFunctionsNumeric.Min)
                    return this.Min;

                return 0;
            }
        }

        public void Summarize(object value)
        {
            this.Count++;
            decimal myvalue = Convert.ToDecimal(value);

            if (this.Function == SummaryFunctionsNumeric.Sum || this.Function == SummaryFunctionsNumeric.Average)
            {
                if (this.Function == SummaryFunctionsNumeric.Sum || this.Function == SummaryFunctionsNumeric.Average)
                    this.Sum += myvalue;
            }

            if (this.Count > 1)
            {
                if (this.Function == SummaryFunctionsNumeric.Max)
                    this.Max = (this.Max > myvalue) ? this.Max : myvalue;
                else if (this.Function == SummaryFunctionsNumeric.Min)
                    this.Min = (this.Min < myvalue) ? this.Min : myvalue;
            }
            else
            {
                if (this.Function == SummaryFunctionsNumeric.Max)
                    this.Max = myvalue;
                else if (this.Function == SummaryFunctionsNumeric.Min)
                    this.Min = myvalue;
            }
        }

        public override void NotifyNewPage(bool status)
        {
            if (status && this.ResetOnNewPage)
                this.Sum = 0;
        }

        public override string GetDesignHtml()
        {
            return "<div class='dropped' $type='@type' eb-type='CalcFieldNumericSummary' id='@id' style='border: @Border px solid;border-color: @BorderColor ; width: @Width px; background-color:@BackColor ; color:@ForeColor ; height: @Height px; position: absolute; left: @Left px; top: @Top px;text-align: @TextAlign;'> @Title </div>".RemoveCR().DoubleQuoted();
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

        public override void DrawMe(PdfContentByte canvas, float reportHeight, float printingTop, string column_val, float detailprintingtop, DbType column_type)
        {
            Phrase phrase;
            var ury = reportHeight - (printingTop + this.TopPt + detailprintingtop);
            var lly = reportHeight - (printingTop + this.TopPt + this.HeightPt + detailprintingtop);
            if (this.DecimalPlaces > 0)
                column_val = Convert.ToDecimal(column_val).ToString("F" + this.DecimalPlaces);
            if (this.InLetters)
            {
                NumberToEnglish numToE = new NumberToEnglish();
                column_val = numToE.changeCurrencyToWords(column_val);
            }
            //if (this.Font == null)
            //    Font = (new EbFont { color = "#000000", Font = "Courier", Caps = false, Size = 10, Strikethrough = false, Style = 0, Underline = false });
            //    phrase = new Phrase(column_val);
            //else
            //{
            phrase = new Phrase(column_val, ITextFont);
            if (this.ForeColor != "")
                phrase.Font.Color = GetColor(this.ForeColor);//ct.Canvas.SetColorFill(GetColor(this.Color));
                                                             //}
            if (this.RenderInMultiLine)
                column_val = RenderMultiLine(column_val, phrase, column_type);
            ColumnText ct = new ColumnText(canvas);
            ct.SetSimpleColumn(phrase, this.LeftPt, lly, this.WidthPt + this.LeftPt, ury, 15, (int)TextAlign);
            ct.Go();
        }

    }

    [EnableInBuilder(BuilderType.Report)]
    public class EbCalcFieldTextSummary : EbCalcField, IEbDataFieldSummary
    {
        [EnableInBuilder(BuilderType.Report)]
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
                if (this.Function == SummaryFunctionsText.Count)
                    return this.Count;
                else if (this.Function == SummaryFunctionsText.Max)
                    return this.Max;
                else if (this.Function == SummaryFunctionsText.Min)
                    return this.Min;

                return 0;
            }
        }

        public void Summarize(object value)
        {
            var myvalue = value.ToString();
            this.Count++;
            if (this.Count > 1)
            {
                if (this.Function == SummaryFunctionsText.Max)
                    this.Max = (this.Max.CompareTo(myvalue) > 0) ? this.Max : myvalue;
                else if (this.Function == SummaryFunctionsText.Min)
                    this.Min = (this.Min.CompareTo(myvalue) > 0) ? myvalue : this.Min;
            }
            else
            {
                if (this.Function == SummaryFunctionsText.Max)
                    this.Max = myvalue;
                else if (this.Function == SummaryFunctionsText.Min)
                    this.Min = myvalue;
            }
        }

        public override string GetDesignHtml()
        {
            return "<div class='dropped' eb-type='CalcFieldTextSummary' id='@id' style='border: @Border px solid;border-color: @BorderColor ; width: @Width px; background-color:@BackColor ; color:@ForeColor ; height: @Height px; position: absolute; left: @Left px; top: @Top px;text-align: @TextAlign;'> @Title </div>".RemoveCR().DoubleQuoted();
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
                if (this.Function == SummaryFunctionsDateTime.Count)
                    return this.Count;
                else if (this.Function == SummaryFunctionsDateTime.Max)
                    return this.Max;
                else if (this.Function == SummaryFunctionsDateTime.Min)
                    return this.Min;

                return 0;
            }
        }

        public void Summarize(object value)
        {
            var myvalue = Convert.ToDateTime(value);
            this.Count++;
            if (this.Count > 1)
            {
                if (this.Function == SummaryFunctionsDateTime.Max)
                    this.Max = (DateTime.Compare(this.Max, myvalue) > 0) ? this.Max : myvalue;
                if (this.Function == SummaryFunctionsDateTime.Min)
                    this.Min = (DateTime.Compare(this.Min, myvalue) > 0) ? myvalue : this.Min;
            }
            else
            {
                if (this.Function == SummaryFunctionsDateTime.Max)
                    this.Max = myvalue;
                if (this.Function == SummaryFunctionsDateTime.Min)
                    this.Min = myvalue;
            }
        }

        public override string GetDesignHtml()
        {
            return "<div class='dropped' eb-type='CalcFieldDatetimeSummary' id='@id' style='border: @Border px solid;border-color: @BorderColor ; width: @Width px; background-color:@BackColor ; color:@ForeColor ; height: @Height px; position: absolute; left: @Left px; top: @Top px;text-align: @TextAlign;'> @Title </div>".RemoveCR().DoubleQuoted();
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
        public SummaryFunctionsBoolean Function { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        [HideInPropertyGrid]
        public bool ResetOnNewPage { get; set; }

        private int Count { get; set; }

        public object SummarizedValue
        {
            get
            {
                if (this.Function == SummaryFunctionsBoolean.Count)
                    return this.Count;
                return 0;
            }
        }

        public void Summarize(object value)
        {
            this.Count++;
        }

        public override string GetDesignHtml()
        {
            return "<div class='dropped' eb-type='CalcFieldBooleanSummary' id='@id' style='border: @Border px solid;border-color: @BorderColor ; width: @Width px; background-color:@BackColor ; color:@ForeColor ; height: @Height px; position: absolute; left: @Left px; top: @Top px;text-align: @TextAlign ;'> @Title </div>".RemoveCR().DoubleQuoted();
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