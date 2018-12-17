using ExpressBase.Common;
using ExpressBase.Common.Extensions;
using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using ExpressBase.Objects.ServiceStack_Artifacts;
using ServiceStack;
using ServiceStack.Redis;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExpressBase.Objects
{
    [EnableInBuilder(BuilderType.UserControl, BuilderType.WebForm, BuilderType.FilterDialog)]
    [HideInToolBox]
    public class EbUserControl : EbControlContainer
    {
        public EbUserControl() { }

        //public string RefId { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl)]
        [UIproperty]
        [Unique]
        [OnChangeUIFunction("Common.LABEL")]
        [PropertyEditor(PropertyEditorType.MultiLanguageKeySelector)]
        public string Label { get; set; }

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

        public override string GetHtml()
        {
            return this.GetHtml(false);
        }

        public override string GetHtml(bool isRootObj)
        {
            string html = string.Empty;
            foreach (EbControl c in this.Controls)
            {
                html += c.GetHtml();
            }

            string coverhtml = @"
        <div id='@id@' ebsid='@ebsid@' @ebformattr@ ctype='@type@' isrendermode='@rmode@' class='@Eb-ctrlContainer@ ebcont-ctrl user-ctrl' eb-type='UserControl' tabindex='1'>
            <span class='eb-ctrl-label' ui-label id='@ebsidLbl'>@Label@ </span>
                @indivstart@
                    @barehtml@
                @indivend@
            <span class='helpText' ui-helptxt >@helpText@ </span>
        </div>"
        .Replace("@barehtml@", html)
        .Replace("@indivstart@", isRootObj ? string.Empty: "<div  id='@ebsid@Wraper' class='ctrl-cover'>")
        .Replace("@indivend@", isRootObj ? string.Empty: "</div>");

            coverhtml = coverhtml
               .Replace("@name@", this.Name)
               .Replace("@type@", this.ObjType)
               .Replace("@ebsid@", this.EbSid)
               .Replace("@rmode@", IsRenderMode.ToString())

               .Replace("@id@", isRootObj ? this.EbSid : "cont_" + this.EbSid)
               .Replace("@ebformattr@", isRootObj ? "eb-form='true'" : string.Empty)
               .Replace("@Eb-ctrlContainer@", isRootObj ? string.Empty : " Eb-ctrlContainer ");
            return ReplacePropsInHTML(coverhtml);

        }

        public override void AfterRedisGet(RedisClient Redis, IServiceClient client)
        {
            try
            {
                for (int i = 0; i < this.Controls.Count; i++)
                {
                    if (this.Controls[i] is EbUserControl)
                    {
                        EbUserControl _temp = Redis.Get<EbUserControl>(this.Controls[i].RefId);
                        if (_temp == null)
                        {
                            var result = client.Get<EbObjectParticularVersionResponse>(new EbObjectParticularVersionRequest { RefId = this.Controls[i].RefId });
                            _temp = EbSerializers.Json_Deserialize(result.Data[0].Json);
                            Redis.Set<EbUserControl>(this.Controls[i].RefId, _temp);
                        }
                        _temp.RefId = this.Controls[i].RefId;
                        foreach (EbControl Control in _temp.Controls)
                        {
                            Control.ChildOf = "EbUserControl";
                        }
                        this.Controls[i] = _temp;
                        this.Controls[i].AfterRedisGet(Redis, client);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("EXCEPTION : UserControlAfterRedisGet " + e.Message);
            }
        }
    }
}
