using ExpressBase.Common;
using ExpressBase.Common.Extensions;
using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Runtime.Serialization;
using ExpressBase.Security;
using Newtonsoft.Json;
using ExpressBase.Common.Structures;
using ExpressBase.Objects.Objects;
using System.Text;
using ServiceStack.RabbitMq;
using ExpressBase.Objects.ServiceStack_Artifacts;
using ExpressBase.Common.Constants;
using ExpressBase.Common.LocationNSolution;
using System.Net;
using ExpressBase.Objects.WebFormRelated;

namespace ExpressBase.Objects
{
    [EnableInBuilder(BuilderType.WebForm)]
    public class EbProvisionUser : EbControlUI, IEbPlaceHolderControl, IEbExtraQryCtrl
    {
        public EbProvisionUser()
        {
            this.Fields = new List<UsrLocFieldAbstract>();
            this.UserTypeToRole = new List<EbUserType>();
        }

        [OnDeserialized]
        public void OnDeserializedMethod(StreamingContext context)
        {
            this.BareControlHtml = this.GetBareHtml();
            this.ObjType = this.GetType().Name.Substring(2, this.GetType().Name.Length - 2);
            if (this.CreateOnlyIf == null)
                this.CreateOnlyIf = new EbScript();
        }

        public override string ToolIconHtml { get { return "<i class='fa fa-user'></i><i class='fa fa-plus'></i>"; } set { } }

        public override string ToolNameAlias { get { return "Prov. User"; } set { } }

        public override string ToolHelpText { get { return "Provision User"; } set { } }

        public override string UIchangeFns
        {
            get
            {
                return @"EbProvisionUser = {
                mapping : function(elementId, props) {
                    let html = '';
                    $(`#cont_${elementId}`).html(`<span class='eb-ctrl-label' ui-label id='@ebsid@Lbl' style='font-style: italic; font-weight: normal;'>ProvisionUser</span>`);
                    $.each(props.Fields.$values, function (i, field) {
                        if (field.ControlName === '' && field.IsRequired){
                            html += `<div class = 'prov-loc-item'>
                                        <span class='eb-ctrl-label'>${field.DisplayName}</span>
                                        <div class='ctrl-cover'><input type='text' autocomplete = 'off' style='width:100%; display:inline-block;' disabled /></div>
                                    </div>`;
                        }
                    });
                    $(`#cont_${elementId}`).append(html);
                }
            }";
            }
        }

        [EnableInBuilder(BuilderType.WebForm)]
        //[HideInPropertyGrid]
        public override bool Hidden { get; set; }

        [EnableInBuilder(BuilderType.WebForm)]
        [HideInPropertyGrid]
        public override bool IsSysControl { get { return false; } }

        [EnableInBuilder(BuilderType.WebForm)]
        [PropertyGroup(PGConstants.CORE)]
        [UIproperty]
        //[OnChangeUIFunction("EbProvisionUser.mapping")]////////////
        [PropertyEditor(PropertyEditorType.Collection)]
        [ListType(typeof(UsrLocFieldAbstract))]
        public List<UsrLocFieldAbstract> Fields { get; set; }

        //[EnableInBuilder(BuilderType.WebForm)]
        //[HideInPropertyGrid]
        //public override EbDbTypes EbDbType { get { return EbDbTypes.String; } }

