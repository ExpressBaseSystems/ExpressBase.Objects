using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace ExpressBase.Objects
{
    [ProtoBuf.ProtoContract]
    public class EbForm : EbControlContainer
    {
        [Browsable(false)]
        public int FormId { get; set; }

        [Browsable(false)]
        public int VersionId { get; set; }

        [TypeConverter(typeof(FileNameConverter)), CategoryAttribute("Document Settings")]
        public string TableId { get; set; }

        public EbForm() { }
    }

    public class FileNameConverter : StringConverter
    {
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        public override StandardValuesCollection
                     GetStandardValues(ITypeDescriptorContext context)
        {
            return new StandardValuesCollection(new string[]{"New File",
                                                     "File1",
                                                     "Document1"});
        }
    }
}
