using ExpressBase.Common;
using ExpressBase.Common.Extensions;
using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using ExpressBase.Common.Structures;
using ExpressBase.Objects.Helpers;
using ExpressBase.Objects.Objects.DVRelated;
using System;
using System.Collections.Generic;
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

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.UserControl)]
        [PropertyGroup("test")]
        public bool IsEditable { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.UserControl)]
        [PropertyGroup("test")]
        public bool IsAddable { get; set; }

        public override string GetToolHtml()
        {
            return @"<div eb-type='@toolName' class='tool'><i class='fa fa-table'></i>  @toolName</div>".Replace("@toolName", this.GetType().Name.Substring(2));
        }
        public override string GetBareHtml()
        {
            string html = @"
<div class='grid-cont'>
    <table id='tbl_@ebsid@' class='table table-bordered dgtbl'>
        <thead>
          <tr>";
            foreach (EbDGColumn col in Controls)
            {
                html += string.Concat("<th>", col.Title, "</th>");
            }

            html += @"
            <th><span class='fa fa-cogs'></span></th>
          </tr>
        </thead>
    </thead>
    <tbody>";

            html += @"
    </tbody>
    </table>
</div>";
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
        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.UserControl)]
        public string Title { get; set; }

        public virtual string InputControlType { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.UserControl)]
        public bool IsEditable { get; set; }

    }

    [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.UserControl)]
    [Alias("String Column")]
    [UsedWithTopObjectParent(typeof(EbObject))]
    public class EbDGStringColumn : EbDGColumn
    {
        [HideInPropertyGrid]
        public override EbDbTypes EbDbType { get { return EbDbTypes.String; } }

        public override string InputControlType { get { return "EbTextBox"; } }
    }

    [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.UserControl)]
    [Alias("Numeric Column")]
    [UsedWithTopObjectParent(typeof(EbObject))]
    public class EbDGNumericColumn : EbDGColumn
    {
        [HideInPropertyGrid]
        public override EbDbTypes EbDbType { get { return EbDbTypes.Decimal; } }

        public override string InputControlType { get { return "EbNumeric"; } }
    }

    [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.UserControl)]
    [Alias("Boolean Column")]
    [UsedWithTopObjectParent(typeof(EbObject))]
    public class EbDGBooleanColumn : EbDGColumn
    {
        [HideInPropertyGrid]
        public override EbDbTypes EbDbType { get { return EbDbTypes.Boolean; } }

        public override string InputControlType { get { return "EbCheckBox"; } }
    }

    [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.UserControl)]
    [Alias("Date Column")]
    [UsedWithTopObjectParent(typeof(EbObject))]
    public class EbDGDateColumn : EbDGColumn
    {
        [HideInPropertyGrid]
        public override EbDbTypes EbDbType { get { return EbDbTypes.Date; } }

        public override string InputControlType { get { return "EbDate"; } }
    }

    [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog)]
    [Alias("PowerSelect Column")]
    [UsedWithTopObjectParent(typeof(EbObject))]
    public class EbDGPowerSelectColumn : EbDGColumn
    {
        public bool MultiSelect { get; set; }

        private EbPowerSelect EbPowerSelect { get; set; }

        public EbDGPowerSelectColumn()
        {
            this.EbPowerSelect = new EbPowerSelect();
        }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm)]
        [PropertyEditor(PropertyEditorType.ObjectSelector)]
        [OSE_ObjectTypes(EbObjectTypes.iDataReader)]
        public string DataSourceId
        {
            get { return this.EbPowerSelect.DataSourceId; }
            set { this.EbPowerSelect.DataSourceId = value; }
        }

        public override string GetBareHtml()
        {
            return this.EbPowerSelect.GetBareHtml();
        }

        public string DBareHtml { get; internal set; }

        [EnableInBuilder(BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.WebForm)]
        [PropertyEditor(PropertyEditorType.CollectionProp, "Columns", "bVisible")]
        public DVColumnCollection Columns
        {
            get { return this.EbPowerSelect.Columns; }
            set { this.EbPowerSelect.Columns = value; }
        }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm)]
        [PropertyEditor(PropertyEditorType.CollectionFrmSrc, "Columns")]
        public override string Name
        {
            get { return this.EbPowerSelect.Name; }
            set { this.EbPowerSelect.Name = value; }
        }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm)]
        [PropertyEditor(PropertyEditorType.CollectionFrmSrc, "Columns")]
        public override string EbSid
        {
            get { return this.EbPowerSelect.EbSid; }
            set { this.EbPowerSelect.EbSid = value; }
        }

        [HideInPropertyGrid]
        public override EbDbTypes EbDbType
        {
            get { return this.EbPowerSelect.EbDbType; }
            set { this.EbPowerSelect.EbDbType = value; }
        }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm)]
        [PropertyEditor(PropertyEditorType.CollectionFrmSrc, "Columns")]
        new public string EbSid_CtxId
        {
            get { return this.EbPowerSelect.EbSid_CtxId; }
            set { this.EbPowerSelect.EbSid_CtxId = value; }
        }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm)]
        [PropertyEditor(PropertyEditorType.CollectionFrmSrc, "Columns")]
        public DVColumnCollection DisplayMembers
        {
            get { return this.EbPowerSelect.DisplayMembers; }
            set { this.EbPowerSelect.DisplayMembers = value; }
        }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm)]
        [PropertyEditor(PropertyEditorType.CollectionFrmSrc, "Columns", 1)]
        [OnChangeExec(@"if (
this.Columns.$values.length === 0 ){
pg.MakeReadOnly('ValueMember');} 
else {pg.MakeReadWrite('ValueMember');}")]
        public DVBaseColumn ValueMember
        {
            get { return this.EbPowerSelect.ValueMember; }
            set { this.EbPowerSelect.ValueMember = value; }
        }

        public override string InputControlType { get { return "EbPowerSelect"; } }
    }
}
