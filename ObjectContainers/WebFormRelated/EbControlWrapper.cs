using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;

namespace ExpressBase.Objects
{
    public class EbControlWrapper
    {
        public string TableName { get; set; }

        public string Path { get; set; }

        public string Root { get; set; }

        public EbControl Control { get; set; }
    }


    public class EbSQLValidator : EbValidator
    {
        public EbSQLValidator() { }

        [EnableInBuilder(BuilderType.WebForm)]
        [PropertyEditor(PropertyEditorType.ScriptEditorSQ)]
        public override EbScript Script { get; set; }
    }

    public class EbRoutines : EbValidator
    {
        public EbRoutines() { }

        [EnableInBuilder(BuilderType.WebForm)]
        [PropertyEditor(PropertyEditorType.ScriptEditorJS, PropertyEditorType.ScriptEditorSQ)]
        public override EbScript Script { get; set; }

        [EnableInBuilder(BuilderType.WebForm)]
        public bool IsDisabledOnNew { get; set; }

        [EnableInBuilder(BuilderType.WebForm)]
        public bool IsDisabledOnEdit { get; set; }

        [EnableInBuilder(BuilderType.WebForm)]
        [PropertyEditor(PropertyEditorType.ScriptEditorCS)]
        public EbScript ExecuteOnlyIf { get; set; }

        public override bool IsWarningOnly { get; set; }

        public override string FailureMSG { get; set; }

        public override bool IsDisabled { get; set; }
    }

    [UsedWithTopObjectParent(typeof(EbObject))]
    [EnableInBuilder(BuilderType.WebForm)]
    public class EbRoutine
    {
        public EbRoutine() { }

        [HideInPropertyGrid]
        [EnableInBuilder(BuilderType.WebForm)]
        public string EbSid { get; set; }

        [EnableInBuilder(BuilderType.WebForm)]
        public virtual string Name { get; set; }

        [EnableInBuilder(BuilderType.WebForm)]
        [PropertyEditor(PropertyEditorType.ScriptEditorCS, PropertyEditorType.ScriptEditorSQ)]
        public EbScript Script { get; set; }
    }
}
