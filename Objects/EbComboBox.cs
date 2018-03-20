using ExpressBase.Common.Extensions;
using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using ExpressBase.Data;
using ExpressBase.Objects.ServiceStack_Artifacts;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace ExpressBase.Objects
{
    public enum DefaultSearchFor
    {
        BeginingWithKeyword,
        EndingWithKeyword,
        ExactMatch,
        Contains,
    }

    [ProtoBuf.ProtoContract]
    [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm)]
    public class EbComboBox : EbControl
    {

        public EbComboBox() {
        }

        [OnDeserialized]
        public void OnDeserializedMethod(StreamingContext context)
        {
            this.BareControlHtml = this.GetBareHtml();
            this.ObjType = this.GetType().Name.Substring(2, this.GetType().Name.Length - 2);
        }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm)]
		[PropertyEditor(PropertyEditorType.ObjectSelector)]
        public string DataSourceId { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm)]
        public string DisplayMember { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm)]
        public string ValueMember { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm)]
        public int DropdownHeight { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm)]
        public int Value { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm)]
        public string Text { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm)]
        public int DropdownWidth { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm)]
        [System.ComponentModel.Category("Behavior")]
        public bool MultiSelect { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm)]
        public int MaxLimit { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm)]
        public int MinLimit { get; set; }


        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm)]
        public DefaultSearchFor DefaultSearchFor { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm)]
        public int NumberOfFields { get; set; }

        //[EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm)]
        public int[] values { get; set; }

        private string VueSelectcode
        {
            get
            {
                string rs = "<div id='@name@Wraper' data-toggle='tooltip' title='@tooltipText'>";
                for (int i = 0; i < this.NumberOfFields; i++)
                    rs += @"
<div style='display:inline-block; width:@perWidth@%; margin-right: -4px;'>
    <div class='input-group'>
        <v-select id='@name@$$' style='width:{3}px;' 
            multiple
            v-model='displayMembers[$$]'
            :on-change='updateCk'
            placeholder = 'label$$'>
        </v-select>
        <span class='input-group-addon' @border-r$$> <i id='@name@TglBtn' class='fa  fa-search' aria-hidden='true'></i> </span>
    </div>
</div>"
.Replace("$$", i.ToString())
.Replace("@perWidth@", ((int)( 100 / this.NumberOfFields)).ToString())
.Replace("@border-r" + i, (i != this.NumberOfFields - 1) ? "style='border-radius: 0px;'" : "");
                return rs + "</div>";
            }
        }

        public override string GetHead()
        {
            return this.RequiredString + @"
<script>
    Vue.component('v-select', VueSelect.VueSelect);
    Vue.config.devtools = true;
</script>
$('#@name@_loading-image').hide();
var @nameEbCombo = new EbSelect('@name', '@DSid', @DDHeight, '@vmName', '', @MaxLimit, @MinLimit, @Required, '@DefaultSearchFor', '@servicestack_url', @values);
"
.Replace("@name", this.Name)
.Replace("@DSid", this.DataSourceId.ToString().Trim())
.Replace("@DDHeight", (this.DropdownHeight == 0) ? "400" : this.DropdownHeight.ToString())
.Replace("@vmName", this.ValueMember.ToString())
.Replace("@dmNames", "['acmaster1_name', 'tdebit', 'tcredit']")
.Replace("@MaxLimit", (!this.MultiSelect || this.MaxLimit == 0) ? "1" : this.MaxLimit.ToString())
.Replace("@MinLimit", this.MinLimit.ToString())
.Replace("@Required", this.Required.ToString().ToLower())
.Replace("@DefaultSearchFor", this.DefaultSearchFor.ToString())
.Replace("@servicestack_url", "https://expressbaseservicestack.azurewebsites.net")
.Replace("@values", "[1000]");//this.values.ToString());
        }

        public override string GetDesignHtml()
        {
            return @"<div id='cont_@name@  ' class='Eb-ctrlContainer' Ctype='TextBox' style='@HiddenString '>
                        <div role='form' data-toggle='validator' style=' width: inherit;'><input type='hidden' name='acmasteridHidden4val' data-ebtype='16' id='acmasterid'> <div id='acmasteridLbl' style='display: inline-block;'></div> <div id='acmasteridWraper' data-toggle='tooltip' title='' data-original-title=''><div style='display: inline-block; width: 33%; margin-right: -4px;'><div class='input-group'><div class='dropdown v-select searchable' id='acmasterid0'><div type='button' class='dropdown-toggle clearfix' style='border-top-left-radius: 5px; border-bottom-left-radius: 5px;'> <input debounce='0' type='search'  readonly  placeholder='label0' class='form-control' id='acmaster1_xid' style='width: 100%; background-color: #fff;'> <i role='presentation' class='open-indicator' style='display: none;'></i> <div class='spinner' style='display: none;'>Loading...</div></div> <!----></div> <span class='input-group-addon' style='border-radius: 0px;'><i id='acmasteridTglBtn' aria-hidden='true' class='fa  fa-search'></i></span></div></div> <div style='display: inline-block; width: 33%; margin-right: -4px;'><div class='input-group'><div class='dropdown v-select searchable' id='acmasterid1'><div type='button' class='dropdown-toggle clearfix'> <input debounce='0' type='search' placeholder='label1' readonly class='form-control' id='acmaster1_name' style='width: 100%; background-color: #fff;'> <i role='presentation' class='open-indicator' style='display: none;'></i> <div class='spinner' style='display: none;'>Loading...</div></div> <!----></div> <span class='input-group-addon' style='border-radius: 0px;'><i id='acmasteridTglBtn' aria-hidden='true' class='fa  fa-search'></i></span></div></div> <div style='display: inline-block; width: 33%; margin-right: -4px;'><div class='input-group'><div class='dropdown v-select searchable' id='acmasterid2'><div type='button' class='dropdown-toggle clearfix'> <input debounce='0' type='search' readonly placeholder='label2' class='form-control' id='tdebit' style='width: 100%; background-color: #fff;'> <i role='presentation' class='open-indicator' style='display: none;'></i> <div class='spinner' style='display: none;'>Loading...</div></div> <!----></div> <span class='input-group-addon'><i id='acmasteridTglBtn' aria-hidden='true' class='fa  fa-search'></i></span></div></div></div> <div id='acmasterid_loadingdiv' class='ebCombo-loader'><i id='acmasterid_loading-image' class='fa fa-spinner fa-pulse fa-2x fa-fw' style='display: none;'></i><span class='sr-only'>Loading...</span></div> <center><div id='acmasteridDDdiv' class='DDdiv expand-transition' style='width: 600px; display: none;'><table id='acmasteridtbl' class='table table-striped table-bordered' style='width: 100%;'></table></div></center></div>
                    </div>"
.Replace("@name@", this.Name)
.RemoveCR().DoubleQuoted();//GetHtmlHelper(RenderMode.Developer).RemoveCR().DoubleQuoted();
        }

        public override string GetHtml()
        {
            return GetHtmlHelper(RenderMode.User);
        }

        public override string GetBareHtml()
        {
            return @"

       <div id='@name@Container'  role='form' data-toggle='validator' style='width:100%;'>
            <input type='hidden' name='@name@Hidden4val' data-ebtype='16' id='@name@'/>
            <div style='display:inline-block;' id='@name@Lbl'>@label</div>

            @VueSelectCode
            <div id='@name@_loadingdiv' class='ebCombo-loader'>
                <i id='@name@_loading-image' class='fa fa-spinner fa-pulse fa-2x fa-fw'></i><span class='sr-only'>Loading...</span>
            </div>
            <center>
                <div id='@name@DDdiv' v-show='DDstate' class='DDdiv expand-transition'  style='width:@DDwidthpx;'> 
                    <table id='@name@tbl' tabindex='1000' style='width:100%' class='table table-striped table-bordered'></table>
                </div>
            </center>
        </div>"
.Replace("@VueSelectCode", this.VueSelectcode)
.Replace("@name@", this.Name)
.Replace("@width", 900.ToString())//this.Width.ToString())
.Replace("@perWidth", (this.NumberOfFields != 0) ? (900 / this.NumberOfFields).ToString() : 900.ToString())
.Replace("@DDwidth", (this.DropdownWidth == 0) ? "300" : this.DropdownWidth.ToString())
;
        }

        private string GetHtmlHelper(RenderMode mode)
        {
            return @"
    <div id='cont_@name@  ' Ctype='ComboBox' class='Eb-ctrlContainer' style='@hiddenString'>
           @barehtml@
    </div>"
.Replace("@barehtml@", this.GetBareHtml())
.Replace("@name@", this.Name)
.Replace("@label", this.Label)
.Replace("@tooltipText", this.ToolTipText);
        }
    }
}