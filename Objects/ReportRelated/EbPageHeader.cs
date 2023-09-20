using DocumentFormat.OpenXml.Wordprocessing;
using ExpressBase.Common.Extensions;
using ExpressBase.Common.Objects.Attributes;
using ExpressBase.Common.Objects;
using System;
using System.Collections.Generic;
using System.Reflection.PortableExecutable;
using System.Text;
using ExpressBase.Common.Data;

namespace ExpressBase.Objects
{
    [EnableInBuilder(BuilderType.Report)]
    public class EbPageHeader : EbReportSection
    {
        public EbReport EbReport { get; internal set; }

        //private float ph_Yposition { get; set; }

        private void DrawFields(EbReportField field, float section_Yposition, int iterator)
        {
            if (!field.IsHidden)
            {
                List<Param> RowParams = null;
                if (field is EbDataField field_org)
                {
                    if (field.IsGroupSummaryField || field.IsPageSummaryField || field.IsReportSummaryField)
                        this.EbReport.CallSummerize(field_org, iterator);
                    if (field.IsInAppearanceScriptCollection)
                        this.EbReport.RunAppearanceExpression(field_org, iterator);
                    if (!string.IsNullOrEmpty(field_org.LinkRefId))
                        RowParams = this.EbReport.CreateRowParamForLink(field_org, iterator);
                }
                field.DrawMe(section_Yposition, this.EbReport, RowParams, iterator);
            }
        }

        public void Draw()
        {
            this.EbReport.RowHeight = 0;
            this.EbReport.MultiRowTop = 0;
            this.EbReport.detailprintingtop = 0;
            if (this.EbReport.PageNumber == 1)
                this.EbReport.ph_Yposition = this.EbReport.ReportHeaderHeight + this.EbReport.Margin.Top;
            else if (this.EbReport.ReportHeaderHeightRepeatAsPH > 0)
                this.EbReport.ph_Yposition = this.EbReport.ReportHeaderHeightRepeatAsPH + this.EbReport.Margin.Top;
            else
                this.EbReport.ph_Yposition = this.EbReport.Margin.Top;

            foreach (EbReportField field in this.GetFields())
            {
                DrawFields(field, this.EbReport.ph_Yposition, 0);
            }

            this.EbReport.ph_Yposition += this.HeightPt;
        }

        public override void AfterRedisGet()
        {
            foreach (EbReportField field in this.GetFields())
            {
                field.OwningSection = this;
                field.AfterRedisGet();
            }
        }

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
}
