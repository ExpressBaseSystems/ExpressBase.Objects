using ExpressBase.Common;
using ExpressBase.Common.Extensions;
using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using ExpressBase.Common.Structures;
using System.Collections.Generic;

namespace ExpressBase.Objects
{
    [EnableInBuilder(BuilderType.MobilePage)]
    public class EbMobileButton : EbMobileControl, IMobileLink, INonPersistControl
    {
        public override string Label { set; get; }
        public override bool Unique { get; set; }
        public override bool ReadOnly { get; set; }
        public override bool DoNotPersist { get; set; }
        public override bool Required { get; set; }
        public override bool Hidden { set; get; }
        public override EbScript ValueExpr { get; set; }
        public override EbScript HiddenExpr { get; set; }
        public override EbScript DisableExpr { get; set; }
        public override EbScript DefaultValueExpression { get; set; }
        public override List<EbMobileValidator> Validators { set; get; }

        #region
        /// <summary>
        /// link setting 
        /// props DataColumns,FormControlMetas are for set values 
        /// </summary>

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyEditor(PropertyEditorType.ObjectSelector)]
        [OSE_ObjectTypes(EbObjectTypes.iMobilePage)]
        [PropertyGroup("Link Settings")]
        [Alias("Link")]
        public string LinkRefId { get; set; }

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
        public WebFormDVModes FormMode { set; get; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyEditor(PropertyEditorType.CollectionFrmSrc, "DataColumns", 1)]
        [PropertyGroup("Link Settings")]
        public EbMobileDataColToControlMap FormId { set; get; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyGroup("Link Settings")]
        [Alias("Columns to controls map")]
        [PropertyEditor(PropertyEditorType.Mapper, "DataColumns", "FormControlMetas", "FormControl")]
        public List<EbMobileDataColToControlMap> LinkFormParameters { get; set; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [MetaOnly]
        public List<EbMobileDataColToControlMap> DataColumns => new List<EbMobileDataColToControlMap>();

        [EnableInBuilder(BuilderType.MobilePage)]
        [MetaOnly]
        public List<EbMobileControlMeta> FormControlMetas => new List<EbMobileControlMeta>();
        /// <summary>
        /// ending region for link
        /// </summary>
        #endregion 

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyGroup("UI")]
        public int RowSpan { set; get; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyGroup("UI")]
        public int ColumnSpan { set; get; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyGroup("UI")]
        [DefaultPropValue("30")]
        public int Width { set; get; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyGroup("UI")]
        [DefaultPropValue("30")]
        public int Height { set; get; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyGroup("UI")]
        public string Text { set; get; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyGroup("UI")]
        [UIproperty]
        [PropertyEditor(PropertyEditorType.FontSelector)]
        public EbFont Font { get; set; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyGroup("UI")]
        [PropertyEditor(PropertyEditorType.Color)]
        public string BackgroundColor { get; set; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyGroup("UI")]
        public int BorderThickness { get; set; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyGroup("UI")]
        [PropertyEditor(PropertyEditorType.Color)]
        public string BorderColor { get; set; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyGroup("UI")]
        public int BorderRadius { get; set; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyGroup("UI")]
        public bool RenderTextAsIcon { get; set; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyGroup("UI")]
        [Alias("Align X")]
        public MobileHorrizontalAlign HorrizontalAlign { set; get; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyGroup("UI")]
        [Alias("Align Y")]
        public MobileVerticalAlign VerticalAlign { set; get; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyGroup("UI")]
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
}
