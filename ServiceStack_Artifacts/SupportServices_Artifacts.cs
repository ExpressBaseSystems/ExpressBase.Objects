using ExpressBase.Common.EbServiceStack.ReqNRes;
using Microsoft.AspNetCore.Http;
using ServiceStack;
using ServiceStack.Text;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using ExpressBase.Security;
using System.Runtime.Serialization;

namespace ExpressBase.Objects.ServiceStack_Artifacts
{
    //class SupportServices_Artifacts
    //{
    //}

    public class SaveBugRequest : EbServiceStackAuthRequest, IReturn<SaveBugResponse>
    {
        public string title { get; set; }

        public string description { get; set; }

        public string priority { get; set; }

        public string solutionid { get; set; }

        public string usertype { get; set; }

        public string status { get; set; }

        public string type_b_f { get; set; }

        public string fullname { get; set; }

        public string email { get; set; }

        public List<FileUploadCls> Fileuploadlst { get; set; } = new List<FileUploadCls>();
    }

    public class SaveBugResponse
    {
        public int Id { get; set; }
        public string status { get; set; }
        public string ErMsg { get; set; }




    }

    public class SubmitTicketRequest : EbServiceStackAuthRequest, IReturn<SubmitTicketResponse>
    {
        public string title { get; set; }

        public string description { get; set; }

        public string priority { get; set; }

        public string solutionid { get; set; }

        public string usertype { get; set; }

        public string status { get; set; }

        public string type_b_f { get; set; }

        public string fullname { get; set; }

        public string email { get; set; }
        public int onBehalfOf { get; set; } 

        public List<FileUploadCls> Fileuploadlst { get; set; } = new List<FileUploadCls>();
    }

    public class SubmitTicketResponse
    {
        public int Id { get; set; }
        public string status { get; set; }
        public string ErMsg { get; set; }
        public string SuccessMessage { get; set; }
    }

    // Request model for fetching ticket details
    public class GetSubmittedTicketsRequest : EbServiceStackAuthRequest, IReturn<TicketDetailsResponse>
    {
        public int UserId { get; set; }  // User ID to filter the tickets
    }

    // Response model for fetching ticket details
    public class TicketDetailsResponse
    {
        public List<SupportTktCls> Tickets { get; set; } = new List<SupportTktCls>(); // List of tickets
        public string ErrorMessage { get; set; }  // Error message if any issue occurs
    }

    public class FetchAdminsRequest : EbServiceStackAuthRequest, IReturn<FetchAdminsResponse>
    {


    }

    public class FetchAdminsResponse
    {
        public List<string> AdminNames { get; set; } = new List<string>();

        public string ErMsg { get; set; }
    }



    public class TenantSolutionsRequest : EbServiceStackAuthRequest, IReturn<TenantSolutionsResponse>
    {
        public string usertype { get; set; }

    }


    public class TenantSolutionsResponse
    {
        public TenantSolutionsResponse()
        {
            sol_id = new List<string>();
            solname = new List<string>();
            sol_exid = new List<string>();
        }

        public List<string> sol_id { get; set; }

        public List<string> solname { get; set; }

        public List<string> sol_exid { get; set; }

        public string ErMsg { get; set; }
    }

    public class FetchSupportRequest : EbServiceStackAuthRequest, IReturn<FetchSupportResponse>
    {
        public int usrid { get; set; }


    }
    public class FetchSupportResponse
    {
        public List<TicketLite> ActiveTicket { get; set; }
        public List<TicketLite> ClosedTicket { get; set; }
        public string ErrorMessage { get; set; }

        public FetchSupportResponse()
        {
            ActiveTicket = new List<TicketLite>();
            ClosedTicket = new List<TicketLite>();
        }
    }

    public class GetTicketByIdRequest : EbServiceStackAuthRequest, IReturn<GetTicketByIdResponse>
    {
        public string TicketId { get; set; }
    }


    public class GetTicketByIdResponse
    {
        public SupportTktCls Ticket { get; set; }
    }



    public class AdminSupportRequest : EbServiceStackAuthRequest, IReturn<AdminSupportResponse>
    {
        public int usrid { get; set; }
    }

    public class AdminSupportResponse
    {
        public AdminSupportResponse()
        {
            supporttkt = new List<SupportTktCls>();
        }
        public List<SupportTktCls> supporttkt { get; set; }

        public string ErMsg { get; set; }
    }





    public class SupportDetailsRequest : EbServiceStackAuthRequest, IReturn<SupportDetailsResponse>
    {
        public string ticketno { get; set; }
        public string Usertype { get; set; }


    }

    public class SupportDetailsResponse
    {
        public SupportDetailsResponse()
        {
            supporttkt = new List<SupportTktCls>();
        }
        public List<SupportTktCls> supporttkt { get; set; }

        public List<string> Filecollection1 { set; get; } = new List<string>();

        public string ErMsg { get; set; }

        public bool SdrStatus { get; set; } = false;



    }


    public class ChangeStatusRequest : EbServiceStackAuthRequest, IReturn<ChangeStatusResponse>
    {
        public string TicketNo { get; set; }
        public string Reason { get; set; }

        public string NewStatus { get; set; }
        public string UserName { get; set; }
        public string Solution_id { get; set; }

    }

