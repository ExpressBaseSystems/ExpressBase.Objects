using System;
using System.Collections.Generic;
using System.Text;

namespace ExpressBase.Objects.Dtos
{
    public class PublicFormV2QueryParamsDto
    {
        public string PublicFormRefId { get; set; }
        public string SourceFormRefId { get; set; }
        public int FormDataId { get; set; }
        public string TimeZone { get; set; }
        public string UserIp { get; set; }
    }
}
