﻿using ExpressBase.Common;
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
using ExpressBase.Objects.WebFormRelated;

namespace ExpressBase.Objects
{
    //[HideInToolBox]
    [EnableInBuilder(BuilderType.WebForm)]
    public class EbReview : EbControlContainer, IEbSpecialContainer, IEbExtraQryCtrl
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
                new EbDGStringColumn() { Name = FormConstants.stage_unique_id, EbDbType = EbDbTypes.String, Title = "Stage"},
                new EbDGStringColumn() { Name =  FormConstants.action_unique_id, EbDbType = EbDbTypes.String, Title = "Action"},
                new EbDGNumericColumn() { Name =  FormConstants.eb_my_actions_id, EbDbType = EbDbTypes.Decimal, Title = "My_Action_Id", Hidden = true},
                new EbDGStringColumn() { Name =  FormConstants.comments, EbDbType = EbDbTypes.String, Title = "Comments"},
                new EbDGDateColumn() { Name =  FormConstants.eb_created_at, EbDbType = EbDbTypes.DateTime, EbDateType = EbDateType.DateTime, DoNotPersist = true},
                new EbDGNumericColumn() { Name =  FormConstants.eb_created_by, EbDbType = EbDbTypes.Decimal, DoNotPersist = true}//,
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

        [PropertyGroup("Behavior")]
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

        [EnableInBuilder(BuilderType.WebForm)]
        [PropertyGroup("Behavior")]
        public override bool Hidden { get; set; }

        [EnableInBuilder(BuilderType.WebForm)]
        [PropertyGroup("Behavior")]
        public bool AllowEditOnCompletion { get; set; }

        [PropertyGroup("Behavior")]
        [EnableInBuilder(BuilderType.WebForm)]
        [PropertyEditor(PropertyEditorType.ScriptEditorCS)]
        public EbScript EntryCriteria { get; set; }

        [PropertyGroup("Behavior")]
        [EnableInBuilder(BuilderType.WebForm)]
        [Unique]
        [PropDataSourceJsFn("return ebcontext.Roles")]
        [PropertyEditor(PropertyEditorType.DropDown, true)]
        public List<Int32> ResetterRoles { get; set; }


        [PropertyGroup("Behavior")]
        [EnableInBuilder(BuilderType.WebForm)]
        public bool EnablePullBack { get; set; }

        [EnableInBuilder(BuilderType.WebForm)]
        [PropertyGroup(PGConstants.APPEARANCE)]
        [PropertyEditor(PropertyEditorType.ScriptEditorJS)]
        public EbScript HideRowExpr { get; set; }

        [EnableInBuilder(BuilderType.WebForm)]
        [PropertyGroup(PGConstants.APPEARANCE)]
        [PropertyEditor(PropertyEditorType.ScriptEditorJS)]
        public EbScript HideColumnExpr { get; set; }

        [EnableInBuilder(BuilderType.WebForm)]
        [PropertyGroup(PGConstants.APPEARANCE)]
        public string StageTitle { get; set; }

        [EnableInBuilder(BuilderType.WebForm)]
        [PropertyGroup(PGConstants.APPEARANCE)]
        public string StatusTitle { get; set; }

        [EnableInBuilder(BuilderType.WebForm)]
        [PropertyGroup(PGConstants.APPEARANCE)]
        public string ReviewedByTitle { get; set; }

        [EnableInBuilder(BuilderType.WebForm)]
        [PropertyGroup(PGConstants.APPEARANCE)]
        public string RemarksTitle { get; set; }

