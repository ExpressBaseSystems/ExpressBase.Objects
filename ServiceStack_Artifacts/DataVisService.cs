using ExpressBase.Common;
using ExpressBase.Data;
using ServiceStack;
using ServiceStack.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace ExpressBase.Objects.ServiceStack_Artifacts
{
    [Route("/dv")]
    [Route("/dv/data/{RefId}")]
    [DataContract]
    public class DataVisDataRequest : EbServiceStackRequest, IReturn<DataSourceDataResponse>
    {
        [DataMember(Order = 0)]
        public string RefId { get; set; }

        [DataMember(Order = 1)]
        public int Start { get; set; }

        [DataMember(Order = 2)]
        public int Length { get; set; }

        [DataMember(Order = 3)]
        public int Draw { get; set; }

        [DataMember(Order = 4)]
        public int OrderByDir { get; set; }

        [DataMember(Order = 5)]
        public string OrderByCol { get; set; }

        [DataMember(Order = 10)]
        public List<Dictionary<string, string>> Params { get; set; }

        [DataMember(Order = 11)]
        public List<Dictionary<string, string>> TFilters { get; set; }
    }

    [Route("/dv")]
    [Route("/dv/columns/{RefId}")]
    [DataContract]
    public class DataVisColumnsRequest : EbServiceStackRequest, IReturn<DataSourceColumnsResponse>
    {
        [DataMember(Order = 1)]
        public string RefId { get; set; }

        [DataMember(Order = 2)]
        public string SearchText { get; set; }

        [DataMember(Order = 3)]
        public string OrderByDirection { get; set; }

        [DataMember(Order = 4)]
        public string SelectedColumnName { get; set; }

        [DataMember(Order = 9)]
        public List<Dictionary<string, string>> Params { get; set; }
    }

    [DataContract]
    public class DataVisDataResponse : EbServiceStackResponse
    {
        [DataMember(Order = 1)]
        public int Draw { get; set; }

        [DataMember(Order = 2)]
        public int RecordsTotal { get; set; }

        [DataMember(Order = 3)]
        public int RecordsFiltered { get; set; }

        [DataMember(Order = 4)]
        public RowColletion Data { get; set; }
    }

    [DataContract]
    public class DataVisColumnsResponse : EbServiceStackResponse
    {
        [DataMember(Order = 1)]
        public List<ColumnColletion> Columns { get; set; }

        [DataMember(Order = 3)]
        public bool IsPaged { get; set; }

        public bool IsNull
        {
            get
            {
                return (this.Columns == null || this.Columns.Count == 0);
            }
        }
    }
}
