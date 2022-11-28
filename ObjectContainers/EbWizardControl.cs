using ExpressBase.Common.Constants;
using ExpressBase.Common.Extensions;
using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ExpressBase.Objects
{
    [EnableInBuilder(BuilderType.WebForm, BuilderType.UserControl)]
    public class EbWizardControl : EbControlContainer
    {
        public EbWizardControl()
        {

            this.Controls = new List<EbControl>();
            this.ObjType = this.GetType().Name.Substring(2, this.GetType().Name.Length - 2);
        }

        [HideInPropertyGrid]
        [JsonIgnore]
        public override string ToolIconHtml { get { return "<i class='fa fa-window-restore'></i>"; } set { } }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.UserControl)]
        [PropertyEditor(PropertyEditorType.Collection)]
        [Alias("Steps")]
        [PropertyGroup(PGConstants.CORE)]
        [UIproperty]
        [PropertyPriority(95)]
        [ListType(typeof(EbWizardStep))]
        [OnChangeUIFunction("EbTabControl.adjustPanesHeightToHighest")]
        public override List<EbControl> Controls { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.UserControl)]
        [DefaultPropValue("150")]
        [PropertyGroup(PGConstants.APPEARANCE)]
        [Alias("Minimum height")]
        public int MinHeight { get; set; }

        public override string GetDesignHtml()
        {
            this.Controls = new List<EbControl>();
            this.Controls.Add(new EbWizardStep { Name = "EbWizard0step1", EbSid_CtxId = "EbWizard0step1", EbSid = "EbWizard0step1", Title = "Step1" });
            return GetHtml().RemoveCR().DoubleQuoted(); ;
        }

        public override string GetJsInitFunc()
        {
            return @"
this.Init = function(id)
{
    let step = new EbObjects.EbWizardStep(this.EbSid + 'step0');
    step.Name = 'step0';
    step.Title = 'step0';
    this.Controls.$values.push(step);
};";
        }


        public override string GetHtml()
        {
            string TabBtnHtml = @"
<div id='cont_@ebsid@' ebsid='@ebsid@' class='Eb-ctrlContainer' Ctype='WizardControl'>
    <div class='RenderAsWizard'>
        <ul class='nav'>".Replace("@ebsid@", EbSid);
            string stepContentHtml = @"
            <div class='tab-content' style='@height@'>"
            .Replace("@height@", this.MinHeight > 0 ? $"height: {this.Height}px;" : "");

            foreach (EbWizardStep tab in Controls)
            {
                TabBtnHtml += @"
            <li class='nav-item' li-of='@ebsid@' ebsid='@ebsid@' style='@display@'>
                <a class='nav-link ppbtn-cont' href='#@ebsid@'  data-toggle='wizard'>
                    <span class='eb-label-editable'>@title@</span>
                    <div class='eb-tab-warn-icon-cont'><i class='icofont-warning-alt'></i></div>
                    <input id='@ebsid@lbltxtb' class='eb-lbltxtb' type='text'/>@ppbtn@
                    <div class='ebtab-close-btn eb-fb-icon' title='Remove'><i class='fa fa-times' aria-hidden='true'></i></div>
                </a>
            </li>".Replace("@display@", tab.Hidden && this.IsRenderMode ? "display: none;" : string.Empty)
            .Replace("@title@", tab.Title)
            .Replace("@ppbtn@", Common.HtmlConstants.CONT_PROP_BTN)
            .Replace("@ebsid@", tab.EbSid_CtxId);
            }

            TabBtnHtml += @"
        </ul><div class='wiz-addbtn'><i class='fa fa-plus' aria-hidden='true'></i></div>";



            foreach (EbControl step in Controls)
                stepContentHtml += step.GetHtml();

            stepContentHtml += "</div></div></div>";

            return string.Concat(TabBtnHtml, stepContentHtml);
        }
    }

    [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.UserControl)]
    [HideInToolBox]
    public class EbWizardStep : EbControlContainer
    {
        [JsonIgnore]
        public override EbScript OnChangeFn { get; set; }

        public EbWizardStep()
        {
            this.Controls = new List<EbControl>();
            this.ObjType = this.GetType().Name.Substring(2, this.GetType().Name.Length - 2);
        }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.UserControl)]
        [HideInPropertyGrid]
        public override List<EbControl> Controls { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.UserControl)]
        [PropertyGroup(PGConstants.CORE)]
        [OnChangeUIFunction("EbTabControl.label")]
        [PropertyEditor(PropertyEditorType.MultiLanguageKeySelector)]
        [PropertyPriority(70)]
        public string Title { get; set; }

        [JsonIgnore]
        public override string Label { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.UserControl)]
        [PropertyEditor(PropertyEditorType.ScriptEditorJS)]
        public override EbScript HiddenExpr { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.UserControl)]
        public override bool Hidden { get; set; }

        public override string GetHtml()
        {
            string html = "<div id='@ebsid@' ebsid='@ebsid@' ctype='@objtype@' class='tab-pane ebcont-ctrl ebcont-inner'  role='tabpanel'>";

            foreach (EbControl ec in this.Controls)
                html += ec.GetHtml();

            return (html + "</div>")
                .Replace("@name@", this.Name)
                .Replace("@ebsid@", this.EbSid)
                .Replace("@objtype@", this.ObjType);
        }

        [JsonIgnore]
        public override string HideJSfn
        {
            get { return @"
debugger;
var li = $('li[ebsid=' + this.EbSid_CtxId+']'); 
li.hide();
var wizDiv = li.closest('.RenderAsWizard');
wizDiv.smartWizard('setState', [li.index()], 'disable');
var visTabs = li.siblings(':visible');
if (visTabs.length === 0)
  li.closest('.RenderAsWizard').hide();
else if (li.find('a').hasClass('active'))
  $(visTabs[0]).find('a').click();
this.isInVisibleInUI = true;"; }
        }

        [JsonIgnore]
        public override string ShowJSfn
        {
            get { return @"
debugger;
var li = $('li[ebsid=' + this.EbSid_CtxId+']'); 
li.show(); 
var wizDiv = li.closest('.RenderAsWizard');
wizDiv.smartWizard('unsetState', [li.index()], 'disable');
var visTabs = li.siblings(':visible');
if (visTabs.length === 0) {
  li.closest('.RenderAsWizard').show();
  li.find('a').click();
}
this.isInVisibleInUI = false;"; }
        }
    }
}

