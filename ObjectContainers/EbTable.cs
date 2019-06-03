using ExpressBase.Common.Extensions;
using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using ExpressBase.Common.Structures;
using ExpressBase.Objects;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace ExpressBase.Objects
{
    [EnableInBuilder(BuilderType.WebForm, BuilderType.UserControl)]
    public class EbTableLayout : EbControlContainer
    {

        public override string UIchangeFns
        {
            get
            {
                return @"EbTable = {
                padding : function(elementId, props) {
                    $(`#cont_${ elementId}>table>tbody>tr>td`).css('padding', `${props.Padding.Top}px ${props.Padding.Right}px ${props.Padding.Bottom}px ${props.Padding.Left}px`);
                }
            }";
            }
        }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.UserControl)]
        [PropertyEditor(PropertyEditorType.Collection)]
        [Alias("Columns")]
        [ListType(typeof(EbTableTd))]
        public override List<EbControl> Controls { get; set; }

        [JsonIgnore]
        public override string Label { get; set; }

        [JsonIgnore]
        public override string BackColor { get; set; }

        [JsonIgnore]
        public override string ForeColor { get; set; }

        [JsonIgnore]
        public override string LabelBackColor { get; set; }

        [JsonIgnore]
        public override string LabelForeColor { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.UserControl)]
        [PropertyEditor(PropertyEditorType.Expandable)]
        [OnChangeUIFunction("EbTable.padding")]
        public new  UISides Padding { get; set; }

        public EbTableLayout()
        {
            this.Controls = new List<EbControl>();
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
this.Init = function(id){
    this.Controls.$values.push(new EbObjects.EbTableTd(id + '_Td0'));
    this.Controls.$values.push(new EbObjects.EbTableTd(id + '_Td1'));
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
            <div id='cont_@ebsid@' ebsid='@ebsid@' class='Eb-ctrlContainer' Ctype='TableLayout'>
                <table id='@ebsid@' class='form-render-table' ><tr>";

            foreach (EbControl ec in this.Controls)
                html += ec.GetHtml();

            return (html + "</tr></table></div>").Replace("@name@", this.Name).Replace("@ebsid@", this.EbSid_CtxId);
        }
    }

    [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.UserControl)]
    [HideInToolBox]
    public class EbTableTd : EbControlContainer
    {
        public EbTableTd()
        {
            this.Controls = new List<EbControl>();
            this.ObjType = this.GetType().Name.Substring(2, this.GetType().Name.Length - 2);
        }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.UserControl)]
        public float WidthPercentage { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.UserControl)]
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
            string html = "<td id='@name@' ebsid='@ebsid@' style='width:@wperc@;'; class='form-render-table-Td tdDropable ebcont-ctrl'> <div style='height: 100%; width: 100%; min-height: 30px;'>";

            foreach (EbControl ec in this.Controls)
                html += ec.GetHtml();

            return (html + "</div></td>")
                .Replace("@name@", this.Name)
                .Replace("@wperc@", (this.WidthPercentage != 0) ? this.WidthPercentage.ToString() + "%" : "auto")
                .Replace("@ebsid@", this.EbSid);
        }
    }

    [EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
    public class UISides4Cont : UISides
    {
        public UISides4Cont() { }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
        [DefaultPropValue(0)]
        public override int Top { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
        [DefaultPropValue(0)]
        public override int Right { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
        [DefaultPropValue(0)]
        public override int Bottom { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
        [DefaultPropValue(0)]
        public override int Left { get; set; }
    }
}

[EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.UserControl)]
public class Position
{

    [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.UserControl)]
    [PropertyEditor(PropertyEditorType.Number)]
    public int X { get; set; }

    [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.UserControl)]
    [PropertyEditor(PropertyEditorType.Number)]
    public int Y { get; set; }

    public Position() { }



    public string GetHtml()
    {
        return "";
    }
}