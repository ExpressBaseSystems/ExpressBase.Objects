using ExpressBase.Common.Extensions;
using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace ExpressBase.Objects
{
    [EnableInBuilder(BuilderType.BotForm)]
    class EbLabel : EbControl
    {

        public EbLabel() { }

        [OnDeserialized]
        public void OnDeserializedMethod(StreamingContext context)
        {
            this.BareControlHtml = this.GetBareHtml();
            this.ObjType = this.GetType().Name.Substring(2, this.GetType().Name.Length - 2);
        }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm)]
        public string ImageID { get; set; }

        [EnableInBuilder(BuilderType.BotForm)]
        [PropertyEditor(PropertyEditorType.ObjectSelector)]
        public string DataSourceId { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm)]
        public string Alt { get; set; }
        public override string GetHead()
        {
            return string.Empty;
        }
        public override string GetDesignHtml()
        {
            return this.GetHtml().RemoveCR().GraveAccentQuoted();
        }

        public override string GetBareHtml()
        {
            return @" 
        <div class='ebimg-cont' style='width:100%;'>
            <span id='@name@'  class='eblabel-cont'>@Label@</span>
        </div>"
.Replace("@name@", this.Name)
    .Replace("@Label@ ", this.Label ?? "@Label@ ");
        }

        public override string GetHtml()
        {
            string EbCtrlHTML = @"
    <div id='cont_@name@' Ctype='Image' class='Eb-ctrlContainer' style='@hiddenString'>
       @barehtml@
    </div>
"
.Replace("@barehtml@", this.GetBareHtml())
.Replace("@name@", this.Name);
            return EbCtrlHTML;
        }
    }
}
