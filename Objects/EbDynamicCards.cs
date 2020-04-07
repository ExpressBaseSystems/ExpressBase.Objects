using ExpressBase.Common;
using ExpressBase.Common.Extensions;
using ExpressBase.Common.JsonConverters;
using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using ExpressBase.Common.Structures;
using ExpressBase.Objects.Objects.DVRelated;
using ExpressBase.Objects.ServiceStack_Artifacts;
using Newtonsoft.Json;
using ServiceStack;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Reflection;
using System.Runtime.Serialization;

namespace ExpressBase.Objects
{
    [EnableInBuilder(BuilderType.BotForm)]
	public class EbDynamicCardSet : EbCardSetParent
    {		
		[EnableInBuilder(BuilderType.BotForm)]
		[HideInPropertyGrid]
		public override List<EbCard> CardCollection { get; set; }

        [EnableInBuilder(BuilderType.BotForm)]
        [OSE_ObjectTypes(EbObjectTypes.iDataReader)]
        [PropertyEditor(PropertyEditorType.ObjectSelector)]
        public string DataSourceId { get; set; }

        [EnableInBuilder(BuilderType.BotForm)]
        [HideInPropertyGrid]
        public DVColumnCollection Columns { get; set; }

        [EnableInBuilder(BuilderType.BotForm)]
        [PropertyEditor(PropertyEditorType.CollectionFrmSrc, "Columns", 1)]
        [OnChangeExec(@"if (this.Columns.$values.length === 0 ){pg.MakeReadOnly('ValueMember');} else {pg.MakeReadWrite('ValueMember');}")]
        public DVBaseColumn ValueMember { get; set; }

		

		public EbDynamicCardSet()
		{
			this.CardCollection = new List<EbCard>();
			this.ObjType = this.GetType().Name.Substring(2, this.GetType().Name.Length - 2);
		}

		[OnDeserialized]
		public void OnDeserializedMethod(StreamingContext context)
        {
            this.BareControlHtml = this.GetBareHtml();
            this.BareControlHtml4Bot = this.BareControlHtml;
            this.ObjType = this.GetType().Name.Substring(2, this.GetType().Name.Length - 2);
			//this.FilterField = "ftype";/////////Hard coding for test filter field //febin
			//this.SearchField = "Title0";/////////Hard coding for test search field //febin
		}
		
        public void InitFromDataBase(JsonServiceClient ServiceClient)
        {
            RowColletion ds = (ServiceClient.Get<DataSourceDataResponse>(new DataSourceDataRequest { RefId = this.DataSourceId , Start = 0, Length = 1000})).Data;

            for (int i = 0; i < ds.Count; i++)
            {
                EbCard Card = new EbCard() { EbSid = "cardEbsid_" + i };
                foreach (EbCardField Field in this.CardFields)
                {
					if (Field.DbFieldMap != null)
					{
						var tempdata = ds[i][Field.DbFieldMap.Data];
						if (Field is EbCardNumericField)
							Card.CustomFields[Field.Name] = Convert.ToDouble(tempdata);
						else
							Card.CustomFields[Field.Name] = tempdata.ToString().Trim();

						//for getting distinct filter values
						if (this.FilterField?.Name != null && Field.Name == this.FilterField.Name && !this.FilterValues.Contains(tempdata.ToString().Trim()))
						{
							this.FilterValues.Add(tempdata.ToString().Trim());
						}
                    }
                }
                Card.CardId = Convert.ToInt32(ds[i][this.ValueMember.Data]);
                Card.Name = "CardIn" + this.Name;//------------------------"CardIn"		

                this.CardCollection.Add(Card);
            }
        }
    }


	//------------------------------------------------PARENT CLASS------------------------------------------------

	[EnableInBuilder(BuilderType.BotForm)]
    //[HideInToolBox]
    public class EbCardSetParent : EbControlUI
    {
        public EbCardSetParent()
        {
            this.SelectedCards = new List<int>();
            this.CardFields = new List<EbCardField>();

			
			this.FilterValues = new List<string>();
        }

		public virtual List<EbCard> CardCollection { get; set; }

		[EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm)]
		[HideInPropertyGrid]
		public override EbDbTypes EbDbType { get { return EbDbTypes.Decimal; } }

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

		[EnableInBuilder(BuilderType.BotForm)]
		public string ButtonText { get; set; }

