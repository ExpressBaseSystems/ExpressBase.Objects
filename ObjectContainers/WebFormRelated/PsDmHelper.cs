using ExpressBase.Common;
using ExpressBase.Common.Constants;
using ExpressBase.Common.Data;
using ExpressBase.Common.LocationNSolution;
using ExpressBase.Common.Objects;
using ExpressBase.Common.Structures;
using ExpressBase.Objects.Helpers;
using ExpressBase.Objects.ServiceStack_Artifacts;
using ServiceStack;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;

namespace ExpressBase.Objects.WebFormRelated
{
    public class PsDmHelper
    {
        private EbWebForm ebForm { get; set; }

        private List<EbControl> drPsList { get; set; }

        private WebformData _FormData { get; set; }

        private Service service { get; set; }

        public PsDmHelper(EbWebForm ebForm, List<EbControl> drPsList, WebformData _FormData, Service service)
        {
            this.ebForm = ebForm;
            this.drPsList = drPsList;
            this._FormData = _FormData;
            this.service = service;
        }

        public void UpdatePsDm_Tables()
        {
            PsDmDict _PsDmDict = new PsDmDict();
            Dictionary<string, List<int>> row_ids = new Dictionary<string, List<int>>();//key: ps EbSid 
            this.GetPsQueryAndParams(_PsDmDict, row_ids);
            _PsDmDict.ExecuteQuery(this.ebForm);
            _PsDmDict.AttachPsDmTable(this.ebForm, this._FormData, this.drPsList, row_ids);
        }

        #region SqlRetrival //for dev side

        public PsDmHelper(EbWebForm ebForm, List<EbControl> drPsList, Service service)
        {
            this.ebForm = ebForm;
            this.drPsList = drPsList;
            this.service = service;
        }

        public List<string> GetPsDmSelectQuery()
        {
            string psqry;
            List<string> QueryList = new List<string>();
            EbDataReader dataReader;

            foreach (EbControl psCtrl in this.drPsList)
            {
                List<Param> ParamsList = (psCtrl as IEbDataReaderControl).ParamsList;
                IEbPowerSelect ipsCtrl = psCtrl as IEbPowerSelect;
                if (ParamsList == null)
                {
                    throw new FormException($"Invalid ParamsList in '{psCtrl.Label ?? psCtrl.Name}'. Contact Admin.", (int)HttpStatusCode.InternalServerError, "Please file a bug", "EbWebForm");
                }
                TableSchema _pstableSchema = this.ebForm.FormSchema.Tables.Find(e => e.TableName == ipsCtrl.TableName);

                if (_pstableSchema == null)
                    throw new FormException("InternalServerError: _pstable is null", (int)HttpStatusCode.InternalServerError, "_pstableSchema is null", "PsDmHelper");
                if (_pstableSchema.TableType == WebFormTableTypes.Grid && !string.IsNullOrWhiteSpace(_pstableSchema.CustomSelectQuery))
                    throw new FormException("EnableSqlRetriver not supported for CustomSelectQuery DG PS", (int)HttpStatusCode.InternalServerError, "--", "PsDmHelper");

                (psqry, dataReader) = ipsCtrl.GetSqlAndDr(this.service);

                if (psCtrl is EbDGPowerSelectColumn dgpsCtrl && dgpsCtrl.StrictSelect)
                    throw new FormException("EnableSqlRetriver not supported for StrictSelect PS", (int)HttpStatusCode.InternalServerError, "__", "PsDmHelper");


                psqry = ReplaceParamsInQuery(psCtrl, psqry);
                psqry = WrapPsQuery(psqry, psCtrl, _pstableSchema);
                QueryList.Add(psqry);
            }
            return QueryList;
        }

