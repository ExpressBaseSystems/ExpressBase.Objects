using ExpressBase.Common;
using ExpressBase.Common.Constants;
using ExpressBase.Common.Objects;
using ExpressBase.Common.Structures;
using ExpressBase.CoreBase.Globals;
using ExpressBase.Objects.ServiceStack_Artifacts;
using ServiceStack;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Net;

namespace ExpressBase.Objects.WebFormRelated
{
    internal static class ReviewStatus
    {
        public const string In_Process = "In Process";
        public const string Completed = "Completed";
        public const string Abandoned = "Abandoned";
    }

    internal enum ReviewStatusEnum
    {
        In_Process = 1,
        Completed = 2,
        Abandoned = 3
    }

    public static class EbReviewHelper
    {
        public static string GetMyActionInsertUpdateQuery(EbWebForm webForm, IDatabase DataDB, List<DbParameter> param, ref int i, bool isInsert, Service service)
        {
            EbReviewHelper_inner inner = new EbReviewHelper_inner(webForm, DataDB, param, isInsert, service);
            if (inner.ReviewNotFound) return string.Empty;
            return inner.GetMyActionInsertUpdateQuery(ref i);
        }

        public static string GetMyActionInsertUpdateQueryxx(EbWebForm webForm, IDatabase DataDB, List<DbParameter> param, ref int i, Service service)
        {
            EbReviewHelper_inner inner = new EbReviewHelper_inner(webForm, DataDB, param, false, service);
            if (inner.ReviewNotFound) return string.Empty;
            return inner.GetMyActionInsertUpdateQueryxx(ref i);
        }

        public static bool CheckReviewCompatibility(EbWebForm webForm)
        {
            if (webForm.TableRowId <= 0)
                throw new FormException("Bad Request. [34]", (int)HttpStatusCode.BadRequest, "Review operation failed due to invalid data id", "EbReviewHelper -> CheckReviewCompatibility");

            EbReview ebReview = (EbReview)webForm.FormSchema.ExtendedControls.Find(e => e is EbReview);
            if (ebReview == null)
                throw new FormException("Review control not found! Contact Admin.", (int)HttpStatusCode.BadRequest, "check the form refid and review control", "EbReviewHelper -> CheckReviewCompatibility");
            bool ok = false;
            if (webForm.FormData?.MultipleTables.ContainsKey(ebReview.TableName) == true)
            {
                SingleTable Table = webForm.FormData.MultipleTables[ebReview.TableName];
                if (Table != null && Table.Count > 0)
                {
                    Table.RemoveAll(e => e.RowId > 0);
                    if (Table.Count == 1)
                        ok = true;
                }
            }
            if (!ok)
                throw new FormException("Bad review request", (int)HttpStatusCode.BadRequest, "check the form refid and review control", "EbReviewHelper -> CheckReviewCompatibility");
            return true;
        }
    }

    class EbReviewHelper_inner
    {
        private EbWebForm webForm { get; set; }
        private IDatabase DataDB { get; set; }
        private List<DbParameter> param { get; set; }
        private bool isInsert { get; set; }
        private Service service { get; set; }
        private EbReview ebReview { get; set; }
        private FG_Root globals { get; set; }
        private SingleTable Table { get; set; }
        private SingleTable TableBkUp { get; set; }
        public bool ReviewNotFound { get; set; }

        internal EbReviewHelper_inner(EbWebForm webForm, IDatabase DataDB, List<DbParameter> param, bool isInsert, Service service)
        {
            this.ebReview = (EbReview)webForm.FormSchema.ExtendedControls.FirstOrDefault(e => e is EbReview);
            if (this.ebReview == null || this.ebReview.FormStages.Count == 0)
            {
                this.ReviewNotFound = true;
                return;
            }

            this.webForm = webForm;
            this.DataDB = DataDB;
            this.param = param;
            this.isInsert = isInsert;
            this.service = service;
            this.globals = null;

            if (this.webForm.FormData?.MultipleTables.ContainsKey(this.ebReview.TableName) == true)
                this.Table = this.webForm.FormData.MultipleTables[this.ebReview.TableName];
            else
                this.Table = new SingleTable();

            if (this.webForm.FormDataBackup?.MultipleTables.ContainsKey(this.ebReview.TableName) == true)
                this.TableBkUp = this.webForm.FormDataBackup.MultipleTables[this.ebReview.TableName];
            else
                this.TableBkUp = new SingleTable();
        }

