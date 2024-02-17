using ExpressBase.Common;
using ExpressBase.Common.Constants;
using ExpressBase.Common.Extensions;
using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using ExpressBase.Common.Structures;
using ExpressBase.Objects.Objects.DVRelated;
using Newtonsoft.Json;
using ServiceStack.Stripe;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace ExpressBase.Objects
{

    [EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
    public class EbListView : EbControlUI
    {
        public EbListView()
        {
            this.ListViewLayout = new ListViewTableLayout();
            this.CalendarObject = new EbCalendar();
            //this.FilterColumn = new DataSourceColumn();
        }
        [OnDeserialized]
        public void OnDeserializedMethod(StreamingContext context)
        {
            this.BareControlHtml = this.GetBareHtml();
            this.BareControlHtml4Bot = this.BareControlHtml;
            this.ObjType = this.GetType().Name.Substring(2, this.GetType().Name.Length - 2);
        }

        public override string ToolIconHtml { get { return "<i class='fa fa-list'></i>"; } set { } }

        public override string ToolNameAlias { get { return "List View"; } set { } }

        public override string ToolHelpText { get { return "List View"; } set { } }

        // public override string UIchangeFns
        // {
        //     get
        //     {
        //         return @"EbCalendar = {
        //         starCount : function(elementId, props) {
        //	let rtngHtml='';
        //		for(let i=props.MaxVal; i>0; i--){
        //			rtngHtml += `<span class='fa fa-star-o wrd_spacing'></span>`;
        //		}
        //	let rtg =  $(`[ebsid = ${elementId}]`).find('.ratingDiv_dc').empty().append(rtngHtml);
        //}
        //     }";
        //     }
        // }

        public override string GetBareHtml()
        {
            return @"<div class='eb-list-view-continer row' id='@ebsid@' name='@name@'>
						<div class='col-lg-4 eb-list-view-left'>
						 <div class='listview-filter' id='label-@ebsid@'>                      
						</div>
                        <div class='listviewlayout'>
                        
                        </div>

						</div>
						<div class='col-lg-8 eb-list-view-right'>
							<div class='calendar-container' id='calendar-@ebsid@'></div>
						</div>
					</div>"
                    .Replace("@ebsid@", String.IsNullOrEmpty(this.EbSid_CtxId) ? "@ebsid@" : this.EbSid_CtxId)
                    .Replace("@name@", this.Name);
        }

        public override string GetDesignHtml()
        {
            string CalendarHtml = DesignHtml4Bot;

            string _html = HtmlConstants.CONTROL_WRAPER_HTML4WEB
              .Replace("@LabelForeColor ", "color:" + (LabelForeColor ?? "@LabelForeColor ") + ";")
              .Replace("@LabelBackColor ", "background-color:" + (LabelBackColor ?? "@LabelBackColor ") + ";")
              .Replace("@barehtml@", CalendarHtml)
                   .RemoveCR().DoubleQuoted()
             .Replace("@ebsid@", String.IsNullOrEmpty(this.EbSid_CtxId) ? "@ebsid@" : this.EbSid_CtxId)
                    .Replace("@name@", this.Name);
            return ReplacePropsInHTML(_html);

            //.Replace("@ebsid@", String.IsNullOrEmpty(this.EbSid_CtxId) ? "@ebsid@" : this.EbSid_CtxId)
            //.Replace("@name@", this.Name)
        }
        public override string DesignHtml4Bot
        {
            get => @"<div class='eb-list-view-continer row' id='@ebsid@' name='@name@'>
						<div class='col-lg-4 eb-list-view-left'>
						 <div class='listview-filter' id='label-@ebsid@'>                      
						</div>
                        <div class='listviewlayout'>
                        <table>
                         <tbody>
                         </tbody>
                        </table>
                        </div>

						</div>
						<div class='col-lg-8 eb-list-view-right'>
							<div class='calendar-container' id='calendar-@ebsid@'></div>
						</div>
					</div>";
            set => base.DesignHtml4Bot = value;
        }

        public override string GetHtml()
        {
            string EbCtrlHTML = HtmlConstants.CONTROL_WRAPER_HTML4WEB
               .Replace("@LabelForeColor ", "color:" + (LabelForeColor ?? "@LabelForeColor ") + ";")
               .Replace("@LabelBackColor ", "background-color:" + (LabelBackColor ?? "@LabelBackColor ") + ";");

            return ReplacePropsInHTML(EbCtrlHTML);
        }
        public override string GetHtml4Bot()
        {
            return ReplacePropsInHTML(HtmlConstants.CONTROL_WRAPER_HTML4BOT);
        }

        //public override string GetValueFromDOMJSfn
        //{
        //    get
        //    {
        //        return @" return $('#' + this.EbSid).rateYo('rating');";
        //    }
        //    set { }
        //}

        //public override string OnChangeBindJSFn
        //{
        //    get
        //    {
        //        return @"$('#' + this.EbSid).rateYo().on('rateyo.set', p1);";
        //    }
        //    set { }
        //}

        //public override string SetValueJSfn
        //{
        //    get
        //    {
        //        return @" $('#' + this.EbSid).rateYo('rating', p1);";
        //    }
        //    set { }
        //}
        //public override string ClearJSfn
        //{
        //    get
        //    {
        //        return @" $('#' + this.EbSid).rateYo('rating', 0);";
        //    }
        //    set { }
        //}


        //--------Hide in property grid------------
        [EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
        [HideInPropertyGrid]
        public override string HelpText { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
        [HideInPropertyGrid]
        public override string ToolTipText { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
        [HideInPropertyGrid]
        public override bool Unique { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
        [HideInPropertyGrid]
        public override List<EbValidator> Validators { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
        [HideInPropertyGrid]
        public override EbScript DefaultValueExpression { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
        [HideInPropertyGrid]
        public override EbScript HiddenExpr { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
        [HideInPropertyGrid]
        public override EbScript DisableExpr { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
        [HideInPropertyGrid]
        public override EbScript ValueExpr { get; set; }

        public override bool SelfTrigger { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
        [HideInPropertyGrid]
        public override bool Required { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
        [HideInPropertyGrid]
        public override bool DoNotPersist { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
        [HideInPropertyGrid]
        public override string BackColor { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
        [HideInPropertyGrid]
        public override string ForeColor { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
        [HideInPropertyGrid]
        public override string LabelBackColor { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
        [HideInPropertyGrid]
        public override string LabelForeColor { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
        [HideInPropertyGrid]
        public override EbScript OnChangeFn { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
        [HideInPropertyGrid]
        public override EbDbTypes EbDbType { get { return EbDbTypes.Decimal; } set { } }


        [EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
        [Alias("RenderCalendar")]
        [DefaultPropValue("true")]
        [PropertyGroup(PGConstants.DATA_SETTINGS)]
        public bool RenderCalendar { get; set; }


        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl)]
        [PropertyEditor(PropertyEditorType.ObjectSelector)]
        [OSE_ObjectTypes(EbObjectTypes.iDataReader)]
        [PropertyPriority(98)]
        [PropertyGroup(PGConstants.DATA_SETTINGS)]
        [Alias("Data Reader")]
        public string DataSourceId { get; set; }


        [EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
        [HideInPropertyGrid]
        public EbCalendar CalendarObject { get; set; }


        //[EnableInBuilder(BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.WebForm, BuilderType.UserControl)]
        //[PropertyGroup(PGConstants.DATA_SETTINGS)]
        //public DataSourceColumn FilterColumn { get; set; }


        [EnableInBuilder(BuilderType.WebForm)]
        [HideInPropertyGrid]
        public ListViewTableLayout ListViewLayout { set; get; }

        [EnableInBuilder(BuilderType.WebForm)]
        [HideInPropertyGrid]
        public ListViewTableLayout ListViewFilterLayout { set; get; }
    }

    [UsedWithTopObjectParent(typeof(EbObject))]
    [EnableInBuilder(BuilderType.WebForm)]
    public class ListViewTableLayout
    {
        [EnableInBuilder(BuilderType.WebForm)]
        [Alias("RowCount")]
        [DefaultPropValue("2")]
        public int RowCount { get; set; } = 2;

        [EnableInBuilder(BuilderType.WebForm)]
        [Alias("ColumnCount")]
        [DefaultPropValue("2")]
        public int ColumnCount { get; set; } = 2;

        [EnableInBuilder(BuilderType.WebForm)]
        [HideInPropertyGrid]
        public List<ListViewRowLayout> RowLayouts { get; set; }

        public ListViewTableLayout()
        {
            this.RowLayouts = new List<ListViewRowLayout>();
            for (int i = 0; i < RowCount; i++)
            {
                this.RowLayouts.Add(new ListViewRowLayout());
                for (int j = 0; j < ColumnCount; j++)
                {
                    this.RowLayouts[i].ColumnLayouts.Add(new ListViewColumnLayout());
                }

            }
        }
    }

    [UsedWithTopObjectParent(typeof(EbObject))]
    [EnableInBuilder(BuilderType.WebForm)]
    public class ListViewRowLayout
    {
        public ListViewRowLayout()
        {
            this.ColumnLayouts = new List<ListViewColumnLayout>();
        }

        [EnableInBuilder(BuilderType.WebForm)]
        [HideInPropertyGrid]
        public List<ListViewColumnLayout> ColumnLayouts { get; set; }

    }
    [UsedWithTopObjectParent(typeof(EbObject))]
    [EnableInBuilder(BuilderType.WebForm)]
    [HideInToolBox]
    public class ListViewColumnLayout
    {
        public ListViewColumnLayout()
        {
            //this.ColumnWidth = 100;
            //this.DataColumn = new DataSourceColumn();
        }
        [EnableInBuilder(BuilderType.WebForm)]
        public int RowSpan { get; set; } = 1;
        [EnableInBuilder(BuilderType.WebForm)]
        public int ColumnSpan { get; set; } = 1;
        [EnableInBuilder(BuilderType.WebForm)]
        public int ColumnWidth { get; set; }

        [EnableInBuilder(BuilderType.WebForm)]
        public bool IsHidden { get; set; }

        [EnableInBuilder(BuilderType.WebForm)]
        [HideInPropertyGrid]
        public ListViewDataColumnBase DataColumn { get; set; }

    }
    public class ListViewDataColumnBase
    {
        public ListViewDataColumnBase()
        {
            //this.IsEnabled = false;
            //this.BackgroundColor = "";
            //this.BorderColor = "";
            //this.BorderRadius = 0;
            //this.BorderThickness = 0;
            //this.ColumnType = "";
            ////this.Font = new EbFont();
        }

        [EnableInBuilder(BuilderType.WebForm)]
        [HideInPropertyGrid]
        public string EbType { get; set; }

        [EnableInBuilder(BuilderType.WebForm)]
        [PropertyGroup(PGConstants.APPEARANCE)]
        [PropertyEditor(PropertyEditorType.FontSelector)]
        public EbFont Font { get; set; }

        [EnableInBuilder(BuilderType.WebForm)]
        [UIproperty]
        [PropertyGroup(PGConstants.APPEARANCE)]
        [PropertyEditor(PropertyEditorType.Color)]
        public string BackgroundColor { get; set; }

        [EnableInBuilder(BuilderType.WebForm)]
        [PropertyGroup(PGConstants.APPEARANCE)]
        public int BorderRadius { get; set; }

        [EnableInBuilder(BuilderType.WebForm)]
        [PropertyGroup(PGConstants.APPEARANCE)]
        public int BorderThickness { get; set; }

        [EnableInBuilder(BuilderType.WebForm)]
        [PropertyGroup(PGConstants.APPEARANCE)]
        [PropertyEditor(PropertyEditorType.Color)]
        public string BorderColor { get; set; }

        [EnableInBuilder(BuilderType.WebForm)]
        [DefaultPropValue("#ffffff")]
        [PropertyGroup(PGConstants.APPEARANCE)]
        [PropertyEditor(PropertyEditorType.Color)]
        public string ForeColor { get; set; }

        [EnableInBuilder(BuilderType.WebForm)]
        [PropertyGroup(PGConstants.APPEARANCE)]
        public int Padding { get; set; }

        [EnableInBuilder(BuilderType.WebForm)]
        [PropertyGroup(PGConstants.APPEARANCE)]
        public int Margin { get; set; }
    }

    [UsedWithTopObjectParent(typeof(EbObject))]
    [EnableInBuilder(BuilderType.WebForm)]
    [HideInToolBox]
    public class ListViewDataColumn : ListViewDataColumnBase
    {
        public ListViewDataColumn()
        {
            this.EbType = "ListViewDataColumn";
        }
        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyGroup(PGConstants.APPEARANCE)]
        [DefaultPropValue("true")]
        public bool Transparent { get; set; }

        [EnableInBuilder(BuilderType.WebForm)]
        [HideInPropertyGrid]
        public string ColumnName { get; set; }
    }

    [UsedWithTopObjectParent(typeof(EbObject))]
    [EnableInBuilder(BuilderType.WebForm)]
    [HideInToolBox]
    public class ListViewButton : ListViewDataColumnBase
    {
        public ListViewButton()
        {
            this.ButtonText = "ButtonText";
            this.EbType = "ListViewButton";
            this.ColumnName = "id";
        }

        [EnableInBuilder(BuilderType.WebForm)]
        [PropertyGroup(PGConstants.CORE)]
        public string ButtonText { get; set; }

        [EnableInBuilder(BuilderType.WebForm)]
        [PropertyEditor(PropertyEditorType.ObjectSelector)]
        [PropertyGroup(PGConstants.CORE)]
        [OSE_ObjectTypes(EbObjectTypes.iWebForm)]
        [Alias("Destination Form")]
        public string FormRefId { get; set; }

        [EnableInBuilder(BuilderType.WebForm)]
        [PropertyGroup(PGConstants.CORE)]
        public bool IsPopupForm { get; set; }

        [EnableInBuilder(BuilderType.WebForm)]
        public string ColumnName { get; set; }
    }

    [UsedWithTopObjectParent(typeof(EbObject))]
    [EnableInBuilder(BuilderType.WebForm)]
    [HideInToolBox]
    public class ListViewFilterButton : ListViewDataColumnBase
    {

        public ListViewFilterButton()
        {
            this.ButtonText = "ButtonText";
            this.EbType = "ListViewFilterButton";
        }
        [EnableInBuilder(BuilderType.WebForm)]
        [PropertyGroup(PGConstants.CORE)]
        public string ButtonText { get; set; }

        [EnableInBuilder(BuilderType.WebForm)]
        public string FilterColumName { get; set; }

        [EnableInBuilder(BuilderType.WebForm)]
        public string FilterColumValue { get; set; }

        [EnableInBuilder(BuilderType.WebForm)]
        [PropertyEditor(PropertyEditorType.ScriptEditorJS)]
        [HelpText("Define filter for Listview")]
        public EbScript FilterScript { get; set; }
    }

    [UsedWithTopObjectParent(typeof(EbObject))]
    [EnableInBuilder(BuilderType.WebForm)]
    [HideInToolBox]
    public class EbCalendar
    {
        public EbCalendar()
        {
            this.MultiMonthYear = true;
            this.DayGridYear = true;
            this.DayGridMonth = true;
            this.TimeGridWeek = true;
            this.TimeGridDay = true;
            this.ListWeek = true;
            this.Selectable = true;
            this.Editable = true;
            this.SlotMinTime = "08:00:00";
            this.SlotMaxTime = "18:00:00";
            this.SlotDuration = "00:15:00";
        }
        
        [EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
        [DefaultPropValue("08:00:00")]
        [Alias("Slot minimum time")]
        [PropertyGroup("Duration in TimeSpan")]
        public string SlotMinTime { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
        [DefaultPropValue("18:00:00")]
        [Alias("Slot maximum time")]
        [PropertyGroup("Duration in TimeSpan")]
        public string SlotMaxTime { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
        [DefaultPropValue("00:15:00")]
        [Alias("Slot Duration")]
        [PropertyGroup("Duration in TimeSpan")]
        public string SlotDuration { get; set; }


        [EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
        [Alias("Show multi month calendar")]
        [DefaultPropValue("false")]
        public bool MultiMonthYear { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
        [Alias("Day Grid Year view")]
        [DefaultPropValue("false")]
        public bool DayGridYear { get; set; }


        [EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
        [Alias("Show Month calendar")]
        [DefaultPropValue("true")]
        public bool DayGridMonth { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
        [Alias("Show week calendar")]
        [DefaultPropValue("true")]
        public bool TimeGridWeek { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
        [Alias("Show day calendar")]
        [DefaultPropValue("true")]
        public bool TimeGridDay { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
        [Alias("Show list week")]
        [DefaultPropValue("true")]
        public bool ListWeek { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
        [Alias("Selectable")]
        [DefaultPropValue("true")]
        public bool Selectable { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
        [Alias("Editable")]
        [DefaultPropValue("true")]
        public bool Editable { get; set; }


        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl)]
        [PropertyEditor(PropertyEditorType.ObjectSelector)]
        [OSE_ObjectTypes(EbObjectTypes.iDataReader)]
        [PropertyPriority(98)]
        [PropertyGroup(PGConstants.DATA_SETTINGS)]
        [Alias("Data Reader")]
        public string DataSourceId { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl)]
        [PropertyEditor(PropertyEditorType.ObjectSelector)]
        [PropertyGroup(PGConstants.DATA_INSERT)]
        [OSE_ObjectTypes(EbObjectTypes.iWebForm)]
        [Alias("Form")]
        public string FormRefId { get; set; }
    }
}
