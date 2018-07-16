using ExpressBase.Common;
using ExpressBase.Common.JsonConverters;
using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using ExpressBase.Common.Structures;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Data;
using System.Runtime.Serialization;

namespace ExpressBase.Objects.Objects.DVRelated
{
    public enum StringRenderType
    {
        Default = 0,
        Chart = 1,
        Link = 2,
        Marker = 3,
        Image = 4,
        Icon = 5
    }

    public enum NumericRenderType
    {
        Default,
        ProgressBar,
        Link
    }

    public enum BooleanRenderType
    {
        Default,
        Icon,
        IsEditable
    }

    public enum DateTimeRenderType
    {
        Default,
        Link
    }

    public enum FontFamily
    {
        Arial,
        Helvetica,
        Times_New_Roman,
        Courier_New,
        Comic_Sans_MS,
        Impact
    }

    public enum DateFormat
    {
        Date,
        Time,
        TimeWithoutTT,
        DateTime,
        DateTimeWithoutTT,
        DateTimeWithoutSeconds,
        DateTimeWithoutSecondsAndTT,
    }

    [EnableInBuilder(BuilderType.DVBuilder, BuilderType.BotForm)]
    public class DVBaseColumn : EbDataVisualizationObject
    {
        [JsonProperty(PropertyName = "data")]
        public int Data { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder, BuilderType.BotForm)]
        [JsonProperty(PropertyName = "name")]
        [Alias("Name")]
        [PropertyEditor(PropertyEditorType.Label)]
        public override string Name { get; set; }

        public EbDbTypes Type { get; set; }

        public string sType { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder, BuilderType.BotForm)]
        [Alias("Title")]
        public string sTitle { get; set; }

		[HideInPropertyGrid]
        [EnableInBuilder(BuilderType.DVBuilder, BuilderType.BotForm)]
        public bool bVisible { get; set; }

        public int Pos { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder, BuilderType.BotForm)]
        [Alias("Width")]
        [JsonIgnore]
        public string sWidth { get; set; }

        //[EnableInBuilder(BuilderType.DVBuilder)]
        [JsonProperty(PropertyName = "className")]
        public string ClassName { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder, BuilderType.BotForm)]
        [PropertyEditor(PropertyEditorType.FontSelector)]
        public EbFont Font { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder, BuilderType.BotForm)]
        [PropertyEditor(PropertyEditorType.DropDown)]
        public ControlType FilterControl { get; set; }

        [PropertyEditor(PropertyEditorType.ScriptEditorCS)]
        [JsonConverter(typeof(Base64Converter))]
        public string Formula { get; set; }
    }

    public class DVColumnCollection : List<DVBaseColumn>
    {
        public DVBaseColumn Get(string name)
        {
            foreach (DVBaseColumn col in this)
            {
                if (col.Name.Equals(name))
                    return col;
            }

            return null;
        }
    }

    [EnableInBuilder(BuilderType.DVBuilder, BuilderType.BotForm, BuilderType.FilterDialog)]
    public class DVStringColumn : DVBaseColumn
    {
        [EnableInBuilder(BuilderType.DVBuilder, BuilderType.BotForm, BuilderType.FilterDialog)]
        [DefaultPropValue("1")]
        [PropertyEditor(PropertyEditorType.DropDown)]
        [OnChangeExec(@"
if(this.RenderAs !== 2)
    pg.HideProperty('LinkRefId')
else
    pg.ShowProperty('LinkRefId')")]
        public StringRenderType RenderAs { get; set; }


        [EnableInBuilder(BuilderType.DVBuilder, BuilderType.BotForm, BuilderType.FilterDialog)]
        [PropertyEditor(PropertyEditorType.ObjectSelector)]
        [OSE_ObjectTypes(EbObjectTypes.iTableVisualization, EbObjectTypes.iChartVisualization, EbObjectTypes.iReport)]
        public string LinkRefId { get; set; }
    }

    [EnableInBuilder(BuilderType.DVBuilder, BuilderType.BotForm, BuilderType.FilterDialog)]
    public class DVNumericColumn : DVBaseColumn
    {
        [EnableInBuilder(BuilderType.DVBuilder, BuilderType.BotForm, BuilderType.FilterDialog)]
        public bool Aggregate { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder, BuilderType.BotForm, BuilderType.FilterDialog)]
        public int DecimalPlaces { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder, BuilderType.BotForm, BuilderType.FilterDialog)]
        [OnChangeExec(@"
if(this.RenderAs !== 2)
    pg.HideProperty('LinkRefId')
else
    pg.ShowProperty('LinkRefId')")]
        public NumericRenderType RenderAs { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder, BuilderType.BotForm, BuilderType.FilterDialog)]
        [PropertyEditor(PropertyEditorType.ObjectSelector)]
        [OSE_ObjectTypes(EbObjectTypes.iTableVisualization, EbObjectTypes.iChartVisualization, EbObjectTypes.iReport)]
        public string LinkRefId { get; set; }
    }

    [EnableInBuilder(BuilderType.DVBuilder, BuilderType.BotForm, BuilderType.FilterDialog)]
    public class DVBooleanColumn : DVBaseColumn
    {
        [EnableInBuilder(BuilderType.DVBuilder, BuilderType.BotForm, BuilderType.FilterDialog)]
        public BooleanRenderType RenderAs { get; set; }
    }

    [EnableInBuilder(BuilderType.DVBuilder, BuilderType.BotForm, BuilderType.FilterDialog)]
    public class DVDateTimeColumn : DVBaseColumn
    {
        [EnableInBuilder(BuilderType.DVBuilder, BuilderType.BotForm, BuilderType.FilterDialog)]
        public DateFormat Format { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder, BuilderType.BotForm, BuilderType.FilterDialog)]
        [OnChangeExec(@"
if(this.RenderAs !== 1)
    pg.HideProperty('LinkRefId')
else
    pg.ShowProperty('LinkRefId')")]
        public DateTimeRenderType RenderAs { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder, BuilderType.BotForm, BuilderType.FilterDialog)]
        [PropertyEditor(PropertyEditorType.ObjectSelector)]
        [OSE_ObjectTypes(EbObjectTypes.iTableVisualization, EbObjectTypes.iChartVisualization, EbObjectTypes.iReport)]
        public string LinkRefId { get; set; }
    }
}
