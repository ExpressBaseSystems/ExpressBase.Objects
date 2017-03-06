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

        [System.ComponentModel.Category("Behavior")]
        [ProtoBuf.ProtoMember(2)]
        public string DisplayMember { get; set; }

        [System.ComponentModel.Category("Behavior")]
        [ProtoBuf.ProtoMember(3)]
        public string ValueMember { get; set; }

        [ProtoBuf.ProtoMember(4)]
        [System.ComponentModel.Category("Layout")]
        public int DropdownHeight { get; set; }

        [ProtoBuf.ProtoMember(5)]
        public int Value { get; set; }

        [ProtoBuf.ProtoMember(6)]
        public string Text { get; set; }

        [ProtoBuf.ProtoMember(7)]
        [System.ComponentModel.Category("Layout")]
        public int DropdownWidth { get; set; }

        [ProtoBuf.ProtoMember(8)]
        [System.ComponentModel.Category("Behavior")]
        public bool MultiSelect { get; set; }

        [ProtoBuf.ProtoMember(9)]
        [System.ComponentModel.Category("Behavior")]
        public int MaxLimit { get; set; }

        [ProtoBuf.ProtoMember(10)]
        [System.ComponentModel.Category("Behavior")]
        public int MinLimit { get; set; }

        public EbComboBox() { }

        public override string GetHead()
        {
            return this.RequiredString + @"
$('#{0}_loading-image').hide();
var VMindex;
var DMindex;
var DtFlag = false;
var cellTr=null;
function InitDT(){
    $('#{0}_loading-image').show();
    $.get('/ds/columns/#######?format=json', function (data)
    {   var searchTextCollection=[];
        var search_colnameCollection=[];
        var order_colname='';
        var cols = [];
        if (data != null){
            $.each(data.columns,
                function(i, value) {
                    _v = true;
                    _c='dt-left';
                    if(value.columnName=='id')
                        _v = false;

                    if(value.columnName=='{4}')
                        VMindex = value.columnIndex;
                    if(value.columnName=='{5}')
                        DMindex = value.columnIndex;
                    console.log('DMindex=' + DMindex);
                    console.log('VMindex=' + VMindex);
                    


                    if(value.columnIndex==0){                   
                        cols.push({'data':null, 'render': function ( data, type, row ) {return '<input type=\'checkbox\'>'}});                
                    }
                    switch(value.type){
                        case 'System.Int32, System.Private.CoreLib': _c='dt-right'; break;
                        case 'System.Decimal, System.Private.CoreLib':_c='dt-right'; break;
                        case 'System.Int16, System.Private.CoreLib': _c='dt-right'; break;
                        case 'System.DateTime, System.Private.CoreLib':_c='dt-center'; break;
                        case 'System.Boolean, System.Private.CoreLib':_c='dt-center'; break;
                    }
                    cols.push({ data: value.columnIndex, className: _c, title: value.columnName, visible: _v });  
                }
            );
        }

       $('#{0}tbl').dataTable(
        {
            keys: true,
            dom: 'rti',
            autoWidth: true,
            scrollX:true,
            scrollY:{3},
            serverSide: true,
		    columns:cols,
            deferRender: true,
            order:[],
            paging:false,
            select:true,
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
	            },
                dataSrc: function(dd) {
                                $('#{0}_loading-image').hide();

                                setTimeout(function(){
                                     Vobj{0}.updateCk();
                                },1);

                                return dd.data; }
                        }
        
       });
      
        //delayed search on combo searchbox
            $( '#{0}container [type=search]').keyup($.debounce(500, function() {
                    var search = $(this).val().toString();
                    if(search.trim()!==''&& DtFlag){
                        $('#{0}tbl').DataTable().search(search).draw();
                        $('#{0}_loading-image').show();
                    }
            }));

        //double click  option in DD
            $('#{0}tbl tbody').on( 'dblclick', 'tr',function ( e ) {
	            var Vmember=$('#{0}tbl').DataTable().row($(this)).data()[VMindex];
                console.log('FRI' + data.columns.indexOf('tcredit') );
	            var Dmember=$('#{0}tbl').DataTable().row($(this)).data()[DMindex];
                if( !(Vobj{0}.valueMember.contains(Vmember)) ){
                    Vobj{0}.displayMember.push( Dmember );
                    Vobj{0}.valueMember.push( Vmember );
                    $(this).find('[type=checkbox]').prop('checked', true);
                }
            });

        

        //checkbox click event
             $( '#{0}tbl tbody').on('click', 'input[type=\'checkbox\']', function(event){
                var indx;
                $.each(data.columns,function(j,value){
                    if(value.columnName=='id')
                        indx=value.columnIndex;
                });
                var $row = $(this).closest('tr');
                var datas = $('#{0}tbl').DataTable().row($row).data();
               if  (!(Vobj{0}.valueMember.contains(datas[VMindex])) ) {   
                    Vobj{0}.displayMember.push(datas[DMindex]);
                    Vobj{0}.valueMember.push(datas[VMindex]);
                }
               else {
                    Vobj{0}.displayMember.splice(Vobj{0}.displayMember.indexOf(datas[DMindex]),1);
                    Vobj{0}.valueMember.splice(Vobj{0}.valueMember.indexOf(datas[VMindex]),1);
                }  
            });


        //hiding v-select native DD
            $( '#{0}container [class=expand]').css('display', 'none');	

        //hide DD on esc when focused in DD
            $('#{0}tbl').keydown(function(e){
        	        if(e.which==27)
          	            Vobj{0}.hideDD();
            });

        //selection highlighting css on arrow keys
            $('#{0}tbl').DataTable().on( 'key-focus', function ( e, datatable, cell, originalEvent ) {
                var row = datatable.row( cell.index().row );
                cellTr = row.nodes();
                $( row.nodes() ).css('color', '#000');
                $( row.nodes() ).animate({ 'zoom': 1.05 }, 10);
                $(row.nodes()).css('font-weight', 'bold');	
                $(row.nodes()).find('.focus').removeClass('focus');
                $( row.nodes() ).addClass('selected');
            });

            $('#{0}tbl').DataTable().on( 'key-blur', function ( e, datatable, cell ) {
                var row = datatable.row( cell.index().row );
                $(row.nodes()).css('color', '#333');
                $( row.nodes()).animate({ 'zoom': 1 }, 10);
                $(row.nodes()).css('font-weight', 'normal');
                $( row.nodes() ).removeClass('selected');	
            });

        //space & enter on option in DD
             $('#{0}tbl').DataTable().on( 'keyup', function ( e, datatable, cell, originalEvent ) {
                    if(e.which===13||e.which===32){
                        var Vmember=$('#{0}tbl').DataTable().row($(cellTr)).data()[VMindex];
	                    var Dmember=$('#{0}tbl').DataTable().row($(cellTr)).data()[DMindex];
                        if( !(Vobj{0}.valueMember.contains(Vmember)) ){
                            Vobj{0}.displayMember.push( Dmember );
                            Vobj{0}.valueMember.push( Vmember );
                            $(cellTr).find('[type=checkbox]').prop('checked', true);
                        }
                    }
             });

        //filter textbox adding
            $( '#{0}container table:eq(0) thead').append( $( '#{0}container table:eq(0) thead tr').clone() );
		        $( '#{0}container table:eq(0) thead tr:eq(1) th').each( function (idx) {
                        $(this).removeClass('sorting');
                        $(this).css('outline', 'none');
                        $(this).css('padding', '3px 2px');
                        $(this).css('background-color', '#fafffa');

                        var title = $(this).text();
				        var idd= 'header_txt1' + title;  
					        var t = '<span hidden>' + title + '</span>';
                            if(idx!==0){
					            if(data.columns[idx].type=='System.Int32, System.Private.CoreLib'|| data.columns[idx].type=='System.Int16, System.Private.CoreLib'){                
						            $(this).html(t+'<input type=\'number\' id='+idd+' style=\'width: 100%\'/>');                
					            }
					            else if(data.columns[idx].type=='System.String, System.Private.CoreLib')
						            $(this).html(t+'<input type=\'text\' id='+idd+'/>');
					            else if(data.columns[idx].type=='System.DateTime, System.Private.CoreLib'){
						            $(this).html(t+'<input type=\'date\' id='+idd+' style=\'width: 100%\'>');                
					            }
					            else if(data.columns[idx].type=='System.Decimal, System.Private.CoreLib'){                
						            $(this).html(t+'<input type=\'number\' id='+idd+' style=\'width: 100%\'/>');               
					            }
					            else
						            $(this).html(t+'');
                            }
		        });

        //searching  on filters
            $('#{0}container table:eq(0) thead tr:eq(1) th').on('keyup','input',function (e) {
                if(e.which===13){
                    searchTextCollection=[];
                    search_colnameCollection=[];
                    $('#{0}container table:eq(0) thead tr:eq(1) th input').each( function (idx) {
                        if($(this).val().toString().trim()!==''){
                            if($.inArray($(this).siblings().text(), search_colnameCollection) == -1){
                                searchTextCollection.push($(this).val());
                                search_colnameCollection.push($(this).siblings().text());
                            }
                        }
                    });			
                    $('#{0}tbl').DataTable().ajax.reload();
                                setTimeout(function(){
                                     Vobj{0}.updateCk();
                                },1);
                }
            });

	    //sorting when click on columnheader
	        $('#{0}container table:eq(0) thead tr:eq(0)').on('click','th',function(){
                var txt=$(this).text();
                if(txt !== '')
                    order_colname =txt;
                $('#{0}tbl').DataTable().draw();
            });
	       
    });
}

