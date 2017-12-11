using ExpressBase.Common;
using ExpressBase.Common.Extensions;
using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using ExpressBase.Data;
using ExpressBase.Objects.ServiceStack_Artifacts;
using ServiceStack;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace ExpressBase.Objects
{

    [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.BotForm)]
    public class EbSimpleSelect : EbControl
    {

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm)]
        [OSE_ObjectTypes(EbObjectType.DataSource)]
        [PropertyEditor(PropertyEditorType.ObjectSelector)]
        public string DataSourceId { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm)]
        public int Value { get; set; }

        public EbSimpleSelect() { }

        public string GetOptionsHtml() {
            this.DataSourceId = "eb_roby_dev-eb_roby_dev-2-1015-1736";
            string _html = string.Empty;
            JsonServiceClient ServiceClient = new JsonServiceClient();
            //DataSourceColumnsResponse cresp = this.Redis.Get<DataSourceColumnsResponse>(string.Format("{0}_columns", this.DataSourceId));
            //if (cresp.IsNull)
            DataSourceColumnsResponse cresp = ServiceClient.Get<DataSourceColumnsResponse>(new DataSourceColumnsRequest { RefId = this.DataSourceId });

            ColumnColletion __columns = (cresp.Columns.Count > 1) ? cresp.Columns[1] : cresp.Columns[0];

            DataSourceDataResponse dresp = ServiceClient.Get<DataSourceDataResponse>(new DataSourceDataRequest { RefId = this.DataSourceId, Draw = 1, Start = 0, Length = 100 });
            var dt = dresp.Data;

            //this.DataSourceId
            //foreach( string option in options)
            //{
            //    _html += "<option> @option@ </option>".Replace("@option@", option);
            //}
            _html = "<option>select1</option><option>select2</option><option>select3</option>";
            return _html;
        }

        [OnDeserialized]
        public void OnDeserializedMethod(StreamingContext context)
        {
            this.BareControlHtml = this.GetBareHtml();
            this.Type = this.GetType().Name.Substring(2, this.GetType().Name.Length - 2);
        }

        public override string GetHead()
        {
            return this.RequiredString + @"
<script>

</script>
";
        }

        public override string GetDesignHtml()
        {
            return @"

    <div id='cont_@name@' Ctype='SimpleSelect' class='Eb-ctrlContainer' style='@hiddenString'>
        <div id='@name@' class='btn-group bootstrap-select show-tick' style='width: 100%;'><button type='button' class='btn dropdown-toggle btn-default'><span class='filter-option pull-left'>Simple Select</span>&nbsp;<span class='bs-caret'><span class='caret'></span></span></button></div>
    </div>".RemoveCR().DoubleQuoted();//GetHtmlHelper(RenderMode.Developer).RemoveCR().DoubleQuoted();
        }

        public override string GetHtml()
        {
            return GetHtmlHelper(RenderMode.User);
        }

        public override string GetBareHtml()
        {
            return @"
        <select id='@name@'>
            @options@
        </select>"
.Replace("@options@", this.GetOptionsHtml())
.Replace("@name@", this.Name);
        }

        private string GetHtmlHelper(RenderMode mode)
        {
            return this.GetBareHtml();
        }
    }
}