        private string ReplaceParamsInQuery(EbControl psCtrl, string qry)
        {
            EbSystemColumns ebs = this.ebForm.SolutionObj.SolutionSettings.SystemColumns;

            foreach (Param _psParam in (psCtrl as IEbDataReaderControl).ParamsList)
            {
                TableSchema _p_table = null;
                ColumnSchema _p_column = null;
                foreach (TableSchema __table in this.ebForm.FormSchema.Tables)
                {
                    _p_column = __table.Columns.Find(e => e.Control.Name == _psParam.Name);
                    if (_p_column != null)
                    {
                        _p_table = __table;
                        break;
                    }
                }
                if (_p_table != null && _p_column != null)
                {
                    string _str;
                    if (_p_table.TableType == WebFormTableTypes.Grid)
                    {
                        _str = $"ANY(SELECT {_p_column.ColumnName} FROM {_p_table.TableName} WHERE {this.ebForm.FormSchema.MasterTable}_id=id__in AND COALESCE({ebs[SystemColumns.eb_del]},{ebs.GetBoolFalse(SystemColumns.eb_del)})={ebs.GetBoolFalse(SystemColumns.eb_del)})";
                    }
                    else
                    {
                        string idCol = this.ebForm.FormSchema.MasterTable == _p_table.TableName ? "id" : $"{this.ebForm.FormSchema.MasterTable}_id";
                        _str = $"(SELECT {_p_column.ColumnName} FROM {_p_table.TableName} WHERE {idCol}=id__in AND COALESCE({ebs[SystemColumns.eb_del]},{ebs.GetBoolFalse(SystemColumns.eb_del)})={ebs.GetBoolFalse(SystemColumns.eb_del)})";
                    }
                    qry = SqlHelper.ReplaceParamByValue(qry, _p_column.ColumnName, _str);
                }
                else
                {
                    string _val;

                    if (_psParam.Name == FormConstants.id || _psParam.Name == this.ebForm.TableName + FormConstants._id)
                        _val = "id__in";
                    else if (_psParam.Name == FormConstants.eb_loc_id)
                        _val = $"(SELECT {ebs[SystemColumns.eb_loc_id]} FROM {this.ebForm.TableName} WHERE id=id__in)";
                    else
                        _val = $"'{_psParam.ValueTo}'";

                    qry = SqlHelper.ReplaceParamByValue(qry, _psParam.Name, _val);
                }
            }
            return qry;
        }

        private string WrapPsQuery(string psqry, EbControl psCtrl, TableSchema _pstableSchema)
        {
            string vms;
            EbSystemColumns ebs = this.ebForm.SolutionObj.SolutionSettings.SystemColumns;
            IEbPowerSelect ipsCtrl = psCtrl as IEbPowerSelect;
            if (_pstableSchema.TableType == WebFormTableTypes.Grid)
            {
                vms = $"ANY(SELECT {psCtrl.Name} FROM {_pstableSchema.TableName} WHERE {this.ebForm.FormSchema.MasterTable}_id=id__in AND COALESCE({ebs[SystemColumns.eb_del]},{ebs.GetBoolFalse(SystemColumns.eb_del)})={ebs.GetBoolFalse(SystemColumns.eb_del)})";
            }
            else
            {
                string idCol = this.ebForm.FormSchema.MasterTable == _pstableSchema.TableName ? "id" : $"{this.ebForm.FormSchema.MasterTable}_id";
                vms = $"(SELECT {psCtrl.Name} FROM {_pstableSchema.TableName} WHERE {idCol}=id__in AND COALESCE({ebs[SystemColumns.eb_del]},{ebs.GetBoolFalse(SystemColumns.eb_del)})={ebs.GetBoolFalse(SystemColumns.eb_del)})";
                if (ipsCtrl.MultiSelect)
                    vms = $"ANY(STRING_TO_ARRAY(({vms}), ',')::INT[])";
            }

            return $"SELECT __A.* FROM ({psqry}) __A WHERE __A.{ipsCtrl.ValueMember.Name} = {vms}";
        }

        #endregion SqlRetrival

