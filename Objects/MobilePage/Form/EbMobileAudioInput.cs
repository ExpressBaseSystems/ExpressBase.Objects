using ExpressBase.Common.Extensions;
using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using ExpressBase.Common.Structures;
using System.Collections.Generic;

namespace ExpressBase.Objects
{
    [EnableInBuilder(BuilderType.MobilePage)]
    public class EbMobileAudioInput : EbMobileControl
    {
        public override bool Unique { get; set; }
        public override EbScript ValueExpr { get; set; }
        public override EbScript DefaultValueExpression { get; set; }
        public override List<EbMobileValidator> Validators { set; get; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [HideInPropertyGrid]
        public override EbDbTypes EbDbType { get { return EbDbTypes.String; } set { } }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyGroup("General")]
        [DefaultPropValue("false")]
        public bool MultiSelect { set; get; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyGroup("General")]
        [DefaultPropValue("2")]
        public int MaximumDuration { set; get; }

        public override string GetDesignHtml()
        {
            return @"<div class='eb_stacklayout mob_control dropped' id='@id' eb-type='EbMobileAudioInput' tabindex='1' onclick='$(this).focus()'>
                            <label class='ctrl_label'> @Label </label>
                            <div class='eb_ctrlhtml eb_mob_audinput_container'>
                                <button class='eb_mob_audinputbtn'><i class='fa fa-microphone'></i></button>
                                <Label>0:00</button>
                            </div>
                        </div>".RemoveCR().DoubleQuoted();
        }

        public override EbControl GetWebFormControl(int counter)
        {
            return new EbAudioInput
            {
                EbSid = this.EbControlType + counter,
                Name = this.Name,
                IsMultipleUpload = this.MultiSelect,
                MaximumDUration = this.MaximumDuration,
                Margin = new UISides { Top = 0, Bottom = 0, Left = 0, Right = 0 },
                Label = this.Label
            };
        }

        public override void UpdateWebFormControl(EbControl control)
        {
            if (control == null || !(control is EbAudioInput fup))
                return;

            base.UpdateWebFormControl(control);

            fup.IsMultipleUpload = this.MultiSelect;
            fup.MaximumDUration = this.MaximumDuration;
        }
    }
}
