using ExpressBase.Common;
using ExpressBase.Common.Extensions;
using ExpressBase.Common.JsonConverters;
using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using ExpressBase.Common.Structures;
using ExpressBase.Objects.Helpers;
using Newtonsoft.Json;
using ServiceStack.Redis;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExpressBase.Objects.EmailRelated
{
    public enum EmailPriority
    {
        High,
        Low,
        Medium
    }
    public enum DateFormat
    {
        ddmmyy,
        mmddyy
    }

    [EnableInBuilder(BuilderType.EmailBuilder)]
    public class EbEmailTemplateBase : EbObject
    {

    }

    [EnableInBuilder(BuilderType.EmailBuilder)]
    public class EbEmailTemplate : EbEmailTemplateBase
    {
        //[EnableInBuilder(BuilderType.EmailBuilder)]
        //public string Description { get; set; }
        

        [EnableInBuilder(BuilderType.EmailBuilder)]
        public EmailPriority Priority { get; set; }      

        [EnableInBuilder(BuilderType.EmailBuilder)]
        public string Subject { get; set; }

        [EnableInBuilder(BuilderType.EmailBuilder)]
        [JsonConverter(typeof(Base64Converter))]
        [HideInPropertyGrid]
        public string Body { get; set; }

        [EnableInBuilder(BuilderType.EmailBuilder)]
        [PropertyEditor(PropertyEditorType.ObjectSelector)]
        [OSE_ObjectTypes(EbObjectTypes.iDataSource)]
        public string DataSourceRefId { get; set; }

        [EnableInBuilder(BuilderType.EmailBuilder)]
        [HideInPropertyGrid]
        public List<string> Parameters { get; set; }

        [EnableInBuilder(BuilderType.EmailBuilder)]
        [HideInPropertyGrid]
        public List<DsColumns> DsColumnsCollection { get; set; }


        [EnableInBuilder(BuilderType.EmailBuilder)]    //remove after use
        [UIproperty]
        [PropertyEditor(PropertyEditorType.Color)]
        [PropertyGroup("Appearance")]
        public string Febintest { get; set; }



        [JsonIgnore]
        public EbDataSource EbDataSource { get; set; }

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
    }
    [EnableInBuilder(BuilderType.EmailBuilder)]
    public class DsColumnsDetails : EbEmailTemplateBase
    {
        [EnableInBuilder(BuilderType.EmailBuilder)]
        public DateFormat DateFormat { get; set; }

        [EnableInBuilder(BuilderType.EmailBuilder)]
        [UIproperty]
        [PropertyGroup("Appearance")]
        public float Width { get; set; }

        [EnableInBuilder(BuilderType.EmailBuilder)]
        [UIproperty]
        [PropertyGroup("Appearance")]
        public float Left { get; set; }

        [EnableInBuilder(BuilderType.EmailBuilder)]
        [UIproperty]
        [PropertyGroup("Appearance")]
        public float Right { get; set; }


        [EnableInBuilder(BuilderType.EmailBuilder)]
        [UIproperty]
        [PropertyGroup("General")]
        public string Title { get; set; }

        //[EnableInBuilder(BuilderType.EmailBuilder)]
        //[UIproperty]
        //[PropertyGroup("Appearance")]
        //public int Border { get; set; }

        [EnableInBuilder(BuilderType.EmailBuilder)]
        [UIproperty]
        [PropertyEditor(PropertyEditorType.Color)]
        [PropertyGroup("Appearance")]
        public string BorderColor { get; set; }
    }


    [EnableInBuilder(BuilderType.EmailBuilder)]
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
    /// related to property grid
  
}
