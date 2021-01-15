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
    public class EbQuestionnaire : EbForm
    {
        public EbQuestionnaire()
        {

        }
        //Collection of EbQuestion
        [HideInPropertyGrid]
        [EnableInBuilder(BuilderType.SurveyControl)]
        public override List<EbControl> Controls { get; set; }
    }

    [HideInToolBox]
    [EnableInBuilder(BuilderType.SurveyControl)]
    public class EbQuestion : EbControlContainer
    {
        [HideInPropertyGrid]
        [EnableInBuilder(BuilderType.SurveyControl)]
        public int QId { get; set; }

        [HideInPropertyGrid]
        [EnableInBuilder(BuilderType.SurveyControl)]
        public override string EbSid { get; set; }

        [HideInPropertyGrid]
        [EnableInBuilder(BuilderType.SurveyControl)]
        public override string Name { get; set; }

        [HideInPropertyGrid]
        [EnableInBuilder(BuilderType.SurveyControl)]
        public EbQSec QSec { get; set; }

        [HideInPropertyGrid]
        [EnableInBuilder(BuilderType.SurveyControl)]
        public EbASec ASec { get; set; }
    }

    [HideInToolBox]
    [EnableInBuilder(BuilderType.SurveyControl)]
    public class EbQSec : EbControlContainer
    {
        [HideInPropertyGrid]
        [EnableInBuilder(BuilderType.SurveyControl)]
        public string Question { get; set; }

        [HideInPropertyGrid]
        [EnableInBuilder(BuilderType.SurveyControl)]
        public override List<EbControl> Controls { get; set; }
    }

    [HideInToolBox]
    [EnableInBuilder(BuilderType.SurveyControl)]
    public class EbASec : EbControlContainer
    {

        [HideInPropertyGrid]
        [EnableInBuilder(BuilderType.SurveyControl)]
        public override List<EbControl> Controls { get; set; }
    }
}