        public string GetMyActionInsertUpdateQueryxx(ref int i)
        {
            string insUpQ = string.Empty, masterId = $"@{this.webForm.TableName}_id"; ;
            bool insMyActRequired = false, entryCriteriaRslt = true;
            EbReviewStage nextStage = null;

            if (Convert.ToString(this.Table[0][FormConstants.stage_unique_id]) == FormConstants.__system_stage &&
                Convert.ToString(this.Table[0][FormConstants.action_unique_id]) == FormConstants.__review_reset)
            {
                insUpQ += this.GetApprovalLinesInsertQry(ref i);
                bool hasRoleMatch = false;
                if (this.ebReview.ResetterRoles != null)
                    hasRoleMatch = this.webForm.UserObj.RoleIds.Select(x => x).Intersect(this.ebReview.ResetterRoles).Any() ||
                        this.webForm.UserObj.RoleIds.Contains((int)SystemRoles.SolutionOwner) ||
                        this.webForm.UserObj.RoleIds.Contains((int)SystemRoles.SolutionAdmin);
                if (!hasRoleMatch)
                    throw new FormException("Access denied to reset review control", (int)HttpStatusCode.Unauthorized, $"User.RolesId does not contains any of permited roleIds[ResetterRoles]", "From GetMyActionInsertUpdateQuery");

                SingleRow RowBkUp = this.TableBkUp.Find(e => e.RowId <= 0);
                if (RowBkUp != null)
                    insUpQ += this.GetMyActionDeleteQry(ref i, Convert.ToInt32(RowBkUp[FormConstants.eb_my_actions_id]));

                if (!string.IsNullOrWhiteSpace(this.ebReview.EntryCriteria?.Code))
                {
                    this.globals = GlobalsGenerator.GetCSharpFormGlobals_NEW(this.webForm, this.webForm.FormData, this.webForm.FormDataBackup, this.DataDB, null, false);
                    object retval = this.webForm.ExecuteCSharpScriptNew(this.ebReview.EntryCriteria.Code, this.globals);//status or stage

                    if (retval is bool)
                    {
                        bool.TryParse(Convert.ToString(retval), out entryCriteriaRslt);
                    }
                    else if (retval is FG_Review_Stage fg_stage)
                    {
                        nextStage = this.ebReview.FormStages.Find(e => e.Name == fg_stage.name);
                    }
                }
                if (entryCriteriaRslt)
                {
                    if (nextStage == null)
                        nextStage = this.ebReview.FormStages[0];
                }
                else if (this.TableBkUp.Find(e => e.RowId > 0) != null)
                {
                    insUpQ += this.GetApprovalUpdateQry(null, true, false);
                    insUpQ += this.GetApprovalLinesDeleteQry();
                }
            }
            else
            {
                nextStage = this.ExecuteOneStage(ref insUpQ, ref i, ref insMyActRequired, true);
            }

            if (nextStage != null)
            {
                string[] _col_val = this.GetApproverEntityValues(ref i, nextStage, out _);
                string autoId = this.GetAutoId(masterId);
                string description = this.GetDescription(nextStage, autoId);

                insUpQ += this.GetMyActionInsertQry(_col_val, nextStage, masterId, description);
                if (this.DataDB.Vendor == DatabaseVendors.MYSQL)
                    insUpQ += "SELECT eb_persist_currval('eb_my_actions_id_seq'); ";

                this.webForm.MyActNotification.Title = "Review required";
                Console.WriteLine("Will try to INSERT eb_my_actions");

                insUpQ += this.GetApprovalUpdateQry(null, false, true);
            }
            return insUpQ;
        }

