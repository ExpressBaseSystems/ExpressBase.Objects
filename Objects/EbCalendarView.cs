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
        DayWise = 0,
        Weekely = 1,
        Monthly = 2,
        HalfYearly = 3,
        Yearly = 4,
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
    public class EbCalendarView : EbDataVisualization, IEBRootObject
    {
        [PropertyEditor(PropertyEditorType.CollectionABCFrmSrc, "LinesColumns")]        
        [EnableInBuilder(BuilderType.Calendar)]
        public List<DVBaseColumn> DataColumns { get; set; }

        [HideInPropertyGrid]
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
        [HideInPropertyGrid]
        public override  DVColumnCollection Columns{ get; set; }

        [EnableInBuilder(BuilderType.Calendar)]
        [PropertyEditor(PropertyEditorType.ObjectSelectorCollection)]
        [OSE_ObjectTypes(EbObjectTypes.iWebForm)]
        public List<ObjectBasicInfo> FormLinks { get; set; }

        public EbCalendarView()
        {
            this.DataColumns = new List<DVBaseColumn>();
            this.KeyColumns = new List<DVBaseColumn>();
            this.DateColumns = new List<DVBaseColumn>();
            this.FormLinks = new List<ObjectBasicInfo>();
           
        }

        public override List<string> DiscoverRelatedRefids()
        {
            List<string> _refids = new List<string>();
            
            return _refids;
        }


    }
}
