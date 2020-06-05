using ExpressBase.Common;
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
using System.Runtime.Serialization;
using System.Text;

namespace ExpressBase.Objects
{
    [EnableInBuilder(BuilderType.BotForm)]
    [ShowInToolBox]
    public class EbDynamicCardSet : EbCardSetParent
    {
        public override string ToolNameAlias { get { return "Dynamic Cards"; } set { } }

        [EnableInBuilder(BuilderType.BotForm, BuilderType.WebForm)]
        [HideInPropertyGrid]
        public override List<EbCard> CardCollection { get; set; }

        [EnableInBuilder(BuilderType.BotForm, BuilderType.WebForm)]
        [OSE_ObjectTypes(EbObjectTypes.iDataReader)]
        [PropertyEditor(PropertyEditorType.ObjectSelector)]
        public string DataSourceId { get; set; }

        [EnableInBuilder(BuilderType.BotForm, BuilderType.WebForm)]
        [HideInPropertyGrid]
        public DVColumnCollection Columns { get; set; }

        [EnableInBuilder(BuilderType.BotForm, BuilderType.WebForm)]
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
        }

        public void InitFromDataBase(JsonServiceClient ServiceClient)
        {
            RowColletion ds = (ServiceClient.Get<DataSourceDataResponse>(new DataSourceDataRequest { RefId = this.DataSourceId, Start = 0, Length = 1000 })).Data;

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
    [HideInToolBox]
    public class EbCardSetParent : EbControlUI
    {
        public EbCardSetParent()
        {
            this.SelectedCards = new List<int>();
            this.CardFields = new List<EbCardField>();

            this.FilterValues = new List<string>();
        }

        public override string UIchangeFns
        {
            get
            {
                return @"EbCardSetParent = {
                UpdateCrdFlds : function(elementId, props) {
    try {
        let $ctrl = $('#cont_' + elementId).find('.ctrl-wraper').children();
        let $cardcont = $ctrl.find('.card-cont');
        $cardcont.empty();
        let cFlds = props.CardFields.$values;
        for (let i = 0; i < cFlds.length; i++) {
            if (!cFlds[i].DesignHtml) {
                cFlds[i] = new EbObjects['Eb' + cFlds[i].ObjType](cFlds[i].EbSid, cFlds[i]);
            }
            $cardcont.append(cFlds[i].DesignHtml);
        }
    }
    catch (e) {
        console.log(e);
    }
                }
            }";
            }
        }

        public List<int> SelectedCards { get; set; }

        public bool IsSummaryRequired { get; set; }//////////////////////////////////// need rethink

        public virtual List<EbCard> CardCollection { get; set; }

        [EnableInBuilder(BuilderType.BotForm, BuilderType.WebForm)]
        public string TableName { get; set; }

        [EnableInBuilder(BuilderType.BotForm, BuilderType.WebForm)]
        [HideInPropertyGrid]
        public override EbDbTypes EbDbType { get { return EbDbTypes.String; } }

        [EnableInBuilder(BuilderType.BotForm, BuilderType.WebForm)]
        [PropertyEditor(PropertyEditorType.Collection)]
        [OnChangeUIFunction("EbCardSetParent.UpdateCrdFlds")]
        public List<EbCardField> CardFields { get; set; }

        [EnableInBuilder(BuilderType.BotForm, BuilderType.WebForm)]
        [PropertyEditor(PropertyEditorType.Boolean)]
        [OnChangeExec(@"if(this.MultiSelect === true){pg.ShowProperty('SummaryTitle');}
		else{pg.HideProperty('SummaryTitle');}")]
        public bool MultiSelect { get; set; }

        [EnableInBuilder(BuilderType.BotForm, BuilderType.WebForm)]
        public string SummaryTitle { get; set; }

        [EnableInBuilder(BuilderType.BotForm, BuilderType.WebForm)]
        public string ButtonText { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
        [HideInPropertyGrid]
        public override bool IsFullViewContol { get => true; set => base.IsFullViewContol = value; }

        [EnableInBuilder(BuilderType.BotForm, BuilderType.WebForm)]
        [PropertyEditor(PropertyEditorType.DDfromDictProp, "CardFields", 1)]
        public EbControl FilterField { get; set; }

        public List<string> FilterValues { get; set; }

        [EnableInBuilder(BuilderType.BotForm, BuilderType.WebForm)]
        [PropertyEditor(PropertyEditorType.DDfromDictProp, "CardFields", 1)]
        public EbControl SearchField { get; set; }

        [HideInPropertyGrid]
        public override bool IsReadOnly
        {
            get
            {
                foreach (EbCardField field in this.CardFields)
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
            string html = @"<div id='@ebsid@' class='Eb-ctrlContainer'>
								<div class='card-outer-cont'>
									@HeaderHtml@ 
									<div class='cards-cont'>"
                                        .Replace("@ebsid@", this.EbSid ?? "@ebsid@")
                                        .Replace("@HeaderHtml@", this.getHeaderHtml());

            if (CardCollection != null)
            {
                foreach (EbCard card in CardCollection)
                {
                    html += @"<div id='@ebsid@' class='card-cont' card-id='@cardid@' filter-value='@FilterValue@' search-value='@SearchValue@' style='width:100%;'>"
                                    .Replace("@ebsid@", card.EbSid ?? "@ebsid@")
                                    .Replace("@cardid@", card.CardId.ToString())
                                    .Replace("@FilterValue@", this.FilterField?.Name == null ? "" : card.CustomFields[this.FilterField.Name].ToString())
                                    .Replace("@SearchValue@", this.SearchField?.Name == null ? "" : card.CustomFields[this.SearchField.Name].ToString());
                    foreach (EbCardField cardField in this.CardFields)
                    {
                        cardField.FieldValue = card.CustomFields.ContainsKey(cardField.Name) ? card.CustomFields[cardField.Name] : null;
                        html += cardField.GetBareHtml();
                    }

                    html += @"<div class='card-selbtn-cont' style='@BtnDisplay@'>
                                <button id='' class='btn btn-default btn-sm'  data-toggle='tooltip' title=''>Select</button>
                            </div>
                        </div>".Replace("@BtnDisplay@", (this.MultiSelect) ? "" : "display:none !important;");
                }
            }
            html += @"	</div>@SummarizeHtml@  <div class='cards-btn-cont' style='margin-top: 20px;'> <button id='' class='btn btn-default ctrl-submit-btn'  data-toggle='tooltip' title=''> @ButtonText@ </button> </div> </div>
					</div>"
                .Replace("@SummarizeHtml@", (this.getCartHtml().IsNullOrEmpty() || !this.MultiSelect) ? "" : this.getCartHtml())
                .Replace("@ButtonText@", this.ButtonText.IsNullOrEmpty() ? (this.IsReadOnly ? "OK" : "Submit") : this.ButtonText);
            return html;
        }

        public string getCartHtml()
        {
            this.IsSummaryRequired = false;
            int tcols = 1;
            string html = @"<div class='card-summary-cont'><div class='card_sumry_tle' ><b> @Summary@ </b></div>
							<table class='table card-summary-table' style='table-layout: fixed; margin-bottom: 0px;'>
								<thead style='font-size:12px;'><tr>".Replace("@Summary@", this.SummaryTitle.IsNullOrEmpty() ? "Summary" : this.SummaryTitle);
            foreach (EbCardField F in this.CardFields)
            {
                if (F.Summarize)
                {
                    string tempst = F.Label.IsNullOrEmpty() ? F.Name : F.Label;
                    string colStyle = ((F.SummarizeColumnWidth > 0) ? "width: " + F.SummarizeColumnWidth + "%;" : "") + ((F is EbCardNumericField) ? "text-align: right;" : "") + " white-space: nowrap; overflow: hidden; text-overflow: ellipsis;";
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
										<input type='text' class='card-head-search-box form-control' placeholder='Search' title='Search'/>            
										<i class='fa fa-search card-head-search-icon' aria-hidden='true'></i>
									</div>";
            string fhtml = string.Empty;
            if (this.FilterValues.Count != 0)
            {
                fhtml += @"<div class='card-head-filterdiv'>
								<select class='card-head-filter-box form-control select-picker'> <option value='All'> All </option>";
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
            get
            {
                string fieldHtml = "";
                if (this.CardFields?.Count > 0)
                {
                    fieldHtml += EbCardFieldsDesignHtml4Bot.GetCardDesign(this.CardFields);
                }
                else
                {
                    fieldHtml += EbCardFieldsDesignHtml4Bot.GetCardDesign();
                }

                return @"
<div id = @ebsid@>
	<div class='card-header-cont'> 
		<div class='card-head-cardno'> 1 of 1 </div>
		<div class='card-head-searchdiv'>
			<input type='text' class='card-head-search-box form-control' placeholder='Search' title='Search'/>            
			<i class='fa fa-search card-head-search-icon' aria-hidden='true'></i>
		</div>
		<div class='card-head-filterdiv'>
			<select class='card-head-filter-box form-control'> <option value='All'> All </option> </select> 
			<i class='fa fa-filter card-head-filter-icon' aria-hidden='true'></i>
		</div>
	</div>
	<div class='cards-cont'>
		<div class='card-cont' style='width: 100%;'>			
			@fieldHtml@
			<div class='card-selbtn-cont'><button class='btn btn-default' disabled>Select</button></div>
		</div>		
	</div>
	<div class='card-summary-cont' style='background-color: #eee;'><div class='card_sumry_tle'><b> @Summary@ </b></div>
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
	<div class='cards-btn-cont' style='margin-top: 20px;'> <button id='' class='btn btn-default ctrl-submit-btn'  data-toggle='tooltip' title='' disabled> @ButtonText@ </button> </div>
</div>"
.Replace("@ebsid@", this.EbSid ?? "null_ebsid")
.Replace("@fieldHtml@", fieldHtml)
.Replace("@ButtonText@", string.IsNullOrEmpty(this.ButtonText) ? "Submit" : this.ButtonText)
.Replace("@Summary@", string.IsNullOrEmpty(this.SummaryTitle) ? "Summary" : this.SummaryTitle);
            }
        }

        public override string GetDesignHtml()
        {
            return @"`<div id=@id class='Eb-ctrlContainer'><div class='cards-cont'>
						<div class='card-cont' style='width: 100%; min-height: 100px; box-shadow: 0px 0px 20px #ccc; border-radius: 1.3em;'>
							<div class='card-selbtn-cont'><button class='btn btn-default' disabled>Select</button></div>
						</div>
						<div class='card-summary-cont' style='box-shadow: 0px 0px 20px #ccc; border-radius: 0; margin: 20px -4px 0 6px;'><div class='card_sumry_tle' ><b> Summary </b></div>
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
            return @"<div id='cont_@ebsid@' Ctype='Cards' class='Eb-ctrlContainer' eb-hidden='@isHidden@'>
						@GetBareHtml@
					</div>"
                        .Replace("@ebsid@", this.EbSid ?? "@ebsid@")
                        .Replace("@GetBareHtml@", this.GetBareHtml());
        }



        //public override string GetDesignHtml()
        //{
        //	return @"`<div id=@id class='Eb-ctrlContainer'><div class='cards-cont'>
        //				<div class='card-cont' style='width: 100%; min-height: 100px; box-shadow: 0px 0px 20px #ccc; border-radius: 1.3em;'>
        //					<div class='card-selbtn-cont'><button class='btn btn-default' style='width:100%;' disabled>Select</button></div>
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


	//------------------------------------------------CARD ------------------------------------------------

	[EnableInBuilder(BuilderType.BotForm)]
    [HideInToolBox]
    public class EbCard : EbControl
    {
        [EnableInBuilder(BuilderType.BotForm, BuilderType.WebForm)]
        [PropertyEditor(PropertyEditorType.Number)]
        public int CardId { get; set; }

        [JsonConverter(typeof(DictionaryConverter))]
        [EnableInBuilder(BuilderType.BotForm, BuilderType.WebForm)]
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
                string html = @"<div id='@ebsid@' class='card-cont' card-id='@cardid@' style='width:100%;'>".Replace("@ebsid@", this.EbSid ?? "@ebsid@").Replace("@cardid@", this.CardId.ToString());
                //foreach (EbCardField CardField in this.Fields)
                //{
                //    html += CardField.GetBareHtml();
                //}
                html += "<div class='card-selbtn-cont'>" + " <button id='' class='btn btn-default'  data-toggle='tooltip' title=''>Select</button>" + "</div></div>";
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
        [EnableInBuilder(BuilderType.BotForm, BuilderType.WebForm)]
        [HideInPropertyGrid]
        public DVColumnCollection Columns { get; set; }

        public virtual object FieldValue { get; set; }

        [EnableInBuilder(BuilderType.BotForm, BuilderType.WebForm)]
        [PropertyEditor(PropertyEditorType.CollectionFrmSrc, "Columns", 1)]
        [OnChangeExec(@"if (this.Columns.$values.length === 0 ){pg.MakeReadOnly('DbFieldMap');} else {pg.MakeReadWrite('DbFieldMap');}")]
        public DVBaseColumn DbFieldMap { get; set; }

        [EnableInBuilder(BuilderType.BotForm, BuilderType.WebForm)]
        [OnChangeExec(@"if(this.Summarize === true){pg.ShowProperty('SummarizeColumnWidth');}
		else{pg.HideProperty('SummarizeColumnWidth');}")]
        public bool Summarize { get; set; }

        [EnableInBuilder(BuilderType.BotForm, BuilderType.WebForm)]
        public int SummarizeColumnWidth { get; set; }

        [EnableInBuilder(BuilderType.BotForm, BuilderType.WebForm)]
        public bool HideInCard { get; set; }

        [EnableInBuilder(BuilderType.BotForm, BuilderType.WebForm)]
        public override bool DoNotPersist { get; set; }

        [EnableInBuilder(BuilderType.BotForm, BuilderType.WebForm)]
        [HideInPropertyGrid]
        public override EbDbTypes EbDbType { get; set; }

        public string DesignHtml { get; set; }

        //[PropertyGroup(PGConstants.APPEARANCE)]
        //[EnableInBuilder(BuilderType.BotForm, BuilderType.WebForm)]
        //[PropertyEditor(PropertyEditorType.FontSelector)]
        //public EbFont Font { get; set; }
    }


    [EnableInBuilder(BuilderType.BotForm)]
    //[PropertyEditor(PropertyEditorType.xxx)]
    [HideInToolBox]
    [Alias("Image")]
    public class EbCardImageField : EbCardField
    {
        [EnableInBuilder(BuilderType.BotForm, BuilderType.WebForm)]
        //[PropertyEditor(PropertyEditorType.ImageSeletor)]
        [Alias("ImageID")]
        [MetaOnly]
        public override object FieldValue { get; set; }

        [HideInPropertyGrid]
        public override string Label { get; set; }

        [EnableInBuilder(BuilderType.BotForm, BuilderType.WebForm)]
        public int HeigthInPixel { get; set; }

        [EnableInBuilder(BuilderType.BotForm, BuilderType.WebForm)]
        [HideInPropertyGrid]
        public override EbDbTypes EbDbType { get { return EbDbTypes.String; } }

        [EnableInBuilder(BuilderType.BotForm, BuilderType.WebForm)]
        public override bool DoNotPersist { get { return true; } }

        public EbCardImageField() { }

        [OnDeserialized]
        public void OnDeserializedMethod(StreamingContext context)
        {
            this.DesignHtml = this.GetDesignHtml();
            this.DesignHtml = this.DesignHtml.Substring(1, this.DesignHtml.Length - 2);
        }

        public override string DesignHtml4Bot
        {
            get => EbCardFieldsDesignHtml4Bot.image;
        }

        public override string GetDesignHtml()
        {
            return @"`<div><img class='card-img' src='../images/image.png' style='width: 100%; height: 200px; opacity: 0.2;'/></div>`";
        }

        public override string GetBareHtml()
        {
            return @"<div style='@divstyle@' class='card-img-cont data-@Name@'><img class='card-img' src='@ImageID@'/></div>"
                .Replace("@ImageID@", (String.IsNullOrEmpty(Convert.ToString(this.FieldValue))) ? "../images/image.png" : this.FieldValue.ToString())
                .Replace("@divstyle@", (this.HeigthInPixel == 0) ? "margin: 10px 0px;" : "height: " + this.HeigthInPixel + "px; ")
                .Replace("@Name@", this.Name ?? "@Name@");
        }
    }

    [EnableInBuilder(BuilderType.BotForm)]
    [HideInToolBox]
    [Alias("Html")]
    public class EbCardHtmlField : EbCardField
    {

        private string _FieldValue { get; set; }


        [EnableInBuilder(BuilderType.BotForm, BuilderType.WebForm)]
        [PropertyEditor(PropertyEditorType.String64)]
        [Alias("ContentHTML")]
        [MetaOnly]
        public override object FieldValue
        {
            get
            {
                if (_FieldValue == null)
                    return null;

                Span<byte> buffer = new Span<byte>(new byte[_FieldValue.Length]);
                bool isBase64 = Convert.TryFromBase64String(_FieldValue, buffer, out int bytesParsed);
                if (isBase64)
                {
                    byte[] data = Convert.FromBase64String(_FieldValue);
                    string decodedString = Encoding.UTF8.GetString(data);
                    return decodedString;
                }
                else
                    return _FieldValue;
            }
            set
            {
                try
                {
                    _FieldValue = (string)value;
                }
                catch (Exception e)
                {
                    _FieldValue = String.Empty;
                }
            }
        }

        [HideInPropertyGrid]
        public override string Label { get; set; }

        [EnableInBuilder(BuilderType.BotForm, BuilderType.WebForm)]
        [HideInPropertyGrid]
        public override EbDbTypes EbDbType { get { return EbDbTypes.String; } }

        [EnableInBuilder(BuilderType.BotForm, BuilderType.WebForm)]
        public override bool DoNotPersist { get { return true; } }

        public EbCardHtmlField()
        {
            FieldValue = string.Empty;
        }

        [OnDeserialized]
        public void OnDeserializedMethod(StreamingContext context)
        {
            this.DesignHtml = this.GetDesignHtml();
            this.DesignHtml = this.DesignHtml.Substring(1, this.DesignHtml.Length - 2);
        }

        public override string DesignHtml4Bot
        {
            get => EbCardFieldsDesignHtml4Bot.contenthtml;
        }

        public override string GetDesignHtml()
        {
            return @"`<div class='card-contenthtml-cont' style='padding:5px; text-align: center; width: 100%; min-height: 50px;'> HTML Content </div>`";
        }

        public override string GetBareHtml()
        {
            return @"<div class='card-contenthtml-cont data-@Name@' style='padding:5px;'> @ContentHTML@ </div>".Replace("@ContentHTML@", (this.FieldValue == null) ? "" : this.FieldValue.ToString()).Replace("@Name@", this.Name ?? "@Name@");
        }
    }

    [EnableInBuilder(BuilderType.BotForm)]
    [HideInToolBox]
    [Alias("Numeric")]
    public class EbCardNumericField : EbCardField
    {

        [EnableInBuilder(BuilderType.BotForm, BuilderType.WebForm)]
        [Alias("Value")]
        [MetaOnly]
        [PropertyEditor(PropertyEditorType.Number)]
        public override object FieldValue { get; set; }


        [EnableInBuilder(BuilderType.BotForm, BuilderType.WebForm)]
        public bool Sum { get; set; }

        [EnableInBuilder(BuilderType.BotForm, BuilderType.WebForm)]
        public override bool IsDisable { get; set; }

        [EnableInBuilder(BuilderType.BotForm, BuilderType.WebForm)]
        [PropertyEditor(PropertyEditorType.Number)]
		[DefaultPropValue("0")]
		[OnChangeExec(@"
		if($(event.target).val() > this.MaximumValue){
			$(event.target).val('0');
			this.MinimumValue = 0 ;
		}
			")]
        public int MinimumValue { get; set; }

        [EnableInBuilder(BuilderType.BotForm, BuilderType.WebForm)]
        [PropertyEditor(PropertyEditorType.Number)]
		[DefaultPropValue("999999")]
		[OnChangeExec(@"
		if($(event.target).val() <= this.MinimumValue){
			$(event.target).val('999999');
			this.MaximumValue = 999999 ;
		}
			")]
        public int MaximumValue { get; set; }

        [EnableInBuilder(BuilderType.BotForm, BuilderType.WebForm)]
        public override string Label { get; set; }

        [EnableInBuilder(BuilderType.BotForm, BuilderType.WebForm)]
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
            get => EbCardFieldsDesignHtml4Bot.numeric;
            set => base.DesignHtml4Bot = value;
        }

        public override string GetDesignHtml()
        {
            return @"`<div class='card-numeric-cont data-@Name@' style='@display@' data-value='@Value@'>
						<div style='display: inline-block; width: 38%;'> <span class='card-inp-title'>Numeric Field </span> </div> 
						<div class='inp-wrap'>
							<button style='padding: 0px; border: none; background-color: transparent; font-size: 14px;' disabled>
								<i class='fa fa-minus' aria-hidden='true' style=' padding: 5px; color: darkblue;'></i>
							</button>
							<div class='cart-inp-wraper'>
								<input class='cart-inp' type='number' readonly>
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
						<div style='display: inline-block; width: 38%;'> <span class='card-inp-title'> @Label@ </span> </div> 
						<div class='inp-wrap'>
							<button limit='@MinValue@' class='card-pls-mns mns'>
								<i class='fa fa-minus' aria-hidden='true'></i>
							</button>
							<div class='cart-inp-wraper'>
								<input class='cart-inp' type='number' value='@Value@' min='@MinValue@' max='@MaxValue@' @ReadOnly@  
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
							<button limit='@MaxValue@' class='card-pls-mns pls'>
								<i class='fa fa-plus' aria-hidden='true'></i>
							</button>
						</div>
					</div>"
                        .Replace("@Value@", (this.FieldValue == null) ? "1" : ((tempvar is Double) ? tempvar.ToString("0.00") : tempvar.ToString()))
                        .Replace("@display@", this.HideInCard ? "display:none;" : "").Replace("@Name@", this.Name ?? "@Name@")
                        .Replace("@Label@", this.Label.IsNullOrEmpty() ? this.Name : this.Label)
                        .Replace("@ReadOnly@", this.IsDisable ? "readonly" : "")
                        .Replace("@PlusMinusDisplay@", this.IsDisable ? "visibility: hidden;" : "display:inline-block;")
                        .Replace("@MinValue@", this.MinimumValue.ToString())
                        .Replace("@MaxValue@", this.MaximumValue.ToString());
        }
    }

    [EnableInBuilder(BuilderType.BotForm)]
    [HideInToolBox]
    [Alias("Text")]
    public class EbCardTextField : EbCardField
    {
        [EnableInBuilder(BuilderType.BotForm, BuilderType.WebForm)]
        [Alias("Text")]
        [MetaOnly]
        //[PropertyEditor(PropertyEditorType.String)]
        public override object FieldValue { get; set; }

        [EnableInBuilder(BuilderType.BotForm, BuilderType.WebForm)]
        public override bool IsDisable { get; set; }

        [EnableInBuilder(BuilderType.BotForm, BuilderType.WebForm)]
        public override string Label { get; set; }

        [EnableInBuilder(BuilderType.BotForm, BuilderType.WebForm)]
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
            get => EbCardFieldsDesignHtml4Bot.text;
        }

        public override string GetDesignHtml()
        {
            return @"`<div class='card-text-cont'>
						<div style='display: inline-block; width: 38%;'> 
							<span class='card-inp-title'>Text Field </span>  
						</div>
						<div class='inp-wrap'>
							<input type='text' value='@Text@' style='text-align: center;' readonly> 
						</div>
					</div>`";
        }

        public override string GetBareHtml()
        {
            return @"<div class='card-text-cont data-@Name@' style='@display@'>
						<div style='display: inline-block; width: 38%;'> 
							<span class='card-inp-title'>@Label@ </span>  
						</div>
						<div class='card-txt-wraper inp-wrap'>
							<input class='card-txt' type='text' value='@Text@' @ReadOnly@> 
						</div>
					</div>"
                    .Replace("@Text@", (this.FieldValue == null) ? "" : this.FieldValue.ToString())
                    .Replace("@display@", this.HideInCard ? "display:none;" : "").Replace("@Name@", this.Name ?? "@Name@")
                    .Replace("@Label@", this.Label.IsNullOrEmpty() ? this.Name : this.Label).Replace("@ReadOnly@", this.IsDisable ? "readonly" : "");
        }
    }

    [EnableInBuilder(BuilderType.BotForm)]
    [HideInToolBox]
    [Alias("Title")]
    public class EbCardTitleField : EbCardField
    {
        [EnableInBuilder(BuilderType.BotForm, BuilderType.WebForm)]
        [Alias("Title")]
        [MetaOnly]
        //[PropertyEditor(PropertyEditorType.String)]
        public override object FieldValue { get; set; }

		[EnableInBuilder(BuilderType.BotForm, BuilderType.WebForm)]
		public override string Label { get; set; }

        [EnableInBuilder(BuilderType.BotForm, BuilderType.WebForm)]
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
            get => EbCardFieldsDesignHtml4Bot.title;
        }

        public override string GetDesignHtml()
        {
            return @"`<div class='card-title-cont' >&nbsp&nbspTitle Field</div>`";
        }

        public override string GetBareHtml()
        {
            return @"<div class='card-title-cont data-@Name@' > &nbsp @Text@ &nbsp <i class='fa fa-check' style='color: green;display: none;' aria-hidden='true'></i></div>"
                    .Replace("@Text@", (this.FieldValue == null) ? "" : this.FieldValue.ToString()).Replace("@Name@", this.Name ?? "@Name@");
        }
    }

    [EnableInBuilder(BuilderType.BotForm)]
    [HideInToolBox]
    [Alias("Location")]
    public class EbCardLocationField : EbCardField
    {
        [EnableInBuilder(BuilderType.BotForm, BuilderType.WebForm)]
        //[PropertyEditor(PropertyEditorType.String)]
        [Alias("Position")]
        [MetaOnly]
        public override object FieldValue { get; set; }

        //[EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.WebForm)]
        //[PropertyEditor(PropertyEditorType.Expandable)]
        //public LatLng Position { get; set; }
        //public LatLng Lat_Long { get; set; }
        //public Decimal Latitude { get; set; }
        //public Decimal Longitude { get; set; }

        [EnableInBuilder(BuilderType.BotForm, BuilderType.WebForm)]
        [HideInPropertyGrid]
        public override EbDbTypes EbDbType { get { return EbDbTypes.String; } }

        [EnableInBuilder(BuilderType.BotForm, BuilderType.WebForm)]
        public override bool DoNotPersist { get { return true; } }

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

        public override string DesignHtml4Bot
        {
            get => EbCardFieldsDesignHtml4Bot.location;
        }

        public override string GetBareHtml()
        {
            return @"	<div id='@ebsid@_Cont' class='card-location-cont' @DataLatLng@>
							<div id='@ebsid@' class='map-div'></div>
						</div>"
                                .Replace("@ebsid@", this.EbSid ?? "@ebsid@")
                                .Replace("@DataLatLng@", this.FieldValue == null || !this.FieldValue.ToString().Contains(",") ? "" : ("data-lat='" + this.FieldValue.ToString().Split(",")[0].Trim() + "' data-lng='" + this.FieldValue.ToString().Split(",")[1].Trim() + "'"));
        }
    }

    public static class EbCardFieldsDesignHtml4Bot
    {
        public const string image = @"<div><img class='card-img' src='../images/image.png' style='width: 100%; height: 200px; opacity: 0.2;'/></div>";

        public const string contenthtml = @"<div class='card-contenthtml-cont' style='padding:5px; text-align: center; width: 100%; min-height: 50px;'> HTML Content </div>";

        public const string numeric = @"<div class='card-numeric-cont data-@Name@'  style='@display@' data-value='@Value@'>
			<div style='display: inline-block; width: 38%;'> <span class='card-inp-title'>Numeric Field </span> </div> 
			<div class='inp-wrap'>
				<button class='card-pls-mns'  disabled>
					<i class='fa fa-minus' aria-hidden='true' style=' padding: 5px; color: darkblue;'></i>
				</button>
				<div class='cart-inp-wraper'>
					<input class='cart-inp' type='number' style='text-align: center; border: none; background: transparent; width: 120px;' value='12345' readonly>
				</div>
				<button class='card-pls-mns'  disabled>
					<i class='fa fa-plus' aria-hidden='true' style=' padding: 5px; color: darkblue;'></i>
				</button>
			</div>
		</div>";

        public const string text = @"<div class='card-text-cont'>
			<div style='display: inline-block; width: 38%;'> 
				<span class='card-inp-title'>Text Field </span>  
			</div>
			<div class='inp-wrap'>
				<input type='text' value='@Text@' style='text-align: center;' readonly> 
			</div>
		</div>";

        public const string title = @"<div class='card-title-cont' style='font-weight: 600; font-size: 20px; padding: 5px;'>&nbsp&nbspTitle Field</div>";

        public const string location = @"<div class='card-location-cont'>
			<div class='map-div' style='position: relative; overflow: hidden;'>
				<img style='width:100%;height: 100%;' src='/images/LocMapImg1.png'>
			</div>
		</div>";

        public static string GetCardDesign()
        {
            return image + title + contenthtml + numeric + text;
        }

        public static string GetCardDesign(List<EbCardField> CardFields)
        {
            string Html = string.Empty;
            foreach (EbCardField field in CardFields)
            {
                if (field is EbCardImageField)
                    Html += image;
                else if (field is EbCardTitleField)
                    Html += title;
                else if (field is EbCardHtmlField)
                    Html += contenthtml;
                else if (field is EbCardNumericField)
                    Html += numeric;
                else if (field is EbCardTextField)
                    Html += text;
                else if (field is EbCardLocationField)
                    Html += location;
            }
            return Html;
        }
    }

}


