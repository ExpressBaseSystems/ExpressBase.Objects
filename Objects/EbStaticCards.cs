using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ExpressBase.Objects
{
    [EnableInBuilder(BuilderType.BotForm)]
	[ShowInToolBox]
    public class EbStaticCardSet : EbCardSetParent
    {
		public override string ToolNameAlias { get { return "Static Cards"; } set { } }

		[EnableInBuilder(BuilderType.BotForm, BuilderType.WebForm)]
        [PropertyEditor(PropertyEditorType.CollectionPropsFrmSrc, "CardFields")]
        public override List<EbCard> CardCollection { get; set; }
		
		public EbStaticCardSet()
		{
			this.CardCollection = new List<EbCard>();
			this.ObjType = this.GetType().Name.Substring(2, this.GetType().Name.Length - 2);
		}

		[OnDeserialized]
		public void OnDeserializedMethod(StreamingContext context)
		{			
			foreach (EbCard Card in this.CardCollection)
			{
				//for getting distinct filter values
				if (this.FilterField?.Name != null && !this.FilterValues.Contains(Card.CustomFields[this.FilterField.Name].ToString()))
				{
					this.FilterValues.Add(Card.CustomFields[this.FilterField.Name].ToString().Trim());
				}
			}

            this.BareControlHtml = this.GetBareHtml();
            this.BareControlHtml4Bot = this.BareControlHtml;
            this.ObjType = this.GetType().Name.Substring(2, this.GetType().Name.Length - 2);
		}		     
    }	
}
