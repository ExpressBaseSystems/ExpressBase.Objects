using ExpressBase.Common;
using ExpressBase.Common.Extensions;
using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
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

        public EbCards()
        {
            this.CardCollection = new List<EbCard>();
        }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm)]
        [OSE_ObjectTypes(EbObjectType.DataSource)]
        [PropertyEditor(PropertyEditorType.ObjectSelector)]
        public string DataSourceId { get; set; }

        [OnDeserialized]
        public void OnDeserializedMethod(StreamingContext context)
        {
            this.BareControlHtml = this.GetBareHtml();
            this.ObjType = this.GetType().Name.Substring(2, this.GetType().Name.Length - 2);
        }

        //public void InitFromDataBase(JsonServiceClient ServiceClient)
        //{
        //    for (int i = 0; i < 2; i++)
        //    {
        //        EbCard Card = new EbCard();
        //        Card.Name = i+" label from DS";
        //        Card.Label = i + " labelfrom DS";
        //        Card.ContentHTML = i + " ContentHTML from DS";
        //        Card.ImageID = i + " ImageURL from DS";
        //        //Card.BareControlHtml = Card.GetBareHtml();
        //        this.CardCollection.Add(Card);
        //    }
        //}


        public void InitFromDataBase(JsonServiceClient ServiceClient)
        {

            this.DataSourceId = "eb_roby_dev-eb_roby_dev-2-1105-1828";
            RowColletion ds = (ServiceClient.Get<DataSourceDataResponse>(new DataSourceDataRequest { RefId = this.DataSourceId })).Data;
            string _html = string.Empty;

            foreach (EbDataRow cardRow in ds)
            {
                EbCard Card = new EbCard();
                Card.Name = cardRow[0].ToString();
                Card.Label = cardRow[1].ToString();
                Card.ContentHTML = cardRow[2].ToString();
                Card.ImageID = cardRow[3].ToString();
                Card.Buttons.Add(new EbButton { Text = "Select" });
                this.CardCollection.Add(Card);
            }
        }

        public override string GetJsInitFunc()
        {
            return @"
this.Init = function(id)
{
    this.CardCollection.push(new EbObjects.EbCard(id + '_card0'));
    this.CardCollection.push(new EbObjects.EbCard(id + '_card1'));
};";
        }

        public override string GetDesignHtml()
        {
            this.CardCollection.Add(new EbCard());
            this.CardCollection.Add(new EbCard());
            return GetHtml().RemoveCR().DoubleQuoted();
        }

        public override string GetBareHtml()
        {
            string html = @"
                <div id='@name@'class='cards-cont'>"
.Replace("@name@", (this.Name != null) ? this.Name : "@name@");
            foreach (EbCard ec in this.CardCollection)
                html += ec.GetHtml();
            return html + "</div>"
.Replace("@name@", (this.Name != null) ? this.Name : "@name@");
        }

        public override string GetHtml()
        {
            return @"
            <div id='cont_@name@' Ctype='Cards' class='Eb-ctrlContainer' style='@hiddenString'>
                @GetBareHtml@
            </div>"
.Replace("@name@", (this.Name != null) ? this.Name : "@name@")
.Replace("@GetBareHtml@", this.GetBareHtml())
;
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

        public override string GetBareHtml()
        {
            string html = @"
<div id='@name@' class='card-cont'>
    <img class='card-img' src='@ImageID@'/>
    <div class='card-bottom'>
        <div id='@name@Lbl' class='card-label' style='@LabelBackColor  @LabelForeColor font-weight: bold'> @Label@ </div>
        <div class='card-content'>
            @ContentHTML@
        </div>
    </div>
    @ButtonCollection@
</div>"
   .Replace("@ButtonCollection@", this.ButtonsString)
   .Replace("@name@", this.Name)
   .Replace("@ContentHTML@", this.ContentHTML) //"Chat has become the center of the smartphone universe, so it makes sense that bots are being used to deliver information in a convenient and engaging manner. But how do brands or media companies")//
   .Replace("@Label@", this.Label)//"TechCrunch")//
   .Replace("@ImageID@", this.ImageID)//"https://tctechcrunch2011.files.wordpress.com/2016/03/chat-bot.jpg?w=738")//
   ;
            return html;
        }

        public override string GetHtml()
        {
            return GetBareHtml();
        }
    }
}
