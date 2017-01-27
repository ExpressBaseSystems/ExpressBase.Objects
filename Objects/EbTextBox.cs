using System;
using System.Collections.Generic;
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

        public EbTextBox() { }

        public override string GetHead()
        {
            return ((!this.Hidden) ? this.UniqueString + this.RequiredString : string.Empty) + this.TextTransformString;
        }

        public override string GetHtml()
        {
            return string.Format(@"
<div style='position:absolute; left:{1}px; top:{2}px; {8}'>
<div>{5}</div>
<input type='{7}' name='{10}{0}' id='{0}' {6} style='width:{3}px; height:{4}px;  display:inline-block;' {9}/>
<div style='display: inline-block;'></div>
</div>",
this.Name, this.Left, this.Top, this.Width, this.Height, 
this.Label, this.MaxLengthString, this.TextModeString, this.HiddenString, (this.Required && !this.Hidden ? "required" : string.Empty),this.SkipPersistString);
        }

        private string RequiredString
        {
            get { return (base.Required ? "$('#{0}').focusout(function() { isRequired(this); });".Replace("{0}", this.Name) : string.Empty); }
        }

        private string HiddenString
        {
            get { return (base.Hidden ? "visibility: hidden;" : string.Empty); }
        }

        private string TextTransformString
        {
            get {
                return (((int)this.TextTransform > 0) ? "$('#{0}').keydown(function(event) { textTransform(this, {1}); }); $('#{0}').on('paste', function(event) { textTransform(this, {1}); });"
                    .Replace("{0}", this.Name)
                    .Replace("{1}", ((int)this.TextTransform).ToString()) : string.Empty);
            }
        }

        private string UniqueString
        {
            get { return (base.Unique ? "$('#{0}').focusout(function() { isUnique(this); });".Replace("{0}", this.Name) : string.Empty); }
        }

        private string MaxLengthString
        {  
            get { return (this.MaxLength > 0) ? string.Format("maxlength='{0}'", this.MaxLength) : string.Empty; }
        }
        private string SkipPersistString
        {
            get { return (base.SkipPersist ? "@Skip$" : string.Empty); }
        }

        private string TextModeString
        {  
            get {

                string returnval = string.Empty;

                switch (this.TextMode)
                {
                    case TextMode.Email:
                        returnval = "email";
                        break;
                    case TextMode.Password:
                        returnval = "password";
                        break;
                    case TextMode.Color:
                        returnval = "color";
                        break;
                    case TextMode.SingleLine:
                        returnval = "text";
                        break;
                }

                return returnval;
            }
        }
    }
}
