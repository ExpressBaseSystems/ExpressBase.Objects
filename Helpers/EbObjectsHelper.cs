using ExpressBase.Common;
using ExpressBase.Common.Data;
using ExpressBase.Common.Extensions;
using ExpressBase.Common.Objects;
using ExpressBase.Common.Structures;
using ExpressBase.Objects.ServiceStack_Artifacts;
using ServiceStack;
using ServiceStack.Redis;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace ExpressBase.Objects.Helpers
{
    public class EbObjectsHelper
    {
        public static List<EbObjectWrapper> GetParticularVersion(IDatabase objectsDb, string RefId)
        {
            List<EbObjectWrapper> wrap = new List<EbObjectWrapper>();

            DbParameter[] parameters = {
                                            objectsDb.GetNewParameter("refid", EbDbTypes.String, RefId)
                                       };

            EbDataTable dt = objectsDb.DoQuery(objectsDb.EB_PARTICULAR_VERSION_OF_AN_OBJ, parameters);

            foreach (EbDataRow dr in dt.Rows)
            {
                EbObjectWrapper _ebObject = (new EbObjectWrapper
                {
                    Json = dr[0].ToString(),
                    VersionNumber = dr[1].ToString(),
                    EbObjectType = (dr[4] != DBNull.Value) ? Convert.ToInt32(dr[4]) : 0,
                    Status = Enum.GetName(typeof(ObjectLifeCycleStatus), Convert.ToInt32(dr[2])),
                    Tags = dr[3].ToString(),
                    DisplayName = dr[5].ToString(),
                    RefId = RefId
                });
                wrap.Add(_ebObject);
            }

            return wrap;
        }

        public static SqlFuncTestResponse SqlFuncTest(List<Param> Parameters, string FunctionName, IDatabase DataDB)
        {
            SqlFuncTestResponse resp = new SqlFuncTestResponse();
            List<DbParameter> parameter = new List<DbParameter>();
            List<string> _params = new List<string>();
            string sql = string.Empty;
            if (Parameters.Count > 0)
            {
                foreach (Param p in Parameters)
                {
                    _params.Add(":" + p.Name);
                    parameter.Add(DataDB.GetNewParameter(p.Name, (EbDbTypes)Convert.ToInt32(p.Type), p.Value));
                }
                sql = string.Format(@"SELECT * FROM {0}({1})", FunctionName, string.Join(",", _params));
                resp.Data = DataDB.DoQuery(sql, parameter.ToArray());
                resp.Reponse = true;
            }
            else
            {
                sql = string.Format(@"SELECT * FROM {0}()", FunctionName);
                resp.Data = DataDB.DoQuery(sql);

                resp.Reponse = true;
            }
            return resp;
        }

        public static string GetRefIdByVerId(IDatabase ObjectsDB, int ObjVerId)
        {
            string sql = "SELECT refid FROM eb_objects_ver WHERE id = @id";
            DbParameter[] p = { ObjectsDB.GetNewParameter("id", EbDbTypes.Int32, ObjVerId) };
            EbDataTable _tbl = ObjectsDB.DoQuery(sql, p);
            return _tbl.Rows.Count > 0 ? Convert.ToString(_tbl.Rows[0][0]) : null;
        }

        public static DataSourceDataSetResponse ExecuteDataset(string RefId, int UserId, List<Param> Params, EbConnectionFactory ebConnectionFactory, IRedisClient Redis)
        {
            DataSourceDataSetResponse resp = new DataSourceDataSetResponse();
            resp.Columns = new List<ColumnColletion>();
            string _sql = string.Empty;
            try
            {
                EbDataReader _ds = Redis.Get<EbDataReader>(RefId);
                if (_ds == null)
                {
                    List<EbObjectWrapper> result = EbObjectsHelper.GetParticularVersion(ebConnectionFactory.ObjectsDB, RefId);
                    _ds = EbSerializers.Json_Deserialize(result[0].Json);
                    if (_ds == null)
                    {
                        resp.ResponseStatus = new ResponseStatus { Message = "DataReader is null.... RefId: " + RefId };
                        return resp;
                    }
                    Redis.Set<EbDataReader>(RefId, _ds);
                }
                if (_ds.FilterDialogRefId != string.Empty && _ds.FilterDialogRefId != null)
                {
                    EbFilterDialog _dsf = Redis.Get<EbFilterDialog>(_ds.FilterDialogRefId);
                    if (_dsf == null)
                    {
                        List<EbObjectWrapper> result = EbObjectsHelper.GetParticularVersion(ebConnectionFactory.ObjectsDB, _ds.FilterDialogRefId); 
                        _dsf = EbSerializers.Json_Deserialize(result[0].Json);
                        Redis.Set<EbFilterDialog>(_ds.FilterDialogRefId, _dsf);
                    }
                    if (Params == null)
                        Params = _dsf.GetDefaultParams();
                }

                IDatabase MyDataStore = _ds.GetDatastore(ebConnectionFactory);

                if (_ds != null)
                {
                    string _c = string.Empty;
                    _sql = _ds.Sql;
                }

                try
                {
                    IEnumerable<DbParameter> parameters = DataHelper.GetParams(MyDataStore, false, Params, 0, 0);
                    resp.DataSet = MyDataStore.DoQueries(_sql, parameters.ToArray<System.Data.Common.DbParameter>());

                    foreach (EbDataTable dt in resp.DataSet.Tables)
                        resp.Columns.Add(dt.Columns);

                    //if (GetLogEnabled(RefId))
                    //{
                    //    TimeSpan T = resp.DataSet.EndTime - resp.DataSet.StartTime;
                    //    InsertExecutionLog(resp.DataSet.RowNumbers, T, resp.DataSet.StartTime, UserId, Params, RefId);
                    //}
                }
                catch (Exception e)
                {
                    resp.ResponseStatus = new ResponseStatus { Message = e.Message };
                    Console.WriteLine("DataSourceDataSetResponse------" + e.StackTrace);
                    Console.WriteLine("DataSourceDataSetResponse------" + e.Message);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message + "\n" + e.StackTrace);
            }
            return resp;
        }
    }
}
