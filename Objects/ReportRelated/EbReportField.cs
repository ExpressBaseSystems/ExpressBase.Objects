using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace ExpressBase.Objects
{
    [ProtoBuf.ProtoContract]
    [ProtoBuf.ProtoInclude(1, typeof(EbReportFieldText))]
    [ProtoBuf.ProtoInclude(1, typeof(EbReportFieldNumeric))]
    public class EbReportField : EbControl
    {
        #region Hidden Inherited Public Properties

        [Browsable(false)]
        public override int TabIndex { get; set; }

        #endregion

        [ProtoBuf.ProtoMember(1)]
        public override string Name { get; set; }

        public EbReportField() { }
    }
}

