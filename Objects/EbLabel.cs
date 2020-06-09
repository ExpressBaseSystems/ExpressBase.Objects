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

    [EnableInBuilder( BuilderType.WebForm, BuilderType.UserControl)]
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

		[HideInPropertyGrid]
		[EnableInBuilder(BuilderType.BotForm)]
		public override bool IsDisable { get => true; }

		[EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl)]
		public override bool IsNonDataInputControl { get => true; }

		[PropertyGroup(PGConstants.CORE)]
        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl)]
        [UIproperty]
        [Unique]
        [OnChangeUIFunction("Common.LABEL")]
        [PropertyEditor(PropertyEditorType.MultiLanguageKeySelector)]
        [HelpText("Label for the control to identify it's purpose.")]
        [Alias("Text")]
        public override string Label { get; set; }

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
        public override string ToolIconHtml { get { return "<i class='fa fa-font'></i>"; } set { } }

        public override string GetDesignHtml() {
            return new EbLabel() { EbSid= "Label1", Label = "Label1" }.GetHtml().RemoveCR().GraveAccentQuoted(); ;
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
    }
}
