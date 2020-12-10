using ExpressBase.Common.Constants;
using ExpressBase.Common.Extensions;
using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using ExpressBase.Common.Structures;
using System.Collections.Generic;

namespace ExpressBase.Objects
{
    [EnableInBuilder(BuilderType.MobilePage)]
    public class EbMobileGeoLocation : EbMobileControl
    {
        [EnableInBuilder(BuilderType.MobilePage)]
        [HideInPropertyGrid]
        public override EbDbTypes EbDbType { get { return EbDbTypes.String; } set { } }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyGroup(PGConstants.CORE)]
        public bool HideSearchBox { set; get; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyGroup(PGConstants.CORE)]
        public int ZoomLevel { set; get; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [HideInPropertyGrid]
        public override string Icon { get { return "fa-map-marker"; } }

        public override EbScript ValueExpr { get; set; }
        public override EbScript DefaultValueExpression { get; set; }
        public override List<EbMobileValidator> Validators { set; get; }

        public override string GetDesignHtml()
        {
            return @"<div class='eb_stacklayout mob_control dropped' id='@id' eb-type='EbMobileGeoLocation' tabindex='1' onclick='$(this).focus()'>
                            <label class='ctrl_label'> @Label </label>
                            <div class='eb_ctrlhtml'>
                               <div class='geoloc-ctrlwrapr'>
                                    <input type='text' style='display: @display' placeholder='Search place' class='eb_mob_textbox' />
                                    <div class='map-container'>
                                        
                                    </div>
                               </div>
                            </div>
                        </div>".Replace("@display", (this.HideSearchBox) ? "none" : "block").RemoveCR().DoubleQuoted();
        }

        public override string EbControlType => nameof(EbInputGeoLocation);

        public override EbControl GetWebFormControl(int counter)
        {
            return new EbInputGeoLocation
            {
                EbSid = this.EbControlType + counter,
                Name = this.Name,
                Margin = new UISides { Top = 0, Bottom = 0, Left = 0, Right = 0 },
                Label = this.Label
            };
        }

        public override void UpdateWebFormControl(EbControl control)
        {
            if (control == null || !(control is EbInputGeoLocation geo))
                return;

            base.UpdateWebFormControl(control);
        }
    }
}