        public string GetMyActionInsertUpdateQuery(ref int i)
        {
            string insUpQ = string.Empty, masterId = $"@{this.webForm.TableName}_id";
            bool insMyActRequired = false, insInEdit = false, entryCriteriaRslt = true, entryCriteriaExecuted = false;
            EbReviewStage nextStage = null;
            if (!string.IsNullOrWhiteSpace(this.ebReview.EntryCriteria?.Code))
            {
                this.globals = GlobalsGenerator.GetCSharpFormGlobals_NEW(this.webForm, this.webForm.FormData, this.webForm.FormDataBackup, this.DataDB, null, false);
                object retval = this.webForm.ExecuteCSharpScriptNew(this.ebReview.EntryCriteria.Code, this.globals);
                if (retval is bool)
                {
                    bool.TryParse(Convert.ToString(retval), out entryCriteriaRslt);
                }
                else if (retval is FG_Review_Stage fg_stage)
                {
                    nextStage = this.ebReview.FormStages.Find(e => e.Name == fg_stage.name);
                }
                entryCriteriaExecuted = true;
            }
            if (this.isInsert)
            {
                if (entryCriteriaRslt)
                {
                    masterId = $"(SELECT eb_currval('{this.webForm.TableName}_id_seq'))";

                    if (entryCriteriaExecuted && this.IsAutoApproveRequired(ref i, ref insUpQ, masterId))
                        return insUpQ;
                    if (nextStage == null)
                        nextStage = this.ebReview.FormStages[0];
                }
                else
                    return string.Empty;
            }
            else
            {
                if (!entryCriteriaRslt)
                    insUpQ += this.DeleteIfExists(ref i);
                else
                {
                    if (this.Table.Count == 1)
                    {
                        nextStage = this.ExecuteOneStage(ref insUpQ, ref i, ref insMyActRequired, false);
                    }
                    else if (this.Table.Count == 0)
                    {
                        if (this.TableBkUp.Count == 0)
                        {
                            insInEdit = true;
                            if (nextStage == null)
                                nextStage = this.ebReview.FormStages[0];
                        }
                        if (!insInEdit)
                        {
                            Console.WriteLine("No items reviewed in this form data save");
                            return string.Empty;
                        }
                    }
                    else
                        throw new FormException("Bad Request for review control", (int)HttpStatusCode.BadRequest, "eb_approval_lines contains more than one rows, only one review allowed at a time", "From GetMyActionInsertUpdateQuery");
                }
            }

            if (this.isInsert || insMyActRequired || insInEdit)// first save or insert myaction required in edit
            {
                string[] _col_val = this.GetApproverEntityValues(ref i, nextStage, out _);
                string autoId = this.GetAutoId(masterId);
                string description = this.GetDescription(nextStage, autoId);
                insUpQ += this.InsertMyActionAndApproval(nextStage, _col_val, masterId, description, insInEdit);
            }

            return insUpQ;
        }

        private EbReviewStage ExecuteOneStage(ref string insUpQ, ref int i, ref bool insMyActRequired, bool ApprovalFlow)
        {
            EbReviewStage nextStage = null;
            insUpQ += this.GetApprovalLinesInsertQry(ref i);
            insUpQ += this.UpdateMyAction(ref i);

            if (!(this.ebReview.FormStages.Find(e => e.EbSid == Convert.ToString(this.Table[0][FormConstants.stage_unique_id])) is EbReviewStage currentStage))
                throw new FormException("Bad Request", (int)HttpStatusCode.BadRequest, $"eb_approval_lines contains an invalid stage_unique_id: {this.Table[0]["stage_unique_id"]} ", "From GetMyActionInsertUpdateQuery");

            if (!(currentStage.StageActions.Find(e => e.EbSid == Convert.ToString(this.Table[0][FormConstants.action_unique_id])) is EbReviewAction currentAction))
                throw new FormException("Bad Request", (int)HttpStatusCode.BadRequest, $"eb_approval_lines contains an invalid action_unique_id: {this.Table[0]["action_unique_id"]} ", "From GetMyActionInsertUpdateQuery");

            if (currentAction.CommentsRequired && string.IsNullOrWhiteSpace(Convert.ToString(this.Table[0]["comments"])))
                throw new FormException("Comments required to complete the review", (int)HttpStatusCode.BadRequest, $"Comments required for stage: {currentStage.EbSid}, action: {currentAction.EbSid} ", "From GetMyActionInsertUpdateQuery");

            if (this.globals == null)
                this.globals = GlobalsGenerator.GetCSharpFormGlobals_NEW(this.webForm, this.webForm.FormData, this.webForm.FormDataBackup, this.DataDB, null, false);

            object stageObj = this.webForm.ExecuteCSharpScriptNew(currentStage.NextStage.Code, this.globals);
            string nxtStName = string.Empty;
            if (stageObj is FG_Review_Stage)
                nxtStName = (stageObj as FG_Review_Stage).name;

            GlobalsGenerator.PostProcessGlobals(this.webForm, this.globals, this.service);
            string _reviewStatus = this.globals.form.review._ReviewStatus;
            if (_reviewStatus == ReviewStatus.Completed || _reviewStatus == ReviewStatus.Abandoned)
            {
                if (ApprovalFlow)
                    this.webForm.AfterSaveRoutines = this.ebReview.OnApprovalRoutines;
                else
                    this.webForm.AfterSaveRoutines.AddRange(this.ebReview.OnApprovalRoutines);
                insMyActRequired = false;
                insUpQ += this.GetApprovalUpdateQry(_reviewStatus, false, false);
            }
            else
            {
                EbReviewStage nxtSt = currentStage;
                if (!nxtStName.IsNullOrEmpty())
                    nxtSt = this.ebReview.FormStages.Find(e => e.Name == nxtStName);

                if (nxtSt != null)
                {
                    //backtrack to the same user - code here if needed
                    nextStage = nxtSt;
                    insMyActRequired = true;
                }
                else
                    throw new FormException("Unable to decide next stage", (int)HttpStatusCode.InternalServerError, "NextStage C# script returned a value that is not recognized as a stage", "Return value : " + nxtStName);
            }
            return nextStage;
        }