		[EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
		[HideInPropertyGrid]
		public override bool IsFullViewContol { get => true; set => base.IsFullViewContol = value; }

		[EnableInBuilder(BuilderType.BotForm)]
		//[HideInPropertyGrid]//
        [PropertyEditor(PropertyEditorType.DDfromDictProp, "CardFields", 1)]
		public EbControl FilterField { get; set; }

		public List<string> FilterValues { get; set; }

		[EnableInBuilder(BuilderType.BotForm)]
		//[HideInPropertyGrid]//
		[PropertyEditor(PropertyEditorType.DDfromDictProp, "CardFields", 1)]
		public EbControl SearchField { get; set; }

		[HideInPropertyGrid]
		public override bool IsReadOnly {
			get
			{
				foreach(EbCardField field in this.CardFields)
				{
					if (!field.DoNotPersist)
						return false;
				}
				return true;
			}
        }

        [HideInPropertyGrid]
        [JsonIgnore]
        public override string ToolIconHtml { get { return "<i class='fa fa-window-restore'></i>"; } set { } }

  //      public override string GetToolHtml()
		//{
		//	return @"<div eb-type='@toolName' class='tool'><i class='fa fa-window-restore'></i>@toolName</div>"
		//			.Replace("@toolName", this.GetType().Name.Substring(2));
		//}

		public override string GetJsInitFunc()
		{
			return @"this.Init = function(id)
					{
						//this.CardCollection.$values.push(new EbObjects.EbCard(id + '_EbCard0'));						
						var field0 = new EbObjects.EbCardImageField(id + '_EbCardImageField0');
						var field1 = new EbObjects.EbCardTitleField(id + '_EbCardTitleField1');
						var field2 = new EbObjects.EbCardHtmlField(id + '_EbCardHtmlField2');
						var field3 = new EbObjects.EbCardNumericField(id + '_EbCardNumericField3');
						var field4 = new EbObjects.EbCardTextField(id + '_EbCardTextField4');
						field0.Name = 'Image0';
						field1.Name = 'Title1';
						field2.Name = 'Html2';
						field3.Name = 'Numeric3';
						field4.Name = 'Text4';
						this.CardFields.$values.push(field0);
						this.CardFields.$values.push(field1);
						this.CardFields.$values.push(field2);
						this.CardFields.$values.push(field3);
						this.CardFields.$values.push(field4);
					};";
		}

		public override string GetBareHtml()
		{
			string html = @"<div id='@name@' class='Eb-ctrlContainer'>@HeaderHtml@ 
								<div style='position: absolute; margin-top: 25px; text-align: center; width: 100%; font-size: 21px; color: #bbb; font-weight: 300;'>Nothing to Display</div> 
								<div class='cards-cont'>"
									.Replace("@name@", this.EbSid ?? "@name@")
									.Replace("@HeaderHtml@", this.getHeaderHtml());
			
			if(CardCollection != null)
			{
				foreach (EbCard card in CardCollection)
				{
					html += @"<div id='@name@' class='card-cont' card-id='@cardid@' filter-value='@FilterValue@' search-value='@SearchValue@' style='width:100%;'>"
									.Replace("@name@", card.EbSid.Trim())
									.Replace("@cardid@", card.CardId.ToString())
									.Replace("@FilterValue@", this.FilterField?.Name == null ? "": card.CustomFields[this.FilterField.Name].ToString())
									.Replace("@SearchValue@", this.SearchField?.Name == null ? "": card.CustomFields[this.SearchField.Name].ToString());
					foreach (EbCardField cardField in this.CardFields)
					{
						cardField.FieldValue = card.CustomFields.ContainsKey(cardField.Name) ? card.CustomFields[cardField.Name] : null;
						html += cardField.GetBareHtml();
					}

					html += "<div class='card-btn-cont' style='@BtnDisplay@'><button id='' class='btn btn-default'  data-toggle='tooltip' title='' style='width:100%;'>Select</button></div></div>".Replace("@BtnDisplay@", this.MultiSelect ? "" : "display:none;");
				}
			}			
			html += "</div>@SummarizeHtml@  <div class='cards-btn-cont' style='margin-top: 20px;'> <button id='' class='btn btn-default ctrl-submit-btn'  data-toggle='tooltip' title=''> @ButtonText@ </button> </div> </div>"
				.Replace("@SummarizeHtml@", (this.getCartHtml().IsNullOrEmpty() || !this.MultiSelect) ? "" : this.getCartHtml())
				.Replace("@ButtonText@", this.ButtonText.IsNullOrEmpty() ? (this.IsReadOnly? "OK": "Submit") : this.ButtonText);
			return html;
		}

		public string getCartHtml()
		{
			this.IsSummaryRequired = false;
			int tcols = 1;
			string html = @"<div class='card-summary-cont'><div style='font-size: 15px; padding:5px 5px 0px 5px; text-align:center;'><b> @Summary@ </b></div>
							<table class='table card-summary-table' style='table-layout: fixed; margin-bottom: 0px;'>
								<thead style='font-size:12px;'><tr>".Replace("@Summary@", this.SummaryTitle.IsNullOrEmpty() ? "Summary" : this.SummaryTitle);
			foreach (EbCardField F in this.CardFields)
			{
				if (F.Summarize)
				{
					string tempst = F.Label.IsNullOrEmpty() ? F.Name : F.Label;
					string colStyle = ((F.SummarizeColumnWidth > 0) ? "width: " + F.SummarizeColumnWidth + "%;" : "") + ((F is EbCardNumericField) ? "text-align: right;":"") + " white-space: nowrap; overflow: hidden; text-overflow: ellipsis;";
					html += "<th style='" + colStyle + "' title='" + tempst + "'>" + tempst + "</th>";
					this.IsSummaryRequired = true;
					tcols++;
				}
			}
			html += @"<th style='width: 26px;'></th></tr></thead><tbody style='font-size:12px;'>  <tr><td style='text-align:center;' colspan=" + tcols + "><i> Nothing to Display </i></td></tr>  </tbody></table></div>";
			if (this.IsSummaryRequired)
				return html;
			else
				return null;
		}

		//public string getFilterHtml()
		//{
		//	string html = string.Empty;
		//	if (this.FilterValues.Count != 0)
		//	{
		//		html += @"<div class='card-filter-cont'><select id='' name='' data-ebtype=''>";
		//		foreach(string val in this.FilterValues)
		//		{
		//			html += @"<option value='" + val + "'>" + val + "</option>";
		//		}
		//		html += "</select></div>";
		//	}
		//	return html;
		//}	
		
		public string getHeaderHtml()
		{
			string html = @"<div class='card-header-cont'> 
								<div class='card-head-cardno'>
									1 of 1
								</div>";
			string shtml = @"	<div class='card-head-searchdiv'>
									<input type='text' class='card-head-search-box form-control' placeholder='Search' title='Search' style=' height: 28px; min-height: 25px; border-radius: 12px;border: none;padding-right: 22px; -webkit-transition: width 0.4s ease-in-out; transition: width 0.4s ease-in-out;'/>            
									<i class='fa fa-search card-head-search-icon form-control-feedback' aria-hidden='true'></i>
								</div>";
			string fhtml = string.Empty;
			if (this.FilterValues.Count != 0)
			{
				fhtml += @"<div class='card-head-filterdiv'>
							<select class='card-head-filter-box form-control'> <option value='All'> All </option>";
				foreach (string val in this.FilterValues)
				{
					fhtml += @"<option value='" + val + "'>" + val + "</option>";
				}
				fhtml += "</select> <i class='fa fa-filter card-head-filter-icon' aria-hidden='true'></i></div>";
			}
			if (this.SearchField?.Name != null)
				html += shtml;
			if (this.FilterField?.Name != null)
				html += fhtml;
			html += "</div>";

			return html;
		}

		public override string DesignHtml4Bot
		{
			//get 
			//{
			//	string html = @"<div id='@name@' class=''>@HeaderHtml@ 
			//					<div style='position: absolute; margin-top: 25px; text-align: center; width: 100%; font-size: 21px; color: #bbb; font-weight: 300;'>Nothing to Display</div> 
			//					<div class='cards-cont'>"
			//							.Replace("@name@", this.EbSid ?? "@name@")
			//							.Replace("@HeaderHtml@", this.getHeaderHtml());

			//	if (CardCollection != null)
			//	{
			//		EbCard card = CardCollection[0];

			//			html += @"<div id='@name@' class='card-cont' card-id='@cardid@' filter-value='@FilterValue@' search-value='@SearchValue@' style='width:100%;'>"
			//							.Replace("@name@", card.EbSid.Trim())
			//							.Replace("@cardid@", card.CardId.ToString())
			//							.Replace("@FilterValue@", this.FilterField?.Name == null ? "" : card.CustomFields[this.FilterField.Name].ToString())
			//							.Replace("@SearchValue@", this.SearchField?.Name == null ? "" : card.CustomFields[this.SearchField.Name].ToString());
			//			foreach (EbCardField cardField in this.CardFields)
			//			{
			//				cardField.FieldValue = card.CustomFields.ContainsKey(cardField.Name) ? card.CustomFields[cardField.Name] : null;
			//				html += cardField.GetBareHtml();
			//			}

			//			html += "<div class='card-btn-cont' style='@BtnDisplay@'><button id='' class='btn btn-default'  data-toggle='tooltip' title='' style='width:100%;'>Select</button></div></div>".Replace("@BtnDisplay@", this.MultiSelect ? "" : "display:none;");

			//	}
			//	html += "</div>@SummarizeHtml@  <div class='cards-btn-cont' style='margin-top: 20px;'> <button id='' class='btn btn-default'  data-toggle='tooltip' title='' style='width:100%; box-shadow: 0px 0px 10px #ccc; border-radius: 1.3em 1.3em 1.3em 1.3em;'> @ButtonText@ </button> </div> </div>"
			//		.Replace("@SummarizeHtml@", (this.getCartHtml().IsNullOrEmpty() || !this.MultiSelect) ? "" : this.getCartHtml())
			//		.Replace("@ButtonText@", this.ButtonText.IsNullOrEmpty() ? (this.IsReadOnly ? "OK" : "Submit") : this.ButtonText);
			//	return html;
			//}
			get => @"<div id=@id class=''><div class='cards-cont'>
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
					</div></div>";

			set => base.DesignHtml4Bot = value;
		}

		public override string GetDesignHtml()
		{
			return @"`<div id=@id class='Eb-ctrlContainer'><div class='cards-cont'>
						<div class='card-cont' style='width: 100%; min-height: 100px; box-shadow: 0px 0px 20px #ccc; border-radius: 1.3em;'>
							<div class='card-btn-cont'><button class='btn btn-default' style='width:100%;' disabled>Select</button></div>
						</div>
						<div class='card-summary-cont' style='box-shadow: 0px 0px 20px #ccc; border-radius: 0; margin: 20px -4px 0 6px;'><div style='font-size: 15px; padding:5px 5px 0px 5px; text-align:center;'><b> Summary </b></div>
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
			return @"<div id='cont_@name@' Ctype='Cards' class='Eb-ctrlContainer' eb-hidden='@isHidden@'>
						@GetBareHtml@
					</div>"
                        .Replace("@name@", this.EbSid ?? "@name@")
						.Replace("@GetBareHtml@", this.GetBareHtml());
		}

		

		//public override string GetDesignHtml()
		//{
		//	return @"`<div id=@id class='Eb-ctrlContainer'><div class='cards-cont'>
		//				<div class='card-cont' style='width: 100%; min-height: 100px; box-shadow: 0px 0px 20px #ccc; border-radius: 1.3em;'>
		//					<div class='card-btn-cont'><button class='btn btn-default' style='width:100%;' disabled>Select</button></div>
		//				</div>
		//				<div class='card-summary-cont' style='box-shadow: 0px 0px 20px #ccc; border-radius: 0; margin: 20px -4px 0 6px;'><div style='font-size: 15px; padding:5px 5px 0px 5px; text-align:center;'><b> Summary </b></div>
		//					<table class='table card-summary-table'>
		//						<thead style='font-size:12px;'>
		//							<tr>
		//								<th>Column 1</th>
		//								<th>Column 2</th>
		//								<th>Column 3</th>
		//							</tr>
		//						</thead>
		//						<tbody style='font-size:12px;'><tr><td style='text-align:center; border: none;' colspan=3><i> Nothing to Display </i></td></tr>  </tbody>
		//					</table>
		//				</div>

		//			</div></div>`";
		//}
	}


