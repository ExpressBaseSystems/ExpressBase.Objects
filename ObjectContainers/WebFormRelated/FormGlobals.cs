using ExpressBase.Common;
using ExpressBase.Common.Data;
using ExpressBase.Common.Extensions;
using ExpressBase.Common.Objects;
using ExpressBase.Common.Structures;
using ExpressBase.Objects.ServiceStack_Artifacts;
using ExpressBase.Security;
using Newtonsoft.Json;
using ServiceStack;
using ServiceStack.Text;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;

namespace ExpressBase.Objects.WebFormRelated
{
    [RuntimeSerializable]
    public class FG_Root
    {
        public dynamic form { get; set; }

        public User user { get; private set; }

        public FG_System system { get; private set; }

        public FG_Root(FG_WebForm fG_WebForm) 
        {
            this.form = fG_WebForm;
        }

        public FG_Root(FG_WebForm fG_WebForm, EbWebForm ebWebForm, Service service)
        {
            this.form = fG_WebForm;
            this.user = ebWebForm.UserObj;
            this.system = new FG_System(ebWebForm, service);
        }
    }

    public class FG_System
    {
        private Service Service { get; set; }
        
        private EbWebForm WebForm { get; set; }

        public FG_System(EbWebForm ebWebForm, Service service)
        {
            this.Service = service;
            this.WebForm = WebForm;
        }

