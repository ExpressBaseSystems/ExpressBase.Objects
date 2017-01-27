using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace ExpressBase.Objects
{
    [ProtoBuf.ProtoContract]
    [ProtoBuf.ProtoInclude(3000, typeof(EbForm))]
    [ProtoBuf.ProtoInclude(3001, typeof(EbDataGridView))]
    [ProtoBuf.ProtoInclude(3002, typeof(EbTableLayout))]
    public class EbControlContainer : EbControl
    {
        [ProtoBuf.ProtoMember(1)]
        [Browsable(false)]
        public List<EbControl> Controls { get; set; }

        public EbControlContainer() { }

        public List<EbControl> GetControls<T>()
        {
            List<EbControl> collection = new List<EbControl>();
            this.GetControls<T>(this.Controls, ref collection);
            return collection;
        }

        public EbControl GetControl(string name)
        {
            EbControl _ctrl = null;
            this.GetRecursive(this.Controls, ref _ctrl, name);
            return _ctrl;
        }

        #region PRIVATE METHODS

        private void GetControls<T>(List<EbControl> sourcecollection, ref List<EbControl> resultcollection)
        {
            foreach (EbControl _control in sourcecollection)
            {
                if (_control is T)
                    resultcollection.Add(_control);
                if (_control is EbControlContainer)
                    this.GetControls<T>((_control as EbControlContainer).Controls, ref resultcollection);
            }
        }

        private EbControl GetRecursive(List<EbControl> controls, ref EbControl _ctrl, string name)
        {
            foreach (EbControl control in controls)
            {
                if (control is EbControlContainer)
                    GetRecursive((control as EbControlContainer).Controls, ref _ctrl, name);
                else
                {
                    if (control.Name == name)
                        _ctrl = control;
                }

                if (_ctrl != null) break;
            }

            return _ctrl;
        }

        #endregion PRIVATE METHODS
    }
}
