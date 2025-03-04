﻿using ExpressBase.Common;
using ExpressBase.Common.Constants;
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
    public enum VerticalAlign
    {
        Top = 0,
        Middle = 1,
        Bottom = 2
    }

    [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.UserControl)]
    public class EbTableLayout : EbControlContainer
    {

        public override string UIchangeFns
        {
            get
            {
                return @"EbTable = {
                padding : function(elementId, props) {
                    $(`#cont_${ elementId}>table>tbody>tr>td`).css('padding', `${props.Padding.Top}px ${props.Padding.Right}px ${props.Padding.Bottom}px ${props.Padding.Left}px`);
                },
                verticalAlign : function(elementId, props) {
                     $(`[ebsid=${props.EbSid}].form-render-table-Td>.tdInnerDiv`).css('justify-content',(props.VerticalAlign === 0 ? 'flex-start' : (props.VerticalAlign === 1 ? 'center': 'flex-end')));
                }
            }";
            }
        }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.UserControl)]
        [PropertyEditor(PropertyEditorType.Collection)]
        [Alias("Columns")]
        [PropertyGroup("Core")]
        [PropertyPriority(70)]
        [ListType(typeof(EbTableTd))]
        public override List<EbControl> Controls { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.UserControl)]
        [HideInPropertyGrid]
        public override string TableName { get; set; }

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

        [JsonIgnore]
        public override EbScript OnChangeFn { get; set; }

        [JsonIgnore]
        public override List<string> DependedValExp { get; set; }

        [JsonIgnore]
        public override List<string> DrDependents { get; set; }

        [JsonIgnore]
        public override List<string> HiddenExpDependants { get; set; }

        [JsonIgnore]
        public override List<string> DisableExpDependants { get; set; }

        [JsonIgnore]
        public override List<string> ValExpParams { get; set; }

        [JsonIgnore]
        public override List<string> DependedDG { get; set; }

        [JsonIgnore]
        public override string DBareHtml { get; set; }

        [JsonIgnore]
        public override string Info { get; set; }

        [JsonIgnore]
        public override string InfoIcon { get; set; }

        [JsonIgnore]
        public override EbScript _OnChange { get; set; }

        [JsonIgnore]
        public override EbFont LabelFontStyle { get; set; }

        [JsonIgnore]
        public override float FontSize { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.UserControl)]
        [PropertyEditor(PropertyEditorType.Expandable)]
        [OnChangeUIFunction("EbTable.padding")]
        [DefaultPropValue(0, 0, 0, 0)]
        public override UISides Padding { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.UserControl)]
        [PropertyEditor(PropertyEditorType.Expandable)]
        [PropertyGroup(PGConstants.APPEARANCE)]
        [UIproperty]
        [OnChangeUIFunction("Common.MARGIN")]
        [DefaultPropValue(0, 0, 0, 0)]
        public override UISides Margin { get; set; }

        public EbTableLayout()
        {
            this.Controls = new List<EbControl>();
            this.ObjType = this.GetType().Name.Substring(2, this.GetType().Name.Length - 2);
        }

        [HideInPropertyGrid]
        [JsonIgnore]
        public override string ToolIconHtml { get { return "<i class='fa fa-th-large'></i>"; } set { } }

        //public override string GetToolHtml()
        //{
        //    return @"<div eb-type='@toolName' class='tool'><i class='fa fa-table'></i>  @toolName</div>".Replace("@toolName", this.GetType().Name.Substring(2));
        //}

        public override string GetDesignHtml()
        {
            this.Controls = new List<EbControl>();
            this.Controls.Add(new EbTableTd { Name = "EbTable0_Td0", EbSid = "TableLayout1_Td0" });
            this.Controls.Add(new EbTableTd { Name = "EbTable0_Td1", EbSid = "TableLayout1_Td1" });
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
                <@@table@@ id='@ebsid@' class='form-render-table' ><@@tr@@>";

            foreach (EbControl ec in this.Controls)
                html += ec.GetHtml();

            return (html + "</@@tr@@></@@table@@></div>")
                .Replace("@name@", this.Name)
                .Replace("@ebsid@", this.EbSid_CtxId)
                .Replace("@@table@@", this.IsRenderMode ? "div" : "table")
                .Replace("@@tr@@", this.IsRenderMode ? "div" : "tr");
        }

        public void AdjustColumnWidth()
        {
            float widthSum = this.Controls.Select(e => (e as EbTableTd).WidthPercentage).Sum();
            List<EbControl> zeroWidthTd = this.Controls.FindAll(e => (e as EbTableTd).WidthPercentage <= 0);
            if (zeroWidthTd.Count > 0)
            {
                if (widthSum < 100)
                {
                    float calcWidth = (100 - widthSum) / zeroWidthTd.Count;
                    foreach (EbControl ec in zeroWidthTd)
                    {
                        (ec as EbTableTd).WidthPercentage = calcWidth;
                    }
                }
                else
                {
                    float calcWidth = 100 / this.Controls.Count;
                    foreach (EbTableTd ec in this.Controls)
                    {
                        ec.WidthPercentage = calcWidth;
                    }
                }
            }
            else if (widthSum != 100)
            {
                foreach (EbTableTd column in this.Controls)
                {
                    column.WidthPercentage = column.WidthPercentage * 100 / widthSum;
                }
            }
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

        [JsonIgnore]
        public override string Label { get; set; }

        [JsonIgnore]
        public override string LabelBackColor { get; set; }

        [JsonIgnore]
        public override string LabelForeColor { get; set; }

        [JsonIgnore]
        public override EbScript OnChangeFn { get; set; }

        [JsonIgnore]
        public override List<string> DependedValExp { get; set; }

        [JsonIgnore]
        public override List<string> DrDependents { get; set; }

        [JsonIgnore]
        public override List<string> HiddenExpDependants { get; set; }

        [JsonIgnore]
        public override List<string> DisableExpDependants { get; set; }

        [JsonIgnore]
        public override List<string> ValExpParams { get; set; }

        [JsonIgnore]
        public override List<string> DependedDG { get; set; }

        [JsonIgnore]
        public override string DBareHtml { get; set; }

        [JsonIgnore]
        public override string Info { get; set; }

        [JsonIgnore]
        public override string InfoIcon { get; set; }

        [JsonIgnore]
        public override EbScript _OnChange { get; set; }

        [JsonIgnore]
        public override EbFont LabelFontStyle { get; set; }

        [JsonIgnore]
        public override float FontSize { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.UserControl)]
        [HideInPropertyGrid]
        public override string TableName { get; set; }

        [PropertyGroup(PGConstants.APPEARANCE)]
        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.UserControl)]
        public float WidthPercentage { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.UserControl)]
        [PropertyGroup(PGConstants.APPEARANCE)]
        [UIproperty]
        [OnChangeUIFunction("EbTable.verticalAlign")]
        public VerticalAlign VerticalAlign { get; set; }

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
            string html = "<@@td@@ id='@name@' ctrl-ebsid='@ebsid@' ebsid='@ebsid@' style='width:@wperc@;' class='form-render-table-Td tdDropable ebResizable ebcont-ctrl ppbtn-cont'> <div class='tdInnerDiv ebcont-inner'>" +
                "@ppbtn@";

            foreach (EbControl ec in this.Controls)
                html += ec.GetHtml();

            return (html + "</div></@@td@@>")
                .Replace("@ppbtn@", Common.HtmlConstants.CONT_PROP_BTN)
                .Replace("@name@", this.Name)
                .Replace("@wperc@", (this.WidthPercentage != 0) ? this.WidthPercentage.ToString() + "%" : "auto")
                .Replace("@ebsid@", this.EbSid)
                .Replace("@@td@@", this.IsRenderMode ? "div" : "td");
        }
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