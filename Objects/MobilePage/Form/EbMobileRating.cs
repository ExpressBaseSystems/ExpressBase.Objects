using ExpressBase.Common.Extensions;
using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using ExpressBase.Common.Structures;

namespace ExpressBase.Objects
{
    public class EbMobileRating : EbMobileControl
    {
        public override EbDbTypes EbDbType { get { return EbDbTypes.Decimal; } set { } }

        [EnableInBuilder(BuilderType.MobilePage)]
        [Alias("Maximum Star")]
        [DefaultPropValue("5")]
        public int MaxValue { get; set; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyEditor(PropertyEditorType.Color)]
        [Alias("Selection Color")]
        [DefaultPropValue("#F39C12")]
        public string SelectionColor { get; set; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [DefaultPropValue("8")]
        [Alias("Spacing")]
        public int Spacing { get; set; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [HideInPropertyGrid]
        public override string Icon { get { return "fa-i-cursor"; } }

        public override string GetDesignHtml()
        {
            return @"<div class='eb_stacklayout mob_control dropped' id='@id' eb-type='EbMobileRating' tabindex='1' onclick='$(this).focus()'>
                            <label class='ctrl_label'> @Label </label>
                            <div class='eb_ctrlhtml'>
                               <span class='fa fa-star-o wrd_spacing'></span>
	                            <span class='fa fa-star-o wrd_spacing'></span>
	                            <span class='fa fa-star-o wrd_spacing'></span>
	                            <span class='fa fa-star-o wrd_spacing'></span>
	                            <span class='fa fa-star-o wrd_spacing'></span>
                            </div>
                        </div>".RemoveCR().DoubleQuoted();
        }
        public override EbControl GetWebFormControl(int counter)
        {
            return new EbRating
            {
                EbSid = this.EbControlType + counter,
                Name = this.Name,
                MaxVal = this.MaxValue,
                RatingColor = this.SelectionColor,
                Spacing = this.Spacing,
                Margin = new UISides { Top = 0, Bottom = 0, Left = 0, Right = 0 },
                Label = this.Label
            };
        }

        public override void UpdateWebFormControl(EbControl control)
        {
            if (control == null || !(control is EbRating rating))
                return;

            base.UpdateWebFormControl(control);

            rating.RatingColor = this.SelectionColor;
            rating.Spacing = this.Spacing;
            rating.MaxVal = this.MaxValue;
        }
    }
}
