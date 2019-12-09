using ExpressBase.Common;
using ExpressBase.Common.Extensions;
using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using ExpressBase.Common.Structures;
using ExpressBase.Data;
using ExpressBase.Objects.Helpers;
using ExpressBase.Objects.Objects.DVRelated;
using ExpressBase.Objects.ServiceStack_Artifacts;
using Newtonsoft.Json;
using ServiceStack;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace ExpressBase.Objects
{

    public enum BootStrapClass
    {
        Default = 0,// 'default' is a key word
        primary = 1,
        info = 2,
        success = 3,
        warning = 4,
        danger = 5
    }

    [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl)]
    public class EbSimpleSelect : EbControlUI
    {

        public EbSimpleSelect()
        {
            this.Options = new List<EbSimpleSelectOption>();
        }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl)]
        [HideInPropertyGrid]
        public override EbDbTypes EbDbType
        {
            get
            {
                return IsDynamic ? ValueMember.Type : EbDbTypes.String;
            }
		}

		[EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl)]
		[DefaultPropValue("100")]
		[PropertyGroup("Appearance")]
		public int DropdownHeight { get; set; }

		public override string SetValueJSfn
        {
            get
            {
                return @" 
                    isContained = false;
                    $('#' + this.EbSid_CtxId + ' option').each(function () {
                        if ($(this).attr('value') == p1) {
                            isContained = true;
                            return false;
                        }
                    });

                    if(!isContained)
                        return;
                    $('#' + this.EbSid_CtxId).selectpicker('val', p1).trigger('change');";
            }
            set { }
        }

        public override string GetValueJSfn
        {
            get
            {
                return @"
                    let val = $('#' + this.EbSid_CtxId).selectpicker('val');
                    val = (val === null) ? '-1' : val.toString();
                    return val;
                ";
            }
            set { }
        }

        public override string IsRequiredOKJSfn
        {
            get
            {
                return @"
                    return !this.isInVisibleInUI ? (!isNaNOrEmpty(this.getValue()) && (this.getValue() !== '-1')) : true;
                ";
            }
            set { }
        }

        public override string GetDisplayMemberJSfn
        {
            get
            {
                return @"
                    return $('#' + this.EbSid_CtxId +' :selected').text();
                ";
            }
            set { }
        }

        public override string DisableJSfn
        {
            get
            {
                return @"
                    return $('#' + this.EbSid_CtxId +'Wraper .dropdown-toggle').attr('disabled', 'disabled').css('pointer-events', 'none').css('background-color', '#f3f3f3');";
            }
            set { }
        }

        public override string EnableJSfn
        {
            get
            {
                return @"
                    return $('#' + this.EbSid_CtxId +'Wraper .dropdown-toggle').prop('disabled',false).css('pointer-events', 'inherit').css('background-color', '#fff');";
            }
            set { }
        }

        [HideInPropertyGrid]
        [JsonIgnore]
        public override string ToolIconHtml { get { return "<i class='fa fa-caret-down'></i>"; } set { } }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl)]
        [OSE_ObjectTypes(EbObjectTypes.iDataReader)]
        [PropertyEditor(PropertyEditorType.ObjectSelector)]
        public string DataSourceId { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl)]
        public BootStrapClass BootStrapStyle { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl)]
        public string PlaceHolder { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl)]
        public bool IsMultiSelect { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl)]
        [OnChangeExec(@"
            if(this.IsMultiSelect === true){
                pg.ShowProperty('IsSearchable');
                pg.ShowProperty('MaxLimit');
                pg.ShowProperty('MinLimit');
            }
		    else{
                pg.HideProperty('IsSearchable');
                pg.HideProperty('MaxLimit');
                pg.HideProperty('MinLimit');
            }")]
        public bool IsSearchable { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl)]
        public int MaxLimit { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl)]
        public int MinLimit { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl)]
        [HideInPropertyGrid]
        public DVColumnCollection Columns { get; set; }

        [EnableInBuilder(BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.WebForm, BuilderType.UserControl)]
        [PropertyEditor(PropertyEditorType.CollectionFrmSrc, "Columns", 1)]
        [OnChangeExec(@"if (this.Columns && this.Columns.$values.length === 0 ){pg.MakeReadOnly('ValueMember');} else {pg.MakeReadWrite('ValueMember');}")]
        public DVBaseColumn ValueMember { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl)]
        [PropertyEditor(PropertyEditorType.Collection)]
        [Alias("Options")]
        public List<EbSimpleSelectOption> Options { get; set; }

        [EnableInBuilder(BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.WebForm, BuilderType.UserControl)]
        [PropertyEditor(PropertyEditorType.CollectionFrmSrc, "Columns", 1)]
        [OnChangeExec(@"if (this.Columns && this.Columns.$values.length === 0 ){pg.MakeReadOnly('DisplayMember');} else {pg.MakeReadWrite('DisplayMember');}")]
        public DVBaseColumn DisplayMember { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl)]
        public int Value { get; set; }

        [HideInPropertyGrid]
        [EnableInBuilder(BuilderType.BotForm)]
        public override bool IsReadOnly { get => this.ReadOnly; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl)]
        [PropertyEditor(PropertyEditorType.Boolean)]
        [OnChangeExec(@"if(this.IsDynamic === true){pg.ShowProperty('DataSourceId');pg.ShowProperty('ValueMember');pg.ShowProperty('DisplayMember');pg.HideProperty('Options');}
		else{pg.HideProperty('DataSourceId');pg.HideProperty('ValueMember');pg.HideProperty('DisplayMember');pg.ShowProperty('Options');}")]
        public bool IsDynamic { get; set; }

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

        //public override string GetToolHtml()
        //{
        //    return @"<div eb-type='@toolName' class='tool'><i class='fa fa-caret-down'></i>  @toolName</div>".Replace("@toolName", this.GetType().Name.Substring(2));
        //}

        public void InitFromDataBase(JsonServiceClient ServiceClient)
        {
            //this.DataSourceId = "eb_roby_dev-eb_roby_dev-2-1015-1739";
            string _html = string.Empty;
            if (this.IsDynamic)
            {
                var result = ServiceClient.Get<FDDataResponse>(new FDDataRequest { RefId = this.DataSourceId });
                foreach (EbDataRow option in result.Data)
                {
                    _html += string.Format("<option value='{0}'>{1}</option>", option[this.ValueMember.Data], option[this.DisplayMember.Data]);
                    //_html += string.Format("<option value='{0}'>{1}</option>", option[0].ToString().Trim(), option[0]);
                }
            }
            _optionHtml = _html;
            this.OptionHtml = _html;
        }


        [OnDeserialized]
        public void OnDeserializedMethod(StreamingContext context)
        {
            this.BareControlHtml = this.GetBareHtml();
            this.BareControlHtml4Bot = this.BareControlHtml;
            this.ObjType = this.GetType().Name.Substring(2, this.GetType().Name.Length - 2);
        }

		public override string GetHtml4Bot()
		{
			return ReplacePropsInHTML(HtmlConstants.CONTROL_WRAPER_HTML4BOT);
		}

		public override string GetDesignHtml()
        {
            //        return @"

            //<div id='cont_@name@' Ctype='SimpleSelect' class='Eb-ctrlContainer' eb-hidden='@isHidden@'>
            //    <div id='@name@' class='btn-group bootstrap-select show-tick' style='width: 100%;'><button type='button' class='btn dropdown-toggle btn-default'><span class='filter-option pull-left'>Simple Select</span>&nbsp;<span class='bs-caret'><span class='caret'></span></span></button></div>
            //</div>".RemoveCR().GraveAccentQuoted();//GetHtmlHelper(RenderMode.Developer).RemoveCR().DoubleQuoted();
            return GetHtml().RemoveCR().DoubleQuoted(); ;
        }

        public override string GetHtml()
        {
            string EbCtrlHTML = HtmlConstants.CONTROL_WRAPER_HTML4WEB
                 .Replace("@LabelForeColor ", "color:" + (LabelForeColor ?? "@LabelForeColor ") + ";")
                 .Replace("@LabelBackColor ", "background-color:" + (LabelBackColor ?? "@LabelBackColor ") + ";");

            return ReplacePropsInHTML(EbCtrlHTML);
        }

        public override string GetBareHtml()
        {
            return @"
        <select id='@ebsid@' ui-inp class='selectpicker' title='@PlaceHolder@' @selOpts@ @MaxLimit@ @multiple@ @IsSearchable@ name='@ebsid@' @bootStrapStyle@ data-ebtype='@data-ebtype@' style='width: 100%;'>
            @-sel-@
            @options@
        </select>"
.Replace("@ebsid@", String.IsNullOrEmpty(this.EbSid_CtxId) ? "@ebsid@" : this.EbSid_CtxId)
.Replace("@name@", this.Name)
.Replace("@HelpText@", this.HelpText)

.Replace("@multiple@", this.IsMultiSelect ? "multiple" : "")
.Replace("@MaxLimit@", IsMultiSelect ? "data-max-options='" + (!IsMultiSelect ? 1 : MaxLimit) + "'" : string.Empty)
.Replace("@IsSearchable@", IsMultiSelect ? "data-live-search='" + this.IsSearchable + "'" : string.Empty)
.Replace("@selOpts@", IsMultiSelect ? "data-actions-box='true'" : string.Empty)
.Replace("@bootStrapStyle@", "data-style='btn-" + this.BootStrapStyle.ToString() + "'")

.Replace("@PlaceHolder@", (PlaceHolder ?? " -- select -- "))
.Replace("@options@", this.OptionHtml)
.Replace("@-sel-@", this.IsMultiSelect ? string.Empty : "<option selected value='-1' style='color: #6f6f6f;'> -- select -- </option>")
.Replace("@data-ebtype@", "16");
        }
    }

    [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl)]
    [HideInToolBox]
    [UsedWithTopObjectParent(typeof(EbObject))]
    public class EbSimpleSelectOption
    {


        public EbSimpleSelectOption() { }

        [HideInPropertyGrid]
        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl)]
        public string EbSid { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl)]
        public string Name { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl)]
        public string Value { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl)]
        public string DisplayName { get; set; }
    }
}
