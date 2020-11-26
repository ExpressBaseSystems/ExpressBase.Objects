using ExpressBase.Common.Extensions;
using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using ExpressBase.Common.Structures;
using ExpressBase.Objects.Objects.DVRelated;
using System.Collections.Generic;

namespace ExpressBase.Objects
{
    [EnableInBuilder(BuilderType.MobilePage)]
    public class EbMobileChart : EbMobileDashBoardControl
    {
        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyPriority(1)]
        [PropertyGroup("Data")]
        public string ChartName { get; set; } 
        [OnChangeExec(@"
            
            ")]

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyPriority(1)]
        [PropertyGroup("Data")]
        public MobileChartTypes ChartTypes { get; set; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyEditor(PropertyEditorType.ObjectSelector)]
        [OSE_ObjectTypes(EbObjectTypes.iDataReader)]
        [PropertyGroup("Data")]
        [PropertyPriority(2)]
        [OnChangeExec(@"
            if(this.DataSourceRefId  !== ''){
        $.ajax({
            type: 'POST',
            url: '../DS/GetData4DashboardControl',
            data: { DataSourceRefId: this.DataSourceRefId, param: this.filtervalues },
            async: false,
            error: function (request, error) {
                this.loader.hide();
                EbPopBox('show', {
                    Message: 'Failed to get data from DataSourse',
                    ButtonStyle: {
                        Text: 'Ok',
                        Color: 'white',
                        Background: '#508bf9',
                        Callback: function () {
                            //$('.dash-loader').hide();
                        }
                    }
                });
            },
            success: function (resp) {
                var columns = JSON.parse(resp.columns);
                this.Columns = columns;
            }.bind(this)
        });   }     
    ")]
        public string DataSourceRefId { set; get; }


        [EnableInBuilder(BuilderType.MobilePage)]
        [MetaOnly]
        public DVColumnCollection Columns { get; set; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyEditor(PropertyEditorType.CollectionFrmSrc, "Columns")]
        [PropertyGroup("Data")]
        [PropertyPriority(3)]
        public List<DVBaseColumn> DataColumns { get; set; }
        public override string GetDesignHtml()
        {
            return @"<div class='mob_dash_control dropped' id='@id' eb-type='EbMobileChart' tabindex='1' onclick='$(this).focus()'>                            
                            <div class='eb_dash_ctrlhtml'>
                              <div class ='link-outer' style='border: solid 1px;height: 22rem;width: 100%;' >                                 
                                <div style='border-bottom: solid 1px;height: 10%;'><p></p> </div>
                                <div style='height:90%;'> </div>
                               </div>
                            </div>
                        </div>".RemoveCR().DoubleQuoted();
        }
    }
}
