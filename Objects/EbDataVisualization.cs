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

    [EnableInBuilder(BuilderType.BotForm)]
    public class EbDataVisualizationObject : EbObject
    {
        [EnableInBuilder(BuilderType.DVBuilder)]
        public override string Name { get => base.Name; set => base.Name = value; }
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
        [PropertyEditor(PropertyEditorType.ObjectSelector)]
        [OSE_ObjectTypes(EbObjectTypes.iDataSource)]
        public string DataSourceRefId { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder)]
        public string Description { get; set; }

        public string EbSid { get; set; }
        [JsonIgnore]
        public EbDataSource EbDataSource { get; set; }

        [PropertyEditor(PropertyEditorType.CollectionProp, "Columns", "bVisible")]
        //[PropertyEditor(PropertyEditorType.CollectionFrmSrcPG, "DSColumns")]
        [EnableInBuilder(BuilderType.DVBuilder)]
        public DVColumnCollection Columns { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder)]
        [HideInPropertyGrid]
        public DVColumnCollection DSColumns { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder)]
        [HideInPropertyGrid]
        [JsonIgnore]
        public object data { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder)]
        [HideInPropertyGrid]
        public string Pippedfrom { get; set; }

        [HideInPropertyGrid]
        [EnableInBuilder(BuilderType.DVBuilder)]
        public string IsPaged { get; set; }
        
        [EnableInBuilder(BuilderType.DVBuilder)]
        public bool IsPaging { get; set; }

        public override void AfterRedisGet(RedisClient Redis)
        {
            try
            {
                this.EbDataSource = Redis.Get<EbDataSource>(this.DataSourceRefId);
                this.EbDataSource.AfterRedisGet(Redis);
            }
            catch (Exception e)
            {

            }
        }

        public override void AfterRedisGet(RedisClient Redis, IServiceClient client)
        {
            try
            {
                this.EbDataSource = Redis.Get<EbDataSource>(this.DataSourceRefId);
                if (this.EbDataSource == null || this.EbDataSource.Sql == null || this.EbDataSource.Sql == string.Empty)
                {
                    var result = client.Get<EbObjectParticularVersionResponse>(new EbObjectParticularVersionRequest { RefId = this.DataSourceRefId });
                    this.EbDataSource = EbSerializers.Json_Deserialize(result.Data[0].Json);
                    Redis.Set<EbDataSource>(this.DataSourceRefId, this.EbDataSource);
                }
                if (this.EbDataSource.FilterDialogRefId != string.Empty)
                    this.EbDataSource.AfterRedisGet(Redis, client);
            }
            catch (Exception e)
            {

            }
        }

        public EbDataSet DoQueries4DataVis(string sql, IEbConnectionFactory df, params DbParameter[] parameters)
        {
            EbDataSet ds = new EbDataSet();

            using (var con = df.DataDBRO.GetNewConnection())
            {
                try
                {
                    con.Open();
                    using (var cmd = df.DataDBRO.GetNewCommand(con, sql))
                    {
                        if (parameters != null && parameters.Length > 0)
                            cmd.Parameters.AddRange(parameters);

                        using (var reader = cmd.ExecuteReader())
                        {
                            do
                            {
                                EbDataTable dt = new EbDataTable();
                                this.AddColumns(dt, reader as NpgsqlDataReader);
                                PrepareDataTable4DataVis(reader, dt);
                                ds.Tables.Add(dt);
                            }
                            while (reader.NextResult());
                        }
                    }
                }
                catch (Npgsql.NpgsqlException npgse) { }
            }

            return ds;
        }

        public void PrepareDataTable4DataVis(DbDataReader reader, EbDataTable dt)
        {
            int _fieldCount = reader.FieldCount;
            while (reader.Read())
            {
                EbDataRow dr = dt.NewDataRow();
                for (int i = 0; i < _fieldCount; i++)
                {
                    var _typ = reader.GetFieldType(i);
                    var _coln = dt.Columns[i].ColumnName;
                    if (_typ == typeof(DateTime))
                    {
                        var _val = reader.IsDBNull(i) ? DateTime.Now : reader.GetDateTime(i);
                        var _dvCol = this.Columns.Get(_coln) as DVDateTimeColumn;
                        if (_dvCol.Format == DateFormat.Date)
                        {
                            dr[i] = _val.ToString("dd-MM-yyyy");
                            continue;
                        }
                        else if (_dvCol.Format == DateFormat.Time)
                        {
                            dr[i] = _val.ToString("HH:mm:ss tt");
                            continue;
                        }
                        else if (_dvCol.Format == DateFormat.TimeWithoutTT)
                        {
                            dr[i] = _val.ToString("HH:mm:ss");
                            continue;
                        }
                    }
                    else if (_typ == typeof(string))
                    {
                        dr[i] = reader.IsDBNull(i) ? string.Empty : reader.GetString(i);
                        continue;
                    }
                    else if (_typ == typeof(bool))
                    {
                        dr[i] = reader.IsDBNull(i) ? false : reader.GetBoolean(i);
                        continue;
                    }
                    else if (_typ == typeof(decimal))
                    {
                        dr[i] = reader.IsDBNull(i) ? 0 : reader.GetDecimal(i);
                        continue;
                    }
                    else if (_typ == typeof(int) || _typ == typeof(Int32))
                    {
                        dr[i] = reader.IsDBNull(i) ? 0 : reader.GetInt32(i);
                        continue;
                    }
                    else if (_typ == typeof(Int64))
                    {
                        dr[i] = reader.IsDBNull(i) ? 0 : reader.GetInt64(i);
                        continue;
                    }
                    else
                    {
                        dr[i] = reader.GetValue(i);
                        continue;
                    }
                }

                dt.Rows.Add(dr);
            }
        }

        private void AddColumns(EbDataTable dt, NpgsqlDataReader reader)
        {
            int pos = 0;
            foreach (NpgsqlDbColumn drow in reader.GetColumnSchema())
            {
                string columnName = System.Convert.ToString(drow["ColumnName"]);
                EbDataColumn column = new EbDataColumn(columnName, ConvertToDbType((Type)(drow["DataType"])));
                column.ColumnIndex = pos++;
                dt.Columns.Add(column);
            }
        }

        private EbDbTypes ConvertToDbType(Type _typ)
        {
            if (_typ == typeof(DateTime))
                return EbDbTypes.DateTime;
            else if (_typ == typeof(string))
                return EbDbTypes.String;
            else if (_typ == typeof(bool))
                return EbDbTypes.Boolean;
            else if (_typ == typeof(decimal))
                return EbDbTypes.Decimal;
            else if (_typ == typeof(int) || _typ == typeof(Int32))
                return EbDbTypes.Int32;
            else if (_typ == typeof(Int64))
                return EbDbTypes.Int64;

            return EbDbTypes.String;
        }


    }

    [EnableInBuilder(BuilderType.DVBuilder, BuilderType.BotForm)]
    public class EbTableVisualization : EbDataVisualization
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

        [EnableInBuilder(BuilderType.DVBuilder)]
        [PropertyEditor(PropertyEditorType.CollectionFrmSrc, "Columns")]
        public List<DVBaseColumn> rowGrouping { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder)]
        public int LeftFixedColumn { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder)]
        public int RightFixedColumn { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder)]
        //[DefaultPropValue("'10'")]
        public int PageLength { get; set; }

		public static EbOperations Operations = TVOperations.Instance;

        public override string GetDesignHtml()
        {
            return @"
            <div id='cont_@name@' Ctype='TableVisualization' class='Eb-ctrlContainer'>
                @GetBareHtml@
            </div>"
 .Replace("@name@", (this.Name != null) ? this.Name : "@name@")
 .Replace("@GetBareHtml@", this.GetBareHtml())
 .RemoveCR().DoubleQuoted();
        }

        public override string GetBareHtml()
        {
            return "<table style='width:100%' class='table table-striped' eb-type='Table' id='@name@'></table>"

 .Replace("@name@", (this.EbSid != null) ? this.EbSid : "@name@");
        }

        public EbTableVisualization()
        {
            this.rowGrouping = new List<DVBaseColumn>();
        }
    }

    [EnableInBuilder(BuilderType.DVBuilder, BuilderType.BotForm)]
    public class EbChartVisualization : EbDataVisualization
    {

        [EnableInBuilder(BuilderType.DVBuilder)]
        [PropertyEditor(PropertyEditorType.DropDown)]
        [DefaultPropValue("0")]
        [OnChangeExec(@"
        if(this.Charttype === 1){
            pg.HideProperty('XaxisTitle')
            pg.HideProperty('YaxisTitle')
            pg.HideProperty('XaxisTitleColor')
            pg.HideProperty('YaxisTitleColor')
            pg.HideProperty('XaxisLabelColor')
            pg.HideProperty('YaxisLabelColor')
            pg.HideProperty('LegendColor')
            pg.HideProperty('ShowTooltip')
            pg.HideProperty('ShowValue')
        }

        else{
            pg.ShowProperty('XaxisTitle')
            pg.ShowProperty('YaxisTitle')
            pg.ShowProperty('XaxisTitleColor')
            pg.ShowProperty('YaxisTitleColor')
            pg.ShowProperty('XaxisLabelColor')
            pg.ShowProperty('YaxisLabelColor')
            pg.ShowProperty('LegendColor')
            pg.ShowProperty('ShowTooltip')
            pg.ShowProperty('ShowValue')
        }")]
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

        [EnableInBuilder(BuilderType.DVBuilder)]
        [PropertyEditor(PropertyEditorType.CollectionFrmSrc, "Columns")]
        public List<DVBaseColumn> Xaxis { get; set; }


        [EnableInBuilder(BuilderType.DVBuilder)]
        [PropertyEditor(PropertyEditorType.CollectionFrmSrc, "Columns")]
        public List<DVBaseColumn> Yaxis { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder)]        
        [HideInPropertyGrid]
        public string Type { get; set; }
        
        [EnableInBuilder(BuilderType.DVBuilder)]
        [PropertyEditor(PropertyEditorType.Text)]
        [DefaultPropValue("XLabel")]
        public string XaxisTitle { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder)]
        [PropertyEditor(PropertyEditorType.Text)]
        [DefaultPropValue("YLabel")]
        public string YaxisTitle { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder)]
        [PropertyEditor(PropertyEditorType.Color)]
        [DefaultPropValue("#000000")]
        public string XaxisTitleColor { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder)]
        [PropertyEditor(PropertyEditorType.Color)]
        [DefaultPropValue("#000000")]
        public string YaxisTitleColor { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder)]
        [PropertyEditor(PropertyEditorType.Color)]
        [DefaultPropValue("#000000")]
        public string XaxisLabelColor { get; set; }

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

		public static EbOperations Operations = CVOperations.Instance;
    }

    [EnableInBuilder(BuilderType.DVBuilder)]
    public class EbGoogleMap : EbChartVisualization
    {
        [EnableInBuilder(BuilderType.DVBuilder)]
        [PropertyEditor(PropertyEditorType.DropDown)]
        [DefaultPropValue("1")]
        [OnChangeExec(@"
        if(this.Charttype === 1){
            pg.HideProperty('XaxisTitle')
            pg.HideProperty('YaxisTitle')
            pg.HideProperty('XaxisTitleColor')
            pg.HideProperty('YaxisTitleColor')
            pg.HideProperty('XaxisLabelColor')
            pg.HideProperty('YaxisLabelColor')
            pg.HideProperty('LegendColor')
            pg.HideProperty('ShowTooltip')
            pg.HideProperty('ShowValue')
        }

        else{
            pg.ShowProperty('XaxisTitle')
            pg.ShowProperty('YaxisTitle')
            pg.ShowProperty('XaxisTitleColor')
            pg.ShowProperty('YaxisTitleColor')
            pg.ShowProperty('XaxisLabelColor')
            pg.ShowProperty('YaxisLabelColor')
            pg.ShowProperty('LegendColor')
            pg.ShowProperty('ShowTooltip')
            pg.ShowProperty('ShowValue')
        }")]
        public ChartType Charttype { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder)]
        [PropertyEditor(PropertyEditorType.CollectionFrmSrc, "Columns")]
        [Alias("Longitude")]
        public List<DVBaseColumn> Xaxis { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder)]
        [PropertyEditor(PropertyEditorType.CollectionFrmSrc, "Columns")]
        [Alias("Lattitude")]
        public List<DVBaseColumn> Yaxis { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder)]
        public bool ShowRoute { get; set; }

        //[EnableInBuilder(BuilderType.DVBuilder)]
        //public bool ShowMarker { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder)]
        [PropertyEditor(PropertyEditorType.CollectionFrmSrc, "Columns")]
        public List<DVBaseColumn> MarkerLabel { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder)]
        [PropertyEditor(PropertyEditorType.CollectionFrmSrc, "Columns")]
        public List<DVBaseColumn> InfoWindow { get; set; }

    }

    public class axis
    {
        public string index { get; set; }

        public string name { get; set; }
    }


}
