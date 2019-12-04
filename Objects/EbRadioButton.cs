
using ExpressBase.Common;
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
    public enum EbValueType {
        Boolean = 3,
        Integer = 11,
        String = 16,
    }

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
			this.BareControlHtml4Bot = this.BareControlHtml;
			this.ObjType = this.GetType().Name.Substring(2, this.GetType().Name.Length - 2);
        }

        [HideInPropertyGrid]
        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl)]
        public override EbDbTypes EbDbType { get { return (EbDbTypes)ValueType; } }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl)]
        [PropertyGroup("Behavior")]
        [PropertyPriority(53)]
        [OnChangeExec(@"
        if(this.ValueType === 16){
            pg.ShowProperty('TrueValue_S');
            pg.HideProperty('TrueValue_I');
            pg.ShowProperty('FalseValue_S');
            pg.HideProperty('FalseValue_I');
        }
        else if(this.ValueType === 11){
            pg.ShowProperty('TrueValue_I');
            pg.HideProperty('TrueValue_S');
            pg.ShowProperty('FalseValue_I');
            pg.HideProperty('FalseValue_S');
        }
        else {
            pg.HideProperty('TrueValue_I');
            pg.HideProperty('TrueValue_S');
            pg.HideProperty('FalseValue_I');
            pg.HideProperty('FalseValue_S');
        }
")]
        public EbValueType ValueType { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl)]
        [PropertyGroup("Behavior")]
        [PropertyPriority(52)]
        [Alias("True Value")]
        [DefaultPropValue("T")]
        public string TrueValue_S { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl)]
        [PropertyGroup("Behavior")]
        [PropertyPriority(51)]
        [Alias("False Value")]
        [DefaultPropValue("F")]
        public string FalseValue_S { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl)]
        [PropertyGroup("Behavior")]
        [Alias("True Value")]
        [DefaultPropValue("1")]
        [PropertyPriority(52)]
        public int TrueValue_I { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl)]
        [PropertyGroup("Behavior")]
        [PropertyPriority(51)]
        [Alias("False Value")]
        [DefaultPropValue("0")]
        public int FalseValue_I { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl)]
        [HideInPropertyGrid]
        public string Tv
        {
            get
            {
                if (EbDbType == EbDbTypes.Int32)
                    return TrueValue_I.ToString();
                else if (EbDbType == EbDbTypes.String)
                    return TrueValue_S;
                else
                    return "true";
            }
        }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl)]
        [HideInPropertyGrid]
        public string Fv { get {
                if (EbDbType == EbDbTypes.Int32)
                    return FalseValue_I.ToString();
                else if (EbDbType == EbDbTypes.String)
                    return FalseValue_S;
                else
                    return "false";
            } }

        public override string DesignHtml4Bot
		{ get => @"<div class='toggle btn btn-xs btn-primary' data-toggle='toggle' style='width: 34px; height: 22px;'>
						<input type='checkbox' checked='' data-toggle='toggle' data-size='mini'>
						<div class='toggle-group'>
							<label class='btn btn-primary btn-xs toggle-on'>On</label>
							<label class='btn btn-default btn-xs active toggle-off'>Off</label>
							<span class='toggle-handle btn btn-default btn-xs'></span>
						</div>
					</div>";
			set => base.DesignHtml4Bot = value; }
		public override string GetHtml4Bot()
		{
			return ReplacePropsInHTML((HtmlConstants.CONTROL_WRAPER_HTML4BOT).Replace("@barehtml@", DesignHtml4Bot));
		}

		[HideInPropertyGrid]
        [JsonIgnore]
        public override string ToolIconHtml { get { return "<i class='fa fa-toggle-on'></i>"; } set { } }

        [HideInPropertyGrid]
        [JsonIgnore]
        public override string ToolNameAlias { get { return "Boolean"; } set { } }

        //public override string GetToolHtml()
        //{
        //    return @"<div eb-type='@toolName' class='tool'><i class='fa fa-toggle-on'></i>  @toolName</div>".Replace("@toolName", this.GetType().Name.Substring(2));
        //}

        [JsonIgnore]
        public override bool Unique { get; set; }

        public override string GetDesignHtml()
        {
            return GetHtml().RemoveCR().DoubleQuoted();
            //return @"<div class='toggle btn btn-xs btn-primary' data-toggle='toggle' style='width: 34px; height: 22px;'><input type='checkbox' checked='' data-toggle='toggle' data-size='mini'><div class='toggle-group'><label class='btn btn-primary btn-xs toggle-on'>On</label><label class='btn btn-default btn-xs active toggle-off'>Off</label><span class='toggle-handle btn btn-default btn-xs'></span></div></div>".RemoveCR().DoubleQuoted();
        }
        public override string GetBareHtml()
        {
            return @"<div class='radio-wrap'><input ui-inp class='bot-checkbox eb-chckbx' true-val='@Tv@' false-val='@Fv@' type ='checkbox' value-type='@ValueType@' id='@ebsid@'> <span id='@name@Lbl' class='eb-chckbxspan'> @label@  </span><br></div>"
.Replace("@ebsid@", String.IsNullOrEmpty(this.EbSid_CtxId) ? "@ebsid@" : this.EbSid_CtxId)
.Replace("@label@", this.Label)
.Replace("@ValueType@", this.EbDbType.ToString())
.Replace("@Tv@", this.Tv)
.Replace("@Fv@", this.Fv);
        }

//        public override string GetBareHtml()
//        {
//            return new EbCheckBox() { Label = this.Label, EbSid_CtxId = this.EbSid_CtxId , EbSid = this.EbSid }.GetBareHtml();
//            //return @"<div class='checkbox'>
//                    //    <label>
//                    //        <input type='radio' id='@ebsid@' ui-inp data-ebtype='@data-ebtype@' style='vertical-align: bottom;' data-toggle='tooltip'>
//                    //        <span class='eb-ctrl-label' ui-label id='@ebsidLbl'>@Label@ @req@ </span>
//                    //    </label>
//                    //</div>"
//;
//        }

        public override string GetHtml()
        {
            string EbCtrlHTML = HtmlConstants.CONTROL_WRAPER_HTML4WEB;
            return ReplacePropsInHTML(EbCtrlHTML);
        }

        [JsonIgnore]
        public override string GetValueJSfn { get { return @"
let $ctrl = $('#' + this.EbSid_CtxId);
let val = ($ctrl.prop('checked') == true) ? $ctrl.attr('true-val') :$ctrl.attr('false-val');
if($ctrl.attr('value-type') === '11')
    val = parseInt(val);
return val"; } set { } }

        //[JsonIgnore]
        //public override string GetValueJSfn { get { return @"return $('#' + this.EbSid_CtxId).prop('checked')? 'true': 'false';"; } set { } }

        [JsonIgnore]
        public override string SetValueJSfn { get { return @"$('#' + this.EbSid_CtxId).prop('checked', (p1 === 'T'? true: false)).trigger('change');"; } set { } }

    }
}