using ExpressBase.Data;
using ExpressBase.Objects.ServiceStack_Artifacts;
using Newtonsoft.Json;
using ServiceStack;
using ServiceStack.Redis;
using System.Collections.Generic;
using System.IO;

namespace ExpressBase.Objects
{
    [ProtoBuf.ProtoContract]
    public class EbDataGridView : EbControlContainer
    {
        [ProtoBuf.ProtoMember(1)]
        public int DataSourceId { get; set; }

        [ProtoBuf.ProtoMember(9)]
        public int FilterDialogId { get; set; }

        [ProtoBuf.ProtoMember(10)]
        public int dvId { get; set; }

        [ProtoBuf.ProtoMember(11)]
        public string dvname { get; set; }

      

        public string Token { get; set; }

        //internal ColumnColletion ColumnColletion { get; set; }

        public EbDataGridView() { }

        public override string GetHead()
        {
            return this.script;
        }
        
        private int FilterBH = 0;

        private EbForm __filterForm;

        private string script;

        private string filters;

        public void SetFilterForm(EbFilterDialog filterForm)    
        {
            string xjson = "{\"$type\": \"System.Collections.Generic.List`1[[ExpressBase.Objects.EbControl, ExpressBase.Objects]], mscorlib\", \"$values\": " +
                filterForm.FilterDialogJson + "}";

            var ControlColl = JsonConvert.DeserializeObject(xjson,
                new JsonSerializerSettings{ TypeNameHandling = TypeNameHandling.All }) as List<EbControl>;
            string _html = "";
            string _head = "";
            if (filterForm != null)
            {
                _html = @"<div style='margin-top:10px;' id='filterBox'>";
                foreach (EbControl c in ControlColl)
                {
                    _html += c.GetHtml();
                    _head += c.GetHead();
                }
                _html += @"</div>";
            }
            this.filters = _html;
            this.script = _head;
        }

        public override void Init4Redis(IRedisClient redisclient, IServiceClient serviceclient)
        {
            base.Redis = redisclient;
            base.ServiceStackClient = serviceclient;
        }


        public override string GetHtml()
        {
            return @"
<div class='tablecontainer' id='@tableId_1container' style='background-color:rgb(260,260,260)'>        
         <ul class='nav nav-tabs' id='table_tabs'>
                <li class='nav-item active'>
                    <a class='nav-link' href='#@tableId_tab_1' data-toggle='tab'><i class='fa fa-home' aria-hidden='true'></i>&nbsp; Home</a>
                </li>
         </ul></br>
         <div class='tab-content' id='table_tabcontent'>
             <div id='@tableId_tab_1' class='tab-pane active'>
                 <div id='TableControls_@tableId_1' class = 'well well-sm' style='margin-bottom:5px!important;'>
                    <label>@dvname</label>
                    <button id='btnGo' class='btn btn-primary' style='float: right;'>Run</button>
                    @filters  
                </div>
                <div style='width:auto;' id='@tableId_1divcont'>
                    <table id='@tableId_1' class='table table-striped table-bordered'></table>
                </div>
                <div id='graphcontainer_tab@tableId_1' style='display: none;'>
                <div style='height: 50px;margin-bottom: 5px!important;' class='well well-sm'>
                    <label>@dvname</label>
                    <div id = 'btnColumnCollapse@tableId_1' class='btn btn-default' style='float: right;'>
                        <i class='fa fa-cog' aria-hidden='true'></i>
                     </div>
                     <div class='dropdown' id='graphDropdown_tab@tableId_1' style='display: inline-block;padding-top: 1px;float:right'>
                             <button class='btn btn-default dropdown-toggle' type='button' data-toggle='dropdown'>
                           <span class='caret'></span></button>
                          <ul class='dropdown-menu'>
                                <li><a href =  '#'><i class='fa fa-line-chart custom'></i> Line</a></li>
                                <li><a href = '#'><i class='fa fa-bar-chart custom'></i> Bar </a></li>
                                <li><a href = '#'><i class='fa fa-area-chart custom'></i> AreaFilled </a></li>
                                <li><a href = '#'><i class='fa fa-pie-chart custom'></i> pie </a></li>
                                <li><a href = '#'> doughnut </a></li>
                                </ul>
                      </div>
                      <button id='reset_zoom@tableId_1' class='btn btn-default' style='float: right;'>Reset zoom</button>
                       
                </div>
                <div id ='columns4Drag@tableId_1' style='display:none;'>
                    <div style='display: inline-block;'>
                        <label class='nav-header disabled'><center><strong>Columns</strong></center><center><font size='1'>Darg n Drop to X or Y Axis</font></center></label>
                        <input id='searchColumn@tableId_1' type='text' class ='form-control' placeholder='search for column'/>
                        <ul class='list-group' style='height: 450px; overflow-y: auto;'>
                         </ul>  
                    </div>
                    <div style='display: inline-block;vertical-align: top;width: 806px;'>
                        <div class='input-group'>
                          <span class='input-group-addon' id='basic-addon3'>X-Axis</span>
                          <div class='form-control' style='padding: 4px;height:33px' id ='X_col_name@tableId_1'></div>
                        </div>
                        <div class='input-group' style='padding-top: 1px;'>
                          <span class='input-group-addon' id='basic-addon3'>Y-Axis</span>
                          <div class='form-control' style='padding: 4px;height:33px' id ='Y_col_name@tableId_1'></div>
                        </div>
                    </div>
                </div>
                <canvas id='myChart@tableId_1' width='auto' height='auto'></canvas>
            </div>
          </div>
        </div>
</div>
<script>
//$.post('GetTVPref4User', { dsid: @dataSourceId }, function(data){
    var EbDataTable_@tableId = new EbDataTable({
        ds_id: @dataSourceId, 
        dv_id: @dvId, 
        ss_url: '@servicestack_url', 
        tid: '@tableId_1' 
        //settings: JSON.parse(data),
        //fnKeyUpCallback: 
    });
//});
</script>"
.Replace("@dataSourceId", this.DataSourceId.ToString().Trim())
.Replace("@tableId", this.Name)
.Replace("@dvname", this.dvname)
//.Replace("@tableViewName", ((string.IsNullOrEmpty(this.Label)) ? "&lt;ReportLabel Undefined&gt;" : this.Label))
.Replace("@servicestack_url", "https://expressbaseservicestack.azurewebsites.net")
.Replace("@filters", this.filters)
.Replace("@FilterBH", this.FilterBH.ToString())
.Replace("@collapsBtn", (this.filters != null) ? @"<div id = 'btnCollapse' class='btn btn-default' data-toggle='collapse' data-target='#filterBox' aria-expanded='true' aria-controls='filterBox'>
                    <i class='fa fa-chevron-down' aria-hidden='true'></i>
                </div>" : string.Empty)
//.Replace("@data.columns", this.ColumnColletion.ToJson())
.Replace("@dvId", this.dvId.ToString());
//.Replace("@tvPref4User", tvPref4User);
        }
    }
}
