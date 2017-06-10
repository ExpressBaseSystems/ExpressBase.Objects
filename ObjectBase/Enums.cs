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
        Table,
        SqlFunction,
        SqlValidator,
        JavascriptFunction,
        JavascriptValidator,
        Application,
        ApplicationModule,
        DataVisualization,
        FilterDialog
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
    [ProtoBuf.ProtoContract]
    public enum ObjectLifeCycleStatus
    {
        Development,
        Test,
        UAT,
        Live,
        Offline,
        Obsolete
    }
}
