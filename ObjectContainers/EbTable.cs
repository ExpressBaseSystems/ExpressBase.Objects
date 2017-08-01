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
    [EnableInBuilder(BuilderType.WebFormBuilder, BuilderType.FilterDialogBuilder)]
    public class EbTableLayout : EbControlContainer
    {
        [EnableInBuilder(BuilderType.WebFormBuilder, BuilderType.FilterDialogBuilder)]
        public string Columns { get; set; }

        public EbTableLayout() { }

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
            string html = "<table><tr>";

            foreach (EbControl ec in base.Controls)
                html += ec.GetHtml();

            return html + "</tr></table>";
        }
    }

    [ProtoBuf.ProtoContract]
    [EnableInBuilder(BuilderType.WebFormBuilder, BuilderType.FilterDialogBuilder)]
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
            string html = "<td>";

            foreach (EbControl ec in base.Controls)
                html += ec.GetHtml();

            return html + "</td>";
        }
    }
}
