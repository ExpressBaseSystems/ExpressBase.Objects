using ExpressBase.Common.Extensions;
using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using ExpressBase.Data;
using ExpressBase.Objects.ServiceStack_Artifacts;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace ExpressBase.Objects
{

    [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.BotForm)]
    public class EbSimpleSelect : EbControl
    {

        [EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm)]
        [OSE_ObjectTypes(EbObjectType.DataSource)]
        [PropertyEditor(PropertyEditorType.ObjectSelector)]
        public string DataSourceId { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm)]
        public int Value { get; set; }

        public EbSimpleSelect() { }

        public override string GetHead()
        {
            return this.RequiredString + @"
<script>

</script>
";
        }

        public override string GetDesignHtml()
        {
            return @"

    <div id='cont_@name@' Ctype='SimpleSelect' class='Eb-ctrlContainer' style='@hiddenString'>
        <div class='btn-group bootstrap-select show-tick'><button type='button' class='btn dropdown-toggle btn-default'><span class='filter-option pull-left'>Simple Select</span>&nbsp;<span class='bs-caret'><span class='caret'></span></span></button></div>
    </div>".RemoveCR().DoubleQuoted();//GetHtmlHelper(RenderMode.Developer).RemoveCR().DoubleQuoted();
        }

        public override string GetHtml()
        {
            return GetHtmlHelper(RenderMode.User);
        }

        private string GetHtmlHelper(RenderMode mode)
        {
            return @"
    <div id='cont_@name  ' Ctype='ComboBox' class='Eb-ctrlContainer' style='@hiddenString'>
       <div id='@nameContainer'  role='form' data-toggle='validator' style='width:100%;'>
            <input type='hidden' name='@nameHidden4val' data-ebtype='16' id='@name'/>
            <div style='display:inline-block;' id='@nameLbl'>@label</div>
            @VueSelectCode
            <div id='@name_loadingdiv' class='ebCombo-loader'>
                <i id='@name_loading-image' class='fa fa-spinner fa-pulse fa-2x fa-fw'></i><span class='sr-only'>Loading...</span>
            </div>
            <center><div id='@nameDDdiv' v-show='DDstate' class='DDdiv expand-transition'  style='width:@DDwidthpx;'> 
                <table id='@nametbl' tabindex='1000' style='width:100%' class='table table-striped table-bordered'></table>
            </div></center>
        </div>
    </div>";
        }
    }
}
