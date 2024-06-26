﻿using ExpressBase.Common;
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
using ServiceStack;
using System.Data;
using System.Threading;

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

        [EnableInBuilder(BuilderType.WebForm)]
        [HideInPropertyGrid]
        public override EbDbTypes EbDbType { get { return EbDbTypes.Int32; } }

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
        public bool BlockUserEditing { get; set; }

        [EnableInBuilder(BuilderType.WebForm)]
        [Alias("Send verification message")]
        public bool SendVerificationMsg { get; set; }

        [EnableInBuilder(BuilderType.WebForm)]
        public bool SendWelcomeMsg { get; set; }

        [EnableInBuilder(BuilderType.WebForm)]
        public override EbScript HiddenExpr { get; set; }

        [EnableInBuilder(BuilderType.WebForm)]
        [DefaultPropValue("150")]
        [PropertyGroup("Appearance")]
        public override int Height { get; set; }


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
	this.Fields.$values.push(new EbObjects.UsrLocField('statusid'));
	this.Fields.$values.push(new EbObjects.UsrLocField('primary_role'));
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

        private Dictionary<string, string> GetFormattedData(IDatabase DataDB, int userId)
        {
            string Qry = GetRefreshDataQuery(userId);
            try
            {
                EbDataTable dt = DataDB.DoQuery(Qry);
                SingleTable Table = new SingleTable();
                EbDataRow Row_U = dt.Rows.Count > 0 ? dt.Rows[0] : null;
                return this.GetFormattedData(Row_U);
            }
            catch (Exception ex)
            {
                throw new FormException("Failed to fetch info of the current user.", (int)HttpStatusCode.InternalServerError, ex.Message, "EbProvisionUser => GetFormattedData...");
            }
        }

        public Dictionary<string, string> GetFormattedData(EbDataRow Row_U)
        {
            Dictionary<string, string> _d = new Dictionary<string, string>();
            if (Row_U != null)
            {
                NTV[] pArr = this.FuncParam;
                for (int k = 0; k < pArr.Length; k++)
                {
                    if (Row_U[pArr[k].Name] != null)
                        _d.Add(pArr[k].Name, Row_U[pArr[k].Name].ToString());
                }
                if (!string.IsNullOrWhiteSpace(Row_U["consids"]?.ToString()) && !string.IsNullOrWhiteSpace(Row_U["consvals"]?.ToString()))
                {
                    int[] conIds = Row_U["consids"].ToString().Split(",").Select(e => int.TryParse(e, out int t) ? t : 0).ToArray();
                    int[] conVals = Row_U["consvals"].ToString().Split(",").Select(e => int.TryParse(e, out int t) ? t : 0).ToArray();
                    Dictionary<int, int> cons = new Dictionary<int, int>();
                    for (int x = 0; x < conIds.Length; x++)
                    {
                        if (!cons.ContainsKey(conIds[x]))
                            cons.Add(conIds[x], conVals[x]);
                    }
                    if (cons.Count > 0)
                        _d.Add(FormConstants.locConstraint, JsonConvert.SerializeObject(cons));
                }
                if (int.TryParse(Row_U["eb_ver_id"]?.ToString(), out int verid))
                {
                    _d.Add("eb_ver_id", verid.ToString());
                }
                if (int.TryParse(Row_U["eb_data_id"]?.ToString(), out int dataid))
                {
                    _d.Add("eb_data_id", dataid.ToString());
                }
                if (!string.IsNullOrWhiteSpace(Row_U["eb_is_mapped_user"]?.ToString()))
                {
                    _d.Add("eb_is_mapped_user", Row_U["eb_is_mapped_user"].ToString());
                }
            }

            return _d;
        }

        public Dictionary<string, object> GetFormattedData(SingleRow Row_U)
        {
            Dictionary<string, object> _d = new Dictionary<string, object>();
            if (Row_U != null)
            {
                NTV[] pArr = this.FuncParam;
                for (int k = 0; k < pArr.Length; k++)
                {
                    if (Row_U[pArr[k].Name] != null)
                        _d.Add(pArr[k].Name, Row_U[pArr[k].Name]);
                }
                if (!string.IsNullOrWhiteSpace(Row_U["consids"]?.ToString()) && !string.IsNullOrWhiteSpace(Row_U["consvals"]?.ToString()))
                {
                    int[] conIds = Row_U["consids"].ToString().Split(",").Select(e => int.TryParse(e, out int t) ? t : 0).ToArray();
                    int[] conVals = Row_U["consvals"].ToString().Split(",").Select(e => int.TryParse(e, out int t) ? t : 0).ToArray();
                    Dictionary<int, int> cons = new Dictionary<int, int>();
                    for (int x = 0; x < conIds.Length; x++)
                    {
                        if (!cons.ContainsKey(conIds[x]))
                            cons.Add(conIds[x], conVals[x]);
                    }
                    if (cons.Count > 0)
                        _d.Add(FormConstants.locConstraint, JsonConvert.SerializeObject(cons));
                }
                if (int.TryParse(Row_U["eb_ver_id"]?.ToString(), out int verid))
                {
                    _d.Add("eb_ver_id", verid);
                }
                if (int.TryParse(Row_U["eb_data_id"]?.ToString(), out int dataid))
                {
                    _d.Add("eb_data_id", dataid);
                }
                if (!string.IsNullOrWhiteSpace(Row_U["eb_is_mapped_user"]?.ToString()))
                {
                    _d.Add("eb_is_mapped_user", Row_U["eb_is_mapped_user"].ToString());
                }
            }

            return _d;
        }

        public string GetSelectQuery(IDatabase DataDB, string masterTbl, string form_ver_id, string form_ref_id)
        {
            return null;
        }

        public string GetRefreshDataQuery(int userId)
        {
            return $@"
SELECT u.id, u.email, u.fullname, u.nickname, u.dob, u.sex, u.alternateemail, u.phnoprimary AS phprimary, u.preferencesjson AS preference, 
    u.eb_user_types_id AS usertype, u.statusid, u.forcepwreset, u.eb_ver_id, u.eb_data_id, u.eb_is_mapped_user,
    STRING_AGG(r2u.role_id::TEXT, ',') AS roles, STRING_AGG(g2u.groupid::TEXT, ',') AS usergroups, 
    STRING_AGG(cons.id::TEXT, ',') AS consids, STRING_AGG(cons.c_value::TEXT, ',') AS consvals
FROM eb_users u 
LEFT JOIN eb_role2user r2u ON u.id = r2u.user_id AND r2u.eb_del='F'
LEFT JOIN eb_user2usergroup g2u ON u.id = g2u.userid AND g2u.eb_del='F'
LEFT JOIN 
(   SELECT m.id, m.key_id, l.c_value
    FROM eb_constraints_master m, eb_constraints_line l
    WHERE m.id = l.master_id AND m.key_type = {(int)EbConstraintKeyTypes.User} AND 
    l.c_type = {(int)EbConstraintTypes.User_Location} AND eb_del = 'F' ORDER BY m.id
) cons ON u.id = cons.key_id 
WHERE u.id = {userId} GROUP BY u.id;";
        }

        public string GetMappedUserQuery(string MasterTable, string eb_del, string false_val)
        {
            string idCol = this.TableName == MasterTable ? "id" : MasterTable + "_id";
            return $@"
SELECT u.id, u.email, u.fullname, u.nickname, u.dob, u.sex, u.alternateemail, u.phnoprimary AS phprimary, u.preferencesjson AS preference, 
    u.eb_user_types_id AS usertype, u.statusid, u.forcepwreset, u.eb_ver_id, u.eb_data_id, u.eb_is_mapped_user,
    STRING_AGG(r2u.role_id::TEXT, ',') AS roles, STRING_AGG(g2u.groupid::TEXT, ',') AS usergroups, 
    STRING_AGG(cons.id::TEXT, ',') AS consids, STRING_AGG(cons.c_value::TEXT, ',') AS consvals
FROM {this.TableName} A, eb_users u 
LEFT JOIN eb_role2user r2u ON u.id = r2u.user_id AND r2u.eb_del='F'
LEFT JOIN eb_user2usergroup g2u ON u.id = g2u.userid AND g2u.eb_del='F'
LEFT JOIN 
(   SELECT m.id, m.key_id, l.c_value
    FROM eb_constraints_master m, eb_constraints_line l
    WHERE m.id = l.master_id AND m.key_type = {(int)EbConstraintKeyTypes.User} AND 
    l.c_type = {(int)EbConstraintTypes.User_Location} AND eb_del = 'F' ORDER BY m.id
) cons ON u.id = cons.key_id 
WHERE u.id = A.{this.Name} AND A.{idCol} = @{MasterTable}_id AND COALESCE(A.{eb_del}, {false_val}) = {false_val} GROUP BY u.id;";

        }

        //insOnUp - user create on update
        private string GetSaveQuery(string param, string mtbl, bool isFormIns, int usrId, bool isMapped)
        {
            string userId_s = usrId > 1 ? usrId.ToString() : "eb_currval('eb_users_id_seq')";
            string mf = isMapped ? "T" : "F";
            if (isFormIns)
            {
                string consqry = string.Empty;
                if (this.AddLocConstraint)
                    consqry = $"SELECT * FROM eb_security_constraints(@eb_createdby, {userId_s}, '1$no description$1;5;' || eb_currval('eb_locations_id_seq'), '');";
                string ee = $"UPDATE {this.TableName} SET {this.Name} = {userId_s} WHERE {(this.TableName == mtbl ? "id" : (mtbl + "_id"))} = eb_currval('{mtbl}_id_seq'); ";
                return $"SELECT * FROM eb_security_user(@eb_createdby, {param}); UPDATE eb_users SET eb_ver_id = @{mtbl}_eb_ver_id, eb_data_id = eb_currval('{mtbl}_id_seq'), eb_is_mapped_user='{mf}' WHERE id={userId_s};" + ee + consqry;
            }
            else
            {
                string ee = $"UPDATE eb_users SET eb_ver_id = @{mtbl}_eb_ver_id, eb_data_id = @{mtbl}_id, eb_is_mapped_user='{mf}' WHERE id={userId_s};";
                ee += $"UPDATE {this.TableName} SET {this.Name} = {userId_s} WHERE {(this.TableName == mtbl ? "id" : (mtbl + "_id"))} = @{mtbl}_id; ";

                return string.Format("SELECT * FROM eb_security_user(@eb_createdby, {0});", param) + ee;
            }

        }

        private bool ContainsKey(Dictionary<string, string> _d, string key)
        {
            return _d.ContainsKey(key) && _d[key] != string.Empty;
        }

        private int GetUserIdByEmailOrPhone(IDatabase DataDB, Dictionary<string, string> _d, ref int flag, bool ins, SingleColumn ocF)
        {
            int emUserId = 0;
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

            if (ds.Tables[0].Rows.Count > 0) //email found
            {
                emUserId = Convert.ToInt32(ds.Tables[0].Rows[0][0]);
                flag = 1;
            }
            if (ds.Tables[1].Rows.Count > 0) //phone found
            {
                int phUserId = Convert.ToInt32(ds.Tables[1].Rows[0][0]);
                flag |= 2;
                if (flag == 3) //email & phone found
                {
                    if (emUserId != phUserId)
                    {
                        throw new FormException($"Unable to continue with {_d["email"]} and {_d["phprimary"]}", (int)HttpStatusCode.BadRequest, $"Email and Phone already exists for different users: {_d["email"]}, {_d["phprimary"]}", "EbProvisionUser => GetUserIdByEmailOrPhone");
                    }
                    return emUserId;
                }
                else if (_d.ContainsKey("email") && !string.IsNullOrWhiteSpace(_d["email"])) //email is unique 
                {
                    flag &= 1;
                    if (oProvUserId != phUserId) //given phone no of other user
                        return phUserId;
                    else //user id is not changed
                        return phUserId;
                }
                else //no email
                    return phUserId;
            }
            else if (flag == 1) //email found
            {
                if (_d.ContainsKey("phprimary") && !string.IsNullOrWhiteSpace(_d["phprimary"])) //phone is unique
                {
                    if (oProvUserId != emUserId) //given email no of other user
                        return emUserId;
                    else
                        return emUserId;
                }
                else //no phone
                    return emUserId;
            }
            return 0;
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

        private void SetUserType_Role_Status(Dictionary<string, string> _d, int nProvUserId, bool isIns)
        {
            if (isIns)
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
            }

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
                    {
                        if (_d.TryGetValue(FormConstants.primary_role, out string priRole_s) && int.TryParse(priRole_s, out int priRole_i) && !ebTyp.Roles.Contains(priRole_i) && priRole_i > 100)
                            ebTyp.Roles.Add(priRole_i);
                        this.AddOrChange(_d, FormConstants.roles, string.Join(CharConstants.COMMA, ebTyp.Roles));
                    }
                }
            }
        }

        private void MergeConstraintsForCreate(Dictionary<string, string> _nd)
        {
            List<int> nCons = new List<int>();
            if (_nd.TryGetValue(FormConstants.consadd, out string nwLocIds))
            {
                string[] loc_ids = nwLocIds.Split(CharConstants.COMMA);//only loc constraint is considered
                for (int i = 0; i < loc_ids.Length; i++)
                {
                    if (int.TryParse(loc_ids[i], out int locId) && locId > 0 && !nCons.Contains(locId))
                        nCons.Add(locId);
                }
                _nd[FormConstants.consadd] = string.Empty;
            }
            if (nCons.Count > 0)
            {
                EbConstraints consObj = new EbConstraints(nCons.Select(e => e.ToString()).ToArray(), EbConstraintKeyTypes.User, EbConstraintTypes.User_Location);
                _nd[FormConstants.consadd] = consObj.GetDataAsString();
            }
        }

        private void CreateUser(Dictionary<string, string> nd, ParameterizeCtrl_Params args, bool isMapped)
        {
            this.AddOrChange(nd, FormConstants.id, "0");
            this.SetUserType_Role_Status(nd, 0, true);
            this.MergeConstraintsForCreate(nd);
            this.SetExtendedSaveQuery(nd, args, isMapped);
        }

        private void MergeConstraintsForEdit(Dictionary<string, string> _d, Dictionary<string, string> _od, int nProvUserId)
        {
            List<int> nCons = new List<int>();
            Dictionary<int, int> oConsDict = null;//old constraints// key -> consMasterId, val -> locId

            if (_d.TryGetValue(FormConstants.consadd, out string nwLocIds))
            {
                string[] loc_ids = nwLocIds.Split(CharConstants.COMMA);//only loc constraint is considered
                for (int i = 0; i < loc_ids.Length; i++)
                {
                    if (int.TryParse(loc_ids[i], out int locId) && locId > 0 && !nCons.Contains(locId))
                        nCons.Add(locId);
                }
                _d[FormConstants.consadd] = string.Empty;
            }

            if (_od.ContainsKey(FormConstants.locConstraint))
                oConsDict = JsonConvert.DeserializeObject<Dictionary<int, int>>(_od[FormConstants.locConstraint]);


            if (oConsDict != null && nCons.Count > 0)
            {
                List<int> addLocIds = new List<int>();
                List<int> delIds = new List<int>();

                foreach (int consId in nCons)
                {
                    if (!oConsDict.ContainsValue(consId))
                        addLocIds.Add(consId);
                }
                foreach (KeyValuePair<int, int> item in oConsDict)
                {
                    if (!nCons.Contains(item.Value))
                        delIds.Add(item.Key);
                }

                if (addLocIds.Count > 0)
                {
                    EbConstraints consObj = new EbConstraints(addLocIds.Select(e => e.ToString()).ToArray(), EbConstraintKeyTypes.User, EbConstraintTypes.User_Location);
                    _d[FormConstants.consadd] = consObj.GetDataAsString();
                }
                if (delIds.Count > 0)
                    this.AddOrChange(_d, FormConstants.consdel, delIds.Join(","));
            }
            else if (oConsDict == null && nCons.Count > 0)
            {
                EbConstraints consObj = new EbConstraints(nCons.Select(e => e.ToString()).ToArray(), EbConstraintKeyTypes.User, EbConstraintTypes.User_Location);
                _d[FormConstants.consadd] = consObj.GetDataAsString();
            }
            else if (oConsDict != null && nCons.Count == 0)
            {
                this.AddOrChange(_d, FormConstants.consdel, oConsDict.Keys.Join(","));
            }

        }

        private void EditUser(Dictionary<string, string> nd, Dictionary<string, string> od, int oPuId, int nPuId, ParameterizeCtrl_Params args, bool isMapped)
        {
            if (od["eb_is_mapped_user"] == "T" || od["eb_is_mapped_user"] == "F")
            {
                int dataId = Convert.ToInt32(od["eb_data_id"]);
                int verId = Convert.ToInt32(od["eb_ver_id"]);
                if (dataId > 0 && verId > 0)
                {
                    if (dataId != (args.webForm as EbWebForm).TableRowId || verId != Convert.ToInt32((args.webForm as EbWebForm).RefId.Split(CharConstants.DASH)[4]))
                        throw new FormException("Selected user already linked with another form submission.", (int)HttpStatusCode.BadRequest, $"Unlink the selected user first then try again. (uid, did, vid)=({nPuId}, {dataId}, {verId})", "EbProvisionUser => EditUser....");
                }
            }

            this.AddOrChange(nd, FormConstants.id, nPuId.ToString());

            if (!nd.ContainsKey(FormConstants.usertype)) // usertype control is not configured
            {
                if (od.ContainsKey(FormConstants.usertype))
                {
                    this.AddOrChange(nd, FormConstants.usertype, od[FormConstants.usertype]);
                }
                else
                {
                    //load usertype from prop if only one usertype is configured
                    List<EbUserType> u_types = this.UserTypeToRole.FindAll(e => e.bVisible);
                    if (u_types.Count == 1)
                        nd.Add(FormConstants.usertype, Convert.ToString(u_types[0].iValue));
                }
            }

            int oldStatus = od.TryGetValue(FormConstants.statusid, out string oldStatus_s) && int.TryParse(oldStatus_s, out int oldStatus_i) ? oldStatus_i : -1;

            if (nd.TryGetValue(FormConstants.statusid, out string newStatus_s) && int.TryParse(newStatus_s, out int newStatus_i) && oldStatus >= 0 && newStatus_i == oldStatus)
            {
                //if status not changed
                this.AddOrChange(nd, FormConstants.statusid, Convert.ToString(oldStatus + 100));
            }

            if (oldStatus == (int)EbUserStatus.Unapproved)
            {
                if (args.usr.Roles.Contains(SystemRoles.SolutionOwner.ToString()) || args.usr.Roles.Contains(SystemRoles.SolutionAdmin.ToString()))
                {
                    int u_type = Convert.ToInt32(od[FormConstants.usertype]);
                    EbUserType ebTyp = this.UserTypeToRole.Find(e => e.iValue == u_type && e.bVisible);
                    if (ebTyp != null && ebTyp.Roles != null && ebTyp.Roles.Count > 0)
                    {
                        if (nd.TryGetValue(FormConstants.primary_role, out string priRole_s) && int.TryParse(priRole_s, out int priRole_i) && !ebTyp.Roles.Contains(priRole_i) && priRole_i > 100)
                            ebTyp.Roles.Add(priRole_i);
                        this.AddOrChange(nd, FormConstants.statusid, ((int)EbUserStatus.Active).ToString());
                        this.AddOrChange(nd, FormConstants.roles, string.Join(CharConstants.COMMA, ebTyp.Roles));
                    }
                }
            }

            //merge existing data(od) with partial new data(nd)
            foreach (KeyValuePair<string, string> item in od)
            {
                if (!nd.ContainsKey(item.Key))
                {
                    string val = item.Value;
                    if (item.Key == FormConstants.roles)
                    {
                        if (nd.TryGetValue(FormConstants.primary_role, out string priRole_s) && int.TryParse(priRole_s, out int priRole_i) && priRole_i > 100)
                        {
                            List<string> st = string.IsNullOrWhiteSpace(item.Value) ? new List<string>() : item.Value.Split(",").ToList();
                            if (!st.Contains(priRole_s))
                            {
                                if (st.Count > 0)// remove old primary roles
                                {
                                    string Qry = $"SELECT id FROM eb_roles WHERE is_primary='T' AND COALESCE(eb_del, 'F')='F' AND id IN(SELECT UNNEST(STRING_TO_ARRAY('{item.Value}', ',')::INTEGER[]));";
                                    EbDataTable dt = args.DataDB.DoQuery(Qry);
                                    foreach (EbDataRow dr in dt.Rows)
                                    {
                                        string old_rid = Convert.ToString(dr[0]);
                                        if (st.Contains(old_rid))
                                            st.Remove(old_rid);
                                    }
                                }
                                st.Add(priRole_s);// add new primary role
                                val = st.Join(",");
                            }
                        }
                    }
                    nd.Add(item.Key, val);
                }
            }

            this.MergeConstraintsForEdit(nd, od, nPuId);////////

            this.SetExtendedSaveQuery(nd, args, isMapped);
        }

        private void SetExtendedSaveQuery(Dictionary<string, string> nd, ParameterizeCtrl_Params args, bool isMapped)
        {
            string fnParams = string.Empty;
            for (int k = 0; k < this.FuncParam.Length; k++, args.i++)
            {
                object _value = this.FuncParam[k].Value;
                if (nd.ContainsKey(this.FuncParam[k].Name) && !string.IsNullOrEmpty(nd[this.FuncParam[k].Name]))
                {
                    _value = nd[this.FuncParam[k].Name];
                }
                fnParams += string.Concat(CharConstants.COLON, this.FuncParam[k].Name, CharConstants.UNDERSCORE, args.i, CharConstants.COMMA, CharConstants.SPACE);
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

            args._extqry += this.GetSaveQuery(fnParams.Substring(0, fnParams.Length - 2), args.tbl, args.ins, Convert.ToInt32(nd[FormConstants.id]), isMapped);//insertOnUpdate
        }

        private void SetExtendedUnlinkQuery(ParameterizeCtrl_Params args, string userId_s)
        {
            string ee = $"UPDATE eb_users SET eb_is_mapped_user='P' WHERE id={userId_s} AND eb_ver_id = @{args.tbl}_eb_ver_id AND eb_data_id = @{args.tbl}_id AND (eb_is_mapped_user='T' OR eb_is_mapped_user='F');";
            args._extqry += ee;
        }

        private void SetControlParams(ParameterizeCtrl_Params args, int UserId)
        {
            if (args.ins)
            {
                args._cols += args.cField.Name + CharConstants.COMMA + CharConstants.SPACE;
                args._vals += CharConstants.AT + args.cField.Name + CharConstants.UNDERSCORE + args.i + CharConstants.COMMA + CharConstants.SPACE;
                args.param.Add(args.DataDB.GetNewParameter(args.cField.Name + CharConstants.UNDERSCORE + args.i, (EbDbTypes)args.cField.Type, UserId));
            }
            else
            {
                args._colvals += string.Concat(args.cField.Name, CharConstants.EQUALS, CharConstants.AT, args.cField.Name, CharConstants.UNDERSCORE, args.i, CharConstants.COMMA, CharConstants.SPACE);
                args.param.Add(args.DataDB.GetNewParameter(args.cField.Name + CharConstants.UNDERSCORE + args.i, (EbDbTypes)args.cField.Type, UserId));
            }
            args.i++;
        }

        public override bool ParameterizeControl(ParameterizeCtrl_Params args, string crudContext)
        {
            int oProvUserId = args.ins ? 0 : (int.TryParse(args.ocF?.Value?.ToString(), out int qq) ? qq : 0);

            if (!this.CreateOnlyIf_b)
            {
                SetControlParams(args, 0);
                if (oProvUserId > 0)
                    this.SetExtendedUnlinkQuery(args, oProvUserId.ToString());
                return false;
            }
            Dictionary<string, string> _d = JsonConvert.DeserializeObject<Dictionary<string, string>>(Convert.ToString(args.cField.F));
            int nProvUserId;
            int flag = 0;
            if ((_d.ContainsKey(FormConstants.email) && _d[FormConstants.email].Trim() != string.Empty) || (_d.ContainsKey(FormConstants.phprimary) && _d[FormConstants.phprimary].Trim() != string.Empty))
            {
                nProvUserId = this.GetUserIdByEmailOrPhone(args.DataDB, _d, ref flag, args.ins, args.ocF);
                if (nProvUserId == 1)
                {
                    SetControlParams(args, 0);
                    if (oProvUserId > 0)
                        this.SetExtendedUnlinkQuery(args, oProvUserId.ToString());
                    return false;
                }
            }
            else
            {
                SetControlParams(args, 0);
                if (oProvUserId > 0)
                    this.SetExtendedUnlinkQuery(args, oProvUserId.ToString());
                return false;
            }

            if (oProvUserId == 0 && nProvUserId == 0) //Create new user
            {
                this.CreateUser(_d, args, false);
            }
            else if (oProvUserId == 0 && nProvUserId > 0) //Link and edit existing user
            {
                if (!this.AllowExistingUser)
                    this.ThrowExistingUserException(_d, flag);

                if (!this.BlockUserEditing)
                {
                    Dictionary<string, string> od = this.GetFormattedData(args.DataDB, nProvUserId);
                    this.EditUser(_d, od, 0, nProvUserId, args, true);
                }
                SetControlParams(args, nProvUserId);
            }
            else if (oProvUserId > 0 && nProvUserId == 0) //Edit already linked/created user
            {
                if (!this.BlockUserEditing)
                {
                    Dictionary<string, string> _od = JsonConvert.DeserializeObject<Dictionary<string, string>>(Convert.ToString(args.ocF.F));
                    this.EditUser(_d, _od, 0, oProvUserId, args, _od["eb_is_mapped_user"].ToString() == "T");
                }
                SetControlParams(args, nProvUserId);
            }
            else if (oProvUserId > 0 && nProvUserId > 0)
            {
                if (oProvUserId == nProvUserId) //Edit existing user
                {
                    if (!this.BlockUserEditing)
                    {
                        Dictionary<string, string> _od = JsonConvert.DeserializeObject<Dictionary<string, string>>(Convert.ToString(args.ocF.F));
                        this.EditUser(_d, _od, oProvUserId, nProvUserId, args, _od["eb_is_mapped_user"].ToString() == "T");
                    }
                    SetControlParams(args, nProvUserId);
                }
                else //Link and edit another user
                {
                    if (!this.AllowExistingUser)
                        this.ThrowExistingUserException(_d, flag);

                    if (!this.BlockUserEditing)
                    {
                        Dictionary<string, string> od = this.GetFormattedData(args.DataDB, nProvUserId);
                        this.EditUser(_d, od, 0, nProvUserId, args, true);
                    }
                    SetControlParams(args, nProvUserId);
                    this.SetExtendedUnlinkQuery(args, oProvUserId.ToString());
                }
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
            System.Random random = new System.Random();
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

        private string MailHtml2
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
                <img src='https://myaccount.expressbase.com/images/logo/ebdboihyfxflxe20220224111752.png' style='max-height: 100px; max-width: 300px; ' />
            </div>
            <br />
            <div style='line-height: 1.4;'>
                Dear {UserName},<br />
                <br />
                You have been added as a user into Sakshyam Portal. Please find below credentials to log in.
                <br />
                <br />
                Solution URL - https://sakshyam.expressbase.com/Ext/UsrSignIn?Page=False <br />
                User name - {Email} <br />
                Password - {Password} <br />
                <br />
                Please make sure you change the password after logging in (in the <b>My Profile</b> page).
            </div>
            <br />
            <br />
            Thanks,<br />
            Sakshyam Portal Team<br />
        </div>
    </body>
</html>";
            }
            set { }
        }

        private string GetOloiMailContent(string currentLang)
        {
            if (currentLang == "ml")
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
                പ്രിയ {UserName},<br />
                <br />
                കെ-ഡിസ്ക്കിൻറെ നൂതനാശയ പോർട്ടലിന്റെ ഉപയോക്താവായി താങ്കളും കൂടി ചേർക്കപ്പെട്ട വിവരം സന്തോഷ പൂർവം അറിയിക്കുന്നു. തുടർ പ്രക്രിയകൾക്കായി താഴെ പറയുന്ന വിവരങ്ങൾ ദയവായി ശ്രദ്ധിക്കുക.
                <br />
                <br />
                URL - https://{eSolutionId}<br />
                യൂസർ നെയിം - {Email} <br />
                പാസ്സ്‌വേർഡ് - {Password} <br />
                <br />
                ലോഗിൻ ചെയ്തതിനു ശേഷം താങ്കളുടെ പാസ്സ്‌വേർഡിൽ വരുത്തിയിട്ടുള്ള മാറ്റം ദയവായി ഉറപ്പു വരുത്തുക.
            </div>
            <br />
            <br />
            Thanks,<br />
            Team OLOI<br />
            <div><img src='https://drive.google.com/uc?export=view&id=1nDANkSWrKWYeoMR0YcG70QG4ZpDT-aUV' style='max-height: 50px; max-width: 150px; ' /></<div>
        </div>
    </body>
</html>";
            }

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
                You have been added as a user in the K-DISC innovation portal. Please find below credentials to log in.
                <br />
                <br />
                URL - https://{eSolutionId}<br />
                User name - {Email} <br />
                Password - {Password} <br />
                <br />
                Please make sure you change the password after logging in (in the <b>My Profile</b> page).
            </div>
            <br />
            <br />
            Thanks,<br />
            Team OLOI<br />
            <div><img src='https://drive.google.com/uc?export=view&id=1nDANkSWrKWYeoMR0YcG70QG4ZpDT-aUV' style='max-height: 50px; max-width: 150px; ' /></<div>
        </div>
    </body>
</html>";
        }

        public void SendWelcomeMail(RabbitMqProducer MessageProducer3, User user, Eb_Solution solution, string currentLang)
        {
            string __html = this.MailHtml;
            if (solution.SolutionID == "ebdboihyfxflxe20220224111752")//sakshyam
                __html = this.MailHtml2;
            else if (solution.SolutionID == "ebdbawg6osdxo920220727085204")//oloi
                __html = this.GetOloiMailContent(currentLang);
            __html = __html
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
                Message = __html,
                Subject = (solution.SolutionID == "ebdboihyfxflxe20220224111752" ? "Welcome to Sakshyam Portal" : (solution.SolutionID == "ebdbawg6osdxo920220727085204" ? $"Welcome to {solution.SolutionName}" : $"Welcome to {solution.SolutionName} Solution")),
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
