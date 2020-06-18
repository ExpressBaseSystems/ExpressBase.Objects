using ExpressBase.Common;
using ExpressBase.Common.Extensions;
using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using ExpressBase.Common.Structures;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace ExpressBase.Objects.Objects
{
	[EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
	public class EbRating : EbControlUI
	{
		public EbRating()
		{

		}
		[OnDeserialized]
		public void OnDeserializedMethod(StreamingContext context)
		{
			this.BareControlHtml = this.GetBareHtml();
			this.BareControlHtml4Bot = this.BareControlHtml;
			this.ObjType = this.GetType().Name.Substring(2, this.GetType().Name.Length - 2);
		}

		public override string ToolIconHtml { get { return "<i class='fa fa-star-o'></i>"; } set { } }

		public override string ToolNameAlias { get { return "Rating"; } set { } }

		public override string ToolHelpText { get { return "Rating"; } set { } }

		public override string UIchangeFns
		{
			get
			{
				return @"EbRating = {
                starCount : function(elementId, props) {
								let rtngHtml='';
									for(let i=props.MaxVal; i>0; i--){
										rtngHtml += `<span class='fa fa-star-o wrd_spacing'></span>`;
									}
								let rtg =  $(`[ebsid = ${elementId}]`).find('.ratingDiv_dc').empty().append(rtngHtml);
							}
            }";
			}
		}

		public override string GetBareHtml()
		{

			var htmlstring = "";
			for (var i = this.MaxVal; i > 0; i--)
			{
				htmlstring += @"<span class='fa fa-star-o wrd_spacing'>";
			}
			return @"<div class='rating-container ' id='@ebsid@' name='@name@'>
						<div class='ratingDiv_dc'>
							@rtngstarHtml@

						</div>
					</div>"
					.Replace("@rtngstarHtml@", htmlstring)
					.Replace("@ebsid@", String.IsNullOrEmpty(this.EbSid_CtxId) ? "@ebsid@" : this.EbSid_CtxId)
					.Replace("@name@", this.Name);
		}

		public override string GetDesignHtml()
		{
			string ratingHtml = DesignHtml4Bot;

			string _html = HtmlConstants.CONTROL_WRAPER_HTML4WEB
			  .Replace("@LabelForeColor ", "color:" + (LabelForeColor ?? "@LabelForeColor ") + ";")
			  .Replace("@LabelBackColor ", "background-color:" + (LabelBackColor ?? "@LabelBackColor ") + ";")
			  .Replace("@barehtml@", ratingHtml)
				   .RemoveCR().DoubleQuoted();
			return ReplacePropsInHTML(_html);

			//.Replace("@ebsid@", String.IsNullOrEmpty(this.EbSid_CtxId) ? "@ebsid@" : this.EbSid_CtxId)
			//.Replace("@name@", this.Name)
		}
		public override string DesignHtml4Bot
		{
			get => @"
<div class='ratingDiv_dc' id='@ebsid@' style='width:100%;'>
	<span class='fa fa-star-o wrd_spacing'></span>
	<span class='fa fa-star-o wrd_spacing'></span>
	<span class='fa fa-star-o wrd_spacing'></span>
	<span class='fa fa-star-o wrd_spacing'></span>
	<span class='fa fa-star-o wrd_spacing'></span>
</div>";
			set => base.DesignHtml4Bot = value;
		}

		public override string GetHtml()
		{
			string EbCtrlHTML = HtmlConstants.CONTROL_WRAPER_HTML4WEB
			   .Replace("@LabelForeColor ", "color:" + (LabelForeColor ?? "@LabelForeColor ") + ";")
			   .Replace("@LabelBackColor ", "background-color:" + (LabelBackColor ?? "@LabelBackColor ") + ";");

			return ReplacePropsInHTML(EbCtrlHTML);
		}
		public override string GetHtml4Bot()
		{
			return ReplacePropsInHTML(HtmlConstants.CONTROL_WRAPER_HTML4BOT);
		}

		public override string GetValueFromDOMJSfn
		{
			get
			{
				return @" return $('#' + this.EbSid).rateYo('rating');";
			}
			set { }
		}

		public override string OnChangeBindJSFn
		{
			get
			{
				return @"$('#' + this.EbSid).rateYo().on('rateyo.set', p1);";
			}
			set { }
		}

		public override string SetValueJSfn
		{
			get
			{
				return @" $('#' + this.EbSid).rateYo('rating', p1);";
			}
			set { }
		}
		public override string ClearJSfn
		{
			get
			{
				return @" $('#' + this.EbSid).rateYo('rating', 0);";
			}
			set { }
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
		public override EbDbTypes EbDbType { get { return EbDbTypes.Decimal; } set { } }



		[EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm)]
		[HideInPropertyGrid]
		public int RatingCount { get; set; }

		[EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm)]
		[OnChangeUIFunction("EbRating.starCount")]
		[DefaultPropValue("5")]
		[Alias("Maximum Star")]
		public int MaxVal { get; set; }

		[EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm)]
		[PropertyEditor(PropertyEditorType.Color)]
		[Alias("Rating Color")]
		[DefaultPropValue("#F39C12")]
		public string RatingColor { get; set; }

		[EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm)]
		[Alias("Remove border")]
		public bool RemoveBorder { get; set; }

		//[EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm)]
		//[DefaultPropValue("true")]
		//[OnChangeUIFunction("EbRating.fullStarFn")]
		//[Alias("Full Star")]
		//public bool FullStar { get; set; }

		[EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm)]
		[DefaultPropValue("false")]
		[Alias("Half Star")]
		public bool HalfStar { get; set; }

		[EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm)]
		[DefaultPropValue("24")]
		[Alias("Star Size")]
		public int StarWidth { get; set; }

		[EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm)]
		[DefaultPropValue("8")]
		[Alias("Spacing")]
		public int Spacing { get; set; }



	}
}