	[EnableInBuilder(BuilderType.BotForm)]
    [HideInToolBox]
    public class EbCard : EbControl
    {
		[EnableInBuilder(BuilderType.BotForm)]
		[PropertyEditor(PropertyEditorType.Number)]
		public int CardId { get; set; }

		[JsonConverter(typeof(DictionaryConverter))]
		[EnableInBuilder(BuilderType.BotForm)]
		[PropertyEditor(PropertyEditorType.DictionaryEditor, "CardFields")]
		public IDictionary<string, object> CustomFields { get; set; }

		public EbCard()
		{
			this.CustomFields = new Dictionary<string, object>();
		}

		public override string GetBareHtml()
		{
			if (this.Name != null)
			{
				string html = @"<div id='@name@' class='card-cont' card-id='@cardid@' style='width:100%;'>".Replace("@name@", this.EbSid.Trim()).Replace("@cardid@", this.CardId.ToString());
				//foreach (EbCardField CardField in this.Fields)
				//{
				//    html += CardField.GetBareHtml();
				//}
				html += "<div class='card-btn-cont'>" + " <button id='' class='btn btn-default'  data-toggle='tooltip' title='' style='width:100%;'>Select</button>" + "</div></div>";
				return html;
			}
			return string.Empty;
		}
	}


