using ExpressBase.Common.Constants;
using ExpressBase.Common.Extensions;
using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace ExpressBase.Objects.Objects
{
    [EnableInBuilder(BuilderType.WebForm, BuilderType.UserControl)]
    public class EbSubmitButton : EbControlUI
    {
        public EbSubmitButton() { }

        public override string UIchangeFns
        {
            get
            {
                return @"EbSubmitButton = {
                ChangeBg : function(elementId, props) {
                console.log('haiii');
                $('#webform_submit').css('background-color' , props.BackColor);
                },
                ChangeTextColor : function(elementId, props) {
                $('#webform_submit').css('color' , props.ForeColor);
                },
                }";
            }
        }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
        [PropertyGroup(PGConstants.APPEARANCE)]
        [PropertyEditor(PropertyEditorType.Color)]
        [UIproperty]
        [OnChangeUIFunction("EbSubmitButton.ChangeBg")]
        public override string BackColor { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
        [PropertyGroup(PGConstants.APPEARANCE)]
        [PropertyEditor(PropertyEditorType.Color)]
        [UIproperty]
        [OnChangeUIFunction("EbSubmitButton.ChangeTextColor")]
        [DefaultPropValue("#333333")]
        [Alias("Text Color")]
        public override string ForeColor { get; set; }

        [OnDeserialized]
        public void OnDeserializedMethod(StreamingContext context)
        {
            this.BareControlHtml = this.GetBareHtml();
            this.ObjType = this.GetType().Name.Substring(2, this.GetType().Name.Length - 2);
        }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
        [Alias("Text")]
        public override string Label { get; set; }

        [HideInPropertyGrid]
        [EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
        public override bool DoNotPersist { get { return true; } }

        public override string ToolIconHtml { get { return "<i class='fa fa-check-circle-o'></i>"; } set { } }

        public override string ToolNameAlias { get { return "Submit Button"; } set { } }

        [HideInPropertyGrid]
        public override EbScript DefaultValueExpression { get => base.DefaultValueExpression; set => base.DefaultValueExpression = value; }

        [HideInPropertyGrid]
        public override EbScript ValueExpr { get; set; }

        [HideInPropertyGrid]
        public override string ToolTipText { get; set; }

        [HideInPropertyGrid]
        public override EbScript OnChangeFn { get; set; }

        [HideInPropertyGrid]
        public override string LabelForeColor { get; set; }

        [HideInPropertyGrid]
        public override string LabelBackColor { get; set; }

        [HideInPropertyGrid]
        public override bool IsDisable { get; set; }

        [HideInPropertyGrid]
        public override EbScript VisibleExpr { get; set; }

        [HideInPropertyGrid]
        public override List<EbValidator> Validators { get; set; }

        [HideInPropertyGrid]
        public override bool Unique { get; set; }

        [HideInPropertyGrid]
        public override bool Hidden { get; set; }

        [HideInPropertyGrid]
        public override bool Required { get; set; }

        [HideInPropertyGrid]
        public override string HelpText { get; set; }

        public override string GetDesignHtml()
        {
            return GetHtmlHelper(RenderMode.Developer).RemoveCR().DoubleQuoted();
        }

        public override string GetHtml()
        {
            return GetHtmlHelper(RenderMode.User);
        }

        public override string GetBareHtml()
        {
            return @"<button id='webform_submit' class='btn btn-default' style='width:100%; @backColor @foreColor @fontStyle' disabled >@Label@</button>"
                .Replace("@ebsid@", this.EbSid_CtxId)
                .Replace("@Label@", this.Label ?? "@Text@")
.Replace("@tabIndex", "tabindex='" + this.TabIndex + "'")
.Replace("@backColor", "background-color:" + this.BackColor + ";")
.Replace("@foreColor", "color:" + this.ForeColor + ";")
//.Replace("@fontStyle", (this.FontSerialized != null) ?
//                            (" font-family:" + this.FontSerialized.FontFamily + ";" + "font-style:" + this.FontSerialized.Style
//                            + ";" + "font-size:" + this.FontSerialized.SizeInPoints + "px;")
//                        : string.Empty)
;
        }

        private string GetHtmlHelper(RenderMode mode)
        {
            string EbCtrlHTML = @"
            <div id='cont_@ebsid@' ebsid='@ebsid@' name='@name@' class='Eb-ctrlContainer' @childOf@ ctype='@type@' eb-hidden='@isHidden@'>
                <div  id='@ebsid@Wraper' class='ctrl-cover'>
                    @barehtml@
                </div>
                <span class='helpText' ui-helptxt >@helpText@ </span>
            </div>";
            return ReplacePropsInHTML(EbCtrlHTML);
        }
    }
}
