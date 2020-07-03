using ExpressBase.Common.Constants;
using ExpressBase.Common.Data;
using ExpressBase.Common.Extensions;
using ExpressBase.Common.Objects;
using ExpressBase.Common.Structures;
using ExpressBase.Objects.Objects;
using ExpressBase.Objects.ServiceStack_Artifacts;
using ServiceStack;
using ServiceStack.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace ExpressBase.Objects.WebFormRelated
{
    public static class BeforeSaveHelper
    {
        //Operations to be performed before form object save - table name required, table name repetition, calculate dependency
        public static void BeforeSave(EbWebForm _this, IServiceClient serviceClient, IRedisClient redis)
        {
            Dictionary<string, string> tbls = new Dictionary<string, string>();
            if (string.IsNullOrEmpty(_this.TableName))
                throw new FormException("Please enter a valid form table name");
            tbls.Add(_this.TableName, "form table");
            EbControl[] Allctrls = _this.Controls.FlattenAllEbControls();
            Dictionary<Type, bool> OneCtrls = new Dictionary<Type, bool>() // Limit more than one ctrl
            {
                { typeof(EbAutoId), false },
                { typeof(EbReview), false },
                { typeof(EbSubmitButton), false },
                { typeof(EbProvisionLocation), false },
                { typeof(EbSysLocation), false }
            };
            PerformRequirdCheck(Allctrls, OneCtrls, tbls, serviceClient, redis, out EbReview ebReviewCtrl);
            PerformRequirdUpdate(_this, _this.TableName);
            Dictionary<int, EbControlWrapper> _dict = new Dictionary<int, EbControlWrapper>();
            GetControlsAsDict(_this, "form", _dict);
            CalcValueExprDependency(_dict);
            ValidateAndUpdateReviewCtrl(_this, ebReviewCtrl, _dict);
            ValidateNotificationProp(_this.Notifications, _dict);
            SetDefaultValueExprExecOrder(_this, _dict);
            CalcHideAndDisableExprDependency(_dict);
        }

        private static void ValidateAndUpdateReviewCtrl(EbWebForm _this, EbReview ebReviewCtrl, Dictionary<int, EbControlWrapper> _dict)
        {
            if (ebReviewCtrl == null)
                return;

            for (int i = 0; i < ebReviewCtrl.FormStages.Count; i++)
            {
                EbReviewStage stage = ebReviewCtrl.FormStages[i];
                if (stage.ApproverEntity == ApproverEntityTypes.Users)
                {
                    Dictionary<string, string> QryParms = new Dictionary<string, string>();//<param, table>
                    string code = stage.ApproverUsers.Code;
                    if (string.IsNullOrEmpty(code))
                        throw new FormException($"Required SQL query for {ebReviewCtrl.Name}(review) control stage {stage.Name}");
                    MatchCollection matchColl = Regex.Matches(code, @"(?<=@)(\w+)|(?<=:)(\w+)");
                    foreach (Match match in matchColl)
                    {
                        KeyValuePair<int, EbControlWrapper> item = _dict.FirstOrDefault(e => e.Value.Control.Name == match.Value);
                        if (item.Value == null)
                            throw new FormException($"Can't resolve {match.Value} in {ebReviewCtrl.Name}(review) control's SQL query of stage {stage.Name}");
                        if (!QryParms.ContainsKey(item.Value.Control.Name))
                            QryParms.Add(item.Value.Control.Name, item.Value.TableName);
                    }
                    stage.QryParams = QryParms;
                }
                else if (stage.ApproverEntity == ApproverEntityTypes.UserGroup)
                {
                    if (stage.ApproverUserGroup <= 0)
                        throw new FormException($"Required a usergroup for stage {stage.Name} of {ebReviewCtrl.Name}(review) control");
                }
                else if (stage.ApproverEntity == ApproverEntityTypes.Role)
                {
                    if (stage.ApproverRoles == null || stage.ApproverRoles?.FindAll(e => e > 0).Count() == 0)
                        throw new FormException($"Required roles for stage {stage.Name} of {ebReviewCtrl.Name}(review) control");
                }
                else
                    throw new FormException($"Invalid ApproverEntity found for stage {stage.Name} of {ebReviewCtrl.Name}(review) control: " + stage.ApproverEntity);

                if (stage.IsAdvanced)
                {
                    //if (stage.StageActions == null || stage.StageActions?.Count == 0)
                    //    throw new FormException($"Required actions for stage {stage.Name} of {ebReviewCtrl.Name}(review) control");
                    //if (stage.NextStage == null || string.IsNullOrEmpty(stage.NextStage.Code))
                    //    throw new FormException($"Required next stage script for stage {stage.Name} of {ebReviewCtrl.Name}(review) control");
                }
                else
                {
                    stage.StageActions = new List<EbReviewAction>() {
                        new EbReviewAction(){ EbSid = stage.Name + "_ebreviewaction1", Name = "On Hold"},
                        new EbReviewAction(){ EbSid = stage.Name + "_ebreviewaction2", Name = "Accepted"},
                        new EbReviewAction(){ EbSid = stage.Name + "_ebreviewaction3", Name = "Rejected"}
                    };
                    string nxtStage = ebReviewCtrl.FormStages.Count == i + 1 ? $"form.review.complete(); \n\tsystem.sendNotificationByUserId(form.eb_created_by, \"Accepted your request for '{_this.DisplayName}'\")" : $@"return form.review.stages[""{ebReviewCtrl.FormStages[i + 1].Name}""]";

                    string code = $@"
if (form.review.currentStage.currentAction.name == ""On Hold""){{
    return form.review.stages[""{stage.Name}""];
}}
if (form.review.currentStage.currentAction.name == ""Accepted""){{
    {nxtStage};
}}
if (form.review.currentStage.currentAction.name == ""Rejected""){{
    form.review.abandon();
    system.sendNotificationByUserId(form.eb_created_by, ""Rejected your request for '{_this.DisplayName}'"");
}}
";
                    stage.NextStage = new EbScript() { Lang = ScriptingLanguage.CSharp, Code = code };
                }
            }
        }

        private static void ValidateNotificationProp(List<EbFormNotification> _Notifications, Dictionary<int, EbControlWrapper> _dict)
        {
            if (_Notifications?.Count <= 0)
                return;
            for (int i = 0; i < _Notifications.Count; i++)
            {
                if (_Notifications[i] is EbFnSystem)
                {
                    EbFnSystem ebFnSys = _Notifications[i] as EbFnSystem;
                    if (ebFnSys.NotifyBy == EbFnSys_NotifyBy.Roles)
                    {
                        if (!(ebFnSys.Roles?.FindAll(e => e > 0).Count() > 0))
                            throw new FormException("Invalid roles found for system notification");
                    }
                    else if (ebFnSys.NotifyBy == EbFnSys_NotifyBy.UserGroup)
                    {
                        if (ebFnSys.UserGroup <= 0)
                            throw new FormException("Invalid user group found for system notification");
                    }
                    else if (ebFnSys.NotifyBy == EbFnSys_NotifyBy.Users)
                    {
                        ebFnSys.QryParams = new Dictionary<string, string>();//<param, table>
                        if (string.IsNullOrEmpty(ebFnSys.Users?.Code))
                            throw new FormException("Required SQL query for system notification");
                        MatchCollection matchColl = Regex.Matches(ebFnSys.Users.Code, @"(?<=@)(\w+)|(?<=:)(\w+)");
                        foreach (Match match in matchColl)
                        {
                            KeyValuePair<int, EbControlWrapper> item = _dict.FirstOrDefault(e => e.Value.Control.Name == match.Value);
                            if (item.Value == null)
                                throw new FormException($"Can't resolve {match.Value} in SQL query of system notification");
                            if (!ebFnSys.QryParams.ContainsKey(item.Value.Control.Name))
                                ebFnSys.QryParams.Add(item.Value.Control.Name, item.Value.TableName);
                        }
                    }
                    else
                        throw new FormException("Invalid NotifyBy found for system notification");
                }
                else if (_Notifications[i] is EbFnEmail)
                {
                    if (string.IsNullOrEmpty((_Notifications[i] as EbFnEmail).RefId))
                        throw new FormException($"Invalid Ref id found for email notification");
                }
            }
        }

        private static void PerformRequirdCheck(EbControl[] Allctrls, Dictionary<Type, bool> OneCtrls, Dictionary<string, string> tbls, IServiceClient serviceClient, IRedisClient redis, out EbReview ebReviewCtrl)
        {
            ebReviewCtrl = null;
            for (int i = 0; i < Allctrls.Length; i++)
            {
                if (OneCtrls.ContainsKey(Allctrls[i].GetType()))
                {
                    Type _type = Allctrls[i].GetType();
                    if (OneCtrls[_type])
                    {
                        string st = string.IsNullOrEmpty(Allctrls[i].ToolNameAlias) ? _type.Name.Substring(2) : Allctrls[i].ToolNameAlias;
                        throw new FormException($"Not allowed more than one {st} control");
                    }
                    OneCtrls[_type] = true;
                    if (Allctrls[i] is EbReview)
                        ebReviewCtrl = Allctrls[i] as EbReview;
                }
                else if (Allctrls[i] is EbApproval)//deprecated
                {
                    string _tn = (Allctrls[i] as EbApproval).TableName;
                    if (string.IsNullOrEmpty(_tn))
                        throw new FormException("Please enter a valid table name for " + Allctrls[i].Label + " (approval control)");
                    if (tbls.ContainsKey(_tn))
                        throw new FormException(string.Format("Same table not allowed for {1} and {2}(approval control) : {0}", _tn, tbls[_tn], Allctrls[i].Label));
                    tbls.Add(_tn, Allctrls[i].Label + "(approval control)");
                }
                else if (Allctrls[i] is EbDataGrid)
                {
                    EbDataGrid DataGrid = Allctrls[i] as EbDataGrid;
                    string _tn = DataGrid.TableName;
                    if (string.IsNullOrEmpty(DataGrid.TableName))
                        throw new FormException("Please enter a valid table name for " + Allctrls[i].Label + " (data grid)");
                    if (tbls.ContainsKey(_tn))
                        throw new FormException(string.Format("Same table not allowed for {1} and {2}(data grid) : {0}", _tn, tbls[_tn], Allctrls[i].Label));
                    tbls.Add(_tn, Allctrls[i].Label + "(data grid)");

                    for (int j = 0; j < DataGrid.Controls.Count; j++)
                    {
                        if (DataGrid.Controls[j] is EbDGUserControlColumn)
                        {
                            if (string.IsNullOrEmpty(DataGrid.Controls[j].RefId))
                                throw new FormException($"User control reference is missing for {(DataGrid.Controls[j] as EbDGColumn).Title} in {DataGrid.Label}.");
                            EbDGColumn DGColumn = DataGrid.Controls[j] as EbDGColumn;
                            (DataGrid.Controls[j] as EbDGUserControlColumn).Columns = new List<EbControl>();
                        }
                    }
                    if (!string.IsNullOrEmpty(DataGrid.DataSourceId) && serviceClient != null)
                        DataGrid.InitDSRelated(serviceClient, redis, Allctrls);
                }
                else if (Allctrls[i] is EbProvisionUser && serviceClient != null)
                {
                    CheckEmailConAvailableResponse Resp = serviceClient.Post<CheckEmailConAvailableResponse>(new CheckEmailConAvailableRequest { });
                    if (!Resp.ConnectionAvailable)
                        throw new FormException("Please configure a email connection, it is required for ProvisionUser control.");
                }
                else if (Allctrls[i] is EbChartControl && serviceClient != null)
                {
                    if (string.IsNullOrEmpty((Allctrls[i] as EbChartControl).TVRefId))
                        throw new FormException($"Please set a Chart View for {Allctrls[i].Label}.");
                    (Allctrls[i] as EbChartControl).FetchParamsMeta(serviceClient, redis);
                }
                else if (Allctrls[i] is EbTVcontrol && serviceClient != null)
                {
                    if (string.IsNullOrEmpty((Allctrls[i] as EbTVcontrol).TVRefId))
                        throw new FormException($"Please set a Table View for {Allctrls[i].Label}.");
                    (Allctrls[i] as EbTVcontrol).FetchParamsMeta(serviceClient, redis);
                }
                else if (Allctrls[i] is EbPowerSelect)
                {
                    EbPowerSelect _ctrl = Allctrls[i] as EbPowerSelect;
                    if (string.IsNullOrEmpty(_ctrl.DataSourceId))
                        throw new FormException("Set Data Reader for " + _ctrl.Label);
                    if (_ctrl.ValueMember == null)
                        throw new FormException("Set Value Member for " + _ctrl.Label);
                    if (_ctrl.RenderAsSimpleSelect && _ctrl.DisplayMember == null)
                        throw new FormException("Set Display Member for " + _ctrl.Label);
                    if (!_ctrl.RenderAsSimpleSelect && (_ctrl.DisplayMembers == null || _ctrl.DisplayMembers.Count == 0))
                        throw new FormException("Set Display Members for " + _ctrl.Label);
                    EbDbTypes _t = _ctrl.ValueMember.Type;
                    if (!(_t == EbDbTypes.Int || _t == EbDbTypes.Int || _t == EbDbTypes.UInt32 || _t == EbDbTypes.UInt64 || _t == EbDbTypes.Int32 || _t == EbDbTypes.Int64 || _t == EbDbTypes.Decimal || _t == EbDbTypes.Double))
                        throw new FormException("Set numeric value member for " + _ctrl.Label);
                }
                else if (Allctrls[i] is EbDGPowerSelectColumn)
                {
                    EbDGPowerSelectColumn _ctrl = Allctrls[i] as EbDGPowerSelectColumn;
                    if (string.IsNullOrEmpty(_ctrl.DataSourceId))
                        throw new FormException("Set Data Reader for " + _ctrl.Name);
                    if (_ctrl.ValueMember == null)
                        throw new FormException("Set Value Member for " + _ctrl.Name);
                    if (_ctrl.RenderAsSimpleSelect && _ctrl.DisplayMember == null)
                        throw new FormException("Set Display Member for " + _ctrl.Name);
                    if (!_ctrl.RenderAsSimpleSelect && (_ctrl.DisplayMembers == null || _ctrl.DisplayMembers.Count == 0))
                        throw new FormException("Set Display Members for " + _ctrl.Name);
                    EbDbTypes _t = _ctrl.ValueMember.Type;
                    if (!(_t == EbDbTypes.Int || _t == EbDbTypes.Int || _t == EbDbTypes.UInt32 || _t == EbDbTypes.UInt64 || _t == EbDbTypes.Int32 || _t == EbDbTypes.Int64 || _t == EbDbTypes.Decimal || _t == EbDbTypes.Double))
                        throw new FormException("Set numeric value member for " + _ctrl.Name);
                }
                else if (Allctrls[i] is EbUserControl)
                {
                    if (string.IsNullOrEmpty(Allctrls[i].RefId))
                        throw new FormException($"User control reference is missing for {Allctrls[i].Label}.");
                }
                if (Allctrls[i] is EbDynamicCardSet)
                {
                    EbDynamicCardSet _ctrl = Allctrls[i] as EbDynamicCardSet;
                    if (string.IsNullOrEmpty(_ctrl.DataSourceId))
                        throw new FormException("Set Data Reader for Dynamic Card - " + _ctrl.Label ?? _ctrl.Name);
                    if (_ctrl.ValueMember == null)
                        throw new FormException("Set Value Member for Dynamic Card - " + _ctrl.Label ?? _ctrl.Name);
                    if (_ctrl.CardFields?.Count == 0)
                        throw new FormException("Set Card Fields for Dynamic Card - " + _ctrl.Label ?? _ctrl.Name);
                    if (string.IsNullOrEmpty(_ctrl.TableName))
                        throw new FormException("Please enter a valid Dynamic Card table name");

                    string _tn = _ctrl.TableName;
                    if (string.IsNullOrEmpty(_ctrl.TableName))
                        throw new FormException("Please enter a valid table name for " + _ctrl.Label + " (dynamic card)");
                    if (tbls.ContainsKey(_tn))
                        throw new FormException(string.Format("Same table not allowed for {1} and {2}(dynamic card) : {0}", _tn, tbls[_tn], _ctrl.Label));
                    tbls.Add(_tn, _ctrl.Label + "(dynamic card)");
                }
                else if (Allctrls[i] is EbStaticCardSet)
                {
                    EbStaticCardSet _ctrl = Allctrls[i] as EbStaticCardSet;
                    if (_ctrl.CardFields?.Count == 0)
                        throw new FormException("Set Card Fields for Static Card - " + _ctrl.Label ?? _ctrl.Name);
                    if (string.IsNullOrEmpty(_ctrl.TableName))
                        throw new FormException("Please enter a valid Static Card table name");
                }
            }
        }

        private static void PerformRequirdUpdate(EbControlContainer _cont, string _tbl)
        {
            if (_cont is EbDataGrid && _cont.IsDynamicTabChild)
            {
                _cont.IsDynamicTabChild = false;
                //(_cont as EbDataGrid).IsAddable = false;
            }
            foreach (EbControl ctrl in _cont.Controls)
            {
                ctrl.IsDynamicTabChild = _cont.IsDynamicTabChild;
                if (ctrl.IsDynamicTabChild && !(ctrl is EbDataGrid))
                    ctrl.DoNotPersist = true;
                if (ctrl is EbTextBox)
                {
                    if ((ctrl as EbTextBox).AutoSuggestion)
                        (ctrl as EbTextBox).TableName = _tbl;
                }
                else if (ctrl is EbDGStringColumn)
                {
                    if ((ctrl as EbDGStringColumn).AutoSuggestion)
                        (ctrl as EbDGStringColumn).TableName = _tbl;
                }
                else if (ctrl is EbControlContainer)
                {
                    if (ctrl is EbTabPane && (ctrl as EbTabPane).IsDynamic)
                    {
                        ctrl.IsDynamicTabChild = true;
                    }
                    string t = _tbl;
                    if (ctrl is EbTableLayout || ctrl is EbTableTd || ctrl is EbTabControl || ctrl is EbTabPane)///////table name filling
                        (ctrl as EbControlContainer).TableName = _tbl;
                    if (!string.IsNullOrEmpty((ctrl as EbControlContainer).TableName))
                        t = (ctrl as EbControlContainer).TableName;
                    PerformRequirdUpdate(ctrl as EbControlContainer, t);
                }
            }
        }

        //Populate Property DependedValExp
        private static void CalcValueExprDependency(Dictionary<int, EbControlWrapper> _dict)
        {
            List<int> CalcFlds = new List<int>();
            List<KeyValuePair<int, int>> dpndcy = new List<KeyValuePair<int, int>>();

            for (int i = 0; i < _dict.Count; i++)
            {
                _dict[i].Control.DependedValExp = new List<string>();
                _dict[i].Control.ValExpParams = new List<string>();

                if (!string.IsNullOrEmpty(_dict[i].Control.ValueExpr?.Code))
                    CalcFlds.Add(i);
                if (_dict[i].Control is EbTVcontrol && (_dict[i].Control as EbTVcontrol).ParamsList?.Count > 0)
                    CalcFlds.Add(i);
                if (!string.IsNullOrEmpty(_dict[i].Control.OnChangeFn?.Code))
                {
                    if (_dict[i].Control.OnChangeFn.Code.Contains(".setValue(") && !(_dict[i].Control is EbScriptButton))
                        throw new FormException("SetValue is not allowed in OnChange expression of " + _dict[i].Control.Name);
                }
            }

            for (int i = 0; i < CalcFlds.Count; i++)
            {
                string code = _dict[CalcFlds[i]].Control.ValueExpr.Code;
                if (_dict[CalcFlds[i]].Control is EbTVcontrol)// +ex
                {
                    List<Param> ParamsList = (_dict[CalcFlds[i]].Control as EbTVcontrol).ParamsList;
                    foreach (Param _p in ParamsList)
                    {
                        KeyValuePair<int, EbControlWrapper> item = _dict.FirstOrDefault(e => !(e.Value.Control is EbDGColumn) && e.Value.Control.Name == _p.Name);
                        if (item.Value != null)
                            dpndcy.Add(new KeyValuePair<int, int>(CalcFlds[i], item.Key));
                        else if (_p.Name != "eb_loc_id" && _p.Name != "eb_currentuser_id")
                            throw new FormException($"Can't resolve parameter {_p.Name} in table view of {_dict[CalcFlds[i]].Control.Name}");
                    }
                }
                else if (_dict[CalcFlds[i]].Control.ValueExpr.Lang == ScriptingLanguage.JS)
                {
                    if (code.Contains("form"))
                    {
                        bool IsAnythingResolved = false;
                        for (int j = 0; j < _dict.Count; j++)
                        {
                            string p = _dict[j].Path, r = _dict[j].Root, n = _dict[j].Control.Name;
                            string regex = $@"{r}.currentRow\[""{n}""\]|{r}.currentRow\['{n}'\]|{r}.currentRow.{n}|{r}.getRowByIndex\((.*?)\)\[""{n}""\]|{r}.getRowByIndex\((.*?)\)\['{n}'\]|{p}";

                            if (Regex.IsMatch(code, regex))
                            {
                                dpndcy.Add(new KeyValuePair<int, int>(CalcFlds[i], j));//<dependent, dominant>
                                IsAnythingResolved = true;
                            }
                        }
                        if (!IsAnythingResolved)
                            throw new FormException($"Can't resolve some form variables in Js Value expression of {_dict[CalcFlds[i]].Control.Name}");
                    }
                }
                else if (_dict[CalcFlds[i]].Control.ValueExpr.Lang == ScriptingLanguage.SQL)
                {
                    MatchCollection matchColl = Regex.Matches(code, @"(?<=@)(\w+)|(?<=:)(\w+)");
                    foreach (Match match in matchColl)
                    {
                        KeyValuePair<int, EbControlWrapper> item = _dict.FirstOrDefault(e => e.Value.Control.Name == match.Value);
                        if (item.Value == null)
                            throw new FormException($"Can't resolve {match.Value} in SQL Value expression of {_dict[CalcFlds[i]].Control.Name}");

                        dpndcy.Add(new KeyValuePair<int, int>(CalcFlds[i], item.Key));//<dependent, dominant>
                        _dict[CalcFlds[i]].Control.ValExpParams.Add(item.Value.Path);
                    }
                }
            }

            for (int i = 0; i < CalcFlds.Count; i++)
            {
                List<int> execOrder = new List<int> { CalcFlds[i] };
                GetValExpDependentsRec(execOrder, dpndcy, CalcFlds[i]);
                if (dpndcy.FindIndex(x => x.Key == CalcFlds[i] && x.Value == CalcFlds[i]) == -1)
                    execOrder.Remove(CalcFlds[i]);
                foreach (int key in execOrder)
                    _dict[CalcFlds[i]].Control.DependedValExp.Add(_dict[key].Path);
            }
        }

        private static void GetValExpDependentsRec(List<int> execOrder, List<KeyValuePair<int, int>> dpndcy, int seeker)
        {
            foreach (KeyValuePair<int, int> item in dpndcy.Where(e => e.Value == seeker))
            {
                if (!execOrder.Contains(item.Key))
                {
                    execOrder.Add(item.Key);
                    GetValExpDependentsRec(execOrder, dpndcy, item.Key);
                }
            }
        }

        //Populate Property DefaultValsExecOrder
        private static void SetDefaultValueExprExecOrder(EbWebForm _this, Dictionary<int, EbControlWrapper> _dict)
        {
            _this.DefaultValsExecOrder = new List<string>();//cleared the old values
            List<int> CalcFlds = new List<int>();
            List<KeyValuePair<int, int>> dpndcy = new List<KeyValuePair<int, int>>();
            List<int> ExecOrd = new List<int>();

            for (int i = 0; i < _dict.Count; i++)
            {
                if (!string.IsNullOrEmpty(_dict[i].Control.DefaultValueExpression?.Code))
                    CalcFlds.Add(i);
            }

            for (int i = 0; i < CalcFlds.Count; i++)
            {
                string code = _dict[CalcFlds[i]].Control.DefaultValueExpression.Code;
                if (_dict[CalcFlds[i]].Control.DefaultValueExpression.Lang == ScriptingLanguage.JS)
                {
                    if (code.Contains("form"))
                    {
                        bool IsAnythingResolved = false;
                        for (int j = 0; j < _dict.Count; j++)
                        {
                            string p = _dict[j].Path, r = _dict[j].Root, n = _dict[j].Control.Name;
                            string regex = $@"{r}.currentRow\[""{n}""\]|{r}.currentRow\['{n}'\]|{r}.currentRow.{n}|{r}.getRowByIndex\((.*?)\)\[""{n}""\]|{r}.getRowByIndex\((.*?)\)\['{n}'\]|{p}";

                            if (Regex.IsMatch(code, regex))
                            {
                                if (CalcFlds[i] != j)//if a control refers itself treated as not circular reference
                                    dpndcy.Add(new KeyValuePair<int, int>(CalcFlds[i], j));//<dependent, dominant>
                                IsAnythingResolved = true;
                            }
                        }
                        if (!IsAnythingResolved)
                            throw new FormException($"Can't resolve some form variables in Js Default value expression of {_dict[CalcFlds[i]].Control.Name}");
                    }
                }
            }
            int stopCounter = 0;
            while (CalcFlds.Count > ExecOrd.Count && stopCounter < CalcFlds.Count)
            {
                for (int i = 0; i < CalcFlds.Count; i++)
                {
                    if (dpndcy.FindIndex(x => x.Value == CalcFlds[i]) == -1 && !ExecOrd.Contains(CalcFlds[i]))
                    {
                        ExecOrd.Add(CalcFlds[i]);
                        dpndcy.RemoveAll(x => x.Key == CalcFlds[i]);
                    }
                }
                stopCounter++;
            }
            if (dpndcy.Count > 0)
                throw new FormException("Avoid circular reference by the following controls in 'DefaultValueExpression' : " + string.Join(',', dpndcy.Select(e => _dict[e.Key].Control.Name).Distinct()));

            foreach (int i in ExecOrd)
                _this.DefaultValsExecOrder.Add(_dict[i].Path);
        }

        private static void CalcHideAndDisableExprDependency(Dictionary<int, EbControlWrapper> _dict)
        {
            for (int i = 0; i < _dict.Count; i++)
            {
                _dict[i].Control.HiddenExpDependants = new List<string>();
                _dict[i].Control.DisableExpDependants = new List<string>();
            }
            for (int i = 0; i < _dict.Count; i++)
            {
                CheckHideAndDisableCode(_dict[i], _dict, _dict[i].Control.HiddenExpr, true);
                CheckHideAndDisableCode(_dict[i], _dict, _dict[i].Control.DisableExpr, false);
            }
        }

        private static void CheckHideAndDisableCode(EbControlWrapper ctrlWrap, Dictionary<int, EbControlWrapper> _dict, EbScript ebScript, bool is4Hide)
        {
            if (string.IsNullOrEmpty(ebScript?.Code))
                return;
            if (ebScript.Lang == ScriptingLanguage.JS)
            {
                if (ebScript.Code.Contains("form"))
                {
                    bool IsAnythingResolved = false;
                    for (int j = 0; j < _dict.Count; j++)
                    {
                        string p = _dict[j].Path, r = _dict[j].Root, n = _dict[j].Control.Name;
                        string regex = $@"{r}.currentRow\[""{n}""\]|{r}.currentRow\['{n}'\]|{r}.currentRow.{n}|{r}.getRowByIndex\((.*?)\)\[""{n}""\]|{r}.getRowByIndex\((.*?)\)\['{n}'\]|{p}";

                        if (Regex.IsMatch(ebScript.Code, regex))
                        {
                            if(is4Hide)
                                _dict[j].Control.HiddenExpDependants.Add(ctrlWrap.Path);
                            else
                                _dict[j].Control.DisableExpDependants.Add(ctrlWrap.Path);

                            IsAnythingResolved = true;
                        }
                    }
                    if (!IsAnythingResolved)
                        throw new FormException($"Can't resolve some form variables in Js {(is4Hide ? "Hide expression" : "Disable expression")} of {ctrlWrap.Control.Name}");
                }
            }
        }

        //get controls in webform as a single dimensional structure 
        public static void GetControlsAsDict(EbControlContainer _container, string _path, Dictionary<int, EbControlWrapper> _dict)
        {
            int _counter = _dict.Count;
            IEnumerable<EbControl> FlatCtrls = _container.Controls.Get1stLvlControls();
            foreach (EbControl control in FlatCtrls)
            {
                string path = _path == string.Empty ? control.Name : _path + CharConstants.DOT + control.Name;
                control.__path = path;
                _dict.Add(_counter++, new EbControlWrapper
                {
                    TableName = _container.TableName,
                    Path = path,
                    Control = control,
                    Root = _path
                });
            }
            foreach (EbControl control in _container.Controls)
            {
                if (control is EbControlContainer)
                {
                    string path = _path;
                    if (control is EbDataGrid)
                        path = _path + CharConstants.DOT + (control as EbControlContainer).Name;
                    GetControlsAsDict(control as EbControlContainer, path, _dict);
                }
            }
        }

        public static void BeforeSave_BotForm(EbBotForm _this, IServiceClient serviceClient, IRedisClient redis)
        {
            Dictionary<string, string> tbls = new Dictionary<string, string>();
            if (string.IsNullOrEmpty(_this.TableName))
                throw new FormException("Please enter a valid form table name");
            tbls.Add(_this.TableName, "form table");
            EbControl[] Allctrls = _this.Controls.FlattenAllEbControls();
            Dictionary<Type, bool> OneCtrls = new Dictionary<Type, bool>() // Limit more than one ctrl
            {
                { typeof(EbAutoId), false }
            };
            PerformRequirdCheck(Allctrls, OneCtrls, tbls, serviceClient, redis, out EbReview ebReviewCtrl);
            Dictionary<int, EbControlWrapper> _dict = new Dictionary<int, EbControlWrapper>();
            GetControlsAsDict(_this, "form", _dict);
            CalcValueExprDependency(_dict);
            ValidateNotificationProp(_this.Notifications, _dict);
        }
    }
}
