using ExpressBase.Common;
using ExpressBase.Common.JsonConverters;
using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using ExpressBase.Common.Structures;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
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

    public enum StringOperators
    {
        Equals = 0,
        Startwith = 1,
        EndsWith = 2,
        Between = 3
    }
    public enum NumericOperators
    {
        Equals = 0,
        LessThan = 1,
        GreaterThan = 2,
        LessThanOrEqual = 3,
        GreaterThanOrEqual = 4,
        Between = 5
    }


    [EnableInBuilder(BuilderType.DVBuilder, BuilderType.BotForm)]
    [HideInPropertyGrid]
    public class DVBaseColumn : EbDataVisualizationObject
    {
        [JsonProperty(PropertyName = "data")]
        public int Data { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder, BuilderType.BotForm)]
        [JsonProperty(PropertyName = "name")]
        [Alias("Name")]
        [PropertyEditor(PropertyEditorType.Label)]
        [HideInPropertyGrid]
        public override string Name { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder, BuilderType.BotForm)]
        [HideInPropertyGrid]
        public string EbSid{ get; set; }

        public virtual EbDbTypes Type { get; set; }

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

        [EnableInBuilder(BuilderType.DVBuilder, BuilderType.BotForm)]
        [PropertyEditor(PropertyEditorType.ScriptEditorCS)]
        [JsonConverter(typeof(Base64Converter))]
        public string Formula { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder, BuilderType.BotForm)]
        [HideInPropertyGrid]
        public bool IsCustomColumn { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder, BuilderType.BotForm, BuilderType.FilterDialog)]
        [PropertyEditor(PropertyEditorType.ObjectSelector)]
        [OSE_ObjectTypes(EbObjectTypes.iTableVisualization, EbObjectTypes.iChartVisualization, EbObjectTypes.iReport, EbObjectTypes.iWebForm)]
        public string LinkRefId { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder)]
        [PropertyEditor(PropertyEditorType.DropDown)]
        public LinkTypeEnum LinkType { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder)]
        [DefaultPropValue("0")]
        [HideForUser]
        public int HideDataRowMoreThan { get; set; }

        public virtual CultureInfo GetColumnCultureInfo(CultureInfo user_cultureinfo)
        {
            return user_cultureinfo;
        }

        public DVBaseColumn ShallowCopy()
        {
            return (DVBaseColumn)this.MemberwiseClone();
        }
    }

    public class DVColumnCollection : List<DVBaseColumn>
    {
        public bool Contains(string name)
        {
            foreach (DVBaseColumn col in this)
            {
                if (col.Name.Equals(name))
                    return true;
            }

            return false;
        }

        public DVBaseColumn Get(string name, EbDbTypes type)
        {
            foreach (DVBaseColumn col in this)
            {
                if (col.Name.Equals(name) && col.Type == type)
                    return col;
            }

            return null;
        }

        public DVBaseColumn Get(string name)
        {
            foreach (DVBaseColumn col in this)
            {
                if (col.Name.Equals(name))
                    return col;
            }

            return null;
        }

        public DVBaseColumn Pop(string name, EbDbTypes type)
        {
            DVBaseColumn tempCol = null;
            foreach (DVBaseColumn col in this)
            {
                if (col.Name.Equals(name) && col.Type == type)
                {
                    tempCol = col;
                    break;
                }
            }

            this.Remove(tempCol);

            return tempCol;
        }
    }

	//public class DVNonVisibleColumnCollection : List<string>
	//{

	//}

    [EnableInBuilder(BuilderType.DVBuilder, BuilderType.BotForm, BuilderType.FilterDialog)]
    [Alias("DVStringColumnAlias")]
    public class DVStringColumn : DVBaseColumn
    {
        [EnableInBuilder(BuilderType.DVBuilder, BuilderType.BotForm, BuilderType.FilterDialog)]
        [DefaultPropValue("0")]
        [PropertyEditor(PropertyEditorType.DropDown)]
        [OnChangeExec(@"
if(this.RenderAs !== 2){
    pg.HideProperty('LinkRefId');
    pg.HideProperty('LinkType');
    pg.setSimpleProperty('LinkRefId', null);
}
else{
    pg.ShowProperty('LinkRefId');
    pg.ShowProperty('LinkType');
    }")]
        public StringRenderType RenderAs { get; set; }


        //[EnableInBuilder(BuilderType.DVBuilder, BuilderType.BotForm, BuilderType.FilterDialog)]
        //[PropertyEditor(PropertyEditorType.ObjectSelector)]
        //[OSE_ObjectTypes(EbObjectTypes.iTableVisualization, EbObjectTypes.iChartVisualization, EbObjectTypes.iReport)]
        //public string LinkRefId { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder)]
        [DefaultPropValue("16")]
        [HideInPropertyGrid]
        public override EbDbTypes Type { get; set; }


        [EnableInBuilder(BuilderType.DVBuilder)]
        [PropertyEditor(PropertyEditorType.DropDown)]
        public StringOperators DefaultOperator { get; set; }
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
if(this.RenderAs !== 2){
    pg.HideProperty('LinkRefId');
    pg.HideProperty('LinkType');
pg.setSimpleProperty('LinkRefId', null);
}
else{
    pg.ShowProperty('LinkRefId');
    pg.ShowProperty('LinkType');
    }")]
        public NumericRenderType RenderAs { get; set; }

        //[EnableInBuilder(BuilderType.DVBuilder, BuilderType.BotForm, BuilderType.FilterDialog)]
        //[PropertyEditor(PropertyEditorType.ObjectSelector)]
        //[OSE_ObjectTypes(EbObjectTypes.iTableVisualization, EbObjectTypes.iChartVisualization, EbObjectTypes.iReport)]
        //public string LinkRefId { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder)]
        [DefaultPropValue("11")]
        [HideInPropertyGrid]
        public override EbDbTypes Type { get; set; }

        public override CultureInfo GetColumnCultureInfo(CultureInfo user_cultureinfo)
        {
            var cultureInfo = user_cultureinfo.Clone() as CultureInfo;

            cultureInfo.NumberFormat.NumberDecimalDigits = this.DecimalPlaces;

            return cultureInfo;
        }

        [EnableInBuilder(BuilderType.DVBuilder)]
        [PropertyEditor(PropertyEditorType.DropDown)]
        public NumericOperators DefaultOperator { get; set; }
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
if(this.RenderAs !== 1){
    pg.HideProperty('LinkRefId');
    pg.HideProperty('LinkType');
pg.setSimpleProperty('LinkRefId', null);
}
else{
    pg.ShowProperty('LinkRefId');
    pg.ShowProperty('LinkType');
    }")]
        public DateTimeRenderType RenderAs { get; set; }

        //[EnableInBuilder(BuilderType.DVBuilder, BuilderType.BotForm, BuilderType.FilterDialog)]
        //[PropertyEditor(PropertyEditorType.ObjectSelector)]
        //[OSE_ObjectTypes(EbObjectTypes.iTableVisualization, EbObjectTypes.iChartVisualization, EbObjectTypes.iReport)]
        //public string LinkRefId { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder)]
        [DefaultPropValue("5")]
        [HideInPropertyGrid]
        public override EbDbTypes Type { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder)]
        [PropertyEditor(PropertyEditorType.DropDown)]
        public NumericOperators DefaultOperator { get; set; }
    }

    [EnableInBuilder(BuilderType.DVBuilder)]
    public class HideColumnData
    {
//        [OnChangeExec(@"
//if(this.Enable === False){
//    pg.HideProperty('UnRestrictedRowCount');
//    pg.HideProperty('ReplaceByCharacter');
//    pg.HideProperty('ReplaceByText');
//}
//else{
//    pg.ShowProperty('UnRestrictedRowCount');
//    pg.ShowProperty('ReplaceByCharacter');
//    pg.ShowProperty('ReplaceByText');
//    }")]
        [EnableInBuilder(BuilderType.DVBuilder)]
        public bool Enable { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder)]
        public int UnRestrictedRowCount { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder)]
        public char ReplaceByCharacter { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder)]
        public char ReplaceByText { get; set; }
    }
}
