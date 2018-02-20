using ExpressBase.Common.Extensions;
using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace ExpressBase.Objects
{
    [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm)]
    public class EbCheckBoxGroup:EbControl
    {
        public EbCheckBoxGroup()
        {
            this.CheckBoxes = new List<EbCheckBox>();
        }

        [OnDeserialized]
        public void OnDeserializedMethod(StreamingContext context)
        {
            this.BareControlHtml = this.GetBareHtml();
            this.ObjType = this.GetType().Name.Substring(2, this.GetType().Name.Length - 2);
        }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm)]
        public decimal Value { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm)]
        [PropertyEditor(PropertyEditorType.Collection)]
        [Alias("CheckBoxes")]
        public List<EbCheckBox> CheckBoxes { get; set; }

        public override string GetBareHtml()
        {
            string html = "<div id='@name@' name='@name@'>";
            foreach (EbCheckBox ec in this.CheckBoxes)
            {
                ec.GName = this.Name;
                html += ec.GetHtml();
            }
            html += "</div>";
            return html.Replace("@name@", (this.Name != null) ? this.Name : "@name@");
        }

        public override string GetDesignHtml()
        {
            return GetHtml().RemoveCR().DoubleQuoted();
        }

        public override string GetHtml()
        {

            string html = @"
            <div id='cont_@name@' class='Eb-ctrlContainer' Ctype='CheckBoxGroup'>
                <div class='radiog-cont'  style='@BackColor '>
                 <span id='@name@Lbl' style='@LabelBackColor @LabelForeColor '> @Label@  </span>
                        @barehtml@
                <span class='helpText'> @HelpText </span></div>
            </div>"
.Replace("@barehtml@", this.GetBareHtml())
.Replace("@name@", (this.Name != null) ? this.Name : "@name@")
.Replace("@label@", this.Label)
.Replace("@LabelForeColor ", "color:" + ((this.LabelForeColor != null) ? this.LabelForeColor : "@LabelForeColor ") + ";")
.Replace("@LabelBackColor ", "background-color:" + ((this.LabelBackColor != null) ? this.LabelBackColor : "@LabelBackColor ") + ";")
.Replace("@BackColor ", ("background-color:" + ((this.BackColor != null) ? this.BackColor : "@BackColor ") + ";"));
            return html;
        }

        public override string GetJsInitFunc()
        {
            return @"
this.Init = function(id)
{
    this.CheckBoxes.push(new EbObjects.EbCheckBox(id + '_Rd0'));
    this.CheckBoxes.push(new EbObjects.EbCheckBox(id + '_Rd1'));
};";
        }
    }

    public class EbCheckBoxAbstract : EbControl
    {

    }

    [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm)]
    [HideInToolBox]
    public class EbCheckBox : EbCheckBoxAbstract
    {
        public EbCheckBox() { }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm)]
        public override string Label { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm)]
        public string Value { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm)]
        public string GName { get; set; }

        public override string GetBareHtml()
        {
            return @"<div ctype='CheckBoxGroup'><input type ='checkbox' value='@value@' id='@name@' name='@gname@'> <span id='@name@Lbl' style='@LabelBackColor @LabelForeColor '> @label@  </span><br></div>"
.Replace("@name@", this.Name)
.Replace("@gname@", this.GName)
.Replace("@label@", this.Label)
.Replace("@label@", this.Value);
        }

        public override string GetHtml()
        {
            return this.GetBareHtml(); ;
        }
    }
}
