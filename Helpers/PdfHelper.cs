using ExpressBase.Common;
using ExpressBase.Common.Data;
using ExpressBase.Objects.ServiceStack_Artifacts;
using iTextSharp.text;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;
using Microsoft.CodeAnalysis.Scripting;
using ServiceStack.Redis;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ExpressBase.Objects.Helpers
{
    public partial class HeaderFooter : PdfPageEventHelper
    {
        private EbReport Report { get; set; }

        public override void OnStartPage(PdfWriter writer, Document document)
        {
        }

        public override void OnEndPage(PdfWriter writer, Document d)
        {
            //var content = writer.DirectContent;
            //var pageBorderRect = new Rectangle(Report.Doc.PageSize);

            //pageBorderRect.Left += Report.Doc.LeftMargin;
            //pageBorderRect.Right -= Report.Doc.RightMargin;
            //pageBorderRect.Top -= Report.Doc.TopMargin;
            //pageBorderRect.Bottom += Report.Doc.BottomMargin;

            //content.SetColorStroke(BaseColor.Red);
            //content.Rectangle(pageBorderRect.Left, pageBorderRect.Bottom, pageBorderRect.Width, pageBorderRect.Height);
            //content.Stroke();

            if (!Report.FooterDrawn && (Report?.DataSet?.Tables[Report.DetailTableIndex]?.Rows.Count > 0))
            {
                if (!(Report.PageNumber == 1))
                    Report.DrawReportHeader(true);
                Report.DrawPageHeader();
                Report.DrawPageFooter();
                if (!Report.IsLastpage)
                    Report.DrawReportFooter(true);
            }
            //if (Report.IsLastpage == true)
            //    Report.DrawReportFooter();
            Report.DrawWaterMark(d, writer);
            Report.SetDetail();
        }

        public HeaderFooter(EbReport _c) : base()
        {
            Report = _c;
        }
    }
}
