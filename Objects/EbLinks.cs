﻿using ExpressBase.Common;
using ExpressBase.Common.Constants;
using ExpressBase.Common.Extensions;
using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using ExpressBase.Common.Structures;
using ExpressBase.Objects.Objects.DVRelated;
using Newtonsoft.Json;
using Org.BouncyCastle.Asn1.Cms;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ExpressBase.Objects
{

    [EnableInBuilder(BuilderType.UserControl, BuilderType.DashBoard)]
    public class EbLinks : EbControlUI
    {

        public EbLinks() { }

        [OnDeserialized]
        public void OnDeserializedMethod(StreamingContext context)
        {
            this.BareControlHtml = this.GetBareHtml();
            this.ObjType = this.GetType().Name.Substring(2, this.GetType().Name.Length - 2);
        }

        public override string ToolIconHtml { get { return "<i class='fa fa-external-link-square'></i>"; } set { } }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.UserControl, BuilderType.DashBoard)]
        [HideInPropertyGrid]
        public string DataObjCtrlName { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.UserControl, BuilderType.DashBoard)]
        [HideInPropertyGrid]
        public string DataObjColName { get; set; }


        [EnableInBuilder(BuilderType.DashBoard)]
        public override string DisplayName { get; set; }

        public override string UIchangeFns
        {
            get
            {
                return @"";
            }
        }

        public override string GetBareHtml()
        {
            return @" < div class='links' style='border:solid 1px'></div>";

        }

        public override string GetDesignHtml()
        {
            return GetHtml().RemoveCR().DoubleQuoted();
        }

        public override string GetHtml()
        {
            string EbCtrlHTML = @"
        <div id='@id' ebsid='@id' name='@name@' class='links' eb-type='Links'>
        </div>";
            return ReplacePropsInHTML(EbCtrlHTML);
        }

        [EnableInBuilder(BuilderType.DashBoard)]
        [PropertyEditor(PropertyEditorType.ObjectSelector)]
        [PropertyGroup("Data Settings")]
        [OSE_ObjectTypes(EbObjectTypes.iWebForm, EbObjectTypes.iDashBoard, EbObjectTypes.iTableVisualization, EbObjectTypes.iChartVisualization, EbObjectTypes.iCalendarView)]
        public string Object_Selector { get; set; }

        [EnableInBuilder(BuilderType.DashBoard)]
        [PropertyGroup("Data Settings")]
        [PropertyEditor(PropertyEditorType.MultiLanguageKeySelector)]
        public string LinkName { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl, BuilderType.DashBoard)]
        [PropertyGroup("Data Settings")]
        [UIproperty]
        [PropertyEditor(PropertyEditorType.GradientColorPicker)]
        [DefaultPropValue("")]
        public string BackgroundColor { get; set; }

        [EnableInBuilder(BuilderType.DashBoard)]
        [PropertyGroup("Data Settings")]
        [UIproperty]
        [PropertyEditor(PropertyEditorType.FontSelector)]
        public EbFont FontStyle { get; set; }

        [EnableInBuilder(BuilderType.DashBoard)]
        [PropertyGroup("Icon")]
        [UIproperty]
        [PropertyEditor(PropertyEditorType.IconPicker)]
        [DefaultPropValue("fa-external-link-square")]
        public string Icon { get; set; }

        [EnableInBuilder(BuilderType.DashBoard)]
        [PropertyGroup("Icon")]
        [UIproperty]
        [DefaultPropValue("fa-link")]
        public string HoverText { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl, BuilderType.DashBoard)]
        [PropertyGroup("Icon")]
        [UIproperty]
        [PropertyEditor(PropertyEditorType.GradientColorPicker)]
        [DefaultPropValue("")]
        public string IconBackgroundColor { get; set; }

        public override void LocalizeControl(Dictionary<string, string> Keys)
        {
            if (!string.IsNullOrWhiteSpace(this.LinkName) && Keys.ContainsKey(this.LinkName))
            {
                this.LinkName = Keys[this.LinkName];
            }
        }

        public override void AddMultiLangKeys(List<string> keysList)
        {
            if (!string.IsNullOrWhiteSpace(this.LinkName) && !keysList.Contains(this.LinkName))
            {
                keysList.Add(this.LinkName);
            }
        }
    }

}