        private void GetPsQueryAndParams(PsDmDict _PsDmDict, Dictionary<string, List<int>> row_ids)
        {
            string psqry;
            int p_i = 0;
            EbDataReader dataReader;

            if (this.ebForm.EbConnectionFactory == null)
                throw new FormException("Something went wrong", (int)HttpStatusCode.InternalServerError, "ConnectionFactory is null", "PsDmHelper -> GetPsQueryAndParams");

            foreach (EbControl psCtrl in this.drPsList)
            {
                row_ids.Add(psCtrl.EbSid, new List<int>());
                List<Param> ParamsList = (psCtrl as IEbDataReaderControl).ParamsList;
                IEbPowerSelect ipsCtrl = psCtrl as IEbPowerSelect;
                if (ParamsList == null)
                {
                    Console.WriteLine($"ParamsList in PowerSelect {psCtrl.Name} is null. Trying to UpdateParamsMeta...");
                    ipsCtrl.UpdateParamsMeta(service, service.Redis);
                    ParamsList = (psCtrl as IEbDataReaderControl).ParamsList;
                    if (ParamsList == null)
                        throw new FormException($"Invalid ParamsList in '{psCtrl.Label ?? psCtrl.Name}'. Contact Admin", (int)HttpStatusCode.InternalServerError, "Save object in dev side", "EbWebForm");
                }
                TableSchema _pstable = this.ebForm.FormSchema.Tables.Find(e => e.TableName == ipsCtrl.TableName);
                if (_pstable == null)
                    throw new FormException("Something went wrong", (int)HttpStatusCode.InternalServerError, "_pstable is null", "EbWebForm");
                SingleTable psTable = this._FormData.MultipleTables.ContainsKey(_pstable.TableName) ? this._FormData.MultipleTables[_pstable.TableName] : null;
                if (psTable == null)
                    Console.WriteLine("[INFO] PS_Table is null.");
                string vm_vals = string.Empty;

                (psqry, dataReader) = ipsCtrl.GetSqlAndDr(this.service);
                IDatabase DataDB = dataReader.GetDatastore(this.ebForm.EbConnectionFactory);

                if (psCtrl is EbDGPowerSelectColumn dgpsCtrl && dgpsCtrl.StrictSelect && psTable?.Count > 0)// separate query
                {
                    _PsDmDict.TryAdd(DataDB, psCtrl);
                    foreach (SingleRow Row in psTable)
                    {
                        if (Row[psCtrl.Name] != null)
                        {
                            string temp = this.AddPsParams(psCtrl, DataDB, _PsDmDict.GetPList(DataDB), Row, ref p_i, psqry);
                            List<int> nums = Convert.ToString(Row[psCtrl.Name]).Split(",").Select(e => { return int.TryParse(e, out int ie) ? ie : 0; }).ToList();
                            _PsDmDict.AppendQuery(DataDB, GetPsDmSelectQuery(temp, ipsCtrl, nums));
                            p_i++;
                            row_ids[psCtrl.EbSid].Add(Row.RowId);
                        }
                    }
                }
                else if (psCtrl is EbDGPowerSelectColumn && psTable?.Count > 0)
                {
                    List<int> vms = new List<int>();
                    foreach (SingleRow Row in psTable)
                    {
                        if (Row[psCtrl.Name] != null)
                        {
                            List<int> nums = Convert.ToString(Row[psCtrl.Name]).Split(",").Select(e => { return int.TryParse(e, out int ie) ? ie : 0; }).ToList();
                            vms.AddRange(nums);
                        }
                    }
                    if (ParamsList.Exists(e => e.Name == psCtrl.Name))
                    {
                        if (SqlHelper.ContainsParameter(psqry, psCtrl.Name))//self parameter
                        {
                            string _str = GetPsDgParamAsQuery(psTable, psCtrl.Name);
                            psqry = SqlHelper.ReplaceParamByValue(psqry, psCtrl.Name, _str);
                        }
                    }
                    _PsDmDict.TryAdd(DataDB, psCtrl);
                    psqry = this.AddPsParams(psCtrl, DataDB, _PsDmDict.GetPList(DataDB), null, ref p_i, psqry);
                    _PsDmDict.AppendQuery(DataDB, GetPsDmSelectQuery(psqry, ipsCtrl, vms));
                    row_ids[psCtrl.EbSid].Add(0);
                }
                else if (psTable?.Count > 0)
                {
                    _PsDmDict.TryAdd(DataDB, psCtrl);
                    psqry = this.AddPsParams(psCtrl, DataDB, _PsDmDict.GetPList(DataDB), null, ref p_i, psqry);
                    List<int> nums = Convert.ToString(psTable[0][psCtrl.Name]).Split(",").Select(e => { return int.TryParse(e, out int ie) ? ie : 0; }).ToList();
                    _PsDmDict.AppendQuery(DataDB, GetPsDmSelectQuery(psqry, ipsCtrl, nums));
                    row_ids[psCtrl.EbSid].Add(0);
                }
            }
        }

