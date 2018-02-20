using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Text;

namespace ExpressBase.Objects.Objects.ReportRelated
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
        }

        public dynamic this[string tableIndex]
        {
            get
            {
                if (tableIndex == "T0")
                    return this.T0;

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
            string name = binder.Name.ToLower();

            object x;
            dictionary.TryGetValue(name, out x);
            if (x != null)
            {
                var _data = x as NTV;

                if (_data.Type == DbType.Int32)
                    result = Convert.ToInt32((x as NTV).Value);
                if (_data.Type == DbType.Decimal)
                    result = Convert.ToDecimal((x as NTV).Value);
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

        public DbType Type { get; set; }

        public object Value { get; set; }
    }
}
