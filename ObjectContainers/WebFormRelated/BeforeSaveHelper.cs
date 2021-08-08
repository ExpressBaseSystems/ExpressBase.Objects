using ExpressBase.Common.Constants;
using ExpressBase.Common.Data;
using ExpressBase.Common.Extensions;
using ExpressBase.Common.Objects;
using ExpressBase.Common.Structures;
using ExpressBase.Objects.Helpers;
using ExpressBase.Objects.Objects;
using ExpressBase.Objects.ServiceStack_Artifacts;
using Newtonsoft.Json.Linq;
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
            if (_this.MakeEbSidUnique)
                UpdateEbSid(_this, Allctrls, false);
            PerformRequirdCheck(Allctrls, OneCtrls, tbls, serviceClient, redis, out EbReview ebReviewCtrl);
            PerformRequirdUpdate(_this, _this.TableName);
            Dictionary<int, EbControlWrapper> _dict = new Dictionary<int, EbControlWrapper>();
            GetControlsAsDict(_this, "form", _dict);
            CalcValueExprDependency(_this, _dict);
            CalcDataReaderDependency(_this, _dict);
            ValidateAndUpdateReviewCtrl(_this, ebReviewCtrl, _dict);
            ValidateNotificationProp(_this.Notifications, _dict);
            SetDefaultValueExprExecOrder(_this, _dict);
            CalcHideAndDisableExprDependency(_dict);

            if (_this.DataPushers?.Count > 0)
            {
                for (int i = 0; i < _this.DataPushers.Count; i++)
                {
                    EbDataPusher dp = _this.DataPushers[i];
                    if (dp is EbApiDataPusher _apiDp)
                    {
                        if (string.IsNullOrEmpty(_apiDp.ApiRefId))
                            throw new FormException($"Required 'Api ref id' for data pusher");
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(dp.FormRefId))
                            throw new FormException($"Required 'Form ref id' for data pusher");

                        if (dp is EbBatchFormDataPusher _batchdp)
                        {
                            if (string.IsNullOrWhiteSpace(_batchdp.SourceDG))
                                throw new FormException($"Required 'Source datagrid' for data pusher");
                        }
                        else if (!(dp is EbFormDataPusher))
                        {
                            //Converting to EbFormDataPusher!!! Old objects may contain 'EbDataPusher' type (base type)

                            _this.DataPushers[i] = new EbFormDataPusher()
                            {
                                EbSid = dp.EbSid,
                                FormRefId = dp.FormRefId,
                                Json = dp.Json,
                                MultiPushIdType = Common.MultiPushIdTypes.Default,
                                Name = dp.Name,
                                PushOnlyIf = dp.PushOnlyIf,
                                SkipLineItemIf = dp.SkipLineItemIf
                            };
                            dp = _this.DataPushers[i];
                        }
                    }
                    if (string.IsNullOrEmpty(dp.Json))
                        throw new FormException($"Required 'Json' for data pusher");
                    try
                    {
                        JToken JTok = JToken.Parse(dp.Json);
                    }
                    catch (Exception e)
                    {
                        throw new FormException($"Failed to parse 'Json' in data pusher: " + e.Message);
                    }
                }
            }
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

                    List<Param> _params = SqlHelper.GetSqlParams(code);
                    foreach (Param _p in _params)
                    {
                        if (EbFormHelper.IsExtraSqlParam(_p.Name, _this.TableName))
                        {
                            if (!QryParms.ContainsKey(_p.Name))
                                QryParms.Add(_p.Name, _this.TableName);
                            continue;
                        }
                        KeyValuePair<int, EbControlWrapper> item = _dict.FirstOrDefault(e => e.Value.Control.Name == _p.Name);
                        if (item.Value == null)
                            throw new FormException($"Can't resolve {_p.Name} in {ebReviewCtrl.Name}(review) control's SQL query of stage {stage.Name}");
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
                _Notifications[i].BeforeSaveValidation(_dict);
        }

        private static void PerformRequirdCheck(EbControl[] Allctrls, Dictionary<Type, bool> OneCtrls, Dictionary<string, string> tbls, IServiceClient serviceClient, IRedisClient redis, out EbReview ebReviewCtrl)
        {
            ebReviewCtrl = null;
            for (int i = 0; i < Allctrls.Length; i++)//DataGrid.InitDSRelated
                Allctrls[i].DependedDG = new List<string>();

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
                                throw new FormException($"User control reference is missing for {(DataGrid.Controls[j] as EbDGColumn).Title} in {DataGrid.Label}.");// DataGrid.Label
                            (DataGrid.Controls[j] as EbDGUserControlColumn).Columns = new List<EbControl>();
                        }
                    }
                    if (!string.IsNullOrEmpty(DataGrid.DataSourceId) && serviceClient != null)
                        DataGrid.InitDSRelated(serviceClient, redis, Allctrls);
                }
                else if (Allctrls[i] is EbProvisionUser)
                {
                    EbProvisionUser provUser = Allctrls[i] as EbProvisionUser;
                    bool isUnameMapped = false;
                    foreach (UsrLocFieldAbstract fld in provUser.Fields)
                    {
                        UsrLocField _field = fld as UsrLocField;
                        if (string.IsNullOrEmpty(_field.ControlName))
                            continue;
                        if (_field.Name == "email" || _field.Name == "phprimary")
                            isUnameMapped = true;
                        if (Allctrls.FirstOrDefault(e => e.Name == _field.ControlName) == null)
                            throw new FormException($"Invalid control name '{_field.ControlName}' for {_field.Name} in ProvisionUser control({provUser.Name}).");
                    }
                    if (!isUnameMapped)
                        throw new FormException("Please set email/phprimary in ProvisionUser control: {provUser.Name}.");

                    if (serviceClient != null)
                    {
                        //CheckEmailConAvailableResponse Resp = serviceClient.Post<CheckEmailConAvailableResponse>(new CheckEmailConAvailableRequest { });
                        //if (!Resp.ConnectionAvailable)
                        //    throw new FormException("Please configure a email connection, it is required for ProvisionUser control.");
                        Console.WriteLine("From BeforeSave: Please configure a email connection, it is required for ProvisionUser control.");
                    }
                }
                else if (Allctrls[i] is EbChartControl)
                {
                    if (string.IsNullOrEmpty((Allctrls[i] as EbChartControl).TVRefId))
                        throw new FormException($"Please set a Chart View for {Allctrls[i].Label}.");
                }
                else if (Allctrls[i] is EbTVcontrol)
                {
                    if (string.IsNullOrEmpty((Allctrls[i] as EbTVcontrol).TVRefId))
                        throw new FormException($"Please set a Table View for {Allctrls[i].Label}.");
                }
                else if (Allctrls[i] is EbPdfControl && serviceClient != null)
                {
                    if (string.IsNullOrEmpty((Allctrls[i] as EbPdfControl).PdfRefid))
                        throw new FormException($"Please set a pdf View for {Allctrls[i].Label}.");
                    (Allctrls[i] as EbPdfControl).FetchParamsMeta(serviceClient, redis);
                }
                else if (Allctrls[i] is IEbPowerSelect)
                {
                    IEbPowerSelect _ctrl = Allctrls[i] as IEbPowerSelect;
                    string _label = (Allctrls[i] is EbDGPowerSelectColumn _dgCtrl) ? _dgCtrl.Title ?? _dgCtrl.Name : (Allctrls[i] as EbControl).Label ?? (Allctrls[i] as EbControl).Name;
                    if (string.IsNullOrEmpty(_ctrl.DataSourceId) && !_ctrl.IsDataFromApi)
                        throw new FormException("Set Data Reader for " + _label);
                    if (string.IsNullOrEmpty(_ctrl.Url) && _ctrl.IsDataFromApi)
                        throw new FormException("Set Api url for " + _label);
                    if (_ctrl.ValueMember == null)
                        throw new FormException("Set Value Member for " + _label);
                    if (_ctrl.RenderAsSimpleSelect && _ctrl.DisplayMember == null)
                        throw new FormException("Set Display Member for " + _label);
                    if (!_ctrl.RenderAsSimpleSelect && (_ctrl.DisplayMembers == null || _ctrl.DisplayMembers.Count == 0))
                        throw new FormException("Set Display Members for " + _label);
                    EbDbTypes _t = _ctrl.ValueMember.Type;
                    if (!(_t == EbDbTypes.Int || _t == EbDbTypes.Int || _t == EbDbTypes.UInt32 || _t == EbDbTypes.UInt64 || _t == EbDbTypes.Int32 || _t == EbDbTypes.Int64 || _t == EbDbTypes.Decimal || _t == EbDbTypes.Double))
                        throw new FormException("Set numeric value member for " + _label);
                    if (_ctrl.IsInsertable && string.IsNullOrWhiteSpace(_ctrl.FormRefId))
                        throw new FormException("Set FormRefId for insertable PowerSelect: " + _label);
                }
                else if (Allctrls[i] is EbUserControl)
                {
                    if (string.IsNullOrEmpty(Allctrls[i].RefId))
                        throw new FormException($"User control reference is missing for {Allctrls[i].Label}.");
                }
                else if (Allctrls[i] is EbSimpleSelect)
                {
                    EbSimpleSelect _ctrl = Allctrls[i] as EbSimpleSelect;
                    if (_ctrl.IsDynamic)
                    {
                        if (string.IsNullOrEmpty(_ctrl.DataSourceId))
                            throw new FormException("Set 'data reader' for simple select - " + _ctrl.Label ?? _ctrl.Name);
                        if (_ctrl.ValueMember == null)
                            throw new FormException("Set 'value member' for simple select - " + _ctrl.Label ?? _ctrl.Name);
                        if (_ctrl.DisplayMember == null)
                            throw new FormException("Set 'display member' for simple select - " + _ctrl.Label ?? _ctrl.Name);
                    }
                    else
                    {
                        if (!(_ctrl.Options?.Count > 0))
                            throw new FormException("Set 'options' for simple select - " + _ctrl.Label ?? _ctrl.Name);
                    }
                }
                else if (Allctrls[i] is EbDGSimpleSelectColumn)
                {
                    EbDGSimpleSelectColumn _ctrl = Allctrls[i] as EbDGSimpleSelectColumn;
                    if (_ctrl.IsDynamic)
                    {
                        if (string.IsNullOrEmpty(_ctrl.DataSourceId))
                            throw new FormException("Set 'data reader' for simple select column - " + _ctrl.Title ?? _ctrl.Name);
                        if (_ctrl.ValueMember == null)
                            throw new FormException("Set 'value member' for simple select column - " + _ctrl.Title ?? _ctrl.Name);
                        if (_ctrl.DisplayMember == null)
                            throw new FormException("Set 'display member' for simple select column - " + _ctrl.Title ?? _ctrl.Name);
                    }
                    else
                    {
                        if (!(_ctrl.Options?.Count > 0))
                            throw new FormException("Set 'options' for simple select column - " + _ctrl.Title ?? _ctrl.Name);
                    }
                }
                else if (Allctrls[i] is EbExportButton _ctrl)
                {
                    if (string.IsNullOrWhiteSpace(_ctrl.FormRefId))
                    {
                        throw new FormException("Set 'Destination Form' for Export Button - " + _ctrl.Label ?? _ctrl.Name);
                    }
                }

                //--------------------------
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
                else if (Allctrls[i] is EbProvisionLocation)//One ctrl
                {
                    EbProvisionLocation provLoc = Allctrls[i] as EbProvisionLocation;
                    foreach (UsrLocFieldAbstract fld in provLoc.Fields)
                    {
                        UsrLocField _field = fld as UsrLocField;
                        if (string.IsNullOrEmpty(_field.ControlName))
                        {
                            if (_field.IsRequired)
                                throw new FormException($"Please map a control for {_field.Name} in ProvisionLocation control({provLoc.Name}).");
                            continue;
                        }
                        if (Allctrls.FirstOrDefault(e => e.Name == _field.ControlName) == null)
                            throw new FormException($"Invalid control name '{_field.ControlName}' for {_field.Name} in ProvisionLocation control({provLoc.Name}).");
                    }
                }
                else if (Allctrls[i] is EbAutoId)//One ctrl
                {
                    EbAutoId _ctrl = Allctrls[i] as EbAutoId;
                    if ((_ctrl.Pattern == null || string.IsNullOrWhiteSpace(_ctrl.Pattern.sPattern)) &&
                        (string.IsNullOrEmpty(_ctrl.Script?.Code) || (_ctrl.Script?.Lang != ScriptingLanguage.CSharp && _ctrl.Script?.Lang != ScriptingLanguage.SQL)))
                        throw new FormException($"Please enter a valid pattern or script for AutoId control.");
                    if (_ctrl.Pattern.SerialLength <= 0)
                        throw new FormException($"Please enter a valid SerialLength for AutoId control.");
                }

                if (Allctrls[i] is IEbDataReaderControl && serviceClient != null)
                    (Allctrls[i] as IEbDataReaderControl).FetchParamsMeta(serviceClient, redis, Allctrls);
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
                else if (ctrl is EbTagInput)
                {
                    if ((ctrl as EbTagInput).AutoSuggestion)
                        (ctrl as EbTagInput).TableName = _tbl;
                }

            }
        }

        public static void UpdateEbSid(EbWebForm _this, EbControl[] Allctrls, bool tsOnly)
        {
            string ts = DateTime.UtcNow.ToString("yMdHms");
            if (_this.EbSid.Contains('_') && !tsOnly)
                ts = _this.EbSid.Substring(_this.EbSid.LastIndexOf('_') + 1);

            for (int i = 0; i < Allctrls.Length; i++)
            {
                string id = Allctrls[i].EbSid;
                if (id == null)
                    continue;
                if (id.Contains('-'))
                    Allctrls[i].EbSid = ts + i + id.Substring(id.IndexOf('-'));
                else
                    Allctrls[i].EbSid = ts + i + '-' + id;
            }
        }

        //Populate Property DependedValExp
        private static void CalcValueExprDependency(EbForm _Form, Dictionary<int, EbControlWrapper> _dict)
        {
            List<int> CalcFlds = new List<int>();
            List<KeyValuePair<int, int>> dpndcy = new List<KeyValuePair<int, int>>();

            for (int i = 0; i < _dict.Count; i++)
            {
                _dict[i].Control.DependedValExp = new List<string>();
                _dict[i].Control.ValExpParams = new List<string>();

                if (!string.IsNullOrEmpty(_dict[i].Control.ValueExpr?.Code))
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
                if (_dict[CalcFlds[i]].Control.ValueExpr.Lang == ScriptingLanguage.JS)
                {
                    if (code.Contains("form"))
                    {
                        bool IsAnythingResolved = false;
                        for (int j = 0; j < _dict.Count; j++)
                        {
                            string p = _dict[j].Path, r = _dict[j].Root, n = _dict[j].Control.Name;
                            string regex = EbFormHelper.GetJsRegex(r, n, p);

                            if (Regex.IsMatch(code, regex))
                            {
                                if (CalcFlds[i] != j || _dict[j].Control.SelfTrigger)
                                    dpndcy.Add(new KeyValuePair<int, int>(CalcFlds[i], j));//<dependent, dominant>
                                IsAnythingResolved = true;
                            }
                        }
                        //if (!IsAnythingResolved)
                        //    throw new FormException($"Can't resolve some form variables in Js Value expression of {_dict[CalcFlds[i]].Control.Name}");
                    }
                }
                else if (_dict[CalcFlds[i]].Control.ValueExpr.Lang == ScriptingLanguage.SQL)
                {
                    List<Param> _params = SqlHelper.GetSqlParams(code);
                    foreach (Param _p in _params)
                    {
                        if (!EbFormHelper.IsExtraSqlParam(_p.Name, _Form.TableName))
                        {
                            KeyValuePair<int, EbControlWrapper> item = _dict.FirstOrDefault(e => e.Value.Control.Name == _p.Name);
                            if (item.Value == null)
                                throw new FormException($"Can't resolve {_p.Name} in SQL Value expression of {_dict[CalcFlds[i]].Control.Name}");

                            if (CalcFlds[i] != item.Key || _dict[item.Key].Control.SelfTrigger)
                                dpndcy.Add(new KeyValuePair<int, int>(CalcFlds[i], item.Key));//<dependent, dominant>
                            _dict[CalcFlds[i]].Control.ValExpParams.Add(item.Value.Path);
                        }
                    }
                }
            }

            foreach (int i in dpndcy.Select(e => e.Value).Distinct())
            {
                List<int> execOrder = new List<int> { i };
                GetValExpDependentsRec(execOrder, dpndcy, i);
                if (dpndcy.FindIndex(x => x.Key == i && x.Value == i) == -1)
                    execOrder.Remove(i);
                foreach (int key in execOrder)
                    _dict[i].Control.DependedValExp.Add(_dict[key].Path);
            }

            // **** Hint ****
            // A; B = A + 10; C = B + 5;
            // _dict = { { 0, A}, { 1, B}, { 2, C} }
            // dpndcy = { (B, A) (C, B) } => { (1, 0) (2, 1) }
            // A => [B, C]; B => [C];


            //Value expression execution order for DoNotPersist ctrls
            _Form.DoNotPersistExecOrder = new List<string>();//cleared the old values
            List<KeyValuePair<int, int>> DnpDpndcy = dpndcy.FindAll(x => _dict[x.Key].Control.DoNotPersist && _dict[x.Value].Control.DoNotPersist && x.Key != x.Value);
            List<int> DnpFlds = CalcFlds.FindAll(x => _dict[x].Control.DoNotPersist);
            List<int> ExecOrd = new List<int>();

            int stopCounter = 0;
            while (DnpFlds.Count > ExecOrd.Count && stopCounter < DnpFlds.Count)
            {
                for (int i = 0; i < DnpFlds.Count; i++)
                {
                    if (DnpDpndcy.FindIndex(x => x.Key == DnpFlds[i]) == -1 && !ExecOrd.Contains(DnpFlds[i]))
                    {
                        ExecOrd.Add(DnpFlds[i]);
                        DnpDpndcy.RemoveAll(x => x.Value == DnpFlds[i]);
                    }
                }
                stopCounter++;
            }

            if (DnpDpndcy.Count > 0)
                throw new FormException("Avoid circular reference by the following controls in 'ValueExpression' : " + string.Join(',', DnpDpndcy.Select(e => _dict[e.Key].Control.Name).Distinct()));

            foreach (int i in ExecOrd)
                _Form.DoNotPersistExecOrder.Add(_dict[i].Path);
        }

        private static void CalcDataReaderDependency(EbForm _Form, Dictionary<int, EbControlWrapper> _dict)
        {
            for (int i = 0; i < _dict.Count; i++)
                _dict[i].Control.DrDependents = new List<string>();

            for (int i = 0; i < _dict.Count; i++)
            {
                if (_dict[i].Control is IEbDataReaderControl && (_dict[i].Control as IEbDataReaderControl).ParamsList?.Count > 0)
                {
                    bool IsDgCtrl = _dict[i].Control is EbDGColumn;
                    foreach (Param _p in (_dict[i].Control as IEbDataReaderControl).ParamsList)
                    {
                        KeyValuePair<int, EbControlWrapper> item = _dict.FirstOrDefault(e => (!(e.Value.Control is EbDGColumn) || IsDgCtrl) && e.Value.Control.Name == _p.Name);
                        if (item.Value != null)
                        {
                            if (item.Key != i)
                                item.Value.Control.DrDependents.Add(_dict[i].Path);
                        }
                        else if (!EbFormHelper.IsExtraSqlParam(_p.Name, _Form.TableName) && _p.Name != _dict[i].Control.Name)
                            throw new FormException($"Can't resolve parameter {_p.Name} in data reader of {_dict[i].Control.Name}");
                    }
                }
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
                if (!string.IsNullOrWhiteSpace(_dict[i].Control.DefaultValueExpression?.Code))
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
                            string regex = EbFormHelper.GetJsRegex(r, n, p);

                            if (Regex.IsMatch(code, regex))
                            {
                                if (CalcFlds[i] != j)//if a control refers itself treated as not circular reference
                                    dpndcy.Add(new KeyValuePair<int, int>(CalcFlds[i], j));//<dependent, dominant>
                                IsAnythingResolved = true;
                            }
                        }
                        //if (!IsAnythingResolved)
                        //    throw new FormException($"Can't resolve some form variables in Js Default value expression of {_dict[CalcFlds[i]].Control.Name}");
                    }
                }
                else if (_dict[CalcFlds[i]].Control.DefaultValueExpression.Lang == ScriptingLanguage.SQL)
                {
                    List<Param> _params = SqlHelper.GetSqlParams(code);
                    foreach (Param _p in _params)
                    {
                        if (!EbFormHelper.IsExtraSqlParam(_p.Name, _this.TableName))
                        {
                            KeyValuePair<int, EbControlWrapper> item = _dict.FirstOrDefault(e => e.Value.Control.Name == _p.Name);
                            if (item.Value == null)
                                throw new FormException($"Can't resolve {_p.Name} in SQL Default Value expression of {_dict[CalcFlds[i]].Control.Name}");
                            if (CalcFlds[i] != item.Key)
                                dpndcy.Add(new KeyValuePair<int, int>(CalcFlds[i], item.Key));//<dependent, dominant>
                            if (!_dict[CalcFlds[i]].Control.ValExpParams.Contains(item.Value.Path))
                                _dict[CalcFlds[i]].Control.ValExpParams.Add(item.Value.Path);
                        }
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

            for (int i = ExecOrd.Count - 1; i >= 0; i--)
                _this.DefaultValsExecOrder.Add(_dict[ExecOrd[i]].Path);

            //foreach (int i in ExecOrd)
            //    _this.DefaultValsExecOrder.Add(_dict[i].Path);
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
                //if (i == 1)
                //    ;
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
                        string regex = EbFormHelper.GetJsRegex(r, n, p);
                        //if (ctrlWrap.Path != "form.datagrid1.stringcolumn2" && ctrlWrap.Path != "form.datagrid1.stringcolumn1")
                        //    ;
                        if (Regex.IsMatch(ebScript.Code, regex))
                        {
                            if (is4Hide)
                                _dict[j].Control.HiddenExpDependants.Add(ctrlWrap.Path);
                            else
                                _dict[j].Control.DisableExpDependants.Add(ctrlWrap.Path);

                            IsAnythingResolved = true;
                        }
                    }
                    //if (!IsAnythingResolved)
                    //    throw new FormException($"Can't resolve some form variables in Js {(is4Hide ? "Hide expression" : "Disable expression")} of {ctrlWrap.Control.Name}");
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
                    //if (control is EbDataGrid)
                    //{
                    //    _counter = _dict.Count;
                    //    string path2 = _path == string.Empty ? control.Name : _path + CharConstants.DOT + control.Name;
                    //    control.__path = path2;
                    //    _dict.Add(_counter++, new EbControlWrapper
                    //    {
                    //        TableName = _container.TableName,
                    //        Path = path2,
                    //        Control = control,
                    //        Root = _path
                    //    });
                    //}

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
            CalcValueExprDependency(_this, _dict);
            CalcDataReaderDependency(_this, _dict);
            ValidateNotificationProp(_this.Notifications, _dict);
        }
    }
}
