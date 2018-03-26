using ExpressBase.Common;
using ExpressBase.Common.Extensions;
using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using ExpressBase.Common.Structures;
using ExpressBase.Objects.Objects.DVRelated;
using ExpressBase.Objects.ServiceStack_Artifacts;
using ServiceStack;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace ExpressBase.Objects
{
	[EnableInBuilder(BuilderType.BotForm)]
	public class EbCards : EbControl
	{
		[EnableInBuilder(BuilderType.BotForm)]
		[PropertyEditor(PropertyEditorType.Collection)]
		public List<EbCard> CardCollection { get; set; }

		//[PropertyGroup("Appearance")]
		//[EnableInBuilder(BuilderType.BotForm)]
		//[PropertyEditor(PropertyEditorType.FontSelector)]
		//public EbFont Font{ get; set; }

		[EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm)]
		[PropertyEditor(PropertyEditorType.Boolean)]
		[OnChangeExec(@"if(this.IsItemCard === true){pg.ShowProperty('Price')}
		else{pg.HideProperty('Price')}")]
		public bool IsItemCard { get; set; }

		[EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm)]
		[OSE_ObjectTypes(EbObjectTypes.iDataSource)]
		[PropertyEditor(PropertyEditorType.ObjectSelector)]
		public string DataSourceId { get; set; }

		[EnableInBuilder(BuilderType.BotForm)]
		[HideInPropertyGrid]
		public List<DVColumnCollection> Columns { get; set; }

		public List<int> SelectedCards { get; set; }

		[EnableInBuilder(BuilderType.BotForm)]
		[PropertyEditor(PropertyEditorType.CollectionFrmSrc, "Columns")]
		public DVBaseColumn Price { get; set; }

		public EbCards()
        {
            this.CardCollection = new List<EbCard>();
        }        

        [OnDeserialized]
        public void OnDeserializedMethod(StreamingContext context)
        {
            this.BareControlHtml = this.GetBareHtml();
            this.ObjType = this.GetType().Name.Substring(2, this.GetType().Name.Length - 2);
        }
        public override  string  GetToolHtml()
        {
            return @"<div eb-type='@toolName' class='tool'><i class='fa fa-window-restore'></i>@toolName</div>".Replace("@toolName", this.GetType().Name.Substring(2));
        }

        public void InitFromDataBase(JsonServiceClient ServiceClient)
        {
            this.DataSourceId = "eb_roby_dev-eb_roby_dev-2-1105-1828";
            RowColletion ds = (ServiceClient.Get<DataSourceDataResponse>(new DataSourceDataRequest { RefId = this.DataSourceId })).Data;
            string _html = string.Empty;

            foreach (EbDataRow cardRow in ds)
            {
                EbCard Card = new EbCard();
                Card.Name = cardRow[0].ToString().Trim();
                Card.Label = cardRow[1].ToString();
                Card.ContentHTML = cardRow[2].ToString();
                Card.ImageID = cardRow[3].ToString();
				Card.IsItemCard = this.IsItemCard;
				Card.Quantity = 1;
				Card.Price = 115;
				Card.Buttons.Add(new EbButton { Text = "Submit" });
				
                this.CardCollection.Add(Card);
            }
        }

        public override string GetJsInitFunc()
        {
            return @"this.Init = function(id)
					{
						this.CardCollection.$values.push(new EbObjects.EbCard(id + '_EbCard0'));
					};";
        }

        public override string GetDesignHtml()
        {
			this.CardCollection.Add(new EbCard());
            return GetHtml().RemoveCR().DoubleQuoted();
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
            string html = @"<div id='@name@' class='cards-cont'>".Replace("@name@", this.Name ?? "@name@");
            foreach (EbCard ec in this.CardCollection)
                html += ec.GetHtml();
            return html + "</div>".Replace("@name@", this.Name ?? "@name@");
        }

    }


    /// ////////////////////////////////

    [EnableInBuilder(BuilderType.BotForm)]
    [HideInToolBox]
    public class EbCard : EbControl
    {
        [EnableInBuilder(BuilderType.BotForm)]
        [PropertyEditor(PropertyEditorType.ImageSeletor)]
        public string ImageID { get; set; }

        [EnableInBuilder(BuilderType.BotForm)]
        public string ContentHTML { get; set; }

        [EnableInBuilder(BuilderType.BotForm)]
        [PropertyEditor(PropertyEditorType.Collection)]
        public List<EbButton> Buttons { get; set; }
		
		public bool IsItemCard { get; set; }

		public int Quantity { get; set; }

		public int Price { get; set; }

		public string ButtonsString
        {
            get
            {
                string html = @"<div class='card-btn-cont'>";
                foreach (EbButton ec in this.Buttons)
                    html += ec.GetHtml();
                return html + "</div>";
            }
            set { }
        }

        public EbCard()
        {
            this.Buttons = new List<EbButton>();
        }

		public override string GetDesignHtml()
		{
			return GetBareHtml().RemoveCR().DoubleQuoted();
		}


		public override string GetBareHtml()
        {
			string html = @"<div id='@name@' class='card-cont' style='width:100%;'>
								<img class='card-img' src='@ImageID@'/>
								<div class='card-bottom'>
									<div id='@name@Lbl' class='card-label' style='@LabelBackColor  @LabelForeColor font-weight: bold'> @Label@ </div>
									<div class='card-content'>
										@ContentHTML@
									</div>
									@CardHtml@
									@ButtonCollection@
								</div>
							</div>"
				   .Replace("@ButtonCollection@", this.ButtonsString)
				   .Replace("@name@", this.Name)//this.IsItemCard ? this.Name : "this.EbSid"
				   .Replace("@ContentHTML@", this.ContentHTML) //"Chat has become the center of the smartphone universe, so it makes sense that bots are being used to deliver information in a convenient and engaging manner. But how do brands or media companies")//
				   .Replace("@Label@", this.Label)//"TechCrunch")//
				   .Replace("@ImageID@", this.ImageID.IsNullOrEmpty() ? "../images/image.png" : this.ImageID)//"https://tctechcrunch2011.files.wordpress.com/2016/03/chat-bot.jpg?w=738")//
				   .Replace("@CardHtml@", this.GetItemCardHtml());//this.IsItemCard ? this.GetItemCardHtml() : ""
			return html;
        }

        public override string GetHtml()
        {
            return GetBareHtml();
        }

		public string GetItemCardHtml()
		{
			string html = @"<div class='footercard' style='width: 100%;'>
								<div style='width: 50%; display: inline-block;'>
									Count : <input class='itemCount' type='number' value='1' min='1' max='10' style='width: 50%;'>
									Price : <input class='itemPrice' type='text' value='@Price@' readonly style='width: 50%;'>
								</div>
								<div style='width: 45%; display: inline-block;'>
									<div ><button id='BtnSelect@name@' class='cardselectbtn btn btn-default' style='width:100%; vertical-align: bottom;'>Select</button></div>
								</div>
								<div class='shoppingcart'></div>
							</div>"
					.Replace("@name@", this.Name)
					.Replace("@Price@", this.Price.ToString());
			return html;
		}
    }
}
