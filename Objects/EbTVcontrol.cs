using ExpressBase.Common;
using ExpressBase.Common.Extensions;
using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using ExpressBase.Common.Structures;
using ExpressBase.Objects.Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Common;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using ExpressBase.Security;
using System.Globalization;
using ExpressBase.Common.LocationNSolution;
using ExpressBase.Common.Constants;
using ExpressBase.Objects.Objects.DVRelated;
using ExpressBase.Common.Data;
using ServiceStack;
using ServiceStack.Redis;
using ExpressBase.Objects.ServiceStack_Artifacts;

namespace ExpressBase.Objects
{
    [EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
    public class EbTVcontrol : EbControlUI
    {
        public EbTVcontrol()
        {
            //EbTableVisualization = new EbTableVisualization();
        }

        [OnDeserialized]
        public void OnDeserializedMethod(StreamingContext context)
        {
            this.BareControlHtml = this.GetBareHtml();
            this.ObjType = this.GetType().Name.Substring(2, this.GetType().Name.Length - 2);
        }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
        [HideInPropertyGrid]
        public string TableVisualizationJson{ get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
        [PropertyGroup(PGConstants.CORE)]
        [PropertyEditor(PropertyEditorType.ObjectSelector)]
        [OSE_ObjectTypes(EbObjectTypes.iTableVisualization)]
        [Alias("Table Visualization")]
        public string TVRefId { get; set; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
        [DefaultPropValue("400")]
        [PropertyGroup(PGConstants.APPEARANCE)]
        public override int Height { get; set; }

        [HideInPropertyGrid]
        [JsonIgnore]
        public override string ToolIconHtml { get { return "<i class='fa fa-bar-chart'></i>"; } set { } }

        [HideInPropertyGrid]
        [JsonIgnore]
        public override string ToolNameAlias { get { return "Datatable"; } set { } }


        public void InitFromDataBase(JsonServiceClient ServiceClient)
        {
            EbObjectParticularVersionResponse result= ServiceClient.Get<EbObjectParticularVersionResponse>(new EbObjectParticularVersionRequest { RefId = TVRefId });
            this.TableVisualizationJson = result.Data[0].Json;
        }

        public override string GetDesignHtml()
        {
            return GetHtml().RemoveCR().DoubleQuoted();
        }

        public override string GetHtml()
        {
            string EbCtrlHTML = HtmlConstants.CONTROL_WRAPER_HTML4WEB
                 .Replace("@LabelForeColor ", "color:" + (LabelForeColor ?? "@LabelForeColor ") + ";")
                 .Replace("@LabelBackColor ", "background-color:" + (LabelBackColor ?? "@LabelBackColor ") + ";");

            return ReplacePropsInHTML(EbCtrlHTML);
        }

        public override string GetHtml4Bot()
        {
            return ReplacePropsInHTML(HtmlConstants.CONTROL_WRAPER_HTML4BOT);
        }

        public override string GetBareHtml()
        {
            string html = @"
<div id='cont_@ebsid@' class='tv-control-cont'>
    <div id='content_@ebsid@' class='wrapper-cont'>
        <table id='@ebsid@' class=table display table-bordered compact'></table>
    </div>
</div>"
    .Replace("@ebsid@", this.EbSid_CtxId);

            return html;
        }

        //public EbTVcontrol()
        //{
        //    this.RowGroupCollection = new List<RowGroupParent>();
        //    this.NotVisibleColumns = new List<DVBaseColumn>();
        //    this.CurrentRowGroup = new RowGroupParent();
        //    this.OrderBy = new List<DVBaseColumn>();
        //    this.ColumnsCollection = new List<DVColumnCollection>();
        //    this.ParamsList = new List<Param>();
        //    this.FormLinks = new List<FormLink>();
        //}

        //public string BotCols { get; set; }

        //public string BotData { get; set; }

        //[OnChangeExec(@"
        //if(this.RowGroupCollection.$values.length > 0){
        //    pg.HideProperty('LeftFixedColumn')
        //    pg.HideProperty('RightFixedColumn')
        //}

        //else{
        //    pg.ShowProperty('LeftFixedColumn')
        //    pg.ShowProperty('RightFixedColumn')
        //}")]
        //[EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
        //[PropertyEditor(PropertyEditorType.Collection)]
        //[HideForUser]
        //[HideInPropertyGrid]
        //public List<RowGroupParent> RowGroupCollection { get; set; }

        //[EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
        //[HideInPropertyGrid]
        //public RowGroupParent CurrentRowGroup { get; set; }

        //[PropertyGroup("Fixed Column")]
        //[EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
        //public int LeftFixedColumn { get; set; }

        //[PropertyGroup("Fixed Column")]
        //[EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
        //public int RightFixedColumn { get; set; }

        //[EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
        //[DefaultPropValue("100")]
        //[PropertyGroup("Paging")]
        //public int PageLength { get; set; }

        //[EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
        //[HideInPropertyGrid]
        //public bool DisableRowGrouping { get; set; }

        //[EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
        //[HideInPropertyGrid]
        //public string SecondaryTableMapField { get; set; }

        //[EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
        //[HideForUser]
        //[PropertyGroup(PGConstants.APPEARANCE)]
        //public bool DisableCopy { get; set; }

        //[EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
        //[HideForUser]
        //[PropertyGroup(PGConstants.APPEARANCE)]
        //public bool AllowMultilineHeader { get; set; }

        //[EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
        //[PropertyEditor(PropertyEditorType.CollectionFrmSrc, "Columns")]
        //[PropertyGroup("User Actions")]
        //[PropertyPriority(1)]
        //[HideInPropertyGrid]
        //public List<DVBaseColumn> OrderBy { get; set; }

        //[EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
        //[HideInPropertyGrid]
        //public List<FormLink> FormLinks { get; set; }

        //[EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
        //[DefaultPropValue("15")]
        //[HideForUser]
        //[PropertyGroup(PGConstants.APPEARANCE)]
        //public int RowHeight { get; set; }

        //[EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
        //[HideForUser]
        //[HideInPropertyGrid]
        //public bool AllowLocalSearch { get; set; }

        //[JsonIgnore]
        //public EbWebForm WebForm { get; set; }

        //public static EbOperations Operations = TVOperations.Instance;

        ////========================DV props +=======================

        //[EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
        //[HideInPropertyGrid]
        //public List<DVBaseColumn> NotVisibleColumns { get; set; }

        //[EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
        //[HideInPropertyGrid]
        //public List<DVColumnCollection> ColumnsCollection { get; set; }

        //[EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
        //[HideInPropertyGrid]
        //public List<Param> ParamsList { get; set; }

        //[EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
        //[PropertyEditor(PropertyEditorType.ObjectSelector)]
        //[OSE_ObjectTypes(EbObjectTypes.iDataReader)]
        //[HideForUser]
        //[PropertyPriority(0)]
        //[Alias("Data Reader")]
        //[PropertyGroup(PGConstants.CORE)]
        //public string DataSourceRefId { get; set; }

        //[JsonIgnore]
        //public EbDataReader EbDataSource { get; set; }

        //[EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
        //[PropertyEditor(PropertyEditorType.CollectionABCpropToggle, "Columns", "bVisible", "_Formula")]
        //[CEOnSelectFn(@";
        //    this.bVisible = true;
        //    NonVC = Parent.NotVisibleColumns.$values;
        //    let  idx = NonVC.findIndex(x => x.name === this.name);
        //    if(idx > -1)
        //        NonVC.splice(idx, 1);")]
        //[CEOnDeselectFn(@"
        //    this.bVisible = false;
        //    Parent.NotVisibleColumns.$values.push(this)")]
        //[PropertyGroup("User Actions")]
        //[PropertyPriority(2)]
        //[HideInPropertyGrid]
        //public virtual DVColumnCollection Columns { get; set; }

        //[EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
        //[HideInPropertyGrid]
        //public bool AutoGen { get; set; }

        ////===============================================================================

        //public override List<string> DiscoverRelatedRefids()
        //{
        //    List<string> refids = new List<string>();
        //    if (!DataSourceRefId.IsEmpty())
        //    {
        //        EbDataReader ds = EbDataSource;
        //        if (ds is null)
        //        {
        //            refids.Add(DataSourceRefId);
        //        }
        //    }
        //    foreach (DVBaseColumn _col in Columns)
        //    {
        //        if (!_col.LinkRefId.IsNullOrEmpty())
        //            refids.Add(_col.LinkRefId);
        //        if (!_col.GroupFormLink.IsNullOrEmpty())
        //            refids.Add(_col.GroupFormLink);
        //        if (!_col.ItemFormLink.IsNullOrEmpty())
        //            refids.Add(_col.ItemFormLink);
        //    }
        //    return refids;
        //}

        //public override void ReplaceRefid(Dictionary<string, string> RefidMap)
        //{
        //    if (!DataSourceRefId.IsEmpty())
        //    {
        //        string dsid = DataSourceRefId;
        //        if (RefidMap.ContainsKey(dsid))
        //            DataSourceRefId = RefidMap[dsid];
        //        else
        //            DataSourceRefId = "failed-to-update-";
        //    }
        //    foreach (DVBaseColumn _col in Columns)
        //    {
        //        if (!_col.LinkRefId.IsNullOrEmpty())
        //        {
        //            if (RefidMap.ContainsKey(_col.LinkRefId))
        //                _col.LinkRefId = RefidMap[_col.LinkRefId];
        //            else if (RefidMap.ContainsKey(_col.GroupFormLink))
        //                _col.GroupFormLink = RefidMap[_col.GroupFormLink];
        //            else if (RefidMap.ContainsKey(_col.ItemFormLink))
        //                _col.ItemFormLink = RefidMap[_col.ItemFormLink];
        //            else
        //                _col.LinkRefId = "failed-to-update-";
        //        }
        //    }
        //}

        //public void AfterRedisGetBasicInfo(IServiceClient client, IRedisClient Redis)
        //{
        //    this.FormLinks = new List<FormLink>();
        //    foreach (DVBaseColumn col in this.Columns)
        //    {
        //        if (col.Check4FormLink())
        //        {
        //            try
        //            {
        //                this.WebForm = Redis.Get<EbWebForm>(col.LinkRefId);
        //                if (this.WebForm == null)
        //                {
        //                    var result = client.Get<EbObjectParticularVersionResponse>(new EbObjectParticularVersionRequest { RefId = col.LinkRefId });
        //                    this.WebForm = EbSerializers.Json_Deserialize(result.Data[0].Json);
        //                    Redis.Set<EbWebForm>(col.LinkRefId, this.WebForm);
        //                }
        //                this.FormLinks.Add(new FormLink { DisplayName = this.WebForm.DisplayName, Refid = col.LinkRefId, Params = col.FormParameters });
        //            }
        //            catch (Exception e)
        //            {
        //                Console.WriteLine("AfterRedisGetBasicInfo " + e.Message);
        //            }
        //        }
        //    }
        //    DVBaseColumn Col = this.Columns.Get("eb_action");
        //    if (Col != null)
        //    {
        //        foreach (DVColumnCollection _colcoll in this.ColumnsCollection)
        //        {
        //            DVBaseColumn __col = _colcoll.Pop(Col.Name, EbDbTypes.String, false);
        //        }
        //        if (Col is DVStringColumn && this.AutoGen)
        //        {
        //            DVBaseColumn actcol = new DVActionColumn
        //            {
        //                Data = Col.Data,
        //                Name = Col.Name,
        //                sTitle = Col.sTitle,
        //                Type = EbDbTypes.String,
        //                bVisible = true,
        //                sWidth = "100px",
        //                ClassName = Col.ClassName,
        //                LinkRefId = Col.LinkRefId,
        //                LinkType = Col.LinkType,
        //                FormMode = Col.FormMode,
        //                FormId = Col.FormId,
        //                Align = Align.Center,
        //                IsCustomColumn = true
        //            };
        //            this.Columns.Remove(Col);
        //            this.Columns.Add(actcol);
        //        }
        //    }
        //}

        //public override void BeforeSave(IServiceClient serviceClient, IRedisClient redis)
        //{
        //    this.AfterRedisGetBasicInfo(serviceClient, redis);
        //}

        //public void BeforeSave(Service service, IRedisClient redis)
        //{
        //    this.AfterRedisGetBasicInfoByService(service, redis);
        //}

        //public void AfterRedisGetBasicInfoByService(Service service, IRedisClient Redis)
        //{
        //    this.FormLinks = new List<FormLink>();
        //    foreach (DVBaseColumn col in this.Columns)
        //    {
        //        if (col.Check4FormLink())
        //        {
        //            try
        //            {
        //                this.WebForm = Redis.Get<EbWebForm>(col.LinkRefId);
        //                if (this.WebForm == null)
        //                {
        //                    var result = service.Gateway.Send<EbObjectParticularVersionResponse>(new EbObjectParticularVersionRequest { RefId = col.LinkRefId });
        //                    this.WebForm = EbSerializers.Json_Deserialize(result.Data[0].Json);
        //                    Redis.Set<EbWebForm>(col.LinkRefId, this.WebForm);
        //                }
        //                this.FormLinks.Add(new FormLink { DisplayName = this.WebForm.DisplayName, Refid = col.LinkRefId, Params = col.FormParameters });
        //            }
        //            catch (Exception e)
        //            {
        //                Console.WriteLine("AfterRedisGetBasicInfo " + e.Message);
        //            }
        //        }
        //    }
        //}
    }
}