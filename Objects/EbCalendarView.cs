using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using ExpressBase.Common.Structures;
using ExpressBase.Objects.Objects.DVRelated;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExpressBase.Objects
{
    public enum AttendanceType
    {
        Hourly = 0,
        DayWise = 1,
        Weekely = 2,
        Monthly = 3,
        Quarterly = 4,
        HalfYearly = 5,
        Yearly = 6,
    }

    [EnableInBuilder(BuilderType.Calendar)]
    public class EbCalendarWrapper : EbObject
    {
        [HideForUser]
        [EnableInBuilder(BuilderType.Calendar)]
        public override string Name { get; set; }

        [HideInPropertyGrid]
        [EnableInBuilder(BuilderType.Calendar)]
        public override string RefId { get; set; }

        [HideForUser]
        [EnableInBuilder(BuilderType.Calendar)]
        public override string DisplayName { get; set; }

        [HideInPropertyGrid]
        [EnableInBuilder(BuilderType.Calendar)]
        public override string Description { get; set; }

        [HideInPropertyGrid]
        [EnableInBuilder(BuilderType.Calendar)]
        public override string VersionNumber { get; set; }

        [HideInPropertyGrid]
        [EnableInBuilder(BuilderType.Calendar)]
        public override string Status { get; set; }
    }

    [EnableInBuilder(BuilderType.Calendar)]
    [BuilderTypeEnum(BuilderType.Calendar)]
    public class EbCalendarView : EbTableVisualization, IEBRootObject
    {
        [PropertyEditor(PropertyEditorType.CollectionABCpropToggle, "DataColumns", "bVisible")]
        [EnableInBuilder(BuilderType.Calendar)]
        public List<DVBaseColumn> DataColumns { get; set; }

        [MetaOnly]
        [EnableInBuilder(BuilderType.Calendar)]
        public List<DVBaseColumn> LinesColumns { get; set; }

        [PropertyEditor(PropertyEditorType.CollectionABCpropToggle, "KeyColumns", "bVisible")]
        [CEOnSelectFn(@";
            this.bVisible = true;")]
        [CEOnDeselectFn(@"
            console.log('ondeselection');
            this.bVisible = false;")]
        [EnableInBuilder(BuilderType.Calendar)]
        public List<DVBaseColumn> KeyColumns { get; set; }

        [EnableInBuilder(BuilderType.Calendar)]
        [HideInPropertyGrid]
        public List<DVBaseColumn> DateColumns { get; set; }

        [PropertyEditor(PropertyEditorType.CollectionFrmSrc, "KeyColumns", 1)]
        [EnableInBuilder(BuilderType.Calendar)]
        public DVBaseColumn PrimaryKey { get; set; }

        [PropertyEditor(PropertyEditorType.CollectionFrmSrc, "LinesColumns", 1)]
        [EnableInBuilder(BuilderType.Calendar)]
        public DVBaseColumn ForeignKey { get; set; }

        [EnableInBuilder(BuilderType.Calendar)]
        [MetaOnly]
        public override  DVColumnCollection Columns{ get; set; }

        [EnableInBuilder(BuilderType.Calendar)]
        [PropertyEditor(PropertyEditorType.ObjectSelectorCollection)]
        [OSE_ObjectTypes(EbObjectTypes.iWebForm)]
        public List<ObjectBasicInfo> ObjectLinks { get; set; }

        [EnableInBuilder(BuilderType.Calendar)]
        [PropertyEditor(PropertyEditorType.DropDown)]
        [HideInPropertyGrid]        
        public AttendanceType CalendarType { get; set; }

        [EnableInBuilder(BuilderType.Calendar)]
        [PropertyEditor(PropertyEditorType.DropDown)]
        [HideForUser]
        [OnChangeExec(@"
        if(this.DefaultCalendarType == 0){
        pg.ShowProperty('StartTime');
        pg.ShowProperty('EndTime');
        pg.ShowProperty('Interval');
        }
        else{
        pg.HideProperty('StartTime');
        pg.HideProperty('EndTime');
        pg.HideProperty('Interval');
        }
        ")]
        public AttendanceType DefaultCalendarType { get; set; }

        [EnableInBuilder(BuilderType.Calendar)]
        [HideForUser]
        [DefaultPropValue("9")]
        public int StartTime { get; set; }

        [EnableInBuilder(BuilderType.Calendar)]
        [HideForUser]
        [DefaultPropValue("7")]
        public int EndTime { get; set; }

        [EnableInBuilder(BuilderType.Calendar)]
        [HideForUser]
        [DefaultPropValue("1")]
        public int Interval { get; set; }

        //[EnableInBuilder(BuilderType.Calendar)]
        //public override int LeftFixedColumn { get; set; }

        public static EbOperations Operations = CalendarOperations.Instance;

        public EbCalendarView()
        {
            this.DataColumns = new List<DVBaseColumn>();
            this.KeyColumns = new List<DVBaseColumn>();
            this.DateColumns = new List<DVBaseColumn>();
            this.ObjectLinks = new List<ObjectBasicInfo>();
            this.PrimaryKey = new DVBaseColumn();
            this.ForeignKey = new DVBaseColumn();
           
        }

        public override List<string> DiscoverRelatedRefids()
        {
            List<string> _refids = new List<string>();
            
            return _refids;
        }


    }
}
