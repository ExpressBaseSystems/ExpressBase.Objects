using ExpressBase.Common.Extensions;
using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace ExpressBase.Objects.ReportRelated
{
    public abstract class EbReportField : EbReportObject
    {
        [EnableInBuilder(BuilderType.Report)]
        [UIproperty]
        [PropertyGroup("Appearance")]
        public EbTextAlign TextAlign { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        [UIproperty]
        [PropertyGroup("Appearance")]
        public int Border { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        [UIproperty]
        [PropertyEditor(PropertyEditorType.Color)]
        [PropertyGroup("Appearance")]
        public string BorderColor { get; set; }

        public BaseColor GetColor(string Color)
        {
            var colr = ColorTranslator.FromHtml(Color).ToArgb();
            return new BaseColor(colr);
        }
        public virtual void DrawMe(PdfContentByte canvas, float reportHeight, float printingTop, float detailprintingtop)
        {
        }
        public virtual void DrawMe(PdfContentByte canvas, float reportHeight, float printingTop, float detailprintingtop, string column_name)
        {
        }
        public virtual void DrawMe(Document d, byte[] fileByte)
        {
        }
        public virtual void DrawMe(PdfReader pdfReader, Document d, PdfStamper stamp, byte[] fileByte)
        {
        }
    }

    [EnableInBuilder(BuilderType.Report)]
    public class EbImg : EbReportField
    {
        [EnableInBuilder(BuilderType.Report)]
        [UIproperty]
        //[HideInPropertyGrid]
        public string Source { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        [PropertyEditor(PropertyEditorType.ImageSeletor)]
        public string Image { get; set; }

        public override string GetDesignHtml()
        {
            return "<div class='img-container dropped' eb-type='Img' id='@id' style='border:1px solid #aaaaaa; width: @Width px; background: @Source ; background-size: cover; height: @Height px; position: absolute; left: @Left px; top: @Top px;'></div>".RemoveCR().DoubleQuoted();
        }
        public override string GetJsInitFunc()
        {
            return @"
    this.Init = function(id)
        {
    this.Height =100;
    this.Width= 100;
    this.Source = 'url(../images/image.png) center no-repeat';
};";
        }
        public override void DrawMe(Document d, byte[] fileByte)
        {
            iTextSharp.text.Image myImage = iTextSharp.text.Image.GetInstance(fileByte);
            myImage.ScaleToFit(this.Width, this.Height);
            //myImage.SpacingBefore = 50f;
            //myImage.SpacingAfter = 10f;
            myImage.Alignment = Element.ALIGN_CENTER;
            d.Add(myImage);
        }
    }

    [EnableInBuilder(BuilderType.Report)]
    public class EbWaterMark : EbReportField
    {
        [EnableInBuilder(BuilderType.Report)]
        [UIproperty]
        //[HideInPropertyGrid]
        public string Source { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        [PropertyEditor(PropertyEditorType.ImageSeletor)]
        public string Image { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        [UIproperty]
        public string WaterMarkText { get; set; }

        public override string GetDesignHtml()
        {
            return "<div class='wm-container dropped' eb-type='WaterMark' id='@id' style='opacity:.5; width: @Width px; background: @Source ; background-size: cover; height: @Height px; position: absolute; left: @Left px; top: @Top px;'> @WaterMarkText </div>".RemoveCR().DoubleQuoted();
        }
        public override string GetJsInitFunc()
        {
            return @"
    this.Init = function(id)
        {
    this.Height =100;
    this.Width= 100;
    this.Source = 'url(../images/image.png) center no-repeat';
};";
        }

        public override void DrawMe(PdfReader pdfReader, Document d, PdfStamper stamp, byte[] fileByte)
        {
           if(this.WaterMarkText != string.Empty)
            {
                PdfContentByte canvas;
                iTextSharp.text.Font fo = new iTextSharp.text.Font(5, 20, 5, BaseColor.LightGray);
                for (int page = 1; page <= pdfReader.NumberOfPages; page++)
                {
                    canvas = stamp.GetUnderContent(page);
                    ColumnText.ShowTextAligned(canvas, Element.ALIGN_CENTER, new Phrase(this.WaterMarkText, fo), d.PageSize.Width / 2, d.PageSize.Height / 2, 45);
                }
            }
           if(this.Image != string.Empty)
            {
                iTextSharp.text.Image img = iTextSharp.text.Image.GetInstance(fileByte);
                img.RotationDegrees = 45;
                img.ScaleToFit(d.PageSize.Width, d.PageSize.Height);
                img.SetAbsolutePosition(0, d.PageSize.Height); // set the position of watermark to appear (0,0 = bottom left corner of the page)
                PdfContentByte waterMark;
                for (int page = 1; page <= pdfReader.NumberOfPages; page++)
                {
                    waterMark = stamp.GetUnderContent(page);
                    waterMark.AddImage(img);
                }
            }           
            stamp.FormFlattening = true;
            stamp.Close();
        }
    }

    [EnableInBuilder(BuilderType.Report)]
    public class EbDateTime : EbReportField
    {
        public override string GetDesignHtml()
        {
            return "<div class='date-time dropped' eb-type='DateTime' id='@id' style='border: @Border px solid;border-color: @BorderColor ; width: @Width px; height: @Height px; background-color:@BackColor ; color:@ForeColor ; position: absolute; left: @Left px; top: @Top px;text-align: @TextAlign;'> @Title </div>".RemoveCR().DoubleQuoted();
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
        public override void DrawMe(PdfContentByte canvas, float reportHeight, float printingTop, float detailprintingtop, string column_val)
        {
            var urx = this.Width + this.Left;
            var ury = reportHeight - (printingTop + this.Top + detailprintingtop);
            var llx = this.Left;
            var lly = reportHeight - (printingTop + this.Top + this.Height + detailprintingtop);

            ColumnText ct = new ColumnText(canvas);
            ct.Canvas.SetColorFill(GetColor(this.ForeColor));
            ct.SetSimpleColumn(new Phrase(column_val), llx, lly, urx, ury, 15, Element.ALIGN_LEFT);
            ct.Go();
        }
    }

    [EnableInBuilder(BuilderType.Report)]
    public class EbPageNo : EbReportField
    {

        public override string GetDesignHtml()
        {
            return "<div class='page-no dropped' eb-type='PageNo' id='@id' style='border: @Border px solid;border-color: @BorderColor ; width: @Width px; height: @Height px; background-color:@BackColor ; color:@ForeColor ; position: absolute; left: @Left px; top: @Top px;text-align: @TextAlign;'> @Title </div>".RemoveCR().DoubleQuoted();
        }
        public override string GetJsInitFunc()
        {
            return @"
    this.Init = function(id)
        {
     this.Height =25;
    this.Width= 100;
    this.ForeColor = '#201c1c';
    this.Border = 1;
    this.BorderColor = '#aaaaaa'
};";
        }
        public override void DrawMe(PdfContentByte canvas, float reportHeight, float printingTop, float detailprintingtop, string column_val)
        {
            var urx = this.Width + this.Left;
            var ury = reportHeight - (printingTop + this.Top + detailprintingtop);
            var llx = this.Left;
            var lly = reportHeight - (printingTop + this.Top + this.Height + detailprintingtop);

            ColumnText ct = new ColumnText(canvas);
            ct.Canvas.SetColorFill(GetColor(this.ForeColor));
            ct.SetSimpleColumn(new Phrase(column_val), llx, lly, urx, ury, 15, Element.ALIGN_LEFT);
            ct.Go();
        }
    }

    [EnableInBuilder(BuilderType.Report)]
    public class EbPageXY : EbReportField
    {

        public override string GetDesignHtml()
        {
            return "<div class='page-x/y dropped' eb-type='PageXY' id='@id' style='border: @Border px solid;border-color: @BorderColor ; width: @Width px; height: @Height px; background-color:@BackColor ; color:@ForeColor ; position: absolute; left: @Left px; top: @Top px;text-align: @TextAlign;'> @Title </div>".RemoveCR().DoubleQuoted();
        }
        public override string GetJsInitFunc()
        {
            return @"
    this.Init = function(id)
        {
     this.Height =25;
    this.Width= 100;
    this.ForeColor = '#201c1c';
    this.Border = 1;
    this.BorderColor = '#aaaaaa'
};";
        }
        public override void DrawMe(PdfContentByte canvas, float reportHeight, float printingTop, float detailprintingtop, string column_val)
        {
            var urx = this.Width + this.Left;
            var ury = reportHeight - (printingTop + this.Top + detailprintingtop);
            var llx = this.Left;
            var lly = reportHeight - (printingTop + this.Top + this.Height + detailprintingtop);

            ColumnText ct = new ColumnText(canvas);
            ct.Canvas.SetColorFill(GetColor(this.ForeColor));
            ct.SetSimpleColumn(new Phrase(column_val), llx, lly, urx, ury, 15, Element.ALIGN_LEFT);
            ct.Go();
        }
    }

    [EnableInBuilder(BuilderType.Report)]
    public class EbUserName : EbReportField
    {

        public override string GetDesignHtml()
        {
            return "<div class='User-name dropped' eb-type='UserName' id='@id' style='border: @Border px solid;border-color: @BorderColor ; width: @Width px; height: @Height px; background-color:@BackColor ; color:@ForeColor ; position: absolute; left: @Left px; top: @Top px;text-align: @TextAlign ;'> @Title </div>".RemoveCR().DoubleQuoted();
        }
        public override string GetJsInitFunc()
        {
            return @"
    this.Init = function(id)
        {
     this.Height =25;
    this.Width= 100;
    this.ForeColor = '#201c1c';
    this.Border = 1;
    this.BorderColor = '#aaaaaa'
};";
        }
    }

    [EnableInBuilder(BuilderType.Report)]
    public class EbText : EbReportField
    {

        public override string GetDesignHtml()
        {
            return "<div class='Text-Field dropped' eb-type='Text' id='@id' style='border: @Border px solid;border-color: @BorderColor ; width: @Width px; height: @Height px; background-color:@BackColor ; color:@ForeColor ; position: absolute; left: @Left px; top: @Top px;text-overflow: ellipsis;overflow: hidden;text-align: @TextAlign ;'> @Title </div>".RemoveCR().DoubleQuoted();
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
        public override void DrawMe(PdfContentByte canvas, float reportHeight, float printingTop, float detailprintingtop)
        {
            var urx = this.Width + this.Left;
            var ury = reportHeight - (printingTop + this.Top + detailprintingtop);
            var llx = this.Left;
            var lly = reportHeight - (printingTop + this.Top + this.Height + detailprintingtop);

            ColumnText ct = new ColumnText(canvas);
            ct.Canvas.SetColorFill(GetColor(this.ForeColor));
            ct.SetSimpleColumn(new Phrase(this.Title), llx, lly, urx, ury, 15, Element.ALIGN_LEFT);
            ct.Go();
        }
    }

    [EnableInBuilder(BuilderType.Report)]
    public class EbBarcode : EbReportField
    {
        [EnableInBuilder(BuilderType.Report)]
        [UIproperty]
        [PropertyGroup("Appearance")]
        public string Source { get; set; }

        public override string GetDesignHtml()
        {
            return "<div class='Bar-code dropped' eb-type='Barcode' id='@id' style=' border: @Border px solid;border-color: @BorderColor ; width: @Width px; height: @Height px; background: @Source ; position: absolute; left: @Left px; top: @Top px;background-size: 100% 100%;'></div>".RemoveCR().DoubleQuoted();
        }
        public override string GetJsInitFunc()
        {
            return @"
    this.Init = function(id)
        {
     this.Height =50;
    this.Width= 150;    
    this.Border = 1;
    this.BorderColor = '#aaaaaa'
    this.Source = 'url(../images/barcode.png) center no-repeat';
};";
        }
    }

    [EnableInBuilder(BuilderType.Report)]
    public class EbQRcode : EbReportField
    {
        [EnableInBuilder(BuilderType.Report)]
        [UIproperty]
        [PropertyGroup("Appearance")]
        public string Source { get; set; }

        public override string GetDesignHtml()
        {
            return "<div class='QRcode dropped' eb-type='QRcode' id='@id' style='border: @Border px solid;border-color: @BorderColor ; width: @Width px; height: @Height px; background: @Source ; position: absolute; left: @Left px; top: @Top px;background-size: 100% 100%;'></div>".RemoveCR().DoubleQuoted();
        }
        public override string GetJsInitFunc()
        {
            return @"
    this.Init = function(id)
        {
     this.Height =100;
    this.Width= 100;    
    this.Border = 1;
    this.BorderColor = '#aaaaaa';
    this.Source = 'url(../images/Qr-code.png) center no-repeat';
};";
        }
    }

    [EnableInBuilder(BuilderType.Report)]
    public class EbSerialNumber : EbReportField
    {
        public override string GetDesignHtml()
        {
            return "<div class='Serial-Number dropped' eb-type='SerialNumber' id='@id' style='border: @Border px solid;border-color: @BorderColor ; width: @Width px; height: @Height px; background-color:@BackColor ; color:@ForeColor ; position: absolute; left: @Left px; top: @Top px;text-align: @TextAlign ;'> @Title </div>".RemoveCR().DoubleQuoted();
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
        public override void DrawMe(PdfContentByte canvas, float reportHeight, float printingTop, float detailprintingtop, string column_val)
        {
            var urx = this.Width + this.Left;
            var ury = reportHeight - (printingTop + this.Top + detailprintingtop);
            var llx = this.Left;
            var lly = reportHeight - (printingTop + this.Top + this.Height + detailprintingtop);

            ColumnText ct = new ColumnText(canvas);
            ct.Canvas.SetColorFill(GetColor(this.ForeColor));
            ct.SetSimpleColumn(new Phrase(column_val), llx, lly, urx, ury, 15, Element.ALIGN_LEFT);
            ct.Go();
        }
    }
}

