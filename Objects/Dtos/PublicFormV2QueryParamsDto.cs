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
        public List<PrefillParam> PrefillParams { get; set; } = new List<PrefillParam>();

    }

    public class PrefillParam
    {
        public string Name { get; set; }
        public PrefillParamType Type { get; set; }
        public string Value { get; set; }
    }

    public enum PrefillParamType
    {
        text,
        number,
        date,
        email,
        phone,
        hidden,
        checkbox,
        radio,
        select,
        textarea
    }
}
