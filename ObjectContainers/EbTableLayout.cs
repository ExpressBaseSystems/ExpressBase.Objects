using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace ExpressBase.Objects
{
    public enum TableLayOutType
    {
        Table,
        GridStack
    }

    [ProtoBuf.ProtoContract]
    public class EbTableLayout : EbControlContainer
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
            //string html = (type == TableLayOutType.Table) ? GetTable() : GetGridStack();
            string html = GetGridStack();

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

        private string GetGridStack()
        {
            GridStack gs = new GridStack(this.Name);

            foreach (EbTableRow row in this.Rows)
            {
                GridStackRow hr = new GridStackRow(this.Name, row);

                foreach (EbTableColumn col in this.Columns)
                    hr.Cells.Add(new GridStackCell(this.Name, col, row, hr));

                gs.Rows.Add(hr);
            }

            string script = @"<script>
$(function () {
    var options = {
        cell_height: 180,
        animate: true,
        placeholderclass: 'Mygrid-stack-placeholder',
        vertical_margin: 10
    };
    $('#grid-stack-@@@@@@@').gridstack(options);
});
</script>".Replace("@@@@@@@", this.Name);

            return gs.GetHtml() + script;
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
