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

    [EnableInBuilder(BuilderType.DVBuilder)]
    public class DVBaseColumn : EbObject
    {
        [JsonProperty(PropertyName = "data")]
        public int Data { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder)]
        [JsonProperty(PropertyName = "name")]
        new public string Name { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder)]
        public DbType Type { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder)]
        [JsonProperty(PropertyName = "title")]
        public string Title { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder)]
        [JsonProperty(PropertyName = "visible")]
        public bool Visible { get; set; }

        public int Pos { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder)]
        [JsonProperty(PropertyName = "width")]
        public int Width { get; set; }
    }

    public class DVColumnCollection : List<DVBaseColumn>
    {

    }

    [EnableInBuilder(BuilderType.DVBuilder)]
    public class DVStringColumn : DVBaseColumn
    {
        public StringRenderType RenderAs { get; set; }

        public string LinkRefId { get; set; }
    }

    [EnableInBuilder(BuilderType.DVBuilder)]
    public class DVNumericColumn : DVBaseColumn
    {
        public bool Aggregate { get; set; }

        public int DecimalPlaces { get; set; }

        public NumericRenderType RenderAs { get; set; }

        public string LinkRefId { get; set; }
    }

    [EnableInBuilder(BuilderType.DVBuilder)]
    public class DVBooleanColumn : DVBaseColumn
    {
        public bool IsEditable { get; set; }

        public BooleanRenderType RenderAs { get; set; }
    }

    [EnableInBuilder(BuilderType.DVBuilder)]
    public class DVDateTimeColumn : DVBaseColumn
    {
        public string Format { get; set; }

        public DateTimeRenderType RenderAs { get; set; }

        public string LinkRefId { get; set; }
    }
}
