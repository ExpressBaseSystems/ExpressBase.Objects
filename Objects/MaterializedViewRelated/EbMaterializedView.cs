using ExpressBase.Common;
using ExpressBase.Common.Constants;
using ExpressBase.Common.JsonConverters;
using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using ExpressBase.Objects.ServiceStack_Artifacts;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;

namespace ExpressBase.Objects
{
    public abstract class EbMaterializedViewBase : EbObject
    {
        public override List<string> DiscoverRelatedRefids()
        {
            return new List<string>();
        }
    }

    [EnableInBuilder(BuilderType.MaterializedView)]
    [BuilderTypeEnum(BuilderType.MaterializedView)]
    public class EbMaterializedView : EbMaterializedViewBase, IEBRootObject
    {
        [EnableInBuilder(BuilderType.MaterializedView)]
        [HideInPropertyGrid]
        public override string RefId { get; set; }

        [EnableInBuilder(BuilderType.MaterializedView)]
        public override string DisplayName { get; set; }

        [EnableInBuilder(BuilderType.MaterializedView)]
        public override string Name { get; set; }

        [EnableInBuilder(BuilderType.MaterializedView)]
        public override string Description { get; set; }

        public bool HideInMenu { get; set; }

        [EnableInBuilder(BuilderType.MaterializedView)]
        [HideInPropertyGrid]
        public override string VersionNumber { get; set; }

        [EnableInBuilder(BuilderType.MaterializedView)]
        [HideInPropertyGrid]
        public override string Status { get; set; }

        [EnableInBuilder(BuilderType.MaterializedView)]
        [HideInPropertyGrid]
        [JsonConverter(typeof(Base64Converter))]
        public string Sql { get; set; }

        [EnableInBuilder(BuilderType.MaterializedView)]
        [PropertyGroup(PGConstants.CORE)]
        [InputMask("[a-z][a-z0-9]*(_[a-z0-9]+)*")]
        public string TableName { get; set; }

        [EnableInBuilder(BuilderType.MaterializedView)]
        [PropertyGroup(PGConstants.CORE)]
        [InputMask("[a-z][a-z0-9]*(_[a-z0-9]+)*")]
        public string KeyColumnName { get; set; }
    }

    public class EbMaterializedViewConfig
    {
        private EbWebForm ebWebForm { get; set; }

        private EbMaterializedView MatViewObj { get; set; }

        private DateTime UpToDate { get; set; }

        public EbMaterializedViewConfig(EbWebForm webForm)
        {
            this.ebWebForm = webForm;
        }

