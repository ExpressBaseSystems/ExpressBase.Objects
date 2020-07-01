using ExpressBase.Common;
using ExpressBase.Common.Constants;
using ExpressBase.Common.Extensions;
using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace ExpressBase.Objects.Objects
{
	[EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog,  BuilderType.UserControl)]
	public class EbScriptButton: EbControlUI
	{
		public EbScriptButton() { }

		[OnDeserialized]
		public void OnDeserializedMethod(StreamingContext context)
		{
			this.BareControlHtml = this.GetBareHtml();
			this.ObjType = this.GetType().Name.Substring(2, this.GetType().Name.Length - 2);
		}

		public override string ToolIconHtml { get { return "<i class='fa fa-square-o'></i>"; } set { } }

		public override string ToolNameAlias { get { return "Script Button"; } set { } }

		public override string ToolHelpText { get { return "Script Button"; } set { } }


		public override string GetBareHtml()
		{


			return @"<button id='@ebsid@' class='btn btn-default' style='width:100%; cursor: pointer; @backColor @foreColor @fontStyle'>@Label@</button>"
				.Replace("@ebsid@", this.EbSid_CtxId)
				.Replace("@Label@", this.Label ?? "Run")
.Replace("@tabIndex", "tabindex='" + this.TabIndex + "'")
.Replace("@backColor", "background-color:" + this.BackColor + ";")
.Replace("@foreColor", "color:" + this.ForeColor + ";");

		}

		public override string GetDesignHtml()
		{
			return GetHtml().RemoveCR().DoubleQuoted();
		}

		public override string GetHtml()
		{
			string EbCtrlHTML = HtmlConstants.CONTROL_WRAPER_HTML4WEB
			   .Replace("@LabelForeColor ", "color:" + (LabelForeColor ?? "@LabelForeColor ") + ";")
			   .Replace("@LabelBackColor ", "background-color:" + (LabelBackColor ?? "@LabelBackColor ") + ";");

			return ReplacePropsInHTML(EbCtrlHTML);
		}



		//--------Hide in property grid------------

		[EnableInBuilder(BuilderType.WebForm)]
		[HideInPropertyGrid]
		public override bool Unique { get; set; }

		[EnableInBuilder(BuilderType.WebForm)]
		[HideInPropertyGrid]
		public override List<EbValidator> Validators { get; set; }

		[EnableInBuilder(BuilderType.WebForm)]
		[HideInPropertyGrid]
		public override EbScript DefaultValueExpression { get; set; }

		[EnableInBuilder(BuilderType.WebForm)]
		[HideInPropertyGrid]
		public override EbScript HideExpr { get; set; }

		[EnableInBuilder(BuilderType.WebForm)]
		[HideInPropertyGrid]
		public override EbScript ValueExpr { get; set; }

		[EnableInBuilder(BuilderType.WebForm)]
		[HideInPropertyGrid]
		public override bool DoNotPersist { get; set; }

		[EnableInBuilder(BuilderType.WebForm)]
		[HideInPropertyGrid]
		public override bool Required { get; set; }

		[EnableInBuilder(BuilderType.WebForm)]
		[HideInPropertyGrid]
		public override string LabelBackColor { get; set; }

		[EnableInBuilder(BuilderType.WebForm)]
		[HideInPropertyGrid]
		public override string LabelForeColor { get; set; }

		[EnableInBuilder(BuilderType.WebForm)]
		[PropertyGroup(PGConstants.EVENTS)]
		[Alias("OnClick")]
		public override EbScript OnChangeFn { get; set; }



		[EnableInBuilder(BuilderType.WebForm)]
		public override string BackColor { get; set; }

		[EnableInBuilder(BuilderType.WebForm)]
		public override string ForeColor { get; set; }

		[EnableInBuilder(BuilderType.WebForm)]
		public override bool IsDisable { get; set; }
		
		[EnableInBuilder(BuilderType.WebForm)]
		public override string HelpText { get; set; }

		//[EnableInBuilder(BuilderType.WebForm)]
		//public override string ToolTipText { get; set; }

		//[EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl)]
		//[PropertyGroup(PGConstants.EVENTS)]
		//[PropertyEditor(PropertyEditorType.ScriptEditorJS)]
		//[HelpText("Define onClick function.")]
		//public EbScript OnClickFn { get; set; }


		[JsonIgnore]
		public override string OnChangeBindJSFn
		{
			get
			{
				return @"$('#' + this.EbSid_CtxId).on('click', p1);";
			}
			set { }
		}
	}
}