        private bool IsAutoApproveRequired(ref int i, ref string query, string masterId)
        {
            EbReviewAction _action;
            EbReviewStage _stage;
            string comments;
            if (this.globals.form.review._ReviewStatus == "AutoCompleted")
            {
                if (this.globals.form.review?._Stage?.name == null || this.globals.form.review?._Action == null)
                    return false;

                _stage = this.ebReview.FormStages.Find(e => e.Name == this.globals.form.review?._Stage?.name);
                if (_stage == null)
                    return false;
                _action = _stage.StageActions.Find(e => e.Name == this.globals.form.review._Action);
                if (_action == null)
                    return false;

                if (this.globals.form.review?._Comments == null)
                    comments = "Auto approved";
                else
                    comments = Convert.ToString(this.globals.form.review._Comments);
            }
            else
                return false;

            string autoId = this.GetAutoId(masterId);
            string description = this.GetDescription(_stage, autoId);
            string[] _col_val = this.GetApproverEntityValues(ref i, _stage, out bool hasPerm);
            if (!hasPerm)
                return false;

            query += $@"
INSERT INTO eb_my_actions(
    {_col_val[0]}, 
    from_datetime, 
    is_completed, 
    eb_stages_id, 
    form_ref_id, 
    form_data_id, 
    eb_del, 
    description, 
    is_form_data_editable, 
    my_action_type,
    action_type,
    hide,
    completed_at,
    completed_by
)
VALUES (
    {_col_val[1]}, 
    {this.DataDB.EB_CURRENT_TIMESTAMP}, 
    'T', 
    (SELECT id FROM eb_stages WHERE stage_unique_id = '{_stage.EbSid}' AND form_ref_id = '{this.webForm.RefId}' AND eb_del = 'F'), 
    '{this.webForm.RefId}', 
    {masterId}, 
    'F', 
    '{description}', 
    '{(_stage.IsFormEditable ? "T" : "F")}', 
    '{MyActionTypes.Approval}',
    {(int)MyActionTypes.Approval},
    '{(_stage.HideNotification ? "T" : "F")}',
    {this.DataDB.EB_CURRENT_TIMESTAMP},
    @eb_createdby
); ";

            query += $@"
INSERT INTO eb_approval(
    review_status, 
    status,
    eb_my_actions_id, 
    eb_src_id, 
    eb_ver_id, 
    eb_created_by, 
    eb_created_at, 
    eb_del,
    eb_lastmodified_by,
    eb_lastmodified_at,
    eb_loc_id
)
VALUES(
    '{ReviewStatus.Completed}', 
    {(int)ReviewStatusEnum.Completed},
    (SELECT eb_currval('eb_my_actions_id_seq')), 
    {masterId}, 
    @{this.webForm.TableName}_eb_ver_id, 
    @eb_createdby, 
    {this.DataDB.EB_CURRENT_TIMESTAMP}, 
    'F',
    @eb_createdby,
    {this.DataDB.EB_CURRENT_TIMESTAMP},
    @eb_loc_id
); ";

            query += $@"
INSERT INTO eb_approval_lines (
    stage_unique_id,
    action_unique_id,
    eb_my_actions_id,
    comments,
    eb_created_by, 
    eb_created_at, 
    eb_loc_id, 
    eb_src_id, 
    eb_ver_id, 
    eb_signin_log_id,
    eb_approval_id
) 
VALUES (
    @stage_unique_id_{i},
    @action_unique_id_{i},
    (SELECT eb_currval('eb_my_actions_id_seq')),
    @comments_{i},
    @eb_createdby, 
    {DataDB.EB_CURRENT_TIMESTAMP}, 
    @eb_loc_id, 
    {masterId},
    @{this.webForm.TableName}_eb_ver_id, 
    @eb_signin_log_id, 
    (SELECT eb_currval('eb_approval_id_seq'))
);

UPDATE 
    eb_my_actions 
SET 
    eb_approval_lines_id = (SELECT eb_currval('eb_approval_lines_id_seq'))
WHERE
    id = (SELECT eb_currval('eb_my_actions_id_seq'));

UPDATE 
    eb_approval 
SET 
    eb_approval_lines_id = (SELECT eb_currval('eb_approval_lines_id_seq'))
WHERE
    id = (SELECT eb_currval('eb_approval_id_seq'));";

            this.param.Add(this.DataDB.GetNewParameter($"stage_unique_id_{i}", EbDbTypes.String, _stage.EbSid));
            this.param.Add(this.DataDB.GetNewParameter($"action_unique_id_{i}", EbDbTypes.String, _action.EbSid));
            this.param.Add(this.DataDB.GetNewParameter($"comments_{i++}", EbDbTypes.String, comments));

            return true;
        }

