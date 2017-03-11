using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpressBase.Objects
{
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
        public bool ShowSerial { get; set; }

        [ProtoBuf.ProtoMember(6)]
        public int ScrollY { get; set; }

        bool fl = false;
        bool serial = false;
        int colCount=0;

        //public string  GetJclGrip()
        //{
        //    string grip = string.Empty;
        //    int col = 10;
        //    grip = "<div class='JCLRgrips' style='width: 1300px; '>";
        //    foreach (EbDataGridViewColumn column in this.Columns)
        //    {
        //        col =col+column.Width + 1;
        //        grip += "<div class='JCLRgrip' style='left: "+col+"px; height: 200px; '><div class='JColResizer'></div></div>";
               
        //    }
        //    grip += "<div class='JCLRgrip JCLRLastGrip' style='left: 1301px; height: 81px; '></div></div>";
        //    return grip;
        //}

        public string GetCols()
        {
            string script = "[";
            if (this.ShowSerial)
            {
                serial = true;
                script += "{";
                script += "searchable: false";
                script += ",orderable: false";
                script += ",targets: 0";
                script += "},";
                colCount++;
            }
            if (!this.HideCheckbox)
            {
                //$("input[name='EbDataGridViewControl1_id']:checked")
                var idd = this.Name + "_select-all";
                string __chk = string.Format("<input type='checkbox' name='{0}_id' value='@value' data-table='{0}' onclick='updateAlSlct(this)'>", this.Name);
                fl = true;
                script += "{";
                script += "data: null";
                script += string.Format(",title: \"<input  id='{0}' type='checkbox' onclick='clickAlSlct(event,this);' data-table='{1}'/>\"", idd, this.Name);
                script += ",width: 10";
                script += ",render: function( data2, type, row, meta ) { var idpos=(_.find(data.columns, {'columnName': 'id'})).columnIndex; return \"@chk\".replace('@value', row[idpos]); }".Replace("@chk", __chk);
                script += ",orderable: false";
                script += "},";
                colCount++;
            }

            foreach (EbDataGridViewColumn column in this.Columns)
            {
                script += "{";
                script += "data: " + "(_.find(data.columns, {'columnName': '{0}'})).columnIndex".Replace("{0}", column.Name);
                script += string.Format(",title: '{0}<span hidden>{1}</span>'", column.Label, column.Name);
                script += ",className: '" + this.GetClassName(column) + "'" ;
                script += ",visible: " + (!column.Hidden).ToString().ToLower();
                script += ",width: " + column.Width.ToString();
                script += ",render: " + this.GetRenderFunc(column);
                script += ",name: '" + column.Name + "'";
                script += "},";
                colCount++;
            }

            return script + "]";
        }

        private string GetClassName(EbDataGridViewColumn column)
        {
            string _c = string.Empty;

            if (column.ColumnType == EbDataGridViewColumnType.Text)
                _c = "dt-body-left";
            else if (column.ColumnType == EbDataGridViewColumnType.Numeric)
                _c = "dt-body-right";
            else
                _c = "dt-body-left";

            return _c;
        }

        // NEED WORK - Currency from current row, Also Locale en-US
        private string GetRenderFunc(EbDataGridViewColumn column)
        {
            string _r = string.Empty;
            string _fwrapper = "function( data, type, row, meta ) { {0} }";

            if (column.ColumnType == EbDataGridViewColumnType.Numeric)
            {
                var ext = column.ExtendedProperties as EbDataGridViewNumericColumnProperties;

                if (ext != null)
                {
                    if (!ext.Localize)
                        _r = string.Format("return parseFloat(data).toFixed({0});", ext.DecimalPlaces);
                    else
                    {
                        if (!ext.IsCurrency)
                            _r = "return parseFloat(data).toLocaleString('en-US', { maximumSignificantDigits: {0} });".Replace("{0}", ext.DecimalPlaces.ToString());
                        else
                            _r = "return parseFloat(data).toLocaleString('en-US', { style: 'currency', currency: 'USD', maximumSignificantDigits: {0} });".Replace("{0}", ext.DecimalPlaces.ToString());
                    }
                }
                else
                    _r = "return data;";

                _r = _fwrapper.Replace("{0}", _r);
            }
            else if(column.ColumnType == EbDataGridViewColumnType.DateTime)
                _r = _fwrapper.Replace("{0}", "return moment.unix(data).format('MM/DD/YYYY');");
            else
                _r = _fwrapper.Replace("{0}", "return data;");

            return _r;
        }

        public string GetFilterControls()
        {
            List<string> _lsRet = new List<string>();

            if (fl == true) _lsRet.Add("<th>&nbsp;</th>");
            if (serial == true) _lsRet.Add("<th>&nbsp;</th>");

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

                    _ls.Append("<td style='padding: 0px; margin: 0px'>");

                    if (column.ColumnType == EbDataGridViewColumnType.Numeric)
                        _ls.Append(span + getFilterForNumeric(header_text1, header_select, data_table, htext_class, data_colum, header_text2));
                    else if (column.ColumnType == EbDataGridViewColumnType.Text)
                        _ls.Append(span + getFilterForString(header_text1, header_select, data_table, htext_class, data_colum));
                    else if (column.ColumnType == EbDataGridViewColumnType.DateTime)
                        _ls.Append(span + getFilterForDateTime(header_text1, header_select, data_table, htext_class, data_colum, header_text2));
                    else
                        _ls.Append(span);

                    _ls.Append("</th>");
                }
                else
                    _ls.Append("<th style='display:none'>&nbsp;</th>");

                _lsRet.Add(_ls.ToString());
            }

            _ls.Clear();
            _ls = null;

            return Newtonsoft.Json.JsonConvert.SerializeObject(_lsRet);
        }

        public string GetAggregateControls(int footer_id)
        {
            List<string> _ls = new List<string>();
            if (fl == true)
                _ls.Add("&nbsp;");
            if(serial==true)
                _ls.Add("&nbsp;");
            foreach (EbDataGridViewColumn column in this.Columns)
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
        <button type='button' class='btn btn-default dropdown-toggle' data-toggle='dropdown' id='{1}'> Sum </button>
        <ul class='dropdown-menu'>
          <li ><a href ='#' onclick='fselect_func(this);' {2} {3} {4}>Sum</a></li>
          <li><a href ='#' onclick='fselect_func(this);' {2} {3} {4}>Avg</a></li>
        </ul>
    </div>
    <input type='text' class='form-control' id='{0}' disabled {2}  {3}>
</div>", footer_txt, footer_select_id, data_table, data_colum, data_decip));
                            //_ls.Add(string.Format(@"<div><input type='text' id='{0}' style='text-align:right;float: right;width: 100px;' disabled>
                            //    <select id='{1}' class='{4}' {5} {6} {7} onchange='fselect_func(this);' style='width: 38px;'>{2}{3}</select></div>",
                            //    footer_txt, footer_select_id,
                            //    (ext.Sum ? "<option value='Sum' selected='selected'>Sum</option>" : string.Empty),
                            //    (ext.Average ? "<option value='Avg'>Avg</option>" : string.Empty), 
                            //    fselect_class, 
                            //    data_table, data_colum, data_decip));
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

            return Newtonsoft.Json.JsonConvert.SerializeObject(_ls);
        }
        
        public string GetAggregateInfo()
        {
            List<AggregateInfo> _ls = new List<AggregateInfo>();

            foreach (EbDataGridViewColumn column in this.Columns)
            {
                var ext = column.ExtendedProperties as EbDataGridViewNumericColumnProperties;

                if (ext != null)
                {
                    if (column.ColumnType == EbDataGridViewColumnType.Numeric && (ext.Sum || ext.Average))
                        _ls.Add(new AggregateInfo { colname = column.Name, coltype = "N" , deci_val=ext.DecimalPlaces });
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
            return (this.ScrollY > 0) ? string.Format("scrollY: '{0}'", this.ScrollY) : "fixedHeader: { footer: true }";
        }

        public string GetFooter()
        {
            string ftr = string.Empty;
            ftr = "<tfoot>";
            for (int i = 0; i < colCount; i++)
                ftr += "<th></th>";
            ftr += "<tr>";
            for (int i = 0; i < colCount; i++)
                ftr += "<th></th>";
            ftr += "</tr>";
            ftr += "</tfoot>";
            return ftr;
        }

        public string getFilterForNumeric( string header_text1,string header_select, string data_table,string htext_class,string data_colum,string header_text2)
        {
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
    <input type='number' class='form-control {2}' id='{0}' onkeypress='call_filter(event, this);' {1}  {3}>
    <span class='input-group-btn'></span>
    <input type='number' class='form-control {2}' id='{5}' style='visibility: hidden' onkeypress='call_filter(event, this);' {1}  {3}>
</div> ", header_text1, data_table,htext_class, data_colum, header_select, header_text2)
;
            return drptext;
        }

        public string getFilterForDateTime(string header_text1, string header_select, string data_table, string htext_class, string data_colum, string header_text2)
        {
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
    <input type='date' class='form-control {2}' id='{0}' onkeypress='call_filter(event, this);' {1}  {3}>
    <span class='input-group-btn'></span>
    <input type='date' class='form-control {2}' id='{5}' style='visibility: hidden' onkeypress='call_filter(event, this);' {1}  {3}>
</div> ", header_text1, data_table, htext_class, data_colum, header_select, header_text2);
            return filter;
        }

        public string getFilterForString(string header_text1, string header_select, string data_table, string htext_class, string data_colum)
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
    <span class='input-group-btn'></span>    
</div> ", header_text1, data_table, htext_class, data_colum, header_select)
;
            return drptext;
        }

        public override string GetHead()
        {
            return @"";
        }

        public override string GetHtml()
        {
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
td.details-control {
    background:  url('http://cdn.mysitemyway.com/etc-mysitemyway/icons/legacy-previews/icons-256/simple-black-square-icons-alphanumeric/126293-simple-black-square-icon-alphanumeric-plus-sign-simple.png') no-repeat center center;
    cursor: pointer;
}
tr.details td.details-control {
    background: url('http://findicons.com/files/icons/2583/sweetieplus/24/badge_square_minus_24_ns.png') no-repeat center center;
}
//table.dataTable.stripe tbody > tr.odd.selected, table.dataTable.stripe tbody > tr.odd > .selected, table.dataTable.display tbody > tr.odd.selected, table.dataTable.display tbody > tr.odd > .selected {
//    background-color: #fbfbfb!important;
//}
//table.dataTable.stripe tbody > tr.even.selected, table.dataTable.stripe tbody > tr.even > .selected, table.dataTable.display tbody > tr.even.selected, table.dataTable.display tbody > tr.even > .selected {
//    background-color: #ffffff!important;
//}
#@tableId_tbl th.resizing {
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
//::-webkit-scrollbar {
//   width: 8px;
//   height:8px;
//}

//::-webkit-scrollbar-track {
//   -webkit-box-shadow: inset 0 0 6px rgba(0,0,0,0.3);
//   border-radius: 8px;
//}

//::-webkit-scrollbar-thumb {
//   border-radius: 8px;
//   -webkit-box-shadow: inset 0 0 6px rgba(0,0,0,0.5);
//}
.dataTables_scroll{
//overflow-x:auto!important;
}
.dataTables_scrollHead {
padding-bottom: 250px; margin-bottom: -250px;
}
.dataTables_scrollFoot{
padding-bottom: 250px; margin-bottom: -250px;
}
</style>
    <div class='tablecontainer' id='@tableId_container'>
        <div>
            <a class='btn btn-default' onclick='showOrHideFilter(this,@scrolly);' data-table='@tableId' data-toggle='tooltip' title='On\/Off Filter'><i class='fa fa-filter' aria-hidden='true'></i></a>
            <button type='button' id='@tableId_btntotalpage' style='height: 32px;display: none;' onClick='showOrHideAggrControl(this,@scrolly);' data-table='@tableId'>Page Total!</button>
        </div>
        <div style='width:auto;'>
              
                    <h3>@tableViewName</h3>
               <div id='@tableId_loadingdiv' class='loadingdiv'>
                    <img id='@tableId_loading-image' src='/images/ajax-loader.gif' alt='Loading...' />
               </div>
               
               <table id='@tableId_tbl' class='table table-striped table-bordered'></table>
          </div>
     </div>

<script>

$('#@tableId_loadingdiv').show();
$('#@tableId_tbl').append( $('@tfoot') );
$.get('/ds/columns/@dataSourceId?format=json', function (data)
{
    var @tableId_ids=[];
    var @tableId_filter_objcol = [];
    var @tableId_order_colname='';
    var @tableId__datacolumns = data.columns;
    $('#@tableId_tbl').DataTable(
    {
        dom:'Blftrip',
        @scrollYOption,
        scrollX : true,
        responsive:true,
        keys: true,
        autoWidth: false,
        @lengthMenu,
        serverSide: true,
        processing:true,
        language: { processing: '<div></div><div></div><div></div><div></div><div></div><div></div><div></div>'},
        columns:@columnsRender, 
        order: [],
        deferRender: true,
        filter: true,
        select: { style: 'os', selector: 'td:first-child' },
        buttons: ['colvis'],
        ajax: {
            url: '/ds/data/@dataSourceId?format=json',
            data: function(dq) { 
                    delete dq.columns;
                    @tableId_filter_objcol = repopulate_filter_arr('@tableId');
                    if (@tableId_filter_objcol.length !== 0)
                    {
                        dq.search_col = @tableId_filter_objcol.map(function(a) {return a.column;}).join(',');
                        dq.selectedvalue = @tableId_filter_objcol.map(function(a) {return a.operator;}).join(',');
                        dq.searchtext = @tableId_filter_objcol.map(function(a) {return a.value;}).join(',');
                    }  

                    if(@tableId_order_colname!=='')
                        dq.order_col=@tableId_order_colname; 
                },
            dataSrc: function(dd) {return dd.data; }
        },
        
        fnRowCallback: function( nRow, aData, iDisplayIndex, iDisplayIndexFull ) {
            colorRow(nRow, aData, iDisplayIndex, iDisplayIndexFull, data.columns);
        },

        fnFooterCallback: function ( nRow, aaData, iStart, iEnd, aiDisplay ) {
            summarize2('@tableId', @eb_agginfo);
        },
        drawCallback:function ( settings ) {
            
            $('#@tableId_tbl').DataTable().columns.adjust();
        }
        //drawCallback: function ( settings ) {
        //    var api = this.api();
        //    var rows = api.rows( { page: 'current'} ).nodes();
        //    var last = null;
            
        //    api.column(3, { page: 'current'} ).data().each(function(group, i) {
        //        if (last !== group)
        //        {
        //            $(rows).eq(i).before(
        //                '<tr class=\'group\'><td colspan=\'8\'>' + group + '</td></tr>'
        //            );

        //            last = group;
        //        }
        //    } );
        //}
    });

    $.fn.dataTable.Api.register( 'column().data().sum()', function () {
        return this.reduce( function (a, b) { return a + b; } );
    } );

    $.fn.dataTable.Api.register( 'column().data().average()', function () {
        var sum= this.reduce( function (a, b) { return a + b; } );
        return sum/this.length;
    } );

    if( @eb_agginfo.length>0 ) {
        createFooter('@tableId', @eb_footer1, @scrolly, 0);
        createFooter('@tableId', @eb_footer2, @scrolly, 1);
    }

    $('#@tableId_loadingdiv').hide();

    createFilterRowHeader('@tableId', @eb_filter_controls, @scrolly);

    $('#@tableId_container thead').on('click','th',function(){
        var txt=$(this).children('span').text();
        if(txt !== '')
            @tableId_order_colname =txt;
    });

    if(@bserial){
        $('#@tableId_tbl').DataTable().on( 'draw.dt', function () {
            $('#@tableId_tbl').DataTable().column(0).nodes().each( function (cell, i) {
                cell.innerHTML = i+1;
            } );
        } );
    }

    $('#@tableId_container [type=search]').on( 'keyup', function () {alert('haa');
        $('#@tableId_tbl').DataTable().search( 'food' ).draw();
    } );


});
</script>"
.Replace("@dataSourceId", this.DataSourceId.ToString().Trim())
.Replace("@tableId", this.Name)
.Replace("@tableViewName", this.Label)
.Replace("@lengthMenu", this.GetLengthMenu())
.Replace("@columnsRender", this.GetCols())
.Replace("@eb_filter_controls", this.GetFilterControls())
.Replace("@eb_footer1", this.GetAggregateControls(1))
.Replace("@eb_footer2", this.GetAggregateControls(2))
.Replace("@eb_agginfo", this.GetAggregateInfo())
.Replace("@bserial", this.serial.ToString().ToLower())
.Replace("@scrolly", this.ScrollY.ToString())
.Replace("@scrollYOption", this.GetScrollYOption())
.Replace("@tfoot", this.GetFooter());
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
        private EbDataGridViewColumnType _columnType = EbDataGridViewColumnType.Text;

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
                else
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
    }

    [ProtoBuf.ProtoContract]
    [ProtoBuf.ProtoInclude(1, typeof(EbDataGridViewNumericColumnProperties))]
    [ProtoBuf.ProtoInclude(2, typeof(EbDataGridViewDateTimeColumnProperties))]
    public class EbDataGridViewColumnProperties
    {

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
    }

    [ProtoBuf.ProtoContract]
    public class EbDataGridViewDateTimeColumnProperties : EbDataGridViewColumnProperties
    {
    }

    [ProtoBuf.ProtoContract]
    public class EbDataGridViewColumnCollection : ObservableCollection<EbDataGridViewColumn>
    {
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
    }
}
