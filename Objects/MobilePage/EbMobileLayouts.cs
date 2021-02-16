using ExpressBase.Common.Extensions;
using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ExpressBase.Objects
{
    [EnableInBuilder(BuilderType.MobilePage)]
    public class EbMobileTableLayout : EbMobileControl, ILayoutControl
    {
        public override string Label { set; get; }
        public override bool Unique { get; set; }
        public override bool ReadOnly { get; set; }
        public override bool DoNotPersist { get; set; }
        public override bool Required { get; set; }
        public override bool Hidden { set; get; }
        public override EbScript ValueExpr { get; set; }
        public override EbScript HiddenExpr { get; set; }
        public override EbScript DisableExpr { get; set; }
        public override EbScript DefaultValueExpression { get; set; }
        public override List<EbMobileValidator> Validators { set; get; }

        public EbMobileTableLayout()
        {
            this.CellCollection = new List<EbMobileTableCell>();
        }

        [EnableInBuilder(BuilderType.MobilePage)]
        [HideInPropertyGrid]
        public int RowCount { set; get; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [HideInPropertyGrid]
        public int ColumCount { set; get; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [HideInPropertyGrid]
        public List<EbMobileTableCell> CellCollection { set; get; }

        public override string GetDesignHtml()
        {
            return @"<div class='eb_mob_tablelayout mob_control dropped' eb-type='EbMobileTableLayout' id='@id'>
                        <div class='eb_mob_tablelayout_inner'>
                            
                        </div>
                    </div>".RemoveCR().DoubleQuoted();
        }

        public override string GetJsInitFunc()
        {
            return @"
                this.Init = function(id)
                {
                    this.RowCount = 2;
                    this.ColumCount= 2;
                };";
        }

        public override EbControl GetWebFormControl(int count)
        {
            return new EbTableLayout
            {
                EbSid = this.EbControlType + count,
                Name = this.Name,
                Label = this.Label
            };
        }
    }

    [EnableInBuilder(BuilderType.MobilePage)]
    public class EbMobileTableCell : EbMobilePageBase
    {
        public EbMobileTableCell()
        {
            this.ControlCollection = new List<EbMobileControl>();
        }

        [EnableInBuilder(BuilderType.MobilePage)]
        public int RowIndex { set; get; }

        [EnableInBuilder(BuilderType.MobilePage)]
        public int ColIndex { set; get; }

        [EnableInBuilder(BuilderType.MobilePage)]
        public int Width { set; get; }

        [EnableInBuilder(BuilderType.MobilePage)]
        public List<EbMobileControl> ControlCollection { set; get; }
    }

    [EnableInBuilder(BuilderType.MobilePage)]
    public class EbMobileStackLayout : EbMobileDashBoardControl
    {
        [EnableInBuilder(BuilderType.MobilePage)]
        [OnChangeExec(@"
            if(this.Orientation === 0){
                $(`#${this.EbSid} .control_container`).css('flex-direction','row');
            }
            else{
                $(`#${this.EbSid} .control_container`).css('flex-direction','column');
            }
            ")]
        public StackOrientation Orientation { set; get; }

        [EnableInBuilder(BuilderType.MobilePage)]
        public bool AllowScrolling { set; get; }

        [EnableInBuilder(BuilderType.MobilePage)]
        public int Spacing { set; get; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [HideInPropertyGrid]
        public List<EbMobileDashBoardControl> ChildControls { set; get; }

        [OnDeserialized]
        public void OnDeserialized(StreamingContext context)
        {
            if (this.ChildControls == null) this.ChildControls = new List<EbMobileDashBoardControl>();
        }

        public override string GetDesignHtml()
        {
            return @"<div class='mob_dash_control dropped' id='@id' eb-type='EbMobileStackLayout' tabindex='1' onclick='$(this).focus()'>                            
                            <div class='eb_dash_ctrlhtml ctrl_as_container'>
                               <div class='control_container dash-stacklayout'>
                               </div>
                            </div>
                        </div>".RemoveCR().DoubleQuoted();
        }
    }
}
