using ExpressBase.Common.Extensions;
using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExpressBase.Objects
{
    [EnableInBuilder(BuilderType.WebForm, BuilderType.UserControl)]
    class EbGroupBox : EbControlContainer
    {
        public EbGroupBox()
        {
            this.Controls = new List<EbControl>();
            this.ObjType = this.GetType().Name.Substring(2, this.GetType().Name.Length - 2);
        }

        public override string UIchangeFns
        {
            get
            {
                return @"EbGroupBox = {
                    padding : function(elementId, props) {
                        $(`#cont_${ elementId}.Eb-ctrlContainer`).closestInner('.group-box').css('padding', props.Padding + 'px');
                    }
                }";
            }
        }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.UserControl)]
        [UIproperty]
        [OnChangeUIFunction("Common.LABEL")]
        public override string Label { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.UserControl)]
        [PropertyGroup("Test")]
        [OnChangeUIFunction("EbGroupBox.padding")]
        [UIproperty]
        [DefaultPropValue("3")]
        public int Padding { get; set; }

        public override string GetToolHtml()
        {
            return @"<div eb-type='@toolName' class='tool'><i class='fa fa-square-o'></i>  @toolName</div>".Replace("@toolName", this.GetType().Name.Substring(2));
        }

        public override string GetDesignHtml()
        {
            return GetHtml().RemoveCR().DoubleQuoted();
        }

        public override string GetHtml()
        {
            string html = @"
            <div id='cont_@ebsid@' ebsid='@ebsid@' class='Eb-ctrlContainer ebcont-ctrl' ctype='@objtype@'>
                <div class='gb-wraper'>
                    <span class='gb-label' ui-label>@glabel@</span>
                    <div class='gb-border'>
                        <div class='group-box'>";

            foreach (EbControl ec in this.Controls)
                html += ec.GetHtml();

            return (html + "</div></div></div>")
                .Replace("@name@", this.Name)
                .Replace("@ebsid@", this.EbSid_CtxId)
                .Replace("@glabel@", this.Label)
                .Replace("@objtype@", this.ObjType);
        }

    }
}