        [EnableInBuilder(BuilderType.WebForm)]
        [PropertyEditor(PropertyEditorType.CollectionProp, "UserTypeToRole", "bVisible")]
        [OnChangeExec(@"if (this.UserTypeToRole && this.UserTypeToRole.$values.length === 0 ){
            $.each(ebcontext.UserTypes, function(i, o){
                 this.UserTypeToRole.$values.push($.extend(new EbObjects.EbUserType, {iValue: i, sValue: o, name: o, EbSid: 'usrTyp_' + Date.now().toString(36).substring(3) + i}));
            }.bind(this));            
        }")]
        public List<EbUserType> UserTypeToRole { get; set; }

        [EnableInBuilder(BuilderType.WebForm)]
        [PropertyEditor(PropertyEditorType.ScriptEditorCS)]
        public EbScript CreateOnlyIf { get; set; }

        public bool CreateOnlyIf_b { get; set; }

        //[EnableInBuilder(BuilderType.WebForm)]
        //[HideInPropertyGrid]
        //public bool SendPwdViaEmail { get; set; }

        [EnableInBuilder(BuilderType.WebForm)]
        public bool AllowExistingUser { get; set; }

        [EnableInBuilder(BuilderType.WebForm)]
        [Alias("Send verification message")]
        public bool SendVerificationMsg { get; set; }

        [EnableInBuilder(BuilderType.WebForm)]
        public override EbScript HiddenExpr { get; set; }


        //--------Hide in property grid------------

        [EnableInBuilder(BuilderType.WebForm)]
        [HideInPropertyGrid]
        public override string HelpText { get; set; }

        [EnableInBuilder(BuilderType.WebForm)]
        [HideInPropertyGrid]
        public override string ToolTipText { get; set; }

        [EnableInBuilder(BuilderType.WebForm)]
        [HideInPropertyGrid]
        public override bool Unique { get; set; }

        [EnableInBuilder(BuilderType.WebForm)]
        [HideInPropertyGrid]
        public override List<EbValidator> Validators { get; set; }

        [EnableInBuilder(BuilderType.WebForm)]
        [HideInPropertyGrid]
        public override EbScript DefaultValueExpression { get; set; }

        [EnableInBuilder(BuilderType.WebForm)]
        [HideInPropertyGrid]
        public override EbScript DisableExpr { get; set; }

        [EnableInBuilder(BuilderType.WebForm)]
        [HideInPropertyGrid]
        public override EbScript ValueExpr { get; set; }

        public override bool SelfTrigger { get; set; }

        [EnableInBuilder(BuilderType.WebForm)]
        [HideInPropertyGrid]
        public override bool IsDisable { get; set; }

        [EnableInBuilder(BuilderType.WebForm)]
        [HideInPropertyGrid]
        public override bool Required { get; set; }

        [EnableInBuilder(BuilderType.WebForm)]
        [HideInPropertyGrid]
        public override bool DoNotPersist { get { return false; } }

        [EnableInBuilder(BuilderType.WebForm)]
        [HideInPropertyGrid]
        public override string BackColor { get; set; }

        [EnableInBuilder(BuilderType.WebForm)]
        [HideInPropertyGrid]
        public override string ForeColor { get; set; }

        [EnableInBuilder(BuilderType.WebForm)]
        [HideInPropertyGrid]
        public override string LabelBackColor { get; set; }

        [EnableInBuilder(BuilderType.WebForm)]
        [HideInPropertyGrid]
        public override string LabelForeColor { get; set; }

        [EnableInBuilder(BuilderType.WebForm)]
        [HideInPropertyGrid]
        public override EbScript OnChangeFn { get; set; }

        //-----------------------------------------------------

        public NTV[] FuncParam = {
            //new NTV (){ Name = "userid", Type = EbDbTypes.Int32, Value = DBNull.Value},//eb_createdby
            new NTV (){ Name = "id", Type = EbDbTypes.Int32, Value = 0},
            new NTV (){ Name = "fullname", Type = EbDbTypes.String, Value = DBNull.Value},/////
            new NTV (){ Name = "nickname", Type = EbDbTypes.String, Value = DBNull.Value},/////
            new NTV (){ Name = "email", Type = EbDbTypes.String, Value = DBNull.Value},//////

            new NTV (){ Name = "pwd", Type = EbDbTypes.String, Value = DBNull.Value},
            new NTV (){ Name = "dob", Type = EbDbTypes.Date, Value = DBNull.Value},//////
            new NTV (){ Name = "sex", Type = EbDbTypes.String, Value = DBNull.Value},/////
            new NTV (){ Name = "alternateemail", Type = EbDbTypes.String, Value = DBNull.Value},///////
            new NTV (){ Name = "phprimary", Type = EbDbTypes.String, Value = DBNull.Value},/////

            new NTV (){ Name = "phsecondary", Type = EbDbTypes.String, Value = DBNull.Value},
            new NTV (){ Name = "phlandphone", Type = EbDbTypes.String, Value = DBNull.Value},
            new NTV (){ Name = "extension", Type = EbDbTypes.String, Value = DBNull.Value},
            new NTV (){ Name = "fbid", Type = EbDbTypes.String, Value = DBNull.Value},
            new NTV (){ Name = "fbname", Type = EbDbTypes.String, Value = DBNull.Value},

            new NTV (){ Name = "roles", Type = EbDbTypes.String, Value = DBNull.Value},
            new NTV (){ Name = "groups", Type = EbDbTypes.String, Value = DBNull.Value},//////
            new NTV (){ Name = "statusid", Type = EbDbTypes.Int32, Value = 0},
            new NTV (){ Name = "hide", Type = EbDbTypes.String, Value = "no"},
            new NTV (){ Name = "anonymoususerid", Type = EbDbTypes.Int32, Value = DBNull.Value},

            new NTV (){ Name = "preference", Type = EbDbTypes.String, Value = "{\"Locale\":\"en-IN\",\"TimeZone\":\"(UTC+05:30) Chennai, Kolkata, Mumbai, New Delhi\"}"},/////
            new NTV (){ Name = "usertype", Type = EbDbTypes.Int32, Value = 1},//////
            new NTV (){ Name = "consadd", Type = EbDbTypes.String, Value = string.Empty},
            new NTV (){ Name = "consdel", Type = EbDbTypes.String, Value = string.Empty},
            new NTV (){ Name = "forcepwreset", Type = EbDbTypes.String, Value = "F"},

            new NTV (){ Name = "isolution_id", Type = EbDbTypes.String, Value = string.Empty}
        };

        public override string GetJsInitFunc()
        {
            return @"
this.Init = function(id)
{
    let efield = new EbObjects.UsrLocField('email');
    efield.DisplayName = 'Email';
    //efield.IsRequired = true;
	this.Fields.$values.push(efield);
	this.Fields.$values.push(new EbObjects.UsrLocField('fullname'));
	this.Fields.$values.push(new EbObjects.UsrLocField('nickname'));
	this.Fields.$values.push(new EbObjects.UsrLocField('dob'));
	this.Fields.$values.push(new EbObjects.UsrLocField('sex'));
	this.Fields.$values.push(new EbObjects.UsrLocField('alternateemail'));
	this.Fields.$values.push(new EbObjects.UsrLocField('phprimary'));
	this.Fields.$values.push(new EbObjects.UsrLocField('usertype'));
	this.Fields.$values.push(new EbObjects.UsrLocField('groups'));
	this.Fields.$values.push(new EbObjects.UsrLocField('preference'));
	this.Fields.$values.push(new EbObjects.UsrLocField('consadd'));
};";
        }

        public override string GetBareHtml()
        {
            return @"<div class='pu-cont' id='@ebsid@' data-ebtype='@data-ebtype@'>
                        <div class='pu-txt-info'>- Design html is not implemented -</div>
                    </div>"
.Replace("@name@", (this.Name != null ? this.Name.Trim() : ""))
.Replace("@data-ebtype@", "16")//( (int)this.EbDateType ).ToString())
.Replace("@toolTipText@", this.ToolTipText)
.Replace("@ebsid@", String.IsNullOrEmpty(this.EbSid_CtxId) ? "@ebsid@" : this.EbSid_CtxId)
.Replace("@value@", "")//"value='" + this.Value + "'")
.Replace("@tabIndex@", "tabindex='" + this.TabIndex + "'")
.Replace("@BackColor@ ", "background-color: #eee;")
    //.Replace("@BackColor@ ", ("background-color:" + ((this.BackColor != null) ? this.BackColor : "@BackColor@ ") + ";"))
    .Replace("@ForeColor@ ", "color:" + ((this.ForeColor != null) ? this.ForeColor : "@ForeColor@ ") + ";")
.Replace("@required@", (this.Required && !this.Hidden ? " required" : string.Empty))
.Replace("@placeHolder@", "placeholder=''");
        }
        //@"<div style='display: flex;'>
        //  <img id='@ebsid@_usrimg'class='sysctrl_usrimg' src='' alt='' onerror=""this.onerror=null; this.src='/images/nulldp.png';"">
        //  <div id='@ebsid@' data-ebtype='@data-ebtype@'  data-toggle='tooltip' title='@toolTipText@' class=' sysctrl_usrname'  name='@name@' autocomplete = 'off' @value@ @tabIndex@ style='width:100%; @BackColor@ @ForeColor@ display:inline-block; @fontStyle@' @required@ @placeHolder@ disabled></div>
        //</div>"


        public override string GetDesignHtml()
        {
            return GetHtml().RemoveCR().GraveAccentQuoted();
        }

        public override string GetHtml()
        {
            string EbCtrlHTML = HtmlConstants.CONTROL_WRAPER_HTML4WEB
               .Replace("@LabelForeColor ", "color:" + (LabelForeColor ?? "@LabelForeColor ") + ";")
               .Replace("@LabelBackColor ", "background-color:" + (LabelBackColor ?? "@LabelBackColor ") + ";");

            return ReplacePropsInHTML(EbCtrlHTML);
        }

        public string TableName { get; set; }

        public bool AddLocConstraint { get; set; }

        public IEnumerable<UsrLocFieldAbstract> PersistingFields
        {
            get
            {
                return this.Fields.Where(f => !string.IsNullOrEmpty((f as UsrLocField).ControlName));
            }
        }

        public string GetSelectQuery(IDatabase DataDB, string masterTbl)
        {
            //if multiple user ctrl placed in form then one select query is enough // imp
            return $@"SELECT u.id, u.email, u.fullname, u.nickname, u.dob, u.sex, u.alternateemail, u.phnoprimary AS phprimary, u.preferencesjson AS preference, eb_user_types_id AS usertype, u.statusid, u.forcepwreset,
                            STRING_AGG(r2u.role_id::TEXT, ',') AS roles, STRING_AGG(g2u.groupid::TEXT, ',') AS usergroups
                        FROM eb_users u LEFT JOIN eb_role2user r2u ON u.id = r2u.user_id LEFT JOIN eb_user2usergroup g2u ON u.id = g2u.userid
                        WHERE eb_ver_id = @{masterTbl}_eb_ver_id AND eb_data_id = @{masterTbl}_id GROUP BY u.id; ";
        }

        public string GetMappedUserQuery(string MasterTable, string eb_del, string false_val)
        {
            string idCol = this.TableName == MasterTable ? "id" : MasterTable + "_id";
            return $@"SELECT B.id, B.fullname, B.email, B.phnoprimary AS phprimary
                FROM {this.TableName} A, eb_users B
                WHERE B.id = A.{this.Name} AND A.{idCol} = @{MasterTable}_id AND COALESCE(A.{eb_del}, {false_val}) = {false_val}; ";
        }

        //insOnUp - user create on update
        private string GetSaveQuery(bool ins, string param, string mtbl, string pemail, string pphone, bool insOnUp)
        {
            if (ins)
            {
                string consqry = string.Empty;
                if (this.AddLocConstraint)
                    consqry = "SELECT * FROM eb_security_constraints(@eb_createdby, eb_currval('eb_users_id_seq'), '1$no description$1;5;' || eb_currval('eb_locations_id_seq'), '');";
                string ee = $"UPDATE {this.TableName} SET {this.Name} = eb_currval('eb_users_id_seq') WHERE {(this.TableName == mtbl ? "id" : (mtbl + "_id"))} = eb_currval('{mtbl}_id_seq'); ";
                return $"SELECT * FROM eb_security_user(@eb_createdby, {param}); UPDATE eb_users SET eb_ver_id = @{mtbl}_eb_ver_id, eb_data_id = eb_currval('{mtbl}_id_seq') WHERE {(pphone == string.Empty ? "email" : "phnoprimary")} = {(pphone == string.Empty ? pemail : pphone)};" + ee + consqry;
            }
            else
            {
                string ee = insOnUp ? $"UPDATE eb_users SET eb_ver_id = @{mtbl}_eb_ver_id, eb_data_id = @{mtbl}_id WHERE {(pphone == string.Empty ? "email" : "phnoprimary")} = {(pphone == string.Empty ? pemail : pphone)};" : string.Empty;
                ee += insOnUp ? $"UPDATE {this.TableName} SET {this.Name} = eb_currval('eb_users_id_seq') WHERE {(this.TableName == mtbl ? "id" : (mtbl + "_id"))} = @{mtbl}_id; " : string.Empty;
                return string.Format("SELECT * FROM eb_security_user(@eb_createdby, {0});", param) + ee;
            }

        }

        private bool ContainsKey(Dictionary<string, string> _d, string key)
        {
            return _d.ContainsKey(key) && _d[key] != string.Empty;
        }

        private int GetUserIdByEmailOrPhone(IDatabase DataDB, Dictionary<string, string> _d, ref int flag, bool ins, SingleColumn ocF)
        {
            int userId = 0;
            string _s = "SELECT id FROM eb_users WHERE LOWER(#) LIKE LOWER(@#) AND eb_del = 'F' AND (statusid = 0 OR statusid = 1 OR statusid = 2 OR statusid = 4);";
            string sql;
            List<DbParameter> parameters = new List<DbParameter>();
            if (ContainsKey(_d, "email"))//01
            {
                sql = _s.Replace("#", "email");
                parameters.Add(DataDB.GetNewParameter("email", EbDbTypes.String, _d["email"]));
            }
            else
                sql = "SELECT 1 WHERE 1 = 0; ";
            if (ContainsKey(_d, "phprimary"))//10
            {
                sql += _s.Replace("#", "phnoprimary");
                parameters.Add(DataDB.GetNewParameter("phnoprimary", EbDbTypes.String, _d["phprimary"]));
            }
            else
                sql += "SELECT 1 WHERE 1 = 0; ";

            EbDataSet ds = DataDB.DoQueries(sql, parameters.ToArray());

            int oProvUserId = ocF == null ? 0 : Convert.ToInt32(ocF.Value);
            //Dictionary<string, string> _od = ocF == null ? new Dictionary<string, string>() : JsonConvert.DeserializeObject<Dictionary<string, string>>(Convert.ToString(ocF.F));
            //int oCreUserId = ContainsKey(_od, "id") ? Convert.ToInt32(_od["id"]) : 0;

            if (ds.Tables[0].Rows.Count > 0)
            {
                userId = Convert.ToInt32(ds.Tables[0].Rows[0][0]);
                flag = 1;
            }
            if (ds.Tables[1].Rows.Count > 0)
            {
                int userId2 = Convert.ToInt32(ds.Tables[1].Rows[0][0]);
                flag |= 2;
                if (flag == 3)
                {
                    if (userId != userId2)
                    {
                        if (!ins)
                        {
                            if (userId == oProvUserId && oProvUserId > 0)
                            {
                                _d.Remove("phprimary");
                                flag &= 1;
                            }
                            if (userId2 == oProvUserId && oProvUserId > 0)
                            {
                                _d.Remove("email");
                                flag &= 2;
                                userId = oProvUserId;
                            }
                        }
                        if (userId != userId2 && flag == 3)
                            throw new FormException($"Unable to continue with {_d["email"]} and {_d["phprimary"]}", (int)HttpStatusCode.BadRequest, $"Email and Phone already exists for different users: {_d["email"]}, {_d["phprimary"]}", "EbProvisionUser => GetUserIdByEmailOrPhone");
                    }
                }
                else if (_d.ContainsKey("email") && _d["email"] != string.Empty && oProvUserId != userId2)
                {
                    _d.Remove("phprimary");
                    flag &= 1;
                    return 0;
                }
                else
                    userId = userId2;
            }
            else if (flag == 1 && _d.ContainsKey("phprimary") && _d["phprimary"] != string.Empty && oProvUserId != userId)
            {
                _d.Remove("email");
                flag &= 2;
                return 0;
            }
            return userId;
        }

        private void ThrowExistingUserException(Dictionary<string, string> _d, int flag)
        {
            if (_d.ContainsKey("email") && _d["email"] != string.Empty && _d.ContainsKey("phprimary") && _d["phprimary"] != string.Empty && flag == 3)
                throw new FormException($"{_d["email"]} and {_d["phprimary"]} already exists", (int)HttpStatusCode.BadRequest, $"Email and Phone already exists : {_d["email"]}, {_d["phprimary"]}", "EbProvisionUser => ParameterizeControl");
            if (_d.ContainsKey("email") && _d["email"] != string.Empty && flag == 1)
                throw new FormException($"{_d["email"]} already exists", (int)HttpStatusCode.BadRequest, $"Email already exists : {_d["email"]}", "EbProvisionUser => ParameterizeControl");
            if (_d.ContainsKey("phprimary") && _d["phprimary"] != string.Empty && flag == 2)
                throw new FormException($"{_d["phprimary"]} already exists", (int)HttpStatusCode.BadRequest, $"Phone number already exists : {_d["phprimary"]}", "EbProvisionUser => ParameterizeControl");
        }

        private void AddOrChange(Dictionary<string, string> _d, string Key, string Value)
        {
            if (_d.ContainsKey(Key))
                _d[Key] = Value;
            else
                _d.Add(Key, Value);
        }

        public void SetUserType_Role_Status(Dictionary<string, string> _d, int nProvUserId)
        {
            this.UserCredentials = new UserCredentials()
            {
                Email = _d.ContainsKey(FormConstants.email) ? _d[FormConstants.email] : string.Empty,
                Phone = _d.ContainsKey(FormConstants.phprimary) ? _d[FormConstants.phprimary] : string.Empty,
                Pwd = this.GetRandomPwd(),
                Name = _d.ContainsKey(FormConstants.fullname) ? _d[FormConstants.fullname] : (_d.ContainsKey(FormConstants.email) ? _d[FormConstants.email] : _d[FormConstants.phprimary]),
                UserId = nProvUserId
            };
            this.AddOrChange(_d, FormConstants.pwd, this.UserCredentials.Pwd);/*(this.UserCredentials.Pwd + this.UserCredentials.Email).ToMD5Hash());*/

            if (!_d.ContainsKey(FormConstants.usertype))
            {
                List<EbUserType> u_types = this.UserTypeToRole.FindAll(e => e.bVisible);
                if (u_types.Count == 1)
                    _d.Add(FormConstants.usertype, Convert.ToString(u_types[0].iValue));
            }

            if (_d.ContainsKey(FormConstants.usertype))
            {
                int u_type = Convert.ToInt32(_d[FormConstants.usertype]);
                EbUserType ebTyp = this.UserTypeToRole.Find(e => e.iValue == u_type && e.bVisible);
                if (ebTyp != null)
                {
                    if (ebTyp.ApprovalRequired)
                        this.AddOrChange(_d, FormConstants.statusid, ((int)EbUserStatus.Unapproved).ToString());
                    else if (ebTyp.Roles != null && ebTyp.Roles.Count > 0)
                        this.AddOrChange(_d, FormConstants.roles, string.Join(CharConstants.COMMA, ebTyp.Roles));
                }
            }
        }

        public override bool ParameterizeControl(ParameterizeCtrl_Params args, string crudContext)
        {
            if (!this.CreateOnlyIf_b)
                return false;
            string c = string.Empty;
            bool doNotUpdate = false;
            bool insertOnUpdate = false;
            Dictionary<string, string> _d = JsonConvert.DeserializeObject<Dictionary<string, string>>(Convert.ToString(args.cField.F));
            int nProvUserId = 0;
            int flag = 0;
            if ((_d.ContainsKey(FormConstants.email) && _d[FormConstants.email].Trim() != string.Empty) || (_d.ContainsKey(FormConstants.phprimary) && _d[FormConstants.phprimary].Trim() != string.Empty))
                nProvUserId = this.GetUserIdByEmailOrPhone(args.DataDB, _d, ref flag, args.ins, args.ocF);
            else
                return false;
            if (args.ins)
            {
                if (nProvUserId > 0)// user already exists
                {
                    if (!this.AllowExistingUser)
                        this.ThrowExistingUserException(_d, flag);

                    args._cols += args.cField.Name + CharConstants.COMMA + CharConstants.SPACE;
                    args._vals += CharConstants.AT + args.cField.Name + CharConstants.UNDERSCORE + args.i + CharConstants.COMMA + CharConstants.SPACE;
                    args.param.Add(args.DataDB.GetNewParameter(args.cField.Name + CharConstants.UNDERSCORE + args.i, (EbDbTypes)args.cField.Type, nProvUserId));
                    args.i++;
                    doNotUpdate = true;
                }
                else
                    this.SetUserType_Role_Status(_d, nProvUserId);
            }
            else
            {
                int oProvUserId = Convert.ToInt32(args.ocF.Value);
                Dictionary<string, string> _od = JsonConvert.DeserializeObject<Dictionary<string, string>>(Convert.ToString(args.ocF.F));
                if (_od.ContainsKey(FormConstants.id) && (oProvUserId == nProvUserId))// means user created by this control
                {
                    this.AddOrChange(_d, FormConstants.id, _od[FormConstants.id]);
                    int oCreUserId = Convert.ToInt32(_od[FormConstants.id]);
                    if (oCreUserId != nProvUserId || !_d.ContainsKey(FormConstants.email))
                        this.AddOrChange(_d, FormConstants.email, _od[FormConstants.email]);// remove this line if you want to edit email via prov user ctrl
                    if (oCreUserId != nProvUserId || !_d.ContainsKey(FormConstants.phprimary))
                        this.AddOrChange(_d, FormConstants.phprimary, _od[FormConstants.phprimary]);
                    this.AddOrChange(_d, FormConstants.usertype, _od[FormConstants.usertype]);
                    int oldStatus = Convert.ToInt32(_od[FormConstants.statusid]);
                    this.AddOrChange(_d, FormConstants.statusid, Convert.ToString(oldStatus + 100));

                    if (oldStatus == (int)EbUserStatus.Unapproved)
                    {
                        if (args.usr.Roles.Contains(SystemRoles.SolutionOwner.ToString()) || args.usr.Roles.Contains(SystemRoles.SolutionAdmin.ToString()))
                        {
                            int u_type = Convert.ToInt32(_od[FormConstants.usertype]);
                            EbUserType ebTyp = this.UserTypeToRole.Find(e => e.iValue == u_type && e.bVisible);
                            if (ebTyp != null && ebTyp.Roles != null && ebTyp.Roles.Count > 0)
                            {
                                this.AddOrChange(_d, FormConstants.statusid, ((int)EbUserStatus.Active).ToString());
                                this.AddOrChange(_d, FormConstants.roles, string.Join(CharConstants.COMMA, ebTyp.Roles));
                            }
                        }
                    }

                    foreach (KeyValuePair<string, string> item in _od)
                    {
                        if (!_d.ContainsKey(item.Key))
                            _d.Add(item.Key, item.Value);
                    }
                }
                else
                {
                    doNotUpdate = true;
                    if (nProvUserId > 1 && oProvUserId != nProvUserId)
                    {
                        if (!this.AllowExistingUser)
                            this.ThrowExistingUserException(_d, flag);

                        args._colvals += string.Concat(args.cField.Name, CharConstants.EQUALS, CharConstants.AT, args.cField.Name, CharConstants.UNDERSCORE, args.i, CharConstants.COMMA, CharConstants.SPACE);
                        args.param.Add(args.DataDB.GetNewParameter(args.cField.Name + CharConstants.UNDERSCORE + args.i, (EbDbTypes)args.cField.Type, nProvUserId));
                        args.i++;
                    }
                    else if (nProvUserId <= 0)
                    {
                        insertOnUpdate = true;
                        this.SetUserType_Role_Status(_d, nProvUserId);
                    }
                }
            }
            if (!doNotUpdate || insertOnUpdate)
            {
                if (_d.ContainsKey(FormConstants.consadd))
                {
                    if (nProvUserId <= 0)
                    {
                        if (_d[FormConstants.consadd].Length > 0)
                        {
                            string[] loc_ids = _d[FormConstants.consadd].Split(CharConstants.COMMA);
                            int h = 0;
                            for (; h < loc_ids.Length; h++)
                            {
                                if (!int.TryParse(loc_ids[h], out int temp) || temp <= 0)
                                    break;
                            }
                            if (h >= loc_ids.Length)
                            {
                                EbConstraints consObj = new EbConstraints(loc_ids, EbConstraintKeyTypes.User, EbConstraintTypes.User_Location);
                                _d[FormConstants.consadd] = consObj.GetDataAsString();
                            }
                        }
                    }
                    else
                        _d[FormConstants.consadd] = string.Empty;
                }

                string p_email = string.Empty, p_phone = string.Empty;
                for (int k = 0; k < this.FuncParam.Length; k++, args.i++)
                {
                    object _value = this.FuncParam[k].Value;
                    if (_d.ContainsKey(this.FuncParam[k].Name) && !string.IsNullOrEmpty(_d[this.FuncParam[k].Name]))
                    {
                        _value = _d[this.FuncParam[k].Name];
                        if (this.FuncParam[k].Name.Equals(FormConstants.email))
                            p_email = string.Concat(CharConstants.COLON, this.FuncParam[k].Name, CharConstants.UNDERSCORE, args.i);
                        if (this.FuncParam[k].Name.Equals(FormConstants.phprimary))
                            p_phone = string.Concat(CharConstants.COLON, this.FuncParam[k].Name, CharConstants.UNDERSCORE, args.i);
                    }
                    c += string.Concat(CharConstants.COLON, this.FuncParam[k].Name, CharConstants.UNDERSCORE, args.i, CharConstants.COMMA, CharConstants.SPACE);
                    if (_value == DBNull.Value)
                    {
                        var p = args.DataDB.GetNewParameter(this.FuncParam[k].Name + CharConstants.UNDERSCORE + args.i, this.FuncParam[k].Type);
                        p.Value = _value;
                        args.param.Add(p);
                    }
                    else
                    {
                        args.param.Add(args.DataDB.GetNewParameter(this.FuncParam[k].Name + CharConstants.UNDERSCORE + args.i, this.FuncParam[k].Type, _value));
                    }
                }

                args._extqry += this.GetSaveQuery(args.ins, c.Substring(0, c.Length - 2), args.tbl, p_email, p_phone, insertOnUpdate);
            }
            return true;
        }

        public override SingleColumn GetSingleColumn(User UserObj, Eb_Solution SoluObj, object Value, bool Default)
        {
            if (Value != null && int.TryParse(Convert.ToString(Value), out int _t))
                Value = _t;
            return new SingleColumn()
            {
                Name = this.Name,
                Type = (int)this.EbDbType,
                Value = Value ?? 0,
                F = "{}",
                Control = this,
                ObjType = this.ObjType
            };
        }

        private string GetRandomPwd()
        {
            string validChars = "ABCDEFGHJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*?_-";
            StringBuilder builder = new StringBuilder();
            Random random = new Random();
            char ch;
            for (int i = 0; i < 10; i++)
            {
                ch = validChars[Convert.ToInt32(Math.Floor(72 * random.NextDouble()))];
                builder.Append(ch);
            }
            return builder.ToString();
        }

        public UserCredentials UserCredentials { get; set; }

        private string MailHtml
        {
            get
            {
                return @"
<html>
    <head>
        <title></title>
    </head>
    <body>
        <div style='border: 1px solid #508bf9;padding:20px 40px 20px 40px; '>
            <div style='text-align: center;'>
                <img src='https://myaccount.expressbase.com/images/logo/{iSolutionId}.png' style='max-height: 100px; max-width: 300px; ' />
            </div>
            <br />
            <div style='line-height: 1.4;'>
                Dear {UserName},<br />
                <br />
                You have been added as a user into {SolutionName} solution. Please find below credentials to log in.
                <br />
                <br />
                Solution URL - https://{eSolutionId}.expressbase.com <br />
                User name - {Email} <br />
                Password - {Password} <br />
                <br />
                Please make sure you change the password after logging in (in the <b>My Profile</b> page).
            </div>
            <br />
            <br />
            Thanks,<br />
            {SolutionAdmin}<br />
        </div>
    </body>
</html>";
            }
            set { }
        }

        public void SendWelcomeMail(RabbitMqProducer MessageProducer3, User user, Eb_Solution solution)
        {
            string Html = this.MailHtml
                .Replace("{SolutionName}", solution.SolutionName)
                .Replace("{eSolutionId}", solution.ExtSolutionID)
                .Replace("{iSolutionId}", solution.SolutionID)
                .Replace("{UserName}", this.UserCredentials.Name)
                .Replace("{Email}", this.UserCredentials.Email)
                .Replace("{Password}", this.UserCredentials.Pwd)
                .Replace("{SolutionAdmin}", string.IsNullOrEmpty(user.FullName) ? $"{solution.SolutionName} Team" : user.FullName);

            //this.EbConnectionFactory.EmailConnection.Send("febincarlos@expressbase.com", "test", "Hiii", null, null, null, "");

            MessageProducer3.Publish(new EmailServicesRequest()
            {
                To = this.UserCredentials.Email,
                Message = Html,
                Subject = $"Welcome to {solution.SolutionName} Solution",
                UserId = user.UserId,
                UserAuthId = user.AuthId,
                SolnId = solution.SolutionID
            });
        }

        public bool IsUserCreated()
        {
            return this.UserCredentials != null;
        }

        public override string GetValueFromDOMJSfn
        {
            get
            {
                return @"return this.DataVals.Value;";
            }
            set { }
        }
        //debugger;
        //let updateDataVals = true;
        //if (!this.hasOwnProperty('_finalObj'))
        //    this._finalObj = {};
        //$.each(this.Fields.$values, function (i, obj) {
        //    if (obj.ControlName !== '') {
        //        if (obj.Control.___DoNotUpdateDataVals){
        //            updateDataVals = false;
        //            return false;
        //        }
        //        this._finalObj[obj.Name] = obj.Control.getValueFromDOM();
        //    }            
        //}.bind(this));
        //if (updateDataVals)
        //    this.DataVals.F = JSON.stringify(this._finalObj); 

        public override string SetValueJSfn
        {
            get
            {
                return @"";
            }
            set { }
        }
        //debugger;
        //if (!this.DataVals)
        //    return;
        //this._finalObj = JSON.parse(this.DataVals.F);
        //$.each(this.Fields.$values, function (i, obj) {
        //    if (obj.ControlName !== '' && obj.Control.DoNotPersist && !(obj.Control.ValueExpr && obj.Control.ValueExpr.Code) && p1 > 0) {
        //        if (this._finalObj.hasOwnProperty(obj.Name))
        //            obj.Control.justSetValue(this._finalObj[obj.Name]);
        //    }
        //}.bind(this));

        public override string OnChangeBindJSFn
        {
            get
            {
                return @"";
            }
            set { }
        }
        //debugger;
        //$.each(this.Fields.$values, function (i, obj) {
        //    if (obj.ControlName !== '') {
        //        $('#' + obj.Control.EbSid_CtxId).on('change', p1);
        //    }
        //}.bind(this));

        public override string GetDisplayMemberFromDOMJSfn { get { return @"return '';"; } set { } }

        public override string RefreshJSfn { get { return @""; } set { } }

        public override string ClearJSfn { get { return @""; } set { } }

        public override string DisableJSfn { get { return JSFnsConstants.Ctrl_DisableJSfn + "$('#' + this.EbSid_CtxId + '_usrimg').css('pointer-events', 'auto');"; } set { } }

    }

