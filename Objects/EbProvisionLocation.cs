using ExpressBase.Common;
using ExpressBase.Common.Constants;
using ExpressBase.Common.Extensions;
using ExpressBase.Common.LocationNSolution;
using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using ExpressBase.Common.Structures;
using ExpressBase.Objects.ServiceStack_Artifacts;
using ExpressBase.Objects.WebFormRelated;
using ExpressBase.Security;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;

namespace ExpressBase.Objects
{
    [EnableInBuilder(BuilderType.WebForm)]
    public class EbProvisionLocation : EbControlUI, IEbPlaceHolderControl, IEbExtraQryCtrl
    {
        public EbProvisionLocation()
        {
            Fields = new List<UsrLocFieldAbstract>();
        }

        [OnDeserialized]
        public void OnDeserializedMethod(StreamingContext context)
        {
            this.BareControlHtml = this.GetBareHtml();
            this.ObjType = this.GetType().Name.Substring(2, this.GetType().Name.Length - 2);
        }

        public override string ToolIconHtml { get { return "<i class='fa fa-map-marker'></i><i class='fa fa-plus'></i>"; } set { } }

        public override string ToolNameAlias { get { return "Prov. Location"; } set { } }

        public override string ToolHelpText { get { return "Provision Location"; } set { } }

