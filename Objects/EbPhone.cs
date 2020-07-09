using ExpressBase.Common;
using ExpressBase.Common.Constants;
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
	[EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
	public class EbPhone : EbControlUI
	{

		public EbPhone()
		{
			this.Templates = new List<ObjectBasicInfo>();
		}
		[OnDeserialized]
		public void OnDeserializedMethod(StreamingContext context)
		{
			this.BareControlHtml = this.GetBareHtml();
			this.BareControlHtml4Bot = this.BareControlHtml;
			this.ObjType = this.GetType().Name.Substring(2, this.GetType().Name.Length - 2);
		}
		public override string ToolIconHtml { get { return "<i class='fa fa-phone'></i>"; } set { } }

		public override string ToolNameAlias { get { return "Phone"; } set { } }

		public override string ToolHelpText { get { return "Phone"; } set { } }
		public override string UIchangeFns
		{
			get
			{
				return @"EbTagInput = {
                
            }";
			}
		}

		//--------Hide in property grid------------
		[EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
		[HideInPropertyGrid]
		public override string HelpText { get; set; }

		[EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
		[HideInPropertyGrid]
		public override string ToolTipText { get; set; }

		[EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
		[HideInPropertyGrid]
		public override bool Unique { get; set; }

		[EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
		[HideInPropertyGrid]
		public override List<EbValidator> Validators { get; set; }

		[EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
		[HideInPropertyGrid]
		public override EbScript DefaultValueExpression { get; set; }

		[EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
		[HideInPropertyGrid]
		public override EbScript HiddenExpr { get; set; }
		
		[EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
		[HideInPropertyGrid]
		public override EbScript DisableExpr { get; set; }

		[EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
		[HideInPropertyGrid]
		public override EbScript ValueExpr { get; set; }

		[EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
		[HideInPropertyGrid]
		public override bool Required { get; set; }

		[EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
		[HideInPropertyGrid]
		public override bool DoNotPersist { get; set; }

		[EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
		[HideInPropertyGrid]
		public override string BackColor { get; set; }

		[EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
		[HideInPropertyGrid]
		public override string ForeColor { get; set; }

		[EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
		[HideInPropertyGrid]
		public override string LabelBackColor { get; set; }

		[EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
		[HideInPropertyGrid]
		public override string LabelForeColor { get; set; }

		[EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
		[HideInPropertyGrid]
		public override EbScript OnChangeFn { get; set; }

		[EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
		[HideInPropertyGrid]
		public override EbDbTypes EbDbType { get { return EbDbTypes.String; } set { } }


		[EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
		[DefaultPropValue("100")]
		[Alias("Dropdown Height")]
		public int DropdownHeight { get; set; }

		[EnableInBuilder(BuilderType.WebForm,  BuilderType.UserControl)]
		[PropertyEditor(PropertyEditorType.ObjectSelectorCollection)]
		[OSE_ObjectTypes(EbObjectTypes.iSmsBuilder)]
		public List<ObjectBasicInfo> Templates { get; set; }

		[EnableInBuilder(BuilderType.WebForm,  BuilderType.UserControl)]
		[DefaultPropValue("false")]
		[Alias("Send message")]
		[OnChangeExec(@"if (this.SendMessage === true ){
								pg.ShowProperty('Templates');
							} 
							else {
								pg.HideProperty('Templates');
							}")]
		public bool SendMessage { get; set; }

		public override string GetBareHtml()
		{
			return @"<div class='PhnCtrlCont' id='@ebsid@_Phnctrl' name='@name@'>
					 <input type='tel' placeholder='' class='phnctrl' id='@ebsid@' style='width:100%; display:inline-block;'>
						<button class='phnContextBtn'><i class='fa fa-bars' style='color:#2980b9' @SendMessagebtn@></i></button>
					</div>"
.Replace("@ebsid@", String.IsNullOrEmpty(this.EbSid_CtxId) ? "@ebsid@" : this.EbSid_CtxId)
.Replace("@name@", this.Name)
.Replace("@toolTipText@", this.ToolTipText)
.Replace("@SendMessagebtn@", this.SendMessage ? "hidden" : "")
.Replace("@value@", "");

		}

		public override string GetDesignHtml()
		{
			return GetHtml().RemoveCR().DoubleQuoted();
		}

		public override string DesignHtml4Bot
		{
			get => @"<div class='PhnCtrlCont' id='@ebsid@_Phnctrl' name='@name@'>
					 <input type='tel' placeholder='' class='phnctrl' id='@ebsid@' style='width:100%; display:inline-block;'>
					</div>";
			set => base.DesignHtml4Bot = value;
		}
		public override string GetHtml()
		{
			string EbCtrlHTML = HtmlConstants.CONTROL_WRAPER_HTML4WEB
			   .Replace("@LabelForeColor ", "color:" + (LabelForeColor ?? "@LabelForeColor ") + ";")
			   .Replace("@LabelBackColor ", "background-color:" + (LabelBackColor ?? "@LabelBackColor ") + ";");

			return ReplacePropsInHTML(EbCtrlHTML);
		}

		[JsonIgnore]
		public override string DisableJSfn
		{
			get { return @"this.__IsDisable = true;
            $('#' + this.EbSid_CtxId ).attr('disabled', 'disabled').css('pointer-events', 'none').find('[ui-inp]').css('background-color', '#f3f3f3');
            $('#' + this.EbSid + '_Phnctrl').find('.iti__flag-container').attr('disabled', 'disabled').css('pointer-events', 'none').css('background-color', '#f3f3f3');
           if(this.SendMessage){
			$('#cont_' + this.EbSid_CtxId).find('.phnContextBtn').show();
			}"; }
			set { }
		}
                  

		[JsonIgnore]
		public override string EnableJSfn { get { return @"this.__IsDisable = false; 
			  $('#cont_' + this.EbSid_CtxId + ' *').prop('disabled', false).css('pointer-events', 'inherit').find('[ui-inp]').css('background-color', '#fff');
					$('#cont_' + this.EbSid_CtxId).find('.phnContextBtn').hide();"; }
			set { }
		}


	}
}
