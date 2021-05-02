using ExpressBase.Common;
using ExpressBase.Common.Constants;
using ExpressBase.Common.Extensions;
using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using ExpressBase.Common.Structures;
using ExpressBase.Objects.ServiceStack_Artifacts;
using Newtonsoft.Json;
using ServiceStack;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace ExpressBase.Objects
{
    [EnableInBuilder(BuilderType.WebForm, BuilderType.UserControl, BuilderType.SurveyControl)]
    [SurveyBuilderRoles(SurveyRoles.AnswerControl)]
    public class EbRenderQuestionsControl : EbControlUI
    {
        public EbRenderQuestionsControl()
        {

        }


        [OnDeserialized]
        public void OnDeserializedMethod(StreamingContext context)
        {
            this.BareControlHtml = this.GetBareHtml();
            this.ObjType = this.GetType().Name.Substring(2, this.GetType().Name.Length - 2);
            if (this.ValueExpr == null)
                this.ValueExpr = new EbScript();
        }

        [HideInPropertyGrid]
        [EnableInBuilder(BuilderType.WebForm, BuilderType.UserControl)]
        public override bool DoNotPersist { get; set; }

        public string QuestionStr { get; set; }


        public override string GetDesignHtml()
        {
            string EbCtrlHTML = HtmlConstants.CONTROL_WRAPER_HTML4WEB

    .Replace("@LabelForeColor ", "color:" + ((this.LabelForeColor != null) ? this.LabelForeColor : "@LabelForeColor ") + ";")
    .Replace("@LabelBackColor ", "background-color:" + ((this.LabelBackColor != null) ? this.LabelBackColor : "@LabelBackColor ") + ";");

            return ReplacePropsInHTML(EbCtrlHTML, @" 
        <div class='qrender-wrap'>
            <i class='fa fa-list' aria-hidden='true'></i>
        </div>").RemoveCR().GraveAccentQuoted(); ;
        }

        public override string GetBareHtml()
        {
            return @" 
        <div class='qrender-wrap'>
            @options@
        </div>"
    .Replace("@options@", this.QuestionStr)
    .Replace("@name@", this.Name)
    .Replace("@ebsid@", this.EbSid);
        }


        public override string GetHtml()
        {
            string EbCtrlHTML = HtmlConstants.CONTROL_WRAPER_HTML4WEB

    .Replace("@LabelForeColor ", "color:" + ((this.LabelForeColor != null) ? this.LabelForeColor : "@LabelForeColor ") + ";")
    .Replace("@LabelBackColor ", "background-color:" + ((this.LabelBackColor != null) ? this.LabelBackColor : "@LabelBackColor ") + ";");
            return ReplacePropsInHTML(EbCtrlHTML);
        }


        public void InitFromDataBase(JsonServiceClient ServiceClient)
        {
            string _html = string.Empty;
            string OuterHtml = @"<div id='cont_@ebsid@' ebsid='@ebsid@' class='Eb-Question-Ctrl' Ctype=''>";
            //this.Options = new List<EbSimpleSelectOption>();

            GetRenderQuestionResponse result = ServiceClient.Get<GetRenderQuestionResponse>(new GetRenderQuestionsRequest { FormRefid = this.RefId, ControlId = this.ContextId });

            foreach (GetRenderQuestions option in result.GetRenderQuestions)
            {
                EbQuestion Resp = EbSerializers.Json_Deserialize<EbQuestion>(option.Questions.ToString());
                OuterHtml += Resp.GetHtml();
            }
            OuterHtml = OuterHtml.Replace("@body@", _html).Replace("@name@", this.Name).Replace("@ebsid@", this.EbSid_CtxId);
            this.QuestionStr = OuterHtml + "</div>";
        }

        [HideInPropertyGrid]
        [JsonIgnore]
        public override string ToolIconHtml { get { return "<i class='fa fa-list'></i>"; } set { } }



        [HideInPropertyGrid]
        [JsonIgnore]
        [EnableInBuilder(BuilderType.WebForm, BuilderType.UserControl)]
        public override string ToolNameAlias { get { return "Render Questions"; } set { } }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
        [PropertyEditor(PropertyEditorType.ObjectSelector)]
        [PropertyGroup(PGConstants.DATA)]
        [OSE_ObjectTypes(EbObjectTypes.iWebForm)]
        [Alias("Source Form")]
        public string FormRefId { get; set; }

        [PropertyGroup(PGConstants.EXTENDED)]
        [EnableInBuilder(BuilderType.WebForm)]
        [PropertyEditor(PropertyEditorType.ScriptEditorJS)]
        [HelpText("Define Question Render  Expression")]
        public EbScript QuestionRenderExpression { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
        [PropertyGroup(PGConstants.DATA)]
        public string SourceControlName { get; set; }


        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl)]
        [PropertyEditor(PropertyEditorType.ScriptEditorJS, PropertyEditorType.ScriptEditorSQ)]
        [Alias("Data Id Expression")]
        [PropertyGroup(PGConstants.VALUE)]
        [HelpText("Define how Data Id of this field should be calculated.")]
        public override EbScript ValueExpr { get; set; }



        //public void InitFromDataBase(JsonServiceClient ServiceClient)
        //{
        //    //RowColletion ds = (ServiceClient.Get<DataSourceDataResponse>(new DataSourceDataRequest { RefId = this.DataSourceId, Start = 0, Length = 1000 })).Data;

        //    //for (int i = 0; i < ds.Count; i++)
        //    //{
        //    //    EbCard Card = new EbCard() { EbSid = "cardEbsid_" + i };
        //    //    foreach (EbCardField Field in this.CardFields)
        //    //    {
        //    //        if (Field.DbFieldMap != null)
        //    //        {
        //    //            var tempdata = ds[i][Field.DbFieldMap.Data];
        //    //            if (Field is EbCardNumericField)
        //    //                Card.CustomFields[Field.Name] = Convert.ToDouble(tempdata);
        //    //            else
        //    //                Card.CustomFields[Field.Name] = tempdata.ToString().Trim();

        //    //            //for getting distinct filter values
        //    //            if (this.FilterField?.Name != null && Field.Name == this.FilterField.Name && !this.FilterValues.Contains(tempdata.ToString().Trim()))
        //    //            {
        //    //                this.FilterValues.Add(tempdata.ToString().Trim());
        //    //            }
        //    //        }
        //    //    }
        //    //    Card.CardId = Convert.ToInt32(ds[i][this.ValueMember.Data]);
        //    //    Card.Name = "CardIn" + this.Name;//------------------------"CardIn"		

        //    //    this.CardCollection.Add(Card);
        //    //}
        //}

    }
}
