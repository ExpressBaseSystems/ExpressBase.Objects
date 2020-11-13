using ExpressBase.Common.Constants;
using ExpressBase.Common.Extensions;
using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using ExpressBase.Common.Structures;

namespace ExpressBase.Objects
{
    [EnableInBuilder(BuilderType.MobilePage)]
    public class EbMobileTextBox : EbMobileControl
    {
        [EnableInBuilder(BuilderType.MobilePage)]
        [HideInPropertyGrid]
        public override EbDbTypes EbDbType { get { return EbDbTypes.String; } set { } }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyGroup(PGConstants.EXTENDED)]
        public int MaxLength { get; set; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyGroup(PGConstants.EXTENDED)]
        public bool AutoSuggestion { get; set; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyGroup(PGConstants.CORE)]
        public TextTransform TextTransform { get; set; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyGroup(PGConstants.CORE)]
        public TextMode TextMode { get; set; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyGroup(PGConstants.BEHAVIOR)]
        public bool AutoCompleteOff { get; set; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [HideInPropertyGrid]
        public override string Icon { get { return "fa-i-cursor"; } }

        public override string GetDesignHtml()
        {
            return @"<div class='eb_stacklayout mob_control dropped' id='@id' eb-type='EbMobileTextBox' tabindex='1' onclick='$(this).focus()'>
                            <label class='ctrl_label'> @Label </label>
                            <div class='eb_ctrlhtml'>
                               <input type='text' class='eb_mob_textbox' />
                            </div>
                        </div>".RemoveCR().DoubleQuoted();
        }

        public override EbControl GetWebFormControl(int counter)
        {
            return new EbTextBox
            {
                EbSid = this.EbControlType + counter,
                Name = this.Name,
                MaxLength = this.MaxLength,
                TextTransform = this.TextTransform,
                TextMode = this.TextMode,
                AutoCompleteOff = this.AutoCompleteOff,
                AutoSuggestion = this.AutoSuggestion,
                Margin = new UISides { Top = 0, Bottom = 0, Left = 0, Right = 0 },
                Label = this.Label
            };
        }

        public override void UpdateWebFormControl(EbControl control)
        {
            if (control == null || !(control is EbTextBox textBox))
                return;

            base.UpdateWebFormControl(control);

            textBox.MaxLength = this.MaxLength;
            textBox.TextTransform = this.TextTransform;
            textBox.TextMode = this.TextMode;
            textBox.AutoCompleteOff = this.AutoCompleteOff;
            textBox.AutoSuggestion = this.AutoSuggestion;
        }
    }
}