        [EnableInBuilder(BuilderType.WebForm)]
        [PropertyGroup(PGConstants.APPEARANCE)]
        public string ExecuteBtnText { get; set; }

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
                                                  <div class='msg-head'>", _FormStage.Name, @" @action@</div>
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
        <div class='rc-inp-head'></div>
        <div class='rc-action-dp-wrap'></div>
        <div class='rc-action-dd-wrap'></div>
        <textarea id='chatSend' placeholder='Add remark' class='rc-txtarea'></textarea>
        <div class='rc-send-btn-wrap'>
        <div class='fs-submit-cont'>
            <div class='btn btn-success fs-submit'>Execute Review <i class='fa fa-check-square-o' aria-hidden='true'></i></div></div>
        </div>
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
                        <th class='slno rc-slno'><span class='grid-col-title'>#</span></th>
                        <th class='grid-col-title rc-stage'><span class='grid-col-title'>@Stage@</span></th>
                        <th class='grid-col-title rc-status'><span class='grid-col-title'>@Status@</span></th>
                        <th class='grid-col-title rc-by'><span class='grid-col-title'>@Reviewed by/At@</span></th>
                        <th class='grid-col-title rc-remarks'><span class='grid-col-title'>@Remarks@</span></th>"
                        .Replace("@_height@", this.Height.ToString())
                        .Replace("@Stage@", string.IsNullOrWhiteSpace(StageTitle) ? "Stage" : StageTitle)
                        .Replace("@Status@", string.IsNullOrWhiteSpace(StatusTitle) ? "Status" : StatusTitle)
                        .Replace("@Reviewed by/At@", string.IsNullOrWhiteSpace(ReviewedByTitle) ? "Reviewed by/At" : ReviewedByTitle)
                        .Replace("@Remarks@", string.IsNullOrWhiteSpace(RemarksTitle) ? "Remarks" : RemarksTitle);
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
                    "<td class='row-no-td rc-stage' col='stage'><span class='fstd-div'>:", _FormStage.Name, ":</span></td>",
                    @"<td class='row-no-td rc-status' col='status' class='fs-ctrl-td'><div class='fstd-div'>", @"
                                <select class='selectpicker'><option value='' selected disabled> - select - </option>");

                foreach (EbReviewAction stageAction in _FormStage_RS.StageActions)
                {
                    string stageActionName = stageAction.Name;
                    _html += ("<option value='" + stageAction.EbSid + "'>:" + stageAction.Name + ":</option>");
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
                            <td class='fs-ctrl-td rc-remarks' col='remarks'><div class='fstd-div'> <textarea class='fs-textarea' style ='@bg@ @bgimg@'>@comment@</textarea> </div></td>
                        </tr>";

                _FormStage_RS.Html = _html;
            }

            html += @"
                    </tbody>
                </table>
            </div>
            <div class='fs-submit-cont' style='display: none;'><div class='btn btn-success fs-submit'>@Execute Review@ <i class='fa fa-check-square-o' aria-hidden='true'></i></div></div>
        </div>".Replace("@Execute Review@", string.IsNullOrWhiteSpace(ExecuteBtnText) ? "Execute Review" : ExecuteBtnText);

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
        [DefaultPropValue("true")]
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

        public string GetSelectQuery(IDatabase DataDB, string MasterTable, string form_ver_id, string form_ref_id)
        {
            return $@"SELECT A.id, S.stage_unique_id, A.is_form_data_editable, A.user_ids, A.role_ids, A.usergroup_id, A.description
                FROM eb_my_actions A, eb_stages S
                WHERE A.form_ref_id = '{form_ref_id}' AND A.form_data_id = @{MasterTable}_id AND 
                COALESCE(A.is_completed, 'F') = 'F' AND COALESCE(A.eb_del, 'F') = 'F' AND A.eb_stages_id = S.id AND COALESCE(S.eb_del, 'F') = 'F'; ";
        }

        public string GetSelectQuery(EbWebForm _Form, string _pshId, string del_col, string del_f_val)
        {
            return $@"SELECT A.id, S.stage_unique_id, A.is_form_data_editable, A.user_ids, A.role_ids, A.usergroup_id, A.description
                FROM eb_my_actions A, eb_stages S
                WHERE A.form_ref_id = '{_Form.RefId}' AND 
                A.form_data_id = (SELECT id FROM {_Form.TableName} WHERE {_Form.DataPusherConfig.SourceTable}_id = @{_Form.DataPusherConfig.SourceTable}_id {_pshId} AND COALESCE({del_col}, {del_f_val}) = {del_f_val} LIMIT 1) AND 
                COALESCE(A.is_completed, 'F') = 'F' AND COALESCE(A.eb_del, 'F') = 'F' AND A.eb_stages_id = S.id AND COALESCE(S.eb_del, 'F') = 'F'; ";
        }

