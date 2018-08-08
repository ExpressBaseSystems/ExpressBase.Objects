using ExpressBase.Common.Extensions;
using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using ExpressBase.Common.Structures;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace ExpressBase.Objects
{
    [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm)]
    public class EbNumeric : EbControlUI
	{
        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm)]
        public override EbDbTypes EbDbType { get { return EbDbTypes.Decimal; } }
        public EbNumeric()
        {
        }

        [OnDeserialized]
        public void OnDeserializedMethod(StreamingContext context)
        {
            this.BareControlHtml = this.GetBareHtml();
            this.ObjType = this.GetType().Name.Substring(2, this.GetType().Name.Length - 2);
        }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm)]
        public int MaxLength { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm)]
        public int DecimalPlaces { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm)]
        public decimal Value { get; set; }

		[HideInPropertyGrid]
		[EnableInBuilder(BuilderType.BotForm)]
		public override bool IsReadOnly { get => this.ReadOnly; }
		
		[EnableInBuilder(BuilderType.BotForm)]
		public bool AutoIncrement { get; set; }

		//[ProtoBuf.ProtoMember(4)]
		private string PlaceHolder
        {
            get
            {
                //for ( int i=0; i< this.DecimalPlaces; i++)
                //    res += "0";
                return (this.DecimalPlaces == 0) ? "0" : "0." + new String('0', this.DecimalPlaces);
            }
        }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm)]
        public bool AllowNegative { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm)]
        public bool IsCurrency { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm)]
        public bool AutoCompleteOff { get; set; }

        //private string MaxLengthString
        //{
        //    get { return ((this.MaxLength > 0) ? "$('#{0}').focus(function() {$(this).select();});".Replace("{0}", this.Name).Replace("{1}", this.Value.ToString()) : string.Empty); }
        //}

        public override string GetToolHtml()
        {
            return @"<div eb-type='@toolName' class='tool'><b>0-9 </b></i>  @toolName</div>".Replace("@toolName", this.GetType().Name.Substring(2));
        }

        public override string GetHead()
        {
            return (((!this.Hidden) ? this.UniqueString + this.RequiredString : string.Empty) + @"
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

        public override string GetDesignHtml()
        {
            return GetHtmlHelper(RenderMode.Developer).RemoveCR().DoubleQuoted();
        }

        public override string GetHtml()
        {
            return GetHtmlHelper(RenderMode.User);
        }

        public override string GetBareHtml()
        {
            return @" 
        <div class='input-group' style='width:100%;'>
                <span style='font-size: @fontSize@' class='input-group-addon'>$</span>   
                <input type='text' data-ebtype='@datetype@' class='numinput' ui-inp id='@name@' name='@name@' value='@value@' @placeHolder autocomplete = '@autoComplete@' data-toggle='tooltip' title='@toolTipText@' style=' width:100%; @backColor@ @foreColor@ @fontStyle@ display:inline-block; @readOnlyString@ @required@ @tabIndex@ />
        </div>"
.Replace("@name@", this.Name)
.Replace("@toolTipText@", this.ToolTipText)
.Replace("@autoComplete@", this.AutoCompleteOff ? "off" : "on")
.Replace("@value@", "")//"value='" + this.Value + "'")
.Replace("@tabIndex@", "tabindex='" + this.TabIndex + "'")
    .Replace("@BackColor@ ", ("background-color:" + ((this.BackColor != null) ? this.BackColor : "@BackColor@ ") + ";"))
    .Replace("@ForeColor@ ", "color:" + ((this.ForeColor != null) ? this.ForeColor : "@ForeColor@ ") + ";")
.Replace("@required@", " required")//(this.Required && !this.Hidden ? " required" : string.Empty))
.Replace("@readOnlyString@", this.ReadOnlyString)
.Replace("@placeHolder@", "placeholder='" + this.PlaceHolder + "'")
.Replace("@datetype@", "11")
//.Replace("@fontStyle@", (this.FontSerialized != null) ?
//                            (" font-family:" + this.FontSerialized.FontFamily + ";" + "font-style:" + this.FontSerialized.Style
//                            + ";" + "font-size:" + this.FontSerialized.SizeInPoints + "px;")
//                        : string.Empty)
;
        }

        private string GetHtmlHelper(RenderMode mode)
        {
            return (@"
<div id='cont_@name@' class='Eb-ctrlContainer' Ctype='Numeric' style='@hiddenString'>
    <div class='eb-ctrl-label' id='@nameLbl' style='@lblBackColor @LblForeColor'>@label@</div>
       @barehtml@            
    <span class='helpText'> @helpText </span>
</div>"
.Replace("@barehtml@", this.GetBareHtml())
.Replace("@name@", this.Name)
.Replace("@left", this.Left.ToString())
.Replace("@top", this.Top.ToString())
.Replace("@height", this.Height.ToString())
.Replace("@label@", this.Label)//5
.Replace("@hiddenString", this.HiddenString)
.Replace("@required", (this.Required && !this.Hidden ? " required" : string.Empty))
.Replace("@readOnlyString", this.ReadOnlyString)
.Replace("@toolTipText", this.ToolTipText)
.Replace("@helpText", this.HelpText)//10
.Replace("@placeHolder", "placeholder='" + this.PlaceHolder + "'")
.Replace("@tabIndex", "tabindex='" + this.TabIndex + "'")
.Replace("@autoComplete", this.AutoCompleteOff ? "off" : "on")
//.Replace("@backColor", "background-color:" + this.BackColorSerialized + ";")
//.Replace("@foreColor", "color:" + this.ForeColorSerialized + ";")//15
//.Replace("@lblBackColor", "background-color:" + this.LabelBackColorSerialized + ";")
//.Replace("@LblForeColor", "color:" + this.LabelForeColorSerialized + ";")
//.Replace("@value", (this.Value == 0) ? "''" : this.Value.ToString())
//.Replace("@fontStyle", (this.FontSerialized != null) ?
//                            (" font-family:" + this.FontSerialized.FontFamily + ";" + "font-style:" + this.FontSerialized.Style
//                            + ";" + "font-size:" + this.FontSerialized.SizeInPoints + "px;")
//                        : string.Empty)
//.Replace("@fontSize", (this.FontSerialized != null) ? (this.FontSerialized.SizeInPoints + "px;") : string.Empty)
);
        }
    }
}
