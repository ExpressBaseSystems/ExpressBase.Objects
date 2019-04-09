
using ExpressBase.Common.Extensions;
using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using ExpressBase.Common.Structures;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace ExpressBase.Objects
{
    [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl)]
    public class EbRadioButton : EbControlUI
    {
        public EbRadioButton() { }

		[HideInPropertyGrid]
		[EnableInBuilder(BuilderType.BotForm)]
		public override bool IsReadOnly { get => this.ReadOnly; }

		[OnDeserialized]
        public void OnDeserializedMethod(StreamingContext context)
        {
            this.BareControlHtml = this.GetBareHtml();
            this.ObjType = this.GetType().Name.Substring(2, this.GetType().Name.Length - 2);
        }

        [HideInPropertyGrid]
        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl)]
        public override EbDbTypes EbDbType { get { return EbDbTypes.Boolean; } }

        public override string DesignHtml4Bot { get => @"<div class='toggle btn btn-xs btn-primary' data-toggle='toggle' style='width: 34px; height: 22px;'><input type='checkbox' checked='' data-toggle='toggle' data-size='mini'><div class='toggle-group'><label class='btn btn-primary btn-xs toggle-on'>On</label><label class='btn btn-default btn-xs active toggle-off'>Off</label><span class='toggle-handle btn btn-default btn-xs'></span></div></div>"; set => base.DesignHtml4Bot = value; }

        public override string GetToolHtml()
        {
            return @"<div eb-type='@toolName' class='tool'><i class='fa fa-toggle-on'></i>  @toolName</div>".Replace("@toolName", this.GetType().Name.Substring(2));
        }

        public override string GetDesignHtml()
        {
            return GetHtml().RemoveCR().DoubleQuoted();
            //return @"<div class='toggle btn btn-xs btn-primary' data-toggle='toggle' style='width: 34px; height: 22px;'><input type='checkbox' checked='' data-toggle='toggle' data-size='mini'><div class='toggle-group'><label class='btn btn-primary btn-xs toggle-on'>On</label><label class='btn btn-default btn-xs active toggle-off'>Off</label><span class='toggle-handle btn btn-default btn-xs'></span></div></div>".RemoveCR().DoubleQuoted();
        }

        public override string GetHead()
        {
            return this.RequiredString + @"
$('#@idcontainer [type=checkbox]').bootstrapToggle();
$('#@idcontainer [type=radio]').on('click', function () {
    $(this).button('toggle');
})




".Replace("@id", this.Name);
        }

        public override string GetBareHtml()
        {
            return @"<div class='checkbox'>
                        <label>
                            <input type='checkbox' id='@ebsid@' ui-inp data-ebtype='@data-ebtype@' style='vertical-align: bottom;' data-toggle='tooltip'>
                            <span class='eb-ctrl-label' ui-label id='@ebsidLbl'>@Label@ @req@ </span>
                        </label>
                    </div>"
//.Replace("@EbSid@", this.EbSid)
//.Replace("@Label@", this.Label)
//.Replace("@ToolTipText@", this.ToolTipText)
//.Replace("@HelpText@", this.HelpText)
//.Replace("@backColor", "background-color:" + this.BackColorSerialized + ";")
//.Replace("@foreColor", "color:" + this.ForeColorSerialized + ";")
//.Replace("@lblBackColor", "background-color:" + this.LabelBackColorSerialized + ";")
//.Replace("@LblForeColor", "color:" + this.LabelForeColorSerialized + ";");
;
        }

        public override string GetHtml()
        {
            string tt = @"<div id='cont_@ebsid@' ebsid='@ebsid@' name='@name@' class='Eb-ctrlContainer' @childOf@ ctype='RadioButton' eb-hidden='@isHidden@'>
                        <div  id='@ebsid@Wraper' class=''>
                            @barehtml@
                        </div>
                        <span class='helpText' ui-helptxt >@helpText@ </span>
                    </div>"
//.Replace("@ebsid@", this.EbSid)
//.Replace("@isHidden@", this.Hidden.ToString())
//.Replace("@barehtml@", this.GetBareHtml());
;
            return ReplacePropsInHTML(tt);
        }

        [JsonIgnore]
        public override string GetValueJSfn { get { return @"return $('#' + this.EbSid_CtxId).prop('checked')? 'true': 'false';"; } set { } }

        [JsonIgnore]
        public override string SetValueJSfn { get { return @"$('#' + this.EbSid_CtxId).prop('checked', (p1 === 'T'? true: false));"; } set { } }

    }
}