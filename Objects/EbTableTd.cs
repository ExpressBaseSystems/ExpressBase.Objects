using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExpressBase.Objects.Objects
{

    [EnableInBuilder(BuilderType.WebFormBuilder, BuilderType.FilterDialogBuilder)]
    [HideInToolBox()]
    public class EbTableTd : EbControlContainer
    {
        public override string BackColor { get; set; }

        public override string ForeColor { get; set; }
    }
}
