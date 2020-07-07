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
using Oracle.ManagedDataAccess.Client;
using System.Net;

namespace ExpressBase.Objects
{
    [UsedWithTopObjectParent(typeof(EbObject))]
    [EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm)]
    [HideInPropertyGrid]
    public class EbFormNotification
    {
        public EbFormNotification() { }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm)]
        [HideInPropertyGrid]
        public string Name { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm)]
        [HideInPropertyGrid]
        public string EbSid { get; set; }

        [PropertyGroup("Behavior")]
        [EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm)]
        [PropertyEditor(PropertyEditorType.ScriptEditorCS)]
        public EbScript SendOnlyIf { get; set; }
    }

    [Alias("System")]
    [EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm)]
    public class EbFnSystem : EbFormNotification
    {
        public EbFnSystem() { }

        [PropertyGroup("Behavior")]
        [EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm)]
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
        [EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm)]
        [PropertyEditor(PropertyEditorType.ScriptEditorCS)]//required ScriptEditorSQ
        public EbScript Users { get; set; }

        [PropertyGroup("Behavior")]
        [EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm)]
        [Unique]
        [PropDataSourceJsFn("return ebcontext.Roles")]
        [PropertyEditor(PropertyEditorType.DropDown, true)]
        public List<Int32> Roles { get; set; }

        [PropertyGroup("Behavior")]
        [EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm)]
        [PropDataSourceJsFn("return ebcontext.UserGroups")]
        [PropertyEditor(PropertyEditorType.DropDown)]
        public int UserGroup { get; set; }

        [PropertyGroup("Data")]
        [EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm)]
        [PropertyEditor(PropertyEditorType.ScriptEditorCS)]
        public EbScript Message { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm)]
        [HideInPropertyGrid]
        public Dictionary<string, string> QryParams { get; set; }//<param, table>
    }

    [Alias("Email")]
    [EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm)]
    public class EbFnEmail : EbFormNotification
    {
        public EbFnEmail() { }

        [PropertyGroup("Behavior")]
        [PropertyEditor(PropertyEditorType.ObjectSelector)]
        [OSE_ObjectTypes(EbObjectTypes.iEmailBuilder)]
        [EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm)]
        public string RefId { get; set; }
    }

    [Alias("Sms")]
    [EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm)]
    public class EbFnSms : EbFormNotification
    {
        public EbFnSms() { }

        [PropertyEditor(PropertyEditorType.ObjectSelector)]
        [OSE_ObjectTypes(EbObjectTypes.iSmsBuilder)]
        [EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm)]
        public string RefId { get; set; }


        [EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm)]
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
                if (!string.IsNullOrEmpty(ebFn.SendOnlyIf?.Code))
                {
                    object soi = _this.ExecuteCSharpScriptNew(ebFn.SendOnlyIf.Code, globals);
                    if (!(soi is bool && Convert.ToBoolean(soi)))
                        continue;
                }
                if (ebFn is EbFnSystem)
                {
                    EbFnSystem ebFnSys = ebFn as EbFnSystem;
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
                        try
                        {
                            NotifyByUserRoleResponse result = service.Gateway.Send<NotifyByUserRoleResponse>(new NotifyByUserRoleRequest
                            {
                                Link = link,
                                Title = message,
                                RoleID = ebFnSys.Roles
                            });
                        }
                        catch(Exception ex)
                        {
                            string temp = $"Exception when tried to send EbFnSys_NotifyBy.Roles\n Message: ${ex.Message} \nLink: ${link} \nTitle: ${message} \nRolesId: ${(ebFnSys?.Roles == null? "null": string.Join(",", ebFnSys.Roles))} \nStackTrace: ${ex.StackTrace}";
                            Console.WriteLine(temp);
                            throw new FormException($"Unable to process notification.", (int)HttpStatusCode.InternalServerError, ex.Message, temp);
                        }
                        resp++;
                    }
                    else if (ebFnSys.NotifyBy == EbFnSys_NotifyBy.UserGroup)
                    {
                        try
                        {
                            NotifyByUserGroupResponse result = service.Gateway.Send<NotifyByUserGroupResponse>(new NotifyByUserGroupRequest
                            {
                                Link = link,
                                Title = message,
                                GroupId = new List<int> { ebFnSys.UserGroup }
                            });
                        }
                        catch(Exception ex)
                        {
                            string temp = $"Exception when tried to send EbFnSys_NotifyBy.UserGroup\n Message: ${ex.Message} \nLink: ${link} \nTitle: ${message} \nGroupId: ${ebFnSys.UserGroup} \nStackTrace: ${ex.StackTrace}";
                            Console.WriteLine(temp);
                            throw new FormException($"Unable to process notification.", (int)HttpStatusCode.InternalServerError, ex.Message, temp);
                        }
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
                                throw new FormException("Bad Request", (int)HttpStatusCode.BadRequest, $"SendNotifications: Notify by UserId parameter {p.Key} is not idetified", $"{p.Value} not found in MultipleTables");
                            TableSchema _table = _this.FormSchema.Tables.Find(e => e.TableName == p.Value);
                            if (_table.TableType != WebFormTableTypes.Normal)
                                throw new FormException("Bad Request", (int)HttpStatusCode.BadRequest, $"SendNotifications: Notify by UserId parameter {p.Key} is not idetified", $"{p.Value} found in MultipleTables but it is not a normal table");
                            if (Table.Count != 1)
                                throw new FormException("Bad Request", (int)HttpStatusCode.BadRequest, $"SendNotifications: Notify by UserId parameter {p.Key} is not idetified", $"{p.Value} found in MultipleTables but table is empty");
                            SingleColumn Column = Table[0].Columns.Find(e => e.Control?.Name == p.Key);
                            if (Column?.Control == null)
                                throw new FormException("Bad Request", (int)HttpStatusCode.BadRequest, $"SendNotifications: Notify by UserId parameter {p.Key} is not idetified", $"{p.Value} found in MultipleTables but data not available");

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
                            try
                            {
                                NotifyByUserIDResponse result = service.Gateway.Send<NotifyByUserIDResponse>(new NotifyByUserIDRequest
                                {
                                    Link = link,
                                    Title = message,
                                    UsersID = uid
                                });
                            }
                            catch (Exception ex)
                            {
                                string temp = $"Exception when tried to send EbFnSys_NotifyBy.Users\n Message: ${ex.Message} \nLink: ${link} \nTitle: ${message} \nUserId: ${uid} \nStackTrace: ${ex.StackTrace}";
                                Console.WriteLine(temp);
                                throw new FormException($"Unable to process notification.", (int)HttpStatusCode.InternalServerError, ex.Message, temp);
                            }
                        }
                        if (uids.Count > 0)
                            resp++;
                    }
                }
                else if (ebFn is EbFnEmail && !string.IsNullOrEmpty((ebFn as EbFnEmail).RefId))
                {
                    try
                    {
                        service.Gateway.Send<EmailAttachmenResponse>(new EmailTemplateWithAttachmentMqRequest
                        {
                            RefId = (ebFn as EbFnEmail).RefId,
                            Params = new List<Param> { { new Param { Name = "id", Type = ((int)EbDbTypes.Int32).ToString(), Value = _this.TableRowId.ToString() } } },
                            SolnId = _this.SolutionObj.SolutionID,
                            UserAuthId = _this.UserObj.AuthId,
                            UserId = _this.UserObj.UserId
                        });
                    }
                    catch (Exception ex)
                    {
                        string temp = $"Exception when tried to send EbFnEmail\n Message: ${ex.Message} \nRefId: ${(ebFn as EbFnEmail).RefId} \nStackTrace: ${ex.StackTrace}";
                        Console.WriteLine(temp);
                        throw new FormException($"Unable to process notification.", (int)HttpStatusCode.InternalServerError, ex.Message, temp);
                    }
                    resp++;
                }
                if (ebFn is EbFnSms && !string.IsNullOrEmpty((ebFn as EbFnSms).RefId))
                {
                    try
                    {
                        service.Gateway.Send<EmailAttachmenResponse>(new SMSInitialRequest
                        {
                            RefId = (ebFn as EbFnSms).RefId,
                            Params = new List<Param> { { new Param { Name = "id", Type = ((int)EbDbTypes.Int32).ToString(), Value = _this.TableRowId.ToString() } } },
                            SolnId = _this.SolutionObj.SolutionID,
                            UserAuthId = _this.UserObj.AuthId,
                            UserId = _this.UserObj.UserId
                        });
                    }
                    catch (Exception ex)
                    {
                        string temp = $"Exception when tried to send EbFnSms\n Message: ${ex.Message} \nRefId: ${(ebFn as EbFnSms).RefId} \nStackTrace: ${ex.StackTrace}";
                        Console.WriteLine(temp);
                        throw new FormException($"Unable to process notification.", (int)HttpStatusCode.InternalServerError, ex.Message, temp);
                    }
                    resp++;
                }
            }
            return resp;
        }
    }
}