	//------------------------------------------------CARD FIELDS------------------------------------------------

	[EnableInBuilder(BuilderType.BotForm)]
    [HideInToolBox]
    public abstract class EbCardField : EbControl
    {
        [EnableInBuilder(BuilderType.BotForm)]
        [HideInPropertyGrid]
        public DVColumnCollection Columns { get; set; }

        public virtual dynamic FieldValue { get; set; }

        [EnableInBuilder(BuilderType.BotForm)]
        [PropertyEditor(PropertyEditorType.CollectionFrmSrc, "Columns", 1)]
        [OnChangeExec(@"if (this.Columns.$values.length === 0 ){pg.MakeReadOnly('DbFieldMap');} else {pg.MakeReadWrite('DbFieldMap');}")]
        public DVBaseColumn DbFieldMap { get; set; }

        [EnableInBuilder(BuilderType.BotForm)]
		[OnChangeExec(@"if(this.Summarize === true){pg.ShowProperty('SummarizeColumnWidth');}
		else{pg.HideProperty('SummarizeColumnWidth');}")]
		public bool Summarize { get; set; }

		[EnableInBuilder(BuilderType.BotForm)]
		public int SummarizeColumnWidth { get; set; }

		[EnableInBuilder(BuilderType.BotForm)]
        public bool HideInCard { get; set; }

        [EnableInBuilder(BuilderType.BotForm)]
        public override bool DoNotPersist { get; set; }

		[EnableInBuilder(BuilderType.BotForm)]
		[HideInPropertyGrid]
		public override EbDbTypes EbDbType { get; set; }

		public string DesignHtml { get; set; }

		//[PropertyGroup(PGConstants.APPEARANCE)]
		//[EnableInBuilder(BuilderType.BotForm)]
		//[PropertyEditor(PropertyEditorType.FontSelector)]
		//public EbFont Font { get; set; }
	}


    [EnableInBuilder(BuilderType.BotForm)]
    //[PropertyEditor(PropertyEditorType.xxx)]
    [HideInToolBox]
    [Alias("Image")]
    public class EbCardImageField : EbCardField
    {
        [EnableInBuilder(BuilderType.BotForm)]
        //[PropertyEditor(PropertyEditorType.ImageSeletor)]
        [Alias("ImageID")]
		[MetaOnly]
		public override dynamic FieldValue { get; set; }

		[HideInPropertyGrid]
		public override string Label { get; set; }

		[EnableInBuilder(BuilderType.BotForm)]
		public int HeigthInPixel { get; set; }

		[EnableInBuilder(BuilderType.BotForm)]
		[HideInPropertyGrid]
		public override EbDbTypes EbDbType { get { return EbDbTypes.String; } }

		public EbCardImageField() { }
		
		[OnDeserialized]
		public void OnDeserializedMethod(StreamingContext context)
		{
			this.DesignHtml = this.GetDesignHtml();
			this.DesignHtml = this.DesignHtml.Substring(1, this.DesignHtml.Length - 2);
		}

		public override string DesignHtml4Bot
		{
			get => @"<div><img class='card-img' src='../images/image.png' style='width: 100%; height: 200px; opacity: 0.2;'/></div>";
			set => base.DesignHtml4Bot = value;
		}

		public override string GetDesignHtml()
        {
            return @"`<div><img class='card-img' src='../images/image.png' style='width: 100%; height: 200px; opacity: 0.2;'/></div>`";
        }

        public override string GetBareHtml()
        {
            return @"<div style='@divstyle@'><img class='card-img' src='@ImageID@'/></div>"
				.Replace("@ImageID@", (String.IsNullOrEmpty(this.FieldValue)) ? "../images/image.png": this.FieldValue)
				.Replace("@divstyle@", (this.HeigthInPixel == 0)? "margin: 10px 0px;" : "height: " + this.HeigthInPixel + "px; display: flex; justify-content: center; margin: 10px 0px;");
        }
    }

    [EnableInBuilder(BuilderType.BotForm)]
    [HideInToolBox]
    [Alias("Html")]
    public class EbCardHtmlField : EbCardField
    {
        [EnableInBuilder(BuilderType.BotForm)]
        [PropertyEditor(PropertyEditorType.String64)]
        [Alias("ContentHTML")]
		[MetaOnly]
		public override dynamic FieldValue { get; set; }

		[HideInPropertyGrid]
		public override string Label { get; set; }

		[EnableInBuilder(BuilderType.BotForm)]
		[HideInPropertyGrid]
		public override EbDbTypes EbDbType { get { return EbDbTypes.String; } }

		public EbCardHtmlField() { }
		
		[OnDeserialized]
		public void OnDeserializedMethod(StreamingContext context)
		{
			this.DesignHtml = this.GetDesignHtml();
			this.DesignHtml = this.DesignHtml.Substring(1, this.DesignHtml.Length - 2);
		}

		public override string DesignHtml4Bot
		{
			get => @"<div class='card-contenthtml-cont' style='padding:5px; text-align: center; width: 100%; min-height: 50px;'> HTML Content </div>";
			set => base.DesignHtml4Bot = value;
		}

		public override string GetDesignHtml()
        {
            return @"`<div class='card-contenthtml-cont' style='padding:5px; text-align: center; width: 100%; min-height: 50px;'> HTML Content </div>`";
        }

        public override string GetBareHtml()
        {
            return @"<div class='card-contenthtml-cont data-@Name@' style='padding:5px;'> @ContentHTML@ </div>".Replace("@ContentHTML@", (this.FieldValue == null) ? "" : this.FieldValue).Replace("@Name@", this.Name);
        }
    }

    [EnableInBuilder(BuilderType.BotForm)]
    [HideInToolBox]
    [Alias("Numeric")]
    public class EbCardNumericField : EbCardField
    {

        [EnableInBuilder(BuilderType.BotForm)]
        [Alias("Value")]
		[MetaOnly]
		[PropertyEditor(PropertyEditorType.Number)]
        public override dynamic FieldValue { get; set; }

        [EnableInBuilder(BuilderType.BotForm)]
        [PropertyEditor(PropertyEditorType.ScriptEditorJS)]
        public EbScript ValueExpression { get; set; }

        [EnableInBuilder(BuilderType.BotForm)]
        public bool Sum { get; set; }

        [EnableInBuilder(BuilderType.BotForm)]
        public override bool ReadOnly { get; set; }

		[EnableInBuilder(BuilderType.BotForm)]
		[PropertyEditor(PropertyEditorType.Number)]
		[DefaultPropValue("1000000")]
		[OnChangeExec(@"
		if($(event.target).val() > this.MaximumValue){
			$(event.target).val('0');
			this.MinimumValue = 0 ;
		}
			")]
		public int MinimumValue { get; set; }

		[EnableInBuilder(BuilderType.BotForm)]
		[PropertyEditor(PropertyEditorType.Number)]
		[OnChangeExec(@"
		if($(event.target).val() <= this.MinimumValue){
			$(event.target).val('999999');
			this.MinimumValue = 999999 ;
		}
			")]
		public int MaximumValue { get; set; }

		[EnableInBuilder(BuilderType.BotForm)]
		public override string Label { get; set; }

		[EnableInBuilder(BuilderType.BotForm)]
		[HideInPropertyGrid]
		public override EbDbTypes EbDbType { get { return EbDbTypes.Double; } }

		public EbCardNumericField() { }

		[OnDeserialized]
		public void OnDeserializedMethod(StreamingContext context)
		{
			this.DesignHtml = this.GetDesignHtml();
			this.DesignHtml = this.DesignHtml.Substring(1, this.DesignHtml.Length - 2);
		}

		public override string DesignHtml4Bot
		{
			get => @"<div class='card-numeric-cont data-@Name@' style='@display@' data-value='@Value@'>
						<div style='display: inline-block; width: 38%;'> <b> &nbsp&nbsp Numeric Field </b> </div> 
						<div style='display: inline-block; width: 58%;'>
							<button style='padding: 0px; border: none; background-color: transparent; font-size: 14px;' disabled>
								<i class='fa fa-minus' aria-hidden='true' style=' padding: 5px; color: darkblue;'></i>
							</button>
							<div style='display:inline-block; border: 1px solid #eee;'>
								<input class='removeArrows' type='number' style='text-align: center; border: none; background: transparent; width: 120px;' value='12345' readonly>
							</div>
							<button style='padding: 0px; border: none; background-color: transparent; font-size: 14px;' disabled>
								<i class='fa fa-plus' aria-hidden='true' style=' padding: 5px; color: darkblue;'></i>
							</button>
						</div>
					</div>";
			set => base.DesignHtml4Bot = value;
		}

		public override string GetDesignHtml()
        {
            return @"`<div class='card-numeric-cont data-@Name@' style='@display@' data-value='@Value@'>
						<div style='display: inline-block; width: 38%;'> <b> &nbsp&nbsp Numeric Field </b> </div> 
						<div style='display: inline-block; width: 58%;'>
							<button style='padding: 0px; border: none; background-color: transparent; font-size: 14px;' disabled>
								<i class='fa fa-minus' aria-hidden='true' style=' padding: 5px; color: darkblue;'></i>
							</button>
							<div style='display:inline-block; border: 1px solid #eee;'>
								<input class='removeArrows' type='number' style='text-align: center; border: none; background: transparent; width: 120px;' value='12345' readonly>
							</div>
							<button style='padding: 0px; border: none; background-color: transparent; font-size: 14px;' disabled>
								<i class='fa fa-plus' aria-hidden='true' style=' padding: 5px; color: darkblue;'></i>
							</button>
						</div>
					</div>`";
        }

        //public override string GetBareHtml()
        //{
        //    return @"<div class='card-numeric-cont data-@Name@' style='@display@'> <b>@Label@</b> <input type='number' value='@Value@' style='text-align:center; width: 100%;' min='1' max='9999' step='1' @ReadOnly@> </div>"
        //            .Replace("@Value@", (this.FieldValue == null) ? "1" : this.FieldValue.ToString())
        //            .Replace("@display@", this.HideInCard ? "display:none;" : "").Replace("@Name@", this.Name ?? "")
        //            .Replace("@Label@", this.Label.IsNullOrEmpty() ? this.Name : this.Label).Replace("@ReadOnly@", this.ReadOnly ? "readonly" : "");
        //}

		public override string GetBareHtml()
		{
			dynamic tempvar;
			if (this.FieldValue is String)
				tempvar = Convert.ToDouble(this.FieldValue);
			else
				tempvar = this.FieldValue;
			//Console.WriteLine(this.FieldValue.ToString() + "----" + this.FieldValue.GetType().ToString());
			return @"<div class='card-numeric-cont data-@Name@' style='@display@' data-value='@Value@'>
						<div style='display: inline-block; width: 38%;'> <b> &nbsp &nbsp @Label@ </b> </div> 
						<div style='display: inline-block; width: 58%;'>
							<button style='@PlusMinusDisplay@ padding: 0px; border: none; background-color: transparent; font-size: 14px;' 
										onclick='	var num = parseFloat($($(event.target).parent().next().children()[0]).val());
													if(num > @MinValue@){
														$($(event.target).parent().next().children()[0]).val(num - 1);
														$(event.target).parents(&quot;.card-numeric-cont&quot;).attr(&quot;data-value&quot;, num - 1);
													}
							'>
								<i class='fa fa-minus' aria-hidden='true' style=' padding: 5px; color: darkblue;'></i>
							</button>
							<div style='display:inline-block; @DivBorder@'>
								<input class='removeArrows' type='number' style='text-align: center; border: none; background: transparent; min-width: 90px;' value='@Value@' min='@MinValue@' max='@MaxValue@' @ReadOnly@  
										onchange='	var mn=parseFloat($(event.target).attr(&quot;min&quot;));
													var mx=parseFloat($(event.target).attr(&quot;max&quot;));
													var va=parseFloat($(event.target).val());
													if(va >= mn &amp;&amp; va <= mx){
														$(event.target).parents(&quot;.card-numeric-cont&quot;).attr(&quot;data-value&quot;, va);
													}
													else
														$(event.target).val($(event.target).parents(&quot;.card-numeric-cont&quot;).attr(&quot;data-value&quot;));
								'>
							</div>
							<button style='@PlusMinusDisplay@ padding: 0px; border: none; background-color: transparent; font-size: 14px;' 
										onclick='	var num = parseFloat($($(event.target).parent().prev().children()[0]).val());
													if(num < @MaxValue@) {
														$($(event.target).parent().prev().children()[0]).val(num + 1);
														$(event.target).parents(&quot;.card-numeric-cont&quot;).attr(&quot;data-value&quot;, num + 1);
													}
							'>
								<i class='fa fa-plus' aria-hidden='true' style=' padding: 5px; color: darkblue;'></i>
							</button>
						</div>
					</div>"
						.Replace("@Value@", (this.FieldValue == null) ? "1" : ((tempvar is Double)?tempvar.ToString("0.00") : tempvar.ToString()))
			            .Replace("@display@", this.HideInCard ? "display:none;" : "").Replace("@Name@", this.Name ?? "")
			            .Replace("@Label@", this.Label.IsNullOrEmpty() ? this.Name : this.Label)
						.Replace("@ReadOnly@", this.ReadOnly ? "readonly" : "")
						.Replace("@PlusMinusDisplay@", this.ReadOnly? "visibility: hidden;" : "display:inline-block;")
						.Replace("@DivBorder@", this.ReadOnly? "": "border: 1px solid #eee;")
						.Replace("@MinValue@", this.MinimumValue.ToString())
						.Replace("@MaxValue@", this.MaximumValue.ToString());
		}
    }

    [EnableInBuilder(BuilderType.BotForm)]
    [HideInToolBox]
    [Alias("Text")]
    public class EbCardTextField : EbCardField
    {
        [EnableInBuilder(BuilderType.BotForm)]
        [Alias("Text")]
		[MetaOnly]
		//[PropertyEditor(PropertyEditorType.String)]
		public override dynamic FieldValue { get; set; }

        [EnableInBuilder(BuilderType.BotForm)]
        [PropertyEditor(PropertyEditorType.ScriptEditorJS)]
        public EbScript ValueExpression { get; set; }
		
		[EnableInBuilder(BuilderType.BotForm)]
        public override bool ReadOnly { get; set; }

		[EnableInBuilder(BuilderType.BotForm)]
		public override string Label { get; set; }

		[EnableInBuilder(BuilderType.BotForm)]
		[HideInPropertyGrid]
		public override EbDbTypes EbDbType { get { return EbDbTypes.String; } }

		public EbCardTextField() { }

		[OnDeserialized]
		public void OnDeserializedMethod(StreamingContext context)
		{
			this.DesignHtml = this.GetDesignHtml();
			this.DesignHtml = this.DesignHtml.Substring(1, this.DesignHtml.Length - 2);
		}

		public override string DesignHtml4Bot
		{
			get => @"<div class='card-text-cont'>
						<div style='display: inline-block; width: 38%;'> 
							<b>&nbsp&nbsp Text Field </b>  
						</div>
						<div style='display: inline-block; width: 58%;'>
							<input type='text' value='@Text@' style='text-align: center;' readonly> 
						</div>
					</div>";
			set => base.DesignHtml4Bot = value;
		}

		public override string GetDesignHtml()
        {
            return @"`<div class='card-text-cont'>
						<div style='display: inline-block; width: 38%;'> 
							<b>&nbsp&nbsp Text Field </b>  
						</div>
						<div style='display: inline-block; width: 58%;'>
							<input type='text' value='@Text@' style='text-align: center;' readonly> 
						</div>
					</div>`";
        }

        public override string GetBareHtml()
        {
            return @"<div class='card-text-cont data-@Name@' style='@display@'>
						<div style='display: inline-block; width: 38%;'> 
							<b>&nbsp&nbsp @Label@ </b>  
						</div>
						<div style='display: inline-block; width: 58%;'>
							<input type='text' value='@Text@' style='text-align: center; width: 100%;' @ReadOnly@> 
						</div>
					</div>"
					.Replace("@Text@", (this.FieldValue == null) ? "" : this.FieldValue)
                    .Replace("@display@", this.HideInCard ? "display:none;" : "").Replace("@Name@", this.Name ?? "")
                    .Replace("@Label@", this.Label.IsNullOrEmpty() ? this.Name : this.Label).Replace("@ReadOnly@", this.ReadOnly ? "readonly" : "");
        }
    }

	[EnableInBuilder(BuilderType.BotForm)]
	[HideInToolBox]
    [Alias("Title")]
    public class EbCardTitleField : EbCardField
	{
		[EnableInBuilder(BuilderType.BotForm)]
		[Alias("Title")]
		[MetaOnly]
		//[PropertyEditor(PropertyEditorType.String)]
		public override dynamic FieldValue { get; set; }

		[HideInPropertyGrid]
		public override string Label { get; set; }

		[EnableInBuilder(BuilderType.BotForm)]
		[HideInPropertyGrid]
		public override EbDbTypes EbDbType { get { return EbDbTypes.String; } }

		public EbCardTitleField() { }

		[OnDeserialized]
		public void OnDeserializedMethod(StreamingContext context)
		{
			this.DesignHtml = this.GetDesignHtml();
			this.DesignHtml = this.DesignHtml.Substring(1, this.DesignHtml.Length - 2);
		}

		public override string DesignHtml4Bot
		{
			get => @"<div class='card-title-cont' style='font-weight: 600; font-size: 20px; padding: 5px;'>&nbsp&nbspTitle Field</div>";
			set => base.DesignHtml4Bot = value;
		}

		public override string GetDesignHtml()
		{
			return @"`<div class='card-title-cont' style='font-weight: 600; font-size: 20px; padding: 5px;'>&nbsp&nbspTitle Field</div>`";
		}

		public override string GetBareHtml()
		{
			return @"<div class='card-title-cont data-@Name@' style='font-weight: 600; font-size: 20px; padding: 5px;'> &nbsp @Text@ &nbsp <i class='fa fa-check' style='color: green;display: none;' aria-hidden='true'></i></div>"
					.Replace("@Text@", (this.FieldValue == null)? "" : this.FieldValue).Replace("@Name@", this.Name);
		}
	}

	[EnableInBuilder(BuilderType.BotForm)]
	[HideInToolBox]
	[Alias("Location")]
	public class EbCardLocationField : EbCardField
	{
		[EnableInBuilder(BuilderType.BotForm)]
		//[PropertyEditor(PropertyEditorType.String)]
		[Alias("Position")]
		[MetaOnly]
		public override dynamic FieldValue { get; set; }

		//[EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm)]
		//[PropertyEditor(PropertyEditorType.Expandable)]
		//public LatLng Position { get; set; }
		//public LatLng Lat_Long { get; set; }
		//public Decimal Latitude { get; set; }
		//public Decimal Longitude { get; set; }

		[EnableInBuilder(BuilderType.BotForm)]
		[HideInPropertyGrid]
		public override EbDbTypes EbDbType { get { return EbDbTypes.String; } }

		public EbCardLocationField() { }

		[OnDeserialized]
		public void OnDeserializedMethod(StreamingContext context)
		{
			this.DesignHtml = this.GetDesignHtml();
			this.DesignHtml = this.DesignHtml.Substring(1, this.DesignHtml.Length - 2);
		}

		public override string GetDesignHtml()
		{
			return @"`<div class='card-location-cont'>
							<div class='map-div' style='position: relative; overflow: hidden;'>
								<img style='width:100%;height: 100%;' src='/images/LocMapImg1.png'>
							</div>
						</div>`";
		}

		public override string DesignHtml4Bot {
			get => @"	<div class='card-location-cont'>
							<div class='map-div' style='position: relative; overflow: hidden;'>
								<img style='width:100%;height: 100%;' src='/images/LocMapImg1.png'>
							</div>
						</div>";
			set => base.DesignHtml4Bot = value;
		}

		public override string GetBareHtml()
		{
			return @"	<div id='@name@_Cont' class='card-location-cont' @DataLatLng@>
							<div id='@name@' class='map-div'></div>
						</div>"
								.Replace("@name@", (this.Name != null) ? this.EbSid : "@name@")
								.Replace("@DataLatLng@", String.IsNullOrEmpty(this.FieldValue) ? "": ("data-lat='" + this.FieldValue.Split(",")[0].Trim() + "' data-lng='" + this.FieldValue.Split(",")[1].Trim() + "'"));
		}
	}

}



//public sealed class MyDynObject : DynamicObject
//{
//	private readonly Dictionary<string, object> _properties;

//	public MyDynObject()
//	{
//		_properties = new Dictionary<string, object>();
//	}

//	public MyDynObject(Dictionary<string, object> properties)
//	{
//		_properties = properties;
//	}

//	public object this[string propertyName]
//	{
//		get { return _properties[propertyName]; }

//		set { _properties[propertyName] = value; }
//	}

//	public override IEnumerable<string> GetDynamicMemberNames()
//	{
//		return _properties.Keys;
//	}

//	public override bool TryGetMember(GetMemberBinder binder, out object result)
//	{
//		if (_properties.ContainsKey(binder.Name))
//		{
//			result = _properties[binder.Name];
//			return true;
//		}
//		else
//		{
//			result = null;
//			return false;
//		}
//	}

//	public override bool TrySetMember(SetMemberBinder binder, object value)
//	{
//		if (_properties.ContainsKey(binder.Name))
//		{
//			_properties[binder.Name] = value;
//			return true;
//		}
//		else
//		{
//			return false;
//		}
//	}
//}


//InitFromDataBase - old code backup
//EbDynamicCard Card = new EbDynamicCard();
//foreach (EbCardField Field in this.CardFields)
//{
//Type classType = Field.GetType();
//Object Obj = Activator.CreateInstance(classType);
//PropertyInfo propInfo = classType.GetProperties()[0];
//if(Field.DbFieldMap != null)
//	propInfo.SetValue(Obj, ds[i][Field.DbFieldMap.ColumnIndex].ToString().Trim());
//EbCardField FieldObj = Obj as EbCardField;

//FieldObj.HideInCard = Field.HideInCard;
//FieldObj.EbSid = Field.EbSid;
//FieldObj.Name = Field.Name;
//FieldObj.ReadOnly = Field.ReadOnly;

//Card.Fields.Add(FieldObj);
//}


//public override string GetBareHtml()
//{
//    string html = @"<div id='@name@'><div class='cards-cont'>".Replace("@name@", this.Name ?? "@name@");
//    foreach (EbDynamicCard card in CardCollection)
//    {
//        //html += Card.GetBareHtml();
//        html += @"<div id='@name@' class='card-cont' card-id='@cardid@' style='width:100%;'>".Replace("@name@", card.Name.Trim()).Replace("@cardid@", card.CardId.ToString());
//        foreach (EbCardField cardField in this.CardFields)
//        {
//            if (cardField.DbFieldMap != null)
//            {
//                cardField.FieldValue = card.CustomFields[cardField.Name].ToString();
//                //    if (cardField is EbCardImageField)
//                //        (cardField as EbCardImageField).FieldValue = card.FieldValues[cardField.Name].ToString();
//                //    else if (cardField is EbCardNumericField)
//                //        (cardField as EbCardNumericField).FieldValue = Convert.ToDouble(card.FieldValues[cardField.Name]);
//                //    else if (cardField is EbCardHtmlField)
//                //        (cardField as EbCardHtmlField).FieldValue = card.FieldValues[cardField.Name].ToString();
//                //    else if (cardField is EbCardTextField)
//                //        (cardField as EbCardTextField).FieldValue = card.FieldValues[cardField.Name].ToString();
//            }
//            html += cardField.GetBareHtml();
//        }
//        html += "<div class='card-btn-cont'>Hard codel o</div></div>";

//    }
//    html += "</div>@SummarizeHtml@@ButtonsString@</div>"
//        .Replace("@ButtonsString@", "<div class='cards-btn-cont> <button id='' class='btn btn-default'  data-toggle='tooltip' title='' style='width:100%;'>Submit</button></div>")
//        .Replace("@SummarizeHtml@", this.getCartHtml() ?? "");
//    return html;
//}

//   public string getCartHtml()
//   {
//       this.IsSummaryRequired = false;
//       int tcols = 1;
//       string html = @"<div class='card-summary-cont'><div style='font-size: 15px; padding:5px 5px 0px 5px; text-align:center;'><b> @Summary@ </b></div>
//		<table class='table card-summary-table' style='table-layout: fixed;'>
//			<thead style='font-size:12px;'><tr>".Replace("@Summary@", this.SummaryTitle.IsNullOrEmpty() ? "Summary" : this.SummaryTitle);
//       foreach (EbCardField F in this.CardFields)
//       {
//           if (F.Summarize)
//           {
//string colStyle = ((F.SummarizeColumnWidth > 0) ? "width: " + F.SummarizeColumnWidth + "%;" : "") + " white-space: nowrap; overflow: hidden; text-overflow: ellipsis;";
//html += "<th style='" + colStyle + "'>" + F.Name + "</th>";
//               this.IsSummaryRequired = true;
//               tcols++;
//           }
//       }
//       html += @"<th style='width: 26px;'></th></tr></thead><tbody style='font-size:12px;'>  <tr><td style='text-align:center;' colspan=" + tcols + "><i> Nothing to Display </i></td></tr>  </tbody></table></div>";
//       if (this.IsSummaryRequired)
//           return html;
//       else
//           return null;
//   }


//  [EnableInBuilder(BuilderType.BotForm)]
//  [HideInToolBox]
//  public class EbDynamicCard : EbCardParent
//  {

//public IDictionary<string, object> CustomFields { get; set; }

//public EbDynamicCard() { }

//      public override string GetBareHtml()
//      {
//          if (this.Name != null)
//          {
//              string html = @"<div id='@name@' class='card-cont' card-id='@cardid@' style='width:100%;'>".Replace("@name@", this.Name.Trim()).Replace("@cardid@", this.CardId.ToString());
//              //foreach (EbCardField CardField in this.Fields)
//              //{
//              //    html += CardField.GetBareHtml();
//              //}
//              html += "<div class='card-btn-cont'>" + " <button id='' class='btn btn-default'  data-toggle='tooltip' title='' style='width:100%;'>Select</button>" + "</div></div>";
//              return html;
//          }
//          return string.Empty;
//      }
//  }