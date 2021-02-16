using ExpressBase.Common.Constants;
using ExpressBase.Common.Extensions;
using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using ExpressBase.Common.Structures;
using System.Collections.Generic;

namespace ExpressBase.Objects
{
    [EnableInBuilder(BuilderType.MobilePage)]
    public class EbMobileDataLink : EbMobileDashBoardControl
    {
        [EnableInBuilder(BuilderType.MobilePage)]
        [HideInPropertyGrid]
        [DefaultPropValue("2")]
        public int RowCount { set; get; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [HideInPropertyGrid]
        [DefaultPropValue("2")]
        public int ColumCount { set; get; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyGroup(PGConstants.APPEARANCE)]
        [DefaultPropValue("5")]
        public int RowSpacing { set; get; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyGroup(PGConstants.APPEARANCE)]
        [DefaultPropValue("5")]
        public int ColumnSpacing { set; get; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyGroup(PGConstants.APPEARANCE)]
        [DefaultPropValue("100")]
        public int Width { set; get; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyEditor(PropertyEditorType.ObjectSelector)]
        [PropertyGroup("Link Settings")]
        [Alias("Link")]
        [OSE_ObjectTypes(EbObjectTypes.iMobilePage)]
        public string LinkRefId { get; set; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [HideInPropertyGrid]
        public List<EbMobileDataCell> CellCollection { set; get; }

        public override string GetDesignHtml()
        {
            return @"<div class='mob_dash_control dropped' id='@id' eb-type='EbMobileDataLink' tabindex='1' onclick='$(this).focus()'>                            
                            <div class='eb_dash_ctrlhtml'>
                              <div class ='eb_mob_datalink_layout'>                                 
                                  
                               </div>
                            </div>
                        </div>".RemoveCR().DoubleQuoted();
        }
    }

    [EnableInBuilder(BuilderType.MobilePage)]
    public class EbMobileDataCell : EbMobilePage
    {
        [EnableInBuilder(BuilderType.MobilePage)]
        public int RowIndex { set; get; }

        [EnableInBuilder(BuilderType.MobilePage)]
        public int ColIndex { set; get; }

        [EnableInBuilder(BuilderType.MobilePage)]
        public int Width { set; get; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [HideInPropertyGrid]
        public List<EbMobileDashBoardControl> ControlCollection { set; get; }
    }
}
