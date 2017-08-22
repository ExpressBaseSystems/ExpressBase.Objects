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

    public class DVBaseColumn
    {
        public int Data { get; set; }

        public string Name { get; set; }

        public DbType Type { get; set; }

        public string Title { get; set; }

        public bool Visible { get; set; }

        public int Pos { get; set; }

        public int Width { get; set; }
    }

    public class DVColumnCollection : List<DVBaseColumn>
    {

    }

    public class DVStringColumn : DVBaseColumn
    {
        public StringRenderType RenderAs { get; set; }

        public string LinkRefId { get; set; }
    }

    public class DVNumericColumn : DVBaseColumn
    {
        public bool Aggregate { get; set; }

        public int DecimalPlaces { get; set; }

        public NumericRenderType RenderAs { get; set; }

        public string LinkRefId { get; set; }
    }

    public class DVBooleanColumn : DVBaseColumn
    {
        public bool IsEditable { get; set; }

        public BooleanRenderType RenderAs { get; set; }
    }

    public class DVDateTimeColumn : DVBaseColumn
    {
        public string Format { get; set; }

        public DateTimeRenderType RenderAs { get; set; }

        public string LinkRefId { get; set; }
    }
}
