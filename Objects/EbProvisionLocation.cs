using ExpressBase.Common;
using ExpressBase.Common.Constants;
using ExpressBase.Common.Extensions;
using ExpressBase.Common.LocationNSolution;
using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using ExpressBase.Common.Structures;
using ExpressBase.Security;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Runtime.Serialization;

namespace ExpressBase.Objects
{
    [EnableInBuilder(BuilderType.WebForm)]
    public class EbProvisionLocation : EbControlUI, IEbPlaceHolderControl
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

        [EnableInBuilder(BuilderType.WebForm)]
        [HideInPropertyGrid]
        public override bool Hidden { get { return true; } }

        [EnableInBuilder(BuilderType.WebForm)]
        [HideInPropertyGrid]
        public override bool IsSysControl { get { return false; } }

        [EnableInBuilder(BuilderType.WebForm)]
        [PropertyGroup(PGConstants.CORE)]
        [UIproperty]
        [OnChangeUIFunction("EbProvisionLocation.mapping")]
        [PropertyEditor(PropertyEditorType.Collection)]
        [ListType(typeof(UsrLocFieldAbstract))]
        public List<UsrLocFieldAbstract> Fields { get; set; }

        [EnableInBuilder(BuilderType.WebForm)]
        [HideInPropertyGrid]
        public override EbDbTypes EbDbType { get { return EbDbTypes.String; } }
        
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

        public override string GetJsInitFunc()
        {
            return @"
this.Init = function(id)
{
    let fields = [];
    fields[0] = new EbObjects.UsrLocField('longname');
    fields[1] = new EbObjects.UsrLocField('shortname');
    fields[2] = new EbObjects.UsrLocField('image');

    fields[0].DisplayName = 'Name';
    fields[1].DisplayName = 'ShortName';
    fields[2].DisplayName = 'Logo';
    fields[0].Type = 'text';
    fields[1].Type = 'text';
    fields[2].Type = 'image';

    for (let i = 0; i < 3; i++){
        fields[i].IsRequired = true;
	    this.Fields.$values.push(fields[i]);
    }
    commonO.ObjCollection['#vernav0'].GetLocationConfig(this);
};";
        }

        public override string GetBareHtml()
        {
            return @"<span class='eb-ctrl-label' ui-label id='@ebsidLbl'> ProvisionLocation </span>";

//            return @"
//            <input id='@ebsid@' data-ebtype='@data-ebtype@'  data-toggle='tooltip' title='@toolTipText@' class='date' type='text' name='@name@' autocomplete = 'off' @value@ @tabIndex@ style='width:100%; @BackColor@ @ForeColor@ display:inline-block; @fontStyle@ @readOnlyString@ @required@ @placeHolder@ disabled />
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
//.Replace("@readOnlyString@", this.ReadOnlyString)
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
            <i class='fa fa-spinner fa-pulse' aria-hidden='true'></i>
        </div>"
               .Replace("@LabelForeColor ", "color:" + (LabelForeColor ?? "@LabelForeColor ") + ";")
               .Replace("@LabelBackColor ", "background-color:" + (LabelBackColor ?? "@LabelBackColor ") + ";");

            return ReplacePropsInHTML(EbCtrlHTML);
        }

        public string VirtualTable { get; set; }
        
        public string GetSelectQuery(string masterTbl)
        {
            return string.Format("SELECT id, longname, shortname, image, meta_json FROM eb_locations WHERE eb_ver_id = :{0}_eb_ver_id AND eb_data_id = :{0}_id;", masterTbl);
        }

        private string GetSaveQuery(bool ins, string param, string mtbl)
        {
            if (ins)
                return $"INSERT INTO eb_locations(longname, shortname, image, meta_json, eb_ver_id, eb_data_id) VALUES({param} :{mtbl}_eb_ver_id, eb_currval('{mtbl}_id_seq'));";
            else
                return $"UPDATE eb_locations SET {param} WHERE eb_ver_id = :{mtbl}_eb_ver_id AND eb_data_id = :{mtbl}_id;";
        }

        public override bool ParameterizeControl(IDatabase DataDB, List<DbParameter> param, string tbl, SingleColumn cField, bool ins, ref int i, ref string _col, ref string _val, ref string _extqry, User usr, SingleColumn ocF)
        {
            Dictionary<string, string> _d = JsonConvert.DeserializeObject<Dictionary<string, string>>(cField.Value);
            param.Add(DataDB.GetNewParameter("shortname_" + i, EbDbTypes.String, _d["shortname"]));
            param.Add(DataDB.GetNewParameter("longname_" + i, EbDbTypes.String, _d["longname"]));
            param.Add(DataDB.GetNewParameter("image_" + i, EbDbTypes.String, _d["image"]));
            param.Add(DataDB.GetNewParameter("meta_json_" + i, EbDbTypes.String, _d["meta_json"]));
            string temp = string.Empty;
            if (ins)
            {
                temp = $"INSERT INTO eb_locations(shortname, longname, image, meta_json, eb_ver_id, eb_data_id) VALUES(:shortname_{i}, :longname_{i}, :image_{i}, :meta_json_{i}, :{tbl}_eb_ver_id, eb_currval('{tbl}_id_seq'));";
                if (DataDB.Vendor == DatabaseVendors.MYSQL)
                    temp += "SELECT eb_persist_currval('eb_locations_id_seq');";
            }
            else
            {
                temp += $"UPDATE eb_locations SET shortname = :shortname_{i}, longname = :longname_{i}, image = :image_{i}, meta_json = :meta_json_{i} WHERE eb_ver_id = :{tbl}_eb_ver_id AND eb_data_id = :{tbl}_id;";
            }
            _extqry = temp + _extqry; //location must be created before user creation
            i++;
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

        public override string GetValueFromDOMJSfn
        {
            get
            {
                return @"
                if (!this.hasOwnProperty('_finalObj'))
                    this._finalObj = {};
                let metaObj = {};
                $.each(this.Fields.$values, function (i, obj) {
                    if (obj.ControlName !== '') {
                        if (obj.Name === 'shortname' || obj.Name === 'longname' || obj.Name === 'image')
                            this._finalObj[obj.Name] = obj.Control.getValueFromDOM();
                        else
                            metaObj[obj.DisplayName] = obj.Control.getValueFromDOM();
                    }
                }.bind(this));
                this._finalObj['meta_json'] = JSON.stringify(metaObj);
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
                let metaObj = JSON.parse(this._finalObj['meta_json']) || {};
                $.each(this.Fields.$values, function (i, obj) {
                    if (obj.ControlName !== '') {
                        if (obj.Name === 'shortname' || obj.Name === 'longname' || obj.Name === 'image')
                            obj.Control.setValue(this._finalObj[obj.Name]);
                        else if (metaObj.hasOwnProperty(obj.DisplayName))
                            obj.Control.setValue(metaObj[obj.DisplayName]);
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
}
