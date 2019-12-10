using ExpressBase.Common;
using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using ExpressBase.Common.Structures;
using ExpressBase.Objects.ServiceStack_Artifacts;
using Newtonsoft.Json;
using ServiceStack;
using ServiceStack.Redis;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExpressBase.Objects
{
  

    [EnableInBuilder(BuilderType.DashBoard)]
    public class EbDashBoardWraper : EbObject 
    {
        [HideForUser]
        [EnableInBuilder(BuilderType.DashBoard)]
        public override string Name { get; set; }

        [HideInPropertyGrid]
        [EnableInBuilder(BuilderType.DashBoard)]
        public override string RefId { get; set; }

        [HideForUser]
        [EnableInBuilder(BuilderType.DashBoard)]
        public override string DisplayName { get; set; }

        [HideInPropertyGrid]
        [EnableInBuilder(BuilderType.DashBoard)]
        public override string Description { get; set; }

        [HideInPropertyGrid]
        [EnableInBuilder(BuilderType.DashBoard)]
        public override string VersionNumber { get; set; }
        
        [HideInPropertyGrid]
        [EnableInBuilder(BuilderType.DashBoard)]
        public override string Status { get; set; }


        // methods
        public EbDashBoardWraper()
        {

        }
    }

    [EnableInBuilder(BuilderType.DashBoard)]
    [BuilderTypeEnum(BuilderType.DashBoard)]
    public class EbDashBoard : EbDashBoardWraper, IEBRootObject
    {
        [HideForUser]
        [EnableInBuilder(BuilderType.DashBoard)]
        [DefaultPropValue("2")]
        public int TileCount { get; set; }

        [EnableInBuilder(BuilderType.DashBoard)]
        [PropertyEditor(PropertyEditorType.ObjectSelector)]
        [PropertyGroup("Data Settings")]
        [OSE_ObjectTypes(EbObjectTypes.iFilterDialog)]
        public string Filter_Dialogue { get; set; }

        [EnableInBuilder(BuilderType.DashBoard)]
        [UIproperty]
        [PropertyGroup("Appearance")]
        [PropertyEditor(PropertyEditorType.Color)]
        public virtual string BackgroundColor { get; set; }

        
        [EnableInBuilder(BuilderType.DashBoard)]
        [PropertyGroup("Appearance")]
        [UIproperty]
        [PropertyEditor(PropertyEditorType.FontSelector)]
        [HideInPropertyGrid]
        public virtual EbFont Font { get; set; }

        [EnableInBuilder(BuilderType.DashBoard)]
        [HideInPropertyGrid]
        public List<Tiles> Tiles { get; set; }

        public static EbOperations Operations = DashBoardOperations.Instance;

        [JsonIgnore]
        public EbFilterDialog FilterDialog { get; set; }


        public EbDashBoard()
        {
            Tiles = new List<Tiles>();
        }

        public override void ReplaceRefid(Dictionary<string, string> RefidMap)
        {
            foreach (Tiles Tile in this.Tiles)
            {
                if (!string.IsNullOrEmpty(Tile.RefId))
                {
                    if (RefidMap.ContainsKey(Tile.RefId))
                        Tile.RefId = RefidMap[Tile.RefId];
                    else
                        Tile.RefId = "failed-to-update-";
                }
            }
        }

        public override List<string> DiscoverRelatedRefids()
        {
            List<string> _refids = new List<string>();
            foreach(var Tile in this.Tiles)
            {
                if(!string.IsNullOrEmpty(Tile.TileRefId))
                    _refids.Add(Tile.TileRefId);
            }
            return _refids;
        }

        public override void AfterRedisGet(RedisClient Redis, IServiceClient client)
        { 
            try
            {
                this.FilterDialog = Redis.Get<EbFilterDialog>(this.Filter_Dialogue);
                if (this.FilterDialog == null && this.Filter_Dialogue != "")
                {
                    var result = client.Get<EbObjectParticularVersionResponse>(new EbObjectParticularVersionRequest { RefId = this.Filter_Dialogue });
                    this.FilterDialog = EbSerializers.Json_Deserialize(result.Data[0].Json);
                    Redis.Set<EbFilterDialog>(this.Filter_Dialogue, this.FilterDialog);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception:" + e.ToString());
            }
        }

    }

    [EnableInBuilder(BuilderType.DashBoard)]
    public class Tiles : EbDashBoardWraper
    {
        [EnableInBuilder(BuilderType.DashBoard)]
        [PropertyEditor(PropertyEditorType.ObjectSelector)]
        [OSE_ObjectTypes(EbObjectTypes.iTableVisualization, EbObjectTypes.iChartVisualization, EbObjectTypes.iGoogleMap, EbObjectTypes.iUserControl)]

        public string TileRefId { get; set; }

        [HideInPropertyGrid]
        [EnableInBuilder(BuilderType.DashBoard)]
        public TileDivRef TileDiv { get; set; }

        
    }

    [EnableInBuilder(BuilderType.DashBoard)]
    public class TileDivRef : EbDashBoardWraper
    {
        [EnableInBuilder(BuilderType.DashBoard)]
        public int Data_x { get; set; }

        [EnableInBuilder(BuilderType.DashBoard)]
        public int Data_y { get; set; }

        [EnableInBuilder(BuilderType.DashBoard)]
        public int Data_height { get; set; }

        [EnableInBuilder(BuilderType.DashBoard)]
        public int Data_width { get; set; }

    }
  
}
