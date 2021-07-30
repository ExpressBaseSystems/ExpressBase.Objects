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
using ServiceStack;
using ExpressBase.Common.Constants;
using ExpressBase.CoreBase.Globals;
using FontStyle = ExpressBase.Common.FontStyle;

namespace ExpressBase.Objects
{
    public abstract class EbReportField : EbReportObject
    {
        [EnableInBuilder(BuilderType.Report)]
        [UIproperty]
        [PropertyGroup(PGConstants.APPEARANCE)]
        [PropertyEditor(PropertyEditorType.Color)]
        public virtual string ForeColor { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        [UIproperty]
        [PropertyGroup(PGConstants.APPEARANCE)]
        public virtual EbTextAlign TextAlign { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        [UIproperty]
        [HideInPropertyGrid]
        public string ParentName { get; set; }//only for builder

        [EnableInBuilder(BuilderType.Report)]
        [UIproperty]
        [PropertyGroup(PGConstants.APPEARANCE)]
        public int Border { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        [UIproperty]
        [PropertyEditor(PropertyEditorType.Color)]
        [PropertyGroup(PGConstants.APPEARANCE)]
        public string BorderColor { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        [PropertyGroup(PGConstants.APPEARANCE)]
        [UIproperty]
        [PropertyEditor(PropertyEditorType.FontSelector)]
        public virtual EbFont Font { get; set; }

        [JsonIgnore]
        public float Llx
        {
            get
            {
                return LeftPt;
            }
        }

        [JsonIgnore]
        public float Urx
        {
            get
            {
                return WidthPt + LeftPt;
            }
        }

        [EnableInBuilder(BuilderType.Report)]
        [PropertyEditor(PropertyEditorType.ScriptEditorCS)]
        public EbScript LayoutExpression { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        [PropertyEditor(PropertyEditorType.ScriptEditorCS)]
        public EbScript HideExpression { get; set; }

        public bool IsHidden { get; set; }

        public BaseColor GetColor(string Color)
        {
            int colr = ColorTranslator.FromHtml(Color).ToArgb();
            return new BaseColor(colr);
        }

        public Phrase GetFormattedPhrase(EbFont Font, EbFont _reportFont, string text)
        {
            iTextSharp.text.Font iTextFont = null;
            if (Font is null)
            {
                if (!(_reportFont is null))
                    Font = _reportFont;
                else
                    Font = (new EbFont { color = "#000000", FontName = "Roboto", Caps = false, Size = 10, Strikethrough = false, Style = 0, Underline = false });
            }
            iTextFont = FontFactory.GetFont(Font.FontName, Font.Size, (int)Font.Style);
            iTextFont.Color = GetColor(Font.color);
            if (Font.Caps)
                text = text.ToUpper();
            if (Font.Strikethrough)
                iTextFont.SetStyle(iTextSharp.text.Font.STRIKETHRU);
            if (Font.Underline)
                iTextFont.SetStyle(iTextSharp.text.Font.UNDERLINE);
            return new Phrase(text, iTextFont);
        }

        public iTextSharp.text.Font GetItextFont(EbFont Font, EbFont _reportFont)
        {
            iTextSharp.text.Font iTextFont = null;
            if (Font is null)
            {
                if (!(_reportFont is null))
                    Font = _reportFont;
                else
                    Font = (new EbFont { color = "#000000", FontName = "Times-Roman", Caps = false, Size = 8, Strikethrough = false, Style = 0, Underline = false });
            }
            iTextFont = FontFactory.GetFont(Font.FontName, Font.Size, (int)Font.Style);
            iTextFont.Color = GetColor(Font.color);
            if (Font.Caps)
                Title = Title.ToUpper();
            if (Font.Strikethrough)
                iTextFont.SetStyle(iTextSharp.text.Font.STRIKETHRU);
            if (Font.Underline)
                iTextFont.SetStyle(iTextSharp.text.Font.UNDERLINE);
            return iTextFont;
        }

        public virtual void DrawMe(float printingTop, EbReport Rep, List<Param> Linkparams, int slno) { }

        public string FormatDate(string column_val, DateFormatReport format, EbReport Rep)
        {
            DateTime dt = Convert.ToDateTime(column_val);
            if (format == DateFormatReport.dddd_MMMM_d_yyyy)
                return String.Format("{0:dddd, MMMM d, yyyy}", dt);
            else if (format == DateFormatReport.M_d_yyyy)
                return String.Format("{0:M/d/yyyy}", dt);
            else if (format == DateFormatReport.ddd_MMM_d_yyyy)
                return String.Format("{0:ddd, MMM d, yyyy}", dt);
            else if (format == DateFormatReport.MM_dd_yy)
                return String.Format("{0:MM/dd/yy}", dt);
            else if (format == DateFormatReport.MM_dd_yyyy)
                return String.Format("{0:MM/dd/yyyy}", dt);
            else if (format == DateFormatReport.dd_MM_yyyy)
                return String.Format("{0:dd-MM-yyyy}", dt);
            else if (format == DateFormatReport.dd_MM_yyyy_slashed)
                return string.Format("{0:dd/MM/yyyy}", dt);
            else if (format == DateFormatReport.from_culture)
                return String.Format(Rep.CultureInfo.DateTimeFormat, dt.ToString());
            else if (format == DateFormatReport.dd_MMMM_yyyy)
                return string.Format("{0:dd MMMM yyyy}", dt);
            return column_val;
        }

        public void SetValuesFromGlobals(PdfGReportField field)
        {
            LeftPt = field.Left;
            WidthPt = field.Width;
            TopPt = field.Top;
            HeightPt = field.Height;
            BackColor = field.BackColor;
            ForeColor = field.ForeColor;
            IsHidden = field.IsHidden;
            if (field.Font != null)
                Font = new EbFont
                {
                    Caps = field.Font.Caps,
                    color = field.Font.color,
                    FontName = field.Font.FontName,
                    Size = field.Font.Size,
                    Strikethrough = field.Font.Strikethrough,
                    Style = (FontStyle)(int)field.Font.Style,
                    Underline = field.Font.Underline
                };

        }
    }

    [EnableInBuilder(BuilderType.Report)]
    public class EbImg : EbReportField
    {
        [EnableInBuilder(BuilderType.Report)]
        [UIproperty]
        [MetaOnly]
        public string Source { get; set; }

        public override EbTextAlign TextAlign { get; set; }

        public override string Title { get; set; }

        public override EbFont Font { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        [PropertyEditor(PropertyEditorType.ImageSeletor)]
        [PropertyGroup("Image")]
        public int ImageRefId { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        [PropertyGroup("Image")]
        public string ImageColName { get; set; }

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
        public override void DrawMe(float printingTop, EbReport Rep, List<Param> Linkparams, int slno)
        {
            byte[] fileByte = new byte[0]; ;
            if (ImageRefId != 0)
                fileByte = Rep.GetImage(ImageRefId);
            else if (!string.IsNullOrEmpty(ImageColName))
            {
                dynamic val = Rep.GetDataFieldValue(ImageColName.Split(".")[1], slno, Convert.ToInt32(ImageColName.Split(".")[0].Substring(1)));
                Console.WriteLine("Image DrawMe val = " + ImageColName + ":" + val);
                if (val != null)
                    if (val is byte[])
                        fileByte = val;
                    else if (val is string && val.ToString() != string.Empty && Convert.ToInt32(val) != 0)
                        fileByte = Rep.GetImage(Convert.ToInt32(val));
            }

            if (fileByte.Length != 0)
            {
                iTextSharp.text.Image myImage = iTextSharp.text.Image.GetInstance(fileByte);
                myImage.ScaleToFit(WidthPt, HeightPt);
                myImage.SetAbsolutePosition(LeftPt, Rep.HeightPt - (printingTop + TopPt + HeightPt + Rep.detailprintingtop));
                myImage.Alignment = (int)TextAlign;
                Rep.Doc.Add(myImage);
            }
        }
    }

    [EnableInBuilder(BuilderType.Report)]
    public class EbWaterMark : EbReportField
    {
        [EnableInBuilder(BuilderType.Report)]
        [UIproperty]
        [MetaOnly]
        public string Source { get; set; }

        public override string Title { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        [PropertyEditor(PropertyEditorType.ImageSeletor)]
        [PropertyGroup("Image")]
        public int ImageRefId { get; set; }

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
        [PropertyGroup(PGConstants.APPEARANCE)]
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

        public override void DrawMe(float printingTop, EbReport Rep, List<Param> Linkparams, int slno)
        {
            Phrase phrase = null;
            if (WaterMarkText != string.Empty)
            {
                phrase = GetFormattedPhrase(this.Font, Rep.Font, WaterMarkText);
                PdfContentByte canvas;
                canvas = Rep.Writer.DirectContentUnder;
                ColumnText.ShowTextAligned(canvas, (int)TextAlign, phrase, Rep.Doc.PageSize.Width / 2, Rep.Doc.PageSize.Height / 2, Rotation);
            }
            if (ImageRefId != 0)
            {
                iTextSharp.text.Image img = iTextSharp.text.Image.GetInstance(Rep.WatermarkImages[ImageRefId]);
                img.RotationDegrees = Rotation;
                img.ScaleToFit(WidthPt, HeightPt);
                img.SetAbsolutePosition(LeftPt, Rep.HeightPt - TopPt - HeightPt);
                PdfGState _state = new PdfGState() { FillOpacity = 0.1F, StrokeOpacity = 0.1F };
                PdfContentByte cb = Rep.Writer.DirectContentUnder;
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
        public DateFormatReport Format { get; set; } = DateFormatReport.from_culture;

        [OnChangeExec(@"
            pg.MakeReadOnly('Title');
        ")]
        [EnableInBuilder(BuilderType.Report)]
        public override string Title { set; get; }

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

        public override void DrawMe(float printingTop, EbReport Rep, List<Param> Linkparams, int slno)
        {

            float ury = Rep.HeightPt - (printingTop + TopPt + Rep.detailprintingtop);
            float lly = Rep.HeightPt - (printingTop + TopPt + HeightPt + Rep.detailprintingtop);
            string column_val = FormatDate(Rep.CurrentTimestamp.ToString(), Format, Rep);
            Phrase phrase = GetFormattedPhrase(this.Font, Rep.Font, column_val);
            ColumnText ct = new ColumnText(Rep.Canvas);
            ct.SetSimpleColumn(phrase, Llx, lly, Urx, ury, 15, (int)TextAlign);
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
        public override void DrawMe(float printingTop, EbReport Rep, List<Param> Linkparams, int slno)
        {

            float ury = Rep.HeightPt - (printingTop + TopPt + Rep.detailprintingtop);
            float lly = Rep.HeightPt - (printingTop + TopPt + HeightPt + Rep.detailprintingtop + Rep.RowHeight);

            ColumnText ct = new ColumnText(Rep.Canvas);
            Phrase phrase = GetFormattedPhrase(this.Font, Rep.Font, Rep.PageNumber.ToString());
            ct.SetSimpleColumn(phrase, Llx, lly, Urx, ury, 15, (int)TextAlign);
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
        public override void DrawMe(float printingTop, EbReport Rep, List<Param> Linkparams, int slno)
        {
            float ury = Rep.HeightPt - (printingTop + TopPt + Rep.detailprintingtop);
            float lly = Rep.HeightPt - (printingTop + TopPt + HeightPt + Rep.detailprintingtop + Rep.RowHeight);

            ColumnText ct = new ColumnText(Rep.Canvas);
            Phrase phrase = GetFormattedPhrase(this.Font, Rep.Font, Rep.PageNumber + "/"/* + writer.PageCount*/);
            ct.SetSimpleColumn(phrase, Llx, lly, Urx, ury, 15, (int)TextAlign);
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
        public override void DrawMe(float printingTop, EbReport Rep, List<Param> Linkparams, int slno)
        {
            float ury = Rep.HeightPt - (printingTop + TopPt + Rep.detailprintingtop);
            float lly = Rep.HeightPt - (printingTop + TopPt + HeightPt + Rep.detailprintingtop + Rep.RowHeight);

            ColumnText ct = new ColumnText(Rep.Canvas);
            Phrase phrase = GetFormattedPhrase(this.Font, Rep.Font, Rep.RenderingUser?.FullName ?? "Machine User");
            ct.SetSimpleColumn(phrase, Llx, lly, Urx, ury, 15, (int)TextAlign);
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
        public override void DrawMe(float printingTop, EbReport Rep, List<Param> Linkparams, int slno)
        {
            float ury = Rep.HeightPt - (printingTop + TopPt + Rep.detailprintingtop);
            float lly = Rep.HeightPt - (printingTop + TopPt + HeightPt + Rep.detailprintingtop + Rep.RowHeight);

            ColumnText ct = new ColumnText(Rep.Canvas);
            Phrase phrase = GetFormattedPhrase(this.Font, Rep.Font, Title);
            ct.SetSimpleColumn(phrase, Llx, lly, Urx, ury, 15, (int)TextAlign);
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
        public override void DrawMe(float printingTop, EbReport Rep, List<Param> Linkparams, int slno)
        {
            string column_val = "";
            foreach (Param p in Rep.Parameters)
                if (p.Name == Title)
                    column_val = p.Value;
            float ury = Rep.HeightPt - (printingTop + TopPt + Rep.detailprintingtop);
            float lly = Rep.HeightPt - (printingTop + TopPt + HeightPt + Rep.detailprintingtop);
            Phrase phrase = GetFormattedPhrase(this.Font, Rep.Font, column_val);
            ColumnText ct = new ColumnText(Rep.Canvas);
            ct.SetSimpleColumn(phrase, Llx, lly, Urx, ury, 15, (int)TextAlign);
            ct.Go();
        }
    }

    [EnableInBuilder(BuilderType.Report)]
    public class EbParamNumeric : EbReportField
    {
        [OnChangeExec(@"
            pg.MakeReadOnly('Title');
        ")]
        [EnableInBuilder(BuilderType.Report)]
        public override string Title { set; get; }

        public override string GetDesignHtml()
        {
            return "<div class='Parameter dropped' eb-type='ParamNumeric' id='@id' style='border: @Border px solid;border-color: @BorderColor ; width: @Width px; height: @Height px; background-color:@BackColor ; color:@ForeColor ; left: @Left px; top: @Top px;text-overflow: ellipsis;overflow: hidden;text-align: @TextAlign ;'> @Title </div>".RemoveCR().DoubleQuoted();
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
        public override void DrawMe(float printingTop, EbReport Rep, List<Param> Linkparams, int slno)
        {
            string column_val = "";
            foreach (Param p in Rep.Parameters)
                if (p.Name == Title)
                    column_val = p.Value;
            float ury = Rep.HeightPt - (printingTop + TopPt + Rep.detailprintingtop);
            float lly = Rep.HeightPt - (printingTop + TopPt + HeightPt + Rep.detailprintingtop);
            Phrase phrase = GetFormattedPhrase(this.Font, Rep.Font, column_val);
            ColumnText ct = new ColumnText(Rep.Canvas);
            ct.SetSimpleColumn(phrase, Llx, lly, Urx, ury, 15, (int)TextAlign);
            ct.Go();
        }

    }
    [EnableInBuilder(BuilderType.Report)]
    public class EbParamText : EbReportField
    {
        [OnChangeExec(@"
            pg.MakeReadOnly('Title');
        ")]
        [EnableInBuilder(BuilderType.Report)]
        public override string Title { set; get; }

        public override string GetDesignHtml()
        {
            return "<div class='Parameter dropped' eb-type='ParamText' id='@id' style='border: @Border px solid;border-color: @BorderColor ; width: @Width px; height: @Height px; background-color:@BackColor ; color:@ForeColor ; left: @Left px; top: @Top px;text-overflow: ellipsis;overflow: hidden;text-align: @TextAlign ;'> @Title </div>".RemoveCR().DoubleQuoted();
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
        public override void DrawMe(float printingTop, EbReport Rep, List<Param> Linkparams, int slno)
        {
            string column_val = "";
            foreach (Param p in Rep.Parameters)
                if (p.Name == Title)
                    column_val = p.Value;
            float ury = Rep.HeightPt - (printingTop + TopPt + Rep.detailprintingtop);
            float lly = Rep.HeightPt - (printingTop + TopPt + HeightPt + Rep.detailprintingtop);
            Phrase phrase = GetFormattedPhrase(this.Font, Rep.Font, column_val);
            ColumnText ct = new ColumnText(Rep.Canvas);
            ct.SetSimpleColumn(phrase, Llx, lly, Urx, ury, 15, (int)TextAlign);
            ct.Go();
        }
    }

    [EnableInBuilder(BuilderType.Report)]
    public class EbParamDateTime : EbReportField
    {
        [OnChangeExec(@"
            pg.MakeReadOnly('Title');
        ")]
        [EnableInBuilder(BuilderType.Report)]
        public override string Title { set; get; }

        [EnableInBuilder(BuilderType.Report)]
        [PropertyGroup("Data Settings")]
        public DateFormatReport Format { get; set; } = DateFormatReport.from_culture;

        public override string GetDesignHtml()
        {
            return "<div class='Parameter dropped' eb-type='ParamDateTime' id='@id' style='border: @Border px solid;border-color: @BorderColor ; width: @Width px; height: @Height px; background-color:@BackColor ; color:@ForeColor ; left: @Left px; top: @Top px;text-overflow: ellipsis;overflow: hidden;text-align: @TextAlign ;'> @Title </div>".RemoveCR().DoubleQuoted();
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
        public override void DrawMe(float printingTop, EbReport Rep, List<Param> Linkparams, int slno)
        {
            string column_val = "";
            foreach (Param p in Rep.Parameters)
                if (p.Name == Title)
                    column_val = p.Value;
            column_val = FormatDate(column_val, Format, Rep);
            float ury = Rep.HeightPt - (printingTop + TopPt + Rep.detailprintingtop);
            float lly = Rep.HeightPt - (printingTop + TopPt + HeightPt + Rep.detailprintingtop);
            Phrase phrase = GetFormattedPhrase(this.Font, Rep.Font, column_val);
            ColumnText ct = new ColumnText(Rep.Canvas);
            ct.SetSimpleColumn(phrase, Llx, lly, Urx, ury, 15, (int)TextAlign);
            ct.Go();
        }

    }

    [EnableInBuilder(BuilderType.Report)]
    public class EbParamBoolean : EbReportField
    {
        [OnChangeExec(@"
            pg.MakeReadOnly('Title');
        ")]
        [EnableInBuilder(BuilderType.Report)]
        public override string Title { set; get; }

        public override string GetDesignHtml()
        {
            return "<div class='Parameter dropped' eb-type='ParamBoolean' id='@id' style='border: @Border px solid;border-color: @BorderColor ; width: @Width px; height: @Height px; background-color:@BackColor ; color:@ForeColor ; left: @Left px; top: @Top px;text-overflow: ellipsis;overflow: hidden;text-align: @TextAlign ;'> @Title </div>".RemoveCR().DoubleQuoted();
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
        public override void DrawMe(float printingTop, EbReport Rep, List<Param> Linkparams, int slno)
        {
            string column_val = "";
            foreach (Param p in Rep.Parameters)
                if (p.Name == Title)
                    column_val = p.Value;
            float ury = Rep.HeightPt - (printingTop + TopPt + Rep.detailprintingtop);
            float lly = Rep.HeightPt - (printingTop + TopPt + HeightPt + Rep.detailprintingtop);
            Phrase phrase = GetFormattedPhrase(this.Font, Rep.Font, column_val);
            ColumnText ct = new ColumnText(Rep.Canvas);
            ct.SetSimpleColumn(phrase, Llx, lly, Urx, ury, 15, (int)TextAlign);
            ct.Go();
        }
    }

    [EnableInBuilder(BuilderType.Report)]
    public class EbBarcode : EbReportField
    {
        public override string ForeColor { get; set; }

        public override string Title { get; set; }

        public override EbFont Font { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        [UIproperty]
        [MetaOnly]
        public string Source { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        [UIproperty]
        [PropertyGroup(PGConstants.DATA)]
        public string Code { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        [UIproperty]
        [PropertyGroup("General")]
        public int Type { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        [UIproperty]
        [PropertyGroup(PGConstants.APPEARANCE)]
        public bool GuardBars { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        [UIproperty]
        [PropertyGroup(PGConstants.APPEARANCE)]
        public float BaseLine { get; set; }

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
        public override void DrawMe(float printingTop, EbReport Rep, List<Param> Linkparams, int slno)
        {
            int tableIndex = Convert.ToInt32(Code.Split('.')[0].Substring(1));
            string column_name = Code.Split('.')[1];
            string column_val = Rep.GetDataFieldValue(column_name, slno, tableIndex);
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
                uccEan128.Code = column_val;
                uccEan128.GuardBars = GuardBars;
                uccEan128.Baseline = BaseLine;
                imageEAN = uccEan128.CreateImageWithBarcode(cb: Rep.Canvas, barColor: null, textColor: null);
                //}

                // imageEAN.ScaleAbsolute(Width, Height);
                imageEAN.SetAbsolutePosition(LeftPt, Rep.HeightPt - (printingTop + TopPt + HeightPt + Rep.detailprintingtop));
                Rep.Doc.Add(imageEAN);
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.ToString());
                ColumnText ct = new ColumnText(Rep.Canvas);
                float x = Rep.HeightPt - (printingTop + TopPt + Rep.detailprintingtop);
                ct.SetSimpleColumn(new Phrase("Error in generating barcode"), LeftPt, x - HeightPt, LeftPt + WidthPt, x, 15, (int)TextAlign);
                ct.Go();
            }
        }
    }

    [EnableInBuilder(BuilderType.Report)]
    public class EbQRcode : EbReportField
    {
        public override string ForeColor { get; set; }

        public override string Title { get; set; }

        public override EbFont Font { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        [UIproperty]
        [MetaOnly]
        public string Source { get; set; }

        [EnableInBuilder(BuilderType.Report)]
        [UIproperty]
        [PropertyGroup(PGConstants.DATA)]
        public string Code { get; set; }

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
        public override void DrawMe(float printingTop, EbReport Rep, List<Param> Linkparams, int slno)
        {
            try
            {
                int tableIndex = Convert.ToInt32(Code.Split('.')[0].Substring(1));
                string column_name = Code.Split('.')[1];
                string column_val = Rep.GetDataFieldValue(column_name, slno, tableIndex);
                QRCodeGenerator qrGenerator = new QRCodeGenerator();
                QRCodeData qrCodeData = qrGenerator.CreateQrCode(column_val, QRCodeGenerator.ECCLevel.Q);
                BitmapByteQRCode qrCode = new BitmapByteQRCode(qrCodeData);
                byte[] qrCodeImage = qrCode.GetGraphic(20);
                iTextSharp.text.Image img = iTextSharp.text.Image.GetInstance(qrCodeImage);
                img.SetAbsolutePosition(LeftPt, Rep.HeightPt - (printingTop + TopPt + HeightPt + Rep.detailprintingtop));
                img.ScaleAbsolute(WidthPt, HeightPt);
                Rep.Doc.Add(img);
            }
            catch (Exception e)
            {
                ColumnText ct = new ColumnText(Rep.Canvas);
                float x = Rep.HeightPt - (printingTop + TopPt + Rep.detailprintingtop);
                ct.SetSimpleColumn(new Phrase("Error in generating barcode"), LeftPt, x - HeightPt, LeftPt + WidthPt, x, 15, (int)TextAlign);
                ct.Go();
                Console.WriteLine("Exception: " + e.ToString());
            }
        }
    }

    [EnableInBuilder(BuilderType.Report)]
    public class EbSerialNumber : EbReportField
    {
        [OnChangeExec(@"
            pg.MakeReadOnly('Title');
        ")]
        [EnableInBuilder(BuilderType.Report)]
        public override string Title { set; get; }

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
                    this.Title = 'slno';
                };";
        }
        public override void DrawMe(float printingTop, EbReport Rep, List<Param> Linkparams, int slno)
        {
            float ury = Rep.HeightPt - (printingTop + TopPt + Rep.detailprintingtop);
            float lly = Rep.HeightPt - (printingTop + TopPt + HeightPt + Rep.detailprintingtop);

            Phrase phrase = GetFormattedPhrase(this.Font, Rep.Font, (Rep.iDetailRowPos + 1).ToString());
            ColumnText ct = new ColumnText(Rep.Canvas);
            ct.SetSimpleColumn(phrase, Llx, lly, Urx, ury, 15, (int)TextAlign);
            ct.Go();
        }
    }

    [EnableInBuilder(BuilderType.Report)]
    public class EbLocFieldImage : EbReportField
    {
        public override EbFont Font { get; set; }

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
        public override void DrawMe(float printingTop, EbReport Rep, List<Param> Linkparams, int slno)
        {
            byte[] fileByte = Rep.GetImage(Convert.ToInt32(Rep.Solution.Locations[42][Title]));
            if (!fileByte.IsEmpty())
            {
                iTextSharp.text.Image myImage = iTextSharp.text.Image.GetInstance(fileByte);
                myImage.ScaleToFit(WidthPt, HeightPt);
                myImage.SetAbsolutePosition(Llx, Rep.HeightPt - (printingTop + TopPt + HeightPt + Rep.detailprintingtop));
                myImage.Alignment = (int)TextAlign;
                Rep.Doc.Add(myImage);
            }
        }

    }

    [EnableInBuilder(BuilderType.Report)]
    public class EbLocFieldText : EbReportField
    {
        [OnChangeExec(@"
            pg.MakeReadOnly('Title');
        ")]
        [EnableInBuilder(BuilderType.Report)]
        public override string Title { set; get; }

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

        public override void DrawMe(float printingTop, EbReport Rep, List<Param> Linkparams, int slno)
        {
            string column_val = Rep.Solution.Locations[42][Title];
            float ury = Rep.HeightPt - (printingTop + TopPt + Rep.detailprintingtop);
            float lly = Rep.HeightPt - (printingTop + TopPt + HeightPt + Rep.detailprintingtop);

            Phrase phrase = GetFormattedPhrase(this.Font, Rep.Font, column_val);
            ColumnText ct = new ColumnText(Rep.Canvas);
            ct.SetSimpleColumn(phrase, Llx, lly, Urx, ury, 15, (int)TextAlign);
            ct.Go();
        }
    }
}

