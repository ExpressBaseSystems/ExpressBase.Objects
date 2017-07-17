using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExpressBase.Objects
{
    [ProtoBuf.ProtoContract]
    public class EbDataVisualization : EbObject
    {
        [ProtoBuf.ProtoMember(1)]
        public string settingsJson { get; set; }

        [ProtoBuf.ProtoMember(2)]
        public int dsid { get; set; }

        [ProtoBuf.ProtoContract]
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

}