var Vobj{0} = new Vue({
                el: '#{0}container',
                data: {
                    options: [],
                    displayMember:[],
                    valueMember:[],
                    id:'{0}',
                    DDstate:false
                },
                watch: {
                        valueMember: function (val) {
                            console.log('watching' + val);
                            if( {6}===1 && !{8} && val.length >1){
                                console.log('single case= ' + val);
                                this.valueMember = this.valueMember.splice( 1, 1);
                                this.displayMember = this.displayMember.splice( 1, 1);
                            }
                            else 
                                if(val.length > {6}){
                                    this.valueMember = this.valueMember.splice( 0, {6});
                                    this.displayMember = this.displayMember.splice( 0, {6});
                                }
                        }
                },
                methods: {
                    //searchOnGrid() {
                    //    console.log('watching');
                    //    if(this.valueMember.length>1){
                    //        console.log(this.valueMember);
                    //    }
                    //},
                    toggleDD: function(){
                            this.DDstate=!this.DDstate;  
                            setTimeout(function(){
                                $('#{0}container table:eq(0)').css('width', $( '#{0}container table:eq(1)').css('width') );
                            },20);
                    },
                    showDD: function(){
                            if(!DtFlag){
                                   DtFlag = true;
                                   InitDT();
                            }
                            this.DDstate=true;
                            setTimeout(function(){
                                $('#{0}container table:eq(0)').css('width', $( '#{0}container table:eq(1)').css('width') );
                            },20);
                    },
                    hideDD: function(){
                            this.DDstate=false;  
                    },
                    updateCk: function(){
                            $( '#{0}container table:eq(1) tbody [type=checkbox]').each(function(i) {
                                var row = $(this).closest('tr');
                                var datas = $('#{0}tbl').DataTable().row(row).data();
                                if( Vobj{0}.displayMember.contains(datas[DMindex]) ){
                                    $(this).prop('checked', true);
                                }
                                else{
                                    $(this).prop('checked', false);
                                }
			                });  
                            // raise error msg
                            setTimeout(function(){
                                if(Vobj{0}.valueMember.length!==Vobj{0}.displayMember.length)
                                    alert('valueMember and displayMember length miss match found !!!!');
                            },30);
                    }
                }

});

