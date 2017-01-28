using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace ExpressBase.Objects
{
    public class EbTableColumn
    {
        public int Id { get; set; }
        public string Name { get; set; } 
        public DbType Type { get; set; }
        public int TableId { get; set; }
    }

    public class EbTableColumnCollection : Dictionary<string, EbTableColumn>
    {

    }
}
