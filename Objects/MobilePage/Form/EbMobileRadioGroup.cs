using ExpressBase.Common.Extensions;
using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using ExpressBase.Common.Structures;
using System.Collections.Generic;

namespace ExpressBase.Objects
{
    public enum EbMobileRGValueType
    {
        Integer = 11,
        String = 16
    }

    [EnableInBuilder(BuilderType.MobilePage)]
    public class EbMobileRadioGroup : EbMobileControl
    {
        [EnableInBuilder(BuilderType.MobilePage)]
        [HideInPropertyGrid]
        public override EbDbTypes EbDbType { get { return (EbDbTypes)ValueType; } set { } }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyGroup("Behavior")]
        public EbMobileRGValueType ValueType { get; set; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyEditor(PropertyEditorType.Collection)]
        [PropertyGroup("Data")]
        public List<EbMobileRGOption> Options { get; set; }

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
            EbRadioGroup ebRadio = new EbRadioGroup
            {
                EbSid = this.EbControlType + counter,
                Name = this.Name,
                Margin = new UISides { Top = 0, Bottom = 0, Left = 0, Right = 0 },
                Label = this.Label,
                Options = new List<EbRadioOptionAbstract>()
            };

            foreach (EbMobileRGOption opt in this.Options)
            {
                ebRadio.Options.Add(new EbRadioOption()
                {
                    EbSid = opt.EbSid,
                    Name = opt.Name,
                    Label = opt.DisplayName,
                    Value = opt.Value
                });
            }
            return ebRadio;
        }

        public override void UpdateWebFormControl(EbControl control)
        {
            if (control == null || !(control is EbRadioGroup radioGroup))
                return;

            base.UpdateWebFormControl(control);
        }
    }


    [EnableInBuilder(BuilderType.MobilePage)]
    public class EbMobileRGOption : EbMobilePageBase
    {
        [HideInPropertyGrid]
        [EnableInBuilder(BuilderType.MobilePage)]
        public string EbSid { get; set; }

        [EnableInBuilder(BuilderType.MobilePage)]
        public override string DisplayName { set; get; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [Alias("Data")]
        public string Value { set; get; }
    }
}
