using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using ExpressBase.Data;
using ServiceStack.Redis;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace ExpressBase.Objects
{
    [EnableInBuilder(BuilderType.WebForm)]
    [HideInToolBox]
    public class EbWebForm : EbControlContainer
    {
        [Browsable(false)]
        public bool IsUpdate { get; set; }

        //[EnableInBuilder(BuilderType.WebForm)]
        public string Description = "form hardCode desc";// { get; set; }

        public EbWebForm() { }

        public override string GetHead()
        {
            string head = string.Empty;

            foreach (EbControl c in this.Controls)
                head += c.GetHead();

            return head;
        }

        public override string GetHtml()
        {
            string html = "<form id='@name@' class='eb-form'>";

            foreach (EbControl c in this.Controls)
                html += c.GetHtml();

            html += "</form>";

            return html.Replace("@name@", this.Name);
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
