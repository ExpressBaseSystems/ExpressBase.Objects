using ExpressBase.Common;
using ExpressBase.Common.Constants;
using ExpressBase.Common.Extensions;
using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ExpressBase.Objects
{
    [EnableInBuilder(BuilderType.SurveyControl, BuilderType.WebForm)]
    [SurveyBuilderRoles(SurveyRoles.QuestionControl, SurveyRoles.AnswerControl)]
    public class EbTextControl : EbLabel
    {
        [HideInPropertyGrid]
        [JsonIgnore]
        [EnableInBuilder(BuilderType.SurveyControl)]
        public override string ToolIconHtml { get { return "<i class=\"fa fa-font\"></i>"; } set { } }

        [PropertyGroup(PGConstants.CORE)]
        [EnableInBuilder(BuilderType.SurveyControl)]
        [UIproperty]
        [Unique]
        [OnChangeUIFunction("Common.LABEL")]
        [PropertyEditor(PropertyEditorType.MultiLanguageKeySelector)]
        [HelpText("Label for the control to identify it's purpose.")]
        [Alias("Text")]
        public override string Label { get; set; }

        [HideInPropertyGrid]
        [JsonIgnore]
        [EnableInBuilder(BuilderType.SurveyControl)]
        public override string ToolNameAlias { get { return "Text"; } set { } }

        public override string GetDesignHtml()
        {
            return new EbLabel() { EbSid = "Text1", Label = "Text1" }.GetHtml().RemoveCR().GraveAccentQuoted(); ;
        }
    }

}
