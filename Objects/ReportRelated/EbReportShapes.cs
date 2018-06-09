using ExpressBase.Common;
using ExpressBase.Common.Extensions;
using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using iTextSharp.text.pdf;
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
        public new EbFont Font { get; set; }
    }

    [EnableInBuilder(BuilderType.Report)]
    public class EbCircle : EbReportFieldShape
    {
        public override string GetDesignHtml()
        {
            return "<div class='circle Ebshapes dropped' eb-type='Circle' id='@id' style='border-radius: 50%; border: @Border px solid; border-color: @BorderColor ; background-color:@BackColor ; width: @Width px; height: @Height px; position: absolute; left: @Left px; top: @Top px;'></div>".RemoveCR().DoubleQuoted();
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
            if (this.Height == this.Width)
            {
                float radius = this.WidthPt / 2;
                float xval = this.LeftPt + radius;
                float yval = reportHeight - (printingTop + this.TopPt + radius + report.detailprintingtop);

                canvas.SetColorStroke(GetColor(this.BorderColor));
                canvas.SetColorFill(GetColor(this.BackColor));
                canvas.SetLineWidth(this.Border);
                canvas.Circle(xval, yval, radius);
                canvas.FillStroke();
            }
            else
            {
                var x1 = this.LeftPt;
                var y1 = reportHeight - (printingTop + this.TopPt + this.HeightPt + report.detailprintingtop);
                var x2 = this.LeftPt + this.WidthPt;
                var y2 = reportHeight - (printingTop + this.TopPt + report.detailprintingtop);
                canvas.SetColorStroke(GetColor(this.BorderColor));
                canvas.SetColorFill(GetColor(this.BackColor));
                canvas.SetLineWidth(this.Border);
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
            return "<div class='rectangle Ebshapes dropped' eb-type='Rect' id='@id' style='border: @Border px solid; border-color: @BorderColor ;background-color:@BackColor ; color:@ForeColor ;width: @Width px; height: @Height px; position: absolute; left: @Left px; top: @Top px;'></div>".RemoveCR().DoubleQuoted();
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
            float x = this.LeftPt;
            float y = reportHeight - (printingTop + this.TopPt + this.HeightPt + report.detailprintingtop);
            float w = this.WidthPt;
            float h = this.HeightPt;
            canvas.SetColorStroke(GetColor(this.BorderColor));
            canvas.SetColorFill(GetColor(this.BackColor));
            canvas.SetLineWidth(this.Border);
            canvas.Rectangle(x, y, w, h);
            canvas.FillStroke();
        }
    }
    [EnableInBuilder(BuilderType.Report)]
    public class EbArrR : EbHl
    {
        public override string GetDesignHtml()
        {
            return "<div class='Ebshapes shape-horrizontal dropped' eb-type='ArrR' id='@id' style='border: @Border px solid; border-color: @BorderColor ; background-color:@BackColor ; color:@ForeColor ;width: @Width px; position: absolute; left: @Left px; top: @Top px;'><div class='arrow-right'></div></div>".RemoveCR().DoubleQuoted();
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
            var x = this.LeftPt + this.WidthPt;
            var y = reportHeight - (printingTop + this.TopPt + report.detailprintingtop);
            canvas.SetColorStroke(GetColor(this.BorderColor));
            canvas.SetColorFill(GetColor(this.BorderColor));
            canvas.SetLineWidth(this.Border);
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
            return "<div class='arrow Ebshapes shape-horrizontal dropped' eb-type='ArrL' id='@id' style='border: @Border px solid; border-color: @BorderColor ; background-color:@BackColor ; color:@ForeColor ; width: @Width px; position: absolute; left: @Left px; top: @Top px;'><div class='arrow-left'></div></div>".RemoveCR().DoubleQuoted();
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
            var x = this.LeftPt;
            var y = reportHeight - (printingTop + this.TopPt + report.detailprintingtop);
            canvas.SetColorStroke(GetColor(this.BorderColor));
            canvas.SetColorFill(GetColor(this.BorderColor));
            canvas.SetLineWidth(this.Border);
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
            return "<div class='Ebshapes shape-vertical dropped' eb-type='ArrD' id='@id' style='border: @Border px solid; border-color: @BorderColor ; background-color:@BackColor ; color:@ForeColor ; height: @Height px; position: absolute; left: @Left px; top: @Top px;'><div class='arrow-down'></div></div>".RemoveCR().DoubleQuoted();
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
            var x = this.LeftPt;
            var y = reportHeight - (printingTop + this.TopPt + this.HeightPt + report.detailprintingtop);
            canvas.SetColorStroke(GetColor(this.BorderColor));
            canvas.SetColorFill(GetColor(this.BorderColor));
            canvas.SetLineWidth(this.Border);
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
            return "<div class='Ebshapes shape-vertical dropped' $type='@type'eb-type='ArrU' id='@id' style='border: @Border px solid; border-color: @BorderColor ; background-color:@BackColor ; color:@ForeColor ; height: @Height px; 50px; position: absolute; left: @Left px; top: @Top px;'><div class='arrow-up'></div></div>".RemoveCR().DoubleQuoted();
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
            var x = this.LeftPt;
            var y = reportHeight - (printingTop + this.TopPt + report.detailprintingtop);
            canvas.SetColorStroke(GetColor(this.BorderColor));
            canvas.SetColorFill(GetColor(this.BorderColor));
            canvas.SetLineWidth(this.Border);
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
            return "<div class='Ebshapes shape-horrizontal dropped' eb-type='ByArrH' id='@id' style='border: @Border px solid; border-color: @BorderColor ; background-color:@BackColor ; color:@ForeColor ; width: @Width px; absolute: relative; left: @Left px; top: @Top px;'><div class='arrow-left'></div><div class='arrow-right'></div></div>".RemoveCR().DoubleQuoted();
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
            var x1 = this.LeftPt + this.WidthPt;
            var y1 = reportHeight - (printingTop + this.TopPt + report.detailprintingtop);
            canvas.SetColorStroke(GetColor(this.BorderColor));
            canvas.SetColorFill(GetColor(this.BorderColor));
            canvas.SetLineWidth(this.Border);
            canvas.MoveTo(x1, y1);
            canvas.LineTo(x1 - 3, y1 - 3);
            canvas.LineTo(x1 - 3, y1 + 3);
            canvas.ClosePathFillStroke();
            var x2 = this.LeftPt;
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
            return "<div class='Ebshapes shape-vertical dropped' eb-type='ByArrV' id='@id' style='border: @Border px solid; border-color: @BorderColor ; background-color:@BackColor ; color:@ForeColor ; height: @Height px; absolute: relative; left: @Left px; top: @Top px;'><div class='arrow-up'></div><div class='arrow-down'></div></div>".RemoveCR().DoubleQuoted();
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
            if (this.TopPt > report.MultiRowTop)
                rowH = report.RowHeight;
            base.DrawMe(canvas, reportHeight, printingTop, report);
            var x1 = this.LeftPt;
            var y1 = reportHeight - (printingTop + this.TopPt + report.detailprintingtop);
            canvas.SetColorStroke(GetColor(this.BorderColor));
            canvas.SetColorFill(GetColor(this.BorderColor));
            canvas.SetLineWidth(this.Border);
            canvas.MoveTo(x1, y1);
            canvas.LineTo(x1 + 3, y1 - 3);
            canvas.LineTo(x1 - 3, y1 - 3);
            canvas.ClosePathFillStroke();
            var x2 = this.LeftPt;
            var y2 = reportHeight - (printingTop + this.TopPt + this.HeightPt + report.detailprintingtop + rowH);
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
            return "<div class='Ebshapes shape-horrizontal dropped' eb-type='Hl' id='@id' style='border: @Border px solid; border-color: @BorderColor ; background-color:@BackColor ; color:@ForeColor ; width: @Width px; position: absolute; left: @Left px; top: @Top px;'></div>".RemoveCR().DoubleQuoted();
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
            if (this.TopPt > report.MultiRowTop)
                rowH = report.RowHeight;
            var x1 = this.LeftPt;
            var y1 = reportHeight - (printingTop + this.TopPt + report.detailprintingtop + rowH);
            var x2 = this.LeftPt + this.WidthPt;
            var y2 = y1;
            canvas.SetColorStroke(GetColor(this.BorderColor));
            canvas.SetLineWidth(this.Border);
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
            return "<div class='Ebshapes shape-vertical dropped' eb-type='Vl' id='@id' style='border: @Border px solid; border-color: @BorderColor ; background-color:@BackColor ; color:@ForeColor ; height: @Height px; position: absolute; left: @Left px; top: @Top px;'></div>".RemoveCR().DoubleQuoted();
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
            var x1 = this.LeftPt;
            var y1 = reportHeight - (printingTop + this.TopPt + report.detailprintingtop);
            var x2 = x1;
            var y2 = reportHeight - (printingTop + this.TopPt + this.HeightPt + report.detailprintingtop);
            canvas.SetColorStroke(GetColor(this.BorderColor));
            canvas.SetLineWidth(this.Border);
            canvas.MoveTo(x1, y1);
            canvas.LineTo(x2, y2);
            canvas.Stroke();
        }
    }
    [EnableInBuilder(BuilderType.Report)]
    public class EbTableLayout : EbReportFieldShape
    {
        [EnableInBuilder(BuilderType.Report)]
        public int ColoumCount { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        public int RowCount { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        [HideInPropertyGrid]
        public List<EbTableLayoutCell> CellCollection { get; set; }

        public override string GetDesignHtml()
        {
            return @"<div class='eb_table_container dropped' id='@id' eb-type='TableLayout' 
style='position: absolute;top: @Top px;left: @Left px;height: @Height px;width: @Width px;'>
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
