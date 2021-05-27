using ExpressBase.Common;
using ExpressBase.Common.Constants;
using ExpressBase.Common.Extensions;
using ExpressBase.Common.LocationNSolution;
using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using ExpressBase.Common.Structures;
using ExpressBase.Objects.ServiceStack_Artifacts;
using ExpressBase.Objects.WebFormRelated;
using ExpressBase.Security;
using Newtonsoft.Json;
using ServiceStack.Redis;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Net;
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
            //this.CountryList=new List<string>();
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
        public override EbScript OnChangeFn { get; set; }

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

        [EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
        [DefaultPropValue("100")]
        [Alias("Dropdown Height")]
        public int DropdownHeight { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
        [Alias("Countries List(country code)")]
        public string CountriesList { get; set; }

        //		[PropertyPriority(9)]
        //		[EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
        //		[Unique]
        //		[DefaultPropValue("100")]
        //		[PropDataSourceJsFn(@"return {
        //ad: 'Andorra'
        //,ae: 'United Arab Emirates (‫الإمارات العربية المتحدة‬‎)'
        //,af: 'Afghanistan (‫افغانستان‬‎)'
        //,ag: 'Antigua and Barbuda'
        //,ai: 'Anguilla'
        //,al: 'Albania (Shqipëri)'
        //,am: 'Armenia (Հայաստան)'
        //,ao: 'Angola'
        //,ar: 'Argentina'
        //,as: 'American Samoa'
        //,at: 'Austria (Österreich)'
        //,au: 'Australia'
        //,aw: 'Aruba'
        //,ax: 'Åland Islands'
        //,az: 'Azerbaijan (Azərbaycan)'
        //,ba: 'Bosnia and Herzegovina (Босна и Херцеговина)'
        //,bb: 'Barbados'
        //,bd: 'Bangladesh (বাংলাদেশ)'
        //,be: 'Belgium (België)'
        //,bf: 'Burkina Faso'
        //,bg: 'Bulgaria (България)'
        //,bh: 'Bahrain (‫البحرين‬‎)'
        //,bi: 'Burundi (Uburundi)'
        //,bj: 'Benin (Bénin)'
        //,bl: 'Saint Barthélemy'
        //,bm: 'Bermuda'
        //,bn: 'Brunei'
        //,bo: 'Bolivia'
        //,bq: 'Caribbean Netherlands'
        //,br: 'Brazil (Brasil)'
        //,bs: 'Bahamas'
        //,bt: 'Bhutan (འབྲུག)'
        //,bw: 'Botswana'
        //,by: 'Belarus (Беларусь)'
        //,bz: 'Belize'
        //,ca: 'Canada'
        //,cc: 'Cocos (Keeling) Islands'
        //,cd: 'Congo (DRC) (Jamhuri ya Kidemokrasia ya Kongo)'
        //,cf: 'Central African Republic (République centrafricaine)'
        //,cg: 'Congo (Republic) (Congo-Brazzaville)'
        //,ch: 'Switzerland (Schweiz)'
        //,ci: 'Côte d’Ivoire'
        //,ck: 'Cook Islands'
        //,cl: 'Chile'
        //,cm: 'Cameroon (Cameroun)'
        //,cn: 'China (中国)'
        //,co: 'Colombia'
        //,cr: 'Costa Rica'
        //,cu: 'Cuba'
        //,cv: 'Cape Verde (Kabu Verdi)'
        //,cw: 'Curaçao'
        //,cx: 'Christmas Island'
        //,cy: 'Cyprus (Κύπρος)'
        //,cz: 'Czech Republic (Česká republika)'
        //,de: 'Germany (Deutschland)'
        //,dj: 'Djibouti'
        //,dk: 'Denmark (Danmark)'
        //,dm: 'Dominica'
        //,do: 'Dominican Republic (República Dominicana)'
        //,dz: 'Algeria (‫الجزائر‬‎)'
        //,ec: 'Ecuador'
        //,ee: 'Estonia (Eesti)'
        //,eg: 'Egypt (‫مصر‬‎)'
        //,eh: 'Western Sahara (‫الصحراء الغربية‬‎)'
        //,er: 'Eritrea'
        //,es: 'Spain (España)'
        //,et: 'Ethiopia'
        //,fi: 'Finland (Suomi)'
        //,fj: 'Fiji'
        //,fk: 'Falkland Islands (Islas Malvinas)'
        //,fm: 'Micronesia'
        //,fo: 'Faroe Islands (Føroyar)'
        //,fr: 'France'
        //,ga: 'Gabon'
        //,gb: 'United Kingdom'
        //,gd: 'Grenada'
        //,ge: 'Georgia (საქართველო)'
        //,gf: 'French Guiana (Guyane française)'
        //,gg: 'Guernsey'
        //,gh: 'Ghana (Gaana)'
        //,gi: 'Gibraltar'
        //,gl: 'Greenland (Kalaallit Nunaat)'
        //,gm: 'Gambia'
        //,gn: 'Guinea (Guinée)'
        //,gp: 'Guadeloupe'
        //,gq: 'Equatorial Guinea (Guinea Ecuatorial)'
        //,gr: 'Greece (Ελλάδα)'
        //,gt: 'Guatemala'
        //,gu: 'Guam'
        //,gw: 'Guinea-Bissau (Guiné Bissau)'
        //,gy: 'Guyana'
        //,hk: 'Hong Kong (香港)'
        //,hn: 'Honduras'
        //,hr: 'Croatia (Hrvatska)'
        //,ht: 'Haiti'
        //,hu: 'Hungary (Magyarország)'
        //,id: 'Indonesia'
        //,ie: 'Ireland'
        //,il: 'Israel (‫ישראל‬‎)'
        //,im: 'Isle of Man'
        //,in: 'India (भारत)'
        //,io: 'British Indian Ocean Territory'
        //,iq: 'Iraq (‫العراق‬‎)'
        //,ir: 'Iran (‫ایران‬‎)'
        //,is: 'Iceland (Ísland)'
        //,it: 'Italy (Italia)'
        //,je: 'Jersey'
        //,jm: 'Jamaica'
        //,jo: 'Jordan (‫الأردن‬‎)'
        //,jp: 'Japan (日本)'
        //,ke: 'Kenya'
        //,kg: 'Kyrgyzstan (Кыргызстан)'
        //,kh: 'Cambodia (កម្ពុជា)'
        //,ki: 'Kiribati'
        //,km: 'Comoros (‫جزر القمر‬‎)'
        //,kn: 'Saint Kitts and Nevis'
        //,kp: 'North Korea (조선 민주주의 인민 공화국)'
        //,kr: 'South Korea (대한민국)'
        //,kw: 'Kuwait (‫الكويت‬‎)'
        //,ky: 'Cayman Islands'
        //,kz: 'Kazakhstan (Казахстан)'
        //,la: 'Laos (ລາວ)'
        //,lb: 'Lebanon (‫لبنان‬‎)'
        //,lc: 'Saint Lucia'
        //,li: 'Liechtenstein'
        //,lk: 'Sri Lanka (ශ්‍රී ලංකාව)'
        //,lr: 'Liberia'
        //,ls: 'Lesotho'
        //,lt: 'Lithuania (Lietuva)'
        //,lu: 'Luxembourg'
        //,lv: 'Latvia (Latvija)'
        //,ly: 'Libya (‫ليبيا‬‎)'
        //,ma: 'Morocco (‫المغرب‬‎)'
        //,mc: 'Monaco'
        //,md: 'Moldova (Republica Moldova)'
        //,me: 'Montenegro (Crna Gora)'
        //,mf: 'Saint Martin (Saint-Martin (partie française))'
        //,mg: 'Madagascar (Madagasikara)'
        //,mh: 'Marshall Islands'
        //,mk: 'Macedonia (FYROM) (Македонија)'
        //,ml: 'Mali'
        //,mm: 'Myanmar (Burma) (မြန်မာ)'
        //,mn: 'Mongolia (Монгол)'
        //,mo: 'Macau (澳門)'
        //,mp: 'Northern Mariana Islands'
        //,mq: 'Martinique'
        //,mr: 'Mauritania (‫موريتانيا‬‎)'
        //,ms: 'Montserrat'
        //,mt: 'Malta'
        //,mu: 'Mauritius (Moris)'
        //,mv: 'Maldives'
        //,mw: 'Malawi'
        //,mx: 'Mexico (México)'
        //,my: 'Malaysia'
        //,mz: 'Mozambique (Moçambique)'
        //,na: 'Namibia (Namibië)'
        //,nc: 'New Caledonia (Nouvelle-Calédonie)'
        //,ne: 'Niger (Nijar)'
        //,nf: 'Norfolk Island'
        //,ng: 'Nigeria'
        //,ni: 'Nicaragua'
        //,nl: 'Netherlands (Nederland)'
        //,no: 'Norway (Norge)'
        //,np: 'Nepal (नेपाल)'
        //,nr: 'Nauru'
        //,nu: 'Niue'
        //,nz: 'New Zealand'
        //,om: 'Oman (‫عُمان‬‎)'
        //,pa: 'Panama (Panamá)'
        //,pe: 'Peru (Perú)'
        //,pf: 'French Polynesia (Polynésie française)'
        //,pg: 'Papua New Guinea'
        //,ph: 'Philippines'
        //,pk: 'Pakistan (‫پاکستان‬‎)'
        //,pl: 'Poland (Polska)'
        //,pm: 'Saint Pierre and Miquelon (Saint-Pierre-et-Miquelon)'
        //,pr: 'Puerto Rico'
        //,ps: 'Palestine (‫فلسطين‬‎)'
        //,pt: 'Portugal'
        //,pw: 'Palau'
        //,py: 'Paraguay'
        //,qa: 'Qatar (‫قطر‬‎)'
        //,re: 'Réunion (La Réunion)'
        //,ro: 'Romania (România)'
        //,rs: 'Serbia (Србија)'
        //,ru: 'Russia (Россия)'
        //,rw: 'Rwanda'
        //,sa: 'Saudi Arabia (‫المملكة العربية السعودية‬‎)'
        //,sb: 'Solomon Islands'
        //,sc: 'Seychelles'
        //,sd: 'Sudan (‫السودان‬‎)'
        //,se: 'Sweden (Sverige)'
        //,sg: 'Singapore'
        //,sh: 'Saint Helena'
        //,si: 'Slovenia (Slovenija)'
        //,sj: 'Svalbard and Jan Mayen'
        //,sk: 'Slovakia (Slovensko)'
        //,sl: 'Sierra Leone'
        //,sm: 'San Marino'
        //,sn: 'Senegal (Sénégal)'
        //,so: 'Somalia (Soomaaliya)'
        //,sr: 'Suriname'
        //,ss: 'South Sudan (‫جنوب السودان‬‎)'
        //,st: 'São Tomé and Príncipe (São Tomé e Príncipe)'
        //,sv: 'El Salvador'
        //,sx: 'Sint Maarten'
        //,sy: 'Syria (‫سوريا‬‎)'
        //,sz: 'Swaziland'
        //,tc: 'Turks and Caicos Islands'
        //,td: 'Chad (Tchad)'
        //,tg: 'Togo'
        //,th: 'Thailand (ไทย)'
        //,tj: 'Tajikistan'
        //,tk: 'Tokelau'
        //,tl: 'Timor-Leste'
        //,tm: 'Turkmenistan'
        //,tn: 'Tunisia (‫تونس‬‎)'
        //,to: 'Tonga'
        //,tr: 'Turkey (Türkiye)'
        //,tt: 'Trinidad and Tobago'
        //,tv: 'Tuvalu'
        //,tw: 'Taiwan (台灣)'
        //,tz: 'Tanzania'
        //,ua: 'Ukraine (Україна)'
        //,ug: 'Uganda'
        //,us: 'United States'
        //,uy: 'Uruguay'
        //,uz: 'Uzbekistan (Oʻzbekiston)'
        //,va: 'Vatican City (Città del Vaticano)'
        //,vc: 'Saint Vincent and the Grenadines'
        //,ve: 'Venezuela'
        //,vg: 'British Virgin Islands'
        //,vi: 'U.S. Virgin Islands'
        //,vn: 'Vietnam (Việt Nam)'
        //,vu: 'Vanuatu'
        //,wf: 'Wallis and Futuna (Wallis-et-Futuna)'
        //,ws: 'Samoa'
        //,xk: 'Kosovo'
        //,ye: 'Yemen (‫اليمن‬‎)'
        //,yt: 'Mayotte'
        //,za: 'South Africa'
        //,zm: 'Zambia'
        //,zw: 'Zimbabwe'};")]
        //		[PropertyEditor(PropertyEditorType.DropDown, true)]
        //		[Alias("Countries List(country code)")]
        //		public List<string> CountryList { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.UserControl)]
        [PropertyEditor(PropertyEditorType.ObjectSelectorCollection)]
        [OSE_ObjectTypes(EbObjectTypes.iSmsBuilder)]
        public List<ObjectBasicInfo> Templates { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.UserControl)]
        [DefaultPropValue("false")]
        [Alias("Send message")]
        [OnChangeExec(@"if (this.SendMessage === true ){
								pg.ShowProperty('Templates');
							} 
							else {
								pg.HideProperty('Templates');
							}")]
        public bool SendMessage { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.UserControl)]
        [DefaultPropValue("false")]
        [Alias("Send OTP")]
        public bool Sendotp { get; set; }


        [EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
        [DefaultPropValue("false")]
        [Alias("Disable country code")]
        public bool DisableCountryCode { get; set; }

        public override string GetBareHtml()
        {
            return @"<div class='PhnCtrlCont' id='@ebsid@_Phnctrl' name='@name@'>
					 <input type='tel' ui-inp placeholder='' class='phnctrl' id='@ebsid@' style='width:100%; display:inline-block;'>
						<div class='phnContextBtn'><i class='fa fa-bars' style='color:#2980b9' @SendMessagebtn@></i></div>
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
            $('#' + this.EbSid_CtxId ).attr('disabled', 'disabled').css('pointer-events', 'none').css('background-color', 'var(--eb-disablegray)');
            $('#' + this.EbSid + '_Phnctrl').find('.iti__flag-container').attr('disabled', 'disabled').css('pointer-events', 'none').css('background-color', 'var(--eb-disablegray)');
           if(this.SendMessage){
			$('#cont_' + this.EbSid_CtxId).find('.phnContextBtn').show();
			}"; }
            set { }
        }


        [JsonIgnore]
        public override string EnableJSfn
        {
            get { return @"this.__IsDisable = false; 
			  $('#cont_' + this.EbSid_CtxId + ' *').prop('disabled', false).css('pointer-events', 'inherit').find('[ui-inp]').css('background-color', '#fff');
					$('#cont_' + this.EbSid_CtxId).find('.phnContextBtn').hide();"; }
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
                { FormConstants.phone_no, Convert.ToString(_column.Value) },
                { FormConstants.is_verified, val == "T"? "true": "false" }
            };
            _column.M = JsonConvert.SerializeObject(meta);
        }

        public override SingleColumn GetSingleColumn(User UserObj, Eb_Solution SoluObj, object Value)
        {
            object _formattedData = Value;
            string _displayMember = Value == null ? string.Empty : Value.ToString();
            return new SingleColumn()
            {
                Name = this.Name,
                Type = (int)this.EbDbType,
                Value = _formattedData,
                Control = this,
                ObjType = this.ObjType,
                F = _displayMember,
                M = "{}"
            };
        }

        private void VerifyOTP(IDatabase DataDB, List<DbParameter> param, SingleColumn cField, int i, User usr)
        {
            Dictionary<string, string> _d = JsonConvert.DeserializeObject<Dictionary<string, string>>(cField.M);
            if (_d.ContainsKey(FormConstants.otp) && _d.ContainsKey(FormConstants.timestamp))
            {
                string otpKey = this.FormRefId + this.EbSid + usr.UserId + _d[FormConstants.timestamp];
                string otpInRedis = this.RedisClient.Get<string>(otpKey);
                if (otpInRedis == null)
                    throw new FormException("OTP expired. Please try again.", (int)HttpStatusCode.BadRequest, $"Otp is not found. timestamp: {_d[FormConstants.timestamp]}", "EbPhone => ParameterizeControl");

                if (otpInRedis == _d[FormConstants.otp])
                    param.Add(DataDB.GetNewParameter(cField.Name + FormConstants._verified + "_" + i, EbDbTypes.Boolean, true));
                else
                    throw new FormException("OTP mismatch. Please try again.", (int)HttpStatusCode.BadRequest, $"Otp verification failed due to mismatch. timestamp: {_d[FormConstants.timestamp]}", "EbPhone => ParameterizeControl");
            }
            else
            {
                param.Add(DataDB.GetNewParameter(cField.Name + FormConstants._verified + "_" + i, EbDbTypes.Boolean, false));
            }
        }

        public override bool ParameterizeControl(ParameterizeCtrl_Params args)
        {
            bool AvoidParam = false;

            if (args.cField.Value == null)
            {
                var p = args.DataDB.GetNewParameter(args.cField.Name, (EbDbTypes)args.cField.Type);
                p.Value = DBNull.Value;
                args.param.Add(p);
                if (this.Sendotp)
                {
                    args.param.Add(args.DataDB.GetNewParameter(args.cField.Name + FormConstants._verified + "_" + args.i, EbDbTypes.Boolean, false));
                }
            }
            else
            {
                args.param.Add(args.DataDB.GetNewParameter(args.cField.Name, (EbDbTypes)args.cField.Type, args.cField.Value));
                if (this.Sendotp)
                {
                    if (args.ins)
                    {
                        this.VerifyOTP(args.DataDB, args.param, args.cField, args.i, args.usr);
                    }
                    else
                    {
                        Dictionary<string, string> _od = JsonConvert.DeserializeObject<Dictionary<string, string>>(args.ocF.M);
                        if (_od[FormConstants.is_verified] == "true")
                        {
							if(String.Equals(args.cField.Value , args.ocF.Value))// phone number changed
								AvoidParam = true; 
                            else
								this.VerifyOTP(args.DataDB, args.param, args.cField, args.i, args.usr);
						}
                        else
                        {
                            this.VerifyOTP(args.DataDB, args.param, args.cField, args.i, args.usr);
                        }
                    }
                }
            }

            if (args.ins)
            {
                args._cols += string.Concat(args.cField.Name, ", ");
                args._vals += string.Concat("@", args.cField.Name, ", ");
                if (this.Sendotp)
                {
                    args._cols += string.Concat(args.cField.Name, FormConstants._verified, ", ");
                    args._vals += string.Concat("@", args.cField.Name, FormConstants._verified, "_", args.i, ", ");
                }
            }
            else
            {
                args._colvals += string.Concat(args.cField.Name, "=@", args.cField.Name, ", ");
                if (this.Sendotp && !AvoidParam)
                {
                    args._colvals += string.Concat(args.cField.Name, FormConstants._verified, "=@", args.cField.Name, FormConstants._verified, "_", args.i, ", ");
                }
            }
            args.i++;
            return true;
        }
    }
}
