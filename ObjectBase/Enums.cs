using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExpressBase.Objects
{
    [ProtoBuf.ProtoContract]
    public enum EbObjectType
    {
        Form,
        View,
        DataSource,
        Report,
        Table
    }

    [ProtoBuf.ProtoContract]
    public enum EbDataGridViewColumnType
    {
        Boolean,
        DateTime,
        Image,
        Numeric,
        Text,
        Null,
        Chart
    }
}
