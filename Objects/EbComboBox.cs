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
    [EnableInBuilder(BuilderType.FormBuilder, BuilderType.FilterDialogBuilder)]
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

        private string VueSelectcode
        {
            get
            {
                string rs = "<div id='@nameWraper' data-toggle='tooltip' title='@tooltipText'>";
                for (int i = 0; i < this.NumberOfFields; i++)
                        rs += @"
<div style='display:inline-block; width:@perWidthpx; margin-right: -4px;'>
    <div class='input-group'>
        <v-select id='@name$$' style='width:{3}px;' 
            multiple
            v-model='displayMembers[$$]'
            :on-change='updateCk'
            placeholder = 'label$$'>
        </v-select>
        <span class='input-group-addon' @border-r$$> <i id='@nameTglBtn' class='fa  fa-search' aria-hidden='true'></i> </span>
    </div>
</div>".Replace("$$", i.ToString()).Replace("@border-r"+i, (i!=this.NumberOfFields-1) ? "style='border-radius: 0px;'" :"");
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

        public override string GetHtml()
        {
            return @"
    <script>
        Vue.component('v-select', VueSelect.VueSelect);
        Vue.config.devtools = true;
    </script>
               
   <div id='@nameContainer'  role='form' data-toggle='validator' style='position:absolute; width:@widthpx; left:@leftpx;  top:@toppx;'>
        <input type='hidden' name='@nameHidden4val' data-ebtype='16' id='@name'/>
        <div style='display:inline-block;' id='@nameLbl'>@label</div>
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
.Replace("@label", this.Label)
.Replace("@width", 900.ToString())//this.Width.ToString())
.Replace("@perWidth", (900/ this.NumberOfFields).ToString())
.Replace("@DDwidth", (this.DropdownWidth == 0) ? "300" : this.DropdownWidth.ToString())
.Replace("@tooltipText", this.ToolTipText);
        }
    }
}