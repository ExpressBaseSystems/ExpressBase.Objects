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

        [ProtoBuf.ProtoMember(7)]
        [System.ComponentModel.Category("Behavior")]
        public bool AutoCompleteOff { get; set; }

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

//$('#j').mask('00/00/0000');

$('#{0}').mask('SZZZZZZZZZZZ', {
    //reverse: true,
    translation: {
        'S':{
            pattern: /-/,
            optional:true
            },
        'Z': {
                pattern: /[0-9.]/,
                optional: true
            }
    }
}); ").Replace("{0}", this.Name).Replace("{1}", "SZZZZZZZZZZZ").Replace("{2}", this.DecimalPlaces.ToString()).Replace("{3}", (this.MaxLength - this.DecimalPlaces).ToString());
        }

        public override string GetHtml()
        {
            return string.Format(@"




<div style='position:absolute; min-height: 12px; left:@leftpx; top:@toppx; @hiddenString'>
    <span id='@nameLbl' style='@lblBackColor @LblForeColor'>@label</span>
            <div  class='input-group'>
                 {19}   
                 <input type='text'  class='numinput' name='@name' value='@value' @placeHolder autocomplete = '@autoComplete' data-toggle='tooltip' title='@toolTipText' id='@name' style='width:@widthpx; height:@heightpx; @backColor @foreColor {17} display:inline-block;@readOnlyString @required @tabIndex />
            </div>
    <span class='helpText'> @helpText </span>
</div>",
this.Name, this.Left, this.Top, this.Width, this.Height, this.Label, //5
this.HiddenString, (this.Required && !this.Hidden ? " required" : string.Empty), this.ReadOnlyString,//8
this.ToolTipText,//9
this.HelpText, "tabindex='" + this.TabIndex + "'", "background-color:" + this.BackColorSerialized + ";",//12
"color:" + this.ForeColorSerialized + ";",  "background-color:" + this.LabelBackColorSerialized + ";",//14
"color:" + this.LabelForeColorSerialized + ";", (this.Value==0) ? "''" : this.Value.ToString() ,//16
(this.FontSerialized != null) ? (" font-family:" + this.FontSerialized.FontFamily + ";" + "font-style:" + this.FontSerialized.Style + ";" + "font-size:" + this.FontSerialized.SizeInPoints + "px;") : string.Empty,//17
this.PlaceHolder, (this.IsCurrency) ? ("<span style='font-family:" + this.FontSerialized.FontFamily + ";" + "font-size:" + this.FontSerialized.SizeInPoints + "px;'" +  "class='input-group-addon'>$</span>") : string.Empty)//19
.Replace("@name", this.Name)
.Replace("@left", this.Left.ToString())
.Replace("@top", this.Top.ToString())
.Replace("@width", this.Width.ToString())
.Replace("@height", this.Height.ToString())
.Replace("@label", this.Label)//5
.Replace("@hiddenString", this.HiddenString)
.Replace("@required", (this.Required && !this.Hidden ? " required" : string.Empty))
.Replace("@readOnlyString", this.ReadOnlyString)
.Replace("@toolTipText", this.ToolTipText)
.Replace("@helpText", this.HelpText)//10
.Replace("@placeHolder", "placeholder='" + this.PlaceHolder + "'")
.Replace("@tabIndex", "tabindex='" + this.TabIndex + "'")
.Replace("@autoComplete", this.AutoCompleteOff  ? "off" : "on")
.Replace("@backColor", "background-color:" + this.BackColorSerialized + ";")
.Replace("@foreColor", "color:" + this.ForeColorSerialized + ";")
.Replace("@lblBackColor", "background-color:" + this.LabelBackColorSerialized + ";")
.Replace("@LblForeColor", "color:" + this.LabelForeColorSerialized + ";")
.Replace("@value",  (this.Value == 0) ? "''" : this.Value.ToString())
//.Replace("@fontStyle", (this.FontSerialized != null) ?
//                            (" font-family:" + this.FontSerialized.FontFamily + ";" + "font-style:" + this.FontSerialized.Style
//                            + ";" + "font-size:" + this.FontSerialized.SizeInPoints + "px;")
//                        : string.Empty)
//.Replace("@attachedLbl", (this.TextMode.ToString() != "SingleLine") ?
//                                (
//                                    "<i class='fa fa-$class input-group-addon' aria-hidden='true'"
//                                    + (
//                                        (this.FontSerialized != null) ?
//                                            ("style='font-size:" + this.FontSerialized.SizeInPoints + "px;'")
//                                        : string.Empty
//                                      )
//                                    + "class='input-group-addon'></i>"
//                                )
//                                .Replace("$class", (this.TextMode.ToString() == "Email") ?
//                                                            ("envelope")
//                                                        : (this.TextMode.ToString() == "Password") ?
//                                                            "key"
//                                                        : ("eyedropper")
//                                )
//                        : string.Empty)
;
        }
    }
}
