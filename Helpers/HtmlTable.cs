using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExpressBase.Objects
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

        public List<HtmlCell> Cells { get; set; }

        public HtmlRow(string name)
        {
            this.Name = name;
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

        public HtmlCell(string name)
        {
            this.Name = name;
        }

        public string GetHtml()
        {
            return "<td></td>";
        }
    }
}
