using ExpressBase.Objects.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExpressBase.Objects.ObjectContainers
{
    [ProtoBuf.ProtoContract]
    [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog)]
    [HideInToolBox]
    public class EbFilterDialog : EbControlContainer
    {
        public EbFilterDialog() { }

        public override string GetHead()
        {
            string head = string.Empty;

            foreach (EbControl c in this.Controls)
                head += c.GetHead();

            return head;
        }

        public override string GetHtml()
        {
            string html = string.Empty;

            foreach (EbControl c in this.Controls)
                html += c.GetHtml();

            return html;
        }

        public string GetControlNames()
        {
            List<string> _lst = new List<string>();

            //foreach (EbControl _c in this.FlattenedControls)
            //{
            //    if (!(_c is EbControlContainer))
            //        _lst.Add(_c.Name);
            //}

            return string.Join(",", _lst.ToArray());
        }
    }
}
