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

namespace ExpressBase.Objects
{
    [EnableInBuilder(BuilderType.WebForm)]
    public class EbProvisionUser : EbControlUI, IEbPlaceHolderControl
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
        [HideInPropertyGrid]
        public override bool Hidden { get { return true; } }

        [EnableInBuilder(BuilderType.WebForm)]
        [HideInPropertyGrid]
        public override bool IsSysControl { get { return false; } }

        [EnableInBuilder(BuilderType.WebForm)]
        [PropertyGroup(PGConstants.CORE)]
        [UIproperty]
        [OnChangeUIFunction("EbProvisionUser.mapping")]
        [PropertyEditor(PropertyEditorType.Collection)]
        [ListType(typeof(UsrLocFieldAbstract))]
        public List<UsrLocFieldAbstract> Fields { get; set; }

        [EnableInBuilder(BuilderType.WebForm)]
        [HideInPropertyGrid]
        public override EbDbTypes EbDbType { get { return EbDbTypes.String; } }

        [EnableInBuilder(BuilderType.WebForm)]
        [PropertyEditor(PropertyEditorType.CollectionProp, "UserTypeToRole", "bVisible")]
        [OnChangeExec(@"if (this.UserTypeToRole && this.UserTypeToRole.$values.length === 0 ){
            $.each(ebcontext.UserTypes, function(i, o){
                 this.UserTypeToRole.$values.push($.extend(new EbObjects.EbUserType, {iValue: i, sValue: o, name: o, EbSid: 'usrTyp_' + Date.now().toString(36).substring(3) + i}));
            }.bind(this));            
        }")]
        public List<EbUserType> UserTypeToRole { get; set; }

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
        public override EbScript VisibleExpr { get; set; }

        [EnableInBuilder(BuilderType.WebForm)]
        [HideInPropertyGrid]
        public override EbScript ValueExpr { get; set; }

        [EnableInBuilder(BuilderType.WebForm)]
        [HideInPropertyGrid]
        public override bool IsDisable { get; set; }

        [EnableInBuilder(BuilderType.WebForm)]
        [HideInPropertyGrid]
        public override bool Required { get; set; }

        [EnableInBuilder(BuilderType.WebForm)]
        [HideInPropertyGrid]
        public override bool DoNotPersist { get { return true; } }

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
            new NTV (){ Name = "consdel", Type = EbDbTypes.String, Value = string.Empty}
        };

        public override string GetJsInitFunc()
        {
            return @"
this.Init = function(id)
{
    let efield = new EbObjects.UsrLocField('email');
    efield.DisplayName = 'Email';
    efield.IsRequired = true;
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
	//this.Fields.$values.push(new EbObjects.UsrLocField('consadd'));
};";
        }

