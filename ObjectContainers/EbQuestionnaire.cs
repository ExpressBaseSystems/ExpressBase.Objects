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
        [EnableInBuilder(BuilderType.SurveyControl, BuilderType.WebForm, BuilderType.UserControl)]
        public int QId { get; set; }

        [HideInPropertyGrid]
        [EnableInBuilder(BuilderType.SurveyControl, BuilderType.WebForm, BuilderType.UserControl)]
        public override string EbSid { get; set; }

        [HideInPropertyGrid]
        [EnableInBuilder(BuilderType.SurveyControl, BuilderType.WebForm, BuilderType.UserControl)]
        public override string EbSid_CtxId { get; set; }

        [EnableInBuilder(BuilderType.SurveyControl, BuilderType.WebForm, BuilderType.UserControl)]
        public override string Name { get; set; }

        [HideInPropertyGrid]
        [EnableInBuilder(BuilderType.SurveyControl, BuilderType.WebForm, BuilderType.UserControl)]
        public EbQSec QSec { get; set; }

        [HideInPropertyGrid]
        [EnableInBuilder(BuilderType.SurveyControl, BuilderType.WebForm, BuilderType.UserControl)]
        public EbASec ASec { get; set; }

        public override string GetJsInitFunc()
        {
            return @"
this.Init = function(id){
    this.QSec = new EbObjects.EbQSec(id + 'QSec');
    this.ASec = new EbObjects.EbASec(id + 'ASec');

    let textObj = new EbObjects.EbTextControl(id + '_text');
    textObj.Label = '';
    this.QSec.Controls.$values.push(textObj);
};";
        }
        public override string GetHtml()
        {
            string _html = @"<div class='q-pane'> <div class='q-sec'>@Qsec@</div><div class='a-sec'> @Asec@</div> </div>";          
            return _html.Replace("@Qsec@", this.QSec.GetHtml()).Replace("@Asec@", this.ASec.GetHtml());
        }
    }

    [HideInToolBox]
    [EnableInBuilder(BuilderType.SurveyControl)]
    public class EbQSec : EbControlContainer
    {

        [HideInPropertyGrid]
        [EnableInBuilder(BuilderType.SurveyControl)]
        public override string EbSid { get; set; }

        [EnableInBuilder(BuilderType.SurveyControl)]
        public override string Name { get; set; }

        [HideInPropertyGrid]
        [EnableInBuilder(BuilderType.SurveyControl)]
        public override string EbSid_CtxId { get; set; }

        [HideInPropertyGrid]
        [EnableInBuilder(BuilderType.SurveyControl)]
        public string Question { get; set; }

        [HideInPropertyGrid]
        [EnableInBuilder(BuilderType.SurveyControl)]
        public override List<EbControl> Controls { get; set; }

        public override  string GetHtml()
        {
            string _html = string.Empty;
            foreach (EbControl EC in this.Controls)
            {
                _html += EC.GetHtml();
            }
            return _html;
        }
    }

    [HideInToolBox]
    [EnableInBuilder(BuilderType.SurveyControl)]
    public class EbASec : EbControlContainer
    {

        [HideInPropertyGrid]
        [EnableInBuilder(BuilderType.SurveyControl)]
        public override string EbSid { get; set; }

        [EnableInBuilder(BuilderType.SurveyControl)]
        public override string Name { get; set; }

        [HideInPropertyGrid]
        [EnableInBuilder(BuilderType.SurveyControl)]
        public override string EbSid_CtxId { get; set; }

        [HideInPropertyGrid]
        [EnableInBuilder(BuilderType.SurveyControl)]
        public override List<EbControl> Controls { get; set; }
        public override string GetHtml()
        {
            string _html = string.Empty;
            foreach (EbControl AsecControl in this.Controls)
            {
                _html += AsecControl.GetHtml();
            }
            return _html;
        }
    }
}
