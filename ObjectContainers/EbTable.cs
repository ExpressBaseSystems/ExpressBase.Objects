using ExpressBase.Objects;
using ExpressBase.Objects.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace ExpressBase.Objects
{
    [ProtoBuf.ProtoContract]
    [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog)]
    public class EbTableLayout : EbControlContainer
    {
        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog)]
        public string Columns { get; set; }

        public EbTableLayout() { }

        public override string GetDesignHtml()
        {
            return @"
<div class='Eb-ctrlContainer' Ctype='TableLayout'>
    <table style='width:100%'   style=' @BackColor  @ForeColor ' >
        <tr>
            <td id='@id_Td0' class='tdDropable' ></td>
            <td id='@id_Td1' class='tdDropable'></td style='min-height:20px;'> 
        </tr>
    </table>
</div>"
    .Replace("@BackColor ", ("background-color:" + ((this.BackColor != null) ? this.BackColor : "@BackColor ") + ";"))
    .Replace("@ForeColor ", "color:" + ((this.ForeColor != null) ? this.ForeColor : "@ForeColor ") + ";").RemoveCR().DoubleQuoted();
        }

        public override string GetJsInitFunc()
        {
            return @"
this.Init = function(id)
{
    this.Controls.Append(new EbObjects.EbTableTdObj(id + '_Td0'));
    this.Controls.Append(new EbObjects.EbTableTdObj(id + '_Td1'));
};
this.Init(id);";
        }

        public override string GetHead()
        {
            string head = string.Empty;

            if (base.Controls != null)
            {
                foreach (EbControl ec in base.Controls)
                    head += ec.GetHead();
            }

            return head;
        }

        public override string GetHtml()
        {
            string html = "<table class='form-render-table' ><tr>";

            foreach (EbControl ec in base.Controls)
                html += ec.GetHtml();

            return html + "</tr></table>";
        }
    }

    [ProtoBuf.ProtoContract]
    [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog)]
    [HideInToolBox]
    public class EbTableTd : EbControlContainer
    {
        public EbTableTd() { }

        public override string GetHead()
        {
            string head = string.Empty;

            if (base.Controls != null)
            {
                foreach (EbControl ec in base.Controls)
                    head += ec.GetHead();
            }

            return head;
        }

        public override string GetHtml()
        {
            string html = "<td class='form-render-table-Td'>";

            foreach (EbControl ec in base.Controls)
                html += ec.GetHtml();

            return html + "</td>";
        }
    }
}
