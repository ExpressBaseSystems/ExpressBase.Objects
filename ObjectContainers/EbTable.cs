using ExpressBase.Common.Extensions;
using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using ExpressBase.Common.Structures;
using ExpressBase.Objects;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace ExpressBase.Objects
{
    [EnableInBuilder(BuilderType.WebForm)]
    public class EbTableLayout : EbControlContainer
    {

        public override string UIchangeFns
        {
            get
            {
                return @"EbTable = {
                padding : function(elementId, props) {
                    $(`#cont_${ elementId}>table>tbody>tr>td`).css('padding', props.Padding + 'px');
                }
            }";
            }
        }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog)]
        [PropertyEditor(PropertyEditorType.Collection)]
        [Alias("Columns")]
        [ListType(typeof(EbTableTd))]
        public override List<EbControl> Controls { get; set; }

        public override string Label { get; set; }

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
        [PropertyGroup("Test")]
        [OnChangeUIFunction("EbTable.padding")]
        [DefaultPropValue("3")]
        [UIproperty]
        public int Padding { get; set; }

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
        [DefaultPropValue("eb_roby_dev-eb_roby_dev-3-671-1325")]
        //[OSE_ObjectTypes(EbObjectTypes.iDataVisualization, EbObjectTypes.iReport, EbObjectTypes.iMobileForm, EbObjectTypes.iTableVisualization)]
        public string ObjectSelectorProp { get; set; }

        public EbTableLayout()
        {
            this.Controls = new List<EbControl>();
            this.Visualizations = new List<EbDataVisualization>();
            this.ObjType = this.GetType().Name.Substring(2, this.GetType().Name.Length - 2);
        }

        public override string GetDesignHtml()
        {
            this.Controls = new List<EbControl>();
            this.Controls.Add(new EbTableTd { Name = "EbTable0_Td0" });
            this.Controls.Add(new EbTableTd { Name = "EbTable0_Td1" });
            return GetHtml().RemoveCR().DoubleQuoted();
        }

        public override string GetJsInitFunc()
        {
            return @"
this.Init = function(id)
{
    this.Controls.$values.push(new EbObjects.EbTableTd(id + '_Td0'));
    this.Controls.$values.push(new EbObjects.EbTableTd(id + '_Td1'));
    //this.Controls.$values.push(new EbObjects.EbTableTd(id + '_Td2'));

    this.CollEdtProp.$values.push(new EbObjects.EbTableTd(id + '_Tdsamp0'));
    this.CollEdtProp.$values.push(new EbObjects.EbTableTd(id + '_Tdsamp1'));
    //this.CollEdtProp.$values.push(new EbObjects.EbTableTd(id + '_Tdsamp2'));
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
            <div id='@name@' ebsid='@ebsid@' class='Eb-ctrlContainer' Ctype='TableLayout'>
                <table class='form-render-table' ><tr>";

            foreach (EbControl ec in this.Controls)
                html += ec.GetHtml();

            return (html + "</tr></table></div>").Replace("@name@", this.Name).Replace("@ebsid@", this.EbSid);
        }
    }

    [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog)]
    [HideInToolBox]
    public class EbTableTd : EbControlContainer
    {
        public EbTableTd()
        {
            this.Controls = new List<EbControl>();
            this.ObjType = this.GetType().Name.Substring(2, this.GetType().Name.Length - 2);
        }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog)]
        public float WidthPercentage { get; set; }

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
            string html = "<td id='@name@' ebsid='@ebsid@' style='width:@wperc@;'; class='form-render-table-Td tdDropable ebcont-ctrl'>";

            foreach (EbControl ec in this.Controls)
                html += ec.GetHtml();

            return (html + "</td>")
                .Replace("@name@", this.Name)
                .Replace("@wperc@", (this.WidthPercentage != 0) ? this.WidthPercentage.ToString() + "%" : "auto")
                .Replace("@ebsid@", this.EbSid);
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