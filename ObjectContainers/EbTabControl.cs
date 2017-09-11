using ExpressBase.Common.Extensions;
using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExpressBase.Objects.ObjectContainers
{
    [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog)]
    public class EbTabControl : EbControlContainer
    {
        public EbTabControl() { }
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
            return @"";
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
            string html = "<div id='menu1' class='tab-pane fade'>";

            foreach (EbControl ec in base.Controls)
                html += ec.GetHtml();

            return html + "</div>";
        }
    }
}
