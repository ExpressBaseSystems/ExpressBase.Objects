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

        public override string GetHead()
        {
            return this.RequiredString + @"
                    ".Replace("{0}", this.Id.ToString());
        }

        public override string GetHtml()
        {
            return string.Format(@"
<div id='{0}container' style='position:absolute; left:{2}px; top:{3}px; '>
    <button class='btn btn-default'  data-toggle='tooltip' title='{4}' id={0}>{1}</button>
</div>
", this.Name, this.Text, this.Left, this.Top, this.ToolTipText);
        }
    }
}
