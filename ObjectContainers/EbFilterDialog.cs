using ExpressBase.Common.Extensions;
using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExpressBase.Objects
{
    [EnableInBuilder(BuilderType.FilterDialog)]
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

            html += string.Format("<input type='hidden' name='all_control_names' id='all_control_names' value='{0}' />", string.Join(",", ControlNames));

            return html;
        }
       

        public IEnumerable<string> ControlNames
        {
            get
            {
                foreach (EbControl _c in this.Controls.Flatten())
                {
                    if (!(_c is EbControlContainer))
                        yield return _c.Name;
                }
            }
        }
    }
}
