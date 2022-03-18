using ExpressBase.Common;
using ExpressBase.Common.Structures;
using ExpressBase.Objects.ServiceStack_Artifacts;
using System;
using System.Collections.Generic;
using System.Dynamic;
using Newtonsoft.Json.Linq;
using ServiceStack.Text;
using ExpressBase.CoreBase.Globals;

namespace ExpressBase.Objects.Objects
{
    public class Globals
    {
        public dynamic T0 { get; set; }
        public dynamic T1 { get; set; }
        public dynamic T2 { get; set; }
        public dynamic T3 { get; set; }
        public dynamic T4 { get; set; }
        public dynamic T5 { get; set; }
        public dynamic T6 { get; set; }
        public dynamic T7 { get; set; }
        public dynamic T8 { get; set; }
        public dynamic T9 { get; set; }
        public dynamic Params { get; set; }
        public dynamic Calc { get; set; }
        public dynamic Summary { get; set; }

        public dynamic CurrentField { get; set; }

        public Globals()
        {
            T0 = new NTVDict();
            T1 = new NTVDict();
            T2 = new NTVDict();
            T3 = new NTVDict();
            T4 = new NTVDict();
            T5 = new NTVDict();
            T6 = new NTVDict();
            T7 = new NTVDict();
            T8 = new NTVDict();
            T9 = new NTVDict();
            Params = new NTVDict();
            Calc = new NTVDict();
            Summary = new NTVDict();
        }

        public dynamic this[string tableIndex]
        {
            get
            {
                if (tableIndex == "T0")
                    return this.T0;
                else if (tableIndex == "T1")
                    return this.T1;
                else if (tableIndex == "T2")
                    return this.T2;
                else if (tableIndex == "T3")
                    return this.T3;
                else if (tableIndex == "T4")
                    return this.T4;
                else if (tableIndex == "T5")
                    return this.T5;
                else if (tableIndex == "T6")
                    return this.T6;
                else if (tableIndex == "T7")
                    return this.T7;
                else if (tableIndex == "T8")
                    return this.T8;
                else if (tableIndex == "T9")
                    return this.T9;
                else if (tableIndex == "Params")
                    return this.Params;
                else if (tableIndex == "Calc")
                    return this.Calc;
                else if (tableIndex == "Summary")
                    return this.Summary;
                else
                    return this.T0;
            }
        }
    }

    public class NTVDict : DynamicObject
    {
        private Dictionary<string, object> dictionary = new Dictionary<string, object>();

        public int Count
        {
            get
            {
                return dictionary.Count;
            }
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            string name = binder.Name;

            object x;
            dictionary.TryGetValue(name, out x);
            if (x != null)
            {
                var _data = x as NTV;

                if (_data.Type == EbDbTypes.Int32)
                    result = Convert.ToDecimal((x as NTV).Value);
                else if (_data.Type == EbDbTypes.Int64)
                    result = Convert.ToDecimal((x as NTV).Value);
                else if (_data.Type == EbDbTypes.Int16)
                    result = Convert.ToDecimal((x as NTV).Value);
                else if (_data.Type == EbDbTypes.Decimal)
                    result = Convert.ToDecimal((x as NTV).Value);
                else if (_data.Type == EbDbTypes.String)
                    result = ((x as NTV).Value).ToString();
                else if (_data.Type == EbDbTypes.DateTime)
                    result = Convert.ToDateTime((x as NTV).Value);
                else if (_data.Type == EbDbTypes.Boolean)
                    result = Convert.ToBoolean((x as NTV).Value);
                else if (_data.Type == EbDbTypes.Object && _data.Value.GetType() == typeof(JObject))
                    result = _data.Value as JObject;
                else
                    result = (x as NTV).Value.ToString();

                return true;
            }

            result = null;
            return false;
        }

        public void Add(string name, NTV value)
        {
            dictionary[name] = value;
        }
    }

    public class NTV
    {
        public string Name { get; set; }

        public EbDbTypes Type { get; set; }

        public object Value { get; set; }
    }

    public class ApiScriptHelper
    {
        internal ApiGlobals Globals { set; get; }

        public ApiScriptHelper(ApiGlobals globals)
        {
            Globals = globals;
        }

