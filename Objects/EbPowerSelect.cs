using ExpressBase.Common;
using ExpressBase.Common.Extensions;
using ExpressBase.Common.LocationNSolution;
using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using ExpressBase.Common.Structures;
using ExpressBase.Security;
using ExpressBase.Objects.Objects.DVRelated;
using ExpressBase.Objects.ServiceStack_Artifacts;
using Newtonsoft.Json;
using ServiceStack;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Common;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using ExpressBase.Common.Constants;

namespace ExpressBase.Objects
{
    public enum DefaultSearchFor
    {
        BeginingWithKeyword = 0,
        EndingWithKeyword = 1,
        ExactMatch = 2,
        Contains = 3,
    }

    [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl)]
    public class EbPowerSelect : EbControlUI
    {

        public EbPowerSelect()
        {
            if (this.Options == null)
            {
                this.Options = new List<EbSimpleSelectOption>();
            }
            if (RenderAsSimpleSelect)
            {
                IsDynamic = true;
            }
            else if (IsInsertable)
                AddButton = new EbButton();
        }

        //public override string SetValueJSfn
        //{
        //    get
        //    {
        //        return @"
        //             this.initializer.setValues(p1, p2);
        //        ";
        //    }
        //    set { }
        //}

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.UserControl)]
        [PropertyGroup(PGConstants.DATA_INSERT)]
        [OnChangeExec(@"
            if (this.IsInsertable === true ){
	            pg.ShowProperty('FormRefId');
            } 
            else {
	            pg.HideProperty('FormRefId');
            }")]
        public bool IsInsertable { get; set; }

        [JsonIgnore]
        public override string IsRequiredOKJSfn
        {
            get
            {
                return @"
                    if(this.RenderAsSimpleSelect){"
                        + JSFnsConstants.SS_IsRequiredOKJSfn +
                    @"}
                    else{
                        let val = this.getValueFromDOM();
                        if(this.MultiSelect){
                            return !(val === '' || val === undefined|| val === null || typeof val === 'number');
                        }
                        else
                            return Number.isInteger(val);
                    }
                ";
            }
            set { }
        }


        //public EbSimpleSelect EbSimpleSelect;


        [JsonIgnore]
        public override string GetDisplayMemberFromDOMJSfn
        {
            get
            {
                return JSFnsConstants.PS_GetDisplayMemberJSfn;
            }
            set { }
        }

        [JsonIgnore]
        public override string GetColumnJSfn
        {
            get
            {
                return @"
if(this.DataVals.R)
    return this.DataVals.R[p1];
else
    return []"
;
            }
            set { }
        }

        [JsonIgnore]
        public override string IsEmptyJSfn { get { return @" return this.initializer.Vobj.valueMembers.length === 0;"; } set { } }


        [JsonIgnore]
        public override string DisableJSfn
        {
            get
            {
                return @"
                    this.__IsDisable = true;
                    if(this.RenderAsSimpleSelect){"
                        + JSFnsConstants.SS_DisableJSfn +
                    @"}
                    else{"
                        + JSFnsConstants.Ctrl_DisableJSfn +
                    @"}
                ";
            }
            set { }
        }

        [JsonIgnore]
        public override string EnableJSfn
        {
            get
            {
                return JSFnsConstants.PS_EnableJSfn;
            }
            set { }
        }

        [JsonIgnore]
        public override string JustSetValueJSfn { get { return JSFnsConstants.PS_JustSetValueJSfn; } set { } }

        [JsonIgnore]
        public override string SetValueJSfn { get { return JSFnsConstants.PS_SetValueJSfn; } set { } }

        [JsonIgnore]
        public override string GetValueFromDOMJSfn
        {
            get
            {
                return @"
                    if(this.RenderAsSimpleSelect){"
                        + JSFnsConstants.EbSimpleSelect_GetValueFromDOMJSfn +
                    @"}
                    else{
                        let val = $('#' + this.EbSid_CtxId).val();
                        return (val === '') ? null : (this.MultiSelect ? val : parseInt(val));
                    }
                ";
            }
            set { }
        }

        [OnDeserialized]
        public void OnDeserializedMethod(StreamingContext context)
        {
            if (this.IsInsertable)
            {
                AddButton = new EbButton()
                {
                    EbSid = EbSid + "_addbtn",
                    EbSid_CtxId = EbSid_CtxId + "_addbtn",
                    Name = Name + "_addbtn",
                    FormRefId = FormRefId,
                    Label = "<i class='fa fa-plus' aria-hidden='true'></i>"
                };
            }
            if (this.Padding == null)
                this.Padding = new UISides() { Bottom = 7, Left = 10, Top = 7, Right = 10 };
            this.BareControlHtml = this.GetBareHtml();
            this.BareControlHtml4Bot = this.BareControlHtml;
            this.ObjType = this.GetType().Name.Substring(2, this.GetType().Name.Length - 2);
        }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl)]
        [HideInPropertyGrid]
        public EbButton AddButton { set; get; }

        public override string SetDisplayMemberJSfn
        {
            get
            {
                return JSFnsConstants.PS_SetDisplayMemberJSfn;
            }
            set { }
        }

        public override string ClearJSfn
        {
            get
            {
                return @"
if (this.initializer)
    this.initializer.clearValues();
else
    console.dev_log(`initializer not found for '${this.Name}'`);
                ";
            }
            set { }
        }



        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl)]
        [PropertyEditor(PropertyEditorType.Expandable)]
        [PropertyGroup(PGConstants.APPEARANCE)]
        [UIproperty]
        [DefaultPropValue(7, 10, 7, 10)]
        [OnChangeUIFunction("Common.INP_PADDING")]
        public UISides Padding { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl)]
        [HideInPropertyGrid]
        public override EbDbTypes EbDbType
        {
            get
            {
                return (this.MultiSelect) ? EbDbTypes.String : EbDbTypes.Decimal;
            }
            set { }
        }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl)]
        [PropertyEditor(PropertyEditorType.ObjectSelector)]
        [OSE_ObjectTypes(EbObjectTypes.iDataReader)]
        [PropertyPriority(98)]
        [PropertyGroup(PGConstants.DATA_SETTINGS)]
        [Alias("Data Reader")]
        public string DataSourceId { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
        [PropertyEditor(PropertyEditorType.ObjectSelector)]
        [PropertyGroup(PGConstants.DATA_INSERT)]
        [OSE_ObjectTypes(EbObjectTypes.iWebForm)]
        [Alias("Form")]
        public string FormRefId { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
        [PropertyEditor(PropertyEditorType.ObjectSelector)]
        [PropertyGroup(PGConstants.DATA)]
        [OSE_ObjectTypes(EbObjectTypes.iWebForm)]
        public string DataImportId { get; set; }

        [EnableInBuilder(BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.WebForm, BuilderType.UserControl)]
        [PropertyEditor(PropertyEditorType.CollectionProp, "Columns", "bVisible")]
        [PropertyGroup(PGConstants.DATA_SETTINGS)]
        //[HideInPropertyGrid]
        public DVColumnCollection Columns { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl)]
        [PropertyEditor(PropertyEditorType.CollectionABCFrmSrc, "Columns")]
        [OnChangeExec(@"
if (this.Columns && this.Columns.$values.length === 0 )
{
pg.MakeReadOnly('DisplayMembers');} else {pg.MakeReadWrite('DisplayMembers');}")]
        [PropertyGroup(PGConstants.DATA_SETTINGS)]
        [PropertyPriority(68)]
        public DVColumnCollection DisplayMembers { get; set; }

        [EnableInBuilder(BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.WebForm, BuilderType.UserControl)]
        [PropertyEditor(PropertyEditorType.CollectionFrmSrc, "Columns", 1)]
        [PropertyPriority(68)]
        [PropertyGroup(PGConstants.DATA_SETTINGS)]
        [OnChangeExec(@"if (this.Columns && this.Columns.$values.length === 0 ){pg.MakeReadOnly('DisplayMember');} else {pg.MakeReadWrite('DisplayMember');}")]
        public DVBaseColumn DisplayMember { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl)]
        [PropertyEditor(PropertyEditorType.CollectionFrmSrc, "Columns", 1)]
        [OnChangeExec(@"if (this.Columns.$values.length === 0 ){pg.MakeReadOnly('ValueMember');} else {pg.MakeReadWrite('ValueMember');}")]
        [PropertyPriority(69)]
        [PropertyGroup(PGConstants.DATA_SETTINGS)]
        public DVBaseColumn ValueMember { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl)]
        [DefaultPropValue("100")]
        [PropertyGroup(PGConstants.APPEARANCE)]
        [Alias("DropdownWidth(%)")]
        public int DropdownWidth { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl)]
        [DefaultPropValue("100")]
        [PropertyGroup(PGConstants.APPEARANCE)]
        public int DropdownHeight { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl)]
        [HideInPropertyGrid]
        public int Value { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl)]
        [HelpText("Specify minimum number of charecters to initiate search")]
        [Category("Search Settings")]
        [PropertyGroup(PGConstants.SEARCH)]
        public int MinSeachLength { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl)]
        [HideInPropertyGrid]
        public string Text { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl)]
        [PropertyGroup(PGConstants.VALIDATIONS)]
        [OnChangeExec(@"
            if (this.MultiSelect === true ){
                pg.MakeReadWrite('MaxLimit');   
                if (this.Required === true ){
                    if(this.MinLimit < 1){
                        pg.setSimpleProperty('MinLimit', 1);
                    }
                    pg.MakeReadWrite('MinLimit');
                }
                else{
                    pg.setSimpleProperty('MinLimit', 0);
                    pg.MakeReadOnly('MinLimit');                 
                }
            } 
            else {
                pg.setSimpleProperty('MaxLimit', 1);
                pg.MakeReadOnly(['MaxLimit','MinLimit']);
                if (this.Required === true ){
                    pg.setSimpleProperty('MinLimit', 1);
                }
                else{
                    pg.setSimpleProperty('MinLimit', 0);
                }
            }")]
        public override bool Required { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl)]
        [PropertyGroup(PGConstants.EXTENDED)]
        [PropertyPriority(65)]
        //[DefaultPropValue("1")]
        [OnChangeExec(@"
            if (this.MultiSelect === true ){
                pg.MakeReadWrite('MaxLimit');   
                if (this.Required === true ){
                    if(this.MinLimit < 1){
                        pg.setSimpleProperty('MinLimit', 1);
                    }
                    pg.MakeReadWrite('MinLimit');
                }
                else{
                    pg.setSimpleProperty('MinLimit', 0);
                    pg.MakeReadOnly('MinLimit');                 
                }
                if(this.MaxLimit === 1)
                    pg.setSimpleProperty('MaxLimit', 0);
                    
            } 
            else {
                pg.setSimpleProperty('MaxLimit', 1);
                pg.MakeReadOnly(['MaxLimit','MinLimit']);
                if (this.Required === true ){
                    pg.setSimpleProperty('MinLimit', 1);
                }
                else{
                    pg.setSimpleProperty('MinLimit', 0);
                }
                if(this.MaxLimit !== 1)
                    pg.setSimpleProperty('MaxLimit', 1);
            }")]
        public bool MultiSelect { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl)]
        //[DefaultPropValue(1)]
        [PropertyGroup(PGConstants.EXTENDED)]
        public int MaxLimit { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl)]
        [PropertyGroup(PGConstants.EXTENDED)]
        public int MinLimit { get; set; }


        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl)]
        [Alias("Search Method")]
        [PropertyGroup(PGConstants.SEARCH)]
        [HelpText("Select Search Method - StartsWith, EndsWith, Contains or Exact Match")]
        public DefaultSearchFor DefaultSearchFor { get; set; }

        //[EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl)]
        //[PropertyGroup("Behavior")]
        //public int NumberOfFields { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl)]
        [PropertyEditor(PropertyEditorType.Boolean)]
        [PropertyGroup(PGConstants.EXTENDED)]
        [DefaultPropValue("true")]
        [OnChangeExec(@"

if(this.RenderAsSimpleSelect == true){ //SS
    if(this.IsDynamic == false){// SS static
	    pg.ShowProperty('Options');
	    pg.HideProperty('ValueMember');
	    pg.HideProperty('DisplayMember'); 
    }
    else{// SS dynamic
	    pg.HideProperty('Options');
	    pg.ShowProperty('ValueMember');
	    pg.ShowProperty('DisplayMember');
    }
}
else// PS
{
	pg.ShowProperty('ValueMember');
	pg.ShowProperty('DisplayMembers');
	pg.ShowProperty('Columns');
	pg.HideProperty('DisplayMember');
	pg.HideProperty('Options');
}
")]
        public bool IsDynamic { get; set; }

        //[EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm)]
        public int[] values { get; set; }

        //[HideInPropertyGrid]
        //[EnableInBuilder(BuilderType.BotForm)]
        //public override bool IsReadOnly { get => this.IsDisable; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.FilterDialog)]
        [PropertyGroup(PGConstants.CORE)]
        [PropertyPriority(50)]
        [OnChangeExec(@"
if(this.RenderAsSimpleSelect == true)// SS
{ 
	pg.ShowProperty('DisplayMember');
	pg.HideProperty('DisplayMembers');
	pg.HideProperty('Columns');
	pg.ShowProperty('IsDynamic');    
}
else// PS
{
	pg.HideProperty('DisplayMember');
	pg.ShowProperty('DisplayMembers');
	pg.ShowProperty('Columns');
	pg.HideProperty('Options');
	pg.HideProperty('IsDynamic');
}
")]
        public bool RenderAsSimpleSelect { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl)]
        //[OnChangeExec(@"
        //    if(this.MultiSelect === true){
        //        pg.ShowProperty('IsSearchable');
        //        pg.ShowProperty('MaxLimit');
        //        pg.ShowProperty('MinLimit');
        //    }
        //    else{
        //        pg.HideProperty('IsSearchable');
        //        pg.HideProperty('MaxLimit');
        //        pg.HideProperty('MinLimit');
        //    }")]
        [PropertyGroup(PGConstants.SEARCH)]
        public bool IsSearchable { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl)]
        [PropertyGroup(PGConstants.APPEARANCE)]
        public BootStrapClass BootStrapStyle { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl)]
        [PropertyGroup(PGConstants.HELP)]
        [DefaultPropValue(" - select - ")]
        public string PlaceHolder { get; set; }

        private string _optionHtml = string.Empty;
        [JsonIgnore]
        public string OptionHtml
        {
            get
            {
                if (_optionHtml.Equals(string.Empty))
                {
                    _optionHtml = string.Empty;
                    if (!this.IsDynamic)
                    {
                        foreach (EbSimpleSelectOption opt in this.Options)
                        {
                            _optionHtml += string.Format("<option  value='{0}'>{1}</option>", opt.Value, opt.DisplayName);
                        }
                    }
                }
                return _optionHtml;
            }
            set { }
        }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl)]
        [PropertyEditor(PropertyEditorType.Collection)]
        [Alias("Options")]
        public List<EbSimpleSelectOption> Options { get; set; }

        private string VueSelectcode
        {
            get
            {
                int noOfFileds = this.DisplayMembers.Count;
                int i = 0;
                string rs = "<div id='@ebsid@Wraper' class='search-wraper' data-toggle='tooltip' title='@tooltipText@'>";
                foreach (DVBaseColumn obj in this.DisplayMembers)
                {
                    rs += @"
<div class='search-block' @perWidth@>
    <div class='input-group'>
        <v-select maped-column='$$' column-type='@type@' id='@ebsid@$$' style='width:{3}px;' 
            multiple
            v-model='displayMembers[""$$""]'
            :on-change='updateCk'
            placeholder = '@sTitle@'>
        </v-select>
        <span class='input-group-addon ps-srch' @border-r$$> <i id='@ebsid@TglBtn' class='fa  fa-search' aria-hidden='true'></i> </span>
    </div>
</div>"
.Replace("$$", obj.Name ?? "")
.Replace("@ebsid@", this.EbSid_CtxId)
.Replace("@type@", ((int)obj.Type).ToString())
.Replace("@sTitle@", obj.sTitle.ToString())
.Replace("@perWidth@", "style='width:" + ((obj.Width == 0) ? (((int)(100 / noOfFileds)).ToString()) : obj.Width.ToString()) + "%'")
.Replace("@border-r" + i, (i != noOfFileds - 1) ? "style='border-radius: 0px;'" : "");
                    i++;
                }
                return rs + "</div>";
            }
        }

        [HideInPropertyGrid]
        [JsonIgnore]
        public override string ToolIconHtml { get { return "<i class='fa fa-search-plus'></i>"; } set { } }

        //public override string GetToolHtml()
        //{
        //    return @"<div eb-type='@toolName' class='tool'><i class='fa fa-search-plus'></i> @toolName</div>".Replace("@toolName", this.GetType().Name.Substring(2));
        //}

        public override string GetDesignHtml()
        {

            string EbCtrlHTML = HtmlConstants.CONTROL_WRAPER_HTML4WEB
                .Replace("@barehtml@", @"
         <div style='display: inline-block; width: 100%; margin-right: -4px;'>
            <div class='input-group'>
               <div class='dropdown v-select searchable' id='acmasterid0'>
                  <div type='button' class='dropdown-toggle clearfix' style='border-top-left-radius: 5px; border-bottom-left-radius: 5px;'>
                     <input debounce='0' ui-inp type='search'  readonly  placeholder='label0' class='form-control' id='acmaster1_xid' style='width: 100%; background-color: #fff;'> <i role='presentation' class='open-indicator' style='display: none;'></i> 
                     <div class='spinner' style='display: none;'>Loading...</div>
                  </div>
               </div>
               <span class='input-group-addon' style='border-radius: 0px;'><i id='acmasteridTglBtn' aria-hidden='true' class='fa  fa-search'></i></span>
            </div>").RemoveCR().DoubleQuoted();
            //return GetHtml().RemoveCR().GraveAccentQuoted();
            return ReplacePropsInHTML(EbCtrlHTML);
        }

        public override string GetHtml()
        {
            return GetHtmlHelper(RenderMode.User);
        }

        public override string DesignHtml4Bot { get => @"
	<div class='combo-wrap' data-toggle='tooltip' title='' data-original-title=''>
		<div style='display: inline-block; width: 100%; margin-right: -4px;'>
			<div class='input-group'>
				<div class='dropdown v-select searchable' id='acmasterid0'>
					<div type='button' class='dropdown-toggle clearfix' style='border-top-left-radius: 5px; border-bottom-left-radius: 5px;'>
						<input debounce='0' type='search'  readonly  placeholder='label0' class='form-control' id='acmaster1_xid' style='width: 100%; background-color: #fff;'> <i role='presentation' class='open-indicator' style='display: none;'></i> 
						<div class='spinner' style='display: none;'>Loading...</div>
					</div>
				</div>
				<span class='input-group-addon'><i id='acmasteridTglBtn' aria-hidden='true' class='fa  fa-search'></i></span>
			</div>
		</div>
	</div>"; set => base.DesignHtml4Bot = value; }

        public override string GetHtml4Bot()
        {
            return ReplacePropsInHTML((HtmlConstants.CONTROL_WRAPER_HTML4BOT).Replace("@barehtml@", DesignHtml4Bot));
        }

        public string SSGetBareHtml(string ebsid)
        {
            return @"
        <select id='@ebsid@' ui-inp class='selectpicker' title='@PlaceHolder@' @selOpts@ @MaxLimit@ @multiple@ @IsSearchable@ name='@ebsid@' @bootStrapStyle@ data-ebtype='@data-ebtype@' style='width: 100%;'>
            @-sel-@
            @options@
        </select>"
   .Replace("@ebsid@", String.IsNullOrEmpty(this.EbSid_CtxId) ? "@ebsid@" : ebsid)
   .Replace("@name@", this.Name)
   .Replace("@HelpText@", this.HelpText)

   .Replace("@multiple@", this.MultiSelect ? "multiple" : "")
   .Replace("@MaxLimit@", MultiSelect ? "data-max-options='" + (!MultiSelect ? 1 : MaxLimit) + "'" : string.Empty)
   .Replace("@IsSearchable@", MultiSelect ? "data-live-search='" + this.IsSearchable + "'" : string.Empty)
   .Replace("@selOpts@", MultiSelect ? "data-actions-box='true'" : string.Empty)
   .Replace("@bootStrapStyle@", "data-style='btn-" + this.BootStrapStyle.ToString() + "'")

   .Replace("@PlaceHolder@", (PlaceHolder ?? string.Empty))
   .Replace("@options@", this.OptionHtml)
   .Replace("@-sel-@", this.MultiSelect ? string.Empty : "<option selected value='-1' style='color: #6f6f6f;'>" + (PlaceHolder.IsNullOrEmpty() || PlaceHolder.Trim() == string.Empty ? "--" : PlaceHolder) + "</option>")
   .Replace("@data-ebtype@", "16");
        }


        public override string GetBareHtml()
        {
            if (this.RenderAsSimpleSelect)
            {
                return this.SSGetBareHtml("@ebsid@"); // temp
            }

            if (this.DisplayMembers != null)
            {
                return @"
<div id='@ebsid@Container' class='ps-cont'  role='form' data-toggle='validator' style='width:100%;'>
    <input type='hidden' ui-inp name='@ebsid@Hidden4val' data-ebtype='8' id='@ebsid@'/>
    @VueSelectCode
    <center class='pow-center'>
        <div id='@ebsid@DDdiv' v-show='DDstate' class='DDdiv expand-transition'  style='width:@DDwidth%;'> 
            @addbtn@
            <table id='@ebsid@tbl' tabindex='1000' style='width:100%' class='table table-bordered'></table>
        </div>
    </center>
</div>"
    .Replace("@VueSelectCode", this.VueSelectcode)
    .Replace("@name@", this.Name)
    .Replace("@ebsid@", this.EbSid_CtxId)
    .Replace("@width", 900.ToString())//this.Width.ToString())
    .Replace("@perWidth", (this.DisplayMembers.Count != 0) ? (900 / this.DisplayMembers.Count).ToString() : 900.ToString())
    .Replace("@DDwidth", (this.DropdownWidth == 0) ? "100" : this.DropdownWidth.ToString())
    .Replace("@addbtn@", this.IsInsertable ? string.Concat("<div class='ps-addbtn-cont'>", this.AddButton.GetBareHtml(), "</div>") : string.Empty)
    .Replace("@tooltipText@", this.ToolTipText ?? string.Empty);
            }
            else
                return string.Empty;
        }

        public void InitFromDataBase_SS(JsonServiceClient ServiceClient)
        {
            //this.DataSourceId = "eb_roby_dev-eb_roby_dev-2-1015-1739";
            string _html = string.Empty;
            var result = ServiceClient.Get<FDDataResponse>(new FDDataRequest { RefId = this.DataSourceId });

            foreach (EbDataRow option in result.Data)
            {
                string val = option[this.ValueMember.Data].ToString();
                string dispName = option[this.DisplayMember.Data].ToString();
                this.Options.Add(new EbSimpleSelectOption { Value = val, DisplayName = dispName });

                _html += string.Format("<option value='{0}'>{1}</option>", val, dispName);
            }
            _optionHtml = _html;
            this.OptionHtml = _html;
        }

        public string GetBareHtml(string ebsid)// temp
        {
            if (this.RenderAsSimpleSelect)
            {
                return this.SSGetBareHtml(ebsid);
            }

            if (this.DisplayMembers != null)
            {
                return @"
<div id='@ebsid@Container'  role='form' data-toggle='validator' style='width:100%;'>
    <input type='hidden' ui-inp name='@ebsid@Hidden4val' data-ebtype='8' id='@ebsid@'/>
    @VueSelectCode
    <center class='pow-center'>
        <div id='@ebsid@DDdiv' v-show='DDstate' class='DDdiv expand-transition'  style='width:@DDwidth%;'> 
            @addbtn@
            <table id='@ebsid@tbl' tabindex='1000' style='width:100%' class='table table-bordered'></table>
        </div>
    </center>
</div>"
    .Replace("@VueSelectCode", this.VueSelectcode)
    .Replace("@name@", this.Name)
    .Replace("@ebsid@", ebsid)
    .Replace("@width", 900.ToString())//this.Width.ToString())
    .Replace("@perWidth", (this.DisplayMembers.Count != 0) ? (900 / this.DisplayMembers.Count).ToString() : 900.ToString())
    .Replace("@DDwidth", (this.DropdownWidth == 0) ? "100" : this.DropdownWidth.ToString())
    .Replace("@addbtn@", this.IsInsertable ? string.Concat("<div class='ps-addbtn-cont'>", this.AddButton.GetBareHtml(), "</div>") : string.Empty)
    .Replace("@tooltipText@", this.ToolTipText ?? string.Empty);
            }
            else
                return string.Empty;
        }

        private string GetHtmlHelper(RenderMode mode)
        {
            string EbCtrlHTML = HtmlConstants.CONTROL_WRAPER_HTML4WEB
.Replace("@Label@ ", ((this.Label != null) ? this.Label : "@Label@ "))
.Replace("@tooltipText@", this.ToolTipText ?? string.Empty);

            return ReplacePropsInHTML(EbCtrlHTML);
        }

        private string GetSql(Service service)
        {
            EbDataReader dr = service.Redis.Get<EbDataReader>(this.DataSourceId);
            if (dr == null)
            {
                var result = service.Gateway.Send<EbObjectParticularVersionResponse>(new EbObjectParticularVersionRequest { RefId = this.DataSourceId });
                dr = EbSerializers.Json_Deserialize(result.Data[0].Json);
                service.Redis.Set<EbDataReader>(this.DataSourceId, dr);
            }

            string Sql = dr.Sql.Trim();
            if (Sql.LastIndexOf(";") == Sql.Length - 1)
                Sql = Sql.Substring(0, Sql.Length - 1);

            return Sql;
        }

        //INCOMPLETE// to get the entire columns(vm+dm+others) in ps query
        public string GetSelectQuery(IDatabase DataDB, Service service, string Col, string Tbl = null, string _id = null, string masterTbl = null)
        {
            string Sql = this.GetSql(service);

            if (Tbl == null || _id == null)// prefill mode
            {
                string s = "";
                if (DataDB.Vendor == DatabaseVendors.MYSQL)
                {
                    s = string.Format(@"SELECT __A.* FROM ({0}) __A 
                                    WHERE FIND_IN_SET(__A.{1}, '{2}');",
                                                        Sql, this.ValueMember.Name, Col);
                }
                else
                {
                    s = string.Format(@"SELECT DISTINCT __A.* FROM ({0}) __A 
                                    WHERE __A.{1} = ANY(STRING_TO_ARRAY('{2}'::TEXT, ',')::INT[]);",
                                                        Sql, this.ValueMember.Name, Col);
                }
                return s;
            }
            else
            {
                // normal mode
                string s = "";
                if (DataDB.Vendor == DatabaseVendors.MYSQL)
                {
                    s = string.Format(@"SELECT __A.* FROM ({0}) __A, {1} __B
                                    WHERE FIND_IN_SET(__A.{2}, __B.{3}) AND __B.{4} = :{5}_id;",
                                         Sql, Tbl, this.ValueMember.Name, Col, _id, masterTbl);
                }
                else
                {
                    s = string.Format(@"SELECT DISTINCT __A.* FROM ({0}) __A, {1} __B
                                    WHERE __A.{2} = ANY(STRING_TO_ARRAY(__B.{3}::TEXT, ',')::INT[]) AND __B.{4} = :{5}_id;",
                                        Sql, Tbl, this.ValueMember.Name, Col, _id, masterTbl);
                }
                return s;
            }
        }

        //for grid lines
        public string GetSelectQuery123(IDatabase DataDB, Service service, string table, string column, string parentTbl, string masterTbl)
        {
            string psSql = this.GetSql(service);
            string s = $@"SELECT __A.* FROM ({psSql}) __A, {table} __B
                            WHERE __A.{this.ValueMember.Name} = ANY(STRING_TO_ARRAY(__B.{column}::TEXT, ',')::INT[]) 
                            AND __B.{parentTbl}_id = @{parentTbl}_id AND __B.{masterTbl}_id = @{masterTbl}_id; ";
            return s;
        }

        //to get vm+dm only for audit trail
        public string GetDisplayMembersQuery(IDatabase DataDB, Service service, string vms)
        {
            string Sql = this.GetSql(service);
            string vm = this.ValueMember.Name;
            string dm = string.Join(',', this.DisplayMembers.Select(e => e.Name));

            string s = "";
            if (DataDB.Vendor == DatabaseVendors.MYSQL)
            {
                s = string.Format(@"SELECT {0}, {1} FROM ({2}) __A
                                        WHERE FIND_IN_SET(__A.{0}, '{3}');",
                            vm, dm, Sql, vms);
            }
            else
            {
                s = string.Format(@"SELECT {0}, {1} FROM ({2}) __A
                                        WHERE __A.{0} = ANY(STRING_TO_ARRAY('{3}'::TEXT, ',')::INT[]);",
                                            vm, dm, Sql, vms);
            }

            return s;
        }

        public override SingleColumn GetSingleColumn(User UserObj, Eb_Solution SoluObj, object Value)
        {
            return new SingleColumn()
            {
                Name = this.Name,
                Type = (int)this.EbDbType,
                Value = Value,
                Control = this,
                ObjType = this.ObjType,
                F = string.Empty,
                D = new Dictionary<int, Dictionary<string, string>>(),
                R = new Dictionary<string, List<dynamic>>()
            };
        }
    }
}