        private string AddPsParams(EbControl psCtrl, IDatabase DataDB, List<DbParameter> param, SingleRow curRow, ref int i, string qry)
        {
            foreach (Param _psParam in (psCtrl as IEbDataReaderControl).ParamsList)
            {
                if (param.Exists(e => e.ParameterName == _psParam.Name))
                    continue;

                TableSchema _p_table = null;
                ColumnSchema _p_column = null;
                foreach (TableSchema __table in this.ebForm.FormSchema.Tables)
                {
                    _p_column = __table.Columns.Find(e => e.Control.Name == _psParam.Name);
                    if (_p_column != null)
                    {
                        _p_table = __table;
                        break;
                    }
                }

                if (_p_table != null && this._FormData.MultipleTables.TryGetValue(_p_table.TableName, out SingleTable Table) && Table.Count > 0)
                {
                    if (_p_table.TableType == WebFormTableTypes.Grid)
                    {
                        if (curRow != null)
                        {
                            if (!(_psParam.Name == psCtrl.Name && qry == null))
                            {
                                param.Add(DataDB.GetNewParameter(_psParam.Name + i, (EbDbTypes)Convert.ToInt32(_psParam.Type), curRow[_psParam.Name]));
                                qry = SqlHelper.RenameParameter(qry, _psParam.Name, _psParam.Name + i);
                            }
                            continue;
                        }
                        else if (_p_column.Control is EbDGPowerSelectColumn || _p_column.Control is EbDGNumericColumn)
                        {
                            string _str = GetPsDgParamAsQuery(Table, _psParam.Name);
                            qry = SqlHelper.ReplaceParamByValue(qry, _psParam.Name, _str);
                            continue;
                        }
                    }
                    else
                    {
                        param.Add(DataDB.GetNewParameter(_psParam.Name, (EbDbTypes)Convert.ToInt32(_psParam.Type), Table[0][_p_column.ColumnName]));
                        continue;
                    }
                }
                if (!EbFormHelper.IsExtraSqlParam(_psParam.Name, this.ebForm.TableName))
                    param.Add(DataDB.GetNewParameter(_psParam.Name, (EbDbTypes)Convert.ToInt32(_psParam.Type), _psParam.ValueTo));
            }
            return qry;
        }

        private string GetPsDmSelectQuery(string psqry, IEbPowerSelect ipsCtrl, List<int> vmArr)
        {
            string vms;
            vmArr = vmArr.Distinct().ToList();
            if (vmArr.Count == 0)
                vms = "0";
            else if (vmArr.Count == 1)
                vms = $"{vmArr[0]}";
            else
                vms = $"ANY(STRING_TO_ARRAY('{vmArr.Join(",")}', ',')::INT[])";

            return $"SELECT __A.* FROM ({psqry}) __A WHERE __A.{ipsCtrl.ValueMember.Name} = {vms};";
        }

