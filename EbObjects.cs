using System.Collections.Generic;
using System.ComponentModel;

namespace ExpressBase.UI
{
    [ProtoBuf.ProtoContract]
    public enum EbObjectType
    {
        Form,
        View,
        DataSource,
    }

    [ProtoBuf.ProtoContract]
    [ProtoBuf.ProtoInclude(1000, typeof(EbControl))]
    [ProtoBuf.ProtoInclude(1001, typeof(EbDataSource))]
    public class EbObject
    {
        [Browsable(false)]
        public int Id { get; set; }

        [ProtoBuf.ProtoMember(1)]
        [Browsable(false)]
        public EbObjectType EbObjectType { get; set; }

        [ProtoBuf.ProtoMember(2)]
        [Browsable(false)]
        public string TargetType { get; set; }

        [ProtoBuf.ProtoMember(3)]
        [Description("Identity")]
        public virtual string Name { get; set; }

        public EbObject() { }
    }

    [ProtoBuf.ProtoContract]
    [ProtoBuf.ProtoInclude(2000, typeof(EbButton))]
    [ProtoBuf.ProtoInclude(2001, typeof(EbTableLayout))]
    [ProtoBuf.ProtoInclude(2002, typeof(EbChart))]
    [ProtoBuf.ProtoInclude(2003, typeof(EbDataGridView))]
    public class EbControl : EbObject
    {
        [ProtoBuf.ProtoMember(4)]
        [Browsable(false)]
        public List<EbControl> Controls { get; set; }

        [ProtoBuf.ProtoMember(5)]
        [Description("Labels")]
        public virtual string Label { get; set; }

        [ProtoBuf.ProtoMember(6)]
        [Description("Labels")]
        public virtual string HelpText { get; set; }

        [ProtoBuf.ProtoMember(7)]
        [Description("Labels")]
        public virtual string ToolTipText { get; set; }

        [ProtoBuf.ProtoMember(8)]
        [Browsable(false)]
        public virtual int CellPositionRow { get; set; }

        [ProtoBuf.ProtoMember(9)]
        [Browsable(false)]
        public virtual int CellPositionColumn { get; set; }

        public EbControl() { }

        public virtual string GetHtml() { return string.Empty; }
    }

    [ProtoBuf.ProtoContract]
    public class EbForm : EbControl
    {
        public EbForm() { }
    }

    [ProtoBuf.ProtoContract]
    public class EbButton : EbControl
    {
        public EbButton() { }
    }

    [ProtoBuf.ProtoContract]
    public class EbTableLayout : EbControl
    {
        [ProtoBuf.ProtoMember(1)]
        [Browsable(false)]
        public List<EbTableColumn> Columns { get; set; }

        [ProtoBuf.ProtoMember(2)]
        [Browsable(false)]
        public List<EbTableRow> Rows { get; set; }

        public EbTableLayout() { }

        public override string GetHtml()
        {
            string html = GetTable();

            if (base.Controls != null)
            {
                foreach (EbControl ec in base.Controls)
                    html = html.Replace(string.Format("{0}_{1}_{2}", this.Name, ec.CellPositionColumn, ec.CellPositionRow), ec.GetHtml());
            }

            return html;
        }

        private string GetTable()
        {
            HtmlTable ht = new HtmlTable(this.Name);

            foreach (EbTableRow row in this.Rows)
            {
                HtmlRow hr = new HtmlRow(this.Name, row);

                foreach (EbTableColumn col in this.Columns)
                    hr.Cells.Add(new HtmlCell(this.Name, col, row));

                ht.Rows.Add(hr);
            }

            return ht.GetHtml();
        }
    }

    [ProtoBuf.ProtoContract]
    public class EbTableColumn 
    {
        [ProtoBuf.ProtoMember(1)]
        public int Index { get; set; }

        [ProtoBuf.ProtoMember(2)]
        public int Width { get; set; }
    }

    [ProtoBuf.ProtoContract]
    public class EbTableRow
    {
        [ProtoBuf.ProtoMember(1)]
        public int Index { get; set; }

        [ProtoBuf.ProtoMember(2)]
        public int Height { get; set; }
    }

    [ProtoBuf.ProtoContract]
    public class EbChart : EbControl
    {
        [ProtoBuf.ProtoMember(1)]
        public string ChartType { get; set; }

        [ProtoBuf.ProtoMember(2)]
        public int DataSourceId { get; set; }

        public EbChart() { }

