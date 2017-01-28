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

        [ProtoBuf.ProtoMember(20)]
        [System.ComponentModel.Category("Behavior")]
        public virtual bool Unique { get; set; }

        [ProtoBuf.ProtoMember(21)]
        [System.ComponentModel.Category("Behavior")]
        public virtual bool ReadOnly { get; set; }

        [ProtoBuf.ProtoMember(22)]
        [System.ComponentModel.Category("Behavior")]
        public virtual bool Hidden { get; set; }

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
                return String.Format("#{0:X6}", this.BackColor.ToArgb() & 0x00FFFFFF);
#else
                return _backColorSerialized;
#endif
            }
            set
            {
#if NET462
                this.BackColor = System.Drawing.ColorTranslator.FromHtml(value);
#else
                _backColorSerialized = value;
#endif
            }
        }

        public EbControl() { }

        public virtual string GetHead() { return string.Empty; }

        public virtual string GetHtml() { return string.Empty; }

        public override void Init4Redis() { }
    }
}
