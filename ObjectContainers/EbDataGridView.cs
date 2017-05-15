using ExpressBase.Data;
using ExpressBase.Objects.ServiceStack_Artifacts;
using ServiceStack;
using ServiceStack.Redis;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpressBase.Objects
{
    public enum enumPaging
    {
        numbers,
        simple,
        full,
        simple_numbers,
        full_numbers,
        first_last_numbers,
    }
    [ProtoBuf.ProtoContract]
    public class EbDataGridView : EbControlContainer
    {
        [ProtoBuf.ProtoMember(1)]
        public int DataSourceId { get; set; }

        [ProtoBuf.ProtoMember(2)]
        public int PageSize { get; set; }

        [ProtoBuf.ProtoMember(3)]
        public EbDataGridViewColumnCollection Columns { get; set; }

        [ProtoBuf.ProtoMember(4)]
        public bool HideCheckbox { get; set; }

        [ProtoBuf.ProtoMember(5)]
        public bool HideSerial { get; set; }

        [ProtoBuf.ProtoMember(6)]
        public int ScrollY { get; set; }

        [ProtoBuf.ProtoMember(7)]
        public enumPaging PagingType { get; set; }

        [ProtoBuf.ProtoMember(8)]
        public bool LoadOnDemand{ get; set; }

        [ProtoBuf.ProtoMember(9)]
        public int FilterDialogId { get; set; }

        internal ColumnColletion ColumnColletion { get; set; }

        public string GetCols()
        {
            return this.Columns.GetColumnDefJs(this.Name, this.HideSerial, this.HideCheckbox);
        }

        public string GetFilterControls()
        {
            List<string> _lsRet = new List<string>();

            if (this.Columns.SerialColumnAdded) _lsRet.Add("<th>&nbsp;</th>");
            if (this.Columns.CheckBoxColumnAdded) _lsRet.Add("<th>&nbsp;</th>");

            StringBuilder _ls = new StringBuilder();

            foreach (EbDataGridViewColumn column in this.Columns)
            {
                _ls.Clear();

                if (!column.Hidden)
                {
                    var span = string.Format("<span hidden>{0}</span>", column.Name);

                    string htext_class = string.Format("{0}_htext", this.Name);

                    string data_colum = string.Format("data-colum='{0}'", column.Name);
                    string data_table = string.Format("data-table='{0}'", this.Name);

                    string header_select = string.Format("{0}_{1}_hdr_sel", this.Name, column.Name);
                    string header_text1 = string.Format("{0}_{1}_hdr_txt1", this.Name, column.Name);
                    string header_text2 = string.Format("{0}_{1}_hdr_txt2", this.Name, column.Name);

                    _ls.Append("<th style='padding: 0px; margin: 0px'>");

                    if (column.ColumnType == EbDataGridViewColumnType.Numeric)
                        _ls.Append(span + getFilterForNumeric(header_text1, header_select, data_table, htext_class, data_colum, header_text2));
                    else if (column.ColumnType == EbDataGridViewColumnType.Text)
                        _ls.Append(span + getFilterForString(header_text1, header_select, data_table, htext_class, data_colum, header_text2));
                    else if (column.ColumnType == EbDataGridViewColumnType.DateTime)
                        _ls.Append(span + getFilterForDateTime(header_text1, header_select, data_table, htext_class, data_colum, header_text2));
                    else if (column.ColumnType == EbDataGridViewColumnType.Boolean)
                        _ls.Append(span + getFilterForBoolean(column.Name));
                    else
                        _ls.Append(span);

                    _ls.Append("</th>");
                }
                //else
                //    _ls.Append("<th style='display:none'></th>");

                _lsRet.Add(_ls.ToString());
            }
            if (this.Columns.EbVoidColumnAdded) _lsRet.Add("<th style='padding: 0px; margin: 0px; text-align:center;'>" + getFilterForBoolean("sys_cancelled")+ "</th>");
            if (this.Columns.EbLineGraphColumnAdded) _lsRet.Add("<th>&nbsp;</th>");
            if (this.Columns.EbLockColumnAdded) _lsRet.Add("<th style='padding: 0px; margin: 0px; text-align:center;'>" + getFilterForBoolean("sys_locked") + "</th>");
            //if (this.Columns.EbToggleColumnAdded) _lsRet.Add("<th style='padding: 0px; margin: 0px'>" + getFilterForBoolean("sys_deleted") + "</th>");
            _ls.Clear();
            _ls = null;

            return Newtonsoft.Json.JsonConvert.SerializeObject(_lsRet);
        }

        public string GetAggregateControls(int footer_id)
        {
            List<string> _ls = new List<string>();

            if (this.Columns.CheckBoxColumnAdded) _ls.Add("&nbsp;");
            if(this.Columns.SerialColumnAdded) _ls.Add("&nbsp;");
            foreach (EbDataGridViewColumn column in this.Columns)
            {
                if (!column.Hidden)
                {
                    var ext = column.ExtendedProperties as EbDataGridViewNumericColumnProperties;

                    string footer_select_id = string.Format("{0}_{1}_ftr_sel{2}", this.Name, column.Name, footer_id);
                    string fselect_class = string.Format("{0}_fselect", this.Name);

                    string data_colum = string.Format("data-column='{0}'", column.Name);
                    string data_table = string.Format("data-table='{0}'", this.Name);
                    string data_decip = string.Format("data-decip={0}", (ext != null) ? ext.DecimalPlaces : 0);

                    string footer_txt = string.Format("{0}_{1}_ftr_txt{2}", this.Name, column.Name, footer_id);

                    if (ext != null)
                    {
                        if (column.ColumnType == EbDataGridViewColumnType.Numeric)
                        {
                            if (ext.Sum || ext.Average)
                            {
                                _ls.Add(string.Format(@"
<div class='input-group'>
    <div class='input-group-btn'>
        <button type='button' class='btn btn-default dropdown-toggle' data-toggle='dropdown' id='{1}'>&sum;</button>
        <ul class='dropdown-menu'>
          <li ><a href ='#' onclick='fselect_func({6}, this,{5});' data-sum='Sum' {2} {3} {4}>&sum;</a></li>
          <li><a href ='#' onclick='fselect_func({6}, this,{5});' {2} {3} {4}>&mnplus;</a></li>
        </ul>
    </div>
    <input type='text' class='form-control' id='{0}' disabled style='text-align: right;'>
</div>", footer_txt, footer_select_id, data_table, data_colum, data_decip, this.ScrollY, this.Name));
                            }
                            else
                                _ls.Add("&nbsp;");
                        }
                        else
                            _ls.Add("&nbsp;");
                    }
                    else
                        _ls.Add("&nbsp;");
            }
                //else
                //    _ls.Add("ttttttt");
        }

            if (this.Columns.EbVoidColumnAdded) _ls.Add("&nbsp;");
            if (this.Columns.EbLineGraphColumnAdded) _ls.Add("&nbsp;");
            if (this.Columns.EbLockColumnAdded) _ls.Add("&nbsp;");
           // if (this.Columns.EbToggleColumnAdded) _ls.Add("&nbsp;");
            return Newtonsoft.Json.JsonConvert.SerializeObject(_ls);
        }
        
        public string GetAggregateInfo()
        {
            List<AggregateInfo> _ls = new List<AggregateInfo>();

            foreach (EbDataGridViewColumn column in this.Columns)
            {
                if (!column.Hidden)
                {
                    var ext = column.ExtendedProperties as EbDataGridViewNumericColumnProperties;

                    if (ext != null)
                    {
                        if (column.ColumnType == EbDataGridViewColumnType.Numeric && (ext.Sum || ext.Average))
                            _ls.Add(new AggregateInfo { colname = column.Name, coltype = "N", deci_val = ext.DecimalPlaces });
                    }
                }
            }

            return Newtonsoft.Json.JsonConvert.SerializeObject(_ls);
        }

        public EbDataGridView()
        {
            this.Columns = new EbDataGridViewColumnCollection();
            this.Columns.CollectionChanged += Columns_CollectionChanged;
        }

        public delegate void ColumnsChangedHandler(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e);
        public event ColumnsChangedHandler ColumnsChanged;
        private void Columns_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (ColumnsChanged != null)
                ColumnsChanged(sender, e);
        }

        //[[100, 500, 1000, 2500, 5000, -1], [100, 500, 1000, 2500, 5000, 'All']]
        private string GetLengthMenu()
        {
            string sLengthMenu = "paging: false";

            if (this.PageSize > 0)
            {
                int[] ia = new int[10];
                for (int i = 0; i < 10; i++)
                    ia[i] = (this.PageSize * (i + 1));

                sLengthMenu = "lengthMenu: " + string.Format("[[{0}, -1], [{0}, 'All']]", string.Join(", ", ia));
            }

            return sLengthMenu;
        }

        private string GetScrollYOption()
        {
            return (this.ScrollY > 0) ? string.Format("scrollY: '{0}', scrollX: true ", this.ScrollY) : "fixedHeader: { footer: true }";
        }

        private string GetSelectOption()
        {
            string __select = string.Empty;
            if (!this.HideCheckbox && !this.HideSerial)
                __select = "select: { style: 'os', selector: 'td:nth-child(1),td:nth-child(2)'}";
            else if(!this.HideSerial || !this.HideCheckbox)
                __select = "select: { style: 'os', selector: 'td:nth-child(1)'}";
            else 
                __select = "select: false";
            return __select;
        }

        public string GetFooter()
        {
            string ftr = string.Empty;
            ftr = "<tfoot>";
            if(this.Columns.CheckBoxColumnAdded)
                ftr += "<th style=\"padding: 0px; margin: 0px\"></th>";
            if(this.Columns.SerialColumnAdded)
                ftr += "<th style=\"padding: 0px; margin: 0px\"></th>";
           
            foreach (EbDataGridViewColumn colum in this.Columns)
            {
                if (!colum.Hidden)
                    ftr += "<th style=\"padding: 0px; margin: 0px\"></th>";
                else
                    ftr += "<th style=\"display:none;\"></th>";
            }
            if (this.Columns.EbVoidColumnAdded)
                ftr += "<th style=\"padding: 0px; margin: 0px\"></th>";
            if (this.Columns.EbLineGraphColumnAdded)
                ftr += "<th style=\"padding: 0px; margin: 0px\"></th>";
            if (this.Columns.EbLockColumnAdded)
                ftr += "<th style=\"padding: 0px; margin: 0px\"></th>";
            //if (this.Columns.EbToggleColumnAdded)
            //    ftr += "<th style=\"padding: 0px; margin: 0px\"></th>";

            ftr += "<tr>";
            if (this.Columns.CheckBoxColumnAdded)
                ftr += "<th style=\"padding: 0px; margin: 0px\"></th>";
            if (this.Columns.SerialColumnAdded)
                ftr += "<th style=\"padding: 0px; margin: 0px\"></th>";

            foreach (EbDataGridViewColumn colum in this.Columns)
            {
                if (!colum.Hidden)
                    ftr += "<th style=\"padding: 0px; margin: 0px\"></th>";
                else
                    ftr += "<th style=\"display:none;\"></th>";
            }
            if (this.Columns.EbVoidColumnAdded)
                ftr += "<th style=\"padding: 0px; margin: 0px\"></th>";
            if (this.Columns.EbLineGraphColumnAdded)
                ftr += "<th style=\"padding: 0px; margin: 0px\"></th>";
            if (this.Columns.EbLockColumnAdded)
                ftr += "<th style=\"padding: 0px; margin: 0px\"></th>";
            //if (this.Columns.EbToggleColumnAdded)
            //    ftr += "<th style=\"padding: 0px; margin: 0px\"></th>";
            ftr += "</tr>";
            ftr += "</tfoot>";
            return ftr;
        }

        public string getFilterForNumeric( string header_text1,string header_select, string data_table,string htext_class,string data_colum,string header_text2)
        {
            string coltype = "data-coltyp='numeric'";
            string drptext = string.Empty;
            drptext = string.Format(@"
<div class='input-group'>
    <div class='input-group-btn'>
        <button type='button' class='btn btn-default dropdown-toggle' data-toggle='dropdown' id='{4}'> = </button>
        <ul class='dropdown-menu'>
          <li ><a href ='#' onclick='setLiValue(this);' {1} {3}>=</a></li>
          <li><a href ='#' onclick='setLiValue(this);' {1} {3}><</a></li>
          <li><a href='#' onclick='setLiValue(this);' {1} {3}>></a></li>
          <li><a href='#' onclick='setLiValue(this);' {1} {3}><=</a></li>
          <li><a href='#' onclick='setLiValue(this);' {1} {3}>>=</a></li>
          <li ><a href='#' onclick='setLiValue(this);' {1} {3}>B</a></li>
        </ul>
    </div>
    <input type='number' class='form-control {2}' id='{0}' onkeypress='call_filter(event, this);' {1}  {3} {6}>
    <span class='input-group-btn'></span>
    <input type='number' class='form-control {2}' id='{5}' style='visibility: hidden' onkeypress='call_filter(event, this);' {1}  {3} {6}>
</div> ", header_text1, data_table, htext_class, data_colum, header_select, header_text2, coltype)
;
            return drptext;
        }

        public string getFilterForDateTime(string header_text1, string header_select, string data_table, string htext_class, string data_colum, string header_text2)
        {
            string coltype = "data-coltyp='date'";
            string filter = string.Format(@"
<div class='input-group'>
    <div class='input-group-btn'>
        <button type='button' class='btn btn-default dropdown-toggle' data-toggle='dropdown' id='{4}'> = </button>
        <ul class='dropdown-menu'>
          <li ><a href ='#' onclick='setLiValue(this);' {1} {3}>=</a></li>
          <li><a href ='#' onclick='setLiValue(this);' {1} {3}><</a></li>
          <li><a href='#' onclick='setLiValue(this);' {1} {3}>></a></li>
          <li><a href='#' onclick='setLiValue(this);' {1} {3}><=</a></li>
          <li><a href='#' onclick='setLiValue(this);' {1} {3}>>=</a></li>
          <li ><a href='#' onclick='setLiValue(this);' {1} {3}>B</a></li>
        </ul>
    </div>
    <input type='date' class='form-control {2}' id='{0}' onkeypress='call_filter(event, this);' {1}  {3} {6}>
    <span class='input-group-btn'></span>
    <input type='date' class='form-control {2}' id='{5}' style='visibility: hidden' onkeypress='call_filter(event, this);' {1}  {3} {6}>
</div> ", header_text1, data_table, htext_class, data_colum, header_select, header_text2, coltype);
            return filter;
        }

        public string getFilterForString(string header_text1, string header_select, string data_table, string htext_class, string data_colum, string header_text2)
        {
            string drptext = string.Empty;
            drptext = string.Format(@"
<div class='input-group'>
    <div class='input-group-btn'>
        <button type='button' class='btn btn-default dropdown-toggle' data-toggle='dropdown' id='{4}'>x*</button>
        <ul class='dropdown-menu'>
          <li ><a href ='#' onclick='setLiValue(this);' {1} {3}>x*</a></li>
          <li><a href ='#' onclick='setLiValue(this);' {1} {3}>*x</a></li>
          <li><a href='#' onclick='setLiValue(this);' {1} {3}>*x*</a></li>
          <li><a href='#' onclick='setLiValue(this);' {1} {3}>=</a></li>
        </ul>
    </div>
    <input type='text' class='form-control {2}' id='{0}' onkeypress='call_filter(event, this);' {1}  {3}>
</div> ", header_text1, data_table, htext_class, data_colum, header_select, header_text2)
;
            return drptext;
        }

        public string getFilterForBoolean(string colum)
        {
            var filter = string.Empty;
            string id = this.Name+"_"+ colum + "_hdr_txt1";
            string cls = this.Name + "_hchk";
            filter = string.Format("<input type='checkbox' id='{1}' data-colum='{0}' onchange='toggleInFilter(this);' data-coltyp='boolean' data-table='{2}' class='{3} {2}_htext'>", colum, id, this.Name, cls);
            return filter;
        }

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
            colDef = "{\"hideSerial\": false, \"hideCheckbox\": false, \"lengthMenu\":[ [100, 200, 300, -1], [100, 200, 300, \"All\"] ],";
            colDef+=" \"scrollY\":300, \"rowGrouping\":\"\",\"leftFixedColumns\":0,\"rightFixedColumns\":0,\"columns\":[";
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
                var cls = (column.Type.ToString() == "System.Boolean") ? "dt-center tdheight" : "tdheight";
                colDef += ",\"className\": \""+cls+"\"";
                colDef += "},";
            }
            colDef = colDef.Substring(0 , colDef.Length - 1) +"],";
            string colext = "\"columnsext\":[";
            foreach (EbDataColumn column in __columnCollection)
            {
                colext += "{";
                if (column.Type.ToString() == "System.Int32" || column.Type.ToString() == "System.Decimal")
                    colext += "\"name\":\""+ column.ColumnName + "\",\"AggInfo\":true,\"DecimalPlace\":2,\"RenderAs\":\"Default\"";
                else if (column.Type.ToString() == "System.Boolean")
                    colext += "\"name\":\"" + column.ColumnName + "\",\"RenderAs\":\"Default\"";
                colext += "},";
            }
            colext = colext.Substring(0,colext.Length-1)+"]";
            return colDef + colext + "}";
        }

        public override string GetHtml()
        {
            this.Redis.Remove(string.Format("{0}_TVPref_{1}_uid_{2}", "eb_roby_dev", this.Id, 1));
            this.ColumnColletion = this.Redis.Get<ColumnColletion>(string.Format("{0}_ds_{1}_columns", "eb_roby_dev", this.DataSourceId));
            tvPref4User = this.Redis.Get<string>(string.Format("{0}_TVPref_{1}_uid_{2}", "eb_roby_dev", this.Id, 1));
            if (string.IsNullOrEmpty(tvPref4User))
            {
                tvPref4User = this.GetColumn4DataTable(this.ColumnColletion);
                this.Redis.Set(string.Format("{0}_TVPref_{1}_uid_{2}", "eb_roby_dev", this.Id, 1), tvPref4User);
            }
            this.Columns.ColumnColletion = this.ColumnColletion;


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
.DTFC_RightHeadWrapper{z-index: 150;}
.DTFC_RightBodyWrapper{z-index: 100;}
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
</style>
<div class='tablecontainer' id='@tableId_container'>
    <div>
        @tableViewName
    </div>
    <div>
        <div class='btn-group' id='@tableId_filterdiv'>
            <a class='btn btn-default' onclick='showOrHideFilter(this,@scrolly);' name='filterbtn' style='display: none;' data-table='@tableId' data-toggle='tooltip' title='On\/Off Filter'><i class='fa fa-filter' aria-hidden='true'></i></a>
            <button type='button' class='btn btn-default dropdown-toggle' data-toggle='dropdown' name='filterbtn' style='display: none;'>
            <span class='caret'></span>  <!-- caret --></button>
            <ul class='dropdown-menu' role='menu'>
                <li><a href = '#' onclick= clearFilter('@tableId')> Clear Filter</a></li>
            </ul>
        </div>
        <button type='button' id='@tableId_btntotalpage' class='btn btn-default' style='display: none;' onClick='showOrHideAggrControl(this,@scrolly);' data-table='@tableId'>&sum;</button>
        <div id='btnGo' class='btn btn-default' >GO</div>
        <div id='@tableId_fileBtns' style='display: inline-block;'>
            <div id='btnCopy' class='btn btn-default'  name='filebtn' style='display: none;' data-toggle='tooltip' title='Copy to Clipboard' onclick= CopyToClipboard('@tableId') ><i class='fa fa-clipboard' aria-hidden='true'></i></div>
            <div class='btn-group'>
                <div id='btnPrint' class='btn btn-default'  name='filebtn' style='display: none;'  data-toggle='tooltip' title='Print' onclick= ExportToPrint('@tableId')><i class='fa fa-print' aria-hidden='true'></i></div>
                    <div class='btn btn-default dropdown-toggle' data-toggle='dropdown' name='filebtn' style='display: none;'>
                        <span class='caret'></span>  <!-- caret --></div>
                        <ul class='dropdown-menu' role='menu'>
                            <li><a href = '#' onclick= printAll('@tableId')> Print All</a></li>
                            <li><a href = '#' onclick= printSelected('@tableId')> Print Selected</a></li>
                        </ul>
            </div>
            <div id='btnExcel' class='btn btn-default'  name='filebtn' style='display: none;' data-toggle='tooltip' title='Excel' onclick= ExportToExcel('@tableId')><i class='fa fa-file-excel-o' aria-hidden='true'></i></div>
            <div id='btnPdf' class='btn btn-default'    name='filebtn' style='display: none;'  data-toggle='tooltip' title='Pdf' onclick= ExportToPdf('@tableId')><i class='fa fa-file-pdf-o' aria-hidden='true'></i></div>
            <div id='btnCsv' class='btn btn-default'    name='filebtn' style='display: none;' data-toggle='tooltip' title='Csv' onclick= ExportToCsv('@tableId')><i class='fa fa-file-text-o' aria-hidden='true'></i></div>
        </div>
        @collapsBtn
        <div id='@tableId_btnSettings' class='btn btn-default' style='display: none;' data-toggle='modal' data-target='#settingsmodal' onclick=GetSettingsModal('@tableId',@tableViewId,'@tableViewName')><i class='fa fa-cog' aria-hidden='true'></i></div>
    </div>
    <div style='width:auto;' id='@tableId_divcont'>
        @filters  
        <table id='@tableId' class='table table-striped table-bordered'></table>
    </div>
</div>
   <!-- Modal for Graph-->
  <!-- <div class='modal fade' id='graphmodal' role='dialog'>
    <div class='modal-dialog modal-lg'>
      <div class='modal-content'>
        <div class='modal-header'>
          <button type = 'button' class='close' data-dismiss='modal'>&times;</button>
          <h4 class='modal-title'><center>Graph</center></h4>
        </div>
        <div class='modal-body'>
            <div id='$$$$$$$_canvasDiv' class='dygraph-Wrapper'>
                <div id='graphdiv' style='width:100%;height:500px;'></div>
            </div>  
        </div>
     </div>
    </div>
 </div> -->

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


var flag=true;
var DtF = true;
var @tableId;
$('#btnGo').click(function(){
    if(DtF){
        DtF = false;
        var tx = @tvPref4User;
        initTable_@tableId(tx);
        $('#filterBox').collapse('hide');
    }
    else
        @tableId.ajax.reload();
});

var @tableId__datacolumns = @data.columns;

function initTable_@tableId(tx){
    alert(JSON.stringify(tx));
    AddSerialAndOrCheckBoxColumns(tx, '@tableId', @tableId__datacolumns);
    @tableId_tvPref4User=tx.columns;
    $('#@tableId').append( $(getFooterFromSettingsTbl(@tableId_tvPref4User)) );

    var @tableId_ids=[];
    var @tableId_order_info = new Object();
    @tableId_order_info.col = '';
    @tableId_order_info.dir = 0;
    var @tableId__columns = @tableId_tvPref4User;
    var @tableId__eb_agginfo = getAgginfo(@tableId_tvPref4User);
    @tableId= $('#@tableId').DataTable(
    {
        dom:'<\'col-sm-2\'l><\'col-sm-2\'i><\'col-sm-4\'B><\'col-sm-4\'p>tr',
        buttons: ['copy', 'csv', 'excel', 'pdf','print', {
                        extend: 'print',
                        exportOptions: {
                            modifier: {
                                selected: true
                            }}}],
        scrollY: tx.scrollY,
        scrollX: true,
        fixedColumns: {
            leftColumns: tx.leftFixedColumns,
            rightColumns:tx.rightFixedColumns
            },
        //@scrollYOption,
        //scroller:true,
        //responsive:true,
        keys: true,
        //autoWidth: false,
        lengthMenu: tx.lengthMenu,
        serverSide: true,
        processing:true,
        language: { processing: '<div class=\'fa fa-spinner fa-pulse  fa-3x fa-fw\'></div>',
                    info:'_START_ - _END_ / _TOTAL_'},
        pagingType:'@pagingType',
        columns:@tableId__columns, 
        order: [],
        deferRender: true,
        filter: true,
        select:true,
        //@selectOption,$.fn.dataTable.pipeline(        pages: 5,)
        retrieve: true,
        ajax: {
            url: '@servicestack_url/ds/data/@dataSourceId',
            type: 'POST',
            timeout: 180000,
            data: function(dq) { 
                delete dq.columns; delete dq.order; delete dq.search;
                dq.Id = @dataSourceId;
                dq.Token = getToken();
                dq.TFilters = JSON.stringify(repopulate_filter_arr('@tableId'));
                dq.Params = JSON.stringify(getFilterValues());
                dq.OrderByCol = @tableId_order_info.col; 
                dq.OrderByDir = @tableId_order_info.dir;
            },
            dataSrc: function(dd) {
                    return dd.data;
            }
        },
        
        fnRowCallback: function( nRow, aData, iDisplayIndex, iDisplayIndexFull ) {
            colorRow(nRow, aData, iDisplayIndex, iDisplayIndexFull, @data.columns);
        },

        fnFooterCallback: function ( nRow, aaData, iStart, iEnd, aiDisplay ) {
            summarize2(@tableId, '@tableId', @tableId__eb_agginfo,@scrolly);
        },
        drawCallback:function ( settings ) {
            $('tbody [data-toggle=toggle]').bootstrapToggle();
            if(tx.rowGrouping !== ''){
                doRowgrouping(@tableId,tx);
            }
            //@tableId.columns.adjust();
        },
        initComplete:function ( settings,json ) {
            createFilterRowHeader('@tableId', @tableId_tvPref4User, @scrolly, @tableId_order_info,tx);
            //@tableId.columns.adjust();
        },
       
    });

    $.fn.dataTable.ext.errMode = 'throw';

    jQuery.fn.dataTable.Api.register( 'sum()', function ( ) {
        return this.flatten().reduce( function ( a, b ) {
            if ( typeof a === 'string' ) {
                a = a.replace(/[^\d.-]/g, '') * 1;
            }
            if ( typeof b === 'string' ) {
                b = b.replace(/[^\d.-]/g, '') * 1;
            }
 
            return a + b;
        }, 0 );
    } );

    jQuery.fn.dataTable.Api.register( 'average()', function () {
        var data = this.flatten();
        var sum = data.reduce( function ( a, b ) {
            return (a*1) + (b*1); // cast values in-case they are strings
        }, 0 );
  
        return sum / data.length;
    });

    if( @tableId__eb_agginfo.length>0 ) {
        createFooter('@tableId', GetAggregateControls(@tableId_tvPref4User,'@tableId',1,@scrolly,@tableId), @scrolly, 0);
        createFooter('@tableId', GetAggregateControls(@tableId_tvPref4User,'@tableId',2,@scrolly,@tableId), @scrolly, 1);
    }

        
    $('#@tableId_fileBtns [name=filebtn]').css('display', 'inline-block');
    $('#@tableId_filterdiv [name=filterbtn]').css('display', 'inline-block');
    $('#@tableId_btnSettings').css('display', 'inline-block');

    //if(@tableId_tvPref4User === null)
    //    createFilterRowHeader('@tableId', @eb_filter_controls, @scrolly);
    //else
       // createFilterRowHeader('@tableId', GetFiltersFromSettingsTbl(@tableId_tvPref4User,'@tableId'), @scrolly);

    //$('#@tableId_container thead').on('click','th',function(){
    //    var col = $(this).children('span').text();
    //    var dir = $(this).attr('aria-sort');
    //    if(col !== '') {
    //        @tableId_order_info.col = col;
    //        @tableId_order_info.dir = (dir === 'undefined') ? 1 : ((dir === 'ascending') ? 2 : 1);
    //    }
    //});

    if(!tx.hideSerial){
        @tableId.on( 'draw.dt', function () {
            @tableId.column(0).nodes().each( function (cell, i) {
                cell.innerHTML = i+1;
            } );
        } );
    }
        
    //new ResizeSensor(jQuery('#@tableId_container'), function() {
    //    if ( $.fn.dataTable.isDataTable( '#@tableId' ) )
    //        @tableId.columns.adjust();
    //});

}    
</script>"
.Replace("@dataSourceId", this.DataSourceId.ToString().Trim())
.Replace("@tableId", this.Name)
.Replace("@tableViewName", ((string.IsNullOrEmpty(this.Label)) ? "&lt;ReportLabel Undefined&gt;" : this.Label))
//.Replace("@lengthMenu", this.GetLengthMenu())
//.Replace("@columnsRender", this.GetCols())
//.Replace("@eb_filter_controls", this.GetFilterControls())
//.Replace("@eb_footer1", this.GetAggregateControls(1))
//.Replace("@eb_footer2", this.GetAggregateControls(2))
//.Replace("@eb_agginfo", this.GetAggregateInfo())
//.Replace("@bserial", this.Columns.SerialColumnAdded.ToString().ToLower())
.Replace("@scrolly", this.ScrollY.ToString())
.Replace("@scrollYOption", this.GetScrollYOption())
//.Replace("@tfoot", this.GetFooter())
.Replace("@pagingType", this.PagingType.ToString())
.Replace("@servicestack_url", "https://expressbaseservicestack.azurewebsites.net")
.Replace("@filters", this.filters)
.Replace("@FilterBH", this.FilterBH.ToString())
.Replace("@collapsBtn", (this.__filterForm != null) ? @"<div id = 'btnCollapse' class='btn btn-default' data-toggle='collapse' data-target='#filterBox' aria-expanded='true' aria-controls='filterBox'>
                    <i class='fa fa-chevron-down' aria-hidden='true'></i>
                </div>" : string.Empty)
.Replace("@selectOption", this.GetSelectOption())
.Replace("@data.columns", this.ColumnColletion.ToJson())
.Replace("@tableViewId", this.Id.ToString())
.Replace("@tvPref4User", tvPref4User);
        }
    }

    public class AggregateInfo
    {
        public string colname { get; set; }
        public string coltype { get; set; }
        public int deci_val{ get; set;}
    }

    [ProtoBuf.ProtoContract]
    public class EbDataGridViewColumn : EbControl
    {
        private EbDataGridViewColumnType _columnType = EbDataGridViewColumnType.Null;

        [ProtoBuf.ProtoMember(1)]
        public EbDataGridViewColumnType ColumnType
        {
            get { return _columnType; }
            set
            {
                if (value == EbDataGridViewColumnType.Numeric)
                    this.ExtendedProperties = new EbDataGridViewNumericColumnProperties();
                else if (value == EbDataGridViewColumnType.DateTime)
                    this.ExtendedProperties = new EbDataGridViewDateTimeColumnProperties();
                else if (value == EbDataGridViewColumnType.Boolean)
                    this.ExtendedProperties = new EbDataGridViewBooleanColumnProperties();
                else if(value == EbDataGridViewColumnType.Text)
                    this.ExtendedProperties = new EbDataGridViewColumnProperties();

                _columnType = value;
            }
        }

        [ProtoBuf.ProtoMember(2)]
#if NET462
        [TypeConverter(typeof(ExpandableObjectConverter))]
#endif
        public EbDataGridViewColumnProperties ExtendedProperties { get; set; }

        public EbDataGridViewColumn()
        {
            this.Width = 100;
        }

        public string GetColumnDefJs(ColumnColletion ColumnColletion)
        {
            string script = "{";
            script += "data: " + ColumnColletion[this.Name].ColumnIndex.ToString();
            script += string.Format(",title: '{0}<span hidden>{1}</span>'", (this.Label != null) ? this.Label : this.Name, this.Name);
            script += ",className: '" + this.GetClassName() + "'";
            script += ",visible: " + (!this.Hidden).ToString().ToLower();
            script += ",width: " + this.Width.ToString();
            script += this.GetRenderFunc();
            script += ",name: '" + this.Name + "'";
            script += "},";

            return script;
        }

        private string GetClassName()
        {
            string _c = string.Empty;

            if (this.ColumnType == EbDataGridViewColumnType.Boolean)
                _c = "dt-body-center";
            else if (this.ColumnType == EbDataGridViewColumnType.Numeric)
                _c = "dt-body-right";
            else
                _c = "dt-body-left";

            return _c;
        }

        private string GetRenderFunc(){
            string _r = string.Empty;
            string _fwrapper = ", render: function( data, type, row, meta ) { {0} }";

            if (this.ColumnType == EbDataGridViewColumnType.Numeric)
            {
                var ext = this.ExtendedProperties as EbDataGridViewNumericColumnProperties;

                if (ext.ShowProgressbar)
                    _r = "return renderProgressCol(data);";
                else
                {
                    if (ext != null)
                    {
                        var deci_places = (ext.DecimalPlaces > 0) ? ext.DecimalPlaces : 2;

                        if (!ext.Localize)
                            _r = string.Format("return parseFloat(data).toFixed({0});", deci_places);
                        else
                        {
                            if (!ext.IsCurrency)
                                _r = "return parseFloat(data).toLocaleString('en-US', { maximumSignificantDigits: {0} });".Replace("{0}", deci_places.ToString());
                            else
                                _r = "return parseFloat(data).toLocaleString('en-US', { style: 'currency', currency: 'USD', maximumSignificantDigits: {0} });".Replace("{0}", deci_places.ToString());
                        }
                    }
                    else
                        _r = "return data;";
                }

                _r = _fwrapper.Replace("{0}", _r);
            }
            else if (this.ColumnType == EbDataGridViewColumnType.DateTime)
                _r = _fwrapper.Replace("{0}", "return moment.unix(data).format('MM/DD/YYYY');");
            else if (this.ColumnType == EbDataGridViewColumnType.Null)
            {
                var ext = this.ExtendedProperties as EbDataGridViewBooleanColumnProperties;
                _r = _fwrapper.Replace("{0}", "return renderToggleCol(data,@ext);".Replace("@ext", ext.IsEditable.ToString().ToLower()));
            }
            else if (this.ColumnType == EbDataGridViewColumnType.Chart)
                _r = _fwrapper.Replace("{0}", "return lineGraphDiv(data);");
            else
                _r = string.Empty; // _fwrapper.Replace("{0}", "return data;");

            return _r;
        }
    }

    [ProtoBuf.ProtoContract]
    [ProtoBuf.ProtoInclude(1, typeof(EbDataGridViewNumericColumnProperties))]
    [ProtoBuf.ProtoInclude(2, typeof(EbDataGridViewDateTimeColumnProperties))]
    [ProtoBuf.ProtoInclude(3, typeof(EbDataGridViewBooleanColumnProperties))]
    public class EbDataGridViewColumnProperties
    {

    }

    [ProtoBuf.ProtoContract]
    public class EbDataGridViewBooleanColumnProperties : EbDataGridViewColumnProperties
    {
        [ProtoBuf.ProtoMember(1)]
        public bool IsEditable { get; set; }
    }

    [ProtoBuf.ProtoContract]
    public class EbDataGridViewNumericColumnProperties : EbDataGridViewColumnProperties
    {
        [ProtoBuf.ProtoMember(1)]
        public int DecimalPlaces { get; set; }

        [ProtoBuf.ProtoMember(2)]
        [Description("Comma/delimeter separated localized display of number/value.")]
        public bool Localize { get; set; }

        [ProtoBuf.ProtoMember(3)]
        public bool IsCurrency { get; set; }

        [ProtoBuf.ProtoMember(4)]
        public bool Sum { get; set; }

        [ProtoBuf.ProtoMember(5)]
        public bool Average { get; set; }

        [ProtoBuf.ProtoMember(6)]
        public bool Max { get; set; }

        [ProtoBuf.ProtoMember(7)]
        public bool Min { get; set; }

        [ProtoBuf.ProtoMember(8)]
        public bool ShowProgressbar { get; set; }
    }

    [ProtoBuf.ProtoContract]
    public class EbDataGridViewDateTimeColumnProperties : EbDataGridViewColumnProperties
    {
    }

    [ProtoBuf.ProtoContract]
    public class EbDataGridViewColumnCollection : ObservableCollection<EbDataGridViewColumn>
    {
        internal ColumnColletion ColumnColletion { get; set; }

        internal bool SerialColumnAdded { get; set; }
        internal bool CheckBoxColumnAdded { get; set; }
        internal bool EbVoidColumnAdded { get; set; }
        internal bool EbLineGraphColumnAdded { get; set; }
        internal bool EbLockColumnAdded { get; set; }
        internal bool EbToggleColumnAdded { get; set; }

        internal int ActualCount
        {
            get
            {
                return this.Count + (SerialColumnAdded ? 1 : 0) + (CheckBoxColumnAdded ? 1 : 0) + (EbVoidColumnAdded? 1:0) + (this.EbLineGraphColumnAdded ? 1: 0);
            }
        }

        public EbDataGridViewColumn this[string columnName]
        {
            get
            {
                foreach (EbDataGridViewColumn col in this)
                {
                    if (col.Name == columnName)
                        return col;
                }

                return null;
            }
        }

        public bool Contains(string columnName)
        {
            foreach (EbDataGridViewColumn col in this)
            {
                if (col.Name == columnName)
                    return true;
            }

            return false;
        }

        internal string GetColumnDefJs(string tableid, bool bHideSerial, bool bHideCheckBox)
        {
            string script = "[";

            //if (!bHideSerial)
            //    script += this.GetSerialColumnDefJs();

            //if (!bHideCheckBox)
            //    script += this.GetCheckBoxColumnDefJs(tableid);

            foreach (EbDataGridViewColumn column in this)
            {
                //if (column.Name == "data_graph")
                //    script += GetLineGraphColumnDefJs(column);
                //else
                    script += column.GetColumnDefJs(this.ColumnColletion);
            }

            //if (!this.Contains("sys_cancelled"))//change to eb_void
            //{
            //    script += GetEbVoidColumnDefJs();
            //}
            
            //if (!this.Contains("sys_locked"))//change to eb_lock
            //{
            //    script += GetEbLockColumnDefJs();
            //}

            //if (!this.Contains("sys_deleted"))//change to eb_lock
            //{
            //    script += GetEbToggleColumnDefJs();
            //}
            return script + "]";
        }

        private string GetSerialColumnDefJs()
        {
            this.SerialColumnAdded = true;
            return "{ width:10, searchable: false, orderable: false ,targets: 0 },";
        }

        private string GetCheckBoxColumnDefJs(string tableid)
        {
            // className:'select-checkbox',
            this.CheckBoxColumnAdded = true;
            return "{ data: null, title: \"<input id='{0}_select-all' type='checkbox' onclick='clickAlSlct(event, this);' data-table='{0}'/>\"".Replace("{0}", tableid)
                + ", width: 10, render: function( data2, type, row, meta ) { return renderCheckBoxCol({0}, {1}, '{0}', row,meta); }, orderable: false },"
                .Replace("{0}", tableid)
                .Replace("{1}", ColumnColletion["id"].ColumnIndex.ToString());
        }

        private string GetEbVoidColumnDefJs()
        {
            this.EbVoidColumnAdded = true;
            return ("{data: {0}, title: \"<i class='fa fa-ban fa-1x' aria-hidden='true'></i><span hidden>sys_cancelled</span>\" "
             + ", width: 10 , className:'dt-center', render: function( data2, type, row, meta ) { return renderEbVoidCol(data2); } },")
             .Replace("{0}", ColumnColletion["sys_cancelled"].ColumnIndex.ToString());
            
        }

        private string GetLineGraphColumnDefJs(EbDataGridViewColumn column)//edit
        {
            this.EbLineGraphColumnAdded = true;
            string script = "{ data: " + ColumnColletion[column.Name].ColumnIndex.ToString();
            return script + @", width: 30, render: function(data2, type, row, meta) { return lineGraphDiv(data2, meta, '" + column.Name + "'); }, orderable: true, className:'linepadding' }, ";
        }

        private string GetEbLockColumnDefJs()
        {
            this.EbLockColumnAdded = true;
            return ("{ data: {0}, title: \"<i class='fa fa-lock fa-1x' aria-hidden='true' ></i><span hidden>sys_locked</span>\" "
                + ", width: 10, className:'dt-center', render: function( data2, type, row, meta ) { return renderLockCol(data2); } },")
                .Replace("{0}", ColumnColletion["sys_locked"].ColumnIndex.ToString());
        }

        private string GetEbToggleColumnDefJs()
        {
            this.EbToggleColumnAdded = true;
            return "{data: {0}, title:\"sys_deleted<span hidden>sys_deleted</span>\", width: 10, render: function( data2, type, row, meta ) { return renderToggleCol(data2); } },"
                .Replace("{0}", ColumnColletion["sys_deleted"].ColumnIndex.ToString());
        }

        //private string GetColumnVisibility()
        //{
        //    foreach(EbDataGridViewColumn in this)
        //    {

        //    }
        //}
    }

    [ProtoBuf.ProtoContract]
    public class SampleColumn
    {
        public string name { get; set; }
        public string title { get; set; }
        public EbDataGridViewColumnType type { get; set; }
        public int data { get; set; }
        public bool visible { get; set; }
        public int width { get; set; }
    }
}
