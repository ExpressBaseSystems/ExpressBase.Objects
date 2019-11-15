using ExpressBase.Common;
using ExpressBase.Common.Extensions;
using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExpressBase.Objects
{
    public abstract class EbMobilePageBase : EbObject
    {

    }

    [EnableInBuilder(BuilderType.MobilePage)]
    [BuilderTypeEnum(BuilderType.MobilePage)]
    public class EbMobilePage : EbMobilePageBase, IEBRootObject
    {
        [EnableInBuilder(BuilderType.MobilePage)]
        [HideInPropertyGrid]
        public override string RefId { get; set; }

        [EnableInBuilder(BuilderType.MobilePage)]
        public override string DisplayName { get; set; }

        [EnableInBuilder(BuilderType.MobilePage)]
        public override string Description { get; set; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [HideInPropertyGrid]
        public override string VersionNumber { get; set; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [HideInPropertyGrid]
        public override string Status { get; set; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [HideInPropertyGrid]
        public EbMobileContainer Container { set; get; }

        public WebFormSchema ToWebFormSchema()
        {
            if (this.Container == null && !(this.Container is EbMobileForm))
                return null;

            WebFormSchema Schema = new WebFormSchema
            {
                FormName = this.Container.Name
            };

            TableSchema TableSchema = new TableSchema
            {
                TableName = this.Container.Name,
                TableType = WebFormTableTypes.Normal
            };
            Schema.Tables.Add(TableSchema);

            return Schema;
        }

        private void PushTableCols(TableSchema TableSchema)
        {
            EbMobileForm MobForm = this.Container as EbMobileForm;

            foreach (EbMobileControl ctrl in MobForm.ChiledControls)
            {
                TableSchema.Columns.Add(new ColumnSchema
                {
                    ColumnName = ctrl.Name,
                    EbDbType = (int)ctrl.EbDbType
                });
            }
        }
    }
}
