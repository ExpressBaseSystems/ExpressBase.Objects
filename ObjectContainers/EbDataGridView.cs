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
            //var ControlColl = filterForm.FilterDialogJson.FromJson<List<EbControl>>();
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
        }

        public override void Init4Redis(IRedisClient redisclient, IServiceClient serviceclient)
        {
            base.Redis = redisclient;
            base.ServiceStackClient = serviceclient;
        }

        //string tvPref4User = string.Empty;

        //public string GetColumn4DataTable(ColumnColletion  __columnCollection)
        //{
        //    string colDef = string.Empty;
        //    colDef = "{\"dvName\": \"<Untitled>\",\"hideSerial\": false, \"hideCheckbox\": false, \"lengthMenu\":[ [100, 200, 300, -1], [100, 200, 300, \"All\"] ],";
        //    colDef+=" \"scrollY\":300, \"rowGrouping\":\"\",\"leftFixedColumns\":0,\"rightFixedColumns\":0,\"columns\":[";
        //    colDef += "{\"width\":10, \"searchable\": false, \"orderable\": false, \"visible\":true, \"name\":\"serial\", \"title\":\"#\"},";
        //    colDef += "{\"width\":10, \"searchable\": false, \"orderable\": false, \"visible\":true, \"name\":\"checkbox\"},";
        //    foreach (EbDataColumn  column in __columnCollection)
        //    {
        //        colDef += "{";
        //        colDef += "\"data\": " + __columnCollection[column.ColumnName].ColumnIndex.ToString();
        //        colDef += string.Format(",\"title\": \"{0}<span hidden>{0}</span>\"", column.ColumnName);
        //        var vis = (column.ColumnName == "id") ? false.ToString().ToLower() : true.ToString().ToLower();
        //        colDef += ",\"visible\": " + vis;
        //        colDef += ",\"width\": " + 100;
        //        colDef += ",\"name\": \"" + column.ColumnName + "\"";
        //        colDef += ",\"type\": \"" + column.Type.ToString() + "\"";
        //        //var cls = (column.Type.ToString() == "System.Boolean") ? "dt-center tdheight" : "tdheight";
        //        colDef += ",\"className\": \"tdheight\"";
        //        colDef += "},";
        //    }
        //    colDef = colDef.Substring(0 , colDef.Length - 1) +"],";
        //    string colext = "\"columnsext\":[";
        //    colext += "{\"name\":\"serial\"},";
        //    colext += "{\"name\":\"checkbox\"},";
        //    foreach (EbDataColumn column in __columnCollection)
        //    {
        //        colext += "{";
        //        if (column.Type.ToString() == "System.Int32" || column.Type.ToString() == "System.Decimal" || column.Type.ToString() == "System.Int16" || column.Type.ToString() == "System.Int64")
        //            colext += "\"name\":\"" + column.ColumnName + "\",\"AggInfo\":true,\"DecimalPlace\":2,\"RenderAs\":\"Default\"";
        //        else if (column.Type.ToString() == "System.Boolean")
        //            colext += "\"name\":\"" + column.ColumnName + "\",\"IsEditable\":false,\"RenderAs\":\"Default\"";
        //        else if (column.Type.ToString() == "System.DateTime")
        //            colext += "\"name\":\"" + column.ColumnName + "\",\"Format\":\"Date\"";
        //        else if (column.Type.ToString() == "System.String")
        //            colext += "\"name\":\"" + column.ColumnName + "\",\"RenderAs\":\"Default\"";
        //        colext += "},";
        //    }
        //    colext = colext.Substring(0,colext.Length-1)+"]";
        //    return colDef + colext + "}";
        //}


        public override string GetHtml()
        {
            //this.Redis.Remove(string.Format("{0}_ds_{1}_columns", "eb_roby_dev", 46));
            ////this.Redis.Remove(string.Format("{0}_TVPref_{1}_uid_{2}", "eb_roby_dev", this.DataSourceId, 1));
            //this.ColumnColletion = this.Redis.Get<ColumnColletion>(string.Format("{0}_ds_{1}_columns", "eb_roby_dev", this.DataSourceId));
            //if (this.ColumnColletion == null)
            //{
            //    var resp = base.ServiceStackClient.Get<DataSourceColumnsResponse>(new DataSourceColumnsRequest { Id = this.DataSourceId, Token = this.Token, TenantAccountId = "eb_roby_dev" });
            //    this.ColumnColletion = resp.Columns;
            //}

            //tvPref4User = this.Redis.Get<string>(string.Format("{0}_TVPref_{1}_uid_{2}", "eb_roby_dev", this.DataSourceId, 1));
            //if (string.IsNullOrEmpty(tvPref4User))
            //{
            //    tvPref4User = this.GetColumn4DataTable(this.ColumnColletion);
            //    this.Redis.Set(string.Format("{0}_TVPref_{1}_uid_{2}", "eb_roby_dev", this.DataSourceId, 1), tvPref4User);
            //}


            //@tableViewName
            return @"
<div class='tablecontainer' id='@tableId_1container' style='background-color:rgb(260,260,260)'>
        <label>
                @dvname
        </label>
         <ul class='nav nav-tabs' id='table_tabs'>
                <li class='nav-item active'>
                    <a class='nav-link' href='#@tableId_tab_1' data-toggle='tab'><i class='fa fa-home' aria-hidden='true'></i>&nbsp; Home</a>
                </li>
         </ul></br>
         <div class='tab-content' id='table_tabcontent'>
             <div id='@tableId_tab_1' class='tab-pane active'>
                 <div id='TableControls_@tableId_1' class = 'well well-sm' style='margin-bottom:5px!important;'>
                    <button id='btnGo' class='btn btn-primary' >Run</button>
                    @filters  
                </div>
                <div style='width:auto;' id='@tableId_1divcont'>
                    <table id='@tableId_1' class='table table-striped table-bordered'></table>
                </div>
                <div id='graphcontainer_tab@tableId_1' style='border:1px solid;display: none;'>
                <div style='height: 50px;margin-bottom: 1px!important;' class= 'well well-sm'>
                     <div class='dropdown' id='graphDropdown_tab@tableId_1' style='display: inline-block;padding-top: 1px;float:right'>
                             <button class='btn btn-default dropdown-toggle' type='button' data-toggle='dropdown'>
                           <span class='caret'></span></button>
                          <ul class='dropdown-menu'>
                                <li><a href='#'>Line</a></li>
                                <li><a href = '#'> Bar </a ></li>
                                <li><a href = '#'> AreaFilled </a></li>
                                <li><a href = '#'> pie </a></li>
                                <li><a href = '#'> doughnut </a></li>
                                </ul>
                      </div>
                      <button id='reset_zoom@tableId_1' class='btn btn-default'>Reset zoom</button>
                      <div id = 'btnColumnCollapse@tableId_1' class='btn btn-default'>
                            <i class='fa fa-chevron-down' aria-hidden='true'></i>
                      </div>
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

//    public class AggregateInfo
//    {
//        public string colname { get; set; }
//        public string coltype { get; set; }
//        public int deci_val{ get; set;}
//    }

//    [ProtoBuf.ProtoContract]
//    public class EbDataGridViewColumn : EbControl
//    {
//        private EbDataGridViewColumnType _columnType = EbDataGridViewColumnType.Null;

//        [ProtoBuf.ProtoMember(1)]
//        public EbDataGridViewColumnType ColumnType
//        {
//            get { return _columnType; }
//            set
//            {
//                if (value == EbDataGridViewColumnType.Numeric)
//                    this.ExtendedProperties = new EbDataGridViewNumericColumnProperties();
//                else if (value == EbDataGridViewColumnType.DateTime)
//                    this.ExtendedProperties = new EbDataGridViewDateTimeColumnProperties();
//                else if (value == EbDataGridViewColumnType.Boolean)
//                    this.ExtendedProperties = new EbDataGridViewBooleanColumnProperties();
//                else if(value == EbDataGridViewColumnType.Text)
//                    this.ExtendedProperties = new EbDataGridViewColumnProperties();

//                _columnType = value;
//            }
//        }

//        [ProtoBuf.ProtoMember(2)]
//#if NET462
//        [TypeConverter(typeof(ExpandableObjectConverter))]
//#endif
//        public EbDataGridViewColumnProperties ExtendedProperties { get; set; }

//        public EbDataGridViewColumn()
//        {
//            this.Width = 100;
//        }

//        public string GetColumnDefJs(ColumnColletion ColumnColletion)
//        {
//            string script = "{";
//            script += "data: " + ColumnColletion[this.Name].ColumnIndex.ToString();
//            script += string.Format(",title: '{0}<span hidden>{1}</span>'", (this.Label != null) ? this.Label : this.Name, this.Name);
//            script += ",className: '" + this.GetClassName() + "'";
//            script += ",visible: " + (!this.Hidden).ToString().ToLower();
//            script += ",width: " + this.Width.ToString();
//            script += this.GetRenderFunc();
//            script += ",name: '" + this.Name + "'";
//            script += "},";

//            return script;
//        }

//        private string GetClassName()
//        {
//            string _c = string.Empty;

//            if (this.ColumnType == EbDataGridViewColumnType.Boolean)
//                _c = "dt-body-center";
//            else if (this.ColumnType == EbDataGridViewColumnType.Numeric)
//                _c = "dt-body-right";
//            else
//                _c = "dt-body-left";

//            return _c;
//        }

//        private string GetRenderFunc(){
//            string _r = string.Empty;
//            string _fwrapper = ", render: function( data, type, row, meta ) { {0} }";

//            if (this.ColumnType == EbDataGridViewColumnType.Numeric)
//            {
//                var ext = this.ExtendedProperties as EbDataGridViewNumericColumnProperties;

//                if (ext.ShowProgressbar)
//                    _r = "return renderProgressCol(data);";
//                else
//                {
//                    if (ext != null)
//                    {
//                        var deci_places = (ext.DecimalPlaces > 0) ? ext.DecimalPlaces : 2;

//                        if (!ext.Localize)
//                            _r = string.Format("return parseFloat(data).toFixed({0});", deci_places);
//                        else
//                        {
//                            if (!ext.IsCurrency)
//                                _r = "return parseFloat(data).toLocaleString('en-US', { maximumSignificantDigits: {0} });".Replace("{0}", deci_places.ToString());
//                            else
//                                _r = "return parseFloat(data).toLocaleString('en-US', { style: 'currency', currency: 'USD', maximumSignificantDigits: {0} });".Replace("{0}", deci_places.ToString());
//                        }
//                    }
//                    else
//                        _r = "return data;";
//                }

//                _r = _fwrapper.Replace("{0}", _r);
//            }
//            else if (this.ColumnType == EbDataGridViewColumnType.DateTime)
//                _r = _fwrapper.Replace("{0}", "return moment.unix(data).format('MM/DD/YYYY');");
//            else if (this.ColumnType == EbDataGridViewColumnType.Null)
//            {
//                var ext = this.ExtendedProperties as EbDataGridViewBooleanColumnProperties;
//                _r = _fwrapper.Replace("{0}", "return renderToggleCol(data,@ext);".Replace("@ext", ext.IsEditable.ToString().ToLower()));
//            }
//            else if (this.ColumnType == EbDataGridViewColumnType.Chart)
//                _r = _fwrapper.Replace("{0}", "return lineGraphDiv(data);");
//            else
//                _r = string.Empty; // _fwrapper.Replace("{0}", "return data;");

//            return _r;
//        }
//    }

//    [ProtoBuf.ProtoContract]
//    [ProtoBuf.ProtoInclude(1, typeof(EbDataGridViewNumericColumnProperties))]
//    [ProtoBuf.ProtoInclude(2, typeof(EbDataGridViewDateTimeColumnProperties))]
//    [ProtoBuf.ProtoInclude(3, typeof(EbDataGridViewBooleanColumnProperties))]
//    public class EbDataGridViewColumnProperties
//    {

//    }

//    [ProtoBuf.ProtoContract]
//    public class EbDataGridViewBooleanColumnProperties : EbDataGridViewColumnProperties
//    {
//        [ProtoBuf.ProtoMember(1)]
//        public bool IsEditable { get; set; }
//    }

//    [ProtoBuf.ProtoContract]
//    public class EbDataGridViewNumericColumnProperties : EbDataGridViewColumnProperties
//    {
//        [ProtoBuf.ProtoMember(1)]
//        public int DecimalPlaces { get; set; }

//        [ProtoBuf.ProtoMember(2)]
//        [Description("Comma/delimeter separated localized display of number/value.")]
//        public bool Localize { get; set; }

//        [ProtoBuf.ProtoMember(3)]
//        public bool IsCurrency { get; set; }

//        [ProtoBuf.ProtoMember(4)]
//        public bool Sum { get; set; }

//        [ProtoBuf.ProtoMember(5)]
//        public bool Average { get; set; }

//        [ProtoBuf.ProtoMember(6)]
//        public bool Max { get; set; }

//        [ProtoBuf.ProtoMember(7)]
//        public bool Min { get; set; }

//        [ProtoBuf.ProtoMember(8)]
//        public bool ShowProgressbar { get; set; }
//    }

//    [ProtoBuf.ProtoContract]
//    public class EbDataGridViewDateTimeColumnProperties : EbDataGridViewColumnProperties
//    {
//    }

//    [ProtoBuf.ProtoContract]
//    public class EbDataGridViewColumnCollection : ObservableCollection<EbDataGridViewColumn>
//    {
//        internal ColumnColletion ColumnColletion { get; set; }

//        internal bool SerialColumnAdded { get; set; }
//        internal bool CheckBoxColumnAdded { get; set; }
//        internal bool EbVoidColumnAdded { get; set; }
//        internal bool EbLineGraphColumnAdded { get; set; }
//        internal bool EbLockColumnAdded { get; set; }
//        internal bool EbToggleColumnAdded { get; set; }

//        internal int ActualCount
//        {
//            get
//            {
//                return this.Count + (SerialColumnAdded ? 1 : 0) + (CheckBoxColumnAdded ? 1 : 0) + (EbVoidColumnAdded? 1:0) + (this.EbLineGraphColumnAdded ? 1: 0);
//            }
//        }

//        public EbDataGridViewColumn this[string columnName]
//        {
//            get
//            {
//                foreach (EbDataGridViewColumn col in this)
//                {
//                    if (col.Name == columnName)
//                        return col;
//                }

//                return null;
//            }
//        }

//        public bool Contains(string columnName)
//        {
//            foreach (EbDataGridViewColumn col in this)
//            {
//                if (col.Name == columnName)
//                    return true;
//            }

//            return false;
//        }

//        internal string GetColumnDefJs(string tableid, bool bHideSerial, bool bHideCheckBox)
//        {
//            string script = "[";

//            //if (!bHideSerial)
//            //    script += this.GetSerialColumnDefJs();

//            //if (!bHideCheckBox)
//            //    script += this.GetCheckBoxColumnDefJs(tableid);

//            foreach (EbDataGridViewColumn column in this)
//            {
//                //if (column.Name == "data_graph")
//                //    script += GetLineGraphColumnDefJs(column);
//                //else
//                    script += column.GetColumnDefJs(this.ColumnColletion);
//            }

//            //if (!this.Contains("sys_cancelled"))//change to eb_void
//            //{
//            //    script += GetEbVoidColumnDefJs();
//            //}
            
//            //if (!this.Contains("sys_locked"))//change to eb_lock
//            //{
//            //    script += GetEbLockColumnDefJs();
//            //}

//            //if (!this.Contains("sys_deleted"))//change to eb_lock
//            //{
//            //    script += GetEbToggleColumnDefJs();
//            //}
//            return script + "]";
//        }

//        private string GetSerialColumnDefJs()
//        {
//            this.SerialColumnAdded = true;
//            return "{ width:10, searchable: false, orderable: false ,targets: 0 },";
//        }

//        private string GetCheckBoxColumnDefJs(string tableid)
//        {
//            // className:'select-checkbox',
//            this.CheckBoxColumnAdded = true;
//            return "{ data: null, title: \"<input id='{0}_select-all' type='checkbox' onclick='clickAlSlct(event, this);' data-table='{0}'/>\"".Replace("{0}", tableid)
//                + ", width: 10, render: function( data2, type, row, meta ) { return renderCheckBoxCol({0}, {1}, '{0}', row,meta); }, orderable: false },"
//                .Replace("{0}", tableid)
//                .Replace("{1}", ColumnColletion["id"].ColumnIndex.ToString());
//        }

//        private string GetEbVoidColumnDefJs()
//        {
//            this.EbVoidColumnAdded = true;
//            return ("{data: {0}, title: \"<i class='fa fa-ban fa-1x' aria-hidden='true'></i><span hidden>sys_cancelled</span>\" "
//             + ", width: 10 , className:'dt-center', render: function( data2, type, row, meta ) { return renderEbVoidCol(data2); } },")
//             .Replace("{0}", ColumnColletion["sys_cancelled"].ColumnIndex.ToString());
            
//        }

//        private string GetLineGraphColumnDefJs(EbDataGridViewColumn column)//edit
//        {
//            this.EbLineGraphColumnAdded = true;
//            string script = "{ data: " + ColumnColletion[column.Name].ColumnIndex.ToString();
//            return script + @", width: 30, render: function(data2, type, row, meta) { return lineGraphDiv(data2, meta, '" + column.Name + "'); }, orderable: true, className:'linepadding' }, ";
//        }

//        private string GetEbLockColumnDefJs()
//        {
//            this.EbLockColumnAdded = true;
//            return ("{ data: {0}, title: \"<i class='fa fa-lock fa-1x' aria-hidden='true' ></i><span hidden>sys_locked</span>\" "
//                + ", width: 10, className:'dt-center', render: function( data2, type, row, meta ) { return renderLockCol(data2); } },")
//                .Replace("{0}", ColumnColletion["sys_locked"].ColumnIndex.ToString());
//        }

//        private string GetEbToggleColumnDefJs()
//        {
//            this.EbToggleColumnAdded = true;
//            return "{data: {0}, title:\"sys_deleted<span hidden>sys_deleted</span>\", width: 10, render: function( data2, type, row, meta ) { return renderToggleCol(data2); } },"
//                .Replace("{0}", ColumnColletion["sys_deleted"].ColumnIndex.ToString());
//        }

//        //private string GetColumnVisibility()
//        //{
//        //    foreach(EbDataGridViewColumn in this)
//        //    {

//        //    }
//        //}
//    }

//    [ProtoBuf.ProtoContract]
//    public class SampleColumn
//    {
//        public string name { get; set; }
//        public string title { get; set; }
//        public EbDataGridViewColumnType type { get; set; }
//        public int data { get; set; }
//        public bool visible { get; set; }
//        public int width { get; set; }
//    }
}
