using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExpressBase.Objects
{
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
<div id='$$$$$$$_contnr' style=' border:solid 1px #79e; margin:1px;' >
    <div id='$$$$$$$_chartMenuDiv' class='optBox'>
            <div style='display:inline-block; margin-left:5%'> <h5>^^^^^^^</h5> </div>
            <div style='float:right;margin-left:-20px; display:inline-block;'>
                <select id='$$$$$$$_ctype'>
                    <option value='bar'>            Bar             </option>
                    <option value='line'>           Line            </option>
                    <option value='pie'>            Pie             </option>
                    <option value='doughnut'>       Doughnut        </option>
                    <option value='radar'>          Radar           </option>
                    <option value='polarArea'>      Polar Area      </option>
                    <option value='bubble'>         Bubble          </option>
                    <option value='scatter'>        Scatter         </option>
                    <option value='horizontalBar'>  Horizontal Bar  </option>
                </select>
                <div id='$$$$$$$_Gsave' class='opt'> </div>
                <button id='$$$$$$$_expand'><img id='$$$$$$$_expandIcon' src='http://localhost:53125/images/Expand-16.png' /></button>
            </div>
    </div>

    <div id='$$$$$$$_container' class='contnr' style='overflow-x:auto; height:45em; overflow-y:auto;'>
        <div id='$$$$$$$_loadingdiv' class='loadingdiv'>
            <img id='$$$$$$$_loading-image' src='/images/ajax-loader.gif' alt='Loading...' />
        </div>
        <div id='$$$$$$$_canvasDiv' style=' min-width:100%;'>
            <canvas id='$$$$$$$_chartCanvas' style='min-width:100%;  '></canvas>
        </div>  
    </div>
</div>
<style>
.contnr {
    width:100%;
}
.loadingdiv {
    vertical-align:middle;
    margin-left: 50%;
    margin-top: 50%;
    display: none;
}
.optBox {
    background-color:#79e;
}
.opt {
    border:solid 1px #DDD;
    display:inline-block;
    background-color:#fff;
}

</style>
<script>
var myChart = null;
var chartConfig = null;
var link = document.createElement('a');
link.innerHTML = '<img id=\'$$$$$$$_saveIcon\' src=\'http://localhost:53125/images/Save-16.png \' /> ';
link.addEventListener('click', function(ev) {
    var can = document.getElementById('$$$$$$$_chartCanvas');
    link.href = can.toDataURL();
    link.id='$$$$$$$_saveLink';
    link.class='opt';
    link.download = 'Chart.png';
}, false);
$('#$$$$$$$_Gsave').append(link);

$('#$$$$$$$_expand').on('click',function() {
    var wi = window.open();
    var html = $('#$$$$$$$_container').html();
    $(wi.document.body).html(html);
});

$('#$$$$$$$_ctype').on('change', function() { chartConfig.type=$('#$$$$$$$_ctype').val(); myChart.update(); });
$('#$$$$$$$_loadingdiv').show();
$.get('/ds/data/#######?format=json', function(data) 
{
    var Ydatapoints = [];
    var Xdatapoints = [];
    $.each(data.data, function(i, value) { Xdatapoints.push(value[1]); Ydatapoints.push(value[2]); });
    if( ('@@@@@@@'!=='pie') && ('@@@@@@@'!=='doughnut') )
    {
        var canWidth=data.data.length*3;
        $('#$$$$$$$_canvasDiv').css('width', canWidth + '%');// canvasDiv zooming(height fixed)
        $('#$$$$$$$_chartCanvas').css('min-height', 42+'em');// to fit small graph into container height
    }
    var ctx = document.getElementById('$$$$$$$_chartCanvas');
    Chart.defaults.global.hover.mode = 'nearest';
    chartConfig = {
        type: '@@@@@@@',
        data: {
            labels: Xdatapoints,
            datasets: [{
                label:'Graph',
                xLabels:['one'],
                data: Ydatapoints,
                backgroundColor: [
                    'rgba(255, 99, 132, 0.8)',
                    'rgba(54, 162, 235, 0.8)',
                    'rgba(255, 206, 86, 0.8)',
                    'rgba(75, 192, 192, 0.8)',
                    'rgba(153, 102, 255, 0.8)',
                    'rgba(255, 159, 64, 0.8)',
                    'rgba(255, 99, 132, 0.8)',
                    'rgba(54, 162, 235, 0.8)',
                    'rgba(255, 206, 86, 0.8)',
                    'rgba(75, 192, 192, 0.8)',
                    'rgba(153, 102, 255, 0.8)',
                    'rgba(255, 159, 64, 0.8)'
                ]
            }]
        },
        zoom: { enabled: true },
        options: {
            fixedStepSize:50,
            hover: { mode: 'index' },
            pan: { enabled: true, mode: 'x' },
            responsive: true,
            zoom: { enabled: true, mode: 'x', limits: { max: 10, min: 5 } },
            scales: { xAxes: [{ responsive:false , ticks: { beginAtZero: true} }],
            yAxes: [{
                type: 'logarithmic',
            }] }
        }
    };
    myChart = new Chart(ctx, chartConfig);
$('#$$$$$$$_loadingdiv').hide();
});
</script>"
.Replace("@@@@@@@", ((string.IsNullOrEmpty(this.ChartType)) ? "bar" : this.ChartType))
.Replace("#######", this.DataSourceId.ToString())
.Replace("$$$$$$$", this.Name)
.Replace("^^^^^^^", this.Label);
        }
    }
}
