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
        [System.ComponentModel.Category("Behavior")]
        public string PlaceHolder { get; set; }

        [ProtoBuf.ProtoMember(5)]
        [System.ComponentModel.Category("Appearance")]
        public string Text { get; set; }

        [ProtoBuf.ProtoMember(6)]
        [System.ComponentModel.Category("Behavior")]
        public bool AutoCompleteOff { get; set; }

        [ProtoBuf.ProtoMember(7)]
        [System.ComponentModel.Category("Behavior")]
        public string MaxDateExpression { get; set; }

        [ProtoBuf.ProtoMember(8)]
        [System.ComponentModel.Category("Behavior")]
        public string MinDateExpression { get; set; }

        public EbTextBox() { }

        public EbTextBox(object parent)
        {
            this.Parent = parent;
        }

        public override string GetHead()
        {
            return ( ((!this.Hidden) ? this.UniqueString + this.RequiredString : string.Empty) + @"".Replace("{0}", this.Name) );
        }

        private string TextTransformString
        {
            get { return (((int)this.TextTransform > 0) ? "$('#{0}').keydown(function(event) { textTransform(this, {1}); }); $('#{0}').on('paste', function(event) { textTransform(this, {1}); });".Replace("{0}", this.Name).Replace("{1}", ((int)this.TextTransform).ToString()) : string.Empty); }
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
            return @"
<div style='position:absolute; left:@leftpx; top:@toppx; @hiddenString'>
    <span id='@nameLbl' style='@lblBackColor @LblForeColor'>@label</span>
        <div  class='input-group' style='width: 1px;'>
            @attachedLbl
            <input type='@textMode'  id='@name' name='@name' autocomplete = '@autoComplete' data-toggle='tooltip' title='@toolTipText' @tabIndex @maxLength style='width:@widthpx; height:@heightpx; @backColor @foreColor display:inline-block; @fontStyle @readOnlyString @required @placeHolder @text @tabIndex  />
        </div>
    <span class='helpText'> @helpText </span>
</div>"
.Replace("@name", this.Name)
.Replace("@left", this.Left.ToString())
.Replace("@top", this.Top.ToString())
.Replace("@width", this.Width.ToString())
.Replace("@height", (this.TextMode.ToString().ToLower() == "color" && this.Height < 24) ? (this.FontSerialized.SizeInPoints + 14).ToString() : this.Height.ToString())
.Replace("@label", this.Label)
.Replace("@maxLength", (this.MaxLength > 0) ? string.Format("maxlength='{0}'", this.MaxLength) : string.Empty)
.Replace("@textMode", this.TextMode.ToString().ToLower())
.Replace("@hiddenString", this.HiddenString)
.Replace("@required", (this.Required && !this.Hidden ? " required" : string.Empty))
.Replace("@readOnlyString", this.ReadOnlyString)
.Replace("@toolTipText", this.ToolTipText)
.Replace("@helpText", this.HelpText)
.Replace("@placeHolder", "placeholder='" + this.PlaceHolder + "'")
.Replace("@text", "value='" + this.Text + "'")
.Replace("@tabIndex", "tabindex='" + this.TabIndex + "'")
.Replace("@autoComplete", (this.AutoCompleteOff || this.TextMode.ToString().ToLower() == "password") ? "off" : "on")
.Replace("@backColor", "background-color:" + this.BackColorSerialized + ";")
.Replace("@foreColor", "color:" + this.ForeColorSerialized + ";")
.Replace("@lblBackColor", "background-color:" + this.LabelBackColorSerialized + ";")
.Replace("@LblForeColor", "color:" + this.LabelForeColorSerialized + ";")
.Replace("@fontStyle", (this.FontSerialized != null) ?
                            (" font-family:" + this.FontSerialized.FontFamily + ";" + "font-style:" + this.FontSerialized.Style 
                            + ";" + "font-size:" + this.FontSerialized.SizeInPoints + "px;")
                        : string.Empty)
.Replace("@attachedLbl", (this.TextMode.ToString() != "SingleLine") ?
                                (
                                    "<i class='fa fa-$class input-group-addon' aria-hidden='true'" 
                                    + ( 
                                        (this.FontSerialized != null) ? 
                                            ("style='font-size:" + this.FontSerialized.SizeInPoints + "px;'")
                                        : string.Empty
                                      )
                                    + "class='input-group-addon'></i>"
                                )
                                .Replace("$class", (this.TextMode.ToString() == "Email") ?
                                                            ("envelope")
                                                        : (this.TextMode.ToString() == "Password") ?
                                                            "key"
                                                        : ("eyedropper")
                                )
                        : string.Empty);
        }
    }
}
