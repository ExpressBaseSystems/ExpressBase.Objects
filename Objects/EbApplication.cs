using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExpressBase.Objects.Objects
{
    [ProtoBuf.ProtoContract]
    public class EbApplication : EbObject
    {
       
    }

    [ProtoBuf.ProtoContract]
    public class EbApplicationModules : EbObject
    {

        [ProtoBuf.ProtoMember(1)]
        public string ApplicationName { get; set; }

    }
}
