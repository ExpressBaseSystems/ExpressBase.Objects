using ExpressBase.Common;
using ExpressBase.Common.Constants;
using ExpressBase.Common.Extensions;
using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using ExpressBase.Common.Structures;
using ExpressBase.Objects.WebFormRelated;
using Newtonsoft.Json;
using ServiceStack.Redis;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace ExpressBase.Objects
{
    [EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
    public class EbEmailControl : EbControlUI
    {
        public EbEmailControl()
        {
        }
        [OnDeserialized]
        public void OnDeserializedMethod(StreamingContext context)
        {
            this.BareControlHtml = this.GetBareHtml();
            this.BareControlHtml4Bot = this.BareControlHtml;
            this.ObjType = this.GetType().Name.Substring(2, this.GetType().Name.Length - 2);
        }
        public override string ToolIconHtml { get { return "<i class='fa fa-envelope '></i>"; } set { } }

        public override string ToolNameAlias { get { return "EmailControl"; } set { } }

        public override string ToolHelpText { get { return "Email"; } set { } }
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
        public override bool Unique { get; set; }

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
        public override EbDbTypes EbDbType { get { return EbDbTypes.String; } set { } }


        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl)]
        [PropertyEditor(PropertyEditorType.Expandable)]
        [PropertyGroup(PGConstants.APPEARANCE)]
        [UIproperty]
        [DefaultPropValue(7, 10, 7, 10)]
        [OnChangeUIFunction("Common.INP_PADDING")]
        public UISides Padding { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.UserControl)]
        [Alias("Send OTP")]
        public bool Sendotp { get; set; }

        public override string GetBareHtml()
        {
            return @"<div class='input-group @ebsid@_cont'>
						  <span class='input-group-addon'> <i class='fa fa-envelope aria-hidden='true' class='input-group-addon'></i> </span>
						  <input type='email' ui-inp placeholder='' id='@ebsid@' name='@name@' style='width:100%; display:inline-block;'>
					</div>"
.Replace("@ebsid@", String.IsNullOrEmpty(this.EbSid_CtxId) ? "@ebsid@" : this.EbSid_CtxId)
.Replace("@name@", this.Name)
.Replace("@toolTipText@", this.ToolTipText)
.Replace("@value@", "");

        }

        public override string GetDesignHtml()
        {
            return GetHtml().RemoveCR().DoubleQuoted();
        }

        public override string GetHtml()
        {
            string EbCtrlHTML = HtmlConstants.CONTROL_WRAPER_HTML4WEB
               .Replace("@LabelForeColor ", "color:" + (LabelForeColor ?? "@LabelForeColor ") + ";")
               .Replace("@LabelBackColor ", "background-color:" + (LabelBackColor ?? "@LabelBackColor ") + ";");

            return ReplacePropsInHTML(EbCtrlHTML);
        }

        public override string DesignHtml4Bot
        {
            get => this.GetBareHtml();
            set => base.DesignHtml4Bot = value;
        }

        public override string GetValueFromDOMJSfn
        {
            get
            {
                return @" return $(`#${this.EbSid}`).val();";
            }
            set { }
        }

        public override string OnChangeBindJSFn
        {
            get
            {
                return @"$(`#${this.EbSid}`).on('change', p1);";
            }
            set { }
        }

        public override string SetValueJSfn
        {
            get
            {
                return @" $(`#${this.EbSid}`).val(p1);";
            }
            set { }
        }

        [JsonIgnore]
        public string FormRefId { get; set; }

        [JsonIgnore]
        internal IRedisClient RedisClient { get; set; }

        public void GetVerificationStatus(EbDataColumn dataColumn, EbDataRow dataRow, SingleRow Row)
        {
            string val = Convert.ToString(dataRow[dataColumn.ColumnIndex]);
            SingleColumn _column = Row.GetColumn(this.Name);
            Dictionary<string, string> meta = new Dictionary<string, string>
            {
                { FormConstants.email_id, Convert.ToString(_column.Value) },
                { FormConstants.is_verified, val == "T"? "true": "false" }
            };
            _column.M = JsonConvert.SerializeObject(meta);
        }

        public override bool ParameterizeControl(ParameterizeCtrl_Params args, string crudContext)
        {
            return EbPhone.ParameterizeControl(args, crudContext, this.FormRefId, this.Name, this.RedisClient, this.Sendotp);
        }
    }
}
