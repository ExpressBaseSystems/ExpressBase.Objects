﻿using ExpressBase.Common;
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
                    object status = this.webForm.ExecuteCSharpScriptNew(this.ebReview.EntryCriteria.Code, this.globals);
                    bool.TryParse(Convert.ToString(status), out entryCriteriaRslt);
                }
                if (entryCriteriaRslt)
                    nextStage = this.ebReview.FormStages[0];
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
                string[] _col_val = this.GetApproverEntityValues(ref i, nextStage);
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
            bool insMyActRequired = false, insInEdit = false, entryCriteriaRslt = true;
            EbReviewStage nextStage = null;
            if (!string.IsNullOrWhiteSpace(this.ebReview.EntryCriteria?.Code))
            {
                this.globals = GlobalsGenerator.GetCSharpFormGlobals_NEW(this.webForm, this.webForm.FormData, this.webForm.FormDataBackup, this.DataDB, null, false);
                object status = this.webForm.ExecuteCSharpScriptNew(this.ebReview.EntryCriteria.Code, this.globals);
                bool.TryParse(Convert.ToString(status), out entryCriteriaRslt);
            }
            if (this.isInsert)
            {
                if (entryCriteriaRslt)
                {
                    masterId = $"(SELECT eb_currval('{this.webForm.TableName}_id_seq'))";
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
                string[] _col_val = this.GetApproverEntityValues(ref i, nextStage);
                string autoId = this.GetAutoId(masterId);
                string description = this.GetDescription(nextStage, autoId);
                insUpQ += this.InsertMyActionAndApproval(nextStage, _col_val, masterId, description, insInEdit);
            }

            return insUpQ;
        }

        private EbReviewStage ExecuteOneStage(ref string insUpQ, ref int i, ref bool insMyActRequired, bool ApprovalFlow)
        {
            EbReviewStage nextStage = null;

            insUpQ += this.UpdateMyAction(ref i);

            if (!(this.ebReview.FormStages.Find(e => e.EbSid == Convert.ToString(this.Table[0]["stage_unique_id"])) is EbReviewStage currentStage))
                throw new FormException("Bad Request", (int)HttpStatusCode.BadRequest, $"eb_approval_lines contains an invalid stage_unique_id: {this.Table[0]["stage_unique_id"]} ", "From GetMyActionInsertUpdateQuery");

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
                    if (this.webForm.AutoId != null && this.isInsert && description.Contains(FormConstants.AutoId_PlaceHolder))
                        description = description.Replace(FormConstants.AutoId_PlaceHolder, autoId);
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

        private string[] GetApproverEntityValues(ref int i, EbReviewStage nextStage)
        {
            string _col = string.Empty, _val = string.Empty;
            this.webForm.MyActNotification = new MyActionNotification() { ApproverEntity = nextStage.ApproverEntity };
            if (nextStage.ApproverEntity == ApproverEntityTypes.Role)
            {
                _col = "role_ids";
                _val = $"@role_ids_{i}";
                string roles = nextStage.ApproverRoles == null ? string.Empty : nextStage.ApproverRoles.Join(",");
                this.param.Add(this.DataDB.GetNewParameter($"@role_ids_{i++}", EbDbTypes.String, roles));
                this.webForm.MyActNotification.RoleIds = nextStage.ApproverRoles;
            }
            else if (nextStage.ApproverEntity == ApproverEntityTypes.UserGroup)
            {
                _col = "usergroup_id";
                _val = $"@usergroup_id_{i}";
                this.param.Add(this.DataDB.GetNewParameter($"@usergroup_id_{i++}", EbDbTypes.Int32, nextStage.ApproverUserGroup));
                this.webForm.MyActNotification.UserGroupId = nextStage.ApproverUserGroup;
            }
            else if (nextStage.ApproverEntity == ApproverEntityTypes.Users)
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
                List<int> uids = new List<int>();
                EbFormHelper.AddExtraSqlParams(_params, this.DataDB, this.webForm.TableName, this.webForm.TableRowId, this.webForm.LocationId, this.webForm.UserObj.UserId);
                EbDataTable dt = this.DataDB.DoQuery(nextStage.ApproverUsers.Code, _params.ToArray());
                foreach (EbDataRow dr in dt.Rows)
                {
                    int.TryParse(dr[0].ToString(), out int temp);
                    if (!uids.Contains(temp))
                        uids.Add(temp);
                }
                _col = "user_ids";
                _val = $"'{uids.Join(",")}'";
                this.webForm.MyActNotification.UserIds = uids;
            }
            else
                throw new FormException("Unable to process review control", (int)HttpStatusCode.InternalServerError, "Invalid value for ApproverEntity : " + nextStage.ApproverEntity, "From GetMyActionInsertUpdateQuery");

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

            this.param.Add(this.DataDB.GetNewParameter($"@eb_my_actions_id_{i++}", EbDbTypes.Int32, this.Table[0][FormConstants.eb_my_actions_id]));
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
            this.param.Add(this.DataDB.GetNewParameter($"@eb_my_actions_id_{i++}", EbDbTypes.Int32, myActId));
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
    eb_my_actions_id, 
    eb_src_id, 
    eb_ver_id, 
    eb_created_by, 
    eb_created_at, 
    eb_del)
VALUES(
    '{ReviewStatus.In_Process}', 
    (SELECT eb_currval('eb_my_actions_id_seq')), 
    {str}, 
    @{this.webForm.TableName}_eb_ver_id, 
    @eb_createdby, 
    {this.DataDB.EB_CURRENT_TIMESTAMP}, 
    'F'); ";
        }

        private string GetApprovalUpdateQry(string _reviewStatus, bool isdel, bool isReset)
        {
            string upStr;
            if (isdel)
                upStr = "eb_del = 'T'";
            else if (_reviewStatus == null)
                upStr = "eb_my_actions_id = (SELECT eb_currval('eb_my_actions_id_seq'))";
            else if (isReset)
                upStr = $"eb_my_actions_id = (SELECT eb_currval('eb_my_actions_id_seq')), review_status = '{ReviewStatus.In_Process}'";
            else
                upStr = $"review_status = '{_reviewStatus}'";

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