        private string GetPsDgParamAsQuery(SingleTable Table, string pName)
        {
            string qryPart;
            List<int> vms = new List<int>();
            foreach (SingleRow Row in Table)
            {
                if (Row[pName] != null)
                {
                    List<int> nums = Convert.ToString(Row[pName]).Split(",").Select(e => { return int.TryParse(e, out int ie) ? ie : 0; }).ToList();
                    vms.AddRange(nums);
                }
            }
            vms = vms.Distinct().ToList();
            if (vms.Count == 0)
                qryPart = "0";
            else if (vms.Count == 1)
                qryPart = $"{vms[0]}";
            else
                qryPart = $"ANY(STRING_TO_ARRAY('{vms.Join(",")}', ',')::INT[])";

            return qryPart;
        }
    }

    public class PsDmDict
    {
        public Dictionary<IDatabase, PsDmDictItem> PsDict { get; set; }

        public PsDmDict()
        {
            this.PsDict = new Dictionary<IDatabase, PsDmDictItem>();
        }

        public void TryAdd(IDatabase DataDB, EbControl psCtrl)
        {
            if (!this.PsDict.ContainsKey(DataDB))
                this.PsDict.Add(DataDB, new PsDmDictItem());
            this.PsDict[DataDB].ControlsList.Add(psCtrl);
        }

        public List<DbParameter> GetPList(IDatabase DataDB)
        {
            return this.PsDict[DataDB].param;
        }

        public void AppendQuery(IDatabase DataDB, string query)
        {
            this.PsDict[DataDB].Query += query;
        }

        public void ExecuteQuery(EbWebForm ebForm)
        {
            foreach (KeyValuePair<IDatabase, PsDmDictItem> item in this.PsDict)
            {
                EbFormHelper.AddExtraSqlParams(item.Value.param, item.Key, ebForm.TableName, ebForm.TableRowId, ebForm.LocationId, ebForm.UserObj.UserId);
                item.Value.dsIndex = 0;
                if (ebForm.DbConnection == null || ebForm.EbConnectionFactory.DataDB != item.Key)
                    item.Value.ds = item.Key.DoQueries(item.Value.Query, item.Value.param.ToArray());
                else
                    item.Value.ds = item.Key.DoQueries(ebForm.DbConnection, item.Value.Query, item.Value.param.ToArray());
            }
        }

        public void AttachPsDmTable(EbWebForm ebForm, WebformData _FormData, List<EbControl> drPsList, Dictionary<string, List<int>> row_ids)
        {
            for (int x = 0; x < drPsList.Count; x++)
            {
                KeyValuePair<IDatabase, PsDmDictItem> item = this.PsDict.FirstOrDefault(e => e.Value.ControlsList.Find(ee => ee.EbSid == drPsList[x].EbSid) != null);
                if (item.Value != null && item.Value.ds?.Tables?.Count > item.Value.dsIndex)
                {
                    for (int y = 0; y < row_ids[drPsList[x].EbSid].Count; y++)
                    {
                        SingleTable Table = new SingleTable();
                        ebForm.GetFormattedData(item.Value.ds.Tables[item.Value.dsIndex], Table);
                        string key = drPsList[x].EbSid;
                        if (drPsList[x] is EbDGPowerSelectColumn dgpsCtrl && dgpsCtrl.StrictSelect)
                            key = drPsList[x].EbSid + row_ids[drPsList[x].EbSid][y];
                        _FormData.PsDm_Tables.Add(key, Table);
                        item.Value.dsIndex++;
                    }
                }
            }
        }
    }

    public class PsDmDictItem
    {
        public List<DbParameter> param { get; set; }

        public List<EbControl> ControlsList { get; set; }

        public string Query { get; set; }

        public EbDataSet ds { get; set; }

        public int dsIndex { get; set; }

        public PsDmDictItem()
        {
            this.param = new List<DbParameter>();
            this.ControlsList = new List<EbControl>();
            this.Query = string.Empty;
        }
    }
}

