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
                        if (Field is EbCardNumericField)
                            Card.FieldValues[Field.Name] = Convert.ToDouble(ds[i][Field.DbFieldMap.ColumnIndex]);
                        else
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
						this.CardFields.$values.push(new EbObjects.EbCardImageField(id + '_EbCardImageField0'));
						this.CardFields.$values.push(new EbObjects.EbCardTitleField(id + '_EbCardTitleField0'));
						this.CardFields.$values.push(new EbObjects.EbCardHtmlField(id + '_EbCardHtmlField0'));
						this.CardFields.$values.push(new EbObjects.EbCardNumericField(id + '_EbCardNumericField0'));
						this.CardFields.$values.push(new EbObjects.EbCardTextField(id + '_EbCardTextField0'));
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
            foreach (EbDynamicCard card in CardCollection)
            {
                //html += Card.GetBareHtml();
                html += @"<div id='@name@' class='card-cont' card-id='@cardid@' style='width:100%;'>".Replace("@name@", card.Name.Trim()).Replace("@cardid@", card.CardId.ToString());
                foreach (EbCardField cardField in this.CardFields)
                {
                    if (cardField.DbFieldMap != null)
                    {
                        cardField.FieldValue = card.FieldValues[cardField.Name].ToString();
                        //    if (cardField is EbCardImageField)
                        //        (cardField as EbCardImageField).FieldValue = card.FieldValues[cardField.Name].ToString();
                        //    else if (cardField is EbCardNumericField)
                        //        (cardField as EbCardNumericField).FieldValue = Convert.ToDouble(card.FieldValues[cardField.Name]);
                        //    else if (cardField is EbCardHtmlField)
                        //        (cardField as EbCardHtmlField).FieldValue = card.FieldValues[cardField.Name].ToString();
                        //    else if (cardField is EbCardTextField)
                        //        (cardField as EbCardTextField).FieldValue = card.FieldValues[cardField.Name].ToString();
                    }
                    html += cardField.GetBareHtml();
                }
                html += "<div class='card-btn-cont'>Hard codel o</div></div>";

            }
            html += "</div>@SummarizeHtml@@ButtonsString@</div>"
                .Replace("@ButtonsString@", "<div class='cards-btn-cont> <button id='' class='btn btn-default'  data-toggle='tooltip' title='' style='width:100%;'>Submit</button></div>")
                .Replace("@SummarizeHtml@", this.getCartHtml() ?? "");
            return html;
        }

        public string getCartHtml()
        {
            this.IsSummaryRequired = false;
            int tcols = 1;
            string html = @"<div class='card-summary-cont'><div style='font-size: 15px; padding:5px 5px 0px 5px; text-align:center;'><b> @Summary@ </b></div>
							<table class='table card-summary-table' style='table-layout: fixed;'>
								<thead style='font-size:12px;'><tr>".Replace("@Summary@", this.SummaryTitle.IsNullOrEmpty() ? "Summary" : this.SummaryTitle);
            foreach (EbCardField F in this.CardFields)
            {
                if (F.Summarize)
                {
					string colStyle = ((F.SummarizeColumnWidth > 0) ? "width: " + F.SummarizeColumnWidth + "%;" : "") + " white-space: nowrap; overflow: hidden; text-overflow: ellipsis;";
					html += "<th style='" + colStyle + "'>" + F.Name + "</th>";
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
                html += "<div class='card-btn-cont'>" + " <button id='' class='btn btn-default'  data-toggle='tooltip' title='' style='width:100%;'>Select</button>" + "</div></div>";
                return html;
            }
            return string.Empty;
        }
    }

    [EnableInBuilder(BuilderType.BotForm)]
    //[HideInToolBox]
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
    public class EbCardParent : EbControl
    {
		[EnableInBuilder(BuilderType.BotForm)]
		[PropertyEditor(PropertyEditorType.Number)]
		public int CardId { get; set; }

        public EbCardParent() { }
    }

    [EnableInBuilder(BuilderType.BotForm)]
    [HideInToolBox]
    public abstract class EbCardField : EbControl
    {
        [EnableInBuilder(BuilderType.BotForm)]
        [HideInPropertyGrid]
        public ColumnColletion Columns { get; set; }

        public virtual dynamic FieldValue { get; set; }

        [EnableInBuilder(BuilderType.BotForm)]
        [PropertyEditor(PropertyEditorType.CollectionFrmSrc, "Columns", 1)]
        [OnChangeExec(@"console.log(100); if (this.Columns.$values.length === 0 ){pg.MakeReadOnly('DbFieldMap');} else {pg.MakeReadWrite('DbFieldMap');}")]
        public EbDataColumn DbFieldMap { get; set; }

        [EnableInBuilder(BuilderType.BotForm)]
        public bool Summarize { get; set; }

		[EnableInBuilder(BuilderType.BotForm)]
		public int SummarizeColumnWidth { get; set; }

		[EnableInBuilder(BuilderType.BotForm)]
        public bool HideInCard { get; set; }

        [EnableInBuilder(BuilderType.BotForm)]
        public bool DoNotPersist { get; set; }

        //[PropertyGroup("Appearance")]
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
        [PropertyEditor(PropertyEditorType.ImageSeletor)]
        [Alias("ImageID")]
		[MetaOnly]
		public override dynamic FieldValue { get; set; }

        public EbCardImageField() { }

        public override string GetDesignHtml()
        {
            return @"`<div><img class='card-img' src='../images/image.png' style='width: 100%; height: 200px; opacity: 0.2;'/></div>`";
        }

        public override string GetBareHtml()
        {
            return @"<img class='card-img' src='@ImageID@'/>".Replace("@ImageID@", (this.FieldValue == null) ? "../images/image.png" : this.FieldValue);
        }
    }

    [EnableInBuilder(BuilderType.BotForm)]
    [HideInToolBox]
    [Alias("Html")]
    public class EbCardHtmlField : EbCardField
    {
        [EnableInBuilder(BuilderType.BotForm)]
        //[PropertyEditor(PropertyEditorType.String)]
        [Alias("ContentHTML")]
		[MetaOnly]
		public override dynamic FieldValue { get; set; }

        public EbCardHtmlField() { }

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
        [PropertyEditor(PropertyEditorType.JS)]
        public string ValueExpression { get; set; }

        [EnableInBuilder(BuilderType.BotForm)]
        public bool Sum { get; set; }

        [EnableInBuilder(BuilderType.BotForm)]
        public override bool ReadOnly { get; set; }

		[EnableInBuilder(BuilderType.BotForm)]
		[PropertyEditor(PropertyEditorType.Number)]
		public int MinimumValue { get; set; }

		[EnableInBuilder(BuilderType.BotForm)]
		[PropertyEditor(PropertyEditorType.Number)]
		public int MaximumValue { get; set; }

		public EbCardNumericField() { }

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
								<input class='removeArrows' type='number' style='text-align: center; border: none; background: transparent; min-width: 125px;' value='@Value@' min='@MinValue@' max='@MaxValue@' @ReadOnly@  
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
						//.Replace("@MinValue@", this.MinimumValue.ToString())
						//.Replace("@MaxValue@", this.MaximumValue.ToString());
						.Replace("@MinValue@", this.MaximumValue.ToString())
						.Replace("@MaxValue@", this.MinimumValue.ToString());			
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
		[PropertyEditor(PropertyEditorType.String)]
		public override dynamic FieldValue { get; set; }

        [EnableInBuilder(BuilderType.BotForm)]
        [PropertyEditor(PropertyEditorType.JS)]
        public string ValueExpression { get; set; }
		
		[EnableInBuilder(BuilderType.BotForm)]
        public override bool ReadOnly { get; set; }

        public EbCardTextField() { }

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
							<input type='text' value='@Text@' style='text-align: center;' @ReadOnly@> 
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
		
		public EbCardTitleField() { }

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

}