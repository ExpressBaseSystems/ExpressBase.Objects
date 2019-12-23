using ExpressBase.Common.Structures;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExpressBase.Objects.Objects.MobilePage
{
    public enum MobVisRenderType
    {
        Link = 0,
        Info = 1,
        Referencing = 3
    }

    public class MobileFormData
    {
        public string MasterTable { set; get; }

        public int LocationId { set; get; }

        public List<MobileTable> Tables { set; get; }
    }

    public class MobileTable : List<MobileTableRow>
    {
        public string TableName { set; get; }
    }

    public class MobileTableRow
    {
        public string RowId { set; get; }

        public bool IsUpdate { set; get; }

        public List<MobileTableColumn> Columns { set; get; }
    }

    public class MobileTableColumn
    {
        public string Name { get; set; }

        public object Value { get; set; }

        public EbDbTypes Type { set; get; }

        public EbMobileControl Control { set; get; }
    }
}
