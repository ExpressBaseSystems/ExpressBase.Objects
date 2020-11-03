using ExpressBase.Common.Constants;
using ExpressBase.Common.Data;
using ExpressBase.Common.Extensions;
using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using ExpressBase.Common.Structures;
using ExpressBase.Objects.Objects.DVRelated;
using System;
using System.Collections.Generic;
namespace ExpressBase.Objects
{
    [EnableInBuilder(BuilderType.MobilePage)]
    class EbMobileAudioInput : EbMobileControl, INonPersistControl
    {
        public override bool DoNotPersist { get; set; }
        public override bool Unique { get; set; }
        public override EbScript ValueExpr { get; set; }
        public override EbScript DefaultValueExpression { get; set; }
        public override List<EbMobileValidator> Validators { set; get; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyGroup("General")]
        public int MaxDUration { set; get; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyGroup("General")]
        public virtual bool MultiSelect { set; get; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyGroup("General")]
        public virtual bool EnableEdit { set; get; }

        public override string GetDesignHtml()
        {
            return @"<div class='eb_stacklayout mob_control dropped' id='@id' eb-type='EbMobileAudioInput' tabindex='1' onclick='$(this).focus()'>
                            <label class='ctrl_label'> @Label </label>
                            <div class='eb_ctrlhtml' style='display:flex'>
                        <button>Start</button>
                        <button>Stop</button>
                        <button>play</button>
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
                MaximumDUration = this.MaxDUration,
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
            fup.MaximumDUration = this.MaxDUration;
        }
    }
}
