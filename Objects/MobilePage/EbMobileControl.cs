using ExpressBase.Common.Constants;
using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using ExpressBase.Common.Structures;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ExpressBase.Objects
{
    [EnableInBuilder(BuilderType.MobilePage)]
    public abstract class EbMobileControl : EbMobilePageBase
    {
        public virtual string EbSid { set; get; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyEditor(PropertyEditorType.MultiLanguageKeySelector)]
        [UIproperty]
        [PropertyGroup(PGConstants.CORE)]
        public virtual string Label { set; get; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [HideInPropertyGrid]
        public virtual EbDbTypes EbDbType { get { return EbDbTypes.String; } set { } }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyGroup(PGConstants.BEHAVIOR)]
        public virtual bool Hidden { set; get; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyGroup(PGConstants.BEHAVIOR)]
        [PropertyEditor(PropertyEditorType.ScriptEditorCS)]
        [Alias("Hide Expression")]
        [HelpText("Define conditions to decide visibility of the control.")]
        public virtual EbScript HiddenExpr { get; set; }

        [PropertyGroup(PGConstants.BEHAVIOR)]
        [EnableInBuilder(BuilderType.MobilePage)]
        [HelpText("Set true if you want to make this control read only.")]
        public virtual bool ReadOnly { get; set; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyEditor(PropertyEditorType.ScriptEditorCS)]
        [Alias("ReadOnly Expression")]
        [PropertyGroup(PGConstants.BEHAVIOR)]
        [HelpText("Define conditions to decide Disabled/Readonly property of the control.")]
        public virtual EbScript DisableExpr { get; set; }

        [PropertyGroup(PGConstants.VALIDATIONS)]
        [EnableInBuilder(BuilderType.MobilePage)]
        public virtual bool Required { get; set; }

        [PropertyGroup(PGConstants.VALIDATIONS)]
        [EnableInBuilder(BuilderType.MobilePage)]
        [HelpText("Set true if want unique value for this control on every form save.")]
        public virtual bool Unique { get; set; }

        [PropertyGroup(PGConstants.VALIDATIONS)]
        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyEditor(PropertyEditorType.Collection)]
        [HelpText("List of validators to consider before form save.")]
        public virtual List<EbMobileValidator> Validators { get; set; }

        [PropertyGroup(PGConstants.DATA)]
        [EnableInBuilder(BuilderType.MobilePage)]
        [HelpText("Set true if you dont want to save value from this field.")]
        public virtual bool DoNotPersist { get; set; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [HideInPropertyGrid]
        public virtual string Icon { get { return string.Empty; } }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyEditor(PropertyEditorType.ScriptEditorCS)]
        [Alias("Value Expression")]
        [PropertyGroup(PGConstants.VALUE)]
        public virtual EbScript ValueExpr { get; set; }

        [PropertyGroup(PGConstants.VALUE)]
        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyEditor(PropertyEditorType.ScriptEditorCS)]
        [HelpText("Define default value of the control.")]
        public virtual EbScript DefaultValueExpression { get; set; }

        public virtual string EbControlType => this.GetType().Name.Replace("Mobile", "");

        public virtual EbControl GetWebFormControl(int counter) { return null; }

        public virtual void UpdateWebFormControl(EbControl control)
        {
            control.Label = this.Label;
            control.Required = this.Required;
            control.DoNotPersist = this.DoNotPersist;
            control.IsReadOnly = this.ReadOnly;
            control.Hidden = this.Hidden;
            control.Unique = this.Unique;
        }

        [OnDeserialized]
        public void OnDeserialized(StreamingContext context)
        {
            if (this.ValueExpr == null) this.ValueExpr = new EbScript();
            if (this.HiddenExpr == null) this.HiddenExpr = new EbScript();
            if (this.DisableExpr == null) this.DisableExpr = new EbScript();
            if (this.DefaultValueExpression == null) this.DefaultValueExpression = new EbScript();
        }

        public override List<string> DiscoverRelatedRefids()
        {
            return base.DiscoverRelatedRefids();
        }
    }
}
