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
	[EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl)]
	public class EbRating : EbControlUI
	{
		public EbRating()
		{

		}
		[OnDeserialized]
		public void OnDeserializedMethod(StreamingContext context)
		{
			this.BareControlHtml = this.GetBareHtml();
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
                    console.log('ebrating');
				var rtngHtml='';
					for(var i=props.MaxVal; i>0; i--){
						rtngHtml += `<span class='fa fa-star-o wrd_spacing'></span>`;
					}
	
                  let rtg =  $(`[ebsid = ${elementId}]`).find('.ratingDiv').empty().append(rtngHtml);
                }
            }";
			}
		}

		public override string GetBareHtml()
		{


			var htmlstring = "";
			for (var i = this.MaxVal; i> 0; i--)
			{
				htmlstring += @"<input type='radio' name='@ebsid@' value='@valuei@' id='@ebsid@ratingID@valuei@' class='rtngStarInpt'> 
								<label for='@ebsid@ratingID@valuei@' class=''>
								<i class='fa fa-star'></i>
								</label>".Replace("@valuei@",i.ToString());
			}
			return @"<div>
					<div class='rating-container ' id='@ebsid@_ratingDiv'>
						@rtngstarHtml@

					</div>
				</div>"
					.Replace("@rtngstarHtml@", htmlstring)
					.Replace("@ebsid@", String.IsNullOrEmpty(this.EbSid_CtxId) ? "@ebsid@" : this.EbSid_CtxId)
					.Replace("@name@", this.Name);
		}

		public override string GetDesignHtml()
		{
			string ratingHtml = @"
					<div class='ratingDiv' id='@ebsid@_ratingDiv' style='width:100%;'>
						<span class='fa fa-star-o wrd_spacing'></span><span class='fa fa-star-o wrd_spacing'></span><span class='fa fa-star-o wrd_spacing'></span><span class='fa fa-star-o wrd_spacing'></span><span class='fa fa-star-o wrd_spacing'></span>
					</div>";

			 string _html= HtmlConstants.CONTROL_WRAPER_HTML4WEB
			   .Replace("@LabelForeColor ", "color:" + (LabelForeColor ?? "@LabelForeColor ") + ";")
			   .Replace("@LabelBackColor ", "background-color:" + (LabelBackColor ?? "@LabelBackColor ") + ";")
			   .Replace("@barehtml@", ratingHtml)
					.RemoveCR().DoubleQuoted();
			return ReplacePropsInHTML(_html);

			//.Replace("@ebsid@", String.IsNullOrEmpty(this.EbSid_CtxId) ? "@ebsid@" : this.EbSid_CtxId)
			//.Replace("@name@", this.Name)
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
				return @" return this.RatingCount = $('[name=' + this.EbSid_CtxId + ']:checked').val();";
			}
			set { }
		}

		public override string OnChangeBindJSFn
		{
			get
			{
				return @"$('input[name = ' + this.EbSid_CtxId + ']').on('change', p1);";
			}
			set { }
		}
		public override string JustSetValueJSfn
		{
			get
			{
				return @" $('input[name = ' + this.EbSid_CtxId + '][value = ' + p1 + ']').prop('checked', true)";
			}
			set { }
		}

		public override string SetValueJSfn
		{
			get
			{
				return JustSetValueJSfn + @".trigger('change');";
			}
			set { }
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
		public override EbDbTypes EbDbType { get { return EbDbTypes.Decimal; } set { } }


		
		[EnableInBuilder(BuilderType.WebForm)]
		[HideInPropertyGrid]
		public int RatingCount { get; set; }

		[EnableInBuilder(BuilderType.WebForm)]
		[OnChangeUIFunction("EbRating.starCount")]
		[DefaultPropValue("5")]
		[Alias("Maximum Star")]
		public int MaxVal { get; set; }

		[EnableInBuilder(BuilderType.WebForm)]
		[HideInPropertyGrid]
		[PropertyEditor(PropertyEditorType.Color)]
		public string IconColor { get; set; }

		[EnableInBuilder(BuilderType.WebForm)]
		[PropertyEditor(PropertyEditorType.Color)]
		[Alias("Remove border")]
		public bool RemoveBorder { get; set; }

		[EnableInBuilder(BuilderType.WebForm)]
		[HideInPropertyGrid]
		[Alias("Icon size")]
		public int IconSize  { get; set; }




	}
}
