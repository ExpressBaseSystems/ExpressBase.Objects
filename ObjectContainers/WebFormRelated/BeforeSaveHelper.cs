using ExpressBase.Common.Constants;
using ExpressBase.Common.Extensions;
using ExpressBase.Common.Objects;
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
            EbReview ebReviewCtrl = null;
            if (string.IsNullOrEmpty(_this.TableName))
                throw new FormException("Please enter a valid form table name");
            tbls.Add(_this.TableName, "form table");
            EbControl[] Allctrls = _this.Controls.FlattenAllEbControls();
            for (int i = 0; i < Allctrls.Length; i++)
            {
                if (Allctrls[i] is EbApproval)
                {
                    string _tn = (Allctrls[i] as EbApproval).TableName;
                    if (string.IsNullOrEmpty(_tn))
                        throw new FormException("Please enter a valid table name for " + Allctrls[i].Label + " (approval control)");
                    if (tbls.ContainsKey(_tn))
                        throw new FormException(string.Format("Same table not allowed for {1} and {2}(approval control) : {0}", _tn, tbls[_tn], Allctrls[i].Label));
                    tbls.Add(_tn, Allctrls[i].Label + "(approval control)");
                }
                else if (Allctrls[i] is EbReview)
                {
                    if (ebReviewCtrl != null)
                        throw new FormException("Only one review control is allowed");
                    ebReviewCtrl = Allctrls[i] as EbReview;
                }
                else if (Allctrls[i] is EbDataGrid)
                {
                    EbDataGrid DataGrid = Allctrls[i] as EbDataGrid;
                    string _tn = (DataGrid).TableName;
                    if (string.IsNullOrEmpty((DataGrid).TableName))
                        throw new FormException("Please enter a valid table name for " + Allctrls[i].Label + " (data grid)");
                    if (tbls.ContainsKey(_tn))
                        throw new FormException(string.Format("Same table not allowed for {1} and {2}(data grid) : {0}", _tn, tbls[_tn], Allctrls[i].Label));
                    tbls.Add(_tn, Allctrls[i].Label + "(data grid)");

                    for (int j = 0; j < (DataGrid).Controls.Count; j++)
                    {
                        if (DataGrid.Controls[j] is EbDGUserControlColumn)
                        {
                            EbDGColumn DGColumn = (DataGrid).Controls[j] as EbDGColumn;

                            (DataGrid.Controls[j] as EbDGUserControlColumn).Columns = new List<EbControl>();

                        }
                    }

                    if (!DataGrid.DataSourceId.IsNullOrEmpty() && serviceClient != null)
                        DataGrid.InitDSRelated(serviceClient, redis, Allctrls);

                }
                else if (Allctrls[i] is EbProvisionUser && serviceClient != null)
                {
                    CheckEmailConAvailableResponse Resp = serviceClient.Post<CheckEmailConAvailableResponse>(new CheckEmailConAvailableRequest { });
                    if (!Resp.ConnectionAvailable)
                        throw new FormException("Please configure a email connection, it is required for ProvisionUser control.");
                }
            }

            PerformRequirdUpdate(_this, _this.TableName);

            Dictionary<int, EbControlWrapper> _dict = new Dictionary<int, EbControlWrapper>();
            GetControlsAsDict(_this, "form", _dict);
            CalcValueExprDependency(_this, _dict);

            if (ebReviewCtrl != null)
            {
                foreach (EbReviewStage stage in ebReviewCtrl.FormStages)
                {
                    if (stage.ApproverEntity == ApproverEntityTypes.Users)
                    {
                        Dictionary<string, string> QryParms = new Dictionary<string, string>();//<param, table>
                        string code = stage.ApproverUsers.Code;
                        if (code.IsNullOrEmpty())
                            throw new FormException($"Required SQL query for {ebReviewCtrl.Name}(review) control stage {stage.Name}");
                        MatchCollection matchColl = Regex.Matches(code, @"(?<=@)(\w+)|(?<=:)(\w+)");
                        foreach (Match match in matchColl)
                        {
                            KeyValuePair<int, EbControlWrapper> item = _dict.FirstOrDefault(e => e.Value.Control.Name == match.Value);
                            if (item.Value == null)
                                throw new FormException($"Can't resolve {match.Value} in {ebReviewCtrl.Name}(review) control's SQL query of stage {stage.Name}");
                            if (QryParms.ContainsKey(item.Value.Control.Name))
                                QryParms.Add(item.Value.Control.Name, item.Value.TableName);
                        }
                        stage.QryParams = QryParms;
                    }
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
                else if (ctrl is EbPowerSelect)
                {
                    EbPowerSelect _ctrl = ctrl as EbPowerSelect;
                    if (_ctrl.DataSourceId.IsNullOrEmpty())
                        throw new FormException("Set Data Reader for " + ctrl.Label);
                    if (_ctrl.ValueMember == null)
                        throw new FormException("Set Value Member for " + ctrl.Label);
                    if (_ctrl.RenderAsSimpleSelect && _ctrl.DisplayMember == null)
                        throw new FormException("Set Display Member for " + ctrl.Label);
                    else if (_ctrl.DisplayMembers == null || _ctrl.DisplayMembers.Count == 0)
                        throw new FormException("Set Display Members for " + ctrl.Label);
                }
                else if (ctrl is EbDGPowerSelectColumn)
                {
                    EbDGPowerSelectColumn _ctrl = ctrl as EbDGPowerSelectColumn;
                    if (_ctrl.DataSourceId.IsNullOrEmpty())
                        throw new FormException("Set Data Reader for " + ctrl.Name);
                    if (_ctrl.ValueMember == null)
                        throw new FormException("Set Value Member for " + ctrl.Name);
                    if (_ctrl.RenderAsSimpleSelect && _ctrl.DisplayMember == null)
                        throw new FormException("Set Display Member for " + ctrl.Name);
                    else if (!_ctrl.RenderAsSimpleSelect && (_ctrl.DisplayMembers == null || _ctrl.DisplayMembers.Count == 0))
                        throw new FormException("Set Display Members for " + ctrl.Name);
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
                    if (!(ctrl as EbControlContainer).TableName.IsNullOrEmpty())
                        t = (ctrl as EbControlContainer).TableName;
                    PerformRequirdUpdate(ctrl as EbControlContainer, t);
                }
            }
        }

        //Populate Property DependedValExp
        private static void CalcValueExprDependency(EbWebForm _this, Dictionary<int, EbControlWrapper> _dict)
        {
            List<int> CalcFlds = new List<int>();
            List<KeyValuePair<int, int>> dpndcy = new List<KeyValuePair<int, int>>();
            List<int> ExeOrd = new List<int>();

            for (int i = 0; i < _dict.Count; i++)
            {
                _dict[i].Control.DependedValExp.Clear();
                _dict[i].Control.ValExpParams.Clear();

                if (_dict[i].Control.ValueExpr != null && !string.IsNullOrEmpty(_dict[i].Control.ValueExpr.Code))
                {
                    CalcFlds.Add(i);
                    ExeOrd.Add(i);
                }
                if (_dict[i].Control.OnChangeFn != null && !string.IsNullOrEmpty(_dict[i].Control.OnChangeFn.Code))
                {
                    if (_dict[i].Control.OnChangeFn.Code.Contains(".setValue("))
                    {
                        throw new FormException("SetValue is not allowed in OnChange expression of " + _dict[i].Control.Name);
                    }
                }
            }

            for (int i = 0; i < CalcFlds.Count; i++)
            {
                string code = _dict[CalcFlds[i]].Control.ValueExpr.Code.ToLower();
                if (_dict[CalcFlds[i]].Control.ValueExpr.Lang == ScriptingLanguage.JS)
                {
                    //MatchCollection matchColl = Regex.Matches(code, $@"(form.(\w+.currentrow\[""\w + ""\]|\w+.currentrow\['\w+'\]|\w+.currentrow.\w+|\w+))"); 

                    if (code.Contains("form"))
                    {
                        bool IsAnythingResolved = false;
                        for (int j = 0; j < _dict.Count; j++)
                        {
                            string p = _dict[j].Path, r = _dict[j].Root, n = _dict[j].Control.Name;
                            //string regex = $@"{r}.currentrow\[""{n}""\]|{r}.currentrow\['{n}'\]|{r}.currentrow.{n}|{r}.getrowbyindex\(\w+\)\[""{n}""\]|{r}.getrowbyindex\(\w+\)|{p}";
                            string regex = $@"{r}.currentrow\[""{n}""\]|{r}.currentrow\['{n}'\]|{r}.currentrow.{n}|{r}.getrowbyindex\(\w+\)\[""{n}""\]|{p}";

                            if (Regex.IsMatch(code, regex))
                            {
                                if (CalcFlds[i] != j)//if a control refers itself treated as not circular reference
                                    dpndcy.Add(new KeyValuePair<int, int>(CalcFlds[i], j));//<dependent, dominant>
                                IsAnythingResolved = true;
                            }

                            //string[] stringArr = new string[] {
                            //    _dict[j].Path,
                            //    _dict[j].Root + ".currentrow." + _dict[j].Control.Name + ".",
                            //    _dict[j].Root + ".currentrow['" + _dict[j].Control.Name + "']",
                            //    _dict[j].Root + ".currentrow[\"" + _dict[j].Control.Name + "\"]",
                            //    _dict[j].Root + "." +  _dict[j].Control.Name + "_sum"
                            //};
                            //if (stringArr.Any(code.Contains))
                            //{
                            //    //if (CalcFlds[i] == j)
                            //    //    throw new FormException("Avoid circular reference by the following control in 'ValueExpression' : " + _dict[CalcFlds[i]].Control.Name);
                            //    if (CalcFlds[i] != j)//if a control refers itself treated as not circular reference
                            //        dpndcy.Add(new KeyValuePair<int, int>(CalcFlds[i], j));//<dependent, dominant>
                            //}
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

                        if (CalcFlds[i] != item.Key)
                            dpndcy.Add(new KeyValuePair<int, int>(CalcFlds[i], item.Key));//<dependent, dominant>
                        _dict[CalcFlds[i]].Control.ValExpParams.Add(item.Value.Path);
                    }

                    //for (int j = 0; j < _dict.Count; j++)
                    //{
                    //    if (code.Contains("@" + _dict[j].Control.Name) || code.Contains(":" + _dict[j].Control.Name))
                    //    {
                    //        if (CalcFlds[i] != j)
                    //            dpndcy.Add(new KeyValuePair<int, int>(CalcFlds[i], j));//<dependent, dominant>
                    //        _dict[CalcFlds[i]].Control.ValExpParams.Add(_dict[j].Path);
                    //    }
                    //}
                }
            }

            int stopCounter = 0;
            while (dpndcy.Count > 0 && stopCounter < CalcFlds.Count)
            {
                for (int i = 0; i < CalcFlds.Count; i++)
                {
                    if (dpndcy.FindIndex(x => x.Value == CalcFlds[i]) == -1)
                    {
                        bool isProcessed = false;
                        foreach (KeyValuePair<int, int> item in dpndcy.Where(e => e.Key == CalcFlds[i]))
                        {
                            _dict[item.Value].Control.DependedValExp.Remove(_dict[item.Key].Path);
                            _dict[item.Value].Control.DependedValExp.Insert(0, _dict[item.Key].Path);
                            ExeOrd.Remove(item.Value);
                            ExeOrd.Insert(0, item.Value);
                            isProcessed = true;
                        }
                        if (isProcessed)
                            dpndcy.RemoveAll(x => x.Key == CalcFlds[i]);
                    }
                }
                stopCounter++;
            }
            if (dpndcy.Count > 0)
            {
                throw new FormException("Avoid circular reference by the following controls in 'ValueExpression' : " + string.Join(',', dpndcy.Select(e => _dict[e.Key].Control.Name).Distinct()));
            }
            else
            {
                FillDependedCtrlRec(_dict, ExeOrd);
            }
        }

        //To populate multilevel DependedValExp property
        private static void FillDependedCtrlRec(Dictionary<int, EbControlWrapper> _dict, List<int> ExeOrd)
        {
            for (int i = ExeOrd.Count - 1; i >= 0; i--)
            {
                List<string> extList = new List<string>();
                foreach (string item in _dict[ExeOrd[i]].Control.DependedValExp)
                {
                    EbControlWrapper ctrlWrap = _dict.Values.FirstOrDefault(e => e.Path.Equals(item));
                    foreach (var path in ctrlWrap.Control.DependedValExp)
                    {
                        if (!_dict[ExeOrd[i]].Control.DependedValExp.Contains(path) && !extList.Contains(path))
                            extList.Add(path);
                    }
                }
                _dict[ExeOrd[i]].Control.DependedValExp.AddRange(extList);
            }
        }

        //get controls in webform as a single dimensional structure 
        public static void GetControlsAsDict(EbControlContainer _container, string _path, Dictionary<int, EbControlWrapper> _dict)
        {
            int _counter = _dict.Count;
            IEnumerable<EbControl> FlatCtrls = _container.Controls.Get1stLvlControls();
            foreach (EbControl control in FlatCtrls)
            {
                string path = _path == string.Empty ? control.Name : _path + CharConstants.DOT  + control.Name;
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

    }
}
