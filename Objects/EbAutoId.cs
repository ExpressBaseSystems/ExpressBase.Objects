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
using ExpressBase.Common.Constants;
using ExpressBase.Objects.ServiceStack_Artifacts;
using System.Net;
using ExpressBase.Objects.WebFormRelated;
using ExpressBase.Common.Data;
using ExpressBase.Common.LocationNSolution;

namespace ExpressBase.Objects
{
    [EnableInBuilder(BuilderType.WebForm)]
    public class EbAutoId : EbControlUI, IEbPlaceHolderControl
    {
        #region Hidden Properties

        [HideInPropertyGrid]
        public override bool IsDisable { get; set; }

        [HideInPropertyGrid]
        public override bool DoNotPersist { get; set; }

        [HideInPropertyGrid]
        public override EbScript ValueExpr { get; set; }

        public override bool SelfTrigger { get; set; }

        [HideInPropertyGrid]
        public override EbScript DefaultValueExpression { get; set; }

        [HideInPropertyGrid]
        public override List<EbValidator> Validators { get; set; }

        [HideInPropertyGrid]
        public override EbScript OnChangeFn { get; set; }

        [HideInPropertyGrid]
        public override EbScript HiddenExpr { get; set; }

        [HideInPropertyGrid]
        public override EbScript DisableExpr { get; set; }

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
        [PropertyGroup(PGConstants.CORE)]
        public EbAutoIdPattern Pattern { get; set; }

        [EnableInBuilder(BuilderType.WebForm)]
        [PropertyEditor(PropertyEditorType.ScriptEditorCS, PropertyEditorType.ScriptEditorSQ)]
        [PropertyGroup(PGConstants.CORE)]
        public EbScript Script { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm)]
        [HideInPropertyGrid]
        public override bool Index { get { return true; } }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm)]
        [HideInPropertyGrid]
        public override EbDbTypes EbDbType { get { return EbDbTypes.String; } set { } }

        public string TableName { get; set; }

        private bool IsSqlExpr { get; set; }

        //HideInPropertyGrid
        //public string OnChange { get; set; }
        public override bool Hidden { get => base.Hidden; set => base.Hidden = value; }
        public override bool Required { get => base.Required; set => base.Required = value; }
        public override bool Unique { get => true; }
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
            string EbCtrlHTML = HtmlConstants.CONTROL_WRAPER_HTML4WEB
               .Replace("@LabelForeColor ", "color:" + (LabelForeColor ?? "@LabelForeColor ") + ";")
               .Replace("@LabelBackColor ", "background-color:" + (LabelBackColor ?? "@LabelBackColor ") + ";");

            return ReplacePropsInHTML(EbCtrlHTML);

            //    string WraperHtml = @"
            //<div id='cont_@ebsid@' ebsid='@ebsid@' name='@name@' class='Eb-ctrlContainer' @childOf@ ctype='@type@' eb-hidden='@isHidden@'>            
            //    <span class='eb-ctrl-label' ui-label id='@ebsidLbl' style='font-weight: 500;'>@Label@ </span> @req@ 
            //        <div  id='@ebsid@Wraper' class='ctrl-cover'>
            //            @barehtml@
            //        </div>
            //    <span class='helpText' ui-helptxt >@helpText@ </span>
            //</div>";

            //    string EbCtrlHTML = WraperHtml
            //       .Replace("@LabelForeColor ", "color:" + (LabelForeColor ?? "@LabelForeColor ") + ";")
            //       .Replace("@LabelBackColor ", "background-color:" + (LabelBackColor ?? "@LabelBackColor ") + ";");

            //    return ReplacePropsInHTML(EbCtrlHTML);
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
            <div class='input-group' style='width:100%;'>
                <span class='input-group-addon' style='@BackColor@ font-size: 18px; color: #aaa;'> <i class='fa fa-key' aria-hidden='true'></i> </span>
                <input id='@ebsid@' data-ebtype='@data-ebtype@'  data-toggle='tooltip' title='@toolTipText@' class='date' type='text' name='@name@' autocomplete = 'off' @value@ @tabIndex@ style='font-weight: 500; width:100%; @BackColor@ @ForeColor@ display:inline-block; @fontStyle@' @required@ @placeHolder@ disabled />
            </div>
            "
.Replace("@name@", (this.Name != null ? this.Name.Trim() : ""))
.Replace("@data-ebtype@", "16")//( (int)this.EbDateType ).ToString())
.Replace("@toolTipText@", this.ToolTipText)
.Replace("@ebsid@", String.IsNullOrEmpty(this.EbSid_CtxId) ? "@ebsid@" : this.EbSid_CtxId)
.Replace("@value@", "")//"value='" + this.Value + "'")
.Replace("@tabIndex@", "tabindex='" + this.TabIndex + "'")
.Replace("@BackColor@", "background-color: #eee !important;border-left: solid 1px #fff;")
    //.Replace("@BackColor@ ", ("background-color:" + ((this.BackColor != null) ? this.BackColor : "@BackColor@ ") + ";"))
    .Replace("@ForeColor@ ", "color:" + ((this.ForeColor != null) ? this.ForeColor : "@ForeColor@ ") + ";")
