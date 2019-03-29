using ExpressBase.Common;
using ExpressBase.Common.Structures;
using ExpressBase.Objects.ServiceStack_Artifacts;
using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Text;

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
                else if(_data.Type == EbDbTypes.Boolean)
                    result = Convert.ToBoolean((x as NTV).Value);
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

    public class ApiGlobals
    {
        public List<EbDataTable> Tables { set; get; }

        public ApiGlobals() { }

        public ApiGlobals(EbDataSet _ds)
        {
            this.Tables = _ds.Tables;
        }
    }

    public class FormGlobals
    {
        public dynamic FORM { get; set; }

        public dynamic USER { get; set; }

        public FormGlobals()
        {
            this.FORM = new FormAsGlobal();
        }
    }

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
    }

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
                result = (ntv.Value).ToString();
            else if (ntv.Type == EbDbTypes.DateTime)
                result = Convert.ToDateTime(ntv.Value);
            else
                result = ntv.Value.ToString();
            return result;
        }
    }
}
