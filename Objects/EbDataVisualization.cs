using ExpressBase.Common;
using ExpressBase.Common.Constants;
using ExpressBase.Common.Data;
using ExpressBase.Common.Extensions;
using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using ExpressBase.Common.Structures;
using ExpressBase.Data;
using ExpressBase.Objects.Objects.DVRelated;
using ExpressBase.Objects.ServiceStack_Artifacts;
using Newtonsoft.Json;
using Npgsql;
using Npgsql.Schema;
using ServiceStack;
using ServiceStack.Redis;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace ExpressBase.Objects
{
    public enum ChartType
    {
        Default,
        GoogleMap
    }

    public enum ControlType
    {
        Default,
        Date,
        Numeric,
        Text
    }

    public enum Position
    {
        top,
        left,
        bottom,
        right
    }

    public enum LinkTypeEnum
    {
        Popout,
        Inline,
        Both,
        Popup,
        Tab
    }

    [EnableInBuilder(BuilderType.BotForm, BuilderType.DashBoard, BuilderType.Calendar)]
    public class EbDataVisualizationObject : EbObject
    {

    }

    [EnableInBuilder(BuilderType.DVBuilder, BuilderType.DashBoard, BuilderType.Calendar)]
    public class ChartColor : EbDataVisualizationObject
    {
        [EnableInBuilder(BuilderType.DVBuilder)]
        public string Color { get; set; }
    }

    [EnableInBuilder(BuilderType.DVBuilder, BuilderType.DashBoard, BuilderType.Calendar)]
    public class EbDataVisualizationSet : EbDataVisualizationObject
    {

        [EnableInBuilder(BuilderType.DVBuilder)]
        [PropertyEditor(PropertyEditorType.Collection)]
        public List<EbDataVisualization> Visualizations { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder)]
        public int DeafaultVisualizationIndex { get; set; }

        public EbDataVisualizationSet()
        {
            this.Visualizations = new List<EbDataVisualization>();
        }
    }


    public abstract class EbDataVisualization : EbDataVisualizationObject
    {
        [EnableInBuilder(BuilderType.DVBuilder, BuilderType.DashBoard, BuilderType.Calendar)]
        [HideInPropertyGrid]
        public override string RefId { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder, BuilderType.DashBoard, BuilderType.Calendar)]
        [PropertyGroup(PGConstants.CORE)]
        public override string DisplayName { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder, BuilderType.DashBoard, BuilderType.Calendar)]
        [PropertyGroup(PGConstants.CORE)]
        [HideInPropertyGrid]
        public override string Name { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder, BuilderType.DashBoard, BuilderType.Calendar)]
        public override string Description { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder, BuilderType.DashBoard, BuilderType.Calendar)]
        [HideInPropertyGrid]
        public override string VersionNumber { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder, BuilderType.DashBoard, BuilderType.Calendar)]
        [HideInPropertyGrid]
        public override string Status { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder, BuilderType.DashBoard, BuilderType.Calendar)]
        [PropertyEditor(PropertyEditorType.ObjectSelector)]
        [OSE_ObjectTypes(EbObjectTypes.iDataReader)]
        [HideForUser]
        [PropertyPriority(0)]
        [Alias("Data Reader")]
        [PropertyGroup(PGConstants.CORE)]
        public string DataSourceRefId { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder, BuilderType.DashBoard, BuilderType.Calendar)]
        [HideInPropertyGrid]
        public string Url { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder, BuilderType.DashBoard, BuilderType.Calendar)]
        [HideInPropertyGrid]
        public string Sql { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder, BuilderType.DashBoard, BuilderType.Calendar)]
        [HideInPropertyGrid]
        public string EbSid { get; set; }

        [JsonIgnore]
        public EbDataReader EbDataSource { get; set; }

        [PropertyEditor(PropertyEditorType.CollectionABCpropToggle, "Columns", "bVisible", "_Formula")]
        [CEOnSelectFn(@";
            this.bVisible = true;
            NonVC = Parent.NotVisibleColumns.$values;
            let  idx = NonVC.findIndex(x => x.name === this.name);
            if(idx > -1)
                NonVC.splice(idx, 1);")]
        [CEOnDeselectFn(@"
            this.bVisible = false;
            Parent.NotVisibleColumns.$values.push(this)")]
        [EnableInBuilder(BuilderType.DVBuilder, BuilderType.DashBoard, BuilderType.Calendar)]
        [PropertyGroup("User Actions")]
        [PropertyPriority(2)]
        [HideInPropertyGrid]
        public virtual DVColumnCollection Columns { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder)]
        [HideInPropertyGrid]
        public DVColumnCollection DSColumns { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder)]
        [HideInPropertyGrid]
        public List<DVColumnCollection> ColumnsCollection { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder)]
        [HideInPropertyGrid]
        public List<Param> ParamsList { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder, BuilderType.DashBoard, BuilderType.Calendar)]
        [HideInPropertyGrid]
        public List<DVBaseColumn> NotVisibleColumns { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder)]
        [HideInPropertyGrid]
        [JsonIgnore]
        public object data { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder)]
        [HideInPropertyGrid]
        public string Pippedfrom { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder)]
        [HideInPropertyGrid]
        public bool AutoGen { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder, BuilderType.DashBoard)]
        [DefaultPropValue("true")]
        [PropertyGroup("Paging")]
        [OnChangeExec(@"
        if(this.IsPaging){
            pg.ShowProperty('PageLength')
        }

        else{
            pg.HideProperty('PageLength')
        }")]
        public virtual bool IsPaging { get; set; }

        public override void AfterRedisGet(RedisClient Redis)
        {
            try
            {
                this.EbDataSource = Redis.Get<EbDataReader>(this.DataSourceRefId);
                this.EbDataSource.AfterRedisGet(Redis);
            }
            catch (Exception e)
            {
                Console.WriteLine("AfterRedisGet " + e.Message);
            }
        }

        public override void AfterRedisGet(RedisClient Redis, IServiceClient client)
        {
            try
            {
                this.EbDataSource = Redis.Get<EbDataReader>(this.DataSourceRefId);
                if (this.EbDataSource == null || this.EbDataSource.Sql == null || this.EbDataSource.Sql == string.Empty)
                {
                    var result = client.Get<EbObjectParticularVersionResponse>(new EbObjectParticularVersionRequest { RefId = this.DataSourceRefId });
                    this.EbDataSource = EbSerializers.Json_Deserialize(result.Data[0].Json);
                    Redis.Set<EbDataReader>(this.DataSourceRefId, this.EbDataSource);
                }
                if (this.EbDataSource.FilterDialogRefId != string.Empty)
                    this.EbDataSource.AfterRedisGet(Redis, client);
            }
            catch (Exception e)
            {
                Console.WriteLine("AfterRedisGet " + e.Message);
            }
        }

        public void AfterRedisGet(IRedisClient Redis, Service service)
        {
            try
            {
                this.EbDataSource = Redis.Get<EbDataReader>(this.DataSourceRefId);
                if (this.EbDataSource == null || this.EbDataSource.Sql == null || this.EbDataSource.Sql == string.Empty)
                {
                    var result = service.Gateway.Send<EbObjectParticularVersionResponse>(new EbObjectParticularVersionRequest { RefId = this.DataSourceRefId });
                    this.EbDataSource = EbSerializers.Json_Deserialize(result.Data[0].Json);
                    Redis.Set<EbDataReader>(this.DataSourceRefId, this.EbDataSource);
                }
                if (this.EbDataSource.FilterDialogRefId != string.Empty)
                    this.EbDataSource.AfterRedisGet(Redis, service);
            }
            catch (Exception e)
            {
                Console.WriteLine("AfterRedisGet " + e.Message);
            }
        }    

    }

    [EnableInBuilder(BuilderType.DVBuilder, BuilderType.BotForm, BuilderType.DashBoard, BuilderType.Calendar)]
    [BuilderTypeEnum(BuilderType.DVBuilder)]
    [UsedWithTopObjectParent(typeof(EbObject))]
    public class EbTableVisualization : EbDataVisualization, IEBRootObject
    {

        [OnDeserialized]
        public void OnDeserializedMethod(StreamingContext context)
        {
            this.BareControlHtml = this.GetBareHtml();
            this.ObjType = this.GetType().Name.Substring(2, this.GetType().Name.Length - 2);
        }

        public string BotCols { get; set; }

        public string BotData { get; set; }

        [HideInPropertyGrid]
        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.Calendar)]
        public string ObjType { get; set; }

        [HideInPropertyGrid]
        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.Calendar)]
        public string BareControlHtml { get; set; }

        [OnChangeExec(@"
        if(this.RowGroupCollection.$values.length > 0){
            pg.HideProperty('LeftFixedColumn')
            pg.HideProperty('RightFixedColumn')
        }

        else{
            pg.ShowProperty('LeftFixedColumn')
            pg.ShowProperty('RightFixedColumn')
        }")]
        [EnableInBuilder(BuilderType.DVBuilder, BuilderType.DashBoard, BuilderType.Calendar)]
        [PropertyEditor(PropertyEditorType.Collection)]
        [HideForUser]
        [HideInPropertyGrid]
        public List<RowGroupParent> RowGroupCollection { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder, BuilderType.DashBoard, BuilderType.Calendar)]
        [HideInPropertyGrid]
        public RowGroupParent CurrentRowGroup { get; set; }

        [PropertyGroup("Fixed Column")]
        [EnableInBuilder(BuilderType.DVBuilder, BuilderType.DashBoard, BuilderType.Calendar)]
        public int LeftFixedColumn { get; set; }

        [PropertyGroup("Fixed Column")]
        [EnableInBuilder(BuilderType.DVBuilder, BuilderType.DashBoard, BuilderType.Calendar)]
        public int RightFixedColumn { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder)]
        [DefaultPropValue("100")]
        [PropertyGroup("Paging")]
        public int PageLength { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder, BuilderType.DashBoard, BuilderType.Calendar)]
        [HideInPropertyGrid]
        public bool DisableRowGrouping { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder)]
        [HideInPropertyGrid]
        public string SecondaryTableMapField { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder, BuilderType.DashBoard)]
        [HideForUser]
        [PropertyGroup(PGConstants.APPEARANCE)]
        public bool DisableCopy { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder, BuilderType.DashBoard)]
        [HideForUser]
        [PropertyGroup(PGConstants.APPEARANCE)]
        public bool AllowMultilineHeader { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder, BuilderType.DashBoard, BuilderType.Calendar)]
        [PropertyEditor(PropertyEditorType.CollectionFrmSrc, "Columns")]
        [PropertyGroup("User Actions")]
        [PropertyPriority(1)]
        [HideInPropertyGrid]
        public List<DVBaseColumn> OrderBy { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder, BuilderType.DashBoard, BuilderType.Calendar)]
        [HideInPropertyGrid]
        public List<FormLink> FormLinks { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder, BuilderType.Calendar)]
        [DefaultPropValue("15")]
        [HideForUser]
        [PropertyGroup(PGConstants.APPEARANCE)]
        public int RowHeight { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder, BuilderType.Calendar)]
        [HideForUser]
        public bool AllowLocalSearch { get; set; }

        [JsonIgnore]
        public EbWebForm WebForm { get; set; }

        public static EbOperations Operations = TVOperations.Instance;

        public EbTableVisualization()
        {
            this.RowGroupCollection = new List<RowGroupParent>();
            this.NotVisibleColumns = new List<DVBaseColumn>();
            this.CurrentRowGroup = new RowGroupParent();
            this.OrderBy = new List<DVBaseColumn>();
            this.ColumnsCollection = new List<DVColumnCollection>();
            this.ParamsList = new List<Param>();
            this.FormLinks = new List<FormLink>();
        }

        public override List<string> DiscoverRelatedRefids()
        {
            List<string> refids = new List<string>();
            if (!DataSourceRefId.IsEmpty())
            {
                EbDataReader ds = EbDataSource;
                if (ds is null)
                {
                    refids.Add(DataSourceRefId);
                }
            }
            foreach (DVBaseColumn _col in Columns)
            {
                if (!_col.LinkRefId.IsNullOrEmpty())
                    refids.Add(_col.LinkRefId);
                if (!_col.GroupFormLink.IsNullOrEmpty())
                    refids.Add(_col.GroupFormLink);
                if (!_col.ItemFormLink.IsNullOrEmpty())
                    refids.Add(_col.ItemFormLink);
            }
            return refids;
        }

        public override void ReplaceRefid(Dictionary<string, string> RefidMap)
        {
            if (!DataSourceRefId.IsEmpty())
            {
                string dsid = DataSourceRefId;
                if (RefidMap.ContainsKey(dsid))
                    DataSourceRefId = RefidMap[dsid];
                else
                    DataSourceRefId = "failed-to-update-";
            }
            foreach (DVBaseColumn _col in Columns)
            {
                if (!_col.LinkRefId.IsNullOrEmpty())
                {
                    if (RefidMap.ContainsKey(_col.LinkRefId))
                        _col.LinkRefId = RefidMap[_col.LinkRefId];
                    else if (RefidMap.ContainsKey(_col.GroupFormLink))
                        _col.GroupFormLink = RefidMap[_col.GroupFormLink];
                    else if (RefidMap.ContainsKey(_col.ItemFormLink))
                        _col.ItemFormLink = RefidMap[_col.ItemFormLink];
                    else
                        _col.LinkRefId = "failed-to-update-";
                }
            }
        }

        public void AfterRedisGetBasicInfo(IServiceClient client, IRedisClient Redis)
        {
            this.FormLinks = new List<FormLink>();
            foreach (DVBaseColumn col in this.Columns)
            {
                if (col.Check4FormLink())
                {
                    try
                    {
                        this.WebForm = Redis.Get<EbWebForm>(col.LinkRefId);
                        if (this.WebForm == null)
                        {
                            var result = client.Get<EbObjectParticularVersionResponse>(new EbObjectParticularVersionRequest { RefId = col.LinkRefId });
                            this.WebForm = EbSerializers.Json_Deserialize(result.Data[0].Json);
                            Redis.Set<EbWebForm>(col.LinkRefId, this.WebForm);
                        }
                        this.FormLinks.Add(new FormLink { DisplayName = this.WebForm.DisplayName, Refid = col.LinkRefId, Params = col.FormParameters });
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("AfterRedisGetBasicInfo " + e.Message);
                    }
                }
            }
        }

        public override void BeforeSave(IServiceClient serviceClient, IRedisClient redis)
        {
            this.AfterRedisGetBasicInfo(serviceClient, redis);
        }

        public void BeforeSave(Service service, IRedisClient redis)
        {
            this.AfterRedisGetBasicInfoByService(service, redis);
        }

        public void AfterRedisGetBasicInfoByService(Service service, IRedisClient Redis)
        {
            this.FormLinks = new List<FormLink>();
            foreach (DVBaseColumn col in this.Columns)
            {
                if (col.Check4FormLink())
                {
                    try
                    {
                        this.WebForm = Redis.Get<EbWebForm>(col.LinkRefId);
                        if (this.WebForm == null)
                        {
                            var result = service.Gateway.Send<EbObjectParticularVersionResponse>(new EbObjectParticularVersionRequest { RefId = col.LinkRefId });
                            this.WebForm = EbSerializers.Json_Deserialize(result.Data[0].Json);
                            Redis.Set<EbWebForm>(col.LinkRefId, this.WebForm);
                        }
                        this.FormLinks.Add(new FormLink { DisplayName = this.WebForm.DisplayName, Refid = col.LinkRefId, Params = col.FormParameters });
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("AfterRedisGetBasicInfo " + e.Message);
                    }
                }
            }
        }
    }

    [EnableInBuilder(BuilderType.DVBuilder, BuilderType.BotForm)]
    [BuilderTypeEnum(BuilderType.DVBuilder)]
    public class EbChartVisualization : EbDataVisualization, IEBRootObject
    {
        [EnableInBuilder(BuilderType.DVBuilder)]
        [HideInPropertyGrid]
        public override DVColumnCollection Columns { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder)]
        [HideInPropertyGrid]
        public override bool IsPaging { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder)]
        [DefaultPropValue("0")]
        [HideInPropertyGrid]
        public ChartType Charttype { get; set; }

        [OnDeserialized]
        public void OnDeserializedMethod(StreamingContext context)
        {
            this.BareControlHtml = this.GetBareHtml();
            this.ObjType = this.GetType().Name.Substring(2, this.GetType().Name.Length - 2);
        }

        [HideInPropertyGrid]
        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm)]
        public string ObjType { get; set; }

        [HideInPropertyGrid]
        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm)]
        public string BareControlHtml { get; set; }

        public override string GetBareHtml()
        {
            return "<canvas style='width:100%' eb-type='Table' id='@name@'></canvas>"

    .Replace("@name@", (this.EbSid != null) ? this.EbSid : "@name@");
        }

        [PropertyGroup("Xaxis")]
        [EnableInBuilder(BuilderType.DVBuilder)]
        [PropertyEditor(PropertyEditorType.CollectionFrmSrc, "Columns")]
        public List<DVBaseColumn> Xaxis { get; set; }

        [PropertyGroup("Yaxis")]
        [EnableInBuilder(BuilderType.DVBuilder)]
        [PropertyEditor(PropertyEditorType.CollectionFrmSrc, "Columns")]
        public List<DVBaseColumn> Yaxis { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder)]
        [HideInPropertyGrid]
        [OnChangeExec(@"
        if(this.Type === 'pie'){
            pg.HideProperty('XaxisTitle')
            pg.HideProperty('YaxisTitle')
            pg.HideProperty('XaxisTitleColor')
            pg.HideProperty('YaxisTitleColor')
            pg.HideProperty('XaxisLabelColor')
            pg.HideProperty('YaxisLabelColor')
        }

        else{
            pg.ShowProperty('XaxisTitle')
            pg.ShowProperty('YaxisTitle')
            pg.ShowProperty('XaxisTitleColor')
            pg.ShowProperty('YaxisTitleColor')
            pg.ShowProperty('XaxisLabelColor')
            pg.ShowProperty('YaxisLabelColor')
        }")]
        public string Type { get; set; }

        [PropertyGroup("Xaxis")]
        [EnableInBuilder(BuilderType.DVBuilder)]
        [PropertyEditor(PropertyEditorType.Text)]
        [DefaultPropValue("XLabel")]
        public string XaxisTitle { get; set; }

        [PropertyGroup("Yaxis")]
        [EnableInBuilder(BuilderType.DVBuilder)]
        [PropertyEditor(PropertyEditorType.Text)]
        [DefaultPropValue("YLabel")]
        public string YaxisTitle { get; set; }

        [PropertyGroup("Xaxis")]
        [EnableInBuilder(BuilderType.DVBuilder)]
        [PropertyEditor(PropertyEditorType.Color)]
        [DefaultPropValue("#000000")]
        public string XaxisTitleColor { get; set; }

        [PropertyGroup("Yaxis")]
        [EnableInBuilder(BuilderType.DVBuilder)]
        [PropertyEditor(PropertyEditorType.Color)]
        [DefaultPropValue("#000000")]
        public string YaxisTitleColor { get; set; }

        [PropertyGroup("Xaxis")]
        [EnableInBuilder(BuilderType.DVBuilder)]
        [PropertyEditor(PropertyEditorType.Color)]
        [DefaultPropValue("#000000")]
        public string XaxisLabelColor { get; set; }

        [PropertyGroup("Yaxis")]
        [EnableInBuilder(BuilderType.DVBuilder)]
        [PropertyEditor(PropertyEditorType.Color)]
        [DefaultPropValue("#000000")]
        public string YaxisLabelColor { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder)]
        [HideInPropertyGrid]
        public List<ChartColor> LegendColor { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder)]
        public bool ShowTooltip { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder)]
        public bool ShowValue { get; set; }

        //[EnableInBuilder(BuilderType.DVBuilder)]
        //public Position LegendPosition { get; set; }

        public static EbOperations Operations = CVOperations.Instance;

        public override List<string> DiscoverRelatedRefids()
        {
            List<string> _refids = new List<string>();
            if (!DataSourceRefId.IsEmpty())
            {
                EbDataReader ds = EbDataSource;
                if (ds is null)
                    _refids.Add(DataSourceRefId);
            }
            return _refids;
        }

        public override void ReplaceRefid(Dictionary<string, string> RefidMap)
        {
            if (!DataSourceRefId.IsEmpty())
            {
                if (RefidMap.ContainsKey(DataSourceRefId))
                    DataSourceRefId = RefidMap[DataSourceRefId];
                else
                    DataSourceRefId = "failed-to-update-";
            }
        }

        public EbChartVisualization()
        {
            this.Xaxis = new List<DVBaseColumn>();
            this.Yaxis = new List<DVBaseColumn>();
            this.LegendColor = new List<ChartColor>();
        }
    }

    [EnableInBuilder(BuilderType.DVBuilder)]
    [BuilderTypeEnum(BuilderType.DVBuilder)]
    public class EbGoogleMap : EbDataVisualization, IEBRootObject
    {
        [EnableInBuilder(BuilderType.DVBuilder)]
        [HideInPropertyGrid]
        public override DVColumnCollection Columns { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder)]
        [HideInPropertyGrid]
        public override bool IsPaging { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder)]
        [PropertyEditor(PropertyEditorType.CollectionFrmSrc, "Columns", 1)]
        [Alias("LatLong")]
        [HideForUser]
        public DVBaseColumn LatLong { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder)]
        public bool ShowRoute { get; set; }

        //[EnableInBuilder(BuilderType.DVBuilder)]
        //public bool ShowMarker { get; set; }

        [PropertyGroup("Marker")]
        [EnableInBuilder(BuilderType.DVBuilder)]
        [PropertyEditor(PropertyEditorType.CollectionFrmSrc, "Columns")]
        [HideInPropertyGrid]
        public List<DVBaseColumn> MarkerLabel { get; set; }

        [PropertyGroup("Marker")]
        [EnableInBuilder(BuilderType.DVBuilder)]
        [PropertyEditor(PropertyEditorType.CollectionFrmSrc, "Columns")]
        public List<DVBaseColumn> InfoWindow { get; set; }

        [PropertyGroup("Marker")]
        [EnableInBuilder(BuilderType.DVBuilder, BuilderType.BotForm)]
        [PropertyEditor(PropertyEditorType.ObjectSelector)]
        [OSE_ObjectTypes(EbObjectTypes.iWebForm)]
        [HideForUser]
        [OnChangeExec(@"
            if(this.MarkerLink !== null)
                pg.ShowProperty('FormParameter');
            else 
                pg.HideProperty('FormParameter');
        ")]
        public string MarkerLink { get; set; }

        [PropertyGroup("Marker")]
        [EnableInBuilder(BuilderType.DVBuilder)]
        [PropertyEditor(PropertyEditorType.CollectionFrmSrc, "Columns")]
        [HideForUser]
        public List<DVBaseColumn> FormParameter { get; set; }

        [PropertyGroup("Zoom")]
        [EnableInBuilder(BuilderType.DVBuilder)]
        [DefaultPropValue("true")]
        [OnChangeExec(@"
        if(this.AutoZoom === true)
            pg.HideProperty('Zoomlevel');
        else
            pg.ShowProperty('Zoomlevel');
        ")]
        public bool AutoZoom { get; set; }

        [PropertyGroup("Zoom")]
        [EnableInBuilder(BuilderType.DVBuilder)]
        [DefaultPropValue("10")]
        public int Zoomlevel { get; set; }

        public static EbOperations Operations = MapOperations.Instance;

        public override List<string> DiscoverRelatedRefids()
        {
            List<string> _refids = new List<string>();
            if (!DataSourceRefId.IsEmpty())
            {
                EbDataReader ds = EbDataSource;
                if (ds is null)
                    _refids.Add(DataSourceRefId);
            }
            return _refids;
        }

        public override void ReplaceRefid(Dictionary<string, string> RefidMap)
        {
            if (!DataSourceRefId.IsEmpty())
            {
                if (RefidMap.ContainsKey(DataSourceRefId))
                    DataSourceRefId = RefidMap[DataSourceRefId];
                else
                    DataSourceRefId = "failed-to-update-";
            }
        }

        public EbGoogleMap()
        {
            this.LatLong = new DVBaseColumn();
            this.MarkerLabel = new List<DVBaseColumn>();
            this.InfoWindow = new List<DVBaseColumn>();
            this.FormParameter = new List<DVBaseColumn>();
            this.AutoZoom = true;
        }

    }

    [EnableInBuilder(BuilderType.DVBuilder)]
    [HideInToolBox]
    [HideInPropertyGrid]
    public class RowGroupParent
    {
        [EnableInBuilder(BuilderType.DVBuilder)]
        public string Name { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder)]
        public string DisplayName { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder)]
        [PropertyEditor(PropertyEditorType.CollectionFrmSrc, "Parent.Columns")]
        public List<DVBaseColumn> RowGrouping { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder)]
        [PropertyEditor(PropertyEditorType.CollectionFrmSrc, "Parent.Columns")]
        public List<DVBaseColumn> OrderBy { get; set; }

        public RowGroupParent()
        {
            this.RowGrouping = new List<DVBaseColumn>();
            this.OrderBy = new List<DVBaseColumn>();
        }
    }

    [EnableInBuilder(BuilderType.DVBuilder)]
    [UsedWithTopObjectParent(typeof(EbDataVisualizationObject))]
    [Alias("Multi Level")]
    public class MultipleLevelRowGroup : RowGroupParent
    {

    }

    [EnableInBuilder(BuilderType.DVBuilder)]
    [UsedWithTopObjectParent(typeof(EbDataVisualizationObject))]
    [Alias("Single Level")]
    public class SingleLevelRowGroup : RowGroupParent
    {

    }

    public class FormLink
    {
        public string DisplayName { get; set; }

        public string Refid { get; set; }

        public List<DVBaseColumn> Params { get; set; }
    }

    [EnableInBuilder(BuilderType.DVBuilder, BuilderType.WebForm, BuilderType.Calendar)]
    public class ObjectBasicInfo : EbObject
    {
        [EnableInBuilder(BuilderType.DVBuilder, BuilderType.WebForm, BuilderType.Calendar)]
        [HideInPropertyGrid]
        public override string Name { get; set; }

        [HideInPropertyGrid]
        [EnableInBuilder(BuilderType.DVBuilder, BuilderType.WebForm, BuilderType.Calendar)]
        public string ObjName { get; set; }

        [HideInPropertyGrid]
        [EnableInBuilder(BuilderType.DVBuilder, BuilderType.WebForm, BuilderType.Calendar)]
        public string ObjDisplayName { get; set; }

        [HideInPropertyGrid]
        [EnableInBuilder(BuilderType.DVBuilder, BuilderType.WebForm, BuilderType.Calendar)]
        public string Version { get; set; }

        [HideInPropertyGrid]
        [EnableInBuilder(BuilderType.DVBuilder, BuilderType.WebForm, BuilderType.Calendar)]
        public string ObjRefId { get; set; }
    }

    [EnableInBuilder(BuilderType.WebForm, BuilderType.DVBuilder, BuilderType.Calendar)]
    public class ObjectBasicForm : ObjectBasicInfo
    {
        [EnableInBuilder(BuilderType.DVBuilder, BuilderType.Calendar)]
        [PropertyEditor(PropertyEditorType.DropDown)]
        [OnChangeExec(@"
if(this.FormMode === 1){
    pg.ShowProperty('FormId');
    pg.HideProperty('FormParameters');
}
else if(this.FormMode === 2){
    pg.HideProperty('FormId');
    pg.ShowProperty('FormParameters');
}
else {
    pg.HideProperty('FormId');
    pg.HideProperty('FormParameters');
}")]
        public WebFormDVModes FormMode { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder, BuilderType.Calendar)]
        [PropertyEditor(PropertyEditorType.DropDown)]
        public LinkTypeEnum LinkType { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder,  BuilderType.Calendar)]
        [PropertyEditor(PropertyEditorType.CollectionFrmSrc, "Parent.LinesColumns")]
        public List<DVBaseColumn> FormId { get; set; }


        [EnableInBuilder(BuilderType.DVBuilder, BuilderType.Calendar)]
        [PropertyEditor(PropertyEditorType.Mapper, "Parent.LinesColumns", "Refid", "FormControl")]
        public List<DVBaseColumn> FormParameters { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder, BuilderType.Calendar)]
        [HideInPropertyGrid]
        public EbControl FormControl { get; set; }
    }

    [EnableInBuilder(BuilderType.DVBuilder, BuilderType.Calendar)]
    public class ObjectBasicVis : ObjectBasicInfo
    {

    }

    [EnableInBuilder(BuilderType.WebForm)]
    public class ObjectBasicReport : ObjectBasicInfo
    {
        [EnableInBuilder(BuilderType.WebForm)]
        public string Title { get; set; }
    }

}
