using ExpressBase.Common;
using ExpressBase.Common.Extensions;
using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Runtime.Serialization;
using ExpressBase.Security;
using Newtonsoft.Json;
using ExpressBase.Common.Structures;
using ExpressBase.Objects.ServiceStack_Artifacts;
using ExpressBase.Common.Constants;
using ExpressBase.Common.LocationNSolution;
using System.Net;
using ExpressBase.Objects.WebFormRelated;

namespace ExpressBase.Objects
{
    [EnableInBuilder(BuilderType.WebForm)]
    class EbProvisionRole : EbControlUI, IEbPlaceHolderControl, IEbExtraQryCtrl
    {
        public EbProvisionRole()
        {
            this.Fields = new List<UsrLocFieldAbstract>();
        }

        [OnDeserialized]
        public void OnDeserializedMethod(StreamingContext context)
        {
            this.BareControlHtml = this.GetBareHtml();
            this.ObjType = this.GetType().Name.Substring(2, this.GetType().Name.Length - 2);
        }

        public override string ToolIconHtml { get { return "<i class='fa fa-user-circle-o'></i><i class='fa fa-plus'></i>"; } set { } }

        public override string ToolNameAlias { get { return "Prov. Role"; } set { } }

        public override string ToolHelpText { get { return "Provision Role"; } set { } }


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
        public bool AllowExistingRole { get; set; }


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

        public override string GetJsInitFunc()
        {
            return @"
this.Init = function(id)
{
    let fields = [];
    fields[0] = new EbObjects.UsrLocField('role_name');
    fields[1] = new EbObjects.UsrLocField('applicationid');
    fields[2] = new EbObjects.UsrLocField('description');
    fields[3] = new EbObjects.UsrLocField('is_primary');

    $.extend(fields[0], { DisplayName: 'Name', Type: 'text', IsRequired: true });
    $.extend(fields[1], { DisplayName: 'Application ID', Type: 'text', IsRequired: true });
    $.extend(fields[2], { DisplayName: 'Description', Type: 'text', IsRequired: false });
    $.extend(fields[2], { DisplayName: 'Is Primary', Type: 'text', IsRequired: false });

    for (let i = 0; i < 4; i++){
	    this.Fields.$values.push(fields[i]);
    }
};";
        }


        public override string GetBareHtml()
        {
            return @"<span class='eb-ctrl-label' ui-label id='@ebsidLbl'> ProvisionRole </span>";
        }

        public override string GetDesignHtml()
        {
            return GetHtml().RemoveCR().DoubleQuoted();
        }

        public override string GetHtml()
        {
            string EbCtrlHTML = @"
        <div id='cont_@ebsid@' ebsid='@ebsid@' name='@name@' class='Eb-ctrlContainer' @childOf@ ctype='@type@' eb-hidden='@isHidden@'>
            <span class='eb-ctrl-label' ui-label id='@ebsidLbl'> ProvisionRole </span>
            <div>- Design html is not implemented -</div>
        </div>"
               .Replace("@LabelForeColor ", "color:" + (LabelForeColor ?? "@LabelForeColor ") + ";")
               .Replace("@LabelBackColor ", "background-color:" + (LabelBackColor ?? "@LabelBackColor ") + ";");

            return ReplacePropsInHTML(EbCtrlHTML);
        }

        public string TableName { get; set; }

        public string GetSelectQuery(IDatabase DataDB, string masterTbl)
        {
            return string.Format("SELECT id, role_name, applicationid, description, is_primary FROM eb_roles WHERE eb_ver_id = :{0}_eb_ver_id AND eb_data_id = :{0}_id AND COALESCE(eb_del, 'F') = 'F';", masterTbl);
        }

