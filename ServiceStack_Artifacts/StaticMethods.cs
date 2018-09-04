using ExpressBase.Common;
using ExpressBase.Common.Data;
using ExpressBase.Common.Structures;
using ExpressBase.Objects.EmailRelated;
using ExpressBase.Objects.Objects;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using System.Text.RegularExpressions;

namespace ExpressBase.Objects.ServiceStack_Artifacts
{
  public class StaticMethods
    {
      public static string CreateObject(DbConnection con, EbConnectionFactory EbConnectionFactory, EbObject_Create_New_ObjectRequest request,string _appids)
        {
            con.Open();
            DbCommand cmd = null;         //
            string[] arr = { };
            string refId;
            String sql = EbConnectionFactory.ObjectsDB.EB_CREATE_NEW_OBJECT;
            cmd = EbConnectionFactory.ObjectsDB.GetNewCommand(con, sql);

            cmd.Parameters.Add(EbConnectionFactory.ObjectsDB.GetNewParameter(":obj_name", EbDbTypes.String, request.Name.Replace("\n", "").Replace("\t", "").Replace("\r", "")));
            cmd.Parameters.Add(EbConnectionFactory.ObjectsDB.GetNewParameter(":obj_desc", EbDbTypes.String, (!string.IsNullOrEmpty(request.Description)) ? request.Description : string.Empty));
            cmd.Parameters.Add(EbConnectionFactory.ObjectsDB.GetNewParameter(":obj_type", EbDbTypes.Int32, GetObjectType(EbSerializers.Json_Deserialize(request.Json))));
            cmd.Parameters.Add(EbConnectionFactory.ObjectsDB.GetNewParameter(":obj_cur_status", EbDbTypes.Int32, (int)request.Status));
            cmd.Parameters.Add(EbConnectionFactory.ObjectsDB.GetNewParameter(":commit_uid", EbDbTypes.Int32, request.UserId));
            cmd.Parameters.Add(EbConnectionFactory.ObjectsDB.GetNewParameter(":src_pid", EbDbTypes.String, request.SourceSolutionId));
            cmd.Parameters.Add(EbConnectionFactory.ObjectsDB.GetNewParameter(":cur_pid", EbDbTypes.String, request.TenantAccountId));
            cmd.Parameters.Add(EbConnectionFactory.ObjectsDB.GetNewParameter(":relations", EbDbTypes.String, (request.Relations != null) ? request.Relations : string.Empty));
            cmd.Parameters.Add(EbConnectionFactory.ObjectsDB.GetNewParameter(":issave", EbDbTypes.String, (request.IsSave == true) ? 'T' : 'F'));
            cmd.Parameters.Add(EbConnectionFactory.ObjectsDB.GetNewParameter(":tags", EbDbTypes.String, (!string.IsNullOrEmpty(request.Tags)) ? request.Tags : string.Empty));
            cmd.Parameters.Add(EbConnectionFactory.ObjectsDB.GetNewParameter(":app_id", EbDbTypes.String, _appids));
            cmd.Parameters.Add(EbConnectionFactory.ObjectsDB.GetNewParameter(":s_obj_id", EbDbTypes.String, request.SourceObjId));
            cmd.Parameters.Add(EbConnectionFactory.ObjectsDB.GetNewParameter(":s_ver_id", EbDbTypes.String, request.SourceVerID));

            if (sql.Contains(":obj_json"))
            {
                cmd.Parameters.Add(EbConnectionFactory.ObjectsDB.GetNewParameter(":obj_json", EbDbTypes.Json, request.Json));
                refId = cmd.ExecuteScalar().ToString();
            }
            else
            {
                refId = cmd.ExecuteScalar().ToString();
                string sql1 = "update eb_objects_ver set obj_json=:jsonobj where refid=:refid";

                NTV[] parms = new NTV[2];
                parms[0] = new NTV() { Name = ":jsonobj", Type = EbDbTypes.Json, Value = request.Json };
                parms[1] = new NTV
                {
                    Name = ":refid",
                    Type = EbDbTypes.String,
                    Value = refId
                };

                Update_Json_Val(con, sql1, parms, EbConnectionFactory);
            }
            return refId;
        }
        public static int GetObjectType(object obj)
        {
            if (obj is EbDataSource)
                return EbObjectTypes.DataSource.IntCode;
            else if (obj is EbTableVisualization)
                return EbObjectTypes.TableVisualization.IntCode;
            else if (obj is EbChartVisualization)
                return EbObjectTypes.ChartVisualization.IntCode;
            else if (obj is EbWebForm)
                return EbObjectTypes.WebForm.IntCode;
            else if (obj is EbReport)
                return EbObjectTypes.Report.IntCode;
            else if (obj is EbFilterDialog)
                return EbObjectTypes.FilterDialog.IntCode;
            else if (obj is EbEmailTemplate)
                return EbObjectTypes.EmailBuilder.IntCode;
            else if (obj is EbBotForm)
                return EbObjectTypes.BotForm.IntCode;
            else
                return -1;
        }

        public static void Update_Json_Val(DbConnection con, String qry, NTV[] param, EbConnectionFactory EbConnectionFactory)
        {
            try
            {
                DbTransaction transaction = con.BeginTransaction();
                DbCommand cmnd = null;
                cmnd = con.CreateCommand();
                cmnd.Transaction = transaction;
                cmnd.CommandText = qry;

                foreach (NTV para in param)
                {
                    DbParameter parm = EbConnectionFactory.ObjectsDB.GetNewParameter(para.Name, para.Type);
                    parm.Value = para.Value;
                    cmnd.Parameters.Add(parm);
                }

                cmnd.ExecuteNonQuery();
                cmnd.Transaction.Commit();
            }

            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.ToString());
            }
        }
    }
}
