﻿using ExpressBase.Common;
using ExpressBase.Common.Constants;
using ExpressBase.Common.Data;
using ExpressBase.Common.Extensions;
using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using ExpressBase.Objects.ServiceStack_Artifacts;
using Newtonsoft.Json;
using ServiceStack;
using ServiceStack.Redis;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;

namespace ExpressBase.Objects
{
    [EnableInBuilder(BuilderType.UserControl, BuilderType.WebForm, BuilderType.FilterDialog)]
    [HideInToolBox]
    [BuilderTypeEnum(BuilderType.WebForm)]
    public class EbUserControl : EbForm
    {
        public EbUserControl() { }

        //public string RefId { get; set; }

        //[EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.BotForm, BuilderType.UserControl)]
        //[UIproperty]
        //[Unique]
        //[OnChangeUIFunction("Common.LABEL")]
        //[PropertyEditor(PropertyEditorType.MultiLanguageKeySelector)]
        //public string Label { get; set; }

        [HideInPropertyGrid]
        [JsonIgnore]
        public override string ToolIconHtml { get { return "<i class='fa fa-puzzle-piece'></i>"; } set { } }

        //public override string GetToolHtml()
        //{
        //    return @"<div eb-type='@toolName' class='tool'><i class='fa fa-puzzle-piece'></i>  @toolName</div>".Replace("@toolName", this.GetType().Name.Substring(2));
        //}
        
        [EnableInBuilder(BuilderType.WebForm, BuilderType.UserControl)]
        [HideInPropertyGrid]
        public override string TableName { get { return this.TableName_Temp; } set { this.TableName_Temp = value; } }

        [EnableInBuilder(BuilderType.WebForm)]
        [PropertyGroup(PGConstants.DATA)]
        [HelpText("Name Of database-table Which you want to store Data collected using this Form")]
        [InputMask("[a-z][a-z0-9]*(_[a-z0-9]+)*")]
        [Alias("TableName")]
        public string TableName_Temp { get; set; }

        //[EnableInBuilder(BuilderType.WebForm, BuilderType.FilterDialog, BuilderType.UserControl)]
        //[HideInPropertyGrid]
        //public string BareHtml { get; set; }

        //public override void BeforeSave()
        //{
        //    this.BareHtml = this.GetBareHtml();

        //}

        public override string GetBareHtml()
        {
            string html = string.Empty;

            foreach (EbControl c in this.Controls)
            {
                html += c.GetHtml();
            }

            return html;
        }

        //[OnDeserialized]
        //public void OnDeserializedMethod(StreamingContext context)
        //{
        //    string html = string.Empty;

        //    foreach (EbControl c in this.Controls)
        //    {
        //        string Html = c.GetHtml();
        //        html += Html;
        //        if (!ChildDBareHtmlColl.ContainsKey(c.EbSid_CtxId))
        //            ChildDBareHtmlColl.Add(c.EbSid_CtxId, html);
        //        temps += (c.EbSid_CtxId + " ,");
        //    }

        //}

        public override string GetDesignHtml()
        {
            string html = @"
        <div id='cont_@ebsid@' ebsid='@ebsid@' name='@name@' class='Eb-ctrlContainer' ctype='@type@' eb-hidden='@isHidden@'>
            <span class='eb-ctrl-label' ui-label id='@ebsidLbl'>____ UserControl ____</span>
                
        </div>";
            return html.RemoveCR().DoubleQuoted();
        }

        //public string GetInnerHtml()
        //{
        //    string ctrlhtml = string.Empty;
        //    foreach (EbControl c in this.Controls)
        //    {
        //        ctrlhtml += c.GetHtml();
        //    }
        //    string coverhtml = @"            
        //        <div  id='@ebsid@Wraper' class=''>
        //            <span class='eb-ctrl-label' ui-label id='@ebsid@Lbl' style='font-style: italic; font-weight: normal;'>@Label@</span>
        //            @barehtml@
        //        </div>
        //        <span class='helpText' ui-helptxt > @helpText@ </span>"
        //        .Replace("@barehtml@", ctrlhtml)
        //        .Replace("@name@", this.Name)
        //        .Replace("@type@", this.ObjType)
        //        .Replace("@ebsid@", this.EbSid)
        //        .Replace("@rmode@", IsRenderMode ? "true" : "false")
        //        .Replace("@id@", "cont_" + this.EbSid)
        //        .Replace("@Label@", string.Concat(this.DisplayName, " - ", this.VersionNumber));

