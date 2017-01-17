using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace ExpressBase.Objects
{
    [ProtoBuf.ProtoContract]
    public class EbTableLayout : EbControl
    {
        [ProtoBuf.ProtoMember(1)]
        [Browsable(false)]
        public List<EbTableColumn> Columns { get; set; }

        [ProtoBuf.ProtoMember(2)]
        [Browsable(false)]
        public List<EbTableRow> Rows { get; set; }

        public EbTableLayout() { }

        public override string GetHtml()
        {
            string html = GetTable();

            if (base.Controls != null)
            {
                foreach (EbControl ec in base.Controls)
                    html = html.Replace(string.Format("{0}_{1}_{2}", this.Name, ec.CellPositionColumn, ec.CellPositionRow), ec.GetHtml());
            }

            return html;
        }

        private string GetTable()
        {
            HtmlTable ht = new HtmlTable(this.Name);

            foreach (EbTableRow row in this.Rows)
            {
                HtmlRow hr = new HtmlRow(this.Name, row);

                foreach (EbTableColumn col in this.Columns)
                    hr.Cells.Add(new HtmlCell(this.Name, col, row));

                ht.Rows.Add(hr);
            }

            return ht.GetHtml();
        }
    }

    [ProtoBuf.ProtoContract]
    public class EbTableColumn
    {
        [ProtoBuf.ProtoMember(1)]
        public int Index { get; set; }

        [ProtoBuf.ProtoMember(2)]
        public int Width { get; set; }
    }

    [ProtoBuf.ProtoContract]
    public class EbTableRow
    {
        [ProtoBuf.ProtoMember(1)]
        public int Index { get; set; }

        [ProtoBuf.ProtoMember(2)]
        public int Height { get; set; }
    }
}
