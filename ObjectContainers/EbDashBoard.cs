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
            foreach (var Tile in this.Tiles)
            {
                if (!string.IsNullOrEmpty(Tile.TileRefId))
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
        [HideInPropertyGrid]
        public string TileRefId { get; set; }


        [EnableInBuilder(BuilderType.DashBoard)]
        public bool Transparent { get; set; }


        [EnableInBuilder(BuilderType.WebForm, BuilderType.DashBoard, BuilderType.UserControl)]
        [PropertyGroup("TileConfig")]
        [OnChangeUIFunction("EbDataLabel.LabelBackgroudColor")]
        [PropertyEditor(PropertyEditorType.Color)]
        public  string TileBackColor { get; set; }

        [EnableInBuilder(BuilderType.DashBoard)]
        [PropertyGroup("TileConfig")]
        [OnChangeExec(@"
                if (this.IsGradient === true ){      
                        pg.ShowProperty('GradientColor1');     
                        pg.ShowProperty('GradientColor2');     
                        pg.ShowProperty('Direction');
                        pg.HideProperty('LabelBackColor');
                }
                else {
                        pg.ShowProperty('LabelBackColor');    
                        pg.HideProperty('GradientColor1');     
                        pg.HideProperty('GradientColor2');     
                        pg.HideProperty('Direction');      
                }
            ")]
        public bool IsGradient { get; set; }

        [EnableInBuilder(BuilderType.DashBoard)]
        [PropertyEditor(PropertyEditorType.Color)]
        [PropertyGroup("TileConfig")]
        [OnChangeUIFunction("EbDataLabel.LabelGradientColor")]
        public string GradientColor1 { get; set; }

        [EnableInBuilder(BuilderType.DashBoard)]
        [PropertyEditor(PropertyEditorType.Color)]
        [PropertyGroup("TileConfig")]
        [OnChangeUIFunction("EbDataLabel.LabelGradientColor")]
        public string GradientColor2 { get; set; }

        [EnableInBuilder(BuilderType.DashBoard)]
        [PropertyGroup("TileConfig")]
        public GradientDirection Direction { get; set; }

        [EnableInBuilder(BuilderType.DashBoard)]
        [PropertyGroup("TileConfig")]
        [DefaultPropValue("#c3c3c3")]
        [PropertyEditor(PropertyEditorType.Color)]
        public string BorderColor { get; set; }

        [EnableInBuilder(BuilderType.DashBoard)]
        [PropertyGroup("TileConfig")]
        [DefaultPropValue("4")]
        [OnChangeExec(@"if(this.BorderRadius > 50){
            this.BorderRadius = 50;
            $('#' + pg.wraperId + 'BorderRadius').val(50);
            }
            else if(this.BorderRadius < 0){
            this.DivWidth = 0;
            $('#' + pg.wraperId + 'BorderRadius').val(0);
            }
            ")]
        public int BorderRadius { get; set; }


        [EnableInBuilder(BuilderType.WebForm, BuilderType.UserControl, BuilderType.DashBoard)]
        [PropertyGroup("Label")]
        [UIproperty]
        public string Label { get; set; }

        [EnableInBuilder(BuilderType.DashBoard)]
        [PropertyGroup("Label")]
        [UIproperty]
        [PropertyEditor(PropertyEditorType.FontSelector)]
        [OnChangeUIFunction("EbDataLabel.Style4StaticLabel")]
        public EbFont LabelFont { get; set; }

        [EnableInBuilder(BuilderType.DashBoard)]
        [PropertyGroup("Label")]
        [UIproperty]
        public int Left { get; set; }
        
        [EnableInBuilder(BuilderType.DashBoard)]
        [PropertyGroup("Label")]
        [UIproperty]
        public int  Top { get; set; }


        [HideInPropertyGrid]
        [EnableInBuilder(BuilderType.DashBoard)]
        public TileDivRef TileDiv { get; set; }

        [EnableInBuilder(BuilderType.DashBoard)]
        [HideInPropertyGrid]
        public List<EbControl> ControlsColl { get; set; }

        [EnableInBuilder(BuilderType.DashBoard)]
        [HideInPropertyGrid]
        public List<EbControl> ComponentsColl { get; set; }

        [EnableInBuilder(BuilderType.DashBoard)]
        [HideInPropertyGrid]
        public List<EbControl> LabelColl { get; set; }

        public Tiles()
        {
            this.ControlsColl = new List<EbControl>();
            this.ComponentsColl = new List<EbControl>();
            this.LabelColl = new List<EbControl>();
        }

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
