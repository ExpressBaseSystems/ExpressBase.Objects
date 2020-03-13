using ExpressBase.Common;
using ExpressBase.Common.Constants;
using ExpressBase.Common.Data;
using ExpressBase.Common.JsonConverters;
using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using ExpressBase.Common.Structures;
using ExpressBase.Objects.ServiceStack_Artifacts;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;

namespace ExpressBase.Objects.Objects.DVRelated
{
    public enum StringRenderType
    {
        Default = 0,
        Chart = 1,
        Link = 2,
        Marker = 3,
        Image = 4,
        Icon = 5,
        Tree = 6,
        Boolean = 7,
        Table = 8,
        LinkFromColumn = 9,
    }

    public enum NumericRenderType
    {
        Default,
        ProgressBar,
        Link,
        Tree,
        Boolean,
        Rating
    }

    public enum BooleanRenderType
    {
        Default,
        Icon,
        Link,
        IsEditable,
        Tree
    }

    public enum DateTimeRenderType
    {
        Default,
        Link,
        Tree
    }

    public enum AdvancedRenderType
    {
        Default,
        Icon

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

    public enum DatePattern
    {
        Default,

        [Description("Month Year")]
        MMMM_yyyy,

        [Description("MMM/yyyy")]
        MMM_yyyy 
    }

    public enum StringOperators
    {
        Equals = 0,
        Startwith = 1,
        EndsWith = 2,
        Between = 3,
        Contains = 4
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

    public enum BooleanOperators
    {
        Equals = 0
    }

    public enum AdvancedBooleanOperators
    {
        IsTrue = 0,
        IsFalse = 1,
        IsNull = 2,
    }

    public enum Align
    {
        Auto = 0,
        Left = 1,
        Right = 2,
        Center = 3
    }

    public enum OrderByDirection
    {
        ASC = 0,
        DESC = 1
    }

    public enum ImageQuality
    {
        Small = 0,
        Medium = 1
    }

    public enum AggregateFun
    {
        Count = 0,
        Sum = 1
    }

    public enum NumericSubType
    {
        None = 0,
        Time_Interval_In_Hour = 1,
        Time_Interval_In_Minute = 2,
        Time_Interval_In_Second = 3
    }

    public enum NumericSubTypeFromat
    {
        Days = 0,
        Hours = 1
    }

    [EnableInBuilder(BuilderType.DVBuilder, BuilderType.WebForm, BuilderType.BotForm, BuilderType.FilterDialog, BuilderType.UserControl, BuilderType.DashBoard, BuilderType.Calendar)]
    public class DVBaseColumn : EbDataVisualizationObject
    {

        public DVBaseColumn()
        {
            _Formula = new EbScript();
            this.StaticParameters = new List<StaticParam>();
            this.FormParameters = new List<DVBaseColumn>();
            this.FormId = new List<DVBaseColumn>();
            this.ParentColumn = new List<DVBaseColumn>();
            this.GroupingColumn = new List<DVBaseColumn>();
            this.GroupFormParameters = new List<DVBaseColumn>();
            this.GroupFormId = new List<DVBaseColumn>();
            this.ItemFormParameters = new List<DVBaseColumn>();
            this.ItemFormId = new List<DVBaseColumn>();
            this.InfoWindow = new List<DVBaseColumn>();
        }

