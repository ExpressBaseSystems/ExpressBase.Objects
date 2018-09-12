using ExpressBase.Common.Extensions;
using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ExpressBase.Objects
{
    [EnableInBuilder(BuilderType.WebForm)]
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
                    $(`#${ elementId}.Eb-ctrlContainer > .tab-content >.tab-pane`).css('padding', props.Padding + 'px');
                },
                adjustPanesHeightToHighest : function(elementId, props) {
                    var maxH = 0;
                    let $panes = $(`#${ elementId}.Eb-ctrlContainer > .tab-content >.tab-pane`);
                    $panes.css('min-height', 'inherit');
                    $panes.each(function () {
                        $this = $(this);
                        if ($this.outerHeight() > maxH) {
                            maxH = $this.outerHeight();
                        }
                    });
                    if($('form[eb-form=true]').attr('IsRenderMode') === 'True')
                        $panes.outerHeight(maxH);      
                    else
                        $panes.css('min-height', maxH +'px');                      
                    }
                }";
            }
        }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog)]
        [PropertyGroup("Test")]
        [OnChangeUIFunction("EbTabControl.padding")]
        [UIproperty]
        [DefaultPropValue("3")]
        public int Padding { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog)]
        [PropertyEditor(PropertyEditorType.Collection)]
        [Alias("TabCollection")]
        [PropertyGroup("test")]
        [UIproperty]
        [ListType(typeof(EbTabPane))]
        [OnChangeUIFunction("EbTabControl.adjustPanesHeightToHighest")]
        public override List<EbControl> Controls { get; set; }

        public override string GetDesignHtml()
        {
            this.Controls = new List<EbControl>();
            this.Controls.Add(new EbTabPane { Name = "EbTab0TabPane0" });
            return GetHtml().RemoveCR().DoubleQuoted(); ;
        }

        public override string GetJsInitFunc()
        {
            return @"
this.Init = function(id)
{
    this.Controls.$values.push(new EbObjects.EbTabPane(this.EbSid + 'TabPane0'));
};";
        }

        public override string GetHtml()
        {
            string TabBtnHtml = "<div id='cont_@ebsid@' ebsid='@ebsid@' class='Eb-ctrlContainer' Ctype='TabControl'><ul class='nav nav-tabs'>".Replace("@ebsid@", EbSid);
            string TabContentHtml = "<div class='tab-content'>";

            foreach (EbControl tab in Controls)
                TabBtnHtml += "<li li-of='@ebsid@' @active><a data-toggle='tab' href='#@ebsid@'>@name@</a></li>".Replace("@name@", tab.Name).Replace("@ebsid@", tab.EbSid);

            TabBtnHtml += "</ul>";

            Regex regex = new Regex(Regex.Escape("@active"));
            TabBtnHtml = regex.Replace(TabBtnHtml, "class='active'", 1).Replace("@active", "");


            foreach (EbControl tab in Controls)
                TabContentHtml += tab.GetHtml();

            TabContentHtml += "</div></div>";
            regex = new Regex(Regex.Escape("@inactive"));
            TabContentHtml = regex.Replace(TabContentHtml, "in active", 1).Replace("@inactive", "");

            return string.Concat(TabBtnHtml, TabContentHtml);
        }
    }

    [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog)]
    [HideInToolBox]
    public class EbTabPane : EbControlContainer
    {
        public EbTabPane()
        {
            this.Controls = new List<EbControl>();
            this.ObjType = this.GetType().Name.Substring(2, this.GetType().Name.Length - 2);
        }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog)]
        [HideInPropertyGrid]
        public override List<EbControl> Controls { get; set; }

        public override string GetHtml()
        {
            string html = "<div id='@ebsid@' ebsid='@ebsid@' class='tab-pane fade @inactive'>";

            foreach (EbControl ec in this.Controls)
                html += ec.GetHtml();

            return (html + "</div>").Replace("@name@", this.Name).Replace("@ebsid@", this.EbSid);
        }
    }
}
