using ExpressBase.Common;
using ExpressBase.Common.Extensions;
using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using ExpressBase.Common.Structures;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace ExpressBase.Objects.Objects
{
	[EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.UserControl)]
	public class EbRichText: EbControlUI
	{
		public EbRichText() { }

		[OnDeserialized]
		public void OnDeserializedMethod(StreamingContext context)
		{
			this.BareControlHtml = this.GetBareHtml();
			this.ObjType = this.GetType().Name.Substring(2, this.GetType().Name.Length - 2);
		}

		public override string ToolIconHtml { get { return "<i class='fa fa-align-center'></i><i class='fa fa-font'></i>"; } set { } }
		public override string ToolNameAlias { get { return "Rich Text"; } set { } }

		public override string ToolHelpText { get { return "Rich Text"; } set { } }

		public override string UIchangeFns
		{
			get
			{
				return @"EbTagInput = {
                
            }";
			}
		}


		//--------Hide in property grid------------
		[EnableInBuilder(BuilderType.WebForm)]
		[HideInPropertyGrid]
		public override string HelpText { get; set; }

		[EnableInBuilder(BuilderType.WebForm)]
		[HideInPropertyGrid]
		public override string ToolTipText { get; set; }

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
		public override EbScript VisibleExpr { get; set; }

		[EnableInBuilder(BuilderType.WebForm)]
		[HideInPropertyGrid]
		public override EbScript ValueExpr { get; set; }

		[EnableInBuilder(BuilderType.WebForm)]
		[HideInPropertyGrid]
		public override bool IsDisable { get; set; }

		[EnableInBuilder(BuilderType.WebForm)]
		[HideInPropertyGrid]
		public override bool Required { get; set; }

		[EnableInBuilder(BuilderType.WebForm)]
		[HideInPropertyGrid]
		public override bool DoNotPersist { get; set; }

		[EnableInBuilder(BuilderType.WebForm)]
		[HideInPropertyGrid]
		public override string BackColor { get; set; }

		[EnableInBuilder(BuilderType.WebForm)]
		[HideInPropertyGrid]
		public override string ForeColor { get; set; }

		[EnableInBuilder(BuilderType.WebForm)]
		[HideInPropertyGrid]
		public override string LabelBackColor { get; set; }

		[EnableInBuilder(BuilderType.WebForm)]
		[HideInPropertyGrid]
		public override string LabelForeColor { get; set; }

		[EnableInBuilder(BuilderType.WebForm)]
		[HideInPropertyGrid]
		public override EbScript OnChangeFn { get; set; }

		[EnableInBuilder(BuilderType.WebForm)]
		[HideInPropertyGrid]
		public override EbDbTypes EbDbType { get { return EbDbTypes.String; } set { } }



		[EnableInBuilder(BuilderType.WebForm)]
		[DefaultPropValue("100")]
		[Alias("Textarea Height")]
		public int TextBoxHeight { get; set; }


		public override string GetBareHtml()
		{


			return @" 
 <div id='@ebsid@_RichTextDiv'  >  
	<textarea    id='@ebsid@' style='width:100%; resize: none'  ></textarea >
</div>"
.Replace("@ebsid@", String.IsNullOrEmpty(this.EbSid_CtxId) ? "@ebsid@" : this.EbSid_CtxId)
.Replace("@name@", this.Name)
.Replace("@toolTipText@", this.ToolTipText)
.Replace("@value@", "");

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
		public override string GetValueFromDOMJSfn
		{
			get
			{
				return @" return $(`#${this.EbSid}`).summernote('code');";
			}
			set { }
		}

		public override string OnChangeBindJSFn
		{
			get
			{
				return @"$(`#${this.EbSid}`).on('summernote.blur', p1);";
			}
			set { }
		}

		public override string SetValueJSfn
		{
			get
			{
				return @" $(`#${this.EbSid}`).summernote('focus');
							$(`#${this.EbSid}`).summernote('code',p1);";
			}
			set { }
		}


		[JsonIgnore]
		public override string EnableJSfn { get { return @"this.__IsDisable = false; $(`#${this.EbSid}`).summernote('enable');"; } set { } }

		[JsonIgnore]
		public override string DisableJSfn { get { return @"this.__IsDisable = true; $(`#${this.EbSid}`).summernote('disable');"; } set { } }
		//public override string ClearJSfn
		//{
		//	get
		//	{
		//		return @"$('input[name = ' + this.EbSid_CtxId + '_tags]').va('');";
		//	}
		//	set { }
		//}

	}
}
