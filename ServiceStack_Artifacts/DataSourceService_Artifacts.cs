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
    [Route("/ds")]
    [Route("/ds/data/{RefId}")]
    [DataContract]
    public class DataSourceDataRequest : IReturn<DataSourceDataResponse>, IEbSSRequest
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

        [DataMember(Order = 6)]
        public string Token { get; set; }

        [DataMember(Order = 7)]
        public string rToken { get; set; }

        [DataMember(Order = 8)]
        public string TenantAccountId { get; set; }

        [DataMember(Order = 9)]
        public int UserId { get; set; }

        [DataMember(Order = 10)]
        public List<Dictionary<string, string>> Params { get; set; }

        [DataMember(Order = 11)]
        public List<Dictionary<string, string>> TFilters { get; set; }
    }

    [Route("/ds")]
    [Route("/ds/columns/{RefId}")]
    [DataContract]
    public class DataSourceColumnsRequest : IReturn<DataSourceColumnsResponse>, IEbSSRequest
    {
        [DataMember(Order = 1)]
        public string RefId { get; set; }

        [DataMember(Order = 2)]
        public string SearchText { get; set; }

        [DataMember(Order = 3)]
        public string OrderByDirection { get; set; }

        [DataMember(Order = 4)]
        public string SelectedColumnName { get; set; }

        [DataMember(Order = 5)]
        public string Token { get; set; }

        [DataMember(Order = 6)]
        public string rToken { get; set; }

        [DataMember(Order = 7)]
        public string TenantAccountId { get; set; }

        [DataMember(Order = 8)]
        public int UserId { get; set; }

        [DataMember(Order = 9)]
        public List<Dictionary<string, string>> Params { get; set; }

        [DataMember(Order = 10)]
        public string CrossDomain { get; set; }
    }

    [DataContract]
    [Csv(CsvBehavior.FirstEnumerable)]
    public class DataSourceDataResponse : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public int Draw { get; set; }

        [DataMember(Order = 2)]
        public int RecordsTotal { get; set; }

        [DataMember(Order = 3)]
        public int RecordsFiltered { get; set; }

        [DataMember(Order = 4)]
        public RowColletion Data { get; set; }

        [DataMember(Order = 5)]
        public string Token { get; set; }

        [DataMember(Order = 6)]
        public ResponseStatus ResponseStatus { get; set; }
    }

    [DataContract]
    [Csv(CsvBehavior.FirstEnumerable)]
    public class DataSourceColumnsResponse : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public List<ColumnColletion> Columns { get; set; }

        [DataMember(Order = 3)]
        public bool IsPaged { get; set; }

        [DataMember(Order = 4)]
        public ResponseStatus ResponseStatus { get; set; }

        public bool IsNull
        {
            get
            {
                return (this.Columns == null || this.Columns.Count == 0);
            }
        }
    }

    [DataContract]
    public class EbObjectWithRelatedDVRequest : EbServiceStackRequest, IReturn<EbObjectWithRelatedDVResponse>
    {
        [DataMember(Order = 1)]
        public string Refid { get; set; }

        [DataMember(Order = 2)]
        public string Ids { get; set; }

    }

    [DataContract]
    public class EbObjectWithRelatedDVResponse : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public EbDataVisualization Dsobj { get; set; }

        [DataMember(Order = 2)]
        public List<EbObjectWrapper> DvList { get; set; }

        [DataMember(Order = 3)]
        public List<EbObjectWrapper> DvTaggedList { get; set; }

        [DataMember(Order = 4)]
        public ResponseStatus ResponseStatus { get; set; }
    }
}
