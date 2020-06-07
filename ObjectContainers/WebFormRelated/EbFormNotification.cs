using ExpressBase.Common;
using ExpressBase.Common.Data;
using ExpressBase.Common.Extensions;
using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using ExpressBase.Common.Structures;
using ExpressBase.CoreBase.Globals;
using ExpressBase.Objects.ServiceStack_Artifacts;
using ExpressBase.Objects.WebFormRelated;
using Newtonsoft.Json;
using ServiceStack;
using System;
using System.Collections.Generic;
using System.Data.Common;

namespace ExpressBase.Objects
{
    [UsedWithTopObjectParent(typeof(EbObject))]
    [EnableInBuilder(BuilderType.WebForm)]
    [HideInPropertyGrid]
    public class EbFormNotification
    {
        public EbFormNotification() { }

        [EnableInBuilder(BuilderType.WebForm)]
        [HideInPropertyGrid]
        public string Name { get; set; }

        [EnableInBuilder(BuilderType.WebForm)]
        [HideInPropertyGrid]
        public string EbSid { get; set; }

        [PropertyGroup("Behavior")]
        [EnableInBuilder(BuilderType.WebForm)]
        [PropertyEditor(PropertyEditorType.ScriptEditorCS)]
        public EbScript SendOnlyIf { get; set; }
    }

    [Alias("System")]
    [EnableInBuilder(BuilderType.WebForm)]
    public class EbFnSystem : EbFormNotification
    {
        public EbFnSystem() { }

