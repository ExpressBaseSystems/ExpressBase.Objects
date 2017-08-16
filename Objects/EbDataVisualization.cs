using ExpressBase.Data;
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

        public string Name { get; set; }

        public string Description { get; set; }

        public List<DTColumnDef> DTColumnDef { get; set; }

        public string RenderAs { get; set; }

        public string IsPaged { get; set; }
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

    public class DTColumnDef
    {
        public int data;

        public string title;

        public DTColumnDef(int data, string title)
        {
            this.data = data;
            this.title = title;
        }
    }
}
