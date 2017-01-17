using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace ExpressBase.Objects
{
    [ProtoBuf.ProtoContract]
    public class EbDataGridView : EbControl
    {
        [ProtoBuf.ProtoMember(1)]
        public int DataSourceId { get; set; }

        [ProtoBuf.ProtoMember(2)]
        public int PageSize { get; set; }

        [ProtoBuf.ProtoMember(3)]
        public EbDataGridViewColumnCollection Columns { get; set; }

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
}
.loadingdiv {
    vertical-align:middle;
    margin: 5% 50%;
    display: none;
}
</style>
<div class='tablecontainer'>
    <h3>@@@@@@@</h3>
    <div id='$$$$$$$_loadingdiv' class='loadingdiv'>
        <img id='$$$$$$$_loading-image' src='/images/ajax-loader.gif' alt='Loading...' />
    </div>
    <table id='$$$$$$$_tbl' style='width:100%;' class='display compact'></table>
</div>
<script>
$('#$$$$$$$_loadingdiv').show();
$.get('/ds/columns/#######?format=json', function (data)
{
    var cols = [];
    var colname='';
    var searchText='';
    if (data != null)
        $.each(data.columns, 
            function(i, value) { 
                cols.push({ 'data': value.columnIndex.toString(), 'title': value.columnName }); });

    $('#$$$$$$$_tbl').dataTable(
    {
        &&&&&&&,
        serverSide: true,
        processing: true,
        language: { processing: '<div></div><div></div><div></div><div></div><div></div><div></div><div></div>', },
        columns: cols,
        order: [],
        deferRender: true,
        ajax: {
            url: '/ds/data/#######?format=json',
            data: function(dq) { 
                    delete dq.columns; 
                    if(colname!=='')
                        dq.col=colname; 
                    if(searchText!=='')
                        dq.searchtext=searchText;
                },
            dataSrc: function(dd) { return dd.data; }
        },
    });
    $('#$$$$$$$_loadingdiv').hide();
    $('#$$$$$$$_tbl_filter input').unbind();
    $('#$$$$$$$_tbl_filter input').bind('keyup', function(e) {
        if(e.keyCode == 13) {
            $('#$$$$$$$_tbl').dataTable().fnFilter(this.value);
        }
    });
    $('#$$$$$$$_tbl thead tr th').each( function () {
        var title = $(this).text();
        $(this).html( title+'<br/><input/>' );
    } );
    $('#$$$$$$$_tbl').on('click','thead th',function(event) {
        var headtitle = $(this).text();
        colname=headtitle;
        });
    $('#$$$$$$$_tbl thead tr th input').keypress(function (e) {
        if(e.which == 13){
            searchText=$(this).val();
        }
     });
});
</script>
".Replace("#######", this.DataSourceId.ToString().Trim())
.Replace("$$$$$$$", this.Name)
.Replace("@@@@@@@", this.Label)
.Replace("&&&&&&&", this.GetLengthMenu());
        }
    }

    [ProtoBuf.ProtoContract]
    public class EbDataGridViewColumn : EbControl
    {
        [ProtoBuf.ProtoMember(1)]
        public int Width { get; set; }

        [ProtoBuf.ProtoMember(2)]
        public EbDataGridViewColumnType ColumnType { get; set; }

        public EbDataGridViewColumn()
        {
            this.Width = 100;
            this.ColumnType = EbDataGridViewColumnType.Text;
        }
    }

    [ProtoBuf.ProtoContract]
    public class EbDataGridViewColumnCollection : ObservableCollection<EbDataGridViewColumn>
    {
    }
}
