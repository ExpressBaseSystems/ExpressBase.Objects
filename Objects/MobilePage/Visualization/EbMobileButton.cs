using ExpressBase.Common;
using ExpressBase.Common.Constants;
using ExpressBase.Common.Extensions;
using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using ExpressBase.Common.Structures;
using System.Collections.Generic;

namespace ExpressBase.Objects
{
    [EnableInBuilder(BuilderType.MobilePage)]
    public class EbMobileButton : EbMobileControl, IMobileLink, INonPersistControl, IMobileUIControl
    {
        public override string Label { set; get; }
        public override bool Unique { get; set; }
        public override bool ReadOnly { get; set; }
        public override bool DoNotPersist { get; set; }
        public override bool Required { get; set; }
        public override bool Hidden { set; get; }
        public override EbScript ValueExpr { get; set; }
        public override EbScript DisableExpr { get; set; }
        public override EbScript DefaultValueExpression { get; set; }
        public override List<EbMobileValidator> Validators { set; get; }
        public EbThickness Padding { set; get; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyGroup(PGConstants.ALIGNMENT)]
        public int RowSpan { set; get; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyGroup(PGConstants.ALIGNMENT)]
        public int ColumnSpan { set; get; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyGroup(PGConstants.ALIGNMENT)]
        [Alias("Align X")]
        public MobileHorrizontalAlign HorrizontalAlign { set; get; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyGroup(PGConstants.ALIGNMENT)]
        [Alias("Align Y")]
        public MobileVerticalAlign VerticalAlign { set; get; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyEditor(PropertyEditorType.ObjectSelector)]
        [OSE_ObjectTypes(EbObjectTypes.iMobilePage)]
        [PropertyGroup("Link Settings")]
        [Alias("Link")]
        [OnChangeExec(@"
                if (this.LinkRefId && this.LinkTypeForm){ 
                        pg.ShowPropertiesExt(['FormMode', 'FormId', 'LinkFormParameters']);
                }
                else {
                        pg.HidePropertiesExt(['FormMode', 'FormId', 'LinkFormParameters']);
                }
            ")]
        public virtual string LinkRefId { get; set; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyGroup("Link Settings")]
        [OnChangeExec(@"
                if (this.FormMode === 1){ 
                        pg.ShowProperty('FormId');
                }
                else {
                        pg.HideProperty('FormId');
                }
            ")]
        public virtual WebFormDVModes FormMode { set; get; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyEditor(PropertyEditorType.CollectionFrmSrc, "DataColumns", 1)]
        [PropertyGroup("Link Settings")]
        public virtual EbMobileDataColToControlMap FormId { set; get; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyGroup("Link Settings")]
        [Alias("Columns to controls map")]
        [PropertyEditor(PropertyEditorType.Mapper, "DataColumns", "FormControlMetas", "FormControl")]
        public virtual List<EbMobileDataColToControlMap> LinkFormParameters { get; set; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [MetaOnly]
        public List<EbMobileDataColToControlMap> DataColumns => new List<EbMobileDataColToControlMap>();

        [EnableInBuilder(BuilderType.MobilePage)]
        [MetaOnly]
        public List<EbMobileControlMeta> FormControlMetas => new List<EbMobileControlMeta>();

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyGroup(PGConstants.DATA)]
        public virtual string Text { set; get; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyGroup(PGConstants.DATA)]
        public virtual bool RenderTextAsIcon { get; set; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyGroup(PGConstants.APPEARANCE)]
        [PropertyEditor(PropertyEditorType.Color)]
        public string BackgroundColor { get; set; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyGroup(PGConstants.APPEARANCE)]
        public int BorderRadius { get; set; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyGroup(PGConstants.APPEARANCE)]
        public int BorderThickness { get; set; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyGroup(PGConstants.APPEARANCE)]
        [PropertyEditor(PropertyEditorType.Color)]
        public string BorderColor { get; set; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyGroup(PGConstants.APPEARANCE)]
        [UIproperty]
        [PropertyEditor(PropertyEditorType.FontSelector)]
        public EbFont Font { get; set; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyGroup(PGConstants.APPEARANCE)]
        [DefaultPropValue("30")]
        public int Width { set; get; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyGroup(PGConstants.APPEARANCE)]
        [DefaultPropValue("30")]
        public int Height { set; get; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyGroup(PGConstants.BEHAVIOR)]
        public bool HideInContext { set; get; }

        public override string GetDesignHtml()
        {
            return @"<div class='eb_stacklayout mob_control dropped' id='@id' eb-type='EbMobileButton' tabindex='1' onclick='$(this).focus()'>
                            <div class='eb_btnctrlhtml'>
                               <button class='ebm-btn'>Button</button>
                            </div>
                        </div>".RemoveCR().DoubleQuoted();
        }

        public EbMobileButton()
        {
            LinkFormParameters = new List<EbMobileDataColToControlMap>();
        }

        public override List<string> DiscoverRelatedRefids()
        {
            List<string> list = new List<string>();

            if (!string.IsNullOrEmpty(LinkRefId))
                list.Add(LinkRefId);

            return list;
        }

        public override void ReplaceRefid(Dictionary<string, string> map)
        {
            if (!string.IsNullOrEmpty(LinkRefId) && map.TryGetValue(LinkRefId, out string dsri))
                this.LinkRefId = dsri;
        }
    }

    [EnableInBuilder(BuilderType.MobilePage)]
    public class EbMobileApprovalButton : EbMobileButton
    {
        public override string LinkRefId { get; set; }
        public override WebFormDVModes FormMode { set; get; }
        public override string Text { set; get; }
        public override bool RenderTextAsIcon { get; set; }
        public override List<EbMobileDataColToControlMap> LinkFormParameters { get; set; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyEditor(PropertyEditorType.ObjectSelector)]
        [OSE_ObjectTypes(EbObjectTypes.iWebForm)]
        [PropertyGroup(PGConstants.CORE)]
        public string FormRefid { get; set; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyEditor(PropertyEditorType.CollectionFrmSrc, "DataColumns", 1)]
        [PropertyGroup(PGConstants.CORE)]
        public override EbMobileDataColToControlMap FormId { set; get; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyGroup(PGConstants.CORE)]
        [Alias("Stage Column Name")]
        public string StageColumn { set; get; }

        public EbDataSet ApprovalData { set; get; }

        public override string GetDesignHtml()
        {
            return @"<div class='eb_stacklayout mob_control dropped' id='@id' eb-type='EbMobileApprovalButton' tabindex='1' onclick='$(this).focus()'>
                            <div class='eb_btnctrlhtml h-100'>
                               <button class='ebm-btn h-100'>Approval Button</button>
                            </div>
                        </div>".RemoveCR().DoubleQuoted();
        }

        public override List<string> DiscoverRelatedRefids()
        {
            List<string> list = new List<string>();

            if (!string.IsNullOrEmpty(FormRefid))
                list.Add(FormRefid);

            return list;
        }

        public override void ReplaceRefid(Dictionary<string, string> map)
        {
            if (!string.IsNullOrEmpty(FormRefid) && map.TryGetValue(FormRefid, out string dsri))
                this.FormRefid = dsri;
        }
    }
}
