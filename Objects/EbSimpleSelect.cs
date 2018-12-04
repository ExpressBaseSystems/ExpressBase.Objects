using ExpressBase.Common;
using ExpressBase.Common.Extensions;
using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using ExpressBase.Common.Structures;
using ExpressBase.Data;
using ExpressBase.Objects.Helpers;
using ExpressBase.Objects.Objects.DVRelated;
using ExpressBase.Objects.ServiceStack_Artifacts;
using Newtonsoft.Json;
using ServiceStack;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace ExpressBase.Objects
{

    [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.BotForm, BuilderType.UserControl)]
    public class EbSimpleSelect : EbControlUI
    {

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl)]
        public override EbDbTypes EbDbType
        {
            get
            {
                return IsDynamic ? ValueMember.Type : EbDbTypes.String;
            }
        }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl)]
        [OSE_ObjectTypes(EbObjectTypes.iDataReader)]
        [PropertyEditor(PropertyEditorType.ObjectSelector)]
        public string DataSourceId { get; set; }

        [EnableInBuilder(BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl)]
        [HideInPropertyGrid]
        public DVColumnCollection Columns { get; set; }

        [EnableInBuilder(BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.WebForm, BuilderType.UserControl)]
        [PropertyEditor(PropertyEditorType.CollectionFrmSrc, "Columns", 1)]
        [OnChangeExec(@"if (this.Columns && this.Columns.$values.length === 0 ){pg.MakeReadOnly('ValueMember');} else {pg.MakeReadWrite('ValueMember');}")]
        public DVBaseColumn ValueMember { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl)]
        [PropertyEditor(PropertyEditorType.Collection)]
        [Alias("Options")]
        public List<EbSimpleSelectOption> Options { get; set; }

        [EnableInBuilder(BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.WebForm, BuilderType.UserControl)]
        [PropertyEditor(PropertyEditorType.CollectionFrmSrc, "Columns", 1)]
        [OnChangeExec(@"if (this.Columns && this.Columns.$values.length === 0 ){pg.MakeReadOnly('DisplayMember');} else {pg.MakeReadWrite('DisplayMember');}")]
        public DVBaseColumn DisplayMember { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl)]
        public int Value { get; set; }

        [HideInPropertyGrid]
        [EnableInBuilder(BuilderType.BotForm)]
        public override bool IsReadOnly { get => this.ReadOnly; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl)]
        [PropertyEditor(PropertyEditorType.Boolean)]
        [OnChangeExec(@"if(this.IsDynamic === true){pg.ShowProperty('DataSourceId');pg.ShowProperty('ValueMember');pg.ShowProperty('DisplayMember');pg.HideProperty('Options');}
		else{pg.HideProperty('DataSourceId');pg.HideProperty('ValueMember');pg.HideProperty('DisplayMember');pg.ShowProperty('Options');}")]
        public bool IsDynamic { get; set; }

        public EbSimpleSelect()
        {
            this.Options = new List<EbSimpleSelectOption>();
        }

        private string _optionHtml = string.Empty;
        [JsonIgnore]
        public string OptionHtml
        {
            get
            {
                if (_optionHtml.Equals(string.Empty))
                {
                    _optionHtml = string.Empty;
                    if (!this.IsDynamic)
                    {
                        foreach (EbSimpleSelectOption opt in this.Options)
                        {
                            _optionHtml += string.Format("<option value='{0}'>{1}</option>", opt.Value, opt.Label);
                        }
                    }
                }
                return _optionHtml;
            }
            set { }
        }

        public override string GetToolHtml()
        {
            return @"<div eb-type='@toolName' class='tool'><i class='fa fa-align-justify'></i>  @toolName</div>".Replace("@toolName", this.GetType().Name.Substring(2));
        }



        public void InitFromDataBase(JsonServiceClient ServiceClient)
        {
            //this.DataSourceId = "eb_roby_dev-eb_roby_dev-2-1015-1739";
            string _html = string.Empty;
            if (this.IsDynamic)
            {
                var result = ServiceClient.Get<FDDataResponse>(new FDDataRequest { RefId = this.DataSourceId });
                foreach (EbDataRow option in result.Data)
                {
                    _html += string.Format("<option value='{0}'>{1}</option>", option[this.ValueMember.Data], option[this.DisplayMember.Data]);
                    //_html += string.Format("<option value='{0}'>{1}</option>", option[0].ToString().Trim(), option[0]);
                }
            }
            _optionHtml = _html;
            this.OptionHtml = _html;
        }


        [OnDeserialized]
        public void OnDeserializedMethod(StreamingContext context)
        {
            this.BareControlHtml = this.GetBareHtml();
            this.ObjType = this.GetType().Name.Substring(2, this.GetType().Name.Length - 2);
        }

        public override string GetHead()
        {
            return this.RequiredString + @"
<script>

</script>
";
        }

        public override string GetDesignHtml()
        {
            return @"

    <div id='cont_@name@' Ctype='SimpleSelect' class='Eb-ctrlContainer' style='@hiddenString'>
        <div id='@name@' class='btn-group bootstrap-select show-tick' style='width: 100%;'><button type='button' class='btn dropdown-toggle btn-default'><span class='filter-option pull-left'>Simple Select</span>&nbsp;<span class='bs-caret'><span class='caret'></span></span></button></div>
    </div>".RemoveCR().GraveAccentQuoted();//GetHtmlHelper(RenderMode.Developer).RemoveCR().DoubleQuoted();
        }

        public override string GetHtml()
        {
            return GetHtmlHelper(RenderMode.User);
        }

        public override string GetBareHtml()
        {
            return @"
        <select id='@ebsid@' name='@name@' data-ebtype='@data-ebtype@' style='width: 100%;'>
            @options@
        </select>"
.Replace("@ebsid@", this.EbSid_CtxId)
.Replace("@name@", this.Name)
.Replace("@options@", this.OptionHtml)
.Replace("@data-ebtype@", "16");
        }

        private string GetHtmlHelper(RenderMode mode)
        {
            string EbCtrlHTML = HtmlConstants.CONTROL_WRAPER_HTML4WEB
               .Replace("@LabelForeColor ", "color:" + (LabelForeColor ?? "@LabelForeColor ") + ";")
               .Replace("@LabelBackColor ", "background-color:" + (LabelBackColor ?? "@LabelBackColor ") + ";")
               .Replace("@HelpText@ ", (HelpText ?? ""))
               .Replace("@Label@ ", (Label ?? ""));

            return ReplacePropsInHTML(EbCtrlHTML);

        }
    }

    [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl)]
    [HideInToolBox]
    public class EbSimpleSelectOption : EbControl
    {
        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl)]
        public string Label { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl)]
        public string Value { get; set; }

        public EbSimpleSelectOption() { }
    }
}
