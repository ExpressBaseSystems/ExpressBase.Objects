using ExpressBase.Common;
using ExpressBase.Common.Extensions;
using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using ExpressBase.Common.Structures;
using ExpressBase.Objects.Helpers;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace ExpressBase.Objects
{
    [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog)]
    public class EbDataGrid : EbControlContainer
    {
        public EbDataGrid()
        {
            this.Controls = new List<EbControl>();
        }

        [OnDeserialized]
        public void OnDeserializedMethod(StreamingContext context)
        {
            this.BareControlHtml = this.GetBareHtml();
            this.ObjType = this.GetType().Name.Substring(2, this.GetType().Name.Length - 2);
        }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog)]
        [PropertyEditor(PropertyEditorType.Collection)]
        [Alias("Columns")]
        [ListType(typeof(EbDGColumn))]
        public override List<EbControl> Controls { get; set; }

        public List<List<SingleRecordField>> Rows { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog)]
        [PropertyGroup("test")]
        public bool IsEditable { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog)]
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
            // need optimization
            if (Rows != null)
            {
                foreach (List<SingleRecordField> row in Rows)
                {
                    //html += @"<tr added='false' row-id='" + +"'>";// include rowid
                    foreach (SingleRecordField td in row)
                    {
                        html += string.Concat("<td>", td.Value, "</td>");
                    }
                    html += @"</tr>";
                }
            }
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
    public abstract class EbDGColumn : EbControl
    {
        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog)]
        public string Title { get; set; }

        public virtual string InputControlType { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog)]
        public bool IsEditable { get; set; }

    }

    [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog)]
    [Alias("String Column")]
    [UsedWithTopObjectParent(typeof(EbObject))]
    public class EbDGStringColumn : EbDGColumn
    {
        [HideInPropertyGrid]
        public override EbDbTypes EbDbType { get { return EbDbTypes.String; } }

        public override string InputControlType { get { return "EbTextBox"; } }
    }

    [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog)]
    [Alias("Numeric Column")]
    [UsedWithTopObjectParent(typeof(EbObject))]
    public class EbDGNumericColumn : EbDGColumn
    {
        [HideInPropertyGrid]
        public override EbDbTypes EbDbType { get { return EbDbTypes.Decimal; } }

        public override string InputControlType { get { return "EbNumeric"; } }
    }

    [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog)]
    [Alias("Boolean Column")]
    [UsedWithTopObjectParent(typeof(EbObject))]
    public class EbDGBooleanColumn : EbDGColumn
    {
        [HideInPropertyGrid]
        public override EbDbTypes EbDbType { get { return EbDbTypes.Boolean; } }

        public override string InputControlType { get { return "EbCheckBox"; } }
    }

    [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog)]
    [Alias("Date Column")]
    [UsedWithTopObjectParent(typeof(EbObject))]
    public class EbDGDateColumn : EbDGColumn
    {
        [HideInPropertyGrid]
        public override EbDbTypes EbDbType { get { return EbDbTypes.Date; } }

        public override string InputControlType { get { return "EbDate"; } }
    }
}
