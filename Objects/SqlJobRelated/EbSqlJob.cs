using ExpressBase.Common;
using ExpressBase.Common.Data;
using ExpressBase.Common.Extensions;
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

    public interface IRefSelect
    {
        string RefName { set; get; }

        string Version { set; get; }

        string Reference { set; get; }
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
        [HideInPropertyGrid]
        public List<string> FirstReaderKeyColumns { get; set; }

        [EnableInBuilder(BuilderType.SqlJob)]
        [HideInPropertyGrid]
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

    [EnableInBuilder(BuilderType.SqlJob)]
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


    [EnableInBuilder(BuilderType.SqlJob)]
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
        public override string GetDesignHtml()
        {
            return @"<div  class='SqlJobItem dropped' eb-type='Loop' id='@id'> <div>
                        <div tabindex='1' class='drpboxInt lineDrp' onclick='$(this).focus();' id='@id_LpStr' >  
                            <div class='CompLabel'> Loop Start</div>
                        </div>
                        <div class='Sql_Dropable'> </div>
                        <div tabindex='1' class='drpbox lineDrp' onclick='$(this).focus();' id='@id_LpEnd'>  
                            <div class='CompLabel'> Loop End</div>
                        </div>
                    </div></div>".RemoveCR().DoubleQuoted();
        }
    }


    [EnableInBuilder(BuilderType.SqlJob)]
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
        public override string GetDesignHtml()
        {
            return @"<div id='@id' class='SqlJobItem dropped' eb-type='Transaction'> <div>
                        <div tabindex='1' class='drpboxInt lineDrp' onclick='$(this).focus();' id='@id_TrStr'>  
                            <div class='CompLabel'> Transaction Start</div>
                        </div>
                        <div class='Sql_Dropable'> </div>
                        <div tabindex='1' class='drpbox lineDrp' onclick='$(this).focus();' id='@id_TrEnd'>  
                            <div class='CompLabel'> Transaction End</div>
                        </div>
                    </div></div>".RemoveCR().DoubleQuoted();
        }
    }

    public abstract class SqlJobResource : EbSqlJobWrapper
    {
        [EnableInBuilder(BuilderType.SqlJob)]
        public int RouteIndex { set; get; }

        [EnableInBuilder(BuilderType.SqlJob)]
        [UIproperty]
        [MetaOnly]
        public string Label { set; get; }

        [EnableInBuilder(BuilderType.SqlJob)]
        [HideInPropertyGrid]
        public object Result { set; get; }

        public virtual List<Param> GetOutParams(List<Param> _param, int step) { return new List<Param>(); }

        public virtual object GetResult() { return this.Result; }

        public virtual EbDataColumn GetColumn(int index, string cname) { return null; }

        public virtual object GetColVal(int index, string cname) { return null; }
    }

    [EnableInBuilder(BuilderType.SqlJob)]
    public class EbSqlJobReader : SqlJobResource, IRefSelect
    {
        [EnableInBuilder(BuilderType.SqlJob)]
        [MetaOnly]
        [UIproperty]
        public string RefName { set; get; }

        [EnableInBuilder(BuilderType.SqlJob)]
        [MetaOnly]
        [UIproperty]
        public string Version { set; get; }



        [EnableInBuilder(BuilderType.SqlJob)]
        [PropertyEditor(PropertyEditorType.ObjectSelector)]
        [OSE_ObjectTypes(EbObjectTypes.iDataReader)]
        public string Reference { get; set; }

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
        public override string GetDesignHtml()
        {
            return @"<div class='SqlJobItem dropped' eb-type='SqlJobReader' id='@id'>
                        <div tabindex='1' class='drpbox lineDrp' onclick='$(this).focus();'  id='@id_JR'>  
                            <div class='CompLabel'> @Label </div>
                            <div class='CompName'> @RefName </div>
                            <div class='CompVersion'> @Version </div>
                        </div>
                    </div>".RemoveCR().DoubleQuoted();
        }
    }


    [EnableInBuilder(BuilderType.SqlJob)]
    public class EbSqlJobWriter : SqlJobResource, IRefSelect
    {

        [EnableInBuilder(BuilderType.SqlJob)]
        [MetaOnly]
        [UIproperty]
        public string RefName { set; get; }

        [EnableInBuilder(BuilderType.SqlJob)]
        [MetaOnly]
        [UIproperty]
        public string Version { set; get; }

        [EnableInBuilder(BuilderType.SqlJob)]
        [PropertyEditor(PropertyEditorType.ObjectSelector)]
        [OSE_ObjectTypes(EbObjectTypes.iDataWriter)]
        public string Reference { get; set; }

        public override string GetDesignHtml()
        {
            return @"<div class='SqlJobItem dropped' eb-type='SqlJobWriter' id='@id'>
                        <div tabindex='1' class='drpbox lineDrp' onclick='$(this).focus();'  id='@id_JW'>  
                            <div class='CompLabel'> @Label </div>
                            <div class='CompName'> @RefName </div>
                            <div class='CompVersion'> @Version </div>
                        </div>
                    </div>".RemoveCR().DoubleQuoted();
        }
    }

    [EnableInBuilder(BuilderType.SqlJob)]
    public class EbSqlProcessor : SqlJobResource
    {
        [EnableInBuilder(BuilderType.SqlJob)]
        [PropertyEditor(PropertyEditorType.ScriptEditorCS)]
        public EbScript Script { get; set; }

        public override string GetDesignHtml()
        {
            return @"<div class='SqlJobItem dropped' eb-type='SqlProcessor' id='@id'>
                        <div tabindex='1' class='drpbox lineDrp' onclick='$(this).focus();' id='@id_Processor'>  
                            <div class='CompLabel'> @Label </div>
                        </div>
                    </div>".RemoveCR().DoubleQuoted();
        }
        //public override List<Param> GetOutParams(List<Param> _param, int step)
        //{
        //    return _param;
        //}
    }


    [EnableInBuilder(BuilderType.SqlJob)]
    public class EmailNode : SqlJobResource
    {
        [EnableInBuilder(BuilderType.SqlJob)]
        [PropertyEditor(PropertyEditorType.ObjectSelector)]
        [PropertyGroup("Data Settings")]
        [OSE_ObjectTypes(EbObjectTypes.iEmailBuilder)]
        public string Reference { get; set; }

        [EnableInBuilder(BuilderType.SqlJob)]
        [MetaOnly]
        [UIproperty]
        public string RefName { set; get; }

        [EnableInBuilder(BuilderType.SqlJob)]
        [MetaOnly]
        [UIproperty]
        public string Version { set; get; }

        public override string GetDesignHtml()
        {
            return @"<div class='SqlJobItem dropped' eb-type='EmailNode' id='@id'>
                        <div tabindex='1' class='drpbox lineDrp' onclick='$(this).focus();'  id='@id_EmailNode'>  
                            <div class='CompLabel'> @Label </div>
                            <div class='CompName'> @RefName </div>
                            <div class='CompVersion'> @Version </div>
                        </div>
                    </div>".RemoveCR().DoubleQuoted();
        }
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
    public enum InOutStatus
        {
            In,
            UnKnown,
            Out,
            Ignored,
            Excluded,
            Error
    }
    internal class Attendance
    {
        internal int Empmaster_id { get; set; }
        internal DateTime In_time { get; set; }
        internal DateTime Out_time { get; set; }
        internal int IWork { get; set; }
        internal int IBreak { get; set; }
        internal int IOverTime { get; set; }
        internal int IOTHours { get; set; }
        internal int IOTMinutes { get; set; }
        internal string Notes { get; set; }
        internal bool IsNightshift { get; set; }

        internal int App_att_inout_id { get; set; }

        internal Attendance(int empmaster_id)
        {
            this.Empmaster_id = empmaster_id;
        }

        internal Attendance(int empmaster_id, DateTime in_time, DateTime out_time, int iWork, int iBreak, int iOverTime, int iOTHours, int iOTMinutes, string notes, bool bNightshift)
        {
            this.Empmaster_id = empmaster_id;
            this.In_time = in_time;
            this.Out_time = out_time;
            this.IWork = iWork;
            this.IBreak = iBreak;
            this.IOverTime = iOverTime;
            this.IOTHours = iOTHours;
            this.IOTMinutes = iOTMinutes;
            this.Notes = notes;
            this.IsNightshift = bNightshift;
        }

        public void Pp(dynamic Params , TableColletion Tables)
        {
            DateTime dateInQuestion = Convert.ToDateTime(Params.date_to_consolidate);
            int empmaster_id = Convert.ToInt32(Params.empid);

            DateTime dtFirstIn = dateInQuestion;
            DateTime dtLastIn = dateInQuestion;
            DateTime dtLastOut = dateInQuestion;
            InOutStatus lastKnownStatus = InOutStatus.UnKnown;
            InOutStatus lastKnownInOutStatus = InOutStatus.UnKnown;
            InOutStatus currentStatus = InOutStatus.UnKnown;
            int iPos = 0;
            InOutStatus status = InOutStatus.In;
            Attendance att = new Attendance(empmaster_id);
            EbDataTable dt_devattlogs = Tables[0];

            if (dt_devattlogs.Rows.Count > 1)
            {

                EbDataRow row = dt_devattlogs.Rows[0];
                foreach (EbDataRow _row_devattlogs in dt_devattlogs.Rows)
                {
                    if (iPos >= dt_devattlogs.Rows.IndexOf(row))
                    {
                        DateTime _punched_at = Convert.ToDateTime(_row_devattlogs["punched_at"]);

                        if (iPos == dt_devattlogs.Rows.IndexOf(row))
                        {
                            currentStatus = status;
                            //  if (!att.IsNightshift)
                            dtFirstIn = _punched_at;
                            att.In_time = dtFirstIn;
                            dtLastIn = dtFirstIn;
                        }
                        if (iPos > dt_devattlogs.Rows.IndexOf(row))
                        {
                            if (lastKnownStatus == InOutStatus.In)
                            {
                                if ((_punched_at - dtLastIn).TotalMinutes > 5)
                                {
                                    currentStatus = InOutStatus.UnKnown;
                                    dtLastOut = _punched_at;
                                    att.Out_time = dtLastOut;
                                    att.IWork += Convert.ToInt32((dtLastOut - dtLastIn).TotalMinutes);
                                }
                                else
                                    currentStatus = InOutStatus.Ignored;
                            }
                            else if (lastKnownStatus == InOutStatus.Out)
                            {
                                if ((_punched_at - dtLastOut).TotalMinutes > 5)
                                {
                                    currentStatus = InOutStatus.In;
                                    dtLastIn = _punched_at;
                                    att.IBreak += Convert.ToInt32((dtLastIn - dtLastOut).TotalMinutes);
                                }
                                else
                                    currentStatus = InOutStatus.Ignored;
                            }
                            else if (lastKnownStatus == InOutStatus.Ignored)
                            {
                                bool bDoneAnything = false;
                                if (dtLastOut > dtLastIn && (_punched_at - dtLastOut).TotalMinutes > 5)
                                {
                                    currentStatus = InOutStatus.In;
                                    dtLastIn = _punched_at;
                                    att.IBreak += Convert.ToInt32((dtLastIn - dtLastOut).TotalMinutes);
                                    bDoneAnything = true;
                                }

                                if (dtLastIn > dtLastOut && (_punched_at - dtLastIn).TotalMinutes > 5)
                                {
                                    currentStatus = InOutStatus.Out;
                                    dtLastOut = _punched_at;
                                    att.Out_time = dtLastOut;
                                    att.IWork += Convert.ToInt32((dtLastOut - dtLastIn).TotalMinutes);
                                    bDoneAnything = true;
                                }

                                if (!bDoneAnything)
                                    currentStatus = InOutStatus.Ignored;
                            }
                        }
                        _row_devattlogs["inout"] = currentStatus;

                        //FillInOutString
                        if (currentStatus == InOutStatus.In)
                            row["inout_s"] = "IN";
                        else if (currentStatus == InOutStatus.Out)
                            row["inout_s"] = "OUT";
                        else if (currentStatus == InOutStatus.Ignored)
                            row["inout_s"] = "Ignored";
                        else if (currentStatus == InOutStatus.Excluded)
                            row["inout_s"] = "Excluded";
                        else if (currentStatus == InOutStatus.Error)
                            row["inout_s"] = "ERROR";

                        if (row["machineno"] != DBNull.Value)
                            row["type"] = "Device";
                        else
                            row["type"] = "Manual";

                        lastKnownStatus = currentStatus;
                        if (currentStatus == InOutStatus.In || currentStatus == InOutStatus.Out)
                            lastKnownInOutStatus = currentStatus;
                    }
                    iPos++;
                }
                if (att.In_time != DateTime.MinValue && att.Out_time != DateTime.MinValue)
                {
                    //this.MarkPresent(att.Empmaster_id, cell, null);
                    //this.Save(devattlogs, att, dateInQuestion, break_time, bonus_ot);
                }
                //  else
                // this.MarkError(cell, att.Empmaster_id, dateInQuestion, devattlogs.Rows.Count, Convert.ToInt32(att.IWork / 60), devattlogs, string.Empty);
                // this.SetWorkBreakOT(dt_devattlogs, true, att);
            }
            else
            {
                //var DateTime_Now = CacheHelper.Get<DateTime>(CacheKeys.SYSVARS_NOW_LOCALE);
                //if (((DateTime)cell.OwningColumn.Tag).Date == DateTime_Now.Date)
                //    this.MarkUnReviewed(cell, empmaster_id, dateInQuestion, devattlogs.Rows.Count, devattlogs);
                //else
                //    this.MarkAbsent(cell, empmaster_id, dateInQuestion);
            }

            //Console.Write(JsonConvert.SerializeObject(att));
            //Job.SetParam(""in_time"", att.In_time.ToString(""yyyy - MM - dd HH: mm""));
            //Job.SetParam(""out_time"", att.Out_time.ToString(""yyyy - MM - dd HH: mm""));
            //Job.SetParam(""duration"", att.IWork);
            //Job.SetParam(""break_time"", att.IBreak);
            //Job.SetParam(""ot_time"", att.IOverTime);
            //Job.SetParam(""ot_time_approved"", (att.IOTHours * 60) + att.IOTMinutes);
            //Job.SetParam(""notes"", (att.Notes == """" || att.Notes == null) ? ""_"" : att.Notes);
            //Job.SetParam(""night_shift"", att.IsNightshift);
            //Job.SetParam(""att_date"", dateInQuestion.ToString(""yyyy - MM - dd HH: mm""));
        }
    }
}