        public override string GetHtml()
        {
            return @"
<div>
    <select id='$$$$$$$_ctype'>
        <option value='line'>Line</option>
        <option value='line'>Pie</option>
        <option value='line'>Doughnut</option>
    </select>
</div>
<div id='$$$$$$$_container' style='width:100%; border: solid 1px; height:50%;'>
    <button id='$$$$$$$_GoptBtn' style='float:right;'>&#9776</button>
    <div style='float:right;margin-left:-20px;'>
        <div id='$$$$$$$_optBox' class='optBox'>
                <div id='$$$$$$$_Gopen' class='opt'>Open..</div>
                <div id='$$$$$$$_Gsave' class='opt'></div>
        </div>
    </div>
    <div id='$$$$$$$_loadingdiv' class='loadingdiv'>
        <img id='loading-image' src='/images/ajax-loader.gif' alt='Loading...' />
    </div>
    <canvas id='$$$$$$$_canvas'></canvas>
</div>

<style>
.loadingdiv {
    //border: solid 1px;
    vertical-align:middle;
    margin-left: 50%;
    display: none;
}
.optBox{
    border: solid 1px #cef;
    background-color:#dff;
    display:inline-block;
    width:75px;
    position: fixed;
    margin-left: -44px;
    padding-left: 5px;
    font-size: 13px;
}

.opt{
    border-top: solid 1px #ddd;
    padding:3px;
}
a.opt{
    color: black;
    border-top: solid 1px #ddd;
    padding:3px;
}
</style>

<script>
$('#$$$$$$$_optBox').hide();
 
var link = document.createElement('a');
link.innerHTML = 'Save..';
link.addEventListener('click', function(ev) {
    var can = document.getElementById('$$$$$$$_canvas');
    link.href = can.toDataURL();
    link.class='opt';
    link.download = 'Chart.png';
}, false);
$('#$$$$$$$_Gsave').append(link);

$('#$$$$$$$_GoptBtn').on('click',function() {
    $('#$$$$$$$_optBox').toggle(20, 'swing', 'show');
});

$('#$$$$$$$_Gsave').on('click',function() {
    $('a').trigger('click');
    $('#$$$$$$$_optBox').hide();
});

$('#$$$$$$$_Gopen').on('click',function() {
    var wi = window.open();
    var html = $('#$$$$$$$_container').html();
    $(wi.document.body).html(html);
    $('#$$$$$$$_optBox').hide();
});

$('#$$$$$$$_loadingdiv').show();
$.get('/ds/data/#######?format=json', function(data) 
{
    var Ydatapoints = [];
    var Xdatapoints = [];
    $.each(data.data, function(i, value) {
        Xdatapoints.push(value[1]);
        Ydatapoints.push(value[2]);
    });
    var ctx = document.getElementById('$$$$$$$_canvas');
    Chart.defaults.global.hover.mode = 'nearest';
    var myChart = new Chart(ctx, {
        type: '@@@@@@@',
        data: {
            labels: Xdatapoints,
            datasets: [{
                label: '@@@@@@@',
                xLabels:['one', 'two'],
                data: Ydatapoints,
                backgroundColor: [
                    'rgba(255, 99, 132, 0.8)',
                    'rgba(54, 162, 235, 0.8)',
                    'rgba(255, 206, 86, 0.8)',
                    'rgba(75, 192, 192, 0.8)',
                    'rgba(153, 102, 255, 0.8)',
                    'rgba(255, 159, 64, 0.8)'
                ],
                borderColor: [
                    'rgba(255,99,132,1)',
                    'rgba(54, 162, 235, 1)',
                    'rgba(255, 206, 86, 1)',
                    'rgba(75, 192, 192, 1)',
                    'rgba(153, 102, 255, 1)',
                    'rgba(255, 159, 64, 1)'
                ],
                borderWidth: 4
            }]
        },
        zoom: { enabled: true },
        options: {
            fixedStepSize:50,
            title: { display: true, text: 'Chart Title', fontSize:30, },
            hover: { mode: 'index' },
            pan: { enabled: true, mode: 'x' },
            responsive: true,
			zoom: { enabled: true, mode: 'x', limits: { max: 10, min: 5 } },
            scales: { xAxes: [{ responsive: true, ticks: { beginAtZero: true} }] }
        }
    });
    $('#$$$$$$$_loadingdiv').hide();
});
</script>
"
.Replace("@@@@@@@", ((string.IsNullOrEmpty(this.ChartType)) ? "bar" : this.ChartType))
.Replace("#######", this.DataSourceId.ToString())
.Replace("$$$$$$$", this.Name);
        }
    }

    [ProtoBuf.ProtoContract]
    public class EbDataGridView : EbControl
    {
        [ProtoBuf.ProtoMember(1)]
        public int DataSourceId { get; set; }

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
</style>
<div class='tablecontainer'>
    <table id='$$$$$$$_tbl' style='width:100%' class='display compact'></table>
</div>
<script>
var cols = [];
$.get('/ds/columns/#######?format=json', function (data)
{
    if (data != null)
        $.each(data.columns, 
            function(i, value) { 
                cols.push({ 'data': value.columnIndex.toString(), 'title': value.columnName }); });

    $('#$$$$$$$_tbl').dataTable(
    {
        lengthMenu: [[100, 500, 1000, 2500, 5000, -1], [100, 500, 1000, 2500, 5000, 'All']],
        serverSide: true,
        processing: true,
        language: { processing: '<div></div><div></div><div></div><div></div><div></div><div></div><div></div>', },
        columns: cols,
        order: [],
        ajax: {
            url: '/ds/data/#######?format=json',
            data: function(dq) { delete dq.columns; },
            dataSrc: function(dd) { return dd.data; }
        },
    });
    $('#$$$$$$$_tbl_filter input').unbind();
    $('#$$$$$$$_tbl_filter input').bind('keyup', function(e) {
        if(e.keyCode == 13) {
            $('#$$$$$$$_tbl').dataTable().fnFilter(this.value);
        }
    });
});
</script>
".Replace("#######", this.DataSourceId.ToString().Trim())
.Replace("$$$$$$$", this.Name); 
        }
    }

    [ProtoBuf.ProtoContract]
    public class EbDataSource : EbObject
    {
        [ProtoBuf.ProtoMember(1)]
        public string Sql { get; set; }
    }
}