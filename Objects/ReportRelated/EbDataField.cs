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

namespace ExpressBase.Objects.ReportRelated
{
    public abstract class EbDataField : EbReportField
    {
        public virtual void NotifyNewPage(bool status) { }

        public void DrawMe(PdfContentByte canvas, float reportHeight, float printingTop, float detailprintingtop, string column_val)
        {
            var ury = reportHeight - (printingTop + this.Top + detailprintingtop);
            var lly = reportHeight - (printingTop + this.Top + this.Height + detailprintingtop);

            ColumnText ct = new ColumnText(canvas);
            ct.SetSimpleColumn(new Phrase(column_val), this.Left, lly, this.Width + this.Left, ury, 15, Element.ALIGN_LEFT);
            ct.Go();
        }
    }

    [EnableInBuilder(BuilderType.Report)]
    public class EbDataFieldText : EbDataField
    {

        public override string GetDesignHtml()
        {
            return "<div class='EbCol dropped' $type='@type' eb-type='DataFieldText' id='@id' style='border: @Border px solid;border-color: @BorderColor ; width: @Width px; background-color:@BackColor ; color:@ForeColor ; height: @Height px; position: absolute; left: @Left px; top: @Top px;'> @Title </div>".RemoveCR().DoubleQuoted();
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
    this.BorderColor = '#aaaaaa'
};";
        }
    }

    [EnableInBuilder(BuilderType.Report)]
    public class EbDataFieldDateTime : EbDataField
    {
        public override string GetDesignHtml()
        {
            return "<div class='EbCol dropped' $type='@type' eb-type='DataFieldDateTime' id='@id' style='border: @Border px solid;border-color: @BorderColor ; width: @Width px; background-color:@BackColor ; color:@ForeColor ; height: @Height px; position: absolute; left: @Left px; top: @Top px;'> @Title </div>".RemoveCR().DoubleQuoted();
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
    this.BorderColor = '#aaaaaa'
};";
        }
    }

    [EnableInBuilder(BuilderType.Report)]
    public class EbDataFieldBoolean : EbDataField
    {
        public override string GetDesignHtml()
        {
            return "<div class='EbCol dropped' $type='@type' eb-type='DataFieldBoolean' id='@id' style='border: @Border px solid;border-color: @BorderColor ; width: @Width px; background-color:@BackColor ; color:@ForeColor ; height: @Height px; position: absolute; left: @Left px; top: @Top px;'> @Title </div>".RemoveCR().DoubleQuoted();
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
    this.BorderColor = '#aaaaaa'
};";
        }
    }

    [EnableInBuilder(BuilderType.Report)]
    public class EbDataFieldNumeric : EbDataField
    {
        public override string GetDesignHtml()
        {
            return "<div class='EbCol dropped' $type='@type' eb-type='DataFieldNumeric' id='@id' style='border: @Border px solid;border-color: @BorderColor ; width: @Width px; background-color:@BackColor ; color:@ForeColor ; height: @Height px; position: absolute; left: @Left px; top: @Top px;'> @Title </div>".RemoveCR().DoubleQuoted();
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
    this.BorderColor = '#aaaaaa'
};";
        }
    }

    [EnableInBuilder(BuilderType.Report)]
    public class EbDataFieldNumericSummary : EbDataFieldNumeric
    {
        [EnableInBuilder(BuilderType.Report)]
        [HideInPropertyGrid]
        public string DataField { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        public SummaryFunctionsNumeric Function { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        [HideInPropertyGrid]
        public bool ResetOnNewPage { get; set; }

        private int Count { get; set; }

        private decimal Sum { get; set; }

        private decimal Max { get; set; }

        private decimal Min { get; set; }

        public decimal SummarizedValue
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

        public void Summarize(dynamic value)
        {
            this.Count++;
            value = Convert.ToDecimal(value);

            if (this.Function == SummaryFunctionsNumeric.Sum || this.Function == SummaryFunctionsNumeric.Average)
            {
                if (this.Function == SummaryFunctionsNumeric.Sum || this.Function == SummaryFunctionsNumeric.Average)
                    this.Sum += value;
            }

            if (this.Count > 1)
            {
                if (this.Function == SummaryFunctionsNumeric.Max)
                    this.Max = (this.Max > value) ? this.Max : value;
                else if (this.Function == SummaryFunctionsNumeric.Min)
                    this.Min = (this.Min < value) ? this.Min : value;
            }
            else
            {
                if (this.Function == SummaryFunctionsNumeric.Max)
                    this.Max = value;
                else if (this.Function == SummaryFunctionsNumeric.Min)
                    this.Min = value;
            }
        }

        public override void NotifyNewPage(bool status)
        {
            if (status && this.ResetOnNewPage)
                this.Sum = 0;
        }

        public override string GetDesignHtml()
        {
            return "<div class='dropped' $type='@type' eb-type='DataFieldNumericSummary' id='@id' style='border: @Border px solid;border-color: @BorderColor ; width: @Width px; background-color:@BackColor ; color:@ForeColor ; height: @Height px; position: absolute; left: @Left px; top: @Top px;'> @Title </div>".RemoveCR().DoubleQuoted();
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
    this.BorderColor = '#aaaaaa'
};";
        }
    }

    [EnableInBuilder(BuilderType.Report)]
    public class EbDataFieldTextSummary : EbDataFieldText
    {
        [EnableInBuilder(BuilderType.Report)]
        [HideInPropertyGrid]
        public string DataField { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        public SummaryFunctionsText Function { get; set; }


        [EnableInBuilder(BuilderType.Report)]
        [HideInPropertyGrid]
        public bool ResetOnNewPage { get; set; }

        private int Count { get; set; }

        private string Max { get; set; } = "";

        private string Min { get; set; } = "";

        public dynamic SummarizedValue
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

        public void Summarize(dynamic value)
        {
            value = value.ToString();
            this.Count++;
            if (this.Count > 1)
            {
                if (this.Function == SummaryFunctionsText.Max)
                    this.Max = (this.Max.CompareTo(value) > 0) ? this.Max : value;
                else if (this.Function == SummaryFunctionsText.Min)
                    this.Min = (this.Min.CompareTo(value) > 0) ? value : this.Min;
            }
            else
            {
                if (this.Function == SummaryFunctionsText.Max)
                    this.Max = value;
                else if (this.Function == SummaryFunctionsText.Min)
                    this.Min = value;
            }
        }

        public override string GetDesignHtml()
        {
            return "<div class='dropped' $type='@type' eb-type='DataFieldTextSummary' id='@id' style='border: @Border px solid;border-color: @BorderColor ; width: @Width px; background-color:@BackColor ; color:@ForeColor ; height: @Height px; position: absolute; left: @Left px; top: @Top px;'> @Title </div>".RemoveCR().DoubleQuoted();
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
    this.BorderColor = '#aaaaaa'
};";
        }
    }

    [EnableInBuilder(BuilderType.Report)]
    public class EbDataFieldDateTimeSummary : EbDataFieldDateTime
    {
        [EnableInBuilder(BuilderType.Report)]
        [HideInPropertyGrid]
        public string DataField { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        public SummaryFunctionsDateTime Function { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        [HideInPropertyGrid]
        public bool ResetOnNewPage { get; set; }

        private int Count { get; set; }

        private DateTime Max { get; set; }

        private DateTime Min { get; set; }

        public dynamic SummarizedValue
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

        public void Summarize(dynamic value)
        {
            value = Convert.ToDateTime(value);
            this.Count++;
            if (this.Count > 1)
            {
                if (this.Function == SummaryFunctionsDateTime.Max)
                    this.Max = (DateTime.Compare(this.Max, value) > 0) ? this.Max : value;
                if (this.Function == SummaryFunctionsDateTime.Min)
                    this.Min = (DateTime.Compare(this.Min, value) > 0) ? value : this.Min;
            }
            else
            {
                if (this.Function == SummaryFunctionsDateTime.Max)
                    this.Max = value;
                if (this.Function == SummaryFunctionsDateTime.Min)
                    this.Min = value;
            }
        }

        public override string GetDesignHtml()
        {
            return "<div class='dropped' $type='@type' eb-type='DataFieldDateTimeSummary' id='@id' style='border: @Border px solid;border-color: @BorderColor ; width: @Width px; background-color:@BackColor ; color:@ForeColor ; height: @Height px; position: absolute; left: @Left px; top: @Top px;'> @Title </div>".RemoveCR().DoubleQuoted();
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
    this.BorderColor = '#aaaaaa'
};";
        }
    }

    [EnableInBuilder(BuilderType.Report)]
    public class EbDataFieldBooleanSummary : EbDataFieldBoolean
    {
        [EnableInBuilder(BuilderType.Report)]
        [HideInPropertyGrid]
        public string DataField { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        public SummaryFunctionsBoolean Function { get; set; }


        [EnableInBuilder(BuilderType.Report)]
        [HideInPropertyGrid]
        public bool ResetOnNewPage { get; set; }

        private int Count { get; set; }

        public decimal SummarizedValue
        {
            get
            {
                if (this.Function == SummaryFunctionsBoolean.Count)
                    return this.Count;
                return 0;
            }
        }

        public void Summarize()
        {
                this.Count++;
        }

        public override string GetDesignHtml()
        {
            return "<div class='dropped' $type='@type' eb-type='DataFieldBooleanSummary' id='@id' style='border: @Border px solid;border-color: @BorderColor ; width: @Width px; background-color:@BackColor ; color:@ForeColor ; height: @Height px; position: absolute; left: @Left px; top: @Top px;'> @Title </div>".RemoveCR().DoubleQuoted();
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
    this.BorderColor = '#aaaaaa'
};";
        }
    }
}
