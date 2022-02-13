using ExpressBase.Common;
using ExpressBase.Common.Structures;
using ExpressBase.Objects.ServiceStack_Artifacts;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Net;
using System.Text;

namespace ExpressBase.Objects.WebFormRelated
{
    public enum FormDraftTypes
    {
        NormalDraft,
        ErrorBin
    }

    public class FormDraftsHelper
    {
        public static InsertDataFromWebformResponse SubmitErrorAndGetResponse(IDatabase DataDB, EbWebForm Form, InsertDataFromWebformRequest request, Exception ex)
        {
            try
            {
                Console.WriteLine("SaveErrorSubmission start");
                Dictionary<string, string> MetaData = new Dictionary<string, string>();

                string Qry = $@"
INSERT INTO eb_form_drafts 
(
    title, 
    form_data_json, 
    form_ref_id, 
    message, 
    stack_trace, 
    is_submitted, 
    eb_loc_id, 
    eb_created_by, 
    eb_created_at, 
    eb_del,
    draft_type,
    eb_signin_log_id
)
VALUES 
(
    @title, 
    @form_data_json, 
    @form_ref_id, 
    @error_message, 
    @error_stacktrace, 
    'F', 
    {request.CurrentLoc}, 
    {request.UserId}, 
    {DataDB.EB_CURRENT_TIMESTAMP}, 
    'F',
    {(int)FormDraftTypes.ErrorBin},
    {Form.UserObj.SignInLogId}
); 
SELECT eb_currval('eb_form_drafts_id_seq');";

                string message, stackTrace;
                if (ex is FormException formEx)
                {
                    message = formEx.Message + "; " + formEx.MessageInternal;
                    stackTrace = formEx.StackTrace + "; " + formEx.StackTraceInternal;
                }
                else
                {
                    message = ex.Message;
                    stackTrace = ex.StackTrace;
                }

                DbParameter[] parameters = new DbParameter[]
                {
                    DataDB.GetNewParameter("title", EbDbTypes.String, Form.DisplayName),
                    DataDB.GetNewParameter("form_data_json", EbDbTypes.String, request.FormData),
                    DataDB.GetNewParameter("form_ref_id", EbDbTypes.String, Form.RefId),
                    DataDB.GetNewParameter("error_message", EbDbTypes.String, message),
                    DataDB.GetNewParameter("error_stacktrace", EbDbTypes.String, stackTrace)
                };

                EbDataSet ds = DataDB.DoQueries(Qry, parameters);
                int _id = Convert.ToInt32(ds.Tables[0].Rows[0][0]);
                Console.WriteLine("SaveErrorSubmission returning");

                return new InsertDataFromWebformResponse()
                {
                    Message = "Error submission saved",
                    RowId = _id,
                    RowAffected = 1000,
                    AffectedEntries = "Error submission id: " + _id,
                    Status = (int)HttpStatusCode.OK,
                    MetaData = MetaData
                };
            }
            catch (Exception _ex)
            {
                Console.WriteLine("Exception in SubmitErrorAndGetResponse\nMessage" + ex.Message + "\nStackTrace" + ex.StackTrace);

                if (ex is FormException formEx)
                {
                    return new InsertDataFromWebformResponse()
                    {
                        Message = formEx.Message,
                        Status = formEx.ExceptionCode,
                        MessageInt = formEx.MessageInternal + ": " + _ex.Message,
                        StackTraceInt = formEx.StackTraceInternal
                    };
                }
                else
                {
                    return new InsertDataFromWebformResponse()
                    {
                        Message = FormErrors.E0132 + ex.Message,
                        Status = (int)HttpStatusCode.InternalServerError,
                        MessageInt = "Exception in SubmitErrorAndGetResponse[service]: " + _ex.Message,
                        StackTraceInt = ex.StackTrace
                    };
                }
            }
        }
    }
}
