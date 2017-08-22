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

    //public class DTColumnDef
    //{
    //    public int data;

    //    public string title;

    //    public string type;

    //    public bool visible;

    //    public string name;

    //    public string width;

    //    public int pos;

    //    public DTColumnDef(int data, string title, string type,bool vis, string name, string width, int pos)
    //    {
    //        this.data = data;
    //        this.title = title;
    //        this.type = type;
    //        this.visible = vis;
    //        this.name = name;
    //        this.width = width;
    //        this.pos = pos;
    //    }
    //}

    //public class DTColumnExtPpty
    //{
    //    public string name;

    //    public bool AggInfo;

    //    public int DecimalPlace;

    //    public string RenderAs;

    //    public int pos;

    //    public DTColumnExtPpty(string name, bool agginfo, int deci, string renderas, int pos)
    //    {
    //        this.name = name;
    //        this.AggInfo = agginfo;
    //        this.DecimalPlace = deci;
    //        this.RenderAs = renderas;
    //        this.pos = pos;
    //    }

    //    public DTColumnExtPpty(int pos)
    //    {
    //        this.pos = pos;
    //    }

    //}
}
