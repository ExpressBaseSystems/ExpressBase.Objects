using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExpressBase.UI
{
    public class HtmlTable
    {
        public string Name { get; set; }
        public List<HtmlRow> Rows { get; set; }

        public HtmlTable(string name)
        {
            this.Name = name;
            this.Rows = new List<HtmlRow>();
        }

        public string GetHtml()
        {
            string html = "<table border='1'>";
            foreach (HtmlRow r in Rows)
                html += r.GetHtml();

            return html + "</table>";
        }
    }

    public class HtmlRow
    {
        public string Name { get; set; }
        private int Row { get; set; }
        public List<HtmlCell> Cells { get; set; }

        public HtmlRow(string name, int r)
        {
            this.Name = name;
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
        public string Name { get; set; }
        private int Column { get; set; }
        private int Row { get; set; }

        public HtmlCell(string name, int c, int r)
        {
            this.Name = name;
            this.Column = c;
            this.Row = r;
        }

        public string GetHtml()
        {
            return @"<td>" + string.Format("{0}_{1}_{2}", this.Name, this.Column, this.Row) + "</td>";
        }
    }
}
