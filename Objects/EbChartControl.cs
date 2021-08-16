using ExpressBase.Common;
using ExpressBase.Common.Constants;
using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using ExpressBase.Common.Structures;
using ExpressBase.Objects.ServiceStack_Artifacts;
using Newtonsoft.Json;
using ServiceStack;
using ServiceStack.Redis;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace ExpressBase.Objects
{

    [EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
    public class EbChartControl : EbTVcontrol
    {
        public EbChartControl() { }

        [OnDeserialized]
        public void OnDeserializedMethod(StreamingContext context)
        {
            this.BareControlHtml = this.GetBareHtml();
            //this.BareControlHtml4Bot = this.BareControlHtml;
            this.ObjType = this.GetType().Name.Substring(2, this.GetType().Name.Length - 2);
        }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
        [HideInPropertyGrid]
        public string ChartVisualizationJson { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
        [PropertyGroup(PGConstants.CORE)]
        [PropertyEditor(PropertyEditorType.ObjectSelector)]
        [OSE_ObjectTypes(EbObjectTypes.iChartVisualization)]
        [Alias("Chart Visualization")]
        public override string TVRefId { get; set; }

        [HideInPropertyGrid]
        [JsonIgnore]
        public override string ToolIconHtml { get { return "<i class='fa fa-pie-chart'></i>"; } set { } }

        [HideInPropertyGrid]
        [JsonIgnore]
        public override string ToolNameAlias { get { return "Chart"; } set { } }

        [HideInPropertyGrid]
        [EnableInBuilder(BuilderType.BotForm)]
        public override bool IsDisable { get => true; }

        [JsonIgnore]
        public override string DisableJSfn { get { return string.Empty; } set { } }

        [JsonIgnore]
        public override string EnableJSfn { get { return string.Empty; } set { } }


        public void InitFromDataBase(JsonServiceClient ServiceClient, IRedisClient redis)
        {
            EbChartVisualization ChartVisualization = EbFormHelper.GetEbObject<EbChartVisualization>(TVRefId, ServiceClient, redis, null);
            this.ChartVisualizationJson = EbSerializers.Json_Serialize(ChartVisualization);
        }

        public override void FetchParamsMeta(IServiceClient ServiceClient, IRedisClient redis, EbControl[] Allctrls, Service service)
        {
            EbChartVisualization ChartVisualization = EbFormHelper.GetEbObject<EbChartVisualization>(TVRefId, ServiceClient, redis, service);
            if (string.IsNullOrEmpty(ChartVisualization.DataSourceRefId))
                throw new FormException($"Missing Data Reader of Chart control view that is connected to {this.Label}.");
            EbDataReader DrObj = EbFormHelper.GetEbObject<EbDataReader>(ChartVisualization.DataSourceRefId, ServiceClient, redis, service);
            this.ParamsList = DrObj.GetParams(redis as RedisClient);
        }

        public override string GetBareHtml()
        {
            string html = @"
<div id='cont_@ebsid@' class='chart-control-cont'>
    <div id='content_@ebsid@' class='wrapper-cont'>
        <div id='@ebsid@' >
			<div id='canvasDivchart@ebsid@'></div>
		</div>
    </div>
</div>"
    .Replace("@ebsid@", this.EbSid_CtxId);

            return html;
        }
    }
}
