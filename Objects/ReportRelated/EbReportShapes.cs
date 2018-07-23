using ExpressBase.Common;
using ExpressBase.Common.Extensions;
using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using iTextSharp.text.pdf;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExpressBase.Objects.ReportRelated
{
    public abstract class EbReportFieldShape : EbReportField
    {
        [EnableInBuilder(BuilderType.Report)]
        [HideInPropertyGrid]
        public override EbFont Font { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        [JsonIgnore]
        public override EbTextAlign TextAlign { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        [HideInPropertyGrid]
        public override string Title { get; set; }
    }

    [EnableInBuilder(BuilderType.Report)]
    public class EbCircle : EbReportFieldShape
    {
        public override string GetDesignHtml()
        {
            return "<div class='circle Ebshapes dropped' eb-type='Circle' id='@id' style='border-radius: 50%; border: @Border px solid; border-color: @BorderColor ; background-color:@BackColor ; width: @Width px; height: @Height px; left: @Left px; top: @Top px;'></div>".RemoveCR().DoubleQuoted();
        }

        public override string GetJsInitFunc()
        {
            return @"
    this.Init = function(id)
        {
    this.Height =50;
    this.Width= 50;
    this.Border = 1;
    this.BorderColor = '#000000'
};";
        }
        public override void DrawMe(PdfContentByte canvas, float reportHeight, float printingTop, EbReport report)
        {
            if (Height == Width)
            {
                float radius = WidthPt / 2;
                float xval = LeftPt + radius;
                float yval = reportHeight - (printingTop + TopPt + radius + report.detailprintingtop);

                canvas.SetColorStroke(GetColor(BorderColor));
                canvas.SetColorFill(GetColor(BackColor));
                canvas.SetLineWidth(Border);
                canvas.Circle(xval, yval, radius);
                canvas.FillStroke();
            }
            else
            {
                var x1 = LeftPt;
                var y1 = reportHeight - (printingTop + TopPt + HeightPt + report.detailprintingtop);
                var x2 = LeftPt + WidthPt;
                var y2 = reportHeight - (printingTop + TopPt + report.detailprintingtop);
                canvas.SetColorStroke(GetColor(BorderColor));
                canvas.SetColorFill(GetColor(BackColor));
                canvas.SetLineWidth(Border);
                canvas.Ellipse(x1, y1, x2, y2);
                canvas.FillStroke();
            }
        }
    }
    [EnableInBuilder(BuilderType.Report)]
    public class EbRect : EbReportFieldShape
    {
        public override string GetDesignHtml()
        {
            return "<div class='rectangle Ebshapes dropped' eb-type='Rect' id='@id' style='border: @Border px solid; border-color: @BorderColor ;background-color:@BackColor ; color:@ForeColor ;width: @Width px; height: @Height px; left: @Left px; top: @Top px;'></div>".RemoveCR().DoubleQuoted();
        }
        public override string GetJsInitFunc()
        {
            return @"
    this.Init = function(id)
        {
    this.Height =50;
    this.Width= 50;
    this.Border = 1;
    this.BorderColor = '#000000'
};";
        }
        public override void DrawMe(PdfContentByte canvas, float reportHeight, float printingTop, EbReport report)
        {
            float x = LeftPt;
            float y = reportHeight - (printingTop + TopPt + HeightPt + report.detailprintingtop);
            float w = WidthPt;
            float h = HeightPt;
            canvas.SetColorStroke(GetColor(BorderColor));
            canvas.SetColorFill(GetColor(BackColor));
            canvas.SetLineWidth(Border);
            canvas.Rectangle(x, y, w, h);
            canvas.FillStroke();
        }
    }
    [EnableInBuilder(BuilderType.Report)]
    public class EbArrR : EbHl
    {
        public override string GetDesignHtml()
        {
            return "<div class='Ebshapes shape-horrizontal dropped' eb-type='ArrR' id='@id' style='border: @Border px solid; border-color: @BorderColor ; background-color:@BackColor ; color:@ForeColor ;width: @Width px; left: @Left px; top: @Top px;'><div class='arrow-right'></div></div>".RemoveCR().DoubleQuoted();
        }
        public override string GetJsInitFunc()
        {
            return @"
    this.Init = function(id)
        {
    this.Width= 50;
    this.Border = 1;
    this.BorderColor = '#000000'
};";
        }
        public override void DrawMe(PdfContentByte canvas, float reportHeight, float printingTop, EbReport report)
        {
            base.DrawMe(canvas, reportHeight, printingTop, report);
            var x = LeftPt + WidthPt;
            var y = reportHeight - (printingTop + TopPt + report.detailprintingtop);
            canvas.SetColorStroke(GetColor(BorderColor));
            canvas.SetColorFill(GetColor(BorderColor));
            canvas.SetLineWidth(Border);
            canvas.MoveTo(x, y);
            canvas.LineTo(x - 3, y - 3);
            canvas.LineTo(x - 3, y + 3);
            canvas.ClosePathFillStroke();
        }
    }
    [EnableInBuilder(BuilderType.Report)]
    public class EbArrL : EbHl
    {
        public override string GetDesignHtml()
        {
            return "<div class='arrow Ebshapes shape-horrizontal dropped' eb-type='ArrL' id='@id' style='border: @Border px solid; border-color: @BorderColor ; background-color:@BackColor ; color:@ForeColor ; width: @Width px; left: @Left px; top: @Top px;'><div class='arrow-left'></div></div>".RemoveCR().DoubleQuoted();
        }
        public override string GetJsInitFunc()
        {
            return @"
    this.Init = function(id)
        {
    this.Width= 50;
    this.Border = 1;
    this.BorderColor = '#000000'
};";
        }
        public override void DrawMe(PdfContentByte canvas, float reportHeight, float printingTop, EbReport report)
        {
            base.DrawMe(canvas, reportHeight, printingTop, report);
            var x = LeftPt;
            var y = reportHeight - (printingTop + TopPt + report.detailprintingtop);
            canvas.SetColorStroke(GetColor(BorderColor));
            canvas.SetColorFill(GetColor(BorderColor));
            canvas.SetLineWidth(Border);
            canvas.MoveTo(x, y);
            canvas.LineTo(x + 3, y + 3);
            canvas.LineTo(x + 3, y - 3);
            canvas.ClosePathFillStroke();
        }


    }
    [EnableInBuilder(BuilderType.Report)]
    public class EbArrD : EbVl
    {
        public override string GetDesignHtml()
        {
            return "<div class='Ebshapes shape-vertical dropped' eb-type='ArrD' id='@id' style='border: @Border px solid; border-color: @BorderColor ; background-color:@BackColor ; color:@ForeColor ; height: @Height px; left: @Left px; top: @Top px;'><div class='arrow-down'></div></div>".RemoveCR().DoubleQuoted();
        }
        public override string GetJsInitFunc()
        {
            return @"
    this.Init = function(id)
        {
    this.Height =50;
    this.Border = 1;
    this.BorderColor = '#000000'
};";
        }
        public override void DrawMe(PdfContentByte canvas, float reportHeight, float printingTop, EbReport report)
        {
            base.DrawMe(canvas, reportHeight, printingTop, report);
            var x = LeftPt;
            var y = reportHeight - (printingTop + TopPt + HeightPt + report.detailprintingtop);
            canvas.SetColorStroke(GetColor(BorderColor));
            canvas.SetColorFill(GetColor(BorderColor));
            canvas.SetLineWidth(Border);
            canvas.MoveTo(x, y);
            canvas.LineTo(x - 3, y + 3);
            canvas.LineTo(x + 3, y + 3);
            canvas.ClosePathFillStroke();
        }
    }
    [EnableInBuilder(BuilderType.Report)]
    public class EbArrU : EbVl
    {
        public override string GetDesignHtml()
        {
            return "<div class='Ebshapes shape-vertical dropped' $type='@type'eb-type='ArrU' id='@id' style='border: @Border px solid; border-color: @BorderColor ; background-color:@BackColor ; color:@ForeColor ; height: @Height px; 50px; left: @Left px; top: @Top px;'><div class='arrow-up'></div></div>".RemoveCR().DoubleQuoted();
        }
        public override string GetJsInitFunc()
        {
            return @"
    this.Init = function(id)
        {
    this.Height =50;
    this.Border = 1;
    this.BorderColor = '#000000'
};";
        }
        public override void DrawMe(PdfContentByte canvas, float reportHeight, float printingTop, EbReport report)
        {
            base.DrawMe(canvas, reportHeight, printingTop, report);
            var x = LeftPt;
            var y = reportHeight - (printingTop + TopPt + report.detailprintingtop);
            canvas.SetColorStroke(GetColor(BorderColor));
            canvas.SetColorFill(GetColor(BorderColor));
            canvas.SetLineWidth(Border);
            canvas.MoveTo(x, y);
            canvas.LineTo(x + 3, y - 3);
            canvas.LineTo(x - 3, y - 3);
            canvas.ClosePathFillStroke();
        }

    }

    [EnableInBuilder(BuilderType.Report)]
    public class EbByArrH : EbHl
    {
        public override string GetDesignHtml()
        {
            return "<div class='Ebshapes shape-horrizontal dropped' eb-type='ByArrH' id='@id' style='border: @Border px solid; border-color: @BorderColor ; background-color:@BackColor ; color:@ForeColor ; width: @Width px; left: @Left px; top: @Top px;'><div class='arrow-left'></div><div class='arrow-right'></div></div>".RemoveCR().DoubleQuoted();
        }
        public override string GetJsInitFunc()
        {
            return @"
    this.Init = function(id)
        {
    this.Width= 50;
    this.Border = 1;
    this.BorderColor = '#000000'
};";
        }
        public override void DrawMe(PdfContentByte canvas, float reportHeight, float printingTop, EbReport report)
        {
            base.DrawMe(canvas, reportHeight, printingTop, report);
            var x1 = LeftPt + WidthPt;
            var y1 = reportHeight - (printingTop + TopPt + report.detailprintingtop);
            canvas.SetColorStroke(GetColor(BorderColor));
            canvas.SetColorFill(GetColor(BorderColor));
            canvas.SetLineWidth(Border);
            canvas.MoveTo(x1, y1);
            canvas.LineTo(x1 - 3, y1 - 3);
            canvas.LineTo(x1 - 3, y1 + 3);
            canvas.ClosePathFillStroke();
            var x2 = LeftPt;
            var y2 = y1;
            canvas.MoveTo(x2, y2);
            canvas.LineTo(x2 + 3, y2 + 3);
            canvas.LineTo(x2 + 3, y2 - 3);
            canvas.ClosePathFillStroke();
        }
    }
    [EnableInBuilder(BuilderType.Report)]
    public class EbByArrV : EbVl
    {
        public override string GetDesignHtml()
        {
            return "<div class='Ebshapes shape-vertical dropped' eb-type='ByArrV' id='@id' style='border: @Border px solid; border-color: @BorderColor ; background-color:@BackColor ; color:@ForeColor ; height: @Height px; left: @Left px; top: @Top px;'><div class='arrow-up'></div><div class='arrow-down'></div></div>".RemoveCR().DoubleQuoted();
        }
        public override string GetJsInitFunc()
        {
            return @"
    this.Init = function(id)
        {
    this.Height =50;
    this.Border = 1;
    this.BorderColor = '#000000'
};";
        }
        public override void DrawMe(PdfContentByte canvas, float reportHeight, float printingTop, EbReport report)
        {
            float rowH = 0;
            if (TopPt > report.MultiRowTop)
                rowH = report.RowHeight;
            base.DrawMe(canvas, reportHeight, printingTop, report);
            var x1 = LeftPt;
            var y1 = reportHeight - (printingTop + TopPt + report.detailprintingtop);
            canvas.SetColorStroke(GetColor(BorderColor));
            canvas.SetColorFill(GetColor(BorderColor));
            canvas.SetLineWidth(Border);
            canvas.MoveTo(x1, y1);
            canvas.LineTo(x1 + 3, y1 - 3);
            canvas.LineTo(x1 - 3, y1 - 3);
            canvas.ClosePathFillStroke();
            var x2 = LeftPt;
            var y2 = reportHeight - (printingTop + TopPt + HeightPt + report.detailprintingtop + rowH);
            canvas.MoveTo(x2, y2);
            canvas.LineTo(x2 - 3, y2 + 3);
            canvas.LineTo(x2 + 3, y2 + 3);
            canvas.ClosePathFillStroke();
        }
    }

    [EnableInBuilder(BuilderType.Report)]
    public class EbHl : EbReportFieldShape
    {
        public override string GetDesignHtml()
        {
            return "<div class='Ebshapes shape-horrizontal dropped' eb-type='Hl' id='@id' style='border: @Border px solid; border-color: @BorderColor ; background-color:@BackColor ; color:@ForeColor ; width: @Width px; left: @Left px; top: @Top px;'></div>".RemoveCR().DoubleQuoted();
        }
        public override string GetJsInitFunc()
        {
            return @"
    this.Init = function(id)
        {
    this.Width= 50;
    this.Border = 1;
    this.BorderColor = '#000000'
};";
        }
        public override void DrawMe(PdfContentByte canvas, float reportHeight, float printingTop, EbReport report)
        {
            float rowH = 0;
            if (TopPt > report.MultiRowTop)
                rowH = report.RowHeight;
            var x1 = LeftPt;
            var y1 = reportHeight - (printingTop + TopPt + report.detailprintingtop + rowH);
            var x2 = LeftPt + WidthPt;
            var y2 = y1;
            canvas.SetColorStroke(GetColor(BorderColor));
            canvas.SetLineWidth(Border);
            canvas.MoveTo(x1, y1);
            canvas.LineTo(x2, y2);
            canvas.Stroke();
        }
    }

    [EnableInBuilder(BuilderType.Report)]
    public class EbVl : EbReportFieldShape
    {
        public override string GetDesignHtml()
        {
            return "<div class='Ebshapes shape-vertical dropped' eb-type='Vl' id='@id' style='border: @Border px solid; border-color: @BorderColor ; background-color:@BackColor ; color:@ForeColor ; height: @Height px; left: @Left px; top: @Top px;'></div>".RemoveCR().DoubleQuoted();
        }
        public override string GetJsInitFunc()
        {
            return @"
    this.Init = function(id)
        {
    this.Height =50;
    this.Border = 1;
    this.BorderColor = '#000000'
};";
        }
        public override void DrawMe(PdfContentByte canvas, float reportHeight, float printingTop, EbReport report)
        {
            var x1 = LeftPt;
            var y1 = reportHeight - (printingTop + TopPt + report.detailprintingtop);
            var x2 = x1;
            var y2 = reportHeight - (printingTop + TopPt + HeightPt + report.detailprintingtop);
            canvas.SetColorStroke(GetColor(BorderColor));
            canvas.SetLineWidth(Border);
            canvas.MoveTo(x1, y1);
            canvas.LineTo(x2, y2);
            canvas.Stroke();
        }
    }
    [EnableInBuilder(BuilderType.Report)]
    public class EbTableLayout : EbReportFieldShape
    {
        [EnableInBuilder(BuilderType.Report)]
        [DefaultPropValue("3")]
        [PropertyGroup("Dimensions")]
        public int ColoumCount { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        [DefaultPropValue("1")]
        [PropertyGroup("Dimensions")]
        public int RowCount { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        [HideInPropertyGrid]
        public List<EbTableLayoutCell> CellCollection { get; set; }

        public override string GetDesignHtml()
        {
            return @"<div class='eb_table_container dropped' id='@id' eb-type='TableLayout' 
style='top: @Top px;left: @Left px;height: @Height px;width: @Width px;'>
<table onclick='$(this).parent().click();' style='border: @Border px solid ; border-color: @BorderColor ;' class='table eb_table_layout'>
<tr><td eb-type='TableLayout'></td><td eb-type='TableLayout'></td><td eb-type='TableLayout'></td></tr>
</table><div class='eb_draggbale_table_handle' onclick='$(this).parent().focus();'><i class='fa fa-arrows'></i></div></div>".RemoveCR().DoubleQuoted();
        }
        public override string GetJsInitFunc()
        {
            return @"
    this.Init = function(id)
        {
    this.Width= 300;
    this.Border = 1;
    this.BorderColor = '#eeeeee';
};";
        }
    }
}
