using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExpressBase.Objects
{
    [EnableInBuilder(BuilderType.UserControl)]
    [HideInToolBox]
    public class EbUserControl : EbControlContainer
    {
        public EbUserControl() { }

    }
}
