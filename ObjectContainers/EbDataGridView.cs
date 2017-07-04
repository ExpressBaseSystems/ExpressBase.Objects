using ExpressBase.Data;
using ExpressBase.Objects.ServiceStack_Artifacts;
using ServiceStack;
using ServiceStack.Redis;
using System.Collections.Generic;

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
            return string.Empty;
        }
        
        private int FilterBH = 0;

        private EbForm __filterForm;   

        public void SetFilterForm(EbFilterDialog filterForm)    
        {
            var ControlColl = filterForm.FilterDialogJson.FromJson<List<EbTextBox>>();
            string rs = "";
            if (filterForm != null)
            {
                rs = @"<div class='collapse collapse in' style='margin-top:10px;' id='filterBox'>
                                <div class='well well-sm'>";
                foreach (EbControl c in ControlColl)
                {
                    rs += c.GetHtml();
                }
                rs += @"</div></div>";
            }
            this.filters = rs;


        }

        private string filters;
    //    {
    //        get
    //        {
    //            string rs = "";
    //    int max = 0;
    //            if(this.__filterForm != null)
    //            {
    //                rs = @"<div class='collapse collapse in' style='margin-top:10px;' id='filterBox'>
    //                            <div class='well well-sm' style='position:relative; height:@FilterBHpx; padding-top:40px;padding-bottom:40px;'>";
    //                foreach (EbControl c in this.__filterForm.Controls)
    //                {
    //                    if (c.Top >= max)
    //                    {
    //                        max = (c.Top + c.Height);
    //                    }
    //c.Top += 10;
    //                    rs += c.GetHtml();
    //                }
    //                this.FilterBH += max;
    //                rs += @"</div></div>";
    //            }
                
    //            return rs;
    //        } 
            
    //    }

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
   
        
        <label> <span class='icon-stack fa-3x'>
                    <i class='fa fa-area-chart icon-stack-3x'></i>
                    <i class='fa fa-table icon-stack-2x'></i>
                </span> &nbsp;&nbsp; 
                @dvname
        </label>
         <ul class='nav nav-tabs' id='table_tabs'>
                <li class='nav-item active'>
                    <a class='nav-link' href='#@tableId_tab_1' data-toggle='tab'><i class='fa fa-home' aria-hidden='true'></i>&nbsp; Home</a>
                </li>
         </ul></br>
         <div class='tab-content' id='table_tabcontent'>
             <div id='@tableId_tab_1' class='tab-pane active'>
                 <div id='TableControls_@tableId_1' class = 'well well-sm'>
                    <div id='btnGo' class='btn btn-primary' >GO</div>
                    @collapsBtn
                </div>
                @filters  
                <div style='width:auto;' id='@tableId_1divcont'>
                    <table id='@tableId_1' class='table table-striped table-bordered'></table>
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
.Replace("@collapsBtn", (this.__filterForm != null) ? @"<div id = 'btnCollapse' class='btn btn-default' data-toggle='collapse' data-target='#filterBox' aria-expanded='true' aria-controls='filterBox'>
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
