using ExpressBase.Common.Constants;
using ExpressBase.Common.Extensions;
using ExpressBase.Common.JsonConverters;
using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using ExpressBase.Common.Structures;
using ExpressBase.Objects;
using Newtonsoft.Json;
using ServiceStack.Redis;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExpressBase.Objects.Objects.SmsRelated
{
    [EnableInBuilder(BuilderType.SmsBuilder)]
    public class EbSmsTemplateBase : EbObject
    {

    }

    [BuilderTypeEnum(BuilderType.SmsBuilder)]
    [EnableInBuilder(BuilderType.SmsBuilder)]
    public class EbSmsTemplate : EbSmsTemplateBase,IEBRootObject
    {
        public bool HideInMenu { get; set; }

        [EnableInBuilder(BuilderType.SmsBuilder)]
        [HideInPropertyGrid]
        public override string RefId { get; set; }

        [EnableInBuilder(BuilderType.SmsBuilder)]
        public override string DisplayName { get; set; }

        [EnableInBuilder(BuilderType.SmsBuilder)]
        public override string Description { get; set; }

        [EnableInBuilder(BuilderType.SmsBuilder)]
        [HideInPropertyGrid]
        public override string VersionNumber { get; set; }

        [EnableInBuilder(BuilderType.SmsBuilder)]
        [HideInPropertyGrid]
        public override string Status { get; set; }

        [EnableInBuilder(BuilderType.SmsBuilder)]
        [HideInPropertyGrid]
        public string To { get; set; }

        [EnableInBuilder(BuilderType.SmsBuilder)]
        [JsonConverter(typeof(Base64Converter))]
        [HideInPropertyGrid]
        public string Body { get; set; }

        [JsonIgnore]
        public EbDataReader EbDataSource { get; set; }

        [EnableInBuilder(BuilderType.SmsBuilder)]
        [PropertyEditor(PropertyEditorType.ObjectSelector)]
        [OSE_ObjectTypes(EbObjectTypes.iDataReader)]
        public string DataSourceRefId { get; set; }

        [EnableInBuilder(BuilderType.SmsBuilder)]
        [HideInPropertyGrid]
        public StringList Parameters { get; set; }

        [EnableInBuilder(BuilderType.SmsBuilder)]
        [HideInPropertyGrid]
        public List<DsColumns> DsColumnsCollection { get; set; }

        public override List<string> DiscoverRelatedRefids()
        {
            List<string> _refids = new List<string>();
            if (!string.IsNullOrEmpty(this.DataSourceRefId))
                    _refids.Add(DataSourceRefId);
            return _refids;
        }

        public override void ReplaceRefid(Dictionary<string, string> RefidMap)
        {
            if (!string.IsNullOrEmpty(this.DataSourceRefId))
            {
                if (RefidMap.ContainsKey(DataSourceRefId))
                    DataSourceRefId = RefidMap[DataSourceRefId];
                else
                    DataSourceRefId = "";
            }
        }

        public override void AfterRedisGet(RedisClient Redis)
        {
            try
            {
                this.EbDataSource = Redis.Get<EbDataReader>(this.DataSourceRefId);
                this.EbDataSource.AfterRedisGet(Redis);
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.ToString());
            }
        }
        [EnableInBuilder( BuilderType.SmsBuilder)]
        public class DsColumns : DsColumnsDetails
        {
            public override string GetDesignHtml()
            {
                return "<span class='ebdscols' eb-type='DsColumns' format='@format'  id='@id' style='border: @Border px solid;border-color: @BorderColor ;background-color: @BackColor;'>@Title </span>".RemoveCR().DoubleQuoted();
            }
            public override string GetJsInitFunc()
            {
                return @"
    this.Init = function(id)
        {
    this.Height =25;
    this.Width= 175;
    this.Border = 1;
    this.BorderColor = '#aaaaaa'
};";
            }
        }
        [EnableInBuilder( BuilderType.SmsBuilder)]
        public class DsColumnsDetails : EbSmsTemplateBase
        {
            //[EnableInBuilder(BuilderType.SmsBuilder)]
            //public DateFormat DateFormat { get; set; }

            [EnableInBuilder(BuilderType.SmsBuilder)]
            [UIproperty]
            [PropertyGroup(PGConstants.APPEARANCE)]
            public float Width { get; set; }

            [EnableInBuilder(BuilderType.SmsBuilder)]
            [UIproperty]
            [PropertyGroup(PGConstants.APPEARANCE)]
            public float Left { get; set; }

            [EnableInBuilder(BuilderType.SmsBuilder)]
            [UIproperty]
            [PropertyGroup(PGConstants.APPEARANCE)]
            public float Right { get; set; }


            [EnableInBuilder(BuilderType.SmsBuilder)]
            [UIproperty]
            [PropertyGroup("General")]
            public string Title { get; set; }

            //[EnableInBuilder(BuilderType.EmailBuilder)]
            //[UIproperty]
            //[PropertyGroup(PGConstants.APPEARANCE)]
            //public int Border { get; set; }

            [EnableInBuilder(BuilderType.SmsBuilder)]
            [UIproperty]
            [PropertyEditor(PropertyEditorType.Color)]
            [PropertyGroup(PGConstants.APPEARANCE)]
            public string BorderColor { get; set; }
        }
    }
}