//set id for searchBox
    $( '#{0}container [type=search]').attr('id','{0}srch');

//enter-DDenabling & if'' showall, esc arrow space key based DD enabling , backspace del-valueMember updating
    $( '#{0}container [type=search]').keydown(
		function(e){
            var search = $(this).val().toString();
            if( e.which===13 ){
                    Vobj{0}.showDD();
                if( search.trim()==='' && !DtFlag ){
                    $('#{0}tbl').DataTable().search(search).draw();
                    $('#{0}_loading-image').show();
                }
            }
            if( (e.which===8||e.which===46) &&search==='' && Vobj{0}.valueMember.length>0 ){
                Vobj{0}.valueMember.pop();  
                console.log('Vobj{0}.valueMember' +Vobj{0}.valueMember);
            }
            if( e.which===40 ){
        	    Vobj{0}.showDD();
                $(this).blur();
                $('#{0}DDdiv table:eq(1) td:eq(0)').trigger('click');
            }
            if( e.which===32 )
        	        Vobj{0}.showDD();
            if( e.which===27 )
        	        Vobj{0}.hideDD();
    });

//toggle indicator button
    $( '#{0} [class=open-indicator]').click(function(){
         if(!DtFlag){
                DtFlag = true;
                InitDT();
         }
        Vobj{0}.toggleDD();
    });

