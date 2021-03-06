﻿using ExpressBase.Common;
using ExpressBase.Common.Data;
using ExpressBase.Common.Extensions;
using ExpressBase.Common.LocationNSolution;
using ExpressBase.Common.Objects;
using ExpressBase.Common.Structures;
using ExpressBase.CoreBase.Globals;
using ExpressBase.Objects.Objects;
using ExpressBase.Objects.ServiceStack_Artifacts;
using Newtonsoft.Json;
using ServiceStack;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Globalization;

namespace ExpressBase.Objects.WebFormRelated
{
    public class DelegateTest
    {
        private IDatabase DataDB { get; set; }

        private DbConnection DbCon { get; set; }

        public DelegateTest(IDatabase DataDB, DbConnection DbCon)
        {
            this.DataDB = DataDB;
            this.DbCon = DbCon;
        }

        public object ExecuteScalar(string Query)
        {
            object val = null;
            if (!string.IsNullOrEmpty(Query))
            {
                //try
                //{
                EbDataTable dt;
                if (this.DbCon == null)
                    dt = DataDB.DoQuery(Query);
                else
                    dt = DataDB.DoQuery(this.DbCon, Query);
                if (dt.Rows.Count > 0 && dt.Rows[0].Count > 0)
                    val = dt.Rows[0][0];
                else
                    Console.WriteLine("0 rows returned: DelegateTest->ExecuteScalar");
                //}
                //catch (Exception ex)
                //{
                //    Console.WriteLine("Exception in DelegateTest->ExecuteScalar: " + ex.Message + "\nQuery: " + Query);
                //}
            }
            else
            {
                Console.WriteLine("Null Query in DelegateTest->ExecuteScalar");
            }
            return val;
        }
    }