        private string InsertMyActionAndApproval(EbReviewStage nextStage, string[] _col_val, string masterId, string description, bool insInEdit)
        {
            string insUpQ = this.GetMyActionInsertQry(_col_val, nextStage, masterId, description);
            if (this.DataDB.Vendor == DatabaseVendors.MYSQL)
                insUpQ += "SELECT eb_persist_currval('eb_my_actions_id_seq'); ";

            this.webForm.MyActNotification.Title = "Review required";
            Console.WriteLine("Will try to INSERT eb_my_actions");

            if (this.isInsert)// eb_approval - insert entry here
                insUpQ += this.GetApprovalInsertQry(true);
            else if (insInEdit)
                insUpQ += this.GetApprovalInsertQry(false);
            else // eb_approval - update eb_my_actions_id
                insUpQ += this.GetApprovalUpdateQry(null, false, false);
            return insUpQ;
        }

        private string GetDescription(EbReviewStage nextStage, string autoId)
        {
            string description = null;
            if (!string.IsNullOrEmpty(nextStage.NotificationContent?.Code?.Trim()))
            {
                if (this.globals == null)
                    this.globals = GlobalsGenerator.GetCSharpFormGlobals_NEW(this.webForm, this.webForm.FormData, this.webForm.FormDataBackup, this.DataDB, null, false);
                object msg = this.webForm.ExecuteCSharpScriptNew(nextStage.NotificationContent.Code, this.globals);
                description = Convert.ToString(msg);
                if (!string.IsNullOrEmpty(description))
                {
                    if (this.webForm.AutoId != null && this.isInsert && description.Contains(FG_Constants.AutoId_PlaceHolder))
                        description = description.Replace(FG_Constants.AutoId_PlaceHolder, autoId);
                }
            }
            if (string.IsNullOrEmpty(description))
                description = $"{this.webForm.DisplayName} {(autoId.IsEmpty() ? string.Empty : (CharConstants.SPACE + autoId))}in {nextStage.Name}";
            return description;
        }

        private string GetAutoId(string masterId)
        {

            string autoId = string.Empty;
            if (this.webForm.AutoId != null)
            {
                if (this.isInsert)
                    autoId = $" ' || (SELECT {this.webForm.AutoId.Name} FROM {this.webForm.AutoId.TableName} WHERE {(this.webForm.AutoId.TableName == this.webForm.TableName ? string.Empty : (this.webForm.TableName + CharConstants.UNDERSCORE))}id = {masterId}) || ' ";
                else if (this.webForm.FormDataBackup.MultipleTables.TryGetValue(this.webForm.AutoId.TableName, out SingleTable _Table) && _Table.Count > 0)
                    autoId = CharConstants.SPACE + Convert.ToString(_Table[0][this.webForm.AutoId.Name]) + CharConstants.SPACE;
            }
            return autoId;
        }