        public override string UIchangeFns
        {
            get
            {
                return @"EbProvisionLocation = {
                mapping : function(elementId, props) {
                    let html = '';
                    $(`#cont_${elementId}`).html(`<span class='eb-ctrl-label' ui-label id='@ebsid@Lbl' style='font-style: italic; font-weight: normal;'> ProvisionLocation </span>`);
                    $.each(props.Fields.$values, function (i, field) {
                        if (field.ControlName === '' && field.IsRequired){
                            if(field.Type === 'image')
                                html += `<div class = 'prov-loc-item'>
                                            <span class='eb-ctrl-label'>${field.DisplayName}</span>
                                            <div class='ctrl-cover'><div style='text-align: center;'><img src='/images/image.png' style='height: 100px; opacity: 0.5;'></div></div>
                                        </div>`;
                            else
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

        [PropertyGroup(PGConstants.CORE)]
        [EnableInBuilder(BuilderType.WebForm)]
        [EbRequired]
        [Unique]
        [regexCheck]
        [InputMask("[a-z][a-z0-9]*(_[a-z0-9]+)*")]
        [DefaultPropValue("eb_prov_loc_id")]
        public override string Name { get; set; }

        //[EnableInBuilder(BuilderType.WebForm)]
        //[HideInPropertyGrid]
        //public override bool Hidden { get; set; }

        [EnableInBuilder(BuilderType.WebForm)]
        [HideInPropertyGrid]
        public override bool IsSysControl { get { return false; } }

        [EnableInBuilder(BuilderType.WebForm)]
        [PropertyGroup(PGConstants.CORE)]
        [UIproperty]
        //[OnChangeUIFunction("EbProvisionLocation.mapping")]
        [PropertyEditor(PropertyEditorType.Collection)]
        [ListType(typeof(UsrLocFieldAbstract))]
        public List<UsrLocFieldAbstract> Fields { get; set; }

        [EnableInBuilder(BuilderType.WebForm)]
        [HideInPropertyGrid]
        public override EbDbTypes EbDbType { get { return EbDbTypes.Int32; } }

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

        //[EnableInBuilder(BuilderType.WebForm)]
        //[HideInPropertyGrid]
        //public override EbScript HiddenExpr { get; set; }

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
    fields[0] = new EbObjects.UsrLocField('longname');
    fields[1] = new EbObjects.UsrLocField('shortname');
    fields[2] = new EbObjects.UsrLocField('image');
    fields[3] = new EbObjects.UsrLocField('is_group');
    fields[4] = new EbObjects.UsrLocField('parent_id');
    fields[5] = new EbObjects.UsrLocField('eb_location_types_id');

    $.extend(fields[0], { DisplayName: 'Name', Type: 'text', IsRequired: true });
    $.extend(fields[1], { DisplayName: 'ShortName', Type: 'text', IsRequired: false });
    $.extend(fields[2], { DisplayName: 'Logo', Type: 'image', IsRequired: false });
    $.extend(fields[3], { DisplayName: 'IsGroup', Type: 'text', IsRequired: false });
    $.extend(fields[4], { DisplayName: 'ParentId', Type: 'integer', IsRequired: false });
    $.extend(fields[5], { DisplayName: 'Type', Type: 'integer', IsRequired: false });

    for (let i = 0; i < 6; i++){
	    this.Fields.$values.push(fields[i]);
    }
    commonO.ObjCollection['#vernav0'].GetLocationConfig(this);
};";
        }

        public static bool IsSystemField(string name)
        {
            return name == FormConstants.longname ||
                name == FormConstants.shortname ||
                name == FormConstants.image ||
                name == FormConstants.is_group ||
                name == FormConstants.parent_id ||
                name == FormConstants.eb_location_types_id;
        }

        public override string GetBareHtml()
        {
            return @"<span class='eb-ctrl-label' ui-label id='@ebsidLbl'> ProvisionLocation </span>";

            //            return @"
            //            <input id='@ebsid@' data-ebtype='@data-ebtype@'  data-toggle='tooltip' title='@toolTipText@' class='date' type='text' name='@name@' autocomplete = 'off' @value@ @tabIndex@ style='width:100%; @BackColor@ @ForeColor@ display:inline-block; @fontStyle@' @required@ @placeHolder@ disabled />
            //            "
            //.Replace("@name@", (this.Name != null ? this.Name.Trim() : ""))
            //.Replace("@data-ebtype@", "16")//( (int)this.EbDateType ).ToString())
            //.Replace("@toolTipText@", this.ToolTipText)
            //.Replace("@ebsid@", String.IsNullOrEmpty(this.EbSid_CtxId) ? "@ebsid@" : this.EbSid_CtxId)
            //.Replace("@value@", "")//"value='" + this.Value + "'")
            //.Replace("@tabIndex@", "tabindex='" + this.TabIndex + "'")
            //.Replace("@BackColor@ ", "background-color: #eee;")
            //    //.Replace("@BackColor@ ", ("background-color:" + ((this.BackColor != null) ? this.BackColor : "@BackColor@ ") + ";"))
            //    .Replace("@ForeColor@ ", "color:" + ((this.ForeColor != null) ? this.ForeColor : "@ForeColor@ ") + ";")
            //.Replace("@required@", (this.Required && !this.Hidden ? " required" : string.Empty))
            //.Replace("@placeHolder@", "placeholder=''");
        }

        public override string GetDesignHtml()
        {
            return GetHtml().RemoveCR().DoubleQuoted();
        }

        public override string GetHtml()
        {
            string EbCtrlHTML = @"
        <div id='cont_@ebsid@' ebsid='@ebsid@' name='@name@' class='Eb-ctrlContainer' @childOf@ ctype='@type@' eb-hidden='@isHidden@'>
            <span class='eb-ctrl-label' ui-label id='@ebsidLbl'> ProvisionLocation </span>
            <div>- Design html is not implemented -</div>
        </div>"
               .Replace("@LabelForeColor ", "color:" + (LabelForeColor ?? "@LabelForeColor ") + ";")
               .Replace("@LabelBackColor ", "background-color:" + (LabelBackColor ?? "@LabelBackColor ") + ";");

            return ReplacePropsInHTML(EbCtrlHTML);
        }

        public string TableName { get; set; }

        public bool IsLocationCreated { get; set; }

        public string GetSelectQuery(IDatabase DataDB, string masterTbl, string form_ver_id, string form_ref_id)
        {
            return $"SELECT id, longname, shortname, image, meta_json, is_group, parent_id, eb_location_types_id FROM eb_locations WHERE eb_ver_id = {form_ver_id} AND eb_data_id = @{masterTbl}_id AND COALESCE(eb_del, 'F') = 'F';";
        }

        private string GetSaveQuery(bool ins, string param, string mtbl)
        {
            if (ins)
                return $"INSERT INTO eb_locations(longname, shortname, image, meta_json, eb_ver_id, eb_data_id) VALUES({param} :{mtbl}_eb_ver_id, eb_currval('{mtbl}_id_seq'));";
            else
                return $"UPDATE eb_locations SET {param} WHERE eb_ver_id = :{mtbl}_eb_ver_id AND eb_data_id = :{mtbl}_id;";
        }

        private void AddParam(IDatabase DataDB, List<DbParameter> param, int i, EbDbTypes type, Dictionary<string, string> _d, Dictionary<string, string> _od, string key, object altVal)
        {
            if (_d.ContainsKey(key))
                altVal = _d[key];
            else if (_od != null && _od.ContainsKey(key))
                altVal = _od[key];

            param.Add(DataDB.GetNewParameter($"{key}_{i}", type, altVal));
        }

        public override bool ParameterizeControl(ParameterizeCtrl_Params args, string crudContext)
        {
            Dictionary<string, string> _d = JsonConvert.DeserializeObject<Dictionary<string, string>>(Convert.ToString(args.cField.F));
            Dictionary<string, string> _od = args.ocF == null ? null : JsonConvert.DeserializeObject<Dictionary<string, string>>(Convert.ToString(args.ocF.F));
            if (string.IsNullOrEmpty(_d[FormConstants.longname]))
                return false;
            string selQry = "SELECT id FROM eb_locations WHERE LOWER(longname) LIKE LOWER(@longname) AND COALESCE(eb_del, 'F') = 'F';";
            EbDataTable dt = args.DataDB.DoQuery(selQry, new DbParameter[] { args.DataDB.GetNewParameter(FormConstants.longname, EbDbTypes.String, _d[FormConstants.longname]) });
            int nProvLocId = 0;
            if (dt.Rows.Count > 0)
                nProvLocId = Convert.ToInt32(dt.Rows[0][0]);
            string temp;
            if (args.ins)
            {
                if (nProvLocId > 0)
                    throw new FormException(_d[FormConstants.longname] + " is not unique.", (int)HttpStatusCode.BadRequest, "Given longname is already exists in eb_locations", "EbProvisionLocation -> ParameterizeControl");

                temp = $@"INSERT INTO eb_locations(longname, shortname, image, meta_json, is_group, parent_id, eb_location_types_id, eb_ver_id, eb_data_id, eb_created_by, eb_created_at, eb_lastmodified_by, eb_lastmodified_at, eb_del) 
                    VALUES(@longname_{args.i}, @shortname_{args.i}, @image_{args.i}, @meta_json_{args.i}, @is_group_{args.i}, @parent_id_{args.i}, @eb_location_types_id_{args.i}, @{args.tbl}_eb_ver_id, eb_currval('{args.tbl}_id_seq'), @{FormConstants.eb_createdby}, {args.DataDB.EB_CURRENT_TIMESTAMP}, @{FormConstants.eb_createdby}, {args.DataDB.EB_CURRENT_TIMESTAMP}, 'F');";

                if (args.DataDB.Vendor == DatabaseVendors.MYSQL)
                    temp += "SELECT eb_persist_currval('eb_locations_id_seq');";
                temp += $"UPDATE {this.TableName} SET {this.Name} = eb_currval('eb_locations_id_seq') WHERE {(this.TableName == args.tbl ? "id" : (args.tbl + "_id"))} = eb_currval('{args.tbl}_id_seq'); ";

                this.IsLocationCreated = true;
            }
            else
            {
                int oProvLocId = args.ocF == null ? 0 : Convert.ToInt32(args.ocF.Value);
                if (nProvLocId > 0 && nProvLocId != oProvLocId)
                    throw new FormException(_d[FormConstants.longname] + " is not unique.", (int)HttpStatusCode.BadRequest, "Given longname is already exists in eb_locations", "EbProvisionLocation -> ParameterizeControl");

                temp = $@"UPDATE eb_locations SET longname = @longname_{args.i}, shortname = @shortname_{args.i}, image = @image_{args.i}, meta_json = @meta_json_{args.i}, 
                            is_group = @is_group_{args.i}, parent_id = @parent_id_{args.i}, eb_location_types_id = @eb_location_types_id_{args.i}, eb_lastmodified_by = @{FormConstants.eb_modified_by}, eb_lastmodified_at = {args.DataDB.EB_CURRENT_TIMESTAMP}
                            WHERE eb_ver_id = :{args.tbl}_eb_ver_id AND eb_data_id = :{args.tbl}_id AND COALESCE(eb_del, 'F') = 'F';";
            }
            args.param.Add(args.DataDB.GetNewParameter("longname_" + args.i, EbDbTypes.String, _d[FormConstants.longname]));
            AddParam(args.DataDB, args.param, args.i, EbDbTypes.String, _d, _od, FormConstants.shortname, _d[FormConstants.longname]);
            AddParam(args.DataDB, args.param, args.i, EbDbTypes.String, _d, _od, FormConstants.image, "../img");
            AddParam(args.DataDB, args.param, args.i, EbDbTypes.String, _d, _od, FormConstants.meta_json, "{}");
            AddParam(args.DataDB, args.param, args.i, EbDbTypes.String, _d, _od, FormConstants.is_group, "T");
            AddParam(args.DataDB, args.param, args.i, EbDbTypes.Decimal, _d, _od, FormConstants.parent_id, 0);
            AddParam(args.DataDB, args.param, args.i, EbDbTypes.Int32, _d, _od, FormConstants.eb_location_types_id, 1);

            args._extqry = temp + args._extqry; //location must be created before user creation
            args.i++;
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

        //GetValueFromDOMJSfn
        //if (!this.hasOwnProperty('_finalObj'))
        //    this._finalObj = {};
        //let metaObj = {};
        //$.each(this.Fields.$values, function (i, obj) {
        //    if (obj.ControlName !== '') {
        //        if (obj.Name === 'shortname' || obj.Name === 'longname' || obj.Name === 'image')
        //            this._finalObj[obj.Name] = obj.Control.getValueFromDOM();
        //        else
        //            metaObj[obj.DisplayName] = obj.Control.getValueFromDOM();
        //    }
        //}.bind(this));
        //this._finalObj['meta_json'] = JSON.stringify(metaObj);
        //return JSON.stringify(this._finalObj);

        //SetValueJSfn
        //this._finalObj = JSON.parse(p1);
        //let metaObj = JSON.parse(this._finalObj['meta_json']) || {};
        //$.each(this.Fields.$values, function (i, obj) {
        //    if (obj.ControlName !== '') {
        //        if (obj.Name === 'shortname' || obj.Name === 'longname' || obj.Name === 'image')
        //            obj.Control.setValue(this._finalObj[obj.Name]);
        //        else if (metaObj.hasOwnProperty(obj.DisplayName))
        //            obj.Control.setValue(metaObj[obj.DisplayName]);
        //    }
        //}.bind(this));

        //OnChangeBindJSFn
        //$.each(this.Fields.$values, function (i, obj) {
        //    if (obj.ControlName !== '') {
        //        $('#' + obj.Control.EbSid_CtxId).on('change', p1);
        //    }
        //}.bind(this));
    }
}
