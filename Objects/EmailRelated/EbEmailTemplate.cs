using ExpressBase.Common;
using ExpressBase.Common.Constants;
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

namespace ExpressBase.Objects
{
    public enum EmailPriority
    {
        High,
        Low,
        Medium
    }
    public enum DateFormatEmail
    {
        ddmmyy,
        mmddyy
    }

    [EnableInBuilder(BuilderType.EmailBuilder)]
    public class EbEmailTemplateBase : EbObject
    {

    }

    [EnableInBuilder(BuilderType.EmailBuilder)]
    [BuilderTypeEnum(BuilderType.EmailBuilder)]
    public class EbEmailTemplate : EbEmailTemplateBase, IEBRootObject
    {
        public bool HideInMenu { get; set; }

        [EnableInBuilder(BuilderType.EmailBuilder)]
        public override string DisplayName { get; set; }

        [EnableInBuilder(BuilderType.EmailBuilder)]
        public override string Name { get; set; }

        [EnableInBuilder(BuilderType.EmailBuilder)]
        public override string Description { get; set; }

        [EnableInBuilder(BuilderType.EmailBuilder)]
        [HideInPropertyGrid]
        public override string RefId { get; set; }

        [EnableInBuilder(BuilderType.EmailBuilder)]
        [HideInPropertyGrid]
        public override string VersionNumber { get; set; }

        [EnableInBuilder(BuilderType.EmailBuilder)]
        [HideInPropertyGrid]
        public override string Status { get; set; }

        [EnableInBuilder(BuilderType.EmailBuilder)]
        public EmailPriority Priority { get; set; }

        [EnableInBuilder(BuilderType.EmailBuilder)]
        [HideInPropertyGrid]
        public string Subject { get; set; }

        [EnableInBuilder(BuilderType.EmailBuilder)]
        [HideInPropertyGrid]
        public string To { get; set; }

        [EnableInBuilder(BuilderType.EmailBuilder)]
        [HideInPropertyGrid]
        public string Cc { get; set; }

        [EnableInBuilder(BuilderType.EmailBuilder)]
        [HideInPropertyGrid]
        public string Bcc { get; set; }

        [EnableInBuilder(BuilderType.EmailBuilder)]
        [HideInPropertyGrid]
        public string ReplyTo { get; set; }

        [EnableInBuilder(BuilderType.EmailBuilder)]
        [JsonConverter(typeof(Base64Converter))]
        [HideInPropertyGrid]
        public string Body { get; set; }

        [EnableInBuilder(BuilderType.EmailBuilder)]
        [PropertyEditor(PropertyEditorType.ObjectSelector)]
        [OSE_ObjectTypes(EbObjectTypes.iDataReader)]
        public string DataSourceRefId { get; set; }

        [EnableInBuilder(BuilderType.EmailBuilder)]
        [HideInPropertyGrid]
        public StringList Parameters { get; set; }

        [EnableInBuilder(BuilderType.EmailBuilder)]
        [HideInPropertyGrid]
        public List<DsColumns> DsColumnsCollection { get; set; }

        [EnableInBuilder(BuilderType.EmailBuilder)]
        [PropertyEditor(PropertyEditorType.ObjectSelector)]
        [OSE_ObjectTypes(EbObjectTypes.iReport)]
        public string AttachmentReportRefID { get; set; }

        public override List<string> DiscoverRelatedRefids()
        {
            List<string> _refids = new List<string>();
            if (!string.IsNullOrEmpty(this.DataSourceRefId))
                _refids.Add(DataSourceRefId);
            if (!string.IsNullOrEmpty(this.AttachmentReportRefID))
                _refids.Add(AttachmentReportRefID);
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
            if (!string.IsNullOrEmpty(this.AttachmentReportRefID))
            {
                if (RefidMap.ContainsKey(AttachmentReportRefID))
                    AttachmentReportRefID = RefidMap[AttachmentReportRefID];
                else
                    AttachmentReportRefID = "";
            }
        }

        [JsonIgnore]
        public EbDataReader EbDataSource { get; set; }

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
    }

    [EnableInBuilder(BuilderType.EmailBuilder)]
    public class DsColumnsDetails : EbEmailTemplateBase
    {
        [EnableInBuilder(BuilderType.EmailBuilder)]
        public DateFormatEmail DateFormat { get; set; }

        [EnableInBuilder(BuilderType.EmailBuilder)]
        [UIproperty]
        [PropertyGroup(PGConstants.APPEARANCE)]
        public float Width { get; set; }

        [EnableInBuilder(BuilderType.EmailBuilder)]
        [UIproperty]
        [PropertyGroup(PGConstants.APPEARANCE)]
        public float Left { get; set; }

        [EnableInBuilder(BuilderType.EmailBuilder)]
        [UIproperty]
        [PropertyGroup(PGConstants.APPEARANCE)]
        public float Right { get; set; }


        [EnableInBuilder(BuilderType.EmailBuilder)]
        [UIproperty]
        [PropertyGroup("General")]
        public string Title { get; set; }

        //[EnableInBuilder(BuilderType.EmailBuilder)]
        //[UIproperty]
        //[PropertyGroup(PGConstants.APPEARANCE)]
        //public int Border { get; set; }

        [EnableInBuilder(BuilderType.EmailBuilder)]
        [UIproperty]
        [PropertyEditor(PropertyEditorType.Color)]
        [PropertyGroup(PGConstants.APPEARANCE)]
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
    public class StringList : List<string> { }
}
