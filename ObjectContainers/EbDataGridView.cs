using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
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

        public string GetCols()
        {
            string script = "[";

            foreach (EbDataGridViewColumn column in this.Columns)
            {
                script += "{";

                script += "'data': " + "(_.find(data.columns, {'columnName': '{0}'})).columnIndex".Replace("{0}", column.Name);
                script += string.Format(",'title': '{0}<span hidden>{1}</span>'", column.Label, column.Name);
                script += ",'className': '" + this.GetClassName(column) + "'" ;
                script += ",'visible': " + (!column.Hidden).ToString().ToLower();
                script += ",'width': " + column.Width.ToString();
                script += ",'render': function( data, type, full ) { {0} }".Replace("{0}", this.GetRenderFunc(column));

                script += "},";
            }

            return script + "]";
        }

        private string GetClassName(EbDataGridViewColumn column)
        {
            string _c = string.Empty;

            if (column.ColumnType == EbDataGridViewColumnType.Text)
                _c = "dt-left";
            else if (column.ColumnType == EbDataGridViewColumnType.Numeric)
                _c = "dt-right";
            else
                _c = "dt-left";

            return _c;
        }

        // NEED WORK - Currency from current row, Also Locale en-US
        private string GetRenderFunc(EbDataGridViewColumn column)
        {
            string _r = string.Empty;

            if (column.ColumnType == EbDataGridViewColumnType.Numeric)
            {
                var ext = column.ExtendedProperties as EbDataGridViewNumericColumnProperties;

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

            return _r;
        }

        public string GetFilterControls()
        {
            List<string> _ls = new List<string>();

            foreach (EbDataGridViewColumn column in this.Columns)
            {
                var span = string.Format("<span hidden>{0}</span>", column.Name);

                if (column.ColumnType == EbDataGridViewColumnType.Numeric)
                    _ls.Add(span + string.Format(@"
<div>
<select id='{0}' style='width: 38px'>
    <option value='&lt;'> &lt; </option>
    <option value='&gt;'> &gt; </option>
    <option value='=' selected='selected'> = </option>
    <option value='<='> <= </option>
    <option value='>='> >= </option>
    <option value='B'> B </option>
</select>
<input type='number' id='{1}' style='width: {2}px; display:inline;' /></div>", "header_select" + column.Name, "header_txt1" + column.Name, column.Width - 38));
                else if (column.ColumnType == EbDataGridViewColumnType.Text)
                    _ls.Add(span + string.Format(@"
<input type='text' id='{0}' style='width: 100%' />", "header_txt1" + column.Name));
                else if (column.ColumnType == EbDataGridViewColumnType.DateTime)
                    _ls.Add(span + string.Format(@"
<div>
<span style='width: 38px'><select id='{0}' style='width: 100%'>
    <option value='&lt;'> &lt; </option>
    <option value='&gt;'> &gt; </option>
    <option value='=' selected='selected'> = </option>
    <option value='<='> <= </option>
    <option value='>='> >= </option>
    <option value='B'> B </option>
</select></span>
<span><input type='date' id='{1}' style='width: 100%' /></span></div>", "header_select" + column.Name, "header_txt1" + column.Name));
                else
                    _ls.Add(span);
            }

            return Newtonsoft.Json.JsonConvert.SerializeObject(_ls);
        }

        public string GetAggregateControls()
        {
            List<string> _ls = new List<string>();

            foreach (EbDataGridViewColumn column in this.Columns)
            {
                var ext = column.ExtendedProperties as EbDataGridViewNumericColumnProperties;

                if (column.ColumnType == EbDataGridViewColumnType.Numeric)
                {
                    if (ext.Sum || ext.Average)
                    {
                        _ls.Add(string.Format(@"<div><select id='{0}' style='width: 38px'>{1}{2}</select>
                             <input type='text' id='{3}' style='text-align:right;width: 100px;'></div>",
                            "footer1_select" + column.Name,
                            (ext.Sum ? "<option value='Sum' selected='selected'>Sum</option>" : string.Empty),
                            (ext.Average ? "<option value='Avg'>Avg</option>" : string.Empty), "footer1_txt" + column.Name));
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

                if (column.ColumnType == EbDataGridViewColumnType.Numeric && (ext.Sum || ext.Average))
                    _ls.Add(new AggregateInfo { colname = column.Name, coltype = "N" });
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

        public override string GetHtml()
        {
            return @"
<style>
.tablecontainer {
    width:100%;
    height:auto;
    border:solid 1px;
    display:inline-block;
    overflow-x:auto;
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
</style>
    <div class='tablecontainer'>
        <div style='width:auto; border:solid 1px yellow;'>
              
                    <h3>@@@@@@@</h3>
               <div id='$$$$$$$_loadingdiv' class='loadingdiv'>
                    <img id='$$$$$$$_loading-image' src='/images/ajax-loader.gif' alt='Loading...' />
               </div>
               <table id='$$$$$$$_tbl' style=' border:solid 1px red;' class='display compact'></table>
          </div>
     </div>
<script>
$('#$$$$$$$_tbl').append( $('<tfoot/>') );
$('#$$$$$$$_loadingdiv').show();
var pageTotal=0;   
var dcolumns = [];
$.get('/ds/columns/#######?format=json', function (data)
{
    var ids=[];
    var cols = [];
    var searchTextCollection=[];
    var search_colnameCollection=[];
    var order_colname='';
    var searchText='';
    var select_collection=[];
    var j=1;
    var eb_filter_controls = @eb_filter_controls;
    var eb_footer1 = @eb_footer1;
    var eb_agginfo = @eb_agginfo;
    dcolumns = data.columns;
    
    $('#$$$$$$$_tbl').dataTable(
    {
        dom: 'l <\'toolbar\'> Bfrtip',
        buttons: ['copy', 'excel', 'pdf'],
        autoWidth: false,
        &&&&&&&,
        serverSide: true,
        processing: true,
        language: { processing: '<div></div><div></div><div></div><div></div><div></div><div></div><div></div>'},
        columns:@columnsRender, 
        order: [],
        deferRender: true,
        select: {
            style: 'os',
            selector: 'td:not(:last-child)' // no row selection on last column
        },
        ajax: {
            url: '/ds/data/#######?format=json',
            data: function(dq) { 
                    delete dq.columns; 
                    if(search_colnameCollection.length!==0){
                        dq.search_col='';
                        $.each(search_colnameCollection,function(i, value) {
                            if(dq.search_col=='')
                                dq.search_col=value;
                            else
                                dq.search_col=dq.search_col+','+value;
                        });
                    }   
                    if(order_colname!=='')
                        dq.order_col=order_colname; 
                    if(searchTextCollection.length!=0){
                        dq.searchtext='';
                        $.each(searchTextCollection,function(i, value) {
                            if(dq.searchtext=='')
                                dq.searchtext=value;
                            else
                                dq.searchtext=dq.searchtext+','+value;
                        });              
                    }
                    if(select_collection.length!=0){
                        dq.selectedvalue='';
                        $.each(select_collection,function(i, value) {
                            if(dq.selectedvalue=='')
                                dq.selectedvalue=value;
                            else
                                dq.selectedvalue=dq.selectedvalue+','+value;
                        });              
                    }
                },
            dataSrc: function(dd) {return dd.data; }
        },
        fnRowCallback: function( nRow, aData, iDisplayIndex, iDisplayIndexFull ) {
             $.each(data.columns,function(i, value) { 
                if(value.columnName==='sys_row_color'){
                    rgb=(aData[value.columnIndex]).toString();
                    var r=rgb.slice(0,-6);
                    r=parseInt(r);
                    if(r<=9)
                        fl='0';
                    r=r.toString(16);
                    if(fl==='0')
                        r='0'+r;

                    var g=rgb.slice(3,-3);
                    g=parseInt(g);
                    if(g<=9)
                        fl='0';
                    g=g.toString(16);
                    if(fl==='0')
                        g='0'+g;
                    var b=rgb.slice(6,9);
                    b=parseInt(b);
                    if(b<=9)
                        fl='0';
                    b=b.toString(16);
                    if(fl==='0')
                        b='0'+b;
                    rgb=r+g+b;
                    //alert(rgb);
                     $(nRow).css('background-color', '#' + rgb);
                }
                if(value.columnName==='sys_cancelled'){
                    var tr=aData[value.columnIndex];
                    if(tr==true)
                        $(nRow).css('color', '#f00');
                }
            });
         },
        fnFooterCallback: function ( nRow, aaData, iStart, iEnd, aiDisplay ) {
            var api = $('#$$$$$$$_tbl').dataTable().api();
            $.each(eb_agginfo, function (index,agginfo) {
                
                 var p = $('#footer1_select' + agginfo.colname).val();
                 if (p === 'Sum') {
                    pageTotal = api.column(5, { page: 'current'} ).data().reduce( function (a, b) { return a + b; }, 0 );
                }
                alert(pageTotal);
            });
        },
   });

   $('div.toolbar').append('<div><button type=\'button\' id=\'$$$$$$$_btnfilter\' style=\'height: 32px;\'>Click Me!</button><button type=\'button\' id=\'$$$$$$$_btntotalpage\' style=\'height: 32px;\'>Page Total!</button></div>');
    
    // CLONE HEADER to FOOTER AND REMOVE SORT ICONS AND HIDDEN column Name SPAN
    $('#$$$$$$$_tbl tfoot').append( $('#$$$$$$$_tbl thead tr').clone());  
    $('#$$$$$$$_tbl tfoot tr:eq(0) th').each( function (idx) { $(this).children().eq(0).children().remove(); } );
    
    $('#$$$$$$$_tbl tfoot').append( $('#$$$$$$$_tbl thead tr').clone());
    $('#$$$$$$$_tbl tfoot tr:eq(1) th').each( function (idx) { $(this).children().remove(); $(this).removeClass('sorting'); $(this).append(eb_footer1[idx]); } );

    var tfoot = $('#$$$$$$$_tbl tfoot');
    $(tfoot).append($('#$$$$$$$_tbl thead tr').clone());  
    $('#$$$$$$$_tbl tfoot tr:eq(2)').hide();

    $('#$$$$$$$_tbl tfoot tr:eq(2) th').each( function (idx) {
        var idd= 'footer2_txt' + $(this).text();
        var idds= 'footer2_select' + $(this).text();
        var t='<span hidden>'+$(this).text()+'</span>'
        if(idx!=0 && idx!=1){
            if(data.columns[idx-1].type=='System.Int32, System.Private.CoreLib'|| data.columns[idx-1].type=='System.Int16, System.Private.CoreLib'){
                $(this).html(t+'<select id='+idds+' width=\'60\' style=\'display:none;\'><option value=\'Sum\' selected=\'selected\'>Sum</option><option value=\'Avg\'> Avg </option></select><input type=\'text\' id='+idd+' style=\'width: 100px;display:none;\' />');
                //alert($(this).text());
            }
            else if(data.columns[idx-1].type=='System.Decimal, System.Private.CoreLib'){                
                $(this).html(t+'<select id='+idds+' width=\'60\' style=\'display:none;\'><option value=\'Sum\' selected=\'selected\'>Sum</option><option value=\'Avg\'> Avg </option></select><input type=\'text\' id='+idd+' style=\'width: 100px;display:none;\' />');               
            }
            else
                $(this).html('');
        }
    } );
        
    $('#$$$$$$$_loadingdiv').hide();

    var rgb='';
    var fl='';

    // FILTER ROW ON HEADER
    $('#$$$$$$$_tbl thead').append( $('#$$$$$$$_tbl thead tr').clone());  
    $('#$$$$$$$_tbl thead tr:eq(1)').hide();
    $('#$$$$$$$_tbl thead tr:eq(1) th').each( function (idx) { $(this).children().remove(); $(this).removeClass('sorting'); $(this).append(eb_filter_controls[idx]); });
    
    $('#$$$$$$$_tbl thead tr:eq(0)').on( 'click', 'th', function(event) {
        order_colname = $(this).children().eq(0).children().eq(0).text();
     });

     $('#$$$$$$$_tbl thead tr:eq(1) th').on('click','input',function(event) {
        event.stopPropagation();
     }); 

    $('#$$$$$$$_tbl thead tr:eq(1) th').on('click','select',function(event) {
        event.stopPropagation();
     }); 

    $('#$$$$$$$_tbl thead tr:eq(1) th input').keypress(function (e) {
        //alert($(this).siblings('span').text());
        searchTextCollection=[];
        search_colnameCollection=[];
        select_collection=[];
         if(e.which == 13){            
            $('#$$$$$$$_tbl thead tr th input').each( function (idx) {
                if($(this).val()!=''){
                    if($.inArray($(this).siblings('span').text(), search_colnameCollection) == -1){
                        searchTextCollection.push($(this).val());
                        search_colnameCollection.push($(this).siblings('span').text());
                        if($(this).prev('select').length==1){
                            if($(this).prev('select').val()=='B'){
                                if($(this).next().val()!=''){
                                    searchTextCollection.splice( $.inArray($(this).val(),searchTextCollection) ,1 );
                                    searchTextCollection.push($(this).val()+'@'+$(this).next().val());
                                }
                            }
                            select_collection.push($(this).prev('select').val());
                        }
                        else
                            select_collection.push('null');
                    }
                }
            });
        }
     });

    $('#$$$$$$$_tbl thead tr:eq(1) th select').on('change',function(e){
        var idd='header_txt2'+$(this).siblings('span').text();
        if($(this).val()=='B'){
           if($(this).next('input').attr('type') == 'date')
                $(this).next().after($('<input type=\'date\'id='+idd+' style=\'min-width: 160px;\'/>'));
           else
                $(this).next().after($('<input type=\'number\'id='+idd+' style=\'min-width: 160px;\'/>'));
        }
        else
           $(this).next().next().remove();
    });


    $('#$$$$$$$_tbl tbody').on('click', '.checkbox', function(event){        
       if (document.getElementById(event.target.id).checked) {           
            var row = $(this).closest('tr');
            var data = $('#$$$$$$$_tbl').dataTable().fnGetData(row);
            ids.push(data[0]);
        }
        else {
            var row = $(this).closest('tr');
            var data = $('#$$$$$$$_tbl').dataTable().fnGetData(row);
            ids.splice(ids.indexOf(data[0]),1);
        }        
    });

    $('#$$$$$$$_tbl tfoot tr:eq(1) th select').on('change',function(e){
        $.each(data.columns,function(j, value) {
            if(e.target.id=='footer1_select'+value.columnName){
                var p=e.target.value;                                
                if(p=='Sum'){
                    var api = $('#$$$$$$$_tbl').dataTable().api();
			        var intVal = function ( i ) {
				        return typeof i === 'number' ? i : 0;
			        };
			        pageTotal =api
				        .column( j+2, { page: 'current'} )
                        .data()
				        .reduce( function (a, b) {
					        return intVal(a) + intVal(b);
				        }, 0 );                                        
                }
                if(p=='Avg'){                                   
                    var api = $('#$$$$$$$_tbl').dataTable().api();
			        var intVal = function ( i ) {
				        return typeof i === 'number' ? i : 0;
			        };
			        pageTotal =api
				        .column( j+2, { page: 'current'} )
				        .data()
				        .reduce( function (a, b) {
					        return intVal(a) + intVal(b);
				        }, 0 );
                    pageTotal=pageTotal / api
				        .column( j+2, { page: 'current'} )
				        .data().length;
                }   
                var k=j+1;            
                ($('#$$$$$$$_tbl tfoot tr:eq(1) th:eq('+k+')').children('input')[0]).value=pageTotal.toFixed(2);                
            }
        });
     }); 

    $('#$$$$$$$_tbl tfoot tr:eq(2) th select').on('change',function(e){alert('haa');
        $.each(data.columns,function(j, value) {
            if(e.target.id=='footer2_select'+value.columnName){             
            }
        });
     }); 

    $('#$$$$$$$_btnfilter').click(function(obj){
        if ($('#$$$$$$$_tbl thead tr:eq(1)').is(':visible'))
            $('#$$$$$$$_tbl thead tr:eq(1)').hide();
        else
            $('#$$$$$$$_tbl thead tr:eq(1)').show();
    });

    $('#$$$$$$$_btntotalpage').click(function(obj){
        if ($('#$$$$$$$_tbl tfoot tr:eq(2)').is(':visible'))
            $('#$$$$$$$_tbl tfoot tr:eq(2)').hide();
        else
            $('#$$$$$$$_tbl tfoot tr:eq(2)').show();
        $('#$$$$$$$_tbl tfoot tr:eq(2) th').each( function (idx) {
            var title = $(this).children('span').text();
            var idd= 'footer2_txt' + title;
            var idds= 'footer2_select' + title;
            $('#'+idd).toggle(); 
            $('#'+idds).toggle();           
        });   
    });

    function format ( d ) {
        var tbl='';
        $.each(data.columns,function(j, value) {
            if(value.columnName=='xid')
                tbl+='XID:'+d[value.columnIndex]+'</br>'
        });
        return tbl;
    }
    // Array to track the ids of the details displayed rows
    var detailRows = [];
 
    $('#$$$$$$$_tbl tbody').on( 'click', 'tr td.details-control', function () {
        var tr = $(this).closest('tr');
        var row = $('#$$$$$$$_tbl').DataTable().row( tr );
        var idx = $.inArray( tr.attr('id'), detailRows );
 
        if ( row.child.isShown() ) {
            tr.removeClass( 'details' );
            row.child.hide();
 
            // Remove from the 'open' array
            detailRows.splice( idx, 1 );
        }
        else {
            tr.addClass( 'details' );
            row.child( format( row.data() ) ).show();
 
            // Add to the 'open' array
            if ( idx === -1 ) {
                detailRows.push( tr.attr('id') );
            }
        }
    } );
 
    // On each draw, loop over the `detailRows` array and show any child rows
    $('#$$$$$$$_tbl').DataTable().on( 'draw', function () {
        $.each( detailRows, function ( i, id ) {
            $('#'+id+' td.details-control').trigger( 'click' );
        } );
    } );

});

</script>
".Replace("#######", this.DataSourceId.ToString().Trim())
.Replace("$$$$$$$", this.Name)
.Replace("@@@@@@@", this.Label)
.Replace("&&&&&&&", this.GetLengthMenu())
.Replace("@columnsRender", this.GetCols())
.Replace("@eb_filter_controls", this.GetFilterControls())
.Replace("@eb_footer1", this.GetAggregateControls())
.Replace("@eb_agginfo", this.GetAggregateInfo());
        }
    }

    public class AggregateInfo
    {
        public string colname { get; set; }
        public string coltype { get; set; }
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
    }
}
