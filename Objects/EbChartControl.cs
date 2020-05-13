using ExpressBase.Common;
using ExpressBase.Common.Constants;
using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using ExpressBase.Common.Structures;
using ExpressBase.Objects.ServiceStack_Artifacts;
using Newtonsoft.Json;
using ServiceStack;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace ExpressBase.Objects
{

    [EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
    public class EbChartControl:EbTVcontrol
    {
        public EbChartControl() { }

        [OnDeserialized]
        public void OnDeserializedMethod(StreamingContext context)
        {
            this.BareControlHtml = this.GetBareHtml();
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


        public void InitFromDataBase(JsonServiceClient ServiceClient)
        {
            EbObjectParticularVersionResponse result = ServiceClient.Get<EbObjectParticularVersionResponse>(new EbObjectParticularVersionRequest { RefId = TVRefId });
            this.ChartVisualizationJson = result.Data[0].Json;
        }

        public void FetchParamsMeta(IServiceClient ServiceClient)
        {
            EbObjectParticularVersionResponse result1 = ServiceClient.Get<EbObjectParticularVersionResponse>(new EbObjectParticularVersionRequest { RefId = TVRefId });
            EbTableVisualization TvObj = EbSerializers.Json_Deserialize<EbTableVisualization>(result1.Data[0].Json);
            if (string.IsNullOrEmpty(TvObj.DataSourceRefId))
                throw new FormException($"Missing Data Reader of Chart control view that is connected to {this.Label}.");
            EbObjectParticularVersionResponse result2 = ServiceClient.Get<EbObjectParticularVersionResponse>(new EbObjectParticularVersionRequest { RefId = TvObj.DataSourceRefId });
            EbDataReader DrObj = EbSerializers.Json_Deserialize<EbDataReader>(result2.Data[0].Json);
            this.ParamsList = DrObj.InputParams;
        }

        public override string GetBareHtml()
        {
            string html = @"
<div id='cont_@ebsid@' class='chart-control-cont'>
    <div id='content_@ebsid@' class='wrapper-cont'>
        <div id='canvasDivchart@ebsid@'></div>
    </div>
</div>"
    .Replace("@ebsid@", this.EbSid_CtxId);

            return html;
        }
    }
}
