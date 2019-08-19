using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using ExpressBase.Common.Structures;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExpressBase.Objects
{
    [EnableInBuilder(BuilderType.DashBoard)]
    public class EbDashBoardWraper : EbObject 
    {

        public EbDashBoardWraper()
        {

        }
    }

    [EnableInBuilder(BuilderType.DashBoard)]
    [BuilderTypeEnum(BuilderType.DashBoard)]
    public class EbDashBoard : EbDashBoardWraper, IEBRootObject
    {
        [EnableInBuilder(BuilderType.DashBoard)]
        [DefaultPropValue("2")]
        public int TileCount { get; set; }

        [EnableInBuilder(BuilderType.DashBoard)]
        public List<Tiles> Tiles { get; set; }

        public EbDashBoard()
        {
            Tiles = new List<Tiles>();
        }
    }

    [EnableInBuilder(BuilderType.DashBoard)]
    public class Tiles : EbDashBoardWraper
    {
        [EnableInBuilder(BuilderType.DashBoard)]
        [PropertyEditor(PropertyEditorType.ObjectSelector)]
        [OSE_ObjectTypes(EbObjectTypes.iTableVisualization, EbObjectTypes.iChartVisualization, EbObjectTypes.iGoogleMap)]
        public string TileRefId { get; set; }

        [EnableInBuilder(BuilderType.DashBoard)]
        public EbObject TileObject { get; set; }

        public Tiles()
        {

        }
    }
}
