using ExpressBase.Common.Extensions;
using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using ExpressBase.Common.Structures;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace ExpressBase.Objects
{
    [HideInToolBox]
    [EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
    public class EbButton : EbControlUI
    {
        public EbButton() { }

        [OnDeserialized]
        public void OnDeserializedMethod(StreamingContext context)
        {
            this.BareControlHtml = this.GetBareHtml();
            this.ObjType = this.GetType().Name.Substring(2, this.GetType().Name.Length - 2);
        }
        
        [EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
        [Alias("Text")]
        public override string Label { get; set; }
        [HideInPropertyGrid]
        [EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
        public override bool DoNotPersist { get { return true; } }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
        [PropertyEditor(PropertyEditorType.ObjectSelector)]
        [OSE_ObjectTypes(EbObjectTypes.iWebForm)]
        public string FormRefId { get; set; }

        public override string GetDesignHtml()
        {
            return GetHtmlHelper(RenderMode.Developer).RemoveCR().DoubleQuoted();
        }

        public override string GetHtml()
        {
            return GetHtmlHelper(RenderMode.User);
        }

        public override string GetBareHtml()
        {
            return @"<button id='@ebsid@' class='btn btn-default' style='width:100%; @backColor @foreColor @fontStyle' disabled >@Label@</button>"
                .Replace("@ebsid@", this.EbSid_CtxId)
                .Replace("@Label@", this.Label ??"@Text@")
.Replace("@tabIndex", "tabindex='" + this.TabIndex + "'")
.Replace("@backColor", "background-color:" + this.BackColor + ";")
.Replace("@foreColor", "color:" + this.ForeColor + ";")
//.Replace("@fontStyle", (this.FontSerialized != null) ?
//                            (" font-family:" + this.FontSerialized.FontFamily + ";" + "font-style:" + this.FontSerialized.Style
//                            + ";" + "font-size:" + this.FontSerialized.SizeInPoints + "px;")
//                        : string.Empty)
;
        }

        private string GetHtmlHelper(RenderMode mode)
        {
            string EbCtrlHTML = @"
            <div id='cont_@ebsid@' ebsid='@ebsid@' name='@name@' class='Eb-ctrlContainer' @childOf@ ctype='@type@' eb-hidden='@isHidden@'>
                <div  id='@ebsid@Wraper' class='ctrl-cover'>
                    @barehtml@
                </div>
                <span class='helpText' ui-helptxt >@helpText@ </span>
            </div>";
            return ReplacePropsInHTML(EbCtrlHTML);
        }
       
    }
}
