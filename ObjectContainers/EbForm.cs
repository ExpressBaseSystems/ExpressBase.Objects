#if !NET462
using ExpressBase.Data;
#endif
using ServiceStack.Redis;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
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

        [Browsable(false)]
        public bool IsUpdate { get; set; }
       
        
        //[ProtoBuf.ProtoMember(1)]
        //[TypeConverter(typeof(EbTableConverter))]
        //public EbTable Table { get; set; }

        public EbForm() { }

#if !NET462
        public void SetData(EbDataSet ds)
        {
            var allContainers = this.GetControls<EbControlContainer>();
            allContainers.Add(this);
            foreach (EbControlContainer container in allContainers)
            {
                foreach (EbDataTable dt in ds.Tables)
                {
                    //if (dt.TableName == container.Table.Name)
                        container.SetData(dt);
                }
            }
        }
#endif
    }

    public class EbTableConverter : TypeConverter
    {
        private EbTableCollection _ebTableCollection = null;
        private EbTableCollection EbTableCollection
        {
            get
            {
                if (_ebTableCollection == null)
                {
                    using (var redisClient = new RedisClient("139.59.39.130", 6379, "Opera754$"))
                        _ebTableCollection = redisClient.Get<EbTableCollection>("EbTableCollection");
                }

                return _ebTableCollection;
            }
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return (sourceType == typeof(string));
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string)
                return this.EbTableCollection[value.ToString()];

            return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string) && value != null)
                return (value as EbTable).Name;

            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            return new StandardValuesCollection(this.EbTableCollection.Values);
        }
    }
}
