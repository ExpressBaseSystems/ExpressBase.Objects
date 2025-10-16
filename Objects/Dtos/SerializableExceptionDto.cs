using System;
using System.Collections.Generic;

namespace ExpressBase.Common.Helpers
{

    [Serializable]
    public class SerializableExceptionDto
    {
        public string Type { get; set; }
        public string Message { get; set; }
        public string StackTrace { get; set; }
        public string Source { get; set; }
        public string TargetSite { get; set; }
        public Dictionary<string, string> Data { get; set; }
        public SerializableExceptionDto InnerException { get; set; }

        public static SerializableExceptionDto FromException(Exception ex)
        {
            if (ex == null) return null;

            var dto = new SerializableExceptionDto
            {
                Type = ex.GetType().FullName,
                Message = ex.Message,
                StackTrace = ex.StackTrace,
                Source = ex.Source,
                TargetSite = ex.TargetSite?.ToString(),
                Data = new Dictionary<string, string>()
            };

            foreach (var key in ex.Data.Keys)
            {
                if (key != null)
                    dto.Data[key.ToString()] = ex.Data[key]?.ToString();
            }

            if (ex.InnerException != null)
                dto.InnerException = FromException(ex.InnerException);

            return dto;
        }

        public override string ToString()
        {
            return $"{Type}: {Message}\n{StackTrace}";
        }
    }
}