        public void sendNotificationByUserId(int userId, string title = null)
        {
            try
            {
                if (string.IsNullOrEmpty(this.WebForm.RefId) || this.WebForm.TableRowId <= 0)
                    return;
                List<Param> p = new List<Param> { { new Param { Name = "id", Type = ((int)EbDbTypes.Int32).ToString(), Value = this.WebForm.TableRowId.ToString() } } };
                string pp = JsonConvert.SerializeObject(p).ToBase64();
                NotifyByUserIDResponse result = Service.Gateway.Send<NotifyByUserIDResponse>(new NotifyByUserIDRequest
                {
                    Link = $"/WebForm/Index?refId={this.WebForm.RefId}&_params={pp}&_mode=1",
                    Title = title ?? this.WebForm.DisplayName + " notification",
                    UsersID = userId
                });

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message + e.StackTrace);
            }
        }
    }

    [RuntimeSerializable]
    public class FG_WebForm : DynamicObject
    {
        public FG_Row FlatCtrls { get; set; }
        
        public List<FG_DataGrid> DataGrids { get; set; }

        public FG_Review Review { get; set; }

        public FG_WebForm() 
        {
            this.FlatCtrls = new FG_Row();
            this.DataGrids = new List<FG_DataGrid>();
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            string name = binder.Name;            
            result = this.FlatCtrls[name];
            if (result == null)
            {
                result = this.DataGrids.Find(e => e.Name == name);
                if (result == null && this.Review != null && name == "review")
                    result = this.Review;
            }
            if (result == null)
                throw new NullReferenceException(name + " is not a control");
            return true;
        }

    }

    public class FG_DataGrid
    {
        public string Name { get; set; }

        private EbDataGrid CtrlObj { get; set; }

        private SingleTable Table { get; set; }

        public FG_DataGrid(EbDataGrid CtrlObj, SingleTable Table)
        {
            this.Name = CtrlObj.Name;
            this.CtrlObj = CtrlObj;
            this.Table = Table;
        }
    }

    public class FG_Review
    {
        public string Name { get; set; }

        private EbReview CtrlObj { get; set; }

        private SingleTable Table { get; set; }

        public Dictionary<string, FG_Review_Stage> stages { get; private set; }

        public FG_Review_Stage currentStage { get; private set; }

        public FG_Review(EbReview CtrlObj, SingleTable Table)
        {
            this.Name = CtrlObj.Name;
            this.CtrlObj = CtrlObj;
            this.Table = Table;

            SingleRow _Row = null;
            foreach(SingleRow Row in this.Table)
            {
                if (Row.RowId <= 0 && Row.Columns.Count > 0)
                {
                    _Row = Row;
                    break;
                }
            }

            this.stages = new Dictionary<string, FG_Review_Stage>();

            foreach(ReviewStageAbstract stage in this.CtrlObj.FormStages)
            {
                EbReviewStage _eb_stage = stage as EbReviewStage;
                List<FG_Review_Action> fg_actions = new List<FG_Review_Action>();
                foreach (ReviewActionAbstract action in _eb_stage.StageActions)
                {
                    fg_actions.Add(new FG_Review_Action((action as EbReviewAction).Name));
                }
                if (_Row != null && Convert.ToString(_Row["stage_unique_id"]) == _eb_stage.EbSid)
                {
                    EbReviewAction eb_curAct = (EbReviewAction)_eb_stage.StageActions.Find(e => (e as EbReviewAction).EbSid == Convert.ToString(_Row["action_unique_id"]));
                    FG_Review_Action fg_curAct = null;
                    if (eb_curAct != null)
                    {
                        fg_curAct = fg_actions.Find(e => e.name == eb_curAct.Name);
                    }
                    this.stages.Add(_eb_stage.Name, new FG_Review_Stage(_eb_stage.Name, fg_actions, fg_curAct));
                    this.currentStage = this.stages[_eb_stage.Name];
                }
                else
                {
                    this.stages.Add(_eb_stage.Name, new FG_Review_Stage(_eb_stage.Name, fg_actions, null));
                }
            }
        }

        public void complete()
        {
            this.CtrlObj.ReviewStatus = "complete";
        }

        public void abandon()
        {
            this.CtrlObj.ReviewStatus = "abandon";
        }

        public void setCurrentStageDataEditable() 
        {
            EbReviewStage curS = (EbReviewStage)this.CtrlObj.FormStages.Find(e => (e as EbReviewStage).Name == this.currentStage.name);
            if (curS != null)
                curS.IsFormEditable = true;
        }

        //public string getCurrentAction()
        //{
        //    string actionId = null;
        //    SingleRow Row = this.Table.Find(e => e.RowId <= 0);
        //    if (Row != null)
        //    {
        //        EbReviewStage curStage = (EbReviewStage)this.CtrlObj.FormStages.Find(e => (e as EbReviewStage).EbSid == Convert.ToString(Row["stage_unique_id"]));
        //        if (curStage != null)
        //        {                    
        //            EbReviewAction curAction = (EbReviewAction)curStage.StageActions.Find(e => (e as EbReviewAction).EbSid == Convert.ToString(Row["action_unique_id"]));
        //            if (curAction != null)
        //                actionId = curAction.Name;
        //        }
        //    }
        //    return actionId;
        //}
        //public string getCurrentStage()//not usable
        //{
        //    string stageId = null;
        //    SingleRow Row = this.Table.Find(e => e.RowId <= 0);
        //    if (Row != null)
        //    {
        //        EbReviewStage curStage = (EbReviewStage)this.CtrlObj.FormStages.Find(e => (e as EbReviewStage).EbSid == Convert.ToString(Row["stage_unique_id"]));
        //        if (curStage != null)
        //            stageId = curStage.Name;
        //    }
        //    return stageId;
        //}
    }

    public class FG_Review_Stage
    {
        public string name { get; private set; }

        public List<FG_Review_Action> actions { get; private set; }

        public FG_Review_Action currentAction { get; private set; }

        public FG_Review_Stage(string Name, List<FG_Review_Action> Actions, FG_Review_Action currentAction)
        {
            this.name = Name;
            this.actions = Actions;
            this.currentAction = currentAction;
        }
    }

    public class FG_Review_Action
    {
        public string name { get; private set; }

        public FG_Review_Action(string Name)
        {
            this.name = Name;
        }
    }

    [RuntimeSerializable]
    public class FG_Row : DynamicObject
    {
        public List<FG_Control> Controls { get; set; }

        public FG_Row()
        {
            this.Controls = new List<FG_Control>();
        }

        public FG_Control this[string name]
        {
            get
            {
                FG_Control ctrl = this.Controls.Find(e => e.Name.Equals(name));
                if (ctrl == null)
                    Console.WriteLine("Null ref for control - form globals");
                return ctrl;
            }
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            result = this.Controls.Find(e => e.Name.Equals(binder.Name));
            if (result == null)
                throw new Exception(binder.Name + " is not a control");
            return true;
        }
    }

    public class FG_Control
    {
        public string Name { get; private set; }

        public EbDbTypes EbDbType { get; private set; }

        public object Value { get; private set; }

        private EbControl Control { get; set; }

        public FG_Control(EbControl Control, object Value)
        {
            this.Control = Control;
            this.Name = Control.Name;
            this.EbDbType = Control.EbDbType;
            this.Value = Value;
        }

        public object getValue()
        {
            return this.Value;
        }
    }
}