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
using System.Runtime.Serialization;
using System.Text;

namespace ExpressBase.Objects.Objects
{
    [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.UserControl)]
    public class EbExportButton : EbControlUI
    {
        public EbExportButton() { }

        [OnDeserialized]
        public void OnDeserializedMethod(StreamingContext context)
        {
            this.BareControlHtml = this.GetBareHtml();
            this.ObjType = this.GetType().Name.Substring(2, this.GetType().Name.Length - 2);
        }

        public override string ToolIconHtml { get { return "<i class='fa fa-external-link'></i>"; } set { } }

        public override string ToolNameAlias { get { return "Export Button"; } set { } }

        public override string ToolHelpText { get { return "Export Button"; } set { } }

        [EnableInBuilder(BuilderType.WebForm)]
        [HideInPropertyGrid]
        public override EbDbTypes EbDbType { get { return EbDbTypes.String; } }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.UserControl)]
        [PropertyEditor(PropertyEditorType.ObjectSelector)]
        [PropertyGroup(PGConstants.CORE)]
        [OSE_ObjectTypes(EbObjectTypes.iWebForm)]
        [Alias("Destination Form")]
        public string FormRefId { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.UserControl)]
        [PropertyGroup(PGConstants.CORE)]
        public bool OpenInNewTab { get; set; }

        [EnableInBuilder(BuilderType.WebForm)]
        [PropertyEditor(PropertyEditorType.Collection)]
        [PropertyGroup(PGConstants.DATA)]
        public List<DataFlowMapAbstract> DataFlowMap { get; set; }

        [EnableInBuilder(BuilderType.WebForm)]
        [PropertyGroup(PGConstants.DATA)]
        public bool ExportMapedDataOnly { get; set; }

        [EnableInBuilder(BuilderType.WebForm)]
        [PropertyEditor(PropertyEditorType.ScriptEditorJS, PropertyEditorType.ScriptEditorSQ)]
        [Alias("Text Expression")]
        [HelpText("Expression for button text.")]
        public override EbScript ValueExpr { get; set; }

        [EnableInBuilder(BuilderType.WebForm)]
        [PropertyEditor(PropertyEditorType.ScriptEditorJS, PropertyEditorType.ScriptEditorSQ)]
        [Alias("Default Text Expression")]
        [HelpText("Default expression for button text.")]
        public override EbScript DefaultValueExpression { get; set; }

        [EnableInBuilder(BuilderType.WebForm)]
        [PropertyGroup(PGConstants.BEHAVIOR)]
        public bool DisableInEditMode { get; set; }

        [JsonIgnore]
        public override string DisableJSfn
        {
            get { return string.Empty; }
            set { }
        }

        [JsonIgnore]
        public override string EnableJSfn
        {
            get { return string.Empty; }
            set { }
        }


        public override string GetBareHtml()
        {
            return @"<div id='@ebsid@' disabled='disabled' class='btn btn-success' style='width:100%; cursor: pointer; @backColor @foreColor'><span>@Label@ </span><i class='fa fa-external-link'></i></div>"
                .Replace("@ebsid@", this.EbSid_CtxId)
                .Replace("@Label@", this.Label ?? "Export")
.Replace("@tabIndex", "tabindex='" + this.TabIndex + "'")
.Replace("@backColor", "background-color:" + this.BackColor + ";")
.Replace("@foreColor", "color:" + this.ForeColor + ";");
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

        public override SingleColumn GetSingleColumn(User UserObj, Eb_Solution SoluObj, object Value, bool Default)
        {
            object _formattedData = Default ? this.Label : Value;
            string _displayMember = Value == null ? string.Empty : Value.ToString();

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

        //--------Hide in property grid------------

        [EnableInBuilder(BuilderType.WebForm)]
        [HideInPropertyGrid]
        public override bool Unique { get; set; }

        [EnableInBuilder(BuilderType.WebForm)]
        [HideInPropertyGrid]
        public override List<EbValidator> Validators { get; set; }

        public override bool SelfTrigger { get; set; }

        [EnableInBuilder(BuilderType.WebForm)]
        [HideInPropertyGrid]
        public override bool DoNotPersist { get { return true; } set { } }

        [EnableInBuilder(BuilderType.WebForm)]
        [HideInPropertyGrid]
        public override bool Required { get; set; }

        [EnableInBuilder(BuilderType.WebForm)]
        [HideInPropertyGrid]
        public override string LabelBackColor { get; set; }

        [EnableInBuilder(BuilderType.WebForm)]
        [HideInPropertyGrid]
        public override string LabelForeColor { get; set; }

        [EnableInBuilder(BuilderType.WebForm)]
        [HideInPropertyGrid]
        public override EbScript OnChangeFn { get; set; }



        [EnableInBuilder(BuilderType.WebForm)]
        [DefaultPropValue("#0f43d6")]
        public override string BackColor { get; set; }

        [EnableInBuilder(BuilderType.WebForm)]
        public override string Label { get; set; }

        [EnableInBuilder(BuilderType.WebForm)]
        [DefaultPropValue("#ffffff")]
        public override string ForeColor { get; set; }

        [EnableInBuilder(BuilderType.WebForm)]
        public override bool IsDisable { get; set; }

        [EnableInBuilder(BuilderType.WebForm)]
        public override string HelpText { get; set; }

        //[EnableInBuilder(BuilderType.WebForm)]
        //public override string ToolTipText { get; set; }

        //[EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl)]
        //[PropertyGroup(PGConstants.EVENTS)]
        //[PropertyEditor(PropertyEditorType.ScriptEditorJS)]
        //[HelpText("Define onClick function.")]
        //public EbScript OnClickFn { get; set; }


        //[JsonIgnore]
        //public override string OnChangeBindJSFn
        //{
        //	get
        //	{
        //		return @"$('#' + this.EbSid_CtxId).on('click', p1);";
        //	}
        //	set { }
        //}
    }
}
