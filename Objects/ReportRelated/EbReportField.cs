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
using QRCoder;
using System.Data;
using ExpressBase.Common;
using ExpressBase.Common.Data;
using Newtonsoft.Json;

namespace ExpressBase.Objects.ReportRelated
{
    public abstract class EbReportField : EbReportObject
    {
        [EnableInBuilder(BuilderType.Report)]
        [UIproperty]
        [PropertyGroup("Appearance")]
        public virtual EbTextAlign TextAlign { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        [UIproperty]
        [HideInPropertyGrid]
        public string  ParentName { get; set; }//only for builder

        [EnableInBuilder(BuilderType.Report)]
        [UIproperty]
        [PropertyGroup("Appearance")]
        public int Border { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        [UIproperty]
        [PropertyEditor(PropertyEditorType.Color)]
        [PropertyGroup("Appearance")]
        public string BorderColor { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        [PropertyGroup("Appearance")]
        [UIproperty]
        [PropertyEditor(PropertyEditorType.FontSelector)]
        public virtual EbFont Font { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        [UIproperty]
        [PropertyGroup("General")]
        [HideInPropertyGrid]
        public override string ForeColor { get; set; } = "";

        public BaseColor GetColor(string Color)
        {
            var colr = ColorTranslator.FromHtml(Color).ToArgb();
            return new BaseColor(colr);
        }
        public virtual void DrawMe(PdfContentByte canvas, float reportHeight, float printingTop, float detailprintingtop) { }
        //public virtual void DrawMe(PdfContentByte canvas, float reportHeight, float printingTop, float detailprintingtop, float rowH)
        //{
        //}
        public virtual void DrawMe(PdfContentByte canvas, float reportHeight, float printingTop, EbReport report) { }
        public virtual void DrawMe(PdfContentByte canvas, float reportHeight, float printingTop, string column_name, float detailprintingtop, DbType column_type) { }
        public virtual void DrawMe(PdfContentByte canvas, float reportHeight, float printingTop, float detailprintingtop, string column_name) { }
        public virtual void DrawMe(Document doc, PdfContentByte canvas, float reportHeight, float printingTop, string column_val, float detailprintingtop, DbType column_type, List<Param> l) { }
        public virtual void DrawMe(Document doc, PdfContentByte canvas, float reportHeight, float printingTop, float detailprintingtop, string column_name) { }
        public virtual void DrawMe(Document d, byte[] fileByte) { }
        public virtual void DrawMe(Document d, PdfWriter writer, byte[] fileByte, float reportHeight) { }
        public virtual void DrawMe(Document d, byte[] fileByte, float reportHeight, float printingTop, float detailprintingtop) { }
        private iTextSharp.text.Font iTextFont = null;
        public virtual iTextSharp.text.Font ITextFont
        {
            get
            {
              if(Font ==null)
                Font = (new EbFont { color = "#000000", Font = "Times-Roman", Caps = false, Size = 10, Strikethrough = false, Style = 0, Underline = false });
                if (/*iTextFont == null &&*/ Font!= null)
                {
                    iTextFont = new iTextSharp.text.Font(BaseFont.CreateFont(Font.Font, BaseFont.CP1252, BaseFont.EMBEDDED), Font.Size, (int)Font.Style, GetColor(Font.color));
                    if (Font.Caps)
                        Title = this.Title.ToUpper();
                    if (Font.Strikethrough)
                        iTextFont.SetStyle(iTextSharp.text.Font.STRIKETHRU);
                    if (Font.Underline)
                        iTextFont.SetStyle(iTextSharp.text.Font.UNDERLINE);
                }
                return iTextFont;
            }
        }

        //public iTextSharp.text.Font SetFont()
        //{
        //    //var x=iTextSharp.text.FontFactory.RegisterDirectory("E:\\ExpressBase.Core\\ExpressBase.Objects\\Fonts\\");
        //    //iTextSharp.text.Font font = FontFactory.GetFont("Verdana", Font.Size, (int)Font.Style, GetColor(Font.color));
        //    BaseFont bf = BaseFont.CreateFont(Font.Font, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
        //    iTextSharp.text.Font font = new iTextSharp.text.Font(bf, Font.Size, (int)Font.Style, GetColor(Font.color));
        //    if (Font.Caps)
        //        Title = this.Title.ToUpper();
        //    if (Font.Strikethrough)
        //        font.SetStyle(iTextSharp.text.Font.STRIKETHRU);
        //    if (Font.Underline)
        //        font.SetStyle(iTextSharp.text.Font.UNDERLINE);
        //    return font;
        //}
    }

    [EnableInBuilder(BuilderType.Report)]
    public class EbImg : EbReportField
    {
        [EnableInBuilder(BuilderType.Report)]
        [UIproperty]
        [MetaOnly]
        public string Source { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        [JsonIgnore]
        public override EbTextAlign TextAlign { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        [PropertyEditor(PropertyEditorType.ImageSeletor)]
        [PropertyGroup("URL")]
        public string Image { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        [HideInPropertyGrid]
        public override string Title { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        [HideInPropertyGrid]
        public override EbFont Font { get; set; }

        public override string GetDesignHtml()
        {
            return "<div class='img-container dropped' eb-type='Img' id='@id' style='border: @Border px solid;border-color: @BorderColor ;width: @Width px; background: @Source ; background-size: cover; height: @Height px; left: @Left px; top: @Top px;'></div>".RemoveCR().DoubleQuoted();
        }
        public override string GetJsInitFunc()
        {
            return @"
    this.Init = function(id)
        {
    this.Height =40;
    this.Width= 40;
    this.Source = 'url(../images/image.png) center no-repeat';
    this.Border = 1;
this.BorderColor = '#eae6e6';
};";
        }
        public override void DrawMe(Document d, byte[] fileByte, float reportHeight, float printingTop, float detailprintingtop)
        {
            iTextSharp.text.Image myImage = iTextSharp.text.Image.GetInstance(fileByte);
            myImage.ScaleToFit(this.WidthPt, this.HeightPt);
            myImage.SetAbsolutePosition(this.LeftPt, reportHeight - (printingTop + this.TopPt + this.HeightPt + detailprintingtop));
            myImage.Alignment = (int)TextAlign;
            d.Add(myImage);
        }
    }

    [EnableInBuilder(BuilderType.Report)]
    public class EbWaterMark : EbReportField
    {
        [EnableInBuilder(BuilderType.Report)]
        [UIproperty]
        [MetaOnly]
        public string Source { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        [HideInPropertyGrid]
        public override string Title { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        [PropertyEditor(PropertyEditorType.ImageSeletor)]
        [PropertyGroup("URL")]
        public string Image { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        [UIproperty]
        [PropertyGroup("General")]
        public string WaterMarkText { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        [UIproperty]
        [PropertyGroup("General")]
        public int Rotation { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        [UIproperty]
        [PropertyGroup("Appearance")]
        public new EbTextAlign TextAlign { get; set; } = EbTextAlign.Center;

        public override string GetDesignHtml()
        {
            return "<div class='wm-container dropped' eb-type='WaterMark' id='@id' style='border: @Border px solid;border-color: @BorderColor ;opacity:.5; width: @Width px; background: @Source ; background-size: cover; height: @Height px; left: @Left px; top: @Top px;'> @WaterMarkText </div>".RemoveCR().DoubleQuoted();
        }
        public override string GetJsInitFunc()
        {
            return @"
    this.Init = function(id)
        {
    this.Height =50;
    this.Width= 50;
    this.Source = 'url(../images/image.png) center no-repeat';
this.Border = 1;
this.BorderColor = '#eae6e6';
};";
        }

        public override void DrawMe(Document d, PdfWriter writer, byte[] fileByte, float reportHeight)
        {
            Phrase phrase = null;
            if (this.WaterMarkText != string.Empty)
            {
                //if (this.Font == null)
                //    Font = (new EbFont { color = "#000000", Font = "Courier", Caps = false, Size = 10, Strikethrough = false, Style = 0, Underline = false });
               // else
                    phrase = new Phrase(this.WaterMarkText, ITextFont);
                PdfContentByte canvas;
                canvas = writer.DirectContentUnder;
                ColumnText.ShowTextAligned(canvas, (int)TextAlign, phrase, d.PageSize.Width / 2, d.PageSize.Height / 2, this.Rotation);
            }
            if (this.Image != string.Empty)
            {
                iTextSharp.text.Image img = iTextSharp.text.Image.GetInstance(fileByte);
                img.RotationDegrees = this.Rotation;
                img.ScaleToFit(this.WidthPt, this.HeightPt);
                img.SetAbsolutePosition(this.LeftPt, reportHeight - this.TopPt - this.HeightPt);
                PdfGState _state = new PdfGState()
                {
                    FillOpacity = 0.1F,
                    StrokeOpacity = 0.1F
                };
                PdfContentByte cb = writer.DirectContentUnder;
                cb.SaveState();
                cb.SetGState(_state);
                cb.AddImage(img);
                cb.RestoreState();
            }
        }
    }

    [EnableInBuilder(BuilderType.Report)]
    public class EbDateTime : EbReportField
    {
        [EnableInBuilder(BuilderType.Report)]
        [UIproperty]
        [PropertyGroup("General")]
        public DateFormatReport Format { get; set; }

        [OnChangeExec(@"
            pg.MakeReadOnly('Title');
        ")]
        [EnableInBuilder(BuilderType.Report)]
        public override string Title { set; get; }

        public string FormatDate(string column_val)
        {
            DateTime dt = Convert.ToDateTime(column_val);
            if (this.Format == DateFormatReport.dddd_MMMM_d_yyyy)
                return String.Format("{0:dddd, MMMM d, yyyy}", dt);
            else if (this.Format == DateFormatReport.M_d_yyyy)
                return String.Format("{0:M/d/yyyy}", dt);
            else if (this.Format == DateFormatReport.ddd_MMM_d_yyyy)
                return String.Format("{0:ddd, MMM d, yyyy}", dt);
            else if (this.Format == DateFormatReport.MM_dd_yy)
                return String.Format("{0:MM/dd/yy}", dt);
            else if (this.Format == DateFormatReport.MM_dd_yyyy)
                return String.Format("{0:MM/dd/yyyy}", dt);
            return column_val;
        }

        public override string GetDesignHtml()
        {
            return "<div class='date-time dropped' eb-type='DateTime' id='@id' style='border: @Border px solid;border-color: @BorderColor ; width: @Width px; height: @Height px; background-color:@BackColor ; color:@ForeColor ; left: @Left px; top: @Top px;text-align: @TextAlign;'> @Title </div>".RemoveCR().DoubleQuoted();
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
        public override void DrawMe(PdfContentByte canvas, float reportHeight, float printingTop, float detailprintingtop, string column_val)
        {

            var urx = this.WidthPt + this.LeftPt;
            var ury = reportHeight - (printingTop + this.TopPt + detailprintingtop);
            var llx = this.LeftPt;
            var lly = reportHeight - (printingTop + this.TopPt + this.HeightPt + detailprintingtop);
            column_val = FormatDate(column_val);
            Phrase phrase = null;
            phrase = new Phrase(column_val,ITextFont);
            ColumnText ct = new ColumnText(canvas);
           // ct.Canvas.SetColorFill(GetColor(this.ForeColor));
            ct.SetSimpleColumn(phrase, llx, lly, urx, ury, 15, (int)TextAlign);
            ct.Go();
        }
    }

    [EnableInBuilder(BuilderType.Report)]
    public class EbPageNo : EbReportField
    {
        [OnChangeExec(@"
            pg.MakeReadOnly('Title');
        ")]
        [EnableInBuilder(BuilderType.Report)]
        public override string Title { set; get; }

        public override string GetDesignHtml()
        {
            return "<div class='page-no dropped' eb-type='PageNo' id='@id' style='border: @Border px solid;border-color: @BorderColor ; width: @Width px; height: @Height px; background-color:@BackColor ; color:@ForeColor ; left: @Left px; top: @Top px;text-align: @TextAlign;'> @Title </div>".RemoveCR().DoubleQuoted();
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
this.BorderColor = '#eae6e6';
};";
        }
        public override void DrawMe(PdfContentByte canvas, float reportHeight, float printingTop, float detailprintingtop, string column_val)
        {
            var urx = this.WidthPt + this.LeftPt;
            var ury = reportHeight - (printingTop + this.TopPt + detailprintingtop);
            var llx = this.LeftPt;
            var lly = reportHeight - (printingTop + this.TopPt + this.HeightPt + detailprintingtop);
            Phrase phrase = null;

            //if (this.Font == null)
            //    Font = (new EbFont { color = "#000000", Font = "Courier", Caps = false, Size = 10, Strikethrough = false, Style = 0, Underline = false });
            //else
            phrase = new Phrase(column_val, ITextFont);
            ColumnText ct = new ColumnText(canvas);
            //ct.Canvas.SetColorFill(GetColor(this.ForeColor));
            ct.SetSimpleColumn(phrase, llx, lly, urx, ury, 15, (int)TextAlign);
            ct.Go();
        }
    }

    [EnableInBuilder(BuilderType.Report)]
    public class EbPageXY : EbReportField
    {
        [OnChangeExec(@"
            pg.MakeReadOnly('Title');
        ")]
        [EnableInBuilder(BuilderType.Report)]
        public override string Title { set; get; }

        public override string GetDesignHtml()
        {
            return "<div class='page-x/y dropped' eb-type='PageXY' id='@id' style='border: @Border px solid;border-color: @BorderColor ; width: @Width px; height: @Height px; background-color:@BackColor ; color:@ForeColor ; left: @Left px; top: @Top px;text-align: @TextAlign;'> @Title </div>".RemoveCR().DoubleQuoted();
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
this.BorderColor = '#eae6e6';
};";
        }
        public override void DrawMe(PdfContentByte canvas, float reportHeight, float printingTop, float detailprintingtop, string column_val)
        {
            var urx = this.WidthPt + this.LeftPt;
            var ury = reportHeight - (printingTop + this.TopPt + detailprintingtop);
            var llx = this.LeftPt;
            var lly = reportHeight - (printingTop + this.TopPt + this.HeightPt + detailprintingtop);

            Phrase phrase = null;
            //if (this.Font == null)
            //    Font = (new EbFont { color = "#000000", Font = "Courier", Caps = false, Size = 10, Strikethrough = false, Style = 0, Underline = false });
            //phrase = new Phrase(column_val);
            // else
            phrase = new Phrase(column_val, ITextFont);
            ColumnText ct = new ColumnText(canvas);
           // ct.Canvas.SetColorFill(GetColor(this.ForeColor));
            ct.SetSimpleColumn(phrase, llx, lly, urx, ury, 15, (int)TextAlign);
            ct.Go();
        }
    }

    [EnableInBuilder(BuilderType.Report)]
    public class EbUserName : EbReportField
    {
        [OnChangeExec(@"
            pg.MakeReadOnly('Title');
        ")]
        [EnableInBuilder(BuilderType.Report)]
        public override string Title { set; get; }

        public override string GetDesignHtml()
        {
            return "<div class='User-name dropped' eb-type='UserName' id='@id' style='border: @Border px solid;border-color: @BorderColor ; width: @Width px; height: @Height px; background-color:@BackColor ; color:@ForeColor ; left: @Left px; top: @Top px;text-align: @TextAlign ;'> @Title </div>".RemoveCR().DoubleQuoted();
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
this.BorderColor = '#eae6e6';
};";
        }
        public override void DrawMe(PdfContentByte canvas, float reportHeight, float printingTop, float detailprintingtop, string column_val)
        {
            var urx = this.WidthPt + this.LeftPt;
            var ury = reportHeight - (printingTop + this.TopPt + detailprintingtop);
            var llx = this.LeftPt;
            var lly = reportHeight - (printingTop + this.TopPt + this.HeightPt + detailprintingtop);

            Phrase phrase = null;
            //if (this.Font == null)
            //    Font = (new EbFont { color = "#000000", Font = "Courier", Caps = false, Size = 10, Strikethrough = false, Style = 0, Underline = false });
            //    phrase = new Phrase(column_val);
            //else
            phrase = new Phrase(column_val, ITextFont);
            ColumnText ct = new ColumnText(canvas);
            //ct.Canvas.SetColorFill(GetColor(this.ForeColor));
            ct.SetSimpleColumn(phrase, llx, lly, urx, ury, 15, (int)TextAlign);
            ct.Go();
        }
    }

    [EnableInBuilder(BuilderType.Report)]
    public class EbText : EbReportField
    {
        
        public override string GetDesignHtml()
        {
            return "<div class='Text-Field dropped' eb-type='Text' id='@id' style='border: @Border px solid;border-color: @BorderColor ; width: @Width px; height: @Height px; background-color:@BackColor ; color:@ForeColor ; left: @Left px; top: @Top px;text-overflow: ellipsis;overflow: hidden;text-align: @TextAlign ;'> @Title </div>".RemoveCR().DoubleQuoted();
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
        public override void DrawMe(PdfContentByte canvas, float reportHeight, float printingTop, EbReport report)
        {
            var urx = this.WidthPt + this.LeftPt;
            var ury = reportHeight - (printingTop + this.TopPt + report.detailprintingtop);
            var llx = this.LeftPt;
            var lly = reportHeight - (printingTop + this.TopPt + this.HeightPt + report.detailprintingtop + report.RowHeight);

            ColumnText ct = new ColumnText(canvas);
            Phrase phrase = null;
            //ct.Canvas.SetColorFill(GetColor(this.ForeColor));
            //if (this.Font == null)
            //    Font = (new EbFont { color = "#000000", Font = "Courier", Caps = false, Size = 10, Strikethrough = false, Style = 0, Underline = false });
            //    phrase = new Phrase(this.Title);
            //else
            phrase = new Phrase(this.Title, ITextFont);

            ct.SetSimpleColumn(phrase, llx, lly, urx, ury, 15,(int)TextAlign);
            ct.Go();
        }
    }

    [EnableInBuilder(BuilderType.Report)]
    public class EbParameter : EbReportField
    {
        [OnChangeExec(@"
            pg.MakeReadOnly('Title');
        ")]
        [EnableInBuilder(BuilderType.Report)]
        public override string Title { set; get; }

        public override string GetDesignHtml()
        {
            return "<div class='Parameter dropped' eb-type='Parameter' id='@id' style='border: @Border px solid;border-color: @BorderColor ; width: @Width px; height: @Height px; background-color:@BackColor ; color:@ForeColor ; left: @Left px; top: @Top px;text-overflow: ellipsis;overflow: hidden;text-align: @TextAlign ;'> @Title </div>".RemoveCR().DoubleQuoted();
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
        public override void DrawMe(PdfContentByte canvas, float reportHeight, float printingTop, float detailprintingtop, string column_val)
        {
            var urx = this.WidthPt + this.LeftPt;
            var ury = reportHeight - (printingTop + this.TopPt + detailprintingtop);
            var llx = this.LeftPt;
            var lly = reportHeight - (printingTop + this.TopPt + this.HeightPt + detailprintingtop);

            Phrase phrase = null;
            //if (this.Font == null)
            //    Font = (new EbFont { color = "#000000", Font = "Courier", Caps = false, Size = 10, Strikethrough = false, Style = 0, Underline = false });
            //    phrase = new Phrase(column_val);
            //else
            phrase = new Phrase(column_val, ITextFont);
            ColumnText ct = new ColumnText(canvas);
            //ct.Canvas.SetColorFill(GetColor(this.ForeColor));
            ct.SetSimpleColumn(phrase, llx, lly, urx, ury, 15, (int)TextAlign);
            ct.Go();
        }
    }

    [EnableInBuilder(BuilderType.Report)]
    public class EbBarcode : EbReportField
    {
        [EnableInBuilder(BuilderType.Report)]
        [UIproperty]
        [MetaOnly]
        public string Source { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        [UIproperty]
        [PropertyGroup("Data")]
        public string Code { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        [UIproperty]
        [PropertyGroup("General")]
        public int Type { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        [HideInPropertyGrid]
        public override string Title { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        [UIproperty]
        [PropertyGroup("Appearance")]
        public bool GuardBars { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        [UIproperty]
        [PropertyGroup("Appearance")]
        public float BaseLine { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        [HideInPropertyGrid]
        public new EbFont Font { get; set; }

        public override string GetDesignHtml()
        {
            return "<div class='Bar-code dropped' eb-type='Barcode' id='@id' style=' border: @Border px solid;border-color: @BorderColor ; width: @Width px; height: @Height px; background: @Source ; left: @Left px; top: @Top px;background-size: 100% 100%;'></div>".RemoveCR().DoubleQuoted();
        }
        public override string GetJsInitFunc()
        {
            return @"
    this.Init = function(id)
        {
     this.Height =40;
    this.Width= 140;
    this.Source = 'url(../images/barcode.png) center no-repeat';
this.Border = 1;
this.BorderColor = '#eae6e6';
};";
        }
        public override void DrawMe(Document doc, PdfContentByte canvas, float reportHeight, float printingTop, float detailprintingtop, string code_val)
        {
            //** BarcodeEan(& BarcodeEansupp)
            //public const int EAN13 = 1;
            //public const int EAN8 = 2;
            //public const int UPCA = 3;
            //public const int UPCE = 4;
            //public const int SUPP2 = 5;
            //public const int SUPP5 = 6; 
            //** Barcode128
            //public const int CODE128 = 9;      
            //public const int CODE128_UCC = 10;
            //public const int CODE128_RAW = 11;   
            //** BarcodePostnet
            //public const int POSTNET = 7;
            //public const int PLANET = 8;      

            //public const int CODABAR = 12;      


            //** Barcode39
            //** BarcodeCodabar
            //** BarcodeDatamatrix
            //** BarcodeInter25
            //** BarcodePdf417
            //Type = 6;
            iTextSharp.text.Image imageEAN = null;
            try
            {
                //if (Type >= 1 && Type <= 6)
                //{
                ////BarcodeEan codeEAN = new BarcodeEan
                ////{
                ////    Code = code_val.PadLeft(10, '0'),
                ////    CodeType = Type,
                ////    GuardBars = GuardBars,
                ////    Baseline = BaseLine
                ////};
                ////imageEAN = codeEAN.CreateImageWithBarcode(cb: canvas, barColor: null, textColor: null);
                //}
                //if (Type == 7 || Type == 8)
                //{
                //    BarcodePostnet codepost = new BarcodePostnet();
                //    codepost.Code = code_val;
                //    codepost.CodeType = Type;
                //    codepost.GuardBars = GuardBars;
                //    codepost.Baseline = BaseLine;
                //    imageEAN = codepost.CreateImageWithBarcode(cb: canvas, barColor: null, textColor: null);
                //}
                //if (Type >= 9 && Type <= 11)
                //{
                Barcode128 uccEan128 = new Barcode128();

                uccEan128.CodeType = Type;
                uccEan128.Code = code_val;
                uccEan128.GuardBars = GuardBars;
                uccEan128.Baseline = BaseLine;
                imageEAN = uccEan128.CreateImageWithBarcode(cb: canvas, barColor: null, textColor: null);
                //}

               // imageEAN.ScaleAbsolute(Width, Height);
                imageEAN.SetAbsolutePosition(LeftPt, reportHeight - (printingTop + this.TopPt + this.HeightPt + detailprintingtop));
                doc.Add(imageEAN);
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.ToString());
                ColumnText ct = new ColumnText(canvas);
                var x = reportHeight - (printingTop + this.TopPt + detailprintingtop);
                ct.SetSimpleColumn(new Phrase("Error in generating barcode"), LeftPt, x - HeightPt, LeftPt + WidthPt, x, 15, (int)TextAlign);
                ct.Go();
            }
        }
    }

    [EnableInBuilder(BuilderType.Report)]
    public class EbQRcode : EbReportField
    {
        [EnableInBuilder(BuilderType.Report)]
        [UIproperty]
        [MetaOnly]
        public string Source { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        [UIproperty]
        [PropertyGroup("Data")]
        public string Code { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        [HideInPropertyGrid]
        public override EbFont Font { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        [HideInPropertyGrid]
        public override string Title { get; set; }

        public override string GetDesignHtml()
        {
            return "<div class='QRcode dropped' eb-type='QRcode' id='@id' style='border: @Border px solid;border-color: @BorderColor ; width: @Width px; height: @Height px; background: @Source ; left: @Left px; top: @Top px;background-size: 100% 100%;'></div>".RemoveCR().DoubleQuoted();
        }
        public override string GetJsInitFunc()
        {
            return @"
    this.Init = function(id)
        {
     this.Height =40;
    this.Width= 40; 
    this.Source = 'url(../images/Qr-code.png) center no-repeat';
this.Border = 1;
this.BorderColor = '#eae6e6';
};";
        }
        public override void DrawMe(Document doc, PdfContentByte canvas, float reportHeight, float printingTop, float detailprintingtop, string code_val)
        {
            try
            {
                QRCodeGenerator qrGenerator = new QRCodeGenerator();
                QRCodeData qrCodeData = qrGenerator.CreateQrCode(code_val, QRCodeGenerator.ECCLevel.Q);
                BitmapByteQRCode qrCode = new BitmapByteQRCode(qrCodeData);
                byte[] qrCodeImage = qrCode.GetGraphic(20);
                iTextSharp.text.Image img = iTextSharp.text.Image.GetInstance(qrCodeImage);
                img.SetAbsolutePosition(LeftPt, reportHeight - (printingTop + this.TopPt + this.HeightPt + detailprintingtop));
                img.ScaleAbsolute(WidthPt, HeightPt);
                doc.Add(img);
            }
            catch (Exception e)
            {
                ColumnText ct = new ColumnText(canvas);
                var x = reportHeight - (printingTop + this.TopPt + detailprintingtop);
                ct.SetSimpleColumn(new Phrase("Error in generating barcode"), LeftPt, x - HeightPt, LeftPt + WidthPt, x, 15, (int)TextAlign);
                ct.Go();
                Console.WriteLine("Exception: " + e.ToString());
            }
        }
    }

    [EnableInBuilder(BuilderType.Report)]
    public class EbSerialNumber : EbReportField
    {
        public override string GetDesignHtml()
        {
            return "<div class='Serial-Number dropped' eb-type='SerialNumber' id='@id' style='border: @Border px solid;border-color: @BorderColor ; width: @Width px; height: @Height px; background-color:@BackColor ; color:@ForeColor ; left: @Left px; top: @Top px;text-align: @TextAlign ;'> @Title </div>".RemoveCR().DoubleQuoted();
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
        public override void DrawMe(PdfContentByte canvas, float reportHeight, float printingTop, float detailprintingtop, string column_val)
        {
            var urx = this.WidthPt + this.LeftPt;
            var ury = reportHeight - (printingTop + this.TopPt + detailprintingtop);
            var llx = this.LeftPt;
            var lly = reportHeight - (printingTop + this.TopPt + this.HeightPt + detailprintingtop);

            Phrase phrase = null;
            //if (this.Font == null)
            //    Font = (new EbFont { color = "#000000", Font = "Courier", Caps = false, Size = 10, Strikethrough = false, Style = 0, Underline = false });
            //    phrase = new Phrase(column_val);
            //else
            phrase = new Phrase(column_val, ITextFont);

            ColumnText ct = new ColumnText(canvas);
            //ct.Canvas.SetColorFill(GetColor(this.ForeColor));
            ct.SetSimpleColumn(phrase, llx, lly, urx, ury, 15, (int)TextAlign);
            ct.Go();
        }
    }

    [EnableInBuilder(BuilderType.Report)]
    public class EbLocFieldImage : EbReportField
    {
        [EnableInBuilder(BuilderType.Report)]
        [JsonIgnore]
        public override EbTextAlign TextAlign { get; set; }

        public override string GetDesignHtml()
        {
            return "<div class='EbLocFieldImg dropped' eb-type='LocFieldImage' id='@id' style='border: @Border px solid;border-color: @BorderColor ;width: @Width px; background: url(../images/image.png) center no-repeat ; background-size: cover; height: @Height px; left: @Left px; top: @Top px;'></div>".RemoveCR().DoubleQuoted();
        }
        public override string GetJsInitFunc()
        {
            return @"
    this.Init = function(id)
        {
    this.Height =40;
    this.Width= 40;
    this.Border = 1;
this.BorderColor = '#eae6e6';
};";
        }
    }

    [EnableInBuilder(BuilderType.Report)]
    public class EbLocFieldText : EbReportField
    {
        public override string GetDesignHtml()
        {
            return "<div class='EbLocFieldText dropped' eb-type='LocFieldText' id='@id' style='border: @Border px solid;border-color: @BorderColor ; width: @Width px; height: @Height px; background-color:@BackColor ; color:@ForeColor ; left: @Left px; top: @Top px;text-align: @TextAlign ;'> @Title </div>".RemoveCR().DoubleQuoted();
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
    public class EbLocFieldImage : EbReportField
    {
        [EnableInBuilder(BuilderType.Report)]
        [JsonIgnore]
        public override EbTextAlign TextAlign { get; set; }

        public override string GetDesignHtml()
        {
            return "<div class='EbLocFieldImg dropped' eb-type='LocFieldImage' id='@id' style='border: @Border px solid;border-color: @BorderColor ;width: @Width px; background: url(../images/image.png) center no-repeat ; background-size: cover; height: @Height px; left: @Left px; top: @Top px;'></div>".RemoveCR().DoubleQuoted();
        }
        public override string GetJsInitFunc()
        {
            return @"
    this.Init = function(id)
        {
    this.Height =40;
    this.Width= 40;
    this.Border = 1;
this.BorderColor = '#eae6e6';
};";
        }
        public override void DrawMe(Document d, byte[] fileByte, float reportHeight, float printingTop, float detailprintingtop)
        {
            iTextSharp.text.Image myImage = iTextSharp.text.Image.GetInstance(fileByte);
            myImage.ScaleToFit(this.WidthPt, this.HeightPt);
            myImage.SetAbsolutePosition(this.LeftPt, reportHeight - (printingTop + this.TopPt + this.HeightPt + detailprintingtop));
            myImage.Alignment = (int)TextAlign;
            d.Add(myImage);
        }

    }

    [EnableInBuilder(BuilderType.Report)]
    public class EbLocFieldText : EbReportField
    {
        [EnableInBuilder(BuilderType.Report)]
        [PropertyGroup("Data Settings")]
        [UIproperty]
        public Boolean RenderInMultiLine { get; set; } = true;
        public override string GetDesignHtml()
        {
            return "<div class='EbLocFieldText dropped' eb-type='LocFieldText' id='@id' style='border: @Border px solid;border-color: @BorderColor ; width: @Width px; height: @Height px; background-color:@BackColor ; color:@ForeColor ; left: @Left px; top: @Top px;text-align: @TextAlign ;'> @Title </div>".RemoveCR().DoubleQuoted();
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

        public override void DrawMe(PdfContentByte canvas, float reportHeight, float printingTop, float detailprintingtop, string column_val)
        {
            var urx = this.WidthPt + this.LeftPt;
            var ury = reportHeight - (printingTop + this.TopPt + detailprintingtop);
            var llx = this.LeftPt;
            var lly = reportHeight - (printingTop + this.TopPt + this.HeightPt + detailprintingtop);
            Phrase phrase = null;
            phrase = new Phrase(column_val, ITextFont);
            ColumnText ct = new ColumnText(canvas);
            ct.SetSimpleColumn(phrase, llx, lly, urx, ury, 15, (int)TextAlign);
            ct.Go();
        }
    }

