using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExpressBase.UI
{
    public class HtmlTable
    {
        public List<HtmlRow> Rows { get; set; }

        public HtmlTable()
        {
            this.Rows = new List<HtmlRow>();
        }

        public string GetHtml()
        {
            string html = "<table border='1' style='margin-top: 10px;'>";
            foreach (HtmlRow r in Rows)
                html += r.GetHtml();

            return html + "</table>";
        }
    }

    public class HtmlRow
    {
        private int Row { get; set; }
        public List<HtmlCell> Cells { get; set; }

        public HtmlRow(int r)
        {
            this.Row = r;
            this.Cells = new List<HtmlCell>();
        }

        public string GetHtml()
        {
            string html = "<tr>";
            foreach (HtmlCell c in Cells)
                html += c.GetHtml();

            return html + "</tr>";
        }
    }

    public class HtmlCell
    {
        private int Column { get; set; }
        private int Row { get; set; }

        public HtmlCell(int c, int r)
        {
            this.Column = c;
            this.Row = r;
        }

        public string GetHtml()
        {
            return @"<td>" + string.Format("td_{0}_{1}", this.Column, this.Row) + "</td>";
        }
    }
}
