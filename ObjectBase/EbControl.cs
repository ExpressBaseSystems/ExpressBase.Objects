using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace ExpressBase.Objects
{
    [ProtoBuf.ProtoContract]
    [ProtoBuf.ProtoInclude(2000, typeof(EbControlContainer))]
    [ProtoBuf.ProtoInclude(2001, typeof(EbButton))]
    [ProtoBuf.ProtoInclude(2002, typeof(EbChart))]
    [ProtoBuf.ProtoInclude(2003, typeof(EbDataGridViewColumn))]
    [ProtoBuf.ProtoInclude(2004, typeof(EbTextBox))]
    [ProtoBuf.ProtoInclude(2005, typeof(EbNumeric))]
    [ProtoBuf.ProtoInclude(2006, typeof(EbDate))]
    [ProtoBuf.ProtoInclude(2007, typeof(EbComboBox))]
    [ProtoBuf.ProtoInclude(2008, typeof(EbRadioGroup))]
#if NET462
    [System.Serializable]
#endif
    public class EbControl : EbObject
    {
        [ProtoBuf.ProtoMember(10)]
        [Description("Labels")]
        [System.ComponentModel.Category("Behavior")]
        public virtual string Label { get; set; }

        [ProtoBuf.ProtoMember(11)]
        [System.ComponentModel.Category("Behavior")]
        [Description("Labels")]
        public virtual string HelpText { get; set; }

        [ProtoBuf.ProtoMember(12)]
        [System.ComponentModel.Category("Behavior")]
        [Description("Labels")]
        public virtual string ToolTipText { get; set; }

        [ProtoBuf.ProtoMember(13)]
        [Browsable(false)]
        public virtual int CellPositionRow { get; set; }

        [ProtoBuf.ProtoMember(14)]
        [Browsable(false)]
        public virtual int CellPositionColumn { get; set; }

        [ProtoBuf.ProtoMember(15)]
        [Browsable(false)]
        public virtual int Left { get; set; }

        [ProtoBuf.ProtoMember(16)]
        [Browsable(false)]
        public virtual int Top { get; set; }

        [ProtoBuf.ProtoMember(17)]
        [System.ComponentModel.Category("Layout")]
        public virtual int Height { get; set; }

        [ProtoBuf.ProtoMember(18)]
        [System.ComponentModel.Category("Layout")]
        public virtual int Width { get; set; }

        [ProtoBuf.ProtoMember(19)]
        [System.ComponentModel.Category("Behavior")]
        public virtual bool Required { get; set; }

        protected string RequiredString
        {
            get { return (this.Required ? "$('#{0}').focusout(function() { isRequired(this); });".Replace("{0}", this.Name) : string.Empty); }
        }

        [ProtoBuf.ProtoMember(20)]
        [System.ComponentModel.Category("Behavior")]
        public virtual bool Unique { get; set; }

        protected string UniqueString
        {
            get { return (this.Unique ? "$('#{0}').focusout(function() { isUnique(this); });".Replace("{0}", this.Name) : string.Empty); }
        }


        [ProtoBuf.ProtoMember(21)]
        [System.ComponentModel.Category("Behavior")]
        public virtual bool ReadOnly { get; set; }

        protected string ReadOnlyString
        {
            get { return (this.ReadOnly ? "background-color: #f0f0f0; border: solid 1px #bbb;' readonly" : "'"); }
        }

        [ProtoBuf.ProtoMember(22)]
        [System.ComponentModel.Category("Behavior")]
        public virtual bool Hidden { get; set; }

        protected string HiddenString
        {
            get { return (this.Hidden ? "visibility: hidden;" : string.Empty); }
        }

        [ProtoBuf.ProtoMember(23)]
        public virtual bool SkipPersist { get; set; }

        [ProtoBuf.ProtoMember(24)]
#if NET462
        [Editor(typeof(System.ComponentModel.Design.MultilineStringEditor), typeof(System.Drawing.Design.UITypeEditor))]
#endif
        public virtual string RequiredExpression { get; set; }

        [ProtoBuf.ProtoMember(25)]
        public virtual string UniqueExpression { get; set; }

        [ProtoBuf.ProtoMember(26)]
        public virtual string ReadOnlyExpression { get; set; }

        [ProtoBuf.ProtoMember(27)]
        public virtual string VisibleExpression { get; set; }

        [ProtoBuf.ProtoMember(28)]
        [System.ComponentModel.Category("Accessibility")]
        public virtual int TabIndex { get; set; }

#if NET462
        [System.ComponentModel.Category("Appearance")]
        public System.Drawing.Color BackColor { get; set; }
#endif

        private string _backColorSerialized = null;
        [ProtoBuf.ProtoMember(29)]
        [Browsable(false)]
        public string BackColorSerialized
        {
            get
            {
#if NET462      
                _backColorSerialized = this.HexConverter(this.BackColor);
#endif
                return _backColorSerialized;
            }
            set
            {
#if NET462
                this.BackColor = System.Drawing.ColorTranslator.FromHtml(value);
#endif
                _backColorSerialized = value;
            }
        }

        //
#if NET462
        [System.ComponentModel.Category("Appearance")]
        public System.Drawing.Color ForeColor { get; set; }
#endif

        private string _foreColorSerialized = null;
        [ProtoBuf.ProtoMember(30)]
        [Browsable(false)]
        public string ForeColorSerialized
        {
            get
            {
#if NET462
                _foreColorSerialized = this.HexConverter(this.ForeColor);
#endif
                return _foreColorSerialized;
            }
            set
            {
#if NET462
                this.ForeColor = System.Drawing.ColorTranslator.FromHtml(value);
#endif
                _foreColorSerialized = value;
            }
        }
        //


#if NET462
        [System.ComponentModel.Category("Appearance")]
        public System.Drawing.Color LabelBackColor { get; set; }
#endif

        private string _labelBackColorSerialized = null;
        [ProtoBuf.ProtoMember(31)]
        [Browsable(false)]
        public string LabelBackColorSerialized
        {
            get
            {
#if NET462
                _labelBackColorSerialized = this.HexConverter(this.LabelBackColor);
#endif
                return _labelBackColorSerialized;
            }
            set
            {
#if NET462
                this.LabelBackColor = System.Drawing.ColorTranslator.FromHtml(value);
#endif
                _labelBackColorSerialized = value;
            }
        }

#if NET462
        [System.ComponentModel.Category("Appearance")]
        public System.Drawing.Color LabelForeColor { get; set; }
#endif

        private string _labelforeColorSerialized = null;
        [ProtoBuf.ProtoMember(32)]
        [Browsable(false)]
        public string LabelForeColorSerialized
        {
            get
            {
#if NET462
                _labelforeColorSerialized = this.HexConverter(this.LabelForeColor);
#endif
                return _labelforeColorSerialized;
            }
            set
            {
#if NET462
                this.LabelForeColor = System.Drawing.ColorTranslator.FromHtml(value);
#endif
                _labelforeColorSerialized = value;
            }
        }

#if NET462
        [System.ComponentModel.Category("Layout")]
        public System.Drawing.Font Font { get; set; }
#endif

        private ProtoFont _fontSerialized = null;
        [ProtoBuf.ProtoMember(33)]
        [Browsable(false)]
        public ProtoFont FontSerialized
        {
            get
            {
#if NET462
                if (this.Font != null)
                    _fontSerialized = new ProtoFont {
                        FontFamily = this.Font.FontFamily.Name,
                        SizeInPoints = this.Font.SizeInPoints,
                        Style = this.Font.Style.ToString()
                    };
#endif
                return _fontSerialized;
            }
            set
            {
#if NET462
                if (_fontSerialized != null)
                    this.Font = new System.Drawing.Font(_fontSerialized.FontFamily, _fontSerialized.SizeInPoints, _fontSerialized.FontStyle);
#endif
                _fontSerialized = value;
            }
        }

        public EbControl() { }

        public virtual string GetHead() { return string.Empty; }

        public virtual string GetHtml() { return string.Empty; }

        public override void Init4Redis() { }

        public virtual void SetData(object value) { }

        public virtual object GetData() { return null; }

#if NET462
        private string HexConverter(System.Drawing.Color c)
        {
            return "#" + c.R.ToString("X2") + c.G.ToString("X2") + c.B.ToString("X2");
        }
#endif
    }

    [ProtoBuf.ProtoContract()]
    public class ProtoFont
    {
        [ProtoBuf.ProtoMember(1)]
        public string FontFamily;

        [ProtoBuf.ProtoMember(2)]
        public float SizeInPoints;

        [ProtoBuf.ProtoMember(3)]
        public string Style;

#if NET462
        public System.Drawing.FontStyle FontStyle
        {
            get
            {
                if (Style == System.Drawing.FontStyle.Bold.ToString())
                    return System.Drawing.FontStyle.Bold;
                if (Style == System.Drawing.FontStyle.Italic.ToString())
                    return System.Drawing.FontStyle.Italic;
                if (Style == System.Drawing.FontStyle.Underline.ToString())
                    return System.Drawing.FontStyle.Underline;
                else
                    return System.Drawing.FontStyle.Regular;
            }
        }
#endif
    }
}
