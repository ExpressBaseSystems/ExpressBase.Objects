using ExpressBase.Common.Structures;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExpressBase.Objects
{
    public interface ILinesEnabled { }

    public interface INonPersistControl { }

    public interface ILayoutControl { }

    public interface IMobileDataPart
    {
        int TableIndex { get; set; }

        int ColumnIndex { get; set; }

        string ColumnName { get; set; }

        EbDbTypes Type { get; set; }
    }

    public interface IMobileUIStyles
    {
        string BackgroundColor { set; get; }

        string ForegroundColor { set; get; }

        int BorderThickness { set; get; }

        string BorderColor { set; get; }

        int BorderRadius { set; get; }
    }

    public interface IMobileLink
    {
        string LinkRefId { set; get; }
    }
}
