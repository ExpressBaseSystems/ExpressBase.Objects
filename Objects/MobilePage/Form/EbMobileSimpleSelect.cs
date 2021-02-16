using ExpressBase.Common.Constants;
using ExpressBase.Common.Data;
using ExpressBase.Common.Extensions;
using ExpressBase.Common.Objects;
using ExpressBase.Common.Objects.Attributes;
using ExpressBase.Common.Structures;
using ExpressBase.Objects.Objects.DVRelated;
using System;
using System.Collections.Generic;

namespace ExpressBase.Objects
{
    [EnableInBuilder(BuilderType.MobilePage)]
    public class EbMobileSimpleSelect : EbMobileControl
    {
        public override bool Unique { get; set; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [HideInPropertyGrid]
        public override EbDbTypes EbDbType
        {
            get
            {
                if (this.ValueMember != null)
                {
                    return this.ValueMember.Type;
                }
                else
                {
                    return EbDbTypes.String;
                }
            }
            set { }
        }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyEditor(PropertyEditorType.Collection)]
        [PropertyGroup(PGConstants.DATA)]
        public List<EbMobileSSOption> Options { set; get; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyEditor(PropertyEditorType.ObjectSelector)]
        [OSE_ObjectTypes(EbObjectTypes.iDataReader)]
        [PropertyGroup(PGConstants.DATA)]
        [OnChangeExec(@"
                if (this.DataSourceRefId !== null && this.DataSourceRefId !== ''){ 
                        pg.ShowProperty('DisplayMember');
                        pg.ShowProperty('ValueMember');
                        pg.ShowProperty('OfflineQuery');
                        pg.ShowProperty('EnablePreload');
                        pg.ShowProperty('AutoFill');
                }
                else {
                        pg.HideProperty('DisplayMember');
                        pg.HideProperty('ValueMember');
                        pg.HideProperty('OfflineQuery');
                        pg.HideProperty('EnablePreload');
                        pg.HideProperty('AutoFill');
                }
            ")]
        public string DataSourceRefId { get; set; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [MetaOnly]
        public List<EbMobileDataColumn> Columns { set; get; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyEditor(PropertyEditorType.CollectionFrmSrc, "Columns", 1)]
        [PropertyGroup(PGConstants.DATA)]
        public EbMobileDataColumn DisplayMember { set; get; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyEditor(PropertyEditorType.CollectionFrmSrc, "Columns", 1)]
        [PropertyGroup(PGConstants.DATA)]
        public EbMobileDataColumn ValueMember { set; get; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyGroup(PGConstants.CORE)]
        public int MinSearchLength { set; get; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyEditor(PropertyEditorType.ScriptEditorCS)]
        [HelpText("sql query to get data from offline database")]
        [PropertyGroup(PGConstants.DATA)]
        public EbScript OfflineQuery { set; get; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [HideInPropertyGrid]
        public List<Param> Parameters { set; get; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyGroup(PGConstants.CORE)]
        public bool MultiSelect { get; set; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyGroup(PGConstants.CORE)]
        public bool EnablePreload { get; set; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyGroup(PGConstants.CORE)]
        public bool AutoFill { get; set; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [HideInPropertyGrid]
        public override string Icon { get { return "fa-caret-down"; } }

        private bool IsSimpleSelect => string.IsNullOrEmpty(this.DataSourceRefId);

        public override string GetDesignHtml()
        {
            return @"<div class='eb_stacklayout mob_control dropped' id='@id' eb-type='EbMobileSimpleSelect' tabindex='1' onclick='$(this).focus()'>
                            <label class='ctrl_label'> @Label </label>
                            <div class='eb_ctrlhtml'>
                               <select class='eb_mob_select'>
                                    <option>--select--</option>
                                </select>
                            </div>
                        </div>".RemoveCR().DoubleQuoted();
        }

        public override string GetJsInitFunc()
        {
            return @"
                this.Init = function(id)
                    {
                        this.MinSearchLength = 3;
                    };";
        }

        public EbMobileSimpleSelect()
        {
            Parameters = new List<Param>();
        }

        public override List<string> DiscoverRelatedRefids()
        {
            List<string> list = new List<string>();

            if (!string.IsNullOrEmpty(DataSourceRefId))
                list.Add(DataSourceRefId);

            return list;
        }

        public override void ReplaceRefid(Dictionary<string, string> map)
        {
            if (!string.IsNullOrEmpty(DataSourceRefId) && map.TryGetValue(DataSourceRefId, out string dsri))
                DataSourceRefId = dsri;
        }

        private void SetSimpleSelectOptions(EbSimpleSelect simpleSelect)
        {
            simpleSelect.Options.Clear();

            foreach (EbMobileSSOption so in this.Options)
            {
                simpleSelect.Options.Add(new EbSimpleSelectOption
                {
                    EbSid = "ss_options_" + Guid.NewGuid().ToString("N"),
                    Name = so.Name,
                    Value = so.Value,
                    DisplayName = so.DisplayName
                });
            }
        }

        private void SetValueMember(EbPowerSelect powerselect)
        {
            if (this.ValueMember != null)
            {
                powerselect.ValueMember = new DVBaseColumn
                {
                    EbSid = "basecol_" + Guid.NewGuid().ToString("N"),
                    Name = this.ValueMember.Name,
                    Type = this.ValueMember.Type,
                    sTitle = this.ValueMember.Name,
                    Data = this.ValueMember.ColumnIndex
                };
            }
        }

        private void SetDisplayMember(EbPowerSelect powerselect)
        {
            if (this.DisplayMember != null)
            {
                powerselect.DisplayMember = new DVBaseColumn
                {
                    EbSid = "basecol_" + Guid.NewGuid().ToString("N"),
                    Name = this.DisplayMember.Name,
                    Type = this.DisplayMember.Type,
                    sTitle = this.DisplayMember.Name,
                    Data = this.DisplayMember.ColumnIndex
                };
            }
        }

        public override string EbControlType => IsSimpleSelect ? nameof(EbSimpleSelect) : nameof(EbPowerSelect);

        public override EbControl GetWebFormControl(int counter)
        {
            EbControl ctrl;
            string ebSid = this.EbControlType + counter;

            if (IsSimpleSelect)
            {
                EbSimpleSelect simpleSelect = new EbSimpleSelect
                {
                    EbSid = ebSid,
                    Name = this.Name,
                    Margin = new UISides { Top = 0, Bottom = 0, Left = 0, Right = 0 },
                    Label = this.Label
                };
                ctrl = simpleSelect;
                this.SetSimpleSelectOptions(simpleSelect);
            }
            else
            {
                EbPowerSelect powerselect = new EbPowerSelect
                {
                    EbSid = ebSid,
                    DataSourceId = this.DataSourceRefId,
                    Name = this.Name,
                    Margin = new UISides { Top = 0, Bottom = 0, Left = 0, Right = 0 },
                    Label = this.Label
                };
                ctrl = powerselect;
                this.SetDisplayMember(powerselect);
                this.SetValueMember(powerselect);
            }
            return ctrl;
        }

        public override void UpdateWebFormControl(EbControl control)
        {
            if (control != null)
            {
                base.UpdateWebFormControl(control);

                if (control is EbSimpleSelect simpleSelect)
                {
                    this.SetSimpleSelectOptions(simpleSelect);
                }
                else if (control is EbPowerSelect powerSelect)
                {
                    powerSelect.DataSourceId = this.DataSourceRefId;
                    this.SetDisplayMember(powerSelect);
                    this.SetValueMember(powerSelect);
                }
            }
        }
    }

    [EnableInBuilder(BuilderType.MobilePage)]
    public class EbMobileSSOption : EbMobilePageBase
    {
        [HideInPropertyGrid]
        [EnableInBuilder(BuilderType.MobilePage)]
        public string EbSid { get; set; }

        [EnableInBuilder(BuilderType.MobilePage)]
        public override string DisplayName { set; get; }

        [EnableInBuilder(BuilderType.MobilePage)]
        public string Value { set; get; }
    }
}
