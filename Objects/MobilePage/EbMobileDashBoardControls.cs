using ExpressBase.Common;
using ExpressBase.Common.Constants;
using ExpressBase.Common.Extensions;
using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using ExpressBase.Common.Structures;
using ExpressBase.Objects.Objects.DVRelated;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExpressBase.Objects
{
    [EnableInBuilder(BuilderType.MobilePage)]
    public class EbMobileTableView : EbMobileDashBoardControls
    {
        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyEditor(PropertyEditorType.ObjectSelector)]
        [OSE_ObjectTypes(EbObjectTypes.iDataReader)]
        [PropertyGroup(PGConstants.DATA)]
        public string DataSourceRefId { set; get; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyEditor(PropertyEditorType.ScriptEditorCS)]
        [HelpText("sql query to get data from offline database")]
        [PropertyGroup(PGConstants.DATA)]
        public EbScript OfflineQuery { set; get; }

        public override string GetDesignHtml()
        {
            return @"<div class='mob_dash_control dropped' id='@id' eb-type='EbMobileTableView' tabindex='1' onclick='$(this).focus()'>                            
                            <div class='eb_dash_ctrlhtml'>
                               <table>
                                    <tr><th></th><th></th><th></th></tr>
                                    <tr><th></th><th></th><th></th></tr>
                                    <tr><th></th><th></th><th></th></tr>
                                </table>
                            </div>
                        </div>".RemoveCR().DoubleQuoted();
        }
    }

    [EnableInBuilder(BuilderType.MobilePage)]
    public class EbMobileLinks : EbMobileDashBoardControls
    {
        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyEditor(PropertyEditorType.ObjectSelector)]
        [PropertyGroup("Data Settings")]
        [OSE_ObjectTypes(EbObjectTypes.iMobilePage)]
        public string Object_Selector { get; set; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyGroup("Data Settings")]
        [OnChangeExec(@"
            if(this.LinkName !== ''){
                  $('#'+pg.CurObj.EbSid +' p').text(this.LinkName);
            }
            ")]
        public string LinkName { get; set; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyGroup("Appearance")]
        [UIproperty]
        [PropertyEditor(PropertyEditorType.Color)]
        [DefaultPropValue("")]
        [OnChangeExec(@"
            if(this.BackgroundColor !== ''){
                  $('#'+pg.CurObj.EbSid +' .link-outer').css('background-color',this.BackgroundColor);
            }
            ")]
        public string BackgroundColor { get; set; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyGroup("Appearance")]
        [UIproperty]
        [PropertyEditor(PropertyEditorType.FontSelector)]
        public EbFont FontStyle { get; set; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyGroup("Appearance")]
        [UIproperty]
        [DefaultPropValue("fa-link")]
        [OnChangeExec(@"
            if(this.HoverText !== ''){
                
            }
            ")]
        public string HoverText { get; set; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyGroup("Appearance")]
        [UIproperty]
        [OnChangeExec(@"
            if(this.BorderRadius > 0){
                $('#'+pg.CurObj.EbSid +' .link-outer').css('border-radius',this.BorderRadius +'px');
            }
            ")]
        public int BorderRadius { get; set; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyGroup("Icon")]
        [UIproperty]
        [PropertyEditor(PropertyEditorType.IconPicker)]
        [DefaultPropValue("fa-external-link-square")]
        [OnChangeExec(@"
            if(this.Icon  !== ''){
               $('#'+pg.CurObj.EbSid +' i').removeClass().addClass('fa ' +this.Icon);
            }
            ")]
        public string Icon { get; set; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyGroup("Icon")]
        [UIproperty]
        [PropertyEditor(PropertyEditorType.Color)]
        [DefaultPropValue("")]
        [OnChangeExec(@"
            if(this.IconBackgroundColor !== ''){
                $('#'+pg.CurObj.EbSid +' i').parent().css('background-color',this.IconBackgroundColor);
            }
            ")]
        public string IconBackgroundColor { get; set; }
        public override string GetDesignHtml()
        {
            return @"<div class='mob_dash_control dropped' id='@id' eb-type='EbMobileLinks' tabindex='1' onclick='$(this).focus()'>                            
                            <div class='eb_dash_ctrlhtml'>
                              <div class ='link-outer' style='border: solid 1px;height: 50px;width: 100%; display:flex' >                                 
                                <div style='border-right: solid 1px;height: 100%;width: 50%;'> 
                                <i style='font-size: 30px;align-content: center;width: 100%;text-align: center;padding-top: 8px;' class='fa fa-external-link-square'></i></div>
                                <div style='height: 100%;width: 100%;'> <p style='margin: 10px;font-size: 16px;'></p></div>
                               </div>
                            </div>
                        </div>".RemoveCR().DoubleQuoted();
        }

    }

    [EnableInBuilder(BuilderType.MobilePage)]
    public class EbMobileCharts : EbMobileDashBoardControls
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
            return @"<div class='mob_dash_control dropped' id='@id' eb-type='EbMobileCharts' tabindex='1' onclick='$(this).focus()'>                            
                            <div class='eb_dash_ctrlhtml'>
                              <div class ='link-outer' style='border: solid 1px;height: 22rem;width: 100%;' >                                 
                                <div style='border-bottom: solid 1px;height: 10%;'><p></p> </div>
                                <div style='height:90%;'> </div>
                               </div>
                            </div>
                        </div>".RemoveCR().DoubleQuoted();
        }
    }
    public enum MobileChartTypes
    {
        BarChart = 0,
        DonutChart,
        LineChart,
        PieChart,
        PointChart,
        RadarChart,
        RadialGaugeChart,
    }
    public class EbMobileDataLabel : EbMobileDashBoardControls
    {

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
        [PropertyPriority(1)]
        public List<DVBaseColumn> DataColumns { get; set; }


        //Static Label

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyGroup("StaticLabel")]
        [UIproperty]
        [OnChangeExec(@"if (this.StaticLabel != null ){      
                          $('#'+pg.CurObj.EbSid +' .staticlabel').text(this.StaticLabel);
                }
            ")]
        public string StaticLabel { get; set; }

        [EnableInBuilder(BuilderType.DashBoard)]
        [PropertyGroup("StaticLabel")]
        [UIproperty]
        [PropertyEditor(PropertyEditorType.FontSelector)]
        public EbFont StaticLabelFont { get; set; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyGroup("StaticLabel")]
        [UIproperty]
        [PropertyEditor(PropertyEditorType.Expandable)]
        public TextPositon StaticLabelPosition { get; set; }


        //dynamic label config

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyGroup("DynamicLabel")]
        [UIproperty]
        [PropertyEditor(PropertyEditorType.FontSelector)]
        public EbFont DynamicLabelFont { get; set; }


        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyGroup("DynamicLabel")]
        [PropertyEditor(PropertyEditorType.Expandable)]
        [UIproperty]
        public TextPositon DynamicLabelPositon { get; set; }



        //Description of Label

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyGroup("Description")]
        [OnChangeExec(@"if (this.Description != null ){      
                          $('#'+pg.CurObj.EbSid +' .description').text(this.Description);
                }
            ")]
        public override string Description { get; set; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyGroup("Description")]
        [UIproperty]
        [PropertyEditor(PropertyEditorType.FontSelector)]
        public EbFont DescriptionFont { get; set; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyGroup("Description")]
        [PropertyEditor(PropertyEditorType.Expandable)]
        [UIproperty]
        public TextPositon DescriptionPosition { get; set; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyGroup("Icon")]
        [UIproperty]
        [PropertyEditor(PropertyEditorType.IconPicker)]
        [DefaultPropValue("fa-building-o")]
        [OnChangeExec(@"if (this.Icon != '' ){      
                          $('#'+pg.CurObj.EbSid +' .m-i').removeClass().addClass('fa ' + this.Icon);
                }
            ")]
        public string Icon { get; set; }


        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyGroup("Icon")]
        [UIproperty]
        [DefaultPropValue("")]
        public string IconText { get; set; }


        [EnableInBuilder(BuilderType.DashBoard)]
        [PropertyGroup("Icon")]
        [UIproperty]
        [PropertyEditor(PropertyEditorType.Color)]
        public string IconColor { get; set; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyGroup("Icon")]
        [UIproperty]
        [DefaultPropValue("true")]
        [OnChangeExec(@"if (this.HideFooter === false ){      
                        pg.ShowProperty('FooterIcon');     
                        pg.ShowProperty('FooterIconColor');     
                        pg.ShowProperty('FooterText');     
                        pg.ShowProperty('FooterTextColor');    
                }
                else {     
                        pg.HideProperty('FooterIcon');     
                        pg.HideProperty('FooterIconColor');     
                        pg.HideProperty('FooterText');     
                        pg.HideProperty('FooterTextColor');                           
                }
            ")]
        public bool HideFooter { get; set; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyGroup("Icon")]
        [UIproperty]
        [PropertyEditor(PropertyEditorType.IconPicker)]
        [DefaultPropValue("fa-calendar-o")]
        [OnChangeExec(@"if (this.FooterIcon != '' ){      
                          $('#'+pg.CurObj.EbSid +' .f-i').removeClass().addClass('fa ' + this.FooterIcon);
                }
            ")]
        public string FooterIcon { get; set; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyGroup("Icon")]
        [UIproperty]
        [PropertyEditor(PropertyEditorType.Color)]
        [DefaultPropValue("#000000")]
        public string FooterIconColor { get; set; }


        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyGroup("Icon")]
        [UIproperty]
        [DefaultPropValue("Today")]
        [OnChangeExec(@"if (this.FooterText != null ){      
                          $('#'+pg.CurObj.EbSid +' .footer-txt').text(this.FooterText);
                }
            ")]
        public string FooterText { get; set; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyGroup("Icon")]
        [UIproperty]
        [PropertyEditor(PropertyEditorType.Color)]
        [DefaultPropValue("#000000")]
        public string FooterTextColor { get; set; }


        public override string GetDesignHtml()
        {
            return @"<div class='mob_dash_control dropped' id='@id' eb-type='EbMobileDataLabel' tabindex='1' onclick='$(this).focus()'>                            
                            <div class='eb_dash_ctrlhtml'>
                              <div class ='link-outer' style='border: solid 1px;height: 10rem;width: 100%;display:flex;' >                                 
                                <div style='border-right: solid 1px;width: 40%;height: 100%;text-align: center;padding-top: 10%;'> <i class='m-i' style='font-size: 45px;'></i> </div>
                                <div style='width:60%;'> <div style='height:75%'><div class='staticlabel'> @staticlabel@ </div> 
                            <div class='dynamiclabel'> @dynamiclabel@ </div> 
                            <div class='class='description-txt''> @description@ </div> </div>
                            <div class='' style='display:flex; height:25% ; border-top:solid 1px'> <i class='f-i' style='padding: 3%;'></i> <div class='footer-txt'></div> </div> 
                            </div>
                               </div>
                            </div>
                        </div>".RemoveCR().DoubleQuoted().Replace("@staticlabel@", this.StaticLabel)
                        .Replace("@description@", this.Description);
        }

    }
}
