using ExpressBase.Common;
using ExpressBase.Common.Extensions;
using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using ExpressBase.Common.Structures;
using ExpressBase.Security;
using ExpressBase.Objects.ServiceStack_Artifacts;
using Newtonsoft.Json;
using ServiceStack;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Linq;
using ExpressBase.Common.Constants;

namespace ExpressBase.Objects
{
    //[HideInToolBox]
    [EnableInBuilder(BuilderType.WebForm)]
    public class EbReview : EbControlContainer, IEbSpecialContainer
    {
        public EbReview()
        {
            FormStages = new List<ReviewStageAbstract>();
            Controls = new List<EbControl>();
            this.OnApprovalRoutines = new List<EbRoutines>();
        }

        [OnDeserialized]
        public void OnDeserializedMethod(StreamingContext context)
        {
            this.ObjType = this.GetType().Name.Substring(2, this.GetType().Name.Length - 2);
            //this.EbDbType = this.EbDbType;    
            Controls = new List<EbControl>() {
                new EbDGStringColumn() { Name = "stage_unique_id", EbDbType = EbDbTypes.String, Label = "Stage"},
                new EbDGStringColumn() { Name = "action_unique_id", EbDbType = EbDbTypes.String, Label = "Action"},
                new EbDGNumericColumn() { Name = "eb_my_actions_id", EbDbType = EbDbTypes.Decimal, Label = "My_Action_Id"},
                new EbDGStringColumn() { Name = "comments", EbDbType = EbDbTypes.String, Label = "Comments"},
                new EbDGDateColumn() { Name = "eb_created_at", EbDbType = EbDbTypes.DateTime, EbDateType = EbDateType.DateTime, DoNotPersist = true, IsSysControl = true},
                new EbDGCreatedByColumn() { Name = "eb_created_by", EbDbType = EbDbTypes.Decimal, DoNotPersist = true, IsSysControl = true}//,
                //new EbDGStringColumn() { Name = "eb_created_by_s", EbDbType = EbDbTypes.String, DoNotPersist = true}
            };
        }

        //C# script variable
        public string ReviewStatus { get; set; }
        
        [EnableInBuilder(BuilderType.WebForm, BuilderType.UserControl)]
        [HideInPropertyGrid]
        public override bool IsSpecialContainer { get { return true; } set { } }

        [JsonIgnore]
        public override UISides Padding { get; set; }

        [EnableInBuilder(BuilderType.WebForm)]
        [PropertyEditor(PropertyEditorType.Label)]
        [ReservedValues()]
        public override string Name { get; set; }

        [PropertyGroup("Events")]
        [EnableInBuilder(BuilderType.WebForm)]
        [PropertyEditor(PropertyEditorType.Collection)]
        public List<EbRoutines> OnApprovalRoutines { get; set; }

        [EnableInBuilder(BuilderType.WebForm)]
        public override string Label { get; set; }

        [EnableInBuilder(BuilderType.WebForm)]
        [DefaultPropValue("200")]
        [PropertyGroup("Appearance")]
        public override int Height { get; set; }

        [EnableInBuilder(BuilderType.WebForm)]
        [DefaultPropValue("true")]
        [PropertyGroup("Behavior")]
        [Alias("Serial numbered")]
        public bool IsShowSerialNumber { get; set; }

        [EnableInBuilder(BuilderType.WebForm)]
        [HideInPropertyGrid]
        public override string TableName { get { return "eb_approval_lines"; } }

        [EnableInBuilder(BuilderType.WebForm)]
        [PropertyEditor(PropertyEditorType.Collection)]
        [PropertyGroup("Behavior")]
        [Alias("Approval stages")]
        [ListType(typeof(ReviewStageAbstract))]
        [PropertyPriority(99)]
        public List<ReviewStageAbstract> FormStages { get; set; }

        [HideInPropertyGrid]
        [EnableInBuilder(BuilderType.WebForm)]
        [PropertyEditor(PropertyEditorType.Collection)]
        [PropertyGroup("Behavior")]
        [ListType(typeof(EbDGColumn))]
        [PropertyPriority(98)]
        [Alias("Columns")]
        public override List<EbControl> Controls { get; set; }

        [HideInPropertyGrid]
        [JsonIgnore]
        public override string ToolIconHtml { get { return "<i class='fa fa-stack-exchange'></i>"; } set { } }

        [HideInPropertyGrid]
        [JsonIgnore]
        public override string ToolNameAlias { get { return "Review New"; } set { } }

        //public override string GetToolHtml()
        //{
        //    return @"<div eb-type='@toolName' class='tool'><i class='fa fa-stack-exchange'></i>  Review control </div>".Replace("@toolName", this.GetType().Name.Substring(2));
        //}


