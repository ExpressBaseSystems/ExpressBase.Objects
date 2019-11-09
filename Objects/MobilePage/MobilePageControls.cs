using ExpressBase.Common.Extensions;
using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExpressBase.Objects
{
    [EnableInBuilder(BuilderType.MobilePage)]
    public class EbMobileControls : EbMobilePageBase
    {
        [EnableInBuilder(BuilderType.MobilePage)]
        [UIproperty]
        public string Label { set; get; }
    }

    [EnableInBuilder(BuilderType.MobilePage)]
    public class EbMobileTextBox : EbMobileControls
    {
        public override string GetDesignHtml()
        {
            return @"<div class='eb_stacklayout' id='@id' eb-type='MobileTextBox'>
                            <label class='ctrl_label'>@Label</label>
                            <div class='eb_ctrlhtml'>
                               <input type='text' class='eb_mob_textbox' />
                            </div>
                        </div>".RemoveCR().DoubleQuoted();
        }
    }
}
