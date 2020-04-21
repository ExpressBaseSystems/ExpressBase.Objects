using ExpressBase.Common.Constants;
using ExpressBase.Common.EbServiceStack.ReqNRes;
using ExpressBase.Common.Extensions;
using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using Newtonsoft.Json;
using ServiceStack;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace ExpressBase.Objects.Objects
{

    [EnableInBuilder(BuilderType.WebForm)]
    public class EbMeetingPicker : EbControlUI
    {
        public EbMeetingPicker() { }
        public override string ToolIconHtml { get { return "<i class='fa fa-i-cursor'></i>"; } set { } }

        [EnableInBuilder(BuilderType.WebForm)]
        [DefaultPropValue("1")]
        public int MeetingId { get; set; }

        public override string UIchangeFns
        {
            get
            {
                return @"EbMeetingPicker = {
                }";
            }
        }

        public override string GetBareHtml()
        {
            string EbCtrlHTML = @" <div id='cont_@ebsid@' ebsid='@ebsid@' name='@name@' class='Eb-ctrlContainer meeting-picker-outer' @childOf@ ctype='@type@'>
                        <div class='head-cont' > @Label@ </div>
                        <div class='date-cont' > <input type='text' id='datepicker'class='form-control'/> </div>
                        <div class='picker-cont'> </div>
                </div>"
                .Replace("@name@", this.Name)
                .Replace("@Label@", this.Label)
                .Replace("@ebsid@", this.EbSid);
            return EbCtrlHTML;
        }

        public override string GetHtml()
        {
            string EbCtrlHTML = @" <div id='cont_@ebsid@' ebsid='@ebsid@' name='@name@' class='Eb-ctrlContainer meeting-picker-outer' @childOf@ ctype='@type@'>
                        <div class='head-cont' > @Label@ </div>
                        <div class='date-cont'> <input type='text' id='@ebsid@_datepicker' class='form-control'/> </div>
                        <div class='picker-cont' > </div>
                </div>"
               .Replace("@name@", this.Name)
               .Replace("@Label@", this.Label);
            return ReplacePropsInHTML(EbCtrlHTML);
        }

        public override string GetDesignHtml()
        {
            return GetDesignHtmlHelper().RemoveCR().DoubleQuoted();
        }

        public string GetDesignHtmlHelper()
        {
            string EbCtrlHTML = @" <div id='cont_@ebsid@' ebsid='@ebsid@' name='@name@' class='Eb-ctrlContainer meeting-picker-outer'   @childOf@ ctype='@type@'>
                        <div class='head-cont' > @Label@ </div>
                        <div class='date-cont'> <input type='text' id='datepicker' class='form-control'/> </div>
                        <div class='picker-cont' > </div>
                </div>"
               .Replace("@name@", this.Name)
               .Replace("@Label@", this.Label);
            return ReplacePropsInHTML(EbCtrlHTML);
        }
    }

    public class MeetingSlots
    {
        public int Meeting_Id { get; set; }
        public int Slot_id { get; set; }
        public int Meeting_schedule_id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Is_approved { get; set; }
        public string Date { get; set; }
        public string Time_from { get; set; }
        public string Time_to { get; set; }
        public int UserType { get; set; }
        public bool IsHide { get; set; }

    }

    public class SlotProcess : MeetingSlots
    {
        public string Venue { get; set; }
        public string Integration { get; set; }
        public int Max_Host { get; set; }
        public int Max_Attendee { get; set; }
        public int No_Host { get; set; }
        public int No_Attendee { get; set; }
        public int MeetingHostCount { get; set; }
        public int MeetingAttendeeCount { get; set; }
        public int SlotHostCount { get; set; }
        public int SlotAttendeeCount { get; set; }
        public int MeetingId { get; set; }

    }
    public class GetMeetingSlotsRequest : EbServiceStackAuthRequest, IReturn<GetMeetingSlotsResponse>
    {
        public int MeetingScheduleId { get; set; }

        public string Date { get; set; }
    }

    [DataContract]
    public class GetMeetingSlotsResponse
    {
        [DataMember(Order = 0)]
        public List<SlotProcess> AllSlots { set; get; }

        public GetMeetingSlotsResponse()
        {
            this.AllSlots = new List<SlotProcess>();
        }
    }

    public class SlotParticipants
    {
        public int UserId { get; set; }
        public int RoleId { get; set; }
        public int UserGroupId { get; set; }
        public int Confirmation { get; set; }
        public int ApprovedSlotId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PhoneNum { get; set; }
        public int TypeOfUser { get; set; }
        public int Participant_type { get; set; }
    }

    [DataContract]
    public class UpdateSlotParticipantRequest : EbServiceStackAuthRequest, IReturn<GetMeetingSlotsResponse>
    {
        [DataMember(Order = 0)]
        public SlotParticipants SlotParticipant { get; set; }
        [DataMember(Order = 1)]
        public DetailsBySlotid SlotInfo { get; set; }
    }

    [DataContract]
    public class UpdateSlotParticipantResponse
    {
        [DataMember(Order = 0)]
        public bool ResponseStatus { get; set; }
    }

    [DataContract]
    public class MeetingSaveValidateRequest : EbServiceStackAuthRequest, IReturn<GetMeetingSlotsResponse>
    {
        [DataMember(Order = 0)]
        public SlotParticipants SlotParticipant { get; set; }
    }

    public class DetailsBySlotid
    {
        public int Slot_id { get; set; }
        public int Meeting_schedule_id { get; set; }
        public string Is_approved { get; set; }
        public int Max_Host { get; set; }
        public int Max_Attendee { get; set; }
        public int No_Host { get; set; }
        public int No_Attendee { get; set; }
        public int SlotHostCount { get; set; }
        public int SlotAttendeeCount { get; set; }
        public int MeetingId { get; set; }
        public int Participant_id { get; set; }
    }

    [DataContract]
    public class MeetingSaveValidateResponse
    {
        [DataMember(Order = 0)]
        public bool ResponseStatus { get; set; }

        [DataMember(Order = 1)]
        public int UserId { get; set; }
    }
}
