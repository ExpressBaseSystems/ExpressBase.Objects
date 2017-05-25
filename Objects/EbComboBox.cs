using ExpressBase.Data;
using ExpressBase.Objects.ServiceStack_Artifacts;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
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
    public class EbComboBox : EbControl
    {
        [ProtoBuf.ProtoMember(1)]
        public int DataSourceId { get; set; }

        [System.ComponentModel.Category("Behavior")]
        [ProtoBuf.ProtoMember(2)]
        public string DisplayMember { get; set; }

        [System.ComponentModel.Category("Behavior")]
        [ProtoBuf.ProtoMember(3)]
        public string ValueMember { get; set; }

        [ProtoBuf.ProtoMember(4)]
        [System.ComponentModel.Category("Layout")]
        public int DropdownHeight { get; set; }

        [ProtoBuf.ProtoMember(5)]
        public int Value { get; set; }

        [ProtoBuf.ProtoMember(6)]
        public string Text { get; set; }

        [ProtoBuf.ProtoMember(7)]
        [System.ComponentModel.Category("Layout")]
        public int DropdownWidth { get; set; }

        [ProtoBuf.ProtoMember(8)]
        [System.ComponentModel.Category("Behavior")]
        public bool MultiSelect { get; set; }

        [ProtoBuf.ProtoMember(9)]
        [System.ComponentModel.Category("Behavior")]
        public int MaxLimit { get; set; }

        [ProtoBuf.ProtoMember(10)]
        [System.ComponentModel.Category("Behavior")]
        public int MinLimit { get; set; }

        [ProtoBuf.ProtoMember(11)]
        [System.ComponentModel.Category("Behavior")]
        public DefaultSearchFor DefaultSearchFor { get; set; }

        [ProtoBuf.ProtoMember(12)]
        [System.ComponentModel.Category("Behavior")]
        public int NumberOfFields { get; set; }

        [ProtoBuf.ProtoMember(13)]
        [System.ComponentModel.Category("Behavior")]
        public int[] values { get; set; }

        private string VueDMcode
        {
            get
            {
                string rs = "";
                for (int i = 1; i <= this.NumberOfFields; i++)
                    rs += "displayMembers$$:[],".Replace("$$", i.ToString());
                return rs;
            }
        }

        private string VueSelectcode
        {
            get
            {
                string rs = "<div id='@nameWraper' data-toggle='tooltip' title='@tooltipText'>";
                for (int i = 1; i <= this.NumberOfFields; i++)
                    rs += @"
<div style='display:inline-block;'>
    <div style='display:inline-block;' id='@nameLbl'>label</div>
        <v-select id='@name$$' style='width:{3}px;' 
            multiple
	        v-model='displayMembers$$'
            :on-change='updateCk'
            placeholder = 'Search...'>
        </v-select>
</div>".Replace("$$", i.ToString());
                return rs + "</div>";
            }
        }

        public EbComboBox() { }

        public EbComboBox(object parent)
        {
            this.Parent = parent;
        }

        public override string GetHead()
        {
            return this.RequiredString + @"
$('#@name_loading-image').hide();
var @nameEbCombo = new EbSelect('@name', '@DSid', @DDHeight, '@ValueMember', '@DisplayMember', @MaxLimit, @MinLimit, @Required, '@DefaultSearchFor', '@MultiSelect', '', '@VueDMcode', '@servicestack_url', @values);
"
.Replace("@name", this.Name)
.Replace("@DSid", this.DataSourceId.ToString().Trim())
.Replace("@DDHeight", (this.DropdownHeight == 0) ? "400" : this.DropdownHeight.ToString())
.Replace("@ValueMember", this.ValueMember.ToString())
.Replace("@DisplayMember", this.DisplayMember.ToString())
.Replace("@MaxLimit", (!this.MultiSelect || this.MaxLimit == 0) ? "1" : this.MaxLimit.ToString())
.Replace("@MinLimit", this.MinLimit.ToString())
.Replace("@MultiSelect", this.MultiSelect.ToString().ToLower())
.Replace("@Required", this.Required.ToString().ToLower())
.Replace("@DefaultSearchFor", this.DefaultSearchFor.ToString())
.Replace("@DMembers", "['acmaster1_name', 'tdebit', 'tcredit']")
.Replace("@VueDMcode", this.VueDMcode)
.Replace("@values", "[1000]")//this.values.ToString())
.Replace("@servicestack_url", "https://expressbaseservicestack.azurewebsites.net");
        }

        public override string GetHtml()
        {
            return @"
    <script>
        Vue.component('v-select', VueSelect.VueSelect);
        Vue.config.devtools = true;
    </script>
               
   <div id='@nameContainer' style='position:absolute; left:@leftpx;  top:@toppx;'>
        <input type='hidden' name='@nameHidden4val' data-ebtype='16' id='@name'/>
        @VueSelectCode
    <div id='@name_loadingdiv' class='ebCombo-loader'>
        <i id='@name_loading-image' class='fa fa-spinner fa-pulse fa-2x fa-fw'></i><span class='sr-only'>Loading...</span>
    </div>
    <center><div id='@nameDDdiv' v-show='DDstate' class='DDdiv expand-transition'  style='width:@DDwidthpx;'> 
        <table id='@nametbl' tabindex='1000' style='width:100%' class='table table-striped table-bordered'></table>
    </div></center>
</div>"
.Replace("@VueSelectCode", this.VueSelectcode)
.Replace("@name", this.Name)
.Replace("@left", this.Left.ToString())
.Replace("@top", this.Top.ToString())
.Replace("@width", this.Width.ToString())
.Replace("@DDwidth", (this.DropdownWidth == 0) ? "300" : this.DropdownWidth.ToString())
.Replace("@tooltipText", this.ToolTipText);
        }
    }
}