        public override string GetBareHtml()
        {
            string html = @"
<div id='cont_@ebsid@' class='fs-grid-cont' style='height:@_height@px;'>
    <table id='tbl_@ebsid@' class='table table-bordered fs-tbl'>
        <thead>
            <tr>
            <th class='slno' style='width:50px'><span class='grid-col-title'>SL No</span></th>
            <th class='grid-col-title'><span class='grid-col-title'>Stage</span></th>
            <th style='display: none;' class='grid-col-title'><span class='grid-col-title'>Approver Role</span></th>
            <th style='width:100px;'><span class='grid-col-title'> Status</span></th>
            <th class='grid-col-title'><span class='grid-col-title'>Reviewed by/At</span></th>
            <th class='grid-col-title'><span class='grid-col-title'>Remarks</span></th>
            ".Replace("@_height@", (this.Height + 74).ToString());
            //foreach (EbFormStage FormStage in FormStages)
            //{
            //    if (!FormStage.Hidden)
            //        html += string.Concat("<th style='width: @Width@; @bg@' @type@ title='", FormStage.Title, "'><span class='grid-col-title'>", FormStage.Title, "</span>@req@</th>")
            //            .Replace("@req@", (FormStage.Required ? "<sup style='color: red'>*</sup>" : string.Empty))
            //            .Replace("@Width@", (FormStage.Width <= 0) ? "auto" : FormStage.Width.ToString() + "%")
            //            .Replace("@type@", "type = '" + FormStage.ObjType + "'")
            //            .Replace("@bg@", FormStage.IsDisable ? "background-color:#fafafa; color:#555" : string.Empty);
            //}

            html += @"
            </tr>
        </thead>";

            html += @"
        <tbody>";
            List<EbFormStage> _FormStages = JsonConvert.DeserializeObject<List<EbFormStage>>(JsonConvert.SerializeObject(FormStages));
            //_FormStages.Reverse();
            int i = 0;
            string FormStageTrHtml = string.Empty;

            foreach (ApprovalStageAbstract FormStage in _FormStages)
            {
                EbFormStage _FormStage = (FormStage as EbFormStage);
                EbReviewStage _FormStage_RS = (FormStages[i++] as EbReviewStage);

                string _html = string.Concat(@"
            <tr name='", _FormStage.Name, "' stage-ebsid='", _FormStage.EbSid, "' rowid='@rowid@' role='", _FormStage.ApproverRole.ToString(), "' style ='@bg@'>",
                    "<td class='row-no-td'>@slno@</td>",
                    "<td col='stage'><span class='fstd-div'>", _FormStage.Name, "</span></td>",
                    "<td style='display: none;'><span class='fstd-div'>", _FormStage.ApproverRole.ToString().Replace("_", " "), "</span></td>",
                    @"<td col='status' class='fs-ctrl-td'><div class='fstd-div'>", @"
                    <select class='selectpicker'>");

                foreach (EbReviewAction stageAction in _FormStage_RS.StageActions) {
                    string stageActionName = stageAction.Name;
                    _html += ("<option value='"+ stageAction.EbSid + "'>"+ stageAction.Name + "</option>");
                }
                _html += @"
                    </select></div>
                </td>
                <td col='review-dtls' class='fs-ctrl-td'>
                    <div class='fstd-div'>
                        <div class='fs-user-cont'>
                            <div class='fs-dp' @dpstyle@></div>
                            <div class='fs-udtls-cont'>
                                <span class='fs-uname'> @uname@ </span>
                                <span class='fs-time'> @time@ </span>
                            </div>
                        </div>
                    </div>
                </td>
                <td col='remarks' class='fs-ctrl-td'><div class='fstd-div'> <textarea class='fs-textarea'>@comment@</textarea> </div></td>
            </tr>";

                _FormStage_RS.Html = _html;
            }

            html += @"
        </tbody>
    </table>
    <div class='fs-submit-cont'><button class='btn btn-success fs-submit'>Execute Review <i class='fa fa-check-square-o' aria-hidden='true'></i></button></div>
</div>";

            return html;
        }

        public void InitRoles(JsonServiceClient serviceClient, User user)
        {
            var result = serviceClient.Get<GetAllRolesResponse>(new GetAllRolesRequest());
            this.Roles = new Dictionary<int, string>();
            foreach (string r in user.Roles)
            {
                var s = result.Roles.FirstOrDefault(kvp => kvp.Value == r);
                if (s.Value != null)
                {
                    this.Roles.Add(s.Key, s.Value);
                }
            }
        }

        [EnableInBuilder(BuilderType.WebForm)]
        [HideInPropertyGrid]
        public Dictionary<int, string> Roles { get; set; }

        public override string GetDesignHtml()
        {
            return GetHtml().RemoveCR().DoubleQuoted();
        }

        public override string GetHtml()
        {
            string EbCtrlHTML = HtmlConstants.CONTROL_WRAPER_HTML4WEB
               .Replace("@LabelForeColor ", "color:" + (LabelForeColor ?? "@LabelForeColor ") + ";")
               .Replace("@LabelBackColor ", "background-color:" + (LabelBackColor ?? "@LabelBackColor ") + ";");

            return ReplacePropsInHTML(EbCtrlHTML);
        }

    }

