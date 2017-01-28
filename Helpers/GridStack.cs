using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExpressBase.Objects
{
    public class GridStack
    {
        public string Name { get; set; }
        public List<GridStackRow> Rows { get; set; }

        public GridStack(string name)
        {
            this.Name = name;
            this.Rows = new List<GridStackRow>();
        }

        public string GetHtml()
        {
            string html = string.Format("<div id='grid-stack-{0}' class='grid-stack'>", this.Name);
            foreach (GridStackRow r in Rows)
                html += r.GetHtml();

            return html + "</div>";
        }
    }

    public class GridStackRow
    {
        public string Name { get; set; }
        private EbTableRow Row { get; set; }
        public List<GridStackCell> Cells { get; set; }

        public GridStackRow(string name, EbTableRow row)
        {
            this.Name = name;
            this.Row = row;
            this.Cells = new List<GridStackCell>();
        }

        public string GetHtml()
        {
            string html = string.Empty;
            foreach (GridStackCell c in Cells)
                html += c.GetHtml();

            return html;
        }
    }

    public class GridStackCell
    {
        public string Name { get; set; }
        private EbTableLayoutColumn Column { get; set; }
        private EbTableRow Row { get; set; }
        private GridStackRow GridStackRow { get; set; }
        private int Width { get; set; }
        private int Height { get; set; }

        public GridStackCell(string name, EbTableLayoutColumn c, EbTableRow r, GridStackRow gsr)
        {
            this.Name = name;
            this.Column = c;
            this.Row = r;
            this.GridStackRow = gsr;
        }

        public string GetHtml()
        {
            decimal wpercent = Convert.ToDecimal(this.Column.Width) / 100m;
            this.Width = Convert.ToInt32(Math.Round((wpercent * 12m), 0));
            this.Height = 5;

            int row = 0;
            int col = 0;

            foreach (GridStackCell cell in this.GridStackRow.Cells)
            {
                if (cell == this)
                    break;
                col += cell.Width;
                row += cell.Height;
            }

            return string.Format(@"<div class='grid-stack-item' data-gs-x='{1}' data-gs-y='{0}' data-gs-width='{2}' data-gs-height='2'><div class='grid-stack-item-content'>", row, col, this.Width, this.Height) 
                    + string.Format("{0}_{1}_{2}", this.Name, this.Column.Index, this.Row.Index) + "</div></div>";
        }
    }
}
