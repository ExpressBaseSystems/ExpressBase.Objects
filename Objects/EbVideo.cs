using ExpressBase.Common;
using ExpressBase.Common.Constants;
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
	[EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
	public class EbVideo: EbControlUI
	{
		public EbVideo()
		{

		}
		[OnDeserialized]
		public void OnDeserializedMethod(StreamingContext context)
		{
			this.BareControlHtml = this.GetBareHtml();
			this.BareControlHtml4Bot = this.BareControlHtml;
			this.ObjType = this.GetType().Name.Substring(2, this.GetType().Name.Length - 2);
		}

		public override string ToolIconHtml { get { return "<i class='fa fa-video-camera'></i>"; } set { } }

		public override string ToolNameAlias { get { return "Video"; } set { } }

		public override string ToolHelpText { get { return "Video"; } set { } }

		//--------Hide in property grid------------
		[EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
		[HideInPropertyGrid]
		public override string HelpText { get; set; }

		[EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
		[HideInPropertyGrid]
		public override string ToolTipText { get; set; }

		[EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
		[HideInPropertyGrid]
		public override bool Unique { get; set; }

		[EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
		[HideInPropertyGrid]
		public override List<EbValidator> Validators { get; set; }

		[EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
		[HideInPropertyGrid]
		public override EbScript DefaultValueExpression { get; set; }

		[EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
		[HideInPropertyGrid]
		public override EbScript HiddenExpr { get; set; }
		
		[EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
		[HideInPropertyGrid]
		public override EbScript DisableExpr { get; set; }

		[EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
		[HideInPropertyGrid]
		public override EbScript ValueExpr { get; set; }

		public override bool SelfTrigger { get; set; }

		[EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
		[HideInPropertyGrid]
		public override bool Required { get; set; }

		[EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
		[HideInPropertyGrid]
		public override bool DoNotPersist { get; set; }

		[EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
		[HideInPropertyGrid]
		public override string BackColor { get; set; }

		[EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
		[HideInPropertyGrid]
		public override string ForeColor { get; set; }

		[EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
		[HideInPropertyGrid]
		public override string LabelBackColor { get; set; }

		[EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
		[HideInPropertyGrid]
		public override string LabelForeColor { get; set; }

		[EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
		[HideInPropertyGrid]
		public override EbScript OnChangeFn { get; set; }

		//[EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm)]
		//[HideInPropertyGrid]
		//public override EbDbTypes EbDbType { get { return EbDbTypes.String; } set { } }

		[EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
		[HideInPropertyGrid]
		public override bool IsFullViewContol { get => true; set => base.IsFullViewContol = value; }

		//[HideInPropertyGrid]
		//[EnableInBuilder(BuilderType.BotForm)]
		//public override bool IsReadOnly { get => true; }

		[HideInPropertyGrid]
		[EnableInBuilder(BuilderType.BotForm)]
		public override bool IsDisable { get => true; }

		[EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
		public override bool IsNonDataInputControl { get => true; }

		[EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
		[PropertyGroup(PGConstants.APPEARANCE)]
		[DefaultPropValue("150")]
		public int MaxHeight { get; set; }

		[EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
		[PropertyGroup(PGConstants.APPEARANCE)]
		[DefaultPropValue("300")]
		public int MaxWidth { get; set; }

		[EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm)]
		[Alias("Video Url")]
		public string VideoUrl { get; set; }

		public override string GetBareHtml()
		{
			return @"<div class=' @ebsid@_cont'>
						  <iframe id='@ebsid@' name='@name@'  width='@framewidth@' height='@frameheight@' controls src='@VideoUrl@'>
							</iframe>
					</div>"
.Replace("@ebsid@", String.IsNullOrEmpty(this.EbSid_CtxId) ? "@ebsid@" : this.EbSid_CtxId)
.Replace("@name@", this.Name)
.Replace("@toolTipText@", this.ToolTipText)
.Replace("@value@", "")
.Replace("@VideoUrl@", this.VideoUrl)
.Replace("@framewidth@", this.MaxWidth.ToString())
.Replace("@frameheight@", this.MaxHeight.ToString());

		}

		public override string GetDesignHtml()
		{
			string EbCtrlHTML = HtmlConstants.CONTROL_WRAPER_HTML4WEB
			  .Replace("@barehtml@", @"                       
                        <div class='videoframecont' style='width:100%; '> 
                             <img id='@ebsid@' name='@name@' src='/images/video_thump.jpg' style='max-width:200px; max-height:150px;'>
                        </div>
                ").RemoveCR().DoubleQuoted();
			
			return ReplacePropsInHTML(EbCtrlHTML); 
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
	}
}
