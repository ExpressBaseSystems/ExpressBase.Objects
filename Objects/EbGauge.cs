using ExpressBase.Common;
using ExpressBase.Common.Extensions;
using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using ExpressBase.Objects.Objects.DVRelated;
using Newtonsoft.Json;
using Org.BouncyCastle.Asn1.Cms;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ExpressBase.Objects
{

    [EnableInBuilder(BuilderType.BotForm, BuilderType.WebForm, BuilderType.UserControl ,BuilderType.DashBoard)]
    [HideInToolBox]
    class EbGauge : EbControlUI
    {

        public EbGauge() { }

        [OnDeserialized]
        public void OnDeserializedMethod(StreamingContext context)
        {
            this.BareControlHtml = this.GetBareHtml();
            this.ObjType = this.GetType().Name.Substring(2, this.GetType().Name.Length - 2);
        }

        [EnableInBuilder(BuilderType.BotForm, BuilderType.WebForm, BuilderType.UserControl, BuilderType.DashBoard)]
        public string DataObjColName { get; set; }

        public override string UIchangeFns
        {
            get
            {
                return @"";
            }
        }
        //public override string GetBareHtml()
        //{
        //    return @"`<div id='gaugeChart' style='border:solid 1px'></div>`";
        //}
        //public override string GetDesignHtml()
        //{
        //    return @"`<div id='gaugeChart' style='border:solid 1px'></div>`";
        //}
        public override string GetHtml()
        {
            return @"`<div id='gaugeChart' style='border:solid 1px'></div>`";
        }
    }
}
