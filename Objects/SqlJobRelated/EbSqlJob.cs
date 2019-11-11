using ExpressBase.Common;
using ExpressBase.Common.Data;
using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using ExpressBase.Common.Structures;
using ExpressBase.Objects.ServiceStack_Artifacts;
using ServiceStack.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExpressBase.Objects
{
    public abstract class EbSqlJobWrapper : EbObject
    {

    }

    [EnableInBuilder(BuilderType.SqlJob)]
    [BuilderTypeEnum(BuilderType.SqlJob)]
    public class EbSqlJob : EbSqlJobWrapper, IEBRootObject
    {

        [EnableInBuilder(BuilderType.SqlJob)]
        [HideInPropertyGrid]
        public override string RefId { get; set; }

        [EnableInBuilder(BuilderType.SqlJob)]
        public override string DisplayName { get; set; }

        [EnableInBuilder(BuilderType.SqlJob)]
        public override string Description { get; set; }

        [EnableInBuilder(BuilderType.SqlJob)]
        [HideInPropertyGrid]
        public override string VersionNumber { get; set; }


        [EnableInBuilder(BuilderType.SqlJob)]
        [HideInPropertyGrid]
        public override string Status { get; set; }

        [EnableInBuilder(BuilderType.SqlJob)]
        [HideInPropertyGrid]
        public SqlJobTypes Type { get; set; }

        [EnableInBuilder(BuilderType.SqlJob)]
        [HideInPropertyGrid]
        public OrderedList Resources { set; get; }

        [EnableInBuilder(BuilderType.SqlJob)]
        public List<string> FirstReaderKeyColumns { get; set; }

        [EnableInBuilder(BuilderType.SqlJob)]
        public List<string> ParameterKeyColumns { get; set; }

        public LoopLocation GetLoop()
        {
            for (int i = 0; i < Resources.Count; i++)
            {
                if (Resources[i] is ISqlJobCollection)
                {
                    if (Resources[i] is EbLoop)
                        return new LoopLocation { Loop = Resources[i] as EbLoop, Step = i, ParentIndex = i };
                    else
                    {
                        for (int j = 0;  j < (Resources[i] as ISqlJobCollection).InnerResources.Count; j++)
                       
                        {
                            if ((Resources[i] as ISqlJobCollection).InnerResources[j] is EbLoop)
                                return new LoopLocation { Loop = (Resources[i] as ISqlJobCollection).InnerResources[j] as EbLoop, Step = j, ParentIndex = i };
                        }
                    }
                }
            }
            return null;
        }

        public override List<string> DiscoverRelatedRefids()
        {
            List<string> refids = new List<string>();
            return refids;
        }

        public override void ReplaceRefid(Dictionary<string, string> RefidMap)
        {
        }
    }

    public class OrderedList : List<SqlJobResource>
    {
        public OrderedList()
        {
            this.Sort((x, y) => x.RouteIndex.CompareTo(y.RouteIndex));
        }
    }

    public enum SqlJobTypes
    {
        UserInitiated,
        Scheduled
    }

    public class EbLoop : SqlJobResource, ISqlJobCollection
    {
        [EnableInBuilder(BuilderType.SqlJob)]
        [HideInPropertyGrid]
        public OrderedList InnerResources { get; set; }
        public override List<Param> GetOutParams(List<Param> _param, int step)
        {
            List<Param> OutParams;
            if (this.InnerResources[0] is ISqlJobCollection)
                OutParams = ((this.InnerResources[0] as ISqlJobCollection).InnerResources[step - 1]).GetOutParams(_param, step);
            else
                OutParams = this.InnerResources[step - 1].GetOutParams(_param, step);
 
            return OutParams;
        }
    }

    public class EbTransaction : SqlJobResource, ISqlJobCollection
    {
        [EnableInBuilder(BuilderType.SqlJob)]
        [HideInPropertyGrid]
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

    public abstract class SqlJobResource : EbSqlJobWrapper
    {
        [EnableInBuilder(BuilderType.SqlJob)]
        public int RouteIndex { set; get; }

        [EnableInBuilder(BuilderType.SqlJob)]
        [HideInPropertyGrid]
        public string Label { set; get; }

        [EnableInBuilder(BuilderType.SqlJob)]
        [HideInPropertyGrid]
        public virtual string Reference { set; get; }

        [EnableInBuilder(BuilderType.SqlJob)]
        [HideInPropertyGrid]
        public object Result { set; get; }

        public virtual List<Param> GetOutParams(List<Param> _param, int step) { return new List<Param>(); }

        public virtual object GetResult() { return this.Result; }

        public virtual EbDataColumn GetColumn(int index, string cname) { return null; }

        public virtual object GetColVal(int index, string cname) { return null; }
    }

    public class EbSqlJobReader : SqlJobResource
    {
        [EnableInBuilder(BuilderType.SqlJob)]
        public override string Reference { get; set; }

        [EnableInBuilder(BuilderType.SqlJob)]
        public string RefName { set; get; }

        [EnableInBuilder(BuilderType.SqlJob)]
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

    public class EbSqlJobWriter : SqlJobResource
    {
        [EnableInBuilder(BuilderType.SqlJob)]
        public override string Reference { get; set; }

        [EnableInBuilder(BuilderType.SqlJob)]
        public string RefName { set; get; }

        [EnableInBuilder(BuilderType.SqlJob)]
        public string Version { set; get; }
    }

    public interface ISqlJobCollection
    {
        [EnableInBuilder(BuilderType.SqlJob)]
        OrderedList InnerResources { get; set; }
    }

    [RuntimeSerializable]
    public class SqlJobScript
    {
        [EnableInBuilder(BuilderType.SqlJob)]
        public Type ResultType { get { return this.Data.GetType(); } }

        [EnableInBuilder(BuilderType.SqlJob)]
        public string Data { set; get; }
    }

    public class TV
    {
        [EnableInBuilder(BuilderType.SqlJob)]
        public object Value { get; set; }

        [EnableInBuilder(BuilderType.SqlJob)]
        public string Type { get; set; }
    }
}
