using ExpressBase.Common.Extensions;
using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using ExpressBase.Common.Structures;
using ExpressBase.Objects.Objects.DVRelated;
using System.Collections.Generic;
using System.Linq;

namespace ExpressBase.Objects
{
    [EnableInBuilder(BuilderType.MobilePage)]
    public class EbMobileDataGrid : EbMobileControl, ILinesEnabled
    {
        public override bool DoNotPersist { get; set; }

        public override bool Unique { get; set; }

        public override bool Required { get; set; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [HideInPropertyGrid]
        public List<EbMobileControl> ChildControls { set; get; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [HideInPropertyGrid]
        public EbMobileTableLayout DataLayout { set; get; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyGroup("Data")]
        public string TableName { set; get; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyEditor(PropertyEditorType.ObjectSelector)]
        [OSE_ObjectTypes(EbObjectTypes.iDataReader)]
        [Alias("Data Source")]
        [PropertyGroup("Data")]
        public string DataSourceRefId { set; get; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyEditor(PropertyEditorType.ScriptEditorSQ)]
        [HelpText("sql query to get data from offline database")]
        [Alias("Offline Query")]
        [PropertyGroup("Data")]
        public EbScript OfflineQuery { set; get; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyGroup("Behavior")]
        public bool DisableAdd { set; get; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyGroup("Behavior")]
        public bool DisableDelete { set; get; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyGroup("Behavior")]
        public bool DisableEdit { set; get; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyEditor(PropertyEditorType.ScriptEditorCS)]
        [PropertyGroup("Appearance")]
        [Alias("Row Color Expression")]
        public EbScript RowColorExpr { get; set; }

        public override EbScript ValueExpr { get => base.ValueExpr; set => base.ValueExpr = value; }

        public override string GetDesignHtml()
        {
            return @"<div class='eb_stacklayout mob_control dropped' id='@id' eb-type='EbMobileDataGrid' tabindex='1' onclick='$(this).focus()'>
                            <label class='ctrl_label'> @Label </label>
                            <div class='eb_ctrlhtml ctrl_as_container' style='padding:10px 0;'>
                               <div class='data_layout'></div>
                               <div class='control_container'></div>
                            </div>
                        </div>".RemoveCR().DoubleQuoted();
        }

        public EbDGColumn GetGridControl(EbMobileControl ctrl)
        {
            EbDGColumn dgColumn = null;
            switch (ctrl)
            {
                case EbMobileTextBox tx:
                    dgColumn = new EbDGStringColumn();
                    break;
                case EbMobileNumericBox txn:
                    dgColumn = new EbDGNumericColumn();
                    break;
                case EbMobileBoolean bl:
                    dgColumn = new EbDGBooleanColumn();
                    break;
                case EbMobileDateTime dt:
                    dgColumn = new EbDGDateColumn
                    {
                        EbDateType = dt.EbDateType
                    };
                    break;
                case EbMobileSimpleSelect ss:
                    if (string.IsNullOrEmpty(ss.DataSourceRefId))
                    {
                        dgColumn = new EbDGSimpleSelectColumn
                        {
                            Options = ss.Options.Select(item => new EbSimpleSelectOption
                            {
                                EbSid = item.EbSid,
                                Name = item.Name,
                                Value = item.Value,
                                DisplayName = item.DisplayName
                            }).ToList()
                        };
                    }
                    else
                    {
                        dgColumn = new EbDGPowerSelectColumn
                        {
                            DataSourceId = ss.DataSourceRefId,
                            ValueMember = new DVBaseColumn
                            {
                                Name = ss.ValueMember.Name,
                                Type = ss.ValueMember.Type,
                                sTitle = ss.ValueMember.Name,
                                Data = ss.ValueMember.ColumnIndex
                            },
                            DisplayMember = new DVBaseColumn
                            {
                                Name = ss.DisplayMember.Name,
                                Type = ss.DisplayMember.Type,
                                sTitle = ss.DisplayMember.Name,
                                Data = ss.DisplayMember.ColumnIndex
                            }
                        };
                    }
                    break;
                default:
                    dgColumn = new EbDGStringColumn();
                    break;
            }
            dgColumn.Title = ctrl.Label;
            dgColumn.Name = ctrl.Name;
            dgColumn.EbSid = ctrl.GetType().Name + "0";
            return dgColumn;
        }

        public override EbControl GetWebFormControl(int count)
        {
            EbDataGrid dg = new EbDataGrid
            {
                EbSid = this.EbControlType + count,
                Name = this.Name,
                Margin = new UISides { Top = 0, Bottom = 0, Left = 0, Right = 0 },
                Label = this.Label,
                TableName = this.TableName
            };

            foreach (EbMobileControl ctrl in this.ChildControls)
                dg.Controls.Add(this.GetGridControl(ctrl));

            return dg;
        }

        public override List<string> DiscoverRelatedRefids()
        {
            List<string> list = new List<string>();

            if (!string.IsNullOrEmpty(DataSourceRefId))
                list.Add(DataSourceRefId);

            foreach (var ctrl in this.ChildControls)
            {
                list.AddRange(ctrl.DiscoverRelatedRefids());
            }
            return list;
        }

        public override void ReplaceRefid(Dictionary<string, string> map)
        {
            if (!string.IsNullOrEmpty(DataSourceRefId) && map.TryGetValue(DataSourceRefId, out string dsri))
                DataSourceRefId = dsri;

            foreach (var ctrl in this.ChildControls)
            {
                ctrl.ReplaceRefid(map);
            }
        }
    }
}
