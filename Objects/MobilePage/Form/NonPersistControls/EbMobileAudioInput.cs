using ExpressBase.Common.Extensions;
using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
namespace ExpressBase.Objects
{
    [EnableInBuilder(BuilderType.MobilePage)]
    class EbMobileAudioInput : EbMobileFileUpload
    {
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

        public override string GetJsInitFunc()
        {
            return @"
                this.Init = function(id)
                {
                    this.MultiSelect= false;
                    this.EnableEdit= false;
                };";
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
