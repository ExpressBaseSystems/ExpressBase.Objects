using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExpressBase.Objects
{
    [ProtoBuf.ProtoContract]
    public class EbComboBox : EbControl
    {
        [ProtoBuf.ProtoMember(1)]
        public int DataSourceId { get; set; }

        [ProtoBuf.ProtoMember(2)]
        public string DisplayMember { get; set; }

        [ProtoBuf.ProtoMember(3)]
        public int ValueMember { get; set; }

        [ProtoBuf.ProtoMember(4)]
        [System.ComponentModel.Category("Layout")]
        public int DropdownHeight { get; set; }

        [ProtoBuf.ProtoMember(5)]
        public int Value { get; set; }

        [ProtoBuf.ProtoMember(6)]
        public string Text{ get; set; }

        public EbComboBox() { }

        public override string GetHead()
        {
            return @"
//var {0}Options= ['one','two'];
//var {0}Options= [];
//$.get('/ds/data/#######?format=json', function(data) 
//{
//     alert(data);
//     $.each(data.data, function(i, value) { 
//            {0}Options.push(value[1]);
//     }); 
//       alert( {0}Options );
//});



$.get('/ds/columns/#######?format=json', function (data)
{
    var cols = [];
    if (data != null){
        $.each(data.columns,
            function(i, value) {
                cols.push({ 'data': value.columnIndex, 'title': value.columnName});  
            }
        );
    }

   $('#{0}tbl').dataTable(
    {
        keys: true,
        dom: 'rti',
        autoWidth: false,
        scrollX:true,
        scrollY:300,
        serverSide: true,
		columns:cols,
        deferRender: true,
        order:[],
        paging:false,
        ajax: {
            url: '/ds/data/#######?format=json',
            data: function(dq) {
                    delete dq.columns;
                },
            dataSrc: function(dd) {return dd.data; }
        }
        
   });

    $('#{0}').click(function(){
    console.log('on focus');
    Vobj{0}.toggleDD();
    });

    $('#{0}tbl tbody').on('dblclick', 'td', function(e){
        var item = $('#{0}tbl').DataTable().cell(this).data();
        if( !(Vobj{0}.options.contains(item)) )
            Vobj{0}.options.push( item );
        else
            Vobj{0}.options.splice(Vobj{0}.options.indexOf(item),1);
    }); 

    $('#{0}tbl tbody').on('dblclick', 'td', function(e){
        
    });

    
});

var Vobj{0} = new Vue({
                el: '#{0}container',
                data: {
                    options: [],
                    id:'{0}',
                    DDstate:false
                },
                methods: {
                    searchOnGrid(search) {
                        $('#{0}tbl').DataTable().search( search ).draw();
                    },
                    toggleDD: function(){
                            this.DDstate=!this.DDstate;  
                    },
                    showDD: function(){
                            this.DDstate=true;  
                    },
                    hideDD: function(){
                            this.DDstate=false;  
                    }
                }

});


$(document).mouseup(function (e)
{
    var container = $('#{0}container');
    if (!container.is(e.target)&& container.has(e.target).length === 0)
                 Vobj{0}.hideDD();
});

    //$('#{0}tbl').dataTable({ 
    //    keys: true,
    //    dom: 'rti'
    //});   

  
"
.Replace("{0}", this.Name)
.Replace("#######", this.DataSourceId.ToString().Trim());
        }

        public override string GetHtml()
        {
            return string.Format(@"
    <script>
        Vue.component('v-select', VueSelect.VueSelect);
        Vue.config.devtools = true;
    </script>

<div id='{0}container' style='position:absolute; left:{1}px; min-height: 12px; top:{2}px;'>
    <v-select id='{0}' multiple 
	    :on-search = 'searchOnGrid'
	    v-model='options'
        placeholder = 'Search...'>
    </v-select>
    <div v-show='DDstate' class='DDdiv'> 
        <table id='{0}tbl' class='display'></table>
    </div>
</div>",
this.Name, this.Left, this.Top)
.Replace("#######", this.DataSourceId.ToString());
        }
    }
}
