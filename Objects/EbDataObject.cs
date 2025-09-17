using ExpressBase.Common.Constants;
using ExpressBase.Common.Extensions;
using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using ExpressBase.Common.Structures;
using ExpressBase.Objects.Objects.DVRelated;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace ExpressBase.Objects
{
    [EnableInBuilder(BuilderType.UserControl, BuilderType.WebForm, BuilderType.DashBoard)]
    public class EbDataObject : EbControlUI, IEbComponent
    {
        public EbDataObject() { }

        [OnDeserialized]
        public void OnDeserializedMethod(StreamingContext context)
        {   
            this.BareControlHtml = this.GetBareHtml();
            this.ObjType = this.GetType().Name.Substring(2, this.GetType().Name.Length - 2);
        }

        public override string ToolIconHtml { get { return "<i class='fa fa-database'></i>"; } set { } }

        public override string ToolNameAlias { get { return "Data Reader"; } set { } }

        public override string UIchangeFns
        {
            get
            {
                return @"EbDataObject = {
                dschanged : function(elementId, props) {
                    $(`#cont_${elementId} .Dt-Rdr-col-cont`).empty();
                    for (let i = 0; i < props['Columns'].$values.length; i++) {
                        let column = props['Columns'].$values[i];
                        let name = column.name;
                        $(`#cont_${elementId} .Dt-Rdr-col-cont`).append(`<div data-ctrl='${props.Name}' data-column='${name}' eb-type='DataLabel' type=${column.Type} class='col-div-blk'> ${name}</div>`);
                    }
                }
            }";
            }
        }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl, BuilderType.DashBoard)]
        [PropertyEditor(PropertyEditorType.ObjectSelector)]
        [PropertyGroup(PGConstants.DATA)]
        [OSE_ObjectTypes(EbObjectTypes.iDataReader)]
        [UIproperty]
        [OnChangeUIFunction("EbDataObject.dschanged")]
        [PropertyPriority(99)]
        public string DataSource { get; set; }

        [EnableInBuilder(BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.WebForm, BuilderType.UserControl, BuilderType.DashBoard)]
        [PropertyEditor(PropertyEditorType.CollectionProp, "Columns", "bVisible")]
        [PropertyGroup("Behavior")]        
        //[HideInPropertyGrid]
        public DVColumnCollection Columns { get; set; }


        [EnableInBuilder(BuilderType.WebForm, BuilderType.UserControl, BuilderType.DashBoard)]
        [HideInPropertyGrid]
        public override bool Hidden { get { return true; } }

        public override string GetBareHtml()
        {
            return @"<span class='eb-ctrl-label' ui-label id='@ebsidLbl'>____ Data Reader Bare HTML ____</span>";
        
        }

        public override string GetDesignHtml()
        {
            return GetHtml().RemoveCR().DoubleQuoted();
        }

        public override string GetHtml()
        {
            string EbCtrlHTML = @"
        <div id='cont_@ebsid@' ebsid='@ebsid@' name='@name@' class='Eb-ctrlContainer' @childOf@ ctype='@type@' eb-hidden='@isHidden@'>
            <span class='eb-ctrl-label' ui-label id='@ebsidLbl'> </span>
            <div ebclass='blk-cont' class='Dt-Rdr-col-cont'> </div>          
        </div>"
               .Replace("@LabelForeColor ", "color:" + (LabelForeColor ?? "@LabelForeColor ") + ";")
               .Replace("@LabelBackColor ", "background-color:" + (LabelBackColor ?? "@LabelBackColor ") + ";");

            return ReplacePropsInHTML(EbCtrlHTML);
        }


    }

    
}
