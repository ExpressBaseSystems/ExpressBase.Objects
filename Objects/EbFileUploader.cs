using ExpressBase.Common;
using ExpressBase.Common.Extensions;
using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using ExpressBase.Common.Structures;
using Newtonsoft.Json;
using ServiceStack;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Runtime.Serialization;

namespace ExpressBase.Objects
{
    [EnableInBuilder(BuilderType.WebForm, BuilderType.UserControl)]
    public class EbFileUploader : EbControlUI
    {

        [HideInPropertyGrid]
        public override bool DoNotPersist { get; set; }

        [HideInPropertyGrid]
        public override EbScript ValueExpr { get; set; }

        public override bool IgnoreDataConsistencyCheck { get; set; }

        public override bool SelfTrigger { get; set; }

        [HideInPropertyGrid]
        public override EbScript DefaultValueExpression { get; set; }

        [HideInPropertyGrid]
        public override List<EbValidator> Validators { get; set; }

        [HideInPropertyGrid]
        public override EbScript OnChangeFn { get; set; }

        [HideInPropertyGrid]
        public override bool Unique { get; set; }

        [JsonIgnore]
        public override string IsRequiredOKJSfn { get { return "return true"; } set { } }

        public EbFileUploader()
        {
            this.Categories = new List<EbFupCategories>();
        }

