using ExpressBase.Common.Extensions;
using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using ExpressBase.Objects.Objects.DVRelated;
using Newtonsoft.Json;
using ServiceStack.Redis;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExpressBase.Objects.ReportRelated
{
    [EnableInBuilder(BuilderType.Report)]
    public class EbReportColSelect: EbReportField
    {       
        [EnableInBuilder(BuilderType.Report)]
        [PropertyEditor(PropertyEditorType.ObjectSelector)]
        [OSE_ObjectTypes(EbObjectType.DataSource)]
        public string DataSourceRefId { get; set; }
    }
}
