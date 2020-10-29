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
    public class EbMobileTextBox : EbMobileControl
    {
        [EnableInBuilder(BuilderType.MobilePage)]
        [HideInPropertyGrid]
        public override EbDbTypes EbDbType { get { return EbDbTypes.String; } set { } }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyGroup(PGConstants.EXTENDED)]
        public int MaxLength { get; set; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyGroup(PGConstants.EXTENDED)]
        public bool AutoSuggestion { get; set; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyGroup(PGConstants.CORE)]
        public TextTransform TextTransform { get; set; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyGroup(PGConstants.CORE)]
        public TextMode TextMode { get; set; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyGroup(PGConstants.BEHAVIOR)]
        public bool AutoCompleteOff { get; set; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [HideInPropertyGrid]
        public override string Icon { get { return "fa-i-cursor"; } }

        public override string GetDesignHtml()
        {
            return @"<div class='eb_stacklayout mob_control dropped' id='@id' eb-type='EbMobileTextBox' tabindex='1' onclick='$(this).focus()'>
                            <label class='ctrl_label'> @Label </label>
                            <div class='eb_ctrlhtml'>
                               <input type='text' class='eb_mob_textbox' />
                            </div>
                        </div>".RemoveCR().DoubleQuoted();
        }

        public override EbControl GetWebFormControl(int counter)
        {
            return new EbTextBox
            {
                EbSid = this.EbControlType + counter,
                Name = this.Name,
                MaxLength = this.MaxLength,
                TextTransform = this.TextTransform,
                TextMode = this.TextMode,
                AutoCompleteOff = this.AutoCompleteOff,
                AutoSuggestion = this.AutoSuggestion,
                Margin = new UISides { Top = 0, Bottom = 0, Left = 0, Right = 0 },
                Label = this.Label
            };
        }

        public override void UpdateWebFormControl(EbControl control)
        {
            if (control == null || !(control is EbTextBox textBox))
                return;

            base.UpdateWebFormControl(control);

            textBox.MaxLength = this.MaxLength;
            textBox.TextTransform = this.TextTransform;
            textBox.TextMode = this.TextMode;
            textBox.AutoCompleteOff = this.AutoCompleteOff;
            textBox.AutoSuggestion = this.AutoSuggestion;
        }
    }

    [EnableInBuilder(BuilderType.MobilePage)]
    public class EbMobileNumericBox : EbMobileControl
    {
        [EnableInBuilder(BuilderType.MobilePage)]
        [HideInPropertyGrid]
        public override EbDbTypes EbDbType { get { return EbDbTypes.Decimal; } set { } }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyGroup(PGConstants.EXTENDED)]
        [DefaultPropValue("2")]
        [HelpText("Number of decimal places")]
        public int DecimalPlaces { get; set; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyGroup(PGConstants.VALIDATIONS)]
        [Alias("Maximum")]
        [HelpText("Maximum value allowed")]
        public int MaxLimit { get; set; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyGroup(PGConstants.VALIDATIONS)]
        [Alias("Minimum")]
        [HelpText("Minimum value allowed")]
        public int MinLimit { get; set; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyGroup("Behavior")]
        public NumericBoxTypes RenderType { get; set; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [HideInPropertyGrid]
        public override string Icon { get { return "fa-i-cursor"; } }

        public override string GetDesignHtml()
        {
            return @"<div class='eb_stacklayout mob_control dropped' id='@id' eb-type='EbMobileNumericBox' tabindex='1' onclick='$(this).focus()'>
                            <label class='ctrl_label'> @Label </label>
                            <div class='eb_ctrlhtml'>
                                <input type='number' class='eb_mob_numericbox' />
                                <div class='eb_mob_numericbox-btntype'>
                                    <div class='wraper'>
                                        <button class='numeric-btn'><i class='fa fa-minus'></i></button>
                                        <div class='nemric-text'>
                                            <input type='text'/>
                                        </div>
                                        <button class='numeric-btn'><i class='fa fa-plus'></i></button>
                                    </div>
                                </div>
                            </div>
                        </div>".RemoveCR().DoubleQuoted();
        }

        public override string EbControlType => nameof(EbNumeric);

        public override EbControl GetWebFormControl(int counter)
        {
            return new EbNumeric
            {
                EbSid = this.EbControlType + counter,
                Name = this.Name,
                DecimalPlaces = this.DecimalPlaces,
                MaxLimit = this.MaxLimit,
                MinLimit = this.MinLimit,
                Margin = new UISides { Top = 0, Bottom = 0, Left = 0, Right = 0 },
                Label = this.Label
            };
        }

        public override void UpdateWebFormControl(EbControl control)
        {
            if (control == null || !(control is EbNumeric numeric))
                return;

            base.UpdateWebFormControl(control);

            numeric.MaxLimit = this.MaxLimit;
            numeric.MinLimit = this.MinLimit;
        }
    }

    [EnableInBuilder(BuilderType.MobilePage)]
    public class EbMobileDateTime : EbMobileControl
    {
        [EnableInBuilder(BuilderType.MobilePage)]
        [HideInPropertyGrid]
        public override EbDbTypes EbDbType { get { return (EbDbTypes)this.EbDateType; } set { } }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyGroup(PGConstants.CORE)]
        public EbDateType EbDateType { get; set; } = EbDateType.DateTime;

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyGroup(PGConstants.EXTENDED)]
        public bool IsNullable { get; set; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyGroup(PGConstants.EXTENDED)]
        public bool BlockBackDatedEntry { set; get; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyGroup(PGConstants.EXTENDED)]
        public bool BlockFutureDatedEntry { set; get; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [HideInPropertyGrid]
        public override string Icon { get { return "fa-calendar"; } }

        public override string GetDesignHtml()
        {
            return @"<div class='eb_stacklayout mob_control dropped' id='@id' eb-type='EbMobileDateTime' tabindex='1' onclick='$(this).focus()'>
                            <label class='ctrl_label'> @Label </label>
                            <div class='eb_ctrlhtml'>
                               <input type='text' class='eb_mob_textbox' placeholder='YYYY-MM-DD'/>
                            </div>
                        </div>".RemoveCR().DoubleQuoted();
        }

        public override string EbControlType => nameof(EbDate);

        public override EbControl GetWebFormControl(int counter)
        {
            return new EbDate
            {
                EbSid = this.EbControlType + counter,
                Name = this.Name,
                IsNullable = this.IsNullable,
                EbDateType = this.EbDateType,
                Margin = new UISides { Top = 0, Bottom = 0, Left = 0, Right = 0 },
                Label = this.Label
            };
        }

        public override void UpdateWebFormControl(EbControl control)
        {
            if (control == null || !(control is EbDate date))
                return;

            base.UpdateWebFormControl(control);

            date.IsNullable = this.IsNullable;
            date.EbDateType = this.EbDateType;
        }
    }

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
                }
                else {
                        pg.HideProperty('DisplayMember');
                        pg.HideProperty('ValueMember');
                        pg.HideProperty('OfflineQuery');
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
            if (control == null || !(control is EbPowerSelect powerSelect) || !(control is EbSimpleSelect simpleSelect))
                return;

            base.UpdateWebFormControl(control);

            if (IsSimpleSelect)
            {
                this.SetSimpleSelectOptions(simpleSelect);
            }
            else
            {
                powerSelect.DataSourceId = this.DataSourceRefId;
                this.SetDisplayMember(powerSelect);
                this.SetValueMember(powerSelect);
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

    [EnableInBuilder(BuilderType.MobilePage)]
    public class EbMobileFileUpload : EbMobileControl, INonPersistControl
    {
        public override bool DoNotPersist { get; set; }
        public override bool Unique { get; set; }
        public override EbScript ValueExpr { get; set; }
        public override EbScript DefaultValueExpression { get; set; }
        public override List<EbMobileValidator> Validators { set; get; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyGroup("General")]
        public bool EnableCameraSelect { set; get; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyGroup("General")]
        public bool EnableFileSelect { set; get; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyGroup("General")]
        public virtual bool MultiSelect { set; get; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyGroup("General")]
        public virtual bool EnableEdit { set; get; }

        public override string GetDesignHtml()
        {
            return @"<div class='eb_stacklayout mob_control dropped' id='@id' eb-type='EbMobileFileUpload' tabindex='1' onclick='$(this).focus()'>
                            <label class='ctrl_label'> @Label </label>
                            <div class='eb_ctrlhtml'>
                               <button class='eb_mob_fupbtn filesbtn'><i class='fa fa-folder-open-o'></i></button>
                               <button class='eb_mob_fupbtn camerabtn'><i class='fa fa-camera'></i></button>
                            </div>
                        </div>".RemoveCR().DoubleQuoted();
        }

        public override string GetJsInitFunc()
        {
            return @"
                this.Init = function(id)
                {
                    this.EnableCameraSelect = true;
                    this.EnableFileSelect= true;
                    this.MultiSelect= true;
                    this.EnableEdit= true;
                };";
        }

        public override EbControl GetWebFormControl(int counter)
        {
            return new EbFileUploader
            {
                EbSid = this.EbControlType + counter,
                Name = this.Name,
                IsMultipleUpload = this.MultiSelect,
                Margin = new UISides { Top = 0, Bottom = 0, Left = 0, Right = 0 },
                Label = this.Label
            };
        }

        public override void UpdateWebFormControl(EbControl control)
        {
            if (control == null || !(control is EbFileUploader fup))
                return;

            base.UpdateWebFormControl(control);

            fup.IsMultipleUpload = this.MultiSelect;
        }
    }

    [EnableInBuilder(BuilderType.MobilePage)]
    public class EbMobileBoolean : EbMobileControl
    {
        [EnableInBuilder(BuilderType.MobilePage)]
        [HideInPropertyGrid]
        public override EbDbTypes EbDbType { get { return EbDbTypes.BooleanOriginal; } set { } }

        [EnableInBuilder(BuilderType.MobilePage)]
        [HideInPropertyGrid]
        public override string Icon { get { return "fa-toggle-on"; } }

        public override string GetDesignHtml()
        {
            return @"<div class='eb_stacklayout mob_control dropped' id='@id' eb-type='EbMobileBoolean' tabindex='1' onclick='$(this).focus()'>
                            <label class='ctrl_label'> @Label </label>
                            <div class='eb_ctrlhtml'>
                               <input type='checkbox' class='eb_mob_checkbox'/>
                            </div>
                        </div>".RemoveCR().DoubleQuoted();
        }

        public override string EbControlType => nameof(EbBooleanSelect);

        public override EbControl GetWebFormControl(int counter)
        {
            return new EbBooleanSelect
            {
                EbSid = this.EbControlType + counter,
                Name = this.Name,
                Margin = new UISides { Top = 0, Bottom = 0, Left = 0, Right = 0 },
                Label = this.Label
            };
        }

        public override void UpdateWebFormControl(EbControl control)
        {
            if (control == null || !(control is EbBooleanSelect boolean))
                return;

            base.UpdateWebFormControl(control);
        }
    }

    [EnableInBuilder(BuilderType.MobilePage)]
    public class EbMobileGeoLocation : EbMobileControl
    {
        [EnableInBuilder(BuilderType.MobilePage)]
        [HideInPropertyGrid]
        public override EbDbTypes EbDbType { get { return EbDbTypes.String; } set { } }

        [EnableInBuilder(BuilderType.MobilePage)]
        public bool HideSearchBox { set; get; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [HideInPropertyGrid]
        public override string Icon { get { return "fa-map-marker"; } }

        public override EbScript ValueExpr { get; set; }
        public override EbScript DefaultValueExpression { get; set; }
        public override List<EbMobileValidator> Validators { set; get; }

        public override string GetDesignHtml()
        {
            return @"<div class='eb_stacklayout mob_control dropped' id='@id' eb-type='EbMobileGeoLocation' tabindex='1' onclick='$(this).focus()'>
                            <label class='ctrl_label'> @Label </label>
                            <div class='eb_ctrlhtml'>
                               <div class='geoloc-ctrlwrapr'>
                                    <input type='text' style='display: @display' placeholder='Search place' class='eb_mob_textbox' />
                                    <div class='map-container'>
                                        
                                    </div>
                               </div>
                            </div>
                        </div>".Replace("@display", (this.HideSearchBox) ? "none" : "block").RemoveCR().DoubleQuoted();
        }

        public override string EbControlType => nameof(EbInputGeoLocation);

        public override EbControl GetWebFormControl(int counter)
        {
            return new EbInputGeoLocation
            {
                EbSid = this.EbControlType + counter,
                Name = this.Name,
                Margin = new UISides { Top = 0, Bottom = 0, Left = 0, Right = 0 },
                Label = this.Label
            };
        }

        public override void UpdateWebFormControl(EbControl control)
        {
            if (control == null || !(control is EbInputGeoLocation geo))
                return;

            base.UpdateWebFormControl(control);
        }
    }

    [EnableInBuilder(BuilderType.MobilePage)]
    public class EbMobileAutoId : EbMobileControl
    {
        public override bool DoNotPersist { get; set; }
        public override bool Unique { get; set; }
        public override bool Required { get; set; }
        public override bool ReadOnly { get { return true; } }
        public override EbScript ValueExpr { get; set; }
        public override EbScript HiddenExpr { get; set; }
        public override EbScript DisableExpr { get; set; }
        public override EbScript DefaultValueExpression { get; set; }
        public override List<EbMobileValidator> Validators { set; get; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [HideInPropertyGrid]
        public override EbDbTypes EbDbType { get { return EbDbTypes.String; } set { } }

        [EnableInBuilder(BuilderType.MobilePage)]
        [HideInPropertyGrid]
        public override string Icon { get { return "fa-key"; } }

        public override string GetDesignHtml()
        {
            return @"<div class='eb_stacklayout mob_control dropped' id='@id' eb-type='EbMobileAutoId' tabindex='1' onclick='$(this).focus()'>
                            <label class='ctrl_label'> @Label </label>
                            <div class='eb_ctrlhtml'>
                               <input type='text' class='eb_mob_autoid' />
                            </div>
                        </div>".RemoveCR().DoubleQuoted();
        }

        public override EbControl GetWebFormControl(int counter)
        {
            return new EbAutoId
            {
                EbSid = this.EbControlType + counter,
                Name = this.Name,
                Margin = new UISides { Top = 0, Bottom = 0, Left = 0, Right = 0 },
                Label = this.Label
            };
        }

        public override void UpdateWebFormControl(EbControl control)
        {
            if (control == null || !(control is EbAutoId auto))
                return;

            base.UpdateWebFormControl(control);
        }
    }

    [EnableInBuilder(BuilderType.MobilePage)]
    public class EbMobileDisplayPicture : EbMobileFileUpload
    {
        public override bool MultiSelect => false;

        public override bool EnableEdit { set; get; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [HideInPropertyGrid]
        public override EbDbTypes EbDbType { get { return EbDbTypes.String; } set { } }

        public override string GetDesignHtml()
        {
            return @"<div class='eb_stacklayout mob_control dropped' id='@id' eb-type='EbMobileDisplayPicture' tabindex='1' onclick='$(this).focus()'>
                            <label class='ctrl_label'> @Label </label>
                            <div class='eb_ctrlhtml'>
                                <div class='dp-avatar'>
                                    <img src='/images/image.png'/>
                                </div>
                                <div class='dp-btn-container'>
                                    <button class='eb_mob_fupbtn filesbtn'><i class='fa fa-folder-open-o'></i></button>
                                    <button class='eb_mob_fupbtn camerabtn'><i class='fa fa-camera'></i></button>
                                </div>
                            </div>
                        </div>".RemoveCR().DoubleQuoted();
        }

        public override string GetJsInitFunc()
        {
            return @"
                this.Init = function(id)
                {
                    this.EnableCameraSelect = true;
                    this.EnableFileSelect= true;
                };";
        }

        public override EbControl GetWebFormControl(int counter)
        {
            return new EbDisplayPicture
            {
                EbSid = this.EbControlType + counter,
                Name = this.Name,
                Margin = new UISides { Top = 0, Bottom = 0, Left = 0, Right = 0 },
                Label = this.Label
            };
        }

        public override void UpdateWebFormControl(EbControl control)
        {
            if (control == null || !(control is EbDisplayPicture dp))
                return;

            base.UpdateWebFormControl(control);
        }
    }

    [EnableInBuilder(BuilderType.MobilePage)]
    public class EbMobileQrReader : EbMobileControl
    {
        [EnableInBuilder(BuilderType.MobilePage)]
        [HideInPropertyGrid]
        public override EbDbTypes EbDbType { get { return EbDbTypes.String; } set { } }

        public override EbScript ValueExpr { get; set; }
        public override EbScript DefaultValueExpression { get; set; }
        public override List<EbMobileValidator> Validators { set; get; }

        public override string GetDesignHtml()
        {
            return @"<div class='eb_stacklayout mob_control dropped' id='@id' eb-type='EbMobileQrReader' tabindex='1' onclick='$(this).focus()'>
                            <label class='ctrl_label'> @Label </label>
                            <div class='eb_ctrlhtml'>
                               <div class='ctrl-group eb-mob-qrreader'>
                                    <input type='text'/><span class='fa fa-qrcode'></span>
                                </div>
                            </div>
                        </div>".RemoveCR().DoubleQuoted();
        }

        public override string EbControlType => nameof(EbTextBox);

        public override EbControl GetWebFormControl(int counter)
        {
            return new EbTextBox
            {
                EbSid = this.EbControlType + counter,
                Name = this.Name,
                Margin = new UISides { Top = 0, Bottom = 0, Left = 0, Right = 0 },
                Label = this.Label
            };
        }

        public override void UpdateWebFormControl(EbControl control)
        {
            if (control == null || !(control is EbTextBox qr))
                return;

            base.UpdateWebFormControl(control);
        }
    }

    public class EbMobileRating : EbMobileControl
    {
        public override EbDbTypes EbDbType { get { return EbDbTypes.Decimal; } set { } }

        [EnableInBuilder(BuilderType.MobilePage)]
        [Alias("Maximum Star")]
        [DefaultPropValue("5")]
        public int MaxValue { get; set; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [PropertyEditor(PropertyEditorType.Color)]
        [Alias("Selection Color")]
        [DefaultPropValue("#F39C12")]
        public string SelectionColor { get; set; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [DefaultPropValue("8")]
        [Alias("Spacing")]
        public int Spacing { get; set; }

        [EnableInBuilder(BuilderType.MobilePage)]
        [HideInPropertyGrid]
        public override string Icon { get { return "fa-i-cursor"; } }

        public override string GetDesignHtml()
        {
            return @"<div class='eb_stacklayout mob_control dropped' id='@id' eb-type='EbMobileRating' tabindex='1' onclick='$(this).focus()'>
                            <label class='ctrl_label'> @Label </label>
                            <div class='eb_ctrlhtml'>
                               <span class='fa fa-star-o wrd_spacing'></span>
	                            <span class='fa fa-star-o wrd_spacing'></span>
	                            <span class='fa fa-star-o wrd_spacing'></span>
	                            <span class='fa fa-star-o wrd_spacing'></span>
	                            <span class='fa fa-star-o wrd_spacing'></span>
                            </div>
                        </div>".RemoveCR().DoubleQuoted();
        }
        public override EbControl GetWebFormControl(int counter)
        {
            return new EbRating
            {
                EbSid = this.EbControlType + counter,
                Name = this.Name,
                MaxVal = this.MaxValue,
                RatingColor = this.SelectionColor,
                Spacing = this.Spacing,
                Margin = new UISides { Top = 0, Bottom = 0, Left = 0, Right = 0 },
                Label = this.Label
            };
        }

        public override void UpdateWebFormControl(EbControl control)
        {
            if (control == null || !(control is EbRating rating))
                return;

            base.UpdateWebFormControl(control);

            rating.RatingColor = this.SelectionColor;
            rating.Spacing = this.Spacing;
            rating.MaxVal = this.MaxValue;
        }
    }
}
