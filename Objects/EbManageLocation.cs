using ExpressBase.Common;
using ExpressBase.Common.Extensions;
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
    [HideInToolBox]
    public class EbManageLocation : EbControlUI, IEbPlaceHolderControl
    {
        public EbManageLocation()
        {
            Fields = new List<MngUsrLocFieldAbstract>();
        }

        [OnDeserialized]
        public void OnDeserializedMethod(StreamingContext context)
        {
            this.BareControlHtml = this.GetBareHtml();
            this.ObjType = this.GetType().Name.Substring(2, this.GetType().Name.Length - 2);
        }

        public override string ToolIconHtml { get { return "<i class='fa fa-map-marker'></i>"; } set { } }

        public override string ToolNameAlias { get { return "Manage Location"; } set { } }

        [EnableInBuilder(BuilderType.WebForm)]
        public override bool Hidden { get { return true; } }

        [EnableInBuilder(BuilderType.WebForm)]
        [HideInPropertyGrid]
        public override bool IsSysControl { get { return true; } }

        [EnableInBuilder(BuilderType.WebForm)]
        [PropertyEditor(PropertyEditorType.Collection)]
        [ListType(typeof(MngUsrLocFieldAbstract))]
        public List<MngUsrLocFieldAbstract> Fields { get; set; }

        [EnableInBuilder(BuilderType.WebForm)]
        public override EbDbTypes EbDbType { get { return EbDbTypes.String; } }


        public override string GetJsInitFunc()
        {
            return @"
this.Init = function(id)
{
	this.Fields.$values.push(new EbObjects.MngUsrLocField('longname'));
	this.Fields.$values.push(new EbObjects.MngUsrLocField('shortname'));
};";
        }

        public override string GetBareHtml()
        {
            return @"
            <input id='@ebsid@' data-ebtype='@data-ebtype@'  data-toggle='tooltip' title='@toolTipText@' class='date' type='text' name='@name@' autocomplete = 'off' @value@ @tabIndex@ style='width:100%; @BackColor@ @ForeColor@ display:inline-block; @fontStyle@ @readOnlyString@ @required@ @placeHolder@ disabled />
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
.Replace("@readOnlyString@", this.ReadOnlyString)
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

        public IEnumerable<MngUsrLocFieldAbstract> PersistingFields
        {
            get
            {
                return this.Fields.Where(f => !string.IsNullOrEmpty((f as MngUsrLocField).ControlName));
            }
        }

        public string GetSelectQuery()
        {
            return "SELECT id, longname, shortname, image, meta_json FROM eb_locations WHERE eb_ver_id = :eb_ver_id AND eb_data_id = :id;";
        }

        private string GetSaveQuery(bool ins, string param, string mtbl)
        {
            if (ins)
                return $"INSERT INTO eb_locations(longname, shortname, image, meta_json, eb_ver_id, eb_data_id) VALUES({param} :eb_ver_id, eb_currval('{mtbl}_id_seq'));";
            else
                return $"UPDATE eb_locations SET {param} WHERE eb_ver_id = :eb_ver_id AND eb_data_id = :{mtbl}_id;";
        }

        public override bool ParameterizeControl(IDatabase DataDB, List<DbParameter> param, string tbl, SingleColumn cField, bool ins, ref int i, ref string _col, ref string _val, ref string _extqry, User usr, SingleColumn ocF)
        {
            return false;
        }
    }
}
