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
        public virtual string Label { get; set; }

        [ProtoBuf.ProtoMember(11)]
        [Description("Labels")]
        public virtual string HelpText { get; set; }

        [ProtoBuf.ProtoMember(12)]
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
        [Browsable(false)]
        public virtual int Height { get; set; }

        [ProtoBuf.ProtoMember(18)]
        [Browsable(false)]
        public virtual int Width { get; set; }

        [ProtoBuf.ProtoMember(19)]
        public virtual bool Required { get; set; }

        [ProtoBuf.ProtoMember(20)]
        public virtual bool Unique { get; set; }

        [ProtoBuf.ProtoMember(21)]
        public virtual bool ReadOnly { get; set; }

        [ProtoBuf.ProtoMember(22)]
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
        public virtual int TabIndex { get; set; }

        public EbControl() { }

        public virtual string GetHead() { return string.Empty; }

        public virtual string GetHtml() { return string.Empty; }

        public override void Init4Redis() { }
    }
}
