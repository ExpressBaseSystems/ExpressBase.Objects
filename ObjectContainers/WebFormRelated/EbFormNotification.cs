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
using System.Linq;
using System.Text.RegularExpressions;
using ExpressBase.Common.NotificationHubs;
using System.Threading.Tasks;
using ExpressBase.Common.Constants;
using ExpressBase.Security;

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

        public virtual void BeforeSaveValidation(Dictionary<int, EbControlWrapper> _dict) { }

        public virtual void SendNotification(EbWebForm _this, EbConnectionFactory ConnFactory, Service service, FG_Root globals, ref int resp) { }

        //for system/mobile notification
        public DbParameter[] GetParameters(EbWebForm _this, IDatabase DataDB, Dictionary<string, string> QryParams)
        {
            List<DbParameter> _p = new List<DbParameter>();
            int _idx = 0;
            foreach (KeyValuePair<string, string> p in QryParams)
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

                ParameterizeCtrl_Params args = new ParameterizeCtrl_Params(DataDB, _p, Column, _idx, _this.UserObj);
                Column.Control.ParameterizeControl(args, _this.CrudContext);
                _idx = args.i;
                _p[_idx - 1].ParameterName = p.Key;
            }
            return _p.ToArray();
        }

        //for system/mobile notification
        public void BeforeSaveValidation4SysMobNotification(Dictionary<int, EbControlWrapper> _dict,
            EbFnSys_NotifyBy NotifyBy, List<Int32> Roles, int UserGroup, EbScript Users,
            Dictionary<string, string> QryParams, string type)
        {
            if (NotifyBy == EbFnSys_NotifyBy.Roles)
            {
                if (!(Roles?.FindAll(e => e > 0).Count() > 0))
                    throw new FormException($"Invalid roles found for {type} notification");
            }
            else if (NotifyBy == EbFnSys_NotifyBy.UserGroup)
            {
                if (UserGroup <= 0)
                    throw new FormException($"Invalid user group found for {type} notification");
            }
            else if (NotifyBy == EbFnSys_NotifyBy.Users)
            {
                QryParams = new Dictionary<string, string>();//<param, table>
                if (string.IsNullOrWhiteSpace(Users?.Code))
                    throw new FormException($"Required SQL query for {type} notification");
                MatchCollection matchColl = Regex.Matches(Users.Code, @"(?<=@)(\w+)|(?<=:)(\w+)");
                foreach (Match match in matchColl)
                {
                    KeyValuePair<int, EbControlWrapper> item = _dict.FirstOrDefault(e => e.Value.Control.Name == match.Value);
                    if (item.Value == null)
                        throw new FormException($"Can't resolve {match.Value} in SQL query of {type} notification");
                    if (!QryParams.ContainsKey(item.Value.Control.Name))
                        QryParams.Add(item.Value.Control.Name, item.Value.TableName);
                }
            }
            else
                throw new FormException($"Invalid NotifyBy found for {type} notification");
        }
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
        [PropertyEditor(PropertyEditorType.ScriptEditorSQ)]
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

        public override void BeforeSaveValidation(Dictionary<int, EbControlWrapper> _dict)
        {
            this.BeforeSaveValidation4SysMobNotification(_dict, this.NotifyBy, this.Roles, this.UserGroup, this.Users, this.QryParams, "system");
        }

        public override void SendNotification(EbWebForm _this, EbConnectionFactory ConnFactory, Service service, FG_Root globals, ref int resp)
        {
            IDatabase DataDB = ConnFactory.DataDB;
            string message = "Notification from " + _this.DisplayName;
            if (!string.IsNullOrEmpty(this.Message?.Code))
            {
                object msg = _this.ExecuteCSharpScriptNew(this.Message.Code, globals);
                message = msg.ToString();
            }
            List<Param> plist = new List<Param> { { new Param { Name = "id", Type = ((int)EbDbTypes.Int32).ToString(), Value = _this.TableRowId.ToString() } } };
            string _params = JsonConvert.SerializeObject(plist).ToBase64();
            string link = $"/WebForm/Index?_r={_this.RefId}&_p={_params}&_m=1";

            if (this.NotifyBy == EbFnSys_NotifyBy.Roles)
            {
                try
                {
                    NotifyByUserRoleResponse result = service.Gateway.Send<NotifyByUserRoleResponse>(new NotifyByUserRoleRequest
                    {
                        Link = link,
                        Title = message,
                        RoleID = this.Roles
                    });
                }
                catch (Exception ex)
                {
                    string temp = $"Exception when tried to send EbFnSys_NotifyBy.Roles\n Message: ${ex.Message} \nLink: ${link} \nTitle: ${message} \nRolesId: ${(this?.Roles == null ? "null" : string.Join(",", this.Roles))} \nStackTrace: ${ex.StackTrace}";
                    //Console.WriteLine(temp);
                    throw new FormException($"Unable to process notification.", (int)HttpStatusCode.InternalServerError, ex.Message, temp);
                }
                resp++;
            }
            else if (this.NotifyBy == EbFnSys_NotifyBy.UserGroup)
            {
                try
                {
                    NotifyByUserGroupResponse result = service.Gateway.Send<NotifyByUserGroupResponse>(new NotifyByUserGroupRequest
                    {
                        Link = link,
                        Title = message,
                        GroupId = new List<int> { this.UserGroup }
                    });
                }
                catch (Exception ex)
                {
                    string temp = $"Exception when tried to send EbFnSys_NotifyBy.UserGroup\n Message: ${ex.Message} \nLink: ${link} \nTitle: ${message} \nGroupId: ${this.UserGroup} \nStackTrace: ${ex.StackTrace}";
                    //Console.WriteLine(temp);
                    throw new FormException($"Unable to process notification.", (int)HttpStatusCode.InternalServerError, ex.Message, temp);
                }
                resp++;
            }
            else if (this.NotifyBy == EbFnSys_NotifyBy.Users)
            {
                DbParameter[] _p = this.GetParameters(_this, ConnFactory.DataDB, this.QryParams);
                List<int> uids = new List<int>();
                EbDataTable dt = DataDB.DoQuery(this.Users.Code, _p);
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
                            UsersID = uid,
                            User_AuthId = _this.UserObj.AuthId
                        });
                    }
                    catch (Exception ex)
                    {
                        string temp = $"Exception when tried to send EbFnSys_NotifyBy.Users\n Message: ${ex.Message} \nLink: ${link} \nTitle: ${message} \nUserId: ${uid} \nStackTrace: ${ex.StackTrace}";
                        Console.WriteLine("NotifyByUserIDRequest Inner Exception 1" + ex.InnerException?.Message + ex.InnerException?.StackTrace);
                        Console.WriteLine("NotifyByUserIDRequest Inner Exception 2 " + ex.InnerException?.InnerException?.Message + ex.InnerException?.InnerException?.StackTrace);

                        throw new FormException($"Unable to process notification.", (int)HttpStatusCode.InternalServerError, ex.Message, temp);
                    }
                }
                if (uids.Count > 0)
                    resp++;
            }
        }
    }

    [Alias("Mobile")]
    [EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm)]
    public class EbFnMobile : EbFormNotification
    {
        public EbFnMobile() { }

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
        [PropertyEditor(PropertyEditorType.ScriptEditorSQ)]
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

        [PropertyGroup("Behavior")]
        [PropertyEditor(PropertyEditorType.ObjectSelector)]
        [OSE_ObjectTypes(EbObjectTypes.iMobilePage)]
        [EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm)]
        public string LinkRefId { get; set; }

        [PropertyGroup("Data")]
        [EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm)]
        [PropertyEditor(PropertyEditorType.ScriptEditorCS)]
        public EbScript MessageTitle { get; set; }

        [PropertyGroup("Data")]
        [EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm)]
        [PropertyEditor(PropertyEditorType.ScriptEditorCS)]
        public EbScript Message { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm)]
        [HideInPropertyGrid]
        public Dictionary<string, string> QryParams { get; set; }//<param, table>

        public string ProcessedMsgTitle { get; set; }
        public string ProcessedMessage { get; set; }

        #region NotificationFromApproval

        public bool IsDirectNotification { get; set; }
        public int NotifyUserId { get; set; }

        #endregion NotificationFromApproval

        public override void BeforeSaveValidation(Dictionary<int, EbControlWrapper> _dict)
        {
            this.BeforeSaveValidation4SysMobNotification(_dict, this.NotifyBy, this.Roles, this.UserGroup, this.Users, this.QryParams, "mobile");
        }

        public override void SendNotification(EbWebForm _this, EbConnectionFactory ConnFactory, Service service, FG_Root globals, ref int resp)
        {
            if (ConnFactory.MobileAppConnection == null)
                return;

            List<int> uids = new List<int>();

            if (this.IsDirectNotification)
            {
                uids.Add(this.NotifyUserId);
            }
            else
            {
                string Qry = null;
                DbParameter[] _p = null;
                if (this.NotifyBy == EbFnSys_NotifyBy.Roles)
                    Qry = $"SELECT user_id FROM eb_role2user WHERE role_id = ANY(STRING_TO_ARRAY('{this.Roles.Join(",")}'::TEXT, ',')::INT[]) AND COALESCE(eb_del, 'F') = 'F'; ";
                else if (this.NotifyBy == EbFnSys_NotifyBy.UserGroup)
                    Qry = $"SELECT userid FROM eb_user2usergroup WHERE groupid = {this.UserGroup} AND COALESCE(eb_del, 'F') = 'F'; ";
                else if (this.NotifyBy == EbFnSys_NotifyBy.Users)
                {
                    Qry = this.Users.Code;
                    _p = this.GetParameters(_this, ConnFactory.DataDB, this.QryParams);
                }

                if (!string.IsNullOrWhiteSpace(Qry))
                {
                    EbDataTable dt;
                    if (_p == null)
                        dt = ConnFactory.DataDB.DoQuery(Qry);
                    else
                        dt = ConnFactory.DataDB.DoQuery(Qry, _p);
                    foreach (EbDataRow dr in dt.Rows)
                    {
                        int.TryParse(dr[0].ToString(), out int temp);
                        if (!uids.Contains(temp))
                            uids.Add(temp);
                    }
                    if (uids.Count == 0)
                        return;

                    this.ProcessedMsgTitle = _this.DisplayName;
                    if (!string.IsNullOrEmpty(this.MessageTitle?.Code))
                    {
                        object msg = _this.ExecuteCSharpScriptNew(this.MessageTitle.Code, globals);
                        this.ProcessedMsgTitle = msg.ToString();
                    }
                    this.ProcessedMessage = string.Empty;
                    if (!string.IsNullOrEmpty(this.Message?.Code))
                    {
                        object msg = _this.ExecuteCSharpScriptNew(this.Message.Code, globals);
                        this.ProcessedMessage = msg.ToString();
                    }
                }
            }

            if (uids.Count > 0)
            {
                List<string> userAuthIds = new List<string>();
                EbNFData Data = new EbNFData()
                {
                    Title = this.ProcessedMsgTitle,
                    Message = this.ProcessedMessage
                };

                EbAzureNFClient client = EbAzureNFClient.Create(ConnFactory.MobileAppConnection);
                foreach (int uid in uids)
                    userAuthIds.Add(client.ConvertToAuthTag(_this.SolutionObj.SolutionID + CharConstants.COLON + uid + CharConstants.COLON + TokenConstants.MC));

                EbNFRequest req = new EbNFRequest()
                {
                    Platform = PNSPlatforms.GCM,
                    Tags = userAuthIds
                };
                req.SetPayload(new EbNFDataTemplateAndroid() { Data = Data });

                try
                {
                    client.Send(req);
                }
                catch (Exception ex)
                {
                    string temp = $"Exception when tried to send EbFnMobile\n Message: ${ex.Message} \nStackTrace: ${ex.StackTrace}";
                    throw new FormException($"Unable to process notification.", (int)HttpStatusCode.InternalServerError, ex.Message, temp);
                }
                resp++;
            }
        }
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

        public override void BeforeSaveValidation(Dictionary<int, EbControlWrapper> _dict)
        {
            if (string.IsNullOrEmpty(this.RefId))
                throw new FormException($"Invalid Ref id found for email notification");
        }

        public override void SendNotification(EbWebForm _this, EbConnectionFactory ConnFactory, Service service, FG_Root globals, ref int resp)
        {
            if (!string.IsNullOrWhiteSpace(this.RefId))
            {
                try
                {
                    service.Gateway.Send<EmailAttachmenResponse>(new EmailTemplateWithAttachmentMqRequest
                    {
                        RefId = this.RefId,
                        Params = new List<Param> { { new Param { Name = "id", Type = ((int)EbDbTypes.Int32).ToString(), Value = _this.TableRowId.ToString() } } },
                        SolnId = _this.SolutionObj.SolutionID,
                        UserAuthId = _this.UserObj.AuthId,
                        UserId = _this.UserObj.UserId
                    });
                }
                catch (Exception ex)
                {
                    string temp = $"Exception when tried to send EbFnEmail\n Message: ${ex.Message} \nRefId: ${this.RefId} \nStackTrace: ${ex.StackTrace}";
                    throw new FormException($"Unable to process notification.", (int)HttpStatusCode.InternalServerError, ex.Message, temp);
                }
                resp++;
            }
        }
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

        public override void BeforeSaveValidation(Dictionary<int, EbControlWrapper> _dict)
        {
            if (string.IsNullOrEmpty(this.RefId))
                throw new FormException($"Invalid Ref id found for SMS notification");
        }

        public override void SendNotification(EbWebForm _this, EbConnectionFactory ConnFactory, Service service, FG_Root globals, ref int resp)
        {
            if (!string.IsNullOrWhiteSpace(this.RefId))
            {
                try
                {
                    service.Gateway.Send<EmailAttachmenResponse>(new SMSInitialRequest
                    {
                        RefId = this.RefId,
                        Params = new List<Param> { { new Param { Name = "id", Type = ((int)EbDbTypes.Int32).ToString(), Value = _this.TableRowId.ToString() } } },
                        SolnId = _this.SolutionObj.SolutionID,
                        UserAuthId = _this.UserObj.AuthId,
                        UserId = _this.UserObj.UserId
                    });
                }
                catch (Exception ex)
                {
                    string temp = $"Exception when tried to send EbFnSms\n Message: ${ex.Message} \nRefId: ${this.RefId} \nStackTrace: ${ex.StackTrace}";
                    throw new FormException($"Unable to process notification.", (int)HttpStatusCode.InternalServerError, ex.Message, temp);
                }
                resp++;
            }
        }
    }

    public enum EbFnSys_NotifyBy
    {
        Users = 1,
        Roles = 2,
        UserGroup = 3
    }

    public static class EbFnGateway
    {
        //Process => Notifications property + Notifications from nextStage script of review ctrl
        public static int SendNotifications(EbWebForm _this, EbConnectionFactory ConnFactory, Service service)
        {
            if (_this.Notifications?.Count == 0)
                return 0;
            int resp = 0;
            try
            {
                FG_Root globals = GlobalsGenerator.GetCSharpFormGlobals_NEW(_this, _this.FormData, _this.FormDataBackup, ConnFactory.DataDB, null, false);
                foreach (EbFormNotification ebFn in _this.Notifications)
                {
                    if (!string.IsNullOrEmpty(ebFn.SendOnlyIf?.Code))
                    {
                        object soi = _this.ExecuteCSharpScriptNew(ebFn.SendOnlyIf.Code, globals);
                        if (!(soi is bool && Convert.ToBoolean(soi)))
                        {
                            Console.WriteLine($"SendNotifications [SendOnlyIf is not TRUE]: {ebFn.GetType().Name}({ebFn.Name}) skipped.");
                            continue;
                        }
                    }
                    ebFn.SendNotification(_this, ConnFactory, service, globals, ref resp);
                }
            }
            catch (FormException ex)
            {
                Console.WriteLine($"Exception: {ex.Message}\nCode: {ex.ExceptionCode}\nMessageInternal: {ex.MessageInternal}\nStackTraceInteranl: {ex.StackTraceInternal}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}\nStackTrace: {ex.StackTrace}");
            }
            return resp;
        }

        //Send my actions push notifications
        public static async Task<EbNFResponse> SendMobileNotification(EbWebForm _this, EbConnectionFactory ConnFactory, Service service)
        {
            EbNFResponse resp = new EbNFResponse("0");
            try
            {
                if (_this.MyActNotification?.SendPushNotification == true && ConnFactory.MobileAppConnection != null)
                {
                    List<int> userIds = new List<int>();
                    if (_this.MyActNotification.ApproverEntity == ApproverEntityTypes.Users)
                    {
                        userIds = _this.MyActNotification.UserIds;
                    }
                    else if (_this.MyActNotification.ApproverEntity == ApproverEntityTypes.Role ||
                        _this.MyActNotification.ApproverEntity == ApproverEntityTypes.DynamicRole ||
                        _this.MyActNotification.ApproverEntity == ApproverEntityTypes.UserGroup)
                    {
                        string Qry;
                        if (_this.MyActNotification.ApproverEntity == ApproverEntityTypes.UserGroup)
                            Qry = $"SELECT userid FROM eb_user2usergroup WHERE groupid = {_this.MyActNotification.UserGroupId} AND COALESCE(eb_del, 'F') = 'F'; ";
                        else// static/dynamic role
                            Qry = $"SELECT user_id FROM eb_role2user WHERE role_id = ANY(STRING_TO_ARRAY('{_this.MyActNotification.RoleIds.Join(",")}'::TEXT, ',')::INT[]) AND COALESCE(eb_del, 'F') = 'F'; ";

                        EbDataTable dt = ConnFactory.DataDB.DoQuery(Qry);
                        foreach (EbDataRow dr in dt.Rows)
                        {
                            int.TryParse(dr[0].ToString(), out int temp);
                            if (!userIds.Contains(temp))
                                userIds.Add(temp);
                        }
                    }
                    else
                        throw new Exception("Invalid approver entity: " + _this.MyActNotification.ApproverEntity);

                    if (userIds.Count == 0)
                        throw new Exception("User Id collection is empty");

                    List<string> userAuthIds = new List<string>();
                    EbNFData Data = new EbNFData()
                    {
                        Title = _this.MyActNotification.Title,
                        Message = _this.MyActNotification.Description,
                        Link = new EbNFLink()
                        {
                            LinkType = EbNFLinkTypes.Action,
                            ActionId = _this.MyActNotification.MyActionId
                        }
                    };

                    EbAzureNFClient client = EbAzureNFClient.Create(ConnFactory.MobileAppConnection);
                    foreach (int uid in userIds)
                    {
                        string authId = _this.SolutionObj.SolutionID + CharConstants.COLON + uid + CharConstants.COLON + TokenConstants.MC;
                        User user = service.Redis.Get<User>(authId);
                        if (user != null)
                        {
                            if (user.LocationIds.Contains(-1) || user.LocationIds.Contains(_this.LocationId))
                                userAuthIds.Add(client.ConvertToAuthTag(authId));
                        }
                    }
                    if (userAuthIds.Count > 0)
                    {
                        EbNFRequest req = new EbNFRequest()
                        {
                            Platform = PNSPlatforms.GCM,
                            Tags = userAuthIds
                        };
                        req.SetPayload(new EbNFDataTemplateAndroid() { Data = Data });
                        resp = await client.Send(req);
                    }
                }
            }
            catch (Exception ex)
            {
                resp.Message = ex.Message;
                Console.WriteLine("Exception in SendMobileNotification: " + ex.Message);
            }
            Console.WriteLine("SendMobileNotification response: " + resp.Message);
            return resp;
        }

        public static void SendSystemNotifications(EbWebForm _this, FG_Root _globals, Service services)
        {
            List<Param> p = new List<Param> { { new Param { Name = "id", Type = ((int)EbDbTypes.Int32).ToString(), Value = _this.TableRowId.ToString() } } };
            string _params = JsonConvert.SerializeObject(p).ToBase64();
            string link = $"/WebForm/Index?_r={_this.RefId}&_p={_params}&_m=1";

            foreach (FG_Notification notification in _globals.system.Notifications)
            {
                try
                {
                    string title = notification.Title ?? _this.DisplayName + " notification";
                    if (notification.NotifyBy == FG_NotifyBy.UserId)
                    {
                        Console.WriteLine($"PostProcessGlobals -> NotifyByUserIDRequest. Tilte: {title}, UserId: {notification.UserId}");
                        NotifyByUserIDResponse result = services.Gateway.Send<NotifyByUserIDResponse>(new NotifyByUserIDRequest
                        {
                            Link = link,
                            Title = title,
                            UsersID = notification.UserId
                        });
                    }
                    else if (notification.NotifyBy == FG_NotifyBy.RoleIds)
                    {
                        Console.WriteLine($"PostProcessGlobals -> NotifyByUserRoleRequest. Tilte: {title}, RoleIds: {notification.RoleIds}");
                        NotifyByUserRoleResponse result = services.Gateway.Send<NotifyByUserRoleResponse>(new NotifyByUserRoleRequest
                        {
                            Link = link,
                            Title = title,
                            RoleID = notification.RoleIds
                        });
                    }
                    else if (notification.NotifyBy == FG_NotifyBy.UserGroupIds)
                    {
                        Console.WriteLine($"PostProcessGlobals -> NotifyByUserGroupRequest. Tilte: {title}, UserGroupId: {notification.UserGroupIds}");
                        NotifyByUserGroupResponse result = services.Gateway.Send<NotifyByUserGroupResponse>(new NotifyByUserGroupRequest
                        {
                            Link = link,
                            Title = title,
                            GroupId = notification.UserGroupIds
                        });
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Exception in PostProcessGlobals: SystemNotification\nMessage: " + e.Message + "\nStackTrace: " + e.StackTrace);
                }
            }
        }
    }


    public class MyActionNotification
    {
        public int MyActionId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool SendPushNotification { get; set; }

        public ApproverEntityTypes ApproverEntity { get; set; }
        public List<int> UserIds { get; set; }
        public List<int> RoleIds { get; set; }
        public int UserGroupId { get; set; }
    }
}
