using ExpressBase.Common;
using ExpressBase.Common.Data;
using ExpressBase.Common.JsonConverters;
using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using ExpressBase.Common.Structures;
using ExpressBase.Objects.ServiceStack_Artifacts;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using Newtonsoft.Json;
using System.Collections.Generic;
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
        Tree = 6
    }

    public enum NumericRenderType
    {
        Default,
        ProgressBar,
        Link,
        Tree
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


    [EnableInBuilder(BuilderType.DVBuilder, BuilderType.WebForm, BuilderType.BotForm, BuilderType.FilterDialog, BuilderType.UserControl)]
    [HideInPropertyGrid]
    public class DVBaseColumn : EbDataVisualizationObject
    {
        [JsonProperty(PropertyName = "data")]
        [EnableInBuilder(BuilderType.DVBuilder, BuilderType.WebForm, BuilderType.BotForm, BuilderType.FilterDialog, BuilderType.UserControl)]
        [HideInPropertyGrid]
        public int Data { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder, BuilderType.WebForm, BuilderType.BotForm, BuilderType.FilterDialog, BuilderType.UserControl)]
        [JsonProperty(PropertyName = "name")]
        [PropertyEditor(PropertyEditorType.Label)]
        [OnChangeExec(@"this.Name = this.name;
        if (this.IsCustomColumn){
            pg.MakeReadWrite('name');// [JsonProperty(PropertyName = 'name')]
        }
        else {
            pg.MakeReadOnly('name');
        }")]
        public override string Name { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder, BuilderType.WebForm, BuilderType.BotForm, BuilderType.FilterDialog, BuilderType.UserControl)]
        [HideInPropertyGrid]
        public string EbSid { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder, BuilderType.WebForm, BuilderType.BotForm, BuilderType.FilterDialog, BuilderType.UserControl)]
        [HideInPropertyGrid]
        public virtual EbDbTypes Type { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder, BuilderType.BotForm)]
        [HideInPropertyGrid]
        public string sType { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder, BuilderType.WebForm, BuilderType.BotForm, BuilderType.FilterDialog, BuilderType.UserControl)]
        [Alias("Title")]
        public string sTitle { get; set; }

        [HideInPropertyGrid]
        [EnableInBuilder(BuilderType.DVBuilder, BuilderType.WebForm, BuilderType.BotForm, BuilderType.FilterDialog, BuilderType.UserControl)]
        public bool bVisible { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder, BuilderType.WebForm, BuilderType.BotForm, BuilderType.FilterDialog, BuilderType.UserControl)]
        [HideInPropertyGrid]
        public int Pos { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder, BuilderType.BotForm)]
        [Alias("Width")]
        [JsonIgnore]
        public string sWidth { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder, BuilderType.WebForm, BuilderType.BotForm, BuilderType.FilterDialog, BuilderType.UserControl)]
        [JsonProperty(PropertyName = "className")]
        [HideInPropertyGrid]
        [DefaultPropValue("tdheight")]
        public string ClassName { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder, BuilderType.BotForm)]
        [PropertyEditor(PropertyEditorType.FontSelector)]
        public EbFont Font { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder, BuilderType.BotForm)]
        [PropertyEditor(PropertyEditorType.DropDown)]
        public ControlType FilterControl { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder, BuilderType.BotForm)]
        [PropertyEditor(PropertyEditorType.ScriptEditorCS)]
        [Alias("Formula")]        
        public EbScript _Formula { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder, BuilderType.BotForm)]
        [PropertyEditor(PropertyEditorType.ScriptEditorCS)]
        [JsonConverter(typeof(Base64Converter))]
        [Alias("Formula old")]
        [HideInPropertyGrid]
        public string Formula { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder, BuilderType.BotForm)]
        [HideInPropertyGrid]
        [OnChangeExec(@"
console.log('IsCustomColumn');
if(this.IsCustomColumn){
    pg.ShowProperty('_Formula');
}
else {
    pg.HideProperty('_Formula');
}")]
        public bool IsCustomColumn { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder, BuilderType.BotForm)]
        [PropertyEditor(PropertyEditorType.ObjectSelector)]
        [OSE_ObjectTypes(EbObjectTypes.iTableVisualization, EbObjectTypes.iChartVisualization, EbObjectTypes.iReport, EbObjectTypes.iWebForm)]
        [OnChangeExec(@"
pg.HideProperty('ParentColumn');
    pg.HideProperty('GroupingColumn');
    pg.HideProperty('GroupFormLink');
    pg.HideProperty('ItemFormLink');
pg.HideProperty('GroupFormParameters');
    pg.HideProperty('GroupFormId');
    pg.HideProperty('ItemFormParameters');
    pg.HideProperty('ItemFormId');
if(this.LinkRefId !== null){
    if(this.LinkRefId.split('-')[2] === '0'){
        pg.ShowProperty('FormMode');
    pg.HideProperty('FormParameters');
    pg.HideProperty('FormId');
    }
    else{
        pg.HideProperty('FormMode');
 pg.HideProperty('FormParameters');
    pg.HideProperty('FormId');
    }
}
else{
    pg.HideProperty('FormMode');
 pg.HideProperty('FormParameters');
    pg.HideProperty('FormId');
}")]
        public string LinkRefId { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder)]
        [PropertyEditor(PropertyEditorType.DropDown)]
        [OnChangeExec(@"
console.log('kkkoiiii');
if(this.FormMode === 1){
    pg.ShowProperty('FormId');
    pg.HideProperty('FormParameters');
}
else if(this.FormMode === 2){
    pg.HideProperty('FormId');
    pg.ShowProperty('FormParameters');
}")]
        public WebFormDVModes FormMode { get; set; }

        //[EnableInBuilder(BuilderType.DVBuilder)]
        [HideInPropertyGrid]
        [JsonIgnore]
        public DVColumnCollection ColumnsRef { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder)]
        [PropertyEditor(PropertyEditorType.CollectionFrmSrc, "ColumnsRef")]
        public List<DVBaseColumn> FormId { get; set; }


        [EnableInBuilder(BuilderType.DVBuilder)]
        //[PropertyEditor(PropertyEditorType.CollectionFrmSrc, "ColumnsRef")]
        [PropertyEditor(PropertyEditorType.Mapper, "ColumnsRef", "LinkRefId", "FormControl")]
        public List<DVBaseColumn> FormParameters { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder)]
        [HideInPropertyGrid]
        public EbControl FormControl { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder, BuilderType.BotForm)]
        [PropertyEditor(PropertyEditorType.DropDown)]
        public LinkTypeEnum LinkType { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder,  BuilderType.BotForm)]
        [DefaultPropValue("0")]
        [HideForUser]
        public int HideDataRowMoreThan { get; set; }

        [PropertyGroup("TreeVisualization")]
        [EnableInBuilder(BuilderType.DVBuilder)]
        [PropertyEditor(PropertyEditorType.CollectionFrmSrc, "ColumnsRef")]
        public List<DVBaseColumn> ParentColumn { get; set; }

        [PropertyGroup("TreeVisualization")]
        [EnableInBuilder(BuilderType.DVBuilder)]
        [PropertyEditor(PropertyEditorType.CollectionFrmSrc, "ColumnsRef")]
        public List<DVBaseColumn> GroupingColumn { get; set; }

        [PropertyGroup("TreeVisualization")]
        [EnableInBuilder(BuilderType.DVBuilder, BuilderType.BotForm)]
        [PropertyEditor(PropertyEditorType.ObjectSelector)]
        [OSE_ObjectTypes(EbObjectTypes.iWebForm)]
        public string GroupFormLink { get; set; }

        [PropertyGroup("TreeVisualization")]
        [EnableInBuilder(BuilderType.DVBuilder, BuilderType.BotForm)]
        [PropertyEditor(PropertyEditorType.ObjectSelector)]
        [OSE_ObjectTypes(EbObjectTypes.iWebForm)]
        public string ItemFormLink { get; set; }

        [PropertyGroup("TreeVisualization")]
        [EnableInBuilder(BuilderType.DVBuilder)]
        [PropertyEditor(PropertyEditorType.CollectionFrmSrc, "ColumnsRef")]
        public List<DVBaseColumn> GroupFormId { get; set; }

        [PropertyGroup("TreeVisualization")]
        [EnableInBuilder(BuilderType.DVBuilder)]
        [PropertyEditor(PropertyEditorType.Mapper, "ColumnsRef", "GroupFormLink", "FormControl")]
        public List<DVBaseColumn> GroupFormParameters { get; set; }

        [PropertyGroup("TreeVisualization")]
        [EnableInBuilder(BuilderType.DVBuilder)]
        [PropertyEditor(PropertyEditorType.CollectionFrmSrc, "ColumnsRef")]
        public List<DVBaseColumn> ItemFormId { get; set; }

        [PropertyGroup("TreeVisualization")]
        [EnableInBuilder(BuilderType.DVBuilder)]
        [PropertyEditor(PropertyEditorType.Mapper, "ColumnsRef", "ItemFormLink", "FormControl")]
        public List<DVBaseColumn> ItemFormParameters { get; set; }

        [PropertyGroup("TreeVisualization")]
        [EnableInBuilder(BuilderType.DVBuilder, BuilderType.BotForm)]
        [HideInPropertyGrid]
        public bool IsTree { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder, BuilderType.BotForm)]
        [PropertyEditor(PropertyEditorType.Collection)]
        public List<StaticParam> StaticParameters { get; set; }

        [PropertyGroup("Tooltip")]
        [EnableInBuilder(BuilderType.DVBuilder)]
        [OnChangeExec(@"
if(this.AllowTooltip){
    pg.ShowProperty('AllowedCharacterLength');
}
else {
    pg.HideProperty('AllowedCharacterLength');
}")]
        public bool AllowTooltip { get; set; }

        [PropertyGroup("Tooltip")]
        [EnableInBuilder(BuilderType.DVBuilder)]        
        public int AllowedCharacterLength { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder)]
        public OrderByDirection Direction { get; set; }

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
                if (__formulaParts .Count == 0)
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

        //[JsonIgnore]
        //private Script __codeAnalysisScript = null;
        //[JsonIgnore]
        //public Script CodeAnalysisScript
        //{
        //    get
        //    {
        //        if (__codeAnalysisScript == null)
        //        {
        //            __codeAnalysisScript = CSharpScript.Create<dynamic>(this.Formula, ScriptOptions.Default.WithReferences("Microsoft.CSharp", "System.Core").WithImports("System.Dynamic"), globalsType: typeof(Globals));
        //            __codeAnalysisScript.Compile();
        //        }

        //        return __codeAnalysisScript;
        //    }
        //}

        [JsonIgnore]
        private Script __codeAnalysisScript = null;

        public Script GetCodeAnalysisScript()
        {
            if (__codeAnalysisScript == null && !string.IsNullOrEmpty(this._Formula.Code))
            {
                __codeAnalysisScript = CSharpScript.Create<dynamic>(this._Formula.Code, ScriptOptions.Default.WithReferences("Microsoft.CSharp", "System.Core").WithImports("System.Dynamic"), globalsType: typeof(Globals));
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

        public DVBaseColumn()
        {
            this.StaticParameters = new List<StaticParam>();
            this.FormParameters = new List<DVBaseColumn>();
            this.FormId = new List<DVBaseColumn>();
            this.ParentColumn = new List<DVBaseColumn>();
            this.GroupingColumn = new List<DVBaseColumn>();
            this.GroupFormParameters = new List<DVBaseColumn>();
            this.GroupFormId = new List<DVBaseColumn>();
            this.ItemFormParameters = new List<DVBaseColumn>();
            this.ItemFormId = new List<DVBaseColumn>();
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
    }
    

    [EnableInBuilder(BuilderType.DVBuilder, BuilderType.WebForm, BuilderType.BotForm, BuilderType.FilterDialog, BuilderType.UserControl)]
    [Alias("DVStringColumnAlias")]
    public class DVStringColumn : DVBaseColumn
    {
        [EnableInBuilder(BuilderType.DVBuilder, BuilderType.BotForm)]
        [DefaultPropValue("0")]
        [PropertyEditor(PropertyEditorType.DropDown)]
        [OnChangeExec(@"
if(this.RenderAs === 2){
    pg.ShowProperty('LinkRefId');
    pg.ShowProperty('LinkType');
    pg.HideProperty('ParentColumn');
    pg.HideProperty('GroupingColumn');
    pg.HideProperty('GroupFormLink');
    pg.HideProperty('ItemFormLink');
    pg.HideProperty('GroupFormParameters');
    pg.HideProperty('GroupFormId');
    pg.HideProperty('ItemFormParameters');
    pg.HideProperty('ItemFormId');
    pg.setSimpleProperty('IsTree', false);
}
else if(this.RenderAs === 6){
    pg.ShowProperty('ParentColumn');
    pg.ShowProperty('GroupingColumn');
    pg.ShowProperty('GroupFormLink');
    pg.ShowProperty('ItemFormLink');
    pg.ShowProperty('GroupFormParameters');
    pg.ShowProperty('GroupFormId');
    pg.ShowProperty('ItemFormParameters');
    pg.ShowProperty('ItemFormId');
    pg.setSimpleProperty('IsTree', true);
    pg.HideProperty('LinkRefId');
    pg.ShowProperty('LinkType');
}
else{
    pg.HideProperty('LinkRefId');
    pg.HideProperty('LinkType');
    pg.setSimpleProperty('LinkRefId', null);
    pg.HideProperty('ParentColumn');
    pg.HideProperty('GroupingColumn');
    pg.HideProperty('GroupFormLink');
    pg.HideProperty('ItemFormLink');
    pg.HideProperty('GroupFormParameters');
    pg.HideProperty('GroupFormId');
    pg.HideProperty('ItemFormParameters');
    pg.HideProperty('ItemFormId');
    pg.setSimpleProperty('IsTree', false);
    }")]
        public StringRenderType RenderAs { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder, BuilderType.WebForm, BuilderType.BotForm, BuilderType.FilterDialog, BuilderType.UserControl)]
        [DefaultPropValue("16")]
        [HideInPropertyGrid]
        public override EbDbTypes Type { get; set; }


        [EnableInBuilder(BuilderType.DVBuilder, BuilderType.WebForm, BuilderType.BotForm, BuilderType.FilterDialog, BuilderType.UserControl)]
        [PropertyEditor(PropertyEditorType.DropDown)]
        public StringOperators DefaultOperator { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder, BuilderType.BotForm)]
        [PropertyEditor(PropertyEditorType.DropDown)]
        public Align Align { get; set; }
        
    }

    [EnableInBuilder(BuilderType.DVBuilder, BuilderType.WebForm, BuilderType.BotForm, BuilderType.FilterDialog, BuilderType.UserControl)]
    public class DVNumericColumn : DVBaseColumn
    {
        [EnableInBuilder(BuilderType.DVBuilder, BuilderType.BotForm)]
        public bool Aggregate { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder, BuilderType.BotForm)]
        public int DecimalPlaces { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder, BuilderType.BotForm)]
        [OnChangeExec(@"
if(this.RenderAs === 2){
    pg.ShowProperty('LinkRefId');
    pg.ShowProperty('LinkType');
    pg.HideProperty('ParentColumn');
    pg.HideProperty('GroupingColumn');
    pg.HideProperty('GroupFormLink');
    pg.HideProperty('ItemFormLink');
    pg.HideProperty('GroupFormParameters');
    pg.HideProperty('GroupFormId');
    pg.HideProperty('ItemFormParameters');
    pg.HideProperty('ItemFormId');
    pg.setSimpleProperty('IsTree', false);
}
else if(this.RenderAs === 3){
    pg.ShowProperty('ParentColumn');
    pg.ShowProperty('GroupingColumn');
    pg.ShowProperty('GroupFormLink');
    pg.ShowProperty('ItemFormLink');
    pg.ShowProperty('GroupFormParameters');
    pg.ShowProperty('GroupFormId');
    pg.ShowProperty('ItemFormParameters');
    pg.ShowProperty('ItemFormId');
    pg.setSimpleProperty('IsTree', true);
    pg.HideProperty('LinkRefId');
    pg.ShowProperty('LinkType');
}
else{
    pg.HideProperty('LinkRefId');
    pg.HideProperty('LinkType');
    pg.setSimpleProperty('LinkRefId', null);
    pg.HideProperty('ParentColumn');
    pg.HideProperty('GroupingColumn');
    pg.HideProperty('GroupFormLink');
    pg.HideProperty('ItemFormLink');
    pg.HideProperty('GroupFormParameters');
    pg.HideProperty('GroupFormId');
    pg.HideProperty('ItemFormParameters');
    pg.HideProperty('ItemFormId');
pg.setSimpleProperty('IsTree', false);
    }")]
        public NumericRenderType RenderAs { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder, BuilderType.WebForm, BuilderType.BotForm, BuilderType.FilterDialog, BuilderType.UserControl)]
        [DefaultPropValue("7")]
        [HideInPropertyGrid]
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

        [EnableInBuilder(BuilderType.DVBuilder, BuilderType.WebForm, BuilderType.BotForm, BuilderType.FilterDialog, BuilderType.UserControl)]
        [PropertyEditor(PropertyEditorType.DropDown)]
        public NumericOperators DefaultOperator { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder, BuilderType.BotForm)]
        public bool SuppresIfZero { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder, BuilderType.BotForm)]
        [PropertyEditor(PropertyEditorType.DropDown)]
        public Align Align { get; set; }

        
    }

    [EnableInBuilder(BuilderType.DVBuilder, BuilderType.WebForm, BuilderType.BotForm, BuilderType.FilterDialog, BuilderType.UserControl)]
    public class DVBooleanColumn : DVBaseColumn
    {
        [EnableInBuilder(BuilderType.DVBuilder, BuilderType.BotForm)]
        [OnChangeExec(@"
if(this.RenderAs === 2){
    pg.ShowProperty('LinkRefId');
    pg.ShowProperty('LinkType');
    pg.HideProperty('ParentColumn');
    pg.HideProperty('GroupingColumn');
    pg.HideProperty('GroupFormLink');
    pg.HideProperty('ItemFormLink');
    pg.HideProperty('GroupFormParameters');
    pg.HideProperty('GroupFormId');
    pg.HideProperty('ItemFormParameters');
    pg.HideProperty('ItemFormId');
    pg.setSimpleProperty('IsTree', false);
}
else if(this.RenderAs === 4){
    pg.ShowProperty('ParentColumn');
    pg.ShowProperty('GroupingColumn');
    pg.ShowProperty('GroupFormLink');
    pg.ShowProperty('ItemFormLink');
    pg.ShowProperty('GroupFormParameters');
    pg.ShowProperty('GroupFormId');
    pg.ShowProperty('ItemFormParameters');
    pg.ShowProperty('ItemFormId');
    pg.setSimpleProperty('IsTree', true);
    pg.HideProperty('LinkRefId');
    pg.ShowProperty('LinkType');
}
else{
    pg.HideProperty('LinkRefId');
    pg.HideProperty('LinkType');
    pg.setSimpleProperty('LinkRefId', null);
    pg.HideProperty('ParentColumn');
    pg.HideProperty('GroupingColumn');
    pg.HideProperty('GroupFormLink');
    pg.HideProperty('ItemFormLink');
pg.HideProperty('GroupFormParameters');
    pg.HideProperty('GroupFormId');
    pg.HideProperty('ItemFormParameters');
    pg.HideProperty('ItemFormId');
pg.setSimpleProperty('IsTree', false);
    }")]
        public BooleanRenderType RenderAs { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder, BuilderType.BotForm)]
        [PropertyEditor(PropertyEditorType.DropDown)]
        public Align Align { get; set; }

        
    }

    [EnableInBuilder(BuilderType.DVBuilder, BuilderType.WebForm, BuilderType.BotForm, BuilderType.FilterDialog, BuilderType.UserControl)]
    public class DVDateTimeColumn : DVBaseColumn
    {
        [EnableInBuilder(BuilderType.DVBuilder, BuilderType.BotForm)]
        [OnChangeExec(@"
if(this.Format === 3){
    pg.ShowProperty('ConvretToUsersTimeZone');
}
else{
    pg.HideProperty('ConvretToUsersTimeZone');
    }")]
        public DateFormat Format { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder, BuilderType.BotForm)]
        public bool ConvretToUsersTimeZone { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder, BuilderType.BotForm)]
        [OnChangeExec(@"
if(this.RenderAs === 1){
    pg.ShowProperty('LinkRefId');
    pg.ShowProperty('LinkType');
    pg.HideProperty('ParentColumn');
    pg.HideProperty('GroupingColumn');
    pg.HideProperty('GroupFormLink');
    pg.HideProperty('ItemFormLink');
    pg.HideProperty('GroupFormParameters');
    pg.HideProperty('GroupFormId');
    pg.HideProperty('ItemFormParameters');
    pg.HideProperty('ItemFormId');
    pg.setSimpleProperty('IsTree', false);
}
else if(this.RenderAs === 2){
    pg.ShowProperty('ParentColumn');
    pg.ShowProperty('GroupingColumn');
    pg.ShowProperty('GroupFormLink');
    pg.ShowProperty('ItemFormLink');
    pg.ShowProperty('GroupFormParameters');
    pg.ShowProperty('GroupFormId');
    pg.ShowProperty('ItemFormParameters');
    pg.ShowProperty('ItemFormId');
    pg.setSimpleProperty('IsTree', true);
    pg.HideProperty('LinkRefId');
    pg.ShowProperty('LinkType');
}
else{
    pg.HideProperty('LinkRefId');
    pg.HideProperty('LinkType');
    pg.setSimpleProperty('LinkRefId', null);
    pg.HideProperty('ParentColumn');
    pg.HideProperty('GroupingColumn');
    pg.HideProperty('GroupFormLink');
    pg.HideProperty('ItemFormLink');
pg.HideProperty('GroupFormParameters');
    pg.HideProperty('GroupFormId');
    pg.HideProperty('ItemFormParameters');
    pg.HideProperty('ItemFormId');
pg.setSimpleProperty('IsTree', false);
    }")]
        public DateTimeRenderType RenderAs { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder, BuilderType.WebForm, BuilderType.BotForm, BuilderType.FilterDialog, BuilderType.UserControl)]
        [DefaultPropValue("5")]
        [HideInPropertyGrid]
        public override EbDbTypes Type { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder, BuilderType.BotForm)]
        [PropertyEditor(PropertyEditorType.DropDown)]
        public NumericOperators DefaultOperator { get; set; }

        [EnableInBuilder(BuilderType.DVBuilder, BuilderType.BotForm)]
        [PropertyEditor(PropertyEditorType.DropDown)]
        public Align Align { get; set; }

        
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
    }
}
