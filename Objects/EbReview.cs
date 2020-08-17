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
            FormStages = new List<EbReviewStage>();
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
        [ListType(typeof(EbReviewStage))]
        [PropertyPriority(99)]
        public List<EbReviewStage> FormStages { get; set; }

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
        public override string ToolNameAlias { get { return "Review"; } set { } }

        //public override string GetToolHtml()
        //{
        //    return @"<div eb-type='@toolName' class='tool'><i class='fa fa-stack-exchange'></i>  Review control </div>".Replace("@toolName", this.GetType().Name.Substring(2));
        //}


        public override string GetBareHtml()
        {
            if (this.RenderAsTable)
                return GetGetBareHtml4Table();

            string html = @"
<div id='cont_@ebsid@' class='fs-grid-cont rc-cmt' style='height:@_height@px;'>
    <div class='rc-msg-box'>
".Replace("@_height@", (this.Height + 74).ToString());
            List<EbReviewStage> _FormStages = JsonConvert.DeserializeObject<List<EbReviewStage>>(JsonConvert.SerializeObject(FormStages));
            //_FormStages.Reverse();
            int i = 0;
            string FormStageTrHtml = string.Empty;

            foreach (EbReviewStage FormStage in _FormStages)
            {
                EbReviewStage _FormStage = (FormStage as EbReviewStage);
                EbReviewStage _FormStage_RS = (FormStages[i++] as EbReviewStage);

                string _html = string.Concat(@"
                                            <div class='message' rowid='@rowid@' name='", _FormStage.Name, @"' stage-ebsid='", _FormStage.EbSid, @"' rowid='@rowid@'>
                                               <div class='fs-dp' @dpstyle@></div>
                                               <div class='bubble'>
                                                  <div class='msg-head'>", _FormStage.Name, @" (@action@)</div>
                                                  <div class='msg-comment'>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;@comment@</div>
                                                  <span class='msg-uname'>@uname@</span>
                                                  <div class='corner'></div>
                                                  <span data-toggle='tooltip' title data-original-title='@timeTitle@'>@time@</span>
                                               </div>
                                            </div>");

                string _DDhtml = "<select class='selectpicker' data-container='body'>";

                foreach (EbReviewAction stageAction in _FormStage_RS.StageActions)
                {
                    string stageActionName = stageAction.Name;
                    _DDhtml += ("<option value='" + stageAction.EbSid + "'>" + stageAction.Name + "</option>");
                }
                _DDhtml += "</select>";

                _FormStage_RS.Html = _html;
                _FormStage_RS.DDHtml = _DDhtml;
            }

            html += @"
    </div>
    <div class='rc-inp-cont'>
        <div class='rc-action-dp-wrap'></div>
        <div class='rc-action-dd-wrap'></div>
        <textarea id='chatSend' placeholder='Add remark' class='rc-txtarea'></textarea>
        <div class='rc-send-btn-wrap'><div class='fs-submit-cont'><button class='btn btn-success fs-submit'>Execute Review <i class='fa fa-check-square-o' aria-hidden='true'></i></button></div></div>
    </div>
</div>";

            return html;
        }

        private string GetGetBareHtml4Table()
        {
            string html = @"
        <div id='cont_@ebsid@' class='fs-grid-cont' style='height:@_height@px;'>
            <div class='rc-tbl-thead-cont'>
                <table class='table table-bordered fs-tblhead'>
                    <thead>
                        <tr>
                        <th class='slno rc-slno' style='width:50px'><span class='grid-col-title'>SL No</span></th>
                        <th class='grid-col-title rc-stage'><span class='grid-col-title'>Stage</span></th>
                        <th class='grid-col-title rc-status'><span class='grid-col-title'> Status</span></th>
                        <th class='grid-col-title rc-by'><span class='grid-col-title'>Reviewed by/At</span></th>
                        <th class='grid-col-title rc-remarks'><span class='grid-col-title'>Remarks</span></th>
                        ".Replace("@_height@", (this.Height + 74).ToString());
            html += @"
                        </tr>
                    </thead>
                </table>
            </div>
            <div class='rc-tbl-tbody-cont'>
                <table id='tbl_@ebsid@' class='table table-bordered fs-tbl'>
                    <tbody>";
            List<EbReviewStage> _FormStages = JsonConvert.DeserializeObject<List<EbReviewStage>>(JsonConvert.SerializeObject(FormStages));
            //_FormStages.Reverse();
            int i = 0;
            string FormStageTrHtml = string.Empty;

            foreach (EbReviewStage FormStage in _FormStages)
            {
                EbReviewStage _FormStage = (FormStage as EbReviewStage);
                EbReviewStage _FormStage_RS = (FormStages[i++] as EbReviewStage);

                string _html = string.Concat(@"
                        <tr name='", _FormStage.Name, "' stage-ebsid='", _FormStage.EbSid, "' rowid='@rowid@' style ='@bg@'>",
                    "<td class='row-no-td rc-slno'>@slno@</td>",
                    "<td class='row-no-td rc-stage' col='stage'><span class='fstd-div'>", _FormStage.Name, "</span></td>",
                    @"<td class='row-no-td rc-status' col='status' class='fs-ctrl-td'><div class='fstd-div'>", @"
                                <select class='selectpicker'>");

                foreach (EbReviewAction stageAction in _FormStage_RS.StageActions)
                {
                    string stageActionName = stageAction.Name;
                    _html += ("<option value='" + stageAction.EbSid + "'>" + stageAction.Name + "</option>");
                }
                _html += @"
                                </select></div>
                            </td>
                            <td class='fs-ctrl-td rc-by' col='review-dtls'>
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
                            <td class='fs-ctrl-td rc-remarks' col='remarks'><div class='fstd-div'> <textarea class='fs-textarea'>@comment@</textarea> </div></td>
                        </tr>";

                _FormStage_RS.Html = _html;
            }

            html += @"
                    </tbody>
                </table>
            </div>
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

        [PropertyGroup(PGConstants.APPEARANCE)]
        [PropertyPriority(6)]
        [EnableInBuilder(BuilderType.WebForm)]
        public bool RenderAsTable { get; set; }

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

        public string GetSelectQuery(string RefId, string MasterTable)
        {
            return $@"SELECT A.id, S.stage_unique_id, A.is_form_data_editable, A.user_ids, A.role_ids, A.usergroup_id
                FROM eb_my_actions A, eb_stages S
                WHERE A.form_ref_id = '{RefId}' AND A.form_data_id = @{MasterTable}_id AND 
                A.is_completed = 'F' AND A.eb_del = 'F' AND A.eb_stages_id = S.id AND S.eb_del = 'F'; ";
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

        [EnableInBuilder(BuilderType.WebForm)]
        [HideInPropertyGrid]
        public string DDHtml { get; set; }

        public EbReviewStage() { }
        public string ObjType { get { return this.GetType().Name.Substring(2, this.GetType().Name.Length - 2); } set { } }

        [PropertyGroup("Core")]
        [PropertyPriority(20)]
        [EnableInBuilder(BuilderType.WebForm)]
        [Unique]
        public string Name { get; set; }

        [PropertyGroup("Behavior")]
        [PropertyPriority(10)]
        [EnableInBuilder(BuilderType.WebForm)]
        //MakeReadOnly MakeReadWrite ShowProperty HideProperty
        [OnChangeExec(@"
if (this.ApproverEntity === 0) this.ApproverEntity = 1;
pg.HideProperty('ApproverRoles');
pg.HideProperty('ApproverUserGroup');
pg.HideProperty('ApproverUsers');
if (this.ApproverEntity === 1)
    pg.ShowProperty('ApproverRoles');
else if (this.ApproverEntity === 2)
    pg.ShowProperty('ApproverUserGroup');
else if (this.ApproverEntity === 3)
    pg.ShowProperty('ApproverUsers');
")]
        public ApproverEntityTypes ApproverEntity { get; set; }

        //[EnableInBuilder(BuilderType.WebForm)]
        //[Unique]
        //[PropDataSourceJsFn("return ebcontext.Roles")]
        //[PropertyEditor(PropertyEditorType.DropDown)]
        //public int ApproverRole { get; set; }

        [PropertyGroup("Behavior")]
        [PropertyPriority(9)]
        [EnableInBuilder(BuilderType.WebForm)]
        [Unique]
        [PropDataSourceJsFn("return ebcontext.Roles")]
        [PropertyEditor(PropertyEditorType.DropDown, true)]
        public List<Int32> ApproverRoles { get; set; }

        [PropertyGroup("Behavior")]
        [PropertyPriority(7)]
        [EnableInBuilder(BuilderType.WebForm)]
        [PropDataSourceJsFn("return ebcontext.UserGroups")]
        [PropertyEditor(PropertyEditorType.DropDown)]
        public int ApproverUserGroup { get; set; }

        [PropertyGroup("Behavior")]
        [PropertyPriority(8)]
        [EnableInBuilder(BuilderType.WebForm)]
        [PropertyEditor(PropertyEditorType.ScriptEditorCS)]//required ScriptEditorSQ
        public EbScript ApproverUsers { get; set; }

        [PropertyGroup("Behavior")]
        [PropertyPriority(4)]
        [EnableInBuilder(BuilderType.WebForm)]
        [PropertyEditor(PropertyEditorType.Collection)]
        [ListType(typeof(EbReviewAction))]
        public List<EbReviewAction> StageActions { get; set; }

        [PropertyGroup("Behavior")]
        [PropertyPriority(3)]
        [EnableInBuilder(BuilderType.WebForm)]
        [PropertyEditor(PropertyEditorType.ScriptEditorCS)]
        public EbScript NextStage { get; set; }

        [EnableInBuilder(BuilderType.WebForm)]
        [HideInPropertyGrid]
        public Dictionary<string, string> QryParams { get; set; }//<param, table>

        [PropertyGroup("Behavior")]
        [PropertyPriority(6)]
        [EnableInBuilder(BuilderType.WebForm)]
        public bool IsFormEditable { get; set; }

        [PropertyGroup("Behavior")]
        [PropertyPriority(5)]
        [EnableInBuilder(BuilderType.WebForm)]
        [OnChangeExec(@"
if(this.IsAdvanced === true){
    pg.ShowProperty('NextStage');
    pg.ShowProperty('StageActions');
}
else{
    pg.HideProperty('NextStage');
    pg.HideProperty('StageActions');
}")]
        public bool IsAdvanced { get; set; }
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

        [PropertyGroup("Core")]
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
