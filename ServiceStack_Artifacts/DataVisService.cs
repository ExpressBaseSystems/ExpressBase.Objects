﻿using ExpressBase.Common;
using ExpressBase.Common.Data;
using ExpressBase.Common.EbServiceStack.ReqNRes;
using ExpressBase.Common.LocationNSolution;
using ExpressBase.Data;
using ExpressBase.Security;
using Newtonsoft.Json;
using ServiceStack;
using ServiceStack.Text;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace ExpressBase.Objects.ServiceStack_Artifacts
{
    [Route("/dv")]
    [Route("/dv/data/{RefId}")]
    [DataContract]
    public class DataVisDataRequest : EbServiceStackAuthRequest, IReturn<DataSourceDataResponse>
    {
        [DataMember(Order = 0)]
        public EbDataVisualization EbDataVisualization { get; set; }
        //public string RefId { get; set; }

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
    public class DataVisColumnsRequest : EbServiceStackAuthRequest, IReturn<DataSourceColumnsResponse>
    {
        [DataMember(Order = 1)]
        public EbDataVisualization EbDataVisualization { get; set; }

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

    //[Route("/table")]
    //[Route("/table/data/{RefId}")]
    [DataContract]
    public class TableDataRequest : EbServiceStackAuthRequest, IReturn<DataSourceDataResponse>
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
        public List<OrderBy> OrderBy { get; set; }

        [DataMember(Order = 6)]
        public string Token { get; set; }

        [DataMember(Order = 7)]
        public string rToken { get; set; }

        [DataMember(Order = 10)]
        public List<Param> Params { get; set; }

        [DataMember(Order = 11)]
        public List<TFilters> TFilters { get; set; }

        [DataMember(Order = 10)]
        public bool Ispaging { get; set; }

        [DataMember(Order = 11)]
        public string DataVizObjString { get; set; }

        [DataMember(Order = 12)]
        public EbDataVisualization EbDataVisualization { get; set; }

        [DataMember(Order = 13)]
        public User UserInfo { get; set; }

        [DataMember(Order = 14)]
        public List<int> RGIndex { get; set; }

        [DataMember(Order = 15)]
        public string CurrentRowGroup { get; set; }

        [DataMember(Order = 16)]
        public string dvRefId { get; set; }

        [DataMember(Order = 17)]
        public bool IsExcel { get; set; }

        [DataMember(Order = 18)]
        public Eb_Solution eb_Solution { get; set; }

        [DataMember(Order = 19)]
        public bool ReplaceEbColumns { get; set; }

        [DataMember(Order = 19)]
        public string TableId { get; set; }

        [DataMember(Order = 19)]
        public bool Modifydv { get; set; }

        [DataMember(Order = 19)]
        public int LocId { get; set; }

        [DataMember(Order = 20)]
        public string SubscriptionId { get; set; }

        [DataMember(Order = 21)]
        public bool showCheckboxColumn { get; set; }

        [DataMember(Order = 22)]
        public string Source { get; set; }

        [DataMember(Order = 23)]
        public int counter { get; set; }

    }

    [Route("/table")]
    [Route("/table/columns/{RefId}")]
    [DataContract]
    public class TableColumnsRequest : IReturn<DataSourceColumnsResponse>, IEbSSRequest
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
        public string SolnId { get; set; }

        [DataMember(Order = 8)]
        public int UserId { get; set; }

        [DataMember(Order = 9)]
        public List<Param> Params { get; set; }

        [DataMember(Order = 10)]
        public string CrossDomain { get; set; }
    }

    [DataContract]
    [Csv(CsvBehavior.FirstEnumerable)]
    public class TableDataResponse : IEbSSResponse
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

        [DataMember(Order = 7)]
        public System.Data.DataSet DataSet { get; set; }

        [DataMember(Order = 8)]
        public bool Ispaged { get; set; }
    }

    [DataContract]
    [Csv(CsvBehavior.FirstEnumerable)]
    public class TableColumnsResponse : IEbSSResponse
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
    public class OrderBy
    {

        [DataMember(Order = 1)]
        public string Column { get; set; }

        [DataMember(Order = 2)]
        public int Direction { get; set; }
    }


    [DataContract]
    public class InlineTableDataRequest : IReturn<DataSourceDataResponse>, IEbSSRequest
    {
        [DataMember(Order = 0)]
        public string RefId { get; set; }

        [DataMember(Order = 1)]
        public int Start { get; set; }

        [DataMember(Order = 2)]
        public int Length { get; set; }

        [DataMember(Order = 6)]
        public string Token { get; set; }

        [DataMember(Order = 7)]
        public string rToken { get; set; }

        [DataMember(Order = 8)]
        public string SolnId { get; set; }

        [DataMember(Order = 9)]
        public int UserId { get; set; }

        [DataMember(Order = 10)]
        public List<Param> Params { get; set; }

        [DataMember(Order = 11)]
        public List<TFilters> TFilters { get; set; }

        [DataMember(Order = 10)]
        public bool Ispaging { get; set; }

        [DataMember(Order = 11)]
        public string DataVizObjString { get; set; }

        [DataMember(Order = 12)]
        public EbDataVisualization EbDataVisualization { get; set; }

        [DataMember(Order = 13)]
        public User UserInfo { get; set; }

        [DataMember(Order = 14)]
        public bool IsExcel { get; set; }

        [DataMember(Order = 15)]
        public Eb_Solution eb_solution { get; set; }

        [DataMember(Order = 16)]
        public bool ReplaceEbColumns { get; set; }

        [DataMember(Order = 16)]
        public string TableId { get; set; }
    }

    [DataContract]
    public class ExportToExcelSyncRequest : EbServiceStackAuthRequest, IReturn<ExportToExcelSyncResponse>
    {
        [DataMember(Order = 1)]
        public List<Param> Params { get; set; }

        [DataMember(Order = 2)]
        public string dvRefId { get; set; }

        [DataMember(Order = 3)]
        public List<TFilters> TFilters { get; set; }

        [DataMember(Order = 4)]
        public string CurrentRowGroup { get; set; }

        [DataMember(Order = 5)]
        public List<OrderBy> OrderBy { get; set; }
    }

    [DataContract]
    public class ExportToExcelSyncResponse : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public int Id { get; set; }

        [DataMember(Order = 2)]
        public string Msg { get; set; }

        [DataMember(Order = 3)]
        public ResponseStatus ResponseStatus { get; set; }
    }

    [DataContract]
    public class ExportToExcelMqRequest : EbServiceStackAuthRequest, IReturn<ExportToExcelServiceResponse>
    {
        [DataMember(Order = 11)]
        public string RefId { get; set; }

        [DataMember(Order = 10)]
        public List<Param> Params { get; set; }

        [DataMember(Order = 2)]
        public int Length { get; set; }

        [DataMember(Order = 11)]
        public string DataVizObjString { get; set; }

        [DataMember(Order = 12)]
        public EbDataVisualization EbDataVisualization { get; set; }

        [DataMember(Order = 13)]
        public User UserInfo { get; set; }

        [DataMember(Order = 16)]
        public string dvRefId { get; set; }

        [DataMember(Order = 16)]
        public bool Ispaging { get; set; }

        [DataMember(Order = 16)]
        public bool IsExcel { get; set; }

        [DataMember(Order = 16)]
        public Eb_Solution eb_Solution { get; set; }

        [DataMember(Order = 16)]
        public List<TFilters> TFilters { get; set; }

        [DataMember(Order = 17)]
        public string SubscriptionId { get; set; }
    }

    [DataContract]
    public class ExportToExcelServiceRequest : EbServiceStackAuthRequest, IReturn<ExportToExcelServiceResponse>
    {
        [DataMember(Order = 11)]
        public string RefId { get; set; }

        [DataMember(Order = 10)]
        public List<Param> Params { get; set; }

        [DataMember(Order = 2)]
        public int Length { get; set; }

        [DataMember(Order = 11)]
        public string DataVizObjString { get; set; }

        [DataMember(Order = 12)]
        public EbDataVisualization EbDataVisualization { get; set; }

        [DataMember(Order = 13)]
        public User UserInfo { get; set; }

        [DataMember(Order = 16)]
        public string dvRefId { get; set; }

        [DataMember(Order = 16)]
        public bool Ispaging { get; set; }

        [DataMember(Order = 16)]
        public bool IsExcel { get; set; }


        [DataMember(Order = 16)]
        public string BToken { get; set; }

        [DataMember(Order = 16)]
        public string RToken { get; set; }

        [DataMember(Order = 16)]
        public Eb_Solution eb_solution { get; set; }

        [DataMember(Order = 16)]
        public List<TFilters> TFilters { get; set; }

        [DataMember(Order = 17)]
        public string SubscriptionId { get; set; }
    }

    [DataContract]
    [Csv(CsvBehavior.FirstEnumerable)]
    public class ExportToExcelServiceResponse : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public RowColletion Data { get; set; }

        [DataMember(Order = 2)]
        public ResponseStatus ResponseStatus { get; set; }

        [DataMember(Order = 3)]
        public DataSet DataSet { get; set; }

        [DataMember(Order = 4)]
        public RowColletion FormattedData { get; set; }

        [DataMember(Order = 5)]
        public Dictionary<int, List<object>> Summary { get; set; }

        [DataMember(Order = 6)]
        public string tableString { get; set; }

    }

    public class FormulaPart
    {
        public string TableName { get; set; }
        public string FieldName { get; set; }
    }

    public class UpdateTreeColumnRequest : EbServiceStackAuthRequest, IReturn<UpdateTreeColumnResponse>
    {
        [DataMember(Order = 1)]
        public string sql { get; set; }
    }

    public class UpdateTreeColumnResponse : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public ResponseStatus ResponseStatus { get; set; }
    }

    [DataContract]
    public class ParticularApprovalColumnRequest : EbServiceStackAuthRequest, IReturn<ParticularApprovalColumnResponse>
    {

        [DataMember(Order = 3)]
        public string RefId { get; set; }

        [DataMember(Order = 4)]
        public int RowId { get; set; }

        [DataMember(Order = 5)]
        public int CurrentLoc { get; set; }

        [DataMember(Order = 6)]
        public User UserObj { get; set; }
    }

    [DataContract]
    public class ParticularApprovalColumnResponse : IEbSSResponse
    {
        [DataMember(Order = 1)]
        public ResponseStatus ResponseStatus { get; set; }

        [DataMember(Order = 1)]
        public string Messaage { get; set; }

        [DataMember(Order = 7)]
        public string _data { get; set; }
    }
}
