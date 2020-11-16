using ExpressBase.Common;
using ExpressBase.Common.Extensions;
using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using ExpressBase.Common.Structures;
using ExpressBase.Objects.Objects.DVRelated;
using System.Collections.Generic;

namespace ExpressBase.Objects
{
    public class EbMobileDataLabel : EbMobileDashBoardControl
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
