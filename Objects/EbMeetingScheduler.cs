using ExpressBase.Common;
using ExpressBase.Common.EbServiceStack.ReqNRes;
using ExpressBase.Common.Extensions;
using ExpressBase.Common.LocationNSolution;
using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using ExpressBase.Common.Structures;
using ExpressBase.Objects.ServiceStack_Artifacts;
using ExpressBase.Security;
using Newtonsoft.Json;
using ServiceStack;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Runtime.Serialization;

namespace ExpressBase.Objects
{

    [EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm)]
    public class EbMeetingScheduler : EbControlUI
    {
        public EbMeetingScheduler() { }
        public override string ToolIconHtml { get { return "<i class='fa fa-i-cursor'></i>"; } set { } }

        public string TableName { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm)]
        [HideInPropertyGrid]
        public override EbDbTypes EbDbType { get { return EbDbTypes.String; } }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm)]
        [DefaultPropValue("1")]
        public int MeetingId { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm)]
        [HideInPropertyGrid]
        public Dictionary<int, string> UsersList { get; set; }

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
            get => @"<div id='@ebsid@' ebsid='@ebsid@' name='@name@' class='Eb-ctrlContainer meeting-scheduler-outer' @childOf@ ctype='@type@'>
                        
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
                       
                </div>".Replace("@date_val@", DateTime.Today.ToString("yyyy-MM-dd"));
            return ReplacePropsInHTML(jk);
        }

        public override string GetBareHtml()
        {
            string EbCtrlHTML = @"<div id='cont_@ebsid@' ebsid='@ebsid@' name='@name@' class='Eb-ctrlContainer meeting-picker-outer' @childOf@ ctype='@type@'>
                       GetBareHtml
                </div>"
                .Replace("@name@", this.Name)
                .Replace("@Label@", this.Label)
                .Replace("@date_val@", DateTime.Today.ToString("yyyy-MM-dd"))
                .Replace("@ebsid@", this.EbSid_CtxId);
            return EbCtrlHTML;
        }


        public override string GetHtml()
        {
            string EbCtrlHTML = @"<div id='cont_@ebsid@' ebsid='@ebsid@' name='@name@' class='Eb-ctrlContainer meeting-scheduler-outer' @childOf@ ctype='@type@'>
                <div class='meeting-info'>
                Title : <input type='text' placeholder='Title' id='@ebsid@_meeting-title' class='mc-input'/>
                Description : <textarea id='@ebsid@_description' class='mc-input'> </textarea> </div>
                <div class='meeting-type'>
                <input type='radio' id='@ebsid@_single' name='meeting-type' value='single' class='mc-input' checked>
                <label for='single'>Single Meeting</label>
                <input type='radio' id='@ebsid@_multiple' name='meeting-type' value='female' class='mc-input'>
                <label for='multiple'>Multiple Meetings</label>
                </div><div class='time' style='display: flex;'>
                <div class='meeting-date-time'>
                <div style='margin-right:15px;'><h5>Date</h5> <input type='text' id='@ebsid@_meeting-date' val='@date_val@' class='mc-input'></div>
                <div style='margin-right:15px;'><h5>Time from</h5><input type='time' id='@ebsid@_time-from' class='mc-input'></div>
                <div style='margin-right:15px;'><h5>Time to</h5><input type='time' id='@ebsid@_time-to' class='mc-input'></div>
                <div class='meeting-duration' style='display:none'>
                <h5>Duration</h5> <input type='text' id='@ebsid@_duration' data-format='HH:mm' data-template='HH : mm' name='datetime' class='mc-input'>
                </div></div></div>
                <div class='meeting-count'>
                Max-host : <input type='number' id='@ebsid@_max-host' class='meeting-spinner mc-input' min='1' max='5' value='1' >
                Min-host : <input type='number' id='@ebsid@_min-host' class='meeting-spinner mc-input' min='1' max='5' value='1'>
                Max-Attendee<input type='number' id='@ebsid@_max-attendee' class='meeting-spinner mc-input' min='1' max='5' value='1'>
                Min-Attendee<input type='number' id='@ebsid@_min-attendee' class='meeting-spinner mc-input' min='1' max='5' value='1'>
                </div>
                <div class='eligible-participant'><div style='width:100%'>
                <h5>Eligible Host</h5> <input type='text' id='@ebsid@_eligible_host_list' class='mc-input'/> </div>
                <div  style='width:100%'><h5>Eligible Attendee</h5><input type='text' id='@ebsid@_eligible_attendee_list' class='mc-input'/></div>
                </div>
                <div class='direct-participant'><div  style='width:100%'>
                <h5>Host</h5><input type='text' id='@ebsid@_host_list' class='mc-input'/></div>
                <div  style='width:100%'><h5>Attendee </h5> <input type='text'  id='@ebsid@_attendee_list' class='mc-input'/></div>
                </div>
                <div class='meeting-recurance'>
                <div class='is-recuring'><input type='checkbox' id='@ebsid@_recuring_meeting' name='recuring' value='recuring'>
                <label for='@ebsid@_recuring_meeting'> Is recuring</label></div>    
                <div class='recuring-days' id='@ebsid@_recuring-days'>
                  <label class='checkbox-inline' ><input type='checkbox' data-label='Sun' data-code='0' checked>Sunday</label>
                  <label class='checkbox-inline' ><input type='checkbox' data-label='Mon' data-code='1' checked>Monday</label>
                  <label class='checkbox-inline' ><input type='checkbox' data-label='Tue' data-code='2' checked>Tuesday</label>
                  <label class='checkbox-inline' ><input type='checkbox' data-label='Wed' data-code='3' checked>Wednesday</label>
                  <label class='checkbox-inline' ><input type='checkbox' data-label='Thu' data-code='4' checked>Thursday</label>
                  <label class='checkbox-inline' ><input type='checkbox' data-label='Fri' data-code='5' checked>Friday</label>
                  <label class='checkbox-inline' ><input type='checkbox' data-label='Sat' data-code='6' checked>Saturday</label>
                </div>
                <div>
                Cron Exp : <input type='text' id='@ebsid@_crone-exp'>
                Cron Exp End Date :<input type='text' id='@ebsid@_crone-exp-end'></div>
                </div>
            <input type='text' id='@ebsid@_MeetingJson' readonly hidden/>
                </div>
                    "
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
                        GetDesignHtmlHelper
                </div>"
               .Replace("@name@", this.Name)
               .Replace("@date_val@", DateTime.Today.ToString("yyyy-MM-dd"))
               .Replace("@Label@", this.Label);
            return ReplacePropsInHTML(EbCtrlHTML);
        }

        public override string GetValueFromDOMJSfn { get { return @"return $('#' + this.EbSid_CtxId +'_MeetingJson').val();"; } set { } }

        public override string OnChangeBindJSFn { get { return @" $('#' +  this.EbSid_CtxId +'_MeetingJson').on('change', p1);"; } set { } }

        public override string SetValueJSfn { get { return @"  $('#' + this.EbSid_CtxId +'_MeetingJson').val(p1).trigger('change');"; } set { } }

        public override string JustSetValueJSfn { get { return @"$('#' + this.EbSid_CtxId +'_MeetingJson').val(p1)"; } set { } }

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
        public override bool ParameterizeControl(IDatabase DataDB, List<DbParameter> param, string tbl, SingleColumn cField, bool ins, ref int i, ref string _col, ref string _val, ref string _extqry, User usr, SingleColumn ocF)
        {
            if (!ins)
                return false;
            MeetingSchedule Mobj = new MeetingSchedule();
            Mobj = JsonConvert.DeserializeObject<MeetingSchedule>(cField.Value.ToString());
            string[] Host = Mobj.Host.Split(',').Select(sValue => sValue.Trim()).ToArray();
            string[] Attendee = Mobj.Attendee.Split(',').Select(sValue => sValue.Trim()).ToArray();
            string query = "";
            if (Mobj.IsRecuring == "T")
            {

            }
            if (Mobj.IsMultipleMeeting == "T")
            {

            }
            else if (Mobj.IsMultipleMeeting == "F")
            {
                query += $@"
            insert into eb_meeting_schedule (title,description,meeting_date,time_from,time_to,duration,is_recuring,is_multiple,venue,
			integration,max_hosts,max_attendees,no_of_attendee,no_of_hosts,eb_created_by,eb_created_at)
			values('{Mobj.Title}','{Mobj.Description}','{Mobj.Date}','{Mobj.TimeFrom}:00','{Mobj.TimeTo}:00', 0,'{Mobj.IsRecuring}',
            '{Mobj.IsMultipleMeeting}','{Mobj.Location}','',{Mobj.MaxHost},{Mobj.MaxAttendee},{Mobj.MinAttendee},{Mobj.MinHost},{usr.UserId},now());
            insert into eb_meeting_slots (eb_meeting_schedule_id,meeting_date,time_from,time_to,eb_created_by,eb_created_at) values 
            (eb_currval('eb_meeting_schedule_id_seq'),'{Mobj.Date}','{Mobj.TimeFrom}','{Mobj.TimeTo}', {usr.UserId},now() );
            insert into eb_my_actions (user_ids,from_datetime,form_ref_id,form_data_id,description,my_action_type , eb_meeting_slots_id, 
            is_completed,eb_del) values('{Mobj.Host}',NOW(),@refid, eb_currval('{tbl}_id_seq'), 'Meeting Request',
            '{MyActionTypes.Meeting}',eb_currval('eb_meeting_slots_id_seq') , 'F','F');  
            insert into eb_my_actions (user_ids,from_datetime,form_ref_id,form_data_id,description,my_action_type , eb_meeting_slots_id, 
            is_completed,eb_del) values('{Mobj.Attendee}',NOW(),@refid, eb_currval('{tbl}_id_seq'), 'Meeting Request',
            '{MyActionTypes.Meeting}',eb_currval('eb_meeting_slots_id_seq') , 'F','F');
             ";
                for (i = 0; i < Host.Length; i++)
                {
                    query += $@"
                     insert into eb_meeting_slot_participants(user_id, confirmation, eb_meeting_schedule_id, approved_slot_id, name, email, type_of_user, participant_type) 
                     values ({Host[i]}, 2, eb_currval('eb_meeting_schedule_id_seq'), eb_currval('eb_meeting_slots_id_seq'), '', '', 1, 1);
                    ";
                }
                for (i = 0; i < Attendee.Length; i++)
                {
                    query += $@"
                     insert into eb_meeting_slot_participants(user_id, confirmation, eb_meeting_schedule_id, approved_slot_id, name, email, type_of_user, participant_type) 
                     values ({Attendee[i]}, 2, eb_currval('eb_meeting_schedule_id_seq'), eb_currval('eb_meeting_slots_id_seq'), '', '', 1, 2);
                    ";
                }
            }
            _extqry += query;
            return true;
        }


        public class MeetingSchedule
        {
            //    Title: '', Description: '', Location: '', IsSingleMeeting: 'T', IsMultipleMeeting: 'F', Date: '',
            //TimeFrom: '', TimeTo: '', Duration: '', MaxHost: 1, MinHost: 1, MaxAttendee: 1, MinAttendee: 1,
            //EligibleHosts: '', EligibleAttendees: '', Host: '', Attendee: '', IsRecuring: 'F', DayCode: 0
            public string Title { get; set; }
            public string Description { get; set; }
            public string Location { get; set; }
            public string IsSingleMeeting { get; set; }
            public string IsMultipleMeeting { get; set; }
            public string IsRecuring { get; set; }
            public int DayCode { get; set; }
            public string Date { get; set; }
            public string TimeFrom { get; set; }
            public string TimeTo { get; set; }
            public string Duration { get; set; }
            public int MaxHost { get; set; }
            public int MinHost { get; set; }
            public int MaxAttendee { get; set; }
            public int MinAttendee { get; set; }
            public string EligibleHosts { get; set; }
            public string EligibleAttendees { get; set; }
            public string Host { get; set; }
            public string Attendee { get; set; }
        }

    }
}

