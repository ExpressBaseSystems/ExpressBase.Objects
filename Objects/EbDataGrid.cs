using ExpressBase.Common;
using ExpressBase.Common.Extensions;
using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using ExpressBase.Common.Structures;
using ExpressBase.Objects.Helpers;
using ExpressBase.Objects.Objects.DVRelated;
using ExpressBase.Objects.ServiceStack_Artifacts;
using Newtonsoft.Json;
using ServiceStack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace ExpressBase.Objects
{
    [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.UserControl)]
    public class EbDataGrid : EbControlContainer
    {
        public EbDataGrid()
        {
            this.Controls = new List<EbControl>();
        }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.UserControl)]
        [DefaultPropValue("200")]
        public override int Height { get; set; }

        [JsonIgnore]
        public override string OnChangeBindJSFn
        {
            get
            {
                return @"
$.each(this.Controls.$values, function (i, col) {
    col.bindOnChange({form:this.formObject, col:col, DG:this, user : this.__userObject});
}.bind(this));
               ";
            }
            set { }
        }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.UserControl)]
        public override bool IsSpecialContainer { get { return true; } set { } }

        [OnDeserialized]
        public void OnDeserializedMethod(StreamingContext context)
        {
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

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.UserControl)]
        [PropertyEditor(PropertyEditorType.Collection)]
        [Alias("Columns")]
        [ListType(typeof(EbDGColumn))]
        public override List<EbControl> Controls { get; set; }

        //[EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.UserControl)]
        //[PropertyGroup("Behavior")]
        //[DefaultPropValue("true")]
        //public bool IsEditable { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.UserControl)]
        [PropertyGroup("Behavior")]
        [DefaultPropValue("true")]
        public bool IsAddable { get; set; }

        public override string GetToolHtml()
        {
            return @"<div eb-type='@toolName' class='tool'><i class='fa fa-table'></i>  @toolName</div>".Replace("@toolName", this.GetType().Name.Substring(2));
        }
        public override string GetBareHtml()
        {
            string html = @"
<div class='grid-cont'>
    <div class='Dg_head'>
        <table id='tbl_@ebsid@_head' class='table table-bordered dgtbl'>
            <thead>
              <tr>";
            foreach (EbDGColumn col in Controls)
            {
                if (!col.Hidden)
                    html += string.Concat("<th style='width: @Width@; @bg@' title='", col.Title, "'><span class='grid-col-title'>", col.Title, "</span>@req@</th>")
                        .Replace("@req@", (col.Required ? "<sup style='color: red'>*</sup>" : string.Empty))
                        .Replace("@Width@", (col.Width <= 0) ? "auto" : col.Width.ToString() + "%")
                        .Replace("@bg@", col.IsDisable ? "background-color:#fafafa; color:#555" : string.Empty);
            }

            html += @"
                @cogs@
              </tr>
            </thead>
        </table>
    </div>".Replace("@cogs@", !this.IsDisable ? "<th style='width:50px;'><span class='fa fa-cogs'></span></th>" : string.Empty);

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
</div>".Replace("@_height@", this.Height.ToString());

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
        public override string OnChangeBindJSFn
        {
            get
            {
                return @"
                if (p1.col.OnChangeFn && p1.col.OnChangeFn.Code && p1.col.OnChangeFn.Code.trim() !== ''){


                  $(`[ebsid=${p1.DG.EbSid}]`).on('change', `[colname=${this.Name}] [ui-inp]`, new Function('form', 'user', `event`, atob(p1.col.OnChangeFn.Code)).bind(this, p1.form, p1.user));
                }; ";
            }
            set { }
        }

        [JsonIgnore]
        public override string SetValueJSfn
        {
            get { return @" $('[ebsid='+this.__DG.EbSid+']').find(`tr[rowid=${this.__rowid}] [colname=${this.Name}] [ui-inp]`).val(p1).trigger('change'); "; }

            set { }
        }

        [JsonIgnore]
        public override string GetValueJSfn
        {
            get { return @" return $('[ebsid='+this.__DG.EbSid+']').find(`tr[rowid=${this.__rowid}] [colname=${this.Name}] [ui-inp]`).val(); "; }

            set { }
        }

        [JsonIgnore]
        public override string EnableJSfn { get { return @"$('[ebsid='+this.__DG.EbSid+']').find(`tr[rowid=${this.__rowid}] [colname=${this.Name}] .ctrl-cover *`).prop('disabled',false).css('pointer-events', 'inherit').find('input').css('background-color','#fff');"; } set { } }

        [JsonIgnore]
        public override string DisableJSfn { get { return @"$('[ebsid='+this.__DG.EbSid+']').find(`tr[rowid=${this.__rowid}] [colname=${this.Name}] .ctrl-cover *`).attr('disabled', 'disabled').css('pointer-events', 'none').find('input').css('background-color','#eee');"; } set { } }

        [JsonIgnore]
        public override string ClearJSfn { get { return @"$('[ebsid='+this.__DG.EbSid+']').find(`tr[rowid=${this.__rowid}] [colname=${this.Name}] [ui-inp]`).val('');"; } set { } }

        [JsonIgnore]
        public override string HideJSfn { get { return @""; } set { } }

        [JsonIgnore]
        public override string ShowJSfn { get { return @""; } set { } }


        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.UserControl)]
        public string Title { get; set; }

        [HideInPropertyGrid]
        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.UserControl)]
        public string DBareHtml { get; set; }

        [EnableInBuilder(BuilderType.WebForm)]
        public virtual string InputControlType { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.UserControl)]
        public bool IsDisable { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.UserControl)]
        [DefaultPropValue("true")]
        public bool IsEditable { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.UserControl)]
        [PropertyGroup("Appearance")]
        public virtual int Width { get; set; }
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
        public TextMode TextMode
        {
            get { return this.EbTextBox.TextMode; }
            set { this.EbTextBox.TextMode = value; }
        }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.UserControl)]
        public override string ToolTipText
        {
            get { return this.EbTextBox.ToolTipText; }
            set { this.EbTextBox.ToolTipText = value; }
        }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.UserControl)]
        [HideInPropertyGrid]
        public override EbDbTypes EbDbType { get { return EbDbTypes.String; } }

        [EnableInBuilder(BuilderType.WebForm)]
        public override string InputControlType { get { return "EbTextBox"; } }

        [OnDeserialized]
        public void OnDeserializedMethod(StreamingContext context)
        {
            DBareHtml = EbTextBox.GetBareHtml();
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
        public override string InputControlType { get { return "EbNumeric"; } }

        [EnableInBuilder(BuilderType.WebForm)]
        public bool IsAggragate { get; set; }

        [EnableInBuilder(BuilderType.WebForm)]
        public bool AllowNegative { get; set; }
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
        public override string InputControlType { get { return "EbCheckBox"; } }
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
        public EbDateType EbDateType
        {
            get { return this.EbDate.EbDateType; }
            set { this.EbDate.EbDateType = value; }
        }

        [JsonIgnore]
        public override string GetValueJSfn
        {
            get
            {
                return this.EbDate.GetValueJSfn;
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
        public override string GetDisplayMemberJSfn
        {
            get
            {
                return this.EbDate.GetDisplayMemberJSfn;
            }
            set { }
        }

        [JsonIgnore]
        public override string OnChangeBindJSFn { get { return @"
if(p1.col.OnChangeFn.Code === null)
    return;
let func =new Function('form', 'user', `event`, atob(p1.col.OnChangeFn.Code)).bind(this, p1.form, p1.user);
$(`[ebsid=${p1.DG.EbSid}]`).on('change', `[colname=${this.Name}] [ui-inp]`, func).siblings('.nullable-check').on('change', `input[type=checkbox]`, func);"; } set { } }

        [EnableInBuilder(BuilderType.WebForm)]
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
        public bool IsNullable
        {
            get { return this.EbDate.IsNullable; }
            set { this.EbDate.IsNullable = value; }
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

        public override string GetDisplayMemberJSfn { get { return @" return $('[ebsid='+this.__DG.EbSid+']').find(`tr[rowid=${this.__rowid}] [colname=${this.Name}] [ui-inp] :selected`).text(); "; } set { } }

        [EnableInBuilder(BuilderType.WebForm)]
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

        [EnableInBuilder(BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl)]
        [HideInPropertyGrid]
        public DVColumnCollection Columns
        {
            get { return this.EbSimpleSelect.Columns; }
            set { this.EbSimpleSelect.Columns = value; }
        }

        [EnableInBuilder(BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.WebForm, BuilderType.UserControl)]
        [PropertyEditor(PropertyEditorType.CollectionFrmSrc, "Columns", 1)]
        [OnChangeExec(@"if (this.Columns && this.Columns.$values.length === 0 ){pg.MakeReadOnly('ValueMember');} else {pg.MakeReadWrite('ValueMember');}")]
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
        [OnChangeExec(@"if (this.Columns && this.Columns.$values.length === 0 ){pg.MakeReadOnly('DisplayMember');} else {pg.MakeReadWrite('DisplayMember');}")]
        public DVBaseColumn DisplayMember
        {
            get { return this.EbSimpleSelect.DisplayMember; }
            set { this.EbSimpleSelect.DisplayMember = value; }
        }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl)]
        public int Value
        {
            get { return this.EbSimpleSelect.Value; }
            set { this.EbSimpleSelect.Value = value; }
        }

        [HideInPropertyGrid]
        [EnableInBuilder(BuilderType.BotForm)]
        public override bool IsReadOnly
        {
            get { return this.EbSimpleSelect.IsReadOnly; }
            set { this.EbSimpleSelect.IsReadOnly = value; }
        }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl)]
        [PropertyEditor(PropertyEditorType.Boolean)]
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
        <input id='' ui-inp data-toggle='tooltip' title='' type='text' tabindex='0' style='width:100%; data-original-title=''>
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
    public class EbDGPowerSelectColumn : EbDGColumn
    {
        public bool MultiSelect { get { return this.EbPowerSelect.MultiSelect; } set { this.EbPowerSelect.MultiSelect = value; } }

        [JsonIgnore]
        public override string SetDisplayMemberJSfn { get { return this.EbPowerSelect.SetDisplayMemberJSfn; } set { } }

        [JsonIgnore]
        private EbPowerSelect EbPowerSelect { get; set; }

        public EbDGPowerSelectColumn()
        {
            this.EbPowerSelect = new EbPowerSelect();
        }

        public override string SetValueJSfn
        {
            get
            {
                return @"
                     this.initializer.setValues(p1);
                ";
            }
            set { }
        }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.UserControl)]
        [PropertyGroup("Appearance")]
        [DefaultPropValue("100")]
        public override int Width { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm)]
        [PropertyEditor(PropertyEditorType.ObjectSelector)]
        [OSE_ObjectTypes(EbObjectTypes.iDataReader)]
        public string DataSourceId { get { return this.EbPowerSelect.DataSourceId; } set { this.EbPowerSelect.DataSourceId = value; } }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl)]
        [DefaultPropValue("1")]
        public int MaxLimit { get { return this.EbPowerSelect.MaxLimit; } set { this.EbPowerSelect.MaxLimit = value; } }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl)]
        public int MinLimit { get { return this.EbPowerSelect.MaxLimit; } set { this.EbPowerSelect.MinLimit = value; } }

        [EnableInBuilder(BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.WebForm)]
        [PropertyEditor(PropertyEditorType.CollectionProp, "Columns", "bVisible")]
        public DVColumnCollection Columns { get { return this.EbPowerSelect.Columns; } set { this.EbPowerSelect.Columns = value; } }

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

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm)]
        [PropertyEditor(PropertyEditorType.CollectionFrmSrc, "Columns")]
        public DVColumnCollection DisplayMembers { get { return this.EbPowerSelect.DisplayMembers; } set { this.EbPowerSelect.DisplayMembers = value; } }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm)]
        public int DropdownHeight { get { return this.EbPowerSelect.DropdownHeight; } set { this.EbPowerSelect.DropdownHeight = value; } }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm)]
        [Alias("DropdownWidth(%)")]
        [DefaultPropValue("100")]
        public int DropdownWidth { get { return this.EbPowerSelect.DropdownWidth; } set { this.EbPowerSelect.DropdownWidth = value; } }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm)]
        [PropertyEditor(PropertyEditorType.CollectionFrmSrc, "Columns", 1)]
        [OnChangeExec(@"if (
this.Columns.$values.length === 0 ){
pg.MakeReadOnly('ValueMember');} 
else {pg.MakeReadWrite('ValueMember');}")]
        public DVBaseColumn ValueMember { get { return this.EbPowerSelect.ValueMember; } set { this.EbPowerSelect.ValueMember = value; } }

        [EnableInBuilder(BuilderType.WebForm)]
        public override string InputControlType { get { return "EbPowerSelect"; } }

        public override string GetBareHtml()
        {
            return this.EbPowerSelect.GetBareHtml();
        }

        public string GetSelectQuery(Service service, string Col, string Tbl = null, string _id = null)
        {
            return this.EbPowerSelect.GetSelectQuery(service, Col, Tbl, _id);
        }

        public string GetDisplayMembersQuery(Service service, string vms)
        {
            return this.EbPowerSelect.GetDisplayMembersQuery(service, vms);
        }
    }
}