    public static class GlobalsGenerator
    {
        public static FormAsGlobal GetFormAsFlatGlobal(EbWebForm _this, WebformData _formdata)
        {
            Dictionary<string, string> grid = new Dictionary<string, string>();
            EbControl[] Allctrls = _this.Controls.FlattenAllEbControls();
            for (int i = 0; i < Allctrls.Length; i++)
            {
                if (Allctrls[i] is EbDataGrid)
                {
                    grid.Add((Allctrls[i] as EbDataGrid).TableName, (Allctrls[i] as EbDataGrid).Name);
                }
            }

            FormAsGlobal _globals = new FormAsGlobal { Name = _this.Name };
            ListNTV listNTV = new ListNTV();
            try
            {
                foreach (KeyValuePair<string, SingleTable> item in _formdata.MultipleTables)
                {
                    if (grid.ContainsKey(item.Key))
                    {
                        FormAsGlobal _grid = new FormAsGlobal { Name = grid[item.Key] };
                        for (int j = 0; j < item.Value.Count; j++)
                        {
                            ListNTV _gridline = new ListNTV();
                            foreach (SingleColumn col in item.Value[j].Columns)
                            {
                                if (col.Name != "id" && col.Name != "eb_row_num")
                                {
                                    NTV n = GetNtvFromFormData(_formdata, item.Key, j, col.Name);
                                    if (n != null)
                                        _gridline.Columns.Add(n);
                                }
                            }
                            _grid.Add(_gridline);
                        }
                        _globals.AddContainer(_grid);
                    }
                    else
                    {
                        foreach (SingleColumn col in item.Value[0].Columns)
                        {
                            if (!(col.Name == "id" && item.Key != _formdata.MasterTable) && item.Value.Count == 1)
                            {
                                NTV n = GetNtvFromFormData(_formdata, item.Key, 0, col.Name);
                                if (n != null)
                                    listNTV.Columns.Add(n);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception in GetFormAsFlatGlobal. Message : " + ex.Message);
                Console.WriteLine(ex.StackTrace);
            }

            _globals.Add(listNTV);
            return _globals;
        }

        public static _FG_WebForm GetCSharpFormGlobals(EbWebForm _this, WebformData _formdata)
        {
            _FG_WebForm fG_WebForm = new _FG_WebForm();
            GetCSharpFormGlobalsRec(fG_WebForm, _this, _formdata);
            return fG_WebForm;
        }

        private static void GetCSharpFormGlobalsRec(_FG_WebForm fG_WebForm, EbControlContainer _container, WebformData _formdata)
        {
            SingleTable Table = _formdata.MultipleTables.ContainsKey(_container.TableName) ? _formdata.MultipleTables[_container.TableName] : new SingleTable();
            if (_container is EbDataGrid)
            {
                _FG_DataGrid fG_DataGrid = new _FG_DataGrid(_container as EbDataGrid, Table);
                fG_WebForm.DataGrids.Add(fG_DataGrid);
            }
            else if (_container is EbReview)
            {
                fG_WebForm.Review = new _FG_Review(_container as EbReview, Table);
            }
            else
            {
                foreach (EbControl _control in _container.Controls)
                {
                    if (_control is EbControlContainer)
                    {
                        GetCSharpFormGlobalsRec(fG_WebForm, _control as EbControlContainer, _formdata);
                    }
                    else
                    {
                        object data = null;
                        if (_formdata.MultipleTables.ContainsKey(_container.TableName) && _formdata.MultipleTables[_container.TableName].Count > 0)
                            data = _formdata.MultipleTables[_container.TableName][0][_control.Name];
                        fG_WebForm.FlatCtrls.Controls.Add(new _FG_Control(_control, data));
                    }
                }
            }

        }

        //get formdata as globals for c# script engine
        public static FormAsGlobal GetFormAsGlobal(EbWebForm _this, WebformData _formData, EbControlContainer _container = null, FormAsGlobal _globals = null)
        {
            if (_container == null)
                _container = _this;
            if (_globals == null)
                _globals = new FormAsGlobal { Name = _this.Name };

            ListNTV listNTV = new ListNTV();

            if (_formData.MultipleTables.ContainsKey(_container.TableName))
            {
                for (int i = 0; i < _formData.MultipleTables[_container.TableName].Count; i++)
                {
                    foreach (EbControl control in _container.Controls)
                    {
                        if (control is EbControlContainer)
                        {
                            FormAsGlobal g = new FormAsGlobal();
                            g.Name = (control as EbControlContainer).Name;
                            _globals.AddContainer(g);
                            g = GetFormAsGlobal(_this, _formData, control as EbControlContainer, g);
                        }
                        else
                        {
                            NTV n = GetNtvFromFormData(_formData, _container.TableName, i, control.Name);
                            if (n != null)
                                listNTV.Columns.Add(n);
                        }
                    }
                }
                _globals.Add(listNTV);
            }
            return _globals;
        }

        private static NTV GetNtvFromFormData(WebformData _formData, string _table, int _row, string _column)
        {
            NTV ntv = null;
            if (_formData.MultipleTables.ContainsKey(_table))
            {
                foreach (SingleColumn col in _formData.MultipleTables[_table][_row].Columns)
                {
                    if (col.Name.Equals(_column))
                    {
                        ntv = new NTV()
                        {
                            Name = _column,
                            Type = (EbDbTypes)col.Type,
                            Value = col.Value
                        };
                        break;
                    }
                }
            }
            return ntv;
        }

        //Excel Import
        public static FG_Root GetCSharpFormGlobals_NEW(EbWebForm _this, EbDataTable Data, int index)
        {
            Dictionary<string, FG_NV_List> dict = new Dictionary<string, FG_NV_List>();
            foreach (EbDataColumn dc in Data.Columns)
            {
                if (!dict.ContainsKey(dc.TableName))
                    dict.Add(dc.TableName, new FG_NV_List());
                dict[dc.TableName].Add(new FG_NV(dc.ColumnName, Data.Rows[index][dc.ColumnIndex]));
            }
            return new FG_Root(new FG_Params(dict));
        }

        public static List<FG_WebForm> GetEmptyDestinationModelGlobals(List<EbWebForm> DestForms)
        {
            List<FG_WebForm> fG_WebFormsList = new List<FG_WebForm>();
            for (int i = 0; i < DestForms.Count; i++)
            {
                EbWebForm Form = DestForms[i];
                WebformData _formdata = Form.GetEmptyModel();
                int createdBy = Form.TableRowId <= 0 ? Form.UserObj.UserId : _formdata.CreatedBy;
                string createdAt = Form.TableRowId <= 0 ? DateTime.UtcNow.ConvertFromUtc(Form.UserObj.Preference.TimeZone).ToString(FormConstants.yyyyMMdd_HHmmss, CultureInfo.InvariantCulture) : _formdata.CreatedAt;
                FG_WebForm fG_WebForm = new FG_WebForm(Form.TableName, Form.TableRowId, Form.LocationId, Form.RefId, createdBy, createdAt);
                GetCSharpFormGlobalsRec_NEW(fG_WebForm, Form, _formdata, null);
                fG_WebFormsList.Add(fG_WebForm);
            }
            return fG_WebFormsList;
        }

        public static FG_Root GetCSharpFormGlobals_NEW(EbWebForm _this, WebformData _formdata, WebformData _formdataBkUp, IDatabase DataDB, DbConnection DbCon, bool isSrcForm)
        {
            FG_User fG_User = new FG_User(_this.UserObj.UserId, _this.UserObj.FullName, _this.UserObj.Email, _this.UserObj.Roles);
            FG_System fG_System = new FG_System();
            FG_DataDB fG_DataDB = null;
            if (DataDB != null)
            {
                DelegateTest OutDelObj = new DelegateTest(DataDB, DbCon);
                fG_DataDB = new FG_DataDB(OutDelObj.ExecuteScalar);
            }
            WebformData _formdataEmpty = _this.GetEmptyModel();
            if (_formdata == null)
                _formdata = _formdataEmpty;
            else
                _formdata.DGsRowDataModel = _formdataEmpty.DGsRowDataModel;
            FG_Locations fG_Locations = Get_FG_Locations(_this.SolutionObj.Locations);
            int createdBy = _this.TableRowId <= 0 ? _this.UserObj.UserId : _formdata.CreatedBy;
            string createdAt = _this.TableRowId <= 0 ? DateTime.UtcNow.ConvertFromUtc(_this.UserObj.Preference.TimeZone).ToString(FormConstants.yyyyMMdd_HHmmss, CultureInfo.InvariantCulture) : _formdata.CreatedAt;
            FG_WebForm fG_WebForm = new FG_WebForm(_this.TableName, _this.TableRowId, _this.LocationId, _this.RefId, createdBy, createdAt);
            GetCSharpFormGlobalsRec_NEW(fG_WebForm, _this, _formdata, _formdataBkUp);

            return new FG_Root(fG_WebForm, fG_User, fG_System, isSrcForm, fG_DataDB, fG_Locations);
        }

        private static void GetCSharpFormGlobalsRec_NEW(FG_WebForm fG_WebForm, EbControlContainer _container, WebformData _formdata, WebformData _formdataBkUp)
        {
            SingleTable Table = _formdata.MultipleTables.ContainsKey(_container.TableName) ? _formdata.MultipleTables[_container.TableName] : new SingleTable();
            SingleTable TableBkUp = _formdataBkUp != null && _formdataBkUp.MultipleTables.ContainsKey(_container.TableName) ? _formdataBkUp.MultipleTables[_container.TableName] : new SingleTable();

            if (_container is EbDataGrid)
                fG_WebForm.DataGrids.Add(GetDataGridGlobal(_container as EbDataGrid, Table, _formdata.DGsRowDataModel[_container.TableName]));
            else if (_container is EbReview)
                fG_WebForm.Review = GetReviewGlobal(_container as EbReview, Table, TableBkUp);
            else
            {
                foreach (EbControl _control in _container.Controls)
                {
                    if (_control is EbControlContainer)
                        GetCSharpFormGlobalsRec_NEW(fG_WebForm, _control as EbControlContainer, _formdata, _formdataBkUp);
                    else
                    {
                        object data = null;
                        if (_control is EbAutoId && fG_WebForm.__mode == "new")// && fG_WebForm.id == 0
                            data = FormConstants.AutoId_PlaceHolder;
                        else
                        {
                            if (Table.Count > 0 && Table[0].GetColumn(_control.Name) != null && !(_control is EbAutoId)) //(!(_control is EbAutoId) || (_control is EbAutoId && fG_WebForm.__mode == "edit")))// && fG_WebForm.id > 0
                                data = Table[0][_control.Name];
                            else if (TableBkUp.Count > 0 && TableBkUp[0].GetColumn(_control.Name) != null)
                                data = TableBkUp[0][_control.Name];
                            else if (Table.Count > 0 && Table[0].GetColumn(_control.Name) != null)// Hint: For BatchDataPusher, AutoId available in 'Table' only
                                data = Table[0][_control.Name];
                        }
                        fG_WebForm.FlatCtrls.Controls.Add(new FG_Control(_control.Name, data));
                    }
                }
            }
        }

        private static FG_Locations Get_FG_Locations(Dictionary<int, EbLocation> ebLocs)
        {
            FG_Locations fG_Locations = new FG_Locations();
            foreach (KeyValuePair<int, EbLocation> locEnrty in ebLocs)
            {
                EbLocation l = locEnrty.Value;
                fG_Locations.Add(new FG_Location()
                {
                    LocId = l.LocId,
                    IsGroup = l.IsGroup,
                    Logo = l.Logo,
                    LongName = l.LongName,
                    Meta = l.Meta,
                    ParentId = l.ParentId,
                    ShortName = l.ShortName,
                    TypeId = l.TypeId,
                    TypeName = l.TypeName,
                    WeekHoliday1 = l.WeekHoliday1,
                    WeekHoliday2 = l.WeekHoliday2
                });
            }
            return fG_Locations;
        }

        private static FG_DataGrid GetDataGridGlobal(EbDataGrid DG, SingleTable Table, SingleRow RowModel)
        {
            List<FG_Row> Rows = new List<FG_Row>();
            foreach (SingleRow Row in Table)
            {
                FG_Row fG_Row = new FG_Row() { id = Convert.ToInt32(Row[FormConstants.id]) };
                foreach (EbControl _control in DG.Controls)
                {
                    fG_Row.Controls.Add(new FG_Control(_control.Name, Row[_control.Name]));
                }
                Rows.Add(fG_Row);
            }
            FG_Row fG_RowModel = new FG_Row();
            foreach (EbControl _control in DG.Controls)
            {
                fG_RowModel.Controls.Add(new FG_Control(_control.Name, RowModel[_control.Name]));
            }

            return new FG_DataGrid(DG.Name, Rows, fG_RowModel);
        }

        private static FG_Review GetReviewGlobal(EbReview Rev, SingleTable Table, SingleTable TableBkUp)
        {
            Dictionary<string, FG_Review_Stage> stages = new Dictionary<string, FG_Review_Stage>();
            FG_Review_Stage currentStage = null;
            SingleRow Row = null;
            foreach (SingleRow _Row in Table)
            {
                if (_Row.RowId <= 0 && _Row.Columns.Count > 0)
                {
                    Row = _Row;
                    break;
                }
            }

            foreach (EbReviewStage stage in Rev.FormStages)
            {
                List<FG_Review_Action> actions = new List<FG_Review_Action>();
                foreach (EbReviewAction action in stage.StageActions)
                {
                    actions.Add(new FG_Review_Action(action.Name));
                }

                if (Row != null && Convert.ToString(Row["stage_unique_id"]) == stage.EbSid)
                {
                    EbReviewAction curAct = stage.StageActions.Find(e => e.EbSid == Convert.ToString(Row["action_unique_id"]));
                    FG_Review_Action fg_curAct = null;
                    if (curAct != null)
                    {
                        fg_curAct = actions.Find(e => e.name == curAct.Name);
                    }
                    stages.Add(stage.Name, new FG_Review_Stage(stage.Name, actions, fg_curAct));
                    currentStage = stages[stage.Name];
                }
                else
                {
                    stages.Add(stage.Name, new FG_Review_Stage(stage.Name, actions, null));
                }
            }
            return new FG_Review(stages, currentStage);
        }

        //send notifications and email etc
        public static void PostProcessGlobals(EbWebForm _this, FG_Root _globals, Service services)
        {
            if (_globals.system == null || _globals.system.Notifications.Count == 0)
                return;

            if (string.IsNullOrEmpty(_this.RefId) || _this.TableRowId <= 0)
                return;

            List<Param> p = new List<Param> { { new Param { Name = "id", Type = ((int)EbDbTypes.Int32).ToString(), Value = _this.TableRowId.ToString() } } };
            string _params = JsonConvert.SerializeObject(p).ToBase64();
            string link = $"/WebForm/Index?_r={_this.RefId}&_p={_params}&_m=1";

            foreach (FG_Notification notification in _globals.system.Notifications)
            {
                try
                {
                    string title = notification.Title ?? _this.DisplayName + " notification";
                    if (notification.NotifyBy == FG_NotifyBy.UserId)
                    {
                        Console.WriteLine($"PostProcessGlobals -> NotifyByUserIDRequest. Tilte: {title}, UserId: {notification.UserId}");
                        NotifyByUserIDResponse result = services.Gateway.Send<NotifyByUserIDResponse>(new NotifyByUserIDRequest
                        {
                            Link = link,
                            Title = title,
                            UsersID = notification.UserId
                        });
                    }
                    else if (notification.NotifyBy == FG_NotifyBy.RoleIds)
                    {
                        Console.WriteLine($"PostProcessGlobals -> NotifyByUserRoleRequest. Tilte: {title}, RoleIds: {notification.RoleIds}");
                        NotifyByUserRoleResponse result = services.Gateway.Send<NotifyByUserRoleResponse>(new NotifyByUserRoleRequest
                        {
                            Link = link,
                            Title = title,
                            RoleID = notification.RoleIds
                        });
                    }
                    else if (notification.NotifyBy == FG_NotifyBy.UserGroupIds)
                    {
                        Console.WriteLine($"PostProcessGlobals -> NotifyByUserGroupRequest. Tilte: {title}, UserGroupId: {notification.UserGroupIds}");
                        NotifyByUserGroupResponse result = services.Gateway.Send<NotifyByUserGroupResponse>(new NotifyByUserGroupRequest
                        {
                            Link = link,
                            Title = title,
                            GroupId = notification.UserGroupIds
                        });
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Exception in PostProcessGlobals\nMessage: " + e.Message + "\nStackTrace: " + e.StackTrace);
                }
            }
        }
    }
}