        //    return ReplacePropsInHTML(coverhtml);
        //}

        public override string GetHtml()
        {
            string ctrlhtml = string.Empty;
            foreach (EbControl c in this.Controls)
            {
                ctrlhtml += c.GetHtml();
            }
            string coverhtml = @"
            <div id='@id@' ebsid='@ebsid@' ctype='@type@' isrendermode='@rmode@' class='Eb-ctrlContainer ebcont-ctrl user-ctrl' eb-type='UserControl' @TabIndex@>
                <div  id='@ebsid@Wraper' class=''>
                    @UcDispName@
                    @barehtml@
                </div>
                <span class='helpText' ui-helptxt > @helpText@ </span>
            </div>"
                .Replace("@UcDispName@", IsRenderMode? string.Empty: "<span class='eb-ctrl-label' ui-label id='@ebsid@Lbl' style='font-style: italic; font-weight: normal;'>@Label@</span>")
                .Replace("@TabIndex@", IsRenderMode? string.Empty: "tabindex='1'")
                .Replace("@barehtml@", ctrlhtml)
                .Replace("@name@", this.Name)
                .Replace("@type@", this.ObjType)
                .Replace("@ebsid@", this.EbSid)
                .Replace("@rmode@", IsRenderMode? "true": "false")
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
                <div id='@id@' ebsid='@ebsid@' eb-form='true' ctype='@type@' isrendermode='@rmode@' class='formB-box form-buider-form ebcont-ctrl' eb-type='UserControl' tabindex='1'>
                    @barehtml@                        
                    <span class='helpText' ui-helptxt >@helpText@ </span>
                </div>"
                    .Replace("@barehtml@", html)
                    .Replace("@name@", this.Name)
                    .Replace("@type@", this.ObjType)
                    .Replace("@ebsid@", this.EbSid)
                    .Replace("@rmode@", IsRenderMode ? "true" : "false")
                    .Replace("@id@", this.EbSid);
                return ReplacePropsInHTML(coverhtml);
            }
            else
                return GetHtml();
        }

        //builder side - now it is using to get design html, rec call from AfterRedisGet
        public void AfterRedisGet(Service service)
        {
            EbFormHelper.AfterRedisGet(this, service.Redis, null, service);
            foreach(EbControl c in this.Controls)
                EbFormHelper.RenameControlsRec(c, this.Name);
        }

        //rendering side
        public override void AfterRedisGet(RedisClient Redis, IServiceClient client)
        {
            EbFormHelper.AfterRedisGet(this, Redis, client, null);
        }

        public override List<string> DiscoverRelatedRefids()
        {
            return EbFormHelper.DiscoverRelatedRefids(this);
        }
        public override void ReplaceRefid(Dictionary<string, string> RefidMap)
        {
            EbFormHelper.ReplaceRefid(this, RefidMap);
        }

        public void SetDataObjectControl(IDatabase DataDB, Service Service , List<Param> param)
        {
            Dictionary<string, EbDataTable> _tables = new Dictionary<string, EbDataTable>();
            EbControl[] Allctrls = this.Controls.FlattenAllEbControls();
            for (int i = 0; i < Allctrls.Length; i++)
            {
                if(Allctrls[i] is EbDataObject)
                {
                    EbDataObject c = Allctrls[i] as EbDataObject;
                    //EbDataReader _temp = Service.Redis.Get<EbDataReader>(c.DataSource);
                    //if (_temp == null)
                    //{
                    //    var result = Service.Gateway.Send<EbObjectParticularVersionResponse>(new EbObjectParticularVersionRequest { RefId = c.DataSource });
                    //    _temp = EbSerializers.Json_Deserialize(result.Data[0].Json);
                    //    Service.Redis.Set<EbDataReader>(c.DataSource, _temp);
                    //}
                    DataSourceDataSetResponse response = Service.Gateway.Send<DataSourceDataSetResponse>(new DataSourceDataSetRequest { RefId = c.DataSource , Params = param });

                    //EbDataTable data = DataDB.DoQuery(_temp.Sql); 
                    
                    EbDataTable data = response.DataSet.Tables[0];
                    _tables.Add(c.Name, data);
                }
            }

            for (int i = 0; i < Allctrls.Length; i++)
            {
                if (Allctrls[i] is EbDataLabel)
                {
                    EbDataLabel c = Allctrls[i] as EbDataLabel;
                    c.DynamicLabel = _tables[c.DataObjCtrlName].Rows[0][c.DataObjColName].ToString();
                }
            }

        }

    }
}
