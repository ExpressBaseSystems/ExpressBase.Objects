using ExpressBase.Common.Data;
using ExpressBase.Common.Extensions;
using ExpressBase.Common.Objects;
using ExpressBase.Common.ServiceClients;
using ExpressBase.Common.Structures;
using ExpressBase.Objects;
using ExpressBase.Objects.Helpers;
using ExpressBase.Objects.ServiceStack_Artifacts;
using ServiceStack;
using ServiceStack.RabbitMq;
using ServiceStack.Redis;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace ExpressBase.Common.Helpers
{
    public class EbApiHelper
    {
        public static T GetEbObject<T>(string refId, IRedisClient Redis, IDatabase ObjectsDB)
        {
            T ebObject = Redis.Get<T>(refId);

            if (ebObject == null)
            {
                List<EbObjectWrapper> wrap = EbObjectsHelper.GetParticularVersion(ObjectsDB, refId);
                ebObject = EbSerializers.Json_Deserialize<T>(wrap[0].Json);
            }
            return ebObject;
        }

        public static T GetEbObject<T>(int ObjId, IRedisClient Redis, IDatabase ObjectsDB)
        {
            List<EbObjectWrapper> wrap = EbObjectsHelper.GetLiveVersion(ObjectsDB, ObjId);
            T ebObject = EbSerializers.Json_Deserialize<T>(wrap[0].Json);
            return ebObject;
        }

        public static EbApi GetApiByName(string name, string version, IDatabase ObjectsDB)
        {
            EbApi api_o = null;
            string sql = ObjectsDB.EB_API_BY_NAME;

            DbParameter[] parameter =
            {
                 ObjectsDB.GetNewParameter("objname",EbDbTypes.String,name),
                 ObjectsDB.GetNewParameter("version",EbDbTypes.String,version)
            };
            EbDataTable dt = ObjectsDB.DoQuery(sql, parameter);
            if (dt.Rows.Count > 0)
            {
                EbDataRow dr = dt.Rows[0];
                EbObjectWrapper _ebObject = (new EbObjectWrapper
                {
                    Json = dr[0].ToString(),
                    VersionNumber = dr[1].ToString(),
                    EbObjectType = (dr[4] != DBNull.Value) ? Convert.ToInt32(dr[4]) : 0,
                    Status = Enum.GetName(typeof(ObjectLifeCycleStatus), Convert.ToInt32(dr[2])),
                    Tags = dr[3].ToString(),
                    RefId = null,
                });
                api_o = EbSerializers.Json_Deserialize<EbApi>(_ebObject.Json);
            }

            return api_o;
        }

        public static List<Param> GetEmailParams(EbEmailTemplate enode, IRedisClient Redis, IDatabase ObjectsDB)
        {
            List<Param> p = new List<Param>();

            if (!string.IsNullOrEmpty(enode.AttachmentReportRefID))
            {
                EbReport o = GetEbObject<EbReport>(enode.AttachmentReportRefID, Redis, ObjectsDB);

                if (!string.IsNullOrEmpty(o.DataSourceRefId))
                {
                    EbDataSourceMain ob = GetEbObject<EbDataSourceMain>(o.DataSourceRefId, Redis, ObjectsDB);

                    p = p.Merge(ob.GetParams(Redis as RedisClient));
                }
            }
            if (!string.IsNullOrEmpty(enode.DataSourceRefId))
            {
                EbDataSourceMain ob = GetEbObject<EbDataSourceMain>(enode.DataSourceRefId, Redis, ObjectsDB);

                p = p.Merge(ob.GetParams(Redis as RedisClient));
            }
            return p;
        }

        public static List<Param> GetReqJsonParameters(ListOrdered Components, IRedisClient Redis, IDatabase ObjectsDB)
        {
            List<Param> parameters = new List<Param>();

            foreach (ApiResources resource in Components)
            {
                if (resource is EbSqlReader || resource is EbSqlWriter || resource is EbSqlFunc)
                {
                    EbDataSourceMain dataSource = GetEbObject<EbDataSourceMain>(resource.Reference, Redis, ObjectsDB);

                    if (dataSource.InputParams == null || dataSource.InputParams.Count <= 0)
                        parameters.Merge(dataSource.GetParams(Redis as RedisClient));
                    else
                        parameters.Merge(dataSource.InputParams);
                }
                else if (resource is EbEmailNode)
                {
                    EbEmailTemplate emailTemplate = GetEbObject<EbEmailTemplate>(resource.Reference, Redis, ObjectsDB);

                    parameters = parameters.Merge(GetEmailParams(emailTemplate, Redis, ObjectsDB));
                }
                else if (resource is EbConnectApi)
                {
                    EbApi ob = GetEbObject<EbApi>(resource.Reference, Redis, ObjectsDB);

                    parameters = parameters.Merge(GetReqJsonParameters(ob.Resources, Redis, ObjectsDB));
                }
                else if (resource is EbThirdPartyApi thirdParty)
                {
                    if (thirdParty.Parameters != null && thirdParty.Parameters.Count > 0)
                    {
                        foreach (var param in thirdParty.Parameters)
                        {
                            parameters.Add(new Param
                            {
                                Name = param.Name,
                                Type = param.Type.ToString(),
                                Value = param.Value
                            });
                        }
                    }
                }
            }

            return parameters;
        }

        public static object GetResult(ApiResources resource, EbApi Api, int index = 0, int parentindex = 0)
        {
            ResultWrapper res = new ResultWrapper();

            switch (resource)
            {
                case EbSqlReader reader:
                    res.Result = reader.ExecuteDataReader(Api);
                    break;
                case EbSqlWriter writer:
                    res.Result = writer.ExecuteDataWriter(Api);
                    break;
                case EbSqlFunc func:
                    res.Result = func.ExecuteSqlFunction(Api);
                    break;
                case EbEmailNode email:
                    res.Result = email.ExecuteEmail(Api);
                    break;
                case EbProcessor processor:
                    res.Result = processor.ExecuteScript(Api);
                    break;
                case EbConnectApi ebApi:
                    res.Result = ebApi.ExecuteConnectApi(Api);
                    break;
                case EbThirdPartyApi thirdParty:
                    res.Result = thirdParty.ExecuteThirdPartyApi(thirdParty, Api);
                    break;
                case EbFormResource form:
                    res.Result = form.ExecuteFormResource(Api);
                    break;
                case EbEmailRetriever retriever:
                    res.Result = retriever.ExecuteEmailRetriever(Api, true);
                    break;
                case EbFtpPuller puller:
                    res.Result = puller.ExecuteFtpPuller();
                    break;
                case EbCSVPusher pusher:
                    res.Result = pusher.ExecuteCSVPusher(Api);
                    break;
                case EbLoop loop:
                    res.Result = loop.DoLoop(Api, index, parentindex);
                    break;
                default:
                    res.Result = null;
                    break;
            }

            return res.Result;
        }
    }
}
