using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExpressBase.Objects
{
    [ProtoBuf.ProtoContract]
    public class EbRadioGroup : EbControl
    {
        public EbRadioGroup() { }

        [ProtoBuf.ProtoMember(1)]
        [System.ComponentModel.Category("Behavior")]
        public int NumberOfOptions { get; set; }

        [ProtoBuf.ProtoMember(2)]
        [System.ComponentModel.Category("Behavior")]
        public string OnValue { get; set; }

        [ProtoBuf.ProtoMember(3)]
        [System.ComponentModel.Category("Behavior")]
        public string OffValue { get; set; }

        [ProtoBuf.ProtoMember(4)]
        [System.ComponentModel.Category("Behavior")]
        public string[] Options = { "radio", "TV", "fridge" };

        private string RadioCode
        {
            get
            {
                string rs = "<div id='@namecontainer' style='position:absolute; left:300px; top:300px;'>";

                if (this.NumberOfOptions <= 2)
                {
                    rs += @"
                        <label > Option two </label >
                        <input type = 'checkbox' data-toggle = 'toggle' data-on = '@OnValue' data-off = '@OffValue'>"
                    .Replace("@OnValue", this.OnValue).Replace("@OffValue", this.OffValue);
                }
                else
                {
                    rs += "<div class='btn-group' data-toggle='buttons'>";
                    for (int i = 1; i <= this.NumberOfOptions; i++)
                    {
                        rs += @"
                            <label class='btn btn-primary '>
                                <input type ='radio' name='options' autocomplete='off'>
                                @option
                            </label>".Replace("@option", this.Options[i]);
                    }
                    rs += "</div>";
                }
                return rs + "</div>".Replace("$tooltipText", this.ToolTipText).Replace("@name", this.Name);
            }
        }

        public override string GetHead()
        {
            return this.RequiredString + @"
$('#@idcontainer [type=checkbox]').bootstrapToggle();
$('#@idcontainer [type=radio]').on('click', function () {
    $(this).button('toggle')
  })
".Replace("@id", this.Name );
        }

        public override string GetHtml()
        {
            return @"
            @RadioCode
"
.Replace("@name", this.Name)
.Replace("@RadioCode", this.RadioCode);
        }
    }
}
