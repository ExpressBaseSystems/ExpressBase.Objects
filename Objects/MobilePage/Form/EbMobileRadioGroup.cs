using ExpressBase.Common.Extensions;
using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using ExpressBase.Common.Structures;

namespace ExpressBase.Objects
{
    [EnableInBuilder(BuilderType.MobilePage)]
    public class EbMobileRadioGroup : EbMobileControl
    {
        [EnableInBuilder(BuilderType.MobilePage)]
        [HideInPropertyGrid]
        public override EbDbTypes EbDbType { get { return (EbDbTypes)ValueType; } set { } }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyGroup("Behavior")]
        public EbValueType ValueType { get; set; }

        public override string GetDesignHtml()
        {
            return @"<div class='eb_stacklayout mob_control dropped' id='@id' eb-type='EbMobileRadioGroup' tabindex='1' onclick='$(this).focus()'>
                            <label class='ctrl_label'> @Label </label>
                            <div class='eb_ctrlhtml'>
                               <div class='eb_mob_radiogroup_container'></div>
                            </div>
                        </div>".RemoveCR().DoubleQuoted();
        }

        public override EbControl GetWebFormControl(int counter)
        {
            return new EbRadioGroup
            {
                EbSid = this.EbControlType + counter,
                Name = this.Name,
                Margin = new UISides { Top = 0, Bottom = 0, Left = 0, Right = 0 },
                Label = this.Label
            };
        }

        public override void UpdateWebFormControl(EbControl control)
        {
            if (control == null || !(control is EbRadioGroup radioGroup))
                return;

            base.UpdateWebFormControl(control);
        }
    }
}
