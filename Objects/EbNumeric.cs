using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace ExpressBase.Objects
{
    [ProtoBuf.ProtoContract]
    public class EbNumeric : EbControl
    {
        public EbNumeric() { }

        public EbNumeric(object parent)
        {
            this.Parent = parent;
        }

        [ProtoBuf.ProtoMember(1)]
        [System.ComponentModel.Category("Behavior")]
        public int MaxLength { get; set; }

        [ProtoBuf.ProtoMember(2)]
        [System.ComponentModel.Category("Behavior")]
        public int DecimalPlaces { get; set; }

        [ProtoBuf.ProtoMember(3)]
        [System.ComponentModel.Category("Appearance")]
        public decimal Value { get; set; }

        //[ProtoBuf.ProtoMember(4)]
        private string PlaceHolder
        {
            get {
                //for ( int i=0; i< this.DecimalPlaces; i++)
                //    res += "0";
                return (this.DecimalPlaces == 0) ? "0" :  "0." + new String('0', this.DecimalPlaces);
            }
        }

        [ProtoBuf.ProtoMember(5)]
        [System.ComponentModel.Category("Behavior")]
        public bool AllowNegative { get; set; }

        [ProtoBuf.ProtoMember(6)]
        [System.ComponentModel.Category("Behavior")]
        public bool IsCurrency { get; set; }

        //private string MaxLengthString
        //{
        //    get { return ((this.MaxLength > 0) ? "$('#{0}').focus(function() {$(this).select();});".Replace("{0}", this.Name).Replace("{1}", this.Value.ToString()) : string.Empty); }
        //}

        public override string GetHead()
        {
            return ( ((!this.Hidden) ? this.UniqueString  + this.RequiredString : string.Empty) + @"
$('#{0}').focusout( function(){   
       var val=$(this).val().toString();
       var l = '{1}'.length-1;
       var ndp = {2}; 
       if( val==0 || val==='' || val==='.')
            $(this).val('');
       else{
            if(ndp!==0){
                if( (!val.includes('.'))  && (l!==val.length))
                    val = val + '.';
                if ((val.includes('.')))
                {
                    var pi = val.indexOf('.');
                    var lmt = pi + ndp;
                    for (pi; pi <= l; pi++)
                    {
                        if (val[pi] == null)
                            val += '0';
                        if (pi === lmt)
                            break;
                    }
                }
             }
             if(val[0]==='.')
                val='0'+val;
             $(this).val(val);
        }
    });

$('#{0}').focus(function() {$(this).select();});         
$('#{0}').keypress(function(e) { 

        var val = $('#{0}').val();
        var cs = document.getElementById('{0}').selectionStart;
        var ce = document.getElementById('{0}').selectionEnd;
            if(e.which==46 && val.includes('.')) {
                 setTimeout(function () {
                        $('#{0}').val(val);
                 }, 1);
            }
            // containes '.' and no selection
            if(val.includes('.') && cs === ce ){
                setTimeout(function () {
                    var pi = val.indexOf('.');  
                    //prevents exceeding decimal part length when containes '.'
                    if( (val.length-pi) === ({2} + 1) &&  (e.which >=48)&&(e.which<=57) && ce>pi )
                        $('#{0}').val(val);
                    //prevents exceeding integer part length when containes '.'
                    if( pi === {3} &&  (e.which >=48)&&(e.which<=57) && ce<=pi)
                        $('#{0}').val(val);

                }, 1);
            }
            //prevents exceeding integer-part length when no '.'
            if(!(val.includes('.')) && val.length === {3} && (e.which >=48)&&(e.which<=57)  ){
                setTimeout(function () {
                   $('#{0}').val( val + '.' + String.fromCharCode(e.which));

                }, 1);
            }
            //prevents del before '.'if it leads to exceed integerpart limit
            if( val.includes('.') && (val.length-1)>{3}  && cs===val.indexOf('.') && e.which===0 ){
                setTimeout(function () {
                   $('#{0}').val(val);
                }, 1);
            }
            //prevents <- after '.' if it leads to exceed integerpart limit
            if( val.includes('.') && (val.length-1)>{3}  && cs===(val.indexOf('.')+1) && e.which===8 ){
                setTimeout(function () {
                   $('#{0}').val(val);
                }, 1);
            }
            //prevents deletion of selection when containes '.' if it leads to exceed integerpart limit
            if( (val.includes('.') && val.length-(ce-cs))>{3} &&  cs<=val.indexOf('.') && ce>val.indexOf('.') ){
                setTimeout(function () {
                   $('#{0}').val(val);
                }, 1);
            }
    });

$('#j').mask('00/00/0000');

//$('#{0}').mask('SZZZZZZZZZZZ', {
//    //reverse: true,
//    translation: {
//        'S':{
//            pattern: /-/,
//            optional:true
//            },
//        'Z': {
//                pattern: /[0-9.]/,
//                optional: true
//            }
//    }
//}); ").Replace("{0}", this.Name).Replace("{1}", "SZZZZZZZZZZZ").Replace("{2}",this.DecimalPlaces.ToString() ).Replace("{3}", (this.MaxLength-this.DecimalPlaces).ToString() ); 
        }

        public override string GetHtml()
        {
            return string.Format(@"




<div style='position:absolute; left:{1}px; min-height: 12px; top:{2}px; {6}'>
    <div id='{0}Lbl' style='{14} {15}'>{5}</div>
            <div  class='input-group'>
                 {19}   
                 <input type='text'  class='numinput' name='{0}' value={16} placeholder='{18}'  data-toggle='tooltip' title='{9}' id='{0}' style='width:{3}px; height:{4}px; {12} {13} {17} {1} display:inline-block;{8} {7} {11} />
            </div>
    <div class='helpText'> {10} </div>
</div>",
this.Name, this.Left, this.Top, this.Width, this.Height, this.Label, //5
this.HiddenString, (this.Required && !this.Hidden ? " required" : string.Empty), this.ReadOnlyString,//8
this.ToolTipText,//9
this.HelpText, "tabindex='" + this.TabIndex + "'", "background-color:" + this.BackColorSerialized + ";",//12
"color:" + this.ForeColorSerialized + ";",  "background-color:" + this.LabelBackColorSerialized + ";",//14
"color:" + this.LabelForeColorSerialized + ";", (this.Value==0) ? "''" : this.Value.ToString() ,//16
(this.FontSerialized != null) ? (" font-family:" + this.FontSerialized.FontFamily + ";" + "font-style:" + this.FontSerialized.Style + ";" + "font-size:" + this.FontSerialized.SizeInPoints + "px;") : string.Empty,//17
this.PlaceHolder, (this.IsCurrency) ? ("<span style='font-family:" + this.FontSerialized.FontFamily + ";" + "font-size:" + this.FontSerialized.SizeInPoints + "px;'" +  "class='input-group-addon'>$</span>") : string.Empty);//19
        }
    }
}
