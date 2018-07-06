using ExpressBase.Common.Data;
using ExpressBase.Common.Extensions;
using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using ExpressBase.Common.Structures;
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

        private List<Param> _paramlist = new List<Param>();
        public List<Param> GetDefaultParams()
        {
            foreach (EbControl c in this.Controls)
            {
                string val = string.Empty;
                if ((c.EbDbType).ToString() == "Decimal")
                    val = "0";
                else if ((c.EbDbType).ToString() == "AnsiString")
                    val = "0";
                else if ((c.EbDbType).ToString() == "String")
                    val = "EB";
                else if ((c as EbDate).ShowDateAs_ == DateShowFormat.Year_Month)
                    val = "01/2018";
                else if ((c as EbDate).ShowDateAs_ == DateShowFormat.Year_Month_Date)
                    val = "01/01/2018";
                Param _p = new Param { Name = c.Name, Type = Convert.ToInt32(c.EbDbType).ToString(), Value = val };
                if (c is EbComboBox) {
                   if ((c as EbComboBox).ValueMember.Type.ToString() == "Int32")
                    {
                        _p.Type ="11";
                    }
                    //_p.Type = Convert.ToInt32((EbDbType)((c as EbComboBox).ValueMember.Type));
                }
                _paramlist.Add(_p);
            }
            return _paramlist;
        }

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
