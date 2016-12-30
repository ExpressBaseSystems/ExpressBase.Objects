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
            string html = "<table border='1' style='table-layout: fixed;' width='100%' height='auto'>";
            foreach (HtmlRow r in Rows)
                html += r.GetHtml();

            return html + "</table>";
        }
    }

    public class HtmlRow
    {
        public string Name { get; set; }
        private EbTableRow Row { get; set; }
        public List<HtmlCell> Cells { get; set; }

        public HtmlRow(string name, EbTableRow row)
        {
            this.Name = name;
            this.Row = row;
            this.Cells = new List<HtmlCell>();
        }

        public string GetHtml()
        {
            string html = string.Format("<tr height='{0}'>", (this.Row.Height > 0) ? this.Row.Height.ToString() : "auto");
            foreach (HtmlCell c in Cells)
                html += c.GetHtml();

            return html + "</tr>";
        }
    }

    public class HtmlCell
    {
        public string Name { get; set; }
        private EbTableColumn Column { get; set; }
        private EbTableRow Row { get; set; }

        public HtmlCell(string name, EbTableColumn c, EbTableRow r)
        {
            this.Name = name;
            this.Column = c;
            this.Row = r;
        }

        public string GetHtml()
        {
            return string.Format("<td width='{0}%'>", this.Column.Width) + string.Format("{0}_{1}_{2}", this.Name, this.Column.Index, this.Row.Index) + "</td>";
        }
    }
}
