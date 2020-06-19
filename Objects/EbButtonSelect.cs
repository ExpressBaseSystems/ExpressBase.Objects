using ExpressBase.Common;
using ExpressBase.Common.Constants;
using ExpressBase.Common.Extensions;
using ExpressBase.Common.LocationNSolution;
using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using ExpressBase.Common.Structures;
using ExpressBase.Objects.Objects.DVRelated;
using ExpressBase.Objects.ServiceStack_Artifacts;
using Newtonsoft.Json;
using ServiceStack;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using static Dropbox.Api.FileProperties.TemplateOwnerType;

namespace ExpressBase.Objects
{
    [EnableInBuilder(BuilderType.BotForm)]
    public class EbButtonSelect : EbControlUI
    {

        public EbButtonSelect()
        {
            this.Buttons = new List<EbButtonSelectOption>();
        }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl)]
        [HideInPropertyGrid]
        public override EbDbTypes EbDbType
        {
            get
            {
                return IsDynamic ? ValueMember.Type : EbDbTypes.String;
            }
        }

        public override string JustSetValueJSfn
        {
            get
            {
                return JSFnsConstants.EbSimpleSelect_JustSetValueJSfn;
            }
            set { }
        }

        public override string SetValueJSfn
        {
            get
            {
                return JSFnsConstants.SS_SetValueJSfn;
            }
            set { }
        }

        public override string GetValueFromDOMJSfn
        {
            get
            {
                return @"return $('#' + this.EbSid_CtxId).find('[active=true]').attr('value');";
            }
            set { }
        }

        public override string IsRequiredOKJSfn
        {
            get
            {
                return @"return true;";
            }
            set { }
        }

        public override string GetDisplayMemberFromDOMJSfn
        {
            get
            {
                return @"return $('#' + this.EbSid_CtxId).find('[active=true]').attr('dm');";
            }
            set { }
        }

        public override string DisableJSfn
        {
            get
            {
                return JSFnsConstants.SS_DisableJSfn;
            }
            set { }
        }

        public override string EnableJSfn
        {
            get
            {
                return JSFnsConstants.SS_EnableJSfn;
            }
            set { }
        }

        [HideInPropertyGrid]
        [JsonIgnore]
        public override string ToolIconHtml { get { return "<i class='fa fa-tasks'></i>"; } set { } }

        [EnableInBuilder(BuilderType.BotForm)]
        [OSE_ObjectTypes(EbObjectTypes.iDataReader)]
        [PropertyEditor(PropertyEditorType.ObjectSelector)]
        [PropertyGroup(PGConstants.DATA_SETTINGS)]
        [Alias("Data Reader")]
        public string DataSourceId { get; set; }

        [EnableInBuilder(BuilderType.BotForm)]
        [PropertyGroup(PGConstants.EXTENDED)]
        public bool MultiSelect { get; set; }

        [EnableInBuilder(BuilderType.BotForm)]
        [PropertyGroup(PGConstants.EXTENDED)]
        public int MaxLimit { get; set; }

        [EnableInBuilder(BuilderType.BotForm)]
        [PropertyGroup(PGConstants.EXTENDED)]
        public int MinLimit { get; set; }

        [EnableInBuilder(BuilderType.BotForm)]
        [PropertyGroup(PGConstants.HELP)]
        [HelpText("specifies a short hint that describes the expected value of an input field (e.g. a sample value or a short description of the expected format)")]
        public string PlaceHolder { get; set; }

        [EnableInBuilder(BuilderType.BotForm)]
        [MetaOnly]
        public DVColumnCollection Columns { get; set; }

        [EnableInBuilder(BuilderType.BotForm)]
        [PropertyEditor(PropertyEditorType.CollectionFrmSrc, "Columns", 1)]
        [OnChangeExec(@"if (this.Columns && this.Columns.$values.length === 0 ){pg.MakeReadOnly('ValueMember');} else {pg.MakeReadWrite('ValueMember');}")]
        [PropertyPriority(69)]
        [PropertyGroup(PGConstants.DATA_SETTINGS)]
        public DVBaseColumn ValueMember { get; set; }

        [EnableInBuilder(BuilderType.BotForm)]
        [PropertyEditor(PropertyEditorType.Collection)]
        [PropertyGroup(PGConstants.CORE)]
        public List<EbButtonSelectOption> Buttons { get; set; }

        [EnableInBuilder(BuilderType.BotForm)]
        [PropertyEditor(PropertyEditorType.CollectionFrmSrc, "Columns", 1)]
        [PropertyPriority(68)]
        [PropertyGroup(PGConstants.DATA_SETTINGS)]
        [OnChangeExec(@"if (this.Columns && this.Columns.$values.length === 0 ){pg.MakeReadOnly('DisplayMember');} else {pg.MakeReadWrite('DisplayMember');}")]
        public DVBaseColumn DisplayMember { get; set; }

        //[EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl)]
        //public int Value { get; set; }

        //[HideInPropertyGrid]
        //[EnableInBuilder(BuilderType.BotForm)]
        //public override bool IsReadOnly { get => this.IsDisable; }

        [EnableInBuilder(BuilderType.BotForm)]
        [HideInPropertyGrid]
        public override bool IsFullViewContol { get => false; set => base.IsFullViewContol = value; }

        [EnableInBuilder(BuilderType.BotForm)]
        [PropertyEditor(PropertyEditorType.Boolean)]
        [PropertyGroup(PGConstants.CORE)]
        [OnChangeExec(@"if(this.IsDynamic === true){pg.ShowProperty('DataSourceId');pg.ShowProperty('ValueMember');pg.ShowProperty('DisplayMember');pg.HideProperty('Options');}
else{pg.HideProperty('DataSourceId');pg.HideProperty('ValueMember');pg.HideProperty('DisplayMember');pg.ShowProperty('Options');}")]
        public bool IsDynamic { get; set; }

        private string _buttonsHtml = string.Empty;
        [JsonIgnore]
        public string ButtonsHtml
        {
            get
            {
                if (_buttonsHtml.Equals(string.Empty))
                {
                    _buttonsHtml = string.Empty;
                    if (!this.IsDynamic)
                    {
                        for (int i = 0; i < this.Buttons.Count; i++)
                        {
                            EbButtonSelectOption button = this.Buttons[i];
                            _buttonsHtml += string.Format(@"
<div  value='{0}' dm='{1}' class='bs-btn' active='false' tabindex='1'>
    {1}
</div>
", button.Value, button.DisplayName);
                        }

                    }
                }
                return _buttonsHtml;
            }
            set { }
        }

        //        [JsonIgnore]
        //        public string ButtonsHtml
        //        {
        //            get
        //            {
        //                if (_buttonsHtml.Equals(string.Empty))
        //                {
        //                    _buttonsHtml = string.Empty;
        //                    if (!this.IsDynamic)
        //                    {
        //                        for (int i = 0; i < this.Buttons.Count; i++)
        //                        {
        //                            EbButtonSelectOption button = this.Buttons[i];
        //                            _buttonsHtml += string.Format(@"
        //<div  value='{0}' class='bs-btn' active='false' tabindex='1'>
        //    <div class='bs-txt-wrap'>
        //        <div class='bs-text'>{1}</div>
        //    </div>
        //    <div class='bs-ckbx-wrap'>
        //        <input type='checkbox'>
        //    </div>
        //</div>
        //", button.Value, button.DisplayName);
        //                        }

        //                    }
        //                }
        //                return _buttonsHtml + @"
        //<div class='bs-btn bs-btn-send' tabindex='1'>
        //    <div class='bs-txt-wrap'>
        //        <div class='bs-text'>OK</div>
        //    </div>
        //    <div class='bs-ckbx-wrap'>
        //       <i class='fa fa-check' aria-hidden='true'></i>
        //    </div>
        //</div>";
        //            }
        //            set { }
        //        }

        public void InitFromDataBase(JsonServiceClient ServiceClient)
        {
            string _html = string.Empty;
            if (this.IsDynamic)
            {
                var result = ServiceClient.Get<FDDataResponse>(new FDDataRequest { RefId = this.DataSourceId });
                foreach (EbDataRow option in result.Data)
                {
                    _html += string.Format("<option value='{0}'>{1}</option>", option[this.ValueMember.Data], option[this.DisplayMember.Data]);
                }
            }
            _buttonsHtml = _html;
            this.ButtonsHtml = _html;
        }


        [OnDeserialized]
        public void OnDeserializedMethod(StreamingContext context)
        {
            this.BareControlHtml = this.GetBareHtml();
            this.BareControlHtml4Bot = this.BareControlHtml;
            this.ObjType = this.GetType().Name.Substring(2, this.GetType().Name.Length - 2);
        }

        public override string GetHtml4Bot()
        {
            return ReplacePropsInHTML(HtmlConstants.CONTROL_WRAPER_HTML4BOT);
        }

        public override string GetDesignHtml()
        {
            //        return @"

            //<div id='cont_@name@' Ctype='SimpleSelect' class='Eb-ctrlContainer' eb-hidden='@isHidden@'>
            //    <div id='@name@' class='btn-group bootstrap-select show-tick' style='width: 100%;'><button type='button' class='btn dropdown-toggle btn-default'><span class='filter-option pull-left'>Simple Select</span>&nbsp;<span class='bs-caret'><span class='caret'></span></span></button></div>
            //</div>".RemoveCR().GraveAccentQuoted();//GetHtmlHelper(RenderMode.Developer).RemoveCR().DoubleQuoted();
            return GetHtml().RemoveCR().DoubleQuoted(); ;
        }

        public override string GetHtml()
        {
            string EbCtrlHTML = HtmlConstants.CONTROL_WRAPER_HTML4WEB
                 .Replace("@LabelForeColor ", "color:" + (LabelForeColor ?? "@LabelForeColor ") + ";")
                 .Replace("@LabelBackColor ", "background-color:" + (LabelBackColor ?? "@LabelBackColor ") + ";");

            return ReplacePropsInHTML(EbCtrlHTML);
        }

        public override string GetBareHtml()
        {
            return @"
        <div id='@ebsid@' class='buttonselect-cont' title='@PlaceHolder@' @multiple@  name='@ebsid@' data-ebtype='@data-ebtype@'>
            @buttons@
        </div>"
.Replace("@ebsid@", String.IsNullOrEmpty(this.EbSid_CtxId) ? "@ebsid@" : this.EbSid_CtxId)
.Replace("@name@", this.Name)
.Replace("@multiple@", this.MultiSelect ? "multiple" : "")
.Replace("@buttons@", this.ButtonsHtml)
.Replace("@PlaceHolder@", this.PlaceHolder)
.Replace("@data-ebtype@", "16");
        }
    }


    [EnableInBuilder(BuilderType.BotForm)]
    [HideInToolBox]
    [UsedWithTopObjectParent(typeof(EbObject))]
    public class EbButtonSelectOption
    {
        public EbButtonSelectOption() { }

        [HideInPropertyGrid]
        [EnableInBuilder(BuilderType.BotForm)]
        public string EbSid { get; set; }

        [EnableInBuilder(BuilderType.BotForm)]
        [PropertyGroup(PGConstants.CORE)]
        public string Name { get; set; }

        [EnableInBuilder(BuilderType.BotForm)]
        [PropertyGroup(PGConstants.CORE)]
        public string Value { get; set; }

        [EnableInBuilder(BuilderType.BotForm)]
        [PropertyGroup(PGConstants.CORE)]
        [Alias("Button Text")]
        public string DisplayName { get; set; }
    }
}
