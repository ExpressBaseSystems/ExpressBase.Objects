using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace ExpressBase.Objects
{
    [ProtoBuf.ProtoContract]
    public class EbReportField : EbControl
    {
        #region Hidden Inherited Public Properties

        [Browsable(false)]
        public override int TabIndex { get; set; }

        #endregion

        [ProtoBuf.ProtoMember(1)]
        [System.ComponentModel.Category("Behavior")]
        public TextTransform TextTransform { get; set; }



        public EbReportField() { }
    }
}

