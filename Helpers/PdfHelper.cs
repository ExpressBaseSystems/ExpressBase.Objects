﻿using ExpressBase.Common;
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
            Report.PageNumber = writer.PageNumber;

            bool needRH = Report.PageNumber == 1 || (Report.ReportHeaderHeightRepeatAsPH > 0);

            if (needRH)
                Report.DrawReportHeader(true);
            if (!Report.DrawDetailCompleted)// omit draw page header if detail section is completed
                Report.DrawPageHeader();

        }

        public override void OnEndPage(PdfWriter writer, Document d)
        {

            if (Report?.DataSet?.Tables[Report.DetailTableIndex]?.Rows.Count > 0)
            {
                Report.DrawPageFooter();
                if (Report.ReportFooterHeightRepeatAsPf > 0)
                    Report.DrawRepeatingReportFooter();

            }

            Report.DrawWaterMark(d, writer);
            Report.SetDetail();
        }

        public HeaderFooter(EbReport _c) : base()
        {
            Report = _c;
        }

        public void DrawBorder()
        { //var content = writer.DirectContent;
          //var pageBorderRect = new Rectangle(Report.Doc.PageSize);

            //pageBorderRect.Left += Report.Doc.LeftMargin;
            //pageBorderRect.Right -= Report.Doc.RightMargin;
            //pageBorderRect.Top -= Report.Doc.TopMargin;
            //pageBorderRect.Bottom += Report.Doc.BottomMargin;

            //content.SetColorStroke(BaseColor.Red);
            //content.Rectangle(pageBorderRect.Left, pageBorderRect.Bottom, pageBorderRect.Width, pageBorderRect.Height);
            //content.Stroke();}
        }
    }
}