        public void SetParam(string name, object value)
        {
            Globals.SetParam(name, value);
        }

        public dynamic GetResourceValue(int index)
        {
            return Globals.GetResourceValue(index);
        }

        public dynamic GetResourceValue(string name)
        {
            return Globals.GetResourceValue(name);
        }

        public void GoTo(int index)
        {
            Globals.GoToResourceByIndex(index);
        }

        public void GoTo(string name)
        {
            Globals.GoToResourceByName(name);
        }

        public void Exit()
        {
            throw new ExplicitExitException("Execution terminated explicitly!");
        }

        public void Exit(string message)
        {
            throw new ExplicitExitException(message);
        }
        public void ExitWithResult(object obj)
        {
            Globals.ExitWithResult(obj);
        }
    }

    public class ApiGlobals : ApiGlobalParent
    {
        private readonly Dictionary<string, object> globalParams;

        public ApiScriptHelper Api { set; get; }

        public dynamic Params { get; set; }

        public List<EbDataTable> Tables { set; get; }

        public ApiGlobals() { }

        public ApiGlobals(Dictionary<string, object> globalParameters)
        {
            this.globalParams = globalParameters;

            this.Api = new ApiScriptHelper(this);
            this.Params = new NTVDict();

            SetGlobalParams(globalParameters);
        }

        public dynamic this[string key]
        {
            get
            {
                if (key == "Params")
                    return this.Params;
                else
                    return null;
            }
        }

        private EbDbTypes GetEbDbType(object value)
        {
            Type type = value.GetType();

            try
            {
                if (type == typeof(JObject))
                {
                    return EbDbTypes.Object;
                }
                else if (type == typeof(JValue))
                {
                    return EbDbTypes.String;
                }
                else
                {
                    return (EbDbTypes)Enum.Parse(typeof(EbDbTypes), type.Name, true);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to parse value '{value}', parse parameter before set, {ex.Message}");
            }
        }

        public void SetGlobalParams(Dictionary<string, object> globalParams)
        {
            foreach (KeyValuePair<string, object> kp in globalParams)
            {
                this["Params"].Add(kp.Key, new NTV
                {
                    Name = kp.Key,
                    Type = GetEbDbType(kp.Value),
                    Value = kp.Value
                });
            }
        }

        internal void SetParam(string name, object value)
        {
            globalParams[name] = value;

            this["Params"].Add(name, new NTV
            {
                Name = name,
                Type = GetEbDbType(value),
                Value = value
            });
        }
    }

    [RuntimeSerializable]
    public class FormGlobals
    {
        public dynamic sourceform { get; set; }

        public dynamic Params { get; set; }

        public dynamic form { get; set; }

        public dynamic user { get; set; }

        public FormGlobals()
        {
            this.form = new FormAsGlobal();
            Params = new NTVDict();
        }
    }

    [RuntimeSerializable]
    public class FormAsGlobal : DynamicObject
    {
        public dynamic Rows { get; set; }

        public List<FormAsGlobal> Containers { get; set; }

        //public string TableName { get; set; }

        public string Name { get; set; }

        public FormAsGlobal()
        {
            this.Rows = new List<ListNTV>();
            this.Containers = new List<FormAsGlobal>();
        }

        public int Count
        {
            get
            {
                return this.Rows.Count;
            }
        }

        public void Add(ListNTV listNTV)
        {
            this.Rows.Add(listNTV);
        }

        public void AddContainer(FormAsGlobal global)
        {
            this.Containers.Add(global);
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            string name = binder.Name;
            object value = null;
            if (this.Count > 0)
            {
                ListNTV temp = this.Rows[0];
                value = temp[name];
            }
            if (value != null)
            {
                result = value;
                return true;
            }
            else
            {
                FormAsGlobal fg = this.Containers.Find(e => e.Name.Equals(name));
                if (fg != null)
                {
                    result = fg;
                    return true;
                }
            }
            result = null;
            return false;
        }

