using ExpressBase.Common;
using ExpressBase.Common.Extensions;
using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using ExpressBase.Common.Structures;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExpressBase.Objects
{
    [EnableInBuilder(BuilderType.MobilePage)]
    public class EbMobileContainer : EbMobilePageBase
    {
       
    }

    [EnableInBuilder(BuilderType.MobilePage)]
    public class EbMobileForm : EbMobileContainer
    {
        [EnableInBuilder(BuilderType.MobilePage)]
        [HideInPropertyGrid]
        public override string Name { set; get; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [HideInPropertyGrid]
        public List<EbMobileControl> ChiledControls { get; set; }

        [EnableInBuilder(BuilderType.MobilePage)]
        public string TableName { set; get; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [Alias("Auto Deploy Visualization")]
        public bool AutoDeployMV { set; get; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [HideInPropertyGrid]
        public string AutoGenMVRefid { set; get; }

        public override string GetDesignHtml()
        {
            return @"<div class='eb_mob_form_container mob_container dropped' eb-type='EbMobileForm' id='@id'>
                        <div class='eb_mob_container_inner'>
                        </div>
                    </div>".RemoveCR().DoubleQuoted();
        }

        public WebFormSchema ToWebFormSchema()
        {
            WebFormSchema Schema = new WebFormSchema
            {
                FormName = this.TableName
            };

            TableSchema TableSchema = new TableSchema
            {
                TableName = this.TableName,
                TableType = WebFormTableTypes.Normal
            };
            this.PushTableCols(TableSchema);

            Schema.Tables.Add(TableSchema);

            return Schema;
        }

        private void PushTableCols(TableSchema TableSchema)
        {
            foreach (EbMobileControl ctrl in this.ChiledControls)
            {
                TableSchema.Columns.Add(new ColumnSchema
                {
                    ColumnName = ctrl.Name,
                    EbDbType = (int)ctrl.EbDbType
                });
            }
        }
    }

    [EnableInBuilder(BuilderType.MobilePage)]
    public class EbMobileVisualization : EbMobileContainer
    {
        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyEditor(PropertyEditorType.ObjectSelector)]
        [OSE_ObjectTypes(EbObjectTypes.iDataReader)]
        public string DataSourceRefId { set; get; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [HideInPropertyGrid]
        public EbMobileTableLayout DataLayout { set; get; }

        public override string GetDesignHtml()
        {
            return @"<div class='eb_mob_vis_container mob_container dropped' eb-type='EbMobileVisualization' id='@id'>
                        <div class='eb_mob_container_inner'>
                        </div>
                    </div>".RemoveCR().DoubleQuoted();
        }
    }
}
