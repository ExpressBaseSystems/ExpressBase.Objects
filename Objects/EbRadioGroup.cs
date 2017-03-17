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

        public override string GetHead()
        {
            return this.RequiredString + @"
$('#{0}container [type=radio]').on('click', function () {
    $(this).button('toggle')
  })
".Replace("{0}", this.Name );
        }

        public override string GetHtml()
        {
            return string.Format(@"
<div id='{0}container' style='position:absolute; left:300px; top:300px; '>



        <div class='btn-group' data-toggle='buttons'>
          <label class='btn btn-primary '>
            <input type ='radio' name='options' id='option1' autocomplete='off'> Radio 1 (preselected)
          </label>
          <label class='btn btn-primary'>
            <input type = 'radio' name='options' id='option2' autocomplete='off'> Radio 2
          </label>
          <label class='btn btn-primary'>
            <input type = 'radio' name='options' id='option3' autocomplete='off' checked> TV 3
          </label>
        </div>

            <input type = 'checkbox' id='{0}' data-toggle='toggle'data-on='Enabled' data-off='Disabled' >
            <label> Option two </label>
            <input type = 'checkbox' id='{0}' data-toggle='toggle'data-on='Enabled' data-off='Disabled' >

</div
", this.Name);
        }
    }
}
