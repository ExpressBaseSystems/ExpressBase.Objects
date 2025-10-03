using System;
using System.Collections.Generic;
using System.Linq;
using ExpressBase.Common;

namespace ExpressBase.Objects.Dtos
{
    public class EbButtonPublicFormAttachServiceDto
    {
        public string PublicFormId { get; set;}
        public int ExpireInDays { get; set;}
        public int ExpireInHours { get; set; }
        public int ExpireInMinutes { get; set; }
        public List<EbButtonPublicFromAttachFieldMaps> FieldMaps { get; set; }
        public DateTime? FutureDateTime { get; set; }

        /// <summary>
        /// Key = DestinationFormControlName, Value = SourceFormPrimaryTableDataValue
        /// </summary>
        public Dictionary<string, object> DestinationValues { get; set; }

        /// <summary>
        /// DTO constructor – initializes all properties and computes expiry & mappings.
        /// </summary>
        public EbButtonPublicFormAttachServiceDto(
            string publicFormId,
            int expireInDays,
            int expireInHours,
            int expireInMinutes,
            List<EbButtonPublicFromAttachFieldMaps> fieldMaps,
            List<SingleColumn> sourceFormPrimaryTableData)
        {
            PublicFormId = publicFormId;
            ExpireInDays = expireInDays;
            ExpireInHours = expireInHours;
            ExpireInMinutes = expireInMinutes;
            FieldMaps = fieldMaps ?? new List<EbButtonPublicFromAttachFieldMaps>();

            if(expireInDays > 0 || expireInHours > 0 || expireInMinutes > 0)
            {
                FutureDateTime = DateTime.Now
               .AddDays(ExpireInDays)
               .AddHours(ExpireInHours)
               .AddMinutes(ExpireInMinutes);
            }
           

            DestinationValues = new Dictionary<string, object>();
            if (FieldMaps.Any() && sourceFormPrimaryTableData != null)
            {
                foreach (var fieldMap in FieldMaps)
                {
                    var value = sourceFormPrimaryTableData
                        .FirstOrDefault(c => c.Name == fieldMap.SourceFormContolName)
                        ?.Value;

                    if (value != null)
                    {
                        DestinationValues[fieldMap.DesitnationFormContolName] = sourceFormPrimaryTableData
                        .FirstOrDefault(c => c.Name == fieldMap.SourceFormContolName);
                    }
                }
            }
        }
    }
}
