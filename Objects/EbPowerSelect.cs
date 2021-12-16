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
using ServiceStack.Redis;
using ExpressBase.Common.Data;
using System.Text.RegularExpressions;
using ExpressBase.Objects.WebFormRelated;
using System.Net;

namespace ExpressBase.Objects
{
    public enum PsSearchOperators
    {
        Contains = 0,
        StartsWith = 1,
        EndsWith = 2,
        Equals = 3
    }

    [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl)]
    public class EbPowerSelect : EbControlUI, IEbPowerSelect, IEbDataReaderControl
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
            //else if (IsInsertable)
            //AddButton = new EbButton();
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

        [EnableInBuilder(BuilderType.WebForm, BuilderType.UserControl)]
        [PropertyGroup("Data import")]
        [OnChangeExec(@"
        if(this.IsImportFromApi){
            pg.ShowProperty('ImportApiUrl');
            pg.ShowProperty('ImportApiMethod');
            pg.ShowProperty('ImportApiHeaders');
            pg.ShowProperty('ImportApiParams');
            pg.HideProperty('DataImportId');
        }
        else{
            pg.HideProperty('ImportApiUrl');
            pg.HideProperty('ImportApiMethod');
            pg.HideProperty('ImportApiHeaders');
            pg.HideProperty('ImportApiParams');
            pg.ShowProperty('DataImportId');
        }")]
        public bool IsImportFromApi { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.UserControl)]
        [PropertyGroup("Data import")]
        [HideForUser]
        public string ImportApiUrl { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.UserControl)]
        [PropertyGroup("Data import")]
        [HideForUser]
        public ApiMethods ImportApiMethod { set; get; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.UserControl)]
        [PropertyEditor(PropertyEditorType.Collection)]
        [PropertyGroup("Data import")]
        [HideForUser]
        public List<ApiRequestHeader> ImportApiHeaders { get; set; }

        [EnableInBuilder(BuilderType.WebForm)]
        [PropertyGroup("Data import")]
        [Alias("Import api parameters")]
        [PropertyEditor(PropertyEditorType.Collection)]
        public List<EbCtrlApiParamAbstract> ImportApiParams { get; set; }

        [HideInPropertyGrid]
        [EnableInBuilder(BuilderType.WebForm, BuilderType.UserControl)]
        public List<Param> ImportParamsList { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.UserControl)]
        [PropertyEditor(PropertyEditorType.ObjectSelector)]
        [PropertyGroup("Data import")]
        [OSE_ObjectTypes(EbObjectTypes.iWebForm)]
        [Alias("Import Form")]
        public string DataImportId { get; set; }


        [EnableInBuilder(BuilderType.WebForm, BuilderType.UserControl, BuilderType.BotForm, BuilderType.FilterDialog)]
        [PropertyGroup(PGConstants.CORE)]
        [HideForUser]
        [OnChangeExec(@"
        if(this.IsDataFromApi){
            pg.ShowGroup('Api');
            pg.HideProperty('DataSourceId');
        }
        else{
            pg.HideGroup('Api');
            pg.ShowProperty('DataSourceId');
        }")]
        public bool IsDataFromApi { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.UserControl, BuilderType.BotForm, BuilderType.FilterDialog)]
        [PropertyGroup("Api")]
        [HideForUser]
        [OnChangeExec(@"
if (this.Columns && this.Columns.$values.length === 0 )
{
pg.MakeReadOnly('DisplayMembers');} else {pg.MakeReadWrite('DisplayMembers');}")]
        public string Url { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.UserControl, BuilderType.BotForm, BuilderType.FilterDialog)]
        [PropertyGroup("Api")]
        [HideForUser]
        public ApiMethods Method { set; get; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.UserControl, BuilderType.BotForm, BuilderType.FilterDialog)]
        [PropertyEditor(PropertyEditorType.Collection)]
        [PropertyGroup("Api")]
        [HideForUser]
        public List<ApiRequestHeader> Headers { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.UserControl, BuilderType.BotForm, BuilderType.FilterDialog)]
        [PropertyGroup("Api")]
        [Alias("Data api parameters")]
        [PropertyEditor(PropertyEditorType.Collection)]
        public List<EbCtrlApiParamAbstract> DataApiParams { get; set; }

        //[EnableInBuilder(BuilderType.WebForm, BuilderType.UserControl, BuilderType.BotForm, BuilderType.FilterDialog)]
        //[PropertyEditor(PropertyEditorType.CollectionFrmSrc, "return [...getFlatCtrlObjs(commonO.Current_obj)];")]
        //[PropertyGroup("Api")]
        //[Alias("Parameter controls")]
        //public List<EbControl> ApiParamCtrls { get; set; }

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
                        let val = this.getValue();
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
    return this.MultiSelect ? this.DataVals.R[p1] :  this.DataVals.R[p1][0];
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
                        + JSFnsConstants.PS_DisableJSfn +
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
        public override string SetDisplayMemberJSfn { get { return JSFnsConstants.PS_SetDisplayMemberJSfn; } set { } }

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
                        val =  (val === '') ? null : (this.MultiSelect ? val : parseInt(val));
                        return ((val === null && this.__isFDcontrol) ? -1 :val)
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
                //AddButton = new EbButton()
                //{
                //    EbSid = EbSid + "_addbtn",
                //    EbSid_CtxId = EbSid_CtxId + "_addbtn",
                //    Name = Name + "_addbtn",
                //    FormRefId = FormRefId,
                //    Label = "<i class='fa fa-plus' aria-hidden='true'></i>"
                //};
            }
            if (this.Padding == null)
                this.Padding = new UISides() { Bottom = 7, Left = 10, Top = 7, Right = 10 };
            this.BareControlHtml = this.GetBareHtml();
            this.BareControlHtml4Bot = this.BareControlHtml;
            this.ObjType = this.GetType().Name.Substring(2, this.GetType().Name.Length - 2);
        }

        //[EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl)]
        //[HideInPropertyGrid]
        //public EbButton AddButton { set; get; }

        public override string JustSetValueJSfn
        {
            get
            {
                return JSFnsConstants.PS_JustSetValueJSfn;
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

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl)]
        [PropertyEditor(PropertyEditorType.ObjectSelector)]
        [PropertyGroup(PGConstants.DATA_INSERT)]
        [OSE_ObjectTypes(EbObjectTypes.iWebForm)]
        [Alias("Form")]
        public string FormRefId { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.UserControl, BuilderType.BotForm, BuilderType.FilterDialog)]
        [PropertyGroup(PGConstants.DATA_INSERT)]
        public bool IsInsertable { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.UserControl, BuilderType.BotForm, BuilderType.FilterDialog)]
        [PropertyGroup(PGConstants.DATA_INSERT)]
        public bool OpenInNewTab { get; set; }

        [EnableInBuilder(BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.WebForm, BuilderType.UserControl)]
        [PropertyEditor(PropertyEditorType.CollectionProp, "Columns", "bVisible", true)]
        [PropertyGroup(PGConstants.DATA_SETTINGS)]
        public DVColumnCollection Columns { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl)]
        [PropertyEditor(PropertyEditorType.CollectionABCFrmSrc, "Columns", true)]
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
        [PropertyGroup(PGConstants.BEHAVIOR)]
        public int DropDownItemLimit { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl)]
        [PropertyGroup(PGConstants.BEHAVIOR)]
        [Alias("Search result limit")]
        public int SearchLimit { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.UserControl)]
        [PropertyGroup(PGConstants.CORE)]
        [DefaultPropValue("true")]
        [OnChangeExec(@"if (this.IsPreload){pg.MakeReadOnly('SearchLimit');} else {pg.MakeReadWrite('SearchLimit');}")]
        [Alias("Preload items")]
        public bool IsPreload { get; set; }

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
        public int MinSearchLength { get; set; }

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
        [PropertyGroup(PGConstants.SEARCH)]
        [HelpText("Select Search Method - StartsWith, EndsWith, Contains or Exact Match")]
        public PsSearchOperators SearchOperator { get; set; }

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

        [HideInPropertyGrid]
        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl)]
        public List<Param> ParamsList { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm)]
        public override bool Index { get; set; }

        [EnableInBuilder(BuilderType.WebForm)]
        [PropertyGroup(PGConstants.VALUE)]
        [Alias("Refresh Dependency")]
        public bool RefreshDpndcy { get; set; }

        private string VueSelectcode
        {
            get
            {
                int noOfFileds = this.DisplayMembers.Count;
                int i = 0;
                int btnWdth = this.IsInsertable ? 25 : 0;
                btnWdth += this.RefreshDpndcy ? 25 : 0;
                string rs = @"
<div id='@ebsid@srch_Wraper' class='search-wraper' data-toggle='tooltip' @addBtnRealtedWidthChange@ title='@tooltipText@'>
    <div class='input-group'>
        <div class='search-block-wraper'>"
.Replace("@addBtnRealtedWidthChange@", btnWdth > 0 ? $"style='width: calc( 100% - {btnWdth}px)'" : string.Empty);
                foreach (DVBaseColumn obj in this.DisplayMembers)
                {
                    rs += @"
            <div class='search-block' @perWidth@>
                <v-select maped-column='$$' column-type='@type@' id='@ebsid@$$' style='width:{3}px;' 
                    multiple
                    v-model='displayMembers[""$$""]'
                    :on-change='updateCk'
                    placeholder = '@sTitle@'>
                </v-select>
            </div>"
.Replace("$$", obj.Name ?? "")
.Replace("@ebsid@", this.EbSid_CtxId)
.Replace("@type@", ((int)obj.Type).ToString())
.Replace("@sTitle@", obj.sTitle.ToString())
.Replace("@perWidth@", "style='width:" + ((obj.Width == 0) ? (((int)(100 / noOfFileds)).ToString()) : obj.Width.ToString()) + "%'")
.Replace("@border-r" + i, (i != noOfFileds - 1) ? "style='border-radius: 0px;'" : "");
                    i++;
                }
                return rs + @"
        </div>
        <span class='input-group-addon ps-srch' @border-r$$> <i id='@ebsid@TglBtn' class='fa  fa-search' aria-hidden='true'></i> </span>
    </div>
</div>";
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
<div id='@ebsid@Container' class='ps-cont'  role='form' data-toggle='validator' style='width:100%;' form-link='@form-link@'>    
    @addbtn@
    @rfshbtn@
    <input type='hidden' ui-inp name='@ebsid@Hidden4val' data-ebtype='8' id='@ebsid@'/>
    @VueSelectCode
    <center class='pow-center'>
        <div id='@ebsid@DDdiv' v-show='DDstate' class='DDdiv expand-transition'  style='width:@DDwidth%; display: none;'> 
            <div class='DDclose'><i class='fa fa-close' aria-hidden='true'></i></div>
            <div class='DDrefresh'><i class='fa fa-refresh' aria-hidden='true'></i></div>
            <table id='@ebsid@tbl' tabindex='1000' style='width:100%' class='table table-bordered'></table>
        </div>
    </center>
    <div id='@ebsid@_pb' class='ps-pb'></div>
</div>"
    .Replace("@VueSelectCode", this.VueSelectcode)
    .Replace("@name@", this.Name)
    .Replace("@ebsid@", this.EbSid_CtxId)
    .Replace("@width", 900.ToString())//this.Width.ToString())
    .Replace("@perWidth", (this.DisplayMembers.Count != 0) ? (900 / this.DisplayMembers.Count).ToString() : 900.ToString())
    .Replace("@DDwidth", (this.DropdownWidth == 0) ? "100" : this.DropdownWidth.ToString())
    .Replace("@addbtn@", this.IsInsertable ? string.Concat("<div class='ps-addbtn' id='" + EbSid_CtxId + "_addbtn'><i class='fa fa-plus' aria-hidden='true'></i></div>") : string.Empty)
    .Replace("@rfshbtn@", this.RefreshDpndcy ? string.Concat("<div class='ps-rfshbtn' id='" + EbSid_CtxId + $"_rfshbtn' {(this.IsInsertable ? "style='right: 25px;'" : "")}><i class='fa fa-refresh' aria-hidden='true'></i></div>") : string.Empty)
    .Replace("@tooltipText@", this.ToolTipText ?? string.Empty)
    .Replace("@form-link@", string.IsNullOrWhiteSpace(this.FormRefId) ? "false" : "true");
            }
            else
                return string.Empty;
        }

        public void InitFromDataBase_SS(JsonServiceClient ServiceClient)
        {
            //this.DataSourceId = "eb_roby_dev-eb_roby_dev-2-1015-1739";
            if (this.IsDataFromApi)
                return;

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
<div id='@ebsid@Container' class='ps-cont'  role='form' data-toggle='validator' style='width:100%;'>    
    @addbtn@
    <input type='hidden' ui-inp name='@ebsid@Hidden4val' data-ebtype='8' id='@ebsid@'/>
    @VueSelectCode
    <center class='pow-center'>
        <div id='@ebsid@DDdiv' v-show='DDstate' class='DDdiv expand-transition'  style='width:@DDwidth%;'> 
            <div class='DDclose'><i class='fa fa-close' aria-hidden='true'></i></div>
            <div class='DDrefresh'><i class='fa fa-refresh' aria-hidden='true'></i></div>
            <table id='@ebsid@tbl' tabindex='1000' style='width:100%' class='table table-bordered'></table>
        </div>
    </center>
    <div id='@ebsid@_pb' class='ps-pb'></div>
</div>"
    .Replace("@VueSelectCode", this.VueSelectcode)
    .Replace("@name@", this.Name)
    .Replace("@ebsid@", ebsid)
    .Replace("@width", 900.ToString())//this.Width.ToString())
    .Replace("@perWidth", (this.DisplayMembers.Count != 0) ? (900 / this.DisplayMembers.Count).ToString() : 900.ToString())
    .Replace("@DDwidth", (this.DropdownWidth == 0) ? "100" : this.DropdownWidth.ToString())
    .Replace("@addbtn@", this.IsInsertable ? string.Concat("<div class='ps-addbtn' id='" + ebsid + "_addbtn'><i class='fa fa-plus' aria-hidden='true'></i></div>") : string.Empty)
    .Replace("@tooltipText@", this.ToolTipText ?? string.Empty);
            }
            else
                return string.Empty;
        }

        public void UpdateParamsMeta(Service Service, IRedisClient Redis)
        {
            if (!this.IsDataFromApi)
            {
                EbDataReader DrObj = EbFormHelper.GetEbObject<EbDataReader>(this.DataSourceId, null, Redis, Service);
                this.ParamsList = DrObj.GetParams(Redis as RedisClient);
            }
            else
            {
                this.ParamsList = new List<Param>();
            }
        }

        public void FetchParamsMeta(IServiceClient ServiceClient, IRedisClient Redis, EbControl[] Allctrls, Service service)
        {
            if (this.IsDataFromApi)
            {
                this.ParamsList = new List<Param>();
                if (this.DataApiParams?.Count > 0)
                {
                    foreach (EbCtrlApiParam p in this.DataApiParams)
                    {
                        if (p.IsStaticParam)
                        {
                            if (string.IsNullOrEmpty(p.Name))
                                throw new FormException("Data Api Parameters name is empty for " + this.Label);
                            if (string.IsNullOrEmpty(p.Value))
                                throw new FormException("Data Api Parameters value is empty for " + this.Label);
                            this.ParamsList.Add(new Param() { Name = p.Name, Type = Convert.ToString((int)EbDbTypes.String), Value = p.Value });
                        }
                        else
                        {
                            if (string.IsNullOrEmpty(p.ControlName))
                                throw new FormException("Data Api Parameters control name is empty for " + this.Label);
                            EbControl paramCtrl = Allctrls.FirstOrDefault(e => e.Name == p.ControlName);
                            if (paramCtrl == null)
                                throw new FormException($"Invalid control name '{p.ControlName}' for 'data api parameters' of power select control({this.Label}).");
                            this.ParamsList.Add(new Param() { Name = p.ControlName, Type = Convert.ToString((int)paramCtrl.EbDbType) });
                        }
                    }
                }
            }
            else
            {
                EbDataReader DrObj = EbFormHelper.GetEbObject<EbDataReader>(this.DataSourceId, ServiceClient, Redis, service);
                this.ParamsList = DrObj.GetParams(Redis as RedisClient);
            }

            if (this.IsImportFromApi)
            {
                if (string.IsNullOrEmpty(this.ImportApiUrl))
                    throw new FormException("Set Import Api Url for " + this.Label);
                //string regex = @"^http(s)?://([\w-]+.)+[\w-]+(/[\w-./?%&=])?$";
                //if (!Regex.IsMatch(this.ImportApiUrl, regex))
                //    throw new FormException("Set a valid Import Api Url for " + this.Label);

                this.ImportParamsList = new List<Param>();
                if (this.ImportApiParams?.Count > 0)
                {
                    foreach (EbCtrlApiParam p in this.ImportApiParams)
                    {
                        if (p.IsStaticParam)
                        {
                            if (string.IsNullOrEmpty(p.Name))
                                throw new FormException("Import Api Parameters name is empty for " + this.Label);
                            if (string.IsNullOrEmpty(p.Value))
                                throw new FormException("Import Api Parameters value is empty for " + this.Label);
                            this.ImportParamsList.Add(new Param() { Name = p.Name, Type = Convert.ToString((int)EbDbTypes.String), Value = p.Value });
                        }
                        else
                        {
                            if (string.IsNullOrEmpty(p.ControlName))
                                throw new FormException("Import Api Parameters control name is empty for " + this.Label);
                            EbControl paramCtrl = Allctrls.FirstOrDefault(e => e.Name == p.ControlName);
                            if (paramCtrl == null)
                                throw new FormException($"Invalid control name '{p.ControlName}' for 'import api parameters' of power select control({this.Label}).");
                            this.ImportParamsList.Add(new Param() { Name = p.ControlName, Type = Convert.ToString((int)paramCtrl.EbDbType) });
                        }
                    }
                }
                else
                    throw new FormException("Set Import Api Parameters for " + this);
            }
        }

        public string GetSql(Service service)
        {
            EbDataReader dr = EbFormHelper.GetEbObject<EbDataReader>(this.DataSourceId, null, service.Redis, service);
            string Sql = dr.Sql.Trim();
            if (Sql.LastIndexOf(";") == Sql.Length - 1)
                Sql = Sql.Substring(0, Sql.Length - 1);

            return Sql;
        }

        //INCOMPLETE// to get the entire columns(vm+dm+others) in ps query
        public string GetSelectQuery(IDatabase DataDB, Service service, string Col, string Tbl = null, string _id = null, string masterTbl = null)
        {
            return EbPowerSelect.GetSelectQuery(this, DataDB, service, Col, Tbl, _id, masterTbl, false);
        }

        public static string GetSelectQuery(EbPowerSelect _this, IDatabase DataDB, Service service, string Col, string Tbl, string _id, string masterTbl, bool IsDgPs)
        {
            if (_this.IsDataFromApi)
                return string.Empty;

            string Sql = _this.GetSql(service);

            if (Tbl == null || _id == null)// prefill mode
            {
                string s = "";
                if (DataDB.Vendor == DatabaseVendors.MYSQL)
                {
                    s = string.Format(@"SELECT __A.* FROM ({0}) __A 
                                    WHERE FIND_IN_SET(__A.{1}, '{2}');",
                                                        Sql, _this.ValueMember.Name, Col);
                }
                else
                {
                    s = string.Format(@"SELECT DISTINCT __A.* FROM ({0}) __A 
                                    WHERE __A.{1} = ANY(STRING_TO_ARRAY('{2}'::TEXT, ',')::INT[]);",
                                                        Sql, _this.ValueMember.Name, Col);
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
                                         Sql, Tbl, _this.ValueMember.Name, Col, _id, masterTbl);
                }
                else
                {
                    if (_this.ParamsList == null)
                        throw new FormException($"Invalid ParamsList in '{_this.Label ?? _this.Name}'. Contact Admin", (int)HttpStatusCode.InternalServerError, "Save object in dev side", "EbPowerSelect -> GetSelectQuery");

                    if (IsDgPs && _this.ParamsList.Exists(e => e.Name == _this.Name))
                    {
                        if (Sql.Contains(":" + _this.Name))
                            Sql = Sql.Replace(":" + _this.Name, $"ANY(STRING_TO_ARRAY(:{_this.Name}{FormConstants.__ebedt}, ',')::INT[])");
                        if (Sql.Contains("@" + _this.Name))
                            Sql = Sql.Replace("@" + _this.Name, $"ANY(STRING_TO_ARRAY(@{_this.Name}{FormConstants.__ebedt}, ',')::INT[])");
                    }

                    s = string.Format(@"SELECT DISTINCT __A.* FROM ({0}) __A, {1} __B
                                    WHERE __A.{2} = ANY(STRING_TO_ARRAY(__B.{3}::TEXT, ',')::INT[]) AND __B.{4} = :{5}_id;",
                                        Sql, Tbl, _this.ValueMember.Name, Col, _id, masterTbl);
                    //Tbl = "invtrans_lines", Col = "itemmaster1_id", _id = "invtrans_id", master = "invtrans"
                }
                return s;
            }
        }

        //for grid lines
        public string GetSelectQuery123(IDatabase DataDB, Service service, string table, string column, string parentTbl, string masterTbl)
        {
            if (this.IsDataFromApi)
                return string.Empty;

            string psSql = this.GetSql(service);
            string s = $@"SELECT __A.* FROM ({psSql}) __A, {table} __B
                            WHERE __A.{this.ValueMember.Name} = ANY(STRING_TO_ARRAY(__B.{column}::TEXT, ',')::INT[]) 
                            AND __B.{parentTbl}_id = @{parentTbl}_id AND __B.{masterTbl}_id = @{masterTbl}_id; ";
            return s;
        }

        //to get vm+dm only for audit trail
        public string GetDisplayMembersQuery(IDatabase DataDB, Service service, string vms, List<DbParameter> param)
        {
            if (this.IsDataFromApi)
                return string.Empty;

            EbDataReader dr = EbFormHelper.GetEbObject<EbDataReader>(this.DataSourceId, null, service.Redis, service);
            string Sql = dr.Sql.Trim();
            if (Sql.LastIndexOf(";") == Sql.Length - 1)
                Sql = Sql.Substring(0, Sql.Length - 1);

            foreach (Param _P in this.ParamsList)
            {
                if (!param.Exists(e => e.ParameterName == _P.Name))
                {
                    if (_P.Name == this.Name)
                    {
                        if (Sql.Contains(":" + this.Name))
                            Sql = Sql.Replace(":" + this.Name, $"ANY(STRING_TO_ARRAY('{vms}'::TEXT, ',')::INT[])");
                        if (Sql.Contains("@" + this.Name))
                            Sql = Sql.Replace("@" + this.Name, $"ANY(STRING_TO_ARRAY('{vms}'::TEXT, ',')::INT[])");
                    }
                    else
                        param.Add(DataDB.GetNewParameter(_P.Name, (EbDbTypes)Convert.ToInt32(_P.Type), _P.ValueTo));
                }
            }

            string vm = this.ValueMember.Name;
            string dm;
            if (this.RenderAsSimpleSelect)
                dm = this.DisplayMember.Name;
            else
                dm = string.Join(',', this.DisplayMembers.Select(e => e.Name));

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

        public override bool ParameterizeControl(ParameterizeCtrl_Params args, string crudContext)
        {
            return EbPowerSelect.ParameterizeControl(this, args, false, crudContext);
        }

        public static bool ParameterizeControl(dynamic _this, ParameterizeCtrl_Params args, bool randomize, string crudContext)
        {
            EbControl _ctrl = _this as EbControl;
            if (_ctrl.BypassParameterization && args.cField.Value == null)
                throw new Exception($"Unable to proceed/bypass with value '{args.cField.Value}' for {_ctrl.Name}");

            string paramName = randomize ? (args.cField.Name + "_" + args.i) : (args.cField.Name + crudContext);
            string _sv = Convert.ToString(args.cField.Value);
            if (args.cField.Value == null || _sv == string.Empty)
            {
                var p = args.DataDB.GetNewParameter(paramName, (EbDbTypes)args.cField.Type);
                p.Value = DBNull.Value;
                args.param.Add(p);
            }
            else if (!_ctrl.BypassParameterization)
            {
                bool throwException = false;
                if (_this.MultiSelect)
                {
                    string[] arr = _sv.Split(',');
                    if (arr.Length != arr.Select(e => int.TryParse(e, out int t)).Count())
                        throwException = true;
                }
                else
                {
                    if (!int.TryParse(_sv, out int _iv))
                        throwException = true;
                }
                if (throwException)
                    throw new FormException($"Invalid value({_sv}) found for PowerSelect: {_this.Name}", (int)HttpStatusCode.InternalServerError, $"Unable to parse '{_sv}' as numeric value for {_this.Name}", "From EbPowerSelect.ParameterizeControl()");
                args.param.Add(args.DataDB.GetNewParameter(paramName, (EbDbTypes)args.cField.Type, args.cField.Value));
            }
            if (args.ins)
            {
                args._cols += args.cField.Name + CharConstants.COMMA + CharConstants.SPACE;
                if (_ctrl.BypassParameterization)
                    args._vals += Convert.ToString(args.cField.Value) + CharConstants.COMMA + CharConstants.SPACE;
                else
                    args._vals += CharConstants.AT + paramName + CharConstants.COMMA + CharConstants.SPACE;
            }
            else
            {
                if (_ctrl.BypassParameterization)
                    args._colvals += args.cField.Name + CharConstants.EQUALS + Convert.ToString(args.cField.Value) + CharConstants.COMMA + CharConstants.SPACE;
                else
                    args._colvals += args.cField.Name + CharConstants.EQUALS + CharConstants.AT + paramName + CharConstants.COMMA + CharConstants.SPACE;
            }
            args.i++;
            return true;
        }

        public override SingleColumn GetSingleColumn(User UserObj, Eb_Solution SoluObj, object Value, bool Default)
        {
            return EbPowerSelect.GetSingleColumn(this, UserObj, SoluObj, Value, Default);
        }

        public static SingleColumn GetSingleColumn(dynamic _this, User UserObj, Eb_Solution SoluObj, object Value, bool Default)
        {
            string _sv = Convert.ToString(Value);
            if (Value != null)
            {
                if (_this.MultiSelect)
                    Value = _sv;
                else if (int.TryParse(_sv, out int _iv))
                    Value = _iv;
                else
                    throw new FormException($"Invalid value({_sv}) found for PowerSelect: {_this.Name}", (int)HttpStatusCode.InternalServerError, $"Unable to parse '{_sv}' as numeric value for {_this.Name}", "From EbPowerSelect.GetSingleColumn()");
            }
            return new SingleColumn()
            {
                Name = _this.Name,
                Type = (int)_this.EbDbType,
                Value = Value,
                Control = _this,
                ObjType = _this.ObjType,
                F = string.Empty,
                D = new Dictionary<int, Dictionary<string, string>>(),
                R = new Dictionary<string, List<object>>()
            };
        }
    }

    public abstract class EbCtrlApiParamAbstract { }

    [UsedWithTopObjectParent(typeof(EbObject))]
    [EnableInBuilder(BuilderType.WebForm)]
    [Alias("ApiParameter")]
    public class EbCtrlApiParam : EbCtrlApiParamAbstract
    {
        public EbCtrlApiParam() { }

        [EnableInBuilder(BuilderType.WebForm)]
        [HideInPropertyGrid]
        public string DisplayName { get; set; }

        [OnChangeExec(@"
        if(this.IsStaticParam){
            pg.ShowProperty('Value');
            pg.ShowProperty('Name');
            pg.HideProperty('ControlName');
        }
        else{
            pg.HideProperty('Value');
            pg.HideProperty('Name');
            pg.ShowProperty('ControlName');
        }")]
        [EnableInBuilder(BuilderType.WebForm)]
        [Alias("Is static parameter")]
        public bool IsStaticParam { get; set; }

        [EnableInBuilder(BuilderType.WebForm)]
        public string ControlName { get; set; }

        [EnableInBuilder(BuilderType.WebForm)]
        public string Name { get; set; }

        [EnableInBuilder(BuilderType.WebForm)]
        public string Value { get; set; }
    }
}