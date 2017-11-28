using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace ExpressBase.Objects.Helpers
{

    [ProtoBuf.ProtoContract]
    public class EbParameters
    {
        [ProtoBuf.ProtoMember(1)]
        public string Name { get; set; }

        [ProtoBuf.ProtoMember(2)]
        public string Value { get; set; }

        [ProtoBuf.ProtoMember(3)]
        public int Type { get; set; }
    }
}
