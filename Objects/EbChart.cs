using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExpressBase.Objects
{
    [ProtoBuf.ProtoContract]
    [EnableInBuilder(BuilderType.WebFormBuilder, BuilderType.FilterDialogBuilder)]
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
<div id='$$$$$$$_contnr' class='graph-container'>
    <div id='$$$$$$$_chartMenuDiv' class='graph-headBox'>
            <div class='graph-head'>  ^^^^^^^ </div>
            <div class='graph-optBox'>
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
                <div id='$$$$$$$_Gsave' class='graph-opt'> </div>
                <button id='$$$$$$$_expand'><img id='$$$$$$$_expandIcon' src='http://localhost:53125/images/Expand-16.png' /></button>
            </div>
    </div>
    <div id='$$$$$$$_container' class='dygraph-divContainer'>
        <div id='$$$$$$$_loadingdiv' class='graph-loadingdiv'>
            <img id='$$$$$$$_loading-image' src='/images/ajax-loader.gif' alt='Loading...' />
        </div>
        <div id='$$$$$$$_canvasDiv' class='dygraph-Wrapper'>
            <div id='$$$$$$$_graphdiv' style='width:100%; margin-top:11px;'></div>
        </div>  
    </div>
</div>

<script>
var link = document.createElement('a');
link.innerHTML = '<img id=\'$$$$$$$_saveIcon\' src=\'http://localhost:53125/images/Save-16.png \' /> ';
link.addEventListener('click', function(ev) {
    Dygraph.Export.asPNG(graph, img);
    link.href = img.src.replace('image/png','image/octet-stream');
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
$('#$$$$$$$_loadingdiv').show();
$.get('/ds/data/#######?format=json', function(data) 
{
    var Y2datapoints = [];
    var dta='';
    var barX=0;  
    var test = 100;
     
     dta ='X,Y1,Y2,Y3\n';
     $.each(data.data, function(i, value) { 
        dta += i;
        for(k=2;k<value.length;k++)
            dta += ','+ value[k];
        dta += '\n';
     });
    if( ('@@@@@@@'!=='pie') && ('@@@@@@@'!=='doughnut') )
    {
        var canWidth=data.data.length*3;
        $('#$$$$$$$_canvasDiv').css('width', canWidth + '%');// canvasDiv zooming(height fixed)
        $('#$$$$$$$_chartCanvas').css('min-height', 42+'em');// to fit small graph into container height
    }   

    colorCollection.push(getRandomColor());
    var ctx = document.getElementById('$$$$$$$_chartCanvas');
    g = new Dygraph(
                document.getElementById('$$$$$$$_graphdiv'),
                dta,
                {
                    ylabel: 'Y Label',
                    xlabel: 'X Label',
                    legend: 'always',
                    animatedZooms: true,
                    'Y1': {
                        plotter: dy_plotters.barChartPlotter
                     },
    				'Y2': {
                        plotter: dy_plotters.barChartPlotter
                    },
                    'Y3': {
                        plotter: dy_plotters.barChartPlotter
                    },
                    'Y4': {
                        plotter: dy_plotters.barChartPlotter
                    },
                    'Y5': {
                        plotter: dy_plotters.barChartPlotter
                    },
                    axes: {
                        x: {
                            valueFormatter: function(x) {
                                    return (x < data.data.length) ? data.data[x][1].toString() : '';
                                },
                            axisLabelFormatter: function (x) {
                                return (x < data.data.length) ? data.data[x][1].toString() : '';
                            },
                        },
                        y: {
                            valueFormatter: function(y) {
                                    return y;
                                },
                            axisLabelFormatter: function (y) {
                                y=y.toString();

                                if(y.slice(-6)==='000000')
                                    return y.slice(0, -6)+'M';
                                
                                else if(y.slice(-3)==='000')
                                    return y.slice(0, -3)+'K';
                                else 
                                    return y;
                            },
                        },
                    },
                    logscale: true
                } 
            );
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
