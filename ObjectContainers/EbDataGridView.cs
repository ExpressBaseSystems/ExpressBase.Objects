using ExpressBase.Data;
using ServiceStack;
using ServiceStack.Redis;

namespace ExpressBase.Objects
{
    [ProtoBuf.ProtoContract]
    public class EbDataGridView : EbControlContainer
    {
        [ProtoBuf.ProtoMember(1)]
        public int DataSourceId { get; set; }

        [ProtoBuf.ProtoMember(9)]
        public int FilterDialogId { get; set; }

        internal ColumnColletion ColumnColletion { get; set; }

        public EbDataGridView() { }

        public override string GetHead()
        {
            return string.Empty;
        }
        
        private int FilterBH = 0;

        private EbForm __filterForm;

        public void SetFilterForm(EbForm filterForm)
        {
            this.__filterForm = filterForm;
        }

        private string filters
        {
            get
            {
                string rs = "";
                int max=0;
                if(this.__filterForm != null)
                {
                    rs = @"<div class='collapse collapse in' style='margin-top:10px;' id='filterBox'>
                                <div class='well well-sm' style='position:relative; height:@FilterBHpx; padding-top:40px;padding-bottom:40px;'>";
                    foreach (EbControl c in this.__filterForm.Controls)
                    {
                        if (c.Top >= max)
                        {
                            max = (c.Top + c.Height);
                        }
                        c.Top += 10;
                        rs += c.GetHtml();
                    }
                    this.FilterBH += max;
                    rs += @"</div></div>";
                }
                
                return rs;
            }
        }

        public override void Init4Redis(IRedisClient redisclient, IServiceClient serviceclient)
        {
            base.Redis = redisclient;
            base.ServiceStackClient = serviceclient;
        }

        string tvPref4User = string.Empty;

        public string GetColumn4DataTable(ColumnColletion  __columnCollection)
        {
            string colDef = string.Empty;
            colDef = "{\"title\": \"<Untitled>\",\"hideSerial\": false, \"hideCheckbox\": false, \"lengthMenu\":[ [100, 200, 300, -1], [100, 200, 300, \"All\"] ],";
            colDef+=" \"scrollY\":300, \"rowGrouping\":\"\",\"leftFixedColumns\":0,\"rightFixedColumns\":0,\"columns\":[";
            colDef += "{\"width\":10, \"searchable\": false, \"orderable\": false, \"visible\":true, \"name\":\"serial\", \"title\":\"#\"},";
            colDef += "{\"width\":10, \"searchable\": false, \"orderable\": false, \"visible\":true, \"name\":\"checkbox\"},";
            foreach (EbDataColumn  column in __columnCollection)
            {
                colDef += "{";
                colDef += "\"data\": " + __columnCollection[column.ColumnName].ColumnIndex.ToString();
                colDef += string.Format(",\"title\": \"{0}<span hidden>{0}</span>\"", column.ColumnName);
                var vis = (column.ColumnName == "id") ? false.ToString().ToLower() : true.ToString().ToLower();
                colDef += ",\"visible\": " + vis;
                colDef += ",\"width\": " + 100;
                colDef += ",\"name\": \"" + column.ColumnName + "\"";
                colDef += ",\"type\": \"" + column.Type.ToString() + "\"";
                //var cls = (column.Type.ToString() == "System.Boolean") ? "dt-center tdheight" : "tdheight";
                colDef += ",\"className\": \"tdheight\"";
                colDef += "},";
            }
            colDef = colDef.Substring(0 , colDef.Length - 1) +"],";
            string colext = "\"columnsext\":[";
            colext += "{\"name\":\"serial\"},";
            colext += "{\"name\":\"checkbox\"},";
            foreach (EbDataColumn column in __columnCollection)
            {
                colext += "{";
                if (column.Type.ToString() == "System.Int32" || column.Type.ToString() == "System.Decimal" || column.Type.ToString() == "System.Int16" || column.Type.ToString() == "System.Int64")
                    colext += "\"name\":\"" + column.ColumnName + "\",\"AggInfo\":true,\"DecimalPlace\":2,\"RenderAs\":\"Default\"";
                else if (column.Type.ToString() == "System.Boolean")
                    colext += "\"name\":\"" + column.ColumnName + "\",\"IsEditable\":false,\"RenderAs\":\"Default\"";
                else if (column.Type.ToString() == "System.DateTime")
                    colext += "\"name\":\"" + column.ColumnName + "\",\"Format\":\"Date\"";
                else if (column.Type.ToString() == "System.String")
                    colext += "\"name\":\"" + column.ColumnName + "\",\"RenderAs\":\"Default\"";
                colext += "},";
            }
            colext = colext.Substring(0,colext.Length-1)+"]";
            return colDef + colext + "}";
        }