    public abstract class ReviewStageAbstract { }

    [UsedWithTopObjectParent(typeof(EbObject))]
    [EnableInBuilder(BuilderType.WebForm)]
    [Alias("Approval Stage")]
    public class EbReviewStage : ReviewStageAbstract
    {
        [EnableInBuilder(BuilderType.WebForm)]
        [HideInPropertyGrid]
        public string EbSid { get; set; }

        [EnableInBuilder(BuilderType.WebForm)]
        [HideInPropertyGrid]
        public string Html { get; set; }

        public EbReviewStage() { }
        public string ObjType { get { return this.GetType().Name.Substring(2, this.GetType().Name.Length - 2); } set { } }

        [EnableInBuilder(BuilderType.WebForm)]
        [Unique]
        public string Name { get; set; }

        [EnableInBuilder(BuilderType.WebForm)]
        [OnChangeExec(@"
if(this.ApproverEntity === 1){
    pg.MakeReadWrite('ApproverRole');
    pg.MakeReadOnly('ApproverUserGroup');
    pg.MakeReadOnly('ApproverUsers');
}
else if(this.ApproverEntity === 2){
    pg.MakeReadOnly('ApproverRole');
    pg.MakeReadWrite('ApproverUserGroup');
    pg.MakeReadOnly('ApproverUsers');
}
else if(this.ApproverEntity === 3){
    pg.MakeReadOnly('ApproverRole');
    pg.MakeReadOnly('ApproverUserGroup');
    pg.MakeReadWrite('ApproverUsers');
}")]
        public ApproverEntityTypes ApproverEntity { get; set; }

        [EnableInBuilder(BuilderType.WebForm)]
        [Unique]
        [PropDataSourceJsFn("return ebcontext.Roles")]
        [PropertyEditor(PropertyEditorType.DropDown)]
        public int ApproverRole { get; set; }

        [EnableInBuilder(BuilderType.WebForm)]
        [Unique]
        [PropDataSourceJsFn("return ebcontext.Roles")]
        [PropertyEditor(PropertyEditorType.DropDown, true)]
        public List<Int32> ApproverRoles { get; set; }

        [EnableInBuilder(BuilderType.WebForm)]
        [PropDataSourceJsFn("return ebcontext.UserGroups")]
        [PropertyEditor(PropertyEditorType.DropDown)]
        public int ApproverUserGroup { get; set; }

        [EnableInBuilder(BuilderType.WebForm)]
        [PropertyEditor(PropertyEditorType.ScriptEditorCS)]//required ScriptEditorSQ
        public EbScript ApproverUsers { get; set; }

        [EnableInBuilder(BuilderType.WebForm)]
        [PropertyEditor(PropertyEditorType.Collection)]
        [ListType(typeof(ReviewActionAbstract))]
        public List<ReviewActionAbstract> StageActions { get; set; }

        [EnableInBuilder(BuilderType.WebForm)]
        [PropertyEditor(PropertyEditorType.ScriptEditorCS)]
        public EbScript NextStage { get; set; }

        [EnableInBuilder(BuilderType.WebForm)]
        [HideInPropertyGrid]
        public Dictionary<string, string> QryParams { get; set; }//<param, table>

        [EnableInBuilder(BuilderType.WebForm)]
        public bool IsFormEditable { get; set; }
    }

    public abstract class ReviewActionAbstract { }

    [UsedWithTopObjectParent(typeof(EbObject))]
    [EnableInBuilder(BuilderType.WebForm)]
    [Alias("Approval Action")]
    public class EbReviewAction : ReviewActionAbstract
    {
        [EnableInBuilder(BuilderType.WebForm)]
        [HideInPropertyGrid]
        public string EbSid { get; set; }

        public EbReviewAction() { }
        public string ObjType { get { return this.GetType().Name.Substring(2, this.GetType().Name.Length - 2); } set { } }

        [EnableInBuilder(BuilderType.WebForm)]
        [Unique]
        public string Name { get; set; }
    }

    public enum ApproverEntityTypes
    {
        Role = 1,
        UserGroup,
        Users
    }
}
