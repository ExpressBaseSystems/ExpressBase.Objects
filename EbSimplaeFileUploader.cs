using ExpressBase.Common;
using ExpressBase.Common.Extensions;
using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using ExpressBase.Common.Structures;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace ExpressBase.Objects
{
	[EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl)]
	class EbSimplaeFileUploader: EbControlUI
	{

		public EbSimplaeFileUploader()
		{

		}
		[OnDeserialized]
		public void OnDeserializedMethod(StreamingContext context)
		{
			this.BareControlHtml = this.GetBareHtml();
			this.BareControlHtml4Bot = this.BareControlHtml;
			this.ObjType = this.GetType().Name.Substring(2, this.GetType().Name.Length - 2);
		}
		public override string ToolIconHtml { get { return "<i class='fa fa-cloud-upload'></i>"; } set { } }
		public override string ToolNameAlias { get { return "Simple FileUploader"; } set { } }
		public override string ToolHelpText { get { return "Simple FileUploader"; } set { } }

		public override string UIchangeFns
		{
			get
			{
				return @"EbSimplaeFileUploader = {
					minFilesFn : function(elementId, props) {
									if(props.MinFiles>props.MaxFiles){
										props.MinFiles=props.MaxFiles;
									}	
								}
				}";
			}
		}


		public override string GetBareHtml()
		{


			return @"<div class='simpleFUPdiv ' id='@ebsid@' name='@name@' style='border: 2px dashed ;border-radius: 4px;min-height:100px;'>
						<div id='@ebsid@_SFUP' class='SFUPcontainer' >						
							<div class='SFUPcontent'>
								<i class='fa fa-cloud-upload'></i><span>Drag &amp; Drop files here or click to browse</span>
							 </div>
						</div>
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

		public override string DesignHtml4Bot
		{
			get => this.GetBareHtml();
			set => base.DesignHtml4Bot = value;
		}



		//--------Hide in property grid------------
		[EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm)]
		[HideInPropertyGrid]
		public override string HelpText { get; set; }

		[EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm)]
		[HideInPropertyGrid]
		public override string ToolTipText { get; set; }

		[EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm)]
		[HideInPropertyGrid]
		public override bool Unique { get; set; }

		[EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm)]
		[HideInPropertyGrid]
		public override List<EbValidator> Validators { get; set; }

		[EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm)]
		[HideInPropertyGrid]
		public override EbScript DefaultValueExpression { get; set; }

		[EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm)]
		[HideInPropertyGrid]
		public override EbScript VisibleExpr { get; set; }

		[EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm)]
		[HideInPropertyGrid]
		public override EbScript ValueExpr { get; set; }

		[EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm)]
		[HideInPropertyGrid]
		public override bool IsDisable { get; set; }

		[EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm)]
		[HideInPropertyGrid]
		public override bool Required { get; set; }

		[EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm)]
		[HideInPropertyGrid]
		public override bool DoNotPersist { get; set; }

		[EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm)]
		[HideInPropertyGrid]
		public override string BackColor { get; set; }

		[EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm)]
		[HideInPropertyGrid]
		public override string ForeColor { get; set; }

		[EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm)]
		[HideInPropertyGrid]
		public override string LabelBackColor { get; set; }

		[EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm)]
		[HideInPropertyGrid]
		public override string LabelForeColor { get; set; }

		[EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm)]
		[HideInPropertyGrid]
		public override EbScript OnChangeFn { get; set; }

		[EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm)]
		[HideInPropertyGrid]
		public override EbDbTypes EbDbType { get { return EbDbTypes.String; } set { } }


		[EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm)]
		[DefaultPropValue("0")]
		[Alias("Maximum Files")]
		public int MaxFiles { get; set; }

		[EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm)]
		[OnChangeUIFunction("EbSimplaeFileUploader.minFilesFn")]
		[DefaultPropValue("0")]
		[Alias("Minimum Files")]
		public int MinFiles { get; set; }

		[EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm)]
		[DefaultPropValue("2")]
		[Alias("Maximum File Size(MB)")]
		public int MaxSize { get; set; }

		[EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm)]
		[DefaultPropValue(".jpg,.png")]
		[Alias(" File Types")]
		public string FileTypes { get; set; }
	}
}
