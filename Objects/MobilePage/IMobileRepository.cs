using ExpressBase.Common;
using System.Collections.Generic;

namespace ExpressBase.Objects
{
    public interface ILinesEnabled { }

    public interface INonPersistControl { }

    public interface ILayoutControl { }

    public interface IMobileLink
    {
        string LinkRefId { set; get; }

        WebFormDVModes FormMode { set; get; }

        List<EbMobileDataColToControlMap> LinkFormParameters { get; set; }
    }
}
