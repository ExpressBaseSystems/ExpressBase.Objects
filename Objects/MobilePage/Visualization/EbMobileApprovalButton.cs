using ExpressBase.Common;
using ExpressBase.Common.Constants;
using ExpressBase.Common.Extensions;
using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using ExpressBase.Common.Structures;
using System.Collections.Generic;

namespace ExpressBase.Objects
{
    [EnableInBuilder(BuilderType.MobilePage)]
    public class EbMobileApprovalButton : EbMobileButton
    {
        public override string LinkRefId { get; set; }
        public override WebFormDVModes FormMode { set; get; }
        public override string Text { set; get; }
        public override bool RenderTextAsIcon { get; set; }
        public override List<EbMobileDataColToControlMap> LinkFormParameters { get; set; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyEditor(PropertyEditorType.ObjectSelector)]
        [OSE_ObjectTypes(EbObjectTypes.iWebForm)]
        [PropertyGroup(PGConstants.CORE)]
        public string FormRefid { get; set; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyEditor(PropertyEditorType.CollectionFrmSrc, "DataColumns", 1)]
        [PropertyGroup(PGConstants.CORE)]
        public override EbMobileDataColToControlMap FormId { set; get; }

        public EbDataTable ApprovalData;

        public int StageNameIndex;

        public int ActionIdIndex;

        public int StatusIndex;

        public override string GetDesignHtml()
        {
            return @"<div class='eb_stacklayout mob_control dropped' id='@id' eb-type='EbMobileApprovalButton' tabindex='1' onclick='$(this).focus()'>
                            <div class='eb_btnctrlhtml h-100'>
                               <button class='ebm-btn h-100'>Approval Button</button>
                            </div>
                        </div>".RemoveCR().DoubleQuoted();
        }

        public override List<string> DiscoverRelatedRefids()
        {
            List<string> list = new List<string>();

            if (!string.IsNullOrEmpty(FormRefid))
                list.Add(FormRefid);

            return list;
        }

        public override void ReplaceRefid(Dictionary<string, string> map)
        {
            if (!string.IsNullOrEmpty(FormRefid) && map.TryGetValue(FormRefid, out string dsri))
                this.FormRefid = dsri;
        }
    }
}
