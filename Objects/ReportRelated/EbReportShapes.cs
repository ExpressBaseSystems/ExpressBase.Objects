using ExpressBase.Common;
using ExpressBase.Common.Data;
using ExpressBase.Common.Extensions;
using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using iTextSharp.text.pdf;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExpressBase.Objects
{
    public abstract class EbReportFieldShape : EbReportField
    {
        public override EbFont Font { get; set; }

        public override EbTextAlign TextAlign { get; set; }

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
        public override void DrawMe(float printingTop, EbReport Rep, List<Param> Linkparams, int slno)
        {
            if (Height == Width)
            {
                float radius = WidthPt / 2;
                float xval = LeftPt + radius;
                float yval = Rep.HeightPt - (printingTop + TopPt + radius + Rep.detailprintingtop);

                Rep.Canvas.SetColorStroke(GetColor(BorderColor));
                Rep.Canvas.SetColorFill(GetColor(BackColor));
                Rep.Canvas.SetLineWidth(Border);
                Rep.Canvas.Circle(xval, yval, radius);
                Rep.Canvas.FillStroke();
            }
            else
            {
                float y1 = Rep.HeightPt - (printingTop + TopPt + HeightPt + Rep.detailprintingtop);
                float y2 = Rep.HeightPt - (printingTop + TopPt + Rep.detailprintingtop);
                Rep.Canvas.SetColorStroke(GetColor(BorderColor));
                Rep.Canvas.SetColorFill(GetColor(BackColor));
                Rep.Canvas.SetLineWidth(Border);
                Rep.Canvas.Ellipse(Llx, y1, Urx, y2);
                Rep.Canvas.FillStroke();
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
        public override void DrawMe(float printingTop, EbReport Rep, List<Param> Linkparams, int slno)
        {
            float y = Rep.HeightPt - (printingTop + TopPt + HeightPt + Rep.detailprintingtop);
            Rep.Canvas.SetColorStroke(GetColor(BorderColor));
            Rep.Canvas.SetColorFill(GetColor(BackColor));
            Rep.Canvas.SetLineWidth(Border);
            Rep.Canvas.Rectangle(Llx, y, WidthPt, HeightPt);
            Rep.Canvas.FillStroke();
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
        public override void DrawMe(float printingTop, EbReport Rep, List<Param> Linkparams, int slno)
        {
            base.DrawMe(printingTop, Rep, Linkparams, slno);
            float y = Rep.HeightPt - (printingTop + TopPt + Rep.detailprintingtop);
            Rep.Canvas.SetColorStroke(GetColor(BorderColor));
            Rep.Canvas.SetColorFill(GetColor(BorderColor));
            Rep.Canvas.SetLineWidth(Border);
            Rep.Canvas.MoveTo(Urx, y);
            Rep.Canvas.LineTo(Urx - 3, y - 3);
            Rep.Canvas.LineTo(Urx - 3, y + 3);
            Rep.Canvas.ClosePathFillStroke();
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
        public override void DrawMe(float printingTop, EbReport Rep, List<Param> Linkparams, int slno)
        {
            base.DrawMe(printingTop,Rep, Linkparams, slno);
            float y = Rep.HeightPt - (printingTop + TopPt + Rep.detailprintingtop);
            Rep.Canvas.SetColorStroke(GetColor(BorderColor));
            Rep.Canvas.SetColorFill(GetColor(BorderColor));
            Rep.Canvas.SetLineWidth(Border);
            Rep.Canvas.MoveTo(Llx, y);
            Rep.Canvas.LineTo(Llx + 3, y + 3);
            Rep.Canvas.LineTo(Llx + 3, y - 3);
            Rep.Canvas.ClosePathFillStroke();
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
        public override void DrawMe(float printingTop, EbReport Rep, List<Param> Linkparams, int slno)
        {
            base.DrawMe(printingTop,  Rep, Linkparams, slno);
            float y = Rep.HeightPt - (printingTop + TopPt + HeightPt + Rep.detailprintingtop);
            Rep.Canvas.SetColorStroke(GetColor(BorderColor));
            Rep.Canvas.SetColorFill(GetColor(BorderColor));
            Rep.Canvas.SetLineWidth(Border);
            Rep.Canvas.MoveTo(Llx, y);
            Rep.Canvas.LineTo(Llx - 3, y + 3);
            Rep.Canvas.LineTo(Llx + 3, y + 3);
            Rep.Canvas.ClosePathFillStroke();
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
        public override void DrawMe(float printingTop, EbReport Rep, List<Param> Linkparams, int slno)
        {
            base.DrawMe(printingTop,Rep, Linkparams, slno);
            float y = Rep.HeightPt - (printingTop + TopPt + Rep.detailprintingtop);
            Rep.Canvas.SetColorStroke(GetColor(BorderColor));
            Rep.Canvas.SetColorFill(GetColor(BorderColor));
            Rep.Canvas.SetLineWidth(Border);
            Rep.Canvas.MoveTo(Llx, y);
            Rep.Canvas.LineTo(Llx + 3, y - 3);
            Rep.Canvas.LineTo(Llx - 3, y - 3);
            Rep.Canvas.ClosePathFillStroke();
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
        public override void DrawMe(float printingTop, EbReport Rep, List<Param> Linkparams, int slno)
        {
            base.DrawMe(printingTop, Rep, Linkparams, slno);
            float y1 = Rep.HeightPt - (printingTop + TopPt + Rep.detailprintingtop);
            Rep.Canvas.SetColorStroke(GetColor(BorderColor));
            Rep.Canvas.SetColorFill(GetColor(BorderColor));
            Rep.Canvas.SetLineWidth(Border);
            Rep.Canvas.MoveTo(Urx, y1);
            Rep.Canvas.LineTo(Urx - 3, y1 - 3);
            Rep.Canvas.LineTo(Urx - 3, y1 + 3);
            Rep.Canvas.ClosePathFillStroke();
            float y2 = y1;
            Rep.Canvas.MoveTo(Llx, y2);
            Rep.Canvas.LineTo(Llx + 3, y2 + 3);
            Rep.Canvas.LineTo(Llx + 3, y2 - 3);
            Rep.Canvas.ClosePathFillStroke();
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
        public override void DrawMe(float printingTop, EbReport Rep, List<Param> Linkparams, int slno)
        {
            float rowH = (TopPt > Rep.MultiRowTop) ? Rep.RowHeight : 0;
            base.DrawMe(printingTop, Rep, Linkparams, slno);
            float y1 = Rep.HeightPt - (printingTop + TopPt + Rep.detailprintingtop);
            Rep.Canvas.SetColorStroke(GetColor(BorderColor));
            Rep.Canvas.SetColorFill(GetColor(BorderColor));
            Rep.Canvas.SetLineWidth(Border);
            Rep.Canvas.MoveTo(Llx, y1);
            Rep.Canvas.LineTo(Llx + 3, y1 - 3);
            Rep.Canvas.LineTo(Llx - 3, y1 - 3);
            Rep.Canvas.ClosePathFillStroke();
            float y2 = Rep.HeightPt - (printingTop + TopPt + HeightPt + Rep.detailprintingtop + rowH);
            Rep.Canvas.MoveTo(Llx, y2);
            Rep.Canvas.LineTo(Llx - 3, y2 + 3);
            Rep.Canvas.LineTo(Llx + 3, y2 + 3);
            Rep.Canvas.ClosePathFillStroke();
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
        public override void DrawMe(float printingTop, EbReport Rep, List<Param> Linkparams,int slno)
        {
            float rowH = (TopPt > Rep.MultiRowTop) ? Rep.RowHeight : 0;
            float y1 = Rep.HeightPt - (printingTop + TopPt + Rep.detailprintingtop + rowH);
            float y2 = y1;
            Rep.Canvas.SetColorStroke(GetColor(BorderColor));
            Rep.Canvas.SetLineWidth(Border);
            Rep.Canvas.MoveTo(Llx, y1);
            Rep.Canvas.LineTo(Urx, y2);
            Rep.Canvas.Stroke();
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
        public override void DrawMe(float printingTop, EbReport Rep, List<Param> Linkparams, int slno)
        {
            float y1 = Rep.HeightPt - (printingTop + TopPt + Rep.detailprintingtop);
            float y2 = Rep.HeightPt - (printingTop + TopPt + HeightPt + Rep.detailprintingtop);
            Rep.Canvas.SetColorStroke(GetColor(BorderColor));
            Rep.Canvas.SetLineWidth(Border);
            Rep.Canvas.MoveTo(Llx, y1);
            Rep.Canvas.LineTo(Llx, y2);
            Rep.Canvas.Stroke();
        }
    }
    [EnableInBuilder(BuilderType.Report)]
    public class EbTable_Layout : EbReportFieldShape
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
            return @"<div class='eb_table_container dropped' id='@id' eb-type='Table_Layout' 
                style='top: @Top px;left: @Left px;height: @Height px;width: @Width px;'>
                <table onclick='$(this).parent().click();' style='border: @Border px solid ; border-color: @BorderColor ;' class='table eb_table_layout'>
                <tr><td eb-type='Table_Layout'></td><td eb-type='TableLayout'></td><td eb-type='Table_Layout'></td></tr>
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
