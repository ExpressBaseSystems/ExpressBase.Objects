﻿using ExpressBase.Common.Extensions;
using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using ExpressBase.Common.Structures;
using System.Collections.Generic;

namespace ExpressBase.Objects
{
    [EnableInBuilder(BuilderType.MobilePage)]
    public class EbMobileDisplayPicture : EbMobileControl
    {
        public override bool Unique { get; set; }
        public override EbScript ValueExpr { get; set; }
        public override EbScript DefaultValueExpression { get; set; }
        public override List<EbMobileValidator> Validators { set; get; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyGroup("General")]
        [DefaultPropValue("true")]
        public bool EnableCameraSelect { set; get; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyGroup("General")]
        [DefaultPropValue("true")]
        public bool EnableFileSelect { set; get; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [HideInPropertyGrid]
        public override EbDbTypes EbDbType { get { return EbDbTypes.String; } set { } }

        public override string GetDesignHtml()
        {
            return @"<div class='eb_stacklayout mob_control dropped' id='@id' eb-type='EbMobileDisplayPicture' tabindex='1' onclick='$(this).focus()'>
                            <label class='ctrl_label'> @Label </label>
                            <div class='eb_ctrlhtml'>
                                <div class='dp-avatar'>
                                    <img src='/images/image.png'/>
                                </div>
                                <div class='dp-btn-container'>
                                    <button class='eb_mob_fupbtn filesbtn'><i class='fa fa-folder-open-o'></i></button>
                                    <button class='eb_mob_fupbtn camerabtn'><i class='fa fa-camera'></i></button>
                                </div>
                            </div>
                        </div>".RemoveCR().DoubleQuoted();
        }

        public override EbControl GetWebFormControl(int counter)
        {
            return new EbDisplayPicture
            {
                EbSid = this.EbControlType + counter,
                Name = this.Name,
                Margin = new UISides { Top = 0, Bottom = 0, Left = 0, Right = 0 },
                Label = this.Label
            };
        }

        public override void UpdateWebFormControl(EbControl control)
        {
            if (control == null || !(control is EbDisplayPicture dp))
                return;

            base.UpdateWebFormControl(control);
        }
    }
}
