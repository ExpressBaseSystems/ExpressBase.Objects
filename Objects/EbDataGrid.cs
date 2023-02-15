using ExpressBase.Common;
using ExpressBase.Common.Extensions;
using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using ExpressBase.Common.Structures;
using ExpressBase.Objects.Objects.DVRelated;
using Newtonsoft.Json;
using ServiceStack;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Runtime.Serialization;
using ExpressBase.Security;
using ServiceStack.Redis;
using ExpressBase.Common.Data;
using System.ComponentModel;
using ExpressBase.Common.LocationNSolution;
using ExpressBase.Common.Constants;

namespace ExpressBase.Objects
{
    [EnableInBuilder(BuilderType.WebForm, BuilderType.UserControl)]
    public class EbDataGrid : EbControlContainer, IEbSpecialContainer
    {
        public EbDataGrid()
        {
            this.Controls = new List<EbControl>();
            this.Validators = new List<EbValidator>();
        }

        public override string UIchangeFns
        {
            get
            {
                return @"EbDataGrid = {
                title : function(elementId, props) {
                    $(`[ebsid=${elementId}]th .eb-label-editable`).text(props.Title);
                }
            }";
            }
        }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.UserControl)]
        [DefaultPropValue("200")]
        [PropertyGroup(PGConstants.APPEARANCE)]
        public override int Height { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.UserControl)]
        [DefaultPropValue("0")]
        [Alias("Left fixed column count")]
        [PropertyGroup(PGConstants.BEHAVIOR)]
        public int LFxdColCount { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.UserControl)]
        [DefaultPropValue("0")]
        [Alias("Right fixed column count")]
        [PropertyGroup(PGConstants.BEHAVIOR)]
        public int RFxdColCount { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.UserControl)]
        [HideInPropertyGrid]
        public int TableWidth
        {
            get
            {
                int tblwidth = 0;
                for (int i = 0; i < this.Controls.Count; i++)
                {
                    tblwidth += (this.Controls[i] as EbDGColumn).Width;
                }
                return tblwidth;
            }
            set { }
        }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl)]
        [PropertyEditor(PropertyEditorType.Collection)]
        public new List<EbValidator> Validators { get; set; }

        //[EnableInBuilder(BuilderType.WebForm, BuilderType.UserControl)]
        //[PropertyEditor(PropertyEditorType.ScriptEditorJS)]
        //[Alias("ReadOnly Expression")]
        //[PropertyGroup(PGConstants.BEHAVIOR)]
        //[HelpText("Define conditions to decide Disabled/Readonly property of the control.")]
        //public new EbScript DisableExpr { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.UserControl)]
        [DefaultPropValue("true")]
        [PropertyGroup(PGConstants.BEHAVIOR)]
        [Alias("Serial numbered")]
        public bool IsShowSerialNumber { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.UserControl)]
        [OSE_ObjectTypes(EbObjectTypes.iDataReader)]
        [PropertyGroup(PGConstants.BEHAVIOR)]
        [PropertyEditor(PropertyEditorType.ObjectSelector)]
        [OnChangeExec(@"
                if (this.DataSourceId){
                    pg.ShowProperty('IsLoadDataSourceInEditMode');
                    pg.ShowProperty('IsLoadDataSourceAlways');
                }
                else {
                    pg.HideProperty('IsLoadDataSourceInEditMode');
                    pg.HideProperty('IsLoadDataSourceAlways');
                }
            ")]
        public string DataSourceId { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.UserControl)]
        [Alias("Load datasource in edit mode ")]
        public bool IsLoadDataSourceInEditMode { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.UserControl)]
        [Alias("Load datasource always ")]
        public bool IsLoadDataSourceAlways { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.UserControl)]
        [Alias("Load ds new mode onload always")]
        public bool IsLoadDsNewModeOnloadAlways { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.UserControl)]
        [Alias("Merge imported data")]
        public bool MergeData { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.UserControl)]
        [DefaultPropValue("true")]
        [PropertyGroup(PGConstants.BEHAVIOR)]
        [Alias("Resizable Columns")]
        public bool IsColumnsResizable { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.UserControl)]
        [PropertyGroup(PGConstants.APPEARANCE)]
        [DefaultPropValue("+ Row")]
        [Alias("AddRow Button Text")]
        public string AddRowBtnTxt { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.UserControl)]
        [PropertyGroup(PGConstants.APPEARANCE)]
        public bool DeferRender { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.UserControl)]
        [PropertyGroup(PGConstants.BEHAVIOR)]
        [DefaultPropValue("true")]
        public bool AscendingOrder { get; set; }

        [HideInPropertyGrid]
        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl)]
        public override EbScript OnChangeFn { get; set; }


        [PropertyGroup("Events")]
        [EnableInBuilder(BuilderType.WebForm, BuilderType.UserControl)]
        [PropertyEditor(PropertyEditorType.ScriptEditorJS)]
        [HelpText("Define actions to do after a datagrid row painted on screen.")]
        public EbScript OnRowPaint { get; set; }

        [PropertyGroup("Data")]
        [EnableInBuilder(BuilderType.WebForm)]
        [PropertyEditor(PropertyEditorType.ScriptEditorJS)]
        public EbScript PersistRowOnlyIf { get; set; }

        [PropertyGroup("Data")]
        [EnableInBuilder(BuilderType.WebForm, BuilderType.UserControl)]
        [OSE_ObjectTypes(EbObjectTypes.iDataReader)]
        [PropertyEditor(PropertyEditorType.ObjectSelector)]
        public string CustomSelectDS { get; set; }

        [JsonIgnore]
        public string CustomSelectDSQuery { get; set; }

        [JsonIgnore]
        public override string OnChangeBindJSFn
        {
            get
            {
                return @"dgEBOnChangeBind.bind(this)();
                        dgOnChangeBind.bind(this)()";
            }
            set { }
        }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.UserControl)]
        [HideInPropertyGrid]
        public override bool IsSpecialContainer { get { return true; } set { } }

        [OnDeserialized]
        public new void OnDeserializedMethod(StreamingContext context)
        {
            if (this.OnRowPaint == null)
                this.OnRowPaint = new EbScript();
            //if (this.DisableExpr == null)
            //    this.DisableExpr = new EbScript();

            this.BareControlHtml = this.GetBareHtml();
            this.ObjType = this.GetType().Name.Substring(2, this.GetType().Name.Length - 2);
            foreach (EbControl contol in Controls)
            {
                if (contol is EbDGPowerSelectColumn)
                {
                    EbDGPowerSelectColumn DGPowerSelectColumn = (contol as EbDGPowerSelectColumn);
                    DGPowerSelectColumn.DBareHtml = DGPowerSelectColumn.GetBareHtml();
                }
            }
        }

        [JsonIgnore]
        public override UISides Padding { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.UserControl)]
        [PropertyEditor(PropertyEditorType.Collection)]
        [Alias("Columns")]
        [PropertyGroup(PGConstants.BEHAVIOR)]
        [ListType(typeof(EbDGColumn))]
        [PropertyPriority(99)]
        public override List<EbControl> Controls { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.UserControl)]
        [HideInPropertyGrid]
        public List<EbDGColumn> LeftFixedCols { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.UserControl)]
        [HideInPropertyGrid]
        public List<EbDGColumn> RightFixedCols { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.UserControl)]
        [HideInPropertyGrid]
        public List<EbDGColumn> FlexibleCols { get; set; }

        public void SetCols()
        {
            this.FlexibleCols = new List<EbDGColumn>(this.Controls.Cast<EbDGColumn>().ToList());
            this.LeftFixedCols = this.FlexibleCols.Cast<object>().ToList().PopRange(0, this.LFxdColCount).Cast<EbDGColumn>().ToList();
            this.RightFixedCols = this.FlexibleCols.Cast<object>().ToList().PopRange(this.FlexibleCols.Count - this.RFxdColCount, this.FlexibleCols.Count).Cast<EbDGColumn>().ToList();

            this.FlexibleCols.RemoveRange(0, this.LFxdColCount);
            this.FlexibleCols.RemoveRange(this.FlexibleCols.Count - this.RFxdColCount, this.RFxdColCount);
        }

        //[EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.UserControl)]
        //[PropertyGroup(PGConstants.BEHAVIOR)]
        //[DefaultPropValue("true")]
        //public bool IsEditable { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.UserControl)]
        [PropertyGroup(PGConstants.BEHAVIOR)]
        [PropertyPriority(98)]
        [DefaultPropValue("true")]
        public bool IsAddable { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.UserControl)]
        [PropertyGroup(PGConstants.BEHAVIOR)]
        [PropertyPriority(98)]
        public bool EnableExcelUpload { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.UserControl)]
        [PropertyGroup(PGConstants.BEHAVIOR)]
        [PropertyPriority(98)]
        [Alias("Show Refresh Button")]
        public bool ShowRefreshBtn { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.UserControl)]
        [PropertyGroup(PGConstants.BEHAVIOR)]
        [OnChangeExec(@"
if (this.IsDisable){
    pg.HideProperty('DisableRowEdit');
    pg.HideProperty('DisableRowDelete');
}
else {
    pg.ShowProperty('DisableRowEdit');
    pg.ShowProperty('DisableRowDelete');
}")]
        public override bool IsDisable { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.UserControl)]
        [PropertyGroup(PGConstants.BEHAVIOR)]
        public bool DisableRowEdit { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.UserControl)]
        [PropertyGroup(PGConstants.BEHAVIOR)]
        public bool DisableRowDelete { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl)]
        [PropertyGroup(PGConstants.BEHAVIOR)]
        [PropertyPriority(99)]
        [HelpText("Set true if you want to hide the control.")]
        public override bool Hidden { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.UserControl)]
        public override bool DoNotPersist { get; set; }

        [HideInPropertyGrid]
        [JsonIgnore]
        public override string ToolIconHtml { get { return "<i class='fa fa-table'></i>"; } set { } }

        //public override string GetToolHtml()
        //{
        //    return @"<div eb-type='@toolName' class='tool'><i class='fa fa-table'></i>  @toolName</div>".Replace("@toolName", this.GetType().Name.Substring(2));
        //}

        public void InitDSRelated(IServiceClient serviceClient, IRedisClient redis, EbControl[] Allctrls, Service service)
        {
            List<string> _params = new List<string>();
            EbDataReader DataReader = EbFormHelper.GetEbObject<EbDataReader>(this.DataSourceId, serviceClient, redis, service);
            this.ParamsList = DataReader.GetParams(redis as RedisClient);
            foreach (Param p in this.ParamsList)
            {
                _params.Add(p.Name);
                for (int i = 0; i < Allctrls.Length; i++)
                {
                    if (p.Name == Allctrls[i].Name && !Allctrls[i].DependedDG.Contains(this.Name))
                    {
                        Allctrls[i].DependedDG.Add(this.Name);
                    }
                }
            }

            this.Eb__paramControls = _params;
        }

        public void AdjustColumnWidth()
        {
            this.Controls.Where(e => (e as EbDGColumn).Width <= 0).ToList().ForEach(e => (e as EbDGColumn).Width = 10);
            int widthSum = this.Controls.Where(e => !e.Hidden).Select(e => (e as EbDGColumn).Width).Sum();
            if (widthSum == 100)//if sum of width is 100%
                return;
            int perSum = 0;
            EbDGColumn lastCtrl = null;
            foreach (EbDGColumn column in this.Controls)
            {
                if (!column.Hidden)
                {
                    column.Width = column.Width * 100 / widthSum;
                    perSum += column.Width;
                    lastCtrl = column;
                }
            }
            if (lastCtrl != null && perSum < 100)
                lastCtrl.Width = lastCtrl.Width + 100 - perSum;
        }

        //Deprecated
        [EnableInBuilder(BuilderType.WebForm, BuilderType.UserControl)]
        [HideInPropertyGrid]
        public List<string> Eb__paramControls { get; set; }

        [HideInPropertyGrid]
        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl)]
        public List<Param> ParamsList { get; set; }

        public override string GetBareHtml()
        {
            //SetCols();
            int btnright = 8;
            if (this.EnableExcelUpload) btnright += 100;
            if (this.ShowRefreshBtn) btnright += 68;

            string html = @"
<div class='grid-cont'>
    @refreshdgdrbtn@
    @exceluploadbtn@
    @addrowbtn@
<div class='Dg_head'>
        <table id='tbl_@ebsid@_head' class='table table-bordered dgtbl'>
            <thead>
              <tr>  
                <th class='slno' style='width:30px'><span class='grid-col-title'>#</span></th>"
.Replace("@addrowbtn@", this.IsAddable ? ("<div id='@ebsid@addrow' class='addrow-btn' tabindex='0' title='Add Row (Alt+R)' style='right: @addbtnrightstyle@px;'>" + (string.IsNullOrEmpty(AddRowBtnTxt) ? "+ Row" : AddRowBtnTxt) + "</div>") : string.Empty)
.Replace("@exceluploadbtn@", this.EnableExcelUpload ? "<div id='@ebsid@excelupload' class='excelupload-btn' tabindex='0' style='right: @exelbtnrightstyle@px;'>Upload Excel</div>" : string.Empty)
.Replace("@refreshdgdrbtn@", this.ShowRefreshBtn ? "<div id='@ebsid@refreshdgdr' class='refreshdgdr-btn' tabindex='0'>Refresh</div>" : string.Empty)
.Replace("@addbtnrightstyle@", btnright.ToString())
.Replace("@exelbtnrightstyle@", this.ShowRefreshBtn ? "76" : "8");
            EbDGColumn lastCtrl = (EbDGColumn)Controls.FindLast(e => !e.Hidden);
            this.AdjustColumnWidth();
            foreach (EbDGColumn col in Controls)
            {
                if (!col.Hidden)
                    html += string.Concat("<th class='ppbtn-cont ebResizable dg-th' ebsid='@ebsid@' name='@name@' style='width: @Width@; display: inline-block;' @type@ title='", col.Title, @"'>
                                                <span class='grid-col-title eb-label-editable'>", col.Title, @"</span>
                                                <input id='@ebsid@lbltxtb' class='eb-lbltxtb' type='text'/>
                                                <div id='@ebsid@Lblic' tabindex='-1' class='label-infoCont'></div>
                                                @req@ @ppbtn@" +
                                            "</th>")
                        .Replace("@ppbtn@", Common.HtmlConstants.CONT_PROP_BTN)
                        .Replace("@req@", (col.Required ? "<sup style='color: red'>*</sup>" : string.Empty))
                        .Replace("@ebsid@", col.EbSid)
                        .Replace("@name@", col.Name)
                        //.Replace("@Width@", lastCtrl == col ? $"calc({col.Width}% - 60px)" : $"{ col.Width}%")
                        .Replace("@Width@", $"{col.Width}%")
                        .Replace("@type@", "type = '" + col.ObjType + "'");
            }

            html += @"
                <th class='ctrlth' style='width:40px'><span class='fa fa fa-cog'></span></th>
              </tr>
            </thead>
        </table>
    </div>";

            html += @"
    <div class='Dg_body' style='overflow-y:scroll;height:@_height@px ;'>
        <table id='tbl_@ebsid@' class='table table-bordered dgtbl'>
                <tbody>
                </tbody>
            </table>
        </div>
    <div class='Dg_footer'>
        <table id='tbl_@ebsid@_footer' class='table table-bordered dgtbl'>
            <tbody>
            </tbody>
        </table>
     </div>
</div>"
            .Replace("@_height@", this.Height.ToString());

            return html;
        }

        public override string GetDesignHtml()
        {
            return GetHtml().RemoveCR().DoubleQuoted();
        }

        public override string GetHtml()
        {
            string EbCtrlHTML = HtmlConstants.CONTROL_WRAPER_HTML4WEB
               .Replace("@LabelForeColor ", "color:" + (LabelForeColor ?? "@LabelForeColor ") + ";")
               .Replace("@LabelBackColor ", "background-color:" + (LabelBackColor ?? "@LabelBackColor ") + ";");

            return ReplacePropsInHTML(EbCtrlHTML);
        }
    }

    [UsedWithTopObjectParent(typeof(EbObject))]
    [HideInPropertyGrid]
    [HideInToolBox]
    public abstract class EbDGColumn : EbControl
    {
        [JsonIgnore]
        public override string OnChangeBindJSFn { get { return @"$(`[ebsid=${p1.DG.EbSid_CtxId }]`).on('change', `[colname=${this.Name}] [ui-inp]`, p2);"; } set { } }

        [JsonIgnore]
        public override string SetDisplayMemberJSfn
        {
            get { return JSFnsConstants.DG_hiddenColCheckCode + @"
{if(this.DataVals.ObjType === 'DGNumericColumn' && p1 === undefined)
    p1 = 0;
document.getElementById(this.EbSid_CtxId).value = p1;}"; }

            set { }
        }

        [JsonIgnore]
        public override string SetValueJSfn
        {
            get { return SetDisplayMemberJSfn + "$(document.getElementById(this.EbSid_CtxId)).data('ctrl_ref', this).trigger('change');"; }

            set { }
        }

        [JsonIgnore]
        public override string GetValueJSfn
        {
            get
            {
                return @"
                    if(this.__isEditing)
                        return this.curRowDataVals.Value;
                    else
                        return this.DataVals.Value;";

            }

            set { }
        }

        [JsonIgnore]
        public override string GetPreviousValueJSfn
        {
            get
            {
                return @"
                    if(this.__isEditing)
                        return this.curRowDataVals.PrevValue;
                    else
                        return this.DataVals.PrevValue;";

            }

            set { }
        }

        [JsonIgnore]
        public override string GetValueFromDOMJSfn
        {
            get { return @"return document.getElementById(this.EbSid_CtxId).value;"; }

            set { }
        }

        [JsonIgnore]
        public override string GetDisplayMemberFromDOMJSfn
        {
            get { return GetValueFromDOMJSfn; }

            set { }
        }

        [JsonIgnore]
        //public override string EnableJSfn { get { return @"this.__IsDisable = false; $('[ebsid='+this.__DG.EbSid_CtxId +']').find(`tr[rowid=${this.__rowid}] [colname=${this.Name}] .ctrl-cover *`).prop('disabled',false).css('pointer-events', 'inherit').find('input').css('background-color','#fff');"; } set { } }
        public override string EnableJSfn { get { return @"
    //if(this.__IsDisable){
        let td = document.getElementById('td_' + this.EbSid_CtxId);
        td.style.backgroundColor ='inherit';
        td.style.pointerEvents = 'inherit';
        td.querySelector('.ctrl-cover').setAttribute('eb-readonly','false');
        td.querySelectorAll('input,select,button').disabled = false;
        td.querySelectorAll('input,select,button').forEach( x=> x.setAttribute('tabindex',0));
        document.getElementById(this.EbSid_CtxId).disabled = false;
    //}
    //this.__IsDisable = false;
"; } set { } }

        [JsonIgnore]
        //public override string DisableJSfn { get { return @"this.__IsDisable = true; $('[ebsid='+this.__DG.EbSid_CtxId +']').find(`tr[rowid=${this.__rowid}] [colname=${this.Name}] .ctrl-cover *`).attr('disabled', 'disabled').css('pointer-events', 'none').find('input').css('background-color','#eee');"; } set { } }
        public override string DisableJSfn { get { return @"
    //if(!this.__IsDisable){
        let td = document.getElementById('td_' + this.EbSid_CtxId);
        td.style.backgroundColor ='rgb(238, 238, 238, 0.6)';
        //td.style.pointerEvents = 'none';
        td.querySelector('.ctrl-cover').setAttribute('eb-readonly','true');
        td.querySelectorAll('input,select,button').disabled = true;
        td.querySelectorAll('input,select,button').forEach( x=> x.setAttribute('tabindex',-1));
        document.getElementById(this.EbSid_CtxId).disabled = true;
    //}
    //this.__IsDisable = true;
"; } set { } }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.UserControl)]
        [HideInPropertyGrid]
        public bool IsDGCtrl { get { return true; } set { } }


        [JsonIgnore]
        public override string ClearJSfn { get { return @"document.getElementById(this.EbSid_CtxId).value = '';"; } set { } }

        [JsonIgnore]
        public override string HideJSfn { get { return @""; } set { } }

        [JsonIgnore]
        public override string ShowJSfn { get { return @""; } set { } }

        [JsonIgnore]
        public override string AddInvalidStyleJSFn { get { return @"DGaddInvalidStyle.bind(this)(p1, p2, p3);"; } set { } }

        [JsonIgnore]
        public override string RemoveInvalidStyleJSFn { get { return @"DGremoveInvalidStyle.bind(this)(p1, p2);"; } set { } }

        [JsonIgnore]
        public override string Label { get; set; }


        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.UserControl)]
        [UIproperty]
        [OnChangeUIFunction("EbDataGrid.title")]
        [PropertyGroup(PGConstants.CORE)]
        [PropertyEditor(PropertyEditorType.MultiLanguageKeySelector)]
        public string Title { get; set; }

        [HideInPropertyGrid]
        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.UserControl)]
        public string DBareHtml { get; set; }

        [EnableInBuilder(BuilderType.WebForm)]
        [HideInPropertyGrid]
        public virtual string InputControlType { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.UserControl)]
        public override bool IsDisable { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.UserControl)]
        [DefaultPropValue("true")]
        [PropertyGroup(PGConstants.BEHAVIOR)]
        public virtual bool IsEditable { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.UserControl)]
        [PropertyGroup(PGConstants.APPEARANCE)]
        public virtual int Width { get; set; }

        public override SingleColumn GetSingleColumn(User UserObj, Eb_Solution SoluObj, object Value, bool Default)
        {
            return EbControl.GetSingleColumn(this, UserObj, SoluObj, Value);
        }

        public override bool ParameterizeControl(ParameterizeCtrl_Params args, string crudContext)
        {
            if (this.BypassParameterization && args.cField.Value == null)
                throw new Exception($"Unable to proceed/bypass with value '{args.cField.Value}' for {this.Name} (dg)");

            if (args.cField.Value == null || (this.EbDbType == EbDbTypes.Decimal && Convert.ToString(args.cField.Value) == string.Empty))
            {
                var p = args.DataDB.GetNewParameter(args.cField.Name + "_" + args.i, (EbDbTypes)args.cField.Type);
                p.Value = DBNull.Value;
                args.param.Add(p);
            }
            else if (!this.BypassParameterization)
                args.param.Add(args.DataDB.GetNewParameter(args.cField.Name + "_" + args.i, (EbDbTypes)args.cField.Type, args.cField.Value));

            if (args.ins)
            {
                args._cols += string.Concat(args.cField.Name, ", ");
                if (this.BypassParameterization)
                    args._vals += Convert.ToString(args.cField.Value) + ", ";
                else
                    args._vals += string.Concat("@", args.cField.Name, "_", args.i, ", ");
            }
            else
            {
                if (this.BypassParameterization)
                    args._colvals += args.cField.Name + "=" + Convert.ToString(args.cField.Value) + ", ";
                else
                    args._colvals += string.Concat(args.cField.Name, "=@", args.cField.Name, "_", args.i, ", ");
            }
            args.i++;
            return true;
        }

        public virtual DVBaseColumn GetDVBaseColumn(int index)
        {
            return new DVStringColumn { Data = index, Name = this.Name, sTitle = this.Title, Type = this.EbDbType, bVisible = !this.Hidden, sWidth = "100px" };
        }
    }

    [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.UserControl)]
    [Alias("String Column")]
    [UsedWithTopObjectParent(typeof(EbObject))]
    public class EbDGStringColumn : EbDGColumn
    {

        [JsonIgnore]
        public EbTextBox EbTextBox { get; set; }

        public EbDGStringColumn()
        {
            this.EbTextBox = new EbTextBox();
        }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.UserControl)]
        [OnChangeExec(@"
if (this.TextMode === 4 ){
    pg.ShowProperty('RowsVisible');
}
else {
    pg.HideProperty('RowsVisible');
}
            ")]
        [PropertyGroup(PGConstants.CORE)]
        public TextMode TextMode
        {
            get { return this.EbTextBox.TextMode; }
            set { this.EbTextBox.TextMode = value; }
        }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl)]
        [PropertyGroup(PGConstants.BEHAVIOR)]
        [DefaultPropValue("3")]
        public int RowsVisible
        {
            get { return this.EbTextBox.RowsVisible; }
            set { this.EbTextBox.RowsVisible = value; }
        }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.UserControl)]
        public override string ToolTipText
        {
            get { return this.EbTextBox.ToolTipText; }
            set { this.EbTextBox.ToolTipText = value; }
        }

        [EnableInBuilder(BuilderType.WebForm)]
        [PropertyGroup(PGConstants.BEHAVIOR)]
        public bool AutoSuggestion
        {
            get { return this.EbTextBox.AutoSuggestion; }
            set { this.EbTextBox.AutoSuggestion = value; }
        }

        [EnableInBuilder(BuilderType.WebForm)]
        [HideInPropertyGrid]
        public string TableName
        {
            get { return this.EbTextBox.TableName; }
            set { this.EbTextBox.TableName = value; }
        }

        [EnableInBuilder(BuilderType.WebForm)]
        [HideInPropertyGrid]
        public List<string> Suggestions
        {
            get { return this.EbTextBox.Suggestions; }
            set { this.EbTextBox.Suggestions = value; }
        }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm)]
        public override bool Index { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.UserControl)]
        [HideInPropertyGrid]
        public override EbDbTypes EbDbType { get { return EbDbTypes.String; } }

        [EnableInBuilder(BuilderType.WebForm)]
        [HideInPropertyGrid]
        public override string InputControlType { get { return "EbTextBox"; } }

        [OnDeserialized]
        public void OnDeserializedMethod(StreamingContext context)
        {
            DBareHtml = EbTextBox.GetBareHtml();
        }
        public void InitFromDataBase(JsonServiceClient ServiceClient)
        {
            EbTextBox.Name = this.Name;
            EbTextBox.TableName = this.TableName;
            EbTextBox.AutoSuggestion = this.AutoSuggestion;
            this.EbTextBox.InitFromDataBase(ServiceClient);
        }
    }

    [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.UserControl)]
    [Alias("Numeric Column")]
    [UsedWithTopObjectParent(typeof(EbObject))]
    public class EbDGNumericColumn : EbDGColumn
    {
        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.UserControl)]
        [HideInPropertyGrid]
        public override EbDbTypes EbDbType { get { return EbDbTypes.Decimal; } }

        [EnableInBuilder(BuilderType.WebForm)]
        [HideInPropertyGrid]
        public override string InputControlType { get { return "EbNumeric"; } }

        [EnableInBuilder(BuilderType.WebForm)]
        [PropertyGroup(PGConstants.EXTENDED)]
        public bool IsAggragate { get; set; }

        [EnableInBuilder(BuilderType.WebForm)]
        [PropertyGroup(PGConstants.EXTENDED)]
        public bool AllowNegative { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl)]
        [DefaultPropValue("2")]
        [PropertyGroup(PGConstants.EXTENDED)]
        [Alias("Decimal Places")]
        [HelpText("Number of decimal places")]
        public int DecimalPlaces { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl)]
        [PropertyGroup(PGConstants.APPEARANCE)]
        [UIproperty]
        [OnChangeUIFunction("Common.CONTROL_ICON")]
        [DefaultPropValue("true")]
        public bool HideInputIcon { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl)]
        [PropertyGroup(PGConstants.CORE)]
        public NumInpMode InputMode { get; set; }

        [JsonIgnore]
        public override string GetValueFromDOMJSfn
        {
            get
            {
                return "let val = " + base.GetValueFromDOMJSfn.Replace("return", "").Replace(";", "") + "; val = val || '0'; " +
                  " return parseFloat(val.replace(/,/g, ''))";
            }

            set { }
        }

        [JsonIgnore]
        public override string GetDisplayMemberFromDOMJSfn
        {
            get
            {
                return "let val = " + base.GetValueFromDOMJSfn.Replace("return", "").Replace(";", "") + "; val = val || '0'; val = parseFloat(val.replace(/,/g, '')); " +
                  "return this.InputMode == 1 ? val.toLocaleString('en-IN', { maximumFractionDigits: this.DecimalPlaces, minimumFractionDigits: this.DecimalPlaces }) : val.toFixed(this.DecimalPlaces);";
            }

            set { }
        }

        [JsonIgnore]
        public override string SetDisplayMemberJSfn
        {
            get
            {
                return JSFnsConstants.DG_hiddenColCheckCode + @"
{
if(this.DataVals.ObjType === 'DGNumericColumn' && p1 === undefined)
    p1 = 0;
let ele = document.getElementById(this.EbSid_CtxId);
let start = ele.selectionStart, end = ele.selectionEnd;
ele.value = p1;
ele.setSelectionRange(start, end);
}";
            }

            set { }
        }

        [JsonIgnore]
        public override string OnChangeBindJSFn { get { return @"$(`[ebsid=${p1.DG.EbSid_CtxId }]`).on('keyup change', `[colname=${this.Name}] [ui-inp]`, p2);"; } set { } }

        public override SingleColumn GetSingleColumn(User UserObj, Eb_Solution SoluObj, object Value, bool Default)
        {
            return EbNumeric.GetSingleColumn(this, UserObj, SoluObj, Value);
        }

        public override DVBaseColumn GetDVBaseColumn(int index)
        {
            return new DVNumericColumn
            {
                Data = index,
                Name = this.Name,
                sTitle = this.Title,
                Type = this.EbDbType,
                bVisible = !this.Hidden,
                sWidth = "100px",
                Align = Align.Right,
                Aggregate = this.IsAggragate,
                DecimalPlaces = this.DecimalPlaces
            };
        }
    }

    [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.UserControl)]
    [Alias("Boolean Column")]
    [UsedWithTopObjectParent(typeof(EbObject))]
    public class EbDGBooleanColumn : EbDGColumn
    {
        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.UserControl)]
        [HideInPropertyGrid]
        public override EbDbTypes EbDbType { get { return EbDbTypes.Boolean; } }

        [EnableInBuilder(BuilderType.WebForm)]
        [HideInPropertyGrid]
        public override string InputControlType { get { return "EbCheckBox"; } }

        public override string GetValueFromDOMJSfn
        {
            get { return @"
if($('[ebsid='+this.__DG.EbSid_CtxId +']').find(`tr[rowid=${this.__rowid}] [colname=${this.Name}] [ui-inp]`).is(':checked'))
{
return true;
}
else{
return false;
}
"; }

            set { }
        }

        public override string GetDisplayMemberFromDOMJSfn
        {
            get { return @"
if($('[ebsid='+this.__DG.EbSid_CtxId +']').find(`tr[rowid=${this.__rowid}] [colname=${this.Name}] [ui-inp]`).is(':checked'))
{
return '✔';
}
else{
return '✖';
}
"; }
            set { }
        }

        [JsonIgnore]
        public override string SetDisplayMemberJSfn
        {
            get
            {
                return JSFnsConstants.DG_hiddenColCheckCode + JSFnsConstants.CB_JustSetValueJSfn;
            }
            set { }
        }
        public override bool ParameterizeControl(ParameterizeCtrl_Params args, string crudContext)
        {
            if (args.cField.Value == null)
            {
                args.param.Add(args.DataDB.GetNewParameter(args.cField.Name + "_" + args.i, this.EbDbType, false));
            }
            else
            {
                args.param.Add(args.DataDB.GetNewParameter(args.cField.Name + "_" + args.i, this.EbDbType, args.cField.Value.ToString().ToLower() == "true" ? true : false));
            }

            if (args.ins)
            {
                args._cols += string.Concat(args.cField.Name, ", ");
                args._vals += string.Concat("@", args.cField.Name, "_", args.i, ", ");
            }
            else
                args._colvals += string.Concat(args.cField.Name, "=@", args.cField.Name, "_", args.i, ", ");
            args.i++;
            return true;
        }

        public override SingleColumn GetSingleColumn(User UserObj, Eb_Solution SoluObj, object Value, bool Default)
        {
            object _formattedData = false;
            string _displayMember = "false";

            if (Value != null)
            {
                if (Value.ToString().ToLower() == "t" || Value.ToString().ToLower() == "true")
                {
                    _formattedData = true;
                    _displayMember = "true";
                }
            }

            return new SingleColumn()
            {
                Name = this.Name,
                Type = (int)this.EbDbType,
                Value = _formattedData,
                Control = this,
                ObjType = this.ObjType,
                F = _displayMember
            };
        }

        public override DVBaseColumn GetDVBaseColumn(int index)
        {
            return new DVBooleanColumn { Data = index, Name = this.Name, sTitle = this.Title, Type = this.EbDbType, bVisible = !this.Hidden, sWidth = "100px" };
        }

    }

    [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.UserControl)]
    [Alias("Date Column")]
    [UsedWithTopObjectParent(typeof(EbObject))]
    public class EbDGDateColumn : EbDGColumn
    {
        [JsonIgnore]
        public EbDate EbDate { get; set; }

        public EbDGDateColumn()
        {
            this.EbDate = new EbDate();
        }
        [OnDeserialized]
        public void OnDeserializedMethod(StreamingContext context)
        {
            this.ObjType = this.GetType().Name.Substring(2, this.GetType().Name.Length - 2);
            DBareHtml = EbDate.GetBareHtml();
        }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.UserControl)]
        [HideInPropertyGrid]
        public override EbDbTypes EbDbType
        {
            get { return this.EbDate.EbDbType; }
            set { this.EbDate.EbDbType = value; }
        }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.UserControl)]
        [PropertyGroup(PGConstants.CORE)]
        public EbDateType EbDateType
        {
            get { return this.EbDate.EbDateType; }
            set { this.EbDate.EbDateType = value; }
        }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.UserControl)]
        [PropertyGroup(PGConstants.CORE)]
        public DateRestrictionRule RestrictionRule
        {
            get { return this.EbDate.RestrictionRule; }
            set { this.EbDate.RestrictionRule = value; }
        }

        [JsonIgnore]
        public override string GetValueFromDOMJSfn
        {
            get
            {
                return this.EbDate.GetValueFromDOMJSfn;
            }
            set { }
        }

        [JsonIgnore]
        public override string SetDisplayMemberJSfn
        {
            get
            {
                return JSFnsConstants.DG_hiddenColCheckCode + this.EbDate.SetDisplayMemberJSfn;
            }
            set { }
        }

        [JsonIgnore]
        public override string SetValueJSfn
        {
            get
            {
                return this.EbDate.SetValueJSfn;
            }
            set { }
        }

        [JsonIgnore]
        public override string GetDisplayMemberFromDOMJSfn
        {
            get
            {
                return this.EbDate.GetDisplayMemberFromDOMJSfn;
            }
            set { }
        }

        [JsonIgnore]
        public override string OnChangeBindJSFn { get { return @"
$(`[ebsid=${p1.DG.EbSid_CtxId}]`).on('change', `[colname=${this.Name}] [ui-inp]`, p2).siblings('.nullable-check').on('change', `input[type=checkbox]`, p2);"; } set { } }

        [EnableInBuilder(BuilderType.WebForm)]
        [HideInPropertyGrid]
        public override string InputControlType { get { return "EbDate"; } }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl)]
        [OnChangeExec(@"
                if (this.DoNotPersist){
                        pg.HideProperty('IsNullable');
                }
                else {
                       pg.ShowProperty('IsNullable');
                }
            ")]
        public override bool DoNotPersist
        {
            get { return this.EbDate.DoNotPersist; }
            set { this.EbDate.DoNotPersist = value; }
        }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl)]
        [PropertyGroup(PGConstants.EXTENDED)]
        public bool IsNullable
        {
            get { return this.EbDate.IsNullable; }
            set { this.EbDate.IsNullable = value; }
        }


        public override bool ParameterizeControl(ParameterizeCtrl_Params args, string crudContext)
        {
            return this.EbDate.ParameterizeControl(args, true, crudContext);
        }

        //EbDGDateColumn
        public override SingleColumn GetSingleColumn(User UserObj, Eb_Solution SoluObj, object Value, bool Default)
        {
            return EbDate.GetSingleColumn(this, UserObj, SoluObj, Value);
        }

        public override DVBaseColumn GetDVBaseColumn(int index)
        {
            return new DVDateTimeColumn { Data = index, Name = this.Name, sTitle = this.Title, sType = "date-uk", Type = this.EbDbType, bVisible = !this.Hidden, sWidth = "100px" };
        }
    }

    [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.UserControl)]
    [Alias("Dropdown Column")]
    [UsedWithTopObjectParent(typeof(EbObject))]
    public class EbDGSimpleSelectColumn : EbDGColumn
    {
        [JsonIgnore]
        public EbSimpleSelect EbSimpleSelect { get; set; }

        public EbDGSimpleSelectColumn()
        {
            this.EbSimpleSelect = new EbSimpleSelect();
        }

        //[JsonIgnore]
        //public override string DisableJSfn
        //{
        //    get
        //    {
        //        return @"this.__IsDisable = true; $('[ebsid='+this.__DG.EbSid_CtxId +']').find(`tr[rowid=${this.__rowid}] [colname=${this.Name}] .ctrl-cover .dropdown-toggle`).attr('disabled', 'disabled').css('pointer-events', 'none').css('background-color', 'var(--eb-disablegray)');";
        //    }
        //    set { }
        //}

        //[JsonIgnore]
        //public override string EnableJSfn
        //{
        //    get
        //    {
        //        return @"this.__IsDisable = false; $('[ebsid='+this.__DG.EbSid_CtxId +']').find(`tr[rowid=${this.__rowid}] [colname=${this.Name}] .ctrl-cover .dropdown-toggle`).prop('disabled',false).css('pointer-events', 'inherit').css('background-color', '#fff');";
        //    }
        //    set { }
        //}

        [JsonIgnore]
        public override string SetDisplayMemberJSfn
        {
            get
            {
                return JSFnsConstants.DG_hiddenColCheckCode + EbSimpleSelect.SetDisplayMemberJSfn;
            }
            set { }
        }

        public override string GetValueFromDOMJSfn
        {
            get
            {
                return @"
let val = $('[ebsid=' + this.__DG.EbSid_CtxId  + ']').find(`tr[rowid=${this.__rowid}] [colname=${this.Name}] [ui-inp]`).val();
if(ebcontext.renderContext === 'WebForm' && val === '-1')
    val = null;
return val;
";
            }
            set { }
        }

        public override string GetDisplayMemberFromDOMJSfn { get { return @" return $('[ebsid='+this.__DG.EbSid_CtxId +']').find(`tr[rowid=${this.__rowid}] [colname=${this.Name}] [ui-inp] :selected`).text(); "; } set { } }

        [EnableInBuilder(BuilderType.WebForm)]
        [HideInPropertyGrid]
        public override string InputControlType { get { return "EbSimpleSelect"; } }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl)]
        [HideInPropertyGrid]
        public override EbDbTypes EbDbType
        {
            get { return this.EbSimpleSelect.EbDbType; }
            set { this.EbSimpleSelect.EbDbType = value; }
        }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl)]
        [OSE_ObjectTypes(EbObjectTypes.iDataReader)]
        [PropertyEditor(PropertyEditorType.ObjectSelector)]
        public string DataSourceId
        {
            get { return this.EbSimpleSelect.DataSourceId; }
            set { this.EbSimpleSelect.DataSourceId = value; }
        }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl)]
        [MetaOnly]
        public DVColumnCollection Columns
        {
            get { return this.EbSimpleSelect.Columns; }
            set { this.EbSimpleSelect.Columns = value; }
        }

        [EnableInBuilder(BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.WebForm, BuilderType.UserControl)]
        [PropertyEditor(PropertyEditorType.CollectionFrmSrc, "Columns", 1)]
        [OnChangeExec(@"if (this.Columns && this.Columns.$values.length === 0 ){pg.MakeReadOnly('ValueMember');} else {pg.MakeReadWrite('ValueMember');}")]
        [PropertyPriority(69)]
        [PropertyGroup(PGConstants.DATA_SETTINGS)]
        public DVBaseColumn ValueMember
        {
            get { return this.EbSimpleSelect.ValueMember; }
            set { this.EbSimpleSelect.ValueMember = value; }
        }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl)]
        [PropertyEditor(PropertyEditorType.Collection)]
        [Alias("Options")]
        public List<EbSimpleSelectOption> Options
        {
            get { return this.EbSimpleSelect.Options; }
            set { this.EbSimpleSelect.Options = value; }
        }

        [EnableInBuilder(BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.WebForm, BuilderType.UserControl)]
        [PropertyEditor(PropertyEditorType.CollectionFrmSrc, "Columns", 1)]
        [PropertyPriority(68)]
        [PropertyGroup(PGConstants.DATA_SETTINGS)]
        [OnChangeExec(@"if (this.Columns && this.Columns.$values.length === 0 ){pg.MakeReadOnly('DisplayMember');} else {pg.MakeReadWrite('DisplayMember');}")]
        public DVBaseColumn DisplayMember
        {
            get { return this.EbSimpleSelect.DisplayMember; }
            set { this.EbSimpleSelect.DisplayMember = value; }
        }

        //[EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl)]
        //public int Value
        //{
        //    get { return this.EbSimpleSelect.Value; }
        //    set { this.EbSimpleSelect.Value = value; }
        //}

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.UserControl)]
        [PropertyGroup(PGConstants.BEHAVIOR)]
        [Alias("Readonly")]
        [HelpText("Control will be Disabled/Readonly if set to TRUE")]
        public override bool IsDisable
        {
            get { return this.EbSimpleSelect.IsDisable; }
            set { this.EbSimpleSelect.IsDisable = value; }
        }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl)]
        [PropertyEditor(PropertyEditorType.Boolean)]
        [PropertyGroup(PGConstants.CORE)]
        [OnChangeExec(@"if(this.IsDynamic === true){pg.ShowProperty('DataSourceId');pg.ShowProperty('ValueMember');pg.ShowProperty('DisplayMember');pg.HideProperty('Options');}
else{pg.HideProperty('DataSourceId');pg.HideProperty('ValueMember');pg.HideProperty('DisplayMember');pg.ShowProperty('Options');}")]
        public bool IsDynamic
        {
            get { return this.EbSimpleSelect.IsDynamic; }
            set { this.EbSimpleSelect.IsDynamic = value; }
        }

        [JsonIgnore]
        public string OptionHtml
        {
            get { return this.EbSimpleSelect.OptionHtml; }
            set { this.EbSimpleSelect.OptionHtml = value; }
        }

        [OnDeserialized]
        public void OnDeserializedMethod(StreamingContext context)
        {
            DBareHtml = EbSimpleSelect.GetBareHtml();
        }

        public string GetDisplayMembersQuery(IDatabase DataDB, Service service, string vms)
        {
            return this.EbSimpleSelect.GetDisplayMembersQuery(DataDB, service, vms);
        }

        public override SingleColumn GetSingleColumn(User UserObj, Eb_Solution SoluObj, object Value, bool Default)
        {
            this.EbSimpleSelect.Name = this.Name;
            return this.EbSimpleSelect.GetSingleColumn(UserObj, SoluObj, Value, Default);
        }
    }

    [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.UserControl)]
    [Alias("BooleanSelect Column")]
    [UsedWithTopObjectParent(typeof(EbObject))]
    public class EbDGBooleanSelectColumn : EbDGColumn
    {
        [JsonIgnore]
        public EbBooleanSelect EbBooleanSelect { get; set; }

        [JsonIgnore]
        private EbDGSimpleSelectColumn EbDGSimpleSelectColumn { set; get; }

        public override string SetDisplayMemberJSfn
        {
            get
            {
                return JSFnsConstants.DG_hiddenColCheckCode + @"
                        {
                            if(p1 === true)
                                p1 = 'true'
                            else if(p1 === false)
                                p1 = 'false'
                           " + EbDGSimpleSelectColumn.SetDisplayMemberJSfn +
                        "}";
            }
            set { }
        }

        public override string SetValueJSfn
        {
            get
            {
                return JSFnsConstants.DG_hiddenColCheckCode +
                    @"
                    {
                        if(p1 === true)
                            p1 = 'true'
                        else if(p1 === false)
                            p1 = 'false'
                       " + EbDGSimpleSelectColumn.SetValueJSfn +
                    "}";
            }
            set { }
        }

        public override string GetValueFromDOMJSfn
        {
            get
            {
                return EbDGSimpleSelectColumn.GetValueFromDOMJSfn.Replace("return val;", "val = (val ==='true'); return val;");
            }
            set { }
        }

        public EbDGBooleanSelectColumn()
        {
            this.EbBooleanSelect = new EbBooleanSelect();
            this.EbDGSimpleSelectColumn = new EbDGSimpleSelectColumn();
        }

        //[JsonIgnore]
        //public override string DisableJSfn
        //{
        //    get
        //    {
        //        return EbDGSimpleSelectColumn.DisableJSfn;
        //    }
        //    set { }
        //}

        //[JsonIgnore]
        //public override string EnableJSfn
        //{
        //    get
        //    {
        //        return EbDGSimpleSelectColumn.EnableJSfn;
        //    }
        //    set { }
        //}

        public override string GetDisplayMemberFromDOMJSfn
        {
            get
            {
                return EbDGSimpleSelectColumn.GetDisplayMemberFromDOMJSfn;
            }
            set { }
        }
        public override string IsRequiredOKJSfn
        {
            get
            {
                return EbDGSimpleSelectColumn.IsRequiredOKJSfn;
            }
            set { }
        }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl)]
        [PropertyGroup(PGConstants.BEHAVIOR)]
        [PropertyPriority(52)]
        [DefaultPropValue("Yes")]
        public string TrueText
        {
            get { return this.EbBooleanSelect.TrueText; }
            set { this.EbBooleanSelect.TrueText = value; }
        }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl)]
        [PropertyGroup(PGConstants.BEHAVIOR)]
        [PropertyPriority(51)]
        [DefaultPropValue("No")]
        public string FalseText
        {
            get { return this.EbBooleanSelect.FalseText; }
            set { this.EbBooleanSelect.FalseText = value; }
        }

        [EnableInBuilder(BuilderType.WebForm)]
        [HideInPropertyGrid]
        public override string InputControlType { get { return "EbBooleanSelect"; } }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl)]
        [HideInPropertyGrid]
        public override EbDbTypes EbDbType
        {
            get { return this.EbBooleanSelect.EbDbType; }
            set { this.EbBooleanSelect.EbDbType = value; }
        }

        [HideInPropertyGrid]
        [EnableInBuilder(BuilderType.BotForm)]
        public override bool IsDisable
        {
            get { return this.EbBooleanSelect.IsDisable; }
            set { this.EbBooleanSelect.IsDisable = value; }
        }

        [OnDeserialized]
        public void OnDeserializedMethod(StreamingContext context)
        {
            DBareHtml = EbBooleanSelect.GetBareHtml();
        }

        //EbDGBooleanSelectColumn
        public override SingleColumn GetSingleColumn(User UserObj, Eb_Solution SoluObj, object Value, bool Default)
        {
            return EbBooleanSelect.GetSingleColumn(this, UserObj, SoluObj, Value);
        }
    }

    [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.UserControl)]
    [UsedWithTopObjectParent(typeof(EbObject))]
    [Alias("UserControl Column")]
    public class EbDGUserControlColumn : EbDGColumn
    {
        [JsonIgnore]
        public EbUserControl EbUserControl { get; set; }

        public EbDGUserControlColumn()
        {
            this.EbUserControl = new EbUserControl();
        }

        [EnableInBuilder(BuilderType.WebForm)]
        [HideInPropertyGrid]
        public override string InputControlType { get { return "EbUserControl"; } }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl)]
        [HideInPropertyGrid]
        [Alias("Controls")]
        public List<EbControl> Columns
        {
            get { return this.EbUserControl.Controls; }
            set { this.EbUserControl.Controls = value; }
        }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl)]
        [OSE_ObjectTypes(EbObjectTypes.iUserControl)]
        [PropertyEditor(PropertyEditorType.ObjectSelector)]
        public override string RefId { get { return this.EbUserControl.RefId; } set { this.EbUserControl.RefId = value; } }


        [HideInPropertyGrid]
        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.UserControl)]
        public string ChildHtml { get; set; }

        public override string GetBareHtml()
        {
            return this.EbUserControl.GetBareHtml();
        }

        public void InitUserControl(EbUserControl ebUserControl)
        {
            this.Columns = ebUserControl.Controls;
            this.ObjType = this.ObjType;
            this.InitDBareHtml();
        }

        public void InitDBareHtml()
        {
            DBareHtml = (@"
<div  id='@ebsid@_wrap'>
    <div class='input-group' style='width:100%;'>          
        <input id='@ebsid@_inp' ui-inp data-toggle='tooltip' title='' type='text' tabindex='0' style='width:100%; data-original-title='' disabled>
        <span id='@ebsid@_showbtn' class='input-group-addon ucspan' data-toggle='modal' data-target='#@colebsid@_usercontrolmodal' style='padding: 0px;'> <button type='button' id='Date1TglBtn' class='fa  fa-ellipsis-h ucbtn' aria-hidden='true' style='padding: 6px 12px;'></button> </span>
    </div>
</div>
").Replace("@colebsid@", EbSid_CtxId).RemoveCR();
            ChildHtml = GetBareHtml();
        }
    }

    [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog)]
    [Alias("PowerSelect Column")]
    [UsedWithTopObjectParent(typeof(EbObject))]
    public class EbDGPowerSelectColumn : EbDGColumn, IEbPowerSelect, IEbDataReaderControl
    {


        //[OnDeserialized]
        //public void OnDeserializedMethod(StreamingContext context)
        //{
        //    if (RenderAsSimpleSelect)
        //    {
        //        IsDynamic = true;
        //        this.EbPowerSelect.Options = new List<EbSimpleSelectOption>();
        //        this.DBareHtml = this.GetBareHtml();
        //    }
        //}

        [JsonIgnore]
        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl)]
        [PropertyEditor(PropertyEditorType.Boolean)]
        [PropertyGroup(PGConstants.EXTENDED)]
        [DefaultPropValue("true")]
        [OnChangeExec(@"

if(this.RenderAsSimpleSelect == true){ //SS
    if(this.IsDynamic == false){// SS static
	    pg.ShowProperty('Options');
	    pg.HideProperty('ValueMember');
	    pg.HideProperty('DisplayMember'); 
    }
    else{// SS dynamic
	    pg.HideProperty('Options');
	    pg.ShowProperty('ValueMember');
	    pg.ShowProperty('DisplayMember');
    }
}
else// PS
{
	pg.ShowProperty('ValueMember');
	pg.ShowProperty('DisplayMembers');
	pg.ShowProperty('Columns');
	pg.HideProperty('DisplayMember');
	pg.HideProperty('Options');
}
")]
        public bool IsDynamic { get; set; }

        [JsonIgnore]
        public override string JustSetValueJSfn { get { return this.EbPowerSelect.JustSetValueJSfn; } set { } }

        [JsonIgnore]
        public override string GetDisplayMemberFromDOMJSfn { get { return this.EbPowerSelect.GetDisplayMemberFromDOMJSfn; } set { } }
        [JsonIgnore]
        public override string GetValueFromDOMJSfn { get { return this.EbPowerSelect.GetValueFromDOMJSfn; } set { } }

        [JsonIgnore]
        public override string GetColumnJSfn { get { return this.EbPowerSelect.GetColumnJSfn; } set { } }

        [JsonIgnore]
        public override string IsRequiredOKJSfn { get { return this.EbPowerSelect.IsRequiredOKJSfn; } set { } }

        [JsonIgnore]
        public EbPowerSelect EbPowerSelect { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.UserControl, BuilderType.BotForm, BuilderType.FilterDialog)]
        [PropertyGroup(PGConstants.CORE)]
        [HideForUser]
        [OnChangeExec(@"
        if(this.IsDataFromApi){
            pg.ShowGroup('Api');
            pg.HideProperty('DataSourceId');
        }
        else{
            pg.HideGroup('Api');
            pg.ShowProperty('DataSourceId');
        }")]
        public bool IsDataFromApi { get { return this.EbPowerSelect.IsDataFromApi; } set { this.EbPowerSelect.IsDataFromApi = value; } }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.UserControl, BuilderType.BotForm, BuilderType.FilterDialog)]
        [PropertyGroup("Api")]
        [HideForUser]
        [OnChangeExec(@"
if (this.Columns && this.Columns.$values.length === 0 )
{
pg.MakeReadOnly('DisplayMembers');} else {pg.MakeReadWrite('DisplayMembers');}")]
        public string Url { get { return this.EbPowerSelect.Url; } set { this.EbPowerSelect.Url = value; } }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.UserControl, BuilderType.BotForm, BuilderType.FilterDialog)]
        [PropertyGroup("Api")]
        [HideForUser]
        public ApiMethods Method { get { return this.EbPowerSelect.Method; } set { this.EbPowerSelect.Method = value; } }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.UserControl, BuilderType.BotForm, BuilderType.FilterDialog)]
        [PropertyEditor(PropertyEditorType.Collection)]
        [PropertyGroup("Api")]
        [HideForUser]
        public List<ApiRequestHeader> Headers { get { return this.EbPowerSelect.Headers; } set { this.EbPowerSelect.Headers = value; } }

        //[EnableInBuilder(BuilderType.WebForm, BuilderType.UserControl, BuilderType.BotForm, BuilderType.FilterDialog)]
        //[PropertyEditor(PropertyEditorType.Collection)]
        //[PropertyGroup("Api")]
        //[MetaOnly]
        //public List<ApiRequestParam> Parameters { get { return this.EbPowerSelect.Parameters; } set { this.EbPowerSelect.Parameters = value; } }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm)]
        [PropertyEditor(PropertyEditorType.ObjectSelector)]
        [OSE_ObjectTypes(EbObjectTypes.iDataReader)]
        [PropertyGroup(PGConstants.DATA_SETTINGS)]
        [OnChangeExec(@"
if (this.Columns && this.Columns.$values.length === 0 )
{
pg.MakeReadOnly('DisplayMembers');} else {pg.MakeReadWrite('DisplayMembers');}")]
        public string DataSourceId { get { return this.EbPowerSelect.DataSourceId; } set { this.EbPowerSelect.DataSourceId = value; } }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm)]
        [PropertyEditor(PropertyEditorType.CollectionFrmSrc, "Columns", 1)]
        [OnChangeExec(@"if (this.Columns && this.Columns.$values.length === 0 ){pg.MakeReadOnly('ValueMember');} else {pg.MakeReadWrite('ValueMember');}")]
        [PropertyPriority(69)]
        [PropertyGroup(PGConstants.DATA_SETTINGS)]
        public DVBaseColumn ValueMember { get { return this.EbPowerSelect.ValueMember; } set { this.EbPowerSelect.ValueMember = value; } }

        [EnableInBuilder(BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.WebForm, BuilderType.UserControl)]
        [PropertyEditor(PropertyEditorType.CollectionFrmSrc, "Columns", 1)]
        [PropertyPriority(68)]
        [PropertyGroup(PGConstants.DATA_SETTINGS)]
        [OnChangeExec(@"if (this.Columns && this.Columns.$values.length === 0 ){pg.MakeReadOnly('DisplayMember');} else {pg.MakeReadWrite('DisplayMember');}")]
        public DVBaseColumn DisplayMember { get { return this.EbPowerSelect.DisplayMember; } set { this.EbPowerSelect.DisplayMember = value; } }


        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl)]
        [PropertyEditor(PropertyEditorType.CollectionABCFrmSrc, "Columns")]
        [PropertyGroup(PGConstants.DATA_SETTINGS)]
        [PropertyPriority(68)]
        public DVColumnCollection DisplayMembers { get { return this.EbPowerSelect.DisplayMembers; } set { this.EbPowerSelect.DisplayMembers = value; } }


        [EnableInBuilder(BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.WebForm, BuilderType.UserControl)]
        [PropertyEditor(PropertyEditorType.CollectionProp, "Columns", "bVisible")]
        [PropertyGroup(PGConstants.DATA_SETTINGS)]
        public DVColumnCollection Columns { get { return this.EbPowerSelect.Columns; } set { this.EbPowerSelect.Columns = value; } }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm)]
        [PropertyGroup(PGConstants.BEHAVIOR)]
        [PropertyPriority(50)]
        [OnChangeExec(@"
if(this.RenderAsSimpleSelect == true)// SS
{
pg.ShowProperty('DisplayMember');
pg.HideProperty('DisplayMembers');
pg.HideProperty('Columns');
pg.ShowProperty('IsDynamic');   
}
else// PS
{
pg.HideProperty('DisplayMember');
pg.ShowProperty('DisplayMembers');
pg.ShowProperty('Columns');
pg.HideProperty('Options');
pg.HideProperty('IsDynamic');
}
")]
        public bool RenderAsSimpleSelect { get { return this.EbPowerSelect.RenderAsSimpleSelect; } set { this.EbPowerSelect.RenderAsSimpleSelect = value; } }

        [JsonIgnore]
        public string TableName { get; set; }


        //[EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl)]
        //[HideInPropertyGrid]
        //public EbButton AddButton  { set; get; }

        public EbDGPowerSelectColumn()
        {
            this.EbPowerSelect = new EbPowerSelect();
            //this.AddButton = new EbButton();
            this.Options = new List<EbSimpleSelectOption>();
        }

        //[EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
        //[PropertyEditor(PropertyEditorType.ObjectSelector)]
        //[OSE_ObjectTypes(EbObjectTypes.iWebForm)]
        //public string FormRefId { get { return this.FormRefId; } set { this.FormRefId = value; } }

        public override string SetDisplayMemberJSfn { get { return JSFnsConstants.DG_hiddenColCheckCode + EbPowerSelect.SetDisplayMemberJSfn; } set { } }

        public override string SetValueJSfn { get { return EbPowerSelect.SetValueJSfn; } set { } }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
        [PropertyEditor(PropertyEditorType.ObjectSelector)]
        [PropertyGroup(PGConstants.DATA_INSERT)]
        [OSE_ObjectTypes(EbObjectTypes.iWebForm)]
        [Alias("Form")]
        public string FormRefId { get { return this.EbPowerSelect.FormRefId; } set { this.EbPowerSelect.FormRefId = value; } }


        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.UserControl)]
        [PropertyGroup(PGConstants.DATA_INSERT)]
        [PropertyPriority(98)]
        public bool IsInsertable { get { return this.EbPowerSelect.IsInsertable; } set { this.EbPowerSelect.IsInsertable = value; } }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.UserControl)]
        [PropertyGroup(PGConstants.DATA_INSERT)]
        [PropertyPriority(98)]
        public bool OpenInNewTab { get { return this.EbPowerSelect.OpenInNewTab; } set { this.EbPowerSelect.OpenInNewTab = value; } }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.UserControl)]
        [PropertyGroup(PGConstants.DATA_INSERT)]
        [PropertyEditor(PropertyEditorType.Collection)]
        public List<DataFlowMapAbstract> DataFlowMap { get { return this.EbPowerSelect.DataFlowMap; } set { this.EbPowerSelect.DataFlowMap = value; } }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.UserControl, BuilderType.BotForm, BuilderType.FilterDialog)]
        [PropertyGroup("Api")]
        [Alias("Data api parameters")]
        [PropertyEditor(PropertyEditorType.Collection)]
        public List<EbCtrlApiParamAbstract> DataApiParams { get { return this.EbPowerSelect.DataApiParams; } set { this.EbPowerSelect.DataApiParams = value; } }

        //[EnableInBuilder(BuilderType.WebForm, BuilderType.UserControl, BuilderType.BotForm, BuilderType.FilterDialog)]
        //[PropertyEditor(PropertyEditorType.CollectionFrmSrc, "return [...commonO.Current_obj.Controls.$values];")]
        //[PropertyGroup("Api")]
        //[Alias("Parameter controls")]
        //public List<EbControl> ApiParamCtrls { get { return this.EbPowerSelect.ApiParamCtrls; } set { this.EbPowerSelect.ApiParamCtrls = value; } }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.UserControl)]
        [PropertyGroup(PGConstants.APPEARANCE)]
        //[DefaultPropValue("100")]
        public override int Width { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl)]
        [HelpText("Specify minimum number of charecters to initiate search")]
        [Category("Search Settings")]
        [PropertyGroup(PGConstants.SEARCH)]
        public int MinSearchLength { get { return this.EbPowerSelect.MinSearchLength; } set { this.EbPowerSelect.MinSearchLength = value; } }


        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl)]
        [PropertyGroup(PGConstants.SEARCH)]
        [HelpText("Select Search Method - StartsWith, EndsWith, Contains or Exact Match")]
        public PsSearchOperators SearchOperator { get { return this.EbPowerSelect.SearchOperator; } set { this.EbPowerSelect.SearchOperator = value; } }


        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl)]
        [DefaultPropValue("100")]
        [PropertyGroup(PGConstants.BEHAVIOR)]
        public int DropDownItemLimit { get { return this.EbPowerSelect.DropDownItemLimit; } set { this.EbPowerSelect.DropDownItemLimit = value; } }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl)]
        [PropertyGroup(PGConstants.BEHAVIOR)]
        [Alias("Search result limit")]
        public int SearchLimit { get { return this.EbPowerSelect.SearchLimit; } set { this.EbPowerSelect.SearchLimit = value; } }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.UserControl)]
        [PropertyGroup(PGConstants.CORE)]
        [DefaultPropValue("true")]
        [OnChangeExec(@"if (this.IsPreload){pg.MakeReadOnly('SearchLimit');} else {pg.MakeReadWrite('SearchLimit');}")]
        [Alias("Preload items")]
        public bool IsPreload { get { return this.EbPowerSelect.IsPreload; } set { this.EbPowerSelect.IsPreload = value; } }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl)]
        [PropertyGroup(PGConstants.BEHAVIOR)]
        [OnChangeExec(@"
            if (this.MultiSelect === true ){
                pg.MakeReadWrite('MaxLimit'); 
                if (this.Required === true ){
                    if(this.MinLimit < 1){
                        pg.setSimpleProperty('MinLimit', 1);
                    }
                    pg.MakeReadWrite('MinLimit');
                }
                else{
                    pg.setSimpleProperty('MinLimit', 0);
                    pg.MakeReadOnly('MinLimit');               
                }
                if(this.MaxLimit === 1)
                    pg.setSimpleProperty('MaxLimit', 0);
                  
            }
            else {
                pg.setSimpleProperty('MaxLimit', 1);
                pg.MakeReadOnly(['MaxLimit','MinLimit']);
                if (this.Required === true ){
                    pg.setSimpleProperty('MinLimit', 1);
                }
                else{
                    pg.setSimpleProperty('MinLimit', 0);
                }
                if(this.MaxLimit !== 1)
                    pg.setSimpleProperty('MaxLimit', 1);
            }")]
        public bool MultiSelect { get { return this.EbPowerSelect.MultiSelect; } set { this.EbPowerSelect.MultiSelect = value; } }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl)]
        [PropertyGroup(PGConstants.VALIDATIONS)]
        [OnChangeExec(@"
            if (this.MultiSelect === true ){
                pg.MakeReadWrite('MaxLimit'); 
                if (this.Required === true ){
                    if(this.MinLimit < 1){
                        pg.setSimpleProperty('MinLimit', 1);
                    }
                    pg.MakeReadWrite('MinLimit');
                }
                else{
                    pg.setSimpleProperty('MinLimit', 0);
                    pg.MakeReadOnly('MinLimit');               
                }
            }
            else {
                pg.setSimpleProperty('MaxLimit', 1);
                pg.MakeReadOnly(['MaxLimit','MinLimit']);
                if (this.Required === true ){
                    pg.setSimpleProperty('MinLimit', 1);
                }
                else{
                    pg.setSimpleProperty('MinLimit', 0);
                }
            }")]
        public override bool Required { get { return this.EbPowerSelect.Required; } set { this.EbPowerSelect.Required = value; } }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl)]
        [DefaultPropValue("1")]
        [PropertyGroup(PGConstants.BEHAVIOR)]
        public int MaxLimit { get { return this.EbPowerSelect.MaxLimit; } set { this.EbPowerSelect.MaxLimit = value; } }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl)]
        [PropertyGroup(PGConstants.BEHAVIOR)]
        public int MinLimit { get { return this.EbPowerSelect.MinLimit; } set { this.EbPowerSelect.MinLimit = value; } }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm)]
        public override string Name { get { return this.EbPowerSelect.Name; } set { this.EbPowerSelect.Name = value; } }

        [HideInPropertyGrid]
        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm)]
        public override string EbSid { get { return this.EbPowerSelect.EbSid; } set { this.EbPowerSelect.EbSid = value; } }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.UserControl)]
        [HideInPropertyGrid]
        public override EbDbTypes EbDbType { get { return this.EbPowerSelect.EbDbType; } set { this.EbPowerSelect.EbDbType = value; } }

        [HideInPropertyGrid]
        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm)]
        new public string EbSid_CtxId { get { return this.EbPowerSelect.EbSid_CtxId; } set { this.EbPowerSelect.EbSid_CtxId = value; } }


        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl)]
        [PropertyEditor(PropertyEditorType.Collection)]
        [Alias("Options")]
        public List<EbSimpleSelectOption> Options { get { return this.EbPowerSelect.Options; } set { this.EbPowerSelect.Options = value; } }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm)]
        [PropertyGroup(PGConstants.APPEARANCE)]
        public int DropdownHeight { get { return this.EbPowerSelect.DropdownHeight; } set { this.EbPowerSelect.DropdownHeight = value; } }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm)]
        [Alias("DropdownWidth(%)")]
        [DefaultPropValue("100")]
        [PropertyGroup(PGConstants.APPEARANCE)]
        public int DropdownWidth { get { return this.EbPowerSelect.DropdownWidth; } set { this.EbPowerSelect.DropdownWidth = value; } }

        [HideInPropertyGrid]
        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl)]
        public List<Param> ParamsList { get { return this.EbPowerSelect.ParamsList; } set { this.EbPowerSelect.ParamsList = value; } }

        [EnableInBuilder(BuilderType.WebForm)]
        public bool StrictSelect { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm)]
        public override bool Index { get; set; }

        [EnableInBuilder(BuilderType.WebForm)]
        [HideInPropertyGrid]
        public override string InputControlType { get { return "EbPowerSelect"; } }

        public override string GetBareHtml()
        {
            return this.EbPowerSelect.GetBareHtml("@ebsid@"); // temp
        }
        public void InitFromDataBase_SS(JsonServiceClient ServiceClient)
        {
            this.EbPowerSelect.InitFromDataBase_SS(ServiceClient);
            this.DBareHtml = this.GetBareHtml();
        }

        public void UpdateParamsMeta(Service Service, IRedisClient Redis)
        {
            this.EbPowerSelect.UpdateParamsMeta(Service, Redis);
        }

        public (string, EbDataReader) GetSqlAndDr(Service Service)
        {
            return this.EbPowerSelect.GetSqlAndDr(Service);
        }

        public void FetchParamsMeta(IServiceClient ServiceClient, IRedisClient Redis, EbControl[] Allctrls, Service service)
        {
            this.EbPowerSelect.FetchParamsMeta(ServiceClient, Redis, Allctrls, service);
        }

        public string GetSelectQuery(IDatabase DataDB, Service service, string Col, string Tbl = null, string _id = null, string masterTbl = null)
        {
            return EbPowerSelect.GetSelectQuery(this.EbPowerSelect, DataDB, service, Col, Tbl, _id, masterTbl, !StrictSelect);
        }

        //public string GetSelectQuery123(IDatabase DataDB, Service service, string table, string column, string parentTbl, string masterTbl)
        //{
        //    return this.EbPowerSelect.GetSelectQuery123(DataDB, service, table, column, parentTbl, masterTbl);
        //}

        public string GetDisplayMembersQuery(IDatabase DataDB, Service service, string vms, List<DbParameter> param)
        {
            return this.EbPowerSelect.GetDisplayMembersQuery(DataDB, service, vms, param);
        }

        public override bool ParameterizeControl(ParameterizeCtrl_Params args, string crudContext)
        {
            return EbPowerSelect.ParameterizeControl(this, args, true, crudContext);
        }

        public override SingleColumn GetSingleColumn(User UserObj, Eb_Solution SoluObj, object Value, bool Default)
        {
            this.EbPowerSelect.Name = this.Name;
            return EbPowerSelect.GetSingleColumn(this, UserObj, SoluObj, Value, Default);
        }

        public override DVBaseColumn GetDVBaseColumn(int index)
        {
            return new DVStringColumn { Data = index, Name = this.Name, sTitle = this.Title, Type = this.EbDbType, bVisible = !this.Hidden, sWidth = "100px", Align = Align.Left };
        }
    }



    [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.UserControl)]
    [Alias("Created By Column")]
    [UsedWithTopObjectParent(typeof(EbObject))]
    public class EbDGCreatedByColumn : EbDGColumn
    {

        [JsonIgnore]
        public EbSysCreatedBy EbSysCreatedBy { get; set; }

        public EbDGCreatedByColumn()
        {
            this.EbSysCreatedBy = new EbSysCreatedBy();
        }

        [OnDeserialized]
        public void OnDeserializedMethod(StreamingContext context)
        {
            this.ObjType = this.GetType().Name.Substring(2, this.GetType().Name.Length - 2);
            DBareHtml = this.EbSysCreatedBy.GetBareHtml();
            this.Name = "eb_created_by";
        }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.UserControl)]
        [HideInPropertyGrid]
        public override EbDbTypes EbDbType
        {
            get { return this.EbSysCreatedBy.EbDbType; }
            set { this.EbSysCreatedBy.EbDbType = value; }
        }

        [EnableInBuilder(BuilderType.WebForm)]
        [HideInPropertyGrid]
        public override string InputControlType { get { return "EbSysCreatedBy"; } }

        //[EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.UserControl)]
        //public override bool IsEditable
        //{
        // get { return !this.EbSysCreatedBy.IsDisable; }
        // set { this.EbSysCreatedBy.IsDisable = !value; }
        //}

        [JsonIgnore]
        public override string GetValueFromDOMJSfn
        {
            get
            {
                return this.EbSysCreatedBy.GetValueFromDOMJSfn;
            }
            set { }
        }

        [JsonIgnore]
        public override string SetValueJSfn
        {
            get
            {
                return this.EbSysCreatedBy.SetValueJSfn;
            }
            set { }
        }

        [JsonIgnore]
        public override string SetDisplayMemberJSfn
        {
            get
            {
                return JSFnsConstants.DG_hiddenColCheckCode + this.EbSysCreatedBy.SetDisplayMemberJSfn;
            }
            set { }
        }

        [JsonIgnore]
        public override string EnableJSfn
        {
            get
            {
                return this.EbSysCreatedBy.EnableJSfn;
            }
            set { }
        }

        [JsonIgnore]
        public override string GetDisplayMemberFromDOMJSfn
        {
            get
            {
                return this.EbSysCreatedBy.GetDisplayMemberFromDOMJSfn;
            }
            set { }
        }

        [JsonIgnore]
        public override string RefreshJSfn
        {
            get
            {
                return this.EbSysCreatedBy.RefreshJSfn;
            }
            set { }
        }

        [JsonIgnore]
        public override string ClearJSfn
        {
            get
            {
                return this.EbSysCreatedBy.ClearJSfn;
            }
            set { }
        }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl)]
        [HideInPropertyGrid]
        public override bool DoNotPersist { get { return true; } }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl)]
        [HideInToolBox]
        public override bool IsSysControl { get { return true; } }

        //EbDGCreatedByColumn
        public override SingleColumn GetSingleColumn(User UserObj, Eb_Solution SoluObj, object Value, bool Default)
        {
            return EbSysCreatedBy.GetSingleColumn(this, UserObj, SoluObj, Value);
        }
    }

    [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.UserControl)]
    [Alias("Created At Column")]
    [UsedWithTopObjectParent(typeof(EbObject))]
    public class EbDGCreatedAtColumn : EbDGColumn
    {

        [JsonIgnore]
        public EbSysCreatedAt EbSysCreatedAt { get; set; }

        public EbDGCreatedAtColumn()
        {
            this.EbSysCreatedAt = new EbSysCreatedAt();
        }

        [OnDeserialized]
        public void OnDeserializedMethod(StreamingContext context)
        {
            this.ObjType = this.GetType().Name.Substring(2, this.GetType().Name.Length - 2);
            DBareHtml = this.EbSysCreatedAt.GetBareHtml();
            this.Name = "eb_created_at";
        }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.UserControl)]
        [HideInPropertyGrid]
        public override EbDbTypes EbDbType
        {
            get { return this.EbSysCreatedAt.EbDbType; }
            set { this.EbSysCreatedAt.EbDbType = value; }
        }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.UserControl)]
        [PropertyGroup(PGConstants.CORE)]
        public EbDateType EbDateType
        {
            get { return this.EbSysCreatedAt.EbDateType; }
            set { this.EbSysCreatedAt.EbDateType = value; }
        }

        [EnableInBuilder(BuilderType.WebForm)]
        [HideInPropertyGrid]
        public override string InputControlType { get { return "EbSysCreatedAt"; } }

        //[EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.UserControl)]
        //public override bool IsEditable
        //{
        // get { return !this.EbSysCreatedAt.IsDisable; }
        // set { this.EbSysCreatedAt.IsDisable = !value; }
        //}

        [JsonIgnore]
        public override string GetValueFromDOMJSfn
        {
            get
            {
                return this.EbSysCreatedAt.GetValueFromDOMJSfn;
            }
            set { }
        }

        [JsonIgnore]
        public override string SetDisplayMemberJSfn
        {
            get
            {
                return JSFnsConstants.DG_hiddenColCheckCode + this.EbSysCreatedAt.SetDisplayMemberJSfn;
            }
            set { }
        }

        [JsonIgnore]
        public override string SetValueJSfn
        {
            get
            {
                return this.EbSysCreatedAt.SetValueJSfn;
            }
            set { }
        }

        [JsonIgnore]
        public override string EnableJSfn
        {
            get
            {
                return this.EbSysCreatedAt.EnableJSfn;
            }
            set { }
        }

        public override string GetDisplayMemberFromDOMJSfn
        {
            get
            {
                return this.EbSysCreatedAt.GetDisplayMemberFromDOMJSfn;
            }
            set { }
        }

        public override string RefreshJSfn
        {
            get
            {
                return this.EbSysCreatedAt.RefreshJSfn;
            }
            set { }
        }

        public override string ClearJSfn
        {
            get
            {
                return this.EbSysCreatedAt.ClearJSfn;
            }
            set { }
        }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl)]
        [HideInPropertyGrid]
        public override bool DoNotPersist { get { return true; } }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl)]
        [HideInToolBox]
        public override bool IsSysControl { get { return true; } }

        //EbDGCreatedAtColumn
        public override SingleColumn GetSingleColumn(User UserObj, Eb_Solution SoluObj, object Value, bool Default)
        {
            return EbDate.GetSingleColumn(this, UserObj, SoluObj, Value);
        }
        public override DVBaseColumn GetDVBaseColumn(int index)
        {
            return new DVDateTimeColumn { Data = index, Name = this.Name, sTitle = this.Title, sType = "date-uk", Type = this.EbDbType, bVisible = !this.Hidden, sWidth = "100px" };
        }
    }


    [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.UserControl)]
    [Alias("Modified By Column")]
    [UsedWithTopObjectParent(typeof(EbObject))]
    public class EbDGModifiedByColumn : EbDGColumn
    {

        [JsonIgnore]
        [HideInPropertyGrid]
        public EbSysModifiedBy EbSysModifiedBy { get; set; }

        public EbDGModifiedByColumn()
        {
            this.EbSysModifiedBy = new EbSysModifiedBy();
        }

        [OnDeserialized]
        public void OnDeserializedMethod(StreamingContext context)
        {
            this.ObjType = this.GetType().Name.Substring(2, this.GetType().Name.Length - 2);
            DBareHtml = this.EbSysModifiedBy.GetBareHtml();
            this.Name = "eb_lastmodified_by";
        }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.UserControl)]
        [HideInPropertyGrid]
        public override EbDbTypes EbDbType
        {
            get { return this.EbSysModifiedBy.EbDbType; }
            set { this.EbSysModifiedBy.EbDbType = value; }
        }

        [EnableInBuilder(BuilderType.WebForm)]
        [HideInPropertyGrid]
        public override string InputControlType { get { return "EbSysModifiedBy"; } }

        //[EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.UserControl)]
        //public override bool IsEditable
        //{
        // get { return !this.EbSysModifiedBy.IsDisable; }
        // set { this.EbSysModifiedBy.IsDisable = !value; }
        //}

        [JsonIgnore]
        public override string GetValueFromDOMJSfn
        {
            get
            {
                return this.EbSysModifiedBy.GetValueFromDOMJSfn;
            }
            set { }
        }

        [JsonIgnore]
        public override string SetDisplayMemberJSfn
        {
            get
            {
                return JSFnsConstants.DG_hiddenColCheckCode + this.EbSysModifiedBy.SetDisplayMemberJSfn;
            }
            set { }
        }

        [JsonIgnore]
        public override string SetValueJSfn
        {
            get
            {
                return this.EbSysModifiedBy.SetValueJSfn;
            }
            set { }
        }

        [JsonIgnore]
        public override string EnableJSfn
        {
            get
            {
                return this.EbSysModifiedBy.EnableJSfn;
            }
            set { }
        }

        [JsonIgnore]
        public override string GetDisplayMemberFromDOMJSfn
        {
            get
            {
                return this.EbSysModifiedBy.GetDisplayMemberFromDOMJSfn;
            }
            set { }
        }

        public override string RefreshJSfn
        {
            get
            {
                return this.EbSysModifiedBy.RefreshJSfn;
            }
            set { }
        }

        public override string ClearJSfn
        {
            get
            {
                return this.EbSysModifiedBy.ClearJSfn;
            }
            set { }
        }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl)]
        [HideInPropertyGrid]
        public override bool DoNotPersist { get { return true; } }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl)]
        [HideInToolBox]
        public override bool IsSysControl { get { return true; } }

        //EbDGModifiedByColumn
        public override SingleColumn GetSingleColumn(User UserObj, Eb_Solution SoluObj, object Value, bool Default)
        {
            return EbSysCreatedBy.GetSingleColumn(this, UserObj, SoluObj, Value);
        }
    }


    [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.UserControl)]
    [Alias("Modified At Column")]
    [UsedWithTopObjectParent(typeof(EbObject))]
    public class EbDGModifiedAtColumn : EbDGColumn
    {

        [JsonIgnore]
        public EbSysModifiedAt EbSysModifiedAt { get; set; }

        public EbDGModifiedAtColumn()
        {
            this.EbSysModifiedAt = new EbSysModifiedAt();
        }

        [OnDeserialized]
        public void OnDeserializedMethod(StreamingContext context)
        {
            this.ObjType = this.GetType().Name.Substring(2, this.GetType().Name.Length - 2);
            DBareHtml = this.EbSysModifiedAt.GetBareHtml();
            this.Name = "eb_lastmodified_at";
        }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.UserControl)]
        [HideInPropertyGrid]
        public override EbDbTypes EbDbType
        {
            get { return this.EbSysModifiedAt.EbDbType; }
            set { this.EbSysModifiedAt.EbDbType = value; }
        }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.UserControl)]
        [PropertyGroup(PGConstants.CORE)]
        public EbDateType EbDateType
        {
            get { return this.EbSysModifiedAt.EbDateType; }
            set { this.EbSysModifiedAt.EbDateType = value; }
        }

        [EnableInBuilder(BuilderType.WebForm)]
        [HideInPropertyGrid]
        public override string InputControlType { get { return "EbSysModifiedAt"; } }

        //[EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.UserControl)]
        //public override bool IsEditable
        //{
        // get { return !this.EbSysModifiedAt.IsDisable; }
        // set { this.EbSysModifiedAt.IsDisable = !value; }
        //}

        [JsonIgnore]
        public override string GetValueFromDOMJSfn
        {
            get
            {
                return this.EbSysModifiedAt.GetValueFromDOMJSfn;
            }
            set { }
        }

        [JsonIgnore]
        public override string SetDisplayMemberJSfn
        {
            get
            {
                return JSFnsConstants.DG_hiddenColCheckCode + this.EbSysModifiedAt.SetDisplayMemberJSfn;
            }
            set { }
        }

        [JsonIgnore]
        public override string SetValueJSfn
        {
            get
            {
                return this.EbSysModifiedAt.SetValueJSfn;
            }
            set { }
        }

        [JsonIgnore]
        public override string EnableJSfn
        {
            get
            {
                return this.EbSysModifiedAt.EnableJSfn;
            }
            set { }
        }

        public override string GetDisplayMemberFromDOMJSfn
        {
            get
            {
                return this.EbSysModifiedAt.GetDisplayMemberFromDOMJSfn;
            }
            set { }
        }

        public override string RefreshJSfn
        {
            get
            {
                return this.EbSysModifiedAt.RefreshJSfn;
            }
            set { }
        }

        public override string ClearJSfn
        {
            get
            {
                return this.EbSysModifiedAt.ClearJSfn;
            }
            set { }
        }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl)]
        [HideInPropertyGrid]
        public override bool DoNotPersist { get { return true; } }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl)]
        [HideInToolBox]
        [HideInPropertyGrid]
        public override bool IsSysControl { get { return true; } }

        //EbDGModifiedAtColumn
        public override SingleColumn GetSingleColumn(User UserObj, Eb_Solution SoluObj, object Value, bool Default)
        {
            return EbDate.GetSingleColumn(this, UserObj, SoluObj, Value);
        }

        public override DVBaseColumn GetDVBaseColumn(int index)
        {
            return new DVDateTimeColumn { Data = index, Name = this.Name, sTitle = this.Title, sType = "date-uk", Type = this.EbDbType, bVisible = !this.Hidden, sWidth = "100px" };
        }
    }


    [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.UserControl)]
    [Alias("User Select Column")]
    [UsedWithTopObjectParent(typeof(EbObject))]
    public class EbDGUserSelectColumn : EbDGColumn
    {

        [JsonIgnore]
        public EbUserSelect EbUserSelect { get; set; }

        public EbDGUserSelectColumn()
        {
            this.EbUserSelect = new EbUserSelect();
        }

        [OnDeserialized]
        public void OnDeserializedMethod(StreamingContext context)
        {
            this.ObjType = this.GetType().Name.Substring(2, this.GetType().Name.Length - 2);
            DBareHtml = this.EbUserSelect.GetBareHtml();
        }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.UserControl)]
        [HideInPropertyGrid]
        public override EbDbTypes EbDbType
        {
            get { return this.EbUserSelect.EbDbType; }
            set { this.EbUserSelect.EbDbType = value; }
        }


        [EnableInBuilder(BuilderType.WebForm, BuilderType.UserControl)]
        [HideInPropertyGrid]
        public List<UserSelectOption> UserList
        {
            get { return this.EbUserSelect.UserList; }
            set { this.EbUserSelect.UserList = value; }
        }
        public void InitOptions(Dictionary<int, string> Users)
        {
            this.EbUserSelect.InitOptions(Users);
        }

        [EnableInBuilder(BuilderType.WebForm)]
        [HideInPropertyGrid]
        public override string InputControlType { get { return "EbUserSelect"; } }

        public override string GetValueFromDOMJSfn { get { return this.EbUserSelect.GetValueFromDOMJSfn; } }
        public override string OnChangeBindJSFn { get { return @"$(`[ebsid=${p1.DG.EbSid_CtxId }]`).on('change', `[colname=${this.Name}] input[type=hidden]`, p2);"; } set { } }
        public override string SetDisplayMemberJSfn { get { return this.EbUserSelect.SetDisplayMemberJSfn; } }
        public override string SetValueJSfn { get { return @"$('#' + this.EbSid_CtxId + ' input[type=hidden]').data('ctrl_ref', this); this._JsCtrlMng.setValue(p1, p2, false);"; } }
        public override string GetDisplayMemberFromDOMJSfn { get { return this.EbUserSelect.GetDisplayMemberFromDOMJSfn; } }
        public override string RefreshJSfn { get { return this.EbUserSelect.RefreshJSfn; } }
        public override string ClearJSfn { get { return this.EbUserSelect.ClearJSfn; } }

        //EbDGUserSelectColumn
        public override SingleColumn GetSingleColumn(User UserObj, Eb_Solution SoluObj, object Value, bool Default)
        {
            this.EbUserSelect.Name = this.Name;
            return this.EbUserSelect.GetSingleColumn(UserObj, SoluObj, Value, Default);
        }
    }


    [EnableInBuilder(BuilderType.WebForm)]
    [Alias("Label Column")]
    [UsedWithTopObjectParent(typeof(EbObject))]
    public class EbDGLabelColumn : EbDGColumn
    {
        [JsonIgnore]
        public EbLabel EbLabel { get; set; }

        public EbDGLabelColumn()
        {
            this.EbLabel = new EbLabel();
        }

        [OnDeserialized]
        public void OnDeserializedMethod(StreamingContext context)
        {
            DBareHtml = "<div id='@div_ebsid@' class='ebdg-label-link'><input type='hidden' id='@ebsid@' ui-inp><span></span></div>";
        }

        [EnableInBuilder(BuilderType.WebForm)]
        [HideInPropertyGrid]
        public override string InputControlType { get { return "EbLabel"; } }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.UserControl)]
        [HideInPropertyGrid]
        public override EbDbTypes EbDbType { get { return this.EbLabel.EbDbType; } }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.UserControl)]
        [HideInPropertyGrid]
        public override bool DoNotPersist { get { return this.EbLabel.DoNotPersist; } }

        [PropertyGroup(PGConstants.CORE)]
        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.UserControl)]
        [UIproperty]
        [Unique]
        [OnChangeUIFunction("Common.LABEL")]
        [PropertyEditor(PropertyEditorType.MultiLanguageKeySelector)]
        [HelpText("Label for the control to identify it's purpose.")]
        public override string Label
        {
            get { return this.EbLabel.Label; }
            set { this.EbLabel.Label = value; }
        }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.UserControl)]
        [PropertyEditor(PropertyEditorType.ScriptEditorJS, PropertyEditorType.ScriptEditorSQ)]
        [Alias("Text Expression")]
        [HelpText("Expression for label text.")]
        public override EbScript ValueExpr
        {
            get { return this.EbLabel.ValueExpr; }
            set { this.EbLabel.ValueExpr = value; }
        }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.UserControl)]
        [PropertyEditor(PropertyEditorType.ScriptEditorJS, PropertyEditorType.ScriptEditorSQ)]
        [Alias("Default Text Expression")]
        [HelpText("Default expression for label text.")]
        public override EbScript DefaultValueExpression
        {
            get { return this.EbLabel.DefaultValueExpression; }
            set { this.EbLabel.DefaultValueExpression = value; }
        }

        [PropertyGroup(PGConstants.CORE)]
        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.UserControl)]
        public EbLabelRenderType RenderAs
        {
            get { return this.EbLabel.RenderAs; }
            set { this.EbLabel.RenderAs = value; }
        }

        [PropertyGroup(PGConstants.CORE)]
        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.UserControl)]
        public string LinkVersionId
        {
            get { return this.EbLabel.LinkVersionId; }
            set { this.EbLabel.LinkVersionId = value; }
        }

        [PropertyGroup(PGConstants.CORE)]
        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.UserControl)]
        public string LinkDataId
        {
            get { return this.EbLabel.LinkDataId; }
            set { this.EbLabel.LinkDataId = value; }
        }

        [PropertyGroup(PGConstants.CORE)]
        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.UserControl)]
        [PropertyEditor(PropertyEditorType.ObjectSelectorCollection)]
        [OSE_ObjectTypes(EbObjectTypes.iWebForm)]
        public List<ObjectBasicInfo> LinkedObjects
        {
            get { return this.EbLabel.LinkedObjects; }
            set { this.EbLabel.LinkedObjects = value; }
        }


        #region HideInPropertyGrid

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.UserControl)]
        [HideInPropertyGrid]
        public override string Info { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.UserControl)]
        [HideInPropertyGrid]
        public override string InfoIcon { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.UserControl)]
        [HideInPropertyGrid]
        public override string HelpText { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.UserControl)]
        [HideInPropertyGrid]
        public override string ToolTipText { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.UserControl)]
        [HideInPropertyGrid]
        public override bool Required { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.UserControl)]
        [HideInPropertyGrid]
        public override bool Unique { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.UserControl)]
        [HideInPropertyGrid]
        public override List<EbValidator> Validators { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.UserControl)]
        [HideInPropertyGrid]
        public override bool SelfTrigger { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.UserControl)]
        [HideInPropertyGrid]
        public override EbScript OnChangeFn { get; set; }

        #endregion

        [JsonIgnore]
        public override string SetDisplayMemberJSfn
        {
            get { return JSFnsConstants.DG_hiddenColCheckCode + @"
{
let ele = document.getElementById(this.EbSid_CtxId);
ele.value = p1;
$(ele).siblings('span').text(p1);
$(ele).closest('td').find('.tdtxt span').text(p1);
}"; }

            set { }
        }
    }

}