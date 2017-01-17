using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace ExpressBase.Objects
{
    [ProtoBuf.ProtoContract]
    [ProtoBuf.ProtoInclude(2000, typeof(EbButton))]
    [ProtoBuf.ProtoInclude(2001, typeof(EbTableLayout))]
    [ProtoBuf.ProtoInclude(2002, typeof(EbChart))]
    [ProtoBuf.ProtoInclude(2003, typeof(EbDataGridView))]
    [ProtoBuf.ProtoInclude(2004, typeof(EbDataGridViewColumn))]
    public class EbControl : EbObject
    {
        [ProtoBuf.ProtoMember(4)]
        [Browsable(false)]
        public List<EbControl> Controls { get; set; }

        [ProtoBuf.ProtoMember(5)]
        [Description("Labels")]
        public virtual string Label { get; set; }

        [ProtoBuf.ProtoMember(6)]
        [Description("Labels")]
        public virtual string HelpText { get; set; }

        [ProtoBuf.ProtoMember(7)]
        [Description("Labels")]
        public virtual string ToolTipText { get; set; }

        [ProtoBuf.ProtoMember(8)]
        [Browsable(false)]
        public virtual int CellPositionRow { get; set; }

        [ProtoBuf.ProtoMember(9)]
        [Browsable(false)]
        public virtual int CellPositionColumn { get; set; }

        public EbControl() { }

        public virtual string GetHtml() { return string.Empty; }
    }
}