        private string[] GetApproverEntityValues(ref int i, EbReviewStage nextStage, out bool hasPerm)
        {
            string _col = string.Empty, _val = string.Empty;
            this.webForm.MyActNotification = new MyActionNotification() { ApproverEntity = nextStage.ApproverEntity };
            if (nextStage.ApproverEntity == ApproverEntityTypes.Role)
            {
                _col = "role_ids";
                _val = $"@role_ids_{i}";
                string roles = nextStage.ApproverRoles == null ? string.Empty : nextStage.ApproverRoles.Join(",");
                this.param.Add(this.DataDB.GetNewParameter($"role_ids_{i++}", EbDbTypes.String, roles));
                this.webForm.MyActNotification.RoleIds = nextStage.ApproverRoles;
                hasPerm = this.webForm.UserObj.RoleIds.Any(e => nextStage.ApproverRoles.Contains(e));
            }
            else if (nextStage.ApproverEntity == ApproverEntityTypes.UserGroup)
            {
                _col = "usergroup_id";
                _val = $"@usergroup_id_{i}";
                this.param.Add(this.DataDB.GetNewParameter($"usergroup_id_{i++}", EbDbTypes.Int32, nextStage.ApproverUserGroup));
                this.webForm.MyActNotification.UserGroupId = nextStage.ApproverUserGroup;
                hasPerm = this.webForm.UserObj.UserGroupIds.Any(e => e == nextStage.ApproverUserGroup);
            }
            else if (nextStage.ApproverEntity == ApproverEntityTypes.Users || nextStage.ApproverEntity == ApproverEntityTypes.DynamicRole)
            {
                string t1 = string.Empty, t2 = string.Empty, t3 = string.Empty;
                List<DbParameter> _params = new List<DbParameter>();
                int _idx = 0, ErrCod = (int)HttpStatusCode.BadRequest;
                string ErrMsg = "GetFirstMyActionInsertQuery: Review control parameter {0} is not idetified";
                foreach (KeyValuePair<string, string> p in nextStage.QryParams)
                {
                    if (EbFormHelper.IsExtraSqlParam(p.Key, this.webForm.TableName))
                        continue;
                    SingleTable _Table = null;
                    if (this.webForm.FormData.MultipleTables.ContainsKey(p.Value))
                        _Table = this.webForm.FormData.MultipleTables[p.Value];
                    else if (this.webForm.FormDataBackup != null && this.webForm.FormDataBackup.MultipleTables.ContainsKey(p.Value))
                        _Table = this.webForm.FormDataBackup.MultipleTables[p.Value];
                    else
                        throw new FormException($"Bad Request", ErrCod, string.Format(ErrMsg, p.Key), $"{p.Value} not found in MultipleTables");
                    TableSchema _table = this.webForm.FormSchema.Tables.Find(e => e.TableName == p.Value);
                    if (_table.TableType != WebFormTableTypes.Normal)
                        throw new FormException($"Bad Request", ErrCod, string.Format(ErrMsg, p.Key), $"{p.Value} found in MultipleTables but it is not a normal table");
                    if (_Table.Count != 1)
                        throw new FormException($"Bad Request", ErrCod, string.Format(ErrMsg, p.Key), $"{p.Value} found in MultipleTables but table is empty");
                    SingleColumn Column = _Table[0].Columns.Find(e => e.Control?.Name == p.Key);
                    if (Column == null || Column.Control == null)
                        throw new FormException($"Bad Request", ErrCod, string.Format(ErrMsg, p.Key), $"{p.Value} found in MultipleTables but data not available");

                    ParameterizeCtrl_Params args = new ParameterizeCtrl_Params(this.DataDB, _params, Column, _idx, this.webForm.UserObj);
                    Column.Control.ParameterizeControl(args, this.webForm.CrudContext);
                    _idx = args.i;
                    _params[_idx - 1].ParameterName = p.Key;
                }
                List<int> ids = new List<int>();
                EbFormHelper.AddExtraSqlParams(_params, this.DataDB, this.webForm.TableName, this.webForm.TableRowId, this.webForm.LocationId, this.webForm.UserObj.UserId);
                string qry = nextStage.ApproverEntity == ApproverEntityTypes.Users ? nextStage.ApproverUsers.Code : nextStage.ApproverRoleQuery.Code;
                EbDataTable dt = this.DataDB.DoQuery(qry, _params.ToArray());
                foreach (EbDataRow dr in dt.Rows)
                {
                    int.TryParse(dr[0].ToString(), out int temp);
                    if (!ids.Contains(temp))
                        ids.Add(temp);
                }
                _val = $"'{ids.Join(",")}'";
                if (nextStage.ApproverEntity == ApproverEntityTypes.Users)
                {
                    _col = "user_ids";
                    this.webForm.MyActNotification.UserIds = ids;
                    hasPerm = ids.Any(e => e == this.webForm.UserObj.UserId);
                }
                else
                {
                    _col = "role_ids";
                    this.webForm.MyActNotification.RoleIds = ids;
                    hasPerm = this.webForm.UserObj.RoleIds.Any(e => ids.Contains(e));
                }
            }
            else
                throw new FormException("Unable to process review control", (int)HttpStatusCode.InternalServerError, "Invalid value for ApproverEntity : " + nextStage.ApproverEntity, "From GetMyActionInsertUpdateQuery");
            if (this.webForm.UserObj.Roles.Contains(SystemRoles.SolutionOwner.ToString()) || this.webForm.UserObj.Roles.Contains(SystemRoles.SolutionAdmin.ToString()))
                hasPerm = true;

            return new string[] { _col, _val };
        }

