using ExpressBase.Common.Extensions;
using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
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
            this.CardCollection.Add(new EbCard());
            this.CardCollection.Add(new EbCard());
        }

        [OnDeserialized]
        public void OnDeserializedMethod(StreamingContext context)
        {
            this.BareControlHtml = this.GetBareHtml();
            this.Type = this.GetType().Name.Substring(2, this.GetType().Name.Length - 2);
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
            return GetHtml().RemoveCR().DoubleQuoted();
        }

        public override string GetBareHtml()
        {
            string html = @"
                <div class='cards-cont'>";
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
                string html = "<div class='card-btn-cont'>";

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
   .Replace("@ContentHTML@", "Chat has become the center of the smartphone universe, so it makes sense that bots are being used to deliver information in a convenient and engaging manner. But how do brands or media companies")//this.ContentHTML)
   .Replace("@Label@", "TechCrunch")//this.Label)
   .Replace("@ImageID@", "https://tctechcrunch2011.files.wordpress.com/2016/03/chat-bot.jpg?w=738")//"this.ImageID")
   ;
            return html;
        }

        public override string GetHtml()
        {
            return GetBareHtml();
        }
    }
}
