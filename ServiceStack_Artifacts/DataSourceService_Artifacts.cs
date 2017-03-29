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
    [Route("/ds/data/{Id}")]
    public class DataSourceDataRequest : IReturn<DataSourceDataResponse>, IEbSSRequest
    {
        public int Id { get; set; }

        public int Start { get; set; }

        public int Length { get; set; }

        public int Draw { get; set; }

        public string SearchText { get; set; }

        public string OrderByDirection { get; set; }

        public string OrderColumnName { get; set; }

        public string SearchColumnName { get; set; }

        public string Token { get; set; }

        public string TenantAccountId { get; set; }
    }

    [Route("/ds")]
    [Route("/ds/columns/{Id}")]
    public class DataSourceColumnsRequest : IReturn<DataSourceColumnsResponse>, IEbSSRequest
    {
        public int Id { get; set; }

        public string SearchText { get; set; }

        public string OrderByDirection { get; set; }

        public string SelectedColumnName { get; set; }

        public string Token { get; set; }

        public string TenantAccountId { get; set; }
    }

    [DataContract]
    [Csv(CsvBehavior.FirstEnumerable)]
    public class DataSourceDataResponse
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
    [Csv(CsvBehavior.FirstEnumerable)]
    public class DataSourceColumnsResponse
    {
        [DataMember(Order = 1)]
        public ColumnColletion Columns { get; set; }
    }
}