//remove ids when tagclose button clicked
    $( '.close').live('click', function(){
                    Vobj{0}.valueMember.splice( delid(), 1);
                    console.log(Vobj{0}.valueMember);
    });

//hide DD when click outside select or DD &  required ( if  not reach minLimit) 
    $(document).mouseup(function (e)
    {
        var container = $('#{0}DDdiv');
        var container1 = $('#{0}');
        if ( (!container.is(e.target)&& container.has(e.target).length === 0) && (!container1.is(e.target)&& container1.has(e.target).length === 0) ){
                     Vobj{0}.hideDD();
                if( Vobj{0}.valueMember.length<{7}&&{7}!==0 )
                    document.getElementById('{0}srch').setCustomValidity('This field  require minimum {7} values');
                else
                    if({9} && Vobj{0}.valueMember.length===0 )
                        document.getElementById('{0}srch').setCustomValidity('This field  is required');
                    else
                        document.getElementById('{0}srch').setCustomValidity('');

        }
    });
  
"
.Replace("{0}", this.Name)
.Replace("#######", this.DataSourceId.ToString().Trim())
.Replace("{3}", (this.DropdownHeight == 0) ? "400" : this.DropdownHeight.ToString())
.Replace("{4}", this.ValueMember.ToString())
.Replace("{5}", this.DisplayMember.ToString())
.Replace("{6}", ( !this.MultiSelect||this.MaxLimit==0 ) ? "1" : this.MaxLimit.ToString())
.Replace("{7}", this.MinLimit.ToString())
.Replace("{8}", this.MultiSelect.ToString().ToLower())
.Replace("{9}", this.Required.ToString().ToLower());

        }


        public override string GetHtml()
        {
            return @"
    <script>
        Vue.component('v-select', VueSelect.VueSelect);
        Vue.config.devtools = true;
    </script>
               
   <div id='{0}container' style='position:absolute; left:{1}px;  top:{2}px;'>
    <v-select id='{0}' style='width:{3}px;' 
        multiple
	    v-model='displayMember'
        :on-change='updateCk'
        placeholder = 'Search...'>
    </v-select>
                <div id='{0}_loadingdiv' class='tbl-loadingdiv'>
                    <img id='{0}_loading-image' width='25' src='/images/ajax-loader.gif' alt='Loading...' />
               </div>
    <div id='{0}DDdiv'v-show='DDstate' class='DDdiv'  style='width:{4}px;'> 
        <table id='{0}tbl' tabindex='1000' class='display'></table>
    </div>
</div>"
.Replace("{0}", this.Name)
.Replace("{1}", this.Left.ToString())
.Replace("{2}", this.Top.ToString())
.Replace("{3}", this.Width.ToString())
.Replace("{4}", (this.DropdownWidth == 0) ? "300" : this.DropdownWidth.ToString());
        }
    }
}