using ExpressBase.Common;
using ExpressBase.Common.Data;
using ExpressBase.Common.Extensions;
using ExpressBase.Common.Objects;
using ExpressBase.Common.Structures;
using ExpressBase.Objects.ServiceStack_Artifacts;
using ServiceStack.Redis;
using System;
using System.Collections.Generic;
using System.Data.Common;
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

    }
}
