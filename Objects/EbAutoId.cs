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

        [HideInPropertyGrid]
        public override EbDbTypes EbDbType { get { return EbDbTypes.String; } set { } }

        public string TableName { get; set; }

        //hint: New mode - EbAutoId - DataPusher - Dest Form
        [JsonIgnore]
        public bool BypassParameterization { get; set; }

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

        public override bool ParameterizeControl(ParameterizeCtrl_Params args)
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
                    bool isSql = false;

                    if (!string.IsNullOrWhiteSpace(this.Script?.Code))
                    {
                        if (this.Script.Lang == ScriptingLanguage.CSharp)
                        {
                            EbWebForm WebForm = args.webForm as EbWebForm;
                            if (WebForm.FormGlobals == null)
                                WebForm.FormGlobals = GlobalsGenerator.GetCSharpFormGlobals_NEW(WebForm, WebForm.FormData, WebForm.FormDataBackup, args.DataDB);

                            args.cField.Value = Convert.ToString(WebForm.ExecuteCSharpScriptNew(this.Script.Code, WebForm.FormGlobals));

                            if (string.IsNullOrWhiteSpace(Convert.ToString(args.cField.Value)))
                                throw new FormException("Unable to process", (int)HttpStatusCode.InternalServerError, "Null or empty string returned by C# script of AutoId: " + args.cField.Name, "EbAutoId => ParameterizeControl");
                        }
                        else if (this.Script.Lang == ScriptingLanguage.SQL)
                            isSql = true;
                        else
                            throw new FormException("Unable to process", (int)HttpStatusCode.InternalServerError, $"Invalid script lang {this.Script.Lang} for AutoId: {args.cField.Name}", "EbAutoId => ParameterizeControl");
                    }

                    if (string.IsNullOrWhiteSpace(Convert.ToString(args.cField.Value)) || this.Pattern.SerialLength == 0)
                        throw new FormException("Unable to process", (int)HttpStatusCode.InternalServerError, "Invalid pattern for AutoId: " + args.cField.Name, "EbAutoId => ParameterizeControl");

                    if (args.DataDB.Vendor == DatabaseVendors.MYSQL)//Not fixed - rewite using MAX
                        args._vals += string.Format("CONCAT(({1}), (SELECT LPAD(CAST((COUNT(*) + 1) AS CHAR(12)), {2}, '0') FROM {3} tbl WHERE tbl.{0} LIKE ({4}))),",
                            args.cField.Name,
                            isSql ? this.Script.Code : $"@{args.cField.Name}_{args.i}",
                            this.Pattern.SerialLength,
                            args.tbl,
                            isSql ? $"({this.Script.Code}) || '%'" : $"'args.cField.Value%'");
                    else
                        args._vals += string.Format("(({1}) || COALESCE((SELECT LPAD((RIGHT(MAX({0}), {2}) :: INTEGER + 1) :: TEXT, {2}, '0') FROM {3} WHERE {0} LIKE ({4}) AND LENGTH(REGEXP_REPLACE(RIGHT({0}, {2}), '\\D','','g')) = {2}), LPAD('1', {2}, '0'))),",
                            args.cField.Name,
                            isSql ? this.Script.Code : $"@{args.cField.Name}_{args.i}",
                            this.Pattern.SerialLength,
                            args.tbl,
                            isSql ? $"({this.Script.Code}) || '%'" : $"'{args.cField.Value}%'");

                    if (!isSql)
                        args.param.Add(args.DataDB.GetNewParameter(args.cField.Name + "_" + args.i, (EbDbTypes)args.cField.Type, Convert.ToString(args.cField.Value)));
                }
                args.i++;
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