    public class ChangeStatusResponse
    {
        public bool RtnStatus { get; set; } = false;

        public string ErMsg { get; set; }
    }

    public class CommentRequest : EbServiceStackAuthRequest, IReturn<CommentResponse>
    {
        public string TicketNo { get; set; }

        public string Comments { get; set; }
        public string UserName { get; set; }
        public string Solution_id { get; set; }
        public string currentUserid { get; set; }


    }

    public class CommentResponse
    {
        public bool CmntStatus { get; set; } = false;

        public string ErMsg { get; set; }
    }

    public class CommentListRequest : EbServiceStackAuthRequest, IReturn<CommentListResponse>
    {
        public string TicketNo { get; set; }
    }


    [DataContract]
    public class CommentListResponse
    {
        [DataMember]
        public List<CommentDto> Comments { get; set; } = new List<CommentDto>();

        [DataMember]
        public bool Success { get; set; }

        [DataMember]
        public string ErrorMessage { get; set; }
    }

    public class CommentDto
    {
        public string UserName { get; set; }
        public string CommentText { get; set; }
        public DateTime CreatedAt { get; set; }
        public string UserId { get; set; } // ✅ New

    }




    public class UpdateTicketRequest : EbServiceStackAuthRequest, IReturn<UpdateTicketResponse>
    {
        public string title { get; set; }

        public string description { get; set; }

        public string priority { get; set; }

        public string ticketid { get; set; }

        public string solution_id { get; set; }

        public string usrname { get; set; }

        public string type_f_b { get; set; }

        public string[] Filedel { get; set; } = new string[0]; // Ensures it's never null

        public List<FileUploadCls> Fileuploadlst { get; set; } = new List<FileUploadCls>();

        public Dictionary<string, string> chngedtkt { set; get; } = new Dictionary<string, string>();

        public Dictionary<SupportTicketFields, string> History_fv { set; get; } = new Dictionary<SupportTicketFields, string>();
    }

    public class UpdateTicketResponse
    {
        public bool status { get; set; }

        public string ErMsg { get; set; }
    }


    public class UpdateTicketAdminRequest : EbServiceStackAuthRequest, IReturn<UpdateTicketResponse>
    {
        public string Remarks { get; set; }

        public string AssignTo { get; set; }

        public string Status { get; set; }

        public string Ticketid { get; set; }

        public string Solution_id { get; set; }

        public string Type_f_b { get; set; }

        public string usrname { get; set; }

        public Dictionary<string, string> chngedtkt { set; get; } = new Dictionary<string, string>();

        public Dictionary<SupportTicketFields, string> History_fv { set; get; } = new Dictionary<SupportTicketFields, string>();

    }
    public class UpdateTicketAdminResponse
    {
        public bool status { get; set; }

        public string ErMsg { get; set; }
    }

    public class SupportHistoryRequest : EbServiceStackAuthRequest, IReturn<SupportHistoryResponse>
    {

        public string TicketNo { get; set; }
        public string UserType { get; set; }

        public User UserObject { get; set; }
    }

    public class SupportHistoryResponse
    {

        public SupportHistoryResponse()
        {
            SpHistory = new List<SupportHistory>();
        }
        public List<SupportHistory> SpHistory { get; set; }

        public string ErMsg { get; set; }

    }

    public class TicketLite
    {
        public string ticketid { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public string status { get; set; }
        public string fullname { get; set; }
        public string lstmodified { get; set; }
    }


    public class SupportTktCls
    {
        public string ticketid { get; set; }

        public string title { get; set; }

        public string description { get; set; }

        public string priority { get; set; }

        public string solutionid { get; set; }

        public string status { get; set; }

        public string assignedto { get; set; }

        public string type_b_f { get; set; }

        public string lstmodified { get; set; }

        public string createdat { get; set; }

        public string remarks { get; set; }

        public string NoDays { get; set; }

        public string Solution_name { get; set; }

        public string fullname { get; set; }
        public string onbehalf { get; set; }
        public string eta { get; set; }              // New field
        public string estimated_hours { get; set; }   // New field
        public string actual_hours { get; set; }      // New field

        public string Esolution_id { get; set; }

        public string NoHour { get; set; }

        public List<FileUploadCls> Fileuploadlst { get; set; } = new List<FileUploadCls>();

        public string ErMsg { get; set; }
    }

    public class FileUploadCls
    {
        public string FileName { get; set; }

        public int FileId { get; set; }

        public string ContentType { get; set; }

        public byte[] Filecollection { set; get; }

        public string FileDataURL { set; get; }

        public string ErMsg { get; set; }
    }


    public enum SupportTicketFields
    {
        title = 1,
        solution_id = 2,
        date_created = 3,
        type_bg_fr = 4,
        priority = 5,
        status = 6,
        description = 7,
        comment = 8,
        assigned_to = 9,
        files = 10,
        reason = 11
    }

    public class SupportHistory
    {
        public int Id { get; set; }

        public string UserName { get; set; }

        public string TicketId { get; set; }

        public string Field { get; set; }

        public int FieldId { set; get; }

        public string Value { set; get; }

        public string CreatedDate { get; set; }

        public string CreatedTime { get; set; }

        public string SolutionId { get; set; }
    }

}
