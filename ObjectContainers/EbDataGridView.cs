using ExpressBase.Data;
using ExpressBase.Objects.ObjectContainers;
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
        public int DvId { get; set; }

        [ProtoBuf.ProtoMember(11)]
        public string Dvname { get; set; }

        [ProtoBuf.ProtoMember(12)]
        public string Login { get; set; }

        [ProtoBuf.ProtoMember(13)]
        public string Dvlist { get; set; }
        [ProtoBuf.ProtoMember(14)]
        public string Dslist { get; set; }
        [ProtoBuf.ProtoMember(15)]
        public Dictionary<int, EbObjectWrapper> DslistAll { get; set; }



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
            if (filterForm != null)
            {
                string _html = string.Empty;
                string _head = string.Empty;

                _html = @"<div style='margin-top:10px;' id='filterBox'>";
                _html += filterForm.GetHead();
                _head += filterForm.GetHtml();
                _html += @"</div>";

                this.filters = _html;
                this.script = _head;
            }
        }


        public override void Init4Redis(IRedisClient redisclient, IServiceClient serviceclient)
        {
            base.Redis = redisclient;
            base.ServiceStackClient = serviceclient;
        }


        public override string GetHtml()
        {
            return @"
    <div class='tablecontainer' style='background-color:rgb(260,260,260);'>        
         <ul class='nav nav-tabs' id='table_tabs'>
                <li class='nav-item active'>
                    <a class='nav-link' href='#@tableId_tab_1' data-toggle='tab'><i class='fa fa-home' aria-hidden='true'></i>&nbsp; Home</a>
                </li>
         </ul></br>
         <div class='tab-content' id='table_tabcontent'>
             <div id='@tableId_tab_1' class='tab-pane active'>

                 <div id='TableControls_@tableId_1' class = 'well well-sm' style='margin-bottom:5px!important;display:none'>
                    <label>@dvname</label>
                    <button id='btnGo' class='btn btn-primary' style='float: right;'>Run</button>
                    @filters  
                </div>
                <div id='@tableId_1container'>
                    <div id='@tableId_1TableColumns4Drag' style='border:1px solid;display:none;height:100%;min-height: 400px;overflow-y: auto;'>
                    </div>         
                    <div style='width:auto;' id='@tableId_1divcont'>
                        <div id ='@tableId_1ColumnsDispaly' style= 'display:none;'class ='colCont'></div>
                        <table id='@tableId_1' class='table table-striped table-bordered'></table>
                    </div>
                    <div id='@tableId_1TableColumnsPPGrid' style='display:none;height:100%;min-height: 400px;overflow-y: auto;'></div>
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
        tid: '@tableId_1' ,
        login:'@login'
        //settings: JSON.parse(data),
        //fnKeyUpCallback: 
    });
//});
</script>"
.Replace("@dataSourceId", this.DataSourceId.ToString().Trim())
.Replace("@tableId", this.Name)
.Replace("@dvname", this.Dvname)
.Replace("@login", this.Login)
.Replace("@datasourcedd", this.getdropdownColumn())
//.Replace("@tableViewName", ((string.IsNullOrEmpty(this.Label)) ? "&lt;ReportLabel Undefined&gt;" : this.Label))
.Replace("@servicestack_url", "https://expressbaseservicestack.azurewebsites.net")
.Replace("@filters", this.filters)
.Replace("@FilterBH", this.FilterBH.ToString())
.Replace("@collapsBtn", (this.filters != null) ? @"<div id = 'btnCollapse' class='btn btn-default' data-toggle='collapse' data-target='#filterBox' aria-expanded='true' aria-controls='filterBox'>
                    <i class='fa fa-chevron-down' aria-hidden='true'></i>
                </div>" : string.Empty)
//.Replace("@data.columns", this.ColumnColletion.ToJson())
.Replace("@dvId", this.DvId.ToString());
            //.Replace("@tvPref4User", tvPref4User);
        }

        public string getdropdownColumn()
        {
            string dd = string.Empty;
            dd += @"<div class='dropdown' id='datatSourceDropdown' style='display: inline-block;'>
                <button class='btn btn-primary dropdown-toggle' type='button' data-toggle='dropdown'>Select DataSource
                    <span class='caret'></span>
                </button>
                <ul class='dropdown-menu scrollable-menu'>";
            foreach(var element in this.DslistAll)
            {
                dd += "<li data-dsid=" + element.Key + ">";
                dd += "<a href = '#'>" + element.Value.Name + "</a>";
                dd += "</li>";
            }

            dd += "</ul>";
            dd += "</div>";
            return dd;
        }
    }
}
