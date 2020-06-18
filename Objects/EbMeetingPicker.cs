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
using static ExpressBase.Objects.EbMeetingScheduler;

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
            string EbCtrlHTML = @"<div id='cont_@ebsid@' ebsid='@ebsid@' name='@name@' class='Eb-ctrlContainer meeting-picker-outer' @childOf@ ctype='@type@'>
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
            string EbCtrlHTML = @"<div id='cont_@ebsid@' ebsid='@ebsid@' name='@name@' class='Eb-ctrlContainer meeting-picker-outer' @childOf@ ctype='@type@'>
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
			 B.no_of_attendee, B.no_of_hosts,B.max_hosts,B.max_attendees,C.participant_type,
			 COALESCE (C.slot_host, 0) as slot_host_count,
		     COALESCE (C.slot_host_attendee, 0) as slot_attendee_count,
			 COALESCE (D.id, 0) as meeting_id,
			 COALESCE (C.id, 0) as participant_id
	            FROM
				(SELECT 
						id, eb_meeting_schedule_id , is_approved, 
					meeting_date, time_from, time_to
	                     FROM 
		                     eb_meeting_slots 
	                     WHERE 
		                     eb_del = 'F' and id = {0})A
						LEFT JOIN	 
							 (SELECT id, no_of_attendee, no_of_hosts,max_hosts,max_attendees FROM  eb_meeting_schedule)B
							 ON
 	                     B.id = A.eb_meeting_schedule_id
						LEFT JOIN	
						(SELECT 
		                     id,eb_meeting_schedule_id,approved_slot_id ,type_of_user,participant_type, COUNT(approved_slot_id)filter(where participant_type = 1) as slot_host,
						 		                    COUNT(approved_slot_id)filter(where participant_type = 2) as slot_host_attendee
	                     FROM 
		                     eb_meeting_slot_participants
	                     GROUP BY
		                     id,eb_meeting_schedule_id, approved_slot_id, type_of_user,participant_type, eb_del
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
 	                     D.eb_meeting_slots_id = A.id;
                                       SELECT 
			         B.user_id, B.role_id, B.user_group_id,B.participant_type
					 FROM
				        (SELECT 
						id, eb_meeting_schedule_id FROM  eb_meeting_slots 
	                     WHERE  eb_del = 'F' and id = {0})A
						LEFT JOIN	 
							 (SELECT id, user_id, role_id,user_group_id,participant_type,eb_meeting_schedule_id  
							  FROM  eb_meeting_scheduled_participants )B
							 ON B.eb_meeting_schedule_id = A.eb_meeting_schedule_id
							 where participant_type is NOT NULL   ;
                     SELECT 
	                  A.id as slot_id,A.eb_meeting_schedule_id,C.id as user_id,C.fullname,C.email,C.phnoprimary as phone
			        FROM 
					(SELECT id , eb_meeting_schedule_id FROM  eb_meeting_slots where id= {0} AND  eb_del = 'F')A	
							 LEFT JOIN	
							 (SELECT id,eb_created_by FROM  eb_meeting_schedule )B
							 ON B.id = A.eb_meeting_schedule_id	
							 LEFT JOIN
							 (select id, fullname,email,phnoprimary from eb_users where eb_del = 'F')C
							 ON C.id = B.eb_created_by where c.id is not  null  ;
                                        ";
            }
        }

        public string ValidateQuery
        {
            get
            {
                return @"
            SELECT 
		     A.id as slot_id , A.eb_meeting_schedule_id, A.is_approved,
			 B.no_of_attendee, B.no_of_hosts,B.max_hosts,B.max_attendees,
			 COALESCE (D.id, 0) as meeting_id
	            FROM
				(SELECT 
						id, eb_meeting_schedule_id , is_approved, 
					meeting_date, time_from, time_to
	                     FROM 
		                     eb_meeting_slots 
	                     WHERE 
		                     eb_del = 'F' and id = {0})A
						LEFT JOIN	 
							 (SELECT id, no_of_attendee, no_of_hosts,max_hosts,max_attendees FROM  eb_meeting_schedule)B
							 ON
 	                     B.id = A.eb_meeting_schedule_id	
                     LEFT JOIN 
                     (SELECT 
		                     id, eb_meeting_slots_id
	                     FROM 
		                     eb_meetings
	                     where
		                     eb_del = 'F') D
		                     ON
 	                     D.eb_meeting_slots_id = A.id ;
                SELECT 
		     A.id as slot_id , A.eb_meeting_schedule_id,
			 COALESCE (B.id, 0) as participant_id,B.participant_type,B.type_of_user
	            FROM
				(SELECT id, eb_meeting_schedule_id
	                     FROM  eb_meeting_slots 
	                     WHERE  eb_del = 'F' and id = {0})A
						LEFT JOIN	
						(SELECT id,eb_meeting_schedule_id,approved_slot_id ,type_of_user,participant_type
	                     FROM eb_meeting_slot_participants
	                     GROUP BY
		                     id,eb_meeting_schedule_id, approved_slot_id, type_of_user,participant_type, eb_del
	                     Having eb_del = 'F')B
                     ON B.eb_meeting_schedule_id = A.eb_meeting_schedule_id and B.approved_slot_id = A.id 
                         where participant_type is not null; 
 
 						select count(*) as slot_attendee_count from eb_meeting_slot_participants where approved_slot_id = {0} 
									   and participant_type=2;
						select count(*) as slot_host_count from eb_meeting_slot_participants where approved_slot_id = {0} 
									   and participant_type=1;
                    SELECT 
			         B.user_id, B.role_id, B.user_group_id,B.participant_type
					 FROM
				        (SELECT 
						id, eb_meeting_schedule_id FROM  eb_meeting_slots 
	                     WHERE  eb_del = 'F' and id = {0})A
						LEFT JOIN	 
							 (SELECT id, user_id, role_id,user_group_id,participant_type,eb_meeting_schedule_id  
							  FROM  eb_meeting_scheduled_participants )B
							 ON B.eb_meeting_schedule_id = A.eb_meeting_schedule_id and B.participant_type ='1'
							 where participant_type is NOT NULL ;
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

            List<ScheduledParticipants> ScheduledParticipants = new List<ScheduledParticipants>();

            List<MeetingScheduleDetails> MSD = new List<MeetingScheduleDetails>(); //MSD Meeting Schedule Details
            List<SlotParticipantsDetails> SPL = new List<SlotParticipantsDetails>(); //SPL Slot Participant List
            SlotParticipantCount SPC = new SlotParticipantCount(); //SPL Slot Participant Count
            HostInfo HostInfo = new HostInfo(); //if there is no eligible hosts
            String _query = string.Format(this.ValidateQuery, ApprovedSlotId);
            EbDataSet ds = DataDB.DoQueries(_query);
            if (ds.Tables[0].Rows.Count == 0)
                throw new FormException("Requested meeting slot is invalid", (int)HttpStatusCodes.BAD_REQUEST, "Query returned 0 rows for Meeting slot : " + ApprovedSlotId, "From EbMeetingPicker.ParameterizeControl()");

            for (int k = 0; k < ds.Tables[0].Rows.Count; k++)
            {
                MSD.Add(new MeetingScheduleDetails()
                {
                    SlotId = Convert.ToInt32(ds.Tables[0].Rows[k]["slot_id"]),
                    MeetingScheduleId = Convert.ToInt32(ds.Tables[0].Rows[k]["eb_meeting_schedule_id"]),
                    MeetingId = Convert.ToInt32(ds.Tables[0].Rows[k]["meeting_id"]),
                    MinAttendees = Convert.ToInt32(ds.Tables[0].Rows[k]["no_of_attendee"]),
                    MinHosts = Convert.ToInt32(ds.Tables[0].Rows[k]["no_of_hosts"]),
                    MaxAttendees = Convert.ToInt32(ds.Tables[0].Rows[k]["max_attendees"]),
                    MaxHosts = Convert.ToInt32(ds.Tables[0].Rows[k]["max_hosts"]),
                    IsApproved = Convert.ToString(ds.Tables[0].Rows[k]["is_approved"])
                });
            }
            for (int k = 0; k < ds.Tables[1].Rows.Count; k++)
            {
                SPL.Add(new SlotParticipantsDetails()
                {
                    SlotId = Convert.ToInt32(ds.Tables[1].Rows[k]["slot_id"]),
                    MeetingScheduleId = Convert.ToInt32(ds.Tables[1].Rows[k]["eb_meeting_schedule_id"]),
                    ParticipantId = Convert.ToInt32(ds.Tables[1].Rows[k]["participant_id"]),
                    ParticipantType = Convert.ToInt32(ds.Tables[1].Rows[k]["participant_type"]),
                    TypeOfUser = Convert.ToInt32(ds.Tables[1].Rows[k]["type_of_user"])
                });
            }
            SPC.SlotHostCount = Convert.ToInt32(ds.Tables[2].Rows[0]["slot_attendee_count"]);
            SPC.SlotAttendeeCount = Convert.ToInt32(ds.Tables[3].Rows[0]["slot_host_count"]);

            for (int k = 0; k < ds.Tables[4].Rows.Count; k++)
            {
                ScheduledParticipants.Add(new ScheduledParticipants()
                {
                    UserId = Convert.ToInt32(ds.Tables[4].Rows[k]["user_id"]),
                    RoleId = Convert.ToInt32(ds.Tables[4].Rows[k]["role_id"]),
                    UserGroupId = Convert.ToInt32(ds.Tables[4].Rows[k]["user_group_id"]),
                    ParticipantType = Convert.ToInt32(ds.Tables[4].Rows[k]["participant_type"]),
                });
            }
            //HostInfo.SlotId = Convert.ToInt32(ds.Tables[5].Rows[0]["slot_id"]);
            //HostInfo.MeetingScheduleId = Convert.ToInt32(ds.Tables[5].Rows[0]["eb_meeting_schedule_id"]);
            //HostInfo.UserId = Convert.ToInt32(ds.Tables[5].Rows[0]["user_id"]);
            //HostInfo.FullName = Convert.ToString(ds.Tables[5].Rows[0]["fullname"]);
            //HostInfo.Email = Convert.ToString(ds.Tables[5].Rows[0]["email"]);
            //HostInfo.Phone = Convert.ToString(ds.Tables[5].Rows[0]["phone"]);

            if (MSD[0].MaxAttendees <= SPC.SlotAttendeeCount)// assuming user in an attendee
                throw new FormException("Unable to continue. Reached maximum attendee limit.", (int)HttpStatusCodes.BAD_REQUEST, $"Max no of attendee : {MSD[0].MinAttendees}, Current attendee count : {SPC.SlotAttendeeCount}", "From EbMeetingPicker.ParameterizeControl()");

            string query = string.Empty;
            if (ScheduledParticipants.Count == 0 && MSD[0].MaxAttendees > SPC.SlotAttendeeCount)
            {
                if (MSD[0].IsApproved == "F" && MSD[0].MinAttendees == (SPC.SlotAttendeeCount + 1) && MSD[0].MinHosts <= SPC.SlotHostCount && MSD[0].MaxHosts >= SPC.SlotHostCount)
                {
                    query += $@"insert into eb_meetings (eb_meeting_slots_id, eb_created_by)
                            values({MSD[0].SlotId}, 1);
                        insert into eb_meeting_slot_participants(user_id, confirmation, eb_meeting_schedule_id, approved_slot_id, name, email, type_of_user, participant_type) 
                            values ({usr.UserId}, 1, {MSD[0].MeetingScheduleId}, {ApprovedSlotId}, '{usr.FullName}', '{usr.Email}', 1, 2); ";
                    for (int k = 0; k < SPL.Count; k++)
                        query += $"insert into eb_meeting_participants(eb_meeting_id, eb_slot_participant_id) values ( eb_currval('eb_meetings_id_seq'),{SPL[k].ParticipantId}); ";

                    query += $"insert into eb_meeting_participants(eb_meeting_id, eb_slot_participant_id ) values (eb_currval('eb_meetings_id_seq'), eb_currval('eb_meeting_slot_participants_id_seq'));";
                    query += $"update eb_meeting_slots set is_approved = 'T' where  id = {ApprovedSlotId}; ";
                }
                else if (MSD[0].IsApproved == "T")
                {
                    query += $@"insert into eb_meeting_slot_participants(user_id, confirmation, eb_meeting_schedule_id, approved_slot_id, name, email, type_of_user, participant_type) 
                            values ({usr.UserId}, 1, {MSD[0].MeetingScheduleId}, {ApprovedSlotId}, '{usr.FullName}', '{usr.Email}', 1, 2);
                        insert into eb_meeting_participants(eb_meeting_id, eb_slot_participant_id) 
                            values({MSD[0].MeetingId}, eb_currval('eb_meeting_slot_participants_id_seq')); ";
                }
                else if (MSD[0].IsApproved == "F" && (SPC.SlotAttendeeCount + 1) < MSD[0].MinAttendees)
                {
                    query += $@"insert into eb_meeting_slot_participants(user_id, confirmation, eb_meeting_schedule_id, approved_slot_id, name, email, type_of_user, participant_type) 
                            values ({usr.UserId}, 1, {MSD[0].MeetingScheduleId}, {ApprovedSlotId}, '{usr.FullName}', '{usr.Email}', 1, 2);  ";
                }
            }
            else
            {
                query += $@"insert into eb_meeting_slot_participants(user_id, confirmation, eb_meeting_schedule_id, approved_slot_id, name, email, type_of_user, participant_type) 
                            values ({usr.UserId}, 1, {MSD[0].MeetingScheduleId}, {ApprovedSlotId}, '{usr.FullName}', '{usr.Email}', 1, 2);";
                for (i = 0; i < ScheduledParticipants.Count; i++)
                {
                    query += $@"insert into eb_my_actions (user_ids,usergroup_id,role_ids,from_datetime,form_ref_id,form_data_id,description,my_action_type , eb_meeting_slots_id,
                        is_completed,eb_del)
                        values('{ScheduledParticipants[i].UserId}',{ScheduledParticipants[i].UserGroupId},'{ScheduledParticipants[i].RoleId}',
                        NOW(),@refid, eb_currval('{tbl}_id_seq'), 'Meeting Request','{MyActionTypes.Meeting}',{ApprovedSlotId} , 'F','F');";
                }
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
        public int Duration { get; set; }

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
        public int Duration { get; set; }

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
        public int Count { get; set; }
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
    public class SlotDetailsRequest
    {
        public int Id { get; set; }
    }
    public class MeetingRequest
    {
        public int MaId { get; set; }
        public string MaIsCompleted { get; set; }
        public int Slotid { get; set; }
        public int MeetingScheduleid { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string MeetingDate { get; set; }
        public string TimeFrom { get; set; }
        public string TimeTo { get; set; }
        public string Venue { get; set; }
        public string Integration { get; set; }
        public int TypeofUser { get; set; }
        public int ParticipantType { get; set; }
        public string fullname { get; set; }
        public int UserId { get; set; }
        public int MeetingId { get; set; }
    }
    public class SlotDetailsResponse
    {
        public List<MeetingRequest> MeetingRequest { get; set; }
        public SlotDetailsResponse()
        {
            this.MeetingRequest = new List<MeetingRequest>();
        }
    }
    public class MeetingUpdateByUsersRequest
    {
        public int Id { get; set; }
        public int MyActionId { get; set; }
        public User UserInfo { get; set; }
    }
    public class MeetingUpdateByUsersResponse
    {
        public bool ResponseStatus { get; set; }
    }
    public class MyAction
    {
        public int Id { get; set; }
        public int SlotId { get; set; }
        public string UserIds { get; set; }
        public int UserGroupId { get; set; }
        public string RoleIds { get; set; }
        public string FormRefId { get; set; }
        public string Description { get; set; }
        public string ExpiryDateTime { get; set; }
        public string ExceptUserIds { get; set; }
        public int FormDataId { get; set; }

    }

    public class SlotParticipantsDetails
    {
        public int ParticipantId { get; set; }
        public int SlotId { get; set; }
        public int MeetingScheduleId { get; set; }
        public int ParticipantType { get; set; }
        public int TypeOfUser { get; set; }
        public int Confirmation { get; set; }
        public int UserId { get; set; }
    }
    public class MeetingScheduleDetails
    {
        public int SlotParticipantId { get; set; }
        public int SlotId { get; set; }
        public int MeetingScheduleId { get; set; }
        public string IsApproved { get; set; }
        public int MinAttendees { get; set; }
        public int MinHosts { get; set; }
        public int MaxAttendees { get; set; }
        public int MaxHosts { get; set; }
        public int MeetingId { get; set; }
        public MeetingOptions MeetingOpts { get; set; }
    }
    public class SlotParticipantCount
    {
        public int SlotAttendeeCount { get; set; }
        public int SlotHostCount { get; set; }
    }

    public class MeetingCancelByHostResponse
    {
        public bool ResponseStatus { get; set; }
    }
    public class MeetingCancelByHostRequest
    {
        public int SlotId { get; set; }
        public int MyActionId { get; set; }
        public User UserInfo { get; set; }
    }
    public class MeetingRejectByHostResponse
    {
        public bool ResponseStatus { get; set; }
    }
    public class MeetingRejectByHostRequest
    {
        public int SlotId { get; set; }
        public int MyActionId { get; set; }
        public User UserInfo { get; set; }
    }
    public class HostInfo
    {
        public int SlotId { get; set; }
        public int MeetingScheduleId { get; set; }
        public int UserId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
    }

    public class MeetingRequestByEligibleAttendeeRequest
    {

    }
    public class MeetingRequestByEligibleAttendeeResponse
    {

    }
    public class GetMeetingDetailRequest
    {
        public int MeetingId { get; set; }
    }
    public class GetMeetingDetailsResponse
    {
        public List<MeetingRequest> MeetingRequest { get; set; }
        public GetMeetingDetailsResponse()
        {
            this.MeetingRequest = new List<MeetingRequest>();
        }
    }

    public enum ParticipantType
    {
        Host = 1,
        Attendee = 2
    }
    public enum TypeOfUser
    {
        EbUser = 1,
        AnonymousUser = 2
    }
}