        public static string GetPrimaryRoleNameQuery()
        {
            return ", (SELECT R.role_name FROM eb_roles R, eb_role2user R2U WHERE R2U.user_id=eb_created_by and R2U.role_id=R.id and R.is_primary='T' AND COALESCE(R2U.eb_del,'F')='F' AND COALESCE(R.eb_del,'F')='F' LIMIT 1) AS primary_role";
        }

        public void MergeFormData(WebformData FormData, WebFormSchema FormSchema)
        {
            if (FormData.MultipleTables.ContainsKey(this.TableName) && FormData.MultipleTables[this.TableName].Count > 0)
            {
                SingleTable rows = FormData.MultipleTables[this.TableName];
                for (int i = 0; i < rows.Count; i++)
                {
                    if (rows[i].RowId > 0)
                        rows.RemoveAt(i--);
                }
                if (rows.Count == 1)//one new entry// need to write code for 'AfterSaveRoutines'
                {
                    //foreach (TableSchema t in FormSchema.Tables)
                    //{
                    //    if (t.TableName != this.TableName)
                    //        FormData.MultipleTables.Remove(t.TableName);// approval execution, hence removing other data if present
                    //}
                    string[] str_t = { FormConstants.stage_unique_id, FormConstants.action_unique_id, FormConstants.eb_my_actions_id, FormConstants.comments };
                    for (int i = 0; i < str_t.Length; i++)
                    {
                        EbControl con = this.Controls.Find(e => e.Name == str_t[i]);
                        FormData.MultipleTables[this.TableName][0].SetEbDbType(con.Name, con.EbDbType);
                        FormData.MultipleTables[this.TableName][0].SetControl(con.Name, con);
                    }
                }
            }
        }

