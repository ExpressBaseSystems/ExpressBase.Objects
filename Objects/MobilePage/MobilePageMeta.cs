using ExpressBase.Common.Structures;
using System.Collections.Generic;

namespace ExpressBase.Objects.Objects.MobilePage
{
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
