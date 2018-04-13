using ExpressBase.Common;
using ExpressBase.Common.Extensions;
using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using ExpressBase.Common.Structures;
using ExpressBase.Objects.ServiceStack_Artifacts;
using ServiceStack;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Reflection;
using System.Runtime.Serialization;

namespace ExpressBase.Objects
{
	[EnableInBuilder(BuilderType.BotForm)]
	public class EbCardSetParent : EbControlUI
	{
		public EbCardSetParent()
		{
			this.SelectedCards = new List<int>();
			this.CardFields = new List<EbCardField>();
		}

		[EnableInBuilder(BuilderType.BotForm)]
		[PropertyEditor(PropertyEditorType.Collection)]
		public List<EbCardField> CardFields { get; set; }

		public bool IsSummaryRequired { get; set; }//////////////////////////////////// need rethink

		[EnableInBuilder(BuilderType.BotForm)]
		[PropertyEditor(PropertyEditorType.Boolean)]
		[OnChangeExec(@"if(this.MultiSelect === true){pg.ShowProperty('SummaryTitle');}
		else{pg.HideProperty('SummaryTitle');}")]
		public bool MultiSelect { get; set; }

		public List<int> SelectedCards { get; set; }

		[EnableInBuilder(BuilderType.BotForm)]
		public string SummaryTitle { get; set; }
	}

	[EnableInBuilder(BuilderType.BotForm)]
	public class EbDynamicCardSet : EbCardSetParent
	{
		public EbDynamicCardSet()
		{
			this.CardCollection = new List<EbDynamicCard>();
			this.ObjType = this.GetType().Name.Substring(2, this.GetType().Name.Length - 2);
		}

		[OnDeserialized]
		public void OnDeserializedMethod(StreamingContext context)
		{
			this.BareControlHtml = this.GetBareHtml();
			this.ObjType = this.GetType().Name.Substring(2, this.GetType().Name.Length - 2);
		}

		[EnableInBuilder(BuilderType.BotForm)]
		[PropertyEditor(PropertyEditorType.Collection)]
		public List<EbDynamicCard> CardCollection { get; set; }

		[EnableInBuilder(BuilderType.BotForm)]
		[OSE_ObjectTypes(EbObjectTypes.iDataSource)]
		[PropertyEditor(PropertyEditorType.ObjectSelector)]
		public string DataSourceId { get; set; }

		[EnableInBuilder(BuilderType.BotForm)]
		[HideInPropertyGrid]
		public ColumnColletion Columns { get; set; }

		[EnableInBuilder(BuilderType.BotForm)]
		[PropertyEditor(PropertyEditorType.CollectionFrmSrc, "Columns", 1)]
		[OnChangeExec(@"console.log(100); if (this.Columns.$values.length === 0 ){pg.MakeReadOnly('ValueMember');} else {pg.MakeReadWrite('ValueMember');}")]
		public EbDataColumn ValueMember { get; set; }

		public override bool isFullViewContol { get => true; set => base.isFullViewContol = value; }

		//public List<EbCardField> SummarizeFields { get; set; }

		public override string GetToolHtml()
		{
			return @"<div eb-type='@toolName' class='tool'><i class='fa fa-window-restore'></i>@toolName</div>".Replace("@toolName", this.GetType().Name.Substring(2));
		}

		public void InitFromDataBase(JsonServiceClient ServiceClient)
		{
			RowColletion ds = (ServiceClient.Get<DataSourceDataResponse>(new DataSourceDataRequest { RefId = this.DataSourceId })).Data;

			for (int i = 0; i < ds.Count; i++)
			{
				EbDynamicCard Card = new EbDynamicCard();
				foreach (EbCardField Field in this.CardFields)
				{
					//Type classType = Field.GetType();
					//Object Obj = Activator.CreateInstance(classType);
					//PropertyInfo propInfo = classType.GetProperties()[0];
					//if(Field.DbFieldMap != null)
					//	propInfo.SetValue(Obj, ds[i][Field.DbFieldMap.ColumnIndex].ToString().Trim());
					//EbCardField FieldObj = Obj as EbCardField;

					if (Field.DbFieldMap != null)
					{
						if (Field is EbCardImageField)
							Card.FieldValues[Field.Name] = ds[i][Field.DbFieldMap.ColumnIndex].ToString().Trim();
						else if (Field is EbCardNumericField)
							Card.FieldValues[Field.Name] = Convert.ToDouble(ds[i][Field.DbFieldMap.ColumnIndex]);
						else if (Field is EbCardHtmlField)
							Card.FieldValues[Field.Name] = ds[i][Field.DbFieldMap.ColumnIndex].ToString().Trim();
						else if (Field is EbCardTextField)
							Card.FieldValues[Field.Name] = ds[i][Field.DbFieldMap.ColumnIndex].ToString().Trim();
						else if (Field is EbCardTitleField)
							Card.FieldValues[Field.Name] = ds[i][Field.DbFieldMap.ColumnIndex].ToString().Trim();
					}



					//FieldObj.HideInCard = Field.HideInCard;
					//FieldObj.EbSid = Field.EbSid;
					//FieldObj.Name = Field.Name;
					//FieldObj.ReadOnly = Field.ReadOnly;

					//Card.Fields.Add(FieldObj);
				}
				//Card.CardId = i.ToString();
				Card.CardId = Convert.ToInt32(ds[i][this.ValueMember.ColumnIndex]);
				Card.Name = "CardIn" + this.Name;//------------------------"CardIn"		

				this.CardCollection.Add(Card);
			}
			//foreach (EbCardField Field in this.CardFields)
			//{
			//	if (Field.Summarize)
			//		this.SummarizeFields.Add(Field);
			//}
		}

		public override string GetJsInitFunc()
		{
			return @"this.Init = function(id)
					{
						//this.CardCollection.$values.push(new EbObjects.EbCard(id + '_EbCard0'));
					};";
		}

		public override string DesignHtml4Bot
		{
			get => @"<div id=@id><div class='cards-cont'>
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
						
					</div></div>"; set => base.DesignHtml4Bot = value;
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
			string html = @"<div id='@name@'><div class='cards-cont'>".Replace("@name@", this.Name ?? "@name@");
			foreach (EbDynamicCard card in CardCollection)
			{
				//html += Card.GetBareHtml();
				html += @"<div id='@name@' class='card-cont' card-id='@cardid@' style='width:100%;'>".Replace("@name@", card.Name.Trim()).Replace("@cardid@", card.CardId.ToString());
				foreach (EbCardField cardField in this.CardFields)
				{
					if (cardField.DbFieldMap != null)
					{
						if (cardField is EbCardImageField)
							(cardField as EbCardImageField).ImageID = card.FieldValues[cardField.Name].ToString();
						else if (cardField is EbCardNumericField)
							(cardField as EbCardNumericField).Value = Convert.ToDouble(card.FieldValues[cardField.Name]);
						else if (cardField is EbCardHtmlField)
							(cardField as EbCardHtmlField).ContentHTML = card.FieldValues[cardField.Name].ToString();
						else if (cardField is EbCardTextField)
							(cardField as EbCardTextField).Text = card.FieldValues[cardField.Name].ToString();
						else if (cardField is EbCardTitleField)
							(cardField as EbCardTitleField).Title = card.FieldValues[cardField.Name].ToString();
					}
					html += cardField.GetBareHtml();
				}
				html += "<div class='card-btn-cont'>Hard codel o</div></div>";

			}
			html += "</div>@SummarizeHtml@@ButtonsString@</div>"
				.Replace("@ButtonsString@", "Hard code button")
				.Replace("@SummarizeHtml@", this.getCartHtml() ?? "");
			return html;
		}
		
		public string getCartHtml()
		{
			this.IsSummaryRequired = false;
			int tcols = 1;
			string html = @"<div class='card-summary-cont'><div style='font-size: 15px; padding:5px 5px 0px 5px; text-align:center;'><b> @Summary@ </b></div>
							<table class='table card-summary-table'>
								<thead style='font-size:12px;'><tr>".Replace("@Summary@", this.SummaryTitle.IsNullOrEmpty() ? "Summary" : this.SummaryTitle);
			foreach (EbCardField F in this.CardFields)
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

	public sealed class MyDynObject : DynamicObject
	{
		private readonly Dictionary<string, object> _properties;

		public MyDynObject()
		{
			_properties = new Dictionary<string, object>();
		}

		public MyDynObject(Dictionary<string, object> properties)
		{
			_properties = properties;
		}

		public object this[string propertyName]
		{
			get { return _properties[propertyName]; }

			set { _properties[propertyName] = value; }
		}

		public override IEnumerable<string> GetDynamicMemberNames()
		{
			return _properties.Keys;
		}

		public override bool TryGetMember(GetMemberBinder binder, out object result)
		{
			if (_properties.ContainsKey(binder.Name))
			{
				result = _properties[binder.Name];
				return true;
			}
			else
			{
				result = null;
				return false;
			}
		}

		public override bool TrySetMember(SetMemberBinder binder, object value)
		{
			if (_properties.ContainsKey(binder.Name))
			{
				_properties[binder.Name] = value;
				return true;
			}
			else
			{
				return false;
			}
		}
	}

	/// ////////////////////////////////

	[EnableInBuilder(BuilderType.BotForm)]
	[HideInToolBox]
	//[GenerateDynamicMetaJsFunc("genCardmeta")]
	public class EbCardParent : EbControl
	{
		public int CardId { get; set; }

		public EbCardParent() { }
	}
	[EnableInBuilder(BuilderType.BotForm)]
	[HideInToolBox]
	public class EbStaticCard : EbCardParent
	{
		[EnableInBuilder(BuilderType.BotForm)]
		public Dictionary<string, object> CustomFields { get; set; }

		public EbStaticCard() {
			CustomFields.Add("baabu", new { name = "saabu" });
		}

		public override string GetBareHtml()
		{
			if (this.Name != null)
			{
				string html = @"<div id='@name@' class='card-cont' card-id='@cardid@' style='width:100%;'>".Replace("@name@", this.Name.Trim()).Replace("@cardid@", this.CardId.ToString());
				//foreach (EbCardField CardField in this.Fields)
				//{
				//    html += CardField.GetBareHtml();
				//}
				html += "<div class='card-btn-cont'>" + "NEED Hard coding" + "</div></div>";
				return html;
			}
			return string.Empty;
		}
	}

	[EnableInBuilder(BuilderType.BotForm)]
	[HideInToolBox]
	public class EbDynamicCard : EbCardParent
	{

		public MyDynObject FieldValues { get; set; }

		public EbDynamicCard() { }

		public override string GetBareHtml()
		{
			if (this.Name != null)
			{
				string html = @"<div id='@name@' class='card-cont' card-id='@cardid@' style='width:100%;'>".Replace("@name@", this.Name.Trim()).Replace("@cardid@", this.CardId.ToString());
				//foreach (EbCardField CardField in this.Fields)
				//{
				//    html += CardField.GetBareHtml();
				//}
				html += "<div class='card-btn-cont'>" + "NEED Hard coding" + "</div></div>";
				return html;
			}
			return string.Empty;
		}
	}

	[EnableInBuilder(BuilderType.BotForm)]
	public abstract class EbCardField : EbControl
	{
		[EnableInBuilder(BuilderType.BotForm)]
		[HideInPropertyGrid]
		public ColumnColletion Columns { get; set; }

		[EnableInBuilder(BuilderType.BotForm)]
		[PropertyEditor(PropertyEditorType.CollectionFrmSrc, "Columns", 1)]
		[OnChangeExec(@"console.log(100); if (this.Columns.$values.length === 0 ){pg.MakeReadOnly('DbFieldMap');} else {pg.MakeReadWrite('DbFieldMap');}")]
		public EbDataColumn DbFieldMap { get; set; }

		[EnableInBuilder(BuilderType.BotForm)]
		public bool Summarize { get; set; }

		[EnableInBuilder(BuilderType.BotForm)]
		public bool HideInCard { get; set; }

		[EnableInBuilder(BuilderType.BotForm)]
		public bool Persist { get; set; }

		[PropertyGroup("Appearance")]
		[EnableInBuilder(BuilderType.BotForm)]
		[PropertyEditor(PropertyEditorType.FontSelector)]
		public EbFont Font { get; set; }

		[EnableInBuilder(BuilderType.BotForm)]
		[HideInPropertyGrid]
		public dynamic Value { get; set; }
	}


	[EnableInBuilder(BuilderType.BotForm)]
	//[PropertyEditor(PropertyEditorType.xxx)]
	[HideInToolBox]
	public class EbCardImageField : EbCardField
	{
		[EnableInBuilder(BuilderType.BotForm)]
		[PropertyEditor(PropertyEditorType.ImageSeletor)]
		public string ImageID { get; set; }

		public EbCardImageField() { }

		public override string GetDesignHtml()
		{
			return @"`<div><img class='card-img' src='../images/image.png' style='width: 100%; height: 200px; opacity: 0.2;'/></div>`";
		}

		public override string GetBareHtml()
		{
			return @"<img class='card-img' src='@ImageID@'/>".Replace("@ImageID@", this.ImageID.IsNullOrEmpty() ? "../images/image.png" : this.ImageID);
		}
	}

	[EnableInBuilder(BuilderType.BotForm)]
	[HideInToolBox]
	public class EbCardHtmlField : EbCardField
	{
		[EnableInBuilder(BuilderType.BotForm)]
		[PropertyEditor(PropertyEditorType.String)]
		public string ContentHTML { get; set; }

		public EbCardHtmlField() { }

		public override string GetDesignHtml()
		{
			return @"`<div class='card-contenthtml-cont' style='padding:5px; text-align: center; width: 100%; min-height: 50px;'> HTML Content </div>`";
		}

		public override string GetBareHtml()
		{
			return @"<div class='card-contenthtml-cont data-@Name@' style='padding:5px;'> @ContentHTML@ </div>".Replace("@ContentHTML@", this.ContentHTML.IsNullOrEmpty() ? "" : this.ContentHTML).Replace("@Name@", this.Name);
		}
	}

	[EnableInBuilder(BuilderType.BotForm)]
	[HideInToolBox]
	public class EbCardNumericField : EbCardField
	{
		[EnableInBuilder(BuilderType.BotForm)]
		[PropertyEditor(PropertyEditorType.JS)]
		public string ValueExpression { get; set; }

		[EnableInBuilder(BuilderType.BotForm)]
		public bool Sum { get; set; }

		[EnableInBuilder(BuilderType.BotForm)]
		public override bool ReadOnly { get; set; }

		//[EnableInBuilder(BuilderType.BotForm)]
		//public double Value { get; set; }

		public EbCardNumericField() { }

		public override string GetDesignHtml()
		{
			return @"`<div class='card-numeric-cont'> <b>Numeric Field</b> <input type='number' value='1' style='text-align:center; width: 100%;' min='1' max='9999' step='1' readonly> </div>`";
		}

		public override string GetBareHtml()
		{
			return @"<div class='card-numeric-cont data-@Name@' style='@display@'> <b>@Label@</b> <input type='number' value='@Value@' style='text-align:center; width: 100%;' min='1' max='9999' step='1' @ReadOnly@> </div>"
					.Replace("@Value@", (this.Value == null) ? "1" : this.Value.ToString())
					.Replace("@display@", this.HideInCard ? "display:none;" : "").Replace("@Name@", this.Name ?? "")
					.Replace("@Label@", this.Label.IsNullOrEmpty() ? this.Name : this.Label).Replace("@ReadOnly@", this.ReadOnly ? "readonly" : "");
		}
	}

	[EnableInBuilder(BuilderType.BotForm)]
	[HideInToolBox]
	public class EbCardTextField : EbCardField
	{
		[EnableInBuilder(BuilderType.BotForm)]
		[PropertyEditor(PropertyEditorType.String)]
		public string Text { get; set; }

		[EnableInBuilder(BuilderType.BotForm)]
		[PropertyEditor(PropertyEditorType.JS)]
		public string ValueExpression { get; set; }

		[EnableInBuilder(BuilderType.BotForm)]
		public override bool ReadOnly { get; set; }

		public EbCardTextField() { }

		public override string GetDesignHtml()
		{
			return @"`<div class='card-text-cont'> <b>Text Field</b> <input type='text' value='Text' style='text-align:center; width:100%;' readonly> </div>`";
		}

		public override string GetBareHtml()
		{
			return @"<div class='card-text-cont data-@Name@' style='@display@'> <b>@Label@</b> <input type='text' value='@Text@' style='text-align:center; width:100%;' @ReadOnly@> </div>"
					.Replace("@Text@", this.Text.IsNullOrEmpty() ? "" : this.Text)
					.Replace("@display@", this.HideInCard ? "display:none;" : "").Replace("@Name@", this.Name ?? "")
					.Replace("@Label@", this.Label.IsNullOrEmpty() ? this.Name : this.Label).Replace("@ReadOnly@", this.ReadOnly ? "readonly" : "");
		}
	}

	[EnableInBuilder(BuilderType.BotForm)]
	[HideInToolBox]
	public class EbCardTitleField : EbCardField
	{
		[EnableInBuilder(BuilderType.BotForm)]
		[PropertyEditor(PropertyEditorType.String)]
		public string Title { get; set; }

		[EnableInBuilder(BuilderType.BotForm)]
		[PropertyEditor(PropertyEditorType.JS)]
		public string ValueExpression { get; set; }

		public EbCardTitleField() { }

		public override string GetDesignHtml()
		{
			return @"`<div class='card-title-cont' style='font-weight: 600; font-size: 20px; padding: 5px;'>Title Field</div>`";
		}

		public override string GetBareHtml()
		{
			return @"<div class='card-title-cont data-@Name@' style='font-weight: 600; font-size: 20px; padding: 5px;'> &nbsp @Text@ &nbsp <i class='fa fa-check' style='color: green;display: none;' aria-hidden='true'></i></div>"
					.Replace("@Text@", this.Title.IsNullOrEmpty() ? "" : this.Title).Replace("@Name@", this.Name);
		}
	}
}