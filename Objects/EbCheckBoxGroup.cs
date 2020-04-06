using ExpressBase.Common;
using ExpressBase.Common.Extensions;
using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using ExpressBase.Objects.Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace ExpressBase.Objects
{
    [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl)]
    public class EbCheckBoxGroup : EbControlUI
    {
        public EbCheckBoxGroup()
        {
            this.CheckBoxes = new List<EbCheckBox>();
        }

        [JsonIgnore]
        public override string OnChangeBindJSFn
        {
            get
            {
                return @"$('input[name = ' + this.EbSid_CtxId + ']').on('change', p1);;";
            }
            set { }
        }
        public override string GetValueFromDOMJSfn
		{
			get
			{
				return @"	
							var cval=[];
						$('[name=' + this.EbSid_CtxId + ']:checked').each(function(){
							cval.push($(this).val());
						});
					return cval.join();
                ";
			}
			set { }
		}
		public override string GetDisplayMemberFromDOMJSfn
        {
			get
			{
				return @"	
							var ctxt=[];
						$('[name=' + this.EbSid_CtxId + ']:checked').each(function(){
							ctxt.push($(this).next('span').text());
						});
					return ctxt.join('<br />');
                ";
			}
			set { }
		}

		[OnDeserialized]
        public void OnDeserializedMethod(StreamingContext context)
        {
            this.BareControlHtml = this.GetBareHtml();
            this.BareControlHtml4Bot = this.BareControlHtml;
            this.ObjType = this.GetType().Name.Substring(2, this.GetType().Name.Length - 2);
        }


		public override string DesignHtml4Bot
		{
			get => @"
<div style='padding:5px'>
    <div class='check-wraper'>
        <input class='bot-checkbox' type='checkbox' value='@value@' id='@ebsid@' name='@gname@'> 
        <span id='@name@Lbl' style='@LabelBackColor @LabelForeColor '> CheckBox1  </span>
    </div>
    <div class='check-wraper'>
        <input class='bot-checkbox' type ='checkbox' value='@value@' id='@ebsid@' name='@gname@'> 
        <span id='@name@Lbl' style='@LabelBackColor @LabelForeColor '> CheckBox2  </span>
    </div>
</div>
";
			set => base.DesignHtml4Bot = value;
		}


		public override string GetHtml4Bot()
		{
			return ReplacePropsInHTML((HtmlConstants.CONTROL_WRAPER_HTML4BOT).Replace("@barehtml@", DesignHtml4Bot));
		}

		[EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl)]
        public decimal Value { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl)]
        [PropertyEditor(PropertyEditorType.Collection)]
        [Alias("CheckBoxes")]
        public List<EbCheckBox> CheckBoxes { get; set; }

        [HideInPropertyGrid]
        [EnableInBuilder(BuilderType.BotForm)]
        public override bool IsReadOnly { get => this.ReadOnly; }

        [HideInPropertyGrid]
        [JsonIgnore]
        public override string ToolIconHtml { get { return "<i class='fa fa-check-square'></i>"; } set { } }

        [HideInPropertyGrid]
        [JsonIgnore]
        public override string ToolNameAlias { get { return "CheckBoxes"; } set { } }

        //      public override string GetToolHtml()
        //{
        //	return @"<div eb-type='@toolName' class='tool'><i class='fa fa-check-square'></i> CheckBoxes </div>".Replace("@toolName", this.GetType().Name.Substring(2));
        //}

        public override string GetBareHtml()
        {
            string html = "<div id='@EbSid@' name='@name@'>".Replace("@EbSid@", (this.EbSid != null) ? this.EbSid : "@EbSid@");
            foreach (EbCheckBox ec in this.CheckBoxes)
            {
                ec.GName = this.EbSid_CtxId;
                html += ec.GetHtml();
            }
            html += "</div>";
            return html.Replace("@name@", (this.Name != null) ? this.Name : "@name@");
        }

        public override string GetDesignHtml()
        {
            string EbCtrlHTML = HtmlConstants.CONTROL_WRAPER_HTML4WEB
                .Replace("@barehtml@", @"
                <div style='padding:5px'>
                    <div class='check-wraper'>
                        <input class='bot-checkbox' type ='checkbox' value='@value@' id='@ebsid@' name='@gname@'> 
                            <span id='@name@Lbl' style='@LabelBackColor @LabelForeColor '> CheckBox1  </span>
                    </div>
                    <div class='check-wraper'>
                        <input class='bot-checkbox' type ='checkbox' value='@value@' id='@ebsid@' name='@gname@'> 
                            <span id='@name@Lbl' style='@LabelBackColor @LabelForeColor '> CheckBox2  </span>
                    </div>
                </div>
                ").RemoveCR().DoubleQuoted();
            //return GetHtml().RemoveCR().GraveAccentQuoted();
            return ReplacePropsInHTML(EbCtrlHTML);
        }

        public override string GetHtml()
        {

            //			string html = @"
            //			<div id='cont_@name@' class='Eb-ctrlContainer' ebsid='@ebsid@' Ctype='CheckBoxGroup'>
            //				<div class='radiog-cont'  style='@BackColor '>
            //				 <div class='eb-ctrl-label' id='@name@Lbl' style='@LabelBackColor @LabelForeColor '> @Label@  </div>
            //						@barehtml@
            //				<span class='helpText'> @HelpText </span></div>
            //			</div>"
            //.Replace("@barehtml@", this.GetBareHtml())
            //.Replace("@name@", (this.Name != null) ? this.Name : "@name@")
            //.Replace("@ebsid@", this.EbSid)
            //.Replace("@label@", this.Label)
            //.Replace("@LabelForeColor ", "color:" + ((this.LabelForeColor != null) ? this.LabelForeColor : "@LabelForeColor ") + ";")
            //.Replace("@LabelBackColor ", "background-color:" + ((this.LabelBackColor != null) ? this.LabelBackColor : "@LabelBackColor ") + ";")
            //.Replace("@BackColor ", ("background-color:" + ((this.BackColor != null) ? this.BackColor : "@BackColor ") + ";"));
            //            Console.WriteLine("==================================================================");
            //            Console.WriteLine(html.RemoveCR());
            //            return html;


            string EbCtrlHTML = HtmlConstants.CONTROL_WRAPER_HTML4WEB
               .Replace("@LabelForeColor ", "color:" + (LabelForeColor ?? "@LabelForeColor ") + ";")
               .Replace("@LabelBackColor ", "background-color:" + (LabelBackColor ?? "@LabelBackColor ") + ";");

            return ReplacePropsInHTML(EbCtrlHTML);

        }

        public override string GetJsInitFunc()
        {
            return @"
this.Init = function(id)
{
	this.CheckBoxes.$values.push(new EbObjects.EbCheckBox(id + '_Rd0'));
	this.CheckBoxes.$values.push(new EbObjects.EbCheckBox(id + '_Rd1'));
};";
        }
    }

    public class EbCheckBoxAbstract : EbControlUI
    {

    }

    [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl)]
    [HideInToolBox]
    public class EbCheckBox : EbCheckBoxAbstract
    {
        public EbCheckBox() { }

        [JsonIgnore]
        public override string GetValueFromDOMJSfn { get { return @" return $('#' + this.EbSid_CtxId).is(':checked'); "; } set { } }

        [JsonIgnore]
        public override string SetValueJSfn { get { return JustSetValueJSfn+ @"$('#' + this.EbSid_CtxId).trigger('change'); "; } set { } }

        [JsonIgnore]
        public override string JustSetValueJSfn
        {
            get
            {
                return JSFnsConstants.CB_JustSetValueJSfn;
            }
            set { }
        }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl)]
        public override string Label { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl)]
        public string Value { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl)]
        public string GName { get; set; }

        public override string GetBareHtml()
        {
            return @"<div class='radio-wrap'>
						<input ui-inp class='bot-checkbox eb-chckbx' type ='checkbox' value='@value@' id='@ebsid@' name='@gname@'> 
						<span id='@name@Lbl' class='eb-chckbxspan'> @label@  </span>
					<br>
					</div>"
.Replace("@ebsid@", String.IsNullOrEmpty(this.EbSid_CtxId) ? "@ebsid@" : this.EbSid_CtxId)
.Replace("@gname@", this.GName)
.Replace("@label@", this.Label)
.Replace("@value@", (this.Value == string.Empty ? "false" : this.Value));
        }

        public override string GetHtml()
        {
            return this.GetBareHtml(); ;
        }
    }
}
