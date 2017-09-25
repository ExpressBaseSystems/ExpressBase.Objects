using System;
using System.Collections.Generic;
using System.Text;

namespace ExpressBase.Objects.Objects
{
   public class EbEmailBuilder : EbObject
    {
        [ProtoBuf.ProtoMember(1)]
        public string html { get; set; }
    }
}
