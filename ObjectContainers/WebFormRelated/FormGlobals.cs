using ExpressBase.Common;
using ExpressBase.Common.Objects;
using ExpressBase.Common.Structures;
using ExpressBase.Security;
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

        public FG_Root(FG_WebForm fG_WebForm) 
        {
            this.form = fG_WebForm;
        }

        public FG_Root(FG_WebForm fG_WebForm, User User)
        {
            this.form = fG_WebForm;
            this.user = User;
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
                if (result == null && this.Review != null && name == this.Review.Name)
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

        public FG_Review(EbReview CtrlObj, SingleTable Table)
        {
            this.Name = CtrlObj.Name;
            this.CtrlObj = CtrlObj;
            this.Table = Table;
        }

        public string getCurrentAction()
        {
            string actionId = null;
            SingleRow Row = this.Table.Find(e => e.RowId <= 0);
            if (Row != null)
            {
                EbReviewStage curStage = (EbReviewStage)this.CtrlObj.FormStages.Find(e => (e as EbReviewStage).EbSid == Convert.ToString(Row["stage_unique_id"]));
                if (curStage != null)
                {                    
                    EbReviewAction curAction = (EbReviewAction)curStage.StageActions.Find(e => (e as EbReviewAction).EbSid == Convert.ToString(Row["action_unique_id"]));
                    if (curAction != null)
                        actionId = curAction.Name;
                }
            }
            return actionId;
        }

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

        public void approve() 
        {
            this.CtrlObj.ReviewStatus = "Appoved";
        }

        public void reject() 
        {
            this.CtrlObj.ReviewStatus = "Rejected";
        }

        public void setNextStageDataEditable() { }
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