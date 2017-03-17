using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace ExpressBase.Objects
{
    public enum TextTransform
    {
        Normal,
        LowerCase,
        UpperCase,
    }

    public enum TextMode
    {
        SingleLine,
        Email,
        Password,
        Color
    }

    [ProtoBuf.ProtoContract]
    public class EbTextBox : EbControl
    {
        [Browsable(false)]
        public object Parent { get; set; }

        [ProtoBuf.ProtoMember(1)]
        [System.ComponentModel.Category("Behavior")]
        public int MaxLength { get; set; }

        [ProtoBuf.ProtoMember(2)]
        [System.ComponentModel.Category("Behavior")]
        public TextTransform TextTransform { get; set; }

        [ProtoBuf.ProtoMember(3)]
        [System.ComponentModel.Category("Behavior")]
        public TextMode TextMode { get; set; }

        [ProtoBuf.ProtoMember(4)]
        public string PlaceHolder { get; set; }

        [ProtoBuf.ProtoMember(6)]
        [System.ComponentModel.Category("Behavior")]
        public bool AutoCompleteOff { get; set; }

        [ProtoBuf.ProtoMember(5)]
        [System.ComponentModel.Category("Appearance")]
        public string Text { get; set; }

        public EbTextBox() { }

        public EbTextBox(object parent)
        {
            this.Parent = parent;
        }

        public override string GetHead()
        {
            return (((!this.Hidden) ? this.UniqueString + this.RequiredString : string.Empty) +  @"".Replace("{0}", this.Name)
        );
        }

        private string TextTransformString
        {
            get { return (((int)this.TextTransform > 0) ? "$('#{0}').keydown(function(event) { textTransform(this, {1}); }); $('#{0}').on('paste', function(event) { textTransform(this, {1}); });"  .Replace("{0}", this.Name) .Replace("{1}", ((int)this.TextTransform).ToString()) : string.Empty); }
        }

        private string MaxLengthString
        {  
            get { return (this.MaxLength > 0) ? string.Format("maxlength='{0}'", this.MaxLength) : string.Empty; }
        }

        private string TextModeString
        {  
            get { string returnval = string.Empty; switch (this.TextMode) { case TextMode.Email: returnval = "email"; break; case TextMode.Password: returnval = "password"; break; case TextMode.Color: returnval = "color"; break; case TextMode.SingleLine: returnval = "text"; break; } return returnval; }
        }


        public override void SetData(object value)
        {
            this.Text = (value != null) ? value.ToString() : string.Empty;
        }

        public override object GetData()
        {
            return this.Text;
        }

        public override string GetHtml()
        {
            return string.Format(@"
<div style='position:absolute; left:200px; top:200px; {8}'>
    <div style='{19} {20}'>{5}</div>
        <div  class='input-group'>
            {22}
            <input type='{7}' data-toggle='tooltip' title='{11}' name='{0}' id='{0}' {6} style='width:{3}px; height:{4}px; {17} {18} display:inline-block; {21} {10} {9} {13} {14} {15} {16} />
        </div>
    <div class='helpText'> {12} </div>
</div>

",
this.Name, this.Left, this.Top, this.Width,(this.TextModeString== "color") ? this.FontSerialized.SizeInPoints + 14 : this.Height, this.Label, this.MaxLengthString, this.TextModeString,//7
this.HiddenString, (this.Required && !this.Hidden ? " required" : string.Empty), this.ReadOnlyString,//10 
this.ToolTipText,
this.HelpText, "placeholder='"+ this.PlaceHolder +"'", "value='"+ this.Text +"'", "tabindex='" + this.TabIndex + "'",//14
this.AutoCompleteOff ? "autocomplete='off'": string.Empty, "background-color:"+ this.BackColorSerialized +";",//16
"color:" + this.ForeColorSerialized + ";", "background-color:" + this.LabelBackColorSerialized + ";", "color:" + this.LabelForeColorSerialized + ";",//19
(this.FontSerialized!=null) ? (" font-family:"+ this.FontSerialized.FontFamily + ";" + "font-style:" + this.FontSerialized.Style +";" + "font-size:" + this.FontSerialized.SizeInPoints + "px;") : string.Empty,//21
(this.TextMode.ToString() != "SingleLine") ? ("<i class='fa fa-$$$ input-group-addon' aria-hidden='true' style='font-size:" + this.FontSerialized.SizeInPoints + "px;'" + "class='input-group-addon'></i>")
.Replace("$$$", (this.TextMode.ToString() == "Email") ? ("envelope") : (this.TextMode.ToString() == "Password") ? "key" : ("eyedropper")) : string.Empty);//22
        }
    }
}
