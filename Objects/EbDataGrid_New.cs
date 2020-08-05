using ExpressBase.Common;
using ExpressBase.Common.Extensions;
using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using ExpressBase.Common.Structures;
using ExpressBase.Objects.Objects.DVRelated;
using System.Runtime.Serialization;

namespace ExpressBase.Objects
{
    [EnableInBuilder(BuilderType.WebForm, BuilderType.UserControl)]
    //[HideInToolBox]
    public class EbDataGrid_New : EbDataGrid, IEbSpecialContainer
    {
        public EbDataGrid_New() : base() { }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.UserControl)]
        [HideInPropertyGrid]
        public DVColumnCollection DVColumnColl { get; set; }

        public override string ToolNameAlias { get { return "DataGrid New"; } set { } }

        public override string ToolIconHtml { get { return "<i class='fa fa-exclamation-circle'></i>"; } set { } }

        public void ProcessDvColumnCollection()
        {
            if (this.DVColumnColl != null)
                return;

            this.DVColumnColl = new DVColumnCollection();
            int indx = 0;
            foreach (EbDGColumn column in this.Controls)
            {
                DVBaseColumn _col = null;

                if (column.EbDbType == EbDbTypes.Int16 || column.EbDbType == EbDbTypes.Int32 || column.EbDbType == EbDbTypes.Int64 || column.EbDbType == EbDbTypes.Double || column.EbDbType == EbDbTypes.Decimal || column.EbDbType == EbDbTypes.VarNumeric)
                    _col = new DVNumericColumn { Data = indx, Name = column.Name, sTitle = column.Name, Type = column.EbDbType, bVisible = true, sWidth = "100px", Align = Align.Right, Aggregate = true };
                else if (column.EbDbType == EbDbTypes.Boolean)
                    _col = new DVBooleanColumn { Data = indx, Name = column.Name, sTitle = column.Name, Type = column.EbDbType, bVisible = true, sWidth = "100px" };
                else if (column.EbDbType == EbDbTypes.DateTime || column.EbDbType == EbDbTypes.Date || column.EbDbType == EbDbTypes.Time)
                    _col = new DVDateTimeColumn { Data = indx, Name = column.Name, sTitle = column.Name, sType = "date-uk", Type = column.EbDbType, bVisible = true, sWidth = "100px" };
                else 
                    _col = new DVStringColumn { Data = indx, Name = column.Name, sTitle = column.Name, Type = column.EbDbType, bVisible = true, sWidth = "100px" };

                _col.EbSid = column.EbDbType.ToString() + indx;
                _col.RenderType = _col.Type;
                DVColumnColl.Add(_col);
                indx++;
            }
            DVColumnColl.Add(new DVStringColumn { Data = indx++, Name = "id", sTitle = "id", Type = EbDbTypes.Int32, bVisible = false, sWidth = "100px" });
            DVColumnColl.Add(new DVStringColumn { IsCustomColumn = true, Data = indx, Name = "settings", sTitle = "<i class='fa fa-cog'></i>", Type = EbDbTypes.String, bVisible = true, sWidth = "100px" });
        }
    }
}