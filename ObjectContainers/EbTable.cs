using ExpressBase.Common.Extensions;
using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using ExpressBase.Objects;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace ExpressBase.Objects
{
    [ProtoBuf.ProtoContract]
    [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog)]
    public class EbTableLayout : EbControlContainer
    {
        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog)]
        [PropertyEditor(PropertyEditorType.Collection)]
        [Alias("Columns")]
        public override List<EbControl> Controls { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog)]
        [PropertyEditor(PropertyEditorType.Expandable)]
        [PropertyGroup("Test")]
        public Position Position { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog)]
        [PropertyEditor(PropertyEditorType.Collection)]
        [PropertyGroup("Test")]
        public List<EbTableTd> CollEdtProp { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog)]
        [PropertyEditor(PropertyEditorType.CollectionFrmSrc)]
        [PropertyGroup("Test")]
        public List<EbTableTd> CollFrmSrc { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog)]
        [PropertyEditor(PropertyEditorType.CollectionFrmSrcPG)]
        [PropertyGroup("Test")]
        public List<EbTableTd> ColumnsR { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog)]
        [PropertyEditor(PropertyEditorType.String)]
        [PropertyGroup("Test")]
        public string doc { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog)]
        [PropertyEditor(PropertyEditorType.ImageSeletor)]
        [PropertyGroup("Test")]
        public string AImg { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog)]
        [PropertyEditor(PropertyEditorType.CollectionA2C)]
        [PropertyGroup("Test")]
        public List<EbTableTd> propA2C { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog)]
        [PropertyEditor(PropertyEditorType.Collection)]
        [PropertyGroup("Test")]
        public List<EbDataVisualization> Visualizations { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog)]
        [PropertyEditor(PropertyEditorType.ObjectSelector)]
        [PropertyGroup("Test")]
        [OSE_ObjectTypes(EbObjectType.DataVisualization, EbObjectType.Report, EbObjectType.MobileForm, EbObjectType.TableVisualization)]
        public string ObjectSelectorProp { get; set; }

        public EbTableLayout()
        {
            this.Controls = new List<EbControl>();
            this.Visualizations = new List<EbDataVisualization>();
        }

        public override string GetDesignHtml()
        {
            return @"
<div class='Eb-ctrlContainer' Ctype='TableLayout'>
    <table style='width:100%'   style=' @BackColor  @ForeColor ' >
        <tr>
            <td id='@id_Td0' class='tdDropable' ></td>
            <td id='@id_Td1' class='tdDropable'></td style='min-height:20px;'>
        </tr>
    </table>
</div>"
    .Replace("@BackColor ", ("background-color:" + ((this.BackColor != null) ? this.BackColor : "@BackColor ") + ";"))
    .Replace("@ForeColor ", "color:" + ((this.ForeColor != null) ? this.ForeColor : "@ForeColor ") + ";").RemoveCR().DoubleQuoted();
        }

        public override string GetJsInitFunc()
        {
            return @"
this.Init = function(id)
{
    this.Controls.Append(new EbObjects.EbTableTd(id + '_Td0'));
    this.Controls.Append(new EbObjects.EbTableTd(id + '_Td1'));
    this.Controls.Append(new EbObjects.EbTableTd(id + '_Td2'));

    this.CollEdtProp.push(new EbObjects.EbTableTd(id + '_Tdsamp0'));
    this.CollEdtProp.push(new EbObjects.EbTableTd(id + '_Tdsamp1'));
    this.CollEdtProp.push(new EbObjects.EbTableTd(id + '_Tdsamp2'));
};";
        }

        public override string GetHead()
        {
            string head = string.Empty;

            if (base.Controls != null)
            {
                foreach (EbControl ec in base.Controls)
                    head += ec.GetHead();
            }

            return head;
        }

        public override string GetHtml()
        {
            string html = @"
            <div class='Eb-ctrlContainer' Ctype='TableLayout'>
                <table class='form-render-table' ><tr>";

            foreach (EbControl ec in this.Controls)
                html += ec.GetHtml();

            return html + "</tr></table></div>";
        }
    }

    [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog)]
    [HideInToolBox]
    public class EbTableTd : EbControlContainer
    {
        public EbTableTd()
        {
            this.Controls = new List<EbControl>();
        }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog)]
        [HideInPropertyGrid]
        public override List<EbControl> Controls { get; set; }

        public override string GetHead()
        {
            string head = string.Empty;

            if (base.Controls != null)
            {
                foreach (EbControl ec in base.Controls)
                    head += ec.GetHead();
            }

            return head;
        }

        public override string GetHtml()
        {
            string html = "<td class='form-render-table-Td tdDropable'>";

            foreach (EbControl ec in this.Controls)
                html += ec.GetHtml();

            return html + "</td>";
        }
    }
}

[EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog)]
public class Position
{

    [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog)]
    [PropertyEditor(PropertyEditorType.Number)]
    public int X { get; set; }

    [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog)]
    [PropertyEditor(PropertyEditorType.Number)]
    public int Y { get; set; }

    public Position() { }



    public string GetHtml()
    {
        return "";
    }
}