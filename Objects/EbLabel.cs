using ExpressBase.Common;
using ExpressBase.Common.Constants;
using ExpressBase.Common.Extensions;
using ExpressBase.Common.LocationNSolution;
using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using ExpressBase.Common.Structures;
using ExpressBase.Security;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ExpressBase.Objects
{
    public enum EbLabelRenderType
    {
        Default = 0,
        Link = 1
    }

    [EnableInBuilder(BuilderType.WebForm, BuilderType.UserControl)]
    public class EbLabel : EbControlUI
    {

        public EbLabel() { }

        [OnDeserialized]
        public void OnDeserializedMethod(StreamingContext context)
        {
            this.BareControlHtml = this.GetBareHtml();
            this.BareControlHtml4Bot = this.BareControlHtml;
            this.ObjType = this.GetType().Name.Substring(2, this.GetType().Name.Length - 2);
        }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.UserControl)]
        [HideInPropertyGrid]
        public override EbDbTypes EbDbType { get { return EbDbTypes.String; } }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.UserControl)]
        [HideInPropertyGrid]
        public override bool DoNotPersist { get { return true; } }

        [PropertyGroup(PGConstants.CORE)]
        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.UserControl)]
        [UIproperty]
        [Unique]
        [OnChangeUIFunction("Common.LABEL")]
        [PropertyEditor(PropertyEditorType.MultiLanguageKeySelector)]
        [HelpText("Label for the control to identify it's purpose.")]
        public override string Label { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.UserControl)]
        [PropertyEditor(PropertyEditorType.ScriptEditorJS, PropertyEditorType.ScriptEditorSQ)]
        [Alias("Text Expression")]
        [HelpText("Expression for label text.")]
        public override EbScript ValueExpr { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.UserControl)]
        [PropertyEditor(PropertyEditorType.ScriptEditorJS, PropertyEditorType.ScriptEditorSQ)]
        [Alias("Default Text Expression")]
        [HelpText("Default expression for label text.")]
        public override EbScript DefaultValueExpression { get; set; }

        [PropertyGroup(PGConstants.CORE)]
        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.UserControl)]
        public EbLabelRenderType RenderAs { get; set; }

        [PropertyGroup(PGConstants.CORE)]
        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.UserControl)]
        [PropertyEditor(PropertyEditorType.ObjectSelectorCollection)]
        [OSE_ObjectTypes(EbObjectTypes.iWebForm)]
        public List<ObjectBasicInfo> LinkedObjects { get; set; }

        [PropertyGroup(PGConstants.CORE)]
        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.UserControl)]
        [HelpText("Control name that contains version id of the intented link object.")]
        public string LinkVersionId { get; set; }

        [PropertyGroup(PGConstants.CORE)]
        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.UserControl)]
        [HelpText("Control name that contains data id of the intented submission.")]
        public string LinkDataId { get; set; }

        #region HideInPropertyGrid

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.UserControl)]
        [HideInPropertyGrid]
        public override string Info { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.UserControl)]
        [HideInPropertyGrid]
        public override string InfoIcon { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.UserControl)]
        [HideInPropertyGrid]
        public override string HelpText { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.UserControl)]
        [HideInPropertyGrid]
        public override string ToolTipText { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.UserControl)]
        [HideInPropertyGrid]
        public override bool Required { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.UserControl)]
        [HideInPropertyGrid]
        public override bool Unique { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.UserControl)]
        [HideInPropertyGrid]
        public override List<EbValidator> Validators { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.UserControl)]
        [HideInPropertyGrid]
        public override bool SelfTrigger { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.UserControl)]
        [HideInPropertyGrid]
        public override EbScript OnChangeFn { get; set; }

        #endregion

        public override string DesignHtml4Bot
        {
            get => @"
				<div id='cont_@ebsid@' ebsid='@ebsid@' name='@name@' ctype='@type@' eb-type='@type@' class='Eb-ctrlContainer iw-mTrigger' >
					<div class='msg-cont '>
						<div class='bot-icon'></div>
						<div class='msg-cont-bot'>
							<div class='msg-wraper-bot'>
							@Label@
							<div class='msg-time'>3:44pm</div>
							</div>
						</div>
					</div>
				</div>".Replace("@Label@", this.Label);
            set => base.DesignHtml4Bot = value;
        }
        public override string GetHtml4Bot()
        {
            return GetWrapedCtrlHtml4bot();
        }
        public override string GetWrapedCtrlHtml4bot()
        {
            string jk = @"
				<div id='cont_@ebsid@' ebsid='@ebsid@' name='@name@' ctype='@type@' eb-type='@type@' class='Eb-ctrlContainer iw-mTrigger' >
					<div class='msg-cont 66666666'>
						<div class='bot-icon'></div>
						<div class='msg-cont-bot'>
							<div class='msg-wraper-bot'>
							@Label@
							<div class='msg-time'>3:44pm</div>
							</div>
						</div>
					</div>
				</div>";
            return ReplacePropsInHTML(jk);

        }
        [HideInPropertyGrid]
        [JsonIgnore]
        public override string ToolIconHtml { get { return "<i class=\"fa fa-font\"></i>"; } set { } }

        public override string GetDesignHtml()
        {
            return new EbLabel() { EbSid = "Label1", Label = "Label1" }.GetHtml().RemoveCR().GraveAccentQuoted(); ;
        }

        public override string GetBareHtml()
        {
            return @"
        <div class='eb-ctrl-label' id='@name@Lbl' style='@LabelBackColor  @LabelForeColor '> @Label@ </div>
"
.Replace("@name@", this.Name)
.Replace("@Label@", this.Label);
        }

        public override string GetHtml()
        {
            string EbCtrlHTML = Common.HtmlConstants.CONTROL_WRAPER_HTML4WEB
               .Replace("@LabelForeColor ", "color:" + (LabelForeColor ?? "@LabelForeColor ") + ";")
               .Replace("@LabelBackColor ", "background-color:" + (LabelBackColor ?? "@LabelBackColor ") + ";");

            return ReplacePropsInHTML(EbCtrlHTML);
        }

        [JsonIgnore]
        public override string DisableJSfn { get { return string.Empty; } set { } }

        public override SingleColumn GetSingleColumn(User UserObj, Eb_Solution SoluObj, object Value, bool Default)
        {
            object _formattedData = Default ? this.Label : Value;
            string _displayMember = Value == null ? string.Empty : Value.ToString();

            return new SingleColumn()
            {
                Name = this.Name,
                Type = (int)this.EbDbType,
                Value = _formattedData,
                Control = this,
                ObjType = this.ObjType,
                F = _displayMember
            };
        }
    }
}
