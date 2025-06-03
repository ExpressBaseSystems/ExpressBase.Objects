using ExpressBase.Common;
using ExpressBase.Common.Data;
using ExpressBase.Common.Extensions;
using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using ExpressBase.Common.Structures;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ExpressBase.Objects
{
    [EnableInBuilder(BuilderType.MobilePage)]
    public class EbMobileVisualization : EbMobileContainer, IMobileLink
    {
        [EnableInBuilder(BuilderType.MobilePage)]
        [MetaOnly]
        public override string Name { get; set; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyGroup("Core")]
        [DefaultPropValue("0")]
        [OnChangeExec(@"
                if (this.Type === 1) { 
                    pg.ShowPropertiesExt(['StaticParameters','Items']);
                    pg.HideGroupsExt(['Data','Link Settings','Action Button Settings']);
                }
                else {
                    pg.HidePropertiesExt(['StaticParameters','Items']);
                    pg.ShowGroupsExt(['Data','Link Settings','Action Button Settings']);
                }
            ")]
        public MobileVisualizationType Type { set; get; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyEditor(PropertyEditorType.Collection)]
        [PropertyGroup("Core")]
        public List<EbMobileStaticParameter> StaticParameters { set; get; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyEditor(PropertyEditorType.Collection)]
        [PropertyGroup("Core")]
        public List<EbMobileStaticListItem> Items { set; get; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyEditor(PropertyEditorType.ObjectSelector)]
        [OSE_ObjectTypes(EbObjectTypes.iDataReader)]
        [Alias("Data Source")]
        [PropertyGroup("Data")]
        public string DataSourceRefId { set; get; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyEditor(PropertyEditorType.ScriptEditorSQ)]
        [HelpText("sql query to get data from offline database")]
        [Alias("Offline Query")]
        [PropertyGroup("Data")]
        public EbScript OfflineQuery { set; get; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyGroup("Data")]
        [DefaultPropValue("30")]
        public int PageLength { set; get; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyGroup("Data")]
        public string MessageOnEmpty { set; get; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [HideInPropertyGrid]
        public List<Param> DataSourceParams { get; set; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [HideInPropertyGrid]
        public string SourceFormRefId { set; get; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [HideInPropertyGrid]
        public EbMobileTableLayout DataLayout { set; get; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [HideInPropertyGrid]
        public List<EbMobileControl> FilterControls { set; get; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [HideInPropertyGrid]
        public List<EbMobileDataColumn> SortColumns { set; get; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [HideInPropertyGrid]
        public List<EbMobileDataColumn> SearchColumns { set; get; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [MetaOnly]
        public List<EbMobileDataColToControlMap> DataColumns => new List<EbMobileDataColToControlMap>();

        [EnableInBuilder(BuilderType.MobilePage)]
        [MetaOnly]
        public List<EbMobileControlMeta> FormControlMetas => new List<EbMobileControlMeta>();

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyEditor(PropertyEditorType.ObjectSelector)]
        [OSE_ObjectTypes(EbObjectTypes.iMobilePage, EbObjectTypes.iReport, EbObjectTypes.iPrintLayout)]
        [PropertyGroup("Link Settings")]
        [Alias("Link")]
        [OnChangeExec(@"
                if (this.LinkRefId && this.LinkTypeForm){ 
                        pg.ShowPropertiesExt(this.LinkSettingsProps);
                }
                else {
                        pg.HidePropertiesExt(this.LinkSettingsProps);
                }
            ")]
        public string LinkRefId { get; set; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyGroup("Link Settings")]
        [OnChangeExec(@"
                if (this.FormMode === 1){ 
                        pg.ShowProperty('FormId');
                }
                else {
                        pg.HideProperty('FormId');
                }
            ")]
        public WebFormDVModes FormMode { set; get; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyGroup("Link Settings")]
        public bool RenderAsPopup { set; get; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyEditor(PropertyEditorType.CollectionFrmSrc, "DataColumns", 1)]
        [PropertyGroup("Link Settings")]
        public EbMobileDataColToControlMap FormId { set; get; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyGroup("Link Settings")]
        [Alias("Columns to controls map")]
        [PropertyEditor(PropertyEditorType.Mapper, "DataColumns", "FormControlMetas", "FormControl")]
        public List<EbMobileDataColToControlMap> LinkFormParameters { get; set; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyEditor(PropertyEditorType.Collection)]
        [PropertyGroup("Link Settings")]
        [Alias("Context to controls map")]
        public List<EbCTCMapper> ContextToControlMap { set; get; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyGroup("Link Settings")]
        public bool ShowLinkIcon { set; get; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyEditor(PropertyEditorType.ScriptEditorCS)]
        [PropertyGroup("Link Settings")]
        [Alias("Link Expression")]
        public EbScript LinkExpr { get; set; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyGroup("Link Settings")]
        [Alias("Link Fail Message")]
        public string LinkExprFailMsg { get; set; }

        #region FAB Settings Properties

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyGroup("Action Button Settings")]
        [Alias("Visibility")]
        [OnChangeExec(@"
                $(`#${this.EbSid} .vis-container-newbtn`).visibility(this.ShowNewButton);
                if (this.ShowNewButton){ 
                        pg.ShowProperty('NewButtonText');
                }
                else {
                        pg.HideProperty('NewButtonText');
                }
            ")]
        public bool ShowNewButton { set; get; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyGroup("Action Button Settings")]
        [Alias("Text")]
        [OnChangeExec(@"
                let mr = this.NewButtonText ? 8 : 0;
                let template = `<span style='margin-right:${mr}px'>${this.NewButtonText || ''}</span><i class='fa fa-plus'></i>`;
                $(`#${this.EbSid} .vis-container-newbtn`).html(template);
            ")]
        public string NewButtonText { set; get; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyGroup("Action Button Settings")]
        [DefaultPropValue("true")]
        [OnChangeExec(@"
                if (this.UseLinkSettings){ 
                    pg.HidePropertiesExt(['FabLinkRefId','ContextToFabControlMap']);
                }
                else {
                        pg.ShowProperty('FabLinkRefId');
                        if(this.FabLinkRefId && this.FabLinkTypeForm) {
                            pg.ShowProperty('ContextToFabControlMap');
                        }
                        else {
                            pg.HideProperty('ContextToFabControlMap');
                        }
                }
            ")]
        public bool UseLinkSettings { set; get; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyEditor(PropertyEditorType.ObjectSelector)]
        [OSE_ObjectTypes(EbObjectTypes.iMobilePage)]
        [PropertyGroup("Action Button Settings")]
        [Alias("Link")]
        [OnChangeExec(@"
                if (this.FabLinkRefId && this.FabLinkTypeForm){ 
                        pg.ShowProperty('ContextToFabControlMap');
                }
                else {
                        pg.HideProperty('ContextToFabControlMap');
                }
            ")]
        public string FabLinkRefId { get; set; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyEditor(PropertyEditorType.Collection)]
        [PropertyGroup("Action Button Settings")]
        [Alias("Context to controls map")]
        public List<EbCTCMapper> ContextToFabControlMap { set; get; }

        #endregion

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyGroup("List Styles")]
        [Alias("Alternate row coloring")]
        public bool EnableAlternateRowColoring { set; get; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyGroup("List Styles")]
        public bool ShowRowSeperator { set; get; }

        [EnableInBuilder(BuilderType.MobilePage)]
        public bool HideContext { set; get; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyEditor(PropertyEditorType.Expandable)]
        [PropertyGroup("List Styles")]
        [DefaultPropValue(5, 5, 5, 5)]
        public EbThickness Margin { set; get; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyEditor(PropertyEditorType.Expandable)]
        [PropertyGroup("List Styles")]
        [DefaultPropValue(10, 10, 10, 10)]
        public EbThickness Padding { set; get; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyGroup("List Styles")]
        [DefaultPropValue("5")]
        public int RowSpacing { set; get; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyGroup("List Styles")]
        [DefaultPropValue("5")]
        public int ColumnSpacing { set; get; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyGroup("List Styles")]
        [DefaultPropValue("10")]
        public int BorderRadius { set; get; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyGroup("List Styles")]
        [PropertyEditor(PropertyEditorType.Color)]
        [Alias("Border/Shadow Color")]
        [DefaultPropValue("#eaedf0")]
        public string BorderColor { set; get; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyGroup("List Styles")]
        [PropertyEditor(PropertyEditorType.Color)]
        [DefaultPropValue("#eaedf0")]
        public string BackgroundColor { set; get; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyGroup("List Styles")]
        public bool BoxShadow { set; get; }

        [OnDeserialized]
        public void OnDeserialized(StreamingContext context)
        {
            if (StaticParameters == null)
                StaticParameters = new List<EbMobileStaticParameter>();
        }

        public EbMobileVisualization()
        {
            DataSourceParams = new List<Param>();
            FilterControls = new List<EbMobileControl>();
            SortColumns = new List<EbMobileDataColumn>();
            SearchColumns = new List<EbMobileDataColumn>();

            Padding = new EbThickness(10);
            Margin = new EbThickness();
        }

        public override string GetDesignHtml()
        {
            return @"<div class='eb_mob_vis_container mob_container dropped' tabindex='1' eb-type='EbMobileVisualization' id='@id'>
                        <div class='eb_mob_container_inner'>
                            <label class='vis-group-label'>Design</label>
                            <div class='vis-table-container'></div>
                            <div class='filter_sort-tab'>
	                            <ul class='nav nav-tabs eb-styledTab filter_sort-tab-tabhead'>
		                            <li class='nav-item active'>
			                            <a class='nav-link' data-toggle='tab' role='tab' href='#filter-tab-@visname@'>
				                            Filter
			                            </a>
		                            </li>
		                            <li class='nav-item'>
			                            <a class='nav-link' data-toggle='tab' role='tab' href='#sort-tab-@visname@'>
				                            Sort
			                            </a>
		                            </li>
                                    <li class='nav-item'>
			                            <a class='nav-link' data-toggle='tab' role='tab' href='#search-tab-@visname@'>
				                            Search
			                            </a>
		                            </li>
	                            </ul>
	                            <div class='tab-content filter_sort-tab-content'>
		                            <div id='filter-tab-@visname@' class='tab-pane h-100 active'>
			                            <div class='vis-filter-container'>
				
			                            </div>
		                            </div>
		                            <div id='sort-tab-@visname@' class='tab-pane h-100'>
			                            <div class='vis-sort-container'>
				
			                            </div>
		                            </div>
                                    <div id='search-tab-@visname@' class='tab-pane h-100'>
			                            <div class='vis-search-container'>
				
			                            </div>
		                            </div>
	                            </div>
                            </div>
                        </div>
                        <div class='vis-container-newbtn'>
                            <i class='fa fa-plus'></i>
                        </div>
                    </div>".RemoveCR().DoubleQuoted().Replace("@visname@", Guid.NewGuid().ToString());
        }

        public override List<string> DiscoverRelatedRefids()
        {
            List<string> list = new List<string>();

            if (!string.IsNullOrEmpty(DataSourceRefId))
                list.Add(DataSourceRefId);

            if (!string.IsNullOrEmpty(LinkRefId))
                list.Add(LinkRefId);

            foreach (var cell in DataLayout.CellCollection)
            {
                foreach (var ctrl in cell.ControlCollection)
                {
                    list.AddRange(ctrl.DiscoverRelatedRefids());
                }
            }
            return list;
        }

        public override void ReplaceRefid(Dictionary<string, string> map)
        {
            if (!string.IsNullOrEmpty(DataSourceRefId) && map.TryGetValue(DataSourceRefId, out string dsri))
                this.DataSourceRefId = dsri;

            if (!string.IsNullOrEmpty(LinkRefId) && map.TryGetValue(LinkRefId, out string lri))
                this.LinkRefId = lri;

            foreach (EbMobileTableCell cell in DataLayout.CellCollection)
            {
                foreach (EbMobileControl ctrl in cell.ControlCollection)
                {
                    ctrl.ReplaceRefid(map);
                }
            }
        }

        public List<T> GetControlsByType<T>()
        {
            List<T> controls = new List<T>();

            foreach (EbMobileTableCell cells in DataLayout.CellCollection)
            {
                foreach (EbMobileControl ctrl in cells.ControlCollection)
                {
                    if (ctrl is T parsed)
                    {
                        controls.Add(parsed);
                    }
                }
            }
            return controls;
        }
    }
}