        public bool HasResetPermission(User _user)
        {
            bool flag = _user.RoleIds.Contains((int)SystemRoles.SolutionOwner) || _user.RoleIds.Contains((int)SystemRoles.SolutionAdmin);

            if (!flag && this.ResetterRoles != null)
                flag = _user.RoleIds.Select(x => x).Intersect(this.ResetterRoles).Any();

            return flag;
        }
    }

    public abstract class ReviewStageAbstract { }

    [UsedWithTopObjectParent(typeof(EbObject))]
    [EnableInBuilder(BuilderType.WebForm)]
    [Alias("Approval Stage")]
    public class EbReviewStage : ReviewStageAbstract
    {
        [EnableInBuilder(BuilderType.WebForm)]
        [PropertyGroup("Core")]
        [Alias("Unique Id")]
        public string EbSid { get; set; }

        [EnableInBuilder(BuilderType.WebForm)]
        [HideInPropertyGrid]
        public string Html { get; set; }

        [EnableInBuilder(BuilderType.WebForm)]
        [HideInPropertyGrid]
        public string DDHtml { get; set; }

        public EbReviewStage()
        {
            this.NotificationContent = new EbScript();
        }
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
pg.HideProperty('ApproverRoleQuery');
if (this.ApproverEntity === 1)
    pg.ShowProperty('ApproverRoles');
else if (this.ApproverEntity === 2)
    pg.ShowProperty('ApproverUserGroup');
else if (this.ApproverEntity === 3)
    pg.ShowProperty('ApproverUsers')
else if (this.ApproverEntity === 4)
    pg.ShowProperty('ApproverRoleQuery');
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
        [PropertyPriority(9)]
        [EnableInBuilder(BuilderType.WebForm)]
        [PropertyEditor(PropertyEditorType.ScriptEditorSQ)]
        public EbScript ApproverRoleQuery { get; set; }

        [PropertyGroup("Behavior")]
        [PropertyPriority(7)]
        [EnableInBuilder(BuilderType.WebForm)]
        [PropDataSourceJsFn("return ebcontext.UserGroups")]
        [PropertyEditor(PropertyEditorType.DropDown)]
        public int ApproverUserGroup { get; set; }

        [PropertyGroup("Behavior")]
        [PropertyPriority(8)]
        [EnableInBuilder(BuilderType.WebForm)]
        [PropertyEditor(PropertyEditorType.ScriptEditorSQ)]
        public EbScript ApproverUsers { get; set; }

        [PropertyGroup("Behavior")]
        [PropertyPriority(6)]
        [EnableInBuilder(BuilderType.WebForm)]
        [PropertyEditor(PropertyEditorType.ScriptEditorCS)]
        public EbScript NotificationContent { get; set; }

        [PropertyGroup("Behavior")]
        [PropertyPriority(3)]
        [EnableInBuilder(BuilderType.WebForm)]
        [PropertyEditor(PropertyEditorType.Collection)]
        [ListType(typeof(EbReviewAction))]
        public List<EbReviewAction> StageActions { get; set; }

        [PropertyGroup("Behavior")]
        [PropertyPriority(2)]
        [EnableInBuilder(BuilderType.WebForm)]
        [PropertyEditor(PropertyEditorType.ScriptEditorCS)]
        public EbScript NextStage { get; set; }

        [EnableInBuilder(BuilderType.WebForm)]
        [HideInPropertyGrid]
        public Dictionary<string, string> QryParams { get; set; }//<param, table>

        [PropertyGroup("Behavior")]
        [PropertyPriority(5)]
        [EnableInBuilder(BuilderType.WebForm)]
        public bool IsFormEditable { get; set; }

        [PropertyGroup("Behavior")]
        [PropertyPriority(6)]
        [EnableInBuilder(BuilderType.WebForm)]
        public bool HideNotification { get; set; }

        [PropertyGroup("Behavior")]
        [PropertyPriority(4)]
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

        [EnableInBuilder(BuilderType.WebForm)]
        [PropertyEditor(PropertyEditorType.Collection)]
        [HelpText("List of validators will be executed before the submission of review control.")]
        public List<EbValidator> Validators { get; set; }

        [EnableInBuilder(BuilderType.WebForm)]
        [PropertyEditor(PropertyEditorType.ScriptEditorJS)]
        public EbScript StageTextExpr { get; set; }

        [EnableInBuilder(BuilderType.WebForm)]
        [PropertyEditor(PropertyEditorType.Collection)]
        [Alias("Associated controls")]
        public List<ReviewAssociatedCtrl> AssocCtrls { get; set; }
    }

    public abstract class ReviewActionAbstract { }

    [UsedWithTopObjectParent(typeof(EbObject))]
    [EnableInBuilder(BuilderType.WebForm)]
    [Alias("Approval Action")]
    public class EbReviewAction : ReviewActionAbstract
    {
        [EnableInBuilder(BuilderType.WebForm)]
        [PropertyGroup("Core")]
        [Alias("Unique Id")]
        public string EbSid { get; set; }

        public EbReviewAction() { }
        public string ObjType { get { return this.GetType().Name.Substring(2, this.GetType().Name.Length - 2); } set { } }

        [PropertyGroup("Core")]
        [EnableInBuilder(BuilderType.WebForm)]
        [Unique]
        public string Name { get; set; }

        [PropertyGroup("Core")]
        [EnableInBuilder(BuilderType.WebForm)]
        public bool CommentsRequired { get; set; }

        [EnableInBuilder(BuilderType.WebForm)]
        [PropertyEditor(PropertyEditorType.ScriptEditorJS)]
        public EbScript ActionTextExpr { get; set; }

        [EnableInBuilder(BuilderType.WebForm)]
        [PropertyEditor(PropertyEditorType.ScriptEditorJS)]
        public EbScript HiddenExpr { get; set; }
    }

    [UsedWithTopObjectParent(typeof(EbObject))]
    [EnableInBuilder(BuilderType.WebForm)]
    [Alias("Associated control")]
    public class ReviewAssociatedCtrl
    {
        public ReviewAssociatedCtrl() { }

        [EnableInBuilder(BuilderType.WebForm)]
        [HideInPropertyGrid]
        public string Name { get; set; }

        [EnableInBuilder(BuilderType.WebForm)]
        public string ControlName { get; set; }
    }

    public enum ApproverEntityTypes
    {
        Role = 1,
        UserGroup,
        Users,
        DynamicRole
    }
}