        //[HideInPropertyGrid]
        //[EnableInBuilder(BuilderType.BotForm)]
        //public override bool IsReadOnly { get => this.IsDisable; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
        [PropertyGroup("General")]
        public FileClass FileType { set; get; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
        [PropertyGroup("General")]
        [PropertyEditor(PropertyEditorType.Collection)]
        public List<EbFupCategories> Categories { set; get; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
        [PropertyGroup("General")]
        public bool IsMultipleUpload { set; get; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
        [PropertyGroup("General")]
        public bool HideEmptyCategory { set; get; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
        [PropertyGroup("General")]
        public bool ShowUploadDate { set; get; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
        [PropertyGroup("General")]
        public bool ShowFileName { set; get; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
        [PropertyGroup("General")]
        public bool EnableTag { set; get; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
        [PropertyGroup("General")]
        public bool EnableCrop { set; get; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
        [PropertyGroup("General")]
        public bool DisableUpload { set; get; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
        [DefaultPropValue("false")]
        public bool ViewByCategory { set; get; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
        [PropertyGroup("General")]
        [Alias("Max file size in MB")]
        public int MaxFileSize { set; get; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
        [PropertyGroup("General")]
        [Alias("Max file size in KB")]
        public int MaxSizeInKB { set; get; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
        [PropertyGroup("Crop Properties")]
        public bool ResizeViewPort { set; get; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
        [PropertyGroup("Appearance")]
        public ViewerPosition ViewerPosition { set; get; }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
        [PropertyEditor(PropertyEditorType.ScriptEditorCS)]
        public EbScript ContextGetExpr { set; get; }


        [EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
        [PropertyEditor(PropertyEditorType.ScriptEditorCS)]
        public EbScript ContextSetExpr { set; get; }

        [OnDeserialized]
        public void OnDeserializedMethod(StreamingContext context)
        {
            this.BareControlHtml = this.GetBareHtml();
            this.BareControlHtml4Bot = this.BareControlHtml;
            this.ObjType = this.GetType().Name.Substring(2, this.GetType().Name.Length - 2);
        }

        public override string DesignHtml4Bot
        {
            get => @"<div class='input-group'style='width: 100 %; '> 
                        <input id='' ui-inp='' data-ebtype='6' class='date' type='text' name=' tabindex='0' style='width: 100%; display: inline - block; background - color: rgb(255, 255, 255); color: rgb(51, 51, 51);' placeholder=''>
                        <span class='input-group-addon'>
							<i class='fa fa fa-upload' aria-hidden='true'  style='padding: 6px 12px;'></i>  
                        </span>
                    </div>";
            set => base.DesignHtml4Bot = value;
        }

        public override string GetHead()
        {
            return string.Empty;
        }

        public override string GetDesignHtml()
        {
            string EbCtrlHTML = HtmlConstants.CONTROL_WRAPER_HTML4WEB
                    .Replace("@barehtml@", @"                       
                        <div class='input-group'style='width: 100 %; '> 
                                 <input id='' ui-inp='' data-ebtype='6' class='date' type='text' name=' tabindex='0' style='width: 100%; display: inline - block; background - color: rgb(255, 255, 255); color: rgb(51, 51, 51);' placeholder=''>
                                 <span class='input-group-addon'>
                                <i class='fa fa fa-upload' aria-hidden='true'  style='padding: 6px 12px;'></i>  
                            </span>
                        </div>
                ").RemoveCR().DoubleQuoted();
            //return this.GetHtml().RemoveCR().GraveAccentQuoted();
            return ReplacePropsInHTML(EbCtrlHTML);
        }

        [HideInPropertyGrid]
        [JsonIgnore]
        public override string ToolIconHtml { get { return "<i class='fa fa-upload'></i>"; } set { } }

        //public override string GetToolHtml()
        //{
        //    return @"<div eb-type='@toolName' class='tool'><i class='fa fa-upload'></i>  @toolName</div>".Replace("@toolName", this.GetType().Name.Substring(2));
        //}

        public override string GetBareHtml()
        {
            return @" 
        <div id='@ebsid@' style='width:100%;'></div>"
            .Replace("@ebsid@", this.EbSid);
        }

        public override string GetHtml()
        {
            //            string EbCtrlHTML = @"
            //    <div id='cont_@name@' Ctype='FileUploader' class='Eb-ctrlContainer' eb-hidden='@isHidden@'>
            //        <div class='eb-ctrl-label' id='@name@Lbl' style='@LabelBackColor  @LabelForeColor '> @Label </div>
            //       @barehtml@
            //        <span class='helpText'> @HelpText </span>
            //    </div>
            //"
            //.Replace("@barehtml@", this.GetBareHtml())
            //.Replace("@name@", this.Name)
            //.Replace("@isHidden@", this.Hidden.ToString())

            //    .Replace("@LabelForeColor ", "color:" + ((this.LabelForeColor != null) ? this.LabelForeColor : "@LabelForeColor ") + ";")
            //    .Replace("@LabelBackColor ", "background-color:" + ((this.LabelBackColor != null) ? this.LabelBackColor : "@LabelBackColor ") + ";")
            //    .Replace("@HelpText ", ((this.HelpText != null) ? this.HelpText : "@HelpText "))
            //    .Replace("@Label ", ((this.Label != null) ? this.Label : "@Label "));
            //            return EbCtrlHTML;

            string EbCtrlHTML = HtmlConstants.CONTROL_WRAPER_HTML4WEB
               .Replace("@LabelForeColor ", "color:" + (LabelForeColor ?? "@LabelForeColor ") + ";")
               .Replace("@LabelBackColor ", "background-color:" + (LabelBackColor ?? "@LabelBackColor ") + ";");

            return ReplacePropsInHTML(EbCtrlHTML);
        }

        //INCOMPLETE
        public string GetSelectQuery(IDatabase DataDB, bool pri_cxt_only = true)
        {
            string Qry;
            if (pri_cxt_only)
                Qry = DataDB.EB_GET_SELECT_FILE_UPLOADER_CXT.Replace("@Name@", this.Name ?? this.EbSid);
            else
                Qry = DataDB.EB_GET_SELECT_FILE_UPLOADER_CXT_SEC.Replace("@Name@", this.Name ?? this.EbSid);

            return Qry;
        }

        public static string GetUpdateQuery(IDatabase DataDB, List<DbParameter> param, Dictionary<string, SingleTable> Tables, string mastertbl, string EbObId, ref int i, int dataId)
        {
            List<string> InnerVals = new List<string>();
            List<string> Innercxt = new List<string>();
            List<string> InnerIds = new List<string>();
            string fullqry = string.Empty;
            foreach (KeyValuePair<string, SingleTable> entry in Tables)
            {
                foreach (SingleRow row in entry.Value)
                {
                    string cn = entry.Key + "_" + i.ToString();
                    i++;
                    if (dataId == 0)
                        InnerVals.Add(string.Format("( CONCAT('{0}_', TRIM(CAST(eb_currval('{1}_id_seq') AS CHAR(32))), '_{2}'))", EbObId, mastertbl, entry.Key));
                    else
                        InnerVals.Add(string.Format("('{0}_{1}_{2}')", EbObId, dataId, entry.Key));

                    param.Add(DataDB.GetNewParameter(cn, EbDbTypes.Decimal, row.Columns[0].Value));
                    InnerIds.Add("@" + cn);
                }
                if (dataId == 0)
                    Innercxt.Add(string.Format("context = CONCAT('{0}_', TRIM(CAST(eb_currval('{1}_id_seq') AS CHAR(32))), '_{2}')", EbObId, mastertbl, entry.Key));
                else
                    Innercxt.Add(string.Format("context = '{0}_{1}_{2}'", EbObId, dataId, entry.Key));
            }

            if (InnerVals.Count > 0)
            {

                for (int k = 0; k < InnerVals.Count; k++)
                {
                    fullqry += string.Format(@"UPDATE 
                                            eb_files_ref AS t
                                        SET
                                            context = {0}                                        
                                        WHERE
                                           t.id = {1} AND t.eb_del = 'F';", InnerVals[k], InnerIds[k]);
                }

                fullqry += string.Format(@"UPDATE eb_files_ref SET eb_del='T' 
                                            WHERE ({0}) AND eb_del='F' AND id NOT IN ({1});", Innercxt.Join(" OR "), InnerIds.Join(","));
            }
            else if (Innercxt.Count > 0)// if all files deleted
            {
                fullqry += string.Format(@"UPDATE eb_files_ref SET eb_del='T' 
                                            WHERE ({0}) AND eb_del='F';", Innercxt.Join(" OR "));
            }
            return fullqry;
        }

        public string GetUpdateQuery2(IDatabase DataDB, List<DbParameter> param, Dictionary<string, SingleTable> ExtTables, string mastertbl, string EbObId, ref int i, int dataId, string secCxtGet, string secCxtSet)
        {
            string pCxtVal;// primary context value
            string sCxtGetVal = string.Empty;// secondary context get value
            string sCxtSetVal = string.Empty;// secondary context set value
            List<string> refIds = null;
            List<string> refIdsAdd = null;
            List<string> refIdsDel = null;
            string fullqry = string.Empty;
            bool isAddDelFlow = false;

            if (ExtTables.ContainsKey(this.Name + "_add") && ExtTables.ContainsKey(this.Name + "_del"))
            {
                refIdsAdd = GetRefidsList(DataDB, param, ExtTables[this.Name + "_add"], ref i);
                refIdsDel = GetRefidsList(DataDB, param, ExtTables[this.Name + "_del"], ref i);
                isAddDelFlow = true;
            }
            else if (ExtTables.ContainsKey(this.Name))//old flow
            {
                refIds = GetRefidsList(DataDB, param, ExtTables[this.Name], ref i);
            }

            if (!secCxtGet.IsNullOrEmpty())
            {
                sCxtGetVal = "contextget_" + i;
                i++;
                param.Add(DataDB.GetNewParameter(sCxtGetVal, EbDbTypes.String, secCxtGet));
            }
            if (!secCxtSet.IsNullOrEmpty())
            {
                sCxtSetVal = "contextset_" + i;
                i++;
                param.Add(DataDB.GetNewParameter(sCxtSetVal, EbDbTypes.String, secCxtSet));
            }

            if (dataId == 0)
                pCxtVal = string.Format("CONCAT('{0}_', TRIM(CAST(eb_currval('{1}_id_seq') AS CHAR(32))), '_{2}')", EbObId, mastertbl, this.Name);
            else
                pCxtVal = string.Format("'{0}_{1}_{2}'", EbObId, dataId, this.Name);

            if (isAddDelFlow)
            {
                if (refIdsAdd.Count > 0)
                {
                    fullqry += string.Format(@" UPDATE eb_files_ref SET context = {0} @upCxt@ , lastmodifiedby = @eb_createdby, lastmodifiedat = {3} 
                                WHERE id = ANY(ARRAY[{1}]) AND eb_del <> 'T' AND context = 'default' @secCxt@;"
                                    .Replace("@secCxt@", !secCxtGet.IsNullOrEmpty() ? "AND context_sec IS NULL" : "")
                                    .Replace("@upCxt@", !secCxtSet.IsNullOrEmpty() ? ", context_sec = @{2}" : ""), pCxtVal, refIdsAdd.Join(","), sCxtSetVal, DataDB.EB_CURRENT_TIMESTAMP);
                }
                if (refIdsDel.Count > 0 && dataId > 0)
                {
                    fullqry += string.Format(@"UPDATE eb_files_ref SET eb_del='T', lastmodifiedby = @eb_createdby, lastmodifiedat = {3}
                                WHERE (context = {0} @secCxt@) AND eb_del='F' AND id IN ({1});"
                                    .Replace("@secCxt@", !secCxtGet.IsNullOrEmpty() ? "OR context_sec = @{2}" : ""), pCxtVal, refIdsDel.Join(","), sCxtGetVal, DataDB.EB_CURRENT_TIMESTAMP);
                }
            }
            else
            {
                if (refIds.Count > 0)
                {
                    if (dataId > 0) // edit mode
                    {
                        fullqry += string.Format(@"UPDATE eb_files_ref SET eb_del='T', lastmodifiedby = @eb_createdby, lastmodifiedat = {3}
                                            WHERE (context = {0} @secCxt@) AND eb_del='F' AND id NOT IN ({1});"
                                                .Replace("@secCxt@", !secCxtGet.IsNullOrEmpty() ? "OR context_sec = @{2}" : ""), pCxtVal, refIds.Join(","), sCxtGetVal, DataDB.EB_CURRENT_TIMESTAMP);
                    }
                    fullqry += string.Format(@" UPDATE eb_files_ref SET context = {0} @upCxt@ , lastmodifiedby = @eb_createdby, lastmodifiedat = {3}
                                                    WHERE id = ANY(ARRAY[{1}]) AND eb_del <> 'T' AND context = 'default' @secCxt@;"
                                                .Replace("@secCxt@", !secCxtGet.IsNullOrEmpty() ? "AND context_sec IS NULL" : "")
                                                .Replace("@upCxt@", !secCxtSet.IsNullOrEmpty() ? ", context_sec = @{2}" : ""), pCxtVal, refIds.Join(","), sCxtSetVal, DataDB.EB_CURRENT_TIMESTAMP);
                }
                else if (dataId > 0) // if all files deleted
                {
                    fullqry += string.Format(@"UPDATE eb_files_ref SET eb_del='T', lastmodifiedby = @eb_createdby, lastmodifiedat = {2}
                                            WHERE (context = {0} @secCxt@) AND eb_del='F';"
                                                .Replace("@secCxt@", !secCxtGet.IsNullOrEmpty() ? "OR context_sec = @{1}" : ""), pCxtVal, sCxtGetVal, DataDB.EB_CURRENT_TIMESTAMP);
                }
            }
            return fullqry;
        }

        private List<string> GetRefidsList(IDatabase DataDB, List<DbParameter> param, SingleTable Table, ref int i)
        {
            List<string> refIds = new List<string>();
            foreach (SingleRow row in Table)
            {
                string cn = this.Name + "_" + i.ToString();
                i++;
                param.Add(DataDB.GetNewParameter(cn, EbDbTypes.Decimal, row.Columns[0].Value));
                refIds.Add("@" + cn);
            }
            return refIds;
        }


        [JsonIgnore]
        public override string EnableJSfn { get { return @"this.__IsDisable = false; $('#cont_' + this.EbSid_CtxId + ' .FUP_Head_W').removeAttr('disabled').css('pointer-events', 'inherit');"; } set { } }

        [JsonIgnore]
        public override string DisableJSfn { get { return @"this.__IsDisable = true; $('#cont_' + this.EbSid_CtxId + ' .FUP_Head_W').attr('disabled', 'disabled').css('pointer-events', 'none');"; } set { } }

    }

    [EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
    [HideInToolBox]
    public class EbFupCategories : EbControl
    {
        public EbFupCategories()
        {

        }

        [EnableInBuilder(BuilderType.WebForm, BuilderType.BotForm, BuilderType.UserControl)]
        [PropertyGroup("General")]
        public string CategoryTitle { set; get; }
    }
}
