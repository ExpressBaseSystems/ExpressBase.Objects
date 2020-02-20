using ExpressBase.Common;
using ExpressBase.Common.Constants;
using ExpressBase.Common.Extensions;
using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using ExpressBase.Common.Structures;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace ExpressBase.Objects
{
    [EnableInBuilder(BuilderType.WebForm, BuilderType.UserControl)]
    public class EbDisplayPicture : EbControlUI
    {
        public EbDisplayPicture() { }

        [OnDeserialized]
        public void OnDeserializedMethod(StreamingContext context)
        {
            this.BareControlHtml = this.GetBareHtml();
            this.ObjType = this.GetType().Name.Substring(2, this.GetType().Name.Length - 2);
        }

        public override string ToolIconHtml { get { return "<i class='fa fa-picture-o'></i>"; } set { } }

        public override string ToolNameAlias { get { return "Display Picture"; } set { } }

        public override string UIchangeFns
        {
            get
            {
                return @"EbDisplayPicture = {
                adjustMaxHeight : function(elementId, props) {
                    $(`#cont_${elementId} .ebimg-cont img`).css('max-height', `${props.MaxHeight}px`);
                }
            }";
            }
        }

        public override string GetBareHtml()
        {
            return @" 
        <div class='ebimg-cont' style='width:100%; text-align:center;'>
            <img id='@name@' class='dpctrl-img' src='@src@'  style='opacity:0.5; max-width:100%; max-height:@maxheight@px;' alt='@alt@'>
        </div>
        <div class='dpctrl-options-cont' style='text-align: right;width: 100%;position: absolute;bottom: 0;background: transparent; visibility: hidden;'>
            <div style='height: 20px;width: 47px;display: none;text-align: center;border: 1px solid #ccc;border-radius: 20px;font-size: 12px;padding: 1px;background-color: #eee;cursor: pointer;'>
                <i class='fa fa-plus' aria-hidden='true'></i> Add
            </div>
            <div style='height: 20px;width: 70px;display: none;text-align: center;border: 1px solid #ccc;border-radius: 20px;font-size: 12px;padding: 1px;background-color: #eee;cursor: pointer;'>
                <i class='fa fa-trash' aria-hidden='true'></i> Remove
            </div>
            <div class='dpctrl-change' style='height: 20px;width: 70px;display: inline-block;text-align: center;border: 1px solid #ccc;border-radius: 20px;font-size: 12px;padding: 1px;background-color: #eee;cursor: pointer;'>
                <i class='fa fa-pencil' aria-hidden='true'></i> Change
            </div>
        </div>"
.Replace("@name@", this.Name)
.Replace("@src@", "/images/image.png")
.Replace("@toolTipText@", this.ToolTipText)
.Replace("@value@", "")//"value='" + this.Value + "'")
.Replace("@alt@ ", "Display Picture")
.Replace("@maxheight@", this.MaxHeight > 0 ? this.MaxHeight.ToString() : "200");
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
        public override EbDbTypes EbDbType { get { return EbDbTypes.String; } }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.UserControl)]
        [PropertyGroup(PGConstants.APPEARANCE)]
        [OnChangeUIFunction("EbDisplayPicture.adjustMaxHeight")]
        [DefaultPropValue("200")]
        public int MaxHeight { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.UserControl)]
        public bool Multiple { get; set; }

        //[EnableInBuilder(BuilderType.WebForm, BuilderType.UserControl)]
        //[OnChangeExec(@"if(this.CropResize === true){pg.ShowProperty('CropAspectRatio');}
        //else{pg.HideProperty('CropAspectRatio');}")]
        //public bool CropResize { get; set; }

        //[EnableInBuilder(BuilderType.WebForm, BuilderType.UserControl)]
        //[HideInPropertyGrid]
        //public List<string> AspectRatio { get { return new List<string> { "1:1", "4:3", "16:9" }; } }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.UserControl)]
        public DpAspectRatio CropAspectRatio { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.UserControl)]
        public bool EnableFullScreen { get; set; }

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
        public override EbScript VisibleExpr { get; set; }

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


        
        public override string EnableJSfn { get { return @"$('#cont_' + this.EbSid_CtxId + ' *').prop('disabled',false).find('[ui-inp]').css('background-color', '#fff'); $('#cont_' + this.EbSid_CtxId).find('.dpctrl-options-cont').show();"; } set { } }

        public override string DisableJSfn { get { return @"$('#cont_' + this.EbSid_CtxId + ' *').attr('disabled', 'disabled').find('[ui-inp]').css('background-color', '#f3f3f3'); $('#cont_' + this.EbSid_CtxId).find('.dpctrl-options-cont').hide()"; } set { } }

    }

    public enum DpAspectRatio
    {
        Free,
        Dp,
        Location
    }
}