        public override bool ParameterizeControl(ParameterizeCtrl_Params args, string crudContext)
        {
            Dictionary<string, string> _d = JsonConvert.DeserializeObject<Dictionary<string, string>>(Convert.ToString(args.cField.F));
            Dictionary<string, string> _od = args.ocF == null ? null : JsonConvert.DeserializeObject<Dictionary<string, string>>(Convert.ToString(args.ocF.F));
            if (string.IsNullOrEmpty(_d[FormConstants.role_name]))
                return false;
            string selQry = "SELECT id FROM eb_roles WHERE LOWER(role_name) LIKE LOWER(@role_name) AND COALESCE(eb_del, 'F') = 'F';";
            EbDataTable dt = args.DataDB.DoQuery(selQry, new DbParameter[] { args.DataDB.GetNewParameter(FormConstants.role_name, EbDbTypes.String, _d[FormConstants.role_name]) });
            int nProvRolId = 0;
            if (dt.Rows.Count > 0)
                nProvRolId = Convert.ToInt32(dt.Rows[0][0]);
            string temp = string.Empty;
            if (args.ins)
            {
                bool doInsert = true;
                if (nProvRolId > 0)
                {
                    if (!this.AllowExistingRole)
                        throw new FormException(_d[FormConstants.role_name] + " is not unique.", (int)HttpStatusCode.BadRequest, "Given role_name is already exists in eb_roles", "EbProvisionRole -> ParameterizeControl");
                    else
                        doInsert = false;
                }
                if (doInsert)
                {
                    temp += $@"INSERT INTO eb_roles(role_name, applicationid, description,is_primary, eb_ver_id, eb_data_id, eb_del) 
                    VALUES(@role_name_{args.i}, @applicationid_{args.i}, @description_{args.i}, @is_primary_{args.i}, @{args.tbl}_eb_ver_id, eb_currval('{args.tbl}_id_seq'), 'F');";
                }

                temp += $"UPDATE {this.TableName} SET {this.Name} = {(doInsert ? "eb_currval('eb_roles_id_seq') + 100" : nProvRolId.ToString())} WHERE {(this.TableName == args.tbl ? "id" : (args.tbl + "_id"))} = eb_currval('{args.tbl}_id_seq'); ";
            }
            else
            {
                int oProvRolId = args.ocF == null ? 0 : Convert.ToInt32(args.ocF.Value);
                if (nProvRolId > 0 && nProvRolId != oProvRolId)
                    throw new FormException(_d[FormConstants.role_name] + " is not unique.", (int)HttpStatusCode.BadRequest, "Given role_name is already exists in eb_roles", "EbProvisionRole -> ParameterizeControl");

                temp = $@"UPDATE eb_roles SET role_name = @role_name_{args.i}, applicationid = @applicationid_{args.i}, description = @description_{args.i}, is_primary = @is_primary_{args.i}
                            WHERE eb_ver_id = :{args.tbl}_eb_ver_id AND eb_data_id = :{args.tbl}_id AND COALESCE(eb_del, 'F') = 'F';";
            }
            args.param.Add(args.DataDB.GetNewParameter("role_name_" + args.i, EbDbTypes.String, _d[FormConstants.role_name]));
            AddParam(args.DataDB, args.param, args.i, EbDbTypes.Int32, _d, _od, FormConstants.applicationid, 0);
            AddParam(args.DataDB, args.param, args.i, EbDbTypes.String, _d, _od, FormConstants.description, string.Empty);
            AddParam(args.DataDB, args.param, args.i, EbDbTypes.String, _d, _od, FormConstants.is_primary, "F");

            args._extqry = temp + args._extqry;
            args.i++;
            return true;
        }

        private void AddParam(IDatabase DataDB, List<DbParameter> param, int i, EbDbTypes type, Dictionary<string, string> _d, Dictionary<string, string> _od, string key, object altVal)
        {
            if (_d.ContainsKey(key))
                altVal = _d[key];
            else if (_od != null && _od.ContainsKey(key))
                altVal = _od[key];

            param.Add(DataDB.GetNewParameter($"{key}_{i}", type, altVal));
        }

        public override SingleColumn GetSingleColumn(User UserObj, Eb_Solution SoluObj, object Value, bool Default)
        {
            if (Value != null && int.TryParse(Convert.ToString(Value), out int _t))
                Value = _t;
            return new SingleColumn()
            {
                Name = this.Name,
                Type = (int)this.EbDbType,
                Value = Value == null ? 0 : Value,
                Control = this,
                ObjType = this.ObjType,
                F = "{}"
            };
        }

        public override string GetValueFromDOMJSfn
        {
            get
            {
                return @"if (this.hasOwnProperty('_valueFE')) return this._valueFE;
                        else return 0;";
            }
            set { }
        }

        public override string SetValueJSfn
        {
            get
            {
                return @"this._valueFE = p1;";
            }
            set { }
        }

        public override string OnChangeBindJSFn { get { return @""; } set { } }

        public override string RefreshJSfn { get { return @""; } set { } }

        public override string ClearJSfn { get { return @""; } set { } }

    }
}
