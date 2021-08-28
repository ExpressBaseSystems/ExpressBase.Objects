using System;
using System.Collections.Generic;
using System.Text;

namespace ExpressBase.Objects.WebFormRelated
{
    public static class FormConstants
    {
        //System table columns
        public const string id = "id";
        public const string eb_ver_id = "eb_ver_id";
        public const string eb_lock = "eb_lock";
        public const string eb_push_id = "eb_push_id";
        public const string eb_src_id = "eb_src_id";
        public const string eb_row_num = "eb_row_num";
        public const string eb_created_by = "eb_created_by";
        public const string eb_createdby = "eb_createdby";
        public const string eb_created_at = "eb_created_at";
        public const string eb_lastmodified_by = "eb_lastmodified_by";
        public const string eb_modified_by = "eb_modified_by";
        public const string eb_lastmodified_at = "eb_lastmodified_at";
        public const string eb_del = "eb_del";
        public const string eb_void = "eb_void";
        public const string eb_loc_id = "eb_loc_id";
        public const string eb_currentuser_id = "eb_currentuser_id";
        public const string eb_signin_log_id = "eb_signin_log_id";
        //_vals
        public const string _id = "_id";
        public const string _ebbkup = "_ebbkup";
        public const string _eb_ver_id = "_eb_ver_id";
        public const string _refid = "_refid";

        //Date
        public const string yyyy = "yyyy";
        public const string MMyyyy = "MM/yyyy";
        public const string yyyyMMdd = "yyyy-MM-dd";
        public const string HHmmss = "HH:mm:ss";
        public const string yyyyMMdd_HHmmss = "yyyy-MM-dd HH:mm:ss";
        //FileUploader
        public const string filename = "filename";
        public const string tags = "tags";
        public const string uploadts = "uploadts";
        public const string filecategory = "filecategory";
        public const string Files = "Files";
        public const string context = "context";
        public const string context_sec = "context_sec";
        //Approval
        public const string stage_unique_id = "stage_unique_id";
        public const string action_unique_id = "action_unique_id";
        public const string eb_my_actions_id = "eb_my_actions_id";
        public const string comments = "comments";
        public const string is_form_data_editable = "is_form_data_editable";
        public const string has_permission = "has_permission";
        public const string __system_stage = "__system_stage";
        public const string __review_reset = "__review_reset";
        //Prov Location
        public const string longname = "longname";
        public const string shortname = "shortname";
        public const string image = "image";
        public const string meta_json = "meta_json";
        public const string is_group = "is_group";
        public const string parent_id = "parent_id";
        public const string eb_location_types_id = "eb_location_types_id";
        //Prov User
        public const string email = "email";
        public const string phprimary = "phprimary";
        public const string fullname = "fullname";
        public const string pwd = "pwd";
        public const string usertype = "usertype";
        public const string statusid = "statusid";
        public const string roles = "roles";
        public const string consadd = "consadd";
        //Globals - DataPusher
        public const string __eb_loop_through = "__eb_loop_through";
        //EbPhone ctrl
        public const string _verified = "_verified";
        public const string is_verified = "is_verified";
        public const string phone_no = "phone_no";
        public const string otp = "otp";
        public const string timestamp = "timestamp";
        //PowerSelect
        public const string __ebedt = "__ebedt";//ebEdited
    }
}
