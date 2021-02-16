using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using ExpressBase.Objects.Objects.DVRelated;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExpressBase.Objects.Objects
{
    public class PivotConfig
    {
        public string Reference { get; set; }

        public List<DVBaseColumn> DRColumns { get; set; }

        public List<DVBaseColumn> DSColumns { get; set; }

        public List<DVBaseColumn> Columns { get; set; }

        public List<DVBaseColumn> Rows { get; set; }

        public List<DVBaseColumn> Values { get; set; }

        public PivotConfig()
        {
            this.DRColumns = new List<DVBaseColumn>();
            this.DSColumns = new List<DVBaseColumn>();
            this.Columns = new List<DVBaseColumn>();
            this.Rows = new List<DVBaseColumn>();
            this.Values = new List<DVBaseColumn>();
        }
    }

}
