using ExpressBase.Common;
using ExpressBase.Common.Data;
using ExpressBase.Common.Extensions;
using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using ExpressBase.Common.Structures;
using System;
using System.Collections.Generic;

namespace ExpressBase.Objects
{
    [EnableInBuilder(BuilderType.MobilePage)]
    public class EbMobileVisualization : EbMobileContainer, IMobileLink
    {
        [EnableInBuilder(BuilderType.MobilePage)]
        [MetaOnly]
        public override string Name { get; set; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyEditor(PropertyEditorType.ObjectSelector)]
        [OSE_ObjectTypes(EbObjectTypes.iDataReader)]
        [Alias("Data Source")]
        [PropertyGroup("Data")]
        public string DataSourceRefId { set; get; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyEditor(PropertyEditorType.ScriptEditorCS)]
        [HelpText("sql query to get data from offline database")]
        [Alias("Offline Query")]
        [PropertyGroup("Data")]
        public EbScript OfflineQuery { set; get; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyGroup("Data")]
        [DefaultPropValue("30")]
        public int PageLength { set; get; }

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
        [MetaOnly]
        public List<EbMobileDataColToControlMap> DataColumns => new List<EbMobileDataColToControlMap>();

        [EnableInBuilder(BuilderType.MobilePage)]
        [MetaOnly]
        public List<EbMobileControlMeta> FormControlMetas => new List<EbMobileControlMeta>();

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyEditor(PropertyEditorType.ObjectSelector)]
        [OSE_ObjectTypes(EbObjectTypes.iMobilePage)]
        [PropertyGroup("Link Settings")]
        [Alias("Link")]
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
        public bool ShowNewButton { set; get; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyGroup("Link Settings")]
        public bool ShowLinkIcon { set; get; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyGroup("List Styles")]
        [Alias("Alternate row coloring")]
        public bool EnableAlternateRowColoring { set; get; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyGroup("List Styles")]
        public bool ShowRowSeperator { set; get; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyEditor(PropertyEditorType.Expandable)]
        [PropertyGroup("List Styles")]
        public EbThickness Margin { set; get; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyEditor(PropertyEditorType.Expandable)]
        [PropertyGroup("List Styles")]
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
        public int BorderRadius { set; get; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyGroup("List Styles")]
        [PropertyEditor(PropertyEditorType.Color)]
        [Alias("Border/Shadow Color")]
        public string BorderColor { set; get; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyGroup("List Styles")]
        [PropertyEditor(PropertyEditorType.Color)]
        public string BackgroundColor { set; get; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyGroup("List Styles")]
        public bool BoxShadow { set; get; }

        public EbMobileVisualization()
        {
            OfflineQuery = new EbScript();
            DataSourceParams = new List<Param>();
            FilterControls = new List<EbMobileControl>();
            SortColumns = new List<EbMobileDataColumn>();
            LinkFormParameters = new List<EbMobileDataColToControlMap>();
            ContextToControlMap = new List<EbCTCMapper>();

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
    }
}
