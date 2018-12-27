using ExpressBase.Common;
using ExpressBase.Common.Extensions;
using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using ExpressBase.Common.Structures;
using ExpressBase.Data;
using ExpressBase.Objects.ServiceStack_Artifacts;
using ServiceStack;
using ServiceStack.Redis;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace ExpressBase.Objects
{
    [EnableInBuilder(BuilderType.WebForm)]
    [HideInToolBox]
    public class EbWebForm : EbControlContainer
    {
        [Browsable(false)]
        public bool IsUpdate { get; set; }

        public bool IsRenderMode { get; set; }

        public EbWebForm() { }

        public override int TableRowId { get; set; }


        public static EbOperations Operations = WFOperations.Instance;

        public override string GetHead()
        {
            string head = string.Empty;

            foreach (EbControl c in this.Controls)
                head += c.GetHead();

            return head;
        }

        public override string GetHtml()
        {
             string html = "<form id='@ebsid@' isrendermode='@rmode@' ebsid='@ebsid@' class='formB-box form-buider-form ebcont-ctrl' eb-form='true' ui-inp eb-type='WebForm' @tabindex@>";

            foreach (EbControl c in this.Controls)
                html += c.GetHtml();

            html += "</form>";

            return html
                .Replace("@name@", this.Name)
                .Replace("@ebsid@", this.EbSid)
                .Replace("@rmode@", IsRenderMode.ToString())
                .Replace("@tabindex@", IsRenderMode ? string.Empty : " tabindex='1'");
        }

        public string GetControlNames()
        {
            List<string> _lst = new List<string>();

            //foreach (EbControl _c in this.FlattenedControls)
            //{
            //    if (!(_c is EbControlContainer))
            //        _lst.Add(_c.Name);
            //}

            return string.Join(",", _lst.ToArray());
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

    public static class EbFormHelper
    {
        public static string DiscoverRelatedRefids(EbControlContainer _this)
        {
            string refids = string.Empty;
            for (int i = 0; i < _this.Controls.Count; i++)
            {
                if (_this.Controls[i] is EbUserControl)
                {
                    refids += _this.Controls[i].RefId + ",";
                }
                else
                {
                    PropertyInfo[] _props = _this.Controls[i].GetType().GetProperties();
                    foreach (PropertyInfo _prop in _props)
                    {
                        if (_prop.IsDefined(typeof(OSE_ObjectTypes)))
                            refids += _prop.GetValue(_this.Controls[i], null).ToString() + ",";
                    }
                }
            }
            return refids;
        }

        public static void AfterRedisGet(EbControlContainer _this, RedisClient Redis, IServiceClient client)
        {
            try
            {
                for (int i = 0; i < _this.Controls.Count; i++)
                {
                    if (_this.Controls[i] is EbUserControl)
                    {
                        EbUserControl _temp = Redis.Get<EbUserControl>(_this.Controls[i].RefId);
                        if (_temp == null)
                        {
                            var result = client.Get<EbObjectParticularVersionResponse>(new EbObjectParticularVersionRequest { RefId = _this.Controls[i].RefId });
                            _temp = EbSerializers.Json_Deserialize(result.Data[0].Json);
                            Redis.Set<EbUserControl>(_this.Controls[i].RefId, _temp);
                        }
                        //_temp.RefId = _this.Controls[i].RefId;
                        (_this.Controls[i] as EbUserControl).Controls = _temp.Controls;
                        foreach (EbControl Control in (_this.Controls[i] as EbUserControl).Controls)
                        {
                            Control.ChildOf = "EbUserControl";
                            Control.Name = _this.Controls[i].Name + "_" + Control.Name;
                        }
                        //_this.Controls[i] = _temp;
                        _this.Controls[i].AfterRedisGet(Redis, client);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("EXCEPTION : FormAfterRedisGet " + e.Message);
            }
        }

        public static void AfterRedisGet(EbControlContainer _this, Service service)
        {
            try
            {
                for (int i = 0; i < _this.Controls.Count; i++)
                {
                    if (_this.Controls[i] is EbUserControl)
                    {
                        EbUserControl _temp = service.Redis.Get<EbUserControl>(_this.Controls[i].RefId);
                        if (_temp == null)
                        {
                            var result = service.Gateway.Send<EbObjectParticularVersionResponse>(new EbObjectParticularVersionRequest { RefId = _this.Controls[i].RefId });
                            _temp = EbSerializers.Json_Deserialize(result.Data[0].Json);
                            service.Redis.Set<EbUserControl>(_this.Controls[i].RefId, _temp);
                        }
                        //_temp.RefId = _this.Controls[i].RefId;
                        (_this.Controls[i] as EbUserControl).Controls = _temp.Controls;
                        foreach (EbControl Control in (_this.Controls[i] as EbUserControl).Controls)
                        {
                            Control.ChildOf = "EbUserControl";
                            Control.Name = _this.Controls[i].Name + "_" + Control.Name;
                        }
                        //_this.Controls[i] = _temp;
                        (_this.Controls[i] as EbUserControl).AfterRedisGet(service);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("EXCEPTION : EbFormAfterRedisGet(service) " + e.Message);
            }
        }
    }
}
