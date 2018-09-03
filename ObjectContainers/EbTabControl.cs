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
        public EbTabControl() {

            this.Controls = new List<EbControl>();
            this.ControlsT = new List<EbControl>();            
            this.ObjType = this.GetType().Name.Substring(2, this.GetType().Name.Length - 2);
        }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog)]
        [PropertyEditor(PropertyEditorType.Collection)]
        [Alias("TabCollection")]
        [PropertyGroup("test")]
        public override List<EbControl> Controls { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog)]
        [PropertyEditor(PropertyEditorType.Collection)]
        [PropertyGroup("test")]
        public List<EbControl> ControlsT { get; set; }

        public override string GetDesignHtml()
        {
            string Html = "<div class='Eb-ctrlContainer' Ctype='TabControl'>";
            this.Controls = new List<EbControl>();
            this.Controls.Add(new EbTabPane { Name = "EbTab0TabPane0" });
            Html += GetHtml() + "</div>";
            return Html.RemoveCR().DoubleQuoted(); ;
        }

        public override string GetJsInitFunc()
        {
            return @"
this.Init = function(id)
{
    this.Controls.$values.push(new EbObjects.EbTabPane('EbTab0TabPane0'));
    this.ControlsT = this.Controls;
};";
        }

        public override string GetHtml()
        {
            string TabBtnHtml = "<div class='Eb-ctrlContainer' Ctype='TabControl'><ul class='nav nav-tabs'>";
            string TabContentHtml = "<div class='tab-content'>";

            foreach (EbControl tab in Controls)
                TabBtnHtml += "<li><a data-toggle='tab' href='#@name@'>@name@</a></li>".Replace("@name@", tab.Name);

            TabBtnHtml += "</ul>";


            foreach (EbControl tab in Controls)
                TabContentHtml += tab.GetHtml();

            TabContentHtml += "</div>";
            Regex regex = new Regex(Regex.Escape("@inactive"));
            TabContentHtml = regex.Replace(TabContentHtml, "in active", 1).Replace("@inactive","");

            return string.Concat(TabBtnHtml, TabContentHtml) ;
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
            string html = "<div id='@name@' class='tab-pane fade @inactive'>";

            foreach (EbControl ec in this.Controls)
                html += ec.GetHtml();

            return (html + "</div>").Replace("@name@", this.Name);
        }
    }
}
