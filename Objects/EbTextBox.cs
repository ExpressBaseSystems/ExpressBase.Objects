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
        MultiLine,
        Email,
        Password,
        Color
    }

    [ProtoBuf.ProtoContract]
    public class EbTextBox : EbControl
    {
        [ProtoBuf.ProtoMember(1)]
        public int MaxLength { get; set; }

        [ProtoBuf.ProtoMember(2)]
        public TextTransform TextTransform { get; set; }

        public EbTextBox() { }

        public override string GetHead()
        {
            return this.UniqueString + this.RequiredString + this.TextTransformString;
        }

        public override string GetHtml()
        {
            return string.Format(@"
<div style='position:absolute; left:{1}px; top:{2}px;'>
<div>{5}</div>
<input type='text' name='{0}' id='{0}' {6} style='width:{3}px; height:{4}px; display:inline-block;' />
<div style='display: inline-block;'></div>
</div>",
this.Name, this.Left, this.Top, this.Width, this.Height, this.Label, this.MaxLengthString);
        }

        private string RequiredString
        {
            get { return (base.Required ? "$('#{0}').focusout(function() { isRequired(this); });".Replace("{0}", this.Name) : string.Empty); }
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
    }
}
