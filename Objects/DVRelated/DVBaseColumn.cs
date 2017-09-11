using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Data;

namespace ExpressBase.Objects.Objects.DVRelated
{
    public enum StringRenderType
    {
        Default,
        Chart,
        Link
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
        Image
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

    [EnableInBuilder(BuilderType.DVBuilder)]
    public class DVBaseColumn : EbObject
    {
        [JsonProperty(PropertyName = "data")]
        public int Data { get; set; }
        
        [EnableInBuilder(BuilderType.DVBuilder)]
        [JsonProperty(PropertyName = "name")]
        [Alias("Name")]
        [PropertyEditor(PropertyEditorType.Label)]
        public override string Name { get; set; }

        public DbType Type { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder)]
        [Alias("Title")]
        public string sTitle { get; set; }
        
        public bool bVisible { get; set; }

        public int Pos { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder)]
        [Alias("Width")]
        public string sWidth { get; set; }

        //[EnableInBuilder(BuilderType.DVBuilder)]
        [JsonProperty(PropertyName = "className")]
        public string ClassName { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder)]
        [Alias("FontFamily")]
        public FontFamily fontfamily { get; set; }
    }

    public class DVColumnCollection : List<DVBaseColumn>
    {

    }

    [EnableInBuilder(BuilderType.DVBuilder)]
    public class DVStringColumn : DVBaseColumn
    {
        [EnableInBuilder(BuilderType.DVBuilder)]
        [OnChangeExec(@"
if(this.RenderAs !== 'Link')
    pg.HideProperty('LinkRefId')
else
    pg.ShowProperty('LinkRefId')")]
        public StringRenderType RenderAs { get; set; }

        
        [EnableInBuilder(BuilderType.DVBuilder)]
        public string LinkRefId { get; set; }
    }

    [EnableInBuilder(BuilderType.DVBuilder)]
    public class DVNumericColumn : DVBaseColumn
    {
        [EnableInBuilder(BuilderType.DVBuilder)]
        public bool Aggregate { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder)]
        public int DecimalPlaces { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder)]
        [OnChangeExec(@"
if(this.RenderAs !== 'Link')
    pg.HideProperty('LinkRefId')
else
    pg.ShowProperty('LinkRefId')")]
        public NumericRenderType RenderAs { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder)]
        public string LinkRefId { get; set; }
    }

    [EnableInBuilder(BuilderType.DVBuilder)]
    public class DVBooleanColumn : DVBaseColumn
    {
        [EnableInBuilder(BuilderType.DVBuilder)]
        public bool IsEditable { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder)]
        public BooleanRenderType RenderAs { get; set; }
    }

    [EnableInBuilder(BuilderType.DVBuilder)]
    public class DVDateTimeColumn : DVBaseColumn
    {
        [EnableInBuilder(BuilderType.DVBuilder)]
        public DateFormat Format { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder)]
        [OnChangeExec(@"
if(this.RenderAs !== 'Link')
    pg.HideProperty('LinkRefId')
else
    pg.ShowProperty('LinkRefId')")]
        public DateTimeRenderType RenderAs { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder)]
        public string LinkRefId { get; set; }
    }
}
