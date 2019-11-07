using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExpressBase.Objects
{
    public abstract class EbMobilePageBase : EbObject
    {

    }

    [EnableInBuilder(BuilderType.MobilePage)]
    [BuilderTypeEnum(BuilderType.MobilePage)]
    public class EbMobilePage : EbMobilePageBase,IEBRootObject
    {

    }
}