        [PropertyGroup("Behavior")]
        [EnableInBuilder(BuilderType.WebForm)]
        [OnChangeExec(@"
if (this.NotifyBy === 0) this.NotifyBy = 1;
pg.HideProperty('Users');
pg.HideProperty('Roles');
pg.HideProperty('UserGroup');
if(this.NotifyBy === 1)
    pg.ShowProperty('Users');
else if(this.NotifyBy === 2)
    pg.ShowProperty('Roles');
else if(this.NotifyBy === 3)
    pg.ShowProperty('UserGroup');
")]
        public EbFnSys_NotifyBy NotifyBy { get; set; }
        
        [PropertyGroup("Behavior")]
        [EnableInBuilder(BuilderType.WebForm)]
        [PropertyEditor(PropertyEditorType.ScriptEditorCS)]//required ScriptEditorSQ
        public EbScript Users { get; set; }

        [PropertyGroup("Behavior")]
        [EnableInBuilder(BuilderType.WebForm)]
        [Unique]
        [PropDataSourceJsFn("return ebcontext.Roles")]
        [PropertyEditor(PropertyEditorType.DropDown, true)]
        public List<Int32> Roles { get; set; }

        [PropertyGroup("Behavior")]
        [EnableInBuilder(BuilderType.WebForm)]
        [PropDataSourceJsFn("return ebcontext.UserGroups")]
        [PropertyEditor(PropertyEditorType.DropDown)]
        public int UserGroup { get; set; }

        [PropertyGroup("Data")]
        [EnableInBuilder(BuilderType.WebForm)]
        [PropertyEditor(PropertyEditorType.ScriptEditorCS)]
        public EbScript Message { get; set; }

        [EnableInBuilder(BuilderType.WebForm)]
        [HideInPropertyGrid]
        public Dictionary<string, string> QryParams { get; set; }//<param, table>
    }

    [Alias("Email")]
    [EnableInBuilder(BuilderType.WebForm)]
    public class EbFnEmail : EbFormNotification
    {
        public EbFnEmail() { }

        [EnableInBuilder(BuilderType.WebForm)]
        public string Test2 { get; set; }
    }

    [Alias("Sms")]
    [EnableInBuilder(BuilderType.WebForm)]
    public class EbFnSms : EbFormNotification
    {
        public EbFnSms() { }

        [EnableInBuilder(BuilderType.WebForm)]
        public string Test3 { get; set; }
    }

    public enum EbFnSys_NotifyBy
    {
        Users = 1,
        Roles = 2,
        UserGroup = 3
    }

    public static class EbFnGateway
    {
        public static int SendNotifications(EbWebForm _this, IDatabase DataDB, Service service)
        {
            if (_this.Notifications?.Count == 0)
                return 0;
            int resp = 0;
            FG_Root globals = GlobalsGenerator.GetCSharpFormGlobals_NEW(_this, _this.FormData, _this.FormDataBackup);
            foreach (EbFormNotification ebFn in _this.Notifications)
            {
                if (ebFn is EbFnSystem)
                {
                    EbFnSystem ebFnSys = ebFn as EbFnSystem;
                    if (string.IsNullOrEmpty(ebFnSys.SendOnlyIf?.Code))
                        continue;
                    object soi = _this.ExecuteCSharpScriptNew(ebFnSys.SendOnlyIf.Code, globals);
                    if (!(soi is bool && Convert.ToBoolean(soi)))
                        continue;
                    string message = "Notification from " + _this.DisplayName;
                    if (!string.IsNullOrEmpty(ebFnSys.Message?.Code))
                    {
                        object msg = _this.ExecuteCSharpScriptNew(ebFnSys.Message.Code, globals);
                        message = msg.ToString();
                    }
                    List<Param> plist = new List<Param> { { new Param { Name = "id", Type = ((int)EbDbTypes.Int32).ToString(), Value = _this.TableRowId.ToString() } } };
                    string _params = JsonConvert.SerializeObject(plist).ToBase64();
                    string link = $"/WebForm/Index?refId={_this.RefId}&_params={_params}&_mode=1";

                    if (ebFnSys.NotifyBy == EbFnSys_NotifyBy.Roles)
                    {
                        NotifyByUserRoleResponse result = service.Gateway.Send<NotifyByUserRoleResponse>(new NotifyByUserRoleRequest
                        {
                            Link = link,
                            Title = message,
                            RoleID = ebFnSys.Roles
                        });
                        resp++;
                    }
                    else if (ebFnSys.NotifyBy == EbFnSys_NotifyBy.UserGroup)
                    {
                        NotifyByUserGroupResponse result = service.Gateway.Send<NotifyByUserGroupResponse>(new NotifyByUserGroupRequest
                        {
                            Link = link,
                            Title = message,
                            GroupId = new List<int> { ebFnSys.UserGroup }
                        });
                        resp++;
                    }
                    else if (ebFnSys.NotifyBy == EbFnSys_NotifyBy.Users)
                    {
                        string t1 = string.Empty, t2 = string.Empty, t3 = string.Empty;
                        List<DbParameter> _p = new List<DbParameter>();
                        int _idx = 0;
                        foreach (KeyValuePair<string, string> p in ebFnSys.QryParams)
                        {
                            SingleTable Table = null;
                            if (_this.FormData.MultipleTables.ContainsKey(p.Value))
                                Table = _this.FormData.MultipleTables[p.Value];
                            else if (_this.FormDataBackup?.MultipleTables.ContainsKey(p.Value) == true)
                                Table = _this.FormDataBackup.MultipleTables[p.Value];
                            else
                                new FormException($"Notify by UserId parameter {p.Key} is not idetified", (int)HttpStatusCodes.BAD_REQUEST, "SendNotifications", $"{p.Value} not found in MultipleTables");
                            TableSchema _table = _this.FormSchema.Tables.Find(e => e.TableName == p.Value);
                            if (_table.TableType != WebFormTableTypes.Normal)
                                new FormException($"Notify by UserId parameter {p.Key} is not idetified", (int)HttpStatusCodes.BAD_REQUEST, "SendNotifications", $"{p.Value} found in MultipleTables but it is not a normal table");
                            if (Table.Count != 1)
                                new FormException($"Notify by UserId parameter {p.Key} is not idetified", (int)HttpStatusCodes.BAD_REQUEST, "SendNotifications", $"{p.Value} found in MultipleTables but table is empty");
                            SingleColumn Column = Table[0].Columns.Find(e => e.Control?.Name == p.Key);
                            if (Column?.Control == null)
                                new FormException($"Notify by UserId parameter {p.Key} is not idetified", (int)HttpStatusCodes.BAD_REQUEST, "SendNotifications", $"{p.Value} found in MultipleTables but data not available");

                            Column.Control.ParameterizeControl(DataDB, _p, null, Column, true, ref _idx, ref t1, ref t2, ref t3, _this.UserObj, null);
                            _p[_idx - 1].ParameterName = p.Key;
                        }
                        List<int> uids = new List<int>();
                        EbDataTable dt = DataDB.DoQuery(ebFnSys.Users.Code, _p.ToArray());
                        foreach (EbDataRow dr in dt.Rows)
                        {
                            int.TryParse(dr[0].ToString(), out int temp);
                            if (!uids.Contains(temp))
                                uids.Add(temp);
                        }
                        foreach (int uid in uids)
                        {
                            NotifyByUserIDResponse result = service.Gateway.Send<NotifyByUserIDResponse>(new NotifyByUserIDRequest
                            {
                                Link = link,
                                Title = message,
                                UsersID = uid
                            });
                        }
                        if (uids.Count > 0)
                            resp++;
                    }
                }
            }
            return resp;
        }
    }
}
