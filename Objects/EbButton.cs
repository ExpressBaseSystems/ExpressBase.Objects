using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace ExpressBase.Objects
{
    [ProtoBuf.ProtoContract]
    [EnableInBuilder(BuilderType.FormBuilder, BuilderType.FilterDialogBuilder)]
    public class EbButton : EbControl
    {
        public EbButton() { }

        public EbButton(object parent)
        {
            this.Parent = parent;
        }

        [ProtoBuf.ProtoMember(1)]
        [System.ComponentModel.Category("Appearance")]
        public string Text { get; set; }

        public override string GetHead()
        {
            return this.RequiredString + @"
                    ".Replace("@name", this.Id.ToString());
        }

        public override string GetHtml()
        {
            return string.Format(@"
<div id='@namecontainer' class='Eb-ctrlContainer' style='position:absolute; left:@leftpx; top:@toppx; '>
    <button id='@name' class='btn btn-default'  data-toggle='tooltip' title='@toolTipText' 
        style='width:@widthpx; height:@heightpx; @backColor @foreColor display:inline-block; @fontStyle'>@text</button>
</div>
"
.Replace("@name", this.Name)
.Replace("@left", this.Left.ToString())
.Replace("@top", this.Top.ToString())
.Replace("@width", this.Width.ToString())
.Replace("@height", this.Height.ToString())
.Replace("@hiddenString", this.HiddenString)
.Replace("@toolTipText", this.ToolTipText)
.Replace("@text", this.Text )
.Replace("@tabIndex", "tabindex='" + this.TabIndex + "'")
//.Replace("@backColor", "background-color:" + this.BackColorSerialized + ";")
//.Replace("@foreColor", "color:" + this.ForeColorSerialized + ";")
//.Replace("@fontStyle", (this.FontSerialized != null) ?
//                            (" font-family:" + this.FontSerialized.FontFamily + ";" + "font-style:" + this.FontSerialized.Style
//                            + ";" + "font-size:" + this.FontSerialized.SizeInPoints + "px;")
//                        : string.Empty)
);
        }
    }
}
