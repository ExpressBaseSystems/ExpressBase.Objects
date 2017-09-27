using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExpressBase.Objects.Objects.EmailRelated
{
    public class EbEmailTemplatePG : EbEmailBuilder
    {
        [EnableInBuilder(BuilderType.EmailBuilder)]
        new public string Name { get; set; }

        [EnableInBuilder(BuilderType.EmailBuilder)]
        public string Description { get; set; }

        [EnableInBuilder(BuilderType.EmailBuilder)]
        [PropertyEditor(PropertyEditorType.ObjectSelector)]
        [OSE_ObjectTypes(EbObjectType.DataSource)]
        public string DataSourceRefId { get; set; }
    }
}
