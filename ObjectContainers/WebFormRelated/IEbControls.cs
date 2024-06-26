﻿using ExpressBase.Common;
using ExpressBase.Common.Data;
using ExpressBase.Common.Objects;
using ExpressBase.Objects.Objects.DVRelated;
using ExpressBase.Objects.ServiceStack_Artifacts;
using ServiceStack;
using ServiceStack.Redis;
using System.Collections.Generic;
using System.Data.Common;

namespace ExpressBase.Objects
{
    public interface IEbPowerSelect
    {
        bool RenderAsSimpleSelect { get; set; }
        string DataSourceId { get; set; }
        DVBaseColumn ValueMember { get; set; }
        DVBaseColumn DisplayMember { get; set; }
        DVColumnCollection DisplayMembers { get; set; }
        bool IsInsertable { get; set; }
        string FormRefId { get; set; }
        bool MultiSelect { get; set; }
        string TableName { get; set; }
        string EbSid_CtxId { get; set; }
        List<EbSimpleSelectOption> Options { get; set; }

        bool IsDataFromApi { get; set; }
        string Url { get; set; }
        ApiMethods Method { set; get; }
        List<ApiRequestHeader> Headers { get; set; }
        List<EbCtrlApiParamAbstract> DataApiParams { get; set; }

        void InitFromDataBase_SS(JsonServiceClient ServiceClient);
        string GetSelectQuery(IDatabase DataDB, Service service, string Col, string Tbl = null, string _id = null, string masterTbl = null);
        //string GetSelectQuery123(IDatabase DataDB, Service service, string table, string column, string parentTbl, string masterTbl);
        string GetDisplayMembersQuery(IDatabase DataDB, Service service, string vms, List<DbParameter> param);
        void UpdateParamsMeta(Service Service, IRedisClient Redis);
        (string, EbDataReader) GetSqlAndDr(Service service);
    }

    public interface IEbDataReaderControl
    {
        List<Param> ParamsList { get; set; }

        void FetchParamsMeta(IServiceClient ServiceClient, IRedisClient Redis, EbControl[] Allctrls, Service service);
    }

    public interface IEbExtraQryCtrl
    {
        string TableName { get; set; }
        string GetSelectQuery(IDatabase DataDB, string MasterTbl, string form_ver_id, string form_ref_id);
    }
}
