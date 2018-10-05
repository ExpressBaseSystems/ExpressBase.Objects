using ExpressBase.Common.Extensions;
using ExpressBase.Common.JsonConverters;
using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using ExpressBase.Common.Structures;
using ExpressBase.Objects.EmailRelated;
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

    [EnableInBuilder(BuilderType.SmsBuilder)]
    public class EbSmsTemplate : EbSmsTemplateBase
    {
        [HideInPropertyGrid]
        public string To { get; set; }

        [EnableInBuilder(BuilderType.SmsBuilder)]
        [JsonConverter(typeof(Base64Converter))]
        [HideInPropertyGrid]
        public string Body { get; set; }

        [JsonIgnore]
        public EbDataSource EbDataSource { get; set; }

        [EnableInBuilder(BuilderType.SmsBuilder)]
        [PropertyEditor(PropertyEditorType.ObjectSelector)]
        [OSE_ObjectTypes(EbObjectTypes.iDataSource)]
        public string DataSourceRefId { get; set; }

        [EnableInBuilder(BuilderType.SmsBuilder)]
        [HideInPropertyGrid]
        public StringList Parameters { get; set; }

        [EnableInBuilder(BuilderType.SmsBuilder)]
        [HideInPropertyGrid]
        public List<DsColumns> DsColumnsCollection { get; set; }

        public override void AfterRedisGet(RedisClient Redis)
        {
            try
            {
                this.EbDataSource = Redis.Get<EbDataSource>(this.DataSourceRefId);
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
            [PropertyGroup("Appearance")]
            public float Width { get; set; }

            [EnableInBuilder(BuilderType.SmsBuilder)]
            [UIproperty]
            [PropertyGroup("Appearance")]
            public float Left { get; set; }

            [EnableInBuilder(BuilderType.SmsBuilder)]
            [UIproperty]
            [PropertyGroup("Appearance")]
            public float Right { get; set; }


            [EnableInBuilder(BuilderType.SmsBuilder)]
            [UIproperty]
            [PropertyGroup("General")]
            public string Title { get; set; }

            //[EnableInBuilder(BuilderType.EmailBuilder)]
            //[UIproperty]
            //[PropertyGroup("Appearance")]
            //public int Border { get; set; }

            [EnableInBuilder(BuilderType.SmsBuilder)]
            [UIproperty]
            [PropertyEditor(PropertyEditorType.Color)]
            [PropertyGroup("Appearance")]
            public string BorderColor { get; set; }
        }
    }
}
