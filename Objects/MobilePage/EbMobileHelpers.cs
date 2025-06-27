using ExpressBase.Common.Constants;
using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using ExpressBase.Common.Structures;
using System.Collections.Generic;

namespace ExpressBase.Objects
{
    [EnableInBuilder(BuilderType.MobilePage)]
    public class EbMobileDataColToControlMap : EbMobilePageBase
    {
        [EnableInBuilder(BuilderType.MobilePage)]
        [HideInPropertyGrid]
        public string EbSid { set; get; }

        [EnableInBuilder(BuilderType.MobilePage)]
        public string ColumnName { set; get; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [HideInPropertyGrid]
        public EbDbTypes Type { get; set; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [HideInPropertyGrid]
        public EbMobileControlMeta FormControl { set; get; }
    }

    [EnableInBuilder(BuilderType.MobilePage)]
    public class EbMobileControlMeta : EbMobilePageBase
    {
        [EnableInBuilder(BuilderType.MobilePage)]
        [HideInPropertyGrid]
        public string EbSid { set; get; }

        [EnableInBuilder(BuilderType.MobilePage)]
        public string ControlName { set; get; }

        [EnableInBuilder(BuilderType.MobilePage)]
        public string ControlType { set; get; }
    }

    [EnableInBuilder(BuilderType.MobilePage)]
    public class EbCTCMapper : EbMobilePageBase
    {
        [EnableInBuilder(BuilderType.MobilePage)]
        [HideInPropertyGrid]
        public string EbSid { set; get; }

        [EnableInBuilder(BuilderType.MobilePage)]
        public string ColumnName { set; get; }

        [EnableInBuilder(BuilderType.MobilePage)]
        public string ControlName { set; get; }
    }

    [EnableInBuilder(BuilderType.MobilePage)]
    public class EbMobileLinkCollection : EbMobilePageBase
    {
        [EnableInBuilder(BuilderType.MobilePage)]
        [HideInPropertyGrid]
        public string EbSid { set; get; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [Alias("Link Name")]
        public string LinkName { get; set; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyEditor(PropertyEditorType.ObjectSelector)]
        [OSE_ObjectTypes(EbObjectTypes.iReport, EbObjectTypes.iPrintLayout)]
        [Alias("Link")]
        public string LinkRefId { get; set; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyEditor(PropertyEditorType.Collection)]
        [Alias("Context to controls map")]
        public List<EbCTCMapper> ContextToControlMap { set; get; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyEditor(PropertyEditorType.ScriptEditorCS)]
        [Alias("Link Expression")]
        public EbScript LinkExpr { get; set; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [Alias("Link Fail Message")]
        public string LinkExprFailMsg { get; set; }
    }

    [EnableInBuilder(BuilderType.MobilePage)]
    public class EbThickness
    {
        [EnableInBuilder(BuilderType.MobilePage)]
        public int Left { set; get; }

        [EnableInBuilder(BuilderType.MobilePage)]
        public int Top { set; get; }

        [EnableInBuilder(BuilderType.MobilePage)]
        public int Right { set; get; }

        [EnableInBuilder(BuilderType.MobilePage)]
        public int Bottom { set; get; }

        public EbThickness() { }

        public EbThickness(int value)
        {
            Left = Top = Right = Bottom = value;
        }

        public EbThickness(int left, int top, int right, int bottom)
        {
            Left = left;
            Top = top;
            Right = right;
            Bottom = bottom;
        }
    }

    [EnableInBuilder(BuilderType.MobilePage)]
    public class EbMobileValidator : EbMobilePageBase
    {
        [HideInPropertyGrid]
        [EnableInBuilder(BuilderType.MobilePage)]
        public string EbSid { get; set; }

        [EnableInBuilder(BuilderType.MobilePage)]
        public bool IsDisabled { get; set; }

        [EnableInBuilder(BuilderType.MobilePage)]
        public bool IsWarningOnly { get; set; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyEditor(PropertyEditorType.ScriptEditorCS)]
        public EbScript Script { get; set; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [Alias("Failure message")]
        public string FailureMSG { get; set; }
    }

    [EnableInBuilder(BuilderType.MobilePage)]
    public class EbMobileStaticParameter : EbMobilePageBase
    {
        [HideInPropertyGrid]
        [EnableInBuilder(BuilderType.MobilePage)]
        public string EbSid { get; set; }

        [EnableInBuilder(BuilderType.MobilePage)]
        public override string Name { get; set; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyGroup(PGConstants.CORE)]
        public string Value { set; get; }

        [EnableInBuilder(BuilderType.MobilePage)]
        public bool EnableSearch { set; get; }
    }

    [EnableInBuilder(BuilderType.MobilePage)]
    public class EbMobileStaticListItem : EbMobilePageBase
    {
        [HideInPropertyGrid]
        [EnableInBuilder(BuilderType.MobilePage)]
        public string EbSid { get; set; }

        [HideInPropertyGrid]
        [EnableInBuilder(BuilderType.MobilePage)]
        public override string Name { get; set; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyEditor(PropertyEditorType.Collection)]
        [PropertyGroup(PGConstants.CORE)]
        public List<EbMobileStaticParameter> Parameters { set; get; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyEditor(PropertyEditorType.ObjectSelector)]
        [OSE_ObjectTypes(EbObjectTypes.iMobilePage)]
        [Alias("Link")]
        [PropertyGroup(PGConstants.CORE)]
        public string LinkRefId { get; set; }
    }
}