        public void SetMatViewObject(EbMaterializedView ebMatView)
        {
            this.MatViewObj = ebMatView;
            string date_s = ebWebForm.SolutionObj?.SolutionSettings?.MaterializedViewDate;
            if (date_s != null)
                this.UpToDate = DateTime.ParseExact(date_s, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            else
                this.UpToDate = DateTime.MinValue;
        }

        public string GetInsertModeQuery(bool useFormDataBackup)
        {
            if (this.MatViewObj == null) return string.Empty;

            string Qry = string.Empty;
            WebformData FormData = useFormDataBackup ? this.ebWebForm.FormDataBackup : this.ebWebForm.FormData;

            if (FormData == null)
                throw new FormException("Unable to process materialized view. FormData is null.", (int)HttpStatusCode.InternalServerError, $"Form name: {this.ebWebForm.DisplayName}; useFormDataBackup: {useFormDataBackup}", "EbMaterializedView -> GetInsertModeQuery");

            (SingleColumn, SingleRow, SingleTable) d = this.GetDataColumnByControlName(this.ebWebForm.MatViewDateCtrl, FormData);
            if (d.Item1 != null && d.Item1.Value != null)
            {
                DateTime _date = Convert.ToDateTime(d.Item1.Value);
                if (this.UpToDate >= _date)
                {
                    (SingleColumn, SingleRow, SingleTable) k = this.GetDataColumnByControlName(this.ebWebForm.MatViewKeyCtrl, FormData);
                    if (k.Item1 != null)
                    {
                        List<int> list = new List<int>();
                        foreach (SingleRow Row in k.Item3)
                        {
                            SingleColumn Column = Row.GetColumn(this.ebWebForm.MatViewKeyCtrl);
                            if (Column.Value != null && int.TryParse(Convert.ToString(Column.Value), out int t) && !list.Contains(t))
                            {
                                list.Add(t);
                            }
                        }
                        if (list.Count > 0)
                        {
                            foreach (int t in list)
                            {
                                Qry += this.GetUpsertQuery(_date, t, this.ebWebForm.LocationId);
                            }
                        }
                    }
                }
            }
            return Qry;
        }

        public string GetUpdateModeQuery()
        {
            if (this.MatViewObj == null) return string.Empty;

            string Qry = string.Empty;

            if (this.ebWebForm.FormData == null)
                throw new FormException("Unable to process materialized view. FormData is null.", (int)HttpStatusCode.InternalServerError, $"Form name: {this.ebWebForm.DisplayName}", "EbMaterializedView -> GetUpdateModeQuery");
            if (this.ebWebForm.FormDataBackup == null)
                throw new FormException("Unable to process materialized view. FormDataBackup is null.", (int)HttpStatusCode.InternalServerError, $"Form name: {this.ebWebForm.DisplayName}", "EbMaterializedView -> GetUpdateModeQuery");

            (SingleColumn, SingleRow, SingleTable) d = this.GetDataColumnByControlName(this.ebWebForm.MatViewDateCtrl, this.ebWebForm.FormData);
            (SingleColumn, SingleRow, SingleTable) od = this.GetDataColumnByControlName(this.ebWebForm.MatViewDateCtrl, this.ebWebForm.FormDataBackup);
            if (d.Item1 != null && od.Item1 != null)
            {
                List<int> list = new List<int>();
                DateTime _date1 = Convert.ToDateTime(d.Item1.Value);
                DateTime _date2 = Convert.ToDateTime(od.Item1.Value);

                if (this.UpToDate < _date1 && this.UpToDate < _date2) return string.Empty;

                int oldLocId = this.ebWebForm.FormDataBackup.MultipleTables[this.ebWebForm.FormSchema.MasterTable][0].LocId;
                bool dateChanged = _date1.Year != _date2.Year || _date1.Month != _date2.Month;
                bool locChanged = this.ebWebForm.LocationId != oldLocId;

                (SingleColumn, SingleRow, SingleTable) k = this.GetDataColumnByControlName(this.ebWebForm.MatViewKeyCtrl, this.ebWebForm.FormData);
                (SingleColumn, SingleRow, SingleTable) ok = this.GetDataColumnByControlName(this.ebWebForm.MatViewKeyCtrl, this.ebWebForm.FormDataBackup);

                if (dateChanged || locChanged)
                {
                    this.TryAddKeyData(k.Item3, list);
                    this.TryAddKeyData(ok.Item3, list);
                }
                else
                {
                    Dictionary<int, string> new_pairs = this.GetKeyAndComputeValues(k.Item3);
                    Dictionary<int, string> old_pairs = this.GetKeyAndComputeValues(ok.Item3);

                    foreach (int except_ele in new_pairs.Keys.Except(old_pairs.Keys).Union(old_pairs.Keys.Except(new_pairs.Keys)))
                        list.Add(except_ele);

                    foreach (int intersect_ele in new_pairs.Keys.Intersect(old_pairs.Keys))
                    {
                        if (new_pairs[intersect_ele] != old_pairs[intersect_ele] && !list.Contains(intersect_ele))
                            list.Add(intersect_ele);
                    }
                }

                if (list.Count > 0)
                {
                    foreach (int t in list)
                    {
                        if (dateChanged && locChanged)
                        {
                            if (this.UpToDate >= _date1)
                            {
                                Qry += this.GetUpsertQuery(_date1, t, this.ebWebForm.LocationId);
                                Qry += this.GetUpsertQuery(_date1, t, oldLocId);
                            }
                            if (this.UpToDate >= _date2)
                            {
                                Qry += this.GetUpsertQuery(_date2, t, this.ebWebForm.LocationId);
                                Qry += this.GetUpsertQuery(_date2, t, oldLocId);
                            }
                        }
                        else if (dateChanged)
                        {
                            if (this.UpToDate >= _date1)
                                Qry += this.GetUpsertQuery(_date1, t, this.ebWebForm.LocationId);
                            if (this.UpToDate >= _date2)
                                Qry += this.GetUpsertQuery(_date2, t, this.ebWebForm.LocationId);
                        }
                        else if (locChanged)
                        {
                            Qry += this.GetUpsertQuery(_date1, t, this.ebWebForm.LocationId);
                            Qry += this.GetUpsertQuery(_date1, t, oldLocId);
                        }
                        else
                        {
                            Qry += this.GetUpsertQuery(_date1, t, this.ebWebForm.LocationId);
                        }
                    }
                }

            }
            return Qry;
        }

        private void TryAddKeyData(SingleTable Table, List<int> list)
        {
            foreach (SingleRow Row in Table)
            {
                SingleColumn Column = Row.GetColumn(this.ebWebForm.MatViewKeyCtrl);
                if (Column.Value != null && int.TryParse(Convert.ToString(Column.Value), out int t) && !list.Contains(t))
                {
                    list.Add(t);
                }
            }
        }

        private string GetUpsertQuery(DateTime _date, int KeyVal, int locId)
        {
            string Qry = $"UPDATE {this.MatViewObj.TableName} SET eb_invalid=TRUE WHERE iyear={_date.Year} AND imonth={_date.Month} AND eb_loc_id={locId} AND {this.MatViewObj.KeyColumnName}={KeyVal};";
            Qry += $"INSERT INTO {this.MatViewObj.TableName} ({this.MatViewObj.KeyColumnName}, eb_loc_id, iyear, imonth, eb_invalid) " +
                $"SELECT {KeyVal}, {locId}, {_date.Year}, {_date.Month}, TRUE WHERE NOT EXISTS (SELECT 1 FROM {this.MatViewObj.TableName} WHERE iyear={_date.Year} AND imonth={_date.Month} AND eb_loc_id={locId} AND {this.MatViewObj.KeyColumnName}={KeyVal});";

            return Qry;
        }

        private Dictionary<int, string> GetKeyAndComputeValues(SingleTable Table)
        {
            Dictionary<int, string> new_pairs = new Dictionary<int, string>();
            foreach (SingleRow Row in Table)
            {
                SingleColumn Column = Row.GetColumn(this.ebWebForm.MatViewKeyCtrl);
                if (Column.Value != null && int.TryParse(Convert.ToString(Column.Value), out int t))
                {
                    string st = string.Empty;
                    foreach (AssociatedCtrl a in this.ebWebForm.MatViewComputeCtrls)
                    {
                        SingleColumn aColumn = Row.GetColumn(a.ControlName);
                        st += Convert.ToString(aColumn.Value);
                    }
                    if (new_pairs.ContainsKey(t))
                        new_pairs[t] = new_pairs[t] + st;
                    else
                        new_pairs[t] = st;
                }
            }
            return new_pairs;
        }

        private (SingleColumn, SingleRow, SingleTable) GetDataColumnByControlName(string ctrlName, WebformData FormData)
        {
            foreach (TableSchema _table in this.ebWebForm.FormSchema.Tables.FindAll(e => e.TableType != WebFormTableTypes.Review && !e.DoNotPersist))
            {
                if (!FormData.MultipleTables.ContainsKey(_table.TableName))
                    continue;

                foreach (SingleRow row in FormData.MultipleTables[_table.TableName])
                {
                    SingleColumn Column = row.GetColumn(ctrlName);
                    if (Column != null)
                    {
                        return (Column, row, FormData.MultipleTables[_table.TableName]);
                    }
                }
            }
            return (null, null, null);
        }
    }
}
