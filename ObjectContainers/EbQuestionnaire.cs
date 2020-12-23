using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExpressBase.Objects
{
    [HideInToolBox]
    [EnableInBuilder(BuilderType.SurveyControl)]
    [BuilderTypeEnum(BuilderType.WebForm)]
    class EbQuestionnaire : EbForm
    {
        public EbQuestionnaire()
        {

        }
    }
}
