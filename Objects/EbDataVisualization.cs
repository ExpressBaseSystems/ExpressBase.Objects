using ExpressBase.Common;
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

    [EnableInBuilder(BuilderType.BotForm)]
    public class EbDataVisualizationObject : EbObject
    {

    }

    [EnableInBuilder(BuilderType.DVBuilder)]
    public class ChartColor : EbDataVisualizationObject
    {
        public string name { get; set; }

        public string color { get; set; }
    }

    [EnableInBuilder(BuilderType.DVBuilder)]
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
        [EnableInBuilder(BuilderType.DVBuilder)]
        [HideInPropertyGrid]
        public override string RefId { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder)]
        public override string DisplayName { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder)]
        public override string Description { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder)]
        [HideInPropertyGrid]
        public override string VersionNumber { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder)]
        [HideInPropertyGrid]
        public override string Status { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder)]
        [PropertyEditor(PropertyEditorType.ObjectSelector)]
        [OSE_ObjectTypes(EbObjectTypes.iDataReader)]
        [HideForUser]
        [PropertyPriority(0)]
        public string DataSourceRefId { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder)]
        [HideInPropertyGrid]
        public string EbSid { get; set; }

        [JsonIgnore]
        public EbDataReader EbDataSource { get; set; }

        //[PropertyEditor(PropertyEditorType.CollectionProp, "Columns", "bVisible")]
        [PropertyEditor(PropertyEditorType.CollectionABCpropToggle, "Columns", "bVisible", "_Formula")]
        //[PropertyEditor(PropertyEditorType.CollectionABCpropToggle, "Columns", "Formula")]

        [CEOnSelectFn(@";
            this.bVisible = true;
            NonVC = Parent.NotVisibleColumns.$values;
            let  idx = NonVC.findIndex(x => x.name === this.name);
            if(idx > -1)
                NonVC.splice(idx, 1);")]
        [CEOnDeselectFn(@"
            this.bVisible = false;
            Parent.NotVisibleColumns.$values.push(this)")]
        [EnableInBuilder(BuilderType.DVBuilder)]
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

        [EnableInBuilder(BuilderType.DVBuilder)]
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

        //public EbDataSet DoQueries4DataVis(string sql, IEbConnectionFactory df, params DbParameter[] parameters)
        //{
        //    EbDataSet ds = new EbDataSet();

        //    using (var con = df.DataDBRO.GetNewConnection())
        //    {
        //        try
        //        {
        //            con.Open();
        //            using (var cmd = df.DataDBRO.GetNewCommand(con, sql))
        //            {
        //                if (parameters != null && parameters.Length > 0)
        //                    cmd.Parameters.AddRange(parameters);

        //                using (var reader = cmd.ExecuteReader())
        //                {
        //                    do
        //                    {
        //                        EbDataTable dt = new EbDataTable();
        //                        this.AddColumns(dt, reader as NpgsqlDataReader);
        //                        PrepareDataTable4DataVis(reader, dt);
        //                        ds.Tables.Add(dt);
        //                    }
        //                    while (reader.NextResult());
        //                }
        //            }
        //        }
        //        catch (Npgsql.NpgsqlException npgse)
        //        {
        //            Console.WriteLine("Exception: " + npgse.ToString());
        //        }
        //    }

        //    return ds;
        //}

        //public void PrepareDataTable4DataVis(DbDataReader reader, EbDataTable dt)
        //{
        //    int _fieldCount = reader.FieldCount;
        //    while (reader.Read())
        //    {
        //        EbDataRow dr = dt.NewDataRow();
        //        for (int i = 0; i < _fieldCount; i++)
        //        {
        //            var _typ = reader.GetFieldType(i);
        //            var _coln = dt.Columns[i].ColumnName;
        //            if (_typ == typeof(DateTime))
        //            {
        //                var _val = reader.IsDBNull(i) ? DateTime.Now : reader.GetDateTime(i);
        //                var _dvCol = this.Columns.Get(_coln) as DVDateTimeColumn;
        //                if (_dvCol.Format == DateFormat.Date)
        //                {
        //                    dr[i] = _val.ToString("dd-MM-yyyy");
        //                    continue;
        //                }
        //                else if (_dvCol.Format == DateFormat.Time)
        //                {
        //                    dr[i] = _val.ToString("HH:mm:ss tt");
        //                    continue;
        //                }
        //                else if (_dvCol.Format == DateFormat.TimeWithoutTT)
        //                {
        //                    dr[i] = _val.ToString("HH:mm:ss");
        //                    continue;
        //                }
        //            }
        //            else if (_typ == typeof(string))
        //            {
        //                dr[i] = reader.IsDBNull(i) ? string.Empty : reader.GetString(i);
        //                continue;
        //            }
        //            else if (_typ == typeof(bool))
        //            {
        //                dr[i] = reader.IsDBNull(i) ? false : reader.GetBoolean(i);
        //                continue;
        //            }
        //            else if (_typ == typeof(decimal))
        //            {
        //                dr[i] = reader.IsDBNull(i) ? 0 : reader.GetDecimal(i);
        //                continue;
        //            }
        //            else if (_typ == typeof(int) || _typ == typeof(Int32))
        //            {
        //                dr[i] = reader.IsDBNull(i) ? 0 : reader.GetInt32(i);
        //                continue;
        //            }
        //            else if (_typ == typeof(Int64))
        //            {
        //                dr[i] = reader.IsDBNull(i) ? 0 : reader.GetInt64(i);
        //                continue;
        //            }
        //            else
        //            {
        //                dr[i] = reader.GetValue(i);
        //                continue;
        //            }
        //        }

        //        dt.Rows.Add(dr);
        //    }
        //}

        //private void AddColumns(EbDataTable dt, NpgsqlDataReader reader)
        //{
        //    int pos = 0;
        //    foreach (NpgsqlDbColumn drow in reader.GetColumnSchema())
        //    {
        //        string columnName = System.Convert.ToString(drow["ColumnName"]);
        //        EbDataColumn column = new EbDataColumn(columnName, ConvertToDbType((Type)(drow["DataType"])));
        //        column.ColumnIndex = pos++;
        //        dt.Columns.Add(column);
        //    }
        //}

        //private EbDbTypes ConvertToDbType(Type _typ)
        //{
        //    if (_typ == typeof(DateTime))
        //        return EbDbTypes.DateTime;
        //    else if (_typ == typeof(string))
        //        return EbDbTypes.String;
        //    else if (_typ == typeof(bool))
        //        return EbDbTypes.Boolean;
        //    else if (_typ == typeof(decimal))
        //        return EbDbTypes.Decimal;
        //    else if (_typ == typeof(int) || _typ == typeof(Int32))
        //        return EbDbTypes.Int32;
        //    else if (_typ == typeof(Int64))
        //        return EbDbTypes.Int64;

        //    return EbDbTypes.String;
        //}


    }

    [EnableInBuilder(BuilderType.DVBuilder, BuilderType.BotForm)]
    [BuilderTypeEnum(BuilderType.DVBuilder)]
    public class EbTableVisualization : EbDataVisualization,IEBRootObject
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
        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm)]
        public string ObjType { get; set; }

        [HideInPropertyGrid]
        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm)]
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
        [EnableInBuilder(BuilderType.DVBuilder)]
        [PropertyEditor(PropertyEditorType.Collection)]
        [HideForUser]
        [HideInPropertyGrid]
        public List<RowGroupParent> RowGroupCollection { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder)]
        [HideInPropertyGrid]
        public RowGroupParent CurrentRowGroup { get; set; }

        [PropertyGroup("Fixed Column")]
        [EnableInBuilder(BuilderType.DVBuilder)]
        public int LeftFixedColumn { get; set; }

        [PropertyGroup("Fixed Column")]
        [EnableInBuilder(BuilderType.DVBuilder)]
        public int RightFixedColumn { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder)]
        [DefaultPropValue("100")]
        [PropertyGroup("Paging")]
        public int PageLength { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder)]
        public bool DisableRowGrouping { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder)]
        [HideForUser]
        public string SecondaryTableMapField { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder)]
        [HideForUser]
        public bool DisableCopy { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder)]
        [HideForUser]
        public bool AllowMultilineHeader { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder)]
        [PropertyEditor(PropertyEditorType.CollectionFrmSrc, "Columns")]
        public List<DVBaseColumn> OrderBy { get; set; }

        public static EbOperations Operations = TVOperations.Instance;

 //       public override string GetDesignHtml()
 //       {
 //           return @"
 //           <div id='cont_@name@' Ctype='TableVisualization' class='Eb-ctrlContainer'>
 //               @GetBareHtml@
 //           </div>"
 //.Replace("@name@", (this.Name != null) ? this.Name : "@name@")
 //.Replace("@GetBareHtml@", this.GetBareHtml())
 //.RemoveCR().DoubleQuoted();
 //       }

 //       public override string GetBareHtml()
 //       {
 //           return "<table style='width:100%' class='table table-striped' eb-type='Table' id='@name@tbl'></table>"

 //.Replace("@name@", (this.EbSid != null) ? this.Name : "@name@");
 //       }

        public EbTableVisualization()
        {
            this.RowGroupCollection = new List<RowGroupParent>();
			this.NotVisibleColumns = new List<DVBaseColumn>();
            this.CurrentRowGroup = new RowGroupParent();
            this.OrderBy = new List<DVBaseColumn>();
            //this.ParentColumn = new List<DVBaseColumn>();
            //this.GroupingColumn = new List<DVBaseColumn>();
            this.ColumnsCollection = new List<DVColumnCollection>();
            this.ParamsList = new List<Param>();
        }

        public override string DiscoverRelatedRefids()
        {
            string refids = "";
            if (!DataSourceRefId.IsEmpty())
            {
                EbDataReader ds = EbDataSource;
                if (ds is null)
                {
                    refids += DataSourceRefId + ",";
                }
            }
            foreach (DVBaseColumn _col in Columns)
            {
                if (!_col.LinkRefId.IsNullOrEmpty())
                {
                    refids += _col.LinkRefId + ",";
                }
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
                    else
                        _col.LinkRefId = "failed-to-update-";
                }
            }
        }
    }

    [EnableInBuilder(BuilderType.DVBuilder, BuilderType.BotForm)]
    [BuilderTypeEnum(BuilderType.DVBuilder)]
    public class EbChartVisualization : EbDataVisualization, IEBRootObject
    {

        [EnableInBuilder(BuilderType.DVBuilder)]
        [PropertyEditor(PropertyEditorType.DropDown)]
        [DefaultPropValue("0")]        
        [HideForUser]
        public virtual ChartType Charttype { get; set; }

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

        [EnableInBuilder(BuilderType.DVBuilder)]
        [PropertyEditor(PropertyEditorType.CollectionFrmSrc, "Columns")]
        public virtual List<DVBaseColumn> Xaxis { get; set; }


        [EnableInBuilder(BuilderType.DVBuilder)]
        [PropertyEditor(PropertyEditorType.CollectionFrmSrc, "Columns")]
        public virtual List<DVBaseColumn> Yaxis { get; set; }

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

        [EnableInBuilder(BuilderType.DVBuilder)]
        [PropertyEditor(PropertyEditorType.Text)]
        [DefaultPropValue("XLabel")]
        public virtual string XaxisTitle { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder)]
        [PropertyEditor(PropertyEditorType.Text)]
        [DefaultPropValue("YLabel")]
        public virtual string YaxisTitle { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder)]
        [PropertyEditor(PropertyEditorType.Color)]
        [DefaultPropValue("#000000")]
        public virtual string XaxisTitleColor { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder)]
        [PropertyEditor(PropertyEditorType.Color)]
        [DefaultPropValue("#000000")]
        public virtual string YaxisTitleColor { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder)]
        [PropertyEditor(PropertyEditorType.Color)]
        [DefaultPropValue("#000000")]
        public virtual string XaxisLabelColor { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder)]
        [PropertyEditor(PropertyEditorType.Color)]
        [DefaultPropValue("#000000")]
        public virtual string YaxisLabelColor { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder)]
        [HideInPropertyGrid]
        public virtual List<ChartColor> LegendColor { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder)]
        public virtual  bool ShowTooltip { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder)]
        public virtual bool ShowValue { get; set; }

        //[EnableInBuilder(BuilderType.DVBuilder)]
        //public Position LegendPosition { get; set; }

        public static EbOperations Operations = CVOperations.Instance;

        public override string DiscoverRelatedRefids()
        {
            if (!DataSourceRefId.IsEmpty())
            {
                EbDataReader ds = EbDataSource;
                if (ds is null)
                    return DataSourceRefId;
            }
            return "";
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
    public class EbGoogleMap : EbChartVisualization
    {
        [EnableInBuilder(BuilderType.DVBuilder)]
        [HideInPropertyGrid]
        public override DVColumnCollection Columns { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder)]
        [HideInPropertyGrid]
        public override bool IsPaging { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder)]
        [PropertyEditor(PropertyEditorType.DropDown)]        
        [HideForUser]
        public override ChartType Charttype { get { return ChartType.GoogleMap; } }

        [EnableInBuilder(BuilderType.DVBuilder)]
        [PropertyEditor(PropertyEditorType.CollectionFrmSrc, "Columns")]
        [Alias("Longitude")]
        [HideForUser]
        public override List<DVBaseColumn> Xaxis { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder)]
        [PropertyEditor(PropertyEditorType.CollectionFrmSrc, "Columns")]
        [Alias("Lattitude")]
        [HideForUser]
        public override List<DVBaseColumn> Yaxis { get; set; }

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
        [DefaultPropValue("True")]
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

        public EbGoogleMap()
        {
            this.Xaxis = new List<DVBaseColumn>();
            this.Yaxis = new List<DVBaseColumn>();
            this.MarkerLabel = new List<DVBaseColumn>();
            this.InfoWindow = new List<DVBaseColumn>();
            this.FormParameter = new List<DVBaseColumn>();
            this.AutoZoom = true;
        }


        public override string XaxisTitle { get; set; }

        
        public override string YaxisTitle { get; set; }

        
        public override string XaxisTitleColor { get; set; }

        
        public override string YaxisTitleColor { get; set; }

        
        public override string XaxisLabelColor { get; set; }

        
        public override string YaxisLabelColor { get; set; }

        
        public override List<ChartColor> LegendColor { get; set; }

        
        public override bool ShowTooltip { get; set; }

        
        public override bool ShowValue { get; set; }

    }

    public class axis
    {
        public string index { get; set; }

        public string name { get; set; }
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
}