        [JsonProperty(PropertyName = "data")]
        [EnableInBuilder(BuilderType.DVBuilder, BuilderType.WebForm, BuilderType.BotForm, BuilderType.FilterDialog, BuilderType.UserControl, BuilderType.DashBoard, BuilderType.Calendar)]
        [HideInPropertyGrid]
        public int Data { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder, BuilderType.WebForm, BuilderType.BotForm, BuilderType.FilterDialog, BuilderType.UserControl, BuilderType.DashBoard, BuilderType.Calendar)]
        [HideInPropertyGrid]
        public int OIndex { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder, BuilderType.WebForm, BuilderType.BotForm, BuilderType.FilterDialog, BuilderType.UserControl, BuilderType.DashBoard, BuilderType.Calendar)]
        [JsonProperty(PropertyName = "name")]
        [PropertyEditor(PropertyEditorType.Label)]
        [OnChangeExec(@"
            pg.MakeReadOnly('name');
        ")]
        public override string Name { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder, BuilderType.WebForm, BuilderType.BotForm, BuilderType.FilterDialog, BuilderType.UserControl, BuilderType.DashBoard, BuilderType.Calendar)]
        [HideInPropertyGrid]
        public string EbSid { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder, BuilderType.WebForm, BuilderType.BotForm, BuilderType.FilterDialog, BuilderType.UserControl, BuilderType.DashBoard, BuilderType.Calendar)]
        [HideInPropertyGrid]
        public virtual EbDbTypes Type { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder, BuilderType.BotForm, BuilderType.Calendar)]
        [HideInPropertyGrid]
        public string sType { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder, BuilderType.DashBoard, BuilderType.Calendar, BuilderType.WebForm, BuilderType.BotForm, BuilderType.FilterDialog)]
        [Alias("Title")]
        [HideInPropertyGrid]
        public string sTitle { get; set; }

        [HideInPropertyGrid]
        [EnableInBuilder(BuilderType.DVBuilder, BuilderType.DashBoard, BuilderType.Calendar, BuilderType.WebForm, BuilderType.BotForm, BuilderType.FilterDialog)]
        public bool bVisible { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder, BuilderType.DashBoard)]
        [HideInPropertyGrid]
        public int Pos { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder, BuilderType.DashBoard, BuilderType.Calendar)]
        [Alias("Width")]
        [HideInPropertyGrid]
        [JsonIgnore]
        public string sWidth { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.FilterDialog)]
        [PropertyGroup(PGConstants.APPEARANCE)]
        public virtual int Width { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder, BuilderType.DashBoard, BuilderType.Calendar, BuilderType.WebForm, BuilderType.BotForm, BuilderType.FilterDialog)]
        [JsonProperty(PropertyName = "className")]
        [HideInPropertyGrid]
        [DefaultPropValue("tdheight")]
        public string ClassName { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder, BuilderType.DashBoard, BuilderType.Calendar)]
        [PropertyEditor(PropertyEditorType.FontSelector)]
        [PropertyGroup(PGConstants.APPEARANCE)]
        public EbFont Font { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder, BuilderType.DashBoard)]
        [PropertyEditor(PropertyEditorType.DropDown)]
        [PropertyGroup("Search")]
        public ControlType FilterControl { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder, BuilderType.DashBoard, BuilderType.Calendar)]
        [PropertyEditor(PropertyEditorType.ScriptEditorCS)]
        [Alias("Formula")]
        public EbScript _Formula { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder)]
        [PropertyEditor(PropertyEditorType.ScriptEditorCS)]
        [JsonConverter(typeof(Base64Converter))]
        [Alias("Formula old")]
        [HideInPropertyGrid]
        public string Formula { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder, BuilderType.DashBoard, BuilderType.Calendar)]
        [HideInPropertyGrid]
        [OnChangeExec(@"
console.log('ggggggg ----------- IsCustomColumn');
if(this.IsCustomColumn){
    pg.ShowProperty('_Formula');
}
else {
    pg.HideProperty('_Formula');
}")]
        public bool IsCustomColumn { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder, BuilderType.DashBoard)]
        [PropertyEditor(PropertyEditorType.ObjectSelector)]
        [OSE_ObjectTypes(EbObjectTypes.iTableVisualization, EbObjectTypes.iChartVisualization, EbObjectTypes.iReport, EbObjectTypes.iWebForm , EbObjectTypes.iDashBoard)]
        [OnChangeExec(@"
if(this.LinkRefId !== null){
    console.log(this.LinkRefId);
        pg.ShowProperty('LinkType');
    if(this.LinkRefId.split('-')[2] === '0'){
        console.log('Link to form');
        pg.ShowGroup('FormSettings');
        pg.ShowProperty('FormMode');
        pg.HideProperty('FormParameters');
        pg.HideProperty('FormId');
    }
    else{
        console.log('Link to Other');
        pg.HideGroup('FormSettings');
    }
}
else{
    pg.HideGroup('FormSettings');
    pg.HideProperty('LinkType');
}")]
        [PropertyGroup("Link")]
        [Alias("Object")]
        public string LinkRefId { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder,  BuilderType.DashBoard)]
        [PropertyEditor(PropertyEditorType.DropDown)]
        [OnChangeExec(@"
console.log('onchange fired');
if(this.FormMode === 1){
    pg.ShowProperty('FormId');
    pg.HideProperty('FormParameters');
}
else if(this.FormMode === 2){
    pg.HideProperty('FormId');
    pg.ShowProperty('FormParameters');
}
else{
    pg.HideProperty('FormId');
    pg.HideProperty('FormParameters');
}
")]
        [PropertyGroup("FormSettings")]
        public WebFormDVModes FormMode { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder, BuilderType.DashBoard)]
        [HideInPropertyGrid]
        [JsonIgnore]
        public DVColumnCollection ColumnsRef { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder,  BuilderType.DashBoard)]
        [PropertyEditor(PropertyEditorType.CollectionFrmSrc, "ColumnsRef")]
        [PropertyGroup("FormSettings")]
        public List<DVBaseColumn> FormId { get; set; }


        [EnableInBuilder(BuilderType.DVBuilder,  BuilderType.DashBoard)]
        [PropertyEditor(PropertyEditorType.Mapper, "ColumnsRef", "LinkRefId", "FormControl")]
        [PropertyGroup("FormSettings")]
        public List<DVBaseColumn> FormParameters { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder,  BuilderType.DashBoard)]
        [HideInPropertyGrid]
        public EbControl FormControl { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder, BuilderType.DashBoard)]
        [PropertyEditor(PropertyEditorType.DropDown)]
        [PropertyGroup(PGConstants.APPEARANCE)]
        public LinkTypeEnum LinkType { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder, BuilderType.DashBoard)]
        [HideForUser]
        [PropertyGroup("Link")]
        public bool ShowLinkifNoData { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder, BuilderType.DashBoard)]
        [PropertyEditor(PropertyEditorType.CollectionFrmSrc, "ColumnsRef")]
        [PropertyGroup("LinkFromColumn")]
        [HideInPropertyGrid]
        public DVBaseColumn RefidColumn { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder, BuilderType.DashBoard)]
        [PropertyEditor(PropertyEditorType.CollectionFrmSrc, "ColumnsRef")]
        [PropertyGroup("LinkFromColumn")]
        [HideInPropertyGrid]
        public DVBaseColumn IdColumn { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder, BuilderType.DashBoard)]
        [DefaultPropValue("0")]
        [HideForUser]
        [PropertyGroup(PGConstants.EXTENDED)]
        public int HideDataRowMoreThan { get; set; }

        [PropertyGroup("TreeVisualization")]
        [EnableInBuilder(BuilderType.DVBuilder, BuilderType.DashBoard)]
        [PropertyEditor(PropertyEditorType.CollectionFrmSrc, "ColumnsRef")]
        public List<DVBaseColumn> ParentColumn { get; set; }

        [PropertyGroup("TreeVisualization")]
        [EnableInBuilder(BuilderType.DVBuilder, BuilderType.DashBoard)]
        [PropertyEditor(PropertyEditorType.CollectionFrmSrc, "ColumnsRef")]
        public List<DVBaseColumn> GroupingColumn { get; set; }

        [PropertyGroup("TreeVisualization")]
        [EnableInBuilder(BuilderType.DVBuilder, BuilderType.DashBoard)]
        [PropertyEditor(PropertyEditorType.ObjectSelector)]
        [OSE_ObjectTypes(EbObjectTypes.iWebForm)]
        public string GroupFormLink { get; set; }

        [PropertyGroup("TreeVisualization")]
        [EnableInBuilder(BuilderType.DVBuilder, BuilderType.DashBoard)]
        [PropertyEditor(PropertyEditorType.ObjectSelector)]
        [OSE_ObjectTypes(EbObjectTypes.iWebForm)]
        public string ItemFormLink { get; set; }

        [PropertyGroup("TreeVisualization")]
        [EnableInBuilder(BuilderType.DVBuilder, BuilderType.DashBoard)]
        [PropertyEditor(PropertyEditorType.CollectionFrmSrc, "ColumnsRef")]
        public List<DVBaseColumn> GroupFormId { get; set; }

        [PropertyGroup("TreeVisualization")]
        [EnableInBuilder(BuilderType.DVBuilder, BuilderType.DashBoard)]
        [PropertyEditor(PropertyEditorType.Mapper, "ColumnsRef", "GroupFormLink", "FormControl")]
        public List<DVBaseColumn> GroupFormParameters { get; set; }

        [PropertyGroup("TreeVisualization")]
        [EnableInBuilder(BuilderType.DVBuilder, BuilderType.DashBoard)]
        [PropertyEditor(PropertyEditorType.CollectionFrmSrc, "ColumnsRef")]
        public List<DVBaseColumn> ItemFormId { get; set; }

        [PropertyGroup("TreeVisualization")]
        [EnableInBuilder(BuilderType.DVBuilder, BuilderType.DashBoard)]
        [PropertyEditor(PropertyEditorType.Mapper, "ColumnsRef", "ItemFormLink", "FormControl")]
        public List<DVBaseColumn> ItemFormParameters { get; set; }

        [PropertyGroup("TreeVisualization")]
        [EnableInBuilder(BuilderType.DVBuilder, BuilderType.DashBoard)]
        [HideInPropertyGrid]
        public bool IsTree { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder, BuilderType.DashBoard)]
        [PropertyEditor(PropertyEditorType.Collection)]
        [HideInPropertyGrid]
        public List<StaticParam> StaticParameters { get; set; }

        [PropertyGroup("Tooltip")]
        [EnableInBuilder(BuilderType.DVBuilder, BuilderType.DashBoard)]
        public int AllowedCharacterLength { get; set; }

        [PropertyGroup("Tooltip")]
        [EnableInBuilder(BuilderType.DVBuilder, BuilderType.DashBoard)]
        [PropertyEditor(PropertyEditorType.CollectionFrmSrc, "ColumnsRef")]
        public List<DVBaseColumn> InfoWindow { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder, BuilderType.DashBoard, BuilderType.Calendar)]
        [PropertyEditor(PropertyEditorType.Collection)]
        [PropertyGroup(PGConstants.CORE)]
        public List<ColumnCondition> ConditionalFormating { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder, BuilderType.DashBoard)]
        [MetaOnly]
        public OrderByDirection Direction { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder, BuilderType.DashBoard)]
        [HideInPropertyGrid]
        public ControlClass ColumnQueryMapping { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder, BuilderType.DashBoard)]
        [HideInPropertyGrid]
        public bool AutoResolve { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder, BuilderType.DashBoard, BuilderType.Calendar)]
        [HideInPropertyGrid]
        public EbDbTypes RenderType { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder, BuilderType.DashBoard)]
        [DefaultPropValue("T")]
        public string TrueValue { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder, BuilderType.DashBoard)]
        [DefaultPropValue("F")]
        public string FalseValue { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder, BuilderType.Calendar)]
        [HideInPropertyGrid]
        public string HeaderTooltipText { get; set; }

        [EnableInBuilder(BuilderType.Calendar)]
        public AggregateFun AggregateFun { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder, BuilderType.DashBoard, BuilderType.Calendar)]
        [PropertyEditor(PropertyEditorType.DropDown)]
        [PropertyGroup(PGConstants.APPEARANCE)]
        public Align Align { get; set; }

        [JsonIgnore]
        private List<string> __formulaDataFieldsUsed = null;
        [JsonIgnore]
        public List<string> FormulaDataFieldsUsed
        {
            get
            {
                if (__formulaDataFieldsUsed == null)
                {
                    var matches = Regex.Matches(this._Formula.Code, @"T[0-9]{1}.\w+").OfType<Match>().Select(m => m.Groups[0].Value).Distinct();
                    __formulaDataFieldsUsed = new List<string>(matches.Count());
                    int j = 0;
                    foreach (var match in matches)
                        __formulaDataFieldsUsed.Add(match);
                }

                return __formulaDataFieldsUsed;
            }
        }

        [JsonIgnore]
        private List<FormulaPart> __formulaParts = new List<FormulaPart>();
        [JsonIgnore]
        public List<FormulaPart> FormulaParts
        {
            get
            {
                if (__formulaParts.Count == 0)
                {
                    foreach (string calcfd in this.FormulaDataFieldsUsed)
                    {
                        string[] splits = calcfd.Split('.');
                        __formulaParts.Add(new FormulaPart { TableName = splits[0], FieldName = splits[1] });
                    }
                }

                return __formulaParts;
            }
        }

        [JsonIgnore]
        private Script __codeAnalysisScript = null;

        public Script GetCodeAnalysisScript()
        {
            if (__codeAnalysisScript == null && !string.IsNullOrEmpty(this._Formula.Code))
            {
                __codeAnalysisScript = CSharpScript.Create<dynamic>(this._Formula.Code, ScriptOptions.Default.WithReferences("Microsoft.CSharp", "System.Core").WithImports("System.Dynamic", "System", "System.Collections.Generic", "System.Diagnostics", "System.Linq"), globalsType: typeof(Globals));
                __codeAnalysisScript.Compile();
            }

            return __codeAnalysisScript;
        }

        public virtual CultureInfo GetColumnCultureInfo(CultureInfo user_cultureinfo)
        {
            return user_cultureinfo;
        }

        public DVBaseColumn ShallowCopy()
        {
            return (DVBaseColumn)this.MemberwiseClone();
        }

        public bool Check4FormLink()
        {
            if (!string.IsNullOrEmpty(this.LinkRefId) && Convert.ToInt32(this.LinkRefId.Split("-")[2]) == (int)EbObjectTypes.WebForm)
            {
                return true;
            }
            return false;
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

        public DVBaseColumn Pop(string name, EbDbTypes type, bool iscustom)
        {
            DVBaseColumn tempCol = null;
            foreach (DVBaseColumn col in this)
            {
                if (col.Name.Equals(name) && col.Type == type && !iscustom)
                {
                    tempCol = col;
                    break;
                }
            }

            this.Remove(tempCol);

            return tempCol;
        }

        public DVColumnCollection(){}

        public DVColumnCollection (DVColumnCollection other)
        {
            foreach(var aa in other)
            {
                this.Add(aa.ShallowCopy());
            }
        }
    }

    [EnableInBuilder(BuilderType.DVBuilder, BuilderType.WebForm, BuilderType.BotForm, BuilderType.FilterDialog, BuilderType.UserControl, BuilderType.DashBoard, BuilderType.Calendar)]
    [Alias("DVStringColumnAlias")]
    public class DVStringColumn : DVBaseColumn
    {
        [OnDeserialized]
        public void OnDeserializedMethod(StreamingContext context)
        {
            if (this.RenderType == EbDbTypes.AnsiString)
                this.RenderType = this.Type;
        }

        [EnableInBuilder(BuilderType.DVBuilder, BuilderType.WebForm, BuilderType.BotForm, BuilderType.FilterDialog, BuilderType.UserControl, BuilderType.DashBoard, BuilderType.Calendar)]
        [DefaultPropValue("0")]
        [PropertyEditor(PropertyEditorType.DropDown)]
        [PropertyGroup(PGConstants.CORE)]
        [PropertyPriority(10)]
        [OnChangeExec(@"
pg.HideProperty('TrueValue');
pg.HideProperty('FalseValue');
pg.HideGroup('FormSettings');
pg.HideGroup('Image');
pg.HideGroup('LinkFromColumn');
if(this.RenderAs === 2){
console.log('Render as link');
    pg.ShowGroup('Link');
    pg.ShowProperty('LinkType');
    pg.ShowProperty('HideLinkifNoData');
    pg.HideGroup('TreeVisualization');
    pg.setSimpleProperty('IsTree', false);
}
else if(this.RenderAs === 6){
console.log('Render as tree');
    pg.ShowGroup('TreeVisualization');
    pg.setSimpleProperty('IsTree', true);
    pg.HideGroup('Link');
    pg.HideProperty('HideLinkifNoData');
    pg.ShowProperty('LinkType');
}
else{
console.log('Render as other');
if(this.RenderAs === 7){
pg.setSimpleProperty('RenderType', 3);
pg.ShowProperty('TrueValue');
pg.ShowProperty('FalseValue');
}
if(this.RenderAs === 4){
    pg.ShowGroup('Image');
}
if(this.RenderAs === 9){
    pg.ShowGroup('LinkFromColumn');
}
    pg.HideGroup('Link');
    pg.HideProperty('LinkType');
    pg.HideProperty('HideLinkifNoData');
    pg.setSimpleProperty('LinkRefId', null);
    pg.HideGroup('TreeVisualization');
    pg.setSimpleProperty('IsTree', false);
    }")]
        public StringRenderType RenderAs { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder, BuilderType.WebForm, BuilderType.BotForm, BuilderType.FilterDialog, BuilderType.UserControl, BuilderType.DashBoard, BuilderType.Calendar)]
        [DefaultPropValue("16")]
        [HideInPropertyGrid]
        public override EbDbTypes Type { get; set; }


        [EnableInBuilder(BuilderType.DVBuilder, BuilderType.BotForm, BuilderType.FilterDialog, BuilderType.UserControl, BuilderType.DashBoard)]
        [PropertyEditor(PropertyEditorType.DropDown)]
        [PropertyGroup("Search")]
        public StringOperators DefaultOperator { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder, BuilderType.DashBoard)]
        [DefaultPropValue("20")]
        [PropertyGroup("Image")]
        public int ImageHeight { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder, BuilderType.DashBoard)]
        [PropertyGroup("Image")]
        [DefaultPropValue("20")]
        public int ImageWidth { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder, BuilderType.DashBoard)]
        [PropertyGroup("Image")]
        public ImageQuality ImageQuality { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder, BuilderType.DashBoard)]
        [OnChangeExec(@"
if(this.AllowMultilineText){
    pg.ShowProperty('NoOfLines');
    pg.ShowProperty('NoOfCharactersPerLine');
}
else {
    pg.HideProperty('NoOfCharactersPerLine');
    pg.HideProperty('NoOfLines');
}")]
        [PropertyGroup(PGConstants.EXTENDED)]
        public bool AllowMultilineText { get; set; }

        [PropertyGroup(PGConstants.EXTENDED)]
        [EnableInBuilder(BuilderType.DVBuilder)]
        public int NoOfLines { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder)]
        [PropertyGroup(PGConstants.EXTENDED)]
        public int NoOfCharactersPerLine { get; set; }

        public DVStringColumn()
        {
            this.ConditionalFormating = new List<ColumnCondition>();
        }

        public DVStringColumn(EbDataColumn col)
        {
            this.Data = col.ColumnIndex;
            this.Name = col.ColumnName;
            this.sTitle = col.ColumnName;
            this.Type = col.Type;
            this.bVisible = true;
            this.ClassName = "tdheight";
        }

    }

    [EnableInBuilder(BuilderType.DVBuilder, BuilderType.WebForm, BuilderType.BotForm, BuilderType.FilterDialog, BuilderType.UserControl, BuilderType.DashBoard, BuilderType.Calendar)]
    public class DVNumericColumn : DVBaseColumn
    {
        [OnDeserialized]
        public void OnDeserializedMethod(StreamingContext context)
        {
            if (this.RenderType == EbDbTypes.AnsiString)
                this.RenderType = this.Type;
        }
        [EnableInBuilder(BuilderType.DVBuilder, BuilderType.WebForm, BuilderType.BotForm, BuilderType.FilterDialog, BuilderType.UserControl, BuilderType.DashBoard, BuilderType.Calendar)]
        [OnChangeExec(@"console.log('------------   this.Type')")]
        [PropertyGroup(PGConstants.CORE)]
        public bool Aggregate { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder, BuilderType.WebForm, BuilderType.BotForm, BuilderType.FilterDialog, BuilderType.UserControl, BuilderType.DashBoard, BuilderType.Calendar)]
        [PropertyGroup(PGConstants.CORE)]
        public int DecimalPlaces { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder, BuilderType.WebForm, BuilderType.BotForm, BuilderType.FilterDialog, BuilderType.UserControl, BuilderType.DashBoard, BuilderType.Calendar)]
        [OnChangeExec(@"
pg.HideProperty('TrueValue');
pg.HideProperty('FalseValue');
pg.HideGroup('FormSettings');
pg.HideGroup('LinkFromColumn');
    pg.HideGroup('Rating');
if(this.RenderAs === 2){
    pg.ShowGroup('Link');
    pg.ShowProperty('LinkType');
    pg.ShowProperty('HideLinkifNoData');
    pg.HideGroup('TreeVisualization');
    pg.setSimpleProperty('IsTree', false);
}
else if(this.RenderAs === 3){
    pg.ShowGroup('TreeVisualization');
    pg.setSimpleProperty('IsTree', true);
    pg.HideGroup('Link');
    pg.HideProperty('HideLinkifNoData');
    pg.ShowProperty('LinkType');
}
else{
if(this.RenderAs === 4){
    pg.setSimpleProperty('RenderType', 3);
    pg.ShowProperty('TrueValue');
    pg.ShowProperty('FalseValue');
}
else if(this.RenderAs === 5){
    pg.ShowGroup('Rating');
}
    pg.HideGroup('Link');
    pg.HideProperty('LinkType');
    pg.HideProperty('HideLinkifNoData');
    pg.setSimpleProperty('LinkRefId', null);
    pg.HideGroup('TreeVisualization');
    pg.setSimpleProperty('IsTree', false);
    }")]
        [PropertyPriority(10)]
        [PropertyGroup(PGConstants.CORE)]
        public NumericRenderType RenderAs { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder, BuilderType.WebForm, BuilderType.BotForm, BuilderType.FilterDialog, BuilderType.UserControl, BuilderType.DashBoard, BuilderType.Calendar)]
        [PropertyGroup("Rating")]
        [DefaultPropValue("5")]
        public int MaxLimit { get; set; }

        [DefaultPropValue("7")]
        [HideInPropertyGrid]
        [EnableInBuilder(BuilderType.DVBuilder, BuilderType.WebForm, BuilderType.BotForm, BuilderType.FilterDialog, BuilderType.UserControl, BuilderType.DashBoard, BuilderType.Calendar)]
        public override EbDbTypes Type { get; set; }

        private CultureInfo cultureInfo = null;
        public override CultureInfo GetColumnCultureInfo(CultureInfo user_cultureinfo)
        {
            if (cultureInfo == null)
            {
                cultureInfo = user_cultureinfo.Clone() as CultureInfo;
                cultureInfo.NumberFormat.NumberDecimalDigits = this.DecimalPlaces;
            }

            return cultureInfo;
        }

        [EnableInBuilder(BuilderType.DVBuilder, BuilderType.BotForm, BuilderType.FilterDialog, BuilderType.UserControl, BuilderType.DashBoard)]
        [PropertyEditor(PropertyEditorType.DropDown)]
        [PropertyGroup("Search")]
        public NumericOperators DefaultOperator { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder, BuilderType.WebForm, BuilderType.BotForm, BuilderType.FilterDialog, BuilderType.UserControl, BuilderType.DashBoard, BuilderType.Calendar)]
        [PropertyGroup(PGConstants.EXTENDED)]
        public bool SuppresIfZero { get; set; }

        public DVNumericColumn()
        {
            this.ConditionalFormating = new List<ColumnCondition>();
        }

        public DVNumericColumn(EbDataColumn col)
        {
            this.Data = col.ColumnIndex;
            this.Name = col.ColumnName;
            this.sTitle = col.ColumnName;
            this.Type = col.Type;
            this.bVisible = true;
            this.ClassName = "tdheight";
        }

    }

    [EnableInBuilder(BuilderType.DVBuilder, BuilderType.WebForm, BuilderType.BotForm, BuilderType.FilterDialog, BuilderType.UserControl, BuilderType.DashBoard, BuilderType.Calendar)]
    public class DVBooleanColumn : DVBaseColumn
    {

        [OnDeserialized]
        public void OnDeserializedMethod(StreamingContext context)
        {
            if (this.RenderType == EbDbTypes.AnsiString)
                this.RenderType = this.Type;
        }

        [EnableInBuilder(BuilderType.DVBuilder, BuilderType.WebForm, BuilderType.BotForm, BuilderType.FilterDialog, BuilderType.UserControl, BuilderType.DashBoard, BuilderType.Calendar)]
        [OnChangeExec(@"
pg.ShowProperty('TrueValue');
pg.ShowProperty('FalseValue');
pg.HideGroup('FormSettings');
pg.HideGroup('LinkFromColumn');
if(this.RenderAs === 2){
    pg.ShowGroup('Link');
    pg.ShowProperty('LinkType');
    pg.ShowProperty('HideLinkifNoData');
    pg.HideGroup('TreeVisualization');
    pg.setSimpleProperty('IsTree', false);
}
else if(this.RenderAs === 4){
   pg.ShowGroup('TreeVisualization');
    pg.setSimpleProperty('IsTree', true);
    pg.HideGroup('Link');
    pg.HideProperty('HideLinkifNoData');
    pg.ShowProperty('LinkType');
}
else{
    pg.HideGroup('Link');
    pg.HideProperty('LinkType');
    pg.HideProperty('HideLinkifNoData');
    pg.setSimpleProperty('LinkRefId', null);
    pg.HideGroup('TreeVisualization');
    pg.setSimpleProperty('IsTree', false);
}")]
        [PropertyGroup(PGConstants.CORE)]
        [PropertyPriority(10)]
        public BooleanRenderType RenderAs { get; set; }

        public DVBooleanColumn()
        {
            this.ConditionalFormating = new List<ColumnCondition>();
        }

        public DVBooleanColumn(EbDataColumn col)
        {
            this.Data = col.ColumnIndex;
            this.Name = col.ColumnName;
            this.sTitle = col.ColumnName;
            this.Type = col.Type;
            this.bVisible = true;
            this.ClassName = "tdheight";
        }

    }

    [EnableInBuilder(BuilderType.DVBuilder, BuilderType.WebForm, BuilderType.BotForm, BuilderType.FilterDialog, BuilderType.UserControl, BuilderType.DashBoard, BuilderType.Calendar)]
    public class DVDateTimeColumn : DVBaseColumn
    {
        [OnDeserialized]
        public void OnDeserializedMethod(StreamingContext context)
        {
            if (this.RenderType == EbDbTypes.AnsiString)
                this.RenderType = this.Type;
        }

        [EnableInBuilder(BuilderType.DVBuilder, BuilderType.WebForm, BuilderType.BotForm, BuilderType.FilterDialog, BuilderType.UserControl, BuilderType.DashBoard, BuilderType.Calendar)]
        [OnChangeExec(@"
if(this.Format === 3){
    pg.ShowProperty('ConvretToUsersTimeZone');
}
else{
    pg.HideProperty('ConvretToUsersTimeZone');
    }")]
        [PropertyGroup(PGConstants.EXTENDED)]
        public DateFormat Format { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder, BuilderType.WebForm, BuilderType.BotForm, BuilderType.FilterDialog, BuilderType.UserControl, BuilderType.DashBoard, BuilderType.Calendar)]
        [PropertyGroup(PGConstants.EXTENDED)]
        public DatePattern Pattern { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder, BuilderType.WebForm, BuilderType.BotForm, BuilderType.FilterDialog, BuilderType.UserControl, BuilderType.DashBoard, BuilderType.Calendar)]
        [PropertyGroup(PGConstants.EXTENDED)]
        public bool ConvretToUsersTimeZone { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder, BuilderType.WebForm, BuilderType.BotForm, BuilderType.FilterDialog, BuilderType.UserControl, BuilderType.DashBoard, BuilderType.Calendar)]
        [OnChangeExec(@"
pg.HideProperty('TrueValue');
pg.HideProperty('FalseValue');
pg.HideGroup('FormSettings');
pg.HideGroup('LinkFromColumn');
if(this.RenderAs === 1){
    pg.ShowGroup('Link');
    pg.ShowProperty('LinkType');
    pg.ShowProperty('HideLinkifNoData');
    pg.HideGroup('TreeVisualization');
    pg.setSimpleProperty('IsTree', false);
}
else if(this.RenderAs === 2){
    pg.ShowGroup('TreeVisualization');
    pg.setSimpleProperty('IsTree', true);
    pg.HideGroup('Link');
    pg.HideProperty('HideLinkifNoData');
    pg.ShowProperty('LinkType');
}
else{
    pg.HideGroup('Link');
    pg.HideProperty('LinkType');
    pg.HideProperty('HideLinkifNoData');
    pg.setSimpleProperty('LinkRefId', null);
    pg.HideGroup('TreeVisualization');
    pg.setSimpleProperty('IsTree', false);
}")]
        [PropertyPriority(10)]
        [PropertyGroup(PGConstants.CORE)]
        public DateTimeRenderType RenderAs { get; set; }

        [DefaultPropValue("5")]
        [HideInPropertyGrid]
        [EnableInBuilder(BuilderType.DVBuilder, BuilderType.WebForm, BuilderType.BotForm, BuilderType.FilterDialog, BuilderType.UserControl, BuilderType.DashBoard, BuilderType.Calendar)]
        public override EbDbTypes Type { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder, BuilderType.WebForm, BuilderType.BotForm, BuilderType.FilterDialog, BuilderType.UserControl, BuilderType.DashBoard, BuilderType.Calendar)]
        [PropertyEditor(PropertyEditorType.DropDown)]
        [PropertyGroup("Search")]
        public NumericOperators DefaultOperator { get; set; }        

        public DVDateTimeColumn()
        {
            this.ConditionalFormating = new List<ColumnCondition>();
        }

        public DVDateTimeColumn(EbDataColumn col)
        {
            this.Data = col.ColumnIndex;
            this.Name = col.ColumnName;
            this.sTitle = col.ColumnName;
            this.Type = col.Type;
            this.bVisible = true;
            this.ClassName = "tdheight";
        }

    }

    [EnableInBuilder(BuilderType.DVBuilder, BuilderType.WebForm, BuilderType.BotForm, BuilderType.FilterDialog, BuilderType.UserControl, BuilderType.DashBoard, BuilderType.Calendar)]
    public class DVButtonColumn : DVBaseColumn
    {
        [EnableInBuilder(BuilderType.DVBuilder, BuilderType.WebForm, BuilderType.BotForm, BuilderType.FilterDialog, BuilderType.UserControl, BuilderType.DashBoard, BuilderType.Calendar)]
        public string ButtonText { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder, BuilderType.WebForm, BuilderType.BotForm, BuilderType.FilterDialog, BuilderType.UserControl, BuilderType.DashBoard, BuilderType.Calendar)]
        public string ButtonClassName { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder, BuilderType.WebForm, BuilderType.BotForm, BuilderType.FilterDialog, BuilderType.UserControl, BuilderType.DashBoard, BuilderType.Calendar)]
        public AdvancedCondition RenderCondition { get; set; }

        public DVButtonColumn()
        {
            this.ConditionalFormating = new List<ColumnCondition>();
        }


    }

    [EnableInBuilder(BuilderType.Calendar)]
    public class CalendarDynamicColumn: DVBaseColumn
    {
        [EnableInBuilder(BuilderType.Calendar)]
        [HideInPropertyGrid]
        public DateTime StartDT { get; set; }
        
        [EnableInBuilder(BuilderType.Calendar)]
        [HideInPropertyGrid]
        public DateTime EndDT { get; set; }

        [EnableInBuilder(BuilderType.Calendar)]
        [HideInPropertyGrid]
        public NumericRenderType RenderAs { get; set; }

        [EnableInBuilder(BuilderType.Calendar)]
        [OnChangeExec(@"
if(this.SubType === 0)
pg.HideProperty('SubTypeFormat');
else
pg.ShowProperty('SubTypeFormat');
")]
        public NumericSubType SubType { get; set; }

        [EnableInBuilder(BuilderType.Calendar)]
        public NumericSubTypeFromat SubTypeFormat { get; set; }

        public CalendarDynamicColumn()
        {
            this.ConditionalFormating = new List<ColumnCondition>();
        }
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

    [EnableInBuilder(BuilderType.DVBuilder)]
    public class StaticParam : EbDataVisualizationObject
    {
        public StaticParam() { }

        [EnableInBuilder(BuilderType.DVBuilder)]
        public EbDbTypes Type { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder)]
        public string Value { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder)]
        [HideInPropertyGrid]
        public string EbSid { get; set; }
    }

    public abstract class ColumnCondition : EbDataVisualizationObject
    {
        [EnableInBuilder(BuilderType.DVBuilder, BuilderType.Calendar)]
        [PropertyEditor(PropertyEditorType.Color)]
        public string FontColor { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder, BuilderType.Calendar)]
        [PropertyEditor(PropertyEditorType.Color)]
        public string BackGroundColor { get; set; }

        [HideInPropertyGrid]
        public string EbSid { get; set; }

        public abstract bool CompareValues(object _unformattedData);
    }

    [EnableInBuilder(BuilderType.DVBuilder,BuilderType.Calendar)]
    public class NumericCondition : ColumnCondition
    {
        public NumericCondition() { }

        [EnableInBuilder(BuilderType.DVBuilder, BuilderType.Calendar)]
        [OnChangeExec(@"
if(this.Operator === 5)
    pg.ShowProperty('Value1');
else
    pg.HideProperty('Value1');
    
")]
        public NumericOperators Operator { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder, BuilderType.Calendar)]
        public int Value { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder, BuilderType.Calendar)]
        public int Value1 { get; set; }

        public override bool CompareValues(object _unformattedData)
        {
            if (this.Operator == NumericOperators.Equals)
                return Convert.ToInt32(_unformattedData) == Convert.ToInt32(this.Value);
            else if (this.Operator == NumericOperators.LessThan)
                return Convert.ToInt32(_unformattedData) < Convert.ToInt32(this.Value);
            else if (this.Operator == NumericOperators.GreaterThan)
                return Convert.ToInt32(_unformattedData) > Convert.ToInt32(this.Value);
            else if (this.Operator == NumericOperators.LessThanOrEqual)
                return Convert.ToInt32(_unformattedData) <= Convert.ToInt32(this.Value);
            else if (this.Operator == NumericOperators.GreaterThanOrEqual)
                return Convert.ToInt32(_unformattedData) >= Convert.ToInt32(this.Value);
            else if (this.Operator == NumericOperators.Between)
                return Convert.ToInt32(_unformattedData) >= Convert.ToInt32(this.Value) && Convert.ToInt32(_unformattedData) <= Convert.ToInt32(this.Value1);

            return false;
        }
    }

    [EnableInBuilder(BuilderType.DVBuilder, BuilderType.Calendar)]
    public class StringCondition : ColumnCondition
    {
        public StringCondition() { }

        [EnableInBuilder(BuilderType.DVBuilder, BuilderType.Calendar)]
        public StringOperators Operator { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder, BuilderType.Calendar)]
        public string Value { get; set; }

        public override bool CompareValues(object _unformattedData)
        {
            string data = _unformattedData.ToString().Trim();
            string searchval = this.Value.Trim();

            if (this.Operator == StringOperators.Startwith)
                return data.StartsWith(searchval, StringComparison.OrdinalIgnoreCase);
            else if (this.Operator == StringOperators.EndsWith)
                return data.EndsWith(searchval, StringComparison.OrdinalIgnoreCase);
            else if (this.Operator == StringOperators.Contains)
                return data.Contains(searchval, StringComparison.OrdinalIgnoreCase);
            else if (this.Operator == StringOperators.Equals)
                return string.Equals(data, searchval, StringComparison.OrdinalIgnoreCase);

            return false;
        }
    }

    [EnableInBuilder(BuilderType.DVBuilder, BuilderType.Calendar)]
    public class BooleanCondition : ColumnCondition
    {
        public BooleanCondition() { }

        [EnableInBuilder(BuilderType.DVBuilder, BuilderType.Calendar)]
        public BooleanOperators Operator { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder, BuilderType.Calendar)]
        public bool Value { get; set; }

        public override bool CompareValues(object _unformattedData)
        {
            return false;
        }
    }

    [EnableInBuilder(BuilderType.DVBuilder, BuilderType.Calendar)]
    public class DateCondition : ColumnCondition
    {
        public DateCondition() { }

        [EnableInBuilder(BuilderType.DVBuilder, BuilderType.Calendar)]
        [OnChangeExec(@"
if(this.Operator === 5)
    pg.ShowProperty('Value1');
else
    pg.HideProperty('Value1');
    
")]
        public NumericOperators Operator { get; set; }
        [DefaultPropValue("")]
        [EnableInBuilder(BuilderType.DVBuilder, BuilderType.Calendar)]
        public DateTime Value { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder, BuilderType.Calendar)]
        public DateTime Value1 { get; set; }

        public override bool CompareValues(object _unformattedData)
        {
            if (this.Operator == NumericOperators.Equals)
                return Convert.ToDateTime(_unformattedData) == this.Value;
            else if (this.Operator == NumericOperators.LessThan)
                return Convert.ToDateTime(_unformattedData) < this.Value;
            else if (this.Operator == NumericOperators.GreaterThan)
                return Convert.ToDateTime(_unformattedData) > this.Value;
            else if (this.Operator == NumericOperators.LessThanOrEqual)
                return Convert.ToDateTime(_unformattedData) <= this.Value;
            else if (this.Operator == NumericOperators.GreaterThanOrEqual)
                return Convert.ToDateTime(_unformattedData) >= this.Value;
            else if (this.Operator == NumericOperators.Between)
                return (Convert.ToDateTime(_unformattedData) >= this.Value) && (Convert.ToDateTime(_unformattedData) <= Convert.ToDateTime(this.Value1));

            return false;
        }
    }

    [EnableInBuilder(BuilderType.DVBuilder, BuilderType.Calendar)]
    public class AdvancedCondition : ColumnCondition
    {
        public AdvancedCondition() { }

        [EnableInBuilder(BuilderType.DVBuilder, BuilderType.Calendar)]
        public AdvancedBooleanOperators Operator { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder, BuilderType.Calendar)]
        [PropertyEditor(PropertyEditorType.ScriptEditorCS)]
        public EbScript Value { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder, BuilderType.Calendar)]
        public AdvancedRenderType RenderAS { get; set; }
        

        [JsonIgnore]
        private List<string> __formulaDataFieldsUsed = null;

        [JsonIgnore]
        public List<string> FormulaDataFieldsUsed
        {
            get
            {
                if (__formulaDataFieldsUsed == null)
                {
                    var matches = Regex.Matches(this.Value.Code, @"T[0-9]{1}.\w+").OfType<Match>().Select(m => m.Groups[0].Value).Distinct();
                    __formulaDataFieldsUsed = new List<string>(matches.Count());
                    int j = 0;
                    foreach (var match in matches)
                        __formulaDataFieldsUsed.Add(match);
                }

                return __formulaDataFieldsUsed;
            }
        }

        [JsonIgnore]
        private List<FormulaPart> __formulaParts = new List<FormulaPart>();
        [JsonIgnore]
        public List<FormulaPart> FormulaParts
        {
            get
            {
                if (__formulaParts.Count == 0)
                {
                    foreach (string calcfd in this.FormulaDataFieldsUsed)
                    {
                        string[] splits = calcfd.Split('.');
                        __formulaParts.Add(new FormulaPart { TableName = splits[0], FieldName = splits[1] });
                    }
                }

                return __formulaParts;
            }
        }

        [JsonIgnore]
        private Script __codeAnalysisScript = null;

        public Script GetCodeAnalysisScript()
        {
            if (__codeAnalysisScript == null && !string.IsNullOrEmpty(this.Value.Code))
            {
                __codeAnalysisScript = CSharpScript.Create<dynamic>(this.Value.Code, ScriptOptions.Default.WithReferences("Microsoft.CSharp", "System.Core").WithImports("System.Dynamic", "System", "System.Collections.Generic", "System.Diagnostics", "System.Linq"), globalsType: typeof(Globals));
                __codeAnalysisScript.Compile();
            }

            return __codeAnalysisScript;
        }

        public bool EvaluateExpression(EbDataRow _datarow, ref Globals globals)
        {
            foreach (FormulaPart formulaPart in this.FormulaParts)
            {
                object __value = null;
                var __partType = _datarow.Table.Columns[formulaPart.FieldName].Type;
                if (__partType == EbDbTypes.Decimal || __partType == EbDbTypes.Int32)
                    __value = (_datarow[formulaPart.FieldName] != DBNull.Value) ? _datarow[formulaPart.FieldName] : 0;
                else
                    __value = _datarow[formulaPart.FieldName];

                globals[formulaPart.TableName].Add(formulaPart.FieldName, new NTV
                {
                    Name = formulaPart.FieldName,
                    Type = __partType,
                    Value = __value
                });
            }
            return Convert.ToBoolean(this.GetCodeAnalysisScript().RunAsync(globals).Result.ReturnValue);
        }

        public bool GetBoolValue()
        {
            if(this.Operator == AdvancedBooleanOperators.IsTrue)
                return true;
            else if(this.Operator == AdvancedBooleanOperators.IsFalse)
                return false;
            else
                return false;//for null
        }

        public override bool CompareValues(object _unformattedData)
        {           
            return false;
        }
    }

    [EnableInBuilder(BuilderType.DVBuilder)]
    public class ControlClass : EbDataVisualizationObject
    {
        [EnableInBuilder(BuilderType.DVBuilder)]
        [HideInPropertyGrid]
        public string DataSourceId { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder)]
        [HideInPropertyGrid]
        public List<DVBaseColumn> DisplayMember { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder)]
        [HideInPropertyGrid]
        public DVBaseColumn ValueMember { get; set; }

        [JsonIgnore]
        public Dictionary<int, string> Values { get; set; }

        public ControlClass()
        {
            this.DisplayMember = new List<DVBaseColumn>();
            this.ValueMember = new DVBaseColumn();
            this.Values = new Dictionary<int, string>();

        }
    }

}
