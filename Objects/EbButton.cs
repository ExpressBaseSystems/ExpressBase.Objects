using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExpressBase.Objects
{
    [ProtoBuf.ProtoContract]
    public class EbButton : EbControl
    {
        public EbButton() { }

        [ProtoBuf.ProtoMember(1)]
        [System.ComponentModel.Category("Appearance")]
        public string Text { get; set; }

        public override string GetHtml()
        {
            return string.Format(@"<button id={}>{2}</button>",this.Name, this.Text);
        }
    }
}
