using ExpressBase.Common;
using ExpressBase.Common.Data;
using ExpressBase.Common.Objects.Attributes;
using ExpressBase.Objects.ServiceStack_Artifacts;
using ServiceStack.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExpressBase.Objects.Objects.SqlJobRelated
{
    public abstract class EbSqlJobWrapper : EbObject
    {

    }
    public class EbSqlJob : EbSqlJobWrapper, IEBRootObject
    {
        [HideInPropertyGrid]
        public override string RefId { get; set; }

        public override string DisplayName { get; set; }

        public override string Description { get; set; }

        public override string VersionNumber { get; set; }

        public override string Status { get; set; }

        public SqlJobTypes Type { get; set; }

        public OrderedList Resources { set; get; }

        public override List<string> DiscoverRelatedRefids()
        {
            List<string> refids = new List<string>();
            return refids;
        }

        public override void ReplaceRefid(Dictionary<string, string> RefidMap)
        {
        }
    }
    public class OrderedList : List<SqlJobResources>
    {
        public OrderedList()
        {
            this.Sort((x, y) => x.RouteIndex.CompareTo(y.RouteIndex));
        }

        //public SqlJobResources GetPreviousResource(int index, int parentindex)
        //{
        //    SqlJobResources _prev;
        //    if (index != 0)
        //        if (this[parentindex] is EbLoop)
        //            _prev = (this[parentindex] as EbLoop).InnerResources[index - 1];
        //        else
        //            _prev= 
        //}
    }

    public enum SqlJobTypes
    {
        UserInitiated,
        Scheduled
    }
    public class EbLoop : SqlJobResources, ISqlJobCollection
    {
        public OrderedList InnerResources { get; set; }
        public override List<Param> GetOutParams(List<Param> _param, int step)
        {
            List<Param> OutParams;
            if (this.InnerResources[0] is ISqlJobCollection)
                OutParams = ((this.InnerResources[0] as ISqlJobCollection).InnerResources[step-1]).GetOutParams(_param, step);
            else
                OutParams = this.InnerResources[step - 1].GetOutParams(_param, step);

            //foreach (Param p in OutParams)
            //{
            //    p.Value = (this.Result as Param).Value;
            //}
            return OutParams;
        }
    }

    public class EbTransaction : SqlJobResources, ISqlJobCollection
    {
        public OrderedList InnerResources { get; set; }

        public override List<Param> GetOutParams(List<Param> _param, int step)
        {
            List<Param> OutParams;
            if (this.InnerResources[0] is ISqlJobCollection)
                OutParams = ((this.InnerResources[0] as ISqlJobCollection).InnerResources[step - 1]).GetOutParams(_param, step);
            else
                OutParams = this.InnerResources[step - 1].GetOutParams(_param, step);

            foreach (Param p in OutParams)
            {
                p.Value = (this.Result as Param).Value;
            }
            return _param;
        }
    }

    public abstract class SqlJobResources : EbSqlJobWrapper
    {

        public int RouteIndex { set; get; }

        public string Label { set; get; }

        public virtual string Reference { set; get; }

        public object Result { set; get; }

        public virtual List<Param> GetOutParams(List<Param> _param, int step) { return new List<Param>(); }

        public virtual object GetResult() { return this.Result; }

        public virtual EbDataColumn GetColumn(int index, string cname) { return null; }

        public virtual object GetColVal(int index, string cname) { return null; }
    }

    public class EbSqlJobReader : SqlJobResources
    {
        public override string Reference { get; set; }

        public string RefName { set; get; }

        public string Version { set; get; }

        public override List<Param> GetOutParams(List<Param> _param, int step)
        {
            List<Param> p = new List<Param>();
            foreach (EbDataTable table in (this.Result as EbDataSet).Tables)
            {
                if (table.Rows.Count > 0)
                {
                    string[] c = _param.Select(item => item.Name).ToArray();
                    foreach (EbDataColumn cl in table.Columns)
                    {
                        if (c.Contains(cl.ColumnName))
                            p.Add(new Param { Name = cl.ColumnName, Type = cl.Type.ToString(), Value = table.Rows[0][cl.ColumnIndex].ToString() });
                    }
                }
            }
            return p;
        }

        public override object GetResult()
        {
            return this.Result;
        }

        public override EbDataColumn GetColumn(int index, string cname)
        {
            return (this.Result as EbDataSet).Tables[index].Columns[cname];
        }

        public override object GetColVal(int index, string cname)
        {
            return (this.Result as EbDataSet).Tables[index].Rows;
        }
    }

    public class EbSqlJobWriter : SqlJobResources
    {
        public override string Reference { get; set; }

        public string RefName { set; get; }

        public string Version { set; get; }
    }

    public interface ISqlJobCollection
    {
        OrderedList InnerResources { get; set; }
    }

    [RuntimeSerializable]
    public class SqlJobScript
    {
        public Type ResultType { get { return this.Data.GetType(); } }

        public string Data { set; get; }
    }
}
