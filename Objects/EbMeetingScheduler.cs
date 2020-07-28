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

        //[EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm)]
        //[HideInPropertyGrid]
        //public Dictionary<int, string> UsersList { get; set; }

        //[EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm)]
        //[HideInPropertyGrid]
        //public string ParticipantsList { get; set; }       

        [EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm)]
        [HideInPropertyGrid]
        public string HostParticipantsList { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm)]
        [HideInPropertyGrid]
        public string AttendeeParticipantsList { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm)]
        [HideInPropertyGrid]
        public MeetingOptions MeetingOpts { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm)]
        [DefaultPropValue("1")]
        public MeetingType MeetingType { get; set; }



        //[EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm)]
        //public MeetingEntityTypes MeetingConfig  { get; set; }     

        //[EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm)]
        //[PropDataSourceJsFn("return ebcontext.Roles")]
        //[PropertyEditor(PropertyEditorType.DropDown, true)]
        //public List<Int32> MeetingRoles { get; set; } 

        //[EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm)]
        //public int MeetingUserGroup { get; set; }

        //[EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm)]
        //public EbScript MeetingUsers { get; set; }


        [PropertyGroup("Host")]
        [EnableInBuilder(BuilderType.WebForm)]
        //MakeReadOnly MakeReadWrite ShowProperty HideProperty
        [OnChangeExec(@"
    if (this.HostConfig === 0) this.HostConfig = 1;
    pg.HideProperty('HostRoles');
    pg.HideProperty('HostUserGroup');
    pg.HideProperty('HostUsers');
    if (this.HostConfig === 1)
        pg.ShowProperty('HostRoles');
    else if (this.HostConfig === 2)
        pg.ShowProperty('HostUserGroup');
    else if (this.HostConfig === 3)
        pg.ShowProperty('HostUsers');
    ")]
        public UsersType HostConfig { get; set; }

        //[EnableInBuilder(BuilderType.WebForm)]
        //[Unique]
        //[PropDataSourceJsFn("return ebcontext.Roles")]
        //[PropertyEditor(PropertyEditorType.DropDown)]
        //public int ApproverRole { get; set; }

        [PropertyGroup("Host")]
        [EnableInBuilder(BuilderType.WebForm)]
        [Unique]
        [PropDataSourceJsFn("return ebcontext.Roles")]
        [PropertyEditor(PropertyEditorType.DropDown, true)]
        public List<Int32> HostRoles { get; set; }

        [PropertyGroup("Host")]
        [EnableInBuilder(BuilderType.WebForm)]
        [PropDataSourceJsFn("return ebcontext.UserGroups")]
        [PropertyEditor(PropertyEditorType.DropDown)]
        public int HostUserGroup { get; set; }

        [PropertyGroup("Host")]
        [EnableInBuilder(BuilderType.WebForm)]
        [PropertyEditor(PropertyEditorType.ScriptEditorCS)]//required ScriptEditorSQ
        public EbScript HostUsers { get; set; }


        [PropertyGroup("Attendee")]
        [EnableInBuilder(BuilderType.WebForm)]
        //MakeReadOnly MakeReadWrite ShowProperty HideProperty
        [OnChangeExec(@"
    if (this.AttendeeConfig === 0) this.AttendeeConfig = 1;
    pg.HideProperty('AttendeeRoles');
    pg.HideProperty('AttendeeUserGroup');
    pg.HideProperty('AttendeeUsers');
    pg.HideProperty('AttendeeContacts');
    pg.HideProperty('AttendeeContactFilter');
    if (this.AttendeeConfig === 1)
        pg.ShowProperty('AttendeeRoles');
    else if (this.AttendeeConfig === 2)
        pg.ShowProperty('AttendeeUserGroup');
    else if (this.AttendeeConfig === 3)
        pg.ShowProperty('AttendeeUsers');
else if (this.AttendeeConfig === 4)
        pg.ShowProperty('AttendeeContactFilter');
        pg.ShowProperty('AttendeeContacts');
    ")]
        public UsersType AttendeeConfig { get; set; }

        //[EnableInBuilder(BuilderType.WebForm)]
        //[Unique]
        //[PropDataSourceJsFn("return ebcontext.Roles")]
        //[PropertyEditor(PropertyEditorType.DropDown)]
        //public int ApproverRole { get; set; }

        [PropertyGroup("Attendee")]
        [EnableInBuilder(BuilderType.WebForm)]
        [Unique]
        [PropDataSourceJsFn("return ebcontext.Roles")]
        [PropertyEditor(PropertyEditorType.DropDown, true)]
        public List<Int32> AttendeeRoles { get; set; }

        [PropertyGroup("Attendee")]
        [EnableInBuilder(BuilderType.WebForm)]
        [PropDataSourceJsFn("return ebcontext.UserGroups")]
        [PropertyEditor(PropertyEditorType.DropDown)]
        public int AttendeeUserGroup { get; set; }

        [PropertyGroup("Attendee")]
        [EnableInBuilder(BuilderType.WebForm)]
        [PropertyEditor(PropertyEditorType.ScriptEditorCS)]//required ScriptEditorSQ
        public EbScript AttendeeUsers { get; set; }

        [EnableInBuilder(BuilderType.WebForm)]
        [PropertyEditor(PropertyEditorType.ObjectSelector)]
        [PropertyGroup("Attendee")]
        [OSE_ObjectTypes(EbObjectTypes.iWebForm)]
        public string AttendeeContactFilter { get; set; }

        [PropertyGroup("Attendee")]
        [EnableInBuilder(BuilderType.WebForm)]
        [PropertyEditor(PropertyEditorType.ScriptEditorCS)]//required ScriptEditorSQ
        public EbScript AttendeeContacts { get; set; }
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

        public void InitParticipantsList(JsonServiceClient ServiceClient)
        {
            ParticipantsListResponse abc = new ParticipantsListResponse();
            ParticipantsListRequest request = new ParticipantsListRequest();
            List<MeetingSuggestion> Opts = new List<MeetingSuggestion>();
            Opts.Add(new MeetingSuggestion()
            {
                MeetingConfig = this.HostConfig,
                MeetingRoles = this.HostRoles,
                MeetingUserGroup = this.HostUserGroup,
                MeetingUsers = this.HostUsers,
                ContactFilter = "",
                Contacts = null,
            });
            Opts.Add(new MeetingSuggestion()
            {
                MeetingConfig = this.AttendeeConfig,
                MeetingRoles = this.AttendeeRoles,
                MeetingUserGroup = this.AttendeeUserGroup,
                MeetingUsers = this.AttendeeUsers,
                ContactFilter = this.AttendeeContactFilter,
                Contacts = this.AttendeeContacts
            });
            abc = ServiceClient.Post<ParticipantsListResponse>(new ParticipantsListRequest { MeetingConfig = Opts });
            this.HostParticipantsList = JsonConvert.SerializeObject(abc.HostParticipantsList);
            this.AttendeeParticipantsList = JsonConvert.SerializeObject(abc.AttendeeParticipantsList);
        }

        public override string GetHtml4Bot()
        {
            return GetWrapedCtrlHtml4bot();
        }
        public override string GetWrapedCtrlHtml4bot()
        {
            string jk = @"<div id='@ebsid@' ebsid='@ebsid@' name='@name@' class='Eb-ctrlContainer meeting-picker-outer eb-meeting-bot' @childOf@ ctype='@type@'>
                       @GetWrapedCtrlHtml4bot@
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
                 @meetinghtml@
            <input type='text' id='@ebsid@_MeetingJson' readonly hidden/>
                </div>
                    ";
            if (this.MeetingType == MeetingType.SingleMeeting)
                EbCtrlHTML = EbCtrlHTML.Replace("@meetinghtml@", this.GetHtml4singleMeeting());
            else if (this.MeetingType == MeetingType.MultipleMeeting)
                EbCtrlHTML = EbCtrlHTML.Replace("@meetinghtml@", this.GetHtml4MultipleMeeting());
            else if (this.MeetingType == MeetingType.AdvancedMeeting)
                EbCtrlHTML = EbCtrlHTML.Replace("@meetinghtml@", this.GetHtml4MultipleMeeting());
            EbCtrlHTML.Replace("@name@", this.Name);
            EbCtrlHTML.Replace("@date_val@", DateTime.Today.ToString("yyyy-MM-dd"));
            EbCtrlHTML.Replace("@Label@", this.Label);
            return ReplacePropsInHTML(EbCtrlHTML);
        }
        public string abc()
        {
            return string.Empty;
        }

        public string GetHtml4singleMeeting()
        {
            string Html = @"
            <div class='single-meeting'><div style='display:flex'><div style='width: 100%;margin-right: 2rem;'><div class='title'> 
            <input type='text' placeholder='Title' id='@ebsid@_meeting-title' class='mc-input m-validate'/> </div>
            <div class='location'><input type='text'  placeholder='Location' id='@ebsid@_location' class='mc-input m-validate' /></div>
            <div class='date'><input type='text' placeholder='Date' id='@ebsid@_meeting-date' val='@date_val@' class='mc-input m-validate' /></div> </div>
            <div style='width: 100%;'><div class='description'><textarea id='@ebsid@_description' placeholder='Describe about the event...' rows='10' cols='10' class='mc-input m-validate' ></textarea></div>
            <div class='integration'><input type='text' placeholder='Integration' id='@ebsid@_integration' class='mc-input' /></div></div></div>
            <div class='slots-table' id='@ebsid@_slots'>
            <table id='@ebsid@_slot-table' class='slot-tbl'> <thead><tr><th>Time From</th><th>Time To</th><th>Host</th><th>Attendee</th><th></th></tr>
            <tbody>
            <tr data-id='0'>
            <td class='time'><input type='time' id='@ebsid@_time-from'  class='mc-input time-from m-validate' /></td>
            <td class='time'><input type='time' id='@ebsid@_time-to'  class='mc-input time-to m-validate' /></td>
            <td><input type='text' id='@ebsid@_host_0'  class='meeting-participants tb-host'/></td>
            <td><input type='text' id='@ebsid@_attendee_0' class='meeting-participants tb-attendee'/></td>
            <td style='width:5rem;'></td></td></tr>
            </tbody>
            </table>
            </div>
            </div>
        ";
            return Html;
        }

        public string GetHtml4MultipleMeeting()
        {
            string Html = @"
            <div class='single-meeting'><div style='display:flex'><div style='width: 100%;margin-right: 2rem;'><div class='title'> 
            <input type='text' placeholder='Title' id='@ebsid@_meeting-title' class='mc-input m-validate'/> </div>
            <div class='location'><input type='text'  placeholder='Location' id='@ebsid@_location' class='mc-input m-validate' /></div>
            <div class='date'><input type='text' placeholder='Date' id='@ebsid@_meeting-date' val='@date_val@' class='mc-input m-validate' /></div> </div>
            <div style='width: 100%;'><div class='description'><textarea id='@ebsid@_description' placeholder='Describe about the event...' rows='10' cols='10' class='mc-input m-validate' ></textarea></div>
            <div class='integration'><input type='text' placeholder='Integration' id='@ebsid@_integration' class='mc-input' /></div></div></div>
            <div class='slots-table' id='@ebsid@_slots'>
            <table id='@ebsid@_slot-table' class='slot-tbl'> <thead><tr><th>Time From</th><th>Time To</th><th>Host</th><th>Attendee</th><th></th></tr>
            <tbody>
            <tr data-id='0'>
            <td class='time'><input type='time' id='@ebsid@_time-from'  class='mc-input time-from m-validate'  /></td>
            <td class='time'><input type='time' id='@ebsid@_time-to'  class='mc-input time-to m-validate' /></td>
            <td><input type='text' id='@ebsid@_host_0'  class='meeting-participants tb-host'/></td>
            <td><input type='text' id='@ebsid@_attendee_0' class='meeting-participants tb-attendee'/></td>
            <td style='width:5rem;'><button id='@ebsid@_remove-slot' class='remove-slot' > <i class='fa fa-window-close'></i></button></td></td></tr>
            </tbody>
            </table>
            <button id='@ebsid@_add-new-slot'> <i class='fa fa-plus-square'></i> Add Slots </button>
            </div>
            </div>

        ";
            return Html;
        }
        public string AdvancedMeeting()
        {
            //string Html = @"
            //    Title : <input type='text' placeholder='Title' id='@ebsid@_meeting-title' class='mc-input' />
            //    Description : <textarea id='@ebsid@_description' class='mc-input' > </textarea> 
            //    <input type='text' id='@ebsid@_meeting-date' val='@date_val@' class='mc-input' />
            //    <input type='text' id='@ebsid@_duration' data-format='HH:mm' data-template='HH : mm' name='datetime' class='mc-input'>
            //    Max-host : <input type='number' id='@ebsid@_max-host' class='meeting-spinner mc-input' min='1' max='5' value='1' >
            //    Min-host : <input type='number' id='@ebsid@_min-host' class='meeting-spinner mc-input' min='1' max='5' value='1'>
            //    Max-Attendee : <input type='number' id='@ebsid@_max-attendee' class='meeting-spinner mc-input' min='1' max='5' value='1'>
            //    Min-Attendee : <input type='number' id='@ebsid@_min-attendee' class='meeting-spinner mc-input' min='1' max='5' value='1'>
            //    <div class='eligible-participant'><div style='width:100%'>
            //    <h5>Eligible Host</h5> <input type='text' id='@ebsid@_eligible_host_list' class='mc-input eligible-userids'/> </div>
            //    <div  style='width:100%'><h5>Eligible Attendee</h5><input type='text' id='@ebsid@_eligible_attendee_list' class='mc-input eligible-userids'/></div>
            //    </div>
            //    <div class='slots-table' id='@ebsid@_slots'>
            //    <table> <thead><tr><th>Sl.No</th><th>Start Time</th><th>End Time</th><th>Host</th><th>Attendee</th></tr>
            //    <tbody></tbody>
            //    </table>
            //    </div>
            //    <div class='meeting-recurance'>
            //    <div class='is-recuring'><input type='checkbox' id='@ebsid@_recuring_meeting' name='recuring' value='recuring'>
            //    <label for='@ebsid@_recuring_meeting'> Is recuring</label></div>    
            //    <div class='recuring-days' id='@ebsid@_recuring-days'>
            //      <label class='checkbox-inline' ><input type='checkbox' data-label='Sun' data-code='0' checked>Sunday</label>
            //      <label class='checkbox-inline' ><input type='checkbox' data-label='Mon' data-code='1' checked>Monday</label>
            //      <label class='checkbox-inline' ><input type='checkbox' data-label='Tue' data-code='2' checked>Tuesday</label>
            //      <label class='checkbox-inline' ><input type='checkbox' data-label='Wed' data-code='3' checked>Wednesday</label>
            //      <label class='checkbox-inline' ><input type='checkbox' data-label='Thu' data-code='4' checked>Thursday</label>
            //      <label class='checkbox-inline' ><input type='checkbox' data-label='Fri' data-code='5' checked>Friday</label>
            //      <label class='checkbox-inline' ><input type='checkbox' data-label='Sat' data-code='6' checked>Saturday</label>
            //    </div>
            //    <div>
            //    Cron Exp : <input type='text' id='@ebsid@_crone-exp'>
            //    Cron Exp End Date :<input type='text' id='@ebsid@_crone-exp-end'></div>
            //    </div>";
            string Html = $@"     
            <div class='single-meeting'><div style='display:flex'><div style='width: 100%;margin-right: 2rem;'><div class='title'> <input type='text' placeholder='Title' id='@ebsid@_meeting-title' class='mc-input'/> </div>
            <div class='location'><input type='text'  placeholder='Location' id='@ebsid@_location' class='mc-input' /></div>
            <div class='date'><input type='text' placeholder='Date' id='@ebsid@_meeting-date' val='@date_val@' class='mc-input' /></div> </div>
            <div style='width: 100%;'><div class='description'><textarea id='@ebsid@_description' placeholder='Describe about the event...' rows='10' cols='10' class='mc-input' ></textarea></div>
            <div class='integration'><input type='text' placeholder='Integration' id='@ebsid@_integration' class='mc-input' /></div></div></div>
            <div class='slots-table' id='@ebsid@_slots'>
            <table id='@ebsid@_slot-table' class='slot-tbl'> <thead><tr><th>Time From</th><th>Time To</th><th>Host</th><th>Attendee</th><th></th></tr>
            <tbody>
            </tbody>
            </table>";
            return Html;
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

        public override string SetDisplayMemberJSfn { get { return @"$('#' + this.EbSid_CtxId +'_MeetingJson').val(p1)"; } set { } }

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
        public int UserIdCount(List<Participants> Participants, UsersType Type)
        {
            var count = Participants.Where(item => item.Type == Type).Count();
            return count;
        }
        public string ValidateQuery
        {
            get
            {
                return @"
            select A.id,A.user_id,B.meeting_date,B.time_from,B.time_to from
            (select id,user_id,approved_slot_id from eb_meeting_slot_participants)A
            left join 
            (select id,meeting_date,time_from,time_to from eb_meeting_slots) B
            ON B.id = A.approved_slot_id and B.time_from BETWEEN '{0}:00' and '{1}:00' AND B.time_to BETWEEN '{0}:00' and '{1}:00'
            and meeting_date ='{2}'
            where B.time_from is not null;
";
            }
        }

        public override bool ParameterizeControl(IDatabase DataDB, List<DbParameter> param, string tbl, SingleColumn cField, bool ins, ref int i, ref string _col, ref string _val, ref string _extqry, User usr, SingleColumn ocF)
        {
            if (!ins)
                return false;
            MeetingSchedule Mobj = new MeetingSchedule();
            Mobj = JsonConvert.DeserializeObject<MeetingSchedule>(cField.Value.ToString());
            //string[] Host = Mobj.Host.Split(',').Select(sValue => sValue.Trim()).ToArray();

            string query = "";
            if (Mobj.MeetingType == MeetingType.SingleMeeting)
            {
                query += AddMeetingSchedule(Mobj, usr);
                for (i = 0; i < Mobj.SlotList.Count; i++)
                {
                    query += $@"
                insert into eb_meeting_slots (eb_meeting_schedule_id,meeting_date,time_from,time_to,eb_created_by,eb_created_at) values 
                (eb_currval('eb_meeting_schedule_id_seq'),'{Mobj.Date}','{Mobj.SlotList[i].TimeFrom}','{Mobj.SlotList[i].TimeTo}', {usr.UserId},now() );";

                    int HostUserIdsCount = Mobj.SlotList[i].Hosts.Where(Item => Item.Type == UsersType.Users).Count();
                    int AttendeeUserIdsCount = Mobj.SlotList[i].Attendees.Where(Item => Item.Type == UsersType.Users).Count();
                    bool IsFixedHost = false;
                    int AttendeeContactIdsCount = Mobj.SlotList[i].Attendees.Where(Item => Item.Type == UsersType.Contact).Count();
                    bool IsFixedAttendee = false;
                    if (this.HostConfig == UsersType.Users && HostUserIdsCount >= Mobj.MinHost && HostUserIdsCount == Mobj.SlotList[i].Hosts.Count && HostUserIdsCount <= Mobj.MaxHost)
                    {
                        IsFixedHost = true;
                        query += MeetingSlotParticipantsQry(Mobj.SlotList[i].Hosts, usr, ParticipantOpt.Fixed, ParticipantType.Host, tbl, Mobj.SlotList[i], Mobj.Date, DataDB);
                    } 
                    else if (this.HostConfig == UsersType.Users && HostUserIdsCount < Mobj.MinHost)
                    {
                        throw new FormException("Schedule Meeting Failed : Minimum Host(s) Required");
                    }
                    else
                    {
                        query += MeetingSlotParticipantsQry(Mobj.SlotList[i].Hosts, usr, ParticipantOpt.Eligible, ParticipantType.Host, tbl, Mobj.SlotList[i], Mobj.Date, DataDB);
                    }
                    if (this.AttendeeConfig == UsersType.Users && AttendeeUserIdsCount >= Mobj.MinAttendee && AttendeeUserIdsCount == Mobj.SlotList[i].Attendees.Count && AttendeeUserIdsCount <= Mobj.MaxAttendee)
                    {
                        IsFixedAttendee = true;
                        query += MeetingSlotParticipantsQry(Mobj.SlotList[i].Attendees, usr, ParticipantOpt.Fixed, ParticipantType.Attendee, tbl, Mobj.SlotList[i], Mobj.Date, DataDB);
                    }
                    else if( this.AttendeeConfig == UsersType.Contact && AttendeeContactIdsCount >= Mobj.MinAttendee && AttendeeContactIdsCount == Mobj.SlotList[i].Attendees.Count && AttendeeContactIdsCount <= Mobj.MaxAttendee)
                    {
                        IsFixedAttendee = true;
                        query += AddPersons(Mobj.SlotList[i].Attendees, usr, ParticipantOpt.Fixed, ParticipantType.Attendee, tbl, Mobj.SlotList[i], Mobj.Date, DataDB);
                    } 
                    else if(this.AttendeeConfig == UsersType.Users && HostUserIdsCount < Mobj.MinAttendee)
                    {
                        throw new FormException("Schedule Meeting Failed : Minimum Attendee(s) Required");
                    }
                    else if(this.AttendeeConfig == UsersType.Contact && AttendeeContactIdsCount < Mobj.MinAttendee)
                    {
                        throw new FormException("Schedule Meeting Failed : Minimum Attendee(s) Required");
                    }
                    else
                    {
                        query += MeetingSlotParticipantsQry(Mobj.SlotList[i].Attendees, usr, ParticipantOpt.Eligible, ParticipantType.Attendee, tbl, Mobj.SlotList[i], Mobj.Date, DataDB);
                    }
                    query += $@"update eb_meeting_slots set meeting_opts = {SetMeetingOpts(IsFixedHost, IsFixedAttendee)} where id = eb_currval('eb_meeting_slots_id_seq') ;";
                }
            }
            else if (Mobj.MeetingType == MeetingType.MultipleMeeting)
            {
                query += AddMeetingSchedule(Mobj, usr);
                for (i = 0; i < Mobj.SlotList.Count; i++)
                {
                    query += $@"
                insert into eb_meeting_slots (eb_meeting_schedule_id,meeting_date,time_from,time_to,eb_created_by,eb_created_at) values 
                (eb_currval('eb_meeting_schedule_id_seq'),'{Mobj.Date}','{Mobj.SlotList[i].TimeFrom}','{Mobj.SlotList[i].TimeTo}', {usr.UserId},now() );";

                    int HostUserIdsCount = Mobj.SlotList[i].Hosts.Where(Item => Item.Type == UsersType.Users).Count();
                    int AttendeeUserIdsCount = Mobj.SlotList[i].Attendees.Where(Item => Item.Type == UsersType.Users).Count();
                    if (HostUserIdsCount == Mobj.MaxHost && HostUserIdsCount == Mobj.SlotList[i].Hosts.Count)
                    {
                        query += MeetingSlotParticipantsQry(Mobj.SlotList[i].Hosts, usr, ParticipantOpt.Fixed, ParticipantType.Host, tbl, Mobj.SlotList[i], Mobj.Date, DataDB);
                    }
                    else
                    {
                        query += MeetingSlotParticipantsQry(Mobj.SlotList[i].Hosts, usr, ParticipantOpt.Eligible, ParticipantType.Host, tbl, Mobj.SlotList[i], Mobj.Date, DataDB);
                    }
                    if (AttendeeUserIdsCount == Mobj.MaxAttendee && AttendeeUserIdsCount == Mobj.SlotList[i].Attendees.Count)
                    {
                        query += MeetingSlotParticipantsQry(Mobj.SlotList[i].Attendees, usr, ParticipantOpt.Fixed, ParticipantType.Attendee, tbl, Mobj.SlotList[i], Mobj.Date, DataDB);
                    }
                    else
                    {
                        query += MeetingSlotParticipantsQry(Mobj.SlotList[i].Attendees, usr, ParticipantOpt.Eligible, ParticipantType.Attendee, tbl, Mobj.SlotList[i], Mobj.Date, DataDB);
                    }
                }
            }
            else if (Mobj.MeetingType == MeetingType.AdvancedMeeting)
            {

            }
            //else if (Mobj.MeetingOpts == MeetingOptions.F_H_F_A && Mobj.IsMultipleMeeting == "F")
            //{
            //    query += AddMeetingSchedule(Mobj, usr);
            //    query += $@"
            //insert into eb_meeting_slots (eb_meeting_schedule_id,meeting_date,time_from,time_to,eb_created_by,eb_created_at) values 
            //(eb_currval('eb_meeting_schedule_id_seq'),'{Mobj.Date}','{Mobj.TimeFrom}','{Mobj.TimeTo}', {usr.UserId},now() );
            //insert into eb_my_actions (user_ids,from_datetime,form_ref_id,form_data_id,description,my_action_type , eb_meeting_slots_id, 
            //is_completed,eb_del) values('{Mobj.Host}',NOW(),@refid, eb_currval('{tbl}_id_seq'), 'Meeting Request',
            //'{MyActionTypes.Meeting}',eb_currval('eb_meeting_slots_id_seq') , 'F','F');  
            //insert into eb_my_actions (user_ids,from_datetime,form_ref_id,form_data_id,description,my_action_type , eb_meeting_slots_id, 
            //is_completed,eb_del) values('{Mobj.Attendee}',NOW(),@refid, eb_currval('{tbl}_id_seq'), 'Meeting Request',
            //'{MyActionTypes.Meeting}',eb_currval('eb_meeting_slots_id_seq') , 'F','F');
            // ";
            //    for (i = 0; i < Host.Length; i++)
            //    {
            //        query += $@"
            //         insert into eb_meeting_slot_participants(user_id, confirmation, eb_meeting_schedule_id, approved_slot_id, name, email, type_of_user, participant_type) 
            //         values ({Host[i]}, 2, eb_currval('eb_meeting_schedule_id_seq'), eb_currval('eb_meeting_slots_id_seq'), '', '', 1, 1);
            //        ";
            //    }
            //    for (i = 0; i < Attendee.Length; i++)
            //    {
            //        query += $@"
            //         insert into eb_meeting_slot_participants(user_id, confirmation, eb_meeting_schedule_id, approved_slot_id, name, email, type_of_user, participant_type) 
            //         values ({Attendee[i]}, 2, eb_currval('eb_meeting_schedule_id_seq'), eb_currval('eb_meeting_slots_id_seq'), '', '', 1, 2);
            //        ";
            //    }
            //}
            //else if (Mobj.MeetingOpts == MeetingOptions.F_H_F_A)
            //{
            //    query += AddMeetingSchedule(Mobj, usr);
            //    for (i = 0; i < Mobj.SlotList.Count; i++)
            //    {
            //        string[] Hosts = Mobj.SlotList[i].FixedHost.Split(',').Select(sValue => sValue.Trim()).ToArray();
            //        string[] Attendees = Mobj.SlotList[i].EligibleAttendees.Split(',').Select(sValue => sValue.Trim()).ToArray();
            //        query += $@"insert into eb_meeting_slots (eb_meeting_schedule_id,meeting_date,time_from,time_to,eb_created_by,eb_created_at) values 
            //        (eb_currval('eb_meeting_schedule_id_seq'),'{Mobj.Date}','{Mobj.SlotList[i].TimeFrom}:00','{Mobj.SlotList[i].TimeTo}:00', {usr.UserId},now() );";
            //        for (int j = 0; j < Hosts.Length; j++)
            //        {
            //            query += $@"
            //         insert into eb_meeting_slot_participants(user_id, confirmation, eb_meeting_schedule_id, approved_slot_id, name, email, type_of_user, participant_type,
            //           eb_created_at,eb_created_by) 
            //         values ({Hosts[j]}, 2, eb_currval('eb_meeting_schedule_id_seq'), eb_currval('eb_meeting_slots_id_seq'), '', '', 1, 1,now(),{usr.UserId});
            //        ";
            //        }
            //        for (i = 0; i < Attendees.Length; i++)
            //        {
            //            query += $@"insert into eb_meeting_slot_participants(user_id, confirmation, eb_meeting_schedule_id, approved_slot_id, name, email, type_of_user, participant_type,
            //        eb_created_at,eb_created_by) 
            //         values ({Attendees[i]}, 2, eb_currval('eb_meeting_schedule_id_seq'), eb_currval('eb_meeting_slots_id_seq'), '', '', 1, 2,now(),{usr.UserId});
            //        ";
            //        }
            //        query += $@" insert into eb_my_actions (user_ids,from_datetime,form_ref_id,form_data_id,description,my_action_type , eb_meeting_slots_id, 
            //        is_completed,eb_del) values('{Mobj.SlotList[i].FixedHost}',NOW(),@refid, eb_currval('{tbl}_id_seq'), 'Meeting Request',
            //        '{MyActionTypes.Meeting}',eb_currval('eb_meeting_slots_id_seq') , 'F','F');
            //        insert into eb_my_actions (user_ids,from_datetime,form_ref_id,form_data_id,description,my_action_type , eb_meeting_slots_id, 
            //        is_completed,eb_del) values('{Mobj.SlotList[i].FixedAttendee}',NOW(),@refid, eb_currval('{tbl}_id_seq'), 'Meeting Request',
            //        '{MyActionTypes.Meeting}',eb_currval('eb_meeting_slots_id_seq') , 'F','F');";
            //    }
            //}
            //else if (Mobj.MeetingOpts == MeetingOptions.F_H_E_A)
            //{
            //    query += AddMeetingSchedule(Mobj, usr);
            //    for (i = 0; i < Mobj.SlotList.Count; i++)
            //    {
            //        string[] Hosts = Mobj.SlotList[i].FixedHost.Split(',').Select(sValue => sValue.Trim()).ToArray();
            //        string[] Attendees = Mobj.SlotList[i].EligibleAttendees.Split(',').Select(sValue => sValue.Trim()).ToArray();
            //        query += $@"insert into eb_meeting_slots (eb_meeting_schedule_id,meeting_date,time_from,time_to,eb_created_by,eb_created_at) values 
            //        (eb_currval('eb_meeting_schedule_id_seq'),'{Mobj.Date}','{Mobj.SlotList[i].TimeFrom}:00','{Mobj.SlotList[i].TimeTo}:00', {usr.UserId},now() );";
            //        for (int j = 0; j < Hosts.Length; j++)
            //        {
            //            query += $@"
            //         insert into eb_meeting_slot_participants(user_id, confirmation, eb_meeting_schedule_id, approved_slot_id, name, email, type_of_user, participant_type,
            //           eb_created_at,eb_created_by) 
            //         values ({Hosts[j]}, 2, eb_currval('eb_meeting_schedule_id_seq'), eb_currval('eb_meeting_slots_id_seq'), '', '', 1, 1,now(),{usr.UserId});
            //        ";
            //        }
            //        query += $@" insert into eb_my_actions (user_ids,from_datetime,form_ref_id,form_data_id,description,my_action_type , eb_meeting_slots_id, 
            //        is_completed,eb_del) values('{Mobj.SlotList[i].FixedHost}',NOW(),@refid, eb_currval('{tbl}_id_seq'), 'Meeting Request',
            //        '{MyActionTypes.Meeting}',eb_currval('eb_meeting_slots_id_seq') , 'F','F');";
            //    }
            //    query += $@"
            //        insert into eb_meeting_scheduled_participants (user_ids,role_ids,eb_meeting_schedule_id,participant_type,type_of_user,
            //  eb_created_at,eb_created_by)values('{ Mobj.EligibleAttendees}','',eb_currval('eb_meeting_schedule_id_seq') , 2 ,1,now(),{usr.UserId});
            //        ";
            //    query += $@"insert into eb_my_actions (user_ids,from_datetime,form_ref_id,form_data_id,description,my_action_type , eb_meeting_schedule_id, 
            //    is_completed,eb_del) values('{Mobj.EligibleAttendees}',NOW(),@refid, eb_currval('{tbl}_id_seq'), 'Meeting Request',
            //    '{MyActionTypes.Meeting}',eb_currval('eb_meeting_schedule_id_seq') , 'F','F');";
            //}
            //else if (Mobj.MeetingOpts == MeetingOptions.E_H_F_A)
            //{
            //    query += AddMeetingSchedule(Mobj, usr);
            //    for (i = 0; i < Mobj.SlotList.Count; i++)
            //    {
            //        string[] Hosts = Mobj.SlotList[i].FixedHost.Split(',').Select(sValue => sValue.Trim()).ToArray();
            //        string[] Attendees = Mobj.SlotList[i].EligibleAttendees.Split(',').Select(sValue => sValue.Trim()).ToArray();
            //        query += $@"insert into eb_meeting_slots (eb_meeting_schedule_id,meeting_date,time_from,time_to,eb_created_by,eb_created_at) values 
            //        (eb_currval('eb_meeting_schedule_id_seq'),'{Mobj.Date}','{Mobj.SlotList[i].TimeFrom}:00','{Mobj.SlotList[i].TimeTo}:00', {usr.UserId},now() );";
            //        for (int j = 0; j < Attendees.Length; j++)
            //        {
            //            query += $@"
            //         insert into eb_meeting_slot_participants(user_id, confirmation, eb_meeting_schedule_id, approved_slot_id, name, email, type_of_user, participant_type,
            //           eb_created_at,eb_created_by) 
            //         values ({Attendees[j]}, 2, eb_currval('eb_meeting_schedule_id_seq'), eb_currval('eb_meeting_slots_id_seq'), '', '', 1, 2,now(),{usr.UserId});
            //        ";
            //        }
            //        query += $@" insert into eb_my_actions (user_ids,from_datetime,form_ref_id,form_data_id,description,my_action_type , eb_meeting_slots_id, 
            //        is_completed,eb_del) values('{Mobj.SlotList[i].FixedAttendee}',NOW(),@refid, eb_currval('{tbl}_id_seq'), 'Meeting Request',
            //        '{MyActionTypes.Meeting}',eb_currval('eb_meeting_slots_id_seq') , 'F','F');";
            //    }
            //    query += $@"
            //        insert into eb_meeting_scheduled_participants (user_ids,role_ids,eb_meeting_schedule_id,participant_type,type_of_user,
            //  eb_created_at,eb_created_by)values('{ Mobj.EligibleHosts}','',eb_currval('eb_meeting_schedule_id_seq') , 1 ,1,now(),{usr.UserId});
            //        ";
            //    query += $@" insert into eb_my_actions (user_ids,from_datetime,form_ref_id,form_data_id,description,my_action_type , eb_meeting_schedule_id, 
            //        is_completed,eb_del) values('{Mobj.EligibleHosts}',NOW(),@refid, eb_currval('{tbl}_id_seq'), 'Meeting Request',
            //        '{MyActionTypes.Meeting}',eb_currval('eb_meeting_schedule_id_seq') , 'F','F');";

            //}
            //else if (Mobj.MeetingOpts == MeetingOptions.E_H_E_A)
            //{
            //    query += AddMeetingSchedule(Mobj, usr);
            //    for (i = 0; i < Mobj.SlotList.Count; i++)
            //    {
            //        query += $@"insert into eb_meeting_slots (eb_meeting_schedule_id,meeting_date,time_from,time_to,eb_created_by,eb_created_at) values 
            //        (eb_currval('eb_meeting_schedule_id_seq'),'{Mobj.Date}','{Mobj.SlotList[i].TimeFrom}:00','{Mobj.SlotList[i].TimeTo}:00', {usr.UserId},now() );";
            //    }
            //    query += $@"
            //        insert into eb_meeting_scheduled_participants (user_ids,role_ids,eb_meeting_schedule_id,participant_type,type_of_user,
            //  eb_created_at,eb_created_by)values('{ Mobj.EligibleHosts}','',eb_currval('eb_meeting_schedule_id_seq') , 1 ,1,now(),{usr.UserId});
            //        insert into eb_meeting_scheduled_participants (user_ids,role_ids,eb_meeting_schedule_id,participant_type,type_of_user,
            //  eb_created_at,eb_created_by)values('{ Mobj.EligibleAttendees}','',eb_currval('eb_meeting_schedule_id_seq') , 2 ,1,now(),{usr.UserId});
            //        ";
            //    query += $@" insert into eb_my_actions (user_ids,from_datetime,form_ref_id,form_data_id,description,my_action_type , eb_meeting_schedule_id, 
            //        is_completed,eb_del) values('{Mobj.SlotList[0].EligibleHosts}',NOW(),@refid, eb_currval('{tbl}_id_seq'), 'Meeting Request',
            //        '{MyActionTypes.Meeting}',eb_currval('eb_meeting_schedule_id_seq') , 'F','F');
            //        insert into eb_my_actions (user_ids,from_datetime,form_ref_id,form_data_id,description,my_action_type , eb_meeting_schedule_id, 
            //        is_completed,eb_del) values('{Mobj.EligibleAttendees}',NOW(),@refid, eb_currval('{tbl}_id_seq'), 'Meeting Request',
            //        '{MyActionTypes.Meeting}',eb_currval('eb_meeting_schedule_id_seq') , 'F','F');";
            //}

            _extqry += query;
            return true;
        }

        public string AddPersons(List<Participants> Participants, User usr, ParticipantOpt Opt, ParticipantType ParticipantType, string tbl, Slots Mobj, string Date, IDatabase DataDB)
        {
            List<Participants> ParticipantsList = new List<Participants>();
            String _query = string.Format(this.ValidateQuery, Mobj.TimeFrom, Mobj.TimeTo, Date);
            EbDataSet ds = DataDB.DoQueries(_query);
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                ParticipantsList.Add(new Participants()
                {
                    Id = Convert.ToInt32(ds.Tables[0].Rows[i]["user_id"]),
                });
            }
            string ExceptUserNames = "";
            string query = "";
            for (int j = 0; j < Participants.Count; j++)
            {
                if (Participants[j].Type == UsersType.Contact)
                {
                    for (int k = 0; k < ParticipantsList.Count; k++)
                    {
                        if (ParticipantsList[k].Id == Participants[j].Id && ParticipantsList[k].Name == Participants[j].Name)
                            ExceptUserNames += Participants[j].Name + " ";
                    }
                    query += $@"
                            insert into eb_meeting_slot_participants(user_id, confirmation, eb_meeting_schedule_id, approved_slot_id, name, email, type_of_user, participant_type,
                            eb_created_at,eb_created_by) 
                            values ({Participants[j].Id}, 1, eb_currval('eb_meeting_schedule_id_seq'), eb_currval('eb_meeting_slots_id_seq'), '', '', 2, {(int)ParticipantType},now(),{usr.UserId});
                           ";
                }
            }
            return query;
        }

        public string MeetingSlotParticipantsQry(List<Participants> Participants, User usr, ParticipantOpt Opt, ParticipantType ParticipantType, string tbl, Slots Mobj, string Date, IDatabase DataDB)
        {
            List<Participants> ParticipantsList = new List<Participants>();
            String _query = string.Format(this.ValidateQuery, Mobj.TimeFrom, Mobj.TimeTo, Date);
            EbDataSet ds = DataDB.DoQueries(_query);
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                ParticipantsList.Add(new Participants()
                {
                    Id = Convert.ToInt32(ds.Tables[0].Rows[i]["user_id"]),
                });
            }
            string ExceptUserNames = "";
            string query = "";
            string userids = "";
            string Roles = "";
            string Contacts = "";
            int UserGroup = 0;
            if (Opt == ParticipantOpt.Fixed)
            {
                //string Roles = "";
                for (int j = 0; j < Participants.Count; j++)
                {
                    if (Participants[j].Type == UsersType.Users)
                    {
                        for (int k = 0; k < ParticipantsList.Count; k++)
                        {
                            if (ParticipantsList[k].Id == Participants[j].Id)
                                ExceptUserNames += Participants[j].Name + " ";
                        }
                        userids += Participants[j].Id + ",";
                        query += $@"
                            insert into eb_meeting_slot_participants(user_id, confirmation, eb_meeting_schedule_id, approved_slot_id, name, email, type_of_user, participant_type,
                            eb_created_at,eb_created_by) 
                            values ({Participants[j].Id}, 2, eb_currval('eb_meeting_schedule_id_seq'), eb_currval('eb_meeting_slots_id_seq'), '', '', 1, {(int)ParticipantType},now(),{usr.UserId});
                           ";
                    }
                }
                if (userids != "")
                    query += $@" insert into eb_my_actions (user_ids,from_datetime,form_ref_id,form_data_id,description,my_action_type , eb_meeting_slots_id, 
                is_completed,eb_del) values('{userids}',NOW(),@refid, eb_currval('{tbl}_id_seq'), 'Meeting Request',
                     '{MyActionTypes.Meeting}',eb_currval('eb_meeting_slots_id_seq') , 'F','F'); ";
                ////if (Roles != "")
                ////    query += $@"insert into eb_my_actions (role_ids,from_datetime,form_ref_id,form_data_id,description,my_action_type , eb_meeting_slots_id, 
                ////    is_completed,eb_del) values('{Roles}',NOW(),@refid, eb_currval('{tbl}_id_seq'), 'Meeting Request',
                ////     '{MyActionTypes.Meeting}',eb_currval('eb_meeting_slots_id_seq') , 'F','F');";
            }

            if (Opt == ParticipantOpt.Eligible)
            {

                for (int j = 0; j < Participants.Count; j++)
                {
                    if (Participants[j].Type == UsersType.Users)
                    {
                        for (int k = 0; k < ParticipantsList.Count; k++)
                        {
                            if (ParticipantsList[k].Id == Participants[j].Id)
                                ExceptUserNames += Participants[j].Name + " ";
                        }
                        userids += Participants[j].Id + ",";
                    }
                    else if (Participants[j].Type == UsersType.Role)
                    {
                        Roles += Participants[j].Id + ",";
                    }
                    else if (Participants[j].Type == UsersType.Contact)
                    {
                        Contacts += Participants[j].Id;
                    }
                    else
                    {
                        UserGroup = Participants[j].Id;
                    }
                }
                if (Contacts != "")
                    query += $@"
                    insert into eb_meeting_scheduled_participants (user_ids,role_ids,eb_meeting_schedule_id,participant_type,type_of_user,
                    eb_created_at,eb_created_by)values('{Contacts}','{Roles}',eb_currval('eb_meeting_schedule_id_seq') , {(int)ParticipantType} ,2,now(),{usr.UserId}); ";
                else
                    query += $@"
                    insert into eb_meeting_scheduled_participants (user_ids,role_ids,eb_meeting_schedule_id,participant_type,type_of_user,
                    eb_created_at,eb_created_by)values('{userids}','{Roles}',eb_currval('eb_meeting_schedule_id_seq') , {(int)ParticipantType} ,1,now(),{usr.UserId});";

                if (userids != "")
                    query += $@" insert into eb_my_actions (user_ids,from_datetime,form_ref_id,form_data_id,description,my_action_type , eb_meeting_schedule_id, 
                    is_completed,eb_del) values('{userids}',NOW(),@refid, eb_currval('{tbl}_id_seq'), 'Meeting Request',
                    '{MyActionTypes.Meeting}',eb_currval('eb_meeting_schedule_id_seq') , 'F','F');";
                if (Roles != "")
                    query += $@" insert into eb_my_actions (role_ids,from_datetime,form_ref_id,form_data_id,description,my_action_type , eb_meeting_schedule_id, 
                    is_completed,eb_del) values('{Roles}',NOW(),@refid, eb_currval('{tbl}_id_seq'), 'Meeting Request',
                    '{MyActionTypes.Meeting}',eb_currval('eb_meeting_schedule_id_seq') , 'F','F');";
                if (UserGroup > 0)
                    query += $@" insert into eb_my_actions (user_group,from_datetime,form_ref_id,form_data_id,description,my_action_type , eb_meeting_schedule_id, 
                    is_completed,eb_del) values('{UserGroup}',NOW(),@refid, eb_currval('{tbl}_id_seq'), 'Meeting Request',
                    '{MyActionTypes.Meeting}',eb_currval('eb_meeting_schedule_id_seq') , 'F','F');";
            }
            if (ExceptUserNames != "")
                throw new FormException("Schedule Meeting Failed :" + ExceptUserNames + "(" + ParticipantType + ")" + "have another Meeting");
            return query;
        }

        //public string MeetingEligibleParticipantsQry(List<Participants> Participants, User usr, ParticipantOpt Opt, ParticipantType ParticipantType, string tbl)
        //{
        //    string str = "";

        //    str += $@"
        //    //        insert into eb_meeting_scheduled_participants (user_ids,role_ids,eb_meeting_schedule_id,participant_type,type_of_user,
        //    //  eb_created_at,eb_created_by)values('{ Mobj.EligibleAttendees}','',eb_currval('eb_meeting_schedule_id_seq') , 2 ,1,now(),{usr.UserId});
        //    //        ";
        //    return str;
        //}

        public int SetMeetingOpts(bool Host, bool Attendee)
        {
            int val = 0;
            if (Host && Attendee)
                val = (int)MeetingOptions.F_H_F_A;
            else if (Host && !Attendee)
                val = (int)MeetingOptions.F_H_E_A;
            else if (!Host && Attendee)
                val = (int)MeetingOptions.E_H_F_A;
            else if (!Host && !Attendee)
                val = (int)MeetingOptions.E_H_E_A;
            return val;
        }
        public string AddMeetingSchedule(MeetingSchedule Mobj, User usr)
        {
            string qry = "";
            if (Mobj.MeetingType == MeetingType.SingleMeeting || Mobj.MeetingType == MeetingType.MultipleMeeting)
            {
                qry = $@"insert into eb_meeting_schedule (title,description,meeting_date,time_from,time_to,duration,is_recuring,is_multiple,venue,
			integration,max_hosts,max_attendees,no_of_attendee,no_of_hosts,eb_created_by,eb_created_at,meeting_opts)
			values('{Mobj.Title}','{Mobj.Description}','{Mobj.Date}','{Mobj.SlotList[0].TimeFrom}:00','{Mobj.SlotList[Mobj.SlotList.Count - 1].TimeTo}:00', 0,'{Mobj.IsRecuring}',
            '{Mobj.IsMultipleMeeting}','{Mobj.Location}','',{Mobj.MaxHost},{Mobj.MaxAttendee},{Mobj.MinAttendee},{Mobj.MinHost},{usr.UserId},now(),{(int)Mobj.MeetingType});";

            }
            else if (Mobj.MeetingType == MeetingType.AdvancedMeeting)
            {
                qry = $@"insert into eb_meeting_schedule (title,description,meeting_date,time_from,time_to,duration,is_recuring,is_multiple,venue,
			integration,max_hosts,max_attendees,no_of_attendee,no_of_hosts,eb_created_by,eb_created_at,meeting_opts)
			values('{Mobj.Title}','{Mobj.Description}','{Mobj.Date}','{Mobj.TimeFrom}:00','{Mobj.TimeTo}:00', 0,'{Mobj.IsRecuring}',
            '{Mobj.IsMultipleMeeting}','{Mobj.Location}','',{Mobj.MaxHost},{Mobj.MaxAttendee},{Mobj.MinAttendee},{Mobj.MinHost},{usr.UserId},now(),{(int)Mobj.MeetingType});";
            }
            return qry;
        }
        public string AddMeetingSlots(MeetingSchedule Mobj, User usr)
        {
            string qry = $@" 
            insert into eb_meeting_slots (eb_meeting_schedule_id,meeting_date,time_from,time_to,eb_created_by,eb_created_at) values 
            (eb_currval('eb_meeting_schedule_id_seq'),'{Mobj.Date}','{Mobj.TimeFrom}','{Mobj.TimeTo}', {usr.UserId},now() );
            ";
            return qry;
        }
        //public string AddMyAction(MeetingSchedule Mobj, User usr, string tbl)
        //{
        //    string qry = $@" insert into eb_my_actions (user_ids,from_datetime,form_ref_id,form_data_id,description,my_action_type , eb_meeting_slots_id, 
        //    is_completed,eb_del) values('{Mobj.EligibleHosts}',NOW(),@refid, eb_currval('{tbl}_id_seq'), 'Meeting Request',
        //    '{MyActionTypes.Meeting}',eb_currval('eb_meeting_slots_id_seq') , 'F','F');  
        //    insert into eb_my_actions (user_ids,from_datetime,form_ref_id,form_data_id,description,my_action_type , eb_meeting_slots_id, 
        //    is_completed,eb_del) values('{Mobj.EligibleAttendees}',NOW(),@refid, eb_currval('{tbl}_id_seq'), 'Meeting Request',
        //    '{MyActionTypes.Meeting}',eb_currval('eb_meeting_slots_id_seq') , 'F','F');";
        //    return qry;
        //}
        public string AddEligibleParticipants(MeetingSchedule Mobj, User usr)
        {
            string qry = $@"
            ";
            return qry;
        }
    }
 
    public class MeetingSchedule
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public string Integration { get; set; }
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
        public MeetingType MeetingType { get; set; }
        public List<Slots> SlotList { get; set; }
        public MeetingSchedule()
        {
            this.SlotList = new List<Slots>();
        }
        public int UserIdCount(List<Participants> Participants, UsersType Type)
        {
            var count = Participants.Where(item => item.Type == Type).Count();
            return count;
        }
    }
    public class Slots
    {
        public int Position { get; set; }
        public string TimeFrom { get; set; }
        public string TimeTo { get; set; }
        public string EligibleHosts { get; set; }
        public string EligibleAttendees { get; set; }
        public string FixedHost { get; set; }
        public string FixedAttendee { get; set; }
        public List<Participants> Hosts { get; set; }
        public List<Participants> Attendees { get; set; }
    }
    public class Participants
    {
        public UsersType Type { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
    }
    public enum UsersType
    {
        Role = 1,
        UserGroup,
        Users,
        Contact,
    }
    public enum MeetingOptions
    {
        F_H_F_A = 1, /*fixed host and fixed attendee*/
        F_H_E_A = 2, /*fixed host and eligible attendee*/
        E_H_F_A = 3, /*eligible host and fixed attendee*/
        E_H_E_A = 4, /*eligible host and eligible attendee*/
    }
    public enum ParticipantOpt
    {
        Fixed = 1,
        Eligible = 2,
    }
    public enum QueryOpts
    {
        insert = 1,
        update = 2
    }
    public enum MeetingType
    {
        SingleMeeting = 1,
        MultipleMeeting = 2,
        AdvancedMeeting = 3
    }

    public class ParticipantsListResponse
    {
        public List<Participants> HostParticipantsList { get; set; }
        public List<Participants> AttendeeParticipantsList { get; set; }

        public ParticipantsListResponse()
        {
            this.HostParticipantsList = new List<Participants>();
            this.AttendeeParticipantsList = new List<Participants>();
        }
    }
    public class ParticipantsListRequest
    {
        public List<MeetingSuggestion> MeetingConfig { get; set; }
        public ParticipantsListRequest()
        {
            this.MeetingConfig = new List<MeetingSuggestion>();
        }
    }
    public class ParticipantsListAjaxResponse
    {
        public List<Participants> HostParticipantsList { get; set; }
        public List<Participants> AttendeeParticipantsList { get; set; }

        public ParticipantsListAjaxResponse()
        {
            this.HostParticipantsList = new List<Participants>();
            this.AttendeeParticipantsList = new List<Participants>();
        }
    }
    public class ParticipantsListAjaxRequest
    {
        public List<MeetingSuggestion> MeetingConfig { get; set; }
        public string TimeFrom { get; set; }
        public string TimeTo { get; set; }
        public ParticipantsListAjaxRequest()
        {
            this.MeetingConfig = new List<MeetingSuggestion>();
        }
    }
    public enum MeetingEntityTypes
    {
        Role = 1,
        UserGroup,
        Users
    }
    public class MeetingSuggestion
    {
        public UsersType MeetingConfig { get; set; }
        public List<Int32> MeetingRoles { get; set; }
        public int MeetingUserGroup { get; set; }
        public string ContactFilter { get; set; }
        public EbScript MeetingUsers { get; set; }
        public EbScript Contacts { get; set; }
    }
}
