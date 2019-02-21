using ExpressBase.Common.Structures;
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

        public ApiGlobals()
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
                else
                    return this.T0;
            }
        }

    }
}
