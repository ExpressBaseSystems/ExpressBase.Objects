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
    public class EbTabControl : EbControlContainer
    {
        public EbTabControl()
        {

            this.Controls = new List<EbControl>();
            this.ObjType = this.GetType().Name.Substring(2, this.GetType().Name.Length - 2);
        }
        public override string UIchangeFns
        {
            get
            {
                return @"EbTabControl = {
                padding : function(elementId, props) {
                    $(`#cont_${ elementId}.Eb-ctrlContainer > .tab-content >.tab-pane`).css('padding', `${props.Padding.Top}px ${props.Padding.Right}px ${props.Padding.Bottom}px ${props.Padding.Left}px`);
                },
                label : function(elementId, props) {
                    $(`li[ebsid='${elementId}'] .eb-label-editable`).text(props.Title);
                },
                adjustPanesHeightToHighest : function(elementId, props) {
                    var maxH = 0;
                    let $panes = $(`#cont_${ elementId}.Eb-ctrlContainer > .tab-content >.tab-pane`);
                    $panes.css('min-height', 'inherit');
                    $panes.each(function () {
                        $this = $(this);
                        if ($this.outerHeight() > maxH) {
                            maxH = $this.outerHeight();
                        }
                    });
                    if (props.MinHeight > maxH)
                        maxH = props.MinHeight;
                    if($('form[eb-form=true]').attr('IsRenderMode') === 'true')
                        $panes.outerHeight(maxH);      
                    else
                        $panes.css('min-height', maxH +'px');                      
                    }
                }";
            }
        }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.UserControl)]
        [OnChangeUIFunction("EbTabControl.padding")]
        [DefaultPropValue(8, 8, 8, 8)]
        [PropertyGroup("Appearance")]
        [PropertyEditor(PropertyEditorType.Expandable)]
        public new UISides Padding { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.UserControl)]
        [OnChangeUIFunction("EbTabControl.adjustPanesHeightToHighest")]
        [DefaultPropValue("150")]
        [PropertyGroup("Appearance")]
        [Alias("Minimum height")]
        public int MinHeight { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.UserControl)]
        [PropertyEditor(PropertyEditorType.Collection)]
        [Alias("TabPanes")]
        [PropertyGroup("test")]
        [UIproperty]
        [PropertyPriority(95)]
        [ListType(typeof(EbTabPane))]
        [OnChangeUIFunction("EbTabControl.adjustPanesHeightToHighest")]
        public override List<EbControl> Controls { get; set; }

        [JsonIgnore]
        public override string Label { get; set; }

        [JsonIgnore]
        public override string BackColor { get; set; }

        [JsonIgnore]
        public override string ForeColor { get; set; }

        [JsonIgnore]
        public override string LabelBackColor { get; set; }

        [JsonIgnore]
        public override string LabelForeColor { get; set; }

        [HideInPropertyGrid]
        [JsonIgnore]
        public override string ToolIconHtml { get { return "<i class='fa fa-clone'></i>"; } set { } }
        //public override string GetToolHtml()
        //{
        //    return @"<div eb-type='@toolName' class='tool'><i class='fa fa-clone'></i>  @toolName</div>".Replace("@toolName", this.GetType().Name.Substring(2));
        //}

        public override string GetDesignHtml()
        {
            this.Controls = new List<EbControl>();
            this.Controls.Add(new EbTabPane { Name = "EbTab0TabPane0", Title = "pane1" });
            return GetHtml().RemoveCR().DoubleQuoted(); ;
        }

        public override string GetJsInitFunc()
        {
            return @"
this.Init = function(id)
{
    let pane = new EbObjects.EbTabPane(this.EbSid + 'TabPane0');
    pane.Name = 'pane1';
    pane.Title = 'pane1';
    this.Controls.$values.push(pane);
};";
        }

        public override string GetHtml()
        {
            string TabBtnHtml = @"
<div id='cont_@ebsid@' ebsid='@ebsid@' class='Eb-ctrlContainer' Ctype='TabControl'>
    <div class='tab-btn-cont'>
        <ul class='nav nav-tabs'>".Replace("@ebsid@", EbSid);
            string TabContentHtml = @"
            <div class='tab-content'>";

            foreach (EbTabPane tab in Controls)
                TabBtnHtml += @"
            <li li-of='@ebsid@' ebsid='@ebsid@' @active>
                <a data-toggle='tab' class='ppbtn-cont' href='#@ebsid@'>
                    <span class='eb-label-editable'>@title@</span>
                    <input id='@ebsid@lbltxtb' class='eb-lbltxtb' type='text'/>@ppbtn@
                    <div class='ebtab-close-btn eb-fb-icon'><i class='fa fa-times' aria-hidden='true'></i></div>
                </a>
                <div class='ebtab-add-btn eb-fb-icon'><i class='fa fa-plus' aria-hidden='true'></i></div>                
            </li>".Replace("@title@", tab.Title).Replace("@ppbtn@", Common.HtmlConstants.CONT_PROP_BTN).Replace("@ebsid@", tab.EbSid);

            TabBtnHtml += @"
        </ul>
    </div>";

            Regex regex = new Regex(Regex.Escape("class='active'"));
            TabBtnHtml = regex.Replace(TabBtnHtml, "active", 1).Replace("@active", "");


            foreach (EbControl tab in Controls)
                TabContentHtml += tab.GetHtml();

            TabContentHtml += "</div></div>";
            regex = new Regex(Regex.Escape("@inactive"));
            TabContentHtml = regex.Replace(TabContentHtml, "in active", 1).Replace("@inactive", "");

            return string.Concat(TabBtnHtml, TabContentHtml);
        }
    }

    [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.UserControl)]
    [HideInToolBox]
    public class EbTabPane : EbControlContainer
    {
        public EbTabPane()
        {
            this.Controls = new List<EbControl>();
            this.ObjType = this.GetType().Name.Substring(2, this.GetType().Name.Length - 2);
        }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.UserControl)]
        [HideInPropertyGrid]
        public override List<EbControl> Controls { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.UserControl)]
        [PropertyGroup("Identity")]
        [OnChangeUIFunction("EbTabControl.label")]
        [PropertyPriority(70)]
        public string Title { get; set; }

        [JsonIgnore]
        public override string Label { get; set; }

        public override string GetHtml()
        {
            string html = "<div id='@ebsid@' ebsid='@ebsid@' ctype='@objtype@' class='tab-pane fade @inactive ebcont-ctrl'>";

            foreach (EbControl ec in this.Controls)
                html += ec.GetHtml();

            return (html + "</div>")
                .Replace("@name@", this.Name)
                .Replace("@ebsid@", this.EbSid)
                .Replace("@objtype@", this.ObjType);
        }
    }
}
