using ExpressBase.Common;
using ExpressBase.Common.Data;
using ExpressBase.Objects.Objects.DVRelated;
using ExpressBase.Objects.ServiceStack_Artifacts;
using ServiceStack;
using ServiceStack.Redis;
using System.Collections.Generic;

namespace ExpressBase.Objects
{
    public interface IEbPowerSelect
    {
        bool RenderAsSimpleSelect { get; set; }
        string DataSourceId { get; set; }
        DVBaseColumn ValueMember { get; set; }
        DVBaseColumn DisplayMember { get; set; }
        DVColumnCollection DisplayMembers { get; set; }

        bool IsDataFromApi { get; set; }
        string Url { get; set; }
        ApiMethods Method { set; get; }
        List<ApiRequestHeader> Headers { get; set; }
        //List<ApiRequestParam> Parameters { get; set; }
                
        void InitFromDataBase_SS(JsonServiceClient ServiceClient);
        string GetSelectQuery(IDatabase DataDB, Service service, string Col, string Tbl = null, string _id = null, string masterTbl = null);
        string GetSelectQuery123(IDatabase DataDB, Service service, string table, string column, string parentTbl, string masterTbl);
        string GetDisplayMembersQuery(IDatabase DataDB, Service service, string vms);
    }

    public interface IEbDataReaderControl
    {
        List<Param> ParamsList { get; set; }

        void FetchParamsMeta(IServiceClient ServiceClient, IRedisClient Redis);
    }
}