        private string DeleteIfExists(ref int i)
        {
            string insUpQ = string.Empty;
            SingleRow RowBkUp = this.TableBkUp.Find(e => e.RowId <= 0);
            if (RowBkUp != null)
                insUpQ = this.GetMyActionDeleteQry(ref i, Convert.ToInt32(RowBkUp[FormConstants.eb_my_actions_id]));
            if (this.TableBkUp.Count > 0)
            {
                insUpQ += this.GetApprovalUpdateQry(null, true, false);
                insUpQ += this.GetApprovalLinesDeleteQry();
            }
            return insUpQ;
        }

        private string UpdateMyAction(ref int i)
        {
            bool permissionGranted = false;
            if (this.TableBkUp.Count > 0)
            {
                SingleRow Row = this.TableBkUp.Find(e => e.RowId <= 0);
                if (Row != null && Convert.ToString(Row[FormConstants.eb_my_actions_id]) == Convert.ToString(this.Table[0][FormConstants.eb_my_actions_id]))
                    permissionGranted = true;
            }
            if (!permissionGranted)
                throw new FormException("Access denied to execute review", (int)HttpStatusCode.Unauthorized, $"Following entry is not present in FormDataBackup. eb_my_actions_id: {this.Table[0][FormConstants.eb_my_actions_id]} ", "From GetMyActionInsertUpdateQuery");

            string insUpQ = this.GetMyActionUpdateQry(i);

            this.param.Add(this.DataDB.GetNewParameter($"eb_my_actions_id_{i++}", EbDbTypes.Int32, this.Table[0][FormConstants.eb_my_actions_id]));
            Console.WriteLine("Will try to UPDATE eb_my_actions");
            return insUpQ;
        }

        private string GetMyActionInsertQry(string[] _col_val, EbReviewStage nextStage, string masterId, string description)
        {
            return $@"
INSERT INTO eb_my_actions(
    {_col_val[0]}, 
    from_datetime, 
    is_completed, 
    eb_stages_id, 
    form_ref_id, 
    form_data_id, 
    eb_del, 
    description, 
    is_form_data_editable, 
    my_action_type,
    action_type,
    hide)
VALUES (
    {_col_val[1]}, 
    {this.DataDB.EB_CURRENT_TIMESTAMP}, 
    'F', 
    (SELECT id FROM eb_stages WHERE stage_unique_id = '{nextStage.EbSid}' AND form_ref_id = '{this.webForm.RefId}' AND eb_del = 'F'), 
    '{this.webForm.RefId}', 
    {masterId}, 
    'F', 
    '{description}', 
    '{(nextStage.IsFormEditable ? "T" : "F")}', 
    '{MyActionTypes.Approval}',
    {(int)MyActionTypes.Approval},
    '{(nextStage.HideNotification ? "T" : "F")}'); ";
        }

        private string GetMyActionUpdateQry(int i)
        {
            string qry = $@"
UPDATE 
    eb_my_actions 
SET 
    completed_at = {this.DataDB.EB_CURRENT_TIMESTAMP}, 
    completed_by = @eb_createdby, 
    is_completed = 'T',
    eb_approval_lines_id = (SELECT eb_currval('eb_approval_lines_id_seq')) 
WHERE 
    id = @eb_my_actions_id_{i} AND 
    eb_del = 'F'; ";
            return qry;
        }

        private string GetMyActionDeleteQry(ref int i, int myActId)
        {
            string delQ = $@"
UPDATE 
    eb_my_actions 
SET 
    completed_at = {this.DataDB.EB_CURRENT_TIMESTAMP},
    completed_by = @eb_createdby, 
    is_completed = 'F', 
    eb_del = 'T'
WHERE 
    id = @eb_my_actions_id_{i} AND 
    eb_del = 'F'; ";
            this.param.Add(this.DataDB.GetNewParameter($"eb_my_actions_id_{i++}", EbDbTypes.Int32, myActId));
            Console.WriteLine("Will try to DELETE eb_my_actions");
            return delQ;
        }

        private string GetApprovalInsertQry(bool takeCurVal)
        {
            string str;
            if (takeCurVal)
                str = $"(SELECT eb_currval('{this.webForm.TableName}_id_seq'))";
            else
                str = $"@{this.webForm.TableName}_id";

            return $@"
INSERT INTO eb_approval(
    review_status, 
    status,
    eb_my_actions_id, 
    eb_src_id, 
    eb_ver_id, 
    eb_created_by, 
    eb_created_at, 
    eb_del,
    eb_loc_id)
VALUES(
    '{ReviewStatus.In_Process}', 
    {(int)ReviewStatusEnum.In_Process},
    (SELECT eb_currval('eb_my_actions_id_seq')), 
    {str}, 
    @{this.webForm.TableName}_eb_ver_id, 
    @eb_createdby, 
    {this.DataDB.EB_CURRENT_TIMESTAMP}, 
    'F',
    @eb_loc_id); ";
        }

