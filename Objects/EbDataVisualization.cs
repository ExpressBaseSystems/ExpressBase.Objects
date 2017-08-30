using ExpressBase.Data;
using ExpressBase.Objects.Objects.DVRelated;
using Newtonsoft.Json;
using ServiceStack.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExpressBase.Objects
{
    public class EbDataVisualization : EbObject
    {
        public string DataSourceRefId { get; set; }

        [JsonIgnore]
        public EbDataSource EbDataSource { get; set; }

        public string Description { get; set; }

        public DVColumnCollection Columns { get; set; }

        //public List<DTColumnDef> columns { get; set; }

        //public List<DTColumnExtPpty> columnsext { get; set; }

        public string RenderAs { get; set; }

        public string IsPaged { get; set; }

        public GOptions options { get; set; }
        public override void AfterRedisGet(RedisClient Redis)
        {
            try
            {
                this.EbDataSource = Redis.Get<EbDataSource>(this.DataSourceRefId);
                this.EbDataSource.AfterRedisGet(Redis);
            }
            catch(Exception e)
            {

            }
        }

        public enum Operations
        {
            Create,
            Edit,
            PageSummary,
            TotalSummary,
            Filtering,
            Zooming,
            Graph,
            PDFExport,
            ExcelExport,
            CSVExport,
            CopyToClipboard,
            Print
        }
    }


    public class GOptions
    {
        public List<axis> Xaxis { get; set; }

        public List<axis> Yaxis { get; set; }

        public string type { get; set; }
    }
    public class axis
    {
        public string index { get; set; }

        public string name { get; set; }
    }
}