.Replace("@required@", (this.Required && !this.Hidden ? " required" : string.Empty))
.Replace("@placeHolder@", "placeholder=''");
        }

        public override string EnableJSfn { get { return @""; } set { } }

        public override bool ParameterizeControl(ParameterizeCtrl_Params args, string crudContext)
        {
            if (args.ins)
            {
                args._cols += string.Concat(args.cField.Name, ", ");
                if (this.BypassParameterization)
                {
                    args._vals += Convert.ToString(args.cField.Value) + CharConstants.COMMA + CharConstants.SPACE;
                }
                else
                {
                    this.IsSqlExpr = false;
                    string SqlCode = null;

                    if (!string.IsNullOrWhiteSpace(this.Script?.Code))
                    {
                        EbWebForm WebForm = args.webForm as EbWebForm;
                        if (this.Script.Lang == ScriptingLanguage.CSharp)
                        {
                            if (WebForm.FormGlobals == null)
                                WebForm.FormGlobals = GlobalsGenerator.GetCSharpFormGlobals_NEW(WebForm, WebForm.FormData, WebForm.FormDataBackup, args.DataDB, null, false);

                            args.cField.Value = Convert.ToString(WebForm.ExecuteCSharpScriptNew(this.Script.Code, WebForm.FormGlobals));

                            if (string.IsNullOrWhiteSpace(Convert.ToString(args.cField.Value)))
                                throw new FormException("Unable to process [null return by AutoId script]", (int)HttpStatusCode.InternalServerError, "Null or empty string returned by C# script of AutoId: " + args.cField.Name, "EbAutoId => ParameterizeControl");
                        }
                        else if (this.Script.Lang == ScriptingLanguage.SQL)
                        {
                            this.IsSqlExpr = true;
                            SqlCode = this.Script.Code;
                            List<Param> _params = SqlHelper.GetSqlParams(SqlCode);
                            foreach (Param _p in _params)
                            {
                                if (!EbFormHelper.IsExtraSqlParam(_p.Name, WebForm.TableName))
                                    SqlCode = SqlCode.Replace(_p.Name, _p.Name + crudContext);
                            }
                        }
                        else
                            throw new FormException("Unable to process [Invalid AutoId lang]", (int)HttpStatusCode.InternalServerError, $"Invalid script lang {this.Script.Lang} for AutoId: {args.cField.Name}", "EbAutoId => ParameterizeControl");
                    }

                    if ((string.IsNullOrWhiteSpace(Convert.ToString(args.cField.Value)) && !this.IsSqlExpr) || this.Pattern.SerialLength == 0 || (this.Pattern.PrefixLength == 0 && this.IsSqlExpr))
                        throw new FormException("Unable to process [Invalid AutoId pattern]", (int)HttpStatusCode.InternalServerError, $"Invalid pattern for AutoId: {this.TableName}.{args.cField.Name}", "EbAutoId => ParameterizeControl");

                    if (args.DataDB.Vendor == DatabaseVendors.MYSQL)//Not fixed - rewite using MAX
                        args._vals += string.Format("CONCAT(({1}), (SELECT LPAD(CAST((COUNT(*) + 1) AS CHAR(12)), {2}, '0') FROM {3} tbl WHERE tbl.{0} LIKE ({4}))),",
                            args.cField.Name,
                            this.IsSqlExpr ? SqlCode : $"@{args.cField.Name}_{args.i}",
                            this.Pattern.SerialLength,
                            args.tbl,
                            this.IsSqlExpr ? $"({SqlCode}) || '%'" : $"'args.cField.Value%'");
                    else
                        args._vals += string.Format(@"
(({1}) || 
  COALESCE
  (
    (
      SELECT LPAD((SUBSTRING(MAX({0}) FROM {5} FOR {2}) :: INTEGER + 1) :: TEXT, {2}, '0') 
      FROM {3} 
      WHERE {0} LIKE ({4}) AND 
      SUBSTRING({0} FROM {5} FOR {2}) ~ '^\d+$'
    ), 
    LPAD('1', {2}, '0')
  )
),".RemoveCR(),
                            args.cField.Name,//0
                            this.IsSqlExpr ? SqlCode : $"@{args.cField.Name}_{args.i}",//1
                            this.Pattern.SerialLength,//2
                            args.tbl,//3
                            this.IsSqlExpr ? $"({SqlCode}) || '{new string('_', this.Pattern.SerialLength)}'" : $"'{args.cField.Value}{new string('_', this.Pattern.SerialLength)}'",//4
                            this.IsSqlExpr ? (this.Pattern.PrefixLength + 1) : (Convert.ToString(args.cField.Value).Length + 1));//5

                    if (!this.IsSqlExpr)
                        args.param.Add(args.DataDB.GetNewParameter(args.cField.Name + "_" + args.i, (EbDbTypes)args.cField.Type, Convert.ToString(args.cField.Value)));
                }
                args.i++;
                return true;
            }
            return false;
        }

        public override SingleColumn GetSingleColumn(User UserObj, Eb_Solution SoluObj, object Value, bool Default)
        {
            string _displayMember = Value == null ? string.Empty : Convert.ToString(Value);
            object _formattedData = Value == null ? null : _displayMember;

            if (this.IsSqlExpr && !Default)
            {
                this.IsSqlExpr = false;
                if (string.IsNullOrWhiteSpace(_displayMember))
                    throw new FormException("AutoId Generation Failed! Please try again.", (int)HttpStatusCode.InternalServerError, $"Invalid AutoId inserted: {Value}", "EbAutoId => GetSingleColumn");
            }
            return new SingleColumn()
            {
                Name = this.Name,
                Type = (int)this.EbDbType,
                Value = _formattedData,
                Control = this,
                ObjType = this.ObjType,
                F = _displayMember
            };
        }
    }

    [EnableInBuilder(BuilderType.WebForm)]
    public class EbAutoIdPattern
    {
        [EnableInBuilder(BuilderType.WebForm)]
        [Alias("Prefix")]
        public string sPattern { get; set; }

        [EnableInBuilder(BuilderType.WebForm)]
        public int PrefixLength { get; set; }

        [EnableInBuilder(BuilderType.WebForm)]
        public int SerialLength { get; set; }
    }
}