        private string GetApprovalUpdateQry(string _reviewStatus, bool isdel, bool isReset)
        {
            string upStr;
            if (isdel)
                upStr = "eb_del = 'T'";
            else if (_reviewStatus == null && !isReset)
                upStr = "eb_my_actions_id = (SELECT eb_currval('eb_my_actions_id_seq'))";
            else if (isReset)
                upStr = $"eb_my_actions_id = (SELECT eb_currval('eb_my_actions_id_seq')), review_status = '{ReviewStatus.In_Process}', status = {(int)ReviewStatusEnum.In_Process}";
            else
                upStr = $"review_status = '{_reviewStatus}', status = {(int)(ReviewStatusEnum)Enum.Parse(typeof(ReviewStatusEnum), _reviewStatus)}";

            return $@"
UPDATE 
    eb_approval 
SET 
    {upStr}, 
    eb_lastmodified_by = @eb_modified_by, 
    eb_lastmodified_at = {this.DataDB.EB_CURRENT_TIMESTAMP} 
WHERE 
    eb_src_id = @{this.webForm.TableName}_id AND 
    eb_ver_id =  @{this.webForm.TableName}_eb_ver_id AND 
    COALESCE(eb_del, 'F') = 'F'; ";
        }

        private string GetApprovalLinesInsertQry(ref int i)
        {
            string qry = $@"
INSERT INTO eb_approval_lines (
    stage_unique_id,
    action_unique_id,
    eb_my_actions_id,
    comments,
    eb_created_by, 
    eb_created_at, 
    eb_loc_id, 
    eb_src_id, 
    eb_ver_id, 
    eb_signin_log_id,
    eb_approval_id
) 
VALUES (
    @stage_unique_id_{i},
    @action_unique_id_{i},
    @eb_my_actions_id_{i},
    @comments_{i},
    @eb_createdby, 
    {DataDB.EB_CURRENT_TIMESTAMP}, 
    @eb_loc_id, 
    @{this.webForm.TableName}_id, 
    @{this.webForm.TableName}_eb_ver_id, 
    @eb_signin_log_id, 
    (SELECT id FROM eb_approval WHERE eb_src_id = @{this.webForm.TableName}_id AND eb_ver_id =  @{this.webForm.TableName}_eb_ver_id AND COALESCE(eb_del, 'F') = 'F' LIMIT 1));

UPDATE 
    eb_approval 
SET 
    eb_approval_lines_id = (SELECT eb_currval('eb_approval_lines_id_seq')), 
    eb_lastmodified_by = @eb_modified_by, 
    eb_lastmodified_at = {DataDB.EB_CURRENT_TIMESTAMP} 
WHERE 
    eb_src_id = @{this.webForm.TableName}_id AND 
    eb_ver_id =  @{this.webForm.TableName}_eb_ver_id AND 
    COALESCE(eb_del, 'F') = 'F';";

            this.param.Add(this.DataDB.GetNewParameter($"stage_unique_id_{i}", EbDbTypes.String, this.Table[0][FormConstants.stage_unique_id]));
            this.param.Add(this.DataDB.GetNewParameter($"action_unique_id_{i}", EbDbTypes.String, this.Table[0][FormConstants.action_unique_id]));
            this.param.Add(this.DataDB.GetNewParameter($"eb_my_actions_id_{i}", EbDbTypes.Int32, this.Table[0][FormConstants.eb_my_actions_id]));
            this.param.Add(this.DataDB.GetNewParameter($"comments_{i++}", EbDbTypes.String, this.Table[0][FormConstants.comments]));
            return qry;
        }

        private string GetApprovalLinesDeleteQry()
        {
            return $@"
UPDATE 
    eb_approval_lines
SET
    eb_lastmodified_by = @eb_modified_by, 
    eb_lastmodified_at = {this.DataDB.EB_CURRENT_TIMESTAMP}, 
    eb_del = 'T'
WHERE
    eb_src_id = @{this.webForm.TableName}_id AND 
    eb_ver_id =  @{this.webForm.TableName}_eb_ver_id AND 
    COALESCE(eb_del, 'F') = 'F'; ";
        }

    }
}