        public Double Sum(string colName)
        {
            Double s = 0;
            foreach (ListNTV listNTV in this.Rows)
            {
                s += Convert.ToDouble(listNTV[colName]);
            }
            return s;
        }
        public Double Avg(string colName)
        {
            Double s = 0;
            foreach (ListNTV listNTV in this.Rows)
            {
                s += Convert.ToDouble(listNTV[colName]);
            }
            return this.Rows.Count > 0 ? s / this.Rows.Count : s;
        }
        public Double Min(string colName)
        {
            Double min = 0;
            if (this.Rows.Count > 0)
                min = Convert.ToDouble(this.Rows[0][colName]);
            foreach (ListNTV listNTV in this.Rows)
            {
                Double t = Convert.ToDouble(listNTV[colName]);
                if (min > t)
                    min = t;
            }
            return min;
        }
        public Double Max(string colName)
        {
            Double max = 0;
            if (this.Rows.Count > 0)
                max = Convert.ToDouble(this.Rows[0][colName]);
            foreach (ListNTV listNTV in this.Rows)
            {
                Double t = Convert.ToDouble(listNTV[colName]);
                if (max < t)
                    max = t;
            }
            return max;
        }
    }

    [RuntimeSerializable]
    public class ListNTV : DynamicObject
    {
        public List<NTV> Columns { get; set; }

        public ListNTV()
        {
            this.Columns = new List<NTV>();
        }

        public void Add(NTV ntv)
        {
            this.Columns.Add(ntv);
        }

        public object this[string name]
        {
            get
            {
                NTV ntv = this.Columns.Find(e => e.Name.Equals(name));
                if (ntv != null)
                    return GetValueFromNTV(ntv);
                else
                    return null;
            }
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            string name = binder.Name;

            NTV ntv = this.Columns.Find(e => e.Name.Equals(name));

            if (ntv != null)
            {
                result = GetValueFromNTV(ntv);
                return true;
            }

            result = null;
            return false;
        }

        private object GetValueFromNTV(NTV ntv)
        {
            object result;
            if (ntv.Type == EbDbTypes.Int32)
                result = Convert.ToDecimal(ntv.Value);
            else if (ntv.Type == EbDbTypes.Int64)
                result = Convert.ToDecimal(ntv.Value);
            else if (ntv.Type == EbDbTypes.Int16)
                result = Convert.ToDecimal(ntv.Value);
            else if (ntv.Type == EbDbTypes.Decimal)
                result = Convert.ToDecimal(ntv.Value);
            else if (ntv.Type == EbDbTypes.String)
                result = Convert.ToString(ntv.Value);
            else if (ntv.Type == EbDbTypes.DateTime)
                result = Convert.ToDateTime(ntv.Value);
            else
                result = Convert.ToString(ntv.Value);
            return result;
        }
    }

    public class SqlJobGlobals
    {
        public SqlJobScriptHelper Job { set; get; }

        public dynamic Params { get; set; }

        public List<EbDataTable> Tables { set; get; }

        public SqlJobGlobals() { }

        public SqlJobGlobals(EbDataSet _ds, ref Dictionary<string, TV> global)
        {
            this.Tables = (_ds == null) ? null : _ds.Tables;

            this.Job = new SqlJobScriptHelper(ref global, this);

            Params = new NTVDict();
        }

        public dynamic this[string key]
        {
            get
            {
                if (key == "Params")
                    return this.Params;
                else
                    return null;
            }
        }
    }

    public class SqlJobScriptHelper
    {
        public Dictionary<string, TV> Parameters { set; get; }

        public SqlJobGlobals Globals { set; get; }

        public SqlJobScriptHelper(ref Dictionary<string, TV> global, SqlJobGlobals sql_global)
        {
            Parameters = global;

            Globals = sql_global;
        }

        public void SetParam(string name, object value)
        {
            if (!Parameters.ContainsKey(name))
                Parameters.Add(name, new TV { Value = value });
            else
                Parameters[name] = new TV { Value = value };
            this.Globals["Params"].Add(name, new NTV
            {
                Name = name,
                Type = (value.GetType() == typeof(JObject) || value.GetType().Name == "JValue") ? EbDbTypes.Object : (EbDbTypes)Enum.Parse(typeof(EbDbTypes), value.GetType().Name, true),
                Value = value as object
            });
        }
    }
}
