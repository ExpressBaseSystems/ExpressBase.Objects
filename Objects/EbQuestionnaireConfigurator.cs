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
	[EnableInBuilder(BuilderType.WebForm, BuilderType.UserControl)]

	public class EbQuestionnaireConfigurator : EbControlUI
	{
		public EbQuestionnaireConfigurator()
		{
		}
		[OnDeserialized]
		public void OnDeserializedMethod(StreamingContext context)
		{
			this.BareControlHtml = this.GetBareHtml();
			this.ObjType = this.GetType().Name.Substring(2, this.GetType().Name.Length - 2);
		}
		public override string ToolIconHtml { get { return "<i class='fa fa-question-circle  '></i>"; } set { } }

		public override string ToolNameAlias { get { return "Questionnaire Configurator"; } set { } }

		public override string ToolHelpText { get { return "Questionnaire Configurator"; } set { } }
		public override string UIchangeFns
		{
			get
			{
				return @"EbQuestionnaireConfigurator = {
                
            }";
			}
		}
		//--------Hide in property grid------------
		[EnableInBuilder(BuilderType.WebForm, BuilderType.UserControl)]
		[HideInPropertyGrid]
		public override string HelpText { get; set; }

		[EnableInBuilder(BuilderType.WebForm, BuilderType.UserControl)]
		[HideInPropertyGrid]
		public override string ToolTipText { get; set; }

		[EnableInBuilder(BuilderType.WebForm, BuilderType.UserControl)]
		public override bool Unique { get; set; }

		[EnableInBuilder(BuilderType.WebForm, BuilderType.UserControl)]
		[HideInPropertyGrid]
		public override bool DoNotPersist { get; set; }

		[EnableInBuilder(BuilderType.WebForm, BuilderType.UserControl)]
		[HideInPropertyGrid]
		public override string BackColor { get; set; }

		[EnableInBuilder(BuilderType.WebForm, BuilderType.UserControl)]
		[HideInPropertyGrid]
		public override string ForeColor { get; set; }

		[EnableInBuilder(BuilderType.WebForm, BuilderType.UserControl)]
		[HideInPropertyGrid]
		public override string LabelBackColor { get; set; }

		[EnableInBuilder(BuilderType.WebForm, BuilderType.UserControl)]
		[HideInPropertyGrid]
		public override string LabelForeColor { get; set; }

		[EnableInBuilder(BuilderType.WebForm, BuilderType.UserControl)]
		[HideInPropertyGrid]
		public override EbDbTypes EbDbType { get { return EbDbTypes.String; } set { } }

		[EnableInBuilder(BuilderType.WebForm, BuilderType.UserControl)]
		[DefaultPropValue("100")]
		[PropertyGroup(PGConstants.APPEARANCE)]
		[Alias("DropdownMaxHeight")]
		public int DropdownHeight { get; set; }


		[EnableInBuilder(BuilderType.WebForm)]
		[HideInPropertyGrid]
		//[JsonIgnore]
		public string OptionHtml { get; set; }
		public override string GetBareHtml()
		{
			return @"<div class='input-group @ebsid@_cont'>
				<select id='@ebsid@' ui-inp class='' multiple @selOpts@ @MaxLimit@  @IsSearchable@ name='@ebsid@'   style='width: 100%;'>
					@options@
				</select>
			</div>"
			.Replace("@ebsid@", String.IsNullOrEmpty(this.EbSid_CtxId) ? "@ebsid@" : this.EbSid_CtxId)
			.Replace("@name@", this.Name)
			.Replace("@HelpText@", this.HelpText)
			.Replace("@toolTipText@", this.ToolTipText)
			.Replace("@options@", this.OptionHtml);

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

		public void InitFromDataBase(JsonServiceClient ServiceClient)
		{
			////query is directly written- table, column namesa are given explicitly in query (only for postgre)
			var result = ServiceClient.Get<GetQuestionsBankResponse>(new GetQuestionsBankRequest { });
			if (result.Questionlst.Count > 0)
			{
				this.OptionHtml = string.Empty;
				foreach (var dct in result.Questionlst)
				{
					EbQuestion qstn = EbSerializers.Json_Deserialize<EbQuestion>(dct.Value);
					this.OptionHtml += $"<option  value='{dct.Key}'>{qstn.Name}</option>";
				}
			}



		}



	}
}
