using ExpressBase.Common;
using ExpressBase.Common.Data;
using ExpressBase.Common.Extensions;
using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using ExpressBase.Common.Structures;
using System;
using System.Collections.Generic;

namespace ExpressBase.Objects
{
    [EnableInBuilder(BuilderType.MobilePage)]
    public class EbMobileForm : EbMobileContainer
    {
        [EnableInBuilder(BuilderType.MobilePage)]
        [HideInPropertyGrid]
        public override string Name { set; get; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [HideInPropertyGrid]
        public List<EbMobileControl> ChildControls { get; set; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyGroup("Core")]
        public string TableName { set; get; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [Alias("Auto Deploy Visualization")]
        [PropertyGroup("Core")]
        public bool AutoDeployMV { set; get; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [HideInPropertyGrid]
        public string AutoGenMVRefid { set; get; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyEditor(PropertyEditorType.ObjectSelector)]
        [OSE_ObjectTypes(EbObjectTypes.iWebForm)]
        [Alias("Web Form")]
        [PropertyGroup("Core")]
        public string WebFormRefId { set; get; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyEditor(PropertyEditorType.ObjectSelector)]
        [OSE_ObjectTypes(EbObjectTypes.iDataReader)]
        [Alias("Render Validator")]
        [PropertyGroup("Rendering")]
        public string RenderValidatorRefId { set; get; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [Alias("Message on Failed")]
        [PropertyGroup("Rendering")]
        public string MessageOnFailed { set; get; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyGroup("Rendering")]
        public string SubmitButtonText { set; get; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [HideInPropertyGrid]
        public List<Param> RenderValidatorParams { get; set; }

        public override string GetDesignHtml()
        {
            return @"<div class='eb_mob_form_container mob_container dropped' tabindex='1' eb-type='EbMobileForm' id='@id'>
                        <div class='eb_mob_container_inner'>
                        </div>
                    </div>".RemoveCR().DoubleQuoted();
        }

        public Dictionary<string, List<TableColumnMeta>> GetTableMetaCollection(IVendorDbTypes vDbTypes)
        {
            Dictionary<string, List<TableColumnMeta>> meta = new Dictionary<string, List<TableColumnMeta>>();
            try
            {
                meta.Add(this.TableName, new List<TableColumnMeta>());

                foreach (EbMobileControl ctrl in this.ChildControls)
                {
                    if (ctrl is EbMobileFileUpload)
                        continue;
                    else if (ctrl is EbMobileTableLayout tlayout)
                    {
                        foreach (EbMobileTableCell cell in tlayout.CellCollection)
                        {
                            foreach (EbMobileControl tctrl in cell.ControlCollection)
                            {
                                if (tctrl is EbMobileFileUpload)
                                    continue;
                                else
                                    AppendMeta(meta[this.TableName], tctrl, vDbTypes);
                            }
                        }
                    }
                    else if (ctrl is EbMobileDataGrid grid)
                    {
                        meta.Add(grid.TableName, new List<TableColumnMeta>());
                        meta[grid.TableName].Add(new TableColumnMeta
                        {
                            Name = this.TableName + "_id",
                            Type = vDbTypes.GetVendorDbTypeStruct(EbDbTypes.Int32)
                        });

                        foreach (EbMobileControl gctrl in grid.ChildControls)
                            AppendMeta(meta[grid.TableName], gctrl, vDbTypes);

                        AppendDefaultMeta(meta[grid.TableName], vDbTypes);
                    }
                    else
                        AppendMeta(meta[this.TableName], ctrl, vDbTypes);
                }
                AppendDefaultMeta(meta[this.TableName], vDbTypes);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return meta;
        }

        private void AppendMeta(List<TableColumnMeta> source, EbMobileControl control, IVendorDbTypes vDbTypes)
        {
            source.Add(new TableColumnMeta
            {
                Name = control.Name,
                Type = vDbTypes.GetVendorDbTypeStruct((EbDbTypes)control.EbDbType)
            });
        }

        private void AppendDefaultMeta(List<TableColumnMeta> metaList, IVendorDbTypes vDbTypes)
        {
            try
            {
                metaList.Add(new TableColumnMeta { Name = "eb_created_by", Type = vDbTypes.Decimal, Label = "Created By" });
                metaList.Add(new TableColumnMeta { Name = "eb_created_at", Type = vDbTypes.DateTime, Label = "Created At" });
                metaList.Add(new TableColumnMeta { Name = "eb_lastmodified_by", Type = vDbTypes.Decimal, Label = "Last Modified By" });
                metaList.Add(new TableColumnMeta { Name = "eb_lastmodified_at", Type = vDbTypes.DateTime, Label = "Last Modified At" });
                metaList.Add(new TableColumnMeta { Name = "eb_del", Type = vDbTypes.Boolean, Default = "F" });
                metaList.Add(new TableColumnMeta { Name = "eb_void", Type = vDbTypes.Boolean, Default = "F", Label = "Void ?" });
                metaList.Add(new TableColumnMeta { Name = "eb_loc_id", Type = vDbTypes.Int32, Label = "Location" });
                metaList.Add(new TableColumnMeta { Name = "eb_device_id", Type = vDbTypes.String, Label = "Device Id" });
                metaList.Add(new TableColumnMeta { Name = "eb_appversion", Type = vDbTypes.String, Label = "App Version" });
                metaList.Add(new TableColumnMeta { Name = "eb_created_at_device", Type = vDbTypes.DateTime, Label = "Sync Time" });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public override List<string> DiscoverRelatedRefids()
        {
            List<string> related = new List<string>();

            if (!string.IsNullOrEmpty(WebFormRefId))
                related.Add(WebFormRefId);

            if (!string.IsNullOrEmpty(AutoGenMVRefid))
                related.Add(AutoGenMVRefid);

            if (!string.IsNullOrEmpty(RenderValidatorRefId))
                related.Add(RenderValidatorRefId);

            foreach (EbMobileControl ctrl in this.ChildControls)
            {
                related.AddRange(ctrl.DiscoverRelatedRefids());
            }
            return related;
        }

        public override void ReplaceRefid(Dictionary<string, string> map)
        {
            if (!string.IsNullOrEmpty(WebFormRefId) && map.TryGetValue(WebFormRefId, out string wri))
                this.WebFormRefId = wri;

            if (!string.IsNullOrEmpty(AutoGenMVRefid) && map.TryGetValue(AutoGenMVRefid, out string agr))
                this.AutoGenMVRefid = agr;

            if (!string.IsNullOrEmpty(RenderValidatorRefId) && map.TryGetValue(RenderValidatorRefId, out string rev))
                this.RenderValidatorRefId = rev;

            foreach (EbMobileControl ctrl in this.ChildControls)
            {
                ctrl.ReplaceRefid(map);
            }
        }

        public EbMobileForm()
        {
            RenderValidatorParams = new List<Param>();
        }
    }
}
