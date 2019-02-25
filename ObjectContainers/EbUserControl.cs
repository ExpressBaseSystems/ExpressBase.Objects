using ExpressBase.Common;
using ExpressBase.Common.Extensions;
using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using ExpressBase.Objects.ServiceStack_Artifacts;
using ServiceStack;
using ServiceStack.Redis;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace ExpressBase.Objects
{
    [EnableInBuilder(BuilderType.UserControl, BuilderType.WebForm, BuilderType.FilterDialog)]
    [HideInToolBox]
    [BuilderTypeEnum(BuilderType.WebForm)]
    public class EbUserControl : EbForm, IEBRootObject
    {
        public EbUserControl() { }

        //public string RefId { get; set; }

        //[EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl)]
        //[UIproperty]
        //[Unique]
        //[OnChangeUIFunction("Common.LABEL")]
        //[PropertyEditor(PropertyEditorType.MultiLanguageKeySelector)]
        //public string Label { get; set; }

        public override string GetToolHtml()
        {
            return @"<div eb-type='@toolName' class='tool'><i class='fa fa-puzzle-piece'></i>  @toolName</div>".Replace("@toolName", this.GetType().Name.Substring(2));
        }

        public bool IsRenderMode { get; set; }

        public override string GetBareHtml()
        {
            return base.GetBareHtml();
        }

        public override string GetDesignHtml()
        {
            string html = @"
        <div id='cont_@ebsid@' ebsid='@ebsid@' name='@name@' class='Eb-ctrlContainer' ctype='@type@' style='@hiddenString@'>
            <span class='eb-ctrl-label' ui-label id='@ebsidLbl'>____ UserControl ____</span>
                
        </div>";
            return html.RemoveCR().DoubleQuoted();
        }

        public string GetInnerHtml()
        {
            string ctrlhtml = string.Empty;
            foreach (EbControl c in this.Controls)
            {
                ctrlhtml += c.GetHtml();
            }
            string coverhtml = @"            
                <div  id='@ebsid@Wraper' class='ctrl-cover'>
                    <span class='eb-ctrl-label' ui-label id='@ebsid@Lbl' style='font-style: italic; font-weight: normal;'>@Label@</span>
                    @barehtml@
                </div>
                <span class='helpText' ui-helptxt > @helpText@ </span>"
                .Replace("@barehtml@", ctrlhtml)
                .Replace("@name@", this.Name)
                .Replace("@type@", this.ObjType)
                .Replace("@ebsid@", this.EbSid)
                .Replace("@rmode@", IsRenderMode.ToString())
                .Replace("@id@", "cont_" + this.EbSid)
                .Replace("@Label@", string.Concat(this.DisplayName, " - ", this.VersionNumber));

            return ReplacePropsInHTML(coverhtml);
        }

        public override string GetHtml()
        {
            string ctrlhtml = string.Empty;
            foreach (EbControl c in this.Controls)
            {
                ctrlhtml += c.GetHtml();
            }
            string coverhtml = @"
            <div id='@id@' ebsid='@ebsid@' ctype='@type@' isrendermode='@rmode@' class='Eb-ctrlContainer ebcont-ctrl user-ctrl' eb-type='UserControl' tabindex='1'>
                <div  id='@ebsid@Wraper' class='ctrl-cover'>
                    
                    @barehtml@
                </div>
                <span class='helpText' ui-helptxt > @helpText@ </span>
            </div>"
                .Replace("@barehtml@", ctrlhtml)                                                   
                .Replace("@name@", this.Name)
                .Replace("@type@", this.ObjType)
                .Replace("@ebsid@", this.EbSid)
                .Replace("@rmode@", IsRenderMode.ToString())
                .Replace("@id@", "cont_" + this.EbSid)
                .Replace("@Label@", string.Concat(this.DisplayName, " - ", this.VersionNumber));

            return ReplacePropsInHTML(coverhtml);
        }

        public override string GetHtml(bool isRootObj)
        {
            if (isRootObj)
            {
                string html = string.Empty;
                foreach (EbControl c in this.Controls)
                {
                    html += c.GetHtml();
                    //<span class='eb-ctrl-label' ui-label id='@ebsidLbl'>@Label@</span>
                }

                string coverhtml = @"
                <div id='@id@' ebsid='@ebsid@' eb-form='true' ctype='@type@' isrendermode='@rmode@' class='ebcont-ctrl user-ctrl' eb-type='UserControl' tabindex='1'>
                    @barehtml@                        
                    <span class='helpText' ui-helptxt >@helpText@ </span>
                </div>"
                    .Replace("@barehtml@", html)
                    .Replace("@name@", this.Name)
                    .Replace("@type@", this.ObjType)
                    .Replace("@ebsid@", this.EbSid)
                    .Replace("@rmode@", IsRenderMode.ToString())
                    .Replace("@id@", this.EbSid);
                return ReplacePropsInHTML(coverhtml);
            }
            else
                return GetHtml();
        }

        public void AfterRedisGet(Service service)
        {
            EbFormHelper.AfterRedisGet(this, service);
        }

        public override void AfterRedisGet(RedisClient Redis, IServiceClient client)
        {
            EbFormHelper.AfterRedisGet(this, Redis, client);
        }

        public override string DiscoverRelatedRefids()
        {
            return EbFormHelper.DiscoverRelatedRefids(this);
        }
    }
}
