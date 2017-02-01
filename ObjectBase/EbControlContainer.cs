#if !NET462
using ExpressBase.Data;
#endif
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace ExpressBase.Objects
{
    public enum EnumOperator
    {
        Equal,
        NotEqual,
        StartsWith,
        Contains,
        GreaterThan,
        GreaterThanOrEqual,
        LessThan,
        LessThanOrEqual
    }

    [ProtoBuf.ProtoContract]
    [ProtoBuf.ProtoInclude(3000, typeof(EbForm))]
    [ProtoBuf.ProtoInclude(3001, typeof(EbDataGridView))]
    [ProtoBuf.ProtoInclude(3002, typeof(EbTableLayout))]
    public class EbControlContainer : EbControl
    {
        [ProtoBuf.ProtoMember(1)]
        [Browsable(false)]
        public List<EbControl> Controls { get; set; }

        [Browsable(false)]
        public List<EbControl> FlattenedControls { get; set; }

        [ProtoBuf.ProtoMember(2)]
        [TypeConverter(typeof(EbTableConverter))]
        public EbTable Table { get; set; }

        public EbControlContainer() { }

        public override void Init4Redis()
        {
            this.FlattenControls();
        }

#if !NET462
        public void SetData(EbDataTable dt)
        {
            var allControls = this.Controls; //this.GetControls<EbControl>();
            foreach (EbDataRow dr in dt.Rows)
            { 
                foreach (EbControl control in allControls)
                    control.SetData(dr[control.Name]);
            }
        }
#endif

        public List<EbControl> GetControls<T>()
        {
            List<EbControl> collection = new List<EbControl>();

            foreach (EbControl control in this.FlattenedControls)
            {
                if (control is T)
                    collection.Add(control);
            }

            return collection;
        }

        public EbControl GetControl(string name)
        {
            EbControl _ctrl = null;

            foreach (EbControl control in this.FlattenedControls)
            {
                if (control.Name == name)
                {
                    _ctrl = control;
                    break;
                }
            }

            return _ctrl;
        }

        public List<EbControl> GetControlsByPropertyValue<T>(string propertyName, object value, EnumOperator operatorType)
        {
            List<EbControl> collection = new List<EbControl>();

            foreach (EbControl control in this.FlattenedControls)
            {
                PropertyInfo pi = control.GetType().GetProperty(propertyName);
                T propValue = (T)pi.GetValue(control, null);
                T tValue = (T)value;

                IComparable icPropValue = (IComparable)propValue;
                IComparable icValue = (IComparable)value;

                bool checkFlag = false;
                switch (operatorType)
                {
                    case EnumOperator.Equal:
                        checkFlag = (propValue.Equals(tValue));
                        break;
                    case EnumOperator.NotEqual:
                        checkFlag = (!propValue.Equals(tValue));
                        break;
                    case EnumOperator.StartsWith:
                        checkFlag = (propValue != null) ? propValue.ToString().StartsWith(value.ToString()) : false;
                        break;
                    case EnumOperator.Contains:
                        checkFlag = (propValue != null) ? propValue.ToString().Contains(value.ToString()) : false;
                        break;
                    case EnumOperator.GreaterThan:
                        checkFlag = (icPropValue.CompareTo(icValue) > 0);
                        break;
                    case EnumOperator.GreaterThanOrEqual:
                        checkFlag = (icPropValue.CompareTo(icValue) >= 0);
                        break;
                    case EnumOperator.LessThan:
                        checkFlag = (icPropValue.CompareTo(icValue) < 0);
                        break;
                    case EnumOperator.LessThanOrEqual:
                        checkFlag = (icPropValue.CompareTo(icValue) <= 0);
                        break;
                }

                if (checkFlag)
                    collection.Add(control);
            }

            return collection;
        }

#region PRIVATE METHODS

        private void FlattenControls()
        {
            if (this.FlattenedControls == null)
                this.FlattenedControls = new List<EbControl>();

            if (this.FlattenedControls.Count > 0)
                this.FlattenedControls.Clear();

            this.FlattenControlsInner(this.Controls);
        }

        private void FlattenControlsInner(List<EbControl> controls)
        {
            foreach (EbControl control in controls)
            {
                FlattenedControls.Add(control);
                if (control is EbControlContainer)
                {
                    if ((control as EbControlContainer).Controls != null)
                        this.FlattenControlsInner((control as EbControlContainer).Controls);
                }
            }
        }

#endregion PRIVATE METHODS
    }
}
