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

        public List<EbControl> FlattenedControls { get; set; }

        public EbControlContainer() { }

        public override void Init4Redis()
        {
            this.FlattenControls();
        }

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
                    this.FlattenControlsInner((control as EbControlContainer).Controls);
            }
        }

        #endregion PRIVATE METHODS
    }
}
