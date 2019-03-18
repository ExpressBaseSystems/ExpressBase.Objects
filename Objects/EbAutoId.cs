using ExpressBase.Common.Extensions;
using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using ExpressBase.Objects.Helpers;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace ExpressBase.Objects
{
    [EnableInBuilder(BuilderType.WebForm)]
    public class EbAutoId : EbControlUI
    {
        public EbAutoId()
        {
        }

        [OnDeserialized]
        public void OnDeserializedMethod(StreamingContext context)
        {
            this.BareControlHtml = this.GetBareHtml();
            this.ObjType = this.GetType().Name.Substring(2, this.GetType().Name.Length - 2);
            this.Name = "eb_auto_id";
        }

        [EnableInBuilder(BuilderType.WebForm)]
        [PropertyEditor(PropertyEditorType.ScriptEditorJS, PropertyEditorType.ScriptEditorCS)]
        public EbScript Prefix { get; set; }

        [EnableInBuilder(BuilderType.WebForm)]
        [PropertyEditor(PropertyEditorType.ScriptEditorCS)]
        public EbScript Suffix { get; set; }

        //HideInPropertyGrid
        public string OnChange { get; set; }
        public override bool Hidden { get => base.Hidden; set => base.Hidden = value; }
        public override bool Required { get => base.Required; set => base.Required = value; }
        public override bool Unique { get => base.Unique; set => base.Unique = value; }
        public string DefaultValue { get; set; }
        public List<EbValidator> Validators { get; set; }

        public override string GetToolHtml()
        {
            return @"<div eb-type='@toolName' class='tool'><i class='fa fa-id-card-o'></i>  @toolName</div>".Replace("@toolName", this.GetType().Name.Substring(2));
        }

        public override string GetHtml()
        {
            return GetHtmlHelper();
        }

        private string GetHtmlHelper()
        {
            string EbCtrlHTML = HtmlConstants.CONTROL_WRAPER_HTML4WEB
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
        <div class='input-group' style='width:100%;'>
            <input id='@ebsid@' data-ebtype='@data-ebtype@'  data-toggle='tooltip' title='@toolTipText@' class='date' type='text' name='@name@' autocomplete = 'off' @value@ @tabIndex@ style='width:100%; @BackColor@ @ForeColor@ display:inline-block; @fontStyle@ @readOnlyString@ @required@ @placeHolder@ disabled />
            <span class='input-group-addon' style='padding: 0px;'> <i id='@ebsid@TglBtn' class='fa  @atchdLbl@' aria-hidden='true' style='padding: 6px 12px;'></i> </span>
        </div>"
.Replace("@name@", (this.Name != null ? this.Name.Trim() : ""))
.Replace("@data-ebtype@", "16")//( (int)this.EbDateType ).ToString())
.Replace("@toolTipText@", this.ToolTipText)
.Replace("@ebsid@", String.IsNullOrEmpty(this.EbSid_CtxId) ? "@ebsid@" : this.EbSid_CtxId)
.Replace("@value@", "")//"value='" + this.Value + "'")
.Replace("@tabIndex@", "tabindex='" + this.TabIndex + "'")
    .Replace("@BackColor@ ", ("background-color:" + ((this.BackColor != null) ? this.BackColor : "@BackColor@ ") + ";"))
    .Replace("@ForeColor@ ", "color:" + ((this.ForeColor != null) ? this.ForeColor : "@ForeColor@ ") + ";")
.Replace("@required@", (this.Required && !this.Hidden ? " required" : string.Empty))
.Replace("@readOnlyString@", this.ReadOnlyString)
.Replace("@placeHolder@", "placeholder=''");
        }
    }
}
