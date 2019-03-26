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

        public override string GetValueJSfn
        {
            get
            {
                return @"
                    return $('[name=' + this.EbSid_CtxId + ']:checked').val();
                ";
            }
            set { }
        }

        public override string SetValueJSfn
        {
            get
            {
                return @"
                     $('input[name = ' + this.EbSid_CtxId + '][value = ' + p1 + ']').prop('checked', true).trigger('change');
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
        public override EbDbTypes EbDbType { get { return EbDbTypes.String; } }

        [HideInPropertyGrid]
        [EnableInBuilder(BuilderType.BotForm)]
        public override bool IsReadOnly { get => this.ReadOnly; }

        [OnDeserialized]
        public void OnDeserializedMethod(StreamingContext context)
        {
            this.BareControlHtml = this.GetBareHtml();
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

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl)]
        [PropertyEditor(PropertyEditorType.Collection)]
        [Alias("Options")]
        public List<EbRadioOptionAbstract> Options { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl)]
        [PropertyEditor(PropertyEditorType.Collection)]
        public bool RenderHorizontally { get; set; }

        public override string GetToolHtml()
        {
            return @"<div eb-type='@toolName' class='tool'> &#9673;  @toolName</div>".Replace("@toolName", this.GetType().Name.Substring(2));
        }

        public override string GetBareHtml()
        {
            string html = "<div id='@ebsid@' data-ebtype='3' name='@name@' style='padding:5px' type='RadioGroup'>";
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

        public override string GetDesignHtml()
        {
            //return @"<div class='btn-group' data-toggle='buttons'> <label class='btn btn-primary active'> <input type='radio' name='options' id='option1' autocomplete='off' checked> Radio 1 </label> <label class='btn btn-primary'> <input type='radio' name='options' id='option2' autocomplete='off'> Radio 2 </label> <label class='btn btn-primary'> <input type='radio' name='options' id='option3' autocomplete='off'> Radio 3 </label> </div>".RemoveCR().DoubleQuoted();
            return GetHtml().RemoveCR().DoubleQuoted();
        }

        public override string GetHtml()
        {

            string EbCtrlHTML = @"
        <div id='cont_@ebsid@'  ebsid='@ebsid@'  class='Eb-ctrlContainer' ebsid='@ebsid@' ctype='@type@' eb-hidden='@isHidden@'>
            <span class='eb-ctrl-label' ui-label id='@ebsidLbl'>@Label@ </span>
                <div  class='@ebsid@Wraper'>
                    @barehtml@
                </div>
            <span class='helpText' ui-helptxt >@HelpText@ </span>
        </div>"
    .Replace("@LabelForeColor ", "color:" + (LabelForeColor ?? "@LabelForeColor ") + ";")
    .Replace("@LabelBackColor ", "background-color:" + (LabelBackColor ?? "@LabelBackColor ") + ";")
    .Replace("@HelpText@ ", (HelpText ?? ""))
    .Replace("@Label@ ", (Label ?? ""));
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
        [OnChangeUIFunction("EbTextBox.Label")]
        public override string Label { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl)]
        public string Value { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl)]
        [HideInPropertyGrid]
        public string GName { get; set; }

        public override string GetBareHtml()
        {/*onclick=""event.stopPropagation();$('#@ebsid@').prop('checked', true);""*/
            return @"<div id='@ebsid@' class='radio-wrap @radio-wrap-block@' onclick=""event.stopPropagation(); $(this).children('input[type=radio]').prop('checked', true); $(this).children('input[type=radio]').trigger('change');"">
                        <input type ='radio' id='@ebsid@' @defaultcheked@ value='@value@' name='@gname@'>
                        <span id='@ebsid@Lbl' ui-label style='@LabelBackColor @LabelForeColor '> @label@  </span>
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
