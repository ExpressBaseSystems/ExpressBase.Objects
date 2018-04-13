using ExpressBase.Common;
using ExpressBase.Common.Extensions;
using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using ExpressBase.Common.Structures;
using ExpressBase.Objects.ServiceStack_Artifacts;
using ServiceStack;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;

namespace ExpressBase.Objects
{
	[EnableInBuilder(BuilderType.BotForm)]
	public class EbCards : EbControl
	{
		[EnableInBuilder(BuilderType.BotForm)]
		[PropertyEditor(PropertyEditorType.Collection)]
		public List<EbCard> CardCollection { get; set; }

		[EnableInBuilder(BuilderType.BotForm)]
		[OSE_ObjectTypes(EbObjectTypes.iDataSource)]
		[PropertyEditor(PropertyEditorType.ObjectSelector)]
		public string DataSourceId { get; set; }

		[EnableInBuilder(BuilderType.BotForm)]
		[HideInPropertyGrid]
		public ColumnColletion Columns { get; set; }

		[EnableInBuilder(BuilderType.BotForm)]
		[PropertyEditor(PropertyEditorType.Boolean)]
		[OnChangeExec(@"if(this.MultiSelect === true){pg.ShowProperty('SummaryTitle');}
		else{pg.HideProperty('SummaryTitle');}")]
		public bool MultiSelect { get; set; }

		public List<int> SelectedCards { get; set; }

		[EnableInBuilder(BuilderType.BotForm)]
		public string SummaryTitle { get; set; }

		[EnableInBuilder(BuilderType.BotForm)]
		[PropertyEditor(PropertyEditorType.CollectionFrmSrc, "Columns", 1)]
		[OnChangeExec(@"console.log(100); if (this.Columns.$values.length === 0 ){pg.MakeReadOnly('ValueMember');} else {pg.MakeReadWrite('ValueMember');}")]
		public EbDataColumn ValueMember { get; set; }

		//[EnableInBuilder(BuilderType.BotForm)]
		//[PropertyEditor(PropertyEditorType.Collection)]
		public List<EbButton> Buttons { get; set; }

		[EnableInBuilder(BuilderType.BotForm)]
		[PropertyEditor(PropertyEditorType.Collection)]
		public List<EbCardField> CardFields { get; set; }

		public bool IsSummaryRequired { get; set; }

        public override bool isFullViewContol { get => true; set => base.isFullViewContol = value; }

        //public List<EbCardField> SummarizeFields { get; set; }

        public EbCards()
		{
			this.CardCollection = new List<EbCard>();
			this.SelectedCards = new List<int>();
			this.CardFields = new List<EbCardField>();
			this.Buttons = new List<EbButton>();
			this.Buttons.Add(new EbButton { Text = "Continue" });
			//this.SummarizeFields = new List<EbCardField>();
		}

		[OnDeserialized]
		public void OnDeserializedMethod(StreamingContext context)
		{
			this.BareControlHtml = this.GetBareHtml();
			this.ObjType = this.GetType().Name.Substring(2, this.GetType().Name.Length - 2);
		}
		public override string GetToolHtml()
		{
			return @"<div eb-type='@toolName' class='tool'><i class='fa fa-window-restore'></i>@toolName</div>".Replace("@toolName", this.GetType().Name.Substring(2));
		}

		public override string GetJsInitFunc()
		{
			return @"this.Init = function(id)
					{
						//this.CardCollection.$values.push(new EbObjects.EbCard(id + '_EbCard0'));
					};";
		}

		public string ButtonsString
		{
			get
			{
				string html = @"<div class='cards-btn-cont'>";
				foreach (EbButton ec in this.Buttons)
					html += ec.GetHtml();
				return html + "</div>";
			}
			set { }
		}

		public override string GetDesignHtml()
		{
			return @"`<div id=@id><div class='cards-cont'>
						<div class='card-cont' style='width: 100%; min-height: 100px; box-shadow: 0px 0px 20px #ccc; border-radius: 1.3em 1.3em 0 0;'>
							<div class='card-btn-cont'><button class='btn btn-default' style='width:100%;' disabled>Select</button></div>
						</div>
						<div class='card-summary-cont' style='box-shadow: 0px 0px 20px #ccc; border-radius: 0 0 1.3em 1.3em; margin: 5px -4px 0 6px;'><div style='font-size: 15px; padding:5px 5px 0px 5px; text-align:center;'><b> Summary </b></div>
							<table class='table card-summary-table'>
								<thead style='font-size:12px;'>
									<tr>
										<th>Column 1</th>
										<th>Column 2</th>
										<th>Column 3</th>
									</tr>
								</thead>
								<tbody style='font-size:12px;'><tr><td style='text-align:center; border: none;' colspan=3><i> Nothing to Display </i></td></tr>  </tbody>
							</table>
						</div>
						
					</div></div>`";
		}

		public override string GetHtml()
		{
			return @"<div id='cont_@name@' Ctype='Cards' class='Eb-ctrlContainer' style='@hiddenString'>
						@GetBareHtml@
					</div>"
						.Replace("@name@", this.Name ?? "@name@")
						.Replace("@GetBareHtml@", this.GetBareHtml());
		}

		public override string GetBareHtml()
		{
			return "";
		}


		public string getCartHtml()
		{
			this.IsSummaryRequired = false;
			int tcols = 1;
			string html = @"<div class='card-summary-cont'><div style='font-size: 15px; padding:5px 5px 0px 5px; text-align:center;'><b> @Summary@ </b></div>
							<table class='table card-summary-table'>
								<thead style='font-size:12px;'><tr>".Replace("@Summary@",this.SummaryTitle.IsNullOrEmpty()? "Summary": this.SummaryTitle);
			foreach(EbCardField F in this.CardFields)
			{
				if (F.Summarize)
				{
					html += "<th>" + F.Name + "</th>";
					this.IsSummaryRequired = true;
					tcols++;
				}
			}
			html += @"<th></th></tr></thead><tbody style='font-size:12px;'>  <tr><td style='text-align:center;' colspan=" + tcols + "><i> Nothing to Display </i></td></tr>  </tbody></table></div>";
			if (this.IsSummaryRequired)
				return html;
			else
				return null;	
		}
	}
	
}