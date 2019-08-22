using ExpressBase.Common;
using ExpressBase.Common.Extensions;
using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using ExpressBase.Common.Structures;
using ExpressBase.Objects.Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Runtime.Serialization;
using System.Text;
using ExpressBase.Security;

namespace ExpressBase.Objects
{
    [EnableInBuilder(BuilderType.WebForm)]
    public class EbAutoId : EbControlUI
    {
        #region Hidden Properties

        [HideInPropertyGrid]
        public override bool IsDisable { get; set; }

        [HideInPropertyGrid]
        public override bool DoNotPersist { get; set; }

        [HideInPropertyGrid]
        public override EbScript ValueExpr { get; set; }

        [HideInPropertyGrid]
        public override EbScript DefaultValueExpression { get; set; }

        [HideInPropertyGrid]
        public override List<EbValidator> Validators { get; set; }

        [HideInPropertyGrid]
        public override EbScript OnChangeFn { get; set; }

        [HideInPropertyGrid]
        public override EbScript VisibleExpr { get; set; }

        #endregion

        public EbAutoId()
        {
        }

        [OnDeserialized]
        public void OnDeserializedMethod(StreamingContext context)
        {
            this.BareControlHtml = this.GetBareHtml();
            this.ObjType = this.GetType().Name.Substring(2, this.GetType().Name.Length - 2);
            //this.Name = "eb_auto_id";
        }

        [EnableInBuilder(BuilderType.WebForm)]
        [PropertyEditor(PropertyEditorType.Expandable)]
        public EbAutoIdPattern Pattern { get; set; }
        
        [HideInPropertyGrid]
        public override EbDbTypes EbDbType { get { return EbDbTypes.String; } set { } }

        //HideInPropertyGrid
        //public string OnChange { get; set; }
        public override bool Hidden { get => base.Hidden; set => base.Hidden = value; }
        public override bool Required { get => base.Required; set => base.Required = value; }
        public override bool Unique { get => base.Unique; set => base.Unique = value; }
        //public override dynamic DefaultValue { get; set; }
        //public List<EbValidator> Validators { get; set; }

        [HideInPropertyGrid]
        [JsonIgnore]
        public override string ToolIconHtml { get { return "<i class='fa fa-id-card-o'></i>"; } set { } }

        //public override string GetToolHtml()
        //{
        //    return @"<div eb-type='@toolName' class='tool'><i class='fa fa-id-card-o'></i>  @toolName</div>".Replace("@toolName", this.GetType().Name.Substring(2));
        //}

        public override string GetHtml()
        {
            return GetHtmlHelper();
        }

        private string GetHtmlHelper()
        {
            string WraperHtml = @"
        <div id='cont_@ebsid@' ebsid='@ebsid@' name='@name@' class='Eb-ctrlContainer' @childOf@ ctype='@type@' eb-hidden='@isHidden@'>
            <i class='fa fa-key' aria-hidden='true' style='font-size: 14px;'></i>
            <span class='eb-ctrl-label' ui-label id='@ebsidLbl'>@Label@ </span> @req@ 
                <div  id='@ebsid@Wraper' class='ctrl-cover'>
                    @barehtml@
                </div>
            <span class='helpText' ui-helptxt >@helpText@ </span>
        </div>";

            string EbCtrlHTML = WraperHtml
               .Replace("@LabelForeColor ", "color:" + (LabelForeColor ?? "@LabelForeColor ") + ";")
               .Replace("@LabelBackColor ", "background-color:" + (LabelBackColor ?? "@LabelBackColor ") + ";");

            return ReplacePropsInHTML(EbCtrlHTML);
        }

        public override string GetDesignHtml()
        {
            string _html = null;

            if (this.Name == null) //if in new mode
                _html = GetHtml();

            else //if edit mode
                _html = Wrap4Developer(GetHtml());

            return _html.RemoveCR().GraveAccentQuoted();
        }

        public string Wrap4Developer(string EbCtrlHTML)
        {
            return @"<div class='controlTile' tabindex='1' onclick='event.stopPropagation();$(this).focus()'>
                                <div class='ctrlHead' style='display:none;'>
                                    <i class='fa fa-arrows moveBtn' aria-hidden='true'></i>
                                    <a href='#' class='close' style='cursor:default' data-dismiss='alert' aria-label='close' title='close'>×</a>
                                </div>"
                                    + EbCtrlHTML
                        + "</div>";
        }

        public override string GetBareHtml()
        {
            return @"
            <input id='@ebsid@' data-ebtype='@data-ebtype@'  data-toggle='tooltip' title='@toolTipText@' class='date' type='text' name='@name@' autocomplete = 'off' @value@ @tabIndex@ style='font-size: 14px; width:100%; @BackColor@ @ForeColor@ display:inline-block; @fontStyle@ @readOnlyString@ @required@ @placeHolder@ disabled />
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
        
        public override string EnableJSfn { get { return @""; } set { } }

        public override bool ParameterizeControl(IDatabase DataDB, List<DbParameter> param, string tbl, SingleColumn rField, bool ins, ref int i, ref string _col, ref string _val, ref string _extqry, User usr)
        {
            if (ins)
            {
                _col += string.Concat(rField.Name, ", ");
                _val += string.Format("CONCAT(:{0}_{1}, (SELECT LPAD(CAST((COUNT(*) + 1) AS CHAR(12)), {2}, '0') FROM {3} WHERE {0} LIKE '{4}%')),", rField.Name, i, this.Pattern.SerialLength, tbl, rField.Value);
                param.Add(DataDB.GetNewParameter(rField.Name + "_" + i, (EbDbTypes)rField.Type, rField.Value));
                i++;
                return true;
            }
            return false;
        }
    }

    [EnableInBuilder(BuilderType.WebForm)]
    public class EbAutoIdPattern
    {
        [EnableInBuilder(BuilderType.WebForm)]
        public string sPattern { get; set; }

        [EnableInBuilder(BuilderType.WebForm)]
        public int SerialLength { get; set; }
    }
}