        public override string GetBareHtml()
        {
            return @"
            <input id='@ebsid@' data-ebtype='@data-ebtype@'  data-toggle='tooltip' title='@toolTipText@' class='date' type='text' name='@name@' autocomplete = 'off' @value@ @tabIndex@ style='width:100%; @BackColor@ @ForeColor@ display:inline-block; @fontStyle@' @required@ @placeHolder@ disabled />
            "
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

        public override string GetDesignHtml()
        {
            return GetHtml().RemoveCR().DoubleQuoted();
        }

        public override string GetHtml()
        {
            string EbCtrlHTML = HtmlConstants.CONTROL_WRAPER_HTML4WEB
               .Replace("@LabelForeColor ", "color:" + (LabelForeColor ?? "@LabelForeColor ") + ";")
               .Replace("@LabelBackColor ", "background-color:" + (LabelBackColor ?? "@LabelBackColor ") + ";");

            return ReplacePropsInHTML(EbCtrlHTML);
        }

        public string VirtualTable { get; set; }

        public bool AddLocConstraint { get; set; }

        public IEnumerable<UsrLocFieldAbstract> PersistingFields
        {
            get
            {
                return this.Fields.Where(f => !string.IsNullOrEmpty((f as UsrLocField).ControlName));
            }
        }

        public string GetSelectQuery(string masterTbl)
        {
            //if multiple user ctrl placed in form then one select query is enough // imp
            return $@"SELECT u.id, u.email, u.fullname, u.nickname, u.dob, u.sex, u.alternateemail, u.phnoprimary AS phprimary, u.preferencesjson AS preference, eb_user_types_id AS usertype, u.statusid,
                            STRING_AGG(r2u.role_id::TEXT, ',') AS roles, STRING_AGG(g2u.groupid::TEXT, ',') AS usergroups
                        FROM eb_users u LEFT JOIN eb_role2user r2u ON u.id = r2u.user_id LEFT JOIN eb_user2usergroup g2u ON u.id = g2u.userid
                        WHERE eb_ver_id = @{masterTbl}_eb_ver_id AND eb_data_id = @{masterTbl}_id GROUP BY u.id";
        }

        private string GetSaveQuery(bool ins, string param, string mtbl, string pemail)
        {
            if (ins)
            {
                string consqry = string.Empty;
                if (this.AddLocConstraint)
                    consqry = "SELECT * FROM eb_security_constraints(:eb_createdby, eb_currval('eb_users_id_seq'), '1$no description$1;5;' || eb_currval('eb_locations_id_seq'), '');";
                return string.Format("SELECT * FROM eb_security_user(:eb_createdby, {0}); UPDATE eb_users SET eb_ver_id = :{1}_eb_ver_id, eb_data_id = eb_currval('{1}_id_seq') WHERE email = {2};", param, mtbl, pemail) + consqry;
            }
            else
                return string.Format("SELECT * FROM eb_security_user(:eb_createdby, {0});", param);
        }

        public override bool ParameterizeControl(IDatabase DataDB, List<DbParameter> param, string tbl, SingleColumn cField, bool ins, ref int i, ref string _col, ref string _val, ref string _extqry, User usr, SingleColumn ocF)
        {
            string c = string.Empty;
            string ep = string.Empty;
            Dictionary<string, string> _d = JsonConvert.DeserializeObject<Dictionary<string, string>>(Convert.ToString(cField.Value));
            if (ins)
            {
                string sql = "SELECT id FROM eb_users WHERE LOWER(email) LIKE LOWER(:email) AND eb_del = 'F' AND (statusid = 0 OR statusid = 1 OR statusid = 2);";
                DbParameter[] parameters = new DbParameter[] { DataDB.GetNewParameter("email", EbDbTypes.String, _d["email"]) };
                EbDataTable dt = DataDB.DoQuery(sql, parameters);
                if (dt.Rows.Count > 0)
                {
                    throw new FormException($"{_d["email"]} already exists", (int)HttpStatusCodes.BAD_REQUEST, $"Email already exists : {_d["email"]}", "EbProvisionUser => ParameterizeControl");
                }

                this.UserCredentials = new UserCredentials()
                {
                    Email = _d["email"],
                    Pwd = this.GetRandomPwd(),
                    Name = _d.ContainsKey("fullname") ? _d["fullname"] : _d["email"]
                };
                _d.Add("pwd", (this.UserCredentials.Pwd + this.UserCredentials.Email).ToMD5Hash());

                int u_type = Convert.ToInt32(_d["usertype"]);
                EbUserType ebTyp = this.UserTypeToRole.Find(e => e.iValue == u_type && e.bVisible);
                if (ebTyp != null)
                {
                    if (ebTyp.ApprovalRequired)
                        _d["statusid"] = ((int)EbUserStatus.Unapproved).ToString();
                    else if (ebTyp.Roles != null && ebTyp.Roles.Count > 0)
                        _d["roles"] = string.Join(',', ebTyp.Roles);
                }
            }
            else
            {
                Dictionary<string, string> _od = JsonConvert.DeserializeObject<Dictionary<string, string>>(Convert.ToString(ocF.Value));
                _d["id"] = _od["id"];
                _d["email"] = _od["email"];
                _d["usertype"] = _od["usertype"];
                int oldStatus = Convert.ToInt32(_od["statusid"]);
                _d["statusid"] = Convert.ToString(oldStatus + 100);

                if (oldStatus == (int)EbUserStatus.Unapproved)
                {
                    if (usr.Roles.Contains(SystemRoles.SolutionOwner.ToString()) || usr.Roles.Contains(SystemRoles.SolutionAdmin.ToString()))
                    {
                        int u_type = Convert.ToInt32(_od["usertype"]);
                        EbUserType ebTyp = this.UserTypeToRole.Find(e => e.iValue == u_type && e.bVisible);
                        if (ebTyp != null && ebTyp.Roles != null && ebTyp.Roles.Count > 0)
                        {
                            _d["roles"] = string.Join(',', ebTyp.Roles);
                            _d["statusid"] = ((int)EbUserStatus.Active).ToString();
                        }
                    }
                }

                foreach (KeyValuePair<string, string> item in _od)
                {
                    if (!_d.ContainsKey(item.Key))
                        _d[item.Key] = item.Value;
                }
            }
            for(int k = 0; k < this.FuncParam.Length; k++, i++)
            {
                if (_d.ContainsKey(this.FuncParam[k].Name))
                {
                    this.FuncParam[k].Value = _d[this.FuncParam[k].Name];
                }
                c += string.Concat(":", this.FuncParam[k].Name, "_", i, ", ");
                if(this.FuncParam[k].Value == DBNull.Value)
                {
                    var p = DataDB.GetNewParameter(this.FuncParam[k].Name + "_" + i, this.FuncParam[k].Type);
                    p.Value = this.FuncParam[k].Value;
                    param.Add(p);
                }
                else
                {
                    param.Add(DataDB.GetNewParameter(this.FuncParam[k].Name + "_" + i, this.FuncParam[k].Type, this.FuncParam[k].Value));
                }
                
                if (this.FuncParam[k].Name.Equals("email"))
                    ep = string.Concat(":", this.FuncParam[k].Name, "_", i);
            }

            _extqry += this.GetSaveQuery(ins, c.Substring(0, c.Length - 2), tbl, ep);
            
            return true;
        }

        public override SingleColumn GetSingleColumn(User UserObj, Eb_Solution SoluObj, object Value)
        {
            return new SingleColumn()
            {
                Name = this.Name,
                Type = (int)this.EbDbType,
                Value = "{}",
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

        private UserCredentials UserCredentials { get; set; }

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

        public void SendWelcomeMail(RabbitMqProducer MessageProducer3,User user,Eb_Solution solution)
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
                return @"
                if (!this.hasOwnProperty('_finalObj'))
                    this._finalObj = {};
                $.each(this.Fields.$values, function (i, obj) {
                    if (obj.ControlName !== '') {
                        this._finalObj[obj.Name] = obj.Control.getValueFromDOM();
                    }            
                }.bind(this));
                return JSON.stringify(this._finalObj);";
            }
            set { }
        }

        public override string SetValueJSfn
        {
            get
            {
                return @"
                this._finalObj = JSON.parse(p1);
                $.each(this.Fields.$values, function (i, obj) {
                    if (obj.ControlName !== '') {
                        obj.Control.setValue(this._finalObj[obj.Name]);
                    }
                }.bind(this));";
            }
            set { }
        }

        public override string OnChangeBindJSFn 
        { 
            get 
            {
                return @"
                    $.each(this.Fields.$values, function (i, obj) {
                        if (obj.ControlName !== '') {
                            $('#' + obj.Control.EbSid_CtxId).on('change', p1);
                        }
                    }.bind(this));";
            } 
            set { } 
        }

        public override string RefreshJSfn { get { return @""; } set { } }

        public override string ClearJSfn { get { return @""; } set { } }
        
    }

    public class UserCredentials
    {
        public UserCredentials() { }
        
        public string Email { get; set; }

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

        [EnableInBuilder(BuilderType.WebForm)]
        [HideInPropertyGrid]
        public EbControl Control { get; set; }
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
