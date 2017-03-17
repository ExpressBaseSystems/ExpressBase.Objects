using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace ExpressBase.Objects
{
    [ProtoBuf.ProtoContract]
    public class EbButton : EbControl
    {
        [Browsable(false)]
        public object Parent { get; set; }

        public EbButton() { }

        public EbButton(object parent)
        {
            this.Parent = parent;
        }

        [ProtoBuf.ProtoMember(1)]
        [System.ComponentModel.Category("Appearance")]
        public string Text { get; set; }

        public override string GetHtml()
        {
            return string.Format(@"<button id={0}>{1}</button>",this.Name, this.Text);
        }
    }
}
