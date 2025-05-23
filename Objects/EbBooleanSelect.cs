﻿using ExpressBase.Common;
using ExpressBase.Common.Constants;
using ExpressBase.Common.Extensions;
using ExpressBase.Common.LocationNSolution;
using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using ExpressBase.Common.Structures;
using ExpressBase.Security;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace ExpressBase.Objects
{
    [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl)]
    public class EbBooleanSelect : EbControlUI
    {

        public EbBooleanSelect()
        {
            this.EbSimpleSelect = new EbSimpleSelect();
        }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl)]
        [HideInPropertyGrid]
        public override EbDbTypes EbDbType { get { return EbDbTypes.BooleanOriginal; } }

        private EbSimpleSelect EbSimpleSelect { set; get; }

        public override string SetDisplayMemberJSfn
        {
            get
            {
                return @"if(p1 === true)
                            p1 = 'true'
                        else if(p1 === false)
                            p1 = 'false'
                       " + EbSimpleSelect.SetDisplayMemberJSfn;
            }
            set { }
        }

        public override string SetValueJSfn
        {
            get
            {
                return @"if(p1 === true)
                            p1 = 'true'
                        else if(p1 === false)
                            p1 = 'false'
                       " + EbSimpleSelect.SetValueJSfn;
            }
            set { }
        }

        public override string GetValueFromDOMJSfn
        {
            get
            {
                return EbSimpleSelect.GetValueFromDOMJSfn.Replace("return val;", "val = (val ==='true'); return val;");
            }
            set { }
        }

        public override string IsRequiredOKJSfn
        {
            get
            {
                return EbSimpleSelect.IsRequiredOKJSfn;
            }
            set { }
        }

        public override string GetDisplayMemberFromDOMJSfn
        {
            get
            {
                return EbSimpleSelect.GetDisplayMemberFromDOMJSfn;
            }
            set { }
        }

        public override string DisableJSfn
        {
            get
            {
                return EbSimpleSelect.DisableJSfn;
            }
            set { }
        }

        public override string EnableJSfn
        {
            get
            {
                return EbSimpleSelect.EnableJSfn;
            }
            set { }
        }

        public override string ToolIconHtml { get { return "<i class='fa fa-sort'></i>"; } set { } }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl)]
        [PropertyGroup(PGConstants.APPEARANCE)]
        public BootStrapClass BootStrapStyle { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl)]
        [PropertyGroup(PGConstants.HELP)]
        public string PlaceHolder { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl)]
        [PropertyGroup("Behavior")]
        [PropertyPriority(52)]
        [DefaultPropValue("Yes")]
        public string TrueText { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl)]
        [PropertyGroup("Behavior")]
        [PropertyPriority(51)]
        [DefaultPropValue("No")]
        public string FalseText { get; set; }


        [OnDeserialized]
        public void OnDeserializedMethod(StreamingContext context)
        {
            this.BareControlHtml = this.GetBareHtml();
            this.BareControlHtml4Bot = this.BareControlHtml;
            this.ObjType = this.GetType().Name.Substring(2, this.GetType().Name.Length - 2);
		}
		public override string DesignHtml4Bot
		{
			get => this.GetBareHtml();
			set => base.DesignHtml4Bot = value;
		}
		public override string GetHtml4Bot()
		{
			return ReplacePropsInHTML(HtmlConstants.CONTROL_WRAPER_HTML4BOT);
		}

		public override string GetDesignHtml()
        {
            //        return @"

            //<div id='cont_@name@' Ctype='SimpleSelect' class='Eb-ctrlContainer' eb-hidden='@isHidden@'>
            //    <div id='@name@' class='btn-group bootstrap-select show-tick' style='width: 100%;'><button type='button' class='btn dropdown-toggle btn-default'><span class='filter-option pull-left'>Simple Select</span>&nbsp;<span class='bs-caret'><span class='caret'></span></span></button></div>
            //</div>".RemoveCR().GraveAccentQuoted();//GetHtmlHelper(RenderMode.Developer).RemoveCR().DoubleQuoted();
            return GetHtml().RemoveCR().DoubleQuoted();
        }

        public override string GetHtml()
        {
            string EbCtrlHTML = HtmlConstants.CONTROL_WRAPER_HTML4WEB
                 .Replace("@LabelForeColor ", "color:" + (LabelForeColor ?? "@LabelForeColor ") + ";")
                 .Replace("@LabelBackColor ", "background-color:" + (LabelBackColor ?? "@LabelBackColor ") + ";");

            return ReplacePropsInHTML(EbCtrlHTML);
        }

        public override string GetBareHtml()
        {
            return @"
        <select id='@ebsid@' ui-inp class='selectpicker' name='@ebsid@' @bootStrapStyle@ data-ebtype='@data-ebtype@' style='width: 100%;'>
            <option value='true'>@TrueText@</option>
            <option value='false'>@FalseText@</option>
        </select>"
.Replace("@ebsid@", String.IsNullOrEmpty(this.EbSid_CtxId) ? "@ebsid@" : this.EbSid_CtxId)
.Replace("@name@", this.Name)
.Replace("@TrueText@", this.TrueText)
.Replace("@FalseText@", this.FalseText)
.Replace("@HelpText@", this.HelpText)

.Replace("@bootStrapStyle@", "data-style='btn-" + this.BootStrapStyle.ToString() + "'")
//.Replace("@-sel-@","<option selected value='-1' style='color: #6f6f6f;'> -- select -- </option>")
.Replace("@data-ebtype@", "30");
        }

        public override SingleColumn GetSingleColumn(User UserObj, Eb_Solution SoluObj, object Value, bool Default)
        {
            return EbBooleanSelect.GetSingleColumn(this, UserObj, SoluObj, Value);
        }

        public static SingleColumn GetSingleColumn(dynamic _this, User UserObj, Eb_Solution SoluObj, object Value)
        {
            object _formattedData = false;
            string _displayMember = _this.FalseText;
            if (Value != null)
            {
                if (Value.ToString() == "True")
                {
                    _formattedData = true;
                    _displayMember = _this.TrueText;
                }
            }

            return new SingleColumn()
            {
                Name = _this.Name,
                Type = (int)_this.EbDbType,
                Value = _formattedData,
                Control = _this,
                ObjType = _this.ObjType,
                F = _displayMember
            };
        }
    }
}
