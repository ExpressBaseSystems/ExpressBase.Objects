using ExpressBase.Common;
using ExpressBase.Common.Extensions;
using ExpressBase.Common.JsonConverters;
using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using ExpressBase.Common.Structures;
using ExpressBase.Objects.ServiceStack_Artifacts;
using Newtonsoft.Json;
using ServiceStack;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;

namespace ExpressBase.Objects
{
    [EnableInBuilder(BuilderType.BotForm)]
    public class EbStaticCardSet : EbCardSetParent
    {
        public EbStaticCardSet()
        {
            this.CardCollection = new List<EbStaticCard>();
		}
		[EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm)]
		[HideInPropertyGrid]
		public EbDbTypes EbDbType { get { return EbDbTypes.Json; } }

		[OnDeserialized]
        public void OnDeserializedMethod(StreamingContext context)
        {
            this.BareControlHtml = this.GetBareHtml();
            this.ObjType = this.GetType().Name.Substring(2, this.GetType().Name.Length - 2);
        }

        [EnableInBuilder(BuilderType.BotForm)]
        [PropertyEditor(PropertyEditorType.CollectionPropsFrmSrc, "CardFields")]
        public List<EbStaticCard> CardCollection { get; set; }

        [EnableInBuilder(BuilderType.BotForm)]
        [PropertyEditor(PropertyEditorType.String)]
        public string StringEditor { get; set; }

        [EnableInBuilder(BuilderType.BotForm)]
        [PropertyEditor(PropertyEditorType.ScriptEditorCS)]
        public string V_CSEditor { get; set; }

        public override bool isFullViewContol { get => true; set => base.isFullViewContol = value; }

        public override string GetToolHtml()
        {
            return @"<div eb-type='@toolName' class='tool'><i class='fa fa-window-restore'></i>@toolName</div>".Replace("@toolName", this.GetType().Name.Substring(2));
        }

        public override string GetJsInitFunc()
        {
            return @"this.Init = function(id)
					{
						//this.CardCollection.$values.push(new EbObjects.EbCard(id + '_EbCard0'));
						this.CardFields.$values.push(new EbObjects.EbCardImageField(id + '_EbCardImageField0'));
						this.CardFields.$values.push(new EbObjects.EbCardTitleField(id + '_EbCardTitleField0'));
						this.CardFields.$values.push(new EbObjects.EbCardHtmlField(id + '_EbCardHtmlField0'));
						this.CardFields.$values.push(new EbObjects.EbCardNumericField(id + '_EbCardNumericField0'));
						this.CardFields.$values.push(new EbObjects.EbCardTextField(id + '_EbCardTextField0'));
					};";
        }

        public override string GetDesignHtml()
        {
            return @"`<div id=@id><div class='cards-cont'>
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
            return @"<div id='cont_@name@' Ctype='Cards' class='Eb-ctrlContainer' style='@hiddenString'>
						@GetBareHtml@
					</div>"
                        .Replace("@name@", this.Name ?? "@name@")
                        .Replace("@GetBareHtml@", this.GetBareHtml());
        }

        public override string GetBareHtml()
        {
			string html = @"<div id='@name@'><div class='cards-cont'>".Replace("@name@", this.Name ?? "@name@");
			foreach (EbStaticCard card in CardCollection)
			{
				html += @"<div id='@name@' class='card-cont' card-id='@cardid@' style='width:100%;'>".Replace("@name@", card.Name.Trim()).Replace("@cardid@", card.CardId.ToString());
				foreach (EbCardField cardField in this.CardFields)
				{				
					cardField.FieldValue = card.CustomFields[cardField.Name];					
					html += cardField.GetBareHtml();
				}

				html += "<div class='card-btn-cont' style='@BtnDisplay@'><button id='' class='btn btn-default'  data-toggle='tooltip' title='' style='width:100%;'>Select</button></div></div>".Replace("@BtnDisplay@", this.MultiSelect ? "" : "display:none;");
			}
			html += "</div>@SummarizeHtml@  <div class='cards-btn-cont' style='margin-top: 20px;'> <button id='' class='btn btn-default'  data-toggle='tooltip' title='' style='width:100%; box-shadow: 0px 0px 10px #ccc; border-radius: 1.3em 1.3em 1.3em 1.3em;'> @ButtonText@ </button> </div>   </div>"
				.Replace("@SummarizeHtml@", (this.getCartHtml().IsNullOrEmpty() || !this.MultiSelect)? "": this.getCartHtml())
				.Replace("@ButtonText@", this.ButtonText.IsNullOrEmpty()? "Submit" : this.ButtonText);
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
					string colStyle = ((F.SummarizeColumnWidth > 0) ? "width: " + F.SummarizeColumnWidth + "%;" : "") + " white-space: nowrap; overflow: hidden; text-overflow: ellipsis;";
					html += "<th style='" + colStyle + "' title='" + F.Name + "'>" + F.Name + "</th>";
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
    }
    [EnableInBuilder(BuilderType.BotForm)]
    [HideInToolBox]
    public class EbStaticCard : EbCardParent
    {
        [JsonConverter(typeof(DictionaryConverter))]
        [EnableInBuilder(BuilderType.BotForm)]
        [PropertyEditor(PropertyEditorType.DictionaryEditor, "CardFields")]
        public IDictionary<string, object> CustomFields { get; set; }

        public EbStaticCard() {}

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
	
}