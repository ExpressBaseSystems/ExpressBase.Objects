using ExpressBase.Common;
using ExpressBase.Common.Extensions;
using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using ExpressBase.Common.Structures;
using ExpressBase.Objects.ServiceStack_Artifacts;
using ServiceStack;
using ServiceStack.Redis;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace ExpressBase.Objects
{
    public static class EbFormHelper
    {
        public static List<string> DiscoverRelatedRefids(EbControlContainer _this)
        {
            List<string> refids = new List<string>();
            EbControl[] _flatCtrls = new List<EbControl>() { _this }.FlattenAllEbControls();
            for (int i = 0; i < _flatCtrls.Length; i++)
            {
                if (_flatCtrls[i] is EbUserControl || _flatCtrls[i] is EbDGUserControlColumn)
                {
                    if (!string.IsNullOrEmpty(_flatCtrls[i].RefId))
                        refids.Add(_flatCtrls[i].RefId);
                }
                else
                {
                    PropertyInfo[] _props = _flatCtrls[i].GetType().GetProperties();
                    foreach (PropertyInfo _prop in _props)
                    {
                        if (_prop.IsDefined(typeof(OSE_ObjectTypes)))
                        {
                            object _val = _prop.GetValue(_flatCtrls[i], null);
                            if (_val != null)
                            {
                                if (_prop.PropertyType == typeof(string))
                                {
                                    if (!_val.ToString().IsEmpty())
                                        refids.Add(_val.ToString());
                                }
                                else if (_prop.PropertyType == typeof(List<ObjectBasicInfo>))
                                {
                                    foreach (ObjectBasicInfo info in _val as List<ObjectBasicInfo>)
                                    {
                                        if (!string.IsNullOrEmpty(info.ObjRefId))
                                            refids.Add(info.ObjRefId);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return refids;
        }

        public static void ReplaceRefid(EbControlContainer _this, Dictionary<string, string> RefidMap)
        {
            EbControl[] _flatCtrls = new List<EbControl>() { _this }.FlattenAllEbControls();
            for (int i = 0; i < _flatCtrls.Length; i++)
            {
                if (_flatCtrls[i] is EbUserControl || _flatCtrls[i] is EbDGUserControlColumn)
                {
                    if (!string.IsNullOrEmpty(_flatCtrls[i].RefId))
                    {
                        if (RefidMap.ContainsKey(_flatCtrls[i].RefId))
                            _flatCtrls[i].RefId = RefidMap[_flatCtrls[i].RefId];
                        else
                            _flatCtrls[i].RefId = "failed-to-update-";
                    }
                }
                else
                {
                    PropertyInfo[] _props = _flatCtrls[i].GetType().GetProperties();
                    foreach (PropertyInfo _prop in _props)
                    {
                        if (_prop.IsDefined(typeof(OSE_ObjectTypes)))
                        {
                            object _val = _prop.GetValue(_flatCtrls[i], null);
                            if (_val != null)
                            {
                                if (_prop.PropertyType == typeof(string))
                                {
                                    string st_val = _val.ToString();
                                    if (!st_val.IsEmpty())
                                    {
                                        if (RefidMap.ContainsKey(st_val))
                                            _prop.SetValue(_flatCtrls[i], RefidMap[st_val], null);
                                        else
                                            _prop.SetValue(_flatCtrls[i], "failed-to-update-");
                                    }
                                }
                                else if (_prop.PropertyType == typeof(List<ObjectBasicInfo>))
                                {
                                    foreach (ObjectBasicInfo info in _val as List<ObjectBasicInfo>)
                                    {
                                        if (!string.IsNullOrEmpty(info.ObjRefId))
                                        {
                                            if (RefidMap.ContainsKey(info.ObjRefId))
                                                info.ObjRefId = info.ObjRefId;
                                            else
                                                info.ObjRefId = "failed-to-update-";
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        //Rendering side -> service = null
        public static void AfterRedisGet(EbControlContainer _this, IRedisClient Redis, IServiceClient client, Service service)
        {
            try
            {
                for (int i = 0; i < _this.Controls.Count; i++)
                {
                    EbControl c = _this.Controls[i];
                    c.IsRenderMode = _this.IsRenderMode;
                    c.IsDynamicTabChild = _this.IsDynamicTabChild;
                    if (c is EbUserControl || c is EbDGUserControlColumn)
                    {
                        EbUserControl _temp = GetEbObject<EbUserControl>(c.RefId, client, Redis, service);
                        if (c is EbDGUserControlColumn)
                        {
                            foreach (EbControl Control in _temp.Controls)
                            {
                                RenameControlsRec(Control, c.Name);
                            }
                            (c as EbDGUserControlColumn).InitUserControl(_temp);
                        }
                        else
                        {
                            (c as EbUserControl).Controls = _temp.Controls;
                            (c as EbUserControl).DisplayName = _temp.DisplayName;
                            (c as EbUserControl).VersionNumber = _temp.VersionNumber;
                            foreach (EbControl Control in (c as EbUserControl).Controls)
                            {
                                RenameControlsRec(Control, c.Name);
                            }
                            c.AfterRedisGet(Redis as RedisClient, client);
                        }
                    }
                    else if (c is EbControlContainer)
                    {
                        if (c is EbTabPane && (c as EbTabPane).IsDynamic)
                            c.IsDynamicTabChild = true;
                        AfterRedisGet(c as EbControlContainer, Redis, client, null);
                    }
                    else if (c is EbProvisionLocation)//add unmapped ctrls as DoNotPersist controls
                    {
                        if (_this.IsRenderMode)
                        {
                            EbProvisionLocation prvnCtrl = c as EbProvisionLocation;
                            for (int j = 0; j < prvnCtrl.Fields.Count; j++)
                            {
                                UsrLocField prvnFld = prvnCtrl.Fields[j] as UsrLocField;
                                if (prvnFld.ControlName.IsNullOrEmpty() && prvnFld.IsRequired)
                                {
                                    if (prvnFld.Type == "image")
                                    {
                                        _this.Controls.Insert(i, new EbDisplayPicture()
                                        {
                                            Name = "namecustom" + i,
                                            EbSid = "ebsidcustom" + i,
                                            EbSid_CtxId = "ebsidcustom" + i,
                                            Label = prvnFld.DisplayName,
                                            DoNotPersist = true,
                                            MaxHeight = 100
                                        });
                                    }
                                    else
                                    {
                                        _this.Controls.Insert(i, new EbTextBox()
                                        {
                                            Name = "namecustom" + i,
                                            EbSid = "ebsidcustom" + i,
                                            EbSid_CtxId = "ebsidcustom" + i,
                                            Label = prvnFld.DisplayName,
                                            DoNotPersist = true
                                        });
                                    }
                                    prvnFld.ControlName = "namecustom" + i;
                                    i++;
                                }
                            }
                        }
                    }
                    else if (c is EbProvisionUser)
                    {
                        if (_this.IsRenderMode)
                        {
                            EbProvisionUser prvnCtrl = c as EbProvisionUser;
                            for (int j = 0; j < prvnCtrl.Fields.Count; j++)
                            {
                                UsrLocField prvnFld = prvnCtrl.Fields[j] as UsrLocField;
                                if (prvnFld.ControlName.IsNullOrEmpty() && prvnFld.IsRequired)
                                {
                                    _this.Controls.Insert(i, new EbTextBox()
                                    {
                                        Name = "namecustom" + i,
                                        EbSid = "ebsidcustom" + i,
                                        EbSid_CtxId = "ebsidcustom" + i,
                                        Label = prvnFld.DisplayName,
                                        DoNotPersist = true
                                    });
                                    prvnFld.ControlName = "namecustom" + i;
                                    i++;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("EXCEPTION : FormAfterRedisGet " + e.Message);
            }
        }

        public static void RenameControlsRec(EbControl _control, string _ucName)
        {
            if (_control is EbControlContainer)
            {
                if (!(_control is EbUserControl))
                {
                    foreach (EbControl _ctrl in (_control as EbControlContainer).Controls)
                    {
                        RenameControlsRec(_ctrl, _ucName);
                    }
                }
            }
            else
            {
                _control.ChildOf = "EbUserControl";
                _control.Name = _ucName + "_" + _control.Name;
                _control.EbSid = _ucName + "_" + _control.EbSid;
            }
        }

        public static void InitDataPushers(EbWebForm _this, IRedisClient Redis, IServiceClient client, Service service)
        {
            _this.FormCollection = new EbWebFormCollection(_this);
            if (_this.DataPushers != null)
            {
                foreach (EbDataPusher pusher in _this.DataPushers)
                {
                    EbWebForm _form = GetEbObject<EbWebForm>(pusher.FormRefId, client, Redis, service);
                    _form.RefId = pusher.FormRefId;
                    _form.UserObj = _this.UserObj;
                    _form.SolutionObj = _this.SolutionObj;
                    _form.AfterRedisGet(Redis as RedisClient, client);
                    _form.DataPusherConfig = new EbDataPusherConfig { SourceTable = _this.FormSchema.MasterTable, MultiPushId = _this.RefId + "_" + pusher.Name };
                    pusher.WebForm = _form;
                    _this.FormCollection.Add(_form);
                    _this.ExeDataPusher = true;
                }
            }
        }

        public static T GetEbObject<T>(string RefId, IServiceClient ServiceClient, IRedisClient Redis, Service Service)
        {
            if (string.IsNullOrEmpty(RefId))
                throw new Exception("RefId is null or empty. FormHelper >GetEbObject. Type: " + typeof(T).Name);

            T _ebObject = default;
            if (Redis != null) _ebObject = Redis.Get<T>(RefId);

            if (_ebObject == null)
            {
                EbObjectParticularVersionResponse resp;
                if (ServiceClient != null)
                    resp = ServiceClient.Get(new EbObjectParticularVersionRequest { RefId = RefId });
                else if (Service != null)
                    resp = Service.Gateway.Send<EbObjectParticularVersionResponse>(new EbObjectParticularVersionRequest { RefId = RefId });
                else
                    throw new Exception($"ServiceClient and Service is null. FormHelper >GetEbObject. RefId: {RefId}, Type: {typeof(T).Name}");

                if (resp.Data == null || resp.Data.Count == 0)
                    throw new Exception($"EbObject not found. FormHelper >GetEbObject. RefId: {RefId}, Type: {typeof(T).Name}");

                _ebObject = EbSerializers.Json_Deserialize(resp.Data[0].Json);
                if (Redis != null) Redis.Set<T>(RefId, _ebObject);
            }
            return _ebObject;
        }
    }

    public class EbColumnExtra
    {
        public static Dictionary<string, EbDbTypes> Params
        {
            get
            {
                return new Dictionary<string, EbDbTypes> {
                    { "eb_row_num",EbDbTypes.Decimal},
                    { "eb_created_at_device",EbDbTypes.DateTime},
                    { "eb_device_id",EbDbTypes.String},
                    { "eb_appversion",EbDbTypes.String}
                };
            }
        }
    }
}