    public class UserCredentials
    {
        public UserCredentials() { }

        public int UserId { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public string Pwd { get; set; }

        public string Name { get; set; }
    }

    public abstract class UsrLocFieldAbstract { }

    [UsedWithTopObjectParent(typeof(EbObject))]
    [EnableInBuilder(BuilderType.WebForm)]
    [Alias("ControlField")]
    public class UsrLocField : UsrLocFieldAbstract
    {
        public UsrLocField() { }

        [EnableInBuilder(BuilderType.WebForm)]
        [HideInPropertyGrid]
        public string Name { get; set; }

        [EnableInBuilder(BuilderType.WebForm)]
        [HideInPropertyGrid]
        public string DisplayName { get; set; }

        [EnableInBuilder(BuilderType.WebForm)]
        public string ControlName { get; set; }

        [EnableInBuilder(BuilderType.WebForm)]
        [HideInPropertyGrid]
        public bool IsRequired { get; set; }

        [EnableInBuilder(BuilderType.WebForm)]
        [HideInPropertyGrid]
        public string Type { get; set; }

        //[EnableInBuilder(BuilderType.WebForm)]//
        //[HideInPropertyGrid]//
        //public EbControl Control { get; set; }
    }

    [UsedWithTopObjectParent(typeof(EbObject))]
    [EnableInBuilder(BuilderType.WebForm)]
    [Alias("ControlField")]
    public class EbUserType
    {
        public EbUserType() { }

        [EnableInBuilder(BuilderType.WebForm)]
        [HideInPropertyGrid]
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [EnableInBuilder(BuilderType.WebForm)]
        [HideInPropertyGrid]
        public string EbSid { get; set; }

        [EnableInBuilder(BuilderType.WebForm)]
        [HideInPropertyGrid]
        public int iValue { get; set; }

        [EnableInBuilder(BuilderType.WebForm)]
        [HideInPropertyGrid]
        public string sValue { get; set; }

        [EnableInBuilder(BuilderType.WebForm)]
        [HideInPropertyGrid]
        public bool bVisible { get; set; }

        [EnableInBuilder(BuilderType.WebForm)]
        [Unique]
        [PropDataSourceJsFn("return ebcontext.Roles")]
        [PropertyEditor(PropertyEditorType.DropDown, true)]
        public List<Int32> Roles { get; set; }

        [EnableInBuilder(BuilderType.WebForm)]
        public bool ApprovalRequired { get; set; }

    }
}
