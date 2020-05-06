using ExpressBase.Common;
using ExpressBase.Common.EbServiceStack.ReqNRes;
using ExpressBase.Common.Extensions;
using ExpressBase.Common.LocationNSolution;
using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using ExpressBase.Common.Structures;
using ExpressBase.Objects.ServiceStack_Artifacts;
using ExpressBase.Security;
using ServiceStack;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Runtime.Serialization;

namespace ExpressBase.Objects
{

    [EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm)]
    public class EbMeetingPicker : EbControlUI
    {
        public EbMeetingPicker() { }
        public override string ToolIconHtml { get { return "<i class='fa fa-i-cursor'></i>"; } set { } }

        public string TableName { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm)]
        [HideInPropertyGrid]
        public override EbDbTypes EbDbType { get { return EbDbTypes.String; } }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm)]
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

        [OnDeserialized]
        public void OnDeserializedMethod(StreamingContext context)
        {
            this.BareControlHtml = this.GetBareHtml();
            this.BareControlHtml4Bot = this.GetWrapedCtrlHtml4bot();
            this.ObjType = this.GetType().Name.Substring(2, this.GetType().Name.Length - 2);
        }

        public override string DesignHtml4Bot
        {
            get => @"<div id='@ebsid@' ebsid='@ebsid@' name='@name@' class='Eb-ctrlContainer meeting-picker-outer' @childOf@ ctype='@type@'>
                        <div class='head-cont' >   <i class='fa fa-calendar' aria-hidden='true'></i>
                        <div>@Label@ </div> </div>
                        <div class='sub-head'>Pick a date</div>
                        <div class='date-cont'> 
                        <div class='input-group'>
                                <input type='text' class='form-control' id='@ebsid@_date' value='@date_val@' readonly/>
                                <div class='input-group-btn'>
                                  <button class='btn btn-primary' type='submit' id='@ebsid@_date_change'>
                                    <i class='fa fa-calendar'></i>
                                  </button>
                                </div>
                        </div>
                        <div id='@ebsid@_datepicker' hidden></div></div>
                        <div class='sub-head'>Pick a timeslot</div>
                        <input type='text' ids='@ebsid@_slot_val' hidden/>
                          <div class='input-group'>
                                <input type='text' class='form-control' id='@ebsid@_slot_val' readonly/>
                                <div class='input-group-btn'>
                                  <button class='btn btn-primary' type='submit' id='@ebsid@_slot_change'>
                                    <i class='fa fa-clock-o'></i>
                                  </button>
                                </div>
                        </div>
                        <div class='picker-cont' id='@ebsid@_picker-cont'> </div>
                </div>"
                .Replace("@name@", this.Name)
                .Replace("@Label@", this.Label)
                .Replace("@ebsid@", this.EbSid_CtxId);
            set => base.DesignHtml4Bot = value;
        }

        public override string GetHtml4Bot()
        {
            return GetWrapedCtrlHtml4bot();
        }
        public override string GetWrapedCtrlHtml4bot()
        {
            string jk = @"<div id='@ebsid@' ebsid='@ebsid@' name='@name@' class='Eb-ctrlContainer meeting-picker-outer eb-meeting-bot' @childOf@ ctype='@type@'>
                        <div class='head-cont' >   <i class='fa fa-calendar' aria-hidden='true'></i>
                        <div>@Label@ </div> </div>
                        <div class='sub-head'>Pick a date</div>
                        <div class='date-cont'> 
                        <div class='input-group'>
                                <input type='text' class='form-control' id='@ebsid@_date' value='@date_val@' readonly/>
                                <div class='input-group-btn'>
                                  <button class='btn btn-primary' type='submit' id='@ebsid@_date_change'>
                                    <i class='fa fa-calendar'></i>
                                  </button>
                                </div>
                        </div>
                        <div id='@ebsid@_datepicker' hidden></div></div>
                        <div class='sub-head'>Pick a timeslot</div>
                        <input type='text' id='@ebsid@_slot_val' hidden readonly/>
                          <div class='input-group'>
                                <input type='text' class='form-control' id='@ebsid@_slot_time' readonly/>
                                <div class='input-group-btn'>
                                  <button class='btn btn-primary' type='submit' id='@ebsid@_slot_change'>
                                    <i class='fa fa-clock-o'></i>
                                  </button>
                                </div>
                        </div>
                        <div class='picker-cont' id='@ebsid@_picker-cont'> </div>
                </div>".Replace("@date_val@", DateTime.Today.ToString("yyyy-MM-dd"));
            return ReplacePropsInHTML(jk);
        }

        public override string GetBareHtml()
        {
            string EbCtrlHTML = @"<div id='@ebsid@' ebsid='@ebsid@' name='@name@' class='Eb-ctrlContainer meeting-picker-outer' @childOf@ ctype='@type@'>
                        <div class='head-cont' >   <i class='fa fa-calendar' aria-hidden='true'></i>
                        <div>@Label@ </div> </div>
                        <div class='sub-head'>Pick a date</div>
                        <div class='date-cont'> 
                        <div class='input-group'>
                                <input type='text' class='form-control' id='@ebsid@_date' value='@date_val@' readonly/>
                                <div class='input-group-btn'>
                                  <button class='btn btn-primary' type='submit' id='@ebsid@_date_change'>
                                    <i class='fa fa-calendar'></i>
                                  </button>
                                </div>
                        </div>
                        <div id='@ebsid@_datepicker' hidden></div></div>
                        <div class='sub-head'>Pick a timeslot</div>
                        <input type='text' ids='@ebsid@_slot_val' hidden/>
                          <div class='input-group'>
                                <input type='text' class='form-control' id='@ebsid@_slot_val' readonly/>
                                <div class='input-group-btn'>
                                  <button class='btn btn-primary' type='submit' id='@ebsid@_slot_change'>
                                    <i class='fa fa-clock-o'></i>
                                  </button>
                                </div>
                        </div>
                        <div class='picker-cont' id='@ebsid@_picker-cont'> </div>
                </div>"
                .Replace("@name@", this.Name)
                .Replace("@Label@", this.Label)
                .Replace("@date_val@", DateTime.Today.ToString("yyyy-MM-dd"))
                .Replace("@ebsid@", this.EbSid_CtxId);
            return EbCtrlHTML;
        }


        public override string GetHtml()
        {
            string EbCtrlHTML = @"<div id='@ebsid@' ebsid='@ebsid@' name='@name@' class='Eb-ctrlContainer meeting-picker-outer' @childOf@ ctype='@type@'>
                        <div class='head-cont' >   <i class='fa fa-calendar' aria-hidden='true'></i>
                        <div>@Label@ </div> </div>
                        <div class='sub-head'>Pick a date</div>
                        <div class='date-cont'> 
                        <div class='input-group'>
                                <input type='text' class='form-control' id='@ebsid@_date' value='@date_val@' readonly/>
                                <div class='input-group-btn'>
                                  <button class='btn btn-primary' type='submit' id='@ebsid@_date_change'>
                                    <i class='fa fa-calendar'></i>
                                  </button>
                                </div>
                        </div>
                        <div id='@ebsid@_datepicker' hidden></div></div>
                        <div class='sub-head'>Pick a timeslot</div>
                        <input type='text' ids='@ebsid@_slot_val' hidden/>
                          <div class='input-group'>
                                <input type='text' class='form-control' id='@ebsid@_slot_val' readonly/>
                                <div class='input-group-btn'>
                                  <button class='btn btn-primary' type='submit' id='@ebsid@_slot_change'>
                                    <i class='fa fa-clock-o'></i>
                                  </button>
                                </div>
                        </div>
                        <div class='picker-cont' id='@ebsid@_picker-cont'> </div>
                </div>"
               .Replace("@name@", this.Name)
               .Replace("@date_val@", DateTime.Today.ToString("yyyy-MM-dd"))
               .Replace("@Label@", this.Label);
            return ReplacePropsInHTML(EbCtrlHTML);
        }

        public override string GetDesignHtml()
        {
            return GetDesignHtmlHelper().RemoveCR().DoubleQuoted();
        }

        public string GetDesignHtmlHelper()
        {
            string EbCtrlHTML = @"<div id='@ebsid@' ebsid='@ebsid@' name='@name@' class='Eb-ctrlContainer meeting-picker-outer' @childOf@ ctype='@type@'>
                        <div class='head-cont' >   <i class='fa fa-calendar' aria-hidden='true'></i>
                        <div>@Label@ </div> </div>
                        <div class='sub-head'>Pick a date</div>
                        <div class='date-cont'> 
                        <div class='input-group'>
                                <input type='text' class='form-control' id='@ebsid@_date' value='@date_val@' readonly/>
                                <div class='input-group-btn'>
                                  <button class='btn btn-primary' type='submit' id='@ebsid@_date_change'>
                                    <i class='fa fa-calendar'></i>
                                  </button>
                                </div>
                        </div>
                        <div id='@ebsid@_datepicker' hidden></div></div>
                        <div class='sub-head'>Pick a timeslot</div>
                        <input type='text' ids='@ebsid@_slot_val' hidden/>
                          <div class='input-group'>
                                <input type='text' class='form-control' id='@ebsid@_slot_val' readonly/>
                                <div class='input-group-btn'>
                                  <button class='btn btn-primary' type='submit' id='@ebsid@_slot_change'>
                                    <i class='fa fa-clock-o'></i>
                                  </button>
                                </div>
                        </div>
                        <div class='picker-cont' id='@ebsid@_picker-cont'> </div>
                </div>"
               .Replace("@name@", this.Name)
               .Replace("@date_val@", DateTime.Today.ToString("yyyy-MM-dd"))
               .Replace("@Label@", this.Label);
            return ReplacePropsInHTML(EbCtrlHTML);
        }

        public override string GetValueFromDOMJSfn { get { return @"return $('#' + this.EbSid_CtxId +'_slot_val').val();"; } set { } }

        public override string OnChangeBindJSFn { get { return @" $('#' +  this.EbSid_CtxId +'_slot_val').on('change', p1);"; } set { } }

        public override string SetValueJSfn { get { return @"  $('#' + this.EbSid_CtxId +'_slot_val').val(p1).trigger('change');"; } set { } }

        public override string JustSetValueJSfn { get { return @"$('#' + this.EbSid_CtxId +'_slot_val').val(p1)"; } set { } }

        public override SingleColumn GetSingleColumn(User UserObj, Eb_Solution SoluObj, object Value)
        {
            return new SingleColumn()
            {
                Name = this.Name,
                Type = (int)this.EbDbType,
                Value = Value,
                Control = this,
                ObjType = this.ObjType,
                F = "[]"
            };
        }

        public string GetSelectQuery(IDatabase DataDB, string MasterTable)
        {
            return string.Empty;
        }

        private string SlotDetailsGetQuery
        {
            get
            {
                return @" 
            SELECT 
		     A.id as slot_id , A.eb_meeting_schedule_id, A.is_approved,
			 B.no_of_attendee, B.no_of_hosts,
			 COALESCE (C.slot_host, 0) as slot_host_count,
		     COALESCE (C.slot_host_attendee, 0) as slot_attendee_count,
			 COALESCE (D.id, 0) as meeting_id,
			 COALESCE (E.id, 0) as participant_id
	            FROM
				(SELECT 
						id, eb_meeting_schedule_id , is_approved, 
					meeting_date, time_from, time_to
	                     FROM 
		                     eb_meeting_slots 
	                     WHERE 
		                     eb_del = 'F' and id = {0})A
						LEFT JOIN	 
							 (SELECT id, no_of_attendee, no_of_hosts FROM  eb_meeting_schedule)B
							 ON
 	                     B.id = A.eb_meeting_schedule_id
						LEFT JOIN	
						(SELECT 
		                     eb_meeting_schedule_id,approved_slot_id ,type_of_user, COUNT(approved_slot_id)filter(where type_of_user = 1) as slot_host,
						 		                    COUNT(approved_slot_id)filter(where type_of_user = 2) as slot_host_attendee
	                     FROM 
		                     eb_meeting_slot_participants
	                     GROUP BY
		                     eb_meeting_schedule_id, approved_slot_id, type_of_user, eb_del
	                     Having
		                     eb_del = 'F')C	
                     ON
 	                     C.eb_meeting_schedule_id = B.id and C.approved_slot_id = A.id
                     LEFT JOIN 
                     (SELECT 
		                     id, eb_meeting_slots_id
	                     FROM 
		                     eb_meetings
	                     where
		                     eb_del = 'F') D
		                     ON
 	                     D.eb_meeting_slots_id = A.id
						 LEFT JOIN (
						 SELECT id , eb_meeting_schedule_id , approved_slot_id ,type_of_user, COUNT(approved_slot_id)filter(where type_of_user = 1) as slot_host,
					COUNT(approved_slot_id)filter(where type_of_user = 2) as slot_host_attendee
	                     FROM 
		                     eb_meeting_slot_participants
							  GROUP BY
		                        eb_meeting_schedule_id, approved_slot_id, type_of_user, eb_del , id)E
								  ON
 	                     E.approved_slot_id = A.id;
                          SELECT 
		             A.id as slot_id , A.eb_meeting_schedule_id,
			         B.user_id, B.role_id, B.user_group_id,B.participant_type FROM
				        (SELECT 
						id, eb_meeting_schedule_id FROM  eb_meeting_slots 
	                     WHERE  eb_del = 'F' and id = {0})A
						LEFT JOIN	 
							 (SELECT id, user_id, role_id,user_group_id,participant_type 
							  FROM  eb_meeting_scheduled_participants)B
							 ON B.id = A.eb_meeting_schedule_id
                                        ";
            }
        }

        public override bool ParameterizeControl(IDatabase DataDB, List<DbParameter> param, string tbl, SingleColumn cField, bool ins, ref int i, ref string _col, ref string _val, ref string _extqry, User usr, SingleColumn ocF)
        {
            if (!ins)
                return false;
            int.TryParse(Convert.ToString(cField.Value), out int ApprovedSlotId);
            if (ApprovedSlotId < 1)
                return false;

            if (cField.Value == null)
            {
                var p = DataDB.GetNewParameter(cField.Name + "_" + i, (EbDbTypes)cField.Type);
                p.Value = DBNull.Value;
                param.Add(p);
            }
            else
                param.Add(DataDB.GetNewParameter(cField.Name + "_" + i, (EbDbTypes)cField.Type, cField.Value));

            _col += string.Concat(cField.Name, ", ");
            _val += string.Concat("@", cField.Name, "_", i, ", ");
            i++;

            List<DetailsBySlotid> SlotObj = new List<DetailsBySlotid>();
            List<ScheduledParticipants> ScheduledParticipants = new List<ScheduledParticipants>();

            String _query = string.Format(this.SlotDetailsGetQuery, ApprovedSlotId);
            EbDataSet ds = DataDB.DoQueries(_query);
            if (ds.Tables[0].Rows.Count == 0)
                throw new FormException("Requested meeting slot is invalid", (int)HttpStatusCodes.BAD_REQUEST, "Query returned 0 rows for Meeting slot : " + ApprovedSlotId, "From EbMeetingPicker.ParameterizeControl()");

            for (int k = 0; k < ds.Tables[0].Rows.Count; k++)
            {
                SlotObj.Add(new DetailsBySlotid()
                {
                    Slot_id = Convert.ToInt32(ds.Tables[0].Rows[k]["slot_id"]),
                    Meeting_schedule_id = Convert.ToInt32(ds.Tables[0].Rows[k]["eb_meeting_schedule_id"]),
                    MeetingId = Convert.ToInt32(ds.Tables[0].Rows[k]["eb_meeting_id"]),
                    No_Attendee = Convert.ToInt32(ds.Tables[0].Rows[k]["no_of_attendee"]),
                    No_Host = Convert.ToInt32(ds.Tables[0].Rows[k]["no_of_host"]),
                    SlotHostCount = Convert.ToInt32(ds.Tables[0].Rows[k]["slot_host_count"]),
                    SlotAttendeeCount = Convert.ToInt32(ds.Tables[0].Rows[k]["slot_attendee_count"]),
                    Is_approved = Convert.ToString(ds.Tables[0].Rows[k]["is_approved"]),
                    Participant_id = Convert.ToInt32(ds.Tables[0].Rows[k]["participant_id"]),
                });
            }
            for (int k = 0; k < ds.Tables[1].Rows.Count; k++)
            {
                ScheduledParticipants.Add(new ScheduledParticipants()
                {
                    Id = Convert.ToInt32(ds.Tables[0].Rows[k]["id"]),
                    SlotId = Convert.ToInt32(ds.Tables[0].Rows[k]["slot_id"]),
                    ScheduleId = Convert.ToInt32(ds.Tables[0].Rows[k]["eb_meeting_schedule_id"]),
                    UserId = Convert.ToInt32(ds.Tables[0].Rows[k]["user_id"]),
                    RoleId = Convert.ToInt32(ds.Tables[0].Rows[k]["role_id"]),
                    UserGroupId = Convert.ToInt32(ds.Tables[0].Rows[k]["user_group_id"]),
                    ParticipantType = Convert.ToInt32(ds.Tables[0].Rows[k]["participant_type"]),
                });
            }


            if (SlotObj[0].No_Attendee <= SlotObj[0].SlotAttendeeCount)// assuming user in an attendee
                throw new FormException("Unable to continue. Reached maximum attendee limit.", (int)HttpStatusCodes.BAD_REQUEST, $"Max no of attendee : {SlotObj[0].No_Attendee}, Current attendee count : {SlotObj[0].SlotAttendeeCount}", "From EbMeetingPicker.ParameterizeControl()");

            string query = string.Empty;
            if (ScheduledParticipants.Count == 0)
            {
                if (SlotObj[0].Is_approved == "T")
                {
                    query = $@"insert into eb_meeting_slot_participants(user_id, confirmation, eb_meeting_schedule_id, approved_slot_id, name, email, type_of_user, participant_type) 
                            values ({usr.UserId}, 1, {SlotObj[0].Meeting_schedule_id}, {ApprovedSlotId}, '{usr.FullName}', '{usr.Email}', 1, 2);
                        insert into eb_meeting_participants(eb_meeting_id, eb_slot_participant_id) 
                            values({SlotObj[0].MeetingId}, eb_currval('eb_meeting_slot_participants_id_seq')); ";
                }
                else if (SlotObj[0].Is_approved == "F")
                {
                    query = $@"insert into eb_meetings (eb_meeting_slots_id, eb_created_by)
                            values({SlotObj[0].Slot_id}, 1);
                        insert into eb_meeting_slot_participants(user_id, confirmation, eb_meeting_schedule_id, approved_slot_id, name, email, type_of_user, participant_type) 
                            values ({usr.UserId}, 1, {SlotObj[0].Meeting_schedule_id}, {ApprovedSlotId}, '{usr.FullName}', '{usr.Email}', 1, 2); ";

                    for (int k = 0; k < SlotObj.Count; k++)
                        query += $"insert into eb_meeting_participants(eb_meeting_id, eb_slot_participant_id) values ( eb_currval('eb_meetings_id_seq'),{SlotObj[k].Participant_id}); ";

                    query += $"insert into eb_meeting_participants(eb_meeting_id, eb_slot_participant_id ) values (eb_currval('eb_meetings_id_seq'), eb_currval('eb_meeting_slot_participants_id_seq'));";
                    query += $"update eb_meeting_slots set is_approved = 'T' where  id = {ApprovedSlotId}; ";
                }
            }
            else
            {

            }
            _extqry += query;
            return true;
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
        public int Attendee_count { get; set; }
        public int Host_count { get; set; }

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

    public class ScheduledParticipants
    {
        public int Id { get; set; }
        public int SlotId { get; set; }
        public int ScheduleId { get; set; }
        public int UserId { get; set; }
        public int RoleId { get; set; }
        public int UserGroupId { get; set; }
        public int ParticipantType { get; set; }
    }

    [DataContract]
    public class MeetingSaveValidateResponse
    {
        [DataMember(Order = 0)]
        public bool ResponseStatus { get; set; }

        [DataMember(Order = 1)]
        public int UserId { get; set; }
    }
    public class AddMeetingSlotRequest
    {
        public string Date { get; set; }
    }
    public class AddMeetingSlotResponse
    {
        public bool Status { get; set; }
    }
}
