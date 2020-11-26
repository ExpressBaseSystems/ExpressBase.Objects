using ExpressBase.Common.Constants;
using ExpressBase.Common.Extensions;
using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using ExpressBase.Common.Structures;

namespace ExpressBase.Objects
{
    [EnableInBuilder(BuilderType.MobilePage)]
    public class EbMobileNumericBox : EbMobileControl
    {
        [EnableInBuilder(BuilderType.MobilePage)]
        [HideInPropertyGrid]
        public override EbDbTypes EbDbType { get { return EbDbTypes.Decimal; } set { } }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyGroup(PGConstants.EXTENDED)]
        [DefaultPropValue("2")]
        [HelpText("Number of decimal places")]
        public int DecimalPlaces { get; set; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyGroup(PGConstants.VALIDATIONS)]
        [Alias("Maximum")]
        [HelpText("Maximum value allowed")]
        public int MaxLimit { get; set; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyGroup(PGConstants.VALIDATIONS)]
        [Alias("Minimum")]
        [HelpText("Minimum value allowed")]
        public int MinLimit { get; set; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyGroup("Behavior")]
        public NumericBoxTypes RenderType { get; set; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [HideInPropertyGrid]
        public override string Icon { get { return "fa-i-cursor"; } }

        public override string GetDesignHtml()
        {
            return @"<div class='eb_stacklayout mob_control dropped' id='@id' eb-type='EbMobileNumericBox' tabindex='1' onclick='$(this).focus()'>
                            <label class='ctrl_label'> @Label </label>
                            <div class='eb_ctrlhtml'>
                                <input type='number' class='eb_mob_numericbox' />
                                <div class='eb_mob_numericbox-btntype'>
                                    <div class='wraper'>
                                        <button class='numeric-btn'><i class='fa fa-minus'></i></button>
                                        <div class='nemric-text'>
                                            <input type='text'/>
                                        </div>
                                        <button class='numeric-btn'><i class='fa fa-plus'></i></button>
                                    </div>
                                </div>
                            </div>
                        </div>".RemoveCR().DoubleQuoted();
        }

        public override string EbControlType => nameof(EbNumeric);

        public override EbControl GetWebFormControl(int counter)
        {
            return new EbNumeric
            {
                EbSid = this.EbControlType + counter,
                Name = this.Name,
                DecimalPlaces = this.DecimalPlaces,
                MaxLimit = this.MaxLimit,
                MinLimit = this.MinLimit,
                Margin = new UISides { Top = 0, Bottom = 0, Left = 0, Right = 0 },
                Label = this.Label
            };
        }

        public override void UpdateWebFormControl(EbControl control)
        {
            if (control == null || !(control is EbNumeric numeric))
                return;

            base.UpdateWebFormControl(control);

            numeric.MaxLimit = this.MaxLimit;
            numeric.MinLimit = this.MinLimit;
        }
    }
}