        public override string GetHtml()
        {
            //this.Redis.Remove(string.Format("{0}_TVPref_{1}_uid_{2}", "eb_roby_dev", this.DataSourceId, 1));
            this.ColumnColletion = this.Redis.Get<ColumnColletion>(string.Format("{0}_ds_{1}_columns", "eb_roby_dev", this.DataSourceId));
            tvPref4User = this.Redis.Get<string>(string.Format("{0}_TVPref_{1}_uid_{2}", "eb_roby_dev", this.DataSourceId, 1));
            if (string.IsNullOrEmpty(tvPref4User))
            {
                tvPref4User = this.GetColumn4DataTable(this.ColumnColletion);
                this.Redis.Set(string.Format("{0}_TVPref_{1}_uid_{2}", "eb_roby_dev", this.DataSourceId, 1), tvPref4User);
            }

            return @"
<style>
.tablecontainer {
    width:100%;
    height:auto;
    display:inline-block;
    padding:1px;
}
.loadingdiv {
    vertical-align:middle;
    margin: 5% 50%;
    display: none;
}
.numericcol{
    float:right;
}
.toolbar {
    float:left;
}

#@tableId th.resizing {
    cursor: e-resize;
}

td.resizer {
  position: absolute;
  top: 0;
  right: -8px;
  bottom: 0;
  left: auto;
  width: 16px;    
  cursor: e-resize;   
    background-color:red;    
}

.dataTables_scrollHead {padding-bottom: 250px; margin-bottom: -250px;}
.dataTables_scrollFoot{padding-bottom: 250px; margin-bottom: -250px;}

.DTFC_LeftHeadWrapper{z-index: 150;}
.DTFC_LeftBodyWrapper{z-index: 100;}
.DTFC_LeftFootWrapper{z-index: 150;}
.DTFC_RightHeadWrapper{z-index: 150;}
.DTFC_RightBodyWrapper{z-index: 100;}
.DTFC_RightFootWrapper{z-index: 150;}
.linepadding{
padding:0px!important;
}

.dt-center {text-align: center;}
.dt-buttons {visibility:hidden;}
th { font-size: 14px; }
td { font-size: 12px; }
.progress {
    margin-bottom: 0px !important;
}
.hideme {
  display:none;
}
 
.tdheight{
height:15px; 
white-space: nowrap;
}
table.dataTable tbody tr.selected, table.dataTable tbody th.selected, table.dataTable tbody td.selected {
    color: #090808;
}

</style>
<div class='tablecontainer' id='@tableId_container'>
    <div>
        @tableViewName
    </div>
    <div>
        <div class='btn-group' id='@tableId_filterdiv'>
            <a class='btn btn-default'  id='4filterbtn' name='filterbtn' style='display: none;' data-table='@tableId' data-toggle='tooltip' title='On\/Off Filter'><i class='fa fa-filter' aria-hidden='true'></i></a>
            <button type='button' class='btn btn-default dropdown-toggle' data-toggle='dropdown' name='filterbtn' style='display: none;'>
            <span class='caret'></span>  <!-- caret --></button>
            <ul class='dropdown-menu' role='menu'>
                <li><a href = '#' id='clearfilterbtn'> Clear Filter</a></li>
            </ul>
        </div>
        <button type='button' id='@tableId_btntotalpage' class='btn btn-default' style='display: none;' data-table='@tableId'>&sum;</button>
        <div id='btnGo' class='btn btn-default' >GO</div>
        <div id='@tableId_fileBtns' style='display: inline-block;'>
            <div id='btnCopy' class='btn btn-default'  name='filebtn' style='display: none;' data-toggle='tooltip' title='Copy to Clipboard' ><i class='fa fa-clipboard' aria-hidden='true'></i></div>
            <div class='btn-group'>
                <div id='btnPrint' class='btn btn-default'  name='filebtn' style='display: none;'  data-toggle='tooltip' title='Print' ><i class='fa fa-print' aria-hidden='true'></i></div>
                    <div class='btn btn-default dropdown-toggle' data-toggle='dropdown' name='filebtn' style='display: none;'>
                        <span class='caret'></span>  <!-- caret --></div>
                        <ul class='dropdown-menu' role='menu'>
                            <li><a href = '#' id='btnprintAll'> Print All</a></li>
                            <li><a href = '#' id='btnprintSelected'> Print Selected</a></li>
                        </ul>
            </div>
            <div id='btnExcel' class='btn btn-default'  name='filebtn' style='display: none;' data-toggle='tooltip' title='Excel' ><i class='fa fa-file-excel-o' aria-hidden='true'></i></div>
            <div id='btnPdf' class='btn btn-default'    name='filebtn' style='display: none;'  data-toggle='tooltip' title='Pdf' ><i class='fa fa-file-pdf-o' aria-hidden='true'></i></div>
            <div id='btnCsv' class='btn btn-default'    name='filebtn' style='display: none;' data-toggle='tooltip' title='Csv' ><i class='fa fa-file-text-o' aria-hidden='true'></i></div>
        </div>
        @collapsBtn
        <div id='@tableId_btnSettings' class='btn btn-default' style='display: none;' data-toggle='modal' data-target='#settingsmodal'><i class='fa fa-cog' aria-hidden='true'></i></div>
    </div>
    <div style='width:auto;' id='@tableId_divcont'>
        @filters  
        <table id='@tableId' class='table table-striped table-bordered'></table>
    </div>
</div>
<script>

//
// Pipelining function for DataTables. To be used to the `ajax` option of DataTables
//
//$.fn.dataTable.pipeline = function ( opts ) {
//    // Configuration options
//    var conf = $.extend( {
//        pages: 5,     // number of pages to cache
//        url: '',      // script url
//        data: null,   // function or object with parameters to send to the server
//                      // matching how `ajax.data` works in DataTables
//        method: 'POST' // Ajax HTTP method
//    }, opts );
 
//    // Private variables for storing the cache
//    var cacheLower = -1;
//    var cacheUpper = null;
//    var cacheLastRequest = null;
//    var cacheLastJson = null;
 
//    return function ( request, drawCallback, settings ) {
//        var ajax          = false;
//        var requestStart  = request.start;
//        var drawStart     = request.start;
//        var requestLength = request.length;
//        var requestEnd    = requestStart + requestLength;
         
//        if ( settings.clearCache ) {
//            // API requested that the cache be cleared
//            ajax = true;
//            settings.clearCache = false;
//        }
//        else if ( cacheLower < 0 || requestStart < cacheLower || requestEnd > cacheUpper ) {
//            // outside cached data - need to make a request
//            ajax = true;
//        }
//        else if ( JSON.stringify( request.order )   !== JSON.stringify( cacheLastRequest.order ) ||
//                  JSON.stringify( request.columns ) !== JSON.stringify( cacheLastRequest.columns ) ||
//                  JSON.stringify( request.search )  !== JSON.stringify( cacheLastRequest.search )
//        ) {
//            // properties changed (ordering, columns, searching)
//            ajax = true;
//        }
         
//        // Store the request for checking next time around
//        cacheLastRequest = $.extend( true, {}, request );
 
//        if ( ajax ) {
//            // Need data from the server
//            if ( requestStart < cacheLower ) {
//                requestStart = requestStart - (requestLength*(conf.pages-1));
 
//                if ( requestStart < 0 ) {
//                    requestStart = 0;
//                }
//            }
             
//            cacheLower = requestStart;
//            cacheUpper = requestStart + (requestLength * conf.pages);
 
//            request.start = requestStart;
//            request.length = requestLength*conf.pages;
 
//            // Provide the same `data` options as DataTables.
//            if ( $.isFunction ( conf.data ) ) {
//                // As a function it is executed with the data object as an arg
//                // for manipulation. If an object is returned, it is used as the
//                // data object to submit
//                var d = conf.data( request );
//                if ( d ) {
//                    $.extend( request, d );
//                }
//            }
//            else if ( $.isPlainObject( conf.data ) ) {
//                // As an object, the data given extends the default
//                $.extend( request, conf.data );
//            }
 
//            settings.jqXHR = $.ajax( {
//                'type':     conf.method,
//                'url':      conf.url,
//                'data':     request,
//                'dataType': 'json',
//                'cache':    false,
//                'success':  function(json) {
//                cacheLastJson = $.extend(true, { }, json);

//                if (cacheLower != drawStart)
//                {
//                    json.data.splice(0, drawStart - cacheLower);
//                }
//                if (requestLength >= -1)
//                {
//                    json.data.splice(requestLength, json.data.length);
//                }

//                drawCallback(json);
//            }
//        } );
//        }
//        else {
//            json = $.extend( true, { }, cacheLastJson );
//            json.draw = request.draw; // Update the echo for each response
//            json.data.splice( 0, requestStart-cacheLower );
//            json.data.splice( requestLength, json.data.length );


//            drawCallback(json);
//}
//    }
//};
 
//// Register an API method that will empty the pipelined data, forcing an Ajax
//// fetch on the next draw (i.e. `table.clearPipeline().draw()`)
//$.fn.dataTable.Api.register( 'clearPipeline()', function()
//{
//    return this.iterator('table', function(settings) {
//        settings.clearCache = true;
//    } );
//} );

var EbDataTable_@tableId = new EbDataTable(@dataSourceId, @dvId, '@servicestack_url', '@tableId', @tvPref4User);
</script>"
.Replace("@dataSourceId", this.DataSourceId.ToString().Trim())
.Replace("@tableId", this.Name)
.Replace("@tableViewName", ((string.IsNullOrEmpty(this.Label)) ? "&lt;ReportLabel Undefined&gt;" : this.Label))
.Replace("@servicestack_url", "https://expressbaseservicestack.azurewebsites.net")
.Replace("@filters", this.filters)
.Replace("@FilterBH", this.FilterBH.ToString())
.Replace("@collapsBtn", (this.__filterForm != null) ? @"<div id = 'btnCollapse' class='btn btn-default' data-toggle='collapse' data-target='#filterBox' aria-expanded='true' aria-controls='filterBox'>
                    <i class='fa fa-chevron-down' aria-hidden='true'></i>
                </div>" : string.Empty)
.Replace("@data.columns", this.ColumnColletion.ToJson())
.Replace("@dvId", this.Id.ToString())
.Replace("@tvPref4User", tvPref4User);
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
