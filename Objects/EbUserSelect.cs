using ExpressBase.Common;
using ExpressBase.Common.Extensions;
using ExpressBase.Common.LocationNSolution;
using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using ExpressBase.Common.Structures;
using ExpressBase.Security;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ExpressBase.Objects
{
    [EnableInBuilder(BuilderType.WebForm, BuilderType.UserControl)]
    //[HideInToolBox]
    public class EbUserSelect : EbControlUI, IEbPlaceHolderControl
    {
        public EbUserSelect() { }

        [OnDeserialized]
        public void OnDeserializedMethod(StreamingContext context)
        {
            this.BareControlHtml = this.GetBareHtml();
            this.ObjType = this.GetType().Name.Substring(2, this.GetType().Name.Length - 2);
        }

        public override string ToolIconHtml { get { return "<i class='fa fa-user'></i><i class='fa fa-check'></i>"; } set { } }

        public override string ToolNameAlias { get { return "User Select"; } set { } }

        public override string GetBareHtml()
        {
            return @" 
                <div id='@ebsid@' class='ulstc-disp-c'>
                    <div style='display: inherit;'>
                        <div class='ulstc-disp-img-c' style='background-image: url(/images/nulldp.png);'></div>
                        <div class='ulstc-disp-txt'>Jon Snow</div>
                    </div>
                    <div style='margin-left: auto; padding: 0px 10px;'><i class='fa fa-sort-desc' aria-hidden='true'></i></div>
                </div>"
            .Replace("@ebsid@", this.EbSid_CtxId)
            .Replace("@toolTipText@", this.ToolTipText);
        }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.UserControl)]
        [HideInPropertyGrid]
        public List<UserSelectOption> UserList { get; set; }

        public void InitOptions(Dictionary<int, string> Users)
        {
            this.UserList = new List<UserSelectOption>(); ;
            foreach (KeyValuePair<int, string> item in Users)
                if (item.Key > 1)
                    this.UserList.Add(new UserSelectOption() { vm = item.Key, dm1 = item.Value, img = item.Key.ToString() });
            this.UserList.Sort((pair1, pair2) => pair1.dm1.CompareTo(pair2.dm1));
        }

        public override string GetHtml()
        {
            string EbCtrlHTML = HtmlConstants.CONTROL_WRAPER_HTML4WEB
                .Replace("@LabelForeColor ", "color:" + ((this.LabelForeColor != null) ? this.LabelForeColor : "@LabelForeColor ") + ";")
                .Replace("@LabelBackColor ", "background-color:" + ((this.LabelBackColor != null) ? this.LabelBackColor : "@LabelBackColor ") + ";");

            return ReplacePropsInHTML(EbCtrlHTML);
        }

        public override string GetDesignHtml()
        {
            return this.GetHtml().RemoveCR().GraveAccentQuoted();
        }

		[EnableInBuilder(BuilderType.WebForm, BuilderType.UserControl)]
        [HideInPropertyGrid]
        public override EbDbTypes EbDbType { get { return EbDbTypes.Decimal; } }

        [EnableInBuilder(BuilderType.FilterDialog, BuilderType.WebForm)]
        public bool LoadCurrentUser { get; set; }

        public override string GetValueFromDOMJSfn
        {
            get
            {
                return @"let itemO = $('#' + this.EbSid_CtxId).data('data-obj');
                         if (itemO)
                            return itemO['vm'];
                         else
                            return '';";
            }
        }

        public override string OnChangeBindJSFn { get { return @"$('#' + this.EbSid_CtxId + ' input[type=hidden]').on('change', p1);"; } set { } }

        public override string JustSetValueJSfn { get { return "this._JsCtrlMng.setValue(p1, p2, true);"; } }

        public override string SetValueJSfn { get { return "this._JsCtrlMng.setValue(p1, p2, false);"; } }

        public override string GetDisplayMemberFromDOMJSfn { get { return "return this._JsCtrlMng.getDisplayMember();"; } }

        public override string RefreshJSfn { get { return "this._JsCtrlMng.refresh();"; } }

        public override string ClearJSfn { get { return "this._JsCtrlMng.clear();"; } }



        //--------Hide in property grid------------start

        [EnableInBuilder(BuilderType.WebForm)]
        [HideInPropertyGrid]
        public override bool Unique { get; set; }

        [EnableInBuilder(BuilderType.WebForm)]
        [HideInPropertyGrid]
        public override List<EbValidator> Validators { get; set; }

        [EnableInBuilder(BuilderType.WebForm)]
        [HideInPropertyGrid]
        public override EbScript DefaultValueExpression { get; set; }

        [EnableInBuilder(BuilderType.WebForm)]
        [HideInPropertyGrid]
        public override EbScript HiddenExpr { get; set; }
        
        [EnableInBuilder(BuilderType.WebForm)]
        [HideInPropertyGrid]
        public override EbScript DisableExpr { get; set; }

        [EnableInBuilder(BuilderType.WebForm)]
        [HideInPropertyGrid]
        public override EbScript ValueExpr { get; set; }

        [EnableInBuilder(BuilderType.WebForm)]
        [HideInPropertyGrid]
        public override string BackColor { get; set; }

        [EnableInBuilder(BuilderType.WebForm)]
        [HideInPropertyGrid]
        public override string ForeColor { get; set; }

        [EnableInBuilder(BuilderType.WebForm)]
        [HideInPropertyGrid]
        public override EbScript OnChangeFn { get; set; }

        //--------Hide in property grid------------end


        public override SingleColumn GetSingleColumn(User UserObj, Eb_Solution SoluObj, object Value)
        {
            object _formattedData = Value;
            string _displayMember = string.Empty;
            
            if (Value == null && this.LoadCurrentUser)
            {
                _formattedData = UserObj.UserId.ToString();
            }

            return new SingleColumn()
            {
                Name = this.Name,
                Type = (int)this.EbDbType,
                Value = _formattedData,
                Control = this,
                ObjType = this.ObjType,
                F = _displayMember
            };
        }

    }

    public class UserSelectOption
    {
        public int vm { get; set; }
        public string dm1 { get; set; }
        public string img { get; set; }
    }

}
