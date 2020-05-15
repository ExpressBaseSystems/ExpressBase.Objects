using ExpressBase.Common;
using ExpressBase.Common.Constants;
using ExpressBase.Common.Extensions;
using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using ExpressBase.Common.Structures;
using ExpressBase.Objects.Helpers;
using Newtonsoft.Json;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace ExpressBase.Objects
{
    public enum EbRadioValueType
    {
        Boolean,
        Integer,
        Text
    }

    [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl)]
    public class EbRadioGroup : EbControlUI
    {
        public EbRadioGroup()
        {
            this.Options = new List<EbRadioOptionAbstract>();
            //this.Options.Add(new EbRadioOption());
            //this.Options.Add(new EbRadioOption());
            //this.Options.CollectionChanged += Options_CollectionChanged;
            this.ValueType = EbRadioValueType.Boolean;
        }

        public override string GetValueFromDOMJSfn
        {
            get
            {
                return @" return $('[name=' + this.EbSid_CtxId + ']:checked').val(); ";
            }
            set { }
        }

        public override string JustSetValueJSfn
        {
            get
            {
                return @" $('input[name = ' + this.EbSid_CtxId + '][value = ""' + p1 + '""]').prop('checked', true)";
            }
            set { }
        }

        public override string SetValueJSfn
        {
            get
            {
                return JustSetValueJSfn + @".trigger('change');";
            }
            set { }
        }
        public override string GetDisplayMemberFromDOMJSfn
        {
            get
            {
                return @"
  return $('[name=' + this.EbSid_CtxId + ']:checked').next('span').text();
                ";
            }
            set { }
        }

        [JsonIgnore]
        public override string OnChangeBindJSFn
        {
            get
            {
                return @"$('input[name = ' + this.EbSid_CtxId + ']').on('change', p1);";
            }
            set { }
        }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl)]
        [HideInPropertyGrid]
        public override EbDbTypes EbDbType { get { return EbDbTypes.String; } }

        [HideInPropertyGrid]
        [EnableInBuilder(BuilderType.BotForm)]
        public override bool IsReadOnly { get => this.ReadOnly; }

        [OnDeserialized]
        public void OnDeserializedMethod(StreamingContext context)
        {
            this.BareControlHtml = this.GetBareHtml();
            this.BareControlHtml4Bot = this.BareControlHtml;
            this.ObjType = this.GetType().Name.Substring(2, this.GetType().Name.Length - 2);
        }

        private void Options_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                if (this.Options.Count == 3 && this.ValueType == EbRadioValueType.Boolean)
                    this.ValueType = EbRadioValueType.Integer;
            }
        }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl)]
        [PropertyGroup("Behavior")]
        public EbRadioValueType ValueType { get; set; }

        [EnableInBuilder(BuilderType.WebForm,BuilderType.DashBoard,BuilderType.Report)]
        [PropertyEditor(PropertyEditorType.IconPicker)]
        [PropertyPriority(100)]
        [PropertyGroup(PGConstants.CORE)]
        public string IconTestProp { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl)]
        [PropertyEditor(PropertyEditorType.Collection)]
        [Alias("Options")]
        public List<EbRadioOptionAbstract> Options { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl)]
        [PropertyEditor(PropertyEditorType.Collection)]
        [Category("Appearance")]
        public bool RenderHorizontally { get; set; }

        [HideInPropertyGrid]
        [JsonIgnore]
        public override string ToolIconHtml { get { return " &#9673; "; } set { } }

        [HideInPropertyGrid]
        [JsonIgnore]
        public override string ToolNameAlias { get { return "Radio Buttons"; } set { } }

        //public override string GetToolHtml()
        //{
        //    return @"<div eb-type='@toolName' class='tool'> &#9673;  @toolName</div>".Replace("@toolName", this.GetType().Name.Substring(2));
        //}

        public override string GetBareHtml()
        {
            string html = "<div id='@ebsid@' class='list-ctrl-box' data-ebtype='3' name='@name@' type='RadioGroup'>";
            foreach (EbRadioOption ec in this.Options)
            {
                ec.GName = this.EbSid_CtxId;
                html += ec.GetHtml()
                    .Replace("@radio-wrap-block@", (this.RenderHorizontally ? "radio-wrap-block" : String.Empty))
                    .Replace("@defaultcheked@", ((this.Options.IndexOf(ec) == 0) ? " checked='checked' " : ""));
            }
            html += "</div>";
            return html
.Replace("@name@", (this.Name != null) ? this.Name : "@name@")
.Replace("@ebsid@", EbSid_CtxId);
        }

        public override string DesignHtml4Bot
        {
            get => @"<div id='' class='radio-wrap @radio-wrap-block@' style='padding: 4px;'>
<input type ='radio' class='eb-radiobtn' id='' @defaultcheked@ value='@value@' name='@gname@'>
<span id='Lbl' class='eb-radiospan' ui-label style='@LabelBackColor @LabelForeColor '> RadioButton1 </span>
</div>
<div id='' class='radio-wrap @radio-wrap-block@' style='padding: 4px;'>
<input type ='radio' class='eb-radiobtn' id='' @defaultcheked@ value='@value@' name='@gname@'>
<span id='Lbl' class='eb-radiospan' ui-label style='@LabelBackColor @LabelForeColor '> RadioButton2  </span>
                </div>";
            set => base.DesignHtml4Bot = value;
        }

        public override string GetHtml4Bot()
        {
            return ReplacePropsInHTML((HtmlConstants.CONTROL_WRAPER_HTML4BOT).Replace("@barehtml@", DesignHtml4Bot));
        }

        public override string GetDesignHtml()
        {
            string EbCtrlHTML = HtmlConstants.CONTROL_WRAPER_HTML4WEB
                .Replace("@barehtml@", @"
                            <div id='@ebsid@' class='radio-wrap @radio-wrap-block@'>
                                <input type ='radio' class='eb-radiobtn' id='@ebsid@' @defaultcheked@ value='@value@' name='@gname@'>
                                <span id='@ebsid@Lbl' class='eb-radiospan' ui-label style='@LabelBackColor @LabelForeColor '> RadioButton1 </span>
                            </div>
                            <div id='@ebsid@' class='radio-wrap @radio-wrap-block@'>
                                <input type ='radio' class='eb-radiobtn' id='@ebsid@' @defaultcheked@ value='@value@' name='@gname@'>
                                <span id='@ebsid@Lbl' class='eb-radiospan' ui-label style='@LabelBackColor @LabelForeColor '> RadioButton2  </span>
                            </div>")
                            .RemoveCR().DoubleQuoted();

            return ReplacePropsInHTML(EbCtrlHTML);
            //return GetHtml().RemoveCR().DoubleQuoted();
        }

        public override string GetHtml()
        {

            //        string EbCtrlHTML = @"
            //    <div id='cont_@ebsid@'  ebsid='@ebsid@'  class='Eb-ctrlContainer' ebsid='@ebsid@' ctype='@type@' eb-hidden='@isHidden@'>
            //        <span class='eb-ctrl-label' ui-label id='@ebsidLbl'>@Label@ </span>
            //            <div  class='@ebsid@Wraper'>
            //                @barehtml@
            //            </div>
            //        <span class='helpText' ui-helptxt >@HelpText@ </span>
            //    </div>"
            //.Replace("@LabelForeColor ", "color:" + (LabelForeColor ?? "@LabelForeColor ") + ";")
            //.Replace("@LabelBackColor ", "background-color:" + (LabelBackColor ?? "@LabelBackColor ") + ";")
            //.Replace("@HelpText@ ", (HelpText ?? ""))
            //.Replace("@Label@ ", (Label ?? ""));
            //        return ReplacePropsInHTML(EbCtrlHTML);


            string EbCtrlHTML = HtmlConstants.CONTROL_WRAPER_HTML4WEB
               .Replace("@LabelForeColor ", "color:" + (LabelForeColor ?? "@LabelForeColor ") + ";")
               .Replace("@LabelBackColor ", "background-color:" + (LabelBackColor ?? "@LabelBackColor ") + ";");

            return ReplacePropsInHTML(EbCtrlHTML);
        }

        public override string GetJsInitFunc()
        {
            return @"
this.Init = function(id)
{
    this.Options.$values.push(new EbObjects.EbRadioOption(id + '_Rd0'));
    this.Options.$values.push(new EbObjects.EbRadioOption(id + '_Rd1'));
};";
        }
    }

    [HideInToolBox]
    [HideInPropertyGrid]
    public class EbRadioOptionAbstract : EbControlUI
    {

    }

    [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl)]
    [HideInToolBox]
    public class EbRadioOption : EbRadioOptionAbstract
    {
        public EbRadioOption() { }

        public override string UIchangeFns
        {
            get
            {
                return @"EbRadioOption = {
                Label : function(elementId, props) {
                        $(`#cont_${ elementId}`).closestInner('[ui-label]').text(props.Label);
                },
            }";
            }
        }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl)]
        [Alias("Option Text")]
        [PropertyGroup(PGConstants.CORE)]
        public override string Label { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl)]
        [Alias("Option Value")]
        [PropertyGroup(PGConstants.CORE)]
        public string Value { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl)]
        [HideInPropertyGrid]
        public string GName { get; set; }

        public override string GetBareHtml()
        {/*onclick=""event.stopPropagation();$('#@ebsid@').prop('checked', true);""*/
            return @"<div id='@ebsid@' class='radio-wrap @radio-wrap-block@' onclick=""event.stopPropagation(); $(this).children('input[type=radio]').prop('checked', true); $(this).children('input[type=radio]').trigger('change');"">
                        <input type ='radio' class='eb-radiobtn' id='@ebsid@' @defaultcheked@ value='@value@' name='@gname@'>
                        <span id='@ebsid@Lbl' class='eb-radiospan' ui-label> @label@  </span>
                    </div>"
.Replace("@name@", this.Name)
.Replace("@ebsid@", this.EbSid_CtxId)
.Replace("@gname@", this.GName)
.Replace("@label@", this.Label)
.Replace("@value@", this.Value);
        }

        public override string GetHtml()
        {
            return this.GetBareHtml(); ;
        }
    }
}