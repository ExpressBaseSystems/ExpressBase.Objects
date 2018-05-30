using ExpressBase.Common;
using ExpressBase.Common.Extensions;
using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using ExpressBase.Common.Structures;
using ExpressBase.Data;
using ExpressBase.Objects.Objects.DVRelated;
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
    public class EbComboBox : EbControlUI
	{

        public EbComboBox() { }

        [OnDeserialized]
        public void OnDeserializedMethod(StreamingContext context)
        {
            this.BareControlHtml = this.GetBareHtml();
            this.ObjType = this.GetType().Name.Substring(2, this.GetType().Name.Length - 2);
        }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm)]
        [PropertyEditor(PropertyEditorType.ObjectSelector)]
        [OSE_ObjectTypes(EbObjectTypes.iDataSource)]
        public string DataSourceId { get; set; }

        [EnableInBuilder(BuilderType.FilterDialog, BuilderType.BotForm)]
        [HideInPropertyGrid]
        public ColumnColletion Columns { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm)]
        [PropertyEditor(PropertyEditorType.CollectionFrmSrc, "Columns")]
        public ColumnColletion DisplayMembers { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm)]
        [PropertyEditor(PropertyEditorType.CollectionFrmSrc, "Columns",1)]
        [OnChangeExec(@"if (this.Columns.$values.length === 0 ){pg.MakeReadOnly('ValueMember');} else {pg.MakeReadWrite('ValueMember');}")]
        public EbDataColumn ValueMember { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm)]
        public int DropdownHeight { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm)]
        public int Value { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm)]
        public string Text { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm)]
        public int DropdownWidth { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm)]
        [OnChangeExec(@"
            if (this.MultiSelect === true ){
                pg.MakeReadWrite('MaxLimit');   
                if (this.Required === true ){
                    if(this.MinLimit < 1)
                        this.MinLimit = 1;                 
                    pg.MakeReadWrite('MinLimit');
                }
                else{
                    this.MinLimit = 0;
                    pg.MakeReadOnly('MinLimit');                 
                }
            } 
            else {
                pg.MakeReadOnly('MaxLimit');                    
                pg.MakeReadOnly('MinLimit');
                this.MaxLimit = 1;
                if (this.Required === true ){
                    this.MinLimit = 1;  
                }
                else{
                    this.MinLimit = 0;  
                }
            }")]
        public override bool Required { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm)]
        [Category("Behavior")]
        [OnChangeExec(@"
            if (this.MultiSelect === true ){
                pg.MakeReadWrite('MaxLimit');   
                if (this.Required === true ){
                    if(this.MinLimit < 1)
                        this.MinLimit = 1;                 
                    pg.MakeReadWrite('MinLimit');
                }
                else{
                    this.MinLimit = 0;
                    pg.MakeReadOnly('MinLimit');                 
                }
            } 
            else {
                pg.MakeReadOnly('MaxLimit');                    
                pg.MakeReadOnly('MinLimit');
                this.MaxLimit = 1;
                if (this.Required === true ){
                    this.MinLimit = 1;  
                }
                else{
                    this.MinLimit = 0;  
                }
            }")]
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

		[HideInPropertyGrid]
		[EnableInBuilder(BuilderType.BotForm)]
		public override bool IsReadOnly { get => this.ReadOnly; }

		private string VueSelectcode
        {
            get
            {
                int noOfFileds = this.DisplayMembers.Count;
                string rs = "<div id='@name@Wraper' data-toggle='tooltip' title='@tooltipText'>";
                for (int i = 0; i < noOfFileds; i++)
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
.Replace("@perWidth@", ((int)(100 / noOfFileds)).ToString())
.Replace("@border-r" + i, (i != noOfFileds - 1) ? "style='border-radius: 0px;'" : "");
                return rs + "</div>";
            }
        }

        public override string GetToolHtml()
        {
            return @"<div eb-type='@toolName' class='tool'> &#9869;  @toolName</div>".Replace("@toolName", this.GetType().Name.Substring(2));
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
            return @"
<div id='cont_@name@  ' class='Eb-ctrlContainer' Ctype='TextBox' style='@HiddenString '>
   <div role='form' data-toggle='validator' style=' width: inherit;'>
    <div class='eb-ctrl-label' style='background-color:@LabelBackColor@; color:@LabelForeColor@ '> @Label@  </div>
      <div class='combo-wrap' data-toggle='tooltip' title='' data-original-title=''>
         <div style='display: inline-block; width: 100%; margin-right: -4px;'>
            <div class='input-group'>
               <div class='dropdown v-select searchable' id='acmasterid0'>
                  <div type='button' class='dropdown-toggle clearfix' style='border-top-left-radius: 5px; border-bottom-left-radius: 5px;'>
                     <input debounce='0' type='search'  readonly  placeholder='label0' class='form-control' id='acmaster1_xid' style='width: 100%; background-color: #fff;'> <i role='presentation' class='open-indicator' style='display: none;'></i> 
                     <div class='spinner' style='display: none;'>Loading...</div>
                  </div>
               </div>
               <span class='input-group-addon' style='border-radius: 0px;'><i id='acmasteridTglBtn' aria-hidden='true' class='fa  fa-search'></i></span>
            </div>
         </div>
      </div>
   </div>
</div>"
    .Replace("@name@", this.Name)
    .RemoveCR().DoubleQuoted();//GetHtmlHelper(RenderMode.Developer).RemoveCR().DoubleQuoted();
        }

        public override string GetHtml()
        {
            return GetHtmlHelper(RenderMode.User);
        }

        public override string DesignHtml4Bot { get => @"
<div id='cont_@name@  ' class='Eb-ctrlContainer' Ctype='TextBox' style='@HiddenString '>
   <div role='form' data-toggle='validator' style=' width: inherit;'>
    <span style='background-color:@LabelBackColor@; color:@LabelForeColor@ '> @Label@  </span>
      <div class='combo-wrap' data-toggle='tooltip' title='' data-original-title=''>
         <div style='display: inline-block; width: 100%; margin-right: -4px;'>
            <div class='input-group'>
               <div class='dropdown v-select searchable' id='acmasterid0'>
                  <div type='button' class='dropdown-toggle clearfix' style='border-top-left-radius: 5px; border-bottom-left-radius: 5px;'>
                     <input debounce='0' type='search'  readonly  placeholder='label0' class='form-control' id='acmaster1_xid' style='width: 100%; background-color: #fff;'> <i role='presentation' class='open-indicator' style='display: none;'></i> 
                     <div class='spinner' style='display: none;'>Loading...</div>
                  </div>
               </div>
               <span class='input-group-addon' style='border-radius: 0px;'><i id='acmasteridTglBtn' aria-hidden='true' class='fa  fa-search'></i></span>
            </div>
         </div>
      </div>
   </div>
</div>"; set => base.DesignHtml4Bot = value; }

        public override string GetBareHtml()
        {
            if (this.DisplayMembers != null)
            {
                return @"
<div id='@name@Container'  role='form' data-toggle='validator' style='width:100%;'>
    <input type='hidden' name='@name@Hidden4val' data-ebtype='16' id='@name@'/>
	<span id='@name@Lbl' style='@LabelBackColor@ @LabelForeColor@ '> @Label@  </span>

    @VueSelectCode
    <div id='@name@_loadingdiv' class='ebCombo-loader'>
        <i id='@name@_loading-image' class='fa fa-spinner fa-pulse fa-2x fa-fw'></i><span class='sr-only'>Loading...</span>
    </div>
    <center>
        <div id='@name@DDdiv' v-show='DDstate' class='DDdiv expand-transition'  style='width:@DDwidth%;'> 
            <table id='@name@tbl' tabindex='1000' style='width:100%' class='table table-bordered'></table>
        </div>
    </center>
</div>"
    .Replace("@VueSelectCode", this.VueSelectcode)
    .Replace("@name@", this.Name)
    .Replace("@Label@ ", ((this.Label != null) ? this.Label : "@Label@ "))
    .Replace("@width", 900.ToString())//this.Width.ToString())
    .Replace("@perWidth", (this.DisplayMembers.Count != 0) ? (900 / this.DisplayMembers.Count).ToString() : 900.ToString())
    .Replace("@DDwidth", (this.DropdownWidth == 0) ? "100" : this.DropdownWidth.ToString())
    ;
            }
            else
                return string.Empty;
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